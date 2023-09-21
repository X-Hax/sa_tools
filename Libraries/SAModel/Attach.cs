using System;
using System.Collections.Generic;
using System.IO;
using SAModel.GC;
using SAModel.XJ;

namespace SAModel
{
	[Serializable]
	public abstract class Attach : ICloneable
	{
		public string Name { get; set; }
		public BoundingSphere Bounds { get; set; }
		public Dictionary<int, List<VertexWeight>> VertexWeights { get; set; }
		[NonSerialized]
		private MeshInfo[] meshInfo;

		public MeshInfo[] MeshInfo
		{
			get { return meshInfo; }
			set { meshInfo = value; }
		}

		public virtual bool HasWeight => VertexWeights != null;

		public static int Size(ModelFormat format)
		{
			switch (format)
			{
				case ModelFormat.Basic:
					return 0x28;
				case ModelFormat.BasicDX:
					return 0x2C;
				case ModelFormat.Chunk:
					return 0x18;
				case ModelFormat.GC:
					return 0x24;
			}
			return -1;
		}

		public static Attach Load(ModelFormat format)
		{
			switch (format)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					return new BasicAttach();
				case ModelFormat.Chunk:
					return new ChunkAttach();
				case ModelFormat.GC:
					return new GCAttach();
			}
			throw new ArgumentOutOfRangeException("format");
		}

		public static Attach Load(byte[] file, int address, uint imageBase, ModelFormat format)
		{
			return Load(file, address, imageBase, format, new Dictionary<int, string>());
		}

		public static Attach Load(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
		{
			switch (format)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					return new BasicAttach(file, address, imageBase, format == ModelFormat.BasicDX, labels);
				case ModelFormat.Chunk:
					return new ChunkAttach(file, address, imageBase, labels);
				case ModelFormat.GC:
					return new GCAttach(file, address, imageBase, labels);
				case ModelFormat.XJ:
					return new XJAttach(file, address, imageBase, labels);
			}
			throw new ArgumentOutOfRangeException("format");
		}

		public abstract byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, List<uint> njOffsets, out uint address);

		public abstract string ToStruct(bool DX);

		public abstract void ToStructVariables(TextWriter writer, bool DX, List<string> labels, string[] textures = null);

		public string ToStructVariables(bool DX, List<string> labels, string[] textures = null)
		{
			using (StringWriter sw = new StringWriter())
			{
				ToStructVariables(sw, DX, labels, textures);
				return sw.ToString();
			}
		}

		public abstract void ProcessVertexData();

		public abstract void ProcessShapeMotionVertexData(NJS_MOTION motion, float frame, int animindex);

		object ICloneable.Clone() => Clone();

		public abstract Attach Clone();
	}
}