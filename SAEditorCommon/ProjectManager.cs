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
		[XmlElement("projectInfo")]
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

	public class BuildSplits
	{
		public static List<SplitEntry> sadxpc_split = new List<SplitEntry>
		{
			//Stages
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG01" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG02" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG03" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG04" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG05" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG06" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG07" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG08" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG09" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG10" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG12" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "ADV00" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "ADV01" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "ADV02" },

			////Bosses
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_CHAOS0" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_CHAOS2" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_CHAOS4" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_CHAOS6" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_EGM1" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_EGM2" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_EGM3" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_ROBO" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_E101" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "B_E101R" },

			////Minigames
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "STG00" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "SBOARD" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "MINICART" },

			////Chao
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "AL_GARDEN00" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "AL_GARDEN01" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "AL_GARDEN02" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "AL_RACE" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "AL_MAIN" },

			////Common
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "Characters" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "Common" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "Events" },
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "sonic" },

			//DLL Entries
			new SplitEntry() { SourceFile="system/CHRMODELS_orig.dll", IniFile = "chrmodels"},
			new SplitEntry() { SourceFile="system/ADV00MODELS.DLL", IniFile = "adv00models" },
			new SplitEntry() { SourceFile="system/ADV01CMODELS.DLL", IniFile = "adv01cmodels" },
			new SplitEntry() { SourceFile="system/ADV01MODELS.DLL", IniFile = "adv01models" },
			new SplitEntry() { SourceFile="system/ADV02MODELS.DLL", IniFile = "adv02models" },
			new SplitEntry() { SourceFile="system/ADV03MODELS.DLL", IniFile = "adv03models" },
			new SplitEntry() { SourceFile="system/BOSSCHAOS0MODELS.DLL", IniFile = "bosschaos0models" },
			new SplitEntry() { SourceFile="system/CHAOSTGGARDEN02MR_DAYTIME.DLL", IniFile = "chaostggarden02mr_daytime" },
			new SplitEntry() { SourceFile="system/CHAOSTGGARDEN02MR_EVENING.DLL", IniFile = "chaostggarden02mr_evening" },
			new SplitEntry() { SourceFile="system/CHAOSTGGARDEN02MR_NIGHT.DLL", IniFile = "chaostggarden02mr_night" }
		};

		public static List<SplitEntry> sa2pc_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile = "sonic2app.exe", IniFile = "splitsonic2app" },
			new SplitEntry() { SourceFile = "resource/gd_PC/DLL/Win32/Data_DLL_orig.dll", IniFile = "data_dll"}
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
			new SplitEntry() { SourceFile="1ST_READ.BIN", IniFile = "1ST_READ" },
			new SplitEntry() { SourceFile="S_MOT.PRS", IniFile = "S_MOT" },

			new SplitEntry() { SourceFile="STG00.PRS", IniFile = "STG00"},
			new SplitEntry() { SourceFile="STG01.PRS", IniFile = "STG01" },
			new SplitEntry() { SourceFile="STG02.PRS", IniFile = "STG02" },
			new SplitEntry() { SourceFile="STG03.PRS", IniFile = "STG03" },
			new SplitEntry() { SourceFile="STG04.PRS", IniFile = "STG04" },
			new SplitEntry() { SourceFile="STG05.PRS", IniFile = "STG05" },
			new SplitEntry() { SourceFile="STG06.PRS", IniFile = "STG06" },
			new SplitEntry() { SourceFile="STG07.PRS", IniFile = "STG07" },
			new SplitEntry() { SourceFile="STG08.PRS", IniFile = "STG08" },
			new SplitEntry() { SourceFile="STG09.PRS", IniFile = "STG09" },
			new SplitEntry() { SourceFile="STG10.PRS", IniFile = "STG10" },

			new SplitEntry() { SourceFile="ADV00.PRS", IniFile = "ADV00" },
			new SplitEntry() { SourceFile="ADV0100.PRS", IniFile = "ADV0100" },
			new SplitEntry() { SourceFile="ADV0130.PRS", IniFile = "ADV0130" },
			new SplitEntry() { SourceFile="ADV02.PRS", IniFile = "ADV02" },

			new SplitEntry() { SourceFile="AL_GARDEN00.PRS", IniFile = "AL_GARDEN00" },
			new SplitEntry() { SourceFile="AL_GARDEN01.PRS", IniFile = "AL_GARDEN01" },
			new SplitEntry() { SourceFile="AL_GARDEN02.PRS", IniFile = "AL_GARDEN02" },
			new SplitEntry() { SourceFile="AL_RACE.PRS", IniFile = "AL_RACE" },

			new SplitEntry() { SourceFile="B_CHAOS0.PRS", IniFile = "B_CHAOS0" },
			new SplitEntry() { SourceFile="B_CHAOS2.PRS", IniFile = "B_CHAOS2" },
			new SplitEntry() { SourceFile="B_CHAOS4.PRS", IniFile = "B_CHAOS4" },
			new SplitEntry() { SourceFile="B_CHAOS7.PRS", IniFile = "B_CHAOS7" },
			new SplitEntry() { SourceFile="B_E101_R.PRS", IniFile = "B_E101_R" },
			new SplitEntry() { SourceFile="B_EGM1.PRS", IniFile = "B_EGM1" },
			new SplitEntry() { SourceFile="B_EGM3.PRS", IniFile = "B_EGM3" },
			new SplitEntry() { SourceFile="B_ROBO.PRS", IniFile = "B_ROBO" },

			new SplitEntry() { SourceFile="LAND1800.BIN", IniFile = "LAND1800" },

			new SplitEntry() { SourceFile="MINICART.PRS", IniFile = "MINICART" },
			new SplitEntry() { SourceFile="SBOARD.PRS", IniFile = "SBOARD" }
			
		};

		public static List<SplitEntry> sa1_final_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile="1ST_READ.BIN", IniFile = "1ST_READ" },
			new SplitEntry() { SourceFile="SA1\\S_MOT.PRS", IniFile = "S_MOT" },
			new SplitEntry() { SourceFile="SA1\\SB_MOT.PRS", IniFile = "SB_MOT" },
			new SplitEntry() { SourceFile="SA1\\M_MOT.PRS", IniFile = "M_MOT" },
			new SplitEntry() { SourceFile="SA1\\K_MOT.PRS", IniFile = "K_MOT" },
			new SplitEntry() { SourceFile="SA1\\A_MOT.PRS", IniFile = "A_MOT" },
			new SplitEntry() { SourceFile="SA1\\B_MOT.PRS", IniFile = "B_MOT" },
			new SplitEntry() { SourceFile="SA1\\E_MOT.PRS", IniFile = "E_MOT" },

			new SplitEntry() { SourceFile="SA1\\STG01.PRS", IniFile = "STG01" },
			new SplitEntry() { SourceFile="SA1\\STG02.PRS", IniFile = "STG02" },
			new SplitEntry() { SourceFile="SA1\\STG03.PRS", IniFile = "STG03" },
			new SplitEntry() { SourceFile="SA1\\STG04.PRS", IniFile = "STG04" },
			new SplitEntry() { SourceFile="SA1\\STG05.PRS", IniFile = "STG05" },
			new SplitEntry() { SourceFile="SA1\\STG06.PRS", IniFile = "STG06" },
			new SplitEntry() { SourceFile="SA1\\STG07.PRS", IniFile = "STG07" },
			new SplitEntry() { SourceFile="SA1\\STG08.PRS", IniFile = "STG08" },
			new SplitEntry() { SourceFile="SA1\\STG09.PRS", IniFile = "STG09" },
			new SplitEntry() { SourceFile="SA1\\STG10.PRS", IniFile = "STG10" },
			new SplitEntry() { SourceFile="SA1\\STG12.PRS", IniFile = "STG12" },

			new SplitEntry() { SourceFile="SA1\\ADV00.PRS", IniFile = "ADV00" },
			new SplitEntry() { SourceFile="SA1\\ADV0100.PRS", IniFile = "ADV0100" },
			new SplitEntry() { SourceFile="SA1\\ADV0130.PRS", IniFile = "ADV0130" },
			new SplitEntry() { SourceFile="SA1\\ADV02.PRS", IniFile = "ADV02" },
			new SplitEntry() { SourceFile="SA1\\ADV03.PRS", IniFile = "ADV03" },

			new SplitEntry() { SourceFile="SA1\\AL_GARDEN00.PRS", IniFile = "AL_GARDEN00" },
			new SplitEntry() { SourceFile="SA1\\AL_GARDEN01.PRS", IniFile = "AL_GARDEN01" },
			new SplitEntry() { SourceFile="SA1\\AL_GARDEN02.PRS", IniFile = "AL_GARDEN02" },
			new SplitEntry() { SourceFile="SA1\\AL_RACE.PRS", IniFile = "AL_RACE" },

			new SplitEntry() { SourceFile="SA1\\B_CHAOS0.PRS", IniFile = "B_CHAOS0" },
			new SplitEntry() { SourceFile="SA1\\B_CHAOS2.PRS", IniFile = "B_CHAOS2" },
			new SplitEntry() { SourceFile="SA1\\B_CHAOS4.PRS", IniFile = "B_CHAOS4" },
			new SplitEntry() { SourceFile="SA1\\B_CHAOS4.PRS", IniFile = "B_CHAOS6" },
			new SplitEntry() { SourceFile="SA1\\B_CHAOS7.PRS", IniFile = "B_CHAOS7" },
			new SplitEntry() { SourceFile="SA1\\B_E101.PRS", IniFile = "B_E101" },
			new SplitEntry() { SourceFile="SA1\\B_E101_R.PRS", IniFile = "B_E101_R" },
			new SplitEntry() { SourceFile="SA1\\B_EGM1.PRS", IniFile = "B_EGM1" },
			new SplitEntry() { SourceFile="SA1\\B_EGM2.PRS", IniFile = "B_EGM2" },
			new SplitEntry() { SourceFile="SA1\\B_EGM3.PRS", IniFile = "B_EGM3" },
			new SplitEntry() { SourceFile="SA1\\B_ROBO.PRS", IniFile = "B_ROBO" },

			new SplitEntry() { SourceFile="SA1\\MINICART.PRS", IniFile = "MINICART" },
			new SplitEntry() { SourceFile="SA1\\SBOARD.PRS", IniFile = "SBOARD" },
			new SplitEntry() { SourceFile="SA1\\SHOOTING.PRS", IniFile = "SHOOTING" },

			new SplitEntry() { SourceFile="SA1\\ADVERTISE.PRS", IniFile = "ADVERTISE" },
			new SplitEntry() { SourceFile="SA1\\SUMMARY.PRS", IniFile = "SUMMARY" }
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
			new SplitEntry() { SourceFile="SonicApp.exe", IniFile = "SonicApp" },
		};

		public static List<SplitEntry> sa2_trial_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile="1ST_READ.bin", IniFile = "1ST_READ" },
			new SplitEntry() { SourceFile="STG13.PRS", IniFile = "STG13" }
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
			new SplitEntry() { SourceFile="1ST_READ.BIN", IniFile = "1ST_READ" },
			new SplitEntry() { SourceFile="SONIC2\\ADVERTISE.PRS", IniFile = "ADVERTISE" },
			new SplitEntry() { SourceFile="SONIC2\\CART.PRS", IniFile = "CART" },
			new SplitEntry() { SourceFile="SONIC2\\EMBLEMGET.PRS", IniFile = "EMBLEMGET" },

			new SplitEntry() { SourceFile="SONIC2\\STG03.PRS", IniFile = "STG03" },
			new SplitEntry() { SourceFile="SONIC2\\STG04.PRS", IniFile = "STG04" },
			new SplitEntry() { SourceFile="SONIC2\\STG05.PRS", IniFile = "STG05" },
			new SplitEntry() { SourceFile="SONIC2\\STG06.PRS", IniFile = "STG06" },
			new SplitEntry() { SourceFile="SONIC2\\STG07.PRS", IniFile = "STG07" },
			new SplitEntry() { SourceFile="SONIC2\\STG08.PRS", IniFile = "STG08" },
			new SplitEntry() { SourceFile="SONIC2\\STG09.PRS", IniFile = "STG09" },
			new SplitEntry() { SourceFile="SONIC2\\STG10.PRS", IniFile = "STG10" },
			new SplitEntry() { SourceFile="SONIC2\\STG11.PRS", IniFile = "STG11" },
			new SplitEntry() { SourceFile="SONIC2\\STG12.PRS", IniFile = "STG12" },
			new SplitEntry() { SourceFile="SONIC2\\STG13.PRS", IniFile = "STG13" },
			new SplitEntry() { SourceFile="SONIC2\\STG14.PRS", IniFile = "STG14" },
			new SplitEntry() { SourceFile="SONIC2\\STG15.PRS", IniFile = "STG15" },
			new SplitEntry() { SourceFile="SONIC2\\STG16.PRS", IniFile = "STG16" },
			new SplitEntry() { SourceFile="SONIC2\\STG17.PRS", IniFile = "STG17" },
			new SplitEntry() { SourceFile="SONIC2\\STG18.PRS", IniFile = "STG18" },
			new SplitEntry() { SourceFile="SONIC2\\STG19.PRS", IniFile = "STG19" },
			new SplitEntry() { SourceFile="SONIC2\\STG20.PRS", IniFile = "STG20" },
			new SplitEntry() { SourceFile="SONIC2\\STG21.PRS", IniFile = "STG21" },
			new SplitEntry() { SourceFile="SONIC2\\STG22.PRS", IniFile = "STG22" },
			new SplitEntry() { SourceFile="SONIC2\\STG23.PRS", IniFile = "STG23" },
			new SplitEntry() { SourceFile="SONIC2\\STG24.PRS", IniFile = "STG24" },
			new SplitEntry() { SourceFile="SONIC2\\STG25.PRS", IniFile = "STG25" },
			new SplitEntry() { SourceFile="SONIC2\\STG26.PRS", IniFile = "STG26" },
			new SplitEntry() { SourceFile="SONIC2\\STG27.PRS", IniFile = "STG27" },
			new SplitEntry() { SourceFile="SONIC2\\STG28.PRS", IniFile = "STG28" },
			new SplitEntry() { SourceFile="SONIC2\\STG29.PRS", IniFile = "STG29" },
			new SplitEntry() { SourceFile="SONIC2\\STG30.PRS", IniFile = "STG30" },
			new SplitEntry() { SourceFile="SONIC2\\STG31.PRS", IniFile = "STG31" },
			new SplitEntry() { SourceFile="SONIC2\\STG32.PRS", IniFile = "STG32" },
			new SplitEntry() { SourceFile="SONIC2\\STG33.PRS", IniFile = "STG33" },
			new SplitEntry() { SourceFile="SONIC2\\STG34.PRS", IniFile = "STG34" },
			new SplitEntry() { SourceFile="SONIC2\\STG35.PRS", IniFile = "STG35" },
			new SplitEntry() { SourceFile="SONIC2\\STG36.PRS", IniFile = "STG36" },
			new SplitEntry() { SourceFile="SONIC2\\STG37.PRS", IniFile = "STG37" },
			new SplitEntry() { SourceFile="SONIC2\\STG38.PRS", IniFile = "STG38" },
			new SplitEntry() { SourceFile="SONIC2\\STG39.PRS", IniFile = "STG39" },
			new SplitEntry() { SourceFile="SONIC2\\STG40.PRS", IniFile = "STG40" },
			new SplitEntry() { SourceFile="SONIC2\\STG41.PRS", IniFile = "STG41" },
			new SplitEntry() { SourceFile="SONIC2\\STG42.PRS", IniFile = "STG42" },
			new SplitEntry() { SourceFile="SONIC2\\STG43.PRS", IniFile = "STG43" },
			new SplitEntry() { SourceFile="SONIC2\\STG44.PRS", IniFile = "STG44" },
			new SplitEntry() { SourceFile="SONIC2\\STG45.PRS", IniFile = "STG45" },

			new SplitEntry() { SourceFile="SONIC2\\BOSS_BIGBOGY.PRS", IniFile = "BOSS_BIGBOGY" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_BIGFOOT.PRS", IniFile = "BOSS_BIGFOOT" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_FDOG.PRS", IniFile = "BOSS_FDOG" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_GOLEM.PRS", IniFile = "BOSS_GOLEM" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_GOLEM_E.PRS", IniFile = "BOSS_GOLEM_E" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_HOTSHOT.PRS", IniFile = "BOSS_HOTSHOT" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_LAST1.PRS", IniFile = "BOSS_LAST1" },
			new SplitEntry() { SourceFile="SONIC2\\BOSS_LAST2.PRS", IniFile = "BOSS_LAST2" },

			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGDARK.PRS", IniFile = "CHAOSTGDARK" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGENTRANCE.PRS", IniFile = "CHAOSTGENTRANCE" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGHERO.PRS", IniFile = "CHAOSTGHERO" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGKINDER.PRS", IniFile = "CHAOSTGKINDER" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLAND000.PRS", IniFile = "CHAOSTGLOBBYLAND000" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLAND00K.PRS", IniFile = "CHAOSTGLOBBYLAND00K" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLAND0DK.PRS", IniFile = "CHAOSTGLOBBYLAND0DK" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLANDH0K.PRS", IniFile = "CHAOSTGLOBBYLANDH0K" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGLOBBYLANDHDK.PRS", IniFile = "CHAOSTGLOBBYLANDHDK" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGNEUT.PRS", IniFile = "CHAOSTGNEUT" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGRACE.PRS", IniFile = "CHAOSTGRACE" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGRACELANDD.PRS", IniFile = "CHAOSTGRACELANDD" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGRACELANDH.PRS", IniFile = "CHAOSTGRACELANDH" },
			new SplitEntry() { SourceFile="SONIC2\\CHAOSTGRACELANDN.PRS", IniFile = "CHAOSTGRACELANDN" }
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
