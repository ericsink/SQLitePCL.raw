dotnet test /p:AltCover=true
dotnet reportgenerator -reports:coverage.xml -targetdir:report "-assemblyfilters:-xunit.*"
