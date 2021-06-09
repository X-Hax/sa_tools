using FraGag.Compression;
using SA_Tools;
using SonicRetro.SAModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using ByteConverter = SonicRetro.SAModel.ByteConverter;

namespace SA2EventViewer
{
	public class Event
	{
		public List<EventScene> Scenes { get; set; }
		public EventUpgrade[] Upgrades { get; set; }

		public Event(string filename)
		{
			byte[] fc;
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				fc = Prs.Decompress(filename);
			else
				fc = File.ReadAllBytes(filename);
			bool battle = false;
			uint key;
			if (fc[0] == 0x81)
			{
				ByteConverter.BigEndian = true;
				key = 0x8125FE60;
				battle = true;
			}
			else
			{
				ByteConverter.BigEndian = false;
				key = 0xC600000;
			}
			List<NJS_MOTION> motions = null;
			if (battle)
				motions = ReadMotionFile(Path.ChangeExtension(filename, null) + "motion.bin");
			Dictionary<string, NJS_OBJECT> models = new Dictionary<string, NJS_OBJECT>();
			int ptr = fc.GetPointer(0x20, key);
			if (ptr != 0)
			{
				int cnt = battle ? 18 : 16;
				Upgrades = new EventUpgrade[cnt];
				for (int i = 0; i < cnt; i++)
				{
					Upgrades[i] = new EventUpgrade(fc, ptr, key, models);
					ptr += EventUpgrade.Size;
				}
			}
			int gcnt = ByteConverter.ToInt32(fc, 8);
			ptr = fc.GetPointer(0, key);
			if (ptr != 0)
			{
				Scenes = new List<EventScene>();
				for (int gn = 0; gn <= gcnt; gn++)
				{
					Scenes.Add(new EventScene(fc, ptr, key, battle, models, motions));
					ptr += EventScene.Size;
				}
			}
		}

		public static List<NJS_MOTION> ReadMotionFile(string filename)
		{
			List<NJS_MOTION> motions = new List<NJS_MOTION>();
			byte[] fc = File.ReadAllBytes(filename);
			int addr = 0;
			while (ByteConverter.ToInt64(fc, addr) != 0)
			{
				int ptr = ByteConverter.ToInt32(fc, addr);
				if (ptr == -1)
					motions.Add(null);
				else
					motions.Add(new NJS_MOTION(fc, ptr, 0, ByteConverter.ToInt32(fc, addr + 4)));
				addr += 8;
			}
			return motions;
		}

		public static NJS_OBJECT GetModel(byte[] file, int address, uint imageBase, Dictionary<string, NJS_OBJECT> models)
		{
			NJS_OBJECT result = null;
			int ptr = file.GetPointer(address, imageBase);
			if (ptr != 0)
			{
				result = new NJS_OBJECT(file, ptr, imageBase, ModelFormat.Chunk, null);
				if (models.ContainsKey(result.Name))
					result = models[result.Name];
				else
					foreach (NJS_OBJECT obj in result.GetObjects())
						models[obj.Name] = obj;
			}
			return result;
		}

		public static NJS_OBJECT GetGCModel(byte[] file, int address, uint imageBase, Dictionary<string, NJS_OBJECT> models)
		{
			NJS_OBJECT result = null;
			int ptr = file.GetPointer(address, imageBase);
			if (ptr != 0)
			{
				result = new NJS_OBJECT(file, ptr, imageBase, ModelFormat.GC, null);
				if (models.ContainsKey(result.Name))
					result = models[result.Name];
				else
					foreach (NJS_OBJECT obj in result.GetObjects())
						models[obj.Name] = obj;
			}
			return result;
		}
	}

	public class EventScene
	{
		public List<EventEntity> Entities { get; set; }
		public List<NJS_MOTION> CameraMotions { get; set; }
		public EventBig Big { get; set; }
		public int FrameCount { get; set; }

		public const int Size = 32;

