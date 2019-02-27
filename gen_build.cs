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
			case WhichLib.E_SQLCIPHER: return "sqlcipher"; // TODO no e_ prefix in cb yet
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
				return string.Format("{0}.dll", basename);
			case LibSuffix.DYLIB:
				return string.Format("lib{0}.dylib", basename);
			case LibSuffix.SO:
				return string.Format("lib{0}.so", basename);
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
			case "xamarin.ios10": return TFM.IOS;
			default:
				throw new NotImplementedException(string.Format("str_to_tfm not found: {0}", s));
		}
	}

	#if not
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
#endif

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

	private static void write_empty(XmlWriter f, string top, TFM tfm)
    {
		f.WriteComment("empty directory in lib to avoid nuget adding a reference");

		f.WriteStartElement("file");
		f.WriteAttributeString("src", "empty\\");
		f.WriteAttributeString("target", string.Format("lib\\{0}", tfm.AsString()));
		f.WriteEndElement(); // file
    }

	public const int MAJOR_VERSION = 2;
	public const int MINOR_VERSION = 0;
	public const int PATCH_VERSION = 0;
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
	public static string NUSPEC_VERSION = NUSPEC_VERSION_RELEASE;
	public static string ASSEMBLY_VERSION = string.Format("{0}.{1}.{2}.{3}", 
		MAJOR_VERSION,
		MINOR_VERSION,
		PATCH_VERSION,
		(int) ((DateTime.Now - new DateTime(2018,1,1)).TotalDays) 
		); 

	private const string NUSPEC_RELEASE_NOTES = "TODO url";

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
		f.WriteElementString("title", id);
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
		var dir_name = lib.AsString_basename_in_cb();
		var lib_name = lib.AsString_libname_in_cb(LibSuffix.DLL);
		return Path.Combine(cb_bin, dir_name, "win", toolset, flavor, arch, lib_name);
	}

	static string make_cb_path_linux(
		string cb_bin,
		WhichLib lib,
		string cpu
		)
	{
		var dir_name = lib.AsString_basename_in_cb();
		var lib_name = lib.AsString_libname_in_cb(LibSuffix.SO);
		return Path.Combine(cb_bin, dir_name, "linux", cpu, lib_name);
	}

	static string make_cb_path_mac(
		string cb_bin,
		WhichLib lib
		)
	{
		var dir_name = lib.AsString_basename_in_cb();
		var lib_name = lib.AsString_libname_in_cb(LibSuffix.DYLIB);
		return Path.Combine(cb_bin, dir_name, "mac", lib_name);
	}

	static void write_nuspec_file_entry_native_linux(
		WhichLib lib,
		string cb_bin,
		string cpu_in_cb,
		string rid,
		XmlWriter f
		)
	{
		var filename = lib.AsString_libname_in_nupkg(LibSuffix.SO);
		write_nuspec_file_entry_native(
			make_cb_path_linux(cb_bin, lib, cpu_in_cb),
			rid,
			filename,
			f
			);
	}

	static void write_nuspec_file_entry_native_mac(
		WhichLib lib,
		string cb_bin,
		XmlWriter f
		)
	{
		var filename = lib.AsString_libname_in_nupkg(LibSuffix.DYLIB);
		write_nuspec_file_entry_native(
			make_cb_path_mac(cb_bin, lib),
			"osx-x64",
			filename,
			f
			);
	}

	static void write_nuspec_file_entry_native_win(
		WhichLib lib,
		string cb_bin,
		string toolset,
		string flavor,
		string cpu,
		string rid,
		XmlWriter f
		)
	{
		var filename = lib.AsString_libname_in_nupkg(LibSuffix.DLL);
		write_nuspec_file_entry_native(
			make_cb_path_win(cb_bin, lib, toolset, flavor, cpu),
			rid,
			filename,
			f
			);
	}

	static void write_nuspec_file_entries_from_cb(
		WhichLib lib,
		string cb_bin,
		XmlWriter f
		)
	{
		write_nuspec_file_entry_native_win(lib, cb_bin, "v140", "plain", "x86", "win-x86", f);
		write_nuspec_file_entry_native_win(lib, cb_bin, "v140", "plain", "x64", "win-x64", f);
		write_nuspec_file_entry_native_win(lib, cb_bin, "v140", "plain", "arm", "win8-arm", f);
		write_nuspec_file_entry_native_win(lib, cb_bin, "v140", "appcontainer", "arm", "win10-arm", f);
		write_nuspec_file_entry_native_win(lib, cb_bin, "v140", "appcontainer", "x64", "win10-x64", f);
		write_nuspec_file_entry_native_win(lib, cb_bin, "v140", "appcontainer", "x86", "win10-x86", f);

		write_nuspec_file_entry_native_mac(lib, cb_bin, f);

		write_nuspec_file_entry_native_linux(lib, cb_bin, "x64", "linux-x64", f);
		write_nuspec_file_entry_native_linux(lib, cb_bin, "x86", "linux-x86", f);
		write_nuspec_file_entry_native_linux(lib, cb_bin, "armhf", "linux-arm", f);
		write_nuspec_file_entry_native_linux(lib, cb_bin, "armsf", "linux-armel", f);
		write_nuspec_file_entry_native_linux(lib, cb_bin, "arm64", "linux-arm64", f);
		write_nuspec_file_entry_native_linux(lib, cb_bin, "musl-x64", "linux-musl-x64", f);
		write_nuspec_file_entry_native_linux(lib, cb_bin, "musl-x64", "alpine-x64", f);
	}

	private static void gen_nuspec_lib_e_sqlite3(string top, string cb_bin, string dir_mt, List<dll_info> dlls)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = string.Format("SQLitePCLRaw.lib.e_sqlite3");
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (
				var dll in dlls
					.Where(d => 
						(d.project_subdir == "SQLitePCLRaw.lib.e_sqlite3.ios")
						|| (d.project_subdir == "SQLitePCLRaw.lib.e_sqlite3.android")
						)
				)
			{
				write_nuspec_file_entry_lib(
						dll.get_src_path(dir_mt), 
						dll.tfm,
						f
						);
			}

			write_nuspec_file_entries_from_cb(WhichLib.E_SQLITE3, cb_bin, f);

			var tname = string.Format("{0}.targets", id);
			var path_targets = Path.Combine(top, tname);
			gen_nuget_targets(path_targets, WhichLib.E_SQLITE3);
			write_nuspec_file_entry(
				path_targets,
				string.Format("build\\net45"), // TODO
				f
				);

			write_empty(f, top, TFM.NET35);
			write_empty(f, top, TFM.NETSTANDARD11);
			write_empty(f, top, TFM.NETSTANDARD20);

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_lib_e_sqlcipher(string top, string cb_bin, string dir_mt, List<dll_info> dlls)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = string.Format("SQLitePCLRaw.lib.e_sqlcipher");
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			write_nuspec_common_metadata(id, f);
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLCipher (see sqlcipher/sqlcipher on GitHub) for use with SQLitePCL.raw.  The build of SQLCipher packaged here is built and maintained by Couchbase (see couchbaselabs/couchbase-lite-libsqlcipher on GitHub).  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.sqlcipher.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (
				var dll in dlls
					.Where(d => 
						(d.project_subdir == "SQLitePCLRaw.lib.e_sqlcipher.ios")
						|| (d.project_subdir == "SQLitePCLRaw.lib.e_sqlcipher.android")
						)
				)
			{
				write_nuspec_file_entry_lib(
						dll.get_src_path(dir_mt), 
						dll.tfm,
						f
						);
			}

			write_nuspec_file_entries_from_cb(WhichLib.E_SQLCIPHER, cb_bin, f);

			var tname = string.Format("{0}.targets", id);
			var path_targets = Path.Combine(top, tname);
			gen_nuget_targets(path_targets, WhichLib.E_SQLCIPHER);
			write_nuspec_file_entry(
				path_targets,
				string.Format("build\\net45"), // TODO
				f
				);

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
		E_SQLCIPHER,
	}

    private static void write_bundle_dependency_group(XmlWriter f, WhichLib what)
    {
        f.WriteStartElement("group");

        add_dep_core(f);

		if (what == WhichLib.E_SQLITE3)
		{
			f.WriteStartElement("dependency");
			f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3", gen.ROOT_NAME));
			f.WriteAttributeString("version", NUSPEC_VERSION);
			f.WriteEndElement(); // dependency
		}
		else if (what == WhichLib.E_SQLCIPHER)
		{
			f.WriteStartElement("dependency");
			f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlcipher", gen.ROOT_NAME));
			f.WriteAttributeString("version", NUSPEC_VERSION);
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

	private static void gen_nuspec_bundle_e_sqlcipher(string top, string dir_mt, List<dll_info> dlls)
	{
		var id = string.Format("{0}.bundle_e_sqlcipher", gen.ROOT_NAME);

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
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: unofficial open source sqlcipher builds included.  Note that these sqlcipher builds are unofficial and unsupported.  For official sqlcipher builds, contact Zetetic.");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, WhichLib.E_SQLCIPHER);
            
			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (
				var dll in dlls
					.Where(d => d.project_subdir == "SQLitePCLRaw.batteries_v2.e_sqlcipher")
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

	private static void gen_nuspec_bundle_zetetic(string top, string dir_mt, List<dll_info> dlls)
    {
		var id = string.Format("{0}.bundle_zetetic", gen.ROOT_NAME);

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
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: reference the official SQLCipher builds from Zetetic, which are not included in this package");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, WhichLib.NONE);
            
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
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: e_sqlite3 included");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, WhichLib.E_SQLITE3);
            
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
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: iOS=system SQLite, others=e_sqlite3 included");

			f.WriteStartElement("dependencies");

			// TODO how to get this to use the system sqlite for ios?

            write_bundle_dependency_group(f, WhichLib.E_SQLITE3);

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
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
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
			write_nuget_target_item("win8-arm", lib, f);
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
			f.WriteEndElement(); // ItemGroup

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static void gen_assemblyinfo(string root, string dir_mt, string assemblyname)
	{
		string cs = File.ReadAllText(Path.Combine(root, "src/cs/AssemblyInfo.cs"));
		var dir_gen = Path.Combine(dir_mt, assemblyname, "Generated");
		Directory.CreateDirectory(dir_gen);
		using (TextWriter tw = new StreamWriter(Path.Combine(dir_gen, "AssemblyInfo.cs")))
		{
			string cs1 = cs
				.Replace("REPLACE_WITH_ASSEMBLY_NAME", '"' + assemblyname + '"')
				.Replace("REPLACE_WITH_ASSEMBLY_VERSION", '"' + ASSEMBLY_VERSION + '"')
				;
			tw.Write(cs1);
		}
	}

	public static void Main(string[] args)
	{
		string root = Directory.GetCurrentDirectory(); // assumes that gen_build.exe is being run from the root directory of the project
		string top = Path.Combine(root, "pkg");
		var cb_bin = Path.GetFullPath(Path.Combine(root, "..", "cb", "bld", "bin"));
		string dir_mt = Path.Combine(root, "mt");

		// --------------------------------
		// create the pkg directory
		Directory.CreateDirectory(top);

		// --------------------------------

		var dlls = find_dlls(dir_mt)
			.Where(d => d.config.ToLower() == "release")
			.ToList()
			;

		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.core");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.impl.callbacks");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.provider.dynamic");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.provider.e_sqlite3");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.provider.e_sqlcipher");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.provider.sqlite3");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.provider.sqlcipher");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.provider.winsqlite3");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.ugly");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.lib.e_sqlite3.android");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.lib.e_sqlite3.ios");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.lib.e_sqlcipher.android");
		gen_assemblyinfo(root, dir_mt, "SQLitePCLRaw.lib.e_sqlcipher.ios");

        gen_nuspec_core(top, root, dir_mt, dlls);
        gen_nuspec_ugly(top, dir_mt, dlls);

		gen_nuspec_lib_e_sqlite3(top, cb_bin, dir_mt, dlls);
		gen_nuspec_lib_e_sqlcipher(top, cb_bin, dir_mt, dlls);

        gen_nuspec_bundle_green(top, dir_mt, dlls);
        gen_nuspec_bundle_e_sqlite3(top, dir_mt, dlls);
        gen_nuspec_bundle_winsqlite3(top, dir_mt, dlls);
        gen_nuspec_bundle_e_sqlcipher(top, dir_mt, dlls);
        gen_nuspec_bundle_zetetic(top, dir_mt, dlls);

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pack.bat")))
		{
            tw.WriteLine("mkdir empty");

            tw.WriteLine("..\\nuget pack {0}.core.nuspec", gen.ROOT_NAME);
            tw.WriteLine("..\\nuget pack {0}.ugly.nuspec", gen.ROOT_NAME);

			tw.WriteLine("..\\nuget pack {0}.lib.e_sqlite3.nuspec", gen.ROOT_NAME);
			tw.WriteLine("..\\nuget pack {0}.lib.e_sqlcipher.nuspec", gen.ROOT_NAME);

            tw.WriteLine("..\\nuget pack {0}.bundle_green.nuspec", gen.ROOT_NAME);
            tw.WriteLine("..\\nuget pack {0}.bundle_e_sqlite3.nuspec", gen.ROOT_NAME);
            tw.WriteLine("..\\nuget pack {0}.bundle_e_sqlcipher.nuspec", gen.ROOT_NAME);
            tw.WriteLine("..\\nuget pack {0}.bundle_zetetic.nuspec", gen.ROOT_NAME);
            tw.WriteLine("..\\nuget pack {0}.bundle_winsqlite3.nuspec", gen.ROOT_NAME);

            tw.WriteLine("dir *.nupkg");
		}

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "push.bat")))
		{
            const string src = "https://www.nuget.org/api/v2/package";

			tw.WriteLine("..\\nuget push -Source {2} {0}.core.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("..\\nuget push -Source {2} {0}.ugly.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			tw.WriteLine("..\\nuget push -Source {2} {0}.lib.e_sqlite3.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("..\\nuget push -Source {2} {0}.lib.e_sqlcipher.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			tw.WriteLine("..\\nuget push -Source {2} {0}.bundle_green.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("..\\nuget push -Source {2} {0}.bundle_e_sqlite3.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("..\\nuget push -Source {2} {0}.bundle_e_sqlcipher.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("..\\nuget push -Source {2} {0}.bundle_zetetic.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("..\\nuget push -Source {2} {0}.bundle_winsqlite3.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
		}
	}
}

