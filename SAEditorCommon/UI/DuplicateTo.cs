using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SA_Tools;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	/// <summary>
	/// The 'Duplicate To' dialog allows the user to clone specified SET Items to any other character's layout. This is useful for quickly porting things from one layout to another.
	/// </summary>
	public partial class DuplicateTo : Form
	{
		List<Item> selectedItems;
		public DuplicateTo(List<Item> selectedItems)
		{
			InitializeComponent();
			this.selectedItems = selectedItems;
		}

		private void DuplicateTo_Load(object sender, EventArgs e)
		{
			#region Setting Checkbox Disables
			// We need to check for invalid states. Specifically, do not let the user clone to the layout they are currently in, or any null layouts
			switch (LevelData.Character)
			{
				case(0):
					sonicCheckBox.Enabled = false;
					sonicCheckBox.Checked = false;
					break;

				case(1):
					tailsCheckBox.Enabled = false;
					tailsCheckBox.Checked = false;
					break;

				case(2):
					knucklesCheckBox.Enabled = false;
					knucklesCheckBox.Checked = false;
					break;

				case(3):
					amyCheckBox.Checked = false;
					amyCheckBox.Enabled = false;
					break;

				case(4):
					gammaCheckBox.Enabled = false;
					gammaCheckBox.Checked = false;
					break;

				case(5):
					bigCheckBox.Enabled = false;
					bigCheckBox.Checked = false;
					break;
			}

			if (LevelData.SETItems[0] == null)
			{
				sonicCheckBox.Enabled = false;
				sonicCheckBox.Checked = false;
			}

			if (LevelData.SETItems[1] == null)
			{
				tailsCheckBox.Enabled = false;
				tailsCheckBox.Checked = false;
			}

			if (LevelData.SETItems[2] == null)
			{
				knucklesCheckBox.Enabled = false;
				knucklesCheckBox.Checked = false;
			}

			if (LevelData.SETItems[3] == null)
			{
				amyCheckBox.Checked = false;
				amyCheckBox.Enabled = false;
			}

			if (LevelData.SETItems[4] == null)
			{
				gammaCheckBox.Enabled = false;
				gammaCheckBox.Checked = false;
			}

			if(LevelData.SETItems[5] == null)
			{
				bigCheckBox.Enabled = false;
				bigCheckBox.Checked = false;
			}
			#endregion
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			foreach (Item item in selectedItems)
			{
				if (item is SETItem)
				{
					SETItem itemConv = (SETItem)item;

					if (sonicCheckBox.Checked)
					{
						LevelData.SETItems[0].Add(new SETItem(itemConv.GetBytes(), 0));
					}

					if (tailsCheckBox.Checked)
					{
						LevelData.SETItems[1].Add(new SETItem(itemConv.GetBytes(), 0));
					}

					if (knucklesCheckBox.Checked)
					{
						LevelData.SETItems[2].Add(new SETItem(itemConv.GetBytes(), 0));
					}

					if (amyCheckBox.Checked)
					{
						LevelData.SETItems[3].Add(new SETItem(itemConv.GetBytes(), 0));
					}

					if (gammaCheckBox.Checked)
					{
						LevelData.SETItems[4].Add(new SETItem(itemConv.GetBytes(), 0));
					}

					if (bigCheckBox.Checked)
					{
						LevelData.SETItems[5].Add(new SETItem(itemConv.GetBytes(), 0));
					}
				}
			}

			this.Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
