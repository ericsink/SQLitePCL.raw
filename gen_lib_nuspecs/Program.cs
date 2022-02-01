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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

public static class gen
{
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
        WhichLib lib,
        string cpu
        )
    {
        var dir_name = lib.AsString_basename_in_cb();
        var lib_name = lib.AsString_libname_in_cb(LibSuffix.DYLIB);
        return Path.Combine("$cb_bin_path$", dir_name, "mac", cpu, lib_name);
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

    static void write_nuspec_file_entry_native_win10(
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
            TFM.WIN10,
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

        write_nuspec_file_entry_native_win10(lib, win_toolset, "appcontainer", "arm64", "win10-arm64", f);
        write_nuspec_file_entry_native_win10(lib, win_toolset, "appcontainer", "arm", "win10-arm", f);
        write_nuspec_file_entry_native_win10(lib, win_toolset, "appcontainer", "x64", "win10-x64", f);
        write_nuspec_file_entry_native_win10(lib, win_toolset, "appcontainer", "x86", "win10-x86", f);

        write_nuspec_file_entry_native_mac(lib, "x86_64", "osx-x64", f);
        write_nuspec_file_entry_native_mac(lib, "arm64", "osx-arm64", f);

        write_nuspec_file_entry_native_linux(lib, "x64", "linux-x64", f);
        write_nuspec_file_entry_native_linux(lib, "x86", "linux-x86", f);
        write_nuspec_file_entry_native_linux(lib, "armhf", "linux-arm", f);
        write_nuspec_file_entry_native_linux(lib, "armsf", "linux-armel", f);
        write_nuspec_file_entry_native_linux(lib, "arm64", "linux-arm64", f);
        write_nuspec_file_entry_native_linux(lib, "musl-x64", "linux-musl-x64", f);
        write_nuspec_file_entry_native_linux(lib, "musl-x64", "alpine-x64", f);
        write_nuspec_file_entry_native_linux(lib, "mips64", "linux-mips64", f);
        write_nuspec_file_entry_native_linux(lib, "s390x", "linux-s390x", f);
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

            var tname = string.Format("{0}.targets", id);
            var path_targets = Path.Combine(dir_proj, tname);
            var relpath_targets = Path.Combine(".", tname);
            gen_nuget_targets(path_targets, WhichLib.E_SQLITE3);
            common.write_nuspec_file_entry(
                relpath_targets,
                string.Format("buildTransitive\\{0}", TFM.NET461.AsString()),
                f
                );

            // TODO need a comment here to explain these
            common.write_empty(f, TFM.NET461);
            common.write_empty(f, TFM.NETSTANDARD20);

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

            var tname = string.Format("{0}.targets", id);
            var path_targets = Path.Combine(dir_proj, tname);
            var relpath_targets = Path.Combine(".", tname);
            gen_nuget_targets(path_targets, WhichLib.E_SQLCIPHER);
            common.write_nuspec_file_entry(
                relpath_targets,
                string.Format("buildTransitive\\{0}", TFM.NET461.AsString()),
                f
                );

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
        var settings = common.XmlWriterSettings_default();
        settings.OmitXmlDeclaration = false;

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

    public static void Main(string[] args)
    {
        string dir_root = Path.GetFullPath(args[0]);

        var dir_src = Path.Combine(dir_root, "src");

        gen_nuspec_lib_e_sqlite3(dir_src);
        gen_nuspec_lib_e_sqlcipher(dir_src);
    }
}

