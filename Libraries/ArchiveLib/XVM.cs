using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using VrSharp.Xvr;

namespace ArchiveLib
{
	public class XVM : GenericArchive
	{
		public const uint Magic_XVM = 0x484D5658; // XVMH archive
		public const uint Magic_XVRT = 0x54525658; // XVR texture header (texture data)
		public bool isBigEndian = false;

		public XVM()
		{
		}

		public XVM(byte[] data)
		{
			int fileCount = BitConverter.ToInt32(data, 0x8);
			int offset = 0x40; //Start past XVM header

			for (int i = 0; i < fileCount; i++)
			{
				int fullSize = BitConverter.ToInt32(data, offset + 0x4) + 8;
				byte[] rawXvr = new byte[fullSize];
				Array.Copy(data, offset, rawXvr, 0, fullSize);

				//Filenames don't exist in xvm
				Entries.Add(new XVMEntry(rawXvr, $"Texture{i}.xvr"));
				offset += fullSize + (fullSize % 0x10);
			}
		}

		public override void CreateIndexFile(string path)
		{
			using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
			{
				foreach (GenericArchiveEntry xvmentry in Entries)
				{
					texList.WriteLine(xvmentry.Name);
				}
				texList.Flush();
				texList.Close();
			}
		}

		public static bool Identify(byte[] file)
		{
			return file.Length < 4 ? false : BitConverter.ToUInt32(file, 0) == Magic_XVM;
		}

		public override byte[] GetBytes()
		{
			List<byte> xvm = new List<byte>();
			xvm.AddRange(BitConverter.GetBytes(Magic_XVM));
			xvm.AddRange(BitConverter.GetBytes(0x38));
			xvm.AddRange(BitConverter.GetBytes(Entries.Count));
			xvm.AddRange(new byte[0x34]);

			foreach(var entry in Entries)
			{
				xvm.AddRange(entry.Data);
				xvm.AddRange(new byte[entry.Data.Length % 0x10]);
			}

			return xvm.ToArray();
		}

		public class XVMEntry : GenericArchiveEntry
		{
			public uint GBIX;

			public XVMEntry(byte[] xvrdata, string name)
			{
				Name = name;
				Data = xvrdata;
				XvrTexture xvrt = new XvrTexture(xvrdata);
				GBIX = xvrt.GlobalIndex;
			}

			public XVMEntry(string filename)
			{
				Name = Path.GetFileName(filename);
				Data = File.ReadAllBytes(filename);
				XvrTexture xvrt = new XvrTexture(Data);
				GBIX = xvrt.GlobalIndex;
			}

			public uint GetGBIX()
			{
				return GBIX;
			}

			public override Bitmap GetBitmap()
			{
				XvrTexture xvrt = new XvrTexture(Data);
				return xvrt.ToBitmap();
			}
		}

	}
}
