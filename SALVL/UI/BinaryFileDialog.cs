using System;
using System.Windows.Forms;

namespace SAModel.SALVL
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

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboFileKeyHint.SelectedIndex)
            {
                case 0: // EXE
                    numericKey.Value = 0x400000;
                    break;
                case 1: // EXE (X360)
					numericKey.Value = 0x82000000;
					checkBoxBigEndian.Checked = true;
					comboLevelFormat.SelectedIndex = 1;
					numericStartOffset.Value = 0xC800;
					break;
				case 2: // DLL
					numericKey.Value = 0x10000000;
                    break;
				case 3: // REL
					numericKey.Value = 0xC900000;
					checkBoxBigEndian.Checked = true;
					if (comboLevelFormat.SelectedIndex == 0)
						checkBoxReverse.Checked = true;
					break;
				case 4: // 1ST_READ
                    numericKey.Value = 0x8C010000;
                    break;
                case 5: // SA1 Level
                    numericKey.Value = 0xC900000;
                    comboLevelFormat.SelectedIndex = 0;
                    break;
                case 6: // SA2 Level
                    numericKey.Value = 0x8C500000;
                    comboLevelFormat.SelectedIndex = 2;
                    break; // Unknown
                case 7:
                    numericKey.Value = 0;
                    break;
            }
        }

        private void BinaryFileDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/wiki/Working-with-Binary-Files") { CreateNoWindow = true });
        }

		private void numericKey_ValueChanged_1(object sender, EventArgs e)
		{
			if (numericKey.Value < 0)
				numericKey.Value = unchecked((uint)int.Parse(numericKey.Value.ToString()));
		}

		private void numericAddress_ValueChanged_1(object sender, EventArgs e)
		{
			if (numericAddress.Value < 0)
				numericAddress.Value = unchecked((uint)int.Parse(numericAddress.Value.ToString()));
		}

		private void numericStartOffset_ValueChanged(object sender, EventArgs e)
		{
			if (numericStartOffset.Value < 0)
				numericStartOffset.Value = unchecked((uint)int.Parse(numericStartOffset.Value.ToString()));
		}

		private void checkBoxHexStartOffset_CheckedChanged(object sender, EventArgs e)
		{
			numericStartOffset.Hexadecimal = checkBoxHexStartOffset.Checked;
		}

		private void CheckBox3_CheckedChanged(object sender, EventArgs e)
		{
			numericAddress.Hexadecimal = checkBoxHexLevel.Checked;
		}
	}
}