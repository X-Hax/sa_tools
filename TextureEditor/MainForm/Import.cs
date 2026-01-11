using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TextureLib;
using static TextureEditor.Program;

namespace TextureEditor
{
	public partial class MainForm
	{
		/// <summary>
		/// Checks whether the dimensions of the specified Bitmap are compatible with the specified texture.
		/// </summary>
		/// <param name="importImage"></param>
		/// <param name="tex"></param>
		/// <returns>True if compatible, false if incompatible.</returns>
		public static bool CheckTextureCompatibility(Bitmap importImage, GenericTexture tex)
		{
			// Dimensions must be power of two
			if (!TextureFunctions.IsPow2(importImage.Width) || !TextureFunctions.IsPow2(importImage.Height))
				return false;
			// Specific format limitations
			switch (tex)
			{
				case PvrTexture pvr:
					switch (pvr.PvrDataFormat)
					{
						// Square textures
						case PvrDataFormat.SquareTwiddled:
						case PvrDataFormat.SquareTwiddledMipmaps:
						case PvrDataFormat.SquareTwiddledMipmapsDma:
						case PvrDataFormat.Vq:
						case PvrDataFormat.VqMipmaps:
						case PvrDataFormat.Index4:
						case PvrDataFormat.Index4Mipmaps:
						case PvrDataFormat.Index8:
						case PvrDataFormat.Index8Mipmaps:
							return importImage.Width == importImage.Height;
						default:
							return true;
					}
				case GvrTexture gvr:
					// Mipmapped textures must be square
					if (gvr.HasMipmaps)
						return importImage.Width == importImage.Height;
					break;
			}
			return true;
		}

