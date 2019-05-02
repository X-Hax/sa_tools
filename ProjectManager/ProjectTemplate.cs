using System.Collections.Generic;
using System.Xml.Serialization;

namespace ProjectManager
{
	[XmlRoot(Namespace = "http://www.sonicretro.org")]
	public class ProjectTemplate
	{
		[XmlAttribute("gameName")]
		public string GameName { get; set; }
		[XmlAttribute("checkFile")]
		public string CheckFile { get; set; }
		[XmlAttribute("gameSystemFolder")]
		public string GameSystemFolder { get; set; }
		[XmlAttribute("modSystemFolder")]
		public string ModSystemFolder { get; set; }
		[XmlAttribute("canBuild")]
		public bool CanBuild { get; set; }
		[XmlAttribute("canUseSADXLVL2")]
		public bool CanUseSADXLVL2 { get; set; }
		[XmlAttribute("canUseSADXTweaker2")]
		public bool CanUseSADXTweaker2 { get; set; }
		[XmlElement("SplitEntryGeneral", typeof(SplitEntryGeneral))]
		[XmlElement("SplitEntryMDL", typeof(SplitEntryMDL))]
		public List<SplitEntry> SplitEntries { get; set; }
	}

	public abstract class SplitEntry
	{
		public string SourceFile { get; set; }
	}

	public class SplitEntryGeneral : SplitEntry
	{
		public string AltSourceFile { get; set; }
		public string IniFile { get; set; }
	}

	public class SplitEntryMDL : SplitEntry
	{
		[XmlAttribute("bigEndian")]
		public bool BigEndian { get; set; }
		[XmlElement("MotionFile")]
		public List<string> MotionFiles { get; set; }
	}
}
