@echo off
SETLOCAL ENABLEEXTENSIONS
if EXIST x86 (
	if EXIST x64 (
		echo Native libraries already built
		exit /b
	)
)

IF NOT DEFINED VCINSTALLDIR (
	echo This script must be run from a Visual Studio command prompt
	exit /b 1
)

if NOT EXIST jni\tomcrypt (
	git clone https://github.com/libtom/libtomcrypt jni\tomcrypt
	git checkout bd7933cc2b43ebe7c4349614c6cf1271251ebee4
)

call "%VCINSTALLDIR%vcvarsall" x86
mkdir x86
mkdir x64
set OUT_DIR=x86
:build
set OUT_FILE=%OUT_DIR%\packaged_sqlcipher.dll
cl /DLTC_NO_FAST ^
/DSQLCIPHER_CRYPTO_LIBTOMCRYPT ^
/DSQLITE_HAS_CODEC ^
/DSQLITE_TEMP_STORE=2 ^
/DSQLITE_DEFAULT_FOREIGN_KEYS=1 ^
/DSQLITE_ENABLE_FTS4 ^
/DSQLITE_ENABLE_FTS3_PARENTHESIS ^
/DSQLITE_ENABLE_COLUMN_METADATA ^
/Ijni\tomcrypt\src\headers ^
sqlite3.c ^
jni\tomcrypt\src\ciphers\aes\aes.c ^
jni\tomcrypt\src\hashes\sha1.c ^
jni\tomcrypt\src\hashes\helper\hash_memory.c ^
jni\tomcrypt\src\hashes\sha2\sha256.c ^
jni\tomcrypt\src\modes\cbc\cbc_encrypt.c ^
jni\tomcrypt\src\modes\cbc\cbc_decrypt.c ^
jni\tomcrypt\src\modes\cbc\cbc_start.c ^
jni\tomcrypt\src\modes\cbc\cbc_done.c ^
jni\tomcrypt\src\mac\hmac\hmac_init.c ^
jni\tomcrypt\src\mac\hmac\hmac_memory.c ^
jni\tomcrypt\src\mac\hmac\hmac_process.c ^
jni\tomcrypt\src\mac\hmac\hmac_done.c ^
jni\tomcrypt\src\misc\zeromem.c ^
jni\tomcrypt\src\misc\crypt\crypt_find_cipher.c ^
jni\tomcrypt\src\misc\crypt\crypt_find_hash.c ^
jni\tomcrypt\src\misc\crypt\crypt_register_hash.c ^
jni\tomcrypt\src\misc\crypt\crypt_register_cipher.c ^
jni\tomcrypt\src\misc\crypt\crypt_register_prng.c ^
jni\tomcrypt\src\misc\crypt\crypt_argchk.c ^
jni\tomcrypt\src\misc\crypt\crypt_cipher_is_valid.c ^
jni\tomcrypt\src\misc\crypt\crypt_cipher_descriptor.c ^
jni\tomcrypt\src\misc\crypt\crypt_prng_descriptor.c ^
jni\tomcrypt\src\misc\crypt\crypt_hash_is_valid.c ^
jni\tomcrypt\src\misc\crypt\crypt_hash_descriptor.c ^
jni\tomcrypt\src\misc\pkcs5\pkcs_5_2.c ^
jni\tomcrypt\src\prngs\fortuna.c ^
/LD ^
/Fe%OUT_FILE% ^
/link ^
/DEF:sqlite3.def

del *.obj
IF "%OUT_DIR%" == "x64" goto:eof
set OUT_DIR=x64
call "%VCINSTALLDIR%vcvarsall" x86_amd64
goto build