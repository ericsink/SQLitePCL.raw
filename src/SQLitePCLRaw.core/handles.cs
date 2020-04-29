/*
   Copyright 2014-2019 SourceGear, LLC

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
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices;

    public class sqlite3_backup : SafeHandle
    {
        sqlite3_backup() : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            int rc = raw.internal_sqlite3_backup_finish(handle);
            // TODO check rc?
            return true;
        }

        public int manual_close()
        {
            int rc = raw.internal_sqlite3_backup_finish(handle);
            // TODO review.  should handle always be nulled here?
            // TODO maybe called SetHandleAsInvalid instead?
            handle = IntPtr.Zero;
            return rc;
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
        internal IntPtr ptr => _p;

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

        internal IntPtr ptr => _p;
    }

    public class sqlite3_blob : SafeHandle
    {
        sqlite3_blob() : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            int rc = raw.internal_sqlite3_blob_close(handle);
            // TODO check rc?
            return true;
        }

        public int manual_close()
        {
            int rc = raw.internal_sqlite3_blob_close(handle);
            // TODO review.  should handle always be nulled here?
            // TODO maybe called SetHandleAsInvalid instead?
            handle = IntPtr.Zero;
            return rc;
        }
    }

    public class sqlite3_stmt : SafeHandle
    {
        private sqlite3 _db;

        internal static sqlite3_stmt From(IntPtr p, sqlite3 db)
        {
            var h = new sqlite3_stmt();
            h.SetHandle(p);
            db.add_stmt(h);
            h._db = db;
            return h;
        }

        sqlite3_stmt() : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            int rc = raw.internal_sqlite3_finalize(handle);
            // TODO check rc?
            _db.remove_stmt(this);
            return true;
        }

        public int manual_close()
        {
            int rc = raw.internal_sqlite3_finalize(handle);
            // TODO review.  should handle always be nulled here?
            // TODO maybe called SetHandleAsInvalid instead?
            handle = IntPtr.Zero;
            _db.remove_stmt(this);
            return rc;
        }

        // TODO rm?  used by the next_stmt code.
        internal IntPtr ptr => handle;

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

    public class sqlite3 : SafeHandle
    {
        sqlite3() : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            int rc = raw.internal_sqlite3_close_v2(handle);
            // TODO check rc?
            dispose_extra();
            return true;
        }

        public int manual_close_v2()
        {
            int rc = raw.internal_sqlite3_close_v2(handle);
            // TODO review.  should handle always be nulled here?
            // TODO maybe called SetHandleAsInvalid instead?
            handle = IntPtr.Zero;
            dispose_extra();
            return rc;
        }

        public int manual_close()
        {
            int rc = raw.internal_sqlite3_close(handle);
            // TODO review.  should handle always be nulled here?
            // TODO maybe called SetHandleAsInvalid instead?
            handle = IntPtr.Zero;
            dispose_extra();
            return rc;
        }

        internal static sqlite3 New(IntPtr p)
        {
            var h = new sqlite3();
            h.SetHandle(p);
#if not // changing this to default OFF for v2
            h.enable_sqlite3_next_stmt(true);
#endif
            return h;
        }

        // this dictionary is used only for the purpose of supporting sqlite3_next_stmt.
        private ConcurrentDictionary<IntPtr, sqlite3_stmt> _stmts = null;

        public void enable_sqlite3_next_stmt(bool enabled)
        {
            if (enabled)
            {
                if (_stmts == null)
                {
                    _stmts = new ConcurrentDictionary<IntPtr, sqlite3_stmt>();
                }
            }
            else
            {
                _stmts = null;
            }
        }

        internal void add_stmt(sqlite3_stmt stmt)
        {
            if (_stmts != null)
            {
                _stmts[stmt.ptr] = stmt;
            }
        }

        internal sqlite3_stmt find_stmt(IntPtr p)
        {
            if (_stmts != null)
            {
                return _stmts[p];
            }
            else
            {
                // any change to the wording of this error message might break a test case
                throw new Exception("The sqlite3_next_stmt() function is disabled.  To enable it, call sqlite3.enable_sqlite3_next_stmt(true) immediately after opening the sqlite3 connection.");
            }
        }

        internal void remove_stmt(sqlite3_stmt s)
        {
            if (_stmts != null)
            {
                _stmts.TryRemove(s.ptr, out var stmt);
            }
        }

        IDisposable extra;

        public T GetOrCreateExtra<T>(Func<T> f)
            where T : class, IDisposable
        {
            if (extra != null)
            {
                return (T)extra;
            }
            else
            {
                var q = f();
                extra = q;
                return q;
            }
        }

        private void dispose_extra()
        {
            if (extra != null)
            {
                extra.Dispose();
                extra = null;
            }
        }

    }
}

