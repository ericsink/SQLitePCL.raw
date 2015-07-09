/*
   Copyright 2014-2015 Zumero, LLC

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

#if PINVOKE_FROM_INTERNAL_SQLITE3

#if PLATFORM_UNIFIED

[assembly: ObjCRuntime.LinkWith(
        "packaged_sqlite3.a",
        LinkTarget = ObjCRuntime.LinkTarget.Simulator | ObjCRuntime.LinkTarget.Simulator64 | ObjCRuntime.LinkTarget.ArmV7 | ObjCRuntime.LinkTarget.ArmV7s | ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]
#else
[assembly: MonoTouch.ObjCRuntime.LinkWith(
        "packaged_sqlite3.a",
        LinkTarget = MonoTouch.ObjCRuntime.LinkTarget.Simulator | MonoTouch.ObjCRuntime.LinkTarget.Simulator64 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7s | MonoTouch.ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]
#endif

#endif

#if PINVOKE_FROM_INTERNAL_SQLCIPHER

#if PLATFORM_UNIFIED
[assembly: ObjCRuntime.LinkWith(
        "packaged_sqlcipher.a",
        LinkTarget = ObjCRuntime.LinkTarget.Simulator | ObjCRuntime.LinkTarget.Simulator64 | ObjCRuntime.LinkTarget.ArmV7 | ObjCRuntime.LinkTarget.ArmV7s | ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]

[assembly: ObjCRuntime.LinkWith(
        "libcrypto.a",
        LinkTarget = ObjCRuntime.LinkTarget.Simulator | ObjCRuntime.LinkTarget.Simulator64 | ObjCRuntime.LinkTarget.ArmV7 | ObjCRuntime.LinkTarget.ArmV7s | ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]
#else
[assembly: MonoTouch.ObjCRuntime.LinkWith(
        "packaged_sqlcipher.a",
        LinkTarget = MonoTouch.ObjCRuntime.LinkTarget.Simulator | MonoTouch.ObjCRuntime.LinkTarget.Simulator64 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7s | MonoTouch.ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]

[assembly: MonoTouch.ObjCRuntime.LinkWith(
        "libcrypto.a",
        LinkTarget = MonoTouch.ObjCRuntime.LinkTarget.Simulator | MonoTouch.ObjCRuntime.LinkTarget.Simulator64 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7s | MonoTouch.ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]
#endif

#endif

namespace SQLitePCL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
#if PINVOKE_ANYCPU_NET45
    using System.Reflection;
#endif
#if PLATFORM_IOS
    using MonoTouch;
#elif PLATFORM_UNIFIED
    using ObjCRuntime;
#endif

    /// <summary>
    /// Implements the <see cref="ISQLite3Provider"/> interface for .Net45 Framework.
    /// </summary>
#if __ANDROID__
    [Android.Runtime.Preserve(AllMembers=true)]
#elif PLATFORM_IOS
	[MonoTouch.Foundation.Preserve(AllMembers = true)]
#elif PLATFORM_UNIFIED
	[Foundation.Preserve(AllMembers = true)]
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


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SQLiteDeleteDelegate(IntPtr pVfs, byte[] zName, int syncDir);
    }
	
	int ISQLite3Provider.sqlite3__vfs__delete(string vfs, string filename, int syncDir)
	{
	    IntPtr ptrVfs = NativeMethods.sqlite3_vfs_find(util.to_utf8(vfs));
	    // this code and the struct it uses was taken from aspnet/DataCommon.SQLite, Apache License 2.0
	    sqlite3_vfs vstruct = (sqlite3_vfs) Marshal.PtrToStructure(ptrVfs, typeof(sqlite3_vfs));
	    return vstruct.xDelete(ptrVfs, util.to_utf8(filename), 1);
	}

        int ISQLite3Provider.sqlite3_close_v2(IntPtr db)
        {
            var rc = NativeMethods.sqlite3_close_v2(db);
		hooks.removeFor(db);
		return rc;
        }

        int ISQLite3Provider.sqlite3_close(IntPtr db)
        {
            var rc = NativeMethods.sqlite3_close(db);
		hooks.removeFor(db);
		return rc;
        }

        int ISQLite3Provider.sqlite3_enable_shared_cache(int enable)
        {
            return NativeMethods.sqlite3_enable_shared_cache(enable);
        }

        void ISQLite3Provider.sqlite3_interrupt(IntPtr db)
        {
            NativeMethods.sqlite3_interrupt(db);
        }

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_exec))] // TODO not xplat
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

        int ISQLite3Provider.sqlite3_table_column_metadata(IntPtr db, string dbName, string tblName, string colName, out string dataType, out string collSeq, out int notNull, out int primaryKey, out int autoInc)
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

        int ISQLite3Provider.sqlite3_db_status(IntPtr db, int op, out int current, out int highest, int resetFlg)
        {
            return NativeMethods.sqlite3_db_status(db, op, out current, out highest, resetFlg);
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

        int ISQLite3Provider.sqlite3_blob_read(IntPtr blob, byte[] b, int bOffset, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = NativeMethods.other_sqlite3_blob_read(blob, ptr + bOffset, n, offset);
            pinned.Free();
	    return rc;
        }

        int ISQLite3Provider.sqlite3_blob_write(IntPtr blob, byte[] b, int bOffset, int n, int offset)
        {
            GCHandle pinned = GCHandle.Alloc(b, GCHandleType.Pinned);
            IntPtr ptr = pinned.AddrOfPinnedObject();
            int rc = NativeMethods.other_sqlite3_blob_write(blob, ptr + bOffset, n, offset);
            pinned.Free();
	    return rc;
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

        int ISQLite3Provider.sqlite3_total_changes(IntPtr db)
        {
            return NativeMethods.sqlite3_total_changes(db);
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

        int ISQLite3Provider.sqlite3_db_readonly(IntPtr db, string dbName)
        {
            return NativeMethods.sqlite3_db_readonly(db, util.to_utf8(dbName)); 
        }
        
        string ISQLite3Provider.sqlite3_db_filename(IntPtr db, string att)
	{
            return util.from_utf8(NativeMethods.sqlite3_db_filename(db, util.to_utf8(att)));
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
        
#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_commit))] // TODO not xplat
#endif
        static int commit_hook_bridge_impl(IntPtr p)
        {
            commit_hook_info hi = commit_hook_info.from_ptr(p);
            return hi.call();
        }

	NativeMethods.callback_commit commit_hook_bridge = new NativeMethods.callback_commit(commit_hook_bridge_impl); 
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

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_scalar_function))] // TODO not xplat
#endif
        static void scalar_function_hook_bridge_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr p = NativeMethods.sqlite3_user_data(context);
            scalar_function_hook_info hi = scalar_function_hook_info.from_ptr(p);
            hi.call(context, num_args, argsptr);
        }

	NativeMethods.callback_scalar_function scalar_function_hook_bridge = new NativeMethods.callback_scalar_function(scalar_function_hook_bridge_impl); 
        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_scalar func)
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
            if (func != null)
            {
                scalar_function_hook_info hi = new scalar_function_hook_info(func, v);
                int rc = NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, 1, hi.ptr, scalar_function_hook_bridge, null, null, null);
                if (rc == 0)
                {
                    info.scalar[key] = hi;
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

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_agg_function_step))] // TODO not xplat
#endif
        static void agg_function_hook_bridge_step_impl(IntPtr context, int num_args, IntPtr argsptr)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            agg_function_hook_info hi = agg_function_hook_info.from_ptr(p);
            hi.call_step(context, agg, num_args, argsptr);
        }

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_agg_function_final))] // TODO not xplat
#endif
        static void agg_function_hook_bridge_final_impl(IntPtr context)
        {
            IntPtr agg = NativeMethods.sqlite3_aggregate_context(context, 8);
            // TODO error check agg nomem

            IntPtr p = NativeMethods.sqlite3_user_data(context);
            agg_function_hook_info hi = agg_function_hook_info.from_ptr(p);
            hi.call_final(context, agg);
        }

	NativeMethods.callback_agg_function_step agg_function_hook_bridge_step = new NativeMethods.callback_agg_function_step(agg_function_hook_bridge_step_impl); 
	NativeMethods.callback_agg_function_final agg_function_hook_bridge_final = new NativeMethods.callback_agg_function_final(agg_function_hook_bridge_final_impl); 
        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
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
            if (func_step != null)
            {
                // TODO both func_step and func_final must be non-null
                agg_function_hook_info hi = new agg_function_hook_info(func_step, func_final, v);
                int rc = NativeMethods.sqlite3_create_function_v2(db, util.to_utf8(name), nargs, 1, hi.ptr, null, agg_function_hook_bridge_step, agg_function_hook_bridge_final, null);
                if (rc == 0)
                {
                    info.agg[key] = hi;
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

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_collation))] // TODO not xplat
#endif
        static int collation_hook_bridge_impl(IntPtr p, int len1, IntPtr pv1, int len2, IntPtr pv2)
        {
            collation_hook_info hi = collation_hook_info.from_ptr(p);
            return hi.call(util.from_utf8(pv1, len1), util.from_utf8(pv2, len2));
        }

	NativeMethods.callback_collation collation_hook_bridge = new NativeMethods.callback_collation(collation_hook_bridge_impl); 
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

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_update))] // TODO not xplat
#endif
        static void update_hook_bridge_impl(IntPtr p, int typ, IntPtr db, IntPtr tbl, Int64 rowid)
        {
            update_hook_info hi = update_hook_info.from_ptr(p);
            hi.call(typ, util.from_utf8(db), util.from_utf8(tbl), rowid);
        }

	NativeMethods.callback_update update_hook_bridge = new NativeMethods.callback_update(update_hook_bridge_impl); 
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

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_rollback))] // TODO not xplat
#endif
        static void rollback_hook_bridge_impl(IntPtr p)
        {
            rollback_hook_info hi = rollback_hook_info.from_ptr(p);
            hi.call();
        }

	NativeMethods.callback_rollback rollback_hook_bridge = new NativeMethods.callback_rollback(rollback_hook_bridge_impl); 
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

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_trace))] // TODO not xplat
#endif
        static void trace_hook_bridge_impl(IntPtr p, IntPtr s)
        {
            trace_hook_info hi = trace_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s));
        }

	NativeMethods.callback_trace trace_hook_bridge = new NativeMethods.callback_trace(trace_hook_bridge_impl); 
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

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_profile))] // TODO not xplat
#endif
        static void profile_hook_bridge_impl(IntPtr p, IntPtr s, long elapsed)
        {
            profile_hook_info hi = profile_hook_info.from_ptr(p);
            hi.call(util.from_utf8(s), elapsed);
        }

	NativeMethods.callback_profile profile_hook_bridge = new NativeMethods.callback_profile(profile_hook_bridge_impl); 
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

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_progress_handler))] // TODO not xplat
#endif
        static int progress_handler_hook_bridge_impl(IntPtr p)
        {
            progress_handler_hook_info hi = progress_handler_hook_info.from_ptr(p);
            return hi.call();
        }

        NativeMethods.callback_progress_handler progress_handler_hook_bridge = new NativeMethods.callback_progress_handler(progress_handler_hook_bridge_impl);
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

        private authorizer_hook_info _authorizer_hook;

#if PLATFORM_IOS || PLATFORM_UNIFIED
        [MonoPInvokeCallback (typeof(NativeMethods.callback_authorizer))] // TODO not xplat
#endif
        static int authorizer_hook_bridge_impl(IntPtr p, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view)
        {
            authorizer_hook_info hi = authorizer_hook_info.from_ptr(p);
            return hi.call(action_code, util.from_utf8(param0), util.from_utf8(param1), util.from_utf8(dbName), util.from_utf8(inner_most_trigger_or_view));
        }

        NativeMethods.callback_authorizer authorizer_hook_bridge = new NativeMethods.callback_authorizer(authorizer_hook_bridge_impl);
        int ISQLite3Provider.sqlite3_set_authorizer(IntPtr db, delegate_authorizer func, object v)
        {
            if (_authorizer_hook != null)
            {
                // TODO maybe turn off the hook here, for now
                _authorizer_hook.free();
                _authorizer_hook = null;
            }

            if (func != null)
            {
                _authorizer_hook = new authorizer_hook_info(func, v);
                return NativeMethods.sqlite3_set_authorizer(db, authorizer_hook_bridge, _authorizer_hook.ptr);
            }
            else
            {
                return NativeMethods.sqlite3_set_authorizer(db, null, IntPtr.Zero);
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

        int ISQLite3Provider.sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            return NativeMethods.sqlite3_status(op, out current, out highwater, resetFlag);
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

        int ISQLite3Provider.sqlite3_stmt_status(IntPtr stm, int op, int resetFlg)
        {
            return NativeMethods.sqlite3_stmt_status(stm, op, resetFlg);
        }

        int ISQLite3Provider.sqlite3_finalize(IntPtr stm)
        {
            return NativeMethods.sqlite3_finalize(stm);
        }

        int ISQLite3Provider.sqlite3_wal_autocheckpoint(IntPtr db, int n)
        {
            return NativeMethods.sqlite3_wal_autocheckpoint(db, n);
        }

        int ISQLite3Provider.sqlite3_wal_checkpoint(IntPtr db, string dbName)
        {
            return NativeMethods.sqlite3_wal_checkpoint(db, util.to_utf8(dbName));
        }

        int ISQLite3Provider.sqlite3_wal_checkpoint_v2(IntPtr db, string dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            return NativeMethods.sqlite3_wal_checkpoint_v2(db, util.to_utf8(dbName), eMode, out logSize, out framesCheckPointed);
        }

        private static class NativeMethods
        {
#if PINVOKE_FROM_INTERNAL_SQLITE3
        private const string SQLITE_DLL = "__Internal";
#elif PINVOKE_FROM_INTERNAL_SQLCIPHER
        private const string SQLITE_DLL = "__Internal";
#elif PINVOKE_FROM_SQLITE3
        private const string SQLITE_DLL = "sqlite3";
#elif PINVOKE_FROM_PACKAGED_SQLITE3
        private const string SQLITE_DLL = "packaged_sqlite3";
#elif PINVOKE_FROM_PACKAGED_SQLCIPHER
        private const string SQLITE_DLL = "packaged_sqlcipher";
#elif PINVOKE_FROM_SQLITE3_DLL
        private const string SQLITE_DLL = "sqlite3.dll";
#elif PINVOKE_ANYCPU_NET45
        private const string SQLITE_DLL = "sqlite3";

	// TODO can the code below be adapted to cope with Mono on Mac or Linux?

        // Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
        // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
        // https://github.com/aspnet/DataCommon.SQLite/blob/dev/src/Microsoft.Data.SQLite/Utilities/NativeLibraryLoader.cs

        [DllImport("kernel32")]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        private static bool TryLoadFromDirectory(string dllName, string baseDirectory)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(dllName), "dllName is null or empty.");
            System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(baseDirectory), "baseDirectory is null or empty.");
            System.Diagnostics.Debug.Assert(System.IO.Path.IsPathRooted(baseDirectory), "baseDirectory is not rooted.");

            var architecture = IntPtr.Size == 4
                ? "x86"
                : "x64";

            var dllPath = System.IO.Path.Combine(baseDirectory, architecture, dllName);
            if (!System.IO.File.Exists(dllPath))
	    {
                return false;
	    }

	    const uint LOAD_WITH_ALTERED_SEARCH_PATH = 8;

            var ptr = LoadLibraryEx(dllPath, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH);
            return ptr != IntPtr.Zero;
        }

        static NativeMethods()
        {
	    if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
	    {
#if OLD_REFLECTION
		    var currentAssembly = typeof(NativeMethods).Assembly;
#else
		    var currentAssembly = typeof(NativeMethods).GetTypeInfo().Assembly;
#endif
		    if (TryLoadFromDirectory("sqlite3.dll", new Uri(AppDomain.CurrentDomain.BaseDirectory).LocalPath))
		    {
			return;
		    }
		    throw new Exception("sqlite3.dll was not loaded.");
	    }
        }
#endif

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_close(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_close_v2(IntPtr db); /* 3.7.14+ */

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_enable_shared_cache(int enable);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_interrupt(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_finalize(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_reset(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_clear_bindings(IntPtr stmt);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_stmt_status(IntPtr stm, int op, int resetFlg);

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
            public static extern int sqlite3_db_readonly(IntPtr db, byte[] dbName);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_db_filename(IntPtr db, byte[] att);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_prepare(IntPtr db, IntPtr pSql, int nBytes, out IntPtr stmt, out IntPtr ptrRemain);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_prepare_v2(IntPtr db, byte[] pSql, int nBytes, out IntPtr stmt, out IntPtr ptrRemain);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_db_status(IntPtr db, int op, out int current, out int highest, int resetFlg);

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

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_vfs_find(byte[] vfs);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public static extern int sqlite3_open16(string fileName, out IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern long sqlite3_last_insert_rowid(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_changes(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_total_changes(IntPtr db);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern long sqlite3_memory_used();

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern long sqlite3_memory_highwater(int resetFlag);
            
            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_status(int op, out int current, out int highwater, int resetFlag);

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
            public static extern void sqlite3_result_zeroblob(IntPtr context, int n);

            // TODO sqlite3_result_value 
 
            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_error_toobig(IntPtr context);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_error_nomem(IntPtr context);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern void sqlite3_result_error_code(IntPtr context, int code);

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
            public delegate int callback_progress_handler(IntPtr puser);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int callback_authorizer(IntPtr puser, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr sqlite3_progress_handler(IntPtr db, int instructions, callback_progress_handler func, IntPtr pvUser);

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

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint="sqlite3_blob_write")]
            public static extern int other_sqlite3_blob_write(IntPtr blob, IntPtr b, int n, int offset);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint="sqlite3_blob_read")]
            public static extern int other_sqlite3_blob_read(IntPtr blob, IntPtr b, int n, int offset);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_blob_bytes(IntPtr blob);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_blob_close(IntPtr blob);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_wal_autocheckpoint(IntPtr db, int n);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_wal_checkpoint(IntPtr db, byte[] dbName);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_wal_checkpoint_v2(IntPtr db, byte[] dbName, int eMode, out int logSize, out int framesCheckPointed);

            [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int sqlite3_set_authorizer(IntPtr db, callback_authorizer cb, IntPtr pvUser);
#if NETFX_CORE
            [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_win32_set_directory", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
            public static extern int sqlite3_win32_set_directory (uint directoryType, string directoryPath);
#endif

        }
    }
}
