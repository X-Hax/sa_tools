using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SonicRetro.SAModel.SAEditorCommon.ModManagement;

namespace ProjectManager
{
    public partial class ProjectActions : Form
    {
        public Action NavigateBack;

        SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverterUI structConverterUI = 
            new SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverterUI();
        
        SonicRetro.SAModel.SAEditorCommon.DLLModGenerator.DLLModGenUI modGenUI = 
           new SonicRetro.SAModel.SAEditorCommon.DLLModGenerator.DLLModGenUI();

        SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow manualBuildWindow =
            new SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow();

        SA_Tools.Game game;
        string projectFolder;
        string projectName;

        public ProjectActions()
        {
            InitializeComponent();
        }

        private void ProjectActions_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            NavigateBack.Invoke();
        }

        public void Init(SA_Tools.Game game, string projectName, string projectFolder)
        {
            this.game = game;
            this.projectName = projectName;
            ProjectNameLAbel.Text = projectName + " : " +  game.ToString();
            this.projectFolder = projectFolder;
        }

        public void CopySystemFolder(string modFolder)
        {
            string projectSystemPath = Path.Combine(projectFolder, SonicRetro.SAModel.SAEditorCommon.GamePathChecker.GetSystemPathName(game));
            string modSystemPath = Path.Combine(modFolder, SonicRetro.SAModel.SAEditorCommon.GamePathChecker.GetSystemPathName(game));

            SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverter.CopyDirectory(
                new DirectoryInfo(projectSystemPath), modSystemPath);
        }

        private void BuildDLLDerivedData_Click(object sender, EventArgs e)
        {
            modGenUI.SetProjectFolder(projectFolder);
            modGenUI.SetModFolder(Path.Combine(Program.Settings.GetModPathForGame(game),
                projectName));

            modGenUI.ShowDialog();
        }

        private void ExeBuildButton_Click(object sender, EventArgs e)
        {
            structConverterUI.SetProjectFolder(projectFolder);
            structConverterUI.SetModFolder(Path.Combine(Program.Settings.GetModPathForGame(game),
                projectName));

            // we need to set the ini file to open properly.
            // we can know which one to load because there's only one exe per game
            string iniFileToOpen = (game == SA_Tools.Game.SADX) ? "sonic_data.ini" : "sonic2app_data.ini";

            structConverterUI.OpenFile(iniFileToOpen);
        }

        private void SADXLVL2Button_Click(object sender, EventArgs e)
        {
            // launch sadxlvl2
            string sadxlvl2Path = "";

#if DEBUG
            sadxlvl2Path = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SADXLVL2/bin/Debug/SADXLVL2.exe";
#endif
#if !DEBUG
            sadxlvl2Path = Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXPC/SADXLVL2/SADXLVL2.exe";
#endif

            string projectArgumentsPath = string.Format("\"{0}\"", Path.Combine(projectFolder, "sadxlvl.ini"));

            System.Diagnostics.ProcessStartInfo sadxlvl2StartInfo = new System.Diagnostics.ProcessStartInfo(
                Path.GetFullPath(sadxlvl2Path), projectArgumentsPath);

            System.Diagnostics.Process sadxlvl2Process = System.Diagnostics.Process.Start(sadxlvl2StartInfo);
        }

        private void SAMDLButton_Click(object sender, EventArgs e)
        {
            // launch samdl
            string samdlPath = "";

#if DEBUG
            samdlPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SAMDL/bin/Debug/SAMDL.exe";
#endif
#if !DEBUG
            samdlPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXPC/SAMDL/SAMDL.exe";
#endif

            System.Diagnostics.ProcessStartInfo samdlStartInfo = new System.Diagnostics.ProcessStartInfo(
                Path.GetFullPath(samdlPath)//,
                /*Path.GetFullPath(projectFolder)*/);

            System.Diagnostics.Process samdlProcess = System.Diagnostics.Process.Start(samdlStartInfo);
        }

