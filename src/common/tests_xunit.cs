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
        public static byte[] to_utf8(string s)
        {
            var ba = new byte[Encoding.UTF8.GetByteCount(s)];
            Encoding.UTF8.GetBytes(s, 0, s.Length, ba, 0);
            return ba;
        }
    }

    [Collection("Init")]
    public class test_cases
    {
        [Fact]
        public void test_call_sqlite3_enable_load_extension()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                var rc = raw.sqlite3_enable_load_extension(db, 0);
                Assert.Equal(raw.SQLITE_OK, rc);
            }
        }

        [Fact]
        public void test_authorizer_with_unregister()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");

                db.exec("CREATE VIEW TEST_VIEW AS SELECT * FROM foo;");

                strdelegate_authorizer denied_authorizer =
                    (object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view) =>
                        {
                            return raw.SQLITE_DENY;
                        };

                raw.sqlite3_set_authorizer(db, denied_authorizer, null);

                var e = Assert.Throws<ugly.sqlite3_exception>(
                    () => db.exec("SELECT * FROM TEST_VIEW;")
                    );
                Assert.Equal(raw.SQLITE_AUTH, e.errcode);

                strdelegate_authorizer no_auth = null;
                raw.sqlite3_set_authorizer(db, no_auth, null);
                db.exec("SELECT * FROM TEST_VIEW;");
            }
        }

        [Fact]
        public void test_rollback_hook_with_unregister()
        {
            using (var db = ugly.open(":memory:"))
            {
                int count = 0;
                db.rollback_hook(v => count += 1, null);
                Assert.Equal(0, count);
                db.exec("CREATE TABLE foo (b int);");
                Assert.Equal(0, count);
                db.exec("INSERT INTO foo (b) VALUES (?)", 37);
                Assert.Equal(0, count);
                db.rollback_hook(null, null);
                db.exec("INSERT INTO foo (b) VALUES (?)", 42);
                Assert.Equal(0, count);

                db.rollback_hook(v => count += 1, null);
                db.exec("BEGIN TRANSACTION");
                Assert.Equal(0, count);
                db.exec("INSERT INTO foo (b) VALUES (?)", 37);
                Assert.Equal(0, count);
                db.exec("ROLLBACK");
                Assert.Equal(1, count);

                db.rollback_hook(null, null);
            }
        }

        [Fact]
        public void test_update_hook_with_unregister()
        {
            using (var db = ugly.open(":memory:"))
            {
                int count = 0;
                db.update_hook((v, typ, dbname, tbl, rowid) => count += 1, null);
                Assert.Equal(0, count);
                db.exec("CREATE TABLE foo (b int);");
                Assert.Equal(0, count);
                db.exec("INSERT INTO foo (b) VALUES (?)", 37);
                Assert.Equal(1, count);
                db.update_hook(null, null);
                db.exec("INSERT INTO foo (b) VALUES (?)", 42);
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public void test_commit_hook_with_unregister()
        {
            using (var db = ugly.open(":memory:"))
            {
                int count = 0;
                db.commit_hook(v => { count++; return 0; }, null);
                Assert.Equal(0, count);
                db.exec("CREATE TABLE foo (b int);");
                Assert.Equal(1, count);
                db.exec("INSERT INTO foo (b) VALUES (?)", 37);
                Assert.Equal(2, count);
                db.commit_hook(null, null);
                db.exec("INSERT INTO foo (b) VALUES (?)", 42);
                Assert.Equal(2, count);
            }
        }

        [Fact]
        public void test_trace_with_unregister()
        {
            using (var db = ugly.open(":memory:"))
            {
                int count = 0;
                db.trace((v, sql) => count += 1, null);
                Assert.Equal(0, count);
                db.exec("CREATE TABLE foo (b int);");
                Assert.Equal(1, count);
                db.exec("INSERT INTO foo (b) VALUES (?)", 37);
                Assert.Equal(2, count);
                db.trace(null, null);
                db.exec("INSERT INTO foo (b) VALUES (?)", 42);
                Assert.Equal(2, count);
            }
        }

        [Fact]
        public void test_profile_with_unregister()
        {
            using (var db = ugly.open(":memory:"))
            {
                int count = 0;
                db.profile((v, sql, ns) => count += 1, null);
                Assert.Equal(0, count);
                db.exec("CREATE TABLE foo (b int);");
                Assert.Equal(1, count);
                db.exec("INSERT INTO foo (b) VALUES (?)", 37);
                Assert.Equal(2, count);
                db.profile(null, null);
                db.exec("INSERT INTO foo (b) VALUES (?)", 42);
                Assert.Equal(2, count);
            }
        }

        [Fact]
        public void CreateCollation_nocase()
        {
            using (var db = ugly.open(":memory:"))
            {
                db.create_collation("MY_NOCASE", null, (v, s1, s2) => string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase));

                var b = db.query_scalar<long>("SELECT 'Νικοσ' = 'ΝΙΚΟΣ' COLLATE MY_NOCASE;");
                Assert.True(b != 0);
            }
        }

        [Fact]
        public void CreateCollation_with_null_comparer_works()
        {
            using (var db = ugly.open(":memory:"))
            {
                db.create_collation("MY_NOCASE", null, (v, s1, s2) => string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase));
                db.create_collation("MY_NOCASE", null, null);

                var ex = Assert.Throws<ugly.sqlite3_exception>(
                    () => db.query_scalar<long>("SELECT 'Νικοσ' = 'ΝΙΚΟΣ' COLLATE MY_NOCASE;"));

                Assert.Equal(raw.SQLITE_ERROR, ex.errcode);
                Assert.Equal("no such collation sequence: MY_NOCASE", ex.errmsg);
            }
        }

        [Fact]
        public void test_native_library_name()
        {
            Assert.NotNull(raw.GetNativeLibraryName());
        }

        [Fact]
        public void test_prepare_null_sql()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                string s = null;
                var rc = raw.sqlite3_prepare_v2(db, s, out var stmt);
                Assert.Equal(raw.SQLITE_MISUSE, rc);
            }
        }

        [Fact]
        public void test_prepare_v2_tail_span()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                var s_valid = "CREATE TABLE foo (x int)";
                var s_bad = " and this is an error";
                var s = s_valid + s_bad;

                var ba = new byte[Encoding.UTF8.GetByteCount(s)];
                Encoding.UTF8.GetBytes(s, 0, s.Length, ba, 0);
                var span_all = ba.AsSpan();

                var rc = raw.sqlite3_prepare_v2(db, span_all, out var _);
                Assert.Equal(raw.SQLITE_ERROR, rc);

                // because all the chars are ASCII, the length of
                // of the string will be the same
                // as the number of bytes in its utf8 representation.

                rc = raw.sqlite3_prepare_v2(db, span_all.Slice(0, s_valid.Length), out var stmt, out var tail);
                using (stmt)
                {
                    Assert.Equal(0, rc);
                    Assert.Equal(0, tail.Length);
                }
            }
        }

        [Fact]
        public void test_prepare_v3_tail_span()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                var s_valid = "CREATE TABLE foo (x int)";
                var s_bad = " and this is an error";
                var s = s_valid + s_bad;

                var ba = new byte[Encoding.UTF8.GetByteCount(s)];
                Encoding.UTF8.GetBytes(s, 0, s.Length, ba, 0);
                var span_all = ba.AsSpan();

                var rc = raw.sqlite3_prepare_v3(db, span_all, 0, out var _);
                Assert.Equal(raw.SQLITE_ERROR, rc);

                // because all the chars are ASCII, the length of
                // of the string will be the same
                // as the number of bytes in its utf8 representation.

                rc = raw.sqlite3_prepare_v3(db, span_all.Slice(0, s_valid.Length), 0, out var stmt, out var tail);
                using (stmt)
                {
                    Assert.Equal(0, rc);
                    Assert.Equal(0, tail.Length);
                }
            }
        }

        [Fact]
        public void test_prepare_tail_string()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                var s = "CREATE TABLE foo (x int)";

                var rc = raw.sqlite3_prepare_v2(db, s, out var stmt, out var tail);
                using (stmt)
                {
                    Assert.Equal(0, rc);
                    Assert.Equal(0, tail.Length);
                }
            }
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

                    Assert.Equal(6, stmt.bind_parameter_count());

                    Assert.Equal(0, stmt.bind_parameter_index(":m"));

                    Assert.Equal(1, stmt.bind_parameter_index(":x"));
                    Assert.Equal(2, stmt.bind_parameter_index(":v"));
                    Assert.Equal(3, stmt.bind_parameter_index(":t"));
                    Assert.Equal(4, stmt.bind_parameter_index(":d"));
                    Assert.Equal(5, stmt.bind_parameter_index(":b"));
                    Assert.Equal(6, stmt.bind_parameter_index(":q"));

                    Assert.Equal(":x", stmt.bind_parameter_name(1));
                    Assert.Equal(":v", stmt.bind_parameter_name(2));
                    Assert.Equal(":t", stmt.bind_parameter_name(3));
                    Assert.Equal(":d", stmt.bind_parameter_name(4));
                    Assert.Equal(":b", stmt.bind_parameter_name(5));
                    Assert.Equal(":q", stmt.bind_parameter_name(6));
                }
            }
        }

        [Fact]
        public void test_blob_reopen()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                const int len_1 = 42;
                const int len_2 = 87;

                db.exec("CREATE TABLE foo (b blob);");
                db.exec("INSERT INTO foo (b) VALUES (randomblob(?))", len_1);
                long rowid_1 = db.last_insert_rowid();
                db.exec("INSERT INTO foo (b) VALUES (randomblob(?))", len_2);
                long rowid_2 = db.last_insert_rowid();

                byte[] blob_1 = db.query_scalar<byte[]>("SELECT b FROM foo WHERE rowid=?;", rowid_1);
                Assert.Equal(blob_1.Length, len_1);

                byte[] blob_2 = db.query_scalar<byte[]>("SELECT b FROM foo WHERE rowid=?;", rowid_2);
                Assert.Equal(blob_2.Length, len_2);

                Func<sqlite3_blob, byte[], bool> Check =
                (bh, ba) =>
                {
                    int len = bh.bytes();
                    if (len != ba.Length)
                    {
                        return false;
                    }

                    byte[] buf = new byte[len];

                    bh.read(buf, 0);
                    for (int i = 0; i < len; i++)
                    {
                        if (ba[i] != buf[i])
                        {
                            return false;
                        }
                    }
                    return true;
                };

                using (sqlite3_blob bh = db.blob_open("main", "foo", "b", rowid_1, 0))
                {
                    Assert.True(Check(bh, blob_1));
                    Assert.False(Check(bh, blob_2));

                    bh.reopen(rowid_2);

                    Assert.False(Check(bh, blob_1));
                    Assert.True(Check(bh, blob_2));
                }
            }

        }

        [Fact]
        public void test_blob_read()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                const int len = 100;

                db.exec("CREATE TABLE foo (b blob);");
                db.exec("INSERT INTO foo (b) VALUES (randomblob(?))", len);
                long rowid = db.last_insert_rowid();

                byte[] blob = db.query_scalar<byte[]>("SELECT b FROM foo;");
                Assert.Equal(blob.Length, len);

                using (sqlite3_blob bh = db.blob_open("main", "foo", "b", rowid, 0))
                {
                    int len2 = bh.bytes();
                    Assert.Equal(len, len2);

                    int passes = 10;

                    Assert.Equal(0, len % passes);

                    int sublen = len / passes;
                    byte[] buf = new byte[sublen];

                    for (int q = 0; q < passes; q++)
                    {
                        bh.read(buf, q * sublen);

                        for (int i = 0; i < sublen; i++)
                        {
                            Assert.Equal(blob[q * sublen + i], buf[i]);
                        }
                    }
                }
            }

        }

        [Fact]
        public void test_blob_read_with_byte_array_offset()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                const int len = 100;

                db.exec("CREATE TABLE foo (b blob);");
                db.exec("INSERT INTO foo (b) VALUES (zeroblob(?))", len);
                long rowid = db.last_insert_rowid();

                byte[] blob = db.query_scalar<byte[]>("SELECT b FROM foo;");
                Assert.Equal(blob.Length, len);

                using (sqlite3_blob bh = db.blob_open("main", "foo", "b", rowid, 0))
                {
                    int len2 = bh.bytes();
                    Assert.Equal(len, len2);

                    byte[] blob2 = new byte[len];
                    for (int i = 0; i < len; i++)
                    {
                        blob2[i] = 73;
                    }

                    var sp = new Span<byte>(blob2, 40, 20);
                    bh.read(sp, 40);

                    for (int i = 0; i < 40; i++)
                    {
                        Assert.Equal(73, blob2[i]);
                    }

                    for (int i = 40; i < 60; i++)
                    {
                        Assert.Equal(0, blob2[i]);
                    }

                    for (int i = 60; i < 100; i++)
                    {
                        Assert.Equal(73, blob2[i]);
                    }
                }
            }

        }

        [Fact]
        public void test_blob_write_with_byte_array_offset()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                const int len = 100;

                db.exec("CREATE TABLE foo (b blob);");
                db.exec("INSERT INTO foo (b) VALUES (zeroblob(?))", len);
                long rowid = db.last_insert_rowid();

                byte[] blob = db.query_scalar<byte[]>("SELECT b FROM foo;");
                Assert.Equal(blob.Length, len);

                for (int i = 0; i < 100; i++)
                {
                    Assert.Equal(0, blob[i]);
                }

                using (sqlite3_blob bh = db.blob_open("main", "foo", "b", rowid, 1))
                {
                    int len2 = bh.bytes();
                    Assert.Equal(len, len2);

                    byte[] blob2 = new byte[len];
                    for (int i = 0; i < 100; i++)
                    {
                        blob2[i] = 73;
                    }

                    var sp = new ReadOnlySpan<byte>(blob2, 40, 20);
                    bh.write(sp, 50);
                }

                byte[] blob3 = db.query_scalar<byte[]>("SELECT b FROM foo;");
                Assert.Equal(len, blob3.Length);

                for (int i = 0; i < 50; i++)
                {
                    Assert.Equal(0, blob3[i]);
                }

                for (int i = 50; i < 70; i++)
                {
                    Assert.Equal(73, blob3[i]);
                }

                for (int i = 70; i < 100; i++)
                {
                    Assert.Equal(0, blob3[i]);
                }

            }

        }

        [Fact]
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
                    Assert.Equal(len, len2);

                    int passes = 10;

                    Assert.Equal(0, len % passes);

                    int sublen = len / passes;
                    byte[] buf = new byte[sublen];
                    for (int i = 0; i < sublen; i++)
                    {
                        buf[i] = (byte)(i % 256);
                    }

                    for (int q = 0; q < passes; q++)
                    {
                        bh.write(buf, q * sublen);
                    }
                }
            }
        }

        [Fact]
        public void test_blob_close()
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

                var rc = raw.sqlite3_blob_open(db, "main", "foo", "b", rowid, 1, out var bh);
                Assert.Equal(0, rc);
                Assert.NotNull(bh);

                rc = raw.sqlite3_blob_close(bh);
                Assert.Equal(0, rc);

                bh.Dispose();
            }
        }

        [Fact]
        public void test_db_readonly()
        {
            using (sqlite3 db = ugly.open_v2(":memory:", raw.SQLITE_OPEN_READONLY, null))
            {
                int result = raw.sqlite3_db_readonly(db, "main");
                Assert.True(result > 0);
            }

            using (sqlite3 db = ugly.open(":memory:"))
            {
                int result = raw.sqlite3_db_readonly(db, "main");
                Assert.Equal(0, result);
            }
        }

        [Fact]
        public void test_get_autocommit()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                Assert.Equal(1, db.get_autocommit());
                db.exec("BEGIN TRANSACTION;");
                Assert.True(db.get_autocommit() == 0);
            }
        }

        [Fact]
        public void test_last_insert_rowid()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x text);");
                db.exec("INSERT INTO foo (x) VALUES ('b')");
                Assert.Equal(1, db.last_insert_rowid());
            }
        }

        [Fact]
        public void test_prepare_v2_overload()
        {
            var libversion = raw.sqlite3_libversion().utf8_to_string();
            using (sqlite3 db = ugly.open(":memory:"))
            {
                // prepare a stmt with raw, not ugly, no tail provided
                var rc = raw.sqlite3_prepare_v2(db, "SELECT sqlite_version()", out var stmt);
                Assert.Equal(0, rc);

                // make sure the stmt works
                stmt.step_row();
                var s = stmt.column_text(0);
                Assert.Equal(libversion, s);

                // finalize it manually
                rc = raw.sqlite3_finalize(stmt);
                Assert.Equal(0, rc);

                // make sure it's okay to Dispose even though finalize was called
                stmt.Dispose();
            }
        }

        [Fact]
        public void test_prepare_v3_overload()
        {
            // identical to v2 version of this test
            var libversion = raw.sqlite3_libversion().utf8_to_string();
            using (sqlite3 db = ugly.open(":memory:"))
            {
                // prepare a stmt with raw, not ugly, no tail provided
                var rc = raw.sqlite3_prepare_v3(db, "SELECT sqlite_version()", 0, out var stmt);
                Assert.Equal(0, rc);

                // make sure the stmt works
                stmt.step_row();
                var s = stmt.column_text(0);
                Assert.Equal(libversion, s);

                // finalize it manually
                rc = raw.sqlite3_finalize(stmt);
                Assert.Equal(0, rc);

                // make sure it's okay to Dispose even though finalize was called
                stmt.Dispose();
            }
        }

        [Fact]
        public void test_prepare_v3()
        {
            var libversion = raw.sqlite3_libversion().utf8_to_string();
            using (sqlite3 db = ugly.open(":memory:"))
            {
                using (sqlite3_stmt stmt = db.prepare_v3("SELECT sqlite_version()", 0))
                {
                    stmt.step_row();
                    var s = stmt.column_text(0);
                    Assert.Equal(libversion, s);
                }
            }
        }

        [Fact]
        public void test_libversion()
        {
            string sourceid = raw.sqlite3_sourceid().utf8_to_string();
            Assert.True(sourceid != null);
            Assert.True(sourceid.Length > 0);

            string libversion = raw.sqlite3_libversion().utf8_to_string();
            Assert.True(libversion != null);
            Assert.True(libversion.Length > 0);
            Assert.Equal('3', libversion[0]);

            int libversion_number = raw.sqlite3_libversion_number();
            Assert.Equal(3, libversion_number / 1000000);
        }

        [Fact]
        public void test_threadsafe()
        {
            int ret = raw.sqlite3_threadsafe();
            Assert.True(ret != 0);
        }

        [Fact]
        public void test_enable_shared_cache()
        {
            int result = raw.sqlite3_enable_shared_cache(1);
            Assert.Equal(raw.SQLITE_OK, result);

            result = raw.sqlite3_enable_shared_cache(0);
            Assert.Equal(raw.SQLITE_OK, result);
        }

        [Fact]
        public void test_sqlite3_memory()
        {
            long memory_used = raw.sqlite3_memory_used();
            long memory_highwater = raw.sqlite3_memory_highwater(0);
#if not
            // these asserts fail on the iOS builtin sqlite.  not sure
            // why.  not sure the asserts are worth doing anyway.
            Assert.True(memory_used > 0);
            Assert.True(memory_highwater >= memory_used);
#endif
        }

        [Fact]
        public void test_sqlite3_status()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                int current;
                int highwater;
                ugly.sqlite3_status(raw.SQLITE_STATUS_MEMORY_USED, out current, out highwater, 0);

                Assert.True(current > 0);
                Assert.True(highwater > 0);
            }
        }

        [Fact]
        public void test_exec_overload_plain()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                {
                    var rc = raw.sqlite3_exec(db, "CREATE TABLE foo (x int);");
                    Assert.Equal(0, rc);
                }
                {
                    var rc = raw.sqlite3_exec(db, "CREATE CREATE ((");
                    Assert.Equal(1, rc);
                }
            }
        }

        [Fact]
        public void test_exec_overload_errmsg()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                {
                    var rc = raw.sqlite3_exec(db, "CREATE TABLE foo (x int);", out var e);
                    Assert.Equal(0, rc);
                    Assert.Null(e);
                }
                {
                    var rc = raw.sqlite3_exec(db, "CREATE CREATE ((", out var e);
                    Assert.Equal(1, rc);
                    Assert.NotNull(e);
                }
            }
        }

        [Fact]
        public void test_exec_callback()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");

                const int count = 13;

                for (int i = 0; i < count; i++)
                {
                    db.exec("INSERT INTO foo (x) VALUES (?)", i);
                }

                int count_cb = 0;
                strdelegate_exec cb =
                    (user_data, values, names) =>
                    {
                        count_cb++;
                        return 0;
                    };

                raw.sqlite3_exec(db, "SELECT * from foo", cb, null, out var errmsg);

                Assert.Equal(count, count_cb);
            }
        }

        [Fact]
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
                        Assert.Equal(0, bak.remaining());
                        Assert.True(bak.pagecount() > 0);
                    }
                }
            }
        }

        [Fact]
        public void test_backup_finish()
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
                        Assert.Equal(0, bak.remaining());
                        Assert.True(bak.pagecount() > 0);

                        bak.finish();
                    }
                }
            }
        }

        [Fact]
        public void test_more_stuff()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
                foreach (var i in Enumerable.Range(1, 40))
                {
                    db.exec("INSERT INTO foo (x) VALUES (?)", i);
                }
                using (sqlite3_stmt stmt = db.prepare("SELECT x from foo"))
                {
                    while (stmt.step() == raw.SQLITE_ROW)
                    {
                        var v = stmt.column_int(0);
                        // TODO do something here?
                        //System.Console.WriteLine("", v);
                    }
                }

            }
        }

        [Fact]
        public void test_compileoption()
        {
            int i = 0;
            while (true)
            {
                string s = raw.sqlite3_compileoption_get(i++).utf8_to_string();
                if (s == null)
                {
                    break;
                }
                int used = raw.sqlite3_compileoption_used(s);
                Assert.True(used != 0);
            }
        }

        [Fact]
        public void test_bernt()
        {
            using (sqlite3 db = ugly.open(""))
            {
                db.exec("CREATE TABLE places_dat (resource_handle TEXT PRIMARY KEY, data BLOB) WITHOUT ROWID;");
                byte[] buf1 = db.query_scalar<byte[]>("SELECT randomblob(16);");
                db.exec("INSERT INTO places_dat VALUES (?,?)", "foo", buf1);
                Assert.Equal(1, db.changes());
                int c1 = db.query_scalar<int>("SELECT COUNT(*) FROM places_dat WHERE resource_handle='foo';");
                Assert.Equal(1, c1);
                byte[] buf2 = new byte[2];
                buf2[0] = 42;
                buf2[1] = 73;
                db.exec("UPDATE places_dat SET data=? WHERE resource_handle=?;", buf2, "foo");
                Assert.Equal(1, db.changes());
                byte[] buf3 = db.query_scalar<byte[]>("SELECT data FROM places_dat WHERE resource_handle='foo';");
                Assert.Equal(2, buf3.Length);
                Assert.Equal(buf2[0], buf3[0]);
                Assert.Equal(buf2[1], buf3[1]);
            }
        }

        [Fact]
        public void test_create_table_temp_db()
        {
            using (sqlite3 db = ugly.open(""))
            {
                db.exec("CREATE TABLE foo (x int);");
            }
        }

        [Fact]
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

        [Fact]
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

                    db.extended_result_codes(0);

                    {
                        int errcode = db.errcode();
                        Assert.Equal(errcode, e.errcode);
                        Assert.Equal(raw.SQLITE_CONSTRAINT, errcode);

                        Assert.Equal(raw.SQLITE_CONSTRAINT_UNIQUE, db.extended_errcode());
                    }

                    db.extended_result_codes(1);

                    {
                        int errcode = db.errcode();
                        Assert.Equal(raw.SQLITE_CONSTRAINT_UNIQUE, errcode);

                        Assert.Equal(raw.SQLITE_CONSTRAINT_UNIQUE, db.extended_errcode());
                    }

                    db.extended_result_codes(0);

                    string errmsg = db.errmsg();
                    Assert.True(errmsg != null);
                    Assert.True(errmsg.Length > 0);
                }
                Assert.True(fail);

                Assert.True(raw.sqlite3_errstr(raw.SQLITE_CONSTRAINT).utf8_to_string() != null);
            }
        }

        [Fact]
        public void test_insert_empty_blob_17494()
        {
            var ka = new byte[] {};
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x blob NOT NULL);");
                db.exec("INSERT INTO foo (x) VALUES(?);", ka);
                var rowid = db.last_insert_rowid();
                var ba = db.query_scalar<byte[]>("SELECT x FROM foo WHERE rowid=" + rowid);
                Assert.Equal(0, ba.Length);
            }
        }

        [Fact]
        public void test_value_blob_issue_289()
        {
            var ka = new byte[]
            {
                37,
                42,
                22,
                198,
                0,
                222,
                16,
                4
            };

            int compare_and_return_sum(byte[] ba)
            {
                Assert.Equal(ka.Length, ba.Length);
                //System.Console.WriteLine($"got ba: {string.Join(",", ba.Select(x => x.ToString()))}");
                int sum = 0;
                for (int i=0; i<ka.Length; i++)
                {
                    Assert.Equal(ka[i], ba[i]);
                    sum += ba[i];
                }
                return sum;
            }

            delegate_function_scalar func289 =
                (ctx, user_data, args) =>
                {
                    var ba = raw.sqlite3_value_blob(args[0]).ToArray();
                    var sum = compare_and_return_sum(ba);
                    raw.sqlite3_result_int(ctx, sum);
                };

            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x blob);");
                db.create_function("func289", 1, null, func289);
                db.exec("INSERT INTO foo (x) VALUES(?);", ka);
                var rowid = db.last_insert_rowid();

                int sum_expected;
                {
                    var tot = 0;
                    foreach (var b in ka)
                    {
                        tot += b;
                    }
                    sum_expected = tot;
                }

                // make sure the blob went into the table correctly
                {
                    var ba = db.query_scalar<byte[]>("SELECT x FROM foo WHERE rowid=" + rowid);
                    var sum_result = compare_and_return_sum(ba);
                    Assert.Equal(sum_expected, sum_result);
                }

                // now call the function to see if the blob arrives properly
                {
                    var sum_result = db.query_scalar<int>("SELECT func289(x) FROM foo;");
                    Assert.Equal(sum_expected, sum_result);
                }
            }
        }

        [Fact]
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
                Assert.Equal(10, blob.Length);
                foreach (var b in blob)
                {
                    Assert.Equal(0, b);
                }
            }
        }

        [Fact]
        public void test_result_null_with_query_scalar()
        {
            delegate_function_scalar null_func =
                (ctx, user_data, args) =>
                {
                    raw.sqlite3_result_null(ctx);
                };

            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x blob);");
                db.create_function("foo", 0, null, null_func);
                db.exec("INSERT INTO foo (x) VALUES(foo());");

                var rowid = db.last_insert_rowid();
                byte[] blob = db.query_scalar<byte[]>("SELECT x FROM foo WHERE rowid=" + rowid);
                Assert.Null(blob);
            }
        }

        [Fact]
        public void test_result_null()
        {
            delegate_function_scalar null_func =
                (ctx, user_data, args) =>
                {
                    raw.sqlite3_result_null(ctx);
                };

            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x blob);");
                db.create_function("foo", 0, null, null_func);
                db.exec("INSERT INTO foo (x) VALUES(foo());");

                var rowid = db.last_insert_rowid();
                using (var stmt = db.prepare("SELECT x FROM foo WHERE rowid=" + rowid))
                {
                    stmt.step_row();
                    Assert.Equal(raw.SQLITE_NULL, stmt.column_type(0));
                }
            }
        }

        [Fact]
        public void test_result_text_string()
        {
            const string MSG = "unless you need to move a piano";
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("foo", 0, null, (ctx, v, args) => raw.sqlite3_result_text(ctx, MSG));
                var s = db.query_scalar<string>("SELECT foo();");
                Assert.Equal(MSG, s);
            }
        }

        [Fact]
        public void test_result_text_span()
        {
            const string MSG = "Captain America";
            var ba = u.to_utf8(MSG);
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("foo", 0, null,
                    (ctx, v, args) =>
                    {
                        var span_all = ba.AsSpan();
                        raw.sqlite3_result_text(ctx, span_all.Slice(0, 3));
                    }
                    );
                var s = db.query_scalar<string>("SELECT foo();");
                Assert.Equal("Cap", s);
            }
        }

        [Fact]
        public void test_result_errors()
        {
            const int code = 10;
            const string MSG = "epic fail";
            const int partlen = 4;

            // ascii string, so char len = utf8 byte len

            delegate_function_scalar errormsg_func =
                (ctx, user_data, args) => raw.sqlite3_result_error(ctx, MSG);

            delegate_function_scalar partial_errormsg_func =
                (ctx, user_data, args) =>
                {
                    var ba = u.to_utf8(MSG);
                    var span_all = ba.AsSpan();
                    raw.sqlite3_result_error(ctx, span_all.Slice(0, partlen));
                };

            delegate_function_scalar errorcode_func =
                (ctx, user_data, args) => raw.sqlite3_result_error_code(ctx, code);

            delegate_function_scalar toobig_func =
                (ctx, user_data, args) => raw.sqlite3_result_error_toobig(ctx);

            delegate_function_scalar nomem_func =
                (ctx, user_data, args) => raw.sqlite3_result_error_nomem(ctx);

            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("errormsg", 0, null, errormsg_func);
                db.create_function("partial_errormsg", 0, null, partial_errormsg_func);
                db.create_function("errorcode", 0, null, errorcode_func);
                db.create_function("toobig", 0, null, toobig_func);
                db.create_function("nomem", 0, null, nomem_func);

                {
                    var e = Assert.Throws<ugly.sqlite3_exception>(
                        () => db.exec("select errormsg();")
                        );
                    Assert.Equal(raw.SQLITE_ERROR, e.errcode);
                    Assert.Equal(MSG, e.errmsg);
                }

                {
                    var e = Assert.Throws<ugly.sqlite3_exception>(
                        () => db.exec("select partial_errormsg();")
                        );
                    Assert.Equal(raw.SQLITE_ERROR, e.errcode);
                    Assert.Equal(MSG.Substring(0, 4), e.errmsg);
                }

                {
                    var e = Assert.Throws<ugly.sqlite3_exception>(
                        () => db.exec("select errorcode();")
                        );
                    Assert.Equal(code, e.errcode);
                }

                {
                    var e = Assert.Throws<ugly.sqlite3_exception>(
                        () => db.exec("select toobig();")
                        );
                    Assert.Equal(raw.SQLITE_TOOBIG, e.errcode);
                }

                {
                    var e = Assert.Throws<ugly.sqlite3_exception>(
                        () => db.exec("select nomem();")
                        );
                    Assert.Equal(raw.SQLITE_NOMEM, e.errcode);
                }
            }
        }

        [Fact]
        public void test_create_table_memory_db()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
            }
        }

        [Fact]
        public void test_open_v2()
        {
            using (sqlite3 db = ugly.open_v2(":memory:", raw.SQLITE_OPEN_READWRITE | raw.SQLITE_OPEN_CREATE, null))
            {
                db.exec("CREATE TABLE foo (x int);");
            }
        }

        [Fact]
        public void test_db_status()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                int current;
                int highwater;

                db.db_status(raw.SQLITE_DBSTATUS_CACHE_USED, out current, out highwater, 0);
                Assert.True(current > 0);
                Assert.Equal(0, highwater);
            }
        }

        [Fact]
        public void test_create_table_explicit_close()
        {
            sqlite3 db = ugly.open(":memory:");
            db.exec("CREATE TABLE foo (x int);");
            db.close();
        }

        [Fact]
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

        [Fact]
        public void test_count()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
                db.exec("INSERT INTO foo (x) VALUES (1);");
                db.exec("INSERT INTO foo (x) VALUES (2);");
                db.exec("INSERT INTO foo (x) VALUES (3);");
                int c = db.query_scalar<int>("SELECT COUNT(*) FROM foo");
                Assert.Equal(3, c);
            }
        }

        [Fact]
        public void test_stmt_complete()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");

                Assert.True(raw.sqlite3_complete("SELECT x FROM") == 0);
                Assert.True(raw.sqlite3_complete("SELECT") == 0);
                Assert.True(raw.sqlite3_complete("INSERT INTO") == 0);
                Assert.True(raw.sqlite3_complete("SELECT x FROM foo") == 0);

                Assert.True(raw.sqlite3_complete("SELECT x FROM foo;") != 0);
                Assert.True(raw.sqlite3_complete("SELECT COUNT(*) FROM foo;") != 0);
                Assert.True(raw.sqlite3_complete("SELECT 5;") != 0);
            }
        }

        [Fact]
        public void test_next_stmt()
        {
            const int ENABLE_DEFAULT = 1;
            const int ENABLE_OFF = 2;
            const int ENABLE_ON = 3;
            void tryit(int enable)
            {
                using (sqlite3 db = ugly.open(":memory:"))
                {
                    switch (enable)
                    {
                        case ENABLE_DEFAULT:
                            // do nothing
                            break;
                        case ENABLE_OFF:
                            db.enable_sqlite3_next_stmt(false);
                            break;
                        case ENABLE_ON:
                            db.enable_sqlite3_next_stmt(true);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    Assert.Null(db.next_stmt(null));
                    using (sqlite3_stmt stmt = db.prepare("SELECT 5;"))
                    {
                        Assert.Equal(stmt, db.next_stmt(null));
                        Assert.Null(db.next_stmt(stmt));
                    }
                    Assert.Null(db.next_stmt(null));
                }
            }

            void should_throw(Action f, string err_should_contain)
            {
                var e = Assert.Throws<Exception>(f);
                Assert.Contains(err_should_contain, e.ToString());
            }

            var msg_should_contain = "is disabled.  To enable it, call sqlite3.enable_sqlite3_next_stmt(true)";
            should_throw(
                () => tryit(ENABLE_DEFAULT),
                msg_should_contain
                );
            should_throw(
                () => tryit(ENABLE_OFF),
                msg_should_contain
                );
            tryit(ENABLE_ON);
        }

        [Fact]
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
                    Assert.Equal(sql, stmt.sql());

                    Assert.Equal(0, stmt.stmt_busy());
                    stmt.step();
                    Assert.True(stmt.stmt_busy() != 0);
                    stmt.step();
                    Assert.True(stmt.stmt_busy() != 0);
                    stmt.step();
                    Assert.True(stmt.stmt_busy() != 0);
                    stmt.step();
                    Assert.True(stmt.stmt_busy() == 0);
                }
            }
        }

        [Fact]
        public void test_stmt_isexplain()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
                db.exec("INSERT INTO foo (x) VALUES (1);");
                string sql = "SELECT x FROM foo";
                using (sqlite3_stmt stmt = db.prepare(sql))
                {
                    Assert.Equal(sql, stmt.sql());

                    Assert.Equal(0, stmt.stmt_isexplain());
                }
                
                sql = "EXPLAIN SELECT x FROM foo";
                using (sqlite3_stmt stmt = db.prepare(sql))
                {
                    Assert.Equal(sql, stmt.sql());

                    Assert.Equal(1, stmt.stmt_isexplain());
                }
                
                sql = "EXPLAIN QUERY PLAN SELECT x FROM foo";
                using (sqlite3_stmt stmt = db.prepare(sql))
                {
                    Assert.Equal(sql, stmt.sql());

                    Assert.Equal(2, stmt.stmt_isexplain());
                }
            }
        }

        [Fact]
        public void test_stmt_status()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");

                using (var stmt = db.prepare("SELECT x FROM foo"))
                {
                    stmt.step();

                    int vmStep = raw.sqlite3_stmt_status(stmt, raw.SQLITE_STMTSTATUS_VM_STEP, 0);
                    Assert.True(vmStep > 0);

                    int vmStep2 = raw.sqlite3_stmt_status(stmt, raw.SQLITE_STMTSTATUS_VM_STEP, 1);
                    Assert.Equal(vmStep, vmStep2);

                    int vmStep3 = raw.sqlite3_stmt_status(stmt, raw.SQLITE_STMTSTATUS_VM_STEP, 0);
                    Assert.Equal(0, vmStep3);
                }
            }
        }

        [Fact]
        public void test_total_changes()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                Assert.Equal(0, db.total_changes());
                Assert.Equal(0, db.changes());

                db.exec("CREATE TABLE foo (x int);");
                Assert.Equal(0, db.total_changes());
                Assert.Equal(0, db.changes());

                db.exec("INSERT INTO foo (x) VALUES (1);");
                Assert.Equal(1, db.total_changes());
                Assert.Equal(1, db.changes());

                db.exec("INSERT INTO foo (x) VALUES (2);");
                db.exec("INSERT INTO foo (x) VALUES (3);");
                Assert.Equal(3, db.total_changes());
                Assert.Equal(1, db.changes());

                db.exec("UPDATE foo SET x=5;");
                Assert.Equal(6, db.total_changes());
                Assert.Equal(3, db.changes());
            }
        }

        [Fact]
        public void test_bind_text()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x text);");
                const string s = "hello world";
                var ba = new byte[Encoding.UTF8.GetByteCount(s)];
                Encoding.UTF8.GetBytes(s, 0, s.Length, ba, 0);
                var span_all = ba.AsSpan();
                using (sqlite3_stmt stmt = db.prepare("INSERT INTO foo (x) VALUES (?)"))
                {
                    var slice = span_all.Slice(0, 5);
                    stmt.bind_text(1, slice);
                    stmt.step();
                }
                using (sqlite3_stmt stmt = db.prepare("SELECT x FROM foo"))
                {
                    stmt.step_row();
                    var s2 = stmt.column_text(0);
                    Assert.Equal("hello", s2);
                }
            }
        }

        [Fact]
        public void test_explicit_prepare()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int);");
                const int num = 7;
                using (sqlite3_stmt stmt = db.prepare("INSERT INTO foo (x) VALUES (?)"))
                {
                    for (int i = 0; i < num; i++)
                    {
                        stmt.reset();
                        stmt.clear_bindings();
                        stmt.bind(1, i);
                        stmt.step();
                    }
                }
                int c = db.query_scalar<int>("SELECT COUNT(*) FROM foo");
                Assert.Equal(num, c);
            }
        }

        [Fact]
        public void test_exec_two_with_tail()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                string errmsg;
                db.exec("CREATE TABLE foo (x int);INSERT INTO foo (x) VALUES (1);", null, null, out errmsg);
                int c = db.query_scalar<int>("SELECT COUNT(*) FROM foo");
                Assert.Equal(1, c);
            }
        }

        [Fact]
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

                        Assert.True(stmt.stmt_readonly() != 0);

                        Assert.Equal("main", stmt.column_database_name(0));
                        Assert.Equal("foo", stmt.column_table_name(0));
                        Assert.Equal("x", stmt.column_origin_name(0));
                        Assert.Equal("mario", stmt.column_name(0));
                        Assert.Equal("int", stmt.column_decltype(0));
                    }
                }
            }
        }

        [Fact]
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

                Assert.Equal(2, logSize);
                Assert.Equal(2, framesCheckPointed);

                // Set autocheckpoint to 1 so that regardless of the number of 
                // commits, explicit checkpoints only checkpoint the last update.
                db.wal_autocheckpoint(1);

                db.exec("INSERT INTO foo (x) VALUES (3);");
                db.exec("INSERT INTO foo (x) VALUES (4);");
                db.exec("INSERT INTO foo (x) VALUES (5);");
                db.wal_checkpoint("main", raw.SQLITE_CHECKPOINT_PASSIVE, out logSize, out framesCheckPointed);

                Assert.Equal(1, logSize);
                Assert.Equal(1, framesCheckPointed);
            }

            ugly.vfs__delete(null, tmpFile, 1);
        }

        [Fact]
        public void test_set_authorizer()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                var data = new Object();

                strdelegate_authorizer authorizer =
                    (object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view) =>
                        {
                            Assert.Equal(data, user_data);

                            switch (action_code)
                            {
                                // When creating a table an insert is first done.
                                case raw.SQLITE_INSERT:
                                    Assert.Equal("sqlite_master", param0);
                                    Assert.Null(param1);
                                    Assert.Equal("main", dbName);
                                    Assert.Null(inner_most_trigger_or_view);
                                    break;
                                case raw.SQLITE_CREATE_TABLE:
                                    Assert.Equal("foo", param0);
                                    Assert.Null(param1);
                                    Assert.Equal("main", dbName);
                                    Assert.Null(inner_most_trigger_or_view);
                                    break;
                                case raw.SQLITE_READ:
                                    Assert.NotNull(param0);
                                    Assert.NotNull(param1);
                                    Assert.Equal("main", dbName);
                                    Assert.Null(inner_most_trigger_or_view);
                                    break;
                            }

                            return raw.SQLITE_OK;
                        };

                raw.sqlite3_set_authorizer(db, authorizer, data);
                db.exec("CREATE TABLE foo (x int);");

                GC.Collect();

                db.exec("SELECT * FROM foo;");
                db.exec("CREATE VIEW TEST_VIEW AS SELECT * FROM foo;");

                strdelegate_authorizer view_authorizer =
                    (object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view) =>
                        {
                            switch (action_code)
                            {
                                case raw.SQLITE_READ:
                                    Assert.NotNull(param0);
                                    Assert.NotNull(param1);
                                    Assert.Equal("main", dbName);

                                    // A Hack. Goal is to prove that inner_most_trigger_or_view is not null when it is returned in the callback
                                    if (param0 == "foo") { Assert.NotNull(inner_most_trigger_or_view); }
                                    break;
                            }

                            return raw.SQLITE_OK;
                        };

                raw.sqlite3_set_authorizer(db, view_authorizer, data);
                db.exec("SELECT * FROM TEST_VIEW;");

                strdelegate_authorizer denied_authorizer =
                    (object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view) =>
                        {
                            return raw.SQLITE_DENY;
                        };

                raw.sqlite3_set_authorizer(db, denied_authorizer, data);

                var e = Assert.Throws<ugly.sqlite3_exception>(
                    () => db.exec("SELECT * FROM TEST_VIEW;")
                    );
                Assert.Equal(raw.SQLITE_AUTH, e.errcode);
            }
        }

        [Fact]
        public void test_column_null()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int, t text);");
                db.exec("INSERT INTO foo (x) VALUES (?)", 32);
                using (sqlite3_stmt stmt = db.prepare("SELECT x,t FROM foo;"))
                {
                    stmt.step();

                    Assert.Equal(32, stmt.column_int(0));
                    var t = stmt.column_text(1);
                    Assert.Null(t);
                }
            }
        }

        [Fact]
        public void test_column_empty_string()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int, t text);");
                db.exec("INSERT INTO foo (x,t) VALUES (?,?)", 32, "");
                using (sqlite3_stmt stmt = db.prepare("SELECT x,t FROM foo;"))
                {
                    stmt.step();

                    Assert.Equal(32, stmt.column_int(0));
                    var t = stmt.column_text(1);
                    Assert.NotNull(t);
                    Assert.Equal(0, t.Length);
                }
            }
        }

        [Fact]
        public void test_other_col_types()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (id int, n int, r real);");
                db.exec("INSERT INTO foo (id,n,r) VALUES (?,?,?)", 1, 32, 3.14);
                db.exec("INSERT INTO foo (id,n,r) VALUES (?,?,?)", 2, 44, null);
                db.exec("INSERT INTO foo (id,n,r) VALUES (?,?,?)", 3, null, 1.414);
                db.exec("INSERT INTO foo (id,n,r) VALUES (?,?,?)", 4, 0, null);
                db.exec("INSERT INTO foo (id,n,r) VALUES (?,?,?)", 5, new DateTime(1968, 3, 15), null);
                using (sqlite3_stmt stmt = db.prepare("SELECT n,r FROM foo WHERE id=1;"))
                {
                    stmt.step();

                    Assert.Equal(32, stmt.column_int(0));
                    Assert.Equal(3.14, stmt.column_double(1));

                    {
                        var v = stmt.column<int?>(0);
                        Assert.True(v.HasValue);
                        Assert.Equal(32, v);
                    }

                    {
                        var v = stmt.column<long?>(0);
                        Assert.True(v.HasValue);
                        Assert.Equal(32, v);
                    }

                    {
                        var v = stmt.column<double?>(1);
                        Assert.True(v.HasValue);
                        Assert.Equal(3.14, v);
                    }
                }
                using (sqlite3_stmt stmt = db.prepare("SELECT n,r FROM foo WHERE id=2;"))
                {
                    stmt.step();

                    Assert.Equal(44, stmt.column_int(0));
                    Assert.Equal(raw.SQLITE_NULL, stmt.column_type(1));

                    {
                        var v = stmt.column<int?>(0);
                        Assert.True(v.HasValue);
                        Assert.Equal(44, v);
                    }

                    {
                        var v = stmt.column<double?>(1);
                        Assert.False(v.HasValue);
                    }
                }
                using (sqlite3_stmt stmt = db.prepare("SELECT n,r FROM foo WHERE id=3;"))
                {
                    stmt.step();

                    Assert.Equal(raw.SQLITE_NULL, stmt.column_type(0));
                    Assert.Equal(1.414, stmt.column_double(1));

                    {
                        var v = stmt.column<int?>(0);
                        Assert.False(v.HasValue);
                    }

                    {
                        var v = stmt.column<long?>(0);
                        Assert.False(v.HasValue);
                    }

                    {
                        var v = stmt.column<double?>(1);
                        Assert.True(v.HasValue);
                        Assert.Equal(1.414, v);
                    }
                }
                using (sqlite3_stmt stmt = db.prepare("SELECT n,r FROM foo WHERE id=4;"))
                {
                    stmt.step();

                    Assert.Equal(0, stmt.column_int(0));
                    Assert.Equal(raw.SQLITE_NULL, stmt.column_type(1));

                    {
                        var v = stmt.column<int?>(0);
                        Assert.True(v.HasValue);
                        Assert.Equal(0, v);
                    }

                    {
                        var v = stmt.column<DateTime>(0);
                        Assert.Equal(1970, v.Year);
                    }
                }
                using (sqlite3_stmt stmt = db.prepare("SELECT n,r FROM foo WHERE id=5;"))
                {
                    stmt.step();

                    {
                        var v = stmt.column<DateTime>(0);
                        Assert.Equal(1968, v.Year);
                        Assert.Equal(3, v.Month);
                        Assert.Equal(15, v.Day);
                    }
                }
                using (sqlite3_stmt stmt = db.prepare("SELECT n,r FROM foo WHERE id=5;"))
                {
                    stmt.step();

                    Assert.Throws<NotSupportedException>(
                        () =>
                        {
                            var v = stmt.column<decimal>(0);
                        }
                        );
                }
            }
        }

        [Fact]
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
                Assert.Equal("int", dataType);
                Assert.Equal("BINARY", collSeq);
                Assert.True(notNull > 0);
                Assert.Equal(0, primaryKey);
                Assert.Equal(0, autoInc);

                raw.sqlite3_table_column_metadata(db, "main", "foo", "rowid", out dataType, out collSeq, out notNull, out primaryKey, out autoInc);
                Assert.Equal("integer", dataType);
                Assert.Equal("BINARY", collSeq);
                Assert.Equal(0, notNull);
                Assert.True(primaryKey > 0);
                Assert.True(primaryKey > 0);
            }
        }

        [Fact]
        public void test_progress_handler_left_registered()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                int count = 0;

                delegate_progress handler = obj =>
                    {
                        Assert.Equal("user_data", obj);
                        count++;
                        return 0;
                    };

                raw.sqlite3_progress_handler(db, 1, handler, "user_data");

                GC.Collect();

                using (sqlite3_stmt stmt = db.prepare("SELECT 1;"))
                {
                    stmt.step();
                }
                Assert.True(count > 0);
            }
        }

        [Fact]
        public void test_progress_handler()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                int count = 0;

                delegate_progress handler = obj =>
                    {
                        Assert.Equal("user_data", obj);
                        count++;
                        return 0;
                    };

                raw.sqlite3_progress_handler(db, 1, handler, "user_data");

                GC.Collect();

                using (sqlite3_stmt stmt = db.prepare("SELECT 1;"))
                {
                    stmt.step();
                }
                Assert.True(count > 0);

                handler = obj => 1;
                raw.sqlite3_progress_handler(db, 1, handler, null);
                using (sqlite3_stmt stmt = db.prepare("SELECT 1;"))
                {
                    var e = Assert.Throws<ugly.sqlite3_exception>(
                        () => stmt.step()
                        );
                    Assert.Equal(raw.SQLITE_INTERRUPT, e.errcode);
                }

                // Fact that assigning null to the handler removes the progress handler.
                handler = null;
                raw.sqlite3_progress_handler(db, 1, handler, null);
                using (sqlite3_stmt stmt = db.prepare("SELECT 1;"))
                {
                    stmt.step();
                }
            }
        }
    }

    [Collection("Init")]
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

        [Fact]
        public void test_row()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.exec("CREATE TABLE foo (x int, v int, t text, d real, b blob, q blob);");
                byte[] blob = db.query_scalar<byte[]>("SELECT randomblob(5);");
                db.exec("INSERT INTO foo (x,v,t,d,b,q) VALUES (?,?,?,?,?,?)", 32, 44, "hello", 3.14, blob, null);
                foreach (row r in db.query<row>("SELECT x,v,t,d,b,q FROM foo;"))
                {
                    Assert.Equal(32, r.x);
                    Assert.Equal(44, r.v);
                    Assert.Equal("hello", r.t);
                    Assert.Equal(3.14, r.d);
                    Assert.Equal(blob.Length, r.b.Length);
                    for (int i = 0; i < blob.Length; i++)
                    {
                        Assert.Equal(r.b[i], blob[i]);
                    }
                    Assert.Null(r.q);
                }
                using (sqlite3_stmt stmt = db.prepare("SELECT x,v,t,d,b,q FROM foo;"))
                {
                    stmt.step();

                    Assert.Equal(db, stmt.db_handle());

                    Assert.Equal(32, stmt.column_int(0));
                    Assert.Equal(44, stmt.column_int64(1));
                    Assert.Equal("hello", stmt.column_text(2));
                    Assert.Equal(3.14, stmt.column_double(3));
                    Assert.Equal(blob.Length, stmt.column_bytes(4));
                    var b2 = stmt.column_blob(4);
                    Assert.Equal(b2.Length, blob.Length);
                    for (int i = 0; i < blob.Length; i++)
                    {
                        Assert.Equal(b2[i], blob[i]);
                    }

                    Assert.Equal(raw.SQLITE_NULL, stmt.column_type(5));

                    Assert.Equal("x", stmt.column_name(0));
                    Assert.Equal("v", stmt.column_name(1));
                    Assert.Equal("t", stmt.column_name(2));
                    Assert.Equal("d", stmt.column_name(3));
                    Assert.Equal("b", stmt.column_name(4));
                    Assert.Equal("q", stmt.column_name(5));
                }
            }
        }

    }

    [Collection("Init")]
    public class class_test_exec_with_callback
    {
        private class work
        {
            public int count;
        }

        private static int my_cb(object v, string[] values, string[] names)
        {
            Assert.Single(values);
            Assert.Single(names);
            Assert.Equal("x", names[0]);
            Assert.Single(values[0]);

            work w = v as work;
            w.count++;

            return 0;
        }

        [Fact]
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
                Assert.Equal(5, w.count);
            }
        }
    }

    [Collection("Init")]
    public class class_test_collation
    {
        private const int val = 5;

        private static int my_collation(object v, string s1, string s2)
        {
            s1 = s1.Replace('e', 'a');
            s2 = s2.Replace('e', 'a');
            return string.Compare(s1, s2);
        }

        [Fact]
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
                Assert.Equal("e", top);
                GC.Collect();
                string top2 = db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;");
                Assert.Equal("e", top2);
            }
        }

        [Fact]
        public void test_collation_remove()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_collation("e2a", null, my_collation);
                db.exec("CREATE TABLE foo (x text);");
                db.exec("INSERT INTO foo (x) VALUES ('b')");
                db.exec("INSERT INTO foo (x) VALUES ('c')");
                db.exec("INSERT INTO foo (x) VALUES ('d')");
                db.exec("INSERT INTO foo (x) VALUES ('e')");
                db.exec("INSERT INTO foo (x) VALUES ('f')");
                {
                    string top = db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;");
                    Assert.Equal("b", top);
                }
                GC.Collect();
                {
                    string top = db.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE e2a ASC LIMIT 1;");
                    Assert.Equal("e", top);
                }
                GC.Collect();
                {
                    string top = db.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE e2a ASC LIMIT 1;");
                    Assert.Equal("e", top);
                }

                db.create_collation("e2a", null, null);

                {
                    string top = db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;");
                    Assert.Equal("b", top);
                }
                {
                    Assert.Throws<ugly.sqlite3_exception>(
                        () =>
                        {
                            string top = db.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE e2a ASC LIMIT 1;");
                        }
                        );
                }
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

        [Fact]
        public void test_collation_2()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_collation("e2a", null, my_collation);
                setup(db);
                Assert.Equal("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                GC.Collect();
                Assert.Equal("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                GC.Collect();
                using (sqlite3 db2 = ugly.open(":memory:"))
                {
                    GC.Collect();
                    db2.create_collation("e2a", null, my_collation);
                    GC.Collect();
                    Assert.Equal("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                    GC.Collect();
                    setup(db2);
                    GC.Collect();
                    Assert.Equal("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                    GC.Collect();
                    Assert.Equal("e", db2.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                }
                GC.Collect();
                Assert.Equal("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
            }
        }

        [Fact]
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
                    Assert.Equal("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                    Assert.Equal("e", db2.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                    GC.Collect();
                    Assert.Equal("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                    Assert.Equal("e", db2.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
                }
                GC.Collect();
                Assert.Equal("e", db.query_scalar<string>("SELECT x FROM foo ORDER BY x ASC LIMIT 1;"));
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

        [Fact]
        public void test_collation_4()
        {
            string res1, res2, res3;
            using (sqlite3 db = ugly.open(":memory:"))
            {
                setup2(db);
                db.create_collation("col", null, (o, p1, p2) => string.Compare(p1, p2));
                res1 = db.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE col");

                using (sqlite3 db2 = ugly.open(":memory:"))
                {
                    setup2(db2);
                    db2.create_collation("col", null, (o, p1, p2) => string.Compare(p2, p1));
                    res2 = db2.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE col");
                }
                res3 = db.query_scalar<string>("SELECT x FROM foo ORDER BY x COLLATE col");
            }
            Assert.Equal(res1, res3);
        }

        private static int my_collation_1(object v, string s1, string s2)
        {
            return string.Compare(s1, s2);
        }

        private static int my_collation_2(object v, string s1, string s2)
        {
            return string.Compare(s2, s1);
        }

        [Fact]
        public void test_collation_5()
        {
            string res1, res2, res3;
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
            Assert.Equal(res1, res3);
        }

    }

    [Collection("Init")]
    public class class_test_cube
    {
        private const int val = 5;

        private static void cube(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            Assert.Single(args);
            long x = args[0].value_int64();
            Assert.Equal(val, x);
            ctx.result_int64(x * x * x);
        }

        [Fact]
        public void test_cube()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("cube", 1, null, cube);
                long c = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.Equal(val * val * val, c);
                GC.Collect();
                long c2 = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.Equal(val * val * val, c2);
            }
        }

        [Fact]
        public void test_cube_with_unregister()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("cube", 1, null, cube);
                long c = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.Equal(val * val * val, c);
                GC.Collect();
                long c2 = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.Equal(val * val * val, c2);

                db.create_function("cube", 1, null, null);
                Assert.Throws<ugly.sqlite3_exception>(
                    () =>
                    {
                        var c3 = db.query_scalar<long>("SELECT cube(?);", 7);
                    }
                    );
            }
        }

        [Fact]
        public void test_cube_with_flags()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("cube", 1, raw.SQLITE_DETERMINISTIC, null, cube);
                long c = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.Equal(val * val * val, c);
                GC.Collect();
                long c2 = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.Equal(val * val * val, c2);
            }
        }
    }

    [Collection("Init")]
    public class class_test_func_names_case
    {
        private const int val = 5;

        private static void cube_wrong(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            Assert.Single(args);
            long x = args[0].value_int64();
            ctx.result_int64(x * 2);
        }

        private static void cube(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            Assert.Single(args);
            long x = args[0].value_int64();
            ctx.result_int64(x * x * x);
        }

        [Fact]
        public void test_func_name_override_1()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("cube", 1, null, cube_wrong);
                long c = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.Equal(val * 2, c);
                GC.Collect();
                db.create_function("Cube", 1, null, cube);
                long c2 = db.query_scalar<long>("SELECT cube(?);", val);
                Assert.Equal(val * val * val, c2);
            }
        }
    }

    [Collection("Init")]
    public class class_test_func_multi
    {
        private const int val = 5;
        private const int val2 = 7;

        private static void mul_2(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            Assert.Single(args);
            long x = args[0].value_int64();
            ctx.result_int64(x * 2);
        }

        private static void mul(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            Assert.Equal(2, args.Length);
            long x = args[0].value_int64();
            long y = args[1].value_int64();
            ctx.result_int64(x * y);
        }

        [Fact]
        public void test_func_name_multi()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("foo", 1, null, mul_2);
                db.create_function("foo", 2, null, mul);
                long c = db.query_scalar<long>("SELECT foo(?);", val);
                Assert.Equal(val * 2, c);
                GC.Collect();
                long c2 = db.query_scalar<long>("SELECT foo(?,?);", val, val2);
                Assert.Equal(val * val2, c2);
            }
        }
    }

    [Collection("Init")]
    public class class_test_makeblob
    {
        private const int val = 5;

        private static void makeblob(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            byte[] b = new byte[args[0].value_int()];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)(i % 256);
            }
            ctx.result_blob(b);
        }

        [Fact]
        public void test_makeblob()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("makeblob", 1, null, makeblob);
                byte[] c = db.query_scalar<byte[]>("SELECT makeblob(?);", val);
                Assert.Equal(val, c.Length);
            }
        }
    }

    [Collection("Init")]
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

        [Fact]
        public void test_scalar_mean_double()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("my_mean", -1, null, mean);
                double d = db.query_scalar<double>("SELECT my_mean(1,2,3,4,5,6,7,8);");
                Assert.True(d >= (36 / 8));
                Assert.True(d <= (36 / 8 + 1));
                GC.Collect();
                double d2 = db.query_scalar<double>("SELECT my_mean(1,2,3,4,5,6,7,8);");
                Assert.True(d2 >= (36 / 8));
                Assert.True(d2 <= (36 / 8 + 1));
            }
        }
    }

    [Collection("Init")]
    public class class_test_countargs
    {
        private static void count_args(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            ctx.result_int(args.Length);
        }

        [Fact]
        public void test_countargs()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("count_args", -1, null, count_args);
                Assert.Equal(8, db.query_scalar<int>("SELECT count_args(1,2,3,4,5,6,7,8);"));
                Assert.Equal(0, db.query_scalar<int>("SELECT count_args();"));
                Assert.Equal(1, db.query_scalar<int>("SELECT count_args(null);"));
                GC.Collect();
                Assert.Equal(8, db.query_scalar<int>("SELECT count_args(1,2,3,4,5,6,7,8);"));
                Assert.Equal(0, db.query_scalar<int>("SELECT count_args();"));
                Assert.Equal(1, db.query_scalar<int>("SELECT count_args(null);"));
            }
        }
    }

    [Collection("Init")]
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

        [Fact]
        public void test_countnullargs()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("count_nulls", -1, null, count_nulls);
                Assert.Equal(0, db.query_scalar<int>("SELECT count_nulls(1,2,3,4,5,6,7,8);"));
                Assert.Equal(0, db.query_scalar<int>("SELECT count_nulls();"));
                Assert.Equal(1, db.query_scalar<int>("SELECT count_nulls(null);"));
                Assert.Equal(2, db.query_scalar<int>("SELECT count_nulls(1,null,3,null,5);"));
                GC.Collect();
                Assert.Equal(0, db.query_scalar<int>("SELECT count_nulls(1,2,3,4,5,6,7,8);"));
                Assert.Equal(0, db.query_scalar<int>("SELECT count_nulls();"));
                Assert.Equal(1, db.query_scalar<int>("SELECT count_nulls(null);"));
                Assert.Equal(2, db.query_scalar<int>("SELECT count_nulls(1,null,3,null,5);"));
            }
        }
    }

    [Collection("Init")]
    public class class_test_len_as_blobs
    {
        private static void len_as_blobs(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            int r = 0;
            foreach (sqlite3_value v in args)
            {
                if (v.value_type() != raw.SQLITE_NULL)
                {
                    Assert.Equal(v.value_blob().Length, v.value_bytes());
                    r += v.value_blob().Length;
                }
            }
            ctx.result_int(r);
        }

        [Fact]
        public void test_len_as_blobs()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("len_as_blobs", -1, null, len_as_blobs);
                Assert.Equal(0, db.query_scalar<int>("SELECT len_as_blobs();"));
                Assert.Equal(0, db.query_scalar<int>("SELECT len_as_blobs(null);"));
                Assert.True(8 <= db.query_scalar<int>("SELECT len_as_blobs(1,2,3,4,5,6,7,8);"));
                GC.Collect();
                Assert.Equal(0, db.query_scalar<int>("SELECT len_as_blobs();"));
                Assert.Equal(0, db.query_scalar<int>("SELECT len_as_blobs(null);"));
                Assert.True(8 <= db.query_scalar<int>("SELECT len_as_blobs(1,2,3,4,5,6,7,8);"));
            }
        }
    }

    [Collection("Init")]
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

        [Fact]
        public void test_concat()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("my_concat", -1, null, concat);
                Assert.Equal("foobar", db.query_scalar<string>("SELECT my_concat('foo', 'bar');"));
                Assert.Equal("abc", db.query_scalar<string>("SELECT my_concat('a', 'b', 'c');"));
                GC.Collect();
                Assert.Equal("foobar", db.query_scalar<string>("SELECT my_concat('foo', 'bar');"));
                Assert.Equal("abc", db.query_scalar<string>("SELECT my_concat('a', 'b', 'c');"));
            }
        }
    }

    [Collection("Init")]
    public class class_test_sum_plus_count
    {
        private class my_state
        {
            public long sum;
            public long count;
        }

        private static void sum_plus_count_step(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        {
            Assert.Single(args);

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

        [Fact]
        public void test_sum_plus_count()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                db.create_function("sum_plus_count", 1, null, sum_plus_count_step, sum_plus_count_final);
                db.exec("CREATE TABLE foo (x int);");
                for (int i = 0; i < 5; i++)
                {
                    db.exec("INSERT INTO foo (x) VALUES (?);", i);
                }
                long c = db.query_scalar<long>("SELECT sum_plus_count(x) FROM foo;");
                Assert.Equal((0 + 1 + 2 + 3 + 4) + 5, c);
                GC.Collect();
                long c2 = db.query_scalar<long>("SELECT sum_plus_count(x) FROM foo;");
                Assert.Equal((0 + 1 + 2 + 3 + 4) + 5, c2);
            }
        }
    }

    [Order(0)]
    public class class_test_sqlite3_config
    {
        // this test class is special.  
        // it needs to run first, before anything else.
        // it is marked with attributes so that fake_xunit
        // will find it and run it, but real xunit will ignore it.

        [Test]
        [Order(0)]
        public void test_log()
        {
            var msgs = new List<string>();
            var rc = raw.sqlite3_config_log(
                (v, errcode, msg) => msgs.Add(msg),
                null
                );
            Assert.Equal(0, rc);

            GC.Collect();

            const string VAL = "hello!";
            raw.sqlite3_log(0, VAL);
            Assert.Single(msgs);
            Assert.Equal(VAL, msgs[0]);

            strdelegate_log no_cb = null;
            rc = raw.sqlite3_config_log(no_cb, null);
            Assert.Equal(0, rc);
        }

        [Test]
        [Order(1)]
        public void test_call_sqlite3_initialize()
        {
            var rc = raw.sqlite3_initialize();
            Assert.Equal(0, rc);
        }

        [Test]
        [Order(2)]
        public void test_call_sqlite3_config_after_initialize()
        {
            var rc = raw.sqlite3_config(raw.SQLITE_CONFIG_SERIALIZED);
            Assert.Equal(raw.SQLITE_MISUSE, rc);
        }

        [Test]
        [Order(3)]
        public void test_call_sqlite3_shutdown()
        {
            var rc = raw.sqlite3_shutdown();
            Assert.Equal(0, rc);
        }

        [Test]
        [Order(4)]
        public void test_call_sqlite3_log_after_shutdown()
        {
            var msgs = new List<string>();
            var rc = raw.sqlite3_config_log(
                (v, errcode, msg) => msgs.Add(msg),
                null
                );
            Assert.Equal(raw.SQLITE_OK, rc);
        }

        [Test]
        [Order(5)]
        public void test_call_sqlite3_config_after_shutdown()
        {
            int rc;

            rc = raw.sqlite3_config(raw.SQLITE_CONFIG_URI, 1);
            Assert.Equal(raw.SQLITE_OK, rc);

            rc = raw.sqlite3_config(raw.SQLITE_CONFIG_URI, 0);
            Assert.Equal(raw.SQLITE_OK, rc);
        }


    }

    [Collection("Init")]
    public class class_test_hooks
    {
        private class work
        {
            public int count_commits;
            public int count_rollbacks;
            public int count_updates;
            public int count_traces;
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

        [Fact]
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

        [Fact]
        public void test_hooks()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                work w = new work();
                Assert.Equal(0, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(0, w.count_updates);
                Assert.Equal(0, w.count_traces);

                db.commit_hook(new delegate_commit(my_commit_hook), w);
                db.rollback_hook(new delegate_rollback(my_rollback_hook), w);
                db.update_hook(my_update_hook, w);
                db.trace(my_trace_hook, w);

                GC.Collect();

                db.exec("CREATE TABLE foo (x int);");

                Assert.Equal(1, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(0, w.count_updates);
                Assert.Equal(1, w.count_traces);

                db.exec("INSERT INTO foo (x) VALUES (1);");

                Assert.Equal(2, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(1, w.count_updates);
                Assert.Equal(2, w.count_traces);

                db.exec("BEGIN TRANSACTION;");

                Assert.Equal(2, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(1, w.count_updates);
                Assert.Equal(3, w.count_traces);

                db.exec("INSERT INTO foo (x) VALUES (2);");

                Assert.Equal(2, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(2, w.count_updates);
                Assert.Equal(4, w.count_traces);

                db.exec("ROLLBACK TRANSACTION;");

                Assert.Equal(2, w.count_commits);
                Assert.Equal(1, w.count_rollbacks);
                Assert.Equal(2, w.count_updates);
                Assert.Equal(5, w.count_traces);

            }
        }

        [Fact]
        public void test_hooks_2()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                var count_commits = 0;

                db.commit_hook(
                    x =>
                    {
                        count_commits++;
                        return 0;
                    },
                    null);

                GC.Collect();

                db.exec("CREATE TABLE foo (x int);");

                GC.Collect();

                Assert.Equal(1, count_commits);

                GC.Collect();

                db.exec("INSERT INTO foo (x) VALUES (1);");

                Assert.Equal(2, count_commits);

                db.exec("BEGIN TRANSACTION;");

                GC.Collect();

                Assert.Equal(2, count_commits);

                db.exec("INSERT INTO foo (x) VALUES (2);");

                Assert.Equal(2, count_commits);

                db.exec("ROLLBACK TRANSACTION;");

                Assert.Equal(2, count_commits);

            }
        }

    }

    [Collection("Init")]
    public class class_test_hooks_profiles
    {
        private class work
        {
            public int count_commits;
            public int count_rollbacks;
            public int count_updates;
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

        private static void my_profile_hook(object v, string sql, long ns)
        {
            work w = v as work;
            w.count_profiles++;
        }

        [Fact]
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

        [Fact]
        public void test_hooks()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                work w = new work();
                Assert.Equal(0, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(0, w.count_updates);
                Assert.Equal(0, w.count_profiles);

                db.commit_hook(new delegate_commit(my_commit_hook), w);
                db.rollback_hook(new delegate_rollback(my_rollback_hook), w);
                db.update_hook(my_update_hook, w);
                db.profile(my_profile_hook, w);

                GC.Collect();

                db.exec("CREATE TABLE foo (x int);");

                Assert.Equal(1, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(0, w.count_updates);
                Assert.Equal(1, w.count_profiles);

                db.exec("INSERT INTO foo (x) VALUES (1);");

                Assert.Equal(2, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(1, w.count_updates);
                Assert.Equal(2, w.count_profiles);

                db.exec("BEGIN TRANSACTION;");

                Assert.Equal(2, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(1, w.count_updates);
                Assert.Equal(3, w.count_profiles);

                db.exec("INSERT INTO foo (x) VALUES (2);");

                Assert.Equal(2, w.count_commits);
                Assert.Equal(0, w.count_rollbacks);
                Assert.Equal(2, w.count_updates);
                Assert.Equal(4, w.count_profiles);

                db.exec("ROLLBACK TRANSACTION;");

                Assert.Equal(2, w.count_commits);
                Assert.Equal(1, w.count_rollbacks);
                Assert.Equal(2, w.count_updates);
                Assert.Equal(5, w.count_profiles);

            }
        }

        [Fact]
        public void test_hooks_2()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                var count_commits = 0;

                db.commit_hook(
                    x =>
                    {
                        count_commits++;
                        return 0;
                    },
                    null);

                GC.Collect();

                db.exec("CREATE TABLE foo (x int);");

                GC.Collect();

                Assert.Equal(1, count_commits);

                GC.Collect();

                db.exec("INSERT INTO foo (x) VALUES (1);");

                Assert.Equal(2, count_commits);

                db.exec("BEGIN TRANSACTION;");

                GC.Collect();

                Assert.Equal(2, count_commits);

                db.exec("INSERT INTO foo (x) VALUES (2);");

                Assert.Equal(2, count_commits);

                db.exec("ROLLBACK TRANSACTION;");

                Assert.Equal(2, count_commits);

            }
        }

    }

    [Collection("Init")]
    public class class_test_crypto
    {
        private static bool is_sqlcipher()
        {
            using (sqlite3 db = ugly.open(":memory:"))
            {
                var s = db.query_scalar<string>("PRAGMA cipher_version");
                return !string.IsNullOrEmpty(s);
            }
        }

        [Fact]
        public void test_encrypted_file_with_pragma()
        {
            if (is_sqlcipher())
            {
                Assert.Contains("sqlcipher", raw.GetNativeLibraryName());
                string name;
                using (sqlite3 db = ugly.open(":memory:"))
                {
                    name = "tmp" + db.query_scalar<string>("SELECT lower(hex(randomblob(16)));");
                }
                const string correct_phrase = "I am Groot";
                var nums = new int[] { 1, 6, 3, 0, 9 };
                using (sqlite3 db = ugly.open(name))
                {
                    db.exec($"PRAGMA key ='{correct_phrase}'");
                    db.exec("CREATE TABLE foo (x int);");
                    foreach (var x in nums)
                    {
                        db.exec("INSERT INTO foo (x) VALUES (?)", x);
                    }
                }
                var sum_expected = nums.Sum();
                bool check_sum(string pass)
                {
                    using (sqlite3 db = ugly.open(name))
                    {
                        if (pass != null)
                        {
                            db.exec($"PRAGMA key ='{pass}'");
                        }
                        bool fail;
                        try
                        {
                            var sum = db.query_scalar<int>("SELECT sum(x) FROM foo;");
                            Assert.Equal(sum_expected, sum);
                            fail = false;
                        }
                        catch (ugly.sqlite3_exception)
                        {
                            fail = true;
                        }
                        return !fail;
                    }
                }
                Assert.True(check_sum(correct_phrase));
                Assert.False(check_sum(null));
                Assert.False(check_sum("wrong phrase"));

                ugly.vfs__delete(null, name, 1);
            }
            else
            {
                Assert.DoesNotContain("sqlcipher", raw.GetNativeLibraryName());
            }
        }

        [Fact]
        public void test_encrypted_file_with_key()
        {
            if (is_sqlcipher())
            {
                Assert.Contains("sqlcipher", raw.GetNativeLibraryName());
                string name;
                using (sqlite3 db = ugly.open(":memory:"))
                {
                    name = "tmp" + db.query_scalar<string>("SELECT lower(hex(randomblob(16)));");
                }
                var correct_key = new byte[] { 8, 6, 7, 5, 3, 0, 9 };
                var nums = new int[] { 1, 6, 3, 0, 9 };
                using (sqlite3 db = ugly.open(name))
                {
                    db.key(correct_key);
                    db.exec("CREATE TABLE foo (x int);");
                    foreach (var x in nums)
                    {
                        db.exec("INSERT INTO foo (x) VALUES (?)", x);
                    }
                }
                var sum_expected = nums.Sum();
                bool check_sum(byte[] k)
                {
                    using (sqlite3 db = ugly.open(name))
                    {
                        if (k != null)
                        {
                            db.key(k);
                        }
                        bool fail;
                        try
                        {
                            var sum = db.query_scalar<int>("SELECT sum(x) FROM foo;");
                            Assert.Equal(sum_expected, sum);
                            fail = false;
                        }
                        catch (ugly.sqlite3_exception)
                        {
                            fail = true;
                        }
                        return !fail;
                    }
                }
                Assert.True(check_sum(correct_key));
                Assert.False(check_sum(null));
                var new_key = new byte[] { 5, 6, 7, 8 };
                Assert.False(check_sum(new_key));

                using (sqlite3 db = ugly.open(name))
                {
                    db.key(correct_key);
                    db.rekey(new_key);
                }
                Assert.False(check_sum(correct_key));
                Assert.False(check_sum(null));
                Assert.True(check_sum(new_key));

                ugly.vfs__delete(null, name, 1);
            }
            else
            {
                Assert.DoesNotContain("sqlcipher", raw.GetNativeLibraryName());
            }
        }

    }
}

