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
using System.Reflection;

using SQLitePCL;

namespace SQLitePCL.Ugly
{
    // SQLitePCL contains several very basic classes:
    //
    //     sqlite3
    //     sqlite3_stmt
    //     sqlite3_blob
    //     sqlite3_backup
    //     sqlite3_context
    //     sqlite3_value
    //
    // Each of these classes is little more than a strongly typed
    // wrapper around an IntPtr.  Like an IntPtr, they are opaque.
    // They contain no instance methods.
    //
    // For example, the sqlite3_stmt class represents a statement
    // handle, but you still have to do things like this:
    //
    //     int rc;
    //
    //     sqlite3 db;
    //     rc = raw.sqlite3_open(":memory:", out db);
    //     if (rc != raw.SQLITE_OK)
    //     {
    //         error
    //     }
    //     sqlite3_stmt stmt;
    //     rc = raw.sqlite3_prepare(db, "CREATE TABLE foo (x int)", out stmt);
    //     if (rc != raw.SQLITE_OK)
    //     {
    //         error
    //     }
    //     rc = raw.sqlite3_step(stmt);
    //     if (rc == raw.SQLITE_DONE)
    //     {
    //         whatever
    //     }
    //     else
    //     {
    //         error
    //     }
    //     raw.sqlite3_finalize(stmt);
    //
    // Below are a bunch of extension methods which throw exceptions
    // instead of returning integer error codes.  Now you can do
    // things like this:
    //
    //     using (sqlite3 db = ugly.open(":memory:))
    //     {
    //         sqlite3_stmt stmt = db.prepare("CREATE TABLE foo (x int)");
    //         stmt.step();
    //     }
    // 
    // This exception-throwing wrapper exists so that I can have something
    // easier against which to write tests.  It retains all the "lower-case
    // and underscores" ugliness of the layer below.  As such, this is not
    // a wrapper intended for public consumption.  It does not do things
    // the C# way.

    public static class ugly
    {
        public class sqlite3_exception : Exception
        {
            int _errcode;
            string _errmsg;

            public sqlite3_exception(int rc) : base(string.Format("rc={0}", rc))
            {
                _errcode = rc;
                _errmsg = null;
            }

            public sqlite3_exception(int rc, string msg) : base(string.Format("rc={0}: {1}", rc, msg))
            {
                _errcode = rc;
                _errmsg = msg;
            }

            public override string ToString()
            {
                return string.Format("{0}: {1}\r\n{2}", _errcode, _errmsg, base.ToString());
            }

            public int errcode
            {
                get
                {
                    return _errcode;
                }
            }

            public string errmsg
            {
                get
                {
                    return _errmsg;
                }
            }

        }

        private static void check_ok(int rc)
        {
            if (raw.SQLITE_OK != rc) throw new sqlite3_exception(rc);
        }

        private static void check_ok(sqlite3 db, int rc)
        {
            if (raw.SQLITE_OK != rc) throw new sqlite3_exception(rc, db.errmsg());
        }

        public static void initialize()
        {
            int rc = raw.sqlite3_initialize();
            check_ok(rc);
        }

        public static void shutdown()
        {
            int rc = raw.sqlite3_shutdown();
            check_ok(rc);
        }

        public static void config(int op)
        {
            int rc = raw.sqlite3_config(op);
            check_ok(rc);
        }

        public static void config(int op, int val)
        {
            int rc = raw.sqlite3_config(op, val);
            check_ok(rc);
        }

        public static sqlite3 open(string filename)
        {
            sqlite3 db;
            int rc = raw.sqlite3_open(filename, out db);
            check_ok(rc);

            return db;
        }

        public static sqlite3 open_v2(string filename, int flags, string vfs)
        {
            sqlite3 db;
            int rc = raw.sqlite3_open_v2(filename, out db, flags, vfs);
            check_ok(rc);

            return db;
        }

