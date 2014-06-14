using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PuyoTools.Modules.Archive;
using System.IO;
using VrSharp;
using VrSharp.PvrTexture;

namespace PVMEditSharp
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		List<TextureInfo> textures = new List<TextureInfo>();

		private void MainForm_Load(object sender, EventArgs e)
		{
			listBox1.DataSource = textures;
			listBox1.DisplayMember = "Name";
		}

		private bool GetTextures(string filename)
		{
			byte[] pvmdata = File.ReadAllBytes(filename);
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
			ArchiveBase pvmfile = new PvmArchive();
			if (!pvmfile.Is(pvmdata, filename))
				return false;
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
								return false;
					vrfile.SetPalette(pvp);
				}
				newtextures.Add(new TextureInfo(Path.GetFileNameWithoutExtension(file.Name), vrfile));
			}
			textures.Clear();
			textures.AddRange(newtextures);
			return true;
		}
	}

	class TextureInfo
	{
		public string Name { get; set; }
		public uint GlobalIndex { get; set; }
		public PvrDataFormat DataFormat { get; set; }
		public PvrPixelFormat PixelFormat { get; set; }
		public Bitmap Image { get; set; }

		public TextureInfo(string name, PvrTexture texture)
		{
			Name = name;
			GlobalIndex = texture.GlobalIndex;
			DataFormat = texture.DataFormat;
			PixelFormat = texture.PixelFormat;
			Image = texture.ToBitmap();
		}
	}
}