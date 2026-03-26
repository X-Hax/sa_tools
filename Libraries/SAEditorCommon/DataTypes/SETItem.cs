using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.SETEditing;
using SAModel.SAEditorCommon.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using SAModel.Direct3D.TextureSystem;
using SAModel.SAEditorCommon.Import;
using System.Linq;

namespace SAModel.SAEditorCommon.DataTypes
{
	[Serializable]
	public class SETItem : Item, ICustomTypeDescriptor, IScaleable
	{
		[Description("Coordinates and radius of a sphere used to determine whether the item should be rendered. Calculated automatically for SET items.")]
		public override BoundingSphere Bounds { get { return objdef.GetBounds(this); } }

		protected ObjectDefinition objdef;
		/// <summary>
		/// For use by <see cref="MissionSETItem"/>.
		/// </summary>
		protected SETItem(EditorItemSelection selectionManager)
			: base(selectionManager)
		{ }

		public SETItem(ushort id, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			ID = id;
			objdef = GetObjectDefinition();
			position = new Vertex();
			rotation = new Rotation(objdef.DefaultXRotation, objdef.DefaultYRotation, objdef.DefaultZRotation);
			Scale = new Vertex(objdef.DefaultXScale, objdef.DefaultYScale, objdef.DefaultZScale);
			isLoaded = true;

			GetHandleMatrix();
		}

		public SETItem(string filename, byte[] file, int address, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			string nopathfname = Path.GetFileName(filename);
			ushort _id = ByteConverter.ToUInt16(file, address);
			ID = _id;
			ClipLevel = (byte)((_id >> 12) & 0xF);
			ushort xrot = ByteConverter.ToUInt16(file, address + 2);
			ushort yrot = ByteConverter.ToUInt16(file, address + 4);
			ushort zrot = ByteConverter.ToUInt16(file, address + 6);
			rotation = new Rotation(xrot, yrot, zrot);
			position = new Vertex(file, address + 8);
			Scale = new Vertex(file, address + 0x14);
			isLoaded = true;
			objdef = GetObjectDefinition();
			SETFileName = nopathfname;

			GetHandleMatrix();
		}

		public virtual ObjectDefinition GetObjectDefinition()
		{
			if (id < LevelData.ObjDefs.Count)
				return LevelData.ObjDefs[id];
			else
				return DefaultObjectDefinition.DefaultInstance;
		}

		[Category("Common"), DisplayName("Description"), Description("The item's description in its definition."), ParenthesizePropertyName(true)]
		public string Name { get { return objdef.Name; } }
		[Category("Common"), DisplayName ("File Source"), Description("The SET file that stores data for this item."), ParenthesizePropertyName(true)]
		public string SETFileName { get; set; }
		[Category("Data"), Description("The item's internal name in the level object list."), ParenthesizePropertyName(true)]
		public string InternalName { get { return objdef.InternalName; } }
		protected bool isLoaded = false;

		protected ushort id;
		[Category("Data"), Description("The ID of the item type in the level object list."), Editor(typeof(IDEditor), typeof(UITypeEditor))]
		public ushort ID
		{
			get { return id; }
			set
			{
				id = (ushort)(value & 0xFFF);
				if (isLoaded)
					objdef = GetObjectDefinition();
			}
		}

		private ushort cliplevel;
		[Browsable(false)]
		public ushort ClipLevel
		{
			get { return cliplevel; }
			set { cliplevel = (byte)(value & 0xF); }
		}

		[Category("Data"), DisplayName("Clip Level"), Description("Game detail setting required to display the object. In SADX, the \"PlayerOrUnsubstantive\" flag is \"player dependent set\" flag (unused). In SA2, it is used by all objects in \"_u\" SET files.")]
		public ClipSetting ClipSetting
		{
			get { return (ClipSetting)ClipLevel; }
			set { ClipLevel = (ushort)value; }
		}

		[Category("Common"), Description("Initial position of the item.")]
		public override Vertex Position { get { return position; } set { position = value; GetHandleMatrix(); } }
		[Category("Common"), Description("Initial rotation of the item. Depending on the object, some or all axes may be ignored or used for other purposes.")]
		public override Rotation Rotation { get { return rotation; } set { rotation = value; GetHandleMatrix(); } }
		new protected Vertex scale = new Vertex();
		[Category("Common"), Description("Depending on the object, these values can define how big the item is, adjust its parameters or pick a variation of the object.")]
		public Vertex Scale { get { return scale; } set { scale = value; GetHandleMatrix(); } }

