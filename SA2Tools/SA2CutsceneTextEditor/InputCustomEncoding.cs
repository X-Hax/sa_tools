using System;
using System.Windows.Forms;

namespace SA2CutsceneTextEditor
{
	public partial class InputCustomEncoding : Form
	{
		public int CustomCodepage = 1252;

		public InputCustomEncoding()
		{
			InitializeComponent();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			CustomCodepage = (int)numericUpDown1.Value;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
