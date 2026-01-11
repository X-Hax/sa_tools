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
		/// <summary>Exports all currently loaded textures in the specified format.</summary>
		/// <param name="expfmt">Target format (PVR, GVR, XVR, DDS or PNG).</param>
		private static void ExportAllAs(TextureFileFormat expfmt)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { Title = "Save Folder", DefaultExt = "", Filter = "Folder|", FileName = "texturepack" })
			{

				if (archiveFilename != null)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(archiveFilename);
					dlg.FileName = Path.GetFileNameWithoutExtension(archiveFilename);
				}

				if (dlg.ShowDialog(primaryForm) == DialogResult.OK)
				{
					Directory.CreateDirectory(dlg.FileName);
					string dir = Path.Combine(Path.GetDirectoryName(dlg.FileName), Path.GetFileName(dlg.FileName));
					using (TextWriter texList = File.CreateText(Path.Combine(dir, "index.txt")))
					{
						foreach (GenericTexture tex in textures)
						{
							byte[] exportData;
							switch (expfmt)
							{
								case TextureFileFormat.Pvr:
									exportData = tex.ToPvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed).RawData;
									break;
								case TextureFileFormat.Gvr:
									exportData = tex.ToGvr(settingsfile.TexEncodeAutoHighQuality && settingsfile.HighQualityGVM, settingsfile.TexEncodeUseCompressed).RawData;
									break;
								case TextureFileFormat.Xvr:
									exportData = tex.ToXvr(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed).RawData;
									break;
								case TextureFileFormat.Dds:
									exportData = tex.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed).RawData;
									break;
								case TextureFileFormat.Png:
								default:
									exportData = tex.ToGdi().RawData;
									break;
							}
							string ext = GenericTexture.IdentifyTextureFileExtension(exportData);
							texList.WriteLine("{0},{1},{2}x{3}", tex.Gbix, tex.Name + ext, tex.Image.Width, tex.Image.Height);
							File.WriteAllBytes(Path.Combine(dir, tex.Name + ext), exportData);
						}
					}
				}
			}
		}

		/// <summary>Exports all currently loaded textures as a folder texture pack.</summary>
		private static void ExportTexturePack()
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { Title = "Save Folder", DefaultExt = "", Filter = "Folder|", FileName = "texturepack" })
			{
				if (archiveFilename != null)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(archiveFilename);
					dlg.FileName = Path.GetFileNameWithoutExtension(archiveFilename);
				}
				if (dlg.ShowDialog(primaryForm) == DialogResult.OK)
				{
					Directory.CreateDirectory(dlg.FileName);
					string dir = Path.Combine(Path.GetDirectoryName(dlg.FileName), Path.GetFileName(dlg.FileName));
					using (TextWriter texList = File.CreateText(Path.Combine(dir, "index.txt")))
					{
						foreach (GenericTexture tex in textures)
						{
							byte[] exportData = settingsfile.UseDDSforTexPack ? tex.ToDds(settingsfile.TexEncodeAutoHighQuality, settingsfile.TexEncodeUseCompressed).RawData : tex.ToGdi().RawData;
							if (tex.PvmxOriginalDimensions.Width != 0)
								texList.WriteLine("{0},{1},{2}x{3}", tex.Gbix, tex.Name + GenericTexture.IdentifyTextureFileExtension(exportData), tex.PvmxOriginalDimensions.Width, tex.PvmxOriginalDimensions.Height);
							else
								texList.WriteLine("{0},{1},{2}x{3}", tex.Gbix, tex.Name + GenericTexture.IdentifyTextureFileExtension(exportData), tex.Image.Width, tex.Image.Height);
							File.WriteAllBytes(Path.Combine(dir, tex.Name + GenericTexture.IdentifyTextureFileExtension(exportData)), exportData.ToArray());
						}
					}
				}
			}
		}

		/// <summary>Exports all mipmaps in the currently selected texture as individual PNG images.</summary>
		private void ExportMipmaps()
		{
			if (listBox1.SelectedIndex == -1)
				return;
			if (!textures[listBox1.SelectedIndex].HasMipmaps)
				return;
			if (textures[listBox1.SelectedIndex].MipmapImages.Length == 0)
				return;
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", FileName = textures[listBox1.SelectedIndex].Name + ".png", Filter = "PNG Files|*.png" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					for (int i = 0; i < textures[listBox1.SelectedIndex].MipmapImages.Length; i++)
					{
						Bitmap bmp = new Bitmap(textures[listBox1.SelectedIndex].MipmapImages[i]);
						bmp.Save(Path.ChangeExtension(dlg.FileName, null) + "_" + i.ToString() + ".png");
					}
				}
			listBox1.Select();
		}
	}
}