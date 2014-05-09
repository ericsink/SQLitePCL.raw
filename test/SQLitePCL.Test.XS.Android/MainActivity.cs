using System.Reflection;
using Android.App;
using Android.OS;
using Xamarin.Android.NUnitLite;

namespace SQLitePCL.Test.XS.Android
{
	[Activity (Label = "SQLitePCL.Test.XS.Android", MainLauncher = true)]
	public class MainActivity : TestSuiteActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			SQLitePCL.Platform.Instance = new SQLitePCL.SQLite3Provider();
			
			// tests can be inside the main assembly
			AddTest (Assembly.GetExecutingAssembly ());
			// or in any reference assemblies
			// AddTest (typeof (Your.Library.TestClass).Assembly);

			// Once you called base.OnCreate(), you cannot add more assemblies.
			base.OnCreate (bundle);
		}
	}
}

