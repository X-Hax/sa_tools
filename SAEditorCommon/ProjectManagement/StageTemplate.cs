using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using SA_Tools;

namespace SAEditorCommon.StageManagement
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

	[XmlRoot(Namespace = "http://www.sonicretro.org")]
	public class StageTemplate
	{
		[XmlAttribute("StageInfo", typeof(StgInfo))]
		public StgInfo StageInfo { get; set; }
		[XmlAttribute("ActInfo", typeof(ActInfo))]
		public List<ActInfo> ActInfos { get; set; }
	}

	[XmlRoot(Namespace = "http://www.sonicretro.org")]
	public class CharacterTemplate
	{
		[XmlAttribute("CharData", typeof(CharInfo))]
		public List<CharInfo> CharData { get; set; }
	}
}
