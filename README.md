
# SQLitePCL.raw

SQLitePCL.raw is a Portable Class Library (PCL) for low-level (raw)
access to SQLite. License:  Apache License v2.

# What's new in 0.9

Before 0.9, the main nuget package contained a bunch of different copies of the
native SQLite library.  The original intent here was to make this nuget
package Just Work for all the common use cases.  But over time, as more
platforms and options got added, the package was getting enormous.

In 0.9, the native code has been moved to separate packages, and the
main package is much smaller.

If you are using the system-provided SQLite library on iOS or Android (see
below for caveats on Android N), or
if you are using the SQLite vsix on Windows, the transition to 0.9 should be
seamless.

In other cases, you will need to add one of the SQLitePCL.plugin packages,
and perhaps one of the SQLitePCL.native packages, and add a line of code to initialize 
things:

    SQLite3Plugin.Init();

The SQLitePCL.plugin packages are platform-specific, not PCLs.  Add the
plugin to either your app project or to a platform-specific library.
The call to SQLite3Plugin.Init() should go somewhere in your app initialization
code.

For iOS and Android, the plugin packages are self-contained.  These plugins
are all you need on that platform.

For other platforms, the plugin is "empty", in the sense that you also need
to add a SQLitePCL.native package to specify exactly which version of the
native SQLite code you want to use.

For example, if you are building a desktop app, you might add

    SQLitePCL.plugin.sqlite3.net45

The "net45" in the name of this package
means that the contents were compiled for .NET 4.5.  But this doesn't
say anything about the native code.  This particularly package can be used
on various Windows platforms or even with Mono on Linux or Mac.  So you
also need to add a SQLitePCL.native package, such as this one:

    SQLitePCL.native.sqlite3.v110_xp

This contain the SQLite library compiled with the Visual C++ v110\_xp toolset.
If you are on a desktop version of Windows and you're not sure which native
SQLite library to use, this is the one you should try first.

The plugins with sqlite3 in the name are for vanilla SQLite.  The ones with
sqlcipher in the name are the SQLCipher variant.  BTW, another difference with
0.9 is that I am no longer building SQLCipher.  The SQLCipher packages simply
contain the SQLCipher builds maintained by Couchbase.

The set of packages I publish does not include every possible combination
of platform and toolset and so on.  If you need one that I have not built,
let me know and I'll see what makes sense.  One example is that I do not
have a package called SQLitePCL.native.sqlite3.linux, because my assumption
is that most people will use the SQLite provided by the distro.

The UseSQLiteFrom feature is no longer supported in 0.9.  If you were
using this, you will need to switch to specifying things by which
plugin/native package you include.

# CONTENT BELOW THIS LINE IS NOT YET UPDATED FOR 0.9

## QuickStart

Add the SQLitePCL.raw NuGet package to your platform projects (iOS, Android, Windows, etc) and to your PCL projects as needed. 

On some platforms, you can select the version of SQLite to use by adding an MSBuild property like the following to your project file:

    <UseSQLiteFrom>packaged_sqlite3</UseSQLiteFrom>

or

    <UseSQLiteFrom>packaged_sqlcipher</UseSQLiteFrom>

If UseSQLiteFrom is missing, SQLitePCL.raw will use the SQLite library included by the device OS, if available (iOS and Android have this, for example). 

Note that on some platforms the SQLitePCL.raw NuGet package does not include an assembly in the NuGet "lib" directory, but rather, uses an msbuild 
"targets" file, which, at build time, adds a reference depending on the UseSQLiteFrom 
property you set in your project file.

## Portable Class Library (PCL)

A Portable Class Library is not merely a class library which happens to avoid using
things that are not portable.  The PCL concept also includes tooling support.
You can tell the compiler which .NET targets you want to support (a profile) 
and then it will complain at you if you try to use something that wouldn't
work.

####Which flavor of PCL is this?

