using System;
using System.Windows.Forms;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SADXLVL2
{
	public partial class EditLevelInfoDialog : Form
	{
		public EditLevelInfoDialog()
		{
			InitializeComponent();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			LevelData.geo.Name = label.Text;
			LevelData.geo.TextureFileName = checkBox1.Checked ? null : textureFile.Text;
			LevelData.geo.TextureList = (uint)textureList.Value;
			LevelData.geo.Author = author.Text;
			LevelData.geo.Description = description.Text;
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			textureFile.Enabled = !checkBox1.Checked;
		}

		private void EditLevelInfoDialog_Load(object sender, EventArgs e)
		{
			label.Text = LevelData.geo.Name;
			if (LevelData.geo.TextureFileName == null)
			{
				checkBox1.Checked = true;
				textureFile.Text = string.Empty;
			}
			else
			{
				checkBox1.Checked = false;
				textureFile.Text = LevelData.geo.TextureFileName;
			}
			textureList.Value = LevelData.geo.TextureList;
			author.Text = LevelData.geo.Author ?? string.Empty;
			description.Text = LevelData.geo.Description ?? string.Empty;
		}
	}
}
