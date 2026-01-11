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
		/// <summary>Import a texture and convert it to the most appropriate format based on its image data.</summary>		
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
				switch (currentFormat)
				{
					case TextureArchiveFormat.PVM:
						PvrTexture oldpvr = (PvrTexture)textures[currentTextureID];
						if (tex is PvrTexture)
							textures[currentTextureID] = new PvrTexture(data) { Name = oldpvr.Name, Gbix = oldpvr.Gbix };
						else
							textures[currentTextureID] = tex.ToPvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed);
						break;
					case TextureArchiveFormat.GVM:
						GvrTexture oldgvr = (GvrTexture)textures[currentTextureID];
						if (tex is GvrTexture)
							textures[currentTextureID] = new GvrTexture(data) { Name = oldgvr.Name, Gbix = oldgvr.Gbix };
						else
							textures[currentTextureID] = tex.ToGvr(settingsfile.TexEncodeAutoHighQuality && settingsfile.HighQualityGVM, settingsfile.TexEncodeUseCompressed);
						break;
					case TextureArchiveFormat.XVM:
						XvrTexture oldxvr = (XvrTexture)textures[currentTextureID];
						if (tex is XvrTexture)
							textures[currentTextureID] = new XvrTexture(data) { Name = oldxvr.Name, Gbix = oldxvr.Gbix };
						else
							textures[currentTextureID] = tex.ToXvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed);
						break;
					case TextureArchiveFormat.PVMX:
						GenericTexture oldpvmx = textures[currentTextureID];
						if (tex is DdsTexture && settingsfile.UseDDSforPVMX)
							textures[currentTextureID] = new DdsTexture(data) { Name = oldpvmx.Name, Gbix = oldpvmx.Gbix };
						else if (tex is GdiTexture && !settingsfile.UseDDSforPVMX)
							textures[currentTextureID] = new GdiTexture(data) { Name = oldpvmx.Name, Gbix = oldpvmx.Gbix };
						else
						{
							tex.Name = oldpvmx.Name;
							tex.Gbix = oldpvmx.Gbix;
							textures[currentTextureID] = settingsfile.UseDDSforPVMX ? tex.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tex.ToGdi();
						}
						break;
					case TextureArchiveFormat.PAK:
						GenericTexture oldpak = textures[currentTextureID];
						if (tex is DdsTexture && settingsfile.UseDDSforPAK)
							textures[currentTextureID] = new DdsTexture(data) { Name = oldpak.Name, Gbix = oldpak.Gbix, PakMetadata = oldpak.PakMetadata };
						else if (tex is GdiTexture && !settingsfile.UseDDSforPAK)
							textures[currentTextureID] = new GdiTexture(data) { Name = oldpak.Name, Gbix = oldpak.Gbix, PakMetadata = oldpak.PakMetadata };
						else
						{
							tex.Name = oldpak.Name;
							tex.Gbix = oldpak.Gbix;
							tex.PakMetadata = oldpak.PakMetadata;
							textures[currentTextureID] = settingsfile.UseDDSforPAK ? tex.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tex.ToGdi();
						}
						break;
					default:
						break;
				}				
				unsaved = true;
				return true;
			}
			return false;
		}

		/// <summary>Loads an image and encodes it in the same format as the currently selected texture.</summary>	
		private void ReplaceImage()
		{
			OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvr", Filter = "Texture Files|*.pvr;*.gvr;*.xvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.dds" };
			if (!string.IsNullOrEmpty(listBox1.GetItemText(listBox1.SelectedItem)))
				dlg.FileName = listBox1.GetItemText(listBox1.SelectedItem) + ".png";
			DialogResult res = dlg.ShowDialog(this);
			if (res == DialogResult.OK)
			{
				GenericTexture src = GenericTexture.LoadTexture(dlg.FileName);
				textures[currentTextureID].Image = src.Image;
				textures[currentTextureID].Encode();
				UpdateTextureInformation();
				unsaved = true;
			}
			listBox1.Select();
		}

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
										if (tex is DdsTexture && settingsfile.UseDDSforPVMX)
											textures.Add(tex);
										else if (tex is GdiTexture && !settingsfile.UseDDSforPVMX)
											textures.Add(tex);
										else
											textures.Add(settingsfile.UseDDSforPVMX ? tex.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tex.ToGdi());
										break;
									case TextureArchiveFormat.PAK:
										if (tex is DdsTexture && settingsfile.UseDDSforPAK)
											textures.Add(tex);
										else if (tex is GdiTexture && !settingsfile.UseDDSforPAK)
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

		private void AddTexture()
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
								foreach (GenericTexture tex in GetTexturesFromFile(file))
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
											if (tx is DdsTexture && settingsfile.UseDDSforPVMX)
												textures.Add(tx);
											else if (tx is GdiTexture && !settingsfile.UseDDSforPVMX)
												textures.Add(tx);
											else
												textures.Add(settingsfile.UseDDSforPVMX ? tx.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed) : tx.ToGdi());
											break;
										case TextureArchiveFormat.PAK:
											if (tx is DdsTexture && settingsfile.UseDDSforPAK)
												textures.Add(tx);
											else if (tx is GdiTexture && !settingsfile.UseDDSforPAK)
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