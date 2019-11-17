using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SA_Tools;
using SonicRetro.SAModel;

namespace buildMTN
{
	class Program
	{
		static void Main(string[] args)
		{
			string dir = Environment.CurrentDirectory;
			try
			{
				Queue<string> argq = new Queue<string>(args);
				bool? be = null;
				if (argq.Count > 0)
					if (argq.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
					{
						be = true;
						argq.Dequeue();
					}
					else if (argq.Peek().Equals("/le", StringComparison.OrdinalIgnoreCase))
					{
						be = false;
						argq.Dequeue();
					}
				string mtnfilename;
				if (argq.Count > 0)
				{
					mtnfilename = argq.Dequeue();
					Console.WriteLine("File: {0}", mtnfilename);
				}
				else
				{
					Console.Write("File: ");
					mtnfilename = Console.ReadLine();
				}
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
				if (!be.HasValue)
					ByteConverter.BigEndian = mtninfo.BigEndian;
				else
					ByteConverter.BigEndian = be.Value;
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

	class MTNInfo
	{
		public bool BigEndian { get; set; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<short, string> Indexes { get; set; } = new Dictionary<short, string>();
	}
}