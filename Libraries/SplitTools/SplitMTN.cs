using SAModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SplitTools.SAArc
{
	public static class SA2MTN
	{
		public static void Split(string filename)
		{
			var dir = Environment.CurrentDirectory;
			
			try
			{
				filename = Path.Combine(Environment.CurrentDirectory, filename);
				Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename));
				
				var file = File.ReadAllBytes(filename);
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					file = FraGag.Compression.Prs.Decompress(file);
				}

				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename));
				var processedAnims = new Dictionary<int, int>();
				var addr = 0;
				var ile = ByteConverter.ToUInt16(file, 0);
				
				if (ile == 0)
				{
					ile = ByteConverter.ToUInt16(file, 8);
					addr = 8;
				}
				
				ByteConverter.BigEndian = true;
				
				if (ile < ByteConverter.ToUInt16(file, addr))
				{
					ByteConverter.BigEndian = false;
				}

				var ini = new MTNInfo { BigEndian = ByteConverter.BigEndian };
				var address = 0;
				var i = ByteConverter.ToInt16(file, address);
				
				while (i != -1)
				{
					var aniaddr = ByteConverter.ToInt32(file, address + 4);
					
					if (!processedAnims.ContainsKey(aniaddr))
					{
						new NJS_MOTION(file, aniaddr, 0, ByteConverter.ToInt16(file, address + 2))
							.Save(Path.GetFileNameWithoutExtension(filename) + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
						
						processedAnims[aniaddr] = i;
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

		public static void Build(bool? isBigEndian, string mtnFilename)
		{
			var dir = Environment.CurrentDirectory;
			
			try
			{
				mtnFilename = Path.Combine(Environment.CurrentDirectory, mtnFilename);
				if (Directory.Exists(mtnFilename))
				{
					mtnFilename += ".prs";
				}

				Environment.CurrentDirectory = Path.GetDirectoryName(mtnFilename);
				var anims = new List<NJS_MOTION>();
				
				foreach (var file in Directory.GetFiles(Path.GetFileNameWithoutExtension(mtnFilename), "*.saanim"))
				{
					if (short.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var i))
					{
						anims.Add(NJS_MOTION.Load(file));
					}
				}

				var mtnInfo = IniSerializer.Deserialize<MTNInfo>(Path.Combine(Path.GetFileNameWithoutExtension(mtnFilename), Path.GetFileNameWithoutExtension(mtnFilename) + ".ini"));
				
				ByteConverter.BigEndian = isBigEndian ?? mtnInfo.BigEndian;

				var animBytes = new List<byte>();
				var animAddress = new Dictionary<string, int>();
				var animParts = new Dictionary<string, short>();
				var imageBase = (uint)(mtnInfo.Indexes.Count * 8) + 8;
				
				foreach (var item in anims)
				{
					animBytes.AddRange(item.GetBytes(imageBase, out var address));
					animAddress[item.Name] = (int)(address + imageBase);
					animParts[item.Name] = (short)item.ModelParts;
					imageBase = (uint)(mtnInfo.Indexes.Count * 8) + 8 + (uint)animBytes.Count;
				}
				
				var mtnFile = new List<byte>();
				
				foreach (var item in mtnInfo.Indexes)
				{
					if (!animParts.TryGetValue(item.Value, out short animPart))
					{
						throw new ArgumentOutOfRangeException(nameof(mtnFilename), $"The animation {item.Value} is missing. Please check your .ini file and make sure the animation name matches.");
					}

					mtnFile.AddRange(ByteConverter.GetBytes(item.Key));
					mtnFile.AddRange(ByteConverter.GetBytes(animPart));
					mtnFile.AddRange(ByteConverter.GetBytes(animAddress[item.Value]));
				}
				
				mtnFile.AddRange([0xFF, 0xFF, 0, 0, 0, 0, 0, 0]);
				mtnFile.AddRange(animBytes);
				
				if (Path.GetExtension(mtnFilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					FraGag.Compression.Prs.Compress(mtnFile.ToArray(), mtnFilename);
				}
				else
				{
					File.WriteAllBytes(mtnFilename, mtnFile.ToArray());
				}
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}
	}
}
