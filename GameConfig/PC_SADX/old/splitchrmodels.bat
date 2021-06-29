IF EXIST data/system/CHRMODELS_orig.dll (
..\..\bin\split binary data/system/CHRMODELS_orig.DLL chrmodels.ini output/
) ELSE (
..\..\bin\split binary data/system/CHRMODELS.DLL chrmodels.ini output/
)