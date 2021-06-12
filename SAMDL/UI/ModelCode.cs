using System;
using System.Windows.Forms;

namespace SAModel.SAMDL
{
	public partial class ModelText : Form
	{
		public string export;

		public ModelText()
		{
			InitializeComponent();
		}

		private void buttonCloseModelText_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void ModelText_Shown(object sender, EventArgs e)
		{
			txtModel.Text = export;
		}
	}
}