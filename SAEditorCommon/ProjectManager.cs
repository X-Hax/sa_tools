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
		[XmlElement("SplitEntry", typeof(SplitEntry))]
		public List<SplitEntry> SplitEntries { get; set; }
		[XmlElement("SplitEntryMDL", typeof(SplitEntryMDL))]
		public List<SplitEntryMDL> SplitMDLEntries { get; set; }
	}

	public class SplitEntry
	{
		[XmlAttribute("SourceFile")]
		public string SourceFile { get; set; }
		[XmlAttribute("IniFile")]
		public string IniFile { get; set; }
		[XmlAttribute("CommonName")]
		public string CommonName { get; set; }
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

	public class BuildSplits
	{
		public static List<SplitEntry> sadxpc_split = new List<SplitEntry>
		{
			//Stages
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG01", CommonName="Emerald Coast Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG02", CommonName="Windy Valley Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG03", CommonName="Twinkle Park Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG04", CommonName="Speed Highway Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG05", CommonName="Red Mountain Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG06", CommonName="Sky Deck Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG07", CommonName="Lost World Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG08", CommonName="Ice Cap Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG09", CommonName="Casinopolis Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG10", CommonName="Final Egg Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG12", CommonName="Hot Shelter Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "ADV00", CommonName="Station Square EXE Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "ADV01", CommonName="Egg Carrier EXE Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "ADV02", CommonName="Mystic Ruins EXE Data" },

			////Bosses
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS0", CommonName="Boss Chaos 0 EXE Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS2", CommonName="Boss Chaos 2 Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS4", CommonName="Boss Chaos 4 Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS6", CommonName="Boss Chaos 6 Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS7", CommonName="Boss Perfect Chaos Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_EGM1", CommonName="Boss Egg Hornet Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_EGM2", CommonName="Boss Egg Walker Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_EGM3", CommonName="Boss Egg Viper Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_ROBO", CommonName="Boss Zero Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_E101", CommonName="Boss E-101 Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_E101R", CommonName="Boss E-101R Data" },

			////Minigames
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG00", CommonName="Hedgehog Hammer Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "SANDBOARD", CommonName="Sandboarding Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "MINICART", CommonName="Twinkle Circuit Data" },

			////Chao
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL_GARDEN00", CommonName="Station Square Chao Garden Models & Objects" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL_GARDEN01", CommonName="Egg Carrier Chao Garden Models & Objects" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL-GARDEN02", CommonName="Mystic Ruins Chao Garden Models & Objects" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL_RACE", CommonName="Chao Race Models & Objects" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL_MAIN", CommonName="Chao Data" },

			////Common
			//new SplitData() { dataFile="sonic.exe", iniFile = "Characters", CommonName="Character EXE Data" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "Objects", CommonName="Common Objects & Enemies" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "Events", CommonName="Event Objects & Animations" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "Misc", CommonName="All other EXE Data" }
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "sonic", CommonName = "Misc Data" },

			//DLL Entries
			new SplitEntry() { SourceFile="system/CHRMODELS_orig.dll", IniFile = "chrmodels", CommonName="Character Models & Animations"},
			new SplitEntry() { SourceFile="system/ADV00MODELS.DLL", IniFile = "adv00models", CommonName="Station Square DLL" },
			new SplitEntry() { SourceFile="system/ADV01CMODELS.DLL", IniFile = "adv01cmodels", CommonName="Egg Carrier External DLL" },
			new SplitEntry() { SourceFile="system/ADV01MODELS.DLL", IniFile = "adv01models", CommonName="Egg Carrier Interior DLL" },
			new SplitEntry() { SourceFile="system/ADV02MODELS.DLL", IniFile = "adv02models", CommonName="Mystic Ruins DLL" },
			new SplitEntry() { SourceFile="system/ADV03MODELS.DLL", IniFile = "adv03models", CommonName="The Past DLL" },
			new SplitEntry() { SourceFile="system/BOSSCHAOS0MODELS.DLL", IniFile = "bosschaos0models", CommonName="Chaos 0 Boss DLL" },
			new SplitEntry() { SourceFile="system/CHAOSTGGARDEN02MR_DAYTIME.DLL", IniFile = "chaostggarden02mr_daytime", CommonName="Mystic Ruins Chao Garden (Daytime) DLL" },
			new SplitEntry() { SourceFile="system/CHAOSTGGARDEN02MR_EVENING.DLL", IniFile = "chaostggarden02mr_evening", CommonName="Mystic Ruins Chao Garden (Evening) DLL" },
			new SplitEntry() { SourceFile="system/CHAOSTGGARDEN02MR_NIGHT.DLL", IniFile = "chaostggarden02mr_night", CommonName="Mystic Ruins Chao Garden (Nighttime) DLL" }
		};

		public static List<SplitEntry> sa2pc_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile = "sonic2app.exe", IniFile = "splitsonic2app", CommonName="EXE Data" },
			new SplitEntry() { SourceFile = "resource/gd_PC/DLL/Win32/Data_DLL_orig.dll", IniFile = "data_dll", CommonName="DLL Data"}
		};
		public static List<SplitEntryMDL> sa2pc_mdlsplit = new List<SplitEntryMDL>
		{
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\amymdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"amymtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\bknuckmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"knuckmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\brougemdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"rougemtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\chaos0mdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"chaos0mtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\cwalkmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"cwalkmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\dwalkmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"dwalkmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\eggmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"eggmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\ewalk1mdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"ewalkmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\ewalk2mdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"ewalkmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\ewalkmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"ewalkmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\knuckmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"knuckmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\metalsonicmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"metalsonicmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\milesmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"milesmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\rougemdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"rougemtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\shadow1mdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"teriosmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\sonic1mdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"sonicmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\sonicmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"sonicmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\sshadowmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"sshadowmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\ssonicmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"ssonicmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\teriosmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"teriosmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\ticalmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"ticalmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\twalk1mdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"twalkmtn.prs"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = true,
				ModelFile = "resource\\gd_PC\\twalkmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"twalkmtn.prs"
				}
			}
};
	}

	public class NonBuildSplits
	{
		public static List<SplitEntry> sa1_autodemo_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile="1ST_READ.BIN", IniFile = "1ST_READ", CommonName="Characters, Objects, Enemies, etc" },
			new SplitEntry() { SourceFile="S_MOT.PRS", IniFile = "S_MOT", CommonName="Extra Sonic Animations" },

			new SplitEntry() { SourceFile="STG00.PRS", IniFile = "STG00", CommonName="Test Levels"},
			new SplitEntry() { SourceFile="STG01.PRS", IniFile = "STG01", CommonName="Emerald Coast" },
			new SplitEntry() { SourceFile="STG02.PRS", IniFile = "STG02", CommonName="Windy Valley" },
			new SplitEntry() { SourceFile="STG03.PRS", IniFile = "STG03", CommonName="Twinkle Park" },
			new SplitEntry() { SourceFile="STG04.PRS", IniFile = "STG04", CommonName="Speed Highway" },
			new SplitEntry() { SourceFile="STG05.PRS", IniFile = "STG05", CommonName="Red Mountain" },
			new SplitEntry() { SourceFile="STG06.PRS", IniFile = "STG06", CommonName="Sky Deck" },
			new SplitEntry() { SourceFile="STG07.PRS", IniFile = "STG07", CommonName="Lost World" },
			new SplitEntry() { SourceFile="STG08.PRS", IniFile = "STG08", CommonName="Ice Cap" },
			new SplitEntry() { SourceFile="STG09.PRS", IniFile = "STG09", CommonName="Casinopolis" },
			new SplitEntry() { SourceFile="STG10.PRS", IniFile = "STG10", CommonName="Final Egg" },

			new SplitEntry() { SourceFile="ADV00.PRS", IniFile = "ADV00", CommonName="Station Square" },
			new SplitEntry() { SourceFile="ADV0100.PRS", IniFile = "ADV0100", CommonName="Egg Carrier (Exteior)" },
			new SplitEntry() { SourceFile="ADV0130.PRS", IniFile = "ADV0130", CommonName="Egg Carrier (Interior)" },
			new SplitEntry() { SourceFile="ADV02.PRS", IniFile = "ADV02", CommonName="Mystic Ruins" },

			new SplitEntry() { SourceFile="AL_GARDEN00.PRS", IniFile = "AL_GARDEN00", CommonName="Station Square Chao Garden" },
			new SplitEntry() { SourceFile="AL_GARDEN01.PRS", IniFile = "AL_GARDEN01", CommonName="Egg Carrier Chao Garden" },
			new SplitEntry() { SourceFile="AL_GARDEN02.PRS", IniFile = "AL_GARDEN02", CommonName="Mystic Ruins Chao Garden" },
			new SplitEntry() { SourceFile="AL_RACE.PRS", IniFile = "AL_RACE", CommonName="Chaop Race" },

			new SplitEntry() { SourceFile="B_CHAOS0.PRS", IniFile = "B_CHAOS0", CommonName="Chaos 0" },
			new SplitEntry() { SourceFile="B_CHAOS2.PRS", IniFile = "B_CHAOS2", CommonName="Chaos 2" },
			new SplitEntry() { SourceFile="B_CHAOS4.PRS", IniFile = "B_CHAOS4", CommonName="Chaos 4" },
			new SplitEntry() { SourceFile="B_CHAOS7.PRS", IniFile = "B_CHAOS7", CommonName="Perfect Chaos" },
			new SplitEntry() { SourceFile="B_E101_R.PRS", IniFile = "B_E101_R", CommonName="E-101 MkII" },
			new SplitEntry() { SourceFile="B_EGM1.PRS", IniFile = "B_EGM1", CommonName="Egg Hornet" },
			new SplitEntry() { SourceFile="B_EGM3.PRS", IniFile = "B_EGM3", CommonName="Egg Viper" },
			new SplitEntry() { SourceFile="B_ROBO.PRS", IniFile = "B_ROBO", CommonName="Zero" },

			new SplitEntry() { SourceFile="LAND1800.BIN", IniFile = "LAND1800", CommonName="Egg Carrier" },

			new SplitEntry() { SourceFile="MINICART.PRS", IniFile = "MINICART", CommonName="Twinkle Circuit" },
			new SplitEntry() { SourceFile="SBOARD.PRS", IniFile = "SBOARD", CommonName="Sandhill" }
			
		};

		public static List<SplitEntry> sa1_final_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile="1ST_READ.BIN", IniFile = "1ST_READ", CommonName="Characters, Objects, Enemies, etc" },
			new SplitEntry() { SourceFile="SA1\\S_MOT.PRS", IniFile = "S_MOT", CommonName="Extra Sonic Animations" },
			new SplitEntry() { SourceFile="SA1\\SB_MOT.PRS", IniFile = "SB_MOT", CommonName="Sonic Snowboard Animations" },
			new SplitEntry() { SourceFile="SA1\\M_MOT.PRS", IniFile = "M_MOT", CommonName="Extra Tails Animations" },
			new SplitEntry() { SourceFile="SA1\\K_MOT.PRS", IniFile = "K_MOT", CommonName="Extra Knuckles Animations" },
			new SplitEntry() { SourceFile="SA1\\A_MOT.PRS", IniFile = "A_MOT", CommonName="Extra Amy Animations" },
			new SplitEntry() { SourceFile="SA1\\B_MOT.PRS", IniFile = "B_MOT", CommonName="Extra Big Animations" },
			new SplitEntry() { SourceFile="SA1\\E_MOT.PRS", IniFile = "E_MOT", CommonName="Extra Gamma Animations" },

			new SplitEntry() { SourceFile="SA1\\STG01.PRS", IniFile = "STG01", CommonName="Emerald Coast" },
			new SplitEntry() { SourceFile="SA1\\STG02.PRS", IniFile = "STG02", CommonName="Windy Valley" },
			new SplitEntry() { SourceFile="SA1\\STG03.PRS", IniFile = "STG03", CommonName="Twinkle Park" },
			new SplitEntry() { SourceFile="SA1\\STG04.PRS", IniFile = "STG04", CommonName="Speed Highway" },
			new SplitEntry() { SourceFile="SA1\\STG05.PRS", IniFile = "STG05", CommonName="Red Mountain" },
			new SplitEntry() { SourceFile="SA1\\STG06.PRS", IniFile = "STG06", CommonName="Sky Deck" },
			new SplitEntry() { SourceFile="SA1\\STG07.PRS", IniFile = "STG07", CommonName="Lost World" },
			new SplitEntry() { SourceFile="SA1\\STG08.PRS", IniFile = "STG08", CommonName="Ice Cap" },
			new SplitEntry() { SourceFile="SA1\\STG09.PRS", IniFile = "STG09", CommonName="Casinopolis" },
			new SplitEntry() { SourceFile="SA1\\STG10.PRS", IniFile = "STG10", CommonName="Final Egg" },
			new SplitEntry() { SourceFile="SA1\\STG12.PRS", IniFile = "STG12", CommonName="Hot Shelter" },

			new SplitEntry() { SourceFile="SA1\\ADV00.PRS", IniFile = "ADV00", CommonName="Station Square" },
			new SplitEntry() { SourceFile="SA1\\ADV0100.PRS", IniFile = "ADV0100", CommonName="Egg Carrier (Exteior)" },
			new SplitEntry() { SourceFile="SA1\\ADV0130.PRS", IniFile = "ADV0130", CommonName="Egg Carrier (Interior)" },
			new SplitEntry() { SourceFile="SA1\\ADV02.PRS", IniFile = "ADV02", CommonName="Mystic Ruins" },
			new SplitEntry() { SourceFile="SA1\\ADV03.PRS", IniFile = "ADV03", CommonName="The Past" },

			new SplitEntry() { SourceFile="SA1\\AL_GARDEN00.PRS", IniFile = "AL_GARDEN00", CommonName="Station Square Chao Garden" },
			new SplitEntry() { SourceFile="SA1\\AL_GARDEN01.PRS", IniFile = "AL_GARDEN01", CommonName="Egg Carrier Chao Garden" },
			new SplitEntry() { SourceFile="SA1\\AL_GARDEN02.PRS", IniFile = "AL_GARDEN02", CommonName="Mystic Ruins Chao Garden" },
			new SplitEntry() { SourceFile="SA1\\AL_RACE.PRS", IniFile = "AL_RACE", CommonName="Chaop Race" },

			new SplitEntry() { SourceFile="SA1\\B_CHAOS0.PRS", IniFile = "B_CHAOS0", CommonName="Chaos 0" },
			new SplitEntry() { SourceFile="SA1\\B_CHAOS2.PRS", IniFile = "B_CHAOS2", CommonName="Chaos 2" },
			new SplitEntry() { SourceFile="SA1\\B_CHAOS4.PRS", IniFile = "B_CHAOS4", CommonName="Chaos 4" },
			new SplitEntry() { SourceFile="SA1\\B_CHAOS4.PRS", IniFile = "B_CHAOS6", CommonName="Chaos 6" },
			new SplitEntry() { SourceFile="SA1\\B_CHAOS7.PRS", IniFile = "B_CHAOS7", CommonName="Perfect Chaos" },
			new SplitEntry() { SourceFile="SA1\\B_E101.PRS", IniFile = "B_E101", CommonName="E-101" },
			new SplitEntry() { SourceFile="SA1\\B_E101_R.PRS", IniFile = "B_E101_R", CommonName="E-101 MkII" },
			new SplitEntry() { SourceFile="SA1\\B_EGM1.PRS", IniFile = "B_EGM1", CommonName="Egg Hornet" },
			new SplitEntry() { SourceFile="SA1\\B_EGM2.PRS", IniFile = "B_EGM2", CommonName="Egg Walker" },
			new SplitEntry() { SourceFile="SA1\\B_EGM3.PRS", IniFile = "B_EGM3", CommonName="Egg Viper" },
			new SplitEntry() { SourceFile="SA1\\B_ROBO.PRS", IniFile = "B_ROBO", CommonName="Zero" },

			new SplitEntry() { SourceFile="SA1\\MINICART.PRS", IniFile = "MINICART", CommonName="Twinkle Circuit" },
			new SplitEntry() { SourceFile="SA1\\SBOARD.PRS", IniFile = "SBOARD", CommonName="Sandhill" },
			new SplitEntry() { SourceFile="SA1\\SHOOTING.PRS", IniFile = "SHOOTING", CommonName="Sky Chase" },

			new SplitEntry() { SourceFile="SA1\\ADVERTISE.PRS", IniFile = "ADVERTISE", CommonName="Menu Models" },
			new SplitEntry() { SourceFile="SA1\\SUMMARY.PRS", IniFile = "SUMMARY", CommonName="Title Screen Model" }
		};

		public static List<SplitEntry> sadxgc_split = new List<SplitEntry>
		{

		};

		public static List<SplitEntry> sadxgc_preview_split = new List<SplitEntry>
		{

		};

		public static List<SplitEntry> sadxgc_review_split = new List<SplitEntry>
		{

		};

		public static List<SplitEntry> sadx360_proto_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile="SonicApp.exe", IniFile = "SonicApp", CommonName="All main executable" },
		};

		public static List<SplitEntry> sa2_trial_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile="1ST_READ.bin", IniFile = "1ST_READ", CommonName="Miscellanous Assets" },
			new SplitEntry() { SourceFile="STG13.PRS", IniFile = "STG13", CommonName="City Escape" }
		};
		public static List<SplitEntryMDL> sa2_trial_mdlsplit = new List<SplitEntryMDL>
		{
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "sonicmdl.prs",
				MotionFiles = new List<string>
				{
					"plcommtn.prs",
					"sonicmtn.prs"
				}
			}
		};

		public static List<SplitEntry> sa2_preview_split = new List<SplitEntry>
		{

		};
		public static List<SplitEntryMDL> sa2_preview_mdlsplit = new List<SplitEntryMDL>
		{

		};

		public static List<SplitEntry> sa2_final_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile="1ST_READ.BIN", IniFile = "1ST_READ", CommonName="Misc. Assets" },
			new SplitEntry() { SourceFile="SONIC2\\ADVERTISE.PRS", IniFile = "ADVERTISE", CommonName="Misc. Menu Assets" },
			new SplitEntry() { SourceFile="SONIC2\\CART.PRS", IniFile = "CART", CommonName="Cart Racing Assets" },
			new SplitEntry() { SourceFile="SONIC2\\EMBLEMGET.PRS", IniFile = "EMBLEMGET", CommonName="Emblem Model" },

			new SplitEntry() { SourceFile="SONIC2\\STG03.PRS", IniFile = "STG03", CommonName="Green Forest Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG04.PRS", IniFile = "STG04", CommonName="White Jungle Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG05.PRS", IniFile = "STG05", CommonName="Pumpkin Hill Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG06.PRS", IniFile = "STG06", CommonName="Sky Rail Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG07.PRS", IniFile = "STG07", CommonName="Aquatic Mine Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG08.PRS", IniFile = "STG08", CommonName="Security Hall Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG09.PRS", IniFile = "STG09", CommonName="Prison Lane Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG10.PRS", IniFile = "STG10", CommonName="Metal Harbor Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG11.PRS", IniFile = "STG11", CommonName="Iron Gate Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG12.PRS", IniFile = "STG12", CommonName="Weapons Bed Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG13.PRS", IniFile = "STG13", CommonName="City Escape Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG14.PRS", IniFile = "STG14", CommonName="Radical Highway Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG15.PRS", IniFile = "STG15", CommonName="Weapons Bed (2P) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG16.PRS", IniFile = "STG16", CommonName="Wild Canyon Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG17.PRS", IniFile = "STG17", CommonName="Mission Street Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG18.PRS", IniFile = "STG18", CommonName="Dry Lagoon Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG19.PRS", IniFile = "STG19", CommonName="Sonic v. Shadow 1 Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG20.PRS", IniFile = "STG20", CommonName="Tails v. Eggman 1 Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG21.PRS", IniFile = "STG21", CommonName="Sand Ocean Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG22.PRS", IniFile = "STG22", CommonName="Crazy Gadget Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG23.PRS", IniFile = "STG23", CommonName="Hidden Base Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG24.PRS", IniFile = "STG24", CommonName="Eternal Engine Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG25.PRS", IniFile = "STG25", CommonName="Death Chamber Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG26.PRS", IniFile = "STG26", CommonName="Egg Quarters Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG27.PRS", IniFile = "STG27", CommonName="Lost Colony Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG28.PRS", IniFile = "STG28", CommonName="Pyramid Case Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG29.PRS", IniFile = "STG29", CommonName="Tails v. Eggman 2 Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG30.PRS", IniFile = "STG30", CommonName="Final Rush Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG31.PRS", IniFile = "STG31", CommonName="Green Hill Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG32.PRS", IniFile = "STG32", CommonName="Meteor Herd Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG33.PRS", IniFile = "STG33", CommonName="Knuckles v. Rouge Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG34.PRS", IniFile = "STG34", CommonName="Cannon's Core (Sonic) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG35.PRS", IniFile = "STG35", CommonName="Cannon's Core (Eggman) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG36.PRS", IniFile = "STG36", CommonName="Cannon's Core (Tails) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG37.PRS", IniFile = "STG37", CommonName="Cannon's Core (Rouge) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG38.PRS", IniFile = "STG38", CommonName="Cannon's Core (Knuckles) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG39.PRS", IniFile = "STG39", CommonName="Mission Street (2P) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG40.PRS", IniFile = "STG40", CommonName="Final Chase Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG41.PRS", IniFile = "STG41", CommonName="Wild Canyon (2P) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG42.PRS", IniFile = "STG42", CommonName="Sonic v. Shadow 2 Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG43.PRS", IniFile = "STG43", CommonName="Cosmic Wall Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG44.PRS", IniFile = "STG44", CommonName="Mad Space Assets" },
			new SplitEntry() { SourceFile="SONIC2\\STG45.PRS", IniFile = "STG45", CommonName="Sand Ocean (2P) Assets" },

			new SplitEntry() { SourceFile="SONIC2\\BOSS_BIGBOGY.PRS", IniFile = "BOSS_BIGBOGY", CommonName="King Boom Boo Assets" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_BIGFOOT.PRS", IniFile = "BOSS_BIGFOOT", CommonName="Bigfoot Assets" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_FDOG.PRS", IniFile = "BOSS_FDOG", CommonName="Flying Dog Assets" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_GOLEM.PRS", IniFile = "BOSS_GOLEM", CommonName="Egg Golem (Sonic) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_GOLEM_E.PRS", IniFile = "BOSS_GOLEM_E", CommonName="Egg Golem (Eggman) Assets" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_HOTSHOT.PRS", IniFile = "BOSS_HOTSHOT", CommonName="Hotshot Assets" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_LAST1.PRS", IniFile = "BOSS_LAST1", CommonName="Biolizard Assets" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_LAST2.PRS", IniFile = "BOSS_LAST2", CommonName="Final Hazard Assets" },

			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGDARK.PRS", IniFile = "CHAOSTGDARK", CommonName="Dark Chao Garden Assets" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGENTRANCE.PRS", IniFile = "CHAOSTGENTRANCE", CommonName="Chao Lobby Assets" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGHERO.PRS", IniFile = "CHAOSTGHERO", CommonName="Hero Chao Garden Assets" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGKINDER.PRS", IniFile = "CHAOSTGKINDER", CommonName="Chao Kindergarten Assets" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLAND000.PRS", IniFile = "CHAOSTGLOBBYLAND000", CommonName="Chao Lobby Stage (Basic)" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLAND00K.PRS", IniFile = "CHAOSTGLOBBYLAND00K", CommonName="Chao Lobby Stage (w/ Kindergarten)" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLAND0DK.PRS", IniFile = "CHAOSTGLOBBYLAND0DK", CommonName="Chao Lobby Stage (w/ Dark + Kindergarten" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLANDH0K.PRS", IniFile = "CHAOSTGLOBBYLANDH0K", CommonName="Chao Lobby Stage (w/ Hero + Kindergarten" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLANDHDK.PRS", IniFile = "CHAOSTGLOBBYLANDHDK", CommonName="Chao Lobby Stage (Complete)" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGNEUT.PRS", IniFile = "CHAOSTGNEUT", CommonName="Neutral Chao Garden" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGRACE.PRS", IniFile = "CHAOSTGRACE", CommonName="Chao Race Lobby Assets" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGRACELANDD.PRS", IniFile = "CHAOSTGRACELANDD", CommonName="Dark Chao Race Assets" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGRACELANDH.PRS", IniFile = "CHAOSTGRACELANDH", CommonName="Hero Chao Race Assets" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGRACELANDN.PRS", IniFile = "CHAOSTGRACELANDN", CommonName="Neutral Chao Race Assets" }
		};
		public static List<SplitEntryMDL> sa2_final_mdlsplit = new List<SplitEntryMDL>
		{
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\AMYMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"AMYMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\BKNUCKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"KNUCKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\BROUGEMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"ROUGEMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\BWALKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"EWALKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\CHAOS0MDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"CHAOS0MTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\CWALKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"CWALKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\EGGMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"EGGMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\EWALK1MDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"EWALKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\EWALKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"EWALKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\HEWALKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"EWALKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\HKNUCKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"KNUCKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\HROUGEMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"ROUGEMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\HSHADOWMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"TERIOSMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\HSONICMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"SONICMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\HTWALKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"TWALKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\KNUCKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"KNUCKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\METALSONICMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"METALSONICMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\MILESMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"MILESMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\PSOSHADOWMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"TERIOSMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\PSOSONICMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"SONICMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\ROUGEMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"ROUGEMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\SONICMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"SONICMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\SSHADOWMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\SSONICMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\TERIOSMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"TERIOSMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\TICALMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"TICALMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\TWALK1MDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"TWALKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\TWALKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"TWALKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\XEWALKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"EWALKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\XKNUCKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"KNUCKMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\XROUGEMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"ROUGEMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\XSHADOWMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"TERIOSMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\XSONICMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"SONICMTN.PRS"
				}
			},
			new SplitEntryMDL()
			{
				BigEndian = false,
				ModelFile = "SONIC2\\XTWALKMDL.PRS",
				MotionFiles = new List<string>
				{
					"PLCOMMTN.PRS",
					"TWALKMTN.PRS"
				}
			}
		};
	}
}
