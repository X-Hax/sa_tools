using System;
using System.Globalization;
using System.IO;
using SonicRetro.SAModel;

namespace SplitNB
{
	class Program
	{
		static void Main(string[] args)
		{
			string dir = Environment.CurrentDirectory;
			try
			{
				string filename;
				if (args.Length > 0)
				{
					filename = args[0];
					Console.WriteLine("File: {0}", filename);
				}
				else
				{
					Console.Write("File: ");
					filename = Console.ReadLine();
				}
				Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename));
				byte[] file = File.ReadAllBytes(filename);
				if (BitConverter.ToInt32(file, 0) != 0x04424A4E)
				{
					Console.WriteLine("Invalid NB file.");
					Environment.CurrentDirectory = dir;
					return;
				}
				Environment.CurrentDirectory = Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename)).FullName;
				int numfiles = BitConverter.ToInt16(file, 4);
				int curaddr = 8;
				for (int i = 0; i < numfiles; i++)
				{
					ushort type = BitConverter.ToUInt16(file, curaddr);
					byte[] chunk = new byte[BitConverter.ToInt32(file, curaddr + 4)];
					Array.Copy(file, curaddr + 8, chunk, 0, chunk.Length);
					switch (type)
					{
						case 1:
							File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".bin", chunk);
							ModelFile.CreateFile(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".sa1mdl", ProcessModel(chunk), null, null, null, null, ModelFormat.Basic);
							break;
						case 3:
							File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".bin", chunk);
							//NJS_MOTION motion = NJS_MOTION.ReadDirect(file, 10, file.Length - 12, 8, ModelFormat.Basic, null);
							//motion.Save(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".saanim");
							break;
						default:
							File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".bin", chunk);
							break;
					}
					curaddr += chunk.Length + 8;
				}
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		static NJS_OBJECT ProcessModel(byte[] file)
		{
			return new NJS_OBJECT(file, file.Length - 52, 0, ModelFormat.Basic, null);
		}
	}
}