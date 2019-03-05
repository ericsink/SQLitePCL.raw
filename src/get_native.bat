echo "this.bat tfm arch"
echo "this.bat net461 x86"
echo "this.bat netcoreapp2.1 x64"
if [%1]==[] exit /b -1
if [%2]==[] exit /b -1
mkdir bin\Debug\%1\runtimes\win-%2\native
copy ..\..\..\cb\bld\bin\e_sqlite3\win\v141\plain\%2\e_sqlite3.dll bin\Debug\%1\runtimes\win-%2\native
