using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenBuild
{
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
		public Dictionary<string, string> deps = new Dictionary<string, string>();
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
							cfg.defines.Add(string.Format("IOS_PACKAGED_{0}", what.ToUpper()));
							//							throw new Exception(what);
							break;
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
							cfg.csfiles_src.Add("imp_ios_internal.cs");
							//							break; throw new Exception(what);
							break;
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
							cfg.csfiles_bld.Add(string.Format("pinvoke_{0}.cs", what));
							break;
							//throw new Exception(what);
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
			cfg.name = string.Format("{0}.v{1}.{2}.{3}.{4}", cfg.root_name, ver, area, (what != null) ? what : "none", env);
			set_batteries_version(cfg, ver);
			cfg.defines.Add("PROVIDER_" + ((what != null) ? what : "none"));
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

}
