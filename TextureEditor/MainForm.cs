using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VrSharp.Gvr;
using VrSharp.Pvr;
using ArchiveLib;
using SAModel.SAEditorCommon;
using VrSharp.Xvr;
using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using BCnEncoder.ImageSharp;

using static ArchiveLib.GenericArchive;
using static TextureEditor.TexturePalette;
using static SAModel.SAEditorCommon.SettingsFile;

using Application = System.Windows.Forms.Application;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;
using Image = System.Drawing.Image;
using BCnEncoder.Decoder;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using SharpDX.Direct3D9;

namespace TextureEditor
{
	public partial class MainForm : Form
	{
		readonly Properties.Settings Settings = Properties.Settings.Default; // MRU list
		Settings_TextureEditor settingsfile; // User settings
		bool unsaved;
		int currentTextureID = -1;
		TextureFormat currentFormat;
		string archiveFilename;
		List<TextureInfo> textures = new List<TextureInfo>();
		bool suppress = false;
		bool nonIndexedPAK = false;

		// Palette stuff
		static int paletteSet = 0;
		bool customPaletteLoaded = false;
		bool paletteApplied = false;
		TexturePalette currentPalette = new TexturePalette();
		TexturePalette defaultPalette = new TexturePalette();

		public MainForm()
		{
			Application.ThreadException += Application_ThreadException;
			InitializeComponent();
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			string log = "Texture Editor: New log entry on " + DateTime.Now.ToString("G") + System.Environment.NewLine +
				"Build date:" + File.GetLastWriteTime(Application.ExecutablePath).ToString(System.Globalization.CultureInfo.InvariantCulture) +
				System.Environment.NewLine + e.Exception.ToString();
			string errDesc = "Texture Editor has crashed with the following error:\n" + e.Exception.GetType().Name + ".\n\n" +
				"If you wish to report a bug, please include the following in your report:";
			ErrorDialog report = new ErrorDialog("Texture Editor", errDesc, log);
			DialogResult dgresult = report.ShowDialog();
			switch (dgresult)
			{
				case DialogResult.OK:
				case DialogResult.Abort:
					Application.Exit();
					break;
				case DialogResult.Cancel:
					listBox1.EndUpdate();
					suppress = false;
					UpdateTextureCount();
					UpdateTextureInformation();
					break;
			}
		}

