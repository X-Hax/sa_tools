using System;
using System.Windows.Forms;

namespace SplitMDL
{
	public partial class SplitMDLGUI : Form
	{
		public SplitMDLGUI()
		{
			InitializeComponent();
		}

		private void fileBrowseButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog())
			{
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					filePathTextBox.Text = fileDialog.FileName;
				}
			}
		}

		private void animFilesBrowse_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog() { Multiselect = true })
			{
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					listBox1.BeginUpdate();

					foreach (string path in fileDialog.FileNames)
					{
						listBox1.Items.Add(path);
					}

					listBox1.EndUpdate();
				}
			}
		}

		private void animFilesClear_Click(object sender, EventArgs e)
		{
			listBox1.Items.Clear();
		}

		private void outputFolderBrowseButton_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
			{
				if (folderBrowser.ShowDialog() == DialogResult.OK)
				{
					outputFolderPath.Text = folderBrowser.SelectedPath;
				}
			}
		}

		private void splitButton_Click(object sender, EventArgs e)
		{
			string[] animationFiles = new string[listBox1.Items.Count];

			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				animationFiles[i] = listBox1.Items[i].ToString();
			}

			SplitTools.SAArc.SA2MDL.Split(filePathTextBox.Text, outputFolderPath.Text,
				animationFiles);
		}
	}
}
