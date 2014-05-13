
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

internal class sqlite3_config
{
	public string env;
	public string cpu;
	public string guid;

	public string get_name()
	{
		return string.Format("sqlite3.{0}.{1}", this.env, this.cpu);
	}

	public string get_filename()
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

internal class cppinterop_config
{
	public string env;
	public string cpu;
	public string guid;

	public string get_name()
	{
		return string.Format("cppinterop.{0}.{1}", this.env, this.cpu);
	}

	public string get_filename()
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

	public string sqlite3_env()
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

internal class pcl_config
{
	public string env;
	public string api;
	public string sqlite;
	public string cpu;
	public string guid;

	public bool is_portable()
	{
		if (env.Length <= 7)
		{
			return false;
		}

		return "profile" == env.Substring(0,7);
	}

	public string get_name()
	{
		if (is_portable())
		{
			return string.Format("portable.{0}", this.env);
		}
		else
		{
			return string.Format("platform.{0}.{1}.{2}.{3}", this.env, this.api, this.sqlite, this.cpu);
		}
	}

	public string get_filename()
	{
		return string.Format("{0}.csproj", get_name());
	}
}

public class gen
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

	private static sqlite3_config[] sqlite3_items =
	{
		new sqlite3_config { env="winxp", cpu="x86" },
		new sqlite3_config { env="winxp", cpu="x64" },

		new sqlite3_config { env="winrt80", cpu="arm" },
		new sqlite3_config { env="winrt80", cpu="x64" },
		new sqlite3_config { env="winrt80", cpu="x86" },

		new sqlite3_config { env="winrt81", cpu="arm" },
		new sqlite3_config { env="winrt81", cpu="x64" },
		new sqlite3_config { env="winrt81", cpu="x86" },

		new sqlite3_config { env="wp80", cpu="arm" },
		new sqlite3_config { env="wp80", cpu="x86" },

		new sqlite3_config { env="wp81_rt", cpu="arm" },
		new sqlite3_config { env="wp81_rt", cpu="x86" },

		new sqlite3_config { env="wp81_sl", cpu="arm" },
		new sqlite3_config { env="wp81_sl", cpu="x86" },

	};

	private static cppinterop_config[] cppinterop_items =
	{
		new cppinterop_config { env="net45", cpu="x64" },
		new cppinterop_config { env="net45", cpu="x86" },

		new cppinterop_config { env="winrt80", cpu="arm" },
		new cppinterop_config { env="winrt80", cpu="x64" },
		new cppinterop_config { env="winrt80", cpu="x86" },

		new cppinterop_config { env="winrt81", cpu="arm" },
		new cppinterop_config { env="winrt81", cpu="x64" },
		new cppinterop_config { env="winrt81", cpu="x86" },

		new cppinterop_config { env="wp80", cpu="arm" },
		new cppinterop_config { env="wp80", cpu="x86" },

		new cppinterop_config { env="wp81_rt", cpu="arm" },
		new cppinterop_config { env="wp81_rt", cpu="x86" },

		new cppinterop_config { env="wp81_sl", cpu="arm" },
		new cppinterop_config { env="wp81_sl", cpu="x86" },

	};

