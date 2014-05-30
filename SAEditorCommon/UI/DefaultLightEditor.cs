using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SonicRetro.SAModel.SAEditorCommon;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    public partial class DefaultLightEditor : Form
    {
        public DefaultLightEditor()
        {
            InitializeComponent();
        }

        private void DefaultLightEditor_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            for (int i = 0; i < EditorOptions.Direct3DDevice.Lights.Count; i++)
            {
                comboBox1.Items.Add(i.ToString());
            }

            SetSelection(0);
        }

        private void SetSelection(int selection)
        {
            if (selection > -1) propertyGrid1.SelectedObject = EditorOptions.Direct3DDevice.Lights[selection];
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
             SetSelection(comboBox1.SelectedIndex);
        }
    }
}
