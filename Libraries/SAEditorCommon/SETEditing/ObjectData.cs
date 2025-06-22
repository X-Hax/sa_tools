﻿using System.Collections.Generic;
using System.ComponentModel;
using SplitTools;

namespace SAModel.SAEditorCommon.SETEditing
{
	public class ObjectData
	{
		[IniName("codefile")]
		public string CodeFile;
		[IniName("codetype")]
		public string CodeType;
		public string Name;
		public string Model;
		[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
		public List<string> TexturePacks = [];
		public string Texture;
		public string Texlist;
		public float? XPos, YPos, ZPos, XScl, YScl, ZScl, DefXScl, DefYScl, DefZScl, GndDst, AddXScl, AddYScl, AddZScl;
		[TypeConverter(typeof(Int32HexConverter))]
		public int? XRot, YRot, ZRot;
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort? DefXRot, DefYRot, DefZRot, AddXRot, AddYRot, AddZRot;
		public string RotType;
		public string SclType;
	}
}
