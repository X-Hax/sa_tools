using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TextureLib;

namespace TextureEditor
{
	public partial class IndexedImageImportDialog : Form
	{
        Bitmap bitmap;
        GenericTexture texinfo;
        bool SACompatible;
        bool gamecube;
        public IndexedTextureFormat outFormat;
        public PixelCodec outCodec;

		public IndexedImageImportDialog(Bitmap setbitmap, GenericTexture settexinfo, bool setSACompatible = true)
		{
			gamecube = settexinfo is GvrTexture;
			bitmap = setbitmap;
			texinfo = settexinfo;
			SACompatible = setSACompatible;
			InitializeComponent();
			labelPaletteInfo.Text = "Counting colors, please wait...";
		}

        private void ProcessBitmap()
        {
            int usedColors = 0;
            int totalColors = bitmap.Palette.Entries.Length;
            if (texinfo is PvrTexture)
            {
                radioButtonRGB5A3.Enabled = radioButtonIntensity8A.Enabled = false;
            }

			using (var snoop_u = new BmpPixelSnoop(new Bitmap(bitmap)))
			{
				List<Color> cols = new();
				for (int h = 0; h < bitmap.Height; h++)
				{
					for (int w = 0; w < bitmap.Width; w++)
					{
						Color color = snoop_u.GetPixel(w, h);
						if (!cols.Contains(color))
						{
							cols.Add(color);
						}
					}
				}
				usedColors = cols.Count;
			}

            if (usedColors <= 16)
            {
                radioButtonIndex4.Checked = true;
                radioButtonIndex4.Enabled = true;
            }
            else if (usedColors <= 256)
            {
                radioButtonIndex8.Checked = true;
                radioButtonIndex4.Enabled = false;
            }
            else
            {
                radioButtonIndex4.Enabled = radioButtonIndex8.Enabled = false;
                radioButtonNonIndexed.Checked = true;
            }
			BitmapAlphaLevel tlevel = TextureLib.TextureFunctions.GetAlphaLevelFromBitmap(bitmap);
            string transparency = "";
            switch (tlevel)
            {
                case BitmapAlphaLevel.None:
                    radioButtonRGB565.Checked = true;
                    transparency = "no transparency";
                    break;
                case BitmapAlphaLevel.OneBitAlpha:
                    transparency = "1-bit transparency";
                    if (SACompatible || !gamecube) 
                        radioButtonARGB1555.Checked = true;
                    else
                        radioButtonRGB5A3.Checked = true;
                    break;
                case BitmapAlphaLevel.FullAlpha:
                    transparency = "partial transparency";
                    if (SACompatible || !gamecube)
                        radioButtonARGB4444.Checked = true;
                    else
                        radioButtonRGB5A3.Checked = true;
                    break;
            }

            labelPaletteInfo.Text = "Palette: " + totalColors.ToString() + " colors (" + usedColors.ToString() + " colors used), " + transparency;
            groupBoxMask.Enabled = groupBoxPalette.Enabled = buttonAuto.Enabled = buttonOK.Enabled = true;
            if (!gamecube)
                radioButtonIntensity8A.Enabled = radioButtonRGB5A3.Enabled = false;
            else
                radioButtonIntensity8A.Enabled = radioButtonRGB5A3.Enabled = true;
        }

		private void IndexedImageImportDialog_Shown(object sender, EventArgs e)
		{
            groupBoxMask.Enabled = groupBoxPalette.Enabled = buttonAuto.Enabled = buttonOK.Enabled = false;
            Application.DoEvents();
            ProcessBitmap();
        }

		private void button1_Click(object sender, EventArgs e)
		{
            groupBoxMask.Enabled = groupBoxPalette.Enabled = buttonAuto.Enabled = buttonOK.Enabled = false;
            labelPaletteInfo.Text = "Counting colors, please wait...";
            Application.DoEvents();
            ProcessBitmap();
        }

        private void IndexedImageImportDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
			// Pixel codec
			if (radioButtonRGB565.Checked)
				outCodec = new RGB565PixelCodec();
			else if (radioButtonARGB1555.Checked)
				outCodec = new ARGB1555PixelCodec();
			else if (radioButtonARGB4444.Checked)
				outCodec = new ARGB4444PixelCodec();
			else if (radioButtonARGB8888.Checked)
				outCodec = new ARGB8888PixelCodec();
			else if (radioButtonIntensity8A.Checked)
				outCodec = new IntensityA8PixelCodec();
			else if (radioButtonRGB5A3.Checked)
				outCodec = new RGB5A3PixelCodec();
            // Indexed format
            if (radioButtonIndex8.Checked)
                outFormat = IndexedTextureFormat.Index8;
            else if (radioButtonIndex4.Checked)
                outFormat = IndexedTextureFormat.Index4;
            else
                outFormat = IndexedTextureFormat.NotIndexed;
        }
	}
}