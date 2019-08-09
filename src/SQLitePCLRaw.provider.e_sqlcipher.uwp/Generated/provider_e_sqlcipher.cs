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
    public sealed class SQLite3Provider_e_sqlcipher : ISQLite3Provider
    {
		const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

        string ISQLite3Provider.GetNativeLibraryName()
        {
            return "e_sqlcipher";
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

        // --------
        // this code pre-dates the vfs code.  it was simply a way
        // to call the xDelete method of a vfs.
        // this code was taken from aspnet/DataCommon.SQLite, Apache License 2.0

		#pragma warning disable 649
		private struct my_sqlite3_vfs
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
                my_sqlite3_vfs vstruct = (my_sqlite3_vfs) Marshal.PtrToStructure(ptrVfs, typeof(my_sqlite3_vfs));
                return vstruct.xDelete(ptrVfs, p_filename, 1);
            }
		}

        // --------

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
        static int exec_hook_bridge_impl(IntPtr p, int n, IntPtr values_ptr, IntPtr names_ptr)
        {
            exec_hook_info hi = exec_hook_info.from_ptr(p);
            return hi.call(n, values_ptr, names_ptr);
        }
		readonly NativeMethods.callback_exec exec_hook_bridge = new NativeMethods.callback_exec(exec_hook_bridge_impl); 

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
            fixed (byte* p = k)
            {
                return NativeMethods.sqlite3_key(db, p, k.Length);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_key_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
        {
            fixed (byte* p = k, p_name = name)
            {
                return NativeMethods.sqlite3_key_v2(db, p_name, p, k.Length);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_rekey(sqlite3 db, ReadOnlySpan<byte> k)
        {
            fixed (byte* p = k)
            {
                return NativeMethods.sqlite3_rekey(db, p, k.Length);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_rekey_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
        {
            fixed (byte* p = k, p_name = name)
            {
                return NativeMethods.sqlite3_rekey_v2(db, p_name, p, k.Length);
            }
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
                return new ReadOnlySpan<byte>(p.ToPointer(), length);
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
            fixed (byte* p = blob)
            {
                return NativeMethods.sqlite3_bind_blob(stm, paramIndex, p, blob.Length, new IntPtr(-1));
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

        // ----------------------------------------------------------------
        // vfs

		private struct native_sqlite3_vfs
		{
			public int iVersion;

			public int szOsFile;
			public int mxPathname;
			public IntPtr pNext;
			public IntPtr zName;
			public IntPtr pAppData;

			public NativeMethods.delegate_vfs_xOpen xOpen;
			public NativeMethods.delegate_vfs_xDelete xDelete;
			public NativeMethods.delegate_vfs_xAccess xAccess;
			public NativeMethods.delegate_vfs_xFullPathname xFullPathname;
			public NativeMethods.delegate_vfs_xDlOpen xDlOpen;
			public NativeMethods.delegate_vfs_xDlError xDlError;
			public NativeMethods.delegate_vfs_xDlSym xDlSym;
			public NativeMethods.delegate_vfs_xDlClose xDlClose;
			public NativeMethods.delegate_vfs_xRandomness xRandomness;
			public NativeMethods.delegate_vfs_xSleep xSleep;
			public NativeMethods.delegate_vfs_xCurrentTime xCurrentTime;
			public NativeMethods.delegate_vfs_xGetLastError xGetLastError;
			public NativeMethods.delegate_vfs_xCurrentTimeInt64 xCurrentTimeInt64;
		}

		private struct native_sqlite3_file
        {
            public IntPtr pMethods;
            public IntPtr pAppData;
        }

		private struct native_sqlite3_io_methods
        {
			public int iVersion;

            public NativeMethods.delegate_io_xClose xClose;
            public NativeMethods.delegate_io_xRead xRead;
            public NativeMethods.delegate_io_xWrite xWrite;
            public NativeMethods.delegate_io_xTruncate xTruncate;
            public NativeMethods.delegate_io_xSync xSync;
            public NativeMethods.delegate_io_xFileSize xFileSize;
            public NativeMethods.delegate_io_xLock xLock;
            public NativeMethods.delegate_io_xUnlock xUnlock;
            public NativeMethods.delegate_io_xCheckReservedLock xCheckReservedLock;
            public NativeMethods.delegate_io_xFileControl xFileControl;
            public NativeMethods.delegate_io_xSectorSize xSectorSize;
            public NativeMethods.delegate_io_xDeviceCharacteristics xDeviceCharacteristics;

#if not
            public NativeMethods.delegate_io_xShmMap xShmMap;
            public NativeMethods.delegate_io_xShmLock xShmLock;
            public NativeMethods.delegate_io_xShmBarrier xShmBarrier;
            public NativeMethods.delegate_io_xShmUnmap xShmUnmap;

            public NativeMethods.delegate_io_xFetch xFetch;
            public NativeMethods.delegate_io_xUnfetch xUnfetch;
#endif
        }

        static sqlite3_vfs extract_vfs(IntPtr p_vfs)
        {
            // TODO how to get pAppData without marshaling the whole struct?
            // maybe don't marshal it or use pAppData at all?  
            // just keep a dictionary by IntPtr?

            var nat = Marshal.PtrToStructure<native_sqlite3_vfs>(p_vfs);
            var h = GCHandle.FromIntPtr(nat.pAppData);
            var vfs = (sqlite3_vfs) h.Target;
            return vfs;
        }

        static sqlite3_io_methods extract_io_methods(IntPtr p_file)
        {
            // TODO see above. again, perf concerns of marshalling the whole struct?

            var nat = Marshal.PtrToStructure<native_sqlite3_file>(p_file);
            var h = GCHandle.FromIntPtr(nat.pAppData);
            var io = (sqlite3_io_methods) h.Target;
            return io;
        }

        // --------

		static unsafe NativeMethods.delegate_vfs_xOpen xOpen_bridge = new NativeMethods.delegate_vfs_xOpen(xOpen_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xOpen))]
        static unsafe int xOpen_bridge_impl(
            void* p_vfs,
            byte* psz_name,
            void* p_file,
            int flags,
            int* p_out_flags
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            var rc = vfs.xOpen(
                utf8z.FromIntPtr((IntPtr)psz_name),
                out var io,
                flags,
                out var out_flags
                );

            var f = new native_sqlite3_file();

            if (io != null)
            {
                var nat_io = new native_sqlite3_io_methods();
                nat_io.iVersion = 1; // TODO
                nat_io.xClose = xClose_bridge;
                nat_io.xRead = xRead_bridge;
                nat_io.xWrite = xWrite_bridge;
                nat_io.xTruncate = xTruncate_bridge;
                nat_io.xSync = xSync_bridge;
                nat_io.xFileSize = xFileSize_bridge;
                nat_io.xLock = xLock_bridge;
                nat_io.xUnlock = xUnlock_bridge;
                nat_io.xCheckReservedLock = xCheckReservedLock_bridge;
                nat_io.xFileControl = xFileControl_bridge;
                nat_io.xSectorSize = xSectorSize_bridge;
                nat_io.xDeviceCharacteristics = xDeviceCharacteristics_bridge;

#if not
                nat_io.xShmMap = xShmMap_bridge;
                nat_io.xShmBarrier = xShmBarrier_bridge;
                nat_io.xShmLock = xShmLock_bridge;
                nat_io.xShmUnmap = xShmUnmap_bridge;

                nat_io.xFetch = xFetch_bridge;
                nat_io.xUnfetch = xUnfetch_bridge;
#endif

                var p = Marshal.AllocHGlobal(Marshal.SizeOf<native_sqlite3_io_methods>());
                // TODO need to free this somewhere
                Marshal.StructureToPtr(nat_io, p, false);
                f.pMethods = p;
            }
            else
            {
                f.pMethods = IntPtr.Zero;
            }

            // TODO worry about whether this handle is enough to prevent GC
            var h = GCHandle.Alloc(io, GCHandleType.Normal);
            f.pAppData = GCHandle.ToIntPtr(h);

            Marshal.StructureToPtr(f, (IntPtr)p_file, false);
            if (p_out_flags != null)
            {
                *p_out_flags = out_flags;
            }


            return rc;
        }

		static unsafe NativeMethods.delegate_vfs_xDelete xDelete_bridge = new NativeMethods.delegate_vfs_xDelete(xDelete_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xDelete))]
        static unsafe int xDelete_bridge_impl(
            void* p_vfs,
            byte* psz_name,
            int flags
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            return vfs.xDelete(utf8z.FromIntPtr((IntPtr)psz_name), flags);
        }

		static unsafe NativeMethods.delegate_vfs_xAccess xAccess_bridge = new NativeMethods.delegate_vfs_xAccess(xAccess_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xAccess))]
        static unsafe int xAccess_bridge_impl(
            void* p_vfs,
            byte* psz_name,
            int flags,
            int* p_out
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            var rc = vfs.xAccess(utf8z.FromPtr(psz_name), flags, out var res);
            *p_out = res;
            return rc;
        }

		static unsafe NativeMethods.delegate_vfs_xFullPathname xFullPathname_bridge = new NativeMethods.delegate_vfs_xFullPathname(xFullPathname_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xFullPathname))]
        static unsafe int xFullPathname_bridge_impl(
            void* p_vfs,
            byte* psz_name,
            int nOut,
            byte* psz_out
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            return vfs.xFullPathname(utf8z.FromPtr(psz_name), new Span<byte>(psz_out, nOut));
        }

		static unsafe NativeMethods.delegate_vfs_xDlOpen xDlOpen_bridge = new NativeMethods.delegate_vfs_xDlOpen(xDlOpen_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xDlOpen))]
        static unsafe void* xDlOpen_bridge_impl(
            void* p_vfs,
            byte* psz_name
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);

            return null; // TODO
        }

		static unsafe NativeMethods.delegate_vfs_xDlError xDlError_bridge = new NativeMethods.delegate_vfs_xDlError(xDlError_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xDlError))]
        static unsafe void xDlError_bridge_impl(
            void* p_vfs,
            int nByte,
            byte* psz_errMsg
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            // TODO
        }

		static unsafe NativeMethods.delegate_vfs_xDlSym xDlSym_bridge = new NativeMethods.delegate_vfs_xDlSym(xDlSym_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xDlSym))]
        static unsafe void* xDlSym_bridge_impl(
            void* p_vfs,
            void* p_dl,
            byte* psz_name
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);

            return null; // TODO
        }

		static unsafe NativeMethods.delegate_vfs_xDlClose xDlClose_bridge = new NativeMethods.delegate_vfs_xDlClose(xDlClose_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xDlClose))]
        static unsafe void xDlClose_bridge_impl(
            void* p_vfs,
            void* p_dl
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            // TODO
        }

		static unsafe NativeMethods.delegate_vfs_xRandomness xRandomness_bridge = new NativeMethods.delegate_vfs_xRandomness(xRandomness_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xRandomness))]
        static unsafe int xRandomness_bridge_impl(
            void* p_vfs,
            int nByte,
            byte* buf
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            return vfs.xRandomness(new Span<byte>(buf, nByte));
        }

		static unsafe NativeMethods.delegate_vfs_xSleep xSleep_bridge = new NativeMethods.delegate_vfs_xSleep(xSleep_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xSleep))]
        static unsafe int xSleep_bridge_impl(
            void* p_vfs,
            int microseconds
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            return vfs.xSleep(microseconds);
        }

		static unsafe NativeMethods.delegate_vfs_xCurrentTime xCurrentTime_bridge = new NativeMethods.delegate_vfs_xCurrentTime(xCurrentTime_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xCurrentTime))]
        static unsafe int xCurrentTime_bridge_impl(
            void* p_vfs,
            double* p
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            var rc = vfs.xCurrentTime(out var res);
            *p = res;
            return rc;
        }

		static unsafe NativeMethods.delegate_vfs_xGetLastError xGetLastError_bridge = new NativeMethods.delegate_vfs_xGetLastError(xGetLastError_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xGetLastError))]
        static unsafe int xGetLastError_bridge_impl(
            void* p_vfs,
            int nByte,
            byte* psz_out
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            return vfs.xGetLastError(new Span<byte>(psz_out, nByte));
        }

		static unsafe NativeMethods.delegate_vfs_xCurrentTimeInt64 xCurrentTimeInt64_bridge = new NativeMethods.delegate_vfs_xCurrentTimeInt64(xCurrentTimeInt64_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_vfs_xCurrentTimeInt64))]
        static unsafe int xCurrentTimeInt64_bridge_impl(
            void* p_vfs,
            long* p
            )
        {
            var vfs = extract_vfs((IntPtr)p_vfs);
            var rc = vfs.xCurrentTimeInt64(out var res);
            *p = res;
            return rc;
        }

        // --------

		static unsafe NativeMethods.delegate_io_xClose xClose_bridge = new NativeMethods.delegate_io_xClose(xClose_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xClose))]
        static unsafe int xClose_bridge_impl(
            void* p_file
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            var rc = io.xClose();
            // TODO free io stuff
            return rc;
        }

		static unsafe NativeMethods.delegate_io_xRead xRead_bridge = new NativeMethods.delegate_io_xRead(xRead_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xRead))]
        static unsafe int xRead_bridge_impl(
            void* p_file,
            void* p_buf,
            int iAmt,
            long iOfst
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return io.xRead(new Span<byte>(p_buf, iAmt), iOfst);
        }

		static unsafe NativeMethods.delegate_io_xWrite xWrite_bridge = new NativeMethods.delegate_io_xWrite(xWrite_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xWrite))]
        static unsafe int xWrite_bridge_impl(
            void* p_file,
            void* p_buf,
            int iAmt,
            long iOfst
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return io.xWrite(new ReadOnlySpan<byte>(p_buf, iAmt), iOfst);
        }

		static unsafe NativeMethods.delegate_io_xTruncate xTruncate_bridge = new NativeMethods.delegate_io_xTruncate(xTruncate_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xTruncate))]
        static unsafe int xTruncate_bridge_impl(
            void* p_file,
            long size
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return io.xTruncate(size);
        }

		static unsafe NativeMethods.delegate_io_xSync xSync_bridge = new NativeMethods.delegate_io_xSync(xSync_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xSync))]
        static unsafe int xSync_bridge_impl(
            void* p_file,
            int flags
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return io.xSync(flags);
        }

		static unsafe NativeMethods.delegate_io_xFileSize xFileSize_bridge = new NativeMethods.delegate_io_xFileSize(xFileSize_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xFileSize))]
        static unsafe int xFileSize_bridge_impl(
            void* p_file,
            long* p_size
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            var rc = io.xFileSize(out var len);
            *p_size = len;
            return rc;
        }

		static unsafe NativeMethods.delegate_io_xLock xLock_bridge = new NativeMethods.delegate_io_xLock(xLock_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xLock))]
        static unsafe int xLock_bridge_impl(
            void* p_file,
            int x
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return io.xLock(x);
        }

		static unsafe NativeMethods.delegate_io_xUnlock xUnlock_bridge = new NativeMethods.delegate_io_xUnlock(xUnlock_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xUnlock))]
        static unsafe int xUnlock_bridge_impl(
            void* p_file,
            int x
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return io.xUnlock(x);
        }

		static unsafe NativeMethods.delegate_io_xCheckReservedLock xCheckReservedLock_bridge = new NativeMethods.delegate_io_xCheckReservedLock(xCheckReservedLock_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xCheckReservedLock))]
        static unsafe int xCheckReservedLock_bridge_impl(
            void* p_file,
            int* pResOut
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            var rc = io.xCheckReservedLock(out var res);
            *pResOut = res;
            return rc;
        }

		static unsafe NativeMethods.delegate_io_xFileControl xFileControl_bridge = new NativeMethods.delegate_io_xFileControl(xFileControl_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xFileControl))]
        static unsafe int xFileControl_bridge_impl(
            void* p_file,
            int op,
            void* pArg
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return io.xFileControl(op, (IntPtr)pArg);
        }

		static unsafe NativeMethods.delegate_io_xSectorSize xSectorSize_bridge = new NativeMethods.delegate_io_xSectorSize(xSectorSize_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xSectorSize))]
        static unsafe int xSectorSize_bridge_impl(
            void* p_file
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return io.xSectorSize();
        }

		static unsafe NativeMethods.delegate_io_xDeviceCharacteristics xDeviceCharacteristics_bridge = new NativeMethods.delegate_io_xDeviceCharacteristics(xDeviceCharacteristics_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xDeviceCharacteristics))]
        static unsafe int xDeviceCharacteristics_bridge_impl(
            void* p_file
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return io.xDeviceCharacteristics();
        }

