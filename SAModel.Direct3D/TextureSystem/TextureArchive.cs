using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PuyoTools.Modules.Archive;
using VrSharp;
using VrSharp.GvrTexture;
using VrSharp.PvrTexture;

namespace SonicRetro.SAModel.Direct3D.TextureSystem
{
	/// <summary>
	/// This TextureArchive class is the primary interface for retrieving a texture list/array from a container format, such as PVM/GVM, and eventually PAK.
	/// </summary>
	public static class TextureArchive
	{
		public static BMPInfo[] GetTextures(string filename)
		{
			List<BMPInfo> functionReturnValue = new List<BMPInfo>();
			bool gvm = false;
			ArchiveBase pvmfile = null;
			byte[] pvmdata = File.ReadAllBytes(filename);
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
			pvmfile = new PvmArchive();
			if (!pvmfile.Is(pvmdata, filename))
			{
				pvmfile = new GvmArchive();
				gvm = true;
			}
			VrSharp.VpPalette pvp = null;
            PvmArchiveReader pvmReader = (PvmArchiveReader)pvmfile.Open(pvmdata);
            ArchiveEntryCollection pvmentries = pvmReader.Entries;
          
			foreach (ArchiveEntry file in pvmentries)
			{
				byte[] data = new byte[file.Length];
				using (Stream str = pvmReader.OpenEntry(file))
					str.Read(data, 0, file.Length);
				VrTexture vrfile = gvm ? (VrTexture)new GvrTexture(data) : (VrTexture)new PvrTexture(data);
				if (vrfile.NeedsExternalPalette)
				{
					using (System.Windows.Forms.OpenFileDialog a = new System.Windows.Forms.OpenFileDialog
					{
						DefaultExt = gvm ? "gvp" : "pvp",
						Filter = gvm ? "GVP Files|*.gvp" : "PVP Files|*.pvp",
						InitialDirectory = System.IO.Path.GetDirectoryName(filename),
						Title = "External palette file"
					})
					{
						if (pvp == null)
							if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
								pvp = gvm ? (VpPalette)new GvpPalette(a.FileName) : (VpPalette)new PvpPalette(a.FileName);
							else
								return new BMPInfo[0];
					}
					if (gvm)
						((GvrTexture)vrfile).SetPalette((GvpPalette)pvp);
					else
						((PvrTexture)vrfile).SetPalette((PvpPalette)pvp);
				}
				try
				{
					functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(file.Name), vrfile.ToBitmap()));
				}
				catch
				{
					functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(file.Name), new Bitmap(1, 1)));
				}
			}
			return functionReturnValue.ToArray();
		}
	}
}