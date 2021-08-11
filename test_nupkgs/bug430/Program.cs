
using System;
using System.Text;
using SQLitePCL;
using SQLitePCL.Ugly;

namespace ConsoleApp1
{
    class Program
    {
        public static byte[] to_utf8(string s)
        {
            var ba = new byte[Encoding.UTF8.GetByteCount(s)];
            Encoding.UTF8.GetBytes(s, 0, s.Length, ba, 0);
            return ba;
        }

        static void Main(string[] args)
        {
            SQLitePCL.Batteries.Init();

            using (var db = ugly.open("tracker.db"))
            {
                var ba = to_utf8("SELECT Id, Url, Parent, IsDirectory, IdentifierTag, ContentTag FROM FileTracker WHERE Url=?1;");
                while (true)
                {
                    db.exec("BEGIN TRANSACTION");
                    ReadOnlySpan<byte> sql = ba.AsSpan();
                    var rc = raw.sqlite3_prepare_v2(db, sql, out var stmt, out var tail);
                    // TODO check rc
                    stmt.bind(1, "file://local/");
                    stmt.step();

                    var result = HandleReaderSingleDataRow(stmt);

                    Console.WriteLine(result?.Id + "");
                    db.exec("ROLLBACK");
                }
            }
        }

        private static DataRow HandleReaderSingleDataRow(sqlite3_stmt reader)
        {
            return new DataRow(
                reader.column_int64(0),
                null,
                reader.column_int64(2) as long?,
                reader.column_int(3) != 0,
                null,
                null);
        }

        public record DataRow(long Id, string Url, long? Parent, bool IsDirectory, string IdentifierTag,
            string ContentTag);
    }
}


