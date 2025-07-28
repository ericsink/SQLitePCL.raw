
# SQLitePCLRaw

SQLitePCLRaw is a .NET Portable Class Library (PCL) for low-level (raw)
access to SQLite.

This library is open source (Apache License v2) and has been downloaded
hundreds of millions of times on nuget.org.

My name is Eric Sink.  I am:

- the maintainer of this library
- the founder of SourceGear, a small software company in Illinois
- part of the SQLite core team

You can email me at either eric@sourcegear.com or eric@sqlite.org

# Technical Support

If you need help using SQLite with .NET, you can post a question
here in this repo, and there is a chance that I or someone else
will answer it.

If you are interested in private technical support with more structure,
SourceGear provides technical support agreements at
various levels.  Contact me for a price quote.

If you are not a SourceGear customer and you contact me directly 
with technical support questions, I may not respond.

# SQLite Builds

The SQLitePCLRaw.lib.e_sqlite3 package is published publicly on nuget.org 
as part of this project.  It contains builds of the native SQLite code library
for server platforms and with the most commonly used features enabled.
I update this package from time to time.

SourceGear operates a paid service at nuget.sourcegear.com
which provides various native SQLite builds which are
updated immediately after each SQLite release.  These include regular
SQLite builds, or various options with encryption support.  We can also provide
custom configurations.

If you are interested in SourceGear's SQLite build service, please
contact me for a price quote.

# Encryption

I no longer publish encryption-enabled SQLite builds without cost.

My recommended solution for encryption support is the SQLite Encryption Extension (SEE), which is the official implementation from the SQLite team:

https://sqlite.org/com/see.html

The SEE is not open source -- a paid license is required.  SourceGear's SQLite build service
provides SEE builds in the form of nuget packages.

The recommended way to use a crypto-enabled SQLite build is to name the shared library `e_sqlite3` and use the `SQLitePCLRaw.config.e_sqlite3` package.
Using this approach, you can use open source alternatives to SEE, including:

- SQLCipher (builds available for purchase from Zetetic)
- SQLite3 Multiple Ciphers (maintained by @utelle, builds available in SourceGear's SQLite build service)

# How you can support this project

If you are not a SourceGear customer and would like support this open 
source project for the benefit of the .NET community, you can do so 
through GitHub Sponsors:

https://github.com/sponsors/ericsink

Any amount is accepted and appreciated.

# The structure of these packages

The main assembly is SQLitePCLRaw.core.  A portable library project would
need to only take a dependency on this one.  All the other packages
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

There is also a `dynamic` provider which does not use a hard-coded
DllImport string.  This one is the most difficult to use but can
support advanced use cases where the loading decision needs
to be made at runtime.

# Included providers

SQLitePCLRaw includes several different providers.  Examples:

- "e\_sqlite3" -- This is the name of all SQLite builds provided
as part of this project.

- "sqlite3" -- This matches the name of the system-provided SQLite
on iOS (which is fine), and Android (which is not allowed).
And it matches the official name of builds provided at sqlite.org.

- "sqlcipher" -- Intended to be used for official SQLCipher builds
from Zetetic.

- "winsqlite3" -- Matches the name of the library provided by
recent builds of Windows.

- "dynamic" -- Uses dynamic loading of the native library
instead of DllImport attributes.  This can be used to support
advanced use cases.

# SQLitePCLRaw.lib.e\_sqlite3

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

And in many cases, you also need a package containing the native SQLite library itself.  For example:

- SQLitePCLRaw.lib.e_sqlite3

Then, in your platform-specific code, you need to call:

    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_whatever());

But the word "whatever" can differ, depending on the platform or desired 
configuration.

In the most common cases, using DllImport to load a library named
"e_sqlite3", the initialization call looks like this:

    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

but some platforms are special, like iOS, for example.

For convenience, the "SQLitePCLRaw.config.e_sqlite3" package provides the 
right dependencies and initialization code for each platform to use
any SQLite library named "e_sqlite3".  Using this package, you
can initialize things on any platform by calling:

    SQLitePCL.Batteries_V2.Init();

But you still need to bring in a native SQLite library with
"e_sqlite3" as the base name.  The "SQLitePCLRaw.config.e_sqlite3" doesn't
do that, because there are multiple possibilities, and it wants to defer 
the choice until later.

If you want to use a plain SQLite library with the most commonly used
features, just add the "SQLitePCLRaw.lib.e_sqlite3" package.

For backward compatibility with previous releases, the SQLitePCLRaw.bundle_e_sqlite3
package continues to be available, but all it does is bring in two dependencies:

- SQLitePCLRaw.config.e_sqlite3
- SQLitePCLRaw.lib.e_sqlite

## How do I build SQLitePCLRaw?

#### Requirements

* Install the .NET SDK
* Install the `t4` cli tool with `dotnet tool install --global dotnet-t4`
* Install the workloads for iOS, Android, tvOS, and MacCatalyst

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
core SQLite team.

In mid-2025, I joined the SQLite core team and became the maintainer of System.Data.SQLite,
but despite the fact that I maintain both, System.Data.SQLite and SQLitePCLRaw are unrelated and likely to remain that way.

## Why is this called SQLitePCLRaw?

[SQLitePCL](https://sqlitepcl.codeplex.com/) was a SQLite Portable Class Library released on Codeplex by MS Open Tech.

This library started as fork of that code, but I removed all the higher-level functionality, and what was
left was "raw".

