mkdir empty
..\nuget pack -properties version=2.0.0-pre20190305063859 -OutputDirectory ..\nupkgs SQLitePCLRaw.lib.e_sqlite3.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -properties version=2.0.0-pre20190305063859 -OutputDirectory ..\nupkgs SQLitePCLRaw.lib.e_sqlcipher.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -properties version=2.0.0-pre20190305063859 -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_green.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -properties version=2.0.0-pre20190305063859 -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_e_sqlite3.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -properties version=2.0.0-pre20190305063859 -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_e_sqlcipher.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -properties version=2.0.0-pre20190305063859 -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_zetetic.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -properties version=2.0.0-pre20190305063859 -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_winsqlite3.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
