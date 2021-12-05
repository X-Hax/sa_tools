using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PLTool
{
	public partial class GradientBasic : Form
	{
        public List<Color> result;

		public GradientBasic()
		{
            InitializeComponent();
            pictureBoxColor1.BackColor = Color.White;
            pictureBoxColor2.BackColor = Color.Black;
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
            for (int f = 0; f < 256; f++)
            {
                float r = Math.Min(255, pictureBoxColor1.BackColor.R * (1 - f / 256.0f) + pictureBoxColor2.BackColor.R * f / 256.0f);
                float g = Math.Min(255, pictureBoxColor1.BackColor.G * (1 - f / 256.0f) + pictureBoxColor2.BackColor.G * f / 256.0f);
                float b = Math.Min(255, pictureBoxColor1.BackColor.B * (1 - f / 256.0f) + pictureBoxColor2.BackColor.B * f / 256.0f);
                result.Add(Color.FromArgb((int)r, (int)g, (int)b));
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

		private void pictureBoxColor1_Click(object sender, EventArgs e)
		{
            SetColor(pictureBoxColor1);
        }

		private void pictureBoxColor2_Click(object sender, EventArgs e)
		{
            SetColor(pictureBoxColor2);
        }
	}
}
