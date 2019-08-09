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

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using SQLitePCL;
using SQLitePCL.Ugly;

namespace SQLitePCL.Tests
{

    // TODO mv this to its own assembly
    class my_vfs : sqlite3_vfs
    {
        class my_io : sqlite3_io_methods
        {
            readonly FileStream _f;
            public my_io(FileStream f)
            {
                _f = f;
            }

            int Close(
                )
            {
                System.Console.WriteLine("Close");
                _f.Close();
                return 0;
            }

            int Read(
                Span<byte> buf,
                long iOfst
                )
            {
                System.Console.WriteLine($"Read: off={iOfst} len={buf.Length}");
                var pos = _f.Seek(iOfst, SeekOrigin.Begin);
                // not in netstandard2.0
                var got = _f.Read(buf);
                if (got == buf.Length)
                {
                    return 0;
                }
                else
                {
                    // TODO zero fill
                    return raw.SQLITE_IOERR_SHORT_READ;
                }
            }

            int Write(
                ReadOnlySpan<byte> buf,
                long iOfst
                )
            {
                System.Console.WriteLine($"Write: off={iOfst} len={buf.Length}");
                var pos = _f.Seek(iOfst, SeekOrigin.Begin);
                // not in netstandard2.0
                _f.Write(buf);
                return 0;
            }

            int Truncate(
                long size
                )
            {
                System.Console.WriteLine($"Truncate: {size}");
                return 0;
            }

            int Sync(
                int flags
                )
            {
                System.Console.WriteLine($"Sync: {flags}");
                return 0;
            }

            int FileSize(
                out long size
                )
            {
                System.Console.WriteLine("FileSize");
                var pos = _f.Seek(0, SeekOrigin.End);
                System.Console.WriteLine($"    {pos}");
                size = pos;
                return 0;
            }

            int Lock(
                int x
                )
            {
                System.Console.WriteLine($"Lock: {x}");
                return 0;
            }

            int Unlock(
                int x
                )
            {
                System.Console.WriteLine($"Unlock: {x}");
                return 0;
            }

            int CheckReservedLock(
                out int res
                )
            {
                System.Console.WriteLine("CheckReservedLock");
                res = 0; // TODO
                return 0;
            }

            int FileControl(
                int op,
                IntPtr pArg
                )
            {
                System.Console.WriteLine($"FileControl: op={op}");
                return 0;
            }

            int SectorSize(
                )
            {
                System.Console.WriteLine("SectorSize");
                return 0;
            }

            int DeviceCharacteristics(
                )
            {
                System.Console.WriteLine("DeviceCharacteristics");
                return 0;
            }

            public delegate_io_xClose xClose => Close;
            public delegate_io_xRead xRead => Read;
            public delegate_io_xWrite xWrite => Write;
            public delegate_io_xTruncate xTruncate => Truncate;
            public delegate_io_xSync xSync => Sync;
            public delegate_io_xFileSize xFileSize => FileSize;
            public delegate_io_xLock xLock => Lock;
            public delegate_io_xUnlock xUnlock => Unlock;
            public delegate_io_xCheckReservedLock xCheckReservedLock => CheckReservedLock;
            public delegate_io_xFileControl xFileControl => FileControl;
            public delegate_io_xSectorSize xSectorSize => SectorSize;
            public delegate_io_xDeviceCharacteristics xDeviceCharacteristics => DeviceCharacteristics;
        }

