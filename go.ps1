msbuild /P:Config=Debug src/tools/tools.sln
./bin/Debug/gen_build.exe
cd bld
./build.ps1 > err.txt 2>&1
./pack.ps1
./bt.ps1

