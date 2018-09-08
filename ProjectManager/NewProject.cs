using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SonicRetro.SAModel.SAEditorCommon.UI;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.ModManagement;

namespace ProjectManager
{
    public struct SplitData
    {
        // both of these are relative to the
        public string dataFile;
        public string iniFile;
    }

    public struct SplitMDLData
    {
        public bool isBigEndian;
        public string dataFile;
        public string[] animationFiles;
    }

    public partial class NewProject : Form
    {
        public delegate void ProjectCreationHandler(SA_Tools.Game game, string projectName, string fullProjectPath);
        public event ProjectCreationHandler ProjectCreated;

        public Action CreationCanceled;

        bool sadxIsValid = false;
        bool sa2pcIsValid = false;

        SplitData[] sadxpcsplits = new SplitData[]
        {
            new SplitData() { dataFile="sonic.exe", iniFile = "splitSADX.ini" },
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

        SplitMDLData[] sa2PCSplitMDL = new SplitMDLData[]
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
                    "plcommtn.prs"
                }
            },
            new SplitMDLData()
            {
                isBigEndian = true,
                dataFile = "resource\\gd_PC\\ssonicmdl.prs",
                animationFiles = new string[]
                {
                    "plcommtn.prs"
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

        SplitData sonic2AppSplit = new SplitData() { dataFile = "sonic2app.exe", iniFile = "splitsonic2app.ini" };

        public NewProject()
        {
            InitializeComponent();

			sadxIsValid = GamePathChecker.CheckSADXPCValid(
				Program.Settings.SADXPCPath, out string sadxInvalidReason);

			sa2pcIsValid = GamePathChecker.CheckSA2PCValid(
				Program.Settings.SA2PCPath, out string sa2pcInvalidReason);

			backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;

            SADXPCButton.Checked = (sadxIsValid);
            SA2RadioButton.Checked = (false);

            SetControls();
        }

		Exception createError = null;
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
			if (createError != null)
				throw createError;
            if(ProjectCreated != null)
            {
                ProjectCreationHandler dispatch = ProjectCreated;

                dispatch(GetGameForRadioButtons(), ProjectNameBox.Text, GetOutputFolder());
            }
        }

        void SetControls()
        {
            SADXPCButton.Enabled = (sadxIsValid);
            SA2RadioButton.Enabled = (sa2pcIsValid);

            NextButton.Enabled = (ProjectNameBox.Text.Length > 0);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            // do any validation
            ProjectNameBox.Enabled = false;
            SA2RadioButton.Enabled = false;
            SADXPCButton.Enabled = false;
            BackButton.Enabled = false;
            NextButton.Enabled = false;
            ControlBox = false;
			createError = null;
#if !DEBUG
            backgroundWorker1.RunWorkerAsync();
#endif
#if DEBUG
            backgroundWorker1_DoWork(null, null);
            BackgroundWorker1_RunWorkerCompleted(null, null);
#endif
        }

        private void ProjectNameBox_TextChanged(object sender, EventArgs e)
        {
            SetControls();
        }

        private SA_Tools.Game GetGameForRadioButtons()
        {
            if (SADXPCButton.Checked) return SA_Tools.Game.SADX;
            else if (SA2RadioButton.Checked) return SA_Tools.Game.SA2B;
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

        private void SplitData_Sub(SplitData splitData, ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder)
        {
            string datafilename = Path.Combine(gameFolder, splitData.dataFile);
            string inifilename = Path.Combine(iniFolder, splitData.iniFile);
            string projectFolderName = outputFolder;

            progress.StepProgress();
            progress.SetStep("Splitting: " + splitData.dataFile);

            #region Validating Inputs
            if (!File.Exists(datafilename))
            {
                Console.WriteLine(datafilename + " not found. Aborting.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();

                throw new Exception(ERRORVALUE.NoSourceFile.ToString());
                //return (int)ERRORVALUE.NoSourceFile;
            }

            if (!File.Exists(inifilename))
            {
                Console.WriteLine(inifilename + " not found. Aborting.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();

                throw new Exception(ERRORVALUE.NoDataMapping.ToString());
                //return (int)ERRORVALUE.NoDataMapping;
            }

            if (!Directory.Exists(projectFolderName))
            {
                // try creating the directory
                bool created = true;

                try
                {
                    // check to see if trailing charcter closes 
                    Directory.CreateDirectory(projectFolderName);
                }
                catch
                {
                    created = false;
                }

                if (!created)
                {
                    // couldn't create directory.
                    Console.WriteLine("Output folder did not exist and couldn't be created.");
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();

                    throw new Exception(ERRORVALUE.InvalidProject.ToString());
                    //return (int)ERRORVALUE.InvalidProject;
                }
            }
            #endregion

            // switch on file extension - if dll, use dll splitter
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(datafilename);

            int result = (fileInfo.Extension.ToLower().Contains("dll")) ? SplitDLL.SplitDLL.SplitDLLFile(datafilename, inifilename, projectFolderName) :
                Split.Split.SplitFile(datafilename, inifilename, projectFolderName);
        }

        private void DoSADXSplit(ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder)
        {
            progress.StepProgress();
            progress.SetStep("Splitting EXE & DLLs");

            foreach (SplitData splitData in sadxpcsplits)
            {
                SplitData_Sub(splitData, progress, gameFolder, iniFolder, outputFolder);
            }

            // do our last split, chrmodels
            SplitData chrmodelsSplitData = new SplitData();

            if(File.Exists(Path.Combine(gameFolder, "system/CHRMODELS_orig.dll")))
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

            SplitData_Sub(chrmodelsSplitData, progress, gameFolder, iniFolder, outputFolder);

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

        private void GenerateSADXModFile(string gameFolder, string projectName)
        {
			SADXModInfo modInfo = new SADXModInfo
			{
				Author = AuthorTextBox.Text,
				Name = ProjectNameBox.Text,
				Version = string.Format("0"),
				Description = DescriptionTextBox.Text
			};

			string outputPath = Path.Combine(gameFolder, string.Format("Projects/{0}/mod.ini", projectName));

            SA_Tools.IniSerializer.Serialize(modInfo, outputPath);
        }

        private void GenerateSA2ModFile(string gameFolder, string projectName)
        {
			SA2ModInfo modInfo = new SA2ModInfo
			{
				Author = AuthorTextBox.Text,
				Name = ProjectNameBox.Text,
				Version = string.Format("0"),
				Description = DescriptionTextBox.Text
			};

			string outputPath = Path.Combine(gameFolder, string.Format("Projects/{0}/mod.ini", projectName));

            SA_Tools.IniSerializer.Serialize(modInfo, outputPath);
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

            foreach(string directory in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                if (directoryInfo.Name.Equals("bin") || directoryInfo.Name.Equals("obj")) continue;

                string copySrcDir = Path.Combine(sourceFolder, directoryInfo.Name);
                string copyDstDir = Path.Combine(destinationFolder, directoryInfo.Name);

                CopyFolder(copySrcDir, copyDstDir);
            }
        }

        private void DoSA2PCSplit(ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder)
        {
            // split data dll
            #region Split Data DLL

            progress.StepProgress();
            progress.SetStep("Splitting Data DLL");

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

			SplitData_Sub(dllSplitData, progress, gameFolder, iniFolder, outputFolder);
            #endregion

            // run split mdl commands
            progress.StepProgress();
            progress.SetStep("Splitting character model files");

            foreach (SplitMDLData splitMDL in sa2PCSplitMDL)
            {
                string filePath = Path.Combine(gameFolder, splitMDL.dataFile);
                string fileOutputFolder = Path.GetDirectoryName(Path.Combine(outputFolder, splitMDL.dataFile));

                SplitMDL.SplitMDL.Split(splitMDL.isBigEndian, filePath,
                    fileOutputFolder, splitMDL.animationFiles);
            }

			// split sonic2app
			#region Split sonic2app
			{
				progress.StepProgress();
                progress.SetStep("Splitting Sonic2App");

				SplitData_Sub(sonic2AppSplit, progress, gameFolder, iniFolder, outputFolder);
			}
			#endregion
		}

        private string GetOutputFolder()
        {
            return Path.Combine(GetGameFolder(), string.Format("Projects/{0}/", ProjectNameBox.Text));
        }

        private string GetGameFolder()
        {
            return ((SADXPCButton.Checked) ? Program.Settings.SADXPCPath : Program.Settings.SA2PCPath);
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
#if !DEBUG
			try
			{
#endif
			// we should disable all form controls
			SA_Tools.Game game = GetGameForRadioButtons();

            using (ProgressDialog progress = new ProgressDialog("Creating project"))
            {
                Invoke((Action)progress.Show);

                // create the folder
                progress.StepProgress();
                progress.SetStep("Creating Folder");

                string gameFolder = GetGameFolder();

                string outputFolder = GetOutputFolder();

                Directory.CreateDirectory(outputFolder);

                // get our ini files to split
                string iniFolder = "";
#if DEBUG
                iniFolder = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../Configuration/" + GetIniFolderForGame(game);
#endif

#if !DEBUG
                iniFolder = Path.GetDirectoryName(Application.ExecutablePath) + "/../" + GetIniFolderForGame(game);
#endif

                // we need to run split
                if (game == SA_Tools.Game.SADX) DoSADXSplit(progress, gameFolder, iniFolder, outputFolder);
                else if (game == SA_Tools.Game.SA2B) DoSA2PCSplit(progress, gameFolder, iniFolder, outputFolder);

                progress.StepProgress();
                progress.SetStep("Creating mod.ini");

                if (game == SA_Tools.Game.SADX) GenerateSADXModFile(gameFolder, ProjectNameBox.Text);
                else GenerateSA2ModFile(gameFolder, ProjectNameBox.Text);

                // create our system directory
                string systemPath = Path.Combine(outputFolder, GamePathChecker.GetSystemPathName(game));
                Directory.CreateDirectory(systemPath);

                Invoke((Action)progress.Close);
            }
#if !DEBUG
			}
			catch (Exception ex)
			{
				createError = ex;
			}
#endif
		}

        private void NewProject_Shown(object sender, EventArgs e)
        {
			// check valid states again
			sadxIsValid = GamePathChecker.CheckSADXPCValid(
				Program.Settings.SADXPCPath, out string sadxInvalidReason);

			sa2pcIsValid = GamePathChecker.CheckSA2PCValid(
				Program.Settings.SA2PCPath, out string sa2pcInvalidReason);

			ProjectNameBox.Enabled = true;
            SA2RadioButton.Enabled = sa2pcIsValid;
            SADXPCButton.Enabled = sadxIsValid;
            BackButton.Enabled = true;
            NextButton.Enabled = ProjectNameBox.Text.Length > 0;
            ControlBox = true;

            SetControls();
        }

        private void NavBack()
        {
            CreationCanceled.Invoke();
        }

        private void NewProject_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            NavBack();
            Hide();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            NavBack();
            Hide();
        }

        private void SA2RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("SA2 Radio button: " + SA2RadioButton.Checked);
        }

        private void SADXPCButton_CheckedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("SADX Radio button: " + SADXPCButton.Checked);
        }
    }
}
