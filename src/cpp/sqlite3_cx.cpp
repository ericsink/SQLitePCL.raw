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

#include "sqlite3.h"

#ifdef NEED_TYPEDEFS
typedef int int32;
typedef long long int64;
typedef double float64;
#endif

#include "sqlite3_cx.h"

using namespace SQLitePCL::cppinterop;
using namespace std;

#if 0
int32 SQLite3RuntimeProvider::set_temp_directory(int64 path)
{
    extern char* sqlite3_temp_directory;
    ::sqlite3_free(sqlite3_temp_directory);
    sqlite3_temp_directory = ::sqlite3_mprintf("%s", (const char*) path);
    return 0;
}
#endif

int32 SQLite3RuntimeProvider::sqlite3_open(int64 filename, int64 pdb)
{
	sqlite3* sqlite3 = nullptr;

	int32 result = ::sqlite3_open((const char*)filename, &sqlite3);

	if (pdb)
	{
        int64* p = (int64*) pdb;
        *p = (int64)sqlite3;
	}

	return result;
}

int32 SQLite3RuntimeProvider::sqlite3_open_v2(int64 filename, int64 pdb, int32 flags, int64 vfs)
{
	sqlite3* sqlite3 = nullptr;

	int32 result = ::sqlite3_open_v2((const char*)filename, &sqlite3, flags, (const char*) vfs);

	if (pdb)
	{
        int64* p = (int64*) pdb;
        *p = (int64)sqlite3;
	}

	return result;
}

int32 SQLite3RuntimeProvider::sqlite3_close_v2(int64 db)
{
	return ::sqlite3_close_v2((sqlite3*)db);
}

int32 SQLite3RuntimeProvider::sqlite3_enable_shared_cache(int32 enable)
{
	return ::sqlite3_enable_shared_cache(enable);
}

int64 SQLite3RuntimeProvider::sqlite3_commit_hook(int64 db, int64 func, int64 v)
{
	return (int64) ::sqlite3_commit_hook((sqlite3*)db, (int(*)(void*)) func, (void*)v);
}

int64 SQLite3RuntimeProvider::sqlite3_rollback_hook(int64 db, int64 func, int64 v)
{
	return (int64) ::sqlite3_rollback_hook((sqlite3*)db, (void(*)(void*)) func, (void*)v);
}

int64 SQLite3RuntimeProvider::sqlite3_update_hook(int64 db, int64 func, int64 v)
{
	return (int64) ::sqlite3_update_hook((sqlite3*)db, (void(*)(void*,int,const char*,const char*,sqlite3_int64)) func, (void*)v);
}

int64 SQLite3RuntimeProvider::sqlite3_trace(int64 db, int64 func, int64 v)
{
	return (int64) ::sqlite3_trace((sqlite3*)db, (void(*)(void*,const char*)) func, (void*)v);
}

int64 SQLite3RuntimeProvider::sqlite3_profile(int64 db, int64 func, int64 v)
{
	return (int64) ::sqlite3_profile((sqlite3*)db, (void(*)(void*,const char*,sqlite3_uint64)) func, (void*)v);
}

void SQLite3RuntimeProvider::sqlite3_progress_handler(int64 db, int32 instructions, int64 func, int64 v)
{
	::sqlite3_progress_handler((sqlite3*)db, instructions, (int(*)(void*)) func, (void*)v);
}

int32 SQLite3RuntimeProvider::sqlite3_create_collation(int64 db, int64 name, int32 textrep, int64 v, int64 func)
{
    return ::sqlite3_create_collation((sqlite3*)db, (const char*) name, textrep, (void*) v, (int(*)(void*,int,const void*,int,const void*)) func);
}

int32 SQLite3RuntimeProvider::sqlite3_close(int64 db)
{
	return ::sqlite3_close((sqlite3*)db);
}

void SQLite3RuntimeProvider::sqlite3_interrupt(int64 db)
{
    ::sqlite3_interrupt((sqlite3*)db);
}

