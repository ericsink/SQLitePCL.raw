
# SQLitePCLRaw

SQLitePCLRaw is a Portable Class Library (PCL) for low-level (raw)
access to SQLite. License:  Apache License v2.

# How the packaging works

The main assembly is SQLitePCLRaw.core.  A portable library project would
need to only take a dep on this one.  All the other packages
deal with initialization and the question of which instance 
of the native SQLite library is involved.

# Many different native SQLite libraries

- On iOS, there is a SQLite library provided with the operating
system, and apps are allowed to use it.

- Android also has a SQLite library, and prior to Android N,
apps were allowed to use it.

- Recent versions of Windows have a SQLite library.

- In some cases, people want to use SQLCipher as their SQLite
library.

- Sometimes people want to compile and bundle their own custom SQLite
library.

SQLitePCLRaw supports any of these cases.

# Providers

In this context, a "provider" is the piece of code which tells
SQLitePCLRaw which instance of the native code to use.

More specifically, a "provider" is an implementation of
the `ISQLite3Provider` interface.  It is necessary to call
`SQLitePCL.raw.SetProvider()` to initialize things.

The SQLitePCLRaw.core package contains no providers.

All the various providers are in packages with ids of
the form SQLitePCLRaw.provider.\*.

# Provider names

There is a `dynamic` provider which does not use a hard-coded
DllImport string.  This one is the most difficult to use but offers
the most control.  It can be used to load a custom-built native
SQLite library.

The DllImport-based providers are named for the exact string which is used
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

SQLitePCLRaw includes several different providers.  Examples:

- "dynamic" -- Uses dynamic loading of the native library
instead of DllImport attributes.

- "e\_sqlite3" -- This is the name of all SQLite builds provided
as part of this project.

- "sqlite3" -- This matches the name of the system-provided SQLite
on iOS (which is fine), and Android (which is not allowed).
And it matches the official name of builds provided at sqlite.org.

- "sqlcipher" -- Intended to be used for official SQLCipher builds
from Zetetic.

- "winsqlite3" -- Matches the name of the library provided by
recent builds of Windows.

# SQLitePCLRaw.lib.e\_sqlite3 packages

A provider is the bridge between the core assembly and the native
code, but the provider does not contain the native code itself.

In some cases (like "winsqlite3") this is because it does not need
to.  The provider is merely a bridge to a SQLite library instance
which is known (or assumed) to be somewhere else.

But in cases where the app is going to be bundling the native
code library, those bits need to make it into your build output
somehow.

For the convenience of developers using SQLitePCLRaw, the `SQLitePCLRaw.lib.e_sqlite3` 
package contains builds of the native SQLite code for several platforms.
The build configuration is the same for every platform, and includes
full-text-search.  

# A trio of packages

So, using SQLitePCLRaw means you need to add two 
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

To make things easier, SQLitePCLRaw includes "bundle" packages.
These packages automatically bring in the right dependencies for
each platform.  They also provide a single Init() call that is the
same for all platforms.

Think of a bundle as way of giving a "batteries included" experience.

So for example, SQLitePCLRaw.bundle\_e\_sqlite3 is a bundle that
uses e\_sqlite3 in all cases.  Just add this package, and call:

    SQLitePCL.Batteries_V2.Init();

The purpose of the bundles is to make things easier by taking
away flexibility and control.  You don't have to use them.

## How do I build this?

#### Requirements

TODO need .NET SDK
* Install the `t4` cli tool with `dotnet tool install --global dotnet-t4`
* Make sure that the *Mobile development with.NET* workload [is installed](https://docs.microsoft.com/en-us/visualstudio/install/modify-visual-studio)

```
cd build
dotnet run
```

## Can this library be used to write a mobile app?

Technically, yes, but that's not what you want to do.
This is *not* the sort of SQLite library you would use to write an app.
It is a *very* thin C# wrapper around the C API for SQLite.  It's "raw".

Consequently, as much as
possible, this library follows the stylistic conventions of SQLite, not those of
the .NET/C# world.

For example, the C function for opening a SQLite file
is `sqlite3\_open()`, so this API provides a method called `sqlite3\_open()`,
not `Sqlite3Open()`.  

Similarly, the functions in this API return integer error codes
rather than throwing .NET exceptions, because that's how the SQLite C API
works.

As a library for app developers, this library is downright hostile.
It feels like using C.  Intentionally.

## So if this library is so unfriendly, why does it exist at all?

This library is designed to be the common portable layer upon which friendlier
wrappers can be built.  Before this existed, every C# SQLite library was writing their
own P/Invoke and COM and marshaling and stuff.
Building on this library instead allows folks to focus more on the upper layer and its
goal of providing a pleasant, easy-to-use API for app developers.

## How does this compare to Microsoft.Data.Sqlite?

[Microsoft.Data.Sqlite](https://github.com/aspnet/Microsoft.Data.Sqlite) is an 
ADO.NET-style SQLite wrapper which is part of Entity Framework Core.  It uses
SQLitePCLRaw.

## How does this compare to sqlite-net?

[sqlite-net](https://github.com/praeclarum/sqlite-net) is a very popular SQLite wrapper by Frank Krueger (@praeclarum).
Unlike SQLitePCLRaw, it is designed to make writing apps easier.  It even includes a lightweight ORM,
and some basic support for LINQ.  

The `sqlite-net-pcl` package uses SQLitePCLRaw:

[https://www.nuget.org/packages/sqlite-net-pcl/](https://www.nuget.org/packages/sqlite-net-pcl/)

## How does this compare to System.Data.SQLite?

[System.Data.SQLite](http://system.data.sqlite.org) is an ADO.NET-style SQLite wrapper developed by the
core SQLite team.  It is very full-featured, supporting LINQ and Entity Framework.  And for obvious reasons, 
it does a fantastic job of the SQLite side of things.

In mid-2025, I joined the SQLite core team and became the maintainer of System.Data.SQLite,
but System.Data.SQLite and SQLitePCLRaw are unrelated and likely to remain that way.

## Why is this called SQLitePCLRaw?

[SQLitePCL](https://sqlitepcl.codeplex.com/) was a SQLite Portable Class Library released on Codeplex by MS Open Tech.

This library is a fork of that code.  Sort of.

It is a fork in the 2007 sense of the word.  I made significant use of the code.  I preserved copyright notices.

However, this is not the the sort of fork which is created for the purpose of
producing a pull request.  The changes I've made are so extensive that I do not
plan to submit a pull request unless one is requested.  I plan to maintain this
code going forward.

# Encryption support

My recommended solution for encryption support is the SQLite Encryption Extension (SEE), which is the official implementation from the SQLite team:

https://sqlite.org/com/see.html

The SEE is not open source -- a paid license is required. 

SQLitePCLRaw.provider.e\_see supports native code builds with the base name `e_see`.  TODO

SQLitePCLRaw also includes providers for two open source alternatives to the SEE: 

- SQLCipher (from Zetetic)
- SQLite3 Multiple Ciphers (from @utelle).

I no longer maintain and distribute encryption-enabled SQLite builds without cost.