		private void UpdateMRUList(string filename)
		{
			if (Settings.MRUList.Count > 10)
			{
				for (int i = 9; i < Settings.MRUList.Count; i++)
				{
					Settings.MRUList.RemoveAt(i);
				}
			}
			archiveFilename = filename;
			if (Settings.MRUList.Contains(filename))
			{
				int i = Settings.MRUList.IndexOf(filename);
				Settings.MRUList.RemoveAt(i);
				recentFilesToolStripMenuItem.DropDownItems.RemoveAt(i);
			}
			Settings.MRUList.Insert(0, filename);
			recentFilesToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename.Replace("&", "&&")));
			Text = currentFormat.ToString() + " Editor - " + filename;
			// Remove archive filename for individual PVR/GVR textures because only archives can be saved
			if (Path.GetExtension(archiveFilename).ToLowerInvariant() == ".pvr" || Path.GetExtension(archiveFilename).ToLowerInvariant() == ".gvr" || Path.GetExtension(archiveFilename).ToLowerInvariant() == ".xvr")
				archiveFilename = null;
		}

		private void UpdateTextureCount()
		{
			if (textures.Count == 1)
				toolStripStatusLabel1.Text = "1 texture";
			else
				toolStripStatusLabel1.Text = textures.Count + " textures";
			alphaSortingToolStripMenuItem.Enabled = currentFormat == TextureFormat.PAK;
		}

		private void UpdateTextureInformation()
		{
			indexTextBox.Text = hexIndexCheckBox.Checked ? listBox1.SelectedIndex.ToString("X") : listBox1.SelectedIndex.ToString();
			int actualPaletteColors = 0;
			bool en = listBox1.SelectedIndex != -1;
			removeTextureButton.Enabled = textureName.Enabled = importButton.Enabled = exportButton.Enabled = saveTextureButton.Enabled = en;
			globalIndex.Enabled = (en && !nonIndexedPAK);
			if (en)
			{
				suppress = true;
				textureUpButton.Enabled = listBox1.SelectedIndex > 0;
				textureDownButton.Enabled = listBox1.SelectedIndex < textures.Count - 1;
				textureName.Text = textures[listBox1.SelectedIndex].Name;
				globalIndex.Value = textures[listBox1.SelectedIndex].GlobalIndex;
				if (textures[listBox1.SelectedIndex].CheckMipmap())
				{
					mipmapCheckBox.Enabled = true;
					mipmapCheckBox.Checked = textures[listBox1.SelectedIndex].Mipmap;
				}
				else
					mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
				switch (textures[listBox1.SelectedIndex])
				{
					case PakTextureInfo pak:
						dataFormatLabel.Text = $"Data Format: {pak.DataFormat}";
						pixelFormatLabel.Text = $"Surface Flags: {pak.GetSurfaceFlags()}";
						dataFormatLabel.Show();
						pixelFormatLabel.Show();
						checkBoxPAKUseAlpha.Enabled = true;
						checkBoxPAKUseAlpha.Show();
						checkBoxPAKUseAlpha.Checked = pak.DataFormat == GvrDataFormat.Rgb5a3;
						textureSizeLabel.Hide();
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = pak.Image.Width;
						numericUpDownOrigSizeY.Value = pak.Image.Height;
						break;
					case PvrTextureInfo pvr:
						switch (pvr.DataFormat)
						{
							case PvrDataFormat.Index4:
							case PvrDataFormat.Index4Mipmaps:
							case PvrDataFormat.Index8:
							case PvrDataFormat.Index8Mipmaps:
								string folder = !string.IsNullOrEmpty(archiveFilename) ? Path.GetDirectoryName(archiveFilename) + "\\" : "";
								string pvppath = folder + pvr.Name + ".pvp";
								if (!customPaletteLoaded && File.Exists(pvppath))
								{
									currentPalette = new TexturePalette(File.ReadAllBytes(pvppath), settingsfile.SACompatiblePalettes);
									paletteSet = Math.Min(currentPalette.GetMaxBanks(pvr.DataFormat == PvrDataFormat.Index8 || pvr.DataFormat == PvrDataFormat.Index8Mipmaps), paletteSet);
								}
								if (!paletteApplied)
								{
									// Re-encode texture if data is missing
									if (pvr.TextureData == null)
									{
										pvr.PixelFormat = PvrPixelFormat.Rgb565; // Same as default palette
										PvrTextureEncoder enc = new PvrTextureEncoder(pvr.Image, pvr.PixelFormat, pvr.DataFormat);
										pvr.TextureData = new MemoryStream();
										enc.Save(pvr.TextureData);
										pvr.TextureData.Seek(0, SeekOrigin.Begin);
									}
									PvrTexture pt = new PvrTexture(pvr.TextureData.ToArray());
									currentPalette.IsGVP = false;
									// Failsafe check if the palette has a smaller number of colors than the indexed image expects
									int neededcolors = (pvr.DataFormat == PvrDataFormat.Index4 | pvr.DataFormat == PvrDataFormat.Index4Mipmaps) ? 16 : 256;
									actualPaletteColors = currentPalette.Colors.Count();
									if (neededcolors - actualPaletteColors > 0)
									{
										for (int i = 0; i < neededcolors - actualPaletteColors; i++)
											currentPalette.Colors.Add(Color.Black);
									}
									try
									{
										pt.SetPalette(new PvpPalette(currentPalette.GetBytes()), paletteSet);
									}
									catch (Exception ex)
									{
										MessageBox.Show(this, "Palette data couldn't be applied: " + ex.Message.ToString(), "Palette application error", MessageBoxButtons.OK, MessageBoxIcon.Error);
										currentPalette = new TexturePalette(defaultPalette.GetBytes());
										pt.SetPalette(new PvpPalette(currentPalette.GetBytes()), 0);
									}
									paletteApplied = true;
									pvr.Image = pt.ToBitmap();
								}
								break;
							default:
								break;
						}
						dataFormatLabel.Text = $"Data Format: {pvr.DataFormat}";
						pixelFormatLabel.Text = $"Pixel Format: {pvr.PixelFormat}";
						dataFormatLabel.Show();
						pixelFormatLabel.Show();
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = pvr.Image.Width;
						numericUpDownOrigSizeY.Value = pvr.Image.Height;
						checkBoxPAKUseAlpha.Enabled = false;
						checkBoxPAKUseAlpha.Hide();
						textureSizeLabel.Hide();
						break;
					case GvrTextureInfo gvr:
						switch (gvr.DataFormat)
						{
							case GvrDataFormat.Index4:
							case GvrDataFormat.Index8:
								string folder = !string.IsNullOrEmpty(archiveFilename) ? Path.GetDirectoryName(archiveFilename) + "\\" : "";
								string gvppath = folder + gvr.Name + ".gvp";
								if (!customPaletteLoaded && File.Exists(gvppath))
								{
									currentPalette = new TexturePalette(File.ReadAllBytes(gvppath), settingsfile.SACompatiblePalettes);
									paletteSet = Math.Min(currentPalette.GetMaxBanks(gvr.DataFormat == GvrDataFormat.Index8), paletteSet);
								}
								if (!paletteApplied)
								{
									if (gvr.TextureData == null)
									{
										gvr.PixelFormat = GvrPixelFormat.Rgb565; // Same as default palette
										GvrTextureEncoder enc = new GvrTextureEncoder(gvr.Image, gvr.PixelFormat, gvr.DataFormat);
										gvr.TextureData = new MemoryStream();
										enc.Save(gvr.TextureData);
										gvr.TextureData.Seek(0, SeekOrigin.Begin);
									}
									GvrTexture gt = new GvrTexture(gvr.TextureData.ToArray());
									currentPalette.IsGVP = true;
									// Failsafe check if the palette has a smaller number of colors than the indexed image expects
									int neededcolors = (gvr.DataFormat == GvrDataFormat.Index4) ? 16 : 256;
									actualPaletteColors = currentPalette.Colors.Count();
									if (neededcolors - actualPaletteColors > 0)
									{
										for (int i = 0; i < neededcolors - actualPaletteColors; i++)
											currentPalette.Colors.Add(Color.Black);
									}
									try
									{
										gt.SetPalette(new GvpPalette(currentPalette.GetBytes()), paletteSet);
									}
									catch (Exception)
									{
										MessageBox.Show(this, "Palette data couldn't be applied. This can be caused by using 16-color palettes on 256-color indexed images. Select a correct palette file and try again.", "Palette application error", MessageBoxButtons.OK, MessageBoxIcon.Error);
										currentPalette = new TexturePalette(defaultPalette.GetBytes());
										gt.SetPalette(new GvpPalette(currentPalette.GetBytes()), 0);
									}
									paletteApplied = true;
									gvr.Image = gt.ToBitmap();
								}
								break;
							default:
								break;
						}
						pixelFormatLabel.Text = $"Pixel Format: {gvr.PixelFormat}";
						dataFormatLabel.Text = $"Data Format: {gvr.DataFormat}";
						dataFormatLabel.Show();
						pixelFormatLabel.Show();
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = gvr.Image.Width;
						numericUpDownOrigSizeY.Value = gvr.Image.Height;
						checkBoxPAKUseAlpha.Enabled = false;
						checkBoxPAKUseAlpha.Hide();
						textureSizeLabel.Hide();
						break;
					case XvrTextureInfo xvr:
						pixelFormatLabel.Text = $"Pixel Format: {xvr.PixelFormat}";
						dataFormatLabel.Text = $"Data Format: DDS";
						dataFormatLabel.Hide();
						dataFormatLabel.Show();
						pixelFormatLabel.Show();
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = xvr.Image.Width;
						numericUpDownOrigSizeY.Value = xvr.Image.Height;
						checkBoxPAKUseAlpha.Enabled = false;
						checkBoxPAKUseAlpha.Hide();
						textureSizeLabel.Hide();
						break;
					case PvmxTextureInfo pvmx:
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = true;
						if (pvmx.Dimensions.HasValue)
						{
							numericUpDownOrigSizeX.Value = pvmx.Dimensions.Value.Width;
							numericUpDownOrigSizeY.Value = pvmx.Dimensions.Value.Height;
						}
						else
						{
							numericUpDownOrigSizeX.Value = pvmx.Image.Width;
							numericUpDownOrigSizeY.Value = pvmx.Image.Height;
						}
						dataFormatLabel.Hide();
						pixelFormatLabel.Hide();
						checkBoxPAKUseAlpha.Enabled = false;
						checkBoxPAKUseAlpha.Hide();
						textureSizeLabel.Text = $"Actual Size: {textures[listBox1.SelectedIndex].Image.Width}x{textures[listBox1.SelectedIndex].Image.Height}";
						textureSizeLabel.Show();
						break;
					default:
						dataFormatLabel.Hide();
						pixelFormatLabel.Hide();
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = textures[listBox1.SelectedIndex].Image.Width;
						numericUpDownOrigSizeY.Value = textures[listBox1.SelectedIndex].Image.Height;
						checkBoxPAKUseAlpha.Enabled = false;
						checkBoxPAKUseAlpha.Hide();
						textureSizeLabel.Hide();
						break;
				}
				suppress = false;
				UpdateTextureView();
				ShowHidePaletteInfo(actualPaletteColors);
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
				UpdateTextureView();
				ShowHidePaletteInfo(actualPaletteColors);
			}
		}

		private List<TextureInfo> GetTexturesFromFile(string fname)
		{
			nonIndexedPAK = false;
			byte[] datafile = File.ReadAllBytes(fname);
			if (Path.GetExtension(fname).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				datafile = FraGag.Compression.Prs.Decompress(datafile);
			List<TextureInfo> newtextures;
			if (PVMXFile.Identify(datafile))
			{
				PVMXFile pvmx = new PVMXFile(datafile);
				newtextures = new List<TextureInfo>();
				foreach (PVMXFile.PVMXEntry pvmxe in pvmx.Entries)
				{
					PvmxTextureInfo texinfo = new PvmxTextureInfo(Path.GetFileNameWithoutExtension(pvmxe.Name), pvmxe.GBIX, pvmxe.GetBitmap());
					if (pvmxe.HasDimensions())
						texinfo.Dimensions = new Size(pvmxe.Width, pvmxe.Height);
					newtextures.Add(texinfo);
				}
			}
			else if (PAKFile.Identify(fname))
			{
				PAKFile pak = new PAKFile(fname);
				string filenoext = Path.GetFileNameWithoutExtension(fname).ToLowerInvariant();
				bool hasIndex = false;
				string indexName = "";
				foreach (PAKFile.PAKEntry fl in pak.Entries)
				{
					// Search for the correct index file
					if (fl.Name.Equals(filenoext + ".inf", StringComparison.OrdinalIgnoreCase))
					{
						hasIndex = true;
						indexName = filenoext + ".inf";
						break;
					}
					// Search for an incorrectly named index file
					else if (Path.GetExtension(fl.Name).ToLowerInvariant() == ".inf")
					{
						MessageBox.Show(this, "The index file name must match the PAK file name for the game to recognize it. Please resave the archive with the desired filename.", "Texture Editor Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						hasIndex = true;
						indexName = fl.Name;
					}
				}
				// Handle non-indexed PAKs in the SOC folder
				if (!hasIndex)
				{
					nonIndexedPAK = true;
					newtextures = new List<TextureInfo>(pak.Entries.Count);
					foreach (PAKFile.PAKEntry fl in pak.Entries)
					{
						// Handle non-texture data to prevent crashing in SOC\model.pak
						Bitmap bmp;
						try
						{
							bmp = fl.GetBitmap();
						}
						catch (Exception ex)
						{
							bmp = new Bitmap(TextureEditor.Properties.Resources.error);
						}
						newtextures.Add(new PakTextureInfo(Path.GetFileNameWithoutExtension(fl.Name), 0, bmp, GvrDataFormat.Dxt1, 0, new MemoryStream(fl.Data)));
					}
				}
				else
				{
					byte[] inf = pak.Entries.Single((file) => file.Name.Equals(indexName, StringComparison.OrdinalIgnoreCase)).Data;
					newtextures = new List<TextureInfo>(inf.Length / 0x3C);
					for (int i = 0; i < inf.Length; i += 0x3C)
					{
						// Load a PAK INF entry
						byte[] pakentry = new byte[0x3C];
						Array.Copy(inf, i, pakentry, 0, 0x3C);
						PAKInfEntry entry = new PAKInfEntry(pakentry);
						// Load texture data
						byte[] dds = pak.Entries.First((file) => file.Name.Equals(entry.GetFilename() + ".dds", StringComparison.OrdinalIgnoreCase)).Data;
						MemoryStream str = new MemoryStream(dds);
						newtextures.Add(new PakTextureInfo(entry.GetFilename(), entry.globalindex, CreateBitmapFromStream(str), entry.Type, entry.fSurfaceFlags, str));
					}
				}
			}
			else
			{
				if (PBFile.Identify(datafile))
					datafile = new PBFile(datafile).GetPVM().GetBytes();
				PuyoArchiveType idResult = PuyoFile.Identify(datafile);
				if (idResult == PuyoArchiveType.Unknown)
				{
					MessageBox.Show(this, "Unknown archive type: " + fname + ".", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return new List<TextureInfo>();
				}
				PuyoFile arc = new PuyoFile(datafile);
				newtextures = new List<TextureInfo>(arc.Entries.Count);
				foreach (GenericArchiveEntry file in arc.Entries)
				{
					MemoryStream str = new MemoryStream(file.Data);
					str.Seek(0, SeekOrigin.Begin);
					if (file is PVMEntry pvme)
						newtextures.Add(new PvrTextureInfo(Path.GetFileNameWithoutExtension(file.Name), str));
					else if (file is GVMEntry gvme)
						newtextures.Add(new GvrTextureInfo(Path.GetFileNameWithoutExtension(file.Name), str));
					else if (file is XVMEntry xvme)
						newtextures.Add(new XvrTextureInfo(Path.GetFileNameWithoutExtension(file.Name), str));
				}
			}
			// Check if TextureInfo match the current format and convert if necessary
			for (int i = 0; i < newtextures.Count; i++)
			{
				switch (currentFormat)
				{
					case TextureFormat.PVM:
						if (!(newtextures[i] is PvrTextureInfo))
							newtextures[i] = new PvrTextureInfo(newtextures[i]);
						break;
					case TextureFormat.GVM:
						if (!(newtextures[i] is GvrTextureInfo))
							newtextures[i] = new GvrTextureInfo(newtextures[i]);
						break;
					case TextureFormat.PVMX:
						if (!(newtextures[i] is PvmxTextureInfo))
							newtextures[i] = new PvmxTextureInfo(newtextures[i]);
						break;
					case TextureFormat.PAK:
						if (!(newtextures[i] is PakTextureInfo))
							newtextures[i] = new PakTextureInfo(newtextures[i]);
						break;
					case TextureFormat.XVM:
						if (!(newtextures[i] is XvrTextureInfo))
							newtextures[i] = new XvrTextureInfo(newtextures[i]);
						break;
				}
			}
			return newtextures;
		}

		private bool LoadArchive(string filename)
		{
			// Load file
			customPaletteLoaded = false;
			byte[] datafile = File.ReadAllBytes(filename);
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				datafile = FraGag.Compression.Prs.Decompress(datafile);

			// Check if the file is a PVR/GVR/XVR
			PuyoArchiveType puyotype = PuyoArchiveType.Unknown;
			if (PvrTexture.Is(datafile))
			{
				puyotype = PuyoArchiveType.PVMFile;
				currentFormat = TextureFormat.PVM;
			}
			else if (GvrTexture.Is(datafile))
			{
				puyotype = PuyoArchiveType.GVMFile;
				currentFormat = TextureFormat.GVM;
			}
			else if (XvrTexture.Is(datafile))
			{
				puyotype = PuyoArchiveType.XVMFile;
				currentFormat = TextureFormat.XVM;
			}
			// If the file is a single PVR/GVR/XVR, create a PVM/GVM/XVM archive and add the texture to it.
			if (puyotype != PuyoArchiveType.Unknown)
			{
				string[] otherfiles = Directory.GetFiles(Path.GetDirectoryName(filename), "*" + Path.GetExtension(filename), SearchOption.TopDirectoryOnly);
				PuyoFile arc = new PuyoFile(puyotype);
				textures.Clear();
				int selIndex = 0;
				for (int i = 0; i < otherfiles.Length; i++)
				{
					try
					{
						datafile = File.ReadAllBytes(otherfiles[i]);
						if (Path.GetFileName(otherfiles[i]) == Path.GetFileName(filename))
							selIndex = i;
						switch (puyotype)
						{
							case PuyoArchiveType.PVMFile:
								arc.Entries.Add(new PVMEntry(datafile, Path.GetFileNameWithoutExtension(otherfiles[i])));
								MemoryStream strp = new MemoryStream(arc.Entries[i].Data);
								textures.Add(new PvrTextureInfo(Path.GetFileNameWithoutExtension(otherfiles[i]).Split('.')[0], strp));
								break;
							case PuyoArchiveType.GVMFile:
								arc.Entries.Add(new GVMEntry(datafile, Path.GetFileNameWithoutExtension(otherfiles[i])));
								MemoryStream strg = new MemoryStream(arc.Entries[i].Data);
								textures.Add(new GvrTextureInfo(Path.GetFileNameWithoutExtension(otherfiles[i]).Split('.')[0], strg));
								break;
							case PuyoArchiveType.XVMFile:
								arc.Entries.Add(new XVMEntry(datafile, Path.GetFileNameWithoutExtension(otherfiles[i])));
								MemoryStream strx = new MemoryStream(arc.Entries[i].Data);
								textures.Add(new XvrTextureInfo(Path.GetFileNameWithoutExtension(otherfiles[i]).Split('.')[0], strx));
								break;
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(this, "Folder loading cancelled. Could not add texture " + otherfiles[i] + ": " + ex.ToString(), "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;
					}
				}
				listBox1.Items.Clear();
				listBox1.Items.AddRange(textures.Select((item) => item.Name).ToArray());
				UpdateTextureCount();
				UpdateMRUList(Path.GetFullPath(filename));
				listBox1.SelectedIndex = listBox1.Items.Count == 0 ? -1 : selIndex;
				return true;
			}
			// Otherwise load the file as an archive
			if (PVMXFile.Identify(datafile))
				currentFormat = TextureFormat.PVMX;
			else if (PAKFile.Identify(filename))
				currentFormat = TextureFormat.PAK;
			else
			{
				if (PBFile.Identify(datafile))
					datafile = new PBFile(datafile).GetPVM().GetBytes();
				PuyoArchiveType identifyResult = PuyoFile.Identify(datafile);
				switch (identifyResult)
				{
					case PuyoArchiveType.PVMFile:
						currentFormat = TextureFormat.PVM;
						break;
					case PuyoArchiveType.GVMFile:
						currentFormat = TextureFormat.GVM;
						break;
					case PuyoArchiveType.XVMFile:
						currentFormat = TextureFormat.XVM;
						break;
					case PuyoArchiveType.Unknown:
						MessageBox.Show(this, "Unknown archive type: \"" + filename + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
				}
			}

			// Load textures
			List<TextureInfo> newtextures = GetTexturesFromFile(filename);
			if (newtextures == null || newtextures.Count == 0)
				return false;
			textures.Clear();
			textures.AddRange(newtextures);
			listBox1.Items.Clear();
			listBox1.Items.AddRange(textures.Select((item) => item.Name).ToArray());
			UpdateTextureCount();
			UpdateMRUList(Path.GetFullPath(filename));
			return true;
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			settingsfile = Settings_TextureEditor.Load();
			System.Collections.Specialized.StringCollection newlist = new System.Collections.Specialized.StringCollection();

			if (Settings.MRUList != null)
			{
				foreach (string file in Settings.MRUList)
				{
					if (File.Exists(file))
					{
						newlist.Add(file);
						recentFilesToolStripMenuItem.DropDownItems.Add(file.Replace("&", "&&"));
					}
				}
			}

			Settings.MRUList = newlist;

			highQualityGVMsToolStripMenuItem.Checked = settingsfile.HighQualityGVM;
			textureFilteringToolStripMenuItem.Checked = settingsfile.EnableFiltering;
			compatibleGVPToolStripMenuItem.Checked = settingsfile.SACompatiblePalettes;
			usePNGInsteadOfDDSToolStripMenuItem.Checked = settingsfile.UsePNGforPAK;

			if (Program.Arguments.Length > 0 && !LoadArchive(Program.Arguments[0]))
				Close();
		}

		private void Textures_DragEnter(object sender, DragEventArgs e)
		{
			Console.WriteLine("DragEnter");

			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			if (files.Length == 1)
			{
				foreach (string file in files)
				{
					switch (Path.GetExtension(file).ToLowerInvariant())
					{
						// Add textures from archives
						case ".prs":
						case ".pvm":
						case ".pb":
						case ".gvm":
						case ".xvm":
						case ".pak":
						case ".pvmx":
							e.Effect = DragDropEffects.Copy;
							return;
					}
				}
			}

			e.Effect = DragDropEffects.None;
		}

		private void Textures_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				if (unsaved)
				{
					DialogResult res = MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					switch (res)
					{
						case DialogResult.Yes:
							SaveTextures();
							break;
						case DialogResult.Cancel:
							return;
						case DialogResult.No:
							break;
					}
				}

				textures.Clear();
				listBox1.Items.Clear();
				archiveFilename = null;

				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

				LoadArchive(files[0]);
				listBox1_SelectedIndexChanged(sender, e);
				unsaved = false;
			}

		}

		private void NewFile(TextureFormat newfilefmt)
		{
			if (unsaved)
			{
				DialogResult res = MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (res)
				{
					case DialogResult.Yes:
						SaveTextures();
						break;
					case DialogResult.Cancel:
						return;
					case DialogResult.No:
						break;
				}
			}
			textures.Clear();
			listBox1.Items.Clear();
			archiveFilename = null;
			currentFormat = newfilefmt;
			Text = newfilefmt.ToString() + " Editor";
			UpdateTextureCount();
			listBox1.SelectedIndex = -1;
		}

		private void SaveTextures()
		{
			byte[] data;
			using (MemoryStream str = new MemoryStream())
			{
				switch (currentFormat)
				{
					case TextureFormat.PVM:
						PuyoFile puyo = new PuyoFile(PuyoArchiveType.PVMFile);
						foreach (PvrTextureInfo tex in textures)
							puyo.Entries.Add(new PVMEntry(EncodePVR(tex).ToArray(), tex.Name));
						data = puyo.GetBytes();
						if (Path.GetExtension(archiveFilename).Equals(".pb", StringComparison.OrdinalIgnoreCase))
							data = puyo.GetPB().GetBytes();
						unsaved = false;
						break;
					case TextureFormat.GVM:
						PuyoFile puyog = new PuyoFile(PuyoArchiveType.GVMFile);
						foreach (GvrTextureInfo tex in textures)
							puyog.Entries.Add(new GVMEntry(EncodeGVR(tex).ToArray(), tex.Name));
						data = puyog.GetBytes();
						unsaved = false;
						break;
					case TextureFormat.XVM:
						PuyoFile puyox = new PuyoFile(PuyoArchiveType.XVMFile);
						foreach (XvrTextureInfo tex in textures)
							puyox.Entries.Add(new XVMEntry(EncodeXVR(tex).ToArray(), tex.Name));
						data = puyox.GetBytes();
						unsaved = false;
						break;
					case TextureFormat.PVMX:
						PVMXFile pvmx = new PVMXFile();
						foreach (PvmxTextureInfo tex in textures)
						{
							MemoryStream ds;
							Size size = new Size(tex.Image.Width, tex.Image.Height);
							if (tex.Dimensions.HasValue)
								size = new Size(tex.Dimensions.Value.Width, tex.Dimensions.Value.Height);
							if (tex.TextureData == null)
							{
								ds = new MemoryStream();
								tex.Image.Save(ds, System.Drawing.Imaging.ImageFormat.Png);
							}
							else
								ds = tex.TextureData;
							pvmx.Entries.Add(new PVMXFile.PVMXEntry(tex.Name + ".png", tex.GlobalIndex, ds.ToArray(), size.Width, size.Height));
						}
						File.WriteAllBytes(archiveFilename, pvmx.GetBytes());
						unsaved = false;
						return;
					case TextureFormat.PAK:
						PAKFile pak = new PAKFile();
						string filenoext = Path.GetFileNameWithoutExtension(archiveFilename).ToLowerInvariant();
						pak.FolderName = filenoext;
						string longdir = nonIndexedPAK ? "..\\..\\..\\sonic2\\resource\\gd_pc\\soc\\" + filenoext : "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
						List<byte> inf = new List<byte>(textures.Count * 0x3C);
						foreach (PakTextureInfo item in textures)
						{
							using (MemoryStream tex = EncodeDDS(item))
							{
								byte[] tb = tex.ToArray();
								string name = item.Name.ToLowerInvariant();
								if (name.Length > 0x1C)
									name = name.Substring(0, 0x1C);
								pak.Entries.Add(new PAKFile.PAKEntry(name + ".dds", longdir + '\\' + name + ".dds", tb));
								// Create a new PAK INF entry
								PAKInfEntry entry = new PAKInfEntry();
								byte[] namearr = Encoding.ASCII.GetBytes(name);
								Array.Copy(namearr, entry.filename, namearr.Length);
								entry.globalindex = item.GlobalIndex;
								entry.nWidth = (uint)item.Image.Width;
								entry.nHeight = (uint)item.Image.Height;
								// Salvage GVR data if available
								if (item is PakTextureInfo pk)
								{
									entry.Type = entry.PixelFormat = pk.DataFormat;
									entry.fSurfaceFlags = pk.SurfaceFlags;
								}
								if (item.Mipmap)
									entry.fSurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
								inf.AddRange(entry.GetBytes());
							}
						}
						if (!nonIndexedPAK)
							pak.Entries.Insert(0, new PAKFile.PAKEntry(filenoext + ".inf", longdir + '\\' + filenoext + ".inf", inf.ToArray()));
						pak.Save(archiveFilename);
						unsaved = false;
						return;
					default:
						return;
				}
			}
			if (Path.GetExtension(archiveFilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				FraGag.Compression.Prs.Compress(data, archiveFilename);
			else
				File.WriteAllBytes(archiveFilename, data);
			unsaved = false;
		}

		private void ConvertTextures(TextureFormat newfmt)
		{
			switch (newfmt)
			{
				case TextureFormat.PVM:
					switch (currentFormat)
					{
						case TextureFormat.GVM:
							textures = new List<TextureInfo>(textures.Cast<GvrTextureInfo>().Select(a => new PvrTextureInfo(a)).Cast<TextureInfo>());
							break;
						case TextureFormat.PVMX:
						case TextureFormat.PAK:
						case TextureFormat.XVM:
							textures = new List<TextureInfo>(textures.Select(a => new PvrTextureInfo(a)).Cast<TextureInfo>());
							break;
					}
					break;
				case TextureFormat.GVM:
					switch (currentFormat)
					{
						case TextureFormat.PVM:
							textures = new List<TextureInfo>(textures.Cast<PvrTextureInfo>().Select(a => new GvrTextureInfo(a)).Cast<TextureInfo>());
							break;
						case TextureFormat.PVMX:
						case TextureFormat.PAK:
						case TextureFormat.XVM:
							textures = new List<TextureInfo>(textures.Select(a => new GvrTextureInfo(a)).Cast<TextureInfo>());
							break;
					}
					break;
				case TextureFormat.PVMX:
					switch (currentFormat)
					{
						case TextureFormat.PVM:
						case TextureFormat.GVM:
						case TextureFormat.PAK:
						case TextureFormat.XVM:
							textures = new List<TextureInfo>(textures.Select(a => new PvmxTextureInfo(a)).Cast<TextureInfo>());
							break;
					}
					break;
				case TextureFormat.PAK:
					switch (currentFormat)
					{
						case TextureFormat.PVM:
						case TextureFormat.GVM:
						case TextureFormat.PVMX:
						case TextureFormat.XVM:
							textures = new List<TextureInfo>(textures.Select(a => new PakTextureInfo(a)).Cast<TextureInfo>());
							break;
					}
					break;
				case TextureFormat.XVM:
					switch (currentFormat)
					{
						case TextureFormat.PVM:
						case TextureFormat.GVM:
						case TextureFormat.PAK:
						case TextureFormat.PVMX:
							textures = new List<TextureInfo>(textures.Select(a => new XvrTextureInfo(a)).Cast<TextureInfo>());
							break;
					}
					break;
			}
			currentFormat = newfmt;
		}

		private void SaveAs(TextureFormat savefmt)
		{
			string ext;
			switch (savefmt)
			{
				case TextureFormat.GVM:
					ext = "gvm";
					break;
				case TextureFormat.PVMX:
					ext = "pvmx";
					break;
				case TextureFormat.PAK:
					ext = "pak";
					break;
				case TextureFormat.XVM:
					ext = "xvm";
					break;
				case TextureFormat.PVM:
				default:
					ext = "pvm";
					break;
			}
			using (SaveFileDialog dlg = new SaveFileDialog() { FilterIndex = (int)savefmt + 1, DefaultExt = ext, Filter = "PVM Files|*.pvm;*.prs;*.pb|GVM Files|*.gvm;*.prs|PVMX Files|*.pvmx|PAK Files|*.pak|XVM Files|*.xvm;*.prs" })
			{
				if (!string.IsNullOrEmpty(archiveFilename))
				{
					dlg.InitialDirectory = Path.GetDirectoryName(archiveFilename);
					dlg.FileName = Path.ChangeExtension(Path.GetFileName(archiveFilename), ext);
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					ConvertTextures((TextureFormat)dlg.FilterIndex - 1);
					UpdateMRUList(dlg.FileName);
					SaveTextures();
					unsaved = false;
				}
			}
		}

		private void importTexturePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "txt", Filter = "index.txt|index.txt", FileName = "index.txt" })
			{
				if (archiveFilename != null)
					dlg.InitialDirectory = Path.GetDirectoryName(archiveFilename);
				if (dlg.ShowDialog(this) == DialogResult.OK)
					using (TextReader texList = File.OpenText(dlg.FileName))
					{
						string dir = Path.GetDirectoryName(dlg.FileName);
						listBox1.BeginUpdate();
						string line = texList.ReadLine();
						while (line != null)
						{
							string[] split = line.Split(',');
							if (split.Length > 1)
							{
								uint gbix = uint.Parse(split[0]);
								string name = Path.ChangeExtension(split[1], null);
								Bitmap bmp;
								byte[] file = File.ReadAllBytes(Path.Combine(dir, split[1]));
								uint check = BitConverter.ToUInt32(file, 0);
								MemoryStream str = new MemoryStream(file);
								bmp = CreateBitmapFromStream(str);
								bool dds = TextureFunctions.CheckIfTextureIsDDS(file);
								switch (currentFormat)
								{
									case TextureFormat.PVM:
										textures.Add(new PvrTextureInfo(name, gbix, bmp));
										break;
									case TextureFormat.GVM:
										textures.Add(new GvrTextureInfo(name, gbix, bmp));
										break;
									case TextureFormat.PVMX:
										PvmxTextureInfo pvmx = new PvmxTextureInfo(name, gbix, bmp);
										if (split.Length > 2)
										{
											string[] dim = split[2].Split('x');
											if (dim.Length > 1)
												pvmx.Dimensions = new Size(int.Parse(dim[0]), int.Parse(dim[1]));
										}
										if (dds)
											pvmx.TextureData = str;
										textures.Add(pvmx);
										break;
									case TextureFormat.PAK:
										PakTextureInfo pak = new PakTextureInfo(name, gbix, bmp);
										if (dds)
											pak.TextureData = str;
										textures.Add(pak);
										break;
									case TextureFormat.XVM:
										textures.Add(new XvrTextureInfo(name, gbix, bmp));
										break;
								}
								listBox1.Items.Add(name);
							}
							line = texList.ReadLine();
						}
						listBox1.EndUpdate();
						listBox1.SelectedIndex = textures.Count - 1;
						UpdateTextureCount();
						unsaved = true;
					}
			}
		}

		private void exportTexturePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { Title = "Save Folder", DefaultExt = "", Filter = "Folder|", FileName = "texturepack" })
			{

				if (archiveFilename != null)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(archiveFilename);
					dlg.FileName = Path.GetFileNameWithoutExtension(archiveFilename);
				}

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Directory.CreateDirectory(dlg.FileName);
					string dir = Path.Combine(Path.GetDirectoryName(dlg.FileName), Path.GetFileName(dlg.FileName));
					using (TextWriter texList = File.CreateText(Path.Combine(dir, "index.txt")))
					{
						foreach (TextureInfo tex in textures)
						{
							System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(tex.Image);
							PalettedTextureFormat indexedfmt = GetPalettedTextureFormat(tex);
							if (indexedfmt != PalettedTextureFormat.NotIndexed)
								bmp = ProcessIndexedBitmap(tex, indexedfmt);
							bmp.Save(Path.Combine(dir, tex.Name + ".png"));
							if (tex is PvmxTextureInfo xtex && xtex.Dimensions.HasValue)
								texList.WriteLine("{0},{1},{2}x{3}", xtex.GlobalIndex, xtex.Name + ".png", xtex.Dimensions.Value.Width, xtex.Dimensions.Value.Height);
							else
								texList.WriteLine("{0},{1},{2}x{3}", tex.GlobalIndex, tex.Name + ".png", tex.Image.Width, tex.Image.Height);
						}
					}
				}
			}
		}

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

		private void UpdateTextureView()
		{
			if (listBox1.SelectedIndex == -1)
				return;
			TextureInfo texinfo = textures[listBox1.SelectedIndex];
			Bitmap image;
			image = texinfo.Image;
			textureImage.Image = ScaleBitmapToWindow(image, textureFilteringToolStripMenuItem.Checked ? System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic : System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor);
		}

		private void addTextureButton_Click(object sender, EventArgs e)
		{
			string defext = null;
			string filter = "Supported Files|*.prs;*.pvm;*.pb;*.gvm;*.pvmx;*.pak;*.pvr;*.gvr;*.xvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.dds";
			switch (currentFormat)
			{
				case TextureFormat.PVM:
					defext = "pvr";
					break;
				case TextureFormat.GVM:
					defext = "gvr";
					break;
				case TextureFormat.PVMX:
					defext = "png";
					break;
				case TextureFormat.PAK:
					defext = "dds";
					break;
				case TextureFormat.XVM:
					defext = "xvr";
					break;
			}
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = defext, Filter = filter, Multiselect = true })
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					uint gbix = textures.Count == 0 ? 0 : textures.Max((item) => item.GlobalIndex);
					if (gbix != uint.MaxValue)
						gbix++;
					listBox1.BeginUpdate();
					foreach (string file in dlg.FileNames)
					{
						switch (Path.GetExtension(file).ToLowerInvariant())
						{
							// Add textures from archives
							case ".prs":
							case ".pvm":
							case ".pb":
							case ".gvm":
							case ".xvm":
							case ".pak":
							case ".pvmx":
								foreach (TextureInfo tex in GetTexturesFromFile(file))
								{
									textures.Add(tex);
									listBox1.Items.Add(tex.Name);
								}
								break;
							// Add individual textures
							default:
								{
									string name = Path.GetFileNameWithoutExtension(file);
									MemoryStream str = new MemoryStream(File.ReadAllBytes(file));
									// Add textures that match current archive format
									if (currentFormat == TextureFormat.PVM && PvrTexture.Is(file))
										textures.Add(new PvrTextureInfo(name, str));
									else if (currentFormat == TextureFormat.GVM && GvrTexture.Is(file))
										textures.Add(new GvrTextureInfo(name, str));
									else if (currentFormat == TextureFormat.XVM && XvrTexture.Is(file))
										textures.Add(new XvrTextureInfo(name, str));
									else
									{
										switch (currentFormat)
										{
											case TextureFormat.PVM:
												Bitmap bp = CreateBitmapFromStream(str);
												if (TextureFunctions.CheckTextureDimensions(bp.Width, bp.Height))
													textures.Add(new PvrTextureInfo(name, gbix, bp));
												else return;
												break;
											case TextureFormat.GVM:
												Bitmap gp = CreateBitmapFromStream(str);
												if (TextureFunctions.CheckTextureDimensions(gp.Width, gp.Height))
													textures.Add(new GvrTextureInfo(name, gbix, gp));
												else return;
												break;
											case TextureFormat.XVM:
												Bitmap xp = CreateBitmapFromStream(str);
												if (TextureFunctions.CheckTextureDimensions(xp.Width, xp.Height))
													textures.Add(new XvrTextureInfo(name, gbix, xp));
												else return;
												break;
											case TextureFormat.PVMX:
												textures.Add(new PvmxTextureInfo(name, gbix, CreateBitmapFromStream(str), str));
												break;
											case TextureFormat.PAK:
												textures.Add(new PakTextureInfo(name, gbix, CreateBitmapFromStream(str), GvrDataFormat.Dxt1, NinjaSurfaceFlags.Mipmapped, str));
												break;
										}
										if (gbix != uint.MaxValue)
											gbix++;
									}
									listBox1.Items.Add(name);
								}
								break;
						}
					}
					listBox1.EndUpdate();
					UpdateTextureCount();
					listBox1.SelectedIndex = textures.Count - 1;
					unsaved = true;
				}
			}
		}

		private void removeTextureButton_Click(object sender, EventArgs e)
		{
			int i = listBox1.SelectedIndex;
			textures.RemoveAt(i);
			listBox1.Items.RemoveAt(i);
			if (i == textures.Count)
				--i;
			listBox1.SelectedIndex = i;
			UpdateTextureCount();
			unsaved = true;
		}

		private void TextureUpButton_Click(object sender, EventArgs e)
		{
			int i = listBox1.SelectedIndex;
			TextureInfo ti = textures[i];
			textures.RemoveAt(i);
			textures.Insert(i - 1, ti);
			listBox1.BeginUpdate();
			listBox1.Items.RemoveAt(i);
			listBox1.Items.Insert(i - 1, ti.Name);
			listBox1.EndUpdate();
			listBox1.SelectedIndex = i - 1;
			unsaved = true;
		}

		private void TextureDownButton_Click(object sender, EventArgs e)
		{
			int i = listBox1.SelectedIndex;
			TextureInfo ti = textures[i];
			textures.RemoveAt(i);
			textures.Insert(i + 1, ti);
			listBox1.BeginUpdate();
			listBox1.Items.RemoveAt(i);
			listBox1.Items.Insert(i + 1, ti.Name);
			listBox1.EndUpdate();
			listBox1.SelectedIndex = i + 1;
			unsaved = true;
		}

		private void HexIndexCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			indexTextBox.Text = hexIndexCheckBox.Checked ? listBox1.SelectedIndex.ToString("X") : listBox1.SelectedIndex.ToString();
		}

		private void textureName_TextChanged(object sender, EventArgs e)
		{
			bool focus = textureName.Focused;
			int st = textureName.SelectionStart;
			int len = textureName.SelectionLength;
			string name = textureName.Text;
			foreach (char c in Path.GetInvalidFileNameChars())
				name = name.Replace(c, '_');
			listBox1.Items[listBox1.SelectedIndex] = textureName.Text = textures[listBox1.SelectedIndex].Name = name;
			if (focus)
				textureName.Focus();
			textureName.SelectionStart = st;
			textureName.SelectionLength = len;
		}

		private void globalIndex_ValueChanged(object sender, EventArgs e)
		{
			if (textures[listBox1.SelectedIndex].GlobalIndex != (uint)globalIndex.Value)
			{
				textures[listBox1.SelectedIndex].GlobalIndex = (uint)globalIndex.Value;
				unsaved = true;
			}
		}

		private void mipmapCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (textures[listBox1.SelectedIndex].Mipmap != mipmapCheckBox.Checked)
			{
				// Erase cached texture data
				if (mipmapCheckBox.Enabled)
					textures[listBox1.SelectedIndex].TextureData = null;
				textures[listBox1.SelectedIndex].Mipmap = mipmapCheckBox.Checked;
				// Update surface flags for PAK textures
				if (textures[listBox1.SelectedIndex] is PakTextureInfo pk)
				{
					if (!mipmapCheckBox.Checked)
						pk.SurfaceFlags &= ~NinjaSurfaceFlags.Mipmapped;
					else
						pk.SurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
				}
				// Set PVR data format
				else if (textures[listBox1.SelectedIndex] is PvrTextureInfo pv)
				{
					if (!mipmapCheckBox.Checked)
					{
						switch (pv.DataFormat)
						{
							case PvrDataFormat.VqMipmaps:
								pv.DataFormat = PvrDataFormat.Vq;
								break;
							case PvrDataFormat.SmallVqMipmaps:
								pv.DataFormat = PvrDataFormat.SmallVq;
								break;
							case PvrDataFormat.Index4Mipmaps:
								pv.DataFormat = PvrDataFormat.Index4;
								paletteApplied = false;
								break;
							case PvrDataFormat.Index8Mipmaps:
								pv.DataFormat = PvrDataFormat.Index8;
								paletteApplied = false;
								break;
							case PvrDataFormat.SquareTwiddledMipmaps:
								pv.DataFormat = PvrDataFormat.SquareTwiddled;
								break;
							case PvrDataFormat.SquareTwiddledMipmapsAlt:
								pv.DataFormat = PvrDataFormat.SquareTwiddled;
								break;
						}
					}
					else
					{
						switch (pv.DataFormat)
						{
							case PvrDataFormat.Vq:
								pv.DataFormat = PvrDataFormat.VqMipmaps;
								break;
							case PvrDataFormat.SmallVq:
								pv.DataFormat = PvrDataFormat.SmallVqMipmaps;
								break;
							case PvrDataFormat.Index4:
								pv.DataFormat = PvrDataFormat.Index4Mipmaps;
								paletteApplied = false;
								break;
							case PvrDataFormat.Index8:
								pv.DataFormat = PvrDataFormat.Index8Mipmaps;
								paletteApplied = false;
								break;
							case PvrDataFormat.SquareTwiddled:
								pv.DataFormat = PvrDataFormat.SquareTwiddledMipmaps;
								break;
						}
					}
				}
				UpdateTextureInformation();
				unsaved = true;
			}
		}

		private void textureImage_MouseMove(object sender, MouseEventArgs e)
		{
			if (listBox1.SelectedIndex != -1 && e.Button == MouseButtons.Left)
			{
				Bitmap bmp = new Bitmap(textures[listBox1.SelectedIndex].Image);
				DataObject dobj = new DataObject();
				dobj.SetImage(bmp);
				string fn = Path.Combine(Path.GetTempPath(), textures[listBox1.SelectedIndex].Name + ".png");
				bmp.Save(fn);
				dobj.SetFileDropList(new System.Collections.Specialized.StringCollection() { fn });
				DoDragDrop(dobj, DragDropEffects.Copy);
			}
		}

		private void textureImage_DragEnter(object sender, DragEventArgs e)
		{
			if (listBox1.SelectedIndex != -1 && (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Bitmap)))
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
		}

		private void textureImage_DragDrop(object sender, DragEventArgs e)
		{
			if (listBox1.SelectedIndex == -1) return;
			Bitmap tex = null;
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				MemoryStream ms = new MemoryStream(File.ReadAllBytes(((string[])e.Data.GetData(DataFormats.FileDrop, true))[0]));
				tex = CreateBitmapFromStream(ms);
			}
			else if (e.Data.GetDataPresent(DataFormats.Bitmap))
				tex = new Bitmap((Image)e.Data.GetData(DataFormats.Bitmap));
			else
				return;
			textures[listBox1.SelectedIndex].Image = tex;
			textures[listBox1.SelectedIndex].TextureData = null;
			UpdateTextureView();
			UpdateTextureInformation();
			unsaved = true;
		}

		private void textureImage_MouseClick(object sender, MouseEventArgs e)
		{
			if (listBox1.SelectedIndex != -1 && e.Button == MouseButtons.Right)
			{
				pasteToolStripMenuItem.Enabled = Clipboard.ContainsImage();
				contextMenuStrip1.Show(textureImage, e.Location);
			}
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetImage(textures[listBox1.SelectedIndex].Image);
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Bitmap tex = new Bitmap(Clipboard.GetImage());
			textures[listBox1.SelectedIndex].Image = tex;
			textures[listBox1.SelectedIndex].TextureData = null;
			UpdateTextureView();
			UpdateTextureInformation();
			unsaved = true;
		}

		private Bitmap CreateBitmapFromStream(MemoryStream texmemstr)
		{
			Bitmap bitmap_temp;
			Bitmap textureimg;
			if (PvrTexture.Is(texmemstr))
			{
				PvrTexture pvrt = new PvrTexture(texmemstr);
				bitmap_temp = pvrt.ToBitmap();
			}
			else if (GvrTexture.Is(texmemstr))
			{
				GvrTexture gvrt = new GvrTexture(texmemstr);
				bitmap_temp = gvrt.ToBitmap();
			}
			else if (XvrTexture.Is(texmemstr))
			{
				XvrTexture xvrt = new XvrTexture(texmemstr);
				bitmap_temp = xvrt.ToBitmap();
			}
			else
			{
				if (TextureFunctions.CheckIfTextureIsDDS(texmemstr.ToArray()))
				{
					PixelFormat pxformat;
					var image = Pfim.Pfimage.FromStream(texmemstr, new Pfim.PfimConfig());
					switch (image.Format)
					{
						case Pfim.ImageFormat.Rgba32:
							pxformat = PixelFormat.Format32bppArgb;
							break;
						case Pfim.ImageFormat.Rgb24:
							pxformat = PixelFormat.Format24bppRgb;
							break;
						case Pfim.ImageFormat.R5g5b5:
							pxformat = PixelFormat.Format16bppRgb555;
							break;
						case Pfim.ImageFormat.R5g5b5a1:
							pxformat = PixelFormat.Format16bppArgb1555;
							break;
						case Pfim.ImageFormat.R5g6b5:
							pxformat = PixelFormat.Format16bppRgb565;
							break;
						default:
							MessageBox.Show("Unsupported image format: " + image.Format.ToString());
							throw new NotImplementedException();
					}
					bitmap_temp = new Bitmap(image.Width, image.Height, pxformat);
					BitmapData bmpData = bitmap_temp.LockBits(new Rectangle(0, 0, bitmap_temp.Width, bitmap_temp.Height), ImageLockMode.WriteOnly, pxformat);
					Marshal.Copy(image.Data, 0, bmpData.Scan0, image.DataLen);
					bitmap_temp.UnlockBits(bmpData);
				}
				else
					bitmap_temp = new Bitmap(texmemstr);
			}
			// The bitmap is cloned to avoid access error on the original file
			textureimg = (Bitmap)bitmap_temp.Clone();
			bitmap_temp.Dispose();
			return textureimg;
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvr", Filter = "Texture Files|*.pvr;*.gvr;*.xvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp" };
			if (!string.IsNullOrEmpty(listBox1.GetItemText(listBox1.SelectedItem)))
				dlg.FileName = listBox1.GetItemText(listBox1.SelectedItem);
			DialogResult res = dlg.ShowDialog(this);
			if (res == DialogResult.OK)
			{
				string name = Path.GetFileNameWithoutExtension(dlg.FileName);
				MemoryStream texmemstr = new MemoryStream(File.ReadAllBytes(dlg.FileName));
				switch (currentFormat)
				{
					case TextureFormat.PVM:
						PvrTextureInfo oldpvr = (PvrTextureInfo)textures[listBox1.SelectedIndex];
						if (PvrTexture.Is(dlg.FileName))
							textures[listBox1.SelectedIndex] = new PvrTextureInfo(oldpvr.Name, texmemstr);
						else
						{
							Bitmap bmp = CreateBitmapFromStream(texmemstr);
							if (!TextureFunctions.CheckTextureDimensions(bmp.Width, bmp.Height))
								return;
							PvrPixelFormat pvrPixelFormat = TextureFunctions.GetPvrPixelFormatFromBitmap(bmp);
							PvrDataFormat pvrDataFormat = TextureFunctions.GetPvrDataFormatFromBitmap(bmp, false, true);
							if (pvrDataFormat == PvrDataFormat.Index4 || pvrDataFormat == PvrDataFormat.Index8 || pvrDataFormat == PvrDataFormat.Index4Mipmaps || pvrDataFormat == PvrDataFormat.Index8Mipmaps)
							{
								IndexedImageImportDialog idximp = new IndexedImageImportDialog(bmp, oldpvr, compatibleGVPToolStripMenuItem.Checked);
								DialogResult dialogResult = idximp.ShowDialog();
								if (dialogResult == DialogResult.OK)
								{
									if (idximp.outFormat == PalettedTextureFormat.NotIndexed)
									{
										pvrDataFormat = TextureFunctions.GetPvrDataFormatFromBitmap(bmp, false, false);
										Bitmap clone = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
										using (Graphics gr = Graphics.FromImage(clone))
										{
											gr.DrawImage(bmp, new Rectangle(0, 0, clone.Width, clone.Height));
										}
										bmp = clone;
									}
									else
									{
										RetrievePaletteFromIndexedBitmap(bmp, idximp.outFormat, idximp.outCodec, false);
										bmp = RetrieveMaskFromIndexedBitmap(bmp, idximp.outFormat);
										if (idximp.outFormat == PalettedTextureFormat.Index8)
											pvrDataFormat = PvrDataFormat.Index8;
										else
											pvrDataFormat = PvrDataFormat.Index4;
										paletteApplied = false;
									}
								}
								else return;
							}
							PvrTextureInfo newpvr = new PvrTextureInfo(oldpvr.Name, oldpvr.GlobalIndex, bmp);
							newpvr.PixelFormat = pvrPixelFormat;
							newpvr.DataFormat = pvrDataFormat;
							newpvr.TextureData = null;
							textures[listBox1.SelectedIndex] = newpvr;
						}
						break;
					case TextureFormat.GVM:
						GvrTextureInfo oldgvr = (GvrTextureInfo)textures[listBox1.SelectedIndex];
						if (GvrTexture.Is(dlg.FileName))
							textures[listBox1.SelectedIndex] = new GvrTextureInfo(oldgvr.Name, texmemstr);
						else
						{
							Bitmap bmp = CreateBitmapFromStream(texmemstr);
							if (!TextureFunctions.CheckTextureDimensions(bmp.Width, bmp.Height))
								return;
							GvrPixelFormat gvrPixelFormat = TextureFunctions.GetGvrPixelFormatFromBitmap(bmp);
							GvrDataFormat gvrDataFormat = TextureFunctions.GetGvrDataFormatFromBitmap(bmp, highQualityGVMsToolStripMenuItem.Checked, true);
							if (gvrDataFormat == GvrDataFormat.Index4 || gvrDataFormat == GvrDataFormat.Index8)
							{
								IndexedImageImportDialog idximp = new IndexedImageImportDialog(bmp, oldgvr, compatibleGVPToolStripMenuItem.Checked);
								DialogResult dialogResult = idximp.ShowDialog();
								if (dialogResult == DialogResult.OK)
								{
									if (idximp.outFormat == PalettedTextureFormat.NotIndexed)
									{
										gvrDataFormat = TextureFunctions.GetGvrDataFormatFromBitmap(bmp, highQualityGVMsToolStripMenuItem.Checked, false);
										Bitmap clone = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
										using (Graphics gr = Graphics.FromImage(clone))
										{
											gr.DrawImage(bmp, new Rectangle(0, 0, clone.Width, clone.Height));
										}
										bmp = clone;
									}
									else
									{
										RetrievePaletteFromIndexedBitmap(bmp, idximp.outFormat, idximp.outCodec, true);
										bmp = RetrieveMaskFromIndexedBitmap(bmp, idximp.outFormat);
										if (idximp.outFormat == PalettedTextureFormat.Index8)
											gvrDataFormat = GvrDataFormat.Index8;
										else
											gvrDataFormat = GvrDataFormat.Index4;
										paletteApplied = false;
									}
								}
								else return;
							}
							GvrTextureInfo newgvr = new GvrTextureInfo(oldgvr.Name, oldgvr.GlobalIndex, bmp);
							newgvr.DataFormat = gvrDataFormat;
							newgvr.PixelFormat = gvrPixelFormat;
							newgvr.TextureData = null;
							textures[listBox1.SelectedIndex] = newgvr;
						}
						break;
					case TextureFormat.XVM:
						XvrTextureInfo oldxvr = (XvrTextureInfo)textures[listBox1.SelectedIndex];
						if (XvrTexture.Is(dlg.FileName))
							textures[listBox1.SelectedIndex] = new XvrTextureInfo(oldxvr.Name, texmemstr);
						else
						{
							Bitmap bmp = CreateBitmapFromStream(texmemstr);
							if (!TextureFunctions.CheckTextureDimensions(bmp.Width, bmp.Height))
								return;
							XvrTextureInfo newxvr = new XvrTextureInfo(oldxvr.Name, oldxvr.GlobalIndex, bmp);
							newxvr.DataFormat = DirectXTexUtility.DXGIFormat.BC3UNORM;
							newxvr.PixelFormat = DirectXTexUtility.DXGIFormat.BC3UNORM;
							newxvr.TextureData = null;
							textures[listBox1.SelectedIndex] = newxvr;
						}
						break;
					case TextureFormat.PVMX:
						PvmxTextureInfo oldpvmx = (PvmxTextureInfo)textures[listBox1.SelectedIndex];
						textures[listBox1.SelectedIndex] = new PvmxTextureInfo(oldpvmx.Name, oldpvmx.GlobalIndex, CreateBitmapFromStream(texmemstr));
						break;
					case TextureFormat.PAK:
						PakTextureInfo oldpak = (PakTextureInfo)textures[listBox1.SelectedIndex];
						textures[listBox1.SelectedIndex] = new PakTextureInfo(name, oldpak.GlobalIndex, CreateBitmapFromStream(texmemstr), oldpak.DataFormat, oldpak.SurfaceFlags);
						break;
					default:
						break;
				}
				UpdateTextureInformation();
				unsaved = true;
			}
			listBox1.Select();
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", FileName = textures[listBox1.SelectedIndex].Name + ".png", Filter = "PNG Files|*.png" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Bitmap bmp = new Bitmap(textures[listBox1.SelectedIndex].Image);
					PalettedTextureFormat indexedfmt = GetPalettedTextureFormat(textures[listBox1.SelectedIndex]);
					if (indexedfmt != PalettedTextureFormat.NotIndexed)
						bmp = ProcessIndexedBitmap(textures[listBox1.SelectedIndex], indexedfmt);
					bmp.Save(dlg.FileName);
				}

			listBox1.Select();
		}

		private void saveTextureButton_Click(object sender, EventArgs e)
		{
			string ext = "png";
			switch (currentFormat)
			{
				case TextureFormat.PVM:
					ext = "pvr";
					break;
				case TextureFormat.GVM:
					ext = "gvr";
					break;
				case TextureFormat.XVM:
					ext = "xvr";
					break;
				case TextureFormat.PAK:
					ext = TextureFunctions.CheckIfTextureIsDDS(textures[listBox1.SelectedIndex].TextureData.ToArray()) ? "dds" : "png";
					break;
				case TextureFormat.PVMX:
					ext = TextureFunctions.CheckIfTextureIsDDS(textures[listBox1.SelectedIndex].TextureData.ToArray()) ? "dds" : "png";
					break;
			}
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = ext, FileName = textures[listBox1.SelectedIndex].Name, Filter = "All Files|*.*" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					TextureInfo info = textures[listBox1.SelectedIndex];
					if (info.TextureData == null)
					{
						if (info is PvrTextureInfo pvrt)
							info.TextureData = EncodePVR(pvrt);
						else if (info is GvrTextureInfo gvrt)
							info.TextureData = EncodeGVR(gvrt);
						else if (info is XvrTextureInfo xvrt)
							info.TextureData = EncodeXVR(xvrt);
						else if (info is PakTextureInfo pakt)
							info.TextureData = EncodeDDS(pakt);
						else
							info.Image.Save(info.TextureData, ImageFormat.Png);
					}
					File.WriteAllBytes(dlg.FileName, textures[listBox1.SelectedIndex].TextureData.ToArray());
				}

			listBox1.Select();
		}

		private void PAKEnableAlphaForAll(bool enable)
		{
			if (textures == null || textures.Count == 0)
				return;
			foreach (PakTextureInfo paktxt in textures)
			{
				if (enable)
					paktxt.DataFormat = GvrDataFormat.Rgb5a3;
				else
				{
					if ((paktxt.SurfaceFlags & NinjaSurfaceFlags.Palettized) != 0)
						paktxt.DataFormat = GvrDataFormat.Index4;
					else paktxt.DataFormat = GvrDataFormat.Dxt1;
				}
			}
			if (listBox1.SelectedIndex != -1)
				UpdateTextureInformation();
			unsaved = true;
		}

		#region Texture encoding
		private MemoryStream EncodePVR(PvrTextureInfo tex)
		{
			if (tex.TextureData != null)
				return TextureFunctions.UpdateGBIX(tex.TextureData, tex.GlobalIndex);
			tex.PixelFormat = TextureFunctions.GetPvrPixelFormatFromBitmap(tex.Image);
			tex.DataFormat = TextureFunctions.GetPvrDataFormatFromBitmap(tex.Image, tex.Mipmap, true);
			PvrTextureEncoder encoder = new PvrTextureEncoder(tex.Image, tex.PixelFormat, tex.DataFormat);
			encoder.GlobalIndex = tex.GlobalIndex;
			MemoryStream pvr = new MemoryStream();
			encoder.Save(pvr);
			pvr.Seek(0, SeekOrigin.Begin);
			return pvr;
		}

		private MemoryStream EncodeGVR(GvrTextureInfo tex)
		{
			if (tex.TextureData != null)
				return TextureFunctions.UpdateGBIX(tex.TextureData, tex.GlobalIndex, true);
			tex.DataFormat = TextureFunctions.GetGvrDataFormatFromBitmap(tex.Image, settingsfile.HighQualityGVM, true);
			tex.PixelFormat = TextureFunctions.GetGvrPixelFormatFromBitmap(tex.Image);
			GvrTextureEncoder encoder = new GvrTextureEncoder(tex.Image, tex.PixelFormat, tex.DataFormat);
			encoder.GlobalIndex = tex.GlobalIndex;
			MemoryStream gvr = new MemoryStream();
			encoder.Save(gvr);
			gvr.Seek(0, SeekOrigin.Begin);
			return gvr;
		}

		private MemoryStream EncodeXVR(XvrTextureInfo tex)
		{
			if (tex.TextureData != null)
			{
				return TextureFunctions.UpdateGBIX(tex.TextureData, tex.GlobalIndex, false, true);
			}
			XvrTextureEncoder encoder = new XvrTextureEncoder(tex.Image, tex.PixelFormat, tex.DataFormat);
			encoder.GlobalIndex = tex.GlobalIndex;
			encoder.HasAlpha = tex.useAlpha = TextureFunctions.GetAlphaLevelFromBitmap(tex.Image) != 0;
			encoder.HasMipmaps = tex.Mipmap;
			MemoryStream xvr = new MemoryStream();
			encoder.Save(xvr);
			xvr.Seek(0, SeekOrigin.Begin);
			return xvr;
		}

		private MemoryStream EncodeDDS(PakTextureInfo tex)
		{
			if (tex.TextureData != null)
				return tex.TextureData;
			if (settingsfile.UsePNGforPAK)
			{
				MemoryStream bmp = new MemoryStream();
				tex.Image.Save(bmp, ImageFormat.Png);
				return bmp;
			}
			Image<Rgba32> image;
			MemoryStream ms = new MemoryStream();
			tex.Image.Save(ms, ImageFormat.Png);
			image = SixLabors.ImageSharp.Image.Load<Rgba32>(ms.ToArray());
			ms.Dispose();
			BcEncoder encoder = new BcEncoder();
			encoder.OutputOptions.GenerateMipMaps = tex.Mipmap;
			encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
			encoder.OutputOptions.Format = (TextureFunctions.GetAlphaLevelFromBitmap(tex.Image) != 0) ? CompressionFormat.Bc3 : CompressionFormat.Bc1;
			encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;
			MemoryStream ddsData = new MemoryStream();
			encoder.EncodeToStream(image, ddsData);
			return ddsData;
		}
		#endregion

		#region Palette related functions

		private void ShowHidePaletteInfo(int palcolors)
		{
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
			PalettedTextureFormat dataformat = GetPalettedTextureFormat(textures[listBox1.SelectedIndex]);
			switch (dataformat)
			{
				case PalettedTextureFormat.Index4:
				case PalettedTextureFormat.Index8:
					int rowsize = dataformat == PalettedTextureFormat.Index8 ? 256 : 16;
					int rowsize_stored = rowsize - currentPalette.StartColor;
					int numrows = currentPalette.Colors.Count / rowsize_stored;
					comboBoxCurrentPaletteBank.Items.Clear();
					int maxbanks = Math.Max(1, currentPalette.Colors.Count / rowsize);
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
					labelPaletteFormat.Text = currentPalette.PixelCodec.ToString() + " : " + palcolors.ToString() + "/" + currentPalette.Colors.Count.ToString() + " colors";
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

		private void buttonLoadPalette_Click(object sender, EventArgs e)
		{
			LoadPaletteDialog();
		}

		private void buttonSavePalette_Click(object sender, EventArgs e)
		{
			SavePaletteDialog(textures[listBox1.SelectedIndex].Name);
		}

		private void buttonResetPalette_Click(object sender, EventArgs e)
		{
			currentPalette = new TexturePalette();
			paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
			paletteApplied = false;
			UpdateTextureInformation();
		}

		private void exportMaskToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportMaskToolStripMenuItem.Checked = true;
			exportPalettedIndexedToolStripMenuItem.Checked = exportPalettedFullToolStripMenuItem.Checked = false;
		}

		private void exportPalettedIndexedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportPalettedIndexedToolStripMenuItem.Checked = true;
			exportMaskToolStripMenuItem.Checked = exportPalettedFullToolStripMenuItem.Checked = false;
		}

		private void exportPalettedFullToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportPalettedFullToolStripMenuItem.Checked = true;
			exportMaskToolStripMenuItem.Checked = exportPalettedIndexedToolStripMenuItem.Checked = false;
		}

		private void numericUpDownStartBank_ValueChanged(object sender, EventArgs e)
		{
			if (currentPalette != null)
				currentPalette.StartBank = (short)numericUpDownStartBank.Value;
		}

		private void comboBoxCurrentPaletteBank_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (currentPalette != null && paletteSet != comboBoxCurrentPaletteBank.SelectedIndex)
			{
				paletteSet = comboBoxCurrentPaletteBank.SelectedIndex;
				paletteApplied = false;
				UpdateTextureInformation();
			}
		}

		private void numericUpDownStartColor_ValueChanged(object sender, EventArgs e)
		{
			if (currentPalette != null)
				currentPalette.StartColor = (short)Math.Min(numericUpDownStartColor.Value, currentPalette.Colors.Count - numericUpDownStartColor.Value);
		}

		private void LoadPaletteDialog()
		{
			using (OpenFileDialog fd = new OpenFileDialog() { Filter = "Texture Palette|*.pvp;*.gvp;*.bmp;*.png", DefaultExt = "pvp", FilterIndex = currentPalette.IsGVP ? 2 : 1 })
			{
				DialogResult dr = fd.ShowDialog(this);
				if (dr == DialogResult.OK)
				{
					customPaletteLoaded = true;
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
					paletteApplied = false;
					paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
					UpdateTextureInformation();
				}
			}
		}

		private void SavePaletteDialog(string defaultFilename)
		{
			using (SaveFileDialog fd = new SaveFileDialog() { Title = "Save Palette File", FileName = defaultFilename, Filter = "PVR Palette|*.pvp|GVR Palette|*.gvp|Bitmaps|*.bmp;*.png", DefaultExt = "pvp", FilterIndex = currentPalette.IsGVP ? 2 : 1 })
			{
				DialogResult dr = fd.ShowDialog(this);
				if (dr == DialogResult.OK)
				{
					switch (Path.GetExtension(fd.FileName).ToLowerInvariant())
					{
						case ".pvp":
						case ".gvp":
							currentPalette.IsGVP = Path.GetExtension(fd.FileName).ToLowerInvariant() == ".gvp";
							File.WriteAllBytes(fd.FileName, currentPalette.GetBytes());
							break;
						case ".png":
						case ".bmp":
							ImageFormat fmt = Path.GetExtension(fd.FileName).ToLowerInvariant() == ".png" ? ImageFormat.Png : ImageFormat.Bmp;
							Bitmap save = currentPalette.GetBitmap();
							save.Save(fd.FileName, fmt);
							break;
					}
				}
			}
		}

		private PalettedTextureFormat GetPalettedTextureFormat(TextureInfo info)
		{
			if (info is PvrTextureInfo pvri)
				switch (pvri.DataFormat)
				{
					case PvrDataFormat.Index4:
					case PvrDataFormat.Index4Mipmaps:
						return PalettedTextureFormat.Index4;
					case PvrDataFormat.Index8:
					case PvrDataFormat.Index8Mipmaps:
						return PalettedTextureFormat.Index8;
					default:
						return PalettedTextureFormat.NotIndexed;
				}
			else if (info is GvrTextureInfo gvri)
				switch (gvri.DataFormat)
				{
					case GvrDataFormat.Index4:
						return PalettedTextureFormat.Index4;
					case GvrDataFormat.Index8:
						return PalettedTextureFormat.Index8;
					default:
						return PalettedTextureFormat.NotIndexed;
				}
			else return PalettedTextureFormat.NotIndexed;
		}

		private Bitmap ProcessIndexedBitmap(TextureInfo tex, PalettedTextureFormat indxfmt)
		{
			// Prepare the mask
			Bitmap unpalettedBitmap;
			if (tex is PvrTextureInfo pvri)
			{
				PvrTexture pvrt = new PvrTexture(pvri.TextureData.ToArray());
				defaultPalette.IsGVP = false;
				pvrt.SetPalette(new PvpPalette(defaultPalette.GetBytes()), 0);
				unpalettedBitmap = pvrt.ToBitmap();
			}
			else if (tex is GvrTextureInfo gvri)
			{
				GvrTexture gvrt = new GvrTexture(gvri.TextureData.ToArray());
				defaultPalette.IsGVP = true;
				gvrt.SetPalette(new GvpPalette(defaultPalette.GetBytes()), 0);
				unpalettedBitmap = gvrt.ToBitmap();
			}
			else return tex.Image;
			TexturePalette applyPalette;
			int applySet;

			// Set the default palette if exporting mask, otherwise use current palette and bank
			if (exportMaskToolStripMenuItem.Checked)
			{
				applyPalette = defaultPalette;
				applySet = 0;
			}
			else
			{
				applyPalette = currentPalette;
				applySet = paletteSet;
			}

			// Palettize
			Bitmap result;
			int setsize = 16;
			switch (indxfmt)
			{
				case PalettedTextureFormat.Index4:
					result = new Bitmap(unpalettedBitmap.Width, unpalettedBitmap.Height, PixelFormat.Format4bppIndexed);
					break;
				case PalettedTextureFormat.Index8:
					setsize = 256;
					result = new Bitmap(unpalettedBitmap.Width, unpalettedBitmap.Height, PixelFormat.Format8bppIndexed);
					break;
				default:
					return unpalettedBitmap;
			}
			Color[] defaultpaletteSet = defaultPalette.Colors.ToArray();
			List<Color> usedPaletteSet = new List<Color>();
			for (int i = 0; i < setsize; i++)
				usedPaletteSet.Add(applyPalette.Colors[applySet * setsize + i]);

			// Replace the palette in the Bitmap
			var newAliasForPalette = result.Palette;
			for (int i = 0; i < usedPaletteSet.Count; i++)
			{
				newAliasForPalette.Entries[i] = usedPaletteSet[i];
			}
			result.Palette = newAliasForPalette;

			// Set pixels in the indexed image
			int remaining = unpalettedBitmap.Height * unpalettedBitmap.Width;
			using (var snoop_u = new BmpPixelSnoop(new Bitmap(unpalettedBitmap)))
			{
				for (int y = 0; y < unpalettedBitmap.Height; y++)
					for (int x = 0; x < unpalettedBitmap.Width; x++)
					{
						for (int p = 0; p < setsize; p++)
						{
							if (snoop_u.GetPixel(x, y) == defaultpaletteSet[p])
							{
								remaining--;
								TextureFunctions.SetPixelIndex(result, x, y, p);
								break;
							}
						}
					}
			}
			if (remaining > 0)
				System.Windows.Forms.MessageBox.Show(remaining.ToString() + " pixels were not found in the palette.",
					"Texture Editor Export Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
			if (exportPalettedFullToolStripMenuItem.Checked)
				return new Bitmap(result);
			else
				return result;
		}

		private void RetrievePaletteFromIndexedBitmap(Bitmap bitmap, PalettedTextureFormat texfmt, PalettePixelCodec codec, bool gvp = false)
		{
			if (bitmap.PixelFormat != PixelFormat.Format4bppIndexed && bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
				return;

			currentPalette.PixelCodec = codec;
			currentPalette.Colors.Clear();

			for (int c = 0; c < bitmap.Palette.Entries.Length; c++)
				currentPalette.Colors.Add(bitmap.Palette.Entries[c]);

			currentPalette.IsGVP = defaultPalette.IsGVP = gvp;

			// Add more colors if an Index4 image was imported as Index8
			int rowsize = texfmt == PalettedTextureFormat.Index8 ? 256 : 16;
			if (currentPalette.Colors.Count < rowsize)
				do
					currentPalette.Colors.Add(Color.Black);
				while (currentPalette.Colors.Count < rowsize);

			// Refresh bank preview
			int rowsize_stored = rowsize - currentPalette.StartColor;
			int numrows = currentPalette.Colors.Count / rowsize_stored;
			comboBoxCurrentPaletteBank.Items.Clear();
			for (int i = 0; i < numrows; i++)
				comboBoxCurrentPaletteBank.Items.Add((currentPalette.StartBank + i).ToString());
			comboBoxCurrentPaletteBank.SelectedIndex = paletteSet;
			paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
		}

		private Bitmap RetrieveMaskFromIndexedBitmap(Bitmap bitmap, PalettedTextureFormat palettedTextureFormat)
		{
			PixelFormat pxfmt = PixelFormat.Format4bppIndexed;
			int stepsize = 16;
			if (palettedTextureFormat == PalettedTextureFormat.Index8)
			{
				stepsize = 256;
				pxfmt = PixelFormat.Format8bppIndexed;
			}
			Bitmap result = new Bitmap(bitmap.Width, bitmap.Height, pxfmt);
			using (var snoop_b = new BmpPixelSnoop(new Bitmap(bitmap)))
			{
				for (int y = 0; y < bitmap.Height; y++)
					for (int x = 0; x < bitmap.Width; x++)
					{
						for (int p = 0; p < stepsize; p++)
						{
							if (snoop_b.GetPixel(x, y) == currentPalette.Colors[p])
							{
								TextureFunctions.SetPixelIndex(result, x, y, p);
								break;
							}
						}
					}
			}
			// Replace the palette in the Bitmap
			var newAliasForPalette = result.Palette;
			for (int i = 0; i < currentPalette.Colors.Count; i++)
			{
				newAliasForPalette.Entries[i] = defaultPalette.Colors[i];
			}
			result.Palette = newAliasForPalette;
			return result;
		}

		private Bitmap GeneratePalettePreview()
		{
			int rows = currentPalette.Colors.Count / 16;
			Bitmap result = new Bitmap(256 + 16, rows * 16 + rows);
			using (var snoop_r = new BmpPixelSnoop(result))
			{
				int offset_y = 0;
				for (int y = 0; y < rows; y++)
				{
					int offset_x = 0;
					for (int x = 0; x < 16; x++)
					{
						// Draw a 16x16 square
						for (int z = 0; z < 16; z++)
							for (int h = 0; h < 16; h++)
								snoop_r.SetPixel(offset_x + x * 16 + z, offset_y + y * 16 + h, currentPalette.Colors[y * 16 + x]);
						offset_x += 1;
					}
					offset_y += 1;
				}
			}
			return result;
		}
		#endregion

		#region Menus

		private void addMipmapsToAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (TextureInfo info in textures)
				if (info.CheckMipmap())
				{
					info.Mipmap = true;
					if (info is PakTextureInfo pk)
						pk.SurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
				}
			if (listBox1.SelectedIndex != -1 && textures[listBox1.SelectedIndex].CheckMipmap())
				mipmapCheckBox.Checked = true;
			unsaved = true;
		}

		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LoadArchive(Settings.MRUList[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (currentTextureID != listBox1.SelectedIndex)
			{
				currentTextureID = listBox1.SelectedIndex;
				paletteApplied = false;
				UpdateTextureInformation();
			}
		}

		private void highQualityGVMsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.HighQualityGVM = highQualityGVMsToolStripMenuItem.Checked;
		}

		private void usePNGInsteadOfDDSToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.UsePNGforPAK = usePNGInsteadOfDDSToolStripMenuItem.Checked;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.Save();
			if (unsaved)
			{
				DialogResult res = MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (res)
				{
					case DialogResult.Yes:
						if (!string.IsNullOrEmpty(archiveFilename))
							SaveTextures();
						else
							SaveAs(currentFormat);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
					case DialogResult.No:
						break;
				}
			}
			try
			{
				settingsfile.Save();
			}
			catch { };
		}

		private void numericUpDownOrigSizeX_ValueChanged(object sender, EventArgs e)
		{
			if (!suppress && textures[listBox1.SelectedIndex] is PvmxTextureInfo tex)
			{
				if (!tex.Dimensions.HasValue || (tex.Dimensions.Value.Width != (int)numericUpDownOrigSizeX.Value || tex.Dimensions.Value.Height != (int)numericUpDownOrigSizeY.Value))
				{
					tex.Dimensions = new Size((int)numericUpDownOrigSizeX.Value, (int)numericUpDownOrigSizeY.Value);
					unsaved = true;
				}
			}
		}

		private void numericUpDownOrigSizeY_ValueChanged(object sender, EventArgs e)
		{
			if (!suppress && textures[listBox1.SelectedIndex] is PvmxTextureInfo tex)
			{
				if (!tex.Dimensions.HasValue || (tex.Dimensions.Value.Width != (int)numericUpDownOrigSizeX.Value || tex.Dimensions.Value.Height != (int)numericUpDownOrigSizeY.Value))
				{
					tex.Dimensions = new Size((int)numericUpDownOrigSizeX.Value, (int)numericUpDownOrigSizeY.Value);
					unsaved = true;
				}
			}
		}

		private void checkBoxPAKUseAlpha_CheckedChanged(object sender, EventArgs e)
		{
			if (!(textures[listBox1.SelectedIndex] is PakTextureInfo))
				return;
			PakTextureInfo pk = (PakTextureInfo)textures[listBox1.SelectedIndex];
			// Use alpha
			if (checkBoxPAKUseAlpha.Checked)
				pk.DataFormat = GvrDataFormat.Rgb5a3;
			// Don't use alpha (palettized)
			else if ((pk.SurfaceFlags & NinjaSurfaceFlags.Palettized) != 0)
				pk.DataFormat = GvrDataFormat.Index4;
			// Don't use alpha (regular)
			else
				pk.DataFormat = GvrDataFormat.Dxt1;
			UpdateTextureInformation();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (archiveFilename == null)
				saveAsToolStripMenuItem_Click(sender, e);
			else
				SaveTextures();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			fileToolStripMenuItem.HideDropDown();
			SaveAs(currentFormat);
		}

		private void saveAsPVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureFormat.PVM);
		}

		private void saveAsGVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureFormat.GVM);
		}

		private void saveAsPVMXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureFormat.PVMX);
		}

		private void saveAsPAKToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureFormat.PAK);
		}

		private void saveXVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureFormat.XVM);
		}

		private void SplitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
		{
			// Maybe not necessary?
			if (listBox1.SelectedIndex > -1)
				UpdateTextureView();
		}

		private void newPVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureFormat.PVM);
			listBox1_SelectedIndexChanged(sender, e);
		}

		private void newGVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureFormat.GVM);
			listBox1_SelectedIndexChanged(sender, e);
		}

		private void newXVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureFormat.XVM);
			listBox1_SelectedIndexChanged(sender, e);
		}

		private void newPVMXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureFormat.PVMX);
			listBox1_SelectedIndexChanged(sender, e);
		}

		private void newPAKToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureFormat.PAK);
			listBox1_SelectedIndexChanged(sender, e);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (unsaved)
			{
				DialogResult res = MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (res)
				{
					case DialogResult.Yes:
						SaveTextures();
						break;
					case DialogResult.Cancel:
						return;
					case DialogResult.No:
						break;
				}
			}
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.xvm;*.prs;*.pvmx;*.pak" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					LoadArchive(dlg.FileName);
					listBox1_SelectedIndexChanged(sender, e);
					unsaved = false;
				}
		}

		private void enablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PAKEnableAlphaForAll(true);
		}

		private void disablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PAKEnableAlphaForAll(false);
		}

		private void checkBoxPAKUseAlpha_Click(object sender, EventArgs e)
		{
			checkBoxPAKUseAlpha_CheckedChanged(sender, e);
			unsaved = true;
		}

		private void texturePreviewZoomTrackBar_Scroll(object sender, EventArgs e)
		{
			UpdateTextureInformation();
		}

		private void textureFilteringToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UpdateTextureInformation();
		}

		private void textureFilteringToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.EnableFiltering = textureFilteringToolStripMenuItem.Checked;
		}

		private void compatibleGVPToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.SACompatiblePalettes = compatibleGVPToolStripMenuItem.Checked;
		}
		#endregion

		private void generateNewGbixToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (textures.Count < 1)
				return;

			DialogResult res = MessageBox.Show(this, "This will generate new GBIX for every texture, are you sure you wish to continue?", "Warning Gbix Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
			
			if (res != DialogResult.Yes)
			{
				return; 
			}

			Random random = new();
			uint randomNumber = (uint)random.Next(100000, 9999999);

			foreach (var texture in textures)
			{
				texture.GlobalIndex = randomNumber;
				randomNumber++;
			}

			listBox1.SelectedIndex = -1;
			listBox1.SelectedItem = null;
			unsaved = true;
		}
	}
}