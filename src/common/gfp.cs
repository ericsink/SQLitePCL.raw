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

using System;

namespace SQLitePCL
{
    public static partial class NativeLibrary
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

        public static IGetFunctionPointer Setup(IntPtr dll)
        {
            var gf = new MyGetFunctionPointer(dll);
            return gf;
        }
    }
}

