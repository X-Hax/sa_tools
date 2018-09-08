using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public partial class ModelLibraryWindow : Form
	{
		public ModelLibraryWindow()
		{
			InitializeComponent();
		}

		private void ModelLibraryWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}

		private void ModelLibraryWindow_Shown(object sender, EventArgs e)
		{
			modelLibraryControl1.FullReRender();
		}
	}
}
