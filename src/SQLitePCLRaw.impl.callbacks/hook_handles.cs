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

    class PtrLen
    {
        public IntPtr p { get; private set; }
        public int len { get; private set; }

        static int GetLength(IntPtr p)
        {
            int i = 0;
            while (Marshal.ReadByte(p, i) > 0)
            {
                i++;
            }
            return i;
        }

        public PtrLen(IntPtr _p)
        {
            p = _p;
            len = GetLength(p);
        }
    }

	class ComparePtrLen : System.Collections.Generic.EqualityComparer<PtrLen>
	{
        Func<IntPtr,IntPtr,int,bool> _f;
        public ComparePtrLen(Func<IntPtr,IntPtr,int,bool> f)
        {
            _f = f;
        }
		public override bool Equals(PtrLen p1, PtrLen p2)
		{
            if (p1.len != p2.len)
            {
                return false;
            }
            return _f(p1.p, p2.p, p1.len);
		}

		public override int GetHashCode(PtrLen p)
		{
            return p.len; // TODO do better
		}
	}

    class FuncName
    {
        public PtrLen name { get; private set; }
        public int n { get; private set; }

        public FuncName(IntPtr _p, int _n)
        {
            name = new PtrLen(_p);
            n = _n;
        }
    }

	class CompareFuncName : System.Collections.Generic.EqualityComparer<FuncName>
	{
        System.Collections.Generic.IEqualityComparer<PtrLen> _ptrlencmp;
        public CompareFuncName(System.Collections.Generic.IEqualityComparer<PtrLen> ptrlencmp)
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
            return p.n + p.name.len; // TODO do better
		}
	}

	public class hook_handles : IDisposable
	{
        public hook_handles(Func<IntPtr,IntPtr,int,bool> f)
        {
            var cmp = new ComparePtrLen(f);
            collation = new ConcurrentDictionary<PtrLen, IDisposable>(cmp);
            scalar = new ConcurrentDictionary<FuncName, IDisposable>(new CompareFuncName(cmp));
            agg = new ConcurrentDictionary<FuncName, IDisposable>(new CompareFuncName(cmp));
        }

		readonly ConcurrentDictionary<PtrLen, IDisposable> collation;
		readonly ConcurrentDictionary<FuncName, IDisposable> scalar;
		readonly ConcurrentDictionary<FuncName, IDisposable> agg;
		public IDisposable update;
		public IDisposable rollback;
		public IDisposable commit;
		public IDisposable trace;
		public IDisposable trace_v2;
		public IDisposable progress;
		public IDisposable profile;
		public IDisposable authorizer;

        public bool RemoveScalarFunction(IntPtr name, int nargs)
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

        public void AddScalarFunction(IntPtr name, int nargs, IDisposable d)
        {
            // TODO need private copy of the utf8 string, and therefore need to free it as well
            var k = new FuncName(name, nargs);
            scalar[k] = d;
        }

        public bool RemoveAggFunction(IntPtr name, int nargs)
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

        public void AddAggFunction(IntPtr name, int nargs, IDisposable d)
        {
            // TODO need private copy of the utf8 string, and therefore need to free it as well
            var k = new FuncName(name, nargs);
            agg[k] = d;
        }

        public bool RemoveCollation(IntPtr name)
        {
            var k = new PtrLen(name);
            if (collation.TryRemove(k, out var h_old))
            {
                h_old.Dispose();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddCollation(IntPtr name, IDisposable d)
        {
            // TODO need private copy of the utf8 string, and therefore need to free it as well
            var k = new PtrLen(name);
            collation[k] = d;
        }

		public void Dispose()
		{
			foreach (var h in collation.Values) h.Dispose();
			foreach (var h in scalar.Values) h.Dispose();
			foreach (var h in agg.Values) h.Dispose();
			if (update!=null) update.Dispose();
			if (rollback!=null) rollback.Dispose();
			if (commit!=null) commit.Dispose();
			if (trace!=null) trace.Dispose();
			if (trace_v2!=null) trace_v2.Dispose();
			if (progress!=null) progress.Dispose();
			if (profile!=null) profile.Dispose();
			if (authorizer!=null) authorizer.Dispose();
		}
	}
}

