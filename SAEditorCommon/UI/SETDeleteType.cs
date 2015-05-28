using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public partial class SETDeleteType : Form
	{
		private ushort deleteType = 0;

		public SETDeleteType()
		{
			InitializeComponent();

			foreach (ObjectDefinition item in LevelData.ObjDefs)
				delTypeDropDown.Items.Add(item.Name);

			delTypeDropDown.SelectedIndex = 0;
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			deleteType = (ushort)delTypeDropDown.SelectedIndex;

			for (int itemIndx = 0; itemIndx < LevelData.SETItems[LevelData.Character].Count; itemIndx++)
			{
				List<SETItem> removeItems = new List<SETItem>();
				if (LevelData.SETItems[LevelData.Character][itemIndx].ID == deleteType)
				{
					removeItems.Add(LevelData.SETItems[LevelData.Character][itemIndx]);
				}

				foreach (SETItem item in removeItems)
				{
					LevelData.SETItems[LevelData.Character].Remove(item);
				}
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
