using System;
using SQLitePCL.Ugly;

namespace smoke
{
    class Program
    {
        static void Main(string[] args)
        {
			var lib = SQLitePCL.Setup.Load("e_sqlite3", s => Console.WriteLine("{0}", s));
			using (var db = ugly.open(":memory:"))
			{
				var s = db.query_scalar<string>("SELECT sqlite_version()");	
				Console.WriteLine("{0}", s);
			}
        }
    }
}
