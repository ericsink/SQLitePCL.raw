# SQLite3 Multiple Ciphers NuGet Package

This library provides C#/.NET bindings for [SQLite3 Multiple Ciphers](https://utelle.github.io/SQLite3MultipleCiphers/). It leverages [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw#readme) to create the bindings.

## Table of Contents

- [Usage](#usage)
- [Passphrase based database encryption support](#passphrase-based-database-encryption-support)
- [Examples for cipher configuration](#examples-for-cipher-configuration)
- [Acknowledgements](#acknowledgements)
- [See also](#see-also)

## Usage

Because the bindings are built using SQLitePCLRaw, you can use them with various .NET libraries.

:warning: **Warning!** Don't use multiple SQLitePCLRaw bundles in the same project.

### Microsoft.Data.Sqlite

For [Microsoft.Data.Sqlite](https://learn.microsoft.com/dotnet/standard/data/sqlite/), be sure to use the [Microsoft.Data.Sqlite.Core](https://www.nuget.org/packages/Microsoft.Data.Sqlite.Core) package instead of the main one to avoid using multiple bundles.

```cs
using Microsoft.Data.Sqlite;

using var connection = new SqliteConnection("Data Source=example.db;Password=Password12!");
connection.Open();

var command = connection.CreateCommand();
command.CommandText = "select sqlite3mc_version()";
var version = (string)command.ExecuteScalar()!;

Console.WriteLine(version);
```

### Dapper

Use [Dapper](https://dapperlib.github.io/Dapper/) with Microsoft.Data.Sqlite by following the same instructions detailed above.

```cs
using Dapper;
using Microsoft.Data.Sqlite;

using var connection = new SqliteConnection("Data Source=example.db;Password=Password12!");
var version = connection.ExecuteScalar<string>("select sqlite3mc_version()");
Console.WriteLine(version);
```

### EF Core

[EF Core](https://learn.microsoft.com/ef/core/) is built on top of Microsoft.Data.Sqlite. Be sure to use the [Microsoft.EntityFrameworkCore.Sqlite.Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite.Core) package instead of the main one to avoid using multiple bundles.

```
options.UseSqlite("Data Source=example.db;Password=Password12!");
```

### SQLite-net

With [SQLite-net](https://github.com/praeclarum/sqlite-net#readme), be sure to use the [sqlite-net-base](https://www.nuget.org/packages/sqlite-net-base) package instead of the main one to avoid using multiple bundles.

```cs
using SQLite;

SQLitePCL.Batteries_V2.Init();

var connection = new SQLiteConnection(new("example.db", storeDateTimeAsTicks: true, key: "Password12!"));
var version = connection.ExecuteScalar<string>("select sqlite3mc_version()");
Console.WriteLine(version);
```

### Low-level bindings

If you *really* want to use the low-level bindings directly, you can. But these are primarly intended to be used by libraries like Microsoft.Data.Sqlite and SQLite-net.

```cs
using static SQLitePCL.raw;

SQLitePCL.Batteries_V2.Init();

var rc = sqlite3_open("example.db", out var db);
if (rc != SQLITE_OK) return;
using (db)
{
    rc = sqlite3_key(db, "Password12!"u8);
    if (rc != SQLITE_OK) return;

    rc = sqlite3_prepare_v2(db, "select sqlite3mc_version()", out var stmt);
    if (rc != SQLITE_OK) return;
    using (stmt)
    {
        rc = sqlite3_step(stmt);
        if (rc != SQLITE_ROW) return;

        var version = sqlite3_column_text(stmt, 0).utf8_to_string();
        Console.WriteLine(version);
    }
}
```
## Passphrase based database encryption support

This NuGet package supports access to **encrypted** [SQLite](https://www.sqlite.org) databases from .NET applications. It is based on the project [SQLite3 Multiple Ciphers](https://utelle.github.io/SQLite3MultipleCiphers/).

**SQLite3 Multiple Ciphers** is an extension to the public domain version of SQLite that allows applications to read and write encrypted database files. Currently 5 different encryption cipher schemes are supported:

- [wxSQLite3](https://github.com/utelle/wxsqlite3): AES 128 Bit CBC - No HMAC
- [wxSQLite3](https://github.com/utelle/wxsqlite3): AES 256 Bit CBC - No HMAC  
Use of the _wxSQLite3_ ciphers is not recommended for new projects.
- [sqleet](https://github.com/resilar/sqleet): ChaCha20 - Poly1305 HMAC  
This cipher scheme is currently the _default_ cipher scheme.
- [SQLCipher](https://www.zetetic.net/sqlcipher/): AES 256 Bit CBC - SHA1/SHA256/SHA512 HMAC  
All _SQLCipher_ variants (from version 1 up to version 4) can be accessed.
- [System.Data.SQLite](http://system.data.sqlite.org): RC4  
Supported for compatibility with earlier _System.Data.SQLite_ versions only. Don't use it in new projects. Since early 2020 the official **System.Data.SQLite** distribution no longer includes the RC4 encryption extension.

In addition to reading and writing encrypted database files it is also possible to read and write plain unencrypted database files.

**SQLite3 Multiple Ciphers** transparently encrypts the entire database file, so that an encrypted SQLite database file appears to be white noise to an outside observer. Not only the database files themselves, but also journal files are encrypted.

For a detailed documentation of the currently supported cipher schemes, their configuration options, and the SQL interface please consult the [SQLite3MultipleCiphers website](https://utelle.github.io/SQLite3MultipleCiphers/).

## Examples for cipher configuration

For accessing a database encrypted with the default cipher scheme specifying just the name of the database file as the _Data Source_ and the passphrase as the _Password_ in the connection string is sufficient:

```cs
using var connection = new SqliteConnection("Data Source=example.db;Password=Password12!");
```

However, for database files encrypted with a non-default cipher scheme the connection string looks a bit different. The following examples illustrate two common use cases.

### How to open an existing database encrypted with _SQLCipher_

If you want to access a database created for example by _bundle_e_sqlcipher_ (or any other tool supporting the original _SQLCipher_ cipher scheme), it is necessary to configure the cipher scheme on establishing the database connection, because _SQLCipher_ is not the default cipher scheme.

The easiest approach to accomplish this is to specify the data source in the connection string as a _Uniform Resource Identifier_ (URI) including the required configuration parameters as URI parameters. In case of _SQLCipher_ two configuration parameters are required:

1. `cipher=sqlcipher` - select the _SQLCipher_ cipher scheme
2. `legacy=4` - select the current _SQLCipher_ version 4 (in use since November 2018)

The resulting connection string looks like this:

```cs
using var connection = new SqliteConnection("Data Source=file:example.db?cipher=sqlcipher&legacy=4;Password=Password12!");
```

**Note**:  
For prior _SQLCipher_ versions use the matching version number as the value of the `legacy` parameter. For non-default _SQLCipher_ encryption variants you may need to specify additional parameters. For a detailed list of parameters see the [SQLite3 Multiple Ciphers documentation](https://utelle.github.io/SQLite3MultipleCiphers/docs/ciphers/cipher_sqlcipher/).

### How to open an existing database that was encrypted with _System.Data.SQLite_

If you want to access a database created for example by _System.Data.SQLite_, it is again necessary to configure the cipher scheme on establishing the database connection.

The easiest approach to accomplish this is to specify the data source in the connection string as a _Uniform Resource Identifier_ (URI) including the required configuration parameters as URI parameters. In case of _System.Data.SQLITE_ RC4 one or two configuration parameters are required:

1. `cipher=rc4` - select the _System.Data.SQLITE_ RC4 cipher scheme
2. `legacy_page_size=<page size in bytes>` - optional, if the database uses the default SQLite page size (currently **4096** bytes); required, if a non-default page size is used.

The resulting connection string looks like this:

```cs
using var connection = new SqliteConnection("Data Source=file:example.db?cipher=rc4;Password=Password12!");
```

## Acknowledgements

The following people have contributed to **SQLite3 Multiple Ciphers NuGet**:

- [Brice Lambson](https://github.com/bricelam)
- [Josh Menzel](https://github.com/jammerxd)
- [Eric Sink](https://github.com/ericsink)
- [Ulrich Telle](https://github.com/utelle)

## See also

- [SQLite3 Multiple Ciphers](https://utelle.github.io/SQLite3MultipleCiphers/)
- [Microsoft.Data.Sqlite](https://learn.microsoft.com/dotnet/standard/data/sqlite/)
- [SQLite-net](https://github.com/praeclarum/sqlite-net#readme)
