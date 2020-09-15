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

namespace SAToolsHub
{
	public partial class newProj : Form
	{
		string sadxPath = Program.Settings.SADXPCPath;
		string sa2pcPath = Program.Settings.SA2PCPath;

		SplitData[] sadxpc_exesplit = new SplitData[]
		{
			//Stages
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG00.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG01.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG02.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG03.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG04.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG05.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG06.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG07.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG08.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG09.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG10.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "STG12.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "ADV00.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "ADV01.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "ADV02.ini" },

			////Bosses
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS0.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS2.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS4.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS6.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_CHAOS7.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_EGM1.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_EGM2.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_EGM3.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_ROBO.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_E101.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "B_E101R.ini" },

			////Minigames
			//new SplitData() { dataFile="sonic.exe", iniFile = "SANDBOARD.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "MINICART.ini" },

			////Chao
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL_GARDEN00.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL_GARDEN01.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL-GARDEN02.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL_RACE.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "AL_MAIN.ini" },

			////Common
			//new SplitData() { dataFile="sonic.exe", iniFile = "Characters.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "Objects.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "Events.ini" },
			//new SplitData() { dataFile="sonic.exe", iniFile = "Misc.ini" }
			new SplitData() { dataFile="sonic.exe", iniFile = "sonic.ini" }
		};

		SplitData[] sadxpc_dllsplit = new SplitData[]
		{
			new SplitData() { dataFile="system/ADV00MODELS.DLL", iniFile = "adv00models.ini" },
			new SplitData() { dataFile="system/ADV01CMODELS.DLL", iniFile = "adv01cmodels.ini" },
			new SplitData() { dataFile="system/ADV01MODELS.DLL", iniFile = "adv01models.ini" },
			new SplitData() { dataFile="system/ADV02MODELS.DLL", iniFile = "adv02models.ini" },
			new SplitData() { dataFile="system/ADV03MODELS.DLL", iniFile = "adv03models.ini" },
			new SplitData() { dataFile="system/BOSSCHAOS0MODELS.DLL", iniFile = "bosschaos0models.ini" },
			new SplitData() { dataFile="system/CHAOSTGGARDEN02MR_DAYTIME.DLL", iniFile = "chaostggarden02mr_daytime.ini" },
			new SplitData() { dataFile="system/CHAOSTGGARDEN02MR_EVENING.DLL", iniFile = "chaostggarden02mr_evening.ini" },
			new SplitData() { dataFile="system/CHAOSTGGARDEN02MR_NIGHT.DLL", iniFile = "chaostggarden02mr_night.ini" }
			// chrmodels and chrmodels_orig are special cases
		};

		SplitData[] sa2pc_exesplit = new SplitData[]
{
			new SplitData() { dataFile = "sonic2app.exe", iniFile = "splitsonic2app.ini" }
		};

		SplitMDLData[] sa2pc_mdlsplit = new SplitMDLData[]
		{
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\amymdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"amymtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\bknuckmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"knuckmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\brougemdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"rougemtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\chaos0mdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"chaos0mtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\cwalkmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"cwalkmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\dwalkmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"dwalkmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\eggmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"eggmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\ewalk1mdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"ewalkmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\ewalk2mdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"ewalkmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\ewalkmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"ewalkmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\knuckmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"knuckmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\metalsonicmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"metalsonicmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\milesmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"milesmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\rougemdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"rougemtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\shadow1mdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"teriosmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\sonic1mdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"sonicmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\sonicmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"sonicmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\sshadowmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"sshadowmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\ssonicmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"ssonicmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\teriosmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"teriosmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\ticalmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"ticalmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\twalk1mdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"twalkmtn.prs"
				}
			},
			new SplitMDLData()
			{
				isBigEndian = true,
				dataFile = "resource\\gd_PC\\twalkmdl.prs",
				animationFiles = new string[]
				{
					"plcommtn.prs",
					"twalkmtn.prs"
				}
			}
		};

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

				case SA_Tools.Game.SA2PC:
					return "SA2PC";
				default:
					break;
			}

			return "";
		}

		private void makeProjectFolders(string projFolder, SA_Tools.Game game)
		{
			string exportFolderPath = Path.Combine(projFolder, "Exports");
			Directory.CreateDirectory(exportFolderPath);

			string exportReadmePath = Path.Combine(exportFolderPath, "readme.txt");
			File.WriteAllLines(exportReadmePath, new string[] { "Use this for storing models for export." });

			string importFolderPath = Path.Combine(projFolder, "Imports");
			Directory.CreateDirectory(importFolderPath);

			string importReadmePath = Path.Combine(importFolderPath, "readme.txt");
			File.WriteAllLines(importReadmePath, new string[] { "Use this for storing models for import." });

			string sourceFolderPath = Path.Combine(projFolder, "Source");
			Directory.CreateDirectory(sourceFolderPath);
			string sourceReadmePath = Path.Combine(sourceFolderPath, "readme.txt");
			File.WriteAllLines(sourceReadmePath, new string[] { "Use this folder for storing your source code." });

			string systemPath = Path.Combine(projFolder, GamePathChecker.GetSystemPathName(game));
			Directory.CreateDirectory(systemPath);
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
			if (game == SA_Tools.Game.SADX)
			{
				SADXModInfo modInfo = new SADXModInfo
				{
					Name = txtName.Text,
					Author = txtAuthor.Text,
					Description = txtDesc.Text,
					Version = string.Format("0")

				};
				string outputPath = Path.Combine(projectFolder, string.Format("mod.ini"));

				SA_Tools.IniSerializer.Serialize(modInfo, outputPath);
			}
			else if (game == SA_Tools.Game.SA2PC)
			{
				SA2ModInfo modInfo = new SA2ModInfo
				{
					Name = txtName.Text,
					Author = txtAuthor.Text,
					Description = txtDesc.Text,
					Version = string.Format("0")
				};
				string outputPath = Path.Combine(projectFolder, string.Format("mod.ini"));

				SA_Tools.IniSerializer.Serialize(modInfo, outputPath);
			}			
		}

		private void splitFiles(SplitData splitData, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder)
		{
			string datafilename = Path.Combine(gameFolder, splitData.dataFile);
			string inifilename = Path.Combine(iniFolder, splitData.iniFile);
			string projectFolderName = outputFolder;

			progress.StepProgress();
			progress.SetStep("Splitting: " + splitData.dataFile);

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
			progress.SetStep("Splitting EXE Content");

			//Split EXE Data
			foreach (SplitData splitExeData in sadxpc_exesplit)
			{
				splitFiles(splitExeData, progress, gameFolder, iniFolder, outputFolder);
			}

			progress.StepProgress();
			progress.SetStep("Splitting DLL Content");
			//Split DLL Data
			foreach (SplitData splitDllData in sadxpc_dllsplit)
			{
				splitFiles(splitDllData, progress, gameFolder, iniFolder, outputFolder);
			}

			// do our last split, chrmodels
			SplitData chrmodelsSplitData = new SplitData();

			if (File.Exists(Path.Combine(gameFolder, "system/CHRMODELS_orig.dll")))
			{
				chrmodelsSplitData = new SplitData()
				{
					dataFile = "system/CHRMODELS_orig.dll",
					iniFile = "chrmodels.ini"
				};
			}
			else
			{
				chrmodelsSplitData = new SplitData()
				{
					dataFile = "system/CHRMODELS.dll",
					iniFile = "chrmodels.ini"
				};
			}

			splitFiles(chrmodelsSplitData, progress, gameFolder, iniFolder, outputFolder);

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
			// split data dll
			#region Split Data DLL

			progress.StepProgress();
			progress.SetStep("Splitting DLL Content");

			SplitData dllSplitData = new SplitData();

			if (File.Exists(Path.Combine(gameFolder, "resource/gd_PC/DLL/Win32/Data_DLL_orig.dll")))
			{
				dllSplitData = new SplitData()
				{
					dataFile = "resource/gd_PC/DLL/Win32/Data_DLL_orig.dll",
					iniFile = "data_dll.ini"
				};
			}
			else
			{
				dllSplitData = new SplitData()
				{
					dataFile = "resource/gd_PC/DLL/Win32/Data_DLL.dll",
					iniFile = "data_dll.ini"
				};
			}

			splitFiles(dllSplitData, progress, gameFolder, iniFolder, outputFolder);
			#endregion

			// run split mdl commands
			progress.StepProgress();
			progress.SetStep("Splitting Character Files");

			foreach (SplitMDLData splitMDL in sa2pc_mdlsplit)
			{
				string filePath = Path.Combine(gameFolder, splitMDL.dataFile);
				string fileOutputFolder = Path.GetDirectoryName(Path.Combine(outputFolder, splitMDL.dataFile));
				Directory.CreateDirectory(fileOutputFolder);

				SA_Tools.SplitMDL.SplitMDL.Split(splitMDL.isBigEndian, filePath,
					fileOutputFolder, splitMDL.animationFiles);
			}

			// split sonic2app
			progress.StepProgress();
			progress.SetStep("Splitting EXE Content");
			foreach (SplitData splitExeData in sa2pc_exesplit)
			{
				splitFiles(splitExeData, progress, gameFolder, iniFolder, outputFolder);
			}
		}

		private void splitGame(SA_Tools.Game game, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string iniFolder, string projFolder)
		{
			switch (game)
			{
				case (SA_Tools.Game.SADX):
					splitSADXPC(progress, gameFolder, iniFolder, projFolder);
					break;
				case (SA_Tools.Game.SA2PC):
					splitSA2PC(progress, gameFolder, iniFolder, projFolder);
					break;
				default:
					break;
			}

		}

		public newProj()
		{
			InitializeComponent();
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
			Stream projFileStream;
			SA_Tools.Game game;
			string gamePath = "";
			string iniFolder = "";


			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Project File (*.xml)|*.xml";
			saveFileDialog1.RestoreDirectory = true;
			string projFolder;
			game = GetGameForRadioButtons();
			if (radSADX.Checked)
			{
				saveFileDialog1.InitialDirectory = sadxPath;
				gamePath = sadxPath;
			}
			else if (radSA2PC.Checked)
			{
				saveFileDialog1.InitialDirectory = sa2pcPath;
				gamePath = sa2pcPath;
			}


			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((projFileStream = saveFileDialog1.OpenFile()) != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ProjectManagement.ProjectTemplate));
					TextWriter writer = new StreamWriter(projFileStream);
					if (checkBox1.Checked)
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

					ProjectManagement.ProjectTemplate templateFile = new ProjectManagement.ProjectTemplate();
					if (radSADX.Checked)
					{
						templateFile.GameName = "SADX";
						templateFile.CanBuild = true;
						templateFile.GameSystemFolder = sadxPath;
					}
					else if (radSA2PC.Checked)
					{
						templateFile.GameName = "SA2PC";
						templateFile.CanBuild = true;
						templateFile.GameSystemFolder = sa2pcPath;
					}
					templateFile.ModSystemFolder = projFolder;

					serializer.Serialize(writer, templateFile);
					projFileStream.Close();

					using (SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress = new SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog("Creating project"))
					{
						Invoke((Action)progress.Show);
#if DEBUG
						iniFolder = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../Configuration/" + GetIniFolderForGame(game);
#endif

#if !DEBUG
						iniFolder = Path.GetDirectoryName(Application.ExecutablePath) + "/../" + GetIniFolderForGame(game);
#endif
						
						splitGame(game, progress, gamePath, iniFolder, projFolder);
						progress.StepProgress();
						progress.SetStep("Creating Folders");
						makeProjectFolders(projFolder, game);
						progress.StepProgress();
						progress.SetStep("Generating Mod File");
						GenerateModFile(game, projFolder);

						Invoke((Action)progress.Close);
					}
				}
			}
			this.Close();
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
	}
}
