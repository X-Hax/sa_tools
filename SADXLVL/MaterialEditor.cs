using System;
using System.Drawing;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXLVL2
{
    public partial class MaterialEditor : Form
    {
        private Material[] materials;
        private Bitmap[] textures;

        public MaterialEditor(Material[] mats, Bitmap[] textures)
        {
            materials = mats;
            this.textures = textures;
            InitializeComponent();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            LevelData.MainForm.DrawLevel();
        }

        private void MaterialEditor_Load(object sender, EventArgs e)
        {
            foreach (Material item in materials)
                comboBox1.Items.Add(item.Name);
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