        public static void sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            int rc = raw.sqlite3_status(op, out current, out highwater, resetFlag);
            check_ok(rc);
        }

        public static void vfs__delete(string vfs, string filename, int syncdir)
        {
            int rc = raw.sqlite3__vfs__delete(vfs, filename, syncdir);
            check_ok(rc);
        }

        public static int errcode(this sqlite3 db)
        {
            return raw.sqlite3_errcode(db);
        }

        public static int extended_errcode(this sqlite3 db)
        {
            return raw.sqlite3_extended_errcode(db);
        }

        public static string errmsg(this sqlite3 db)
        {
            return raw.sqlite3_errmsg(db).utf8_to_string();
        }

        public static string db_filename(this sqlite3 db, string att)
        {
            return raw.sqlite3_db_filename(db, att).utf8_to_string();
        }

        public static void commit_hook(this sqlite3 db, delegate_commit f, object v)
        {
            raw.sqlite3_commit_hook(db, f, v);
        }

        public static void rollback_hook(this sqlite3 db, delegate_rollback f, object v)
        {
            raw.sqlite3_rollback_hook(db, f, v);
        }

        public static void trace(this sqlite3 db, strdelegate_trace f, object v)
        {
            raw.sqlite3_trace(db, f, v);
        }

        public static void profile(this sqlite3 db, strdelegate_profile f, object v)
        {
            raw.sqlite3_profile(db, f, v);
        }

        public static void update_hook(this sqlite3 db, strdelegate_update f, object v)
        {
            raw.sqlite3_update_hook(db, f, v);
        }

        public static void create_collation(this sqlite3 db, string name, object v, strdelegate_collation f)
        {
            int rc = raw.sqlite3_create_collation(db, name, v, f);
            check_ok(rc);
        }

        public static void create_function(this sqlite3 db, string name, int nargs, object v, delegate_function_scalar f)
        {
            int rc = raw.sqlite3_create_function(db, name, nargs, v, f);
            check_ok(rc);
        }

        public static void create_function(this sqlite3 db, string name, int nargs, object v, delegate_function_aggregate_step f_step, delegate_function_aggregate_final f_final)
        {
            int rc = raw.sqlite3_create_function(db, name, nargs, v, f_step, f_final);
            check_ok(rc);
        }

        public static void create_function(this sqlite3 db, string name, int nargs, int flags, object v, delegate_function_scalar f)
        {
            int rc = raw.sqlite3_create_function(db, name, nargs, flags, v, f);
            check_ok(rc);
        }

        public static void create_function(this sqlite3 db, string name, int nargs, int flags, object v, delegate_function_aggregate_step f_step, delegate_function_aggregate_final f_final)
        {
            int rc = raw.sqlite3_create_function(db, name, nargs, flags, v, f_step, f_final);
            check_ok(rc);
        }

        public static sqlite3_stmt prepare(this sqlite3 db, string sql)
        {
            sqlite3_stmt stmt;
            string tail;
            int rc = raw.sqlite3_prepare_v2(db, sql, out stmt, out tail);
            // TODO maybe throw if there is a tail?  this function is called
            // in ways that assume the sql string contains only one statement.
            check_ok(db, rc);

            return stmt;
        }

        public static sqlite3_stmt prepare_v3(this sqlite3 db, string sql, uint flags)
        {
            sqlite3_stmt stmt;
            string tail;
            int rc = raw.sqlite3_prepare_v3(db, sql, flags, out stmt, out tail);
            // TODO maybe throw if there is a tail?  this function is called
            // in ways that assume the sql string contains only one statement.
            check_ok(db, rc);

            return stmt;
        }

        public static void db_status(this sqlite3 db, int op, out int current, out int highest, int resetFlg)
        {
            int rc = raw.sqlite3_db_status(db, op, out current, out highest, resetFlg);
            check_ok(db, rc);
        }

