using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TextureLib;
using static TextureEditor.Program;
using System.Drawing;

namespace TextureEditor
{
	public partial class MainForm
	{
		/// <summary>Updates the status bar with the current number of textures in the texture list.</summary>
		private void UpdateTextureCount()
		{
			if (textures.Count == 1)
				toolStripStatusLabelTextures.Text = "1 texture";
			else
				toolStripStatusLabelTextures.Text = textures.Count + " textures";
			alphaSortingToolStripMenuItem.Enabled = currentFormat == TextureArchiveFormat.PAK;
		}

		/// <summary>Updates all information in the texture panel, including palette information and preview image.</summary>
		private void UpdateTextureInformation()
		{
			indexTextBox.Text = hexIndexCheckBox.Checked ? listBox1.SelectedIndex.ToString("X") : listBox1.SelectedIndex.ToString();
			bool en = listBox1.SelectedIndex != -1;
			removeTextureButton.Enabled = textureName.Enabled = importButton.Enabled = buttonReplaceImage.Enabled = buttonExportImage.Enabled = saveTextureButton.Enabled = en;
			globalIndex.Enabled = (en && !usingSocPak);
			if (en)
			{
				suppress = true;
				GenericTexture currentTexture = textures[listBox1.SelectedIndex];
				textureUpButton.Enabled = listBox1.SelectedIndex > 0;
				textureDownButton.Enabled = listBox1.SelectedIndex < textures.Count - 1;
				textureName.Text = currentTexture.Name;
				globalIndex.Value = currentTexture.Gbix;
				numericUpDownOrigSizeX.Value = currentTexture.Width;
				numericUpDownOrigSizeY.Value = currentTexture.Height;
				extraFormatLabel.Hide();
				textureSizeLabel.Hide();
				dataFormatLabel.Show();
				pixelFormatLabel.Show();
				checkBoxPAKUseAlpha.Enabled = false;
				checkBoxPAKUseAlpha.Hide();
				extraFormatLabel.Hide();
				textureSizeLabel.Hide();
				toolStripStatusLabelPalette.Visible = false;
				if (currentTexture.CanHaveMipmaps())
				{
					mipmapCheckBox.Enabled = true;
					exportMipmapsAsPNGToolStripMenuItem.Enabled = mipmapCheckBox.Checked = currentTexture.HasMipmaps;
				}
				else
					exportMipmapsAsPNGToolStripMenuItem.Enabled = mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
				if (currentFormat == TextureArchiveFormat.PAK && !usingSocPak)
				{
					// Enable the PAK alpha flag
					checkBoxPAKUseAlpha.Enabled = true;
					checkBoxPAKUseAlpha.Show();
					checkBoxPAKUseAlpha.Checked = currentTexture.PakMetadata.PakGvrFormat == GvrDataFormat.Rgb5a3;
					// Display Ninja surface flags (maybe have checkboxes for them instead?)
					NinjaSurfaceFlags njflags = currentTexture.PakMetadata.PakNinjaFlags;
					List<string> flags = new List<string>();
					if ((njflags & NinjaSurfaceFlags.NotTwiddled) != 0)
						flags.Add("Not Twiddled");
					else
						flags.Add("Twiddled");
					if ((njflags & NinjaSurfaceFlags.Mipmapped) != 0)
						flags.Add("Mipmapped");
					if ((njflags & NinjaSurfaceFlags.Palettized) != 0)
						flags.Add("Palettized");
					if ((njflags & NinjaSurfaceFlags.Stride) != 0)
						flags.Add("Stride");
					if ((njflags & NinjaSurfaceFlags.VQ) != 0)
						flags.Add("VQ");
					string ninjaFlagsString = string.Join(", ", flags);
					// Display PAK metadata
					extraFormatLabel.Text = $"PAK GVR Format: {currentTexture.PakMetadata.PakGvrFormat}" + System.Environment.NewLine + $"PAK Ninja Surface Flags: {ninjaFlagsString}";
					extraFormatLabel.Show();

				}
				else if (currentFormat == TextureArchiveFormat.PVMX)
				{
					textureSizeLabel.Text = $"Actual Size: {textures[listBox1.SelectedIndex].Image.Width}x{textures[listBox1.SelectedIndex].Image.Height}";
					textureSizeLabel.Show();
					numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = true;
					if (currentTexture.PvmxOriginalDimensions.Width != 0)
					{
						numericUpDownOrigSizeX.Value = currentTexture.PvmxOriginalDimensions.Width;
						numericUpDownOrigSizeY.Value = currentTexture.PvmxOriginalDimensions.Height;
					}
					else
					{
						numericUpDownOrigSizeX.Value = currentTexture.Image.Width;
						numericUpDownOrigSizeY.Value = currentTexture.Image.Height;
					}
				}
				switch (currentTexture)
				{
					case PvrTexture pvr:
						dataFormatLabel.Text = $"PVR Data Format: {pvr.PvrDataFormat}";
						if (!pvr.Indexed)
							pixelFormatLabel.Text = $"PVR Pixel Format: {pvr.PvrPixelFormat}";
						else
							pixelFormatLabel.Hide();
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = pvr.Image.Width;
						numericUpDownOrigSizeY.Value = pvr.Image.Height;
						switch (pvr.PvrDataFormat)
						{
							case PvrDataFormat.Index4:
							case PvrDataFormat.Index4Mipmaps:
							case PvrDataFormat.Index8:
							case PvrDataFormat.Index8Mipmaps:
								toolStripStatusLabelPalette.Visible = true;
								if (!SetChaoPalette(pvr))
								{
									string folder = !string.IsNullOrEmpty(archiveFilename) ? Path.GetDirectoryName(archiveFilename) + "\\" : "";
									string pvppath = folder + pvr.Name + ".pvp";
									if (File.Exists(pvppath))
									{
										currentPalette = new TexturePalette(File.ReadAllBytes(pvppath));
										toolStripStatusLabelPalette.Text = "Palette: " + Path.GetFileName(pvppath);
										paletteSet = 0;
									}
									else
										currentPalette = TexturePalette.CreateDefaultPalette(pvr.GetIndexedFormat() == IndexedTextureFormat.Index8);
								}
								int actualPaletteColors = currentPalette.GetNumColors();
								// Failsafe check if the palette has a smaller number of colors than the indexed image expects
								int neededcolors = (pvr.PvrDataFormat == PvrDataFormat.Index4 | pvr.PvrDataFormat == PvrDataFormat.Index4Mipmaps) ? 16 : 256;
								if (neededcolors - actualPaletteColors > 0)
									currentPalette.AddColors(new Color[neededcolors - actualPaletteColors]);
								// Set palette
								try
								{
									pvr.SetPalette(currentPalette, paletteSet);
								}
								catch (Exception ex)
								{
									MessageBox.Show(this, "Palette data couldn't be applied: " + ex.Message.ToString(), "Palette application error", MessageBoxButtons.OK, MessageBoxIcon.Error);
									currentPalette = TexturePalette.CreateDefaultPalette(pvr.GetIndexedFormat() == IndexedTextureFormat.Index8);
									toolStripStatusLabelPalette.Text = "Palette: default";
									pvr.SetPalette(currentPalette, 0);
								}
								break;
							default:
								break;
						}
						break;
					case GvrTexture gvr:
						dataFormatLabel.Text = $"GVR Data Format: {gvr.GvrDataFormat}";
						// The GVR palette format field is only relevant for textures with an internal CLUT
						if (gvr.Indexed && !gvr.RequiresPaletteFile)
							pixelFormatLabel.Text = $"GVR Palette Format: {gvr.GvrPaletteFormat}";
						else
							pixelFormatLabel.Hide();
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = gvr.Image.Width;
						numericUpDownOrigSizeY.Value = gvr.Image.Height;
						checkBoxPAKUseAlpha.Enabled = false;
						checkBoxPAKUseAlpha.Hide();
						extraFormatLabel.Visible = false;
						textureSizeLabel.Hide();
						switch (gvr.GvrDataFormat)
						{
							case GvrDataFormat.Index4:
							case GvrDataFormat.Index8:
								toolStripStatusLabelPalette.Visible = true;
								if (!SetChaoPalette(gvr))
								{
									string folder = !string.IsNullOrEmpty(archiveFilename) ? Path.GetDirectoryName(archiveFilename) + "\\" : "";
									string gvppath = folder + gvr.Name + ".gvp";
									if (File.Exists(gvppath))
									{
										currentPalette = new TexturePalette(File.ReadAllBytes(gvppath), compatibleGVPToolStripMenuItem.Checked);
										toolStripStatusLabelPalette.Text = "Palette: " + Path.GetFileName(gvppath);
										paletteSet = 0;
									}
									else
										TexturePalette.CreateDefaultPalette(gvr.GetIndexedFormat() == IndexedTextureFormat.Index8);
								}
								int actualPaletteColors = currentPalette.GetNumColors();
								// Failsafe check if the palette has a smaller number of colors than the indexed image expects
								int neededcolors = (gvr.GvrDataFormat == GvrDataFormat.Index4) ? 16 : 256;
								if (neededcolors - actualPaletteColors > 0)
								{
									for (int i = 0; i < neededcolors - actualPaletteColors; i++)
										currentPalette.AddColors(new Color[neededcolors - actualPaletteColors]);
								}
								// Set palette
								try
								{
									gvr.SetPalette(currentPalette, paletteSet);
								}
								catch (Exception)
								{
									MessageBox.Show(this, "Palette data couldn't be applied. This can be caused by using 16-color palettes on 256-color indexed images. Select a correct palette file and try again.", "Palette application error", MessageBoxButtons.OK, MessageBoxIcon.Error);
									currentPalette = TexturePalette.CreateDefaultPalette(gvr.GetIndexedFormat() == IndexedTextureFormat.Index8);
									toolStripStatusLabelPalette.Text = "Palette: default";
									gvr.SetPalette(currentPalette, 0);
								}
								break;
							default:
								break;
						}
						break;
					case XvrTexture xvr:
						dataFormatLabel.Text = $"Data Format: DDS (XVR)";
						dataFormatLabel.Hide();
						dataFormatLabel.Show();
						pixelFormatLabel.Text = $"XVR Format: {xvr.XvrType}";
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = xvr.Image.Width;
						numericUpDownOrigSizeY.Value = xvr.Image.Height;
						checkBoxPAKUseAlpha.Enabled = false;
						checkBoxPAKUseAlpha.Hide();
						extraFormatLabel.Hide();
						textureSizeLabel.Hide();
						break;
					case DdsTexture dds:
						dataFormatLabel.Text = $"Data Format: DDS";
						pixelFormatLabel.Text = $"DDS Pixel Format: {dds.DdsFormat}"; 
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						break;
					case GdiTexture gdi:
						dataFormatLabel.Text = "Data Format: " + GenericTexture.GetTextureFileType(gdi.RawData).ToString().ToUpperInvariant();
						pixelFormatLabel.Text = $"GDI Pixel Format: {gdi.GdiPixelFormat}";
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						break;
					default:
						break;
				}
				suppress = false;
				UpdateTextureMipmap();
				UpdateTextureView();
				ShowHidePaletteInfo();
			}
			else
			{
				textureUpButton.Enabled = false;
				textureDownButton.Enabled = false;
				mipmapCheckBox.Enabled = false;
				dataFormatLabel.Text = $"Data Format: Unknown";
				pixelFormatLabel.Text = $"Pixel Format: Unknown";
				textureSizeLabel.Hide();
				indexTextBox.Text = "-1";
				textureImage.Image = new Bitmap(64, 64);
				UpdateTextureMipmap();
				UpdateTextureView();
				ShowHidePaletteInfo();
			}
		}

