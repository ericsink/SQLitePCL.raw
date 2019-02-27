mkdir empty
mkdir nupkg
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.core.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.provider.dynamic.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.provider.e_sqlite3.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.provider.e_sqlcipher.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.provider.sqlite3.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.provider.sqlcipher.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.provider.winsqlite3.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.ugly.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.lib.e_sqlite3.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.lib.e_sqlcipher.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_green.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_e_sqlite3.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_e_sqlcipher.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_zetetic.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_winsqlite3.nuspec
dir nupkg\*.nupkg
