#!/bin/sh
set -x
DEFS='-DNDEBUG -DSQLITE_DEFAULT_FOREIGN_KEYS=1 -DSQLITE_ENABLE_FTS3_PARENTHESIS -DSQLITE_ENABLE_FTS4 -DSQLITE_ENABLE_FTS5 -DSQLITE_ENABLE_COLUMN_METADATA -DSQLITE_ENABLE_JSON1 -DSQLITE_ENABLE_RTREE '
echo Building arm64
aarch64-linux-gnu-gcc -shared -fPIC -O $DEFS -o arm64/libe_sqlite3.so ../sqlite3/sqlite3.c 
echo Building armhf
arm-linux-gnueabihf-gcc -shared -fPIC -O $DEFS -o armhf/libe_sqlite3.so ../sqlite3/sqlite3.c 
echo Building armsf
arm-linux-gnueabi-gcc -shared -fPIC -O $DEFS-o armsf/libe_sqlite3.so ../sqlite3/sqlite3.c 
echo Building x64
gcc -m64 -shared -fPIC -O $DEFS-o x64/libe_sqlite3.so ../sqlite3/sqlite3.c 
echo Building x86
gcc -m32 -shared -fPIC -O $DEFS-o x86/libe_sqlite3.so ../sqlite3/sqlite3.c 
echo Building musl-x64
musl-gcc -m64 -shared -fPIC -O $DEFS-o musl-x64/libe_sqlite3.so ../sqlite3/sqlite3.c 
echo Done
# sudo apt-get install gcc-arm-linux-gnueabihf
# sudo apt-get install musl-dev musl-tools
# sudo apt-get install gcc-aarch64-linux-gnu
