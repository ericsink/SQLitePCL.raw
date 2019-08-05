using System;
using SQLitePCL.Ugly;

namespace smoke
{
    class Program
    {
        static void Main(string[] args)
        {
			SQLitePCL.Batteries_V2.Init();
			using (var db = ugly.open(":memory:"))
			{
				var s = db.query_scalar<string>("SELECT sqlite_version()");	
				Console.WriteLine("{0}", s);
			}
        }
    }
}
