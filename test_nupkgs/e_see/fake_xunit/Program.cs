
using System;
using SQLitePCL;
using SQLitePCL.Ugly;

public static class foo
{
    public static int Main()
    {
        SQLitePCL.Batteries_V2.Init();
        using (sqlite3 db = ugly.open(":memory:"))
        {
            var see_activation_code = System.Environment.GetEnvironmentVariable("SEE_ACTIVATION_CODE");
            db.exec($"PRAGMA activate_extensions='see-{see_activation_code}';");
        }
        return Xunit.Run.AllTestsInCurrentAssembly();
    }
}

