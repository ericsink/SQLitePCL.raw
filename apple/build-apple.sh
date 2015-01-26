#!/bin/bash

# end this script if any command fails
set -e
# echo commands as processed
set -x

# These commands attempt to confirm that this script is running inside
# the right directory.

cat ./build-apple.sh > /dev/null

if [ "$Z_SQLCIPHER" == "1" ]; then
    Z_SQLDIR=../sqlcipher
    Z_CODEC_ARGS="-DSQLITE_HAS_CODEC -I../openssl/ios/include"
else
    Z_SQLDIR=../sqlite3
fi

mkdir -p ./obj/ios/i386
mkdir -p ./obj/ios/x86_64
mkdir -p ./obj/ios/armv7
mkdir -p ./obj/ios/armv7s
mkdir -p ./obj/ios/arm64
mkdir -p ./lib/ios

mkdir -p ./obj/mac/i386
mkdir -p ./obj/mac/x86_64
mkdir -p ./lib/mac

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

xcrun clang $Z_CODEC_ARGS -arch i386 $Z_CFLAGS -c -o ./obj/mac/i386/sqlite3.c.o $Z_SQLDIR/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch x86_64 $Z_CFLAGS -c -o ./obj/mac/x86_64/sqlite3.c.o $Z_SQLDIR/sqlite3.c

libtool -static -o ./lib/mac/sqlite3.a \
	./obj/mac/i386/sqlite3.c.o \
	./obj/mac/x86_64/sqlite3.c.o

xcrun clang $Z_CODEC_ARGS -arch i386 -isysroot $IOS_SIM_ROOT $Z_CFLAGS -c -o ./obj/ios/i386/sqlite3.c.o $Z_SQLDIR/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch x86_64 -isysroot $IOS_SIM_ROOT $Z_CFLAGS -c -o ./obj/ios/x86_64/sqlite3.c.o $Z_SQLDIR/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch arm64 -isysroot $IOS_SDK_ROOT $Z_CFLAGS -c -o ./obj/ios/arm64/sqlite3.c.o $Z_SQLDIR/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch armv7 -isysroot $IOS_SDK_ROOT $Z_CFLAGS -c -o ./obj/ios/armv7/sqlite3.c.o $Z_SQLDIR/sqlite3.c
xcrun clang $Z_CODEC_ARGS -arch armv7s -isysroot $IOS_SDK_ROOT $Z_CFLAGS -c -o ./obj/ios/armv7s/sqlite3.c.o $Z_SQLDIR/sqlite3.c

libtool -static -o ./lib/ios/sqlite3.a \
	./obj/ios/i386/sqlite3.c.o \
	./obj/ios/x86_64/sqlite3.c.o \
	./obj/ios/armv7/sqlite3.c.o \
	./obj/ios/armv7s/sqlite3.c.o \
	./obj/ios/arm64/sqlite3.c.o

echo ----------------------------------------------------------------
echo build-apple.sh done

