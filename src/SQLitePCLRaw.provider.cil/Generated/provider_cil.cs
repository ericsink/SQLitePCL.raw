/*
   Copyright 2014-2021 SourceGear, LLC

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
	using System.Text;

	[Preserve(AllMembers = true)]
    public sealed class SQLite3Provider_cil : ISQLite3Provider
    {
		const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

        string ISQLite3Provider.GetNativeLibraryName()
        {
            return "e_sqlite3";
        }

        unsafe bool my_streq(IntPtr p, IntPtr q, int len)
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

        unsafe int ISQLite3Provider.sqlite3_close_v2(IntPtr db)
        {
            var rc = NativeMethods.sqlite3_close_v2(db);
			return rc;
        }

        unsafe int ISQLite3Provider.sqlite3_close(IntPtr db)
        {
            var rc = NativeMethods.sqlite3_close(db);
			return rc;
        }

        unsafe void ISQLite3Provider.sqlite3_free(IntPtr p)
        {
            NativeMethods.sqlite3_free(p);
        }

        unsafe int ISQLite3Provider.sqlite3_stricmp(IntPtr p, IntPtr q)
        {
            return NativeMethods.sqlite3_stricmp(p, q);
        }

        unsafe int ISQLite3Provider.sqlite3_strnicmp(IntPtr p, IntPtr q, int n)
        {
            return NativeMethods.sqlite3_strnicmp(p, q, n);
        }

        unsafe int ISQLite3Provider.sqlite3_enable_shared_cache(int enable)
        {
            return NativeMethods.sqlite3_enable_shared_cache(enable);
        }

        unsafe void ISQLite3Provider.sqlite3_interrupt(sqlite3 db)
        {
            NativeMethods.sqlite3_interrupt(db);
        }

        
        static int exec_hook_bridge_impl(IntPtr p, int n, IntPtr values_ptr, IntPtr names_ptr)
        {
            exec_hook_info hi = exec_hook_info.from_ptr(p);
            return hi.call(n, values_ptr, names_ptr);
        }
		// shouldn't there be a impl/bridge thing here?  no, because this callback is not stored so it doesn't need further GC protection

        unsafe int ISQLite3Provider.sqlite3_exec(sqlite3 db, utf8z sql, delegate_exec func, object user_data, out IntPtr errMsg)
        {
            int rc;

			delegate* <IntPtr, int, IntPtr, IntPtr, int> cb;
			exec_hook_info hi;
            if (func != null)
            {
				cb = &exec_hook_bridge_impl;
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
                    rc = NativeMethods.sqlite3_exec(db, p_sql, (IntPtr) cb, h, out errMsg);
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

        unsafe IntPtr ISQLite3Provider.sqlite3_db_handle(IntPtr stmt)
        {
            return NativeMethods.sqlite3_db_handle(stmt);
        }

        unsafe int ISQLite3Provider.sqlite3_blob_open(sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z col_utf8, long rowid, int flags, out IntPtr blob)
        {
            fixed (byte* p_db = db_utf8, p_table = table_utf8, p_col = col_utf8)
            {
                return NativeMethods.sqlite3_blob_open(db, p_db, p_table, p_col, rowid, flags, out blob);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_blob_bytes(sqlite3_blob blob)
        {
            return NativeMethods.sqlite3_blob_bytes(blob);
        }

        unsafe int ISQLite3Provider.sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
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

        unsafe int ISQLite3Provider.sqlite3_blob_close(IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_close(blob);
        }

        unsafe int ISQLite3Provider.sqlite3_snapshot_get(sqlite3 db, utf8z schema, out IntPtr snap)
        {
            fixed (byte* p_schema = schema)
            {
                return NativeMethods.sqlite3_snapshot_get(db, p_schema, out snap);
            }
        }

        int ISQLite3Provider.sqlite3_snapshot_cmp(sqlite3_snapshot p1, sqlite3_snapshot p2)
        {
            return NativeMethods.sqlite3_snapshot_cmp(p1, p2);
        }

        unsafe int ISQLite3Provider.sqlite3_snapshot_open(sqlite3 db, utf8z schema, sqlite3_snapshot snap)
        {
            fixed (byte* p_schema = schema)
            {
                return NativeMethods.sqlite3_snapshot_open(db, p_schema, snap);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_snapshot_recover(sqlite3 db, utf8z name)
        {
            fixed (byte* p_name = name)
            {
                return NativeMethods.sqlite3_snapshot_recover(db, p_name);
            }
        }

        void ISQLite3Provider.sqlite3_snapshot_free(IntPtr snap)
        {
            NativeMethods.sqlite3_snapshot_free(snap);
        }

        unsafe sqlite3_backup ISQLite3Provider.sqlite3_backup_init(sqlite3 destDb, utf8z destName, sqlite3 sourceDb, utf8z sourceName)
        {
            fixed (byte* p_destName = destName, p_sourceName = sourceName)
            {
                return NativeMethods.sqlite3_backup_init(destDb, p_destName, sourceDb, p_sourceName);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_backup_step(sqlite3_backup backup, int nPage)
        {
            return NativeMethods.sqlite3_backup_step(backup, nPage);
        }

        unsafe int ISQLite3Provider.sqlite3_backup_remaining(sqlite3_backup backup)
        {
            return NativeMethods.sqlite3_backup_remaining(backup);
        }

        unsafe int ISQLite3Provider.sqlite3_backup_pagecount(sqlite3_backup backup)
        {
            return NativeMethods.sqlite3_backup_pagecount(backup);
        }

        unsafe int ISQLite3Provider.sqlite3_backup_finish(IntPtr backup)
        {
            return NativeMethods.sqlite3_backup_finish(backup);
        }

        unsafe IntPtr ISQLite3Provider.sqlite3_next_stmt(sqlite3 db, IntPtr stmt)
        {
            return NativeMethods.sqlite3_next_stmt(db, stmt);
        }

        unsafe long ISQLite3Provider.sqlite3_last_insert_rowid(sqlite3 db)
        {
            return NativeMethods.sqlite3_last_insert_rowid(db);
        }

        unsafe int ISQLite3Provider.sqlite3_changes(sqlite3 db)
        {
            return NativeMethods.sqlite3_changes(db);
        }

        unsafe int ISQLite3Provider.sqlite3_total_changes(sqlite3 db)
        {
            return NativeMethods.sqlite3_total_changes(db);
        }

        unsafe int ISQLite3Provider.sqlite3_extended_result_codes(sqlite3 db, int onoff)
        {
            return NativeMethods.sqlite3_extended_result_codes(db, onoff);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_errstr(int rc)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_errstr(rc));
        }

        unsafe int ISQLite3Provider.sqlite3_errcode(sqlite3 db)
        {
            return NativeMethods.sqlite3_errcode(db);
        }

        unsafe int ISQLite3Provider.sqlite3_extended_errcode(sqlite3 db)
        {
            return NativeMethods.sqlite3_extended_errcode(db);
        }

        unsafe int ISQLite3Provider.sqlite3_busy_timeout(sqlite3 db, int ms)
        {
            return NativeMethods.sqlite3_busy_timeout(db, ms);
        }

        unsafe int ISQLite3Provider.sqlite3_get_autocommit(sqlite3 db)
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

        unsafe int ISQLite3Provider.sqlite3_libversion_number()
        {
            return NativeMethods.sqlite3_libversion_number();
        }

        unsafe int ISQLite3Provider.sqlite3_threadsafe()
        {
            return NativeMethods.sqlite3_threadsafe();
        }

        unsafe int ISQLite3Provider.sqlite3_config(int op)
        {
            return NativeMethods.sqlite3_config_none(op);
        }

        unsafe int ISQLite3Provider.sqlite3_config(int op, int val)
        {
            return NativeMethods.sqlite3_config_int(op, val);
        }

        unsafe int ISQLite3Provider.sqlite3_db_config(sqlite3 db, int op, utf8z val)
        {
            fixed (byte* p_val = val)
            {
                return NativeMethods.sqlite3_db_config_charptr(db, op, p_val);
            }
        }

        unsafe int ISQLite3Provider.sqlite3_db_config(sqlite3 db, int op, int val, out int result)
        {
            int out_result = 0;
            int native_result = NativeMethods.sqlite3_db_config_int_outint(db, op, val, &out_result);

            result = out_result;

            return native_result;
        }

        unsafe  int ISQLite3Provider.sqlite3_db_config(sqlite3 db, int op, IntPtr ptr, int int0, int int1)
        {
            return NativeMethods.sqlite3_db_config_intptr_int_int(db, op, ptr, int0, int1);
        }

        int ISQLite3Provider.sqlite3_limit(sqlite3 db, int id, int newVal)
        {
            return NativeMethods.sqlite3_limit(db, id, newVal);
        }

        unsafe int ISQLite3Provider.sqlite3_initialize()
        {
            return NativeMethods.sqlite3_initialize();
        }

        unsafe int ISQLite3Provider.sqlite3_shutdown()
        {
            return NativeMethods.sqlite3_shutdown();
        }

        unsafe int ISQLite3Provider.sqlite3_enable_load_extension(sqlite3 db, int onoff)
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
        
        
        static int commit_hook_bridge_impl(IntPtr p)
        {
            commit_hook_info hi = commit_hook_info.from_ptr(p);
            return hi.call();
        }

        
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
			NativeMethods.sqlite3_commit_hook(db, (IntPtr) cb, h);
			info.commit = h.ForDispose();
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        
        unsafe static void scalar_function_hook_bridge_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr p = NativeMethods.sqlite3_user_data(context);
            function_hook_info hi = function_hook_info.from_ptr(p);
            hi.call_scalar(context, num_args, argsptr);
        }

        

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
			int rc = NativeMethods.sqlite3_create_function_v2(db, name, nargs, arg4, h, (IntPtr) cb, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if ((rc == 0) && (cb != null))
			{
                info.AddScalarFunction(name, nargs, h.ForDispose());
			}
			return rc;
        }

        // ----------------------------------------------------------------

		static IDisposable disp_log_hook_handle;

        
        static void log_hook_bridge_impl(IntPtr p, int rc, IntPtr s)
        {
            log_hook_info hi = log_hook_info.from_ptr(p);
            hi.call(rc, utf8z.FromIntPtr(s));
        }

        
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
			var rc = NativeMethods.sqlite3_config_log(raw.SQLITE_CONFIG_LOG, (IntPtr) cb, h);
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

        
        unsafe static void agg_function_step_hook_bridge_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            function_hook_info hi = function_hook_info.from_ptr(p);
            hi.call_step(context, agg, num_args, argsptr);
        }

        
        unsafe static void agg_function_final_hook_bridge_impl(IntPtr context)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            function_hook_info hi = function_hook_info.from_ptr(p);
            hi.call_final(context, agg);
        }

        
        

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
				cb_step = &agg_function_step_hook_bridge_impl;
				cb_final = &agg_function_final_hook_bridge_impl;
                hi = new function_hook_info(func_step, func_final, v);
            }
            else
            {
				cb_step = null;
				cb_final = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			int rc = NativeMethods.sqlite3_create_function_v2(db, name, nargs, arg4, h, IntPtr.Zero, (IntPtr) cb_step, (IntPtr) cb_final, IntPtr.Zero);
			if ((rc == 0) && (cb_step != null))
			{
                info.AddAggFunction(name, nargs, h.ForDispose());
			}
			return rc;
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        
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
			int rc = NativeMethods.sqlite3_create_collation(db, name, 1, h, (IntPtr) cb);
			if ((rc == 0) && (cb != null))
			{
                info.AddCollation(name, h.ForDispose());
			}
			return rc;
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        
        static void update_hook_bridge_impl(IntPtr p, int typ, IntPtr db, IntPtr tbl, Int64 rowid)
        {
            update_hook_info hi = update_hook_info.from_ptr(p);
            hi.call(typ, utf8z.FromIntPtr(db), utf8z.FromIntPtr(tbl), rowid);
        }

        
        unsafe void ISQLite3Provider.sqlite3_update_hook(sqlite3 db, delegate_update func, object v)
        {
			var info = get_hooks(db);
            if (info.update != null)
            {
                // TODO maybe turn off the hook here, for now
                info.update.Dispose();
                info.update = null;
            }

			delegate* <IntPtr, int, IntPtr, IntPtr, Int64, void> cb;
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
			NativeMethods.sqlite3_update_hook(db, (IntPtr) cb, h);
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        
        static void rollback_hook_bridge_impl(IntPtr p)
        {
            rollback_hook_info hi = rollback_hook_info.from_ptr(p);
            hi.call();
        }

        
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
			NativeMethods.sqlite3_rollback_hook(db, (IntPtr) cb, h);
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        
        static void trace_hook_bridge_impl(IntPtr p, IntPtr s)
        {
            trace_hook_info hi = trace_hook_info.from_ptr(p);
            hi.call(utf8z.FromIntPtr(s));
        }

        
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
			NativeMethods.sqlite3_trace(db, (IntPtr) cb, h);
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        
        static void profile_hook_bridge_impl(IntPtr p, IntPtr s, long elapsed)
        {
            profile_hook_info hi = profile_hook_info.from_ptr(p);
            hi.call(utf8z.FromIntPtr(s), elapsed);
        }

        
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
			NativeMethods.sqlite3_profile(db, (IntPtr) cb, h);
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        
        static int progress_handler_hook_bridge_impl(IntPtr p)
        {
            progress_hook_info hi = progress_hook_info.from_ptr(p);
            return hi.call();
        }

        
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
				cb = &progress_handler_hook_bridge_impl;
                hi = new progress_hook_info(func, v);
            }
            else
            {
				cb = null;
				hi = null;
            }
			var h = new hook_handle(hi);
			info.progress = h.ForDispose();
			NativeMethods.sqlite3_progress_handler(db, instructions, (IntPtr) cb, h);
        }

        // ----------------------------------------------------------------

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        
        static int authorizer_hook_bridge_impl(IntPtr p, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view)
        {
            authorizer_hook_info hi = authorizer_hook_info.from_ptr(p);
            return hi.call(action_code, utf8z.FromIntPtr(param0), utf8z.FromIntPtr(param1), utf8z.FromIntPtr(dbName), utf8z.FromIntPtr(inner_most_trigger_or_view));
        }

        
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
			return NativeMethods.sqlite3_set_authorizer(db, (IntPtr) cb, h);
        }

        // ----------------------------------------------------------------

        unsafe long ISQLite3Provider.sqlite3_memory_used()
        {
            return NativeMethods.sqlite3_memory_used();
        }

        unsafe long ISQLite3Provider.sqlite3_memory_highwater(int resetFlag)
        {
            return NativeMethods.sqlite3_memory_highwater(resetFlag);
        }

        long ISQLite3Provider.sqlite3_soft_heap_limit64(long n)
        {
            return NativeMethods.sqlite3_soft_heap_limit64(n);
        }
        
        long ISQLite3Provider.sqlite3_hard_heap_limit64(long n)
        {
            return NativeMethods.sqlite3_hard_heap_limit64(n);
        }

        int ISQLite3Provider.sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            return NativeMethods.sqlite3_status(op, out current, out highwater, resetFlag);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_sourceid()
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_sourceid());
        }

        unsafe void ISQLite3Provider.sqlite3_result_int64(IntPtr ctx, long val)
        {
            NativeMethods.sqlite3_result_int64(ctx, val);
        }

        unsafe void ISQLite3Provider.sqlite3_result_int(IntPtr ctx, int val)
        {
            NativeMethods.sqlite3_result_int(ctx, val);
        }

        unsafe void ISQLite3Provider.sqlite3_result_double(IntPtr ctx, double val)
        {
            NativeMethods.sqlite3_result_double(ctx, val);
        }

        unsafe void ISQLite3Provider.sqlite3_result_null(IntPtr stm)
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

        unsafe void ISQLite3Provider.sqlite3_result_zeroblob(IntPtr ctx, int n)
        {
            NativeMethods.sqlite3_result_zeroblob(ctx, n);
        }

        // TODO sqlite3_result_value

        unsafe void ISQLite3Provider.sqlite3_result_error_toobig(IntPtr ctx)
        {
            NativeMethods.sqlite3_result_error_toobig(ctx);
        }

        unsafe void ISQLite3Provider.sqlite3_result_error_nomem(IntPtr ctx)
        {
            NativeMethods.sqlite3_result_error_nomem(ctx);
        }

        unsafe void ISQLite3Provider.sqlite3_result_error_code(IntPtr ctx, int code)
        {
            NativeMethods.sqlite3_result_error_code(ctx, code);
        }

        unsafe ReadOnlySpan<byte> ISQLite3Provider.sqlite3_value_blob(IntPtr p)
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

        unsafe int ISQLite3Provider.sqlite3_value_bytes(IntPtr p)
        {
            return NativeMethods.sqlite3_value_bytes(p);
        }

        unsafe double ISQLite3Provider.sqlite3_value_double(IntPtr p)
        {
            return NativeMethods.sqlite3_value_double(p);
        }

        unsafe int ISQLite3Provider.sqlite3_value_int(IntPtr p)
        {
            return NativeMethods.sqlite3_value_int(p);
        }

        unsafe long ISQLite3Provider.sqlite3_value_int64(IntPtr p)
        {
            return NativeMethods.sqlite3_value_int64(p);
        }

        unsafe int ISQLite3Provider.sqlite3_value_type(IntPtr p)
        {
            return NativeMethods.sqlite3_value_type(p);
        }

        unsafe utf8z ISQLite3Provider.sqlite3_value_text(IntPtr p)
        {
            return utf8z.FromPtr(NativeMethods.sqlite3_value_text(p));
        }

        unsafe int ISQLite3Provider.sqlite3_bind_int(sqlite3_stmt stm, int paramIndex, int val)
        {
            return NativeMethods.sqlite3_bind_int(stm, paramIndex, val);
        }

        unsafe int ISQLite3Provider.sqlite3_bind_int64(sqlite3_stmt stm, int paramIndex, long val)
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

        unsafe int ISQLite3Provider.sqlite3_bind_text16(sqlite3_stmt stm, int paramIndex, ReadOnlySpan<char> t)
        {
            fixed (char* p_t = t)
            {
                // mul span length times 2 to get num bytes, which is what sqlite wants
                return NativeMethods.sqlite3_bind_text16(stm, paramIndex, p_t, t.Length * 2, new IntPtr(-1));
            }
        }

        unsafe int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stm, int paramIndex, utf8z t)
        {
            fixed (byte* p_t = t)
            {
                return NativeMethods.sqlite3_bind_text(stm, paramIndex, p_t, -1, new IntPtr(-1));
            }
        }

        unsafe int ISQLite3Provider.sqlite3_bind_double(sqlite3_stmt stm, int paramIndex, double val)
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

        unsafe int ISQLite3Provider.sqlite3_bind_zeroblob(sqlite3_stmt stm, int paramIndex, int size)
        {
            return NativeMethods.sqlite3_bind_zeroblob(stm, paramIndex, size);
        }

        unsafe int ISQLite3Provider.sqlite3_bind_null(sqlite3_stmt stm, int paramIndex)
        {
            return NativeMethods.sqlite3_bind_null(stm, paramIndex);
        }

        unsafe int ISQLite3Provider.sqlite3_bind_parameter_count(sqlite3_stmt stm)
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

        unsafe int ISQLite3Provider.sqlite3_step(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_step(stm);
        }

        unsafe int ISQLite3Provider.sqlite3_stmt_isexplain(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_stmt_isexplain(stm);
        }

        unsafe int ISQLite3Provider.sqlite3_stmt_busy(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_stmt_busy(stm);
        }

        unsafe int ISQLite3Provider.sqlite3_stmt_readonly(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_stmt_readonly(stm);
        }

        unsafe int ISQLite3Provider.sqlite3_column_int(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_int(stm, columnIndex);
        }

        unsafe long ISQLite3Provider.sqlite3_column_int64(sqlite3_stmt stm, int columnIndex)
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

        unsafe double ISQLite3Provider.sqlite3_column_double(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_double(stm, columnIndex);
        }

        unsafe ReadOnlySpan<byte> ISQLite3Provider.sqlite3_column_blob(sqlite3_stmt stm, int columnIndex)
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

        unsafe int ISQLite3Provider.sqlite3_column_type(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_type(stm, columnIndex);
        }

        unsafe int ISQLite3Provider.sqlite3_column_bytes(sqlite3_stmt stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_bytes(stm, columnIndex);
        }

        unsafe int ISQLite3Provider.sqlite3_column_count(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_column_count(stm);
        }

        unsafe int ISQLite3Provider.sqlite3_data_count(sqlite3_stmt stm)
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

        unsafe int ISQLite3Provider.sqlite3_reset(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_reset(stm);
        }

        unsafe int ISQLite3Provider.sqlite3_clear_bindings(sqlite3_stmt stm)
        {
            return NativeMethods.sqlite3_clear_bindings(stm);
        }

        unsafe int ISQLite3Provider.sqlite3_stmt_status(sqlite3_stmt stm, int op, int resetFlg)
        {
            return NativeMethods.sqlite3_stmt_status(stm, op, resetFlg);
        }

        unsafe int ISQLite3Provider.sqlite3_finalize(IntPtr stm)
        {
            return NativeMethods.sqlite3_finalize(stm);
        }

        unsafe int ISQLite3Provider.sqlite3_wal_autocheckpoint(sqlite3 db, int n)
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

		unsafe int ISQLite3Provider.sqlite3_keyword_count()
		{
			return NativeMethods.sqlite3_keyword_count();
		}

		unsafe int ISQLite3Provider.sqlite3_keyword_name(int i, out string name)
		{
			var rc = NativeMethods.sqlite3_keyword_name(i, out var p_name, out var length);

			// p_name is NOT null-terminated
			name = Encoding.UTF8.GetString(p_name, length);
			return rc;
		}

	static class NativeMethods
	{
		public unsafe static int sqlite3_close(IntPtr db)
        {
            var ret =
            foo.sqlite3_close(db);
            return (int) ret;
        }

		public unsafe static int sqlite3_close_v2(IntPtr db)
        {
            var ret =
            foo.sqlite3_close_v2(db);
            return (int) ret;
        }

		public unsafe static int sqlite3_enable_shared_cache(int enable)
        {
            var ret =
            foo.sqlite3_enable_shared_cache(enable);
            return (int) ret;
        }

		public unsafe static void sqlite3_interrupt(sqlite3 db)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                foo.sqlite3_interrupt(db.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
            }

		public unsafe static int sqlite3_finalize(IntPtr stmt)
        {
            var ret =
            foo.sqlite3_finalize(stmt);
            return (int) ret;
        }

		public unsafe static int sqlite3_reset(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_reset(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_clear_bindings(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_clear_bindings(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_stmt_status(sqlite3_stmt stm, int op, int resetFlg)
        {
                bool got_stm = false;
                stm.DangerousAddRef(ref got_stm);
                if (!got_stm) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_stmt_status(stm.DangerousGetHandle(), op, resetFlg);
                if (got_stm)
                {
                    stm.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static byte* sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_parameter_name(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static byte* sqlite3_column_database_name(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_database_name(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static byte* sqlite3_column_decltype(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_decltype(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static byte* sqlite3_column_name(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_name(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static byte* sqlite3_column_origin_name(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_origin_name(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static byte* sqlite3_column_table_name(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_table_name(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static byte* sqlite3_column_text(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_text(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static byte* sqlite3_errmsg(sqlite3 db)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_errmsg(db.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static int sqlite3_db_readonly(sqlite3 db, byte* dbName)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_db_readonly(db.DangerousGetHandle(), (IntPtr)dbName);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static byte* sqlite3_db_filename(sqlite3 db, byte* att)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_db_filename(db.DangerousGetHandle(), (IntPtr)att);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static int sqlite3_prepare_v2(sqlite3 db, byte* pSql, int nBytes, out IntPtr stmt, out byte* ptrRemain)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    IntPtr tmp_stmt;
                    byte* tmp_ptrRemain;
                var ret =
            foo.sqlite3_prepare_v2(db.DangerousGetHandle(), (IntPtr)pSql, nBytes, (IntPtr) (&tmp_stmt), (IntPtr) (&tmp_ptrRemain));
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    stmt = tmp_stmt;
                    ptrRemain = tmp_ptrRemain;
                return (int) ret;
        }

		public unsafe static int sqlite3_prepare_v3(sqlite3 db, byte* pSql, int nBytes, uint flags, out IntPtr stmt, out byte* ptrRemain)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    IntPtr tmp_stmt;
                    byte* tmp_ptrRemain;
                var ret =
            foo.sqlite3_prepare_v3(db.DangerousGetHandle(), (IntPtr)pSql, nBytes, (int)flags, (IntPtr) (&tmp_stmt), (IntPtr) (&tmp_ptrRemain));
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    stmt = tmp_stmt;
                    ptrRemain = tmp_ptrRemain;
                return (int) ret;
        }

		public unsafe static int sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    int tmp_current;
                    int tmp_highest;
                var ret =
            foo.sqlite3_db_status(db.DangerousGetHandle(), op, (IntPtr) (&tmp_current), (IntPtr) (&tmp_highest), resetFlg);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    current = tmp_current;
                    highest = tmp_highest;
                return (int) ret;
        }

		public unsafe static int sqlite3_complete(byte* pSql)
        {
            var ret =
            foo.sqlite3_complete((IntPtr)pSql);
            return (int) ret;
        }

		public unsafe static int sqlite3_compileoption_used(byte* pSql)
        {
            var ret =
            foo.sqlite3_compileoption_used((IntPtr)pSql);
            return (int) ret;
        }

		public unsafe static byte* sqlite3_compileoption_get(int n)
        {
            var ret =
            foo.sqlite3_compileoption_get(n);
            return (byte*) ret;
        }

		public unsafe static int sqlite3_table_column_metadata(sqlite3 db, byte* dbName, byte* tblName, byte* colName, out byte* ptrDataType, out byte* ptrCollSeq, out int notNull, out int primaryKey, out int autoInc)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    byte* tmp_ptrDataType;
                    byte* tmp_ptrCollSeq;
                    int tmp_notNull;
                    int tmp_primaryKey;
                    int tmp_autoInc;
                var ret =
            foo.sqlite3_table_column_metadata(db.DangerousGetHandle(), (IntPtr)dbName, (IntPtr)tblName, (IntPtr)colName, (IntPtr) (&tmp_ptrDataType), (IntPtr) (&tmp_ptrCollSeq), (IntPtr) (&tmp_notNull), (IntPtr) (&tmp_primaryKey), (IntPtr) (&tmp_autoInc));
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    ptrDataType = tmp_ptrDataType;
                    ptrCollSeq = tmp_ptrCollSeq;
                    notNull = tmp_notNull;
                    primaryKey = tmp_primaryKey;
                    autoInc = tmp_autoInc;
                return (int) ret;
        }

		public unsafe static byte* sqlite3_value_text(IntPtr p)
        {
            var ret =
            foo.sqlite3_value_text(p);
            return (byte*) ret;
        }

		public unsafe static int sqlite3_enable_load_extension(sqlite3 db, int enable)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_enable_load_extension(db.DangerousGetHandle(), enable);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_limit(sqlite3 db, int id, int newVal)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_limit(db.DangerousGetHandle(), id, newVal);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_initialize()
        {
            var ret =
            foo.sqlite3_initialize();
            return (int) ret;
        }

		public unsafe static int sqlite3_shutdown()
        {
            var ret =
            foo.sqlite3_shutdown();
            return (int) ret;
        }

		public unsafe static byte* sqlite3_libversion()
        {
            var ret =
            foo.sqlite3_libversion();
            return (byte*) ret;
        }

		public unsafe static int sqlite3_libversion_number()
        {
            var ret =
            foo.sqlite3_libversion_number();
            return (int) ret;
        }

		public unsafe static int sqlite3_threadsafe()
        {
            var ret =
            foo.sqlite3_threadsafe();
            return (int) ret;
        }

		public unsafe static byte* sqlite3_sourceid()
        {
            var ret =
            foo.sqlite3_sourceid();
            return (byte*) ret;
        }

		public unsafe static IntPtr sqlite3_malloc(int n)
        {
            var ret =
            foo.sqlite3_malloc(n);
            return (IntPtr) ret;
        }

		public unsafe static IntPtr sqlite3_realloc(IntPtr p, int n)
        {
            var ret =
            foo.sqlite3_realloc(p, n);
            return (IntPtr) ret;
        }

		public unsafe static void sqlite3_free(IntPtr p)
        {
            foo.sqlite3_free(p);
        }

		public unsafe static int sqlite3_stricmp(IntPtr p, IntPtr q)
        {
            var ret =
            foo.sqlite3_stricmp(p, q);
            return (int) ret;
        }

		public unsafe static int sqlite3_strnicmp(IntPtr p, IntPtr q, int n)
        {
            var ret =
            foo.sqlite3_strnicmp(p, q, n);
            return (int) ret;
        }

		public unsafe static int sqlite3_open(byte* filename, out IntPtr db)
        {
                IntPtr tmp_db;
                var ret =
            foo.sqlite3_open((IntPtr)filename, (IntPtr) (&tmp_db));
                db = tmp_db;
                return (int) ret;
        }

		public unsafe static int sqlite3_open_v2(byte* filename, out IntPtr db, int flags, byte* vfs)
        {
                IntPtr tmp_db;
                var ret =
            foo.sqlite3_open_v2((IntPtr)filename, (IntPtr) (&tmp_db), flags, (IntPtr)vfs);
                db = tmp_db;
                return (int) ret;
        }

		public unsafe static IntPtr sqlite3_vfs_find(byte* vfs)
        {
            var ret =
            foo.sqlite3_vfs_find((IntPtr)vfs);
            return (IntPtr) ret;
        }

		public unsafe static long sqlite3_last_insert_rowid(sqlite3 db)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_last_insert_rowid(db.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (long) ret;
        }

		public unsafe static int sqlite3_changes(sqlite3 db)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_changes(db.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_total_changes(sqlite3 db)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_total_changes(db.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static long sqlite3_memory_used()
        {
            var ret =
            foo.sqlite3_memory_used();
            return (long) ret;
        }

		public unsafe static long sqlite3_memory_highwater(int resetFlag)
        {
            var ret =
            foo.sqlite3_memory_highwater(resetFlag);
            return (long) ret;
        }

		public unsafe static long sqlite3_soft_heap_limit64(long n)
        {
            var ret =
            foo.sqlite3_soft_heap_limit64(n);
            return (long) ret;
        }

		public unsafe static long sqlite3_hard_heap_limit64(long n)
        {
            var ret =
            foo.sqlite3_hard_heap_limit64(n);
            return (long) ret;
        }

		public unsafe static int sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
                int tmp_current;
                    int tmp_highwater;
                var ret =
            foo.sqlite3_status(op, (IntPtr) (&tmp_current), (IntPtr) (&tmp_highwater), resetFlag);
                current = tmp_current;
                    highwater = tmp_highwater;
                return (int) ret;
        }

		public unsafe static int sqlite3_busy_timeout(sqlite3 db, int ms)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_busy_timeout(db.DangerousGetHandle(), ms);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_blob(sqlite3_stmt stmt, int index, byte* val, int nSize, IntPtr nTransient)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_blob(stmt.DangerousGetHandle(), index, (IntPtr)val, nSize, nTransient);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_zeroblob(stmt.DangerousGetHandle(), index, size);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_double(sqlite3_stmt stmt, int index, double val)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_double(stmt.DangerousGetHandle(), index, val);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_int(sqlite3_stmt stmt, int index, int val)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_int(stmt.DangerousGetHandle(), index, val);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_int64(stmt.DangerousGetHandle(), index, val);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_null(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_null(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_text(sqlite3_stmt stmt, int index, byte* val, int nlen, IntPtr pvReserved)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_text(stmt.DangerousGetHandle(), index, (IntPtr)val, nlen, pvReserved);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_text16(sqlite3_stmt stmt, int index, char* val, int nlen, IntPtr pvReserved)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_text16(stmt.DangerousGetHandle(), index, (IntPtr)val, nlen, pvReserved);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_parameter_count(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_parameter_count(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_bind_parameter_index(sqlite3_stmt stmt, byte* strName)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_bind_parameter_index(stmt.DangerousGetHandle(), (IntPtr)strName);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_column_count(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_count(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_data_count(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_data_count(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_step(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_step(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static byte* sqlite3_sql(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_sql(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (byte*) ret;
        }

		public unsafe static double sqlite3_column_double(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_double(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (double) ret;
        }

		public unsafe static int sqlite3_column_int(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_int(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static long sqlite3_column_int64(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_int64(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (long) ret;
        }

		public unsafe static IntPtr sqlite3_column_blob(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_blob(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (IntPtr) ret;
        }

		public unsafe static int sqlite3_column_bytes(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_bytes(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_column_type(sqlite3_stmt stmt, int index)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_column_type(stmt.DangerousGetHandle(), index);
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_aggregate_count(IntPtr context)
        {
            var ret =
            foo.sqlite3_aggregate_count(context);
            return (int) ret;
        }

		public unsafe static IntPtr sqlite3_value_blob(IntPtr p)
        {
            var ret =
            foo.sqlite3_value_blob(p);
            return (IntPtr) ret;
        }

		public unsafe static int sqlite3_value_bytes(IntPtr p)
        {
            var ret =
            foo.sqlite3_value_bytes(p);
            return (int) ret;
        }

		public unsafe static double sqlite3_value_double(IntPtr p)
        {
            var ret =
            foo.sqlite3_value_double(p);
            return (double) ret;
        }

		public unsafe static int sqlite3_value_int(IntPtr p)
        {
            var ret =
            foo.sqlite3_value_int(p);
            return (int) ret;
        }

		public unsafe static long sqlite3_value_int64(IntPtr p)
        {
            var ret =
            foo.sqlite3_value_int64(p);
            return (long) ret;
        }

		public unsafe static int sqlite3_value_type(IntPtr p)
        {
            var ret =
            foo.sqlite3_value_type(p);
            return (int) ret;
        }

		public unsafe static IntPtr sqlite3_user_data(IntPtr context)
        {
            var ret =
            foo.sqlite3_user_data(context);
            return (IntPtr) ret;
        }

		public unsafe static void sqlite3_result_blob(IntPtr context, IntPtr val, int nSize, IntPtr pvReserved)
        {
            foo.sqlite3_result_blob(context, val, nSize, pvReserved);
        }

		public unsafe static void sqlite3_result_double(IntPtr context, double val)
        {
            foo.sqlite3_result_double(context, val);
        }

		public unsafe static void sqlite3_result_error(IntPtr context, byte* strErr, int nLen)
        {
            foo.sqlite3_result_error(context, (IntPtr)strErr, nLen);
        }

		public unsafe static void sqlite3_result_int(IntPtr context, int val)
        {
            foo.sqlite3_result_int(context, val);
        }

		public unsafe static void sqlite3_result_int64(IntPtr context, long val)
        {
            foo.sqlite3_result_int64(context, val);
        }

		public unsafe static void sqlite3_result_null(IntPtr context)
        {
            foo.sqlite3_result_null(context);
        }

		public unsafe static void sqlite3_result_text(IntPtr context, byte* val, int nLen, IntPtr pvReserved)
        {
            foo.sqlite3_result_text(context, (IntPtr)val, nLen, pvReserved);
        }

		public unsafe static void sqlite3_result_zeroblob(IntPtr context, int n)
        {
            foo.sqlite3_result_zeroblob(context, n);
        }

		public unsafe static void sqlite3_result_error_toobig(IntPtr context)
        {
            foo.sqlite3_result_error_toobig(context);
        }

		public unsafe static void sqlite3_result_error_nomem(IntPtr context)
        {
            foo.sqlite3_result_error_nomem(context);
        }

		public unsafe static void sqlite3_result_error_code(IntPtr context, int code)
        {
            foo.sqlite3_result_error_code(context, code);
        }

		public unsafe static IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes)
        {
            var ret =
            foo.sqlite3_aggregate_context(context, nBytes);
            return (IntPtr) ret;
        }

		public unsafe static int sqlite3_config_none(int op)
        {
                IntPtr p_extras = IntPtr.Zero;
                var ret =
            foo.sqlite3_config(op, p_extras);
            return (int) ret;
        }

		public unsafe static int sqlite3_config_int(int op, int val)
        {
                var extras = new IntPtr[] {
                    (IntPtr) (1),
                        (IntPtr) val,
                    };
                var pinned_extras = System.Runtime.InteropServices.GCHandle.Alloc(extras, GCHandleType.Pinned);
                IntPtr p_extras = pinned_extras.AddrOfPinnedObject();
                var ret =
            foo.sqlite3_config(op, p_extras);
            pinned_extras.Free();
            return (int) ret;
        }

		public unsafe static int sqlite3_config_log(int op, IntPtr func, hook_handle pvUser)
        {
                var extras = new IntPtr[] {
                    (IntPtr) (2),
                        (IntPtr) func,
                        (IntPtr) pvUser.DangerousGetHandle(),
                    };
                var pinned_extras = System.Runtime.InteropServices.GCHandle.Alloc(extras, GCHandleType.Pinned);
                IntPtr p_extras = pinned_extras.AddrOfPinnedObject();
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_config(op, p_extras);
            pinned_extras.Free();
                if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_db_config_charptr(sqlite3 db, int op, byte* val)
        {
                var extras = new IntPtr[] {
                    (IntPtr) (1),
                        (IntPtr) val,
                    };
                var pinned_extras = System.Runtime.InteropServices.GCHandle.Alloc(extras, GCHandleType.Pinned);
                IntPtr p_extras = pinned_extras.AddrOfPinnedObject();
                    bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_db_config(db.DangerousGetHandle(), op, p_extras);
            pinned_extras.Free();
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_db_config_int_outint(sqlite3 db, int op, int val, int* result)
        {
                var extras = new IntPtr[] {
                    (IntPtr) (2),
                        (IntPtr) val,
                        (IntPtr) result,
                    };
                var pinned_extras = System.Runtime.InteropServices.GCHandle.Alloc(extras, GCHandleType.Pinned);
                IntPtr p_extras = pinned_extras.AddrOfPinnedObject();
                    bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_db_config(db.DangerousGetHandle(), op, p_extras);
            pinned_extras.Free();
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_db_config_intptr_int_int(sqlite3 db, int op, IntPtr ptr, int int0, int int1)
        {
                var extras = new IntPtr[] {
                    (IntPtr) (3),
                        (IntPtr) ptr,
                        (IntPtr) int0,
                        (IntPtr) int1,
                    };
                var pinned_extras = System.Runtime.InteropServices.GCHandle.Alloc(extras, GCHandleType.Pinned);
                IntPtr p_extras = pinned_extras.AddrOfPinnedObject();
                    bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_db_config(db.DangerousGetHandle(), op, p_extras);
            pinned_extras.Free();
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_create_collation(sqlite3 db, byte[] strName, int nType, hook_handle pvUser, IntPtr func)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    var pinned_strName = System.Runtime.InteropServices.GCHandle.Alloc(strName, GCHandleType.Pinned);
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_create_collation(db.DangerousGetHandle(), pinned_strName.AddrOfPinnedObject(), nType, pvUser.DangerousGetHandle(), func);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    pinned_strName.Free();
                    if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static IntPtr sqlite3_update_hook(sqlite3 db, IntPtr func, hook_handle pvUser)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_update_hook(db.DangerousGetHandle(), func, pvUser.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
                return (IntPtr) ret;
        }

		public unsafe static IntPtr sqlite3_commit_hook(sqlite3 db, IntPtr func, hook_handle pvUser)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_commit_hook(db.DangerousGetHandle(), func, pvUser.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
                return (IntPtr) ret;
        }

		public unsafe static IntPtr sqlite3_profile(sqlite3 db, IntPtr func, hook_handle pvUser)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_profile(db.DangerousGetHandle(), func, pvUser.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
                return (IntPtr) ret;
        }

		public unsafe static void sqlite3_progress_handler(sqlite3 db, int instructions, IntPtr func, hook_handle pvUser)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                foo.sqlite3_progress_handler(db.DangerousGetHandle(), instructions, func, pvUser.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
            }

		public unsafe static IntPtr sqlite3_trace(sqlite3 db, IntPtr func, hook_handle pvUser)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_trace(db.DangerousGetHandle(), func, pvUser.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
                return (IntPtr) ret;
        }

		public unsafe static IntPtr sqlite3_rollback_hook(sqlite3 db, IntPtr func, hook_handle pvUser)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_rollback_hook(db.DangerousGetHandle(), func, pvUser.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
                return (IntPtr) ret;
        }

		public unsafe static IntPtr sqlite3_db_handle(IntPtr stmt)
        {
            var ret =
            foo.sqlite3_db_handle(stmt);
            return (IntPtr) ret;
        }

		public unsafe static IntPtr sqlite3_next_stmt(sqlite3 db, IntPtr stmt)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_next_stmt(db.DangerousGetHandle(), stmt);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (IntPtr) ret;
        }

		public unsafe static int sqlite3_stmt_isexplain(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_stmt_isexplain(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_stmt_busy(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_stmt_busy(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_stmt_readonly(sqlite3_stmt stmt)
        {
                bool got_stmt = false;
                stmt.DangerousAddRef(ref got_stmt);
                if (!got_stmt) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_stmt_readonly(stmt.DangerousGetHandle());
                if (got_stmt)
                {
                    stmt.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_exec(sqlite3 db, byte* strSql, IntPtr cb, hook_handle pvParam, out IntPtr errMsg)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    bool got_pvParam = false;
                pvParam.DangerousAddRef(ref got_pvParam);
                if (!got_pvParam) throw new Exception("SafeHandle.DangerousAddRef failed");
                    IntPtr tmp_errMsg;
                var ret =
            foo.sqlite3_exec(db.DangerousGetHandle(), (IntPtr)strSql, cb, pvParam.DangerousGetHandle(), (IntPtr) (&tmp_errMsg));
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    if (got_pvParam)
                {
                    pvParam.DangerousRelease();
                }
                    errMsg = tmp_errMsg;
                return (int) ret;
        }

		public unsafe static int sqlite3_get_autocommit(sqlite3 db)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_get_autocommit(db.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_extended_result_codes(sqlite3 db, int onoff)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_extended_result_codes(db.DangerousGetHandle(), onoff);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_errcode(sqlite3 db)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_errcode(db.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_extended_errcode(sqlite3 db)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_extended_errcode(db.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static byte* sqlite3_errstr(int rc)
        {
            var ret =
            foo.sqlite3_errstr(rc);
            return (byte*) ret;
        }

		public unsafe static void sqlite3_log(int iErrCode, byte* zFormat)
        {
                IntPtr p_extras = IntPtr.Zero;
                foo.sqlite3_log(iErrCode, (IntPtr)zFormat, p_extras);
        }

		public unsafe static int sqlite3_file_control(sqlite3 db, byte[] zDbName, int op, IntPtr pArg)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    var pinned_zDbName = System.Runtime.InteropServices.GCHandle.Alloc(zDbName, GCHandleType.Pinned);
                var ret =
            foo.sqlite3_file_control(db.DangerousGetHandle(), pinned_zDbName.AddrOfPinnedObject(), op, pArg);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    pinned_zDbName.Free();
                return (int) ret;
        }

		public unsafe static sqlite3_backup sqlite3_backup_init(sqlite3 destDb, byte* zDestName, sqlite3 sourceDb, byte* zSourceName)
        {
                bool got_destDb = false;
                destDb.DangerousAddRef(ref got_destDb);
                if (!got_destDb) throw new Exception("SafeHandle.DangerousAddRef failed");
                    bool got_sourceDb = false;
                sourceDb.DangerousAddRef(ref got_sourceDb);
                if (!got_sourceDb) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_backup_init(destDb.DangerousGetHandle(), (IntPtr)zDestName, sourceDb.DangerousGetHandle(), (IntPtr)zSourceName);
                if (got_destDb)
                {
                    destDb.DangerousRelease();
                }
                    if (got_sourceDb)
                {
                    sourceDb.DangerousRelease();
                }
                return sqlite3_backup.From(ret);
        }

		public unsafe static int sqlite3_backup_step(sqlite3_backup backup, int nPage)
        {
                bool got_backup = false;
                backup.DangerousAddRef(ref got_backup);
                if (!got_backup) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_backup_step(backup.DangerousGetHandle(), nPage);
                if (got_backup)
                {
                    backup.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_backup_remaining(sqlite3_backup backup)
        {
                bool got_backup = false;
                backup.DangerousAddRef(ref got_backup);
                if (!got_backup) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_backup_remaining(backup.DangerousGetHandle());
                if (got_backup)
                {
                    backup.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_backup_pagecount(sqlite3_backup backup)
        {
                bool got_backup = false;
                backup.DangerousAddRef(ref got_backup);
                if (!got_backup) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_backup_pagecount(backup.DangerousGetHandle());
                if (got_backup)
                {
                    backup.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_backup_finish(IntPtr backup)
        {
            var ret =
            foo.sqlite3_backup_finish(backup);
            return (int) ret;
        }

		public unsafe static int sqlite3_snapshot_get(sqlite3 db, byte* schema, out IntPtr snap)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    IntPtr tmp_snap;
                var ret =
            foo.sqlite3_snapshot_get(db.DangerousGetHandle(), (IntPtr)schema, (IntPtr) (&tmp_snap));
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    snap = tmp_snap;
                return (int) ret;
        }

		public unsafe static int sqlite3_snapshot_open(sqlite3 db, byte* schema, sqlite3_snapshot snap)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_snapshot_open(db.DangerousGetHandle(), (IntPtr)schema, snap.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_snapshot_recover(sqlite3 db, byte* name)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_snapshot_recover(db.DangerousGetHandle(), (IntPtr)name);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_snapshot_cmp(sqlite3_snapshot p1, sqlite3_snapshot p2)
        {
            var ret =
            foo.sqlite3_snapshot_cmp(p1.DangerousGetHandle(), p2.DangerousGetHandle());
            return (int) ret;
        }

		public unsafe static void sqlite3_snapshot_free(IntPtr snap)
        {
            foo.sqlite3_snapshot_free(snap);
        }

		public unsafe static int sqlite3_blob_open(sqlite3 db, byte* sdb, byte* table, byte* col, long rowid, int flags, out IntPtr blob)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    IntPtr tmp_blob;
                var ret =
            foo.sqlite3_blob_open(db.DangerousGetHandle(), (IntPtr)sdb, (IntPtr)table, (IntPtr)col, rowid, flags, (IntPtr) (&tmp_blob));
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    blob = tmp_blob;
                return (int) ret;
        }

		public unsafe static int sqlite3_blob_write(sqlite3_blob blob, byte* b, int n, int offset)
        {
                bool got_blob = false;
                blob.DangerousAddRef(ref got_blob);
                if (!got_blob) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_blob_write(blob.DangerousGetHandle(), (IntPtr)b, n, offset);
                if (got_blob)
                {
                    blob.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_blob_read(sqlite3_blob blob, byte* b, int n, int offset)
        {
                bool got_blob = false;
                blob.DangerousAddRef(ref got_blob);
                if (!got_blob) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_blob_read(blob.DangerousGetHandle(), (IntPtr)b, n, offset);
                if (got_blob)
                {
                    blob.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_blob_bytes(sqlite3_blob blob)
        {
                bool got_blob = false;
                blob.DangerousAddRef(ref got_blob);
                if (!got_blob) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_blob_bytes(blob.DangerousGetHandle());
                if (got_blob)
                {
                    blob.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
        {
                bool got_blob = false;
                blob.DangerousAddRef(ref got_blob);
                if (!got_blob) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_blob_reopen(blob.DangerousGetHandle(), rowid);
                if (got_blob)
                {
                    blob.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_blob_close(IntPtr blob)
        {
            var ret =
            foo.sqlite3_blob_close(blob);
            return (int) ret;
        }

		public unsafe static int sqlite3_wal_autocheckpoint(sqlite3 db, int n)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_wal_autocheckpoint(db.DangerousGetHandle(), n);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_wal_checkpoint(sqlite3 db, byte* dbName)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_wal_checkpoint(db.DangerousGetHandle(), (IntPtr)dbName);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_wal_checkpoint_v2(sqlite3 db, byte* dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    int tmp_logSize;
                    int tmp_framesCheckPointed;
                var ret =
            foo.sqlite3_wal_checkpoint_v2(db.DangerousGetHandle(), (IntPtr)dbName, eMode, (IntPtr) (&tmp_logSize), (IntPtr) (&tmp_framesCheckPointed));
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    logSize = tmp_logSize;
                    framesCheckPointed = tmp_framesCheckPointed;
                return (int) ret;
        }

		public unsafe static int sqlite3_set_authorizer(sqlite3 db, IntPtr cb, hook_handle pvUser)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_set_authorizer(db.DangerousGetHandle(), cb, pvUser.DangerousGetHandle());
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_create_function_v2(sqlite3 db, byte[] strName, int nArgs, int nType, hook_handle pvUser, IntPtr func, IntPtr fstep, IntPtr ffinal, IntPtr fdestroy)
        {
                bool got_db = false;
                db.DangerousAddRef(ref got_db);
                if (!got_db) throw new Exception("SafeHandle.DangerousAddRef failed");
                    var pinned_strName = System.Runtime.InteropServices.GCHandle.Alloc(strName, GCHandleType.Pinned);
                    bool got_pvUser = false;
                pvUser.DangerousAddRef(ref got_pvUser);
                if (!got_pvUser) throw new Exception("SafeHandle.DangerousAddRef failed");
                var ret =
            foo.sqlite3_create_function_v2(db.DangerousGetHandle(), pinned_strName.AddrOfPinnedObject(), nArgs, nType, pvUser.DangerousGetHandle(), func, fstep, ffinal, fdestroy);
                if (got_db)
                {
                    db.DangerousRelease();
                }
                    pinned_strName.Free();
                    if (got_pvUser)
                {
                    pvUser.DangerousRelease();
                }
                return (int) ret;
        }

		public unsafe static int sqlite3_keyword_count()
        {
            var ret =
            foo.sqlite3_keyword_count();
            return (int) ret;
        }

		public unsafe static int sqlite3_keyword_name(int i, out byte* name, out int length)
        {
                byte* tmp_name;
                    int tmp_length;
                var ret =
            foo.sqlite3_keyword_name(i, (IntPtr) (&tmp_name), (IntPtr) (&tmp_length));
                name = tmp_name;
                    length = tmp_length;
                return (int) ret;
        }

	}


    }
}