void SQLite3RuntimeProvider::sqlite3_free(int64 p)
{
    ::sqlite3_free((void*) p);
}

int32 SQLite3RuntimeProvider::sqlite3_exec(int64 db, int64 sql, int64 cb, int64 v, int64 perr)
{
    char* psz_errmsg = nullptr;
    int32 result = ::sqlite3_exec((sqlite3*) db, (const char*) sql, (int(*)(void*,int,char**,char**)) cb, (void*) v, &psz_errmsg);
    if (perr)
    {
        int64* p = (int64*) perr;
        *p = (int64) psz_errmsg;
    }
    else
    {
        ::sqlite3_free(psz_errmsg);
    }
    return result;
}

int32 SQLite3RuntimeProvider::sqlite3_complete(int64 sql)
{
    return ::sqlite3_complete((const char*) sql);
}

int32 SQLite3RuntimeProvider::sqlite3_compileoption_used(int64 s)
{
    return ::sqlite3_compileoption_used((const char*) s);
}

int64 SQLite3RuntimeProvider::sqlite3_compileoption_get(int32 n)
{
    return (int64) ::sqlite3_compileoption_get(n);
}

int32 SQLite3RuntimeProvider::sqlite3_table_column_metadata(int64 db, int64 dbName, int64 tableName, int64 columnName, int64 dataType, int64 collSeq, int64 notNull, int64 primaryKey, int64 autoInc)
{
	const char* pzDataType = nullptr;
	const char* pzCollSeq = nullptr;
	int32 pNotNull = 0;
	int32 pPrimaryKey = 0;
	int32 pAutoinc = 0;

	int32 result = ::sqlite3_table_column_metadata(
		((sqlite3 *) db), ((const char *) dbName), ((const char *) tableName), ((const char *) columnName), 
		&pzDataType, &pzCollSeq, &pNotNull, &pPrimaryKey, &pAutoinc);

	if (dataType)
	{
		int64* p = (int64*)dataType;
		*p = (int64)pzDataType;
	}

	if (collSeq)
	{
		int64* p = (int64*)collSeq;
		*p = (int64)pzCollSeq;
	}

	if (notNull)
	{
		int64* p = (int64*)notNull;
		*p = pNotNull;
	}

	if (primaryKey)
	{
		int64* p = (int64*)primaryKey;
		*p = pPrimaryKey;
	}

	if (autoInc)
	{
		int64* p = (int64*)autoInc;
		*p = pAutoinc;
	}

	return result;
}

int32 SQLite3RuntimeProvider::sqlite3_prepare_v2(int64 db, int64 zSql, int32 nByte, int64 ppStmpt, int64 ppTail)
{
	sqlite3_stmt* sqlite3_stmt = nullptr;
    const char* psz_tail = nullptr;

	int32 result = ::sqlite3_prepare_v2((sqlite3*) db,(const char*)zSql,nByte, &sqlite3_stmt,&psz_tail);

	if (ppStmpt)
	{
        int64* p = (int64*) ppStmpt;
		*p = (int64)sqlite3_stmt;
	}

    if (ppTail)
    {
        int64* p = (int64*) ppTail;
        *p = (int64) psz_tail;
    }

	return result;
}

int32 SQLite3RuntimeProvider::sqlite3_db_status(int64 db, int32 op, int64 current, int64 highest, int32 resetFlg)
{
    int32 pCur = 0; 
    int32 pHiwtr = 0;

    int32 result = ::sqlite3_db_status((sqlite3*) db, op, &pCur, &pHiwtr, resetFlg);

    if (current)
    {
        int64* p = (int64*) current;
        *p = pCur;
    }

    if (resetFlg)
    {
        int64* p = (int64*) highest;
        *p = pHiwtr;
    }

    return result;
}

int64 SQLite3RuntimeProvider::sqlite3_errmsg(int64 db)
{
	return (int64)::sqlite3_errmsg((sqlite3*)db);
}

int32 SQLite3RuntimeProvider::sqlite3_db_readonly(int64 db, int64 dbName)
{
        return (int32)::sqlite3_db_readonly((sqlite3*)db, (const char*)dbName);
}

