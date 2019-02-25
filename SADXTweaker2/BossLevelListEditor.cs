using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class BossLevelListEditor : Form
	{
		public BossLevelListEditor()
		{
			InitializeComponent();
		}

		private string ClipboardFormat = typeof(SA1LevelAct).AssemblyQualifiedName;
		private List<KeyValuePair<string, SA1LevelAct[]>> levelLists = new List<KeyValuePair<string, SA1LevelAct[]>>();

		private SA1LevelAct[] CurrentList
		{
			get { return levelLists[levelList.SelectedIndex].Value; }
			set
			{
				levelLists[levelList.SelectedIndex] = new KeyValuePair<string, SA1LevelAct[]>(levelLists[levelList.SelectedIndex].Key, value);
			}
		}

		private SA1LevelAct CurrentItem
		{
			get { return CurrentList[objectList.SelectedIndex]; }
			set { CurrentList[objectList.SelectedIndex] = value; }
		}

		private void BossLevelListEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("bosslevellist", StringComparison.OrdinalIgnoreCase))
				{
					levelLists.Add(new KeyValuePair<string, SA1LevelAct[]>(item.Value.Filename, BossLevelList.Load(item.Value.Filename)));
					levelList.Items.Add(item.Key);
				}
			levelList.EndUpdate();
			levelList.SelectedIndex = 0;
		}

		private void BossLevelListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, SA1LevelAct[]> item in levelLists)
						TrialLevelList.Save(item.Value, item.Key);
					break;
			}
		}

		private void levelList_SelectedIndexChanged(object sender, EventArgs e) { ReloadObjectList(); }

		private void ReloadObjectList()
		{
			if (levelList.SelectedIndex == -1) return;
			objectList.Items.Clear();
			objectList.BeginUpdate();
			int i = 0;
			foreach (SA1LevelAct item in CurrentList)
				objectList.Items.Add(i++ + ": " + item.ToString());
			objectList.EndUpdate();
			ReloadObjectData();
		}

		private void objectList_SelectedIndexChanged(object sender, EventArgs e) { ReloadObjectData(); }

		private void ReloadObjectData()
		{
			if (objectList.SelectedIndex == -1)
				deleteButton.Enabled = level.Enabled = copyButton.Enabled = pasteButton.Enabled = false;
			else
			{
				deleteButton.Enabled = level.Enabled = copyButton.Enabled = pasteButton.Enabled = true;
				level.Value = CurrentItem;
			}
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			List<SA1LevelAct> currentList = new List<SA1LevelAct>(CurrentList);
			currentList.Add(new SA1LevelAct());
			CurrentList = currentList.ToArray();
			objectList.Items.Add(currentList.Count - 1 + ": " + new SA1LevelAct().ToString());
			objectList.SelectedIndex = currentList.Count - 1;
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			List<SA1LevelAct> currentList = new List<SA1LevelAct>(CurrentList);
			int i = objectList.SelectedIndex;
			currentList.RemoveAt(i);
			CurrentList = currentList.ToArray();
			ReloadObjectList();
			objectList.SelectedIndex = Math.Min(i, objectList.Items.Count - 1);
		}

		private void level_ValueChanged(object sender, EventArgs e)
		{
			if (objectList.SelectedIndex == -1) return;
			CurrentItem = level.Value;
			objectList.Items[objectList.SelectedIndex] = objectList.SelectedIndex + ": " + level.Value.ToString();
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(ClipboardFormat, CurrentItem);
		}

		private void pasteButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData(ClipboardFormat))
			{
				CurrentItem = (SA1LevelAct)Clipboard.GetData(ClipboardFormat);
				ReloadObjectData();
			}
		}
	}
}