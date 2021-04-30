using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;
using Ookii.Dialogs.Wpf;
using SonicRetro.SAModel.SAEditorCommon.ModManagement;
using SAEditorCommon.ProjectManagement;
using SA_Tools.Split;
using SA_Tools.SAArc;

namespace SAToolsHub
{
	public partial class newProj : Form
	{
		Stream projFileStream;
		Templates.ProjectTemplate projectFile;
		SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog splitProgress;

		string templatesPath;
		string gameName;
		string gamePath;
		string projFolder;
		string dataFolder;
		string projName;
		List<Templates.SplitEntry> splitEntries = new List<Templates.SplitEntry>();
		List<Templates.SplitEntryMDL> splitMdlEntries = new List<Templates.SplitEntryMDL>();

		public newProj()
		{
			InitializeComponent();
			backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
			backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
		}

		// TODO: newProj - Migrate some Additional Functions out.
		#region Additional Functions
		private int setProgressMaxStep()
		{
			switch (gameName)
			{
				case "SADXPC":
					return (splitEntries.Count + 3);
				case "SA2PC":
					return (splitEntries.Count + splitMdlEntries.Count + 2);
				case "SA2":
				case "SA2TT":
				case "SA2P":
					return(splitEntries.Count + splitMdlEntries.Count);
				default:
					return (splitEntries.Count);
			}
		}

		Dictionary<string, string> loadTemplateList(string folder)
		{
			Dictionary<string, string> templates = new Dictionary<string, string>();
			string[] templateNames = Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly);
			var ordered = templateNames.OrderByDescending(f => f);

			foreach (string file in ordered)
			{
				templates.Add(Path.GetFileNameWithoutExtension(file), file);
			}

			return templates;
		}

		void openTemplate(string templateSplit)
		{
			Templates.SplitTemplate template = ProjectFunctions.openTemplateFile(templateSplit);

			if (template != null)
			{
				gameName = template.GameInfo.GameName;
				gamePath = template.GameInfo.GameSystemFolder;
				dataFolder = template.GameInfo.DataFolder;
				splitEntries = template.SplitEntries;
				splitMdlEntries = template.SplitMDLEntries;
			}
			else
				comboBox1.SelectedIndex = -1;
			
		}

		private void makeProjectFolders(string projFolder, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string game)
		{
			progress.StepProgress();
			progress.SetStep("Making Additional Mod Folders");
			string[] readMeSADX = {
				"Welcome to your new SADX Mod! The info below will assist with some additional folders created for your Mod.\n\n" +
				"Exports - You can store models for Exports here.\n" +
				"Imports - You can store models for Imports here.\n" +
				"Code - You can store your source Code here.\n" +
				"system - SADX's system folder. Store your textures (PVM) here. Stage object layouts are stored here as well.\n" +
				"textures - You can store PVMX and Texture Pack Archives here.\n\n" +
				"Please refer to the Help drop down in the SA Tools Hub for additional resources or you can reach members for help in the X-Hax Discord."
			};

			string[] readMeSA2PC = {
				"Welcome to your new SA2PC Mod! The info below will assist with some additional folders created for your Mod.\n\n" +
				"Exports - You can store models for Exports here.\n" +
				"Imports - You can store models for Imports here.\n" +
				"Code - You can store your source Code here.\n" +
				"gd_PC - SA2's system folder. Store your textures (GVM/PAK) here. Stage object layouts are stored here as well.\n\n" +
				"Please refer to the Help drop down in the SA Tools Hub for additional resources or you can reach members for help in the X-Hax Discord."
			};

			string systemPath;

			//Exports Folder
			string exportFolderPath = Path.Combine(projFolder, "Exports");
			if (!Directory.Exists(exportFolderPath))
				Directory.CreateDirectory(exportFolderPath);

			//Imports Folder
			string importFolderPath = Path.Combine(projFolder, "Imports");
			if (!Directory.Exists(importFolderPath))
				Directory.CreateDirectory(importFolderPath);

			//Source Folder
			string sourceFolderPath = Path.Combine(projFolder, "Code");
			if (!Directory.Exists(sourceFolderPath))
				Directory.CreateDirectory(sourceFolderPath);

			//Game System Folder
			

			string projReadMePath = Path.Combine(projFolder, "ReadMe.txt");

			switch (game)
			{
				case ("SADXPC"):
					systemPath = Path.Combine(projFolder, "system");
					if (!Directory.Exists(systemPath))
						Directory.CreateDirectory(systemPath);
					string texturesPath = Path.Combine(projFolder, "textures");
					if (!Directory.Exists(texturesPath))
						Directory.CreateDirectory(texturesPath);

					File.WriteAllLines(projReadMePath, readMeSADX);
					break;
				case ("SA2PC"):
					systemPath = Path.Combine(projFolder, "gd_PC");
					if (!Directory.Exists(systemPath))
						Directory.CreateDirectory(systemPath);
					File.WriteAllLines(projReadMePath, readMeSA2PC);
					break;
				default:
					break;
			}
		}

