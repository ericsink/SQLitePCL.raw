/*
   Copyright 2014-2022 SourceGear, LLC

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

public enum TFM
{
    NONE,
    UWP,
    NETSTANDARD20,
    NET461,
    NET60,
    NET70,
    NET80,
    MACCATALYST,
    XAMARINMAC20,
}

public static class common
{
    public const string ROOT_NAME = "SQLitePCLRaw";

    public static string AsString(this TFM e)
    {
        switch (e)
        {
            case TFM.NONE: throw new Exception("TFM.NONE.AsString()");
            case TFM.UWP: return "uap10.0";
            case TFM.NETSTANDARD20: return "netstandard2.0";
            case TFM.NET461: return "net461";
            case TFM.NET60: return "net6.0";
            case TFM.NET70: return "net7.0";
            case TFM.NET80: return "net8.0";
            case TFM.MACCATALYST: return "net6.0-maccatalyst15.2";
            case TFM.XAMARINMAC20: return "xamarin.mac20";
            default:
                throw new NotImplementedException(string.Format("TFM.AsString for {0}", e));
        }
    }

    public static void write_nuspec_file_entry(string src, string target, XmlWriter f)
    {
        f.WriteStartElement("file");
        f.WriteAttributeString("src", src);
        f.WriteAttributeString("target", target);
        f.WriteEndElement(); // file
    }

    public static void write_empty(XmlWriter f, TFM tfm)
    {
        f.WriteComment("empty directory in lib to avoid nuget adding a reference");

        f.WriteStartElement("file");
        f.WriteAttributeString("src", "_._");
        f.WriteAttributeString("target", string.Format("lib/{0}/_._", tfm.AsString()));
        f.WriteEndElement(); // file
    }

    const string PACKAGE_TAGS = "sqlite;xamarin";

    public static void write_nuspec_common_metadata(
        string id,
        XmlWriter f
        )
    {
        f.WriteAttributeString("minClientVersion", "2.12"); // TODO not sure this is right

        f.WriteElementString("id", id);
        f.WriteElementString("title", id);
        f.WriteElementString("version", "$version$");
        f.WriteElementString("authors", "$authors$");
        f.WriteElementString("copyright", "$copyright$");
        f.WriteElementString("requireLicenseAcceptance", "false");
        write_license(f);
        f.WriteStartElement("repository");
        f.WriteAttributeString("type", "git");
        f.WriteAttributeString("url", "https://github.com/ericsink/SQLitePCL.raw");
        f.WriteEndElement(); // repository
        f.WriteElementString("summary", "$summary$");
        f.WriteElementString("tags", PACKAGE_TAGS);
    }

    public static XmlWriterSettings XmlWriterSettings_default()
    {
        var settings = new XmlWriterSettings();
        settings.NewLineChars = "\n";
        settings.Indent = true;
        return settings;
    }

    public static void gen_dummy_csproj(string dir_proj, string id)
    {
        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = true;

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, $"{id}.csproj"), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("Project");
            f.WriteAttributeString("Sdk", "Microsoft.NET.Sdk");

            f.WriteStartElement("PropertyGroup");

            f.WriteElementString("TargetFramework", "netstandard2.0");
            f.WriteElementString("NoBuild", "true");
            f.WriteElementString("IncludeBuildOutput", "false");
            f.WriteElementString("NuspecFile", $"{id}.nuspec");
            f.WriteElementString("NuspecProperties", "version=$(version);src_path=$(src_path);cb_bin_path=$(cb_bin_path);authors=$(Authors);copyright=$(Copyright);summary=$(Description)");

            f.WriteEndElement(); // PropertyGroup

            f.WriteEndElement(); // Project

            f.WriteEndDocument();
        }
    }

    private static void write_license(XmlWriter f)
    {
        f.WriteStartElement("license");
        f.WriteAttributeString("type", "expression");
        f.WriteString("Apache-2.0");
        f.WriteEndElement();
    }

}

public static class gen
{
    enum LibSuffix
    {
        DLL,
        DYLIB,
        SO,
        A,
    }

    // in cb, the sqlcipher builds do not have the e_ prefix.
    // we do this so we can continue to build v1.
    // but in v2, we want things to be called e_sqlcipher.

    static string AsString_basename_in_cb(this WhichLib e)
    {
        switch (e)
        {
            case WhichLib.E_SQLITE3: return "e_sqlite3";
            case WhichLib.E_SQLCIPHER: return "e_sqlcipher"; // TODO no e_ prefix in cb yet
            case WhichLib.E_SQLITE3MC: return "e_sqlite3mc";
            default:
                throw new NotImplementedException(string.Format("WhichLib.AsString for {0}", e));
        }
    }

    static string AsString_basename_in_nupkg(this WhichLib e)
    {
        switch (e)
        {
            case WhichLib.E_SQLITE3: return "e_sqlite3";
            case WhichLib.E_SQLCIPHER: return "e_sqlcipher";
            case WhichLib.E_SQLITE3MC: return "e_sqlite3mc";
            default:
                throw new NotImplementedException(string.Format("WhichLib.AsString for {0}", e));
        }
    }

    static string basename_to_libname(string basename, LibSuffix suffix)
    {
        switch (suffix)
        {
            case LibSuffix.DLL:
                return $"{basename}.dll";
            case LibSuffix.DYLIB:
                return $"lib{basename}.dylib";
            case LibSuffix.SO:
                return $"lib{basename}.so";
            case LibSuffix.A:
                return $"{basename}.a";
            default:
                throw new NotImplementedException();
        }
    }

    static string AsString_libname_in_nupkg(this WhichLib e, LibSuffix suffix)
    {
        var basename = e.AsString_basename_in_nupkg();
        return basename_to_libname(basename, suffix);
    }

    static string AsString_libname_in_cb(this WhichLib e, LibSuffix suffix)
    {
        var basename = e.AsString_basename_in_cb();
        return basename_to_libname(basename, suffix);
    }

    private static void write_nuspec_file_entry_native(string src, string rid, string filename, XmlWriter f)
    {
        common.write_nuspec_file_entry(
            src,
            string.Format("runtimes\\{0}\\native\\{1}", rid, filename),
            f
            );
    }

    private static void write_nuspec_file_entry_nativeassets(string src, string rid, TFM tfm, string filename, XmlWriter f)
    {
        common.write_nuspec_file_entry(
            src,
            string.Format("runtimes\\{0}\\nativeassets\\{1}\\{2}", rid, tfm.AsString(), filename),
            f
            );
    }

    static string make_cb_path_win(
        WhichLib lib,
        string toolset,
        string flavor,
        string arch
        )
    {
        var dir_name = lib.AsString_basename_in_cb();
        var lib_name = lib.AsString_libname_in_cb(LibSuffix.DLL);
        return nuget_path_combine("$cb_bin_path$", dir_name, "win", toolset, flavor, arch, lib_name);
    }

    static string make_cb_path_linux(
        WhichLib lib,
        string cpu
        )
    {
        var dir_name = lib.AsString_basename_in_cb();
        var lib_name = lib.AsString_libname_in_cb(LibSuffix.SO);
        return nuget_path_combine("$cb_bin_path$", dir_name, "linux", cpu, lib_name);
    }

    static string make_cb_path_wasm(
        WhichLib lib,
        TFM tfm
        )
    {
        var dir_name = lib.AsString_basename_in_cb();
        var lib_name = lib.AsString_libname_in_cb(LibSuffix.A);
        return nuget_path_combine("$cb_bin_path$", dir_name, "wasm", tfm.AsString(), lib_name);
    }

    static string make_cb_path_mac(
        WhichLib lib,
        string cpu
        )
    {
        var dir_name = lib.AsString_basename_in_cb();
        var lib_name = lib.AsString_libname_in_cb(LibSuffix.DYLIB);
        return nuget_path_combine("$cb_bin_path$", dir_name, "mac", cpu, lib_name);
    }

    static string make_cb_path_maccatalyst(
        WhichLib lib,
        string cpu
        )
    {
        var dir_name = lib.AsString_basename_in_cb();
        var lib_name = lib.AsString_libname_in_cb(LibSuffix.DYLIB);
        return nuget_path_combine("$cb_bin_path$", dir_name, "maccatalyst", cpu, lib_name);
    }

    static void write_nuspec_file_entry_native_linux(
        WhichLib lib,
        string cpu_in_cb,
        string rid,
        XmlWriter f
        )
    {
        var filename = lib.AsString_libname_in_nupkg(LibSuffix.SO);
        write_nuspec_file_entry_native(
            make_cb_path_linux(lib, cpu_in_cb),
            rid,
            filename,
            f
            );
    }

    static void write_nuspec_file_entry_native_wasm(
        WhichLib lib,
        TFM tfm,
        XmlWriter f
        )
    {
        var filename = lib.AsString_libname_in_nupkg(LibSuffix.A);
        write_nuspec_file_entry_nativeassets(
            make_cb_path_wasm(lib, tfm),
            "browser-wasm",
            tfm,
            filename,
            f
            );
    }

    static void write_nuspec_file_entry_native_mac(
        WhichLib lib,
        string cpu_in_cb,
        string rid,
        XmlWriter f
        )
    {
        var filename = lib.AsString_libname_in_nupkg(LibSuffix.DYLIB);
        write_nuspec_file_entry_native(
            make_cb_path_mac(lib, cpu_in_cb),
            rid,
            filename,
            f
            );
    }

    static void write_nuspec_file_entry_native_maccatalyst(
        WhichLib lib,
        string cpu_in_cb,
        string rid,
        XmlWriter f
        )
    {
        var filename = lib.AsString_libname_in_nupkg(LibSuffix.DYLIB);
        write_nuspec_file_entry_native(
            make_cb_path_maccatalyst(lib, cpu_in_cb),
            rid,
            filename,
            f
            );
    }

    static void write_nuspec_file_entry_native_win(
        WhichLib lib,
        string toolset,
        string flavor,
        string cpu,
        string rid,
        XmlWriter f
        )
    {
        var filename = lib.AsString_libname_in_nupkg(LibSuffix.DLL);
        write_nuspec_file_entry_native(
            make_cb_path_win(lib, toolset, flavor, cpu),
            rid,
            filename,
            f
            );
    }

    static void write_nuspec_file_entry_native_uwp(
        WhichLib lib,
        string toolset,
        string flavor,
        string cpu,
        string rid,
        XmlWriter f
        )
    {
        var filename = lib.AsString_libname_in_nupkg(LibSuffix.DLL);
        write_nuspec_file_entry_nativeassets(
            make_cb_path_win(lib, toolset, flavor, cpu),
            rid,
            TFM.UWP,
            filename,
            f
            );
    }

    static void write_nuspec_file_entries_from_cb(
        WhichLib lib,
        string win_toolset,
        XmlWriter f
        )
    {
        write_nuspec_file_entry_native_win(lib, win_toolset, "plain", "x86", "win-x86", f);
        write_nuspec_file_entry_native_win(lib, win_toolset, "plain", "x64", "win-x64", f);
        write_nuspec_file_entry_native_win(lib, win_toolset, "plain", "arm", "win-arm", f);
        write_nuspec_file_entry_native_win(lib, win_toolset, "plain", "arm64", "win-arm64", f);

        write_nuspec_file_entry_native_uwp(lib, win_toolset, "appcontainer", "arm64", "win10-arm64", f);
        write_nuspec_file_entry_native_uwp(lib, win_toolset, "appcontainer", "arm", "win10-arm", f);
        write_nuspec_file_entry_native_uwp(lib, win_toolset, "appcontainer", "x64", "win10-x64", f);
        write_nuspec_file_entry_native_uwp(lib, win_toolset, "appcontainer", "x86", "win10-x86", f);

        write_nuspec_file_entry_native_mac(lib, "x86_64", "osx-x64", f);
        write_nuspec_file_entry_native_mac(lib, "arm64", "osx-arm64", f);

        write_nuspec_file_entry_native_maccatalyst(lib, "x86_64", "maccatalyst-x64", f);
        write_nuspec_file_entry_native_maccatalyst(lib, "arm64", "maccatalyst-arm64", f);

        write_nuspec_file_entry_native_linux(lib, "x64", "linux-x64", f);
        write_nuspec_file_entry_native_linux(lib, "x86", "linux-x86", f);
        write_nuspec_file_entry_native_linux(lib, "armhf", "linux-arm", f);
        write_nuspec_file_entry_native_linux(lib, "armsf", "linux-armel", f);
        write_nuspec_file_entry_native_linux(lib, "arm64", "linux-arm64", f);

        write_nuspec_file_entry_native_linux(lib, "musl-x64", "linux-musl-x64", f);
        write_nuspec_file_entry_native_linux(lib, "musl-armhf", "linux-musl-arm", f);
        write_nuspec_file_entry_native_linux(lib, "musl-arm64", "linux-musl-arm64", f);
        write_nuspec_file_entry_native_linux(lib, "musl-s390x", "linux-musl-s390x", f);

        write_nuspec_file_entry_native_linux(lib, "mips64", "linux-mips64", f);
        write_nuspec_file_entry_native_linux(lib, "s390x", "linux-s390x", f);
	write_nuspec_file_entry_native_linux(lib, "ppc64le", "linux-ppc64le", f);

        write_nuspec_file_entry_native_wasm(lib, TFM.NET60, f);
        write_nuspec_file_entry_native_wasm(lib, TFM.NET70, f);
        write_nuspec_file_entry_native_wasm(lib, TFM.NET80, f);
    }

    static void write_nuspec_wasm_targets_file_entry(
        string dir_src,
        string id,
        WhichLib lib,
        TFM tfm,
        XmlWriter f
        )
    {
        var tname = string.Format("{0}.targets", id);
        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(Path.Combine(dir_proj, tfm.AsString()));
        var path_targets = Path.Combine(dir_proj, tfm.AsString(), tname);
        var relpath_targets = nuget_path_combine(".", tfm.AsString(), tname);
        gen_nuget_targets_wasm(path_targets, tfm, lib);
        common.write_nuspec_file_entry(
            relpath_targets,
            string.Format("buildTransitive\\{0}", tfm.AsString()),
            f
            );
    }

    private static void gen_nuspec_lib_e_sqlite3(string dir_src)
    {
        string id = string.Format("{0}.lib.e_sqlite3", common.ROOT_NAME);

        var settings = common.XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        common.gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            common.write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "This package contains platform-specific native code builds of SQLite for use with SQLitePCLRaw.  To use this, you need SQLitePCLRaw.core as well as one of the SQLitePCLRaw.provider.* packages.  Convenience packages are named SQLitePCLRaw.bundle_*.");

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entries_from_cb(WhichLib.E_SQLITE3, "v142", f);

#if not
            {
                var tname = string.Format("{0}.props", id);
                var path_props = Path.Combine(dir_proj, tname);
                var relpath_props = nuget_path_combine(".", tname);
                gen_nuget_props(path_props, WhichLib.E_SQLITE3);
                common.write_nuspec_file_entry(
                    relpath_props,
                    "buildTransitive\\",
                    f
                    );
            }
#endif
            {
                var tname = string.Format("{0}.targets", id);
                Directory.CreateDirectory(Path.Combine(dir_proj, "net461"));
                var path_targets = Path.Combine(dir_proj, "net461", tname);
                var relpath_targets = nuget_path_combine(".", "net461", tname);
                gen_nuget_targets(path_targets, WhichLib.E_SQLITE3);
                common.write_nuspec_file_entry(
                    relpath_targets,
                    string.Format("buildTransitive\\{0}", TFM.NET461.AsString()),
                    f
                    );
            }
#if not
            {
                var tname = string.Format("{0}.targets", id);
                Directory.CreateDirectory(Path.Combine(dir_proj, "xamarin.mac20"));
                var path_targets = Path.Combine(dir_proj, "xamarin.mac20", tname);
                var relpath_targets = nuget_path_combine(".", "xamarin.mac20", tname);
                gen_nuget_targets_legacy_xamarin_mac(path_targets, WhichLib.E_SQLITE3);
                common.write_nuspec_file_entry(
                    relpath_targets,
                    string.Format("buildTransitive\\{0}", TFM.XAMARINMAC20.AsString()),
                    f
                    );
            }
#endif
#if not
            {
                var tname = string.Format("{0}.targets", id);
                Directory.CreateDirectory(Path.Combine(dir_proj, "net6.0-maccatalyst"));
                var path_targets = Path.Combine(dir_proj, "net6.0-maccatalyst", tname);
                var relpath_targets = nuget_combine(".", "net6.0-maccatalyst", tname);
                gen_nuget_targets_maccatalyst(path_targets, WhichLib.E_SQLITE3);
                common.write_nuspec_file_entry(
                    relpath_targets,
                    string.Format("buildTransitive\\{0}", TFM.MACCATALYST.AsString()),
                    f
                    );
            }
#endif

            write_nuspec_wasm_targets_file_entry(dir_src, id, WhichLib.E_SQLITE3, TFM.NET60, f);
            write_nuspec_wasm_targets_file_entry(dir_src, id, WhichLib.E_SQLITE3, TFM.NET70, f);
            write_nuspec_wasm_targets_file_entry(dir_src, id, WhichLib.E_SQLITE3, TFM.NET80, f);

            // TODO need a comment here to explain these
            common.write_empty(f, TFM.NET461);
            common.write_empty(f, TFM.NETSTANDARD20);
#if not
            common.write_empty(f, TFM.XAMARINMAC20);
#endif

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    private static void gen_nuspec_lib_e_sqlcipher(string dir_src)
    {
        string id = string.Format("{0}.lib.e_sqlcipher", common.ROOT_NAME);

        var settings = common.XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        common.gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            common.write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "This package contains platform-specific native code builds of SQLCipher (see sqlcipher/sqlcipher on GitHub) for use with SQLitePCLRaw.  Note that these sqlcipher builds are unofficial and unsupported.  For official sqlcipher builds, contact Zetetic.  To use this package, you need SQLitePCLRaw.core as well as one of the SQLitePCLRaw.provider.* packages.  Convenience packages are named SQLitePCLRaw.bundle_*.");

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entries_from_cb(WhichLib.E_SQLCIPHER, "v142", f);

            {
                var tname = string.Format("{0}.targets", id);
                Directory.CreateDirectory(Path.Combine(dir_proj, "net461"));
                var path_targets = Path.Combine(dir_proj, "net461", tname);
                var relpath_targets = nuget_path_combine(".", "net461", tname);
                gen_nuget_targets(path_targets, WhichLib.E_SQLCIPHER);
                common.write_nuspec_file_entry(
                    relpath_targets,
                    string.Format("buildTransitive\\{0}", TFM.NET461.AsString()),
                    f
                    );
            }

            write_nuspec_wasm_targets_file_entry(dir_src, id, WhichLib.E_SQLCIPHER, TFM.NET60, f);
            write_nuspec_wasm_targets_file_entry(dir_src, id, WhichLib.E_SQLCIPHER, TFM.NET70, f);
            write_nuspec_wasm_targets_file_entry(dir_src, id, WhichLib.E_SQLCIPHER, TFM.NET80, f);

            // TODO need a comment here to explain these
            common.write_empty(f, TFM.NET461);
            common.write_empty(f, TFM.NETSTANDARD20);

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    private static void gen_nuspec_lib_e_sqlite3mc(string dir_src)
    {
        string id = string.Format("{0}.lib.e_sqlite3mc", common.ROOT_NAME);

        var settings = common.XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        common.gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            common.write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "This package contains platform-specific native code builds of SQLite3 Multiple Ciphers (see utelle/SQLite3MultipleCiphers on GitHub) for use with SQLitePCLRaw.  To use this package, you need SQLitePCLRaw.core as well as one of the SQLitePCLRaw.provider.* packages.  Convenience packages are named SQLitePCLRaw.bundle_*.");

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entries_from_cb(WhichLib.E_SQLITE3MC, "v142", f);

            {
                var tname = string.Format("{0}.targets", id);
                Directory.CreateDirectory(Path.Combine(dir_proj, "net461"));
                var path_targets = Path.Combine(dir_proj, "net461", tname);
                var relpath_targets = nuget_path_combine(".", "net461", tname);
                gen_nuget_targets(path_targets, WhichLib.E_SQLITE3MC);
                common.write_nuspec_file_entry(
                    relpath_targets,
                    string.Format("buildTransitive\\{0}", TFM.NET461.AsString()),
                    f
                    );
            }

            write_nuspec_wasm_targets_file_entry(dir_src, id, WhichLib.E_SQLITE3MC, TFM.NET60, f);
            write_nuspec_wasm_targets_file_entry(dir_src, id, WhichLib.E_SQLITE3MC, TFM.NET70, f);
            write_nuspec_wasm_targets_file_entry(dir_src, id, WhichLib.E_SQLITE3MC, TFM.NET80, f);

            // TODO need a comment here to explain these
            common.write_empty(f, TFM.NET461);
            common.write_empty(f, TFM.NETSTANDARD20);

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    enum WhichLib
    {
        NONE,
        E_SQLITE3,
        E_SQLCIPHER,
        E_SQLITE3MC,
    }

    static LibSuffix get_lib_suffix_from_rid(string rid)
    {
        var parts = rid.Split('-');
        var front = parts[0].ToLower();
        if (front.StartsWith("win"))
        {
            return LibSuffix.DLL;
        }
        else if (front.StartsWith("osx"))
        {
            return LibSuffix.DYLIB;
        }
        else if (
            front.StartsWith("linux")
            || front.StartsWith("alpine")
            )
        {
            return LibSuffix.SO;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    static void write_nuget_target_item(
        string rid,
        WhichLib lib,
        XmlWriter f
        )
    {
        var suffix = get_lib_suffix_from_rid(rid);
        var filename = lib.AsString_libname_in_nupkg(suffix);

        f.WriteStartElement("Content");
        f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\{0}\\native\\{1}", rid, filename));
        f.WriteElementString("Link", string.Format("runtimes\\{0}\\native\\{1}", rid, filename));
        f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
        f.WriteElementString("Pack", "false");
        f.WriteEndElement(); // Content
    }

#if not
    static void write_nuget_prop_item(
        string rid,
        WhichLib lib,
        XmlWriter f
        )
    {
        var suffix = get_lib_suffix_from_rid(rid);
        var filename = lib.AsString_libname_in_nupkg(suffix);

        var rid_idsafe = rid.Replace("-", "_");
        var prop_name = $"path__{lib.AsString_basename_in_nupkg()}__{rid_idsafe}";

        f.WriteElementString(prop_name, string.Format("$(MSBuildThisFileDirectory)..\\runtimes\\{0}\\native\\{1}", rid, filename));
    }

    private static void gen_nuget_props(string dest, WhichLib lib)
    {
        var settings = common.XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        using (XmlWriter f = XmlWriter.Create(dest, settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            f.WriteAttributeString("ToolsVersion", "4.0");

            f.WriteStartElement("PropertyGroup");
            write_nuget_prop_item("win-x86", lib, f);
            write_nuget_prop_item("win-x64", lib, f);
            write_nuget_prop_item("win-arm", lib, f);
            write_nuget_prop_item("osx-x64", lib, f);
            write_nuget_prop_item("osx-arm64", lib, f);
            write_nuget_prop_item("linux-x86", lib, f);
            write_nuget_prop_item("linux-x64", lib, f);
            write_nuget_prop_item("linux-arm", lib, f);
            write_nuget_prop_item("linux-armel", lib, f);
            write_nuget_prop_item("linux-arm64", lib, f);
            write_nuget_prop_item("linux-x64", lib, f);
            write_nuget_prop_item("linux-mips64", lib, f);
            write_nuget_prop_item("linux-s390x", lib, f);
	    write_nuget_prop_item("linux-ppc64le", lib, f);
            f.WriteEndElement(); // PropertyGroup

            f.WriteEndElement(); // Project

            f.WriteEndDocument();
        }
    }
#endif

    private static void gen_nuget_targets(string dest, WhichLib lib)
    {
        var settings = common.XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        // TODO these targets appear to be for old situations where RuntimeIdentifier
        // was not set, like desktop mono, pre-net6 days

        using (XmlWriter f = XmlWriter.Create(dest, settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            f.WriteAttributeString("ToolsVersion", "4.0");

            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(RuntimeIdentifier)' == '' AND '$(OS)' == 'Windows_NT' ");
            write_nuget_target_item("win-x86", lib, f);
            write_nuget_target_item("win-x64", lib, f);
            write_nuget_target_item("win-arm", lib, f);
            f.WriteEndElement(); // ItemGroup

            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(RuntimeIdentifier)' == '' AND '$(OS)' == 'Unix' AND Exists('/Library/Frameworks') ");
            // TODO in what case does this make sense?
            write_nuget_target_item("osx-x64", lib, f);
            f.WriteEndElement(); // ItemGroup

            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(RuntimeIdentifier)' == '' AND '$(OS)' == 'Unix' AND !Exists('/Library/Frameworks') ");
            write_nuget_target_item("linux-x86", lib, f);
            write_nuget_target_item("linux-x64", lib, f);
            write_nuget_target_item("linux-arm", lib, f);
            write_nuget_target_item("linux-armel", lib, f);
            write_nuget_target_item("linux-arm64", lib, f);
            write_nuget_target_item("linux-x64", lib, f);
            write_nuget_target_item("linux-mips64", lib, f);
            write_nuget_target_item("linux-s390x", lib, f);
            f.WriteEndElement(); // ItemGroup

            f.WriteEndElement(); // Project

            f.WriteEndDocument();
        }
    }

#if not
    private static void gen_nuget_targets_legacy_xamarin_mac(string dest, WhichLib lib)
    {
        var settings = common.XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        using (XmlWriter f = XmlWriter.Create(dest, settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            f.WriteAttributeString("ToolsVersion", "4.0");

            var filename = lib.AsString_libname_in_nupkg(LibSuffix.DYLIB);

            // --------
            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(RuntimeIdentifier)' == 'osx-x64' ");

            f.WriteStartElement("NativeFileReference");
            f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\osx-x64\\native\\{0}", filename));
            f.WriteElementString("Kind", "Dynamic");
            f.WriteElementString("SmartLink", "False");
            f.WriteEndElement(); // NativeFileReference

            f.WriteEndElement(); // ItemGroup

            // --------
            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(RuntimeIdentifier)' == 'osx-arm64' ");

            f.WriteStartElement("NativeFileReference");
            f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\osx-arm64\\native\\{0}", filename));
            f.WriteElementString("Kind", "Dynamic");
            f.WriteElementString("SmartLink", "False");
            f.WriteEndElement(); // NativeFileReference

            f.WriteEndElement(); // ItemGroup

            f.WriteEndElement(); // Project

            f.WriteEndDocument();
        }

    }
#endif

    private static void gen_nuget_targets_wasm(string dest, TFM tfm, WhichLib lib)
    {
        var settings = common.XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        using (XmlWriter f = XmlWriter.Create(dest, settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            f.WriteAttributeString("ToolsVersion", "4.0");

            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(RuntimeIdentifier)' == 'browser-wasm' ");

            var filename = lib.AsString_libname_in_nupkg(LibSuffix.A);

            f.WriteStartElement("NativeFileReference");
            f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\browser-wasm\\nativeassets\\{0}\\{1}", tfm.AsString(), filename));
            f.WriteEndElement(); // NativeFileReference

            f.WriteEndElement(); // ItemGroup

            f.WriteEndElement(); // Project

            f.WriteEndDocument();
        }

    }

    private static string nuget_path_combine(params string[] paths)
    {
        return Path.Combine(paths).Replace('/', '\\');
    }

#if not
    private static void gen_nuget_targets_maccatalyst(string dest, WhichLib lib)
    {
        var settings = common.XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        using (XmlWriter f = XmlWriter.Create(dest, settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            f.WriteAttributeString("ToolsVersion", "4.0");

            var filename = lib.AsString_libname_in_nupkg(LibSuffix.DYLIB);

            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(RuntimeIdentifier)' == 'maccatalyst-x64' ");
            f.WriteStartElement("NativeReference");
            f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\maccatalyst-x64\\nativeassets\\net6.0\\{0}", filename));
            f.WriteEndElement(); // Content
            f.WriteEndElement(); // ItemGroup

            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(RuntimeIdentifier)' == 'maccatalyst-arm64' ");
            f.WriteStartElement("NativeReference");
            f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\maccatalyst-arm64\\nativeassets\\net6.0\\{0}", filename));
            f.WriteEndElement(); // Content
            f.WriteEndElement(); // ItemGroup

            f.WriteEndElement(); // Project

            f.WriteEndDocument();
        }

    }
#endif

    public static void Main(string[] args)
    {
        string dir_root = Path.GetFullPath(args[0]);

        var dir_src = Path.Combine(dir_root, "src");

        gen_nuspec_lib_e_sqlite3(dir_src);
        gen_nuspec_lib_e_sqlcipher(dir_src);
        gen_nuspec_lib_e_sqlite3mc(dir_src);
    }
}

