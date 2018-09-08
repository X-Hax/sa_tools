using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public partial class NewObjectDialog : Form
	{
		private bool IsMission;
		public NewObjectDialog(bool isMission)
		{
			InitializeComponent();
			objLstPanel.Visible = IsMission = isMission;
		}

		private void NewObjectDialog_Load(object sender, EventArgs e)
		{
			var defs = LevelData.ObjDefs;
			if (IsMission)
				defs = LevelData.MisnObjDefs;
			listBox1.BeginUpdate();
			foreach (var item in defs)
				listBox1.Items.Add(item.Name);
			listBox1.EndUpdate();
			listBox1.SelectedIndex = 0;
		}

		private void objLst_CheckedChanged(object sender, EventArgs e)
		{
			var defs = LevelData.ObjDefs;
			if (IsMission && objLstMission.Checked)
				defs = LevelData.MisnObjDefs;
			listBox1.BeginUpdate();
			listBox1.Items.Clear();
			foreach (var item in defs)
				listBox1.Items.Add(item.Name);
			listBox1.EndUpdate();
			listBox1.SelectedIndex = 0;
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
				numericUpDown1.Value = listBox1.SelectedIndex;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDown1.Value < listBox1.Items.Count)
				listBox1.SelectedIndex = (int)numericUpDown1.Value;
			else
				listBox1.SelectedIndex = -1;
		}

		public ushort ID { get { return (ushort)numericUpDown1.Value; } }

		public MsnObjectList ObjectList { get { return objLstLevel.Checked ? MsnObjectList.Level : MsnObjectList.Mission; } }

		private void listBox1_DoubleClick(object sender, EventArgs e)
		{
			okButton.PerformClick();
		}
	}
}
