
cd gen_nuspecs
dotnet run ..
if %errorlevel% neq 0 exit /b %errorlevel%
cd ..

call .\gen_providers.bat

del .\nupkgs\*.nupkg

cd src

setlocal enabledelayedexpansion

for %%f in (
	SQLitePCLRaw.core 
	SQLitePCLRaw.impl.callbacks 
	SQLitePCLRaw.provider.dynamic
	SQLitePCLRaw.provider.internal
	SQLitePCLRaw.provider.e_sqlite3
	SQLitePCLRaw.provider.e_sqlcipher
	SQLitePCLRaw.provider.sqlite3
	SQLitePCLRaw.provider.sqlcipher
	SQLitePCLRaw.provider.winsqlite3
	SQLitePCLRaw.ugly
	) do (
		cd %%f
		dotnet pack -c Release
		if !errorlevel! neq 0 exit /b !errorlevel!
		cd ..
)

for %%f in (
	SQLitePCLRaw.batteries_v2.e_sqlite3.dllimport
	SQLitePCLRaw.batteries_v2.e_sqlite3.dynamic
	SQLitePCLRaw.batteries_v2.e_sqlcipher.dllimport
	SQLitePCLRaw.batteries_v2.e_sqlcipher.dynamic
	SQLitePCLRaw.batteries_v2.sqlcipher.dllimport
	SQLitePCLRaw.batteries_v2.sqlcipher.dynamic
	SQLitePCLRaw.batteries_v2.sqlite3
	SQLitePCLRaw.batteries_v2.winsqlite3
	) do (
		cd %%f
		dotnet build -c Release
		if !errorlevel! neq 0 exit /b !errorlevel!
		cd ..
)

for %%f in (
	SQLitePCLRaw.lib.e_sqlite3.android
	SQLitePCLRaw.lib.e_sqlcipher.android
	SQLitePCLRaw.lib.e_sqlite3.ios
	SQLitePCLRaw.lib.e_sqlcipher.ios
	SQLitePCLRaw.lib.sqlcipher.ios.placeholder
	SQLitePCLRaw.batteries_v2.e_sqlite3.internal.ios
	SQLitePCLRaw.batteries_v2.e_sqlcipher.internal.ios
	SQLitePCLRaw.batteries_v2.sqlcipher.internal.ios
	) do (
		cd %%f
		..\..\nuget restore %%f.csproj
		msbuild /p:Configuration=Release
		if !errorlevel! neq 0 exit /b !errorlevel!
		cd ..
)

cd ..

cd nuspecs
call .\pack.bat
if %errorlevel% neq 0 exit /b %errorlevel%
cd ..

dir nupkgs

cd test_nupkgs

cd smoke
dotnet run
if %errorlevel% neq 0 exit /b %errorlevel%
cd ..

cd with_xunit
dotnet test
if %errorlevel% neq 0 exit /b %errorlevel%
cd ..

cd..


