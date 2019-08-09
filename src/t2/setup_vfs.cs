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
    [Order(1)]
    public class class_setup_vfs
    {
        [Test]
        public void test_setup_vfs()
        {
            var vfs = new SQLitePCL.Tests.my_vfs();
            var rc = SQLitePCL.raw.sqlite3_vfs_register(SQLitePCL.utf8z.FromString("whatever"), vfs, 1);
            Assert.Equal(0, rc);
        }
    }
}