		public EventScene(byte[] file, int address, uint imageBase, bool battle, Dictionary<string, NJS_OBJECT> models, List<NJS_MOTION> motions)
		{
			int ptr = file.GetPointer(address, imageBase);
			if (ptr != 0)
			{
				int cnt = ByteConverter.ToInt32(file, address + 4);
				Entities = new List<EventEntity>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					Entities.Add(new EventEntity(file, ptr, imageBase, battle, models, motions));
					ptr += EventEntity.Size(battle);
				}
			}
			ptr = file.GetPointer(address + 8, imageBase);
			if (ptr != 0)
			{
				int cnt = ByteConverter.ToInt32(file, address + 12);
				CameraMotions = new List<NJS_MOTION>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					if (battle)
						CameraMotions.Add(motions[ByteConverter.ToInt32(file, ptr)]);
					else
						CameraMotions.Add(new NJS_MOTION(file, file.GetPointer(ptr, imageBase), imageBase, 1));
					ptr += sizeof(int);
				}
			}
			ptr = file.GetPointer(address + 24, imageBase);
			if (ptr != 0)
				Big = new EventBig(file, ptr, imageBase, battle, models, motions);
			FrameCount = ByteConverter.ToInt32(file, address + 28);
		}
	}

	public class EventEntity
	{
		[Browsable(false)]
		public NJS_OBJECT Model { get; set; }
		[Browsable(false)]
		public NJS_MOTION Motion { get; set; }
		[Browsable(false)]
		public NJS_MOTION ShapeMotion { get; set; }
		[Browsable(false)]
		public NJS_OBJECT GCModel { get; set; }
		[Browsable(false)]
		public NJS_OBJECT ShadowModel { get; set; }
		public Vertex Position { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint Flags { get; set; }
		public uint Layer { get; set; }

		public static int Size(bool battle) => battle ? 44 : 32;

		public EventEntity(byte[] file, int address, uint imageBase, bool battle, Dictionary<string, NJS_OBJECT> models, List<NJS_MOTION> motions)
		{
			Model = Event.GetModel(file, address, imageBase, models);
			if (battle)
			{
				Motion = motions[ByteConverter.ToInt32(file, address + 4)];
				ShapeMotion = motions[ByteConverter.ToInt32(file, address + 8)];
				GCModel = Event.GetGCModel(file, address + 12, imageBase, models);
				ShadowModel = Event.GetModel(file, address + 16, imageBase, models);
				Position = new Vertex(file, address + 24);
				Flags = ByteConverter.ToUInt32(file, address + 36);
				Layer = ByteConverter.ToUInt32(file, address + 40);
			}
			else
			{
				int ptr = file.GetPointer(address + 4, imageBase);
				if (ptr != 0)
					Motion = new NJS_MOTION(file, ptr, imageBase, Model.CountAnimated());
				ptr = file.GetPointer(address + 8, imageBase);
				if (ptr != 0)
					ShapeMotion = new NJS_MOTION(file, ptr, imageBase, Model.CountMorph());
				Position = new Vertex(file, address + 16);
				Flags = ByteConverter.ToUInt32(file, address + 28);
			}
		}
	}

	public class EventBig
	{
		public NJS_OBJECT Model { get; set; }
		public List<(NJS_MOTION a, NJS_MOTION b)> Motions { get; set; }
		public int Unknown { get; set; }

		public EventBig(byte[] file, int address, uint imageBase, bool battle, Dictionary<string, NJS_OBJECT> models, List<NJS_MOTION> motions)
		{
			Model = Event.GetModel(file, address, imageBase, models);
			int ptr = file.GetPointer(address + 4, imageBase);
			if (ptr != 0)
			{
				int cnt = ByteConverter.ToInt32(file, address + 8);
				Motions = new List<(NJS_MOTION a, NJS_MOTION b)>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					if (battle)
						Motions.Add((motions[ByteConverter.ToInt32(file, ptr)], motions[ByteConverter.ToInt32(file, ptr + 4)]));
					else
					{
						NJS_MOTION a = null;
						int ptr2 = file.GetPointer(ptr, imageBase);
						if (ptr2 != 0)
							a = new NJS_MOTION(file, ptr2, imageBase, Model.CountAnimated());
						NJS_MOTION b = null;
						ptr2 = file.GetPointer(ptr + 4, imageBase);
						if (ptr2 != 0)
							b = new NJS_MOTION(file, ptr2, imageBase, Model.CountAnimated());
						Motions.Add((a, b));
					}
					ptr += 8;
				}
			}
			Unknown = ByteConverter.ToInt32(file, address + 12);
		}
	}

	public class EventUpgrade
	{
		public NJS_OBJECT RootNode { get; set; }
		public NJS_OBJECT AttachNode1 { get; set; }
		public NJS_OBJECT Model1 { get; set; }
		public NJS_OBJECT AttachNode2 { get; set; }
		public NJS_OBJECT Model2 { get; set; }

		public const int Size = 20;

		public EventUpgrade(byte[] file, int address, uint imageBase, Dictionary<string, NJS_OBJECT> models)
		{
			RootNode = Event.GetModel(file, address, imageBase, models);
			AttachNode1 = Event.GetModel(file, address + 4, imageBase, models);
			Model1 = Event.GetModel(file, address + 8, imageBase, models);
			AttachNode2 = Event.GetModel(file, address + 12, imageBase, models);
			Model2 = Event.GetModel(file, address + 16, imageBase, models);
		}
	}
}
