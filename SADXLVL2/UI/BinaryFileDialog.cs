using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXLVL2
{
	public partial class BinaryFileDialog : Form
	{
		public BinaryFileDialog()
		{
			InitializeComponent();
		}

		private void Dialog1_Load(object sender, EventArgs e)
		{
			comboLevelFormat.SelectedIndex = 1;
		}

		private void CheckBox3_CheckedChanged(object sender, EventArgs e)
		{
			numericAddress.Hexadecimal = checkboxHex.Checked;
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (comboFileKeyHint.SelectedIndex)
			{
				case 0:
					numericKey.Value = 0x00400000u;
					break;
				case 1:
					numericKey.Value = 0x10000000u;
					break;
				case 2:
					numericKey.Value = 0x8C010000u;
					break;
				case 3:
					numericKey.Value = 0x0C900000u;
					comboLevelFormat.SelectedIndex = 0;
					break;
				case 4:
					numericKey.Value = 0x8C500000u;
					comboLevelFormat.SelectedIndex = 2;
					break;
				case 5:
					numericKey.Value = 0u;
					break;
			}
		}

		private void BinaryFileDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.Diagnostics.Process.Start("https://github.com/sonicretro/sa_tools/wiki/Working-with-Binary-Files");
		}
	}
}
