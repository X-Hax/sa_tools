using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using SAModel.SAEditorCommon.ModManagement;
using SAEditorCommon.ProjectManagement;
using SplitTools.SAArc;

namespace SAToolsHub
{
	public partial class buildWindow : Form
	{
		SAModel.SAEditorCommon.ManualBuildWindow manualBuildWindow =
			new SAModel.SAEditorCommon.ManualBuildWindow();

		Dictionary<string, SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType> assemblies =
				new Dictionary<string, SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType>();

		string gameEXE;
		string modName;
		string modFolder;
		string sysFolder;
		List<string> iniEXEFiles = new List<string>();
		List<string> iniDLLFiles = new List<string>();
		List<string> sa2MdlMtnFiles = new List<string>();

		public buildWindow()
		{
			InitializeComponent();
		}

		// TODO: buildWindow - Migrate some Additional Functions out.
		// TODO: buildWindow - Swap to using Async instead of Background Worker?
		#region Additional Functions
		void setAssemblies()
		{
			assemblies.Clear();

			List<Templates.SplitEntry> splitEntries = new List<Templates.SplitEntry>();

			foreach (Templates.SplitEntry exeEntry in chkBoxEXE.CheckedItems)
			{
				splitEntries.Add(exeEntry);
			}
			foreach (Templates.SplitEntry dllEntry in chkBoxDLL.CheckedItems)
			{
				splitEntries.Add(dllEntry);
			}
			if (SAToolsHub.setGame == "SA2PC")
			{
				foreach (string chrFile in chkBoxMDL.CheckedItems)
				{
                    sa2MdlMtnFiles.Add(Path.Combine(Path.Combine(SAToolsHub.projectDirectory, "figure", "bin"), chrFile));
                }
			}

			foreach (Templates.SplitEntry splitEntry in splitEntries)
			{
				switch (SAToolsHub.setGame)
				{
					case ("SADXPC"):
                        if (splitEntry.SourceFile.Contains("exe"))
                        {
                            assemblies.Add(splitEntry.IniFile, SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);
                        }
                        else
                        {
                            assemblies.Add(splitEntry.IniFile, SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
                        }
						break;

					case ("SA2PC"):
						if (splitEntry.SourceFile.Contains("exe"))
						{
							assemblies.Add(splitEntry.IniFile, SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);
						}
						else
						{
							if (splitEntry.IniFile == "data_dll")
							{
								assemblies.Add((splitEntry.IniFile + "_orig"), SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
							}
							else
							{
								assemblies.Add(splitEntry.IniFile, SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
							}
						}
						break;
				}
			}
		}

		void genAssemblies()
		{
			SplitTools.IniData EXEiniData = new SplitTools.IniData();
			SplitTools.SplitDLL.DllIniData DLLiniData = new SplitTools.SplitDLL.DllIniData();
			Dictionary<string, bool> itemsEXEToExport = new Dictionary<string, bool>();
			Dictionary<string, bool> itemsDLLToExport = new Dictionary<string, bool>();

			foreach (KeyValuePair<string, SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType> assembly in assemblies)
			{
				string iniPath = Path.Combine(SAToolsHub.projectDirectory.ToString(), assembly.Key + "_data.ini");

				Dictionary<string, bool> itemsToExport = new Dictionary<string, bool>();

				switch (assembly.Value)
				{
					case SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe:
						iniEXEFiles.Add(iniPath);
						break;

					case SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL:
						if (SAToolsHub.setGame == "SA2PC")
						{
							iniDLLFiles.Add(iniPath);
						}
						else
						{
							SplitTools.SplitDLL.DllIniData dllIniData =
								SAModel.SAEditorCommon.DLLModGenerator.DLLModGen.LoadINI(iniPath, ref itemsToExport);

							SAModel.SAEditorCommon.DLLModGenerator.DLLModGen.ExportINI(dllIniData,
								itemsToExport, Path.Combine(modFolder, assembly.Key + "_data.ini"));
						}
						break;
					default:
						break;
				}
			}

			if (iniEXEFiles.Count > 0)
			{
				EXEiniData = SAModel.SAEditorCommon.StructConverter.StructConverter.LoadMultiINI(iniEXEFiles, ref itemsEXEToExport, false);

				SAModel.SAEditorCommon.StructConverter.StructConverter.ExportINI(EXEiniData,
					itemsEXEToExport, Path.Combine(modFolder, gameEXE + "_data.ini"));
			}

			if (iniDLLFiles.Count > 0)
			{
				DLLiniData = SAModel.SAEditorCommon.DLLModGenerator.DLLModGen.LoadMultiINI(iniDLLFiles, ref itemsDLLToExport);

				SAModel.SAEditorCommon.DLLModGenerator.DLLModGen.ExportINI(DLLiniData,
					itemsDLLToExport, Path.Combine(modFolder, "Data_DLL_orig_data.ini"));
			}

			if (sa2MdlMtnFiles.Count > 0)
			{
				string filePath;

				if (Directory.Exists(Path.Combine(SAToolsHub.projectDirectory, "figure\\bin")))
					filePath = Path.Combine(SAToolsHub.projectDirectory, "figure\\bin");
				else
					filePath = Path.Combine(SAToolsHub.projectDirectory, "resource/gd_PC");

				foreach (string folder in sa2MdlMtnFiles)
				{
					string file = Path.Combine(filePath, folder);
					if (file.Contains("mdl"))
						sa2MDL.Build(true, file);
					if (file.Contains("mtn"))
						sa2MTN.Build(true, file);
				}
			}
		}

		void CopySA2Files(string sysFolder)
		{
			string charPath = Path.Combine(SAToolsHub.projectDirectory, "figure", "bin");
			string[] charFiles = Directory.GetFiles(charPath, "*.prs");
			foreach (string file in charFiles)
			{
				File.Copy(file, Path.Combine(sysFolder, Path.GetFileName(file)));
			}
		}

		void createMod()
		{
			string baseModIniPath = Path.Combine(SAToolsHub.projectDirectory, "mod.ini");
			string outputModIniPath = Path.Combine(modFolder, "mod.ini");
			const string dataSuffix = "_data.ini";

			switch (SAToolsHub.setGame)
			{
				case "SADXPC":
					SADXModInfo sadxModInfo = SplitTools.IniSerializer.Deserialize<SADXModInfo>(baseModIniPath);

					// set all of our assemblies properly
					string ADV00MODELS = "adv00_dll";
					string ADV01CMODELS = "adv0130_dll";
					string ADV01MODELS = "adv0100_dll";
					string ADV02MODELS = "adv02_dll";
					string ADV03MODELS = "adv03_dll";
					string BOSSCHAOS0MODELS = "b_chaos0_dll";
					string CHAOSTGGARDEN02MR_DAYTIME = "chaostggarden02mr_daytime";
					string CHAOSTGGARDEN02MR_EVENING = "chaostggarden02mr_evening";
					string CHAOSTGGARDEN02MR_NIGHT = "chaostggarden02mr_night";

					if (assemblies.ContainsKey(ADV00MODELS)) sadxModInfo.ADV00MODELSData = ADV00MODELS + dataSuffix;
					if (assemblies.ContainsKey(ADV01CMODELS)) sadxModInfo.ADV01CMODELSData = ADV01CMODELS + dataSuffix;
					if (assemblies.ContainsKey(ADV01MODELS)) sadxModInfo.ADV01MODELSData = ADV01MODELS + dataSuffix;
					if (assemblies.ContainsKey(ADV02MODELS)) sadxModInfo.ADV02MODELSData = ADV02MODELS + dataSuffix;
					if (assemblies.ContainsKey(ADV03MODELS)) sadxModInfo.ADV03MODELSData = ADV03MODELS + dataSuffix;
					if (assemblies.ContainsKey(BOSSCHAOS0MODELS)) sadxModInfo.BOSSCHAOS0MODELSData = BOSSCHAOS0MODELS + dataSuffix;
					if (assemblies.ContainsKey(CHAOSTGGARDEN02MR_DAYTIME)) sadxModInfo.CHAOSTGGARDEN02MR_DAYTIMEData = CHAOSTGGARDEN02MR_DAYTIME + dataSuffix;
					if (assemblies.ContainsKey(CHAOSTGGARDEN02MR_EVENING)) sadxModInfo.CHAOSTGGARDEN02MR_EVENINGData = CHAOSTGGARDEN02MR_EVENING + dataSuffix;
					if (assemblies.ContainsKey(CHAOSTGGARDEN02MR_NIGHT)) sadxModInfo.CHAOSTGGARDEN02MR_NIGHTData = CHAOSTGGARDEN02MR_NIGHT + dataSuffix;
					if (iniEXEFiles.Count > 0) sadxModInfo.EXEData = "sonic_data.ini";
					if (assemblies.ContainsKey("chrmodels")) sadxModInfo.CHRMODELSData = "chrmodels_data.ini";

					SplitTools.IniSerializer.Serialize(sadxModInfo, outputModIniPath);
					break;

				case "SA2PC":
					SA2ModInfo sa2ModInfo = SplitTools.IniSerializer.Deserialize<SA2ModInfo>(baseModIniPath);

					if (File.Exists(Path.Combine(SAToolsHub.projectDirectory, "Data_DLL_orig.ini")))
					{
						if (assemblies.ContainsKey("Data_DLL_orig")) sa2ModInfo.DLLData = "Data_DLL_orig.ini";
					}
					else
					{
						if (assemblies.ContainsKey("Data_DLL")) sa2ModInfo.DLLData = "Data_DLL.ini";
					}

					if (iniEXEFiles.Count > 0) sa2ModInfo.EXEData = "sonic2app_data.ini";

					SplitTools.IniSerializer.Serialize(sa2ModInfo, outputModIniPath);
					break;
				default:
					break;
			}
		}

		public void CopySystemFolder(string modFolder)
		{
			string projectSystemPath = Path.Combine(SAToolsHub.projectDirectory, sysFolder);
			string modSystemPath = Path.Combine(modFolder, sysFolder);

			SAModel.SAEditorCommon.StructConverter.StructConverter.CopyDirectory(
				new DirectoryInfo(projectSystemPath), modSystemPath);

			switch (SAToolsHub.setGame)
			{
				case ("SADXPC"):
					string texturesPath = Path.Combine(SAToolsHub.projectDirectory, "textures");
					SAModel.SAEditorCommon.StructConverter.StructConverter.CopyDirectory(new DirectoryInfo(texturesPath), modSystemPath);
					break;

				case ("SA2PC"):
					CopySA2Files(modSystemPath);
					break;
			}
		}
		#endregion

		#region Background Worker
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			using (SAModel.SAEditorCommon.UI.ProgressDialog progress = new SAModel.SAEditorCommon.UI.ProgressDialog("Building Project"))
			{
				Action showProgress = () =>
				{
					Invoke((Action)progress.Show);
				};

				Action<int> setSteps = (int count) =>
				{
					progress.SetMaxSteps(count);
				};

				Action<string> stepProgress = (string update) =>
				{
					progress.StepProgress();
					progress.SetStep(update);
				};

				AutoBuild(showProgress, setSteps, stepProgress);
			}
		}

		private void BackgroundWorker1_RunWorkerCompleteAlert(object sender, RunWorkerCompletedEventArgs e)
		{
			backgroundWorker1.RunWorkerCompleted -= BackgroundWorker1_RunWorkerCompleteAlert;
			MessageBox.Show("Build complete!");
			this.Close();
		}

		private void AutoBuild(Action showProgress, Action<int> maxSteps, Action<string> updateProgress)
		{
			showProgress();

			string modsFolder = SAToolsHub.gameDirectory + "\\mods";
			modFolder = Path.Combine(modsFolder, modName);

			if (!Directory.Exists(modFolder))
				Directory.CreateDirectory(modFolder);

			maxSteps(4);

			updateProgress("Setting up Assets for Mod Export");
			setAssemblies();

			updateProgress("Exporting Mod Assets");
			genAssemblies();

			updateProgress("Copying Mod game system folder");
			CopySystemFolder(modFolder);

			updateProgress("Creating mod.ini");
			createMod();
		}
		#endregion

		#region Form Functions
		private void buildWindow_Shown(object sender, EventArgs e)
		{
			bool sa2 = true;
			chkBoxEXE.Items.Clear();
			chkBoxDLL.Items.Clear();
			chkBoxMDL.Items.Clear();

			switch (SAToolsHub.setGame)
			{
				case ("SADXPC"):
					SADXModInfo sadxMod = SplitTools.IniSerializer.Deserialize<SADXModInfo>(Path.Combine(SAToolsHub.projectDirectory, "mod.ini"));
					modName = sadxMod.Name;
					gameEXE = "sonic";
					sysFolder = "system";

					tabControl1.TabPages.Remove(tabMDL);
					break;
				case ("SA2PC"):
					SA2ModInfo sa2Mod = SplitTools.IniSerializer.Deserialize<SA2ModInfo>(Path.Combine(SAToolsHub.projectDirectory, "mod.ini"));
					modName = sa2Mod.Name;
					gameEXE = "sonic2app";
					sysFolder = "gd_PC";

					if (!tabControl1.Contains(tabMDL))
						tabControl1.TabPages.Add(tabMDL);
					DirectoryInfo charFiles = new DirectoryInfo(Path.Combine(SAToolsHub.projectDirectory, "figure\\bin"));

					foreach (DirectoryInfo dir in charFiles.GetDirectories())
					{
						string name = dir.Name;
						if (name.Contains("mdl") || name.Contains("mtn"))
						{
							chkBoxMDL.Items.Add(name);
						}
					}
					break;
			}

			foreach(Templates.SplitEntry splitEntry in SAToolsHub.projSplitEntries)
			{
				string srcFile = splitEntry.SourceFile.ToLower();
				if (srcFile.Contains("exe"))
				{
					chkBoxEXE.Items.Add(splitEntry);
					if (splitEntry.CmnName != null)
						chkBoxEXE.DisplayMember = "CmnName";
					else
						chkBoxEXE.DisplayMember = "IniFile";
				}

				if (srcFile.Contains("dll"))
				{
					chkBoxDLL.Items.Add(splitEntry);
					if (splitEntry.CmnName != null)
						chkBoxDLL.DisplayMember = "CmnName";
					else
						chkBoxDLL.DisplayMember = "IniFile";
				}
			}
		}
		
		private void btnManual_Click(object sender, EventArgs e)
		{
			SplitTools.Game game;
			switch (SAToolsHub.setGame)
			{
				case ("SADXPC"):
					game = SplitTools.Game.SADX;
					break;
				case ("SA2PC"):
					game = SplitTools.Game.SA2B;
					break;
				default:
					game = SplitTools.Game.SADX;
					break;
			}

			setAssemblies();
			manualBuildWindow.Initalize(game, modName, SAToolsHub.projectDirectory.ToString(),
				Path.Combine(SAToolsHub.gameDirectory.ToString(), "mods"), assemblies, gameEXE);
			manualBuildWindow.ShowDialog();
		}

		private void btnAuto_Click(object sender, EventArgs e)
		{
#if !DEBUG
			backgroundWorker1.RunWorkerAsync();
			backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleteAlert;
#endif
#if DEBUG
			backgroundWorker1_DoWork(null, null);
			BackgroundWorker1_RunWorkerCompleteAlert(null, null);
#endif
		}

		private void btnChkAll_Click(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == tabEXE)
			{
				for (int i = 0; i < chkBoxEXE.Items.Count; i++)
				{
					chkBoxEXE.SetItemChecked(i, true);
				}
			}

			if (tabControl1.SelectedTab == tabDLL)
			{
				for (int i = 0; i < chkBoxDLL.Items.Count; i++)
				{
					chkBoxDLL.SetItemChecked(i, true);
				}
			}

			if (tabControl1.SelectedTab == tabMDL)
			{
				for (int i = 0; i < chkBoxMDL.Items.Count; i++)
				{
					chkBoxMDL.SetItemChecked(i, true);
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == tabEXE)
			{
				for (int i = 0; i < chkBoxEXE.Items.Count; i++)
				{
					chkBoxEXE.SetItemChecked(i, false);
				}
			}

			if (tabControl1.SelectedTab == tabDLL)
			{
				for (int i = 0; i < chkBoxDLL.Items.Count; i++)
				{
					chkBoxDLL.SetItemChecked(i, false);
				}
			}

			if (tabControl1.SelectedTab == tabMDL)
			{
				for (int i = 0; i < chkBoxMDL.Items.Count; i++)
				{
					chkBoxMDL.SetItemChecked(i, false);
				}
			}
		}
		#endregion
	}
}