        private void SADXTweaker2Button_Click(object sender, EventArgs e)
        {
            // launch sadxtweaker2
            string sadxtweaker2Path = "";

#if DEBUG
            sadxtweaker2Path = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SADXTweaker2/bin/Debug/SADXTweaker2.exe";
#endif
#if !DEBUG
            sadxtweaker2Path = Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXPC/SADXTweaker2/SADXTweaker2.exe.exe";
#endif
            string sonicDataPath = Path.GetFullPath(Path.Combine(projectFolder, "sonic_data.ini"));
            System.Diagnostics.ProcessStartInfo sadxTweaker2StartInfo = new System.Diagnostics.ProcessStartInfo(
                Path.GetFullPath(sadxtweaker2Path),
                string.Format("\"{0}\"", sonicDataPath));

            System.Diagnostics.Process sadxTweaker2Process = System.Diagnostics.Process.Start(sadxTweaker2StartInfo);
        }

        private List<String> GetSADXAssemblyNames()
        {
            List<String> sadxAssemblyNames = new List<string>();

            sadxAssemblyNames.Add("ADV00MODELS");
            sadxAssemblyNames.Add("ADV01CMODELS");
            sadxAssemblyNames.Add("ADV01MODELS");
            sadxAssemblyNames.Add("ADV02MODELS");
            sadxAssemblyNames.Add("ADV03MODELS");
            sadxAssemblyNames.Add("BOSSCHAOS0MODELS");
            sadxAssemblyNames.Add("CHAOSTGGARDEN02MR_DAYTIME");
            sadxAssemblyNames.Add("CHAOSTGGARDEN02MR_EVENING");
            sadxAssemblyNames.Add("CHAOSTGGARDEN02MR_NIGHT");

            return sadxAssemblyNames;
        }

        private void ManualBuildbutton_Click(object sender, EventArgs e)
        {
            // we have to get the assemblies for our game

            Dictionary<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType> assemblies = 
                new Dictionary<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType>();

            switch (game)
            {

                case SA_Tools.Game.SADX:
                    List<String> sadxAssemblyNames = GetSADXAssemblyNames();

                    // check for chrmodels or chrmodels_orig
                    string chrmodels = "chrmodels";
                    string chrmodelsOrig = "chrmodels_orig";

                    string chrmodelsCompletePath = Path.Combine(projectFolder, chrmodels + "_data.ini");
                    string chrmodelsOrigCompletePath = Path.Combine(projectFolder, chrmodelsOrig + "_data.ini");

                    if (File.Exists(chrmodelsCompletePath))
                    {
                        sadxAssemblyNames.Add(chrmodels);
                    }
                    else
                    {
                        sadxAssemblyNames.Add(chrmodelsOrig);
                    }

                    assemblies.Add("sonic", SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);

                    foreach (string assemblyName in sadxAssemblyNames)
                    {
                        assemblies.Add(assemblyName, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
                    }
                    break;
                case SA_Tools.Game.SA2B:

                    // dll
                    assemblies.Add("Data_DLL", SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);

                    // exe
                    assemblies.Add("sonic2app", SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);
                    break;
                default:
                    break;
            }

            manualBuildWindow.Initalize(game, projectName,
                projectFolder, Path.Combine(Program.Settings.GetModPathForGame(game),
                projectName), assemblies);

            manualBuildWindow.ShowDialog();
        }

        private void AutoBuildButton_Click(object sender, EventArgs e)
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
            AutoBuild();
        }

        private void BackgroundWorker1_RunWorkerCompleteAlert(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker1.RunWorkerCompleted -= BackgroundWorker1_RunWorkerCompleteAlert;
            MessageBox.Show("Build complete!");
        }

