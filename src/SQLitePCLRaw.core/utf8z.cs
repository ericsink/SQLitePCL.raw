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

namespace SQLitePCL
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Collections.Generic;

    // TODO consider Length property, which returns sp.Length - 1, but what about null
    // TODO consider way to get span, not included the zero terminator, but what about null
    public readonly ref struct utf8z
    {
        // this span will contain a zero terminator byte
        // if sp.Length is 0, it represents a null string
        // if sp.Length is 1, the only byte must be zero, and it is an empty string
        readonly ReadOnlySpan<byte> sp;

        public ref readonly byte GetPinnableReference()
        {
            return ref sp.GetPinnableReference();
        }

        utf8z(ReadOnlySpan<byte> a)
        {
            // no check here.  anything that calls this
            // constructor must make assurances about the
            // zero terminator.
            sp = a;
        }

        // TODO we could consider making this public
        static utf8z FromSpan(ReadOnlySpan<byte> a)
        {
            if (
                (a.Length > 0)
                && (a[a.Length - 1] != 0)
                )
            {
                throw new ArgumentException("zero terminator required");
            }
            return new utf8z(a);
        }

        public static utf8z FromString(string s)
        {
            if (s == null)
            {
                return new utf8z(ReadOnlySpan<byte>.Empty);
            }
            else
            {
                return new utf8z(s.to_utf8_with_z());
            }
        }

        unsafe static long my_strlen(byte* p)
        {
            var q = p;
            while (*q != 0)
            {
                q++;
            }
            return q - p;
        }

        unsafe static ReadOnlySpan<byte> find_zero_terminator(byte* p)
        {
            var len = (int)my_strlen(p);
            return new ReadOnlySpan<byte>(p, len + 1);
        }

        unsafe public static utf8z FromPtr(byte* p)
        {
            if (p == null)
            {
                return new utf8z(ReadOnlySpan<byte>.Empty);
            }
            else
            {
                return new utf8z(find_zero_terminator(p));
            }
        }

        // TODO maybe remove this and just use FromSpan?
        unsafe public static utf8z FromPtrLen(byte* p, int len)
        {
            if (p == null)
            {
                return new utf8z(ReadOnlySpan<byte>.Empty);
            }
            else
            {
                // the given len does NOT include the zero terminator
                var sp = new ReadOnlySpan<byte>(p, len + 1);
                return FromSpan(sp);
            }
        }

        unsafe public static utf8z FromIntPtr(IntPtr p)
        {
            if (p == IntPtr.Zero)
            {
                return new utf8z(ReadOnlySpan<byte>.Empty);
            }
            else
            {
                return new utf8z(find_zero_terminator((byte*) (p.ToPointer())));
            }
        }

        public string utf8_to_string()
        {
            if (sp.Length == 0)
            {
                return null;
            }

            unsafe
            {
                fixed (byte* q = sp)
                {
                    return Encoding.UTF8.GetString(q, sp.Length - 1);
                }
            }
        }
    }

}

