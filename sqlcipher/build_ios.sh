#!/bin/sh

DIRNAME=`dirname $0`
SDKROOT=`xcrun -sdk iphoneos --show-sdk-path | tail -n1`
if [ -e $DIRNAME/packaged_sqlcipher.a ]
then
echo "Native library already built"
  exit 0
fi

clang -x c -arch arm64 -arch armv7 -fmessage-length=0 -fdiagnostics-show-note-include-stack -fmacro-backtrace-limit=0 -std=c99 -Wno-trigraphs -fpascal-strings -Os -Wno-missing-field-initializers -Wno-missing-prototypes -Wno-missing-braces -Wparentheses -Wswitch -Wno-unused-function -Wno-unused-label -Wno-unused-parameter -Wunused-variable -Wunused-value -Wno-empty-body -Wno-uninitialized -Wno-unknown-pragmas -Wno-shadow -Wno-four-char-constants -Wno-conversion -Wno-constant-conversion -Wno-int-conversion -Wno-bool-conversion -Wno-enum-conversion -Wshorten-64-to-32 -Wpointer-sign -Wno-newline-eof -isysroot $SDKROOT -fstrict-aliasing -Wdeprecated-declarations -miphoneos-version-min=5.1.1 -g -Wno-sign-conversion -DSQLCIPHER_CRYPTO_CC -DSQLITE_HAS_CODEC -DSQLITE_TEMP_STORE=2 -DSQLITE_DEFAULT_FOREIGN_KEYS=1 -DSQLITE_ENABLE_FTS4 -DSQLITE_ENABLE_FTS3_PARENTHESIS -DSQLITE_ENABLE_COLUMN_METADATA -MMD -MT dependencies -framework Security -c $DIRNAME/sqlite3.c -o $DIRNAME/packaged_sqlcipher.o
ar rcs packaged_sqlcipher.a packaged_sqlcipher.o

rm -rf $DIRNAME/*.d $DIRNAME/*.dSYM packaged_sqlcipher.o


