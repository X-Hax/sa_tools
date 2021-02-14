using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;

namespace SonicRetro.SAModel
{
	[Serializable]
	public class NJS_OBJECT : ICloneable
	{
		[Browsable(false)]
		public Attach Attach { get; set; }

		public Vertex Position { get; set; }
		public Rotation Rotation { get; set; }
		public Vertex Scale { get; set; }

		[Browsable(false)]
		public NJS_OBJECT Parent { get; private set; }

		private List<NJS_OBJECT> children;
		[Browsable(false)]
		public ReadOnlyCollection<NJS_OBJECT> Children { get; private set; }

		[Browsable(false)]
		public NJS_OBJECT Sibling { get; private set; }

		public string Name { get; set; }

		public bool IgnorePosition { get; set; }

		public bool IgnoreRotation { get; set; }

		public bool IgnoreScale { get; set; }

		public bool RotateZYX { get; set; }

		public bool SkipDraw { get; set; }

		public bool SkipChildren { get; set; }

		[DefaultValue(true)]
		public bool Animate { get; set; }

		[DefaultValue(true)]
		public bool Morph { get; set; }

		public static int Size
		{
			get { return 0x34; }
		}

		[Browsable(false)]
		public bool HasWeight
		{
			get
			{
				if (Attach is ChunkAttach ca && ca.HasWeight) return true;
				foreach (NJS_OBJECT child in Children)
					if (child.HasWeight)
						return true;
				if (Parent == null && Sibling != null && Sibling.HasWeight)
					return true;
				return false;
			}
		}

		[DisplayName("Code Path")]
		[Description("The syntax to access this node from the root node in C++ code.")]
		public string CodePath
		{
			get
			{
				if (Parent == null)
					return "root";
				StringBuilder result = new StringBuilder("->child");
				int idx = Parent.Children.IndexOf(this);
				for (int i = 0; i < idx; i++)
					result.Append("->sibling");
				return Parent.CodePath + result.ToString();
			}
		}

		public NJS_OBJECT()
		{
			Name = "object_" + Extensions.GenerateIdentifier();
			Position = new Vertex();
			Rotation = new Rotation();
			Scale = new Vertex(1, 1, 1);
			children = new List<NJS_OBJECT>();
			Children = new ReadOnlyCollection<NJS_OBJECT>(children);
		}

		public NJS_OBJECT(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, Attach> attaches)
			: this(file, address, imageBase, format, new Dictionary<int, string>(), attaches)
		{ }

		public NJS_OBJECT(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels, Dictionary<int, Attach> attaches)
			: this(file, address, imageBase, format, null, labels, attaches)
		{ }

		private NJS_OBJECT(byte[] file, int address, uint imageBase, ModelFormat format, NJS_OBJECT parent, Dictionary<int, string> labels, Dictionary<int, Attach> attaches)
		{
			if (labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "object_" + address.ToString("X8");
			if (address > file.Length - 52)
			{
				Position = new Vertex();
				Rotation = new Rotation();
				Scale = new Vertex(1, 1, 1);
				children = new List<NJS_OBJECT>();
				Children = new ReadOnlyCollection<NJS_OBJECT>(children);
				return;
			}
			ObjectFlags flags = (ObjectFlags)ByteConverter.ToInt32(file, address);
			RotateZYX = (flags & ObjectFlags.RotateZYX) == ObjectFlags.RotateZYX;
			SkipDraw = (flags & ObjectFlags.NoDisplay) == ObjectFlags.NoDisplay;
			SkipChildren = (flags & ObjectFlags.NoChildren) == ObjectFlags.NoChildren;
			IgnorePosition = (flags & ObjectFlags.NoPosition) == ObjectFlags.NoPosition;
			IgnoreRotation = (flags & ObjectFlags.NoRotate) == ObjectFlags.NoRotate;
			IgnoreScale = (flags & ObjectFlags.NoScale) == ObjectFlags.NoScale;
			Animate = (flags & ObjectFlags.NoAnimate) == 0;
			Morph = (flags & ObjectFlags.NoMorph) == 0;
			int tmpaddr = ByteConverter.ToInt32(file, address + 4);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				if(attaches != null && attaches.ContainsKey(tmpaddr))
				{
					Attach = attaches[tmpaddr];
				}
				else
				{
					Attach = Attach.Load(file, tmpaddr, imageBase, format, labels);
					if (attaches != null)
						attaches.Add(tmpaddr, Attach);
				}
				
			}
			Position = new Vertex(file, address + 8);
			Rotation = new Rotation(file, address + 0x14);
			Scale = new Vertex(file, address + 0x20);
			Parent = parent;
			children = new List<NJS_OBJECT>();
			Children = new ReadOnlyCollection<NJS_OBJECT>(children);
			NJS_OBJECT child = null;
			tmpaddr = ByteConverter.ToInt32(file, address + 0x2C);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				child = new NJS_OBJECT(file, tmpaddr, imageBase, format, this, labels, attaches);
			}
			while (child != null)
			{
				children.Add(child);
				child = child.Sibling;
			}
			tmpaddr = ByteConverter.ToInt32(file, address + 0x30);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				Sibling = new NJS_OBJECT(file, tmpaddr, imageBase, format, parent, labels, attaches);
			}

