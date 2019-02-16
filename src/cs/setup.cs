/*
   Copyright 2014-2019 Zumero, LLC

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

namespace SQLitePCL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

	static class NativeMethods_Win
	{
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
	}

	class GetFunctionPtr_Win : IGetFunctionPtr
	{
		readonly IntPtr _dll;
		public GetFunctionPtr_Win(string dll)
		{
			_dll = NativeMethods_Win.LoadLibrary(dll);
		}

		public IntPtr GetFunctionPtr(string name)
		{
			var f = NativeMethods_Win.GetProcAddress(_dll, name);
			//System.Console.WriteLine("{0}.{1} : {2}", _dll, name, f);
			return f;
		}
	}

	public static class Setup
	{
		// the hope is that all use cases can be handled by
		// adding the flexibility here

		public static void Load(string name)
		{
			var gf = new GetFunctionPtr_Win(name);
			SQLite3Provider_dyn.NativeMethods = new MyDelegates(gf);
		}
	}

}