        public static long last_insert_rowid(this sqlite3 db)
        {
            return raw.sqlite3_last_insert_rowid(db);
        }

        public static int changes(this sqlite3 db)
        {
            return raw.sqlite3_changes(db);
        }

        public static int total_changes(this sqlite3 db)
        {
            return raw.sqlite3_total_changes(db);
        }

        public static void busy_timeout(this sqlite3 db, int ms)
        {
            int rc = raw.sqlite3_busy_timeout(db, ms);
            check_ok(rc);
        }

        public static int extended_result_codes(this sqlite3 db, int onoff)
        {
            return raw.sqlite3_extended_result_codes(db, onoff);
        }

        public static int enable_load_extension(this sqlite3 db, int onoff)
        {
            return raw.sqlite3_enable_load_extension(db, onoff);
        }

        public static int get_autocommit(this sqlite3 db)
        {
            return raw.sqlite3_get_autocommit(db);
        }

        public static sqlite3_backup backup_init(this sqlite3 db, string SourceDBName, sqlite3 destDB, string DestDBName)
        {
            return raw.sqlite3_backup_init(destDB, DestDBName, db, SourceDBName);
        }

        public static sqlite3_stmt next_stmt(this sqlite3 db, sqlite3_stmt s)
        {
            return raw.sqlite3_next_stmt(db, s);
        }

        public static sqlite3_blob blob_open(this sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z column_utf8, long rowid, int flags)
        {
            sqlite3_blob blob;
            int rc = raw.sqlite3_blob_open(db, db_utf8, table_utf8, column_utf8, rowid, flags, out blob);
            check_ok(rc);
            return blob;
        }

        public static sqlite3_blob blob_open(this sqlite3 db, string sdb, string table, string column, long rowid, int flags)
        {
            sqlite3_blob blob;
            int rc = raw.sqlite3_blob_open(db, sdb, table, column, rowid, flags, out blob);
            check_ok(rc);
            return blob;
        }

        public static T query_scalar<T>(this sqlite3 db, string sql, params object[] a)
        {
            using (sqlite3_stmt stmt = db.prepare(sql, a))
            {
                stmt.step();
                return stmt.column<T>(0);
            }
        }

        public static IEnumerable<T> query<T>(this sqlite3 db, string sql, params object[] a) where T : class, new()
        {
            using (sqlite3_stmt stmt = db.prepare(sql, a))
            {
                while (raw.SQLITE_ROW == stmt.step())
                {
                    yield return stmt.row<T>();
                }
            }
        }

        public static IEnumerable<T> query_one_column<T>(this sqlite3 db, string sql, params object[] a)
        {
            using (sqlite3_stmt stmt = db.prepare(sql, a))
            {
                if (1 != stmt.column_count())
                {
                    throw new InvalidOperationException("the SELECT expression for query_one_column() must have exactly one column");
                }

                while (raw.SQLITE_ROW == stmt.step())
                {
                    yield return stmt.column<T>(0);
                }
            }
        }

        // allows only one statement in the sql string
        public static sqlite3_stmt prepare(this sqlite3 db, string sql, params object[] a)
        {
            sqlite3_stmt s = db.prepare(sql);
            s.bind(a);
            return s;
        }

        public static void exec(this sqlite3 db, string sql, strdelegate_exec callback, object user_data, out string errMsg)
        {
            int rc = raw.sqlite3_exec(db, sql, callback, user_data, out errMsg);
            check_ok(rc);
        }

        // allows only one statement in the sql string
        public static void exec(this sqlite3 db, string sql, params object[] a)
        {
            using (sqlite3_stmt stmt = db.prepare(sql, a))
            {
                stmt.step();
            }
        }

        public static void close(this sqlite3 db)
        {
            int rc = raw.sqlite3_close(db);
            check_ok(rc);
        }

        public static void close_v2(this sqlite3 db)
        {
            int rc = raw.sqlite3_close_v2(db);
            check_ok(rc);
        }

