using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using SonicRetro.SAModel.SAEditorCommon.UI;

namespace ModGenerator
{
    /// <summary>
    /// This form's job is to generate a new project folder for modding, as well as explain to the user what's involved in starting a new mod project.
    /// </summary>
	public partial class NewProject : Form
	{
		private List<char> invalidChars;
		private bool hidden = false;
        private ModBuildProgress progressWindow;
        private string cancellationMessage = "";

        // sadx split options
        private bool splitSonicExe = true;
        private KeyValuePair<string, bool>[] sadxSplitDLLFiles = { new KeyValuePair<string, bool>("ADV00MODELS", true), new KeyValuePair<string, bool>("ADV01CMODELS", true),
                                                                 new KeyValuePair<string, bool>("ADV01MODELS", true), new KeyValuePair<string, bool>("ADV02MODELS", true),
                                                                 new KeyValuePair<string, bool>("ADV03MODELS", true), new KeyValuePair<string, bool>("BOSSCHAOS0MODELS", true),
                                                                 new KeyValuePair<string, bool>("CHAOSTGGARDEN02MR_DAYTIME", true), new KeyValuePair<string, bool>("CHAOSTGGARDEN02MR_EVENING", true),
                                                                 new KeyValuePair<string, bool>("CHAOSTGGARDEN02MR_NIGHT", true), new KeyValuePair<string, bool>("CHRMODELS", true)};

        public bool SplitSonicExe { get { return splitSonicExe; } set { splitSonicExe = value; } }
        public KeyValuePair<string, bool>[] SadxSplitDLLFiles { get { return sadxSplitDLLFiles; } set { sadxSplitDLLFiles = value; } } 

		public NewProject()
		{
			InitializeComponent();

			invalidChars = new List<char>(Path.GetInvalidPathChars());
            progressWindow = new ModBuildProgress();
            SupplyDefaultPath();

            ToolTip gamePathTip = new ToolTip();
            gamePathTip.SetToolTip(gamePathTextBox, "The game's root folder goes here. It should have the executable file (eg: sonic.exe). The Projects and Mods folders should be within this folder. If the Projects folder does not exist, it will be created.");
            gamePathTip.SetToolTip(browseButton, "The game's root folder goes here. It should have the executable file (eg: sonic.exe). The Projects and Mods folders should be within this folder. If the Projects folder does not exist, it will be created.");
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.hidden = true;
			this.Hide();
		}

		private void browseButton_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = folderBrowserDialog1.ShowDialog();

			if (dialogResult == System.Windows.Forms.DialogResult.OK)
			{
				gamePathTextBox.Text = folderBrowserDialog1.SelectedPath;
			}

			SetContinue();
		}

		/// <summary>
		/// Activates the OK button, if it is valid to do so.
		/// </summary>
		private void SetContinue()
		{
			if (Directory.Exists(gamePathTextBox.Text) && projectNameLabel.Text.Length > 0 && ProjectPathValid())
			{
				acceptButton.Enabled = true;
			}
		}

		private bool ProjectPathValid()
		{
			foreach (char currentCharacter in projectNameLabel.Text)
			{
				if (invalidChars.Contains(currentCharacter)) return false;
			}

			return true;
		}

		private void acceptButton_Click(object sender, EventArgs e)
		{
			// validate our selection before continuing.
			if (Directory.Exists(gamePathTextBox.Text))
			{
                string projectFolder = string.Concat(gamePathTextBox.Text, "\\Projects\\", ProjectNameText.Text, "\\");
                if(Directory.Exists(projectFolder)) // check for our project folder existing already, if so give the user a prompt.
                {
                    DialogResult userOverWriteResult = MessageBox.Show("This project exists already. Would you like to over-write the files?\nNote: You can use the advanced settings button to specify which files to split.", "Overwrite Warning!", MessageBoxButtons.OKCancel);
                    if (userOverWriteResult == System.Windows.Forms.DialogResult.Cancel) return;
                }

                // we need to temporarily lock out all of our controls and display a progress bar or something.
                this.Enabled = false;
                progressWindow.Show();
                splitBackgroundWorker.RunWorkerAsync();
			}
			else MessageBox.Show("Game directory is invalid!");
		}

		private void NewProject_FormClosed(object sender, FormClosedEventArgs e)
		{
            if (!this.hidden) Application.Exit();
		}

		private void ProjectNameText_TextChanged(object sender, EventArgs e)
		{
			SetContinue();
		}

		private void gamePathTextBox_TextChanged(object sender, EventArgs e)
		{
			SetContinue();
		}

