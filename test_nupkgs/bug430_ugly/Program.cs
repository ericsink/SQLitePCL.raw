
using System;
using System.Text;
using SQLitePCL;
using SQLitePCL.Ugly;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        private static bool IsBusy(int rc)
            => rc == raw.SQLITE_LOCKED
                || rc == raw.SQLITE_BUSY
                || rc == raw.SQLITE_LOCKED_SHAREDCACHE;

        static bool _prepared = false;
        static List<sqlite3_stmt> _preparedStatements = new();

        private static void DisposePreparedStatements()
        {
            if (_preparedStatements != null)
            {
                foreach (var stmt in _preparedStatements)
                {
                    stmt.Dispose();
                }

                _preparedStatements.Clear();
            }

            _prepared = false;
        }

        static IEnumerable<sqlite3_stmt> enumerate(sqlite3 db, string commandText)
        {
			DisposePreparedStatements();

            var byteCount = Encoding.UTF8.GetByteCount(commandText);
            var sql = new byte[byteCount + 1];
            Encoding.UTF8.GetBytes(commandText, 0, commandText.Length, sql, 0);
            int rc;
            sqlite3_stmt stmt;
            var start = 0;
            do
            {
                ReadOnlySpan<byte> tail;
                while (IsBusy(rc = raw.sqlite3_prepare_v2(db, sql.AsSpan(start), out stmt, out tail)))
                {
                    System.Threading.Thread.Sleep(150);
                }
                start = sql.Length - tail.Length;

                if (rc != 0)
                {
                    throw new Exception();
                }

                if (stmt.IsInvalid)
                {
                    if (start < byteCount)
                    {
                        continue;
                    }

                    break;
                }

                _preparedStatements.Add(stmt);

                yield return stmt;
            }
            while (start < byteCount);

            _prepared = true;
        }

        static void Main(string[] args)
        {
            SQLitePCL.Batteries.Init();

            using (var db = ugly.open("tracker.db"))
            {
                while (true)
                {
                    db.exec("BEGIN TRANSACTION");
                    try
                    {
                        var commandText = "SELECT Id, Url, Parent, IsDirectory, IdentifierTag, ContentTag FROM FileTracker WHERE Url=?1;";
                        foreach (var stmt in _prepared ? _preparedStatements : enumerate(db, commandText))
                        {
                            stmt.clear_bindings();
                            var i = stmt.bind_parameter_index("?1");
                            stmt.bind_text(i, "file://local/");
                            stmt.step();

                            var result = HandleReaderSingleDataRow(stmt);

                            stmt.reset();

                            Console.WriteLine(result?.Id + "");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e}");
                    }
                    finally
                    {
                        db.exec("ROLLBACK");
                    }
                }
            }
        }

        private static DataRow HandleReaderSingleDataRow(sqlite3_stmt reader)
        {
            return new DataRow(
                reader.column_int64(0),
                reader.column_text(1),
                reader.column_int64(2) as long?,
                reader.column_int(3) != 0,
                reader.column_text(4),
                reader.column_text(5)
                );
        }

        public record DataRow(long Id, string Url, long? Parent, bool IsDirectory, string IdentifierTag,
            string ContentTag);
    }
}


