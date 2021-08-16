
using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection =
                new SqliteConnection(
                    @"Data Source=tracker.db"))
            {
                connection.Open();

                while (true)
                {
                    using (connection.BeginTransaction())
                    {
                        var queryCommand = connection.CreateCommand();
                        queryCommand.CommandText = $@"
                        SELECT Id, Url, Parent, IsDirectory, IdentifierTag, ContentTag FROM FileTracker WHERE Url=?1;";
                        queryCommand.Parameters.Add(new SqliteParameter("?1", "file://local/"));

                        var reader = queryCommand.ExecuteReader();

                        var result = HandleReaderSingleDataRow(reader);

                        Console.WriteLine(result?.Id + "");
                    }
                }
            }
        }

        private static DataRow HandleReaderSingleDataRow(IDataReader reader)
        {
            if (!reader.Read())
            {
                return null;
            }

            return new DataRow(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetValue(2) as long?,
                reader.GetBoolean(3),
                reader.GetValue(4) as string,
                reader.GetValue(5) as string);
        }

        public record DataRow(long Id, string Url, long? Parent, bool IsDirectory, string IdentifierTag,
            string ContentTag);
    }
}


