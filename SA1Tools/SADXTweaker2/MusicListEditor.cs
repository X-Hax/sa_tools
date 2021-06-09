using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class MusicListEditor : Form
	{
		public MusicListEditor()
		{
			InitializeComponent();
		}

		private string ClipboardFormat = typeof(MusicListEntry).AssemblyQualifiedName;
		private MusicListEntry[] musicfiles;
		private string musiclistfile;

		private MusicListEntry CurrentItem
		{
			get { return musicfiles[trackNum.SelectedIndex]; }
			set { musicfiles[trackNum.SelectedIndex] = value; }
		}

		private void MusicListEditor_Load(object sender, EventArgs e)
		{
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("musiclist", StringComparison.OrdinalIgnoreCase))
				{
					musicfiles = MusicList.Load(item.Value.Filename);
					musiclistfile = item.Value.Filename;
					break;
				}
			filename.Directory = Program.IniData.MusicFolder;
			trackNum.SelectedIndex = 0;
		}

		private void MusicListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					musicfiles.Save(musiclistfile);
					break;
			}
		}

		private void trackNum_SelectedIndexChanged(object sender, EventArgs e) { ReloadObjectData(); }

		private void ReloadObjectData()
		{
			if (trackNum.SelectedIndex == -1)
				groupBox1.Enabled = copyButton.Enabled = pasteButton.Enabled = false;
			else
			{
				groupBox1.Enabled = copyButton.Enabled = pasteButton.Enabled = true;
				filename.Value = CurrentItem.Filename;
				loop.Checked = CurrentItem.Loop;
			}
		}

		private void filename_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Filename = filename.Value;
		}

		private void loop_CheckedChanged(object sender, EventArgs e)
		{
			CurrentItem.Loop = loop.Checked;
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(ClipboardFormat, CurrentItem);
		}

		private void pasteButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData(ClipboardFormat))
			{
				CurrentItem = (MusicListEntry)Clipboard.GetData(ClipboardFormat);
				ReloadObjectData();
			}
		}
	}
}