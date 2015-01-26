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

    // typed wrapper for an IntPtr.  still opaque.
    public class sqlite3_backup : IDisposable
    {
        private bool _disposed = false;
        private IntPtr _p;

        internal sqlite3_backup(IntPtr p)
        {
            _p = p;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool both)
        {
            if (_disposed)
            {
                return;
            }

            raw.sqlite3_backup_finish(this);
            _disposed = true;
        }

        internal void done()
        {
            _p = IntPtr.Zero;
        }

        internal IntPtr ptr
        {
            get
            {
                return _p;
            }
        }
    }

    // typed wrapper for an IntPtr.  still opaque.  the upper layers can't
    // do anything with this except hand it back to us on calls to
    // raw.sqlite3_result_*.
    //
    // except for the 'state' property below, which the upper layers can 
    // use to store state between calls to
    // xStep/xFinal for an aggregate function.
    public class sqlite3_context
    {
        private IntPtr _p;
        private object _user_data;

        // must be called by one of the two subclass (scalar
        // or agg)
        protected sqlite3_context(object user_data)
        {
            _user_data = user_data;
        }

        // used by raw.sqlite3_user_data (which is internal
        // to the PCL assembly)
        internal object user_data
        {
            get
            {
                return _user_data;
            }
        }

        // used by raw.sqlite3_result_* (which is internal to the
        // PCL assembly) to fetch the actual context pointer to pass 
        // back to sqlite.
        internal IntPtr ptr
        {
            get
            {
                return _p;
            }
        }

        // used by either the scalar or agg subclass, located
        // in util.cs, compiled into the platform assembly.  each
        // call to xFunc, xStep, or xFinal actually gives us a
        // different context pointer.  however, for an aggregate
        // function, we want this sqlite3_context object to be the
        // same throughout all the calls to xStep or xFinal.  so
        // we fix the pointer on each call.  and we want this to be
        // invisible to the upper layers, so make this protected and do 
        // the fixup in a subclass.
        protected void set_context_ptr(IntPtr p)
        {
            _p = p;
        }

        // this is available to the upper layers, to store state during 
        // the run of an aggregate function.  not needed for scalar
        // functions.
        public object state;
    }

    // typed wrapper for an IntPtr.  still opaque.  the upper layers can't
    // do anything with this except hand it back to us on calls to
    // raw.sqlite3_value_*
    public class sqlite3_value
    {
        private IntPtr _p;

        public sqlite3_value(IntPtr p)
        {
            _p = p;
        }

        internal IntPtr ptr
        {
            get
            {
                return _p;
            }
        }
    }

    // typed wrapper for an IntPtr.  still opaque.
    public class sqlite3_blob : IDisposable
    {
        private bool _disposed = false;
        private IntPtr _p;

        internal sqlite3_blob(IntPtr p)
        {
            _p = p;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool both)
        {
            if (_disposed)
            {
                return;
            }

            raw.sqlite3_blob_close(this);
            _disposed = true;
        }

        internal void done()
        {
            _p = IntPtr.Zero;
        }

        internal IntPtr ptr
        {
            get
            {
                return _p;
            }
        }
    }

    // typed wrapper for an IntPtr.  still opaque.
    public class sqlite3_stmt : IDisposable
    {
        private bool _disposed = false;
        private IntPtr _p;
        private sqlite3 _db;

        internal sqlite3_stmt(IntPtr p, sqlite3 db)
        {
            _p = p;
            _db = db;
            _db.add_stmt(this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool both)
        {
            if (_disposed)
            {
                return;
            }

            raw.sqlite3_finalize(this);
            _disposed = true;
        }

        internal void done()
        {
            _db.remove_stmt(this);
            _p = IntPtr.Zero;
            _db = null;
        }

        internal IntPtr ptr
        {
            get
            {
                return _p;
            }
        }

        // We keep track of the db connection handle for this stmt, even though
        // the underlying sqlite C library keeps track of it as well.  On a call
        // to sqlite3_db_handle(), if we called the C function and get a pointer
        // and then wrap it in a new instance of our sqlite3 class, we would end
        // up with two instances of that class having the same wrapped IntPtr.
        // This seems bad.  So we implement it here at this layer as well.
        
        internal sqlite3 db
        {
            get
            {
                return _db;
            }
        }
    }

    // typed wrapper for an IntPtr.  still opaque.
    public class sqlite3 : IDisposable
    {
        private bool _disposed = false;
        private IntPtr _p;
        private Dictionary<IntPtr, sqlite3_stmt> _stmts = new Dictionary<IntPtr, sqlite3_stmt>();

        internal sqlite3(IntPtr p)
        {
            _p = p;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool both)
        {
            if (_disposed)
            {
                return;
            }

            // We intentionally use sqlite3_close() here instead of sqlite3_close_v2().
            // The latter is not supported on the sqlite3 library which is preinstalled
            // with iOS.
            
            // Note, however, that sqlite3_close() can fail.  And we are ignoring the
            // return code, because the only thing we could do with it is to throw,
            // which is somewhat forbidden from within Dispose().
            //
            // http://msdn.microsoft.com/en-us/library/bb386039.aspx

            raw.sqlite3_close(this);
            _disposed = true;
        }

        internal void done()
        {
            _p = IntPtr.Zero;
        }

        internal IntPtr ptr
        {
            get
            {
                return _p;
            }
        }

        internal void add_stmt(sqlite3_stmt stmt)
        {
            _stmts[stmt.ptr] = stmt;
        }

        internal sqlite3_stmt find_stmt(IntPtr p)
        {
            return _stmts[p];
        }

        internal void remove_stmt(sqlite3_stmt s)
        {
            _stmts.Remove(s.ptr);
        }
    }

}

