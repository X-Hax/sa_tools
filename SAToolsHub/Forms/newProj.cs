using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;
using SAModel.SAEditorCommon.ModManagement;
using SAModel.SAEditorCommon.ProjectManagement;
using System.Xml;
using SplitTools.Split;
using SplitTools;

namespace SAToolsHub
{
	public partial class newProj : Form
	{
		// Variables
		string templatesPath;
		string gameName;
		string projectType;
		string gamePath;
		string projFolder;
		string dataFolder;
		string gameDataFolder;
		string checkFile;
		string projName;
		bool isNJA;

		// Variables (split)
		Stream projFileStream;
		Templates.ProjectTemplate projectFile;
		List<Templates.SplitEntry> splitEntries = new List<Templates.SplitEntry>();
		List<Templates.SplitEntryMDL> splitMdlEntries = new List<Templates.SplitEntryMDL>();
		List<Templates.SplitEntryEvent> splitEventEntries = new List<Templates.SplitEntryEvent>();
		ProjectSplitResult splitCheck;

		// UI
		Dictionary<string, string> gamePlatforms = new Dictionary<string, string>();
		SAModel.SAEditorCommon.UI.ProgressDialog splitProgress;
		int selectedTemplateIndex = -1;

		public newProj()
		{
			InitializeComponent();
			backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
			backgroundWorker1.WorkerSupportsCancellation = true;
			backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
		}

		#region Form Functions
		private void newProj_Shown(object sender, EventArgs e)
		{
			gamePlatforms.Clear();
			gamePlatforms.Add("PC", "PC");
			gamePlatforms.Add("Dreamcast", "DC");
			gamePlatforms.Add("Gamecube", "GC");
			gamePlatforms.Add("Xbox 360", "X360");
			//gamePlatforms.Add("Playstation 3", "PS3");
			buttonCreateProject.Enabled = false;
			comboBoxPlatform.Items.Clear();
			comboBoxTemplate.Items.Clear();
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);

			if (Directory.Exists(Path.Combine(appPath, "GameConfig")))
				templatesPath = Path.Combine(appPath, "GameConfig");
			else
				templatesPath = Path.Combine(appPath, "..\\GameConfig");
			foreach (var item in gamePlatforms)
				comboBoxPlatform.Items.Add(item.Key);
			comboBoxPlatform.SelectedIndex = 0;
			comboBoxLabels.SelectedIndex = 0;
		}

