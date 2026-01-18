using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TextureLib;
using ArchiveLib;
using static TextureEditor.Program;
using PSO.PRS;

namespace TextureEditor
{
	// The line below prevents Windows Forms Designer from opening on this .cs file
	partial class FormViewBlocker { }

	public partial class MainForm
	{
		/// <summary>
		/// The main file loading function. Processes the specified file and loads texture(s) from it, replacing the current texture list.
		/// If the specified file is an individual PVR/GVR/XVR file, it will also add all PVR/GVR/XVR files located in the same folder.
		/// </summary>
		/// <param name="filename">Path to the file containing texture(s).</param>
		/// <returns>True on success, false on failure.</returns>
		private bool LoadFile(string filename)
		{
			// Load file
			byte[] datafile = File.ReadAllBytes(filename);
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				datafile = PRS.Decompress(datafile);
			// Check if the file is a PVR/GVR/XVR
			PuyoArchiveType puyotype = PuyoArchiveType.Unknown;
			if (PvrTexture.Identify(datafile))
			{
				puyotype = PuyoArchiveType.PVMFile;
				currentFormat = TextureArchiveFormat.PVM;
			}
			else if (GvrTexture.Identify(datafile))
			{
				puyotype = PuyoArchiveType.GVMFile;
				currentFormat = TextureArchiveFormat.GVM;
			}
			else if (XvrTexture.Identify(datafile))
			{
				puyotype = PuyoArchiveType.XVMFile;
				currentFormat = TextureArchiveFormat.XVM;
			}
			// If the file is a single PVR/GVR/XVR, create a PVM/GVM/XVM archive and add the texture to it.
			if (puyotype != PuyoArchiveType.Unknown)
			{
				string[] otherfiles = Directory.GetFiles(Path.GetDirectoryName(filename), "*" + Path.GetExtension(filename), SearchOption.TopDirectoryOnly);
				PuyoFile arc = new PuyoFile(puyotype);
				textures.Clear();
				// Set selected texture index
				int selIndex = 0;
				// Set added entry index
				int entryID = 0;
				for (int i = 0; i < otherfiles.Length; i++)
				{
					try
					{
						string nameNoExt = Path.GetFileNameWithoutExtension(otherfiles[i]).Split('.')[0];
						datafile = File.ReadAllBytes(otherfiles[i]);
						// Decompress if PRS compressed
						if (Path.GetExtension(otherfiles[i]).Equals(".prs", StringComparison.OrdinalIgnoreCase))
							datafile = PRS.Decompress(datafile);
						// Set selection
						if (Path.GetFileName(otherfiles[i]) == Path.GetFileName(filename))
							selIndex = entryID;
						switch (puyotype)
						{
							case PuyoArchiveType.PVMFile:
								if (PvrTexture.Identify(datafile))
								{
									arc.Entries.Add(new PVMEntry(datafile, nameNoExt + ".pvr"));
									textures.Add(new PvrTexture(arc.Entries[entryID].Data) { Name = nameNoExt });
									entryID++;
								}
								break;
							case PuyoArchiveType.GVMFile:
								if (GvrTexture.Identify(datafile))
								{
									arc.Entries.Add(new GVMEntry(datafile, nameNoExt + ".gvr"));
									textures.Add(new GvrTexture(arc.Entries[entryID].Data) { Name = nameNoExt });
									entryID++;
								}
								break;
							case PuyoArchiveType.XVMFile:
								if (XvrTexture.Identify(datafile))
								{
									arc.Entries.Add(new XVMEntry(datafile, nameNoExt + ".xvr"));
									textures.Add(new XvrTexture(arc.Entries[entryID].Data) { Name = nameNoExt });
									entryID++;
								}
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
				// Check for Chao textures
				foreach (var item in textures)
				{
					if (SAModel.SAEditorCommon.ChaoPalettes.CheckIfTextureIsChaoPalettedTexture(item.Name))
					{
						chaoToolStripMenuItem.Visible = true;
						break;
					}
				}
				return true;
			}
			// Otherwise load the file as an archive
			if (PVMXFile.Identify(datafile))
				currentFormat = TextureArchiveFormat.PVMX;
			else if (PAKFile.Identify(filename))
				currentFormat = TextureArchiveFormat.PAK;
			else
			{
				if (PBFile.Identify(datafile))
					datafile = new PBFile(datafile).GetPVM().GetBytes();
				PuyoArchiveType identifyResult = PuyoFile.Identify(datafile);
				switch (identifyResult)
				{
					case PuyoArchiveType.PVMFile:
						currentFormat = TextureArchiveFormat.PVM;
						break;
					case PuyoArchiveType.GVMFile:
						currentFormat = TextureArchiveFormat.GVM;
						break;
					case PuyoArchiveType.XVMFile:
						currentFormat = TextureArchiveFormat.XVM;
						break;
					case PuyoArchiveType.Unknown:
					default:
						MessageBox.Show(this, "Unknown archive type: \"" + filename + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
				}
			}

			// Load textures
			List<GenericTexture> newtextures = GetTexturesFromFile(filename, false);
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

		/// <summary>
		/// Processes a texture archive and retrieves a list of textures from it.
		/// </summary>
		/// <param name="fname">Path to the archive file.</param>
		/// <param name="convert">Check and convert the textures to match the current archive format if necessary (when importing).</param>
		/// <returns>A List of GenericTexture entries.</returns>
		private List<GenericTexture> GetTexturesFromFile(string fname, bool convert)
		{
			usingSocPak = false;
			byte[] datafile = File.ReadAllBytes(fname);
			if (Path.GetExtension(fname).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				datafile = PRS.Decompress(datafile);
			List<GenericTexture> newtextures;
			// PVMX
			if (PVMXFile.Identify(datafile))
			{
				PVMXFile pvmx = new PVMXFile(datafile);
				newtextures = new List<GenericTexture>();
				foreach (PVMXFile.PVMXEntry pvmxe in pvmx.Entries)
				{
					GenericTexture texinfo = GenericTexture.LoadTexture(pvmxe.Data);
					texinfo.Name = Path.GetFileNameWithoutExtension(pvmxe.Name);
					texinfo.Gbix = pvmxe.GBIX;
					texinfo.PvmxOriginalDimensions.Width = pvmxe.Width;
					texinfo.PvmxOriginalDimensions.Height = pvmxe.Height;
					newtextures.Add(texinfo);
				}
			}
			// PAK
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
					usingSocPak = true;
					newtextures = new List<GenericTexture>(pak.Entries.Count);
					foreach (PAKFile.PAKEntry fl in pak.Entries)
					{
						GenericTexture paktex = GenericTexture.LoadTexture(fl.Data);
						paktex.Name = Path.GetFileNameWithoutExtension(fl.Name);
						newtextures.Add(paktex);
					}
				}
				// Handle indexed PAKs
				else
				{
					usingSocPak = false;
					byte[] inf = pak.Entries.Single((file) => file.Name.Equals(indexName, StringComparison.OrdinalIgnoreCase)).Data;
					newtextures = new List<GenericTexture>(inf.Length / 0x3C);
					for (int i = 0; i < inf.Length; i += 0x3C)
					{
						// Load a PAK INF entry
						byte[] pakentry = new byte[0x3C];
						Array.Copy(inf, i, pakentry, 0, 0x3C);
						PAKInfEntry entry = new PAKInfEntry(pakentry);
						// Load texture data
						try
						{
							// The substring thing removes the ".dds" extension in the entry name
							// Names are trimmed to avoid trailing spaces which the game ignores but the tools don't
							byte[] dds = pak.Entries.First((file) => Path.GetExtension(file.Name) != ".inf" && file.Name.Substring(0, file.Name.Length - 4).Trim().Equals(entry.GetFilename().Trim(), StringComparison.OrdinalIgnoreCase)).Data;
							GenericTexture paktx = GenericTexture.LoadTexture(dds);
							paktx.PakMetadata.PakGvrFormat = entry.TypeInf;
							paktx.PakMetadata.PakNinjaFlags = entry.fSurfaceFlags;
							paktx.Gbix = entry.GlobalIndex;
							paktx.Name = entry.GetFilename().Trim();
							newtextures.Add(paktx);
						}
						catch (Exception ex)
						{
							MessageBox.Show(this, $"Could not add texture {entry.GetFilename().Trim() + ".dds: " + ex.Message.ToString()}", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
			// PVM/GVM/XVM/PB
			else
			{
				// If it's a PB file, convert it to PVM and load as PVM
				if (PBFile.Identify(datafile))
					datafile = new PBFile(datafile).GetPVM().GetBytes();
				// If it isn't, try loading a PVM/GVM/XVM
				PuyoArchiveType idResult = PuyoFile.Identify(datafile);
				if (idResult == PuyoArchiveType.Unknown)
				{
					MessageBox.Show(this, "Unknown archive type: " + fname + ".", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return new List<GenericTexture>();
				}
				PuyoFile arc = new PuyoFile(datafile);
				newtextures = new List<GenericTexture>(arc.Entries.Count);
				foreach (GenericArchive.GenericArchiveEntry file in arc.Entries)
				{
					if (file is PVMEntry pvme)
						newtextures.Add(new PvrTexture(file.Data) { Name = Path.GetFileNameWithoutExtension(file.Name) });
					else if (file is GVMEntry gvme)
						newtextures.Add(new GvrTexture(file.Data) { Name = Path.GetFileNameWithoutExtension(file.Name) });
					else if (file is XVMEntry xvme)
						newtextures.Add(new XvrTexture(file.Data) { Name = Path.GetFileNameWithoutExtension(file.Name) });
				}
				// Check if the file contains paletted Chao textures to show or hide the Chao Settings menu
				chaoToolStripMenuItem.Visible = SAModel.SAEditorCommon.ChaoPalettes.CheckIfArchiveHasPalettedChaoTextures(arc);
			}
			// Check if GenericTexture match the current format and convert if necessary.
			// This part is here because GetTexturesFromArchive() can also be called when adding PVM/GVM etc. using the "Add Texture..." button.
			if (convert)
			{
				for (int i = 0; i < newtextures.Count; i++)
				{
					switch (currentFormat)
					{
						case TextureArchiveFormat.PVM:
							if (!(newtextures[i] is PvrTexture))
								newtextures[i] = newtextures[i].ToPvr(preferHighQualityToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked);
							break;
						case TextureArchiveFormat.GVM:
							if (!(newtextures[i] is GvrTexture))
								newtextures[i] = newtextures[i].ToGvr(preferHighQualityToolStripMenuItem.Checked && highQualityGVMsToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked);
							break;
						case TextureArchiveFormat.PVMX:
							if ((!useDDSInPVMXToolStripMenuItem.Checked && !(newtextures[i] is GdiTexture)) || (useDDSInPVMXToolStripMenuItem.Checked && !(newtextures[i] is DdsTexture)))
								newtextures[i] = useDDSInPVMXToolStripMenuItem.Checked ? newtextures[i].ToDds(preferHighQualityToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked) : newtextures[i].ToGdi();
							break;
						case TextureArchiveFormat.PAK:
							if ((!useDDSInPAKsToolStripMenuItem.Checked && !(newtextures[i] is GdiTexture)) || (useDDSInPAKsToolStripMenuItem.Checked && !(newtextures[i] is DdsTexture)) || !(newtextures[i] is InvalidTexture))
								newtextures[i] = useDDSInPAKsToolStripMenuItem.Checked ? newtextures[i].ToDds(preferHighQualityToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked) : newtextures[i].ToGdi();
							break;
						case TextureArchiveFormat.XVM:
							if (!(newtextures[i] is XvrTexture))
								newtextures[i] = newtextures[i].ToXvr(preferHighQualityToolStripMenuItem.Checked, allowCompressedFormatsToolStripMenuItem.Checked);
							break;
					}
				}
			}
			return newtextures;
		}
	}
}