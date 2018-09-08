using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class SoundTestListEditor : Form
	{
		public SoundTestListEditor()
		{
			InitializeComponent();
		}

		private string ClipboardFormat = typeof(SoundTestListEntry).AssemblyQualifiedName;
		private List<KeyValuePair<string, SoundTestListEntry[]>> objectLists = new List<KeyValuePair<string, SoundTestListEntry[]>>();
		private MusicListEntry[] musicfiles;

		private SoundTestListEntry[] CurrentList
		{
			get { return objectLists[levelList.SelectedIndex].Value; }
			set
			{
				objectLists[levelList.SelectedIndex] = new KeyValuePair<string, SoundTestListEntry[]>(objectLists[levelList.SelectedIndex].Key, value);
			}
		}

		private SoundTestListEntry CurrentItem
		{
			get { return CurrentList[objectList.SelectedIndex]; }
			set { CurrentList[objectList.SelectedIndex] = value; }
		}

		private void SoundTestListEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("soundtestlist", StringComparison.OrdinalIgnoreCase))
				{
					objectLists.Add(new KeyValuePair<string, SoundTestListEntry[]>(item.Value.Filename, SoundTestList.Load(item.Value.Filename)));
					levelList.Items.Add(item.Key);
				}
				else if (item.Value.Type.Equals("musiclist", StringComparison.OrdinalIgnoreCase) & musicfiles == null)
				{
					musicfiles = MusicList.Load(item.Value.Filename);
				}
			levelList.EndUpdate();
			levelList.SelectedIndex = 0;
		}

		private void SoundTestListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, SoundTestListEntry[]> item in objectLists)
						item.Value.Save(item.Key);
					break;
			}
		}

		private void levelList_SelectedIndexChanged(object sender, EventArgs e) { ReloadSoundTestList(); }

		private void ReloadSoundTestList()
		{
			if (levelList.SelectedIndex == -1) return;
			objectList.Items.Clear();
			objectList.BeginUpdate();
			int i = 0;
			foreach (SoundTestListEntry item in CurrentList)
				objectList.Items.Add(i++ + ": " + item.Title);
			objectList.EndUpdate();
			ReloadObjectData();
		}

		private void objectList_SelectedIndexChanged(object sender, EventArgs e) { ReloadObjectData(); }

		private void ReloadObjectData()
		{
			if (objectList.SelectedIndex == -1)
				deleteButton.Enabled = groupBox1.Enabled = copyButton.Enabled = pasteButton.Enabled = false;
			else
			{
				deleteButton.Enabled = groupBox1.Enabled = copyButton.Enabled = pasteButton.Enabled = true;
				trackName.Text = CurrentItem.Title;
				trackNum.SelectedIndex = CurrentItem.Track;
			}
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			List<SoundTestListEntry> currentList = new List<SoundTestListEntry>(CurrentList);
			currentList.Add(new SoundTestListEntry());
			CurrentList = currentList.ToArray();
			objectList.Items.Add(currentList.Count - 1 + ": ");
			objectList.SelectedIndex = currentList.Count - 1;
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			List<SoundTestListEntry> currentList = new List<SoundTestListEntry>(CurrentList);
			int i = objectList.SelectedIndex;
			currentList.RemoveAt(i);
			CurrentList = currentList.ToArray();
			ReloadSoundTestList();
			objectList.SelectedIndex = Math.Min(i, objectList.Items.Count - 1);
		}

		private void trackName_TextChanged(object sender, EventArgs e)
		{
			CurrentItem.Title = trackName.Text;
			objectList.Items[objectList.SelectedIndex] = objectList.SelectedIndex + ": " + trackName.Text;
		}

		private void trackNum_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Track = trackNum.SelectedIndex;
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(ClipboardFormat, CurrentItem);
		}

		private void pasteButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData(ClipboardFormat))
			{
				CurrentItem = (SoundTestListEntry)Clipboard.GetData(ClipboardFormat);
				ReloadObjectData();
			}
		}
	}
}