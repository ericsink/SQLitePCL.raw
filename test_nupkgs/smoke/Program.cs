using System;
using SQLitePCL;
using SQLitePCL.Ugly;

namespace smoke
{
    class Program
    {
        class MyGetFunctionPointer : IGetFunctionPointer
        {
            readonly IntPtr _dll;
            public MyGetFunctionPointer(IntPtr dll)
            {
                _dll = dll;
            }

            public IntPtr GetFunctionPointer(string name)
            {
                if (NativeLibrary.TryGetExport(_dll, name, out var f))
                {
                    //System.Console.WriteLine("{0}.{1} : {2}", _dll, name, f);
                    return f;
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
        }
        static void DoDynamic(string name)
        {
		    var dll = SQLitePCL.NativeLibrary.Load(name);
            var gf = new MyGetFunctionPointer(dll);
            SQLitePCL.SQLite3Provider_Cdecl.Setup(gf);
            SQLitePCL.raw.SetProvider(new SQLite3Provider_Cdecl());
        }
        static void Main(string[] args)
        {
            DoDynamic("e_sqlite3.dll");
			using (var db = ugly.open(":memory:"))
			{
				var s = db.query_scalar<string>("SELECT sqlite_version()");	
				Console.WriteLine("{0}", s);
			}
        }
    }
}