        public static void key(this sqlite3 db, ReadOnlySpan<byte> k)
        {
            int rc = raw.sqlite3_key(db, k);
            check_ok(rc);
        }

        public static void rekey(this sqlite3 db, ReadOnlySpan<byte> k)
        {
            int rc = raw.sqlite3_rekey(db, k);
            check_ok(rc);
        }

        public static void wal_autocheckpoint(this sqlite3 db, int n)
        {
            int rc = raw.sqlite3_wal_autocheckpoint(db, n);
            check_ok(rc);
        }

        public static void wal_checkpoint(this sqlite3 db, string dbName)
        {
            int rc = raw.sqlite3_wal_checkpoint(db, dbName);
            check_ok(rc);
        }

        public static void wal_checkpoint(this sqlite3 db, string dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            int rc = raw.sqlite3_wal_checkpoint_v2(db, dbName, eMode, out logSize, out framesCheckPointed);
            check_ok(rc);
        }

        public static int step(this sqlite3_stmt stmt)
        {
            int rc = raw.sqlite3_step(stmt);
            if (
                (rc != raw.SQLITE_ROW)
                && (rc != raw.SQLITE_DONE)
                )
            {
                throw new sqlite3_exception(rc, stmt.db_handle().errmsg());
            }

            return rc;
        }

        public static void step_row(this sqlite3_stmt stmt)
        {
            int rc = stmt.step();
            if (rc != raw.SQLITE_ROW)
            {
                throw new sqlite3_exception(rc, "expected SQLITE_ROW");
            }
        }

        public static void step_done(this sqlite3_stmt stmt)
        {
            int rc = stmt.step();
            if (rc != raw.SQLITE_DONE)
            {
                throw new sqlite3_exception(rc, "expected SQLITE_DONE");
            }
        }

        public static int stmt_isexplain(this sqlite3_stmt stmt)
        {
            return raw.sqlite3_stmt_isexplain(stmt);
        }

        public static int stmt_busy(this sqlite3_stmt stmt)
        {
            return raw.sqlite3_stmt_busy(stmt);
        }

        public static int stmt_readonly(this sqlite3_stmt stmt)
        {
            return raw.sqlite3_stmt_readonly(stmt);
        }

        public static string column_name(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_name(stmt, index).utf8_to_string();
        }

        public static string column_database_name(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_database_name(stmt, index).utf8_to_string();
        }

        public static string column_table_name(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_table_name(stmt, index).utf8_to_string();
        }

        public static string column_origin_name(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_origin_name(stmt, index).utf8_to_string();
        }

        public static string column_decltype(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_decltype(stmt, index).utf8_to_string();
        }

        public static int column_count(this sqlite3_stmt stmt)
        {
            return raw.sqlite3_column_count(stmt);
        }

        public static int data_count(this sqlite3_stmt stmt)
        {
            return raw.sqlite3_data_count(stmt);
        }

        public static string column_text(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_text(stmt, index).utf8_to_string();
        }

        public static int column_int(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_int(stmt, index);
        }

        public static int column_type(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_type(stmt, index);
        }

        public static int column_bytes(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_bytes(stmt, index);
        }

        public static long column_int64(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_int64(stmt, index);
        }

        public static double column_double(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_double(stmt, index);
        }

        public static ReadOnlySpan<byte> column_blob(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_blob(stmt, index);
        }

        // TODO maybe this doesn't need to be here.  since it cannot be
        // named finalize(), and since sqlite3_stmt implements IDisposable,
        // I'm not sure this extension method adds much value.
        public static void sqlite3_finalize(this sqlite3_stmt stmt)
        {
            //int rc = 
            raw.sqlite3_finalize(stmt);
            //check_ok(rc);
        }

        public static void bind_text(this sqlite3_stmt stmt, int index, ReadOnlySpan<byte> s)
        {
            int rc = raw.sqlite3_bind_text(stmt, index, s);
            check_ok(rc);
        }

