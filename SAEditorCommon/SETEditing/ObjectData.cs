using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

using SADXPCTools;

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
        [IniName("XRot")]
        public string XRotString;
        [IniIgnore]
        public int? XRot { get { return XRotString == null ? null : (int?)int.Parse(XRotString, System.Globalization.NumberStyles.HexNumber); } set { XRotString = value.HasValue ? null : value.Value.ToString("X"); } }
        [IniName("YRot")]
        public string YRotString;
        [IniIgnore]
        public int? YRot { get { return YRotString == null ? null : (int?)int.Parse(YRotString, System.Globalization.NumberStyles.HexNumber); } set { YRotString = value.HasValue ? null : value.Value.ToString("X"); } }
        [IniName("ZRot")]
        public string ZRotString;
        [IniIgnore]
        public int? ZRot { get { return ZRotString == null ? null : (int?)int.Parse(ZRotString, System.Globalization.NumberStyles.HexNumber); } set { ZRotString = value.HasValue ? null : value.Value.ToString("X"); } }
        public Dictionary<string, string> CustomProperties;
    }
}
