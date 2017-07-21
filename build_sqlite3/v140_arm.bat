call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86_arm
@echo on
set TOOLSET=v140
set PLATFORM=arm
CL.exe /nologo /c /Zi /W1 /WX- /sdl- /O2 /Oi /Oy- %SQLITE_OPTIONS% /D NDEBUG /D _USRDLL /D _WINDLL /D _ARM_WINAPI_PARTITION_DESKTOP_SDK_AVAILABLE=1 /Gm- /EHsc /MT /GS /Gy /fp:precise /Zc:wchar_t /Zc:forScope /Fo".\obj\%TOOLSET%\%PLATFORM%\\" /Fd".\obj\%TOOLSET%\%PLATFORM%\vc110.pdb" /Gd /TC /analyze- /errorReport:queue ..\sqlite3\sqlite3.c
link.exe /nologo /ERRORREPORT:QUEUE /OUT:"bin\%TOOLSET%\%PLATFORM%\e_sqlite3.dll" kernel32.lib user32.lib uuid.lib /MANIFEST /MANIFESTUAC:"level='asInvoker' uiAccess='false'" /manifest:embed /PDB:"bin\%TOOLSET%\%PLATFORM%\e_sqlite3.pdb" /SUBSYSTEM:CONSOLE /OPT:REF /OPT:ICF /TLBID:1 /WINMD:NO /DYNAMICBASE /NXCOMPAT /IMPLIB:"bin\%TOOLSET%\%PLATFORM%\e_sqlite3.lib" /MACHINE:ARM /DLL obj\%TOOLSET%\%PLATFORM%\sqlite3.obj

