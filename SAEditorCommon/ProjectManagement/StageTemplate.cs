using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using SA_Tools;

namespace SAEditorCommon.StageManagement
{
	public class StgInfo
	{
		[XmlElement("StgID", typeof(string))]
		public string StgID { get; set; }
		[XmlElement("Name", typeof(string))]
		public string Name { get; set; }
		[XmlElement("ActTotal", typeof(int))]
		public int ActTotal { get; set; }
		[XmlElement("StgRoot", typeof(string))]
		public string StgRoot { get; set; }
		[XmlElement("ObjTexLists", typeof(string))]
		public string ObjTexList { get; set; }
	}

	public class ActInfo
	{
		[XmlElement("ActID", typeof(string))]
		public string ActID { get; set; }
		[XmlElement("Landtable", typeof(string))]
		public string Landtable { get; set; }
		[XmlElement("ActTexList", typeof(string))]
		public string ActTexList { get; set; }
		[XmlElement("ObjList", typeof(string))]
		public string ObjList { get; set; }
		[XmlElement("KillColli", typeof(string))]
		public string KillColli { get; set; }
		[XmlElement("UseCodeItem", typeof(bool))]
		public bool UseCodeItem { get; set; }
		[XmlElement("CodeItems", typeof(string), IsNullable = true)]
		public List<string> CodeItems { get; set; }
	}

	public class CharInfo
	{
		[XmlElement("Name", typeof(string))]
		public string Name { get; set; }
		[XmlElement("CharID", typeof(int))]
		public int CharID { get; set; }
		[XmlElement("CharTexList", typeof(string))]
		public string CharTexList { get; set; }
		[XmlElement("Height", typeof(float))]
		public float Height { get; set; }
		[XmlElement("StartPosList", typeof(string))]
		public string StartPosList { get; set; }
	}

	[XmlRoot(Namespace = "http://www.sonicretro.org")]
	public class StageTemplate
	{
		[XmlElement("StageInfo", typeof(StgInfo))]
		public StgInfo StageInfo { get; set; }
		[XmlElement("ActInfo", typeof(ActInfo))]
		public List<ActInfo> ActInfos { get; set; }
	}

	[XmlRoot(Namespace = "http://www.sonicretro.org")]
	public class CharacterTemplate
	{
		[XmlElement("CharData", typeof(CharInfo))]
		public List<CharInfo> CharData { get; set; }
	}
}
