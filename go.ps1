csc /w:4 gen_build.cs
./gen_build.exe
cd bld
./build.ps1 > err.txt 2>&1
./pack.ps1
./bt.ps1

