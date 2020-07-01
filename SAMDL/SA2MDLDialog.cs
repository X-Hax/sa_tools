using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAMDL
{
	public partial class SA2MDLDialog : Form
	{
		public SA2MDLDialog()
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

		private void modelChoice_SelectedIndexChanged(object sender, EventArgs e)
		{
			okButton.Enabled = true;
		}

		private void SA2MDLDialog_Load(object sender, EventArgs e)
		{
			okButton.Enabled = false;
		}
	}
}