See [my blog entry](http://www.ericsink.com/entries/pcl_bait_and_switch.html) about the two different ways of doing PCLs.

SQLitePCL.raw uses the Bait and Switch approach.  It provides a portable assembly (the Bait)
which can be referenced by projects that are libraries.  When building an executable or app,
reference the appropriate platform-specific alternative.

#### Which platform assemblies are provided in this library?

In the pcl subdirectory, each csproj with a name of the form SQLitePCL.platform.\* contains an implementation
of a platform assembly.

The following platforms are currently supported:

 * .NET 4.5 on Windows or Mac OS X

 * .NET 4.0 on Windows or Mac OS X

 * Windows Phone 8

 * Windows Phone 8.1, both SL and RT flavors

 * WinRT

 * Xamarin.iOS, both Classic and Unified

 * Xamarin.Android

Some of these platforms have more than one implementation of the platform assembly,
for reasons explained later in this README.

## Is this available in NuGet?

Yes.  The package ID is SQLitePCL.raw

## There's another NuGet package called SQLitePCL.raw\_basic.  What is it??

That's the old package ID.  With the 0.8.0 release, I transitioned to
just 'SQLitePCL.raw' as the package ID (without the \_basic suffix).
Eventually I may stop publishing updates to the old ID.  For now, the
package gets published under both IDs and they are identical.

## Is this available in the Xamarin Component Store?

No.

## What is the state of this code?  Is it completely done?

(I'm not sure it'll ever be completely done, but...) 

The code itself is in pretty good shape.
There are some more tests I want to write.  There are some platform configurations
I still want to support.

Look in todo.txt for an informal working list of tasks pending.  This file is not
always kept terribly current.  :-)

## How do I build this?

For all practical purposes, it's impossible.  :-)

gen\_build.cs is a C# script which generates a solution with all the project configurations in the bld folder, along with other files for the build system.

On my build machine, I have Visual Studio 2012 *and* Visual Studio 2013 (update 2) *and* Visual Studio 2015 and Xamarin Android + iOS and  
the relevant Android SDK(s).

build\_mac.sh builds the iOS libraries and has to be run on the mac.
The resulting assemblies are actually committed to the repository so the main build
process can use them.  Run apple/libs/mac/cp\_mac.ps1 to copy the
necessary files over to the bld directory.

Builds for sqlite and sqlcipher for Mac, iOS, and Android also happen
on a Mac.  See the sh files in the apple directory.  For Android,
run ndk-build in android/sqlite3.

## Versions

You can check the version of SQLite being used by your app with this code:

    //Display the version of SQLite used
    //using SQLitePCL;
    sqlite3 db;
    raw.sqlite3_open(":memory:", out db);
    sqlite3_stmt stmt;
    raw.sqlite3_prepare_v2(db, "select sqlite_version()", out stmt);
    raw.sqlite3_step(stmt);
    raw.sqlite3_column_type(stmt, 0);
    var version = raw.sqlite3_column_text(stmt, 0);
    Console.WriteLine("SQLITE version " + version);
    raw.sqlite3_finalize(stmt);
    stmt.Dispose();
    db.Dispose();

For information:  
iOS 9.2 is shipped with sqlite 3.8.10.2  
iOS 8.4 is shipped with sqlite 3.8.5  

## Which PCL profile is supported?

The build system contains projects to build the PCL with profile 78, 
profile 136, profile 158, and profile 259.  Others might work as well, but those are the ones I 
have tried.

The test suites are configured to use the profile 78 version in most places, but
profile 259 for Windows Phone 8.1.

The main reason to keep profile 158 is because MvvmCross is using it.  If they
end up switching to the new reflection APIs and profile 259, then 158 won't be
very important here either.  Folks are saying that profile 259 is the new 78.

## Can this library be used to write a mobile app?

Technically, yes, but that's not what you want to do.
This is *not* the sort of SQLite library you would use to write an app.
It is a *very* thin C# wrapper around the C API for SQLite.  It's "raw".

Consequently, as much as
possible, this library follows the stylistic conventions of SQLite, not those of
the .NET/C# world.

For example, the C function for opening a SQLite file
is sqlite3\_open(), so this API provides a method called sqlite3\_open(),
not Sqlite3Open().  

