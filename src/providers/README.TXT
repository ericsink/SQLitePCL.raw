
this works:

t4 -o tmp.cs -p:NAME=tmp -p:CONV=Cdecl -p:KIND=dynamic provider.tt

same, but with "dotnet " in front, to use the local tool, fails:

dotnet t4 -o tmp.cs -p:NAME=tmp -p:CONV=Cdecl -p:KIND=dynamic provider.tt

