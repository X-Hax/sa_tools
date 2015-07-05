using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using SonicRetro.SAModel.SAEditorCommon.UI;

namespace ModGenerator
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

            #region Check Game Paths
            // check our sa2 and sadx paths for validity, if they're no good, ask our user to specify them.
            // Also, give them the option of changing the game path folders. (As in one separate from this).
            string errorMessage = "None supplied";
            DialogResult lookForNewPath = System.Windows.Forms.DialogResult.None;
            if (Properties.Settings.Default.SADXPath == "" || (!ProjectSelector.VerifyGamePath(SA_Tools.Game.SADX, Properties.Settings.Default.SADXPath, out errorMessage)))
            {
                // show an error message that the sadx path is invalid, ask for a new one.
                lookForNewPath = MessageBox.Show(string.Format("The on-record SADX game directory doesn't appear to be valid because: {0}\nOK to supply one, Cancel to ignore.", errorMessage), "Directory Warning", MessageBoxButtons.OKCancel);
                if(lookForNewPath == System.Windows.Forms.DialogResult.OK)
                {
                    if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK) Properties.Settings.Default.SADXPath = folderBrowser.SelectedPath;
                }
            }

            errorMessage = "None Supplied";
            if (Properties.Settings.Default.SA2Path == "" || (!ProjectSelector.VerifyGamePath(SA_Tools.Game.SA2B, Properties.Settings.Default.SA2Path, out errorMessage)))
            {
                // show an error message that the sadx path is invalid, ask for a new one.
                lookForNewPath = MessageBox.Show(string.Format("The on-record SA2PC game directory doesn't appear to be valid because: {0}\nOK to supply one, Cancel to ignore.", errorMessage), "Directory Warning", MessageBoxButtons.OKCancel);
                if (lookForNewPath == System.Windows.Forms.DialogResult.OK)
                {
                    if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK) Properties.Settings.Default.SA2Path = folderBrowser.SelectedPath;
                }
            }

            Properties.Settings.Default.Save();
            #endregion

            // set tool tips
            ToolTip newProjectTip = new ToolTip();
            ToolTip existingProjectTip = new ToolTip();

            newProjectTip.SetToolTip(NewProjectButton, "If you want to start a new project, or split data from sonic.exe and/or the system dll files, use this option.");
            existingProjectTip.SetToolTip(buildExistingButton, "If you've already generated a project using the button above, and would like to get it running in-game, this is what you want.");
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
	}
}