int64 SQLite3RuntimeProvider::sqlite3_db_filename(int64 db, int64 att)
{
	return (int64)::sqlite3_db_filename((sqlite3*)db, (const char*)att);
}

int32 SQLite3RuntimeProvider::sqlite3__vfs__delete(int64 vfs, int64 pathname, int32 dirsync)
{
	sqlite3_vfs* pvfs = ::sqlite3_vfs_find((const char*) vfs);
	return pvfs->xDelete(pvfs, (const char*) pathname, dirsync);
}

int64 SQLite3RuntimeProvider::sqlite3_sql(int64 stmt)
{
	return (int64)::sqlite3_sql((sqlite3_stmt*)stmt);
}

int64 SQLite3RuntimeProvider::sqlite3_db_handle(int64 stmt)
{
	return (int64)::sqlite3_db_handle((sqlite3_stmt*)stmt);
}

int64 SQLite3RuntimeProvider::sqlite3_next_stmt(int64 db, int64 stmt)
{
	return (int64)::sqlite3_next_stmt((sqlite3*)db, (sqlite3_stmt*)stmt);
}

int64 SQLite3RuntimeProvider::sqlite3_memory_used()
{
	return (int64)::sqlite3_memory_used();
}

int64 SQLite3RuntimeProvider::sqlite3_memory_highwater(int32 resetFlag)
{
	return (int64)::sqlite3_memory_highwater(resetFlag);
}

int32 SQLite3RuntimeProvider::sqlite3_status(int32 op, int64 current, int64 highwater, int32 resetFlag)
{
	int32 pCurrent; 
        int32 pHighwater;

	int32 result = ::sqlite3_status(op, &pCurrent, &pHighwater, resetFlag);

	if (current)
	{
		int64* p = (int64*) current;
		*p = pCurrent;
	} 

	if (highwater)
	{
		int64* p = (int64*) highwater;
		*p = pHighwater;
	}

	return result;
}

int32 SQLite3RuntimeProvider::sqlite3_threadsafe(void)
{
	return (int32)::sqlite3_threadsafe();
}

int32 SQLite3RuntimeProvider::sqlite3_initialize(void)
{
	return (int32)::sqlite3_initialize();
}

int32 SQLite3RuntimeProvider::sqlite3_shutdown(void)
{
	return (int32)::sqlite3_shutdown();
}

int32 SQLite3RuntimeProvider::sqlite3_config_none(int32 op)
{
	return (int32)::sqlite3_config(op);
}

int32 SQLite3RuntimeProvider::sqlite3_config_int(int32 op, int32 val)
{
	return (int32)::sqlite3_config(op, val);
}

int32 SQLite3RuntimeProvider::sqlite3_enable_load_extension(int64 db, int32 onoff)
{
	return (int32)::sqlite3_enable_load_extension((sqlite3*) db, onoff);
}

int32 SQLite3RuntimeProvider::sqlite3_libversion_number(void)
{
	return (int32)::sqlite3_libversion_number();
}

int64 SQLite3RuntimeProvider::sqlite3_libversion(void)
{
	return (int64)::sqlite3_libversion();
}

int64 SQLite3RuntimeProvider::sqlite3_sourceid(void)
{
	return (int64)::sqlite3_sourceid();
}

int64 SQLite3RuntimeProvider::sqlite3_last_insert_rowid(int64 db)
{
	return (int64)::sqlite3_last_insert_rowid((sqlite3*)db);
}

int32 SQLite3RuntimeProvider::sqlite3_errcode(int64 db)
{
    return (int32)::sqlite3_errcode((sqlite3*)db);
}

int32 SQLite3RuntimeProvider::sqlite3_extended_errcode(int64 db)
{
    return (int32)::sqlite3_extended_errcode((sqlite3*)db);
}

int32 SQLite3RuntimeProvider::sqlite3_extended_result_codes(int64 db, int32 onoff)
{
	return (int32)::sqlite3_extended_result_codes((sqlite3*)db, onoff);
}