#if not
		static unsafe NativeMethods.delegate_io_xShmMap xShmMap_bridge = new NativeMethods.delegate_io_xShmMap(xShmMap_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xShmMap))]
        static unsafe int xShmMap_bridge_impl(
            void* p_file,
            int iPg,
            int pgsz,
            int x,
            void* p
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return -1; // TODO
        }

		static unsafe NativeMethods.delegate_io_xShmLock xShmLock_bridge = new NativeMethods.delegate_io_xShmLock(xShmLock_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xShmLock))]
        static unsafe int xShmLock_bridge_impl(
            void* p_file,
            int offset,
            int n,
            int flags
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return -1; // TODO
        }

		static unsafe NativeMethods.delegate_io_xShmBarrier xShmBarrier_bridge = new NativeMethods.delegate_io_xShmBarrier(xShmBarrier_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xShmBarrier))]
        static unsafe void xShmBarrier_bridge_impl(
            void* p_file
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            // TODO
        }

		static unsafe NativeMethods.delegate_io_xShmUnmap xShmUnmap_bridge = new NativeMethods.delegate_io_xShmUnmap(xShmUnmap_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xShmUnmap))]
        static unsafe int xShmUnmap_bridge_impl(
            void* p_file,
            int deleteFlag
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return -1; // TODO
        }

		static unsafe NativeMethods.delegate_io_xFetch xFetch_bridge = new NativeMethods.delegate_io_xFetch(xFetch_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xFetch))]
        static unsafe int xFetch_bridge_impl(
            void* p_file,
            long iOfst,
            int iAmt,
            void** p
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return -1; // TODO
        }

		static unsafe NativeMethods.delegate_io_xUnfetch xUnfetch_bridge = new NativeMethods.delegate_io_xUnfetch(xUnfetch_bridge_impl); 
        [MonoPInvokeCallback (typeof(NativeMethods.delegate_io_xUnfetch))]
        static unsafe int xUnfetch_bridge_impl(
            void* p_file,
            long iOfst,
            void* p
            )
        {
            var io = extract_io_methods((IntPtr) p_file);
            return -1; // TODO
        }
