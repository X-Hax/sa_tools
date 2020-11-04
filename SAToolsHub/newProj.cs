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
using SA_Tools.Split;

namespace SAToolsHub
{
	public partial class newProj : Form
	{
		public Action CreationCanceled;
		private static BackgroundWorker backgroundWorker1 = new BackgroundWorker();
		Stream projFileStream;
		SaveFileDialog saveFileDialog1;
		SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog splitProgress;

		//Moddable Game Paths
		string sadxPath = Program.Settings.SADXPCPath;
		string sa2pcPath = Program.Settings.SA2PCPath;
		//SA1 Paths
		string sa1Path = Program.Settings.SA1Path;
		string sa1adPath = Program.Settings.SA1ADPath;
		//SADX (Non-PC) Paths
		string sadxgcPath = Program.Settings.SADXGCPath;
		string sadxgcpPath = Program.Settings.SADXGCPPath;
		string sadxgcrPath = Program.Settings.SADXGCRPath;
		string sadx360Path = Program.Settings.SADX360Path;
		//SA2 Paths
		string sa2Path = Program.Settings.SA2Path;
		string sa2ttPath = Program.Settings.SA2TTPath;
		string sa2pPath = Program.Settings.SA2PPath;

		SA_Tools.Game game;
		string gameName;
		string gamePath;
		string iniFolder;
		string projFolder;
		bool ableBuild = new bool();
		List<SplitEntry> splitEntries = new List<SplitEntry>();

		public newProj()
		{
			InitializeComponent();
			backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
			backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
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
			if (tabControl1.SelectedTab == tabMod)
			{
				if (radSADX.Checked) return SA_Tools.Game.SADX;
				else if (radSA2PC.Checked) return SA_Tools.Game.SA2B;
				else return SA_Tools.Game.SA1;
			}
			else if (tabControl1.SelectedTab == tabBin)
			{
				if (radSA1.Checked) return SA_Tools.Game.SA1;
				else if (radSA1AD.Checked) return SA_Tools.Game.SA1AD;
				else if (radSA2.Checked) return SA_Tools.Game.SA2;
				else if (radSA2TT.Checked) return SA_Tools.Game.SA2TT;
				else if (radSA2P.Checked) return SA_Tools.Game.SA2P;
				else if (radSADXP.Checked) return SA_Tools.Game.SADXGCP;
				else if (radSADXR.Checked) return SA_Tools.Game.SADXGCR;
				else if (radSADX360.Checked) return SA_Tools.Game.SADX360;
				else return SA_Tools.Game.SA1;
			}
			else
				return SA_Tools.Game.SA1;
		}

		private string GetIniFolderForGame(SA_Tools.Game game)
		{
			switch (game)
			{
				case SA_Tools.Game.SADX:
					return "SADXPC";

				case SA_Tools.Game.SA2B:
					return "SA2PC";

				case SA_Tools.Game.SA1:
					return "SA1";

				case SA_Tools.Game.SA1AD:
					return "Autodemo";

				case SA_Tools.Game.SA2:
					return "SA2";

				case SA_Tools.Game.SA2TT:
					return "SA2TheTrial";

				case SA_Tools.Game.SADX360:
					return "SADX360";
				
				default:
					break;
			}

			return "";
		}

