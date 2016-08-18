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
using System.IO;
using System.Text;
using SQLitePCL;
using SQLitePCL.Ugly;

using Xunit;

namespace SQLitePCL.Test
{
	static class Common
	{
		public static void Init()
		{
#if PROVIDER_e_sqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
#elif PROVIDER_custom_sqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_custom_sqlite3());
#elif PROVIDER_sqlite3
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#elif PROVIDER_cppinterop
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_cppinterop());
#elif PROVIDER_internal
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_internal());
#elif PROVIDER_sqlcipher
		    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlcipher());
#elif PROVIDER_bundle
            SQLitePCL.Batteries.Init();
#elif PROVIDER_none
	    // used for when the tests are in a PCL
#else
#error test_cases.cs built with no provider specified
#endif
		}
	}

	public class test_cases
	{
        [Fact]
        public void passing()
        {
            Assert.Equal(5-3, 1+1);
        }

        [Fact]
        public void test_bind_parameter_index()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int, v int, t text, d real, b blob, q blob);");
                using (sqlite3_stmt stmt = db.prepare("INSERT INTO foo (x,v,t,d,b,q) VALUES (:x,:v,:t,:d,:b,:q)"))
                {
					Assert.True(stmt.stmt_readonly() == 0);

					Assert.Equal(stmt.bind_parameter_count(), 6);

                    Assert.Equal(stmt.bind_parameter_index(":m"), 0);

                    Assert.Equal(stmt.bind_parameter_index(":x"), 1);
                    Assert.Equal(stmt.bind_parameter_index(":v"), 2);
                    Assert.Equal(stmt.bind_parameter_index(":t"), 3);
                    Assert.Equal(stmt.bind_parameter_index(":d"), 4);
                    Assert.Equal(stmt.bind_parameter_index(":b"), 5);
                    Assert.Equal(stmt.bind_parameter_index(":q"), 6);

                    Assert.Equal(stmt.bind_parameter_name(1), ":x");
                    Assert.Equal(stmt.bind_parameter_name(2), ":v");
                    Assert.Equal(stmt.bind_parameter_name(3), ":t");
                    Assert.Equal(stmt.bind_parameter_name(4), ":d");
                    Assert.Equal(stmt.bind_parameter_name(5), ":b");
                    Assert.Equal(stmt.bind_parameter_name(6), ":q");
                }
            }
        }

    }

}

