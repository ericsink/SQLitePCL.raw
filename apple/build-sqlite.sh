#!/bin/bash

# end this script if any command fails
set -e
# echo commands as processed
set -x

# These commands attempt to confirm that this script is running inside
# the right directory.

cat ./build-sqlite.sh > /dev/null

if [ "$Z_SQLCIPHER" == "1" ]; then
    Z_SQL=sqlcipher
    Z_CODEC_ARGS="-DSQLITE_HAS_CODEC -I./include"
else
    Z_SQL=sqlite3
fi

Z_CFLAGS="-O -DNDEBUG -DSQLITE_DEFAULT_FOREIGN_KEYS=1 -DSQLITE_ENABLE_FTS3_PARENTHESIS -DSQLITE_ENABLE_FTS4 -DSQLITE_ENABLE_COLUMN_METADATA -DSQLITE_ENABLE_JSON1"

mkdir -p ./obj/ios/$Z_SQL/i386
mkdir -p ./obj/ios/$Z_SQL/x86_64
mkdir -p ./obj/ios/$Z_SQL/armv7
mkdir -p ./obj/ios/$Z_SQL/armv7s
mkdir -p ./obj/ios/$Z_SQL/arm64
mkdir -p ./libs/ios/$Z_SQL

mkdir -p ./obj/mac/$Z_SQL/i386
mkdir -p ./obj/mac/$Z_SQL/x86_64
mkdir -p ./libs/mac/$Z_SQL

xcrun --sdk macosx clang -arch i386 $Z_CFLAGS $Z_CODEC_ARGS -c -o ./obj/mac/$Z_SQL/i386/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun --sdk macosx clang -arch x86_64 $Z_CFLAGS $Z_CODEC_ARGS -c -o ./obj/mac/$Z_SQL/x86_64/sqlite3.c.o ../$Z_SQL/sqlite3.c

libtool -static -o ./libs/mac/$Z_SQL/esqlite3.a \
	./obj/mac/$Z_SQL/i386/sqlite3.c.o \
	./obj/mac/$Z_SQL/x86_64/sqlite3.c.o

xcrun --sdk macosx clang -dynamiclib -arch i386 -arch x86_64 $Z_CFLAGS $Z_CODEC_ARGS -o ./libs/mac/$Z_SQL/libesqlite3.dylib ../$Z_SQL/sqlite3.c ./libs/mac/libcrypto.a

xcrun --sdk iphonesimulator clang -arch i386 $Z_CFLAGS $Z_CODEC_ARGS -c -o ./obj/ios/$Z_SQL/i386/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun --sdk iphonesimulator clang -arch x86_64 $Z_CFLAGS $Z_CODEC_ARGS -c -o ./obj/ios/$Z_SQL/x86_64/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun --sdk iphoneos clang -arch arm64 $Z_CFLAGS $Z_CODEC_ARGS -c -o ./obj/ios/$Z_SQL/arm64/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun --sdk iphoneos clang -arch armv7 $Z_CFLAGS $Z_CODEC_ARGS -c -o ./obj/ios/$Z_SQL/armv7/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun --sdk iphoneos clang -arch armv7s $Z_CFLAGS $Z_CODEC_ARGS  -c -o ./obj/ios/$Z_SQL/armv7s/sqlite3.c.o ../$Z_SQL/sqlite3.c

libtool -static -o ./libs/ios/$Z_SQL/esqlite3.a \
	./obj/ios/$Z_SQL/i386/sqlite3.c.o \
	./obj/ios/$Z_SQL/x86_64/sqlite3.c.o \
	./obj/ios/$Z_SQL/armv7/sqlite3.c.o \
	./obj/ios/$Z_SQL/armv7s/sqlite3.c.o \
	./obj/ios/$Z_SQL/arm64/sqlite3.c.o

xcrun --sdk iphoneos clang -dynamiclib $Z_CFLAGS $Z_CODEC_ARGS ../$Z_SQL/sqlite3.c -arch armv7 -arch armv7s -arch arm64 -o ./obj/ios/$Z_SQL/esqlite3.device -framework Foundation -fapplication-extension ./libs/ios/libcrypto.a
install_name_tool -id @rpath/esqlite3.framework/esqlite3  ./obj/ios/$Z_SQL/esqlite3.device

xcrun --sdk iphonesimulator clang -dynamiclib $Z_CFLAGS $Z_CODEC_ARGS ../$Z_SQL/sqlite3.c -arch i386 -arch x86_64 -o ./obj/ios/$Z_SQL/esqlite3.simulator -framework Foundation -fapplication-extension -mios-simulator-version-min=8.0 ./libs/ios/libcrypto.a
install_name_tool -id @rpath/esqlite3.framework/esqlite3  ./obj/ios/$Z_SQL/esqlite3.simulator

lipo -create  ./obj/ios/$Z_SQL/esqlite3.device  ./obj/ios/$Z_SQL/esqlite3.simulator -output  ./libs/ios/$Z_SQL/framework_esqlite3

echo ----------------------------------------------------------------
echo build-sqlite.sh done

