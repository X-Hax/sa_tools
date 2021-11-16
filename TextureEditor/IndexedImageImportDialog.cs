using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TextureEditor.TexturePalette;

namespace TextureEditor
{
	public partial class IndexedImageImportDialog : Form
	{
		Bitmap bitmap;
		TextureInfo texinfo;
		bool SACompatible;
		bool gamecube;
		public PalettedTextureFormat outFormat;
		public PixelCodec outCodec;

		public IndexedImageImportDialog(Bitmap setbitmap, TextureInfo settexinfo, bool setSACompatible=true)
		{
			gamecube = settexinfo is GvrTextureInfo;
			bitmap = setbitmap;
			texinfo = settexinfo;
			SACompatible = setSACompatible;
			InitializeComponent();
			labelPaletteInfo.Text = "Counting colors, please wait...";
		}

		private void ProcessBitmap()
		{
			int usedColors = 0;
			int totalColors = 0;
			if (texinfo is PvrTextureInfo)
			{
				radioButtonRGB5A3.Enabled = radioButtonIntensity8A.Enabled = false;
			}

			using (var snoop_u = new BmpPixelSnoop(new Bitmap(bitmap)))
			{
				for (int p = 0; p < bitmap.Palette.Entries.Length; p++)
				{
					totalColors += 1;
					bool match = false;
					for (int h = 0; h < bitmap.Height; h++)
						for (int w = 0; w < bitmap.Width; w++)
							if (snoop_u.GetPixel(w, h) == bitmap.Palette.Entries[p])
								match = true;
					if (match)
						usedColors += 1;
				}
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
			int tlevel = TextureFunctions.GetAlphaLevelFromBitmap(bitmap);
			string transparency = "";
			switch (tlevel)
			{
				case 0:
					radioButtonRGB565.Checked = true;
					transparency = "no transparency";
					break;
				case 1:
					transparency = "1-bit transparency";
					if (SACompatible || !gamecube) 
						radioButtonARGB1555.Checked = true;
					else
						radioButtonRGB5A3.Checked = true;
					break;
				case 2:
					transparency = "partial transparency";
					if (SACompatible || !gamecube)
						radioButtonARGB4444.Checked = true;
					else
						radioButtonRGB5A3.Checked = true;
					break;
			}

			labelPaletteInfo.Text = "Palette: " + totalColors.ToString() + " colors (" + usedColors.ToString() + " colors used) , " + transparency;
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
				outCodec = PixelCodec.RGB565;
			else if (radioButtonARGB1555.Checked)
				outCodec = PixelCodec.ARGB1555;
			else if (radioButtonARGB4444.Checked)
				outCodec = PixelCodec.ARGB4444;
			else if (radioButtonARGB8888.Checked)
				outCodec = PixelCodec.ARGB8888;
			else if (radioButtonIntensity8A.Checked)
				outCodec = PixelCodec.Intensity8A;
			else if (radioButtonRGB5A3.Checked)
				outCodec = PixelCodec.RGB5A3;
			// Indexed format
			if (radioButtonIndex8.Checked)
				outFormat = PalettedTextureFormat.Index8;
			else if (radioButtonIndex4.Checked)
				outFormat = PalettedTextureFormat.Index4;
			else
				outFormat = PalettedTextureFormat.NotIndexed;
		}
	}
}
