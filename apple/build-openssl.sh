#!/bin/bash

# This script builds the iOS and Mac openSSL libraries
# Download openssl http://www.openssl.org/source/ and place the tarball next to this script

# Credits:
# https://github.com/st3fan/ios-openssl
# https://github.com/x2on/OpenSSL-for-iPhone/blob/master/build-libssl.sh
#
# https://gist.github.com/foozmeat/5154962


set -e

usage ()
{
    echo "usage: $0 [minimum iOS SDK version (default 8.1)]"
    exit 127
}

if [ $1 -e "-h" ]; then
    usage
fi

if [ -z $1 ]; then
    SDK_VERSION="8.1"
else
    SDK_VERSION=$1
fi

OPENSSL_VERSION="openssl-1.0.1p"
DEVELOPER=`xcode-select -print-path`

OPENSSL_WATCHOS_VERSION="openssl-1.0.2h"
WATCHOS_MIN_VERSION="3.0"

# TODO
#EXCLUDES=" no-idea no-camellia no-ec "

# TODO make build_crypto, but then make install messes it up

buildMac()
{
    ARCH=$1

    echo "Building ${OPENSSL_VERSION} for ${ARCH}"

    TARGET="darwin-i386-cc"

    if [[ $ARCH == "x86_64" ]]; then
        TARGET="darwin64-x86_64-cc"
    fi

    pushd . > /dev/null
    cd "${OPENSSL_VERSION}"
    ./Configure ${TARGET} ${EXCLUDES} --openssldir="/tmp/${OPENSSL_VERSION}-${ARCH}" &> "/tmp/${OPENSSL_VERSION}-${ARCH}.log"
    make >> "/tmp/${OPENSSL_VERSION}-${ARCH}.log" 2>&1
    make install >> "/tmp/${OPENSSL_VERSION}-${ARCH}.log" 2>&1
    make clean >> "/tmp/${OPENSSL_VERSION}-${ARCH}.log" 2>&1
    popd > /dev/null
}

buildIOS()
{
    ARCH=$1

    pushd . > /dev/null
    cd "${OPENSSL_VERSION}"
  
    if [[ "${ARCH}" == "i386" || "${ARCH}" == "x86_64" ]]; then
        PLATFORM="iPhoneSimulator"
    else
        PLATFORM="iPhoneOS"
        sed -ie "s!static volatile sig_atomic_t intr_signal;!static volatile intr_signal;!" "crypto/ui/ui_openssl.c"
    fi
  
    export $PLATFORM
    export CROSS_TOP="${DEVELOPER}/Platforms/${PLATFORM}.platform/Developer"
    export CROSS_SDK="${PLATFORM}${SDK_VERSION}.sdk"
    export BUILD_TOOLS="${DEVELOPER}"
    export CC="${BUILD_TOOLS}/usr/bin/gcc -arch ${ARCH}"
   
    echo "Building ${OPENSSL_VERSION} for ${PLATFORM} ${SDK_VERSION} ${ARCH}"

    if [[ "${ARCH}" == "x86_64" ]]; then
        ./Configure darwin64-x86_64-cc ${EXCLUDES} --openssldir="/tmp/${OPENSSL_VERSION}-iOS-${ARCH}" &> "/tmp/${OPENSSL_VERSION}-iOS-${ARCH}.log"
    else
        ./Configure iphoneos-cross ${EXCLUDES} --openssldir="/tmp/${OPENSSL_VERSION}-iOS-${ARCH}" &> "/tmp/${OPENSSL_VERSION}-iOS-${ARCH}.log"
    fi
    # add -isysroot to CC=
    sed -ie "s!^CFLAG=!CFLAG=-isysroot ${CROSS_TOP}/SDKs/${CROSS_SDK} -miphoneos-version-min=${SDK_VERSION} !" "Makefile"

    make >> "/tmp/${OPENSSL_VERSION}-iOS-${ARCH}.log" 2>&1
    make install >> "/tmp/${OPENSSL_VERSION}-iOS-${ARCH}.log" 2>&1
    make clean >> "/tmp/${OPENSSL_VERSION}-iOS-${ARCH}.log" 2>&1
    popd > /dev/null
}

