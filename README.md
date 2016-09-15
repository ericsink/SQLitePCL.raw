
# SQLitePCL.raw

SQLitePCL.raw is a Portable Class Library (PCL) for low-level (raw)
access to SQLite. License:  Apache License v2.

# TLDR

Add this package:

    SQLitePCLRaw.bundle\_e\_sqlite3 
    
And call this function:

    SQLitePCL.Batteries.Init()

# Compatibility

SQLitePCL.raw should work just about anywhere you want
it to, including:

- Xamarin.Android
- Xamarin.iOS
- UWP
- Windows Phone 8.1
- .NET 4.5
- .NET 4.0
- .NET 3.5
- Linux
- MacOS
- NetStandard 1.1
- Windows Phone 8 (with limitations)
- Windows Phone 8.1 Silverlight (with limitations)

(I have received a pull request for WatchOS support, but it's not merged yet.)

These packages should be fully compatible with NuGet 2.x or 3.x, either 
packages.config or project.json.

# New package names for release 1.0

With the release of version 1.0, all the nuget package ids
are different.  The are all prefixed with "SQLitePCLRaw.".

The main package is SQLitePCLRaw.core, previously called SQLitePCL.raw

# Old packages ids

Before 1.0, the package ids were

- SQLitePCL.raw
- SQLitePCL.raw\_basic
- SQLitePCL.ugly
- SQLitePCL.plugin.\*
- SQLitePCL.native.\*
- SQLitePCL.bundle\*

All of these packages are being deprecated.

Note:  Once a package (like sqlite-net-pcl, for example) has migrated to the 1.0 release, 
developers using
that package may need to explicitly remove these old packages
from their build.

# Migrating to 1.0

Aside from the all new package ids, the 1.0 release contains some 
minor breaking changes to the way things get initialized.
Migrating from previous versions will in some cases require an
extra initialization call.  The actual API for talking to SQLite
has not changed.

# SemVer

Starting with the 1.0 release, I will be attempting to follow
SemVer for the verson numbers.

# How the packaging works

The main assembly is SQLitePCLRaw.core.  A PCL project would
need to only take a dep on this one.  All the other packages
deal with initialization and the question of which instance 
of the native SQLite library is involved.

# Many different native SQLite libraries

In some cases, apps use a SQLite library which is externally
provided.  In other cases, an instance of the SQLite library
is bundled with the app.

- On iOS, there is a SQLite library provided with the operating
system, and apps are allowed to use it.

- Android also has a SQLite library, and prior to Android N,
apps were allowed to use it.

- Recent versions of Windows 10 have a SQLite library.

- In some cases, people want to use SQLCipher as their SQLite
library.

- Sometimes people want to compile and bundle their own custom SQLite
library.

SQLitePCL.raw supports any of these cases.

# Providers

In this context, a "provider" is the piece of code which tells
SQLitePCL.raw which instance of the native code to use.

As of 1.0, the SQLitePCLRaw.core package contains no providers.
This is the essence of how things have changed in the last few
releases.

- In 0.8.x and prior, the main package tried to contain all the
providers that might be needed.  But the package was getting
enormous, and the use of funky MSBuild properties was a
frustrating way to do configuration.

- In 0.9.x, the main package contained one default provider
as a fallback, and all the others were moved into packages
named SQLitePCL.plugin.\*.  But this was problematic for
certain UWP scenarios (which could not handle unused
providers in the build), and Android N (where the so-called
default provider is always the wrong thing to use).

- In 1.0, the main package contains no providers, and
requires that one be, er, provided.

More specifically, a "provider" is an implementation of
the ISQLite3Provider interface.  It is necessary to call
SQLitePCL.raw.SetProvider() to initialize things.

All the various providers are in packages with ids of
the form SQLitePCLRaw.provider.\*.

# Provider names

Providers are named for the exact string which is used
for DllImport (pinvoke).

For example:

    [DllImport("foo")]
    public static extern int whatever();

This pinvoke will look for a library called "foo".

- On Windows, that means "foo.dll".
- On Unix, "libfoo.so"
- On MacOS, "libfoo.dylib"

(The actual rules are more complicated than this.)

So, a provider where all the DllImport attributes were
using "foo", would have "foo" in its package id and
in its class name.

# Included providers

SQLitePCL.raw includes the following providers:

- "sqlite3" -- This matches the name of the system-provided SQLite
on iOS (which is fine), and Android (which is not allowed).  It
also matches the name of the SQLite library provided by the
extension SDKs in the Visual Studio gallery.  And it matches
the official name of builds provided at sqlite.org.

- "sqlcipher" -- Intended to be used for SQLCipher builds with
(what is assumed to be) the most common form of the library name.

- "winsqlite3" -- Matches the name of the library provided by
recent builds of Windows 10.

- "e\_sqlite3" -- This is the name of all SQLite builds provided
as part of this project.

- "custom\_sqlite3" -- If you want to build your own SQLite library,
give it this name and use this provider.

- "sqlite3\_xamarin" -- Matches the name of the SQLite library provided
by Xamarin.Android for use with Mono.Data.Sqlite.  This is only for
situations where you need to use SQLitePCL.raw to work with the
same SQLite instance as Mono.Data.Sqlite on Android.

# SQLitePCLRaw.lib

A provider is the bridge between the core assembly and the native
code, but the provider does not contain the native code itself.

In some cases (like "winsqlite3") this is because it does not need
to.  The provider is merely a bridge to a SQLite library instance
which is known (or assumed) to be somewhere else.

But in cases where the app is going to be bundling the native
code library, those bits need to make it into your build output
somehow.

Packages with ids named "SQLitePCLRaw.lib.\*" contain native
code.  This project distributes two kinds of these packages:

- "sqlcipher" -- These are re-packaging of the SQLCipher builds
maintained by Couchbase.

- "e\_sqlite3" -- These are builds of the SQLite library provided
for the convenience of SQLitePCL.raw users.  I try to keep them
reasonably current with respect to SQLite itself (www.sqlite.org).
The build configuration is the same for every platform, and includes
full-text-search.  If you are building an app on multiple platforms
and you want to use the same recent version of SQLite on each platform,
e\_sqlite3 should be a good choice.

# A trio of packages

So, using SQLitePCL.raw means you need to add two 
packages:

- SQLitePCLRaw.core
- SQLitePCLRaw.provider.whatever

And in many cases one of these as well:

- SQLitePCLRaw.lib.whatever

And in your platform-specific code, you need to call:

    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_whatever());

