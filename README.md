
# SQLitePCLRaw

SQLitePCLRaw is a Portable Class Library (PCL) for low-level (raw)
access to SQLite. License:  Apache License v2.

# How you can support this project

In the .NET ecosystem, SQLitePCLRaw is very popular.  According to nuget.org, it has been downloaded hundreds of millions of times.

Maintaining this package is a non-trivial effort, and I am asking the .NET community to support that effort.  Here are three ways you can do that:

1.  Send me money
2.  Do business with my company
3.  Do me a favor

## (1) Send me money

To support this project financially, you can use GitHub Sponsors:

https://github.com/sponsors/ericsink

Any amount is accepted and appreciated.

## (2) Do business with my company

My business partner (Corey Steffen) and I own a software development company called SourceGear.  Our team does software development projects for other companies, including server side, web applications, and mobile apps.  You can support my efforts by engaging with our company.

https://services.sourcegear.com/

We use a variety of technologies depending on the needs of the customer, but we are obviously adept in the use of .NET and SQLite.  Please contact me to discuss how our team could be of service.

## (3) Do me a favor

Want to support my efforts without using money?  Do me a favor by telling people about my word games.  :-)

(a) Word Zero -- a two-person game similar to Scrabble.  Currently iOS-only, available in the App Store.  Options for both free and paid play.

(b) Word Box -- a free daily word puzzle, available at https://wordbox.game/

I am always wanting to attract more players, but purchasing ads for these kinds of games is very expensive.  You would be doing me a favor by spreading the word.  For example, Word Box has an easy way to post your score for the day on social media.

And if you choose this "Do me a favor" option, make sure to let me know, so I can thank you.

# How the packaging works

The main assembly is SQLitePCLRaw.core.  A portable library project would
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

SQLitePCLRaw supports any of these cases.

# Providers

In this context, a "provider" is the piece of code which tells
SQLitePCLRaw which instance of the native code to use.

More specifically, a "provider" is an implementation of
the ISQLite3Provider interface.  It is necessary to call
SQLitePCL.raw.SetProvider() to initialize things.

The SQLitePCLRaw.core package contains no providers.

All the various providers are in packages with ids of
the form SQLitePCLRaw.provider.\*.

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

- "e\_sqlite3" -- These are builds of the SQLite library provided
for the convenience of SQLitePCLRaw users.  I try to keep them
reasonably current with respect to SQLite itself (www.sqlite.org).
The build configuration is the same for every platform, and includes
full-text-search.  If you are building an app on multiple platforms
and you want to use the same recent version of SQLite on each platform,
e\_sqlite3 should be a good choice.

- "e\_sqlcipher" -- These are unofficial and unsupported builds
of the open source SQLCipher code.  You should not use these packages.
You should buy official supported builds from Zetetic.

- "e\_sqlite3mc" -- These are builds of SQLite3MultipleCiphers, another
library which adds encryption capabilities to SQLite.

The build scripts for all of the above are in the ericsink/cb repo.

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

SQLitePCLRaw.bundle\_green is a bundle that
uses e\_sqlite3 everywhere except iOS, where the system-provided
SQLite is used.

The purpose of the bundles is to make things easier by taking
away flexibility and control.  You don't have to use them.

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
it does a fantastic job of the SQLite side of things.  But it is not at all mobile-friendly.

## Why is this called SQLitePCLRaw?

[SQLitePCL](https://sqlitepcl.codeplex.com/) was a SQLite Portable Class Library released on Codeplex by MS Open Tech.

This library is a fork of that code.  Sort of.

It is a fork in the 2007 sense of the word.  I made significant use of the code.  I preserved copyright notices.

However, this is not the the sort of fork which is created for the purpose of
producing a pull request.  The changes I've made are so extensive that I do not
plan to submit a pull request unless one is requested.  I plan to maintain this
code going forward.

