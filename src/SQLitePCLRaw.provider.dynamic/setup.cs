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

		static bool TryLoad(
			string name, 
			Loader plat, 
			Action<string> log,
			out IGetFunctionPointer gf
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
						gf = new GetFunctionPointer_Win(ptr);
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
						gf = new GetFunctionPointer_dlopen(ptr);
						return true;
					}
					else
					{
						// TODO log errno?
						gf = null;
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
				gf = null;
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
			Action<string> log,
			out string name,
			out IGetFunctionPointer gf
			)
		{
			foreach (var s in a)
			{
				if (TryLoad(s, plat, log, out var api))
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

			{
				var dir = System.AppContext.BaseDirectory;
				a.Add(Path.Combine(dir, "runtimes", "win-x64", "native", libname));
			}

			{
				var dir = System.IO.Directory.GetCurrentDirectory();
				a.Add(Path.Combine(dir, "runtimes", "win-x64", "native", libname));
			}

			{
				var dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				a.Add(Path.Combine(dir, "runtimes", "win-x64", "native", libname));
			}

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

		public static string Load(
			string basename
			)
		{
			return Load(basename, s => {});
		}

		public static string Load(
			string basename,
			Action<string> log
			)
		{
			// TODO make this code accept a string that already has the suffix?

			var plat = WhichLoader();
			log($"plat: {plat}");
			var suffix = WhichLibSuffix();
			log($"suffix: {suffix}");
			var a = MakePossibilitiesFor(basename, suffix);
			log("possibilities:");
			foreach (var s in a)
			{
				log($"    {s}");
			}
			if (Search(a, plat, log, out var lib, out var gf))
			{
				log($"found: {lib}");
				SQLite3Provider_Cdecl.Setup(gf);
				raw.SetProvider(new SQLite3Provider_Cdecl());
				return lib;
			}
			else
			{
				log("NOT FOUND");
				return null;
			}
		}
	}
}

