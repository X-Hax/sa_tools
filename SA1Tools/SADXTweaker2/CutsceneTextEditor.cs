using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SplitTools;

namespace SADXTweaker2
{
	public partial class CutsceneTextEditor : Form
	{
		public CutsceneTextEditor()
		{
			InitializeComponent();
		}

		private List<KeyValuePair<string, CutsceneText>> objectLists = new List<KeyValuePair<string, CutsceneText>>();

		private string[] CurrentList
		{
			get { return objectLists[levelList.SelectedIndex].Value.Text[language.SelectedIndex]; }
			set
			{
				objectLists[levelList.SelectedIndex].Value.Text[language.SelectedIndex] = value;
			}
		}

		private string CurrentItem
		{
			get { return CurrentList[objectList.SelectedIndex]; }
			set { CurrentList[objectList.SelectedIndex] = value; }
		}

		private void CutsceneTextEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, SplitTools.FileInfo> item in Program.IniData.SelectMany(a => a.Files).Where(b => b.Value.Type.Equals("cutscenetext", StringComparison.OrdinalIgnoreCase)))
			{
				string path = Path.Combine(Program.project.GameInfo.ProjectFolder, item.Value.Filename);
				objectLists.Add(new KeyValuePair<string, CutsceneText>(path, new CutsceneText(path)));
				levelList.Items.Add(item.Key);
			}
			levelList.EndUpdate();
			language.SelectedIndex = 1;
			levelList.SelectedIndex = 0;
		}

		private void CutsceneTextEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, CutsceneText> item in objectLists)
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
			int i = 0;
			foreach (string item in CurrentList)
				objectList.Items.Add(i++ + ": " + item.Split('\n')[0]);
			objectList.EndUpdate();
			ReloadObjectData();
		}

		bool noreload = false;
		private void objectList_SelectedIndexChanged(object sender, EventArgs e) { if (!noreload) ReloadObjectData(); }

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
			noreload = true;
			objectList.Items[objectList.SelectedIndex] = objectList.SelectedIndex + ": " + CurrentItem.Split('\n')[0];
			noreload = false;
		}
	}
}