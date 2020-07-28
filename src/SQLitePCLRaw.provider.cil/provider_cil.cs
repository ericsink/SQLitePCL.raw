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
	using System.Reflection;

	[Preserve(AllMembers = true)]
    public sealed class SQLite3Provider_cil : ISQLite3Provider
    {
		const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

        string ISQLite3Provider.GetNativeLibraryName()
        {
            return "(cil)";
        }

        bool my_streq(IntPtr p, IntPtr q, int len)
        {
            return 0 == NativeMethods.sqlite3_strnicmp(p, q, len);
        }

        hook_handles get_hooks(sqlite3 db)
        {
			return db.GetOrCreateExtra<hook_handles>(() => new hook_handles(my_streq));
        }

        unsafe int ISQLite3Provider.sqlite3_win32_set_directory(int typ, utf8z path)
        {
            return raw.SQLITE_ERROR;
        }

        unsafe int ISQLite3Provider.sqlite3_open(utf8z filename, out IntPtr db)
        {
            fixed (byte* p = filename)
            {
                IntPtr x;
                var rc = foo.sqlite3_open((IntPtr) p, (IntPtr) (&x));
                db = x;
                return rc;
            }
        }

        unsafe int ISQLite3Provider.sqlite3_open_v2(utf8z filename, out IntPtr db, int flags, utf8z vfs)
        {
            fixed (byte* p_filename = filename, p_vfs = vfs)
            {
                IntPtr x;
                var rc = foo.sqlite3_open_v2((IntPtr) p_filename, (IntPtr) (&x), flags, (IntPtr) p_vfs);
                db = x;
                return rc;
            }
        }

		#pragma warning disable 649
		private struct sqlite3_vfs
		{
			public int iVersion;
			public int szOsFile;
			public int mxPathname;
			public IntPtr pNext;
			public IntPtr zName;
			public IntPtr pAppData;
			public IntPtr xOpen;
			public SQLiteDeleteDelegate xDelete;
			public IntPtr xAccess;
			public IntPtr xFullPathname;
			public IntPtr xDlOpen;
			public IntPtr xDlError;
			public IntPtr xDlSym;
			public IntPtr xDlClose;
			public IntPtr xRandomness;
			public IntPtr xSleep;
			public IntPtr xCurrentTime;
			public IntPtr xGetLastError;

			[UnmanagedFunctionPointer(CALLING_CONVENTION)]
			public unsafe delegate int SQLiteDeleteDelegate(IntPtr pVfs, byte* zName, int syncDir);
		}
		#pragma warning restore 649
		
		unsafe int ISQLite3Provider.sqlite3__vfs__delete(utf8z vfs, utf8z filename, int syncDir)
		{
            fixed (byte* p_vfs = vfs, p_filename = filename)
            {
                IntPtr ptrVfs = foo.sqlite3_vfs_find((IntPtr) p_vfs);
                // this code and the struct it uses was taken from aspnet/DataCommon.SQLite, Apache License 2.0
                sqlite3_vfs vstruct = (sqlite3_vfs) Marshal.PtrToStructure(ptrVfs, typeof(sqlite3_vfs));
                return vstruct.xDelete(ptrVfs, p_filename, 1);
            }
		}

        int ISQLite3Provider.sqlite3_close_v2(IntPtr db)
        {
            var rc = foo.sqlite3_close_v2(db);
			return rc;
        }

        int ISQLite3Provider.sqlite3_close(IntPtr db)
        {
            var rc = foo.sqlite3_close(db);
			return rc;
        }

        void ISQLite3Provider.sqlite3_free(IntPtr p)
        {
            foo.sqlite3_free(p);
        }

        int ISQLite3Provider.sqlite3_stricmp(IntPtr p, IntPtr q)
        {
            return foo.sqlite3_stricmp(p, q);
        }

        int ISQLite3Provider.sqlite3_strnicmp(IntPtr p, IntPtr q, int n)
        {
            return foo.sqlite3_strnicmp(p, q, n);
        }

        int ISQLite3Provider.sqlite3_enable_shared_cache(int enable)
        {
            return foo.sqlite3_enable_shared_cache(enable);
        }

        void ISQLite3Provider.sqlite3_interrupt(sqlite3 db)
        {
            foo.sqlite3_interrupt(db.ToIntPtr());
        }

        [MonoPInvokeCallback (typeof(NativeMethods.callback_exec))]
        static int exec_hook_bridge(IntPtr p, int n, IntPtr values_ptr, IntPtr names_ptr)
        {
            exec_hook_info hi = exec_hook_info.from_ptr(p);
            return hi.call(n, values_ptr, names_ptr);
        }
		// TODO shouldn't there be a impl/bridge thing here?

        unsafe int ISQLite3Provider.sqlite3_exec(sqlite3 db, utf8z sql, delegate_exec func, object user_data, out IntPtr errMsg)
        {
            int rc;

			delegate* <IntPtr, int, IntPtr, IntPtr, int> cb;
			exec_hook_info hi;
            if (func != null)
            {
				cb = &exec_hook_bridge;
                hi = new exec_hook_info(func, user_data);
            }
            else
            {
				cb = null;
                hi = null;
            }
			var h = new hook_handle(hi);
            unsafe
            {
                fixed (byte* p_sql = sql)
                {
                    IntPtr x;
                    rc = foo.sqlite3_exec(db.ToIntPtr(), (IntPtr) p_sql, (IntPtr) cb, h.ToIntPtr(), (IntPtr) (&x));
                    errMsg = x;
                }
            }
			h.Dispose();

            return rc;
        }

        unsafe int ISQLite3Provider.sqlite3_complete(utf8z sql)
        {
            fixed (byte* p = sql)
            {
                return NativeMethods.sqlite3_complete((IntPtr) p);
            }
        }

        unsafe utf8z ISQLite3Provider.sqlite3_compileoption_get(int n)
        {
            return utf8z.FromPtr((byte*) foo.sqlite3_compileoption_get(n));
        }

        unsafe int ISQLite3Provider.sqlite3_compileoption_used(utf8z s)
        {
            fixed (byte* p = s)
            {
                return foo.sqlite3_compileoption_used((IntPtr) p);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_table_column_metadata(sqlite3 db, utf8z dbName, utf8z tblName, utf8z colName, out utf8z dataType, out utf8z collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            fixed (byte* p_dbName = dbName, p_tblName = tblName, p_colName = colName)
            {
                IntPtr p_dataType;
                IntPtr p_collSeq;
                int x_notNull;
                int x_primaryKey;
                int x_autoInc;
                var rc = NativeMethods.sqlite3_table_column_metadata(
                            db.ToIntPtr(), (IntPtr) p_dbName, (IntPtr) p_tblName, (IntPtr) p_colName, 
                            (IntPtr) (&p_dataType), (IntPtr) (&p_collSeq), (IntPtr) (&x_notNull), (IntPtr) (&x_primaryKey), (IntPtr) (&x_autoInc));
                dataType = utf8z.FromPtr((byte*) p_dataType);
                collSeq = utf8z.FromPtr((byte*) p_collSeq);
                notNull = x_notNull;
                primaryKey = x_primaryKey;
                autoInc = x_autoInc;
                return rc;
            }
        }

        unsafe int ISQLite3Provider.sqlite3_key(sqlite3 db, ReadOnlySpan<byte> k)
        {
            fixed (byte* p = k)
            {
                return NativeMethods.sqlite3_key(db.ToIntPtr(), p, k.Length);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_key_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
        {
            fixed (byte* p = k, p_name = name)
            {
                return NativeMethods.sqlite3_key_v2(db.ToIntPtr(), p_name, p, k.Length);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_rekey(sqlite3 db, ReadOnlySpan<byte> k)
        {
            fixed (byte* p = k)
            {
                return NativeMethods.sqlite3_rekey(db.ToIntPtr(), p, k.Length);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_rekey_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
        {
            fixed (byte* p = k, p_name = name)
            {
                return NativeMethods.sqlite3_rekey_v2(db.ToIntPtr(), p_name, p, k.Length);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out IntPtr stm, out ReadOnlySpan<byte> tail)
        {
            fixed (byte* p_sql = sql)
            {
                IntPtr x_stm;
                byte* p_tail;
                var rc = foo.sqlite3_prepare_v2(db.ToIntPtr(), (IntPtr) p_sql, sql.Length, (IntPtr) (&x_stm), (IntPtr) (&p_tail));
                stm = x_stm;
                var len_consumed = (int) (p_tail - p_sql);
                int len_remain = sql.Length - len_consumed;
                if (len_remain > 0)
                {
                    tail = sql.Slice(len_consumed, len_remain);
                }
                else
                {
                    tail = ReadOnlySpan<byte>.Empty;
                }
                return rc;
            }
        }

        unsafe int ISQLite3Provider.sqlite3_prepare_v2(sqlite3 db, utf8z sql, out IntPtr stm, out utf8z tail)
        {
            fixed (byte* p_sql = sql)
            {
                IntPtr x_stm;
                byte* p_tail;
                var rc = foo.sqlite3_prepare_v2(db.ToIntPtr(), (IntPtr) p_sql, -1, (IntPtr) (&x_stm), (IntPtr) (&p_tail));
                stm = x_stm;
                // TODO we could skip the strlen by using the length we were given
                tail = utf8z.FromPtr(p_tail);
                return rc;
            }
        }

        unsafe int ISQLite3Provider.sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out IntPtr stm, out ReadOnlySpan<byte> tail)
        {
            fixed (byte* p_sql = sql)
            {
                IntPtr x_stm;
                byte* p_tail;
                var rc = NativeMethods.sqlite3_prepare_v3(db.ToIntPtr(), (IntPtr) p_sql, sql.Length, (int) flags, (IntPtr) (&x_stm), (IntPtr) (&p_tail));
                stm = x_stm;
                var len_consumed = (int) (p_tail - p_sql);
                int len_remain = sql.Length - len_consumed;
                if (len_remain > 0)
                {
                    tail = sql.Slice(len_consumed, len_remain);
                }
                else
                {
                    tail = ReadOnlySpan<byte>.Empty;
                }
                return rc;
            }
        }

        unsafe int ISQLite3Provider.sqlite3_prepare_v3(sqlite3 db, utf8z sql, uint flags, out IntPtr stm, out utf8z tail)
        {
            fixed (byte* p_sql = sql)
            {
                IntPtr x_stm;
                byte* p_tail;
                var rc = NativeMethods.sqlite3_prepare_v3(db.ToIntPtr(), (IntPtr) p_sql, -1, (int) flags, (IntPtr) (&x_stm), (IntPtr) (&p_tail));
                stm = x_stm;
                // TODO we could skip the strlen by using the length we were given
                tail = utf8z.FromPtr(p_tail);
                return rc;
            }
        }

        unsafe int ISQLite3Provider.sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg)
        {
            int x_current;
            int x_highest;
            var rc = NativeMethods.sqlite3_db_status(db.ToIntPtr(), op, (IntPtr) (&x_current), (IntPtr) (&x_highest), resetFlg);
            current = x_current;
            highest = x_highest;
            return rc;
        }

        unsafe utf8z ISQLite3Provider.sqlite3_sql(sqlite3_stmt stmt)
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_sql(stmt.ToIntPtr()));
        }

        IntPtr ISQLite3Provider.sqlite3_db_handle(IntPtr stmt)
        {
            return NativeMethods.sqlite3_db_handle(stmt);
        }

        unsafe int ISQLite3Provider.sqlite3_blob_open(sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z col_utf8, long rowid, int flags, out sqlite3_blob blob)
        {
            fixed (byte* p_db = db_utf8, p_table = table_utf8, p_col = col_utf8)
            {
                IntPtr x_blob = IntPtr.Zero;
                var rc = foo.sqlite3_blob_open(db.ToIntPtr(), (IntPtr) p_db, (IntPtr) p_table, (IntPtr) p_col, rowid, flags, (IntPtr) (&x_blob));
                blob = sqlite3_blob.New(x_blob);
                return rc;
            }
        }

        int ISQLite3Provider.sqlite3_blob_bytes(sqlite3_blob blob)
        {
            return foo.sqlite3_blob_bytes(blob.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
        {
            return foo.sqlite3_blob_reopen(blob.ToIntPtr(), rowid);
        }

        unsafe int ISQLite3Provider.sqlite3_blob_read(sqlite3_blob blob, Span<byte> b, int offset)
        {
            fixed (byte* p = b)
            {
                return foo.sqlite3_blob_read(blob.ToIntPtr(), (IntPtr) p, b.Length, offset);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_blob_write(sqlite3_blob blob, ReadOnlySpan<byte> b, int offset)
        {
            fixed (byte* p = b)
            {
                return foo.sqlite3_blob_write(blob.ToIntPtr(), (IntPtr) p, b.Length, offset);
            }
        }

        int ISQLite3Provider.sqlite3_blob_close(IntPtr blob)
        {
            return foo.sqlite3_blob_close(blob);
        }

        unsafe sqlite3_backup ISQLite3Provider.sqlite3_backup_init(sqlite3 destDb, utf8z destName, sqlite3 sourceDb, utf8z sourceName)
        {
            fixed (byte* p_destName = destName, p_sourceName = sourceName)
            {
                var x = NativeMethods.sqlite3_backup_init(destDb.ToIntPtr(), (IntPtr) p_destName, sourceDb.ToIntPtr(), (IntPtr) p_sourceName);
                return sqlite3_backup.New(x);
            }
        }

        int ISQLite3Provider.sqlite3_backup_step(sqlite3_backup backup, int nPage)
        {
            return foo.sqlite3_backup_step(backup.ToIntPtr(), nPage);
        }

        int ISQLite3Provider.sqlite3_backup_remaining(sqlite3_backup backup)
        {
            return foo.sqlite3_backup_remaining(backup.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_backup_pagecount(sqlite3_backup backup)
        {
            return foo.sqlite3_backup_pagecount(backup.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_backup_finish(IntPtr backup)
        {
            return foo.sqlite3_backup_finish(backup);
        }

        IntPtr ISQLite3Provider.sqlite3_next_stmt(sqlite3 db, IntPtr stmt)
        {
            return foo.sqlite3_next_stmt(db.ToIntPtr(), stmt);
        }

        long ISQLite3Provider.sqlite3_last_insert_rowid(sqlite3 db)
        {
            return foo.sqlite3_last_insert_rowid(db.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_changes(sqlite3 db)
        {
            return foo.sqlite3_changes(db.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_total_changes(sqlite3 db)
        {
            return foo.sqlite3_total_changes(db.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_extended_result_codes(sqlite3 db, int onoff)
        {
            return foo.sqlite3_extended_result_codes(db.ToIntPtr(), onoff);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_errstr(int rc)
        {
            return utf8z.FromPtr((byte*) foo.sqlite3_errstr(rc));
        }

        int ISQLite3Provider.sqlite3_errcode(sqlite3 db)
        {
            return foo.sqlite3_errcode(db.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_extended_errcode(sqlite3 db)
        {
            return foo.sqlite3_extended_errcode(db.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_busy_timeout(sqlite3 db, int ms)
        {
            return foo.sqlite3_busy_timeout(db.ToIntPtr(), ms);
        }

        int ISQLite3Provider.sqlite3_get_autocommit(sqlite3 db)
        {
            return foo.sqlite3_get_autocommit(db.ToIntPtr());
        }

        unsafe int ISQLite3Provider.sqlite3_db_readonly(sqlite3 db, utf8z dbName)
        {
            fixed (byte* p_dbName = dbName)
            {
                return foo.sqlite3_db_readonly(db.ToIntPtr(), (IntPtr) p_dbName); 
            }
        }
        
        unsafe utf8z ISQLite3Provider.sqlite3_db_filename(sqlite3 db, utf8z att)
		{
            fixed (byte* p_att = att)
            {
                return utf8z.FromPtr((byte*) foo.sqlite3_db_filename(db.ToIntPtr(), (IntPtr) p_att));
            }
		}

        unsafe utf8z ISQLite3Provider.sqlite3_errmsg(sqlite3 db)
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_errmsg(db.ToIntPtr()));
        }

        unsafe utf8z ISQLite3Provider.sqlite3_libversion()
        {
            return utf8z.FromPtr((byte*) foo.sqlite3_libversion());
        }

        int ISQLite3Provider.sqlite3_libversion_number()
        {
            return foo.sqlite3_libversion_number();
        }

        int ISQLite3Provider.sqlite3_threadsafe()
        {
            return foo.sqlite3_threadsafe();
        }

        int ISQLite3Provider.sqlite3_config(int op)
        {
            // TODO need to send an args with count 0 ?
            return foo.sqlite3_config(op, IntPtr.Zero);
        }

        struct config1arg
        {
            public long count;
            public long v;
        }

        unsafe int ISQLite3Provider.sqlite3_config(int op, int val)
        {
            var args = new config1arg();
            args.count = 1;
            args.v = val;
            return foo.sqlite3_config(op, (IntPtr) (&args));
        }

        int ISQLite3Provider.sqlite3_initialize()
        {
            return foo.sqlite3_initialize();
        }

        int ISQLite3Provider.sqlite3_shutdown()
        {
            return foo.sqlite3_shutdown();
        }

        int ISQLite3Provider.sqlite3_enable_load_extension(sqlite3 db, int onoff)
        {
            return foo.sqlite3_enable_load_extension(db.ToIntPtr(), onoff);
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  The implementation details
        // can vary depending on the .NET implementation, so we hide these
        // in platform-specific code underneath the ISQLite3Provider boundary.
        //
        // The caller gives us a delegate and an object they want passed to that
        // delegate.  We do not actually pass that stuff down to SQLite as
        // the callback.  Instead, we store the information and pass down a bridge
        // function, with an IntPtr that can be used to retrieve the info later.
        //
        // When SQLite calls the bridge function, we lookup the info we previously
        // stored and call the delegate provided by the upper layer.
        //
        // The class we use to remember the original info (delegate and user object)
        // is shared but not portable.  It is in the util.cs file which is compiled
        // into each platform assembly.
        
        [MonoPInvokeCallback (typeof(NativeMethods.callback_commit))]
        static int commit_hook_bridge_impl(IntPtr p)
        {
            commit_hook_info hi = commit_hook_info.from_ptr(p);
            return hi.call();
        }

		readonly NativeMethods.callback_commit commit_hook_bridge = new NativeMethods.callback_commit(commit_hook_bridge_impl); 
        unsafe void ISQLite3Provider.sqlite3_commit_hook(sqlite3 db, delegate_commit func, object v)
        {
			var info = get_hooks(db);
            if (info.commit != null)
            {
                // TODO maybe turn off the hook here, for now
                info.commit.Dispose();
                info.commit = null;
            }

			delegate* <IntPtr, int> cb;
			commit_hook_info hi;
            if (func != null)
            {
				cb = &commit_hook_bridge_impl;
                hi = new commit_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			NativeMethods.sqlite3_commit_hook(db.ToIntPtr(), (IntPtr) cb, h.ToIntPtr());
			info.commit = h.ForDispose();
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(NativeMethods.callback_scalar_function))]
        static void scalar_function_hook_bridge_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr p = NativeMethods.sqlite3_user_data(context);
            function_hook_info hi = function_hook_info.from_ptr(p);
            hi.call_scalar(context, num_args, argsptr);
        }

		readonly NativeMethods.callback_scalar_function scalar_function_hook_bridge = new NativeMethods.callback_scalar_function(scalar_function_hook_bridge_impl); 

        unsafe int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_scalar func)
        {
			var info = get_hooks(db);
            if (info.RemoveScalarFunction(name, nargs))
            {
                // TODO maybe turn off the hook here, for now
            }

            // 1 is SQLITE_UTF8
			int arg4 = 1 | flags;
            delegate* <IntPtr, int, IntPtr, void> cb;
			function_hook_info hi;
            if (func != null)
            {
				cb = &scalar_function_hook_bridge_impl;
                hi = new function_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
            fixed (byte* p_name = name)
            {
                int rc = NativeMethods.sqlite3_create_function_v2(db.ToIntPtr(), (IntPtr) p_name, nargs, arg4, h.ToIntPtr(), (IntPtr) cb, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                if ((rc == 0) && (cb != null))
                {
                    info.AddScalarFunction(name, nargs, h.ForDispose());
                }
                return rc;
            }
        }

        // ----------------------------------------------------------------

		static IDisposable disp_log_hook_handle;

        [MonoPInvokeCallback (typeof(NativeMethods.callback_log))]
        static void log_hook_bridge_impl(IntPtr p, int rc, IntPtr s)
        {
            log_hook_info hi = log_hook_info.from_ptr(p);
            hi.call(rc, utf8z.FromIntPtr(s));
        }

        struct logargs
        {
            public long count;
            public IntPtr cb;
            public IntPtr h;
        }

		readonly NativeMethods.callback_log log_hook_bridge = new NativeMethods.callback_log(log_hook_bridge_impl); 
        unsafe int ISQLite3Provider.sqlite3_config_log(delegate_log func, object v)
        {
            if (disp_log_hook_handle != null)
            {
                // TODO maybe turn off the hook here, for now
                disp_log_hook_handle.Dispose();
                disp_log_hook_handle = null;
            }

            delegate* <IntPtr, int, IntPtr, void> cb;
			log_hook_info hi;
            if (func != null)
            {
				cb = &log_hook_bridge_impl;
                hi = new log_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			disp_log_hook_handle = h; // TODO if valid
            var args = new logargs();
            args.count = 2;
            args.cb = (IntPtr) cb;
            args.h = h.ToIntPtr();
			var rc = NativeMethods.sqlite3_config(raw.SQLITE_CONFIG_LOG, (IntPtr) (&args));
			return rc;
        }

        unsafe void ISQLite3Provider.sqlite3_log(int errcode, utf8z s)
        {
            fixed (byte* p = s)
            {
                long x = 0;
                NativeMethods.sqlite3_log(errcode, (IntPtr) p, (IntPtr) (&x));
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(NativeMethods.callback_agg_function_step))]
        static void agg_function_hook_bridge_step_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            function_hook_info hi = function_hook_info.from_ptr(p);
            hi.call_step(context, agg, num_args, argsptr);
        }

        [MonoPInvokeCallback (typeof(NativeMethods.callback_agg_function_final))]
        static void agg_function_hook_bridge_final_impl(IntPtr context)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            function_hook_info hi = function_hook_info.from_ptr(p);
            hi.call_final(context, agg);
        }

		NativeMethods.callback_agg_function_step agg_function_hook_bridge_step = new NativeMethods.callback_agg_function_step(agg_function_hook_bridge_step_impl); 
		NativeMethods.callback_agg_function_final agg_function_hook_bridge_final = new NativeMethods.callback_agg_function_final(agg_function_hook_bridge_final_impl); 

        unsafe int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
			var info = get_hooks(db);
            if (info.RemoveAggFunction(name, nargs))
            {
                // TODO maybe turn off the hook here, for now
            }

            // 1 is SQLITE_UTF8
			int arg4 = 1 | flags;
            delegate* <IntPtr, int, IntPtr, void> cb_step;
            delegate* <IntPtr, void> cb_final;
			function_hook_info hi;
            if (func_step != null)
            {
                // TODO both func_step and func_final must be non-null
				cb_step = &agg_function_hook_bridge_step_impl;
				cb_final = &agg_function_hook_bridge_final_impl;
                hi = new function_hook_info(func_step, func_final, v);
            }
            else
            {
				cb_step = null;
				cb_final = null;
				hi = null;
            }
			var h = new hook_handle(hi);
            fixed (byte* p_name = name)
            {
			int rc = NativeMethods.sqlite3_create_function_v2(db.ToIntPtr(), (IntPtr) p_name, nargs, arg4, h.ToIntPtr(), IntPtr.Zero, (IntPtr) cb_step, (IntPtr) cb_final, IntPtr.Zero);
			if ((rc == 0) && (cb_step != null))
			{
                info.AddAggFunction(name, nargs, h.ForDispose());
			}
			return rc;
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(NativeMethods.callback_collation))]
        static int collation_hook_bridge_impl(IntPtr p, int len1, IntPtr pv1, int len2, IntPtr pv2)
        {
            collation_hook_info hi = collation_hook_info.from_ptr(p);
            ReadOnlySpan<byte> s1;
            ReadOnlySpan<byte> s2;
            unsafe
            {
                s1 = new ReadOnlySpan<byte>(pv1.ToPointer(), len1);
                s2 = new ReadOnlySpan<byte>(pv2.ToPointer(), len2);
            }
            return hi.call(s1, s2);
        }

		readonly NativeMethods.callback_collation collation_hook_bridge = new NativeMethods.callback_collation(collation_hook_bridge_impl); 
        unsafe int ISQLite3Provider.sqlite3_create_collation(sqlite3 db, byte[] name, object v, delegate_collation func)
        {
			var info = get_hooks(db);
            if (info.RemoveCollation(name))
            {
                // TODO maybe turn off the hook here, for now
            }

            delegate* <IntPtr, int, IntPtr, int, IntPtr, int> cb;
			collation_hook_info hi;
            if (func != null)
            {
				cb = &collation_hook_bridge_impl;
                hi = new collation_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
            // 1 is SQLITE_UTF8
            fixed (byte* p_name = name)
            {
                int rc = NativeMethods.sqlite3_create_collation(db.ToIntPtr(), (IntPtr) p_name, 1, h.ToIntPtr(), (IntPtr) cb);
                if ((rc == 0) && (cb != null))
                {
                    info.AddCollation(name, h.ForDispose());
                }
                return rc;
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(NativeMethods.callback_update))]
        static void update_hook_bridge_impl(IntPtr p, int typ, IntPtr db, IntPtr tbl, Int64 rowid)
        {
            update_hook_info hi = update_hook_info.from_ptr(p);
            hi.call(typ, utf8z.FromIntPtr(db), utf8z.FromIntPtr(tbl), rowid);
        }

		readonly NativeMethods.callback_update update_hook_bridge = new NativeMethods.callback_update(update_hook_bridge_impl); 
        unsafe void ISQLite3Provider.sqlite3_update_hook(sqlite3 db, delegate_update func, object v)
        {
			var info = get_hooks(db);
            if (info.update != null)
            {
                // TODO maybe turn off the hook here, for now
                info.update.Dispose();
                info.update = null;
            }

            delegate* <IntPtr, int, IntPtr, IntPtr, long, void> cb;
			update_hook_info hi;
            if (func != null)
            {
				cb = &update_hook_bridge_impl;
                hi = new update_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
            info.update = h.ForDispose();
			NativeMethods.sqlite3_update_hook(db.ToIntPtr(), (IntPtr) cb, h.ToIntPtr());
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(NativeMethods.callback_rollback))]
        static void rollback_hook_bridge_impl(IntPtr p)
        {
            rollback_hook_info hi = rollback_hook_info.from_ptr(p);
            hi.call();
        }

		readonly NativeMethods.callback_rollback rollback_hook_bridge = new NativeMethods.callback_rollback(rollback_hook_bridge_impl); 
        unsafe void ISQLite3Provider.sqlite3_rollback_hook(sqlite3 db, delegate_rollback func, object v)
        {
			var info = get_hooks(db);
            if (info.rollback != null)
            {
                // TODO maybe turn off the hook here, for now
                info.rollback.Dispose();
                info.rollback = null;
            }

            delegate* <IntPtr, void> cb;
			rollback_hook_info hi;
            if (func != null)
            {
				cb = &rollback_hook_bridge_impl;
                hi = new rollback_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.rollback = h.ForDispose();
			NativeMethods.sqlite3_rollback_hook(db.ToIntPtr(), (IntPtr) cb, h.ToIntPtr());
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(NativeMethods.callback_trace))]
        static void trace_hook_bridge_impl(IntPtr p, IntPtr s)
        {
            trace_hook_info hi = trace_hook_info.from_ptr(p);
            hi.call(utf8z.FromIntPtr(s));
        }

		readonly NativeMethods.callback_trace trace_hook_bridge = new NativeMethods.callback_trace(trace_hook_bridge_impl); 
        unsafe void ISQLite3Provider.sqlite3_trace(sqlite3 db, delegate_trace func, object v)
        {
			var info = get_hooks(db);
            if (info.trace != null)
            {
                // TODO maybe turn off the hook here, for now
                info.trace.Dispose();
                info.trace = null;
            }

            delegate* <IntPtr, IntPtr, void> cb;
			trace_hook_info hi;
            if (func != null)
            {
				cb = &trace_hook_bridge_impl;
                hi = new trace_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.trace = h.ForDispose();
			NativeMethods.sqlite3_trace(db.ToIntPtr(), (IntPtr) cb, h.ToIntPtr());
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(NativeMethods.callback_profile))]
        static void profile_hook_bridge_impl(IntPtr p, IntPtr s, long elapsed)
        {
            profile_hook_info hi = profile_hook_info.from_ptr(p);
            hi.call(utf8z.FromIntPtr(s), elapsed);
        }

		readonly NativeMethods.callback_profile profile_hook_bridge = new NativeMethods.callback_profile(profile_hook_bridge_impl); 
        unsafe void ISQLite3Provider.sqlite3_profile(sqlite3 db, delegate_profile func, object v)
        {
			var info = get_hooks(db);
            if (info.profile != null)
            {
                // TODO maybe turn off the hook here, for now
                info.profile.Dispose();
                info.profile = null;
            }

            delegate* <IntPtr, IntPtr, long, void> cb;
			profile_hook_info hi;
            if (func != null)
            {
				cb = &profile_hook_bridge_impl;
                hi = new profile_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.profile = h.ForDispose();
			NativeMethods.sqlite3_profile(db.ToIntPtr(), (IntPtr) cb, h.ToIntPtr());
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(NativeMethods.callback_progress_handler))]
        static int progress_hook_bridge_impl(IntPtr p)
        {
            progress_hook_info hi = progress_hook_info.from_ptr(p);
            return hi.call();
        }

        readonly NativeMethods.callback_progress_handler progress_hook_bridge = new NativeMethods.callback_progress_handler(progress_hook_bridge_impl);
        unsafe void ISQLite3Provider.sqlite3_progress_handler(sqlite3 db, int instructions, delegate_progress func, object v)
        {
			var info = get_hooks(db);
            if (info.progress != null)
            {
                // TODO maybe turn off the hook here, for now
                info.progress.Dispose();
                info.progress = null;
            }

            delegate* <IntPtr, int> cb;
			progress_hook_info hi;
            if (func != null)
            {
				cb = &progress_hook_bridge_impl;
                hi = new progress_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.progress = h.ForDispose();
			NativeMethods.sqlite3_progress_handler(db.ToIntPtr(), instructions, (IntPtr) cb, h.ToIntPtr());
        }

        // ----------------------------------------------------------------

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(NativeMethods.callback_authorizer))]
        static int authorizer_hook_bridge_impl(IntPtr p, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view)
        {
            authorizer_hook_info hi = authorizer_hook_info.from_ptr(p);
            return hi.call(action_code, utf8z.FromIntPtr(param0), utf8z.FromIntPtr(param1), utf8z.FromIntPtr(dbName), utf8z.FromIntPtr(inner_most_trigger_or_view));
        }

        readonly NativeMethods.callback_authorizer authorizer_hook_bridge = new NativeMethods.callback_authorizer(authorizer_hook_bridge_impl);
        unsafe int ISQLite3Provider.sqlite3_set_authorizer(sqlite3 db, delegate_authorizer func, object v)
        {
			var info = get_hooks(db);
            if (info.authorizer != null)
            {
                // TODO maybe turn off the hook here, for now
                info.authorizer.Dispose();
                info.authorizer = null;
            }

            delegate* <IntPtr, int, IntPtr, IntPtr, IntPtr, IntPtr, int> cb;
			authorizer_hook_info hi;
            if (func != null)
            {
				cb = &authorizer_hook_bridge_impl;
                hi = new authorizer_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.authorizer = h.ForDispose();
			return NativeMethods.sqlite3_set_authorizer(db.ToIntPtr(), (IntPtr) cb, h.ToIntPtr());
        }

        // ----------------------------------------------------------------

        long ISQLite3Provider.sqlite3_memory_used()
        {
            return NativeMethods.sqlite3_memory_used();
        }

        long ISQLite3Provider.sqlite3_memory_highwater(int resetFlag)
        {
            return NativeMethods.sqlite3_memory_highwater(resetFlag);
        }

        unsafe int ISQLite3Provider.sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            int x_current;
            int x_highwater;
            var rc = NativeMethods.sqlite3_status(op, (IntPtr) (&x_current), (IntPtr) (&x_highwater), resetFlag);
            current = x_current;
            highwater = x_highwater;
            return rc;
        }

        unsafe utf8z ISQLite3Provider.sqlite3_sourceid()
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_sourceid());
        }

        void ISQLite3Provider.sqlite3_result_int64(IntPtr ctx, long val)
        {
            NativeMethods.sqlite3_result_int64(ctx, val);
        }

        void ISQLite3Provider.sqlite3_result_int(IntPtr ctx, int val)
        {
            NativeMethods.sqlite3_result_int(ctx, val);
        }

        void ISQLite3Provider.sqlite3_result_double(IntPtr ctx, double val)
        {
            NativeMethods.sqlite3_result_double(ctx, val);
        }

        void ISQLite3Provider.sqlite3_result_null(IntPtr stm)
        {
            NativeMethods.sqlite3_result_null(stm);
        }

        unsafe void ISQLite3Provider.sqlite3_result_error(IntPtr ctx, ReadOnlySpan<byte> val)
        {
            fixed (byte* p = val)
            {
                NativeMethods.sqlite3_result_error(ctx, (IntPtr) p, val.Length);
            }
        }

        unsafe void ISQLite3Provider.sqlite3_result_error(IntPtr ctx, utf8z val)
        {
            fixed (byte* p = val)
            {
                NativeMethods.sqlite3_result_error(ctx, (IntPtr) p, -1);
            }
        }

        unsafe void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, ReadOnlySpan<byte> val)
        {
            fixed (byte* p = val)
            {
                NativeMethods.sqlite3_result_text(ctx, (IntPtr) p, val.Length, new IntPtr(-1));
            }
        }

        unsafe void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, utf8z val)
        {
            fixed (byte* p = val)
            {
                NativeMethods.sqlite3_result_text(ctx, (IntPtr) p, -1, new IntPtr(-1));
            }
        }

        unsafe void ISQLite3Provider.sqlite3_result_blob(IntPtr ctx, ReadOnlySpan<byte> blob)
        {
            fixed (byte* p = blob)
            {
                NativeMethods.sqlite3_result_blob(ctx, (IntPtr) p, blob.Length, new IntPtr(-1));
            }
        }

        void ISQLite3Provider.sqlite3_result_zeroblob(IntPtr ctx, int n)
        {
            NativeMethods.sqlite3_result_zeroblob(ctx, n);
        }

        // TODO sqlite3_result_value

        void ISQLite3Provider.sqlite3_result_error_toobig(IntPtr ctx)
        {
            NativeMethods.sqlite3_result_error_toobig(ctx);
        }

        void ISQLite3Provider.sqlite3_result_error_nomem(IntPtr ctx)
        {
            NativeMethods.sqlite3_result_error_nomem(ctx);
        }

        void ISQLite3Provider.sqlite3_result_error_code(IntPtr ctx, int code)
        {
            NativeMethods.sqlite3_result_error_code(ctx, code);
        }

        ReadOnlySpan<byte> ISQLite3Provider.sqlite3_value_blob(IntPtr p)
        {
            IntPtr blobPointer = NativeMethods.sqlite3_value_blob(p);
            if (blobPointer == IntPtr.Zero)
            {
                return null;
            }

            var length = NativeMethods.sqlite3_value_bytes(p);
            unsafe
            {
                return new ReadOnlySpan<byte>(blobPointer.ToPointer(), length);
            }
        }

        int ISQLite3Provider.sqlite3_value_bytes(IntPtr p)
        {
            return NativeMethods.sqlite3_value_bytes(p);
        }

        double ISQLite3Provider.sqlite3_value_double(IntPtr p)
        {
            return NativeMethods.sqlite3_value_double(p);
        }

        int ISQLite3Provider.sqlite3_value_int(IntPtr p)
        {
            return NativeMethods.sqlite3_value_int(p);
        }

        long ISQLite3Provider.sqlite3_value_int64(IntPtr p)
        {
            return NativeMethods.sqlite3_value_int64(p);
        }

        int ISQLite3Provider.sqlite3_value_type(IntPtr p)
        {
            return NativeMethods.sqlite3_value_type(p);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_value_text(IntPtr p)
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_value_text(p));
        }

        int ISQLite3Provider.sqlite3_bind_int(sqlite3_stmt stm, int paramIndex, int val)
        {
            return NativeMethods.sqlite3_bind_int(stm.ToIntPtr(), paramIndex, val);
        }

        int ISQLite3Provider.sqlite3_bind_int64(sqlite3_stmt stm, int paramIndex, long val)
        {
            return NativeMethods.sqlite3_bind_int64(stm.ToIntPtr(), paramIndex, val);
        }

        unsafe int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stm, int paramIndex, ReadOnlySpan<byte> t)
        {
            fixed (byte* p_t = t)
            {
                return NativeMethods.sqlite3_bind_text(stm.ToIntPtr(), paramIndex, (IntPtr) p_t, t.Length, new IntPtr(-1));
            }
        }

        unsafe int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stm, int paramIndex, utf8z t)
        {
            fixed (byte* p_t = t)
            {
                return NativeMethods.sqlite3_bind_text(stm.ToIntPtr(), paramIndex, (IntPtr) p_t, -1, new IntPtr(-1));
            }
        }

        int ISQLite3Provider.sqlite3_bind_double(sqlite3_stmt stm, int paramIndex, double val)
        {
            return NativeMethods.sqlite3_bind_double(stm.ToIntPtr(), paramIndex, val);
        }

        unsafe int ISQLite3Provider.sqlite3_bind_blob(sqlite3_stmt stm, int paramIndex, ReadOnlySpan<byte> blob)
        {
            if (blob.Length == 0)
            {
                // passing a zero-length blob to sqlite3_bind_blob() requires
                // a non-null pointer, even though conceptually, that pointer
                // point to zero things, ie nothing.

                var ba_fake = new byte[] { 42 };
                ReadOnlySpan<byte> span_fake = ba_fake;
                fixed (byte* p_fake = span_fake)
                {
                    return NativeMethods.sqlite3_bind_blob(stm.ToIntPtr(), paramIndex, (IntPtr) p_fake, 0, new IntPtr(-1));
                }
            }
            else
            {
                fixed (byte* p = blob)
                {
                    return NativeMethods.sqlite3_bind_blob(stm.ToIntPtr(), paramIndex, (IntPtr) p, blob.Length, new IntPtr(-1));
                }
            }
        }

        int ISQLite3Provider.sqlite3_bind_zeroblob(sqlite3_stmt stm, int paramIndex, int size)
        {
            return NativeMethods.sqlite3_bind_zeroblob(stm.ToIntPtr(), paramIndex, size);
        }

        int ISQLite3Provider.sqlite3_bind_null(sqlite3_stmt stm, int paramIndex)
        {
            return NativeMethods.sqlite3_bind_null(stm.ToIntPtr(), paramIndex);
        }

        int ISQLite3Provider.sqlite3_bind_parameter_count(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_bind_parameter_count(stm.ToIntPtr());
        }

        unsafe utf8z ISQLite3Provider.sqlite3_bind_parameter_name(sqlite3_stmt stm, int paramIndex)
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_bind_parameter_name(stm.ToIntPtr(), paramIndex));
        }

        unsafe int ISQLite3Provider.sqlite3_bind_parameter_index(sqlite3_stmt stm, utf8z paramName)
        {
            fixed (byte* p_paramName = paramName)
            {
                return NativeMethods.sqlite3_bind_parameter_index(stm.ToIntPtr(), (IntPtr) p_paramName);
            }
        }

        int ISQLite3Provider.sqlite3_step(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_step(stm.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_stmt_isexplain(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_stmt_isexplain(stm.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_stmt_busy(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_stmt_busy(stm.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_stmt_readonly(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_stmt_readonly(stm.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_column_int(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_int(stm.ToIntPtr(), columnIndex);
        }

        long ISQLite3Provider.sqlite3_column_int64(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_int64(stm.ToIntPtr(), columnIndex);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_text(sqlite3_stmt stm, int columnIndex)
        {
            var p = NativeMethods.sqlite3_column_text(stm.ToIntPtr(), columnIndex);
            var length = NativeMethods.sqlite3_column_bytes(stm.ToIntPtr(), columnIndex);
            return utf8z.FromPtrLen((byte*) p, length);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_decltype(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_column_decltype(stm.ToIntPtr(), columnIndex));
        }

        double ISQLite3Provider.sqlite3_column_double(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_double(stm.ToIntPtr(), columnIndex);
        }

        ReadOnlySpan<byte> ISQLite3Provider.sqlite3_column_blob(sqlite3_stmt stm, int columnIndex)
        {
            IntPtr blobPointer = NativeMethods.sqlite3_column_blob(stm.ToIntPtr(), columnIndex);
            if (blobPointer == IntPtr.Zero)
            {
                return null;
            }

            var length = NativeMethods.sqlite3_column_bytes(stm.ToIntPtr(), columnIndex);
            unsafe
            {
                return new ReadOnlySpan<byte>(blobPointer.ToPointer(), length);
            }
        }

        int ISQLite3Provider.sqlite3_column_type(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_type(stm.ToIntPtr(), columnIndex);
        }

        int ISQLite3Provider.sqlite3_column_bytes(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_bytes(stm.ToIntPtr(), columnIndex);
        }

        int ISQLite3Provider.sqlite3_column_count(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_column_count(stm.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_data_count(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_data_count(stm.ToIntPtr());
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_name(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_column_name(stm.ToIntPtr(), columnIndex));
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_origin_name(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_column_origin_name(stm.ToIntPtr(), columnIndex));
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_table_name(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_column_table_name(stm.ToIntPtr(), columnIndex));
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_database_name(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr((byte*) NativeMethods.sqlite3_column_database_name(stm.ToIntPtr(), columnIndex));
        }

        int ISQLite3Provider.sqlite3_reset(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_reset(stm.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_clear_bindings(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_clear_bindings(stm.ToIntPtr());
        }

        int ISQLite3Provider.sqlite3_stmt_status(sqlite3_stmt stm, int op, int resetFlg)
        {
            return NativeMethods.sqlite3_stmt_status(stm.ToIntPtr(), op, resetFlg);
        }

        int ISQLite3Provider.sqlite3_finalize(IntPtr stm)
        {
            return NativeMethods.sqlite3_finalize(stm);
        }

        int ISQLite3Provider.sqlite3_wal_autocheckpoint(sqlite3 db, int n)
        {
            return NativeMethods.sqlite3_wal_autocheckpoint(db.ToIntPtr(), n);
        }

        unsafe int ISQLite3Provider.sqlite3_wal_checkpoint(sqlite3 db, utf8z dbName)
        {
            fixed (byte* p_dbName = dbName)
            {
                return NativeMethods.sqlite3_wal_checkpoint(db.ToIntPtr(), (IntPtr) p_dbName);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_wal_checkpoint_v2(sqlite3 db, utf8z dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            fixed (byte* p_dbName = dbName)
            {
                int x_logSize;
                int x_framesCheckPointed;
                var rc = NativeMethods.sqlite3_wal_checkpoint_v2(db.ToIntPtr(), (IntPtr) p_dbName, eMode, (IntPtr) (&x_logSize), (IntPtr) (&x_framesCheckPointed));
                logSize = x_logSize;
                framesCheckPointed = x_framesCheckPointed;
                return rc;
            }
        }

	static class NativeMethods
	{
		public static MyDelegateTypes.sqlite3_close sqlite3_close = (MyDelegateTypes.sqlite3_close) foo.sqlite3_close;
		public static MyDelegateTypes.sqlite3_close_v2 sqlite3_close_v2 = (MyDelegateTypes.sqlite3_close_v2) foo.sqlite3_close_v2;
		public static MyDelegateTypes.sqlite3_enable_shared_cache sqlite3_enable_shared_cache = (MyDelegateTypes.sqlite3_enable_shared_cache) foo.sqlite3_enable_shared_cache;
		public static MyDelegateTypes.sqlite3_interrupt sqlite3_interrupt = (MyDelegateTypes.sqlite3_interrupt) foo.sqlite3_interrupt;
		public static MyDelegateTypes.sqlite3_finalize sqlite3_finalize = (MyDelegateTypes.sqlite3_finalize) foo.sqlite3_finalize;
		public static MyDelegateTypes.sqlite3_reset sqlite3_reset = (MyDelegateTypes.sqlite3_reset) foo.sqlite3_reset;
		public static MyDelegateTypes.sqlite3_clear_bindings sqlite3_clear_bindings = (MyDelegateTypes.sqlite3_clear_bindings) foo.sqlite3_clear_bindings;
		public static MyDelegateTypes.sqlite3_stmt_status sqlite3_stmt_status = (MyDelegateTypes.sqlite3_stmt_status) foo.sqlite3_stmt_status;
		public static MyDelegateTypes.sqlite3_bind_parameter_name sqlite3_bind_parameter_name = (MyDelegateTypes.sqlite3_bind_parameter_name) foo.sqlite3_bind_parameter_name;
		public static MyDelegateTypes.sqlite3_column_database_name sqlite3_column_database_name = (MyDelegateTypes.sqlite3_column_database_name) foo.sqlite3_column_database_name;
		public static MyDelegateTypes.sqlite3_column_decltype sqlite3_column_decltype = (MyDelegateTypes.sqlite3_column_decltype) foo.sqlite3_column_decltype;
		public static MyDelegateTypes.sqlite3_column_name sqlite3_column_name = (MyDelegateTypes.sqlite3_column_name) foo.sqlite3_column_name;
		public static MyDelegateTypes.sqlite3_column_origin_name sqlite3_column_origin_name = (MyDelegateTypes.sqlite3_column_origin_name) foo.sqlite3_column_origin_name;
		public static MyDelegateTypes.sqlite3_column_table_name sqlite3_column_table_name = (MyDelegateTypes.sqlite3_column_table_name) foo.sqlite3_column_table_name;
		public static MyDelegateTypes.sqlite3_column_text sqlite3_column_text = (MyDelegateTypes.sqlite3_column_text) foo.sqlite3_column_text;
		public static MyDelegateTypes.sqlite3_errmsg sqlite3_errmsg = (MyDelegateTypes.sqlite3_errmsg) foo.sqlite3_errmsg;
		public static MyDelegateTypes.sqlite3_db_readonly sqlite3_db_readonly = (MyDelegateTypes.sqlite3_db_readonly) foo.sqlite3_db_readonly;
		public static MyDelegateTypes.sqlite3_db_filename sqlite3_db_filename = (MyDelegateTypes.sqlite3_db_filename) foo.sqlite3_db_filename;
		public static MyDelegateTypes.sqlite3_prepare_v2 sqlite3_prepare_v2 = (MyDelegateTypes.sqlite3_prepare_v2) foo.sqlite3_prepare_v2;
		public static MyDelegateTypes.sqlite3_prepare_v3 sqlite3_prepare_v3 = (MyDelegateTypes.sqlite3_prepare_v3) foo.sqlite3_prepare_v3;
		public static MyDelegateTypes.sqlite3_db_status sqlite3_db_status = (MyDelegateTypes.sqlite3_db_status) foo.sqlite3_db_status;
		public static MyDelegateTypes.sqlite3_complete sqlite3_complete = (MyDelegateTypes.sqlite3_complete) foo.sqlite3_complete;
		public static MyDelegateTypes.sqlite3_compileoption_used sqlite3_compileoption_used = (MyDelegateTypes.sqlite3_compileoption_used) foo.sqlite3_compileoption_used;
		public static MyDelegateTypes.sqlite3_compileoption_get sqlite3_compileoption_get = (MyDelegateTypes.sqlite3_compileoption_get) foo.sqlite3_compileoption_get;
		public static MyDelegateTypes.sqlite3_table_column_metadata sqlite3_table_column_metadata = (MyDelegateTypes.sqlite3_table_column_metadata) foo.sqlite3_table_column_metadata;
		public static MyDelegateTypes.sqlite3_value_text sqlite3_value_text = (MyDelegateTypes.sqlite3_value_text) foo.sqlite3_value_text;
		public static MyDelegateTypes.sqlite3_enable_load_extension sqlite3_enable_load_extension = (MyDelegateTypes.sqlite3_enable_load_extension) foo.sqlite3_enable_load_extension;
		public static MyDelegateTypes.sqlite3_load_extension sqlite3_load_extension = (MyDelegateTypes.sqlite3_load_extension) foo.sqlite3_load_extension;
		public static MyDelegateTypes.sqlite3_initialize sqlite3_initialize = (MyDelegateTypes.sqlite3_initialize) foo.sqlite3_initialize;
		public static MyDelegateTypes.sqlite3_shutdown sqlite3_shutdown = (MyDelegateTypes.sqlite3_shutdown) foo.sqlite3_shutdown;
		public static MyDelegateTypes.sqlite3_libversion sqlite3_libversion = (MyDelegateTypes.sqlite3_libversion) foo.sqlite3_libversion;
		public static MyDelegateTypes.sqlite3_libversion_number sqlite3_libversion_number = (MyDelegateTypes.sqlite3_libversion_number) foo.sqlite3_libversion_number;
		public static MyDelegateTypes.sqlite3_threadsafe sqlite3_threadsafe = (MyDelegateTypes.sqlite3_threadsafe) foo.sqlite3_threadsafe;
		public static MyDelegateTypes.sqlite3_sourceid sqlite3_sourceid = (MyDelegateTypes.sqlite3_sourceid) foo.sqlite3_sourceid;
		public static MyDelegateTypes.sqlite3_malloc sqlite3_malloc = (MyDelegateTypes.sqlite3_malloc) foo.sqlite3_malloc;
		public static MyDelegateTypes.sqlite3_realloc sqlite3_realloc = (MyDelegateTypes.sqlite3_realloc) foo.sqlite3_realloc;
		public static MyDelegateTypes.sqlite3_free sqlite3_free = (MyDelegateTypes.sqlite3_free) foo.sqlite3_free;
		public static MyDelegateTypes.sqlite3_stricmp sqlite3_stricmp = (MyDelegateTypes.sqlite3_stricmp) foo.sqlite3_stricmp;
		public static MyDelegateTypes.sqlite3_strnicmp sqlite3_strnicmp = (MyDelegateTypes.sqlite3_strnicmp) foo.sqlite3_strnicmp;
		public static MyDelegateTypes.sqlite3_open sqlite3_open = (MyDelegateTypes.sqlite3_open) foo.sqlite3_open;
		public static MyDelegateTypes.sqlite3_open_v2 sqlite3_open_v2 = (MyDelegateTypes.sqlite3_open_v2) foo.sqlite3_open_v2;
		public static MyDelegateTypes.sqlite3_vfs_find sqlite3_vfs_find = (MyDelegateTypes.sqlite3_vfs_find) foo.sqlite3_vfs_find;
		public static MyDelegateTypes.sqlite3_last_insert_rowid sqlite3_last_insert_rowid = (MyDelegateTypes.sqlite3_last_insert_rowid) foo.sqlite3_last_insert_rowid;
		public static MyDelegateTypes.sqlite3_changes sqlite3_changes = (MyDelegateTypes.sqlite3_changes) foo.sqlite3_changes;
		public static MyDelegateTypes.sqlite3_total_changes sqlite3_total_changes = (MyDelegateTypes.sqlite3_total_changes) foo.sqlite3_total_changes;
		public static MyDelegateTypes.sqlite3_memory_used sqlite3_memory_used = (MyDelegateTypes.sqlite3_memory_used) foo.sqlite3_memory_used;
		public static MyDelegateTypes.sqlite3_memory_highwater sqlite3_memory_highwater = (MyDelegateTypes.sqlite3_memory_highwater) foo.sqlite3_memory_highwater;
		public static MyDelegateTypes.sqlite3_status sqlite3_status = (MyDelegateTypes.sqlite3_status) foo.sqlite3_status;
		public static MyDelegateTypes.sqlite3_busy_timeout sqlite3_busy_timeout = (MyDelegateTypes.sqlite3_busy_timeout) foo.sqlite3_busy_timeout;
		public static MyDelegateTypes.sqlite3_bind_blob sqlite3_bind_blob = (MyDelegateTypes.sqlite3_bind_blob) foo.sqlite3_bind_blob;
		public static MyDelegateTypes.sqlite3_bind_zeroblob sqlite3_bind_zeroblob = (MyDelegateTypes.sqlite3_bind_zeroblob) foo.sqlite3_bind_zeroblob;
		public static MyDelegateTypes.sqlite3_bind_double sqlite3_bind_double = (MyDelegateTypes.sqlite3_bind_double) foo.sqlite3_bind_double;
		public static MyDelegateTypes.sqlite3_bind_int sqlite3_bind_int = (MyDelegateTypes.sqlite3_bind_int) foo.sqlite3_bind_int;
		public static MyDelegateTypes.sqlite3_bind_int64 sqlite3_bind_int64 = (MyDelegateTypes.sqlite3_bind_int64) foo.sqlite3_bind_int64;
		public static MyDelegateTypes.sqlite3_bind_null sqlite3_bind_null = (MyDelegateTypes.sqlite3_bind_null) foo.sqlite3_bind_null;
		public static MyDelegateTypes.sqlite3_bind_text sqlite3_bind_text = (MyDelegateTypes.sqlite3_bind_text) foo.sqlite3_bind_text;
		public static MyDelegateTypes.sqlite3_bind_parameter_count sqlite3_bind_parameter_count = (MyDelegateTypes.sqlite3_bind_parameter_count) foo.sqlite3_bind_parameter_count;
		public static MyDelegateTypes.sqlite3_bind_parameter_index sqlite3_bind_parameter_index = (MyDelegateTypes.sqlite3_bind_parameter_index) foo.sqlite3_bind_parameter_index;
		public static MyDelegateTypes.sqlite3_column_count sqlite3_column_count = (MyDelegateTypes.sqlite3_column_count) foo.sqlite3_column_count;
		public static MyDelegateTypes.sqlite3_data_count sqlite3_data_count = (MyDelegateTypes.sqlite3_data_count) foo.sqlite3_data_count;
		public static MyDelegateTypes.sqlite3_step sqlite3_step = (MyDelegateTypes.sqlite3_step) foo.sqlite3_step;
		public static MyDelegateTypes.sqlite3_sql sqlite3_sql = (MyDelegateTypes.sqlite3_sql) foo.sqlite3_sql;
		public static MyDelegateTypes.sqlite3_column_double sqlite3_column_double = (MyDelegateTypes.sqlite3_column_double) foo.sqlite3_column_double;
		public static MyDelegateTypes.sqlite3_column_int sqlite3_column_int = (MyDelegateTypes.sqlite3_column_int) foo.sqlite3_column_int;
		public static MyDelegateTypes.sqlite3_column_int64 sqlite3_column_int64 = (MyDelegateTypes.sqlite3_column_int64) foo.sqlite3_column_int64;
		public static MyDelegateTypes.sqlite3_column_blob sqlite3_column_blob = (MyDelegateTypes.sqlite3_column_blob) foo.sqlite3_column_blob;
		public static MyDelegateTypes.sqlite3_column_bytes sqlite3_column_bytes = (MyDelegateTypes.sqlite3_column_bytes) foo.sqlite3_column_bytes;
		public static MyDelegateTypes.sqlite3_column_type sqlite3_column_type = (MyDelegateTypes.sqlite3_column_type) foo.sqlite3_column_type;
		public static MyDelegateTypes.sqlite3_aggregate_count sqlite3_aggregate_count = (MyDelegateTypes.sqlite3_aggregate_count) foo.sqlite3_aggregate_count;
		public static MyDelegateTypes.sqlite3_value_blob sqlite3_value_blob = (MyDelegateTypes.sqlite3_value_blob) foo.sqlite3_value_blob;
		public static MyDelegateTypes.sqlite3_value_bytes sqlite3_value_bytes = (MyDelegateTypes.sqlite3_value_bytes) foo.sqlite3_value_bytes;
		public static MyDelegateTypes.sqlite3_value_double sqlite3_value_double = (MyDelegateTypes.sqlite3_value_double) foo.sqlite3_value_double;
		public static MyDelegateTypes.sqlite3_value_int sqlite3_value_int = (MyDelegateTypes.sqlite3_value_int) foo.sqlite3_value_int;
		public static MyDelegateTypes.sqlite3_value_int64 sqlite3_value_int64 = (MyDelegateTypes.sqlite3_value_int64) foo.sqlite3_value_int64;
		public static MyDelegateTypes.sqlite3_value_type sqlite3_value_type = (MyDelegateTypes.sqlite3_value_type) foo.sqlite3_value_type;
		public static MyDelegateTypes.sqlite3_user_data sqlite3_user_data = (MyDelegateTypes.sqlite3_user_data) foo.sqlite3_user_data;
		public static MyDelegateTypes.sqlite3_result_blob sqlite3_result_blob = (MyDelegateTypes.sqlite3_result_blob) foo.sqlite3_result_blob;
		public static MyDelegateTypes.sqlite3_result_double sqlite3_result_double = (MyDelegateTypes.sqlite3_result_double) foo.sqlite3_result_double;
		public static MyDelegateTypes.sqlite3_result_error sqlite3_result_error = (MyDelegateTypes.sqlite3_result_error) foo.sqlite3_result_error;
		public static MyDelegateTypes.sqlite3_result_int sqlite3_result_int = (MyDelegateTypes.sqlite3_result_int) foo.sqlite3_result_int;
		public static MyDelegateTypes.sqlite3_result_int64 sqlite3_result_int64 = (MyDelegateTypes.sqlite3_result_int64) foo.sqlite3_result_int64;
		public static MyDelegateTypes.sqlite3_result_null sqlite3_result_null = (MyDelegateTypes.sqlite3_result_null) foo.sqlite3_result_null;
		public static MyDelegateTypes.sqlite3_result_text sqlite3_result_text = (MyDelegateTypes.sqlite3_result_text) foo.sqlite3_result_text;
		public static MyDelegateTypes.sqlite3_result_zeroblob sqlite3_result_zeroblob = (MyDelegateTypes.sqlite3_result_zeroblob) foo.sqlite3_result_zeroblob;
		public static MyDelegateTypes.sqlite3_result_error_toobig sqlite3_result_error_toobig = (MyDelegateTypes.sqlite3_result_error_toobig) foo.sqlite3_result_error_toobig;
		public static MyDelegateTypes.sqlite3_result_error_nomem sqlite3_result_error_nomem = (MyDelegateTypes.sqlite3_result_error_nomem) foo.sqlite3_result_error_nomem;
		public static MyDelegateTypes.sqlite3_result_error_code sqlite3_result_error_code = (MyDelegateTypes.sqlite3_result_error_code) foo.sqlite3_result_error_code;
		public static MyDelegateTypes.sqlite3_aggregate_context sqlite3_aggregate_context = (MyDelegateTypes.sqlite3_aggregate_context) foo.sqlite3_aggregate_context;
		public static MyDelegateTypes.sqlite3_key sqlite3_key = null;
		public static MyDelegateTypes.sqlite3_key_v2 sqlite3_key_v2 = null;
		public static MyDelegateTypes.sqlite3_rekey sqlite3_rekey = null;
		public static MyDelegateTypes.sqlite3_rekey_v2 sqlite3_rekey_v2 = null;
		public static MyDelegateTypes.sqlite3_config sqlite3_config = (MyDelegateTypes.sqlite3_config) foo.sqlite3_config;
		public static MyDelegateTypes.sqlite3_create_function_v2 sqlite3_create_function_v2 = (MyDelegateTypes.sqlite3_create_function_v2) foo.sqlite3_create_function_v2;
		public static MyDelegateTypes.sqlite3_create_collation sqlite3_create_collation = (MyDelegateTypes.sqlite3_create_collation) foo.sqlite3_create_collation;
		public static MyDelegateTypes.sqlite3_update_hook sqlite3_update_hook = (MyDelegateTypes.sqlite3_update_hook) foo.sqlite3_update_hook;
		public static MyDelegateTypes.sqlite3_commit_hook sqlite3_commit_hook = (MyDelegateTypes.sqlite3_commit_hook) foo.sqlite3_commit_hook;
		public static MyDelegateTypes.sqlite3_profile sqlite3_profile = (MyDelegateTypes.sqlite3_profile) foo.sqlite3_profile;
		public static MyDelegateTypes.sqlite3_progress_handler sqlite3_progress_handler = (MyDelegateTypes.sqlite3_progress_handler) foo.sqlite3_progress_handler;
		public static MyDelegateTypes.sqlite3_trace sqlite3_trace = (MyDelegateTypes.sqlite3_trace) foo.sqlite3_trace;
		public static MyDelegateTypes.sqlite3_rollback_hook sqlite3_rollback_hook = (MyDelegateTypes.sqlite3_rollback_hook) foo.sqlite3_rollback_hook;
		public static MyDelegateTypes.sqlite3_db_handle sqlite3_db_handle = (MyDelegateTypes.sqlite3_db_handle) foo.sqlite3_db_handle;
		public static MyDelegateTypes.sqlite3_next_stmt sqlite3_next_stmt = (MyDelegateTypes.sqlite3_next_stmt) foo.sqlite3_next_stmt;
		public static MyDelegateTypes.sqlite3_stmt_isexplain sqlite3_stmt_isexplain = (MyDelegateTypes.sqlite3_stmt_isexplain) foo.sqlite3_stmt_isexplain;
		public static MyDelegateTypes.sqlite3_stmt_busy sqlite3_stmt_busy = (MyDelegateTypes.sqlite3_stmt_busy) foo.sqlite3_stmt_busy;
		public static MyDelegateTypes.sqlite3_stmt_readonly sqlite3_stmt_readonly = (MyDelegateTypes.sqlite3_stmt_readonly) foo.sqlite3_stmt_readonly;
		public static MyDelegateTypes.sqlite3_exec sqlite3_exec = (MyDelegateTypes.sqlite3_exec) foo.sqlite3_exec;
		public static MyDelegateTypes.sqlite3_get_autocommit sqlite3_get_autocommit = (MyDelegateTypes.sqlite3_get_autocommit) foo.sqlite3_get_autocommit;
		public static MyDelegateTypes.sqlite3_extended_result_codes sqlite3_extended_result_codes = (MyDelegateTypes.sqlite3_extended_result_codes) foo.sqlite3_extended_result_codes;
		public static MyDelegateTypes.sqlite3_errcode sqlite3_errcode = (MyDelegateTypes.sqlite3_errcode) foo.sqlite3_errcode;
		public static MyDelegateTypes.sqlite3_extended_errcode sqlite3_extended_errcode = (MyDelegateTypes.sqlite3_extended_errcode) foo.sqlite3_extended_errcode;
		public static MyDelegateTypes.sqlite3_errstr sqlite3_errstr = (MyDelegateTypes.sqlite3_errstr) foo.sqlite3_errstr;
		public static MyDelegateTypes.sqlite3_log sqlite3_log = (MyDelegateTypes.sqlite3_log) foo.sqlite3_log;
		public static MyDelegateTypes.sqlite3_file_control sqlite3_file_control = (MyDelegateTypes.sqlite3_file_control) foo.sqlite3_file_control;
		public static MyDelegateTypes.sqlite3_backup_init sqlite3_backup_init = (MyDelegateTypes.sqlite3_backup_init) foo.sqlite3_backup_init;
		public static MyDelegateTypes.sqlite3_backup_step sqlite3_backup_step = (MyDelegateTypes.sqlite3_backup_step) foo.sqlite3_backup_step;
		public static MyDelegateTypes.sqlite3_backup_remaining sqlite3_backup_remaining = (MyDelegateTypes.sqlite3_backup_remaining) foo.sqlite3_backup_remaining;
		public static MyDelegateTypes.sqlite3_backup_pagecount sqlite3_backup_pagecount = (MyDelegateTypes.sqlite3_backup_pagecount) foo.sqlite3_backup_pagecount;
		public static MyDelegateTypes.sqlite3_backup_finish sqlite3_backup_finish = (MyDelegateTypes.sqlite3_backup_finish) foo.sqlite3_backup_finish;
		public static MyDelegateTypes.sqlite3_blob_open sqlite3_blob_open = (MyDelegateTypes.sqlite3_blob_open) foo.sqlite3_blob_open;
		public static MyDelegateTypes.sqlite3_blob_write sqlite3_blob_write = (MyDelegateTypes.sqlite3_blob_write) foo.sqlite3_blob_write;
		public static MyDelegateTypes.sqlite3_blob_read sqlite3_blob_read = (MyDelegateTypes.sqlite3_blob_read) foo.sqlite3_blob_read;
		public static MyDelegateTypes.sqlite3_blob_bytes sqlite3_blob_bytes = (MyDelegateTypes.sqlite3_blob_bytes) foo.sqlite3_blob_bytes;
		public static MyDelegateTypes.sqlite3_blob_reopen sqlite3_blob_reopen = (MyDelegateTypes.sqlite3_blob_reopen) foo.sqlite3_blob_reopen;
		public static MyDelegateTypes.sqlite3_blob_close sqlite3_blob_close = (MyDelegateTypes.sqlite3_blob_close) foo.sqlite3_blob_close;
		public static MyDelegateTypes.sqlite3_wal_autocheckpoint sqlite3_wal_autocheckpoint = (MyDelegateTypes.sqlite3_wal_autocheckpoint) foo.sqlite3_wal_autocheckpoint;
		public static MyDelegateTypes.sqlite3_wal_checkpoint sqlite3_wal_checkpoint = (MyDelegateTypes.sqlite3_wal_checkpoint) foo.sqlite3_wal_checkpoint;
		public static MyDelegateTypes.sqlite3_wal_checkpoint_v2 sqlite3_wal_checkpoint_v2 = (MyDelegateTypes.sqlite3_wal_checkpoint_v2) foo.sqlite3_wal_checkpoint_v2;
		public static MyDelegateTypes.sqlite3_set_authorizer sqlite3_set_authorizer = (MyDelegateTypes.sqlite3_set_authorizer) foo.sqlite3_set_authorizer;
		//public static MyDelegateTypes.sqlite3_win32_set_directory8 sqlite3_win32_set_directory8 = (MyDelegateTypes.sqlite3_win32_set_directory8) foo.sqlite3_win32_set_directory8;

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_log(IntPtr pUserData, int errorCode, IntPtr pMessage);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_scalar_function(IntPtr context, int nArgs, IntPtr argsptr);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_agg_function_step(IntPtr context, int nArgs, IntPtr argsptr);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_agg_function_final(IntPtr context);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_destroy(IntPtr p);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate int callback_collation(IntPtr puser, int len1, IntPtr pv1, int len2, IntPtr pv2);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_update(IntPtr p, int typ, IntPtr db, IntPtr tbl, long rowid);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate int callback_commit(IntPtr puser);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_profile(IntPtr puser, IntPtr statement, long elapsed);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate int callback_progress_handler(IntPtr puser);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate int callback_authorizer(IntPtr puser, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_trace(IntPtr puser, IntPtr statement);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_rollback(IntPtr puser);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate int callback_exec(IntPtr db, int n, IntPtr values, IntPtr names);
	}

	static class MyDelegateTypes
	{
		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_close(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_close_v2(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_enable_shared_cache(int enable);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_interrupt(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_finalize(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_reset(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_clear_bindings(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_stmt_status(IntPtr stm, int op, int resetFlg);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_bind_parameter_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_column_database_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_column_decltype(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_column_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_column_origin_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_column_table_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_column_text(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_errmsg(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_db_readonly(IntPtr db, IntPtr dbName);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_db_filename(IntPtr db, IntPtr att);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_prepare_v2(IntPtr db, IntPtr pSql, int nBytes, IntPtr stmt, IntPtr ptrRemain);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_prepare_v3(IntPtr db, IntPtr pSql, int nBytes, int flags, IntPtr stmt, IntPtr ptrRemain);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_db_status(IntPtr db, int op, IntPtr current, IntPtr highest, int resetFlg);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_complete(IntPtr pSql);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_compileoption_used(IntPtr pSql);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_compileoption_get(int n);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_table_column_metadata(IntPtr db, IntPtr dbName, IntPtr tblName, IntPtr colName, IntPtr ptrDataType, IntPtr ptrCollSeq, IntPtr notNull, IntPtr primaryKey, IntPtr autoInc);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_value_text(IntPtr p);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_enable_load_extension(
		IntPtr db, int enable);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_load_extension(IntPtr db, IntPtr fileName, IntPtr procName, IntPtr pError);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_initialize();

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_shutdown();

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_libversion();

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_libversion_number();

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_threadsafe();

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_sourceid();

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_malloc(int n);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_realloc(IntPtr p, int n);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_free(IntPtr p);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_stricmp(IntPtr p, IntPtr q);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_strnicmp(IntPtr p, IntPtr q, int n);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_open(IntPtr filename, IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_open_v2(IntPtr filename, IntPtr db, int flags, IntPtr vfs);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_vfs_find(IntPtr vfs);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate long sqlite3_last_insert_rowid(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_changes(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_total_changes(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate long sqlite3_memory_used();

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate long sqlite3_memory_highwater(int resetFlag);
		
		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_status(int op, IntPtr current, IntPtr highwater, int resetFlag);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_busy_timeout(IntPtr db, int ms);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_bind_blob(IntPtr stmt, int index, IntPtr val, int nSize, IntPtr nTransient);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_bind_zeroblob(IntPtr stmt, int index, int size);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_bind_double(IntPtr stmt, int index, double val);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_bind_int(IntPtr stmt, int index, int val);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_bind_int64(IntPtr stmt, int index, long val);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_bind_null(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_bind_text(IntPtr stmt, int index, IntPtr val, int nlen, IntPtr pvReserved);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_bind_parameter_count(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_bind_parameter_index(IntPtr stmt, IntPtr strName);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_column_count(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_data_count(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_step(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_sql(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate double sqlite3_column_double(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_column_int(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate long sqlite3_column_int64(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_column_blob(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_column_bytes(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_column_type(IntPtr stmt, int index);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_aggregate_count(IntPtr context);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_value_blob(IntPtr p);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_value_bytes(IntPtr p);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate double sqlite3_value_double(IntPtr p);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_value_int(IntPtr p);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate long sqlite3_value_int64(IntPtr p);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_value_type(IntPtr p);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_user_data(IntPtr context);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_blob(IntPtr context, IntPtr val, int nSize, IntPtr pvReserved);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_double(IntPtr context, double val);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_error(IntPtr context, IntPtr strErr, int nLen);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_int(IntPtr context, int val);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_int64(IntPtr context, long val);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_null(IntPtr context);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_text(IntPtr context, IntPtr val, int nLen, IntPtr pvReserved);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_zeroblob(IntPtr context, int n);

		// TODO sqlite3_result_value 

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_error_toobig(IntPtr context);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_error_nomem(IntPtr context);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_result_error_code(IntPtr context, int code);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_key(IntPtr db, byte* key, int keylen);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_key_v2(IntPtr db, byte* dbname, byte* key, int keylen);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_rekey(IntPtr db, byte* key, int keylen);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_rekey_v2(IntPtr db, byte* dbname, byte* key, int keylen);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_config(int op, IntPtr extra);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_create_collation(IntPtr db, IntPtr strName, int nType, IntPtr pvUser, IntPtr func);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_update_hook(IntPtr db, IntPtr func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_commit_hook(IntPtr db, IntPtr func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_profile(IntPtr db, IntPtr func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_progress_handler(IntPtr db, int instructions, IntPtr func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_trace(IntPtr db, IntPtr func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_rollback_hook(IntPtr db, IntPtr func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_db_handle(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_stmt_isexplain(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_stmt_busy(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_stmt_readonly(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_exec(IntPtr db, IntPtr strSql, IntPtr cb, IntPtr pvParam, IntPtr errMsg);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_get_autocommit(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_extended_result_codes(IntPtr db, int onoff);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_errcode(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_extended_errcode(IntPtr db);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_errstr(int rc); /* 3.7.15+ */

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate void sqlite3_log(int iErrCode, IntPtr zFormat, IntPtr extra);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_file_control(IntPtr db, IntPtr zDbName, int op, IntPtr pArg);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate IntPtr sqlite3_backup_init(IntPtr destDb, IntPtr zDestName, IntPtr sourceDb, IntPtr zSourceName);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_backup_step(IntPtr backup, int nPage);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_backup_remaining(IntPtr backup);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_backup_pagecount(IntPtr backup);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_backup_finish(IntPtr backup);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_blob_open(IntPtr db, IntPtr sdb, IntPtr table, IntPtr col, long rowid, int flags, IntPtr blob);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_blob_write(IntPtr blob, IntPtr b, int n, int offset);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_blob_read(IntPtr blob, IntPtr b, int n, int offset);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_blob_bytes(IntPtr blob);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_blob_reopen(IntPtr blob, long rowid);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_blob_close(IntPtr blob);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_wal_autocheckpoint(IntPtr db, int n);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_wal_checkpoint(IntPtr db, IntPtr dbName);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_wal_checkpoint_v2(IntPtr db, IntPtr dbName, int eMode, IntPtr logSize, IntPtr framesCheckPointed);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_set_authorizer(IntPtr db, IntPtr cb, IntPtr pvUser);


		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public unsafe delegate int sqlite3_create_function_v2(IntPtr db, IntPtr strName, int nArgs, int nType, IntPtr pvUser, IntPtr func, IntPtr fstep, IntPtr ffinal, IntPtr fdestroy);

	}

    }
}
