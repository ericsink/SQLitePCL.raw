/*
   Copyright 2014-2016 Zumero, LLC

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
    public sealed class SQLite3Provider_e_sqlite3 : ISQLite3Provider
    {
        public SQLite3Provider_e_sqlite3()
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

        int ISQLite3Provider.sqlite3_enable_shared_cache(int enable)
        {
            return SQLite3RuntimeProvider.sqlite3_enable_shared_cache(enable);
        }
        
        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        static private int commit_hook_bridge_impl(IntPtr p)
        {
            commit_hook_info hi = commit_hook_info.from_ptr(p);
            return hi.call();
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate int callback_commit(IntPtr p);

        callback_commit commit_hook_bridge = new callback_commit(commit_hook_bridge_impl);
        void ISQLite3Provider.sqlite3_commit_hook(IntPtr db, delegate_commit func, object v)
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
                SQLite3RuntimeProvider.sqlite3_commit_hook(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(commit_hook_bridge).ToInt64(), info.commit.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_commit_hook(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate int callback_collation(IntPtr puser, int len1, IntPtr pv1, int len2, IntPtr pv2);

        static int collation_hook_bridge_impl(IntPtr p, int len1, IntPtr pv1, int len2, IntPtr pv2)
        {
            collation_hook_info hi = collation_hook_info.from_ptr(p);
            return hi.call(util.from_utf8(pv1, len1), util.from_utf8(pv2, len2));
        }

        callback_collation collation_hook_bridge = new callback_collation(collation_hook_bridge_impl);

        int ISQLite3Provider.sqlite3_create_collation(IntPtr db, string name, object v, delegate_collation func)
        {
		var info = hooks.getOrCreateFor(db);
            if (info.collation.ContainsKey(name))
            {
                collation_hook_info hi = info.collation[name];

                // TODO maybe turn off the hook here, for now
                hi.free();

                info.collation.Remove(name);
            }

            GCHandle pinned = GCHandle.Alloc(util.to_utf8(name), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();

            int rc;

            // 1 is SQLITE_UTF8
            if (func != null)
            {
                collation_hook_info hi = new collation_hook_info(func, v);
                rc = SQLite3RuntimeProvider.sqlite3_create_collation(db.ToInt64(), ptr.ToInt64(), 1, hi.ptr.ToInt64(), Marshal.GetFunctionPointerForDelegate(collation_hook_bridge).ToInt64());
                if (rc == 0)
                {
                    info.collation[name] = hi;
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

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_scalar_function(IntPtr context, int nArgs, IntPtr argsptr);

        static void scalar_function_hook_bridge_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr p = new IntPtr(SQLite3RuntimeProvider.sqlite3_user_data(context.ToInt64()));
            scalar_function_hook_info hi = scalar_function_hook_info.from_ptr(p);
            hi.call(context, num_args, argsptr);
        }

        callback_scalar_function scalar_function_hook_bridge = new callback_scalar_function(scalar_function_hook_bridge_impl);

        int my_sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_scalar func)
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

            GCHandle pinned = GCHandle.Alloc(util.to_utf8(name), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();

            int rc;

            // 1 is SQLITE_UTF8
			int arg4 = 1 | flags;
            if (func != null)
            {
                scalar_function_hook_info hi = new scalar_function_hook_info(func, v);
                rc = SQLite3RuntimeProvider.sqlite3_create_function_v2(db.ToInt64(), ptr.ToInt64(), nargs, arg4, hi.ptr.ToInt64(), Marshal.GetFunctionPointerForDelegate(scalar_function_hook_bridge).ToInt64(), 0, 0, 0);
                if (rc == 0)
                {
                    info.scalar[key] = hi;
                }
            }
            else
            {
                rc = SQLite3RuntimeProvider.sqlite3_create_function_v2(db.ToInt64(), ptr.ToInt64(), nargs, arg4, 0, 0, 0, 0, 0);
            }

            pinned.Free();

            return rc;
        }

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_scalar func)
		{
			return my_sqlite3_create_function(db, name, nargs, 0, v, func);
		}

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_scalar func)
		{
			return my_sqlite3_create_function(db, name, nargs, flags, v, func);
		}

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs


        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_agg_function_step(IntPtr context, int nArgs, IntPtr argsptr);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_agg_function_final(IntPtr context);

        static void agg_function_hook_bridge_step_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr agg = new IntPtr(SQLite3RuntimeProvider.sqlite3_aggregate_context(context.ToInt64(), 8));
            // TODO error check agg nomem

            IntPtr p = new IntPtr(SQLite3RuntimeProvider.sqlite3_user_data(context.ToInt64()));
            agg_function_hook_info hi = agg_function_hook_info.from_ptr(p);
            hi.call_step(context, agg, num_args, argsptr);
        }

        static void agg_function_hook_bridge_final_impl(IntPtr context)
        {
            IntPtr agg = new IntPtr(SQLite3RuntimeProvider.sqlite3_aggregate_context(context.ToInt64(), 8));
            // TODO error check agg nomem

            IntPtr p = new IntPtr(SQLite3RuntimeProvider.sqlite3_user_data(context.ToInt64()));
            agg_function_hook_info hi = agg_function_hook_info.from_ptr(p);
            hi.call_final(context, agg);
        }

        callback_agg_function_step agg_function_hook_bridge_step = new callback_agg_function_step(agg_function_hook_bridge_step_impl);
        callback_agg_function_final agg_function_hook_bridge_final = new callback_agg_function_final(agg_function_hook_bridge_final_impl);

        int my_sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
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

            GCHandle pinned = GCHandle.Alloc(util.to_utf8(name), GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();

            int rc;

            // 1 is SQLITE_UTF8
			int arg4 = 1 | flags;
            if (func_step != null)
            {
                // TODO both func_step and func_final must be non-null
                agg_function_hook_info hi = new agg_function_hook_info(func_step, func_final, v);
                rc = SQLite3RuntimeProvider.sqlite3_create_function_v2(db.ToInt64(), ptr.ToInt64(), nargs, arg4, hi.ptr.ToInt64(), 0, Marshal.GetFunctionPointerForDelegate(agg_function_hook_bridge_step).ToInt64(), Marshal.GetFunctionPointerForDelegate(agg_function_hook_bridge_final).ToInt64(), 0);
                if (rc == 0)
                {
                    info.agg[key] = hi;
                }
            }
            else
            {
                rc = SQLite3RuntimeProvider.sqlite3_create_function_v2(db.ToInt64(), ptr.ToInt64(), nargs, arg4, 0, 0, 0, 0, 0);
            }

            pinned.Free();

            return rc;
        }

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
		{
			return my_sqlite3_create_function(db, name, nargs, 0, v, func_step, func_final);
		}

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
		{
			return my_sqlite3_create_function(db, name, nargs, flags, v, func_step, func_final);
		}

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        static private void rollback_hook_bridge_impl(IntPtr p)
        {
            rollback_hook_info hi = rollback_hook_info.from_ptr(p);
            hi.call();
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_rollback(IntPtr p);

        callback_rollback rollback_hook_bridge = new callback_rollback(rollback_hook_bridge_impl);

        void ISQLite3Provider.sqlite3_rollback_hook(IntPtr db, delegate_rollback func, object v)
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
                SQLite3RuntimeProvider.sqlite3_rollback_hook(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(rollback_hook_bridge).ToInt64(), info.rollback.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_rollback_hook(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        static private void update_hook_bridge_impl(IntPtr p, int typ, IntPtr db, IntPtr tbl, long rowid)
        {
            update_hook_info hi = update_hook_info.from_ptr(p);
            hi.call(typ, util.from_utf8(db), util.from_utf8(tbl), rowid);
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_update(IntPtr p, int typ, IntPtr db, IntPtr tbl, long rowid);

        callback_update update_hook_bridge = new callback_update(update_hook_bridge_impl);

        void ISQLite3Provider.sqlite3_update_hook(IntPtr db, delegate_update func, object v)
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
                SQLite3RuntimeProvider.sqlite3_update_hook(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(update_hook_bridge).ToInt64(), info.update.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_update_hook(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        static private void trace_hook_bridge_impl(IntPtr p, IntPtr s)
        {
            trace_hook_info hi = trace_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s));
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_trace(IntPtr p, IntPtr s);

        callback_trace trace_hook_bridge = new callback_trace(trace_hook_bridge_impl);

        void ISQLite3Provider.sqlite3_trace(IntPtr db, delegate_trace func, object v)
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
                SQLite3RuntimeProvider.sqlite3_trace(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(trace_hook_bridge).ToInt64(), info.trace.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_trace(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        static private void profile_hook_bridge_impl(IntPtr p, IntPtr s, long elapsed)
        {
            profile_hook_info hi = profile_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s), elapsed);
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void callback_profile(IntPtr p, IntPtr s, long elapsed);

        callback_profile profile_hook_bridge = new callback_profile(profile_hook_bridge_impl);

        void ISQLite3Provider.sqlite3_profile(IntPtr db, delegate_profile func, object v)
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
                SQLite3RuntimeProvider.sqlite3_profile(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(profile_hook_bridge).ToInt64(), info.profile.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_profile(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

 // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        static private int progress_handler_hook_bridge_impl(IntPtr p)
        {
            progress_handler_hook_info hi = progress_handler_hook_info.from_ptr(p);
            return hi.call();
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate int callback_progress_handler(IntPtr p);

        callback_progress_handler progress_handler_hook_bridge = new callback_progress_handler(progress_handler_hook_bridge_impl);

        void ISQLite3Provider.sqlite3_progress_handler(IntPtr db, int instructions, delegate_progress_handler func, object v)
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
                SQLite3RuntimeProvider.sqlite3_progress_handler(db.ToInt64(), instructions, Marshal.GetFunctionPointerForDelegate(progress_handler_hook_bridge).ToInt64(), info.progress.ptr.ToInt64());
            }
            else
            {
                SQLite3RuntimeProvider.sqlite3_progress_handler(db.ToInt64(), instructions, IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        // Passing a callback into SQLite is tricky.  See comments near commit_hook
        // implementation in pinvoke/SQLite3Provider.cs

        static private int authorizer_hook_bridge_impl(IntPtr p, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view)
        {
            authorizer_hook_info hi = authorizer_hook_info.from_ptr(p);
            return hi.call(action_code, util.from_utf8(param0), util.from_utf8(param1), util.from_utf8(dbName), util.from_utf8(inner_most_trigger_or_view));
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate int callback_authorizer(IntPtr puser, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view);

        callback_authorizer authorizer_hook_bridge = new callback_authorizer(authorizer_hook_bridge_impl);

        int ISQLite3Provider.sqlite3_set_authorizer(IntPtr db, delegate_authorizer func, object v)
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
                return SQLite3RuntimeProvider.sqlite3_set_authorizer(db.ToInt64(), Marshal.GetFunctionPointerForDelegate(authorizer_hook_bridge).ToInt64(), info.authorizer.ptr.ToInt64());
            }
            else
            {
                return SQLite3RuntimeProvider.sqlite3_set_authorizer(db.ToInt64(), IntPtr.Zero.ToInt64(), IntPtr.Zero.ToInt64());
            }
        }

        // ----------------------------------------------------------------

        int ISQLite3Provider.sqlite3_close(IntPtr db)
        {
            var rc = SQLite3RuntimeProvider.sqlite3_close(db.ToInt64());
		hooks.removeFor(db);
		return rc;
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

        int ISQLite3Provider.sqlite3_table_column_metadata(IntPtr db, string dbName, string tblName, string colName, out string dataType, out string collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            // TODO null string?
            GCHandle db_name_pinned = GCHandle.Alloc(util.to_utf8(dbName), GCHandleType.Pinned);
            IntPtr db_name_ptr = db_name_pinned.AddrOfPinnedObject();

            // TODO null string?
            GCHandle tbl_name_pinned = GCHandle.Alloc(util.to_utf8(tblName), GCHandleType.Pinned);
            IntPtr tbl_name_ptr = tbl_name_pinned.AddrOfPinnedObject();

            // TODO null string?
            GCHandle col_name_pinned = GCHandle.Alloc(util.to_utf8(colName), GCHandleType.Pinned);
            IntPtr col_name_ptr = col_name_pinned.AddrOfPinnedObject();

            byte[] buf_data_type = new byte[8];
            GCHandle buf_data_type_pinned = GCHandle.Alloc(buf_data_type, GCHandleType.Pinned);
            IntPtr buf_data_type_ptr = buf_data_type_pinned.AddrOfPinnedObject();

            byte[] buf_coll_seq = new byte[8];
            GCHandle buf_coll_seq_pinned = GCHandle.Alloc(buf_coll_seq, GCHandleType.Pinned);
            IntPtr buf_coll_seq_ptr = buf_coll_seq_pinned.AddrOfPinnedObject();

            int buf_not_null = 0;
            GCHandle buf_not_null_pinned = GCHandle.Alloc(buf_not_null, GCHandleType.Pinned);
            IntPtr buf_not_null_ptr = buf_not_null_pinned.AddrOfPinnedObject();

            int buf_primary_key = 0;
            GCHandle buf_primary_key_pinned = GCHandle.Alloc(buf_primary_key, GCHandleType.Pinned);
            IntPtr buf_primary_key_ptr = buf_primary_key_pinned.AddrOfPinnedObject();

            int buf_auto_inc = 0;
            GCHandle buf_auto_inc_pinned = GCHandle.Alloc(buf_auto_inc, GCHandleType.Pinned);
            IntPtr buf_auto_inc_ptr = buf_auto_inc_pinned.AddrOfPinnedObject();

            int rc = SQLite3RuntimeProvider.sqlite3_table_column_metadata(
                        db.ToInt64(), db_name_ptr.ToInt64(), tbl_name_ptr.ToInt64(), col_name_ptr.ToInt64(),
                        buf_data_type_ptr.ToInt64(), buf_coll_seq_ptr.ToInt64(), buf_not_null_ptr.ToInt64(), buf_primary_key_ptr.ToInt64(), buf_auto_inc_ptr.ToInt64());  

            col_name_pinned.Free();
            tbl_name_pinned.Free();
            db_name_pinned.Free();

            long dataTypePtr = Marshal.ReadInt64(buf_data_type_ptr);
            if (dataTypePtr == 0)
            {
                dataType = null;
            }
            else
            {
                dataType = util.from_utf8(new IntPtr(dataTypePtr));
                if (dataType.Length == 0)
                {
                    dataType = null;
                }
            }
            buf_data_type_pinned.Free();

            long collSeqPtr = Marshal.ReadInt64(buf_coll_seq_ptr);
            if (collSeqPtr == 0)
            {
                collSeq = null;
            }
            else
            {
                collSeq = util.from_utf8(new IntPtr(collSeqPtr));
                if (collSeq.Length == 0)
                {
                    collSeq = null;
                }
            }
            buf_coll_seq_pinned.Free();

            notNull = Marshal.ReadInt32(buf_not_null_ptr);
            buf_not_null_pinned.Free();

            primaryKey = Marshal.ReadInt32(buf_primary_key_ptr);
            buf_primary_key_pinned.Free();

            autoInc = Marshal.ReadInt32(buf_auto_inc_ptr);
            buf_auto_inc_pinned.Free();

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

        int ISQLite3Provider.sqlite3_db_status(IntPtr db, int op, out int current, out int highest, int resetFlg)
        {
            int buf_current = 0;
            GCHandle buf_current_pinned = GCHandle.Alloc(buf_current, GCHandleType.Pinned);
            IntPtr buf_current_ptr = buf_current_pinned.AddrOfPinnedObject();

            int buf_highest = 0;
            GCHandle buf_highest_pinned = GCHandle.Alloc(buf_highest, GCHandleType.Pinned);
            IntPtr buf_highest_ptr = buf_highest_pinned.AddrOfPinnedObject();

            int result = SQLite3RuntimeProvider.sqlite3_db_status(db.ToInt64(), op, buf_current_ptr.ToInt64(), buf_highest_ptr.ToInt64(), resetFlg);

            current = Marshal.ReadInt32(buf_current_ptr);
            buf_current_pinned.Free();

            highest = Marshal.ReadInt32(buf_highest_ptr);
            buf_highest_pinned.Free();

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

        int ISQLite3Provider.sqlite3_threadsafe()
        {
            return SQLite3RuntimeProvider.sqlite3_threadsafe();
        }

        int ISQLite3Provider.sqlite3_initialize()
        {
            return SQLite3RuntimeProvider.sqlite3_initialize();
        }

        int ISQLite3Provider.sqlite3_shutdown()
        {
            return SQLite3RuntimeProvider.sqlite3_shutdown();
        }

        int ISQLite3Provider.sqlite3_config_log(delegate_log func, object v)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_config(int op)
        {
            return SQLite3RuntimeProvider.sqlite3_config_none(op);
        }

        int ISQLite3Provider.sqlite3_config(int op, int val)
        {
            return SQLite3RuntimeProvider.sqlite3_config_int(op, val);
        }

        int ISQLite3Provider.sqlite3_enable_load_extension(IntPtr db, int onoff)
        {
            return SQLite3RuntimeProvider.sqlite3_enable_load_extension(db.ToInt64(), onoff);
        }

        long ISQLite3Provider.sqlite3_memory_used()
        {
            return SQLite3RuntimeProvider.sqlite3_memory_used();
        }

        long ISQLite3Provider.sqlite3_memory_highwater(int resetFlag)
        {
            return SQLite3RuntimeProvider.sqlite3_memory_highwater(resetFlag);
        }

        int ISQLite3Provider.sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            int buf_current = 0;
            GCHandle buf_current_pinned = GCHandle.Alloc(buf_current, GCHandleType.Pinned);
            IntPtr buf_current_ptr = buf_current_pinned.AddrOfPinnedObject();

            int buf_highwater = 0;
            GCHandle buf_highwater_pinned = GCHandle.Alloc(buf_highwater, GCHandleType.Pinned);
            IntPtr buf_highwater_ptr = buf_highwater_pinned.AddrOfPinnedObject();

            int result = SQLite3RuntimeProvider.sqlite3_status(op, buf_current_ptr.ToInt64(), buf_highwater_ptr.ToInt64(), resetFlag);

            current = Marshal.ReadInt32(buf_current_ptr);
            buf_current_pinned.Free();

            highwater = Marshal.ReadInt32(buf_highwater_ptr);
            buf_highwater_pinned.Free();

            return result;
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

        int ISQLite3Provider.sqlite3_total_changes(IntPtr db)
        {
            return SQLite3RuntimeProvider.sqlite3_total_changes(db.ToInt64());
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

        int ISQLite3Provider.sqlite3_db_readonly(IntPtr db, string dbName)
        {
            int result;            

            if (dbName != null)
            {
                GCHandle pinned = GCHandle.Alloc(util.to_utf8(dbName), GCHandleType.Pinned);
                IntPtr ptr = pinned.AddrOfPinnedObject();
                result = SQLite3RuntimeProvider.sqlite3_db_readonly(db.ToInt64(), ptr.ToInt64());
                pinned.Free();
            }
            else
            {
                result = SQLite3RuntimeProvider.sqlite3_db_readonly(db.ToInt64(), IntPtr.Zero.ToInt64());
            }

            return result;
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
	    return result;
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

        void ISQLite3Provider.sqlite3_result_zeroblob(IntPtr ctx, int n)
        {
            SQLite3RuntimeProvider.sqlite3_result_zeroblob(ctx.ToInt64(), n);
        }

        // TODO sqlite3_result_value

        void ISQLite3Provider.sqlite3_result_error_toobig(IntPtr ctx)
        {
            SQLite3RuntimeProvider.sqlite3_result_error_toobig(ctx.ToInt64());
        }

        void ISQLite3Provider.sqlite3_result_error_nomem(IntPtr ctx)
        {
            SQLite3RuntimeProvider.sqlite3_result_error_nomem(ctx.ToInt64());
        }

        void ISQLite3Provider.sqlite3_result_error_code(IntPtr ctx, int code)
        {
            SQLite3RuntimeProvider.sqlite3_result_error_code(ctx.ToInt64(), code);
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

        int ISQLite3Provider.sqlite3_blob_open(IntPtr db, byte[] db_utf8, byte[] table_utf8, byte[] col_utf8, long rowid, int flags, out IntPtr blob)
        {
            // TODO null string?
            GCHandle sdb_pinned = GCHandle.Alloc(db_utf8, GCHandleType.Pinned);
            IntPtr sdb_ptr = sdb_pinned.AddrOfPinnedObject();

            // TODO null string?
            GCHandle table_pinned = GCHandle.Alloc(table_utf8, GCHandleType.Pinned);
            IntPtr table_ptr = table_pinned.AddrOfPinnedObject();

            // TODO null string?
            GCHandle col_pinned = GCHandle.Alloc(col_utf8, GCHandleType.Pinned);
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
            return ((ISQLite3Provider)this).sqlite3_bind_blob(stm, paramIndex, blob, blob.Length);
        }

        int ISQLite3Provider.sqlite3_bind_blob(IntPtr stm, int paramIndex, byte[] blob, int nSize)
        {
            GCHandle pinned = GCHandle.Alloc(blob, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = SQLite3RuntimeProvider.sqlite3_bind_blob(stm.ToInt64(), paramIndex, ptr.ToInt64(), nSize, -1);
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

        int ISQLite3Provider.sqlite3_column_blob(IntPtr stm, int columnIndex, byte[] result, int offset)
        {
            if (result == null || offset >= result.Length)
            {
                return raw.SQLITE_ERROR;
            }
            IntPtr blobPointer = new IntPtr(SQLite3RuntimeProvider.sqlite3_column_blob(stm.ToInt64(), columnIndex));
            if (blobPointer == IntPtr.Zero)
            {
                return raw.SQLITE_ERROR;
            }

            var length = SQLite3RuntimeProvider.sqlite3_column_bytes(stm.ToInt64(), columnIndex);
            if (offset + length > result.Length)
            {
                return raw.SQLITE_ERROR;
            }
            Marshal.Copy(blobPointer, (byte[])result, offset, length);
            return raw.SQLITE_OK;
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

        int ISQLite3Provider.sqlite3_stmt_status(IntPtr stm, int op, int resetFlg)
        {
            return SQLite3RuntimeProvider.sqlite3_stmt_status(stm.ToInt64(), op, resetFlg);
        }

        int ISQLite3Provider.sqlite3_finalize(IntPtr stm)
        {
            return SQLite3RuntimeProvider.sqlite3_finalize(stm.ToInt64());
        }

        int ISQLite3Provider.sqlite3_wal_autocheckpoint(IntPtr db, int n)
        {
            return SQLite3RuntimeProvider.sqlite3_wal_autocheckpoint(db.ToInt64(), n);
        }

        int ISQLite3Provider.sqlite3_wal_checkpoint(IntPtr db, string dbName)
        {
            // TODO null string?
            GCHandle db_name_ptr_pinned = GCHandle.Alloc(util.to_utf8(dbName), GCHandleType.Pinned);
            IntPtr db_name_ptr = db_name_ptr_pinned.AddrOfPinnedObject();
            int result = SQLite3RuntimeProvider.sqlite3_wal_checkpoint(db.ToInt64(), db_name_ptr.ToInt64());
            db_name_ptr_pinned.Free();
            return result;
        }

        int ISQLite3Provider.sqlite3_wal_checkpoint_v2(IntPtr db, string dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            // TODO null string?
            GCHandle db_name_ptr_pinned = GCHandle.Alloc(util.to_utf8(dbName), GCHandleType.Pinned);
            IntPtr db_name_ptr = db_name_ptr_pinned.AddrOfPinnedObject();

            int buf_log_size = 0;
            GCHandle buf_log_size_pinned = GCHandle.Alloc(buf_log_size, GCHandleType.Pinned);
            IntPtr buf_log_size_ptr = buf_log_size_pinned.AddrOfPinnedObject();
            
            int buf_frames_check_pointed = 0;
            GCHandle buf_frames_check_pointed_pinned = GCHandle.Alloc(buf_frames_check_pointed, GCHandleType.Pinned);
            IntPtr buf_frames_check_pointed_ptr = buf_frames_check_pointed_pinned.AddrOfPinnedObject();

            int result = SQLite3RuntimeProvider.sqlite3_wal_checkpoint_v2(db.ToInt64(), db_name_ptr.ToInt64(), eMode, buf_log_size_ptr.ToInt64(), buf_frames_check_pointed_ptr.ToInt64());

            framesCheckPointed = Marshal.ReadInt32(buf_frames_check_pointed_ptr);
            buf_frames_check_pointed_pinned.Free();

            logSize = Marshal.ReadInt32(buf_log_size_ptr);
            buf_log_size_pinned.Free();

            db_name_ptr_pinned.Free();

            return result;
        }

        int ISQLite3Provider.sqlite3_win32_set_directory(int typ, string path)
        {
            throw new NotImplementedException();
        }

    }
}
