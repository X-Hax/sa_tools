using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.DirectX.Direct3D;
using PAKLib;
using PuyoTools.Modules.Archive;
using VrSharp;
using VrSharp.GvrTexture;
using VrSharp.PvrTexture;

namespace GVM2PAK
{
	class Program
	{
		static void Main(string[] args)
		{
			Queue<string> files = new Queue<string>(args);
			if (files.Count == 0)
			{
				Console.Write("File: ");
				files.Enqueue(Console.ReadLine());
			}
			while (files.Count > 0)
			{
				string filename = files.Dequeue();
				PAKFile pak = new PAKFile();
				List<byte> inf = new List<byte>();
				string filenoext = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
				string longdir = "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
				byte[] filedata = File.ReadAllBytes(filename);
				using (System.Windows.Forms.Panel panel1 = new System.Windows.Forms.Panel())
				using (Device d3ddevice = new Device(0, DeviceType.Hardware, panel1, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { new PresentParameters() { Windowed = true, SwapEffect = SwapEffect.Discard, EnableAutoDepthStencil = true, AutoDepthStencilFormat = DepthFormat.D24X8 } }))
				{
					if (PvrTexture.Is(filedata))
					{
						if (!AddTexture(pak, filenoext, longdir, false, inf, d3ddevice, filename, new MemoryStream(filedata)))
							continue;
						goto end;
					}
					else if (GvrTexture.Is(filedata))
					{
						if (!AddTexture(pak, filenoext, longdir, true, inf, d3ddevice, filename, new MemoryStream(filedata)))
							continue;
						goto end;
					}
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
					ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
					bool fail = false;
					foreach (ArchiveEntry file in pvmentries)
						if (!AddTexture(pak, filenoext, longdir, gvm, inf, d3ddevice, file.Name, file.Open()))
						{
							fail = true;
							break;
						}
					if (fail)
						continue;
				}
			end:
				pak.Files.Insert(0, new PAKFile.File(filenoext + '\\' + filenoext + ".inf", longdir + '\\' + filenoext + ".inf", inf.ToArray()));
				pak.Save(Path.ChangeExtension(filename, "pak"));
			}
		}

		static bool AddTexture(PAKFile pak, string filenoext, string longdir, bool gvm, List<byte> inf, Device d3ddevice, string filename, Stream data)
		{
			VrTexture vrfile = gvm ? (VrTexture)new GvrTexture(data) : (VrTexture)new PvrTexture(data);
			if (vrfile.NeedsExternalPalette)
			{
				Console.WriteLine("Cannot convert texture files which require external palettes!");
				return false;
			}
			Bitmap bmp;
			try { bmp = vrfile.ToBitmap(); }
			catch { bmp = new Bitmap(1, 1); }
			Stream tex = TextureLoader.SaveToStream(ImageFileFormat.Dds, Texture.FromBitmap(d3ddevice, bmp, Usage.SoftwareProcessing, Pool.Managed));
			byte[] tb = new byte[tex.Length];
			tex.Read(tb, 0, tb.Length);
			pak.Files.Add(new PAKFile.File(filenoext + '\\' + Path.ChangeExtension(filename, ".dds"), longdir + '\\' + Path.ChangeExtension(filename, ".dds"), tb));
			int i = inf.Count;
			inf.AddRange(Encoding.ASCII.GetBytes(Path.ChangeExtension(filename, null)));
			inf.AddRange(new byte[0x1C - (inf.Count - i)]);
			if (vrfile.HasGlobalIndex)
				inf.AddRange(BitConverter.GetBytes(vrfile.GlobalIndex));
			else
				inf.AddRange(BitConverter.GetBytes(-1));
			if (gvm)
			{
				GvrTexture gvr = (GvrTexture)vrfile;
				inf.AddRange(BitConverter.GetBytes((int)gvr.DataFormat));
				inf.AddRange(BitConverter.GetBytes((int)gvr.PixelFormat));
				inf.AddRange(BitConverter.GetBytes((int)gvr.DataFormat));
			}
			else
			{
				PvrTexture pvr = (PvrTexture)vrfile;
				inf.AddRange(BitConverter.GetBytes((int)pvr.DataFormat));
				inf.AddRange(BitConverter.GetBytes((int)pvr.PixelFormat));
				inf.AddRange(BitConverter.GetBytes((int)pvr.DataFormat));
			}
			inf.AddRange(BitConverter.GetBytes((int)vrfile.TextureWidth));
			inf.AddRange(BitConverter.GetBytes((int)vrfile.TextureHeight));
			inf.AddRange(BitConverter.GetBytes(vrfile.PvrtOffset));
			inf.AddRange(BitConverter.GetBytes(0x80000000));
			return true;
		}
	}
}