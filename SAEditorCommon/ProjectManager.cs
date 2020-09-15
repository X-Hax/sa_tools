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
	public class ProjectSettings
	{
		public string SADXPCPath { get; set; }
		public string SA2PCPath { get; set; }
		public string projectPath { get; set; }

		private static string GetSettingsPath()
		{
			return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Settings.ini");
		}

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

		public static ProjectSettings Load()
		{
			return Load(GetSettingsPath());
		}

		public void Save()
		{
			IniSerializer.Serialize(this, GetSettingsPath());
		}
	}

	public struct SplitData
	{
		// both of these are relative to the
		public string dataFile;
		public string iniFile;
	}

	public struct SplitMDLData
	{
		public bool isBigEndian;
		public string dataFile;
		public string[] animationFiles;
	}

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
