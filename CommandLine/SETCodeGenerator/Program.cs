using SplitTools;
using System;
using System.IO;
using Vertex = SAModel.Vertex;
using Rotation = SAModel.Rotation;
using SAModel;

namespace SETCodeGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			string listname;
			Console.Write("List: ");
			if (args.Length > 0)
				Console.WriteLine(listname = args[0]);
			else
				listname = Console.ReadLine();

			string setname;
			Console.Write("SET: ");
			if (args.Length > 1)
				Console.WriteLine(setname = args[1]);
			else
				setname = Console.ReadLine();

			string cppname;
			Console.Write("Code: ");
			if (args.Length > 2)
				Console.WriteLine(cppname = args[1]);
			else
				cppname = Console.ReadLine();

			ObjectListEntry[] objlstini = ObjectList.Load(listname, false);

			byte[] setfile = File.ReadAllBytes(setname);

			using (StreamWriter sw = File.CreateText(cppname))
			{
				for (int i = 0; i < objlstini.Length; i++)
					sw.WriteLine("ObjectFunc(OF{0}, {1}); // {2}", i, objlstini[i].Code.ToCHex(), objlstini[i].Name);

				sw.WriteLine("void LoadObjects()");
				sw.WriteLine("{");
				sw.WriteLine("\tObjectMaster *obj;");
				sw.WriteLine("\tEntityData1 *ent;");
				int count = BitConverter.ToInt32(setfile, 0);
				int address = 0x20;
				for (int i = 0; i < count; i++)
				{
					SETItem ent = new SETItem(setfile, address);
					sw.WriteLine("\tobj = LoadObject((LoadObj){0}, {1}, OF{2}); // {3}", objlstini[ent.ID].Arg1, objlstini[ent.ID].Arg2, ent.ID, objlstini[ent.ID].Name);
					sw.WriteLine("\tif (obj)");
					sw.WriteLine("\t{");
					sw.WriteLine("\t\tent = obj->Data1;");
					if (ent.Position.X != 0)
						sw.WriteLine("\t\tent->Position.x = {0};", ent.Position.X.ToC());
					if (ent.Position.Y != 0)
						sw.WriteLine("\t\tent->Position.y = {0};", ent.Position.Y.ToC());
					if (ent.Position.Z != 0)
						sw.WriteLine("\t\tent->Position.z = {0};", ent.Position.Z.ToC());
					if (ent.Rotation.X != 0)
						sw.WriteLine("\t\tent->Rotation.x = {0};", ent.Rotation.X.ToCHex());
					if (ent.Rotation.Y != 0)
						sw.WriteLine("\t\tent->Rotation.y = {0};", ent.Rotation.Y.ToCHex());
					if (ent.Rotation.Z != 0)
						sw.WriteLine("\t\tent->Rotation.z = {0};", ent.Rotation.Z.ToCHex());
					if (ent.Scale.X != 0)
						sw.WriteLine("\t\tent->Scale.x = {0};", ent.Scale.X.ToC());
					if (ent.Scale.Y != 0)
						sw.WriteLine("\t\tent->Scale.y = {0};", ent.Scale.Y.ToC());
					if (ent.Scale.Z != 0)
						sw.WriteLine("\t\tent->Scale.z = {0};", ent.Scale.Z.ToC());
					sw.WriteLine("\t}");
					address += 0x20;
				}
				sw.WriteLine("}");
			}
		}
	}

	class SETItem
	{
		public SETItem(byte[] file, int address)
		{
			ushort _id = ByteConverter.ToUInt16(file, address);
			ID = _id;
			ClipLevel = (byte)(_id >> 12);
			ushort xrot = BitConverter.ToUInt16(file, address + 2);
			ushort yrot = BitConverter.ToUInt16(file, address + 4);
			ushort zrot = BitConverter.ToUInt16(file, address + 6);
			Rotation = new Rotation(xrot, yrot, zrot);
			Position = new Vertex(file, address + 8);
			Scale = new Vertex(file, address + 0x14);
		}

		protected ushort id;
		public ushort ID
		{
			get { return id; }
			set { id = (ushort)(value & 0xFFF); }
		}

		private ushort cliplevel;
		public ushort ClipLevel
		{
			get { return cliplevel; }
			set { cliplevel = (byte)(value & 0xF); }
		}

		public Vertex Position { get; set; }

		public Rotation Rotation { get; set; }

		public Vertex Scale { get; set; }
	}
}
