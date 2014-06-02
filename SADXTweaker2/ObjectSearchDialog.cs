using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class ObjectSearchDialog : Form
	{
		public ObjectSearchDialog()
		{
			InitializeComponent();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		Dictionary<string, MasterObjectListEntry> objects;
		List<KeyValuePair<string, MasterObjectListEntry>> listItems = new List<KeyValuePair<string, MasterObjectListEntry>>();

		public KeyValuePair<string, MasterObjectListEntry> SelectedObject { get { return listItems[listView1.SelectedIndices[0]]; } }

		private void ObjectSearchDialog_Load(object sender, EventArgs e)
		{
			objects = IniFile.Deserialize<Dictionary<string, MasterObjectListEntry>>(Program.IniData.MasterObjectList);
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			listItems.Clear();
			if (objects.ContainsKey(textBox1.Text))
				listItems.Add(new KeyValuePair<string, MasterObjectListEntry>(textBox1.Text, objects[textBox1.Text]));
			else
				listItems.AddRange(objects.Where((item) => item.Value.Names.Any((n) => n.Contains(textBox1.Text))));
			listView1.Items.Clear();
			foreach (ListViewItem item in listItems.Select((item) => new ListViewItem(new[] { item.Key, item.Value.Name })))
				listView1.Items.Add(item);
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			okButton.Enabled = listView1.SelectedIndices.Count > 0;
		}
	}
}