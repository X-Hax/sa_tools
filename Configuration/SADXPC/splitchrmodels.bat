IF EXIST system/CHRMODELS_orig.dll (
"../ProjectManager/projectmanager.exe" -m Split -f data/system/CHRMODELS_orig.DLL -d chrmodels.ini -o output/
) ELSE (
"../ProjectManager/projectmanager.exe" -m Split -f data/system/CHRMODELS.DLL -d chrmodels.ini -o output/
)