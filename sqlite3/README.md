
This directory contains msbuild/vcxproj stuff for building the SQLite C code
as static libraries in all the various ways that we need.

The SQLite code itself is located in ..\src\sqlite3.

All the vcxproj files are basically hand-written.  There is one for each
platform, but each one tries to do the minimum necessary for that specific
platform while letting import of "sqlite3\_common.msbuild" do most of the 
actual work.

These are referenced by the various cppinterop projects (C++/CLI or C++/CX
wrappers that expose native code to .NET or WinRT or WindowsPhone).