But the word "whatever" is different on each platform.
For example, on Android, using e\_sqlite3, you need:

- SQLitePCLRaw.core
- SQLitePCLRaw.provider.e\_sqlite3.android
- SQLitePCLRaw.lib.e\_sqlite3.android

and you need to call:

    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

# Bundles

To make things easier, SQLitePCL.raw includes "bundle" packages.
These packages automatically bring in the right dependencies for
each platform.  They also provide a single Init() call that is the
same for all platforms.

Think of a bundle as way of giving a "batteries included" experience.

So for example, SQLitePCLRaw.bundle\_e\_sqlite3 is a bundle that
uses e\_sqlite3 in all cases.  Just add this package, and call:

    SQLitePCL.Batteries.Init();

SQLitePCLRaw.bundle\_green is a bundle that
uses e\_sqlite3 everywhere except iOS, where the system-provided
SQLite is used.

SQLitePCLRaw.bundle\_sqlcipher does not exist yet, but probably
will soon.

The purpose of the bundles is to make things easier by taking
away flexibility and control.  You don't have to use them.

# e\_sqlite3 builds for Windows

For Windows, I have a lib.e\_sqlite3 package for each platform
toolset:

- v110
- v110\_wp80
- v110\_xp
- v120
- v120\_wp81
- v140

In most desktop scenarios, v110\_xp is what people want.  It
contains a statically linked C runtime lib.  This is the one
that gets included by bundle\_e\_sqlite3 for net45.

For UWP, v140 is the one you want.

In general, nobody cares about this.  The bundles do the right thing.
But the options are there for people who need fine-grained control
over such things.

# e\_sqlite3 builds for Linux and Mac

SQLitePCLRaw.lib.e\_sqlite3 packages are provided for
Linux and Mac.  Both bundle\_green and bundle\_e\_sqlite3
have a dependency on these for net35/net40/net45, so
that these bundles will "do the right thing" when used
with Mono on Linux and Mac.

# NetStandard

Release 1.0 supports netstandard.  However, it also still
includes some PCL profiles and platform-specific builds,
some of which *should* be unnecessary.  I may at some point
simplify things by removing redundant stuff and letting
netstandard be used everywhere it can be.

# Windows Phone Silverlight

Two packages are provided for compatibility with Windows Phone 8.0 and Windows Phone 8.1 Silverlight:

- SQLitePCLRaw.provider.e\_sqlite3.wp80
- SQLitePCLRaw.lib.e\_sqlite3.v110\_wp80

These environments do not support pinvoke, so it's a
special case.  The e\_sqlite3 provider is the only one
available.

Note that bundle\_green and bundle\_e\_sqlite3 both support Windows Phone Silverlight.

# CONTENT BELOW THIS LINE IS NOT FULLY UPDATED FOR 1.0

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

## Why are you making this so complicated?

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

## On WinRT-ish platforms, why do I get "Unable to load DLL 'sqlite3': The specified module could not be found." ?

When using any of the RT flavored forms of Windows (Windows Store, Metro, WP81, etc) you must add a
reference to the Visual C++ runtime extension SDK.

