@echo off
SETLOCAL ENABLEEXTENSIONS
set ERR_CODE=0
if EXIST %~dp0libs (
	echo Native libraries already built
	exit /b
)

if NOT EXIST jni\tomcrypt (
	git clone https://github.com/libtom/libtomcrypt jni\tomcrypt
	git checkout bd7933cc2b43ebe7c4349614c6cf1271251ebee4
)

if NOT DEFINED TMP_DRIVE_LETTER (
	echo Please specify an available temporary drive letter (such as X for X:\) as TMP_DRIVE_LETTER
	exit /b 1
)

if NOT DEFINED NDK_ROOT (
	echo Please specify the path to the Android NDK as NDK_ROOT
	exit /b 2
)

if NOT EXIST %NDK_ROOT%\ndk-build.cmd (
	echo ndk-build not found at %NDK_ROOT%
	exit /b 3
)

rem This hilarious dance ensures that the ndk-build script does not exceed the 260
rem character maximum file path length on Windows.  However, I don't know what drive letters
rem the user has free so I ask them to specify one
set CWD=%~dp0
set CWD=%CWD:~0,-1%
subst %TMP_DRIVE_LETTER%: %CWD%
pushd %TMP_DRIVE_LETTER%:\jni
call %NDK_ROOT%\ndk-build.cmd %1
popd
subst %TMP_DRIVE_LETTER%: /d
rmdir /s /q obj