int64 SQLite3RuntimeProvider::sqlite3_errstr(int32 rc)
{
	return (int64)::sqlite3_errstr(rc);
}

int32 SQLite3RuntimeProvider::sqlite3_changes(int64 db)
{
	return (int32)::sqlite3_changes((sqlite3*)db);
}

int32 SQLite3RuntimeProvider::sqlite3_total_changes(int64 db)
{
        return (int32)::sqlite3_total_changes((sqlite3*)db);
}

int32 SQLite3RuntimeProvider::sqlite3_busy_timeout(int64 db, int32 ms)
{
	return (int32)::sqlite3_busy_timeout((sqlite3*)db, ms);
}

int32 SQLite3RuntimeProvider::sqlite3_get_autocommit(int64 db)
{
	return (int64)::sqlite3_get_autocommit((sqlite3*)db);
}

int32 SQLite3RuntimeProvider::sqlite3_blob_open(int64 db, int64 sdb, int64 table, int64 col, int64 rowid, int32 flags, int64 pblob)
{
	sqlite3_blob* b = nullptr;

    int32 result = (int32)::sqlite3_blob_open((sqlite3*)db, (const char*) sdb, (const char*) table, (const char*) col, rowid, flags, &b);

    if (pblob)
    {
        int64* p = (int64*) pblob;
        *p = (int64)b;
    }

    return result;
}

int32 SQLite3RuntimeProvider::sqlite3_blob_bytes(int64 blob)
{
    return (int32)::sqlite3_blob_bytes((sqlite3_blob*)blob);
}

int32 SQLite3RuntimeProvider::sqlite3_blob_close(int64 blob)
{
    return (int32)::sqlite3_blob_close((sqlite3_blob*)blob);
}

int32 SQLite3RuntimeProvider::sqlite3_blob_read(int64 blob, int64 ptr, int32 n, int32 offset)
{
    return (int32)::sqlite3_blob_read((sqlite3_blob*)blob, (void*) ptr, n, offset);
}

int32 SQLite3RuntimeProvider::sqlite3_blob_write(int64 blob, int64 ptr, int32 n, int32 offset)
{
    return (int32)::sqlite3_blob_write((sqlite3_blob*)blob, (void*) ptr, n, offset);
}

int64 SQLite3RuntimeProvider::sqlite3_backup_init(int64 destDb, int64 destName, int64 sourceDb, int64 sourceName)
{
    return (int64)::sqlite3_backup_init((sqlite3*)destDb, (const char*) destName, (sqlite3*)sourceDb, (const char*) sourceName);
}

int32 SQLite3RuntimeProvider::sqlite3_backup_step(int64 backup, int32 nPage)
{
    return (int32)::sqlite3_backup_step((sqlite3_backup*)backup, nPage);
}

int32 SQLite3RuntimeProvider::sqlite3_backup_finish(int64 backup)
{
    return (int32)::sqlite3_backup_finish((sqlite3_backup*)backup);
}

int32 SQLite3RuntimeProvider::sqlite3_backup_remaining(int64 backup)
{
    return (int32)::sqlite3_backup_remaining((sqlite3_backup*)backup);
}

int32 SQLite3RuntimeProvider::sqlite3_backup_pagecount(int64 backup)
{
    return (int32)::sqlite3_backup_pagecount((sqlite3_backup*)backup);
}

int32 SQLite3RuntimeProvider::sqlite3_bind_int(int64 stmHandle, int32 iParam, int32 val)
{
	return ::sqlite3_bind_int((sqlite3_stmt*)stmHandle, iParam, val);
}

int32 SQLite3RuntimeProvider::sqlite3_bind_int64(int64 stmHandle, int32 iParam, int64 val)
{
	return ::sqlite3_bind_int64((sqlite3_stmt*)stmHandle,iParam,(sqlite3_int64)val);
}