		private void makeProjectFolders(string projFolder, SA_Tools.Game game)
		{
			string[] readMeSADX = {
				"Welcome to your new SADX Mod! The info below will assist with some additional folders created for your Mod.\n\n" +
				"Exports - You can store models for Exports here.\n" +
				"Imports - You can store models for Imports here.\n" +
				"Source - You can store your source Code here.\n" +
				"system - SADX's system folder. Store your textures (PVM) here. Stage object layouts are stored here as well.\n" +
				"textures - You can store PVMX and Texture Pack Archives here.\n\n" +
				"Please refer to the Help drop down in the SA Tools Hub for additional resources or you can reach members for help in the X-Hax Discord."
			};

			string[] readMeSA2PC = {
				"Welcome to your new SA2PC Mod! The info below will assist with some additional folders created for your Mod.\n\n" +
				"Exports - You can store models for Exports here.\n" +
				"Imports - You can store models for Imports here.\n" +
				"Source - You can store your source Code here.\n" +
				"gd_PC - SA2's system folder. Store your textures (GVM/PAK) here. Stage object layouts are stored here as well.\n\n" +
				"Please refer to the Help drop down in the SA Tools Hub for additional resources or you can reach members for help in the X-Hax Discord."
			};

			//Exports Folder
			string exportFolderPath = Path.Combine(projFolder, "Exports");
			if (!Directory.Exists(exportFolderPath))
				Directory.CreateDirectory(exportFolderPath);

			//Imports Folder
			string importFolderPath = Path.Combine(projFolder, "Imports");
			if (!Directory.Exists(importFolderPath))
				Directory.CreateDirectory(importFolderPath);

			//Source Folder
			string sourceFolderPath = Path.Combine(projFolder, "Source");
			if (!Directory.Exists(sourceFolderPath))
				Directory.CreateDirectory(sourceFolderPath);

			//Game System Folder
			string systemPath = Path.Combine(projFolder, GamePathChecker.GetSystemPathName(game));
			if (!Directory.Exists(systemPath))
				Directory.CreateDirectory(systemPath);

			string projReadMePath = Path.Combine(projFolder, "ReadMe.txt");

			switch (game)
			{
				case (SA_Tools.Game.SADX):
					string texturesPath = Path.Combine(projFolder, "textures");
					if (!Directory.Exists(texturesPath))
						Directory.CreateDirectory(texturesPath);

					File.WriteAllLines(projReadMePath, readMeSADX);
					break;
				case (SA_Tools.Game.SA2B):
					File.WriteAllLines(projReadMePath, readMeSA2PC);
					break;
				default:
					break;
			}			
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
			if (Directory.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "/../" + GetIniFolderForGame(SA_Tools.Game.SADX)))
				return Path.GetDirectoryName(Application.ExecutablePath) + "/../" + GetIniFolderForGame(SA_Tools.Game.SADX) + "/objdefs/";
			else
				return Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SADXObjectDefinitions/";
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

		private void enableButtons()
		{
			bool sadxpcIsValid = GamePathChecker.CheckSADXPCValid(
				Program.Settings.SADXPCPath, out string sadxFailReason);

			bool sa2pcIsValid = GamePathChecker.CheckSA2PCValid(
				Program.Settings.SA2PCPath, out string sa2pcInvalidReason);

			bool sa1IsValid = GamePathChecker.CheckBinaryPath("DC", SA_Tools.Game.SA1,
				Program.Settings.SA1Path, out string sa1InvalidReason);

			bool sa1adIsValid = GamePathChecker.CheckBinaryPath("DC", SA_Tools.Game.SA1AD,
				Program.Settings.SA1ADPath, out string sa1adInvalidReason);

			bool sa2IsValid = GamePathChecker.CheckBinaryPath("DC", SA_Tools.Game.SA2,
				Program.Settings.SA2Path, out string sa2InvalidReason);

			bool sa2ttIsValid = GamePathChecker.CheckBinaryPath("DC", SA_Tools.Game.SA2TT,
				Program.Settings.SA2TTPath, out string sa2ttInvalidReason);

			bool sa2pIsValid = GamePathChecker.CheckBinaryPath("DC", SA_Tools.Game.SA2P,
				Program.Settings.SA2PPath, out string sa2pInvalidReason);

			bool sadxgcIsValid = GamePathChecker.CheckBinaryPath("GC", SA_Tools.Game.SADXGC,
				Program.Settings.SADXGCPath, out string sadxgcIsInvalid);

			bool sadxgcpIsValid = GamePathChecker.CheckBinaryPath("GC", SA_Tools.Game.SADXGCP,
				Program.Settings.SADXGCPPath, out string sadxgcpIsInvalid);

			bool sadxgcrIsValid = GamePathChecker.CheckBinaryPath("GC", SA_Tools.Game.SADXGCR,
				Program.Settings.SADXGCRPath, out string sadxgcrIsInvalid);

			bool sadx360IsValid = GamePathChecker.CheckBinaryPath("360", SA_Tools.Game.SADX360,
				Program.Settings.SADX360Path, out string sadx360InvalidReason);

			radSADX.Enabled = sadxpcIsValid;
			radSA2PC.Enabled = sa2pcIsValid;
			radSA1.Enabled = sa1IsValid;
			radSA1AD.Enabled = sa1adIsValid;
			radSA2.Enabled = sa2IsValid;
			radSA2TT.Enabled = sa2ttIsValid;
			radSA2P.Enabled = sa2pIsValid;
			radSADXGC.Enabled = sadxgcIsValid;
			radSADXP.Enabled = sadxgcpIsValid;
			radSADXR.Enabled = sadxgcrIsValid;
			radSADX360.Enabled = sadx360IsValid;
		}

