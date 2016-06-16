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

namespace SQLitePCL
{
    public static class Batteries
    {
	    public static void Init()
	    {
#if BATTERY_ESQLITE3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_esqlite3());
#elif BATTERY_INTERNAL
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_internal());
#elif BATTERY_SQLCIPHER
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlcipher());
#elif BATTERY_NONE
		    // intentionally don't do anything here
#else
		    //throw new Exception("batteries.cs built with nothing specified");
#endif
	    }
    }
}
