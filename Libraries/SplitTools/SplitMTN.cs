using SAModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SplitTools.SAArc
{
	public static class sa2MTN
	{
		public static void Split(bool? isBigEndian, string filename)
		{
			string dir = Environment.CurrentDirectory;
			try
			{
				filename = Path.Combine(Environment.CurrentDirectory, filename);
				Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename));
				byte[] file = File.ReadAllBytes(filename);
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					file = FraGag.Compression.Prs.Decompress(file);
				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename));
				Dictionary<int, int> processedanims = new Dictionary<int, int>();
				switch (isBigEndian)
				{
					case true:
						ByteConverter.BigEndian = true;
						break;
					case null:
						int addr = 0;
						ushort ile = ByteConverter.ToUInt16(file, 0);
						if (ile == 0)
						{
							ile = ByteConverter.ToUInt16(file, 8);
							addr = 8;
						}
						ByteConverter.BigEndian = true;
						if (ile < ByteConverter.ToUInt16(file, addr))
							ByteConverter.BigEndian = false;
						break;
				}
				MTNInfo ini = new MTNInfo { BigEndian = ByteConverter.BigEndian };
				int address = 0;
				short i = ByteConverter.ToInt16(file, address);
				while (i != -1)
				{
					int aniaddr = ByteConverter.ToInt32(file, address + 4);
					if (!processedanims.ContainsKey(aniaddr))
					{
						new NJS_MOTION(file, aniaddr, 0, ByteConverter.ToInt16(file, address + 2))
						.Save(Path.GetFileNameWithoutExtension(filename) + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
						processedanims[aniaddr] = i;
					}
					ini.Indexes[i] = "animation_" + aniaddr.ToString("X8");
					address += 8;
					i = ByteConverter.ToInt16(file, address);
				}
				IniSerializer.Serialize(ini, Path.Combine(Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + ".ini"));
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		public static void Build(bool? isBigEndian, string mtnfilename)
		{
			string dir = Environment.CurrentDirectory;
			try
			{
				mtnfilename = Path.Combine(Environment.CurrentDirectory, mtnfilename);
				if (Directory.Exists(mtnfilename))
					mtnfilename += ".prs";
				Environment.CurrentDirectory = Path.GetDirectoryName(mtnfilename);
				List<NJS_MOTION> anims = new List<NJS_MOTION>();
				foreach (string file in Directory.GetFiles(Path.GetFileNameWithoutExtension(mtnfilename), "*.saanim"))
					if (short.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out short i))
						anims.Add(NJS_MOTION.Load(file));
				MTNInfo mtninfo = IniSerializer.Deserialize<MTNInfo>(
					Path.Combine(Path.GetFileNameWithoutExtension(mtnfilename), Path.GetFileNameWithoutExtension(mtnfilename) + ".ini"));
				if (!isBigEndian.HasValue)
					ByteConverter.BigEndian = mtninfo.BigEndian;
				else
					ByteConverter.BigEndian = isBigEndian.Value;
				List<byte> animbytes = new List<byte>();
				Dictionary<string, int> animaddrs = new Dictionary<string, int>();
				Dictionary<string, short> animparts = new Dictionary<string, short>();
				uint imageBase = (uint)(mtninfo.Indexes.Count * 8) + 8;
				foreach (NJS_MOTION item in anims)
				{
					animbytes.AddRange(item.GetBytes(imageBase, out uint address));
					animaddrs[item.Name] = (int)(address + imageBase);
					animparts[item.Name] = (short)item.ModelParts;
					imageBase = (uint)(mtninfo.Indexes.Count * 8) + 8 + (uint)animbytes.Count;
				}
				List<byte> mtnfile = new List<byte>();
				foreach (KeyValuePair<short, string> item in mtninfo.Indexes)
				{
					mtnfile.AddRange(ByteConverter.GetBytes(item.Key));
					mtnfile.AddRange(ByteConverter.GetBytes(animparts[item.Value]));
					mtnfile.AddRange(ByteConverter.GetBytes(animaddrs[item.Value]));
				}
				mtnfile.AddRange(new byte[] { 0xFF, 0xFF, 0, 0, 0, 0, 0, 0 });
				mtnfile.AddRange(animbytes);
				if (Path.GetExtension(mtnfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					FraGag.Compression.Prs.Compress(mtnfile.ToArray(), mtnfilename);
				else
					File.WriteAllBytes(mtnfilename, mtnfile.ToArray());
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}
	}
}
