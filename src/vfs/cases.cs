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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using SQLitePCL;
using SQLitePCL.Ugly;

using Xunit;

namespace SQLitePCL.Tests
{

    [CollectionDefinition("Init")]
    public class InitCollection : ICollectionFixture<Init>
    {
    }

    public class Init
    {
        public Init()
        {
            //SQLitePCL.Setup.Load("c:/Windows/system32/winsqlite3.dll");
            //SQLitePCL.Setup.Load("e_sqlite3.dll");
            SQLitePCL.Batteries_V2.Init();
        }
    }

    static class u
    {
		public static string NewId_hex()
        {
            string id = Guid
                .NewGuid()
                .ToString()
                .Replace("{", "")
                .Replace("}", "")
                .Replace("-", "");
            return id;
        }
    }

    [Collection("Init")]
    public class class_test_vfs
    {
        [Fact]
        public void test_vfs()
        {
            const string VFS_NAME = "fried";

            var vfs = new my_vfs();
            var rc = raw.sqlite3_vfs_register(utf8z.FromString(VFS_NAME), vfs, 0);
            Assert.Equal(0, rc);

            var filename = "fried_" + u.NewId_hex();
            rc = raw.sqlite3_open_v2(filename, out var db, raw.SQLITE_OPEN_READWRITE | raw.SQLITE_OPEN_CREATE, VFS_NAME);
            Assert.Equal(0, rc);

            rc = raw.sqlite3_exec(db, "CREATE TABLE foo (x int);");
            Assert.Equal(0, rc);

            rc = raw.sqlite3_close_v2(db);
            Assert.Equal(0, rc);

            // TODO unregister vfs
        }
    }
}

