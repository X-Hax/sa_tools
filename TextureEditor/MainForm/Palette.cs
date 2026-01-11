using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TextureLib;
using static TextureEditor.Program;

namespace TextureEditor
{
	public partial class MainForm
	{
		private void DisplayPaletteSelectedColorInfo(Point coordinates)
		{
			int colorID = (coordinates.Y / 17) * 16 + coordinates.X / 17;
			Color c = currentPalette.GetColorAnyBank(colorID);
			labelCurrentPaletteColor.Text = $"Color {colorID}: A{c.A} R{c.R} G{c.G} B{c.B}";
			labelCurrentPaletteColor.Show();
		}

		private void ShowHidePaletteInfo()
		{
			labelCurrentPaletteColor.Hide();
			if (listBox1.SelectedIndex == -1)
			{
				labelPaletteFormat.Hide();
				palettePreview.Hide();
				buttonLoadPalette.Hide();
				buttonSavePalette.Hide();
				buttonResetPalette.Hide();
				panelPaletteInfo.Hide();
				return;
			}
			IndexedTextureFormat dataformat = textures[listBox1.SelectedIndex].GetIndexedFormat();
			switch (dataformat)
			{
				case IndexedTextureFormat.Index4:
				case IndexedTextureFormat.Index8:
					// If this is a GVR with an internal palette, load its palette
					if (textures[listBox1.SelectedIndex] is GvrTexture g && !g.RequiresPaletteFile)
						currentPalette = g.Palette;
					int rowsize = dataformat == IndexedTextureFormat.Index8 ? 256 : 16;
					int rowsize_stored = rowsize - currentPalette.StartColor;
					int numrows = currentPalette.GetNumColors() / rowsize_stored;
					comboBoxCurrentPaletteBank.Items.Clear();
					int maxbanks = Math.Max(1, currentPalette.GetNumColors() / rowsize);
					numericUpDownStartBank.Maximum = maxbanks - 1;
					numericUpDownStartBank.Value = Math.Max(0, Math.Min(currentPalette.StartBank, numericUpDownStartBank.Maximum));
					numericUpDownStartColor.Maximum = rowsize - 1;
					numericUpDownStartColor.Value = Math.Max(0, Math.Min(currentPalette.StartColor, numericUpDownStartColor.Maximum));
					currentPalette.StartBank = Math.Max((short)0, Math.Min((short)(maxbanks - 1), currentPalette.StartBank)); // Apparently some PVP files in PSO have broken bank IDs
					for (int i = 0; i < numrows; i++)
						if (currentPalette.StartBank + i < maxbanks)
							comboBoxCurrentPaletteBank.Items.Add((currentPalette.StartBank + i).ToString());
					paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = Math.Min(comboBoxCurrentPaletteBank.Items.Count - 1, paletteSet);
					palettePreview.Image = GeneratePalettePreview();
					string extint = textures[listBox1.SelectedIndex].RequiresPaletteFile ? "External" : "Internal";
					labelPaletteFormat.Text = $"Palette: {currentPalette.GetEncodedColorFormat()} ({extint}), {currentPalette.GetNumColors().ToString()} colors";
					labelPaletteFormat.Show();
					palettePreview.Show();
					panelPaletteInfo.Show();
					buttonLoadPalette.Show();
					buttonSavePalette.Show();
					buttonResetPalette.Show();
					textureSizeLabel.Hide();
					numericUpDownOrigSizeX.Value = textures[listBox1.SelectedIndex].Image.Width;
					numericUpDownOrigSizeY.Value = textures[listBox1.SelectedIndex].Image.Height;
					return;
				default:
					labelPaletteFormat.Hide();
					palettePreview.Hide();
					buttonLoadPalette.Hide();
					buttonSavePalette.Hide();
					buttonResetPalette.Hide();
					panelPaletteInfo.Hide();
					return;
			}
		}

		private void ResetPalette()
		{
			currentPalette = TexturePalette.CreateDefaultPalette(true);
			paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
			UpdateTextureInformation();
		}

		private void SetPaletteBank()
		{
			if (currentPalette != null && paletteSet != comboBoxCurrentPaletteBank.SelectedIndex)
			{
				paletteSet = comboBoxCurrentPaletteBank.SelectedIndex;
				textures[listBox1.SelectedIndex].SetPalette(currentPalette, paletteSet);
				UpdateTextureView();
			}
		}

		private void SetPaletteStartColor()
		{
			if (currentPalette != null)
				currentPalette.StartColor = (short)Math.Min(numericUpDownStartColor.Value, currentPalette.GetNumColors() - numericUpDownStartColor.Value);
		}

		private void LoadPaletteDialog()
		{
			using (OpenFileDialog fd = new OpenFileDialog() { Filter = "Texture Palette|*.pvp;*.gvp;*.bmp;*.png", DefaultExt = "pvp", FilterIndex = currentFormat == TextureArchiveFormat.PVM ? 1 : 2 })
			{
				DialogResult dr = fd.ShowDialog(this);
				if (dr == DialogResult.OK)
				{
					switch (Path.GetExtension(fd.FileName).ToLowerInvariant())
					{
						case ".png":
						case ".bmp":
							Bitmap bp = new Bitmap(fd.FileName);
							currentPalette = new TexturePalette(new Bitmap(bp));
							bp.Dispose();
							break;
						case ".pvp":
						case ".gvp":
						default:
							currentPalette = new TexturePalette(File.ReadAllBytes(fd.FileName), compatibleGVPToolStripMenuItem.Checked);
							break;
					}
					paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
					UpdateTextureInformation();
				}
			}
		}

		private void SavePaletteDialog(string defaultFilename)
		{
			using (SaveFileDialog fd = new SaveFileDialog() { Title = "Save Palette File", FileName = defaultFilename, Filter = "PVR Palette|*.pvp|GVR Palette|*.gvp|Bitmaps|*.bmp;*.png", DefaultExt = "pvp", FilterIndex = currentFormat == TextureArchiveFormat.PVM ? 1 : 2 })
			{
				DialogResult dr = fd.ShowDialog(this);
				if (dr == DialogResult.OK)
				{
					switch (Path.GetExtension(fd.FileName).ToLowerInvariant())
					{
						case ".pvp":
							currentPalette.SavePVP(fd.FileName);
							break;
						case ".gvp":
							currentPalette.SaveGVP(fd.FileName);
							break;
						case ".png":
							currentPalette.SavePNG(fd.FileName);
							break;
					}
				}
			}
		}

		private Bitmap GeneratePalettePreview()
		{
			int rows = currentPalette.GetNumColors() / 16;
			Bitmap result = new Bitmap(256 + 16, rows * 16 + rows);
			int offset_y = 0;
			for (int y = 0; y < rows; y++)
			{
				int offset_x = 0;
				for (int x = 0; x < 16; x++)
				{
					Color c = currentPalette.GetColorAnyBank(y * 16 + x);
					// Draw a 16x16 square
					for (int z = 0; z < 16; z++)
						for (int h = 0; h < 16; h++)
						{
							result.SetPixel(offset_x + x * 16 + z, offset_y + y * 16 + h, c);
						}
					offset_x += 1;
				}
				offset_y += 1;
			}
			return result;
		}
	}
}