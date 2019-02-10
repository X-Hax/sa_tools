rollout nodeRoll "Nodetable exporter v1.6" width:162 height:300
(
	button help_button "Help" pos:[8,8] width:139 height:26

	label lbl1 "Output Path" pos:[9,60] width:133 height:13
	edittext outpath "" pos:[3,75] width:145 height:20
	button btn_browse "Browse" pos:[7,100] width:78 height:26	
	edittext startGbixValue "First GBIX value" pos:[8, 140] width:145 height:20
	edittext pvmName "PVM Name" pos:[8,176] width:145 height:20
	button exp_exec "Export Data" pos:[8,220] width:139 height:26
	
	on SA_MeshExporter open do
	(
		-- add a way to retrieve configuration settings here, such as default export folder, and location of PVM Compiler
		startGBIXValue.text = "0"
	)
	
	on help_button pressed do
	(
		local mb = dotnetclass "MessageBox"
		local button = (dotnetclass "MessageBoxButtons").OK
		local icon = (dotnetclass "MessageBoxIcon").Warning
		
		mb.show ("To export, select the models you would like to export in the scene. Supply a valid path in the 'output path' text box, and then press 'export data'.") "Exporting" button icon
		mb.show ("This exporter supports exporting surface flags! To use them, you'll need to use the user-defined properties in each object you want to apply them to.\nSelect an object, right-click on it and select 'object properties'. Then navigate to the 'user defined' tab.") "Surface Properties" button icon
		mb.show ("Surface flags are one per-line. they use the format 'flagname=value'\nExample: solid=true\nThe only valid values for 'value' are true and false.\nFlagnames are all lower-case, no spaces.") "Surface Flags" button icon
		mb.show ("Available surface flags:\n   solid\n   water\n   nofriction\n   noacceleration\n   cannotland\n   increasedacceleration\n   diggable\n   unclimbable\n   hurt\n   footprints\n   visible\n") "Surface Flags" button icon
		mb.show ("If you don't apply surface flags to the model, it will assume the model is solid and visible.\nIn fact, you MUST turn solid and visible off manually. Supplying other flags will not disable solid/visible, you must set them to false yourself.") "Surface Flags" button icon
		
		mb.show ("The 'first GBIX' value is for the texturelist. PVR files have a 'global texture ID' that is used to prevent the game from loading the same texture twice. The value you supply here is the starting value, meaning each texture will get an id of this plus the current texture number") "Texture Lists" button icon
		mb.show ("In regards to textures, this exporter creates two files - a texturelist.txt file for compiling a PVM, and a texturelist.c file for compiling into your mod *.dll file. It also creates a series of png files for compiling into your pvm file.") "Texture Lists" button icon
	)
	
	on exp_exec pressed do
	(
		if(outpath.text == "") do 
		(
			return messageBox "No path supplied"
		)
		
		gbixValue = startGbixValue.text as integer64
		
		scenepath = outpath.text + "\\nodetable.txt"
		scenefile = createfile scenepath
		
		mtlpath = outpath.text + "\\scene.mtl"
		mtlfile = createfile mtlpath
			
		targetNodeCount = 0
		exportedNodes = 0
			
		if(selection.count == 0) do
		(
			return messageBox "Nothing to export! This exporter only exports the currently selected objects!"
		)
		
		/*---------------Checking for Duplicate node names------------*/		
		duplicates = false
		for s=1 to selection.count do
		(
			firstname = selection[s].name
			for subsel = 1 to selection.count do
			(
				if(subsel == s) do continue
				secondname = selection[subsel].name
				
				if(selection[s].name == selection[subsel].name) do
				(
					duplicates = true
					messageBox ("Duplicate scene node detected:" + firstname)
				)
			)
		)
		
		if(duplicates == true) do return messageBox "Fix the duplicate names, then export again."
		
		/*-------------Begin Creating Materal File--------------*/
		uniqueMaterials = #()

		for matIndx=1 to sceneMaterials.count do
		(
			currentMaterial = sceneMaterials[matIndx]
			numSubs = getNumSubMtls currentMaterial
			
			if(numSubs >= 1) then -- multi material, must scan within for dupes
			(
				for subIndx = 1 to numSubs do -- loop through all sub materials
				(
					match = false
					currentSub = currentMaterial.materialList[subIndx]
					
					for uniqueIndx = 1 to uniqueMaterials.count do
					(
						if(uniqueMaterials[uniqueIndx] == currentSub) do 
						(
							match = true
							break
						)
						
						if(uniqueMaterials[uniqueIndx].name == currentSub.name) do
						(
							match = true
							break
						)
					)
					
					if(match == false) do
					(
						if(currentSub.diffuseMapEnable == true) do (append uniqueMaterials currentSub)
					)
				)
			)
			else if(numSubs == 0) do -- Single material, ok to check for dupes
			(
				match = false
				for uniqueIndx = 1 to uniqueMaterials.count do
				(
					if(uniqueMaterials[uniqueIndx] == currentMaterial) do
					(
						match = true
						break
					)
					
					if(uniqueMaterials[uniqueIndx].name == currentMaterial.name) do
					(
						match = true
						break
					)
				)
				
				if(match == false) do
				(
					if(currentMaterial.diffuseMapEnable == true) do append uniqueMaterials currentMaterial
				)
			)
		)
		
		/*--------------------------Creating texturelist from materials!---------------------------*/
		uniqueTexMaps = #()

		for matIndx=1 to sceneMaterials.count do
		(
			currentMaterial = sceneMaterials[matIndx]
			numSubs = getNumSubMtls currentMaterial
			
			if(numSubs >= 1) then -- multi material, must scan within for dupes
			(
				for subIndx = 1 to numSubs do -- loop through all sub materials
				(
					match = false
					currentSub = currentMaterial.materialList[subIndx]
					sanitizedTexMapName = getFilenameFile(currentSub.diffuseMap.fileName)
					
					for uniqueIndx = 1 to uniqueTexMaps.count do
					(
						if(currentSub.diffuseMapEnable == true) do
						(
							if(uniqueTexMaps[uniqueIndx] == sanitizedTexMapName) do 
							(
								match = true
								break
							)
						)
					)
					
					if(match == false) do
					(
						if(currentSub.diffuseMapEnable == true) do 
						(
							append uniqueTexMaps sanitizedTexMapName
						)
					)
				)
			)
			else if(numSubs == 0) do -- Single material, ok to check for dupes
			(
				match = false
				sanitizedTexMapName = getFilenameFile (currentMaterial.diffuseMap.fileName)
				
				for uniqueIndx = 1 to uniqueTexMaps.count do
				(
					if(currentMaterial.diffuseMapEnable == true) do
					(				
						if(uniqueTexMaps[uniqueIndx] == sanitizedTexMapName) do
						(
							match = true
							break
						)
					)
				)
				
				if(match == false) do
				(
					if(currentMaterial.diffuseMapEnable == true) do
					(
						append uniqueTexMaps sanitizedTexMapName
					)
				)
			)
		)
		
		texListPath = outpath.text + "\\" + pvmName.text + ".txt"
		texListFile = createFile texListPath
		
		format ((formattedprint gbixValue) + "\n") to:texListFile

		for finalIndx = 1 to uniqueTexMaps.count do
		(
			format (uniqueTexMaps[finalIndx] + "\n") to:texListFile
		)
		flush texListFile
		close texListFile
		
		/*---------------Exporting C file--------------*/
		cFilePath = outpath.text + "\\" + pvmName.text + ".c"
		cFile = createFile cFilePath
		
		format "// Generated by Dude's Nodetable 1.6 exporter (maxscript version)\n\n" to:cFile
		format "#include \"stdafx.h\"\n" to:cFile
		format "#include \"SADXModLoader.h\"\n\n" to:cFile
		
		format ("NJS_TEXNAME " + pvmName.text + "_texlist_entries[] = {\n") to:cFile
		
		for finalIndx = 1 to uniqueTexMaps.count do
		(
			format ("\t{ \"" + uniqueTexMaps[finalIndx] + "\" },\n") to:cFile
		)
		
		format "};\n\n" to:cFile
		
		format ("NJS_TEXLIST " + pvmName.text + "_texlist = { arrayptrandlength(" + pvmName.text + "_texlist_entries" +  ") };\n") to:cFile
		
		flush cFile
		close cFile
		/*--------------------------Done exporting texturelists---------------------------*/
		sceneMatCount = uniqueMaterials.count
		for m=1 to sceneMatCount do
		(
			currentsub = uniqueMaterials[m]
			format "newmtl %\n" currentsub.name to:mtlfile
			diff = currentsub.diffuse
			diffscale = #()
			diffscale[1] = diff.red as float
			diffscale[2] = diff.green as float
			diffscale[3] = diff.blue as float
			diffscale[1] = diffscale[1]/255
			diffscale[2] = diffscale[2]/255
			diffscale[3] = diffscale[3]/255
			format "Kd % % %\n" diffscale[1] diffscale[2] diffscale[3] to:mtlfile
			spec = currentsub.specular
			specscale = #()
			specscale[1] = spec.red as float
			specscale[2] = spec.green as float
			specscale[3] = spec.blue as float
			specscale[1] = specscale[1]/255
			specscale[2] = specscale[2]/255
			specscale[3] = specscale[3]/255
			format "Ks % % %\n" specscale[1] specscale[2] specscale[3] to:mtlfile
			outputOpacity = currentsub.opacity / 100
			format "d %\n"  outputOpacity to:mtlfile
			format "Ke %\n" currentsub.Glossiness to:mtlfile
				
			if(currentsub.opacityMapEnable) do -- add check for opacity enable here
			(
				format "Map_Ka %\n" currentsub.diffuseMap.filename to:mtlfile
			)
			
			if(currentsub.diffuseMapEnable == true) then -- add check for diffuse enable here
			(
				format "Map_Kd %\n" currentsub.diffuseMap.filename to:mtlfile
				
				sanitiziedMapName = getFilenameFile(currentsub.diffuseMap.filename)
				texid = 0 -- safety value I guess, since undefined is no good
			
				for finalIndx = 1 to uniqueTexMaps.count do
				(
					if(uniqueTexMaps[finalIndx] == sanitiziedMapName) do
					(
						texid = finalIndx - 1
						break
					)
				)
				
				format "texid %\n" texid to:mtlfile
			)
			else 
			(
				format "texid 0\n" texid to:mtlfile
			)
			
			if(currentsub.twoSided) do -- add check for double sided
			(
				format "-doublesided\n" to:mtlfile
			)
			
			if(currentsub.diffusemap.coordinates.mappingType == 1) do
			(
				format "-enviroMap\n" to:mtlfile
			)
			
			format "-U_Mirror %\n" currentsub.diffusemap.coordinates.U_Mirror to:mtlfile
			format "-V_Mirror %\n" currentsub.diffusemap.coordinates.V_Mirror to:mtlfile
			format "-U_Tile %\n" currentsub.diffusemap.coordinates.U_Tile to:mtlfile
			format "-V_Tile %\n" currentsub.diffusemap.coordinates.V_Tile to:mtlfile
			
			if(currentsub.selfillumination == 100.0) then
			(
				format "-IgnoreLighting true\n" to:mtlfile
			)
			else
			(
				format "-IgnoreLighting false\n" to:mtlfile
			)
			
			format "-flatshaded %\n\n" currentsub.faceted to:mtlfile
		)
		/*----------------END Creating Material File----------------*/
		
		/*-------------Sanitizing input selection-----------------*/
		removeList = #()
		for sanitNode = 1 to selection.count do
		(
			if(classof selection[sanitNode] != editable_mesh) and (classof selection[sanitNode] != editable_poly) and (classof selection[sanitNode] != edit_mesh) and (classof selection[sanitNode] != edit_poly) do
			(
				append removeList selection[sanitNode]
			)
		)
		
		for sanitNode = 1 to removeList.count do
		(
			deselect removeList[sanitNode]
		)
		/*-------------End input sanitization-----------------------*/
		
		/* adding debug list
		expectedList = #()
		actualList = #()*/
		
		/*for n = 1 to selection.count do
		(
			append expectedList selection[n].name
		)*/
		
		masterskiplist = #()
		meshflags = #() as bitarray	
		append meshflags 1 -- used for Vcolor
		append meshflags 2 -- used for TexCoords
		nodetotal = selection.count
		targetNodeCount = nodetotal
		format "ver 1.6;\n" to:scenefile
		format "node count %;\n\n" nodetotal to:scenefile
		for n=1 to nodetotal do
		(
			skipflag = false
			mesh = selection[n].mesh
			if masterskiplist.count > 1 do
			(
				for skipcounter = 1 to masterskiplist.count do
				(
					if selection[n].name == masterskiplist[skipcounter] then skipflag = true
					--else format "1:% 2:% \n" selection[n].name masterskiplist[skipcounter]
				)
			)
			
			if skipflag == false then
			(			
			/*exportedNodes = exportedNodes + 1
			append actualList selection[n].name*/
			format "node %; #%\n" (n-1) selection[n].name to:scenefile
			append masterskiplist selection[n].name
			obj_exp_path = (outpath.text + "\\" + (n-1) as string + ".obj")
			objfile = createfile obj_exp_path
			format "mtllib scene.mtl\n" to:objfile
			format "g node_% #% \n" (n-1) selection[n].name to:objfile
		
			old_x = selection[n].pos.x
			old_y = selection[n].pos.y
			old_z = selection[n].pos.z
		
			old_pos = selection[n].pos
			
			camRotEuler = quatToEuler2 selection[n].rotation.controller.value
			
			format "pos % % %; \n" old_x old_y old_z to:scenefile
			format "rot % % %; \n" camRotEuler.x camRotEuler.y camRotEuler.z to:scenefile
				
			/*------------------------------Exporting surface flags--------------------------------*/
			surfaceFlags = 0x00000000 as integer64
				
			solidSurfaceValue = getUserProp selection[n] "solid"
			visibleSurfaceValue = getUserProp selection[n] "visible"
			waterSurfaceValue = getUserProp selection[n] "water"
			noFrictionSurfaceValue = getUserProp selection[n] "nofriction"
			noAccelSurfaceValue = getUserProp selection[n] "noacceleration"
			cannotLandSurfaceValue = getUserProp selection[n] "cannotland"
			increaseAccelSurfaceValue = getUserProp selection[n] "increasedacceleration"
			diggableSurfaceValue = getUserProp selection[n] "diggable"
			unclimbableSurfaceValue = getUserProp selection[n] "unclimbable"
			hurtSurfaceValue = getUserProp selection[n] "hurt"
			footprintsSurfaceValue = getUserProp selection[n] "footprints"
			
			print solidSurfaceValue
			print visibleSurfaceValue
			
			if(visibleSurfaceValue == undefined) then
			(
				surfaceFlags += 2147483648L -- 0x80 00 00 00
			)
			else if (visibleSurfaceValue == true) then
			(
				surfaceFlags += 2147483648L -- 0x80 00 00 00
			)
			
			if(solidSurfaceValue == undefined) then
			(
				surfaceFlags += 1L -- if no value is provided, assume they want solid. This seems to be the opposite mindset from the original SADX tools
			)
			else if (solidSurfaceValue == true) then
			(
				surfaceFlags += 1L
				print "Undefined - setting solid flag"
			)
			
			if(waterSurfaceValue != undefined) and (waterSurfaceValue == true) do
			(
				surfaceFlags += 2L
			)
			
			if(noFrictionSurfaceValue != undefined) and (noFrictionSurfaceValue == true) do
			(
				surfaceFlags += 8L
			)
			
			if(noAccelSurfaceValue != undefined) and (noAccelSurfaceValue == true) do
			(
				surfaceFlags += 8L
			)
			
			if(cannotLandSurfaceValue != undefined) and (cannotLandSurfaceValue) do
			(
				surfaceFlags += 64L
			)
			
			if(increaseAccelSurfaceValue != undefined) and (increaseAccelSurfaceValue == true) do
			(
				surfaceFlags += 128L
			)
			
			if(diggableSurfaceValue != undefined) and (diggableSurfaceValue == true) do
			(
				surfaceFlags += 128L
			)
			
			if(unclimbableSurfaceValue != undefined) and (unclimbableSurfaceValue == true) do
			(
				surfaceFlags += 4096L
			)
			
			if(hurtSurfaceValue != undefined) and (hurtSurfaceValue == true) do
			(
				surfaceFlags += 65536L
			)
			
			if(footprintsSurfaceValue != undefined) and (footprintsSurfaceValue == true) do
			(
				surfaceFlags += 1048576L
			)
				
			surfaceFlagsString = formattedPrint surfaceflags format:"#010X"
			format "surfaceflags %;\n\n" surfaceFlagsString to:scenefile
			/*------------------------------END surface flags--------------------------------*/
			
			/*-------------Vertex Exporting------------*/
			newnode = copy selection[n]
			newnode.pos = [0,0,0]
			newnode.rotation = quat 0 0 0 1
			newnode.rotation.x_rotation = 0
			newnode.rotation.y_rotation = 0
			newnode.rotation.z_rotation = 0
			convertToMesh newnode
		
			--V_count = selection[n].mesh.numVerts
			V_count = newnode.numVerts
			
			for i = 1 to V_count do
			(
				p = (GetVert newnode i)
				format "v % % % \n" p.x p.y p.z to:objfile
			)
			
			for i = 1 to V_count do
			(
				p = (GetNormal newnode i)
				format "vn % % % \n" p.x p.y p.z to:objfile 
			)
			
			delete newnode
			
			--========= Material Generation / Face Arrays =========--
			-- get material/mesh totals
			mesh = selection[n].mesh
			--ConvertTo mesh (Editable_Mesh)
			poly_total = mesh.numFaces
			mesh_total = 0
			IDlist = #()
			mesh_facelists=#()
			
			for j=1 to mesh.numFaces do
			(
				-- get id from face
				Curr_ID = getFaceMatID mesh j
				--format "Face: % matID: % \n" j Curr_ID
				-- have we found this ID before?  no?
				idfound = finditem IDlist Curr_ID
				if  idfound == 0 then
				(
					-- format "New MatID found on face %, building array to hold facelist\n" j
					append IDlist Curr_ID
					append mesh_facelists #(Curr_ID,#(j))
				)
				else
				(
					-- format "adding face % to facelist for ID %, index: % \n" j Curr_ID idfound 
					append mesh_facelists[idfound][2] j
				)
			)
			print mesh_facelists
			mesh_total = IDlist.count
			-- format "mesh_facelists[1][1] %\n" mesh_facelists[1][1]
			
			--========= Mesh / poly / uv Generation =========--
			
			if GetNumTVerts (selection[n].mesh) > 0 then
			(
				meshflags[1] = true
				totaltverts = getNumTVerts selection[n].mesh
				for i=1 to totaltverts do -- Mesh data loop
				(
					tvert = getTVert selection[n].mesh i
					format "vt % % #index: %\n" (tvert[1] * -1) tvert[2] i to:objfile
				)
			) else if GetNumTVerts (selection[n].mesh) == 0 then meshflags[1] = false
			
			for i=1 to IDlist.count do -- Mesh data loop
			(
				-- cw changes here:			
				face_list = mesh_facelists[i][2]
				face_bits = face_list as bitarray
				setFaceSelection mesh face_bits
				
				--_curSel = getFaceSelection mesh
				_curSel = face_list
				currentpoly = 0

			
				if (getNumCPVVerts (selection[n].mesh) > 0) then
				(
				meshflags[2] = true
				vcVerts = getNumCPVVerts selection[n].mesh
					for j=1 to vcverts do
						(
						vcface = getVertColor selection[n].mesh j -- should return an array, of size 3
						format "vc % % % %\n" (vcface.alpha as integer) (vcface.red as integer) (vcface.green as integer) (vcface.blue as integer) to:objfile
						)
				) else if GetNumCPVVerts (selection[n].mesh) == 0 then meshflags[2] = false
			)
			
			currentpoly = 0	
			
			for i=1 to IDlist.count do -- Mesh data loop
			(
					-- cw changes here:			
					face_list = mesh_facelists[i][2]
					face_bits = face_list as bitarray
					setFaceSelection mesh face_bits
				
					--_curSel = getFaceSelection mesh
					_curSel = face_list
					currentpoly = 0
					currID = getFaceMatID selection[n].mesh _curSel[1]
						format "\n" to:objfile
					curMeshMtlName = ""
					
					if(selection[n].material != undefined) do
					(
						if(getnumSubmtls selection[n].material > 0) then
						(
							curMeshMtlName = selection[n].material.materiallist[currID].name
						)
						else 
						(
							curMeshMtlName = selection[n].material.name
						)
						
						format "usemtl % \n" curMeshMtlName to:objfile
						for j=1 to _curSel.count do
						(
							p = getFace mesh _curSel[j]
							if meshflags[1] == true and meshflags[2] == true then
							(
								tvface = (getTVFace mesh _curSel[j]) -- should return an array of size 3
								vcface = (getVCFace mesh _curSel[j]) -- should return an array of size 3
								format "f %/%/%/% %/%/%/% %/%/%/% \n" (p[1] as integer) (tvface[1] as integer) (p[1] as integer) (vcface[1] as integer) (p[2] as integer) (tvface[2] as integer) (p[2] as integer) (vcface[2] as integer) (p[3] as integer) (tvface[3] as integer) (p[3] as integer) (vcface[3] as integer) to:objfile 
							)
							else if meshflags[1] == true and meshflags[2] == false then
							(
								tvface = (getTVFace mesh _curSel[j])
								format "f %/%/% %/%/% %/%/%\n" (p[1] as integer) (tvface[1] as integer) (p[1] as integer) (p[2] as integer) (tvface[2] as integer) (p[2] as integer) (p[3] as integer) (tvface[3] as integer) (p[3] as integer) to:objfile
							)
							else if meshflags[1] == false and meshflags[2] == false then
							(
								format "f %//% %//% %//%\n" (p[1] as integer) (p[1] as integer) (p[2] as integer) (p[2] as integer) (p[3] as integer) (p[3] as integer) to:objfile
							)
							else if meshflags[1] == false and meshflags[2] == true then
							(
								vcface = (getVCFace mesh _curSel[j])
								format "f %//%/% %//%/% %//%/%\n" (p[1] as integer) (p[1] as integer) (vcface[1] as integer) (p[2] as integer) (p[2] as integer) (vcface[2] as integer) (p[3] as integer) (p[3] as integer) (vcface[3] as integer) to:objfile
							)
						)
					)					
				)
				if InstanceMgr.Getinstances selection[n] &instances > 1 do -- if higher than 1, we know we have instanced nodes to handle
				(
					InstanceMgr.Getinstances selection[n] &instances /* don't forget that instances changes every loop */

					for curr_inst_count = 1 to instances.count do
					(
						current_instance = getNodeByName (instances[curr_inst_count].name)
						if(current_instance == undefined) do continue
						if(current_instance.name == selection[n].name) do continue
						
						for skipIndx = 1 to masterskiplist.count do
						(
							if(current_instance.name == masterskiplist[skipIndx]) do continue
						)
						
						/*exportedNodes = exportedNodes + 1
						append actualList current_instance.name*/
						format "instance %; #%\n" (n-1) current_instance.name to:scenefile
						format "pos % % %; \n" current_instance.pos[1] current_instance.pos[2] current_instance.pos[3] to:scenefile
						camRotEuler = quatToEuler2 current_instance.rotation.controller.value
						format "rot % % %; \n" camRotEuler.x camRotEuler.y camRotEuler.z to:scenefile
						/*------------------------------Exporting surface flags--------------------------------*/
						surfaceFlags = 00000000L
							
						solidSurfaceValue = getUserProp current_instance "solid"
						visibleSurfaceValue = getUserProp current_instance "visible"
						waterSurfaceValue = getUserProp current_instance "water"
						noFrictionSurfaceValue = getUserProp current_instance "nofriction"
						noAccelSurfaceValue = getUserProp current_instance "noacceleration"
						cannotLandSurfaceValue = getUserProp current_instance "cannotland"
						increaseAccelSurfaceValue = getUserProp current_instance "increasedacceleration"
						diggableSurfaceValue = getUserProp current_instance "diggable"
						unclimbableSurfaceValue = getUserProp current_instance "unclimbable"
						hurtSurfaceValue = getUserProp current_instance "hurt"
						footprintsSurfaceValue = getUserProp current_instance "footprints"
						
						print solidSurfaceValue
						print visibleSurfaceValue
						
						if(visibleSurfaceValue == undefined) then
						(
							surfaceFlags += 2147483648L -- 0x80 00 00 00
						)
						else if (visibleSurfaceValue == true) then
						(
							surfaceFlags += 2147483648L -- 0x80 00 00 00
						)
						
						if(solidSurfaceValue == undefined) then
						(
							surfaceFlags += 1L -- if no value is provided, assume they want solid. This seems to be the opposite mindset from the original SADX tools
						)
						else if (solidSurfaceValue == true) then
						(
							surfaceFlags += 1L
							print "Undefined - setting solid flag"
						)
						
						if(waterSurfaceValue != undefined) and (waterSurfaceValue == true) do
						(
							surfaceFlags += 2L
						)
						
						if(noFrictionSurfaceValue != undefined) and (noFrictionSurfaceValue == true) do
						(
							surfaceFlags += 8L
						)
						
						if(noAccelSurfaceValue != undefined) and (noAccelSurfaceValue == true) do
						(
							surfaceFlags += 8L
						)
						
						if(cannotLandSurfaceValue != undefined) and (cannotLandSurfaceValue) do
						(
							surfaceFlags += 64L
						)
						
						if(increaseAccelSurfaceValue != undefined) and (increaseAccelSurfaceValue == true) do
						(
							surfaceFlags += 128L
						)
						
						if(diggableSurfaceValue != undefined) and (diggableSurfaceValue == true) do
						(
							surfaceFlags += 128L
						)
						
						if(unclimbableSurfaceValue != undefined) and (unclimbableSurfaceValue == true) do
						(
							surfaceFlags += 4096L
						)
						
						if(hurtSurfaceValue != undefined) and (hurtSurfaceValue == true) do
						(
							surfaceFlags += 65536L
						)
						
						if(footprintsSurfaceValue != undefined) and (footprintsSurfaceValue == true) do
						(
							surfaceFlags += 1048576L
						)
							
						surfaceFlagsString = formattedPrint surfaceflags format:"#010X"
						format "surfaceflags %;\n\n" surfaceFlagsString to:scenefile
						currentname = instances[curr_inst_count].name
						append masterskiplist currentname
					)
				)
			)
			else format "skipped node %\n" n
		/*----------------END MESH EXPORT---------------*/
		
		fflush objfile
		fclose objfile
		) -- done with all nodes
		
		flush mtlfile
		close mtlfile
		flush scenefile
		close scenefile
		
		-- debug code to check expected output against actual output
		/*if(targetNodeCount != exportedNodes) do
		(
			Messagebox "count mismatch - check scene for duplicate names" width: 500
		)*/
		
		messagebox ("Done Exporting!")
		gc light:false
	)
	on btn_browse pressed do
	(
		filename = getSavePath caption:"Select a folder to export compiled model data to:" initialDir:(getDir #maxroot)
	
		if (filename != undefined) then
		outpath.text = filename
		
		if (filename == undefined) then
		messagebox "Please input a directory"
	)
)

