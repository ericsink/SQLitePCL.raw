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
    public static class Batteries_V2
    {
	    public static void Init()
	    {
#if EMBEDDED_INIT
            SQLitePCL.lib.embedded.Init();
#endif

#if PROVIDER_sqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#elif PROVIDER_e_sqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
#elif PROVIDER_custom_sqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_custom_sqlite3());
#elif PROVIDER_winsqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());
#elif PROVIDER_internal
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_internal());
#elif PROVIDER_sqlcipher
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlcipher());
#elif PROVIDER_none
            throw new Exception("This is the 'bait'.  You probably need to add one of the SQLitePCLRaw.bundle_* nuget packages to your platform project.");
#else
#error batteries_v2.cs built with nothing specified
#endif
	    }
    }
}

