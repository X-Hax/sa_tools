@echo off
echo Main.rel split
..\bin\split binary data/_Main.rel _Main.ini output/

echo STG split
..\bin\split binary data/STG00.rel STG00.ini output/
..\bin\split binary data/_STG01.rel _STG01.ini output/
..\bin\split binary data/_STG02.rel _STG02.ini output/
..\bin\split binary data/_STG03.rel _STG03.ini output/
..\bin\split binary data/_STG04.rel _STG04.ini output/
..\bin\split binary data/_STG05.rel _STG05.ini output/
..\bin\split binary data/_STG06.rel _STG06.ini output/
..\bin\split binary data/_STG07.rel _STG07.ini output/
..\bin\split binary data/_STG08.rel _STG08.ini output/
..\bin\split binary data/_STG09.rel _STG09.ini output/
..\bin\split binary data/_STG10.rel _STG10.ini output/
..\bin\split binary data/_STG12.rel _STG12.ini output/