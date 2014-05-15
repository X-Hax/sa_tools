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

        public MaterialEditor(Material[] mats, BMPInfo[] textures)
        {
            materials = mats;
            this.textures = textures;
            InitializeComponent();
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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
                propertyGrid1.SelectedObject = materials[comboBox1.SelectedIndex];
        }
    }
}
