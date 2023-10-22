using System;
using System.Windows.Forms;

namespace SAModel.SAMDL
{
	public partial class PickModelDialog : Form
	{
		public PickModelDialog()
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
