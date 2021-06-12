using SAModel.SAEditorCommon.SETEditing;
using SAModel.SAEditorCommon.UI;
using System;
using System.ComponentModel;

namespace SAModel.SAEditorCommon.DataTypes
{
	public class MissionSETItem : SETItem
	{
		public MissionSETItem(MsnObjectList list, ushort id, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			ObjectList = list;
			ID = id;
			objdef = GetObjectDefinition();
			position = new Vertex();
			rotation = new Rotation(objdef.DefaultXRotation, objdef.DefaultYRotation, objdef.DefaultZRotation);
			scale = new Vertex(objdef.DefaultXScale, objdef.DefaultYScale, objdef.DefaultZScale);
			isLoaded = true;

			GetHandleMatrix();
		}

		public MissionSETItem(byte[] setfile, int setaddress, byte[] prmfile, int prmaddress, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			ushort _id = ByteConverter.ToUInt16(setfile, setaddress);
			ID = _id;
			ClipLevel = (byte)(_id >> 12);
			ushort xrot = BitConverter.ToUInt16(setfile, setaddress + 2);
			ushort yrot = BitConverter.ToUInt16(setfile, setaddress + 4);
			ushort zrot = BitConverter.ToUInt16(setfile, setaddress + 6);
			rotation = new Rotation(xrot, yrot, zrot);
			position = new Vertex(setfile, setaddress + 8);
			scale = new Vertex(setfile, setaddress + 0x14);
			Array.Copy(prmfile, prmaddress, PRMBytes, 0, 0xC);
			isLoaded = true;
			objdef = GetObjectDefinition();

			GetHandleMatrix();
		}
		[Category("Common")]
		public override Vertex Position { get { return position; } set { position = value; GetHandleMatrix(); } }
		[Category("Common")]
		public override Rotation Rotation { get { return rotation; } set { rotation = value; GetHandleMatrix(); } }

		protected override void GetHandleMatrix()
		{
			transformMatrix = GetObjectDefinition().GetHandleMatrix(this);
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
		[ParenthesizePropertyName(true)]
		[Category("Data"), Description("The bytes from the PRM file associated with this object.")]
		public byte[] PRMBytes
		{
			get { return prmbytes; }
		}

		[Category("Data"), Description("The mission this object is associated with, from 1 to 60.")]
		public byte MissionNumber
		{
			get { return (byte)(PRMBytes[0] + 1); }
			set { PRMBytes[0] = (byte)Math.Max(Math.Min(value - 1, 59), 0); }
		}

		[Category("Data"), Description("When this object will appear.")]
		public Appear Appear
		{
			get { return (Appear)PRMBytes[1]; }
			set { PRMBytes[1] = (byte)value; }
		}

		[Category("Data"), Description("The object list to use with the ID to determine the type of object. Changing the value resets ID to 0.")]
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

		protected override void DeleteInternal(EditorItemSelection selectionManager)
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
