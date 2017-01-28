using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.SAEditorCommon.UI;
using System;
using System.ComponentModel;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
	public class MissionSETItem : SETItem
	{
		public MissionSETItem(EditorItemSelection selectionManager)
			: base(selectionManager)
		{ }

		public MissionSETItem(byte[] setfile, int setaddress, byte[] prmfile, int prmaddress, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			isLoaded = false;
			ushort _id = ByteConverter.ToUInt16(setfile, setaddress);
			ID = _id;
			ClipLevel = (byte)(_id >> 12);
			ushort xrot = BitConverter.ToUInt16(setfile, setaddress + 2);
			ushort yrot = BitConverter.ToUInt16(setfile, setaddress + 4);
			ushort zrot = BitConverter.ToUInt16(setfile, setaddress + 6);
			Rotation = new Rotation(xrot, yrot, zrot);
			Position = new Vertex(setfile, setaddress + 8);
			Scale = new Vertex(setfile, setaddress + 0x14);
			Array.Copy(prmfile, prmaddress, PRMBytes, 0, 0xC);
			isLoaded = true;
			objdef = GetObjectDefinition();
		}

		public override ObjectDefinition GetObjectDefinition()
		{
			switch (ObjectList)
			{
				case MsnObjectList.Level:
					return base.GetObjectDefinition();
				case MsnObjectList.Mission:
					if (id < LevelData.MisnObjDefs.Count)
						return LevelData.MisnObjDefs[id];
					else
						return DefaultObjectDefinition.DefaultInstance;
				default:
					throw new ArgumentOutOfRangeException("Invalid value for ObjectList.");
			}
		}

		private byte[] prmbytes = new byte[0xC];
		[ParenthesizePropertyName]
		[Description("The bytes from the PRM file associated with this object.")]
		public byte[] PRMBytes
		{
			get { return prmbytes; }
		}

		[Description("The mission this object is associated with, from 1 to 60.")]
		public byte MissionNumber
		{
			get { return (byte)(PRMBytes[0] + 1); }
			set { PRMBytes[0] = (byte)Math.Max(Math.Min(value - 1, 59), 0); }
		}

		[Description("When this object will appear.")]
		public Appear Appear
		{
			get { return (Appear)PRMBytes[1]; }
			set { PRMBytes[1] = (byte)value; }
		}

		[Description("The object list to use with the ID to determine the type of object. Changing the value resets ID to 0.")]
		public MsnObjectList ObjectList
		{
			get { return (MsnObjectList)PRMBytes[2]; }
			set
			{
				if (PRMBytes[2] != (byte)value)
				{
					PRMBytes[2] = (byte)value;
					id = 0;
					objdef = GetObjectDefinition();
				}
			}
		}

		public override void Paste()
		{
			LevelData.MissionSETItems[LevelData.Character].Add(this);
		}

		public override void Delete()
		{
			LevelData.MissionSETItems[LevelData.Character].Remove(this);
		}

		public byte[] GetPRMBytes()
		{
			return PRMBytes;
		}
	}

	public enum Appear
	{
		BeforeMission,
		DuringMission,
		DuringTimer = 5
	}

	public enum MsnObjectList { Level, Mission }
}
