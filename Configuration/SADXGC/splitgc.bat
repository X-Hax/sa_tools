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

echo ADV split
..\bin\split binary data/_ADV00.rel _ADV00.ini output/
..\bin\split binary data/_ADV0100.rel _ADV0100.ini output/
..\bin\split binary data/_ADV0130.rel _ADV0130.ini output/
..\bin\split binary data/_ADV02.rel _ADV02.ini output/
..\bin\split binary data/_ADV03.rel _ADV03.ini output/