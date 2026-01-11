using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TextureLib;
using ArchiveLib;
using static TextureEditor.Program;
using PSO.PRS;
using System.Drawing;
using System.Text;

namespace TextureEditor
{
	public partial class MainForm
	{
		/// <summary>
		/// Loops through the texture list and converts all textures to formats usable in the specified archive format (unless they can already be used in that format).
		/// Conversions to GVR/PNG/DDS are done according to user preferences specified in the settings file.
		/// </summary>
		/// <param name="newfmt">Target texture archive format.</param>
		private void ConvertTextures(TextureArchiveFormat newfmt)
		{
			for (int i = 0; i < textures.Count; i++)
			{
				if (textures[i] is InvalidTexture)
					continue;
				switch (newfmt)
				{
					case TextureArchiveFormat.PVM:
						if (textures[i] is PvrTexture)
							continue;
						textures[i] = textures[i].ToPvr(preferHighQualityToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked);
						break;
					case TextureArchiveFormat.GVM:
						if (textures[i] is GvrTexture)
							continue;
						textures[i] = textures[i].ToGvr(preferHighQualityToolStripMenuItem.Checked && highQualityGVMsToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked);
						break;
					case TextureArchiveFormat.XVM:
						if (textures[i] is XvrTexture)
							continue;
						textures[i] = textures[i].ToXvr(preferHighQualityToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked);
						break;
					case TextureArchiveFormat.PVMX:
						if (textures[i] is DdsTexture && useDDSInPVMXToolStripMenuItem.Checked)
							continue;
						else if (textures[i] is GdiTexture && !useDDSInPVMXToolStripMenuItem.Checked)
							continue;
						textures[i] = useDDSInPVMXToolStripMenuItem.Checked ? textures[i].ToDds(preferHighQualityToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked) : textures[i].ToGdi();
						break;
					case TextureArchiveFormat.PAK:
						if (textures[i] is DdsTexture && useDDSInPAKsToolStripMenuItem.Checked)
							continue;
						else if (textures[i] is GdiTexture && !useDDSInPAKsToolStripMenuItem.Checked)
							continue;
						textures[i] = useDDSInPAKsToolStripMenuItem.Checked ? textures[i].ToDds(preferHighQualityToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked) : textures[i].ToGdi();
						break;
				}
			}
			currentFormat = newfmt;
		}

		/// <summary>Dialog to retrieve a single texture from the current texture list and save its data as-is.</summary>
		private void SaveSingleTexture()
		{
			string ext = ".png";
			switch (textures[listBox1.SelectedIndex])
			{
				case PvrTexture:
					ext = ".pvr";
					break;
				case GvrTexture:
					ext = ".gvr";
					break;
				case XvrTexture:
					ext = ".xvr";
					break;
				case DdsTexture:
					ext = ".dds";
					break;
				case GdiTexture:
					ext = ".png";
					break;
			}
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = ext, FileName = textures[listBox1.SelectedIndex].Name, Filter = "All Files|*.*" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					File.WriteAllBytes(dlg.FileName, textures[listBox1.SelectedIndex].RawData);
				}
			listBox1.Select();
		}

