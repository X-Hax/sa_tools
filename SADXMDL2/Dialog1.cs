using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXMDL2
{
    public partial class Dialog1 : Form
    {
        public Dialog1()
        {
            InitializeComponent();
        }

        private void Dialog1_Load(object sender, EventArgs e)
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
                    numericUpDown2.Value = 0u;
                    break;
            }
        }
    }
}
