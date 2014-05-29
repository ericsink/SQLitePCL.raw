
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

	// This function is called by Main to initialize the project lists.
	//
	public static void init()
	{
		init_sqlite3(false);
		init_sqlite3(true);
		init_cppinterop(false);
		init_cppinterop(true);
		init_pcl_bait();
		init_pcl_pinvoke();
		init_pcl_cppinterop(false);
		init_pcl_cppinterop(true);
	}

	private static void init_sqlite3(bool dyn)
	{
		items_sqlite3.Add(new config_sqlite3 { env="winxp", cpu="x86", dll=dyn });
		items_sqlite3.Add(new config_sqlite3 { env="winxp", cpu="x64", dll=dyn });

		items_sqlite3.Add(new config_sqlite3 { env="winrt80", cpu="arm", dll=dyn });
		items_sqlite3.Add(new config_sqlite3 { env="winrt80", cpu="x64", dll=dyn });
		items_sqlite3.Add(new config_sqlite3 { env="winrt80", cpu="x86", dll=dyn });

		items_sqlite3.Add(new config_sqlite3 { env="winrt81", cpu="arm", dll=dyn });
		items_sqlite3.Add(new config_sqlite3 { env="winrt81", cpu="x64", dll=dyn });
		items_sqlite3.Add(new config_sqlite3 { env="winrt81", cpu="x86", dll=dyn });

		items_sqlite3.Add(new config_sqlite3 { env="wp80", cpu="arm", dll=dyn });
		items_sqlite3.Add(new config_sqlite3 { env="wp80", cpu="x86", dll=dyn });

		items_sqlite3.Add(new config_sqlite3 { env="wp81_rt", cpu="arm", dll=dyn });
		items_sqlite3.Add(new config_sqlite3 { env="wp81_rt", cpu="x86", dll=dyn });

		items_sqlite3.Add(new config_sqlite3 { env="wp81_sl", cpu="arm", dll=dyn });
		items_sqlite3.Add(new config_sqlite3 { env="wp81_sl", cpu="x86", dll=dyn });
	}

	private static void init_cppinterop(bool dyn)
	{
		items_cppinterop.Add(new config_cppinterop { env="net45", cpu="x64", dll=dyn });
		items_cppinterop.Add(new config_cppinterop { env="net45", cpu="x86", dll=dyn });

		items_cppinterop.Add(new config_cppinterop { env="winrt80", cpu="arm", dll=dyn });
		items_cppinterop.Add(new config_cppinterop { env="winrt80", cpu="x64", dll=dyn });
		items_cppinterop.Add(new config_cppinterop { env="winrt80", cpu="x86", dll=dyn });

		items_cppinterop.Add(new config_cppinterop { env="winrt81", cpu="arm", dll=dyn });
		items_cppinterop.Add(new config_cppinterop { env="winrt81", cpu="x64", dll=dyn });
		items_cppinterop.Add(new config_cppinterop { env="winrt81", cpu="x86", dll=dyn });

		items_cppinterop.Add(new config_cppinterop { env="wp80", cpu="arm", dll=dyn });
		items_cppinterop.Add(new config_cppinterop { env="wp80", cpu="x86", dll=dyn });

		items_cppinterop.Add(new config_cppinterop { env="wp81_rt", cpu="arm", dll=dyn });
		items_cppinterop.Add(new config_cppinterop { env="wp81_rt", cpu="x86", dll=dyn });

		items_cppinterop.Add(new config_cppinterop { env="wp81_sl", cpu="arm", dll=dyn });
		items_cppinterop.Add(new config_cppinterop { env="wp81_sl", cpu="x86", dll=dyn });
	}

	private static void init_pcl_bait()
	{
		items_pcl.Add(new config_pcl { env="profile78", cpu="anycpu" });
		items_pcl.Add(new config_pcl { env="profile259", cpu="anycpu" });
		items_pcl.Add(new config_pcl { env="profile158", cpu="anycpu" });
	}

	private static void init_pcl_cppinterop(bool dyn)
	{
		items_pcl.Add(new config_pcl { env="net45", api="cppinterop", what="sqlite3", cpu="x86", dll=dyn});
		items_pcl.Add(new config_pcl { env="net45", api="cppinterop", what="sqlite3", cpu="x64", dll=dyn});

		items_pcl.Add(new config_pcl { env="winrt80", api="cppinterop", what="sqlite3", cpu="arm", dll=dyn});
		items_pcl.Add(new config_pcl { env="winrt80", api="cppinterop", what="sqlite3", cpu="x64", dll=dyn});
		items_pcl.Add(new config_pcl { env="winrt80", api="cppinterop", what="sqlite3", cpu="x86", dll=dyn});

		items_pcl.Add(new config_pcl { env="winrt81", api="cppinterop", what="sqlite3", cpu="arm", dll=dyn});
		items_pcl.Add(new config_pcl { env="winrt81", api="cppinterop", what="sqlite3", cpu="x64", dll=dyn});
		items_pcl.Add(new config_pcl { env="winrt81", api="cppinterop", what="sqlite3", cpu="x86", dll=dyn});

		items_pcl.Add(new config_pcl { env="wp80", api="cppinterop", what="sqlite3", cpu="arm", dll=dyn});
		items_pcl.Add(new config_pcl { env="wp80", api="cppinterop", what="sqlite3", cpu="x86", dll=dyn});

		items_pcl.Add(new config_pcl { env="wp81_rt", api="cppinterop", what="sqlite3", cpu="arm", dll=dyn});
		items_pcl.Add(new config_pcl { env="wp81_rt", api="cppinterop", what="sqlite3", cpu="x86", dll=dyn});

		items_pcl.Add(new config_pcl { env="wp81_sl", api="cppinterop", what="sqlite3", cpu="arm", dll=dyn});
		items_pcl.Add(new config_pcl { env="wp81_sl", api="cppinterop", what="sqlite3", cpu="x86", dll=dyn});
	}

	private static void init_pcl_pinvoke()
	{
		items_pcl.Add(new config_pcl { env="android", api="pinvoke", what="sqlite3", cpu="anycpu"});
		items_pcl.Add(new config_pcl { env="ios", api="pinvoke", what="sqlite3", cpu="anycpu"});
		items_pcl.Add(new config_pcl { env="ios", api="pinvoke", what="internal_other", cpu="anycpu"});

		items_pcl.Add(new config_pcl { env="net45", api="pinvoke", what="sqlite3", cpu="anycpu"});
		items_pcl.Add(new config_pcl { env="net45", api="pinvoke", what="sqlite3", cpu="x86"});
		items_pcl.Add(new config_pcl { env="net45", api="pinvoke", what="sqlite3", cpu="x64"});

		items_pcl.Add(new config_pcl { env="winrt80", api="pinvoke", what="sqlite3", cpu="anycpu"});
		items_pcl.Add(new config_pcl { env="winrt80", api="pinvoke", what="sqlite3", cpu="arm"});
		items_pcl.Add(new config_pcl { env="winrt80", api="pinvoke", what="sqlite3", cpu="x64"});
		items_pcl.Add(new config_pcl { env="winrt80", api="pinvoke", what="sqlite3", cpu="x86"});

		items_pcl.Add(new config_pcl { env="winrt81", api="pinvoke", what="sqlite3", cpu="anycpu"});
		items_pcl.Add(new config_pcl { env="winrt81", api="pinvoke", what="sqlite3", cpu="arm"});
		items_pcl.Add(new config_pcl { env="winrt81", api="pinvoke", what="sqlite3", cpu="x64"});
		items_pcl.Add(new config_pcl { env="winrt81", api="pinvoke", what="sqlite3", cpu="x86"});
	}

	public static List<config_pcl> find_pcls(string env, string api, string what, string cpu, string linkage)
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
					(what != null)
					&& (cfg.what != what)
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

			if (linkage != null)
			{
				if (
						(linkage == "static")
						&& (cfg.dll)
				   )
				{
					continue;
				}
				if (
						(linkage == "dynamic")
						&& (!cfg.dll)
				   )
				{
					continue;
				}
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

public class config_sqlite3 : config_info
{
	public string env;
	public string cpu;
	public string guid;
	public bool dll = false;

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_dest_subpath(), s));
	}

	public void get_products(List<string> a)
	{
		if (dll)
		{
			add_product(a, "sqlite3.dll");
		}
	}

	private string area()
	{
		return "sqlite3_" + (dll ? "dynamic" : "static");
	}

	public string get_dest_subpath()
	{
		return string.Format("{0}\\{1}\\{2}", area(), env, cpu);
	}

	public string get_name()
	{
		return string.Format("{0}.{1}.{2}", area(), env, cpu);
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
	public bool dll = false;

	public config_sqlite3 get_sqlite3_item()
	{
		foreach (config_sqlite3 cfg in projects.items_sqlite3)
		{
			if (
					(cfg.env == sqlite3_env())
					&& (cfg.cpu == cpu)
					&& (cfg.dll == dll)
			   )
			{
				return cfg;
			}
		}
		throw new Exception(get_name());
	}

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_dest_subpath(), s));
	}

	public void get_products(List<string> a, bool needy)
	{
		add_product(a, "SQLitePCL.cppinterop.dll");
		switch (env)
		{
			case "wp80":
				add_product(a, "SQLitePCL.cppinterop.winmd");
				break;

			case "winrt80":
			case "winrt81":
			case "wp81_rt":
			case "wp81_sl":
				add_product(a, "SQLitePCL.cppinterop.pri");
				add_product(a, "SQLitePCL.cppinterop.winmd");
				break;
		}

		if (!needy)
		{
			config_sqlite3 other = get_sqlite3_item();
			other.get_products(a);
		}
	}

	private string area()
	{
		return "cppinterop_sqlite3_" + (dll ? "dynamic" : "static");
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

	private string sqlite3_env()
	{
		if (env == "net45")
		{
			return "winxp";
		}
		else
		{
			return env;
		}
	}
}

