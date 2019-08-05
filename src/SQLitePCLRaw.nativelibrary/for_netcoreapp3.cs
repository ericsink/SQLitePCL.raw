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

namespace SQLitePCL
{
    public static partial class NativeLibrary
    {
        public static IntPtr Load(string libraryName, System.Reflection.Assembly assy, int flags)
        {
            // TODO convert flags
            return System.Runtime.InteropServices.NativeLibrary.Load(libraryName, assy, null);
        }
        public static bool TryLoad(string libraryName, System.Reflection.Assembly assy, int flags, out IntPtr handle)
        {
            // TODO convert flags
            return System.Runtime.InteropServices.NativeLibrary.TryLoad(libraryName, assy, null, out handle);
        }
        public static IntPtr GetExport(IntPtr handle, string name)
        {
            return System.Runtime.InteropServices.NativeLibrary.GetExport(handle, name);
        }
        public static bool TryGetExport(IntPtr handle, string name, out IntPtr address)
        {
            return System.Runtime.InteropServices.NativeLibrary.TryGetExport(handle, name, out address);
        }
        public static void Free(IntPtr handle)
        {
            System.Runtime.InteropServices.NativeLibrary.Free(handle);
        }
    }
}

