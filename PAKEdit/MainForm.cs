using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PAKLib;
using Microsoft.DirectX.Direct3D;

namespace PAKEdit
{
	public partial class MainForm : Form
	{
		readonly Properties.Settings Settings = Properties.Settings.Default;

		public MainForm()
		{
			InitializeComponent();
		}

		Device d3ddevice;
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
			Text = "PAK Editor - " + filename;
		}

		private void GetTextures(string filename)
		{
			PAKFile pak = new PAKFile(filename);
			string filenoext = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
			byte[] inf = pak.Files.Single((file) => file.Name.Equals(filenoext + '\\' + filenoext + ".inf", StringComparison.OrdinalIgnoreCase)).Data;
			List<TextureInfo> newtextures = new List<TextureInfo>(inf.Length / 0x3C);
			for (int i = 0; i < inf.Length; i += 0x3C)
			{
				StringBuilder sb = new StringBuilder(0x1C);
				for (int j = 0; j < 0x1C; j++)
					if (inf[i + j] != 0)
						sb.Append((char)inf[i + j]);
					else
						break;
				byte[] dds = pak.Files.Single((file) => file.Name.Equals(filenoext + '\\' + sb.ToString() + ".dds", StringComparison.OrdinalIgnoreCase)).Data;
				using (MemoryStream str = new MemoryStream(dds))
				using (Texture tex = TextureLoader.FromStream(d3ddevice, str))
				using (Stream bmp = TextureLoader.SaveToStream(ImageFileFormat.Png, tex))
					newtextures.Add(new TextureInfo(sb.ToString(), BitConverter.ToUInt32(inf, i + 0x1C), new Bitmap(bmp)));
			}
			textures.Clear();
			textures.AddRange(newtextures);
			listBox1.Items.Clear();
			listBox1.Items.AddRange(textures.Select((item) => item.Name).ToArray());
			SetFilename(Path.GetFullPath(filename));
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			System.Collections.Specialized.StringCollection newlist = new System.Collections.Specialized.StringCollection();
			if (Settings.MRUList != null)
				foreach (string file in Settings.MRUList)
					if (File.Exists(file))
					{
						newlist.Add(file);
						recentFilesToolStripMenuItem.DropDownItems.Add(file.Replace("&", "&&"));
					}
			Settings.MRUList = newlist;
			d3ddevice = new Device(0, DeviceType.Hardware, dummyPanel, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { new PresentParameters() { Windowed = true, SwapEffect = SwapEffect.Discard, EnableAutoDepthStencil = true, AutoDepthStencilFormat = DepthFormat.D24X8 } });
			if (Program.Arguments.Length > 0)
				GetTextures(Program.Arguments[0]);
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textures.Clear();
			listBox1.Items.Clear();
			filename = null;
			Text = "PAK Editor";
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pak", Filter = "PAK Files|*.pak" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					GetTextures(dlg.FileName);
					listBox1_SelectedIndexChanged(sender, e);
				}
		}

		private void SaveTextures()
		{
			PAKFile pak = new PAKFile();
			string filenoext = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
			string longdir = "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
			List<byte> inf = new List<byte>(textures.Count * 0x3C);
			foreach (TextureInfo item in textures)
			{
				Stream tex = TextureLoader.SaveToStream(ImageFileFormat.Dds, Texture.FromBitmap(d3ddevice, item.Image, Usage.SoftwareProcessing, Pool.Managed));
				byte[] tb = new byte[tex.Length];
				tex.Read(tb, 0, tb.Length);
				string name = item.Name;
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
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "pak", Filter = "PAK Files|*.pak" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					SetFilename(dlg.FileName);
					SaveTextures();
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
				textureImage.Image = textures[listBox1.SelectedIndex].Image;
			}
		}

		private KeyValuePair<string, Bitmap>? BrowseForTexture()
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvr", Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
					return new KeyValuePair<string, Bitmap>(Path.GetFileNameWithoutExtension(dlg.FileName), new Bitmap(dlg.FileName));
				else
					return null;
		}

		private void addTextureButton_Click(object sender, EventArgs e)
		{
			KeyValuePair<string, Bitmap>? tex = BrowseForTexture();
			if (tex.HasValue)
			{
				uint gbix = textures.Max((item) => item.GlobalIndex);
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

		private void importButton_Click(object sender, EventArgs e)
		{
			KeyValuePair<string, Bitmap>? tex = BrowseForTexture();
			if (tex.HasValue)
				textureImage.Image = textures[listBox1.SelectedIndex].Image = tex.Value.Value;
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", FileName = textures[listBox1.SelectedIndex].Name + ".png", Filter = "PNG Files|*.png" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
					textures[listBox1.SelectedIndex].Image.Save(dlg.FileName);
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
		public Bitmap Image { get; set; }

		public TextureInfo() { }

		public TextureInfo(string name, uint gbix, Bitmap bitmap)
		{
			Name = name;
			GlobalIndex = gbix;
			Image = bitmap;
		}
	}
}