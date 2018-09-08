using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAMDL
{
	public partial class FindDialog : Form
	{
		public FindDialog()
		{
			InitializeComponent();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			okButton.Enabled = textBox1.Text.Length > 0;
		}

		public string SearchText { get { return textBox1.Text; } }
	}
}
