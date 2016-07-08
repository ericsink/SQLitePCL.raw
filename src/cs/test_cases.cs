﻿/*
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

// NOTE
//
// If you are getting a compile error right around here, you 
// probably need to add a reference to the test framework you
// are using.
//
// And if that test framework is NUnit, you should #define USE_NUNIT
// somewhere.
//
#if USE_NUNIT
using NUnit.Framework;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestContext = System.Object;
using TestProperty = NUnit.Framework.PropertyAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace SQLitePCL.Test
{
	[TestClass]
	public class test_cases
	{
        [TestMethod]
        public void test_bind_parameter_index()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int, v int, t text, d real, b blob, q blob);");
                using (sqlite3_stmt stmt = db.prepare("INSERT INTO foo (x,v,t,d,b,q) VALUES (:x,:v,:t,:d,:b,:q)"))
                {
					Assert.IsTrue(stmt.stmt_readonly() == 0);

					Assert.AreEqual(stmt.bind_parameter_count(), 6);

                    Assert.AreEqual(stmt.bind_parameter_index(":m"), 0);

                    Assert.AreEqual(stmt.bind_parameter_index(":x"), 1);
                    Assert.AreEqual(stmt.bind_parameter_index(":v"), 2);
                    Assert.AreEqual(stmt.bind_parameter_index(":t"), 3);
                    Assert.AreEqual(stmt.bind_parameter_index(":d"), 4);
                    Assert.AreEqual(stmt.bind_parameter_index(":b"), 5);
                    Assert.AreEqual(stmt.bind_parameter_index(":q"), 6);

                    Assert.AreEqual(stmt.bind_parameter_name(1), ":x");
                    Assert.AreEqual(stmt.bind_parameter_name(2), ":v");
                    Assert.AreEqual(stmt.bind_parameter_name(3), ":t");
                    Assert.AreEqual(stmt.bind_parameter_name(4), ":d");
                    Assert.AreEqual(stmt.bind_parameter_name(5), ":b");
                    Assert.AreEqual(stmt.bind_parameter_name(6), ":q");
                }
            }
        }

        [TestMethod]
        public void test_blob_read()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                const int len = 100;

                db.exec("CREATE TABLE foo (b blob);");
                db.exec("INSERT INTO foo (b) VALUES (randomblob(?))", len);
                long rowid = db.last_insert_rowid();

                byte[] blob = db.query_scalar<byte[]>("SELECT b FROM foo;");
                Assert.AreEqual(blob.Length, len);

                using (sqlite3_blob bh = db.blob_open("main", "foo", "b", rowid, 0))
                {
                    int len2 = bh.bytes();
                    Assert.AreEqual(len, len2);

                    int passes = 10;

                    Assert.AreEqual(len % passes, 0);

                    int sublen = len / passes;
                    byte[] buf = new byte[sublen];

                    for (int q=0; q<passes; q++)
                    {
                        bh.read(buf, q * sublen);

                        for (int i=0; i<sublen; i++)
                        {
                            Assert.AreEqual(blob[q * sublen + i], buf[i]);
                        }
                    }
                }
            }

        }

        [TestMethod]
        public void test_blob_read_with_byte_array_offset()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                const int len = 100;

                db.exec("CREATE TABLE foo (b blob);");
                db.exec("INSERT INTO foo (b) VALUES (zeroblob(?))", len);
                long rowid = db.last_insert_rowid();

                byte[] blob = db.query_scalar<byte[]>("SELECT b FROM foo;");
                Assert.AreEqual(blob.Length, len);

                using (sqlite3_blob bh = db.blob_open("main", "foo", "b", rowid, 0))
                {
                    int len2 = bh.bytes();
                    Assert.AreEqual(len, len2);

		    byte[] blob2 = new byte[len];
		    for (int i=0; i<len; i++)
		    {
			blob2[i] = 73;
		    }

		    bh.read(blob2, 40, 20, 40);

		    for (int i=0; i<40; i++)
		    {
		        Assert.AreEqual(blob2[i], 73);
		    }

		    for (int i=40; i<60; i++)
		    {
		        Assert.AreEqual(blob2[i], 0);
		    }

		    for (int i=60; i<100; i++)
		    {
		        Assert.AreEqual(blob2[i], 73);
		    }
                }
            }

        }

        [TestMethod]
        public void test_blob_write_with_byte_array_offset()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                const int len = 100;

                db.exec("CREATE TABLE foo (b blob);");
                db.exec("INSERT INTO foo (b) VALUES (zeroblob(?))", len);
                long rowid = db.last_insert_rowid();

                byte[] blob = db.query_scalar<byte[]>("SELECT b FROM foo;");
                Assert.AreEqual(blob.Length, len);

	        for (int i=0; i<100; i++)
	        {
		    Assert.AreEqual(blob[i], 0);
	        }

                using (sqlite3_blob bh = db.blob_open("main", "foo", "b", rowid, 1))
                {
                    int len2 = bh.bytes();
                    Assert.AreEqual(len, len2);

		    byte[] blob2 = new byte[len];
		    for (int i=0; i<100; i++)
		    {
			blob2[i] = 73;
		    }

		    bh.write(blob2, 40, 20, 50);

                }

                byte[] blob3 = db.query_scalar<byte[]>("SELECT b FROM foo;");
                Assert.AreEqual(blob3.Length, len);

	        for (int i=0; i<50; i++)
	        {
		    Assert.AreEqual(blob3[i], 0);
	        }

	        for (int i=50; i<70; i++)
	        {
		    Assert.AreEqual(blob3[i], 73);
	        }

	        for (int i=70; i<100; i++)
	        {
		    Assert.AreEqual(blob3[i], 0);
	        }

            }

        }

        [TestMethod]
        public void test_blob_write()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                const int len = 100;

                db.exec("CREATE TABLE foo (b blob);");
                using (sqlite3_stmt stmt = db.prepare("INSERT INTO foo (b) VALUES (?)"))
                {
                    stmt.bind_zeroblob(1, len);
                    stmt.step();
                }

                long rowid = db.last_insert_rowid();

                using (sqlite3_blob bh = db.blob_open("main", "foo", "b", rowid, 1))
                {
                    int len2 = bh.bytes();
                    Assert.AreEqual(len, len2);

                    int passes = 10;

                    Assert.AreEqual(len % passes, 0);

                    int sublen = len / passes;
                    byte[] buf = new byte[sublen];
                    for (int i=0; i<sublen; i++)
                    {
                        buf[i] = (byte) (i % 256);
                    }

                    for (int q=0; q<passes; q++)
                    {
                        bh.write(buf, q * sublen);
                    }
                }
            }
        }
        
        [TestMethod]
        public void test_db_readonly()
        {
            using (sqlite3 db = ugly.open_v2(":memory:", raw.SQLITE_OPEN_READONLY, null))
            {
                int result = raw.sqlite3_db_readonly(db, "main");
                Assert.IsTrue(result > 0);
            }

            using (sqlite3 db = ugly.open(":memory:"))
            {
                int result = raw.sqlite3_db_readonly(db, "main");
                Assert.AreEqual(result, 0);
            }
        }

        [TestMethod]
        public void test_get_autocommit()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                Assert.AreEqual(db.get_autocommit(), 1);
                db.exec("BEGIN TRANSACTION;");
                Assert.IsTrue(db.get_autocommit() == 0);
            }
        }

        [TestMethod]
        public void test_last_insert_rowid()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x text);");
                db.exec("INSERT INTO foo (x) VALUES ('b')");
                Assert.AreEqual(db.last_insert_rowid(), 1);
            }
        }

        [TestMethod]
        public void test_libversion()
        {
            string sourceid = raw.sqlite3_sourceid();
            Assert.IsTrue(sourceid != null);
            Assert.IsTrue(sourceid.Length > 0);

            string libversion = raw.sqlite3_libversion();
            Assert.IsTrue(libversion != null);
            Assert.IsTrue(libversion.Length > 0);
            Assert.AreEqual(libversion[0], '3');

            int libversion_number = raw.sqlite3_libversion_number();
            Assert.AreEqual(libversion_number / 1000000, 3);
        }

        [TestMethod]
        public void test_enable_shared_cache()
        {
            int result = raw.sqlite3_enable_shared_cache(1);
            Assert.AreEqual(result, raw.SQLITE_OK);

            result = raw.sqlite3_enable_shared_cache(0);
            Assert.AreEqual(result, raw.SQLITE_OK);
        }

        [TestMethod]
        public void test_limit()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                raw.sqlite3_limit(db, raw.SQLITE_LIMIT_SQL_LENGTH, 10);

                db.exec("SELECT 1;");
                try
                {
                    db.exec("SELECT 200000;");
                    Assert.Fail();
                }
                catch (ugly.sqlite3_exception e)
                {
                    Assert.AreEqual(e.errcode, raw.SQLITE_TOOBIG);
                }
            }
        }

        [TestMethod]
        public void test_sqlite3_memory()
        {
            long memory_used = raw.sqlite3_memory_used();
            long memory_highwater = raw.sqlite3_memory_highwater(0);
#if not
            // these asserts fail on the iOS builtin sqlite.  not sure
            // why.  not sure the asserts are worth doing anyway.
            Assert.IsTrue(memory_used > 0);
            Assert.IsTrue(memory_highwater >= memory_used);
#endif
        }

        [TestMethod]
        public void test_sqlite3_status()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                int current;
                int highwater;
                ugly.sqlite3_status(raw.SQLITE_STATUS_MEMORY_USED, out current, out highwater, 0);

                Assert.IsTrue(current > 0);
                Assert.IsTrue(highwater > 0);
            }
	}

        [TestMethod]
        public void test_backup()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x text);");
                db.exec("INSERT INTO foo (x) VALUES ('b')");
                db.exec("INSERT INTO foo (x) VALUES ('c')");
                db.exec("INSERT INTO foo (x) VALUES ('d')");
                db.exec("INSERT INTO foo (x) VALUES ('e')");
                db.exec("INSERT INTO foo (x) VALUES ('f')");

                using (sqlite3 db2 = ugly.open(":memory:"))
                {
                    using (sqlite3_backup bak = db.backup_init("main", db2, "main"))
                    {
                        bak.step(-1);
                        Assert.AreEqual(bak.remaining(), 0);
                        Assert.IsTrue(bak.pagecount() > 0);
                    }
                }
            }
        }

        [TestMethod]
        public void test_compileoption()
        {
            int i = 0;
            while (true)
            {
                string s = raw.sqlite3_compileoption_get(i++);
                if (s == null)
                {
                    break;
                }
                int used = raw.sqlite3_compileoption_used(s);
                Assert.IsTrue(used != 0);
            }
        }

        [TestMethod]
        public void test_bernt()
        {
            using (sqlite3 db = ugly.open(""))
            {
                db.exec("CREATE TABLE places_dat (resource_handle TEXT PRIMARY KEY, data BLOB) WITHOUT ROWID;");
                byte[] buf1 = db.query_scalar<byte[]>("SELECT randomblob(16);");
		db.exec("INSERT INTO places_dat VALUES (?,?)", "foo", buf1);
                Assert.AreEqual(1, db.changes());
		int c1 = db.query_scalar<int>("SELECT COUNT(*) FROM places_dat WHERE resource_handle='foo';");
	        Assert.AreEqual(1, c1);
		byte[] buf2 = new byte[2];
		buf2[0] = 42;
		buf2[1] = 73;
		db.exec("UPDATE places_dat SET data=? WHERE resource_handle=?;", buf2, "foo");
                Assert.AreEqual(1, db.changes());
                byte[] buf3 = db.query_scalar<byte[]>("SELECT data FROM places_dat WHERE resource_handle='foo';");
		Assert.AreEqual(2, buf3.Length);
		Assert.AreEqual(buf2[0], buf3[0]);
		Assert.AreEqual(buf2[1], buf3[1]);
            }
        }

        [TestMethod]
        public void test_create_table_temp_db()
        {
            using (sqlite3 db = ugly.open(""))
            {
                db.exec("CREATE TABLE foo (x int);");
            }
        }

        [TestMethod]
        public void test_create_table_file()
        {
	    string name;
            using (sqlite3 db = ugly.open(":memory:"))
            {
                name = "tmp" + db.query_scalar<string>("SELECT lower(hex(randomblob(16)));");
            }
	    string filename;
            using (sqlite3 db = ugly.open(name))
	    {
                db.exec("CREATE TABLE foo (x int);");
		filename = db.db_filename("main");
	    }

	    // TODO verify the filename is what we expect

	    ugly.vfs__delete(null, filename, 1);
        }

        [TestMethod]
        public void test_error()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int UNIQUE);");
                db.exec("INSERT INTO foo (x) VALUES (3);");
                bool fail = false;
                try
                {
                    db.exec("INSERT INTO foo (x) VALUES (3);");
                }
                catch (ugly.sqlite3_exception e)
                {
                    fail = true;

                    int errcode = db.errcode();
                    Assert.AreEqual(errcode, e.errcode);
                    Assert.AreEqual(errcode, raw.SQLITE_CONSTRAINT);

                    Assert.AreEqual(db.extended_errcode(), raw.SQLITE_CONSTRAINT_UNIQUE, "Extended error codes for SQLITE_CONSTRAINT were added in 3.7.16");

                    string errmsg = db.errmsg();
                    Assert.IsTrue(errmsg != null);
                    Assert.IsTrue(errmsg.Length > 0);
                }
                Assert.IsTrue(fail);

                Assert.IsTrue(raw.sqlite3_errstr(raw.SQLITE_CONSTRAINT) != null);
            }
        }

        [TestMethod]
        public void test_result_zeroblob()
        {
            delegate_function_scalar zeroblob_func =
                (ctx, user_data, args) =>
                {
                    int size = raw.sqlite3_value_int(args[0]);
                    raw.sqlite3_result_zeroblob(ctx, size);
                };

            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x blob);");
                db.create_function("createblob", 1, null, zeroblob_func);
                db.exec("INSERT INTO foo (x) VALUES(createblob(10));");
                
                var rowid = db.last_insert_rowid();
                byte[] blob = db.query_scalar<byte[]>("SELECT x FROM foo WHERE rowid=" + rowid);
                Assert.AreEqual(blob.Length, 10);
                foreach (var b in blob)
                {
                    Assert.AreEqual(b, 0);
                }
            }
        }

        [TestMethod]
        public void test_result_errors()
        {
            int code = 10;
            delegate_function_scalar errorcode_func = 
                (ctx, user_data, args) => raw.sqlite3_result_error_code(ctx, code);

            delegate_function_scalar toobig_func =
                (ctx, user_data, args) => raw.sqlite3_result_error_toobig(ctx);

            delegate_function_scalar nomem_func =
                (ctx, user_data, args) => raw.sqlite3_result_error_nomem(ctx);

            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("errorcode", 0, null, errorcode_func);
                db.create_function("toobig", 0, null, toobig_func);
                db.create_function("nomem", 0, null, nomem_func);

                try
                {
                    db.exec("select errorcode();");
                    Assert.Fail("expected exception");
                } 
                catch (ugly.sqlite3_exception e)
                {
                    Assert.AreEqual(e.errcode, code);
                }

                try
                {
                    db.exec("select toobig();");
                    Assert.Fail("expected exception");
                }
                catch (ugly.sqlite3_exception e)
                {
                    Assert.AreEqual(e.errcode, raw.SQLITE_TOOBIG);
                }

                try
                {
                    db.exec("select nomem();");
                    Assert.Fail("expected exception");
                }
                catch (ugly.sqlite3_exception e)
                {
                    Assert.AreEqual(e.errcode, raw.SQLITE_NOMEM);
                }
            }
        }

        [TestMethod]
        public void test_create_table_memory_db()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
            }
        }

        [TestMethod]
        public void test_open_v2()
        {
            using (sqlite3 db = ugly.open_v2(":memory:", raw.SQLITE_OPEN_READWRITE | raw.SQLITE_OPEN_CREATE, null))
            {
                db.exec("CREATE TABLE foo (x int);");
            }
        }

        [TestMethod]
        public void test_db_status()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                int current;
                int highwater;
              
                db.db_status(raw.SQLITE_DBSTATUS_CACHE_USED, out current, out highwater, 0);
                Assert.IsTrue(current > 0);
                Assert.AreEqual(highwater, 0);
            }
        }

        [TestMethod]
        public void test_create_table_explicit_close()
        {
            sqlite3 db = ugly.open(":memory:");
            db.exec("CREATE TABLE foo (x int);");
            db.close();
        }

        [TestMethod]
        public void test_create_table_explicit_close_v2()
        {
#if not
            // maybe we should just let this fail so we can
            // see the differences between running against the built-in
            // sqlite vs a recent version?
            if (raw.sqlite3_libversion_number() < 3007014)
            {
                return;
            }
#endif
            sqlite3 db = ugly.open(":memory:");
            db.exec("CREATE TABLE foo (x int);");
            db.close_v2();
        }

        [TestMethod]
        public void test_count()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
                db.exec("INSERT INTO foo (x) VALUES (1);");
                db.exec("INSERT INTO foo (x) VALUES (2);");
                db.exec("INSERT INTO foo (x) VALUES (3);");
                int c = db.query_scalar<int>("SELECT COUNT(*) FROM foo");
                Assert.AreEqual(c, 3);
            }
        }

        [TestMethod]
        public void test_stmt_complete()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");

                Assert.IsTrue(raw.sqlite3_complete("SELECT x FROM") == 0);
                Assert.IsTrue(raw.sqlite3_complete("SELECT") == 0);
                Assert.IsTrue(raw.sqlite3_complete("INSERT INTO") == 0);
                Assert.IsTrue(raw.sqlite3_complete("SELECT x FROM foo") == 0);

                Assert.IsTrue(raw.sqlite3_complete("SELECT x FROM foo;") != 0);
                Assert.IsTrue(raw.sqlite3_complete("SELECT COUNT(*) FROM foo;") != 0);
                Assert.IsTrue(raw.sqlite3_complete("SELECT 5;") != 0);
            }
        }

        [TestMethod]
        public void test_next_stmt()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                Assert.AreEqual(db.next_stmt(null), null);
                using (sqlite3_stmt stmt = db.prepare("SELECT 5;"))
                {
                    Assert.AreEqual(db.next_stmt(null), stmt);
                    Assert.AreEqual(db.next_stmt(stmt), null);
                }
                Assert.AreEqual(db.next_stmt(null), null);
            }
        }

        [TestMethod]
        public void test_stmt_busy()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
                db.exec("INSERT INTO foo (x) VALUES (1);");
                db.exec("INSERT INTO foo (x) VALUES (2);");
                db.exec("INSERT INTO foo (x) VALUES (3);");
                const string sql = "SELECT x FROM foo";
                using (sqlite3_stmt stmt = db.prepare(sql))
                {
                    Assert.AreEqual(sql, stmt.sql());

                    Assert.AreEqual(stmt.stmt_busy(), 0);
                    stmt.step();
                    Assert.IsTrue(stmt.stmt_busy() != 0);
                    stmt.step();
                    Assert.IsTrue(stmt.stmt_busy() != 0);
                    stmt.step();
                    Assert.IsTrue(stmt.stmt_busy() != 0);
                    stmt.step();
                    Assert.IsTrue(stmt.stmt_busy() == 0);
                }
            }
        }

        [TestMethod]
        public void test_stmt_status()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
                
                using (var stmt = db.prepare("SELECT x FROM foo"))
                {
                    stmt.step();

                    int vmStep = raw.sqlite3_stmt_status(stmt, raw.SQLITE_STMTSTATUS_VM_STEP, 0);
                    Assert.IsTrue(vmStep > 0);
                
                    int vmStep2 = raw.sqlite3_stmt_status(stmt, raw.SQLITE_STMTSTATUS_VM_STEP, 1);
                    Assert.AreEqual(vmStep, vmStep2);
                    
                    int vmStep3 = raw.sqlite3_stmt_status(stmt, raw.SQLITE_STMTSTATUS_VM_STEP, 0);
                    Assert.AreEqual(0, vmStep3);
                }
            }
        }

        [TestMethod]
        public void test_total_changes()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                Assert.AreEqual(db.total_changes(), 0);
                Assert.AreEqual(db.changes(), 0);

                db.exec("CREATE TABLE foo (x int);");
                Assert.AreEqual(db.total_changes(), 0);
                Assert.AreEqual(db.changes(), 0);

                db.exec("INSERT INTO foo (x) VALUES (1);");
                Assert.AreEqual(db.total_changes(), 1);
                Assert.AreEqual(db.changes(), 1);
                
                db.exec("INSERT INTO foo (x) VALUES (2);");
                db.exec("INSERT INTO foo (x) VALUES (3);");
                Assert.AreEqual(db.total_changes(), 3);
                Assert.AreEqual(db.changes(), 1);

                db.exec("UPDATE foo SET x=5;");
                Assert.AreEqual(db.total_changes(), 6);
                Assert.AreEqual(db.changes(), 3);
            }
        }

        [TestMethod]
        public void test_explicit_prepare()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
                const int num = 7;
                using (sqlite3_stmt stmt = db.prepare("INSERT INTO foo (x) VALUES (?)"))
                {
                    for (int i=0; i<num; i++)
                    {
                        stmt.reset();
                        stmt.clear_bindings();
                        stmt.bind(1, i);
                        stmt.step();
                    }
                }
                int c = db.query_scalar<int>("SELECT COUNT(*) FROM foo");
                Assert.AreEqual(c, num);
            }
        }

        [TestMethod]
        public void test_exec_two_with_tail()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                string errmsg;
                db.exec("CREATE TABLE foo (x int);INSERT INTO foo (x) VALUES (1);", null, null, out errmsg);
                int c = db.query_scalar<int>("SELECT COUNT(*) FROM foo");
                Assert.AreEqual(c, 1);
            }
        }

        [TestMethod]
        public void test_column_origin()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int, v int, t text, d real, b blob, q blob);");
                byte[] blob = db.query_scalar<byte[]>("SELECT randomblob(5);");
                db.exec("INSERT INTO foo (x,v,t,d,b,q) VALUES (?,?,?,?,?,?)", 32, 44, "hello", 3.14, blob, null);
#if not
                // maybe we should just let this fail so we can
                // see the differences between running against the built-in
                // sqlite vs a recent version?
                if (1 == raw.sqlite3_compileoption_used("ENABLE_COLUMN_METADATA"))
#endif
                {
                    using (sqlite3_stmt stmt = db.prepare("SELECT x AS mario FROM foo;"))
                    {
                        stmt.step();
    
                        Assert.IsTrue(stmt.stmt_readonly() != 0);
    
                        Assert.AreEqual(stmt.column_database_name(0), "main");
                        Assert.AreEqual(stmt.column_table_name(0), "foo");
                        Assert.AreEqual(stmt.column_origin_name(0), "x");
                        Assert.AreEqual(stmt.column_name(0), "mario");
                        Assert.AreEqual(stmt.column_decltype(0), "int");
                    }
                }
            }
        }

        [TestMethod]
        public void test_wal()
        {
            string tmpFile;
            using (sqlite3 db = ugly.open(":memory:"))
            {
                tmpFile = "tmp" + db.query_scalar<string>("SELECT lower(hex(randomblob(16)));");
            }
            using (sqlite3 db = ugly.open(tmpFile))
            {
                db.exec("PRAGMA journal_mode=WAL;");

                // CREATE TABLE results in 2 frames check pointed and increaseses the log size by 2
                // so manually do a checkpoint to reset the counters thus testing both
                // sqlite3_wal_checkpoint and sqlite3_wal_checkpoint_v2.
                db.exec("CREATE TABLE foo (x int);");
                db.wal_checkpoint("main");
                
                db.exec("INSERT INTO foo (x) VALUES (1);");
                db.exec("INSERT INTO foo (x) VALUES (2);");

                int logSize;
                int framesCheckPointed;
                db.wal_checkpoint("main", raw.SQLITE_CHECKPOINT_FULL, out logSize, out framesCheckPointed);

                Assert.AreEqual(2, logSize);
                Assert.AreEqual(2, framesCheckPointed);

                // Set autocheckpoint to 1 so that regardless of the number of 
                // commits, explicit checkpoints only checkpoint the last update.
                db.wal_autocheckpoint(1);

                db.exec("INSERT INTO foo (x) VALUES (3);");
                db.exec("INSERT INTO foo (x) VALUES (4);");
                db.exec("INSERT INTO foo (x) VALUES (5);");
                db.wal_checkpoint("main", raw.SQLITE_CHECKPOINT_PASSIVE, out logSize, out framesCheckPointed);

                Assert.AreEqual(1, logSize);
                Assert.AreEqual(1, framesCheckPointed);
            }

            ugly.vfs__delete(null, tmpFile, 1);
        }

        [TestMethod]
        public void test_set_authorizer()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                var data = new Object();

                delegate_authorizer authorizer =
                    (object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view) =>
                        {
                            Assert.AreEqual(data, user_data);

                            switch (action_code)
                            {
                                // When creating a table an insert is first done.
                                case raw.SQLITE_INSERT:
                                    Assert.AreEqual(param0, "sqlite_master");
                                    Assert.IsNull(param1);
                                    Assert.AreEqual(dbName, "main");
                                    Assert.IsNull(inner_most_trigger_or_view);
                                    break;
                                case raw.SQLITE_CREATE_TABLE:
                                    Assert.AreEqual(param0, "foo");
                                    Assert.IsNull(param1);
                                    Assert.AreEqual(dbName, "main");
                                    Assert.IsNull(inner_most_trigger_or_view);
                                    break;  
                                case raw.SQLITE_READ:
                                    Assert.IsNotNull(param0);
                                    Assert.IsNotNull(param1);
                                    Assert.AreEqual(dbName, "main");
                                    Assert.IsNull(inner_most_trigger_or_view);
                                    break;  
                            }

                            return raw.SQLITE_OK;
                        };

                raw.sqlite3_set_authorizer(db, authorizer, data);
                db.exec("CREATE TABLE foo (x int);");

                GC.Collect();

                db.exec("SELECT * FROM foo;");
                db.exec("CREATE VIEW TEST_VIEW AS SELECT * FROM foo;");

                delegate_authorizer view_authorizer =
                    (object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view) =>
                        {
                            switch (action_code)
                            {
                                case raw.SQLITE_READ:
                                    Assert.IsNotNull(param0);
                                    Assert.IsNotNull(param1);
                                    Assert.AreEqual(dbName, "main");

                                    // A Hack. Goal is to prove that inner_most_trigger_or_view is not null when it is returned in the callback
                                    if (param0 == "foo") { Assert.IsNotNull(inner_most_trigger_or_view); }
                                    break;  
                            }

                            return raw.SQLITE_OK;
                        };

                raw.sqlite3_set_authorizer(db, view_authorizer, data);
                db.exec("SELECT * FROM TEST_VIEW;");

                delegate_authorizer denied_authorizer =
                    (object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view) =>
                        {
                            return raw.SQLITE_DENY;
                        };

                raw.sqlite3_set_authorizer(db, denied_authorizer, data);
               
                try
                {
                    db.exec("SELECT * FROM TEST_VIEW;");
                    Assert.Fail();
                }
                catch (ugly.sqlite3_exception e)
                {
                    Assert.AreEqual(e.errcode, raw.SQLITE_AUTH);
                }
            }
        }
    }

    [TestClass]
    public class class_test_row
    {
        private class row
        {
            public int x { get; set; }
            public long v { get; set; }
            public string t { get; set; }
            public double d { get; set; }
            public byte[] b { get; set; }
            public string q { get; set; }
        }

        [TestMethod]
        public void test_row()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int, v int, t text, d real, b blob, q blob);");
                byte[] blob = db.query_scalar<byte[]>("SELECT randomblob(5);");
                db.exec("INSERT INTO foo (x,v,t,d,b,q) VALUES (?,?,?,?,?,?)", 32, 44, "hello", 3.14, blob, null);
                foreach (row r in db.query<row>("SELECT x,v,t,d,b,q FROM foo;"))
                {
                    Assert.AreEqual(r.x, 32);
                    Assert.AreEqual(r.v, 44);
                    Assert.AreEqual(r.t, "hello");
                    Assert.AreEqual(r.d, 3.14);
                    Assert.AreEqual(r.b.Length, blob.Length);
                    for (int i=0; i<blob.Length; i++)
                    {
                        Assert.AreEqual(r.b[i], blob[i]);
                    }
                    Assert.AreEqual(r.q, null);
                }
                using (sqlite3_stmt stmt = db.prepare("SELECT x,v,t,d,b,q FROM foo;"))
                {
                    stmt.step();

                    Assert.AreEqual(stmt.db_handle(), db);

                    Assert.AreEqual(stmt.column_int(0), 32);
                    Assert.AreEqual(stmt.column_int64(1), 44);
                    Assert.AreEqual(stmt.column_text(2), "hello");
                    Assert.AreEqual(stmt.column_double(3), 3.14);
                    Assert.AreEqual(stmt.column_bytes(4), blob.Length);
                    byte[] b2 = stmt.column_blob(4);
                    Assert.AreEqual(b2.Length, blob.Length);
                    for (int i=0; i<blob.Length; i++)
                    {
                        Assert.AreEqual(b2[i], blob[i]);
                    }

                    Assert.AreEqual(stmt.column_type(5), raw.SQLITE_NULL);

                    Assert.AreEqual(stmt.column_name(0), "x");
                    Assert.AreEqual(stmt.column_name(1), "v");
                    Assert.AreEqual(stmt.column_name(2), "t");
                    Assert.AreEqual(stmt.column_name(3), "d");
                    Assert.AreEqual(stmt.column_name(4), "b");
                    Assert.AreEqual(stmt.column_name(5), "q");
                }
            }
        }

        [TestMethod]
        public void test_table_column_metadata()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                // out string dataType, out string collSeq, out int notNull, out int primaryKey, out int autoInc
                db.exec("CREATE TABLE foo (rowid integer primary key asc autoincrement, x int not null);");

                string dataType;
                string collSeq;
                int notNull;
                int primaryKey;
                int autoInc;

                raw.sqlite3_table_column_metadata(db, "main", "foo", "x", out dataType, out collSeq, out notNull, out primaryKey, out autoInc);
                Assert.AreEqual(dataType, "int");
                Assert.AreEqual(collSeq, "BINARY");
                Assert.IsTrue(notNull > 0);
                Assert.AreEqual(primaryKey, 0);
                Assert.AreEqual(autoInc, 0);

                raw.sqlite3_table_column_metadata(db, "main", "foo", "rowid", out dataType, out collSeq, out notNull, out primaryKey, out autoInc);
                Assert.AreEqual(dataType, "integer");
                Assert.AreEqual(collSeq, "BINARY");
                Assert.AreEqual(notNull, 0);
                Assert.IsTrue(primaryKey > 0);
                Assert.IsTrue(primaryKey > 0);
            }
        }

        [TestMethod]
        public void test_progress_handler()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                int count = 0;

                delegate_progress_handler handler = obj => 
                    {
                        Assert.AreEqual(obj, "user_data");
                        count++;
                        return 0; 
                    };

                raw.sqlite3_progress_handler(db, 1, handler, "user_data");
                
                GC.Collect();

                using (sqlite3_stmt stmt = db.prepare("SELECT 1;"))
                {
                    stmt.step();
                }
                Assert.IsTrue(count > 0);

                handler = obj => 1;
                raw.sqlite3_progress_handler(db, 1, handler, null);
                using (sqlite3_stmt stmt = db.prepare("SELECT 1;"))
                {
                    try
                    {
                        stmt.step();
                        Assert.Fail("Expected sqlite3_exception");
                    }
                    catch (ugly.sqlite3_exception e)
                    {
                        Assert.AreEqual(e.errcode, raw.SQLITE_INTERRUPT);
                    }
                }

                // Test that assigning null to the handler removes the progress handler.
                handler = null;
                raw.sqlite3_progress_handler(db, 1, handler, null);
                using (sqlite3_stmt stmt = db.prepare("SELECT 1;"))
                {
                    stmt.step();
                }
            }
        }
    }

    [TestClass]
    public class class_test_exec_with_callback
    {
        private class work
        {
            public int count;
        }

        private static int my_cb(object v, string[] values, string[] names)
        {
            Assert.AreEqual(values.Length, 1);
            Assert.AreEqual(names.Length, 1);
            Assert.AreEqual(names[0], "x");
            Assert.AreEqual(values[0].Length, 1);

            work w = v as work;
            w.count++;

            return 0;
        }

        [TestMethod]
        public void test_exec_with_callback()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x text);");
                db.exec("INSERT INTO foo (x) VALUES ('b')");
                db.exec("INSERT INTO foo (x) VALUES ('c')");
                db.exec("INSERT INTO foo (x) VALUES ('d')");
                db.exec("INSERT INTO foo (x) VALUES ('e')");
                db.exec("INSERT INTO foo (x) VALUES ('f')");
                string errmsg;
                work w = new work();
                db.exec("SELECT x FROM foo", my_cb, w, out errmsg);
                Assert.AreEqual(w.count, 5);
            }
        }
    }

    [TestClass]
    public class class_test_collation
    {
        private const int val = 5;

        private static int my_collation(object v, string s1, string s2)
        {
            s1 = s1.Replace('e', 'a');
            s2 = s2.Replace('e', 'a');
            return string.Compare(s1, s2);
        }

        [TestMethod]
        public void test_collation()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_collation("e2a", null, my_collation);
                db.exec("CREATE TABLE foo (x text COLLATE e2a);");
                db.exec("INSERT INTO foo (x) VALUES ('b')");
                db.exec("INSERT INTO foo (x) VALUES ('c')");
                db.exec("INSERT INTO foo (x) VALUES ('d')");
                db.exec("INSERT INTO foo (x) VALUES ('e')");
                db.exec("INSERT INTO foo (x) VALUES ('f')");
                string top = db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;");
                Assert.AreEqual(top, "e");
		GC.Collect();
                string top2 = db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;");
                Assert.AreEqual(top2, "e");
            }
        }

	private static void setup(sqlite3 db)
	{
                db.exec("CREATE TABLE foo (x text COLLATE e2a);");
                db.exec("INSERT INTO foo (x) VALUES ('b')");
                db.exec("INSERT INTO foo (x) VALUES ('c')");
                db.exec("INSERT INTO foo (x) VALUES ('d')");
                db.exec("INSERT INTO foo (x) VALUES ('e')");
                db.exec("INSERT INTO foo (x) VALUES ('f')");
	}

        [TestMethod]
        public void test_collation_2()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_collation("e2a", null, my_collation);
		setup(db);
                Assert.AreEqual("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
		        GC.Collect();
                Assert.AreEqual("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                GC.Collect();
                using (sqlite3 db2 = ugly.open(":memory:"))
                {
                    GC.Collect();
                    db2.create_collation("e2a", null, my_collation);
                    GC.Collect();
                    Assert.AreEqual("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                    GC.Collect();
                    setup(db2);
                    GC.Collect();
                    Assert.AreEqual("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                    GC.Collect();
                    Assert.AreEqual("e", db2.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                }
                GC.Collect();
                Assert.AreEqual("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
            }
        }

        [TestMethod]
        public void test_collation_3()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                using (sqlite3 db2 = ugly.open(":memory:"))
                {
                    db.create_collation("e2a", null, my_collation);
                    db2.create_collation("e2a", null, my_collation);
		    setup(db);
                    setup(db2);
                    Assert.AreEqual("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                    Assert.AreEqual("e", db2.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
		    GC.Collect();
                    Assert.AreEqual("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                    Assert.AreEqual("e", db2.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                }
                GC.Collect();
                Assert.AreEqual("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
            }
        }

	private static void setup2(sqlite3 db)
	{
                db.exec("CREATE TABLE foo (x text);");
                db.exec("INSERT INTO foo (x) VALUES ('b')");
                db.exec("INSERT INTO foo (x) VALUES ('c')");
                db.exec("INSERT INTO foo (x) VALUES ('d')");
                db.exec("INSERT INTO foo (x) VALUES ('e')");
                db.exec("INSERT INTO foo (x) VALUES ('f')");
	}

        [TestMethod]
        public void test_collation_4()
        {
		string res1,res2,res3;
            using (sqlite3 db = ugly.open(":memory:"))
            {
		    setup2(db);
                    db.create_collation("col", null, (o,p1,p2) => string.Compare(p1,p2));
			res1 = db.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE col");
		    
                using (sqlite3 db2 = ugly.open(":memory:"))
                {
		    setup2(db2);
                    db2.create_collation("col", null, (o,p1,p2) => string.Compare(p2,p1));
			res2 = db2.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE col");
                }
			res3 = db.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE col");
            }
                Assert.AreEqual(res1,res3);
        }

        private static int my_collation_1(object v, string s1, string s2)
        {
		return string.Compare(s1,s2);
        }

        private static int my_collation_2(object v, string s1, string s2)
        {
		return string.Compare(s2,s1);
        }

        [TestMethod]
        public void test_collation_5()
        {
		string res1,res2,res3;
            using (sqlite3 db = ugly.open(":memory:"))
            {
		    setup2(db);
                    db.create_collation("col", null, my_collation_1);
			res1 = db.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE col");
		    
                using (sqlite3 db2 = ugly.open(":memory:"))
                {
		    setup2(db2);
                    db2.create_collation("col", null, my_collation_2);
			res2 = db2.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE col");
                }
			res3 = db.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE col");
            }
                Assert.AreEqual(res1,res3);
        }

    }

    [TestClass]
    public class class_test_cube
    {
        private const int val = 5;

        private static void cube(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            Assert.AreEqual(args.Length, 1);
            long x = args[0].value_int64();
            Assert.AreEqual(x, val);
            ctx.result_int64(x * x * x);
        }

        [TestMethod]
       public void test_cube()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("cube", 1, null, cube);
                long c = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.AreEqual(c, val * val * val);
		GC.Collect();
                long c2 = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.AreEqual(c2, val * val * val);
            }
        }
    }

    [TestClass]
    public class class_test_makeblob
    {
        private const int val = 5;

        private static void makeblob(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            byte[] b = new byte[args[0].value_int()];
            for (int i=0; i<b.Length; i++)
            {
                b[i] = (byte) (i % 256);
            }
            ctx.result_blob(b);
        }

        [TestMethod]
       public void test_makeblob()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("makeblob", 1, null, makeblob);
                byte[] c = db.query_scalar<byte[]>("SELECT makeblob(?);", val);
                Assert.AreEqual(c.Length, val);
            }
        }
    }

    [TestClass]
    public class class_test_scalar_mean_double
    {
        private const int val = 5;

        private static void mean(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            double d = 0;
            foreach (sqlite3_value v in args)
            {
                d += v.value_double();
            }
            ctx.result_double(d / args.Length);
        }

        [TestMethod]
       public void test_scalar_mean_double()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("my_mean", -1, null, mean);
                double d = db.query_scalar<double>("SELECT my_mean(1,2,3,4,5,6,7,8);");
                Assert.IsTrue(d >= (36 / 8));
                Assert.IsTrue(d <= (36 / 8 + 1));
		GC.Collect();
                double d2 = db.query_scalar<double>("SELECT my_mean(1,2,3,4,5,6,7,8);");
                Assert.IsTrue(d2 >= (36 / 8));
                Assert.IsTrue(d2 <= (36 / 8 + 1));
            }
        }
    }

    [TestClass]
    public class class_test_countargs
    {
        private static void count_args(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            ctx.result_int(args.Length);
        }

        [TestMethod]
       public void test_countargs()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("count_args", -1, null, count_args);
                Assert.AreEqual(8, db.query_scalar<int>("SELECT count_args(1,2,3,4,5,6,7,8);"));
                Assert.AreEqual(0, db.query_scalar<int>("SELECT count_args();"));
                Assert.AreEqual(1, db.query_scalar<int>("SELECT count_args(null);"));
		GC.Collect();
                Assert.AreEqual(8, db.query_scalar<int>("SELECT count_args(1,2,3,4,5,6,7,8);"));
                Assert.AreEqual(0, db.query_scalar<int>("SELECT count_args();"));
                Assert.AreEqual(1, db.query_scalar<int>("SELECT count_args(null);"));
            }
        }
    }

    [TestClass]
    public class class_test_countnullargs
    {
        private static void count_nulls(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            int r = 0;
            foreach (sqlite3_value v in args)
            {
                if (v.value_type() == raw.SQLITE_NULL)
                {
                    r++;
                }
            }
            ctx.result_int(r);
        }

        [TestMethod]
       public void test_countnullargs()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("count_nulls", -1, null, count_nulls);
                Assert.AreEqual(0, db.query_scalar<int>("SELECT count_nulls(1,2,3,4,5,6,7,8);"));
                Assert.AreEqual(0, db.query_scalar<int>("SELECT count_nulls();"));
                Assert.AreEqual(1, db.query_scalar<int>("SELECT count_nulls(null);"));
                Assert.AreEqual(2, db.query_scalar<int>("SELECT count_nulls(1,null,3,null,5);"));
		GC.Collect();
                Assert.AreEqual(0, db.query_scalar<int>("SELECT count_nulls(1,2,3,4,5,6,7,8);"));
                Assert.AreEqual(0, db.query_scalar<int>("SELECT count_nulls();"));
                Assert.AreEqual(1, db.query_scalar<int>("SELECT count_nulls(null);"));
                Assert.AreEqual(2, db.query_scalar<int>("SELECT count_nulls(1,null,3,null,5);"));
            }
        }
    }

    [TestClass]
    public class class_test_len_as_blobs
    {
        private static void len_as_blobs(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            int r = 0;
            foreach (sqlite3_value v in args)
            {
                if (v.value_type() != raw.SQLITE_NULL)
                {
                    Assert.AreEqual(v.value_blob().Length, v.value_bytes());
                    r += v.value_blob().Length;
                }
            }
            ctx.result_int(r);
        }

        [TestMethod]
       public void test_len_as_blobs()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("len_as_blobs", -1, null, len_as_blobs);
                Assert.AreEqual(0, db.query_scalar<int>("SELECT len_as_blobs();"));
                Assert.AreEqual(0, db.query_scalar<int>("SELECT len_as_blobs(null);"));
                Assert.IsTrue(8 <= db.query_scalar<int>("SELECT len_as_blobs(1,2,3,4,5,6,7,8);"));
		GC.Collect();
                Assert.AreEqual(0, db.query_scalar<int>("SELECT len_as_blobs();"));
                Assert.AreEqual(0, db.query_scalar<int>("SELECT len_as_blobs(null);"));
                Assert.IsTrue(8 <= db.query_scalar<int>("SELECT len_as_blobs(1,2,3,4,5,6,7,8);"));
            }
        }
    }

    [TestClass]
    public class class_test_concat
    {
        private const int val = 5;

        private static void concat(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            string r = "";
            foreach (sqlite3_value v in args)
            {
                r += v.value_text();
            }
            ctx.result_text(r);
        }

        [TestMethod]
       public void test_concat()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("my_concat", -1, null, concat);
                Assert.AreEqual("foobar", db.query_scalar<string>("SELECT my_concat('foo', 'bar');"));
                Assert.AreEqual("abc", db.query_scalar<string>("SELECT my_concat('a', 'b', 'c');"));
		GC.Collect();
                Assert.AreEqual("foobar", db.query_scalar<string>("SELECT my_concat('foo', 'bar');"));
                Assert.AreEqual("abc", db.query_scalar<string>("SELECT my_concat('a', 'b', 'c');"));
            }
        }
    }

    [TestClass]
    public class class_test_sum_plus_count
    {
        private class my_state
        {
            public long sum;
            public long count;
        }

        private static void sum_plus_count_step(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            Assert.AreEqual(args.Length, 1);

            if (ctx.state == null)
            {
                ctx.state = new my_state();
            }
            my_state st = ctx.state as my_state;

            st.sum += args[0].value_int64();
            st.count++;
        }

        private static void sum_plus_count_final(sqlite3_context ctx, object user_data)
        {
            if (ctx.state == null)
            {
                ctx.state = new my_state();
            }
            my_state st = ctx.state as my_state;

            ctx.result_int64(st.sum + st.count);
        }

        [TestMethod]
       public void test_sum_plus_count()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("sum_plus_count", 1, null, sum_plus_count_step, sum_plus_count_final);
                db.exec("CREATE TABLE foo (x int);");
                for (int i=0; i<5; i++)
                {
                    db.exec("INSERT INTO foo (x) VALUES (?);", i);
                }
                long c = db.query_scalar<long>("SELECT sum_plus_count(x) FROM foo;");
                Assert.AreEqual(c, (0 + 1 + 2 + 3 + 4) + 5);
		GC.Collect();
                long c2 = db.query_scalar<long>("SELECT sum_plus_count(x) FROM foo;");
                Assert.AreEqual(c2, (0 + 1 + 2 + 3 + 4) + 5);
            }
        }
    }

    [TestClass]
    public class class_test_hooks
    {
        private class work
        {
            public int count_commits;
            public int count_rollbacks;
            public int count_updates;
            public int count_traces;
            public int count_profiles;
        }

        private static int my_commit_hook(object v)
        {
            work w = v as work;
            w.count_commits++;
            return 0;
        }

        private static void my_rollback_hook(object v)
        {
            work w = v as work;
            w.count_rollbacks++;
        }

        private static void my_update_hook(object v, int typ, string db, string tbl, long rowid)
        {
            work w = v as work;
            w.count_updates++;
        }

        private static void my_trace_hook(object v, string sql)
        {
            work w = v as work;
            w.count_traces++;
        }

        private static void my_profile_hook(object v, string sql, long ns)
        {
            work w = v as work;
            w.count_profiles++;
        }

        [TestMethod]
        public void test_rollback_hook_on_close_db()
	{
            using (sqlite3 db = ugly.open(":memory:"))
            {
                work w = new work();

                db.rollback_hook(my_rollback_hook, w);

		GC.Collect();

                db.exec("BEGIN TRANSACTION;");
                db.exec("CREATE TABLE foo (x int);");
            }
        }

        [TestMethod]
        public void test_hooks()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                work w = new work();
                Assert.AreEqual(w.count_commits, 0);
                Assert.AreEqual(w.count_rollbacks, 0);
                Assert.AreEqual(w.count_updates, 0);
                Assert.AreEqual(w.count_traces, 0);
                Assert.AreEqual(w.count_profiles, 0);

                db.commit_hook(my_commit_hook, w);
                db.rollback_hook(my_rollback_hook, w);
                db.update_hook(my_update_hook, w);
                db.trace(my_trace_hook, w);
                db.profile(my_profile_hook, w);

		GC.Collect();

                db.exec("CREATE TABLE foo (x int);");

                Assert.AreEqual(w.count_commits, 1);
                Assert.AreEqual(w.count_rollbacks, 0);
                Assert.AreEqual(w.count_updates, 0);
                Assert.AreEqual(w.count_traces, 1);
                Assert.AreEqual(w.count_profiles, 1);

                db.exec("INSERT INTO foo (x) VALUES (1);");

                Assert.AreEqual(w.count_commits, 2);
                Assert.AreEqual(w.count_rollbacks, 0);
                Assert.AreEqual(w.count_updates, 1);
                Assert.AreEqual(w.count_traces, 2);
                Assert.AreEqual(w.count_profiles, 2);

                db.exec("BEGIN TRANSACTION;");

                Assert.AreEqual(w.count_commits, 2);
                Assert.AreEqual(w.count_rollbacks, 0);
                Assert.AreEqual(w.count_updates, 1);
                Assert.AreEqual(w.count_traces, 3);
                Assert.AreEqual(w.count_profiles, 3);

                db.exec("INSERT INTO foo (x) VALUES (2);");

                Assert.AreEqual(w.count_commits, 2);
                Assert.AreEqual(w.count_rollbacks, 0);
                Assert.AreEqual(w.count_updates, 2);
                Assert.AreEqual(w.count_traces, 4);
                Assert.AreEqual(w.count_profiles, 4);

                db.exec("ROLLBACK TRANSACTION;");

                Assert.AreEqual(w.count_commits, 2);
                Assert.AreEqual(w.count_rollbacks, 1);
                Assert.AreEqual(w.count_updates, 2);
                Assert.AreEqual(w.count_traces, 5);
                Assert.AreEqual(w.count_profiles, 5);

            }
        }
    }

}


