#!/bin/sh
rm -f xam.zip
zip xam.zip bin/android/Release/SQLitePCL.dll bin/android/*/Release/SQLitePCL.dll bin/ios/*/Release/SQLitePCL.dll
ls -l xam.zip

