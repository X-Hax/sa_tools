IF EXIST system/CHRMODELS_orig.dll (
splitDLL system/CHRMODELS_orig.DLL chrmodels.ini
) ELSE (
splitDLL system/CHRMODELS.DLL chrmodels.ini
)