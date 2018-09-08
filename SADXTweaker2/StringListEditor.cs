using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class StringListEditor : Form
	{
		public StringListEditor()
		{
			InitializeComponent();
		}

		private List<KeyValuePair<string, string[]>> objectLists = new List<KeyValuePair<string, string[]>>();

		private string[] CurrentList
		{
			get { return objectLists[levelList.SelectedIndex].Value; }
			set
			{
				objectLists[levelList.SelectedIndex] = new KeyValuePair<string, string[]>(objectLists[levelList.SelectedIndex].Key, value);
			}
		}

		private string CurrentItem
		{
			get { return CurrentList[objectList.SelectedIndex]; }
			set { CurrentList[objectList.SelectedIndex] = value; }
		}

		private void StringListEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("stringarray", StringComparison.OrdinalIgnoreCase))
				{
					objectLists.Add(new KeyValuePair<string, string[]>(item.Value.Filename, StringArray.Load(item.Value.Filename)));
					levelList.Items.Add(item.Key);
				}
			levelList.EndUpdate();
			levelList.SelectedIndex = 0;
		}

		private void StringListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, string[]> item in objectLists)
						item.Value.Save(item.Key);
					break;
			}
		}

		private void levelList_SelectedIndexChanged(object sender, EventArgs e) { ReloadStringArray(); }

		private void ReloadStringArray()
		{
			if (levelList.SelectedIndex == -1) return;
			objectList.Items.Clear();
			objectList.BeginUpdate();
			int i = 0;
			foreach (string item in CurrentList)
				objectList.Items.Add(i++ + ": " + item.Split('\n')[0]);
			objectList.EndUpdate();
			ReloadObjectData();
		}

		private void objectList_SelectedIndexChanged(object sender, EventArgs e) { ReloadObjectData(); }

		private void ReloadObjectData()
		{
			if (objectList.SelectedIndex == -1)
				richTextBox1.Enabled = false;
			else
			{
				richTextBox1.Enabled = true;
				richTextBox1.Text = CurrentItem.Replace("\n", Environment.NewLine);
			}
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			CurrentItem = richTextBox1.Text.Replace(Environment.NewLine, "\n");
			objectList.Items[objectList.SelectedIndex] = objectList.SelectedIndex + ": " + CurrentItem.Split('\n')[0];
		}
	}
}