buildWatchOS()
{
    ARCH=$1

    pushd . > /dev/null
    cd "${OPENSSL_WATCHOS_VERSION}"

    if [[ "${ARCH}" == "i386" ]]; then
        PLATFORM="WatchSimulator"
    else
        PLATFORM="WatchOS"
        sed -ie "s!static volatile sig_atomic_t intr_signal;!static volatile intr_signal;!" "crypto/ui/ui_openssl.c"
    fi

    # No fork on watchOS
    sed -ie "s!define HAVE_FORK 1!define HAVE_FORK 0!" "apps/speed.c"
    export $PLATFORM
    export CROSS_TOP="${DEVELOPER}/Platforms/${PLATFORM}.platform/Developer"
    export CROSS_SDK="${PLATFORM}3.0.sdk"
    export BUILD_TOOLS="${DEVELOPER}"
    export CC="${BUILD_TOOLS}/usr/bin/gcc -arch ${ARCH}"

    echo "Building ${OPENSSL_WATCHOS_VERSION} for ${PLATFORM} ${SDK_VERSION} ${ARCH}"

    ./Configure no-asm iphoneos-cross ${EXCLUDES} --openssldir="/tmp/${OPENSSL_WATCHOS_VERSION}-watchOS-${ARCH}" &> "/tmp/${OPENSSL_WATCHOS_VERSION}-watchOS-${ARCH}.log"

    if [[ "${ARCH}" == "i386" ]]; then
        sed -ie "s!^CFLAG=!CFLAG=-isysroot ${CROSS_TOP}/SDKs/${CROSS_SDK} -mwatchos-simulator-version-min=${WATCHOS_MIN_VERSION} !" "Makefile"
    else
        sed -ie "s!^CFLAG=!CFLAG=-isysroot ${CROSS_TOP}/SDKs/${CROSS_SDK} -mwatchos-version-min=${WATCHOS_MIN_VERSION} -fembed-bitcode !" "Makefile"
    fi

    make clean >> "/tmp/${OPENSSL_WATCHOS_VERSION}-watchOS-${ARCH}.log" 2>&1
    make depend >> "/tmp/${OPENSSL_WATCHOS_VERSION}-watchOS-${ARCH}.log" 2>&1
    make >> "/tmp/${OPENSSL_WATCHOS_VERSION}-watchOS-${ARCH}.log" 2>&1
    make install >> "/tmp/${OPENSSL_WATCHOS_VERSION}-watchOS-${ARCH}.log" 2>&1
    make clean >> "/tmp/${OPENSSL_WATCHOS_VERSION}-watchOS-${ARCH}.log" 2>&1
    popd > /dev/null
}

echo "Cleaning up"
rm -rf include/openssl/* libs/ios/libcrypto* libs/mac/libcrypto* libs/watchos/libcrypto*

mkdir -p libs/ios
mkdir -p libs/mac
mkdir -p libs/watchos
mkdir -p include/openssl/

rm -rf "/tmp/${OPENSSL_VERSION}-*"
rm -rf "/tmp/${OPENSSL_VERSION}-*.log"

rm -rf "${OPENSSL_VERSION}"

echo "Unpacking openssl"
tar xfz "../tarballs/${OPENSSL_VERSION}.tar.gz"

buildMac "i386"
buildMac "x86_64"

echo "Copying headers"
cp /tmp/${OPENSSL_VERSION}-i386/include/openssl/* include/openssl/

echo "Building Mac libraries"
lipo \
    "/tmp/${OPENSSL_VERSION}-i386/lib/libcrypto.a" \
    "/tmp/${OPENSSL_VERSION}-x86_64/lib/libcrypto.a" \
    -create -output libs/mac/libcrypto.a

#lipo \
    #"/tmp/${OPENSSL_VERSION}-i386/lib/libssl.a" \
    #"/tmp/${OPENSSL_VERSION}-x86_64/lib/libssl.a" \
    #-create -output libs/mac/libssl.a

buildIOS "armv7"
buildIOS "armv7s"
if [[ $SDK_VERSION == "8.1" ]]; then
    buildIOS "arm64"
    buildIOS "x86_64"
fi
buildIOS "i386"

echo "Building iOS libraries"
lipo \
    "/tmp/${OPENSSL_VERSION}-iOS-armv7/lib/libcrypto.a" \
    "/tmp/${OPENSSL_VERSION}-iOS-armv7s/lib/libcrypto.a" \
    "/tmp/${OPENSSL_VERSION}-iOS-i386/lib/libcrypto.a" \
    -create -output libs/ios/libcrypto.a

#lipo \
    #"/tmp/${OPENSSL_VERSION}-iOS-armv7/lib/libssl.a" \
    #"/tmp/${OPENSSL_VERSION}-iOS-armv7s/lib/libssl.a" \
    #"/tmp/${OPENSSL_VERSION}-iOS-i386/lib/libssl.a" \
    #-create -output libs/ios/libssl.a

if [[ $SDK_VERSION == "8.1" ]]; then
    echo "Adding 64-bit libraries"
    lipo \
        "libs/ios/libcrypto.a" \
        "/tmp/${OPENSSL_VERSION}-iOS-arm64/lib/libcrypto.a" \
        "/tmp/${OPENSSL_VERSION}-iOS-x86_64/lib/libcrypto.a" \
        -create -output libs/ios/libcrypto.a

    #lipo \
        #"libs/ios/libssl.a" \
        #"/tmp/${OPENSSL_VERSION}-iOS-arm64/lib/libssl.a" \
        #"/tmp/${OPENSSL_VERSION}-iOS-x86_64/lib/libssl.a" \
        #-create -output libs/ios/libssl.a

fi

# WatchOS libcrypto.a
echo "Unpacking OpenSSL ${OPENSSL_WATCHOS_VERSION}"
tar xfz "../tarballs/${OPENSSL_WATCHOS_VERSION}.tar.gz"

buildWatchOS "armv7k"
buildWatchOS "i386"

echo "Building WatchOS libraries"
lipo \
    "/tmp/${OPENSSL_WATCHOS_VERSION}-watchOS-armv7k/lib/libcrypto.a" \
    "/tmp/${OPENSSL_WATCHOS_VERSION}-watchOS-i386/lib/libcrypto.a" \
    -create -output libs/watchos/libcrypto.a

echo "Cleaning up"
rm -rf /tmp/${OPENSSL_VERSION}-*
rm -rf ${OPENSSL_VERSION}
rm -rf /tmp/${OPENSSL_WATCHOS_VERSION}-*
rm -rf ${OPENSSL_WATCHOS_VERSION}

echo "Done"

