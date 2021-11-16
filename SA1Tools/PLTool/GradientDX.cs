using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PLTool
{
	public partial class GradientDX : Form
	{
        public List<Color> result;
        bool freeze = true; // Prevent controls from overwriting the colors at start

        public GradientDX(List<Color> original)
		{
			InitializeComponent();
            pictureBoxColor1.BackColor = Color.Red;
            pictureBoxColor2.BackColor = Color.Blue;
            pictureBoxAmbient.BackColor = Color.Black;
            UpdateNumerics();
            RefreshAllLabels();
            pictureBoxOriginal.Image = DrawGradientFromList(original);
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
            labelCO1POW.Text = ((float)trackBarCO1Pow.Value / 100.0f).ToString("0.00");

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

        private Bitmap DrawGradientFromList(List<Color> list)
        {
            Bitmap image = new Bitmap(256, 32);
            using (Graphics gfx = Graphics.FromImage(image))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                for (int i = 0; i < 256; i++)
                {
                    gfx.FillRectangle(new SolidBrush(list[i]), i, 0, 1, 32);
                }
            }
            return image;
        }

        private void ApplyPalette()
        {
            result = new List<Color>();
            for (int f = 0; f < 256; f++)
            {
                // CO1
                double r1 = (float)numericUpDownCO1R.Value * Math.Pow((1 - f / 256.0f), (float)trackBarCO1Pow.Value / 100.0f);
                double g1 = (float)numericUpDownCO1G.Value* Math.Pow((1 - f / 256.0f), (float)trackBarCO1Pow.Value / 100.0f);
                double b1 = (float)numericUpDownCO1B.Value* Math.Pow((1 - f / 256.0f), (float)trackBarCO1Pow.Value / 100.0f);
                //colors1.Add(Color.FromArgb((int)r1, (int)g1, (int)b1));
                // CO2
                double r2 = (float)numericUpDownCO2R.Value * Math.Pow((f / 256.0f), (float)trackBarCO2Pow.Value / 100.0f);
                double g2 = (float)numericUpDownCO2G.Value * Math.Pow((f / 256.0f), (float)trackBarCO2Pow.Value / 100.0f);
                double b2 = (float)numericUpDownCO2B.Value * Math.Pow((f / 256.0f), (float)trackBarCO2Pow.Value / 100.0f);
                // Final
                double r = Math.Min(255, r1 + r2 + (float)numericUpDownAmbientR.Value);
                double g = Math.Min(255, g1 + g2 + (float)numericUpDownAmbientG.Value);
                double b = Math.Min(255, b1 + b2 + (float)numericUpDownAmbientB.Value);
                result.Add(Color.FromArgb(255, (int)r, (int)g, (int)b));
            }
            pictureBoxResult.Image = DrawGradientFromList(result);
        }

		private void numericUpDownCO1R_ValueChanged(object sender, EventArgs e)
		{
            RefreshAllLabels();
            UpdateColors();
            ApplyPalette();
        }

		private void pictureBoxColor1_Click(object sender, EventArgs e)
		{
            using (colorDialog1 = new ColorDialog())
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    numericUpDownCO1R.Value = colorDialog1.Color.R;
                    numericUpDownCO1G.Value = colorDialog1.Color.G;
                    numericUpDownCO1B.Value = colorDialog1.Color.B;
                }
            }
		}

		private void pictureBoxColor2_Click(object sender, EventArgs e)
		{
            using (colorDialog1 = new ColorDialog())
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    numericUpDownCO2R.Value = colorDialog1.Color.R;
                    numericUpDownCO2G.Value = colorDialog1.Color.G;
                    numericUpDownCO2B.Value = colorDialog1.Color.B;
                }
            }
        }

		private void pictureBoxAmbient_Click(object sender, EventArgs e)
		{
            using (colorDialog1 = new ColorDialog())
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    numericUpDownAmbientR.Value = colorDialog1.Color.R;
                    numericUpDownAmbientG.Value = colorDialog1.Color.G;
                    numericUpDownAmbientB.Value = colorDialog1.Color.B;
                }
            }
        }
	}
}
