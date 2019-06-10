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
    public sealed class SQLite3Provider_e_sqlite3 : ISQLite3Provider
    {
		const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;


        bool my_streq(IntPtr p, IntPtr q, int len)
        {
            return 0 == NativeMethods.sqlite3_strnicmp(p, q, len);
        }

        hook_handles get_hooks(sqlite3 db)
        {
			return db.GetOrCreateExtra<hook_handles>(() => new hook_handles(my_streq));
        }

        int ISQLite3Provider.sqlite3_win32_set_directory(int typ, IntPtr path)
        {
            return NativeMethods.sqlite3_win32_set_directory8((uint) typ, path);
        }

        int ISQLite3Provider.sqlite3_open(IntPtr filename, out IntPtr db)
        {
            return NativeMethods.sqlite3_open(filename, out db);
        }

        int ISQLite3Provider.sqlite3_open_v2(IntPtr filename, out IntPtr db, int flags, IntPtr vfs)
        {
            return NativeMethods.sqlite3_open_v2(filename, out db, flags, vfs);
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
			public delegate int SQLiteDeleteDelegate(IntPtr pVfs, IntPtr zName, int syncDir);
		}
		#pragma warning restore 649
		
		int ISQLite3Provider.sqlite3__vfs__delete(IntPtr vfs, IntPtr filename, int syncDir)
		{
			IntPtr ptrVfs = NativeMethods.sqlite3_vfs_find(vfs);
			// this code and the struct it uses was taken from aspnet/DataCommon.SQLite, Apache License 2.0
			sqlite3_vfs vstruct = (sqlite3_vfs) Marshal.PtrToStructure(ptrVfs, typeof(sqlite3_vfs));
			return vstruct.xDelete(ptrVfs, filename, 1);
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

        int ISQLite3Provider.sqlite3_exec(sqlite3 db, IntPtr sql, delegate_exec_low func, object user_data, out IntPtr errMsg)
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
			rc = NativeMethods.sqlite3_exec(db, sql, cb, h, out errMsg);
			h.Dispose();

            return rc;
        }

        int ISQLite3Provider.sqlite3_complete(IntPtr sql)
        {
            return NativeMethods.sqlite3_complete(sql);
        }

        IntPtr ISQLite3Provider.sqlite3_compileoption_get(int n)
        {
            return NativeMethods.sqlite3_compileoption_get(n);
        }

        int ISQLite3Provider.sqlite3_compileoption_used(IntPtr s)
        {
            return NativeMethods.sqlite3_compileoption_used(s);
        }

        int ISQLite3Provider.sqlite3_table_column_metadata(sqlite3 db, IntPtr dbName, IntPtr tblName, IntPtr colName, out IntPtr dataType, out IntPtr collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            return NativeMethods.sqlite3_table_column_metadata(
                        db, dbName, tblName, colName, 
                        out dataType, out collSeq, out notNull, out primaryKey, out autoInc);
        }

        int ISQLite3Provider.sqlite3_prepare_v2(sqlite3 db, IntPtr sql, out IntPtr stm, out IntPtr tail)
        {
            return NativeMethods.sqlite3_prepare_v2(db, sql, -1, out stm, out tail);
        }

        int ISQLite3Provider.sqlite3_prepare_v3(sqlite3 db, IntPtr sql, uint flags, out IntPtr stm, out IntPtr tail)
        {
            return NativeMethods.sqlite3_prepare_v3(db, sql, -1, flags, out stm, out tail);
        }

        int ISQLite3Provider.sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg)
        {
            return NativeMethods.sqlite3_db_status(db, op, out current, out highest, resetFlg);
        }

        IntPtr ISQLite3Provider.sqlite3_sql(sqlite3_stmt stmt)
        {
            return NativeMethods.sqlite3_sql(stmt);
        }

        IntPtr ISQLite3Provider.sqlite3_db_handle(IntPtr stmt)
        {
            return NativeMethods.sqlite3_db_handle(stmt);
        }

        int ISQLite3Provider.sqlite3_blob_open(sqlite3 db, byte[] db_utf8, byte[] table_utf8, byte[] col_utf8, long rowid, int flags, out sqlite3_blob blob)
        {
            return NativeMethods.sqlite3_blob_open(db, db_utf8, table_utf8, col_utf8, rowid, flags, out blob);
        }

        int ISQLite3Provider.sqlite3_blob_bytes(sqlite3_blob blob)
        {
            return NativeMethods.sqlite3_blob_bytes(blob);
        }

        int ISQLite3Provider.sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
        {
            return NativeMethods.sqlite3_blob_reopen(blob, rowid);
        }

        int ISQLite3Provider.sqlite3_blob_read(sqlite3_blob blob, byte[] b, int bOffset, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = NativeMethods.sqlite3_blob_read(blob, new IntPtr(ptr.ToInt64() + bOffset), n, offset);
            pinned.Free();
			return rc;
        }

        int ISQLite3Provider.sqlite3_blob_write(sqlite3_blob blob, byte[] b, int bOffset, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = NativeMethods.sqlite3_blob_write(blob, new IntPtr(ptr.ToInt64() + bOffset), n, offset);
            pinned.Free();
			return rc;
        }

        int ISQLite3Provider.sqlite3_blob_close(IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_close(blob);
        }

        sqlite3_backup ISQLite3Provider.sqlite3_backup_init(sqlite3 destDb, IntPtr destName, sqlite3 sourceDb, IntPtr sourceName)
        {
            return NativeMethods.sqlite3_backup_init(destDb, destName, sourceDb, sourceName);
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

        IntPtr ISQLite3Provider.sqlite3_errstr(int rc)
        {
            return NativeMethods.sqlite3_errstr(rc);
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

        int ISQLite3Provider.sqlite3_db_readonly(sqlite3 db, IntPtr dbName)
        {
            return NativeMethods.sqlite3_db_readonly(db, dbName); 
        }
        
        IntPtr ISQLite3Provider.sqlite3_db_filename(sqlite3 db, IntPtr att)
		{
            return NativeMethods.sqlite3_db_filename(db, att);
		}

        IntPtr ISQLite3Provider.sqlite3_errmsg(sqlite3 db)
        {
            return NativeMethods.sqlite3_errmsg(db);
        }

        IntPtr ISQLite3Provider.sqlite3_libversion()
        {
            return NativeMethods.sqlite3_libversion();
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

        int my_sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_scalar func)
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
			if (rc == 0)
			{
                info.AddScalarFunction(name, nargs, h.ForDispose());
			}
			return rc;
        }

        int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, object v, delegate_function_scalar func)
		{
			return my_sqlite3_create_function(db, name, nargs, 0, v, func);
		}

        int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_scalar func)
		{
			return my_sqlite3_create_function(db, name, nargs, flags, v, func);
		}

        // ----------------------------------------------------------------

		static IDisposable disp_log_hook_handle;

        [MonoPInvokeCallback (typeof(NativeMethods.callback_log))]
        static void log_hook_bridge_impl(IntPtr p, int rc, IntPtr s)
        {
            log_hook_info hi = log_hook_info.from_ptr(p);
            hi.call(rc, s);
        }

		readonly NativeMethods.callback_log log_hook_bridge = new NativeMethods.callback_log(log_hook_bridge_impl); 
        int ISQLite3Provider.sqlite3_config_log(delegate_log_low func, object v)
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

        int my_sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
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
			if (rc == 0)
			{
                info.AddAggFunction(name, nargs, h.ForDispose());
			}
			return rc;
        }

        int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
		{
			return my_sqlite3_create_function(db, name, nargs, 0, v, func_step, func_final);
		}

        int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
		{
			return my_sqlite3_create_function(db, name, nargs, flags, v, func_step, func_final);
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
        int ISQLite3Provider.sqlite3_create_collation(sqlite3 db, byte[] name, object v, delegate_collation_low func)
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
			if (rc == 0)
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
            hi.call(typ, db, tbl, rowid);
        }

		readonly NativeMethods.callback_update update_hook_bridge = new NativeMethods.callback_update(update_hook_bridge_impl); 
        void ISQLite3Provider.sqlite3_update_hook(sqlite3 db, delegate_update_low func, object v)
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

        [MonoPInvokeCallback (typeof(NativeMethods.callback_trace_v2))]
        static void trace_v2_hook_bridge_impl(uint t, IntPtr c, IntPtr p, IntPtr x)
        {
            trace_v2_hook_info hi = trace_v2_hook_info.from_ptr(c);
            hi.call(t, p, x);
        }

		readonly NativeMethods.callback_trace_v2 trace_v2_hook_bridge = new NativeMethods.callback_trace_v2(trace_v2_hook_bridge_impl); 
        int ISQLite3Provider.sqlite3_trace_v2(sqlite3 db, uint uMask, delegate_trace_v2 func, object v)
        {
			var info = get_hooks(db);
            if (info.trace_v2 != null)
            {
                // TODO maybe turn off the hook here, for now
                info.trace_v2.Dispose();
                info.trace_v2 = null;
            }

			NativeMethods.callback_trace_v2 cb;
			trace_v2_hook_info hi;
            if (func != null)
            {
				cb = trace_v2_hook_bridge;
                hi = new trace_v2_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.trace_v2 = h.ForDispose();
			return NativeMethods.sqlite3_trace_v2(db, uMask, cb, h);
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
            return hi.call(action_code, param0, param1, dbName, inner_most_trigger_or_view);
        }

        readonly NativeMethods.callback_authorizer authorizer_hook_bridge = new NativeMethods.callback_authorizer(authorizer_hook_bridge_impl);
        int ISQLite3Provider.sqlite3_set_authorizer(sqlite3 db, delegate_authorizer_low func, object v)
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

        IntPtr ISQLite3Provider.sqlite3_sourceid()
        {
            return NativeMethods.sqlite3_sourceid();
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

        void ISQLite3Provider.sqlite3_result_error(IntPtr ctx, IntPtr val)
        {
            NativeMethods.sqlite3_result_error(ctx, val, -1);
        }

        void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, IntPtr val)
        {
            NativeMethods.sqlite3_result_text(ctx, val, -1, new IntPtr(-1));
        }

        void ISQLite3Provider.sqlite3_result_blob(IntPtr ctx, byte[] blob)
        {
            NativeMethods.sqlite3_result_blob(ctx, blob, blob.Length, new IntPtr(-1));
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

        IntPtr ISQLite3Provider.sqlite3_value_text(IntPtr p)
        {
            return NativeMethods.sqlite3_value_text(p);
        }

        int ISQLite3Provider.sqlite3_bind_int(sqlite3_stmt stm, int paramIndex, int val)
        {
            return NativeMethods.sqlite3_bind_int(stm, paramIndex, val);
        }

        int ISQLite3Provider.sqlite3_bind_int64(sqlite3_stmt stm, int paramIndex, long val)
        {
            return NativeMethods.sqlite3_bind_int64(stm, paramIndex, val);
        }

        int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stm, int paramIndex, IntPtr t)
        {
            return NativeMethods.sqlite3_bind_text(stm, paramIndex, t, -1, new IntPtr(-1));
        }

        int ISQLite3Provider.sqlite3_bind_double(sqlite3_stmt stm, int paramIndex, double val)
        {
            return NativeMethods.sqlite3_bind_double(stm, paramIndex, val);
        }

        int ISQLite3Provider.sqlite3_bind_blob(sqlite3_stmt stm, int paramIndex, byte[] blob)
        {
            return NativeMethods.sqlite3_bind_blob(stm, paramIndex, blob, blob.Length, new IntPtr(-1));
        }

        int ISQLite3Provider.sqlite3_bind_blob(sqlite3_stmt stm, int paramIndex, byte[] blob, int nSize)
        {
            return NativeMethods.sqlite3_bind_blob(stm, paramIndex, blob, nSize, new IntPtr(-1));
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

        IntPtr ISQLite3Provider.sqlite3_bind_parameter_name(sqlite3_stmt stm, int paramIndex)
        {
            return NativeMethods.sqlite3_bind_parameter_name(stm, paramIndex);
        }

        int ISQLite3Provider.sqlite3_bind_parameter_index(sqlite3_stmt stm, IntPtr paramName)
        {
            return NativeMethods.sqlite3_bind_parameter_index(stm, paramName);
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

        ReadOnlySpan<byte> ISQLite3Provider.sqlite3_column_text(sqlite3_stmt stm, int columnIndex)
        {
            IntPtr p = NativeMethods.sqlite3_column_text(stm, columnIndex);
            if (p == IntPtr.Zero)
            {
                return null;
            }

            var length = NativeMethods.sqlite3_column_bytes(stm, columnIndex);
            unsafe
            {
                return new ReadOnlySpan<byte>(p.ToPointer(), length);
            }
        }

        IntPtr ISQLite3Provider.sqlite3_column_decltype(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_decltype(stm, columnIndex);
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

        IntPtr ISQLite3Provider.sqlite3_column_name(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_name(stm, columnIndex);
        }

        IntPtr ISQLite3Provider.sqlite3_column_origin_name(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_origin_name(stm, columnIndex);
        }

        IntPtr ISQLite3Provider.sqlite3_column_table_name(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_table_name(stm, columnIndex);
        }

        IntPtr ISQLite3Provider.sqlite3_column_database_name(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_database_name(stm, columnIndex);
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

        int ISQLite3Provider.sqlite3_wal_checkpoint(sqlite3 db, IntPtr dbName)
        {
            return NativeMethods.sqlite3_wal_checkpoint(db, dbName);
        }

        int ISQLite3Provider.sqlite3_wal_checkpoint_v2(sqlite3 db, IntPtr dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            return NativeMethods.sqlite3_wal_checkpoint_v2(db, dbName, eMode, out logSize, out framesCheckPointed);
        }

	static class NativeMethods
	{
        private const string SQLITE_DLL = "e_sqlite3";

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_close(IntPtr db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_close_v2(IntPtr db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_enable_shared_cache(int enable);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_interrupt(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_finalize(IntPtr stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_reset(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_clear_bindings(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_stmt_status(sqlite3_stmt stm, int op, int resetFlg);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_column_database_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_column_decltype(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_column_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_column_origin_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_column_table_name(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_column_text(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_errmsg(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_db_readonly(sqlite3 db, IntPtr dbName);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_db_filename(sqlite3 db, IntPtr att);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_prepare_v2(sqlite3 db, IntPtr pSql, int nBytes, out IntPtr stmt, out IntPtr ptrRemain);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_prepare_v3(sqlite3 db, IntPtr pSql, int nBytes, uint flags, out IntPtr stmt, out IntPtr ptrRemain);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_complete(IntPtr pSql);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_compileoption_used(IntPtr pSql);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_compileoption_get(int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_table_column_metadata(sqlite3 db, IntPtr dbName, IntPtr tblName, IntPtr colName, out IntPtr ptrDataType, out IntPtr ptrCollSeq, out int notNull, out int primaryKey, out int autoInc);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_value_text(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_enable_load_extension(
		sqlite3 db, int enable);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_load_extension(
		sqlite3 db, byte[] fileName, byte[] procName, ref IntPtr pError);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_initialize();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_shutdown();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_libversion();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_libversion_number();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_threadsafe();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_sourceid();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_malloc(int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_realloc(IntPtr p, int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_free(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_stricmp(IntPtr p, IntPtr q);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_strnicmp(IntPtr p, IntPtr q, int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_open(IntPtr filename, out IntPtr db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_open_v2(IntPtr filename, out IntPtr db, int flags, IntPtr vfs);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_vfs_find(IntPtr vfs);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern long sqlite3_last_insert_rowid(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_changes(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_total_changes(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern long sqlite3_memory_used();

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern long sqlite3_memory_highwater(int resetFlag);
		
		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_status(int op, out int current, out int highwater, int resetFlag);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_busy_timeout(sqlite3 db, int ms);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_bind_blob(sqlite3_stmt stmt, int index, byte[] val, int nSize, IntPtr nTransient);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_bind_double(sqlite3_stmt stmt, int index, double val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_bind_int(sqlite3_stmt stmt, int index, int val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_bind_null(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_bind_text(sqlite3_stmt stmt, int index, IntPtr val, int nlen, IntPtr pvReserved);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_bind_parameter_count(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_bind_parameter_index(sqlite3_stmt stmt, IntPtr strName);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_column_count(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_data_count(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_step(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_sql(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern double sqlite3_column_double(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_column_int(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern long sqlite3_column_int64(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_column_blob(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_column_bytes(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_column_type(sqlite3_stmt stmt, int index);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_aggregate_count(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_value_blob(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_value_bytes(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern double sqlite3_value_double(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_value_int(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern long sqlite3_value_int64(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_value_type(IntPtr p);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_user_data(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_blob(IntPtr context, byte[] val, int nSize, IntPtr pvReserved);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_double(IntPtr context, double val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_error(IntPtr context, IntPtr strErr, int nLen);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_int(IntPtr context, int val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_int64(IntPtr context, long val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_null(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_text(IntPtr context, IntPtr val, int nLen, IntPtr pvReserved);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_zeroblob(IntPtr context, int n);

		// TODO sqlite3_result_value 

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_error_toobig(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_error_nomem(IntPtr context);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_result_error_code(IntPtr context, int code);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_key(sqlite3 db, byte[] key, int keylen);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_rekey(sqlite3 db, byte[] key, int keylen);

		// Since sqlite3_config() takes a variable argument list, we have to overload declarations
		// for all possible calls that we want to use.
		[DllImport(SQLITE_DLL, ExactSpelling=true, EntryPoint = "sqlite3_config", CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_config_none(int op);

		[DllImport(SQLITE_DLL, ExactSpelling=true, EntryPoint = "sqlite3_config", CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_config_int(int op, int val);

		[DllImport(SQLITE_DLL, ExactSpelling=true, EntryPoint = "sqlite3_config", CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_config_log(int op, NativeMethods.callback_log func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_create_collation(sqlite3 db, byte[] strName, int nType, hook_handle pvUser, NativeMethods.callback_collation func);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_update_hook(sqlite3 db, NativeMethods.callback_update func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_commit_hook(sqlite3 db, NativeMethods.callback_commit func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_progress_handler(sqlite3 db, int instructions, NativeMethods.callback_progress_handler func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_trace_v2(sqlite3 db, uint uMask, NativeMethods.callback_trace_v2 func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_rollback_hook(sqlite3 db, NativeMethods.callback_rollback func, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_db_handle(IntPtr stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_next_stmt(sqlite3 db, IntPtr stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_stmt_busy(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_stmt_readonly(sqlite3_stmt stmt);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_exec(sqlite3 db, IntPtr strSql, NativeMethods.callback_exec cb, hook_handle pvParam, out IntPtr errMsg);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_get_autocommit(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_extended_result_codes(sqlite3 db, int onoff);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_errcode(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_extended_errcode(sqlite3 db);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern IntPtr sqlite3_errstr(int rc); /* 3.7.15+ */

		// Since sqlite3_log() takes a variable argument list, we have to overload declarations
		// for all possible calls.  For now, we are only exposing a single string, and 
		// depend on the caller to format the string.
		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern void sqlite3_log(int iErrCode, byte[] zFormat);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_file_control(sqlite3 db, byte[] zDbName, int op, IntPtr pArg);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern sqlite3_backup sqlite3_backup_init(sqlite3 destDb, IntPtr zDestName, sqlite3 sourceDb, IntPtr zSourceName);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_backup_step(sqlite3_backup backup, int nPage);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_backup_remaining(sqlite3_backup backup);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_backup_pagecount(sqlite3_backup backup);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_backup_finish(IntPtr backup);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_blob_open(sqlite3 db, byte[] sdb, byte[] table, byte[] col, long rowid, int flags, out sqlite3_blob blob);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_blob_write(sqlite3_blob blob, IntPtr b, int n, int offset);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_blob_read(sqlite3_blob blob, IntPtr b, int n, int offset);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_blob_bytes(sqlite3_blob blob);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_blob_reopen(sqlite3_blob blob, long rowid);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_blob_close(IntPtr blob);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_wal_autocheckpoint(sqlite3 db, int n);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_wal_checkpoint(sqlite3 db, IntPtr dbName);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_wal_checkpoint_v2(sqlite3 db, IntPtr dbName, int eMode, out int logSize, out int framesCheckPointed);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_set_authorizer(sqlite3 db, NativeMethods.callback_authorizer cb, hook_handle pvUser);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION, CharSet=CharSet.Unicode)]
		public static extern int sqlite3_win32_set_directory8(uint directoryType, IntPtr directoryPath);

		[DllImport(SQLITE_DLL, ExactSpelling=true, CallingConvention = CALLING_CONVENTION)]
		public static extern int sqlite3_create_function_v2(sqlite3 db, byte[] strName, int nArgs, int nType, hook_handle pvUser, NativeMethods.callback_scalar_function func, NativeMethods.callback_agg_function_step fstep, NativeMethods.callback_agg_function_final ffinal, NativeMethods.callback_destroy fdestroy);


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
	public delegate int callback_progress_handler(IntPtr puser);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate int callback_authorizer(IntPtr puser, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_trace_v2(uint t, IntPtr puser, IntPtr p, IntPtr x);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate void callback_rollback(IntPtr puser);

	[UnmanagedFunctionPointer(CALLING_CONVENTION)]
	public delegate int callback_exec(IntPtr db, int n, IntPtr values, IntPtr names);
	}


    }
}
