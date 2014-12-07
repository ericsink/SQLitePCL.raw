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
    using SQLitePCL.cppinterop;
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;

    /// <summary>
    /// Implements the <see cref="ISQLite3Provider"/> interface
    /// </summary>
    internal sealed class SQLite3Provider : ISQLite3Provider
    {
        public SQLite3Provider()
        {
#if WINDOWS_PHONE
            IntPtr db;
            int rc;

            ISQLite3Provider me = this as ISQLite3Provider;
            rc = me.sqlite3_open(":memory:", out db);
            if (0 == rc)
            {
                string errmsg;

                rc = me.sqlite3_exec(db, 
                        string.Format("PRAGMA temp_store_directory = '{0}';", 
                            Windows.Storage.ApplicationData.Current.LocalFolder.Path), 
                        null, null, out errmsg);
                // ignore rc, I guess

                rc = me.sqlite3_exec(db, 
                        string.Format("PRAGMA data_store_directory = '{0}';", 
                            Windows.Storage.ApplicationData.Current.LocalFolder.Path), 
                        null, null, out errmsg);
                // ignore rc, I guess

                rc = me.sqlite3_close(db);
                // ignore rc, I guess
            }
#elif NETFX_CORE
            IntPtr db;
            int rc;

            ISQLite3Provider me = this as ISQLite3Provider;
            rc = me.sqlite3_open(":memory:", out db);
            if (0 == rc)
            {
                string errmsg;

                rc = me.sqlite3_exec(db, 
                        string.Format("PRAGMA temp_store_directory = '{0}';", 
                            Windows.Storage.ApplicationData.Current.TemporaryFolder.Path), 
                        null, null, out errmsg);
                // ignore rc, I guess

                rc = me.sqlite3_exec(db, 
                        string.Format("PRAGMA data_store_directory = '{0}';", 
                            Windows.Storage.ApplicationData.Current.LocalFolder.Path), 
                        null, null, out errmsg);
                // ignore rc, I guess

                rc = me.sqlite3_close(db);
                // ignore rc, I guess
            }
#endif
        }

        int ISQLite3Provider.sqlite3_open(string filename, out IntPtr db)
        {
            // TODO null string?
            GCHandle filename_pinned = GCHandle.Alloc(util.to_utf8(filename), GCHandleType.Pinned);
            IntPtr filename_ptr = filename_pinned.AddrOfPinnedObject();

            byte[] bufdb = new byte[8];
            GCHandle bufdb_pinned = GCHandle.Alloc(bufdb, GCHandleType.Pinned);
            IntPtr bufdb_ptr = bufdb_pinned.AddrOfPinnedObject();

            var result = SQLite3RuntimeProvider.sqlite3_open(filename_ptr.ToInt64(), bufdb_ptr.ToInt64());
            filename_pinned.Free();

            db = new IntPtr(Marshal.ReadInt64(bufdb_ptr));
            bufdb_pinned.Free();

            return result;
        }

        int ISQLite3Provider.sqlite3_open_v2(string filename, out IntPtr db, int flags, string vfs)
        {
            // TODO null string?
            GCHandle filename_pinned = GCHandle.Alloc(util.to_utf8(filename), GCHandleType.Pinned);
            IntPtr filename_ptr = filename_pinned.AddrOfPinnedObject();

            // TODO null string?
            GCHandle vfs_pinned = GCHandle.Alloc(util.to_utf8(vfs), GCHandleType.Pinned);
            IntPtr vfs_ptr = vfs_pinned.AddrOfPinnedObject();

            byte[] bufdb = new byte[8];
            GCHandle bufdb_pinned = GCHandle.Alloc(bufdb, GCHandleType.Pinned);
            IntPtr bufdb_ptr = bufdb_pinned.AddrOfPinnedObject();

            var result = SQLite3RuntimeProvider.sqlite3_open_v2(filename_ptr.ToInt64(), bufdb_ptr.ToInt64(), flags, vfs_ptr.ToInt64());

            filename_pinned.Free();
            vfs_pinned.Free();

            db = new IntPtr(Marshal.ReadInt64(bufdb_ptr));
            bufdb_pinned.Free();

            return result;
        }

        int ISQLite3Provider.sqlite3_close_v2(IntPtr db)
        {
            return SQLite3RuntimeProvider.sqlite3_close_v2(db.ToInt64());
        }
        
        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private commit_hook_info _commit_hook;

        static private int commit_hook_bridge(IntPtr p)
        {
            commit_hook_info hi = commit_hook_info.from_ptr(p);
            return hi.call();
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate int callback_commit(IntPtr p);

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
                SQLite3RuntimeProvider.sqlite3_commit_hook(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(new callback_commit(commit_hook_bridge)).ToInt64(), _commit_hook.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_commit_hook(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private Dictionary<string, collation_hook_info> _collation_hooks = new Dictionary<string, collation_hook_info>();

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate int callback_collation(IntPtr puser, int len1, IntPtr pv1, int len2, IntPtr pv2);

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

            GCHandle pinned = GCHandle.Alloc(util.to_utf8(name), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();

            int rc;

            // 1 is SQLITE_UTF8
            if (func != null)
            {
                collation_hook_info hi = new collation_hook_info(func, v);
                rc = SQLite3RuntimeProvider.sqlite3_create_collation(db.ToInt64(), ptr.ToInt64(), 1, hi.ptr.ToInt64(), Marshal.GetFunctionPointerForDelegate(new callback_collation(collation_hook_bridge)).ToInt64() );
                if (rc == 0)
                {
                    _collation_hooks[name] = hi;
                }
            }
            else
            {
                rc = SQLite3RuntimeProvider.sqlite3_create_collation(db.ToInt64(), ptr.ToInt64(), 1, IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }

            pinned.Free();

            return rc;
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        // the keys for this dictionary are nargs.name, not just the name
        private Dictionary<string, scalar_function_hook_info> _scalar_functions = new Dictionary<string, scalar_function_hook_info>();

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_scalar_function(IntPtr context, int nArgs, IntPtr argsptr);

        static void scalar_function_hook_bridge(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr p = new IntPtr(SQLite3RuntimeProvider.sqlite3_user_data(context.ToInt64()));
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

            GCHandle pinned = GCHandle.Alloc(util.to_utf8(name), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();

            int rc;

            // 1 is SQLITE_UTF8
            if (func != null)
            {
                scalar_function_hook_info hi = new scalar_function_hook_info(func, v);
                rc = SQLite3RuntimeProvider.sqlite3_create_function_v2(db.ToInt64(), ptr.ToInt64(), nargs, 1, hi.ptr.ToInt64(), Marshal.GetFunctionPointerForDelegate(new callback_scalar_function(scalar_function_hook_bridge)).ToInt64(), 0, 0, 0);
                if (rc == 0)
                {
                    _scalar_functions[key] = hi;
                }
            }
            else
            {
                rc = SQLite3RuntimeProvider.sqlite3_create_function_v2(db.ToInt64(), ptr.ToInt64(), nargs, 1, 0, 0, 0, 0, 0);
            }

            pinned.Free();

            return rc;
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        // the keys for this dictionary are nargs.name, not just the name
        private Dictionary<string, agg_function_hook_info> _agg_functions = new Dictionary<string, agg_function_hook_info>();

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_agg_function_step(IntPtr context, int nArgs, IntPtr argsptr);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_agg_function_final(IntPtr context);

        static void agg_function_hook_bridge_step(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr agg = new IntPtr(SQLite3RuntimeProvider.sqlite3_aggregate_context(context.ToInt64(), 8));
            // TODO error check agg nomem

            IntPtr p = new IntPtr(SQLite3RuntimeProvider.sqlite3_user_data(context.ToInt64()));
            agg_function_hook_info hi = agg_function_hook_info.from_ptr(p);
            hi.call_step(context, agg, num_args, argsptr);
        }

        static void agg_function_hook_bridge_final(IntPtr context)
        {
            IntPtr agg = new IntPtr(SQLite3RuntimeProvider.sqlite3_aggregate_context(context.ToInt64(), 8));
            // TODO error check agg nomem

            IntPtr p = new IntPtr(SQLite3RuntimeProvider.sqlite3_user_data(context.ToInt64()));
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

            GCHandle pinned = GCHandle.Alloc(util.to_utf8(name), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();

            int rc;

            // 1 is SQLITE_UTF8
            if (func_step != null)
            {
                // TODO both func_step and func_final must be non-null
                agg_function_hook_info hi = new agg_function_hook_info(func_step, func_final, v);
                rc = SQLite3RuntimeProvider.sqlite3_create_function_v2(db.ToInt64(), ptr.ToInt64(), nargs, 1, hi.ptr.ToInt64(), 0, Marshal.GetFunctionPointerForDelegate(new callback_agg_function_step(agg_function_hook_bridge_step)).ToInt64(), Marshal.GetFunctionPointerForDelegate(new callback_agg_function_final(agg_function_hook_bridge_final)).ToInt64(), 0);
                if (rc == 0)
                {
                    _agg_functions[key] = hi;
                }
            }
            else
            {
                rc = SQLite3RuntimeProvider.sqlite3_create_function_v2(db.ToInt64(), ptr.ToInt64(), nargs, 1, 0, 0, 0, 0, 0);
            }

            pinned.Free();

            return rc;
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private rollback_hook_info _rollback_hook;

        static private void rollback_hook_bridge(IntPtr p)
        {
            rollback_hook_info hi = rollback_hook_info.from_ptr(p);
            hi.call();
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_rollback(IntPtr p);

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
                SQLite3RuntimeProvider.sqlite3_rollback_hook(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(new callback_rollback(rollback_hook_bridge)).ToInt64(), _rollback_hook.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_rollback_hook(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private update_hook_info _update_hook;

        static private void update_hook_bridge(IntPtr p, int typ, IntPtr db, IntPtr tbl, long rowid)
        {
            update_hook_info hi = update_hook_info.from_ptr(p);
            hi.call(typ, util.from_utf8(db), util.from_utf8(tbl), rowid);
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_update(IntPtr p, int typ, IntPtr db, IntPtr tbl, long rowid);

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
                SQLite3RuntimeProvider.sqlite3_update_hook(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(new callback_update(update_hook_bridge)).ToInt64(), _update_hook.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_update_hook(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private trace_hook_info _trace_hook;

        static private void trace_hook_bridge(IntPtr p, IntPtr s)
        {
            trace_hook_info hi = trace_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s));
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_trace(IntPtr p, IntPtr s);

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
                SQLite3RuntimeProvider.sqlite3_trace(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(new callback_trace(trace_hook_bridge)).ToInt64(), _trace_hook.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_trace(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        private profile_hook_info _profile_hook;

        static private void profile_hook_bridge(IntPtr p, IntPtr s, long elapsed)
        {
            profile_hook_info hi = profile_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s), elapsed);
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_profile(IntPtr p, IntPtr s, long elapsed);

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
                SQLite3RuntimeProvider.sqlite3_profile(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(new callback_profile(profile_hook_bridge)).ToInt64(), _profile_hook.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_profile(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        int ISQLite3Provider.sqlite3_close(IntPtr db)
        {
            return SQLite3RuntimeProvider.sqlite3_close(db.ToInt64());
        }

        void ISQLite3Provider.sqlite3_interrupt(IntPtr db)
        {
            SQLite3RuntimeProvider.sqlite3_interrupt(db.ToInt64());
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate int callback_exec(IntPtr p, int n, IntPtr values, IntPtr names);

        static int exec_hook_bridge(IntPtr p, int n, IntPtr values_ptr, IntPtr names_ptr)
        {
            exec_hook_info hi = exec_hook_info.from_ptr(p);
            return hi.call(n, values_ptr, names_ptr);
        }

        int ISQLite3Provider.sqlite3_exec(IntPtr db, string sql, delegate_exec func, object user_data, out string errMsg)
        {
            // TODO null string?
            GCHandle pinned = GCHandle.Alloc(util.to_utf8(sql), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();

            int rc;

            byte[] buf8 = new byte[8];
            GCHandle buf8_pinned = GCHandle.Alloc(buf8, GCHandleType.Pinned);
            IntPtr buf8_ptr = buf8_pinned.AddrOfPinnedObject();

            if (func != null)
            {
                exec_hook_info hi = new exec_hook_info(func, user_data);
                callback_exec cb = new callback_exec(exec_hook_bridge);
                GCHandle h_cb = GCHandle.Alloc(cb);

                rc = SQLite3RuntimeProvider.sqlite3_exec(db.ToInt64(), ptr.ToInt64(), Marshal.GetFunctionPointerForDelegate(cb).ToInt64(), hi.ptr.ToInt64(), buf8_ptr.ToInt64());
                hi.free();
                h_cb.Free();
            }
            else
            {
                rc = SQLite3RuntimeProvider.sqlite3_exec(db.ToInt64(), ptr.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64(), buf8_ptr.ToInt64());
            }

            IntPtr errmsg_ptr = new IntPtr(Marshal.ReadInt64(buf8_ptr));
            buf8_pinned.Free();

            if (errmsg_ptr == IntPtr.Zero)
            {
                errMsg = null;
            }
            else
            {
                errMsg = util.from_utf8(errmsg_ptr);
                SQLite3RuntimeProvider.sqlite3_free(errmsg_ptr.ToInt64());
            }

            pinned.Free();

            return rc;
        }

        string ISQLite3Provider.sqlite3_compileoption_get(int n)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_compileoption_get(n)));
        }

        int ISQLite3Provider.sqlite3_compileoption_used(string s)
        {
            // TODO null string?
            GCHandle pinned = GCHandle.Alloc(util.to_utf8(s), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = SQLite3RuntimeProvider.sqlite3_compileoption_used(ptr.ToInt64());
            pinned.Free();
            return rc;
        }

        int ISQLite3Provider.sqlite3_complete(string sql)
        {
            // TODO null string?
            GCHandle pinned = GCHandle.Alloc(util.to_utf8(sql), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = SQLite3RuntimeProvider.sqlite3_complete(ptr.ToInt64());
            pinned.Free();
            return rc;
        }

        int ISQLite3Provider.sqlite3_prepare_v2(IntPtr db, string sql, out IntPtr stm, out string remain)
        {
            // TODO null string?
            GCHandle pinned = GCHandle.Alloc(util.to_utf8(sql), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();

            byte[] bufstmt = new byte[8];
            GCHandle bufstmt_pinned = GCHandle.Alloc(bufstmt, GCHandleType.Pinned);
            IntPtr bufstmt_ptr = bufstmt_pinned.AddrOfPinnedObject();

            byte[] buftail = new byte[8];
            GCHandle buftail_pinned = GCHandle.Alloc(buftail, GCHandleType.Pinned);
            IntPtr buftail_ptr = buftail_pinned.AddrOfPinnedObject();

            var result = SQLite3RuntimeProvider.sqlite3_prepare_v2(db.ToInt64(), ptr.ToInt64(), -1, bufstmt_ptr.ToInt64(), buftail_ptr.ToInt64());
            pinned.Free();

            stm = new IntPtr(Marshal.ReadInt64(bufstmt_ptr));

            long tailPtr = Marshal.ReadInt64(buftail_ptr);
            if (tailPtr == 0)
            {
                remain = null;
            }
            else
            {
                remain = util.from_utf8(new IntPtr(tailPtr));
                if (remain.Length == 0)
                {
                    remain = null;
                }
            }

            bufstmt_pinned.Free();
            buftail_pinned.Free();

            return result;
        }

        string ISQLite3Provider.sqlite3_sql(IntPtr stmt)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_sql(stmt.ToInt64())));
        }

        IntPtr ISQLite3Provider.sqlite3_db_handle(IntPtr stmt)
        {
            return new IntPtr(SQLite3RuntimeProvider.sqlite3_db_handle(stmt.ToInt64()));
        }

        IntPtr ISQLite3Provider.sqlite3_next_stmt(IntPtr db, IntPtr stmt)
        {
            return new IntPtr(SQLite3RuntimeProvider.sqlite3_next_stmt(db.ToInt64(), stmt.ToInt64()));
        }

        long ISQLite3Provider.sqlite3_last_insert_rowid(IntPtr db)
        {
            return SQLite3RuntimeProvider.sqlite3_last_insert_rowid(db.ToInt64());
        }

        string ISQLite3Provider.sqlite3_libversion()
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_libversion()));
        }

        int ISQLite3Provider.sqlite3_libversion_number()
        {
            return SQLite3RuntimeProvider.sqlite3_libversion_number();
        }

        long ISQLite3Provider.sqlite3_memory_used()
        {
            return SQLite3RuntimeProvider.sqlite3_memory_used();
        }

        long ISQLite3Provider.sqlite3_memory_highwater(int resetFlag)
        {
            return SQLite3RuntimeProvider.sqlite3_memory_highwater(resetFlag);
        }

        string ISQLite3Provider.sqlite3_sourceid()
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_sourceid()));
        }

        string ISQLite3Provider.sqlite3_errstr(int rc)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_errstr(rc)));
        }

        int ISQLite3Provider.sqlite3_errcode(IntPtr db)
        {
            return SQLite3RuntimeProvider.sqlite3_errcode(db.ToInt64());
        }

        int ISQLite3Provider.sqlite3_extended_errcode(IntPtr db)
        {
            return SQLite3RuntimeProvider.sqlite3_extended_errcode(db.ToInt64());
        }

        int ISQLite3Provider.sqlite3_extended_result_codes(IntPtr db, int onoff)
        {
            return SQLite3RuntimeProvider.sqlite3_extended_result_codes(db.ToInt64(), onoff);
        }

        int ISQLite3Provider.sqlite3_changes(IntPtr db)
        {
            return SQLite3RuntimeProvider.sqlite3_changes(db.ToInt64());
        }

        int ISQLite3Provider.sqlite3_busy_timeout(IntPtr db, int ms)
        {
            return SQLite3RuntimeProvider.sqlite3_busy_timeout(db.ToInt64(), ms);
        }

        int ISQLite3Provider.sqlite3_get_autocommit(IntPtr db)
        {
            return SQLite3RuntimeProvider.sqlite3_get_autocommit(db.ToInt64());
        }

	int ISQLite3Provider.sqlite3__vfs__delete(string vfs, string pathname, int syncDir)
	{
            GCHandle pinned_vfs = GCHandle.Alloc(util.to_utf8(vfs), GCHandleType.Pinned);
            IntPtr ptr_vfs = pinned_vfs.AddrOfPinnedObject();

            GCHandle pinned_pathname = GCHandle.Alloc(util.to_utf8(pathname), GCHandleType.Pinned);
            IntPtr ptr_pathname = pinned_pathname.AddrOfPinnedObject();

            int rc = SQLite3RuntimeProvider.sqlite3__vfs__delete(ptr_vfs.ToInt64(), ptr_pathname.ToInt64(), syncDir);

	    pinned_vfs.Free();
	    pinned_pathname.Free();

	    return rc;
	}

        string ISQLite3Provider.sqlite3_db_filename(IntPtr db, string att)
	{
	    string result;

	    if (att != null)
	    {
		    GCHandle pinned = GCHandle.Alloc(util.to_utf8(att), GCHandleType.Pinned);
		    IntPtr ptr = pinned.AddrOfPinnedObject();
		    result = util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_db_filename(db.ToInt64(), ptr.ToInt64())));
		    pinned.Free();
	    }
	    else
	    {
		    result = util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_db_filename(db.ToInt64(), IntPtr.Zero.ToInt64())));
	    }
            return result.Length == 0 ? null : result;
	}

        string ISQLite3Provider.sqlite3_errmsg(IntPtr db)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_errmsg(db.ToInt64())));
        }

        void ISQLite3Provider.sqlite3_result_int64(IntPtr ctx, long val)
        {
            SQLite3RuntimeProvider.sqlite3_result_int64(ctx.ToInt64(), val);
        }

        void ISQLite3Provider.sqlite3_result_int(IntPtr ctx, int val)
        {
            SQLite3RuntimeProvider.sqlite3_result_int(ctx.ToInt64(), val);
        }

        void ISQLite3Provider.sqlite3_result_double(IntPtr ctx, double val)
        {
            SQLite3RuntimeProvider.sqlite3_result_double(ctx.ToInt64(), val);
        }

        void ISQLite3Provider.sqlite3_result_null(IntPtr ctx)
        {
            SQLite3RuntimeProvider.sqlite3_result_null(ctx.ToInt64());
        }

        void ISQLite3Provider.sqlite3_result_error(IntPtr ctx, string val)
        {
            GCHandle pinned = GCHandle.Alloc(util.to_utf8(val), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            SQLite3RuntimeProvider.sqlite3_result_error(ctx.ToInt64(), ptr.ToInt64(), -1);
            pinned.Free();
        }

        void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, string val)
        {
            // TODO null string?
            GCHandle pinned = GCHandle.Alloc(util.to_utf8(val), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            SQLite3RuntimeProvider.sqlite3_result_text(ctx.ToInt64(), ptr.ToInt64(), -1, -1);
            pinned.Free();
        }

        void ISQLite3Provider.sqlite3_result_blob(IntPtr ctx, byte[] blob)
        {
            GCHandle pinned = GCHandle.Alloc(blob, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            SQLite3RuntimeProvider.sqlite3_result_blob(ctx.ToInt64(), ptr.ToInt64(), blob.Length, -1);
            pinned.Free();
        }

        byte[] ISQLite3Provider.sqlite3_value_blob(IntPtr p)
        {
            IntPtr blobPointer = new IntPtr(SQLite3RuntimeProvider.sqlite3_value_blob(p.ToInt64()));
            if (blobPointer == IntPtr.Zero)
            {
                return null;
            }

            var length = SQLite3RuntimeProvider.sqlite3_value_bytes(p.ToInt64());
            byte[] result = new byte[length];
            Marshal.Copy(blobPointer, (byte[])result, 0, length);
            return result;
        }

        int ISQLite3Provider.sqlite3_value_bytes(IntPtr p)
        {
            return SQLite3RuntimeProvider.sqlite3_value_bytes(p.ToInt64());
        }

        double ISQLite3Provider.sqlite3_value_double(IntPtr p)
        {
            return SQLite3RuntimeProvider.sqlite3_value_double(p.ToInt64());
        }

        int ISQLite3Provider.sqlite3_value_int(IntPtr p)
        {
            return SQLite3RuntimeProvider.sqlite3_value_int(p.ToInt64());
        }

        long ISQLite3Provider.sqlite3_value_int64(IntPtr p)
        {
            return SQLite3RuntimeProvider.sqlite3_value_int64(p.ToInt64());
        }

        int ISQLite3Provider.sqlite3_value_type(IntPtr p)
        {
            return SQLite3RuntimeProvider.sqlite3_value_type(p.ToInt64());
        }

        string ISQLite3Provider.sqlite3_value_text(IntPtr p)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_value_text(p.ToInt64())));
        }

        int ISQLite3Provider.sqlite3_blob_open(IntPtr db, string sdb, string table, string col, long rowid, int flags, out IntPtr blob)
        {
            // TODO null string?
            GCHandle sdb_pinned = GCHandle.Alloc(util.to_utf8(sdb), GCHandleType.Pinned);
            IntPtr sdb_ptr = sdb_pinned.AddrOfPinnedObject();

            // TODO null string?
            GCHandle table_pinned = GCHandle.Alloc(util.to_utf8(table), GCHandleType.Pinned);
            IntPtr table_ptr = table_pinned.AddrOfPinnedObject();

            // TODO null string?
            GCHandle col_pinned = GCHandle.Alloc(util.to_utf8(col), GCHandleType.Pinned);
            IntPtr col_ptr = col_pinned.AddrOfPinnedObject();

            byte[] buf8 = new byte[8];
            GCHandle buf8_pinned = GCHandle.Alloc(buf8, GCHandleType.Pinned);
            IntPtr buf8_ptr = buf8_pinned.AddrOfPinnedObject();

            int result = SQLite3RuntimeProvider.sqlite3_blob_open(db.ToInt64(), sdb_ptr.ToInt64(), table_ptr.ToInt64(), col_ptr.ToInt64(), rowid, flags, buf8_ptr.ToInt64());

            sdb_pinned.Free();
            table_pinned.Free();
            col_pinned.Free();

            blob = new IntPtr(Marshal.ReadInt64(buf8_ptr));
            buf8_pinned.Free();

            return result;
        }

        int ISQLite3Provider.sqlite3_blob_bytes(IntPtr blob)
        {
            return SQLite3RuntimeProvider.sqlite3_blob_bytes(blob.ToInt64());
        }

        int ISQLite3Provider.sqlite3_blob_close(IntPtr blob)
        {
            return SQLite3RuntimeProvider.sqlite3_blob_close(blob.ToInt64());
        }

        int ISQLite3Provider.sqlite3_blob_read(IntPtr blob, byte[] b, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = SQLite3RuntimeProvider.sqlite3_blob_read(blob.ToInt64(), ptr.ToInt64(), n, offset);
            pinned.Free();
            return rc;
        }

        int ISQLite3Provider.sqlite3_blob_write(IntPtr blob, byte[] b, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = SQLite3RuntimeProvider.sqlite3_blob_write(blob.ToInt64(), ptr.ToInt64(), n, offset);
            pinned.Free();
            return rc;
        }

        int ISQLite3Provider.sqlite3_blob_read(IntPtr blob, byte[] b, int bOffset, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = SQLite3RuntimeProvider.sqlite3_blob_read(blob.ToInt64(), ptr.ToInt64() + bOffset, n, offset);
            pinned.Free();
            return rc;
        }

        int ISQLite3Provider.sqlite3_blob_write(IntPtr blob, byte[] b, int bOffset, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = SQLite3RuntimeProvider.sqlite3_blob_write(blob.ToInt64(), ptr.ToInt64() + bOffset, n, offset);
            pinned.Free();
            return rc;
        }

        IntPtr ISQLite3Provider.sqlite3_backup_init(IntPtr destDb, string destName, IntPtr sourceDb, string sourceName)
        {
            // TODO null string?
            GCHandle dest_pinned = GCHandle.Alloc(util.to_utf8(destName), GCHandleType.Pinned);
            IntPtr dest_ptr = dest_pinned.AddrOfPinnedObject();

            // TODO null string?
            GCHandle source_pinned = GCHandle.Alloc(util.to_utf8(sourceName), GCHandleType.Pinned);
            IntPtr source_ptr = source_pinned.AddrOfPinnedObject();

            IntPtr result = new IntPtr(SQLite3RuntimeProvider.sqlite3_backup_init(destDb.ToInt64(), dest_ptr.ToInt64(), sourceDb.ToInt64(), source_ptr.ToInt64()));

            dest_pinned.Free();
            source_pinned.Free();

            return result;
        }

        int ISQLite3Provider.sqlite3_backup_step(IntPtr backup, int nPage)
        {
            return SQLite3RuntimeProvider.sqlite3_backup_step(backup.ToInt64(), nPage);
        }

        int ISQLite3Provider.sqlite3_backup_finish(IntPtr backup)
        {
            return SQLite3RuntimeProvider.sqlite3_backup_finish(backup.ToInt64());
        }

        int ISQLite3Provider.sqlite3_backup_remaining(IntPtr backup)
        {
            return SQLite3RuntimeProvider.sqlite3_backup_remaining(backup.ToInt64());
        }

        int ISQLite3Provider.sqlite3_backup_pagecount(IntPtr backup)
        {
            return SQLite3RuntimeProvider.sqlite3_backup_pagecount(backup.ToInt64());
        }

        int ISQLite3Provider.sqlite3_bind_int(IntPtr stm, int paramIndex, int value)
        {
            return SQLite3RuntimeProvider.sqlite3_bind_int(stm.ToInt64(), paramIndex, value);
        }

        int ISQLite3Provider.sqlite3_bind_int64(IntPtr stm, int paramIndex, long value)
        {
            return SQLite3RuntimeProvider.sqlite3_bind_int64(stm.ToInt64(), paramIndex, value);
        }

        int ISQLite3Provider.sqlite3_bind_text(IntPtr stm, int paramIndex, string t)
        {
            // TODO null string?
            GCHandle pinned = GCHandle.Alloc(util.to_utf8(t), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int result = SQLite3RuntimeProvider.sqlite3_bind_text(stm.ToInt64(), paramIndex, ptr.ToInt64(), -1, -1);
            pinned.Free();
            return result;
        }

        int ISQLite3Provider.sqlite3_bind_double(IntPtr stm, int paramIndex, double value)
        {
            return SQLite3RuntimeProvider.sqlite3_bind_double(stm.ToInt64(), paramIndex, value);
        }

        int ISQLite3Provider.sqlite3_bind_blob(IntPtr stm, int paramIndex, byte[] blob)
        {
            GCHandle pinned = GCHandle.Alloc(blob, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = SQLite3RuntimeProvider.sqlite3_bind_blob(stm.ToInt64(), paramIndex, ptr.ToInt64(), blob.Length, -1);
            pinned.Free();
            return rc;
        }

        int ISQLite3Provider.sqlite3_bind_zeroblob(IntPtr stm, int paramIndex, int size)
        {
            return SQLite3RuntimeProvider.sqlite3_bind_zeroblob(stm.ToInt64(), paramIndex, size);
        }

        int ISQLite3Provider.sqlite3_bind_null(IntPtr stm, int paramIndex)
        {
            return SQLite3RuntimeProvider.sqlite3_bind_null(stm.ToInt64(), paramIndex);
        }

        int ISQLite3Provider.sqlite3_bind_parameter_count(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_bind_parameter_count(stm.ToInt64());
        }

        string ISQLite3Provider.sqlite3_bind_parameter_name(IntPtr stm, int paramIndex)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_bind_parameter_name(stm.ToInt64(), paramIndex)));
        }

        int ISQLite3Provider.sqlite3_bind_parameter_index(IntPtr stm, string paramName)
        {
            // TODO null string?
            GCHandle pinned = GCHandle.Alloc(util.to_utf8(paramName), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int result = SQLite3RuntimeProvider.sqlite3_bind_parameter_index(stm.ToInt64(), ptr.ToInt64());
            pinned.Free();
            return result;
        }

        int ISQLite3Provider.sqlite3_step(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_step(stm.ToInt64());
        }

        int ISQLite3Provider.sqlite3_stmt_busy(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_stmt_busy(stm.ToInt64());
        }

        int ISQLite3Provider.sqlite3_stmt_readonly(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_stmt_readonly(stm.ToInt64());
        }

        int ISQLite3Provider.sqlite3_column_int(IntPtr stm, int columnIndex)
        {
            return SQLite3RuntimeProvider.sqlite3_column_int(stm.ToInt64(), columnIndex);
        }

        long ISQLite3Provider.sqlite3_column_int64(IntPtr stm, int columnIndex)
        {
            return SQLite3RuntimeProvider.sqlite3_column_int64(stm.ToInt64(), columnIndex);
        }

        string ISQLite3Provider.sqlite3_column_text(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_column_text(stm.ToInt64(), columnIndex)));
        }

        string ISQLite3Provider.sqlite3_column_decltype(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_column_decltype(stm.ToInt64(), columnIndex)));
        }

        double ISQLite3Provider.sqlite3_column_double(IntPtr stm, int columnIndex)
        {
            return SQLite3RuntimeProvider.sqlite3_column_double(stm.ToInt64(), columnIndex);
        }

        byte[] ISQLite3Provider.sqlite3_column_blob(IntPtr stm, int columnIndex)
        {
            IntPtr blobPointer = new IntPtr(SQLite3RuntimeProvider.sqlite3_column_blob(stm.ToInt64(), columnIndex));
            if (blobPointer == IntPtr.Zero)
            {
                return null;
            }

            var length = SQLite3RuntimeProvider.sqlite3_column_bytes(stm.ToInt64(), columnIndex);
            byte[] result = new byte[length];
            Marshal.Copy(blobPointer, (byte[])result, 0, length);
            return result;
        }

        int ISQLite3Provider.sqlite3_column_type(IntPtr stm, int columnIndex)
        {
            return SQLite3RuntimeProvider.sqlite3_column_type(stm.ToInt64(), columnIndex);
        }

        int ISQLite3Provider.sqlite3_column_bytes(IntPtr stm, int columnIndex)
        {
            return SQLite3RuntimeProvider.sqlite3_column_bytes(stm.ToInt64(), columnIndex);
        }

        int ISQLite3Provider.sqlite3_column_count(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_column_count(stm.ToInt64());
        }

        int ISQLite3Provider.sqlite3_data_count(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_data_count(stm.ToInt64());
        }

        string ISQLite3Provider.sqlite3_column_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_column_name(stm.ToInt64(), columnIndex)));
        }

        string ISQLite3Provider.sqlite3_column_origin_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_column_origin_name(stm.ToInt64(), columnIndex)));
        }

        string ISQLite3Provider.sqlite3_column_table_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_column_table_name(stm.ToInt64(), columnIndex)));
        }

        string ISQLite3Provider.sqlite3_column_database_name(IntPtr stm, int columnIndex)
        {
            return util.from_utf8(new IntPtr(SQLite3RuntimeProvider.sqlite3_column_database_name(stm.ToInt64(), columnIndex)));
        }

        int ISQLite3Provider.sqlite3_reset(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_reset(stm.ToInt64());
        }

        int ISQLite3Provider.sqlite3_clear_bindings(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_clear_bindings(stm.ToInt64());
        }

        int ISQLite3Provider.sqlite3_finalize(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_finalize(stm.ToInt64());
        }
    }
}
