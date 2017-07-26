call %VCVARSBAT% %TOOLCHAIN%
@echo on
mkdir .\obj\%SUBDIR%
mkdir .\bin\%SUBDIR%
CL.exe /nologo /c /Zi /W1 /WX- /sdl- /O2 /Oi /Oy- %SQLITE_OPTIONS% /D NDEBUG /D _USRDLL %EXTRA_CL_OPTIONS% /D _WINDLL /Gm- /EHsc %CRT_OPTION% /GS /Gy /fp:precise /Zc:wchar_t /Zc:forScope /Fo".\obj\%SUBDIR%\\" /Gd /TC /analyze- /errorReport:queue ..\sqlite3\sqlite3.c
link.exe /nologo /ERRORREPORT:QUEUE /OUT:"bin\%SUBDIR%\e_sqlite3.dll" %EXTRA_LINK_OPTIONS% %LIBS% %MANIFEST_OPTIONS% /SUBSYSTEM:CONSOLE /OPT:REF /OPT:ICF /TLBID:1 /WINMD:NO /DYNAMICBASE /NXCOMPAT /IMPLIB:"bin\%SUBDIR%\e_sqlite3.lib" /MACHINE:%MACHINE% /DLL obj\%SUBDIR%\sqlite3.obj

