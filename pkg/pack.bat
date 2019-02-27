mkdir empty
..\nuget pack SQLitePCLRaw.core.nuspec
..\nuget pack SQLitePCLRaw.ugly.nuspec
..\nuget pack SQLitePCLRaw.lib.e_sqlite3.nuspec
..\nuget pack SQLitePCLRaw.lib.e_sqlcipher.nuspec
..\nuget pack SQLitePCLRaw.bundle_green.nuspec
..\nuget pack SQLitePCLRaw.bundle_e_sqlite3.nuspec
..\nuget pack SQLitePCLRaw.bundle_e_sqlcipher.nuspec
..\nuget pack SQLitePCLRaw.bundle_zetetic.nuspec
..\nuget pack SQLitePCLRaw.bundle_winsqlite3.nuspec
dir *.nupkg
