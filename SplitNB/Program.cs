using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SonicRetro.SAModel;

namespace SplitNB
{
	class Program
	{
		private enum AnimSections
		{
			MKEY_F = 0,
			MKEY_A = 1,
			MDATA2_HEADER = 3,
			MOTION = 4,
		}
		private enum ModelSections
		{
			POLY = 0,
			VERTEX = 1,
			NORMAL = 2,
			UV = 3,
			MATERIAL = 4,
			MESHSET = 5,
			MODEL = 6,
			OBJECT = 7,
		}
		static void Main(string[] args)
		{
			Dictionary<int, NJS_OBJECT> modellist = new Dictionary<int, NJS_OBJECT>();
			Dictionary<int, NJS_MOTION> animlist = new Dictionary<int, NJS_MOTION>();
			bool extractchunks = false;
			string dir = Environment.CurrentDirectory;
			string filename;
			if (args.Length > 0)
			{
				filename = args[0];
				Console.WriteLine("File: {0}", filename);
				if (args.Length > 1 && args[1] == "-b") extractchunks = true;
			}
			else
			{
				Console.WriteLine("SplitNB extracts models and animations from .NB files.\n");
				Console.WriteLine("Usage: SplitNB <NB file> [-b]\n");
				Console.WriteLine("Arguments:");
				Console.WriteLine("-b to extract binary chunks from the NB file\n");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
			Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename));
			byte[] file = File.ReadAllBytes(filename);
			if (BitConverter.ToInt32(file, 0) != 0x04424A4E)
			{
				Console.WriteLine("Invalid NB file.");
				Environment.CurrentDirectory = dir;
				return;
			}
			if (Directory.Exists(Path.GetFileNameWithoutExtension(filename))) Directory.Delete(Path.GetFileNameWithoutExtension(filename), true);
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
					case 0:
						Console.Write("\nSection {0} at {1} is empty", i.ToString("D2", NumberFormatInfo.InvariantInfo), curaddr.ToString("X"));
						break;
					case 1:
						Console.WriteLine("\nSection {0} at {1} is a model", i.ToString("D2", NumberFormatInfo.InvariantInfo), curaddr.ToString("X"));
						if (extractchunks) File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".bin", chunk);
						NJS_OBJECT mdl = ProcessModel(chunk);
						modellist.Add(i, mdl);
						break;
					case 3:
						Console.WriteLine("\nSection {0} at {1} is a motion", i.ToString("D2", NumberFormatInfo.InvariantInfo), curaddr.ToString("X"));
						if (extractchunks) File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".bin", chunk);
						NJS_MOTION mot = ProcessMotion(chunk);
						animlist.Add(i, mot);
						break;
					default:
						Console.WriteLine("\nSection {0} at {1} is an unknown type", i.ToString("D2", NumberFormatInfo.InvariantInfo), curaddr.ToString("X"));
						if (extractchunks) File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".bin", chunk);
						break;
				}
				curaddr += chunk.Length + 8;
			}
			//Save models and animations
			foreach (var modelitem in modellist)
			{
				List<string> anims = new List<string>();
				foreach (var animitem in animlist)
				{
					if (modelitem.Value.CountAnimated() == animitem.Value.ModelParts)
					{
						animitem.Value.Save(animitem.Key.ToString("D2") + ".saanim");
						anims.Add(animitem.Key.ToString("D2") + ".saanim");
					}
				}
				ModelFile.CreateFile(modelitem.Key.ToString("D2") + ".sa1mdl", modelitem.Value, anims.ToArray(), null, null, null, ModelFormat.Basic);
			}
		}
		static NJS_MOTION ProcessMotion(byte[] file)
		{
			int frames;
			InterpolationMode intmode;
			AnimFlags animtype;
			int curaddr = 0;
			do
			{
				AnimSections section_type = (AnimSections)BitConverter.ToInt16(file, curaddr);
				int section_size = BitConverter.ToInt32(file, curaddr + 4);
				int section_addr = curaddr + 8;
				Console.Write("Subsection type {0}, size {1}, data begins at {2}", section_type.ToString(), section_size, section_addr.ToString("X"));
				switch (section_type)
				{
					case AnimSections.MKEY_A:
						Console.Write(", calculated item count: {0}\n", (float)section_size / 16.0f);
						break;
					case AnimSections.MKEY_F:
						Console.Write(", calculated item count: {0}\n", (float)section_size / 16.0f);
						break;
					case AnimSections.MDATA2_HEADER:
						Console.Write(", number of MDATA entries: {0}\n", BitConverter.ToInt32(file, section_addr) >> BitConverter.ToInt32(file, section_addr + 4));
						break;
					case AnimSections.MOTION:
						frames = BitConverter.ToInt32(file, section_addr + 4);
						intmode = (InterpolationMode)BitConverter.ToInt16(file, section_addr + 10);
						animtype = (AnimFlags)BitConverter.ToInt16(file, section_addr + 8);
						int mdataaddr = BitConverter.ToInt32(file, section_addr);
						Console.Write("\nMDATA header at: {0}, frames: {1}, flags: {2}, interpolation: {3}", mdataaddr.ToString("X"), frames, animtype.ToString(), intmode.ToString());
						//Create motion stub
						NJS_MOTION mot = new NJS_MOTION();
						mot.Name = "animation_" + section_addr.ToString("X8");
						mot.MdataName = mot.Name + "_mdat";
						mot.Frames = frames;
						mot.InterpolationMode = intmode;
						//Read the MDATA header and get the number of MDATA entries
						mot.ModelParts = BitConverter.ToInt32(file, mdataaddr) >> BitConverter.ToInt32(file, mdataaddr + 4);
						Console.WriteLine(", model parts: {0}", mot.ModelParts);
						int tmpaddr = mdataaddr + 8; //Start of actual MDATA array
						for (int u = 0; u < mot.ModelParts; u++)
						{
							//Console.Write("\nMotion data {0}", u.ToString());
							AnimModelData data = new AnimModelData();
							if (animtype.HasFlag(AnimFlags.Position))
							{
								uint posoff = ByteConverter.ToUInt32(file, tmpaddr + u * 16);
								data.PositionName = mot.Name + "_mkey_" + u.ToString() + "_pos_" + posoff.ToString("X8");
								int pos_count = ByteConverter.ToInt32(file, tmpaddr + u * 16 + 8);
								//Console.Write(", position at: {0} ({1} entries)", posoff.ToString("X"), pos_count);
								for (int p = 0; p < pos_count; p++)
								{
									int index = ByteConverter.ToInt32(file, (int)posoff + p * 16);
									float pos_x = ByteConverter.ToSingle(file, (int)posoff + p * 16 + 4);
									float pos_y = ByteConverter.ToSingle(file, (int)posoff + p * 16 + 8);
									float pos_z = ByteConverter.ToSingle(file, (int)posoff + p * 16 + 12);
									//Console.WriteLine("\nAdded position index {3}: X: {0} Y: {1} Z: {2}", pos_x, pos_y, pos_z, index);
									data.Position.Add(index, new Vertex(pos_x, pos_y, pos_z));
								}
							}
							if (animtype.HasFlag(AnimFlags.Rotation))
							{
								uint rotoff = ByteConverter.ToUInt32(file, tmpaddr + u * 16 + 4);
								data.RotationName = mot.Name + "_mkey_" + u.ToString() + "_rot_" + rotoff.ToString("X8");
								int rot_count = ByteConverter.ToInt32(file, tmpaddr + u * 16 + 12);
								//Console.Write(", rotation at: {0} ({1} entries)", rotoff.ToString("X"), rot_count);
								for (int p = 0; p < rot_count; p++)
								{
									int index = ByteConverter.ToInt32(file, (int)rotoff + p * 16);
									int rot_x = ByteConverter.ToInt32(file, (int)rotoff + p * 16 + 4);
									int rot_y = ByteConverter.ToInt32(file, (int)rotoff + p * 16 + 8);
									int rot_z = ByteConverter.ToInt32(file, (int)rotoff + p * 16 + 12);
									//Console.WriteLine("\nAdded rotation index {3}: X: {0} Y: {1} Z: {2}", rot_x, rot_y, rot_z, index);
									data.Rotation.Add(index, new Rotation(rot_x, rot_y, rot_z));
								}
							}
							mot.Models.Add(u, data);
						}
						return mot;
				}
				curaddr += 8 + section_size;
			}
			while (curaddr < file.Length);
			return null;
		}
		static NJS_OBJECT ProcessModel(byte[] file)
		{
			//Like animations, model sections also have subsections. 
			//Each subsection begins with an 8-byte header that specifies subsection type and size, which is then followed by data.
			//The pointers are all valid and we can just get the root object from the end of the file, but subsections still need to be figured out to rebuild NB files.
			int curaddr = 0;
			do
			{
				int temp = BitConverter.ToInt16(file, curaddr);
				if (temp < 0 || temp > 7)
				{
					Console.WriteLine("Skipping two padding bytes at {0}", curaddr.ToString("X"));
					curaddr += 2;
				}
				ModelSections section_type = (ModelSections)BitConverter.ToInt16(file, curaddr);
				int itemnum = BitConverter.ToInt16(file, curaddr + 2);
				int section_size = BitConverter.ToInt32(file, curaddr + 4);
				int section_addr = curaddr + 8;
				Console.Write("Subsection type {0}, size {1}, data begins at {2}", section_type.ToString(), section_size, section_addr.ToString("X"));
				switch (section_type)
				{
					case ModelSections.POLY:
						Console.Write(", calculated item count: {0}\n", (float)section_size / 2.0f);
						break;
					case ModelSections.VERTEX:
						Console.Write(", calculated item count: {0}\n", (float)section_size / 12.0f);
						break;
					case ModelSections.NORMAL:
						Console.Write(", calculated item count: {0}\n", (float)section_size / 12.0f);
						break;
					case ModelSections.UV:
						Console.Write(", calculated item count: {0}\n", (float)section_size / 4.0f);
						break;
					case ModelSections.MATERIAL:
						Console.Write(", item count: {0} (calculated {1})\n", itemnum, (float)section_size / 20.0f);
						break;
					case ModelSections.MESHSET:
						Console.Write(", item count: {0} (calculated {1})\n", itemnum, (float)section_size / 24.0f);
						break;
					case ModelSections.MODEL:
						Console.Write(", item count: {0} (calculated {1})\n", itemnum, (float)section_size / 40.0f);
						break;
					case ModelSections.OBJECT:
						Console.Write(", item count: {0} (calculated {1})\n", itemnum, (float)section_size / 52.0f);
						break;
				}
				curaddr += 8 + section_size;
			}
			while (curaddr < file.Length);
			return new NJS_OBJECT(file, file.Length - 52, 0, ModelFormat.Basic, null);
		}
	}
}