int32 SQLite3RuntimeProvider::sqlite3_bind_text(int64 stmHandle, int32 iParam, int64 val, int32 length, int64 destructor)
{	
	return ::sqlite3_bind_text((sqlite3_stmt*)stmHandle, iParam, (const char*)val, length, (void(*)(void*))destructor);
}

int32 SQLite3RuntimeProvider::sqlite3_bind_double(int64 stmHandle, int32 iParam, float64 val)
{
	return ::sqlite3_bind_double((sqlite3_stmt*)stmHandle, iParam, val);
}

int32 SQLite3RuntimeProvider::sqlite3_bind_blob(int64 stmHandle, int32 iParam, int64 val, int32 length, int64 destructor)
{
	return ::sqlite3_bind_blob((sqlite3_stmt*)stmHandle, iParam, (void*) val, length, (void(*)(void*))destructor);
}

int32 SQLite3RuntimeProvider::sqlite3_bind_zeroblob(int64 stmHandle, int32 iParam, int32 size)
{
	return ::sqlite3_bind_zeroblob((sqlite3_stmt*)stmHandle, iParam, size);
}

int32 SQLite3RuntimeProvider::sqlite3_bind_null(int64 stmHandle, int32 iParam)
{
	return ::sqlite3_bind_null((sqlite3_stmt*)stmHandle, iParam);
}

int32 SQLite3RuntimeProvider::sqlite3_create_function_v2(int64 db, int64 name, int32 nargs, int32 etextrep, int64 v, int64 func, int64 step, int64 fin, int64 destroy)
{
    return ::sqlite3_create_function_v2((sqlite3*) db, (const char*) name, nargs, etextrep, (void*) v, (void(*)(sqlite3_context*,int,sqlite3_value**)) func, (void(*)(sqlite3_context*,int,sqlite3_value**)) step, (void(*)(sqlite3_context*)) fin, (void(*)(void*)) destroy);
}

int64 SQLite3RuntimeProvider::sqlite3_user_data(int64 ctx)
{
    return (int64) ::sqlite3_user_data((sqlite3_context*)ctx);
}

void SQLite3RuntimeProvider::sqlite3_result_text(int64 ctx, int64 val, int32 length, int64 destructor)
{
    ::sqlite3_result_text((sqlite3_context*) ctx, (const char*) val, length, (void(*)(void*))destructor);
}

int64 SQLite3RuntimeProvider::sqlite3_aggregate_context(int64 ctx, int32 n)
{
    return (int64) ::sqlite3_aggregate_context((sqlite3_context*) ctx, n);
}

void SQLite3RuntimeProvider::sqlite3_result_int(int64 ctx, int32 val)
{
    ::sqlite3_result_int((sqlite3_context*) ctx, val);
}

void SQLite3RuntimeProvider::sqlite3_result_int64(int64 ctx, int64 val)
{
    ::sqlite3_result_int64((sqlite3_context*) ctx, val);
}

void SQLite3RuntimeProvider::sqlite3_result_double(int64 ctx, double val)
{
    ::sqlite3_result_double((sqlite3_context*) ctx, val);
}

void SQLite3RuntimeProvider::sqlite3_result_null(int64 ctx)
{
    ::sqlite3_result_null((sqlite3_context*) ctx);
}

void SQLite3RuntimeProvider::sqlite3_result_error(int64 ctx, int64 val, int32 length)
{
    ::sqlite3_result_error((sqlite3_context*) ctx, (const char*) val, length);
}

void SQLite3RuntimeProvider::sqlite3_result_blob(int64 ctx, int64 val, int32 length, int64 destructor)
{
	return ::sqlite3_result_blob((sqlite3_context*)ctx, (void*) val, length, (void(*)(void*))destructor);
}

void SQLite3RuntimeProvider::sqlite3_result_zeroblob(int64 ctx, int32 n)
{
    ::sqlite3_result_zeroblob((sqlite3_context*) ctx, n);
}

// TODO sqlite3_result_value

void SQLite3RuntimeProvider::sqlite3_result_error_toobig(int64 ctx)
{
    ::sqlite3_result_error_toobig((sqlite3_context*)ctx);
}

