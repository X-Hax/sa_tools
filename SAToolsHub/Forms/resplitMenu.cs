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
	public partial class resplitMenu : Form
	{
		SAModel.SAEditorCommon.UI.ProgressDialog splitProgress;
		bool overwrite = false;
		List<Templates.SplitEntry> splitEntries = new List<Templates.SplitEntry>();
		List<Templates.SplitEntryMDL> splitMDLEntries = new List<Templates.SplitEntryMDL>();

		public resplitMenu()
		{
			InitializeComponent();
		}

		#region General Functions
		#endregion

		#region Form Functions
		private void resplitMenu_Shown(object sender, EventArgs e)
		{
			Templates.SplitTemplate template = ProjectFunctions.openTemplateFile(SAToolsHub.GetTemplate());
			splitEntries = template.SplitEntries;
			splitMDLEntries = template.SplitMDLEntries;
			checkedListBox1.Items.Clear();

			foreach (Templates.SplitEntry splitEntry in splitEntries)
			{
				checkedListBox1.Items.Add(splitEntry);
				if (splitEntry.CmnName != null)
					checkedListBox1.DisplayMember = "CmnName";
				else
					checkedListBox1.DisplayMember = "IniFile";
			}

			foreach (Templates.SplitEntryMDL mdlEntry in splitMDLEntries)
			{
				checkedListBox1.Items.Add(mdlEntry);
				string mdlFile = Path.GetFileNameWithoutExtension(mdlEntry.ModelFile);
				checkedListBox1.DisplayMember = mdlFile;
			}
		}

		private void chkAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBox1.Items.Count; i++)
			{
				checkedListBox1.SetItemChecked(i, true);
			}
		}

		private void unchkAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBox1.Items.Count; i++)
			{
				checkedListBox1.SetItemChecked(i, false);
			}
		}

		private void btnSplit_Click(object sender, EventArgs e)
		{

		}

		private void chkOverwrite_CheckedChanged(object sender, EventArgs e)
		{
			if (chkOverwrite.Checked)
				overwrite = true;
			else if (!chkOverwrite.Checked)
				overwrite = false;
		}
		#endregion

		#region Background Worker
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			using (splitProgress = new SAModel.SAEditorCommon.UI.ProgressDialog("Creating project"))
			{
				Invoke((Action)splitProgress.Show);

				splitGame(gameName, splitProgress);

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
				DialogResult successDiag = MessageBox.Show("Project successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
				if (successDiag == DialogResult.OK)
				{
					SAToolsHub.newProjFile = Path.Combine(projFolder, projName);
					this.Close();
				}
			}
		}
		#endregion
	}
}
