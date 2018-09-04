using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	/// <summary>
	/// The 'Duplicate To' dialog allows the user to clone specified SET Items to any other character's layout. This is useful for quickly porting things from one layout to another.
	/// </summary>
	public partial class DuplicateTo : Form
	{
		EditorItemSelection items;
		public DuplicateTo(EditorItemSelection items)
		{
			InitializeComponent();
			this.items = items;
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

			if (!LevelData.CharHasSETItems(0))
			{
				sonicCheckBox.Enabled = false;
				sonicCheckBox.Checked = false;
			}

			if (!LevelData.CharHasSETItems(1))
			{
				tailsCheckBox.Enabled = false;
				tailsCheckBox.Checked = false;
			}

			if (!LevelData.CharHasSETItems(2))
			{
				knucklesCheckBox.Enabled = false;
				knucklesCheckBox.Checked = false;
			}

			if (!LevelData.CharHasSETItems(3))
			{
				amyCheckBox.Checked = false;
				amyCheckBox.Enabled = false;
			}

			if (!LevelData.CharHasSETItems(4))
			{
				gammaCheckBox.Enabled = false;
				gammaCheckBox.Checked = false;
			}

			if(!LevelData.CharHasSETItems(5))
			{
				bigCheckBox.Enabled = false;
				bigCheckBox.Checked = false;
			}
			#endregion
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			List<Item> selection = items.GetSelection();

			foreach (Item item in selection)
			{
				if (item is SETItem)
				{
					SETItem itemConv = (SETItem)item;

					if (sonicCheckBox.Checked)
					{
						LevelData.AddSETItem(0, new SETItem(itemConv.GetBytes(), 0, items));
					}

					if (tailsCheckBox.Checked)
					{
						LevelData.AddSETItem(1, new SETItem(itemConv.GetBytes(), 0, items));
					}

					if (knucklesCheckBox.Checked)
					{
						LevelData.AddSETItem(2, new SETItem(itemConv.GetBytes(), 0, items));
					}

					if (amyCheckBox.Checked)
					{
						LevelData.AddSETItem(3, new SETItem(itemConv.GetBytes(), 0, items));
					}

					if (gammaCheckBox.Checked)
					{
						LevelData.AddSETItem(4, new SETItem(itemConv.GetBytes(), 0, items));
					}

					if (bigCheckBox.Checked)
					{
						LevelData.AddSETItem(5, new SETItem(itemConv.GetBytes(), 0, items));
					}
				}
				else if (item is CAMItem)
				{
					CAMItem itemConv = (CAMItem)item;

					if ((sonicCheckBox.Checked) && (LevelData.CAMItems[0] != null))
					{
						LevelData.CAMItems[0].Add(new CAMItem(itemConv.GetBytes(), 0, items));
					}

					if (tailsCheckBox.Checked)
					{
						LevelData.CAMItems[1].Add(new CAMItem(itemConv.GetBytes(), 0, items));
					}

					if (knucklesCheckBox.Checked)
					{
						LevelData.CAMItems[2].Add(new CAMItem(itemConv.GetBytes(), 0, items));
					}

					if (amyCheckBox.Checked)
					{
						LevelData.CAMItems[3].Add(new CAMItem(itemConv.GetBytes(), 0, items));
					}

					if (gammaCheckBox.Checked)
					{
						LevelData.CAMItems[4].Add(new CAMItem(itemConv.GetBytes(), 0, items));
					}

					if (bigCheckBox.Checked)
					{
						LevelData.CAMItems[5].Add(new CAMItem(itemConv.GetBytes(), 0, items));
					}
				}
			}

			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
