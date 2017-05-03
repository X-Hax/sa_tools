clearlistener()

gsource = GetOpenFileName \
caption:"Indexed Texture File" \
types: "index.txt (*.txt)|*.txt|"

if ((gsource)!=undefined) do(
	g = openfile gsource
	gpath = GetFileNamePath gsource
	TexCount = 0
	while not eof g do (
		readline g
		TexCount += 1
	)
	seek g 0
	meditMaterials[1] = MultiMaterial numsubs:TexCount
	For tx = 1 to TexCount do (
		gbix = ReadDelimitedString g ","
		TexName = ReadDelimitedString g "."
		pngext = ReadChars g 3
		meditMaterials[1].materialList[tx].diffuseMap = Bitmaptexture fileName:((gpath)+(TexName)+(".png"))
		meditMaterials[1].names[tx] = TexName
		meditMaterials[1].materialList[tx].name = TexName
		meditMaterials[1].materialList[tx].ShowInViewport = on
		meditMaterials[1].materialList[tx].ambient = color 178 178 178
		meditMaterials[1].materialList[tx].Diffuse = color 178 178 178
		meditmaterials[1].materialList[tx].Specular = color 255 255 255
		meditMaterials[1].materialList[tx].specularLevel = 0
		meditMaterials[1].materialList[tx].glossiness = 0
		meditMaterials[1].materialList[tx].Soften = 0
		meditMaterials[1].materialList[tx].opacity = 100
		meditMaterials[1].materialList[tx].selfIllumAmount = 0
		meditMaterials[1].materialList[tx].shaderType = 1
		meditMaterials[1].materialList[tx].adTextureLock = on
	)
	pvmName = filterstring gpath "\\"
	pvmName = pvmName[pvmName.count]
	meditMaterials[1].name = pvmName
	close g
)