
using System;

public static class foo
{
    public static void Main(string[] args)
    {
        SQLitePCL.Batteries_V2.Init();
        Xunit.Run.AllTestsInCurrentAssembly();
    }
}

