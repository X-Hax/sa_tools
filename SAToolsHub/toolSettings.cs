using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;
using SonicRetro.SAModel.SAEditorCommon;

namespace SAToolsHub
{
	public partial class toolSettings : Form
	{
		private string errMessage = ("No games have been configured.\n\n" +
				"You will not be able to create projects until your game paths have been configured.\n\n" +
				"You can return to this window using the Settings drop down.");

		public toolSettings()
		{
			InitializeComponent();

			SetValues();
		}

		void SetValues()
		{
			string sadxPCPath = Program.Settings.SADXPCPath;
			string sa2PCPath = Program.Settings.SA2PCPath;
			string sa1Path = Program.Settings.SA1Path;
			string sa1adPath = Program.Settings.SA1ADPath;
			string sa2Path = Program.Settings.SA2Path;
			string sa2ttPath = Program.Settings.SA2TTPath;
			string sa2pPath = Program.Settings.SA2PPath;
			string sadxgcPath = Program.Settings.SADXGCPath;
			string sadxgcpPath = Program.Settings.SADXGCPPath;
			string sadxgcrPath = Program.Settings.SADXGCRPath;
			string sadx360Path = Program.Settings.SADX360Path;

			txtSADXPC.Text = sadxPCPath;
			txtSA2PC.Text = sa2PCPath;
			txtSA1Final.Text = sa1Path;
			txtSA1Autodemo.Text = sa1adPath;
			txtSA2Final.Text = sa2Path;
			txtSA2Trial.Text = sa2ttPath;
			txtSA2Preview.Text = sa2pPath;
			txtSADXGCFinal.Text = sadxgcPath;
			txtSADXGCPreview.Text = sadxgcpPath;
			txtSADXGCReview.Text = sadxgcrPath;
			txtSADX360.Text = sadx360Path;
		}

		void SaveSettings()
		{
			Program.Settings.SADXPCPath = txtSADXPC.Text;
			Program.Settings.SA2PCPath = txtSA2PC.Text;
			Program.Settings.SA1Path = txtSA1Final.Text;
			Program.Settings.SA1ADPath = txtSA1Autodemo.Text;
			Program.Settings.SA2Path = txtSA2Final.Text;
			Program.Settings.SA2TTPath = txtSA2Trial.Text;
			Program.Settings.SA2PPath = txtSA2Preview.Text;
			Program.Settings.SADXGCPath = txtSADXGCFinal.Text;
			Program.Settings.SADXGCPPath = txtSADXGCPreview.Text;
			Program.Settings.SADXGCRPath = txtSADXGCReview.Text;
			Program.Settings.SADX360Path = txtSADX360.Text;

			Program.Settings.Save();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			SaveSettings();

			// check validity
			string sadxFailReason = "";
			bool sadxpcValid = GamePathChecker.CheckSADXPCValid(
				Program.Settings.SADXPCPath, out sadxFailReason);

			if (!sadxpcValid)
			{
				MessageBox.Show(sadxFailReason, "SADX PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			string sa2PCFailReason = "";
			bool sa2PCValid = GamePathChecker.CheckSA2PCValid(
				Program.Settings.SA2PCPath, out sa2PCFailReason);

			if (!sa2PCValid)
			{
				MessageBox.Show(sa2PCFailReason, "SA2 PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			if (!Program.AnyGamesConfigured())
			{
				DialogResult dialogResult = MessageBox.Show(errMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

				if (dialogResult == DialogResult.OK)
				{
					this.Hide();
					return;
				}
				else
				{
					return;
				}
			}

			// hide and save
			this.Close();
		}
		
		private void button10_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnSADXPC_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSADXPC.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSA2PC_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSA2PC.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSA1Final_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSA1Final.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSA1Autodemo_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSA1Autodemo.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSa2Final_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSA2Final.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSA2Trial_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSA2Trial.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSA2Preview_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSA2Preview.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSADXGCFinal_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSADXGCFinal.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSADXGCPreview_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSADXGCPreview.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSADXGCReview_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSADXGCReview.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSADX360_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtSADX360.Text = folderDialog.SelectedPath;
			}
		}
	}
}
