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
    using System.Runtime.InteropServices;

	static class NativeMethods_dlopen
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
		public GetFunctionPtr_Win(IntPtr dll)
		{
			_dll = dll;
		}

		public IntPtr GetFunctionPtr(string name)
		{
			var f = NativeMethods_Win.GetProcAddress(_dll, name);
			//System.Console.WriteLine("{0}.{1} : {2}", _dll, name, f);
			return f;
		}
	}

	class GetFunctionPtr_dlopen : IGetFunctionPtr
	{
		readonly IntPtr _dll;
		public GetFunctionPtr_dlopen(IntPtr dll)
		{
			_dll = dll;
		}

		public IntPtr GetFunctionPtr(string name)
		{
			var f = NativeMethods_dlopen.dlsym(_dll, name);
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
			var dll = NativeMethods_Win.LoadLibrary(name);
			if (dll != IntPtr.Zero)
			{
				var gf = new GetFunctionPtr_Win(dll);
				SQLite3Provider_dyn.NativeMethods = new MyDelegates(gf);
			}
			else
			{
				dll = NativeMethods_dlopen.dlopen(name, 0); // TODO flags
				if (dll != IntPtr.Zero)
				{
					var gf = new GetFunctionPtr_dlopen(dll);
					SQLite3Provider_dyn.NativeMethods = new MyDelegates(gf);
				}
				else
				{
					throw new NotImplementedException();
				}

			}
		}
	}

}
