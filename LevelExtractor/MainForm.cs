using System;
using System.IO;
using System.Windows.Forms;

namespace SonicRetro.SAModel.LevelExtractor
{
	public partial class MainForm : Form
	{
		Properties.Settings Settings = Properties.Settings.Default;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			comboBox2.SelectedIndex = 1;
			author.Text = Settings.Author;
		}

		private void CheckBox3_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown1.Hexadecimal = CheckBox3.Checked;
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (ComboBox1.SelectedIndex)
			{
				case 0:
					numericUpDown2.Value = 0x00400000u;
					break;
				case 1:
					numericUpDown2.Value = 0x10000000u;
					break;
				case 2:
					numericUpDown2.Value = 0x8C010000u;
					break;
				case 3:
					numericUpDown2.Value = 0x0C900000u;
					break;
				case 4:
					numericUpDown2.Value = 0x8C500000u;
					break;
				case 5:
					numericUpDown2.Value = 0u;
					break;
			}
		}

		private void fileSelector1_FileNameChanged(object sender, EventArgs e)
		{
			button1.Enabled = true;
			file = File.ReadAllBytes(fileSelector1.FileName);
		}

		enum SectOffs
		{
			VSize = 8,
			VAddr = 0xC,
			FSize = 0x10,
			FAddr = 0x14,
			Flags = 0x24,
			Size = 0x28
		}

		byte[] file;
		private void button1_Click(object sender, EventArgs e)
		{
			LandTableFormat format = (LandTableFormat)comboBox2.SelectedIndex;
			LandTableFormat outfmt = format;
			if (format == LandTableFormat.SADX) outfmt = LandTableFormat.SA1;
			ByteConverter.BigEndian = checkBox1.Checked;
			using (SaveFileDialog sd = new SaveFileDialog() { DefaultExt = outfmt.ToString().ToLowerInvariant() + "lvl", Filter = outfmt.ToString().ToUpperInvariant() + "LVL Files|*." + outfmt.ToString().ToLowerInvariant() + "lvl|All Files|*.*" })
				if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					new LandTable(file, (int)NumericUpDown1.Value, (uint)numericUpDown2.Value, format) { Author = author.Text, Description = description.Text }.SaveToFile(sd.FileName, outfmt);
					Settings.Author = author.Text;
					Settings.Save();
				}
		}
	}
}