using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class LevelTextureListEditor : Form
	{
		public LevelTextureListEditor()
		{
			InitializeComponent();
		}

		private string ClipboardFormat = typeof(TextureListEntry).AssemblyQualifiedName;
		private List<KeyValuePair<string, LevelTextureList>> textureLists = new List<KeyValuePair<string, LevelTextureList>>();

		private TextureListEntry[] CurrentList
		{
			get { return textureLists[levelList.SelectedIndex].Value.TextureList; }
			set
			{
				textureLists[levelList.SelectedIndex].Value.TextureList = value;
			}
		}

		private TextureListEntry CurrentItem
		{
			get { return CurrentList[textureList.SelectedIndex]; }
			set { CurrentList[textureList.SelectedIndex] = value; }
		}

		private void LevelTextureListEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("leveltexlist", StringComparison.OrdinalIgnoreCase))
				{
					textureLists.Add(new KeyValuePair<string, LevelTextureList>(item.Value.Filename, LevelTextureList.Load(item.Value.Filename)));
					levelList.Items.Add(item.Key);
				}
			levelList.EndUpdate();
			levelList.SelectedIndex = 0;
			textureName.Directory = Program.IniData.SystemFolder;
		}

		private void LevelTextureListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, LevelTextureList> item in textureLists)
						item.Value.Save(item.Key);
					break;
			}
		}

		private void levelList_SelectedIndexChanged(object sender, EventArgs e) { ReloadTextureList(); }

		private void ReloadTextureList()
		{
			if (levelList.SelectedIndex == -1) return;
			textureList.Items.Clear();
			textureList.BeginUpdate();
			int i = 0;
			foreach (TextureListEntry item in CurrentList)
				textureList.Items.Add(i++ + ": " + item.Name);
			textureList.EndUpdate();
			ReloadTextureData();
		}

		private void textureList_SelectedIndexChanged(object sender, EventArgs e) { ReloadTextureData(); }

		private void ReloadTextureData()
		{
			if (textureList.SelectedIndex == -1)
				deleteButton.Enabled = groupBox1.Enabled = copyButton.Enabled = pasteButton.Enabled = false;
			else
			{
				deleteButton.Enabled = groupBox1.Enabled = copyButton.Enabled = pasteButton.Enabled = true;
				textureName.Value = CurrentItem.Name;
				textureAddress.Value = CurrentItem.Textures;
			}
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			List<TextureListEntry> currentList = new List<TextureListEntry>(CurrentList);
			currentList.Add(new TextureListEntry());
			CurrentList = currentList.ToArray();
			textureList.Items.Add(currentList.Count - 1 + ": ");
			textureList.SelectedIndex = currentList.Count - 1;
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			List<TextureListEntry> currentList = new List<TextureListEntry>(CurrentList);
			int i = textureList.SelectedIndex;
			currentList.RemoveAt(i);
			CurrentList = currentList.ToArray();
			ReloadTextureList();
			textureList.SelectedIndex = Math.Min(i, textureList.Items.Count - 1);
		}

		private void textureName_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Name = textureName.Value;
			textureList.Items[textureList.SelectedIndex] = textureList.SelectedIndex + ": " + textureName.Value;
		}

		private void textureAddress_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Textures = (uint)textureAddress.Value;
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(ClipboardFormat, CurrentItem);
		}

		private void pasteButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData(ClipboardFormat))
			{
				CurrentItem = (TextureListEntry)Clipboard.GetData(ClipboardFormat);
				ReloadTextureData();
			}
		}
	}
}