        int Open(
            utf8z name,
            out sqlite3_io_methods io,
            int flags,
            out int out_flags
            )
        {
            System.Console.WriteLine($"Open: {name.utf8_to_string()}");
            FileMode mod;
            FileAccess acc;
            FileStream f;
            if (0 != (flags & raw.SQLITE_OPEN_READONLY))
            {
                mod = FileMode.Open;
                acc = FileAccess.Read;
            }
            else if (0 != (flags & raw.SQLITE_OPEN_READWRITE))
            {
                acc = FileAccess.ReadWrite;
                if (0 != (flags & raw.SQLITE_OPEN_CREATE))
                {
                    mod = FileMode.OpenOrCreate;
                }
                else
                {
                    mod = FileMode.Open;
                }
            }
            else
            {
                System.Console.WriteLine("    Open cannot");
                io = null;
                out_flags = 0;
                return raw.SQLITE_CANTOPEN;
            }
            try
            {
                f = File.Open(name.utf8_to_string(), mod, acc);
            }
            catch
            {
                System.Console.WriteLine("    Open catch");
                io = null;
                out_flags = 0;
                return raw.SQLITE_CANTOPEN;
            }

            io = new my_io(f);
            out_flags = 0;
            System.Console.WriteLine("    Open ok");
            return 0;
        }

        int Delete(
            utf8z name,
            int flags
            )
        {
            System.Console.WriteLine($"Delete: {name.utf8_to_string()}");
            try
            {
                File.Delete(name.utf8_to_string());
                return 0;
            }
            catch
            {
                return 1; // TODO
            }
        }

        int Access(
            utf8z psz_name,
            int flags,
            out int res
            )
        {
            System.Console.WriteLine($"Access: {psz_name.utf8_to_string()}");
            // TODO handle directories as well
            if (flags == raw.SQLITE_ACCESS_EXISTS)
            {
                if (File.Exists(psz_name.utf8_to_string()))
                {
                    System.Console.WriteLine($"    yes");
                    res = 1;
                    return 0;
                }
                else
                {
                    System.Console.WriteLine($"    no");
                    res = 0;
                    return 0;
                }
            }
            else if (flags == raw.SQLITE_ACCESS_READWRITE)
            {
                // TODO this is wrong
                if (File.Exists(psz_name.utf8_to_string()))
                {
                    System.Console.WriteLine($"    yes");
                    res = 1;
                    return 0;
                }
                else
                {
                    System.Console.WriteLine($"    no");
                    res = 0;
                    return 0;
                }
            }
            else
            {
                System.Console.WriteLine($"    wrong");
                res = 0;
                return 0;
            }
        }

        int FullPathname(
            utf8z psz_name,
            Span<byte> sz_res
            )
        {
            System.Console.WriteLine($"FullPathname: {psz_name.utf8_to_string()}");
            // TODO make this a full path
            psz_name.AsSpan().CopyTo(sz_res);
            return 0;
        }

        // TODO need better quality random numbers than this
        readonly Random _r = new Random();
        int Randomness(
            Span<byte> res
            )
        {
            System.Console.WriteLine($"Randomness: {res.Length} bytes");
            // not in netstandard 2.0
            _r.NextBytes(res);
            return res.Length;
        }

        int Sleep(
            int microseconds
            )
        {
            System.Console.WriteLine($"Sleep: {microseconds}");
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(microseconds / 1000));
            return 0;
        }

        int CurrentTime(
            out double res
            )
        {
            // TODO probably don't need this if CurrentTimeInt64 is implemented
            System.Console.WriteLine("CurrentTime");
            res = 0;
            return 1;
        }

        int GetLastError(
            Span<byte> sz_res
            )
        {
            System.Console.WriteLine("GetLastError");
            return 1;
        }

        int CurrentTimeInt64(
            out long res
            )
        {
            System.Console.WriteLine("CurrentTimeInt64");
            res = 0;
            return 1;
        }

        public int maxPathname => 255; // TODO
        public delegate_vfs_xOpen xOpen => Open;
        public delegate_vfs_xDelete xDelete => Delete;
        public delegate_vfs_xAccess xAccess => Access;
        public delegate_vfs_xFullPathname xFullPathname => FullPathname;
        public delegate_vfs_xRandomness xRandomness => Randomness;
        public delegate_vfs_xSleep xSleep => Sleep;
        public delegate_vfs_xCurrentTime xCurrentTime => CurrentTime;
        public delegate_vfs_xGetLastError xGetLastError => GetLastError;
        public delegate_vfs_xCurrentTimeInt64 xCurrentTimeInt64 => CurrentTimeInt64;

    }

}

