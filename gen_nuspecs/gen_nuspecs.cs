/*
   Copyright 2014-2019 SourceGear, LLC

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
#define NETCOREAPP3_NATIVELIBRARY
//#define WINSQLITE3_DYNAMIC
#define SQLITE3_DYNAMIC

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

public static class gen
{
    public const string ROOT_NAME = "SQLitePCLRaw";

    enum TFM
    {
        NONE,
        IOS,
        TVOS,
        ANDROID,
        UWP,
        NETSTANDARD20,
        NET461,
        XAMARIN_MAC,
        NETCOREAPP31,
    }

    enum LibSuffix
    {
        DLL,
        DYLIB,
        SO,
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

    static string AsString(this TFM e)
    {
        switch (e)
        {
            case TFM.NONE: throw new Exception("TFM.NONE.AsString()");
            case TFM.IOS: return "Xamarin.iOS10";
            case TFM.TVOS: return "Xamarin.tvOS10";
            case TFM.ANDROID: return "MonoAndroid80";
            case TFM.UWP: return "uap10.0";
            case TFM.NETSTANDARD20: return "netstandard2.0";
            case TFM.XAMARIN_MAC: return "Xamarin.Mac20";
            case TFM.NET461: return "net461";
            case TFM.NETCOREAPP31: return "netcoreapp3.1";
            default:
                throw new NotImplementedException(string.Format("TFM.AsString for {0}", e));
        }
    }

    private static void write_nuspec_file_entry(string src, string target, XmlWriter f)
    {
        f.WriteStartElement("file");
        f.WriteAttributeString("src", src);
        f.WriteAttributeString("target", target);
        f.WriteEndElement(); // file
    }

    private static void write_nuspec_file_entry_lib(string src, TFM tfm, XmlWriter f)
    {
        write_nuspec_file_entry(
            src,
            string.Format("lib\\{0}\\", tfm.AsString()),
            f
            );
    }

    static string make_mt_path(
        string dir_name,
        string assembly_name,
        TFM tfm
        )
    {
        switch (tfm)
        {
            case TFM.ANDROID:
                return Path.Combine(
                    "$src_path$",
                    dir_name,
                    "bin",
                    "Release",
                    "monoandroid80",
                    "80", // TODO why does the android build end up with this extra subdir?
                    string.Format("{0}.dll", assembly_name)
                    );

            default:
                return Path.Combine(
                    "$src_path$",
                    dir_name,
                    "bin",
                    "Release",
                    tfm.AsString(),
                    string.Format("{0}.dll", assembly_name)
                    );
        }
    }

    static string make_mt_path_batteries(
        string basename,
        TFM tfm
        )
    {
        return make_mt_path(
            $"SQLitePCLRaw.batteries_v2.{basename}",
            "SQLitePCLRaw.batteries_v2",
            tfm
            );
    }

    static string make_mt_path(
        string name,
        TFM tfm
        )
    {
        return make_mt_path(name, name, tfm);
    }

    private static void write_nuspec_file_entry_lib_mt(string name, TFM tfm, XmlWriter f)
    {
        write_nuspec_file_entry(
            make_mt_path(name, tfm),
            string.Format("lib\\{0}\\", tfm.AsString()),
            f
            );
    }

    private static void write_nuspec_file_entry_lib_mt(string name, TFM tfm_build, TFM tfm_dest, XmlWriter f)
    {
        write_nuspec_file_entry(
            make_mt_path(name, tfm_build),
            string.Format("lib\\{0}\\", tfm_dest.AsString()),
            f
            );
    }

    private static void write_nuspec_file_entry_lib_mt(string dir_name, string assembly_name, TFM tfm_build, TFM tfm_dest, XmlWriter f)
    {
        write_nuspec_file_entry(
            make_mt_path(dir_name, assembly_name, tfm_build),
            string.Format("lib\\{0}\\", tfm_dest.AsString()),
            f
            );
    }

    private static void write_nuspec_file_entry_lib_batteries(string basename, TFM tfm_build, TFM tfm_dest, XmlWriter f)
    {
        write_nuspec_file_entry(
            make_mt_path_batteries(basename, tfm_build),
            string.Format("lib\\{0}\\", tfm_dest.AsString()),
            f
            );
    }

    private static void write_nuspec_file_entry_lib_batteries(string basename, TFM tfm_both, XmlWriter f)
    {
        write_nuspec_file_entry_lib_batteries(
            basename,
            tfm_build: tfm_both,
            tfm_dest: tfm_both,
            f
            );
    }

    private static void write_nuspec_file_entry_native(string src, string rid, string filename, XmlWriter f)
    {
        write_nuspec_file_entry(
            src,
            string.Format("runtimes\\{0}\\native\\{1}", rid, filename),
            f
            );
    }

    private static void write_nuspec_file_entry_nativeassets(string src, string rid, TFM tfm, string filename, XmlWriter f)
    {
        write_nuspec_file_entry(
            src,
            string.Format("runtimes\\{0}\\nativeassets\\{1}\\{2}", rid, tfm.AsString(), filename),
            f
            );
    }

    private static void write_empty(XmlWriter f, TFM tfm)
    {
        f.WriteComment("empty directory in lib to avoid nuget adding a reference");

        f.WriteStartElement("file");
        f.WriteAttributeString("src", "empty\\");
        f.WriteAttributeString("target", string.Format("lib\\{0}", tfm.AsString()));
        f.WriteEndElement(); // file
    }

    private static void add_dep_core(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", string.Format("{0}.core", gen.ROOT_NAME));
        f.WriteAttributeString("version", "$version$");
        f.WriteEndElement(); // dependency
    }

    const string PACKAGE_TAGS = "sqlite;xamarin";

    private static void write_nuspec_common_metadata(
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
        f.WriteElementString("releaseNotes", "$releaseNotes$");
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
        return Path.Combine("$cb_bin_path$", dir_name, "win", toolset, flavor, arch, lib_name);
    }

    static string make_cb_path_linux(
        WhichLib lib,
        string cpu
        )
    {
        var dir_name = lib.AsString_basename_in_cb();
        var lib_name = lib.AsString_libname_in_cb(LibSuffix.SO);
        return Path.Combine("$cb_bin_path$", dir_name, "linux", cpu, lib_name);
    }

    static string make_cb_path_mac(
        WhichLib lib
        )
    {
        var dir_name = lib.AsString_basename_in_cb();
        var lib_name = lib.AsString_libname_in_cb(LibSuffix.DYLIB);
        return Path.Combine("$cb_bin_path$", dir_name, "mac", lib_name);
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

    static void write_nuspec_file_entry_native_mac(
        WhichLib lib,
        XmlWriter f
        )
    {
        var filename = lib.AsString_libname_in_nupkg(LibSuffix.DYLIB);
        write_nuspec_file_entry_native(
            make_cb_path_mac(lib),
            "osx-x64",
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

        write_nuspec_file_entry_native_mac(lib, f);

        write_nuspec_file_entry_native_linux(lib, "x64", "linux-x64", f);
        write_nuspec_file_entry_native_linux(lib, "x86", "linux-x86", f);
        write_nuspec_file_entry_native_linux(lib, "armhf", "linux-arm", f);
        write_nuspec_file_entry_native_linux(lib, "armsf", "linux-armel", f);
        write_nuspec_file_entry_native_linux(lib, "arm64", "linux-arm64", f);
        write_nuspec_file_entry_native_linux(lib, "musl-x64", "linux-musl-x64", f);
        write_nuspec_file_entry_native_linux(lib, "musl-x64", "alpine-x64", f);
        write_nuspec_file_entry_native_linux(lib, "mips64", "linux-mips64", f);
    }

    private static XmlWriterSettings XmlWriterSettings_default()
    {
        var settings = new XmlWriterSettings();
        settings.NewLineChars = "\n";
        settings.Indent = true;
        return settings;
    }

    private static void gen_dummy_csproj(string dir_proj, string id)
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
            f.WriteElementString("NuspecProperties", "version=$(version);src_path=$(src_path);cb_bin_path=$(cb_bin_path);authors=$(Authors);copyright=$(Copyright);summary=$(Description);releaseNotes=$(PackageReleaseNotes)");

            f.WriteEndElement(); // PropertyGroup

            f.WriteEndElement(); // Project

            f.WriteEndDocument();
        }
    }

    private static void gen_nuspec_lib_e_sqlite3(string dir_src)
    {
        string id = string.Format("{0}.lib.e_sqlite3", gen.ROOT_NAME);

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "This package contains platform-specific native code builds of SQLite for use with SQLitePCLRaw.  To use this, you need SQLitePCLRaw.core as well as one of the SQLitePCLRaw.provider.* packages.  Convenience packages are named SQLitePCLRaw.bundle_*.");

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

#if not
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.lib.e_sqlite3.ios",
                    TFM.IOS,
                    f
                    );

            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.lib.e_sqlite3.android",
                    TFM.ANDROID,
                    f
                    );
#endif

            write_nuspec_file_entries_from_cb(WhichLib.E_SQLITE3, "v142", f);

            var tname = string.Format("{0}.targets", id);
            var path_targets = Path.Combine(dir_proj, tname);
            var relpath_targets = Path.Combine(".", tname);
            gen_nuget_targets(path_targets, WhichLib.E_SQLITE3);
            write_nuspec_file_entry(
                relpath_targets,
                string.Format("build\\{0}", TFM.NET461.AsString()),
                f
                );

            // TODO need a comment here to explain these
            write_empty(f, TFM.NET461);
            write_empty(f, TFM.NETSTANDARD20);

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    private static void gen_nuspec_lib_e_sqlcipher(string dir_src)
    {
        string id = string.Format("{0}.lib.e_sqlcipher", gen.ROOT_NAME);

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "This package contains platform-specific native code builds of SQLCipher (see sqlcipher/sqlcipher on GitHub) for use with SQLitePCLRaw.  Note that these sqlcipher builds are unofficial and unsupported.  For official sqlcipher builds, contact Zetetic.  To use this package, you need SQLitePCLRaw.core as well as one of the SQLitePCLRaw.provider.* packages.  Convenience packages are named SQLitePCLRaw.bundle_*.");

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

#if not
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.lib.e_sqlcipher.ios",
                    TFM.IOS,
                    f
                    );

            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.lib.e_sqlcipher.android",
                    TFM.ANDROID,
                    f
                    );
#endif

            write_nuspec_file_entries_from_cb(WhichLib.E_SQLCIPHER, "v141", f);

            var tname = string.Format("{0}.targets", id);
            var path_targets = Path.Combine(dir_proj, tname);
            var relpath_targets = Path.Combine(".", tname);
            gen_nuget_targets(path_targets, WhichLib.E_SQLCIPHER);
            write_nuspec_file_entry(
                relpath_targets,
                string.Format("build\\{0}", TFM.NET461.AsString()),
                f
                );

            // TODO need a comment here to explain these
            write_empty(f, TFM.NET461);
            write_empty(f, TFM.NETSTANDARD20);

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

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

    enum WhichProvider
    {
        E_SQLITE3,
        E_SQLCIPHER,
        SQLCIPHER,
        INTERNAL,
        SQLITE3,
        WINSQLITE3,
        DYNAMIC_CDECL,
        DYNAMIC_STDCALL,
    }

    static string AsString(this WhichProvider e)
    {
        switch (e)
        {
            case WhichProvider.E_SQLITE3: return "e_sqlite3";
            case WhichProvider.E_SQLCIPHER: return "e_sqlcipher";
            case WhichProvider.SQLCIPHER: return "sqlcipher";
            case WhichProvider.INTERNAL: return "internal";
            case WhichProvider.SQLITE3: return "sqlite3";
            case WhichProvider.WINSQLITE3: return "winsqlite3";
            case WhichProvider.DYNAMIC_CDECL: return "dynamic_cdecl";
            case WhichProvider.DYNAMIC_STDCALL: return "dynamic_stdcall";
            default:
                throw new NotImplementedException(string.Format("WhichProvider.AsString for {0}", e));
        }
    }

    private static void gen_nuspec_bundle_winsqlite3(string dir_src)
    {
        string id = string.Format("{0}.bundle_winsqlite3", gen.ROOT_NAME);

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: .no SQLite library included, uses winsqlite3.dll from Windows 10.");

            f.WriteStartElement("dependencies");

#if WINSQLITE3_DYNAMIC 
            // use dynamic provider
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_STDCALL, WhichLib.NONE, TFM.NETSTANDARD20);
#else
            // use dllimport provider
            write_bundle_dependency_group(f, WhichProvider.WINSQLITE3, WhichLib.NONE, TFM.NETSTANDARD20);
#endif
            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

#if WINSQLITE3_DYNAMIC
            // use dynamic provider
            write_nuspec_file_entry_lib_batteries(
                    "winsqlite3.dynamic",
                    TFM.NETSTANDARD20,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.nativelibrary",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NETSTANDARD20,
                    f
                    );
#else
            // use dllimport provider
            write_nuspec_file_entry_lib_batteries(
                    "winsqlite3.dllimport",
                    TFM.NETSTANDARD20,
                    f
                    );
#endif

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
    }

    private static void write_bundle_dependency_group(
        XmlWriter f,
        WhichProvider prov,
        WhichLib what,
        TFM tfm
        )
    {
        f.WriteStartElement("group");
        if (tfm != TFM.NONE)
        {
            f.WriteAttributeString("targetFramework", tfm.AsString());
        }

        add_dep_core(f);

        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", string.Format("{0}.provider.{1}", gen.ROOT_NAME, prov.AsString()));
        f.WriteAttributeString("version", "$version$");
        f.WriteEndElement(); // dependency

        if (what == WhichLib.E_SQLITE3)
        {
            string id;
            switch (tfm)
            {
                case TFM.IOS:
                    id = string.Format("{0}.lib.e_sqlite3.ios", gen.ROOT_NAME);
                    break;
                case TFM.TVOS:
                    id = string.Format("{0}.lib.e_sqlite3.tvos", gen.ROOT_NAME);
                    break;
                case TFM.ANDROID:
                    id = string.Format("{0}.lib.e_sqlite3.android", gen.ROOT_NAME);
                    break;
                default:
                    id = string.Format("{0}.lib.e_sqlite3", gen.ROOT_NAME);
                    break;
            }
            f.WriteStartElement("dependency");
            f.WriteAttributeString("id", id);
            f.WriteAttributeString("version", "$version$");
            f.WriteEndElement(); // dependency
        }
        else if (what == WhichLib.E_SQLCIPHER)
        {
            string id;
            switch (tfm)
            {
                case TFM.IOS:
                    id = string.Format("{0}.lib.e_sqlcipher.ios", gen.ROOT_NAME);
                    break;
                case TFM.TVOS:
                    id = string.Format("{0}.lib.e_sqlcipher.tvos", gen.ROOT_NAME);
                    break;
                case TFM.ANDROID:
                    id = string.Format("{0}.lib.e_sqlcipher.android", gen.ROOT_NAME);
                    break;
                default:
                    id = string.Format("{0}.lib.e_sqlcipher", gen.ROOT_NAME);
                    break;
            }
            f.WriteStartElement("dependency");
            f.WriteAttributeString("id", id);
            f.WriteAttributeString("version", "$version$");
            f.WriteEndElement(); // dependency
        }
        else if (what == WhichLib.NONE)
        {
        }
        else
        {
            throw new NotImplementedException();
        }

        f.WriteEndElement(); // group
    }

    private static void gen_nuspec_bundle_e_sqlcipher(string dir_src)
    {
        var id = string.Format("{0}.bundle_e_sqlcipher", gen.ROOT_NAME);

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: unofficial open source sqlcipher builds included.  Note that these sqlcipher builds are unofficial and unsupported.  For official sqlcipher builds, contact Zetetic.");

            f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, WhichProvider.INTERNAL, WhichLib.E_SQLCIPHER, TFM.IOS);
            write_bundle_dependency_group(f, WhichProvider.E_SQLCIPHER, WhichLib.E_SQLCIPHER, TFM.ANDROID);
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.E_SQLCIPHER, TFM.NET461);
            write_bundle_dependency_group(f, WhichProvider.E_SQLCIPHER, WhichLib.E_SQLCIPHER, TFM.NETSTANDARD20);
#if NETCOREAPP3_NATIVELIBRARY
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.E_SQLCIPHER, TFM.NETCOREAPP31);
#endif

            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entry_lib_batteries(
                    "e_sqlcipher.internal.ios",
                    TFM.IOS,
                    f
                    );

            write_nuspec_file_entry_lib_batteries(
                    "e_sqlcipher.dynamic",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NET461,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.nativelibrary",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NET461,
                    f
                    );

#if NETCOREAPP3_NATIVELIBRARY
            write_nuspec_file_entry_lib_batteries(
                    "e_sqlcipher.dynamic",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NETCOREAPP31,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.nativelibrary",
                    tfm_build: TFM.NETCOREAPP31,
                    tfm_dest: TFM.NETCOREAPP31,
                    f
                    );
#endif

            write_nuspec_file_entry_lib_batteries(
                    "e_sqlcipher.dllimport",
                    TFM.NETSTANDARD20,
                    f
                    );

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    private static void gen_nuspec_bundle_zetetic(string dir_src)
    {
        var id = string.Format("{0}.bundle_zetetic", gen.ROOT_NAME);

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: reference the official SQLCipher builds from Zetetic, which are not included in this package");

            f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, WhichProvider.INTERNAL, WhichLib.NONE, TFM.IOS);
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.NONE, TFM.NET461);
            write_bundle_dependency_group(f, WhichProvider.SQLCIPHER, WhichLib.NONE, TFM.NETSTANDARD20);
#if NETCOREAPP3_NATIVELIBRARY
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.NONE, TFM.NETCOREAPP31);
#endif

            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entry_lib_batteries(
                    "sqlcipher.internal.ios",
                    TFM.IOS,
                    f
                    );

            write_nuspec_file_entry_lib_batteries(
                    "sqlcipher.dynamic",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NET461,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.nativelibrary",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NET461,
                    f
                    );

#if NETCOREAPP3_NATIVELIBRARY
            write_nuspec_file_entry_lib_batteries(
                    "sqlcipher.dynamic",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NETCOREAPP31,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.nativelibrary",
                    tfm_build: TFM.NETCOREAPP31,
                    tfm_dest: TFM.NETCOREAPP31,
                    f
                    );
#endif

            write_nuspec_file_entry_lib_batteries(
                    "sqlcipher.dllimport",
                    TFM.NETSTANDARD20,
                    f
                    );

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    private static void gen_nuspec_provider_e_sqlite3(string dir_src)
    {
        string id = string.Format("{0}.provider.e_sqlite3", gen.ROOT_NAME);

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "SQLitePCL.raw is a Portable Class Library (PCL) for low-level (raw) access to SQLite.  Packages named 'SQLitePCLRaw.provider.*' (like this one) are 'plugins' that allow SQLitePCLRaw.core to access the native SQLite library.  This provider does DllImport of 'e_sqlite3', the SQLite builds provided with SQLitePCLRaw.");

            f.WriteStartElement("dependencies");
            add_dep_core(f);
            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.provider.e_sqlite3.most",
                    "SQLitePCLRaw.provider.e_sqlite3",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NETSTANDARD20,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.provider.e_sqlite3.uwp",
                    "SQLitePCLRaw.provider.e_sqlite3",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.UWP,
                    f
                    );

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    private static void gen_nuspec_provider_e_sqlcipher(string dir_src)
    {
        string id = string.Format("{0}.provider.e_sqlcipher", gen.ROOT_NAME);

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "SQLitePCL.raw is a Portable Class Library (PCL) for low-level (raw) access to SQLite.  Packages named 'SQLitePCLRaw.provider.*' (like this one) are 'plugins' that allow SQLitePCLRaw.core to access the native SQLite library.  This provider does DllImport of 'e_sqlcipher', the unofficial SQLCipher builds provided with SQLitePCLRaw.");

            f.WriteStartElement("dependencies");
            add_dep_core(f);
            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.provider.e_sqlcipher.most",
                    "SQLitePCLRaw.provider.e_sqlcipher",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NETSTANDARD20,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.provider.e_sqlcipher.uwp",
                    "SQLitePCLRaw.provider.e_sqlcipher",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.UWP,
                    f
                    );

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    private static void gen_nuspec_provider_sqlcipher(string dir_src)
    {
        string id = string.Format("{0}.provider.sqlcipher", gen.ROOT_NAME);

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "SQLitePCL.raw is a Portable Class Library (PCL) for low-level (raw) access to SQLite.  Packages named 'SQLitePCLRaw.provider.*' (like this one) are 'plugins' that allow SQLitePCLRaw.core to access the native SQLite library.  This provider does DllImport of 'sqlcipher' and is intended for use with official SQLCipher builds from Zetetic.");

            f.WriteStartElement("dependencies");
            add_dep_core(f);
            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.provider.sqlcipher.most",
                    "SQLitePCLRaw.provider.sqlcipher",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NETSTANDARD20,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.provider.sqlcipher.uwp",
                    "SQLitePCLRaw.provider.sqlcipher",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.UWP,
                    f
                    );

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    enum WhichBasicBundle
    {
        E_SQLITE3,
        GREEN,
    }

    private static void gen_nuspec_bundle_e_sqlite3_or_green(
        string dir_src,
        WhichBasicBundle bund
        )
    {
        string bund_name;
        switch (bund)
        {
            case WhichBasicBundle.E_SQLITE3: bund_name = "e_sqlite3"; break;
            case WhichBasicBundle.GREEN: bund_name = "green"; break;
            default: throw new NotImplementedException(); break;
        }
        string id = $"{gen.ROOT_NAME}.bundle_{bund_name}";

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            switch (bund)
            {
                case WhichBasicBundle.E_SQLITE3:
                    f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: e_sqlite3 included");
                    break;
                case WhichBasicBundle.GREEN:
                    f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: iOS=system SQLite, others=e_sqlite3 included.  Note that this bundle is identical to bundle_e_sqlite3, except on iOS where it uses the system SQLite library instead of e_sqlite3.  In other words, when you use this bundle in a cross-platform app, your app is not using the same SQLite build configuration on all platforms.");
                    break;
                default: 
                    throw new NotImplementedException();
            }

            f.WriteStartElement("dependencies");

            switch (bund)
            {
                case WhichBasicBundle.E_SQLITE3:
                    write_bundle_dependency_group(f, WhichProvider.INTERNAL, WhichLib.E_SQLITE3, TFM.IOS);
                    write_bundle_dependency_group(f, WhichProvider.INTERNAL, WhichLib.E_SQLITE3, TFM.TVOS);
                    break;
                case WhichBasicBundle.GREEN:
#if SQLITE3_DYNAMIC
                    write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.NONE, TFM.IOS);
#else
                    write_bundle_dependency_group(f, WhichProvider.SQLITE3, WhichLib.NONE, TFM.IOS);
#endif
#if notyet
#if SQLITE3_DYNAMIC
                    write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.NONE, TFM.TVOS);
#else
                    write_bundle_dependency_group(f, WhichProvider.SQLITE3, WhichLib.NONE, TFM.TVOS);
#endif
#endif
                    break;
                default: 
                    throw new NotImplementedException();
            }

            write_bundle_dependency_group(f, WhichProvider.E_SQLITE3, WhichLib.E_SQLITE3, TFM.ANDROID);
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.E_SQLITE3, TFM.NET461);
            write_bundle_dependency_group(f, WhichProvider.E_SQLITE3, WhichLib.E_SQLITE3, TFM.NETSTANDARD20);
#if NETCOREAPP3_NATIVELIBRARY
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.E_SQLITE3, TFM.NETCOREAPP31);
#endif
            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            switch (bund)
            {
                case WhichBasicBundle.E_SQLITE3:
                    write_nuspec_file_entry_lib_batteries(
                            "e_sqlite3.internal.ios",
                            TFM.IOS,
                            f
                            );

                    write_nuspec_file_entry_lib_batteries(
                            "e_sqlite3.internal.tvos",
                            TFM.TVOS,
                            f
                            );
                    break;
                case WhichBasicBundle.GREEN:
#if SQLITE3_DYNAMIC
                    write_nuspec_file_entry_lib_batteries(
                            "sqlite3.dynamic",
                            tfm_build: TFM.NETSTANDARD20,
                            tfm_dest: TFM.IOS,
                            f
                            );
                    write_nuspec_file_entry_lib_mt(
                            "SQLitePCLRaw.nativelibrary",
                            tfm_build: TFM.NETSTANDARD20,
                            tfm_dest: TFM.IOS,
                            f
                            );
#if notyet
                    write_nuspec_file_entry_lib_batteries(
                            "sqlite3.dynamic",
                            tfm_build: TFM.NETSTANDARD20,
                            tfm_dest: TFM.TVOS,
                            f
                            );
                    write_nuspec_file_entry_lib_mt(
                            "SQLitePCLRaw.nativelibrary",
                            tfm_build: TFM.NETSTANDARD20,
                            tfm_dest: TFM.TVOS,
                            f
                            );
#endif
#else
                    write_nuspec_file_entry_lib_batteries(
                            "sqlite3.dllimport",
                            tfm_build: TFM.NETSTANDARD20,
                            tfm_dest: TFM.IOS,
                            f
                            );
#if notyet
                    write_nuspec_file_entry_lib_batteries(
                            "sqlite3.dllimport",
                            tfm_build: TFM.NETSTANDARD20,
                            tfm_dest: TFM.TVOS,
                            f
                            );
#endif
#endif

                    break;
                default: 
                    throw new NotImplementedException();
            }

            write_nuspec_file_entry_lib_batteries(
                    "e_sqlite3.dynamic",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NET461,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.nativelibrary",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NET461,
                    f
                    );

#if NETCOREAPP3_NATIVELIBRARY
            write_nuspec_file_entry_lib_batteries(
                    "e_sqlite3.dynamic",
                    tfm_build: TFM.NETSTANDARD20,
                    tfm_dest: TFM.NETCOREAPP31,
                    f
                    );
            write_nuspec_file_entry_lib_mt(
                    "SQLitePCLRaw.nativelibrary",
                    tfm_build: TFM.NETCOREAPP31,
                    tfm_dest: TFM.NETCOREAPP31,
                    f
                    );
#endif

            write_nuspec_file_entry_lib_batteries(
                    "e_sqlite3.dllimport",
                    TFM.NETSTANDARD20,
                    f
                    );

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
    }

    private static void gen_nuspec_bundle_sqlite3(
        string dir_src
        )
    {
        string bund_name = "sqlite3";
        string id = $"{gen.ROOT_NAME}.bundle_{bund_name}";

        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        var dir_proj = Path.Combine(dir_src, id);
        Directory.CreateDirectory(dir_proj);
        gen_dummy_csproj(dir_proj, id);

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, string.Format("{0}.nuspec", id)), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            f.WriteStartElement("metadata");
            write_nuspec_common_metadata(id, f);
            f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: always just DLLImport(sqlite3), aka, the 'system SQLite'");

            f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, WhichProvider.SQLITE3, WhichLib.NONE, TFM.NETSTANDARD20);
            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entry_lib_batteries(
                    "sqlite3.dllimport",
                    TFM.NETSTANDARD20,
                    f
                    );

            f.WriteEndElement(); // files

            f.WriteEndElement(); // package

            f.WriteEndDocument();
        }
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

    private static void gen_nuget_targets(string dest, WhichLib lib)
    {
        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

        using (XmlWriter f = XmlWriter.Create(dest, settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            f.WriteAttributeString("ToolsVersion", "4.0");

            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(OS)' == 'Windows_NT' ");
            write_nuget_target_item("win-x86", lib, f);
            write_nuget_target_item("win-x64", lib, f);
            write_nuget_target_item("win-arm", lib, f);
            f.WriteEndElement(); // ItemGroup

            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(OS)' == 'Unix' AND Exists('/Library/Frameworks') ");
            write_nuget_target_item("osx-x64", lib, f);
            f.WriteEndElement(); // ItemGroup

            f.WriteStartElement("ItemGroup");
            f.WriteAttributeString("Condition", " '$(OS)' == 'Unix' AND !Exists('/Library/Frameworks') ");
            write_nuget_target_item("linux-x86", lib, f);
            write_nuget_target_item("linux-x64", lib, f);
            write_nuget_target_item("linux-arm", lib, f);
            write_nuget_target_item("linux-armel", lib, f);
            write_nuget_target_item("linux-arm64", lib, f);
            write_nuget_target_item("linux-x64", lib, f);
            write_nuget_target_item("linux-mips64", lib, f);
            f.WriteEndElement(); // ItemGroup

            f.WriteEndElement(); // Project

            f.WriteEndDocument();
        }
    }

    public static void Main(string[] args)
    {
        string dir_root = Path.GetFullPath(args[0]);

        var dir_src = Path.Combine(dir_root, "src");

        gen_nuspec_lib_e_sqlite3(dir_src);
        gen_nuspec_lib_e_sqlcipher(dir_src);

        gen_nuspec_provider_e_sqlite3(dir_src);
        gen_nuspec_provider_e_sqlcipher(dir_src);
        gen_nuspec_provider_sqlcipher(dir_src);

        gen_nuspec_bundle_e_sqlite3_or_green(dir_src, WhichBasicBundle.GREEN);
        gen_nuspec_bundle_e_sqlite3_or_green(dir_src, WhichBasicBundle.E_SQLITE3);
        gen_nuspec_bundle_sqlite3(dir_src);
        gen_nuspec_bundle_winsqlite3(dir_src);
        gen_nuspec_bundle_e_sqlcipher(dir_src);
        gen_nuspec_bundle_zetetic(dir_src);
    }
}