		private void GenerateModFile(string game, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string projectFolder, string name)
		{
			progress.StepProgress();
			progress.SetStep("Generating mod.ini");
			string outputPath;
			switch (game)
			{
				case ("SADXPC"):
					SADXModInfo modInfoSADX = new SADXModInfo
					{
						Name = name

					};
					outputPath = Path.Combine(projectFolder, string.Format("mod.ini"));

					SA_Tools.IniSerializer.Serialize(modInfoSADX, outputPath);
					break;

				case ("SA2PC"):
					SA2ModInfo modInfoSA2PC = new SA2ModInfo
					{
						Name = name
					};
					outputPath = Path.Combine(projectFolder, string.Format("mod.ini"));

					SA_Tools.IniSerializer.Serialize(modInfoSA2PC, outputPath);
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
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);

			if (Directory.Exists(Path.Combine(appPath, "Configuration")))
				return Path.Combine(appPath, "SADXObjectDefinitions");
			else
				return Path.Combine(appPath, dataFolder, "objdefs");
		}

		private void splitFiles(Templates.SplitEntry splitData, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder)
		{
			string datafilename; 
			switch (splitData.IniFile)
			{
				case ("chrmodels"):
					if (File.Exists(Path.Combine(gameFolder, "system/chrmodels_orig.dll")))
						datafilename = Path.Combine(gameFolder, "system/chrmodels_orig.dll");
					else
						datafilename = Path.Combine(gameFolder, splitData.SourceFile);
					break;
				case ("DLL_Data"):
					if (File.Exists(Path.Combine(gameFolder, "resource/gd_PC/DLL/Win32/DLL_Data_orig.dll")))
						datafilename = Path.Combine(gameFolder, "resource/gd_PC/DLL/Win32/DLL_Data_orig.dll");
					else
						datafilename = Path.Combine(gameFolder, splitData.SourceFile);
					break;
				default:
					datafilename = Path.Combine(gameFolder, splitData.SourceFile);
					break;
			}
			string inifilename = Path.Combine(iniFolder, (splitData.IniFile.ToLower() + ".ini"));
			string projectFolderName = (outputFolder + "\\");

			string splitItem;

			if (splitData.CmnName != null)
				splitItem = splitData.CmnName;
			else
				splitItem = splitData.IniFile;

			progress.StepProgress();
			progress.SetStep("Splitting " + splitItem + " from " + splitData.SourceFile);

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
			string ext = fileInfo.Extension;

			switch (ext.ToLower())
			{
				case ".dll":
					SA_Tools.SplitDLL.SplitDLL.SplitDLLFile(datafilename, inifilename, projectFolderName);
					break;
				case ".nb":
					SA_Tools.Split.SplitNB.SplitNBFile(datafilename, false, projectFolderName, 0, inifilename);
					break;
				default:
					SA_Tools.Split.Split.SplitFile(datafilename, inifilename, projectFolderName);
					break;
			}
		}

