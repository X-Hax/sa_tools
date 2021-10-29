/*
saanim Json I/O Script
By Shadowth117
Shoutouts to MainMemory and the others who've worked on SATools for their work that makes this possible.
Credit to Mambo4 of cgsociety for posting a bit of a usage guide for the Json library with Maxscript and some .json reading code lines.
Credit to TGE for help on debugging the script as well as for borrowed code from his Renderware animation importer
*/

struct nodeData 
(
	ref = undefined,
	tfm = undefined
)

rollout tagRoll "saanim Json I/O" 
(
local tfmData = #()
	
Label lab2 "Please pick an saanim .json for your model"
button animImportSA1 "Load SA1 Anim" across:1 width:120 align:#center
button animImportSA2 "Load SA2 Anim" across:1 width:120 align:#center
button animExportSA1 "Export SA1 Anim" across:1 width:120 align:#center
button animExportSA2 "Export SA2 Anim" across:1 width:120 align:#center
Label lab3 "Note: Anim Data is not fully reset because of node positionings." align:#left
Label lab4 "Reload model before anim each time for best results." align:#left
checkBox loadClear "Clear Anim Data on Load" checked:true align:#left
checkBox clearProp "Clear bone000 User Properties" checked:true align:#left
checkBox saToolSA2 "SA2 Model is from SA Tools" checked:true align:#left
checkBox tgeMode "Model is from TGE's converter" checked:false align:#left
	
--Function to return a json file the user picks
fn getJsonFileAsString filePath =
(
	local  jsonString=""
	IOFile = dotNetClass "System.IO.File"
	jsonString = IOFile.ReadAllText(filePath)
	IOFile = undefined
	return jsonString
)

--Animation Clearer
fn ClearAnimations =
(
	for obj in objects do
	(
		DeleteKeys obj.controller #allkeys
	)
)

/*
--Pose Saver
fn SavePose =
(
	for obj in objects do
	(
		append tfmData (nodeData ref:obj tfm:(deepcopy obj.transform))
	)
)
	
--Pose Loader
fn LoadPose =
(
	for obj in tfmData do
	(
		obj.ref.transform = (deepcopy obj.tfm)
	)
)*/

--BAMS angle to Euler Angle conversion
fn BAMStoEuler BAMSstr = 
(
	BAMSFilter = filterstring BAMSstr " ,"
	BAMSX = ("0x" + (BAMSFilter [1] as string)) as integer
	BAMSY = ("0x" + (BAMSFilter [2] as string)) as integer
	BAMSZ = ("0x" + (BAMSFilter [3] as string)) as integer
    XEuler = ((BAMSX) * 360.0 / 65536.0)
	YEuler = ((BAMSY) * 360.0 / 65536.0)
	ZEuler = ((BAMSZ) * 360.0 / 65536.0)
	
	saAngles = eulerAngles XEuler YEuler ZEuler 
)	

--Euler Angle to BAMS angle conversion
fn eulerToBAMS eulerVal = 
(
	BAMSX = ((eulerVal.x) * 65536.0 / 360.0)
	BAMSY = ((eulerVal.y) * 65536.0 / 360.0)
	BAMSZ = ((eulerVal.z) * 65536.0 / 360.0)
	BAMSX = bit.intAsHex BAMSX
	BAMSY = bit.intAsHex BAMSY
	BAMSZ = bit.intAsHex BAMSZ
	
	BAMSstring = toUpper (BAMSX + ", " + BAMSY + ", " + BAMSZ)
)

--More so SA Tools model anim loading than SA1, but this was made before SA2 exports worked.
--Mainly requires the strange y-z flipping on root position to not happen for sa2
--Ideally the loadSA2Anim function would be refactored into this, but I will probably do that another time.
fn loadSA1Anim sa1Anim startPth sa2Mode =
(
	
	--use dotnet.loadAssembly to load the Newtonsoft.Json library:
	json = dotnet.loadAssembly (startPth + "\\Newtonsoft.Json.dll")
	
	--Create our .Net object and parse the .json
    anmObj = dotNetObject "Newtonsoft.Json.Linq.Jobject"
	anmObjParse = anmObj.parse sa1Anim
	anmObj = undefined
	
	--Grab easy values and store them in Settings for later
	FrameCount = anmObjParse.item["Frames"].value
	AnimName = anmObjParse.item["Name"].value
	ModelParts = anmObjParse.item["ModelParts"].value
	InterpMode = anmObjParse.item["InterpolationMode"].value
	objArray = objects as array
	
	--Max's dae import does funny things to scaling to all root nodes. This fixes that.
	LoadScale = objArray[1].scale.x
	objArray[1].scale.x = (1 / LoadScale)
	objArray[1].scale.y = (1 / LoadScale)
	objArray[1].scale.z = (1 / LoadScale)
	
	try(delete $Settings) catch()
    nDummy = dummy pos: ( [0, 0, 0] ) name: ( "Settings")
	setUserPropBuffer $Settings ""
	setUserProp $Settings "Original Frames" (FrameCount as Integer)
	setUserProp $Settings "Name" (AnimName as String)
	setUserProp $Settings "ModelParts" (ModelParts as Integer) 
	setUserProp $Settings "InterpolationMode" (InterpMode as Integer)
	
	animationRange = interval 0 (FrameCount-1)
	
	animButtonState = true
	
    -- Check if scale is used because Sega goes XZY for position on non root nodes if it's used for whatever reason =w=
	scaleCheck = false
	 i = 1
	curToken = anmObjParse.item["Models"].First
	try (
	for i = 1 to ((anmObjParse.item["Models"].Last.name as Integer) + 1) do
	(
		if ((i-1) != (curToken.name as integer)) then (continue)
		saBoneObj = objArray[i]
		
		--Loop through Scale
		scaleCount = curToken.First.First.Next.Next.First.count
		if (scaleCount != 0) then
		(
			scaleCheck = true
		)
		saBoneObj = undefined
		curToken = curToken.Next
	)
    )
	catch ()
	
	-- Sort through "Models" and store names and values
	 i = 1
	curToken = anmObjParse.item["Models"].First
	try (
	for i = 1 to ((anmObjParse.item["Models"].Last.name as Integer) + 1) do
	(
		if ((i-1) != (curToken.name as integer)) then (continue)
		saBoneObj = objArray[i]

		--Loop through Position 
		posCount = curToken.First.First.First.count
		if (posCount != 0) then
		(
			curPos = curToken.First.First.First.First
			for j=1 to posCount do(
				posTime = (curPos.name as integer)
				posCorrected = execute ("[" + curPos.First.value + "]")
				posCorrected.x = (posCorrected.x)
				if ( i == 1 or scaleCheck == false and sa2Mode == false) then
				(
					posTemp = (posCorrected.z)
					posCorrected.z = (posCorrected.y)
					posCorrected.y = (posTemp)
				)
				at time posTime saBoneObj.Controller.Position = (posCorrected)
				curPos = curPos.Next
			)
		)
		--Loop through Rotation
		rotCount = curToken.First.First.Next.First.count	
		if (rotCount != 0) then
		(
			curRot = curToken.First.First.Next.First.First
			for j=1 to rotCount do(
				rotTime = (curRot.name as integer)
				eulRot = BAMStoEuler curRot.First.value
				--rotTemp = (eulRot.z)
				--eulRot.z = eulRot.y
				--eulRot.y = -rotTemp
				at time rotTime saBoneObj.Controller.Rotation = ( eulRot ) as quat
				curRot = curRot.Next
			)
		)
		--Loop through Scale
		scaleCount = curToken.First.First.Next.Next.First.count
		if (scaleCount != 0) then
		(
			curScale = curToken.First.First.Next.Next.First.First
			for j=1 to scaleCount do(
				sclTime = (curScale.name as integer)
				at time sclTime saBoneObj.Controller.Scale = (execute ("[" + curScale.First.value + "]"))
				curScale = curScale.Next
			)
		)
		saBoneObj = undefined
		curToken = curToken.Next
	)
    )
	catch ()
	animButtonState = false
	--Unscale at end to be what was there before
	objArray[1].scale.x = (LoadScale)
	objArray[1].scale.y = (LoadScale)
	objArray[1].scale.z = (LoadScale)
	
	gc()
)

fn loadSA2Anim sa2Anim startPth =
(
	
	--use dotnet.loadAssembly to load the Newtonsoft.Json library:
	json = dotnet.loadAssembly (startPth + "\\Newtonsoft.Json.dll")
	
	--Create our .Net object and parse the .json
    anmObj = dotNetObject "Newtonsoft.Json.Linq.Jobject"
	anmObjParse = anmObj.parse sa2Anim
	anmObj = undefined
	
	--Grab easy values and store them in bone000 for later
	FrameCount = anmObjParse.item["Frames"].value
	AnimName = anmObjParse.item["Name"].value
	ModelParts = anmObjParse.item["ModelParts"].value
	InterpMode = anmObjParse.item["InterpolationMode"].value
	
	--Max's dae import does funny things to scaling to all root nodes. This fixes that.
	objArray = objects as array
	LoadScale = objArray[1].scale.x
	for o in objects do
	(
		if (o.parent == undefined) then
		(
			o.scale.x = (1 / LoadScale)
		    o.scale.y = (1 / LoadScale)
			o.scale.z = (1 / LoadScale)
		)
	)
		
	if (clearProp.checked == true and tgeMode.checked == false) then
	(	
		setUserPropBuffer $bone000 ""
		setUserProp $bone000 "Original Frames" (FrameCount as Integer)
		setUserProp $bone000 "Name" (AnimName as String)
		setUserProp $bone000 "ModelParts" (ModelParts as Integer)
		setUserProp $bone000 "InterpolationMode" (InterpMode as Integer)
	) else if (clearProp.checked == true) then (
		setUserPropBuffer $node_0 ""
		setUserProp $node_0 "Original Frames" (FrameCount as Integer)
		setUserProp $node_0 "Name" (AnimName as String)
		setUserProp $node_0 "ModelParts" (ModelParts as Integer)
		setUserProp $node_0 "InterpolationMode" (InterpMode as Integer)
	)
	if tgeMode.checked then ( deleteKeys objects #allKeys )
	animationRange = interval 0 (FrameCount-1)
	
	maxops.setDefaultTangentType #smooth #smooth
	animButtonState = true
	-- Sort through "Models" and store names and values
	 i = 1
	curToken = anmObjParse.item["Models"].First
	try (
	for i = 1 to anmObjParse.item["Models"].count do
	(
		-- Deal with bones based on mode
		saBone = (curToken.name as integer)
		if (tgeMode.checked == false) then
		(
			if (saBone < 10) then
			(
				saBone = ("bone00" + saBone as string)
			) else if (saBone < 100) then
			(
				saBone = ("bone0" + saBone as string)
			) else
			(
				saBone = ("bone" + saBone as string)
			)
	    ) else 
		(
			saBone = ("node_" + saBone as string)
		)
		saBoneObj = (getNodeByName saBone)
		
		--Loop through Position 
		posCount = curToken.First.First.First.count
		if (posCount != 0) then
		(
			curPos = curToken.First.First.First.First
			for j=1 to posCount do(
				posTime = (curPos.name as integer)
				posCorrected = execute ("[" + curPos.First.value + "]")
				--posTemp = (posCorrected.x)
				--posCorrected.x = posCorrected.z
				--posCorrected.z = posTemp
				at time posTime saBoneObj.Controller.Position = (posCorrected)
				curPos = curPos.Next
			)
		)
		--Loop through Rotation
		rotCount = curToken.First.First.Next.First.count	
		if (rotCount != 0) then
		(
			curRot = curToken.First.First.Next.First.First
			for j=1 to rotCount do(
				rotTime = (curRot.name as integer)
				eulRot = BAMStoEuler curRot.First.value
				at time rotTime saBoneObj.Controller.Rotation = ( eulRot ) as quat
				curRot = curRot.Next
			)
		)
		--Loop through Scale
		scaleCount = curToken.First.First.Next.Next.First.count
		if (scaleCount != 0) then
		(
			curScale = curToken.First.First.Next.Next.First.First
			for j=1 to scaleCount do(
				sclTime = (curScale.name as integer)
				at time sclTime saBoneObj.Controller.Scale = (execute ("[" + curScale.First.value + "]"))
				curScale = curScale.Next
			)
		)
		saBoneObj = undefined
		curToken = curToken.Next
	)
    )
	catch()
	animButtonState = false
	--Unscale at end to be what was there before
	for o in objects do
	(
		if (o.parent == undefined) then
		(
			objArray[1].scale.x = (LoadScale)
		    objArray[1].scale.y = (LoadScale)
			objArray[1].scale.z = (LoadScale)
		)
	)
	gc()
)

	
fn ExportSA1Anim jPath startPth sa2Mode =
(
	
	--use dotnet.loadAssembly to load the Newtonsoft.Json library:
	json = dotnet.loadAssembly (startPth + "\\Newtonsoft.Json.dll")
	
	--Create our .Net objects and store previously loaded information
    anmObj = dotNetObject "Newtonsoft.Json.Linq.Jobject"
	modelObj = "{\n\"Models\": {\n"

	scaleExCheck = false
	--Check if anything uses scale so we can account for that. SA1 handles that funny because reasons
	for i=1 to objects.count do
	(
		o = objects[i]
		--Only Store keys if they have them 
		if (o.scale.controller.keys.count > 0) then
		(
			scaleExCheck = true
		)
	)
	
	--Loop through objects and gather their keyframes
	for i=1 to objects.count do
	(
		o = objects[i]
		--Only add values for the bone if it's used
		if ((o.position.controller.keys.count > 0 or o.rotation.controller.keys.count > 0  or o.scale.controller.keys.count > 0)) then
		(
			boneId = ((i - 1) as String)
			append modelObj ("\"" + boneId + "\": {\n")
			append modelObj ("\"Position\": {\n")
			
			--Only Store keys if they have them 
			if (o.position.controller.keys.count > 0) then
			(
				for itemKey in o.position.controller.keys do 
				(
					at time itemKey.Time
					posKeyTime = (filterString (itemKey.Time as String) "f")
					posKeyValue = (filterString (at time itemKey.Time o.Controller.Position as String) "[,]")
					if ( i == 1 or scaleExCheck == false and sa2Mode == false) then
					(
						tempPosVal = posKeyValue[2]
						posKeyValue[2] = posKeyValue[3]
						posKeyValue[3] = tempPosVal
					)

					append modelObj ("\"" + posKeyTime[1] + "\": \"" + posKeyValue[1] + ", " + posKeyValue[2] + ", " + posKeyValue[3] + "\",\n")
				)
			)
			append modelObj ("},\n")
			append modelObj ("\"Rotation\": {\n")
			
			--Only Store keys if they have them 
			if (o.rotation.controller.keys.count > 0) then
			(
				for itemKey in o.rotation.controller.keys do 
				(
					at time itemKey.Time
					rotKeyTime = (filterString (itemKey.Time as String) "f")
					rotConv = (quatToEuler (at time itemKey.Time o.Controller.Rotation))
					rotStr = eulerToBAMS rotConv
					append modelObj ("\"" + rotKeyTime[1] + "\": \"" + rotStr + "\",\n")
				)
			)
			append modelObj ("},\n")
			
			append modelObj ("\"Scale\": {\n")
			
			--Only Store keys if they have them 
			if (o.scale.controller.keys.count > 0) then
			(
				for itemKey in o.scale.controller.keys do 
				(
					at time itemKey.Time
					scaleKeyTime = (filterString (itemKey.Time as String) "f")
					scaleKeyValue = (filterString ((at time itemKey.Time o.Controller.Scale) as String) "[,]")
					append modelObj ("\"" + scaleKeyTime[1] + "\": \"" + scaleKeyValue[1] + ", " + scaleKeyValue[2] + ", " + scaleKeyValue[3] + "\",\n")
				)
			)
			append modelObj ("},\n")
			
			append modelObj ("},\n")
		)
	)
	aRange = (animationrange.end+1) as String
	aRange = substring aRange 1 (aRange.count-1)
    append  modelObj ("},\n\"Frames\": " + aRange + ",\n\"Name\": \"" + ((getUserProp $Settings "Name") as String) + "\",\n\"ModelParts\": " + ((getUserProp $Settings "ModelParts") as String) + ",\n\"InterpolationMode\": " + ((getUserProp $Settings "InterpolationMode") as String) + "\n}")
	
	--Convert the json to a string
	anmObjOut = anmObj.parse modelObj
	jsonOut = anmObjOut.toString()
	
	--Create and write to file
	fs = openFile jPath mode:"wt" --open specified json file
	format "%" jsonOut to:fs --write string to the file
	close fs
	
	anmObj = undefined
	gc()
)

fn ExportSA2Anim jPath startPth =
(
	
	--use dotnet.loadAssembly to load the Newtonsoft.Json library:
	json = dotnet.loadAssembly (startPth + "\\Newtonsoft.Json.dll")
	
	--Create our .Net objects and store previously loaded information
    anmObj = dotNetObject "Newtonsoft.Json.Linq.Jobject"
	modelObj = "{\n\"Models\": {\n"
	--Loop through objects and gather their keyframes
	for o in objects do
	(
		if (tgeMode.checked == false) then
		(
		    boneCheck = (findString (o.name) "bone")
		) else
		(
			boneCheck = (findString (o.name) "node_")
		)
		--Only add values for the bone if it's used
		if (boneCheck != undefined and (o.position.controller.keys.count > 0 or o.rotation.controller.keys.count > 0  or o.scale.controller.keys.count > 0)) then
		(
			if (tgeMode.checked == false) then
			(
				boneName = (substring (o.name) boneCheck 7)
				boneId = (((substring boneName 5 3) as Integer) as String)
		    ) else
			(
				boneId = (((substring o.name 6 3) as Integer) as String)
			)
			append modelObj ("\"" + boneId + "\": {\n")
			append modelObj ("\"Position\": {\n")
			--Only Store keys if they have them 
			if (o.position.controller.keys.count > 0) then
			(
				for itemKey in o.position.controller.keys do 
				(
					at time itemKey.Time
					posKeyTime = (filterString (itemKey.Time as String) "f")
					posKeyValue = (filterString (at time itemKey.Time o.Controller.Position as String) "[,]")
					append modelObj ("\"" + posKeyTime[1] + "\": \"" + posKeyValue[1] + ", " + posKeyValue[2] + ", " + posKeyValue[3] + "\",\n")
				)
			)
			append modelObj ("},\n")
			append modelObj ("\"Rotation\": {\n")
			
			--Only Store keys if they have them 
			if (o.rotation.controller.keys.count > 0) then
			(
				for itemKey in o.rotation.controller.keys do 
				(
					at time itemKey.Time
					rotKeyTime = (filterString (itemKey.Time as String) "f")
					rotConv = (quatToEuler (at time itemKey.Time o.Controller.Rotation))
					rotStr = eulerToBAMS rotConv
					append modelObj ("\"" + rotKeyTime[1] + "\": \"" + rotStr + "\",\n")
				)
			)
			append modelObj ("},\n")
			
			append modelObj ("\"Scale\": {\n")
			
			--Only Store keys if they have them 
			if (o.scale.controller.keys.count > 0) then
			(
				for itemKey in o.scale.controller.keys do 
				(
					at time itemKey.Time
					scaleKeyTime = (filterString (itemKey.Time as String) "f")
					scaleKeyValue = (filterString ((at time itemKey.Time o.Controller.Scale) as String) "[,]")
					append modelObj ("\"" + scaleKeyTime[1] + "\": \"" + scaleKeyValue[1] + ", " + scaleKeyValue[2] + ", " + scaleKeyValue[3] + "\",\n")
				)
			)
			append modelObj ("},\n")
			
			append modelObj ("},\n")
		)
	)
	aRange = (animationrange.end+1) as String
	aRange = substring aRange 1 (aRange.count-1)
	if (tgeMode.checked == false) then
	(
		append  modelObj ("},\n\"Frames\": " + aRange + ",\n\"Name\": \"" + ((getUserProp $bone000 "Name") as String) + "\",\n\"ModelParts\": " +  ((getUserProp $bone000 "ModelParts") as String) + ",\n\"InterpolationMode\": " + ((getUserProp $bone000 "InterpolationMode") as String) + "\n}")
	)
    else
	(
		append  modelObj ("},\n\"Frames\": " + aRange + ",\n\"Name\": \"" + ((getUserProp $node_0 "Name") as String) + "\",\n\"ModelParts\": " +  ((getUserProp $node_0 "ModelParts") as String) + ",\n\"InterpolationMode\": " + ((getUserProp $node_0 "InterpolationMode") as String) + "\n}")
	)
	--Convert the json to a string
	anmObjOut = anmObj.parse modelObj
	jsonOut = anmObjOut.toString()
	
	--Create and write to file
	fs = openFile jPath mode:"wt" --open specified json file
	format "%" jsonOut to:fs --write string to the file
	close fs
	
	anmObj = undefined
	gc()
)

on animImportSA1 pressed do
(
	if loadClear.checked then
	(
		ClearAnimations()
	)
	-- Script start dir
	startPath = getFilenamePath(getThisScriptFilename())
	-- Open up a dialog box
	local jsonPath = getOpenFileName caption:"Import saanim .json" \
			types:"saanim json (*.json)|*.json|All Files (*.*)|*.*|"
	if jsonPath != undefined then
	(
		--Load our Json
		jsonString = getJsonFileAsString jsonPath
		--Load the Animation in
		loadSA1Anim jsonString startPath false
	)
)

on animImportSA2 pressed do
(
	if ((tgeMode.checked == true and $node_0 != undefined) or $bone000 != undefined or saToolSA2.checked == true) then
    (
		if loadClear.checked then
		(
			ClearAnimations()
		)
		-- Script start dir
		startPath = getFilenamePath(getThisScriptFilename())
		-- Open up a dialog box
		local jsonPath = getOpenFileName caption:"Import saanim .json" \
				types:"saanim json (*.json)|*.json|All Files (*.*)|*.*|"
			
		if jsonPath != undefined then
		(
			--Load our Json
			jsonString = getJsonFileAsString jsonPath
			--Load the Animation in
			if saToolSA2.checked == true then
			(
				loadSA1Anim jsonString startPath saToolSA2.checked
			) else
			(
				loadSA2Anim jsonString startPath
			)
		)
    ) else (
        messageBox "Please import an appropriate SA2/B model"
    )
)

on animExportSA1 pressed do
(
	if $Settings != undefined then
    (
		-- Script start dir
		startPath = getFilenamePath(getThisScriptFilename())
		-- Open up a dialog box
		local jsonPath = getSaveFileName caption:"Export saanim .json" \
				types:"saanim json (*.json)|*.json|All Files (*.*)|*.*|"
			
		if jsonPath != undefined then
		(
			--Export the Animation in
			exportSA1Anim jsonPath startPath false
		)

    ) else (
        messageBox "Please load an appropriate SA1/DX model, animation, and ensure a 'Settings' object with appropriate parameters exists"
    )
)

on animExportSA2 pressed do
(
	if ((tgeMode.checked == true and $node_0 != undefined) or $bone000 != undefined or saToolSA2.checked) then
    (
		-- Script start dir
		startPath = getFilenamePath(getThisScriptFilename())
		-- Open up a dialog box
		local jsonPath = getSaveFileName caption:"Export saanim .json" \
				types:"saanim json (*.json)|*.json|All Files (*.*)|*.*|"

		if jsonPath != undefined then
		(
			--Export the Animation in
			if saToolSA2.checked == true then
			(
				exportSA1Anim jsonPath startPath saToolSA2.checked 
			)
			else
			(
				exportSA2Anim jsonPath startPath 
			)
		)

    ) else (
        messageBox "Please load an appropriate SA2/B model and an animation"
    )
)


)
createDialog tagRoll 320 240




