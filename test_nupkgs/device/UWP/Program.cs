using System;

// This example code shows how you could implement the required main function for a 
// Console UWP Application. You can replace all the code inside Main with your own custom code.

// You should also change the Alias value in the AppExecutionAlias Extension in the 
// Package.appxmanifest to a value that you define. To edit this file manually, right-click
// it in Solution Explorer and select View Code, or open it with the XML Editor.

namespace App19
{
    class Program
    {
        static int Main()
        {
            SQLitePCL.Batteries_V2.Init();
            SQLitePCL.raw.sqlite3_win32_set_directory(/*data directory type*/1, Windows.Storage.ApplicationData.Current.LocalFolder.Path);
            SQLitePCL.raw.sqlite3_win32_set_directory(/*temp directory type*/2, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path);
            var rc = Xunit.Run.AllTestsInCurrentAssembly();
            Console.ReadLine();
            return rc;
        }
    }
}
