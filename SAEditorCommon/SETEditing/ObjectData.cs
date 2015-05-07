using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

using SA_Tools;
using System.ComponentModel;

namespace SonicRetro.SAModel.SAEditorCommon.SETEditing
{
    public class ObjectData
    {
        [IniName("codefile")]
        public string CodeFile;
        [IniName("codetype")]
        public string CodeType;
        public string Name;
        public string Model;
        public string Texture;
        public float? XPos, YPos, ZPos, XScl, YScl, ZScl;
		[TypeConverter(typeof(Int32HexConverter))]
		public int? XRot, YRot, ZRot;
        public Dictionary<string, string> CustomProperties;
    }
}
