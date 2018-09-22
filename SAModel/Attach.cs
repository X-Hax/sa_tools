using System;
using System.Collections.Generic;
using System.IO;

namespace SonicRetro.SAModel
{
	[Serializable]
	public abstract class Attach : ICloneable
	{
		public string Name { get; set; }
		public BoundingSphere Bounds { get; set; }
		[NonSerialized]
		private MeshInfo[] meshInfo;

		public MeshInfo[] MeshInfo
		{
			get { return meshInfo; }
			set { meshInfo = value; }
		}

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
			}
			throw new ArgumentOutOfRangeException(nameof(format));
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
			}
			throw new ArgumentOutOfRangeException(nameof(format));
		}

		public abstract byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address);

		public byte[] GetBytes(uint imageBase, bool DX, out uint address)
		{
			return GetBytes(imageBase, DX, new Dictionary<string, uint>(), out address);
		}

		public byte[] GetBytes(uint imageBase, bool DX)
		{
			return GetBytes(imageBase, DX, out uint address);
		}

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

		public abstract void ProcessShapeMotionVertexData(NJS_MOTION motion, int frame, int animindex);

		public abstract BasicAttach ToBasicModel();

		public abstract ChunkAttach ToChunkModel();

		object ICloneable.Clone() => Clone();

		public abstract Attach Clone();
	}
}