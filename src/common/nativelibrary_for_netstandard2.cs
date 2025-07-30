/*
   Copyright 2014-2025 SourceGear, LLC

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
using System.Linq;
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

        enum Loader
        {
            win,
            dlopen,
        }

        public static IntPtr Load(string path)
        {
            var logWriter = new StringWriter();
            // TODO check file exists?
            logWriter.WriteLine($"Library {path} not found");
            var plat = WhichLoader();
            if (TryLoad(path, plat, s => logWriter.WriteLine(s), out var api))
            {
                return api;
            }
            else
            {
                throw new Exception(logWriter.ToString());
            }
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

        public static bool TryGetExport(IntPtr handle, string name, out IntPtr address)
        {
            var h = MyGetExport(handle, name);
            address = h;
            return h != IntPtr.Zero;
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

    }

}


