using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SADXFontEdit
{
	public partial class FileTypeDialog : Form
	{
		public int charset;
		public bool fontdata;
		public bool custom;
		public string customfile;

		public FileTypeDialog(string filename)
		{
			InitializeComponent();
			if (Path.GetFileName(filename).Substring(0, 8).ToLowerInvariant() == "fontdata")
			{
				radioButton_Fontdata.Checked = true;
				radioButton_Simple.Checked = false;
				groupBox_Codepage.Enabled = true;
			}
			else
			{
				radioButton_Fontdata.Checked = false;
				radioButton_Simple.Checked = true;
				groupBox_Codepage.Enabled = false;
			}
			checkBox_CharMap.Enabled = !groupBox_Codepage.Enabled;
			buttonLoadCharMap.Enabled = !groupBox_Codepage.Enabled;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			fontdata = radioButton_Fontdata.Checked;
			charset = (int)numericUpDown1.Value;
		}

		private void radioButton_Custom_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDown1.Enabled = radioButton_Custom.Checked;
		}

		private void radioButton_1252_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton_1252.Checked) numericUpDown1.Value = 1252;
			else if (radioButton_J2022.Checked) numericUpDown1.Value = 50220;
			else if (radioButton_Unicode.Checked) numericUpDown1.Value = 1200;
		}

		private void radioButton_J2022_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton_1252.Checked) numericUpDown1.Value = 1252;
			else if (radioButton_J2022.Checked) numericUpDown1.Value = 50220;
			else if (radioButton_Unicode.Checked) numericUpDown1.Value = 1200;
		}

		private void radioButton_Unicode_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton_1252.Checked) numericUpDown1.Value = 1252;
			else if (radioButton_J2022.Checked) numericUpDown1.Value = 50220;
			else if (radioButton_Unicode.Checked) numericUpDown1.Value = 1200;
		}

		private void radioButton_Simple_CheckedChanged(object sender, EventArgs e)
		{
			groupBox_Codepage.Enabled = radioButton_Fontdata.Checked;
			checkBox_CharMap.Enabled = !groupBox_Codepage.Enabled;
			buttonLoadCharMap.Enabled = !groupBox_Codepage.Enabled;
		}

		private void buttonLoadCharMap_Click(object sender, EventArgs e)
		{
			OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "txt",
				Filter = "Text files|*.txt|All Files|*.*"
			};
			if (a.ShowDialog() == DialogResult.OK)
			{
				customfile = a.FileName;
			}
		}

		private void checkBox_CharMap_CheckedChanged(object sender, EventArgs e)
		{
			custom = checkBox_CharMap.Checked;
		}
	}
}
