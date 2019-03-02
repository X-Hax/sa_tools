using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PuyoTools.Modules.Archive;
using System.IO;
using VrSharp.PvrTexture;
using VrSharp.GvrTexture;

namespace PVMEditSharp
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
			List<TextureInfo> newtextures = new List<TextureInfo>(pvmentries.Count);
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

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs" })
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
							MemoryStream pvr = new MemoryStream();
							encoder.Save(pvr);
							pvr.Seek(0, SeekOrigin.Begin);
							writer.CreateEntry(pvr, tex.Name);
						}
						break;
				}
				writer.Flush();
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

		private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dlg = new FolderBrowserDialog())
			{
				if (filename != null)
					dlg.SelectedPath = Path.GetDirectoryName(filename);
				if (dlg.ShowDialog(this) == DialogResult.OK)
					foreach (TextureInfo tex in textures)
						tex.Image.Save(Path.Combine(dlg.SelectedPath, tex.Name + ".png"));
			}
		}

		private void exportTexturePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dlg = new FolderBrowserDialog())
			{
				if (filename != null)
					dlg.SelectedPath = Path.GetDirectoryName(filename);

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					string path = Path.Combine(dlg.SelectedPath, Path.GetFileNameWithoutExtension(filename));

					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);

					using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
					{
						foreach (TextureInfo tex in textures)
						{
							tex.Image.Save(Path.Combine(path, tex.Name + ".png"));
							texList.WriteLine("{0},{1}", tex.GlobalIndex, tex.Name + ".png");
						}
					}
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
										textures.Add(new PvrTextureInfo(name, gbix, new Bitmap(dlg.FileName)));
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
			foreach (PvrTextureInfo info in textures)
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

	enum TextureFormat { PVM, GVM }
}