		/// <summary>Updates the texture's preview image based on the filtering, zoom and mipmap settings.</summary>
		private void UpdateTextureView()
		{
			if (listBox1.SelectedIndex == -1)
				return;
			GenericTexture texinfo = textures[listBox1.SelectedIndex];
			Bitmap image;
			if (texinfo.HasMipmaps)
				image = texinfo.MipmapImages[trackBarMipmapLevel.Value];
			else
				image = texinfo.Image;
			textureImage.Image = ScaleBitmapToWindow(image, textureFilteringToolStripMenuItem.Checked ? System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic : System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor);
		}

		/// <summary>Scales a Bitmap to fit the current window size. Updates the "Zoom" text field.</summary>
		/// <param name="image">Source Bitmap.</param>
		/// <param name="mode">Filtering mode (HighQualityBicubic or NearestNeighbor based on filtering settings).</param>
		/// <returns>A scaled Bitmap.</returns>
		private Bitmap ScaleBitmapToWindow(Bitmap image, System.Drawing.Drawing2D.InterpolationMode mode)
		{
			float scale = 1.0f;
			switch (texturePreviewZoomTrackBar.Value)
			{
				case 0:
					scale = 1.0f / 16.0f;
					break;
				case 1:
					scale = 1.0f / 8.0f;
					break;
				case 2:
					scale = 1.0f / 4.0f;
					break;
				case 3:
					scale = 1.0f / 2.0f;
					break;
				case 4:
					scale = 1.0f;
					break;
				case 5:
					scale = 2.0f;
					break;
				case 6:
					scale = 4.0f;
					break;
				case 7:
					scale = 8.0f;
					break;
				case 8:
					scale = 16.0f;
					break;
			}
			if (trackBarMipmapLevel.Enabled && trackBarMipmapLevel.Value > 0)
				scale = scale * (float)(Math.Pow(2, trackBarMipmapLevel.Value));
			float newwidth = ((float)image.Width * scale);
			float newheight = ((float)image.Height * scale);
			if (newwidth < 1)
			{
				float ratio = 1.0f / newwidth;
				newwidth = newwidth * ratio;
				newheight = newheight * ratio;
			}
			if (newheight < 1)
			{
				float ratio = 1.0f / newheight;
				newwidth = newwidth * ratio;
				newheight = newheight * ratio;
			}
			if (newwidth > 2048)
			{
				float ratio = newwidth / 2048.0f;
				newwidth = newwidth / ratio;
				newheight = newheight / ratio;
			}
			if (newheight > 2048)
			{
				float ratio = newheight / 2048.0f;
				newwidth = newwidth / ratio;
				newheight = newheight / ratio;
			}
			labelZoomInfo.Text = "Zoom: " + (newwidth / (float)image.Width).ToString() + "x (" + newwidth.ToString() + "x" + newheight.ToString() + ")";
			Bitmap bmp = new Bitmap((int)newwidth, (int)newheight);
			using (Graphics gfx = Graphics.FromImage(bmp))
			{
				gfx.InterpolationMode = mode;
				gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				gfx.DrawImage(image, 0, 0, newwidth, newheight);
			}
			return bmp;
		}

