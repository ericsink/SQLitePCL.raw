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
    public sealed class SQLite3Provider_sqlite3 : ISQLite3Provider
    {
		const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

        string ISQLite3Provider.GetNativeLibraryName()
        {
            return "sqlite3";
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
            fixed (byte* p = path)
            {
                return NativeMethods.sqlite3_win32_set_directory8((uint) typ, p);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_open(utf8z filename, out IntPtr db)
        {
            fixed (byte* p = filename)
            {
                return NativeMethods.sqlite3_open(p, out db);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_open_v2(utf8z filename, out IntPtr db, int flags, utf8z vfs)
        {
            fixed (byte* p_filename = filename, p_vfs = vfs)
            {
                return NativeMethods.sqlite3_open_v2(p_filename, out db, flags, p_vfs);
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
                IntPtr ptrVfs = NativeMethods.sqlite3_vfs_find(p_vfs);
                // this code and the struct it uses was taken from aspnet/DataCommon.SQLite, Apache License 2.0
                sqlite3_vfs vstruct = (sqlite3_vfs) Marshal.PtrToStructure(ptrVfs, typeof(sqlite3_vfs));
                return vstruct.xDelete(ptrVfs, p_filename, 1);
            }
		}

        int ISQLite3Provider.sqlite3_close_v2(IntPtr db)
        {
            var rc = NativeMethods.sqlite3_close_v2(db);
			return rc;
        }

        int ISQLite3Provider.sqlite3_close(IntPtr db)
        {
            var rc = NativeMethods.sqlite3_close(db);
			return rc;
        }

        void ISQLite3Provider.sqlite3_free(IntPtr p)
        {
            NativeMethods.sqlite3_free(p);
        }

        int ISQLite3Provider.sqlite3_stricmp(IntPtr p, IntPtr q)
        {
            return NativeMethods.sqlite3_stricmp(p, q);
        }

        int ISQLite3Provider.sqlite3_strnicmp(IntPtr p, IntPtr q, int n)
        {
            return NativeMethods.sqlite3_strnicmp(p, q, n);
        }

        int ISQLite3Provider.sqlite3_enable_shared_cache(int enable)
        {
            return NativeMethods.sqlite3_enable_shared_cache(enable);
        }

        void ISQLite3Provider.sqlite3_interrupt(sqlite3 db)
        {
            NativeMethods.sqlite3_interrupt(db);
        }

        [MonoPInvokeCallback (typeof(NativeMethods.callback_exec))]
        static int exec_hook_bridge(IntPtr p, int n, IntPtr values_ptr, IntPtr names_ptr)
        {
            exec_hook_info hi = exec_hook_info.from_ptr(p);
            return hi.call(n, values_ptr, names_ptr);
        }
		// TODO shouldn't there be a impl/bridge thing here?

        int ISQLite3Provider.sqlite3_exec(sqlite3 db, utf8z sql, delegate_exec func, object user_data, out IntPtr errMsg)
        {
            int rc;

			NativeMethods.callback_exec cb;
			exec_hook_info hi;
            if (func != null)
            {
				cb = exec_hook_bridge;
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
                    rc = NativeMethods.sqlite3_exec(db, p_sql, cb, h, out errMsg);
                }
            }
			h.Dispose();

            return rc;
        }

        unsafe int ISQLite3Provider.sqlite3_complete(utf8z sql)
        {
            fixed (byte* p = sql)
            {
                return NativeMethods.sqlite3_complete(p);
            }
        }

        unsafe utf8z ISQLite3Provider.sqlite3_compileoption_get(int n)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_compileoption_get(n));
        }

        unsafe int ISQLite3Provider.sqlite3_compileoption_used(utf8z s)
        {
            fixed (byte* p = s)
            {
                return NativeMethods.sqlite3_compileoption_used(p);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_table_column_metadata(sqlite3 db, utf8z dbName, utf8z tblName, utf8z colName, out utf8z dataType, out utf8z collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            fixed (byte* p_dbName = dbName, p_tblName = tblName, p_colName = colName)
            {
                var rc = NativeMethods.sqlite3_table_column_metadata(
                            db, p_dbName, p_tblName, p_colName, 
                            out var p_dataType, out var p_collSeq, out notNull, out primaryKey, out autoInc);
                dataType = utf8z.FromPtr(p_dataType);
                collSeq = utf8z.FromPtr(p_collSeq);
                return rc;
            }
        }

        unsafe int ISQLite3Provider.sqlite3_key(sqlite3 db, ReadOnlySpan<byte> k)
        {
            return raw.SQLITE_ERROR;
        }

        unsafe int ISQLite3Provider.sqlite3_key_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
        {
            return raw.SQLITE_ERROR;
        }

        unsafe int ISQLite3Provider.sqlite3_rekey(sqlite3 db, ReadOnlySpan<byte> k)
        {
            return raw.SQLITE_ERROR;
        }

        unsafe int ISQLite3Provider.sqlite3_rekey_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
        {
            return raw.SQLITE_ERROR;
        }

        unsafe int ISQLite3Provider.sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out IntPtr stm, out ReadOnlySpan<byte> tail)
        {
            fixed (byte* p_sql = sql)
            {
                var rc = NativeMethods.sqlite3_prepare_v2(db, p_sql, sql.Length, out stm, out var p_tail);
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
                var rc = NativeMethods.sqlite3_prepare_v2(db, p_sql, -1, out stm, out var p_tail);
                // TODO we could skip the strlen by using the length we were given
                tail = utf8z.FromPtr(p_tail);
                return rc;
            }
        }

        unsafe int ISQLite3Provider.sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out IntPtr stm, out ReadOnlySpan<byte> tail)
        {
            fixed (byte* p_sql = sql)
            {
                var rc = NativeMethods.sqlite3_prepare_v3(db, p_sql, sql.Length, flags, out stm, out var p_tail);
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
                var rc = NativeMethods.sqlite3_prepare_v3(db, p_sql, -1, flags, out stm, out var p_tail);
                // TODO we could skip the strlen by using the length we were given
                tail = utf8z.FromPtr(p_tail);
                return rc;
            }
        }

        int ISQLite3Provider.sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg)
        {
            return NativeMethods.sqlite3_db_status(db, op, out current, out highest, resetFlg);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_sql(sqlite3_stmt stmt)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_sql(stmt));
        }

        IntPtr ISQLite3Provider.sqlite3_db_handle(IntPtr stmt)
        {
            return NativeMethods.sqlite3_db_handle(stmt);
        }

        unsafe int ISQLite3Provider.sqlite3_blob_open(sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z col_utf8, long rowid, int flags, out sqlite3_blob blob)
        {
            fixed (byte* p_db = db_utf8, p_table = table_utf8, p_col = col_utf8)
            {
                return NativeMethods.sqlite3_blob_open(db, p_db, p_table, p_col, rowid, flags, out blob);
            }
        }

        int ISQLite3Provider.sqlite3_blob_bytes(sqlite3_blob blob)
        {
            return NativeMethods.sqlite3_blob_bytes(blob);
        }

        int ISQLite3Provider.sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
        {
            return NativeMethods.sqlite3_blob_reopen(blob, rowid);
        }

        unsafe int ISQLite3Provider.sqlite3_blob_read(sqlite3_blob blob, Span<byte> b, int offset)
        {
            fixed (byte* p = b)
            {
                return NativeMethods.sqlite3_blob_read(blob, p, b.Length, offset);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_blob_write(sqlite3_blob blob, ReadOnlySpan<byte> b, int offset)
        {
            fixed (byte* p = b)
            {
                return NativeMethods.sqlite3_blob_write(blob, p, b.Length, offset);
            }
        }

        int ISQLite3Provider.sqlite3_blob_close(IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_close(blob);
        }

        unsafe sqlite3_backup ISQLite3Provider.sqlite3_backup_init(sqlite3 destDb, utf8z destName, sqlite3 sourceDb, utf8z sourceName)
        {
            fixed (byte* p_destName = destName, p_sourceName = sourceName)
            {
                return NativeMethods.sqlite3_backup_init(destDb, p_destName, sourceDb, p_sourceName);
            }
        }

        int ISQLite3Provider.sqlite3_backup_step(sqlite3_backup backup, int nPage)
        {
            return NativeMethods.sqlite3_backup_step(backup, nPage);
        }

        int ISQLite3Provider.sqlite3_backup_remaining(sqlite3_backup backup)
        {
            return NativeMethods.sqlite3_backup_remaining(backup);
        }

        int ISQLite3Provider.sqlite3_backup_pagecount(sqlite3_backup backup)
        {
            return NativeMethods.sqlite3_backup_pagecount(backup);
        }

        int ISQLite3Provider.sqlite3_backup_finish(IntPtr backup)
        {
            return NativeMethods.sqlite3_backup_finish(backup);
        }

        IntPtr ISQLite3Provider.sqlite3_next_stmt(sqlite3 db, IntPtr stmt)
        {
            return NativeMethods.sqlite3_next_stmt(db, stmt);
        }

        long ISQLite3Provider.sqlite3_last_insert_rowid(sqlite3 db)
        {
            return NativeMethods.sqlite3_last_insert_rowid(db);
        }

        int ISQLite3Provider.sqlite3_changes(sqlite3 db)
        {
            return NativeMethods.sqlite3_changes(db);
        }

        int ISQLite3Provider.sqlite3_total_changes(sqlite3 db)
        {
            return NativeMethods.sqlite3_total_changes(db);
        }

        int ISQLite3Provider.sqlite3_extended_result_codes(sqlite3 db, int onoff)
        {
            return NativeMethods.sqlite3_extended_result_codes(db, onoff);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_errstr(int rc)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_errstr(rc));
        }

        int ISQLite3Provider.sqlite3_errcode(sqlite3 db)
        {
            return NativeMethods.sqlite3_errcode(db);
        }

        int ISQLite3Provider.sqlite3_extended_errcode(sqlite3 db)
        {
            return NativeMethods.sqlite3_extended_errcode(db);
        }

        int ISQLite3Provider.sqlite3_busy_timeout(sqlite3 db, int ms)
        {
            return NativeMethods.sqlite3_busy_timeout(db, ms);
        }

        int ISQLite3Provider.sqlite3_get_autocommit(sqlite3 db)
        {
            return NativeMethods.sqlite3_get_autocommit(db);
        }

        unsafe int ISQLite3Provider.sqlite3_db_readonly(sqlite3 db, utf8z dbName)
        {
            fixed (byte* p_dbName = dbName)
            {
                return NativeMethods.sqlite3_db_readonly(db, p_dbName); 
            }
        }
        
        unsafe utf8z ISQLite3Provider.sqlite3_db_filename(sqlite3 db, utf8z att)
		{
            fixed (byte* p_att = att)
            {
                return utf8z.FromPtr(NativeMethods.sqlite3_db_filename(db, p_att));
            }
		}

        unsafe utf8z ISQLite3Provider.sqlite3_errmsg(sqlite3 db)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_errmsg(db));
        }

        unsafe utf8z ISQLite3Provider.sqlite3_libversion()
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_libversion());
        }

        int ISQLite3Provider.sqlite3_libversion_number()
        {
            return NativeMethods.sqlite3_libversion_number();
        }

        int ISQLite3Provider.sqlite3_threadsafe()
        {
            return NativeMethods.sqlite3_threadsafe();
        }

        int ISQLite3Provider.sqlite3_config(int op)
        {
            return NativeMethods.sqlite3_config_none(op);
        }

        int ISQLite3Provider.sqlite3_config(int op, int val)
        {
            return NativeMethods.sqlite3_config_int(op, val);
        }

        int ISQLite3Provider.sqlite3_initialize()
        {
            return NativeMethods.sqlite3_initialize();
        }

        int ISQLite3Provider.sqlite3_shutdown()
        {
            return NativeMethods.sqlite3_shutdown();
        }

        int ISQLite3Provider.sqlite3_enable_load_extension(sqlite3 db, int onoff)
        {
            return NativeMethods.sqlite3_enable_load_extension(db, onoff);
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
        void ISQLite3Provider.sqlite3_commit_hook(sqlite3 db, delegate_commit func, object v)
        {
			var info = get_hooks(db);
            if (info.commit != null)
            {
                // TODO maybe turn off the hook here, for now
                info.commit.Dispose();
                info.commit = null;
            }

			NativeMethods.callback_commit cb;
			commit_hook_info hi;
            if (func != null)
            {
				cb = commit_hook_bridge;
                hi = new commit_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			NativeMethods.sqlite3_commit_hook(db, cb, h);
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

        int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_scalar func)
        {
			var info = get_hooks(db);
            if (info.RemoveScalarFunction(name, nargs))
            {
                // TODO maybe turn off the hook here, for now
            }

            // 1 is SQLITE_UTF8
			int arg4 = 1 | flags;
			NativeMethods.callback_scalar_function cb;
			function_hook_info hi;
            if (func != null)
            {
				cb = scalar_function_hook_bridge;
                hi = new function_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			int rc = NativeMethods.sqlite3_create_function_v2(db, name, nargs, arg4, h, cb, null, null, null);
			if ((rc == 0) && (cb != null))
			{
                info.AddScalarFunction(name, nargs, h.ForDispose());
			}
			return rc;
        }

        // ----------------------------------------------------------------

		static IDisposable disp_log_hook_handle;

        [MonoPInvokeCallback (typeof(NativeMethods.callback_log))]
        static void log_hook_bridge_impl(IntPtr p, int rc, IntPtr s)
        {
            log_hook_info hi = log_hook_info.from_ptr(p);
            hi.call(rc, utf8z.FromIntPtr(s));
        }

		readonly NativeMethods.callback_log log_hook_bridge = new NativeMethods.callback_log(log_hook_bridge_impl); 
        int ISQLite3Provider.sqlite3_config_log(delegate_log func, object v)
        {
            if (disp_log_hook_handle != null)
            {
                // TODO maybe turn off the hook here, for now
                disp_log_hook_handle.Dispose();
                disp_log_hook_handle = null;
            }

			NativeMethods.callback_log cb;
			log_hook_info hi;
            if (func != null)
            {
				cb = log_hook_bridge;
                hi = new log_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			disp_log_hook_handle = h; // TODO if valid
			var rc = NativeMethods.sqlite3_config_log(raw.SQLITE_CONFIG_LOG, cb, h);
			return rc;
        }

        unsafe void ISQLite3Provider.sqlite3_log(int errcode, utf8z s)
        {
            fixed (byte* p = s)
            {
                NativeMethods.sqlite3_log(errcode, p);
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

        int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
			var info = get_hooks(db);
            if (info.RemoveAggFunction(name, nargs))
            {
                // TODO maybe turn off the hook here, for now
            }

            // 1 is SQLITE_UTF8
			int arg4 = 1 | flags;
			NativeMethods.callback_agg_function_step cb_step;
			NativeMethods.callback_agg_function_final cb_final;
			function_hook_info hi;
            if (func_step != null)
            {
                // TODO both func_step and func_final must be non-null
				cb_step = agg_function_hook_bridge_step;
				cb_final = agg_function_hook_bridge_final;
                hi = new function_hook_info(func_step, func_final, v);
            }
            else
            {
				cb_step = null;
				cb_final = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			int rc = NativeMethods.sqlite3_create_function_v2(db, name, nargs, arg4, h, null, cb_step, cb_final, null);
			if ((rc == 0) && (cb_step != null))
			{
                info.AddAggFunction(name, nargs, h.ForDispose());
			}
			return rc;
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
        int ISQLite3Provider.sqlite3_create_collation(sqlite3 db, byte[] name, object v, delegate_collation func)
        {
			var info = get_hooks(db);
            if (info.RemoveCollation(name))
            {
                // TODO maybe turn off the hook here, for now
            }

			NativeMethods.callback_collation cb;
			collation_hook_info hi;
            if (func != null)
            {
				cb = collation_hook_bridge;
                hi = new collation_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
            // 1 is SQLITE_UTF8
			int rc = NativeMethods.sqlite3_create_collation(db, name, 1, h, cb);
			if ((rc == 0) && (cb != null))
			{
                info.AddCollation(name, h.ForDispose());
			}
			return rc;
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
        void ISQLite3Provider.sqlite3_update_hook(sqlite3 db, delegate_update func, object v)
        {
			var info = get_hooks(db);
            if (info.update != null)
            {
                // TODO maybe turn off the hook here, for now
                info.update.Dispose();
                info.update = null;
            }

			NativeMethods.callback_update cb;
			update_hook_info hi;
            if (func != null)
            {
				cb = update_hook_bridge;
                hi = new update_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
            info.update = h.ForDispose();
			NativeMethods.sqlite3_update_hook(db, cb, h);
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
        void ISQLite3Provider.sqlite3_rollback_hook(sqlite3 db, delegate_rollback func, object v)
        {
			var info = get_hooks(db);
            if (info.rollback != null)
            {
                // TODO maybe turn off the hook here, for now
                info.rollback.Dispose();
                info.rollback = null;
            }

			NativeMethods.callback_rollback cb;
			rollback_hook_info hi;
            if (func != null)
            {
				cb = rollback_hook_bridge;
                hi = new rollback_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.rollback = h.ForDispose();
			NativeMethods.sqlite3_rollback_hook(db, cb, h);
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
        void ISQLite3Provider.sqlite3_trace(sqlite3 db, delegate_trace func, object v)
        {
			var info = get_hooks(db);
            if (info.trace != null)
            {
                // TODO maybe turn off the hook here, for now
                info.trace.Dispose();
                info.trace = null;
            }

			NativeMethods.callback_trace cb;
			trace_hook_info hi;
            if (func != null)
            {
				cb = trace_hook_bridge;
                hi = new trace_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.trace = h.ForDispose();
			NativeMethods.sqlite3_trace(db, cb, h);
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
        void ISQLite3Provider.sqlite3_profile(sqlite3 db, delegate_profile func, object v)
        {
			var info = get_hooks(db);
            if (info.profile != null)
            {
                // TODO maybe turn off the hook here, for now
                info.profile.Dispose();
                info.profile = null;
            }

			NativeMethods.callback_profile cb;
			profile_hook_info hi;
            if (func != null)
            {
				cb = profile_hook_bridge;
                hi = new profile_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.profile = h.ForDispose();
			NativeMethods.sqlite3_profile(db, cb, h);
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
        void ISQLite3Provider.sqlite3_progress_handler(sqlite3 db, int instructions, delegate_progress func, object v)
        {
			var info = get_hooks(db);
            if (info.progress != null)
            {
                // TODO maybe turn off the hook here, for now
                info.progress.Dispose();
                info.progress = null;
            }

			NativeMethods.callback_progress_handler cb;
			progress_hook_info hi;
            if (func != null)
            {
				cb = progress_hook_bridge;
                hi = new progress_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.progress = h.ForDispose();
			NativeMethods.sqlite3_progress_handler(db, instructions, cb, h);
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
        int ISQLite3Provider.sqlite3_set_authorizer(sqlite3 db, delegate_authorizer func, object v)
        {
			var info = get_hooks(db);
            if (info.authorizer != null)
            {
                // TODO maybe turn off the hook here, for now
                info.authorizer.Dispose();
                info.authorizer = null;
            }

			NativeMethods.callback_authorizer cb;
			authorizer_hook_info hi;
            if (func != null)
            {
				cb = authorizer_hook_bridge;
                hi = new authorizer_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.authorizer = h.ForDispose();
			return NativeMethods.sqlite3_set_authorizer(db, cb, h);
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

        int ISQLite3Provider.sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            return NativeMethods.sqlite3_status(op, out current, out highwater, resetFlag);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_sourceid()
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_sourceid());
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
                NativeMethods.sqlite3_result_error(ctx, p, val.Length);
            }
        }

        unsafe void ISQLite3Provider.sqlite3_result_error(IntPtr ctx, utf8z val)
        {
            fixed (byte* p = val)
            {
                NativeMethods.sqlite3_result_error(ctx, p, -1);
            }
        }

        unsafe void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, ReadOnlySpan<byte> val)
        {
            fixed (byte* p = val)
            {
                NativeMethods.sqlite3_result_text(ctx, p, val.Length, new IntPtr(-1));
            }
        }

        unsafe void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, utf8z val)
        {
            fixed (byte* p = val)
            {
                NativeMethods.sqlite3_result_text(ctx, p, -1, new IntPtr(-1));
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
            return utf8z.FromPtr(NativeMethods.sqlite3_value_text(p));
        }

        int ISQLite3Provider.sqlite3_bind_int(sqlite3_stmt stm, int paramIndex, int val)
        {
            return NativeMethods.sqlite3_bind_int(stm, paramIndex, val);
        }

        int ISQLite3Provider.sqlite3_bind_int64(sqlite3_stmt stm, int paramIndex, long val)
        {
            return NativeMethods.sqlite3_bind_int64(stm, paramIndex, val);
        }

        unsafe int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stm, int paramIndex, ReadOnlySpan<byte> t)
        {
            fixed (byte* p_t = t)
            {
                return NativeMethods.sqlite3_bind_text(stm, paramIndex, p_t, t.Length, new IntPtr(-1));
            }
        }

        unsafe int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stm, int paramIndex, utf8z t)
        {
            fixed (byte* p_t = t)
            {
                return NativeMethods.sqlite3_bind_text(stm, paramIndex, p_t, -1, new IntPtr(-1));
            }
        }

        int ISQLite3Provider.sqlite3_bind_double(sqlite3_stmt stm, int paramIndex, double val)
        {
            return NativeMethods.sqlite3_bind_double(stm, paramIndex, val);
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
                    return NativeMethods.sqlite3_bind_blob(stm, paramIndex, p_fake, 0, new IntPtr(-1));
                }
            }
            else
            {
                fixed (byte* p = blob)
                {
                    return NativeMethods.sqlite3_bind_blob(stm, paramIndex, p, blob.Length, new IntPtr(-1));
                }
            }
        }

        int ISQLite3Provider.sqlite3_bind_zeroblob(sqlite3_stmt stm, int paramIndex, int size)
        {
            return NativeMethods.sqlite3_bind_zeroblob(stm, paramIndex, size);
        }

        int ISQLite3Provider.sqlite3_bind_null(sqlite3_stmt stm, int paramIndex)
        {
            return NativeMethods.sqlite3_bind_null(stm, paramIndex);
        }

        int ISQLite3Provider.sqlite3_bind_parameter_count(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_bind_parameter_count(stm);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_bind_parameter_name(sqlite3_stmt stm, int paramIndex)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_bind_parameter_name(stm, paramIndex));
        }

        unsafe int ISQLite3Provider.sqlite3_bind_parameter_index(sqlite3_stmt stm, utf8z paramName)
        {
            fixed (byte* p_paramName = paramName)
            {
                return NativeMethods.sqlite3_bind_parameter_index(stm, p_paramName);
            }
        }

        int ISQLite3Provider.sqlite3_step(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_step(stm);
        }

        int ISQLite3Provider.sqlite3_stmt_isexplain(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_stmt_isexplain(stm);
        }

        int ISQLite3Provider.sqlite3_stmt_busy(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_stmt_busy(stm);
        }

        int ISQLite3Provider.sqlite3_stmt_readonly(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_stmt_readonly(stm);
        }

        int ISQLite3Provider.sqlite3_column_int(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_int(stm, columnIndex);
        }

        long ISQLite3Provider.sqlite3_column_int64(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_int64(stm, columnIndex);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_text(sqlite3_stmt stm, int columnIndex)
        {
            byte* p = NativeMethods.sqlite3_column_text(stm, columnIndex);
            var length = NativeMethods.sqlite3_column_bytes(stm, columnIndex);
            return utf8z.FromPtrLen(p, length);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_decltype(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_column_decltype(stm, columnIndex));
        }

        double ISQLite3Provider.sqlite3_column_double(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_double(stm, columnIndex);
        }

        ReadOnlySpan<byte> ISQLite3Provider.sqlite3_column_blob(sqlite3_stmt stm, int columnIndex)
        {
            IntPtr blobPointer = NativeMethods.sqlite3_column_blob(stm, columnIndex);
            if (blobPointer == IntPtr.Zero)
            {
                return null;
            }

            var length = NativeMethods.sqlite3_column_bytes(stm, columnIndex);
            unsafe
            {
                return new ReadOnlySpan<byte>(blobPointer.ToPointer(), length);
            }
        }

        int ISQLite3Provider.sqlite3_column_type(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_type(stm, columnIndex);
        }

        int ISQLite3Provider.sqlite3_column_bytes(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_bytes(stm, columnIndex);
        }

        int ISQLite3Provider.sqlite3_column_count(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_column_count(stm);
        }

        int ISQLite3Provider.sqlite3_data_count(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_data_count(stm);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_name(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_column_name(stm, columnIndex));
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_origin_name(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_column_origin_name(stm, columnIndex));
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_table_name(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_column_table_name(stm, columnIndex));
        }

        unsafe utf8z ISQLite3Provider.sqlite3_column_database_name(sqlite3_stmt stm, int columnIndex)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_column_database_name(stm, columnIndex));
        }

        int ISQLite3Provider.sqlite3_reset(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_reset(stm);
        }

        int ISQLite3Provider.sqlite3_clear_bindings(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_clear_bindings(stm);
        }

        int ISQLite3Provider.sqlite3_stmt_status(sqlite3_stmt stm, int op, int resetFlg)
        {
            return NativeMethods.sqlite3_stmt_status(stm, op, resetFlg);
        }

        int ISQLite3Provider.sqlite3_finalize(IntPtr stm)
        {
            return NativeMethods.sqlite3_finalize(stm);
        }

        int ISQLite3Provider.sqlite3_wal_autocheckpoint(sqlite3 db, int n)
        {
            return NativeMethods.sqlite3_wal_autocheckpoint(db, n);
        }

        unsafe int ISQLite3Provider.sqlite3_wal_checkpoint(sqlite3 db, utf8z dbName)
        {
            fixed (byte* p_dbName = dbName)
            {
                return NativeMethods.sqlite3_wal_checkpoint(db, p_dbName);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_wal_checkpoint_v2(sqlite3 db, utf8z dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            fixed (byte* p_dbName = dbName)
            {
                return NativeMethods.sqlite3_wal_checkpoint_v2(db, p_dbName, eMode, out logSize, out framesCheckPointed);
            }
        }

	static class NativeMethods
	{
        private const string SQLITE_DLL = "sqlite3";

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_close(IntPtr db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_close_v2(IntPtr db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_enable_shared_cache(int enable);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_interrupt(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_finalize(IntPtr stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_reset(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_clear_bindings(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_stmt_status(sqlite3_stmt stm, int op, int resetFlg);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_column_database_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_column_decltype(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_column_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_column_origin_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_column_table_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_column_text(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_errmsg(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_db_readonly(sqlite3 db, byte* dbName);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_db_filename(sqlite3 db, byte* att);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_prepare_v2(sqlite3 db, byte* pSql, int nBytes, out IntPtr stmt, out byte* ptrRemain);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_prepare_v3(sqlite3 db, byte* pSql, int nBytes, uint flags, out IntPtr stmt, out byte* ptrRemain);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_complete(byte* pSql);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_compileoption_used(byte* pSql);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_compileoption_get(int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_table_column_metadata(sqlite3 db, byte* dbName, byte* tblName, byte* colName, out byte* ptrDataType, out byte* ptrCollSeq, out int notNull, out int primaryKey, out int autoInc);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_value_text(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_enable_load_extension(
		sqlite3 db, int enable);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_load_extension(
		sqlite3 db, byte[] fileName, byte[] procName, ref IntPtr pError);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_initialize();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_shutdown();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_libversion();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_libversion_number();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_threadsafe();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_sourceid();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_malloc(int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_realloc(IntPtr p, int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_free(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_stricmp(IntPtr p, IntPtr q);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_strnicmp(IntPtr p, IntPtr q, int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_open(byte* filename, out IntPtr db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_open_v2(byte* filename, out IntPtr db, int flags, byte* vfs);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_vfs_find(byte* vfs);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe long sqlite3_last_insert_rowid(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_changes(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_total_changes(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe long sqlite3_memory_used();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe long sqlite3_memory_highwater(int resetFlag);
		
		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_status(int op, out int current, out int highwater, int resetFlag);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_busy_timeout(sqlite3 db, int ms);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_bind_blob(sqlite3_stmt stmt, int index, byte* val, int nSize, IntPtr nTransient);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_bind_double(sqlite3_stmt stmt, int index, double val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_bind_int(sqlite3_stmt stmt, int index, int val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_bind_null(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_bind_text(sqlite3_stmt stmt, int index, byte* val, int nlen, IntPtr pvReserved);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_bind_parameter_count(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_bind_parameter_index(sqlite3_stmt stmt, byte* strName);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_column_count(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_data_count(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_step(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_sql(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe double sqlite3_column_double(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_column_int(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe long sqlite3_column_int64(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_column_blob(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_column_bytes(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_column_type(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_aggregate_count(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_value_blob(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_value_bytes(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe double sqlite3_value_double(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_value_int(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe long sqlite3_value_int64(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_value_type(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_user_data(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_blob(IntPtr context, IntPtr val, int nSize, IntPtr pvReserved);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_double(IntPtr context, double val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_error(IntPtr context, byte* strErr, int nLen);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_int(IntPtr context, int val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_int64(IntPtr context, long val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_null(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_text(IntPtr context, byte* val, int nLen, IntPtr pvReserved);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_zeroblob(IntPtr context, int n);

		// TODO sqlite3_result_value 

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_error_toobig(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_error_nomem(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_result_error_code(IntPtr context, int code);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes);


		// Since sqlite3_config() takes a variable argument list, we have to overload declarations
		// for all possible calls that we want to use.
		[DllImport(SQLITE_DLL, ExactSpelling=true, EntryPoint = "sqlite3_config", CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_config_none(int op);

		[DllImport(SQLITE_DLL, ExactSpelling=true, EntryPoint = "sqlite3_config", CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_config_int(int op, int val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, EntryPoint = "sqlite3_config", CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_config_log(int op, NativeMethods.callback_log func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_create_collation(sqlite3 db, byte[] strName, int nType, hook_handle pvUser, NativeMethods.callback_collation func);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_update_hook(sqlite3 db, NativeMethods.callback_update func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_commit_hook(sqlite3 db, NativeMethods.callback_commit func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_profile(sqlite3 db, NativeMethods.callback_profile func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_progress_handler(sqlite3 db, int instructions, NativeMethods.callback_progress_handler func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_trace(sqlite3 db, NativeMethods.callback_trace func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_rollback_hook(sqlite3 db, NativeMethods.callback_rollback func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_db_handle(IntPtr stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe IntPtr sqlite3_next_stmt(sqlite3 db, IntPtr stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_stmt_isexplain(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_stmt_busy(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_stmt_readonly(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_exec(sqlite3 db, byte* strSql, NativeMethods.callback_exec cb, hook_handle pvParam, out IntPtr errMsg);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_get_autocommit(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_extended_result_codes(sqlite3 db, int onoff);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_errcode(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_extended_errcode(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe byte* sqlite3_errstr(int rc); /* 3.7.15+ */

		// Since sqlite3_log() takes a variable argument list, we have to overload declarations
		// for all possible calls.  For now, we are only exposing a single string, and 
		// depend on the caller to format the string.
		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe void sqlite3_log(int iErrCode, byte* zFormat);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_file_control(sqlite3 db, byte[] zDbName, int op, IntPtr pArg);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe sqlite3_backup sqlite3_backup_init(sqlite3 destDb, byte* zDestName, sqlite3 sourceDb, byte* zSourceName);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_backup_step(sqlite3_backup backup, int nPage);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_backup_remaining(sqlite3_backup backup);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_backup_pagecount(sqlite3_backup backup);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_backup_finish(IntPtr backup);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_blob_open(sqlite3 db, byte* sdb, byte* table, byte* col, long rowid, int flags, out sqlite3_blob blob);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_blob_write(sqlite3_blob blob, byte* b, int n, int offset);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_blob_read(sqlite3_blob blob, byte* b, int n, int offset);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_blob_bytes(sqlite3_blob blob);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_blob_reopen(sqlite3_blob blob, long rowid);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_blob_close(IntPtr blob);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_wal_autocheckpoint(sqlite3 db, int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_wal_checkpoint(sqlite3 db, byte* dbName);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_wal_checkpoint_v2(sqlite3 db, byte* dbName, int eMode, out int logSize, out int framesCheckPointed);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_set_authorizer(sqlite3 db, NativeMethods.callback_authorizer cb, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_win32_set_directory8(uint directoryType, byte* directoryPath);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_create_function_v2(sqlite3 db, byte[] strName, int nArgs, int nType, hook_handle pvUser, NativeMethods.callback_scalar_function func, NativeMethods.callback_agg_function_step fstep, NativeMethods.callback_agg_function_final ffinal, NativeMethods.callback_destroy fdestroy);


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


    }
}
