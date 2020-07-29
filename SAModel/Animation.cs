using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SonicRetro.SAModel
{
	public class NJS_ACTION
	{
		public NJS_OBJECT Model { get; private set; }
		public NJS_MOTION Animation { get; private set; }

		public NJS_ACTION(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, Attach> attaches)
			: this(file, address, imageBase, format, new Dictionary<int, string>(), attaches)
		{
		}

		public NJS_ACTION(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels, Dictionary<int, Attach> attaches)
		{
			Model = new NJS_OBJECT(file, (int)(ByteConverter.ToUInt32(file, address) - imageBase), imageBase, format, attaches);
			Animation = new NJS_MOTION(file, (int)(ByteConverter.ToUInt32(file, address + 4) - imageBase), imageBase,
				Model.CountAnimated(), labels);
		}
		public NJS_ACTION(NJS_OBJECT model, NJS_MOTION animation)
		{
			Model = model;
			Animation = animation;
		}

		public byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
		{
			List<byte> result = new List<byte>();
			result.AddRange(Model.GetBytes(imageBase, DX, labels, out uint modeladdr));
			uint tmp = (uint)result.Count;
			result.AddRange(Animation.GetBytes(imageBase + tmp, labels, out uint head2));
			address = (uint)result.Count;
			result.AddRange(ByteConverter.GetBytes(modeladdr + imageBase));
			result.AddRange(ByteConverter.GetBytes(head2 + tmp + imageBase));
			return result.ToArray();
		}

		public byte[] GetBytes(uint imageBase, bool DX, out uint address)
		{
			return GetBytes(imageBase, DX, new Dictionary<string, uint>(), out address);
		}

		public byte[] GetBytes(uint imageBase, bool DX)
		{
			return GetBytes(imageBase, DX, out uint address);
		}
	}

	public class NJS_MOTION
	{
		public const ulong SAANIM = 0x4D494E414153u;
		public const ulong FormatMask = 0xFFFFFFFFFFFFu;
		public const ulong CurrentVersion = 1;
		public const ulong SAANIMVer = SAANIM | (CurrentVersion << 56);

		public int Frames { get; set; }
		public string Name { get; set; }
		public int ModelParts { get; set; }
		public InterpolationMode InterpolationMode { get; set; }
		public bool ShortRot { get; set; }

		public Dictionary<int, AnimModelData> Models = new Dictionary<int, AnimModelData>();

		public NJS_MOTION()
		{
			Name = "animation_" + Extensions.GenerateIdentifier();
		}

		public NJS_MOTION(byte[] file, int address, uint imageBase, int nummodels, Dictionary<int, string> labels = null, bool shortrot = false)
		{
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "animation_" + address.ToString("X8");
			if (address > file.Length - 12) return;
			Frames = ByteConverter.ToInt32(file, address + 4);
			AnimFlags animtype = (AnimFlags)ByteConverter.ToUInt16(file, address + 8);
			ushort tmp = ByteConverter.ToUInt16(file, address + 10);
			switch ((StructEnums.NJD_MTYPE_FN)tmp & StructEnums.NJD_MTYPE_FN.NJD_MTYPE_MASK)
			{
				case StructEnums.NJD_MTYPE_FN.NJD_MTYPE_LINER:
					InterpolationMode = InterpolationMode.Linear;
					break;
				case StructEnums.NJD_MTYPE_FN.NJD_MTYPE_SPLINE:
					InterpolationMode = InterpolationMode.Spline;
					break;
				case StructEnums.NJD_MTYPE_FN.NJD_MTYPE_USER:
					InterpolationMode = InterpolationMode.User;
					break;
			}
			ShortRot = shortrot;
			int framesize = (tmp & 0xF) * 8;
			address = (int)(ByteConverter.ToUInt32(file, address) - imageBase);
			for (int i = 0; i < nummodels; i++)
			{
				AnimModelData data = new AnimModelData();
				bool hasdata = false;
				uint posoff = 0;
				if (address > file.Length - 4) continue;
				if (animtype.HasFlag(AnimFlags.Position))
				{
					posoff = ByteConverter.ToUInt32(file, address);
					if (posoff > 0)
						posoff -= imageBase;
					address += 4;
				}
				uint rotoff = 0;
				if (animtype.HasFlag(AnimFlags.Rotation))
				{
					rotoff = ByteConverter.ToUInt32(file, address);
					if (rotoff > 0)
						rotoff -= imageBase;
					address += 4;
				}
				uint scloff = 0;
				if (animtype.HasFlag(AnimFlags.Scale))
				{
					scloff = ByteConverter.ToUInt32(file, address);
					if (scloff > 0)
						scloff -= imageBase;
					address += 4;
				}
				uint vecoff = 0;
				if (animtype.HasFlag(AnimFlags.Vector))
				{
					vecoff = ByteConverter.ToUInt32(file, address);
					if (vecoff > 0)
						vecoff -= imageBase;
					address += 4;
				}
				uint vertoff = 0;
				if (animtype.HasFlag(AnimFlags.Vertex))
				{
					vertoff = ByteConverter.ToUInt32(file, address);
					if (vertoff > 0)
						vertoff -= imageBase;
					address += 4;
				}
				uint normoff = 0;
				if (animtype.HasFlag(AnimFlags.Normal))
				{
					normoff = ByteConverter.ToUInt32(file, address);
					if (normoff > 0)
						normoff -= imageBase;
					address += 4;
				}
				uint targoff = 0;
				if (animtype.HasFlag(AnimFlags.Target))
				{
					targoff = ByteConverter.ToUInt32(file, address);
					if (targoff > 0)
						targoff -= imageBase;
					address += 4;
				}
				uint rolloff = 0;
				if (animtype.HasFlag(AnimFlags.Roll))
				{
					rolloff = ByteConverter.ToUInt32(file, address);
					if (rolloff > 0)
						rolloff -= imageBase;
					address += 4;
				}
				uint angoff = 0;
				if (animtype.HasFlag(AnimFlags.Angle))
				{
					angoff = ByteConverter.ToUInt32(file, address);
					if (angoff > 0)
						angoff -= imageBase;
					address += 4;
				}
				uint coloff = 0;
				if (animtype.HasFlag(AnimFlags.Color))
				{
					coloff = ByteConverter.ToUInt32(file, address);
					if (coloff > 0)
						coloff -= imageBase;
					address += 4;
				}
				uint intoff = 0;
				if (animtype.HasFlag(AnimFlags.Intensity))
				{
					intoff = ByteConverter.ToUInt32(file, address);
					if (intoff > 0)
						intoff -= imageBase;
					address += 4;
				}
				uint spotoff = 0;
				if (animtype.HasFlag(AnimFlags.Spot))
				{
					spotoff = ByteConverter.ToUInt32(file, address);
					if (spotoff > 0)
						spotoff -= imageBase;
					address += 4;
				}
				uint pntoff = 0;
				if (animtype.HasFlag(AnimFlags.Point))
				{
					pntoff = ByteConverter.ToUInt32(file, address);
					if (pntoff > 0)
						pntoff -= imageBase;
					address += 4;
				}
				int tmpaddr;
				if (animtype.HasFlag(AnimFlags.Position))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (posoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)posoff;
						for (int j = 0; j < frames; j++)
						{
							data.Position.Add(ByteConverter.ToInt32(file, tmpaddr), new Vertex(file, tmpaddr + 4));
							tmpaddr += 16;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Rotation))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (rotoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)rotoff;
						for (int j = 0; j < frames; j++)
						{
							if (shortrot)
							{
								if (!data.Rotation.ContainsKey(ByteConverter.ToInt16(file, tmpaddr))) data.Rotation.Add(ByteConverter.ToInt16(file, tmpaddr), new Rotation(ByteConverter.ToInt16(file, tmpaddr + 2), ByteConverter.ToInt16(file, tmpaddr + 4), ByteConverter.ToInt16(file, tmpaddr + 6)));
								tmpaddr += 8;
							}
							else
							{
								if (!data.Rotation.ContainsKey(ByteConverter.ToInt32(file, tmpaddr))) data.Rotation.Add(ByteConverter.ToInt32(file, tmpaddr), new Rotation(file, tmpaddr + 4));
								tmpaddr += 16;
							}
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Scale))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (scloff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)scloff;
						for (int j = 0; j < frames; j++)
						{
							data.Scale.Add(ByteConverter.ToInt32(file, tmpaddr), new Vertex(file, tmpaddr + 4));
							tmpaddr += 16;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Vector))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (vecoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)vecoff;
						for (int j = 0; j < frames; j++)
						{
							data.Vector.Add(ByteConverter.ToInt32(file, tmpaddr), new Vertex(file, tmpaddr + 4));
							tmpaddr += 16;
						}
					}
					address += 4;
				}
				int vtxcount = -1;
				if (animtype.HasFlag(AnimFlags.Vertex))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (vertoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)vertoff;
						List<int> ptrs = new List<int>();
						for (int j = 0; j < frames; j++)
						{
							ptrs.AddUnique((int)(ByteConverter.ToUInt32(file, tmpaddr + 4) - imageBase));
							tmpaddr += 8;
						}
						if (ptrs.Count > 1)
						{
							ptrs.Sort();
							vtxcount = (ptrs[1] - ptrs[0]) / Vertex.Size;
						}
						else
							vtxcount = ((int)vertoff - ptrs[0]) / Vertex.Size;
						tmpaddr = (int)vertoff;
						for (int j = 0; j < frames; j++)
						{
							Vertex[] verts = new Vertex[vtxcount];
							int newaddr = (int)(ByteConverter.ToUInt32(file, tmpaddr + 4) - imageBase);
							for (int k = 0; k < verts.Length; k++)
							{
								verts[k] = new Vertex(file, newaddr);
								newaddr += Vertex.Size;
							}
							if (!data.Vertex.ContainsKey(ByteConverter.ToInt32(file, tmpaddr))) data.Vertex.Add(ByteConverter.ToInt32(file, tmpaddr), verts);
							tmpaddr += 8;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Normal))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (normoff != 0 && frames > 0)
					{
						hasdata = true;
						if (vtxcount < 0)
						{
							tmpaddr = (int)normoff;
							List<int> ptrs = new List<int>();
							for (int j = 0; j < frames; j++)
							{
								ptrs.AddUnique((int)(ByteConverter.ToUInt32(file, tmpaddr + 4) - imageBase));
								tmpaddr += 8;
							}
							if (ptrs.Count > 1)
							{
								ptrs.Sort();
								vtxcount = (ptrs[1] - ptrs[0]) / Vertex.Size;
							}
							else
								vtxcount = ((int)normoff - ptrs[0]) / Vertex.Size;
						}
						tmpaddr = (int)normoff;
						for (int j = 0; j < frames; j++)
						{
							Vertex[] verts = new Vertex[vtxcount];
							int newaddr = (int)(ByteConverter.ToUInt32(file, tmpaddr + 4) - imageBase);
							for (int k = 0; k < verts.Length; k++)
							{
								verts[k] = new Vertex(file, newaddr);
								newaddr += Vertex.Size;
							}
							if (!data.Normal.ContainsKey(ByteConverter.ToInt32(file, tmpaddr))) data.Normal.Add(ByteConverter.ToInt32(file, tmpaddr), verts);
							tmpaddr += 8;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Target))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (targoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)targoff;
						for (int j = 0; j < frames; j++)
						{
							data.Target.Add(ByteConverter.ToInt32(file, tmpaddr), new Vertex(file, tmpaddr + 4));
							tmpaddr += 16;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Roll))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (rolloff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)rolloff;
						for (int j = 0; j < frames; j++)
						{
							data.Roll.Add(ByteConverter.ToInt32(file, tmpaddr), ByteConverter.ToInt32(file, tmpaddr + 4));
							tmpaddr += 8;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Angle))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (angoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)angoff;
						for (int j = 0; j < frames; j++)
						{
							data.Angle.Add(ByteConverter.ToInt32(file, tmpaddr), ByteConverter.ToInt32(file, tmpaddr + 4));
							tmpaddr += 8;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Color))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (coloff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)coloff;
						for (int j = 0; j < frames; j++)
						{
							data.Color.Add(ByteConverter.ToInt32(file, tmpaddr), ByteConverter.ToUInt32(file, tmpaddr + 4));
							tmpaddr += 8;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Intensity))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (intoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)intoff;
						for (int j = 0; j < frames; j++)
						{
							data.Intensity.Add(ByteConverter.ToInt32(file, tmpaddr), ByteConverter.ToSingle(file, tmpaddr + 4));
							tmpaddr += 8;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Spot))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (spotoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)spotoff;
						for (int j = 0; j < frames; j++)
						{
							data.Spot.Add(ByteConverter.ToInt32(file, tmpaddr), new Spotlight(file, tmpaddr + 4));
							tmpaddr += 20;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Point))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (pntoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)pntoff;
						for (int j = 0; j < frames; j++)
						{
							data.Point.Add(ByteConverter.ToInt32(file, tmpaddr), new float[] { ByteConverter.ToSingle(file, tmpaddr + 4), ByteConverter.ToSingle(file, tmpaddr + 8) });
							tmpaddr += 12;
						}
					}
					address += 4;
				}
				if (hasdata)
					Models.Add(i, data);
			}
			ModelParts = nummodels;
		}

		public static NJS_MOTION ReadHeader(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, Attach> attaches)
		{
			return ReadHeader(file, address, imageBase, format, new Dictionary<int, string>(), attaches);
		}

		public static NJS_MOTION ReadHeader(byte[] file, int address, uint imageBase, ModelFormat format,
			Dictionary<int, string> labels, Dictionary<int, Attach> attaches)
		{
			NJS_OBJECT Model = new NJS_OBJECT(file, (int)(ByteConverter.ToUInt32(file, address) - imageBase), imageBase, format, attaches);
			return new NJS_MOTION(file, (int)(ByteConverter.ToUInt32(file, address + 4) - imageBase), imageBase,
				Model.CountAnimated(), labels);
		}

		public static NJS_MOTION ReadDirect(byte[] file, int count, int motionaddress, uint imageBase, ModelFormat format, Dictionary<int, Attach> attaches)
		{
			return ReadDirect(file, count, motionaddress, imageBase, format, new Dictionary<int, string>(), attaches);
		}

		public static NJS_MOTION ReadDirect(byte[] file, int count, int motionaddress, uint imageBase, ModelFormat format,
			Dictionary<int, string> labels, Dictionary<int, Attach> attaches)
		{
			return new NJS_MOTION(file, motionaddress, imageBase,
				count, labels);
		}

		public static NJS_MOTION Load(string filename, int nummodels = -1)
		{
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			byte[] file = File.ReadAllBytes(filename);
			ulong magic = ByteConverter.ToUInt64(file, 0) & FormatMask;
			if (magic == SAANIM)
			{
				byte version = file[7];
				if (version > CurrentVersion)
				{
					ByteConverter.BigEndian = be;
					throw new FormatException("Not a valid SAANIM file.");
				}
				int aniaddr = ByteConverter.ToInt32(file, 8);
				Dictionary<int, string> labels = new Dictionary<int, string>();
				int tmpaddr = BitConverter.ToInt32(file, 0xC);
				if (tmpaddr != 0)
					labels.Add(aniaddr, file.GetCString(tmpaddr));
				if (version > 0)
					nummodels = BitConverter.ToInt32(file, 0x10);
				else if (nummodels == -1)
				{
					ByteConverter.BigEndian = be;
					throw new NotImplementedException("Cannot open version 0 animations without a model!");
				}
				NJS_MOTION anim = new NJS_MOTION(file, aniaddr, 0, nummodels & int.MaxValue, labels, nummodels < 0);
				ByteConverter.BigEndian = be;
				return anim;
			}
			ByteConverter.BigEndian = be;
			throw new FormatException("Not a valid SAANIM file.");
		}

		public static bool CheckAnimationFile(string filename)
		{
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			byte[] file = File.ReadAllBytes(filename);
			ulong magic = ByteConverter.ToUInt64(file, 0) & FormatMask;
			ByteConverter.BigEndian = be;
			if (magic == SAANIM)
				return file[7] <= CurrentVersion;
			return false;
		}

		public byte[] GetBytes(uint imageBase, Dictionary<string, uint> labels, out uint address)
		{
			List<byte> result = new List<byte>();
			uint[] posoffs = new uint[ModelParts];
			int[] posframes = new int[ModelParts];
			bool hasPos = false;
			uint[] rotoffs = new uint[ModelParts];
			int[] rotframes = new int[ModelParts];
			bool hasRot = false;
			uint[] scloffs = new uint[ModelParts];
			int[] sclframes = new int[ModelParts];
			bool hasScl = false;
			uint[] vecoffs = new uint[ModelParts];
			int[] vecframes = new int[ModelParts];
			bool hasVec = false;
			uint[] vertoffs = new uint[ModelParts];
			int[] vertframes = new int[ModelParts];
			bool hasVert = false;
			uint[] normoffs = new uint[ModelParts];
			int[] normframes = new int[ModelParts];
			bool hasNorm = false;
			uint[] targoffs = new uint[ModelParts];
			int[] targframes = new int[ModelParts];
			bool hasTarg = false;
			uint[] rolloffs = new uint[ModelParts];
			int[] rollframes = new int[ModelParts];
			bool hasRoll = false;
			uint[] angoffs = new uint[ModelParts];
			int[] angframes = new int[ModelParts];
			bool hasAng = false;
			uint[] coloffs = new uint[ModelParts];
			int[] colframes = new int[ModelParts];
			bool hasCol = false;
			uint[] intoffs = new uint[ModelParts];
			int[] intframes = new int[ModelParts];
			bool hasInt = false;
			uint[] spotoffs = new uint[ModelParts];
			int[] spotframes = new int[ModelParts];
			bool hasSpot = false;
			uint[] pntoffs = new uint[ModelParts];
			int[] pntframes = new int[ModelParts];
			bool hasPnt = false;
			foreach (KeyValuePair<int, AnimModelData> model in Models)
			{
				if (model.Value.Position.Count > 0)
				{
					hasPos = true;
					result.Align(4);
					posoffs[model.Key] = imageBase + (uint)result.Count;
					posframes[model.Key] = model.Value.Position.Count;
					foreach (KeyValuePair<int, Vertex> item in model.Value.Position)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}
				}
				if (model.Value.Rotation.Count > 0)
				{
					hasRot = true;
					result.Align(4);
					rotoffs[model.Key] = imageBase + (uint)result.Count;
					rotframes[model.Key] = model.Value.Rotation.Count;
					foreach (KeyValuePair<int, Rotation> item in model.Value.Rotation)
					{
						if (ShortRot)
						{
							result.AddRange(ByteConverter.GetBytes((short)item.Key));
							result.AddRange(ByteConverter.GetBytes((short)item.Value.X));
							result.AddRange(ByteConverter.GetBytes((short)item.Value.Y));
							result.AddRange(ByteConverter.GetBytes((short)item.Value.Z));
						}
						else
						{
							result.AddRange(ByteConverter.GetBytes(item.Key));
							result.AddRange(item.Value.GetBytes());
						}
					}
				}
				if (model.Value.Scale.Count > 0)
				{
					hasScl = true;
					result.Align(4);
					scloffs[model.Key] = imageBase + (uint)result.Count;
					sclframes[model.Key] = model.Value.Scale.Count;
					foreach (KeyValuePair<int, Vertex> item in model.Value.Scale)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}
				}
				if (model.Value.Vector.Count > 0)
				{
					hasVec = true;
					result.Align(4);
					vecoffs[model.Key] = imageBase + (uint)result.Count;
					vecframes[model.Key] = model.Value.Vector.Count;
					foreach (KeyValuePair<int, Vertex> item in model.Value.Vector)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}
				}
				if (model.Value.Vertex.Count > 0)
				{
					hasVert = true;
					result.Align(4);
					List<uint> offs = new List<uint>();
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
					{
						result.Align(4);
						offs.Add(imageBase + (uint)result.Count);
						foreach (Vertex v in item.Value)
							result.AddRange(v.GetBytes());
					}
					vertoffs[model.Key] = imageBase + (uint)result.Count;
					vertframes[model.Key] = model.Value.Vertex.Count;
					int i = 0;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(offs[i++]));
					}
				}
				if (model.Value.Normal.Count > 0)
				{
					hasNorm = true;
					result.Align(4);
					List<uint> offs = new List<uint>();
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Normal)
					{
						result.Align(4);
						offs.Add(imageBase + (uint)result.Count);
						foreach (Vertex v in item.Value)
							result.AddRange(v.GetBytes());
					}
					normoffs[model.Key] = imageBase + (uint)result.Count;
					normframes[model.Key] = model.Value.Normal.Count;
					int i = 0;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Normal)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(offs[i++]));
					}
				}
				if (model.Value.Target.Count > 0)
				{
					hasTarg = true;
					result.Align(4);
					targoffs[model.Key] = imageBase + (uint)result.Count;
					targframes[model.Key] = model.Value.Target.Count;
					foreach (KeyValuePair<int, Vertex> item in model.Value.Target)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}
				}
				if (model.Value.Roll.Count > 0)
				{
					hasRoll = true;
					result.Align(4);
					rolloffs[model.Key] = imageBase + (uint)result.Count;
					rollframes[model.Key] = model.Value.Roll.Count;
					foreach (KeyValuePair<int, int> item in model.Value.Roll)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value));
					}
				}
				if (model.Value.Angle.Count > 0)
				{
					hasAng = true;
					result.Align(4);
					angoffs[model.Key] = imageBase + (uint)result.Count;
					angframes[model.Key] = model.Value.Angle.Count;
					foreach (KeyValuePair<int, int> item in model.Value.Angle)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value));
					}
				}
				if (model.Value.Color.Count > 0)
				{
					hasCol = true;
					result.Align(4);
					coloffs[model.Key] = imageBase + (uint)result.Count;
					colframes[model.Key] = model.Value.Color.Count;
					foreach (KeyValuePair<int, uint> item in model.Value.Color)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value));
					}
				}
				if (model.Value.Intensity.Count > 0)
				{
					hasInt = true;
					result.Align(4);
					intoffs[model.Key] = imageBase + (uint)result.Count;
					intframes[model.Key] = model.Value.Intensity.Count;
					foreach (KeyValuePair<int, float> item in model.Value.Intensity)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value));
					}
				}
				if (model.Value.Spot.Count > 0)
				{
					hasSpot = true;
					result.Align(4);
					spotoffs[model.Key] = imageBase + (uint)result.Count;
					spotframes[model.Key] = model.Value.Spot.Count;
					foreach (KeyValuePair<int, Spotlight> item in model.Value.Spot)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}
				}
				if (model.Value.Point.Count > 0)
				{
					hasPnt = true;
					result.Align(4);
					pntoffs[model.Key] = imageBase + (uint)result.Count;
					pntframes[model.Key] = model.Value.Point.Count;
					foreach (KeyValuePair<int, float[]> item in model.Value.Point)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value[0]));
						result.AddRange(ByteConverter.GetBytes(item.Value[1]));
					}
				}
			}
			result.Align(4);
			AnimFlags flags = 0;
			ushort numpairs = 0;
			if (hasPos)
			{
				flags |= AnimFlags.Position;
				numpairs++;
			}
			if (hasRot)
			{
				flags |= AnimFlags.Rotation;
				numpairs++;
			}
			if (hasScl)
			{
				flags |= AnimFlags.Scale;
				numpairs++;
			}
			if (hasVec)
			{
				flags |= AnimFlags.Vector;
				numpairs++;
			}
			if (hasVert)
			{
				flags |= AnimFlags.Vertex;
				numpairs++;
			}
			if (hasNorm)
			{
				flags |= AnimFlags.Normal;
				numpairs++;
			}
			if (hasTarg)
			{
				flags |= AnimFlags.Target;
				numpairs++;
			}
			if (hasRoll)
			{
				flags |= AnimFlags.Roll;
				numpairs++;
			}
			if (hasAng)
			{
				flags |= AnimFlags.Angle;
				numpairs++;
			}
			if (hasCol)
			{
				flags |= AnimFlags.Color;
				numpairs++;
			}
			if (hasInt)
			{
				flags |= AnimFlags.Intensity;
				numpairs++;
			}
			if (hasSpot)
			{
				flags |= AnimFlags.Spot;
				numpairs++;
			}
			if (hasPnt)
			{
				flags |= AnimFlags.Point;
				numpairs++;
			}
			switch (flags)
			{
				case AnimFlags.Position:
				case AnimFlags.Rotation:
					hasPos = true;
					hasRot = true;
					flags = AnimFlags.Position | AnimFlags.Rotation;
					numpairs = 2;
					break;
				case AnimFlags.Scale:
					hasRot = true;
					flags |= AnimFlags.Rotation;
					numpairs++;
					break;
				case AnimFlags.Vertex:
				case AnimFlags.Normal:
					hasVert = true;
					hasNorm = true;
					flags = AnimFlags.Vertex | AnimFlags.Normal;
					numpairs = 2;
					break;
			}
			uint modeldata = imageBase + (uint)result.Count;
			for (int i = 0; i < ModelParts; i++)
			{
				if (hasPos)
					result.AddRange(ByteConverter.GetBytes(posoffs[i]));
				if (hasRot)
					result.AddRange(ByteConverter.GetBytes(rotoffs[i]));
				if (hasScl)
					result.AddRange(ByteConverter.GetBytes(scloffs[i]));
				if (hasVec)
					result.AddRange(ByteConverter.GetBytes(vecoffs[i]));
				if (hasVert)
					result.AddRange(ByteConverter.GetBytes(vertoffs[i]));
				if (hasNorm)
					result.AddRange(ByteConverter.GetBytes(normoffs[i]));
				if (hasTarg)
					result.AddRange(ByteConverter.GetBytes(targoffs[i]));
				if (hasRoll)
					result.AddRange(ByteConverter.GetBytes(rolloffs[i]));
				if (hasAng)
					result.AddRange(ByteConverter.GetBytes(angoffs[i]));
				if (hasCol)
					result.AddRange(ByteConverter.GetBytes(coloffs[i]));
				if (hasInt)
					result.AddRange(ByteConverter.GetBytes(intoffs[i]));
				if (hasSpot)
					result.AddRange(ByteConverter.GetBytes(spotoffs[i]));
				if (hasPnt)
					result.AddRange(ByteConverter.GetBytes(pntoffs[i]));
				if (hasPos)
					result.AddRange(ByteConverter.GetBytes(posframes[i]));
				if (hasRot)
					result.AddRange(ByteConverter.GetBytes(rotframes[i]));
				if (hasScl)
					result.AddRange(ByteConverter.GetBytes(sclframes[i]));
				if (hasVec)
					result.AddRange(ByteConverter.GetBytes(vecframes[i]));
				if (hasVert)
					result.AddRange(ByteConverter.GetBytes(vertframes[i]));
				if (hasNorm)
					result.AddRange(ByteConverter.GetBytes(normframes[i]));
				if (hasTarg)
					result.AddRange(ByteConverter.GetBytes(targframes[i]));
				if (hasRoll)
					result.AddRange(ByteConverter.GetBytes(rollframes[i]));
				if (hasAng)
					result.AddRange(ByteConverter.GetBytes(angframes[i]));
				if (hasCol)
					result.AddRange(ByteConverter.GetBytes(colframes[i]));
				if (hasInt)
					result.AddRange(ByteConverter.GetBytes(intframes[i]));
				if (hasSpot)
					result.AddRange(ByteConverter.GetBytes(spotframes[i]));
				if (hasPnt)
					result.AddRange(ByteConverter.GetBytes(pntframes[i]));
			}
			result.Align(4);
			address = (uint)result.Count;
			result.AddRange(ByteConverter.GetBytes(modeldata));
			result.AddRange(ByteConverter.GetBytes(Frames));
			result.AddRange(ByteConverter.GetBytes((ushort)flags));
			switch (InterpolationMode)
			{
				case InterpolationMode.Linear:
					numpairs |= (ushort)StructEnums.NJD_MTYPE_FN.NJD_MTYPE_LINER;
					break;
				case InterpolationMode.Spline:
					numpairs |= (ushort)StructEnums.NJD_MTYPE_FN.NJD_MTYPE_SPLINE;
					break;
				case InterpolationMode.User:
					numpairs |= (ushort)StructEnums.NJD_MTYPE_FN.NJD_MTYPE_USER;
					break;
			}
			result.AddRange(ByteConverter.GetBytes(numpairs));
			labels.Add(Name, address + imageBase);
			return result.ToArray();
		}

		public byte[] GetBytes(uint imageBase, out uint address)
		{
			return GetBytes(imageBase, new Dictionary<string, uint>(), out address);
		}

		public byte[] GetBytes(uint imageBase)
		{
			return GetBytes(imageBase, out _);
		}

		public void ToStructVariables(TextWriter writer)
		{
			bool hasPos = false;
			bool hasRot = false;
			bool hasScl = false;
			bool hasVec = false;
			bool hasVert = false;
			bool hasNorm = false;
			bool hasTarg = false;
			bool hasRoll = false;
			bool hasAng = false;
			bool hasCol = false;
			bool hasInt = false;
			bool hasSpot = false;
			bool hasPnt = false;
			string id = Name.MakeIdentifier();
			foreach (KeyValuePair<int, AnimModelData> model in Models)
			{
				if (model.Value.Position.Count > 0)
				{
					hasPos = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_pos[] = {");
					List<string> lines = new List<string>(model.Value.Position.Count);
					foreach (KeyValuePair<int, Vertex> item in model.Value.Position)
						lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToC() + ", " + item.Value.Y.ToC() + ", " + item.Value.Z.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Rotation.Count > 0)
				{
					hasRot = true;
					if (ShortRot)
						writer.Write("NJS_MKEY_SA ");
					else
						writer.Write("NJS_MKEY_A ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_rot[] = {");
					List<string> lines = new List<string>(model.Value.Rotation.Count);
					foreach (KeyValuePair<int, Rotation> item in model.Value.Rotation)
						lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToCHex() + ", " + item.Value.Y.ToCHex() + ", " + item.Value.Z.ToCHex() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Scale.Count > 0)
				{
					hasScl = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_scl[] = {");
					List<string> lines = new List<string>(model.Value.Scale.Count);
					foreach (KeyValuePair<int, Vertex> item in model.Value.Scale)
						lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToC() + ", " + item.Value.Y.ToC() + ", " + item.Value.Z.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Vector.Count > 0)
				{
					hasVec = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_vec[] = {");
					List<string> lines = new List<string>(model.Value.Vector.Count);
					foreach (KeyValuePair<int, Vertex> item in model.Value.Vector)
						lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToC() + ", " + item.Value.Y.ToC() + ", " + item.Value.Z.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Vertex.Count > 0)
				{
					hasVert = true;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
					{
						writer.Write("NJS_VECTOR ");
						writer.Write(id);
						writer.Write("_");
						writer.Write(model.Key);
						writer.Write("_vert_");
						writer.Write(item.Key);
						writer.WriteLine("[] = {");
						List<string> l2 = new List<string>(item.Value.Length);
						foreach (Vertex v in item.Value)
							l2.Add("\t" + v.ToStruct());
						writer.WriteLine(string.Join("," + Environment.NewLine, l2.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
					writer.Write("NJS_MKEY_P ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_vert[] = {");
					List<string> lines = new List<string>(model.Value.Vertex.Count);
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
						lines.Add("\t{ " + item.Key + ", " + id + "_" + model.Key + "_vert_" + item.Key + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Normal.Count > 0)
				{
					hasNorm = true;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Normal)
					{
						writer.Write("NJS_VECTOR ");
						writer.Write(id);
						writer.Write("_");
						writer.Write(model.Key);
						writer.Write("_norm_");
						writer.Write(item.Key);
						writer.WriteLine("[] = {");
						List<string> l2 = new List<string>(item.Value.Length);
						foreach (Vertex v in item.Value)
							l2.Add("\t" + v.ToStruct());
						writer.WriteLine(string.Join("," + Environment.NewLine, l2.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
					writer.Write("NJS_MKEY_P ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_norm[] = {");
					List<string> lines = new List<string>(model.Value.Vertex.Count);
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
						lines.Add("\t{ " + item.Key + ", " + id + "_" + model.Key + "_norm_" + item.Key + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Target.Count > 0)
				{
					hasTarg = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_targ[] = {");
					List<string> lines = new List<string>(model.Value.Target.Count);
					foreach (KeyValuePair<int, Vertex> item in model.Value.Target)
						lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToC() + ", " + item.Value.Y.ToC() + ", " + item.Value.Z.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Roll.Count > 0)
				{
					hasRoll = true;
					writer.Write("NJS_MKEY_A1 ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_roll[] = {");
					List<string> lines = new List<string>(model.Value.Roll.Count);
					foreach (KeyValuePair<int, int> item in model.Value.Roll)
						lines.Add("\t{ " + item.Key + ", " + item.Value.ToCHex() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Angle.Count > 0)
				{
					hasAng = true;
					writer.Write("NJS_MKEY_A1 ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_ang[] = {");
					List<string> lines = new List<string>(model.Value.Angle.Count);
					foreach (KeyValuePair<int, int> item in model.Value.Angle)
						lines.Add("\t{ " + item.Key + ", " + item.Value.ToCHex() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Color.Count > 0)
				{
					hasCol = true;
					writer.Write("NJS_MKEY_UI32 ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_col[] = {");
					List<string> lines = new List<string>(model.Value.Color.Count);
					foreach (KeyValuePair<int, uint> item in model.Value.Color)
						lines.Add("\t{ " + item.Key + ", " + item.Value.ToCHex() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Intensity.Count > 0)
				{
					hasInt = true;
					writer.Write("NJS_MKEY_F1 ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_int[] = {");
					List<string> lines = new List<string>(model.Value.Intensity.Count);
					foreach (KeyValuePair<int, float> item in model.Value.Intensity)
						lines.Add("\t{ " + item.Key + ", " + item.Value.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Spot.Count > 0)
				{
					hasSpot = true;
					writer.Write("NJS_MKEY_SPOT ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_spot[] = {");
					List<string> lines = new List<string>(model.Value.Spot.Count);
					foreach (KeyValuePair<int, Spotlight> item in model.Value.Spot)
						lines.Add("\t{ " + item.Key + ", " + item.Value.Near.ToC() + ", " + item.Value.Far.ToC() + ", " + item.Value.InsideAngle.ToCHex() + ", " + item.Value.OutsideAngle.ToCHex() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				if (model.Value.Point.Count > 0)
				{
					hasPnt = true;
					writer.Write("NJS_MKEY_F2 ");
					writer.Write(id);
					writer.Write("_");
					writer.Write(model.Key);
					writer.WriteLine("_pnt[] = {");
					List<string> lines = new List<string>(model.Value.Point.Count);
					foreach (KeyValuePair<int, float[]> item in model.Value.Point)
						lines.Add("\t{ " + item.Key + ", " + item.Value[0].ToC() + ", " + item.Value[1].ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
			}
			AnimFlags flags = 0;
			ushort numpairs = 0;
			if (hasPos)
			{
				flags |= AnimFlags.Position;
				numpairs++;
			}
			if (hasRot)
			{
				flags |= AnimFlags.Rotation;
				numpairs++;
			}
			if (hasScl)
			{
				flags |= AnimFlags.Scale;
				numpairs++;
			}
			if (hasVec)
			{
				flags |= AnimFlags.Vector;
				numpairs++;
			}
			if (hasVert)
			{
				flags |= AnimFlags.Vertex;
				numpairs++;
			}
			if (hasNorm)
			{
				flags |= AnimFlags.Normal;
				numpairs++;
			}
			if (hasTarg)
			{
				flags |= AnimFlags.Target;
				numpairs++;
			}
			if (hasRoll)
			{
				flags |= AnimFlags.Roll;
				numpairs++;
			}
			if (hasAng)
			{
				flags |= AnimFlags.Angle;
				numpairs++;
			}
			if (hasCol)
			{
				flags |= AnimFlags.Color;
				numpairs++;
			}
			if (hasInt)
			{
				flags |= AnimFlags.Intensity;
				numpairs++;
			}
			if (hasSpot)
			{
				flags |= AnimFlags.Spot;
				numpairs++;
			}
			if (hasPnt)
			{
				flags |= AnimFlags.Point;
				numpairs++;
			}
			switch (flags)
			{
				case AnimFlags.Position:
				case AnimFlags.Rotation:
					hasPos = true;
					hasRot = true;
					flags = AnimFlags.Position | AnimFlags.Rotation;
					numpairs = 2;
					break;
				case AnimFlags.Scale:
					hasRot = true;
					flags |= AnimFlags.Rotation;
					numpairs++;
					break;
				case AnimFlags.Vertex:
				case AnimFlags.Normal:
					hasVert = true;
					hasNorm = true;
					flags = AnimFlags.Vertex | AnimFlags.Normal;
					numpairs = 2;
					break;
			}
			writer.Write("NJS_MDATA");
			writer.Write(numpairs);
			writer.Write(" ");
			writer.Write(id);
			writer.WriteLine("_mdat[] = {");
			List<string> mdats = new List<string>(ModelParts);
			for (int i = 0; i < ModelParts; i++)
			{
				List<string> elems = new List<string>(numpairs * 2);
				if (hasPos)
				{
					if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
						elems.Add(string.Format("{0}_{1}_pos", id, i));
					else
						elems.Add("NULL");
				}
				if (hasRot)
				{
					if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
						elems.Add(string.Format("{0}_{1}_rot", id, i));
					else
						elems.Add("NULL");
				}
				if (hasScl)
				{
					if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
						elems.Add(string.Format("{0}_{1}_scl", id, i));
					else
						elems.Add("NULL");
				}
				if (hasVec)
				{
					if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
						elems.Add(string.Format("{0}_{1}_vec", id, i));
					else
						elems.Add("NULL");
				}
				if (hasVert)
				{
					if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
						elems.Add(string.Format("{0}_{1}_vert", id, i));
					else
						elems.Add("NULL");
				}
				if (hasNorm)
				{
					if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
						elems.Add(string.Format("{0}_{1}_norm", id, i));
					else
						elems.Add("NULL");
				}
				if (hasTarg)
				{
					if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
						elems.Add(string.Format("{0}_{1}_targ", id, i));
					else
						elems.Add("NULL");
				}
				if (hasRoll)
				{
					if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
						elems.Add(string.Format("{0}_{1}_roll", id, i));
					else
						elems.Add("NULL");
				}
				if (hasAng)
				{
					if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
						elems.Add(string.Format("{0}_{1}_ang", id, i));
					else
						elems.Add("NULL");
				}
				if (hasCol)
				{
					if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
						elems.Add(string.Format("{0}_{1}_col", id, i));
					else
						elems.Add("NULL");
				}
				if (hasInt)
				{
					if (Models.ContainsKey(i) && Models[i].Intensity.Count > 0)
						elems.Add(string.Format("{0}_{1}_int", id, i));
					else
						elems.Add("NULL");
				}
				if (hasSpot)
				{
					if (Models.ContainsKey(i) && Models[i].Spot.Count > 0)
						elems.Add(string.Format("{0}_{1}_spot", id, i));
					else
						elems.Add("NULL");
				}
				if (hasPnt)
				{
					if (Models.ContainsKey(i) && Models[i].Point.Count > 0)
						elems.Add(string.Format("{0}_{1}_pnt", id, i));
					else
						elems.Add("NULL");
				}
				if (hasPos)
				{
					if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_pos)", id, i));
					else
						elems.Add("0");
				}
				if (hasRot)
				{
					if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_rot)", id, i));
					else
						elems.Add("0");
				}
				if (hasScl)
				{
					if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_scl)", id, i));
					else
						elems.Add("0");
				}
				if (hasVec)
				{
					if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_vec)", id, i));
					else
						elems.Add("0");
				}
				if (hasVert)
				{
					if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_vert)", id, i));
					else
						elems.Add("0");
				}
				if (hasNorm)
				{
					if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_norm)", id, i));
					else
						elems.Add("0");
				}
				if (hasTarg)
				{
					if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_targ)", id, i));
					else
						elems.Add("0");
				}
				if (hasRoll)
				{
					if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_roll)", id, i));
					else
						elems.Add("0");
				}
				if (hasAng)
				{
					if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_ang)", id, i));
					else
						elems.Add("0");
				}
				if (hasCol)
				{
					if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_col)", id, i));
					else
						elems.Add("0");
				}
				if (hasInt)
				{
					if (Models.ContainsKey(i) && Models[i].Intensity.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_int)", id, i));
					else
						elems.Add("0");
				}
				if (hasSpot)
				{
					if (Models.ContainsKey(i) && Models[i].Spot.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_spot)", id, i));
					else
						elems.Add("0");
				}
				if (hasPnt)
				{
					if (Models.ContainsKey(i) && Models[i].Point.Count > 0)
						elems.Add(string.Format("LengthOfArray<Uint32>({0}_{1}_pnt)", id, i));
					else
						elems.Add("0");
				}
				mdats.Add("\t{ " + string.Join(", ", elems.ToArray()) + " }");
			}
			writer.WriteLine(string.Join("," + Environment.NewLine, mdats.ToArray()));
			writer.WriteLine("};");
			writer.WriteLine();
			writer.Write("NJS_MOTION ");
			writer.Write(id);
			writer.Write(" = { ");
			writer.Write("{0}_mdat, ", id);
			writer.Write(Frames);
			writer.Write(", ");
			writer.Write(((StructEnums.NJD_MTYPE)flags).ToString().Replace(", ", " | "));
			writer.Write(", ");
			switch (InterpolationMode)
			{
				case InterpolationMode.Linear:
					writer.Write("{0} | ", StructEnums.NJD_MTYPE_FN.NJD_MTYPE_LINER);
					break;
				case InterpolationMode.Spline:
					writer.Write("{0} | ", StructEnums.NJD_MTYPE_FN.NJD_MTYPE_SPLINE);
					break;
				case InterpolationMode.User:
					writer.Write("{0} | ", StructEnums.NJD_MTYPE_FN.NJD_MTYPE_USER);
					break;
			}
			writer.Write(numpairs);
			writer.WriteLine(" };");
		}

		public string ToStructVariables()
		{
			using (StringWriter sw = new StringWriter())
			{
				ToStructVariables(sw);
				return sw.ToString();
			}
		}

		public byte[] WriteHeader(uint imageBase, uint modeladdr, Dictionary<string, uint> labels, out uint address)
		{
			List<byte> result = new List<byte>();
			result.AddRange(GetBytes(imageBase, labels, out uint head2));
			address = (uint)result.Count;
			result.AddRange(ByteConverter.GetBytes(modeladdr));
			result.AddRange(ByteConverter.GetBytes(head2 + imageBase));
			return result.ToArray();
		}

		public byte[] WriteHeader(uint imageBase, uint modeladdr, out uint address)
		{
			return WriteHeader(imageBase, modeladdr, new Dictionary<string, uint>(), out address);
		}

		public byte[] WriteHeader(uint imageBase, uint modeladdr)
		{
			return WriteHeader(imageBase, modeladdr, new Dictionary<string, uint>(), out _);
		}

		public void Save(string filename)
		{
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			List<byte> file = new List<byte>();
			file.AddRange(ByteConverter.GetBytes(SAANIMVer));
			byte[] anim = GetBytes(0x14, out uint addr);
			file.AddRange(ByteConverter.GetBytes(addr + 0x14));
			file.Align(0x10);
			file.AddRange(ByteConverter.GetBytes(ModelParts | (ShortRot ? int.MinValue : 0)));
			file.Align(0x14);
			file.AddRange(anim);
			file.Align(4);
			file.RemoveRange(0xC, 4);
			file.InsertRange(0xC, ByteConverter.GetBytes(file.Count + 4));
			file.AddRange(Encoding.UTF8.GetBytes(Name));
			file.Add(0);
			file.Align(4);
			File.WriteAllBytes(filename, file.ToArray());
			ByteConverter.BigEndian = be;
		}
	}

	public class AnimModelData
	{
		public Dictionary<int, Vertex> Position = new Dictionary<int, Vertex>();
		public Dictionary<int, Rotation> Rotation = new Dictionary<int, Rotation>();
		public Dictionary<int, Vertex> Scale = new Dictionary<int, Vertex>();
		public Dictionary<int, Vertex> Vector = new Dictionary<int, Vertex>();
		public Dictionary<int, Vertex[]> Vertex = new Dictionary<int, Vertex[]>();
		public Dictionary<int, Vertex[]> Normal = new Dictionary<int, Vertex[]>();
		public Dictionary<int, Vertex> Target = new Dictionary<int, Vertex>();
		public Dictionary<int, int> Roll = new Dictionary<int, int>();
		public Dictionary<int, int> Angle = new Dictionary<int, int>();
		public Dictionary<int, uint> Color = new Dictionary<int, uint>();
		public Dictionary<int, float> Intensity = new Dictionary<int, float>();
		public Dictionary<int, Spotlight> Spot = new Dictionary<int, Spotlight>();
		public Dictionary<int, float[]> Point = new Dictionary<int, float[]>();

		public AnimModelData()
		{
		}

		public Vertex GetPosition(int frame)
		{
			if (Position.ContainsKey(frame))
				return Position[frame];
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Position.Keys)
				keys.Add(k);
			for (int i = 0; i < Position.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Position.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			if (f2 == 0)
				return GetPosition(0);
			Vertex val = new Vertex()
			{
				X = (((Position[f2].X - Position[f1].X) / (f2 - f1)) * (frame - f1)) + Position[f1].X,
				Y = (((Position[f2].Y - Position[f1].Y) / (f2 - f1)) * (frame - f1)) + Position[f1].Y,
				Z = (((Position[f2].Z - Position[f1].Z) / (f2 - f1)) * (frame - f1)) + Position[f1].Z
			};
			return val;
		}

		public Rotation GetRotation(int frame)
		{
			if (Rotation.ContainsKey(frame))
				return Rotation[frame];
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Rotation.Keys)
				keys.Add(k);
			for (int i = 0; i < Rotation.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Rotation.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			if (f2 == 0)
				return GetRotation(0);
			Rotation val = new Rotation()
			{
				X = (int)Math.Round((((Rotation[f2].X - Rotation[f1].X) / (double)(f2 - f1)) * (frame - f1)) + Rotation[f1].X, MidpointRounding.AwayFromZero),
				Y = (int)Math.Round((((Rotation[f2].Y - Rotation[f1].Y) / (double)(f2 - f1)) * (frame - f1)) + Rotation[f1].Y, MidpointRounding.AwayFromZero),
				Z = (int)Math.Round((((Rotation[f2].Z - Rotation[f1].Z) / (double)(f2 - f1)) * (frame - f1)) + Rotation[f1].Z, MidpointRounding.AwayFromZero)
			};
			return val;
		}

		public Vertex GetScale(int frame)
		{
			if (Scale.ContainsKey(frame))
				return Scale[frame];
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Scale.Keys)
				keys.Add(k);
			for (int i = 0; i < Scale.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Scale.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			if (f2 == 0)
				return GetScale(0);
			Vertex val = new Vertex()
			{
				X = (((Scale[f2].X - Scale[f1].X) / (f2 - f1)) * (frame - f1)) + Scale[f1].X,
				Y = (((Scale[f2].Y - Scale[f1].Y) / (f2 - f1)) * (frame - f1)) + Scale[f1].Y,
				Z = (((Scale[f2].Z - Scale[f1].Z) / (f2 - f1)) * (frame - f1)) + Scale[f1].Z
			};
			return val;
		}

		public Vertex GetVector(int frame)
		{
			if (Vector.ContainsKey(frame))
				return Vector[frame];
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Vector.Keys)
				keys.Add(k);
			for (int i = 0; i < Vector.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Vector.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			if (f2 == 0)
				return GetVector(0);
			Vertex val = new Vertex()
			{
				X = (((Vector[f2].X - Vector[f1].X) / (f2 - f1)) * (frame - f1)) + Vector[f1].X,
				Y = (((Vector[f2].Y - Vector[f1].Y) / (f2 - f1)) * (frame - f1)) + Vector[f1].Y,
				Z = (((Vector[f2].Z - Vector[f1].Z) / (f2 - f1)) * (frame - f1)) + Vector[f1].Z
			};
			return val;
		}

		public Vertex[] GetVertex(int frame)
		{
			if (Vertex.ContainsKey(frame))
				return Vertex[frame];
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Vertex.Keys)
				keys.Add(k);
			for (int i = 0; i < Vertex.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Vertex.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			if (f2 == 0)
				return GetVertex(0);
			Vertex[] result = new Vertex[Vertex[f1].Length];
			for (int i = 0; i < Vertex[f1].Length; i++)
				result[i] = new Vertex()
				{
					X = (((Vertex[f2][i].X - Vertex[f1][i].X) / (f2 - f1)) * (frame - f1)) + Vertex[f1][i].X,
					Y = (((Vertex[f2][i].Y - Vertex[f1][i].Y) / (f2 - f1)) * (frame - f1)) + Vertex[f1][i].Y,
					Z = (((Vertex[f2][i].Z - Vertex[f1][i].Z) / (f2 - f1)) * (frame - f1)) + Vertex[f1][i].Z
				};
			return result;
		}

		public Vertex[] GetNormal(int frame)
		{
			if (Normal.ContainsKey(frame))
				return Normal[frame];
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Normal.Keys)
				keys.Add(k);
			for (int i = 0; i < Normal.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Normal.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			if (f2 == 0)
				return GetNormal(0);
			Vertex[] result = new Vertex[Normal[f1].Length];
			for (int i = 0; i < Normal[f1].Length; i++)
				result[i] = new Vertex()
				{
					X = (((Normal[f2][i].X - Normal[f1][i].X) / (f2 - f1)) * (frame - f1)) + Normal[f1][i].X,
					Y = (((Normal[f2][i].Y - Normal[f1][i].Y) / (f2 - f1)) * (frame - f1)) + Normal[f1][i].Y,
					Z = (((Normal[f2][i].Z - Normal[f1][i].Z) / (f2 - f1)) * (frame - f1)) + Normal[f1][i].Z
				};
			return result;
		}

		public Vertex GetTarget(int frame)
		{
			if (Target.ContainsKey(frame))
				return Target[frame];
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Target.Keys)
				keys.Add(k);
			for (int i = 0; i < Target.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Target.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			if (f2 == 0)
				return GetTarget(0);
			Vertex val = new Vertex()
			{
				X = (((Target[f2].X - Target[f1].X) / (f2 - f1)) * (frame - f1)) + Target[f1].X,
				Y = (((Target[f2].Y - Target[f1].Y) / (f2 - f1)) * (frame - f1)) + Target[f1].Y,
				Z = (((Target[f2].Z - Target[f1].Z) / (f2 - f1)) * (frame - f1)) + Target[f1].Z
			};
			return val;
		}

		public int GetRoll(int frame)
		{
			if (Roll.ContainsKey(frame))
				return Roll[frame];
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Roll.Keys)
				keys.Add(k);
			for (int i = 0; i < Roll.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Roll.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			if (f2 == 0)
				return GetRoll(0);
			return (int)Math.Round((((Roll[f2] - Roll[f1]) / (double)(f2 - f1)) * (frame - f1)) + Roll[f1], MidpointRounding.AwayFromZero);
		}

		public int GetAngle(int frame)
		{
			if (Angle.ContainsKey(frame))
				return Angle[frame];
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Angle.Keys)
				keys.Add(k);
			for (int i = 0; i < Angle.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Angle.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			if (f2 == 0)
				return GetAngle(0);
			return (int)Math.Round((((Angle[f2] - Angle[f1]) / (double)(f2 - f1)) * (frame - f1)) + Angle[f1], MidpointRounding.AwayFromZero);
		}
	}

	public class Spotlight
	{
		public float Near { get; set; }
		public float Far { get; set; }
		public int InsideAngle { get; set; }
		public int OutsideAngle { get; set; }

		public Spotlight() { }

		public Spotlight(byte[] file, int address)
		{
			Near = ByteConverter.ToSingle(file, address);
			Far = ByteConverter.ToSingle(file, address + 4);
			InsideAngle = ByteConverter.ToInt32(file, address + 8);
			OutsideAngle = ByteConverter.ToInt32(file, address + 12);
		}

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(16);
			result.AddRange(ByteConverter.GetBytes(Near));
			result.AddRange(ByteConverter.GetBytes(Far));
			result.AddRange(ByteConverter.GetBytes(InsideAngle));
			result.AddRange(ByteConverter.GetBytes(OutsideAngle));
			return result.ToArray();
		}
	}
}