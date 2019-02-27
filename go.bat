csc /w:4 gen_build.cs
.\gen_build.exe

call .\gen.bat

cd src

cd SQLitePCLRaw.core
dotnet build -c Release
cd ..

cd SQLitePCLRaw.provider.dynamic
dotnet build -c Release
cd ..

cd SQLitePCLRaw.provider.e_sqlite3
dotnet build -c Release
cd ..

cd SQLitePCLRaw.provider.e_sqlcipher
dotnet build -c Release
cd ..

cd SQLitePCLRaw.provider.sqlite3
dotnet build -c Release
cd ..

cd SQLitePCLRaw.provider.sqlcipher
dotnet build -c Release
cd ..

cd SQLitePCLRaw.provider.winsqlite3
dotnet build -c Release
cd ..

cd SQLitePCLRaw.ugly
dotnet build -c Release
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

cd ..

cd pkg
call .\pack.bat

cd ..

