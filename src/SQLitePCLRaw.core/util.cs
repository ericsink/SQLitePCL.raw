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
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Collections.Generic;

    public readonly ref struct sz
    {
        readonly ReadOnlySpan<byte> sp;

        public ref readonly byte GetPinnableReference()
        {
            return ref sp.GetPinnableReference();
        }

        sz(ReadOnlySpan<byte> a)
        {
            if (
                (a.Length > 0)
                && (a[a.Length - 1] != 0)
                )
            {
                throw new ArgumentException("zero terminated string required");
            }
            sp = a;
        }

        public static sz FromString(string s)
        {
            if (s == null)
            {
                return new sz(ReadOnlySpan<byte>.Empty);
            }
            else
            {
                return new sz(s.to_utf8_with_z());
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

        unsafe static ReadOnlySpan<byte> to_span(byte* p)
        {
            var len = (int) my_strlen(p);
            return new ReadOnlySpan<byte>(p, len + 1);
        }

        unsafe static ReadOnlySpan<byte> to_span(IntPtr p)
        {
            return to_span((byte*) (p.ToPointer()));
        }

        unsafe public static sz FromPtr(byte* p)
        {
            if (p == null)
            {
                return new sz(ReadOnlySpan<byte>.Empty);
            }
            else
            {
                return new sz(to_span(p));
            }
        }

        unsafe public static sz FromPtr(byte* p, int len)
        {
            if (p == null)
            {
                return new sz(ReadOnlySpan<byte>.Empty);
            }
            else
            {
                var sp = new ReadOnlySpan<byte>(p, len + 1);
                return new sz(sp);
            }
        }

        public static sz FromIntPtr(IntPtr p)
        {
            if (p == IntPtr.Zero)
            {
                return new sz(ReadOnlySpan<byte>.Empty);
            }
            else
            {
                return new sz(to_span(p));
            }
        }

        public override string ToString()
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

    static class util
    {
        public static sz to_sz(this string s)
        {
            return sz.FromString(s);
        }

        public static byte[] to_utf8_with_z(this string sourceText)
        {
            if (sourceText == null)
            {
                return null;
            }

            int nlen = Encoding.UTF8.GetByteCount(sourceText);

            var byteArray = new byte[nlen + 1];
            var wrote = Encoding.UTF8.GetBytes(sourceText, 0, sourceText.Length, byteArray, 0);
            byteArray[wrote] = 0;

            return byteArray;
        }

        static int my_strlen(System.IntPtr nativeString)
        {
            var offset = 0;

            if (nativeString != IntPtr.Zero)
            {
                // TODO would this be faster if it used unsafe code with a pointer?
                while (Marshal.ReadByte(nativeString, offset) > 0)
                {
                    offset++;
                }
            }

            return offset;
        }

        public static string from_utf8_z(IntPtr nativeString)
        {
            return from_utf8(nativeString, my_strlen(nativeString));
        }

        public static string from_utf8(IntPtr nativeString, int size)
        {
            string result = null;

            if (nativeString != IntPtr.Zero)
            {
                unsafe
                {
                    result = Encoding.UTF8.GetString((byte*) nativeString.ToPointer(), size);
                }
            }

            return result;
        }

        public static string from_utf8(ReadOnlySpan<byte> p)
        {
            if (p == null)
            {
                return null;
            }
            if (p.Length == 0)
            {
                // assumes no zero terminator
                return "";
            }
            unsafe
            {
                fixed (byte* q = p)
                {
                    return Encoding.UTF8.GetString(q, p.Length);
                }
            }
        }

        public static string from_utf8_with_z(ReadOnlySpan<byte> p)
        {
            if (p == null)
            {
                return null;
            }
            if (p.Length == 0)
            {
                throw new Exception("span is zero length but was supposed to have a zero terminator");
            }
            if (p[p.Length - 1] != 0)
            {
                throw new Exception("span was supposed to have a zero terminator but last byte is not zero");
            }
            unsafe
            {
                fixed (byte* q = p)
                {
                    return Encoding.UTF8.GetString(q, p.Length - 1);
                }
            }
        }
    }

}

