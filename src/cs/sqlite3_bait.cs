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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Implements the <see cref="ISQLite3Provider"/> interface
    /// </summary>
    public sealed class SQLite3Provider_bait : ISQLite3Provider
    {
        private const string GRIPE = "You need to call SQLitePCL.raw.SetProvider().  If you are using a bundle package, this is done by calling SQLitePCL.Batteries.Init().";

        public SQLite3Provider_bait()
        {
        }

        int ISQLite3Provider.sqlite3_open(string filename, out IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_open_v2(string filename, out IntPtr db, int flags, string vfs)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_close_v2(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_close(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_enable_shared_cache(int enable)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_interrupt(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3__vfs__delete(string vfs, string pathname, int syncDir)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_exec(IntPtr db, string sql, delegate_exec func, object user_data, out string errMsg)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_complete(string sql)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_compileoption_get(int n)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_compileoption_used(string s)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_table_column_metadata(IntPtr db, string dbName, string tblName, string colName, out string dataType, out string collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_prepare_v2(IntPtr db, string sql, out IntPtr stm, out string remain)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_sql(IntPtr stmt)
        {
            throw new Exception(GRIPE);
        }

        IntPtr ISQLite3Provider.sqlite3_db_handle(IntPtr stmt)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_blob_open(IntPtr db, byte[] db_utf8, byte[] table_utf8, byte[] col_utf8, long rowid, int flags, out IntPtr blob)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_blob_open(IntPtr db, string sdb, string table, string col, long rowid, int flags, out IntPtr blob)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_blob_bytes(IntPtr blob)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_blob_close(IntPtr blob)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_blob_read(IntPtr blob, byte[] b, int n, int offset)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_blob_write(IntPtr blob, byte[] b, int n, int offset)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_blob_read(IntPtr blob, byte[] b, int bOffset, int n, int offset)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_blob_write(IntPtr blob, byte[] b, int bOffset, int n, int offset)
        {
            throw new Exception(GRIPE);
        }

        IntPtr ISQLite3Provider.sqlite3_backup_init(IntPtr destDb, string destName, IntPtr sourceDb, string sourceName)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_backup_step(IntPtr backup, int nPage)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_backup_finish(IntPtr backup)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_backup_remaining(IntPtr backup)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_backup_pagecount(IntPtr backup)
        {
            throw new Exception(GRIPE);
        }

        IntPtr ISQLite3Provider.sqlite3_next_stmt(IntPtr db, IntPtr stmt)
        {
            throw new Exception(GRIPE);
        }

        long ISQLite3Provider.sqlite3_last_insert_rowid(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_changes(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_total_changes(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_extended_result_codes(IntPtr db, int onoff)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_errstr(int rc)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_errcode(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_extended_errcode(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_busy_timeout(IntPtr db, int ms)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_get_autocommit(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_db_readonly(IntPtr db, string dbName)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_db_filename(IntPtr db, string att)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_errmsg(IntPtr db)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_libversion()
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_libversion_number()
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_threadsafe()
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_initialize()
        {
            throw new Exception(GRIPE);
        }
        int ISQLite3Provider.sqlite3_shutdown()
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_config(int op)
        {
            throw new Exception(GRIPE);
        }
        int ISQLite3Provider.sqlite3_config(int op, int val)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_enable_load_extension(IntPtr db, int onoff)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_config_log(delegate_log func, object v)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_commit_hook(IntPtr db, delegate_commit func, object v)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_scalar func)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_scalar func)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_create_function(IntPtr db, string name, int nargs, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_db_status(IntPtr db, int op, out int current, out int highest, int resetFlg)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_create_collation(IntPtr db, string name, object v, delegate_collation func)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_update_hook(IntPtr db, delegate_update func, object v)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_rollback_hook(IntPtr db, delegate_rollback func, object v)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_trace(IntPtr db, delegate_trace func, object v)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_profile(IntPtr db, delegate_profile func, object v)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_progress_handler(IntPtr db, int instructions, delegate_progress_handler func, object v)
        {
            throw new Exception(GRIPE);
        }

        long ISQLite3Provider.sqlite3_memory_used()
        {
            throw new Exception(GRIPE);
        }

        long ISQLite3Provider.sqlite3_memory_highwater(int resetFlag)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_sourceid()
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_int64(IntPtr ctx, long val)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_int(IntPtr ctx, int val)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_double(IntPtr ctx, double val)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_null(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_error(IntPtr ctx, string val)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, string val)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_blob(IntPtr ctx, byte[] blob)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_zeroblob(IntPtr ctx, int n)
        {
            throw new Exception(GRIPE);
        }

        // TODO sqlite3_result_value

        void ISQLite3Provider.sqlite3_result_error_toobig(IntPtr ctx)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_error_nomem(IntPtr ctx)
        {
            throw new Exception(GRIPE);
        }

        void ISQLite3Provider.sqlite3_result_error_code(IntPtr ctx, int code)
        {
            throw new Exception(GRIPE);
        }

        byte[] ISQLite3Provider.sqlite3_value_blob(IntPtr p)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_value_bytes(IntPtr p)
        {
            throw new Exception(GRIPE);
        }

        double ISQLite3Provider.sqlite3_value_double(IntPtr p)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_value_int(IntPtr p)
        {
            throw new Exception(GRIPE);
        }

        long ISQLite3Provider.sqlite3_value_int64(IntPtr p)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_value_type(IntPtr p)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_value_text(IntPtr p)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_int(IntPtr stm, int paramIndex, int val)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_int64(IntPtr stm, int paramIndex, long val)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_text(IntPtr stm, int paramIndex, string t)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_double(IntPtr stm, int paramIndex, double val)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_blob(IntPtr stm, int paramIndex, byte[] blob)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_blob(IntPtr stm, int paramIndex, byte[] blob, int nSize)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_zeroblob(IntPtr stm, int paramIndex, int size)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_null(IntPtr stm, int paramIndex)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_parameter_count(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_bind_parameter_name(IntPtr stm, int paramIndex)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_bind_parameter_index(IntPtr stm, string paramName)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_step(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_stmt_busy(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_stmt_readonly(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_column_int(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        long ISQLite3Provider.sqlite3_column_int64(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_column_text(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_column_decltype(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        double ISQLite3Provider.sqlite3_column_double(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        byte[] ISQLite3Provider.sqlite3_column_blob(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_column_blob(IntPtr stm, int columnIndex, byte[] result, int offset)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_column_type(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_column_bytes(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_column_count(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_data_count(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_column_name(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_column_origin_name(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_column_table_name(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        string ISQLite3Provider.sqlite3_column_database_name(IntPtr stm, int columnIndex)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_reset(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_clear_bindings(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_stmt_status(IntPtr stm, int op, int resetFlg)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_finalize(IntPtr stm)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_wal_autocheckpoint(IntPtr db, int n)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_wal_checkpoint(IntPtr db, string dbName)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_wal_checkpoint_v2(IntPtr db, string dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_set_authorizer(IntPtr db, delegate_authorizer func, object v)
        {
            throw new Exception(GRIPE);
        }

        int ISQLite3Provider.sqlite3_win32_set_directory(int typ, string path)
        {
            throw new Exception(GRIPE);
        }
    }
}
