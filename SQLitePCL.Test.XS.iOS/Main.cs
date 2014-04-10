using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SQLitePCL.Test.XS.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			SQLitePCL.Platform.Instance = new SQLitePCL.SQLite3Provider();
			
				
			// if you want to use a different Application Delegate class from "UnitTestAppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "UnitTestAppDelegate");
		}
	}
}
