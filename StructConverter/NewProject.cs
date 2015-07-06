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
	public partial class NewProject : Form
	{
		List<char> invalidChars;
		bool hidden = false;
        ModBuildProgress progressWindow;
        private string cancellationMessage = "";

		public NewProject()
		{
			InitializeComponent();

			invalidChars = new List<char>(Path.GetInvalidPathChars());
            progressWindow = new ModBuildProgress();
            SupplyDefaultPath();
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
                    DialogResult userOverWriteResult = MessageBox.Show("This project exists already. Would you like to over-write the files?", "Overwrite Warning!", MessageBoxButtons.OKCancel);
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
                    if (MainForm.VerifyGamePath(SA_Tools.Game.SADX, gamePathTextBox.Text, out cancellationMessage))
                    {
                        #region Get Sub-Dirs and Required Files
                        splitBackgroundWorker.ReportProgress(1, "Getting Directories...");
                        string systemFolder = string.Concat(gamePathTextBox.Text, "\\system\\");
                        string splitConfigDirectory = string.Concat(gamePathTextBox.Text, "\\SplitConfig\\");
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
                        args[2] = string.Concat(gamePathTextBox.Text, "\\Projects\\", ProjectNameText.Text, "\\");

                        splitBackgroundWorker.ReportProgress(25, "Splitting sonic.exe...");

                        string errorMessage = "";
                        if (!Split.EXEFile(args, out errorMessage))
                        {
                            cancellationMessage = string.Concat("There was an error splitting sonic.exe: ", errorMessage);
                            splitBackgroundWorker.CancelAsync();
                            return;
                        }

                        string[] splitFiles = { "ADV00MODELS", "ADV01CMODELS", "ADV01MODELS", "ADV02MODELS", "ADV03MODELS",
                                        "BOSSCHAOS0MODELS", "CHAOSTGGARDEN02MR_DAYTIME", "CHAOSTGGARDEN02MR_EVENING", "CHAOSTGGARDEN02MR_NIGHT",
                                        "CHRMODELS" };

                        int fileIndex = 0;
                        foreach (string file in splitFiles)
                        {
                            args[0] = string.Concat(gamePathTextBox.Text, "\\system\\", file, ".DLL"); // our dll file to split
                            args[1] = string.Concat(gamePathTextBox.Text, "\\SplitConfig\\", file.ToLower(), ".ini"); // our ini file data mapping

                            // Don't unpack the modloader's chrmodels.dll
                            if (file == "CHRMODELS")
                            {
                                long chrSize = new FileInfo(args[0]).Length;

                                if (chrSize == 819200) // we've found modloader's dll, change it.
                                {
                                    args[0] = string.Concat(gamePathTextBox.Text, "\\system\\", "CHRMODELS_orig", ".DLL"); // our dll file to split
                                }
                            }

                            splitBackgroundWorker.ReportProgress((90 - 25 / splitFiles.Length) + 25, string.Format("Splitting {0}.DLL", file)); // our calculation for this is giving invalid values    
                            // greater than 100. Do the math out, then fix this.

                            if (!Split.DLL(args, out errorMessage))
                            {
                                cancellationMessage = string.Format("There was an error splitting {0}.\nThe problem was: {1}\nAborting. Your mod has NOT been successfully generated.\nReport this to #x-hax irc.badnik.net", errorMessage);
                                splitBackgroundWorker.CancelAsync();
                                return;
                            }
                            fileIndex++;
                        }
                        #endregion

                        #region Moving sadxlvl.ini
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
                MessageBox.Show("Success! Your mod project has been created!");
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
	}
}
