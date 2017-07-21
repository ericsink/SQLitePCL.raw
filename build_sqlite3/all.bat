SET SQLITE_OPTIONS=/D SQLITE_ENABLE_COLUMN_METADATA /D SQLITE_ENABLE_FTS3_PARENTHESIS /D SQLITE_ENABLE_FTS4 /D SQLITE_ENABLE_JSON1 /D SQLITE_ENABLE_RTREE /D SQLITE_DEFAULT_FOREIGN_KEYS=1 /D "SQLITE_API=__declspec(dllexport)" /D SQLITE_WIN32_FILEMAPPING_API=1
mkdir obj
mkdir bin

mkdir obj\v110_xp
mkdir bin\v110_xp

mkdir obj\v110_xp\x86
mkdir bin\v110_xp\x86
cmd /c v110_xp_x86.bat

mkdir obj\v110_xp\x64
mkdir bin\v110_xp\x64
cmd /c v110_xp_x64.bat

mkdir obj\v140
mkdir bin\v140

mkdir obj\v140\x86
mkdir bin\v140\x86
cmd /c v140_x86.bat

mkdir obj\v140\x64
mkdir bin\v140\x64
cmd /c v140_x64.bat

mkdir obj\v140\arm
mkdir bin\v140\arm
cmd /c v140_arm.bat

