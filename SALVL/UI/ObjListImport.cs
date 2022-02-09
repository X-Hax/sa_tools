using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using SplitTools;

namespace SAModel.SALVL
{
	public partial class ObjListImport : Form
	{
		static IniDataSALVL salvl;
		static Dictionary<string, IniLevelData> levels;
		static List<string> keyList = new List<string>();
		static string folder;
		public ObjectListEntry impObjListItem;
		public ObjectData impObjDefsItem;
		static List<ObjectListEntry> objList;
		static Dictionary<string, ObjectData> objDefs;

		public ObjListImport(IniDataSALVL file, string modfolder)
		{
			InitializeComponent();
			salvl = file;
			levels = file.Levels;
			folder = modfolder;

			PopulateComboBox();
		}

		private void PopulateComboBox()
		{
			lstLevelSelect.BeginUpdate();
			foreach (KeyValuePair<string, IniLevelData> lvl in levels)
			{
				if (lvl.Value.ObjectList != null || lvl.Value.ObjectList != "")
				{
					keyList.Add(lvl.Key);
					string name = lvl.Key.Split('\\')[1];
					lstLevelSelect.Items.Add(name);
				}
			}
			lstLevelSelect.EndUpdate();
		}

		private void PopulateListBox()
		{

			listBox1.BeginUpdate();
			listBox1.Items.Clear();
			string objListPath = Path.Combine(folder, levels[keyList[lstLevelSelect.SelectedIndex]].ObjectList);
			string objDefPath = Path.Combine(folder, levels[keyList[lstLevelSelect.SelectedIndex]].ObjectDefinition);
			objList = new List<ObjectListEntry>(ObjectList.Load(objListPath, salvl.IsSA2));
			objDefs = IniSerializer.Deserialize<Dictionary<string, ObjectData>>(objDefPath);

			foreach (ObjectListEntry obj in objList)
			{
				string key = obj.Name;
				string val;

				if (objDefs[obj.Name].Name != null)
					val = objDefs[obj.Name].Name;
				else
					val = obj.Name;

				KeyValuePair<string, string> entry = new KeyValuePair<string, string>(key, val);
				listBox1.Items.Add(entry);
				listBox1.DisplayMember = "Value";
			}
			listBox1.EndUpdate();
		}

		private void lstLevelSelect_SelectedIndexChanged(object sender, EventArgs e)
		{
			PopulateListBox();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			impObjListItem = objList[listBox1.SelectedIndex];
			impObjDefsItem = objDefs[((KeyValuePair<string, string>)listBox1.SelectedItem).Key];
			this.Close();
		}

		private void listBox1_DoubleClick(object sender, EventArgs e)
		{
			btnOK_Click(sender, e);
		}
	}
}
