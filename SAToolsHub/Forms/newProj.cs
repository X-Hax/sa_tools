using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;
using SAModel.SAEditorCommon.ModManagement;
using SAEditorCommon.ProjectManagement;
using SplitTools.Split;
using SplitTools.SAArc;

namespace SAToolsHub
{
	public partial class newProj : Form
	{
		Stream projFileStream;
		Templates.ProjectTemplate projectFile;
		SAModel.SAEditorCommon.UI.ProgressDialog splitProgress;

		string templatesPath;
		string gameName;
		string gamePath;
		string projFolder;
		string dataFolder;
        string gameDataFolder;
        string checkFile;
        string projName;
        int selectedTemplateIndex = -1;
		int splitCheck;
		List<Templates.SplitEntry> splitEntries = new List<Templates.SplitEntry>();
		List<Templates.SplitEntryMDL> splitMdlEntries = new List<Templates.SplitEntryMDL>();

		public newProj()
		{
			InitializeComponent();
			backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
			backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
		}

		#region Form Functions
		private void newProj_Shown(object sender, EventArgs e)
		{
			comboBox1.Items.Clear();
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);

            if (Directory.Exists(Path.Combine(appPath, "GameConfig")))
                templatesPath = Path.Combine(appPath, "GameConfig");
            else
                templatesPath = Path.Combine(appPath, "..\\GameConfig");
            Dictionary<string, string> templateList = loadTemplateList(templatesPath);

			foreach (KeyValuePair<string, string> entry in templateList)
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
					projInfo.CheckFile = checkFile;
					projInfo.GameFolder = gamePath;
					projInfo.GameDataFolder = gameDataFolder;
					projInfo.ProjectFolder = projFolder;
					projInfo.CanBuild = (gameName == "SADXPC" || gameName == "SA2PC");

					projectFile.GameInfo = projInfo;
					projectFile.SplitEntries = splitEntries;
                    if (splitMdlEntries != null)
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
            // This is needed to prevent opening the template file twice.
			// SelectedIndexChanged fires after a dialog closes even if the selection didn't change.
            if (selectedTemplateIndex == comboBox1.SelectedIndex)
                return;

            selectedTemplateIndex = comboBox1.SelectedIndex;
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

		// TODO: newProj - Migrate some Additional Functions out.
		#region Additional Functions
		private int setProgressMaxStep()
		{
            int result = splitEntries.Count;
            if (splitMdlEntries != null)
                result += splitMdlEntries.Count;
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
                if (file.Contains("DX") && file.Contains("PC"))
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
				gamePath = ProjectFunctions.GetGamePath(template.GameInfo.GameName);
                // This should never happen under normal circumstances
                if (gamePath == "")
                    throw new Exception("Game path not set");
                dataFolder = template.GameInfo.DataFolder;
                gameDataFolder = template.GameInfo.GameDataFolderName;
                checkFile = template.GameInfo.CheckFile;
				splitEntries = template.SplitEntries;
				splitMdlEntries = template.SplitMDLEntries;
			}
			else
				comboBox1.SelectedIndex = -1;
			
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
						Name = name
					};
					outputPath = Path.Combine(projectFolder, string.Format("mod.ini"));

					SplitTools.IniSerializer.Serialize(modInfoSADX, outputPath);
					break;

				case ("SA2PC"):
					SA2ModInfo modInfoSA2PC = new SA2ModInfo
					{
						Name = name
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

			if (Directory.Exists(Path.Combine(appPath, "GameConfig", dataFolder, "objdefs")))
				return Path.Combine(appPath, "GameConfig", dataFolder, "objdefs");
			else
				return Path.Combine(appPath, "..\\SA1Tools\\SADXObjectDefinitions");
		}

		int splitGame(string game, SAModel.SAEditorCommon.UI.ProgressDialog progress)
		{
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);
			string iniFolder;

			progress.SetMaxSteps(setProgressMaxStep());

			if (Directory.Exists(Path.Combine(appPath, "GameConfig", dataFolder)))
				iniFolder = Path.Combine(appPath, "GameConfig", dataFolder);
			else
				iniFolder = Path.Combine(appPath, "..\\GameConfig", dataFolder);

			progress.SetTask("Splitting Game Content");
			foreach (Templates.SplitEntry splitEntry in splitEntries)
			{
				ProjectFunctions.SplitTemplateEntry(splitEntry, progress, gamePath, iniFolder, projFolder);
				if (File.Exists(Path.Combine(projFolder, "SplitLog.log")))
					return 0;
			}
			// SALVL stuff
			if (File.Exists(Path.Combine(iniFolder, "sadxlvl.ini")))
            {
                progress.SetStep("Copying Object Definitions");
                string objdefsPath = GetObjDefsDirectory();
                string outputObjdefsPath = Path.Combine(projFolder, "objdefs");
                if (Directory.Exists(objdefsPath))
                    CopyFolder(objdefsPath, outputObjdefsPath);
                progress.SetTask("Finalizing SALVL Supported Setup");
                File.Copy(Path.Combine(iniFolder, "sadxlvl.ini"), Path.Combine(projFolder, "sadxlvl.ini"), true);
                File.Copy(Path.Combine(iniFolder, "objdefs.ini"), Path.Combine(projFolder, "objdefs.ini"), true);
            }
            // Split MDL files for SA2
            if (splitMdlEntries.Count > 0)
            {
                progress.SetTask("Splitting Character Models");
                foreach (Templates.SplitEntryMDL splitMDL in splitMdlEntries)
                    ProjectFunctions.SplitTemplateMDLEntry(splitMDL, progress, gamePath, projFolder);
            }
            // Project folders for buildable PC games
			if (game == "SADXPC" || game == "SA2PC")
			{
				progress.SetTask("Finalizing Project Setup");
				makeProjectFolders(projFolder, progress, game);
				GenerateModFile(game, progress, projFolder, Path.GetFileNameWithoutExtension(projName));
                progress.StepProgress();
            }

			return 1;
		}
		#endregion

		// TODO: newProj - Swap to using Async instead of Background Worker?
		#region Background Worker
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			using (splitProgress = new SAModel.SAEditorCommon.UI.ProgressDialog("Creating project"))
			{
				Invoke((Action)splitProgress.Show);

				splitCheck = splitGame(gameName, splitProgress);

				Invoke((Action)splitProgress.Close);
			}
		}

		private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e != null && e.Error != null)
			{
				MessageBox.Show("Project failed to split: " + e.Error.Message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				if (splitCheck == 0)
					MessageBox.Show("Item failed to split properly. Please check the SplitLog.log file at:\n\n" + projFolder, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
		}
		#endregion
	}
}