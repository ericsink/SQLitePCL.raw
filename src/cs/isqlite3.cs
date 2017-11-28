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
    using System;

    public delegate void delegate_log(object user_data, int errorCode, string msg);
    public delegate int delegate_authorizer(object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view);
    public delegate int delegate_commit(object user_data);
    public delegate void delegate_rollback(object user_data);
    public delegate void delegate_trace(object user_data, string statement);
    public delegate void delegate_profile(object user_data, string statement, long ns);
    public delegate int delegate_progress_handler(object user_data);
    public delegate void delegate_update(object user_data, int type, string database, string table, long rowid);
    public delegate int delegate_collation(object user_data, string s1, string s2);
    public delegate int delegate_exec(object user_data, string[] values, string[] names);

    public delegate void delegate_function_scalar(sqlite3_context ctx, object user_data, sqlite3_value[] args);
    public delegate void delegate_function_aggregate_step(sqlite3_context ctx, object user_data, sqlite3_value[] args);
    public delegate void delegate_function_aggregate_final(sqlite3_context ctx, object user_data);

    /// <summary>
    ///
    /// This interface provides core functionality of the SQLite3 API.  It is the
    /// boundary between the portable class library and the platform-specific code
    /// below.
    ///
    /// In general, it is defined to be as low-level as possible while still remaninig
    /// "portable".  For example, a sqlite3 connection handle appears here as an IntPtr.
    /// Same goes for the C-level sqlite3_stmt pointer, also an IntPtr.
    ///
    /// However, this layer does deal in C# strings, not the utf8 pointers that the
    /// SQLite C API uses.  This is because the code to marshal the utf8 pointers
    /// to/from C# strings is not "portable".  It would require referencing assemblies
    /// here that we do not want to reference.  We prefer to keep the PCL itself clean
    /// and accept a little extra mess in the platform assemblies.
    ///
    /// This whole library is designed in 4 layers:
    ///
    /// (1)  The SQLite C API itself
    ///
    /// (2)  The declarations of the C API.  This is either pinvoke or C++ COM glue,
    ///      depending on the platform.
    ///
    /// (3)  A C# layer in the platform assembly which implements this interface.  This
    ///      includes converting strings to/from utf8.  It also needs to be a non-static
    ///      class, which layer (2) is not.
    ///
    /// (4)  The raw API, here in the PCL, which wraps an instance of this interface in
    ///      an API which replaces all the IntPtrs with strong typed (but still opaque) 
    ///      counterparts.
    ///
    /// Even the top layer is still very low-level, which is why it is called "raw".
    /// This API is not intended to be used by app developers.  Rather it is designed
    /// to be a portable foundation for higher-level SQLite APIs.
    ///
    /// The philosophy of this library is to remain as similar to the underlying
    /// SQLite API as possible, even to the point of keeping the sqlite3_style_names
    /// and style.  It is expected that higher-level APIs built on this wrapper
    /// would present an API which is friendlier to C# developers.
    ///
    /// </summary>
    public interface ISQLite3Provider
    {
        int sqlite3_open(string filename, out IntPtr db);
        int sqlite3_open_v2(string filename, out IntPtr db, int flags, string vfs);
        int sqlite3_close_v2(IntPtr db); /* 3.7.14+ */
        int sqlite3_close(IntPtr db);

        int sqlite3_enable_shared_cache(int enable);

        void sqlite3_interrupt(IntPtr db);

        int sqlite3__vfs__delete(string vfs, string pathname, int syncDir);

        int sqlite3_threadsafe();
        string sqlite3_libversion();
        int sqlite3_libversion_number();
        string sqlite3_sourceid();
        long sqlite3_memory_used();
        long sqlite3_memory_highwater(int resetFlag);
        int sqlite3_status(int op, out int current, out int highwater, int resetFlag);

        int sqlite3_db_readonly(IntPtr db, string dbName);
        string sqlite3_db_filename(IntPtr db, string att);
        string sqlite3_errmsg(IntPtr db);
        long sqlite3_last_insert_rowid(IntPtr db);
        int sqlite3_changes(IntPtr db);
        int sqlite3_total_changes(IntPtr db);
        int sqlite3_get_autocommit(IntPtr db);
        int sqlite3_busy_timeout(IntPtr db, int ms);

        int sqlite3_extended_result_codes(IntPtr db, int onoff);
        int sqlite3_errcode(IntPtr db);
        int sqlite3_extended_errcode(IntPtr db);
        string sqlite3_errstr(int rc); /* 3.7.15+ */

        int sqlite3_prepare_v2(IntPtr db, string sql, out IntPtr stmt, out string remain);
        int sqlite3_step(IntPtr stmt);
        int sqlite3_finalize(IntPtr stmt);
        int sqlite3_reset(IntPtr stmt);
        int sqlite3_clear_bindings(IntPtr stmt);
        int sqlite3_stmt_status(IntPtr stmt, int op, int resetFlg);
        string sqlite3_sql(IntPtr stmt);
        IntPtr sqlite3_db_handle(IntPtr stmt);
        IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt);

        int sqlite3_bind_zeroblob(IntPtr stmt, int index, int size);
        string sqlite3_bind_parameter_name(IntPtr stmt, int index);
        int sqlite3_bind_blob(IntPtr stmt, int index, byte[] blob);
        int sqlite3_bind_blob(IntPtr stmt, int index, byte[] blob, int nSize);
        int sqlite3_bind_double(IntPtr stmt, int index, double val);
        int sqlite3_bind_int(IntPtr stmt, int index, int val);
        int sqlite3_bind_int64(IntPtr stmt, int index, long val);
        int sqlite3_bind_null(IntPtr stmt, int index);
        int sqlite3_bind_text(IntPtr stmt, int index, string text);
        int sqlite3_bind_parameter_count(IntPtr stmt);
        int sqlite3_bind_parameter_index(IntPtr stmt, string strName);

        string sqlite3_column_database_name(IntPtr stmt, int index);
        string sqlite3_column_name(IntPtr stmt, int index);
        string sqlite3_column_origin_name(IntPtr stmt, int index);
        string sqlite3_column_table_name(IntPtr stmt, int index);
        string sqlite3_column_text(IntPtr stmt, int index);
        int sqlite3_data_count(IntPtr stmt);
        int sqlite3_column_count(IntPtr stmt);
        double sqlite3_column_double(IntPtr stmt, int index);
        int sqlite3_column_int(IntPtr stmt, int index);
        long sqlite3_column_int64(IntPtr stmt, int index);
        byte[] sqlite3_column_blob(IntPtr stmt, int index);
        int sqlite3_column_blob(IntPtr stm, int columnIndex, byte[] result, int offset);
        int sqlite3_column_bytes(IntPtr stmt, int index);
        int sqlite3_column_type(IntPtr stmt, int index);
        string sqlite3_column_decltype(IntPtr stmt, int index);

        IntPtr sqlite3_backup_init(IntPtr destDb, string destName, IntPtr sourceDb, string sourceName);
        int sqlite3_backup_step(IntPtr backup, int nPage);
        int sqlite3_backup_finish(IntPtr backup);
        int sqlite3_backup_remaining(IntPtr backup);
        int sqlite3_backup_pagecount(IntPtr backup);

        int sqlite3_blob_open(IntPtr db, byte[] db_utf8, byte[] table_utf8, byte[] col_utf8, long rowid, int flags, out IntPtr blob);
        int sqlite3_blob_open(IntPtr db, string sdb, string table, string col, long rowid, int flags, out IntPtr blob);
        int sqlite3_blob_bytes(IntPtr blob);
        int sqlite3_blob_close(IntPtr blob);
        int sqlite3_blob_write(IntPtr blob, byte[] b, int n, int offset);
        int sqlite3_blob_read(IntPtr blob, byte[] b, int n, int offset); // TODO return blob[] ?

        // these two overloads allow specification of an offset into the byte[]
        int sqlite3_blob_write(IntPtr blob, byte[] b, int bOffset, int n, int offset);
        int sqlite3_blob_read(IntPtr blob, byte[] b, int bOffset, int n, int offset); // TODO return blob[] ?

        int sqlite3_config_log(delegate_log func, object v);
        void sqlite3_commit_hook(IntPtr db, delegate_commit func, object v);
        void sqlite3_rollback_hook(IntPtr db, delegate_rollback func, object v);
        void sqlite3_trace(IntPtr db, delegate_trace func, object v);
        void sqlite3_profile(IntPtr db, delegate_profile func, object v);
        void sqlite3_progress_handler(IntPtr db, int instructions, delegate_progress_handler func, object v);
        void sqlite3_update_hook(IntPtr db, delegate_update func, object v);
        int sqlite3_create_collation(IntPtr db, string name, object v, delegate_collation func);
        int sqlite3_create_function(IntPtr db, string name, int nArg, object v, delegate_function_scalar func);
        int sqlite3_create_function(IntPtr db, string name, int nArg, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final);
        int sqlite3_create_function(IntPtr db, string name, int nArg, int flags, object v, delegate_function_scalar func);
        int sqlite3_create_function(IntPtr db, string name, int nArg, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final);

        int sqlite3_db_status(IntPtr db, int op, out int current, out int highest, int resetFlg);

        void sqlite3_result_blob(IntPtr context, byte[] val);
        void sqlite3_result_double(IntPtr context, double val);
        void sqlite3_result_error(IntPtr context, string strErr);
        void sqlite3_result_int(IntPtr context, int val);
        void sqlite3_result_int64(IntPtr context, long val);
        void sqlite3_result_null(IntPtr context);
        void sqlite3_result_text(IntPtr context, string val);
        void sqlite3_result_zeroblob(IntPtr context, int n);
        // TODO sqlite3_result_value
        void sqlite3_result_error_toobig(IntPtr context);
        void sqlite3_result_error_nomem(IntPtr context);
        void sqlite3_result_error_code(IntPtr context, int code);

        byte[] sqlite3_value_blob(IntPtr p);
        int sqlite3_value_bytes(IntPtr p);
        double sqlite3_value_double(IntPtr p);
        int sqlite3_value_int(IntPtr p);
        long sqlite3_value_int64(IntPtr p);
        int sqlite3_value_type(IntPtr p);
        string sqlite3_value_text(IntPtr p);

        int sqlite3_stmt_busy(IntPtr stmt);
        int sqlite3_stmt_readonly(IntPtr stmt);

        int sqlite3_exec(IntPtr db, string sql, delegate_exec callback, object user_data, out string errMsg);

        int sqlite3_complete(string sql);

        int sqlite3_compileoption_used(string sql);
        string sqlite3_compileoption_get(int n);

        int sqlite3_wal_autocheckpoint(IntPtr db, int n);
        int sqlite3_wal_checkpoint(IntPtr db, string dbName);
        int sqlite3_wal_checkpoint_v2(IntPtr db, string dbName, int eMode, out int logSize, out int framesCheckPointed);

        int sqlite3_table_column_metadata(IntPtr db, string dbName, string tblName, string colName, out string dataType, out string collSeq, out int notNull, out int primaryKey, out int autoInc);

        int sqlite3_set_authorizer(IntPtr db, delegate_authorizer authorizer, object user_data);

