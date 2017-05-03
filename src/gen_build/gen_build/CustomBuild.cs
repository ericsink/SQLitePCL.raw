using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenBuild
{
	// TODO Create .snk for build if it does not exists - right now just use 'sn -k ...name...'
	// TODO Generate bundle from dylib/so/etc.
	// TODO Generate tests?
	public class CustomBuild
	{
		private string _name;
		private string _libRoot;

		public CustomBuild(string name)
			: this(name, null)
		{		
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="libRoot">e.g., C:\Users\tim\ownCloud\Loqu8\ext - containing sqlite\macosx\universal\lib\libxsqlite3.dylib</param>
		public CustomBuild(string name, string libRoot)
		{
			_name = name;
			_libRoot = libRoot;
		}

		private List<config_sqlite3> _itemsSqlite3;
		public List<config_cppinterop> _itemsCppinterop;
		public List<config_csproj> _itemsCsproj;

		public List<config_sqlite3> ItemsSqlite3
		{
			get
			{
				if (_itemsSqlite3 == null)
					_itemsSqlite3 = genItemsSqlite3();

				return _itemsSqlite3;
			}
		}

		public List<config_cppinterop> ItemsCppinterop
		{
			get
			{
				if (_itemsCppinterop == null)
					_itemsCppinterop = genItemsCppinterop(_name);

				return _itemsCppinterop;
			}
		}

		public List<config_csproj> ItemsCsproj
		{
			get
			{
				if (_itemsCsproj == null)
					_itemsCsproj = genItemsCsproj(_name);

				return _itemsCsproj;
			}
		}

		protected virtual List<config_sqlite3> genItemsSqlite3()
		{
			var items_sqlite3 = new List<config_sqlite3>();
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v110_xp", cpu = "x86" });
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v110_xp", cpu = "x64" });

			//items_sqlite3.Add(new config_sqlite3 { toolset = "v110", cpu = "arm" });
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v110", cpu = "x64" });
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v110", cpu = "x86" });

			//items_sqlite3.Add(new config_sqlite3 { toolset = "v120", cpu = "arm" });
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v120", cpu = "x64" });
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v120", cpu = "x86" });

			//items_sqlite3.Add(new config_sqlite3 { toolset = "v140", cpu = "arm" });
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v140", cpu = "x64" });
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v140", cpu = "x86" });

			//items_sqlite3.Add(new config_sqlite3 { toolset = "v110_wp80", cpu = "arm" });
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v110_wp80", cpu = "x86" });

			//items_sqlite3.Add(new config_sqlite3 { toolset = "v120_wp81", cpu = "arm" });
			//items_sqlite3.Add(new config_sqlite3 { toolset = "v120_wp81", cpu = "x86" });

			foreach (config_sqlite3 cfg in items_sqlite3)
			{
				cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
			}
			return items_sqlite3;
		}

		protected virtual List<config_cppinterop> genItemsCppinterop(string name)
		{
			var items_cppinterop = new List<config_cppinterop>();
			//items_cppinterop.Add(new config_cppinterop { env = "wp80", cpu = "arm" });
			//items_cppinterop.Add(new config_cppinterop { env = "wp80", cpu = "x86" });

			foreach (config_cppinterop cfg in items_cppinterop)
			{
				cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
			}
			return items_cppinterop;
		}

		protected virtual List<config_csproj> genItemsCsproj(string name)
		{
			var items_csproj = new List<config_csproj>();
			genCoreItemsCsproj(items_csproj);

			// generate provider
			//items_csproj.Add(config_csproj.create_provider(name, "netstandard11"));
			//items_csproj.Add(config_csproj.create_provider(name, "net35"));
			//items_csproj.Add(config_csproj.create_provider(name, "net40"));
			items_csproj.Add(config_csproj.create_provider(name, "net45"));
			items_csproj.Add(config_csproj.create_provider(name, "android"));
			//items_csproj.Add(config_csproj.create_provider(name, "win8"));
			//items_csproj.Add(config_csproj.create_provider(name, "win81"));
			//items_csproj.Add(config_csproj.create_provider(name, "wpa81"));
			items_csproj.Add(config_csproj.create_provider(name, "uwp10"));
			items_csproj.Add(config_csproj.create_provider(name, "macos"));
			// ios would only make sense here with dylibs - prefer internal
			//items_csproj.Add(config_csproj.create_provider(customName, "ios_unified"));

			items_csproj.Add(config_csproj.create_provider("internal", "ios_unified"));

			if (!string.IsNullOrEmpty(_libRoot)) {
				// generate batteries
				items_csproj.Add(config_csproj.create_embedded(_name, "android"));
				items_csproj.Add(config_csproj.create_embedded(_name, "ios_unified"));

				// e.g., bundle_xsqlite3
				var ver = 2;
				var area = string.Format("batteries_{0}", _name);
				items_csproj.Add(config_csproj.create_internal_batteries(area, ver, "ios_unified", _name));
				// TODO items_csproj.Add(config_csproj.create_internal_batteries("batteries_e_sqlite3", "watchos", "e_sqlite3"));
				items_csproj.Add(config_csproj.create_batteries(area, ver, "android", _name));
				items_csproj.Add(config_csproj.create_batteries(area, ver, "macos", _name));
				//items_csproj.Add(config_csproj.create_batteries(area, ver, "win8", _name));
				//items_csproj.Add(config_csproj.create_batteries(area, ver, "wpa81", _name));
				//items_csproj.Add(config_csproj.create_batteries(area, ver, "win81", _name));
				items_csproj.Add(config_csproj.create_batteries(area, ver, "uwp10", _name));
				//items_csproj.Add(config_csproj.create_batteries(area, ver, "net35", _name));
				//items_csproj.Add(config_csproj.create_batteries(area, ver, "net40", _name));
				items_csproj.Add(config_csproj.create_batteries(area, ver, "net45", _name));
			}

			foreach (config_csproj cfg in items_csproj)
			{
				cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
			}
			return items_csproj;
		}

		private static void genCoreItemsCsproj(List<config_csproj> items_csproj)
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
		}
	}
}
