t4 -o mt\SQLitePCLRaw.provider.dynamic\Generated\provider_cdecl.cs -p:NAME=Cdecl -p:CONV=Cdecl -p:KIND=dynamic src\cs\provider.tt
t4 -o mt\SQLitePCLRaw.provider.dynamic\Generated\provider_stdcall.cs -p:NAME=StdCall -p:CONV=StdCall -p:KIND=dynamic src\cs\provider.tt
t4 -o mt\SQLitePCLRaw.provider.e_sqlite3\Generated\provider_e_sqlite3.cs -p:NAME=e_sqlite3 -p:CONV=Cdecl -p:KIND=dllimport -p:NAME_FOR_DLLIMPORT=e_sqlite3 src\cs\provider.tt

