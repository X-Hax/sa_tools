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

echo Boss split
..\bin\split binary data/B_CHAOS0.rel B_CHAOS0.ini output/
..\bin\split binary data/B_CHAOS2.rel B_CHAOS2.ini output/
..\bin\split binary data/B_CHAOS4.rel B_CHAOS4.ini output/
..\bin\split binary data/_B_CHAOS6.rel _B_CHAOS6.ini output/
..\bin\split binary data/_B_CHAOS7.rel _B_CHAOS7.ini output/
..\bin\split binary data/B_EGM1.rel B_EGM1.ini output/
..\bin\split binary data/B_EGM2.rel B_EGM2.ini output/
..\bin\split binary data/B_EGM3.rel B_EGM3.ini output/
..\bin\split binary data/B_E101.rel B_E101.ini output/
..\bin\split binary data/B_E101_R.rel B_E101_R.ini output/
..\bin\split binary data/B_ROBO.rel B_ROBO.ini output/

echo Subgame split
..\bin\split binary data/_Minicart.rel _Minicart.ini output/
..\bin\split binary data/_Shooting.rel _Shooting.ini output/
..\bin\split binary data/sboard.rel sboard.ini output/

echo Chao split
..\bin\split binary data/ChaoStgGarden00SS.rel ChaoStgGarden00SS.ini output/
..\bin\split binary data/ChaoStgGarden01EC.rel ChaoStgGarden01EC.ini output/
..\bin\split binary data/ChaoStgGarden02MR_Daytime.rel ChaoStgGarden02MR_Daytime.ini output/
..\bin\split binary data/ChaoStgGarden02MR_Evening.rel ChaoStgGarden02MR_Evening.ini output/
..\bin\split binary data/ChaoStgGarden02MR_Night.rel ChaoStgGarden02MR_Night.ini output/
..\bin\split binary data/ChaoStgBlackmarket.rel ChaoStgBlackmarket.ini output/
..\bin\split binary data/ChaoStgEntrance.rel ChaoStgEntrance.ini output/
..\bin\split binary data/ChaoStgRace.rel ChaoStgRace.ini output/