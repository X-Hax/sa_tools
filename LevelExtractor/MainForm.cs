using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SAModel.LevelExtractor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 1;
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            NumericUpDown1.Hexadecimal = CheckBox3.Checked;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ComboBox1.SelectedIndex)
            {
                case 0:
                    numericUpDown2.Value = 0x00400000u;
                    break;
                case 1:
                    numericUpDown2.Value = 0x10000000u;
                    break;
                case 2:
                    numericUpDown2.Value = 0x8C010000u;
                    break;
                case 3:
                    numericUpDown2.Value = 0x0C900000u;
                    break;
                case 4:
                    numericUpDown2.Value = 0x8C500000u;
                    break;
                case 5:
                    numericUpDown2.Value = 0u;
                    break;
            }
        }

        private void fileSelector1_FileNameChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sd = new SaveFileDialog() { DefaultExt = "sa1lvl", Filter = "SA1LVL Files|*.sa1lvl|All Files|*.*" })
            {
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    new LandTable(System.IO.File.ReadAllBytes(fileSelector1.FileName), (int)NumericUpDown1.Value, (uint)numericUpDown2.Value, (ModelFormat)comboBox2.SelectedIndex).SaveToFile(sd.FileName, ModelFormat.SA1);
                }
            }
        }
    }
}
