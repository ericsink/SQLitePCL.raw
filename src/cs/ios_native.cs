/*
   Copyright 2014-2016 Zumero, LLC

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

#if IOS_PACKAGED_SQLITE3

#if PLATFORM_UNIFIED

[assembly: ObjCRuntime.LinkWith(
        "packaged_sqlite3.a",
        LinkTarget = ObjCRuntime.LinkTarget.Simulator | ObjCRuntime.LinkTarget.Simulator64 | ObjCRuntime.LinkTarget.ArmV7 | ObjCRuntime.LinkTarget.ArmV7s | ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]
#else
[assembly: MonoTouch.ObjCRuntime.LinkWith(
        "packaged_sqlite3.a",
        LinkTarget = MonoTouch.ObjCRuntime.LinkTarget.Simulator | MonoTouch.ObjCRuntime.LinkTarget.Simulator64 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7s | MonoTouch.ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]
#endif

#endif

#if IOS_PACKAGED_SQLCIPHER

#if PLATFORM_UNIFIED
[assembly: ObjCRuntime.LinkWith(
        "packaged_sqlcipher.a",
        LinkTarget = ObjCRuntime.LinkTarget.Simulator | ObjCRuntime.LinkTarget.Simulator64 | ObjCRuntime.LinkTarget.ArmV7 | ObjCRuntime.LinkTarget.ArmV7s | ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]

[assembly: ObjCRuntime.LinkWith(
        "libcrypto.a",
        LinkTarget = ObjCRuntime.LinkTarget.Simulator | ObjCRuntime.LinkTarget.Simulator64 | ObjCRuntime.LinkTarget.ArmV7 | ObjCRuntime.LinkTarget.ArmV7s | ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]
#else
[assembly: MonoTouch.ObjCRuntime.LinkWith(
        "packaged_sqlcipher.a",
        LinkTarget = MonoTouch.ObjCRuntime.LinkTarget.Simulator | MonoTouch.ObjCRuntime.LinkTarget.Simulator64 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7s | MonoTouch.ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]

[assembly: MonoTouch.ObjCRuntime.LinkWith(
        "libcrypto.a",
        LinkTarget = MonoTouch.ObjCRuntime.LinkTarget.Simulator | MonoTouch.ObjCRuntime.LinkTarget.Simulator64 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7 | MonoTouch.ObjCRuntime.LinkTarget.ArmV7s | MonoTouch.ObjCRuntime.LinkTarget.Arm64,
        ForceLoad=true,
        LinkerFlags="",
        Frameworks=""
        )
        ]
#endif

#endif

	public class esqlite3
{
	public static void Init()
	{
	}
}

