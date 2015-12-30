#!/bin/sh


if [ -d `dirname $0`/libs ]
then
  echo "Native libraries already built"
  exit 0
fi

if [ ! -d jni/tomcrypt ]
then
	git clone https://github.com/libtom/libtomcrypt jni/tomcrypt
	git checkout bd7933cc2b43ebe7c4349614c6cf1271251ebee4
fi

if [ ! -d $HOME/Library/Developer/Xamarin/android-ndk ]
then
  echo "Android NDK not found in the default location!"
  popd
  exit 1
fi

LATEST_VERSION=`ls $HOME/Library/Developer/Xamarin/android-ndk/ | egrep -o "[0-9]+." | sort -g | tail -n1`
echo "Using Android NDK version ${LATEST_VERSION}"

NDK_ROOT="$HOME/Library/Developer/Xamarin/android-ndk/android-ndk-r${LATEST_VERSION}"
if [ ! -f $NDK_ROOT/ndk-build ]
then
  echo "ndk-build not found at $NDK_ROOT"
  popd
  exit 2
fi

pushd `dirname $0`/jni
$NDK_ROOT/ndk-build $1
popd
