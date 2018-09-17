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
			uint key;
			if (fc[0] == 0x81)
			{
				ByteConverter.BigEndian = true;
				key = 0x8125FE60;
			}
			else
			{
				ByteConverter.BigEndian = false;
				key = 0xC600000;
			}
			Dictionary<string, NJS_OBJECT> models = new Dictionary<string, NJS_OBJECT>();
			int ptr = fc.GetPointer(0x20, key);
			if (ptr != 0)
			{
				Upgrades = new EventUpgrade[18];
				for (int i = 0; i < 18; i++)
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
					Scenes.Add(new EventScene(fc, ptr, key, models));
					ptr += EventScene.Size;
				}
			}
		}

		public static NJS_OBJECT GetModel(byte[] file, int address, uint imageBase, Dictionary<string, NJS_OBJECT> models)
		{
			NJS_OBJECT result = null;
			int ptr = file.GetPointer(address, imageBase);
			if (ptr != 0)
			{
				result = new NJS_OBJECT(file, ptr, imageBase, ModelFormat.Chunk);
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
		public List<int> CameraMotionIDs { get; set; }
		public List<int> UnknownMotionIDs { get; set; }
		public int FrameCount { get; set; }

		public const int Size = 32;

		public EventScene(byte[] file, int address, uint imageBase, Dictionary<string, NJS_OBJECT> models)
		{
			int ptr = file.GetPointer(address, imageBase);
			if (ptr != 0)
			{
				int cnt = ByteConverter.ToInt32(file, address + 4);
				Entities = new List<EventEntity>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					Entities.Add(new EventEntity(file, ptr, imageBase, models));
					ptr += EventEntity.Size;
				}
			}
			ptr = file.GetPointer(address + 8, imageBase);
			if (ptr != 0)
			{
				int cnt = ByteConverter.ToInt32(file, address + 12);
				CameraMotionIDs = new List<int>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					CameraMotionIDs.Add(ByteConverter.ToInt32(file, ptr));
					ptr += sizeof(int);
				}
			}
			ptr = file.GetPointer(address + 16, imageBase);
			if (ptr != 0)
			{
				int cnt = ByteConverter.ToInt32(file, address + 20);
				UnknownMotionIDs = new List<int>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					UnknownMotionIDs.Add(ByteConverter.ToInt32(file, ptr));
					ptr += sizeof(int);
				}
			}
			FrameCount = ByteConverter.ToInt32(file, address + 28);
		}
	}

	public class EventEntity
	{
		[Browsable(false)]
		public NJS_OBJECT Model { get; set; }
		public int MotionID { get; set; }
		public int ShapeMotionID { get; set; }
		[Browsable(false)]
		public NJS_OBJECT Model2 { get; set; }
		public Vertex Position { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public int field_24 { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public int field_28 { get; set; }

		public const int Size = 44;

		public EventEntity(byte[] file, int address, uint imageBase, Dictionary<string, NJS_OBJECT> models)
		{
			Model = Event.GetModel(file, address, imageBase, models);
			MotionID = ByteConverter.ToInt32(file, address + 4);
			ShapeMotionID = ByteConverter.ToInt32(file, address + 8);
			Model2 = Event.GetModel(file, address + 16, imageBase, models);
			Position = new Vertex(file, address + 24);
			field_24 = ByteConverter.ToInt32(file, address + 36);
			field_28 = ByteConverter.ToInt32(file, address + 40);
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
