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
using System.Linq;
using System.Xml;

public static class projects
{
	// Each item in these Lists corresponds to a project file that will be
	// generated.
	//
	public static List<config_sqlite3> items_sqlite3 = new List<config_sqlite3>();
	public static List<config_cppinterop> items_cppinterop = new List<config_cppinterop>();
	public static List<config_csproj> items_csproj = new List<config_csproj>();

	public static List<config_csproj> items_test = new List<config_csproj>();
	public static List<config_testapp> items_testapp = new List<config_testapp>();

	public static List<config_esqlite3> items_esqlite3 = new List<config_esqlite3>();

	// This function is called by Main to initialize the project lists.
	//
	public static void init()
	{
		init_sqlite3();

		init_cppinterop();

        init_csproj();

		init_tests();

		init_testapps();

		init_esqlite3();
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
	}

    private static void init_bundles(int ver)
    {
        // bundle_winsqlite3
        items_csproj.Add(config_csproj.create_batteries("batteries_winsqlite3", ver, "uwp10", "winsqlite3"));
        
        // bundle_green
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"ios_unified", "sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"macos", "sqlite3"));
        // TODO items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"watchos", "sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"android", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"win8", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"wpa81", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"win81", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"uwp10", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"net35", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"net40", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"net45", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_wp80_batteries("batteries_green", ver, "arm"));
        items_csproj.Add(config_csproj.create_wp80_batteries("batteries_green", ver, "x86"));

        // the following item builds for netstandard11 
        // but overrides the nuget target env to place it in netcoreapp
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"netstandard11", "e_sqlite3", "netcoreapp"));

        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"profile111", null));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"profile136", null));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"profile259", null));
        items_csproj.Add(config_csproj.create_batteries("batteries_green",  ver,"netstandard11", null));

        // bundle_e_sqlite3
        items_csproj.Add(config_csproj.create_internal_batteries("batteries_e_sqlite3", ver, "ios_unified", "e_sqlite3"));
        // TODO items_csproj.Add(config_csproj.create_internal_batteries("batteries_e_sqlite3", "watchos", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "android", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "macos", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "win8", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "wpa81", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "win81", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "uwp10", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "net35", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "net40", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "net45", "e_sqlite3"));
        items_csproj.Add(config_csproj.create_wp80_batteries("batteries_e_sqlite3", ver, "arm"));
        items_csproj.Add(config_csproj.create_wp80_batteries("batteries_e_sqlite3", ver, "x86"));

        // the following item builds for netstandard11 
        // but overrides the nuget target env to place it in netcoreapp
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "netstandard11", "e_sqlite3", "netcoreapp"));

        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "profile111", null));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "profile136", null));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "profile259", null));
        items_csproj.Add(config_csproj.create_batteries("batteries_e_sqlite3", ver, "netstandard11", null));

        // bundle_sqlcipher
        items_csproj.Add(config_csproj.create_internal_batteries("batteries_sqlcipher", ver, "ios_unified", "sqlcipher"));
        // TODO items_csproj.Add(config_csproj.create_internal_batteries("batteries_sqlcipher", "watchos", "sqlcipher"));
        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "macos", "sqlcipher"));
        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "android", "sqlcipher"));
        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "net35", "sqlcipher"));
        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "net40", "sqlcipher"));
        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "net45", "sqlcipher"));

        // the following item builds for netstandard11 
        // but overrides the nuget target env to place it in netcoreapp
        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "netstandard11", "sqlcipher", "netcoreapp"));

        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "profile111", null));
        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "profile136", null));
        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "profile259", null));
        items_csproj.Add(config_csproj.create_batteries("batteries_sqlcipher", ver, "netstandard11", null));

    }

    private static void init_csproj()
    {
        items_csproj.Add(config_csproj.create_core("net35"));
        items_csproj.Add(config_csproj.create_core("net40"));
        items_csproj.Add(config_csproj.create_core("net45"));
        items_csproj.Add(config_csproj.create_core("ios_unified"));
        items_csproj.Add(config_csproj.create_core("macos"));
        // TODO items_csproj.Add(config_csproj.create_core("watchos"));
        items_csproj.Add(config_csproj.create_core("android"));
        items_csproj.Add(config_csproj.create_core("win8"));
        items_csproj.Add(config_csproj.create_core("win81"));
        items_csproj.Add(config_csproj.create_core("wpa81"));
        items_csproj.Add(config_csproj.create_core("uwp10"));
        items_csproj.Add(config_csproj.create_core("profile111"));
        items_csproj.Add(config_csproj.create_core("profile136"));
        items_csproj.Add(config_csproj.create_core("profile259"));
        items_csproj.Add(config_csproj.create_core("netstandard10"));
        items_csproj.Add(config_csproj.create_core("netstandard11"));

        items_csproj.Add(config_csproj.create_provider("sqlite3_xamarin", "android"));

        items_csproj.Add(config_csproj.create_provider("winsqlite3", "uwp10"));
        items_csproj.Add(config_csproj.create_provider("winsqlite3", "net45"));

        items_csproj.Add(config_csproj.create_provider("sqlite3", "netstandard11"));
        items_csproj.Add(config_csproj.create_provider("sqlite3", "net35"));
        items_csproj.Add(config_csproj.create_provider("sqlite3", "net40"));
        items_csproj.Add(config_csproj.create_provider("sqlite3", "net45"));
        items_csproj.Add(config_csproj.create_provider("sqlite3", "ios_unified"));
        items_csproj.Add(config_csproj.create_provider("sqlite3", "macos"));
        // TODO items_csproj.Add(config_csproj.create_provider("sqlite3", "watchos"));
        items_csproj.Add(config_csproj.create_provider("sqlite3", "android")); // bad idea
        items_csproj.Add(config_csproj.create_provider("sqlite3", "win8"));
        items_csproj.Add(config_csproj.create_provider("sqlite3", "win81"));
        items_csproj.Add(config_csproj.create_provider("sqlite3", "wpa81"));
        items_csproj.Add(config_csproj.create_provider("sqlite3", "uwp10"));

        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "netstandard11"));
        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "net35"));
        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "net40"));
        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "net45"));
        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "android"));
        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "win8"));
        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "win81"));
        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "wpa81"));
        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "uwp10"));
        items_csproj.Add(config_csproj.create_provider("e_sqlite3", "macos"));
        // ios would only make sense here with dylibs
        //items_csproj.Add(config_csproj.create_provider("e_sqlite3", "ios_unified"));

        items_csproj.Add(config_csproj.create_provider("internal", "ios_unified"));
        // TODO items_csproj.Add(config_csproj.create_provider("internal", "watchos"));

        items_csproj.Add(config_csproj.create_embedded("e_sqlite3", "android"));
        items_csproj.Add(config_csproj.create_embedded("e_sqlite3", "ios_unified"));
        // TODO items_csproj.Add(config_csproj.create_embedded("e_sqlite3", "watchos"));

        items_csproj.Add(config_csproj.create_embedded("sqlcipher", "android"));
        items_csproj.Add(config_csproj.create_embedded("sqlcipher", "ios_unified"));

        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "netstandard11"));
        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "net35"));
        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "net40"));
        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "net45"));
        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "android"));
        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "win8"));
        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "win81"));
        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "wpa81"));
        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "uwp10"));
        items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "macos"));
        // ios would only make sense here with dylibs
        //items_csproj.Add(config_csproj.create_provider("custom_sqlite3", "ios_unified"));

        items_csproj.Add(config_csproj.create_provider("sqlcipher", "netstandard11"));
        items_csproj.Add(config_csproj.create_provider("sqlcipher", "net35"));
        items_csproj.Add(config_csproj.create_provider("sqlcipher", "net40"));
        items_csproj.Add(config_csproj.create_provider("sqlcipher", "net45"));
        items_csproj.Add(config_csproj.create_provider("sqlcipher", "android"));
        items_csproj.Add(config_csproj.create_provider("sqlcipher", "win8"));
        items_csproj.Add(config_csproj.create_provider("sqlcipher", "win81"));
        items_csproj.Add(config_csproj.create_provider("sqlcipher", "wpa81"));
        items_csproj.Add(config_csproj.create_provider("sqlcipher", "uwp10"));
        items_csproj.Add(config_csproj.create_provider("sqlcipher", "macos"));
        // ios would only make sense here with dylibs
        //items_csproj.Add(config_csproj.create_provider("sqlcipher", "ios_unified"));

        items_csproj.Add(config_csproj.create_wp80_provider("arm"));
        items_csproj.Add(config_csproj.create_wp80_provider("x86"));

        items_csproj.Add(config_csproj.create_ugly("net35"));
        items_csproj.Add(config_csproj.create_ugly("net40"));
        items_csproj.Add(config_csproj.create_ugly("net45"));
        items_csproj.Add(config_csproj.create_ugly("android"));
        items_csproj.Add(config_csproj.create_ugly("ios_unified"));
        items_csproj.Add(config_csproj.create_ugly("macos"));
        // TODO items_csproj.Add(config_csproj.create_ugly("watchos"));
        items_csproj.Add(config_csproj.create_ugly("win8"));
        items_csproj.Add(config_csproj.create_ugly("win81"));
        items_csproj.Add(config_csproj.create_ugly("wpa81"));
        items_csproj.Add(config_csproj.create_ugly("uwp10"));
        items_csproj.Add(config_csproj.create_ugly("profile111"));
        items_csproj.Add(config_csproj.create_ugly("profile136"));
        items_csproj.Add(config_csproj.create_ugly("profile259"));
        //items_csproj.Add(config_csproj.create_ugly("netstandard10"));
        items_csproj.Add(config_csproj.create_ugly("netstandard11"));

        init_bundles(1);
        init_bundles(2);
    }

	private static void init_tests()
	{
        // using netstandard for the tests would require switching to the
        // xunit pre
        
        //items_test.Add(config_csproj.create_portable_test("netstandard11"));
        items_test.Add(config_csproj.create_portable_test("profile259"));

        items_test.Add(config_csproj.create_bundle_test("net45", "e_sqlite3"));
        items_test.Add(config_csproj.create_bundle_test("net45", "green"));

        // in main
        //items_csproj.Add(config_csproj.create_portable_test_main("netstandard11"));
        //xunit only supports 259
        //items_csproj.Add(config_csproj.create_portable_test_main("profile111"));
        //items_csproj.Add(config_csproj.create_portable_test_main("profile136"));
        items_csproj.Add(config_csproj.create_portable_test_main("profile259"));
	}

    private static void init_testapps()
    {
        items_testapp.Add(new config_testapp { 
                env="android", 
                cpu="anycpu", 
                bundle="bundle_sqlcipher",
                });
        items_testapp.Add(new config_testapp { 
                env="android", 
                cpu="anycpu", 
                bundle="bundle_e_sqlite3",
                });
        items_testapp.Add(new config_testapp { 
                env="android", 
                cpu="anycpu", 
                bundle="bundle_green",
                });

        items_testapp.Add(new config_testapp { 
                env="wp81", 
                cpu="x86", 
                bundle="bundle_e_sqlite3",
                });
        items_testapp.Add(new config_testapp { 
                env="wp81", 
                cpu="x86", 
                bundle="bundle_green",
                });

        items_testapp.Add(new config_testapp { 
                env="uwp10", 
                cpu="x86", 
                bundle="bundle_green",
                });
        items_testapp.Add(new config_testapp { 
                env="uwp10", 
                cpu="x86", 
                bundle="bundle_e_sqlite3",
                });
        items_testapp.Add(new config_testapp { 
                env="uwp10", 
                cpu="x64", 
                bundle="bundle_winsqlite3",
                });
    }

	private static void init_esqlite3()
	{
		items_esqlite3.Add(new config_esqlite3 { toolset="v110_xp" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v110" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v110_wp80" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v120" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v120_wp81" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v140" });
	}

	public static string get_nuget_target_path(string env)
	{
		if (config_cs.env_is_portable(env))
		{
			return string.Format("lib\\{0}\\", projects.get_portable_nuget_target_string(env));
		}
		else if (env == "wp80")
		{
            // this goes into build/wp80/cpu, but the cpu isn't
            // a param to this function, so the wp80 case has to
            // be handled another way
            throw new NotImplementedException();
		}
		else
		{
			return string.Format("lib\\{0}\\", config_cs.get_nuget_framework_name(env));
		}
	}

    public static string rid_front_half(string toolset)
    {
        switch (toolset)
        {
            case "v110_xp":
                return "win7";
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
		case "uwp10":
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

	public static config_csproj find(string area, string env)
    {
		foreach (config_csproj cfg in projects.items_csproj)
		{
			if (cfg.area == area && cfg.env == env)
			{
				return cfg;
			}
		}
        return null;
    }

	public static config_csproj find(string name)
    {
		foreach (config_csproj cfg in projects.items_csproj)
		{
			if (cfg.name == name)
			{
				return cfg;
			}
		}
        return null;
    }

	public static config_csproj find(string area, string what, string env, string cpu)
    {
		foreach (config_csproj cfg in projects.items_csproj)
		{
			if (cfg.area == area && cfg.what == what && cfg.env == env && cfg.cpu == cpu)
			{
				return cfg;
			}
		}
        return null;
    }

	public static config_csproj find_core(string env)
	{
        config_csproj cfg = find("core", env);
        if (cfg == null)
        {
            // TODO need to find a core that is compatible with env
            // TODO this should be smarter
            switch (env)
            {
                case "net40":
                    cfg = find("core", "profile136");
                    break;
                case "net45":
                    cfg = find("core", "profile111");
                    break;
                case "win81":
                    cfg = find("core", "profile111");
                    break;
                default:
                    cfg = find("core", "netstandard11");
                    //cfg = find("core", "profile259");
                    break;
            }
        }
        if (cfg != null)
        {
            return cfg;
        }
		throw new Exception(string.Format("core not found for {0}", env));
	}

    public static config_csproj find_provider(string what, string env, string cpu)
    {
        config_csproj cfg = find("provider", what, env, cpu);
        if (cfg == null)
        {
            switch (env)
            {
                default:
                    cfg = find("provider", what, "netstandard11", "anycpu");
                    //cfg = find("provider", what, "profile259", "anycpu");
                    break;
            }
        }
        if (cfg != null)
        {
            return cfg;
        }
		throw new Exception(string.Format("provider not found for {0}/{1}", what, env));
    }

    public static config_csproj find_name(string name)
    {
        config_csproj cfg = find(name);
        if (cfg != null)
        {
            return cfg;
        }
		throw new Exception(string.Format("csproj not found {0}", name));
    }

	public static config_csproj find_ugly(string env)
	{
        config_csproj cfg = find("ugly", env);
        if (cfg == null)
        {
            // TODO need to find one that is compatible with env
            // TODO this should be smarter
            cfg = find("ugly", "netstandard11");
            //cfg = find("ugly", "profile259");
        }
        if (cfg != null)
        {
            return cfg;
        }
		throw new Exception(string.Format("ugly not found for {0}", env));
	}

}

public interface config_info
{
	string get_project_filename();
	string get_name();
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
		a.Add(Path.Combine(get_name(), "bin", "release", s));
	}

	public void get_products(List<string> a)
	{
		add_product(a, "e_sqlite3.dll");
	}

	private string area()
	{
		return "sqlite3";
	}

    string rid()
    {
        return string.Format("{0}-{1}", projects.rid_front_half(toolset), cpu);
    }

	public string get_nuget_target_path()
	{
		return string.Format("runtimes\\{0}\\native\\", rid());
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
		a.Add(Path.Combine(get_name(), "bin", "release", s));
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
		return string.Format("{0}.{1}.{2}.{3}", gen.ROOT_NAME, area(), env, cpu);
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
		a.Add(Path.Combine(get_name(), "bin", "release", s));
	}

	public string get_name()
	{
        // TODO could include the word dynamic here
		return string.Format("lib.e_sqlite3.{0}", toolset);
	}

	public string get_title()
	{
		return string.Format("Native code only (e_sqlite3, compiled with {0}) for SQLitePCLRaw", toolset);
	}

	public string get_id()
	{
		return string.Format("{0}.{1}", gen.ROOT_NAME, get_name());
	}

	public string get_project_filename()
	{
		throw new NotImplementedException();
	}
	
}

public static class config_cs
{
	public static bool env_is_portable(string env)
	{
		return env.StartsWith("profile");
	}

	public static bool env_is_netstandard(string env)
	{
		return env.StartsWith("netstandard");
	}

	public static string get_full_framework_name(string env)
	{
        // what this function really does is return whatever
        // framework name is full enough for project.json/frameworks.
		switch (env)
		{
			case "ios_classic":
				return "MonoTouch,Version=v1.0";
			case "ios_unified":
				return "Xamarin.iOS10";
			case "macos":
				return "Xamarin.Mac20";
			case "watchos":
				return "Xamarin.WatchOS";
			case "android":
				return "MonoAndroid,Version=v2.3";
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
			case "uwp10":
				return "uap10.0";
			case "win8":
				return ".NETCore,Version=4.5.1";
			case "win81":
				return ".NETCore,Version=4.5.1";
            case "profile111":
                return ".NETPortable,Version=v4.5,Profile=profile111";
            case "profile136":
                return ".NETPortable,Version=v4.0,Profile=profile136";
            case "profile259":
                return ".NETPortable,Version=v4.5,Profile=profile259";
            case "netstandard10":
                return "netstandard1.0";
            case "netstandard11":
                return "netstandard1.1";
			default:
				throw new Exception(env);
		}
	}
					
	public static string get_nuget_framework_name(string env)
	{
		switch (env)
		{
			case "ios_classic":
				return "MonoTouch";
			case "ios_unified":
				return "Xamarin.iOS10";
			case "macos":
				return "Xamarin.Mac20";
			case "watchos":
				return "Xamarin.WatchOS";
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
			case "uwp10":
				return "uap10.0";
			case "win8":
				return "win8";
			case "win81":
				return "win81";
			case "netstandard11":
				return "netstandard1.1";
			case "netstandard10":
				return "netstandard1.0";
            case "profile111":
            case "profile136":
            case "profile259":
                return projects.get_portable_nuget_target_string(env);
            case "netcoreapp":
                return "netcoreapp";
			default:
				throw new Exception(env);
		}
	}
					
}

public class config_testapp
{
	public string guid;
    public string env;
    public string cpu;
    public string bundle;

    public string pattern
    {
        get
        {
            switch (env)
            {
                case "wp81": return "Tests.WP81";
                case "android": return "Tests.Android";
                case "uwp10": return "Tests.UWP10";
                default: throw new Exception();
            }
        }
    }

    public string name
    {
        get
        {
            return string.Format("tests_{0}_{1}_{2}", bundle, env, cpu);
        }
    }
}

public class config_csproj : config_info
{
    public string area;
    public string what; // TODO call this provider_name ?
    public string name;
	public string guid;
	public string assemblyname;
	public string env;
    public string nuget_override_target_env;
    public bool CopyNuGetImplementations;
	public string cpu = "anycpu";
	public List<string> csfiles_src = new List<string>();
	public List<string> csfiles_bld = new List<string>();
	public List<string> defines = new List<string>();
	public List<string> runtimes = new List<string>();
	public Dictionary<string,string> deps = new Dictionary<string,string>();
    public bool ref_core;
    public bool ref_ugly;
    public string ref_provider;
    public string ref_embedded;
    public bool ref_cppinterop = false;

    string root_name
    {
        get
        {
            return gen.ROOT_NAME;
        }
    }

    public static config_csproj create_core(string env)
    {
        var cfg = new config_csproj();
        cfg.area = "core";
        cfg.name = string.Format("{0}.core.{1}", cfg.root_name, env);
        cfg.assemblyname = string.Format("{0}.core", cfg.root_name);
        cfg.env = env;

        cfg.csfiles_src.Add("raw.cs");
        cfg.csfiles_src.Add("intptrs.cs");
        cfg.csfiles_src.Add("isqlite3.cs");
        cfg.csfiles_src.Add("sqlite3_bait.cs");

        return cfg;
    }

    public static config_csproj create_embedded(string what, string env)
    {
        var cfg = new config_csproj();
        cfg.area = "lib";
        switch (env)
        {
            case "ios_unified":
            case "ios_classic":
			case "watchos":
                cfg.name = string.Format("{0}.lib.{1}.{2}.{3}", cfg.root_name, what, env, "static");
                break;
            default:
                cfg.name = string.Format("{0}.lib.{1}.{2}", cfg.root_name, what, env);
                break;
        }
        cfg.what = what;
        cfg.assemblyname = string.Format("{0}.lib.{1}", cfg.root_name, what);
        cfg.env = env;
        switch (cfg.env)
        {
            case "ios_classic":
            case "ios_unified":
            case "watchos":
                switch (what)
                {
                    case "custom_sqlite3":
                        cfg.defines.Add("IOS_PACKAGED_CUSTOM_SQLITE3");
                        break;
                    case "e_sqlite3":
                        cfg.defines.Add("IOS_PACKAGED_E_SQLITE3");
                        break;
                    case "sqlcipher":
                        cfg.defines.Add("IOS_PACKAGED_SQLCIPHER");
                        break;
                    default:
                        throw new Exception(what);
                }
                break;
        }
        cfg.csfiles_src.Add("embedded_init.cs");
        switch (cfg.env)
        {
            case "ios_unified":
            case "ios_classic":
            case "watchos":
                switch (what)
                {
                    case "custom_sqlite3":
                    case "e_sqlite3":
                    case "sqlcipher":
                        cfg.csfiles_src.Add("imp_ios_internal.cs");
                        break;
                    default:
                        throw new Exception(what);
                }
                break;
            default:
                break;
        }

        return cfg;
    }

    // TODO 'what' should be the name used in DllImport
    public static config_csproj create_provider(string what, string env)
    {
        var cfg = new config_csproj();
        cfg.area = "provider";
        cfg.what = what;
        cfg.name = string.Format("{0}.provider.{1}.{2}", cfg.root_name, what, env);
        cfg.assemblyname = string.Format("{0}.provider.{1}", cfg.root_name, what);
        cfg.env = env;
        cfg.csfiles_src.Add("util.cs");
        cfg.ref_core = true;
        switch (cfg.env)
        {
            case "net35":
            case "net40":
            case "net45":
                cfg.defines.Add("PRELOAD_FROM_ARCH_DIRECTORY");
                break;
        }
        switch (cfg.env)
        {
            case "ios_unified":
            case "ios_classic":
            case "watchos":
                switch (what)
                {
                    case "sqlite3":
                        cfg.csfiles_bld.Add("pinvoke_sqlite3.cs");
                        break;
                    case "internal":
                        cfg.csfiles_bld.Add("pinvoke_ios_internal.cs");
                        break;
                    default:
                        // TODO e_sqlite3 and custom_sqlite3 could be supported here,
                        // but without dylibs, they don't make much sense.
                        throw new Exception(what);
                }
                break;
            default:
                switch (what)
                {
                    case "sqlite3":
                        cfg.csfiles_bld.Add("pinvoke_sqlite3.cs");
                        break;
                    case "winsqlite3":
                        cfg.csfiles_bld.Add("pinvoke_winsqlite3.cs");
                        break;
                    case "sqlite3_xamarin":
                        cfg.csfiles_bld.Add("pinvoke_sqlite3_xamarin.cs");
                        break;
                    case "custom_sqlite3":
                        cfg.csfiles_bld.Add("pinvoke_custom_sqlite3.cs");
                        break;
                    case "e_sqlite3":
                        cfg.csfiles_bld.Add("pinvoke_e_sqlite3.cs");
                        break;
                    case "sqlcipher":
                        cfg.csfiles_bld.Add("pinvoke_sqlcipher.cs");
                        break;
                    default:
                        throw new Exception(what);
                }
                break;
        }

        return cfg;
    }

    public static config_csproj create_wp80_provider(string cpu)
    {
        var cfg = new config_csproj();
        cfg.area = "provider";
        cfg.cpu = cpu;
        cfg.env = "wp80";
        cfg.what = "e_sqlite3";
        cfg.name = string.Format("{0}.provider.{1}.{2}.{3}", cfg.root_name, cfg.what, cfg.env, cfg.cpu);
        cfg.assemblyname = string.Format("{0}.provider.{1}", cfg.root_name, cfg.what);
        cfg.csfiles_src.Add("util.cs");
        cfg.csfiles_src.Add("sqlite3_cppinterop.cs");
        cfg.ref_core = true;
        cfg.ref_cppinterop = true;
        return cfg;
    }

    public static config_csproj create_ugly(string env)
    {
        var cfg = new config_csproj();
        cfg.area = "ugly";
        cfg.name = string.Format("{0}.ugly.{1}", cfg.root_name, env);
        cfg.assemblyname = string.Format("{0}.ugly", cfg.root_name);
        cfg.env = env;
        cfg.csfiles_src.Add("ugly.cs");
        cfg.ref_core = true;
        return cfg;
    }

    public static config_csproj create_bundle_test(string env, string bundle)
    {
        var cfg = new config_csproj();
        cfg.area = "test";
        cfg.name = string.Format("{0}.test.bundle_{1}.{2}", cfg.root_name, bundle, env);
        cfg.assemblyname = string.Format("{0}.tests", cfg.root_name);
        cfg.env = env;
        cfg.CopyNuGetImplementations = true;
        cfg.csfiles_src.Add("tests_xunit.cs");
        cfg.defines.Add("PROVIDER_bundle");

        cfg.deps["xunit"] = "2.1.0";

        cfg.deps[string.Format("{0}.ugly", gen.ROOT_NAME)] = gen.NUSPEC_VERSION;
        cfg.deps[string.Format("{0}.bundle_{1}", gen.ROOT_NAME, bundle)] = gen.NUSPEC_VERSION;
        return cfg;
    }

    public static config_csproj create_portable_test(string env)
    {
        var cfg = new config_csproj();
        cfg.area = "test";
        cfg.name = string.Format("{0}.test.portable.{1}", cfg.root_name, env);
        cfg.assemblyname = string.Format("{0}.tests", cfg.root_name);
        cfg.env = env;
        cfg.csfiles_src.Add("tests_xunit.cs");
        cfg.defines.Add("PROVIDER_none");

        cfg.deps["xunit"] = "2.1.0";
        cfg.deps[string.Format("{0}.ugly", gen.ROOT_NAME)] = gen.NUSPEC_VERSION;
        return cfg;
    }

    public static config_csproj create_portable_test_main(string env)
    {
        var cfg = new config_csproj();
        cfg.area = "test";
        cfg.name = string.Format("{0}.mtest.portable.{1}", cfg.root_name, env);
        cfg.assemblyname = string.Format("{0}.tests", cfg.root_name);
        cfg.env = env;
        cfg.csfiles_src.Add("tests_xunit.cs");
        cfg.defines.Add("PROVIDER_none");

        cfg.deps["xunit"] = "2.1.0";
        cfg.ref_ugly = true;
        cfg.ref_core = true;
        return cfg;
    }

    public static config_csproj create_batteries(string area, int ver, string env, string what)
    {
        return create_batteries(area, ver, env, what, null);
    }

    private static void set_batteries_version(config_csproj cfg, int ver)
    {
        switch (ver)
        {
            case 1:
                cfg.assemblyname = string.Format("{0}.{1}", cfg.root_name, cfg.area);
                cfg.csfiles_src.Add("batteries.cs");
                break;
            case 2:
                cfg.assemblyname = string.Format("{0}.batteries_v2", cfg.root_name);
                cfg.csfiles_src.Add("batteries_v2.cs");
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static config_csproj create_batteries(string area, int ver, string env, string what, string nuget_override_target_env)
    {
        var cfg = new config_csproj();
        cfg.env = env;
        cfg.area = area;
        cfg.nuget_override_target_env = nuget_override_target_env;
        cfg.name = string.Format("{0}.v{1}.{2}.{3}.{4}", cfg.root_name, ver, area, (what!=null)?what:"none", env);
        set_batteries_version(cfg, ver);
        cfg.defines.Add("PROVIDER_" + ((what!=null)?what:"none"));
        cfg.ref_core = true;
        cfg.ref_provider = what;
        return cfg;
    }

    public static config_csproj create_internal_batteries(string area, int ver, string env, string lib)
    {
        var cfg = new config_csproj();
        cfg.env = env;
        cfg.area = area;
        cfg.name = string.Format("{0}.v{1}.{2}.{3}.{4}", cfg.root_name, ver, area, "internal", env);
        set_batteries_version(cfg, ver);
        cfg.defines.Add("PROVIDER_internal");
        cfg.defines.Add("EMBEDDED_INIT");
        cfg.ref_core = true;
        cfg.ref_provider = "internal";
        cfg.ref_embedded = string.Format("{0}.lib.{1}.{2}.{3}", cfg.root_name, lib, env, "static");
        return cfg;
    }

    public static config_csproj create_wp80_batteries(string area, int ver, string cpu)
    {
        var cfg = new config_csproj();
        cfg.area = area;
        cfg.cpu = cpu;
        cfg.what = "e_sqlite3";
        cfg.env = "wp80";
        cfg.name = string.Format("{0}.{1}.{2}.{3}.{4}.{5}", cfg.root_name, ver, area, cfg.what, cfg.env, cfg.cpu);
        set_batteries_version(cfg, ver);
        cfg.defines.Add("PROVIDER_e_sqlite3");
        cfg.ref_core = true;
        cfg.ref_provider = cfg.what;
        return cfg;
    }

	public bool is_portable()
	{
		return config_cs.env_is_portable(env);
	}

	public bool is_netstandard()
	{
		return config_cs.env_is_netstandard(env);
	}

    public string get_name()
    {
        return name;
    }

	public string get_project_filename()
	{
		return string.Format("{0}.csproj", name);
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

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_name(), "bin", "release", s));
	}

	public void get_products(List<string> a)
	{
		add_product(a, string.Format("{0}.dll", assemblyname));
		if (ref_cppinterop)
		{
			var other = get_cppinterop_item();
            other.get_products(a);
		}
	}

    // TODO rm this func
	public string get_id()
	{
		return get_name();
	}

}

public static class gen
{
	private const string GUID_CSHARP = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
	private const string GUID_CPP = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
	private const string GUID_FOLDER = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
	private const string GUID_PCL = "{786C830F-07A1-408B-BD7F-6EE04809D6DB}";
	private const string GUID_IOS = "{6BC8ED88-2882-458C-8E55-DFD12B67127B}";
	private const string GUID_WATCHOS = "{FC940695-DFE0-4552-9F25-99AF4A5619A1}";
	private const string GUID_UNIFIED_IOS = "{FEACFBD2-3405-455C-9665-78FE426C6842}";
	private const string GUID_UNIFIED_MAC = "{A3F8F2AB-B479-4A4A-A458-A89E7DC349F1}";
	private const string GUID_ANDROID = "{EFBA0AD7-5A72-4C68-AF49-83D382785DCF}";
	private const string GUID_WINRT = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
	private const string GUID_WP8 = "{C089C8C0-30E0-4E22-80C0-CE093F111A43}";
	private const string GUID_WP81RT = "{76F1466A-8B6D-4E39-A767-685A06062A39}";
	private const string GUID_TEST = "{3AC096D0-A1C2-E12C-1390-A8335801FDAB}";
	private const string GUID_UAP = "{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A}";

    public const string ROOT_NAME = "SQLitePCLRaw";

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

	private static void write_section(string top, config_csproj cfg, XmlWriter f, bool debug, List<string> defines)
	{
		string name = debug ? "debug" : "release";
		f.WriteStartElement("PropertyGroup");
		f.WriteAttributeString("Condition", string.Format(" '$(Configuration)|$(Platform)' == '{0}|{1}' ", name, cfg.cpu));

		f.WriteElementString("OutputPath", string.Format("bin\\{0}", name));
		f.WriteElementString("IntermediateOutputPath", string.Format("obj\\{0}", name));

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
		foreach (string d in cfg.defines)
		{
			defs += d;
			defs += ";";
		}
		f.WriteElementString("DefineConstants", defs);

		f.WriteElementString("DebugSymbols", debug ? "true" : "false");
		f.WriteElementString("Optimize", debug ? "false" : "true");
		f.WriteElementString("DebugType", debug ? "full" : "none");

        if (!debug)
        {
            f.WriteElementString("SignAssembly", "true");
            f.WriteElementString("AssemblyOriginatorKeyFile", Path.Combine(top, "..", "sn", string.Format("{0}.snk", cfg.assemblyname)));
        }

		f.WriteEndElement(); // PropertyGroup
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

	private static void write_project_dot_json(string subdir, string framework, Dictionary<string,string> deps, List<string> runtimes)
	{
		using (TextWriter tw = new StreamWriter(Path.Combine(subdir, "project.json")))
		{
			tw.WriteLine("{");
			tw.WriteLine("    \"dependencies\" : {");
            foreach (var id in deps.Keys)
            {
                tw.WriteLine(string.Format("         \"{0}\": \"{1}\",", id, deps[id]));
            }
			tw.WriteLine("    },");
			tw.WriteLine("    \"frameworks\" : {");
			tw.WriteLine(string.Format("         \"{0}\": {{}}", framework));
			tw.WriteLine("    },");
            if (runtimes != null && runtimes.Count > 0)
            {
                tw.WriteLine("    \"runtimes\" : {");
                foreach (var r in runtimes)
                {
                    tw.WriteLine(string.Format("         \"{0}\": {{}},", r));
                }
                tw.WriteLine("    }");
            }
            else
            {
                tw.WriteLine("    \"supports\" : {}");
            }
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
		else if (config_cs.env_is_netstandard(env))
		{
			// TODO not sure about this
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
				case "uwp10":
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
				case "watchos":
					f.WriteStartElement("Import");
                    // TODO this is failing on my machine
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Xamarin\\WatchOS\\Xamarin.WatchOS.CSharp.targets");
					f.WriteEndElement(); // Import

					break;
				case "macos":
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
				case "uwp10":
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
			case "macos":
			case "watchos":
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
			case "watchos":
				write_reference(f, "Xamarin.WatchOS");
				break;
			case "macos":
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
		else if (config_cs.env_is_netstandard(env))
		{
			if (env == "netstandard10")
			{
				defines.Add("NO_CONCURRENTDICTIONARY");
			}
			f.WriteElementString("TargetFrameworkProfile", "");
			f.WriteElementString("TargetFrameworkVersion", "v5.0");
			f.WriteElementString("MinimumVisualStudioVersion", "14.0");
		}

		switch (env)
		{
			case "profile136":
				defines.Add("OLD_REFLECTION");
				break;
			case "ios_classic":
				defines.Add("PLATFORM_IOS");
				break;
			case "macos":
				f.WriteElementString("TargetFrameworkIdentifier", "Xamarin.Mac");
				f.WriteElementString("TargetFrameworkVersion", "v2.0");
				defines.Add("PLATFORM_UNIFIED"); // TODO need this?
				break;
			case "ios_unified":
			case "watchos":
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
			case "uwp10":
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
			case "uwp10":
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
		else if (config_cs.env_is_netstandard(env))
		{
            // TODO GUID_PCL ?
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
				case "watchos":
					write_project_type_guids(f, GUID_WATCHOS, GUID_CSHARP);
					break;
				case "macos":
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
				case "uwp10":
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
			f.WriteElementString("TargetName", "e_sqlite3");

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
			f.WriteElementString("OutDir", string.Format("bin\\$(Configuration)\\"));
			f.WriteElementString("IntDir", string.Format("obj\\$(Configuration)\\"));
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
			f.WriteElementString("OutDir", string.Format("bin\\$(Configuration)\\"));
			f.WriteElementString("IntDir", string.Format("obj\\$(Configuration)\\"));
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
			string sqlite3_item_path = Path.Combine(top, cfg.get_sqlite3_item().get_name(),
                    "bin",
                    "release",
                    "e_sqlite3.lib");
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

    private static void gen_testapp(
            config_testapp cfg,
            string root
            )
    {
        string vtests = string.Format("Tests_{0}", NUSPEC_VERSION);
		Directory.CreateDirectory(vtests);
        DirectoryCopy(Path.Combine(root, "Tests", cfg.pattern), Path.Combine(root, vtests, cfg.name), true);

        File.Delete(Path.Combine(root, vtests, cfg.name, "project.lock.json"));
        File.Delete(Path.Combine(root, vtests, cfg.name, string.Format("{0}.nuget.props", cfg.pattern)));
        File.Delete(Path.Combine(root, vtests, cfg.name, string.Format("{0}.nuget.targets", cfg.pattern)));
        File.Delete(Path.Combine(root, vtests, cfg.name, string.Format("{0}.csproj.user", cfg.pattern)));
	try
	{
		Directory.Delete(Path.Combine(root, vtests, cfg.name, "bin"), true);
	}
	catch
	{
	}
	try
	{
		Directory.Delete(Path.Combine(root, vtests, cfg.name, "obj"), true);
	}
	catch
	{
	}

        string old_csproj = Path.Combine(root, vtests, cfg.name, string.Format("{0}.csproj", cfg.pattern));
        string csproj = Path.Combine(root, vtests, cfg.name, string.Format("{0}.csproj", cfg.name));
        File.Move(old_csproj, csproj);
        string project_dot_json = Path.Combine(root, vtests, cfg.name, "project.json");

        replace(csproj, cfg.pattern, cfg.name);
        fix_guid(csproj, cfg.guid);
        replace(project_dot_json, "bundle_e_sqlite3", cfg.bundle);
        fix_version(project_dot_json);
    }

    private static void write_android_native_libs(string root, XmlWriter f, string which)
    {
        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\x86\\libe_sqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("x86\\libe_sqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\x86_64\\libe_sqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("x86_64\\libe_sqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\armeabi\\libe_sqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("armeabi\\libe_sqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\arm64-v8a\\libe_sqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("arm64-v8a\\libe_sqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\armeabi-v7a\\libe_sqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("armeabi-v7a\\libe_sqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

#if not
        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\mips\\libe_sqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("mips\\libe_sqlite3.so", which));
        f.WriteEndElement(); // EmbeddedNativeLibrary

        f.WriteStartElement("EmbeddedNativeLibrary");
        f.WriteAttributeString("Include", Path.Combine(root, string.Format("android\\{0}\\libs\\mips64\\libe_sqlite3.so", which)));
        f.WriteElementString("CopyToOutputDirectory", "Always");
        f.WriteElementString("Link", string.Format("mips64\\libe_sqlite3.so", which));
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

	private static void gen_assemblyinfo(config_csproj cfg, string root, string top)
	{
		string cs = File.ReadAllText(Path.Combine(root, "src/cs/AssemblyInfo.cs"));
		using (TextWriter tw = new StreamWriter(Path.Combine(top, string.Format("AssemblyInfo.{0}.cs", cfg.assemblyname))))
		{
			string cs1 = cs.Replace("REPLACE_WITH_ASSEMBLY_NAME", '"' + cfg.assemblyname + '"');
			tw.Write(cs1);
		}
	}

	private static void gen_csproj(config_csproj cfg, string root, string top)
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
            if (cfg.CopyNuGetImplementations)
            {
                f.WriteElementString("CopyNuGetImplementations", "true");
            }

			f.WriteStartElement("Configuration");
			f.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
			f.WriteString("Debug");
			f.WriteEndElement(); // Configuration

			f.WriteElementString("SchemaVersion", "2.0");
			f.WriteElementString("Platform", cfg.cpu.Replace(" ", ""));
			f.WriteElementString("RootNamespace", "TODO");
			f.WriteElementString("AssemblyName", cfg.assemblyname);

			List<string> defines = write_header_properties(f, cfg.env);

			f.WriteEndElement(); // PropertyGroup

			write_section(top, cfg, f, true, defines);
			write_section(top, cfg, f, false, defines);

			f.WriteStartElement("ItemGroup");
			write_standard_assemblies(f, cfg.env);
			f.WriteEndElement(); // ItemGroup

			f.WriteStartElement("ItemGroup");
            string src = Path.Combine(root, "src\\cs");
			foreach (string csfile in cfg.csfiles_src)
			{
				write_cs_compile(f, src, csfile);
			}
			foreach (string csfile in cfg.csfiles_bld)
			{
				write_cs_compile(f, top, csfile);
			}
			write_cs_compile(f, top, string.Format("AssemblyInfo.{0}.cs", cfg.assemblyname));
			f.WriteEndElement(); // ItemGroup

            if (cfg.ref_core)
            {
                f.WriteStartElement("ItemGroup");

                f.WriteStartElement("ProjectReference");
                {
                    config_csproj other = projects.find_core(cfg.env);
                    f.WriteAttributeString("Include", other.get_project_path(top));
                    f.WriteElementString("Project", other.guid);
                    f.WriteElementString("Name", other.get_name());
                    //f.WriteElementString("Private", "true");
                }
                f.WriteEndElement(); // ProjectReference

                f.WriteEndElement(); // ItemGroup
            }

            if (cfg.ref_ugly)
            {
                f.WriteStartElement("ItemGroup");

                f.WriteStartElement("ProjectReference");
                {
                    config_csproj other = projects.find_ugly(cfg.env);
                    f.WriteAttributeString("Include", other.get_project_path(top));
                    f.WriteElementString("Project", other.guid);
                    f.WriteElementString("Name", other.get_name());
                    //f.WriteElementString("Private", "true");
                }
                f.WriteEndElement(); // ProjectReference

                f.WriteEndElement(); // ItemGroup
            }

            if (cfg.ref_provider != null)
            {
                f.WriteStartElement("ItemGroup");

                f.WriteStartElement("ProjectReference");
                {
                    config_csproj other = projects.find_provider(cfg.ref_provider, cfg.env, cfg.cpu);
                    f.WriteAttributeString("Include", other.get_project_path(top));
                    f.WriteElementString("Project", other.guid);
                    f.WriteElementString("Name", other.get_name());
                    //f.WriteElementString("Private", "true");
                }
                f.WriteEndElement(); // ProjectReference

                f.WriteEndElement(); // ItemGroup
            }

            if (cfg.ref_embedded != null)
            {
                f.WriteStartElement("ItemGroup");

                f.WriteStartElement("ProjectReference");
                {
                    config_csproj other = projects.find_name(cfg.ref_embedded);
                    f.WriteAttributeString("Include", other.get_project_path(top));
                    f.WriteElementString("Project", other.guid);
                    f.WriteElementString("Name", other.get_name());
                    //f.WriteElementString("Private", "true");
                }
                f.WriteEndElement(); // ProjectReference

                f.WriteEndElement(); // ItemGroup
            }

			if (cfg.ref_cppinterop)
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

            if (cfg.area == "lib")
            {
			switch (cfg.env)
			{
				case "ios_unified":
                        if (cfg.what == "e_sqlite3")
                        {
                            f.WriteStartElement("ItemGroup");
                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            // TODO underscore
                            f.WriteAttributeString("Include", Path.Combine(root, "apple\\libs\\ios\\sqlite3\\e_sqlite3.a"));
                            f.WriteElementString("Link", "e_sqlite3.a");
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
			else
			{
				throw new NotImplementedException(string.Format("{0}, {1}", cfg.env, cfg.what));
			}
					break;

					case "watchos":
                        if (cfg.what == "e_sqlite3")
                        {
                            f.WriteStartElement("ItemGroup");
                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            // TODO underscore
                            f.WriteAttributeString("Include", Path.Combine(root, "apple\\libs\\watchos\\sqlite3\\e_sqlite3.a"));
                            f.WriteElementString("Link", "e_sqlite3.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture
                            f.WriteEndElement(); // ItemGroup
                        }
            else if (cfg.what == "sqlcipher")
                        {
                            f.WriteStartElement("ItemGroup");

                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            f.WriteAttributeString("Include", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\watchos\\libsqlcipher.a"));
                            f.WriteElementString("Link", "libsqlcipher.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture

                            f.WriteEndElement(); // ItemGroup
                        }
            else
            {
                throw new NotImplementedException(string.Format("{0}, {1}", cfg.env, cfg.what));
            }
                    break;

                    case "ios_classic":
                        if (cfg.what == "e_sqlite3")
                        {
                            f.WriteStartElement("ItemGroup");
                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            // TODO underscore
                            f.WriteAttributeString("Include", Path.Combine(root, "apple\\libs\\ios\\sqlite3\\e_sqlite3.a"));
                            f.WriteElementString("Link", "e_sqlite3.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture
                            f.WriteEndElement(); // ItemGroup
                        }
			else if (cfg.what == "sqlcipher")
                        {
                            f.WriteStartElement("ItemGroup");

                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            // TODO underscore
                            f.WriteAttributeString("Include", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\ios\\libsqlcipher.a"));
                            f.WriteElementString("Link", "libsqlcipher.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture

                            // TODO warning says this is deprecated
                            f.WriteStartElement("ManifestResourceWithNoCulture");
                            f.WriteAttributeString("Include", Path.Combine(root, "apple\\libs\\ios\\libcrypto.a"));
                            f.WriteElementString("Link", "libcrypto.a");
                            f.WriteEndElement(); // ManifestResourceWithNoCulture

                            f.WriteEndElement(); // ItemGroup
                        }
			else
			{
				throw new NotImplementedException(string.Format("{0}, {1}", cfg.env, cfg.what));
			}
						break;

					case "android":
						if (cfg.what == "e_sqlite3")
						{
                            f.WriteStartElement("ItemGroup");
                            write_android_native_libs(root, f, "sqlite3");
                            f.WriteEndElement(); // ItemGroup
						}
						else if (cfg.what == "sqlcipher")
						{
                            f.WriteStartElement("ItemGroup");
                            write_android_native_libs_sqlcipher(root, f);
                            f.WriteEndElement(); // ItemGroup
                        }
			else
			{
				throw new NotImplementedException(string.Format("{0}, {1}", cfg.env, cfg.what));
			}

						break;
            }
            }

            f.WriteStartElement("ItemGroup");
            f.WriteStartElement("None");
            f.WriteAttributeString("Include", "project.json");
            f.WriteEndElement(); // None
            f.WriteEndElement(); // ItemGroup

			write_csproj_footer(f, cfg.env);

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}

		string subdir = cfg.get_project_subdir(top);
        switch (cfg.env)
        {
            case "net35":
            case "net40":
            case "net45":
                cfg.runtimes.Add("win");
                break;
			case "uwp10":
                cfg.deps["Microsoft.NETCore.UniversalWindowsPlatform"] = "5.2.2";
                cfg.runtimes.Add("win10-arm");
                cfg.runtimes.Add("win10-x86");
                cfg.runtimes.Add("win10-x64");
                cfg.runtimes.Add("win10-arm-aot");
                cfg.runtimes.Add("win10-x86-aot");
                cfg.runtimes.Add("win10-x64-aot");
				break;
			case "netstandard10":
                cfg.deps["NETStandard.Library"] = "1.6.0";
				break;
			case "netstandard11":
                cfg.deps["NETStandard.Library"] = "1.6.0";
				break;
        }

        write_project_dot_json(subdir, config_cs.get_full_framework_name(cfg.env), cfg.deps, cfg.runtimes);
	}

	public static void gen_solution(string top)
	{
		using (StreamWriter f = new StreamWriter(Path.Combine(top, "sqlitepcl.sln")))
		{
			f.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			f.WriteLine("# Visual Studio 14");
			f.WriteLine("VisualStudioVersion = 14.0");
			f.WriteLine("MinimumVisualStudioVersion = 12.0");

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

			foreach (config_csproj cfg in projects.items_csproj)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\\{2}\", \"{3}\"",
						GUID_CSHARP,
						cfg.get_name(),
						cfg.get_project_filename(),
						cfg.guid
						);
                // TODO project dependency
                if (cfg.ref_core)
                {
                    // TODO
                }
                if (cfg.ref_provider != null)
                {
                    // TODO
                }
                if (cfg.ref_cppinterop)
                {
					f.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
					config_cppinterop other = cfg.get_cppinterop_item();
					f.WriteLine("\t\t{0} = {0}", other.guid);
					f.WriteLine("\tEndProjectSection");
                }
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
			foreach (config_csproj cfg in projects.items_csproj)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.fixed_cpu());
			}
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
			f.WriteLine("\t\tHideSolutionNode = FALSE");
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("EndGlobal");
		}
	}

	public static void gen_test_solution(string top)
	{
		using (StreamWriter f = new StreamWriter(Path.Combine(top, "test.sln")))
		{
			f.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			f.WriteLine("# Visual Studio 14");
			f.WriteLine("VisualStudioVersion = 14.0");
			f.WriteLine("MinimumVisualStudioVersion = 12.0");

			foreach (config_csproj cfg in projects.items_test)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\\{2}\", \"{3}\"",
						GUID_CSHARP,
						cfg.get_name(),
						cfg.get_project_filename(),
						cfg.guid
						);
				f.WriteLine("EndProject");
			}

			f.WriteLine("Global");

			f.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			f.WriteLine("\t\tDebug|Mixed Platforms = Debug|Mixed Platforms");
			f.WriteLine("\t\tRelease|Mixed Platforms = Release|Mixed Platforms");
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
			foreach (config_csproj cfg in projects.items_test)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.fixed_cpu());
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.fixed_cpu());
			}
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
			f.WriteLine("\t\tHideSolutionNode = FALSE");
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("EndGlobal");
		}
	}

	public static void gen_testapp_solution(string root, List<config_testapp> a)
	{
        string vtests = string.Format("Tests_{0}", NUSPEC_VERSION);
		using (StreamWriter f = new StreamWriter(Path.Combine(vtests, "testapps.sln")))
		{
			f.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			f.WriteLine("# Visual Studio 14");
			f.WriteLine("VisualStudioVersion = 14.0");
			f.WriteLine("MinimumVisualStudioVersion = 12.0");

			foreach (config_testapp cfg in a)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\\{2}.csproj\", \"{3}\"",
						GUID_CSHARP,
						cfg.name,
						cfg.name,
						cfg.guid
						);
				f.WriteLine("EndProject");
			}

			f.WriteLine("Global");

			f.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			f.WriteLine("\t\tDebug|Mixed Platforms = Debug|Mixed Platforms");
			f.WriteLine("\t\tRelease|Mixed Platforms = Release|Mixed Platforms");
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
			foreach (config_testapp cfg in projects.items_testapp)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.cpu);
                if (cfg.env == "uwp10")
                {
                    f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Deploy.0 = Debug|{1}", cfg.guid, cfg.cpu);
                }
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.cpu);
                if (cfg.env == "uwp10")
                {
                    f.WriteLine("\t\t{0}.Release|Mixed Platforms.Deploy.0 = Release|{1}", cfg.guid, cfg.cpu);
                }
			}
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
			f.WriteLine("\t\tHideSolutionNode = FALSE");
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("EndGlobal");
		}
	}

	private static void write_nuspec_file_entry(config_csproj cfg, XmlWriter f)
    {
        if (cfg.nuget_override_target_env != null)
        {
            write_nuspec_file_entry(cfg, cfg.nuget_override_target_env, f);
        }
        else
        {
            write_nuspec_file_entry(cfg, cfg.env, f);
        }
    }

	private static void write_nuspec_file_entry(config_csproj cfg, string target_env, XmlWriter f)
	{
        // note that target_env may not be the same as cfg.env
        // for example we may want to build with netstandard11
        // settings, and then drop that assembly into more than
        // one place in the nuget file
		f.WriteComment(string.Format("{0}", cfg.get_name()));
		var a = new List<string>();
		cfg.get_products(a);

		foreach (string s in a)
		{
			f.WriteStartElement("file");
			f.WriteAttributeString("src", s);
			f.WriteAttributeString("target", projects.get_nuget_target_path(target_env));
			f.WriteEndElement(); // file
		}
	}

	private static void write_nuspec_file_entry_wp80(config_csproj cfg, XmlWriter f)
	{
		f.WriteComment(string.Format("{0}", cfg.get_name()));
		var a = new List<string>();
		cfg.get_products(a);

		foreach (string s in a)
		{
			f.WriteStartElement("file");
			f.WriteAttributeString("src", s);
			f.WriteAttributeString("target", string.Format("build\\{0}\\{1}\\", cfg.env, cfg.cpu));
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
			f.WriteAttributeString("src", s);
			f.WriteAttributeString("target", cfg.get_nuget_target_path());
			f.WriteEndElement(); // file
		}
	}

	private static void write_cppinterop_with_targets_file(XmlWriter f, List<config_csproj> a, string env, string top, string id)
	{
        foreach (var cfg in a)
        {
            write_nuspec_file_entry_wp80(cfg, f);
        }

		f.WriteComment("empty directory in lib to avoid nuget adding a reference to the bait");

		Directory.CreateDirectory(Path.Combine(Path.Combine(top, "empty"), config_cs.get_nuget_framework_name(env)));

		f.WriteStartElement("file");
		f.WriteAttributeString("src", string.Format("empty\\{0}\\", config_cs.get_nuget_framework_name(env)));
		f.WriteAttributeString("target", string.Format("lib\\{0}", config_cs.get_nuget_framework_name(env)));
		f.WriteEndElement(); // file

		f.WriteComment("msbuild .targets file to inject reference for the right cpu");

		string tname = string.Format("{0}.targets", id);
		gen_nuget_targets_cppinterop(top, tname, a);

		f.WriteStartElement("file");
		f.WriteAttributeString("src", tname);
		f.WriteAttributeString("target", string.Format("build\\{0}\\{1}.targets", config_cs.get_nuget_framework_name(env), id));
		f.WriteEndElement(); // file
	}

	//public static string NUSPEC_VERSION = string.Format("1.1.5-pre{0}", DateTime.Now.ToString("yyyyMMddHHmmss")); 
	//public static string NUSPEC_VERSION = "1.0.0-PLACEHOLDER";
	public static string NUSPEC_VERSION = "1.1.5";

	private const string NUSPEC_RELEASE_NOTES = "1.1.5:  bug fix path in lib.foo.linux targets file.  1.1.4:  tweak use of nuget .targets files for compat with .NET Core.  1.1.3:  add SQLITE_CHECKPOINT_TRUNCATE symbol definition.  add new blob overloads to enable better performance in certain cases.  chg winsqlite3 to use StdCall.  fix targets files for better compat with VS 2017 nuget pack.  add 32-bit linux build for e_sqlite3.  update to latest libcrypto builds from couchbase folks.  1.1.2:  ability to FreezeProvider().  update e_sqlite3 builds to 3.16.1.  1.1.1:  add support for config_log.  update e_sqlite3 builds to 3.15.2.  fix possible memory corruption when using prepare_v2() with multiple statements.  better errmsg from ugly.step().  add win8 dep groups in bundles.  fix batteries_v2.Init() to be 'last call wins' like the v1 version is.  chg raw.SetProvider() to avoid calling sqlite3_initialize() so that sqlite3_config() can be used.  better support for Xamarin.Mac.  1.1.0:  fix problem with winsqlite3 on UWP.  remove iOS Classic support.  add sqlite3_enable_load_extension.  add sqlite3_config/initialize/shutdown.  add Batteries_V2.Init().  1.0.1:  fix problem with bundle_e_sqlite3 on iOS.  fix issues with .NET Core.  add bundle_sqlcipher.  1.0.0 release:  Contains minor breaking changes since 0.9.x.  All package names now begin with SQLitePCLRaw.  Now supports netstandard.  Fixes for UWP and Android N.  Change all unit tests to xunit.  Support for winsqlite3.dll and custom SQLite builds.";

    private static void add_dep_core(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", string.Format("{0}.core", gen.ROOT_NAME));
        f.WriteAttributeString("version", NUSPEC_VERSION);
        f.WriteEndElement(); // dependency
    }

    private static void add_dep_ugly(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", string.Format("{0}.ugly", gen.ROOT_NAME));
        f.WriteAttributeString("version", NUSPEC_VERSION);
        f.WriteEndElement(); // dependency
    }

    private static void add_dep_xunit(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", "xunit");
        f.WriteAttributeString("version", "2.1.0");
        f.WriteEndElement(); // dependency
    }

    private static void add_dep_netstandard(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", "NETStandard.Library");
        f.WriteAttributeString("version", "1.6.0");
        f.WriteEndElement(); // dependency
    }

	private static void gen_nuspec_core(string top, string root)
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
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
            f.WriteElementString("title", id);
			f.WriteElementString("description", "SQLitePCL.raw is a Portable Class Library (PCL) for low-level (raw) access to SQLite.  This package does not provide an API which is friendly to app developers.  Rather, it provides an API which handles platform and configuration issues, upon which a friendlier API can be built.  In order to use this package, you will need to also add one of the SQLitePCLRaw.provider.* packages and call raw.SetProvider().  Convenience packages are named SQLitePCLRaw.bundle_*.");
			f.WriteElementString("authors", "Eric Sink, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite pcl database xamarin monotouch ios monodroid android wp8 wpa netstandard uwp");

			f.WriteStartElement("dependencies");

            write_dependency_group(f, "android", DEP_NONE);
            write_dependency_group(f, "ios_unified", DEP_NONE);
            write_dependency_group(f, "macos", DEP_NONE);
            // TODO write_dependency_group(f, "watchos", DEP_NONE);
            write_dependency_group(f, "net35", DEP_NONE);
            write_dependency_group(f, "net40", DEP_NONE);
            write_dependency_group(f, "net45", DEP_NONE);
            write_dependency_group(f, "win81", DEP_NONE);
            write_dependency_group(f, "wpa81", DEP_NONE);
            write_dependency_group(f, "wp80", DEP_NONE);
            write_dependency_group(f, "uwp10", DEP_NONE);
            write_dependency_group(f, "profile111", DEP_NONE);
            write_dependency_group(f, "profile136", DEP_NONE);
            write_dependency_group(f, "profile259", DEP_NONE);
            write_dependency_group(f, "netstandard11", DEP_NONE);
            write_dependency_group(f, null, DEP_NONE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
                if (cfg.area == "core")
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
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");
			f.WriteElementString("authors", "Eric Sink, D. Richard Hipp, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite");

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
                    if (tname != null) 
                    {
                        f.WriteStartElement("file");
                        f.WriteAttributeString("src", tname);
                        f.WriteAttributeString("target", string.Format("build\\net35\\{0}.targets", id));
                        f.WriteEndElement(); // file
                    }
					break;
				default:
					tname = gen_nuget_targets_sqlite3_itself(top, cfg.get_id(), cfg.toolset);
                    if (tname != null) 
                    {
                        f.WriteStartElement("file");
                        f.WriteAttributeString("src", tname);
                        f.WriteAttributeString("target", string.Format("build\\{0}.targets", id));
                        f.WriteEndElement(); // file
                    }
					break;
			}


			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_embedded(string top, string root, config_csproj cfg)
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
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");
			f.WriteElementString("authors", "Eric Sink, D. Richard Hipp, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite xamarin");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

            write_nuspec_file_entry(
                    cfg, 
                    f
                    );

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_provider_wp80(string top, string root, string what)
    {
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = string.Format("{0}.provider.{1}.{2}", gen.ROOT_NAME, what, "wp80");
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			string desc = string.Format("A SQLitePCL.raw 'provider' bridges the gap between SQLitePCLRaw.core and a particular instance of the native SQLite library.  Install this package in your app project and call SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());");
            desc = desc + "  Depending on the platform, you may also need to add one of the SQLitePCLRaw.lib.* packages.  Convenience packages are named SQLitePCLRaw.bundle_*.";
			f.WriteElementString("description", desc);
			f.WriteElementString("authors", "Eric Sink, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite wp8");

			f.WriteStartElement("dependencies");

            write_dependency_group(f, "wp80", DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

            var a = projects.items_csproj.Where(cfg => (cfg.area == "provider" && cfg.env == "wp80")).ToList();

            write_cppinterop_with_targets_file(f, a, "wp80", top, id);

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_provider(string top, string root, config_csproj cfg)
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
			f.WriteElementString("title", id);
			string desc = string.Format("A SQLitePCL.raw 'provider' bridges the gap between SQLitePCLRaw.core and a particular instance of the native SQLite library.  Install this package in your app project and call SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_{0}());", cfg.what);
            desc = desc + "  Depending on the platform, you may also need to add one of the SQLitePCLRaw.lib.* packages.  Convenience packages are named SQLitePCLRaw.bundle_*.";
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

			f.WriteStartElement("dependencies");

            write_dependency_group(f, cfg.env, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			write_nuspec_file_entry(
					cfg, 
					f
					);

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_e_sqlite3(string top, string root, string plat)
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
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", string.Format("Native code only (e_sqlite3, {0}) for SQLitePCLRaw", plat));
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");
			f.WriteElementString("authors", "Eric Sink, D. Richard Hipp, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			string tname = string.Format("{0}.targets", id);
            switch (plat)
            {
                case "osx":
                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(root, "apple", "libs", "mac", "sqlite3", "libe_sqlite3.dylib"));
                    f.WriteAttributeString("target", "runtimes\\osx-x64\\native\\libe_sqlite3.dylib");
                    f.WriteEndElement(); // file
                    gen_nuget_targets_osx(top, tname, "libe_sqlite3.dylib");
                    break;
                case "linux":
                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(root, "linux", "x64", "libe_sqlite3.so"));
                    f.WriteAttributeString("target", "runtimes\\linux-x64\\native\\libe_sqlite3.so");
                    f.WriteEndElement(); // file

                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(root, "linux", "x86", "libe_sqlite3.so"));
                    f.WriteAttributeString("target", "runtimes\\linux-x86\\native\\libe_sqlite3.so");
                    f.WriteEndElement(); // file

                    gen_nuget_targets_linux(top, tname, "libe_sqlite3.so");
                    break;
                default:
                    throw new Exception();
            }

            f.WriteStartElement("file");
            f.WriteAttributeString("src", tname);
            f.WriteAttributeString("target", string.Format("build\\net35\\{0}.targets", id));
            f.WriteEndElement(); // file

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

		string id = string.Format("SQLitePCLRaw.lib.sqlcipher.{0}", plat);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", string.Format("Native code only (sqlcipher, {0}) for SQLitePCLRaw", plat));
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLCipher (see sqlcipher/sqlcipher on GitHub) for use with SQLitePCL.raw.  The build of SQLCipher packaged here is built and maintained by Couchbase (see couchbaselabs/couchbase-lite-libsqlcipher on GitHub).  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.sqlcipher.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");
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
					f.WriteAttributeString("target", string.Format("runtimes\\win7-x86\\native\\sqlcipher.dll"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\windows\\x86_64\\sqlcipher.dll"));
					f.WriteAttributeString("target", string.Format("runtimes\\win7-x64\\native\\sqlcipher.dll"));
					f.WriteEndElement(); // file

					gen_nuget_targets_windows(top, tname, "sqlcipher.dll");
					break;
				case "osx":
					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\osx\\libsqlcipher.dylib"));
					f.WriteAttributeString("target", string.Format("runtimes\\osx-x64\\native\\libsqlcipher.dylib"));
					f.WriteEndElement(); // file

					gen_nuget_targets_osx(top, tname, "libsqlcipher.dylib");
					break;
				case "linux":
					// TODO do we need amd64 version here?

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\linux\\x86_64\\libsqlcipher.so"));
					f.WriteAttributeString("target", string.Format("runtimes\\linux-x64\\native\\libsqlcipher.so"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(root, "couchbase-lite-libsqlcipher\\libs\\linux\\x86\\libsqlcipher.so"));
					f.WriteAttributeString("target", string.Format("runtimes\\linux-x86\\native\\libsqlcipher.so"));
					f.WriteEndElement(); // file

					gen_nuget_targets_linux(top, tname, "libsqlcipher.so");
					break;
				default:
					throw new Exception();
			}
			f.WriteStartElement("file");
			f.WriteAttributeString("src", tname);
			f.WriteAttributeString("target", string.Format("build\\net35\\{0}.targets", id));
			f.WriteEndElement(); // file

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_tests(string top)
	{
		string id = string.Format("{0}.tests", gen.ROOT_NAME);

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
			f.WriteElementString("title", id);
			f.WriteElementString("description", "tests");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "tests");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

            write_dependency_group(f, "android", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "ios_unified", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "macos", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            // TODO write_dependency_group(f, "watchos", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "net35", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "net40", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "net45", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "win81", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "wpa81", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "wp80", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "uwp10", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "profile111", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "profile136", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "profile259", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, "netstandard11", DEP_CORE | DEP_UGLY | DEP_XUNIT);
            write_dependency_group(f, null, DEP_CORE | DEP_UGLY | DEP_XUNIT);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "test")
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

	private static void gen_nuspec_ugly(string top)
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
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "These extension methods for SQLitePCL.raw provide a more usable API while remaining stylistically similar to the sqlite3 C API, which most C# developers would consider 'ugly'.  This package exists for people who (1) really like the sqlite3 C API, and (2) really like C#.  So far, evidence suggests that 100% of the people matching both criteria are named Eric Sink, but this package is available just in case he is not the only one of his kind.");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Extension methods for SQLitePCLRaw, providing an ugly-but-usable API");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

            write_dependency_group(f, "android", DEP_CORE);
            write_dependency_group(f, "ios_unified", DEP_CORE);
            write_dependency_group(f, "macos", DEP_CORE);
            // TODO write_dependency_group(f, "watchos", DEP_CORE);
            write_dependency_group(f, "net35", DEP_CORE);
            write_dependency_group(f, "net40", DEP_CORE);
            write_dependency_group(f, "net45", DEP_CORE);
            write_dependency_group(f, "win81", DEP_CORE);
            write_dependency_group(f, "wpa81", DEP_CORE);
            write_dependency_group(f, "wp80", DEP_CORE);
            write_dependency_group(f, "uwp10", DEP_CORE);
            write_dependency_group(f, "profile111", DEP_CORE);
            write_dependency_group(f, "profile136", DEP_CORE);
            write_dependency_group(f, "profile259", DEP_CORE);
            write_dependency_group(f, "netstandard11", DEP_CORE);
            write_dependency_group(f, null, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "ugly")
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

	private static void gen_nuspec_bundle_winsqlite3(string top)
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
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: .no SQLite library included, uses winsqlite3.dll");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Batteries-included package to bring in SQLitePCL.raw and dependencies");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

			// --------
			f.WriteStartElement("group");
			f.WriteAttributeString("targetFramework", config_cs.get_nuget_framework_name("uwp10"));

			f.WriteStartElement("dependency");
			f.WriteAttributeString("id", string.Format("{0}.core", gen.ROOT_NAME));
			f.WriteAttributeString("version", NUSPEC_VERSION);
			f.WriteEndElement(); // dependency

			f.WriteStartElement("dependency");
			f.WriteAttributeString("id", string.Format("{0}.provider.winsqlite3.uwp10", gen.ROOT_NAME));
			f.WriteAttributeString("version", NUSPEC_VERSION);
			f.WriteEndElement(); // dependency

			f.WriteEndElement(); // group

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "batteries_winsqlite3" && cfg.env != "wp80")
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

    private const int DEP_NONE = 0;
    private const int DEP_CORE = 1;
    private const int DEP_UGLY = 2;
    private const int DEP_XUNIT = 4;

    private static void write_dependency_group(XmlWriter f, string env, int flags)
    {
        f.WriteStartElement("group");
        if (env != null)
        {
            f.WriteAttributeString("targetFramework", config_cs.get_nuget_framework_name(env));
            switch (env)
            {
                case "uwp10":
                case "netstandard11":
                    add_dep_netstandard(f);
                    break;
            }
        }
        if ((flags & DEP_CORE) != 0)
        {
            add_dep_core(f);
        }
        if ((flags & DEP_UGLY) != 0)
        {
            add_dep_ugly(f);
        }
        if ((flags & DEP_XUNIT) != 0)
        {
            add_dep_xunit(f);
        }
        f.WriteEndElement(); // group
    }

    private static void write_bundle_dependency_group(XmlWriter f, string env, string what)
    {
        write_bundle_dependency_group(f, env, env, what);
    }

    private static void write_bundle_dependency_group(XmlWriter f, string env_target, string env_deps, string what)
    {
        // --------
        f.WriteStartElement("group");
        f.WriteAttributeString("targetFramework", config_cs.get_nuget_framework_name(env_target));

        add_dep_core(f);

        if (
                ((env_deps == "ios_unified") || (env_deps == "ios_classic") || (env_deps == "watchos"))
                && (what != "sqlite3")
           )
        {
            f.WriteStartElement("dependency");
            f.WriteAttributeString("id", string.Format("{0}.provider.{1}.{2}", gen.ROOT_NAME, "internal", env_deps));
            f.WriteAttributeString("version", NUSPEC_VERSION);
            f.WriteEndElement(); // dependency
        }
        else
        {
            f.WriteStartElement("dependency");
            f.WriteAttributeString("id", string.Format("{0}.provider.{1}.{2}", gen.ROOT_NAME, what, env_deps));
            f.WriteAttributeString("version", NUSPEC_VERSION);
            f.WriteEndElement(); // dependency
        }

        if (what == "e_sqlite3")
        {
            switch (env_deps)
            {
                case "android":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.android", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "macos":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "ios_unified":
                case "ios_classic":
                case "watchos":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.{1}.static", gen.ROOT_NAME, env_deps));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "net35":
                case "net40":
                case "net45":
                case "netstandard11": // TODO because this is used for netcoreapp, kinda hackish
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.v110_xp", gen.ROOT_NAME));
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
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.{1}", gen.ROOT_NAME, projects.cs_env_to_toolset(env_deps)));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
            }
        }
        else if (what == "sqlcipher")
        {
            switch (env_deps)
            {
                case "android":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.android", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "macos":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "ios_unified":
                case "ios_classic":
                case "watchos":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.{1}.static", gen.ROOT_NAME, env_deps));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "net35":
                case "net40":
                case "net45":
                case "netstandard11": // TODO because this is used for netcoreapp, kinda hackish
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
                default:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.{1}", gen.ROOT_NAME, projects.cs_env_to_toolset(env_deps)));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
            }
        }

        f.WriteEndElement(); // group
    }

	private static void gen_nuspec_bundle_sqlcipher(string top)
    {
		string id = string.Format("{0}.bundle_sqlcipher", gen.ROOT_NAME);

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
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: sqlcipher included");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Batteries-included package to bring in SQLitePCL.raw and dependencies");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, "android", "sqlcipher");
            write_bundle_dependency_group(f, "ios_unified", "sqlcipher");
            write_bundle_dependency_group(f, "macos", "sqlcipher");
            // TODO write_bundle_dependency_group(f, "watchos", "sqlcipher");
            write_bundle_dependency_group(f, "net35", "sqlcipher");
            write_bundle_dependency_group(f, "net40", "sqlcipher");
            write_bundle_dependency_group(f, "net45", "sqlcipher");
            write_bundle_dependency_group(f, "netcoreapp", "netstandard11", "sqlcipher");
            
            write_dependency_group(f, "profile111", DEP_CORE);
            write_dependency_group(f, "profile136", DEP_CORE);
            write_dependency_group(f, "profile259", DEP_CORE);
            write_dependency_group(f, "netstandard11", DEP_CORE);
            write_dependency_group(f, null, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "batteries_sqlcipher")
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

	private static void gen_nuspec_bundle_e_sqlite3(string top)
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
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: e_sqlite3 included");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Batteries-included package to bring in SQLitePCL.raw and dependencies");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, "android", "e_sqlite3");
            write_bundle_dependency_group(f, "ios_unified", "e_sqlite3");
            write_bundle_dependency_group(f, "macos", "e_sqlite3");
            // TODO write_bundle_dependency_group(f, "watchos", "e_sqlite3");
            write_bundle_dependency_group(f, "wpa81", "e_sqlite3");
            write_bundle_dependency_group(f, "wp80", "e_sqlite3");
            write_bundle_dependency_group(f, "win8", "e_sqlite3");
            write_bundle_dependency_group(f, "win81", "e_sqlite3");
            write_bundle_dependency_group(f, "uwp10", "e_sqlite3");
            write_bundle_dependency_group(f, "net35", "e_sqlite3");
            write_bundle_dependency_group(f, "net40", "e_sqlite3");
            write_bundle_dependency_group(f, "net45", "e_sqlite3");
            write_bundle_dependency_group(f, "netcoreapp", "netstandard11", "e_sqlite3");
            
            write_dependency_group(f, "profile111", DEP_CORE);
            write_dependency_group(f, "profile136", DEP_CORE);
            write_dependency_group(f, "profile259", DEP_CORE);
            write_dependency_group(f, "netstandard11", DEP_CORE);
            write_dependency_group(f, null, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "batteries_e_sqlite3" && cfg.env != "wp80")
				{
					write_nuspec_file_entry(
							cfg, 
							f
							);
				}
			}

            var a = projects.items_csproj.Where(cfg => (cfg.area == "batteries_e_sqlite3" && cfg.env == "wp80")).ToList();

            write_cppinterop_with_targets_file(f, a, "wp80", top, id);
            
			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_bundle_green(string top)
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
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: iOS=system SQLite, others=SQLite included");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2016 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Batteries-included package to bring in SQLitePCL.raw and dependencies");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, "android", "e_sqlite3");
            write_bundle_dependency_group(f, "ios_unified", "sqlite3");
            write_bundle_dependency_group(f, "macos", "e_sqlite3");
            // TODO write_bundle_dependency_group(f, "watchos", "sqlite3");
            write_bundle_dependency_group(f, "wpa81", "e_sqlite3");
            write_bundle_dependency_group(f, "wp80", "e_sqlite3");
            write_bundle_dependency_group(f, "win8", "e_sqlite3");
            write_bundle_dependency_group(f, "win81", "e_sqlite3");
            write_bundle_dependency_group(f, "uwp10", "e_sqlite3");
            write_bundle_dependency_group(f, "net35", "e_sqlite3");
            write_bundle_dependency_group(f, "net40", "e_sqlite3");
            write_bundle_dependency_group(f, "net45", "e_sqlite3");
            write_bundle_dependency_group(f, "netcoreapp", "netstandard11", "e_sqlite3");

            write_dependency_group(f, "profile111", DEP_CORE);
            write_dependency_group(f, "profile136", DEP_CORE);
            write_dependency_group(f, "profile259", DEP_CORE);
            write_dependency_group(f, "netstandard11", DEP_CORE);
            write_dependency_group(f, null, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "batteries_green" && cfg.env != "wp80")
				{
					write_nuspec_file_entry(
							cfg, 
							f
							);
				}
			}

            var a = projects.items_csproj.Where(cfg => (cfg.area == "batteries_green" && cfg.env == "wp80")).ToList();

            write_cppinterop_with_targets_file(f, a, "wp80", top, id);

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
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\{0}", Path.Combine(other.get_nuget_target_path(), "e_sqlite3.dll")));
				// TODO link
				// TODO condition/exists ?
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteElementString("Pack", "false");
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
			f.WriteAttributeString("Condition", " '$(OS)' == 'Windows_NT' ");
			foreach (config_sqlite3 other in projects.items_sqlite3)
			{
				if (toolset != other.toolset)
				{
					continue;
				}

				f.WriteStartElement("Content");
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\{0}", Path.Combine(other.get_nuget_target_path(), "e_sqlite3.dll")));
				// TODO condition/exists ?
				f.WriteElementString("Link", string.Format("{0}\\e_sqlite3.dll", other.cpu.ToLower()));
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
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\win7-x86\\native\\{0}", filename));
			f.WriteElementString("Link", string.Format("{0}\\{1}", "x86", filename));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\win7-x64\\native\\{0}", filename));
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

	private static void gen_nuget_targets_osx(string top, string tname, string filename)
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
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\osx-x64\\native\\{0}", filename));
			f.WriteElementString("Link", filename);
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

	private static void gen_nuget_targets_cppinterop(string top, string tname, List<config_csproj> a)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		Dictionary<string,string> cpus = new Dictionary<string,string>();
		foreach (var cfg in a)
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

			foreach (var cfg in a)
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
				
				f.WriteComment(string.Format("{0}", cfg.get_name()));
				f.WriteStartElement("ItemGroup");
                f.WriteAttributeString("Condition", string.Format(" '$(Platform.ToLower())' == '{0}' ", cfg.cpu.ToLower()));

				f.WriteStartElement("Reference");
				// TODO should Include be the HintPath?
				// https://github.com/onovotny/WinRTTimeZones/blob/master/NuGet/WinRTTimeZones.WP8.targets
				f.WriteAttributeString("Include", cfg.assemblyname);

				f.WriteElementString("HintPath", string.Format("$(MSBuildThisFileDirectory)..\\..\\{0}\\{1}.dll", string.Format("build\\{0}\\{1}\\", cfg.env, cfg.cpu), cfg.assemblyname));

				// TODO private?

				// TODO name?

				f.WriteEndElement(); // Reference

				f.WriteEndElement(); // ItemGroup
			}

			f.WriteEndElement(); // Target

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }

    static void replace(string path, string oldstr, string newstr)
    {
        string txt = File.ReadAllText(path);
        using (TextWriter tw = new StreamWriter(path))
        {
            string cs1 = txt.Replace(oldstr, newstr);
            tw.Write(cs1);
        }
    }

    static void fix_version(string path)
    {
        replace(path, "1.0.0-PLACEHOLDER", NUSPEC_VERSION);
    }

    static void fix_guid(string path, string guid)
    {
        var a = File.ReadAllLines(path);
        using (TextWriter tw = new StreamWriter(path))
        {
            foreach (var s in a)
            {
                if (s.Contains("ProjectGuid"))
                {
                    tw.WriteLine("<ProjectGuid>{0}</ProjectGuid>", guid);
                }
                else
                {
                    tw.WriteLine(s);
                }
            }
        }
    }

	public static void Main(string[] args)
	{
		projects.init();

		string root = Directory.GetCurrentDirectory(); // assumes that gen_build.exe is being run from the root directory of the project
		string top = Path.Combine(root, "bld");

		// --------------------------------
		// create the bld directory
		Directory.CreateDirectory(top);

		string cs_pinvoke = File.ReadAllText(Path.Combine(root, "src/cs/sqlite3_pinvoke.cs"));
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_sqlite3.cs")))
		{
			string cs = cs_pinvoke
                .Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "sqlite3")
                .Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "sqlite3")
                .Replace("REPLACE_WITH_CALLING_CONVENTION", "Cdecl")
                ;
			tw.Write(cs);
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_e_sqlite3.cs")))
		{
			string cs = cs_pinvoke
                .Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "e_sqlite3")
                .Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "e_sqlite3")
                .Replace("REPLACE_WITH_CALLING_CONVENTION", "Cdecl")
                ;
			tw.Write(cs);
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_sqlcipher.cs")))
		{
			string cs = cs_pinvoke
                .Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "sqlcipher")
                .Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "sqlcipher")
                .Replace("REPLACE_WITH_CALLING_CONVENTION", "Cdecl")
                ;
			tw.Write(cs);
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_custom_sqlite3.cs")))
		{
			string cs = cs_pinvoke
                .Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "custom_sqlite3")
                .Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "custom_sqlite3")
                .Replace("REPLACE_WITH_CALLING_CONVENTION", "Cdecl")
                ;
			tw.Write(cs);
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_winsqlite3.cs")))
		{
			string cs = cs_pinvoke
                .Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "winsqlite3")
                .Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "winsqlite3")
                .Replace("REPLACE_WITH_CALLING_CONVENTION", "StdCall")
                ;
			tw.Write(cs);
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_sqlite3_xamarin.cs")))
		{
			string cs = cs_pinvoke
                .Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "sqlite3_xamarin")
                .Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "sqlite3_xamarin")
                .Replace("REPLACE_WITH_CALLING_CONVENTION", "Cdecl")
                ;
			tw.Write(cs);
		}
		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pinvoke_ios_internal.cs")))
		{
			string cs = cs_pinvoke
                .Replace("REPLACE_WITH_SIMPLE_DLL_NAME", "internal")
                .Replace("REPLACE_WITH_ACTUAL_DLL_NAME", "__Internal")
                .Replace("REPLACE_WITH_CALLING_CONVENTION", "Cdecl")
                ;
			tw.Write(cs);
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

		foreach (config_csproj cfg in projects.items_csproj)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		foreach (config_csproj cfg in projects.items_test)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		foreach (config_testapp cfg in projects.items_testapp)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		// --------------------------------
		// generate all the AssemblyInfo files

		foreach (config_csproj cfg in projects.items_csproj)
		{
			gen_assemblyinfo(cfg, root, top);
		}

		foreach (config_csproj cfg in projects.items_test)
		{
			gen_assemblyinfo(cfg, root, top);
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

		foreach (config_csproj cfg in projects.items_csproj)
		{
			gen_csproj(cfg, root, top);
		}

		foreach (config_csproj cfg in projects.items_test)
		{
			gen_csproj(cfg, root, top);
		}

		foreach (config_testapp cfg in projects.items_testapp)
		{
			gen_testapp(cfg, root);
		}

		// --------------------------------

		gen_solution(top);
		gen_testapp_solution(top, projects.items_testapp);
		gen_test_solution(top);

		// --------------------------------

        gen_nuspec_core(top, root);
        gen_nuspec_ugly(top);
        gen_nuspec_bundle_green(top);
        gen_nuspec_bundle_e_sqlite3(top);
        gen_nuspec_bundle_winsqlite3(top);
        gen_nuspec_bundle_sqlcipher(top);
        gen_nuspec_provider_wp80(top, root, "e_sqlite3");
        gen_nuspec_tests(top);

		foreach (config_csproj cfg in projects.items_csproj)
		{
            if (cfg.area == "provider" && cfg.env != "wp80")
            {
                gen_nuspec_provider(top, root, cfg);
            }
		}

		foreach (config_csproj cfg in projects.items_csproj)
		{
            if (cfg.area == "lib")
            {
                gen_nuspec_embedded(top, root, cfg);
            }
		}

		foreach (config_esqlite3 cfg in projects.items_esqlite3)
		{
			gen_nuspec_esqlite3(top, root, cfg);
		}

		gen_nuspec_e_sqlite3(top, root, "osx");
		gen_nuspec_e_sqlite3(top, root, "linux");

		gen_nuspec_sqlcipher(top, root, "windows");
		gen_nuspec_sqlcipher(top, root, "osx");
		gen_nuspec_sqlcipher(top, root, "linux");

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "build.ps1")))
		{
			tw.WriteLine("../../nuget restore sqlitepcl.sln");
			tw.WriteLine("msbuild /p:Configuration=Release sqlitepcl.sln");
		}

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pack.ps1")))
		{
            tw.WriteLine("../../nuget pack {0}.core.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../../nuget pack {0}.ugly.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../../nuget pack {0}.bundle_green.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../../nuget pack {0}.bundle_e_sqlite3.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../../nuget pack {0}.bundle_sqlcipher.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../../nuget pack {0}.bundle_winsqlite3.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../../nuget pack {0}.provider.e_sqlite3.wp80.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../../nuget pack {0}.tests.nuspec", gen.ROOT_NAME);

			tw.WriteLine("../../nuget pack {0}.lib.e_sqlite3.osx.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../../nuget pack {0}.lib.e_sqlite3.linux.nuspec", gen.ROOT_NAME);

			tw.WriteLine("../../nuget pack {0}.lib.sqlcipher.windows.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../../nuget pack {0}.lib.sqlcipher.osx.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../../nuget pack {0}.lib.sqlcipher.linux.nuspec", gen.ROOT_NAME);

			foreach (config_csproj cfg in projects.items_csproj)
			{
                if (cfg.area == "provider" && cfg.env != "wp80")
                {
                    string id = cfg.get_id();
                    tw.WriteLine("../../nuget pack {0}.nuspec", id);
                }
			}
			foreach (config_csproj cfg in projects.items_csproj)
			{
                if (cfg.area == "lib")
                {
                    string id = cfg.get_id();
                    tw.WriteLine("../../nuget pack {0}.nuspec", id);
                }
			}
			foreach (config_esqlite3 cfg in projects.items_esqlite3)
			{
				string id = cfg.get_id();
				tw.WriteLine("../../nuget pack {0}.nuspec", id);
			}
			tw.WriteLine("ls *.nupkg");
		}

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "bt.ps1")))
		{
            string vtests = string.Format("Tests_{0}", NUSPEC_VERSION);
			tw.WriteLine("cd ../{0}", vtests);
			tw.WriteLine("../../nuget restore -Source '{0}' -Source https://www.nuget.org/api/v2 ./testapps.sln", top);
		}

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "push.ps1")))
		{
            const string src = "https://www.nuget.org/api/v2/package";

			tw.WriteLine("ls *.nupkg");
			tw.WriteLine("../../nuget push -Source {2} {0}.core.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../../nuget push -Source {2} {0}.ugly.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../../nuget push -Source {2} {0}.bundle_green.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../../nuget push -Source {2} {0}.bundle_e_sqlite3.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../../nuget push -Source {2} {0}.bundle_sqlcipher.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../../nuget push -Source {2} {0}.bundle_winsqlite3.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../../nuget push -Source {2} {0}.provider.e_sqlite3.wp80.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("#../../nuget push -Source {2} {0}.tests.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			tw.WriteLine("../../nuget push -Source {2} {0}.lib.e_sqlite3.osx.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../../nuget push -Source {2} {0}.lib.e_sqlite3.linux.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			tw.WriteLine("../../nuget push -Source {2} {0}.lib.sqlcipher.windows.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../../nuget push -Source {2} {0}.lib.sqlcipher.osx.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../../nuget push -Source {2} {0}.lib.sqlcipher.linux.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			foreach (config_csproj cfg in projects.items_csproj)
			{
                if (cfg.area == "provider" && cfg.env != "wp80")
                {
                    string id = cfg.get_id();
                    tw.WriteLine("../../nuget push -Source {2} {0}.{1}.nupkg", id, NUSPEC_VERSION, src);
                }
			}
			foreach (config_csproj cfg in projects.items_csproj)
			{
                if (cfg.area == "lib")
                {
                    string id = cfg.get_id();
                    tw.WriteLine("../../nuget push -Source {2} {0}.{1}.nupkg", id, NUSPEC_VERSION, src);
                }
			}
			foreach (config_esqlite3 cfg in projects.items_esqlite3)
			{
				string id = cfg.get_id();
				tw.WriteLine("../../nuget push -Source {2} {0}.{1}.nupkg", id, NUSPEC_VERSION, src);
			}
		}
	}
}

