using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;
using Ookii.Dialogs.Wpf;
using SonicRetro.SAModel.SAEditorCommon.UI;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.ModManagement;
using ProjectManagement;
using Fclp.Internals.Extensions;
using System.Threading;
using System.Linq;

namespace SAToolsHub
{
	public partial class newProj : Form
	{
		public Action CreationCanceled;
		private static BackgroundWorker backgroundWorker1 = new BackgroundWorker();
		Stream projFileStream;
		SaveFileDialog saveFileDialog1;
		SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog splitProgress;
		string sadxPath = Program.Settings.SADXPCPath;
		string sa2pcPath = Program.Settings.SA2PCPath;
		SA_Tools.Game game;
		string gamePath;
		string iniFolder;
		string projFolder;

		List<SplitEntry> sadxpc_split = new List<SplitEntry>
		{
			//Stages
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG01", CommonName="Emerald Coast Data" },
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
			new SplitEntry() { SourceFile="sonic.exe", IniFile = "sonic", CommonName = "EXE Data" },

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

		List<SplitEntry> sa2pc_split = new List<SplitEntry>
		{
			new SplitEntry() { SourceFile = "sonic2app.exe", IniFile = "splitsonic2app", CommonName="EXE Data" },
			new SplitEntry() { SourceFile = "resource/gd_PC/DLL/Win32/Data_DLL_orig.dll", IniFile = "data_dll", CommonName="DLL Data"}
		};

		List<SplitEntryMDL> sa2pc_mdlsplit = new List<SplitEntryMDL>
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

		public newProj()
		{
			InitializeComponent();
			backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
			backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);

			if (sadxPath.Length > 0)
			{
				radSADX.Checked = true;
			}
			else if (sadxPath.Length <= 0 && sa2pcPath.Length > 0)
			{
				radSA2PC.Checked = true;
			}
		}

		private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!(e.Error == null))
			{
				MessageBox.Show("Project failed to split!", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				DialogResult successDiag = MessageBox.Show("Project successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
				if (successDiag == DialogResult.OK)
				{
					SAToolsHub.newProjFile = Path.Combine((Path.GetDirectoryName(saveFileDialog1.FileName)), saveFileDialog1.FileName);
					this.Close();
				}
			}
		}

		private SA_Tools.Game GetGameForRadioButtons()
		{
			if (radSADX.Checked) return SA_Tools.Game.SADX;
			else if (radSA2PC.Checked) return SA_Tools.Game.SA2B;
			else return SA_Tools.Game.SA1;
		}

		private string GetIniFolderForGame(SA_Tools.Game game)
		{
			switch (game)
			{
				case SA_Tools.Game.SA1:
					return "SA1";

				case SA_Tools.Game.SADX:
					return "SADXPC";

				case SA_Tools.Game.SA2:
					return "SA2";

				case SA_Tools.Game.SA2B:
					return "SA2PC";
				default:
					break;
			}

			return "";
		}

		private void makeProjectFolders(string projFolder, SA_Tools.Game game)
		{
			string exportFolderPath = Path.Combine(projFolder, "Exports");
			if (!Directory.Exists(exportFolderPath))
				Directory.CreateDirectory(exportFolderPath);

			string importFolderPath = Path.Combine(projFolder, "Imports");
			if (!Directory.Exists(importFolderPath))
				Directory.CreateDirectory(importFolderPath);

			string sourceFolderPath = Path.Combine(projFolder, "Source");
			if (!Directory.Exists(sourceFolderPath))
				Directory.CreateDirectory(sourceFolderPath);

			string systemPath = Path.Combine(projFolder, GamePathChecker.GetSystemPathName(game));
			if (!Directory.Exists(systemPath))
				Directory.CreateDirectory(systemPath);

			string exportReadmePath = Path.Combine(exportFolderPath, "readme.txt");
			File.WriteAllLines(exportReadmePath, new string[] { "Use this for storing models for export." });

			string importReadmePath = Path.Combine(importFolderPath, "readme.txt");
			File.WriteAllLines(importReadmePath, new string[] { "Use this for storing models for import." });

			string sourceReadmePath = Path.Combine(sourceFolderPath, "readme.txt");
			File.WriteAllLines(sourceReadmePath, new string[] { "Use this folder for storing your source code." });
		}

		private void CopyFolder(string sourceFolder, string destinationFolder)
		{
			string[] files = Directory.GetFiles(sourceFolder);

			Directory.CreateDirectory(destinationFolder);

			foreach (string objdef in files)
			{
				FileInfo objdefFileInfo = new FileInfo(objdef);
				if (objdefFileInfo.Name.Equals("SADXObjectDefinitions.csproj")) continue;

				// copy
				string filePath = Path.Combine(sourceFolder, objdefFileInfo.Name);
				string destinationPath = Path.Combine(destinationFolder, objdefFileInfo.Name);
				File.Copy(filePath, destinationPath, true);
			}

			string[] directories = Directory.GetDirectories(sourceFolder);

			foreach (string directory in directories)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(directory);
				if (directoryInfo.Name.Equals("bin") || directoryInfo.Name.Equals("obj")) continue;

				string copySrcDir = Path.Combine(sourceFolder, directoryInfo.Name);
				string copyDstDir = Path.Combine(destinationFolder, directoryInfo.Name);

				CopyFolder(copySrcDir, copyDstDir);
			}
		}

