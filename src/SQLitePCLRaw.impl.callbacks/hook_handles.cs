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

	class CompareBuf : System.Collections.Generic.EqualityComparer<byte[]>
	{
        Func<IntPtr,IntPtr,int,bool> _f;
        public CompareBuf(Func<IntPtr,IntPtr,int,bool> f)
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
        public hook_handles(Func<IntPtr,IntPtr,int,bool> f)
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
		public IDisposable trace_v2;
		public IDisposable progress;
		public IDisposable profile;
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

