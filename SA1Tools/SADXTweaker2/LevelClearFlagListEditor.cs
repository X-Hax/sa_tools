using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class LevelClearFlagListEditor : Form
	{
		public LevelClearFlagListEditor()
		{
			InitializeComponent();
		}

		private string ClipboardFormat = typeof(LevelClearFlag).AssemblyQualifiedName;
		private List<KeyValuePair<string, LevelClearFlag[]>> objectLists = new List<KeyValuePair<string, LevelClearFlag[]>>();

		private LevelClearFlag[] CurrentList
		{
			get { return objectLists[levelList.SelectedIndex].Value; }
			set
			{
				objectLists[levelList.SelectedIndex] = new KeyValuePair<string, LevelClearFlag[]>(objectLists[levelList.SelectedIndex].Key, value);
			}
		}

		private LevelClearFlag CurrentItem
		{
			get { return CurrentList[objectList.SelectedIndex]; }
			set { CurrentList[objectList.SelectedIndex] = value; }
		}

		private void LevelClearFlagListEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("levelclearflags", StringComparison.OrdinalIgnoreCase))
				{
					objectLists.Add(new KeyValuePair<string, LevelClearFlag[]>(item.Value.Filename, LevelClearFlagList.Load(item.Value.Filename)));
					levelList.Items.Add(item.Key);
				}
			levelList.EndUpdate();
			levelList.SelectedIndex = 0;
		}

		private void LevelClearFlagListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, LevelClearFlag[]> item in objectLists)
						item.Value.Save(item.Key);
					break;
			}
		}

		private void levelList_SelectedIndexChanged(object sender, EventArgs e) { ReloadLevelClearFlagList(); }

		private void ReloadLevelClearFlagList()
		{
			if (levelList.SelectedIndex == -1) return;
			objectList.Items.Clear();
			objectList.BeginUpdate();
			int i = 0;
			foreach (LevelClearFlag item in CurrentList)
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
				level.SelectedIndex = (int)CurrentItem.Level;
				flag.Value = CurrentItem.Flag;
			}
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			List<LevelClearFlag> currentList = new List<LevelClearFlag>(CurrentList);
			currentList.Add(new LevelClearFlag());
			CurrentList = currentList.ToArray();
			objectList.Items.Add(currentList.Count - 1 + ": " + SA1LevelIDs.HedgehogHammer.ToString());
			objectList.SelectedIndex = currentList.Count - 1;
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			List<LevelClearFlag> currentList = new List<LevelClearFlag>(CurrentList);
			int i = objectList.SelectedIndex;
			currentList.RemoveAt(i);
			CurrentList = currentList.ToArray();
			ReloadLevelClearFlagList();
			objectList.SelectedIndex = Math.Min(i, objectList.Items.Count - 1);
		}

		private void level_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentItem.Level = (SA1LevelIDs)level.SelectedIndex;
			objectList.Items[objectList.SelectedIndex] = objectList.SelectedIndex + ": " + CurrentItem.Level.ToString();
		}

		private void flag_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Flag = (ushort)flag.Value;
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(ClipboardFormat, CurrentItem);
		}

		private void pasteButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData(ClipboardFormat))
			{
				CurrentItem = (LevelClearFlag)Clipboard.GetData(ClipboardFormat);
				ReloadObjectData();
			}
		}
	}
}
