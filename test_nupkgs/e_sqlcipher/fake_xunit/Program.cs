
using System;

public static class foo
{
    public static int Main()
    {
        SQLitePCL.Batteries_V2.Init();
        return Xunit.Run.AllTestsInCurrentAssembly();
    }
}

