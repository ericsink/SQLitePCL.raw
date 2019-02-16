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
	using System.Reflection;

	interface IGetFunctionPtr
	{
		IntPtr GetFunctionPtr(string name);
	}

	class MyDelegates
	{
		public MyDelegates(IGetFunctionPtr gf)
		{
			foreach (var p in typeof(MyDelegates).GetTypeInfo().DeclaredProperties)
			{
				var delegate_type = p.PropertyType;
				var name = p.Name;
				// TODO check for EntryPoint attribute
				var fn_ptr = gf.GetFunctionPtr(name);
				if (fn_ptr != IntPtr.Zero)
				{
					var d = Marshal.GetDelegateForFunctionPointer(fn_ptr, delegate_type);
					p.SetValue(this, d);
				}
				else
				{
					// TODO set null
				}
			}
		}

		public MyDelegateTypes.sqlite3_close sqlite3_close { get; private set; }

		public MyDelegateTypes.sqlite3_close_v2 sqlite3_close_v2 { get; private set; }

		public MyDelegateTypes.sqlite3_enable_shared_cache sqlite3_enable_shared_cache { get; private set; }

		public MyDelegateTypes.sqlite3_interrupt sqlite3_interrupt { get; private set; }

		public MyDelegateTypes.sqlite3_finalize sqlite3_finalize { get; private set; }

		public MyDelegateTypes.sqlite3_reset sqlite3_reset { get; private set; }

		public MyDelegateTypes.sqlite3_clear_bindings sqlite3_clear_bindings { get; private set; }

		public MyDelegateTypes.sqlite3_stmt_status sqlite3_stmt_status { get; private set; }

		public MyDelegateTypes.sqlite3_bind_parameter_name sqlite3_bind_parameter_name { get; private set; }

		public MyDelegateTypes.sqlite3_column_database_name sqlite3_column_database_name { get; private set; }

		public MyDelegateTypes.sqlite3_column_database_name16 sqlite3_column_database_name16 { get; private set; }

		public MyDelegateTypes.sqlite3_column_decltype sqlite3_column_decltype { get; private set; }

		public MyDelegateTypes.sqlite3_column_decltype16 sqlite3_column_decltype16 { get; private set; }

		public MyDelegateTypes.sqlite3_column_name sqlite3_column_name { get; private set; }

		public MyDelegateTypes.sqlite3_column_name16 sqlite3_column_name16 { get; private set; }

		public MyDelegateTypes.sqlite3_column_origin_name sqlite3_column_origin_name { get; private set; }

		public MyDelegateTypes.sqlite3_column_origin_name16 sqlite3_column_origin_name16 { get; private set; }

		public MyDelegateTypes.sqlite3_column_table_name sqlite3_column_table_name { get; private set; }

		public MyDelegateTypes.sqlite3_column_table_name16 sqlite3_column_table_name16 { get; private set; }

		public MyDelegateTypes.sqlite3_column_text sqlite3_column_text { get; private set; }

		public MyDelegateTypes.sqlite3_column_text16 sqlite3_column_text16 { get; private set; }

		public MyDelegateTypes.sqlite3_errmsg sqlite3_errmsg { get; private set; }

		public MyDelegateTypes.sqlite3_db_readonly sqlite3_db_readonly { get; private set; }

		public MyDelegateTypes.sqlite3_db_filename sqlite3_db_filename { get; private set; }

		public MyDelegateTypes.sqlite3_prepare sqlite3_prepare { get; private set; }

		public MyDelegateTypes.sqlite3_prepare_v2 sqlite3_prepare_v2 { get; private set; }

		public MyDelegateTypes.sqlite3_db_status sqlite3_db_status { get; private set; }

		public MyDelegateTypes.sqlite3_complete sqlite3_complete { get; private set; }

		public MyDelegateTypes.sqlite3_compileoption_used sqlite3_compileoption_used { get; private set; }

		public MyDelegateTypes.sqlite3_compileoption_get sqlite3_compileoption_get { get; private set; }

		public MyDelegateTypes.sqlite3_table_column_metadata sqlite3_table_column_metadata { get; private set; }

		public MyDelegateTypes.sqlite3_value_text sqlite3_value_text { get; private set; }

		public MyDelegateTypes.sqlite3_value_text16 sqlite3_value_text16 { get; private set; }

		public MyDelegateTypes.sqlite3_enable_load_extension sqlite3_enable_load_extension { get; private set; }

		public MyDelegateTypes.sqlite3_load_extension sqlite3_load_extension { get; private set; }

		public MyDelegateTypes.sqlite3_initialize sqlite3_initialize { get; private set; }

		public MyDelegateTypes.sqlite3_shutdown sqlite3_shutdown { get; private set; }

		public MyDelegateTypes.sqlite3_libversion sqlite3_libversion { get; private set; }

		public MyDelegateTypes.sqlite3_libversion_number sqlite3_libversion_number { get; private set; }

		public MyDelegateTypes.sqlite3_threadsafe sqlite3_threadsafe { get; private set; }

		public MyDelegateTypes.sqlite3_sourceid sqlite3_sourceid { get; private set; }

		public MyDelegateTypes.sqlite3_malloc sqlite3_malloc { get; private set; }

		public MyDelegateTypes.sqlite3_realloc sqlite3_realloc { get; private set; }

		public MyDelegateTypes.sqlite3_free sqlite3_free { get; private set; }

		public MyDelegateTypes.sqlite3_open sqlite3_open { get; private set; }

		public MyDelegateTypes.sqlite3_open_v2 sqlite3_open_v2 { get; private set; }

		public MyDelegateTypes.sqlite3_vfs_find sqlite3_vfs_find { get; private set; }

		public MyDelegateTypes.sqlite3_open16 sqlite3_open16 { get; private set; }

		public MyDelegateTypes.sqlite3_last_insert_rowid sqlite3_last_insert_rowid { get; private set; }

		public MyDelegateTypes.sqlite3_changes sqlite3_changes { get; private set; }

		public MyDelegateTypes.sqlite3_total_changes sqlite3_total_changes { get; private set; }

		public MyDelegateTypes.sqlite3_memory_used sqlite3_memory_used { get; private set; }

		public MyDelegateTypes.sqlite3_memory_highwater sqlite3_memory_highwater { get; private set; }
		
		public MyDelegateTypes.sqlite3_status sqlite3_status { get; private set; }

		public MyDelegateTypes.sqlite3_busy_timeout sqlite3_busy_timeout { get; private set; }

		public MyDelegateTypes.sqlite3_bind_blob sqlite3_bind_blob { get; private set; }

		public MyDelegateTypes.sqlite3_bind_zeroblob sqlite3_bind_zeroblob { get; private set; }

		public MyDelegateTypes.sqlite3_bind_double sqlite3_bind_double { get; private set; }

		public MyDelegateTypes.sqlite3_bind_int sqlite3_bind_int { get; private set; }

		public MyDelegateTypes.sqlite3_bind_int64 sqlite3_bind_int64 { get; private set; }

		public MyDelegateTypes.sqlite3_bind_null sqlite3_bind_null { get; private set; }

		public MyDelegateTypes.sqlite3_bind_text sqlite3_bind_text { get; private set; }

		public MyDelegateTypes.sqlite3_bind_parameter_count sqlite3_bind_parameter_count { get; private set; }

		public MyDelegateTypes.sqlite3_bind_parameter_index sqlite3_bind_parameter_index { get; private set; }

		public MyDelegateTypes.sqlite3_column_count sqlite3_column_count { get; private set; }

		public MyDelegateTypes.sqlite3_data_count sqlite3_data_count { get; private set; }

		public MyDelegateTypes.sqlite3_step sqlite3_step { get; private set; }

		public MyDelegateTypes.sqlite3_sql sqlite3_sql { get; private set; }

		public MyDelegateTypes.sqlite3_column_double sqlite3_column_double { get; private set; }

		public MyDelegateTypes.sqlite3_column_int sqlite3_column_int { get; private set; }

		public MyDelegateTypes.sqlite3_column_int64 sqlite3_column_int64 { get; private set; }

		public MyDelegateTypes.sqlite3_column_blob sqlite3_column_blob { get; private set; }

		public MyDelegateTypes.sqlite3_column_bytes sqlite3_column_bytes { get; private set; }

		public MyDelegateTypes.sqlite3_column_type sqlite3_column_type { get; private set; }

		public MyDelegateTypes.sqlite3_aggregate_count sqlite3_aggregate_count { get; private set; }

		public MyDelegateTypes.sqlite3_value_blob sqlite3_value_blob { get; private set; }

		public MyDelegateTypes.sqlite3_value_bytes sqlite3_value_bytes { get; private set; }

		public MyDelegateTypes.sqlite3_value_double sqlite3_value_double { get; private set; }

		public MyDelegateTypes.sqlite3_value_int sqlite3_value_int { get; private set; }

		public MyDelegateTypes.sqlite3_value_int64 sqlite3_value_int64 { get; private set; }

		public MyDelegateTypes.sqlite3_value_type sqlite3_value_type { get; private set; }

		public MyDelegateTypes.sqlite3_user_data sqlite3_user_data { get; private set; }

		public MyDelegateTypes.sqlite3_result_blob sqlite3_result_blob { get; private set; }

		public MyDelegateTypes.sqlite3_result_double sqlite3_result_double { get; private set; }

		public MyDelegateTypes.sqlite3_result_error sqlite3_result_error { get; private set; }

		public MyDelegateTypes.sqlite3_result_int sqlite3_result_int { get; private set; }

		public MyDelegateTypes.sqlite3_result_int64 sqlite3_result_int64 { get; private set; }

		public MyDelegateTypes.sqlite3_result_null sqlite3_result_null { get; private set; }

		public MyDelegateTypes.sqlite3_result_text sqlite3_result_text { get; private set; }

		public MyDelegateTypes.sqlite3_result_zeroblob sqlite3_result_zeroblob { get; private set; }

		// TODO sqlite3_result_value 

		public MyDelegateTypes.sqlite3_result_error_toobig sqlite3_result_error_toobig { get; private set; }

		public MyDelegateTypes.sqlite3_result_error_nomem sqlite3_result_error_nomem { get; private set; }

		public MyDelegateTypes.sqlite3_result_error_code sqlite3_result_error_code { get; private set; }

		public MyDelegateTypes.sqlite3_aggregate_context sqlite3_aggregate_context { get; private set; }

		public MyDelegateTypes.sqlite3_bind_text16 sqlite3_bind_text16 { get; private set; }

		public MyDelegateTypes.sqlite3_result_error16 sqlite3_result_error16 { get; private set; }

		public MyDelegateTypes.sqlite3_result_text16 sqlite3_result_text16 { get; private set; }

		public MyDelegateTypes.sqlite3_key sqlite3_key { get; private set; }

		public MyDelegateTypes.sqlite3_rekey sqlite3_rekey { get; private set; }

		public MyDelegateTypes.sqlite3_config_none sqlite3_config_none { get; private set; }

		public MyDelegateTypes.sqlite3_config_int sqlite3_config_int { get; private set; }

		public MyDelegateTypes.sqlite3_config_log sqlite3_config_log { get; private set; }

		public MyDelegateTypes.sqlite3_create_function_v2 sqlite3_create_function_v2 { get; private set; }

		public MyDelegateTypes.sqlite3_create_collation sqlite3_create_collation { get; private set; }

		public MyDelegateTypes.sqlite3_update_hook sqlite3_update_hook { get; private set; }

		public MyDelegateTypes.sqlite3_commit_hook sqlite3_commit_hook { get; private set; }

		public MyDelegateTypes.sqlite3_profile sqlite3_profile { get; private set; }

		public MyDelegateTypes.sqlite3_progress_handler sqlite3_progress_handler { get; private set; }

		public MyDelegateTypes.sqlite3_trace sqlite3_trace { get; private set; }

		public MyDelegateTypes.sqlite3_rollback_hook sqlite3_rollback_hook { get; private set; }

		public MyDelegateTypes.sqlite3_db_handle sqlite3_db_handle { get; private set; }

		public MyDelegateTypes.sqlite3_next_stmt sqlite3_next_stmt { get; private set; }

		public MyDelegateTypes.sqlite3_stmt_busy sqlite3_stmt_busy { get; private set; }

		public MyDelegateTypes.sqlite3_stmt_readonly sqlite3_stmt_readonly { get; private set; }

		public MyDelegateTypes.sqlite3_exec sqlite3_exec { get; private set; }

		public MyDelegateTypes.sqlite3_get_autocommit sqlite3_get_autocommit { get; private set; }

		public MyDelegateTypes.sqlite3_extended_result_codes sqlite3_extended_result_codes { get; private set; }

		public MyDelegateTypes.sqlite3_errcode sqlite3_errcode { get; private set; }

		public MyDelegateTypes.sqlite3_extended_errcode sqlite3_extended_errcode { get; private set; }

		public MyDelegateTypes.sqlite3_errstr sqlite3_errstr { get; private set; }

		public MyDelegateTypes.sqlite3_log sqlite3_log { get; private set; }

		public MyDelegateTypes.sqlite3_file_control sqlite3_file_control { get; private set; }

		public MyDelegateTypes.sqlite3_backup_init sqlite3_backup_init { get; private set; }

		public MyDelegateTypes.sqlite3_backup_step sqlite3_backup_step { get; private set; }

		public MyDelegateTypes.sqlite3_backup_finish sqlite3_backup_finish { get; private set; }

		public MyDelegateTypes.sqlite3_backup_remaining sqlite3_backup_remaining { get; private set; }

		public MyDelegateTypes.sqlite3_backup_pagecount sqlite3_backup_pagecount { get; private set; }

		public MyDelegateTypes.sqlite3_blob_open sqlite3_blob_open { get; private set; }

		public MyDelegateTypes.sqlite3_blob_write sqlite3_blob_write { get; private set; }

		public MyDelegateTypes.sqlite3_blob_read sqlite3_blob_read { get; private set; }

		public MyDelegateTypes.sqlite3_blob_bytes sqlite3_blob_bytes { get; private set; }

		public MyDelegateTypes.sqlite3_blob_close sqlite3_blob_close { get; private set; }

		public MyDelegateTypes.sqlite3_wal_autocheckpoint sqlite3_wal_autocheckpoint { get; private set; }

		public MyDelegateTypes.sqlite3_wal_checkpoint sqlite3_wal_checkpoint { get; private set; }

		public MyDelegateTypes.sqlite3_wal_checkpoint_v2 sqlite3_wal_checkpoint_v2 { get; private set; }

		public MyDelegateTypes.sqlite3_set_authorizer sqlite3_set_authorizer { get; private set; }

		public MyDelegateTypes.sqlite3_win32_set_directory sqlite3_win32_set_directory  { get; private set; }
	}

	static class MyDelegateTypes
	{
		const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_close(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_close_v2(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_enable_shared_cache(int enable);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_interrupt(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_finalize(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_reset(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_clear_bindings(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_stmt_status(IntPtr stm, int op, int resetFlg);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_bind_parameter_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_database_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_database_name16(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_decltype(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_decltype16(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_name16(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_origin_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_origin_name16(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_table_name(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_table_name16(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_text(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_text16(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_errmsg(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_db_readonly(IntPtr db, byte[] dbName);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_db_filename(IntPtr db, byte[] att);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_prepare(IntPtr db, IntPtr pSql, int nBytes, out IntPtr stmt, out IntPtr ptrRemain);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_prepare_v2(IntPtr db, IntPtr pSql, int nBytes, out IntPtr stmt, out IntPtr ptrRemain);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_db_status(IntPtr db, int op, out int current, out int highest, int resetFlg);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_complete(byte[] pSql);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_compileoption_used(byte[] pSql);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_compileoption_get(int n);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_table_column_metadata(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, out IntPtr ptrDataType, out IntPtr ptrCollSeq, out int notNull, out int primaryKey, out int autoInc);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_value_text(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_value_text16(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_enable_load_extension(
		IntPtr db, int enable);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_load_extension(
		IntPtr db, byte[] fileName, byte[] procName, ref IntPtr pError);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_initialize();

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_shutdown();

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_libversion();

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_libversion_number();

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_threadsafe();

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_sourceid();

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_malloc(int n);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_realloc(IntPtr p, int n);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_free(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_open(byte[] filename, out IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, byte[] vfs);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_vfs_find(byte[] vfs);

		[UnmanagedFunctionPointer( CALLING_CONVENTION, CharSet = CharSet.Unicode)]
		public delegate int sqlite3_open16(string fileName, out IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate long sqlite3_last_insert_rowid(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_changes(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_total_changes(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate long sqlite3_memory_used();

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate long sqlite3_memory_highwater(int resetFlag);
		
		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_status(int op, out int current, out int highwater, int resetFlag);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_busy_timeout(IntPtr db, int ms);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_bind_blob(IntPtr stmt, int index, byte[] val, int nSize, IntPtr nTransient);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_bind_zeroblob(IntPtr stmt, int index, int size);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_bind_double(IntPtr stmt, int index, double val);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_bind_int(IntPtr stmt, int index, int val);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_bind_int64(IntPtr stmt, int index, long val);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_bind_null(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_bind_text(IntPtr stmt, int index, byte[] val, int nlen, IntPtr pvReserved);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_bind_parameter_count(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_bind_parameter_index(IntPtr stmt, byte[] strName);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_column_count(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_data_count(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_step(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_sql(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate double sqlite3_column_double(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_column_int(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate long sqlite3_column_int64(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_column_blob(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_column_bytes(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_column_type(IntPtr stmt, int index);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_aggregate_count(IntPtr context);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_value_blob(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_value_bytes(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate double sqlite3_value_double(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_value_int(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate long sqlite3_value_int64(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_value_type(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_user_data(IntPtr context);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_blob(IntPtr context, byte[] val, int nSize, IntPtr pvReserved);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_double(IntPtr context, double val);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_error(IntPtr context, byte[] strErr, int nLen);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_int(IntPtr context, int val);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_int64(IntPtr context, long val);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_null(IntPtr context);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_text(IntPtr context, byte[] val, int nLen, IntPtr pvReserved);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_zeroblob(IntPtr context, int n);

		// TODO sqlite3_result_value 

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_error_toobig(IntPtr context);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_error_nomem(IntPtr context);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_result_error_code(IntPtr context, int code);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes);

		[UnmanagedFunctionPointer( CALLING_CONVENTION, CharSet = CharSet.Unicode)]
		public delegate int sqlite3_bind_text16(IntPtr stmt, int index, string val, int nlen, IntPtr pvReserved);

		[UnmanagedFunctionPointer( CALLING_CONVENTION, CharSet = CharSet.Unicode)]
		public delegate void sqlite3_result_error16(IntPtr context, string strName, int nLen);

		[UnmanagedFunctionPointer( CALLING_CONVENTION, CharSet = CharSet.Unicode)]
		public delegate void sqlite3_result_text16(IntPtr context, string strName, int nLen, IntPtr pvReserved);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_key(IntPtr db, byte[] key, int keylen);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_rekey(IntPtr db, byte[] key, int keylen);

		// Since sqlite3_config() takes a variable argument list, we have to overload declarations
		// for all possible calls that we want to use.
		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		[EntryPoint("sqlite3_config")]
		public delegate int sqlite3_config_none(int op);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		[EntryPoint("sqlite3_config")]
		public delegate int sqlite3_config_int(int op, int val);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate void callback_log(IntPtr pUserData, int errorCode, IntPtr pMessage);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		[EntryPoint("sqlite3_config")]
		public delegate int sqlite3_config_log(int op, callback_log func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate void callback_agg_function_final(IntPtr context);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate void callback_scalar_function(IntPtr context, int nArgs, IntPtr argsptr);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate void callback_agg_function_step(IntPtr context, int nArgs, IntPtr argsptr);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate void callback_destroy(IntPtr p);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_create_function_v2(IntPtr db, byte[] strName, int nArgs, int nType, IntPtr pvUser, callback_scalar_function func, callback_agg_function_step fstep, callback_agg_function_final ffinal, callback_destroy fdestroy);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate int callback_collation(IntPtr puser, int len1, IntPtr pv1, int len2, IntPtr pv2);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_create_collation(IntPtr db, byte[] strName, int nType, IntPtr pvUser, callback_collation func);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate void callback_update(IntPtr p, int typ, IntPtr db, IntPtr tbl, long rowid);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_update_hook(IntPtr db, callback_update func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate int callback_commit(IntPtr puser);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_commit_hook(IntPtr db, callback_commit func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate void callback_profile(IntPtr puser, IntPtr statement, long elapsed);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_profile(IntPtr db, callback_profile func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate int callback_progress_handler(IntPtr puser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate int callback_authorizer(IntPtr puser, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_progress_handler(IntPtr db, int instructions, callback_progress_handler func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate void callback_trace(IntPtr puser, IntPtr statement);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_trace(IntPtr db, callback_trace func, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate void callback_rollback(IntPtr puser);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_rollback_hook(IntPtr db, callback_rollback func, IntPtr pvUser);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_db_handle(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_stmt_busy(IntPtr stmt);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_stmt_readonly(IntPtr stmt);

		[UnmanagedFunctionPointer(CALLING_CONVENTION)]
		public delegate int callback_exec(IntPtr db, int n, IntPtr values, IntPtr names);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_exec(IntPtr db, byte[] strSql, callback_exec cb, IntPtr pvParam, out IntPtr errMsg);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_get_autocommit(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_extended_result_codes(IntPtr db, int onoff);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_errcode(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_extended_errcode(IntPtr db);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_errstr(int rc); /* 3.7.15+ */

		// Since sqlite3_log() takes a variable argument list, we have to overload declarations
		// for all possible calls.  For now, we are only exposing a single string, and 
		// depend on the caller to format the string.
		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate void sqlite3_log(int iErrCode, byte[] zFormat);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_file_control(IntPtr db, byte[] zDbName, int op, IntPtr pArg);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate IntPtr sqlite3_backup_init(IntPtr destDb, byte[] zDestName, IntPtr sourceDb, byte[] zSourceName);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_backup_step(IntPtr backup, int nPage);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_backup_finish(IntPtr backup);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_backup_remaining(IntPtr backup);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_backup_pagecount(IntPtr backup);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_blob_open(IntPtr db, byte[] sdb, byte[] table, byte[] col, long rowid, int flags, out IntPtr blob);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_blob_write(IntPtr blob, IntPtr b, int n, int offset);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_blob_read(IntPtr blob, IntPtr b, int n, int offset);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_blob_bytes(IntPtr blob);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_blob_close(IntPtr blob);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_wal_autocheckpoint(IntPtr db, int n);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_wal_checkpoint(IntPtr db, byte[] dbName);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_wal_checkpoint_v2(IntPtr db, byte[] dbName, int eMode, out int logSize, out int framesCheckPointed);

		[UnmanagedFunctionPointer( CALLING_CONVENTION)]
		public delegate int sqlite3_set_authorizer(IntPtr db, callback_authorizer cb, IntPtr pvUser);

		[UnmanagedFunctionPointer(CALLING_CONVENTION, CharSet=CharSet.Unicode)]
		public delegate int sqlite3_win32_set_directory (uint directoryType, string directoryPath);

	}
}


