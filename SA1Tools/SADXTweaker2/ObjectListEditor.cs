using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class ObjectListEditor : Form
	{
		public ObjectListEditor()
		{
			InitializeComponent();
		}

		private string ClipboardFormat = typeof(SA1ObjectListEntry).AssemblyQualifiedName;
		private List<KeyValuePair<string, SA1ObjectListEntry[]>> objectLists = new List<KeyValuePair<string, SA1ObjectListEntry[]>>();

		private SA1ObjectListEntry[] CurrentList
		{
			get { return objectLists[levelList.SelectedIndex].Value; }
			set
			{
				objectLists[levelList.SelectedIndex] = new KeyValuePair<string, SA1ObjectListEntry[]>(objectLists[levelList.SelectedIndex].Key, value);
			}
		}

		private SA1ObjectListEntry CurrentItem
		{
			get { return CurrentList[objectList.SelectedIndex]; }
			set { CurrentList[objectList.SelectedIndex] = value; }
		}

		private void ObjectListEditor_Load(object sender, EventArgs e)
		{
			levelList.BeginUpdate();
			foreach (KeyValuePair<string, FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("objlist", StringComparison.OrdinalIgnoreCase))
				{
					objectLists.Add(new KeyValuePair<string, SA1ObjectListEntry[]>(item.Value.Filename, (SA1ObjectListEntry[])ObjectList.Load(item.Value.Filename, false)));
					levelList.Items.Add(item.Key);
				}
			levelList.EndUpdate();
			levelList.SelectedIndex = 0;
		}

		private void ObjectListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					foreach (KeyValuePair<string, SA1ObjectListEntry[]> item in objectLists)
						item.Value.Save(item.Key);
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
			foreach (SA1ObjectListEntry item in CurrentList)
				objectList.Items.Add(i++ + ": " + item.Name);
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
				arg1.Value = CurrentItem.Arg1;
				arg2.Value = CurrentItem.Arg2;
				flags.Value = CurrentItem.Flags;
				distance.Value = (decimal)CurrentItem.Distance;
				codeAddress.Text = CurrentItem.CodeString;
				objectName.Text = CurrentItem.Name;
			}
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			List<SA1ObjectListEntry> currentList = new List<SA1ObjectListEntry>(CurrentList);
			currentList.Add(new SA1ObjectListEntry());
			CurrentList = currentList.ToArray();
			objectList.Items.Add(currentList.Count - 1 + ": ");
			objectList.SelectedIndex = currentList.Count - 1;
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			List<SA1ObjectListEntry> currentList = new List<SA1ObjectListEntry>(CurrentList);
			int i = objectList.SelectedIndex;
			currentList.RemoveAt(i);
			CurrentList = currentList.ToArray();
			ReloadObjectList();
			objectList.SelectedIndex = Math.Min(i, objectList.Items.Count - 1);
		}

		private void arg1_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Arg1 = (byte)arg1.Value;
		}

		private void arg2_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Arg2 = (byte)arg2.Value;
		}

		private void flags_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Flags = (ushort)flags.Value;
		}

		private void distance_ValueChanged(object sender, EventArgs e)
		{
			CurrentItem.Distance = (float)distance.Value;
		}

		private void codeAddress_TextChanged(object sender, EventArgs e)
		{
			CurrentItem.CodeString = codeAddress.Text;
		}

		private void objectName_TextChanged(object sender, EventArgs e)
		{
			CurrentItem.Name = objectName.Text;
			objectList.Items[objectList.SelectedIndex] = objectList.SelectedIndex + ": " + objectName.Text;
		}

		private void copyButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(ClipboardFormat, CurrentItem);
		}

		private void pasteButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData(ClipboardFormat))
			{
				CurrentItem = (SA1ObjectListEntry)Clipboard.GetData(ClipboardFormat);
				ReloadObjectData();
			}
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			if (Program.IniData.MasterObjectList == null)
				MessageBox.Show(this, "No master object list specified in project file.", Text);
			else
				using (ObjectSearchDialog dlg = new ObjectSearchDialog())
					if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
						KeyValuePair<string, MasterObjectListEntry> selobj = dlg.SelectedObject;
						List<SA1ObjectListEntry> currentList = new List<SA1ObjectListEntry>(CurrentList);
						SA1ObjectListEntry obj = new SA1ObjectListEntry();
						obj.Arg1 = selobj.Value.Arg1;
						obj.Arg2 = selobj.Value.Arg2;
						obj.Flags = selobj.Value.Flags;
						obj.Distance = selobj.Value.Distance;
						obj.CodeString = selobj.Key;
						obj.Name = selobj.Value.Name;
						currentList.Add(obj);
						CurrentList = currentList.ToArray();
						objectList.Items.Add(currentList.Count - 1 + ": ");
						objectList.SelectedIndex = currentList.Count - 1;
					}
		}
	}
}