void SQLite3RuntimeProvider::sqlite3_result_error_nomem(int64 ctx)
{
    ::sqlite3_result_error_nomem((sqlite3_context*)ctx);
}

void SQLite3RuntimeProvider::sqlite3_result_error_code(int64 ctx, int32 code)
{
    ::sqlite3_result_error_code((sqlite3_context*)ctx, code);
}

int32 SQLite3RuntimeProvider::sqlite3_value_int(int64 v)
{
    return ::sqlite3_value_int((sqlite3_value*) v);
}

int32 SQLite3RuntimeProvider::sqlite3_value_type(int64 v)
{
    return ::sqlite3_value_type((sqlite3_value*) v);
}

int32 SQLite3RuntimeProvider::sqlite3_value_bytes(int64 v)
{
    return ::sqlite3_value_bytes((sqlite3_value*) v);
}

double SQLite3RuntimeProvider::sqlite3_value_double(int64 v)
{
    return ::sqlite3_value_double((sqlite3_value*) v);
}

int64 SQLite3RuntimeProvider::sqlite3_value_int64(int64 v)
{
    return (int64) ::sqlite3_value_int64((sqlite3_value*) v);
}

int64 SQLite3RuntimeProvider::sqlite3_value_text(int64 v)
{
    return (int64) ::sqlite3_value_text((sqlite3_value*) v);
}

int64 SQLite3RuntimeProvider::sqlite3_value_blob(int64 v)
{
    return (int64) ::sqlite3_value_blob((sqlite3_value*) v);
}

int32 SQLite3RuntimeProvider::sqlite3_bind_parameter_count(int64 stmHandle)
{
	return ::sqlite3_bind_parameter_count((sqlite3_stmt*)stmHandle);
}

int64 SQLite3RuntimeProvider::sqlite3_bind_parameter_name(int64 stmHandle, int32 iParam)
{
	return (int64)::sqlite3_bind_parameter_name((sqlite3_stmt*)stmHandle, iParam);
}

int32 SQLite3RuntimeProvider::sqlite3_bind_parameter_index(int64 stmHandle, int64 paramName)
{
	return ::sqlite3_bind_parameter_index((sqlite3_stmt*)stmHandle, (const char*)paramName);
}

int32 SQLite3RuntimeProvider::sqlite3_step(int64 stmHandle)
{
	return ::sqlite3_step((sqlite3_stmt*)stmHandle);
}

int32 SQLite3RuntimeProvider::sqlite3_stmt_busy(int64 stmHandle)
{
	return ::sqlite3_stmt_busy((sqlite3_stmt*)stmHandle);
}

int32 SQLite3RuntimeProvider::sqlite3_stmt_readonly(int64 stmHandle)
{
	return ::sqlite3_stmt_readonly((sqlite3_stmt*)stmHandle);
}

int32 SQLite3RuntimeProvider::sqlite3_column_int(int64 stmHandle, int32 iCol)
{
	return ::sqlite3_column_int((sqlite3_stmt*)stmHandle, iCol);
}

int64 SQLite3RuntimeProvider::sqlite3_column_int64(int64 stmHandle, int32 iCol)
{
	return (int64)::sqlite3_column_int64((sqlite3_stmt*)stmHandle, iCol);
}

int64 SQLite3RuntimeProvider::sqlite3_column_text(int64 stmHandle, int32 iCol)
{
	return (int64)::sqlite3_column_text((sqlite3_stmt*)stmHandle, iCol);
}

int64 SQLite3RuntimeProvider::sqlite3_column_decltype(int64 stmHandle, int32 iCol)
{
	return (int64)::sqlite3_column_decltype((sqlite3_stmt*)stmHandle, iCol);
}

float64 SQLite3RuntimeProvider::sqlite3_column_double(int64 stmHandle, int32 iCol)
{
	return ::sqlite3_column_double((sqlite3_stmt*)stmHandle, iCol);
}

int64 SQLite3RuntimeProvider::sqlite3_column_blob(int64 stmHandle, int32 iCol)
{
	return (int64)::sqlite3_column_blob((sqlite3_stmt*)stmHandle, iCol);
}

