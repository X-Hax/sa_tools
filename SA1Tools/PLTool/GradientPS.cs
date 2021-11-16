using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PLTool
{
	enum SelectColorMode
	{
		LeftColor,
		Color1,
		Color2,
		RightColor,
		Ambient
	}

	public partial class GradientPS : Form
	{
		public List<Color> result;
		public List<Color> original;
		bool freeze = true; // Prevent controls from overwriting the colors at start

		public GradientPS(List<Color> originalColors)
		{
			InitializeComponent();
			pictureBoxLeftColor.BackColor = pictureBoxLeftPreview.BackColor = originalColors[0];
			pictureBoxRightColor.BackColor = pictureBoxRightPreview.BackColor = originalColors[255];
			pictureBoxAmbientColor.BackColor = Color.Black;
			pictureBoxCurrentPalette.Image = DrawGeneratedPreview(originalColors);
			pictureBox1.BackColor = originalColors[85];
			pictureBox2.BackColor = originalColors[170];
			pictureBoxNewPalette.Image = DrawGeneratedPreview(CreateGradient(), true);
			freeze = false;
			original = originalColors;
		}

		private Bitmap DrawGeneratedPreview(List<Color> list, bool drawTicks = false)
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
				// Draw Color 1/2
				if (drawTicks)
				{
					gfx.FillRectangle(new SolidBrush(Color.Red), trackBarColor1.Value, 0, 1, 8);
					gfx.FillRectangle(new SolidBrush(Color.Black), trackBarColor1.Value, 24, 1, 8);
					gfx.FillRectangle(new SolidBrush(Color.Blue), trackBarColor2.Value, 0, 1, 8);
					gfx.FillRectangle(new SolidBrush(Color.Black), trackBarColor2.Value, 24, 1, 8);
				}
			}
			return image;
		}

		private List<Color> CreateGradient()
		{
			result = new List<Color>();
			int c1 = trackBarColor1.Value < trackBarColor2.Value ? trackBarColor1.Value : trackBarColor2.Value;
			int c2 = trackBarColor1.Value < trackBarColor2.Value ? trackBarColor2.Value : trackBarColor1.Value;
			Color color1 = trackBarColor1.Value < trackBarColor2.Value ? pictureBox1.BackColor : pictureBox2.BackColor;
			Color color2 = trackBarColor1.Value < trackBarColor2.Value ? pictureBox2.BackColor : pictureBox1.BackColor;
			// Left - Color 1
			for (int f = 0; f < c1; f++)
			{
				float r = Math.Min(255, pictureBoxLeftColor.BackColor.R * (1 - f / (float)c1) + color1.R * f / (float)c1 + pictureBoxAmbientColor.BackColor.R);
				float g = Math.Min(255, pictureBoxLeftColor.BackColor.G * (1 - f / (float)c1) + color1.G * f / (float)c1 + pictureBoxAmbientColor.BackColor.G);
				float b = Math.Min(255, pictureBoxLeftColor.BackColor.B * (1 - f / (float)c1) + color1.B * f / (float)c1 + pictureBoxAmbientColor.BackColor.B);
				result.Add(Color.FromArgb((int)r, (int)g, (int)b));
			}
			// Color 1 - Color 2
			for (int f2 = 0; f2 < c2-c1; f2++)
			{
				float r2 = Math.Min(255, color1.R * (1 - f2 / (float)(c2 - c1)) + color2.R * f2 / (float)(c2 - c1) + pictureBoxAmbientColor.BackColor.R);
				float g2 = Math.Min(255, color1.G * (1 - f2 / (float)(c2 - c1)) + color2.G * f2 / (float)(c2 - c1) + pictureBoxAmbientColor.BackColor.G);
				float b2 = Math.Min(255, color1.B * (1 - f2 / (float)(c2 - c1)) + color2.B * f2 / (float)(c2 - c1) + pictureBoxAmbientColor.BackColor.B);
				result.Add(Color.FromArgb((int)r2, (int)g2, (int)b2));
			}
			// Color 2 - Right
			for (int f3 = 0; f3 < 256-c2; f3++)
			{
				float r3 = Math.Min(255, color2.R * (1 - f3 / (float)(256 - c2)) + pictureBoxRightColor.BackColor.R * f3 / (float)(256 - c2) + pictureBoxAmbientColor.BackColor.R);
				float g3 = Math.Min(255, color2.G * (1 - f3 / (float)(256 - c2)) + pictureBoxRightColor.BackColor.G * f3 / (float)(256 - c2) + pictureBoxAmbientColor.BackColor.G);
				float b3 = Math.Min(255, color2.B * (1 - f3 / (float)(256 - c2)) + pictureBoxRightColor.BackColor.B * f3 / (float)(256 - c2) + pictureBoxAmbientColor.BackColor.B);
				result.Add(Color.FromArgb((int)r3, (int)g3, (int)b3));
			}
			return result;
		}

		private void UpdateLabels()
		{
			labelColor1.Text = trackBarColor1.Value.ToString();
			labelColor2.Text = trackBarColor2.Value.ToString();
		}

		private void trackBarColor1_ValueChanged(object sender, EventArgs e)
		{
			UpdateLabels();
			if (!freeze)
				pictureBoxNewPalette.Image = DrawGeneratedPreview(CreateGradient(), true);
		}

		private void SelectColor(SelectColorMode mode)
		{
			using (colorDialog1 = new ColorDialog())
			{
				if (colorDialog1.ShowDialog() == DialogResult.OK)
				{
					switch (mode)
					{
						case SelectColorMode.LeftColor:
							pictureBoxLeftColor.BackColor = pictureBoxLeftPreview.BackColor = colorDialog1.Color;
							break;
						case SelectColorMode.RightColor:
							pictureBoxRightColor.BackColor = pictureBoxRightPreview.BackColor = colorDialog1.Color;
							break;
						case SelectColorMode.Color1:
							pictureBox1.BackColor = colorDialog1.Color;
							break;
						case SelectColorMode.Color2:
							pictureBox2.BackColor = colorDialog1.Color;
							break;
						case SelectColorMode.Ambient:
							pictureBoxAmbientColor.BackColor = colorDialog1.Color;
							break;
					}
					if (!freeze)
						pictureBoxNewPalette.Image = DrawGeneratedPreview(CreateGradient(), true);
				}
			}
		}

		private void pictureBoxLeftColor_Click(object sender, EventArgs e)
		{
			SelectColor(SelectColorMode.LeftColor);
		}

		private void pictureBoxLeftPreview_Click(object sender, EventArgs e)
		{
			SelectColor(SelectColorMode.LeftColor);
		}

		private void pictureBoxRightPreview_Click(object sender, EventArgs e)
		{
			SelectColor(SelectColorMode.RightColor);
		}

		private void pictureBoxRightColor_Click(object sender, EventArgs e)
		{
			SelectColor(SelectColorMode.RightColor);
		}

		private void pictureBoxAmbientColor_Click(object sender, EventArgs e)
		{
			SelectColor(SelectColorMode.Ambient);
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			SelectColor(SelectColorMode.Color1);
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			SelectColor(SelectColorMode.Color2);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			MouseEventArgs ms = (MouseEventArgs)e;
			contextMenuStrip1.Items[4].Text = "Use current values (0, " + trackBarColor1.Value.ToString() + ", " + trackBarColor2.Value.ToString() + ", 255)";
			contextMenuStrip1.Show(buttonPresets, ms.Location);
		}

		private void TwoColors_Click(object sender, EventArgs e)
		{
			pictureBox1.BackColor = original[0];
			pictureBox2.BackColor = original[255];
			trackBarColor1.Value = 0;
			trackBarColor2.Value = 255;
			pictureBoxLeftColor.BackColor = pictureBoxLeftPreview.BackColor = original[0];
			pictureBoxRightColor.BackColor = pictureBoxRightPreview.BackColor = original[255];
		}

		private void ThreeColors_Click(object sender, EventArgs e)
		{
			pictureBox1.BackColor = original[127];
			pictureBox2.BackColor = original[127];
			trackBarColor1.Value = 127;
			trackBarColor2.Value = 127;
			pictureBoxLeftColor.BackColor = pictureBoxLeftPreview.BackColor = original[0];
			pictureBoxRightColor.BackColor = pictureBoxRightPreview.BackColor = original[255];
		}

		private void FourColors85_Click(object sender, EventArgs e)
		{
			pictureBox1.BackColor = original[85];
			pictureBox2.BackColor = original[170];
			trackBarColor1.Value = 85;
			trackBarColor2.Value = 170;
			pictureBoxLeftColor.BackColor = pictureBoxLeftPreview.BackColor = original[0];
			pictureBoxRightColor.BackColor = pictureBoxRightPreview.BackColor = original[255];
		}

		private void FourColors64_Click(object sender, EventArgs e)
		{
			pictureBox1.BackColor = original[64];
			pictureBox2.BackColor = original[192];
			trackBarColor1.Value = 64;
			trackBarColor2.Value = 192;
			pictureBoxLeftColor.BackColor = pictureBoxLeftPreview.BackColor = original[0];
			pictureBoxRightColor.BackColor = pictureBoxRightPreview.BackColor = original[255];
		}

		private void AutoColors_Click(object sender, EventArgs e)
		{
			pictureBox1.BackColor = original[trackBarColor1.Value];
			pictureBox2.BackColor = original[trackBarColor2.Value];
			pictureBoxLeftColor.BackColor = pictureBoxLeftPreview.BackColor = original[0];
			pictureBoxRightColor.BackColor = pictureBoxRightPreview.BackColor = original[255];
			UpdateLabels();
			pictureBoxNewPalette.Image = DrawGeneratedPreview(CreateGradient(), true);
		}
	}
}
