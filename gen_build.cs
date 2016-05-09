/*
   Copyright 2014-2016 Zumero, LLC

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
using System.Xml;

public static class projects
{
	// Each item in these Lists corresponds to a project file that will be
	// generated.
	//
	public static List<config_sqlite3> items_sqlite3 = new List<config_sqlite3>();
	public static List<config_cppinterop> items_cppinterop = new List<config_cppinterop>();
	public static List<config_pcl> items_pcl = new List<config_pcl>();
	public static List<config_higher> items_higher = new List<config_higher>();
	public static List<config_tests> items_tests = new List<config_tests>();
	public static List<config_plugin> items_plugin = new List<config_plugin>();
	public static List<config_esqlite3> items_esqlite3 = new List<config_esqlite3>();

	// This function is called by Main to initialize the project lists.
	//
	public static void init()
	{
		init_sqlite3();
		init_cppinterop();
		init_pcl_bait();
		init_pcl_pinvoke();
		init_plugin();
		init_esqlite3();
		init_pcl_cppinterop();
		init_higher();
		init_tests();
	}

	private static void init_sqlite3()
	{
		items_sqlite3.Add(new config_sqlite3 { toolset="v110_xp", cpu="x86" });
		items_sqlite3.Add(new config_sqlite3 { toolset="v110_xp", cpu="x64" });

		items_sqlite3.Add(new config_sqlite3 { toolset="v110", cpu="arm" });
		items_sqlite3.Add(new config_sqlite3 { toolset="v110", cpu="x64" });
		items_sqlite3.Add(new config_sqlite3 { toolset="v110", cpu="x86" });

		items_sqlite3.Add(new config_sqlite3 { toolset="v120", cpu="arm" });
		items_sqlite3.Add(new config_sqlite3 { toolset="v120", cpu="x64" });
		items_sqlite3.Add(new config_sqlite3 { toolset="v120", cpu="x86" });

		items_sqlite3.Add(new config_sqlite3 { toolset="v140", cpu="arm" });
		items_sqlite3.Add(new config_sqlite3 { toolset="v140", cpu="x64" });
		items_sqlite3.Add(new config_sqlite3 { toolset="v140", cpu="x86" });

		items_sqlite3.Add(new config_sqlite3 { toolset="v110_wp80", cpu="arm" });
		items_sqlite3.Add(new config_sqlite3 { toolset="v110_wp80", cpu="x86" });

		items_sqlite3.Add(new config_sqlite3 { toolset="v120_wp81", cpu="arm" });
		items_sqlite3.Add(new config_sqlite3 { toolset="v120_wp81", cpu="x86" });
	}

	private static void init_cppinterop()
	{
		items_cppinterop.Add(new config_cppinterop { env="wp80", cpu="arm"});
		items_cppinterop.Add(new config_cppinterop { env="wp80", cpu="x86"});

		items_cppinterop.Add(new config_cppinterop { env="wp81_sl", cpu="arm"});
		items_cppinterop.Add(new config_cppinterop { env="wp81_sl", cpu="x86"});
	}

	private static void init_pcl_bait()
	{
		items_pcl.Add(new config_pcl { env="profile78", cpu="anycpu" });
		items_pcl.Add(new config_pcl { env="profile259", cpu="anycpu" });
		items_pcl.Add(new config_pcl { env="profile158", cpu="anycpu" });
		items_pcl.Add(new config_pcl { env="profile136", cpu="anycpu" });
		items_pcl.Add(new config_pcl { env="profile111", cpu="anycpu" });
	}

	private static void init_tests()
	{
		// TODO it is not confirmed that any other environments will work here.
		// for now that's fine.

		items_tests.Add(new config_tests { env="win8", pcl="profile78" });
	}

	private static void init_higher()
	{
		items_higher.Add(new config_higher { name="ugly", assemblyname="SQLitePCL.ugly", env="profile78", csfiles=new List<string>() {"src\\cs\\ugly.cs"} });
		items_higher.Add(new config_higher { name="ugly", assemblyname="SQLitePCL.ugly", env="profile111", csfiles=new List<string>() {"src\\cs\\ugly.cs"} });
		items_higher.Add(new config_higher { name="ugly", assemblyname="SQLitePCL.ugly", env="profile259", csfiles=new List<string>() {"src\\cs\\ugly.cs"} });
		items_higher.Add(new config_higher { name="ugly", assemblyname="SQLitePCL.ugly", env="profile158", csfiles=new List<string>() {"src\\cs\\ugly.cs"}, defines=new List<string>() {"OLD_REFLECTION"} });
		items_higher.Add(new config_higher { name="ugly", assemblyname="SQLitePCL.ugly", env="profile136", csfiles=new List<string>() {"src\\cs\\ugly.cs"}, defines=new List<string>() {"OLD_REFLECTION"} });
		items_higher.Add(new config_higher { name="ugly", assemblyname="SQLitePCL.ugly", env="net35", csfiles=new List<string>() {"src\\cs\\ugly.cs"}, defines=new List<string>() {"OLD_REFLECTION"} });
	}

	private static void init_pcl_cppinterop()
	{
		items_pcl.Add(new config_pcl { env="wp80", api="cppinterop", cpu="arm"});
		items_pcl.Add(new config_pcl { env="wp80", api="cppinterop", cpu="x86"});

		items_pcl.Add(new config_pcl { env="wp81_sl", api="cppinterop", cpu="arm"});
		items_pcl.Add(new config_pcl { env="wp81_sl", api="cppinterop", cpu="x86"});
	}

	private static void init_esqlite3()
	{
		items_esqlite3.Add(new config_esqlite3 { toolset="v110_xp" });
	}

	private static void init_plugin()
	{
		items_plugin.Add(new config_plugin { env="ios_classic", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="ios_classic", what="sqlcipher" });

		items_plugin.Add(new config_plugin { env="ios_unified", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="ios_unified", what="sqlcipher" });

		items_plugin.Add(new config_plugin { env="android", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="android", what="sqlcipher" });

		items_plugin.Add(new config_plugin { env="net35", what="sqlcipher", empty=true });
		items_plugin.Add(new config_plugin { env="net40", what="sqlcipher", empty=true });
		items_plugin.Add(new config_plugin { env="net45", what="sqlcipher", empty=true });

		items_plugin.Add(new config_plugin { env="net45", what="sqlite3", empty=true });
		items_plugin.Add(new config_plugin { env="net40", what="sqlite3", empty=true });
		items_plugin.Add(new config_plugin { env="net35", what="sqlite3", empty=true });

		items_plugin.Add(new config_plugin { env="net45", toolset="v110_xp", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="net40", toolset="v110_xp", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="net35", toolset="v110_xp", what="sqlite3" });

		items_plugin.Add(new config_plugin { env="win8", toolset="v110", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="win8", toolset="v110_xp", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="win81", toolset="v120", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="win81", toolset="v110_xp", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="wpa81", toolset="v120_wp81", what="sqlite3" });
		items_plugin.Add(new config_plugin { env="uap10.0", toolset="v140", what="sqlite3" });
	}

	private static void init_pcl_pinvoke()
	{
		items_pcl.Add(new config_pcl { env="android", api="pinvoke"});
		items_pcl.Add(new config_pcl { env="ios_classic", api="pinvoke"});
		items_pcl.Add(new config_pcl { env="ios_unified", api="pinvoke"});
		//items_pcl.Add(new config_pcl { env="unified_mac", api="pinvoke"});
		items_pcl.Add(new config_pcl { env="net45", api="pinvoke"});
		items_pcl.Add(new config_pcl { env="net40", api="pinvoke"});
		items_pcl.Add(new config_pcl { env="net35", api="pinvoke"});
		items_pcl.Add(new config_pcl { env="win8", api="pinvoke"});
		items_pcl.Add(new config_pcl { env="win81", api="pinvoke"});
		items_pcl.Add(new config_pcl { env="uap10.0", api="pinvoke"});
		items_pcl.Add(new config_pcl { env="wpa81", api="pinvoke"});
	}

    public static string cs_env_to_toolset(string env)
    {
	    switch (env) {
		case "net45":
			return "v110_xp";
		case "net40":
			return "v110_xp";
		case "net35":
			return "v110_xp";
		case "wp80":
			return "v110_wp80";
		case "wp81_sl":
			return "v120";
		case "wpa81":
			return "v120_wp81";
		case "uap10.0":
			return "v140";
		case "win8":
			return "v110";
		case "win81":
			return "v120";
		default:
			throw new Exception(env);
	    }
    }

	public static string get_portable_nuget_target_string(string env)
	{
		switch (env)
		{
			case "profile78":
				return "portable-net45+netcore45+wp8+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			case "profile259":
				return "portable-net45+netcore45+wpa81+wp8+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			case "profile111":
				return "portable-net45+netcore45+wpa81+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			case "profile158":
				return "portable-net45+sl5+netcore45+wp8+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			case "profile136":
				return "portable-net40+sl5+netcore45+wp8+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			default:
				throw new Exception(env);
		}
	}

	public static config_sqlite3 find_sqlite3(string toolset, string cpu)
	{
		foreach (config_sqlite3 cfg in projects.items_sqlite3)
		{
			if (
					(cfg.toolset == toolset)
					&& (cfg.cpu == cpu)
			   )
			{
				return cfg;
			}
		}
		//throw new Exception();
		return null;
	}

	public static List<config_sqlite3> find_sqlite3(string toolset)
	{
		var result = new List<config_sqlite3>();
		foreach (config_sqlite3 cfg in projects.items_sqlite3)
		{
			if (
					(cfg.toolset == toolset)
			   )
			{
				result.Add(cfg);
			}
		}
		return result;
	}

	public static config_pcl find_bait(string env)
	{
		foreach (config_pcl cfg in projects.items_pcl)
		{
			if (cfg.env == env)
			{
				return cfg;
			}
		}
		throw new Exception(env);
	}

	public static config_higher find_ugly(string env)
	{
		foreach (config_higher cfg in projects.items_higher)
		{
			if (
					(cfg.name == "ugly")
					&& (cfg.env == env)
			   )
			{
				return cfg;
			}
		}
		throw new Exception(env);
	}

	public static List<config_pcl> find_pcls(string env, string api, string cpu)
	{
		List<config_pcl> a = new List<config_pcl>();
		foreach (config_pcl cfg in projects.items_pcl)
		{
			if (
					(env != null)
					&& (cfg.env != env)
			   )
			{
				continue;
			}

			if (
					(api != null)
					&& (cfg.api != api)
			   )
			{
				continue;
			}

			if (
					(cpu != null)
					&& (cfg.cpu != cpu)
			   )
			{
				continue;
			}

			a.Add(cfg);
		}
		return a;
	}

}

public interface config_info
{
	string get_project_filename();
	string get_name();
	string get_dest_subpath();
}

public static class config_info_ext
{
	public static string get_project_subdir(this config_info cfg, string top)
	{
		string subdir = Path.Combine(Path.Combine(top, cfg.get_name()));
		Directory.CreateDirectory(subdir);
		return subdir;
	}

	public static string get_project_path(this config_info cfg, string top)
	{
		string subdir = cfg.get_project_subdir(top);
		string proj = Path.Combine(subdir, cfg.get_project_filename());
		return proj;
	}

}

public class config_sqlite3 : config_info
{
	public string toolset;
	public string cpu;
	public string guid;

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_dest_subpath(), s));
	}

	public void get_products(List<string> a)
	{
		add_product(a, "esqlite3.dll");
	}

	private string area()
	{
		return "sqlite3";
	}

	public string get_nuget_target_path()
	{
		return string.Format("build\\native\\{0}\\", get_dest_subpath());
	}

	public string get_dest_subpath()
	{
		return string.Format("{0}\\{1}\\{2}", area(), toolset, cpu);
	}

	public string get_name()
	{
		return string.Format("{0}.{1}.{2}", area(), toolset, cpu);
	}

	public string get_project_filename()
	{
		return string.Format("{0}.vcxproj", get_name());
	}

	public string fixed_cpu()
	{
		if (cpu == "x86")
		{
			return "Win32";
		}
		else
		{
			return cpu;
		}
	}
}

public class config_cppinterop : config_info
{
	public string env;
	public string cpu;
	public string guid;

	public config_sqlite3 get_sqlite3_item()
	{
		string toolset = projects.cs_env_to_toolset(env);
		config_sqlite3 other = projects.find_sqlite3(toolset, cpu);
		if (other == null)
		{
			throw new Exception(get_name());
		}
		return other;
	}

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_dest_subpath(), s));
	}

	public void get_products(List<string> a)
	{
		add_product(a, "SQLitePCL.cppinterop.dll");
		switch (env)
		{
			case "wp80":
				add_product(a, "SQLitePCL.cppinterop.winmd");
				break;

			case "wp81_sl":
				add_product(a, "SQLitePCL.cppinterop.pri");
				add_product(a, "SQLitePCL.cppinterop.winmd");
				break;

			default:
				throw new Exception("cppinterop invalid env");
		}
	}

	private string area()
	{
		return "cppinterop";
	}

	public string get_name()
	{
		return string.Format("{0}.{1}.{2}", area(), env, cpu);
	}

	public string get_dest_subpath()
	{
		return string.Format("{0}\\{1}\\{2}", area(), env, cpu);
	}

	public string get_project_filename()
	{
		return string.Format("{0}.vcxproj", get_name());
	}

	public string fixed_cpu()
	{
		if (cpu == "x86")
		{
			return "Win32";
		}
		else
		{
			return cpu;
		}
	}

	private string sqlite3_toolset()
	{
        return projects.cs_env_to_toolset(env);
	}
}

public class config_higher : config_info
{
	public string env;
	public string name;
	public string assemblyname;
	public string guid;
	public List<string> csfiles;
	public List<string> defines;

	public string get_dest_subpath()
	{
		return string.Format("{0}\\{1}", name, env);
	}

	public string get_name()
	{
		return string.Format("{0}_{1}", name, env);
	}

	public string get_project_filename()
	{
		return string.Format("{0}.csproj", get_name());
	}

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_dest_subpath(), s));
	}

	public bool is_portable()
	{
		return config_cs.env_is_portable(env);
	}

	public string get_nuget_target_path()
	{
		if (is_portable())
		{
			return string.Format("lib\\{0}\\", projects.get_portable_nuget_target_string(env));
		}
		else
		{
			return string.Format("lib\\{0}\\", config_cs.get_nuget_framework_name(env));
		}
	}

	public void get_products(List<string> a)
	{
		add_product(a, string.Format("{0}.dll", assemblyname));
	}
}

public class config_tests : config_info
{
	public string env;
	public string pcl;
	public string guid;

	public string get_project_filename()
	{
		return string.Format("{0}.csproj", get_name());
	}

	public string get_dest_subpath()
	{
		return string.Format("{0}\\{1}", "tests", env);
	}

	public string get_name()
	{
		return string.Format("{0}_{1}", "tests", env);
	}

}

public class config_plugin : config_info
{
	public string env;
	public string what;
	public string guid;
	public string toolset;
	public bool empty;

	public List<config_sqlite3> get_sqlite3_items()
	{
		List<config_sqlite3> other = projects.find_sqlite3(toolset);
		if (other == null)
		{
			throw new Exception(get_name());
		}
		return other;
	}

	public string get_nuget_target_path(string where)
	{
		return string.Format("lib\\{0}\\", config_cs.get_nuget_framework_name(env));
	}

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_dest_subpath(), s));
	}

	public void get_products(List<string> a)
	{
		add_product(a, "SQLite3Plugin.dll");
	}

	private const string AREA = "plugin";

	public string get_dest_subpath()
	{
		// TODO what if toolset is null/empty here?
		return string.Format("{0}\\{1}\\{2}\\{3}", AREA, what, env, toolset);
	}

	public string get_name()
	{
		if (string.IsNullOrWhiteSpace(toolset)) {
			return string.Format("plugin.{0}.{1}", what, env);
		} else {
			return string.Format("plugin.{0}.{1}.{2}", what, env, toolset);
		}
	}

	public string get_title()
	{
		if (string.IsNullOrWhiteSpace(toolset)) {
			return string.Format("Plugin ({0}, {1}) for SQLitePCL.raw", what, env);
		} else {
			return string.Format("Plugin ({0}, {1}, compiled with {2}) for SQLitePCL.raw", what, env, toolset);
		}
	}

	public string get_id()
	{
		return string.Format("SQLitePCL.{0}", get_name());
	}

	public string get_project_filename()
	{
		return string.Format("{0}.csproj", get_name());
	}
	
}

public class config_esqlite3 : config_info
{
	public string guid;
	public string toolset;

	public List<config_sqlite3> get_sqlite3_items()
	{
		List<config_sqlite3> other = projects.find_sqlite3(toolset);
		if (other == null)
		{
			throw new Exception(get_name());
		}
		return other;
	}

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_dest_subpath(), s));
	}

	private const string AREA = "native";

	public string get_nuget_target_path()
	{
		return string.Format("build\\native\\{0}\\", get_dest_subpath());
	}

	public string get_dest_subpath()
	{
		return string.Format("native\\sqlite3\\{0}", toolset);
	}

	public string get_name()
	{
		return string.Format("native.sqlite3.{0}", toolset);
	}

	public string get_title()
	{
		return string.Format("Native code only (sqlite3, compiled with {0}) for SQLitePCL.raw", toolset);
	}

	public string get_id()
	{
		return string.Format("SQLitePCL.{0}", get_name());
	}

	public string get_project_filename()
	{
		throw new NotImplementedException();
	}
	
}

public class config_cs
{
	public static bool env_needs_project_dot_json(string env)
	{
		switch (env)
		{
			case "uap10.0":
				return true;
			default:
				return false;
		}
	}

	public static bool env_is_portable(string env)
	{
		return env.StartsWith("profile");
	}

	public static string get_nuget_framework_name(string env)
	{
		// TODO maybe I should just use the TFM names?
		switch (env)
		{
			case "ios_classic":
				return "MonoTouch";
			case "ios_unified":
				return "Xamarin.iOS10";
			case "unified_mac":
				return "Xamarin.Mac20";
			case "android":
				return "MonoAndroid";
			case "net45":
				return "net45";
			case "net40":
				return "net40";
			case "net35":
				return "net35";
			case "wp80":
				return "wp8";
			case "wp81_sl":
				return "wp81";
			case "wpa81":
				return "wpa81";
			case "uap10.0":
				return "uap10.0";
			case "win8":
				return "win8";
			case "win81":
				return "win81";
			default:
				throw new Exception(env);
		}
	}
					
}

public class config_pcl : config_info
{
	public string env;
	public string api;
	public string guid;

	public string cpu = "anycpu";

	public config_sqlite3 get_sqlite3_item()
	{
		if (is_cppinterop())
		{
			config_cppinterop other = get_cppinterop_item();
			return other.get_sqlite3_item();
		}

		if (is_pinvoke())
		{
			config_sqlite3 other = projects.find_sqlite3(projects.cs_env_to_toolset(env), cpu);
			return other;
		}

		return null;
	}

	public config_cppinterop get_cppinterop_item()
	{
		foreach (config_cppinterop cfg in projects.items_cppinterop)
		{
			if (
					(cfg.env == env)
					&& (cfg.cpu == cpu)
			   )
			{
				return cfg;
			}
		}
		throw new Exception(get_name());
	}

	public bool needs_project_dot_json()
	{
		return config_cs.env_needs_project_dot_json(env);
	}

	private string nat()
	{
		if (is_pinvoke())
		{
			return string.Format("{0}", api);
		}
		else if (is_cppinterop())
		{
			return string.Format("{0}", api);
		}
		else
		{
			throw new Exception(get_name());
		}
	}

	public string get_nuget_target_subpath()
	{
		return string.Format("{0}\\{1}", nat(), cpu);
	}

	public string get_nuget_target_path(string where)
	{
		if ("build" == where)
		{
			if (is_portable())
			{
				throw new Exception(get_name());
			}
			else
			{
				return string.Format("build\\{0}\\{1}\\{2}\\", config_cs.get_nuget_framework_name(env), nat(), cpu);
			}
		}
		else if ("lib" == where)
		{
			if (is_portable())
			{
				return string.Format("lib\\{0}\\", projects.get_portable_nuget_target_string(env));
			}
			else
			{
				return string.Format("lib\\{0}\\", config_cs.get_nuget_framework_name(env));
			}
		}
		else
		{
			throw new Exception(get_name());
		}
	}

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_dest_subpath(), s));
	}

	public void get_products(List<string> a)
	{
		add_product(a, "SQLitePCL.raw.dll");
		switch (env)
		{
			case "win8":
			case "win81":
			case "wpa81":
			case "uap10.0":
				add_product(a, "SQLitePCL.raw.pri");
				break;
		}

		if (is_cppinterop())
		{
			config_cppinterop other = get_cppinterop_item();
			other.get_products(a);
		}
	}

	public bool is_portable()
	{
		return config_cs.env_is_portable(env);
	}

	public bool is_cppinterop()
	{
		if (is_portable())
		{
			return false;
		}

		return api == "cppinterop";
	}

	public bool is_pinvoke()
	{
		if (is_portable())
		{
			return false;
		}

		return api == "pinvoke";
	}

	private const string AREA = "pcl";

	public string get_dest_subpath()
	{
		if (is_portable())
		{
			return string.Format("{0}\\{1}", AREA, env);
		}
		else
		{
			return string.Format("{0}\\{1}\\{2}\\{3}", AREA, env, nat(), cpu);
		}
	}

	public string get_name()
	{
		if (is_portable())
		{
			return string.Format("portable.{0}", env);
		}
		else
		{
			return string.Format("platform.{0}.{1}.{2}", env, nat(), cpu);
		}
	}

	public string get_project_filename()
	{
		return string.Format("{0}.csproj", get_name());
	}
	
	public string fixed_cpu()
	{
		if (cpu == "anycpu")
		{
			return "Any CPU";
		}
		else
		{
			return cpu;
		}
	}
}

public static class gen
{
	private const string GUID_CSHARP = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
	private const string GUID_CPP = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
	private const string GUID_FOLDER = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
	private const string GUID_PCL = "{786C830F-07A1-408B-BD7F-6EE04809D6DB}";
	private const string GUID_IOS = "{6BC8ED88-2882-458C-8E55-DFD12B67127B}";
	private const string GUID_UNIFIED_IOS = "{FEACFBD2-3405-455C-9665-78FE426C6842}";
	private const string GUID_UNIFIED_MAC = "{A3F8F2AB-B479-4A4A-A458-A89E7DC349F1}";
	private const string GUID_ANDROID = "{EFBA0AD7-5A72-4C68-AF49-83D382785DCF}";
	private const string GUID_WINRT = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
	private const string GUID_WP8 = "{C089C8C0-30E0-4E22-80C0-CE093F111A43}";
	private const string GUID_WP81RT = "{76F1466A-8B6D-4E39-A767-685A06062A39}";
	private const string GUID_TEST = "{3AC096D0-A1C2-E12C-1390-A8335801FDAB}";
	private const string GUID_UAP = "{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A}";

	private static void write_reference(XmlWriter f, string s)
	{
		f.WriteStartElement("Reference");
		f.WriteAttributeString("Include", s);
		f.WriteEndElement(); // Reference
	}

	private static void write_cs_compile(XmlWriter f, string root, string s)
	{
		string path = Path.Combine(root, s);
		f.WriteStartElement("Compile");
		f.WriteAttributeString("Include", path);
		f.WriteEndElement(); // Compile
	}

	private static void write_cpp_includepath(XmlWriter f, string root, string s)
	{
		f.WriteElementString("IncludePath", string.Format("{0};$(IncludePath)", Path.Combine(root, s)));
	}

	private static void write_cpp_define(XmlWriter f, string s)
	{
		f.WriteElementString("PreprocessorDefinitions", string.Format("{0};%(PreprocessorDefinitions)", s));
	}

	private static void write_project_type_guids(XmlWriter f, string s1)
	{
		f.WriteElementString("ProjectTypeGuids", string.Format("{0}", s1));
	}

	private static void write_project_type_guids(XmlWriter f, string s1, string s2)
	{
		f.WriteElementString("ProjectTypeGuids", string.Format("{0};{1}", s1, s2));
	}

	private static void write_section(string top, string dest_subpath, XmlWriter f, bool debug, List<string> defines)
	{
		string name = debug ? "debug" : "release";
		f.WriteStartElement("PropertyGroup");
		f.WriteAttributeString("Condition", string.Format(" '$(Configuration)' == '{0}' ", name));

		f.WriteElementString("OutputPath", Path.Combine(top, string.Format("{0}\\bin\\{1}\\", name, dest_subpath)));
		f.WriteElementString("IntermediateOutputPath", Path.Combine(top, string.Format("{0}\\obj\\{1}\\", name, dest_subpath)));

		string defs;
		if (debug)
		{
			defs = "DEBUG;";
		}
		else
		{
			defs = "";
		}
		if (defines != null)
		{
			foreach (string d in defines)
			{
				defs += d;
				defs += ";";
			}
		}

		if (defs.Length > 0)
		{
			f.WriteElementString("DefineConstants", defs);
		}

		f.WriteElementString("DebugSymbols", debug ? "true" : "false");
		f.WriteElementString("Optimize", debug ? "false" : "true");
		f.WriteElementString("DebugType", debug ? "full" : "none");

		f.WriteEndElement(); // PropertyGroup
	}

	private static void write_section(string top, config_pcl cfg, XmlWriter f, bool debug, List<string> defines)
	{
		string name = debug ? "debug" : "release";
		f.WriteStartElement("PropertyGroup");
		f.WriteAttributeString("Condition", string.Format(" '$(Configuration)|$(Platform)' == '{0}|{1}' ", name, cfg.cpu));

		f.WriteElementString("OutputPath", Path.Combine(top, string.Format("{0}\\bin\\{1}\\", name, cfg.get_dest_subpath())));
		f.WriteElementString("IntermediateOutputPath", Path.Combine(top, string.Format("{0}\\obj\\{1}\\", name, cfg.get_dest_subpath())));

		string defs;
		if (debug)
		{
			defs = "DEBUG;";
		}
		else
		{
			defs = "";
		}
		foreach (string d in defines)
		{
			defs += d;
			defs += ";";
		}
		f.WriteElementString("DefineConstants", defs);

		f.WriteElementString("DebugSymbols", debug ? "true" : "false");
		f.WriteElementString("Optimize", debug ? "false" : "true");
		f.WriteElementString("DebugType", debug ? "full" : "none");

		f.WriteEndElement(); // PropertyGroup
	}

	private static void write_section(string top, config_plugin cfg, XmlWriter f, bool debug, List<string> defines)
	{
		string name = debug ? "debug" : "release";
		f.WriteStartElement("PropertyGroup");
		f.WriteAttributeString("Condition", string.Format(" '$(Configuration)' == '{0}' ", name));

		f.WriteElementString("OutputPath", Path.Combine(top, string.Format("{0}\\bin\\{1}\\", name, cfg.get_dest_subpath())));
		f.WriteElementString("IntermediateOutputPath", Path.Combine(top, string.Format("{0}\\obj\\{1}\\", name, cfg.get_dest_subpath())));

		string defs;
		if (debug)
		{
			defs = "DEBUG;";
		}
		else
		{
			defs = "";
		}
		foreach (string d in defines)
		{
			defs += d;
			defs += ";";
		}
		f.WriteElementString("DefineConstants", defs);

		f.WriteElementString("DebugSymbols", debug ? "true" : "false");
		f.WriteElementString("Optimize", debug ? "false" : "true");
		f.WriteElementString("DebugType", debug ? "full" : "none");

		f.WriteEndElement(); // PropertyGroup
	}

	private static void write_project_dot_json(string subdir)
	{
		using (TextWriter tw = new StreamWriter(Path.Combine(subdir, "project.json")))
		{
			tw.WriteLine("{");
			tw.WriteLine("    \"dependencies\" : {");
			tw.WriteLine("         \"Microsoft.NETCore.UniversalWindowsPlatform\": \"5.0.0\"");
			tw.WriteLine("    },");
			tw.WriteLine("    \"frameworks\" : {");
			tw.WriteLine("         \"uap10.0\": {}");
			tw.WriteLine("    },");
			tw.WriteLine("    \"runtimes\" : {");
			tw.WriteLine("         \"win10-arm\": {},");
			tw.WriteLine("         \"win10-arm-aot\": {},");
			tw.WriteLine("         \"win10-x86\": {},");
			tw.WriteLine("         \"win10-x86-aot\": {},");
			tw.WriteLine("         \"win10-x64\": {},");
			tw.WriteLine("         \"win10-x64-aot\": {}");
			tw.WriteLine("    }");
			tw.WriteLine("}");
		}
	}

	private static void write_csproj_footer(XmlWriter f, string env)
	{
		if (config_cs.env_is_portable(env))
		{
			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(MSBuildExtensionsPath32)\\Microsoft\\Portable\\$(TargetFrameworkVersion)\\Microsoft.Portable.CSharp.targets");
			f.WriteEndElement(); // Import
		}
		else
		{
			switch (env)
			{
				case "win8":
					f.WriteStartElement("PropertyGroup");
					f.WriteAttributeString("Condition", " '$(VisualStudioVersion)' == '' or '$(VisualStudioVersion)' < '11.0' ");
					f.WriteElementString("VisualStudioVersion", "11.0");
					f.WriteEndElement(); // PropertyGroup
					break;
				case "win81":
					f.WriteStartElement("PropertyGroup");
					f.WriteAttributeString("Condition", " '$(VisualStudioVersion)' == '' or '$(VisualStudioVersion)' < '12.0' ");
					f.WriteElementString("VisualStudioVersion", "12.0");
					f.WriteEndElement(); // PropertyGroup
					break;
				case "uap10.0":
					f.WriteStartElement("PropertyGroup");
					f.WriteAttributeString("Condition", " '$(VisualStudioVersion)' == '' or '$(VisualStudioVersion)' < '14.0' ");
					f.WriteElementString("VisualStudioVersion", "14.0");
					f.WriteEndElement(); // PropertyGroup
					break;
				case "wpa81":
					f.WriteStartElement("PropertyGroup");
					f.WriteAttributeString("Condition", " '$(VisualStudioVersion)' == '' or '$(VisualStudioVersion)' < '12.0' ");
					f.WriteElementString("VisualStudioVersion", "12.0");
					f.WriteEndElement(); // PropertyGroup

					f.WriteStartElement("PropertyGroup");
					f.WriteAttributeString("Condition", " '$(TargetPlatformIdentifier)' == '' ");
					f.WriteElementString("TargetPlatformIdentifier", "WindowsPhoneApp");
					f.WriteEndElement(); // PropertyGroup
					break;
				case "wp81_sl":
					break;
			}

			switch (env)
			{
				case "ios_unified":
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.iOS.CSharp.targets");
					f.WriteEndElement(); // Import

					break;
				case "unified_mac":
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Xamarin\\Mac\\Xamarin.Mac.CSharp.targets");
					f.WriteEndElement(); // Import

					break;
				case "ios_classic":
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.MonoTouch.CSharp.targets");
					f.WriteEndElement(); // Import

					break;
				case "android":
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Novell\\Novell.MonoDroid.CSharp.targets");
					f.WriteEndElement(); // Import

					break;
				case "win8":
				case "win81":
				case "wpa81":
				case "uap10.0":
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Microsoft\\WindowsXaml\\v$(VisualStudioVersion)\\Microsoft.Windows.UI.Xaml.CSharp.targets");
					f.WriteEndElement(); // Import
					break;
				case "net45":
				case "net40":
				case "net35":
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
					f.WriteEndElement(); // Import
					break;
				case "wp80":
				case "wp81_sl":
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Microsoft\\$(TargetFrameworkIdentifier)\\$(TargetFrameworkVersion)\\Microsoft.$(TargetFrameworkIdentifier).$(TargetFrameworkVersion).Overrides.targets");
					f.WriteEndElement(); // Import

					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Microsoft\\$(TargetFrameworkIdentifier)\\$(TargetFrameworkVersion)\\Microsoft.$(TargetFrameworkIdentifier).CSharp.targets");
					f.WriteEndElement(); // Import
					break;
			}
		}

	}

	private static void write_standard_assemblies(XmlWriter f, string env)
	{
		switch (env)
		{
			case "ios_classic":
			case "ios_unified":
			case "unified_mac":
			case "android":
			case "net45":
			case "net40":
			case "net35":
				write_reference(f, "System");
				write_reference(f, "System.Core");
				break;
		}
		switch (env)
		{
			case "ios_unified":
				write_reference(f, "Xamarin.iOS");
				break;
			case "unified_mac":
				write_reference(f, "Xamarin.Mac");
				break;
			case "ios_classic":
				write_reference(f, "monotouch");
				break;
			case "android":
				write_reference(f, "Mono.Android");
				break;
		}
	}

	private static List<string> write_header_properties(XmlWriter f, string env)
	{
		f.WriteElementString("DefaultLanguage", "en-us");
		//f.WriteElementString("FileAlignment", "512");
		f.WriteElementString("WarningLevel", "4");
		//f.WriteElementString("PlatformTarget", cfg.cpu.Replace(" ", ""));
		f.WriteElementString("OutputType", "Library");

		List<string> defines = new List<string>();

		if (config_cs.env_is_portable(env))
		{
			switch (env)
			{
				case "profile136":
				case "profile158":
					f.WriteElementString("TargetFrameworkVersion", "v4.0");
					break;
				case "profile111":
				case "profile78":
				case "profile259":
					f.WriteElementString("TargetFrameworkVersion", "v4.5");
					break;
				default:
					throw new Exception();
			}
			defines.Add("NO_CONCURRENTDICTIONARY");
			f.WriteElementString("TargetFrameworkProfile", env);
		}

		switch (env)
		{
			case "ios_classic":
				defines.Add("PLATFORM_IOS");
				break;
			case "ios_unified":
			case "unified_mac":
				defines.Add("PLATFORM_UNIFIED");
				break;
			case "android":
				defines.Add("__MOBILE__");
				defines.Add("__ANDROID__");
				f.WriteElementString("AndroidUseLatestPlatformdk", "true");
				break;
			case "win8":
				//f.WriteElementString("TargetPlatformVersion", "8.0");
				f.WriteElementString("UseVSHostingProcess", "false");
				//f.WriteElementString("MinimumVisualStudioVersion", "11.0");
				// TargetFrameworkVersion?
				defines.Add("NETFX_CORE");
				break;
			case "win81":
				f.WriteElementString("TargetPlatformVersion", "8.1");
				f.WriteElementString("MinimumVisualStudioVersion", "12.0");
				f.WriteElementString("TargetFrameworkVersion", null);
				defines.Add("NETFX_CORE");
				break;
			case "uap10.0":
				f.WriteElementString("TargetPlatformIdentifier", "UAP");
				f.WriteElementString("TargetPlatformVersion", "10.0.10240.0");
				f.WriteElementString("TargetPlatformMinVersion", "10.0.10240.0");
				f.WriteElementString("MinimumVisualStudioVersion", "14.0");
				f.WriteElementString("TargetFrameworkVersion", null);
				defines.Add("NETFX_CORE");
				defines.Add("WINDOWS_UWP");
				break;
			case "net45":
				f.WriteElementString("ProductVersion", "12.0.0");
				defines.Add("OLD_REFLECTION");
				f.WriteElementString("TargetFrameworkVersion", "v4.5");
				break;
			case "net40":
				f.WriteElementString("ProductVersion", "12.0.0");
				defines.Add("OLD_REFLECTION");
				f.WriteElementString("TargetFrameworkVersion", "v4.0");
				break;
			case "net35":
				f.WriteElementString("ProductVersion", "12.0.0");
				defines.Add("OLD_REFLECTION");
				defines.Add("NO_CONCURRENTDICTIONARY");
				f.WriteElementString("TargetFrameworkVersion", "v3.5");
				break;
			case "wp80":
				f.WriteElementString("TargetFrameworkIdentifier", "WindowsPhone");
				f.WriteElementString("TargetFrameworkVersion", "v8.0");
				f.WriteElementString("MinimumVisualStudioVersion", "11.0");
				f.WriteElementString("SilverlightVersion", "v8.0");
				f.WriteElementString("SilverlightApplication", "false");
				f.WriteElementString("ValidateXaml", "true");
				f.WriteElementString("ThrowErrorsInValidation", "true");
				defines.Add("WINDOWS_PHONE");
				defines.Add("SILVERLIGHT");
				defines.Add("NO_CONCURRENTDICTIONARY");
				f.WriteElementString("NoStdLib", "true");
				f.WriteElementString("NoConfig", "true");
				break;
			case "wpa81":
				f.WriteElementString("TargetPlatformVersion", "8.1");
				f.WriteElementString("MinimumVisualStudioVersion", "12.0");
				f.WriteElementString("UseVSHostingProcess", "false");
				defines.Add("NETFX_CORE");
				defines.Add("WINDOWS_PHONE_APP");
				break;
			case "wp81_sl":
				f.WriteElementString("TargetFrameworkIdentifier", "WindowsPhone");
				f.WriteElementString("TargetFrameworkVersion", "v8.1");
				f.WriteElementString("MinimumVisualStudioVersion", "12.0");
				f.WriteElementString("SilverlightVersion", "v8.1");
				f.WriteElementString("SilverlightApplication", "false");
				f.WriteElementString("ValidateXaml", "true");
				f.WriteElementString("ThrowErrorsInValidation", "true");
				defines.Add("WINDOWS_PHONE");
				defines.Add("SILVERLIGHT");
				defines.Add("NO_CONCURRENTDICTIONARY");
				f.WriteElementString("NoStdLib", "true");
				f.WriteElementString("NoConfig", "true");
				break;
		}

		return defines;
	}

	private static void write_toolsversion(XmlWriter f, string env)
	{
		switch (env)
		{
			case "uap10.0":
				f.WriteAttributeString("ToolsVersion", "14.0");
				break;
			case "win81":
			case "wpa81":
			case "wp81_sl":
				f.WriteAttributeString("ToolsVersion", "12.0");
				break;
			default:
				f.WriteAttributeString("ToolsVersion", "4.0");
				break;
		}
	}

	private static void write_project_type_guids_for_env(XmlWriter f, string env)
	{
		if (config_cs.env_is_portable(env))
		{
			write_project_type_guids(f, GUID_PCL, GUID_CSHARP);
		}
		else
		{
			switch (env)
			{
				case "ios_classic":
					write_project_type_guids(f, GUID_IOS, GUID_CSHARP);
					break;
				case "ios_unified":
					write_project_type_guids(f, GUID_UNIFIED_IOS, GUID_CSHARP);
					break;
				case "unified_mac":
					write_project_type_guids(f, GUID_UNIFIED_MAC, GUID_CSHARP);
					break;
				case "android":
					write_project_type_guids(f, GUID_ANDROID, GUID_CSHARP);
					break;
				case "win8":
				case "win81":
					write_project_type_guids(f, GUID_WINRT, GUID_CSHARP);
					break;
				case "wp80":
					write_project_type_guids(f, GUID_WP8, GUID_CSHARP);
					break;
				case "wpa81":
					write_project_type_guids(f, GUID_WP81RT, GUID_CSHARP);
					break;
				case "wp81_sl":
					write_project_type_guids(f, GUID_WP8, GUID_CSHARP);
					break;
				case "uap10.0":
					write_project_type_guids(f, GUID_UAP, GUID_CSHARP);
					break;
				default:
					write_project_type_guids(f, GUID_CSHARP);
					break;
			}
		}

	}

	private static void gen_sqlite3(config_sqlite3 cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string proj = cfg.get_project_path(top);
		using (XmlWriter f = XmlWriter.Create(proj, settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");
			f.WriteAttributeString("DefaultTargets", "Build");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Label", "ProjectConfigurations");

			f.WriteStartElement("ProjectConfiguration");
			f.WriteAttributeString("Include", string.Format("Debug|{0}", cfg.fixed_cpu()));
			f.WriteElementString("Configuration", "Debug");
			f.WriteElementString("Platform", cfg.fixed_cpu());
			f.WriteEndElement(); // ProjectConfiguration

			f.WriteStartElement("ProjectConfiguration");
			f.WriteAttributeString("Include", string.Format("Release|{0}", cfg.fixed_cpu()));
			f.WriteElementString("Configuration", "Release");
			f.WriteElementString("Platform", cfg.fixed_cpu());
			f.WriteEndElement(); // ProjectConfiguration

			f.WriteEndElement(); // ItemGroup

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ProjectGuid", cfg.guid);
			f.WriteElementString("Keyword", "Win32Proj");
			f.WriteElementString("DefaultLanguage", "en-us");

			switch (cfg.toolset)
			{
				case "v110_xp":
					break;
				case "v110":
					f.WriteElementString("MinimumVisualStudioVersion", "11.0");
					f.WriteElementString("WindowsAppContainer", "true");
					f.WriteElementString("AppContainerApplication", "true");
					break;
				case "v120":
					f.WriteElementString("MinimumVisualStudioVersion", "12.0");
					f.WriteElementString("WindowsAppContainer", "true");
					f.WriteElementString("AppContainerApplication", "true");
					break;
				case "v140":
					f.WriteElementString("MinimumVisualStudioVersion", "14.0");
					f.WriteElementString("ApplicationType", "Windows Store");
					f.WriteElementString("AppContainerApplication", "true");
					f.WriteElementString("ApplicationTypeRevision", "10.0");
					f.WriteElementString("WindowsTargetPlatformMinVersion", "10.0.10240.0");
					f.WriteElementString("WindowsTargetPlatformVersion", "10.0.10586.0");
					break;
				case "v110_wp80":
					f.WriteElementString("MinimumVisualStudioVersion", "11.0");
					break;
				case "v120_wp81":
					f.WriteElementString("MinimumVisualStudioVersion", "12.0");
					f.WriteElementString("AppContainerApplication", "true");
					f.WriteElementString("ApplicationType", "Windows Phone");
					f.WriteElementString("ApplicationTypeRevision", "8.1");
					break;
				default:
					throw new Exception("unknown toolset");
			}

			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.Default.props");
			f.WriteEndElement(); // Import

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ConfigurationType", "DynamicLibrary");
			f.WriteElementString("TargetName", "esqlite3");

			f.WriteElementString("PlatformToolset", cfg.toolset);

			f.WriteEndElement(); // PropertyGroup

			switch (cfg.toolset)
			{
				case "v110_xp":
					break;
				case "v110":
				case "v120":
				case "v110_wp80":
				case "v120_wp81":
				case "v140":
					f.WriteStartElement("ItemDefinitionGroup");
					f.WriteStartElement("ClCompile");
					write_cpp_define(f, "SQLITE_OS_WINRT");
					f.WriteEndElement(); // ClCompile
					f.WriteEndElement(); // ItemDefinitionGroup
					break;
				default:
					throw new Exception("unknown toolset");
			}

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteStartElement("ClCompile");
			write_cpp_define(f, "SQLITE_WIN32_FILEMAPPING_API=1");
			f.WriteEndElement(); // ClCompile
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("PropertyGroup");
			f.WriteAttributeString("Condition", string.Format(" '$(Configuration)' == 'Debug' "));
			f.WriteElementString("UseDebugLibraries", "true");
			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("PropertyGroup");
			f.WriteAttributeString("Condition", string.Format(" '$(Configuration)' == 'Release' "));
			f.WriteElementString("UseDebugLibraries", "false");
			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.props");
			f.WriteEndElement(); // Import

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("OutDir", Path.Combine(top, string.Format("$(Configuration)\\bin\\{0}\\", cfg.get_dest_subpath())));
			f.WriteElementString("IntDir", Path.Combine(top, string.Format("$(Configuration)\\obj\\{0}\\", cfg.get_dest_subpath())));
			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteStartElement("ClCompile");
			write_cpp_define(f, "_USRDLL");
			write_cpp_define(f, "SQLITE_API=__declspec(dllexport)");
			//write_cpp_define(f, "SQLITE_OMIT_LOAD_EXTENSION");
			// TODO write_cpp_define(f, "SQLITE_THREADSAFE=whatever");
			//write_cpp_define(f, "SQLITE_TEMP_STORE=whatever");
			write_cpp_define(f, "SQLITE_DEFAULT_FOREIGN_KEYS=1");
			write_cpp_define(f, "SQLITE_ENABLE_RTREE");
			write_cpp_define(f, "SQLITE_ENABLE_JSON1");
			// TODO FTS5?
			write_cpp_define(f, "SQLITE_ENABLE_FTS4");
			write_cpp_define(f, "SQLITE_ENABLE_FTS3_PARENTHESIS");
			write_cpp_define(f, "SQLITE_ENABLE_COLUMN_METADATA");
			f.WriteElementString("PrecompiledHeader", "NotUsing");
			f.WriteElementString("CompileAsWinRT", "false");
			f.WriteElementString("SDLCheck", "false");
			f.WriteEndElement(); // ClCompile
			f.WriteStartElement("Link");
			f.WriteElementString("SubSystem", "Console");
			f.WriteElementString("IgnoreAllDefaultLibraries", "false");
			f.WriteElementString("GenerateWindowsMetadata", "false");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteAttributeString("Condition", string.Format("'$(Configuration)'=='{0}' ", "Debug"));
			f.WriteStartElement("ClCompile");
			f.WriteElementString("Optimization", "Disabled");
			write_cpp_define(f, "_DEBUG");
			f.WriteEndElement(); // ClCompile
			f.WriteStartElement("Link");
			f.WriteElementString("GenerateDebugInformation", "true");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteStartElement("ClCompile");
			f.WriteAttributeString("Condition", string.Format("'$(Configuration)'=='{0}' ", "Release"));
			f.WriteElementString("Optimization", "MaxSpeed");
			f.WriteElementString("FunctionLevelLinking", "true");
			f.WriteElementString("IntrinsicFunctions", "true");
			if (cfg.toolset == "v110_xp") {
				f.WriteElementString("RuntimeLibrary", "MultiThreaded");
			}
			write_cpp_define(f, "NDEBUG");
			f.WriteEndElement(); // ClCompile
			f.WriteStartElement("Link");
			f.WriteElementString("GenerateDebugInformation", "false");
			f.WriteElementString("EnableCOMDATFolding", "true");
			f.WriteElementString("OptimizeReferences", "true");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemGroup");
			{
				string path = Path.Combine(root, "sqlite3\\sqlite3.c");
				f.WriteStartElement("ClCompile");
				f.WriteAttributeString("Include", path);
				f.WriteEndElement(); // ClCompile
			}
			f.WriteEndElement(); // ItemGroup

			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.targets");
			f.WriteEndElement(); // Import

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static void gen_cppinterop(config_cppinterop cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string proj = cfg.get_project_path(top);
		using (XmlWriter f = XmlWriter.Create(proj, settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			switch (cfg.env)
			{
				case "wp80":
					f.WriteAttributeString("ToolsVersion", "4.0");
					break;
				case "wp81_sl":
					f.WriteAttributeString("ToolsVersion", "12.0");
					break;
				default:
					throw new Exception("invalid cppinterop env");
			}
			f.WriteAttributeString("DefaultTargets", "Build");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Label", "ProjectConfigurations");

			f.WriteStartElement("ProjectConfiguration");
			f.WriteAttributeString("Include", string.Format("Debug|{0}", cfg.fixed_cpu()));
			f.WriteElementString("Configuration", "Debug");
			f.WriteElementString("Platform", cfg.fixed_cpu());
			f.WriteEndElement(); // ProjectConfiguration

			f.WriteStartElement("ProjectConfiguration");
			f.WriteAttributeString("Include", string.Format("Release|{0}", cfg.fixed_cpu()));
			f.WriteElementString("Configuration", "Release");
			f.WriteElementString("Platform", cfg.fixed_cpu());
			f.WriteEndElement(); // ProjectConfiguration

			f.WriteEndElement(); // ItemGroup

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ProjectGuid", cfg.guid);
			f.WriteElementString("DefaultLanguage", "en-us");
			f.WriteElementString("RootNamespace", "SQLitePCL.cppinterop");

			switch (cfg.env)
			{
				case "wp80":
					f.WriteElementString("WinMDAssembly", "true");
					f.WriteElementString("MinimumVisualStudioVersion", "11.0");
					break;
				case "wp81_sl":
					f.WriteElementString("Keyword", "Win32Proj");
					f.WriteElementString("MinimumVisualStudioVersion", "12.0");
					f.WriteElementString("AppContainerApplication", "true");
					f.WriteElementString("ApplicationType", "Windows Phone Silverlight");
					f.WriteElementString("ApplicationTypeRevision", "8.1");
					break;
			}

			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.Default.props");
			f.WriteEndElement(); // Import

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ConfigurationType", "DynamicLibrary");

			switch (cfg.env)
			{
				case "wp80":
					f.WriteElementString("PlatformToolset", "v110_wp80");
					break;
				case "wp81_sl":
					f.WriteElementString("PlatformToolset", "v120");
					break;
			}

			f.WriteEndElement(); // PropertyGroup

			switch (cfg.env)
			{
				case "wp80":
					f.WriteStartElement("ItemDefinitionGroup");
					f.WriteStartElement("ClCompile");
					write_cpp_define(f, "_WINRT_DLL");
					f.WriteElementString("AdditionalUsingDirectories", "$(WindowsSDK_WindowsMetaData);$(AdditionalUsingDirectories)");
					f.WriteElementString("CompileAsWinRT", "true");
					f.WriteEndElement(); // ClCompile
					f.WriteStartElement("Link");
					f.WriteElementString("GenerateWindowsMetadata", "true");
					f.WriteElementString("IgnoreSpecificDefaultLibraries", "ole32.lib;%(IgnoreSpecificDefaultLibraries)");
					f.WriteElementString("AdditionalDependencies", "WindowsPhoneCore.lib;runtimeobject.lib;PhoneAppModelHost.lib;%(AdditionalDependencies)");
					f.WriteEndElement(); // Link
					f.WriteEndElement(); // ItemDefinitionGroup
					break;
				case "wp81_sl":
					f.WriteStartElement("ItemDefinitionGroup");
					f.WriteStartElement("ClCompile");
					write_cpp_define(f, "_WINRT_DLL");
					f.WriteEndElement(); // ClCompile
					f.WriteStartElement("Link");
					f.WriteEndElement(); // Link
					f.WriteEndElement(); // ItemDefinitionGroup
					break;
			}

			f.WriteStartElement("PropertyGroup");
			f.WriteAttributeString("Condition", string.Format(" '$(Configuration)' == 'Debug' "));
			f.WriteElementString("UseDebugLibraries", "true");
			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("PropertyGroup");
			f.WriteAttributeString("Condition", string.Format(" '$(Configuration)' == 'Release' "));
			f.WriteElementString("UseDebugLibraries", "false");
			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.props");
			f.WriteEndElement(); // Import

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("TargetName", "SQLitePCL.cppinterop");
			f.WriteElementString("OutDir", Path.Combine(top, string.Format("$(Configuration)\\bin\\{0}\\", cfg.get_dest_subpath())));
			f.WriteElementString("IntDir", Path.Combine(top, string.Format("$(Configuration)\\obj\\{0}\\", cfg.get_dest_subpath())));
			write_cpp_includepath(f, root, "sqlite3\\");
			f.WriteElementString("LinkIncremental", "false");
			f.WriteElementString("GenerateManifest", "false");
			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteStartElement("ClCompile");
			f.WriteElementString("PrecompiledHeader", "NotUsing");
			f.WriteElementString("AdditionalOptions", "/bigobj %(AdditionalOptions)");
			//f.WriteElementString("CompileAsWinRT", "false");
			//f.WriteElementString("SDLCheck", "false");
			f.WriteEndElement(); // ClCompile
			f.WriteStartElement("Link");
			f.WriteElementString("SubSystem", "Console");
			f.WriteElementString("IgnoreAllDefaultLibraries", "false");
			string sqlite3_item_path = Path.Combine(top, string.Format("$(Configuration)\\bin\\{0}\\esqlite3.lib", cfg.get_sqlite3_item().get_dest_subpath()));
			f.WriteElementString("AdditionalDependencies", string.Format("{0};%(AdditionalDependencies)", sqlite3_item_path));
			//f.WriteElementString("GenerateWindowsMetadata", "false");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteAttributeString("Condition", string.Format("'$(Configuration)'=='{0}' ", "Debug"));
			f.WriteStartElement("ClCompile");
			f.WriteElementString("Optimization", "Disabled");
			write_cpp_define(f, "_DEBUG");
			f.WriteEndElement(); // ClCompile
			f.WriteStartElement("Link");
			f.WriteElementString("GenerateDebugInformation", "true");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteStartElement("ClCompile");
			f.WriteAttributeString("Condition", string.Format("'$(Configuration)'=='{0}' ", "Release"));
			f.WriteElementString("Optimization", "MaxSpeed");
			//f.WriteElementString("FunctionLevelLinking", "true");
			//f.WriteElementString("IntrinsicFunctions", "true");
			write_cpp_define(f, "NDEBUG");
			f.WriteEndElement(); // ClCompile
			f.WriteStartElement("Link");
			f.WriteElementString("GenerateDebugInformation", "false");
			//f.WriteElementString("EnableCOMDATFolding", "true");
			//f.WriteElementString("OptimizeReferences", "true");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemGroup");
			f.WriteStartElement("ClCompile");
			f.WriteAttributeString("Include", Path.Combine(root, "src\\cpp\\sqlite3_cx.cpp"));
			f.WriteEndElement(); // ClCompile
			f.WriteEndElement(); // ItemGroup

			switch (cfg.env)
			{
				case "wp80":
					f.WriteStartElement("ItemGroup");
					f.WriteStartElement("Reference");
					f.WriteAttributeString("Include", "platform.winmd");
					f.WriteElementString("IsWinMDFile", "true");
					f.WriteElementString("Private", "false");
					f.WriteEndElement(); // Reference
					f.WriteEndElement(); // ItemGroup
					break;
				case "wp81_sl":
					break;
			}

#if not
			// TODO if this supported other ways of getting sqlite, like
			// _dynamic_sqlite3 or _static_sqlcipher, the following would be different

			f.WriteStartElement("ItemGroup");
			f.WriteStartElement("ProjectReference");
			{
				config_sqlite3 other = cfg.get_sqlite3_item();
				f.WriteAttributeString("Include", other.get_project_filename());
				f.WriteElementString("Project", other.guid);
			}
			f.WriteEndElement(); // ProjectReference
			f.WriteEndElement(); // ItemGroup
#endif

			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.targets");
			f.WriteEndElement(); // Import

			switch (cfg.env)
			{
				case "wp80":
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Microsoft\\WindowsPhone\\v$(TargetPlatformVersion)\\Microsoft.Cpp.WindowsPhone.$(TargetPlatformVersion).targets");
					f.WriteEndElement(); // Import
					break;
				case "wp81_sl":
					break;
			}

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

    private static void write_android_native_libs(string root, XmlWriter f, string which)
    {
        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\x86\\libesqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("x86\\libesqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\x86_64\\libesqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("x86_64\\libesqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\armeabi\\libesqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("armeabi\\libesqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\arm64-v8a\\libesqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("arm64-v8a\\libesqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\armeabi-v7a\\libesqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("armeabi-v7a\\libesqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

#if not
        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\mips\\libesqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("mips\\libesqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\mips64\\libesqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("mips64\\libesqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary
#endif

    }

    private static void write_android_native_libs_sqlcipher(string root, XmlWriter f)
    {
        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("couchbase-lite-libsqlcipher\\libs\\android\\x86\\libsqlcipher.so")));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("x86\\libsqlcipher.so"));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("couchbase-lite-libsqlcipher\\libs\\android\\x86_64\\libsqlcipher.so")));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("x86_64\\libsqlcipher.so"));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("couchbase-lite-libsqlcipher\\libs\\android\\armeabi\\libsqlcipher.so")));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("armeabi\\libsqlcipher.so"));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("couchbase-lite-libsqlcipher\\libs\\android\\arm64-v8a\\libsqlcipher.so")));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("arm64-v8a\\libsqlcipher.so"));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("couchbase-lite-libsqlcipher\\libs\\android\\armeabi-v7a\\libsqlcipher.so")));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("armeabi-v7a\\libsqlcipher.so"));
        f.WriteEndElement(); // EmbeddedNativeLibrary

#if not
        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("couchbase-lite-libsqlcipher\\libs\\android\\mips\\libsqlcipher.so")));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("mips\\libsqlcipher.so"));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("couchbase-lite-libsqlcipher\\libs\\android\\mips64\\libsqlcipher.so")));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("mips64\\libsqlcipher.so"));
        f.WriteEndElement(); // EmbeddedNativeLibrary
#endif

    }

	private static void gen_plugin(config_plugin cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string proj = cfg.get_project_path(top);
		using (XmlWriter f = XmlWriter.Create(proj, settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			write_toolsversion(f, cfg.env);
			f.WriteAttributeString("DefaultTargets", "Build");

			// TODO is this actually needed?
			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
			f.WriteAttributeString("Condition", "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')");
			f.WriteEndElement(); // Import

			f.WriteStartElement("PropertyGroup");

			f.WriteElementString("ProjectGuid", cfg.guid);

			write_project_type_guids_for_env(f, cfg.env);

			f.WriteStartElement("Configuration");
			f.WriteAttributeString("Condition", " '$(Configuration)' == '' ");

			f.WriteString("Debug");

			f.WriteEndElement(); // Configuration

			f.WriteElementString("SchemaVersion", "2.0");
			f.WriteElementString("Platform", "AnyCPU");
			f.WriteElementString("DefaultLanguage", "en-us");
			//f.WriteElementString("FileAlignment", "512");
			f.WriteElementString("WarningLevel", "4");
			//f.WriteElementString("PlatformTarget", cfg.cpu.Replace(" ", ""));
			f.WriteElementString("OutputType", "Library");
			f.WriteElementString("RootNamespace", "SQLitePCL");
			f.WriteElementString("AssemblyName", "SQLite3Plugin"); // match the name in get_products()

			List<string> defines = write_header_properties(f, cfg.env);

			switch (cfg.env)
			{
				case "net35":
				case "net40":
				case "net45":
					defines.Add("PRELOAD_FROM_ARCH_DIRECTORY");
					break;
			}

			switch (cfg.what)
			{
				case "sqlite3":
					defines.Add("IOS_PACKAGED_SQLITE3");
					break;
				case "sqlcipher":
					defines.Add("IOS_PACKAGED_SQLCIPHER");
					break;
				default:
					throw new Exception(cfg.what);
			}

			f.WriteEndElement(); // PropertyGroup

			write_section(top, cfg, f, true, defines);
			write_section(top, cfg, f, false, defines);

			f.WriteStartElement("ItemGroup");
			write_standard_assemblies(f, cfg.env);
			f.WriteEndElement(); // ItemGroup

			switch (cfg.env)
			{
				case "ios_unified":
				case "ios_classic":
					f.WriteStartElement("ItemGroup");
					write_cs_compile(f, root, "src\\cs\\imp_ios_internal.cs");
					write_cs_compile(f, top, "pinvoke_ios_internal.cs");
					write_cs_compile(f, root, "src\\cs\\util.cs");
					f.WriteEndElement(); // ItemGroup
					break;
				case "android":
					f.WriteStartElement("ItemGroup");
					switch (cfg.what)
					{
						case "sqlite3":
							write_cs_compile(f, root, "src\\cs\\imp_esqlite3.cs");
							write_cs_compile(f, top, "pinvoke_esqlite3.cs");
							break;
						case "sqlcipher":
							write_cs_compile(f, root, "src\\cs\\imp_sqlcipher.cs");
							write_cs_compile(f, top, "pinvoke_sqlcipher.cs");
							break;
						default:
							throw new Exception(cfg.what);
					}
					write_cs_compile(f, root, "src\\cs\\util.cs");
					f.WriteEndElement(); // ItemGroup
					break;
				default:
					f.WriteStartElement("ItemGroup");
					switch (cfg.what)
					{
						case "sqlite3":
							write_cs_compile(f, root, "src\\cs\\imp_esqlite3.cs");
							write_cs_compile(f, top, "pinvoke_esqlite3.cs");
							break;
						case "sqlcipher":
							write_cs_compile(f, root, "src\\cs\\imp_sqlcipher.cs");
							write_cs_compile(f, top, "pinvoke_sqlcipher.cs");
							break;
						default:
							throw new Exception(cfg.what);
					}
					write_cs_compile(f, root, "src\\cs\\util.cs");
					f.WriteEndElement(); // ItemGroup
					break;
			}

			f.WriteStartElement("ItemGroup");
			f.WriteStartElement("ProjectReference");
			{
				config_pcl other = projects.find_bait(cfg.env);
				f.WriteAttributeString("Include", other.get_project_path(top));
				f.WriteElementString("Project", other.guid);
				f.WriteElementString("Name", other.get_name());
				//f.WriteElementString("Private", "true");
			}
			f.WriteEndElement(); // ProjectReference
			f.WriteEndElement(); // ItemGroup

			switch (cfg.env)
			{
				case "ios_unified":
                        if (cfg.what == "sqlite3")
                        {
                            f.WriteStartElement("ItemGroup");
                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            f.WriteAttributeString("Include", Path.Combine(root, "apple\\libs\\ios\\sqlite3\\esqlite3.a"));
                            f.WriteElementString("Link", "esqlite3.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture
                            f.WriteEndElement(); // ItemGroup
                        }
			else if (cfg.what == "sqlcipher")
                        {
                            f.WriteStartElement("ItemGroup");

                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            f.WriteAttributeString("Include", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\ios\\libsqlcipher.a"));
                            f.WriteElementString("Link", "libsqlcipher.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture

                            f.WriteEndElement(); // ItemGroup
                        }
					break;

					case "ios_classic":
                        if (cfg.what == "sqlite3")
                        {
                            f.WriteStartElement("ItemGroup");
                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            f.WriteAttributeString("Include", Path.Combine(root, "apple\\libs\\ios\\sqlite3\\esqlite3.a"));
                            f.WriteElementString("Link", "esqlite3.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture
                            f.WriteEndElement(); // ItemGroup
                        }
			else if (cfg.what == "sqlcipher")
                        {
                            f.WriteStartElement("ItemGroup");

                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            f.WriteAttributeString("Include", Path.Combine(root, "apple\\libs\\ios\\sqlcipher\\esqlite3.a"));
                            f.WriteElementString("Link", "esqlite3.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture

                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            f.WriteAttributeString("Include", Path.Combine(root, "apple\\libs\\ios\\libcrypto.a"));
                            f.WriteElementString("Link", "libcrypto.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture

                            f.WriteEndElement(); // ItemGroup
                        }
						break;

					case "android":
						if (cfg.what == "sqlite3")
						{
                            f.WriteStartElement("ItemGroup");
                            write_android_native_libs(root, f, "sqlite3");
                            f.WriteEndElement(); // ItemGroup
						}

						if (cfg.what == "sqlcipher")
						{
                            f.WriteStartElement("ItemGroup");
                            write_android_native_libs_sqlcipher(root, f);
                            f.WriteEndElement(); // ItemGroup
                        }

						break;
				}

			write_csproj_footer(f, cfg.env);

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}

		if (config_cs.env_needs_project_dot_json(cfg.env))
		{
			string subdir = cfg.get_project_subdir(top);
			write_project_dot_json(subdir);
		}
	}

	private static void gen_pcl(config_pcl cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string proj = cfg.get_project_path(top);
		using (XmlWriter f = XmlWriter.Create(proj, settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			write_toolsversion(f, cfg.env);
			f.WriteAttributeString("DefaultTargets", "Build");

			switch (cfg.env)
			{
				case "wp81_sl":
					break;
				default:
					// TODO is this actually needed?
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
					f.WriteAttributeString("Condition", "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')");
					f.WriteEndElement(); // Import
					break;
			}

			f.WriteStartElement("PropertyGroup");

			f.WriteElementString("ProjectGuid", cfg.guid);
			write_project_type_guids_for_env(f, cfg.env);

			f.WriteStartElement("Configuration");
			f.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
			f.WriteString("Debug");
			f.WriteEndElement(); // Configuration

			if (cfg.env == "unified_mac") {
				f.WriteElementString ("TargetFrameworkVersion", "v2.0");
				f.WriteElementString ("TargetFrameworkIdentifier", "Xamarin.Mac");
			}

			f.WriteElementString("SchemaVersion", "2.0");
			f.WriteElementString("Platform", cfg.cpu.Replace(" ", ""));
			f.WriteElementString("RootNamespace", "SQLitePCL");
			f.WriteElementString("AssemblyName", "SQLitePCL.raw"); // match the name in get_products()

			List<string> defines = write_header_properties(f, cfg.env);

			if (cfg.is_portable())
			{
				defines.Add("USE_PROVIDER_BAIT");
			}
			else if (cfg.is_cppinterop())
			{
				defines.Add("USE_PROVIDER_CPPINTEROP");
			}
			else if (cfg.is_pinvoke())
			{
				defines.Add("USE_PROVIDER_PINVOKE");
			}

			f.WriteEndElement(); // PropertyGroup

			write_section(top, cfg, f, true, defines);
			write_section(top, cfg, f, false, defines);

			f.WriteStartElement("ItemGroup");
			write_standard_assemblies(f, cfg.env);
			f.WriteEndElement(); // ItemGroup

			f.WriteStartElement("ItemGroup");
			write_cs_compile(f, root, "src\\cs\\AssemblyInfo.cs");
			write_cs_compile(f, root, "src\\cs\\raw.cs");
			write_cs_compile(f, root, "src\\cs\\intptrs.cs");
			write_cs_compile(f, root, "src\\cs\\isqlite3.cs");
			f.WriteEndElement(); // ItemGroup

			if (cfg.is_portable())
			{
				f.WriteStartElement("ItemGroup");
				write_cs_compile(f, root, "src\\cs\\sqlite3_bait.cs");
				f.WriteEndElement(); // ItemGroup
			}
			else if (cfg.is_cppinterop())
			{
				f.WriteStartElement("ItemGroup");
				write_cs_compile(f, root, "src\\cs\\sqlite3_cppinterop.cs");
				write_cs_compile(f, root, "src\\cs\\util.cs");
				f.WriteEndElement(); // ItemGroup
			}
			else if (cfg.is_pinvoke())
			{
				f.WriteStartElement("ItemGroup");
				write_cs_compile(f, top, "pinvoke_default.cs");
				write_cs_compile(f, root, "src\\cs\\util.cs");
				f.WriteEndElement(); // ItemGroup
			}

			if (cfg.is_cppinterop())
			{
				f.WriteStartElement("ItemGroup");
				f.WriteStartElement("ProjectReference");
				{
					config_cppinterop other = cfg.get_cppinterop_item();
					f.WriteAttributeString("Include", other.get_project_path(top));
					f.WriteElementString("Project", other.guid);
					f.WriteElementString("Name", other.get_name());
					//f.WriteElementString("Private", "true");
				}
				f.WriteEndElement(); // ProjectReference
				f.WriteEndElement(); // ItemGroup
			}

			write_csproj_footer(f, cfg.env);

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}

		if (cfg.needs_project_dot_json())
		{
			string subdir = cfg.get_project_subdir(top);
			write_project_dot_json(subdir);
		}
	}

	// TODO the following function works when cfg.env is win8.  it might
	// not work for any other configuration.  for now, that's fine.
	// TODO broken?
	private static void gen_tests(config_tests cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string proj = cfg.get_project_path(top);
		using (XmlWriter f = XmlWriter.Create(proj, settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			write_toolsversion(f, cfg.env);
			f.WriteAttributeString("DefaultTargets", "Build");

			switch (cfg.env)
			{
				case "wp81_sl":
					break;
				default:
					// TODO is this actually needed?
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
					f.WriteAttributeString("Condition", "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')");
					f.WriteEndElement(); // Import
					break;
			}

			f.WriteStartElement("PropertyGroup");

			f.WriteElementString("ProjectGuid", cfg.guid);
			write_project_type_guids_for_env(f, cfg.env);

			f.WriteStartElement("Configuration");
			f.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
			f.WriteString("Debug");
			f.WriteEndElement(); // Configuration

			if (cfg.env == "unified_mac") {
				f.WriteElementString ("TargetFrameworkVersion", "v2.0");
				f.WriteElementString ("TargetFrameworkIdentifier", "Xamarin.Mac");
			}

			f.WriteElementString("SchemaVersion", "2.0");
			//f.WriteElementString("Platform", cfg.cpu.Replace(" ", ""));
			f.WriteElementString("DefaultLanguage", "en-us");
			//f.WriteElementString("FileAlignment", "512");
			f.WriteElementString("WarningLevel", "4");
			//f.WriteElementString("PlatformTarget", cfg.cpu.Replace(" ", ""));
			f.WriteElementString("OutputType", "Library");
			// TODO f.WriteElementString("RootNamespace", "SQLitePCL");
			// TODO f.WriteElementString("AssemblyName", "SQLitePCL"); // match the name in get_products()

			List<string> defines = write_header_properties(f, cfg.env);
			f.WriteElementString("TestProjectType", "UnitTest");

			// TODO maybe define NUNIT

			f.WriteEndElement(); // PropertyGroup

			write_section(top, cfg.get_dest_subpath(), f, true, defines);
			write_section(top, cfg.get_dest_subpath(), f, false, defines);

			f.WriteStartElement("ItemGroup");
			write_standard_assemblies(f, cfg.env);
			switch (cfg.env)
			{
				case "net45":
					write_reference(f, "Microsoft.VisualStudio.QualityTools.UnitTestFramework");
					break;
				case "win8":
					f.WriteStartElement("SDKReference");
					f.WriteAttributeString("Include", "MSTestFramework, Version=11.0");
					f.WriteEndElement(); // SDKReference
					break;
			}
			f.WriteEndElement(); // ItemGroup

			f.WriteStartElement("ItemGroup");
			write_cs_compile(f, root, "src\\cs\\test_cases.cs");
			f.WriteEndElement(); // ItemGroup

			f.WriteStartElement("ItemGroup");
			f.WriteStartElement("ProjectReference");
			if (cfg.env == "win8")
			{
				config_pcl other = projects.find_bait(cfg.pcl);
				f.WriteAttributeString("Include", other.get_project_path(top));
				f.WriteElementString("Project", other.guid);
				f.WriteElementString("Name", other.get_name());
				//f.WriteElementString("Private", "true");
			}
			f.WriteEndElement(); // ProjectReference
			f.WriteStartElement("ProjectReference");
			{
				config_higher other = projects.find_ugly(cfg.pcl);
				f.WriteAttributeString("Include", other.get_project_path(top));
				f.WriteElementString("Project", other.guid);
				f.WriteElementString("Name", other.get_name());
				//f.WriteElementString("Private", "true");
			}
			f.WriteEndElement(); // ProjectReference
			f.WriteEndElement(); // ItemGroup

			write_csproj_footer(f, cfg.env);

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static void gen_higher(
			config_higher cfg,
			string root, 
			string top
			)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string proj = cfg.get_project_path(top);
		using (XmlWriter f = XmlWriter.Create(proj, settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			write_toolsversion(f, cfg.env);
			f.WriteAttributeString("DefaultTargets", "Build");

			// TODO is this actually needed?
			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
			f.WriteAttributeString("Condition", "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')");
			f.WriteEndElement(); // Import

			f.WriteStartElement("PropertyGroup");

			f.WriteElementString("ProjectGuid", cfg.guid);
			write_project_type_guids_for_env(f, cfg.env);

			f.WriteStartElement("Configuration");
			f.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
			f.WriteString("Debug");
			f.WriteEndElement(); // Configuration

			f.WriteElementString("SchemaVersion", "2.0");
			f.WriteElementString("Platform", "AnyCPU");
			f.WriteElementString("DefaultLanguage", "en-us");
			//f.WriteElementString("FileAlignment", "512");
			f.WriteElementString("WarningLevel", "4");
			//f.WriteElementString("PlatformTarget", cfg.cpu.Replace(" ", ""));
			//f.WriteElementString("RootNamespace", "whatever"); // TODO
			f.WriteElementString("AssemblyName", cfg.assemblyname);

			List<string> defines = write_header_properties(f, cfg.env);

			f.WriteEndElement(); // PropertyGroup

			write_section(top, cfg.get_dest_subpath(), f, true, cfg.defines);
			write_section(top, cfg.get_dest_subpath(), f, false, cfg.defines);

			f.WriteStartElement("ItemGroup");
			foreach (string csfile in cfg.csfiles)
			{
				write_cs_compile(f, root, csfile);
			}
			f.WriteEndElement(); // ItemGroup

			f.WriteStartElement("ItemGroup");
			f.WriteStartElement("ProjectReference");
			{
				config_pcl other = projects.find_bait(cfg.env);
				f.WriteAttributeString("Include", other.get_project_path(top));
				f.WriteElementString("Project", other.guid);
				f.WriteElementString("Name", other.get_name());
				//f.WriteElementString("Private", "true");
			}
			f.WriteEndElement(); // ProjectReference
			f.WriteEndElement(); // ItemGroup

			if (cfg.is_portable()) 
			{
			}
			else 
			{
				f.WriteStartElement("ItemGroup");
				write_reference(f, "System");
				write_reference(f, "System.Core");
				f.WriteEndElement(); // ItemGroup
			}

			if (cfg.is_portable()) 
			{
				f.WriteStartElement("Import");
				f.WriteAttributeString("Project", "$(MSBuildExtensionsPath32)\\Microsoft\\Portable\\$(TargetFrameworkVersion)\\Microsoft.Portable.CSharp.targets");
				f.WriteEndElement(); // Import
			} 
			else
			{
				f.WriteStartElement("Import");
				f.WriteAttributeString("Project", "$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
				f.WriteEndElement(); // Import
			}

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	public static string write_folder(StreamWriter f, string name)
	{
		string folder_guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		f.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
				GUID_FOLDER,
				name,
				name,
				folder_guid
				);
		f.WriteLine("EndProject");
		return folder_guid;
	}

	public static void gen_solution(string top)
	{
		using (StreamWriter f = new StreamWriter(Path.Combine(top, "sqlitepcl.sln")))
		{
			// TODO change to VS 2015
			f.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			f.WriteLine("# Visual Studio 2013");
			f.WriteLine("VisualStudioVersion = 12.0");
			f.WriteLine("MinimumVisualStudioVersion = 12.0");

			// solution folders

			string folder_sqlite3 = write_folder(f, "sqlite3");
			string folder_cppinterop = write_folder(f, "cppinterop");
			string folder_platforms = write_folder(f, "platforms");
			string folder_portable = write_folder(f, "portable");
			string folder_plugin = write_folder(f, "plugin");

			foreach (config_sqlite3 cfg in projects.items_sqlite3)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\\{2}\", \"{3}\"",
						GUID_CPP,
						cfg.get_name(),
						cfg.get_project_filename(),
						cfg.guid
						);
				f.WriteLine("EndProject");
			}

			foreach (config_cppinterop cfg in projects.items_cppinterop)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\\{2}\", \"{3}\"",
						GUID_CPP,
						cfg.get_name(),
						cfg.get_project_filename(),
						cfg.guid
						);
				f.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
				config_sqlite3 other = cfg.get_sqlite3_item();
				f.WriteLine("\t\t{0} = {0}", other.guid);
				f.WriteLine("\tEndProjectSection");

				f.WriteLine("EndProject");
			}

			foreach (config_pcl cfg in projects.items_pcl)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\\{2}\", \"{3}\"",
						GUID_CSHARP,
						cfg.get_name(),
						cfg.get_project_filename(),
						cfg.guid
						);
				if (cfg.is_cppinterop())
				{
					f.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
					config_cppinterop other = cfg.get_cppinterop_item();
					f.WriteLine("\t\t{0} = {0}", other.guid);
					f.WriteLine("\tEndProjectSection");
				}
				f.WriteLine("EndProject");
			}

			foreach (config_plugin cfg in projects.items_plugin)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\\{2}\", \"{3}\"",
						GUID_CSHARP,
						cfg.get_name(),
						cfg.get_project_filename(),
						cfg.guid
						);
				switch (cfg.env)
				{
					case "ios_classic":
					case "ios_unified":
					case "android":
						break;
					default:
						f.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
						foreach (config_sqlite3 other in cfg.get_sqlite3_items()) {
							f.WriteLine("\t\t{0} = {0}", other.guid);
						}
						f.WriteLine("\tEndProjectSection");
						break;
				}
				f.WriteLine("EndProject");
			}

			foreach (config_higher cfg in projects.items_higher)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\\{2}\", \"{3}\"",
						GUID_CSHARP,
						cfg.get_name(),
						cfg.get_project_filename(),
						cfg.guid
						);
				f.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
				config_pcl other = projects.find_bait(cfg.env);
				f.WriteLine("\t\t{0} = {0}", other.guid);
				f.WriteLine("\tEndProjectSection");
				f.WriteLine("EndProject");
			}

			foreach (config_tests cfg in projects.items_tests)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\\{2}\", \"{3}\"",
						GUID_CSHARP,
						cfg.get_name(),
						cfg.get_project_filename(),
						cfg.guid
						);
				f.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
				config_pcl other = projects.find_bait(cfg.pcl);
				f.WriteLine("\t\t{0} = {0}", other.guid);
				f.WriteLine("\tEndProjectSection");
				f.WriteLine("EndProject");
			}

			f.WriteLine("Global");

			f.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			f.WriteLine("\t\tDebug|Mixed Platforms = Debug|Mixed Platforms");
			f.WriteLine("\t\tRelease|Mixed Platforms = Release|Mixed Platforms");
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
			foreach (config_sqlite3 cfg in projects.items_sqlite3)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.fixed_cpu());
			}
			foreach (config_cppinterop cfg in projects.items_cppinterop)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.fixed_cpu());
			}
			foreach (config_pcl cfg in projects.items_pcl)
			{
				if (cfg.env == "unified_mac") continue;

				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.fixed_cpu());
			}
			foreach (config_plugin cfg in projects.items_plugin)
			{
				if (cfg.env == "unified_mac") continue;

				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, "Any CPU");
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, "Any CPU");
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, "Any CPU");
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, "Any CPU");
			}
			foreach (config_higher cfg in projects.items_higher)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, "Any CPU");
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, "Any CPU");
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, "Any CPU");
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, "Any CPU");
			}
			foreach (config_tests cfg in projects.items_tests)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, "Any CPU");
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, "Any CPU");
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, "Any CPU");
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, "Any CPU");
			}
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
			f.WriteLine("\t\tHideSolutionNode = FALSE");
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("\tGlobalSection(NestedProjects) = preSolution");
			foreach (config_sqlite3 cfg in projects.items_sqlite3)
			{
				f.WriteLine("\t\t{0} = {1}", cfg.guid, folder_sqlite3);
			}
			foreach (config_cppinterop cfg in projects.items_cppinterop)
			{
				f.WriteLine("\t\t{0} = {1}", cfg.guid, folder_cppinterop);
			}
			foreach (config_pcl cfg in projects.items_pcl)
			{
				if (cfg.is_portable())
				{
					f.WriteLine("\t\t{0} = {1}", cfg.guid, folder_portable);
				}
				else
				{
					f.WriteLine("\t\t{0} = {1}", cfg.guid, folder_platforms);
				}
			}
			foreach (config_plugin cfg in projects.items_plugin)
			{
				f.WriteLine("\t\t{0} = {1}", cfg.guid, folder_plugin);
			}
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("EndGlobal");
		}
	}

	private static void write_nuspec_file_entry(config_plugin cfg, XmlWriter f)
	{
		f.WriteComment(string.Format("{0}", cfg.get_name()));
		var a = new List<string>();
		cfg.get_products(a);

		foreach (string s in a)
		{
			f.WriteStartElement("file");
			f.WriteAttributeString("src", string.Format("release\\bin\\{0}", s));
			f.WriteAttributeString("target", cfg.get_nuget_target_path("lib"));
			f.WriteEndElement(); // file
		}
	}

	private static void write_nuspec_file_entry(config_pcl cfg, XmlWriter f, string where)
	{
		f.WriteComment(string.Format("{0}", cfg.get_name()));
		var a = new List<string>();
		cfg.get_products(a);

		foreach (string s in a)
		{
			f.WriteStartElement("file");
			f.WriteAttributeString("src", string.Format("release\\bin\\{0}", s));
			f.WriteAttributeString("target", cfg.get_nuget_target_path(where));
			f.WriteEndElement(); // file
		}
	}

	private static void write_nuspec_file_entry(config_higher cfg, XmlWriter f)
	{
		f.WriteComment(string.Format("{0}", cfg.get_name()));
		var a = new List<string>();
		cfg.get_products(a);

		foreach (string s in a)
		{
			f.WriteStartElement("file");
			f.WriteAttributeString("src", string.Format("release\\bin\\{0}", s));
			f.WriteAttributeString("target", cfg.get_nuget_target_path());
			f.WriteEndElement(); // file
		}
	}

	private static void write_nuspec_file_entry(config_sqlite3 cfg, XmlWriter f)
	{
		f.WriteComment(string.Format("{0}", cfg.get_name()));
		var a = new List<string>();
		cfg.get_products(a);

		foreach (string s in a)
		{
			f.WriteStartElement("file");
			f.WriteAttributeString("src", string.Format("release\\bin\\{0}", s));
			f.WriteAttributeString("target", cfg.get_nuget_target_path());
			f.WriteEndElement(); // file
		}
	}

	private static void write_nuspec_file_entries(XmlWriter f, string where, List<config_pcl> a)
	{
		foreach (config_pcl cfg in a)
		{
			write_nuspec_file_entry(cfg, f, where);
		}
	}

	private static void write_cppinterop_with_targets_file(XmlWriter f, List<config_pcl> a, string env, string top, string id)
	{
		f.WriteComment(string.Format("platform assemblies for {0}", env));

		write_nuspec_file_entries(
				f, 
				"build", 
				a
				);

		f.WriteComment("empty directory in lib to avoid nuget adding a reference to the bait");

		Directory.CreateDirectory(Path.Combine(Path.Combine(top, "empty"), config_cs.get_nuget_framework_name(env)));

		f.WriteStartElement("file");
		f.WriteAttributeString("src", string.Format("empty\\{0}\\", config_cs.get_nuget_framework_name(env)));
		f.WriteAttributeString("target", string.Format("lib\\{0}", config_cs.get_nuget_framework_name(env)));
		f.WriteEndElement(); // file

		f.WriteComment("msbuild .targets file to inject reference for the right cpu");

		string tname = string.Format("{0}.targets", env);
		gen_nuget_targets_cppinterop(top, tname, a);

		f.WriteStartElement("file");
		f.WriteAttributeString("src", tname);
		f.WriteAttributeString("target", string.Format("build\\{0}\\{1}.targets", config_cs.get_nuget_framework_name(env), id));
		f.WriteEndElement(); // file
	}

	private const string NUSPEC_VERSION = "0.9.0-pre8";
	private const string NUSPEC_RELEASE_NOTES = "Major restructuring of the NuGet packages.  Main package (SQLitePCL.raw) no longer has native code embedded in it.  For situations where you do not want to use the default SQLite for your platform, add one of the SQLitePCL.plugin.* packages.  In some cases, upgrading from previous versions will require changes.  See the SQLitePCL.raw page on GitHub for more info.";

	private static void gen_nuspec_basic(string top, string root, string id)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			if (id == "SQLitePCL.raw_basic") {
				f.WriteElementString("title", "SQLitePCL.raw_basic (deprecated)");
			} else {
				f.WriteElementString("title", "SQLitePCL.raw");
			}
			f.WriteElementString("description", "SQLitePCL.raw is a Portable Class Library (PCL) for low-level (raw) access to SQLite.  This package does not provide an API which is friendly to app developers.  Rather, it provides an API which handles platform and configuration issues, upon which a friendlier API can be built.  On platforms (like Android or iOS) where SQLite is preinstalled, this package may be all you need.  On other platforms, or if you want to use a different SQLite build, see the SQLitePCL.plugin.* packages.  (Note that with the 0.8.0 release, the ID of this package changed from 'SQLitePCL.raw_basic' to 'SQLitePCL.raw'.  Eventually, the old ID will stop getting updates.)");
			f.WriteElementString("authors", "Eric Sink, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			// write all the bait

			f.WriteComment("BEGIN bait assemblies");
			// TODO we don't need ALL the bait assemblies in here
			foreach (config_pcl cfg in projects.items_pcl)
			{
				if (cfg.is_portable())
				{
					write_nuspec_file_entry(
							cfg, 
							f, 
							"lib"
							);
				}
			}
			f.WriteComment("END bait assemblies");

			f.WriteComment("BEGIN platform assemblies");

			f.WriteComment("BEGIN platform assemblies that use pinvoke");
			write_nuspec_file_entries(f, "lib",
					projects.find_pcls(
						null,
						"pinvoke",
						null
						)
					);
			f.WriteComment("END platform assemblies that use pinvoke");

			// TODO remove this directory first?
			Directory.CreateDirectory(Path.Combine(top, "empty"));
			
			f.WriteComment("BEGIN platform assemblies that use cppinterop");
			Dictionary<string, string> pcl_env_cppinterop = new Dictionary<string, string>();
			pcl_env_cppinterop["wp80"] = null;
			pcl_env_cppinterop["wp81_sl"] = null;

			foreach (string env in pcl_env_cppinterop.Keys)
			{
				write_cppinterop_with_targets_file(f, 
						projects.find_pcls(
							env,
							"cppinterop",
							null
							),
						env,
						top,
						id
						);
			}
			f.WriteComment("END platform assemblies that use cppinterop");

			f.WriteComment("END platform assemblies");

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_esqlite3(string top, string root, config_esqlite3 cfg)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = cfg.get_id();
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", cfg.get_title());
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCL.raw as well as SQLitePCL.plugin.sqlite3.net45 or similar.");
			f.WriteElementString("authors", "D. Richard Hipp, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_sqlite3 other in cfg.get_sqlite3_items()) 
			{
				write_nuspec_file_entry(
						other, 
						f
						);
			}
			string tname;
			switch (cfg.toolset) {
				case "v110_xp":
					tname = gen_nuget_targets_pinvoke_anycpu(top, cfg.get_id(), cfg.toolset);
					break;
				default:
					tname = gen_nuget_targets_sqlite3_itself(top, cfg.get_id(), cfg.toolset);
					break;
			}

			if (tname != null) {
				f.WriteStartElement("file");
				f.WriteAttributeString("src", tname);
				f.WriteAttributeString("target", string.Format("build\\{0}.targets", id));
				f.WriteEndElement(); // file
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_plugin(string top, string root, config_plugin cfg)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = cfg.get_id();
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", cfg.get_title());
			string desc = "A SQLitePCL.raw plugin can be used to instruct SQLitePCL.raw to reference a different implementation of the native SQLite library than it normally would use.  Install this package in your app project and call SQLite3Plugin.Init().";
			if (cfg.empty)
			{
				desc = desc + "  This particular plugin package contains no native code, so you will need to add one of the SQLitePCL.native.* packages.";
			}
			f.WriteElementString("description", desc);
			f.WriteElementString("authors", "Eric Sink, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			write_nuspec_file_entry(
					cfg, 
					f
					);

			switch (cfg.env) {
				case "ios_classic":
				case "ios_unified":
				case "android":
					break;
				case "net45":
				case "net40":
				case "net35":
				case "win8":
				case "win81":
				case "uap10.0":
				case "wpa81":
					switch (cfg.what) {
					case "sqlite3":
						if (cfg.toolset != null)
						{
							foreach (config_sqlite3 other in cfg.get_sqlite3_items()) 
							{
								write_nuspec_file_entry(
										other, 
										f
										);
							}
						}
						break;
					case "sqlcipher":
						switch (cfg.env) {
						case "net45":
						case "net40":
						case "net35":
							break;
						default:
							throw new Exception();
						}
					break;
					default:
						throw new Exception();
					}

					string tname;
					switch (cfg.env) {
						case "net45":
						case "net40":
						case "net35":
							switch (cfg.what) {
							case "sqlite3":
								if (cfg.toolset != null)
								{
									tname = gen_nuget_targets_pinvoke_anycpu(top, cfg.get_id(), cfg.toolset);
								}
								else
								{
									tname = null;
								}
								break;
							case "sqlcipher":
								tname = null;
								break;
							default:
								throw new Exception();
							}
							break;
						case "win8":
						case "win81":
						case "uap10.0":
						case "wpa81":
							tname = gen_nuget_targets_sqlite3_itself(top, cfg.get_id(), cfg.toolset);
							break;
						default:
							throw new Exception();
					}

					if (tname != null) {
						f.WriteStartElement("file");
						f.WriteAttributeString("src", tname);
						f.WriteAttributeString("target", string.Format("build\\{0}\\{1}.targets", config_cs.get_nuget_framework_name(cfg.env), id));
						f.WriteEndElement(); // file
					}
					break;
				default:
					throw new Exception();
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_sqlcipher(string top, string root, string plat)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = string.Format("SQLitePCL.native.sqlcipher.{0}", plat);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", string.Format("Native code only (sqlcipher, {0}) for SQLitePCL.raw", plat));
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLCipher (see sqlcipher/sqlcipher on GitHub) for use with SQLitePCL.raw.  The build of SQLCipher packaged here is built and maintained by Couchbase (see couchbaselabs/couchbase-lite-libsqlcipher on GitHub).  To use this, you need SQLitePCL.raw as well as SQLitePCL.plugin.sqlcipher.net45 or similar.");
			f.WriteElementString("authors", "Couchbase, SQLite, Zetetic");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			string tname = string.Format("{0}.targets", id);
			switch (plat) {
				case "windows":
					// TODO do we need amd64 version here?

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\windows\\x86\\sqlcipher.dll"));
					f.WriteAttributeString("target", string.Format("build\\native\\{0}\\{1}\\sqlcipher.dll", "windows", "x86"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\windows\\x86_64\\sqlcipher.dll"));
					f.WriteAttributeString("target", string.Format("build\\native\\{0}\\{1}\\sqlcipher.dll", "windows", "x86_64"));
					f.WriteEndElement(); // file

					gen_nuget_targets_sqlcipher_windows(top, tname);
					break;
				case "osx":
					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\osx\\libsqlcipher.dylib"));
					f.WriteAttributeString("target", string.Format("build\\native\\osx\\libsqlcipher.dylib"));
					f.WriteEndElement(); // file

					gen_nuget_targets_sqlcipher_osx(top, tname);
					break;
				case "linux":
					// TODO do we need amd64 version here?

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\linux\\x86_64\\libsqlcipher.so"));
					f.WriteAttributeString("target", string.Format("build\\native\\{0}\\{1}\\libsqlcipher.so", "linux", "x86_64"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\linux\\x86\\libsqlcipher.so"));
					f.WriteAttributeString("target", string.Format("build\\native\\{0}\\{1}\\libsqlcipher.so", "linux", "x86"));
					f.WriteEndElement(); // file

					gen_nuget_targets_sqlcipher_linux(top, tname);
					break;
				default:
					throw new Exception();
			}
			f.WriteStartElement("file");
			f.WriteAttributeString("src", tname);
			f.WriteAttributeString("target", string.Format("build\\{0}.targets", id));
			f.WriteEndElement(); // file

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_ugly(string top)
	{
		string id = "SQLitePCL.ugly";

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", "SQLitePCL.ugly");
			f.WriteElementString("description", "These extension methods for SQLitePCL.raw provide a more usable API while remaining stylistically similar to the sqlite3 C API, which most C# developers would consider 'ugly'.  This package exists for people who (1) really like the sqlite3 C API, and (2) really like C#.  So far, evidence suggests that 100% of the people matching both criteria are named Eric Sink, but this package is available just in case he is not the only one of his kind.");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Extension methods for SQLitePCL.raw, providing an ugly-but-usable API");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");
			f.WriteStartElement("dependency");
			f.WriteAttributeString("id", "SQLitePCL.raw");
			f.WriteAttributeString("version", NUSPEC_VERSION);
			f.WriteEndElement(); // dependency
			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_higher cfg in projects.items_higher)
			{
				if (cfg.name == "ugly")
				{
					write_nuspec_file_entry(
							cfg, 
							f
							);
				}
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_tests(string top, string root)
	{
		string id = "SQLitePCL.tests";

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for the unified stuff

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", "Test cases for SQLitePCL.raw");
			f.WriteElementString("description", "Create a new unit test project.  Add this NuGetPackage.  Build.");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "test_cases.cs is a bunch unit tests for SQLitePCL.raw");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");
			f.WriteStartElement("dependency");
			f.WriteAttributeString("id", "SQLitePCL.ugly");
			f.WriteAttributeString("version", NUSPEC_VERSION);
			f.WriteEndElement(); // dependency
			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			f.WriteStartElement("file");
			f.WriteAttributeString("src", Path.Combine(root, "src\\cs\\test_cases.cs"));
			f.WriteAttributeString("target", "content");
			f.WriteEndElement(); // file

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

			foreach (config_sqlite3 other in projects.items_sqlite3)
			{
				if (other.toolset != toolset)
				{
					continue;
				}

				f.WriteStartElement("ItemGroup");
				f.WriteAttributeString("Condition", string.Format(" '$(Platform.ToLower())' == '{0}' ", other.cpu.ToLower()));

				f.WriteStartElement("Content");
				// TODO call other.get_products() instead of hard-coding the sqlite3.dll name here
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\{0}", Path.Combine(other.get_nuget_target_path(), "esqlite3.dll")));
				// TODO link
				// TODO condition/exists ?
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteEndElement(); // Content

				f.WriteEndElement(); // ItemGroup
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
			// TODO problem with net45 on Linux because of the 'e'
			f.WriteAttributeString("Condition", " '$(OS)' == 'Windows_NT' ");
			foreach (config_sqlite3 other in projects.items_sqlite3)
			{
				if (toolset != other.toolset)
				{
					continue;
				}

				// TODO for net45, handle more than just windows.  deal with mono on mac and linux

				f.WriteStartElement("Content");
				// TODO call other.get_products() instead of hard-coding the sqlite3.dll name here
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\{0}", Path.Combine(other.get_nuget_target_path(), "esqlite3.dll")));
				// TODO condition/exists ?
				f.WriteElementString("Link", string.Format("{0}\\esqlite3.dll", other.cpu.ToLower()));
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
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

	private static void gen_nuget_targets_sqlcipher_windows(string top, string tname)
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
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\build\\native\\{0}\\x86\\sqlcipher.dll", "windows"));
			f.WriteElementString("Link", string.Format("{0}\\sqlcipher.dll", "x86"));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
			f.WriteEndElement(); // Content

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\build\\native\\{0}\\x86_64\\sqlcipher.dll", "windows"));
			f.WriteElementString("Link", string.Format("{0}\\sqlcipher.dll", "x64"));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
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

	private static void gen_nuget_targets_sqlcipher_osx(string top, string tname)
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

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\build\\native\\osx\\libsqlcipher.dylib"));
			f.WriteElementString("Link", "libsqlcipher.dylib");
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
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

	private static void gen_nuget_targets_sqlcipher_linux(string top, string tname)
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

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\build\\native\\linux\\x86_64\\libsqlcipher.so"));
			f.WriteElementString("Link", "libsqlcipher.so");
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
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

	private static void gen_nuget_targets_cppinterop(string top, string tname, List<config_pcl> a)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		Dictionary<string,string> cpus = new Dictionary<string,string>();
		foreach (config_pcl cfg in a)
		{
			cpus[cfg.cpu.ToLower()] = null;
		}

		string cond = "";
		string ok = "";
		foreach (string cpu in cpus.Keys)
		{
			if (cond.Length > 0)
			{
				cond += " AND ";
				ok += " or ";
			}

			cond += string.Format("($(Platform.ToLower()) != '{0}')", cpu);
			ok += cpu;
		}

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("check_cpu_{0}", Guid.NewGuid().ToString()));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");
			f.WriteAttributeString("Condition", string.Format(" ( {0} ) ", cond));
			f.WriteStartElement("Warning");
			f.WriteAttributeString("Text", string.Format("$(Platform) is not supported. The Platform configuration must be {0}", ok));
			f.WriteEndElement(); // Warning
			f.WriteEndElement(); // Target

			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", Guid.NewGuid().ToString()));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			foreach (config_pcl cfg in a)
			{
				switch (cfg.env)
				{
					case "wp81_sl":
						// TODO SDKReference
						f.WriteStartElement("Message");
						f.WriteAttributeString("Text", "NOTE that you may need to add a reference to Microsoft Visual C++ Runtime.");
						f.WriteAttributeString("Importance", "High");
						f.WriteEndElement(); // Message
						break;
				}
				
				bool b_platform_condition = true;

				switch (cfg.env)
				{
					// TODO unified?
					case "ios_classic":
						b_platform_condition = false;
						break;
					case "android":
						b_platform_condition = false;
						break;

					default:
						break;
				}

				f.WriteComment(string.Format("{0}", cfg.get_name()));
				f.WriteStartElement("ItemGroup");
				if (b_platform_condition)
				{
					f.WriteAttributeString("Condition", string.Format(" '$(Platform.ToLower())' == '{0}' ", cfg.cpu.ToLower()));
				}

				f.WriteStartElement("Reference");
				// TODO should Include be the HintPath?
				// https://github.com/onovotny/WinRTTimeZones/blob/master/NuGet/WinRTTimeZones.WP8.targets
				f.WriteAttributeString("Include", "SQLitePCL.raw");

				f.WriteElementString("HintPath", string.Format("$(MSBuildThisFileDirectory){0}", Path.Combine(cfg.get_nuget_target_subpath(), "SQLitePCL.raw.dll")));

				// TODO private?

				// TODO name?

				f.WriteEndElement(); // Reference

				{
					config_sqlite3 other = cfg.get_sqlite3_item();
					if (other != null)
					{
						f.WriteStartElement("Content");
						// TODO call other.get_products() instead of hard-coding the sqlite3.dll name here
						f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\{0}", Path.Combine(other.get_nuget_target_path(), "sqlite3.dll")));
						// TODO link
						// TODO condition/exists ?
						f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
						f.WriteEndElement(); // Content
					}
				}

				f.WriteEndElement(); // ItemGroup
			}

			f.WriteEndElement(); // Target

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	public static void Main(string[] args)
	{
		projects.init();

		string root = Directory.GetCurrentDirectory(); // assumes that gen_build.exe is being run from the root directory of the project
		string top = Path.Combine(root, "bld");

		// --------------------------------
		// create the build directory

		Directory.CreateDirectory(top);

		string cs_pinvoke = File.ReadAllText(Path.Combine(root, "src/cs/sqlite3_pinvoke.cs"));
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_default.cs")))
		{
			string cs1 = cs_pinvoke.Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "default");
			string cs2 = cs1.Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "sqlite3");
			tw.Write(cs2);
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_esqlite3.cs")))
		{
			string cs1 = cs_pinvoke.Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "esqlite3");
			string cs2 = cs1.Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "esqlite3");
			tw.Write(cs2);
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_sqlcipher.cs")))
		{
			string cs1 = cs_pinvoke.Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "sqlcipher");
			string cs2 = cs1.Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "sqlcipher");
			tw.Write(cs2);
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_ios_internal.cs")))
		{
			string cs1 = cs_pinvoke.Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "internal");
			string cs2 = cs1.Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "__Internal");
			tw.Write(cs2);
		}

		// --------------------------------
		// assign all the guids

		foreach (config_sqlite3 cfg in projects.items_sqlite3)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		foreach (config_cppinterop cfg in projects.items_cppinterop)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		foreach (config_pcl cfg in projects.items_pcl)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		foreach (config_plugin cfg in projects.items_plugin)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		foreach (config_higher cfg in projects.items_higher)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		foreach (config_tests cfg in projects.items_tests)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		// --------------------------------
		// generate all the project files

		foreach (config_sqlite3 cfg in projects.items_sqlite3)
		{
			gen_sqlite3(cfg, root, top);
		}

		foreach (config_cppinterop cfg in projects.items_cppinterop)
		{
			gen_cppinterop(cfg, root, top);
		}

		foreach (config_pcl cfg in projects.items_pcl)
		{
			gen_pcl(cfg, root, top);
		}

		foreach (config_plugin cfg in projects.items_plugin)
		{
			gen_plugin(cfg, root, top);
		}

		foreach (config_higher cfg in projects.items_higher)
		{
			gen_higher(cfg, root, top);
		}

		foreach (config_tests cfg in projects.items_tests)
		{
			gen_tests(cfg, root, top);
		}

		// --------------------------------

		gen_solution(top);

		// --------------------------------

		gen_nuspec_basic(top, root, "SQLitePCL.raw_basic");
		gen_nuspec_basic(top, root, "SQLitePCL.raw");

		gen_nuspec_ugly(top);

		foreach (config_plugin cfg in projects.items_plugin)
		{
			gen_nuspec_plugin(top, root, cfg);
		}

		foreach (config_esqlite3 cfg in projects.items_esqlite3)
		{
			gen_nuspec_esqlite3(top, root, cfg);
		}

		gen_nuspec_sqlcipher(top, root, "windows");
		gen_nuspec_sqlcipher(top, root, "osx");
		gen_nuspec_sqlcipher(top, root, "linux");

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "build.ps1")))
		{
			tw.WriteLine("../../nuget restore sqlitepcl.sln");
			tw.WriteLine("msbuild /p:Configuration=Release sqlitepcl.sln");
			tw.WriteLine("../../refgen UAP,Version=v10.0 uap10.0 ./SQLitePCL.raw.nuspec ./platform.uap10.0.pinvoke.anycpu/platform.uap10.0.pinvoke.anycpu.csproj ./release/bin/pcl/uap10.0/pinvoke/anycpu/SQLitePCL.raw.dll");
			tw.WriteLine("../../refgen UAP,Version=v10.0 uap10.0 ./SQLitePCL.raw_basic.nuspec ./platform.uap10.0.pinvoke.anycpu/platform.uap10.0.pinvoke.anycpu.csproj ./release/bin/pcl/uap10.0/pinvoke/anycpu/SQLitePCL.raw.dll");
			foreach (config_plugin cfg in projects.items_plugin)
			{
				if (config_cs.env_needs_project_dot_json(cfg.env))
				{
					string id = cfg.get_id();
					var a = new List<string>();
					cfg.get_products(a);
					// TODO assert a.count is 1
					tw.WriteLine("../../refgen UAP,Version=v10.0 {0} ./{1}.nuspec {2} ./release/bin/{3}", cfg.env, cfg.get_id(), cfg.get_project_path(top), a[0]);
				}
			}
		}

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pack.ps1")))
		{
			tw.WriteLine("echo \"Run apple/libs/mac/cp_mac.ps1\"");
			tw.WriteLine("# TODO");
			tw.WriteLine("../../nuget pack SQLitePCL.raw.nuspec");
			tw.WriteLine("../../nuget pack SQLitePCL.raw_basic.nuspec");
			tw.WriteLine("../../nuget pack SQLitePCL.ugly.nuspec");
			foreach (config_plugin cfg in projects.items_plugin)
			{
				string id = cfg.get_id();
				tw.WriteLine("../../nuget pack {0}.nuspec", id);
			}
			foreach (config_esqlite3 cfg in projects.items_esqlite3)
			{
				string id = cfg.get_id();
				tw.WriteLine("../../nuget pack {0}.nuspec", id);
			}
			tw.WriteLine("../../nuget pack SQLitePCL.native.sqlcipher.windows.nuspec");
			tw.WriteLine("../../nuget pack SQLitePCL.native.sqlcipher.osx.nuspec");
			tw.WriteLine("../../nuget pack SQLitePCL.native.sqlcipher.linux.nuspec");
			tw.WriteLine("ls *.nupkg");
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "push.ps1")))
		{
			tw.WriteLine("# TODO");
			tw.WriteLine("ls *.nupkg");
			tw.WriteLine("../../nuget push SQLitePCL.raw.{0}.nupkg", NUSPEC_VERSION);
			tw.WriteLine("../../nuget push SQLitePCL.raw_basic.{0}.nupkg", NUSPEC_VERSION);
			tw.WriteLine("../../nuget push SQLitePCL.ugly.{0}.nupkg", NUSPEC_VERSION);
			foreach (config_plugin cfg in projects.items_plugin)
			{
				string id = cfg.get_id();
				tw.WriteLine("../../nuget push {0}.{1}.nupkg", id, NUSPEC_VERSION);
			}
			foreach (config_esqlite3 cfg in projects.items_esqlite3)
			{
				string id = cfg.get_id();
				tw.WriteLine("#../../nuget push {0}.{1}.nupkg", id, NUSPEC_VERSION);
			}
			tw.WriteLine("../../nuget push SQLitePCL.native.sqlcipher.windows.{0}.nupkg", NUSPEC_VERSION);
			tw.WriteLine("../../nuget push SQLitePCL.native.sqlcipher.osx.{0}.nupkg", NUSPEC_VERSION);
			tw.WriteLine("../../nuget push SQLitePCL.native.sqlcipher.linux.{0}.nupkg", NUSPEC_VERSION);
		}
	}
}