#endif

        // --------

        unsafe int ISQLite3Provider.sqlite3_vfs_register(utf8z name, sqlite3_vfs top_vfs, int def)
        {
            var vfs = new native_sqlite3_vfs();
            vfs.iVersion = 2;
            vfs.szOsFile = Marshal.SizeOf<native_sqlite3_file>();
            vfs.mxPathname = top_vfs.maxPathname;
            {
                var len = name.AsSpan().Length;
                var ptr_name = Marshal.AllocHGlobal(len); // TODO free this
                var sp_name = new Span<byte>((void*)ptr_name, len);
                name.AsSpan().CopyTo(sp_name);
                vfs.zName = ptr_name;
            }

            vfs.xOpen = xOpen_bridge;
            vfs.xDelete = xDelete_bridge;
            vfs.xAccess = xAccess_bridge;
            vfs.xFullPathname = xFullPathname_bridge;

            vfs.xDlOpen = xDlOpen_bridge;
            vfs.xDlError = xDlError_bridge;
            vfs.xDlSym = xDlSym_bridge;
            vfs.xDlClose = xDlClose_bridge;

            vfs.xRandomness = xRandomness_bridge;
            vfs.xSleep = xSleep_bridge;
            vfs.xCurrentTime = xCurrentTime_bridge;
            vfs.xGetLastError = xGetLastError_bridge;

            vfs.xCurrentTimeInt64 = xCurrentTimeInt64_bridge;

            // TODO worry about whether this handle is enough to prevent GC
            var h = GCHandle.Alloc(top_vfs, GCHandleType.Normal);
            vfs.pAppData = GCHandle.ToIntPtr(h);

            var p = Marshal.AllocHGlobal(Marshal.SizeOf<native_sqlite3_vfs>());
            // TODO need to free this somewhere
            Marshal.StructureToPtr(vfs, p, false);

            return NativeMethods.sqlite3_vfs_register(p, def);
        }

	static class NativeMethods
	{
        private const string SQLITE_DLL = "e_sqlcipher";

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

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_key(sqlite3 db, byte* key, int keylen);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_key_v2(sqlite3 db, byte* dbname, byte* key, int keylen);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_rekey(sqlite3 db, byte* key, int keylen);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_rekey_v2(sqlite3 db, byte* dbname, byte* key, int keylen);

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
		public static extern unsafe int sqlite3_vfs_register(IntPtr p, int def);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern unsafe int sqlite3_vfs_unregister(IntPtr p);

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

    // --------
    // delegate types for vfs and io methods

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_vfs_xOpen(
        void* p_vfs,
        byte* psz_name,
        void* p_file,
        int flags,
        int* p_out_flags
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_vfs_xDelete(
        void* p_vfs,
        byte* psz_name,
        int flags
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_vfs_xAccess(
        void* p_vfs,
        byte* psz_name,
        int flags,
        int* p_out
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_vfs_xFullPathname(
        void* p_vfs,
        byte* psz_name,
        int nOut,
        byte* psz_out
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate void* delegate_vfs_xDlOpen(
        void* p_vfs,
        byte* psz_name
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate void delegate_vfs_xDlError(
        void* p_vfs,
        int nByte,
        byte* psz_errMsg
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate void* delegate_vfs_xDlSym(
        void* p_vfs,
        void* p_dl,
        byte* psz_name
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate void delegate_vfs_xDlClose(
        void* p_vfs,
        void* p_dl
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_vfs_xRandomness(
        void* p_vfs,
        int nByte,
        byte* psz_out
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_vfs_xSleep(
        void* p_vfs,
        int microseconds
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_vfs_xCurrentTime(
        void* p_vfs,
        double* p
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_vfs_xGetLastError(
        void* p_vfs,
        int nByte,
        byte* psz_out
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_vfs_xCurrentTimeInt64(
        void* p_vfs,
        long* p
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xClose(
        void* p_file
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xRead(
        void* p_file,
        void* p_buf,
        int iAmt,
        long iOfst
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xWrite(
        void* p_file,
        void* p_buf,
        int iAmt,
        long iOfst
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xTruncate(
        void* p_file,
        long size
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xSync(
        void* p_file,
        int flags
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xFileSize(
        void* p_file,
        long* p_size
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xLock(
        void* p_file,
        int x
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xUnlock(
        void* p_file,
        int x
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xCheckReservedLock(
        void* p_file,
        int* pResOut
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xFileControl(
        void* p_file,
        int op,
        void* pArg
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xSectorSize(
        void* p_file
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xDeviceCharacteristics(
        void* p_file
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xShmMap(
        void* p_file,
        int iPg,
        int pgsz,
        int x,
        void* p
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xShmLock(
        void* p_file,
        int offset,
        int n,
        int flags
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate void delegate_io_xShmBarrier(
        void* p_file
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xShmUnmap(
        void* p_file,
        int deleteFlag
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xFetch(
        void* p_file,
        long iOfst,
        int iAmt,
        void** p
        );

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public unsafe delegate int delegate_io_xUnfetch(
        void* p_file,
        long iOfst,
        void* p
        );

	}


    }
}
