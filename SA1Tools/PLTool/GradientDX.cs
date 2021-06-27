using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLTool
{
	public partial class GradientDX : Form
	{
        public List<Color> result;
        bool freeze = true; // Prevent controls from overwriting the colors at start

        public GradientDX()
		{
			InitializeComponent();
            pictureBoxColor1.BackColor = Color.Red;
            pictureBoxColor2.BackColor = Color.Blue;
            pictureBoxAmbient.BackColor = Color.Black;
            UpdateNumerics();
            RefreshAllLabels();
            freeze = false;
        }

        private void UpdateColors()
        {
            if (freeze)
                return;
            pictureBoxColor1.BackColor = Color.FromArgb((int)numericUpDownCO1R.Value, (int)numericUpDownCO1G.Value, (int)numericUpDownCO1B.Value);
            pictureBoxColor2.BackColor = Color.FromArgb((int)numericUpDownCO2R.Value, (int)numericUpDownCO2G.Value, (int)numericUpDownCO2B.Value);
            pictureBoxAmbient.BackColor = Color.FromArgb((int)numericUpDownAmbientR.Value, (int)numericUpDownAmbientG.Value, (int)numericUpDownAmbientB.Value);
        }

        private void UpdateNumerics()
        {
            // CO1
            numericUpDownCO1R.Value = pictureBoxColor1.BackColor.R;
            numericUpDownCO1G.Value = pictureBoxColor1.BackColor.G;
            numericUpDownCO1B.Value = pictureBoxColor1.BackColor.B;

            // CO2
            numericUpDownCO2R.Value = pictureBoxColor2.BackColor.R;
            numericUpDownCO2G.Value = pictureBoxColor2.BackColor.G;
            numericUpDownCO2B.Value = pictureBoxColor2.BackColor.B;

            // AMB
            numericUpDownAmbientR.Value = pictureBoxAmbient.BackColor.R;
            numericUpDownAmbientG.Value = pictureBoxAmbient.BackColor.G;
            numericUpDownAmbientB.Value = pictureBoxAmbient.BackColor.B;
        }

        private void RefreshAllLabels()
        {
            // CO1
            labelCO1R.Text = ((float)numericUpDownCO1R.Value / 255.0f).ToString("0.000");
            labelCO1G.Text = ((float)numericUpDownCO1G.Value / 255.0f).ToString("0.000");
            labelCO1G.Text = ((float)numericUpDownCO1B.Value / 255.0f).ToString("0.000");
            labelCO1POW.Text = ((float)trackBarCO1Pow.Value/100.0f).ToString("0.00");

            // CO2
            labelCO2R.Text = ((float)numericUpDownCO2R.Value / 255.0f).ToString("0.000");
            labelCO2G.Text = ((float)numericUpDownCO2G.Value / 255.0f).ToString("0.000");
            labelCO2G.Text = ((float)numericUpDownCO2B.Value / 255.0f).ToString("0.000");
            labelCO2POW.Text = ((float)trackBarCO2Pow.Value / 100.0f).ToString("0.00");

            // AMB
            labelAmbientR.Text = ((float)numericUpDownAmbientR.Value / 255.0f).ToString("0.000");
            labelAmbientG.Text = ((float)numericUpDownAmbientG.Value / 255.0f).ToString("0.000");
            labelAmbientB.Text = ((float)numericUpDownAmbientB.Value / 255.0f).ToString("0.000");
        }

		private void numericUpDownCO1R_ValueChanged(object sender, EventArgs e)
		{
            RefreshAllLabels();
            UpdateColors();
        }
	}
}