		/// <summary>
		/// Saves the current texture list in the specified archive format.
		/// </summary>
		/// <param name="savefmt">Target archive format: PVM, GVM, XVM, PVMX or PAK.</param>
		private void SaveAs(TextureArchiveFormat savefmt)
		{
			string ext;
			switch (savefmt)
			{
				case TextureArchiveFormat.GVM:
					ext = "gvm";
					break;
				case TextureArchiveFormat.PVMX:
					ext = "pvmx";
					break;
				case TextureArchiveFormat.PAK:
					ext = "pak";
					break;
				case TextureArchiveFormat.XVM:
					ext = "xvm";
					break;
				case TextureArchiveFormat.PVM:
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
					ConvertTextures((TextureArchiveFormat)dlg.FilterIndex - 1);
					UpdateMRUList(dlg.FileName);
					SaveTextures();
					unsaved = false;
				}
			}
		}

		/// <summary>The main "Save" function. Saves the current texture list in the current texture archive format.</summary>
		private void SaveTextures()
		{
			byte[] data;
			switch (currentFormat)
			{
				// PVMX and PAK nope out early because they don't need the PRS compression check.
				case TextureArchiveFormat.PVMX:
					PVMXFile pvmx = new PVMXFile();
					foreach (GenericTexture tex in textures)
					{
						Size size = new Size(tex.Image.Width, tex.Image.Height);
						if (tex.PvmxOriginalDimensions.Width != 0)
							size = new Size(tex.PvmxOriginalDimensions.Width, tex.PvmxOriginalDimensions.Height);
						pvmx.Entries.Add(new PVMXFile.PVMXEntry(tex.Name + GenericTexture.IdentifyTextureFileExtension(tex.RawData), tex.Gbix, tex.GetBytes(), size.Width, size.Height));
					}
					File.WriteAllBytes(archiveFilename, pvmx.GetBytes());
					unsaved = false;
					return;
				case TextureArchiveFormat.PAK:
					PAKFile pak = new PAKFile();
					string filenoext = Path.GetFileNameWithoutExtension(archiveFilename).ToLowerInvariant();
					pak.FolderName = filenoext;
					string longdir = usingSocPak ? "..\\..\\..\\sonic2\\resource\\gd_pc\\soc\\" + filenoext : "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
					List<byte> inf = new List<byte>(textures.Count * 0x3C);
					// Add individual PAK entries
					foreach (GenericTexture item in textures)
					{
						string extension = "";
						if (!(item is InvalidTexture))
							extension = GenericTexture.IdentifyTextureFileExtension(item.RawData);
						string name = item.Name.ToLowerInvariant();
						if (name.Length > 0x1C)
							name = name.Substring(0, 0x1C);
						name = name.Trim();
						pak.Entries.Add(new PAKFile.PAKEntry(name + extension, longdir + '\\' + name + extension, item.GetBytes().ToArray()));
						// Create a new PAK INF entry
						if (!usingSocPak)
						{
							PAKInfEntry entry = new PAKInfEntry();
							byte[] namearr = Encoding.ASCII.GetBytes(name);
							Array.Copy(namearr, entry.Filename, namearr.Length);
							entry.GlobalIndex = item.Gbix;
							entry.nWidth = (uint)item.Image.Width;
							entry.nHeight = (uint)item.Image.Height;
							entry.TypeInf = entry.PixelFormatInf = item.PakMetadata.PakGvrFormat;
							entry.fSurfaceFlags = item.PakMetadata.PakNinjaFlags;
							if (item.HasMipmaps)
								entry.fSurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
							inf.AddRange(entry.GetBytes());
						}
					}
					// Insert the INF file
					if (!usingSocPak)
						pak.Entries.Insert(0, new PAKFile.PAKEntry(filenoext + ".inf", longdir + '\\' + filenoext + ".inf", inf.ToArray()));
					pak.Save(archiveFilename);
					unsaved = false;
					return;
				// PVM, GVM and XVM
				case TextureArchiveFormat.PVM:
					PuyoFile puyo = new PuyoFile(PuyoArchiveType.PVMFile);
					foreach (PvrTexture tex in textures)
						puyo.Entries.Add(new PVMEntry(tex.GetBytes(), tex.Name));
					data = puyo.GetBytes();
					if (Path.GetExtension(archiveFilename).Equals(".pb", StringComparison.OrdinalIgnoreCase))
						data = puyo.GetPB().GetBytes();
					break;
				case TextureArchiveFormat.GVM:
					PuyoFile puyog = new PuyoFile(PuyoArchiveType.GVMFile);
					foreach (GvrTexture tex in textures)
						puyog.Entries.Add(new GVMEntry(tex.GetBytes(), tex.Name));
					data = puyog.GetBytes();
					break;
				case TextureArchiveFormat.XVM:
					PuyoFile puyox = new PuyoFile(PuyoArchiveType.XVMFile);
					foreach (XvrTexture tex in textures)
						puyox.Entries.Add(new XVMEntry(tex.GetBytes(), tex.Name));
					data = puyox.GetBytes();
					break;
				default:
					return;
			}
			// If the file extension is PRS, compress it.
			if (Path.GetExtension(archiveFilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				File.WriteAllBytes(archiveFilename, PRS.Compress(data, 255));
			else
				File.WriteAllBytes(archiveFilename, data);
			// Remove the "unsaved changes" flag.
			unsaved = false;
		}
	}
}