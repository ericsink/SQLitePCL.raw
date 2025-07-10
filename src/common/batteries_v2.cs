﻿/*
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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SQLitePCL
{
    // for source-level compatibility with v1
    public static class Batteries
    {
	    public static void Init()
	    {
            Batteries_V2.Init();
        }
    }

    public static class Batteries_V2
    {
#if PROVIDER_dynamic
        class MyGetFunctionPointer : IGetFunctionPointer
        {
            readonly IntPtr _dll;
            public MyGetFunctionPointer(IntPtr dll)
            {
                _dll = dll;
            }

            public IntPtr GetFunctionPointer(string name)
            {
                if (NativeLibrary.TryGetExport(_dll, name, out var f))
                {
                    //System.Console.WriteLine("{0}.{1} : {2}", _dll, name, f);
                    return f;
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
        }
        static IGetFunctionPointer MakeDynamic(string name, int flags)
        {
            // TODO should this be GetExecutingAssembly()?
            var assy = typeof(SQLitePCL.raw).Assembly;
            var dll = SQLitePCL.NativeLibrary.Load(name, assy, flags);
            var gf = new MyGetFunctionPointer(dll);
            return gf;
        }
#endif
#if PROVIDER_dynamic_cdecl
        static void DoDynamic_cdecl(string name, int flags)
        {
            var gf = MakeDynamic(name, flags);
            SQLitePCL.SQLite3Provider_dynamic_cdecl.Setup(name, gf);
            SQLitePCL.raw.SetProvider(new SQLite3Provider_dynamic_cdecl());
        }
#endif
#if PROVIDER_dynamic_stdcall
        static void DoDynamic_stdcall(string name, int flags)
        {
            var gf = MakeDynamic(name, flags);
            SQLitePCL.SQLite3Provider_dynamic_stdcall.Setup(name, gf);
            SQLitePCL.raw.SetProvider(new SQLite3Provider_dynamic_stdcall());
        }
#endif

	    public static void Init()
	    {
#if EMBEDDED_INIT
            SQLitePCL.lib.embedded.Init();
#endif

#if PROVIDER_sqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#elif PROVIDER_e_sqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
#elif PROVIDER_e_see
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_see());
#elif PROVIDER_e_sqlite3mc
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3mc());
#elif PROVIDER_sqlcipher
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlcipher());
#elif PROVIDER_winsqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());
#elif PROVIDER_internal
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_internal());
#else
#error batteries_v2.cs built with nothing specified
#endif
	    }
    }
}

