/*
   Copyright 2014-2019 SourceGear, LLC

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

// Copyright © Microsoft Open Technologies, Inc.
// All Rights Reserved
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache 2 License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SQLitePCL
{
    public static partial class NativeLibrary
    {
        static class NativeLib_dlopen
        {
            const string SO = "dl";

            public const int RTLD_NOW = 2; // for dlopen's flags 

            [DllImport(SO)]
            public static extern IntPtr dlopen(string dllToLoad, int flags);

            [DllImport(SO)]
            public static extern IntPtr dlsym(IntPtr hModule, string procedureName);

            [DllImport(SO)]
            public static extern int dlclose(IntPtr hModule);

        }

        static class NativeLib_Win
        {
            [DllImport("kernel32", SetLastError = true)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            public const uint LOAD_WITH_ALTERED_SEARCH_PATH = 8;

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool FreeLibrary(IntPtr hModule);

        }

        enum LibSuffix
        {
            DLL,
            DYLIB,
            SO,
        }

        enum Loader
        {
            win,
            dlopen,
        }

        public static IntPtr Load(string libraryName, System.Reflection.Assembly assy, int flags)
        {
            var h = MyLoad(libraryName, assy, flags, s => { });
            if (h == IntPtr.Zero)
            {
                throw new Exception($"Library {libraryName} not found");
            }
            return h;
        }
        static IntPtr MyGetExport(IntPtr handle, string name)
        {
            var plat = WhichLoader();
            if (plat == Loader.win)
            {
                return NativeLib_Win.GetProcAddress(handle, name);
            }
            else if (plat == Loader.dlopen)
            {
                return NativeLib_dlopen.dlsym(handle, name);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static IntPtr GetExport(IntPtr handle, string name)
        {
            var h = MyGetExport(handle, name);
            if (h == IntPtr.Zero)
            {
                throw new Exception($"Symbol {name} not found");
            }
            return h;
        }
        public static void Free(IntPtr handle)
        {
            var plat = WhichLoader();
            if (plat == Loader.win)
            {
                NativeLib_Win.FreeLibrary(handle);
            }
            else if (plat == Loader.dlopen)
            {
                NativeLib_dlopen.dlclose(handle);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static bool TryLoad(string libraryName, System.Reflection.Assembly assy, int flags, out IntPtr handle)
        {
            var h = MyLoad(libraryName, assy, flags, s => { });
            handle = h;
            return h != IntPtr.Zero;
        }
        public static bool TryGetExport(IntPtr handle, string name, out IntPtr address)
        {
            var h = MyGetExport(handle, name);
            address = h;
            return h != IntPtr.Zero;
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

        static bool TryLoad(
            string name,
            Loader plat,
            Action<string> log,
            out IntPtr h
            )
        {
            try
            {
                if (plat == Loader.win)
                {
                    log($"win TryLoad: {name}");
                    var ptr = NativeLib_Win.LoadLibrary(name);
                    if (ptr != IntPtr.Zero)
                    {
                        log($"LoadLibrary gave: {ptr}");
                        h = ptr;
                        return true;
                    }
                    else
                    {
                        var err = Marshal.GetLastWin32Error();
                        // NOT HERE: log($"error code: {err}");
                        throw new System.ComponentModel.Win32Exception();
                    }
                }
                else if (plat == Loader.dlopen)
                {
                    log($"dlopen TryLoad: {name}");
                    var ptr = NativeLib_dlopen.dlopen(name, NativeLib_dlopen.RTLD_NOW);
                    log($"dlopen gave: {ptr}");
                    if (ptr != IntPtr.Zero)
                    {
                        h = ptr;
                        return true;
                    }
                    else
                    {
                        // TODO log errno?
                        h = IntPtr.Zero;
                        return false;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (NotImplementedException)
            {
                throw;
            }
            catch (Exception e)
            {
                log($"thrown: {e}");
                h = IntPtr.Zero;
                return false;
            }
        }

        static Loader WhichLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Loader.win;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Loader.dlopen;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Loader.dlopen;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        static LibSuffix WhichLibSuffix()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LibSuffix.DLL;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LibSuffix.SO;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LibSuffix.DYLIB;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        static string get_rid_front()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "win";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "osx";
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        static string get_rid_back()
        {
            switch (RuntimeInformation.OSArchitecture)
            {
                case Architecture.Arm:
                    return "arm";
                case Architecture.Arm64:
                    return "arm64";
                case Architecture.X64:
                    return (IntPtr.Size == 8) ? "x64" : "x86";
                case Architecture.X86:
                    return "x86";
                default:
                    throw new NotImplementedException();
            }
        }

        static string get_rid()
        {
            var front = get_rid_front();
            var back = get_rid_back();
            return $"{front}-{back}";
        }

        static bool Search(
            IList<string> a,
            Loader plat,
            Action<string> log,
            out string name,
            out IntPtr h
            )
        {
            foreach (var s in a)
            {
                if (TryLoad(s, plat, log, out var api))
                {
                    name = s;
                    h = api;
                    return true;
                }
            }
            name = null;
            h = IntPtr.Zero;
            return false;
        }

        static List<string> MakePossibilitiesFor(
            string basename,
            System.Reflection.Assembly assy,
            int flags,
            LibSuffix suffix
            )
        {
            var a = new List<string>();

#if not
			a.Add(basename);
#endif

            var libname = basename_to_libname(basename, suffix);
            if ((flags & WHERE_PLAIN) != 0)
            {
                a.Add(libname);
            }

#if not // TODO is this ever useful?
			{
				var dir = System.IO.Directory.GetCurrentDirectory();
				a.Add(Path.Combine(dir, "runtimes", rid, "native", libname));
			}
#endif

            if ((flags & WHERE_RUNTIME_RID) != 0)
            {
                var rid = get_rid();
                var dir = System.IO.Path.GetDirectoryName(assy.Location);
                a.Add(Path.Combine(dir, "runtimes", rid, "native", libname));
            }

            if ((flags & WHERE_ARCH) != 0)
            {
                var dir = System.IO.Path.GetDirectoryName(assy.Location);
                var arch = get_rid_back();
                a.Add(Path.Combine(dir, arch, libname));
            }

            return a;
        }

#if not
		static IntPtr Load_ios_internal()
		{
			// TODO err check this
			var dll = NativeLib_dlopen.dlopen(null, NativeLib_dlopen.RTLD_NOW);
            return dll;
		}
#endif

        static IntPtr MyLoad(
            string basename,
            System.Reflection.Assembly assy,
            int flags,
            Action<string> log
            )
        {
            // TODO make this code accept a string that already has the suffix?
            // TODO does S.R.I.NativeLibrary do that?

            var plat = WhichLoader();
            log($"plat: {plat}");
            var suffix = WhichLibSuffix();
            log($"suffix: {suffix}");
            var a = MakePossibilitiesFor(basename, assy, flags, suffix);
            log("possibilities:");
            foreach (var s in a)
            {
                log($"    {s}");
            }
            if (Search(a, plat, log, out var lib, out var h))
            {
                log($"found: {lib}");
                return h;
            }
            else
            {
                log("NOT FOUND");
                return IntPtr.Zero;
            }
        }
    }
}

