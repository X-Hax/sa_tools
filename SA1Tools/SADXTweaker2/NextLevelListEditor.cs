using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SplitTools;

namespace SADXTweaker2
{
	public partial class NextLevelListEditor : Form
	{
		public NextLevelListEditor()
		{
			InitializeComponent();
		}

		private string ClipboardFormat = typeof(NextLevelListEntry).AssemblyQualifiedName;
		private List<KeyValuePair<string, NextLevelListEntry[]>> objectLists = new List<KeyValuePair<string, NextLevelListEntry[]>>();

		private NextLevelListEntry[] CurrentList
		{
			get { return objectLists[levelList.SelectedIndex].Value; }
			set
			{
				objectLists[levelList.SelectedIndex] = new KeyValuePair<string, NextLevelListEntry[]>(objectLists[levelList.SelectedIndex].Key, value);
			}
		}

		private NextLevelListEntry CurrentItem
		{
			get { return CurrentList[objectList.SelectedIndex]; }
			set { CurrentList[objectList.SelectedIndex] = value; }
		}

		private void NextLevelListEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("nextlevellist", StringComparison.OrdinalIgnoreCase))
				{
					objectLists.Add(new KeyValuePair<string, NextLevelListEntry[]>(item.Value.Filename, NextLevelList.Load(item.Value.Filename)));
					levelList.Items.Add(item.Key);
				}
			levelList.EndUpdate();
			levelList.SelectedIndex = 0;
		}

		private void NextLevelListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, NextLevelListEntry[]> item in objectLists)
						item.Value.Save(item.Key);
					break;
			}
		}

		private void levelList_SelectedIndexChanged(object sender, EventArgs e) { ReloadNextLevelList(); }

		private void ReloadNextLevelList()
		{
			if (levelList.SelectedIndex == -1) return;
			objectList.Items.Clear();
			objectList.BeginUpdate();
			int i = 0;
			foreach (NextLevelListEntry item in CurrentList)
				objectList.Items.Add(i++ + ": " + item.Level.ToString());
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
				cgMovie.Value = CurrentItem.CGMovie;
				level.SelectedIndex = (int)CurrentItem.Level;
				nextLevel.Value = new SA1LevelAct(CurrentItem.NextLevel, CurrentItem.NextAct);
				entrance.Value = CurrentItem.StartPos;
				altNextLevel.Value = new SA1LevelAct(CurrentItem.AltNextLevel, CurrentItem.AltNextAct);
				altEntrance.Value = CurrentItem.AltStartPos;
			}
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			List<NextLevelListEntry> currentList = new List<NextLevelListEntry>(CurrentList);
			currentList.Add(new NextLevelListEntry());
			CurrentList = currentList.ToArray();
			objectList.Items.Add(currentList.Count - 1 + ": " + SA1LevelIDs.HedgehogHammer.ToString());
			objectList.SelectedIndex = currentList.Count - 1;
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			List<NextLevelListEntry> currentList = new List<NextLevelListEntry>(CurrentList);
			int i = objectList.SelectedIndex;
			currentList.RemoveAt(i);
			CurrentList = currentList.ToArray();
			ReloadNextLevelList();
			objectList.SelectedIndex = Math.Min(i, objectList.Items.Count - 1);
		}

		private void cgMovie_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.CGMovie = (byte)cgMovie.Value;
		}

		private void level_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentItem.Level = (SA1LevelIDs)level.SelectedIndex;
			objectList.Items[objectList.SelectedIndex] = objectList.SelectedIndex + ": " + CurrentItem.Level.ToString();
		}

		private void nextLevel_ValueChanged(object sender, EventArgs e)
		{
			if (levelList.SelectedIndex == -1) return;
			CurrentItem.NextLevel = nextLevel.Value.Level;
			CurrentItem.NextAct = nextLevel.Value.Act;
		}

		private void entrance_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.StartPos = (byte)entrance.Value;
		}

		private void altNextLevel_ValueChanged(object sender, EventArgs e)
		{
			if (levelList.SelectedIndex == -1) return;
			CurrentItem.AltNextLevel = altNextLevel.Value.Level;
			CurrentItem.AltNextAct = altNextLevel.Value.Act;
		}

		private void altEntrance_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.AltStartPos = (byte)altEntrance.Value;
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(ClipboardFormat, CurrentItem);
		}

		private void pasteButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData(ClipboardFormat))
			{
				CurrentItem = (NextLevelListEntry)Clipboard.GetData(ClipboardFormat);
				ReloadObjectData();
			}
		}
	}
}