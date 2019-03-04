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

    public static class util
    {
        public static byte[] to_utf8(this string sourceText)
        {
            if (sourceText == null)
            {
                return null;
            }

            byte[] byteArray;
            int nlen = Encoding.UTF8.GetByteCount(sourceText) + 1;

            byteArray = new byte[nlen];
            nlen = Encoding.UTF8.GetBytes(sourceText, 0, sourceText.Length, byteArray, 0);
            byteArray[nlen] = 0;

            return byteArray;
        }

        private static int GetNativeUTF8Size(System.IntPtr nativeString)
        {
            var offset = 0;

            if (nativeString != IntPtr.Zero)
            {
                while (Marshal.ReadByte(nativeString, offset) > 0)
                {
                    offset++;
                }

                offset++;
            }

            return offset;
        }

        public static string from_utf8(IntPtr nativeString)
        {
            string result = null;

            if (nativeString != IntPtr.Zero)
            {
                int size = GetNativeUTF8Size(nativeString);
                var array = new byte[size - 1];
                Marshal.Copy(nativeString, array, 0, size - 1);
                result = Encoding.UTF8.GetString(array, 0, array.Length);
            }

            return result;
        }

        public static string from_utf8(IntPtr nativeString, int size)
        {
            string result = null;

            if (nativeString != IntPtr.Zero)
            {
                var array = new byte[size];
                Marshal.Copy(nativeString, array, 0, size);
                result = Encoding.UTF8.GetString(array, 0, array.Length);
            }

            return result;
        }
    }

}

