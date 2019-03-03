mkdir empty
..\nuget pack -OutputDirectory ..\nupkgs SQLitePCLRaw.lib.e_sqlite3.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -OutputDirectory ..\nupkgs SQLitePCLRaw.lib.e_sqlcipher.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_green.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_e_sqlite3.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_e_sqlcipher.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_zetetic.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
..\nuget pack -OutputDirectory ..\nupkgs SQLitePCLRaw.bundle_winsqlite3.nuspec
if %errorlevel% neq 0 exit /b %errorlevel%
