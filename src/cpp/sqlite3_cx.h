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

#pragma once
namespace SQLitePCL
{
	namespace cppinterop
	{ 
				public ref class SQLite3RuntimeProvider sealed
				{
				public:
#if not
                    static int32 set_temp_directory(int64 path);
#endif
					static int32 sqlite3_open(int64 filename, int64 pdb);
					static int32 sqlite3_open_v2(int64 filename, int64 pdb, int32 flags, int64 vfs);

					static int32 sqlite3_close_v2(int64 db);
					static int32 sqlite3_close(int64 db);

					static int32 sqlite3_enable_shared_cache(int32 enable);

                    static void sqlite3_interrupt(int64 db);

                    static int64 sqlite3_commit_hook(int64 db, int64 func, int64 v);
                    static int64 sqlite3_rollback_hook(int64 db, int64 func, int64 v);
                    static int64 sqlite3_trace(int64 db, int64 func, int64 v);
                    static int64 sqlite3_profile(int64 db, int64 func, int64 v);
                    static void sqlite3_progress_handler(int64 db, int32 instructions, int64 func, int64 v);
                    static int64 sqlite3_update_hook(int64 db, int64 func, int64 v);
                    static int32 sqlite3_create_collation(int64 db, int64 name, int32 textrep, int64 v, int64 func);

                    static int32 sqlite3_exec(int64 db, int64 sql, int64 cb, int64 v, int64 perr);

                    static int32 sqlite3_compileoption_used(int64 s);
                    static int64 sqlite3_compileoption_get(int32 n);

                    static int32 sqlite3_table_column_metadata(int64 db, int64 dbName, int64 tableName, int64 columnName, int64 dataType, int64 collSeq, int64 notNull, int64 primaryKey, int64 autoInc);

                    static int32 sqlite3_complete(int64 sql);

					static int32 sqlite3_prepare_v2(int64 db, int64 zSql, int32 nByte, int64 ppStmpt, int64 pzTail);

					static int32 sqlite3_db_status(int64 db, int32 op, int64 current, int64 highest, int32 resetFlg);

					static int64 sqlite3_errmsg(int64 db);

					static int32 sqlite3_db_readonly(int64 db, int64 dbName);
					
					static int64 sqlite3_db_filename(int64 db, int64 att);

					static int32 sqlite3__vfs__delete(int64 vfs, int64 pathname, int32 dirsync);

					static int64 sqlite3_sql(int64 stmt);

					static int64 sqlite3_db_handle(int64 stmt);

					static int64 sqlite3_next_stmt(int64 db, int64 stmt);

					static int64 sqlite3_last_insert_rowid(int64 db);

					static int32 sqlite3_changes(int64 db);

                                        static int32 sqlite3_total_changes(int64 db);

					static int32 sqlite3_errcode(int64 db);

					static int32 sqlite3_extended_errcode(int64 db);

					static int32 sqlite3_extended_result_codes(int64 db, int32 onoff);

					static int64 sqlite3_errstr(int32 rc);

					static int32 sqlite3_busy_timeout(int64 db, int32 ms);

					static int32 sqlite3_get_autocommit(int64 db);

					static int64 sqlite3_libversion();

					static int64 sqlite3_sourceid();

					static int32 sqlite3_libversion_number();
					static int32 sqlite3_threadsafe();
					static int32 sqlite3_initialize();
					static int32 sqlite3_shutdown();
					static int32 sqlite3_config_none(int32 op);
					static int32 sqlite3_config_int(int32 op, int32 val);
					static int32 sqlite3_enable_load_extension(int64 db, int32 onoff);

					static int64 sqlite3_memory_used();
					static int64 sqlite3_memory_highwater(int32 resetFlag);

					static int32 sqlite3_status(int32 op,  int64 current, int64 highwater, int32 resetFlag);

                    static void sqlite3_free(int64 p);

                    static int32 sqlite3_blob_open(int64 db, int64 sdb, int64 table, int64 col, int64 rowid, int32 flags, int64 pblob);
                    static int32 sqlite3_blob_bytes(int64 blob);
                    static int32 sqlite3_blob_close(int64 blob);
                    static int32 sqlite3_blob_read(int64 blob, int64 ptr, int32 n, int32 offset);
                    static int32 sqlite3_blob_write(int64 blob, int64 ptr, int32 n, int32 offset);

                    static int64 sqlite3_backup_init(int64 destDb, int64 zDestName, int64 sourceDb, int64 zSourceName);
                    static int32 sqlite3_backup_step(int64 backup, int32 nPage);
                    static int32 sqlite3_backup_finish(int64 backup);
                    static int32 sqlite3_backup_remaining(int64 backup);
                    static int32 sqlite3_backup_pagecount(int64 backup);

                    static int32 sqlite3_create_function_v2(int64 db, int64 name, int32 nargs, int32 etextrep, int64 v, int64 func, int64 step, int64 fin, int64 destroy);
					static int64 sqlite3_aggregate_context(int64 ctx, int32 n);
					static int64 sqlite3_user_data(int64 ctx);
					static void sqlite3_result_text(int64 ctx, int64 val, int32 length, int64 destructor);
					static void sqlite3_result_int(int64 ctx, int32 val);
					static void sqlite3_result_int64(int64 ctx, int64 val);
					static void sqlite3_result_double(int64 ctx, double val);
					static void sqlite3_result_null(int64 ctx);
					static void sqlite3_result_error(int64 ctx, int64 val, int32 length);
					static void sqlite3_result_blob(int64 ctx, int64 val, int32 length, int64 destructor);
					static void sqlite3_result_zeroblob(int64 ctx, int32 n);
					static void sqlite3_result_error_toobig(int64 ctx);
					static void sqlite3_result_error_nomem(int64 ctx);
					static void sqlite3_result_error_code(int64 ctx, int code); 

                    static int32 sqlite3_value_int(int64 v);
                    static int32 sqlite3_value_type(int64 v);
                    static int32 sqlite3_value_bytes(int64 v);
                    static double sqlite3_value_double(int64 v);
                    static int64 sqlite3_value_int64(int64 v);
					static int64 sqlite3_value_text(int64 v);
					static int64 sqlite3_value_blob(int64 v);

					static int32 sqlite3_bind_int(int64 stmHandle, int32 iParam, int32 val);

					static int32 sqlite3_bind_int64(int64 stmHandle, int32 iParam, int64 val);

					static int32 sqlite3_bind_text(int64 stmHandle, int32 iParam, int64 val, int32 length, int64 destructor);

					static int32 sqlite3_bind_double(int64 stmHandle, int32 iParam, float64 val);

					static int32 sqlite3_bind_blob(int64 stmHandle, int32 iParam, int64 val, int32 length, int64 destructor);

					static int32 sqlite3_bind_zeroblob(int64 stmHandle, int32 iParam, int32 size);

					static int32 sqlite3_bind_null(int64 stmHandle, int32 iParam);

					static int32 sqlite3_bind_parameter_count(int64 stmHandle);

					static int64 sqlite3_bind_parameter_name(int64 stmHandle, int32 iParam);

					static int32 sqlite3_bind_parameter_index(int64 stmHandle, int64 paramName);

					static int32 sqlite3_step(int64 stmHandle);

					static int32 sqlite3_stmt_busy(int64 stmHandle);
					static int32 sqlite3_stmt_readonly(int64 stmHandle);

					static int32 sqlite3_column_int(int64 stmHandle, int32 iCol);

					static int64 sqlite3_column_int64(int64 stmHandle, int32 iCol);

					static int64 sqlite3_column_text(int64 stmHandle, int32 iCol);

					static int64 sqlite3_column_decltype(int64 stmHandle, int32 iCol);

					static float64 sqlite3_column_double(int64 stmHandle, int32 iCol);

					static int64 sqlite3_column_blob(int64 stmHandle, int32 iCol);

					static int32 sqlite3_column_type(int64 stmHandle, int32 iCol);

					static int32 sqlite3_column_bytes(int64 stmHandle, int32 iCol);

					static int32 sqlite3_column_count(int64 stmHandle);

					static int32 sqlite3_data_count(int64 stmHandle);

					static int64 sqlite3_column_name(int64 stmHandle, int32 iCol);

					static int64 sqlite3_column_origin_name(int64 stmHandle, int32 iCol);

					static int64 sqlite3_column_table_name(int64 stmHandle, int32 iCol);

					static int64 sqlite3_column_database_name(int64 stmHandle, int32 iCol);

					static int32 sqlite3_reset(int64 stmHandle);

					static int32 sqlite3_clear_bindings(int64 stmHandle);

					static int32 sqlite3_finalize(int64 stmHandle);

					static int32 sqlite3_wal_autocheckpoint(int64 db, int32 n);

					static int32 sqlite3_wal_checkpoint(int64 db, int64 dbName);

					static int32 sqlite3_wal_checkpoint_v2(int64 db, int64 dbName, int32 eMode, int64 logSize, int64 framesCheckPointed);

					static int32 sqlite3_stmt_status(int64 stmHandle, int32 op, int32 resetFlg);
	
					static int32 sqlite3_set_authorizer(int64 db, int64 func, int64 v);

				};
	}
}

