#!/bin/sh
# TODO use same options as other platforms
gcc -shared -fPIC -O -o libe_sqlite3.so ../sqlite3/sqlite3.c -lc

