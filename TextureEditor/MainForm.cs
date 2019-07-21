using PAKLib;
using PuyoTools.Modules.Archive;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VrSharp.GvrTexture;
using VrSharp.PvrTexture;
using Bitmap = System.Drawing.Bitmap;

namespace TextureEditor
{
	public partial class MainForm : Form
	{
		readonly Properties.Settings Settings = Properties.Settings.Default;

		public MainForm()
		{
			InitializeComponent();
		}

		TextureFormat format;
		string filename;
		List<TextureInfo> textures = new List<TextureInfo>();

		private void SetFilename(string filename)
		{
			this.filename = filename;
			if (Settings.MRUList.Contains(filename))
			{
				int i = Settings.MRUList.IndexOf(filename);
				Settings.MRUList.RemoveAt(i);
				recentFilesToolStripMenuItem.DropDownItems.RemoveAt(i);
			}
			Settings.MRUList.Insert(0, filename);
			recentFilesToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename.Replace("&", "&&")));
			Text = format.ToString() + " Editor - " + filename;
		}

		private void UpdateTextureCount()
		{
			if (textures.Count == 1)
				toolStripStatusLabel1.Text = "1 texture";
			else
				toolStripStatusLabel1.Text = textures.Count + " textures";
		}

		private bool GetTextures(string filename)
		{
			byte[] pvmdata = File.ReadAllBytes(filename);
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
			ArchiveBase pvmfile = new PvmArchive();
			List<TextureInfo> newtextures;
			if (PvmxArchive.Is(pvmdata))
			{
				format = TextureFormat.PVMX;
				newtextures = new List<TextureInfo>(PvmxArchive.GetTextures(pvmdata));
			}
			else if (PAKFile.Is(filename))
			{
				format = TextureFormat.PAK;
				PAKFile pak = new PAKFile(filename);
				string filenoext = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
				byte[] inf = pak.Files.Single((file) => file.Name.Equals(filenoext + '\\' + filenoext + ".inf", StringComparison.OrdinalIgnoreCase)).Data;
				newtextures = new List<TextureInfo>(inf.Length / 0x3C);
				using (var factory = new ImagingFactory2())
					for (int i = 0; i < inf.Length; i += 0x3C)
					{
						StringBuilder sb = new StringBuilder(0x1C);
						for (int j = 0; j < 0x1C; j++)
							if (inf[i + j] != 0)
								sb.Append((char)inf[i + j]);
							else
								break;
						byte[] dds = pak.Files.First((file) => file.Name.Equals(filenoext + '\\' + sb.ToString() + ".dds", StringComparison.OrdinalIgnoreCase)).Data;
						Bitmap bmp;
						using (MemoryStream str = new MemoryStream(dds))
						using (var decoder = new BitmapDecoder(factory, str, DecodeOptions.CacheOnDemand))
						using (var converter = new FormatConverter(factory))
						{
							converter.Initialize(decoder.GetFrame(0), PixelFormat.Format32bppBGRA, BitmapDitherType.None, null, 0, BitmapPaletteType.Custom);
							bmp = new Bitmap(converter.Size.Width, converter.Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
							System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
							byte[] pix = new byte[Math.Abs(data.Stride) * data.Height];
							converter.CopyPixels(pix, data.Stride);
							System.Runtime.InteropServices.Marshal.Copy(pix, 0, data.Scan0, pix.Length);
							bmp.UnlockBits(data);
						}
						newtextures.Add(new PakTextureInfo(sb.ToString(), BitConverter.ToUInt32(inf, i + 0x1C), new Bitmap(bmp)));
					}
			}
			else
			{
				if (pvmfile.Is(pvmdata, filename))
					format = TextureFormat.PVM;
				else
				{
					pvmfile = new GvmArchive();
					if (!pvmfile.Is(pvmdata, filename))
					{
						MessageBox.Show(this, "Could not open file \"" + filename + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
					format = TextureFormat.GVM;
				}
				ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
				newtextures = new List<TextureInfo>(pvmentries.Count);
				switch (format)
				{
					case TextureFormat.PVM:
						PvpPalette pvp = null;
						foreach (ArchiveEntry file in pvmentries)
						{
							PvrTexture vrfile = new PvrTexture(file.Open());
							if (vrfile.NeedsExternalPalette)
							{
								if (pvp == null)
									using (OpenFileDialog a = new OpenFileDialog
									{
										DefaultExt = "pvp",
										Filter = "PVP Files|*.pvp",
										InitialDirectory = Path.GetDirectoryName(filename),
										Title = "External palette file"
									})
										if (a.ShowDialog(this) == DialogResult.OK)
											pvp = new PvpPalette(a.FileName);
										else
										{
											MessageBox.Show(this, "Could not open file \"" + Program.Arguments[0] + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
											return false;
										}
								vrfile.SetPalette(pvp);
							}
							newtextures.Add(new PvrTextureInfo(Path.GetFileNameWithoutExtension(file.Name), vrfile));
						}
						break;
					case TextureFormat.GVM:
						GvpPalette gvp = null;
						foreach (ArchiveEntry file in pvmentries)
						{
							GvrTexture vrfile = new GvrTexture(file.Open());
							if (vrfile.NeedsExternalPalette)
							{
								if (gvp == null)
									using (OpenFileDialog a = new OpenFileDialog
									{
										DefaultExt = "gvp",
										Filter = "GVP Files|*.gvp",
										InitialDirectory = Path.GetDirectoryName(filename),
										Title = "External palette file"
									})
										if (a.ShowDialog(this) == DialogResult.OK)
											gvp = new GvpPalette(a.FileName);
										else
										{
											MessageBox.Show(this, "Could not open file \"" + Program.Arguments[0] + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
											return false;
										}
								vrfile.SetPalette(gvp);
							}
							newtextures.Add(new GvrTextureInfo(Path.GetFileNameWithoutExtension(file.Name), vrfile));
						}
						break;
				}
			}
			textures.Clear();
			textures.AddRange(newtextures);
			listBox1.Items.Clear();
			listBox1.Items.AddRange(textures.Select((item) => item.Name).ToArray());
			UpdateTextureCount();
			SetFilename(Path.GetFullPath(filename));
			return true;
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
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

			if (Program.Arguments.Length > 0 && !GetTextures(Program.Arguments[0]))
				this.Close();
		}

		private void newPVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textures.Clear();
			listBox1.Items.Clear();
			filename = null;
			format = TextureFormat.PVM;
			Text = "PVM Editor";
			UpdateTextureCount();
		}

		private void newGVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textures.Clear();
			listBox1.Items.Clear();
			filename = null;
			format = TextureFormat.GVM;
			Text = "GVM Editor";
			UpdateTextureCount();
		}

		private void newPVMXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textures.Clear();
			listBox1.Items.Clear();
			filename = null;
			format = TextureFormat.PVMX;
			Text = "PVMX Editor";
			UpdateTextureCount();
		}

		private void newPAKToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textures.Clear();
			listBox1.Items.Clear();
			filename = null;
			format = TextureFormat.PAK;
			Text = "PAK Editor";
			UpdateTextureCount();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs;*.pvmx;*.pak" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					GetTextures(dlg.FileName);
					listBox1_SelectedIndexChanged(sender, e);
				}
		}

		private void SaveTextures()
		{
			byte[] data;
			using (MemoryStream str = new MemoryStream())
			{
				ArchiveWriter writer = null;
				switch (format)
				{
					case TextureFormat.PVM:
						writer = new PvmArchiveWriter(str);
						foreach (PvrTextureInfo tex in textures)
						{
							if (tex.DataFormat != PvrDataFormat.Index4 && tex.DataFormat != PvrDataFormat.Index8)
							{
								System.Drawing.Imaging.BitmapData bmpd = tex.Image.LockBits(new Rectangle(Point.Empty, tex.Image.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
								int stride = bmpd.Stride;
								byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
								System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
								tex.Image.UnlockBits(bmpd);
								int tlevels = 0;
								for (int y = 0; y < tex.Image.Height; y++)
								{
									int srcaddr = y * Math.Abs(stride);
									for (int x = 0; x < tex.Image.Width; x++)
									{
										Color c = Color.FromArgb(BitConverter.ToInt32(bits, srcaddr + (x * 4)));
										if (c.A == 0)
											tlevels = 1;
										else if (c.A < 255)
										{
											tlevels = 2;
											break;
										}
									}
									if (tlevels == 2)
										break;
								}
								if (tlevels == 0)
									tex.PixelFormat = PvrPixelFormat.Rgb565;
								else if (tlevels == 1)
									tex.PixelFormat = PvrPixelFormat.Argb1555;
								else if (tlevels == 2)
									tex.PixelFormat = PvrPixelFormat.Argb4444;
								if (tex.Image.Width == tex.Image.Height)
									if (tex.Mipmap)
										tex.DataFormat = PvrDataFormat.SquareTwiddledMipmaps;
									else
										tex.DataFormat = PvrDataFormat.SquareTwiddled;
								else
									tex.DataFormat = PvrDataFormat.Rectangle;
							}
							PvrTextureEncoder encoder = new PvrTextureEncoder(tex.Image, tex.PixelFormat, tex.DataFormat);
							encoder.GlobalIndex = tex.GlobalIndex;
							MemoryStream pvr = new MemoryStream();
							encoder.Save(pvr);
							pvr.Seek(0, SeekOrigin.Begin);
							writer.CreateEntry(pvr, tex.Name);
						}
						break;
					case TextureFormat.GVM:
						writer = new GvmArchiveWriter(str);
						foreach (GvrTextureInfo tex in textures)
						{
							if (tex.DataFormat != GvrDataFormat.Index4 && tex.DataFormat != GvrDataFormat.Index8)
							{
								System.Drawing.Imaging.BitmapData bmpd = tex.Image.LockBits(new Rectangle(Point.Empty, tex.Image.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
								int stride = bmpd.Stride;
								byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
								System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
								tex.Image.UnlockBits(bmpd);
								int tlevels = 0;
								for (int y = 0; y < tex.Image.Height; y++)
								{
									int srcaddr = y * Math.Abs(stride);
									for (int x = 0; x < tex.Image.Width; x++)
									{
										Color c = Color.FromArgb(BitConverter.ToInt32(bits, srcaddr + (x * 4)));
										if (c.A == 0)
											tlevels = 1;
										else if (c.A < 255)
										{
											tlevels = 2;
											break;
										}
									}
									if (tlevels == 2)
										break;
								}
								if (!tex.Mipmap)
									tex.DataFormat = GvrDataFormat.Argb8888;
								else if (tlevels == 0)
									tex.DataFormat = GvrDataFormat.Rgb565;
								else
									tex.DataFormat = GvrDataFormat.Rgb5a3;
							}
							GvrTextureEncoder encoder = new GvrTextureEncoder(tex.Image, tex.PixelFormat, tex.DataFormat);
							encoder.GlobalIndex = tex.GlobalIndex;
							MemoryStream gvr = new MemoryStream();
							encoder.Save(gvr);
							gvr.Seek(0, SeekOrigin.Begin);
							writer.CreateEntry(gvr, tex.Name);
						}
						break;
					case TextureFormat.PVMX:
						PvmxArchive.Save(str, textures.Cast<PvmxTextureInfo>());
						break;
					case TextureFormat.PAK:
						PAKFile pak = new PAKFile();
						string filenoext = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
						string longdir = "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
						List<byte> inf = new List<byte>(textures.Count * 0x3C);
						using (var factory = new ImagingFactory2())
							foreach (TextureInfo item in textures)
							{
								byte[] tb;
								using (MemoryStream ms = new MemoryStream())
								using (var encoder = new BitmapEncoder(factory, ContainerFormatGuids.Dds))
								{
									encoder.Initialize(ms);
									using (var frame = new BitmapFrameEncode(encoder))
									{
										frame.Initialize();
										frame.SetSize(item.Image.Width, item.Image.Height);
										Guid fdc = PixelFormat.Format32bppBGRA;
										frame.SetPixelFormat(ref fdc);
										Bitmap img = item.Image;
										if (img.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
											img = new Bitmap(img);
										System.Drawing.Imaging.BitmapData bmpdata = item.Image.LockBits(new Rectangle(0, 0, item.Image.Width, item.Image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
										frame.WritePixels(bmpdata.Height, bmpdata.Scan0, bmpdata.Stride);
										frame.Commit();
									}
									encoder.Commit();
									tb = ms.ToArray();
								}
								string name = item.Name.ToLowerInvariant();
								if (name.Length > 0x1C)
									name = name.Substring(0, 0x1C);
								pak.Files.Add(new PAKFile.File(filenoext + '\\' + name + ".dds", longdir + '\\' + name + ".dds", tb));
								inf.AddRange(Encoding.ASCII.GetBytes(name));
								if (name.Length != 0x1C)
									inf.AddRange(new byte[0x1C - name.Length]);
								inf.AddRange(BitConverter.GetBytes(item.GlobalIndex));
								inf.AddRange(new byte[0xC]);
								inf.AddRange(BitConverter.GetBytes(item.Image.Width));
								inf.AddRange(BitConverter.GetBytes(item.Image.Height));
								inf.AddRange(new byte[4]);
								inf.AddRange(BitConverter.GetBytes(0x80000000));
							}
						pak.Files.Insert(0, new PAKFile.File(filenoext + '\\' + filenoext + ".inf", longdir + '\\' + filenoext + ".inf", inf.ToArray()));
						pak.Save(filename);

						return;
				}
				writer?.Flush();
				data = str.ToArray();
				str.Close();
			}
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				FraGag.Compression.Prs.Compress(data, filename);
			else
				File.WriteAllBytes(filename, data);
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (filename == null)
				saveAsToolStripMenuItem_Click(sender, e);
			else
				SaveTextures();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string defext = null;
			string filter = null;
			switch (format)
			{
				case TextureFormat.PVM:
					defext = "pvm";
					filter = "PVM Files|*.pvm;*.prs";
					break;
				case TextureFormat.GVM:
					defext = "gvm";
					filter = "GVM Files|*.gvm;*.prs";
					break;
				case TextureFormat.PVMX:
					defext = "pvmx";
					filter = "PVMX Files|*.pvmx";
					break;
				case TextureFormat.PAK:
					defext = "pak";
					filter = "PAK Files|*.pak";
					break;
			}
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = defext, Filter = filter })
			{
				if (filename != null)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(filename);
					dlg.FileName = Path.GetFileName(filename);
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					SetFilename(dlg.FileName);
					SaveTextures();
				}
			}
		}

		private void saveAsPVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "pvm", Filter = "PVM Files|*.pvm;*.prs" })
			{
				if (filename != null)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(filename);
					dlg.FileName = Path.ChangeExtension(Path.GetFileName(filename), "pvm");
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					switch (format)
					{
						case TextureFormat.GVM:
							textures = new List<TextureInfo>(textures.Cast<GvrTextureInfo>().Select(a => new PvrTextureInfo(a)));
							break;
						case TextureFormat.PVMX:
						case TextureFormat.PAK:
							textures = new List<TextureInfo>(textures.Select(a => new PvrTextureInfo(a)));
							break;
					}
					format = TextureFormat.PVM;
					SetFilename(dlg.FileName);
					SaveTextures();
				}
			}
		}

		private void saveAsGVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "gvm", Filter = "GVM Files|*.gvm;*.prs" })
			{
				if (filename != null)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(filename);
					dlg.FileName = Path.ChangeExtension(Path.GetFileName(filename), "gvm");
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					switch (format)
					{
						case TextureFormat.PVM:
							textures = new List<TextureInfo>(textures.Cast<PvrTextureInfo>().Select(a => new GvrTextureInfo(a)));
							break;
						case TextureFormat.PVMX:
						case TextureFormat.PAK:
							textures = new List<TextureInfo>(textures.Select(a => new GvrTextureInfo(a)));
							break;
					}
					format = TextureFormat.GVM;
					SetFilename(dlg.FileName);
					SaveTextures();
				}
			}
		}

		private void saveAsPVMXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "pvmx", Filter = "PVMX Files|*.pvmx" })
			{
				if (filename != null)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(filename);
					dlg.FileName = Path.ChangeExtension(Path.GetFileName(filename), "pvmx");
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					switch (format)
					{
						case TextureFormat.PVM:
						case TextureFormat.GVM:
						case TextureFormat.PAK:
							textures = new List<TextureInfo>(textures.Select(a => new PvmxTextureInfo(a)));
							break;
					}
					format = TextureFormat.PVMX;
					SetFilename(dlg.FileName);
					SaveTextures();
				}
			}
		}

		private void saveAsPAKToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "pak", Filter = "PAK Files|*.pak" })
			{
				if (filename != null)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(filename);
					dlg.FileName = Path.ChangeExtension(Path.GetFileName(filename), "pak");
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					switch (format)
					{
						case TextureFormat.PVM:
						case TextureFormat.GVM:
						case TextureFormat.PVMX:
							textures = new List<TextureInfo>(textures.Select(a => new PakTextureInfo(a)));
							break;
					}
					format = TextureFormat.PAK;
					SetFilename(dlg.FileName);
					SaveTextures();
				}
			}
		}

		private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dlg = new FolderBrowserDialog())
			{
				if (filename != null)
					dlg.SelectedPath = Path.GetDirectoryName(filename);

				if (dlg.ShowDialog(this) == DialogResult.OK)
					using (TextWriter texList = File.CreateText(Path.Combine(dlg.SelectedPath, "index.txt")))
						foreach (TextureInfo tex in textures)
						{
							tex.Image.Save(Path.Combine(dlg.SelectedPath, tex.Name + ".png"));
							if (tex is PvmxTextureInfo xtex && xtex.Dimensions.HasValue)
								texList.WriteLine("{0},{1},{2}x{3}", xtex.GlobalIndex, xtex.Name + ".png", xtex.Dimensions.Value.Width, xtex.Dimensions.Value.Height);
							else
								texList.WriteLine("{0},{1}", tex.GlobalIndex, tex.Name + ".png");
						}
			}
		}

		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			GetTextures(Settings.MRUList[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool en = listBox1.SelectedIndex != -1;
			removeTextureButton.Enabled = textureName.Enabled = globalIndex.Enabled = importButton.Enabled = exportButton.Enabled = en;
			if (en)
			{
				textureName.Text = textures[listBox1.SelectedIndex].Name;
				globalIndex.Value = textures[listBox1.SelectedIndex].GlobalIndex;
				if (textures[listBox1.SelectedIndex].CheckMipmap())
				{
					mipmapCheckBox.Enabled = true;
					mipmapCheckBox.Checked = textures[listBox1.SelectedIndex].Mipmap;
				}
				else
					mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
				textureImage.Image = textures[listBox1.SelectedIndex].Image;
			}
			else
				mipmapCheckBox.Enabled = false;
		}

		private KeyValuePair<string, Bitmap>? BrowseForTexture(string textureName = null)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvr", Filter = "Texture Files|*.pvr;*.gvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp" })
			{
				if (!String.IsNullOrEmpty(textureName))
					dlg.FileName = textureName;

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					string name = Path.GetFileNameWithoutExtension(dlg.FileName);
					if (PvrTexture.Is(dlg.FileName))
						return new KeyValuePair<string, Bitmap>(name, new PvrTexture(dlg.FileName).ToBitmap());
					else if (GvrTexture.Is(dlg.FileName))
						return new KeyValuePair<string, Bitmap>(name, new GvrTexture(dlg.FileName).ToBitmap());
					else
						return new KeyValuePair<string, Bitmap>(name, new Bitmap(dlg.FileName));
				}
				else
				{
					return null;
				}
			}
		}

		private void addTextureButton_Click(object sender, EventArgs e)
		{
			string defext = null;
			string filter = null;
			switch (format)
			{
				case TextureFormat.PVM:
					defext = "pvr";
					filter = "Texture Files|*.prs;*.pvm;*.pvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp";
					break;
				case TextureFormat.GVM:
					defext = "gvr";
					filter = "Texture Files|*.prs;*.gvm;*.gvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp";
					break;
				case TextureFormat.PVMX:
				case TextureFormat.PAK:
					defext = "png";
					filter = "Texture Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp";
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
							case ".prs":
							case ".pvm":
							case ".gvm":
								byte[] pvmdata = File.ReadAllBytes(file);
								if (Path.GetExtension(file).Equals(".prs", StringComparison.OrdinalIgnoreCase))
									pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
								ArchiveBase pvmfile = null;
								switch (format)
								{
									case TextureFormat.PVM:
										pvmfile = new PvmArchive();
										break;
									case TextureFormat.GVM:
										pvmfile = new GvmArchive();
										break;
								}
								if (!pvmfile.Is(pvmdata, file))
								{
									MessageBox.Show(this, "Could not open file \"" + file + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
									continue;
								}
								ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
								List<PvrTextureInfo> newtextures = new List<PvrTextureInfo>(pvmentries.Count);
								switch (format)
								{
									case TextureFormat.PVM:
										PvpPalette pvp = null;
										foreach (ArchiveEntry entry in pvmentries)
										{
											PvrTexture vrfile = new PvrTexture(entry.Open());
											if (vrfile.NeedsExternalPalette)
											{
												if (pvp == null)
													using (OpenFileDialog a = new OpenFileDialog
													{
														DefaultExt = "pvp",
														Filter = "PVP Files|*.pvp",
														InitialDirectory = Path.GetDirectoryName(file),
														Title = "External palette file"
													})
														if (a.ShowDialog(this) == DialogResult.OK)
															pvp = new PvpPalette(a.FileName);
														else
														{
															MessageBox.Show(this, "Could not open file \"" + Program.Arguments[0] + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
															continue;
														}
												vrfile.SetPalette(pvp);
											}
											string name = Path.GetFileNameWithoutExtension(entry.Name);
											textures.Add(new PvrTextureInfo(name, vrfile));
											listBox1.Items.Add(name);
										}
										break;
									case TextureFormat.GVM:
										GvpPalette gvp = null;
										foreach (ArchiveEntry entry in pvmentries)
										{
											GvrTexture vrfile = new GvrTexture(entry.Open());
											if (vrfile.NeedsExternalPalette)
											{
												if (gvp == null)
													using (OpenFileDialog a = new OpenFileDialog
													{
														DefaultExt = "gvp",
														Filter = "GVP Files|*.gvp",
														InitialDirectory = Path.GetDirectoryName(file),
														Title = "External palette file"
													})
														if (a.ShowDialog(this) == DialogResult.OK)
															gvp = new GvpPalette(a.FileName);
														else
														{
															MessageBox.Show(this, "Could not open file \"" + Program.Arguments[0] + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
															continue;
														}
												vrfile.SetPalette(gvp);
											}
											string name = Path.GetFileNameWithoutExtension(entry.Name);
											textures.Add(new GvrTextureInfo(name, vrfile));
											listBox1.Items.Add(name);
										}
										break;
								}
								break;
							default:
								{
									string name = Path.GetFileNameWithoutExtension(file);
									if (format == TextureFormat.PVM && PvrTexture.Is(file))
										textures.Add(new PvrTextureInfo(name, new PvrTexture(file)));
									else if (format == TextureFormat.GVM && GvrTexture.Is(file))
										textures.Add(new GvrTextureInfo(name, new GvrTexture(file)));
									else
									{
										switch (format)
										{
											case TextureFormat.PVM:
												textures.Add(new PvrTextureInfo(name, gbix, new Bitmap(file)));
												break;
											case TextureFormat.GVM:
												textures.Add(new GvrTextureInfo(name, gbix, new Bitmap(file)));
												break;
											case TextureFormat.PVMX:
												textures.Add(new PvmxTextureInfo(name, gbix, new Bitmap(file)));
												break;
											case TextureFormat.PAK:
												textures.Add(new PakTextureInfo(name, gbix, new Bitmap(file)));
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
					listBox1.SelectedIndex = textures.Count - 1;
					UpdateTextureCount();
				}
			}
		}

		private void removeTextureButton_Click(object sender, EventArgs e)
		{
			int i = listBox1.SelectedIndex;
			textures.RemoveAt(i);
			listBox1.Items.RemoveAt(i);
			UpdateTextureCount();
		}

		private void textureName_TextChanged(object sender, EventArgs e)
		{
			bool focus = textureName.Focused;
			string name = textureName.Text;
			foreach (char c in Path.GetInvalidFileNameChars())
				name = name.Replace(c, '_');
			listBox1.Items[listBox1.SelectedIndex] = textureName.Text = textures[listBox1.SelectedIndex].Name = name;
			if (focus)
				textureName.Focus();
		}

		private void globalIndex_ValueChanged(object sender, EventArgs e)
		{
			textures[listBox1.SelectedIndex].GlobalIndex = (uint)globalIndex.Value;
		}

		private void mipmapCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			textures[listBox1.SelectedIndex].Mipmap = mipmapCheckBox.Checked;
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			KeyValuePair<string, Bitmap>? tex = BrowseForTexture(listBox1.GetItemText(listBox1.SelectedItem));
			if (tex.HasValue)
			{
				textureImage.Image = textures[listBox1.SelectedIndex].Image = tex.Value.Value;
				if (textures[listBox1.SelectedIndex].CheckMipmap())
				{
					mipmapCheckBox.Enabled = true;
					mipmapCheckBox.Checked = textures[listBox1.SelectedIndex].Mipmap;
				}
				else
					mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
			}

			listBox1.Select();
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", FileName = textures[listBox1.SelectedIndex].Name + ".png", Filter = "PNG Files|*.png" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
					textures[listBox1.SelectedIndex].Image.Save(dlg.FileName);

			listBox1.Select();
		}

		private void addMipmapsToAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (TextureInfo info in textures)
				if (info.CheckMipmap())
					info.Mipmap = true;
			if (listBox1.SelectedIndex != -1 && textures[listBox1.SelectedIndex].CheckMipmap())
				mipmapCheckBox.Checked = true;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.Save();
		}
	}

	abstract class TextureInfo
	{
		public string Name { get; set; }
		public uint GlobalIndex { get; set; }
		public bool Mipmap { get; set; }
		public Bitmap Image { get; set; }

		public abstract bool CheckMipmap();
	}

	class PvrTextureInfo : TextureInfo
	{
		public PvrDataFormat DataFormat { get; set; }
		public PvrPixelFormat PixelFormat { get; set; }

		public PvrTextureInfo() { }

		public PvrTextureInfo(TextureInfo tex)
		{
			Name = tex.Name;
			GlobalIndex = tex.GlobalIndex;
			Image = tex.Image;
			Mipmap = tex.Mipmap;
			PixelFormat = PvrPixelFormat.Unknown;
			DataFormat = PvrDataFormat.Unknown;
		}

		public PvrTextureInfo(GvrTextureInfo tex)
			: this((TextureInfo)tex)
		{
			switch (tex.DataFormat)
			{
				case GvrDataFormat.Index4:
					DataFormat = PvrDataFormat.Index4;
					break;
				case GvrDataFormat.Index8:
					DataFormat = PvrDataFormat.Index8;
					break;
			}
		}

		public PvrTextureInfo(PvrTextureInfo tex)
			: this((TextureInfo)tex)
		{
			PixelFormat = tex.PixelFormat;
			DataFormat = tex.DataFormat;
		}

		public PvrTextureInfo(string name, uint gbix, Bitmap bitmap)
		{
			Name = name;
			GlobalIndex = gbix;
			DataFormat = PvrDataFormat.Unknown;
			PixelFormat = PvrPixelFormat.Unknown;
			Image = bitmap;
		}

		public PvrTextureInfo(string name, PvrTexture texture)
		{
			Name = name;
			GlobalIndex = texture.GlobalIndex;
			DataFormat = texture.DataFormat;
			Mipmap = DataFormat == PvrDataFormat.SquareTwiddledMipmaps || DataFormat == PvrDataFormat.SquareTwiddledMipmapsAlt;
			PixelFormat = texture.PixelFormat;
			Image = texture.ToBitmap();
		}

		public override bool CheckMipmap()
		{
			return DataFormat != PvrDataFormat.Index4 && DataFormat != PvrDataFormat.Index8 && Image.Width == Image.Height;
		}
	}

	class GvrTextureInfo : TextureInfo
	{
		public GvrDataFormat DataFormat { get; set; }
		public GvrPixelFormat PixelFormat { get; set; }

		public GvrTextureInfo() { }

		public GvrTextureInfo(TextureInfo tex)
		{
			Name = tex.Name;
			GlobalIndex = tex.GlobalIndex;
			Image = tex.Image;
			Mipmap = tex.Mipmap;
			PixelFormat = GvrPixelFormat.Unknown;
			DataFormat = GvrDataFormat.Unknown;
		}

		public GvrTextureInfo(PvrTextureInfo tex)
			: this((TextureInfo)tex)
		{
			switch (tex.DataFormat)
			{
				case PvrDataFormat.Index4:
					DataFormat = GvrDataFormat.Index4;
					break;
				case PvrDataFormat.Index8:
					DataFormat = GvrDataFormat.Index8;
					break;
			}
		}

		public GvrTextureInfo(GvrTextureInfo tex)
			: this((TextureInfo)tex)
		{
			PixelFormat = tex.PixelFormat;
			DataFormat = tex.DataFormat;
		}

		public GvrTextureInfo(string name, uint gbix, Bitmap bitmap)
		{
			Name = name;
			GlobalIndex = gbix;
			DataFormat = GvrDataFormat.Unknown;
			PixelFormat = GvrPixelFormat.Unknown;
			Image = bitmap;
		}

		public GvrTextureInfo(string name, GvrTexture texture)
		{
			Name = name;
			GlobalIndex = texture.GlobalIndex;
			DataFormat = texture.DataFormat;
			Mipmap = texture.HasMipmaps;
			PixelFormat = texture.PixelFormat;
			Image = texture.ToBitmap();
		}

		public override bool CheckMipmap()
		{
			return DataFormat != GvrDataFormat.Index4 && DataFormat != GvrDataFormat.Index8;
		}
	}

	class PvmxTextureInfo : TextureInfo
	{
		public Size? Dimensions { get; set; }

		public PvmxTextureInfo() { }

		public PvmxTextureInfo(TextureInfo tex)
		{
			Name = tex.Name;
			GlobalIndex = tex.GlobalIndex;
			Image = tex.Image;
		}

		public PvmxTextureInfo(PvmxTextureInfo tex)
			: this((TextureInfo)tex)
		{
			Dimensions = tex.Dimensions;
		}

		public PvmxTextureInfo(string name, uint gbix, Bitmap bitmap)
		{
			Name = name;
			GlobalIndex = gbix;
			Image = bitmap;
		}

		public override bool CheckMipmap()
		{
			return false;
		}
	}

	class PakTextureInfo : TextureInfo
	{
		public PakTextureInfo() { }

		public PakTextureInfo(TextureInfo tex)
		{
			Name = tex.Name;
			GlobalIndex = tex.GlobalIndex;
			Image = tex.Image;
		}

		public PakTextureInfo(string name, uint gbix, Bitmap bitmap)
		{
			Name = name;
			GlobalIndex = gbix;
			Image = bitmap;
		}

		public override bool CheckMipmap()
		{
			return false;
		}
	}

	enum TextureFormat { PVM, GVM, PVMX, PAK }
}