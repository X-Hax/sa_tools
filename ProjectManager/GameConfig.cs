using System;
using System.Windows.Forms;

using SonicRetro.SAModel.SAEditorCommon;

namespace ProjectManager
{
	public partial class GameConfig : Form
	{
		public GameConfig()
		{
			InitializeComponent();

			// try loading our settings
			SetValues();
		}

		void SetValues()
		{
			string sadxPCPath = Program.Settings.SADXPCPath;
			string sa2PCPath = Program.Settings.SA2PCPath;

			SADXPath.Text = sadxPCPath;
			SA2PCPath.Text = sa2PCPath;
		}

		void SaveSettings()
		{
			Program.Settings.SADXPCPath = SADXPath.Text;
			Program.Settings.SA2PCPath = SA2PCPath.Text;

			Program.Settings.Save();
		}

		private void AcceptButton_Click(object sender, EventArgs e)
		{
			SaveSettings();

			// check validity
			string sadxFailReason = "";
			bool sadxpcValid = GamePathChecker.CheckSADXPCValid(
				Program.Settings.SADXPCPath, out sadxFailReason);

			if(!sadxpcValid)
			{
				MessageBox.Show(sadxFailReason, "SADX PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			string sa2PCFailReason = "";
			bool sa2PCValid = GamePathChecker.CheckSA2PCValid(
				Program.Settings.SA2PCPath, out sa2PCFailReason);

			if(!sa2PCValid)
			{
				MessageBox.Show(sa2PCFailReason, "SA2 PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			if(!Program.AnyGamesConfigured())
			{
				DialogResult dialogResult = MessageBox.Show("No games configured", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

				if (dialogResult == DialogResult.Cancel)
				{
					this.DialogResult = DialogResult.Abort;
					Application.Exit();
					return;
				}
				else
				{
					return;
				}
			}

			// hide and save
			this.Hide();
		}

		private void GameConfig_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.DialogResult == DialogResult.Abort) return;
			e.Cancel = true;
			this.Hide();
		}

		private void SADXBrowseButton_Click(object sender, EventArgs e)
		{
			DialogResult browserResult = folderBrowserDialog1.ShowDialog();

			if(browserResult == DialogResult.OK)
			{
				SADXPath.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void SA2PCBrowse_Click(object sender, EventArgs e)
		{
			DialogResult browserResult = folderBrowserDialog1.ShowDialog();

			if (browserResult == DialogResult.OK)
			{
				SA2PCPath.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void GameConfig_VisibleChanged(object sender, EventArgs e)
		{
			if(Visible)
			{
				SetValues();
			}
		}
	}
}
