csc /w:4 gen_nuspecs.cs
.\gen_nuspecs.exe

call .\gen_providers.bat

del .\nupkgs\*.nupkg

cd src

cd SQLitePCLRaw.core
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.impl.callbacks
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.provider.dynamic
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.provider.internal
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.provider.e_sqlite3
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.provider.e_sqlcipher
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.provider.sqlite3
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.provider.sqlcipher
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.provider.winsqlite3
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.batteries_v2.e_sqlite3
dotnet build -c Release
cd ..

cd SQLitePCLRaw.batteries_v2.e_sqlcipher
dotnet build -c Release
cd ..

cd SQLitePCLRaw.batteries_v2.sqlite3
dotnet build -c Release
cd ..

cd SQLitePCLRaw.batteries_v2.sqlcipher
dotnet build -c Release
cd ..

cd SQLitePCLRaw.batteries_v2.winsqlite3
dotnet build -c Release
cd ..

cd SQLitePCLRaw.ugly
dotnet pack -c Release
cd ..

cd SQLitePCLRaw.lib.e_sqlite3.android
..\..\nuget restore SQLitePCLRaw.lib.e_sqlite3.android.csproj
msbuild /p:Configuration=Release
cd ..

cd SQLitePCLRaw.lib.e_sqlcipher.android
..\..\nuget restore SQLitePCLRaw.lib.e_sqlcipher.android.csproj
msbuild /p:Configuration=Release
cd ..

cd SQLitePCLRaw.lib.e_sqlite3.ios
..\..\nuget restore SQLitePCLRaw.lib.e_sqlite3.ios.csproj
msbuild /p:Configuration=Release
cd ..

cd SQLitePCLRaw.lib.e_sqlcipher.ios
..\..\nuget restore SQLitePCLRaw.lib.e_sqlcipher.ios.csproj
msbuild /p:Configuration=Release
cd ..

cd SQLitePCLRaw.lib.sqlcipher.ios.placeholder
..\..\nuget restore SQLitePCLRaw.lib.sqlcipher.csproj
msbuild /p:Configuration=Release
cd ..

cd SQLitePCLRaw.batteries_v2.e_sqlite3.internal.ios
..\..\nuget restore SQLitePCLRaw.batteries_v2.e_sqlite3.internal.ios.csproj
msbuild /p:Configuration=Release
cd ..

cd SQLitePCLRaw.batteries_v2.e_sqlcipher.internal.ios
..\..\nuget restore SQLitePCLRaw.batteries_v2.e_sqlcipher.internal.ios.csproj
msbuild /p:Configuration=Release
cd ..

cd SQLitePCLRaw.batteries_v2.sqlcipher.internal.ios
..\..\nuget restore SQLitePCLRaw.batteries_v2.sqlcipher.internal.ios.csproj
msbuild /p:Configuration=Release
cd ..

cd ..

cd nuspecs
call .\pack.bat
cd ..

dir nupkgs

cd smoke
dotnet run
cd ..

cd newtest
dotnet test
cd ..


