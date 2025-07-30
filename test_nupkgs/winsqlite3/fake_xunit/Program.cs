
using System;

public static class foo
{
    public static int Main()
    {
        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());
        return Xunit.Run.AllTestsInCurrentAssembly();
    }
}