#if not // maybe never

        // because the wp8 C++ layer wouldn't link unless built against sqlcipher
        // and the functionality is available with PRAGMAs
        int sqlite3_key(IntPtr db, byte[] key, int keylen);
        int sqlite3_rekey(IntPtr db, byte[] key, int keylen);

        // because it's deprecated and harmful
        int sqlite3_prepare(IntPtr db, IntPtr pSql, int nBytes, out IntPtr stmt, out IntPtr ptrRemain);

        // because there's no good reason for a C# app to be calling the sqlite C memory allocator
        IntPtr sqlite3_malloc(int n);
        IntPtr sqlite3_realloc(IntPtr p, int n);
        void sqlite3_free(IntPtr p);

        // because these are for internal use by SQLite
        sqlite3_mutex_*

        // because it's deprecated
        int sqlite3_aggregate_count(IntPtr context); // deprecated

        // because these are inherently non-portable, and because the SQLite module
        // for WP8 doesn't even compile them in.
        int sqlite3_load_extension(IntPtr db, byte[] fileName, byte[] procName, ref IntPtr pError);

        // TODO
        void sqlite3_interrupt(IntPtr db);
        int sqlite3_file_control(IntPtr db, byte[] zDbName, int op, IntPtr pArg);
#endif

        int sqlite3_initialize();
        int sqlite3_shutdown();

        // sqlite3_config() takes a variable argument list
        int sqlite3_config(int op);
        int sqlite3_config(int op, int val);

        int sqlite3_enable_load_extension(IntPtr db, int enable);