		/// <summary>Imports a texture and converts it to the most appropriate format based on its image data.</summary>		
		/// <returns>True on success, false on cancel.</returns>
		public static bool ImportTexture()
		{
			OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "png", Filter = "Texture Files|*.pvr;*.gvr;*.xvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.dds" };
			DialogResult res = dlg.ShowDialog(primaryForm);
			if (res == DialogResult.OK)
			{
				string name = Path.GetFileNameWithoutExtension(dlg.FileName);
				byte[] data = File.ReadAllBytes(dlg.FileName);
				GenericTexture tex = GenericTexture.LoadTexture(data);
				if (!TextureFunctions.IsPow2(tex.Width) || !TextureFunctions.IsPow2(tex.Height))
				{
					MessageBox.Show(primaryForm, "The image is not compatible with this texture format.\nMake sure the image's dimensions are power of two.", "Texture Editor Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					return false;
				}
				switch (currentFormat)
				{
					case TextureArchiveFormat.PVM:
						PvrTexture oldpvr = (PvrTexture)textures[currentTextureID];
						if (tex is PvrTexture)
							textures[currentTextureID] = new PvrTexture(data) { Name = oldpvr.Name, Gbix = oldpvr.Gbix };
						else
						{
							tex.Name = oldpvr.Name;
							tex.Gbix = oldpvr.Gbix;
							textures[currentTextureID] = tex.ToPvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed);
						}
						break;
					case TextureArchiveFormat.GVM:
						GvrTexture oldgvr = (GvrTexture)textures[currentTextureID];
						if (tex is GvrTexture)
							textures[currentTextureID] = new GvrTexture(data) { Name = oldgvr.Name, Gbix = oldgvr.Gbix };
						else
						{
							tex.Name = oldgvr.Name;
							tex.Gbix = oldgvr.Gbix;
							textures[currentTextureID] = tex.ToGvr(settingsfile.TexEncodeAutoHighQuality && settingsfile.HighQualityGVM, settingsfile.TexEncodeUseCompressed);
						}
						break;
					case TextureArchiveFormat.XVM:
						XvrTexture oldxvr = (XvrTexture)textures[currentTextureID];
						if (tex is XvrTexture)
							textures[currentTextureID] = new XvrTexture(data) { Name = oldxvr.Name, Gbix = oldxvr.Gbix };
						else
						{
							tex.Name = oldxvr.Name;
							tex.Gbix = oldxvr.Gbix;
							textures[currentTextureID] = tex.ToXvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed);
						}
						break;
					case TextureArchiveFormat.PVMX:
						GenericTexture oldpvmx = textures[currentTextureID];
						tex.Name = oldpvmx.Name;
						tex.Gbix = oldpvmx.Gbix;
						tex.PvmxOriginalDimensions = oldpvmx.PvmxOriginalDimensions;
						if (tex is DdsTexture || tex is GdiTexture)
							textures[currentTextureID] = tex;
						else
							textures[currentTextureID] = settingsfile.UseDDSforPVMX ? tex.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tex.ToGdi();
						break;
					case TextureArchiveFormat.PAK:
						GenericTexture oldpak = textures[currentTextureID];
						tex.Name = oldpak.Name;
						tex.Gbix = oldpak.Gbix;
						tex.PakMetadata = oldpak.PakMetadata;
						if (tex is DdsTexture || tex is GdiTexture)
							textures[currentTextureID] = tex;
						else
							textures[currentTextureID] = settingsfile.UseDDSforPAK ? tex.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tex.ToGdi();
						break;
					default:
						break;
				}				
				unsaved = true;
				return true;
			}
			return false;
		}

		/// <summary>Loads an image and encodes it in the same format as the currently selected texture, or ask if it's indexed.</summary>	
		private void ReplaceImage()
		{
			OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvr", Filter = "Texture Files|*.pvr;*.gvr;*.xvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.dds" };
			if (!string.IsNullOrEmpty(listBox1.GetItemText(listBox1.SelectedItem)))
				dlg.FileName = listBox1.GetItemText(listBox1.SelectedItem) + ".png";
			DialogResult res = dlg.ShowDialog(this);
			if (res == DialogResult.OK)
			{
				GenericTexture src = GenericTexture.LoadTexture(dlg.FileName);
				if (!CheckTextureCompatibility(src.Image, textures[currentTextureID]))
				{
					MessageBox.Show(primaryForm, "The image is not compatible with this texture format.\nMake sure the image is square and its dimensions are power of two.", "Texture Editor Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					return;
				}
				IndexedTextureFormat outFormat = IndexedTextureFormat.NotIndexed;
				PixelCodec outPaletteCodec = null;
				bool clut = false;
				// If the input image is indexed, show a dialog
				if (src.Indexed)
				{
					using (IndexedImageImportDialog importDialog = new IndexedImageImportDialog(src.Image, textures[currentTextureID], settingsfile.SACompatiblePalettes))
					{
						if (importDialog.ShowDialog(this) == DialogResult.OK)
						{
							outFormat = importDialog.outFormat;
							outPaletteCodec = importDialog.outCodec;
							clut = importDialog.outInternal;
						}
						else
							return;
					}
				}
				// Import a non-indexed texture
				if (outFormat == IndexedTextureFormat.NotIndexed)
				{
					textures[currentTextureID].Image = src.Image;
					textures[currentTextureID].Encode();
					UpdateTextureInformation();
				}
				// Import an indexed texture
				else
				{
					bool alreadyIndexed = textures[currentTextureID].Indexed;
					ImportIndexedImage(src.Image, outFormat, outPaletteCodec, clut);
					if (alreadyIndexed)
					{
						UpdateTextureView();
						ShowHidePaletteInfo();
					}
					else
						UpdateTextureInformation();
				}	
				unsaved = true;
			}
			listBox1.Select();
		}

		/// <summary>Loads a folder texture pack and adds textures from it to the current list, converting them to the format appropriate to the current archive format.</summary>
		private void ImportTexturePack()
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
								byte[] file = File.ReadAllBytes(Path.Combine(dir, split[1]));
								uint check = BitConverter.ToUInt32(file, 0);
								MemoryStream str = new MemoryStream(file);
								GenericTexture tex = GenericTexture.LoadTexture(file);
								tex.Name = name;
								tex.Gbix = gbix;
								switch (currentFormat)
								{
									case TextureArchiveFormat.PVM:
										textures.Add(tex.ToPvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed));
										break;
									case TextureArchiveFormat.GVM:
										textures.Add(tex.ToGvr(settingsfile.TexEncodeAutoHighQuality && settingsfile.HighQualityGVM, settingsfile.TexEncodeUseCompressed));
										break;
									case TextureArchiveFormat.PVMX:
										if (split.Length > 2)
										{
											string[] dim = split[2].Split('x');
											if (dim.Length > 1)
												tex.PvmxOriginalDimensions = new Size(int.Parse(dim[0]), int.Parse(dim[1]));
										}
										if (tex is DdsTexture || tex is GdiTexture)
											textures.Add(tex);
										else
											textures.Add(settingsfile.UseDDSforPVMX ? tex.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tex.ToGdi());
										break;
									case TextureArchiveFormat.PAK:
										if (tex is DdsTexture || tex is GdiTexture)
											textures.Add(tex);
										else
											textures.Add(settingsfile.UseDDSforPAK ? tex.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tex.ToGdi());
										break;
									case TextureArchiveFormat.XVM:
										textures.Add(tex.ToXvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed));
										break;
								}
								listBox1.Items.Add(name);
							}
							line = texList.ReadLine();
						}
						listBox1.EndUpdate();
						currentTextureID = textures.Count - 1;
						UpdateTextureCount();
						unsaved = true;
					}
			}
		}

		/// <summary>Adds a single texture or textures from an archive to the current texture list, converting them to the format appropriate to the current archive format.</summary>
		private void AddTextures()
		{
			string defext = null;
			string filter = "Supported Files|*.prs;*.pvm;*.pb;*.gvm;*.pvmx;*.pak;*.pvr;*.gvr;*.xvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.dds";
			switch (currentFormat)
			{
				case TextureArchiveFormat.PVM:
					defext = "pvr";
					break;
				case TextureArchiveFormat.GVM:
					defext = "gvr";
					break;
				case TextureArchiveFormat.PVMX:
					defext = "png";
					break;
				case TextureArchiveFormat.PAK:
					defext = "dds";
					break;
				case TextureArchiveFormat.XVM:
					defext = "xvr";
					break;
			}
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = defext, Filter = filter, Multiselect = true })
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					uint gbix = textures.Count == 0 ? 0 : textures.Max((item) => item.Gbix);
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
								foreach (GenericTexture tex in GetTexturesFromFile(file, true))
								{
									textures.Add(tex);
									listBox1.Items.Add(tex.Name);
								}
								break;
							// Add individual textures
							default:
								{
									string name = Path.GetFileNameWithoutExtension(file);
									byte[] dt = File.ReadAllBytes(file);
									GenericTexture tx = GenericTexture.LoadTexture(dt);
									tx.Name = name;
									tx.Gbix = gbix;
									switch (currentFormat)
									{
										case TextureArchiveFormat.PVM:
											if (tx is PvrTexture)
												textures.Add(tx);
											else
												textures.Add(tx.ToPvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed));
											break;
										case TextureArchiveFormat.GVM:
											if (tx is GvrTexture)
												textures.Add(tx);
											else
												textures.Add(tx.ToGvr(settingsfile.TexEncodeAutoHighQuality && settingsfile.HighQualityGVM, settingsfile.TexEncodeUseCompressed));
											break;
										case TextureArchiveFormat.XVM:
											if (tx is XvrTexture)
												textures.Add(tx);
											else
												textures.Add(tx.ToXvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed));
											break;
										case TextureArchiveFormat.PVMX:
											if (tx is DdsTexture || tx is GdiTexture)
												textures.Add(tx);
											else
												textures.Add(settingsfile.UseDDSforPVMX ? tx.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tx.ToGdi());
											break;
										case TextureArchiveFormat.PAK:
											if (tx is DdsTexture || tx is GdiTexture)
												textures.Add(tx);
											else
												textures.Add(settingsfile.UseDDSforPAK ? tx.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tx.ToGdi());
											break;
									}
									if (gbix != uint.MaxValue)
										gbix++;
									listBox1.Items.Add(name);
								}
								break;
						}
					}
					listBox1.EndUpdate();
					UpdateTextureCount();
					currentTextureID = textures.Count - 1;
					unsaved = true;
				}
			}
		}
	}
}