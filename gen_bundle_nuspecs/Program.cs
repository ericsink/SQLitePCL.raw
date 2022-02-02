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
    private static void write_nuspec_file_entry_lib(string src, TFM tfm, XmlWriter f)
    {
        common.write_nuspec_file_entry(
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
            case TFM.XAMARIN_ANDROID:
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
        common.write_nuspec_file_entry(
            make_mt_path(name, tfm),
            string.Format("lib\\{0}\\", tfm.AsString()),
            f
            );
    }

    private static void write_nuspec_file_entry_lib_mt(string name, TFM tfm_build, TFM tfm_dest, XmlWriter f)
    {
        common.write_nuspec_file_entry(
            make_mt_path(name, tfm_build),
            string.Format("lib\\{0}\\", tfm_dest.AsString()),
            f
            );
    }

    private static void write_nuspec_file_entry_lib_mt(string dir_name, string assembly_name, TFM tfm_build, TFM tfm_dest, XmlWriter f)
    {
        common.write_nuspec_file_entry(
            make_mt_path(dir_name, assembly_name, tfm_build),
            string.Format("lib\\{0}\\", tfm_dest.AsString()),
            f
            );
    }

    private static void write_nuspec_file_entry_lib_batteries(string basename, TFM tfm_build, TFM tfm_dest, XmlWriter f)
    {
        common.write_nuspec_file_entry(
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
        string id = string.Format("{0}.bundle_winsqlite3", common.ROOT_NAME);

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
            f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: .no SQLite library included, uses winsqlite3.dll from Windows 10.");

            f.WriteStartElement("dependencies");

            // use dllimport provider
            write_bundle_dependency_group(f, WhichProvider.WINSQLITE3, WhichLib.NONE, TFM.NETSTANDARD20);
            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            // use dllimport provider
            write_nuspec_file_entry_lib_batteries(
                    "winsqlite3.dllimport",
                    TFM.NETSTANDARD20,
                    f
                    );

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

        common.add_dep_core(f);

        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", string.Format("{0}.provider.{1}", common.ROOT_NAME, prov.AsString()));
        f.WriteAttributeString("version", "$version$");
        f.WriteEndElement(); // dependency

        if (what == WhichLib.E_SQLITE3)
        {
            var id = tfm.GetOS() switch
            {
                XamarinOS.IOS => string.Format("{0}.lib.e_sqlite3.ios", common.ROOT_NAME),
                XamarinOS.TVOS => string.Format("{0}.lib.e_sqlite3.tvos", common.ROOT_NAME),
                XamarinOS.ANDROID => string.Format("{0}.lib.e_sqlite3.android", common.ROOT_NAME),
                XamarinOS.NONE => string.Format("{0}.lib.e_sqlite3", common.ROOT_NAME),
            };
            f.WriteStartElement("dependency");
            f.WriteAttributeString("id", id);
            f.WriteAttributeString("version", "$version$");
            f.WriteEndElement(); // dependency
        }
        else if (what == WhichLib.E_SQLCIPHER)
        {
            var id = tfm.GetOS() switch
            {
                XamarinOS.IOS => string.Format("{0}.lib.e_sqlcipher.ios", common.ROOT_NAME),
                XamarinOS.TVOS => string.Format("{0}.lib.e_sqlcipher.tvos", common.ROOT_NAME),
                XamarinOS.ANDROID => string.Format("{0}.lib.e_sqlcipher.android", common.ROOT_NAME),
                XamarinOS.NONE => string.Format("{0}.lib.e_sqlcipher", common.ROOT_NAME),
            };
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
        var id = string.Format("{0}.bundle_e_sqlcipher", common.ROOT_NAME);

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
            f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: unofficial open source sqlcipher builds included.  Note that these sqlcipher builds are unofficial and unsupported.  For official sqlcipher builds, contact Zetetic.");

            f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, WhichProvider.INTERNAL, WhichLib.E_SQLCIPHER, TFM.XAMARIN_IOS);
            write_bundle_dependency_group(f, WhichProvider.E_SQLCIPHER, WhichLib.E_SQLCIPHER, TFM.XAMARIN_ANDROID);
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.E_SQLCIPHER, TFM.NET461);
            write_bundle_dependency_group(f, WhichProvider.E_SQLCIPHER, WhichLib.E_SQLCIPHER, TFM.NETSTANDARD20);

            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entry_lib_batteries(
                    "e_sqlcipher.internal.ios",
                    TFM.XAMARIN_IOS,
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
        var id = string.Format("{0}.bundle_zetetic", common.ROOT_NAME);

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
            f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: reference the official SQLCipher builds from Zetetic, which are not included in this package");

            f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, WhichProvider.INTERNAL, WhichLib.NONE, TFM.XAMARIN_IOS);
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.NONE, TFM.NET461);
            write_bundle_dependency_group(f, WhichProvider.SQLCIPHER, WhichLib.NONE, TFM.NETSTANDARD20);

            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            write_nuspec_file_entry_lib_batteries(
                    "sqlcipher.internal.ios",
                    TFM.XAMARIN_IOS,
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
        string id = $"{common.ROOT_NAME}.bundle_{bund_name}";

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
                    write_bundle_dependency_group(f, WhichProvider.INTERNAL, WhichLib.E_SQLITE3, TFM.XAMARIN_IOS);
                    write_bundle_dependency_group(f, WhichProvider.INTERNAL, WhichLib.E_SQLITE3, TFM.TVOS);
                    break;
                case WhichBasicBundle.GREEN:
                    write_bundle_dependency_group(f, WhichProvider.SQLITE3, WhichLib.NONE, TFM.XAMARIN_IOS);
#if notyet
                    write_bundle_dependency_group(f, WhichProvider.SQLITE3, WhichLib.NONE, TFM.TVOS);
#endif
                    break;
                default: 
                    throw new NotImplementedException();
            }

            write_bundle_dependency_group(f, WhichProvider.E_SQLITE3, WhichLib.E_SQLITE3, TFM.XAMARIN_ANDROID);
            write_bundle_dependency_group(f, WhichProvider.DYNAMIC_CDECL, WhichLib.E_SQLITE3, TFM.NET461);
            write_bundle_dependency_group(f, WhichProvider.E_SQLITE3, WhichLib.E_SQLITE3, TFM.NETSTANDARD20);
            f.WriteEndElement(); // dependencies

            f.WriteEndElement(); // metadata

            f.WriteStartElement("files");

            switch (bund)
            {
                case WhichBasicBundle.E_SQLITE3:
                    write_nuspec_file_entry_lib_batteries(
                            "e_sqlite3.internal.ios",
                            TFM.XAMARIN_IOS,
                            f
                            );

                    write_nuspec_file_entry_lib_batteries(
                            "e_sqlite3.internal.tvos",
                            TFM.TVOS,
                            f
                            );
                    break;
                case WhichBasicBundle.GREEN:
                    write_nuspec_file_entry_lib_batteries(
                            "sqlite3.dllimport",
                            tfm_build: TFM.NETSTANDARD20,
                            tfm_dest: TFM.XAMARIN_IOS,
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
        string id = $"{common.ROOT_NAME}.bundle_{bund_name}";

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

    public static void Main(string[] args)
    {
        string dir_root = Path.GetFullPath(args[0]);

        var dir_src = Path.Combine(dir_root, "src");

        gen_nuspec_bundle_e_sqlite3_or_green(dir_src, WhichBasicBundle.GREEN);
        gen_nuspec_bundle_e_sqlite3_or_green(dir_src, WhichBasicBundle.E_SQLITE3);
        gen_nuspec_bundle_sqlite3(dir_src);
        gen_nuspec_bundle_winsqlite3(dir_src);
        gen_nuspec_bundle_e_sqlcipher(dir_src);
        gen_nuspec_bundle_zetetic(dir_src);
    }
}