        public static void bind_text(this sqlite3_stmt stmt, int index, utf8z s)
        {
            int rc = raw.sqlite3_bind_text(stmt, index, s);
            check_ok(rc);
        }

        public static void bind_text(this sqlite3_stmt stmt, int index, string s)
        {
            int rc = raw.sqlite3_bind_text(stmt, index, s);
            check_ok(rc);
        }

        public static void bind_blob(this sqlite3_stmt stmt, int index, ReadOnlySpan<byte> b)
        {
            int rc = raw.sqlite3_bind_blob(stmt, index, b);
            check_ok(rc);
        }

        public static void bind_zeroblob(this sqlite3_stmt stmt, int index, int size)
        {
            int rc = raw.sqlite3_bind_zeroblob(stmt, index, size);
            check_ok(rc);
        }

        public static void bind_int(this sqlite3_stmt stmt, int index, int v)
        {
            int rc = raw.sqlite3_bind_int(stmt, index, v);
            check_ok(rc);
        }

        public static void bind_int64(this sqlite3_stmt stmt, int index, long v)
        {
            int rc = raw.sqlite3_bind_int64(stmt, index, v);
            check_ok(rc);
        }

        public static void bind_double(this sqlite3_stmt stmt, int index, double v)
        {
            int rc = raw.sqlite3_bind_double(stmt, index, v);
            check_ok(rc);
        }

        public static void bind_null(this sqlite3_stmt stmt, int index)
        {
            int rc = raw.sqlite3_bind_null(stmt, index);
            check_ok(rc);
        }

        public static string bind_parameter_name(this sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_bind_parameter_name(stmt, index).utf8_to_string();
        }

        public static int bind_parameter_index(this sqlite3_stmt stmt, string name)
        {
            return raw.sqlite3_bind_parameter_index(stmt, name);
        }

        public static int bind_parameter_count(this sqlite3_stmt stmt)
        {
            return raw.sqlite3_bind_parameter_count(stmt);
        }

        public static void reset(this sqlite3_stmt stmt)
        {
            int rc = raw.sqlite3_reset(stmt);
            check_ok(rc);
        }

        public static void clear_bindings(this sqlite3_stmt stmt)
        {
            int rc = raw.sqlite3_clear_bindings(stmt);
            check_ok(rc);
        }

        public static sqlite3 db_handle(this sqlite3_stmt stmt)
        {
            return raw.sqlite3_db_handle(stmt);
        }

        public static string sql(this sqlite3_stmt stmt)
        {
            return raw.sqlite3_sql(stmt).utf8_to_string();
        }

        public static T column<T>(this sqlite3_stmt stmt, int index)
        {
            return (T)stmt.column(index, typeof(T));
        }

