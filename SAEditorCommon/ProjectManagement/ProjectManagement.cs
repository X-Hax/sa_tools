using System.Collections.Generic;
using System.Xml.Serialization;

namespace SAEditorCommon.ProjectManagement
{
	public class SplitTemplate
	{
		[XmlElement("GameInfo", typeof(SplitInfo))]
		public SplitInfo GameInfo { get; set; }
		[XmlElement("SplitEntry", typeof(SplitEntry))]
		public List<SplitEntry> SplitEntries { get; set; }
		[XmlElement("SplitEntryMDL", typeof(SplitEntryMDL))]
		public List<SplitEntryMDL> SplitMDLEntries { get; set; }
	}

	public class ProjectTemplate
	{
		[XmlElement("GameInfo", typeof(ProjectInfo))]
		public ProjectInfo GameInfo { get; set; }
		[XmlElement("SplitEntry", typeof(SplitEntry))]
		public List<SplitEntry> SplitEntries { get; set; }
		[XmlElement("SplitEntryMDL", typeof(SplitEntryMDL))]
		public List<SplitEntryMDL> SplitMDLEntries { get; set; }
	}

	public class SplitInfo
	{
		[XmlAttribute("gameName")]
		public string GameName { get; set; }
		[XmlAttribute("gameSystemFolder")]
		public string GameSystemFolder { get; set; }
		[XmlAttribute("dataFolder")]
		public string DataFolder { get; set; }
	}

	public class ProjectInfo
	{
		[XmlAttribute("gameName")]
		public string GameName { get; set; }
		[XmlAttribute("gameSystemFolder")]
		public string GameSystemFolder { get; set; }
		[XmlAttribute("modSystemFolder")]
		public string ModSystemFolder { get; set; }
		[XmlAttribute("canBuild")]
		public bool CanBuild { get; set; }
	}

	public class SplitEntry
	{
		[XmlAttribute("SourceFile")]
		public string SourceFile { get; set; }
		[XmlAttribute("IniFile")]
		public string IniFile { get; set; }
		[XmlAttribute("CmnName")]
		public string CmnName { get; set; }
	}

	public class SplitEntryMDL
	{
		[XmlAttribute("BigEndian")]
		public bool BigEndian { get; set; }
		[XmlAttribute("ModelFile")]
		public string ModelFile { get; set; }
		[XmlElement("MotionFile")]
		public List<string> MotionFiles { get; set; }
	}
}
