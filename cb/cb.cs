
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class cb
{
    enum VCVersion
    {
        v110,
        v120,
        v140,
    }

    enum Machine
    {
        x86,
        x64,
        arm,
    }

    enum Flavor
    {
        plain,
        appcontainer,
        xp,
        wp80,
        wp81,
    }

    static string get_crt_option(VCVersion v, Flavor f)
    {
        switch (f)
        {
            case Flavor.wp80:
            case Flavor.wp81:
                return "/MD";
            case Flavor.appcontainer:
                if (v == VCVersion.v120)
                {
                    return "/MD";
                }
                else
                {
                    return "/MT";
                }
            default:
                return "/MT";
        }
    }

    static string get_vcvarsbat(VCVersion v, Flavor f)
    {
        if (f == Flavor.wp80)
        {
            if (v == VCVersion.v110)
            {
                return "C:\\Program Files (x86)\\Microsoft Visual Studio 11.0\\VC\\WPSDK\\WP80\\vcvarsphoneall.bat";
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        switch (v)
        {
            case VCVersion.v110:
                return "C:\\Program Files (x86)\\Microsoft Visual Studio 11.0\\VC\\vcvarsall.bat";
            case VCVersion.v120:
                return "C:\\Program Files (x86)\\Microsoft Visual Studio 12.0\\VC\\vcvarsall.bat";
            case VCVersion.v140:
                return "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat";
            default:
                throw new NotImplementedException();
        }
    }

    static string get_toolchain(Machine m)
    {
        switch (m)
        {
            case Machine.x86:
                return "x86";
            case Machine.x64:
                return "x86_amd64";
            case Machine.arm:
                return "x86_arm";
            default:
                throw new NotImplementedException();
        }
    }

    static void write_bat(
        string libname,
        trio t,
        IList<string> cfiles,
        Dictionary<string,string> defines,
        IList<string> includes
        )
    {
        var vcversion = t.v;
        var flavor = t.f;
        var machine = t.m;

        var vcvarsbat = get_vcvarsbat(vcversion, flavor);
        var toolchain = get_toolchain(machine);
        var crt_option = get_crt_option(vcversion, flavor);
        var subdir = t.subdir(libname);
        var dest = t.bat(libname);
		using (TextWriter tw = new StreamWriter(dest))
        {
            tw.WriteLine("@echo on");
            tw.WriteLine("SET VCVARSBAT=\"{0}\"", vcvarsbat);
            tw.WriteLine("SET TOOLCHAIN={0}", toolchain);
            tw.WriteLine("SET SUBDIR={0}", subdir);
            tw.WriteLine("call %VCVARSBAT% %TOOLCHAIN%");
            tw.WriteLine("@echo on");
            tw.WriteLine("mkdir .\\obj\\%SUBDIR%");
            tw.WriteLine("mkdir .\\bin\\%SUBDIR%");
            foreach (var s in cfiles)
            {
                tw.Write("CL.exe");
                tw.Write(" /nologo");
                tw.Write(" /c");
                //tw.Write(" /Zi");
                tw.Write(" /W1");
                tw.Write(" /WX-");
                tw.Write(" /sdl-");
                tw.Write(" /O2");
                tw.Write(" /Oi");
                tw.Write(" /Oy-");
                foreach (var d in defines.Keys.OrderBy(q => q))
                {
                    var v = defines[d];
                    tw.Write(" /D {0}", d);
                    if (v != null)
                    {
                        tw.Write("={0}", v);
                    }
                }
                if (machine == Machine.arm)
                {
                    tw.Write(" /D _ARM_WINAPI_PARTITION_DESKTOP_SDK_AVAILABLE=1");
                }
                switch (flavor)
                {
                    case Flavor.wp80:
                    case Flavor.wp81:
                        tw.Write(" /D WINAPI_FAMILY=WINAPI_FAMILY_PHONE_APP");
                        tw.Write(" /D SQLITE_OS_WINRT");
                        break;
                }
                if (flavor == Flavor.xp)
                {
                    tw.Write(" /D _USING_V110_SDK71_");
                }
                tw.Write(" /D NDEBUG");
                tw.Write(" /D _USRDLL");
                tw.Write(" /D _WINDLL");
                tw.Write(" /Gm-");
                tw.Write(" /EHsc");
                tw.Write(" {0}", crt_option);
                tw.Write(" /GS");
                tw.Write(" /Gy");
                tw.Write(" /fp:precise");
                tw.Write(" /Zc:wchar_t");
                tw.Write(" /Zc:forScope");
                tw.Write(" /Fo\".\\obj\\%SUBDIR%\\\\\"");
                tw.Write(" /Gd");
                tw.Write(" /TC");
                tw.Write(" /analyze-");
                foreach (var p in includes)
                {
                    tw.Write(" /I{0}", p);
                }
                tw.WriteLine(" {0}", s);
            }
            tw.Write("link.exe");
            tw.Write(" /nologo");
            tw.Write(" /OUT:\"bin\\%SUBDIR%\\{0}.dll\"", libname);
            if (flavor == Flavor.xp)
            {
                switch (machine)
                {
                    case Machine.x86:
                        tw.Write(" /SUBSYSTEM:CONSOLE,\"5.01\"");
                        break;
                    case Machine.x64:
                        tw.Write(" /SUBSYSTEM:CONSOLE,\"5.02\"");
                        break;
                    case Machine.arm:
                        tw.Write(" /SUBSYSTEM:CONSOLE,\"6.02\"");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                tw.Write(" /SUBSYSTEM:CONSOLE");
            }
            tw.Write(" /OPT:REF");
            tw.Write(" /OPT:ICF");
            tw.Write(" /TLBID:1");
            tw.Write(" /WINMD:NO");
            tw.Write(" /DYNAMICBASE");
            tw.Write(" /NXCOMPAT");
            tw.Write(" /MACHINE:{0}", machine.ToString().ToUpper());
            tw.Write(" /DLL");
            switch (flavor)
            {
                case Flavor.appcontainer:
                case Flavor.wp81:
                    tw.Write(" /APPCONTAINER");
                    break;
                default:
                    break;
            }
            if (flavor != Flavor.wp80)
            {
                tw.Write(" /MANIFEST /MANIFESTUAC:\"level='asInvoker' uiAccess='false'\" /manifest:embed");
            }
            if (flavor == Flavor.wp80)
            {
                tw.Write(" WindowsPhoneCore.lib RuntimeObject.lib PhoneAppModelHost.lib");
            }
            foreach (var s in cfiles)
            {
                var b = Path.GetFileNameWithoutExtension(s);
                tw.Write(" obj\\%SUBDIR%\\{0}.obj", b);
            }
            tw.WriteLine();
        }
    }

    class trio
    {
        public VCVersion v { get; private set; }
        public Flavor f { get; private set; }
        public Machine m { get; private set; }

        public trio(VCVersion av, Flavor af, Machine am)
        {
            v = av;
            f = af;
            m = am;
        }

        public string bat(string libname)
        {
            var dest = string.Format("{0}_{1}_{2}_{3}.bat", libname, v, f, m);
            return dest;
        }
        public string subdir(string libname)
        {
            var s = string.Format("{0}\\{1}\\{2}\\{3}", libname, v, f, m);
            return s;
        }
    }

    static void write_multibat(
        string libname,
        IList<trio> trios,
        IList<string> cfiles,
        Dictionary<string,string> defines,
        IList<string> includes
        )
    {
        foreach (var t in trios)
        {
            write_bat(
                libname,
                t,
                cfiles,
                defines,
                includes
                );
        }

		using (TextWriter tw = new StreamWriter(string.Format("{0}.bat", libname)))
        {
            foreach (var t in trios)
            {
                tw.WriteLine("cmd /c {0}", t.bat(libname));
            }
        }
    }

    public static void Main()
    {
        var cfiles = new string[]
        {
            "..\\sqlite3\\sqlite3.c",
        };
        var defines = new Dictionary<string,string>
        {
            { "SQLITE_ENABLE_COLUMN_METADATA", null },
            { "SQLITE_ENABLE_FTS4", null },
            { "SQLITE_ENABLE_JSON1", null },
            { "SQLITE_ENABLE_RTREE", null },
            { "SQLITE_DEFAULT_FOREIGN_KEYS", "1" },
            { "SQLITE_API", "__declspec(dllexport)" },
            { "SQLITE_WIN32_FILEMAPPING_API", "1" },
        };
        var includes = new string[]
        {
        };

        var trios = new trio[]
        {
            new trio(VCVersion.v110, Flavor.wp80, Machine.x86),
            new trio(VCVersion.v110, Flavor.wp80, Machine.arm),

            new trio(VCVersion.v120, Flavor.wp81, Machine.x86),
            new trio(VCVersion.v120, Flavor.wp81, Machine.arm),

            new trio(VCVersion.v110, Flavor.xp, Machine.x86),
            new trio(VCVersion.v110, Flavor.xp, Machine.x64),
            new trio(VCVersion.v110, Flavor.xp, Machine.arm),

            new trio(VCVersion.v110, Flavor.plain, Machine.x86),
            new trio(VCVersion.v110, Flavor.plain, Machine.x64),
            new trio(VCVersion.v110, Flavor.plain, Machine.arm),

            new trio(VCVersion.v110, Flavor.appcontainer, Machine.x86),
            new trio(VCVersion.v110, Flavor.appcontainer, Machine.x64),
            new trio(VCVersion.v110, Flavor.appcontainer, Machine.arm),

#if not
            new trio(VCVersion.v120, Flavor.plain, Machine.x86),
            new trio(VCVersion.v120, Flavor.plain, Machine.x64),
            new trio(VCVersion.v120, Flavor.plain, Machine.arm),
#endif

            new trio(VCVersion.v120, Flavor.appcontainer, Machine.x86),
            new trio(VCVersion.v120, Flavor.appcontainer, Machine.x64),
            new trio(VCVersion.v120, Flavor.appcontainer, Machine.arm),

            new trio(VCVersion.v140, Flavor.plain, Machine.x86),
            new trio(VCVersion.v140, Flavor.plain, Machine.x64),
            new trio(VCVersion.v140, Flavor.plain, Machine.arm),

            new trio(VCVersion.v140, Flavor.appcontainer, Machine.x86),
            new trio(VCVersion.v140, Flavor.appcontainer, Machine.x64),
            new trio(VCVersion.v140, Flavor.appcontainer, Machine.arm),

        };

        write_multibat(
            "e_sqlite3",
            trios,
            cfiles,
            defines,
            includes
            );

    }
}

