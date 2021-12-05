using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PLTool
{
	public partial class ColorPattern : Form
	{
        public List<Color> result;
		public ColorPattern()
		{
			InitializeComponent();
            patternColor1.BackColor = Color.FromArgb(0, 204, 255);
            patternColor2.BackColor = Color.FromArgb(0, 255, 0);
            patternColor3.BackColor = Color.FromArgb(0, 0, 255);
            patternColor4.BackColor = Color.FromArgb(255, 255, 0);
            patternColor5.BackColor = Color.FromArgb(0, 255, 255);
            patternColor6.BackColor = Color.FromArgb(255, 0, 255);
            patternColor7.BackColor = Color.FromArgb(192, 192, 192);
            patternColor8.BackColor = Color.FromArgb(255, 255, 255);
            UpdatePreview();
        }

		private Bitmap DrawStretchedBitmap(Bitmap bitmap, int width, int height)
		{
			Bitmap result = new Bitmap(width, height);
			using (Graphics gfx = Graphics.FromImage(result))
			{
				gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
				gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				gfx.DrawImage(bitmap, 0, 0, width, height);
			}
			return result;
		}

		private Bitmap DrawPreview()
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
                    gfx.FillRectangle(new SolidBrush(result[i]), i, 0, 1, 32);
                }
            }
            return DrawStretchedBitmap(image, pictureBoxPreview.Width, pictureBoxPreview.Height);
        }

        private void UpdatePreview()
        {
            result = new List<Color>();
            result.Clear();
            for (int f = 0; f < 32; f++)
            {
                result.Add(patternColor1.BackColor);
                result.Add(patternColor2.BackColor);
                result.Add(patternColor3.BackColor);
                result.Add(patternColor4.BackColor);
                result.Add(patternColor5.BackColor);
                result.Add(patternColor6.BackColor);
                result.Add(patternColor7.BackColor);
                result.Add(patternColor8.BackColor);
            }
            pictureBoxPreview.Image = DrawPreview();
        }

        private void SetColor(PictureBox sender)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                sender.BackColor = colorDialog1.Color;
                UpdatePreview();
            }
        }

        private void patternColor1_Click(object sender, EventArgs e)
		{
            SetColor((PictureBox)sender);
        }
	}
}
