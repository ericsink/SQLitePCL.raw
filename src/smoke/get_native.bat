if [%1]==[] exit /b -1
mkdir bin\Debug\%1\runtimes\win-x64\native
copy ..\..\..\cb\bld\bin\e_sqlite3\win\v141\plain\x64\e_sqlite3.dll bin\Debug\%1\runtimes\win-x64\native