		private void setProjVariables(SA_Tools.Game game)
		{
			switch (game)
			{
				case (SA_Tools.Game.SADX):
					gameName = "SADX";
					gamePath = sadxPath;
					splitEntries = ProjectManagement.BuildSplits.sadxpc_split;
					break;
				case (SA_Tools.Game.SA2B):
					gameName = "SA2PC";
					gamePath = sa2Path;
					splitEntries = ProjectManagement.BuildSplits.sa2pc_split;
					break;
				case (SA_Tools.Game.SA1):
					gameName = "SA1";
					gamePath = sa1Path;
					splitEntries = ProjectManagement.NonBuildSplits.sa1_final_split;
					break;
				case (SA_Tools.Game.SA1AD):
					gameName = "SA1AD";
					gamePath = sa1adPath;
					splitEntries = ProjectManagement.NonBuildSplits.sa1_autodemo_split;
					break;
				case (SA_Tools.Game.SA2):
					gameName = "SA2";
					gamePath = sa2Path;
					splitEntries = ProjectManagement.NonBuildSplits.sa2_final_split;
					break;
				case (SA_Tools.Game.SA2TT):
					gameName = "SA2TT";
					gamePath = sa2ttPath;
					splitEntries = ProjectManagement.NonBuildSplits.sa2_trial_split;
					break;
				case (SA_Tools.Game.SA2P):
					gameName = "SA2P";
					gamePath = sa2pPath;
					splitEntries = ProjectManagement.NonBuildSplits.sa2_preview_split;
					break;
				case (SA_Tools.Game.SADXGC):
					gameName = "SADXGC";
					gamePath = sadxgcPath;
					//splitEntries = ProjectManagement.NonBuildSplits.sadxgc_final_split;
					break;
				case (SA_Tools.Game.SADXGCP):
					gameName = "SADXGCP";
					gamePath = sadxgcpPath;
					splitEntries = ProjectManagement.NonBuildSplits.sadxgc_preview_split;
					break;
				case (SA_Tools.Game.SADXGCR):
					gameName = "SADXGCR";
					gamePath = sadxgcrPath;
					splitEntries = ProjectManagement.NonBuildSplits.sadxgc_review_split;
					break;
				case (SA_Tools.Game.SADX360):
					gameName = "SADX360";
					gamePath = sadx360Path;
					splitEntries = ProjectManagement.NonBuildSplits.sadx360_proto_split;
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

			foreach (SplitEntry splitData in BuildSplits.sadxpc_split)
			{
				splitFiles(splitData, progress, gameFolder, iniFolder, outputFolder);
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
			foreach (SplitEntry splitData in BuildSplits.sa2pc_split)
			{
				splitFiles(splitData, progress, gameFolder, iniFolder, outputFolder);
			}

			// run split mdl commands
			progress.StepProgress();
			progress.SetTask("Splitting Character Files");

			foreach (SplitEntryMDL splitMDL in BuildSplits.sa2pc_mdlsplit)
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

		private void splitBinary(SA_Tools.Game game, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder)
		{
			List<SplitEntry> gameSplitData = new List<SplitEntry>();
			List<SplitEntryMDL> splitEntryMDL = new List<SplitEntryMDL>();

			progress.StepProgress();
			progress.SetTask("Splitting Game Content");

			switch (game)
			{
				case (SA_Tools.Game.SA1):
					gameSplitData = NonBuildSplits.sa1_final_split;
					break;

				case (SA_Tools.Game.SA1AD):
					gameSplitData = NonBuildSplits.sa1_autodemo_split;
					break;

				case (SA_Tools.Game.SADXGCP):
					gameSplitData = NonBuildSplits.sadxgc_preview_split;
					break;

				case (SA_Tools.Game.SADXGCR):
					gameSplitData = NonBuildSplits.sadxgc_review_split;
					break;

				case (SA_Tools.Game.SADX360):
					gameSplitData = NonBuildSplits.sadx360_proto_split;
					break;

				case (SA_Tools.Game.SA2):
					gameSplitData = NonBuildSplits.sa2_final_split;
					splitEntryMDL = NonBuildSplits.sa2_final_mdlsplit;
					break;

				case (SA_Tools.Game.SA2TT):
					gameSplitData = NonBuildSplits.sa2_trial_split;
					splitEntryMDL = NonBuildSplits.sa2_trial_mdlsplit;
					break;

				case (SA_Tools.Game.SA2P):
					gameSplitData = NonBuildSplits.sa2_preview_split;
					splitEntryMDL = NonBuildSplits.sa2_preview_mdlsplit;
					break;

				default:
					break;
			}

			foreach (SplitEntry splitData in gameSplitData)
				splitFiles(splitData, progress, gameFolder, iniFolder, outputFolder);

			if (splitEntryMDL.Count > 0)
			{
				foreach (SplitEntryMDL splitMDL in splitEntryMDL)
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
					splitBinary(game, progress, gameFolder, iniFolder, projFolder);
					break;
			}

		}

		private void newProj_Shown(object sender, EventArgs e)
		{
			enableButtons();

			txtAuthor.Text = String.Empty;
			txtDesc.Text = String.Empty;
			txtName.Text = String.Empty;
			txtProjFolder.Text = String.Empty;
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			//This is only here as a failsafe in case something goes catastrophically wrong with the RadioButtons check.
			if (tabControl1.SelectedTab == tabMod)
			{
				radSA1.Checked = false;
				radSA1AD.Checked = false;
				radSA2.Checked = false;
				radSA2P.Checked = false;
				radSA2TT.Checked = false;
				radSADX360.Checked = false;
				radSADXGC.Checked = false;
				radSADXP.Checked = false;
				radSADXR.Checked = false;
			}
			else if (tabControl1.SelectedTab == tabBin)
			{
				radSADX.Checked = false;
				radSA2PC.Checked = false;
			}
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
			if (tabControl1.SelectedTab == tabControl1.TabPages["PC Games"])
				saveFileDialog1.FileName = txtName.Text;

			if (checkBox1.Checked && (!txtProjFolder.Text.IsNullOrWhiteSpace()))
			{
				saveFileDialog1.InitialDirectory = txtProjFolder.Text;
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
					setProjVariables(game);
					if (game == SA_Tools.Game.SADX || game == SA_Tools.Game.SA2B)
						ableBuild = true;
					else
						ableBuild = false;

					templateFile.GameName = gameName;
					templateFile.CanBuild = ableBuild;
					templateFile.GameSystemFolder = gamePath;
					templateFile.ModSystemFolder = projFolder;
					templateFile.SplitEntries = splitEntries;
					switch (gameName)
					{
						case ("SA2PC"):
							templateFile.SplitMDLEntries = ProjectManagement.BuildSplits.sa2pc_mdlsplit;
							break;
						case ("SA2"):
							templateFile.SplitMDLEntries = ProjectManagement.NonBuildSplits.sa2_final_mdlsplit;
							break;
						case ("SA2TT"):
							templateFile.SplitMDLEntries = ProjectManagement.NonBuildSplits.sa2_trial_mdlsplit;
							break;
						case ("SA2P"):
							templateFile.SplitMDLEntries = ProjectManagement.NonBuildSplits.sa2_preview_mdlsplit;
							break;
						default:
							break;
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
				string configFolder = GetIniFolderForGame(GetGameForRadioButtons());

				if (Directory.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "/../" + configFolder))
					iniFolder = Path.GetDirectoryName(Application.ExecutablePath) + "/../" + configFolder;
				else
					iniFolder = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../Configuration/" + configFolder;

				splitGame(game, splitProgress, gamePath, iniFolder, projFolder);
				if (game == SA_Tools.Game.SADX || game == SA_Tools.Game.SA2B)
				{
					splitProgress.StepProgress();
					splitProgress.SetTask("Creating Folders");
					makeProjectFolders(projFolder, game);
					splitProgress.StepProgress();
					splitProgress.SetTask("Generating Mod File");
					GenerateModFile(game, projFolder);
				}
				
				Invoke((Action)splitProgress.Close);
			}
		}
	}
}
