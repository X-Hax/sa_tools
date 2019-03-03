using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

namespace SonicRetro.SAModel
{
	[Serializable]
	public class NJS_MESHSET : ICloneable
	{
		public ushort MaterialID { get; set; }
		public Basic_PolyType PolyType { get; private set; }
		public ReadOnlyCollection<Poly> Poly { get; private set; }
		public string PolyName { get; set; }
		public int PAttr { get; set; }
		public Vertex[] PolyNormal { get; private set; }
		public string PolyNormalName { get; set; }
		public Color[] VColor { get; private set; }
		public string VColorName { get; set; }
		public UV[] UV { get; private set; }
		public string UVName { get; set; }

		public static int Size(bool DX)
		{
			return DX ? 0x1C : 0x18;
		}

		public NJS_MESHSET(byte[] file, int address, uint imageBase)
			: this(file, address, imageBase, new Dictionary<int, string>())
		{
		}

		public NJS_MESHSET(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			MaterialID = ByteConverter.ToUInt16(file, address);
			PolyType = (Basic_PolyType)(MaterialID >> 0xE);
			MaterialID &= 0x3FFF;
			Poly[] polys = new Poly[ByteConverter.ToInt16(file, address + 2)];
			int tmpaddr = (int)(ByteConverter.ToUInt32(file, address + 4) - imageBase);
			if (labels.ContainsKey(tmpaddr))
				PolyName = labels[tmpaddr];
			else
				PolyName = "poly_" + tmpaddr.ToString("X8");
			int striptotal = 0;
			for (int i = 0; i < polys.Length; i++)
			{
				polys[i] = SAModel.Poly.CreatePoly(PolyType, file, tmpaddr);
				striptotal += polys[i].Indexes.Length;
				tmpaddr += polys[i].Size;
			}
			Poly = new ReadOnlyCollection<Poly>(polys);
			PAttr = ByteConverter.ToInt32(file, address + 8);
			tmpaddr = ByteConverter.ToInt32(file, address + 0xC);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				if (labels.ContainsKey(tmpaddr))
					PolyNormalName = labels[tmpaddr];
				else
					PolyNormalName = "polynormal_" + tmpaddr.ToString("X8");
				PolyNormal = new Vertex[polys.Length];
				for (int i = 0; i < polys.Length; i++)
				{
					PolyNormal[i] = new Vertex(file, tmpaddr);
					tmpaddr += Vertex.Size;
				}
			}
			else
				PolyNormalName = "polynormal_" + Extensions.GenerateIdentifier();
			tmpaddr = ByteConverter.ToInt32(file, address + 0x10);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				if (labels.ContainsKey(tmpaddr))
					VColorName = labels[tmpaddr];
				else
					VColorName = "vcolor_" + tmpaddr.ToString("X8");
				VColor = new Color[striptotal];
				for (int i = 0; i < striptotal; i++)
				{
					VColor[i] = SAModel.VColor.FromBytes(file, tmpaddr);
					tmpaddr += SAModel.VColor.Size(ColorType.ARGB8888_32);
				}
			}
			else
				VColorName = "vcolor_" + Extensions.GenerateIdentifier();
			tmpaddr = ByteConverter.ToInt32(file, address + 0x14);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				if (labels.ContainsKey(tmpaddr))
					UVName = labels[tmpaddr];
				else
					UVName = "uv_" + tmpaddr.ToString("X8");
				UV = new UV[striptotal];
				for (int i = 0; i < striptotal; i++)
				{
					UV[i] = new UV(file, tmpaddr);
					tmpaddr += SAModel.UV.Size;
				}
			}
			else
				UVName = "uv_" + Extensions.GenerateIdentifier();
		}

		public NJS_MESHSET(Basic_PolyType polyType, int polyCount, bool hasPolyNormal, bool hasUV, bool hasVColor)
		{
			if (polyType == Basic_PolyType.NPoly | polyType == Basic_PolyType.Strips)
			{
				throw new ArgumentException("Cannot create a Poly of that type!\nTry another overload to create Strip-type Polys.", "polyType");
			}
			PolyName = "poly_" + Extensions.GenerateIdentifier();
			PolyType = polyType;
			Poly[] polys = new Poly[polyCount];
			int striptotal = 0;
			for (int i = 0; i < polys.Length; i++)
			{
				polys[i] = SAModel.Poly.CreatePoly(PolyType);
				striptotal += polys[i].Indexes.Length;
			}
			Poly = new ReadOnlyCollection<Poly>(polys);
			if (hasPolyNormal)
			{
				PolyNormalName = "polynormal_" + Extensions.GenerateIdentifier();
				PolyNormal = new Vertex[polys.Length];
				for (int i = 0; i < polys.Length; i++)
					PolyNormal[i] = new Vertex();
			}
			if (hasVColor)
			{
				VColorName = "vcolor_" + Extensions.GenerateIdentifier();
				VColor = new Color[striptotal];
			}
			if (hasUV)
			{
				UVName = "uv_" + Extensions.GenerateIdentifier();
				UV = new UV[striptotal];
				for (int i = 0; i < striptotal; i++)
					UV[i] = new UV();
			}
		}

		public NJS_MESHSET(Poly[] polys, bool hasPolyNormal, bool hasUV, bool hasVColor)
		{
			PolyName = "poly_" + Extensions.GenerateIdentifier();
			PolyType = polys[0].PolyType;
			int striptotal = 0;
			for (int i = 0; i < polys.Length; i++)
				striptotal += polys[i].Indexes.Length;
			Poly = new ReadOnlyCollection<Poly>(polys);
			if (hasVColor)
			{
				VColorName = "vcolor_" + Extensions.GenerateIdentifier();
				VColor = new Color[striptotal];
			}
			if (hasUV)
			{
				UVName = "uv_" + Extensions.GenerateIdentifier();
				UV = new UV[striptotal];
				for (int i = 0; i < striptotal; i++)
					UV[i] = new UV();
			}
		}

		public byte[] GetBytes(uint polyAddress, uint polyNormalAddress, uint vColorAddress, uint uVAddress, bool DX)
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes((ushort)((MaterialID & 0x3FFF) | ((int)PolyType << 0xE))));
			result.AddRange(ByteConverter.GetBytes((ushort)Poly.Count));
			result.AddRange(ByteConverter.GetBytes(polyAddress));
			result.AddRange(ByteConverter.GetBytes(PAttr));
			result.AddRange(ByteConverter.GetBytes(polyNormalAddress));
			result.AddRange(ByteConverter.GetBytes(vColorAddress));
			result.AddRange(ByteConverter.GetBytes(uVAddress));
			if (DX)
				result.AddRange(new byte[4]);
			return result.ToArray();
		}

		public string ToStruct(bool DX)
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append((StructEnums.NJD_MESHSET)((int)PolyType << 0xE));
			result.Append(" | ");
			result.Append(MaterialID & 0x3FFF);
			result.Append(", ");
			result.Append(Poly != null ? (ushort)Poly.Count : 0);
			result.Append(", ");
			result.Append(Poly != null ? PolyName : "NULL");
			result.Append(", NULL, ");
			result.Append(PolyNormal != null ? PolyNormalName : "NULL");
			result.Append(", ");
			result.Append(VColor != null ? VColorName : "NULL");
			result.Append(", ");
			result.Append(UV != null ? UVName : "NULL");
			if (DX)
				result.Append(", NULL");
			result.Append(" }");
			return result.ToString();
		}
		
		public string ToNJA(bool DX)
		{
			StringBuilder result = new StringBuilder("MESHSTART ");
			result.Append("TypeMatId ( " + (StructEnums.NJD_MESHSET)((int)PolyType << 0xE) + ",");
			result.Append(MaterialID & 0x3FFF);
			result.Append("), ");
			result.Append("MeshNum ");
			result.Append(Poly != null ? (ushort)Poly.Count : 0);
			result.Append(", ");
			result.Append("Meshes ");
			result.Append(Poly != null ? PolyName : "NULL");
			result.Append(", ");
			result.Append("PolyAttrs NULL, ");
			result.Append("PolyNormal ");
			result.Append(PolyNormal != null ? PolyNormalName : "NULL");
			result.Append(", ");
			result.Append("VertColor ");
			result.Append(VColor != null ? VColorName : "NULL");
			result.Append(", ");
			result.Append("VertUV ");
			result.Append(UV != null ? UVName : "NULL");
			if (DX)
				result.Append(", NULL");
			result.Append(" MESHEND");
			return result.ToString();
		}

		object ICloneable.Clone() => Clone();

		public NJS_MESHSET Clone()
		{
			NJS_MESHSET result = (NJS_MESHSET)MemberwiseClone();
			List<Poly> polys = new List<Poly>(Poly.Count);
			foreach (Poly item in Poly)
				polys.Add(item.Clone());
			result.Poly = new ReadOnlyCollection<Poly>(polys);
			if (PolyNormal != null)
			{
				result.PolyNormal = new Vertex[PolyNormal.Length];
				for (int i = 0; i < PolyNormal.Length; i++)
					result.PolyNormal[i] = PolyNormal[i].Clone();
			}
			if (VColor != null)
				result.VColor = (Color[])VColor.Clone();
			if (UV != null)
			{
				result.UV = new UV[UV.Length];
				for (int i = 0; i < UV.Length; i++)
					result.UV[i] = UV[i].Clone();
			}
			return result;
		}
	}
}