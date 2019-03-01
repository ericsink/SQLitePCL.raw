rem https://github.com/mono/t4
t4 -o src\SQLitePCLRaw.provider.dynamic\Generated\provider_cdecl.cs -p:NAME=Cdecl -p:CONV=Cdecl -p:KIND=dynamic src\common\provider.tt
t4 -o src\SQLitePCLRaw.provider.dynamic\Generated\provider_stdcall.cs -p:NAME=StdCall -p:CONV=StdCall -p:KIND=dynamic src\common\provider.tt
t4 -o src\SQLitePCLRaw.provider.e_sqlite3\Generated\provider_e_sqlite3.cs -p:NAME=e_sqlite3 -p:CONV=Cdecl -p:KIND=dllimport -p:NAME_FOR_DLLIMPORT=e_sqlite3 src\common\provider.tt
t4 -o src\SQLitePCLRaw.provider.e_sqlcipher\Generated\provider_e_sqlcipher.cs -p:NAME=e_sqlcipher -p:CONV=Cdecl -p:KIND=dllimport -p:NAME_FOR_DLLIMPORT=e_sqlcipher src\common\provider.tt
t4 -o src\SQLitePCLRaw.provider.sqlite3\Generated\provider_sqlite3.cs -p:NAME=sqlite3 -p:CONV=Cdecl -p:KIND=dllimport -p:NAME_FOR_DLLIMPORT=sqlite3 src\common\provider.tt
t4 -o src\SQLitePCLRaw.provider.sqlcipher\Generated\provider_sqlcipher.cs -p:NAME=sqlcipher -p:CONV=Cdecl -p:KIND=dllimport -p:NAME_FOR_DLLIMPORT=sqlcipher src\common\provider.tt
t4 -o src\SQLitePCLRaw.provider.winsqlite3\Generated\provider_winsqlite3.cs -p:NAME=winsqlite3 -p:CONV=StdCall -p:KIND=dllimport -p:NAME_FOR_DLLIMPORT=winsqlite3 src\common\provider.tt
t4 -o src\SQLitePCLRaw.provider.internal\Generated\provider_internal.cs -p:NAME=internal -p:CONV=Cdecl -p:KIND=dllimport -p:NAME_FOR_DLLIMPORT=__Internal src\common\provider.tt

