#!/bin/sh
if which ndk-build > /dev/null
then
  NDK_LOC=$(which ndk-build)
  NDK_LOC=${NDK_LOC%/*}
  if [ ! -d bin/x86-4.6 ]
  then
  	$NDK_LOC/build/tools/make-standalone-toolchain.sh --platform=android-9 --toolchain=x86-4.6 --install-dir=./bin/x86-4.6
  fi
  if [ ! -d bin/x86_64-4.9 ]
  then
  	$NDK_LOC/build/tools/make-standalone-toolchain.sh --platform=android-21 --toolchain=x86_64-4.9 --install-dir=./bin/x86_64-4.9
  fi
  if [ ! -d bin/arm-linux-androideabi-4.6 ]
  then
  	$NDK_LOC/build/tools/make-standalone-toolchain.sh --platform=android-9 --toolchain=arm-linux-androideabi-4.6 --install-dir=./bin/arm-linux-androideabi-4.6
  fi
  if [ ! -d bin/aarch64-linux-android-4.9 ]
  then
  	$NDK_LOC/build/tools/make-standalone-toolchain.sh --platform=android-21 --toolchain=aarch64-linux-android-4.9 --install-dir=./bin/aarch64-linux-android-4.9
  fi
else
  echo "The Android NDK is not in the path!"
  return 1
fi
