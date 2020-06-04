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

namespace SQLitePCL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public delegate int strdelegate_collation(object user_data, string s1, string s2);
    public delegate void strdelegate_update(object user_data, int type, string database, string table, long rowid);
    public delegate void strdelegate_log(object user_data, int errorCode, string msg);
    public delegate int strdelegate_authorizer(object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view);
    public delegate void strdelegate_trace(object user_data, string s);
    public delegate void strdelegate_profile(object user_data, string statement, long ns);
    public delegate int strdelegate_exec(object user_data, string[] values, string[] names);

    public static class raw
    {
        private static ISQLite3Provider _imp;
        private static bool _frozen;

        static raw()
        {
            _frozen = false;
        }

        static public void SetProvider(ISQLite3Provider imp)
        {
            if (_frozen) return;
            int version = imp.sqlite3_libversion_number();
#if not // don't do this, because it ends up calling sqlite3_initialize
		IntPtr db;
		int rc;
	        rc = imp.sqlite3_open(":memory:", out db);
		if (rc != 0) throw new Exception();
		rc = imp.sqlite3_close(db);
		if (rc != 0) throw new Exception();
#endif
            _imp = imp;
        }

        static public void FreezeProvider(bool b = true)
        {
            _frozen = b;
        }

        private static ISQLite3Provider Provider
        {
            get
            {
                if (_imp == null) throw new Exception("You need to call SQLitePCL.raw.SetProvider().  If you are using a bundle package, this is done by calling SQLitePCL.Batteries.Init().");
                return _imp;
            }
        }

        static public string GetNativeLibraryName()
        {
            return Provider.GetNativeLibraryName();
        }

        public const int SQLITE_UTF8 = 1;
        public const int SQLITE_UTF16LE = 2;
        public const int SQLITE_UTF16BE = 3;
        public const int SQLITE_UTF16 = 4;  /* Use native byte order */
        public const int SQLITE_ANY = 5;  /* sqlite3_create_function only */
        public const int SQLITE_UTF16_ALIGNED = 8;  /* sqlite3_create_function only */

        public const int SQLITE_DETERMINISTIC = 0x800;

        public const int SQLITE_CONFIG_SINGLETHREAD = 1;  /* nil */
        public const int SQLITE_CONFIG_MULTITHREAD = 2;  /* nil */
        public const int SQLITE_CONFIG_SERIALIZED = 3;  /* nil */
        public const int SQLITE_CONFIG_MALLOC = 4;  /* sqlite3_mem_methods* */
        public const int SQLITE_CONFIG_GETMALLOC = 5;  /* sqlite3_mem_methods* */
        public const int SQLITE_CONFIG_SCRATCH = 6;  /* void*, int utf8z, int N */
        public const int SQLITE_CONFIG_PAGECACHE = 7;  /* void*, int utf8z, int N */
        public const int SQLITE_CONFIG_HEAP = 8;  /* void*, int nByte, int min */
        public const int SQLITE_CONFIG_MEMSTATUS = 9;  /* boolean */
        public const int SQLITE_CONFIG_MUTEX = 10;  /* sqlite3_mutex_methods* */
        public const int SQLITE_CONFIG_GETMUTEX = 11;  /* sqlite3_mutex_methods* */
                                                       /* previously SQLITE_CONFIG_CHUNKALLOC 12 which is now unused. */
        public const int SQLITE_CONFIG_LOOKASIDE = 13;  /* int int */
        public const int SQLITE_CONFIG_PCACHE = 14;  /* no-op */
        public const int SQLITE_CONFIG_GETPCACHE = 15;  /* no-op */
        public const int SQLITE_CONFIG_LOG = 16;  /* xFunc, void* */
        public const int SQLITE_CONFIG_URI = 17;  /* int */
        public const int SQLITE_CONFIG_PCACHE2 = 18;  /* sqlite3_pcache_methods2* */
        public const int SQLITE_CONFIG_GETPCACHE2 = 19;  /* sqlite3_pcache_methods2* */
        public const int SQLITE_CONFIG_COVERING_INDEX_SCAN = 20;  /* int */
        public const int SQLITE_CONFIG_SQLLOG = 21;  /* xSqllog, void* */

        public const int SQLITE_OPEN_READONLY = 0x00000001;  /* Ok for sqlite3_open_v2() */
        public const int SQLITE_OPEN_READWRITE = 0x00000002;  /* Ok for sqlite3_open_v2() */
        public const int SQLITE_OPEN_CREATE = 0x00000004;  /* Ok for sqlite3_open_v2() */
        public const int SQLITE_OPEN_DELETEONCLOSE = 0x00000008;  /* VFS only */
        public const int SQLITE_OPEN_EXCLUSIVE = 0x00000010;  /* VFS only */
        public const int SQLITE_OPEN_AUTOPROXY = 0x00000020;  /* VFS only */
        public const int SQLITE_OPEN_URI = 0x00000040;  /* Ok for sqlite3_open_v2() */
        public const int SQLITE_OPEN_MEMORY = 0x00000080;  /* Ok for sqlite3_open_v2() */
        public const int SQLITE_OPEN_MAIN_DB = 0x00000100;  /* VFS only */
        public const int SQLITE_OPEN_TEMP_DB = 0x00000200;  /* VFS only */
        public const int SQLITE_OPEN_TRANSIENT_DB = 0x00000400;  /* VFS only */
        public const int SQLITE_OPEN_MAIN_JOURNAL = 0x00000800;  /* VFS only */
        public const int SQLITE_OPEN_TEMP_JOURNAL = 0x00001000;  /* VFS only */
        public const int SQLITE_OPEN_SUBJOURNAL = 0x00002000;  /* VFS only */
        public const int SQLITE_OPEN_MASTER_JOURNAL = 0x00004000;  /* VFS only */
        public const int SQLITE_OPEN_NOMUTEX = 0x00008000;  /* Ok for sqlite3_open_v2() */
        public const int SQLITE_OPEN_FULLMUTEX = 0x00010000;  /* Ok for sqlite3_open_v2() */
        public const int SQLITE_OPEN_SHAREDCACHE = 0x00020000;  /* Ok for sqlite3_open_v2() */
        public const int SQLITE_OPEN_PRIVATECACHE = 0x00040000;  /* Ok for sqlite3_open_v2() */
        public const int SQLITE_OPEN_WAL = 0x00080000;  /* VFS only */

        public const int SQLITE_PREPARE_PERSISTENT = 0x01;
        public const int SQLITE_PREPARE_NORMALIZE = 0x02;
        public const int SQLITE_PREPARE_NO_VTAB = 0x04;

        public const int SQLITE_INTEGER = 1;
        public const int SQLITE_FLOAT = 2;
        public const int SQLITE_TEXT = 3;
        public const int SQLITE_BLOB = 4;
        public const int SQLITE_NULL = 5;

        public const int SQLITE_OK = 0;
        public const int SQLITE_ERROR = 1;
        public const int SQLITE_INTERNAL = 2;
        public const int SQLITE_PERM = 3;
        public const int SQLITE_ABORT = 4;
        public const int SQLITE_BUSY = 5;
        public const int SQLITE_LOCKED = 6;
        public const int SQLITE_NOMEM = 7;
        public const int SQLITE_READONLY = 8;
        public const int SQLITE_INTERRUPT = 9;
        public const int SQLITE_IOERR = 10;
        public const int SQLITE_CORRUPT = 11;
        public const int SQLITE_NOTFOUND = 12;
        public const int SQLITE_FULL = 13;
        public const int SQLITE_CANTOPEN = 14;
        public const int SQLITE_PROTOCOL = 15;
        public const int SQLITE_EMPTY = 16;
        public const int SQLITE_SCHEMA = 17;
        public const int SQLITE_TOOBIG = 18;
        public const int SQLITE_CONSTRAINT = 19;
        public const int SQLITE_MISMATCH = 20;
        public const int SQLITE_MISUSE = 21;
        public const int SQLITE_NOLFS = 22;
        public const int SQLITE_AUTH = 23;
        public const int SQLITE_FORMAT = 24;
        public const int SQLITE_RANGE = 25;
        public const int SQLITE_NOTADB = 26;
        public const int SQLITE_NOTICE = 27;
        public const int SQLITE_WARNING = 28;
        public const int SQLITE_ROW = 100;
        public const int SQLITE_DONE = 101;

        public const int SQLITE_IOERR_READ = (SQLITE_IOERR | (1 << 8));
        public const int SQLITE_IOERR_SHORT_READ = (SQLITE_IOERR | (2 << 8));
        public const int SQLITE_IOERR_WRITE = (SQLITE_IOERR | (3 << 8));
        public const int SQLITE_IOERR_FSYNC = (SQLITE_IOERR | (4 << 8));
        public const int SQLITE_IOERR_DIR_FSYNC = (SQLITE_IOERR | (5 << 8));
        public const int SQLITE_IOERR_TRUNCATE = (SQLITE_IOERR | (6 << 8));
        public const int SQLITE_IOERR_FSTAT = (SQLITE_IOERR | (7 << 8));
        public const int SQLITE_IOERR_UNLOCK = (SQLITE_IOERR | (8 << 8));
        public const int SQLITE_IOERR_RDLOCK = (SQLITE_IOERR | (9 << 8));
        public const int SQLITE_IOERR_DELETE = (SQLITE_IOERR | (10 << 8));
        public const int SQLITE_IOERR_BLOCKED = (SQLITE_IOERR | (11 << 8));
        public const int SQLITE_IOERR_NOMEM = (SQLITE_IOERR | (12 << 8));
        public const int SQLITE_IOERR_ACCESS = (SQLITE_IOERR | (13 << 8));
        public const int SQLITE_IOERR_CHECKRESERVEDLOCK = (SQLITE_IOERR | (14 << 8));
        public const int SQLITE_IOERR_LOCK = (SQLITE_IOERR | (15 << 8));
        public const int SQLITE_IOERR_CLOSE = (SQLITE_IOERR | (16 << 8));
        public const int SQLITE_IOERR_DIR_CLOSE = (SQLITE_IOERR | (17 << 8));
        public const int SQLITE_IOERR_SHMOPEN = (SQLITE_IOERR | (18 << 8));
        public const int SQLITE_IOERR_SHMSIZE = (SQLITE_IOERR | (19 << 8));
        public const int SQLITE_IOERR_SHMLOCK = (SQLITE_IOERR | (20 << 8));
        public const int SQLITE_IOERR_SHMMAP = (SQLITE_IOERR | (21 << 8));
        public const int SQLITE_IOERR_SEEK = (SQLITE_IOERR | (22 << 8));
        public const int SQLITE_IOERR_DELETE_NOENT = (SQLITE_IOERR | (23 << 8));
        public const int SQLITE_IOERR_MMAP = (SQLITE_IOERR | (24 << 8));
        public const int SQLITE_IOERR_GETTEMPPATH = (SQLITE_IOERR | (25 << 8));
        public const int SQLITE_IOERR_CONVPATH = (SQLITE_IOERR | (26 << 8));
        public const int SQLITE_LOCKED_SHAREDCACHE = (SQLITE_LOCKED | (1 << 8));
        public const int SQLITE_BUSY_RECOVERY = (SQLITE_BUSY | (1 << 8));
        public const int SQLITE_BUSY_SNAPSHOT = (SQLITE_BUSY | (2 << 8));
        public const int SQLITE_CANTOPEN_NOTEMPDIR = (SQLITE_CANTOPEN | (1 << 8));
        public const int SQLITE_CANTOPEN_ISDIR = (SQLITE_CANTOPEN | (2 << 8));
        public const int SQLITE_CANTOPEN_FULLPATH = (SQLITE_CANTOPEN | (3 << 8));
        public const int SQLITE_CANTOPEN_CONVPATH = (SQLITE_CANTOPEN | (4 << 8));
        public const int SQLITE_CORRUPT_VTAB = (SQLITE_CORRUPT | (1 << 8));
        public const int SQLITE_READONLY_RECOVERY = (SQLITE_READONLY | (1 << 8));
        public const int SQLITE_READONLY_CANTLOCK = (SQLITE_READONLY | (2 << 8));
        public const int SQLITE_READONLY_ROLLBACK = (SQLITE_READONLY | (3 << 8));
        public const int SQLITE_READONLY_DBMOVED = (SQLITE_READONLY | (4 << 8));
        public const int SQLITE_ABORT_ROLLBACK = (SQLITE_ABORT | (2 << 8));
        public const int SQLITE_CONSTRAINT_CHECK = (SQLITE_CONSTRAINT | (1 << 8));
        public const int SQLITE_CONSTRAINT_COMMITHOOK = (SQLITE_CONSTRAINT | (2 << 8));
        public const int SQLITE_CONSTRAINT_FOREIGNKEY = (SQLITE_CONSTRAINT | (3 << 8));
        public const int SQLITE_CONSTRAINT_FUNCTION = (SQLITE_CONSTRAINT | (4 << 8));
        public const int SQLITE_CONSTRAINT_NOTNULL = (SQLITE_CONSTRAINT | (5 << 8));
        public const int SQLITE_CONSTRAINT_PRIMARYKEY = (SQLITE_CONSTRAINT | (6 << 8));
        public const int SQLITE_CONSTRAINT_TRIGGER = (SQLITE_CONSTRAINT | (7 << 8));
        public const int SQLITE_CONSTRAINT_UNIQUE = (SQLITE_CONSTRAINT | (8 << 8));
        public const int SQLITE_CONSTRAINT_VTAB = (SQLITE_CONSTRAINT | (9 << 8));
        public const int SQLITE_CONSTRAINT_ROWID = (SQLITE_CONSTRAINT | (10 << 8));
        public const int SQLITE_NOTICE_RECOVER_WAL = (SQLITE_NOTICE | (1 << 8));
        public const int SQLITE_NOTICE_RECOVER_ROLLBACK = (SQLITE_NOTICE | (2 << 8));
        public const int SQLITE_WARNING_AUTOINDEX = (SQLITE_WARNING | (1 << 8));

        public const int SQLITE_CREATE_INDEX = 1;    /* Index Name      Table Name      */
        public const int SQLITE_CREATE_TABLE = 2;    /* Table Name      NULL            */
        public const int SQLITE_CREATE_TEMP_INDEX = 3;    /* Index Name      Table Name      */
        public const int SQLITE_CREATE_TEMP_TABLE = 4;    /* Table Name      NULL            */
        public const int SQLITE_CREATE_TEMP_TRIGGER = 5;    /* Trigger Name    Table Name      */
        public const int SQLITE_CREATE_TEMP_VIEW = 6;    /* View Name       NULL            */
        public const int SQLITE_CREATE_TRIGGER = 7;    /* Trigger Name    Table Name      */
        public const int SQLITE_CREATE_VIEW = 8;    /* View Name       NULL            */
        public const int SQLITE_DELETE = 9;    /* Table Name      NULL            */
        public const int SQLITE_DROP_INDEX = 10;   /* Index Name      Table Name      */
        public const int SQLITE_DROP_TABLE = 11;   /* Table Name      NULL            */
        public const int SQLITE_DROP_TEMP_INDEX = 12;   /* Index Name      Table Name      */
        public const int SQLITE_DROP_TEMP_TABLE = 13;   /* Table Name      NULL            */
        public const int SQLITE_DROP_TEMP_TRIGGER = 14;   /* Trigger Name    Table Name      */
        public const int SQLITE_DROP_TEMP_VIEW = 15;   /* View Name       NULL            */
        public const int SQLITE_DROP_TRIGGER = 16;   /* Trigger Name    Table Name      */
        public const int SQLITE_DROP_VIEW = 17;   /* View Name       NULL            */
        public const int SQLITE_INSERT = 18;   /* Table Name      NULL            */
        public const int SQLITE_PRAGMA = 19;   /* Pragma Name     1st arg or NULL */
        public const int SQLITE_READ = 20;   /* Table Name      Column Name     */
        public const int SQLITE_SELECT = 21;   /* NULL            NULL            */
        public const int SQLITE_TRANSACTION = 22;   /* Operation       NULL            */
        public const int SQLITE_UPDATE = 23;   /* Table Name      Column Name     */
        public const int SQLITE_ATTACH = 24;   /* Filename        NULL            */
        public const int SQLITE_DETACH = 25;   /* Database Name   NULL            */
        public const int SQLITE_ALTER_TABLE = 26;   /* Database Name   Table Name      */
        public const int SQLITE_REINDEX = 27;   /* Index Name      NULL            */
        public const int SQLITE_ANALYZE = 28;   /* Table Name      NULL            */
        public const int SQLITE_CREATE_VTABLE = 29;   /* Table Name      Module Name     */
        public const int SQLITE_DROP_VTABLE = 30;   /* Table Name      Module Name     */
        public const int SQLITE_FUNCTION = 31;   /* NULL            Function Name   */
        public const int SQLITE_SAVEPOINT = 32;   /* Operation       Savepoint Name  */
        public const int SQLITE_COPY = 0;    /* No longer used */
        public const int SQLITE_RECURSIVE = 33;   /* NULL            NULL            */

        public const int SQLITE_CHECKPOINT_PASSIVE = 0;
        public const int SQLITE_CHECKPOINT_FULL = 1;
        public const int SQLITE_CHECKPOINT_RESTART = 2;
        public const int SQLITE_CHECKPOINT_TRUNCATE = 3;

        public const int SQLITE_DBSTATUS_LOOKASIDE_USED = 0;
        public const int SQLITE_DBSTATUS_CACHE_USED = 1;
        public const int SQLITE_DBSTATUS_SCHEMA_USED = 2;
        public const int SQLITE_DBSTATUS_STMT_USED = 3;
        public const int SQLITE_DBSTATUS_LOOKASIDE_HIT = 4;
        public const int SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE = 5;
        public const int SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL = 6;
        public const int SQLITE_DBSTATUS_CACHE_HIT = 7;
        public const int SQLITE_DBSTATUS_CACHE_MISS = 8;
        public const int SQLITE_DBSTATUS_CACHE_WRITE = 9;
        public const int SQLITE_DBSTATUS_DEFERRED_FKS = 10;

        public const int SQLITE_STATUS_MEMORY_USED = 0;
        public const int SQLITE_STATUS_PAGECACHE_USED = 1;
        public const int SQLITE_STATUS_PAGECACHE_OVERFLOW = 2;
        public const int SQLITE_STATUS_SCRATCH_USED = 3;
        public const int SQLITE_STATUS_SCRATCH_OVERFLOW = 4;
        public const int SQLITE_STATUS_MALLOC_SIZE = 5;
        public const int SQLITE_STATUS_PARSER_STACK = 6;
        public const int SQLITE_STATUS_PAGECACHE_SIZE = 7;
        public const int SQLITE_STATUS_SCRATCH_SIZE = 8;
        public const int SQLITE_STATUS_MALLOC_COUNT = 9;

        public const int SQLITE_STMTSTATUS_FULLSCAN_STEP = 1;
        public const int SQLITE_STMTSTATUS_SORT = 2;
        public const int SQLITE_STMTSTATUS_AUTOINDEX = 3;
        public const int SQLITE_STMTSTATUS_VM_STEP = 4;

        // Authorizer Return Codes
        public const int SQLITE_DENY = 1;   /* Abort the SQL statement with an error */
        public const int SQLITE_IGNORE = 2;   /* Don't allow access, but don't generate an error */

        public const int SQLITE_TRACE_STMT = 0x01;
        public const int SQLITE_TRACE_PROFILE = 0x02;
        public const int SQLITE_TRACE_ROW = 0x04;
        public const int SQLITE_TRACE_CLOSE = 0x08;

        static public int sqlite3_open(utf8z filename, out sqlite3 db)
        {
            int rc = Provider.sqlite3_open(filename, out var p_db);
            // TODO check rc?
            db = sqlite3.New(p_db);
            return rc;
        }

        static public int sqlite3_open(string filename, out sqlite3 db)
        {
            return sqlite3_open(filename.to_utf8z(), out db);
        }

        static public int sqlite3_open_v2(utf8z filename, out sqlite3 db, int flags, utf8z vfs)
        {
            int rc = Provider.sqlite3_open_v2(filename, out var p_db, flags, vfs);
            // TODO check rc?
            db = sqlite3.New(p_db);
            return rc;
        }
        static public int sqlite3_open_v2(string filename, out sqlite3 db, int flags, string vfs)
        {
            return sqlite3_open_v2(filename.to_utf8z(), out db, flags, vfs.to_utf8z());
        }
        static public int sqlite3__vfs__delete(utf8z vfs, utf8z pathname, int syncdir)
        {
            return Provider.sqlite3__vfs__delete(vfs, pathname, syncdir);
        }
        static public int sqlite3__vfs__delete(string vfs, string pathname, int syncdir)
        {
            return sqlite3__vfs__delete(vfs.to_utf8z(), pathname.to_utf8z(), syncdir);
        }

        // called by the SafeHandle
        static internal int internal_sqlite3_close_v2(IntPtr p)
        {
            return Provider.sqlite3_close_v2(p);
        }

        // called by the SafeHandle
        static internal int internal_sqlite3_close(IntPtr p)
        {
            return Provider.sqlite3_close(p);
        }

        // called by apps that want the return code
        static public int sqlite3_close_v2(sqlite3 db)
        {
            return db.manual_close_v2();
        }

        // called by apps that want the return code
        static public int sqlite3_close(sqlite3 db)
        {
            return db.manual_close();
        }

        static public int sqlite3_enable_shared_cache(int enable)
        {
            return Provider.sqlite3_enable_shared_cache(enable);
        }

        static public void sqlite3_interrupt(sqlite3 db)
        {
            Provider.sqlite3_interrupt(db);
        }

        static public int sqlite3_config_log(delegate_log f, object v)
        {
            return Provider.sqlite3_config_log(f, v);
        }

        static public int sqlite3_config_log(strdelegate_log f, object v)
        {
            delegate_log cb;
            if (f == null)
            {
                cb = null;
            }
            else
            {
                cb =
                (ob, e, msg) =>
                {
                    f(ob, e, msg.utf8_to_string());
                };
            }
            return sqlite3_config_log(cb, v);
        }

        static public void sqlite3_log(int errcode, utf8z s)
        {
            Provider.sqlite3_log(errcode, s);
        }

        static public void sqlite3_log(int errcode, string s)
        {
            sqlite3_log(errcode, s.to_utf8z());
        }

        static public void sqlite3_commit_hook(sqlite3 db, delegate_commit f, object v)
        {
            Provider.sqlite3_commit_hook(db, f, v);
        }

        static public void sqlite3_rollback_hook(sqlite3 db, delegate_rollback f, object v)
        {
            Provider.sqlite3_rollback_hook(db, f, v);
        }

        static public void sqlite3_trace(sqlite3 db, delegate_trace f, object v)
        {
            Provider.sqlite3_trace(db, f, v);
        }

        static public void sqlite3_trace(sqlite3 db, strdelegate_trace f, object v)
        {
            delegate_trace cb;
            if (f == null)
            {
                cb = null;
            }
            else
            {
                cb =
                (ob, sp) =>
                {
                    f(v, sp.utf8_to_string());
                };
            }
            sqlite3_trace(db, cb, v);
        }

        static public void sqlite3_profile(sqlite3 db, delegate_profile f, object v)
        {
            Provider.sqlite3_profile(db, f, v);
        }

        static public void sqlite3_profile(sqlite3 db, strdelegate_profile f, object v)
        {
            delegate_profile cb;
            if (f == null)
            {
                cb = null;
            }
            else
            {
                cb =
                (ob, sp, ns) =>
                {
                    f(v, sp.utf8_to_string(), ns);
                };
            }
            sqlite3_profile(db, cb, v);
        }

        static public void sqlite3_progress_handler(sqlite3 db, int instructions, delegate_progress func, object v)
        {
            Provider.sqlite3_progress_handler(db, instructions, func, v);
        }

        static public void sqlite3_update_hook(sqlite3 db, delegate_update f, object v)
        {
            Provider.sqlite3_update_hook(db, f, v);
        }

        static public void sqlite3_update_hook(sqlite3 db, strdelegate_update f, object v)
        {
            delegate_update cb;
            if (f == null)
            {
                cb = null;
            }
            else
            {
                cb =
                (ob, typ, dbname, tbl, rowid) =>
                {
                    f(ob, typ, dbname.utf8_to_string(), tbl.utf8_to_string(), rowid);
                };
            }
            sqlite3_update_hook(db, cb, v);
        }

        static public int sqlite3_create_collation(sqlite3 db, string name, object v, strdelegate_collation f)
        {
            var p = name.to_utf8_with_z();
            delegate_collation cb;
            if (f == null)
            {
                cb = null;
            }
            else
            {
                cb =
                (ob, s1, s2) =>
                {
                    return f(ob, s1.utf8_span_to_string(), s2.utf8_span_to_string());
                };
            }
            return Provider.sqlite3_create_collation(db, p, v, cb);
        }

        static public int sqlite3_create_function(sqlite3 db, string name, int nArg, int flags, object v, delegate_function_scalar func)
        {
            var p = name.to_utf8_with_z();
            var rc = Provider.sqlite3_create_function(db, p, nArg, flags, v, func);
            return rc;
        }

        static public int sqlite3_create_function(sqlite3 db, string name, int nArg, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
            var p = name.to_utf8_with_z();
            var rc = Provider.sqlite3_create_function(db, p, nArg, flags, v, func_step, func_final);
            return rc;
        }

        static public int sqlite3_create_function(sqlite3 db, string name, int nArg, object v, delegate_function_scalar func)
        {
            return sqlite3_create_function(db, name, nArg, 0, v, func);
        }

        static public int sqlite3_create_function(sqlite3 db, string name, int nArg, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
            return sqlite3_create_function(db, name, nArg, 0, v, func_step, func_final);
        }

        static public int sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg)
        {
            return Provider.sqlite3_db_status(db, op, out current, out highest, resetFlg);
        }

        // TODO do we need this to be public?
        public static string utf8_span_to_string(this ReadOnlySpan<byte> p)
        {
            if (p.Length == 0)
            {
                return "";
            }
            unsafe
            {
                fixed (byte* q = p)
                {
                    return System.Text.Encoding.UTF8.GetString(q, p.Length);
                }
            }
        }

        static public int sqlite3_key(sqlite3 db, ReadOnlySpan<byte> k)
        {
            return Provider.sqlite3_key(db, k);
        }

        static public int sqlite3_key_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
        {
            return Provider.sqlite3_key_v2(db, name, k);
        }

        static public int sqlite3_rekey(sqlite3 db, ReadOnlySpan<byte> k)
        {
            return Provider.sqlite3_rekey(db, k);
        }

        static public int sqlite3_rekey_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
        {
            return Provider.sqlite3_rekey_v2(db, name, k);
        }

        static public utf8z sqlite3_libversion()
        {
            return Provider.sqlite3_libversion();
        }

        static public int sqlite3_libversion_number()
        {
            return Provider.sqlite3_libversion_number();
        }

        static public int sqlite3_threadsafe()
        {
            return Provider.sqlite3_threadsafe();
        }

        static public int sqlite3_initialize()
        {
            return Provider.sqlite3_initialize();
        }

        static public int sqlite3_shutdown()
        {
            return Provider.sqlite3_shutdown();
        }

        static public int sqlite3_config(int op)
        {
            return Provider.sqlite3_config(op);
        }

        static public int sqlite3_config(int op, int val)
        {
            return Provider.sqlite3_config(op, val);
        }

        static public int sqlite3_enable_load_extension(sqlite3 db, int onoff)
        {
            return Provider.sqlite3_enable_load_extension(db, onoff);
        }

        static public utf8z sqlite3_sourceid()
        {
            return Provider.sqlite3_sourceid();
        }

        static public long sqlite3_memory_used()
        {
            return Provider.sqlite3_memory_used();
        }

        static public long sqlite3_memory_highwater(int resetFlag)
        {
            return Provider.sqlite3_memory_highwater(resetFlag);
        }

        static public int sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            return Provider.sqlite3_status(op, out current, out highwater, resetFlag);
        }

        static public utf8z sqlite3_errmsg(sqlite3 db)
        {
            return Provider.sqlite3_errmsg(db);
        }

        static public int sqlite3_db_readonly(sqlite3 db, utf8z dbName)
        {
            return Provider.sqlite3_db_readonly(db, dbName);
        }
        static public int sqlite3_db_readonly(sqlite3 db, string dbName)
        {
            return sqlite3_db_readonly(db, dbName.to_utf8z());
        }

        static public utf8z sqlite3_db_filename(sqlite3 db, utf8z att)
        {
            return Provider.sqlite3_db_filename(db, att);
        }
        static public utf8z sqlite3_db_filename(sqlite3 db, string att)
        {
            return sqlite3_db_filename(db, att.to_utf8z());
        }

        static public long sqlite3_last_insert_rowid(sqlite3 db)
        {
            return Provider.sqlite3_last_insert_rowid(db);
        }

        static public int sqlite3_changes(sqlite3 db)
        {
            return Provider.sqlite3_changes(db);
        }

        static public int sqlite3_total_changes(sqlite3 db)
        {
            return Provider.sqlite3_total_changes(db);
        }

        static public int sqlite3_get_autocommit(sqlite3 db)
        {
            return Provider.sqlite3_get_autocommit(db);
        }

        static public int sqlite3_busy_timeout(sqlite3 db, int ms)
        {
            return Provider.sqlite3_busy_timeout(db, ms);
        }

        static public int sqlite3_extended_result_codes(sqlite3 db, int onoff)
        {
            return Provider.sqlite3_extended_result_codes(db, onoff);
        }

        static public int sqlite3_errcode(sqlite3 db)
        {
            return Provider.sqlite3_errcode(db);
        }

        static public int sqlite3_extended_errcode(sqlite3 db)
        {
            return Provider.sqlite3_extended_errcode(db);
        }

        static public utf8z sqlite3_errstr(int rc)
        {
            return Provider.sqlite3_errstr(rc);
        }

        static public int sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out sqlite3_stmt stmt)
        {
            int rc = Provider.sqlite3_prepare_v2(db, sql, out var p, out var sp_tail);
            stmt = sqlite3_stmt.From(p, db);
            return rc;
        }

        static public int sqlite3_prepare_v2(sqlite3 db, utf8z sql, out sqlite3_stmt stmt)
        {
            int rc = Provider.sqlite3_prepare_v2(db, sql, out var p, out var sp_tail);
            stmt = sqlite3_stmt.From(p, db);
            return rc;
        }

        static public int sqlite3_prepare_v2(sqlite3 db, string sql, out sqlite3_stmt stmt)
        {
            return sqlite3_prepare_v2(db, sql.to_utf8z(), out stmt);
        }

        static public int sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out sqlite3_stmt stmt, out ReadOnlySpan<byte> tail)
        {
            int rc = Provider.sqlite3_prepare_v2(db, sql, out var p, out tail);
            stmt = sqlite3_stmt.From(p, db);
            return rc;
        }

        static public int sqlite3_prepare_v2(sqlite3 db, utf8z sql, out sqlite3_stmt stmt, out utf8z tail)
        {
            int rc = Provider.sqlite3_prepare_v2(db, sql, out var p, out tail);
            stmt = sqlite3_stmt.From(p, db);
            return rc;
        }

        static public int sqlite3_prepare_v2(sqlite3 db, string sql, out sqlite3_stmt stmt, out string tail)
        {
            var ba = sql.to_utf8_with_z();
            var sp = new ReadOnlySpan<byte>(ba);
            int rc = sqlite3_prepare_v2(db, sp, out stmt, out var sp_tail);
            tail = utf8_span_to_string(sp_tail.Slice(0, sp_tail.Length - 1));
            return rc;
        }

        static public int sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out sqlite3_stmt stmt)
        {
            int rc = Provider.sqlite3_prepare_v3(db, sql, flags, out var p, out var sp_tail);
            stmt = sqlite3_stmt.From(p, db);
            return rc;
        }

        static public int sqlite3_prepare_v3(sqlite3 db, utf8z sql, uint flags, out sqlite3_stmt stmt)
        {
            int rc = Provider.sqlite3_prepare_v3(db, sql, flags, out var p, out var sp_tail);
            stmt = sqlite3_stmt.From(p, db);
            return rc;
        }

        static public int sqlite3_prepare_v3(sqlite3 db, string sql, uint flags, out sqlite3_stmt stmt)
        {
            return sqlite3_prepare_v3(db, sql.to_utf8z(), flags, out stmt);
        }

        static public int sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out sqlite3_stmt stmt, out ReadOnlySpan<byte> tail)
        {
            int rc = Provider.sqlite3_prepare_v3(db, sql, flags, out var p, out tail);
            stmt = sqlite3_stmt.From(p, db);
            return rc;
        }

        static public int sqlite3_prepare_v3(sqlite3 db, utf8z sql, uint flags, out sqlite3_stmt stmt, out utf8z tail)
        {
            int rc = Provider.sqlite3_prepare_v3(db, sql, flags, out var p, out tail);
            stmt = sqlite3_stmt.From(p, db);
            return rc;
        }

        static public int sqlite3_prepare_v3(sqlite3 db, string sql, uint flags, out sqlite3_stmt stmt, out string tail)
        {
            int rc = sqlite3_prepare_v3(db, sql.to_utf8z(), flags, out stmt, out var sp_tail);
            tail = sp_tail.utf8_to_string();
            return rc;
        }

        static public int sqlite3_exec(sqlite3 db, string sql, strdelegate_exec callback, object user_data, out string errMsg)
        {
            delegate_exec cb;

            if (callback != null)
            {
                cb =
                (ob, values, names) =>
                {
                    var a_v = new string[values.Length];
                    var a_n = new string[names.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        a_v[i] = util.from_utf8_z(values[i]);
                        a_n[i] = util.from_utf8_z(names[i]);
                    }
                    return callback(ob, a_v, a_n);
                };
            }
            else
            {
                cb = null;
            }

            var rc = Provider.sqlite3_exec(db, sql.to_utf8z(), cb, user_data, out var p_errMsg);
            if (p_errMsg == IntPtr.Zero)
            {
                errMsg = null;
            }
            else
            {
                errMsg = util.from_utf8_z(p_errMsg);
                Provider.sqlite3_free(p_errMsg);
            }
            return rc;
        }

        static public int sqlite3_exec(sqlite3 db, string sql, out string errMsg)
        {
            var rc = Provider.sqlite3_exec(db, sql.to_utf8z(), null, null, out var p_errMsg);
            if (p_errMsg == IntPtr.Zero)
            {
                errMsg = null;
            }
            else
            {
                errMsg = util.from_utf8_z(p_errMsg);
                Provider.sqlite3_free(p_errMsg);
            }
            return rc;
        }

        static public int sqlite3_exec(sqlite3 db, string sql)
        {
            var rc = Provider.sqlite3_exec(db, sql.to_utf8z(), null, null, out var p_errMsg);
            if (p_errMsg == IntPtr.Zero)
            {
            }
            else
            {
                Provider.sqlite3_free(p_errMsg);
            }
            return rc;
        }

        static public int sqlite3_step(sqlite3_stmt stmt)
        {
            return Provider.sqlite3_step(stmt);
        }

        // called by apps that want the return code
        static public int sqlite3_finalize(sqlite3_stmt stmt)
        {
            return stmt.manual_close();
        }

        // called by the SafeHandle
        static public int internal_sqlite3_finalize(IntPtr stmt)
        {
            return Provider.sqlite3_finalize(stmt);
        }

        static public int sqlite3_reset(sqlite3_stmt stmt)
        {
            return Provider.sqlite3_reset(stmt);
        }

        static public int sqlite3_clear_bindings(sqlite3_stmt stmt)
        {
            return Provider.sqlite3_clear_bindings(stmt);
        }

        public static int sqlite3_stmt_status(sqlite3_stmt stmt, int op, int resetFlg)
        {
            return Provider.sqlite3_stmt_status(stmt, op, resetFlg);
        }

        static public int sqlite3_complete(utf8z sql)
        {
            return Provider.sqlite3_complete(sql);
        }
        static public int sqlite3_complete(string sql)
        {
            return sqlite3_complete(sql.to_utf8z());
        }

        static public int sqlite3_compileoption_used(utf8z s)
        {
            return Provider.sqlite3_compileoption_used(s);
        }
        static public int sqlite3_compileoption_used(string s)
        {
            return sqlite3_compileoption_used(s.to_utf8z());
        }

        static public utf8z sqlite3_compileoption_get(int n)
        {
            return Provider.sqlite3_compileoption_get(n);
        }

        static public int sqlite3_table_column_metadata(sqlite3 db, utf8z dbName, utf8z tblName, utf8z colName, out utf8z dataType, out utf8z collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            return Provider.sqlite3_table_column_metadata(db, dbName, tblName, colName, out dataType, out collSeq, out notNull, out primaryKey, out autoInc);
        }

        static public int sqlite3_table_column_metadata(sqlite3 db, string dbName, string tblName, string colName, out string dataType, out string collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            var rc = sqlite3_table_column_metadata(db, dbName.to_utf8z(), tblName.to_utf8z(), colName.to_utf8z(), out var p_dataType, out var p_collSeq, out notNull, out primaryKey, out autoInc);
            dataType = p_dataType.utf8_to_string();
            collSeq = p_collSeq.utf8_to_string();
            return rc;
        }

        static public utf8z sqlite3_sql(sqlite3_stmt stmt)
        {
            return Provider.sqlite3_sql(stmt);
        }

        static public sqlite3 sqlite3_db_handle(sqlite3_stmt stmt)
        {
#if not
            IntPtr p = Provider.sqlite3_db_handle(stmt.ptr);
            Assert(p == stmt.db);
#endif
            return stmt.db;
        }

        static public sqlite3_stmt sqlite3_next_stmt(sqlite3 db, sqlite3_stmt stmt)
        {
            IntPtr p = Provider.sqlite3_next_stmt(db, (stmt != null) ? stmt.ptr : IntPtr.Zero);

            if (p == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return db.find_stmt(p);
            }
        }

        static public int sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size)
        {
            return Provider.sqlite3_bind_zeroblob(stmt, index, size);
        }

        static public utf8z sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_bind_parameter_name(stmt, index);
        }

        // probably unnecessary since we pass user_data back as one of the
        // params to xFunc, xStep, and xFinal.
        static public object sqlite3_user_data(sqlite3_context context)
        {
            return context.user_data;
        }

        static public void sqlite3_result_null(sqlite3_context context)
        {
            Provider.sqlite3_result_null(context.ptr);
        }

        static public void sqlite3_result_blob(sqlite3_context context, ReadOnlySpan<byte> val)
        {
            Provider.sqlite3_result_blob(context.ptr, val);
        }

        static public void sqlite3_result_error(sqlite3_context context, ReadOnlySpan<byte> val)
        {
            Provider.sqlite3_result_error(context.ptr, val);
        }

        static public void sqlite3_result_error(sqlite3_context context, utf8z val)
        {
            Provider.sqlite3_result_error(context.ptr, val);
        }

        static public void sqlite3_result_error(sqlite3_context context, string val)
        {
            sqlite3_result_error(context, val.to_utf8z());
        }

        static public void sqlite3_result_text(sqlite3_context context, ReadOnlySpan<byte> val)
        {
            Provider.sqlite3_result_text(context.ptr, val);
        }

        static public void sqlite3_result_text(sqlite3_context context, utf8z val)
        {
            Provider.sqlite3_result_text(context.ptr, val);
        }

        static public void sqlite3_result_text(sqlite3_context context, string val)
        {
            sqlite3_result_text(context, val.to_utf8z());
        }

        static public void sqlite3_result_double(sqlite3_context context, double val)
        {
            Provider.sqlite3_result_double(context.ptr, val);
        }

        static public void sqlite3_result_int(sqlite3_context context, int val)
        {
            Provider.sqlite3_result_int(context.ptr, val);
        }

        static public void sqlite3_result_int64(sqlite3_context context, long val)
        {
            Provider.sqlite3_result_int64(context.ptr, val);
        }

        static public void sqlite3_result_zeroblob(sqlite3_context context, int n)
        {
            Provider.sqlite3_result_zeroblob(context.ptr, n);
        }

        // TODO sqlite3_result_value

        static public void sqlite3_result_error_toobig(sqlite3_context context)
        {
            Provider.sqlite3_result_error_toobig(context.ptr);
        }

        static public void sqlite3_result_error_nomem(sqlite3_context context)
        {
            Provider.sqlite3_result_error_nomem(context.ptr);
        }

        static public void sqlite3_result_error_code(sqlite3_context context, int code)
        {
            Provider.sqlite3_result_error_code(context.ptr, code);
        }

        static public ReadOnlySpan<byte> sqlite3_value_blob(sqlite3_value val)
        {
            return Provider.sqlite3_value_blob(val.ptr);
        }

        static public int sqlite3_value_bytes(sqlite3_value val)
        {
            return Provider.sqlite3_value_bytes(val.ptr);
        }

        static public double sqlite3_value_double(sqlite3_value val)
        {
            return Provider.sqlite3_value_double(val.ptr);
        }

        static public int sqlite3_value_int(sqlite3_value val)
        {
            return Provider.sqlite3_value_int(val.ptr);
        }

        static public long sqlite3_value_int64(sqlite3_value val)
        {
            return Provider.sqlite3_value_int64(val.ptr);
        }

        static public int sqlite3_value_type(sqlite3_value val)
        {
            return Provider.sqlite3_value_type(val.ptr);
        }

        static public utf8z sqlite3_value_text(sqlite3_value val)
        {
            return Provider.sqlite3_value_text(val.ptr);
        }

        static public int sqlite3_bind_blob(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> blob)
        {
            return Provider.sqlite3_bind_blob(stmt, index, blob);
        }

        static public int sqlite3_bind_double(sqlite3_stmt stmt, int index, double val)
        {
            return Provider.sqlite3_bind_double(stmt, index, val);
        }

        static public int sqlite3_bind_int(sqlite3_stmt stmt, int index, int val)
        {
            return Provider.sqlite3_bind_int(stmt, index, val);
        }

        static public int sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val)
        {
            return Provider.sqlite3_bind_int64(stmt, index, val);
        }

        static public int sqlite3_bind_null(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_bind_null(stmt, index);
        }

        static public int sqlite3_bind_text(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> val)
        {
            return Provider.sqlite3_bind_text(stmt, index, val);
        }

        static public int sqlite3_bind_text(sqlite3_stmt stmt, int index, utf8z val)
        {
            return Provider.sqlite3_bind_text(stmt, index, val);
        }

        static public int sqlite3_bind_text(sqlite3_stmt stmt, int index, string val)
        {
            return sqlite3_bind_text(stmt, index, val.to_utf8z());
        }

        static public int sqlite3_bind_parameter_count(sqlite3_stmt stmt)
        {
            return Provider.sqlite3_bind_parameter_count(stmt);
        }

        static public int sqlite3_bind_parameter_index(sqlite3_stmt stmt, utf8z strName)
        {
            return Provider.sqlite3_bind_parameter_index(stmt, strName);
        }

        static public int sqlite3_bind_parameter_index(sqlite3_stmt stmt, string strName)
        {
            return sqlite3_bind_parameter_index(stmt, strName.to_utf8z());
        }

        static public int sqlite3_stmt_isexplain(sqlite3_stmt stmt)
        {
            return _imp.sqlite3_stmt_isexplain(stmt);
        }

        static public int sqlite3_stmt_busy(sqlite3_stmt stmt)
        {
            return Provider.sqlite3_stmt_busy(stmt);
        }

        static public int sqlite3_stmt_readonly(sqlite3_stmt stmt)
        {
            return Provider.sqlite3_stmt_readonly(stmt);
        }

        static public utf8z sqlite3_column_database_name(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_database_name(stmt, index);
        }

        static public utf8z sqlite3_column_name(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_name(stmt, index);
        }

        static public utf8z sqlite3_column_origin_name(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_origin_name(stmt, index);
        }

        static public utf8z sqlite3_column_table_name(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_table_name(stmt, index);
        }

        static public utf8z sqlite3_column_text(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_text(stmt, index);
        }

        static public int sqlite3_column_count(sqlite3_stmt stmt)
        {
            return Provider.sqlite3_column_count(stmt);
        }

        static public int sqlite3_data_count(sqlite3_stmt stmt)
        {
            return Provider.sqlite3_data_count(stmt);
        }

        static public double sqlite3_column_double(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_double(stmt, index);
        }

        static public int sqlite3_column_int(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_int(stmt, index);
        }

        static public long sqlite3_column_int64(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_int64(stmt, index);
        }

        static public ReadOnlySpan<byte> sqlite3_column_blob(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_blob(stmt, index);
        }

        static public int sqlite3_column_bytes(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_bytes(stmt, index);
        }

        static public int sqlite3_column_type(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_type(stmt, index);
        }

        static public utf8z sqlite3_column_decltype(sqlite3_stmt stmt, int index)
        {
            return Provider.sqlite3_column_decltype(stmt, index);
        }

        static public sqlite3_backup sqlite3_backup_init(sqlite3 destDb, string destName, sqlite3 sourceDb, string sourceName)
        {
            return Provider.sqlite3_backup_init(destDb, destName.to_utf8z(), sourceDb, sourceName.to_utf8z());
        }

        static public int sqlite3_backup_step(sqlite3_backup backup, int nPage)
        {
            return Provider.sqlite3_backup_step(backup, nPage);
        }

        static public int sqlite3_backup_remaining(sqlite3_backup backup)
        {
            return Provider.sqlite3_backup_remaining(backup);
        }

        static public int sqlite3_backup_pagecount(sqlite3_backup backup)
        {
            return Provider.sqlite3_backup_pagecount(backup);
        }

        // called by something that wants the return code
        static public int sqlite3_backup_finish(sqlite3_backup backup)
        {
            return backup.manual_close();
        }

        // this is called by the SafeHandle
        static internal int internal_sqlite3_backup_finish(IntPtr p)
        {
            return Provider.sqlite3_backup_finish(p);
        }

        static public int sqlite3_blob_open(sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z col_utf8, long rowid, int flags, out sqlite3_blob blob)
        {
            return Provider.sqlite3_blob_open(db, db_utf8, table_utf8, col_utf8, rowid, flags, out blob);
        }

        static public int sqlite3_blob_open(sqlite3 db, string sdb, string table, string col, long rowid, int flags, out sqlite3_blob blob)
        {
            return sqlite3_blob_open(db, sdb.to_utf8z(), table.to_utf8z(), col.to_utf8z(), rowid, flags, out blob);
        }

        static public int sqlite3_blob_bytes(sqlite3_blob blob)
        {
            return Provider.sqlite3_blob_bytes(blob);
        }

        static public int sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
        {
            return Provider.sqlite3_blob_reopen(blob, rowid);
        }

        static public int sqlite3_blob_write(sqlite3_blob blob, ReadOnlySpan<byte> b, int offset)
        {
            return Provider.sqlite3_blob_write(blob, b, offset);
        }

        static public int sqlite3_blob_read(sqlite3_blob blob, Span<byte> b, int offset)
        {
            return Provider.sqlite3_blob_read(blob, b, offset);
        }

        // called by something that wants the return code
        static public int sqlite3_blob_close(sqlite3_blob blob)
        {
            return blob.manual_close();
        }

        // this is called by the SafeHandle
        static internal int internal_sqlite3_blob_close(IntPtr blob)
        {
            return Provider.sqlite3_blob_close(blob);
        }

        static public int sqlite3_wal_autocheckpoint(sqlite3 db, int n)
        {
            return Provider.sqlite3_wal_autocheckpoint(db, n);
        }

        static public int sqlite3_wal_checkpoint(sqlite3 db, string dbName)
        {
            return Provider.sqlite3_wal_checkpoint(db, dbName.to_utf8z());
        }

        static public int sqlite3_wal_checkpoint_v2(sqlite3 db, string dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            return Provider.sqlite3_wal_checkpoint_v2(db, dbName.to_utf8z(), eMode, out logSize, out framesCheckPointed);
        }

        static public int sqlite3_set_authorizer(sqlite3 db, delegate_authorizer f, object user_data)
        {
            return Provider.sqlite3_set_authorizer(db, f, user_data);
        }

        static public int sqlite3_set_authorizer(sqlite3 db, strdelegate_authorizer f, object user_data)
        {
            delegate_authorizer cb;
            if (f == null)
            {
                cb = null;
            }
            else
            {
                cb =
                (ob, a, p0, p1, dbname, v) =>
                {
                    return f(ob, a, p0.utf8_to_string(), p1.utf8_to_string(), dbname.utf8_to_string(), v.utf8_to_string());
                };
            }
            return sqlite3_set_authorizer(db, cb, user_data);
        }

        static public int sqlite3_win32_set_directory(int typ, string path)
        {
            return Provider.sqlite3_win32_set_directory(typ, path.to_utf8z());
        }
    }
}

