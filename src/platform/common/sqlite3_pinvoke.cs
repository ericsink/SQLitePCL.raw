/*
   Copyright 2014 Zumero, LLC

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

    /// <summary>
    /// Implements the <see cref="ISQLite3Provider"/> interface for .Net45 Framework.
    /// </summary>
#if __ANDROID__
    [Android.Runtime.Preserve(AllMembers=true)]
#elif PLATFORM_IOS
	[MonoTouch.Foundation.Preserve(AllMembers = true)]
#endif
    public sealed class SQLite3Provider : ISQLite3Provider
    {
        public SQLite3Provider()
        {
#if NETFX_CORE
            // FYI, the wp8 code does this same thing except using PRAGMAs

			NativeMethods.sqlite3_win32_set_directory(/*data directory type*/1, Windows.Storage.ApplicationData.Current.LocalFolder.Path);
			NativeMethods.sqlite3_win32_set_directory(/*temp directory type*/2, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path);
#endif
        }

        int ISQLite3Provider.sqlite3_open(string filename, out IntPtr db)
        {
            return NativeMethods.sqlite3_open(util.to_utf8(filename), out db);
        }

        int ISQLite3Provider.sqlite3_open_v2(string filename, out IntPtr db, int flags, string vfs)
        {
            return NativeMethods.sqlite3_open_v2(util.to_utf8(filename), out db, flags, util.to_utf8(vfs));
        }

        int ISQLite3Provider.sqlite3_close_v2(IntPtr db)
        {
            return NativeMethods.sqlite3_close_v2(db);
        }

        int ISQLite3Provider.sqlite3_close(IntPtr db)
        {
            return NativeMethods.sqlite3_close(db);
        }

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_exec))] // TODO not xplat
#endif
        static int exec_hook_bridge(IntPtr p, int n, IntPtr values_ptr, IntPtr names_ptr)
        {
            exec_hook_info hi = exec_hook_info.from_ptr(p);
            return hi.call(n, values_ptr, names_ptr);
        }

        int ISQLite3Provider.sqlite3_exec(IntPtr db, string sql, delegate_exec func, object user_data, out string errMsg)
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

        int ISQLite3Provider.sqlite3_complete(string sql)
        {
            return NativeMethods.sqlite3_complete(util.to_utf8(sql));
        }

        string ISQLite3Provider.sqlite3_compileoption_get(int n)
        {
            return util.from_utf8(NativeMethods.sqlite3_compileoption_get(n));
        }

        int ISQLite3Provider.sqlite3_compileoption_used(string s)
        {
            return NativeMethods.sqlite3_compileoption_used(util.to_utf8(s));
        }

        int ISQLite3Provider.sqlite3_prepare_v2(IntPtr db, string sql, out IntPtr stm, out string remain)
        {
            IntPtr tail;
            int rc = NativeMethods.sqlite3_prepare_v2(db, util.to_utf8(sql), -1, out stm, out tail);
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
            return rc;
        }

        string ISQLite3Provider.sqlite3_sql(IntPtr stmt)
        {
            return util.from_utf8(NativeMethods.sqlite3_sql(stmt));
        }

        IntPtr ISQLite3Provider.sqlite3_db_handle(IntPtr stmt)
        {
            return NativeMethods.sqlite3_db_handle(stmt);
        }

        int ISQLite3Provider.sqlite3_blob_open(IntPtr db, string sdb, string table, string col, long rowid, int flags, out IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_open(db, util.to_utf8(sdb), util.to_utf8(table), util.to_utf8(col), rowid, flags, out blob);
        }

        int ISQLite3Provider.sqlite3_blob_bytes(IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_bytes(blob);
        }

        int ISQLite3Provider.sqlite3_blob_close(IntPtr blob)
        {
            return NativeMethods.sqlite3_blob_close(blob);
        }

        int ISQLite3Provider.sqlite3_blob_read(IntPtr blob, byte[] b, int n, int offset)
        {
            return NativeMethods.sqlite3_blob_read(blob, b, n, offset);
        }

        int ISQLite3Provider.sqlite3_blob_write(IntPtr blob, byte[] b, int n, int offset)
        {
            return NativeMethods.sqlite3_blob_write(blob, b, n, offset);
        }

        IntPtr ISQLite3Provider.sqlite3_backup_init(IntPtr destDb, string destName, IntPtr sourceDb, string sourceName)
        {
            return NativeMethods.sqlite3_backup_init(destDb, util.to_utf8(destName), sourceDb, util.to_utf8(sourceName));
        }

        int ISQLite3Provider.sqlite3_backup_step(IntPtr backup, int nPage)
        {
            return NativeMethods.sqlite3_backup_step(backup, nPage);
        }

        int ISQLite3Provider.sqlite3_backup_finish(IntPtr backup)
        {
            return NativeMethods.sqlite3_backup_finish(backup);
        }

        int ISQLite3Provider.sqlite3_backup_remaining(IntPtr backup)
        {
            return NativeMethods.sqlite3_backup_remaining(backup);
        }

        int ISQLite3Provider.sqlite3_backup_pagecount(IntPtr backup)
        {
            return NativeMethods.sqlite3_backup_pagecount(backup);
        }

        IntPtr ISQLite3Provider.sqlite3_next_stmt(IntPtr db, IntPtr stmt)
        {
            return NativeMethods.sqlite3_next_stmt(db, stmt);
        }

        long ISQLite3Provider.sqlite3_last_insert_rowid(IntPtr db)
        {
            return NativeMethods.sqlite3_last_insert_rowid(db);
        }

        int ISQLite3Provider.sqlite3_changes(IntPtr db)
        {
            return NativeMethods.sqlite3_changes(db);
        }

        int ISQLite3Provider.sqlite3_extended_result_codes(IntPtr db, int onoff)
        {
            return NativeMethods.sqlite3_extended_result_codes(db, onoff);
        }

        string ISQLite3Provider.sqlite3_errstr(int rc)
        {
            return util.from_utf8(NativeMethods.sqlite3_errstr(rc));
        }

        int ISQLite3Provider.sqlite3_errcode(IntPtr db)
        {
            return NativeMethods.sqlite3_errcode(db);
        }

        int ISQLite3Provider.sqlite3_extended_errcode(IntPtr db)
        {
            return NativeMethods.sqlite3_extended_errcode(db);
        }

        int ISQLite3Provider.sqlite3_busy_timeout(IntPtr db, int ms)
        {
            return NativeMethods.sqlite3_busy_timeout(db, ms);
        }

        int ISQLite3Provider.sqlite3_get_autocommit(IntPtr db)
        {
            return NativeMethods.sqlite3_get_autocommit(db);
        }

        string ISQLite3Provider.sqlite3_errmsg(IntPtr db)
        {
            return util.from_utf8(NativeMethods.sqlite3_errmsg(db));
        }

        string ISQLite3Provider.sqlite3_libversion()
        {
            return util.from_utf8(NativeMethods.sqlite3_libversion());
        }

        int ISQLite3Provider.sqlite3_libversion_number()
        {
            return NativeMethods.sqlite3_libversion_number();
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
        
        private commit_hook_info _commit_hook;

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_commit))] // TODO not xplat
#endif
        static int commit_hook_bridge(IntPtr p)
        {
            commit_hook_info hi = commit_hook_info.from_ptr(p);
            return hi.call();
        }

        void ISQLite3Provider.sqlite3_commit_hook(IntPtr db, delegate_commit func, object v)
        {
            if (_commit_hook != null)
            {
                // TODO maybe turn off the hook here, for now
                _commit_hook.free();
                _commit_hook = null;
            }

            if (func != null)
            {
                _commit_hook = new commit_hook_info(func, v);
                NativeMethods.sqlite3_commit_hook(db, commit_hook_bridge, _commit_hook.ptr);
            }
            else
            {
                NativeMethods.sqlite3_commit_hook(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        // the keys for this dictionary are nargs.name, not just the name
        private Dictionary<string, scalar_function_hook_info> _scalar_functions = new Dictionary<string, scalar_function_hook_info>();

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_scalar_function))] // TODO not xplat
#endif
        static void scalar_function_hook_bridge(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr p = NativeMethods.sqlite3_user_data(context);
            scalar_function_hook_info hi = scalar_function_hook_info.from_ptr(p);
            hi.call(context, num_args, argsptr);
        }

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_scalar func)
        {
            string key = string.Format("{0}.{1}", nargs, name);
            if (_scalar_functions.ContainsKey(key))
            {
                scalar_function_hook_info hi = _scalar_functions[key];

                // TODO maybe turn off the hook here, for now
                hi.free();

                _scalar_functions.Remove(key);
            }

            // 1 is SQLITE_UTF8
            if (func != null)
            {
                scalar_function_hook_info hi = new scalar_function_hook_info(func, v);
                int rc = NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, 1, hi.ptr, scalar_function_hook_bridge, null, null, null);
                if (rc == 0)
                {
                    _scalar_functions[key] = hi;
                }
                return rc;
            }
            else
            {
                return NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, 1, IntPtr.Zero, null, null, null, null);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        // the keys for this dictionary are nargs.name, not just the name
        private Dictionary<string, agg_function_hook_info> _agg_functions = new Dictionary<string, agg_function_hook_info>();

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_agg_function_step))] // TODO not xplat
#endif
        static void agg_function_hook_bridge_step(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            agg_function_hook_info hi = agg_function_hook_info.from_ptr(p);
            hi.call_step(context, agg, num_args, argsptr);
        }

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_agg_function_final))] // TODO not xplat
#endif
        static void agg_function_hook_bridge_final(IntPtr context)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            agg_function_hook_info hi = agg_function_hook_info.from_ptr(p);
            hi.call_final(context, agg);
        }

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
            string key = string.Format("{0}.{1}", nargs, name);
            if (_agg_functions.ContainsKey(key))
            {
                agg_function_hook_info hi = _agg_functions[key];

                // TODO maybe turn off the hook here, for now
                hi.free();

                _agg_functions.Remove(key);
            }

            // 1 is SQLITE_UTF8
            if (func_step != null)
            {
                // TODO both func_step and func_final must be non-null
                agg_function_hook_info hi = new agg_function_hook_info(func_step, func_final, v);
                int rc = NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, 1, hi.ptr, null, agg_function_hook_bridge_step, agg_function_hook_bridge_final, null);
                if (rc == 0)
                {
                    _agg_functions[key] = hi;
                }
                return rc;
            }
            else
            {
                return NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, 1, IntPtr.Zero, null, null, null, null);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private Dictionary<string, collation_hook_info> _collation_hooks = new Dictionary<string, collation_hook_info>();

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_collation))] // TODO not xplat
#endif
        static int collation_hook_bridge(IntPtr p, int len1, IntPtr pv1, int len2, IntPtr pv2)
        {
            collation_hook_info hi = collation_hook_info.from_ptr(p);
            return hi.call(util.from_utf8(pv1, len1), util.from_utf8(pv2, len2));
        }

        int ISQLite3Provider.sqlite3_create_collation(IntPtr db, string name, object v, delegate_collation func)
        {
            if (_collation_hooks.ContainsKey(name))
            {
                collation_hook_info hi = _collation_hooks[name];

                // TODO maybe turn off the hook here, for now
                hi.free();

                _collation_hooks.Remove(name);
            }

            // 1 is SQLITE_UTF8
            if (func != null)
            {
                collation_hook_info hi = new collation_hook_info(func, v);
                int rc = NativeMethods.sqlite3_create_collation(db, util.to_utf8(name), 1, hi.ptr, collation_hook_bridge);
                if (rc == 0)
                {
                    _collation_hooks[name] = hi;
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

        private update_hook_info _update_hook;

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_update))] // TODO not xplat
#endif
        static void update_hook_bridge(IntPtr p, int typ, IntPtr db, IntPtr tbl, Int64 rowid)
        {
            update_hook_info hi = update_hook_info.from_ptr(p);
            hi.call(typ, util.from_utf8(db), util.from_utf8(tbl), rowid);
        }

        void ISQLite3Provider.sqlite3_update_hook(IntPtr db, delegate_update func, object v)
        {
            if (_update_hook != null)
            {
                // TODO maybe turn off the hook here, for now
                _update_hook.free();
                _update_hook = null;
            }

            if (func != null)
            {
                _update_hook = new update_hook_info(func, v);
                NativeMethods.sqlite3_update_hook(db, update_hook_bridge, _update_hook.ptr);
            }
            else
            {
                NativeMethods.sqlite3_update_hook(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private rollback_hook_info _rollback_hook;

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_rollback))] // TODO not xplat
#endif
        static void rollback_hook_bridge(IntPtr p)
        {
            rollback_hook_info hi = rollback_hook_info.from_ptr(p);
            hi.call();
        }

        void ISQLite3Provider.sqlite3_rollback_hook(IntPtr db, delegate_rollback func, object v)
        {
            if (_rollback_hook != null)
            {
                // TODO maybe turn off the hook here, for now
                _rollback_hook.free();
                _rollback_hook = null;
            }

            if (func != null)
            {
                _rollback_hook = new rollback_hook_info(func, v);
                NativeMethods.sqlite3_rollback_hook(db, rollback_hook_bridge, _rollback_hook.ptr);
            }
            else
            {
                NativeMethods.sqlite3_rollback_hook(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private trace_hook_info _trace_hook;

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_trace))] // TODO not xplat
#endif
        static void trace_hook_bridge(IntPtr p, IntPtr s)
        {
            trace_hook_info hi = trace_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s));
        }

        void ISQLite3Provider.sqlite3_trace(IntPtr db, delegate_trace func, object v)
        {
            if (_trace_hook != null)
            {
                // TODO maybe turn off the hook here, for now
                _trace_hook.free();
                _trace_hook = null;
            }

            if (func != null)
            {
                _trace_hook = new trace_hook_info(func, v);
                NativeMethods.sqlite3_trace(db, trace_hook_bridge, _trace_hook.ptr);
            }
            else
            {
                NativeMethods.sqlite3_trace(db, null, IntPtr.Zero);
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private profile_hook_info _profile_hook;

#if PLATFORM_IOS
        [MonoTouch.MonoPInvokeCallback (typeof(NativeMethods.callback_profile))] // TODO not xplat
#endif
        static void profile_hook_bridge(IntPtr p, IntPtr s, long elapsed)
        {
            profile_hook_info hi = profile_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s), elapsed);
        }

        void ISQLite3Provider.sqlite3_profile(IntPtr db, delegate_profile func, object v)
        {
            if (_profile_hook != null)
            {
                // TODO maybe turn off the hook here, for now
                _profile_hook.free();
                _profile_hook = null;
            }

            if (func != null)
            {
                _profile_hook = new profile_hook_info(func, v);
                NativeMethods.sqlite3_profile(db, profile_hook_bridge, _profile_hook.ptr);
            }
            else
            {
                NativeMethods.sqlite3_profile(db, null, IntPtr.Zero);
            }
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

        string ISQLite3Provider.sqlite3_sourceid()
        {
            return util.from_utf8(NativeMethods.sqlite3_sourceid());
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

        void ISQLite3Provider.sqlite3_result_error(IntPtr ctx, string val)
        {
            NativeMethods.sqlite3_result_error(ctx, util.to_utf8(val), -1);
        }

        void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, string val)
        {
            NativeMethods.sqlite3_result_text(ctx, util.to_utf8(val), -1, new IntPtr(-1));
        }

        void ISQLite3Provider.sqlite3_result_blob(IntPtr ctx, byte[] blob)
        {
            NativeMethods.sqlite3_result_blob(ctx, blob, blob.Length, new IntPtr(-1));
        }

        byte[] ISQLite3Provider.sqlite3_value_blob(IntPtr p)
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

        string ISQLite3Provider.sqlite3_value_text(IntPtr p)
        {
            return util.from_utf8(NativeMethods.sqlite3_value_text(p));
        }

        int ISQLite3Provider.sqlite3_bind_int(IntPtr stm, int paramIndex, int val)
        {
            return NativeMethods.sqlite3_bind_int(stm, paramIndex, val);
        }

        int ISQLite3Provider.sqlite3_bind_int64(IntPtr stm, int paramIndex, long val)
        {
            return NativeMethods.sqlite3_bind_int64(stm, paramIndex, val);
        }

        int ISQLite3Provider.sqlite3_bind_text(IntPtr stm, int paramIndex, string t)
        {
            return NativeMethods.sqlite3_bind_text(stm, paramIndex, util.to_utf8(t), -1, new IntPtr(-1));
        }

        int ISQLite3Provider.sqlite3_bind_double(IntPtr stm, int paramIndex, double val)
        {
            return NativeMethods.sqlite3_bind_double(stm, paramIndex, val);
        }

        int ISQLite3Provider.sqlite3_bind_blob(IntPtr stm, int paramIndex, byte[] blob)
        {
            return NativeMethods.sqlite3_bind_blob(stm, paramIndex, blob, blob.Length, new IntPtr(-1));
        }

        int ISQLite3Provider.sqlite3_bind_zeroblob(IntPtr stm, int paramIndex, int size)
        {
            return NativeMethods.sqlite3_bind_zeroblob(stm, paramIndex, size);
        }

        int ISQLite3Provider.sqlite3_bind_null(IntPtr stm, int paramIndex)
        {
            return NativeMethods.sqlite3_bind_null(stm, paramIndex);
        }

        int ISQLite3Provider.sqlite3_bind_parameter_count(IntPtr stm)
        {
            return NativeMethods.sqlite3_bind_parameter_count(stm);
        }

        string ISQLite3Provider.sqlite3_bind_parameter_name(IntPtr stm, int paramIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_bind_parameter_name(stm, paramIndex));
        }

        int ISQLite3Provider.sqlite3_bind_parameter_index(IntPtr stm, string paramName)
        {
            return NativeMethods.sqlite3_bind_parameter_index(stm, util.to_utf8(paramName));
        }

        int ISQLite3Provider.sqlite3_step(IntPtr stm)
        {
            return NativeMethods.sqlite3_step(stm);
        }

        int ISQLite3Provider.sqlite3_stmt_busy(IntPtr stm)
        {
            return NativeMethods.sqlite3_stmt_busy(stm);
        }

        int ISQLite3Provider.sqlite3_stmt_readonly(IntPtr stm)
        {
            return NativeMethods.sqlite3_stmt_readonly(stm);
        }

        int ISQLite3Provider.sqlite3_column_int(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_int(stm, columnIndex);
        }

        long ISQLite3Provider.sqlite3_column_int64(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_int64(stm, columnIndex);
        }

        string ISQLite3Provider.sqlite3_column_text(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_text(stm, columnIndex));
        }

        string ISQLite3Provider.sqlite3_column_decltype(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_decltype(stm, columnIndex));
        }

        double ISQLite3Provider.sqlite3_column_double(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_double(stm, columnIndex);
        }

        byte[] ISQLite3Provider.sqlite3_column_blob(IntPtr stm, int columnIndex)
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

        int ISQLite3Provider.sqlite3_column_type(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_type(stm, columnIndex);
        }

        int ISQLite3Provider.sqlite3_column_bytes(IntPtr stm, int columnIndex)
        {
            return NativeMethods.sqlite3_column_bytes(stm, columnIndex);
        }

        int ISQLite3Provider.sqlite3_column_count(IntPtr stm)
        {
            return NativeMethods.sqlite3_column_count(stm);
        }

        int ISQLite3Provider.sqlite3_data_count(IntPtr stm)
        {
            return NativeMethods.sqlite3_data_count(stm);
        }

        string ISQLite3Provider.sqlite3_column_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_name(stm, columnIndex));
        }

        string ISQLite3Provider.sqlite3_column_origin_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_origin_name(stm, columnIndex));
        }

        string ISQLite3Provider.sqlite3_column_table_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_table_name(stm, columnIndex));
        }

        string ISQLite3Provider.sqlite3_column_database_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(NativeMethods.sqlite3_column_database_name(stm, columnIndex));
        }

        int ISQLite3Provider.sqlite3_reset(IntPtr stm)
        {
            return NativeMethods.sqlite3_reset(stm);
        }

        int ISQLite3Provider.sqlite3_clear_bindings(IntPtr stm)
        {
            return NativeMethods.sqlite3_clear_bindings(stm);
        }

        int ISQLite3Provider.sqlite3_finalize(IntPtr stm)
        {
            return NativeMethods.sqlite3_finalize(stm);
        }

        private static class NativeMethods
        {
#if PINVOKE_FROM_INTERNAL
        private const string SQLITE_DLL = "__Internal";
#elif PINVOKE_FROM_SQLITE3
		private const string SQLITE_DLL = "sqlite3";
#elif PINVOKE_FROM_SQLITE3_DLL
		private const string SQLITE_DLL = "sqlite3.dll";
#endif

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_close(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_close_v2(IntPtr db); /* 3.7.14+ */

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_finalize(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_reset(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_clear_bindings(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_bind_parameter_name(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_database_name(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_database_name16(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_decltype(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_decltype16(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_name(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_name16(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_origin_name(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_origin_name16(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_table_name(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_table_name16(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_text(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_text16(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_errmsg(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_prepare(IntPtr db, IntPtr pSql, int nBytes, out IntPtr stmt, out IntPtr ptrRemain);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_prepare_v2(IntPtr db, byte[] pSql, int nBytes, out IntPtr stmt, out IntPtr ptrRemain);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_complete(byte[] pSql);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_compileoption_used(byte[] pSql);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_compileoption_get(int n);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_table_column_metadata(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, out IntPtr ptrDataType, out IntPtr ptrCollSeq, out int notNull, out int primaryKey, out int autoInc);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_value_text(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_value_text16(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_enable_load_extension(
            IntPtr db, int enable);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_load_extension(
            IntPtr db, byte[] fileName, byte[] procName, ref IntPtr pError);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_libversion();

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_libversion_number();

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_sourceid();

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_malloc(int n);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_realloc(IntPtr p, int n);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_free(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_open(byte[] filename, out IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, byte[] vfs);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public static extern int sqlite3_open16(string fileName, out IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_interrupt(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern long sqlite3_last_insert_rowid(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_changes(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern long sqlite3_memory_used();

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern long sqlite3_memory_highwater(int resetFlag);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_shutdown();

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_busy_timeout(IntPtr db, int ms);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_bind_blob(IntPtr stmt, int index, byte[] val, int nSize, IntPtr nTransient);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_bind_zeroblob(IntPtr stmt, int index, int size);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_bind_double(IntPtr stmt, int index, double val);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_bind_int(IntPtr stmt, int index, int val);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_bind_int64(IntPtr stmt, int index, long val);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_bind_null(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_bind_text(IntPtr stmt, int index, byte[] val, int nlen, IntPtr pvReserved);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_bind_parameter_count(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_bind_parameter_index(IntPtr stmt, byte[] strName);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_column_count(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_data_count(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_step(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_sql(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern double sqlite3_column_double(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_column_int(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern long sqlite3_column_int64(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_column_blob(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_column_bytes(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_column_type(IntPtr stmt, int index);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_aggregate_count(IntPtr context);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_value_blob(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_value_bytes(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern double sqlite3_value_double(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_value_int(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern long sqlite3_value_int64(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_value_type(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_user_data(IntPtr context);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_blob(IntPtr context, byte[] val, int nSize, IntPtr pvReserved);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_double(IntPtr context, double val);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_error(IntPtr context, byte[] strErr, int nLen);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_int(IntPtr context, int val);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_int64(IntPtr context, long val);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_null(IntPtr context);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_text(IntPtr context, byte[] val, int nLen, IntPtr pvReserved);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public static extern int sqlite3_bind_text16(IntPtr stmt, int index, string val, int nlen, IntPtr pvReserved);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public static extern void sqlite3_result_error16(IntPtr context, string strName, int nLen);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public static extern void sqlite3_result_text16(IntPtr context, string strName, int nLen, IntPtr pvReserved);

#if not // TODO removed, perhaps temporarily.  not sure if we want these or PRAGMA key.
            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_key(IntPtr db, byte[] key, int keylen);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_rekey(IntPtr db, byte[] key, int keylen);
#endif

            // Since sqlite3_config() takes a variable argument list, we have to overload declarations
            // for all possible calls that we want to use.
            [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_config_none(int op);

            [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_config_int(int op, int val);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void callback_log(IntPtr pUserData, int errorCode, IntPtr pMessage);

            [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_config_log(int op, callback_log func, IntPtr pvUser);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void callback_agg_function_final(IntPtr context);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void callback_scalar_function(IntPtr context, int nArgs, IntPtr argsptr);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void callback_agg_function_step(IntPtr context, int nArgs, IntPtr argsptr);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void callback_destroy(IntPtr p);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_create_function_v2(IntPtr db, byte[] strName, int nArgs, int nType, IntPtr pvUser, callback_scalar_function func, callback_agg_function_step fstep, callback_agg_function_final ffinal, callback_destroy fdestroy);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int callback_collation(IntPtr puser, int len1, IntPtr pv1, int len2, IntPtr pv2);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_create_collation(IntPtr db, byte[] strName, int nType, IntPtr pvUser, callback_collation func);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void callback_update(IntPtr p, int typ, IntPtr db, IntPtr tbl, long rowid);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_update_hook(IntPtr db, callback_update func, IntPtr pvUser);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int callback_commit(IntPtr puser);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_commit_hook(IntPtr db, callback_commit func, IntPtr pvUser);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void callback_profile(IntPtr puser, IntPtr statement, long elapsed);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_profile(IntPtr db, callback_profile func, IntPtr pvUser);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void callback_trace(IntPtr puser, IntPtr statement);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_trace(IntPtr db, callback_trace func, IntPtr pvUser);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void callback_rollback(IntPtr puser);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_rollback_hook(IntPtr db, callback_rollback func, IntPtr pvUser);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_db_handle(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_stmt_busy(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_stmt_readonly(IntPtr stmt);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int callback_exec(IntPtr db, int n, IntPtr values, IntPtr names);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_exec(IntPtr db, byte[] strSql, callback_exec cb, IntPtr pvParam, out IntPtr errMsg);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_get_autocommit(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_extended_result_codes(IntPtr db, int onoff);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_errcode(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_extended_errcode(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_errstr(int rc); /* 3.7.15+ */

            // Since sqlite3_log() takes a variable argument list, we have to overload declarations
            // for all possible calls.  For now, we are only exposing a single string, and 
            // depend on the caller to format the string.
            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_log(int iErrCode, byte[] zFormat);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_file_control(IntPtr db, byte[] zDbName, int op, IntPtr pArg);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_backup_init(IntPtr destDb, byte[] zDestName, IntPtr sourceDb, byte[] zSourceName);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_backup_step(IntPtr backup, int nPage);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_backup_finish(IntPtr backup);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_backup_remaining(IntPtr backup);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_backup_pagecount(IntPtr backup);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_blob_open(IntPtr db, byte[] sdb, byte[] table, byte[] col, long rowid, int flags, out IntPtr blob);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_blob_write(IntPtr blob, byte[] b, int n, int offset);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_blob_read(IntPtr blob, byte[] b, int n, int offset);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_blob_bytes(IntPtr blob);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_blob_close(IntPtr blob);

#if NETFX_CORE
            [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_win32_set_directory", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
            public static extern int sqlite3_win32_set_directory (uint directoryType, string directoryPath);
#endif

        }
    }
}