			//Assimp.AssimpContext context = new AssimpContext();
			//Scene scene = context.ImportFile("F:\\untitled.obj", PostProcessSteps.Triangulate);
			//AssimpLoad(scene, scene.RootNode);
		}

		public byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
		{
			FixSiblings();
			List<byte> result = new List<byte>();
			uint childaddr = 0;
			uint siblingaddr = 0;
			uint attachaddr = 0;
			byte[] tmpbyte;
			if (Children.Count > 0)
			{
				if (labels.ContainsKey(Children[0].Name))
					childaddr = labels[Children[0].Name];
				else
				{
					result.Align(4);
					result.AddRange(Children[0].GetBytes(imageBase, DX, labels, out childaddr));
					childaddr += imageBase;
				}
			}
			if (Sibling != null)
			{
				if (labels.ContainsKey(Sibling.Name))
					siblingaddr = labels[Sibling.Name];
				else
				{
					result.Align(4);
					tmpbyte = Sibling.GetBytes(imageBase + (uint)result.Count, DX, labels, out siblingaddr);
					siblingaddr += imageBase + (uint)result.Count;
					result.AddRange(tmpbyte);
				}
			}
			if (Attach != null)
			{
				if (labels.ContainsKey(Attach.Name))
					attachaddr = labels[Attach.Name];
				else
				{
					result.Align(4);
					tmpbyte = Attach.GetBytes(imageBase + (uint)result.Count, DX, labels, out attachaddr);
					attachaddr += imageBase + (uint)result.Count;
					result.AddRange(tmpbyte);
				}
			}
			result.Align(4);
			address = (uint)result.Count;
			ObjectFlags flags = GetFlags();
			result.AddRange(ByteConverter.GetBytes((int)flags));
			result.AddRange(ByteConverter.GetBytes(attachaddr));
			result.AddRange(Position.GetBytes());
			result.AddRange(Rotation.GetBytes());
			result.AddRange(Scale.GetBytes());
			result.AddRange(ByteConverter.GetBytes(childaddr));
			result.AddRange(ByteConverter.GetBytes(siblingaddr));
			labels.Add(Name, address + imageBase);
			return result.ToArray();
		}

		public void FixSiblings()
		{
			for (int i = 1; i < Children.Count; i++)
				Children[i - 1].Sibling = Children[i];
		}

		public ModelFormat GetModelFormat()
		{
			// BasicAttach has no internal distinction between Basic and BasicDX
			ModelFormat result = ModelFormat.BasicDX;
			if (Attach is ChunkAttach) result = ModelFormat.Chunk;
			else if (Attach is GC.GCAttach) result = ModelFormat.GC;
			return result;
		}

		public ObjectFlags GetFlags()
		{
			ObjectFlags flags = 0;
			if (IgnorePosition || Position.IsEmpty)
				flags = ObjectFlags.NoPosition;
			if (IgnoreRotation || Rotation.IsEmpty)
				flags |= ObjectFlags.NoRotate;
			if (IgnoreScale || (Scale.X == 1 && Scale.Y == 1 && Scale.Z == 1))
				flags |= ObjectFlags.NoScale;
			if (Attach == null || SkipDraw)
				flags |= ObjectFlags.NoDisplay;
			if (Children.Count == 0 || SkipChildren)
				flags |= ObjectFlags.NoChildren;
			if (RotateZYX)
				flags |= ObjectFlags.RotateZYX;
			if (!Animate)
				flags |= ObjectFlags.NoAnimate;
			if (!Morph)
				flags |= ObjectFlags.NoMorph;
			return flags;
		}

		public byte[] GetBytes(uint imageBase, bool DX, out uint address)
		{
			return GetBytes(imageBase, DX, new Dictionary<string, uint>(), out address);
		}

		public byte[] GetBytes(uint imageBase, bool DX)
		{
			return GetBytes(imageBase, DX, out uint address);
		}

		public NJS_OBJECT[] GetObjects()
		{
			List<NJS_OBJECT> result = new List<NJS_OBJECT> { this };
			foreach (NJS_OBJECT item in Children)
				result.AddRange(item.GetObjects());
			if (Parent == null && Sibling != null)
				result.AddRange(Sibling.GetObjects());
			return result.ToArray();
		}

		public int CountAnimated()
		{
			int result = Animate ? 1 : 0;
			foreach (NJS_OBJECT item in Children)
				result += item.CountAnimated();
			if (Parent == null && Sibling != null)
				result += Sibling.CountAnimated();
			return result;
		}

		public int CountMorph()
		{
			int result = Morph ? 1 : 0;
			foreach (NJS_OBJECT item in Children)
				result += item.CountMorph();
			if (Parent == null && Sibling != null)
				result += Sibling.CountMorph();
			return result;
		}

		public void AddChild(NJS_OBJECT child)
		{
			children.Add(child);
			child.Parent = this;
			if (child.Sibling != null) AddChild(child.Sibling);
		}

		public void AddChildren(IEnumerable<NJS_OBJECT> children)
		{
			foreach (NJS_OBJECT child in children)
				AddChild(child);
		}

		public void InsertChild(int index, NJS_OBJECT child)
		{
			children.Insert(index, child);
			child.Parent = this;
		}

		public void RemoveChild(NJS_OBJECT child)
		{
			children.Remove(child);
			child.Parent = null;
		}

		public void RemoveChildAt(int index)
		{
			NJS_OBJECT child = children[index];
			children.RemoveAt(index);
			child.Parent = null;
		}

		public void ClearChildren()
		{
			foreach (NJS_OBJECT child in children)
				child.Parent = null;
			children.Clear();
		}

		public void ProcessVertexData()
		{
#if modellog
			Extensions.Log("Processing Object " + Name + Environment.NewLine);
#endif
			if (Attach != null)
				Attach.ProcessVertexData();
			foreach (NJS_OBJECT item in Children)
				item.ProcessVertexData();
			if (Parent == null && Sibling != null)
				Sibling.ProcessVertexData();
		}

		public void ProcessShapeMotionVertexData(NJS_MOTION motion, int frame)
		{
			int animindex = -1;
			NJS_OBJECT obj = this;
			do
			{
				obj.ProcessShapeMotionVertexData(motion, frame, ref animindex);
				obj = obj.Sibling;
			} while (obj != null);
		}

		private void ProcessShapeMotionVertexData(NJS_MOTION motion, int frame, ref int animindex)
		{
			if (Morph)
				animindex++;
			if (Attach != null)
				Attach.ProcessShapeMotionVertexData(motion, frame, animindex);
			foreach (NJS_OBJECT item in Children)
				item.ProcessShapeMotionVertexData(motion, frame, ref animindex);
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(((StructEnums.NJD_EVAL)GetFlags()).ToString().Replace(", ", " | "));
			result.Append(", ");
			result.Append(Attach != null ? "&" + Attach.Name : "NULL");
			foreach (float value in Position.ToArray())
			{
				result.Append(", ");
				result.Append(value.ToC());
			}
			foreach (int value in Rotation.ToArray())
			{
				result.Append(", ");
				result.Append(value.ToCHex());
			}
			foreach (float value in Scale.ToArray())
			{
				result.Append(", ");
				result.Append(value.ToC());
			}
			result.Append(", ");
			result.Append(Children.Count > 0 ? "&" + Children[0].Name : "NULL");
			result.Append(", ");
			result.Append(Sibling != null ? "&" + Sibling.Name : "NULL");
			result.Append(" }");
			return result.ToString();
		}

		public void ToNJA(TextWriter writer, bool DX, List<string> labels, string[] textures = null)
		{
			for (int i = 1; i < Children.Count; i++)
				Children[i - 1].Sibling = Children[i];
			for (int i = Children.Count - 1; i >= 0; i--)
			{
				if (!labels.Contains(Children[i].Name))
				{
					labels.Add(Children[i].Name);
					Children[i].ToNJA(writer, DX, labels, textures);
					writer.WriteLine();
				}
			}
			if (Parent == null && Sibling != null && !labels.Contains(Sibling.Name))
			{
				labels.Add(Sibling.Name);
				Sibling.ToNJA(writer, DX, labels, textures);
				writer.WriteLine();
			}
			writer.WriteLine("OBJECT_START" + Environment.NewLine);
			if (Attach is BasicAttach)
			{
				BasicAttach basicattach = Attach as BasicAttach;
				basicattach.ToNJA(writer, DX, labels, textures);
			}
			else if (Attach is ChunkAttach)
			{
				//ChunkAttach ChunkAttach = Attach as ChunkAttach;
				//ChunkAttach.ToNJA(writer, labels, textures);
			}

			writer.Write("OBJECT ");
			writer.Write(Name);
			writer.WriteLine("[]");
			writer.WriteLine("START");
			writer.WriteLine("EvalFlags ( " + ((StructEnums.NJD_EVAL)GetFlags()).ToString().Replace(", ", " | ") + " ),");
			writer.WriteLine("Model " + (Attach != null ? Attach.Name : "NULL") + ",");
			writer.WriteLine("OPosition ( " + Position.X.ToC() + ", " + Position.Y.ToC() + ", " + Position.Z.ToC()+" ),");
			writer.WriteLine("OAngle ( " + ((float)Rotation.X / 182.044f).ToC() + ", " + ((float)Rotation.Y / 182.044f).ToC() + ", " + ((float)Rotation.Z / 182.044f).ToC() + " ),");
			writer.WriteLine("OScale ( " + Scale.X.ToC() + ", " + Scale.Y.ToC() + ", "+ Scale.Z.ToC() + " ),");
			writer.WriteLine("Child " + (Children.Count > 0 ? Children[0].Name : "NULL") + ",");
			writer.WriteLine("Sibling " + (Sibling != null ? Sibling.Name : "NULL"));
			writer.WriteLine("END" + Environment.NewLine);
			writer.WriteLine("OBJECT_END");
			if (Parent == null)
			{
				writer.WriteLine(Environment.NewLine + "DEFAULT_START");
				writer.WriteLine(Environment.NewLine + "#ifndef DEFAULT_OBJECT_NAME");
				writer.WriteLine("#define DEFAULT_OBJECT_NAME " + Name);
				writer.WriteLine("#endif");
				writer.WriteLine(Environment.NewLine + "DEFAULT_END");
			}
		}

		public void ToStructVariables(TextWriter writer, bool DX, List<string> labels, string[] textures = null)
		{
			for (int i = 1; i < Children.Count; i++)
				Children[i - 1].Sibling = Children[i];
			for (int i = Children.Count - 1; i >= 0; i--)
			{
				if (!labels.Contains(Children[i].Name))
				{
					labels.Add(Children[i].Name);
					Children[i].ToStructVariables(writer, DX, labels, textures);
					writer.WriteLine();
				}
			}
			if (Parent == null && Sibling != null && !labels.Contains(Sibling.Name))
			{
				labels.Add(Sibling.Name);
				Sibling.ToStructVariables(writer, DX, labels, textures);
				writer.WriteLine();
			}
			if (Attach != null && !labels.Contains(Attach.Name))
			{
				labels.Add(Attach.Name);
				Attach.ToStructVariables(writer, DX, labels, textures);
				writer.WriteLine();
			}
			writer.Write("NJS_OBJECT ");
			writer.Write(Name);
			writer.Write(" = ");
			writer.Write(ToStruct());
			writer.WriteLine(";");
		}

		public string ToStructVariables(bool DX, List<string> labels, string[] textures = null)
		{
			using (StringWriter sw = new StringWriter())
			{
				ToStructVariables(sw, DX, labels, textures);
				return sw.ToString();
			}
		}

		static readonly List<PolyChunk>[] PolyCache = new List<PolyChunk>[255];
		public void StripPolyCache()
		{
			if (Attach is ChunkAttach attach && attach.Poly != null)
			{
				for (int i = 0; i < attach.Poly.Count; i++)
				{
					switch (attach.Poly[i].Type)
					{
						case ChunkType.Bits_CachePolygonList:
							PolyCache[((PolyChunkBitsCachePolygonList)attach.Poly[i]).List] = attach.Poly.Skip(i + 1).ToList();
							attach.Poly = attach.Poly.Take(i).ToList();
							break;
						case ChunkType.Bits_DrawPolygonList:
							int list = ((PolyChunkBitsDrawPolygonList)attach.Poly[i]).List;
							attach.Poly.RemoveAt(i);
							attach.Poly.InsertRange(i--, PolyCache[list]);
							break;
					}
				}
				if (attach.Poly.Count == 0)
				{
					attach.Poly = null;
					attach.PolyName = null;
				}
			}
			foreach (NJS_OBJECT child in Children)
				child.StripPolyCache();
			if (Parent == null && Sibling != null)
				Sibling.StripPolyCache();
		}

		object ICloneable.Clone() => Clone();

		public NJS_OBJECT Clone()
		{
			NJS_OBJECT result = (NJS_OBJECT)MemberwiseClone();
			if (Attach != null)
				result.Attach = Attach.Clone();
			result.Position = Position.Clone();
			result.Rotation = Rotation.Clone();
			result.Scale = Scale.Clone();
			result.children = new List<NJS_OBJECT>(children.Count);
			result.Children = new ReadOnlyCollection<NJS_OBJECT>(result.children);
			if (children.Count > 0)
			{
				NJS_OBJECT child = children[0].Clone();
				while (child != null)
				{
					result.children.Add(child);
					child = child.Sibling;
				}
			}
			if (Sibling != null)
				result.Sibling = Sibling.Clone();
			return result;
		}
	}
}
