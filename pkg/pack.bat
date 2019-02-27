mkdir empty
mkdir nupkg
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.core.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.ugly.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.lib.e_sqlite3.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.lib.e_sqlcipher.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_green.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_e_sqlite3.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_e_sqlcipher.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_zetetic.nuspec
..\nuget pack -OutputDirectory nupkg SQLitePCLRaw.bundle_winsqlite3.nuspec
dir nupkg\*.nupkg