Similarly, the functions in this API return integer error codes
rather than throwing .NET exceptions, because that's how the SQLite C API
works.

As a library for app developers, this library is downright hostile.
It feels like using C.  Intentionally.

## So if this library is so unfriendly, why does it exist at all?

This library is designed to be the common portable layer upon which friendlier
wrappers can be built.  Right now, every C# SQLite library writes their
own P/Invoke and COM and marshaling and stuff.
Build on this library instead and focus more on the upper layer and its
goal of providing a pleasant, easy-to-use API for app developers.

## How does this compare to sqlite-net?

[sqlite-net](https://github.com/praeclarum/sqlite-net) is a very popular SQLite wrapper by Frank Krueger (@praeclarum).
Unlike SQLitePCL.raw, it is designed to make writing apps easier.  It even includes a lightweight ORM,
and some basic support for LINQ.  

SQLitePCL.raw wants to replace the bottom half of sqlite-net so that it
can become a PCL.

In fact, that has happened.  Frank Krueger has released a NuGet package
(sqlite-net-pcl) which is SQLite-net with SQLitePCL.raw underneath:

[https://www.nuget.org/packages/sqlite-net-pcl/](https://www.nuget.org/packages/sqlite-net-pcl/)

When people ask me to recommend a friendlier SQLite wrapper, sqlite-net is the
one that I usually recommend.

## How does this compare to SQLitePCL.pretty?
[SQLitePCL.pretty](https://github.com/bordoley/SQLitePCL.pretty) is another friendly SQLite API wrapper.
It is built on top of SQLitePCL.raw, and its name resembles my SQLitePCL.Ugly wrapper (described below), but I am not
the developer, and I'm afraid I have no actual experience using it.  SQLitePCL.pretty is developed by @bordoley, who has also been a contributor of several fine pull requests to SQLitePCL.raw itself.

@bordoley's own description of SQLitePCL.pretty:

"It is designed to make interacting with the SQLite API easier, exposing the full feature
set of SQLite in an idiomatic and *pretty* C# API.

Interesting features include the ability to iterate through query result sets using LINQ, support for binary
streaming of data in and out of SQLite using .NET streams, and a powerful async API built on the RX framework."

## How does this compare to the SQLite stuff in MvvmCross?

Same story.  MvvmCross is using a PCL-ified fork of sqlite-net.
I would welcome MvvmCross to use my version of sqlite-net built on SQLitePCL.raw.

## How does this compare to SQLite.Net-PCL?

[SQLite.Net-PCL](https://github.com/oysteinkrog/SQLite.Net-PCL) appears to be a PCL fork of sqlite-net,
apparently with a bit more proactive stance toward eliminating ifdefs.  
I don't know much else about it.

## How does this compare to System.Data.SQLite?

[System.Data.SQLite](http://system.data.sqlite.org) is an ADO.NET-style SQLite wrapper developed by the
core SQLite team.  It is very full-featured, supporting LINQ and Entity Framework.  And for obvious reasons, 
it does a fantastic job of the SQLite side of things.  But it is not at all mobile-friendly.

## How does this compare to Mono.Data.Sqlite?

Mono.Data.Sqlite is an ADO.NET-style SQLite wrapper which is built into Mono and the Xamarin
platform.  It shares a common ancestry with System.Data.SQLite, as both began as forks from
the same code.

## How does this compare to Microsoft.Data.Sqlite?

[Microsoft.Data.Sqlite](https://github.com/aspnet/Microsoft.Data.Sqlite) is an 
ADO.NET-style SQLite wrapper which is part of the ASP.NET 5 / EF7 effort at Microsoft.

## How does this compare to SQLitePCL?

[SQLitePCL](https://sqlitepcl.codeplex.com/) is a SQLite Portable Class Library released on Codeplex by MS Open Tech.

This library is a fork of that code.  Sort of.

It is a fork in the 2007 sense of the word.  I made significant use of the code.  I preserved copyright notices.

However, this is not the the sort of fork which is created for the purpose of
producing a pull request.  The changes I've made are so extensive that I do not
plan to submit a pull request unless one is requested.  I plan to maintain this
code going forward.

## So how is SQLitePCL.raw different from what you started with?

Since beginning with the SQLitePCL code, I've made a bunch of improvements to
the build and its supported platforms:

 * Added support for more PCL profiles

 * Added build support for Xamarin.iOS

 * Added build support for Xamarin.Android

 * Added support for Windows Phone 8.1, RT and SL

 * Updated WinRT to support both P/Invoke and C++ Interop

 * Updated Net45 to support both P/Invoke and C++ Interop

 * Added a new set of automated test projects, sharing the same test code for MS Test and NUnit

 * Simplified the platform injection interfaces from 3 down to 1

 * Reorganized the tree to eliminate code duplication

 * Lots of bug fixes

I've also implemented nearly the entire SQLite API, including the following highlights which were not present in the original code:

 * Have SQLite call a .NET delegate when a transaction ends (sqlite3\_commit\_hook, sqlite3\_rollback\_hook)

 * Give SQLite a .NET delegate to be called for diagnostic purposes (sqlite3\_trace, sqlite3\_profile)

 * Write custom functions and collations in managed code (sqlite3\_create\_function, sqlite3\_create\_collation)

 * Access SQLite blobs using the streaming API, to avoid the requirement of reading the entire blob into memory (sqlite3\_blob\_etc)

 * Obtain the SQLite extended result codes and error messages

 * Use the SQLite backup API (sqlite3\_backup\_etc)

 * Ask the SQLite library for information about itself (sqlite3\_compileoption_*, sqlite3\_libversion, etc)

Bottom line, this library is a robust low-level wrapper which stays very true to the SQLite library itself, while presenting a portable API to the layer above.

## So what is the architecture of this library?

Even within this thin library, there are three layers to be explained:

At the very bottom is the SQLite library itself.  Written in C.  This is 
unmanaged code, compiled for x86 or ARM or whatever.

Building on that, SQLitePCL.raw has three layers.

(1) We need something that can call the C code.  For the Xamarin
implementations this is DllImport (aka P/Invoke).  For Windows Phone 8, this
is a C++ wrapper.  For the others, it is both.  In either case, this presents an extremely low-level API.
C pointers become System.IntPtr.  Strings are not actually .NET strings, but rather, are in utf-8
encoding, represented in C# as either an IntPtr or a byte[].

(2) Next layer up, we have a
C# layer which makes things just a tiny bit friendlier.  Not much.
It maps strings to/from utf8.  And it manages delegates.  The
implementation of this layer varies depending on which assembly
it is.  For the PCL assembly itself (the Bait assembly), this is
implemented as a set of stubs that throw errors.

(3) Finally, we have one more layer called "raw".  
This layer
is the top layer of the PCL, the one that is presented publicly.
It is identical in all of the assemblies, portable or not.
It adds one more C# nicety, which is that all IntPtrs are packaged up
inside typed wrapper classes.  For example, at the level of the C API,
a database connection is represented by a sqlite3\*.  One layer up,
inside the C# code, this becomes an IntPtr, and it remains an IntPtr
at each layer until the top one (raw) which instead uses an instance of
the sqlite3 class, which does nothing much except contain an IntPtr.
In other words, it adds nothing except type checking.

For example, consider the C function sqlite3\_prepare\_v2().  In C, this
function looks like this:

    int sqlite3_prepare_v2(
      sqlite3* db,
      const char* pzSql,
      int nByte,
      sqlite3_stmt** ppStmt,
      const char** pzTail
      );

One layer up, it becomes:

    [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_prepare_v2(
      IntPtr db, 
      byte[] pSql, 
      int nBytes, 
      out IntPtr stmt, 
      out IntPtr ptrRemain
      );

Both the sqlite3 and the sqlite3\_stmt pointers became IntPtr, thus losing their type info and our ability to distinguish them by type.
The string argument became a byte[], because it's utf8.  The pzTail argument for returning a string becomes an IntPtr, but is also
a utf8 C string.

One layer up, in the interface, the function looks like this:

    int sqlite3_prepare_v2(
      IntPtr db, 
      string sql, 
      out IntPtr stmt, 
      out string remain
      );

The utf8 stuff is gone, and we've got strings.  But IntPtr is still there.

Finally, in the raw API, this function is:

    static public int sqlite3_prepare_v2(
      sqlite3 db, 
      string sql, 
      out sqlite3_stmt stmt, 
      out string tail
      );

The sqlite3 and sqlite3\_stmt classes are those typed wrappers for IntPtrs that I mentioned.  They have the exact same names as their counterparts in the SQLite C code.

## Is there anything else in those IntPtr classes?

Actually, yes, glad you asked.  Several of them support IDisposable as well.
And there is also a little bit of plumbing to make sure that each pointer from
the C layer lives inside only one instance of its corresponding IntPtr class.

## So those IntPtr classes don't have any methods on them?

Nope.

## What is SQLitePCL.Ugly?

Well, it's a bunch of extension methods for the IntPtr classes.
It's like a fourth layer which provides method call syntax.
It also switches the error handling model from integer return
codes to exception throwing.

For example, the sqlite3\_stmt class represents a statement
handle, but you still have to do things like this:

    int rc;

    sqlite3 db;
    rc = raw.sqlite3_open(":memory:", out db);
    if (rc != raw.SQLITE_OK)
    {
        error
    }
    sqlite3_stmt stmt;
    rc = raw.sqlite3_prepare(db, "CREATE TABLE foo (x int)", out stmt);
    if (rc != raw.SQLITE_OK)
    {
        error
    }
    rc = raw.sqlite3_step(stmt);
    if (rc == raw.SQLITE_DONE)
    {
        whatever
    }
    else
    {
        error
    }
    raw.sqlite3_finalize(stmt);

The Ugly layer allows me to do things like this:

    using (sqlite3 db = ugly.open(":memory:"))
    {
        sqlite3_stmt stmt = db.prepare("CREATE TABLE foo (x int)");
        stmt.step();
    }

This exception-throwing wrapper exists so that I can have something
easier against which to write tests.  It retains all the "lower-case
and underscores" ugliness of the layer(s) below.  
It does not do things "The C# Way".
As such, this is not
a wrapper intended for public consumption.  

## But why did you have to make the Ugly layer, er, ugly?

I am very familiar with the underlying SQLite C API.  I just wanted
to write tests against something similar to it.

## Wait a minute -- you're not a true camelCase believer, are you?

Guilty.  I actually kinda like the old "lower-case and underscores"
convention from my Unix days.

Also, sometimes when I am driving alone in my truck, I listen to country music.

## What are the SQLitePCL.Test.\* projects?

The test suite is a shared code file located in src/test/test\_cases.cs.
Each platform has a test project.  For .NET, Windows Phone, and WinRT,
I use the unit testing features of Visual Studio (MS Test).  For iOS and Android,
I use the unit testing features built into Xamarin Studio (NUnit).

## Why do some tests fail on iOS and/or Android?

Because the version of SQLite preinstalled on the device or emulator 
is too old.

For example, the function sqlite3\_close\_v2() was added in SQLite version
3.7.14.  As of Android KitKat, all versions of Android have shipped with
SQLite 3.7.11 or older.

In practice, these issues are commonly handled by avoiding the use of new-ish
SQLite functions.  Alternatively, you can bundle a recent version of SQLite
into your mobile app rather than using the build that is preinstalled on the
platform.  (See below for how to get SQLitePCL.raw to do this for you.)

## Why do some of the platforms have multiple platform assemblies?

Because there are multiple options available in how the native SQLite code
gets included in an app.

First of all, there are two ways to call native code from C#:  

  1.  P/Invoke

  2.  C++ Interop

Xamarin platforms only support P/Invoke.  The Silverlight flavors of 
Windows Phone 8 only support C++ Interop.
The other platforms support both.

P/Invoke requires a platform-native shared library (such as a .dll or .so).

The C++ Interop builds include the SQLite bits directly, resulting in a mixed mode assembly.

On iOS and Android, a sqlite3 library is included as part of the OS.  Most people just
use it.  

Some people want/need to include a SQLite version bundled as part of their app.
Reasons to do this include:

  1.  They want something more recent than the version provided with iOS/Android.

  2.  They're on WinRT or something else that doesn't bundle SQLite at all.

  3.  They need a custom SQLite, built with compile options customized for their situation.  A common case here is wanting to use the FTS (full text search) feature of SQLite.

  4.  They are substituting SQLCipher as their implementation of SQLite.

And then, consider the fact that for a single processor (Intel, ARM, etc) the
native SQLite code can be either 32 bit or 64 bit,
and the distinction is important.

This library wants to support all of these use cases and hide all the details behind
a PCL.

## But why?  Why are you making this so complicated?

Hey, don't blame me.  I'm not making this complicated.  I'm just trying to
support all the valid use cases.

And more importantly, it is *critical* for the app to get this right.  Here's why:

If you have
two instances of the SQLite library linked into your app, you can corrupt a SQLite
database file.  

That's bad.  And it's kind of an easy mistake to make, especially on mobile platforms
where the OS provides SQLite preinstalled.

## Whoa.  I don't want my SQLite files corrupted.  How do I make sure that won't happen?

On iOS/Android, you have two choices:

  1.  Only use the SQLite provided by the OS.  Make sure your that no part of your app bundles another copy of the SQLite library.

  2.  Understand all the linkage issues and be very careful.

On other platforms, make sure you are including exactly one instance of the SQLite library.

## How can I use SQLitePCL.raw with a recent version of the sqlite3 lib on iOS/Android?

Don't even ask this question until you read the previous three questions above
and understand the risks.

## OK, I understand the risks.  How do I do this?

As of SQLitePCL.raw 0.8.0, you can use a bundled/recent version of sqlite3
by setting an MSBuild property in your project file.  The name of the
property is "UseSQLiteFrom", and if you set this property value to
"packaged\_sqlite3" then SQLitePCL.raw will choose an assembly with a
bundled copy of the native sqlite3 library instead of referencing the
one built-in to iOS/Android.

    <UseSQLiteFrom>packaged\_sqlite3</UseSQLiteFrom>

This feature does not work for iOS Classic (but iOS Unified, the new 64-bit stuff
you should be using anyway, works fine).

## For those native sqlite3 libraries built-in to SQLitePCL.raw: How were they compiled?

    SQLITE_DEFAULT_FOREIGN_KEYS=1
    SQLITE_ENABLE_FTS4
    SQLITE_ENABLE_FTS3_PARENTHESIS
    SQLITE_ENABLE_COLUMN_METADATA

## Is SQLitePCL.raw compatible with SQLCipher?

    <UseSQLiteFrom>packaged\_sqlcipher</UseSQLiteFrom>

This only works on iOS Unified and Android.

## What is the difference between the nuget package "SQLitePCL.raw\_basic" and "SQLitePCL.raw" ?

They are identical.

I am transitioning the name of the package to just "SQLitePCL.raw" removing the \_basic suffix.
Eventually I may deprecate the old name.

## On WinRT-ish platforms, why do I get "Unable to load DLL 'sqlite3': The specified module could not be found." ?

When using any of the RT flavored forms of Windows (Windows Store, Metro, WP81, etc) you must add a
reference to the Visual C++ runtime extension SDK.

## Why do I get "Unhandled managed exception: Could not load file or assembly 'SQLitePCL.raw' or one of its dependencies."

Are you using Microsoft.Bcl.Build?  Its .targets file does some black magic
which has caused incompatibilities with other packages, including
SQLitePCL.raw.

I'm looking for a fix.  For a disappointing short-term workaround, you can just
manually add a reference to the proper SQLitePCL.raw.dll within your packages
directory.  The incompatibility only affects the ability for the .targets file
to add the reference.


