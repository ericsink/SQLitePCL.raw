/*
   Copyright 2014-2019 Zumero, LLC

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
    public const string ROOT_NAME = "SQLitePCLRaw";

	enum TFM
	{
		NONE,
		IOS,
		ANDROID,
		UWP,
		NETSTANDARD11,
		NETSTANDARD20,
		NET35,
		NET40,
		NET45,
		XAMARIN_MAC,
		NETCOREAPP,
	}

	static string AsString(this WhichLib e)
	{
		switch (e)
		{
			case WhichLib.E_SQLITE3: return "e_sqlite3";
			case WhichLib.SQLCIPHER: return "sqlcipher";
			default:
				throw new NotImplementedException(string.Format("WhichLib.AsString for {0}", e));
		}
	}

	static string AsString(this TFM e)
	{
		switch (e)
		{
			case TFM.NONE: throw new Exception("TFM.NONE.AsString()");
			case TFM.IOS: return "Xamarin.iOS10";
			case TFM.ANDROID: return "MonoAndroid";
			case TFM.UWP: return "uap10.0";
			case TFM.NETSTANDARD11: return "netstandard1.1";
			case TFM.NETSTANDARD20: return "netstandard2.0";
			case TFM.XAMARIN_MAC: return "Xamarin.Mac20";
			case TFM.NET35: return "net35";
			case TFM.NET40: return "net40";
			case TFM.NET45: return "net45";
			case TFM.NETCOREAPP: return "netcoreapp";
			default:
				throw new NotImplementedException(string.Format("TFM.AsString for {0}", e));
		}
	}

	static TFM str_to_tfm(string s)
	{
		switch (s.ToLower().Trim())
		{
			case "netstandard1.1": return TFM.NETSTANDARD11;
			case "netstandard2.0": return TFM.NETSTANDARD20;
			default:
				throw new NotImplementedException(string.Format("str_to_tfm not found: {0}", s));
		}
	}

    static string rid_front_half(string toolset)
    {
        switch (toolset)
        {
            case "v110_xp":
				// for our builds, v110_xp should always correspond to a win-whatever RID
                return "win";
            case "v110":
                return "win8";
            case "v110_wp80":
                return "wp80";
            case "v120":
                return "win81";
            case "v120_wp81":
                return "wpa81";
            case "v140":
                return "win10";
            default:
                throw new Exception();
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

	private static void write_nuspec_file_entry_native(string src, string rid, XmlWriter f)
	{
		write_nuspec_file_entry(
			src,
			string.Format("runtimes\\{0}\\native\\", rid),
			f
			);
	}

	private static void write_nuspec_file_entry_nativeassets(string src, string rid, TFM tfm, XmlWriter f)
	{
		write_nuspec_file_entry(
			src,
			string.Format("runtimes\\{0}\\nativeassets\\{1}\\", rid, tfm.AsString()),
			f
			);
	}

	private static void write_empty(XmlWriter f, string top, TFM tfm)
    {
		f.WriteComment("empty directory in lib to avoid nuget adding a reference");

		Directory.CreateDirectory(Path.Combine(Path.Combine(top, "empty"), tfm.AsString()));

		f.WriteStartElement("file");
		f.WriteAttributeString("src", string.Format("empty\\{0}\\", tfm.AsString()));
		f.WriteAttributeString("target", string.Format("lib\\{0}", tfm.AsString()));
		f.WriteEndElement(); // file
    }

	public const int MAJOR_VERSION = 1;
	public const int MINOR_VERSION = 1;
	public const int PATCH_VERSION = 14;
	public static string NUSPEC_VERSION_PRE = string.Format("{0}.{1}.{2}-pre{3}", 
		MAJOR_VERSION,
		MINOR_VERSION,
		PATCH_VERSION,
		DateTime.Now.ToString("yyyyMMddHHmmss")
		); 
	public static string NUSPEC_VERSION_RELEASE = string.Format("{0}.{1}.{2}",
		MAJOR_VERSION,
		MINOR_VERSION,
		PATCH_VERSION
		);
	public static string NUSPEC_VERSION = NUSPEC_VERSION_PRE;
	public static string ASSEMBLY_VERSION = string.Format("{0}.{1}.{2}.{3}", 
		MAJOR_VERSION,
		MINOR_VERSION,
		PATCH_VERSION,
		(int) ((DateTime.Now - new DateTime(2018,1,1)).TotalDays) 
		); 

	private const string NUSPEC_RELEASE_NOTES = "1.1.13:  fix problems with unofficial sqlcipher builds for Android and iOS.  use new license tag for nuspecs.  1.1.12:  update e_sqlite3 builds to 3.26.0.  bug fix for bundle_zetetic on iOS.  1.1.11:  put a copy of alpine-x64/e_sqlite3 into linux-musl-x64, for .NET Core 2.1.  1.1.10:  improve bundle_zetetic.  update e_sqlite3 to 3.22.0 and turn on FTS5.  fix bundled sqlcipher build for UWP.  AssemblyVersion now being updated properly.  attempt fix crash involving CLR finalizer.  add e_sqlite3 builds for linux-arm64 and alpine-x64.  change generic Windows builds to use win-foo instead of win7-foo.  add support for SQLITE_DETERMINISTIC.  added sqlite3_blob_open overload to support higher perf in certain cases.  fix problem with Mac-but-not-Xamarin and targets file.   1.1.9:  bug fixes for Xamarin.Mac.  add a sqlcipher build for UWP.  1.1.8:  SQLite builds for .NET Core ARM, linux and Windows IoT.  Finalizers.  Fix Xam.Mac issue with bundle_green.  Fix edge case in one of the sqlite3_column_blob() overloads.  New 'bundle_zetetic' for use with official SQLCipher builds from Zetetic.  1.1.7:  Drop SQLite down to 3.18.2.  1.1.6:  AssetTargetFallback fixes.  update sqlite builds to 3.19.3.  1.1.5:  bug fix path in lib.foo.linux targets file.  1.1.4:  tweak use of nuget .targets files for compat with .NET Core.  1.1.3:  add SQLITE_CHECKPOINT_TRUNCATE symbol definition.  add new blob overloads to enable better performance in certain cases.  chg winsqlite3 to use StdCall.  fix targets files for better compat with VS 2017 nuget pack.  add 32-bit linux build for e_sqlite3.  update to latest libcrypto builds from couchbase folks.  1.1.2:  ability to FreezeProvider().  update e_sqlite3 builds to 3.16.1.  1.1.1:  add support for config_log.  update e_sqlite3 builds to 3.15.2.  fix possible memory corruption when using prepare_v2() with multiple statements.  better errmsg from ugly.step().  add win8 dep groups in bundles.  fix batteries_v2.Init() to be 'last call wins' like the v1 version is.  chg raw.SetProvider() to avoid calling sqlite3_initialize() so that sqlite3_config() can be used.  better support for Xamarin.Mac.  1.1.0:  fix problem with winsqlite3 on UWP.  remove iOS Classic support.  add sqlite3_enable_load_extension.  add sqlite3_config/initialize/shutdown.  add Batteries_V2.Init().  1.0.1:  fix problem with bundle_e_sqlite3 on iOS.  fix issues with .NET Core.  add bundle_sqlcipher.  1.0.0 release:  Contains minor breaking changes since 0.9.x.  All package names now begin with SQLitePCLRaw.  Now supports netstandard.  Fixes for UWP and Android N.  Change all unit tests to xunit.  Support for winsqlite3.dll and custom SQLite builds.";

    private static void add_dep_core(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", string.Format("{0}.core", gen.ROOT_NAME));
        f.WriteAttributeString("version", NUSPEC_VERSION);
        f.WriteEndElement(); // dependency
    }

    private static void add_dep_netstandard(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", "NETStandard.Library");
        f.WriteAttributeString("version", "1.6.0");
        f.WriteEndElement(); // dependency
    }

    private const int DEP_NONE = 0;
    private const int DEP_CORE = 1;

    private static void write_dependency_group(XmlWriter f, TFM tfm, int flags)
    {
        f.WriteStartElement("group");
        if (tfm != TFM.NONE)
        {
            f.WriteAttributeString("targetFramework", tfm.AsString());
            switch (tfm)
            {
                case TFM.UWP:
                case TFM.NETSTANDARD11:
                    add_dep_netstandard(f);
                    break;
            }
        }
        if ((flags & DEP_CORE) != 0)
        {
            add_dep_core(f);
        }
        f.WriteEndElement(); // group
    }

	class dll_info
	{
		public string project_subdir {get;set;}
		public string config {get;set;}
		public string dll {get;set;}
		public string tfm_dir_name {get;set;}
		public TFM tfm => str_to_tfm(tfm_dir_name);
		public string get_src_path(string dir)
		{
			return Path.Combine(
				dir,
				project_subdir,
				"bin",
				config,
				tfm_dir_name,
				dll
				);
		}
	}

	static List<dll_info> find_dlls(
		string dir
		)
	{
		var a = new List<dll_info>();
		foreach (var dir_project in Directory.GetDirectories(dir, "SQLitePCLRaw.*"))
		{
			var project_name = Path.GetFileName(dir_project);
			var dir_bin = Path.Combine(dir_project, "bin");
			if (Directory.Exists(dir_bin))
			{
				foreach (var dir_config in Directory.GetDirectories(dir_bin))
				{
					foreach (var dir_tfm in Directory.GetDirectories(dir_config))
					{
						foreach (var dll_path in Directory.GetFiles(dir_tfm, "*.dll"))
						{
							var config_name = Path.GetFileName(dir_config);
							var target_name = Path.GetFileName(dir_tfm);
							var dll_name = Path.GetFileName(dll_path);
							System.Console.WriteLine("{0} - {1} - {2} - {3}", project_name, config_name, target_name, dll_name);
							a.Add(
								new dll_info
								{
									project_subdir = project_name,
									config = config_name,
									tfm_dir_name = target_name,
									dll = dll_name,
								}
								);
						}
					}
				}
			}
		}
		return a;
	}

	private static void write_nuspec_common_metadata(
		string id,
		XmlWriter f
		)
	{
		f.WriteAttributeString("minClientVersion", "2.8.1");
		f.WriteElementString("id", id);
		f.WriteElementString("version", NUSPEC_VERSION);
		f.WriteElementString("authors", "Eric Sink, et al");
		f.WriteElementString("owners", "Eric Sink");
		f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
		f.WriteElementString("requireLicenseAcceptance", "false");
		write_license(f);
		f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
		f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
		f.WriteElementString("tags", "sqlite pcl database xamarin monotouch ios monodroid android wp8 wpa netstandard uwp");
		f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
	}

	private static void gen_nuspec_core(string top, string root, string dir_mt, List<dll_info> dlls)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

        string id = string.Format("{0}.core", gen.ROOT_NAME);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
            f.WriteElementString("title", id);
			f.WriteElementString("description", "SQLitePCL.raw is a Portable Class Library (PCL) for low-level (raw) access to SQLite.  This package does not provide an API which is friendly to app developers.  Rather, it provides an API which handles platform and configuration issues, upon which a friendlier API can be built.  In order to use this package, you will need to also add one of the SQLitePCLRaw.provider.* packages and call raw.SetProvider().  Convenience packages are named SQLitePCLRaw.bundle_*.");

			f.WriteStartElement("dependencies");

            write_dependency_group(f, TFM.ANDROID, DEP_NONE);
            write_dependency_group(f, TFM.IOS, DEP_NONE);
            write_dependency_group(f, TFM.XAMARIN_MAC, DEP_NONE);
            write_dependency_group(f, TFM.NET35, DEP_NONE);
            write_dependency_group(f, TFM.NET40, DEP_NONE);
            write_dependency_group(f, TFM.NET45, DEP_NONE);
            write_dependency_group(f, TFM.UWP, DEP_NONE);
            write_dependency_group(f, TFM.NETSTANDARD11, DEP_NONE);
            write_dependency_group(f, TFM.NONE, DEP_NONE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (
				var dll in dlls
					.Where(d => d.project_subdir == "SQLitePCLRaw.core")
				)
			{
				write_nuspec_file_entry_lib(
						dll.get_src_path(dir_mt), 
						dll.tfm,
						f
						);
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	static string make_cb_path_win(
		string cb_bin,
		WhichLib lib,
		string toolset,
		string flavor,
		string arch
		)
	{
		var name = lib.AsString();
		return Path.Combine(cb_bin, name, "win", toolset, flavor, arch, string.Format("{0}.dll", name));
	}

	static string make_cb_path_linux(
		string cb_bin,
		WhichLib lib,
		string cpu
		)
	{
		var name = lib.AsString();
		return Path.Combine(cb_bin, name, "linux", cpu, string.Format("lib{0}.so", name));
	}

	static string make_cb_path_mac(
		string cb_bin,
		WhichLib lib
		)
	{
		var name = lib.AsString();
		return Path.Combine(cb_bin, name, "mac", string.Format("lib{0}.dylib", name));
	}

	private static void gen_nuspec_e_sqlite3_win(string top, string cb_bin)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = string.Format("SQLitePCLRaw.lib.e_sqlite3.windows");
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			Action<string,string,string,string> write_file_entry = (a_toolset, flavor, arch, rid) =>
			{
				write_nuspec_file_entry_native(
					make_cb_path_win(cb_bin, WhichLib.E_SQLITE3, a_toolset, flavor, arch),
					rid,
					f
					);
			};

			write_file_entry("v140", "plain", "x86", "win-x86");
			write_file_entry("v140", "plain", "x64", "win-x64");
			write_file_entry("v140", "plain", "arm", "win8-arm");
			write_file_entry("v140", "appcontainer", "arm", "win10-arm");
			write_file_entry("v140", "appcontainer", "x64", "win10-x64");
			write_file_entry("v140", "appcontainer", "x86", "win10-x86");

#if TODO // targets file
			string tname;
			switch (toolset) {
				case "v110_xp":
					tname = gen_nuget_targets_pinvoke_anycpu(top, id, toolset);
                    if (tname != null) 
                    {
                        f.WriteStartElement("file");
                        f.WriteAttributeString("src", tname);
                        f.WriteAttributeString("target", string.Format("build\\net35\\{0}.targets", id));
                        f.WriteEndElement(); // file

                        write_empty(f, top, TFM.NET35);
                        write_empty(f, top, TFM.NETSTANDARD11);
                        write_empty(f, top, TFM.NETSTANDARD20);
                    }
					break;
				default:
					tname = gen_nuget_targets_sqlite3_itself(top, id, toolset);
                    if (tname != null) 
                    {
                        f.WriteStartElement("file");
                        f.WriteAttributeString("src", tname);
                        f.WriteAttributeString("target", string.Format("build\\{0}.targets", id));
                        f.WriteEndElement(); // file
                    }
					break;
			}
#endif

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	class config_embedded
	{
		public string id {get;set;}
		public string src {get;set;}
		public TFM tfm {get;set;}
	}

	private static void gen_nuspec_embedded(
		string top,
		config_embedded cfg
		)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		var id = cfg.id;
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

            write_nuspec_file_entry_lib(
					cfg.src,
					cfg.tfm,
                    f
                    );

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_e_sqlite3_otherplat(string top, string cb_bin, string plat)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = string.Format("SQLitePCLRaw.lib.e_sqlite3.{0}", plat);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("title", string.Format("Native code only (e_sqlite3, {0}) for SQLitePCLRaw", plat));
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			string tname = string.Format("{0}.targets", id);
			Action<string, string> write_linux_item = 
			(cpu, rid) =>
			{
				write_nuspec_file_entry_native(
					make_cb_path_linux(cb_bin, WhichLib.E_SQLITE3, cpu),
					rid,
					f
					);
			};

            switch (plat)
            {
                case "osx":
					write_nuspec_file_entry_native(
						make_cb_path_mac(cb_bin, WhichLib.E_SQLITE3),
						"osx-x64",
						f
						);
                    gen_nuget_targets_osx(top, tname, "libe_sqlite3.dylib", forxammac: false);
                    break;
                case "linux":
					write_linux_item("x64", "linux-x64");
					write_linux_item("x86", "linux-x86");
					write_linux_item("armhf", "linux-arm");
					write_linux_item("armsf", "linux-armel");
					write_linux_item("arm64", "linux-arm64");
					write_linux_item("musl-x64", "linux-musl-x64");
					write_linux_item("musl-x64", "alpine-x64");

                    gen_nuget_targets_linux(top, tname, "libe_sqlite3.so");
                    break;
                default:
                    throw new Exception();
            }

            f.WriteStartElement("file");
            f.WriteAttributeString("src", tname);
            f.WriteAttributeString("target", string.Format("build\\net35\\{0}.targets", id));
            f.WriteEndElement(); // file

            if (plat == "osx")
            {
                write_empty(f, top, TFM.XAMARIN_MAC);
                tname = string.Format("{0}.Xamarin.Mac20.targets", id);
                gen_nuget_targets_osx(top, tname, "libe_sqlite3.dylib", forxammac: true);

                f.WriteStartElement("file");
                f.WriteAttributeString("src", tname);
                f.WriteAttributeString("target", string.Format("build\\Xamarin.Mac20\\{0}.targets", id));
                f.WriteEndElement(); // file
            }

            write_empty(f, top, TFM.NET35);
            write_empty(f, top, TFM.NETSTANDARD11);
            write_empty(f, top, TFM.NETSTANDARD20);

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_sqlcipher(string top, string cb_bin, string plat)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = string.Format("SQLitePCLRaw.lib.sqlcipher.{0}", plat);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("title", string.Format("Native code only (sqlcipher, {0}) for SQLitePCLRaw", plat));
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLCipher (see sqlcipher/sqlcipher on GitHub) for use with SQLitePCL.raw.  The build of SQLCipher packaged here is built and maintained by Couchbase (see couchbaselabs/couchbase-lite-libsqlcipher on GitHub).  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.sqlcipher.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			string tname = string.Format("{0}.targets", id);
			switch (plat) {
				case "windows":
					write_nuspec_file_entry_native(
						make_cb_path_win(cb_bin, WhichLib.SQLCIPHER, "v140", "plain", "x86"),
						"win-x86",
						f
						);

					write_nuspec_file_entry_native(
						make_cb_path_win(cb_bin, WhichLib.SQLCIPHER, "v140", "plain", "x64"),
						"win-x64",
						f
						);

					write_nuspec_file_entry_native(
						make_cb_path_win(cb_bin, WhichLib.SQLCIPHER, "v140", "plain", "arm"),
						"win-arm", // TODO the other one uses win8-arm
						f
						);

					write_nuspec_file_entry_nativeassets(
						make_cb_path_win(cb_bin, WhichLib.SQLCIPHER, "v140", "appcontainer", "x64"),
						"win10-x64",
						TFM.UWP,
						f
						);

					write_nuspec_file_entry_nativeassets(
						make_cb_path_win(cb_bin, WhichLib.SQLCIPHER, "v140", "appcontainer", "x86"),
						"win10-x86",
						TFM.UWP,
						f
						);

					write_nuspec_file_entry_nativeassets(
						make_cb_path_win(cb_bin, WhichLib.SQLCIPHER, "v140", "appcontainer", "arm"),
						"win10-arm",
						TFM.UWP,
						f
						);

					gen_nuget_targets_windows(top, tname, "sqlcipher.dll");
					break;
				case "osx":
					write_nuspec_file_entry_native(
						make_cb_path_mac(cb_bin, WhichLib.SQLCIPHER),
						"osx-x64",
						f
						);

					gen_nuget_targets_osx(top, tname, "libsqlcipher.dylib", forxammac: false);
					break;
				case "linux":
					write_nuspec_file_entry_native(
						make_cb_path_linux(cb_bin, WhichLib.SQLCIPHER, "x64"),
						"linux-x64",
						f
						);

					write_nuspec_file_entry_native(
						make_cb_path_linux(cb_bin, WhichLib.SQLCIPHER, "x86"),
						"linux-x86",
						f
						);

					// TODO arm?

					// TODO musl?

					gen_nuget_targets_linux(top, tname, "libsqlcipher.so");
					break;
				default:
					throw new Exception();
			}
			f.WriteStartElement("file");
			f.WriteAttributeString("src", tname);
			f.WriteAttributeString("target", string.Format("build\\net35\\{0}.targets", id));
			f.WriteEndElement(); // file

            if (plat == "osx")
            {
                write_empty(f, top, TFM.XAMARIN_MAC);
                tname = string.Format("{0}.Xamarin.Mac20.targets", id);
                gen_nuget_targets_osx(top, tname, "libsqlcipher.dylib", forxammac: true);

                f.WriteStartElement("file");
                f.WriteAttributeString("src", tname);
                f.WriteAttributeString("target", string.Format("build\\Xamarin.Mac20\\{0}.targets", id));
                f.WriteEndElement(); // file
            }

            write_empty(f, top, TFM.NET35);
            write_empty(f, top, TFM.UWP);
            write_empty(f, top, TFM.NETSTANDARD11);
            write_empty(f, top, TFM.NETSTANDARD20);

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

	private static void gen_nuspec_ugly(string top, string dir_mt, List<dll_info> dlls)
	{
		string id = string.Format("{0}.ugly", gen.ROOT_NAME);

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "These extension methods for SQLitePCL.raw provide a more usable API while remaining stylistically similar to the sqlite3 C API, which most C# developers would consider 'ugly'.  This package exists for people who (1) really like the sqlite3 C API, and (2) really like C#.  So far, evidence suggests that 100% of the people matching both criteria are named Eric Sink, but this package is available just in case he is not the only one of his kind.");

			f.WriteStartElement("dependencies");

            write_dependency_group(f, TFM.ANDROID, DEP_CORE);
            write_dependency_group(f, TFM.IOS, DEP_CORE);
            write_dependency_group(f, TFM.XAMARIN_MAC, DEP_CORE);
            write_dependency_group(f, TFM.NET35, DEP_CORE);
            write_dependency_group(f, TFM.NET40, DEP_CORE);
            write_dependency_group(f, TFM.NET45, DEP_CORE);
            write_dependency_group(f, TFM.UWP, DEP_CORE);
            write_dependency_group(f, TFM.NETSTANDARD11, DEP_CORE);
            write_dependency_group(f, TFM.NONE, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (
				var dll in dlls
					.Where(d => d.project_subdir == "SQLitePCLRaw.ugly")
				)
			{
				write_nuspec_file_entry_lib(
						dll.get_src_path(dir_mt), 
						dll.tfm,
						f
						);
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_bundle_winsqlite3(string top, string dir_mt, List<dll_info> dlls)
    {
		string id = string.Format("{0}.bundle_winsqlite3", gen.ROOT_NAME);

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: .no SQLite library included, uses winsqlite3.dll");

			f.WriteStartElement("dependencies");

			// --------
			f.WriteStartElement("group");
			f.WriteAttributeString("targetFramework", TFM.UWP.AsString());

			f.WriteStartElement("dependency");
			f.WriteAttributeString("id", string.Format("{0}.core", gen.ROOT_NAME));
			f.WriteAttributeString("version", NUSPEC_VERSION);
			f.WriteEndElement(); // dependency

			f.WriteEndElement(); // group

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (
				var dll in dlls
					.Where(d => d.project_subdir == "SQLitePCLRaw.batteries_v2.winsqlite3")
				)
			{
				write_nuspec_file_entry_lib(
						dll.get_src_path(dir_mt), 
						dll.tfm,
						f
						);
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	enum IncludeLib
	{
		YES,
		NO,
	}

	enum WhichLib
	{
		NONE,
		E_SQLITE3,
		SQLCIPHER,
	}

    private static void write_bundle_dependency_group(XmlWriter f, TFM tfm, WhichLib what)
    {
        write_bundle_dependency_group(f, tfm, tfm, what, IncludeLib.YES);
    }

    private static void write_bundle_dependency_group(XmlWriter f, TFM tfm, WhichLib what, IncludeLib lib)
    {
        write_bundle_dependency_group(f, tfm, tfm, what, lib);
    }

    private static void write_bundle_dependency_group(XmlWriter f, TFM tfm_target, TFM tfm_deps, WhichLib what, IncludeLib lib)
    {
        f.WriteStartElement("group");
        f.WriteAttributeString("targetFramework", tfm_target.AsString());

        add_dep_core(f);

        if (lib == IncludeLib.YES)
        {
        if (what == WhichLib.E_SQLITE3)
        {
            switch (tfm_deps)
            {
                case TFM.ANDROID:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.android", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case TFM.XAMARIN_MAC:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case TFM.IOS:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.ios", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case TFM.NET35:
                case TFM.NET40:
                case TFM.NET45:
                case TFM.NETSTANDARD11: // TODO because this is used for netcoreapp, kinda hackish
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.windows", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency

                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency

                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.linux", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                default:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.windows", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
            }
        }
        else if (what == WhichLib.SQLCIPHER)
        {
            switch (tfm_deps)
            {
                case TFM.ANDROID:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.android", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case TFM.XAMARIN_MAC:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case TFM.IOS:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.ios", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case TFM.NET35:
                case TFM.NET40:
                case TFM.NET45:
                case TFM.NETSTANDARD11: // TODO because this is used for netcoreapp, kinda hackish
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.windows", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency

                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency

                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.linux", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case TFM.UWP:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.windows", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                default:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.windows", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
            }
        }
        }

        f.WriteEndElement(); // group
    }

	enum SQLCipherBundleKind
	{
		Unofficial,
		Zetetic,
	}

	private static void gen_nuspec_bundle_sqlcipher(string top, SQLCipherBundleKind kind, string dir_mt, List<dll_info> dlls)
    {
		string id;
		switch (kind)
		{
			case SQLCipherBundleKind.Unofficial:
				id = string.Format("{0}.bundle_sqlcipher", gen.ROOT_NAME);
				break;
			case SQLCipherBundleKind.Zetetic:
				id = string.Format("{0}.bundle_zetetic", gen.ROOT_NAME);
				break;
			default:
				throw new NotImplementedException();
		}

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("title", id);
			switch (kind)
			{
				case SQLCipherBundleKind.Unofficial:
					f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: unofficial open source sqlcipher builds included.  Note that these sqlcipher builds are unofficial and unsupported.  For official sqlcipher builds, contact Zetetic.");
					break;
				case SQLCipherBundleKind.Zetetic:
					f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: reference the official SQLCipher builds from Zetetic, which are not included in this package");
					break;
				default:
					throw new NotImplementedException();
			}

			f.WriteStartElement("dependencies");

			IncludeLib lib_deps;
			switch (kind)
			{
				case SQLCipherBundleKind.Unofficial:
					lib_deps = IncludeLib.YES;
					break;
				case SQLCipherBundleKind.Zetetic:
					lib_deps = IncludeLib.NO;
					break;
				default:
					throw new NotImplementedException();
			}

            write_bundle_dependency_group(f, TFM.ANDROID, WhichLib.SQLCIPHER, lib_deps);
            write_bundle_dependency_group(f, TFM.IOS, WhichLib.SQLCIPHER, lib_deps);
            write_bundle_dependency_group(f, TFM.XAMARIN_MAC, WhichLib.SQLCIPHER, lib_deps);
            write_bundle_dependency_group(f, TFM.NET35, WhichLib.SQLCIPHER, lib_deps);
            write_bundle_dependency_group(f, TFM.NET40, WhichLib.SQLCIPHER, lib_deps);
            write_bundle_dependency_group(f, TFM.NET45, WhichLib.SQLCIPHER, lib_deps);
            write_bundle_dependency_group(f, TFM.NETCOREAPP, TFM.NETSTANDARD11, WhichLib.SQLCIPHER, lib_deps);
            write_bundle_dependency_group(f, TFM.UWP, WhichLib.SQLCIPHER);
            
            write_dependency_group(f, TFM.NETSTANDARD11, DEP_CORE);
            write_dependency_group(f, TFM.NONE, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (
				var dll in dlls
					.Where(d => d.project_subdir == "SQLitePCLRaw.batteries_v2.sqlcipher")
				)
			{
				write_nuspec_file_entry_lib(
						dll.get_src_path(dir_mt), 
						dll.tfm,
						f
						);
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_bundle_e_sqlite3(string top, string dir_mt, List<dll_info> dlls)
	{
		string id = string.Format("{0}.bundle_e_sqlite3", gen.ROOT_NAME);

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: e_sqlite3 included");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, TFM.ANDROID, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.IOS, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.XAMARIN_MAC, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.UWP, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.NET35, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.NET40, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.NET45, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.NETCOREAPP, TFM.NETSTANDARD11, WhichLib.E_SQLITE3, IncludeLib.YES);
            
            write_dependency_group(f, TFM.NETSTANDARD11, DEP_CORE);
            write_dependency_group(f, TFM.NONE, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (
				var dll in dlls
					.Where(d => d.project_subdir == "SQLitePCLRaw.batteries_v2.e_sqlite3")
				)
			{
				write_nuspec_file_entry_lib(
						dll.get_src_path(dir_mt), 
						dll.tfm,
						f
						);
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_bundle_green(string top, string dir_mt, List<dll_info> dlls)
	{
		string id = string.Format("{0}.bundle_green", gen.ROOT_NAME);

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: iOS=system SQLite, others=e_sqlite3 included");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, TFM.ANDROID, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.IOS, WhichLib.NONE);
            write_bundle_dependency_group(f, TFM.XAMARIN_MAC, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.UWP, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.NET35, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.NET40, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.NET45, WhichLib.E_SQLITE3);
            write_bundle_dependency_group(f, TFM.NETCOREAPP, TFM.NETSTANDARD11, WhichLib.E_SQLITE3, IncludeLib.YES);

            write_dependency_group(f, TFM.NETSTANDARD11, DEP_CORE);
            write_dependency_group(f, TFM.NONE, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

#if not
			foreach (
				var dll in dlls
					.Where(d => d.project_subdir == "SQLitePCLRaw.core") // TODO green
				)
			{
				write_nuspec_file_entry_lib(
						dll.get_src_path(dir_mt), 
						dll.tfm,
						f
						);
			}
#endif

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static string gen_nuget_targets_sqlite3_itself(string top, string id, string toolset)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		// TODO should we put the cpu check code here, like the original version of this function (below)?

		string tname = string.Format("{0}.targets", id);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			switch (toolset)
			{
				case "v110_xp":
					// statically linked
					break;
				case "v110":
					f.WriteStartElement("ItemGroup");
					f.WriteAttributeString("Condition", " '$(Platform.Trim().Substring(0,3).ToLower())' != 'any' ");
					f.WriteStartElement("SDKReference");
					f.WriteAttributeString("Include", "Microsoft.VCLibs, Version=11.0");
					f.WriteEndElement(); // SDKReference
					f.WriteEndElement(); // ItemGroup
					break;
				case "v120":
					f.WriteStartElement("ItemGroup");
					f.WriteAttributeString("Condition", " '$(Platform.Trim().Substring(0,3).ToLower())' != 'any' ");
					f.WriteStartElement("SDKReference");
					f.WriteAttributeString("Include", "Microsoft.VCLibs, Version=12.0");
					f.WriteEndElement(); // SDKReference
					f.WriteEndElement(); // ItemGroup
					break;
				case "v140":
#if not // TODO do we need this?  we should, but testing says we don't.
					f.WriteStartElement("ItemGroup");
					f.WriteStartElement("SDKReference");
					f.WriteAttributeString("Include", "Microsoft.VCLibs, Version=14.0");
					f.WriteEndElement(); // SDKReference
					f.WriteEndElement(); // ItemGroup
#endif
					break;
			}

			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", Guid.NewGuid().ToString()));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Windows_NT' ");

			var front = rid_front_half(toolset);
			Action<string> write_item = (cpu) =>
			{
				f.WriteStartElement("ItemGroup");
				f.WriteAttributeString("Condition", string.Format(" '$(Platform.ToLower())' == '{0}' ", cpu.ToLower()));

				f.WriteStartElement("Content");
				// TODO call other.get_products() instead of hard-coding the sqlite3.dll name here
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\runtimes\\{0}-{1}\\native\\e_sqlite3.dll", front, cpu));
				// TODO link
				// TODO condition/exists ?
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteElementString("Pack", "false");
				f.WriteEndElement(); // Content

				f.WriteEndElement(); // ItemGroup
			};

			switch (toolset)
			{
				case "v110_xp":
					write_item("x86");
					write_item("x64");
					write_item("arm");
					break;
				case "v110":
					write_item("arm");
					write_item("x64");
					write_item("x86");
					break;
				case "v120":
					write_item("arm");
					write_item("x64");
					write_item("x86");
					break;
				case "v140":
					write_item("arm");
					write_item("x64");
					write_item("x86");
					break;
				case "v110_wp80":
					write_item("arm");
					write_item("x86");
					break;
				case "v120_wp81":
					write_item("arm");
					write_item("x86");
					break;
				default:
					throw new NotImplementedException();
			}

			f.WriteEndElement(); // Target

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
		return tname;
	}

	// TODO change the name of this to something like dual arch
	private static string gen_nuget_targets_pinvoke_anycpu(string top, string id, string toolset)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string tname = string.Format("{0}.targets", id);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			var guid = Guid.NewGuid().ToString();
			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", guid));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Windows_NT' ");
			{
				f.WriteStartElement("Content");
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\{0}", Path.Combine("runtimes\\win-x86\\native", "e_sqlite3.dll")));
				// TODO condition/exists ?
				f.WriteElementString("Link", string.Format("{0}\\e_sqlite3.dll", "x86"));
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteElementString("Pack", "false");
				f.WriteEndElement(); // Content
			}
			{
				f.WriteStartElement("Content");
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\{0}", Path.Combine("runtimes\\win-x64\\native", "e_sqlite3.dll")));
				// TODO condition/exists ?
				f.WriteElementString("Link", string.Format("{0}\\e_sqlite3.dll", "x64"));
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteElementString("Pack", "false");
				f.WriteEndElement(); // Content
			}
			f.WriteEndElement(); // ItemGroup

			f.WriteEndElement(); // Target

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ResolveAssemblyReferencesDependsOn", 
					string.Format("$(ResolveAssemblyReferencesDependsOn);InjectReference_{0}", guid));
			f.WriteEndElement(); // PropertyGroup

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
		return tname;
	}

	private static void gen_nuget_targets_windows(string top, string tname, string filename)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			var guid = Guid.NewGuid().ToString();
			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", guid));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Windows_NT' ");

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\win-x86\\native\\{0}", filename));
			f.WriteElementString("Link", string.Format("{0}\\{1}", "x86", filename));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\win-x64\\native\\{0}", filename));
			f.WriteElementString("Link", string.Format("{0}\\{1}", "x64", filename));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content

			f.WriteEndElement(); // ItemGroup

			f.WriteEndElement(); // Target

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ResolveAssemblyReferencesDependsOn", 
					string.Format("$(ResolveAssemblyReferencesDependsOn);InjectReference_{0}", guid));
			f.WriteEndElement(); // PropertyGroup

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static void gen_nuget_targets_osx(string top, string tname, string filename, bool forxammac)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			var guid = Guid.NewGuid().ToString();
			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", guid));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Unix' AND Exists('/Library/Frameworks') ");

			if (forxammac)
			{
				f.WriteStartElement("NativeReference");
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\osx-x64\\native\\{0}", filename));
				f.WriteElementString("Kind", "Dynamic");
				f.WriteElementString("SmartLink", "False");
				f.WriteEndElement(); // NativeReference
			}
			else
			{
				f.WriteStartElement("Content");
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\osx-x64\\native\\{0}", filename));
				f.WriteElementString("Link", filename);
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteElementString("Pack", "false");
				f.WriteEndElement(); // Content
			}

			f.WriteEndElement(); // ItemGroup

			f.WriteEndElement(); // Target

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ResolveAssemblyReferencesDependsOn", 
					string.Format("$(ResolveAssemblyReferencesDependsOn);InjectReference_{0}", guid));
			f.WriteEndElement(); // PropertyGroup

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static void gen_nuget_targets_linux(string top, string tname, string filename)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			var guid = Guid.NewGuid().ToString();
			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", guid));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Unix' AND !Exists('/Library/Frameworks') ");

#if TODO // load library before dllimport doesn't seem to work on linux
			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\linux-x64\\native\\{0}", filename));
			f.WriteElementString("Link", string.Format("{0}\\{1}", "x64", filename));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\linux-x86\\native\\{0}", filename));
			f.WriteElementString("Link", string.Format("{0}\\{1}", "x86", filename));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content
#else
			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\linux-x64\\native\\{0}", filename));
			f.WriteElementString("Link", filename);
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content
#endif

			f.WriteEndElement(); // ItemGroup

			f.WriteEndElement(); // Target

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ResolveAssemblyReferencesDependsOn", 
					string.Format("$(ResolveAssemblyReferencesDependsOn);InjectReference_{0}", guid));
			f.WriteEndElement(); // PropertyGroup

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	public static void Main(string[] args)
	{
		string root = Directory.GetCurrentDirectory(); // assumes that gen_build.exe is being run from the root directory of the project
		string top = Path.Combine(root, "bld");
		var cb_bin = Path.GetFullPath(Path.Combine(root, "..", "cb", "bld", "bin"));
		string dir_mt = Path.Combine(root, "mt");

		// --------------------------------
		// create the bld directory
		Directory.CreateDirectory(top);

		// --------------------------------

		var dlls = find_dlls(dir_mt);

        gen_nuspec_core(top, root, dir_mt, dlls);
        gen_nuspec_ugly(top, dir_mt, dlls);
        gen_nuspec_bundle_green(top, dir_mt, dlls);
        gen_nuspec_bundle_e_sqlite3(top, dir_mt, dlls);
        gen_nuspec_bundle_winsqlite3(top, dir_mt, dlls);
        gen_nuspec_bundle_sqlcipher(top, SQLCipherBundleKind.Unofficial, dir_mt, dlls);
        gen_nuspec_bundle_sqlcipher(top, SQLCipherBundleKind.Zetetic, dir_mt, dlls);

		var items_embedded = new config_embedded[]
		{
			new config_embedded
			{
				id = "SQLitePCLRaw.lib.e_sqlite3.android",
				src = "TODO",
				tfm = TFM.ANDROID
			},
			new config_embedded
			{
				id = "SQLitePCLRaw.lib.e_sqlite3.ios",
				src = "TODO",
				tfm = TFM.IOS
			},
			new config_embedded
			{
				id = "SQLitePCLRaw.lib.sqlcipher.android",
				src = "TODO",
				tfm = TFM.ANDROID
			},
			new config_embedded
			{
				id = "SQLitePCLRaw.lib.sqlcipher.ios",
				src = "TODO",
				tfm = TFM.IOS
			},
		};

		foreach (var cfg in items_embedded)
		{
			gen_nuspec_embedded(
				top, 
				cfg
				);
		}

		gen_nuspec_e_sqlite3_win(top, cb_bin);
		gen_nuspec_e_sqlite3_otherplat(top, cb_bin, "osx");
		gen_nuspec_e_sqlite3_otherplat(top, cb_bin, "linux");

		gen_nuspec_sqlcipher(top, cb_bin, "windows");
		gen_nuspec_sqlcipher(top, cb_bin, "osx");
		gen_nuspec_sqlcipher(top, cb_bin, "linux");

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "build.ps1")))
		{
			// our build configs for ancient PCL profiles require an old version of nuget
			tw.WriteLine("../nuget_old.exe restore sqlitepcl.sln");
			tw.WriteLine("msbuild /p:Configuration=Release sqlitepcl.sln");
		}

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pack.ps1")))
		{
            tw.WriteLine("../nuget pack {0}.core.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.ugly.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_green.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_e_sqlite3.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_sqlcipher.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_zetetic.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_winsqlite3.nuspec", gen.ROOT_NAME);

			tw.WriteLine("../nuget pack {0}.lib.e_sqlite3.windows.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../nuget pack {0}.lib.e_sqlite3.osx.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../nuget pack {0}.lib.e_sqlite3.linux.nuspec", gen.ROOT_NAME);

			tw.WriteLine("../nuget pack {0}.lib.sqlcipher.windows.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../nuget pack {0}.lib.sqlcipher.osx.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../nuget pack {0}.lib.sqlcipher.linux.nuspec", gen.ROOT_NAME);

			foreach (var cfg in items_embedded)
			{
				tw.WriteLine("../nuget pack {0}.nuspec", cfg.id);
			}
			tw.WriteLine("ls *.nupkg");
		}

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "push.ps1")))
		{
            const string src = "https://www.nuget.org/api/v2/package";

			tw.WriteLine("ls *.nupkg");
			tw.WriteLine("../nuget push -Source {2} {0}.core.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.ugly.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_green.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_e_sqlite3.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_sqlcipher.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_zetetic.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_winsqlite3.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			tw.WriteLine("../nuget push -Source {2} {0}.lib.e_sqlite3.windows.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.lib.e_sqlite3.osx.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.lib.e_sqlite3.linux.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			tw.WriteLine("../nuget push -Source {2} {0}.lib.sqlcipher.windows.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.lib.sqlcipher.osx.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.lib.sqlcipher.linux.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			foreach (var cfg in items_embedded)
			{
				tw.WriteLine("../nuget push -Source {2} {0}.{1}.nupkg", cfg.id, NUSPEC_VERSION, src);
			}
		}
	}
}

