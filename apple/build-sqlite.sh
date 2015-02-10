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

mkdir -p ./$Z_SQL/obj/ios/i386
mkdir -p ./$Z_SQL/obj/ios/x86_64
mkdir -p ./$Z_SQL/obj/ios/armv7
mkdir -p ./$Z_SQL/obj/ios/armv7s
mkdir -p ./$Z_SQL/obj/ios/arm64
mkdir -p ./libs/ios

mkdir -p ./$Z_SQL/obj/mac/i386
mkdir -p ./$Z_SQL/obj/mac/x86_64
mkdir -p ./libs/mac

if [ -d /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator8.1.sdk ]; then
	IOS_SIM_ROOT=/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator8.1.sdk
elif [ -d /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator7.1.sdk ]; then
	IOS_SIM_ROOT=/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator7.1.sdk
elif [ -d /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator7.0.sdk ]; then
	IOS_SIM_ROOT=/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator7.0.sdk
else
	IOS_SIM_ROOT=/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator6.1.sdk
fi

if [ -d /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.1.sdk ]; then
	IOS_SDK_ROOT=/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.1.sdk
elif [ -d /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS7.1.sdk ]; then
	IOS_SDK_ROOT=/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS7.1.sdk
elif [ -d /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS7.0.sdk ]; then
	IOS_SDK_ROOT=/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS7.0.sdk
else
	IOS_SDK_ROOT=/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS6.1.sdk
fi

Z_CFLAGS="-O -DNDEBUG -DSQLITE_DEFAULT_FOREIGN_KEYS=1 -DSQLITE_ENABLE_FTS3_PARENTHESIS -DSQLITE_ENABLE_FTS4 -DSQLITE_ENABLE_COLUMN_METADATA"

xcrun clang $Z_CODEC_ARGS -arch i386 $Z_CFLAGS -c -o ./$Z_SQL/obj/mac/i386/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch x86_64 $Z_CFLAGS -c -o ./$Z_SQL/obj/mac/x86_64/sqlite3.c.o ../$Z_SQL/sqlite3.c

libtool -static -o ./libs/mac/packaged_$Z_SQL.a \
	./$Z_SQL/obj/mac/i386/sqlite3.c.o \
	./$Z_SQL/obj/mac/x86_64/sqlite3.c.o

if [ "$Z_SQLCIPHER" == "1" ]; then
    echo "TODO sqlcipher dylib"
else
    xcrun clang -dynamiclib $Z_CODEC_ARGS -arch i386 -arch x86_64 $Z_CFLAGS -o ./libs/mac/libpackaged_sqlite3.dylib ../$Z_SQL/sqlite3.c
fi

xcrun clang $Z_CODEC_ARGS -arch i386 -isysroot $IOS_SIM_ROOT $Z_CFLAGS -c -o ./$Z_SQL/obj/ios/i386/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch x86_64 -isysroot $IOS_SIM_ROOT $Z_CFLAGS -c -o ./$Z_SQL/obj/ios/x86_64/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch arm64 -isysroot $IOS_SDK_ROOT $Z_CFLAGS -c -o ./$Z_SQL/obj/ios/arm64/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch armv7 -isysroot $IOS_SDK_ROOT $Z_CFLAGS -c -o ./$Z_SQL/obj/ios/armv7/sqlite3.c.o ../$Z_SQL/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch armv7s -isysroot $IOS_SDK_ROOT $Z_CFLAGS -c -o ./$Z_SQL/obj/ios/armv7s/sqlite3.c.o ../$Z_SQL/sqlite3.c

libtool -static -o ./libs/ios/packaged_$Z_SQL.a \
	./$Z_SQL/obj/ios/i386/sqlite3.c.o \
	./$Z_SQL/obj/ios/x86_64/sqlite3.c.o \
	./$Z_SQL/obj/ios/armv7/sqlite3.c.o \
	./$Z_SQL/obj/ios/armv7s/sqlite3.c.o \
	./$Z_SQL/obj/ios/arm64/sqlite3.c.o

echo ----------------------------------------------------------------
echo build-sqlite.sh done

