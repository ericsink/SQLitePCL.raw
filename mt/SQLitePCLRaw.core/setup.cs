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

using System;
using System.Runtime.InteropServices;

namespace SQLitePCL
{
	public static class Setup
	{
		// the hope is that all use cases can be handled by
		// adding the flexibility here

		public static void Load_ios_internal()
		{
			var dll = NativeLib_dlopen.dlopen(null, NativeLib_dlopen.RTLD_NOW);
			var gf = new GetFunctionPointer_dlopen(dll);
			SQLite3Provider_Cdecl.Setup(gf);
			raw.SetProvider(new SQLite3Provider_Cdecl());
		}

		public static void Load(string name)
		{
			IntPtr dll;
			if (NativeLib_Win.try_LoadLibrary(name, out dll))
			{
				var gf = new GetFunctionPointer_Win(dll);
				SQLite3Provider_Cdecl.Setup(gf);
				raw.SetProvider(new SQLite3Provider_Cdecl());
			}
			else if (NativeLib_dlopen.try_dlopen(name, out dll))
			{
				var gf = new GetFunctionPointer_dlopen(dll);
				SQLite3Provider_Cdecl.Setup(gf);
				raw.SetProvider(new SQLite3Provider_Cdecl());
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
