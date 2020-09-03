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

// This code is just a set of things that are used in the implementation
// of providers.  It doesn't really need to be in core, since nothing
// in core actually depends on it.  But having it be separate isn't
// worth the trouble.

namespace SQLitePCL
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Collections.Generic;

    public sealed class PreserveAttribute : System.Attribute
    {
        public bool AllMembers;
        public bool Conditional;
    }

    public sealed class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute(Type t) { }
    }

    public class SafeGCHandle : SafeHandle
    {
        public SafeGCHandle(object v, GCHandleType typ)
            : base(IntPtr.Zero, true)
        {
            if (v != null)
            {
                var h = GCHandle.Alloc(v, typ);
                SetHandle(GCHandle.ToIntPtr(h));
            }
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            var h = GCHandle.FromIntPtr(handle);
            h.Free();
            return true;
        }

    }

    public class hook_handle : SafeGCHandle
    {
        public hook_handle(object target)
            : base(target, GCHandleType.Normal)
        {
        }

        public IDisposable ForDispose()
        {
            if (IsInvalid)
            {
                return null;
            }
            else
            {
                return this;
            }
        }
    }

    class CompareBuf : System.Collections.Generic.EqualityComparer<byte[]>
    {
        Func<IntPtr, IntPtr, int, bool> _f;
        public CompareBuf(Func<IntPtr, IntPtr, int, bool> f)
        {
            _f = f;
        }
        public override bool Equals(byte[] p1, byte[] p2)
        {
            if (p1.Length != p2.Length)
            {
                return false;
            }
            var h1 = GCHandle.Alloc(p1, GCHandleType.Pinned);
            var h2 = GCHandle.Alloc(p2, GCHandleType.Pinned);
            var result = _f(h1.AddrOfPinnedObject(), h2.AddrOfPinnedObject(), p1.Length);
            h1.Free();
            h2.Free();
            return result;
        }

        public override int GetHashCode(byte[] p)
        {
            return p.Length; // TODO do better
        }
    }

    class FuncName
    {
        public byte[] name { get; private set; }
        public int n { get; private set; }

        public FuncName(byte[] _name, int _n)
        {
            name = _name;
            n = _n;
        }
    }

    class CompareFuncName : System.Collections.Generic.EqualityComparer<FuncName>
    {
        System.Collections.Generic.IEqualityComparer<byte[]> _ptrlencmp;
        public CompareFuncName(System.Collections.Generic.IEqualityComparer<byte[]> ptrlencmp)
        {
            _ptrlencmp = ptrlencmp;
        }
        public override bool Equals(FuncName p1, FuncName p2)
        {
            if (p1.n != p2.n)
            {
                return false;
            }
            return _ptrlencmp.Equals(p1.name, p2.name);
        }

        public override int GetHashCode(FuncName p)
        {
            return p.n + p.name.Length; // TODO do better
        }
    }

    public class hook_handles : IDisposable
    {
        public hook_handles(Func<IntPtr, IntPtr, int, bool> f)
        {
            var cmp = new CompareBuf(f);
            collation = new ConcurrentDictionary<byte[], IDisposable>(cmp);
            scalar = new ConcurrentDictionary<FuncName, IDisposable>(new CompareFuncName(cmp));
            agg = new ConcurrentDictionary<FuncName, IDisposable>(new CompareFuncName(cmp));
        }

        readonly ConcurrentDictionary<byte[], IDisposable> collation;
        readonly ConcurrentDictionary<FuncName, IDisposable> scalar;
        readonly ConcurrentDictionary<FuncName, IDisposable> agg;
        public IDisposable update;
        public IDisposable rollback;
        public IDisposable commit;
        public IDisposable trace;
        public IDisposable profile;
        public IDisposable progress;
        public IDisposable authorizer;

        public bool RemoveScalarFunction(byte[] name, int nargs)
        {
            var k = new FuncName(name, nargs);
            if (scalar.TryRemove(k, out var h_old))
            {
                h_old.Dispose();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddScalarFunction(byte[] name, int nargs, IDisposable d)
        {
            var k = new FuncName(name, nargs);
            scalar[k] = d;
        }

        public bool RemoveAggFunction(byte[] name, int nargs)
        {
            var k = new FuncName(name, nargs);
            if (agg.TryRemove(k, out var h_old))
            {
                h_old.Dispose();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddAggFunction(byte[] name, int nargs, IDisposable d)
        {
            var k = new FuncName(name, nargs);
            agg[k] = d;
        }

        public bool RemoveCollation(byte[] name)
        {
            if (collation.TryRemove(name, out var h_old))
            {
                h_old.Dispose();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddCollation(byte[] name, IDisposable d)
        {
            collation[name] = d;
        }

        public void Dispose()
        {
            foreach (var h in collation.Values) h.Dispose();
            foreach (var h in scalar.Values) h.Dispose();
            foreach (var h in agg.Values) h.Dispose();
            if (update != null) update.Dispose();
            if (rollback != null) rollback.Dispose();
            if (commit != null) commit.Dispose();
            if (trace != null) trace.Dispose();
            if (profile != null) profile.Dispose();
            if (progress != null) progress.Dispose();
            if (authorizer != null) authorizer.Dispose();
        }
    }

    public class log_hook_info
    {
        private delegate_log _func;
        private object _user_data;

        public log_hook_info(delegate_log func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public static log_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            log_hook_info hi = h.Target as log_hook_info;
            return hi;
        }

        public void call(int rc, utf8z msg)
        {
            _func(_user_data, rc, msg);
        }
    }

    public class commit_hook_info
    {
        public delegate_commit _func { get; private set; }
        public object _user_data { get; private set; }

        public commit_hook_info(delegate_commit func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public int call()
        {
            return _func(_user_data);
        }

        public static commit_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            commit_hook_info hi = h.Target as commit_hook_info;
            return hi;
        }
    }

    public class rollback_hook_info
    {
        private delegate_rollback _func;
        private object _user_data;

        public rollback_hook_info(delegate_rollback func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public static rollback_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            rollback_hook_info hi = h.Target as rollback_hook_info;
            // TODO assert(hi._h == h)
            return hi;
        }

        public void call()
        {
            _func(_user_data);
        }
    }

    public class trace_hook_info
    {
        private delegate_trace _func;
        private object _user_data;

        public trace_hook_info(delegate_trace func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public static trace_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            trace_hook_info hi = h.Target as trace_hook_info;
            return hi;
        }

        public void call(utf8z s)
        {
            _func(_user_data, s);
        }
    }

    public class profile_hook_info
    {
        private delegate_profile _func;
        private object _user_data;

        public profile_hook_info(delegate_profile func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public static profile_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            profile_hook_info hi = h.Target as profile_hook_info;
            return hi;
        }

        public void call(utf8z s, long elapsed)
        {
            _func(_user_data, s, elapsed);
        }
    }

    public class progress_hook_info
    {
        private delegate_progress _func;
        private object _user_data;

        public progress_hook_info(delegate_progress func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public static progress_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            progress_hook_info hi = h.Target as progress_hook_info;
            return hi;
        }

        public int call()
        {
            return _func(_user_data);
        }
    }

    public class update_hook_info
    {
        private delegate_update _func;
        private object _user_data;

        public update_hook_info(delegate_update func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public static update_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            update_hook_info hi = h.Target as update_hook_info;
            return hi;
        }

        public void call(int typ, utf8z db, utf8z tbl, long rowid)
        {
            _func(_user_data, typ, db, tbl, rowid);
        }
    }

    public class collation_hook_info
    {
        private delegate_collation _func;
        private object _user_data;

        public collation_hook_info(delegate_collation func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public static collation_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            collation_hook_info hi = h.Target as collation_hook_info;
            return hi;
        }

        public int call(ReadOnlySpan<byte> s1, ReadOnlySpan<byte> s2)
        {
            return _func(_user_data, s1, s2);
        }
    }

    public class exec_hook_info
    {
        private delegate_exec _func;
        private object _user_data;

        public exec_hook_info(delegate_exec func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public static exec_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            exec_hook_info hi = h.Target as exec_hook_info;
            return hi;
        }

        public int call(int n, IntPtr values_ptr, IntPtr names_ptr)
        {
            var values = new IntPtr[n];
            var names = new IntPtr[n];
            // TODO warning on the following line.  SizeOf(Type) replaced in .NET 4.5.1 with SizeOf<T>()
            int ptr_size = Marshal.SizeOf(typeof(IntPtr));
            for (int i = 0; i < n; i++)
            {
                IntPtr vp;

                vp = Marshal.ReadIntPtr(values_ptr, i * ptr_size);
                values[i] = vp;

                vp = Marshal.ReadIntPtr(names_ptr, i * ptr_size);
                names[i] = vp;
            }

            return _func(_user_data, values, names);
        }
    }

    public class function_hook_info
    {
        private delegate_function_scalar _func_scalar;
        private delegate_function_aggregate_step _func_step;
        private delegate_function_aggregate_final _func_final;
        private object _user_data;

        private class agg_sqlite3_context : sqlite3_context
        {
            public agg_sqlite3_context(object v) : base(v)
            {
            }

            public void fix_ptr(IntPtr p)
            {
                set_context_ptr(p);
            }
        }

        public function_hook_info(
            delegate_function_scalar func_scalar,
            object user_data
            )
        {
            _func_scalar = func_scalar;
            _user_data = user_data;
        }

        public function_hook_info(
            delegate_function_aggregate_step func_step,
            delegate_function_aggregate_final func_final,
            object user_data
            )
        {
            _func_step = func_step;
            _func_final = func_final;
            _user_data = user_data;
        }

        public static function_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            function_hook_info hi = h.Target as function_hook_info;
            return hi;
        }

        private sqlite3_context get_context(IntPtr context, IntPtr agg_context)
        {
            // agg_context is a pointer to 8 bytes of storage, obtained from
            // sqlite3_aggregate_context().  we will use it to store the
            // sqlite3_context object so we can use the same object for all
            // calls to xStep and xFinal for this instance/invocation of the
            // aggregate function.

            agg_sqlite3_context ctx;
            IntPtr c = Marshal.ReadIntPtr(agg_context);
            if (c == IntPtr.Zero)
            {
                // this is the first call to xStep or xFinal.  we need a new
                // sqlite3_context object to pass to the user's callback.
                ctx = new agg_sqlite3_context(_user_data);

                // and store a handle in the agg_context storage area so we
                // can get this back next time.
                GCHandle h = GCHandle.Alloc(ctx);
                Marshal.WriteIntPtr(agg_context, (IntPtr)h);
            }
            else
            {
                // we've been through here before.  retrieve the sqlite3_context
                // object from the agg_context storage area.
                GCHandle h = (GCHandle)c;
                ctx = h.Target as agg_sqlite3_context;
            }

            // we are reusing the same sqlite3_context object for each call
            // to xStep/xFinal within the same instance/invocation of the
            // user's agg function.  but SQLite actually gives us a different
            // context pointer on each call.  so we need to fix it up.
            ctx.fix_ptr(context);

            return ctx;
        }

        private class scalar_sqlite3_context : sqlite3_context
        {
            public scalar_sqlite3_context(IntPtr p, object v) : base(v)
            {
                set_context_ptr(p);
            }
        }

        public void call_scalar(IntPtr context, int num_args, IntPtr argsptr)
        {
            scalar_sqlite3_context ctx = new scalar_sqlite3_context(context, _user_data);

            sqlite3_value[] a = new sqlite3_value[num_args];
            // TODO warning on the following line.  SizeOf(Type) replaced in .NET 4.5.1 with SizeOf<T>()
            int ptr_size = Marshal.SizeOf(typeof(IntPtr));
            for (int i = 0; i < num_args; i++)
            {
                IntPtr vp = Marshal.ReadIntPtr(argsptr, i * ptr_size);
                a[i] = new sqlite3_value(vp);
            }

            _func_scalar(ctx, _user_data, a);
        }

        public void call_step(IntPtr context, IntPtr agg_context, int num_args, IntPtr argsptr)
        {
            sqlite3_context ctx = get_context(context, agg_context);

            sqlite3_value[] a = new sqlite3_value[num_args];
            // TODO warning on the following line.  SizeOf(Type) replaced in .NET 4.5.1 with SizeOf<T>()
            int ptr_size = Marshal.SizeOf(typeof(IntPtr));
            for (int i = 0; i < num_args; i++)
            {
                IntPtr vp = Marshal.ReadIntPtr(argsptr, i * ptr_size);
                a[i] = new sqlite3_value(vp);
            }

            _func_step(ctx, _user_data, a);
        }

        public void call_final(IntPtr context, IntPtr agg_context)
        {
            sqlite3_context ctx = get_context(context, agg_context);

            _func_final(ctx, _user_data);

            IntPtr c = Marshal.ReadIntPtr(agg_context);
            GCHandle h = (GCHandle)c;
            h.Free();
        }

    }

    public class authorizer_hook_info
    {
        private delegate_authorizer _func;
        private object _user_data;

        public authorizer_hook_info(delegate_authorizer func, object v)
        {
            _func = func;
            _user_data = v;
        }

        public static authorizer_hook_info from_ptr(IntPtr p)
        {
            GCHandle h = (GCHandle)p;
            authorizer_hook_info hi = h.Target as authorizer_hook_info;
            return hi;
        }

        public int call(int action_code, utf8z param0, utf8z param1, utf8z dbName, utf8z inner_most_trigger_or_view)
        {
            return _func(_user_data, action_code, param0, param1, dbName, inner_most_trigger_or_view);
        }
    }

}


