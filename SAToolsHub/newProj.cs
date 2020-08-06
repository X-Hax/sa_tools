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
	public partial class newProj : Form
	{
		public newProj()
		{
			InitializeComponent();

			setProjFolder();
		}

		void setProjFolder()
		{
			string projFolder = Program.Settings.projectPath;

			txtProjFolder.Text = projFolder;
		}

		private void newProj_Shown(object sender, EventArgs e)
		{
			txtAuthor.Text = String.Empty;
			txtDesc.Text = String.Empty;
			txtName.Text = String.Empty;
			txtProjFolder.Text = String.Empty;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtProjFolder.Text = folderDialog.SelectedPath;
			}
		}
	}
}
