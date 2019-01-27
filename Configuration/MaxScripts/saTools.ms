fileIn "saScripts\\pvmImport.ms"
fileIn "saScripts\\lvlImport.ms"
fileIn "saScripts\\ntblExport.ms"

if (heapSize < 40000000) then
	heapSize = 400000000 

chglog = #(
	"TexPack Importer: 1.00 (Developed by ItsEasyActually)\n",
	"SA Level Importer: 1.00 (Developed by ItsEasyActually)\n",
	"Node Table Texporter: 1.60 (Developed by JCorvinus (Dude))\n",
	"\n",
	"[2019-01-27]	Initial Release\n"
)

clearlistener()

rollout impRoll "SA Level Importer" width:350 height:91
(
	button importSA1LVL "Import SA1LVL File" pos:[10,33] width:150 height:42
	button importSABinary "Import SA1/DX Binary File" pos:[180,33] width:150 height:42 enabled:false 
 
 
	label lbl2 "What type of file would you like to import from?" pos:[54,8] width:231 height:15
	on importSA1LVL pressed do
		(importSA1LVLFile())
	on importSABinary pressed do
		(createDialog binRoll)
)

rollout guiRoll "Quick Guide" width:296 height:210
(
	label lbl7 "Super Quick Guide to Importing Data" pos:[61,10] width:180 height:21
	label lbl8 "First, be sure to import your TexPack/PVM. You can export one using PVMEditSharp in the SA Tools. Typically the text file will be named index.txt." pos:[10,35] width:280 height:47
	label lbl9 "Second, use the SA Level Importer to select your SA1LVL or your Binary/EXE file and import your level. If you made sure to import your TexPack first, your level will import with the textures auto-assigned to the level." pos:[10,87] width:280 height:59
	label lbl18 "This script separates visible geometry and invisible geometry by layers. Goto Tools/Manage Layers, and you can toggle the 'Collision' layer on and off." pos:[10,150] width:280 height:48
)

rollout chgRoll "ChangeLog and Contact Info" width:425 height:285
(
	listbox logger "Changelog" pos:[10,10] width:405 height:10 items:chglog
	label lbl16 "If you find any errors with these utilities or would like to suggest some improvements, please submit an issue on the GitHub Repo for the SA Tools, specifically mentioning the Sonic Adventure Tools for 3DS Max." pos:[10,169] width:405 height:42
	label lbl17 "Alternatively, you can contact me on the following services:" pos:[10,220] width:405 height:15
	label lbl18 "Twitter: @ItsEasyActually" pos:[10,235] width:405 height:15
	label lbl19 "Discord: ItsEasyActually" pos:[10,250] width:405 height:15
	label lbl20 "Sonic Retro: darkspines35" pos:[10,265] width:405 height:15
)

rollout abtRoll "About these Tools" width:291 height:227
(
	label lbl8 "--- TexPack Importer ---" pos:[91,8] width:119 height:15
	label lbl9 "This tool will import a PVM file that has been exported to the TexPack format via PVMEditSharp. It will auto-populate a multi-sub material in material slot 1." pos:[11,23] width:270 height:46
	label lbl10 "--- SA Level Importer ---" pos:[89,73] width:131 height:15
	label lbl11 "This tool is for importing level files directly into 3DS Max from their raw data. Currently, SA1LVL files are supported, thus only the Sonic Adventure/Ninja Basic format is supported. Binary/EXE support is in the works." pos:[11,88] width:270 height:56
	label lbl12 "--- Node Table Exporter ---" pos:[86,148] width:131 height:15
	label lbl13 "This tool is used for exporting to the NodeTable Format when exporting levels directly from 3DS Max. Please see its Help button for instructions on usage." pos:[11,163] width:270 height:40
)

rollout helpRoll "SA Tools Helper" width:175 height:107
(
	button guideBtn "Quick Guide" pos:[10,56] width:150 height:40
	button abtBtn	"About" pos:[10,11] width:150 height:40
	
	on guideBtn pressed do
	(createDialog guiRoll)
	on abtBtn pressed do 
	(createDialog abtRoll)
)

utility openApp "Sonic Adventure Tools" width:162 height:206
(
	button lvlBtn "SA Level Importer" pos:[6,113] width:150 height:26
	button texBtn "TexPack Importer" pos:[6,83] width:150 height:26
	button helpBtn "Help" pos:[6,34] width:150 height:26
	label lbl1 "--------------------------------------" pos:[7,64] width:148 height:15
	button chgBtn "Changelog" pos:[6,4] width:150 height:26
	button nodeBtn "Node Table Exporter" pos:[6,143] width:150 height:26
	on lvlBtn pressed do
		(CreateDialog impRoll)
	on texBtn pressed do
		(ImportPVMFile())
	on helpBtn pressed do
		(CreateDialog helpRoll)
	on chgBtn pressed do
		(CreateDialog chgRoll)
	on nodeBtn pressed do
		(CreateDialog nodeRoll)
)
