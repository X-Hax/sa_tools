-- Sonic Adventure Level Import  --

-- Structs --
struct LANDTABLE_STRUCT (
	COL_Count,		-- COL Object Count
	ANIM_Count,		-- ANIM Object Count
	FLAGS,				-- LandTable Flags for what info exists.
	UNKNOWN,			-- Unknown Float, typically 3000
	COL_Array,			-- Pointer to COL Array.
	ANIM_Array,		-- Pointer to ANIM Array.
	TEX_FILENAME,	-- Pointer to Texture Filename (Optional)
	TEX_LIST,			-- Pointer to Texture List
	INT1,					-- Unknown Int32
	INT2					-- Unknown Int32
)

struct COL_STRUCT (
	BOUNDS_POS,		-- Bounding Sphere Position
	BOUNDS_RADIUS,	-- Bounding Sphere Size
	INT1,					-- Unknown Int32
	INT2,					-- Unknown Int32
	NJS_OBJECT,		-- Pointer to NJS_OBJECT
	NULL,					-- Null Int32
	FLAGS				-- Surface Flags
)

struct ANIM_STRUCT (
	UnkInt1,			-- Unknown Int32
	UnkFloat1,		-- Unknown Float
	UnkFloat2,		-- Unknown Float
	NJS_OBJECT,	-- Pointer to NJS_OBJECT
	NJS_MOTION,	-- Pointer to NJS_MOTION
	UnkInt2			-- Unknown Int32
)

struct NJS_OBJECT_STRUCT (
	FLAGS,			-- NJS_OBJECT Flags
	NJS_MODEL,	-- Pointer to NJS_MODEL
	POS,				-- Position
	ROT,				-- Rotation
	SCA,				-- Scale
	NJS_CHILD,		-- Pointer to NJS_CHILD (Optional)
	NJS_SIBLING	-- Pointer to NJS_SIBLING (Optional)
)

struct NJS_MODEL_STRUCT (
	NJS_VERTEX,			-- Pointer to NJS_VERTEX
	NJS_NORMALS,		-- Pointer to NJS_NORMALS
	VERTEX_Count,		-- Vertex Count
	NJS_MESHSET,		-- Pointer to NJS_MESHSET
	NJS_MATERIAL,		-- Pointer to NJS_MATERIAL
	MESHSET_Count,		-- MESHSET Count (Int16)
	MATERIAL_Count,	-- MATERIAL Count (Int16)
	PIV,						-- Pivot X
	RADIUS					-- Radius
)

struct NJS_MESHSET_STRUCT (
	NJS_MATERIAL_ID,	-- NJS_MATERIAL ID
	NJS_POLY_Type,		-- NJS_POLY Type
	NJS_POLY_Count,	-- NJS_POLY Totals
	NJS_POLY,				-- Pointer to NJS_POLY
	NJS_PATTR,			-- Pointer to NJS_PATTR
	NJS_POLYNORMAL,	-- Pointer to NJS_POLYNORMAL
	NJS_VERTEXCOLOR,	-- Pointer to NJS_VERTEXCOLOR
	NJS_UV					-- Pointer to NJS_UV
)

struct NJS_MATERIAL_STRUCT (
	DIFFUSE_B,		-- Diffuse Blue
	DIFFUSE_G,	-- Diffuse Green
	DIFFUSE_R,		-- Diffuse Red
	DIFFUSE_A,	-- Diffuse Alpha
	SPECULAR_B,	-- Specular Blue
	SPECULAR_G,	-- Specular Green
	SPECULAR_R,	-- Specular Red
	SPECULAR_A,	-- Specular Alpha
	EXPONENT,		-- Exponent
	TEXTURE_ID,	-- Texture ID
	UNUSED,		-- Three Bytes, must be skipped
	FLAGS			-- NJS_MATERIAL Flags
)

struct VERTCOLOR (
	BB,	-- Vertex Color Blue
	GG,	-- Vertex Color Green
	RR,	-- Vertex Color Red
	AA	-- Vertex Color Alpha
)

--MaxScript Functions--
fn ClearSGs =
(
	try
	(
		local numFaces = polyop.getNumfaces $
		local faces = (for i in 1 to numFaces collect i) as bitarray
		polyOp.setFaceSmoothGroup $ faces 1
	)
	catch
	(
	)
)