int32 SQLite3RuntimeProvider::sqlite3_column_type(int64 stmHandle, int32 iCol)
{
	return ::sqlite3_column_type((sqlite3_stmt*)stmHandle, iCol);
}

int32 SQLite3RuntimeProvider::sqlite3_column_bytes(int64 stmHandle, int32 iCol)
{
	return ::sqlite3_column_bytes((sqlite3_stmt*)stmHandle, iCol);
}

int32 SQLite3RuntimeProvider::sqlite3_column_count(int64 stmHandle)
{
	return ::sqlite3_column_count((sqlite3_stmt*)stmHandle);
}

int32 SQLite3RuntimeProvider::sqlite3_data_count(int64 stmHandle)
{
	return ::sqlite3_data_count((sqlite3_stmt*)stmHandle);
}

int64 SQLite3RuntimeProvider::sqlite3_column_name(int64 stmHandle, int32 iCol)
{
	return (int64)::sqlite3_column_name((sqlite3_stmt*)stmHandle, iCol);
}

int64 SQLite3RuntimeProvider::sqlite3_column_origin_name(int64 stmHandle, int32 iCol)
{
	return (int64)::sqlite3_column_origin_name((sqlite3_stmt*)stmHandle, iCol);
}

int64 SQLite3RuntimeProvider::sqlite3_column_table_name(int64 stmHandle, int32 iCol)
{
	return (int64)::sqlite3_column_table_name((sqlite3_stmt*)stmHandle, iCol);
}

int64 SQLite3RuntimeProvider::sqlite3_column_database_name(int64 stmHandle, int32 iCol)
{
	return (int64)::sqlite3_column_database_name((sqlite3_stmt*)stmHandle, iCol);
}

int32 SQLite3RuntimeProvider::sqlite3_reset(int64 stmHandle)
{
	return ::sqlite3_reset((sqlite3_stmt*)stmHandle);
}

int32 SQLite3RuntimeProvider::sqlite3_clear_bindings(int64 stmHandle)
{
	return ::sqlite3_clear_bindings((sqlite3_stmt*)stmHandle);
}

int32 SQLite3RuntimeProvider::sqlite3_stmt_status(int64 stmHandle, int32 op, int32 resetFlg)
{
	return ::sqlite3_stmt_status((sqlite3_stmt*)stmHandle, op, resetFlg);
}

int32 SQLite3RuntimeProvider::sqlite3_finalize(int64 stmHandle)
{
	return ::sqlite3_finalize((sqlite3_stmt*)stmHandle);
}

int32 SQLite3RuntimeProvider::sqlite3_wal_autocheckpoint(int64 db, int32 n)
{
	return ::sqlite3_wal_autocheckpoint((sqlite3*)db, n);
}

int32 SQLite3RuntimeProvider::sqlite3_wal_checkpoint(int64 db, int64 dbName)
{
	return ::sqlite3_wal_checkpoint((sqlite3*)db, (const char*)dbName);
}

int32 SQLite3RuntimeProvider::sqlite3_wal_checkpoint_v2(int64 db, int64 dbName, int32 eMode, int64 logSize, int64 framesCheckPointed)
{
	int32 pnLog = 0;
	int32 pnCkpt = 0;

	int32 result = ::sqlite3_wal_checkpoint_v2((sqlite3*)db, (const char *)dbName, eMode, &pnLog, &pnCkpt);

	if (logSize)
	{
		int64* p = (int64*)logSize;
		*p = pnLog;
	}

	if (framesCheckPointed)
	{
		int64* p = (int64*)framesCheckPointed;
		*p = pnCkpt;
	}

	return result;
}

int32 SQLite3RuntimeProvider::sqlite3_set_authorizer(int64 db, int64 func, int64 v)
{
	return ::sqlite3_set_authorizer((sqlite3*)db, (int(*)(void*,int,const char*,const char*,const char*,const char*)) func, (void*)v); 
}    
