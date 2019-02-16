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
}

