using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SALVL
{
    public partial class AdvancedInfoDialog : Form
    {
        public AdvancedInfoDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
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

        private void AdvancedInfoDialog_Load(object sender, EventArgs e)
        {
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
