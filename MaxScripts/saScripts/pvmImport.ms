fn ImportPVMFile = (
	gsource = GetOpenFileName \
	caption:"Indexed PVM File" \
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
			meditMaterials[1].materialList[tx].ShowInViewport = on
			meditMaterials[1].materialList[tx].shaderType = 4
			--meditMaterials[1].materialList[tx].Specular = color 175 175 175
			--meditMaterials[1].materialList[tx].glossiness = 0
		)
		pvmName = filterstring gpath "\\"
		pvmName = pvmName[pvmName.count]
		meditMaterials[1].name = pvmName
		print "Done!"
		close g
	)
)

