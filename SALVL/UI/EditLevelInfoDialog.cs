using System;
using System.Windows.Forms;

using SAModel.SAEditorCommon.DataTypes;

namespace SAModel.SALVL
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
            LevelData.geo.FarClipping = (float)numericUpDownClipDist.Value;
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
            labelAttribute.Text = "0x" + ((short)LevelData.geo.Attributes).ToString("X");
            numericUpDownClipDist.Value = (int)LevelData.geo.FarClipping;
            checkBoxAttributeAnimation.Checked = LevelData.geo.Attributes.HasFlag(SA1LandtableAttributes.EnableMotion);
            checkBoxAttributeClip.Checked = LevelData.geo.Attributes.HasFlag(SA1LandtableAttributes.CustomDrawDistance);
            checkBoxAttributePVM.Checked = LevelData.geo.Attributes.HasFlag(SA1LandtableAttributes.LoadTextureFile);
            checkBoxAttributeTexlist.Checked = LevelData.geo.Attributes.HasFlag(SA1LandtableAttributes.LoadTexlist);
            textureList.Value = LevelData.geo.TextureList;
			author.Text = LevelData.geo.Author ?? string.Empty;
			description.Text = LevelData.geo.Description ?? string.Empty;
		}

        private void checkBoxAttributeAnimation_Click(object sender, EventArgs e)
        {
            if (!checkBoxAttributeAnimation.Checked)
                LevelData.geo.Attributes &= ~SA1LandtableAttributes.EnableMotion;
            else
                LevelData.geo.Attributes |= SA1LandtableAttributes.EnableMotion;
            labelAttribute.Text = "0x" + ((short)LevelData.geo.Attributes).ToString("X");
        }
		private void checkBoxAttributeClip_Click(object sender, EventArgs e)
		{
            if (!checkBoxAttributeClip.Checked)
                LevelData.geo.Attributes &= ~SA1LandtableAttributes.CustomDrawDistance;
            else
                LevelData.geo.Attributes |= SA1LandtableAttributes.CustomDrawDistance;
            labelAttribute.Text = "0x" + ((short)LevelData.geo.Attributes).ToString("X");
        }

		private void checkBoxAttributeTexlist_Click(object sender, EventArgs e)
		{
            if (!checkBoxAttributeTexlist.Checked)
                LevelData.geo.Attributes &= ~SA1LandtableAttributes.LoadTexlist;
            else
                LevelData.geo.Attributes |= SA1LandtableAttributes.LoadTexlist;
            labelAttribute.Text = "0x" + ((short)LevelData.geo.Attributes).ToString("X");
        }

		private void checkBoxAttributePVM_Click(object sender, EventArgs e)
		{
            if (!checkBoxAttributePVM.Checked)
                LevelData.geo.Attributes &= ~SA1LandtableAttributes.LoadTextureFile;
            else
                LevelData.geo.Attributes |= SA1LandtableAttributes.LoadTextureFile;
            labelAttribute.Text = "0x" + ((short)LevelData.geo.Attributes).ToString("X");
        }
	}
}
