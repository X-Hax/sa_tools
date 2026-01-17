using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using TextureLib;
using static SAModel.SAEditorCommon.ChaoPalettes;
using static TextureEditor.Program;

namespace TextureEditor
{
	// The line below prevents Windows Forms Designer from opening on this .cs file
	partial class FormViewBlocker	{ }

	public partial class MainForm
	{
		/// <summary>Displays information on a specific color when the user clicks on the palette preview.</summary>
		/// <param name="coordinates">Mouse pointer location relative to the palette preview image.</param>
		private void DisplayPaletteSelectedColorInfo(Point coordinates)
		{
			int colorID = (coordinates.Y / 17) * 16 + coordinates.X / 17;
			Color c = currentPalette.GetColorAnyBank(colorID);
			labelCurrentPaletteColor.Text = $"Color {colorID}: A{c.A} R{c.R} G{c.G} B{c.B}";
			labelCurrentPaletteColor.Show();
		}

		/// <summary>Updates palette information for the currently selected texture.</summary>
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
					{
						currentPalette = g.Palette;
						toolStripStatusLabelPalette.Text = "Palette: from the GVR file";
					}
					// Get the number of palette banks
					int banksize = dataformat == IndexedTextureFormat.Index8 ? 256 : 16;
					int numbanks = currentPalette.GetNumColors() / banksize;
					int maxbanks = Math.Max(1, currentPalette.GetNumColors() / banksize);
					// Apparently some PVP files in PSO have broken bank IDs, so here's a workaround for that
					currentPalette.StartBank = Math.Max(0, Math.Min((short)(maxbanks - 1), currentPalette.StartBank));
					// Start Bank value
					numericUpDownStartBank.Maximum = maxbanks - 1;
					numericUpDownStartBank.Value = Math.Max(0, Math.Min(currentPalette.StartBank, numericUpDownStartBank.Maximum));
					// Start Color value
					numericUpDownStartColor.Maximum = banksize - 1;
					numericUpDownStartColor.Value = Math.Max(0, Math.Min(currentPalette.StartColor, numericUpDownStartColor.Maximum));
					// Update the combo box for currently selected bank
					comboBoxCurrentPaletteBank.Items.Clear();
					for (int i = 0; i < numbanks; i++)
						if (i < maxbanks)
							comboBoxCurrentPaletteBank.Items.Add((currentPalette.StartBank + i).ToString());
					// Set currently selected bank
					paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = Math.Min(comboBoxCurrentPaletteBank.Items.Count - 1, paletteSet);
					// Generate palette preview
					palettePreview.Image = GeneratePalettePreview();
					// Set palette information
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

		/// <summary>Creates a default palette and replaces the current palette with it.</summary>
		private void ResetPalette()
		{

			currentPalette = TexturePalette.CreateDefaultPalette(textures[listBox1.SelectedIndex].GetIndexedFormat() == IndexedTextureFormat.Index8);
			toolStripStatusLabelPalette.Text = "Palette: default";
			paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
			textures[listBox1.SelectedIndex].SetPalette(currentPalette, paletteSet);
			UpdateTextureView();
			ShowHidePaletteInfo();
		}

		/// <summary>Sets the currently selected palette bank ID and updates the texture preview.</summary>
		private void SetPaletteBank()
		{
			if (currentPalette != null && paletteSet != comboBoxCurrentPaletteBank.SelectedIndex)
			{
				paletteSet = comboBoxCurrentPaletteBank.SelectedIndex;
				textures[listBox1.SelectedIndex].SetPalette(currentPalette, paletteSet);
				UpdateTextureView();
			}
		}

		/// <summary>Sets the loaded palette's start color.</summary>
		private void SetPaletteStartColor()
		{
			if (currentPalette != null)
				currentPalette.StartColor = (short)Math.Min(numericUpDownStartColor.Value, currentPalette.GetNumColors() - numericUpDownStartColor.Value);
		}

