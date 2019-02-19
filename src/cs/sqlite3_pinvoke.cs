/*
   Copyright 2014-2019 Zumero, LLC

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

	[Preserve(AllMembers = true)]
    static class SQLite3Provider_dyn
    {
		// TODO very unhappy that this needs to be static
		public static MyDelegates NativeMethods;

        public static int sqlite3_win32_set_directory(int typ, string path)
        {
            return NativeMethods.sqlite3_win32_set_directory((uint) typ, path);
        }

        public static int sqlite3_open(string filename, out IntPtr db)
        {
            return NativeMethods.sqlite3_open(util.to_utf8(filename), out db);
        }

        public static int sqlite3_open_v2(string filename, out IntPtr db, int flags, string vfs)
        {
            return NativeMethods.sqlite3_open_v2(util.to_utf8(filename), out db, flags, util.to_utf8(vfs));
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
        public MyDelegateTypes.SQLiteDeleteDelegate xDelete;
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
    }
    #pragma warning restore 649
	
	public static int sqlite3__vfs__delete(string vfs, string filename, int syncDir)
	{
	    IntPtr ptrVfs = NativeMethods.sqlite3_vfs_find(util.to_utf8(vfs));
	    // this code and the struct it uses was taken from aspnet/DataCommon.SQLite, Apache License 2.0
	    sqlite3_vfs vstruct = (sqlite3_vfs) Marshal.PtrToStructure(ptrVfs, typeof(sqlite3_vfs));
	    return vstruct.xDelete(ptrVfs, util.to_utf8(filename), 1);
	}

        public static int sqlite3_close_v2(IntPtr db)
        {
            var rc = NativeMethods.sqlite3_close_v2(db);
		hooks.removeFor(db);
		return rc;
        }

        public static int sqlite3_close(IntPtr db)
        {
            var rc = NativeMethods.sqlite3_close(db);
		hooks.removeFor(db);
		return rc;
        }

        public static int sqlite3_enable_shared_cache(int enable)
        {
            return NativeMethods.sqlite3_enable_shared_cache(enable);
        }

        public static void sqlite3_interrupt(IntPtr db)
        {
            NativeMethods.sqlite3_interrupt(db);
        }

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_exec))]
        static int exec_hook_bridge(IntPtr p, int n, IntPtr values_ptr, IntPtr names_ptr)
        {
            exec_hook_info hi = exec_hook_info.from_ptr(p);
            return hi.call(n, values_ptr, names_ptr);
        }
// TODO shouldn't there be a impl/bridge thing here

        public static int sqlite3_exec(IntPtr db, string sql, delegate_exec func, object user_data, out string errMsg)
        {
            IntPtr errmsg_ptr;
            int rc;

            if (func != null)
            {
                exec_hook_info hi = new exec_hook_info(func, user_data);
                rc = NativeMethods.sqlite3_exec(db, util.to_utf8(sql), exec_hook_bridge, hi.ptr, out errmsg_ptr);
                hi.free();
            }
            else
            {
                rc = NativeMethods.sqlite3_exec(db, util.to_utf8(sql), null, IntPtr.Zero, out errmsg_ptr);
            }

            if (errmsg_ptr == IntPtr.Zero)
            {
                errMsg = null;
            }
            else
            {
                errMsg = util.from_utf8(errmsg_ptr);
                NativeMethods.sqlite3_free(errmsg_ptr);
            }

            return rc;
        }

        public static int sqlite3_complete(string sql)
        {
            return NativeMethods.sqlite3_complete(util.to_utf8(sql));
        }

        public static string sqlite3_compileoption_get(int n)
        {
            return util.from_utf8(NativeMethods.sqlite3_compileoption_get(n));
        }

        public static int sqlite3_compileoption_used(string s)
        {
            return NativeMethods.sqlite3_compileoption_used(util.to_utf8(s));
        }

        public static int sqlite3_table_column_metadata(IntPtr db, string dbName, string tblName, string colName, out string dataType, out string collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            IntPtr datatype_ptr;
            IntPtr collseq_ptr;

            int rc = NativeMethods.sqlite3_table_column_metadata(
                        db, util.to_utf8(dbName), util.to_utf8(tblName), util.to_utf8(colName), 
                        out datatype_ptr, out collseq_ptr, out notNull, out primaryKey, out autoInc);

            if (datatype_ptr == IntPtr.Zero)
            {
                dataType = null;
            }
            else
            {
                dataType = util.from_utf8(datatype_ptr);
                if (dataType.Length == 0)
                {
                    dataType = null;
                }
            }

            if (collseq_ptr == IntPtr.Zero)
            {
                collSeq = null;
            }
            else
            {
                collSeq = util.from_utf8(collseq_ptr);
                if (collSeq.Length == 0)
                {
                    collSeq = null;
                }
            }         
  
            return rc; 
        }

        public static int sqlite3_prepare_v2(IntPtr db, string sql, out IntPtr stm, out string remain)
        {
            var ba_sql = util.to_utf8(sql);
            GCHandle pinned_sql = GCHandle.Alloc(ba_sql, GCHandleType.Pinned);
            IntPtr ptr_sql = pinned_sql.AddrOfPinnedObject();
            IntPtr tail;
            int rc = NativeMethods.sqlite3_prepare_v2(db, ptr_sql, -1, out stm, out tail);
            if (tail == IntPtr.Zero)
            {
                remain = null;
            }
            else
            {
                remain = util.from_utf8(tail);
                if (remain.Length == 0)
                {
                    remain = null;
                }
            }
            pinned_sql.Free();
            return rc;
        }

        public static int sqlite3_db_status(IntPtr db, int op, out int current, out int highest, int resetFlg)
        {
            return NativeMethods.sqlite3_db_status(db, op, out current, out highest, resetFlg);
        }

        public static string sqlite3_sql(IntPtr stmt)
        {
            return util.from_utf8(NativeMethods.sqlite3_sql(stmt));
        }

        public static IntPtr sqlite3_db_handle(IntPtr stmt)
        {
            return NativeMethods.sqlite3_db_handle(stmt);
        }

        public static int sqlite3_blob_open(IntPtr db, byte[] db_utf8, byte[] table_utf8, byte[] col_utf8, long rowid, int flags, out IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_open(db, db_utf8, table_utf8, col_utf8, rowid, flags, out blob);
        }

        public static int sqlite3_blob_open(IntPtr db, string sdb, string table, string col, long rowid, int flags, out IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_open(db, util.to_utf8(sdb), util.to_utf8(table), util.to_utf8(col), rowid, flags, out blob);
        }

        public static int sqlite3_blob_bytes(IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_bytes(blob);
        }

        public static int sqlite3_blob_close(IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_close(blob);
        }

        public static int sqlite3_blob_read(IntPtr blob, byte[] b, int n, int offset)
        {
            return sqlite3_blob_read(blob, b, 0, n, offset);
        }

        public static int sqlite3_blob_write(IntPtr blob, byte[] b, int n, int offset)
        {
            return sqlite3_blob_write(blob, b, 0, n, offset);
        }

        public static int sqlite3_blob_read(IntPtr blob, byte[] b, int bOffset, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = NativeMethods.sqlite3_blob_read(blob, new IntPtr(ptr.ToInt64() + bOffset), n, offset);
            pinned.Free();
	    return rc;
        }

        public static int sqlite3_blob_write(IntPtr blob, byte[] b, int bOffset, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = NativeMethods.sqlite3_blob_write(blob, new IntPtr(ptr.ToInt64() + bOffset), n, offset);
            pinned.Free();
	    return rc;
        }

        public static IntPtr sqlite3_backup_init(IntPtr destDb, string destName, IntPtr sourceDb, string sourceName)
        {
            return NativeMethods.sqlite3_backup_init(destDb, util.to_utf8(destName), sourceDb, util.to_utf8(sourceName));
        }

        public static int sqlite3_backup_step(IntPtr backup, int nPage)
        {
            return NativeMethods.sqlite3_backup_step(backup, nPage);
        }

        public static int sqlite3_backup_finish(IntPtr backup)
        {
            return NativeMethods.sqlite3_backup_finish(backup);
        }

        public static int sqlite3_backup_remaining(IntPtr backup)
        {
            return NativeMethods.sqlite3_backup_remaining(backup);
        }

        public static int sqlite3_backup_pagecount(IntPtr backup)
        {
            return NativeMethods.sqlite3_backup_pagecount(backup);
        }

        public static IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt)
        {
            return NativeMethods.sqlite3_next_stmt(db, stmt);
        }

        public static long sqlite3_last_insert_rowid(IntPtr db)
        {
            return NativeMethods.sqlite3_last_insert_rowid(db);
        }

        public static int sqlite3_changes(IntPtr db)
        {
            return NativeMethods.sqlite3_changes(db);
        }

        public static int sqlite3_total_changes(IntPtr db)
        {
            return NativeMethods.sqlite3_total_changes(db);
        }

        public static int sqlite3_extended_result_codes(IntPtr db, int onoff)
        {
            return NativeMethods.sqlite3_extended_result_codes(db, onoff);
        }

        public static string sqlite3_errstr(int rc)
        {
            return util.from_utf8(NativeMethods.sqlite3_errstr(rc));
        }

        public static int sqlite3_errcode(IntPtr db)
        {
            return NativeMethods.sqlite3_errcode(db);
        }

        public static int sqlite3_extended_errcode(IntPtr db)
        {
            return NativeMethods.sqlite3_extended_errcode(db);
        }

        public static int sqlite3_busy_timeout(IntPtr db, int ms)
        {
            return NativeMethods.sqlite3_busy_timeout(db, ms);
        }

        public static int sqlite3_get_autocommit(IntPtr db)
        {
            return NativeMethods.sqlite3_get_autocommit(db);
        }

        public static int sqlite3_db_readonly(IntPtr db, string dbName)
        {
            return NativeMethods.sqlite3_db_readonly(db, util.to_utf8(dbName)); 
        }
        
        public static string sqlite3_db_filename(IntPtr db, string att)
	{
            return util.from_utf8(NativeMethods.sqlite3_db_filename(db, util.to_utf8(att)));
	}

        public static string sqlite3_errmsg(IntPtr db)
        {
            return util.from_utf8(NativeMethods.sqlite3_errmsg(db));
        }

        public static string sqlite3_libversion()
        {
            return util.from_utf8(NativeMethods.sqlite3_libversion());
        }

        public static int sqlite3_libversion_number()
        {
            return NativeMethods.sqlite3_libversion_number();
        }

        public static int sqlite3_threadsafe()
        {
            return NativeMethods.sqlite3_threadsafe();
        }

        public static int sqlite3_config(int op)
        {
            return NativeMethods.sqlite3_config_none(op);
        }

        public static int sqlite3_config(int op, int val)
        {
            return NativeMethods.sqlite3_config_int(op, val);
        }

        public static int sqlite3_initialize()
        {
            return NativeMethods.sqlite3_initialize();
        }

        public static int sqlite3_shutdown()
        {
            return NativeMethods.sqlite3_shutdown();
        }

        public static int sqlite3_enable_load_extension(IntPtr db, int onoff)
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
        
        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_commit))]
        static int commit_hook_bridge(IntPtr p)
        {
            commit_hook_info hi = commit_hook_info.from_ptr(p);
            return hi.call();
        }

        public static void sqlite3_commit_hook(IntPtr db, delegate_commit func, object v)
        {
			var info = hooks.getOrCreateFor(db);
            if (info.commit != null)
            {
                // TODO maybe turn off the hook here, for now
                info.commit.free();
                info.commit = null;
            }

            if (func != null)
            {
                info.commit = new commit_hook_info(func, v);
                NativeMethods.sqlite3_commit_hook(db, commit_hook_bridge, info.commit.ptr);
            }
            else
            {
                NativeMethods.sqlite3_commit_hook(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_scalar_function))]
        static void scalar_function_hook_bridge_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr p = NativeMethods.sqlite3_user_data(context);
            scalar_function_hook_info hi = scalar_function_hook_info.from_ptr(p);
            hi.call(context, num_args, argsptr);
        }

	static MyDelegateTypes.callback_scalar_function scalar_function_hook_bridge = new MyDelegateTypes.callback_scalar_function(scalar_function_hook_bridge_impl); 

        static int my_sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_scalar func)
        {
        // the keys for this dictionary are nargs.name, not just the name
            string key = string.Format("{0}.{1}", nargs, name);
		var info = hooks.getOrCreateFor(db);
            if (info.scalar.ContainsKey(key))
            {
                scalar_function_hook_info hi = info.scalar[key];

                // TODO maybe turn off the hook here, for now
                hi.free();

                info.scalar.Remove(key);
            }

            // 1 is SQLITE_UTF8
			int arg4 = 1 | flags;
            if (func != null)
            {
                scalar_function_hook_info hi = new scalar_function_hook_info(func, v);
                int rc = NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, arg4, hi.ptr, scalar_function_hook_bridge, null, null, null);
                if (rc == 0)
                {
                    info.scalar[key] = hi;
                }
                return rc;
            }
            else
            {
                return NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, arg4, IntPtr.Zero, null, null, null, null);
            }
        }

        public static int sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_scalar func)
		{
			return my_sqlite3_create_function(db, name, nargs, 0, v, func);
		}

        public static int sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_scalar func)
		{
			return my_sqlite3_create_function(db, name, nargs, flags, v, func);
		}

        // ----------------------------------------------------------------

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_log))]
        static void log_hook_bridge_impl(IntPtr p, int rc, IntPtr s)
        {
            log_hook_info hi = log_hook_info.from_ptr(p);
            hi.call(rc, util.from_utf8(s));
        }

	static MyDelegateTypes.callback_log log_hook_bridge = new MyDelegateTypes.callback_log(log_hook_bridge_impl); 
        public static int sqlite3_config_log(delegate_log func, object v)
        {
            if (hooks.log != null)
            {
                // TODO maybe turn off the hook here, for now
                hooks.log.free();
                hooks.log = null;
            }

            if (func != null)
            {
                hooks.log = new log_hook_info(func, v);
                return NativeMethods.sqlite3_config_log(raw.SQLITE_CONFIG_LOG, log_hook_bridge, hooks.log.ptr);
            }
            else
            {
                return NativeMethods.sqlite3_config_log(raw.SQLITE_CONFIG_LOG, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_agg_function_step))]
        static void agg_function_hook_bridge_step_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            agg_function_hook_info hi = agg_function_hook_info.from_ptr(p);
            hi.call_step(context, agg, num_args, argsptr);
        }

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_agg_function_final))]
        static void agg_function_hook_bridge_final_impl(IntPtr context)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            agg_function_hook_info hi = agg_function_hook_info.from_ptr(p);
            hi.call_final(context, agg);
        }

	static MyDelegateTypes.callback_agg_function_step agg_function_hook_bridge_step = new MyDelegateTypes.callback_agg_function_step(agg_function_hook_bridge_step_impl); 
	static MyDelegateTypes.callback_agg_function_final agg_function_hook_bridge_final = new MyDelegateTypes.callback_agg_function_final(agg_function_hook_bridge_final_impl); 

        static int my_sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
        // the keys for this dictionary are nargs.name, not just the name
            string key = string.Format("{0}.{1}", nargs, name);
		var info = hooks.getOrCreateFor(db);
            if (info.agg.ContainsKey(key))
            {
                agg_function_hook_info hi = info.agg[key];

                // TODO maybe turn off the hook here, for now
                hi.free();

                info.agg.Remove(key);
            }

            // 1 is SQLITE_UTF8
			int arg4 = 1 | flags;
            if (func_step != null)
            {
                // TODO both func_step and func_final must be non-null
                agg_function_hook_info hi = new agg_function_hook_info(func_step, func_final, v);
                int rc = NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, arg4, hi.ptr, null, agg_function_hook_bridge_step, agg_function_hook_bridge_final, null);
                if (rc == 0)
                {
                    info.agg[key] = hi;
                }
                return rc;
            }
            else
            {
                return NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, arg4, IntPtr.Zero, null, null, null, null);
            }
        }

        public static int sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
		{
			return my_sqlite3_create_function(db, name, nargs, 0, v, func_step, func_final);
		}

        public static int sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
		{
			return my_sqlite3_create_function(db, name, nargs, flags, v, func_step, func_final);
		}

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_collation))]
        static int collation_hook_bridge_impl(IntPtr p, int len1, IntPtr pv1, int len2, IntPtr pv2)
        {
            collation_hook_info hi = collation_hook_info.from_ptr(p);
            return hi.call(util.from_utf8(pv1, len1), util.from_utf8(pv2, len2));
        }

	static MyDelegateTypes.callback_collation collation_hook_bridge = new MyDelegateTypes.callback_collation(collation_hook_bridge_impl); 
        public static int sqlite3_create_collation(IntPtr db, string name, object v, delegate_collation func)
        {
		var info = hooks.getOrCreateFor(db);
            if (info.collation.ContainsKey(name))
            {
                collation_hook_info hi = info.collation[name];

                // TODO maybe turn off the hook here, for now
                hi.free();

                info.collation.Remove(name);
            }

            // 1 is SQLITE_UTF8
            if (func != null)
            {
                collation_hook_info hi = new collation_hook_info(func, v);
                int rc = NativeMethods.sqlite3_create_collation(db, util.to_utf8(name), 1, hi.ptr, collation_hook_bridge);
                if (rc == 0)
                {
                    info.collation[name] = hi;
                }
                return rc;
            }
            else
            {
                return NativeMethods.sqlite3_create_collation(db, util.to_utf8(name), 1, IntPtr.Zero, null);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_update))]
        static void update_hook_bridge(IntPtr p, int typ, IntPtr db, IntPtr tbl, Int64 rowid)
        {
            update_hook_info hi = update_hook_info.from_ptr(p);
            hi.call(typ, util.from_utf8(db), util.from_utf8(tbl), rowid);
        }

        public static void sqlite3_update_hook(IntPtr db, delegate_update func, object v)
        {
		var info = hooks.getOrCreateFor(db);
            if (info.update != null)
            {
                // TODO maybe turn off the hook here, for now
                info.update.free();
                info.update = null;
            }

            if (func != null)
            {
                info.update = new update_hook_info(func, v);
                NativeMethods.sqlite3_update_hook(db, update_hook_bridge, info.update.ptr);
            }
            else
            {
                NativeMethods.sqlite3_update_hook(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_rollback))]
        static void rollback_hook_bridge_impl(IntPtr p)
        {
            rollback_hook_info hi = rollback_hook_info.from_ptr(p);
            hi.call();
        }

	static MyDelegateTypes.callback_rollback rollback_hook_bridge = new MyDelegateTypes.callback_rollback(rollback_hook_bridge_impl); 
        public static void sqlite3_rollback_hook(IntPtr db, delegate_rollback func, object v)
        {
		var info = hooks.getOrCreateFor(db);
            if (info.rollback != null)
            {
                // TODO maybe turn off the hook here, for now
                info.rollback.free();
                info.rollback = null;
            }

            if (func != null)
            {
                info.rollback = new rollback_hook_info(func, v);
                NativeMethods.sqlite3_rollback_hook(db, rollback_hook_bridge, info.rollback.ptr);
            }
            else
            {
                NativeMethods.sqlite3_rollback_hook(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_trace))]
        static void trace_hook_bridge_impl(IntPtr p, IntPtr s)
        {
            trace_hook_info hi = trace_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s));
        }

	static MyDelegateTypes.callback_trace trace_hook_bridge = new MyDelegateTypes.callback_trace(trace_hook_bridge_impl); 
        public static void sqlite3_trace(IntPtr db, delegate_trace func, object v)
        {
		var info = hooks.getOrCreateFor(db);
            if (info.trace != null)
            {
                // TODO maybe turn off the hook here, for now
                info.trace.free();
                info.trace = null;
            }

            if (func != null)
            {
                info.trace = new trace_hook_info(func, v);
                NativeMethods.sqlite3_trace(db, trace_hook_bridge, info.trace.ptr);
            }
            else
            {
                NativeMethods.sqlite3_trace(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_profile))]
        static void profile_hook_bridge_impl(IntPtr p, IntPtr s, long elapsed)
        {
            profile_hook_info hi = profile_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s), elapsed);
        }

	static MyDelegateTypes.callback_profile profile_hook_bridge = new MyDelegateTypes.callback_profile(profile_hook_bridge_impl); 
        public static void sqlite3_profile(IntPtr db, delegate_profile func, object v)
        {
		var info = hooks.getOrCreateFor(db);
            if (info.profile != null)
            {
                // TODO maybe turn off the hook here, for now
                info.profile.free();
                info.profile = null;
            }

            if (func != null)
            {
                info.profile = new profile_hook_info(func, v);
                NativeMethods.sqlite3_profile(db, profile_hook_bridge, info.profile.ptr);
            }
            else
            {
                NativeMethods.sqlite3_profile(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_progress_handler))]
        static int progress_handler_hook_bridge_impl(IntPtr p)
        {
            progress_handler_hook_info hi = progress_handler_hook_info.from_ptr(p);
            return hi.call();
        }

        static MyDelegateTypes.callback_progress_handler progress_handler_hook_bridge = new MyDelegateTypes.callback_progress_handler(progress_handler_hook_bridge_impl);
        public static void sqlite3_progress_handler(IntPtr db, int instructions, delegate_progress_handler func, object v)
        {
		var info = hooks.getOrCreateFor(db);
            if (info.progress != null)
            {
                // TODO maybe turn off the hook here, for now
                info.progress.free();
                info.progress = null;
            }

            if (func != null)
            {
                info.progress = new progress_handler_hook_info(func, v);
                NativeMethods.sqlite3_progress_handler(db, instructions, progress_handler_hook_bridge, info.progress.ptr);
            }
            else
            {
                NativeMethods.sqlite3_progress_handler(db, instructions, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [MonoPInvokeCallback (typeof(MyDelegateTypes.callback_authorizer))]
        static int authorizer_hook_bridge_impl(IntPtr p, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view)
        {
            authorizer_hook_info hi = authorizer_hook_info.from_ptr(p);
            return hi.call(action_code, util.from_utf8(param0), util.from_utf8(param1), util.from_utf8(dbName), util.from_utf8(inner_most_trigger_or_view));
        }

        static MyDelegateTypes.callback_authorizer authorizer_hook_bridge = new MyDelegateTypes.callback_authorizer(authorizer_hook_bridge_impl);
        public static int sqlite3_set_authorizer(IntPtr db, delegate_authorizer func, object v)
        {
		var info = hooks.getOrCreateFor(db);
            if (info.authorizer != null)
            {
                // TODO maybe turn off the hook here, for now
                info.authorizer.free();
                info.authorizer = null;
            }

            if (func != null)
            {
                info.authorizer = new authorizer_hook_info(func, v);
                return NativeMethods.sqlite3_set_authorizer(db, authorizer_hook_bridge, info.authorizer.ptr);
            }
            else
            {
                return NativeMethods.sqlite3_set_authorizer(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        public static long sqlite3_memory_used()
        {
            return NativeMethods.sqlite3_memory_used();
        }

        public static long sqlite3_memory_highwater(int resetFlag)
        {
            return NativeMethods.sqlite3_memory_highwater(resetFlag);
        }

        public static int sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            return NativeMethods.sqlite3_status(op, out current, out highwater, resetFlag);
        }

        public static string sqlite3_sourceid()
        {
            return util.from_utf8(NativeMethods.sqlite3_sourceid());
        }

        public static void sqlite3_result_int64(IntPtr ctx, long val)
        {
            NativeMethods.sqlite3_result_int64(ctx, val);
        }

        public static void sqlite3_result_int(IntPtr ctx, int val)
        {
            NativeMethods.sqlite3_result_int(ctx, val);
        }

        public static void sqlite3_result_double(IntPtr ctx, double val)
        {
            NativeMethods.sqlite3_result_double(ctx, val);
        }

        public static void sqlite3_result_null(IntPtr stm)
        {
            NativeMethods.sqlite3_result_null(stm);
        }

        public static void sqlite3_result_error(IntPtr ctx, string val)
        {
            NativeMethods.sqlite3_result_error(ctx, util.to_utf8(val), -1);
        }

        public static void sqlite3_result_text(IntPtr ctx, string val)
        {
            NativeMethods.sqlite3_result_text(ctx, util.to_utf8(val), -1, new IntPtr(-1));
        }

        public static void sqlite3_result_blob(IntPtr ctx, byte[] blob)
        {
            NativeMethods.sqlite3_result_blob(ctx, blob, blob.Length, new IntPtr(-1));
        }

        public static void sqlite3_result_zeroblob(IntPtr ctx, int n)
        {
            NativeMethods.sqlite3_result_zeroblob(ctx, n);
        }

        // TODO sqlite3_result_value

        public static void sqlite3_result_error_toobig(IntPtr ctx)
        {
            NativeMethods.sqlite3_result_error_toobig(ctx);
        }

        public static void sqlite3_result_error_nomem(IntPtr ctx)
        {
            NativeMethods.sqlite3_result_error_nomem(ctx);
        }

        public static void sqlite3_result_error_code(IntPtr ctx, int code)
        {
            NativeMethods.sqlite3_result_error_code(ctx, code);
        }

        public static byte[] sqlite3_value_blob(IntPtr p)
        {
            IntPtr blobPointer = NativeMethods.sqlite3_value_blob(p);
            if (blobPointer == IntPtr.Zero)
            {
                return null;
            }

            var length = NativeMethods.sqlite3_value_bytes(p);
            byte[] result = new byte[length];
            Marshal.Copy(blobPointer, (byte[])result, 0, length);
            return result;
        }

        public static int sqlite3_value_bytes(IntPtr p)
        {
            return NativeMethods.sqlite3_value_bytes(p);
        }

        public static double sqlite3_value_double(IntPtr p)
        {
            return NativeMethods.sqlite3_value_double(p);
        }

        public static int sqlite3_value_int(IntPtr p)
        {
            return NativeMethods.sqlite3_value_int(p);
        }

        public static long sqlite3_value_int64(IntPtr p)
        {
            return NativeMethods.sqlite3_value_int64(p);
        }

        public static int sqlite3_value_type(IntPtr p)
        {
            return NativeMethods.sqlite3_value_type(p);
        }

        public static string sqlite3_value_text(IntPtr p)
        {
            return util.from_utf8(NativeMethods.sqlite3_value_text(p));
        }

        public static int sqlite3_bind_int(IntPtr stm, int paramIndex, int val)
        {
            return NativeMethods.sqlite3_bind_int(stm, paramIndex, val);
        }

        public static int sqlite3_bind_int64(IntPtr stm, int paramIndex, long val)
        {
            return NativeMethods.sqlite3_bind_int64(stm, paramIndex, val);
        }

        public static int sqlite3_bind_text(IntPtr stm, int paramIndex, string t)
        {
            return NativeMethods.sqlite3_bind_text(stm, paramIndex, util.to_utf8(t), -1, new IntPtr(-1));
        }

        public static int sqlite3_bind_double(IntPtr stm, int paramIndex, double val)
        {
            return NativeMethods.sqlite3_bind_double(stm, paramIndex, val);
        }

        public static int sqlite3_bind_blob(IntPtr stm, int paramIndex, byte[] blob)
        {
            return NativeMethods.sqlite3_bind_blob(stm, paramIndex, blob, blob.Length, new IntPtr(-1));
        }

        public static int sqlite3_bind_blob(IntPtr stm, int paramIndex, byte[] blob, int nSize)
        {
            return NativeMethods.sqlite3_bind_blob(stm, paramIndex, blob, nSize, new IntPtr(-1));
        }

        public static int sqlite3_bind_zeroblob(IntPtr stm, int paramIndex, int size)
        {
            return NativeMethods.sqlite3_bind_zeroblob(stm, paramIndex, size);
        }

        public static int sqlite3_bind_null(IntPtr stm, int paramIndex)
        {
            return NativeMethods.sqlite3_bind_null(stm, paramIndex);
        }

        public static int sqlite3_bind_parameter_count(IntPtr stm)
        {
            return NativeMethods.sqlite3_bind_parameter_count(stm);
        }

        public static string sqlite3_bind_parameter_name(IntPtr stm, int paramIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_bind_parameter_name(stm, paramIndex));
        }

        public static int sqlite3_bind_parameter_index(IntPtr stm, string paramName)
        {
            return NativeMethods.sqlite3_bind_parameter_index(stm, util.to_utf8(paramName));
        }

        public static int sqlite3_step(IntPtr stm)
        {
            return NativeMethods.sqlite3_step(stm);
        }

        public static int sqlite3_stmt_busy(IntPtr stm)
        {
            return NativeMethods.sqlite3_stmt_busy(stm);
        }

        public static int sqlite3_stmt_readonly(IntPtr stm)
        {
            return NativeMethods.sqlite3_stmt_readonly(stm);
        }

        public static int sqlite3_column_int(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_int(stm, columnIndex);
        }

        public static long sqlite3_column_int64(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_int64(stm, columnIndex);
        }

        public static string sqlite3_column_text(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_text(stm, columnIndex));
        }

        public static string sqlite3_column_decltype(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_decltype(stm, columnIndex));
        }

        public static double sqlite3_column_double(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_double(stm, columnIndex);
        }

        public static byte[] sqlite3_column_blob(IntPtr stm, int columnIndex)
        {
            IntPtr blobPointer = NativeMethods.sqlite3_column_blob(stm, columnIndex);
            if (blobPointer == IntPtr.Zero)
            {
                return null;
            }

            var length = NativeMethods.sqlite3_column_bytes(stm, columnIndex);
            byte[] result = new byte[length];
            Marshal.Copy(blobPointer, (byte[])result, 0, length);
            return result;
        }

        public static int sqlite3_column_blob(IntPtr stm, int columnIndex, byte[] result, int offset)
        {
            if (result == null || offset >= result.Length)
            {
                return raw.SQLITE_ERROR;
            }
            IntPtr blobPointer = NativeMethods.sqlite3_column_blob(stm, columnIndex);
            if (blobPointer == IntPtr.Zero)
            {
                return raw.SQLITE_ERROR;
            }

            var length = NativeMethods.sqlite3_column_bytes(stm, columnIndex);
            if (offset + length > result.Length)
            {
                return raw.SQLITE_ERROR;
            }
            Marshal.Copy(blobPointer, (byte[])result, offset, length);
            return raw.SQLITE_OK;
        }

        public static int sqlite3_column_type(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_type(stm, columnIndex);
        }

        public static int sqlite3_column_bytes(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_bytes(stm, columnIndex);
        }

        public static int sqlite3_column_count(IntPtr stm)
        {
            return NativeMethods.sqlite3_column_count(stm);
        }

        public static int sqlite3_data_count(IntPtr stm)
        {
            return NativeMethods.sqlite3_data_count(stm);
        }

        public static string sqlite3_column_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_name(stm, columnIndex));
        }

        public static string sqlite3_column_origin_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_origin_name(stm, columnIndex));
        }

        public static string sqlite3_column_table_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_table_name(stm, columnIndex));
        }

        public static string sqlite3_column_database_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_database_name(stm, columnIndex));
        }

        public static int sqlite3_reset(IntPtr stm)
        {
            return NativeMethods.sqlite3_reset(stm);
        }

        public static int sqlite3_clear_bindings(IntPtr stm)
        {
            return NativeMethods.sqlite3_clear_bindings(stm);
        }

        public static int sqlite3_stmt_status(IntPtr stm, int op, int resetFlg)
        {
            return NativeMethods.sqlite3_stmt_status(stm, op, resetFlg);
        }

        public static int sqlite3_finalize(IntPtr stm)
        {
            return NativeMethods.sqlite3_finalize(stm);
        }

        public static int sqlite3_wal_autocheckpoint(IntPtr db, int n)
        {
            return NativeMethods.sqlite3_wal_autocheckpoint(db, n);
        }

        public static int sqlite3_wal_checkpoint(IntPtr db, string dbName)
        {
            return NativeMethods.sqlite3_wal_checkpoint(db, util.to_utf8(dbName));
        }

        public static int sqlite3_wal_checkpoint_v2(IntPtr db, string dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            return NativeMethods.sqlite3_wal_checkpoint_v2(db, util.to_utf8(dbName), eMode, out logSize, out framesCheckPointed);
        }

    }
}