        public static object column(this sqlite3_stmt stmt, int index, Type t)
        {
            if (typeof(String) == t)
            {
                return stmt.column_text(index);
            }
            else if (
                       (typeof(Int32) == t)
                    || (typeof(Boolean) == t)
                    || (typeof(Byte) == t)
                    || (typeof(UInt16) == t)
                    || (typeof(Int16) == t)
                    || (typeof(sbyte) == t)
                    )
            {
                return Convert.ChangeType(stmt.column_int(index), t, null);
            }
            else if (
                       (typeof(double) == t)
                    || (typeof(float) == t)
                    )
            {
                return Convert.ChangeType(stmt.column_double(index), t, null);
            }
            else if (typeof(DateTime) == t)
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return origin.AddSeconds(stmt.column_int64(index));
            }
            else if (
                       (typeof(Int64) == t)
                    || (typeof(UInt32) == t)
                    )
            {
                return Convert.ChangeType(stmt.column_int64(index), t, null);
            }
            else if (typeof(System.Nullable<long>) == t)
            {
                if (stmt.column_type(index) == raw.SQLITE_NULL)
                {
                    return null;
                }
                else
                {
                    long? x = stmt.column_int64(index);
                    return x;
                }
            }
            else if (typeof(System.Nullable<double>) == t)
            {
                if (stmt.column_type(index) == raw.SQLITE_NULL)
                {
                    return null;
                }
                else
                {
                    double? x = stmt.column_double(index);
                    return x;
                }
            }
            else if (typeof(System.Nullable<int>) == t)
            {
                if (stmt.column_type(index) == raw.SQLITE_NULL)
                {
                    return null;
                }
                else
                {
                    int? x = stmt.column_int(index);
                    return x;
                }
            }
            else if (typeof(byte[]) == t)
            {
                // TODO hmmm.  how should this function adapt to Span/Memory ?
                // need a way to ask for ReadOnlySpan<byte> ?
                if (stmt.column_type(index) == raw.SQLITE_NULL)
                {
                    return null;
                }
                else
                {
                    return stmt.column_blob(index).ToArray();
                }
            }
            else
            {
                throw new NotSupportedException("Invalid type conversion" + t);
            }
        }

        public static T row<T>(this sqlite3_stmt stmt) where T : new()
        {
            Type typ = typeof(T);
            var obj = new T();
            for (int i = 0; i < stmt.column_count(); i++)
            {
                string colname = stmt.column_name(i);

#if OLD_REFLECTION
                var prop = typ.GetProperty(colname);
#else
                var prop = typ.GetTypeInfo().GetDeclaredProperty(colname);
#endif
                if (
                        (null != prop)
                        && prop.CanWrite
                        )
                {
                    prop.SetValue(obj, stmt.column(i, prop.PropertyType), null);
                }
                else
                {
                    throw new NotSupportedException("property not found");
                }

            }
            return obj;
        }

        public static void bind(this sqlite3_stmt stmt, params object[] a)
        {
            if (a == null)
            {
                return;
            }

            int count = Math.Min(stmt.bind_parameter_count(), a.Length);
            // TODO instead of Math.Min(), consider comparing the two
            // counts and throwing if they're not equal.

            for (int i = 0; i < count; i++)
            {
                int ndx = i + 1;
                if (a[i] == null)
                {
                    //Console.WriteLine("bind: {0} null", i);
                    stmt.bind_null(ndx);
                }
                else
                {
                    Type t = a[i].GetType();
                    //Console.WriteLine("bind: {0} {1} -- {2}", i, t, a[i]);
                    if (typeof(String) == t)
                    {
                        stmt.bind_text(ndx, (string)a[i]);
                    }
                    else if (
                               (typeof(Int32) == t)
                            || (typeof(Boolean) == t)
                            || (typeof(Byte) == t)
                            || (typeof(UInt16) == t)
                            || (typeof(Int16) == t)
                            || (typeof(sbyte) == t)
                            || (typeof(Int64) == t)
                            || (typeof(UInt32) == t)
                            )
                    {
                        stmt.bind_int64(ndx, (long)(Convert.ChangeType(a[i], typeof(long), null)));
                    }
                    else if (
                               (typeof(double) == t)
                            || (typeof(float) == t)
                            )
                    {
                        stmt.bind_double(ndx, (double)(Convert.ChangeType(a[i], typeof(double), null)));
                    }
                    else if (typeof(DateTime) == t)
                    {
                        DateTime d = (DateTime)a[i];
                        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        TimeSpan diff = d.ToUniversalTime() - origin;
                        stmt.bind_int64(ndx, (long)diff.TotalSeconds);
                    }
                    else if (typeof(byte[]) == t)
                    {
                        stmt.bind_blob(ndx, (byte[])a[i]);
                    }
                    else
                    {
                        throw new NotSupportedException("Invalid type conversion" + t);
                    }
                }
            }
        }

        public static void bind(this sqlite3_stmt stmt, int ndx, double v)
        {
            stmt.bind_double(ndx, v);
        }

        public static void bind(this sqlite3_stmt stmt, int ndx, long v)
        {
            stmt.bind_int64(ndx, v);
        }

        public static void bind(this sqlite3_stmt stmt, int ndx, int v)
        {
            stmt.bind_int(ndx, v);
        }

        public static void bind(this sqlite3_stmt stmt, int ndx, string v)
        {
            stmt.bind_text(ndx, v);
        }

        public static void bind(this sqlite3_stmt stmt, int ndx, byte[] v)
        {
            stmt.bind_blob(ndx, v);
        }

        public static void write(this sqlite3_blob blob, ReadOnlySpan<byte> b, int offset)
        {
            int rc = raw.sqlite3_blob_write(blob, b, offset);
            check_ok(rc);
        }

        public static void read(this sqlite3_blob blob, Span<byte> b, int offset)
        {
            int rc = raw.sqlite3_blob_read(blob, b, offset);
            check_ok(rc);
        }

        public static int bytes(this sqlite3_blob blob)
        {
            return raw.sqlite3_blob_bytes(blob);
        }

        public static void reopen(this sqlite3_blob blob, long rowid)
        {
            int rc = raw.sqlite3_blob_reopen(blob, rowid);
            check_ok(rc);
        }

        public static void close(this sqlite3_blob blob)
        {
            int rc = raw.sqlite3_blob_close(blob);
            check_ok(rc);
        }

        public static int step(this sqlite3_backup backup, int nPage)
        {
            return raw.sqlite3_backup_step(backup, nPage);
        }

        public static int finish(this sqlite3_backup backup)
        {
            return raw.sqlite3_backup_finish(backup);
        }

        public static int remaining(this sqlite3_backup backup)
        {
            return raw.sqlite3_backup_remaining(backup);
        }

        public static int pagecount(this sqlite3_backup backup)
        {
            return raw.sqlite3_backup_pagecount(backup);
        }

        public static void result_text(this sqlite3_context ctx, ReadOnlySpan<byte> s)
        {
            raw.sqlite3_result_text(ctx, s);
        }

        public static void result_text(this sqlite3_context ctx, utf8z s)
        {
            raw.sqlite3_result_text(ctx, s);
        }

        public static void result_text(this sqlite3_context ctx, string s)
        {
            raw.sqlite3_result_text(ctx, s);
        }

        public static void result_blob(this sqlite3_context ctx, ReadOnlySpan<byte> b)
        {
            raw.sqlite3_result_blob(ctx, b);
        }

        public static void result_int(this sqlite3_context ctx, int v)
        {
            raw.sqlite3_result_int(ctx, v);
        }

        public static void result_int64(this sqlite3_context ctx, long v)
        {
            raw.sqlite3_result_int64(ctx, v);
        }

        public static void result_double(this sqlite3_context ctx, double v)
        {
            raw.sqlite3_result_double(ctx, v);
        }

        public static void result_null(this sqlite3_context ctx)
        {
            raw.sqlite3_result_null(ctx);
        }

        public static string value_text(this sqlite3_value val)
        {
            return raw.sqlite3_value_text(val).utf8_to_string();
        }

        public static int value_int(this sqlite3_value val)
        {
            return raw.sqlite3_value_int(val);
        }

        public static int value_type(this sqlite3_value val)
        {
            return raw.sqlite3_value_type(val);
        }

        public static int value_bytes(this sqlite3_value val)
        {
            return raw.sqlite3_value_bytes(val);
        }

        public static long value_int64(this sqlite3_value val)
        {
            return raw.sqlite3_value_int64(val);
        }

        public static double value_double(this sqlite3_value val)
        {
            return raw.sqlite3_value_double(val);
        }

        public static ReadOnlySpan<byte> value_blob(this sqlite3_value val)
        {
            return raw.sqlite3_value_blob(val);
        }

    }

}
