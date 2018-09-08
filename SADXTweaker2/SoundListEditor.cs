using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class SoundListEditor : Form
	{
		public SoundListEditor()
		{
			InitializeComponent();
		}

		private string ClipboardFormat = typeof(SoundListEntry).AssemblyQualifiedName;
		private List<KeyValuePair<string, SoundListEntry[]>> soundLists = new List<KeyValuePair<string, SoundListEntry[]>>();

		private SoundListEntry[] CurrentList
		{
			get { return soundLists[levelList.SelectedIndex].Value; }
			set
			{
				soundLists[levelList.SelectedIndex] = new KeyValuePair<string, SoundListEntry[]>(soundLists[levelList.SelectedIndex].Key, value);
			}
		}

		private SoundListEntry CurrentItem
		{
			get { return CurrentList[soundList.SelectedIndex]; }
			set { CurrentList[soundList.SelectedIndex] = value; }
		}

		private void SoundListEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("soundlist", StringComparison.OrdinalIgnoreCase))
				{
					soundLists.Add(new KeyValuePair<string, SoundListEntry[]>(item.Value.Filename, SoundList.Load(item.Value.Filename)));
					levelList.Items.Add(item.Key);
				}
			levelList.EndUpdate();
			levelList.SelectedIndex = 0;
			soundName.Directory = Program.IniData.SoundFolder;
		}

		private void SoundListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, SoundListEntry[]> item in soundLists)
						item.Value.Save(item.Key);
					break;
			}
		}

		private void levelList_SelectedIndexChanged(object sender, EventArgs e) { ReloadSoundList(); }

		private void ReloadSoundList()
		{
			if (levelList.SelectedIndex == -1) return;
			soundList.Items.Clear();
			soundList.BeginUpdate();
			int i = 0;
			foreach (SoundListEntry item in CurrentList)
				soundList.Items.Add(i++ + ": " + item.Filename);
			soundList.EndUpdate();
			ReloadTextureData();
		}

		private void soundList_SelectedIndexChanged(object sender, EventArgs e) { ReloadTextureData(); }

		private void ReloadTextureData()
		{
			if (soundList.SelectedIndex == -1)
				deleteButton.Enabled = groupBox1.Enabled = copyButton.Enabled = pasteButton.Enabled = false;
			else
			{
				deleteButton.Enabled = groupBox1.Enabled = copyButton.Enabled = pasteButton.Enabled = true;
				soundBank.Value = CurrentItem.Bank;
				soundName.Value = CurrentItem.Filename;
			}
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			List<SoundListEntry> currentList = new List<SoundListEntry>(CurrentList);
			currentList.Add(new SoundListEntry());
			CurrentList = currentList.ToArray();
			soundList.Items.Add(currentList.Count - 1 + ": ");
			soundList.SelectedIndex = currentList.Count - 1;
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			List<SoundListEntry> currentList = new List<SoundListEntry>(CurrentList);
			int i = soundList.SelectedIndex;
			currentList.RemoveAt(i);
			CurrentList = currentList.ToArray();
			ReloadSoundList();
			soundList.SelectedIndex = Math.Min(i, soundList.Items.Count - 1);
		}

		private void soundBank_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Bank = (int)soundBank.Value;
		}

		private void soundName_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Filename = soundName.Value;
			soundList.Items[soundList.SelectedIndex] = soundList.SelectedIndex + ": " + soundName.Value;
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(ClipboardFormat, CurrentItem);
		}

		private void pasteButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData(ClipboardFormat))
			{
				CurrentItem = (SoundListEntry)Clipboard.GetData(ClipboardFormat);
				ReloadTextureData();
			}
		}
	}
}