using System.Collections.Generic;
using System.ComponentModel;
using SA_Tools;

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
    }
}
