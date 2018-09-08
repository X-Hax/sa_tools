using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PuyoTools.Modules.Archive;
using System.IO;
using VrSharp.PvrTexture;

namespace PVMEditSharp
{
	public partial class MainForm : Form
	{
		readonly Properties.Settings Settings = Properties.Settings.Default;

		public MainForm()
		{
			InitializeComponent();
		}

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
			Text = "PVM Editor - " + filename;
		}

		private bool GetTextures(string filename)
		{
			byte[] pvmdata = File.ReadAllBytes(filename);
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
			ArchiveBase pvmfile = new PvmArchive();
			if (!pvmfile.Is(pvmdata, filename))
			{
				MessageBox.Show(this, "Could not open file \"" + filename + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			PvpPalette pvp = null;
			ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
			List<TextureInfo> newtextures = new List<TextureInfo>(pvmentries.Count);
			foreach (ArchiveEntry file in pvmentries)
			{
				PvrTexture vrfile = new PvrTexture(file.Open());
				if (vrfile.NeedsExternalPalette)
				{
					if (pvp == null)
						using (System.Windows.Forms.OpenFileDialog a = new System.Windows.Forms.OpenFileDialog
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
				newtextures.Add(new TextureInfo(Path.GetFileNameWithoutExtension(file.Name), vrfile));
			}
			textures.Clear();
			textures.AddRange(newtextures);
			listBox1.Items.Clear();
			listBox1.Items.AddRange(textures.Select((item) => item.Name).ToArray());
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

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textures.Clear();
			listBox1.Items.Clear();
			filename = null;
			Text = "PVM Editor";
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvm", Filter = "PVM Files|*.pvm;*.prs" })
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
				PvmArchiveWriter writer = new PvmArchiveWriter(str);
				foreach (TextureInfo tex in textures)
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
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "pvm", Filter = "PVM Files|*.pvm;*.prs" })
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
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvr", Filter = "Texture Files|*.pvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp" })
			{
				if (!String.IsNullOrEmpty(textureName))
					dlg.FileName = textureName;

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					string name = Path.GetFileNameWithoutExtension(dlg.FileName);
					if (PvrTexture.Is(dlg.FileName))
						return new KeyValuePair<string, Bitmap>(name, new PvrTexture(dlg.FileName).ToBitmap());
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
			KeyValuePair<string, Bitmap>? tex = BrowseForTexture();
			if (tex.HasValue)
			{
				uint gbix = textures.Count == 0 ? 0 : textures.Max((item) => item.GlobalIndex);
				if (gbix != uint.MaxValue)
					gbix++;
				textures.Add(new TextureInfo(tex.Value.Key, gbix, tex.Value.Value));
				listBox1.Items.Add(tex.Value.Key);
				listBox1.SelectedIndex = textures.Count - 1;
			}
		}

		private void removeTextureButton_Click(object sender, EventArgs e)
		{
			int i = listBox1.SelectedIndex;
			textures.RemoveAt(i);
			listBox1.Items.RemoveAt(i);
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

	class TextureInfo
	{
		public string Name { get; set; }
		public uint GlobalIndex { get; set; }
		public PvrDataFormat DataFormat { get; set; }
		public PvrPixelFormat PixelFormat { get; set; }
		public bool Mipmap { get; set; }
		public Bitmap Image { get; set; }

		public TextureInfo() { }

		public TextureInfo(string name, uint gbix, Bitmap bitmap)
		{
			Name = name;
			GlobalIndex = gbix;
			DataFormat = PvrDataFormat.Unknown;
			PixelFormat = PvrPixelFormat.Unknown;
			Image = bitmap;
		}

		public TextureInfo(string name, PvrTexture texture)
		{
			Name = name;
			GlobalIndex = texture.GlobalIndex;
			DataFormat = texture.DataFormat;
			Mipmap = DataFormat == PvrDataFormat.SquareTwiddledMipmaps || DataFormat == PvrDataFormat.SquareTwiddledMipmapsAlt;
			PixelFormat = texture.PixelFormat;
			Image = texture.ToBitmap();
		}

		public bool CheckMipmap()
		{
			return DataFormat != PvrDataFormat.Index4 && DataFormat != PvrDataFormat.Index8 && Image.Width == Image.Height;
		}
	}
}