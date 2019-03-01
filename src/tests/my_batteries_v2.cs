/*
   Copyright 2014-2016 Zumero, LLC

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
	    public static void Init()
	    {
			var dir = Directory.GetCurrentDirectory();
			while (true)
			{
				var path = Path.Combine(dir, "e_sqlite3.dll");
				if (File.Exists(path))
				{
					var path2 = Path.Combine(dir, "e_sqlite3");
					SQLitePCL.Setup.Load(path2);
					break;
				}
				else
				{
					if (dir.ToLower() == "c:\\")
					{
						break;
					}
					dir = Path.GetFullPath(Path.Combine(dir, ".."));
				}
			}
	    }
    }
}

