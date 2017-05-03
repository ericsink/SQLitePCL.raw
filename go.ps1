msbuild /P:Config=Release src/gen_build/gen_build.sln
.\bin\gen_build.exe
cd bld
./build.ps1 > err.txt 2>&1
./pack.ps1
./bt.ps1