	private static pcl_config[] pcl_items = 
	{
		new pcl_config { env="profile78", cpu="anycpu" },
		new pcl_config { env="profile259", cpu="anycpu" },
		//new pcl_config { env="profile158", cpu="anycpu" },

		new pcl_config { env="android", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="anycpu"},
		new pcl_config { env="ios", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="anycpu"},
		new pcl_config { env="ios", api="pinvoke", sqlite="internal_other", cpu="anycpu"},

		new pcl_config { env="net45", api="cppinterop", sqlite="bundled_sqlite3", cpu="x86"},
		new pcl_config { env="net45", api="cppinterop", sqlite="bundled_sqlite3", cpu="x64"},

		new pcl_config { env="net45", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="anycpu"},
		new pcl_config { env="net45", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="x86"},
		new pcl_config { env="net45", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="x64"},

		new pcl_config { env="winrt80", api="cppinterop", sqlite="bundled_sqlite3", cpu="arm"},
		new pcl_config { env="winrt80", api="cppinterop", sqlite="bundled_sqlite3", cpu="x64"},
		new pcl_config { env="winrt80", api="cppinterop", sqlite="bundled_sqlite3", cpu="x86"},

		new pcl_config { env="winrt80", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="anycpu"},
		new pcl_config { env="winrt80", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="arm"},
		new pcl_config { env="winrt80", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="x64"},
		new pcl_config { env="winrt80", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="x86"},

		new pcl_config { env="winrt81", api="cppinterop", sqlite="bundled_sqlite3", cpu="arm"},
		new pcl_config { env="winrt81", api="cppinterop", sqlite="bundled_sqlite3", cpu="x64"},
		new pcl_config { env="winrt81", api="cppinterop", sqlite="bundled_sqlite3", cpu="x86"},

		new pcl_config { env="winrt81", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="anycpu"},
		new pcl_config { env="winrt81", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="arm"},
		new pcl_config { env="winrt81", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="x64"},
		new pcl_config { env="winrt81", api="pinvoke", sqlite="sharedlib_sqlite3", cpu="x86"},

		new pcl_config { env="wp80", api="cppinterop", sqlite="bundled_sqlite3", cpu="arm"},
		new pcl_config { env="wp80", api="cppinterop", sqlite="bundled_sqlite3", cpu="x86"},

		new pcl_config { env="wp81_rt", api="cppinterop", sqlite="bundled_sqlite3", cpu="arm"},
		new pcl_config { env="wp81_rt", api="cppinterop", sqlite="bundled_sqlite3", cpu="x86"},

		new pcl_config { env="wp81_sl", api="cppinterop", sqlite="bundled_sqlite3", cpu="arm"},
		new pcl_config { env="wp81_sl", api="cppinterop", sqlite="bundled_sqlite3", cpu="x86"},
	};

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

	private static void write_section(pcl_config cfg, XmlWriter f, bool debug, List<string> defines)
	{
		string name = debug ? "debug" : "release";
		f.WriteStartElement("PropertyGroup");
		f.WriteAttributeString("Condition", string.Format(" '$(Configuration)|$(Platform)' == '{0}|{1}' ", name, cfg.cpu));

		if (cfg.is_portable())
		{
			f.WriteElementString("OutputPath", string.Format("{0}\\bin\\pcl\\{1}\\", name, cfg.env));
			f.WriteElementString("IntermediateOutputPath", string.Format("{0}\\obj\\pcl\\{1}\\", name, cfg.env));
		}
		else
		{
			f.WriteElementString("OutputPath", string.Format("{0}\\bin\\pcl\\{1}\\{2}\\{3}\\{4}\\", name, cfg.env, cfg.api, cfg.sqlite, cfg.cpu));
			f.WriteElementString("IntermediateOutputPath", string.Format("{0}\\obj\\pcl\\{1}\\{2}\\{3}\\{4}\\", name, cfg.env, cfg.api, cfg.sqlite, cfg.cpu));
		}

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
		f.WriteElementString("DebugType", debug ? "full" : "pdbonly");

		f.WriteEndElement(); // PropertyGroup
	}

