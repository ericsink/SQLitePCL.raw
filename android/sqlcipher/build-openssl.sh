#!/bin/sh

set -x

./export-toolchain.sh

TARBALLS=../../tarballs
ORIGPATH="$PATH"
export PATH="$ORIGPATH:$(pwd)/bin/x86-4.6/bin"

# Setup paths to stuff we need

OPENSSL_VERSION="1.0.1p"
INSTALL_DIR="`pwd`/installed/x86"

export AR=i686-linux-android-ar
export CC=i686-linux-android-gcc
export RANLIB=i686-linux-android-ranlib

rm -rf "openssl-${OPENSSL_VERSION}"
tar xfz "${TARBALLS}/openssl-${OPENSSL_VERSION}.tar.gz"
pushd .
cd "openssl-${OPENSSL_VERSION}"
./Configure android-x86 --prefix="${INSTALL_DIR}"
make
make install
popd
rm -rf "openssl-${OPENSSL_VERSION}"
rm -rf ${INSTALL_DIR}/ssl/man

export PATH="$ORIGPATH:$(pwd)/bin/arm-linux-androideabi-4.6/bin"
INSTALL_DIR="`pwd`/installed/armeabi"

export AR=arm-linux-androideabi-ar
export CC=arm-linux-androideabi-gcc
export RANLIB=arm-linux-androideabi-ranlib

rm -rf "openssl-${OPENSSL_VERSION}"
tar xfz "${TARBALLS}/openssl-${OPENSSL_VERSION}.tar.gz"
pushd .
cd "openssl-${OPENSSL_VERSION}"
./Configure android --prefix="${INSTALL_DIR}"
make
make install
popd
rm -rf "openssl-${OPENSSL_VERSION}"
rm -rf ${INSTALL_DIR}/ssl/man


export PATH="$ORIGPATH:$(pwd)/bin/arm-linux-androideabi-4.6/bin"
INSTALL_DIR="`pwd`/installed/armeabi-v7a"

export AR=arm-linux-androideabi-ar
export CC=arm-linux-androideabi-gcc
export RANLIB=arm-linux-androideabi-ranlib

rm -rf "openssl-${OPENSSL_VERSION}"
tar xfz "${TARBALLS}/openssl-${OPENSSL_VERSION}.tar.gz"
pushd .
cd "openssl-${OPENSSL_VERSION}"
./Configure android-armv7 --prefix="${INSTALL_DIR}"
make
make install
popd
rm -rf "openssl-${OPENSSL_VERSION}"
rm -rf ${INSTALL_DIR}/ssl/man


export PATH="$ORIGPATH:$(pwd)/bin/aarch64-linux-android-4.9/bin"
INSTALL_DIR="`pwd`/installed/arm64-v8a"

export AR=aarch64-linux-android-ar
export CC=aarch64-linux-android-gcc
export RANLIB=aarch64-linux-android-ranlib

rm -rf "openssl-${OPENSSL_VERSION}"
tar xfz "${TARBALLS}/openssl-${OPENSSL_VERSION}.tar.gz"
cp Configure.supports64 openssl-${OPENSSL_VERSION}/Configure
pushd .
cd "openssl-${OPENSSL_VERSION}"
./Configure android-arm64 --prefix="${INSTALL_DIR}"
make
make install
popd
rm -rf "openssl-${OPENSSL_VERSION}"
rm -rf ${INSTALL_DIR}/ssl/man


export PATH="$ORIGPATH:$(pwd)/bin/x86_64-4.9/bin"
INSTALL_DIR="`pwd`/installed/x86_64"

export AR=x86_64-linux-android-ar
export CC=x86_64-linux-android-gcc
export RANLIB=x86_64-linux-android-ranlib

rm -rf "openssl-${OPENSSL_VERSION}"
tar xfz "${TARBALLS}/openssl-${OPENSSL_VERSION}.tar.gz"
cp Configure.supports64 openssl-${OPENSSL_VERSION}/Configure
pushd .
cd "openssl-${OPENSSL_VERSION}"
./Configure android-x86_64 --prefix="${INSTALL_DIR}"
make
make install
popd
rm -rf "openssl-${OPENSSL_VERSION}"
rm -rf ${INSTALL_DIR}/ssl/man
