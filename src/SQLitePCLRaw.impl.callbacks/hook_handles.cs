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

	public class hook_handles : IDisposable
	{
		// TODO note that sqlite function names can be case-insensitive.  but we're using
		// a dictionary with a string key to keep track of them.  this has the potential
		// to cause problems.  fixing it with a case-insensitive string comparer is not
		// correct here, since the .NET notion of case-insensitivity is different (more
		// complete) than SQLite's notion.

		public ConcurrentDictionary<string, IDisposable> collation = new ConcurrentDictionary<string, IDisposable>();
		public ConcurrentDictionary<string, IDisposable> scalar = new ConcurrentDictionary<string, IDisposable>();
		public ConcurrentDictionary<string, IDisposable> agg = new ConcurrentDictionary<string, IDisposable>();
		public IDisposable update;
		public IDisposable rollback;
		public IDisposable commit;
		public IDisposable trace;
		public IDisposable progress;
		public IDisposable profile;
		public IDisposable authorizer;

		public void Dispose()
		{
			foreach (var h in collation.Values) h.Dispose();
			foreach (var h in scalar.Values) h.Dispose();
			foreach (var h in agg.Values) h.Dispose();
			if (update!=null) update.Dispose();
			if (rollback!=null) rollback.Dispose();
			if (commit!=null) commit.Dispose();
			if (trace!=null) trace.Dispose();
			if (progress!=null) progress.Dispose();
			if (profile!=null) profile.Dispose();
			if (authorizer!=null) authorizer.Dispose();
		}
	}
}