        private void splitBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (ProgressDialog progress = new ProgressDialog("Generating Mod " + ProjectNameText.Text, 0))
            {
                if (SADXRadioButton.Checked)
                {
                    string sonicExePath = Path.Combine(gamePathTextBox.Text, "sonic.exe");

                    // verify our game folder
                    if (SonicRetro.SAModel.SAEditorCommon.UI.ProjectSelector.VerifyGamePath(SA_Tools.Game.SADX, gamePathTextBox.Text, out cancellationMessage))
                    {
                        #region Get Sub-Dirs and Required Files
                        splitBackgroundWorker.ReportProgress(1, "Getting Directories...");
                        string systemFolder = string.Concat(gamePathTextBox.Text, "\\system\\");
                        string splitConfigDirectory = string.Concat(gamePathTextBox.Text, "\\SplitConfig\\");
                        string projectFolder = string.Concat(gamePathTextBox.Text, "\\Projects\\", ProjectNameText.Text, "\\");
                        #endregion

                        #region Create Necessary Split INI Files
                        if (!Directory.Exists(splitConfigDirectory)) Directory.CreateDirectory(splitConfigDirectory);
                        // check to see if all of our split ini files exist in the proper directory.
                        string[] configFiles = Directory.GetFiles(string.Concat(Application.StartupPath, "\\Config\\SADX\\Split\\"));
                        splitBackgroundWorker.ReportProgress(10, "Copying Data Mappings...");
                        foreach (string file in configFiles)
                        {
                            // check to see if the file exists in the game directory, if not, move it from the local config folder.
                            string destinationFile = string.Concat(splitConfigDirectory, Path.GetFileName(file));
                            FileInfo sourceFileHandle = new FileInfo(file);
                            FileInfo destinationFileInfo = new FileInfo(destinationFile);

                            if (!File.Exists(destinationFile) ||
                                ((sourceFileHandle.Length != destinationFileInfo.Length) || (sourceFileHandle.CreationTime != destinationFileInfo.CreationTime)))
                            {
                                File.Copy(string.Concat(file), destinationFile, true);
                            }
                        }
                        #endregion

                        #region Emulating old SplitAll.bat
                        // splitSADX.bat
                        string[] args = new string[3];
                        args[0] = sonicExePath;
                        args[1] = string.Concat(gamePathTextBox.Text, "\\SplitConfig\\splitSADX.ini");
                        args[2] = projectFolder;

                        string errorMessage = "";

                        if (splitSonicExe)
                        {
                            splitBackgroundWorker.ReportProgress(25, "Splitting sonic.exe...");

                            if (!Split.EXEFile(args, out errorMessage))
                            {
                                cancellationMessage = string.Concat("There was an error splitting sonic.exe: ", errorMessage);
                                splitBackgroundWorker.CancelAsync();
                                return;
                            }
                        }

                        int fileIndex = 0; // used for tracking percentages, which doesn't even work anyways.
                        foreach (KeyValuePair<string, bool> file in sadxSplitDLLFiles)
                        {
                            if (file.Value)
                            {
                                args[0] = string.Concat(gamePathTextBox.Text, "\\system\\", file.Key, ".DLL"); // our dll file to split
                                args[1] = string.Concat(gamePathTextBox.Text, "\\SplitConfig\\", file.Key.ToLower(), ".ini"); // our ini file data mapping

                                // Don't unpack the modloader's chrmodels.dll
                                if (file.Key == "CHRMODELS")
                                {
                                    long chrSize = new FileInfo(args[0]).Length;

                                    if (chrSize == 819200) // we've found modloader's dll, change it.
                                    {
                                        args[0] = string.Concat(gamePathTextBox.Text, "\\system\\", "CHRMODELS_orig", ".DLL"); // our dll file to split
                                    }
                                }

                                splitBackgroundWorker.ReportProgress((90 - 25 / sadxSplitDLLFiles.Length) + 25, string.Format("Splitting {0}.DLL", file.Key)); // our calculation for this is giving invalid values    
                                // greater than 100. Do the math out, then fix this.

                                if (!Split.DLL(args, out errorMessage))
                                {
                                    cancellationMessage = string.Format("There was an error splitting {0}.\nThe problem was: {1}\nAborting. Your mod has NOT been successfully generated.\nReport this to #x-hax irc.badnik.net", errorMessage);
                                    splitBackgroundWorker.CancelAsync();
                                    return;
                                }
                            }
                            fileIndex++;
                        }
                        #endregion

                        #region Moving sadxlvl.ini & objdefs.ini
                        string applicationDirectory = Application.StartupPath;
                        string sadxlvlINIPath = string.Concat(applicationDirectory, "\\Config\\SADX\\sadxlvl.ini");

                        splitBackgroundWorker.ReportProgress(90, "Moving sadxlvl.ini");

                        if (File.Exists(sadxlvlINIPath))
                        {
                            // copy sadxlvl ini to project folder
                            File.Copy(sadxlvlINIPath, string.Concat(gamePathTextBox.Text, "\\Projects\\", ProjectNameText.Text, "\\sadxlvl.ini"), true);
                        }
                        else
                        {
                            cancellationMessage = "Error - couldn't find sadxlvl.ini! Report this error at irc.badnik.net #x-hax";
                            splitBackgroundWorker.CancelAsync();
                            return;
                        }

                        string objdefsINIpath = string.Concat(applicationDirectory, "\\Config\\SADX\\objdefs.ini");

                        splitBackgroundWorker.ReportProgress(95, "Moving objdefs.ini");

                        if(File.Exists(objdefsINIpath))
                        {
                            // copy objdefs ini to project folder
                            File.Copy(objdefsINIpath, string.Concat(gamePathTextBox.Text, "\\Projects\\", ProjectNameText.Text, "\\objdefs.ini"), true);
                        }
                        else
                        {
                            cancellationMessage = "Error - couldn't find objdefs.ini! Report this error at irc.badnik.net #x-hax";
                            splitBackgroundWorker.CancelAsync();
                            return;
                        }
                        #endregion

                        #region Creating Template Sub-Folders
                        Directory.CreateDirectory(string.Concat(projectFolder, "\\system\\"));
                        Directory.CreateDirectory(string.Concat(projectFolder, "\\textures\\"));
                        Directory.CreateDirectory(string.Concat(projectFolder, "\\objdefs\\"));
                        Directory.CreateDirectory(string.Concat(projectFolder, "\\dllcache\\")).Attributes |= FileAttributes.Hidden;
                        #endregion
                    }
                    else // Cancel. Message should already be saved.
                    {
                        splitBackgroundWorker.CancelAsync();
                        return;
                    }
                }
                else if (SA2PCButton.Checked)
                {
                    cancellationMessage = "No SA2 mod support yet!";
                    splitBackgroundWorker.CancelAsync();
                    return;
                }

                splitBackgroundWorker.ReportProgress(100);
            }
        }

        private void splitBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Enabled = true;
            progressWindow.Hide();

            if(e.Error != null) // let's check out what went wrong.
            {
                MessageBox.Show(e.Error.Message);
                Application.Exit();
            }
            else if (e.Cancelled)
            {
                MessageBox.Show(cancellationMessage);
                Application.Exit();
            }
            else
            {
                if (SADXRadioButton.Checked) MessageBox.Show(string.Format("Your mod project has been created in <SADX Game Folder>/Projects/{0}.\nSystem and textures folders were created. Files placed in system will override sadx's default system folder.\nThe textures folder is for HD override textures. You can use PVMSharp to convert a PVM file into a texture folder with individual PNG files, which you can then edit in any graphics program.", ProjectNameText.Text));
                if (SA2PCButton.Checked) MessageBox.Show("Success! Your mod project has been created!");
                Application.Exit();
            }
        }

        private void splitBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                progressWindow.SetProgress(e.UserState.ToString(), e.ProgressPercentage);
            }
        }

        private void SupplyDefaultPath()
        {
            if (SADXRadioButton.Checked)
            {
                gamePathTextBox.Text = Properties.Settings.Default.SADXPath;
            }
            else if(SA2PCButton.Checked)
            {
                gamePathTextBox.Text = Properties.Settings.Default.SA2Path;
            }
        }

        private void SA2PCButton_CheckedChanged(object sender, EventArgs e)
        {
            SupplyDefaultPath();
        }

        private void SADXRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SupplyDefaultPath();
        }

        private void advancedSettingsButton_Click(object sender, EventArgs e)
        {
            using (SplitSettings settingsDialog = new SplitSettings(this))
            {
                settingsDialog.ShowDialog();
            }
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This dialog allows you to generate a new mod project folder. A mod project folder contains work-in-progress files for your mod. It can't be loaded into the game directly, and once you want to play your mod, you will need to use the previous menu's 'Build an Existing Mod' option. You can also use this dialog box with an existing mod IF you'd like to get missing data from the game (ie: you don't like your current version of Station Square, and want to get the original back.)");
        }
	}
}
