using System;
using System.Linq;
using System.Windows.Forms;
using static ArchiveLib.GenericArchive;

namespace SADXsndSharp
{
	public partial class OpenFromOffset : Form
	{
		public int outOffset = 0;
		public ArchiveFileType outFileType;

		public OpenFromOffset()
		{
			InitializeComponent();
			foreach (var item in SupportedArchiveFiles)
				comboBox1.Items.Add(item.Value.Item2);
			comboBox1.SelectedIndex = 0;
			outFileType = SupportedArchiveFiles.ElementAt(0).Key;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownOffset.Value < 0)
				numericUpDownOffset.Value = unchecked((uint)int.Parse(numericUpDownOffset.Value.ToString()));
			outOffset = (int)numericUpDownOffset.Value;
		}

		private void checkBoxHex_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownOffset.Hexadecimal = checkBoxHex.Checked;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			outFileType = SupportedArchiveFiles.ElementAt(comboBox1.SelectedIndex).Key;
		}
	}
}