		private string GetObjDefsDirectory()
		{
#if DEBUG
			return Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SADXObjectDefinitions/";
#endif

#if !DEBUG
			return Path.GetDirectoryName(Application.ExecutablePath) + "/../" + GetIniFolderForGame(SA_Tools.Game.SADX) + "/objdefs/";
#endif
		}

		private void GenerateModFile(SA_Tools.Game game, string projectFolder)
		{
			string outputPath;
			switch (game)
			{
				case (SA_Tools.Game.SADX):
					SADXModInfo modInfoSADX = new SADXModInfo
					{
						Name = txtName.Text,
						Author = txtAuthor.Text,
						Description = txtDesc.Text,
						Version = string.Format("0")

					};
					outputPath = Path.Combine(projectFolder, string.Format("mod.ini"));

					SA_Tools.IniSerializer.Serialize(modInfoSADX, outputPath);
					break;

				case (SA_Tools.Game.SA2B):
					SA2ModInfo modInfoSA2PC = new SA2ModInfo
					{
						Name = txtName.Text,
						Author = txtAuthor.Text,
						Description = txtDesc.Text,
						Version = string.Format("0")
					};
					outputPath = Path.Combine(projectFolder, string.Format("mod.ini"));

					SA_Tools.IniSerializer.Serialize(modInfoSA2PC, outputPath);
					break;

				default:
					break;
			}
	
		}

