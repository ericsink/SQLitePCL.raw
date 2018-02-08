
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
			    return "/MD";
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
        IList<string> includes,
        IList<string> libs
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
		    if ((flavor == Flavor.appcontainer) && (vcversion == VCVersion.v140))
		    {
				tw.WriteLine("call %VCVARSBAT% %TOOLCHAIN% store");
		    }
			else
		    {
				tw.WriteLine("call %VCVARSBAT% %TOOLCHAIN%");
		    }
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
                        tw.Write(" /D MBEDTLS_NO_PLATFORM_ENTROPY");
                        break;
					case Flavor.appcontainer:
                        tw.Write(" /D WINAPI_FAMILY=WINAPI_FAMILY_APP");
                        tw.Write(" /D __WRL_NO_DEFAULT_LIB__");
                        tw.Write(" /D SQLITE_OS_WINRT");
                        tw.Write(" /D MBEDTLS_NO_PLATFORM_ENTROPY");
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
                tw.Write(" /Zc:inline");
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
            if (flavor == Flavor.wp80)
            {
                tw.Write(" WindowsPhoneCore.lib RuntimeObject.lib PhoneAppModelHost.lib");
            }
			else if (flavor == Flavor.appcontainer)
			{
                tw.Write(" /MANIFEST:NO");
			}
			else
            {
                tw.Write(" /MANIFEST /MANIFESTUAC:\"level='asInvoker' uiAccess='false'\" /manifest:embed");
            }
		    if ((flavor == Flavor.appcontainer) && (vcversion == VCVersion.v140))
            {
                tw.Write(" WindowsApp.lib");
            }
            foreach (var s in libs)
            {
                tw.Write(" {0}", s);
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

        public string basename(string libname)
        {
            var dest = string.Format("{0}_{1}_{2}_{3}", libname, v, f, m);
            return dest;
        }
        public string bat(string libname)
        {
            var dest = string.Format("{0}.bat", basename(libname));
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
        IList<string> includes,
        IList<string> libs
        )
    {
        foreach (var t in trios)
        {
            write_bat(
                libname,
                t,
                cfiles,
                defines,
                includes,
                libs
                );
        }

		using (TextWriter tw = new StreamWriter(string.Format("{0}.bat", libname)))
        {
            tw.WriteLine("@echo on");
            foreach (var t in trios)
            {
                tw.WriteLine("cmd /c {0} > err_{1}.txt 2>&1", t.bat(libname), t.basename(libname));
            }
        }
    }

    static void add_basic_sqlite3_defines(Dictionary<string,string> defines)
    {
        defines["SQLITE_ENABLE_COLUMN_METADATA"] = null;
        defines["SQLITE_ENABLE_FTS3_PARENTHESIS"] = null;
        defines["SQLITE_ENABLE_FTS4"] = null;
        defines["SQLITE_ENABLE_FTS5"] = null;
        defines["SQLITE_ENABLE_JSON1"] = null;
        defines["SQLITE_ENABLE_RTREE"] = null;
        defines["SQLITE_DEFAULT_FOREIGN_KEYS"] = "1";
        defines["SQLITE_WIN32_FILEMAPPING_API"] = "1";
        defines["SQLITE_API"] = "__declspec(dllexport)";
    }

    static void write_e_sqlite3(
        )
    {
        var cfiles = new string[]
        {
            "..\\sqlite3\\sqlite3.c",
        };
        var defines = new Dictionary<string,string>();
		add_basic_sqlite3_defines(defines);
        var includes = new string[]
        {
        };
        var libs = new string[]
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
            includes,
            libs
            );

    }

    static void write_sqlcipher(
        )
    {
        var mbedtls_dir =  "..\\..\\couchbase-lite-libsqlcipher\\vendor\\mbedtls";
        var sqlcipher_dir = "..\\..\\couchbase-lite-libsqlcipher\\vendor\\sqlcipher";
        var borden_dir = "..\\..\\couchbase-lite-libsqlcipher\\src\\c";

        var mbedtls_cfiles = new string[]
        {
            "aes.c",
            "aesni.c",
            "arc4.c",
            "asn1parse.c",
            "asn1write.c",
            "base64.c",
            "bignum.c",
            "blowfish.c",
            "camellia.c",
            "ccm.c",
            "certs.c",
            "cipher.c",
            "cipher_wrap.c",
            "cmac.c",
            "ctr_drbg.c",
            "debug.c",
            "des.c",
            "dhm.c",
            "ecdh.c",
            "ecdsa.c",
            "ecjpake.c",
            "ecp.c",
            "ecp_curves.c",
            "entropy.c",
            "entropy_poll.c",
            "error.c",
            "gcm.c",
            "havege.c",
            "hmac_drbg.c",
            "md2.c",
            "md4.c",
            "md5.c",
            "md.c",
            "md_wrap.c",
            "memory_buffer_alloc.c",
            //"net_sockets.c",
            "oid.c",
            "padlock.c",
            "pem.c",
            "pk.c",
            "pkcs11.c",
            "pkcs12.c",
            "pkcs5.c",
            "pkparse.c",
            "pk_wrap.c",
            "pkwrite.c",
            "platform.c",
            "ripemd160.c",
            "rsa.c",
            "sha1.c",
            "sha256.c",
            "sha512.c",
            "ssl_cache.c",
            "ssl_ciphersuites.c",
            "ssl_cli.c",
            "ssl_cookie.c",
            "ssl_srv.c",
            "ssl_ticket.c",
            "ssl_tls.c",
            "threading.c",
            "timing.c",
            "version.c",
            "version_features.c",
            "x509.c",
            "x509_create.c",
            "x509_crl.c",
            "x509_crt.c",
            "x509_csr.c",
            "x509write_crt.c",
            "x509write_csr.c",
            "xtea.c",
        };

        var cfiles = new List<string>();
        cfiles.Add(Path.Combine(borden_dir, "sqlite3.c"));
        cfiles.Add(Path.Combine(borden_dir, "crypto_mbedtls.c"));
        foreach (var s in mbedtls_cfiles)
        {
            cfiles.Add(Path.Combine(mbedtls_dir, "library", s));
        }

        var defines = new Dictionary<string,string>
        {
            { "SQLITE_HAS_CODEC", null },
            { "SQLCIPHER_CRYPTO_MBEDTLS", null },
            { "CIPHER", "\\\"AES-256-CBC\\\"" },
        };
		add_basic_sqlite3_defines(defines);

        var includes = new List<string>();
        includes.Add(Path.Combine(mbedtls_dir, "include"));
        includes.Add(Path.Combine(sqlcipher_dir, "src"));
        includes.Add(sqlcipher_dir);

        var libs = new string[]
        {
            "advapi32.lib",
			"bcrypt.lib",
        };

        var trios = new trio[]
        {
#if not
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
#endif

#if not
            new trio(VCVersion.v120, Flavor.plain, Machine.x86),
            new trio(VCVersion.v120, Flavor.plain, Machine.x64),
            new trio(VCVersion.v120, Flavor.plain, Machine.arm),

            new trio(VCVersion.v120, Flavor.appcontainer, Machine.x86),
            new trio(VCVersion.v120, Flavor.appcontainer, Machine.x64),
            new trio(VCVersion.v120, Flavor.appcontainer, Machine.arm),
#endif

            new trio(VCVersion.v140, Flavor.plain, Machine.x86),
            new trio(VCVersion.v140, Flavor.plain, Machine.x64),
            new trio(VCVersion.v140, Flavor.plain, Machine.arm),

            new trio(VCVersion.v140, Flavor.appcontainer, Machine.x86),
            new trio(VCVersion.v140, Flavor.appcontainer, Machine.x64),
            new trio(VCVersion.v140, Flavor.appcontainer, Machine.arm),

        };

        write_multibat(
            "sqlcipher",
            trios,
            cfiles,
            defines,
            includes,
            libs
            );

    }

    public static void Main()
    {
        write_e_sqlite3();
        write_sqlcipher();

		using (TextWriter tw = new StreamWriter("all.bat"))
        {
            tw.WriteLine("cmd /c e_sqlite3.bat");
            tw.WriteLine("cmd /c sqlcipher.bat");
        }
    }
}