#if not // utf16 versions, not needed since we're using utf8 everywhere
        IntPtr sqlite3_column_database_name16(IntPtr stmt, int index);
        IntPtr sqlite3_column_decltype16(IntPtr stmt, int index);
        IntPtr sqlite3_column_name16(IntPtr stmt, int index);
        IntPtr sqlite3_column_origin_name16(IntPtr stmt, int index);
        IntPtr sqlite3_column_table_name16(IntPtr stmt, int index);
        IntPtr sqlite3_column_text16(IntPtr stmt, int index);
        IntPtr sqlite3_value_text16(IntPtr p);
        int sqlite3_open16(string fileName, out IntPtr db);
        int sqlite3_bind_text16(IntPtr stmt, int index, string val, int nlen, IntPtr pvReserved);
        void sqlite3_result_error16(IntPtr context, string strName, int nLen);
        void sqlite3_result_text16(IntPtr context, string strName, int nLen, IntPtr pvReserved);
#endif

#if not
        // Since sqlite3_log() takes a variable argument list, we have to overload declarations
        // for all possible calls.  For now, we are only exposing a single string, and 
        // depend on the caller to format the string.

        void sqlite3_log(int iErrCode, byte[] zFormat);
#endif

        int sqlite3_win32_set_directory(int typ, string path);
    }
}

