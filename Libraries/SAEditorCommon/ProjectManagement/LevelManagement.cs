using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using SplitTools;

namespace SAEditorCommon.LevelManagement
{
	#region Stage Management
	public class StageManager
	{
		public class StgInfo
		{
			[XmlAttribute("StgID", typeof(string))]
			public string StgID { get; set; }
			[XmlAttribute("Name", typeof(string))]
			public string Name { get; set; }
			[XmlAttribute("ActTotal", typeof(int))]
			public int ActTotal { get; set; }
			[XmlAttribute("StgRoot", typeof(string))]
			public string StgRoot { get; set; }
			[XmlAttribute("ObjTexLists", typeof(string))]
			public string ObjTexList { get; set; }
		}

		public class ActInfo
		{
			[XmlAttribute("ActID", typeof(string))]
			public string ActID { get; set; }
			[XmlAttribute("Landtable", typeof(string))]
			public string Landtable { get; set; }
			[XmlAttribute("ActTexList", typeof(string))]
			public string ActTexList { get; set; }
			[XmlAttribute("ObjList", typeof(string))]
			public string ObjList { get; set; }
			[XmlAttribute("KillColli", typeof(string))]
			public string KillColli { get; set; }
			[XmlAttribute("UseCodeItem", typeof(bool))]
			public bool UseCodeItem { get; set; }
			[XmlAttribute("CodeItems", typeof(string))]
			public List<string> CodeItems { get; set; }
		}

		public class CharInfo
		{
			[XmlAttribute("Name", typeof(string))]
			public string Name { get; set; }
			[XmlAttribute("CharID", typeof(int))]
			public int CharID { get; set; }
			[XmlAttribute("CharTexList", typeof(string))]
			public string CharTexList { get; set; }
			[XmlAttribute("Height", typeof(float))]
			public float Height { get; set; }
			[XmlAttribute("StartPosList", typeof(string))]
			public string StartPosList { get; set; }
		}

		public class StageTemplate
		{
			[XmlAttribute("StageInfo", typeof(StgInfo))]
			public StgInfo StageInfo { get; set; }
			[XmlAttribute("ActInfo", typeof(ActInfo))]
			public List<ActInfo> ActInfos { get; set; }
		}

		public class CharacterTemplate
		{
			[XmlAttribute("CharData", typeof(CharInfo))]
			public List<CharInfo> CharData { get; set; }
		}
	}
	#endregion

	#region Object Management
	public class ObjectManager
	{
		public enum RotType
		{
			XYZ,
			Y,
			ZYX,
			NoRotation
		}

		public enum SclType
		{
			XYZ,
			X,
			Y,
			Z,
			XY,
			YZ,
			XZ,
			NoScale
		}

		public class classAltVariables
		{
			public int AltRotX { get; set; }
			public int AltRotY { get; set; }
			public int AltRotZ { get; set; }
			public float AltScaleX { get; set; }
			public float AltScaleY { get; set; }
			public float AltScaleZ { get; set; }
		}

		public class classBasicDisplay
		{
			[XmlAttribute("TextureFile", typeof(string))]
			public string TextureFile { get; set; }
			[XmlAttribute("Rotation", typeof(RotType))]
			public RotType Rotation { get; set; }
			[XmlAttribute("Scale", typeof(SclType))]
			public SclType Scale { get; set; }
			[XmlAttribute("AltVariables", typeof(classAltVariables))]
			public classAltVariables AltVariables { get; set; }
		}

		public class classCodeHandler
		{
			[XmlAttribute("CodeFile", typeof(string))]
			public string CodeFile { get; set; }
			[XmlAttribute("NamespaceClass", typeof(string))]
			public string NamepaceClass { get; set; }
		}

		public class ObjectTemplate
		{
			[XmlAttribute("ItemName", typeof(string))]
			public string ItemName { get; set; }
			[XmlAttribute("ItemIcon", typeof(string))]
			public string ItemIcon { get; set; }
			[XmlAttribute("CommonName", typeof(string))]
			public string CommonName { get; set; }
			[XmlAttribute("BasicDisplay", typeof(classBasicDisplay))]
			public classBasicDisplay BasicDisplay { get; set; }
			[XmlAttribute("CodeHandler", typeof(classCodeHandler))]
			public classCodeHandler CodeHandler { get; set; }
		}
	}
	#endregion
}