	private static void gen_sqlite3(sqlite3_config cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, cfg.get_filename()), settings))
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
			f.WriteElementString("ConfigurationType", "StaticLibrary");
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
			f.WriteElementString("OutDir", string.Format("$(Configuration)\\bin\\sqlite3\\{0}\\{1}\\", cfg.env, cfg.cpu));
			f.WriteElementString("IntDir", string.Format("$(Configuration)\\obj\\sqlite3\\{0}\\{1}\\", cfg.env, cfg.cpu));
			f.WriteEndElement(); // PropertyGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteStartElement("ClCompile");
			write_cpp_define(f, "_LIB");
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
			f.WriteElementString("GenerateDebugInformation", "true");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteAttributeString("Condition", string.Format("'$(Configuration)'=='{0}' ", "Debug"));
			f.WriteStartElement("ClCompile");
			f.WriteElementString("Optimization", "Disabled");
			write_cpp_define(f, "_DEBUG");
			f.WriteEndElement(); // ClCompile
			f.WriteStartElement("Link");
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
			f.WriteElementString("EnableCOMDATFolding", "true");
			f.WriteElementString("OptimizeReferences", "true");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemGroup");
			{
				string path = Path.Combine(root, "sqlite3\\src\\sqlite3.c");
				f.WriteStartElement("ClCompile");
				f.WriteAttributeString("Include", path);
				f.WriteEndElement(); // Compile
			}
			f.WriteEndElement(); // ItemGroup

			f.WriteStartElement("Import");
			f.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.targets");
			f.WriteEndElement(); // Import

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static sqlite3_config find_sqlite3(string env, string cpu)
	{
		foreach (sqlite3_config cfg in sqlite3_items)
		{
			if (
					(cfg.env == env)
					&& (cfg.cpu == cpu)
			   )
			{
				return cfg;
			}
		}
		throw new Exception();
	}

	private static cppinterop_config find_cppinterop(string env, string cpu)
	{
		foreach (cppinterop_config cfg in cppinterop_items)
		{
			if (
					(cfg.env == env)
					&& (cfg.cpu == cpu)
			   )
			{
				return cfg;
			}
		}
		throw new Exception();
	}

	private static void gen_cppinterop(cppinterop_config cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, cfg.get_filename()), settings))
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
			f.WriteElementString("OutDir", string.Format("$(Configuration)\\bin\\cppinterop\\{0}\\{1}\\", cfg.env, cfg.cpu));
			f.WriteElementString("IntDir", string.Format("$(Configuration)\\obj\\cppinterop\\{0}\\{1}\\", cfg.env, cfg.cpu));
			write_cpp_includepath(f, root, "sqlite3\\src\\");
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
			//f.WriteElementString("GenerateWindowsMetadata", "false");
			f.WriteElementString("GenerateDebugInformation", "true");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemDefinitionGroup");
			f.WriteAttributeString("Condition", string.Format("'$(Configuration)'=='{0}' ", "Debug"));
			f.WriteStartElement("ClCompile");
			f.WriteElementString("Optimization", "Disabled");
			write_cpp_define(f, "_DEBUG");
			f.WriteEndElement(); // ClCompile
			f.WriteStartElement("Link");
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
			//f.WriteElementString("EnableCOMDATFolding", "true");
			//f.WriteElementString("OptimizeReferences", "true");
			f.WriteEndElement(); // Link
			f.WriteEndElement(); // ItemDefinitionGroup

			f.WriteStartElement("ItemGroup");
			f.WriteStartElement("ClCompile");
			f.WriteAttributeString("Include", Path.Combine(root, "cppinterop\\src\\sqlite3_cx.cpp"));
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

			f.WriteStartElement("ItemGroup");
			f.WriteStartElement("ProjectReference");
			{
				sqlite3_config other = find_sqlite3(cfg.sqlite3_env(), cfg.cpu);
				f.WriteAttributeString("Include", other.get_filename());
				f.WriteElementString("Project", other.guid);
			}
			f.WriteEndElement(); // ProjectReference
			f.WriteEndElement(); // ItemGroup

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

	private static void gen_pcl(pcl_config cfg, string root, string top)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, cfg.get_filename()), settings))
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
			f.WriteElementString("PlatformTarget", cfg.cpu.Replace(" ", ""));
			f.WriteElementString("OutputType", "Library");
			f.WriteElementString("RootNamespace", "SQLitePCL");
			f.WriteElementString("AssemblyName", "SQLitePCL");

			// TODO ProductVersion 12.0.0

			List<string> defines = new List<string>();

			if (cfg.is_portable())
			{
				f.WriteElementString("TargetFrameworkVersion", "v4.5");
			}
			else
			{
				switch (cfg.env)
				{
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
						f.WriteElementString("SilverlightVersion", "v8.0");
						f.WriteElementString("SilverlightApplication", "false");
						f.WriteElementString("ValidateXaml", "true");
						f.WriteElementString("ThrowErrorsInValidation", "true");
						defines.Add("WINDOWS_PHONE");
						defines.Add("SILVERLIGHT");
						f.WriteElementString("NoStdLib", "true");
						f.WriteElementString("NoConfig", "true");
						break;
				}
			}

			if (cfg.api == "pinvoke")
			{
				switch (cfg.sqlite)
				{
					case "sharedlib_sqlite3":
						defines.Add("PINVOKE_FROM_SQLITE3");
						break;
					case "internal_other":
						defines.Add("PINVOKE_FROM_INTERNAL");
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
			write_cs_compile(f, root, "pcl\\src\\AssemblyInfo.cs");
			write_cs_compile(f, root, "pcl\\src\\raw.cs");
			write_cs_compile(f, root, "pcl\\src\\intptrs.cs");
			write_cs_compile(f, root, "pcl\\src\\isqlite3.cs");
			f.WriteEndElement(); // ItemGroup

			if (cfg.is_portable())
			{
				f.WriteStartElement("ItemGroup");
				write_cs_compile(f, root, "pcl\\src\\sqlite3_bait.cs");
				f.WriteEndElement(); // ItemGroup
			}
			else
			{
				if (cfg.api == "cppinterop")
				{
					f.WriteStartElement("ItemGroup");
					write_cs_compile(f, root, "pcl\\src\\sqlite3_cppinterop.cs");
					write_cs_compile(f, root, "pcl\\src\\util.cs");
					f.WriteEndElement(); // ItemGroup
				}
				else if (cfg.api == "pinvoke")
				{
					f.WriteStartElement("ItemGroup");
					write_cs_compile(f, root, "pcl\\src\\sqlite3_pinvoke.cs");
					write_cs_compile(f, root, "pcl\\src\\util.cs");
					f.WriteEndElement(); // ItemGroup
				}
			}

			if (cfg.api == "cppinterop")
			{
				f.WriteStartElement("ItemGroup");
				f.WriteStartElement("ProjectReference");
				{
					cppinterop_config other = find_cppinterop(cfg.env, cfg.cpu);
					f.WriteAttributeString("Include", other.get_filename());
					f.WriteElementString("Project", other.guid);
					f.WriteElementString("Name", other.get_name());
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
						f.WriteAttributeString("Project", "$(MSBuildBinPath)\\Microsoft.CSharp.targets");
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

			foreach (sqlite3_config cfg in sqlite3_items)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
						GUID_CPP,
						cfg.get_name(),
						cfg.get_filename(),
						cfg.guid
						);
				f.WriteLine("EndProject");
			}

			foreach (cppinterop_config cfg in cppinterop_items)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
						GUID_CPP,
						cfg.get_name(),
						cfg.get_filename(),
						cfg.guid
						);
				f.WriteLine("EndProject");
			}

			foreach (pcl_config cfg in pcl_items)
			{
				f.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
						GUID_CSHARP,
						cfg.get_name(),
						cfg.get_filename(),
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
			foreach (sqlite3_config cfg in sqlite3_items)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.cpu);
			}
			foreach (cppinterop_config cfg in cppinterop_items)
			{
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.ActiveCfg = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Debug|Mixed Platforms.Build.0 = Debug|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.ActiveCfg = Release|{1}", cfg.guid, cfg.cpu);
				f.WriteLine("\t\t{0}.Release|Mixed Platforms.Build.0 = Release|{1}", cfg.guid, cfg.cpu);
			}
			foreach (pcl_config cfg in pcl_items)
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
			foreach (sqlite3_config cfg in sqlite3_items)
			{
				f.WriteLine("\t\t{0} = {1}", cfg.guid, folder_sqlite3);
			}
			foreach (cppinterop_config cfg in cppinterop_items)
			{
				f.WriteLine("\t\t{0} = {1}", cfg.guid, folder_cppinterop);
			}
			foreach (pcl_config cfg in pcl_items)
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

	public static void Main(string[] args)
	{
		string root = ".."; // assumes that gen_build.exe is being run from the root directory of the project
		string top = "bld";

		Directory.CreateDirectory(top);

		// assign all the guids

		foreach (sqlite3_config cfg in sqlite3_items)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		foreach (cppinterop_config cfg in cppinterop_items)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}


		foreach (pcl_config cfg in pcl_items)
		{
			cfg.guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
		}

		foreach (sqlite3_config cfg in sqlite3_items)
		{
			gen_sqlite3(cfg, root, top);
		}

		foreach (cppinterop_config cfg in cppinterop_items)
		{
			gen_cppinterop(cfg, root, top);
		}

		foreach (pcl_config cfg in pcl_items)
		{
			gen_pcl(cfg, root, top);
		}

		gen_solution(top);

		// TODO write out the nuspec file
	}
}

