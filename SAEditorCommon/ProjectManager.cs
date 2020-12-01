using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using SA_Tools;

namespace ProjectManagement
{
	/// <summary>
	/// Holds settings necessary for the project manager.
	/// </summary>
	/// 
	public class ProjectSettings
	{
		public string SADXPCPath { get; set; }
		public string SA2PCPath { get; set; }
		public string SA1Path { get; set; }
		public string SA1ADPath { get; set; }
		public string SADXGCPath { get; set; }
		public string SADXGCPPath { get; set; }
		public string SADXGCRPath { get; set; }
		public string SADX360Path { get; set; }
		public string SA2Path { get; set; }
		public string SA2TTPath { get; set; }
		public string SA2PPath { get; set; }

		public static ProjectSettings Load(string iniPath)
		{
			if (File.Exists(iniPath))
			{
				return (ProjectSettings)IniSerializer.Deserialize(typeof(ProjectSettings), iniPath);
			}
			else
			{
				ProjectSettings result = new ProjectSettings()
				{
					SA2PCPath = "",
					SADXPCPath = ""
				};

				return result;
			}
		}
		public static ProjectSettings Load()
		{
			return Load(GetSettingsPath());
		}

		private static string GetSettingsPath()
		{
			return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Settings.ini");
		}

		public string GetModPathForGame(SA_Tools.Game game)
		{
			switch (game)
			{
				case Game.SA1:
				case Game.SA2:
					throw new System.NotSupportedException();

				case Game.SADX:
					return  Path.Combine(SADXPCPath, "mods");
				
				case Game.SA2B:
					return Path.Combine(SA2PCPath, "mods");

				default:
					throw new System.NotSupportedException();
			}
		}

		public string GetExecutableForGame(SA_Tools.Game game)
		{
			switch (game)
			{
				case Game.SA1:
				case Game.SA2:
					throw new System.NotSupportedException();

				case Game.SADX:
					return "sonic.exe";

				case Game.SA2B:
					return "sonic2App.exe";

				default:
					throw new System.NotSupportedException();
			}
		}

		public string GetGamePath(SA_Tools.Game game)
		{
			switch (game)
			{
				case Game.SA1:
				case Game.SA2:
					throw new System.NotSupportedException();

				case Game.SADX:
					return SADXPCPath;

				case Game.SA2B:
					return SA2PCPath;

				default:
					throw new System.NotSupportedException();
			}
		}

		public void Save()
		{
			IniSerializer.Serialize(this, GetSettingsPath());
		}
	}

	[XmlRoot(Namespace = "http://www.sonicretro.org")]
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