		[Browsable(true)]
		[DisplayName("Export Model")]
		public void ExportModel()
		{
			try
			{
				// Get texture names
				string[] textureNames = null;
				BMPInfo[] textures = objdef.ExportTextures(this);
				if (textures != null)
				{
					textureNames = new string[textures.Length];
					for (int i = 0; i < textures.Length; i++)
						textureNames[i] = textures[i].Name;
				}
				// Transform
				SAModel.Direct3D.MatrixStack transform = new SAModel.Direct3D.MatrixStack();
				List<ModelTransform> objs = objdef.GetModels(this, transform);
				// No models
				if (objs == null || objs.Count == 0)
					return;
				string filePath = ModelImportExport.ExportModelDialog(objs[0].Model, objdef.InternalName);
				// Dialog cancelled
				if (string.IsNullOrEmpty(filePath))
					return;
				// Export first model
				//objs[0].Model.ProcessTransforms(objs[0].Transform);
				ModelImportExport.ExportModel(objs[0].Model, filePath, objs[0].Transform, textureNames, objdef.Name);
				// Texture out path
				string outPath = Path.GetDirectoryName(filePath);
				// Exporting multiple models
				if (objs.Count > 1)
				{
					for (int i = 1; i < objs.Count; i++)
					{
						//objs[i].Model.ProcessTransforms(objs[i].Transform);
						// Set output path to "path\model_1.samdl" etc.
						string modelOutPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_" + i.ToString() + Path.GetExtension(filePath));
						ModelImportExport.ExportModel(objs[i].Model, modelOutPath, objs[i].Transform, textureNames, objdef.Name);
					}
				}
				// Save textures for non-SAModel formats
				// File not saved
				if (string.IsNullOrEmpty(outPath))
					return;
				// No textures
				if (textures == null)
					return;
				// Don't export textures for SAModel formats
				if (Path.GetExtension(filePath).Contains("mdl", StringComparison.InvariantCultureIgnoreCase))
					return;
				// Don't export textures for GJ
				if (Path.GetExtension(filePath).Equals(".gj", StringComparison.InvariantCultureIgnoreCase))
					return;
				// Don't export textures for NJ
				if (Path.GetExtension(filePath).Equals(".nj", StringComparison.InvariantCultureIgnoreCase))
					return;
				// Don't export textures for XJ
				if (Path.GetExtension(filePath).Equals(".xj", StringComparison.InvariantCultureIgnoreCase))
					return;
				// Make a list of used texture IDs
				List<int> usedTextureIDs = new List<int>();
				for (int i = 0; i < objs.Count; i++)
					usedTextureIDs.AddRange(objs[i].Model.GetUsedTextureIDs());
				usedTextureIDs = usedTextureIDs.Distinct().ToList();
				// Export used texture bitmaps
				for (int t = 0; t < textures.Length; t++)
					if (usedTextureIDs.Contains(t))
						textures[t].Image.Save(Path.Combine(outPath, textures[t].Name + ".png"));
			}
			catch (Exception ex)
			{
				Logger.Add("Exporting model failed: " + ex.ToString());
			}
		}

		public Vertex GetScale()
		{
			return Scale;
		}

		public void SetScale(Vertex scale)
		{
			this.Scale = scale;
		}

		protected override void GetHandleMatrix()
		{
			transformMatrix = GetObjectDefinition().GetHandleMatrix(this);
		}

		public override void Paste()
		{
			LevelData.AddSETItem(LevelData.Character, this);
		}

		protected override void DeleteInternal(EditorItemSelection selectionManager)
		{
			LevelData.RemoveSETItem(LevelData.Character, this);
		}

		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
		{
			return objdef.CheckHit(this, Near, Far, Viewport, Projection, View, new MatrixStack());
		}

		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (!camera.SphereInFrustum(Bounds))
				return EmptyRenderInfo;

			return objdef.Render(this, dev, camera, transform);
		}

		public void SetOrientation(Vertex direction)
		{
			objdef.SetOrientation(this, direction);
		}

		public void PointTo(Vertex location)
		{
			objdef.PointTo(this, location);
		}