public class config_pcl : config_info
{
	public string env;
	public string api;
	public string what;
	public string cpu;
	public string guid;
	public bool dll; // TODO should be string linkage, so it can be null for cases where it is not used

	public config_cppinterop get_cppinterop_item()
	{
		foreach (config_cppinterop cfg in projects.items_cppinterop)
		{
			if (
					(cfg.env == env)
					&& (cfg.cpu == cpu)
					&& (cfg.dll == dll)
			   )
			{
				return cfg;
			}
		}
		throw new Exception(get_name());
	}

	public static string get_nuget_framework_name(string env)
	{
		// TODO maybe I should just use these names?
		switch (env)
		{
			case "ios":
				return "MonoTouch";
			case "android":
				return "MonoAndroid";
			case "net45":
				return "net45";
			case "wp80":
				return "wp8";
			case "wp81_sl":
				return "wp81";
			case "wp81_rt":
				return "wpa81";
			case "winrt80":
				return "netcore45";
			case "winrt81":
				return "netcore451";
			default:
				throw new Exception(env);
		}
	}
					
	private string nat()
	{
		if (is_pinvoke())
		{
			return string.Format("{0}_{1}", api, what);
		}
		else if (is_cppinterop())
		{
			return string.Format("{0}_{1}_{2}", api, what, (dll ? "dynamic" : "static"));
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
				return string.Format("build\\{0}\\{1}\\{2}\\", get_nuget_framework_name(env), nat(), cpu);
			}
		}
		else if ("lib" == where)
		{
			if (is_portable())
			{
				return string.Format("lib\\{0}\\", get_portable_nuget_target_string());
			}
			else
			{
				return string.Format("lib\\{0}\\", get_nuget_framework_name(env));
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

	public void get_products(List<string> a, bool needy)
	{
		add_product(a, "SQLitePCL.dll");
		switch (env)
		{
			case "winrt80":
			case "winrt81":
			case "wp81_rt":
			//case "wp81_sl":
				add_product(a, "SQLitePCL.pri");
				break;
		}

		if (is_cppinterop())
		{
			config_cppinterop other = get_cppinterop_item();
			other.get_products(a, needy);
		}
	}

	public bool is_portable()
	{
		return env.StartsWith("profile");
	}

	public string get_portable_nuget_target_string()
	{
		switch (env)
		{
			case "profile78":
				return "portable-net45+netcore45+wp8+MonoAndroid+MonoTouch";
			case "profile259":
				// TODO so, when Xamarin adds support for profile 259, the nuget target string below will be wrong?
				return "portable-net45+netcore45+wpa81+wp8";
			case "profile158":
				return "portable-net45+sl5+netcore45+wp8+MonoAndroid+MonoTouch";
			default:
				throw new Exception(env);
		}
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
}

public static class gen
{
	private const string GUID_CSHARP = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
	private const string GUID_CPP = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
	private const string GUID_FOLDER = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
	private const string GUID_PCL = "{786C830F-07A1-408B-BD7F-6EE04809D6DB}";
	private const string GUID_IOS = "{6BC8ED88-2882-458C-8E55-DFD12B67127B}";
	private const string GUID_ANDROID = "{EFBA0AD7-5A72-4C68-AF49-83D382785DCF}";
	private const string GUID_WINRT = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
	private const string GUID_WP8 = "{C089C8C0-30E0-4E22-80C0-CE093F111A43}";
	private const string GUID_WP81RT = "{76F1466A-8B6D-4E39-A767-685A06062A39}";

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

	private static void write_section(config_pcl cfg, XmlWriter f, bool debug, List<string> defines)
	{
		string name = debug ? "debug" : "release";
		f.WriteStartElement("PropertyGroup");
		f.WriteAttributeString("Condition", string.Format(" '$(Configuration)|$(Platform)' == '{0}|{1}' ", name, cfg.cpu));

		f.WriteElementString("OutputPath", string.Format("{0}\\bin\\{1}\\", name, cfg.get_dest_subpath()));
		f.WriteElementString("IntermediateOutputPath", string.Format("{0}\\obj\\{1}\\", name, cfg.get_dest_subpath()));

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

	private static void gen_sqlite3(config_sqlite3 cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, cfg.get_project_filename()), settings))
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

			switch (cfg.env)
			{
				case "winxp":
					break;
				case "winrt80":
					f.WriteElementString("MinimumVisualStudioVersion", "11.0");
					f.WriteElementString("WindowsAppContainer", "true");
					f.WriteElementString("AppContainerApplication", "true");
					break;
				case "winrt81":
					f.WriteElementString("MinimumVisualStudioVersion", "12.0");
					f.WriteElementString("WindowsAppContainer", "true");
					f.WriteElementString("AppContainerApplication", "true");
					break;
				case "wp80":
					f.WriteElementString("MinimumVisualStudioVersion", "11.0");
					break;
				case "wp81_rt":
					f.WriteElementString("MinimumVisualStudioVersion", "12.0");
					f.WriteElementString("AppContainerApplication", "true");
					f.WriteElementString("ApplicationType", "Windows Phone");
					f.WriteElementString("ApplicationTypeRevision", "8.1");
					break;
				case "wp81_sl":
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
			if (cfg.dll)
			{
				f.WriteElementString("ConfigurationType", "DynamicLibrary");
			}
			else
			{
				f.WriteElementString("ConfigurationType", "StaticLibrary");
			}
			f.WriteElementString("TargetName", "sqlite3");

			switch (cfg.env)
			{
				case "winxp":
					f.WriteElementString("PlatformToolset", "v110_xp");
					break;
				case "winrt80":
					f.WriteElementString("PlatformToolset", "v110");
					break;
				case "winrt81":
					f.WriteElementString("PlatformToolset", "v120");
					break;
				case "wp80":
					f.WriteElementString("PlatformToolset", "v110_wp80");
					break;
				case "wp81_rt":
					f.WriteElementString("PlatformToolset", "v120_wp81");
					break;
				case "wp81_sl":
					f.WriteElementString("PlatformToolset", "v120");
					break;
			}

			f.WriteEndElement(); // PropertyGroup

			switch (cfg.env)
			{
				case "winxp":
					break;
				case "winrt80":
				case "winrt81":
				case "wp80":
				case "wp81_rt":
				case "wp81_sl":
					f.WriteStartElement("ItemDefinitionGroup");
					f.WriteStartElement("ClCompile");
					write_cpp_define(f, "SQLITE_OS_WINRT");
					f.WriteEndElement(); // ClCompile
					f.WriteEndElement(); // ItemDefinitionGroup
					break;
			}

			switch (cfg.env)
			{
				case "winxp":
					break;
				case "winrt80":
				case "winrt81":
					break;
				case "wp80":
				case "wp81_rt":
				case "wp81_sl":
					f.WriteStartElement("ItemDefinitionGroup");
					f.WriteStartElement("ClCompile");
					write_cpp_define(f, "SQLITE_WIN32_FILEMAPPING_API=1");
					f.WriteEndElement(); // ClCompile
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
			f.WriteElementString("OutDir", string.Format("$(Configuration)\\bin\\{0}\\", cfg.get_dest_subpath()));
			f.WriteElementString("IntDir", string.Format("$(Configuration)\\obj\\{0}\\", cfg.get_dest_subpath()));
			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteStartElement("ClCompile");
			if (cfg.dll)
			{
				write_cpp_define(f, "_USRDLL");
				write_cpp_define(f, "SQLITE_API=__declspec(dllexport)");
			}
			else
			{
				write_cpp_define(f, "_LIB");
			}
			//write_cpp_define(f, "SQLITE_OMIT_LOAD_EXTENSION");
			//write_cpp_define(f, "SQLITE_THREADSAFE=whatever");
			//write_cpp_define(f, "SQLITE_TEMP_STORE=whatever");
			write_cpp_define(f, "SQLITE_DEFAULT_FOREIGN_KEYS=1");
			//write_cpp_define(f, "SQLITE_ENABLE_RTREE");
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

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, cfg.get_project_filename()), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			switch (cfg.env)
			{
				case "winrt81":
				case "wp81_rt":
				case "wp81_sl":
					f.WriteAttributeString("ToolsVersion", "12.0");
					break;
				default:
					f.WriteAttributeString("ToolsVersion", "4.0");
					break;
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
				case "net45":
					f.WriteElementString("Keyword", "ManagedCProj");
					f.WriteElementString("TargetFrameworkVersion", "v4.5");
					break;
				case "winrt80":
					f.WriteElementString("Keyword", "Win32Proj");
					f.WriteElementString("MinimumVisualStudioVersion", "11.0");
					f.WriteElementString("WindowsAppContainer", "true");
					f.WriteElementString("AppContainerApplication", "true");
					f.WriteElementString("ApplicationType", "Windows Store");
					f.WriteElementString("ApplicationTypeRevision", "8.0");
					break;
				case "winrt81":
					f.WriteElementString("Keyword", "Win32Proj");
					f.WriteElementString("MinimumVisualStudioVersion", "12.0");
					f.WriteElementString("WindowsAppContainer", "true");
					f.WriteElementString("AppContainerApplication", "true");
					f.WriteElementString("ApplicationType", "Windows Store");
					f.WriteElementString("ApplicationTypeRevision", "8.1");
					break;
				case "wp80":
					f.WriteElementString("WinMDAssembly", "true");
					f.WriteElementString("MinimumVisualStudioVersion", "11.0");
					break;
				case "wp81_rt":
					f.WriteElementString("Keyword", "Win32Proj");
					f.WriteElementString("MinimumVisualStudioVersion", "12.0");
					f.WriteElementString("AppContainerApplication", "true");
					f.WriteElementString("ApplicationType", "Windows Phone");
					f.WriteElementString("ApplicationTypeRevision", "8.1");
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
				case "net45":
					f.WriteElementString("PlatformToolset", "v110");
					f.WriteElementString("CLRSupport", "true");
					break;
				case "winrt80":
					f.WriteElementString("PlatformToolset", "v110");
					break;
				case "winrt81":
					f.WriteElementString("PlatformToolset", "v120");
					break;
				case "wp80":
					f.WriteElementString("PlatformToolset", "v110_wp80");
					break;
				case "wp81_rt":
					f.WriteElementString("PlatformToolset", "v120_wp81");
					break;
				case "wp81_sl":
					f.WriteElementString("PlatformToolset", "v120");
					break;
			}

			f.WriteEndElement(); // PropertyGroup

			switch (cfg.env)
			{
				case "net45":
					f.WriteStartElement("ItemDefinitionGroup");
					f.WriteStartElement("ClCompile");
					write_cpp_define(f, "NEED_TYPEDEFS");
					write_cpp_define(f, "WIN32");
					f.WriteEndElement(); // ClCompile
					f.WriteEndElement(); // ItemDefinitionGroup
					break;
				case "winrt80":
					f.WriteStartElement("ItemDefinitionGroup");
					f.WriteStartElement("ClCompile");
					write_cpp_define(f, "_WINRT_DLL");
					f.WriteElementString("AdditionalUsingDirectories", "$(WindowsSDK_WindowsMetaData);$(AdditionalUsingDirectories)");
					f.WriteEndElement(); // ClCompile
					f.WriteStartElement("Link");
					f.WriteElementString("AdditionalDependencies", "runtimeobject.lib;%(AdditionalDependencies)");
					f.WriteEndElement(); // Link
					f.WriteEndElement(); // ItemDefinitionGroup
					break;
				case "winrt81":
					f.WriteStartElement("ItemDefinitionGroup");
					f.WriteStartElement("ClCompile");
					write_cpp_define(f, "_WINRT_DLL");
					f.WriteElementString("AdditionalUsingDirectories", "$(WindowsSDK_WindowsMetaData);$(AdditionalUsingDirectories)");
					f.WriteEndElement(); // ClCompile
					f.WriteStartElement("Link");
					f.WriteElementString("AdditionalDependencies", "runtimeobject.lib;%(AdditionalDependencies)");
					f.WriteEndElement(); // Link
					f.WriteEndElement(); // ItemDefinitionGroup
					break;
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
				case "wp81_rt":
					f.WriteStartElement("ItemDefinitionGroup");
					f.WriteStartElement("ClCompile");
					write_cpp_define(f, "_WINRT_DLL");
					f.WriteEndElement(); // ClCompile
					f.WriteStartElement("Link");
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
			f.WriteElementString("OutDir", string.Format("$(Configuration)\\bin\\{0}\\", cfg.get_dest_subpath()));
			f.WriteElementString("IntDir", string.Format("$(Configuration)\\obj\\{0}\\", cfg.get_dest_subpath()));
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
			f.WriteElementString("AdditionalDependencies", string.Format("$(Configuration)\\bin\\{0}\\sqlite3.lib;%(AdditionalDependencies)", cfg.get_sqlite3_item().get_dest_subpath()));
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
				case "winrt80":
					break;
				case "winrt81":
					break;
				case "wp81_rt":
					break;
				case "net45":
					f.WriteStartElement("ItemGroup");
					f.WriteStartElement("Reference");
					f.WriteAttributeString("Include", "System");
					f.WriteEndElement(); // Reference
					f.WriteEndElement(); // ItemGroup
					break;
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
				case "winrt80":
					break;
				case "winrt81":
					break;
				case "wp81_rt":
					break;
				case "net45":
					break;
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

	private static void gen_pcl(config_pcl cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, cfg.get_project_filename()), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			switch (cfg.env)
			{
				case "winrt81":
				case "wp81_rt":
				case "wp81_sl":
					f.WriteAttributeString("ToolsVersion", "12.0");
					break;
				default:
					f.WriteAttributeString("ToolsVersion", "4.0");
					break;
			}
			f.WriteAttributeString("DefaultTargets", "Build");

			switch (cfg.env)
			{
				case "wp81_sl":
					break;
				default:
					f.WriteStartElement("Import");
					f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
					f.WriteAttributeString("Condition", "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')");
					f.WriteEndElement(); // Import
					break;
			}

			f.WriteStartElement("PropertyGroup");

			f.WriteElementString("ProjectGuid", cfg.guid);
			if (cfg.is_portable())
			{
				write_project_type_guids(f, GUID_PCL, GUID_CSHARP);
			}
			else
			{
				switch (cfg.env)
				{
					case "ios":
						write_project_type_guids(f, GUID_IOS, GUID_CSHARP);
						break;
					case "android":
						write_project_type_guids(f, GUID_ANDROID, GUID_CSHARP);
						break;
					case "winrt80":
					case "winrt81":
						write_project_type_guids(f, GUID_WINRT, GUID_CSHARP);
						break;
					case "wp80":
						write_project_type_guids(f, GUID_WP8, GUID_CSHARP);
						break;
					case "wp81_rt":
						write_project_type_guids(f, GUID_WP81RT, GUID_CSHARP);
						break;
					case "wp81_sl":
						write_project_type_guids(f, GUID_WP8, GUID_CSHARP);
						break;
				}
			}

			f.WriteStartElement("Configuration");
			f.WriteAttributeString("Condition", " '$(Configuration)' == '' ");

			f.WriteString("Debug");

			f.WriteEndElement(); // Configuration

			f.WriteElementString("SchemaVersion", "2.0");
			f.WriteElementString("Platform", cfg.cpu.Replace(" ", ""));
			f.WriteElementString("DefaultLanguage", "en-us");
			//f.WriteElementString("FileAlignment", "512");
			f.WriteElementString("WarningLevel", "4");
			//f.WriteElementString("PlatformTarget", cfg.cpu.Replace(" ", ""));
			f.WriteElementString("OutputType", "Library");
			f.WriteElementString("RootNamespace", "SQLitePCL");
			f.WriteElementString("AssemblyName", "SQLitePCL");

			List<string> defines = new List<string>();

			switch (cfg.env)
			{
				case "profile158":
					f.WriteElementString("TargetFrameworkVersion", "v4.0");
					break;
				case "profile78":
				case "profile259":
					f.WriteElementString("TargetFrameworkVersion", "v4.5");
					break;
				case "ios":
					defines.Add("PLATFORM_IOS");
					break;
				case "android":
					defines.Add("__MOBILE__");
					defines.Add("__ANDROID__");
					f.WriteElementString("AndroidUseLatestPlatformdk", "true");
					break;
				case "winrt80":
					//f.WriteElementString("TargetPlatformVersion", "8.0");
					f.WriteElementString("UseVSHostingProcess", "false");
					//f.WriteElementString("MinimumVisualStudioVersion", "11.0");
					// TargetFrameworkVersion?
					defines.Add("NETFX_CORE");
					break;
				case "winrt81":
					f.WriteElementString("TargetPlatformVersion", "8.1");
					f.WriteElementString("MinimumVisualStudioVersion", "12.0");
					f.WriteElementString("TargetFrameworkVersion", null);
					defines.Add("NETFX_CORE");
					break;
				case "net45":
					f.WriteElementString("ProductVersion", "12.0.0");
					f.WriteElementString("TargetFrameworkVersion", "v4.5");
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
					f.WriteElementString("NoStdLib", "true");
					f.WriteElementString("NoConfig", "true");
					break;
				case "wp81_rt":
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
					f.WriteElementString("NoStdLib", "true");
					f.WriteElementString("NoConfig", "true");
					break;
			}

			if (cfg.is_pinvoke())
			{
				switch (cfg.what)
				{
					case "internal_other":
						defines.Add("PINVOKE_FROM_INTERNAL");
						break;
					case "sqlite3":
					default:
						defines.Add("PINVOKE_FROM_SQLITE3");
						break;
				}
			}

			if (cfg.is_portable())
			{
				f.WriteElementString("TargetFrameworkProfile", cfg.env);
			}

			f.WriteEndElement(); // PropertyGroup

			write_section(cfg, f, true, defines);
			write_section(cfg, f, false, defines);

			f.WriteStartElement("ItemGroup");
			switch (cfg.env)
			{
				case "ios":
				case "android":
				case "net45":
					write_reference(f, "System");
					write_reference(f, "System.Core");
					break;
			}
			switch (cfg.env)
			{
				case "ios":
					write_reference(f, "monotouch");
					break;
				case "android":
					write_reference(f, "Mono.Android");
					break;
			}
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
				write_cs_compile(f, root, "src\\cs\\sqlite3_pinvoke.cs");
				write_cs_compile(f, root, "src\\cs\\util.cs");
				f.WriteEndElement(); // ItemGroup
			}

			if (cfg.is_cppinterop())
			{
				f.WriteStartElement("ItemGroup");
				f.WriteStartElement("ProjectReference");
				{
					config_cppinterop other = cfg.get_cppinterop_item();
					f.WriteAttributeString("Include", other.get_project_filename());
					f.WriteElementString("Project", other.guid);
					f.WriteElementString("Name", other.get_name());
					//f.WriteElementString("Private", "true");
				}
				f.WriteEndElement(); // ProjectReference
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
				switch (cfg.env)
				{
					case "winrt80":
						f.WriteStartElement("PropertyGroup");
						f.WriteAttributeString("Condition", " '$(VisualStudioVersion)' == '' or '$(VisualStudioVersion)' < '11.0' ");
						f.WriteElementString("VisualStudioVersion", "11.0");
						f.WriteEndElement(); // PropertyGroup
						break;
					case "winrt81":
						f.WriteStartElement("PropertyGroup");
						f.WriteAttributeString("Condition", " '$(VisualStudioVersion)' == '' or '$(VisualStudioVersion)' < '12.0' ");
						f.WriteElementString("VisualStudioVersion", "12.0");
						f.WriteEndElement(); // PropertyGroup
						break;
					case "wp81_rt":
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

				switch (cfg.env)
				{
					case "ios":
						f.WriteStartElement("Import");
						f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.MonoTouch.CSharp.targets");
						f.WriteEndElement(); // Import
						break;
					case "android":
						f.WriteStartElement("Import");
						f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Novell\\Novell.MonoDroid.CSharp.targets");
						f.WriteEndElement(); // Import
						break;
					case "winrt80":
					case "winrt81":
					case "wp81_rt":
						f.WriteStartElement("Import");
						f.WriteAttributeString("Project", "$(MSBuildExtensionsPath)\\Microsoft\\WindowsXaml\\v$(VisualStudioVersion)\\Microsoft.Windows.UI.Xaml.CSharp.targets");
						f.WriteEndElement(); // Import
						break;
					case "net45":
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
			f.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			f.WriteLine("# Visual Studio 2013");
			f.WriteLine("VisualStudioVersion = 12.0");
			f.WriteLine("MinimumVisualStudioVersion = 12.0");

			// solution folders

			string folder_sqlite3 = write_folder(f, "sqlite3");
			string folder_cppinterop = write_folder(f, "cppinterop");
			string folder_platforms = write_folder(f, "platforms");
			string folder_portable = write_folder(f, "portable");

			foreach (config_sqlite3 cfg in projects.items_sqlite3)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
						GUID_CPP,
						cfg.get_name(),
						cfg.get_project_filename(),
						cfg.guid
						);
				f.WriteLine("EndProject");
			}

			foreach (config_cppinterop cfg in projects.items_cppinterop)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
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
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
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

			f.WriteLine("Global");

			f.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			f.WriteLine("\t\tDebug|Mixed Platforms = Debug|Mixed Platforms");
			f.WriteLine("\t\tRelease|Mixed Platforms = Release|Mixed Platforms");
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
			foreach (config_sqlite3 cfg in projects.items_sqlite3)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.cpu);
			}
			foreach (config_cppinterop cfg in projects.items_cppinterop)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.cpu);
			}
			foreach (config_pcl cfg in projects.items_pcl)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.cpu);
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
			f.WriteLine("\tEndGlobalSection");

			f.WriteLine("EndGlobal");
		}
	}

	private static void write_nuspec_file_entry(config_pcl cfg, XmlWriter f, string where, bool needy)
	{
		f.WriteComment(string.Format("{0}", cfg.get_name()));
		var a = new List<string>();
		cfg.get_products(a, needy);

		foreach (string s in a)
		{
			f.WriteStartElement("file");
			f.WriteAttributeString("src", string.Format("release\\bin\\{0}", s));
			f.WriteAttributeString("target", cfg.get_nuget_target_path(where));
			f.WriteEndElement(); // file
		}
	}

	private static void write_nuspec_file_entries(XmlWriter f, string where, List<config_pcl> a, bool needy)
	{
		foreach (config_pcl cfg in a)
		{
			write_nuspec_file_entry(cfg, f, where, needy);
		}
	}

	private static void gen_nuspec_basic(string top, bool needy)
	{
		string id;

		if (needy)
		{
			id = "SQLitePCL.raw_needy";
		}
		else
		{
			id = "SQLitePCL.raw_basic";
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
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.1 for the WP 8.1 stuff?

			f.WriteElementString("id", id);
			f.WriteElementString("version", "0.2.0-alpha");
			if (needy)
			{
				f.WriteElementString("title", "SQLitePCL.raw - no SQLite instances included");
				f.WriteElementString("description", "This NuGet package for SQLitePCL.raw is 'needy' in the sense that it includes no instances of the SQLite library itself.  For iOS and Android, the mobile OS includes an instance of the SQLite library, which is probably somewhat outdated, but suitable for most applications.  For all of the Windows platforms, use of this package means that an appropriate instance of sqlite3.dll needs to be provided separately.");
			}
			else
			{
				f.WriteElementString("title", "SQLitePCL.raw - basic use cases");
				f.WriteElementString("description", "This NuGet package for SQLitePCL.raw is 'basic' in the sense that it includes the configurations that are likely to Just Work for most use cases.  Specifically, for iOS and Android, this package uses the SQLite library which is built-in to the mobile OS.  For all of the Windows platforms, this package bundles an instance of the SQLite library.");
			}
			f.WriteElementString("authors", "Eric Sink, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			f.WriteElementString("licenseUrl", "https://raw.github.com/ericsink/SQLitePCL.raw/master/LICENSE.TXT");
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			//f.WriteElementString("releaseNotes", "TODO");
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			// write all the bait

			f.WriteComment("bait");
			foreach (config_pcl cfg in projects.items_pcl)
			{
				if (cfg.is_portable())
				{
					write_nuspec_file_entry(
							cfg, 
							f, 
							"lib", 
							false // unused flag
							);
				}
			}

			f.WriteComment("special case, platform assemblies for ios go in the lib directory");
			write_nuspec_file_entries(f, "lib",
					projects.find_pcls(
						"ios",
						"pinvoke",
						"sqlite3",
						"anycpu",
						null
						),
						true // TODO
					);


			f.WriteComment("special case, platform assemblies for android go in the lib directory");
			write_nuspec_file_entries(f, "lib",
					projects.find_pcls(
						"android",
						"pinvoke",
						"sqlite3",
						"anycpu",
						null
						)
						,true // TODO
					);

			// all the other envs go into build

			Dictionary<string, string> pcl_env = new Dictionary<string, string>();
			pcl_env["net45"] = null;
			pcl_env["winrt80"] = null;
			pcl_env["winrt81"] = null;
			pcl_env["wp80"] = null;
			pcl_env["wp81_rt"] = null;
			pcl_env["wp81_sl"] = null;

			// TODO remove this directory first?
			Directory.CreateDirectory(Path.Combine(top, "empty"));

			foreach (string env in pcl_env.Keys)
			{
				f.WriteComment(string.Format("platform assemblies for {0}", env));

				List<config_pcl> a = projects.find_pcls(
							env,
							"cppinterop",
							"sqlite3",
							null,
							"dynamic"
							);

				write_nuspec_file_entries(
						f, 
						"build", 
						a, 
						needy
						);

				f.WriteComment("empty directory in lib to avoid nuget adding a reference to the bait");

				Directory.CreateDirectory(Path.Combine(Path.Combine(top, "empty"), config_pcl.get_nuget_framework_name(env)));

				f.WriteStartElement("file");
				f.WriteAttributeString("src", string.Format("empty\\{0}\\", config_pcl.get_nuget_framework_name(env)));
				f.WriteAttributeString("target", string.Format("lib\\{0}", config_pcl.get_nuget_framework_name(env)));
				f.WriteEndElement(); // file

				f.WriteComment("msbuild .targets file to inject reference for the right cpu");

				string tname = string.Format("{0}_{1}.targets", needy?"needy":"regular", env);
				gen_nuget_targets(top, tname, needy, a);

				f.WriteStartElement("file");
				f.WriteAttributeString("src", tname);
				f.WriteAttributeString("target", string.Format("build\\{0}\\{1}.targets", config_pcl.get_nuget_framework_name(env), id));
				f.WriteEndElement(); // file
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuget_targets(string top, string tname, bool needy, List<config_pcl> a)
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
		foreach (string cpu in cpus.Keys)
		{
			if (cond.Length > 0)
			{
				cond += " AND ";
			}

			cond += string.Format("($(Platform.ToLower()) != '{0}')", cpu);
		}

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", "check_cpu");
			f.WriteAttributeString("BeforeTargets", "Build");
			f.WriteAttributeString("Condition", string.Format(" ( {0} ) ", cond));
			f.WriteStartElement("Error");
			f.WriteAttributeString("Text", "Unsupported: $(Platform)");
			f.WriteEndElement(); // Error
			f.WriteEndElement(); // Target

			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", "InjectReference");
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			foreach (config_pcl cfg in a)
			{
				bool b_platform_condition = true;

				switch (cfg.env)
				{
					case "ios":
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
				f.WriteAttributeString("Include", "SQLitePCL");

				f.WriteElementString("HintPath", string.Format("$(MSBuildThisFileDirectory){0}", Path.Combine(cfg.get_nuget_target_subpath(), "SQLitePCL.dll")));

				// TODO private?

				// TODO name?

				f.WriteEndElement(); // Reference

				// TODO make this optional, for needy package
				if (cfg.dll)
				{
					f.WriteStartElement("Content");
					f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory){0}", Path.Combine(cfg.get_nuget_target_subpath(), "sqlite3.dll")));
					// TODO link
					f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
					f.WriteEndElement(); // Content

					// TODO SDKReference, for all the RT-flavor platforms
				}
				// TODO might need SDKReference even for !cfg.dll

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

		string root = ".."; // assumes that gen_build.exe is being run from the root directory of the project
		string top = "bld";

		// --------------------------------
		// create the build directory

		Directory.CreateDirectory(top);

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

		// --------------------------------

		gen_solution(top);

		// --------------------------------

		gen_nuspec_basic(top, false);
		gen_nuspec_basic(top, true);

	}
}

