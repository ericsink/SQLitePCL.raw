
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
        var dest_bat = t.bat(libname);
        var dest_linkargs = t.linkargs(libname);
		using (TextWriter tw = new StreamWriter(dest_linkargs))
		{
            tw.Write(" /nologo");
            tw.Write(" /OUT:\"bin\\{1}\\{0}.dll\"", libname, subdir);
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
                tw.Write(" obj\\{1}\\{0}.obj", b, subdir);
            }
            tw.WriteLine();
		}
		using (TextWriter tw = new StreamWriter(dest_bat))
        {
            tw.WriteLine("@echo on");
			tw.WriteLine("SETLOCAL");
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
			tw.Write(" @{0}", dest_linkargs);
			tw.WriteLine();
			tw.WriteLine("ENDLOCAL");
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
        public string linkargs(string libname)
        {
            var dest = string.Format("{0}.linkargs", basename(libname));
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
                tw.WriteLine("cmd /c {0} > err_{1}.buildoutput.txt 2>&1", t.bat(libname), t.basename(libname));
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
		var tomcrypt_src_dir = "..\\..\\libtomcrypt\\src";
		var tomcrypt_include_dir = "..\\..\\libtomcrypt\\src\\headers";
        var sqlcipher_dir = "..\\sqlcipher_new";
        var tomcrypt_cfiles = new string[]
		{
"modes\\cbc\\cbc_decrypt.c",
"modes\\cbc\\cbc_done.c",
"modes\\cbc\\cbc_encrypt.c",
"modes\\cbc\\cbc_getiv.c",
"modes\\cbc\\cbc_setiv.c",
"modes\\cbc\\cbc_start.c",
"prngs\\fortuna.c",
"mac\\hmac\\hmac_done.c",
"mac\\hmac\\hmac_file.c",
"mac\\hmac\\hmac_init.c",
"mac\\hmac\\hmac_memory.c",
"mac\\hmac\\hmac_memory_multi.c",
"mac\\hmac\\hmac_process.c",
"hashes\\sha2\\sha256.c",
"ciphers\\aes\\aes.c",
"misc\\crypt\\crypt_argchk.c",
"misc\\crypt\\crypt_hash_is_valid.c",
"misc\\zeromem.c",
"misc\\crypt\\crypt_hash_descriptor.c",
"hashes\\helper\\hash_memory.c",
"misc\\crypt\\crypt_cipher_descriptor.c",
"misc\\crypt\\crypt_cipher_is_valid.c",
"misc\\crypt\\crypt_find_cipher.c",
"misc\\crypt\\crypt_register_hash.c",
"misc\\crypt\\crypt_register_cipher.c",
"misc\\crypt\\crypt_find_hash.c",
"misc\\compare_testvector.c",
"misc\\pkcs5\\pkcs_5_2.c",
"misc\\crypt\\crypt_register_prng.c",
"hashes\\sha1.c",
"misc\\crypt\\crypt_prng_descriptor.c",
		};

        var other_tomcrypt_cfiles = new string[]
        {

"ciphers\\aes\\aes_tab.c",
"ciphers\\anubis.c",
"ciphers\\blowfish.c",
"ciphers\\camellia.c",
"ciphers\\cast5.c",
"ciphers\\des.c",
"ciphers\\idea.c",
"ciphers\\kasumi.c",
"ciphers\\khazad.c",
"ciphers\\kseed.c",
"ciphers\\multi2.c",
"ciphers\\noekeon.c",
"ciphers\\rc2.c",
"ciphers\\rc5.c",
"ciphers\\rc6.c",
"ciphers\\safer\\safer.c",
"ciphers\\safer\\saferp.c",
"ciphers\\safer\\safer_tab.c",
"ciphers\\serpent.c",
"ciphers\\skipjack.c",
"ciphers\\twofish\\twofish.c",
"ciphers\\twofish\\twofish_tab.c",
"ciphers\\xtea.c",
"encauth\\ccm\\ccm_add_aad.c",
"encauth\\ccm\\ccm_add_nonce.c",
"encauth\\ccm\\ccm_done.c",
"encauth\\ccm\\ccm_init.c",
"encauth\\ccm\\ccm_memory.c",
"encauth\\ccm\\ccm_process.c",
"encauth\\ccm\\ccm_reset.c",
"encauth\\ccm\\ccm_test.c",
"encauth\\chachapoly\\chacha20poly1305_add_aad.c",
"encauth\\chachapoly\\chacha20poly1305_decrypt.c",
"encauth\\chachapoly\\chacha20poly1305_done.c",
"encauth\\chachapoly\\chacha20poly1305_encrypt.c",
"encauth\\chachapoly\\chacha20poly1305_init.c",
"encauth\\chachapoly\\chacha20poly1305_memory.c",
"encauth\\chachapoly\\chacha20poly1305_setiv.c",
"encauth\\chachapoly\\chacha20poly1305_setiv_rfc7905.c",
"encauth\\chachapoly\\chacha20poly1305_test.c",
"encauth\\eax\\eax_addheader.c",
"encauth\\eax\\eax_decrypt.c",
"encauth\\eax\\eax_decrypt_verify_memory.c",
"encauth\\eax\\eax_done.c",
"encauth\\eax\\eax_encrypt.c",
"encauth\\eax\\eax_encrypt_authenticate_memory.c",
"encauth\\eax\\eax_init.c",
"encauth\\eax\\eax_test.c",
"encauth\\gcm\\gcm_add_aad.c",
"encauth\\gcm\\gcm_add_iv.c",
"encauth\\gcm\\gcm_done.c",
"encauth\\gcm\\gcm_gf_mult.c",
"encauth\\gcm\\gcm_init.c",
"encauth\\gcm\\gcm_memory.c",
"encauth\\gcm\\gcm_mult_h.c",
"encauth\\gcm\\gcm_process.c",
"encauth\\gcm\\gcm_reset.c",
"encauth\\gcm\\gcm_test.c",
"encauth\\ocb\\ocb_decrypt.c",
"encauth\\ocb\\ocb_decrypt_verify_memory.c",
"encauth\\ocb\\ocb_done_decrypt.c",
"encauth\\ocb\\ocb_done_encrypt.c",
"encauth\\ocb\\ocb_encrypt.c",
"encauth\\ocb\\ocb_encrypt_authenticate_memory.c",
"encauth\\ocb\\ocb_init.c",
"encauth\\ocb\\ocb_ntz.c",
"encauth\\ocb\\ocb_shift_xor.c",
"encauth\\ocb\\ocb_test.c",
"encauth\\ocb\\s_ocb_done.c",
"encauth\\ocb3\\ocb3_add_aad.c",
"encauth\\ocb3\\ocb3_decrypt.c",
"encauth\\ocb3\\ocb3_decrypt_last.c",
"encauth\\ocb3\\ocb3_decrypt_verify_memory.c",
"encauth\\ocb3\\ocb3_done.c",
"encauth\\ocb3\\ocb3_encrypt.c",
"encauth\\ocb3\\ocb3_encrypt_authenticate_memory.c",
"encauth\\ocb3\\ocb3_encrypt_last.c",
"encauth\\ocb3\\ocb3_init.c",
"encauth\\ocb3\\ocb3_int_ntz.c",
"encauth\\ocb3\\ocb3_int_xor_blocks.c",
"encauth\\ocb3\\ocb3_test.c",
"hashes\\blake2b.c",
"hashes\\blake2s.c",
"hashes\\chc\\chc.c",
"hashes\\helper\\hash_file.c",
"hashes\\helper\\hash_filehandle.c",
"hashes\\helper\\hash_memory_multi.c",
"hashes\\md2.c",
"hashes\\md4.c",
"hashes\\md5.c",
"hashes\\rmd128.c",
"hashes\\rmd160.c",
"hashes\\rmd256.c",
"hashes\\rmd320.c",
"hashes\\sha2\\sha224.c",
"hashes\\sha2\\sha384.c",
"hashes\\sha2\\sha512.c",
"hashes\\sha2\\sha512_224.c",
"hashes\\sha2\\sha512_256.c",
"hashes\\sha3.c",
"hashes\\sha3_test.c",
"hashes\\tiger.c",
"hashes\\whirl\\whirl.c",
"hashes\\whirl\\whirltab.c",
"mac\\blake2\\blake2bmac.c",
"mac\\blake2\\blake2bmac_file.c",
"mac\\blake2\\blake2bmac_memory.c",
"mac\\blake2\\blake2bmac_memory_multi.c",
"mac\\blake2\\blake2bmac_test.c",
"mac\\blake2\\blake2smac.c",
"mac\\blake2\\blake2smac_file.c",
"mac\\blake2\\blake2smac_memory.c",
"mac\\blake2\\blake2smac_memory_multi.c",
"mac\\blake2\\blake2smac_test.c",
"mac\\f9\\f9_done.c",
"mac\\f9\\f9_file.c",
"mac\\f9\\f9_init.c",
"mac\\f9\\f9_memory.c",
"mac\\f9\\f9_memory_multi.c",
"mac\\f9\\f9_process.c",
"mac\\f9\\f9_test.c",
"mac\\hmac\\hmac_test.c",
"mac\\omac\\omac_done.c",
"mac\\omac\\omac_file.c",
"mac\\omac\\omac_init.c",
"mac\\omac\\omac_memory.c",
"mac\\omac\\omac_memory_multi.c",
"mac\\omac\\omac_process.c",
"mac\\omac\\omac_test.c",
"mac\\pelican\\pelican.c",
"mac\\pelican\\pelican_memory.c",
"mac\\pelican\\pelican_test.c",
"mac\\pmac\\pmac_done.c",
"mac\\pmac\\pmac_file.c",
"mac\\pmac\\pmac_init.c",
"mac\\pmac\\pmac_memory.c",
"mac\\pmac\\pmac_memory_multi.c",
"mac\\pmac\\pmac_ntz.c",
"mac\\pmac\\pmac_process.c",
"mac\\pmac\\pmac_shift_xor.c",
"mac\\pmac\\pmac_test.c",
"mac\\poly1305\\poly1305.c",
"mac\\poly1305\\poly1305_file.c",
"mac\\poly1305\\poly1305_memory.c",
"mac\\poly1305\\poly1305_memory_multi.c",
"mac\\poly1305\\poly1305_test.c",
"mac\\xcbc\\xcbc_done.c",
"mac\\xcbc\\xcbc_file.c",
"mac\\xcbc\\xcbc_init.c",
"mac\\xcbc\\xcbc_memory.c",
"mac\\xcbc\\xcbc_memory_multi.c",
"mac\\xcbc\\xcbc_process.c",
"mac\\xcbc\\xcbc_test.c",
"math\\fp\\ltc_ecc_fp_mulmod.c",
"math\\gmp_desc.c",
"math\\ltm_desc.c",
"math\\multi.c",
"math\\radix_to_bin.c",
"math\\rand_bn.c",
"math\\rand_prime.c",
"math\\tfm_desc.c",
"misc\\adler32.c",
"misc\\base32\\base32_decode.c",
"misc\\base32\\base32_encode.c",
"misc\\base64\\base64_decode.c",
"misc\\base64\\base64_encode.c",
"misc\\burn_stack.c",
"misc\\copy_or_zeromem.c",
"misc\\crc32.c",
"misc\\crypt\\crypt.c",
"misc\\crypt\\crypt_constants.c",
"misc\\crypt\\crypt_find_cipher_any.c",
"misc\\crypt\\crypt_find_cipher_id.c",
"misc\\crypt\\crypt_find_hash_any.c",
"misc\\crypt\\crypt_find_hash_id.c",
"misc\\crypt\\crypt_find_hash_oid.c",
"misc\\crypt\\crypt_find_prng.c",
"misc\\crypt\\crypt_fsa.c",
"misc\\crypt\\crypt_inits.c",
"misc\\crypt\\crypt_ltc_mp_descriptor.c",
"misc\\crypt\\crypt_prng_is_valid.c",
"misc\\crypt\\crypt_prng_rng_descriptor.c",
"misc\\crypt\\crypt_register_all_ciphers.c",
"misc\\crypt\\crypt_register_all_hashes.c",
"misc\\crypt\\crypt_register_all_prngs.c",
"misc\\crypt\\crypt_sizes.c",
"misc\\crypt\\crypt_unregister_cipher.c",
"misc\\crypt\\crypt_unregister_hash.c",
"misc\\crypt\\crypt_unregister_prng.c",
"misc\\error_to_string.c",
"misc\\hkdf\\hkdf.c",
"misc\\hkdf\\hkdf_test.c",
"misc\\mem_neq.c",
"misc\\pkcs5\\pkcs_5_1.c",
"misc\\pkcs5\\pkcs_5_test.c",
"misc\\pk_get_oid.c",
"modes\\cfb\\cfb_decrypt.c",
"modes\\cfb\\cfb_done.c",
"modes\\cfb\\cfb_encrypt.c",
"modes\\cfb\\cfb_getiv.c",
"modes\\cfb\\cfb_setiv.c",
"modes\\cfb\\cfb_start.c",
"modes\\ctr\\ctr_decrypt.c",
"modes\\ctr\\ctr_done.c",
"modes\\ctr\\ctr_encrypt.c",
"modes\\ctr\\ctr_getiv.c",
"modes\\ctr\\ctr_setiv.c",
"modes\\ctr\\ctr_start.c",
"modes\\ctr\\ctr_test.c",
"modes\\ecb\\ecb_decrypt.c",
"modes\\ecb\\ecb_done.c",
"modes\\ecb\\ecb_encrypt.c",
"modes\\ecb\\ecb_start.c",
"modes\\f8\\f8_decrypt.c",
"modes\\f8\\f8_done.c",
"modes\\f8\\f8_encrypt.c",
"modes\\f8\\f8_getiv.c",
"modes\\f8\\f8_setiv.c",
"modes\\f8\\f8_start.c",
"modes\\f8\\f8_test_mode.c",
"modes\\lrw\\lrw_decrypt.c",
"modes\\lrw\\lrw_done.c",
"modes\\lrw\\lrw_encrypt.c",
"modes\\lrw\\lrw_getiv.c",
"modes\\lrw\\lrw_process.c",
"modes\\lrw\\lrw_setiv.c",
"modes\\lrw\\lrw_start.c",
"modes\\lrw\\lrw_test.c",
"modes\\ofb\\ofb_decrypt.c",
"modes\\ofb\\ofb_done.c",
"modes\\ofb\\ofb_encrypt.c",
"modes\\ofb\\ofb_getiv.c",
"modes\\ofb\\ofb_setiv.c",
"modes\\ofb\\ofb_start.c",
"modes\\xts\\xts_decrypt.c",
"modes\\xts\\xts_done.c",
"modes\\xts\\xts_encrypt.c",
"modes\\xts\\xts_init.c",
"modes\\xts\\xts_mult_x.c",
"modes\\xts\\xts_test.c",
"pk\\asn1\\der\\bit\\der_decode_bit_string.c",
"pk\\asn1\\der\\bit\\der_decode_raw_bit_string.c",
"pk\\asn1\\der\\bit\\der_encode_bit_string.c",
"pk\\asn1\\der\\bit\\der_encode_raw_bit_string.c",
"pk\\asn1\\der\\bit\\der_length_bit_string.c",
"pk\\asn1\\der\\boolean\\der_decode_boolean.c",
"pk\\asn1\\der\\boolean\\der_encode_boolean.c",
"pk\\asn1\\der\\boolean\\der_length_boolean.c",
"pk\\asn1\\der\\choice\\der_decode_choice.c",
"pk\\asn1\\der\\generalizedtime\\der_decode_generalizedtime.c",
"pk\\asn1\\der\\generalizedtime\\der_encode_generalizedtime.c",
"pk\\asn1\\der\\generalizedtime\\der_length_generalizedtime.c",
"pk\\asn1\\der\\ia5\\der_decode_ia5_string.c",
"pk\\asn1\\der\\ia5\\der_encode_ia5_string.c",
"pk\\asn1\\der\\ia5\\der_length_ia5_string.c",
"pk\\asn1\\der\\integer\\der_decode_integer.c",
"pk\\asn1\\der\\integer\\der_encode_integer.c",
"pk\\asn1\\der\\integer\\der_length_integer.c",
"pk\\asn1\\der\\object_identifier\\der_decode_object_identifier.c",
"pk\\asn1\\der\\object_identifier\\der_encode_object_identifier.c",
"pk\\asn1\\der\\object_identifier\\der_length_object_identifier.c",
"pk\\asn1\\der\\octet\\der_decode_octet_string.c",
"pk\\asn1\\der\\octet\\der_encode_octet_string.c",
"pk\\asn1\\der\\octet\\der_length_octet_string.c",
"pk\\asn1\\der\\printable_string\\der_decode_printable_string.c",
"pk\\asn1\\der\\printable_string\\der_encode_printable_string.c",
"pk\\asn1\\der\\printable_string\\der_length_printable_string.c",
"pk\\asn1\\der\\sequence\\der_decode_sequence_ex.c",
"pk\\asn1\\der\\sequence\\der_decode_sequence_flexi.c",
"pk\\asn1\\der\\sequence\\der_decode_sequence_multi.c",
"pk\\asn1\\der\\sequence\\der_decode_subject_public_key_info.c",
"pk\\asn1\\der\\sequence\\der_encode_sequence_ex.c",
"pk\\asn1\\der\\sequence\\der_encode_sequence_multi.c",
"pk\\asn1\\der\\sequence\\der_encode_subject_public_key_info.c",
"pk\\asn1\\der\\sequence\\der_length_sequence.c",
"pk\\asn1\\der\\sequence\\der_sequence_free.c",
"pk\\asn1\\der\\sequence\\der_sequence_shrink.c",
"pk\\asn1\\der\\set\\der_encode_set.c",
"pk\\asn1\\der\\set\\der_encode_setof.c",
"pk\\asn1\\der\\short_integer\\der_decode_short_integer.c",
"pk\\asn1\\der\\short_integer\\der_encode_short_integer.c",
"pk\\asn1\\der\\short_integer\\der_length_short_integer.c",
"pk\\asn1\\der\\teletex_string\\der_decode_teletex_string.c",
"pk\\asn1\\der\\teletex_string\\der_length_teletex_string.c",
"pk\\asn1\\der\\utctime\\der_decode_utctime.c",
"pk\\asn1\\der\\utctime\\der_encode_utctime.c",
"pk\\asn1\\der\\utctime\\der_length_utctime.c",
"pk\\asn1\\der\\utf8\\der_decode_utf8_string.c",
"pk\\asn1\\der\\utf8\\der_encode_utf8_string.c",
"pk\\asn1\\der\\utf8\\der_length_utf8_string.c",
"pk\\dh\\dh.c",
"pk\\dh\\dh_check_pubkey.c",
"pk\\dh\\dh_export.c",
"pk\\dh\\dh_export_key.c",
"pk\\dh\\dh_free.c",
"pk\\dh\\dh_generate_key.c",
"pk\\dh\\dh_import.c",
"pk\\dh\\dh_set.c",
"pk\\dh\\dh_set_pg_dhparam.c",
"pk\\dh\\dh_shared_secret.c",
"pk\\dsa\\dsa_decrypt_key.c",
"pk\\dsa\\dsa_encrypt_key.c",
"pk\\dsa\\dsa_export.c",
"pk\\dsa\\dsa_free.c",
"pk\\dsa\\dsa_generate_key.c",
"pk\\dsa\\dsa_generate_pqg.c",
"pk\\dsa\\dsa_import.c",
"pk\\dsa\\dsa_make_key.c",
"pk\\dsa\\dsa_set.c",
"pk\\dsa\\dsa_set_pqg_dsaparam.c",
"pk\\dsa\\dsa_shared_secret.c",
"pk\\dsa\\dsa_sign_hash.c",
"pk\\dsa\\dsa_verify_hash.c",
"pk\\dsa\\dsa_verify_key.c",
"pk\\ecc\\ecc.c",
"pk\\ecc\\ecc_ansi_x963_export.c",
"pk\\ecc\\ecc_ansi_x963_import.c",
"pk\\ecc\\ecc_decrypt_key.c",
"pk\\ecc\\ecc_encrypt_key.c",
"pk\\ecc\\ecc_export.c",
"pk\\ecc\\ecc_free.c",
"pk\\ecc\\ecc_get_size.c",
"pk\\ecc\\ecc_import.c",
"pk\\ecc\\ecc_make_key.c",
"pk\\ecc\\ecc_shared_secret.c",
"pk\\ecc\\ecc_sign_hash.c",
"pk\\ecc\\ecc_sizes.c",
"pk\\ecc\\ecc_test.c",
"pk\\ecc\\ecc_verify_hash.c",
"pk\\ecc\\ltc_ecc_is_valid_idx.c",
"pk\\ecc\\ltc_ecc_map.c",
"pk\\ecc\\ltc_ecc_mul2add.c",
"pk\\ecc\\ltc_ecc_mulmod.c",
"pk\\ecc\\ltc_ecc_mulmod_timing.c",
"pk\\ecc\\ltc_ecc_points.c",
"pk\\ecc\\ltc_ecc_projective_add_point.c",
"pk\\ecc\\ltc_ecc_projective_dbl_point.c",
"pk\\katja\\katja_decrypt_key.c",
"pk\\katja\\katja_encrypt_key.c",
"pk\\katja\\katja_export.c",
"pk\\katja\\katja_exptmod.c",
"pk\\katja\\katja_free.c",
"pk\\katja\\katja_import.c",
"pk\\katja\\katja_make_key.c",
"pk\\pkcs1\\pkcs_1_i2osp.c",
"pk\\pkcs1\\pkcs_1_mgf1.c",
"pk\\pkcs1\\pkcs_1_oaep_decode.c",
"pk\\pkcs1\\pkcs_1_oaep_encode.c",
"pk\\pkcs1\\pkcs_1_os2ip.c",
"pk\\pkcs1\\pkcs_1_pss_decode.c",
"pk\\pkcs1\\pkcs_1_pss_encode.c",
"pk\\pkcs1\\pkcs_1_v1_5_decode.c",
"pk\\pkcs1\\pkcs_1_v1_5_encode.c",
"pk\\rsa\\rsa_decrypt_key.c",
"pk\\rsa\\rsa_encrypt_key.c",
"pk\\rsa\\rsa_export.c",
"pk\\rsa\\rsa_exptmod.c",
"pk\\rsa\\rsa_free.c",
"pk\\rsa\\rsa_get_size.c",
"pk\\rsa\\rsa_import.c",
"pk\\rsa\\rsa_import_pkcs8.c",
"pk\\rsa\\rsa_import_x509.c",
"pk\\rsa\\rsa_make_key.c",
"pk\\rsa\\rsa_set.c",
"pk\\rsa\\rsa_sign_hash.c",
"pk\\rsa\\rsa_sign_saltlen_get.c",
"pk\\rsa\\rsa_verify_hash.c",
"prngs\\chacha20.c",
"prngs\\rc4.c",
"prngs\\rng_get_bytes.c",
"prngs\\rng_make_prng.c",
"prngs\\sober128.c",
"prngs\\sprng.c",
"prngs\\yarrow.c",
"stream\\chacha\\chacha_crypt.c",
"stream\\chacha\\chacha_done.c",
"stream\\chacha\\chacha_ivctr32.c",
"stream\\chacha\\chacha_ivctr64.c",
"stream\\chacha\\chacha_keystream.c",
"stream\\chacha\\chacha_setup.c",
"stream\\chacha\\chacha_test.c",
"stream\\rabbit\\rabbit.c",
"stream\\rc4\\rc4_stream.c",
"stream\\rc4\\rc4_test.c",
"stream\\salsa20\\salsa20_crypt.c",
"stream\\salsa20\\salsa20_done.c",
"stream\\salsa20\\salsa20_ivctr64.c",
"stream\\salsa20\\salsa20_keystream.c",
"stream\\salsa20\\salsa20_setup.c",
"stream\\salsa20\\salsa20_test.c",
"stream\\sober128\\sober128tab.c",
"stream\\sober128\\sober128_stream.c",
"stream\\sober128\\sober128_test.c",
"stream\\sosemanuk\\sosemanuk.c",
"stream\\sosemanuk\\sosemanuk_test.c",
		};
 
        var cfiles = new List<string>();
        cfiles.Add(Path.Combine(sqlcipher_dir, "sqlite3.c"));
        foreach (var s in tomcrypt_cfiles)
        {
            cfiles.Add(Path.Combine(tomcrypt_src_dir, s));
        }

        var defines = new Dictionary<string,string>
        {
            { "_WIN32", null }, // for tomcrypt
            { "ENDIAN_LITTLE", null }, // for tomcrypt arm
            { "LTC_NO_PROTOTYPES", null },
            { "LTC_SOURCE", null },
            { "SQLITE_HAS_CODEC", null },
            { "SQLITE_TEMP_STORE", "2" },
            { "SQLCIPHER_CRYPTO_LIBTOMCRYPT", null },
            { "CIPHER", "\\\"AES-256-CBC\\\"" },
        };
		add_basic_sqlite3_defines(defines);

        var includes = new List<string>();
        includes.Add(sqlcipher_dir);
        includes.Add(tomcrypt_include_dir);

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

