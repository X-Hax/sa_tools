using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class RecapScreenEditor : Form
	{
		public RecapScreenEditor()
		{
			InitializeComponent();
		}

		private List<KeyValuePair<string, RecapScreen[][]>> objectLists = new List<KeyValuePair<string, RecapScreen[][]>>();

		private RecapScreen[][] CurrentList
		{
			get { return objectLists[levelList.SelectedIndex].Value; }
		}

		private RecapScreen CurrentItem
		{
			get { return CurrentList[objectList.SelectedIndex][language.SelectedIndex]; }
			set { CurrentList[objectList.SelectedIndex][language.SelectedIndex] = value; }
		}

		private void RecapScreenEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("recapscreen", StringComparison.OrdinalIgnoreCase))
				{
					objectLists.Add(new KeyValuePair<string, RecapScreen[][]>(item.Value.Filename, RecapScreenList.Load(item.Value.Filename, int.Parse(item.Value.CustomProperties["length"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo))));
					levelList.Items.Add(item.Key);
				}
			levelList.EndUpdate();
			language.SelectedIndex = 1;
			levelList.SelectedIndex = 0;
		}

		private void RecapScreenEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, RecapScreen[][]> item in objectLists)
						item.Value.Save(item.Key);
					break;
			}
		}

		private void levelList_SelectedIndexChanged(object sender, EventArgs e) { ReloadStringArray(); }

		private void language_SelectedIndexChanged(object sender, EventArgs e) { ReloadStringArray(); }

		private void ReloadStringArray()
		{
			if (levelList.SelectedIndex == -1) return;
			objectList.Items.Clear();
			objectList.BeginUpdate();
			int i = 1;
			foreach (RecapScreen[] item in CurrentList)
				objectList.Items.Add(i++);
			objectList.EndUpdate();
			ReloadObjectData();
		}

		private void objectList_SelectedIndexChanged(object sender, EventArgs e) { ReloadObjectData(); }

		private void ReloadObjectData()
		{
			if (objectList.SelectedIndex == -1)
				panel1.Enabled = false;
			else
			{
				panel1.Enabled = true;
				numericUpDown1.Value = (decimal)CurrentItem.Speed;
				richTextBox1.Text = CurrentItem.Text.Replace("\n", Environment.NewLine);
			}
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			CurrentItem.Text = richTextBox1.Text.Replace(Environment.NewLine, "\n");
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Speed = (float)numericUpDown1.Value;
		}
	}
}