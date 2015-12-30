#!/bin/bash

DIRNAME=`dirname $0`
echo $DIRNAME
pushd "$DIRNAME"

if [ -e $DIRNAME/libpackaged_sqlcipher.so ] 
then
  echo "Native library already built"
  exit 0
fi

make
rm `find . -name "*.d"` `find . -name "*.o"`

popd