fn SKIP fstream fixedLength = (
	for i = 1 to fixedLength do (
		ReadByte fstream
	)
)

--SA Functions--
fn DEGREES fstream = (
	ANG_X = (((ReadShort fstream#unsigned as float)/65536.0)*360.0)
	ANG2_X = ReadShort fstream
	ANG_Y = (((ReadShort fstream#unsigned as float)/65536.0)*360.0)
	ANG2_Y = ReadShort fstream
	ANG_Z = (((ReadShort fstream#unsigned as float)/65536.0)*360.0)
	ANG2_Z = ReadShort fstream
	ANG = eulerangles ANG_X ANG_Y ANG_Z
	return ANG
)

fn VERTEX fstream = (
	Vertex_X = ReadFloat fstream
	Vertex_Y = ReadFloat fstream
	Vertex_Z = ReadFloat fstream
	VERT = Point3 Vertex_X Vertex_Y Vertex_Z
	return VERT
)

fn ReadUVs fstream = (
	(ReadShort fstream as float)/255
)

-- Import Functions --
fn importSA1LVLFile = (
	start = timestamp()
	fsource = GetOpenFileName \
	caption:"SA Importer" \
	types: "sa1lvl Files (*.sa1lvl)|*.sa1lvl|"

	if fsource != undefined Do (
		f = fopen fsource "rb"
		fseek f 8#seek_set
		LandTable_Pointer = ReadLong f
		
		fseek f LandTable_Pointer#seek_set

		LandTable=(LANDTABLE_STRUCT \
			COL_Count: (ReadShort f) \
			ANIM_Count: (ReadShort f) \
			FLAGS: (ReadLong f) \
			UNKNOWN: (ReadFloat f) \
			COL_Array: (ReadLong f) \
			ANIM_Array: (ReadLong f) \
			TEX_FILENAME: (ReadLong f) \
			TEX_LIST: (ReadLong f) \
			INT1: (ReadLong f) \
			INT2: (ReadLong f) \
		)
		
		If LandTable.COL_Count != 0 Do (
			fseek f LandTable.COL_Array#seek_set
			COL_Array = #()
			for cc = 1 to LandTable.COL_Count Do (
				COL=(COL_STRUCT \
					BOUNDS_POS: (VERTEX f) \
					BOUNDS_RADIUS: (ReadFloat f) \
					INT1: (ReadLong f) \
					INT2: (ReadLong f) \
					NJS_OBJECT: (ReadLong f) \
					NULL: (ReadLong f) \
					FLAGS: (ReadLong f) \
				)
				append COL_Array(COL)
			)
		)
				
		If LandTable.ANIM_Count != 0 Do (
			fseek f LandTable.ANIM_Array#seek_set
			ANIM_Array = #()
			for cc = 1 to LandTable.ANIM_Count Do (
				ANIM=(ANIM_Struct \
					UnkInt1: (ReadLong f) \
					UnkFloat1: (ReadFloat f) \
					UnkFloat2: (ReadFloat f) \
					NJS_OBJECT: (ReadLong f) \
					NJS_MOTION: (ReadLong f) \
					UnkInt2: (ReadLong f) \
				)
				append ANIM_Array(ANIM)
			)
		)
		
		ColliLayer = LayerManager.newLayerFromName "Colli"
		ColliLayer = LayerManager.getLayerFromName "Colli"
		If COL_Array != undefined Do (
			disableSceneRedraw()
			For cc = 1 to COL_Array.count Do (
				VERT_Array = #()
				NORMALS_Array = #()
				MSH_Array = #()
				UVSET_Array = #()
				MATERIALS_Array = #()
				MESHSET_Array = #()
				MeshArray = #()
				fseek f COL_Array[cc].NJS_OBJECT#seek_set
				NJS_OBJECT=(NJS_OBJECT_STRUCT \
					FLAGS: (ReadLong f) \
					NJS_MODEL: (ReadLong f) \
					POS: (VERTEX f) \
					ROT: (DEGREES f) \
					SCA: (VERTEX f) \
					NJS_CHILD: (ReadLong f) \
					NJS_SIBLING: (ReadLong f) \
				)
		
				If NJS_OBJECT.NJS_MODEL != 0 Do (
					fseek f NJS_OBJECT.NJS_MODEL#seek_set
					NJS_MODEL=(NJS_MODEL_STRUCT \
						NJS_VERTEX: (ReadLong f) \
						NJS_NORMALS: (ReadLong f) \
						VERTEX_Count: (ReadLong f) \
						NJS_MESHSET: (ReadLong f) \
						NJS_MATERIAL: (ReadLong f) \
						MESHSET_Count: (ReadShort f) \
						MATERIAL_Count: (ReadShort f) \
						PIV: (VERTEX f) \
						RADIUS: (ReadFloat f) \
					)
			
					fseek f NJS_MODEL.NJS_VERTEX#seek_set
					For vv = 1 to NJS_MODEL.VERTEX_Count Do (
						VERTICE = VERTEX f
						append VERT_Array(VERTICE)
					)
					fseek f NJS_MODEL.NJS_NORMALS#seek_set
					For nn = 1 to NJS_MODEL.VERTEX_Count Do (
						NORMAL = VERTEX f
						append NORMALS_Array(NORMAL)
					)
					fseek f NJS_MODEL.NJS_MESHSET#seek_set
					For mm = 1 to NJS_MODEL.MESHSET_Count Do (
						NJS_MESHSET=(NJS_MESHSET_STRUCT \
						NJS_MATERIAL_ID: (ReadByte f#unsigned+1) \
						NJS_POLY_Type: (ReadByte f#unsigned) \
						NJS_POLY_Count: (ReadShort f) \
						NJS_POLY: (ReadLong f) \
						NJS_PATTR: (ReadLong f) \
						NJS_POLYNORMAL: (ReadLong f) \
						NJS_VERTEXCOLOR: (ReadLong f) \
						NJS_UV: (ReadLong f) \
						)
						append MESHSET_Array(NJS_MESHSET)
					)
					fseek f NJS_MODEL.NJS_MATERIAL#seek_set
					For ma = 1 to NJS_MODEL.MATERIAL_Count Do (
						NJS_MATERIAL=(NJS_MATERIAL_STRUCT \
							Diffuse_B: (ReadByte f#unsigned) \
							Diffuse_G: (ReadByte f#unsigned) \
							Diffuse_R: (ReadByte f#unsigned) \
							Diffuse_A: (ReadByte f#unsigned) \
							Specular_B: (ReadByte f#unsigned) \
							Specular_G: (ReadByte f#unsigned) \
							Specular_R: (ReadByte f#unsigned) \
							Specular_A: (ReadByte f#unsigned) \
							Exponent: (ReadFloat f) \
							Texture_ID: (ReadByte f + 1) \
							UNUSED: (SKIP f 3) \
							FLAGS: (ReadLong f) \
						)
						append MATERIALS_Array(NJS_MATERIAL)
					)
					For ms = 1 to MESHSET_Array.count Do (
						Face_Array = #()
						UV_Array = #()
						UVIndex = #()
						UVTotal = 0
						UVID = 0
						fseek f MESHSET_Array[ms].NJS_POLY#seek_set
						Case of (
							(MESHSET_Array[ms].NJS_POLY_TYPE == 0x00):(
								For fs = 1 to MESHSET_Array[ms].NJS_POLY_Count Do (
									fa = ReadShort f#unsigned +1
									fb = ReadShort f#unsigned +1
									fc = ReadShort f#unsigned +1
									u1 = UVID + 1
									u2 = UVID + 2
									u3 = UVID + 3
									append Face_Array[fa,fb,fc]
									append UVIndex[u1,u2,u3]
									UVID += 3
								)
								UVTotal = MESHSET_Array[ms].NJS_POLY_Count
							)
							(MESHSET_Array[ms].NJS_POLY_TYPE == 0x40):(
								For fs = 1 to MESHSET_Array[ms].NJS_POLY_Count Do (
									fa = ReadShort f#unsigned +1
									fb = ReadShort f#unsigned +1
									fc = ReadShort f#unsigned +1
									fd = ReadShort f#unsigned +1
									u1 = UVID + 1
									u2 = UVID + 2
									u3 = UVID + 3
									u4 = UVID + 4
									append Face_Array[fa,fb,fc]
									append UVIndex[u1,u2,u3]
									append Face_Array[fb,fd,fc]
									append UVIndex[u2,u4,u3]
									UVID += 4
								)
								UVTotal = MESHSET_Array[ms].NJS_POLY_Count
							)
							(MESHSET_Array[ms].NJS_POLY_TYPE == 0xC0):(
								For fs = 1 to MESHSET_Array[ms].NJS_POLY_Count Do (
									StripTotal = ReadByte f
									StripDir = ReadByte f#unsigned
									If StripDir == 0x00 Do (StripDir = -1)
									StartDirection = StripDir
									fa = ReadShort f#unsigned + 1
									fb = ReadShort f#unsigned + 1
									u1 = UVTotal + 1
									u2 = UVTotal + 2
									FaceDirection = StartDirection
									IndexCounter = 2
									Do (
										fc = ReadShort f#unsigned 
										IndexCounter += 1
										u3 = UVTotal + IndexCounter
										if (fc==0xFFFF) then (
											fa = ReadShort f#unsigned + 1
											fb = ReadShort f#unsigned + 1
											FaceDirection = StartDirection*1
											IndexCounter += 2
										) else (
											fc += 1
											FaceDirection *= -1
											if (fa!=fb)AND(fb!=fc)AND(fc!=fa) then (
												if FaceDirection > 0 then (
													append Face_Array[fa,fb,fc]
													append UVIndex[u1,u2,u3]
												) else (
													append Face_Array[fa,fc,fb]
													append UVIndex[u1,u3,u2]
												)
											)
											fa = fb
											u1 = u2
											fb = fc
											u2 = u3
										)
									)
									while IndexCounter < StripTotal
									UVTotal += StripTotal
								)
							)
						)
						if MESHSET_Array[ms].NJS_UV != 0 Do (
							fseek f MESHSET_Array[ms].NJS_UV#seek_set
							For uvw = 1 to UVTotal Do (
								tu = ReadUVs f
								tv = ReadUVs f*-1
								append UV_Array[tu,tv,0]
							)
						)
				--print Face_Array
						msh = mesh vertices:Vert_Array faces:Face_Array
						if MESHSET_Array[ms].NJS_UV != 0 Do (
							msh.numTVerts = UV_Array.count
							buildTVFaces msh
							for j = 1 to UV_Array.count do setTVert msh j UV_Array[j]
							for j = 1 to UVIndex.count do setTVFace msh j UVIndex[j]
						)
						msh.Name = "object_" + (bit.IntAsHex COL_Array[cc].NJS_OBJECT)
						move msh NJS_OBJECT.POS
						rotate msh NJS_OBJECT.ROT
				
				
						msh = convertTo msh PolyMeshObject
						If COL_Array[cc].FLAGS < 0 Do (
							msh.material = meditmaterials[1]
							msh.editablePoly.SetSelection #face #{1..Face_array.count}
							polyop.setFaceMatID msh (polyop.getFaceSelection msh) MATERIALS_Array[MESHSET_Array[ms].NJS_MATERIAL_ID].Texture_ID
							clearSelection()
						)
						append MeshArray msh
					)
				)
				selectmore MeshArray
				while selection.count > 1 do
				(
					selcount = selection.count
					for i = selcount to 2 by -2 do
					(
						polyop.attach selection[i] selection[i-1]
					)
				)
				update selection[1]
				clearSelection()
			)
		)
		OBJArray = objects as array
		
		For o = 1 to OBJArray.count Do (
			select OBJArray[o]
			maxVerts = $.numVerts
			$.EditablePoly.SetSelection #Vertex #{1..maxVerts}
			$.weldThreshold = 0.001
			$.EditablePoly.weldFlaggedVertices ()
			ClearSGs ()
			If $.mat == undefined Do (
				ColliLayer.addNode $
			)
			clearselection()
		)
		enableSceneRedraw()
		fclose f
	)
	actionMan.executeAction 0 "311"  -- Tools: Zoom Extents All Selected
	end = timestamp()
	Print ("Level Loaded in " + (((end - start)/1000.0)as string) + " seconds!")
)

fn importSABinaryFile = (
	if (heapSize < 40000000) then
	heapSize = 400000000 
	start = timestamp()
	fsource = GetOpenFileName \
	caption:"SA Importer" \
	types: "Binary Files (*.bin)|*.bin| Executable Files (*.exe)|*.exe|"

	if fsource != undefined Do (
-- 		f = fopen fsource "rb"
-- 		Case Of (
-- 			(ddl1 == "Dreamcast Stage Binary"):(
-- 				key = 0xC900000
-- 			)
-- 			(ddl1 == "SADX Executable"):(
-- 				key = 0x4000000
-- 			)
-- 		)
-- 		fseek f addr#seek_set

		LandTable=(LANDTABLE_STRUCT \
			COL_Count: (ReadShort f) \
			ANIM_Count: (ReadShort f) \
			FLAGS: (ReadLong f) \
			UNKNOWN: (ReadFloat f) \
			COL_Array: (ReadLong f - key) \
			ANIM_Array: (ReadLong f - key) \
			TEX_FILENAME: (ReadLong f - key) \
			TEX_LIST: (ReadLong f - key) \
			INT1: (ReadLong f) \
			INT2: (ReadLong f) \
		)
		
		If LandTable.COL_Count != 0 Do (
			fseek f LandTable.COL_Array#seek_set
			COL_Array = #()
			for cc = 1 to LandTable.COL_Count Do (
				COL=(COL_STRUCT \
					BOUNDS_POS: (VERTEX f) \
					BOUNDS_RADIUS: (ReadFloat f) \
					INT1: (ReadLong f) \
					INT2: (ReadLong f) \
					NJS_OBJECT: (ReadLong f - key) \
					NULL: (ReadLong f) \
					FLAGS: (ReadLong f) \
				)
				append COL_Array(COL)
			)
		)
				
		If LandTable.ANIM_Count != 0 Do (
			fseek f LandTable.ANIM_Array#seek_set
			ANIM_Array = #()
			for cc = 1 to LandTable.ANIM_Count Do (
				ANIM=(ANIM_Struct \
					UnkInt1: (ReadLong f) \
					UnkFloat1: (ReadFloat f) \
					UnkFloat2: (ReadFloat f) \
					NJS_OBJECT: (ReadLong f - key) \
					NJS_MOTION: (ReadLong f - key) \
					UnkInt2: (ReadLong f) \
				)
				append ANIM_Array(ANIM)
			)
		)
		
		ColliLayer = LayerManager.newLayerFromName "Colli"
		ColliLayer = LayerManager.getLayerFromName "Colli"
		If COL_Array != undefined Do (
			disableSceneRedraw()
			For cc = 1 to COL_Array.count Do (
				VERT_Array = #()
				NORMALS_Array = #()
				MSH_Array = #()
				UVSET_Array = #()
				MATERIALS_Array = #()
				MESHSET_Array = #()
				MeshArray = #()
				fseek f COL_Array[cc].NJS_OBJECT#seek_set
				NJS_OBJECT=(NJS_OBJECT_STRUCT \
					FLAGS: (ReadLong f) \
					NJS_MODEL: (ReadLong f - key) \
					POS: (VERTEX f) \
					ROT: (DEGREES f) \
					SCA: (VERTEX f) \
					NJS_CHILD: (ReadLong f - key) \
					NJS_SIBLING: (ReadLong f - key) \
				)
		
				If NJS_OBJECT.NJS_MODEL != 0 Do (
					fseek f NJS_OBJECT.NJS_MODEL#seek_set
					NJS_MODEL=(NJS_MODEL_STRUCT \
						NJS_VERTEX: (ReadLong f - key) \
						NJS_NORMALS: (ReadLong f - key) \
						VERTEX_Count: (ReadLong f) \
						NJS_MESHSET: (ReadLong f - key) \
						NJS_MATERIAL: (ReadLong f - key) \
						MESHSET_Count: (ReadShort f) \
						MATERIAL_Count: (ReadShort f) \
						PIV: (VERTEX f) \
						RADIUS: (ReadFloat f) \
					)
			
					fseek f NJS_MODEL.NJS_VERTEX#seek_set
					For vv = 1 to NJS_MODEL.VERTEX_Count Do (
						VERTICE = VERTEX f
						append VERT_Array(VERTICE)
					)
					fseek f NJS_MODEL.NJS_NORMALS#seek_set
					For nn = 1 to NJS_MODEL.VERTEX_Count Do (
						NORMAL = VERTEX f
						append NORMALS_Array(NORMAL)
					)
					fseek f NJS_MODEL.NJS_MESHSET#seek_set
					For mm = 1 to NJS_MODEL.MESHSET_Count Do (
						NJS_MESHSET=(NJS_MESHSET_STRUCT \
						NJS_MATERIAL_ID: (ReadByte f#unsigned+1) \
						NJS_POLY_Type: (ReadByte f#unsigned) \
						NJS_POLY_Count: (ReadShort f) \
						NJS_POLY: (ReadLong f - key) \
						NJS_PATTR: (ReadLong f - key) \
						NJS_POLYNORMAL: (ReadLong f - key) \
						NJS_VERTEXCOLOR: (ReadLong f - key) \
						NJS_UV: (ReadLong f - key) \
						)
						append MESHSET_Array(NJS_MESHSET)
					)
					fseek f NJS_MODEL.NJS_MATERIAL#seek_set
					For ma = 1 to NJS_MODEL.MATERIAL_Count Do (
						NJS_MATERIAL=(NJS_MATERIAL_STRUCT \
							Diffuse_B: (ReadByte f#unsigned) \
							Diffuse_G: (ReadByte f#unsigned) \
							Diffuse_R: (ReadByte f#unsigned) \
							Diffuse_A: (ReadByte f#unsigned) \
							Specular_B: (ReadByte f#unsigned) \
							Specular_G: (ReadByte f#unsigned) \
							Specular_R: (ReadByte f#unsigned) \
							Specular_A: (ReadByte f#unsigned) \
							Exponent: (ReadFloat f) \
							Texture_ID: (ReadByte f + 1) \
							UNUSED: (SKIP f 3) \
							FLAGS: (ReadLong f) \
						)
						append MATERIALS_Array(NJS_MATERIAL)
					)
					For ms = 1 to MESHSET_Array.count Do (
						Face_Array = #()
						UV_Array = #()
						UVIndex = #()
						UVTotal = 0
						UVID = 0
						fseek f MESHSET_Array[ms].NJS_POLY#seek_set
						Case of (
							(MESHSET_Array[ms].NJS_POLY_TYPE == 0x00):(
								For fs = 1 to MESHSET_Array[ms].NJS_POLY_Count Do (
									fa = ReadShort f#unsigned +1
									fb = ReadShort f#unsigned +1
									fc = ReadShort f#unsigned +1
									u1 = UVID + 1
									u2 = UVID + 2
									u3 = UVID + 3
									append Face_Array[fa,fb,fc]
									append UVIndex[u1,u2,u3]
									UVID += 3
								)
								UVTotal = MESHSET_Array[ms].NJS_POLY_Count
							)
							(MESHSET_Array[ms].NJS_POLY_TYPE == 0x40):(
								For fs = 1 to MESHSET_Array[ms].NJS_POLY_Count Do (
									fa = ReadShort f#unsigned +1
									fb = ReadShort f#unsigned +1
									fc = ReadShort f#unsigned +1
									fd = ReadShort f#unsigned +1
									u1 = UVID + 1
									u2 = UVID + 2
									u3 = UVID + 3
									u4 = UVID + 4
									append Face_Array[fa,fb,fc]
									append UVIndex[u1,u2,u3]
									append Face_Array[fb,fd,fc]
									append UVIndex[u2,u4,u3]
									UVID += 4
								)
								UVTotal = MESHSET_Array[ms].NJS_POLY_Count
							)
							(MESHSET_Array[ms].NJS_POLY_TYPE == 0xC0):(
								For fs = 1 to MESHSET_Array[ms].NJS_POLY_Count Do (
									StripTotal = ReadByte f
									StripDir = ReadByte f#unsigned
									If StripDir == 0x00 Do (StripDir = -1)
									StartDirection = StripDir
									fa = ReadShort f#unsigned + 1
									fb = ReadShort f#unsigned + 1
									u1 = UVTotal + 1
									u2 = UVTotal + 2
									FaceDirection = StartDirection
									IndexCounter = 2
									Do (
										fc = ReadShort f#unsigned 
										IndexCounter += 1
										u3 = UVTotal + IndexCounter
										if (fc==0xFFFF) then (
											fa = ReadShort f#unsigned + 1
											fb = ReadShort f#unsigned + 1
											FaceDirection = StartDirection*1
											IndexCounter += 2
										) else (
											fc += 1
											FaceDirection *= -1
											if (fa!=fb)AND(fb!=fc)AND(fc!=fa) then (
												if FaceDirection > 0 then (
													append Face_Array[fa,fb,fc]
													append UVIndex[u1,u2,u3]
												) else (
													append Face_Array[fa,fc,fb]
													append UVIndex[u1,u3,u2]
												)
											)
											fa = fb
											u1 = u2
											fb = fc
											u2 = u3
										)
									)
									while IndexCounter < StripTotal
									UVTotal += StripTotal
								)
							)
						)
						if MESHSET_Array[ms].NJS_UV != 0 Do (
							fseek f MESHSET_Array[ms].NJS_UV#seek_set
							For uvw = 1 to UVTotal Do (
								tu = ReadUVs f
								tv = ReadUVs f*-1
								append UV_Array[tu,tv,0]
							)
						)
				--print Face_Array
						msh = mesh vertices:Vert_Array faces:Face_Array
						if MESHSET_Array[ms].NJS_UV != 0 Do (
							msh.numTVerts = UV_Array.count
							buildTVFaces msh
							for j = 1 to UV_Array.count do setTVert msh j UV_Array[j]
							for j = 1 to UVIndex.count do setTVFace msh j UVIndex[j]
						)
						msh.Name = "object_" + (bit.IntAsHex COL_Array[cc].NJS_OBJECT)
						move msh NJS_OBJECT.POS
						rotate msh NJS_OBJECT.ROT
				
				
						msh = convertTo msh PolyMeshObject
						If COL_Array[cc].FLAGS < 0 Do (
							msh.material = meditmaterials[1]
							msh.editablePoly.SetSelection #face #{1..Face_array.count}
							polyop.setFaceMatID msh (polyop.getFaceSelection msh) MATERIALS_Array[MESHSET_Array[ms].NJS_MATERIAL_ID].Texture_ID
							clearSelection()
						)
						append MeshArray msh
					)
				)
				selectmore MeshArray
				while selection.count > 1 do
				(
					selcount = selection.count
					for i = selcount to 2 by -2 do
					(
						polyop.attach selection[i] selection[i-1]
					)
				)
				update selection[1]
				clearSelection()
			)
		)
		OBJArray = objects as array
		
		For o = 1 to OBJArray.count Do (
			select OBJArray[o]
			maxVerts = $.numVerts
			$.EditablePoly.SetSelection #Vertex #{1..maxVerts}
			$.weldThreshold = 0.001
			$.EditablePoly.weldFlaggedVertices ()
			ClearSGs ()
			If $.mat == undefined Do (
				ColliLayer.addNode $
			)
			clearselection()
		)
		enableSceneRedraw()
		fclose f
	)
	actionMan.executeAction 0 "311"  -- Tools: Zoom Extents All Selected
	end = timestamp()
	Print ("Level Loaded in " + (((end - start)/1000.0)as string) + " seconds!")
)

--GUI Items --
keys = #(
	"Dreamcast Stage Binary\n",
	"SADX Executable\n"
)

rollout binRoll "Import Adventure Binary File" width:320 height:112
(
	dropdownList ddl1 "Binary Key Selection" pos:[10,14] width:140 height:40 items:keys
	edittext addr "" pos:[165,34] width:140 height:20
	label lbl31 "LandTable Address" pos:[165,14] width:150 height:14
	button btn21 "Select Binary" pos:[10,65] width:295 height:40 enabled:false
	on ddl1 selected sel do
(
	keysel = sel
	)
	on addr entered text do
(
	LandTableAddr = addr
	)
)
