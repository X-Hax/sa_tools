using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
			{
				Name = labels[address];
			}
			else
			{
				Name = $"action_{address:X8}";
			}

			if (address > file.Length - 4)
			{
				Model = new NJS_OBJECT();
				Animation = new NJS_MOTION();
				return;
			}
			else
			{
				var objaddr = (int)(ByteConverter.ToUInt32(file, address) - imageBase);
				if (objaddr > file.Length - 4)
				{
					Model = new NJS_OBJECT();
					Animation = new NJS_MOTION();
					return;
				}
				else
				{
					Model = new NJS_OBJECT(file, objaddr, imageBase, format, labels, attaches);
				}
			}
			if (address > file.Length - 8)
			{
				Animation = new NJS_MOTION();
				return;
			}
			else
			{
				Animation = new NJS_MOTION(file, (int)(ByteConverter.ToUInt32(file, address + 4) - imageBase), imageBase,
					Model.CountAnimated(), labels, false, Model.GetVertexCounts(), Name, Model.Name);
			}
		}

		public NJS_ACTION(NJS_OBJECT model, NJS_MOTION animation)
		{
			Name = $"action_{animation.Name}";
			Model = model;
			Animation = animation;
		}

		public byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
		{
			var result = new List<byte>();
			result.AddRange(Model.GetBytes(imageBase, DX, labels, new List<uint>(), out var modeladdr));
			var tmp = (uint)result.Count;
			result.AddRange(Animation.GetBytes(imageBase + tmp, labels, out var head2));
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
			return GetBytes(imageBase, DX, out var address);
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
			Name = $"animation_{Extensions.GenerateIdentifier()}";
			MdataName = $"{Name}_mdat";
		}

		public int CalculateModelParts(byte[] file, int address, uint imageBase)
		{
			var mdatap = ByteConverter.ToInt32(file, address);
			var animtype = (AnimFlags)ByteConverter.ToUInt16(file, address + 8);
			
			if (animtype == 0)
			{
				return 0;
			}

			var mdata = BitOperations.PopCount((uint)animtype);

			var mdatasize = 0;
			var lost = false;
			
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
			
			if (lost)
			{
				return 0;
			}

			// Check MKEY pointers
			var mdatas = 0;
			
			for (var u = 0; u < 255; u++)
			{
				for (var m = 0; m < mdata; m++)
				{
					if (lost)
					{
						continue;
					}

					var pointer = ByteConverter.ToUInt32(file, mdatap - (int)imageBase + mdatasize * u + 4 * m);
					
					if (pointer != 0 && (pointer < imageBase || pointer - (int)imageBase >= file.Length - 36))
					{
						lost = true;
					}

					if (!lost)
					{
						var framecount = ByteConverter.ToInt32(file, mdatap - (int)imageBase + mdatasize * u + 4 * mdata + 4 * m);
						
						if (framecount < 0 || framecount > 100 || (pointer == 0 && framecount != 0))
						{
							lost = true;
						}
					}
				}
				
				if (!lost)
				{
					mdatas++;
				}
			}
			
			return mdatas;
		}

		public bool IsShapeMotion()
		{
			return Models.Any(mdl => mdl.Value.Vertex.Count > 0 || mdl.Value.Normal.Count > 0);
		}

		public bool OptimizeShape()
		{
			return optimizeMotions = true;
		}

		public NJS_MOTION(byte[] file, int address, uint imageBase, int numModels, Dictionary<int, string> labels = null, bool shortRot = false, int[] numVerts = null, string actionName = null, string objectName = null, bool shortCheck = true)
		{
			if (numModels == 0)
			{
				numModels = CalculateModelParts(file, address, imageBase);
			}

			ActionName = actionName;
			ObjectName = objectName;
			
			if (labels != null && labels.TryGetValue(address, out var name))
			{
				Name = name;
				
				if (int.TryParse(Name, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out _))
				{
					Name = $"animation_{address:X8}";
				}
			}
			else
			{
				Name = $"animation_{address:X8}";
			}

			if (address > file.Length - 12)
			{
				return;
			}

			Frames = ByteConverter.ToInt32(file, address + 4);
			var animType = (AnimFlags)ByteConverter.ToUInt16(file, address + 8);
			var tmp = ByteConverter.ToUInt16(file, address + 10);

			InterpolationMode = ((StructEnums.NJD_MTYPE_FN)tmp & StructEnums.NJD_MTYPE_FN.NJD_MTYPE_MASK) switch
			{
				StructEnums.NJD_MTYPE_FN.NJD_MTYPE_LINER => InterpolationMode.Linear,
				StructEnums.NJD_MTYPE_FN.NJD_MTYPE_SPLINE => InterpolationMode.Spline,
				StructEnums.NJD_MTYPE_FN.NJD_MTYPE_USER => InterpolationMode.User,
				_ => InterpolationMode
			};
			
			ShortRot = shortRot;
			address = (int)(ByteConverter.ToUInt32(file, address) - imageBase);
			
			if (labels != null && labels.TryGetValue(address, out var mDataName))
			{
				MdataName = mDataName;
			}
			else
			{
				MdataName = $"{Name}_mdat_{address:X8}";
			}

			for (var i = 0; i < numModels; i++)
			{
				var data = new AnimModelData();
				var hasData = BitOperations.PopCount((uint)animType) > 0;
				
				if (address > file.Length - 4)
				{
					continue;
				}
				
				var posOffset = CalculateOffset(animType, AnimFlags.Position, imageBase, ref file, ref address);
				var rotOffset = CalculateOffset(animType, AnimFlags.Rotation, imageBase, ref file, ref address);
				var sclOffset = CalculateOffset(animType, AnimFlags.Scale, imageBase, ref file, ref address);
				var vecOffset = CalculateOffset(animType, AnimFlags.Vector, imageBase, ref file, ref address);
				var vertOffset = CalculateOffset(animType, AnimFlags.Vertex, imageBase, ref file, ref address);
				var normOffset = CalculateOffset(animType, AnimFlags.Normal, imageBase, ref file, ref address);
				var targOffset = CalculateOffset(animType, AnimFlags.Target, imageBase, ref file, ref address);
				var rollOffset = CalculateOffset(animType, AnimFlags.Roll, imageBase, ref file, ref address);
				var angOffset = CalculateOffset(animType, AnimFlags.Angle, imageBase, ref file, ref address);
				var colOffset = CalculateOffset(animType, AnimFlags.Color, imageBase, ref file, ref address);
				var intOffset = CalculateOffset(animType, AnimFlags.Intensity, imageBase, ref file, ref address);
				var spotOffset = CalculateOffset(animType, AnimFlags.Spot, imageBase, ref file, ref address);
				var pntOffset = CalculateOffset(animType, AnimFlags.Point, imageBase, ref file, ref address);
				var quatOffset = CalculateOffset(animType, AnimFlags.Quaternion, imageBase, ref file, ref address);
				
				if (animType.HasFlag(AnimFlags.Position))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (posOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(posOffset, out var positionName))
						{
							data.PositionName = positionName;
						}
						else
						{
							data.PositionName = $"{Name}_mkey_{i}_pos_{posOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Position.Add(ByteConverter.ToInt32(file, posOffset), new Vertex(file, posOffset + 4));
							posOffset += 16;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Rotation))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (rotOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(rotOffset, out var rotationName))
						{
							data.RotationName = rotationName;
						}
						else
						{
							data.RotationName = $"{Name}_mkey_{i}_rot_{rotOffset:X8}";
						}

						if (shortCheck)
						{
							// Check if the animation uses short rotation or not
							for (var j = 0; j < frames; j++)
							{
								// If any of the rotation frames go outside the file, assume it uses shorts
								if (rotOffset + 4 + 12 > file.Length)
								{
									ShortRot = true;
									break;
								}
								
								// If any of the rotation frames isn't in the range from -65535 to 65535, assume it uses shorts
								var rot = new Rotation(file, rotOffset + 4);
								if (rot.X > 65535 || rot.X < -65535 ||
									rot.Y > 65535 || rot.Y < -65535 ||
									rot.Z > 65535 || rot.Z < -65535)
								{
									ShortRot = true;
									break;
								}
							}
						}
						
						// Read rotation values
						for (var j = 0; j < frames; j++)
						{
							if (ShortRot)
							{
								if (!data.Rotation.ContainsKey(ByteConverter.ToInt16(file, rotOffset)))
								{
									data.Rotation.Add(ByteConverter.ToInt16(file, rotOffset), new Rotation(ByteConverter.ToInt16(file, rotOffset + 2), ByteConverter.ToInt16(file, rotOffset + 4), ByteConverter.ToInt16(file, rotOffset + 6)));
								}

								rotOffset += 8;
							}
							else
							{
								if (!data.Rotation.ContainsKey(ByteConverter.ToInt32(file, rotOffset)))
								{
									data.Rotation.Add(ByteConverter.ToInt32(file, rotOffset), new Rotation(file, rotOffset + 4));
								}

								rotOffset += 16;
							}
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Scale))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (sclOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(sclOffset, out var scaleName))
						{
							data.ScaleName = scaleName;
						}
						else
						{
							data.ScaleName = $"{Name}_mkey_{i}_scl_{sclOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Scale.Add(ByteConverter.ToInt32(file, sclOffset), new Vertex(file, sclOffset + 4));
							sclOffset += 16;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Vector))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (vecOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(vecOffset, out var vectorName))
						{
							data.VectorName = vectorName;
						}
						else
						{
							data.VectorName = $"{Name}_mkey_{i}_vec_{vecOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Vector.Add(ByteConverter.ToInt32(file, vecOffset), new Vertex(file, vecOffset + 4));
							vecOffset += 16;
						}
					}
					address += 4;
				}
				
				var vertexCount = -1;
				
				if (animType.HasFlag(AnimFlags.Vertex))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (vertOffset != 0 && frames > 0)
					{
						var tempAddress = vertOffset;
						if (labels != null && labels.TryGetValue(tempAddress, out var vertexName))
						{
							data.VertexName = vertexName;
						}
						else
						{
							data.VertexName = $"{Name}_mkey_{i}_vert_{tempAddress:X8}";
						}

						var ptrs = new List<int>();
						data.VertexItemName = new string[frames];
						for (var j = 0; j < frames; j++)
						{
							ptrs.AddUnique((int)(ByteConverter.ToUInt32(file, tempAddress + 4) - imageBase));
							var itemaddr = (int)(ByteConverter.ToUInt32(file, tempAddress + 4) - imageBase);
							
							if (labels != null && labels.TryGetValue(itemaddr, out var verterxItemName))
							{
								data.VertexItemName[j] = verterxItemName;
							}
							else
							{
								data.VertexItemName[j] = $"{Name}_{i}_vtx_{j}_{itemaddr:X8}";
							}

							tempAddress += 8;
						}
						
						// Use vertex counts specified in split if available
						if (numVerts != null && numVerts.Length > 0)
						{
							vertexCount = numVerts[i];
						}
						else
						{
							if (ptrs.Count > 1)
							{
								ptrs.Sort();
								vertexCount = (ptrs[1] - ptrs[0]) / Vertex.Size;
							}
							else
							{
								vertexCount = (tempAddress - ptrs[0]) / Vertex.Size;
							}
						}
						
						tempAddress = vertOffset;
						for (var j = 0; j < frames; j++)
						{
							var verts = new Vertex[vertexCount];
							var newaddr = (int)(ByteConverter.ToUInt32(file, tempAddress + 4) - imageBase);
							for (var k = 0; k < verts.Length; k++)
							{
								verts[k] = new Vertex(file, newaddr);
								newaddr += Vertex.Size;
							}
							if (!data.Vertex.ContainsKey(ByteConverter.ToInt32(file, tempAddress)))
							{
								data.Vertex.Add(ByteConverter.ToInt32(file, tempAddress), verts);
							}

							tempAddress += 8;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Normal))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (normOffset != 0 && frames > 0)
					{
						data.NormalItemName = new string[frames];
						
						// Use vertex counts specified in split if available
						if (numVerts != null && numVerts.Length > 0)
						{
							vertexCount = numVerts[i];
						}
						else if (vertexCount < 0)
						{
							var tempAddress = normOffset;
							var ptrs = new List<int>();
							
							for (var j = 0; j < frames; j++)
							{
								ptrs.AddUnique((int)(ByteConverter.ToUInt32(file, tempAddress + 4) - imageBase));
								tempAddress += 8;
							}
							
							if (ptrs.Count > 1)
							{
								ptrs.Sort();
								vertexCount = (ptrs[1] - ptrs[0]) / Vertex.Size;
							}
							else
							{
								vertexCount = (normOffset - ptrs[0]) / Vertex.Size;
							}
						}
						
						if (labels != null && labels.TryGetValue(normOffset, out var normalName))
						{
							data.NormalName = normalName;
						}
						else
						{
							data.NormalName = $"{Name}_mkey_{i}_norm_{normOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							var verts = new Vertex[vertexCount];
							var newAddress = (int)(ByteConverter.ToUInt32(file, normOffset + 4) - imageBase);
							
							if (labels != null && labels.TryGetValue(newAddress, out var normalItemName))
							{
								data.NormalItemName[j] = normalItemName;
							}
							else
							{
								data.NormalItemName[j] = $"{Name}_{i}_nrm_{j}_{newAddress:X8}";
							}

							for (var k = 0; k < verts.Length; k++)
							{
								verts[k] = new Vertex(file, newAddress);
								newAddress += Vertex.Size;
							}
							if (!data.Normal.ContainsKey(ByteConverter.ToInt32(file, normOffset)))
							{
								data.Normal.Add(ByteConverter.ToInt32(file, normOffset), verts);
							}

							normOffset += 8;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Target))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (targOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(targOffset, out var targetName))
						{
							data.TargetName = targetName;
						}
						else
						{
							data.TargetName = $"{Name}_mkey_{i}_target_{targOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Target.Add(ByteConverter.ToInt32(file, targOffset), new Vertex(file, targOffset + 4));
							targOffset += 16;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Roll))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (rollOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(rollOffset, out var rollName))
						{
							data.RollName = rollName;
						}
						else
						{
							data.RollName = $"{Name}_mkey_{i}_roll_{rollOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Roll.Add(ByteConverter.ToInt32(file, rollOffset), ByteConverter.ToInt32(file, rollOffset + 4));
							rollOffset += 8;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Angle))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (angOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(angOffset, out var angleName))
						{
							data.AngleName = angleName;
						}
						else
						{
							data.AngleName = $"{Name}_mkey_{i}_ang_{angOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Angle.Add(ByteConverter.ToInt32(file, angOffset), ByteConverter.ToInt32(file, angOffset + 4));
							angOffset += 8;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Color))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (colOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(colOffset, out var colorName))
						{
							data.ColorName = colorName;
						}
						else
						{
							data.ColorName = $"{Name}_mkey_{i}_col_{colOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Color.Add(ByteConverter.ToInt32(file, colOffset), ByteConverter.ToUInt32(file, colOffset + 4));
							colOffset += 8;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Intensity))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (intOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(intOffset, out var intensityName))
						{
							data.IntensityName = intensityName;
						}
						else
						{
							data.IntensityName = $"{Name}_mkey_{i}_int_{intOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Intensity.Add(ByteConverter.ToInt32(file, intOffset), ByteConverter.ToSingle(file, intOffset + 4));
							intOffset += 8;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Spot))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (spotOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(spotOffset, out var spotName))
						{
							data.SpotName = spotName;
						}
						else
						{
							data.SpotName = $"{Name}_mkey_{i}_spot_{spotOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Spot.Add(ByteConverter.ToInt32(file, spotOffset), new Spotlight(file, spotOffset + 4));
							spotOffset += 20;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Point))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (pntOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(pntOffset, out var pointName))
						{
							data.PointName = pointName;
						}
						else
						{
							data.PointName = $"{Name}_mkey_{i}_point_{pntOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							data.Point.Add(ByteConverter.ToInt32(file, pntOffset), [ByteConverter.ToSingle(file, pntOffset + 4), ByteConverter.ToSingle(file, pntOffset + 8)]);
							pntOffset += 12;
						}
					}
					
					address += 4;
				}
				
				if (animType.HasFlag(AnimFlags.Quaternion))
				{
					var frames = ByteConverter.ToInt32(file, address);
					
					if (quatOffset != 0 && frames > 0)
					{
						if (labels != null && labels.TryGetValue(quatOffset, out var quaternionName))
						{
							data.QuaternionName = quaternionName;
						}
						else
						{
							data.QuaternionName = $"{Name}_mkey_{i}_quat_{quatOffset:X8}";
						}

						for (var j = 0; j < frames; j++)
						{
							// WXYZ order
							data.Quaternion.Add(ByteConverter.ToInt32(file, quatOffset), [ByteConverter.ToSingle(file, quatOffset + 4), ByteConverter.ToSingle(file, quatOffset + 8), ByteConverter.ToSingle(file, quatOffset + 12), ByteConverter.ToSingle(file, quatOffset + 16)]);
							quatOffset += 20;
						}
					}
					
					address += 4;
				}
				
				if (hasData)
				{
					data.NbKeyframes = Frames;
					Models.Add(i, data);
				}
			}
			
			ModelParts = numModels;
		}

		private static int CalculateOffset(AnimFlags animType, AnimFlags flag, uint imageBase, ref byte[] file, ref int address)
		{
			uint offset = 0;
			
			if (animType.HasFlag(flag))
			{
				offset = ByteConverter.ToUInt32(file, address);

				if (offset > 0)
				{
					offset -= imageBase;
				}
				
				address += 4;
			}
			
			return (int)offset;
		}

		public static NJS_MOTION ReadHeader(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, Attach> attaches)
		{
			return ReadHeader(file, address, imageBase, format, new Dictionary<int, string>(), attaches);
		}

		private static NJS_MOTION ReadHeader(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels, Dictionary<int, Attach> attaches)
		{
			var model = new NJS_OBJECT(file, (int)(ByteConverter.ToUInt32(file, address) - imageBase), imageBase, format, attaches);
			return new NJS_MOTION(file, (int)(ByteConverter.ToUInt32(file, address + 4) - imageBase), imageBase, model.CountAnimated(), labels);
		}

		public static NJS_MOTION ReadDirect(byte[] file, int count, int motionAddress, uint imageBase, Dictionary<int, Attach> attaches, bool shortRot = false)
		{
			return ReadDirect(file, count, motionAddress, imageBase, new Dictionary<int, string>(), attaches, shortRot);
		}

		public static NJS_MOTION ReadDirect(byte[] file, int count, int motionAddress, uint imageBase, Dictionary<int, string> labels, Dictionary<int, Attach> attaches, bool shortRot = false)
		{
			return new NJS_MOTION(file, motionAddress, imageBase, count, labels, shortRot);
		}

		public static NJS_MOTION Load(string filename, int numModels = -1)
		{
			var be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			var file = File.ReadAllBytes(filename);
			var magic = ByteConverter.ToUInt64(file, 0) & FormatMask;
			
			if (magic == SAANIM)
			{
				var version = file[7];
				
				if (version > CurrentVersion)
				{
					ByteConverter.BigEndian = be;
					throw new FormatException("Not a valid SAANIM file.");
				}
				
				string description = null;
				string actionName = null;
				string objectName = null;
				
				var aniaddr = ByteConverter.ToInt32(file, 8);
				var labels = new Dictionary<int, string>();
				var tmpaddr = BitConverter.ToInt32(file, 0xC);
				
				if (version >= 2)
				{
					if (tmpaddr != 0)
					{
						var finished = false;
						while (!finished)
						{
							var type = (ChunkTypes)ByteConverter.ToUInt32(file, tmpaddr);
							var chunksz = ByteConverter.ToInt32(file, tmpaddr + 4);
							var nextchunk = tmpaddr + 8 + chunksz;
							tmpaddr += 8;
							var chunk = new byte[chunksz];
							Array.Copy(file, tmpaddr, chunk, 0, chunksz);
							var chunkaddr = 0;
							
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
					{
						labels.Add(aniaddr, file.GetCString(tmpaddr));
					}
				}
				if (version > 0)
				{
					numModels = BitConverter.ToInt32(file, 0x10);
				}
				else if (numModels == -1)
				{
					ByteConverter.BigEndian = be;
					throw new NotImplementedException("Cannot open version 0 animations without a model!");
				}
				
				var anim = new NJS_MOTION(file, aniaddr, 0, numModels & int.MaxValue, labels, numModels < 0, shortCheck: false) { Description = description, ActionName = actionName, ObjectName = objectName };
				ByteConverter.BigEndian = be;
				return anim;
			}
			
			ByteConverter.BigEndian = be;
			throw new FormatException("Not a valid SAANIM file.");
		}

		public static bool CheckAnimationFile(string filename)
		{
			var be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			var file = File.ReadAllBytes(filename);
			var magic = ByteConverter.ToUInt64(file, 0) & FormatMask;
			ByteConverter.BigEndian = be;
			
			if (magic == SAANIM)
			{
				return file[7] <= CurrentVersion;
			}

			return false;
		}
		
		public struct Data(uint offset, int frame)
		{
			public uint Offset = offset;
			public int Frame = frame;
		}

		public byte[] GetBytes(uint imageBase, Dictionary<string, uint> labels, out uint address, bool useNMDM = false)
		{
			var result = new List<byte>();
			var parameterData = new List<byte>();
			var pofOffsets = new List<uint>();
			AnimFlags flags = 0;

			var data = new Dictionary<AnimFlags, Data[]>();

			pofOffsets.Add(0); // First offset in the motion

			foreach (var model in Models)
			{
				if (model.Value.Position.Count > 0)
				{
					var posData = new Data[ModelParts];
					
					flags |= AnimFlags.Position;
					result.Align(4);
					posData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(posData[model.Key].Offset) && model.Value.PositionName != null)
					{
						if (!labels.TryAdd(model.Value.PositionName, posData[model.Key].Offset))
						{
							var newName = model.Value.PositionName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, posData[model.Key].Offset);
						}
					}
					
					posData[model.Key].Frame = model.Value.Position.Count;
					foreach (var item in model.Value.Position)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}

					data[AnimFlags.Position] = posData;
				}
				
				if (model.Value.Rotation.Count > 0)
				{
					var rotData = new Data[ModelParts];
					
					flags |= AnimFlags.Rotation;
					result.Align(4);
					rotData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(rotData[model.Key].Offset) && model.Value.RotationName != null)
					{
						if (!labels.TryAdd(model.Value.RotationName, rotData[model.Key].Offset))
						{
							var newName = model.Value.RotationName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, rotData[model.Key].Offset);
						}
					}
					
					rotData[model.Key].Frame = model.Value.Rotation.Count;
					foreach (var item in model.Value.Rotation)
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
					
					data[AnimFlags.Rotation] = rotData;
				}
				
				if (model.Value.Scale.Count > 0)
				{
					var sclData = new Data[ModelParts];
					
					flags |= AnimFlags.Scale;
					result.Align(4);
					sclData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(sclData[model.Key].Offset) && model.Value.ScaleName != null)
					{
						if (!labels.TryAdd(model.Value.ScaleName, sclData[model.Key].Offset))
						{
							var newName = model.Value.ScaleName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, sclData[model.Key].Offset);
						}
					}
					
					sclData[model.Key].Frame = model.Value.Scale.Count;
					foreach (var item in model.Value.Scale)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}
					
					data[AnimFlags.Scale] = sclData;
				}
				
				if (model.Value.Vector.Count > 0)
				{
					var vecData = new Data[ModelParts];
					
					flags |= AnimFlags.Vector;
					result.Align(4);
					vecData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(vecData[model.Key].Offset) && model.Value.VectorName != null)
					{
						if (!labels.TryAdd(model.Value.VectorName, vecData[model.Key].Offset))
						{
							var newName = model.Value.VectorName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, vecData[model.Key].Offset);
						}
					}
					
					vecData[model.Key].Frame = model.Value.Vector.Count;
					foreach (var item in model.Value.Vector)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}

					data[AnimFlags.Vector] = vecData;
				}
				
				if (model.Value.Vertex.Count > 0)
				{
					var vertData = new Data[ModelParts];
					
					flags |= AnimFlags.Vertex;
					result.Align(4);
					var offs = new List<uint>();
					var voffs = new List<(Vertex[] vlist, uint off)>();
					
					foreach (var item in model.Value.Vertex)
					{
						var found = false;
						foreach (var (vlist, off) in voffs)
						{
							if (item.Value.SequenceEqual(vlist))
							{
								offs.Add(off);
								found = true;
								break;
							}
						}

						if (optimizeMotions && found)
						{
							continue;
						}

						result.Align(4);
						offs.Add(imageBase + (uint)result.Count);
						if (optimizeMotions)
						{
							voffs.Add((item.Value, imageBase + (uint)result.Count));
						}

						foreach (var v in item.Value)
						{
							result.AddRange(v.GetBytes());
						}
					}
					
					vertData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(vertData[model.Key].Offset) && model.Value.VertexName != null)
					{
						if (!labels.TryAdd(model.Value.VertexName, vertData[model.Key].Offset))
						{
							var newName = model.Value.VertexName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, vertData[model.Key].Offset);
						}
					}
					
					vertData[model.Key].Frame = model.Value.Vertex.Count;
					var i = 0;
					
					foreach (var item in model.Value.Vertex)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(offs[i++]));
					}
					
					for (var u = 0; u < model.Value.Vertex.Count; u++)
					{
						if (!labels.ContainsValue(offs[u]) && model.Value.VertexItemName[u] != null)
						{
							if (!labels.ContainsKey(model.Value.VertexItemName[u]))
							{
								labels.Add(model.Value.VertexItemName[u], offs[u]);
							}
							else
							{
								var newName = model.Value.VertexItemName[u];
								do
								{
									newName += "_dup";
								} while (labels.ContainsKey(newName));
								labels.Add(newName, offs[u]);
							}
						}
					}

					data[AnimFlags.Vertex] = vertData;
				}
				
				if (model.Value.Normal.Count > 0)
				{
					var normData = new Data[ModelParts];
					
					flags |= AnimFlags.Normal;
					result.Align(4);
					var offs = new List<uint>();
					var voffs = new List<(Vertex[] vlist, uint off)>();
					foreach (var item in model.Value.Normal)
					{
						var found = false;
						foreach (var (vlist, off) in voffs)
						{
							if (item.Value.SequenceEqual(vlist))
							{
								offs.Add(off);
								found = true;
								break;
							}
						}

						if (optimizeMotions && found)
						{
							continue;
						}

						result.Align(4);
						offs.Add(imageBase + (uint)result.Count);
						if (optimizeMotions)
						{
							voffs.Add((item.Value, imageBase + (uint)result.Count));
						}

						foreach (var v in item.Value)
						{
							result.AddRange(v.GetBytes());
						}
					}
					
					normData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(normData[model.Key].Offset) && model.Value.NormalName != null)
					{
						if (!labels.TryAdd(model.Value.NormalName, normData[model.Key].Offset))
						{
							var newName = model.Value.NormalName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, normData[model.Key].Offset);
						}
					}
					
					normData[model.Key].Frame = model.Value.Normal.Count;
					
					var i = 0;
					foreach (var item in model.Value.Normal)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(offs[i++]));
					}
					
					for (var u = 0; u < model.Value.Normal.Count; u++)
					{
						if (!labels.ContainsValue(offs[u]) && model.Value.NormalItemName[u] != null)
						{
							if (!labels.ContainsKey(model.Value.NormalItemName[u]))
							{
								labels.Add(model.Value.NormalItemName[u], offs[u]);
							}
							else
							{
								var newName = model.Value.NormalItemName[u];
								do
								{
									newName += "_dup";
								} while (labels.ContainsKey(newName));
								labels.Add(newName, offs[u]);
							}
						}
					}
					
					data[AnimFlags.Normal] = normData;
				}
				
				if (model.Value.Target.Count > 0)
				{
					var targData = new Data[ModelParts];
					
					flags |= AnimFlags.Target;
					result.Align(4);
					targData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(targData[model.Key].Offset) && model.Value.TargetName != null)
					{
						if (!labels.TryAdd(model.Value.TargetName, targData[model.Key].Offset))
						{
							var newName = model.Value.TargetName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, targData[model.Key].Offset);
						}
					}
					
					targData[model.Key].Frame = model.Value.Target.Count;
					foreach (var item in model.Value.Target)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}
					
					data[AnimFlags.Target] = targData;
				}
				
				if (model.Value.Roll.Count > 0)
				{
					var rollData = new Data[ModelParts];
					
					flags |= AnimFlags.Roll;
					result.Align(4);
					rollData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(rollData[model.Key].Offset) && model.Value.RollName != null)
					{
						if (!labels.TryAdd(model.Value.RollName, rollData[model.Key].Offset))
						{
							var newName = model.Value.RollName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, rollData[model.Key].Offset);
						}
					}
					
					rollData[model.Key].Frame = model.Value.Roll.Count;
					foreach (var item in model.Value.Roll)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value));
					}
					
					data[AnimFlags.Roll] = rollData;
				}
				
				if (model.Value.Angle.Count > 0)
				{
					var angData = new Data[ModelParts];
					
					flags |= AnimFlags.Angle;
					result.Align(4);
					angData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(angData[model.Key].Offset) && model.Value.AngleName != null)
					{
						if (!labels.TryAdd(model.Value.AngleName, angData[model.Key].Offset))
						{
							var newName = model.Value.AngleName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, angData[model.Key].Offset);
						}
					}
					
					angData[model.Key].Frame = model.Value.Angle.Count;
					foreach (var item in model.Value.Angle)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value));
					}

					data[AnimFlags.Angle] = angData;
				}
				
				if (model.Value.Color.Count > 0)
				{
					var colData = new Data[ModelParts];
					
					flags |= AnimFlags.Color;
					result.Align(4);
					colData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(colData[model.Key].Offset) && model.Value.ColorName != null)
					{
						if (!labels.TryAdd(model.Value.ColorName, colData[model.Key].Offset))
						{
							var newName = model.Value.ColorName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, colData[model.Key].Offset);
						}
					}
					
					colData[model.Key].Frame = model.Value.Color.Count;
					foreach (var item in model.Value.Color)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value));
					}
					
					data[AnimFlags.Color] = colData;
				}
				
				if (model.Value.Intensity.Count > 0)
				{
					var intData = new Data[ModelParts];
					
					flags |= AnimFlags.Intensity;
					result.Align(4);
					intData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(intData[model.Key].Offset) && model.Value.IntensityName != null)
					{
						if (!labels.TryAdd(model.Value.IntensityName, intData[model.Key].Offset))
						{
							var newName = model.Value.IntensityName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, intData[model.Key].Offset);
						}
					}
					
					intData[model.Key].Frame = model.Value.Intensity.Count;
					foreach (var item in model.Value.Intensity)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value));
					}

					data[AnimFlags.Intensity] = intData;
				}
				
				if (model.Value.Spot.Count > 0)
				{
					var spotData = new Data[ModelParts];
					
					flags |= AnimFlags.Spot;
					result.Align(4);
					spotData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(spotData[model.Key].Offset) && model.Value.SpotName != null)
					{
						if (!labels.TryAdd(model.Value.SpotName, spotData[model.Key].Offset))
						{
							var newName = model.Value.SpotName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, spotData[model.Key].Offset);
						}
					}
					
					spotData[model.Key].Frame = model.Value.Spot.Count;
					foreach (var item in model.Value.Spot)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(item.Value.GetBytes());
					}
					
					data[AnimFlags.Spot] = spotData;
				}
				
				if (model.Value.Point.Count > 0)
				{
					var pntData = new Data[ModelParts];
					
					flags |= AnimFlags.Point;
					result.Align(4);
					pntData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(pntData[model.Key].Offset) && model.Value.PointName != null)
					{
						if (!labels.TryAdd(model.Value.PointName, pntData[model.Key].Offset))
						{
							var newName = model.Value.PointName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, pntData[model.Key].Offset);
						}
					}
					
					pntData[model.Key].Frame = model.Value.Point.Count;
					foreach (var item in model.Value.Point)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value[0]));
						result.AddRange(ByteConverter.GetBytes(item.Value[1]));
					}

					data[AnimFlags.Point] = pntData;
				}
				
				if (model.Value.Quaternion.Count > 0)
				{
					var quatData = new Data[ModelParts];
					
					flags |= AnimFlags.Quaternion;
					result.Align(4);
					quatData[model.Key].Offset = imageBase + (uint)result.Count;

					if (!labels.ContainsValue(quatData[model.Key].Offset) && model.Value.QuaternionName != null)
					{
						if (!labels.TryAdd(model.Value.QuaternionName, quatData[model.Key].Offset))
						{
							var newName = model.Value.QuaternionName;
							do
							{
								newName += "_dup";
							} while (labels.ContainsKey(newName));
							labels.Add(newName, quatData[model.Key].Offset);
						}
					}
					
					quatData[model.Key].Frame = model.Value.Quaternion.Count;
					foreach (var item in model.Value.Quaternion)
					{
						result.AddRange(ByteConverter.GetBytes(item.Key));
						result.AddRange(ByteConverter.GetBytes(item.Value[0]));
						result.AddRange(ByteConverter.GetBytes(item.Value[1]));
						result.AddRange(ByteConverter.GetBytes(item.Value[2]));
						result.AddRange(ByteConverter.GetBytes(item.Value[3]));
					}

					data[AnimFlags.Quaternion] = quatData;
				}
			}
			
			result.Align(4);

			var numPairs = BitOperations.PopCount((uint)flags);
			
			switch (flags)
			{
				case AnimFlags.Position:
				case AnimFlags.Rotation:
					flags = AnimFlags.Position | AnimFlags.Rotation;
					numPairs = 2;
					break;
				case AnimFlags.Scale:
					flags |= AnimFlags.Rotation;
					numPairs++;
					break;
				case AnimFlags.Vertex:
				case AnimFlags.Normal:
					flags = AnimFlags.Vertex | AnimFlags.Normal;
					numPairs = 2;
					break;
			}
			
			var modelData = imageBase + (uint)result.Count;
			
			// Dealing with uninitialized data. 
			// This is to avoid MDATA and MOTIONS sharing the same address, which interferes with labels.
			if (result.Count == 0 && numPairs == 0 && Models.Count == 0 && flags == 0)
			{
				flags |= AnimFlags.Position;
				result.Align(4);

				data[AnimFlags.Position][0] = new Data(imageBase + (uint)result.Count, 1);
				
				result.AddRange(ByteConverter.GetBytes(0));
				var temp = new Vertex(0.0f, 0.0f, 0.0f);
				result.AddRange(temp.GetBytes());
			}
			
			if (!labels.ContainsValue(modelData) && MdataName != null)
			{
				if (!labels.TryAdd(MdataName, modelData))
				{
					var newName = MdataName;
					do
					{
						newName += "_dup";
					} while (labels.ContainsKey(newName));
					labels.Add(newName, modelData);
				}
			}
			
			for (var i = 0; i < ModelParts; i++)
			{
				// TODO: Avoid double iteration
				
				foreach (var value in Enum.GetValues<AnimFlags>())
				{
					if (flags.HasFlag(value))
					{
						var offset = data[value][i].Offset;
						result.AddRange(ByteConverter.GetBytes(offset));
						pofOffsets.Add(offset);
					}
				}
				
				foreach (var value in Enum.GetValues<AnimFlags>())
				{
					if (flags.HasFlag(value))
					{
						var frame = data[value][i].Frame;
						result.AddRange(ByteConverter.GetBytes(frame));
					}
				}
			}
			
			result.Align(4);
			address = (uint)result.Count;

			parameterData.AddRange(ByteConverter.GetBytes(modelData));
			parameterData.AddRange(ByteConverter.GetBytes(Frames));
			parameterData.AddRange(ByteConverter.GetBytes((ushort)flags));

			numPairs |= (ushort)(InterpolationMode switch
			{
				InterpolationMode.Linear => StructEnums.NJD_MTYPE_FN.NJD_MTYPE_LINER,
				InterpolationMode.Spline => StructEnums.NJD_MTYPE_FN.NJD_MTYPE_SPLINE,
				InterpolationMode.User => StructEnums.NJD_MTYPE_FN.NJD_MTYPE_USER,
			});
			
			parameterData.AddRange(ByteConverter.GetBytes(numPairs));
			
			if (!labels.ContainsValue(address + imageBase) && Name != null)
			{
				if (!labels.ContainsKey(Name))
				{
					labels.Add(Name, address + imageBase);
				}
				else
				{
					var newName = Name;
					do
					{
						newName += "_dup";
					} while (labels.ContainsKey(newName));
					labels.Add(newName, address + imageBase);
				}
			}

			if (useNMDM)
			{
				result.InsertRange(0, parameterData.ToArray());
				result.InsertRange(0, BitConverter.GetBytes(result.Count)); // This int is always little endian!
				result.InsertRange(0, "NMDM"u8.ToArray()); // NMDM Magic
				result.AddRange(POF0Helper.GetPOFData(pofOffsets));
			}
			else
			{
				result.AddRange(parameterData.ToArray());
			}

			return result.ToArray();
		}

		public byte[] GetBytes(uint imageBase, out uint address)
		{
			return GetBytes(imageBase, new Dictionary<string, uint>(), out address);
		}

		public void ToStructVariables(TextWriter writer, List<string> labels = null)
		{
			var hasPos = false;
			var hasRot = false;
			var hasScl = false;
			var hasVec = false;
			var hasVert = false;
			var hasNorm = false;
			var hasTarg = false;
			var hasRoll = false;
			var hasAng = false;
			var hasCol = false;
			var hasInt = false;
			var hasSpot = false;
			var hasPnt = false;
			var hasQuat = false;
			var id = Name.MakeIdentifier();
			if (labels == null)
			{
				labels = new List<string>();
			}

			foreach (var model in Models)
			{
				if (model.Value.Position.Count > 0 && !labels.Contains(model.Value.PositionName))
				{
					hasPos = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(model.Value.PositionName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Position.Count);
					foreach (var item in model.Value.Position)
					{
						lines.Add(
							$"\t{{ {item.Key}, {item.Value.X.ToC()}, {item.Value.Y.ToC()}, {item.Value.Z.ToC()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.PositionName);
				}
				if (model.Value.Rotation.Count > 0 && !labels.Contains(model.Value.RotationName))
				{
					hasRot = true;
					if (ShortRot)
					{
						writer.Write("NJS_MKEY_SA ");
					}
					else
					{
						writer.Write("NJS_MKEY_A ");
					}

					writer.Write(model.Value.RotationName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Rotation.Count);
					foreach (var item in model.Value.Rotation)
					{
						if (ShortRot)
						{
							lines.Add(
								$"\t{{ {item.Key}, {((short)item.Value.X).ToCHex()}, {((short)item.Value.Y).ToCHex()}, {((short)item.Value.Z).ToCHex()} }}");
						}
						else
						{
							lines.Add(
								$"\t{{ {item.Key}, {item.Value.X.ToCHex()}, {item.Value.Y.ToCHex()}, {item.Value.Z.ToCHex()} }}");
						}
					}
					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.RotationName);
				}
				if (model.Value.Scale.Count > 0 && !labels.Contains(model.Value.ScaleName))
				{
					hasScl = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(model.Value.ScaleName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Scale.Count);
					foreach (var item in model.Value.Scale)
					{
						lines.Add(
							$"\t{{ {item.Key}, {item.Value.X.ToC()}, {item.Value.Y.ToC()}, {item.Value.Z.ToC()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.ScaleName);
				}
				if (model.Value.Vector.Count > 0 && !labels.Contains(model.Value.VectorName))
				{
					hasVec = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(model.Value.VectorName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Vector.Count);
					foreach (var item in model.Value.Vector)
					{
						lines.Add(
							$"\t{{ {item.Key}, {item.Value.X.ToC()}, {item.Value.Y.ToC()}, {item.Value.Z.ToC()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.VectorName);
				}
				if (model.Value.Vertex.Count > 0 && !labels.Contains(model.Value.VertexName))
				{
					hasVert = true;
					var z = 0;
					foreach (var item in model.Value.Vertex)
					{
						if (!labels.Contains(model.Value.VertexItemName[z]))
						{
							writer.Write("NJS_VECTOR ");
							writer.Write(model.Value.VertexItemName[z].MakeIdentifier());
							writer.WriteLine("[] = {");
							var l2 = new List<string>(item.Value.Length);
							foreach (var v in item.Value)
							{
								l2.Add($"\t{v.ToStruct()}");
							}

							writer.WriteLine(string.Join($",{Environment.NewLine}", l2.ToArray()));
							writer.WriteLine("};");
							writer.WriteLine();
							labels.Add(model.Value.VertexItemName[z]);
						}
						z++;					
					}
					writer.Write("NJS_MKEY_P ");
					writer.Write(model.Value.VertexName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Vertex.Count);
					var v_c = 0;
					foreach (var item in model.Value.Vertex)
					{
						lines.Add($"\t{{ {item.Key}, {model.Value.VertexItemName[v_c++].MakeIdentifier()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.VertexName);
				}
				if (model.Value.Normal.Count > 0 && !labels.Contains(model.Value.NormalName))
				{
					hasNorm = true;
					var z = 0;
					foreach (var item in model.Value.Normal)
					{
						if (!labels.Contains(model.Value.NormalItemName[z]))
						{
							writer.Write("NJS_VECTOR ");
							writer.Write(model.Value.NormalItemName[z].MakeIdentifier());
							writer.WriteLine("[] = {");
							var l2 = new List<string>(item.Value.Length);
							foreach (var v in item.Value)
							{
								l2.Add($"\t{v.ToStruct()}");
							}

							writer.WriteLine(string.Join($",{Environment.NewLine}", l2.ToArray()));
							writer.WriteLine("};");
							writer.WriteLine();
							labels.Add(model.Value.NormalItemName[z]);
						}
						z++;			
					}
					writer.Write("NJS_MKEY_P ");
					writer.Write(model.Value.NormalName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Vertex.Count);
					var v_c = 0;
					foreach (var item in model.Value.Vertex)
					{
						lines.Add($"\t{{ {item.Key}, {model.Value.NormalItemName[v_c++].MakeIdentifier()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.NormalName);
				}
				if (model.Value.Target.Count > 0 && !labels.Contains(model.Value.TargetName))
				{
					hasTarg = true;
					writer.Write("NJS_MKEY_F ");
					writer.Write(model.Value.TargetName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Target.Count);
					foreach (var item in model.Value.Target)
					{
						lines.Add(
							$"\t{{ {item.Key}, {item.Value.X.ToC()}, {item.Value.Y.ToC()}, {item.Value.Z.ToC()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.TargetName);
				}
				if (model.Value.Roll.Count > 0 && !labels.Contains(model.Value.RollName))
				{
					hasRoll = true;
					writer.Write("NJS_MKEY_A1 ");
					writer.Write(model.Value.RollName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Roll.Count);
					foreach (var item in model.Value.Roll)
					{
						lines.Add($"\t{{ {item.Key}, {item.Value.ToCHex()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.RollName);
				}
				if (model.Value.Angle.Count > 0 && !labels.Contains(model.Value.AngleName))
				{
					hasAng = true;
					writer.Write("NJS_MKEY_A1 ");
					writer.Write(model.Value.AngleName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Angle.Count);
					foreach (var item in model.Value.Angle)
					{
						lines.Add($"\t{{ {item.Key}, {item.Value.ToCHex()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.AngleName);
				}
				if (model.Value.Color.Count > 0 && !labels.Contains(model.Value.ColorName))
				{
					hasCol = true;
					writer.Write("NJS_MKEY_UI32 ");
					writer.Write(model.Value.ColorName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Color.Count);
					foreach (var item in model.Value.Color)
					{
						lines.Add($"\t{{ {item.Key}, {item.Value.ToCHex()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.ColorName);
				}
				if (model.Value.Intensity.Count > 0 && !labels.Contains(model.Value.IntensityName))
				{
					hasInt = true;
					writer.Write("NJS_MKEY_F1 ");
					writer.Write(model.Value.IntensityName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Intensity.Count);
					foreach (var item in model.Value.Intensity)
					{
						lines.Add($"\t{{ {item.Key}, {item.Value.ToC()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.IntensityName);
				}
				if (model.Value.Spot.Count > 0 && !labels.Contains(model.Value.SpotName))
				{
					hasSpot = true;
					writer.Write("NJS_MKEY_SPOT ");
					writer.Write(model.Value.SpotName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Spot.Count);
					foreach (var item in model.Value.Spot)
					{
						lines.Add(
							$"\t{{ {item.Key}, {item.Value.Near.ToC()}, {item.Value.Far.ToC()}, {item.Value.InsideAngle.ToCHex()}, {item.Value.OutsideAngle.ToCHex()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.SpotName);
				}
				if (model.Value.Point.Count > 0 && !labels.Contains(model.Value.PointName))
				{
					hasPnt = true;
					writer.Write("NJS_MKEY_F2 ");
					writer.Write(model.Value.PointName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Point.Count);
					foreach (var item in model.Value.Point)
					{
						lines.Add($"\t{{ {item.Key}, {item.Value[0].ToC()}, {item.Value[1].ToC()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
					labels.Add(model.Value.PointName);
				}
				if (model.Value.Quaternion.Count > 0 && !labels.Contains(model.Value.QuaternionName))
				{
					hasPnt = true;
					writer.Write("NJS_MKEY_QUAT ");
					writer.Write(model.Value.QuaternionName.MakeIdentifier());
					writer.WriteLine("[] = {");
					var lines = new List<string>(model.Value.Quaternion.Count);
					foreach (var item in model.Value.Quaternion)
					{
						lines.Add(
							$"\t{{ {item.Key}, {item.Value[0].ToC()}, {item.Value[1].ToC()}{item.Value[2].ToC()}{item.Value[3].ToC()} }}");
					}

					writer.WriteLine(string.Join($",{Environment.NewLine}", lines.ToArray()));
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
				if (numpairs == 0)
				{
					writer.Write(2);
				}
				else
				{
					writer.Write(numpairs);
				}

				writer.Write(" ");
				writer.Write(MdataName.MakeIdentifier());
				writer.WriteLine("[] = {");
				var mdats = new List<string>(ModelParts);
				for (var i = 0; i < ModelParts; i++)
				{
					var elems = new List<string>(numpairs * 2);
					if (hasPos)
					{
						if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
						{
							elems.Add(Models[i].PositionName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasRot)
					{
						if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
						{
							elems.Add(Models[i].RotationName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasScl)
					{
						if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
						{
							elems.Add(Models[i].ScaleName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasVec)
					{
						if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
						{
							elems.Add(Models[i].VectorName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasVert)
					{
						if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
						{
							elems.Add(Models[i].VertexName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasNorm)
					{
						if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
						{
							elems.Add(Models[i].NormalName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasTarg)
					{
						if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
						{
							elems.Add(Models[i].TargetName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasRoll)
					{
						if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
						{
							elems.Add(Models[i].RollName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasAng)
					{
						if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
						{
							elems.Add(Models[i].AngleName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasCol)
					{
						if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
						{
							elems.Add(Models[i].ColorName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasInt)
					{
						if (Models.ContainsKey(i) && Models[i].Intensity.Count > 0)
						{
							elems.Add(Models[i].IntensityName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasSpot)
					{
						if (Models.ContainsKey(i) && Models[i].Spot.Count > 0)
						{
							elems.Add(Models[i].SpotName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasPnt)
					{
						if (Models.ContainsKey(i) && Models[i].Point.Count > 0)
						{
							elems.Add(Models[i].PointName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasQuat)
					{
						if (Models.ContainsKey(i) && Models[i].Quaternion.Count > 0)
						{
							elems.Add(Models[i].QuaternionName.MakeIdentifier());
						}
						else
						{
							elems.Add("NULL");
						}
					}
					if (hasPos)
					{
						if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].PositionName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasRot)
					{
						if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].RotationName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasScl)
					{
						if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].ScaleName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasVec)
					{
						if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].VectorName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasVert)
					{
						if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].VertexName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasNorm)
					{
						if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].NormalName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasTarg)
					{
						if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].TargetName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasRoll)
					{
						if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].RollName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasAng)
					{
						if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].AngleName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasCol)
					{
						if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].ColorName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasInt)
					{
						if (Models.ContainsKey(i) && Models[i].Intensity.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].IntensityName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasSpot)
					{
						if (Models.ContainsKey(i) && Models[i].Spot.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].SpotName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasPnt)
					{
						if (Models.ContainsKey(i) && Models[i].Point.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].PointName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					if (hasQuat)
					{
						if (Models.ContainsKey(i) && Models[i].Quaternion.Count > 0)
						{
							elems.Add($"LengthOfArray<Uint32>({Models[i].QuaternionName.MakeIdentifier()})");
						}
						else
						{
							elems.Add("0");
						}
					}
					mdats.Add($"\t{{ {string.Join(", ", elems.ToArray())} }}");
				}
				writer.WriteLine(string.Join($",{Environment.NewLine}", mdats.ToArray()));
				writer.WriteLine("};");
				writer.WriteLine();
				labels.Add(MdataName);
			}
			
			if (!labels.Contains(Name))
			{
				writer.Write("NJS_MOTION ");
				writer.Write(Name.MakeIdentifier());
				writer.Write(" = { ");
				writer.Write("{0}, ", MdataName.MakeIdentifier());
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
				writer.Write(ActionName.MakeIdentifier());
				writer.Write(" = { &");
				writer.Write(ObjectName.MakeIdentifier());
				writer.Write(", &");
				writer.Write(Name.MakeIdentifier());
				writer.WriteLine(" };");
				labels.Add(ActionName);
			}
		}

		public string ToStructVariables(List<string> labels = null)
		{
			using var sw = new StringWriter();
			ToStructVariables(sw, labels);
			return sw.ToString();
		}

		public void ToNJA(TextWriter writer, List<string> labels = null, bool isDum = false)
		{
			var hasPos = false;
			var hasRot = false;
			var hasScl = false;
			var hasVec = false;
			var hasVert = false;
			var hasNorm = false;
			var hasTarg = false;
			var hasRoll = false;
			var hasAng = false;
			var hasCol = false;
			var hasInt = false;
			var hasSpot = false;
			var hasPnt = false;
			var hasQuat = false;
			
			if (labels == null)
			{
				labels = new List<string>();
			}

			writer.WriteLine(IsShapeMotion() ? "SHAPE_MOTION_START" : "MOTION_START");
			if (!isDum)
			{
				writer.WriteLine();
				
				foreach (var model in Models)
				{
					// Not implemented: Target, Roll, Angle, Color, Intensity, Spot, Point
					if (model.Value.Position.Count > 0 && !labels.Contains(model.Value.PositionName))
					{
						hasPos = true;
						writer.WriteLine("POSITION {0}[]", model.Value.PositionName.MakeIdentifier());
						writer.WriteLine("START");
						
						foreach (var item in model.Value.Position)
						{
							writer.WriteLine($"         MKEYF( {item.Key},   {item.Value.X.ToNJA()}, {item.Value.Y.ToNJA()}, {item.Value.Z.ToNJA()} ),");
						}

						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.PositionName);
					}
					if (model.Value.Rotation.Count > 0 && !labels.Contains(model.Value.RotationName))
					{
						hasRot = true;
						
						if (ShortRot)
						{
							writer.Write("SROTATION ");
						}
						else
						{
							writer.Write("ROTATION ");
						}

						writer.Write(model.Value.RotationName.MakeIdentifier());
						writer.WriteLine("[]");
						writer.WriteLine("START");
						
						foreach (var item in model.Value.Rotation)
						{
							if (ShortRot)
							{
								writer.WriteLine($"         MKEYSA( {item.Key},   {(((short)item.Value.X) / 182.044f).ToNJA()}, {(((short)item.Value.Y) / 182.044f).ToNJA()}, {(((short)item.Value.Z) / 182.044f).ToNJA()} ),");
							}
							else
							{
								writer.WriteLine($"         MKEYA( {item.Key},   {(item.Value.X / 182.044f).ToNJA()}, {(item.Value.Y / 182.044f).ToNJA()}, {(item.Value.Z / 182.044f).ToNJA()} ),");
							}
						}
						
						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.RotationName);
					}
					if (model.Value.Scale.Count > 0 && !labels.Contains(model.Value.ScaleName))
					{
						hasScl = true;
						writer.WriteLine("SCALE {0}[]", model.Value.ScaleName.MakeIdentifier());
						writer.WriteLine("START");
						
						foreach (var item in model.Value.Scale)
						{
							writer.WriteLine($"         MKEYF( {item.Key},   {item.Value.X.ToNJA()}, {item.Value.Y.ToNJA()}, {item.Value.Z.ToNJA()} ),");
						}

						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.ScaleName);
					}
					if (model.Value.Vector.Count > 0 && !labels.Contains(model.Value.VectorName))
					{
						hasVec = true;
						writer.WriteLine("VECTOR {0}[]", model.Value.VectorName.MakeIdentifier());
						writer.WriteLine("START");
						
						foreach (var item in model.Value.Vector)
						{
							writer.WriteLine($"         MKEYF( {item.Key},   {item.Value.X.ToNJA()}, {item.Value.Y.ToNJA()}, {item.Value.Z.ToNJA()} ),");
						}

						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.VectorName);
					}
					if (model.Value.Vertex.Count > 0 && !labels.Contains(model.Value.VertexName))
					{
						hasVert = true;
						
						var z = 0;
						foreach (var item in model.Value.Vertex)
						{
							if (!labels.Contains(model.Value.VertexItemName[z]))
							{
								writer.WriteLine("POINT    {0}[]", model.Value.VertexItemName[z].MakeIdentifier());
								writer.WriteLine("START");

								foreach (var v in item.Value)
								{
									writer.WriteLine("         VERT{0},", v.ToNJA());
								}

								writer.WriteLine("END");
								writer.WriteLine();
								labels.Add(model.Value.VertexItemName[z]);
							}
							z++;
						}
						
						writer.WriteLine();
						writer.WriteLine("POINTER    {0}[]", model.Value.VertexName.MakeIdentifier());
						writer.WriteLine("START");
						
						var v_c = 0;
						foreach (var item in model.Value.Vertex)
						{
							writer.WriteLine($"         MKEYP( {item.Key}, {model.Value.VertexItemName[v_c++].MakeIdentifier()}),");
						}

						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.VertexName);
					}
					
					if (model.Value.Normal.Count > 0 && !labels.Contains(model.Value.NormalName))
					{
						hasNorm = true;
						
						var z = 0;
						foreach (var item in model.Value.Normal)
						{
							if (!labels.Contains(model.Value.NormalItemName[z]))
							{
								writer.WriteLine("NORMAL    {0}[]", model.Value.NormalItemName[z].MakeIdentifier());
								writer.WriteLine("START");
								
								foreach (var v in item.Value)
								{
									writer.WriteLine("         NORM{0},", v.ToNJA());
								}

								writer.WriteLine("END");
								writer.WriteLine();
								labels.Add(model.Value.NormalItemName[z]);
							}
							z++;
						}
						
						writer.WriteLine();
						writer.WriteLine("POINTER    {0}[]", model.Value.NormalName.MakeIdentifier());
						writer.WriteLine("START");
						
						var v_c = 0;
						foreach (var item in model.Value.Normal)
						{
							writer.WriteLine($"         MKEYP( {item.Key}, {model.Value.NormalItemName[v_c++].MakeIdentifier()}),");
						}

						writer.WriteLine("END");
						writer.WriteLine();
						labels.Add(model.Value.NormalName);
					}
					
					if (model.Value.Quaternion.Count > 0 && !labels.Contains(model.Value.QuaternionName))
					{
						hasQuat = true;
						writer.WriteLine("QROTATION {0}[]", model.Value.QuaternionName.MakeIdentifier());
						writer.WriteLine("START");
						
						foreach (var item in model.Value.Quaternion)
						{
							writer.WriteLine($"         MKEYQ( {item.Key},   {item.Value[0].ToNJA()}, {item.Value[1].ToNJA()}, {item.Value[2].ToNJA()}, {item.Value[3].ToNJA()} ),");
						}

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
					{
						writer.Write(2);
					}
					else
					{
						writer.Write(numpairs);
					}

					writer.Write(" ");
					writer.Write(MdataName.MakeIdentifier());
					writer.WriteLine("[]");
					writer.WriteLine("START");
					var mdats = new List<string>(ModelParts);
					
					for (var i = 0; i < ModelParts; i++)
					{
						var elems = new List<string>(numpairs * 2);
						if (hasPos)
						{
							if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
							{
								elems.Add(Models[i].PositionName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasRot)
						{
							if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
							{
								elems.Add(Models[i].RotationName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasScl)
						{
							if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
							{
								elems.Add(Models[i].ScaleName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasVec)
						{
							if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
							{
								elems.Add(Models[i].VectorName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasVert)
						{
							if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
							{
								elems.Add(Models[i].VertexName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasNorm)
						{
							if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
							{
								elems.Add(Models[i].NormalName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasTarg)
						{
							if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
							{
								elems.Add(Models[i].TargetName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasRoll)
						{
							if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
							{
								elems.Add(Models[i].PointName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasAng)
						{
							if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
							{
								elems.Add(Models[i].AngleName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasCol)
						{
							if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
							{
								elems.Add(Models[i].ColorName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasInt)
						{
							if (Models.ContainsKey(i) && Models[i].Intensity.Count > 0)
							{
								elems.Add(Models[i].IntensityName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasSpot)
						{
							if (Models.ContainsKey(i) && Models[i].Spot.Count > 0)
							{
								elems.Add(Models[i].SpotName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasPnt)
						{
							if (Models.ContainsKey(i) && Models[i].Point.Count > 0)
							{
								elems.Add(Models[i].PointName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasQuat)
						{
							if (Models.ContainsKey(i) && Models[i].Quaternion.Count > 0)
							{
								elems.Add(Models[i].QuaternionName.MakeIdentifier());
							}
							else
							{
								elems.Add("NULL");
							}
						}
						if (hasPos)
						{
							if (Models.ContainsKey(i) && Models[i].Position.Count > 0)
							{
								elems.Add(Models[i].Position.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						if (hasRot)
						{
							if (Models.ContainsKey(i) && Models[i].Rotation.Count > 0)
							{
								elems.Add(Models[i].Rotation.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						if (hasScl)
						{
							if (Models.ContainsKey(i) && Models[i].Scale.Count > 0)
							{
								elems.Add(Models[i].Scale.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						if (hasVec)
						{
							if (Models.ContainsKey(i) && Models[i].Vector.Count > 0)
							{
								elems.Add(Models[i].Vector.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						if (hasVert)
						{
							if (Models.ContainsKey(i) && Models[i].Vertex.Count > 0)
							{
								elems.Add(Models[i].Vertex.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						if (hasNorm)
						{
							if (Models.ContainsKey(i) && Models[i].Normal.Count > 0)
							{
								elems.Add(Models[i].Normal.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						if (hasTarg)
						{
							if (Models.ContainsKey(i) && Models[i].Target.Count > 0)
							{
								elems.Add(Models[i].Target.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						if (hasRoll)
						{
							if (Models.ContainsKey(i) && Models[i].Roll.Count > 0)
							{
								elems.Add(Models[i].Roll.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						if (hasAng)
						{
							if (Models.ContainsKey(i) && Models[i].Angle.Count > 0)
							{
								elems.Add(Models[i].Angle.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						if (hasCol)
						{
							if (Models.ContainsKey(i) && Models[i].Color.Count > 0)
							{
								elems.Add(Models[i].Color.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						
						if (hasInt)
						{
							if (Models.TryGetValue(i, out var data) && data.Intensity.Count > 0)
							{
								elems.Add(data.Intensity.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						
						if (hasSpot)
						{
							if (Models.TryGetValue(i, out var data) && data.Spot.Count > 0)
							{
								elems.Add(data.Spot.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						
						if (hasPnt)
						{
							if (Models.TryGetValue(i, out var data) && data.Point.Count > 0)
							{
								elems.Add(data.Point.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						
						if (hasQuat)
						{
							if (Models.TryGetValue(i, out var data) && data.Quaternion.Count > 0)
							{
								elems.Add(data.Quaternion.Count.ToString());
							}
							else
							{
								elems.Add("0");
							}
						}
						
						if (elems.Count > 0)
						{
							mdats.Add($"    {string.Join(", ", elems.ToArray())}");
						}
						else
						{
							mdats.Add("    NULL, NULL, NULL, 0, 0, 0");
						}
					}
					
					writer.WriteLine($"{string.Join($",{Environment.NewLine}", mdats.ToArray())},");
					writer.WriteLine("END");
					writer.WriteLine();
					labels.Add(MdataName);
				}
				
				if (!labels.Contains(Name))
				{
					writer.Write("MOTION ");
					writer.Write(Name.MakeIdentifier());
					writer.WriteLine("[]");
					writer.WriteLine("START");
					writer.WriteLine("MdataArray     {0}, ", MdataName.MakeIdentifier());
					writer.WriteLine("MFrameNum      {0},", Frames);
					writer.WriteLine("MotionBit      0x{0},", ((int)flags).ToString("X"));
					var interpol = numpairs > 0 ? numpairs : 2;
					
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
				writer.WriteLine("ACTION {0}[]", ActionName.MakeIdentifier());
				writer.WriteLine("START");
				writer.WriteLine("ObjectHead      {0},", ObjectName.MakeIdentifier());
				writer.WriteLine($"Motion          {Name.MakeIdentifier()}");
				writer.WriteLine("END");
			}
			
			writer.WriteLine(IsShapeMotion() ? "SHAPE_MOTION_END" : "MOTION_END");
			writer.WriteLine();
			writer.WriteLine("DEFAULT_START");
			writer.WriteLine();
			writer.WriteLine($"#ifndef DEFAULT_{(IsShapeMotion() ? "SHAPE" : "MOTION")}_NAME");
			writer.WriteLine($"#define DEFAULT_{(IsShapeMotion() ? "SHAPE" : "MOTION")}_NAME {Name.MakeIdentifier()}");
			writer.WriteLine("#endif");
			
			if (!string.IsNullOrEmpty(ActionName))
			{
				writer.WriteLine("#ifndef DEFAULT_ACTION_NAME");
				writer.WriteLine($"#define DEFAULT_ACTION_NAME {ActionName.MakeIdentifier()}");
				writer.WriteLine("#endif");
			}
			
			writer.WriteLine();
			writer.WriteLine("DEFAULT_END");
		}

		public byte[] WriteHeader(uint imageBase, uint modelAddress, Dictionary<string, uint> labels, out uint address)
		{
			var result = new List<byte>();
			result.AddRange(GetBytes(imageBase, labels, out var head2));
			address = (uint)result.Count;
			result.AddRange(ByteConverter.GetBytes(modelAddress));
			result.AddRange(ByteConverter.GetBytes(head2 + imageBase));
			return result.ToArray();
		}

		public void Save(string filename, bool nometa = false)
		{
			var be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			var file = new List<byte>();
			file.AddRange(ByteConverter.GetBytes(SAANIMVer));
			var labels = new Dictionary<string, uint>();
			var anim = GetBytes(0x14, labels, out var addr);
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
				var chunk = new List<byte>((labels.Count * 8) + 8);
				var straddr = (labels.Count * 8) + 8;
				var strbytes = new List<byte>();
				
				foreach (var label in labels)
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
					var chunkd = new List<byte>(Description.Length + 1);
					chunkd.AddRange(Encoding.UTF8.GetBytes(Description));
					chunkd.Add(0);
					chunkd.Align(4);
					file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Description));
					file.AddRange(ByteConverter.GetBytes(chunkd.Count));
					file.AddRange(chunkd);
				}
				
				if (!string.IsNullOrEmpty(ActionName))
				{
					var chunkd = new List<byte>(ActionName.Length + 1);
					chunkd.AddRange(Encoding.UTF8.GetBytes(ActionName));
					chunkd.Add(0);
					chunkd.Align(4);
					file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.ActionName));
					file.AddRange(ByteConverter.GetBytes(chunkd.Count));
					file.AddRange(chunkd);
				}
				
				if (!string.IsNullOrEmpty(ObjectName))
				{
					var chunkd = new List<byte>(ObjectName.Length + 1);
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
		public readonly Dictionary<int, Vertex> Position = new();
		public readonly Dictionary<int, Rotation> Rotation = new();
		public readonly Dictionary<int, Vertex> Scale = new();
		public readonly Dictionary<int, Vertex> Vector = new();
		public readonly Dictionary<int, Vertex[]> Vertex = new();
		public readonly Dictionary<int, Vertex[]> Normal = new();
		public readonly Dictionary<int, Vertex> Target = new();
		public readonly Dictionary<int, int> Roll = new();
		public readonly Dictionary<int, int> Angle = new();
		public readonly Dictionary<int, uint> Color = new();
		public readonly Dictionary<int, float> Intensity = new();
		public readonly Dictionary<int, Spotlight> Spot = new();
		public readonly Dictionary<int, float[]> Point = new();
		public readonly Dictionary<int, float[]> Quaternion = new();
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

		public Vertex GetPosition(float frame)
		{
			if (Math.Floor(frame) == frame && Position.ContainsKey((int)Math.Floor(frame)))
			{
				return Position[(int)Math.Floor(frame)];
			}

			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			
			foreach (var k in Position.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Position.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Position.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];
			var val = new Vertex()
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
			{
				return Rotation[(int)Math.Floor(frame)];
			}

			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			
			foreach (var k in Rotation.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Rotation.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Rotation.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];
			var val = new Rotation()
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
			{
				return Scale[(int)Math.Floor(frame)];
			}

			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			
			foreach (var k in Scale.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Scale.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Scale.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];
			var val = new Vertex
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
			{
				return Vector[(int)Math.Floor(frame)];
			}

			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			
			foreach (var k in Vector.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Vector.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Vector.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];
			var val = new Vertex
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
			{
				return Vertex[(int)Math.Floor(frame)];
			}

			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			
			foreach (var k in Vertex.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Vertex.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Vertex.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];
			var result = new Vertex[Vertex[f1].Length];
			
			for (var i = 0; i < Vertex[f1].Length; i++)
			{
				result[i] = new Vertex
				{
					X = ((Vertex[f2z][i].X - Vertex[f1][i].X) / diff * (frame - f1)) + Vertex[f1][i].X,
					Y = ((Vertex[f2z][i].Y - Vertex[f1][i].Y) / diff * (frame - f1)) + Vertex[f1][i].Y,
					Z = ((Vertex[f2z][i].Z - Vertex[f1][i].Z) / diff * (frame - f1)) + Vertex[f1][i].Z
				};
			}

			return result;
		}

		public Vertex[] GetNormal(float frame)
		{
			if (Math.Floor(frame) == frame && Normal.ContainsKey((int)Math.Floor(frame)))
			{
				return Normal[(int)Math.Floor(frame)];
			}

			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			
			foreach (var k in Normal.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Normal.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Normal.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];
			var result = new Vertex[Normal[f1].Length];
			
			for (var i = 0; i < Normal[f1].Length; i++)
			{
				result[i] = new Vertex
				{
					X = ((Normal[f2z][i].X - Normal[f1][i].X) / diff * (frame - f1)) + Normal[f1][i].X,
					Y = ((Normal[f2z][i].Y - Normal[f1][i].Y) / diff * (frame - f1)) + Normal[f1][i].Y,
					Z = ((Normal[f2z][i].Z - Normal[f1][i].Z) / diff * (frame - f1)) + Normal[f1][i].Z
				};
			}

			return result;
		}

		public Vertex GetTarget(float frame)
		{
			if (Math.Floor(frame) == frame && Target.ContainsKey((int)Math.Floor(frame)))
			{
				return Target[(int)Math.Floor(frame)];
			}

			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			
			foreach (var k in Target.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Target.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Target.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];
			var val = new Vertex
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
			{
				return Roll[(int)Math.Floor(frame)];
			}

			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			
			foreach (var k in Roll.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Roll.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Roll.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];
			return (int)Math.Round((((Roll[f2z] - Roll[f1]) / (double)diff) * (frame - f1)) + Roll[f1], MidpointRounding.AwayFromZero);
		}

		public int GetAngle(float frame)
		{
			if (Math.Floor(frame) == frame && Angle.ContainsKey((int)Math.Floor(frame)))
			{
				return Angle[(int)Math.Floor(frame)];
			}

			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			
			foreach (var k in Angle.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Angle.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Angle.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];
			return (int)Math.Round((((Angle[f2z] - Angle[f1]) / (double)diff) * (frame - f1)) + Angle[f1], MidpointRounding.AwayFromZero);
		}

		public Rotation GetQuaternion(float frame)
		{
			if (Math.Floor(frame) == frame && Quaternion.ContainsKey((int)Math.Floor(frame)))
			{
				return RotFromQuat(FloatsAsQuat(Quaternion[(int)Math.Floor(frame)]));
			}
			var f1 = 0;
			var f2 = 0;
			var keys = new List<int>();
			foreach (var k in Quaternion.Keys)
			{
				keys.Add(k);
			}

			for (var i = 0; i < Quaternion.Count; i++)
			{
				if (keys[i] < frame)
				{
					f1 = keys[i];
				}
			}
			
			for (var i = Quaternion.Count - 1; i >= 0; i--)
			{
				if (keys[i] > frame)
				{
					f2 = keys[i];
				}
			}
			
			var diff = f2 != 0 ? (f2 - f1) : NbKeyframes - f1 + keys[0];
			var f2z = f2 != 0 ? f2 : keys[0];

			return RotFromQuat(System.Numerics.Quaternion.Slerp(FloatsAsQuat(Quaternion[f1]), FloatsAsQuat(Quaternion[f2z]), diff * (frame - f1)));
		}

		private static Quaternion FloatsAsQuat(float[] ninjaQuats)
		{
			return new Quaternion(ninjaQuats[1], ninjaQuats[2], ninjaQuats[3], ninjaQuats[0]);
		}

		private static Rotation RotFromQuat(Quaternion quat)
		{
			// Roll (x-axis rotation)
			double sinRollCos = 2 * (quat.W * quat.X + quat.Y * quat.Z);
			double cosRollCos = 1 - 2 * (quat.X * quat.X + quat.Y * quat.Y);
			var x = (float)Math.Atan2(sinRollCos, cosRollCos);

			// Pitch (y-axis rotation)
			double sinPitch = 2 * (quat.W * quat.Y - quat.Z * quat.X);
			float y;
			
			if (Math.Abs(sinPitch) >= 1)
			{
				y = (float)CopySign(Math.PI / 2, sinPitch); // use 90 degrees if out of range
			}
			else
			{
				y = (float)Math.Asin(sinPitch);
			}

			// Yaw (z-axis rotation)
			double sinYawCos = 2 * (quat.W * quat.Z + quat.X * quat.Y);
			double cosYawCos = 1 - 2 * (quat.Y * quat.Y + quat.Z * quat.Z);
			var z = (float)Math.Atan2(sinYawCos, cosYawCos);

			return new Rotation(SAModel.Rotation.RadToBAMS(x), SAModel.Rotation.RadToBAMS(y), SAModel.Rotation.RadToBAMS(z));
		}
		
		private static double CopySign(double valMain, double valSign)
		{
			var final = Math.Abs(valMain);
			
			if (valSign >= 0)
			{
				return final;
			}

			return -final;
		}
	}

	public class Spotlight : IEquatable<Spotlight>
	{
		public float Near { get; set; }
		public float Far { get; set; }
		public int InsideAngle { get; set; }
		public int OutsideAngle { get; set; }
		
		public Spotlight(byte[] file, int address)
		{
			Near = ByteConverter.ToSingle(file, address);
			Far = ByteConverter.ToSingle(file, address + 4);
			InsideAngle = ByteConverter.ToInt32(file, address + 8);
			OutsideAngle = ByteConverter.ToInt32(file, address + 12);
		}

		public byte[] GetBytes()
		{
			var result = new List<byte>(16);
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