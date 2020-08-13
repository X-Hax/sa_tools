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
		private SA_Tools.Game game;

		public newProj()
		{
			InitializeComponent();
		}

		private SA_Tools.Game GetGameForRadioButtons()
		{
			if (radSADX.Checked) return SA_Tools.Game.SADX;
			else if (radSA2PC.Checked) return SA_Tools.Game.SA2B;
			else return SA_Tools.Game.SA1;
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

		private void btnCreate_Click(object sender, EventArgs e)
		{

		}

		private void newProj_Load(object sender, EventArgs e)
		{

		}
	}
}
