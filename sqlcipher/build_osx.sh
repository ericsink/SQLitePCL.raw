#!/bin/sh

DIRNAME=`dirname $0`
SDKROOT=`xcrun -sdk macosx --show-sdk-path | tail -n1`
if [ -e $DIRNAME/libpackaged_sqlcipher.dylib ]
then
  echo "Native library already built"
  exit 0
fi

clang -arch i386 -fmessage-length=0 -fdiagnostics-show-note-include-stack -fmacro-backtrace-limit=0 -std=c99 -Wno-trigraphs -fpascal-strings -Os -Wno-missing-field-initializers -Wno-missing-prototypes -Wno-missing-braces -Wparentheses -Wswitch -Wno-unused-function -Wno-unused-label -Wno-unused-parameter -Wunused-variable -Wunused-value -Wno-empty-body -Wno-uninitialized -Wno-unknown-pragmas -Wno-shadow -Wno-four-char-constants -Wno-conversion -Wno-constant-conversion -Wno-int-conversion -Wno-bool-conversion -Wno-enum-conversion -Wno-shorten-64-to-32 -Wpointer-sign -Wno-newline-eof -isysroot $SDKROOT -fasm-blocks -fstrict-aliasing -Wdeprecated-declarations -mmacosx-version-min=10.9 -g -Wno-sign-conversion -DSQLCIPHER_CRYPTO_CC -DSQLITE_HAS_CODEC -DSQLITE_TEMP_STORE=2 -DSQLITE_DEFAULT_FOREIGN_KEYS=1 -DSQLITE_ENABLE_FTS4 -DSQLITE_ENABLE_FTS3_PARENTHESIS -DSQLITE_ENABLE_COLUMN_METADATA -MMD -MT dependencies -dynamiclib -framework Security $DIRNAME/sqlite3.c -o $DIRNAME/libpackaged_sqlcipherx86.dylib &
clang -arch x86_64 -fmessage-length=0 -fdiagnostics-show-note-include-stack -fmacro-backtrace-limit=0 -std=c99 -Wno-trigraphs -fpascal-strings -Os -Wno-missing-field-initializers -Wno-missing-prototypes -Wno-missing-braces -Wparentheses -Wswitch -Wno-unused-function -Wno-unused-label -Wno-unused-parameter -Wunused-variable -Wunused-value -Wno-empty-body -Wno-uninitialized -Wno-unknown-pragmas -Wno-shadow -Wno-four-char-constants -Wno-conversion -Wno-constant-conversion -Wno-int-conversion -Wno-bool-conversion -Wno-enum-conversion -Wno-shorten-64-to-32 -Wpointer-sign -Wno-newline-eof -isysroot $SDKROOT -fasm-blocks -fstrict-aliasing -Wdeprecated-declarations -mmacosx-version-min=10.9 -g -Wno-sign-conversion -DSQLCIPHER_CRYPTO_CC -DSQLITE_HAS_CODEC -DSQLITE_TEMP_STORE=2 -DSQLITE_DEFAULT_FOREIGN_KEYS=1 -DSQLITE_ENABLE_FTS4 -DSQLITE_ENABLE_FTS3_PARENTHESIS -DSQLITE_ENABLE_COLUMN_METADATA -MMD -MT dependencies -dynamiclib -framework Security $DIRNAME/sqlite3.c -o $DIRNAME/libpackaged_sqlcipherx64.dylib &
wait %1 %2

lipo -create $DIRNAME/libpackaged_sqlcipherx86.dylib $DIRNAME/libpackaged_sqlcipherx64.dylib -output $DIRNAME/libpackaged_sqlcipher.dylib
rm -rf $DIRNAME/libpackaged_sqlcipherx86.dylib $DIRNAME/libpackaged_sqlcipherx64.dylib $DIRNAME/*.d $DIRNAME/*.dSYM