		/// <summary>Dialog that loads PVP/GVP/PNG files into the current palette.</summary>
		private void LoadPaletteDialog(string defaultFilename)
		{
			using (OpenFileDialog fd = new OpenFileDialog() { FileName = Path.ChangeExtension(defaultFilename, null), Filter = "Texture Palette|*.pvp;*.gvp;*.bmp;*.png", DefaultExt = "pvp", FilterIndex = currentFormat == TextureArchiveFormat.PVM ? 1 : 2 })
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
							toolStripStatusLabelPalette.Text = "Palette: from Bitmap.";
							bp.Dispose();
							break;
						case ".pvp":
						case ".gvp":
						default:
							currentPalette = new TexturePalette(File.ReadAllBytes(fd.FileName), compatibleGVPToolStripMenuItem.Checked);
							toolStripStatusLabelPalette.Text = "Palette: " + Path.GetFileName(fd.FileName);
							break;
					}
					paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
					UpdateTextureInformation();
				}
			}
		}

		/// <summary>Dialog that saves the currently loaded palette as PVP/GVP/PNG.</summary>
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

		/// <summary>Draws a Bitmap containing a grid with the current palette's colors.</summary>
		/// <returns>Preview Bitmap.</returns>
		private static Bitmap GeneratePalettePreview()
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
							result.SetPixel(offset_x + x * 16 + z, offset_y + y * 16 + h, c);
					offset_x += 1;
				}
				offset_y += 1;
			}
			return result;
		}

		/// <summary>Checks whether the specified texture is a known paletted Chao texture and applies the palette to it.</summary>
		/// <returns>True if the palette was set, false if not.</returns>
		private bool SetChaoPalette(GenericTexture tex)
		{
			string texName = tex.Name.ToLowerInvariant();
			if (CheckIfTextureIsChaoPalettedTexture(texName))
			{
				var result = GetChaoPaletteAndBankFromTextureName(texName, settingsfile.ChaoAlignment, settingsfile.ChaoSecondEvolution);
				string palext = ".pvp";
				if (tex is GvrTexture)
					palext = ".gvp";
				string palname = result.Key;
				int palbank = result.Value;
				if (!string.IsNullOrEmpty(palname))
				{
					string folder = !string.IsNullOrEmpty(archiveFilename) ? Path.GetDirectoryName(archiveFilename) + "\\" : "";
					string pvppath = folder + palname + palext;
					if (File.Exists(pvppath))
					{
						currentPalette = new TexturePalette(File.ReadAllBytes(pvppath));
						paletteSet = palbank;
						toolStripStatusLabelPalette.Text = "Palette: " + palname + palext;
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Imports an indexed Bitmap into the currently selected texture, and encodes it in the specified indexed format.
		/// Also replaces the current palette with one extracted from the Bitmap and encoded using the specified pixel codec.
		/// </summary>
		/// <param name="indexedBitmap">Indexed Bitmap to import.</param>
		/// <param name="targetFormat">Target indexed format: Index4 or Index8.</param>
		/// <param name="paletteCodec">Pixel codec for encoding the palette.</param>
		/// <param name="clut">True if importing as a GVR texture's internal CLUT.</param>
		public void ImportIndexedImage(Bitmap indexedBitmap, IndexedTextureFormat targetFormat, PixelCodec paletteCodec, bool clut)
		{
			// Check if the texture format is compatible
			if (textures[currentTextureID] is not PvrTexture && textures[currentTextureID] is not GvrTexture)
			{
				MessageBox.Show(this, "Current texture format does not support indexed textures.", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			// Retrieve the palette from the indexed bitmap
			TexturePalette sourcePalette = TexturePalette.FromIndexedBitmap(indexedBitmap);
			// Set target format: PVR
			if (textures[currentTextureID] is PvrTexture pvr)
			{
				PvrDataFormat targetPvrFormat;
				switch (targetFormat)
				{
					case IndexedTextureFormat.Index4:
						targetPvrFormat = pvr.HasMipmaps ? PvrDataFormat.Index4Mipmaps : PvrDataFormat.Index4;
						break;
					case IndexedTextureFormat.Index8:
					default:
						targetPvrFormat = pvr.HasMipmaps ? PvrDataFormat.Index8Mipmaps : PvrDataFormat.Index8;
						break;
				}
				// Encode the source palette using the specified palette codec
				sourcePalette.Encode(paletteCodec);
				// Set the final palette
				currentPalette = new TexturePalette(sourcePalette.RawData, paletteCodec, sourcePalette.GetNumColors());
				// Create the texture
				textures[currentTextureID] = new PvrTexture(new Bitmap(indexedBitmap), targetPvrFormat, paletteCodec.GetPvrPixelFormat(), pvr.HasMipmaps, currentPalette, pvr.Gbix, pvr.Name, false, true);
			}
			// Set target format: GVR
			else if (textures[currentTextureID] is GvrTexture gvr)
			{
				GvrDataFormat targetGvrFormat = targetFormat == IndexedTextureFormat.Index4 ? GvrDataFormat.Index4 : GvrDataFormat.Index8;
				// Encode the source palette using the specified palette codec
				if (clut)
					paletteCodec.BigEndian = true;
				sourcePalette.Encode(paletteCodec, clut);
				// Set the final palette
				currentPalette = new TexturePalette(sourcePalette.RawData, paletteCodec, sourcePalette.GetNumColors(), 0, clut);
				// Create the texture
				textures[currentTextureID] = new GvrTexture(new Bitmap(indexedBitmap), targetGvrFormat, gvr.HasMipmaps, currentPalette, gvr.Gbix, gvr.Name, paletteCodec.GetGvrPaletteFormat(), false, !clut, settingsfile.SACompatiblePalettes);
			}
			textures[currentTextureID].Decode();
			toolStripStatusLabelPalette.Text = "Palette: from Bitmap";
		}

		/// <summary>
		/// Exports the currently selected PVR or GVR texture's image as an indexed Bitmap.
		/// The texture must be an Indexed PVR or GVR with a Palette applied.
		/// </summary>
		/// <returns>System.Drawing.Bitmap in Index4 or Index8 format.</returns>
		public Bitmap ExportIndexedImage()
		{
			byte[] origData; // Original texture's encoded data
			DataCodec codec; // Codec used to decode the original texture's encoded data
			int offset = 0; // Location of texture data (Needed for mipmapped PVR formats and GVRs with CLUT)
			GenericTexture texture = textures[currentTextureID];
			switch (texture)
			{
				case PvrTexture pvr:
					codec = PvrDataCodec.Create(pvr.PvrDataFormat, PixelCodec.GetPixelCodec(pvr.PvrPixelFormat));
					PvrDataCodec c = (PvrDataCodec)codec;
					origData = pvr.HeaderlessData;
					offset = pvr.HeaderlessData.Length - c.CalculateTextureSize(pvr.Width, pvr.Height);
					break;
				case GvrTexture gvr:
					codec = GvrDataCodec.GetDataCodec(gvr.GvrDataFormat);
					origData = gvr.HeaderlessData;
					GvrDataCodec g = (GvrDataCodec)codec;
					// Check for CLUT
					if (g.PaletteEntries != 0 && !gvr.RequiresPaletteFile)
					{
						PixelCodec paletteCodec = PixelCodec.GetPixelCodec(gvr.GvrPaletteFormat, settingsfile.SACompatiblePalettes);
						int paletteSize = paletteCodec.BytesPerPixel * g.PaletteEntries;
						offset += paletteSize;
					}
					break;
				default:
					MessageBox.Show(this, "Currently selected texture is not a PVR or GVR texture.", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return null;
			}
			if (texture.Palette == null)
			{
				MessageBox.Show(this, "Current texture is missing a palette.", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
			int bankSize = 16;
			IndexedTextureFormat sourceFormat = texture.GetIndexedFormat();
			switch (sourceFormat)
			{
				case IndexedTextureFormat.Index8:
					bankSize = 256;
					break;
				case IndexedTextureFormat.Index4:
					bankSize = 16;
					break;
				case IndexedTextureFormat.NotIndexed:
				default:
					MessageBox.Show(this, "Current texture is not indexed.", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return null;
			}
			// Export palette
			Color[] colors = texture.Palette.GetColors();
			Bitmap targetBitmap = new Bitmap(texture.Width, texture.Height, sourceFormat == IndexedTextureFormat.Index4 ? System.Drawing.Imaging.PixelFormat.Format4bppIndexed : System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
			// Hack for .NET 8.0 and older. Replace with a direct ColorPalette constructor in the future.
			Bitmap dummyBitmap = new Bitmap(1, 1, sourceFormat == IndexedTextureFormat.Index4 ? System.Drawing.Imaging.PixelFormat.Format4bppIndexed : System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
			ColorPalette replacePalette = dummyBitmap.Palette;
			for (int i = 0; i < bankSize; i++)
				replacePalette.Entries[i] = colors[bankSize * texture.PaletteBank + i];
			targetBitmap.Palette = replacePalette;
			// Decode to Index8
			byte[] indexData = codec.Decode(origData[offset..], texture.Width, texture.Height, null);
			// Set pixels in the target Bitmap
			for (int y = 0; y < texture.Height; y++)
				for (int x = 0; x < texture.Width; x++)
				{
					int pixIndex = indexData[y * texture.Width + x];
					// Index4 hack
					if (sourceFormat == IndexedTextureFormat.Index4)
						pixIndex = pixIndex & 0x0F;
					TextureFunctions.SetPixelIndex(targetBitmap, x, y, pixIndex);
				}
			return targetBitmap;
		}
	}
}