		private void splitFiles(SplitEntry splitData, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder)
		{
			string datafilename = Path.Combine(gameFolder, splitData.SourceFile);
			string inifilename = Path.Combine(iniFolder, (splitData.IniFile + ".ini"));
			string projectFolderName = (outputFolder + "\\");

			progress.StepProgress();
			progress.SetStep("Splitting " + splitData.CommonName + " data from " + splitData.SourceFile);

			#region Validating Inputs
			if (!File.Exists(datafilename))
			{
				MessageBox.Show((datafilename + " is missing.\n\nPress OK to abort."), "Split Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				throw new Exception(SA_Tools.Split.SplitERRORVALUE.NoSourceFile.ToString());
				//return (int)ERRORVALUE.NoSourceFile;
			}

			if (!File.Exists(inifilename))
			{
				MessageBox.Show((inifilename + " is missing.\n\nPress OK to abort."), "Split Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				throw new Exception(SA_Tools.Split.SplitERRORVALUE.NoDataMapping.ToString());
				//return (int)ERRORVALUE.NoDataMapping;
			}
			#endregion

			// switch on file extension - if dll, use dll splitter
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(datafilename);

			int result = (fileInfo.Extension.ToLower().Contains("dll")) ? SA_Tools.SplitDLL.SplitDLL.SplitDLLFile(datafilename, inifilename, projectFolderName) :
				SA_Tools.Split.Split.SplitFile(datafilename, inifilename, projectFolderName);
		}

		private void splitSADXPC(SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder)
		{
			progress.StepProgress();
			progress.SetTask("Splitting Game Content");

			//Split EXE Data
			foreach (SplitEntry splitExeData in sadxpc_split)
			{
				splitFiles(splitExeData, progress, gameFolder, iniFolder, outputFolder);
			}

			// copy sadxlvl.ini
			string sadxlvlIniSourcePath = Path.GetFullPath(Path.Combine(iniFolder, "sadxlvl.ini"));
			string sadxlvlIniOutputPath = Path.GetFullPath(Path.Combine(outputFolder, "sadxlvl.ini"));
			File.Copy(sadxlvlIniSourcePath, sadxlvlIniOutputPath, true);

			// copy objdefs.ini
			File.Copy(Path.Combine(iniFolder, "objdefs.ini"), Path.Combine(outputFolder, "objdefs.ini"), true);

			// copy objdefs files (this needs to be turned into a recursive folder copy)
			string objdefsPath = GetObjDefsDirectory();
			string outputObjdefsPath = Path.Combine(outputFolder, "objdefs");

			CopyFolder(objdefsPath, outputObjdefsPath);
		}

		private void splitSA2PC(SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder)
		{
			progress.StepProgress();
			progress.SetTask("Splitting Game Content");
			foreach (SplitEntry splitExeData in sa2pc_split)
			{
				splitFiles(splitExeData, progress, gameFolder, iniFolder, outputFolder);
			}

			// run split mdl commands
			progress.StepProgress();
			progress.SetTask("Splitting Character Files");

			foreach (SplitEntryMDL splitMDL in sa2pc_mdlsplit)
			{
				progress.StepProgress();
				progress.SetStep("Splitting " + splitMDL.ModelFile);
				string filePath = Path.Combine(gameFolder, splitMDL.ModelFile);
				string fileOutputFolder = Path.GetDirectoryName(Path.Combine(outputFolder, splitMDL.ModelFile));
				Directory.CreateDirectory(fileOutputFolder);

				SA_Tools.SplitMDL.SplitMDL.Split(splitMDL.BigEndian, filePath,
					fileOutputFolder, splitMDL.MotionFiles.ToArray());
			}
		}

		private void splitGame(SA_Tools.Game game, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string iniFolder, string projFolder)
		{
			switch (game)
			{
				case (SA_Tools.Game.SADX):
					splitSADXPC(progress, gameFolder, iniFolder, projFolder);
					break;
				case (SA_Tools.Game.SA2B):
					splitSA2PC(progress, gameFolder, iniFolder, projFolder);
					break;
				default:
					break;
			}

		}

		private void newProj_Shown(object sender, EventArgs e)
		{
			bool sadxpcIsValid = GamePathChecker.CheckSADXPCValid(
				Program.Settings.SADXPCPath, out string sadxFailReason);

			bool sa2pcIsValid = GamePathChecker.CheckSA2PCValid(
				Program.Settings.SA2PCPath, out string sa2pcInvalidReason);

			radSADX.Enabled = sadxpcIsValid;
			radSA2PC.Enabled = sa2pcIsValid;

			txtAuthor.Text = String.Empty;
			txtDesc.Text = String.Empty;
			txtName.Text = String.Empty;
			txtProjFolder.Text = String.Empty;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			
			var folderDialog = new VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtProjFolder.Text = folderDialog.SelectedPath;
			}
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Project File (*.xml)|*.xml";
			saveFileDialog1.RestoreDirectory = true;
			saveFileDialog1.FileName = txtName.Text;

			if (checkBox1.Checked && (!txtProjFolder.Text.IsNullOrWhiteSpace()))
			{
				saveFileDialog1.InitialDirectory = txtProjFolder.Text;
			}
			else
			{
				if (radSADX.Checked)
				{
					saveFileDialog1.InitialDirectory = sadxPath;
				}
				else if (radSA2PC.Checked)
				{
					saveFileDialog1.InitialDirectory = sa2pcPath;
				}
			}



			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((projFileStream = saveFileDialog1.OpenFile()) != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ProjectManagement.ProjectTemplate));
					TextWriter writer = new StreamWriter(projFileStream);
					if (checkBox1.Checked && (!txtProjFolder.Text.IsNullOrWhiteSpace()))
					{
						projFolder = txtProjFolder.Text;
					}
					else
					{
						projFolder = Path.Combine((Path.GetDirectoryName(saveFileDialog1.FileName)), Path.GetFileNameWithoutExtension(saveFileDialog1.FileName));
						if (!Directory.Exists(projFolder))
						{
							Directory.CreateDirectory(projFolder);
						}
					}

					game = GetGameForRadioButtons();
					ProjectManagement.ProjectTemplate templateFile = new ProjectManagement.ProjectTemplate();
					if (radSADX.Checked)
					{
						gamePath = sadxPath;
						templateFile.GameName = "SADX";
						templateFile.CanBuild = true;
						templateFile.GameSystemFolder = sadxPath;
						templateFile.ModSystemFolder = projFolder;
						templateFile.SplitEntries = sadxpc_split;
					}
					else if (radSA2PC.Checked)
					{
						gamePath = sa2pcPath;
						templateFile.GameName = "SA2PC";
						templateFile.CanBuild = true;
						templateFile.GameSystemFolder = sa2pcPath;
						templateFile.ModSystemFolder = projFolder;
						templateFile.SplitEntries = sa2pc_split;
						templateFile.SplitMDLEntries = sa2pc_mdlsplit;
					}
					

					serializer.Serialize(writer, templateFile);
					projFileStream.Close();
#if !DEBUG
					backgroundWorker1.RunWorkerAsync();
#endif
#if DEBUG
					backgroundWorker1_DoWork(null, null);
					BackgroundWorker1_RunWorkerCompleted(null, null);
#endif
				}
			}
		}

		private void newProj_Load(object sender, EventArgs e)
		{

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox1.Checked)
			{
				txtProjFolder.Enabled = true;
				btnBrowse.Enabled = true;
			}
			else
			{
				txtProjFolder.Enabled = false;
				btnBrowse.Enabled = false;
			}
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			using (splitProgress = new SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog("Creating project"))
			{
				Invoke((Action)splitProgress.Show);
#if DEBUG
				iniFolder = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../Configuration/" + GetIniFolderForGame(GetGameForRadioButtons());
#endif

#if !DEBUG
				iniFolder = Path.GetDirectoryName(Application.ExecutablePath) + "/../" + GetIniFolderForGame(GetGameForRadioButtons());
#endif
				splitGame(game, splitProgress, gamePath, iniFolder, projFolder);
				splitProgress.StepProgress();
				splitProgress.SetTask("Creating Folders");
				makeProjectFolders(projFolder, game);
				splitProgress.StepProgress();
				splitProgress.SetTask("Generating Mod File");
				GenerateModFile(game, projFolder);

				Invoke((Action)splitProgress.Close);
			}
		}
	}
}
