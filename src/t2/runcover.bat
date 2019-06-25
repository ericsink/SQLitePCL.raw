del coverage.xml
del /Q report\*
del /Q __Instrumented\*
dotnet build
altcover --inputDirectory=.\bin\Debug\netcoreapp2.2 --opencover
dotnet .\__Instrumented\tests.dll
dotnet reportgenerator -reports:coverage.xml -targetdir:report "-assemblyfilters:-xunit.*"
