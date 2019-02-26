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
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SQLitePCL
{
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

	public static class Setup
	{
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

		static bool TryLoad(string name, Loader plat, out IGetFunctionPointer gf)
		{
			if (plat == Loader.win)
			{
				if (NativeLib_Win.try_Load(name, out var api))
				{
					gf = api;
					return true;
				}
				else
				{
					gf = null;
					return false;
				}
			}
			else if (plat == Loader.dlopen)
			{
				if (NativeLib_dlopen.try_Load(name, out var api))
				{
					gf = api;
					return true;
				}
				else
				{
					gf = null;
					return false;
				}
			}
			else
			{
				throw new NotImplementedException();
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

		static bool Search(
			IList<string> a, 
			Loader plat, 
			out string name,
			out IGetFunctionPointer gf
			)
		{
			foreach (var s in a)
			{
				if (TryLoad(s, plat, out var api))
				{
					name = s;
					gf = api;
					return true;
				}
			}
			name = null;
			gf = null;
			return false;
		}

		static List<string> MakePossibilitiesFor(
			string basename,
			LibSuffix suffix
			)
		{
			var a = new List<string>();

#if not
			a.Add(basename);
#endif

			var libname = basename_to_libname(basename, suffix);
			a.Add(libname);

#if not // TODO GetCurrentDirectory is in netstandard 1.3
			var cwd = System.IO.Directory.GetCurrentDirectory();
#endif

#if not // TODO GetExecutingAssembly is only in netstandard 2.0
			var dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#endif

			// TODO add other names and paths, depending on platform

			return a;
		}

		public static void Load_ios_internal()
		{
			// TODO err check this
			var dll = NativeLib_dlopen.dlopen(null, NativeLib_dlopen.RTLD_NOW);
			var gf = new GetFunctionPointer_dlopen(dll);
			SQLite3Provider_Cdecl.Setup(gf);
			raw.SetProvider(new SQLite3Provider_Cdecl());
		}

		public static string Load(string basename)
		{
			var plat = WhichLoader();
			//System.Console.WriteLine("plat: {0}", plat);
			var suffix = WhichLibSuffix();
			//System.Console.WriteLine("suffix: {0}", suffix);
			var a = MakePossibilitiesFor(basename, suffix);
			//System.Console.WriteLine("possibilities:");
			foreach (var s in a)
			{
				//System.Console.WriteLine("    {0}", s);
			}
			if (Search(a, plat, out var lib, out var gf))
			{
				//System.Console.WriteLine("found: {0}", lib);
				SQLite3Provider_Cdecl.Setup(gf);
				raw.SetProvider(new SQLite3Provider_Cdecl());
				return lib;
			}
			else
			{
				return null;
			}
		}
	}
}