		public byte[] GetBytes()
		{
			List<byte> bytes = new List<byte>(0x20);
			bytes.AddRange(ByteConverter.GetBytes((ushort)(id | (cliplevel << 12))));
			unchecked
			{
				bytes.AddRange(ByteConverter.GetBytes((ushort)Rotation.X));
				bytes.AddRange(ByteConverter.GetBytes((ushort)Rotation.Y));
				bytes.AddRange(ByteConverter.GetBytes((ushort)Rotation.Z));
			}
			bytes.AddRange(Position.GetBytes());
			bytes.AddRange(Scale.GetBytes());
			return bytes.ToArray();
		}

		public static List<SETItem> Load(string filename, EditorItemSelection selectionManager) => Load(filename, System.IO.File.ReadAllBytes(filename), selectionManager);

		public static List<SETItem> Load(string filename, byte[] setfile, EditorItemSelection selectionManager)
		{
			bool bigendianbk = ByteConverter.BigEndian;
			string nopathfname = Path.GetFileName(filename);
			// Load the value as both Little and Big Endian and compare the result.
			// If the BE number is larger, this is an LE file.
			ByteConverter.BigEndian = false;
			uint test_le = ByteConverter.ToUInt32(setfile, 0);
			ByteConverter.BigEndian = true;
			uint test_be = ByteConverter.ToUInt32(setfile, 0);
			if (test_be > test_le)
			{
				ByteConverter.BigEndian = false;
			}
			int count = ByteConverter.ToInt32(setfile, 0);
			List<SETItem> list = new List<SETItem>(count);
			int address = 0x20;
			for (int j = 0; j < count; j++)
			{
				SETItem ent = new SETItem(nopathfname, setfile, address, selectionManager);
				list.Add(ent);
				address += 0x20;
			}
			ByteConverter.BigEndian = bigendianbk;
			return list;
		}

		public static void Save(List<SETItem> items, string filename, bool bigendian = false) => System.IO.File.WriteAllBytes(filename, Save(items, bigendian));

		public static byte[] Save(List<SETItem> items, bool bigendian = false)
		{
			List<byte> file = new List<byte>(items.Count * 0x20 + 0x20);
			bool bigendianbk = ByteConverter.BigEndian;
			ByteConverter.BigEndian = bigendian;
			file.AddRange(ByteConverter.GetBytes(items.Count));
			file.Align(0x20);

			foreach (SETItem item in items)
				file.AddRange(item.GetBytes());
			ByteConverter.BigEndian = bigendianbk;
			return file.ToArray();
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection result = TypeDescriptor.GetProperties(this, attributes, true);

			List<PropertyDescriptor> props = new List<PropertyDescriptor>(result.Count);
			foreach (PropertyDescriptor item in result)
				props.Add(item);

			if (objdef.CustomProperties != null)
				foreach (PropertySpec property in objdef.CustomProperties)
				{
					List<Attribute> attrs = new List<Attribute>();

					// Additionally, append the custom attributes associated with the
					// PropertySpec, if any.
					if (property.Attributes != null)
						attrs.AddRange(property.Attributes);

					// Create a new property descriptor for the property item, and add
					// it to the list.
					PropertySpecDescriptor pd = new PropertySpecDescriptor(property,
						property.Name, attrs.ToArray());
					props.Add(pd);
				}

			if (this is MissionSETItem && objdef.MissionProperties != null)
				foreach (PropertySpec property in objdef.MissionProperties)
				{
					List<Attribute> attrs = new List<Attribute>();

					// Additionally, append the custom attributes associated with the
					// PropertySpec, if any.
					if (property.Attributes != null)
						attrs.AddRange(property.Attributes);

					// Create a new property descriptor for the property item, and add
					// it to the list.
					PropertySpecDescriptor pd = new PropertySpecDescriptor(property,
						property.Name, attrs.ToArray());
					props.Add(pd);
				}

			return new PropertyDescriptorCollection(props.ToArray(), true);
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
	}

	public enum ClipSetting : ushort
	{
		/// <summary>The object will appear in all Clip Level settings.</summary>
		All,
		/// <summary>The object will only appear in High (Far) Clip Level settings.</summary>
		HighOnly,
		/// <summary>The object will appear in High (Far) and Medium Clip Level settings.</summary>
		MediumAndHigh,
		/// <summary>
		/// SADX: The object sets the "player dependent set" flag (unused).
		/// SA2: Additional objects in '_u' SET files.
		/// </summary>
		PlayerOrUnsubstantive = 8,
	}
}