		///<summary>Copies the texture's preview image to the clipboard.</summary>
		private void TexturePreviewCopy()
		{
			Clipboard.SetImage(textures[listBox1.SelectedIndex].Image);
		}

		///<summary>Re-encodes the currently selected texture using the Bitmap from the clipboard, retaining the current texture format.</summary>
		private void TexturePreviewPaste()
		{
			Bitmap tex = new Bitmap(Clipboard.GetImage());
			textures[listBox1.SelectedIndex].ImportBitmap(tex);
			UpdateTextureView();
			UpdateTextureInformation();
			unsaved = true;
		}

		///<summary>Updates mipmap-related controls for the currently selected texture.</summary>
		private void UpdateTextureMipmap()
		{
			if (listBox1.SelectedIndex != -1)
			{
				GenericTexture currentTexture = textures[listBox1.SelectedIndex];
				if (currentTexture.HasMipmaps)
				{
					trackBarMipmapLevel.Visible = labelMipmapLevel.Visible = true;
					trackBarMipmapLevel.Maximum = currentTexture.MipmapImages.Length - 1;
					trackBarMipmapLevel.Value = Math.Min(trackBarMipmapLevel.Value, currentTexture.MipmapImages.Length - 1);
					trackBarMipmapLevel.Enabled = true;
					int mipmapW = Math.Max(1, currentTexture.Image.Width / (int)Math.Pow(2, trackBarMipmapLevel.Value));
					int mipmapH = Math.Max(1, currentTexture.Image.Height / (int)Math.Pow(2, trackBarMipmapLevel.Value));
					labelMipmapLevel.Text = $"Mipmap Level: {trackBarMipmapLevel.Value} ({mipmapW}x{mipmapH})";
					return;
				}
			}
			// Texture not selected or no mipmaps
			trackBarMipmapLevel.Value = 0;
			trackBarMipmapLevel.Enabled = false;
			labelMipmapLevel.Text = "Mipmap Level: N/A";
			trackBarMipmapLevel.Visible = labelMipmapLevel.Visible = false;
		}
	}
}