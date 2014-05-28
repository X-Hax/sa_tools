using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D.TextureSystem;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    internal partial class MaterialEditor : Form
    {
        #region Events
        public delegate void FormUpdatedHandler (object sender, EventArgs e);
        public event FormUpdatedHandler FormUpdated;
        #endregion

        private Material[] materials;
        private BMPInfo[] textures;

        private ToolTip clampToolTip = new ToolTip();

        public MaterialEditor(Material[] mats, BMPInfo[] textures)
        {
            materials = mats;
            this.textures = textures;
            InitializeComponent();

            clampToolTip.SetToolTip(clampUCheck, "Enable/Disable tiling on the U Axis.");
            clampToolTip.SetToolTip(clampVCheck, "Enable/Disable tiling on the V Axis.");
            clampToolTip.SetToolTip(flipUCheck, "If checked, tiling on the U Axis is mirrored.");
            clampToolTip.SetToolTip(flipVCheck, "If checked, tiling on the V Axis is mirrored.");
            clampToolTip.SetToolTip(ignoreSpecCheck, "If checked, no specular (commonly mis-identified as 'gloss' will be present.");
            clampToolTip.SetToolTip(useAlphaCheck, "If checked, texture transparency will be enabled (and possibly non-texture transparency). ");
            clampToolTip.SetToolTip(useTextureCheck, "If checked, the texture map displayed to the left will be used. Otherwise the model will be a solid color.");
            clampToolTip.SetToolTip(envMapCheck, "If checked, the texture's uv maps will be mapped to the environment and the model will appear 'shiny'.");
            clampToolTip.SetToolTip(doubleSideCheck, "Doesn't do anything, since Sonic Adventure does not support backface cull.");
            clampToolTip.SetToolTip(flatShadeCheck, "If checked, polygon smoothing will be disabled and the model will appear faceted, like a cut gem or die.");
            clampToolTip.SetToolTip(ignoreLightCheck, "If checked, the model will not have any lighting applied.");
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            FormUpdated(this, null);
        }

        private void MaterialEditor_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < materials.Length; i++)
                comboBox1.Items.Add(i);
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;

            SetControls(comboBox1.SelectedIndex);
        }

        private void SetControls(int index)
        {
            // setting general
            diffuseColorBox.BackColor = materials[index].DiffuseColor;
            specColorBox.BackColor = materials[index].SpecularColor;
            textureBox.BackgroundImage = textures[materials[index].TextureID].Image;
            exponentTextBox.Text = materials[index].Exponent.ToString();
            filterModeDropDown.SelectedIndex = (int)materials[index].FilterMode;
            srcAlphaCombo.SelectedIndex = (int)materials[index].SourceAlpha;
            dstAlphaCombo.SelectedIndex = (int)materials[index].DestinationAlpha;

            // setting flags
            pickStatusCheck.Checked = materials[index].PickStatus;
            superSampleCheck.Checked = materials[index].SuperSample;
            clampUCheck.Checked = materials[index].ClampU;
            clampVCheck.Checked = materials[index].ClampV;
            flipUCheck.Checked = materials[index].FlipU;
            flipVCheck.Checked = materials[index].FlipV;
            ignoreSpecCheck.Checked = materials[index].IgnoreSpecular;
            useAlphaCheck.Checked = materials[index].UseAlpha;
            useTextureCheck.Checked = materials[index].UseTexture;
            envMapCheck.Checked = materials[index].EnvironmentMap;
            doubleSideCheck.Checked = materials[index].DoubleSided;
            flatShadeCheck.Checked = materials[index].FlatShading;
            ignoreLightCheck.Checked = materials[index].IgnoreLighting;
            userFlagsNumeric.Value = materials[index].UserFlags;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                SetControls(comboBox1.SelectedIndex);
            }
        }

        #region General Control Event Methods
        private void textureBox_Click(object sender, EventArgs e)
        {
            TexturePicker texPicker = new TexturePicker(textures, materials[comboBox1.SelectedIndex].TextureID);
            DialogResult texPickerResult = texPicker.ShowDialog();

            if (texPickerResult == System.Windows.Forms.DialogResult.OK)
            {
                materials[comboBox1.SelectedIndex].TextureID = texPicker.SelectedValue;
                textureBox.BackgroundImage = textures[materials[comboBox1.SelectedIndex].TextureID].Image;

                FormUpdated(this, null);
            }

            texPicker.Dispose();
        }

        private void diffuseColorBox_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                diffuseColorBox.BackColor = colorDialog.Color;
                materials[comboBox1.SelectedIndex].DiffuseColor = colorDialog.Color;
                FormUpdated(this, null);
            }
        }

        private void specColorBox_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                specColorBox.BackColor = colorDialog.Color;
                materials[comboBox1.SelectedIndex].SpecularColor = colorDialog.Color;
                FormUpdated(this, null);
            }
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            // check to see if exponent can be parsed
            float expParse = 0f;

            if (!float.TryParse(exponentTextBox.Text, out expParse))
            {
                MessageBox.Show("Specular exponent was invalid - setting to 10");
                materials[comboBox1.SelectedIndex].Exponent = 10;
            }
            else
            {
                materials[comboBox1.SelectedIndex].Exponent = expParse;
            }

            this.Close();
        }

        private void exponentTextBox_Leave(object sender, EventArgs e)
        {
            // check to see if exponent can be parsed
            float expParse = 0f;

            if (!float.TryParse(exponentTextBox.Text, out expParse))
            {
                MessageBox.Show("Specular exponent was invalid - setting to 10");
                materials[comboBox1.SelectedIndex].Exponent = 10;
            }
            else
            {
                materials[comboBox1.SelectedIndex].Exponent = expParse;
            }
        }
        #endregion

        #region Flag Check Event Methods
        private void pickStatusCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].PickStatus = pickStatusCheck.Checked;
        }

        private void superSampleCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].SuperSample = superSampleCheck.Checked;
        }

        private void clampUCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].ClampU = clampUCheck.Checked;
        }

        private void clampVCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].ClampV = clampVCheck.Checked;
        }

        private void flipUCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].FlipU = flipUCheck.Checked;
        }

        private void flipVCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].FlipV = flipVCheck.Checked;
        }

        private void ignoreSpecCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].IgnoreSpecular = ignoreSpecCheck.Checked;
        }

        private void useAlphaCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].UseAlpha = useAlphaCheck.Checked;
        }

        private void useTextureCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].UseTexture = useTextureCheck.Checked;
        }

        private void envMapCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].EnvironmentMap = envMapCheck.Checked;
        }

        private void doubleSideCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].DoubleSided = doubleSideCheck.Checked;
        }

        private void flatShadeCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].FlatShading = flatShadeCheck.Checked;
        }

        private void ignoreLightCheck_Click(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].IgnoreLighting = ignoreLightCheck.Checked;
        }

        private void userFlagsNumeric_ValueChanged(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].UserFlags = (byte)userFlagsNumeric.Value;
        }

        private void filterModeDropDown_SelectionChangeCommitted(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].FilterMode = (FilterMode)filterModeDropDown.SelectedIndex;
        }

        private void srcAlphaCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].SourceAlpha = (AlphaInstruction)srcAlphaCombo.SelectedIndex;
        }

        private void dstAlphaCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            materials[comboBox1.SelectedIndex].DestinationAlpha = (AlphaInstruction)dstAlphaCombo.SelectedIndex;
        }
        #endregion
    }
}
