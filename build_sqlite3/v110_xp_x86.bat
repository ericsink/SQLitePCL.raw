call "C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\vcvarsall.bat" x86
@echo on
set TOOLSET=v110_xp
set PLATFORM=x86
CL.exe /nologo /c /Zi /W1 /WX- /sdl- /O2 /Oi /Oy- %SQLITE_OPTIONS% /D NDEBUG /D _USRDLL /D _USING_V110_SDK71_ /D _WINDLL /Gm- /EHsc /MT /GS /Gy /fp:precise /Zc:wchar_t /Zc:forScope /Fo".\obj\%TOOLSET%\%PLATFORM%\\" /Fd".\obj\%TOOLSET%\%PLATFORM%\vc110.pdb" /Gd /TC /analyze- /errorReport:queue ..\sqlite3\sqlite3.c
link.exe /nologo /ERRORREPORT:QUEUE /OUT:"bin\%TOOLSET%\%PLATFORM%\e_sqlite3.dll" kernel32.lib user32.lib uuid.lib /MANIFEST /MANIFESTUAC:"level='asInvoker' uiAccess='false'" /manifest:embed /PDB:"bin\%TOOLSET%\%PLATFORM%\e_sqlite3.pdb" /SUBSYSTEM:CONSOLE /OPT:REF /OPT:ICF /TLBID:1 /WINMD:NO /DYNAMICBASE /NXCOMPAT /IMPLIB:"bin\%TOOLSET%\%PLATFORM%\e_sqlite3.lib" /MACHINE:X86 /SAFESEH /DLL obj\%TOOLSET%\%PLATFORM%\sqlite3.obj

