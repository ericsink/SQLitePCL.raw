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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace SQLitePCL
{
    public static class Batteries_V2
    {
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
        static void DoDynamic_cdecl(string name, int flags)
        {
            var gf = MakeDynamic(name, flags);
            SQLitePCL.SQLite3Provider_dynamic_cdecl.Setup(name, gf);
            SQLitePCL.raw.SetProvider(new SQLite3Provider_dynamic_cdecl());
        }

        public static void Init()
        {
            DoDynamic_cdecl("e_sqlite3", NativeLibrary.WHERE_PLAIN);
        }
    }
}

