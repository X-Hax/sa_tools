using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SALVL
{
	public partial class StructExportDialog : Form
	{
		public StructExportDialog()
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

		public LandTableFormat Format
		{
			get { return (LandTableFormat)comboBox1.SelectedIndex; }
			set { comboBox1.SelectedIndex = (int)value; }
		}
	}
}