		private void splitMdlFiles(Templates.SplitEntryMDL splitMDL, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string outputFolder)
		{
			string filePath = Path.Combine(gameFolder, splitMDL.ModelFile);
			string fileOutputFolder = Path.Combine(outputFolder, "Characters");

			progress.StepProgress();
			progress.SetStep("Splitting models from " + splitMDL.ModelFile);

			#region Validating Inputs
			if (!File.Exists(filePath))
			{
				MessageBox.Show((filePath + " is missing.\n\nPress OK to abort."), "Split Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				throw new Exception(SplitERRORVALUE.NoSourceFile.ToString());
				//return (int)ERRORVALUE.NoSourceFile;
			}
			#endregion

			sa2MDL.Split(splitMDL.BigEndian, filePath,
				fileOutputFolder, splitMDL.MotionFiles.ToArray());
		}

		void splitGame(string game, SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress)
		{
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);
			string iniFolder;

			progress.SetMaxSteps(setProgressMaxStep());

			if (Directory.Exists(Path.Combine(appPath, dataFolder)))
				iniFolder = Path.Combine(appPath, dataFolder);
			else
				iniFolder = Path.Combine(appPath, "Configuration", dataFolder);

			progress.SetTask("Splitting Game Content");
			foreach (Templates.SplitEntry splitEntry in splitEntries)
			{
				splitFiles(splitEntry, progress, gamePath, iniFolder, projFolder);
			}

			if (game == "SADXPC")
			{
				progress.SetTask("Finalizing Moddable Project Setup");
				makeProjectFolders(projFolder, progress, gameName);
				progress.StepProgress();
				progress.SetStep("Copying Object Definitions");
				string objdefsPath = GetObjDefsDirectory();
				string outputObjdefsPath = Path.Combine(projFolder, "objdefs");
				if (Directory.Exists(objdefsPath))
				{
					CopyFolder(objdefsPath, outputObjdefsPath);
					File.Copy(Path.Combine(iniFolder, "sadxlvl.ini"), Path.Combine(projFolder, "sadxlvl.ini"));
					File.Copy(Path.Combine(iniFolder, "objdefs.ini"), Path.Combine(projFolder, "objdefs.ini"));
				}
				else
				{
					MessageBox.Show(("Path to objdefs is missing\n\nPress OK to abort."), "Split Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);

					throw new Exception(SplitERRORVALUE.UnhandledException.ToString());
				}
				GenerateModFile(gameName, progress, projFolder, Path.GetFileNameWithoutExtension(projName));
			}
				
			if (game == "SA2PC")
			{
				if (splitMdlEntries.Count > 0)
				{
					progress.SetTask("Splitting Character Models");
					foreach (Templates.SplitEntryMDL splitMDL in splitMdlEntries)
					{
						splitMdlFiles(splitMDL, progress, gamePath, projFolder);
					}
				}
				progress.SetTask("Finalizing Moddable Project Setup");
				makeProjectFolders(projFolder, progress, gameName);
				GenerateModFile(gameName, progress, projFolder, Path.GetFileNameWithoutExtension(projName));
			}
				
		}
		#endregion

		// TODO: newProj - Swap to using Async instead of Background Worker?
		#region Background Worker
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			using (splitProgress = new SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog("Creating project"))
			{
				Invoke((Action)splitProgress.Show);

				splitGame(gameName, splitProgress);

				Invoke((Action)splitProgress.Close);
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
					SAToolsHub.newProjFile = Path.Combine(projFolder, projName);
					this.Close();
				}
			}
		}
		#endregion

		#region Form Functions
		private void newProj_Shown(object sender, EventArgs e)
		{
			comboBox1.Items.Clear();
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);
			if (Directory.Exists(Path.Combine(appPath, "Configuration")))
				templatesPath = Path.Combine(appPath, "Configuration/Templates/");
			else
				templatesPath = Path.Combine(appPath, "Templates");

			Dictionary<string, string> templateList = loadTemplateList(templatesPath);

			foreach(KeyValuePair<string, string> entry in templateList)
			{
				comboBox1.Items.Add(entry);
			}
			comboBox1.DisplayMember = "Key";

			btnCreate.Enabled = false;
		}
		
		private void btnAltFolderBrowse_Click(object sender, EventArgs e)
		{
			var fsd = new FolderSelect.FolderSelectDialog();
			fsd.Title = "Please select the path for split data to be stored at";
			if (fsd.ShowDialog(IntPtr.Zero))
			{
				txtProjFolder.Text = fsd.FileName;
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

		private void btnCreate_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Project File (*.sap)|*.sap";
			saveFileDialog1.RestoreDirectory = true;

			if (checkBox1.Checked && (txtProjFolder.Text != null))
			{
				saveFileDialog1.InitialDirectory = txtProjFolder.Text;
			}

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((projFileStream = saveFileDialog1.OpenFile()) != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Templates.ProjectTemplate));
					TextWriter writer = new StreamWriter(projFileStream);
					if (checkBox1.Checked && (txtProjFolder.Text != null))
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

					projName = saveFileDialog1.FileName;
					projectFile = new Templates.ProjectTemplate();
					Templates.ProjectInfo projInfo = new Templates.ProjectInfo();

					projInfo.GameName = gameName;
					if (gameName == "SADXPC" || gameName == "SA2PC")
						projInfo.CanBuild = true;
					else
						projInfo.CanBuild = false;
					projInfo.GameSystemFolder = gamePath;
					projInfo.ModSystemFolder = projFolder;

					projectFile.GameInfo = projInfo;
					projectFile.SplitEntries = splitEntries;
					if (gameName == "SA2" || gameName == "SA2GC" || gameName == "SA2PC")
						projectFile.SplitMDLEntries = splitMdlEntries;

					serializer.Serialize(writer, projectFile);
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

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			string templateFile = "";
			if (comboBox1.SelectedIndex > -1)
			{
				templateFile = Path.Combine(templatesPath, ((KeyValuePair<string, string>)comboBox1.SelectedItem).Value.ToString());
			}

			if (templateFile.Length > 0)
				openTemplate(templateFile);

			if (gameName != null && gamePath != null)
				btnCreate.Enabled = true;
		}
		#endregion
	}
}