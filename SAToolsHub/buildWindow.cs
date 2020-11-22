using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon.ModManagement;
using ProjectManagement;
using SA_Tools;

namespace SAToolsHub
{
	public partial class buildWindow : Form
	{
		SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow manualBuildWindow =
			new SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow();

		Dictionary<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType> assemblies =
				new Dictionary<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType>();

		string gameEXE;
		string modName;
		string modFolder;
		string sysFolder;

		//private IniFile CombineEXEEntries(SplitEntry splitEntry)
		//{

		//}

		void setAssemblies()
		{
			assemblies.Clear();
			List<SplitEntry> splitEntry = SAToolsHub.projSplitEntries;
			
			switch (SAToolsHub.setGame)
			{
				case ("SADXPC"):
					for (int i = 0; i < checkedListBox1.Items.Count; i++)
					{
						if (checkedListBox1.GetItemChecked(i))
						{
							if (splitEntry[i].SourceFile.Contains("exe"))
							{
								assemblies.Add(splitEntry[i].IniFile, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);
							}
							else
							{
								if (splitEntry[i].IniFile == "chrmodels")
								{
									assemblies.Add((splitEntry[i].IniFile + "_orig"), SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
								}
								else
								{
									assemblies.Add(splitEntry[i].IniFile, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
								}
							}
						}
					}
					break;

				case ("SA2PC"):
					for (int i = 0; i < checkedListBox1.Items.Count; i++)
					{
						if (checkedListBox1.GetItemChecked(i))
						{
							if (splitEntry[i].SourceFile.Contains("exe"))
							{
								assemblies.Add(splitEntry[i].IniFile, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);
							}
							else
							{
								if (splitEntry[i].IniFile == "data_dll")
								{
									assemblies.Add((splitEntry[i].IniFile + "_orig"), SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
								}
								else
								{
									assemblies.Add(splitEntry[i].IniFile, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
								}
							}
						}
					}
					break;

				default:
					break;
			}
		}

		void genAssemblies()
		{
			SA_Tools.IniData EXEiniData = new SA_Tools.IniData();
			Dictionary<string, bool> itemsEXEToExport = new Dictionary<string, bool>();

			foreach (KeyValuePair<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType> assembly in assemblies)
			{
				string iniPath = Path.Combine(SAToolsHub.projectDirectory.ToString(), assembly.Key + "_data.ini");

				Dictionary<string, bool> itemsToExport = new Dictionary<string, bool>();

				switch (assembly.Value)
				{
					case SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe:
						EXEiniData = SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverter.LoadINI(iniPath, ref itemsEXEToExport);
						break;

					case SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL:
						SA_Tools.SplitDLL.DllIniData dllIniData =
							SonicRetro.SAModel.SAEditorCommon.DLLModGenerator.DLLModGen.LoadINI(iniPath, ref itemsToExport);

						SonicRetro.SAModel.SAEditorCommon.DLLModGenerator.DLLModGen.ExportINI(dllIniData,
							itemsToExport, Path.Combine(modFolder, assembly.Key + "_data.ini"));
						break;
					default:
						break;
				}
			}
			if (itemsEXEToExport.Count > 0)
			{
				SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverter.ExportINI(EXEiniData,
					itemsEXEToExport, Path.Combine(modFolder, gameEXE + "_data.ini"));
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
					SADXModInfo sadxModInfo = SA_Tools.IniSerializer.Deserialize<SADXModInfo>(baseModIniPath);

					// set all of our assemblies properly
					string ADV00MODELS = "ADV00MODELS";
					string ADV01CMODELS = "ADV01CMODELS";
					string ADV01MODELS = "ADV01MODELS";
					string ADV02MODELS = "ADV02MODELS";
					string ADV03MODELS = "ADV03MODELS";
					string BOSSCHAOS0MODELS = "BOSSCHAOS0MODELS";
					string CHAOSTGGARDEN02MR_DAYTIME = "CHAOSTGGARDEN02MR_DAYTIME";
					string CHAOSTGGARDEN02MR_EVENING = "CHAOSTGGARDEN02MR_EVENING";
					string CHAOSTGGARDEN02MR_NIGHT = "CHAOSTGGARDEN02MR_NIGHT";

					if (assemblies.ContainsKey(ADV00MODELS)) sadxModInfo.ADV00MODELSData = ADV00MODELS + dataSuffix;
					if (assemblies.ContainsKey(ADV01CMODELS)) sadxModInfo.ADV01CMODELSData = ADV01CMODELS + dataSuffix;
					if (assemblies.ContainsKey(ADV01MODELS)) sadxModInfo.ADV01MODELSData = ADV01MODELS + dataSuffix;
					if (assemblies.ContainsKey(ADV02MODELS)) sadxModInfo.ADV02MODELSData = ADV02MODELS + dataSuffix;
					if (assemblies.ContainsKey(ADV03MODELS)) sadxModInfo.ADV03MODELSData = ADV03MODELS + dataSuffix;
					if (assemblies.ContainsKey(BOSSCHAOS0MODELS)) sadxModInfo.BOSSCHAOS0MODELSData = BOSSCHAOS0MODELS + dataSuffix;
					if (assemblies.ContainsKey(CHAOSTGGARDEN02MR_DAYTIME)) sadxModInfo.CHAOSTGGARDEN02MR_DAYTIMEData = CHAOSTGGARDEN02MR_DAYTIME + dataSuffix;
					if (assemblies.ContainsKey(CHAOSTGGARDEN02MR_EVENING)) sadxModInfo.CHAOSTGGARDEN02MR_EVENINGData = CHAOSTGGARDEN02MR_EVENING + dataSuffix;
					if (assemblies.ContainsKey(CHAOSTGGARDEN02MR_NIGHT)) sadxModInfo.CHAOSTGGARDEN02MR_NIGHTData = CHAOSTGGARDEN02MR_NIGHT + dataSuffix;
					if (assemblies.ContainsKey("sonic")) sadxModInfo.EXEData = "sonic_data.ini";

					// save our output
					SA_Tools.IniSerializer.Serialize(sadxModInfo, outputModIniPath);
					break;

				case "SA2PC":
					SA2ModInfo sa2ModInfo = SA_Tools.IniSerializer.Deserialize<SA2ModInfo>(baseModIniPath);

					if (assemblies.ContainsKey("Data_DLL_orig")) sa2ModInfo.DLLData = "Data_DLL_orig" + dataSuffix;
					if (assemblies.ContainsKey("sonic2app")) sa2ModInfo.EXEData = "sonic2app_data.ini";

					// save our output
					SA_Tools.IniSerializer.Serialize(sa2ModInfo, outputModIniPath);
					break;
				default:
					break;
			}
		}

		public void CopySystemFolder(string modFolder)
		{
			string projectSystemPath = Path.Combine(SAToolsHub.projectDirectory, sysFolder);
			string modSystemPath = Path.Combine(modFolder, sysFolder);

			SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverter.CopyDirectory(
				new DirectoryInfo(projectSystemPath), modSystemPath);

			if (SAToolsHub.setGame == "SADX")
			{
				string texturesPath = Path.Combine(SAToolsHub.projectDirectory, "textures");
				SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverter.CopyDirectory(new DirectoryInfo(texturesPath), modSystemPath);
			}
		}

		public buildWindow()
		{
			InitializeComponent();
		}

		private void buildWindow_Shown(object sender, EventArgs e)
		{
			chkAll.Checked = false;
			checkedListBox1.Items.Clear();

			switch (SAToolsHub.setGame)
			{
				case ("SADX"):
					SADXModInfo sadxMod = SA_Tools.IniSerializer.Deserialize<SADXModInfo>(Path.Combine(SAToolsHub.projectDirectory.ToString(), "mod.ini"));
					modName = sadxMod.Name;
					gameEXE = "sonic";
					sysFolder = "system";
					break;
				case ("SA2PC"):
					SA2ModInfo sa2Mod = SA_Tools.IniSerializer.Deserialize<SA2ModInfo>(Path.Combine(SAToolsHub.projectDirectory.ToString(), "mod.ini"));
					modName = sa2Mod.Name;
					gameEXE = "sonic2app";
					break;
			}

			foreach(SplitEntry splitEntry in SAToolsHub.projSplitEntries)
			{
				checkedListBox1.Items.Add(splitEntry.IniFile);
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAll.Checked)
			{
				for (int i = 0; i < checkedListBox1.Items.Count; i++)
					checkedListBox1.SetItemCheckState(i, CheckState.Checked);
				checkedListBox1.Enabled = false;
			}
			else if (!chkAll.Checked)
			{
				checkedListBox1.Enabled = true;
			}
		}

		private void btnManual_Click(object sender, EventArgs e)
		{
			setAssemblies();
			//manualBuildWindow.Initalize(game, modName, SAToolsHub.projectDirectory.ToString(),
				//Path.Combine(SAToolsHub.gameSystemDirectory.ToString(), "mods"), assemblies);
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

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			using (SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress = new SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog("Building Project"))
			{
				Action showProgress = () =>
				{
					Invoke((Action)progress.Show);
				};

				Action<string> stepProgress = (string update) =>
				{
					progress.StepProgress();
					progress.SetStep(update);
				};

				AutoBuild(showProgress, stepProgress);
			}
		}

		private void BackgroundWorker1_RunWorkerCompleteAlert(object sender, RunWorkerCompletedEventArgs e)
		{
			backgroundWorker1.RunWorkerCompleted -= BackgroundWorker1_RunWorkerCompleteAlert;
			MessageBox.Show("Build complete!");
		}

		private void AutoBuild(Action showProgress, Action<string> updateProgress)
		{
			showProgress();

			string modsFolder = SAToolsHub.gameSystemDirectory + "/mods";
			modFolder = Path.Combine(modsFolder, modName);
			
			if (!Directory.Exists(modFolder))
				Directory.CreateDirectory(modFolder);

			updateProgress("Setting up Assets for Mod Export");
			setAssemblies();

			updateProgress("Exporting Mod Assets");
			genAssemblies();

			updateProgress("Copying Mod game system folder");
			CopySystemFolder(modFolder);

			updateProgress("Creating mod.ini");
			createMod();
		}
	}
}