		private void newProj_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (backgroundWorker1.IsBusy)
				backgroundWorker1.CancelAsync();
		}

		private void btnAltFolderBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fsd = new FolderBrowserDialog { Description = "Please select the path for split data to be stored at", UseDescriptionForTitle = true };
			if (fsd.ShowDialog() == DialogResult.OK)
			{
				textBoxProjFolder.Text = fsd.SelectedPath;
				// If a game template is selected, enable the Create button
				if (comboBoxTemplate.SelectedIndex != -1)
					buttonCreateProject.Enabled = true;
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxSaveDifferentFolder.Checked)
			{
				textBoxProjFolder.Enabled = true;
				buttonBrowse.Enabled = true;
			}
			else
			{
				textBoxProjFolder.Enabled = false;
				buttonBrowse.Enabled = false;
			}
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			if (backgroundWorker1.IsBusy)
				return;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Project File (*.sap)|*.sap";
			saveFileDialog1.RestoreDirectory = true;

			if (checkBoxSaveDifferentFolder.Checked && textBoxProjFolder.Text != "")
			{
				saveFileDialog1.InitialDirectory = textBoxProjFolder.Text;
			}

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((projFileStream = saveFileDialog1.OpenFile()) != null)
				{
					XmlSerializer serializer = new(typeof(Templates.ProjectTemplate));
					XmlWriter xmlWriter = XmlWriter.Create(projFileStream, new XmlWriterSettings() { Indent = true });
					if (checkBoxSaveDifferentFolder.Checked && (textBoxProjFolder.Text != null))
					{
						projFolder = textBoxProjFolder.Text;
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
					projInfo.ProjectType = projectType;
					projInfo.CheckFile = checkFile;
					projInfo.GameDataFolder = gameDataFolder;
					projInfo.ProjectFolder = (checkBoxSaveDifferentFolder.Checked && textBoxProjFolder.Text != "") ? projFolder : Path.GetFileNameWithoutExtension(saveFileDialog1.FileName);
					projInfo.CanBuild = (gameName == "SADXPC" || gameName == "SA2PC");

					projectFile.GameInfo = projInfo;
					projectFile.SplitEntries = splitEntries;
					if (splitMdlEntries != null)
						projectFile.SplitMDLEntries = splitMdlEntries;
					if (splitEventEntries != null)
						projectFile.SplitEventEntries = splitEventEntries;

					serializer.Serialize(xmlWriter, projectFile);
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
			// This is needed to prevent opening the template file twice.
			// SelectedIndexChanged fires after a dialog closes even if the selection didn't change.
			if (selectedTemplateIndex == comboBoxTemplate.SelectedIndex)
				return;
			selectedTemplateIndex = comboBoxTemplate.SelectedIndex;
			string templateFile = "";
			if (comboBoxTemplate.SelectedIndex > -1)
			{
				templateFile = Path.Combine(templatesPath, ((KeyValuePair<string, string>)comboBoxTemplate.SelectedItem).Value.ToString());
			}

			if (templateFile.Length > 0)
				openTemplate(templateFile);

			if (gameName != null && gamePath != null)
				buttonCreateProject.Enabled = true;
		}

		private void comboBoxPlatform_SelectedIndexChanged(object sender, EventArgs e)
		{
			FilterByPlatform(gamePlatforms[comboBoxPlatform.SelectedItem.ToString()]);
		}

		private void FilterByPlatform(string platform)
		{
			comboBoxTemplate.Items.Clear();
			Dictionary<string, string> templateList = loadTemplateList(templatesPath);
			foreach (KeyValuePair<string, string> entry in templateList)
			{
				if (entry.Key.Contains(platform))
				{
					if (radioButtonSA2.Checked && entry.Key.Contains("SA2"))
						comboBoxTemplate.Items.Add(entry);
					else if (radioButtonSA1.Checked && !entry.Key.Contains("SA2"))
						comboBoxTemplate.Items.Add(entry);
				}
			}
			buttonCreateProject.Enabled = false;
			comboBoxTemplate.DisplayMember = "Key";
			comboBoxTemplate.SelectedIndex = -1;
			selectedTemplateIndex = -1;
		}

		private void radioButtonSA2_CheckedChanged(object sender, EventArgs e)
		{
			FilterByPlatform(gamePlatforms[comboBoxPlatform.SelectedItem.ToString()]);
		}

		private void newProj_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/wiki/SA-Tools-Hub#creating-a-project") { CreateNoWindow = true });
		}

		private void checkBoxAdvanced_CheckedChanged(object sender, EventArgs e)
		{
			groupBoxAdvancedOptions.Enabled = checkBoxAdvanced.Checked;
		}
		#endregion

		// TODO: newProj - Migrate some Additional Functions out.
		#region Additional Functions
		private int setProgressMaxStep()
		{
			int result = splitEntries.Count;
			if (splitMdlEntries != null)
				result += splitMdlEntries.Count;
			if (splitEventEntries != null)
				result += splitEventEntries.Count;
			switch (gameName)
			{
				case "SADXPC":
					result += 4;
					break;
				case "SA2PC":
					result += 3;
					break;
				default:
					break;
			}
			return result;
		}

		string[] SortTemplateList(string[] originalList)
		{
			var ordered = originalList.OrderBy(str => Path.GetFileNameWithoutExtension(str));
			List<string> result = new List<string>();
			// Put SADXPC first and SA2PC second
			foreach (string file in ordered)
			{
				if (file.Contains("SADX") && file.Contains("PC"))
					result.Insert(0, file);
				else if (file.Contains("SA2") && file.Contains("PC"))
					result.Add(file);
			}
			// Add other items
			foreach (string file in ordered)
			{
				if (!result.Contains(file))
					result.Add(file);
			}
			return result.ToArray();
		}

		Dictionary<string, string> loadTemplateList(string folder)
		{
			Dictionary<string, string> templates = new Dictionary<string, string>();
			string[] templateNames = SortTemplateList(Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly));

			for (int i = 0; i < templateNames.Length; i++)
			{
				templates.Add(Path.GetFileNameWithoutExtension(templateNames[i]), templateNames[i]);
			}

			return templates;
		}

		void openTemplate(string templateSplit)
		{
			Templates.SplitTemplate template = ProjectFunctions.openTemplateFile(templateSplit);

			if (template != null)
			{
				gameName = template.GameInfo.GameName;
				projectType = template.GameInfo.ProjectType;
				gamePath = ProjectFunctions.GetGamePath(template.GameInfo.GameName);
				isNJA = template.GameInfo.NJA;
				// This should never happen under normal circumstances
				if (gamePath == "")
					throw new Exception("Game path not set");
				dataFolder = template.GameInfo.DataFolder;
				gameDataFolder = template.GameInfo.GameDataFolderName;
				checkFile = template.GameInfo.CheckFile;
				splitEntries = template.SplitEntries;
				splitMdlEntries = template.SplitMDLEntries;
				splitEventEntries = template.SplitEventEntries;
			}
			else
				comboBoxTemplate.SelectedIndex = -1;

		}

		private void makeProjectFolders(string projFolder, SAModel.SAEditorCommon.UI.ProgressDialog progress, string game)
		{
			progress.StepProgress();
			progress.SetStep("Making Additional Mod Folders");
			string[] readMeSADX = {
				"Welcome to your new SADX Mod! The info below will assist with some additional folders created for your Mod.\n\n" +
				"Code - You can store your source Code here.\n" +
				"system - SADX's system folder. Store your textures (PVM) here. Stage object layouts are stored here as well.\n" +
				"textures - You can store PVMX and Texture Pack Archives here.\n\n" +
				"Please refer to the Help drop down in the SA Tools Hub for additional resources or you can reach members for help in the X-Hax Discord."
			};

			string[] readMeSA2PC = {
				"Welcome to your new SA2PC Mod! The info below will assist with some additional folders created for your Mod.\n\n" +
				"Code - You can store your source Code here.\n" +
				"gd_PC - SA2's system folder. Store your textures (GVM/PAK) here. Stage object layouts are stored here as well.\n\n" +
				"Please refer to the Help drop down in the SA Tools Hub for additional resources or you can reach members for help in the X-Hax Discord."
			};

			string systemPath;
			// Source Folder
			string sourceFolderPath = Path.Combine(projFolder, "Code");
			if (!Directory.Exists(sourceFolderPath))
				Directory.CreateDirectory(sourceFolderPath);

			// Game System Folder
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
					string prsysPath = Path.Combine(projFolder, "gd_PC", "PRS");
					if (!Directory.Exists(prsysPath))
						Directory.CreateDirectory(prsysPath);
					File.WriteAllLines(projReadMePath, readMeSA2PC);
					break;
				default:
					break;
			}
		}

		private void GenerateModFile(string game, SAModel.SAEditorCommon.UI.ProgressDialog progress, string projectFolder, string name)
		{
			progress.StepProgress();
			progress.SetStep("Generating mod.ini");
			string outputPath;
			switch (game)
			{
				case ("SADXPC"):
					SADXModInfo modInfoSADX = new SADXModInfo
					{
						Name = name,
						ModID = "sadx." + name
					};
					outputPath = Path.Combine(projectFolder, string.Format("mod.ini"));

					SplitTools.IniSerializer.Serialize(modInfoSADX, outputPath);
					break;

				case ("SA2PC"):
					SA2ModInfo modInfoSA2PC = new SA2ModInfo
					{
						Name = name,
						ModID = "sa2." + name
			};
					outputPath = Path.Combine(projectFolder, string.Format("mod.ini"));

					SplitTools.IniSerializer.Serialize(modInfoSA2PC, outputPath);
					break;

				default:
					break;
			}

		}

		private void CopyFolder(string sourceFolder, string destinationFolder)
		{
			if (!Directory.Exists(sourceFolder))
				return;

			string[] files = Directory.GetFiles(sourceFolder);

			Directory.CreateDirectory(destinationFolder);

			foreach (string objdef in files)
			{
				System.IO.FileInfo objdefFileInfo = new System.IO.FileInfo(objdef);
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

		private string GetObjDefsDirectory(bool SA2 = false)
		{
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);

			if (Directory.Exists(Path.Combine(appPath, "GameConfig", dataFolder, "objdefs")))
				return Path.Combine(appPath, "GameConfig", dataFolder, "objdefs");
			else if (SA2)
				return Path.Combine(appPath, "..\\SA2Tools\\SA2ObjectDefinitions");
			else
				return Path.Combine(appPath, "..\\SA1Tools\\SADXObjectDefinitions");
		}

		private static void RemoveEmptyFolders(string startLocation)
		{
			foreach (var directory in Directory.GetDirectories(startLocation))
			{
				RemoveEmptyFolders(directory);
				if (Directory.GetFiles(directory).Length == 0 &&
					Directory.GetDirectories(directory).Length == 0)
				{
					Directory.Delete(directory, false);
				}
			}
		}

		ProjectSplitResult splitGame(string game, SAModel.SAEditorCommon.UI.ProgressDialog progress, DoWorkEventArgs e)
		{
			SplitFlags splitFlags = SplitFlags.Log | SplitFlags.Overwrite;
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);
			string iniFolder;
			if (isNJA)
				splitFlags |= SplitFlags.NJA;
			if (comboBoxLabels.SelectedIndex == 2 && !isNJA)
				splitFlags |= SplitFlags.NoMeta;
			if (comboBoxLabels.SelectedIndex != 1 && !isNJA)
				splitFlags |= SplitFlags.NoLabels;
			progress.SetMaxSteps(setProgressMaxStep());

			if (Directory.Exists(Path.Combine(appPath, "GameConfig", dataFolder)))
				iniFolder = Path.Combine(appPath, "GameConfig", dataFolder);
			else
				iniFolder = Path.Combine(appPath, "..\\GameConfig", dataFolder);

			progress.SetTask("Splitting Game Content");
			// Delete log if it exists
			if (File.Exists(Path.Combine(projFolder, "SplitLog.log")))
				File.Delete(Path.Combine(projFolder, "SplitLog.log"));
			foreach (Templates.SplitEntry splitEntry in splitEntries)
			{
				if (backgroundWorker1.CancellationPending == true)
				{
					e.Cancel = true;
					return ProjectSplitResult.Cancelled;
				}
				ProjectFunctions.SplitTemplateEntry(splitEntry, progress, gamePath, iniFolder, projFolder, splitFlags);
				if (File.Exists(Path.Combine(projFolder, "SplitLog.log")))
					return ProjectSplitResult.ItemFailure;
			}
			// SALVL stuff
			if (File.Exists(Path.Combine(iniFolder, "sadxlvl.ini")))
			{
				progress.SetStep("Copying Object Definitions");
				string outputObjdefsPath = Path.Combine(projFolder, "objdefs");
				string objdefsinifolder = Path.Combine(appPath, "GameConfig", dataFolder, "objdefs");
				// objdefs INI folder
				if (!Directory.Exists(objdefsinifolder))
					objdefsinifolder = Path.Combine(appPath, "..\\GameConfig", dataFolder, "objdefs");
				CopyFolder(objdefsinifolder, outputObjdefsPath);
				// objdefs CS folder
				string objdefsPath = GetObjDefsDirectory();
				if (Directory.Exists(objdefsPath))
					CopyFolder(objdefsPath, outputObjdefsPath);
				progress.SetTask("Finalizing SALVL Supported Setup");
				File.Copy(Path.Combine(iniFolder, "sadxlvl.ini"), Path.Combine(projFolder, "sadxlvl.ini"), true);
				File.Copy(Path.Combine(iniFolder, "objdefs.ini"), Path.Combine(projFolder, "objdefs.ini"), true);
			}
			if (File.Exists(Path.Combine(iniFolder, "sa2lvl.ini")))
			{
				progress.SetStep("Copying Object Definitions");
				string objdefsPath = GetObjDefsDirectory(true);
				string outputObjdefsPath = Path.Combine(projFolder, "objdefs");
				if (Directory.Exists(objdefsPath))
					CopyFolder(objdefsPath, outputObjdefsPath);
				progress.SetTask("Finalizing SALVL Supported Setup");
				File.Copy(Path.Combine(iniFolder, "sa2lvl.ini"), Path.Combine(projFolder, "sa2lvl.ini"), true);
				File.Copy(Path.Combine(iniFolder, "objdefs.ini"), Path.Combine(projFolder, "objdefs.ini"), true);
			}
			// Split MDL files for SA2
			if (splitMdlEntries.Count > 0)
			{
				progress.SetTask("Splitting Character Models");
				foreach (Templates.SplitEntryMDL splitMDL in splitMdlEntries)
				{
					if (backgroundWorker1.CancellationPending == true)
					{
						e.Cancel = true;
						return ProjectSplitResult.Cancelled;
					}
					ProjectFunctions.SplitTemplateMDLEntry(splitMDL, progress, gamePath, projFolder, iniFolder);
				}
			}
			// Split Event files for SA2
			if (splitEventEntries.Count > 0)
			{
				progress.SetTask("Splitting Event Data");
				foreach (Templates.SplitEntryEvent splitEvent in splitEventEntries)
				{
					if (backgroundWorker1.CancellationPending == true)
					{
						e.Cancel = true;
						return ProjectSplitResult.Cancelled;
					}
					ProjectFunctions.SplitTemplateEventEntry(splitEvent, progress, gamePath, projFolder, iniFolder);
				}
			}
			// Process and clean up non-NJA stuff
			if (isNJA)
			{
				string duplistpath = Path.Combine(iniFolder, "duppath.txt");
				Dictionary<string, string> duplist = new Dictionary<string, string>();
				if (File.Exists(duplistpath))
					duplist = IniSerializer.Deserialize<Dictionary<string, string>>(duplistpath);
				progress.SetTask("Processing and cleaning decomp files");
				progress.SetStep("");
				string[] files = Directory.GetFiles(projFolder, "*.*", SearchOption.AllDirectories);
				foreach (string file in files)
				{
					switch (Path.GetExtension(file).ToLowerInvariant())
					{
						case ".sa1lvl":
							// .c file export disabled for now
							//StructConversion.ConvertFileToText(file, StructConversion.TextType.NJA, file[..file.LastIndexOf(".")], false, true);
							// Generate dupmodel and dupmotion files
							if (duplist.ContainsKey(Path.GetFileName(file)))
								SAModel.SAEditorCommon.ProjectManagement.NJAExporter.GenerateDup(file, Path.Combine(Path.GetDirectoryName(file), duplist[Path.GetFileName(file)]));
							File.Delete(file);
							break;
						case ".sa1mdl":
						case ".sa2mdl":
						case ".sa2bmdl":
						case ".saanim":
						case ".satex":
							StructConversion.ConvertFileToText(file, StructConversion.TextType.NJA, file[..file.LastIndexOf(".")], false, true);
							File.Delete(file);
							break;
						case ".nja":
						case ".nam":
						case ".nad":
						case ".nas":
						case ".dup":
						case ".dum":
						case ".tls":
						case ".cut":
						case ".sap":
							break;
						default:
							File.Delete(file);
							break;
					}
				}
				RemoveEmptyFolders(projFolder);
				return ProjectSplitResult.Success;
			}
			// Project folders for buildable PC games
			if (game == "SADXPC" || game == "SA2PC")
			{
				progress.SetTask("Finalizing Project Setup");
				makeProjectFolders(projFolder, progress, game);
				GenerateModFile(game, progress, projFolder, Path.GetFileNameWithoutExtension(projName));
				progress.StepProgress();
			}
			return ProjectSplitResult.Success;
		}
		#endregion

		// TODO: newProj - Swap to using Async instead of Background Worker?
		#region Background Worker
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			using (splitProgress = new SAModel.SAEditorCommon.UI.ProgressDialog("Creating project"))
			{
				Invoke((Action)splitProgress.Show);

				splitCheck = splitGame(gameName, splitProgress, e);
			}
		}

		private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			switch (splitCheck)
			{
				case ProjectSplitResult.Cancelled:
					MessageBox.Show(this, "Project split has been cancelled.", "SA Tools Hub", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					break;
				case ProjectSplitResult.ProjectFailure:
					MessageBox.Show(this, "Project failed to split: " + e.Error.Message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;
				case ProjectSplitResult.ItemFailure:
					if (MessageBox.Show(this, "Item failed to split properly: " + e.Error.Message + ". The log file is located at:\n\n" + projFolder + ".\n\nWould you like to open it?", "Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
						System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("notepad", $"\"" + Path.Combine(projFolder, "SplitLog.log") + "\"") { CreateNoWindow = false });
					break;
				case ProjectSplitResult.Success:
					MessageBox.Show(this, "Project successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
					SAToolsHub.newProjFile = Path.Combine(projFolder, projName);
					Close();
					break;
			}
		}

		private enum ProjectSplitResult
		{
			ItemFailure,
			Success,
			ProjectFailure,
			Cancelled
		}
		#endregion
	}
}