        private void AutoBuild()
        {
            using (SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog progress = new SonicRetro.SAModel.SAEditorCommon.UI.ProgressDialog("Building Project"))
            {
                Invoke((Action)progress.Show);

                Dictionary<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType> assemblies =
                    new Dictionary<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType>();

                string modFolder = Path.Combine(Program.Settings.GetModPathForGame(game),
                    projectName);

                progress.StepProgress();
                progress.SetStep("Getting Assembies");
                switch (game)
                {
                    case SA_Tools.Game.SADX:
                        List<String> sadxAssemblyNames = GetSADXAssemblyNames();

                        // check for chrmodels or chrmodels_orig
                        string chrmodels = "chrmodels";
                        string chrmodelsOrig = "chrmodels_orig";

                        string chrmodelsCompletePath = Path.Combine(projectFolder, chrmodels + "_data.ini");
                        string chrmodelsOrigCompletePath = Path.Combine(projectFolder, chrmodelsOrig + "_data.ini");

                        if (File.Exists(chrmodelsCompletePath))
                        {
                            sadxAssemblyNames.Add(chrmodels);
                        }
                        else
                        {
                            sadxAssemblyNames.Add(chrmodelsOrig);
                        }

                        assemblies.Add("sonic", SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);

                        foreach (string assemblyName in sadxAssemblyNames)
                        {
                            assemblies.Add(assemblyName, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
                        }
                        break;

                    case SA_Tools.Game.SA2B:
                        // dll
                        assemblies.Add("Data_DLL", SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);

                        // exe
                        assemblies.Add("sonic2app", SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);
                        break;

                    default:
                        break;
                }

                // export only the modified items in each assembly
                progress.StepProgress();
                progress.SetStep("Exporting Assembies");

                foreach (KeyValuePair<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType> assembly in
                    assemblies)
                {
                    string iniPath = Path.Combine(projectFolder, assembly.Key + "_data.ini");

                    Dictionary<string, bool> itemsToExport = new Dictionary<string, bool>();

                    switch (assembly.Value)
                    {
                        case SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe:
                            SA_Tools.IniData iniData = SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverter.LoadINI(iniPath, ref itemsToExport);

                            SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverter.ExportINI(iniData,
                                itemsToExport, Path.Combine(modFolder, assembly.Key + "_data.ini"));
                            break;

                        case SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL:
                            SonicRetro.SAModel.SAEditorCommon.DLLModGenerator.DllIniData dllIniData =
                                SonicRetro.SAModel.SAEditorCommon.DLLModGenerator.DLLModGen.LoadINI(iniPath, ref itemsToExport);

                            SonicRetro.SAModel.SAEditorCommon.DLLModGenerator.DLLModGen.ExportINI(dllIniData,
                                itemsToExport, Path.Combine(modFolder, assembly.Key + "_data.ini"));
                            break;
                        default:
                            break;
                    }
                }

                // copy system folder
                progress.StepProgress();
                progress.SetStep("Copying System Folder");
                CopySystemFolder(modFolder);

                // generate final mod.ini based off of one in project folder
                progress.StepProgress();
                progress.SetStep("Creating Mod.ini");

                string baseModIniPath = Path.Combine(projectFolder, "mod.ini");
                string outputModIniPath = Path.Combine(modFolder, "mod.ini");

                switch (game)
                {
                    case SA_Tools.Game.SADX:
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

                        string dataSuffix = "_data.ini";

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

                    case SA_Tools.Game.SA2B:
                        SA2ModInfo sa2ModInfo = SA_Tools.IniSerializer.Deserialize<SA2ModInfo>(baseModIniPath);

                        sa2ModInfo.EXEFile = "sonic2app_data.ini";
                        sa2ModInfo.DLLFile = "Data_DLL_data.ini";

                        // save our output
                        SA_Tools.IniSerializer.Serialize(sa2ModInfo, outputModIniPath);
                        break;
                    default:
                        break;
                }

                // execute our post-build script
                string projectSettingsPath = Path.Combine(projectFolder, "ProjectSettings.ini");

                if (File.Exists(projectSettingsPath))
                {
                    ProjectSettings projectSettings = SA_Tools.IniSerializer.Deserialize<ProjectSettings>(projectSettingsPath);

                    string storedEnvironmentDirectory = Environment.CurrentDirectory;

                    if (File.Exists(projectSettings.PostBuildScript))
                    {
                        System.Diagnostics.ProcessStartInfo procStartInfo =
                            new System.Diagnostics.ProcessStartInfo(projectSettings.PostBuildScript);

                        System.Diagnostics.Process process = System.Diagnostics.Process.Start(procStartInfo);

                        while (!process.HasExited)
                        {
                            System.Threading.Thread.Sleep(100);
                        }

                        Environment.CurrentDirectory = storedEnvironmentDirectory;
                    }
                }
            }
        }

        private void systemButton_Click(object sender, EventArgs e)
        {
            string modFolder = Path.Combine(Program.Settings.GetModPathForGame(game),
                projectName);

            CopySystemFolder(modFolder);
        }

        private void PlayMod()
        {
            List<string> otherMods = new List<string>();

            string projectSettingsPath = Path.Combine(projectFolder, "ProjectSettings.ini");

            if (File.Exists(projectSettingsPath))
            {
                ProjectSettings projectSettings = SA_Tools.IniSerializer.Deserialize<ProjectSettings>(projectSettingsPath);
                foreach(string otherMod in projectSettings.OtherModsToRun)
                {
                    otherMods.Add(otherMod);
                }
            }
                string modLoaderConfig = "";
            switch (game)
            {
                case SA_Tools.Game.SADX:
                    modLoaderConfig = Path.Combine(Program.Settings.SADXPCPath, "mods/SADXModLoader.ini");

                    SADXLoaderInfo sadxLoaderInfo = SA_Tools.IniSerializer.Deserialize<SADXLoaderInfo>(modLoaderConfig);

                    sadxLoaderInfo.Mods.Clear();
                    sadxLoaderInfo.Mods.Add(projectName);

                    foreach(string otherMod in otherMods)
                    {
                        sadxLoaderInfo.Mods.Add(otherMod);
                    }

                    SA_Tools.IniSerializer.Serialize(sadxLoaderInfo, modLoaderConfig);
                    break;

                case SA_Tools.Game.SA2B:
                    modLoaderConfig = Path.Combine(Program.Settings.SA2PCPath, "mods/SA2ModLoader.ini");

                    SA2LoaderInfo sa2LoaderInfo = SA_Tools.IniSerializer.Deserialize<SA2LoaderInfo>(modLoaderConfig);
                    sa2LoaderInfo.Mods.Clear();
                    sa2LoaderInfo.Mods.Add(projectName);

                    foreach(string otherMod in otherMods)
                    {
                        sa2LoaderInfo.Mods.Add(otherMod);
                    }

                    SA_Tools.IniSerializer.Serialize(sa2LoaderInfo, modLoaderConfig);
                    break;

                default:
                    break;
            }

            string environmentDirectory = Environment.CurrentDirectory;

            string gameExecutable = Path.Combine(Program.Settings.GetGamePath(game), Program.Settings.GetExecutableForGame(game));

            Environment.CurrentDirectory = Path.GetDirectoryName(gameExecutable);

            System.Diagnostics.Process gameProcess = System.Diagnostics.Process.Start(gameExecutable);

            Environment.CurrentDirectory = environmentDirectory; // restore the old directory in case any other code relies upon it
        }

        private void RunWorker_PlayModOnCompletion(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker1.RunWorkerCompleted -= RunWorker_PlayModOnCompletion;
            PlayMod();
        }

        private void BuildAndRunButton_Click(object sender, EventArgs e)
        {

#if !DEBUG
            backgroundWorker1.RunWorkerCompleted += RunWorker_PlayModOnCompletion;

            backgroundWorker1.RunWorkerAsync();
            
#endif
#if DEBUG
            backgroundWorker1_DoWork(null, null);
            RunWorker_PlayModOnCompletion(null, null);
#endif
        }

        private void ConfigBuildButton_Click(object sender, EventArgs e)
        {
            using (ModConfigEditor configEditor = new ModConfigEditor())
            {
                configEditor.Init(game, projectName, projectFolder);
                configEditor.ShowDialog();
            }
        }
    }
}
