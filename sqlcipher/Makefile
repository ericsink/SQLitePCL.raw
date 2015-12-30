CC := clang
LIBTOMCRYPT_PATH=jni/tomcrypt

CFLAGS := $(USR_CFLAGS) \
-I$(LIBTOMCRYPT_PATH)/src/headers \
-c \
-std=c99 \
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
-MT dependencies \
-fPIC

LDFLAGS := $(USR_LDFLAGS) -shared

SOURCES = $(LIBTOMCRYPT_PATH)/src/ciphers/aes/aes.o \
$(LIBTOMCRYPT_PATH)/src/hashes/sha1.o \
$(LIBTOMCRYPT_PATH)/src/hashes/helper/hash_memory.o \
$(LIBTOMCRYPT_PATH)/src/hashes/sha2/sha256.o \
$(LIBTOMCRYPT_PATH)/src/modes/cbc/cbc_encrypt.o \
$(LIBTOMCRYPT_PATH)/src/modes/cbc/cbc_decrypt.o \
$(LIBTOMCRYPT_PATH)/src/modes/cbc/cbc_start.o \
$(LIBTOMCRYPT_PATH)/src/modes/cbc/cbc_done.o \
$(LIBTOMCRYPT_PATH)/src/mac/hmac/hmac_init.o \
$(LIBTOMCRYPT_PATH)/src/mac/hmac/hmac_memory.o \
$(LIBTOMCRYPT_PATH)/src/mac/hmac/hmac_process.o \
$(LIBTOMCRYPT_PATH)/src/mac/hmac/hmac_done.o \
$(LIBTOMCRYPT_PATH)/src/misc/zeromem.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_find_cipher.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_find_hash.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_register_hash.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_register_cipher.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_register_prng.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_argchk.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_cipher_is_valid.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_cipher_descriptor.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_prng_descriptor.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_hash_is_valid.o \
$(LIBTOMCRYPT_PATH)/src/misc/crypt/crypt_hash_descriptor.o \
$(LIBTOMCRYPT_PATH)/src/misc/pkcs5/pkcs_5_2.o \
$(LIBTOMCRYPT_PATH)/src/prngs/fortuna.o \
sqlite3.o

TARGET ?= libpackaged_sqlcipher.so

all: $(TARGET)

$(TARGET): $(SOURCES)
	$(CC) $(LDFLAGS) -o $@ $^
    
%.o: %.c
	$(CC) $(CFLAGS) -o $@ -x c $<
