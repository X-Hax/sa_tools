using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    /// <summary>
    /// A simple dialog that allows us to select a project from a project list.
    /// </summary>
    public partial class ProjectSelector : Form
    {
        private string projectFolderPath; // we'll treat any immediate sub-folder of this one to be a selectable project.
        private string selectedProjectName;
        public string SelectedProjectName { get { return selectedProjectName; } set { selectedProjectName = value; } }
        /// <summary>Full path version of the selected project name.</summary>
        public string SelectedProjectPath { get { return string.Concat(projectFolderPath, selectedProjectName, "\\"); } }
        public bool NoProjects { get { return listBox1.Items.Count == 0; } }

        public ProjectSelector(string gameFolder)
        {
            InitializeComponent();

            projectFolderPath = string.Concat(gameFolder, "\\Projects\\");

            if (Directory.Exists(projectFolderPath))
            {
                string[] projectDirNames = Directory.GetDirectories(projectFolderPath);
                foreach (string projectName in projectDirNames) listBox1.Items.Add(new DirectoryInfo(projectName).Name);

                acceptButton.Enabled = false;
            }
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            selectedProjectName = (string)listBox1.SelectedItem;
            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            acceptButton.Enabled = true;
        }

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

                    if (!Directory.Exists(GDFolderPath))
                    {
                        errorMessage = "GD folder doesn't exist! Doesn't appear to be a real game folder.";
                        return false;
                    }

                    if (!File.Exists(SA2ModLoaderDLLPath))
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
