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
#if false
	class GetFunctionPointer_dotnetcore3 : IGetFunctionPointer
	{
		readonly IntPtr _dll;
		public GetFunctionPointer_dotnetcore3(IntPtr dll)
		{
			_dll = dll;
		}

		public IntPtr GetFunctionPointer(string name)
		{
            if (System.Runtime.InteropServices.NativeLibrary.TryGetExport(_dll, name, out var f))
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
#endif

    public static class Batteries_V2
    {
        public static void Init()
        {
#if false
            var dll = System.Runtime.InteropServices.NativeLibrary.Load("e_sqlite3");
            var gf = new GetFunctionPointer_dotnetcore3(dll);
            SQLitePCL.Setup.Load(gf);
#else
            SQLitePCL.Setup.Load("e_sqlite3", s => File.AppendAllLines("log.txt", new string[] { s }));
#endif
        }
    }
}

