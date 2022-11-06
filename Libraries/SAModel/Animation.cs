using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SAModel
{
	public class NJS_ACTION
	{
		public string Name { get; set; }
		public NJS_OBJECT Model { get; private set; }
		public NJS_MOTION Animation { get; private set; }

		public NJS_ACTION(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, Attach> attaches)
			: this(file, address, imageBase, format, new Dictionary<int, string>(), attaches)
		{
		}

		public NJS_ACTION(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels, Dictionary<int, Attach> attaches)
		{
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else Name = "action_" + address.ToString("X8");
			if (address > file.Length - 4)
			{
				Model = new NJS_OBJECT();
				Animation = new NJS_MOTION();
				return;
			}
			else
			{
				int objaddr = (int)(ByteConverter.ToUInt32(file, address) - imageBase);
				if (objaddr > file.Length - 4)
				{
					Model = new NJS_OBJECT();
					Animation = new NJS_MOTION();
					return;
				}
				else Model = new NJS_OBJECT(file, objaddr, imageBase, format, labels, attaches);
			}
			if (address > file.Length - 8)
			{
				Animation = new NJS_MOTION();
				return;
			}
			else
				Animation = new NJS_MOTION(file, (int)(ByteConverter.ToUInt32(file, address + 4) - imageBase), imageBase,
					Model.CountAnimated(), labels, false, Model.GetVertexCounts(), Name, Model.Name);
		}

		public NJS_ACTION(NJS_OBJECT model, NJS_MOTION animation)
		{
			Name = "action_" + animation.Name;
			Model = model;
			Animation = animation;
		}

		public byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
		{
			List<byte> result = new List<byte>();
			result.AddRange(Model.GetBytes(imageBase, DX, labels, new List<uint>(), out uint modeladdr));
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
		public const ulong CurrentVersion = 2;
		public const ulong SAANIMVer = SAANIM | (CurrentVersion << 56);

		public int Frames { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string MdataName { get; set; }
		public int ModelParts { get; set; }
		public InterpolationMode InterpolationMode { get; set; }
		public bool ShortRot { get; set; }
		public string ActionName { get; set; }
		public string ObjectName { get; set; }

		public Dictionary<int, AnimModelData> Models = new Dictionary<int, AnimModelData>();

		static bool optimizeMotions = false; // Set to false to preserve duplicate data

		public NJS_MOTION()
		{
			Name = "animation_" + Extensions.GenerateIdentifier();
			MdataName = Name + "_mdat";
		}

		public int CalculateModelParts(byte[] file, int address, uint imageBase)
		{
			int mdatap = ByteConverter.ToInt32(file, address);
			AnimFlags animtype = (AnimFlags)ByteConverter.ToUInt16(file, address + 8);
			if (animtype == 0) return 0;
			int mdata = 0;
			if (animtype.HasFlag(AnimFlags.Position)) mdata++;
			if (animtype.HasFlag(AnimFlags.Rotation)) mdata++;
			if (animtype.HasFlag(AnimFlags.Scale)) mdata++;
			if (animtype.HasFlag(AnimFlags.Vector)) mdata++;
			if (animtype.HasFlag(AnimFlags.Vertex)) mdata++;
			if (animtype.HasFlag(AnimFlags.Normal)) mdata++;
			if (animtype.HasFlag(AnimFlags.Color)) mdata++;
			if (animtype.HasFlag(AnimFlags.Intensity)) mdata++;
			if (animtype.HasFlag(AnimFlags.Target)) mdata++;
			if (animtype.HasFlag(AnimFlags.Spot)) mdata++;
			if (animtype.HasFlag(AnimFlags.Point)) mdata++;
			if (animtype.HasFlag(AnimFlags.Roll)) mdata++;
			if (animtype.HasFlag(AnimFlags.Quaternion)) mdata++;
			int mdatasize = 0;
			bool lost = false;
			switch (mdata)
			{
				case 1:
				case 2:
					mdatasize = 16;
					break;
				case 3:
					mdatasize = 24;
					break;
				case 4:
					mdatasize = 32;
					break;
				case 5:
					mdatasize = 40;
					break;
				default:
					lost = true;
					break;
			}
			if (lost) return 0;
			// Check MKEY pointers
			int mdatas = 0;
			for (int u = 0; u < 255; u++)
			{
				for (int m = 0; m < mdata; m++)
				{
					if (lost) continue;
					uint pointer = ByteConverter.ToUInt32(file, mdatap - (int)imageBase + mdatasize * u + 4 * m);
					if (pointer != 0 && (pointer < imageBase || pointer - (int)imageBase >= file.Length - 36))
						lost = true;
					if (!lost)
					{
						int framecount = ByteConverter.ToInt32(file, mdatap - (int)imageBase + mdatasize * u + 4 * mdata + 4 * m);
						if (framecount < 0 || framecount > 100 || (pointer == 0 && framecount != 0))
							lost = true;
					}
				}
				if (!lost)
					mdatas++;
			}
			return mdatas;
		}

		public bool IsShapeMotion()
		{
			foreach (var mdl in Models)
			{
				if (mdl.Value.Vertex.Count > 0 || mdl.Value.Normal.Count > 0)
					return true;
			}
			return false;
		}

		public bool OptimizeShape()
		{
			return optimizeMotions = true;
		}

		public NJS_MOTION(byte[] file, int address, uint imageBase, int nummodels, Dictionary<int, string> labels = null, bool shortrot = false, int[] numverts = null, string actionName = null, string objectName = null)
		{
			if (nummodels == 0) 
				nummodels = CalculateModelParts(file, address, imageBase);
			ActionName = actionName;
			ObjectName = objectName;
			if (labels != null && labels.ContainsKey(address))
			{
				Name = labels[address];
				if (int.TryParse(Name, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out int num) == true)
					Name = "animation_" + address.ToString("X8");
			}
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
			if (labels != null && labels.ContainsKey(address))
				MdataName = labels[address];
			else
				MdataName = Name + "_mdat_" + address.ToString("X8");
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
				uint quatoff = 0;
				if (animtype.HasFlag(AnimFlags.Quaternion))
				{
					quatoff = ByteConverter.ToUInt32(file, address);
					if (quatoff > 0)
						quatoff -= imageBase;
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.PositionName = labels[tmpaddr];
						else data.PositionName = Name + "_mkey_" + i.ToString() + "_pos_" + tmpaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.RotationName = labels[tmpaddr];
						else data.RotationName = Name + "_mkey_" + i.ToString() + "_rot_" + tmpaddr.ToString("X8");
						for (int j = 0; j < frames; j++)
						{
							if (ShortRot)
							{
								if (!data.Rotation.ContainsKey(ByteConverter.ToInt16(file, tmpaddr)))
									data.Rotation.Add(ByteConverter.ToInt16(file, tmpaddr), new Rotation(ByteConverter.ToInt16(file, tmpaddr + 2), ByteConverter.ToInt16(file, tmpaddr + 4), ByteConverter.ToInt16(file, tmpaddr + 6)));
								tmpaddr += 8;
							}
							else
							{
								if (!data.Rotation.ContainsKey(ByteConverter.ToInt32(file, tmpaddr)))
									data.Rotation.Add(ByteConverter.ToInt32(file, tmpaddr), new Rotation(file, tmpaddr + 4));
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.ScaleName = labels[tmpaddr];
						else data.ScaleName = Name + "_mkey_" + i.ToString() + "_scl_" + tmpaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.VectorName = labels[tmpaddr];
						else data.VectorName = Name + "_mkey_" + i.ToString() + "_vec_" + tmpaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.VertexName = labels[tmpaddr];
						else data.VertexName = Name + "_mkey_" + i.ToString() + "_vert_" + tmpaddr.ToString("X8");
						List<int> ptrs = new List<int>();
						data.VertexItemName = new string[frames];
						for (int j = 0; j < frames; j++)
						{
							ptrs.AddUnique((int)(ByteConverter.ToUInt32(file, tmpaddr + 4) - imageBase));
							int itemaddr = (int)(ByteConverter.ToUInt32(file, tmpaddr + 4) - imageBase);
							if (labels != null && labels.ContainsKey(itemaddr))
								data.VertexItemName[j] = labels[itemaddr];
							else data.VertexItemName[j] = Name + "_" + i.ToString() + "_vtx_" + j.ToString() + "_" + itemaddr.ToString("X8");
							tmpaddr += 8;
						}
						// Use vertex counts specified in split if available
						if (numverts != null && numverts.Length > 0)
							vtxcount = numverts[i];
						else
						{
							if (ptrs.Count > 1)
							{
								ptrs.Sort();
								vtxcount = (ptrs[1] - ptrs[0]) / Vertex.Size;
							}
							else
								vtxcount = ((int)vertoff - ptrs[0]) / Vertex.Size;
						}
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
						data.NormalItemName = new string[frames];
						// Use vertex counts specified in split if available
						if (numverts != null && numverts.Length > 0)
							vtxcount = numverts[i];
						else if (vtxcount < 0)
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.NormalName = labels[tmpaddr];
						else data.NormalName = Name + "_mkey_" + i.ToString() + "_norm_" + tmpaddr.ToString("X8");
						for (int j = 0; j < frames; j++)
						{
							Vertex[] verts = new Vertex[vtxcount];
							int newaddr = (int)(ByteConverter.ToUInt32(file, tmpaddr + 4) - imageBase);
							if (labels != null && labels.ContainsKey(newaddr))
								data.NormalItemName[j] = labels[newaddr];
							else data.NormalItemName[j] = Name + "_" + i.ToString() + "_nrm_" + j.ToString() + "_" + newaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.TargetName = labels[tmpaddr];
						else data.TargetName = Name + "_mkey_" + i.ToString() + "_target_" + tmpaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.RollName = labels[tmpaddr];
						else data.RollName = Name + "_mkey_" + i.ToString() + "_roll_" + tmpaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.AngleName = labels[tmpaddr];
						else data.AngleName = Name + "_mkey_" + i.ToString() + "_ang_" + tmpaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.ColorName = labels[tmpaddr];
						else data.ColorName = Name + "_mkey_" + i.ToString() + "_col_" + tmpaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.IntensityName = labels[tmpaddr];
						else data.IntensityName = Name + "_mkey_" + i.ToString() + "_int_" + tmpaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.SpotName = labels[tmpaddr];
						else data.SpotName = Name + "_mkey_" + i.ToString() + "_spot_" + tmpaddr.ToString("X8");
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
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.PointName = labels[tmpaddr];
						else data.PointName = Name + "_mkey_" + i.ToString() + "_point_" + tmpaddr.ToString("X8");
						for (int j = 0; j < frames; j++)
						{
							data.Point.Add(ByteConverter.ToInt32(file, tmpaddr), new float[] { ByteConverter.ToSingle(file, tmpaddr + 4), ByteConverter.ToSingle(file, tmpaddr + 8) });
							tmpaddr += 12;
						}
					}
					address += 4;
				}
				if (animtype.HasFlag(AnimFlags.Quaternion))
				{
					int frames = ByteConverter.ToInt32(file, address);
					if (quatoff != 0 && frames > 0)
					{
						hasdata = true;
						tmpaddr = (int)quatoff;
						if (labels != null && labels.ContainsKey(tmpaddr))
							data.QuaternionName = labels[tmpaddr];
						else data.QuaternionName = Name + "_mkey_" + i.ToString() + "_quat_" + tmpaddr.ToString("X8");
						for (int j = 0; j < frames; j++)
						{
							//WXYZ order
							data.Quaternion.Add(ByteConverter.ToInt32(file, tmpaddr), new float[] { ByteConverter.ToSingle(file, tmpaddr + 4), ByteConverter.ToSingle(file, tmpaddr + 8), ByteConverter.ToSingle(file, tmpaddr + 12), ByteConverter.ToSingle(file, tmpaddr + 16) });
							tmpaddr += 20;
						}
					}
					address += 4;
				}
				if (hasdata)
				{
					data.NbKeyframes = Frames;
					Models.Add(i, data);
				}
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

		public static NJS_MOTION ReadDirect(byte[] file, int count, int motionaddress, uint imageBase, Dictionary<int, Attach> attaches, bool shortrot = false)
		{
			return ReadDirect(file, count, motionaddress, imageBase, new Dictionary<int, string>(), attaches, shortrot);
		}

		public static NJS_MOTION ReadDirect(byte[] file, int count, int motionaddress, uint imageBase, Dictionary<int, string> labels, Dictionary<int, Attach> attaches, bool shortrot = false)
		{
			return new NJS_MOTION(file, motionaddress, imageBase,
				count, labels, shortrot);
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
				string description = null;
				string actionName = null;
				string objectName = null;
				int aniaddr = ByteConverter.ToInt32(file, 8);
				Dictionary<int, string> labels = new Dictionary<int, string>();
				int tmpaddr = BitConverter.ToInt32(file, 0xC);
				if (version >= 2)
				{
					if (tmpaddr != 0)
					{
						bool finished = false;
						while (!finished)
						{
							ChunkTypes type = (ChunkTypes)ByteConverter.ToUInt32(file, tmpaddr);
							int chunksz = ByteConverter.ToInt32(file, tmpaddr + 4);
							int nextchunk = tmpaddr + 8 + chunksz;
							tmpaddr += 8;
							byte[] chunk = new byte[chunksz];
							Array.Copy(file, tmpaddr, chunk, 0, chunksz);
							int chunkaddr = 0;
							switch (type)
							{
								case ChunkTypes.Label:
									while (ByteConverter.ToInt64(chunk, chunkaddr) != -1)
									{
										labels.Add(ByteConverter.ToInt32(chunk, chunkaddr),
											chunk.GetCString(ByteConverter.ToInt32(chunk, chunkaddr + 4)));
										chunkaddr += 8;
									}
									break;
								case ChunkTypes.Description:
									description = file.GetCString(tmpaddr);
									break;
								case ChunkTypes.ActionName:
									actionName = file.GetCString(tmpaddr);
									break;
								case ChunkTypes.ObjectName:
									objectName = file.GetCString(tmpaddr);
									break;
								case ChunkTypes.End:
									finished = true;
									break;
							}
							tmpaddr = nextchunk;
						}
					}
				}
				else
				{
					if (tmpaddr != 0)
						labels.Add(aniaddr, file.GetCString(tmpaddr));
				}
				if (version > 0)
					nummodels = BitConverter.ToInt32(file, 0x10);
				else if (nummodels == -1)
				{
					ByteConverter.BigEndian = be;
					throw new NotImplementedException("Cannot open version 0 animations without a model!");
				}
				NJS_MOTION anim = new NJS_MOTION(file, aniaddr, 0, nummodels & int.MaxValue, labels, nummodels < 0) { Description = description, ActionName = actionName, ObjectName = objectName };
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

		public byte[] GetBytes(uint imageBase, Dictionary<string, uint> labels, out uint address, bool useNMDM = false)
		{
			List<byte> result = new List<byte>();
			List<byte> pof0 = new List<byte>();
			List<int> pof0Real = new List<int>();
			List<byte> parameterData = new List<byte>();

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
			uint[] quatoffs = new uint[ModelParts];
			int[] quatframes = new int[ModelParts];
			bool hasQuat = false;

			pof0.Add(0x40); //NJ Motions all start with 0x40, ie address 0 after unmasking
			pof0Real.Add(0);

			foreach (KeyValuePair<int, AnimModelData> model in Models)
			{
				if (model.Value.Position.Count > 0)
				{
					hasPos = true;
					result.Align(4);
					posoffs[model.Key] = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(posoffs[model.Key]) && model.Value.PositionName != null)
					{
						if (!labels.ContainsKey(model.Value.PositionName))
							labels.Add(model.Value.PositionName, posoffs[model.Key]);
						else
						{
							string newname = model.Value.PositionName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, posoffs[model.Key]);
						}
					}
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

					if (!labels.ContainsValue(rotoffs[model.Key]) && model.Value.RotationName != null)
					{
						if (!labels.ContainsKey(model.Value.RotationName))
							labels.Add(model.Value.RotationName, rotoffs[model.Key]);
						else
						{
							string newname = model.Value.RotationName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, rotoffs[model.Key]);
						}
					}
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

					if (!labels.ContainsValue(scloffs[model.Key]) && model.Value.ScaleName != null)
					{
						if (!labels.ContainsKey(model.Value.ScaleName))
							labels.Add(model.Value.ScaleName, scloffs[model.Key]);
						else
						{
							string newname = model.Value.ScaleName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, scloffs[model.Key]);
						}
					}
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

					if (!labels.ContainsValue(vecoffs[model.Key]) && model.Value.VectorName != null)
					{
						if (!labels.ContainsKey(model.Value.VectorName))
							labels.Add(model.Value.VectorName, vecoffs[model.Key]);
						else
						{
							string newname = model.Value.VectorName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, vecoffs[model.Key]);
						}
					}
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
					List<(Vertex[] vlist, uint off)> voffs = new List<(Vertex[] vlist, uint off)>();
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
					{
						bool found = false;
						foreach (var (vlist, off) in voffs)
							if (item.Value.SequenceEqual(vlist))
							{
								offs.Add(off);
								found = true;
								break;
							}
						if (optimizeMotions && found) continue;
						result.Align(4);
						offs.Add(imageBase + (uint)result.Count);
						if (optimizeMotions) voffs.Add((item.Value, imageBase + (uint)result.Count));
						foreach (Vertex v in item.Value)
							result.AddRange(v.GetBytes());
					}
					vertoffs[model.Key] = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(vertoffs[model.Key]) && model.Value.VertexName != null)
					{
						if (!labels.ContainsKey(model.Value.VertexName))
							labels.Add(model.Value.VertexName, vertoffs[model.Key]);
						else
						{
							string newname = model.Value.VertexName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, vertoffs[model.Key]);
						}
					}
					vertframes[model.Key] = model.Value.Vertex.Count;
					int i = 0;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(offs[i++]));
					}
					for (int u = 0; u < model.Value.Vertex.Count; u++)
					{
						if (!labels.ContainsValue(offs[u]) && model.Value.VertexItemName[u] != null)
						{
							if (!labels.ContainsKey(model.Value.VertexItemName[u]))
								labels.Add(model.Value.VertexItemName[u], offs[u]);
							else
							{
								string newname = model.Value.VertexItemName[u];
								do
								{
									newname += "_dup";
								} while (labels.ContainsKey(newname));
								labels.Add(newname, offs[u]);
							}
						}
					}
				}
				if (model.Value.Normal.Count > 0)
				{
					hasNorm = true;
					result.Align(4);
					List<uint> offs = new List<uint>();
					List<(Vertex[] vlist, uint off)> voffs = new List<(Vertex[] vlist, uint off)>();
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Normal)
					{
						bool found = false;
						foreach (var (vlist, off) in voffs)
							if (item.Value.SequenceEqual(vlist))
							{
								offs.Add(off);
								found = true;
								break;
							}
						if (optimizeMotions && found) continue;
						result.Align(4);
						offs.Add(imageBase + (uint)result.Count);
						if (optimizeMotions) voffs.Add((item.Value, imageBase + (uint)result.Count));
						foreach (Vertex v in item.Value)
							result.AddRange(v.GetBytes());
					}
					normoffs[model.Key] = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(normoffs[model.Key]) && model.Value.NormalName != null)
					{
						if (!labels.ContainsKey(model.Value.NormalName))
							labels.Add(model.Value.NormalName, normoffs[model.Key]);
						else
						{
							string newname = model.Value.NormalName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, normoffs[model.Key]);
						}
					}
					normframes[model.Key] = model.Value.Normal.Count;
					int i = 0;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Normal)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(offs[i++]));
					}
					for (int u = 0; u < model.Value.Normal.Count; u++)
					{
						if (!labels.ContainsValue(offs[u]) && model.Value.NormalItemName[u] != null)
						{
							if (!labels.ContainsKey(model.Value.NormalItemName[u]))
								labels.Add(model.Value.NormalItemName[u], offs[u]);
							else
							{
								string newname = model.Value.NormalItemName[u];
								do
								{
									newname += "_dup";
								} while (labels.ContainsKey(newname));
								labels.Add(newname, offs[u]);
							}
						}
					}
				}
				if (model.Value.Target.Count > 0)
				{
					hasTarg = true;
					result.Align(4);
					targoffs[model.Key] = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(targoffs[model.Key]) && model.Value.TargetName != null)
					{
						if (!labels.ContainsKey(model.Value.TargetName))
							labels.Add(model.Value.TargetName, targoffs[model.Key]);
						else
						{
							string newname = model.Value.TargetName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, targoffs[model.Key]);
						}
					}
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

					if (!labels.ContainsValue(rolloffs[model.Key]) && model.Value.RollName != null)
					{
						if (!labels.ContainsKey(model.Value.RollName))
							labels.Add(model.Value.RollName, rolloffs[model.Key]);
						else
						{
							string newname = model.Value.RollName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, rolloffs[model.Key]);
						}
					}
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

					if (!labels.ContainsValue(angoffs[model.Key]) && model.Value.AngleName != null)
					{
						if (!labels.ContainsKey(model.Value.AngleName))
							labels.Add(model.Value.AngleName, angoffs[model.Key]);
						else
						{
							string newname = model.Value.AngleName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, angoffs[model.Key]);
						}
					}
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

					if (!labels.ContainsValue(coloffs[model.Key]) && model.Value.ColorName != null)
					{
						if (!labels.ContainsKey(model.Value.ColorName))
							labels.Add(model.Value.ColorName, coloffs[model.Key]);
						else
						{
							string newname = model.Value.ColorName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, coloffs[model.Key]);
						}
					}
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

					if (!labels.ContainsValue(intoffs[model.Key]) && model.Value.IntensityName != null)
					{
						if (!labels.ContainsKey(model.Value.IntensityName))
							labels.Add(model.Value.IntensityName, intoffs[model.Key]);
						else
						{
							string newname = model.Value.IntensityName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, intoffs[model.Key]);
						}
					}
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

					if (!labels.ContainsValue(spotoffs[model.Key]) && model.Value.SpotName != null)
					{
						if (!labels.ContainsKey(model.Value.SpotName))
							labels.Add(model.Value.SpotName, spotoffs[model.Key]);
						else
						{
							string newname = model.Value.SpotName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, spotoffs[model.Key]);
						}
					}
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

					if (!labels.ContainsValue(pntoffs[model.Key]) && model.Value.PointName != null)
					{
						if (!labels.ContainsKey(model.Value.PointName))
							labels.Add(model.Value.PointName, pntoffs[model.Key]);
						else
						{
							string newname = model.Value.PointName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, pntoffs[model.Key]);
						}
					}
					pntframes[model.Key] = model.Value.Point.Count;
					foreach (KeyValuePair<int, float[]> item in model.Value.Point)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value[0]));
						result.AddRange(ByteConverter.GetBytes(item.Value[1]));
					}
				}
				if (model.Value.Quaternion.Count > 0)
				{
					hasQuat = true;
					result.Align(4);
					quatoffs[model.Key] = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(quatoffs[model.Key]) && model.Value.QuaternionName != null)
					{
						if (!labels.ContainsKey(model.Value.QuaternionName))
							labels.Add(model.Value.QuaternionName, quatoffs[model.Key]);
						else
						{
							string newname = model.Value.QuaternionName;
							do
							{
								newname += "_dup";
							} while (labels.ContainsKey(newname));
							labels.Add(newname, quatoffs[model.Key]);
						}
					}
					quatframes[model.Key] = model.Value.Quaternion.Count;
					foreach (KeyValuePair<int, float[]> item in model.Value.Quaternion)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value[0]));
						result.AddRange(ByteConverter.GetBytes(item.Value[1]));
						result.AddRange(ByteConverter.GetBytes(item.Value[2]));
						result.AddRange(ByteConverter.GetBytes(item.Value[3]));
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
			if (hasQuat)
			{
				flags |= AnimFlags.Quaternion;
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
			//Dealing with uninitialized data. 
			//This is to avoid MDATA and MOTIONS sharing the same address, which interferes with labels.
			if (result.Count == 0 && numpairs == 0 && Models.Count == 0 && flags == 0)
			{
				hasPos = true;
				result.Align(4);
				posoffs[0] = imageBase + (uint)result.Count;
				posframes[0] = 1;
				result.AddRange(ByteConverter.GetBytes(0));
				Vertex temp = new Vertex(0.0f, 0.0f, 0.0f);
				result.AddRange(temp.GetBytes());
			}
			if (!labels.ContainsValue(modeldata) && MdataName != null)
			{
				if (!labels.ContainsKey(MdataName)) labels.Add(MdataName, modeldata);
				else
				{
					string newname = MdataName;
					do
					{
						newname += "_dup";
					} while (labels.ContainsKey(newname));
					labels.Add(newname, modeldata);
				}
			}
			for (int i = 0; i < ModelParts; i++)
			{
				//Offsets
				if (hasPos)
					AddOffsets(result, imageBase, pof0Real, pof0, posoffs[i]);
				if (hasRot)
					AddOffsets(result, imageBase, pof0Real, pof0, rotoffs[i]);
				if (hasScl)
					AddOffsets(result, imageBase, pof0Real, pof0, scloffs[i]);
				if (hasVec)
					AddOffsets(result, imageBase, pof0Real, pof0, vecoffs[i]);
				if (hasVert)
					AddOffsets(result, imageBase, pof0Real, pof0, vertoffs[i]);
				if (hasNorm)
					AddOffsets(result, imageBase, pof0Real, pof0, normoffs[i]);
				if (hasTarg)
					AddOffsets(result, imageBase, pof0Real, pof0, targoffs[i]);
				if (hasRoll)
					AddOffsets(result, imageBase, pof0Real, pof0, rolloffs[i]);
				if (hasAng)
					AddOffsets(result, imageBase, pof0Real, pof0, angoffs[i]);
				if (hasCol)
					AddOffsets(result, imageBase, pof0Real, pof0, coloffs[i]);
				if (hasInt)
					AddOffsets(result, imageBase, pof0Real, pof0, intoffs[i]);
				if (hasSpot)
					AddOffsets(result, imageBase, pof0Real, pof0, spotoffs[i]);
				if (hasPnt)
					AddOffsets(result, imageBase, pof0Real, pof0, pntoffs[i]);
				if (hasQuat)
					AddOffsets(result, imageBase, pof0Real, pof0, quatoffs[i]);

				//Frame count
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
				if (hasQuat)
					result.AddRange(ByteConverter.GetBytes(quatframes[i]));
			}
			result.Align(4);
			address = (uint)result.Count;

			parameterData.AddRange(ByteConverter.GetBytes(modeldata));
			parameterData.AddRange(ByteConverter.GetBytes(Frames));
			parameterData.AddRange(ByteConverter.GetBytes((ushort)flags));
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
			parameterData.AddRange(ByteConverter.GetBytes(numpairs));
			if (!labels.ContainsValue(address + imageBase) && Name != null)
			{
				if (!labels.ContainsKey(Name)) labels.Add(Name, address + imageBase);
				else
				{
					string newname = Name;
					do
					{
						newname += "_dup";
					} while (labels.ContainsKey(newname));
					labels.Add(newname, address + imageBase);
				}
			}
			POF0Helper.finalizePOF0(pof0);

			if (useNMDM)
			{
				result.InsertRange(0, parameterData.ToArray());
				result.InsertRange(0, BitConverter.GetBytes(result.Count())); //This int is always little endian!
				result.InsertRange(0, new byte[] { 0x4E, 0x4D, 0x44, 0x4D }); //NMDM Magic
				result.AddRange(pof0);
			}
			else
			{
				result.AddRange(parameterData.ToArray());
			}

			return result.ToArray();
		}

		private void AddOffsets(List<byte> result, uint imageBase, List<int> pof0Real, List<byte> pof0, uint offset)
		{
			int pointerOffset = (int)(result.Count + imageBase);
			result.AddRange(ByteConverter.GetBytes(offset));
			if (offset != 0)
			{
				pof0.AddRange(POF0Helper.calcPOF0Pointer(pof0Real.Last(), pointerOffset));
				pof0Real.Add(pointerOffset);
			}
		}

		public byte[] GetBytes(uint imageBase, out uint address)
		{
			return GetBytes(imageBase, new Dictionary<string, uint>(), out address);
		}

		public byte[] GetBytes(uint imageBase)
		{
			return GetBytes(imageBase, out _);
		}

		public void ToStructVariables(TextWriter writer, List<string> labels = null)
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
			bool hasQuat = false;
			string id = Name.MakeIdentifier();
			if (labels == null) 
				labels = new List<string>();
			foreach (KeyValuePair<int, AnimModelData> model in Models)
			{
				if (model.Value.Position.Count > 0 && !labels.Contains(model.Value.PositionName))
				{
					hasPos = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(model.Value.PositionName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Position.Count);
					foreach (KeyValuePair<int, Vertex> item in model.Value.Position)
						lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToC() + ", " + item.Value.Y.ToC() + ", " + item.Value.Z.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.PositionName);
				}
				if (model.Value.Rotation.Count > 0 && !labels.Contains(model.Value.RotationName))
				{
					hasRot = true;
					if (ShortRot)
						writer.Write("NJS_MKEY_SA ");
					else
						writer.Write("NJS_MKEY_A ");
					writer.Write(model.Value.RotationName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Rotation.Count);
					foreach (KeyValuePair<int, Rotation> item in model.Value.Rotation)
					{
						if (ShortRot)
							lines.Add("\t{ " + item.Key + ", " + ((short)item.Value.X).ToCHex() + ", " + ((short)item.Value.Y).ToCHex() + ", " + ((short)item.Value.Z).ToCHex() + " }");
						else
							lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToCHex() + ", " + item.Value.Y.ToCHex() + ", " + item.Value.Z.ToCHex() + " }");
					}
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.RotationName);
				}
				if (model.Value.Scale.Count > 0 && !labels.Contains(model.Value.ScaleName))
				{
					hasScl = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(model.Value.ScaleName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Scale.Count);
					foreach (KeyValuePair<int, Vertex> item in model.Value.Scale)
						lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToC() + ", " + item.Value.Y.ToC() + ", " + item.Value.Z.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.ScaleName);
				}
				if (model.Value.Vector.Count > 0 && !labels.Contains(model.Value.VectorName))
				{
					hasVec = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(model.Value.VectorName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Vector.Count);
					foreach (KeyValuePair<int, Vertex> item in model.Value.Vector)
						lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToC() + ", " + item.Value.Y.ToC() + ", " + item.Value.Z.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.VectorName);
				}
				if (model.Value.Vertex.Count > 0 && !labels.Contains(model.Value.VertexName))
				{
					hasVert = true;
					int z = 0;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
					{
						if (!labels.Contains(model.Value.VertexItemName[z]))
						{
							writer.Write("NJS_VECTOR ");
							writer.Write(model.Value.VertexItemName[z]);
							writer.WriteLine("[] = {");
							List<string> l2 = new List<string>(item.Value.Length);
							foreach (Vertex v in item.Value)
								l2.Add("\t" + v.ToStruct());
							writer.WriteLine(string.Join("," + Environment.NewLine, l2.ToArray()));
							writer.WriteLine("};");
							writer.WriteLine();
							labels.Add(model.Value.VertexItemName[z]);
						}
						z++;					
					}
					writer.Write("NJS_MKEY_P ");
					writer.Write(model.Value.VertexName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Vertex.Count);
					int v_c = 0;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
						lines.Add("\t{ " + item.Key + ", " + model.Value.VertexItemName[v_c++] + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.VertexName);
				}
				if (model.Value.Normal.Count > 0 && !labels.Contains(model.Value.NormalName))
				{
					hasNorm = true;
					int z = 0;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Normal)
					{
						if (!labels.Contains(model.Value.NormalItemName[z]))
						{
							writer.Write("NJS_VECTOR ");
							writer.Write(model.Value.NormalItemName[z]);
							writer.WriteLine("[] = {");
							List<string> l2 = new List<string>(item.Value.Length);
							foreach (Vertex v in item.Value)
								l2.Add("\t" + v.ToStruct());
							writer.WriteLine(string.Join("," + Environment.NewLine, l2.ToArray()));
							writer.WriteLine("};");
							writer.WriteLine();
							labels.Add(model.Value.NormalItemName[z]);
						}
						z++;			
					}
					writer.Write("NJS_MKEY_P ");
					writer.Write(model.Value.NormalName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Vertex.Count);
					int v_c = 0;
					foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
						lines.Add("\t{ " + item.Key + ", " + model.Value.NormalItemName[v_c++] + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.NormalName);
				}
				if (model.Value.Target.Count > 0 && !labels.Contains(model.Value.TargetName))
				{
					hasTarg = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(model.Value.TargetName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Target.Count);
					foreach (KeyValuePair<int, Vertex> item in model.Value.Target)
						lines.Add("\t{ " + item.Key + ", " + item.Value.X.ToC() + ", " + item.Value.Y.ToC() + ", " + item.Value.Z.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.TargetName);
				}
				if (model.Value.Roll.Count > 0 && !labels.Contains(model.Value.RollName))
				{
					hasRoll = true;
					writer.Write("NJS_MKEY_A1 ");
					writer.Write(model.Value.RollName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Roll.Count);
					foreach (KeyValuePair<int, int> item in model.Value.Roll)
						lines.Add("\t{ " + item.Key + ", " + item.Value.ToCHex() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.RollName);
				}
				if (model.Value.Angle.Count > 0 && !labels.Contains(model.Value.AngleName))
				{
					hasAng = true;
					writer.Write("NJS_MKEY_A1 ");
					writer.Write(model.Value.AngleName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Angle.Count);
					foreach (KeyValuePair<int, int> item in model.Value.Angle)
						lines.Add("\t{ " + item.Key + ", " + item.Value.ToCHex() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.AngleName);
				}
				if (model.Value.Color.Count > 0 && !labels.Contains(model.Value.ColorName))
				{
					hasCol = true;
					writer.Write("NJS_MKEY_UI32 ");
					writer.Write(model.Value.ColorName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Color.Count);
					foreach (KeyValuePair<int, uint> item in model.Value.Color)
						lines.Add("\t{ " + item.Key + ", " + item.Value.ToCHex() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.ColorName);
				}
				if (model.Value.Intensity.Count > 0 && !labels.Contains(model.Value.IntensityName))
				{
					hasInt = true;
					writer.Write("NJS_MKEY_F1 ");
					writer.Write(model.Value.IntensityName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Intensity.Count);
					foreach (KeyValuePair<int, float> item in model.Value.Intensity)
						lines.Add("\t{ " + item.Key + ", " + item.Value.ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.IntensityName);
				}
				if (model.Value.Spot.Count > 0 && !labels.Contains(model.Value.SpotName))
				{
					hasSpot = true;
					writer.Write("NJS_MKEY_SPOT ");
					writer.Write(model.Value.SpotName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Spot.Count);
					foreach (KeyValuePair<int, Spotlight> item in model.Value.Spot)
						lines.Add("\t{ " + item.Key + ", " + item.Value.Near.ToC() + ", " + item.Value.Far.ToC() + ", " + item.Value.InsideAngle.ToCHex() + ", " + item.Value.OutsideAngle.ToCHex() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.SpotName);
				}
				if (model.Value.Point.Count > 0 && !labels.Contains(model.Value.PointName))
				{
					hasPnt = true;
					writer.Write("NJS_MKEY_F2 ");
					writer.Write(model.Value.PointName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Point.Count);
					foreach (KeyValuePair<int, float[]> item in model.Value.Point)
						lines.Add("\t{ " + item.Key + ", " + item.Value[0].ToC() + ", " + item.Value[1].ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.PointName);
				}
				if (model.Value.Quaternion.Count > 0 && !labels.Contains(model.Value.QuaternionName))
				{
					hasPnt = true;
					writer.Write("NJS_MKEY_QUAT ");
					writer.Write(model.Value.QuaternionName);
					writer.WriteLine("[] = {");
					List<string> lines = new List<string>(model.Value.Quaternion.Count);
					foreach (KeyValuePair<int, float[]> item in model.Value.Quaternion)
						lines.Add("\t{ " + item.Key + ", " + item.Value[0].ToC() + ", " + item.Value[1].ToC() + item.Value[2].ToC() + item.Value[3].ToC() + " }");
					writer.WriteLine(string.Join("," + Environment.NewLine, lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.QuaternionName);
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
			if (hasQuat)
			{
				flags |= AnimFlags.Quaternion;
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
			if (!labels.Contains(MdataName))
			{
				writer.Write("NJS_MDATA");
				if (numpairs == 0) writer.Write(2);
				else writer.Write(numpairs);
				writer.Write(" ");
				writer.Write(MdataName);
				writer.WriteLine("[] = {");
				List<string> mdats = new List<string>(ModelParts);
				for (int i = 0; i < ModelParts; i++)
				{
					List<string> elems = new List<string>(numpairs * 2);
					if (hasPos)
					{
						if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
							elems.Add(Models[i].PositionName);
						else
							elems.Add("NULL");
					}
					if (hasRot)
					{
						if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
							elems.Add(Models[i].RotationName);
						else
							elems.Add("NULL");
					}
					if (hasScl)
					{
						if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
							elems.Add(Models[i].ScaleName);
						else
							elems.Add("NULL");
					}
					if (hasVec)
					{
						if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
							elems.Add(Models[i].VectorName);
						else
							elems.Add("NULL");
					}
					if (hasVert)
					{
						if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
							elems.Add(Models[i].VertexName);
						else
							elems.Add("NULL");
					}
					if (hasNorm)
					{
						if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
							elems.Add(Models[i].NormalName);
						else
							elems.Add("NULL");
					}
					if (hasTarg)
					{
						if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
							elems.Add(Models[i].TargetName);
						else
							elems.Add("NULL");
					}
					if (hasRoll)
					{
						if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
							elems.Add(Models[i].PointName);
						else
							elems.Add("NULL");
					}
					if (hasAng)
					{
						if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
							elems.Add(Models[i].AngleName);
						else
							elems.Add("NULL");
					}
					if (hasCol)
					{
						if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
							elems.Add(Models[i].ColorName);
						else
							elems.Add("NULL");
					}
					if (hasInt)
					{
						if (Models.ContainsKey(i) && Models[i].Intensity.Count > 0)
							elems.Add(Models[i].IntensityName);
						else
							elems.Add("NULL");
					}
					if (hasSpot)
					{
						if (Models.ContainsKey(i) && Models[i].Spot.Count > 0)
							elems.Add(Models[i].SpotName);
						else
							elems.Add("NULL");
					}
					if (hasPnt)
					{
						if (Models.ContainsKey(i) && Models[i].Point.Count > 0)
							elems.Add(Models[i].PointName);
						else
							elems.Add("NULL");
					}
					if (hasQuat)
					{
						if (Models.ContainsKey(i) && Models[i].Quaternion.Count > 0)
							elems.Add(Models[i].QuaternionName);
						else
							elems.Add("NULL");
					}
					if (hasPos)
					{
						if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].PositionName));
						else
							elems.Add("0");
					}
					if (hasRot)
					{
						if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].RotationName));
						else
							elems.Add("0");
					}
					if (hasScl)
					{
						if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].ScaleName));
						else
							elems.Add("0");
					}
					if (hasVec)
					{
						if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].VectorName));
						else
							elems.Add("0");
					}
					if (hasVert)
					{
						if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].VertexName));
						else
							elems.Add("0");
					}
					if (hasNorm)
					{
						if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].NormalName));
						else
							elems.Add("0");
					}
					if (hasTarg)
					{
						if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].TargetName));
						else
							elems.Add("0");
					}
					if (hasRoll)
					{
						if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].RollName));
						else
							elems.Add("0");
					}
					if (hasAng)
					{
						if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].AngleName));
						else
							elems.Add("0");
					}
					if (hasCol)
					{
						if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].ColorName));
						else
							elems.Add("0");
					}
					if (hasInt)
					{
						if (Models.ContainsKey(i) && Models[i].Intensity.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].IntensityName));
						else
							elems.Add("0");
					}
					if (hasSpot)
					{
						if (Models.ContainsKey(i) && Models[i].Spot.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].SpotName));
						else
							elems.Add("0");
					}
					if (hasPnt)
					{
						if (Models.ContainsKey(i) && Models[i].Point.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].PointName));
						else
							elems.Add("0");
					}
					if (hasQuat)
					{
						if (Models.ContainsKey(i) && Models[i].Quaternion.Count > 0)
							elems.Add(string.Format("LengthOfArray<Uint32>({0})", Models[i].QuaternionName));
						else
							elems.Add("0");
					}
					mdats.Add("\t{ " + string.Join(", ", elems.ToArray()) + " }");
				}
				writer.WriteLine(string.Join("," + Environment.NewLine, mdats.ToArray()));
				writer.WriteLine("};");
				writer.WriteLine();
				labels.Add(MdataName);
			}
			if (!labels.Contains(Name))
			{
				writer.Write("NJS_MOTION ");
				writer.Write(Name);
				writer.Write(" = { ");
				writer.Write("{0}, ", MdataName);
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
				labels.Add(Name);
			}
			if (!string.IsNullOrEmpty(ActionName) && !string.IsNullOrEmpty(ObjectName) && !labels.Contains(ActionName))
			{
				writer.WriteLine();
				writer.Write("NJS_ACTION ");
				writer.Write(ActionName);
				writer.Write(" = { &");
				writer.Write(ObjectName);
				writer.Write(", &");
				writer.Write(Name);
				writer.WriteLine(" };");
				labels.Add(ActionName);
			}
		}

		public string ToStructVariables(List<string> labels = null)
		{
			using (StringWriter sw = new StringWriter())
			{
				ToStructVariables(sw, labels);
				return sw.ToString();
			}
		}

		public void ToNJA(TextWriter writer, List<string> labels = null, bool isDum = false)
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
			bool hasQuat = false;
			string id = Name.MakeIdentifier();
			if (labels == null)
				labels = new List<string>();
			writer.WriteLine(IsShapeMotion() ? "SHAPE_MOTION_START" : "MOTION_START");
			if (!isDum)
			{
				writer.WriteLine();
				foreach (KeyValuePair<int, AnimModelData> model in Models)
				{
					// Not implemented: Target, Roll, Angle, Color, Intensity, Spot, Point
					if (model.Value.Position.Count > 0 && !labels.Contains(model.Value.PositionName))
					{
						hasPos = true;
						writer.WriteLine("POSITION {0}[]", model.Value.PositionName);
						writer.WriteLine("START");
						foreach (KeyValuePair<int, Vertex> item in model.Value.Position)
							writer.WriteLine("         MKEYF( " + item.Key + ",   " + item.Value.X.ToNJA() + ", " + item.Value.Y.ToNJA() + ", " + item.Value.Z.ToNJA() + " ),");
						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.PositionName);
					}
					if (model.Value.Rotation.Count > 0 && !labels.Contains(model.Value.RotationName))
					{
						hasRot = true;
						if (ShortRot)
							writer.Write("SROTATION ");
						else
							writer.Write("ROTATION ");
						writer.Write(model.Value.RotationName);
						writer.WriteLine("[]");
						writer.WriteLine("START");
						foreach (KeyValuePair<int, Rotation> item in model.Value.Rotation)
						{

							if (ShortRot)
								writer.WriteLine("         MKEYSA( " + item.Key + ",   " + (((short)item.Value.X) / 182.044f).ToNJA() + ", " + (((short)item.Value.Y) / 182.044f).ToNJA() + ", " + (((short)item.Value.Z) / 182.044f).ToNJA() + " ),");
							else
								writer.WriteLine("         MKEYA( " + item.Key + ",   " + (item.Value.X / 182.044f).ToNJA() + ", " + (item.Value.Y / 182.044f).ToNJA() + ", " + (item.Value.Z / 182.044f).ToNJA() + " ),");
						}
						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.RotationName);
					}
					if (model.Value.Scale.Count > 0 && !labels.Contains(model.Value.ScaleName))
					{
						hasScl = true;
						writer.WriteLine("SCALE {0}[]", model.Value.ScaleName);
						writer.WriteLine("START");
						foreach (KeyValuePair<int, Vertex> item in model.Value.Scale)
							writer.WriteLine("         MKEYF( " + item.Key + ",   " + item.Value.X.ToNJA() + ", " + item.Value.Y.ToNJA() + ", " + item.Value.Z.ToNJA() + " ),");
						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.ScaleName);
					}
					if (model.Value.Vector.Count > 0 && !labels.Contains(model.Value.VectorName))
					{
						hasVec = true;
						writer.WriteLine("VECTOR {0}[]", model.Value.VectorName);
						writer.WriteLine("START");
						foreach (KeyValuePair<int, Vertex> item in model.Value.Vector)
							writer.WriteLine("         MKEYF( " + item.Key + ",   " + item.Value.X.ToNJA() + ", " + item.Value.Y.ToNJA() + ", " + item.Value.Z.ToNJA() + " ),");
						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.VectorName);
					}
					if (model.Value.Vertex.Count > 0 && !labels.Contains(model.Value.VertexName))
					{
						hasVert = true;
						int z = 0;
						foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
						{
							if (!labels.Contains(model.Value.VertexItemName[z]))
							{
								writer.WriteLine("POINT    {0}[]", model.Value.VertexItemName[z]);
								writer.WriteLine("START");
								List<string> l2 = new List<string>(item.Value.Length);
								foreach (Vertex v in item.Value)
									writer.WriteLine("         VERT{0},", v.ToNJA());
								writer.WriteLine("END");
								writer.WriteLine();
								labels.Add(model.Value.VertexItemName[z]);
							}
							z++;
						}
						writer.WriteLine();
						writer.WriteLine("POINTER    {0}[]", model.Value.VertexName);
						writer.WriteLine("START");
						List<string> lines = new List<string>(model.Value.Vertex.Count);
						int v_c = 0;
						foreach (KeyValuePair<int, Vertex[]> item in model.Value.Vertex)
							writer.WriteLine("         MKEYP( " + item.Key + ", " + model.Value.VertexItemName[v_c++] + "),");
						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.VertexName);
					}
					if (model.Value.Normal.Count > 0 && !labels.Contains(model.Value.NormalName))
					{
						hasNorm = true;
						int z = 0;
						foreach (KeyValuePair<int, Vertex[]> item in model.Value.Normal)
						{
							if (!labels.Contains(model.Value.NormalItemName[z]))
							{
								writer.WriteLine("NORMAL    {0}[]", model.Value.NormalItemName[z]);
								writer.WriteLine("START");
								foreach (Vertex v in item.Value)
									writer.WriteLine("         NORM{0},", v.ToNJA());
								writer.WriteLine("END");
								writer.WriteLine();
								labels.Add(model.Value.NormalItemName[z]);
							}
							z++;
						}
						writer.WriteLine();
						writer.WriteLine("POINTER    {0}[]", model.Value.NormalName);
						writer.WriteLine("START");
						int v_c = 0;
						foreach (KeyValuePair<int, Vertex[]> item in model.Value.Normal)
							writer.WriteLine("         MKEYP( " + item.Key + ", " + model.Value.NormalItemName[v_c++] + "),");
						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.NormalName);
					}
					if (model.Value.Quaternion.Count > 0 && !labels.Contains(model.Value.QuaternionName))
					{
						hasPnt = true;
						writer.Write("QROTATION {0}[]", model.Value.QuaternionName);
						writer.WriteLine("START");
						foreach (KeyValuePair<int, float[]> item in model.Value.Quaternion)
							writer.WriteLine("         MKEYQ( " + item.Key + ",   " + item.Value[0].ToNJA() + ", " + item.Value[1].ToNJA() + item.Value[2].ToC() + item.Value[3].ToNJA() + " ),");
						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.QuaternionName);
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
				if (hasQuat)
				{
					flags |= AnimFlags.Quaternion;
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
				if (!labels.Contains(MdataName))
				{
					writer.Write("MDATA");
					if (numpairs == 0)
						writer.Write(2);
					else
						writer.Write(numpairs);
					writer.Write(" ");
					writer.Write(MdataName);
					writer.WriteLine("[]");
					writer.WriteLine("START");
					List<string> mdats = new List<string>(ModelParts);
					for (int i = 0; i < ModelParts; i++)
					{
						List<string> elems = new List<string>(numpairs * 2);
						if (hasPos)
						{
							if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
								elems.Add(Models[i].PositionName);
							else
								elems.Add("NULL");
						}
						if (hasRot)
						{
							if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
								elems.Add(Models[i].RotationName);
							else
								elems.Add("NULL");
						}
						if (hasScl)
						{
							if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
								elems.Add(Models[i].ScaleName);
							else
								elems.Add("NULL");
						}
						if (hasVec)
						{
							if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
								elems.Add(Models[i].VectorName);
							else
								elems.Add("NULL");
						}
						if (hasVert)
						{
							if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
								elems.Add(Models[i].VertexName);
							else
								elems.Add("NULL");
						}
						if (hasNorm)
						{
							if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
								elems.Add(Models[i].NormalName);
							else
								elems.Add("NULL");
						}
						if (hasTarg)
						{
							if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
								elems.Add(Models[i].TargetName);
							else
								elems.Add("NULL");
						}
						if (hasRoll)
						{
							if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
								elems.Add(Models[i].PointName);
							else
								elems.Add("NULL");
						}
						if (hasAng)
						{
							if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
								elems.Add(Models[i].AngleName);
							else
								elems.Add("NULL");
						}
						if (hasCol)
						{
							if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
								elems.Add(Models[i].ColorName);
							else
								elems.Add("NULL");
						}
						if (hasInt)
						{
							if (Models.ContainsKey(i) && Models[i].Intensity.Count > 0)
								elems.Add(Models[i].IntensityName);
							else
								elems.Add("NULL");
						}
						if (hasSpot)
						{
							if (Models.ContainsKey(i) && Models[i].Spot.Count > 0)
								elems.Add(Models[i].SpotName);
							else
								elems.Add("NULL");
						}
						if (hasPnt)
						{
							if (Models.ContainsKey(i) && Models[i].Point.Count > 0)
								elems.Add(Models[i].PointName);
							else
								elems.Add("NULL");
						}
						if (hasQuat)
						{
							if (Models.ContainsKey(i) && Models[i].Quaternion.Count > 0)
								elems.Add(Models[i].QuaternionName);
							else
								elems.Add("NULL");
						}
						if (hasPos)
						{
							if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
								elems.Add(Models[i].Position.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasRot)
						{
							if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
								elems.Add(Models[i].Rotation.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasScl)
						{
							if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
								elems.Add(Models[i].Scale.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasVec)
						{
							if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
								elems.Add(Models[i].Vector.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasVert)
						{
							if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
								elems.Add(Models[i].Vertex.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasNorm)
						{
							if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
								elems.Add(Models[i].Normal.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasTarg)
						{
							if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
								elems.Add(Models[i].Target.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasRoll)
						{
							if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
								elems.Add(Models[i].Roll.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasAng)
						{
							if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
								elems.Add(Models[i].Angle.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasCol)
						{
							if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
								elems.Add(Models[i].Color.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasInt)
						{
							if (Models.ContainsKey(i) && Models[i].Intensity.Count > 0)
								elems.Add(Models[i].Intensity.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasSpot)
						{
							if (Models.ContainsKey(i) && Models[i].Spot.Count > 0)
								elems.Add(Models[i].Spot.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasPnt)
						{
							if (Models.ContainsKey(i) && Models[i].Point.Count > 0)
								elems.Add(Models[i].Point.Count.ToString());
							else
								elems.Add("0");
						}
						if (hasQuat)
						{
							if (Models.ContainsKey(i) && Models[i].Quaternion.Count > 0)
								elems.Add(Models[i].Quaternion.Count.ToString());
							else
								elems.Add("0");
						}
						if (elems.Count > 0)
							mdats.Add("    " + string.Join(", ", elems.ToArray()));
						else
							mdats.Add("    NULL, NULL, NULL, 0, 0, 0");
					}
					writer.WriteLine(string.Join("," + Environment.NewLine, mdats.ToArray()) + ",");
					writer.WriteLine("END");
					writer.WriteLine();
					labels.Add(MdataName);
				}
				if (!labels.Contains(Name))
				{
					writer.Write("MOTION ");
					writer.Write(Name);
					writer.WriteLine("[]");
					writer.WriteLine("START");
					writer.WriteLine("MdataArray     {0}, ", MdataName);
					writer.WriteLine("MFrameNum      {0},", Frames);
					writer.WriteLine("MotionBit      0x{0},", ((int)flags).ToString("X"));
					int interpol = numpairs > 0 ? numpairs : 2;
					switch (InterpolationMode)
					{
						case InterpolationMode.Spline:
							interpol = (interpol | (int)StructEnums.NJD_MTYPE_FN.NJD_MTYPE_SPLINE);
							break;
						case InterpolationMode.User:
							interpol = (interpol | (int)StructEnums.NJD_MTYPE_FN.NJD_MTYPE_USER);
							break;
							// Technically does nothing because it's 0000
							//case InterpolationMode.Linear:
							//interpol = (interpol | (int)StructEnums.NJD_MTYPE_FN.NJD_MTYPE_LINER);
							//break;
					}
					writer.WriteLine("InterpolFct    0x{0},", ((int)interpol).ToString("X"));
					writer.WriteLine("END");
					writer.WriteLine();
				}
			}
			if (!string.IsNullOrEmpty(ActionName) && !string.IsNullOrEmpty(ObjectName))
			{
				writer.WriteLine();
				writer.WriteLine("ACTION {0}[]", ActionName);
				writer.WriteLine("START");
				writer.WriteLine("ObjectHead      {0},", ObjectName);
				writer.WriteLine("Motion          " + Name);
				writer.WriteLine("END");
			}
			writer.WriteLine(IsShapeMotion() ? "SHAPE_MOTION_END" : "MOTION_END");
			writer.WriteLine();
			writer.WriteLine("DEFAULT_START");
			writer.WriteLine();
			writer.WriteLine("#ifndef DEFAULT_" + (IsShapeMotion() ? "SHAPE" : "MOTION") + "_NAME");
			writer.WriteLine("#define DEFAULT_" + (IsShapeMotion() ? "SHAPE" : "MOTION") + "_NAME " + Name);
			writer.WriteLine("#endif");
			if (!string.IsNullOrEmpty(ActionName))
			{
				writer.WriteLine("#ifndef DEFAULT_ACTION_NAME");
				writer.WriteLine("#define DEFAULT_ACTION_NAME " + ActionName);
				writer.WriteLine("#endif");
			}
			writer.WriteLine();
			writer.WriteLine("DEFAULT_END");
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

		public void Save(string filename, bool nometa = false)
		{
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			List<byte> file = new List<byte>();
			file.AddRange(ByteConverter.GetBytes(SAANIMVer));
			Dictionary<string, uint> labels = new Dictionary<string, uint>();
			byte[] anim = GetBytes(0x14, labels, out uint addr);
			file.AddRange(ByteConverter.GetBytes(addr + 0x14));
			file.Align(0x10);
			file.AddRange(ByteConverter.GetBytes(ModelParts | (ShortRot ? int.MinValue : 0)));
			file.Align(0x14);
			file.AddRange(anim);
			file.Align(4);
			file.RemoveRange(0xC, 4);
			file.InsertRange(0xC, ByteConverter.GetBytes(file.Count + 4));
			if (labels.Count > 0 && !nometa)
			{
				List<byte> chunk = new List<byte>((labels.Count * 8) + 8);
				int straddr = (labels.Count * 8) + 8;
				List<byte> strbytes = new List<byte>();
				foreach (KeyValuePair<string, uint> label in labels)
				{
					chunk.AddRange(ByteConverter.GetBytes(label.Value));
					chunk.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
					strbytes.AddRange(Encoding.UTF8.GetBytes(label.Key));
					strbytes.Add(0);
					strbytes.Align(4);
				}
				chunk.AddRange(ByteConverter.GetBytes(-1L));
				chunk.AddRange(strbytes);
				file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Label));
				file.AddRange(ByteConverter.GetBytes(chunk.Count));
				file.AddRange(chunk);
				if (!string.IsNullOrEmpty(Description))
				{
					List<byte> chunkd = new List<byte>(Description.Length + 1);
					chunkd.AddRange(Encoding.UTF8.GetBytes(Description));
					chunkd.Add(0);
					chunkd.Align(4);
					file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Description));
					file.AddRange(ByteConverter.GetBytes(chunkd.Count));
					file.AddRange(chunkd);
				}
				if (!string.IsNullOrEmpty(ActionName))
				{
					List<byte> chunkd = new List<byte>(ActionName.Length + 1);
					chunkd.AddRange(Encoding.UTF8.GetBytes(ActionName));
					chunkd.Add(0);
					chunkd.Align(4);
					file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.ActionName));
					file.AddRange(ByteConverter.GetBytes(chunkd.Count));
					file.AddRange(chunkd);
				}
				if (!string.IsNullOrEmpty(ObjectName))
				{
					List<byte> chunkd = new List<byte>(ObjectName.Length + 1);
					chunkd.AddRange(Encoding.UTF8.GetBytes(ObjectName));
					chunkd.Add(0);
					chunkd.Align(4);
					file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.ObjectName));
					file.AddRange(ByteConverter.GetBytes(chunkd.Count));
					file.AddRange(chunkd);
				}
			}
			file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.End));
			file.AddRange(new byte[4]);
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
		public Dictionary<int, float[]> Quaternion = new Dictionary<int, float[]>();
		public string PositionName;
		public string RotationName;
		public string ScaleName;
		public string VectorName;
		public string VertexName;
		public string[] VertexItemName;
		public string[] NormalItemName;
		public string NormalName;
		public string TargetName;
		public string RollName;
		public string AngleName;
		public string ColorName;
		public string IntensityName;
		public string SpotName;
		public string PointName;
		public string QuaternionName;
		public int NbKeyframes;
		public AnimModelData()
		{
		}

		public Vertex GetPosition(float frame)
		{
			if (Math.Floor(frame) == frame && Position.ContainsKey((int)Math.Floor(frame)))
				return Position[(int)Math.Floor(frame)];
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
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];
			Vertex val = new Vertex()
			{
				X = ((Position[f2z].X - Position[f1].X) / diff * (frame - f1)) + Position[f1].X,
				Y = ((Position[f2z].Y - Position[f1].Y) / diff * (frame - f1)) + Position[f1].Y,
				Z = ((Position[f2z].Z - Position[f1].Z) / diff * (frame - f1)) + Position[f1].Z
			};
			return val;
		}

		public Rotation GetRotation(float frame)
		{
			if (Math.Floor(frame) == frame && Rotation.ContainsKey((int)Math.Floor(frame)))
				return Rotation[(int)Math.Floor(frame)];
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
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];
			Rotation val = new Rotation()
			{
				X = (int)Math.Round(((Rotation[f2z].X - Rotation[f1].X) / (double)diff * (frame - f1)) + Rotation[f1].X, MidpointRounding.AwayFromZero),
				Y = (int)Math.Round(((Rotation[f2z].Y - Rotation[f1].Y) / (double)diff * (frame - f1)) + Rotation[f1].Y, MidpointRounding.AwayFromZero),
				Z = (int)Math.Round(((Rotation[f2z].Z - Rotation[f1].Z) / (double)diff * (frame - f1)) + Rotation[f1].Z, MidpointRounding.AwayFromZero)
			};
			return val;
		}

		public Vertex GetScale(float frame)
		{
			if (Math.Floor(frame) == frame && Scale.ContainsKey((int)Math.Floor(frame)))
				return Scale[(int)Math.Floor(frame)];
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
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];
			Vertex val = new Vertex()
			{
				X = ((Scale[f2z].X - Scale[f1].X) / diff * (frame - f1)) + Scale[f1].X,
				Y = ((Scale[f2z].Y - Scale[f1].Y) / diff * (frame - f1)) + Scale[f1].Y,
				Z = ((Scale[f2z].Z - Scale[f1].Z) / diff * (frame - f1)) + Scale[f1].Z
			};
			return val;
		}

		public Vertex GetVector(float frame)
		{
			if (Math.Floor(frame) == frame && Vector.ContainsKey((int)Math.Floor(frame)))
				return Vector[(int)Math.Floor(frame)];
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
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];
			Vertex val = new Vertex()
			{
				X = ((Vector[f2z].X - Vector[f1].X) / diff * (frame - f1)) + Vector[f1].X,
				Y = ((Vector[f2z].Y - Vector[f1].Y) / diff * (frame - f1)) + Vector[f1].Y,
				Z = ((Vector[f2z].Z - Vector[f1].Z) / diff * (frame - f1)) + Vector[f1].Z
			};
			return val;
		}

		public Vertex[] GetVertex(float frame)
		{
			if (Math.Floor(frame) == frame && Vertex.ContainsKey((int)Math.Floor(frame)))
				return Vertex[(int)Math.Floor(frame)];
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
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];
			Vertex[] result = new Vertex[Vertex[f1].Length];
			for (int i = 0; i < Vertex[f1].Length; i++)
				result[i] = new Vertex()
				{
					X = ((Vertex[f2z][i].X - Vertex[f1][i].X) / diff * (frame - f1)) + Vertex[f1][i].X,
					Y = ((Vertex[f2z][i].Y - Vertex[f1][i].Y) / diff * (frame - f1)) + Vertex[f1][i].Y,
					Z = ((Vertex[f2z][i].Z - Vertex[f1][i].Z) / diff * (frame - f1)) + Vertex[f1][i].Z
				};
			return result;
		}

		public Vertex[] GetNormal(float frame)
		{
			if (Math.Floor(frame) == frame && Normal.ContainsKey((int)Math.Floor(frame)))
				return Normal[(int)Math.Floor(frame)];
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
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];
			Vertex[] result = new Vertex[Normal[f1].Length];
			for (int i = 0; i < Normal[f1].Length; i++)
				result[i] = new Vertex()
				{
					X = ((Normal[f2z][i].X - Normal[f1][i].X) / diff * (frame - f1)) + Normal[f1][i].X,
					Y = ((Normal[f2z][i].Y - Normal[f1][i].Y) / diff * (frame - f1)) + Normal[f1][i].Y,
					Z = ((Normal[f2z][i].Z - Normal[f1][i].Z) / diff * (frame - f1)) + Normal[f1][i].Z
				};
			return result;
		}

		public Vertex GetTarget(float frame)
		{
			if (Math.Floor(frame) == frame && Target.ContainsKey((int)Math.Floor(frame)))
				return Target[(int)Math.Floor(frame)];
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
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];
			Vertex val = new Vertex()
			{
				X = ((Target[f2z].X - Target[f1].X) / diff * (frame - f1)) + Target[f1].X,
				Y = ((Target[f2z].Y - Target[f1].Y) / diff * (frame - f1)) + Target[f1].Y,
				Z = ((Target[f2z].Z - Target[f1].Z) / diff * (frame - f1)) + Target[f1].Z
			};
			return val;
		}

		public int GetRoll(float frame)
		{
			if (Math.Floor(frame) == frame && Roll.ContainsKey((int)Math.Floor(frame)))
				return Roll[(int)Math.Floor(frame)];
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
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];
			return (int)Math.Round((((Roll[f2z] - Roll[f1]) / (double)diff) * (frame - f1)) + Roll[f1], MidpointRounding.AwayFromZero);
		}

		public int GetAngle(float frame)
		{
			if (Math.Floor(frame) == frame && Angle.ContainsKey((int)Math.Floor(frame)))
				return Angle[(int)Math.Floor(frame)];
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
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];
			return (int)Math.Round((((Angle[f2z] - Angle[f1]) / (double)diff) * (frame - f1)) + Angle[f1], MidpointRounding.AwayFromZero);
		}

		public Rotation GetQuaternion(float frame)
		{
			if (Math.Floor(frame) == frame && Quaternion.ContainsKey((int)Math.Floor(frame)))
			{
				return RotFromQuat(FloatsAsQuat(Quaternion[(int)Math.Floor(frame)]));
			}
			int f1 = 0;
			int f2 = 0;
			List<int> keys = new List<int>();
			foreach (int k in Quaternion.Keys)
				keys.Add(k);
			for (int i = 0; i < Quaternion.Count; i++)
			{
				if (keys[i] < frame)
					f1 = keys[i];
			}
			for (int i = Quaternion.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
					f2 = keys[i];
			}
			int diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			int f2z = f2 != 0 ? f2 : keys[0];

			return RotFromQuat(System.Numerics.Quaternion.Slerp(FloatsAsQuat(Quaternion[f1]), FloatsAsQuat(Quaternion[f2z]), diff * (frame - f1)));
		}

		public System.Numerics.Quaternion FloatsAsQuat(float[] ninjaQuats)
		{
			return new System.Numerics.Quaternion(ninjaQuats[1], ninjaQuats[2], ninjaQuats[3], ninjaQuats[0]);
		}

		public Rotation RotFromQuat(System.Numerics.Quaternion quat)
		{
			float X;
			float Y;
			float Z;

			// roll (x-axis rotation)
			double sinr_cosp = 2 * (quat.W * quat.X + quat.Y * quat.Z);
			double cosr_cosp = 1 - 2 * (quat.X * quat.X + quat.Y * quat.Y);
			X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

			// pitch (y-axis rotation)
			double sinp = 2 * (quat.W * quat.Y - quat.Z * quat.X);
			if (Math.Abs(sinp) >= 1)
				Y = (float)CopySign(Math.PI / 2, sinp); // use 90 degrees if out of range
			else
				Y = (float)Math.Asin(sinp);

			// yaw (z-axis rotation)
			double siny_cosp = 2 * (quat.W * quat.Z + quat.X * quat.Y);
			double cosy_cosp = 1 - 2 * (quat.Y * quat.Y + quat.Z * quat.Z);
			Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

			return new Rotation(SAModel.Rotation.RadToBAMS(X), SAModel.Rotation.RadToBAMS(Y), SAModel.Rotation.RadToBAMS(Z));
		}
		public static double CopySign(double valMain, double valSign)
		{
			double final = Math.Abs(valMain);
			if (valSign >= 0)
			{
				return final;
			}
			else
			{
				return -final;
			}
		}
	}

	public class Spotlight : IEquatable<Spotlight>
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

		public bool Equals(Spotlight other)
		{
			return Near == other.Near && Far == other.Far && InsideAngle == other.InsideAngle && OutsideAngle == other.OutsideAngle;
		}
	}

	public enum ChunkTypes : uint
	{
		Label = 0x4C42414C,
		Description = 0x43534544,
		ActionName = 0x4143544E,
		ObjectName = 0x4F424A4E,
		End = 0x444E45
	}
}