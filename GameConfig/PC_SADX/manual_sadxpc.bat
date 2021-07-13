@echo off
cls
echo EXE split
..\..\bin\split binary data/sonic.exe STG00.ini output/
..\..\bin\split binary data/sonic.exe STG01.ini output/
..\..\bin\split binary data/sonic.exe STG02.ini output/
..\..\bin\split binary data/sonic.exe STG03.ini output/
..\..\bin\split binary data/sonic.exe STG04.ini output/
..\..\bin\split binary data/sonic.exe STG05.ini output/
..\..\bin\split binary data/sonic.exe STG06.ini output/
..\..\bin\split binary data/sonic.exe STG07.ini output/
..\..\bin\split binary data/sonic.exe STG08.ini output/
..\..\bin\split binary data/sonic.exe STG09.ini output/
..\..\bin\split binary data/sonic.exe STG10.ini output/
..\..\bin\split binary data/sonic.exe STG12.ini output/
..\..\bin\split binary data/sonic.exe ADV00.ini output/
..\..\bin\split binary data/sonic.exe ADV0100.ini output/
..\..\bin\split binary data/sonic.exe ADV0130.ini output/
..\..\bin\split binary data/sonic.exe ADV02.ini output/
..\..\bin\split binary data/sonic.exe ADV03.ini output/
..\..\bin\split binary data/sonic.exe ADVERTISE.ini output/
..\..\bin\split binary data/sonic.exe Animals.ini output/
..\..\bin\split binary data/sonic.exe B_CHAOS0.ini output/
..\..\bin\split binary data/sonic.exe B_CHAOS2.ini output/
..\..\bin\split binary data/sonic.exe B_CHAOS4.ini output/
..\..\bin\split binary data/sonic.exe B_CHAOS6.ini output/
..\..\bin\split binary data/sonic.exe B_CHAOS7.ini output/
..\..\bin\split binary data/sonic.exe B_E101.ini output/
..\..\bin\split binary data/sonic.exe B_E101_R.ini output/
..\..\bin\split binary data/sonic.exe B_EGM1.ini output/
..\..\bin\split binary data/sonic.exe B_EGM2.ini output/
..\..\bin\split binary data/sonic.exe B_EGM3.ini output/
..\..\bin\split binary data/sonic.exe B_ROBO.ini output/
..\..\bin\split binary data/sonic.exe Chao.ini output/
..\..\bin\split binary data/sonic.exe Characters.ini output/
..\..\bin\split binary data/sonic.exe CommonObjects.ini output/
..\..\bin\split binary data/sonic.exe Debug.ini output/
..\..\bin\split binary data/sonic.exe Enemies.ini output/
..\..\bin\split binary data/sonic.exe Event.ini output/
..\..\bin\split binary data/sonic.exe Fish.ini output/
..\..\bin\split binary data/sonic.exe MINICART.ini output/
..\..\bin\split binary data/sonic.exe Mission.ini output/
..\..\bin\split binary data/sonic.exe SBOARD.ini output/
..\..\bin\split binary data/sonic.exe SHOOTING.ini output/
..\..\bin\split binary data/sonic.exe Texlists.ini output/
..\..\bin\split binary data/sonic.exe Sounds.ini output/
..\..\bin\split binary data/sonic.exe Misc.ini output/
echo DLL split
..\..\bin\split binary data/system/CHRMODELS_orig.DLL CHRMODELS.ini output/
..\..\bin\split binary data/system/BOSSCHAOS0MODELS.DLL B_CHAOS0_DLL.ini output/
..\..\bin\split binary data/system/CHAOSTGGARDEN02MR_DAYTIME.DLL chaostggarden02mr_daytime.ini output/
..\..\bin\split binary data/system/CHAOSTGGARDEN02MR_EVENING.DLL chaostggarden02mr_evening.ini output/
..\..\bin\split binary data/system/CHAOSTGGARDEN02MR_NIGHT.DLL chaostggarden02mr_night.ini output/
..\..\bin\split binary data/system/ADV00MODELS.DLL ADV00_DLL.ini output/
..\..\bin\split binary data/system/ADV01MODELS.DLL ADV0100_DLL.ini output/
..\..\bin\split binary data/system/ADV01CMODELS.DLL ADV0130_DLL.ini output/
..\..\bin\split binary data/system/ADV02MODELS.DLL ADV02_DLL.ini output/
..\..\bin\split binary data/system/ADV03MODELS.DLL ADV03_DLL.ini output/
echo NB split
..\..\bin\split nb data/system/E101R_GC.NB output/ -ini B_E101_R_NB.ini
..\..\bin\split nb data/system/EROBO_GC.NB output/ -ini B_ROBO_NB.ini