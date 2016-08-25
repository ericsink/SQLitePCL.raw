using System.Reflection;
using Xunit.Runners.UI;
namespace Tests.UWP10
{
    sealed partial class App : RunnerApplication
    {
        protected override void OnInitializeRunner()
        {
            SQLitePCL.Batteries.Init();
            // tests can be inside the main assembly
            AddTestAssembly(GetType().GetTypeInfo().Assembly);
            // otherwise you need to ensure that the test assemblies will 
            // become part of the app bundle
            AddTestAssembly(typeof(SQLitePCL.Tests.Init).GetTypeInfo().Assembly);
        }
    }
}
