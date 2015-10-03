LOCAL_PATH := $(call my-dir)

### Build libtomcrypt ###

include $(CLEAR_VARS)
LOCAL_MODULE := libtomcrypt
LOCAL_C_INCLUDES := $(LOCAL_PATH)/tomcrypt/src/headers
LIBTOMCRYPT_PATH := $(LOCAL_PATH)/tomcrypt
LOCAL_SRC_FILES := $(LIBTOMCRYPT_PATH)/src/ciphers/aes/aes.c \
$(LIBTOMCRYPT_PATH)/src/hashes/sha1.c \
$(LIBTOMCRYPT_PATH)/src/hashes/helper/hash_memory.c \
$(LIBTOMCRYPT_PATH)/src/hashes/sha2/sha256.c \
$(LIBTOMCRYPT_PATH)/src/modes/cbc/cbc_encrypt.c \
$(LIBTOMCRYPT_PATH)/src/modes/cbc/cbc_decrypt.c \
$(LIBTOMCRYPT_PATH)/src/modes/cbc/cbc_start.c \
$(LIBTOMCRYPT_PATH)/src/modes/cbc/cbc_done.c \
$(LIBTOMCRYPT_PATH)/src/mac/hmac/hmac_init.c \
$(LIBTOMCRYPT_PATH)/src/mac/hmac/hmac_memory.c \
$(LIBTOMCRYPT_PATH)/src/mac/hmac/hmac_process.c \
$(LIBTOMCRYPT_PATH)/src/mac/hmac/hmac_done.c \
$(LIBTOMCRYPT_PATH)/src/misc/zeromem.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_find_cipher.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_find_hash.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_register_hash.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_register_cipher.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_register_prng.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_argchk.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_cipher_is_valid.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_cipher_descriptor.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_prng_descriptor.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_hash_is_valid.c \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_hash_descriptor.c \
$(LIBTOMCRYPT_PATH)/src/misc/pkcs5/pkcs_5_2.c \
$(LIBTOMCRYPT_PATH)/src/prngs/fortuna.c
include $(BUILD_STATIC_LIBRARY)

## Build libsqlcipher ###

include $(CLEAR_VARS)

LOCAL_MODULE := packaged_sqlcipher

LOCAL_CFLAGS := -std=c99 \
	-fmessage-length=0 \
	-fdiagnostics-show-note-include-stack \
	-fmacro-backtrace-limit=0 \
	-Wno-trigraphs \
	-fpascal-strings \
	-Os \
	-Wno-missing-field-initializers \
	-Wno-missing-prototypes \
	-Wno-missing-braces \
	-Wparentheses \
	-Wswitch \
	-Wno-unused-function \
	-Wno-unused-label \
	-Wno-unused-parameter \
	-Wunused-variable \
	-Wunused-value \
	-Wno-empty-body \
	-Wno-uninitialized \
	-Wno-unknown-pragmas \
	-Wno-shadow \
	-Wno-four-char-constants \
	-Wno-conversion \
	-Wno-constant-conversion \
	-Wno-int-conversion \
	-Wno-bool-conversion \
	-Wno-enum-conversion \
	-Wshorten-64-to-32 \
	-Wpointer-sign \
	-Wno-newline-eof \
	-fstrict-aliasing \
	-Wdeprecated-declarations \
	-g \
	-Wno-sign-conversion \
	-DSQLCIPHER_CRYPTO_LIBTOMCRYPT \
	-DSQLITE_HAS_CODEC \
	-DSQLITE_TEMP_STORE=2 \
	-DSQLITE_DEFAULT_FOREIGN_KEYS=1 \
	-DSQLITE_ENABLE_FTS4 \
	-DSQLITE_ENABLE_FTS3_PARENTHESIS \
	-DSQLITE_ENABLE_COLUMN_METADATA \
	-MMD \
	-MT dependencies

LOCAL_SRC_FILES := $(LOCAL_PATH)/../sqlite3.c
LOCAL_STATIC_LIBRARIES := libtomcrypt
LOCAL_C_INCLUDES := $(LOCAL_PATH)/tomcrypt/src/headers
include $(BUILD_SHARED_LIBRARY)