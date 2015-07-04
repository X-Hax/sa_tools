using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ModGenerator
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

            // check our sa2 and sadx paths for validity, if they're no good, ask our user to specify them.
            // Also, give them the option of changing the game path folders. (As ine one separate from this).

            string errorMessage = "None supplied";
            DialogResult lookForNewPath = System.Windows.Forms.DialogResult.None;
            if (Properties.Settings.Default.SADXPath == "" || (!VerifyGamePath(SA_Tools.Game.SADX, Properties.Settings.Default.SADXPath, out errorMessage)))
            {
                // show an error message that the sadx path is invalid, ask for a new one.
                lookForNewPath = MessageBox.Show(string.Format("The on-record SADX game directory doesn't appear to be valid because: {0}\nOK to supply one, Cancel to ignore.", errorMessage), "Directory Warning", MessageBoxButtons.OKCancel);
                if(lookForNewPath == System.Windows.Forms.DialogResult.OK)
                {
                    if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK) Properties.Settings.Default.SADXPath = folderBrowser.SelectedPath;
                }
            }

            errorMessage = "None Supplied";
            if (Properties.Settings.Default.SA2Path == "" || (!VerifyGamePath(SA_Tools.Game.SA2B, Properties.Settings.Default.SA2Path, out errorMessage)))
            {
                // show an error message that the sadx path is invalid, ask for a new one.
                lookForNewPath = MessageBox.Show(string.Format("The on-record SA2PC game directory doesn't appear to be valid because: {0}\nOK to supply one, Cancel to ignore.", errorMessage), "Directory Warning", MessageBoxButtons.OKCancel);
                if (lookForNewPath == System.Windows.Forms.DialogResult.OK)
                {
                    if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK) Properties.Settings.Default.SA2Path = folderBrowser.SelectedPath;
                }
            }

            Properties.Settings.Default.Save();
		}

		#region New Project methods
		private void NewProjectButton_Click(object sender, EventArgs e)
		{
            NewProject newProjectForm = new NewProject();
            this.Enabled = false; // This used to be a hide/show system but for some reason it was stuffing the window into the background.
            DialogResult projectResult = newProjectForm.ShowDialog();
            if (projectResult == System.Windows.Forms.DialogResult.Cancel)
            {
                this.Enabled = true;
            }
            else Application.Exit();
		}
		#endregion

		#region Build Existing Methods
		private void buildExistingButton_Click(object sender, EventArgs e)
		{
			ModBuilder newModbuilderForm = new ModBuilder();
			this.Hide();
			newModbuilderForm.ShowDialog();
			this.Close();
		}
		#endregion

        /// <summary>
        /// Attempts to verify a game's path.
        /// </summary>
        /// <param name="gameType">Which game we're looking to verify (SADX, SA2PC)</param>
        /// <param name="gameFolderPath">Location where the game executable is stored.</param>
        /// <param name="errorMessage">Reason, if any, for failure.</param>
        /// <returns>TRUE if the path is valid, FALSE if there was a problem. See errorMessage for more information.</returns>
        public static bool VerifyGamePath(SA_Tools.Game gameType, string gameFolderPath, out string errorMessage)
        {
            switch (gameType)
            {
                case SA_Tools.Game.SA1:
                    errorMessage = "SADC is not supported!";
                    return false;
                    
                case SA_Tools.Game.SADX:
                    // look for Sonic.exe
                    string sonicExePath = Path.Combine(gameFolderPath, "sonic.exe");

                    if (!File.Exists(sonicExePath))
                    {
                        errorMessage = "No sonic.exe found - are you sure this is a real game folder?";
                        return false;
                    }

                    // look for modloader specific files
                    string modLoaderDLLPath = string.Concat(gameFolderPath, "\\mods\\SADXModLoader.dll");
                    if (!File.Exists(modLoaderDLLPath)) // I apologize in advance for the awful code below - love, Josh.
                    {
                        errorMessage = "Error - Modloader not installed.";
                        return false;
                    }

                    string systemFolder = string.Concat(gameFolderPath, "\\system\\");
                    if (!Directory.Exists(systemFolder)) // look for system folder
                    {
                        errorMessage = "Error - no system folder found.";
                        return false;
                    }

                    errorMessage = "none";
                    return true;

                case SA_Tools.Game.SA2:
                    errorMessage = "SA2 DC is not supported!";
                    return false;

                case SA_Tools.Game.SA2B:
                    string GDFolderPath = string.Concat(gameFolderPath, "\\resource\\gd_PC");
                    string SA2ModLoaderDLLPath = Path.Combine(GDFolderPath, "Data_DLL_orig.dll");

                    if(!Directory.Exists(GDFolderPath))
                    {
                        errorMessage = "GD folder doesn't exist! Doesn't appear to be a real game folder.";
                        return false;
                    }

                    if(!File.Exists(SA2ModLoaderDLLPath))
                    {
                        errorMessage = "Modloader not installed!";
                        return false;
                    }

                    errorMessage = "none";
                    return true;
            }

            errorMessage = "Unknown Game Type.";
            return false;
        }
	}
}
