using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
	[Serializable]
	public class SETItem : Item, ICustomTypeDescriptor, IScaleable
	{
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

		public SETItem(byte[] file, int address, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			ushort _id = ByteConverter.ToUInt16(file, address);
			ID = _id;
			ClipLevel = (byte)(_id >> 12);
			ushort xrot = BitConverter.ToUInt16(file, address + 2);
			ushort yrot = BitConverter.ToUInt16(file, address + 4);
			ushort zrot = BitConverter.ToUInt16(file, address + 6);
			rotation = new Rotation(xrot, yrot, zrot);
			position = new Vertex(file, address + 8);
			Scale = new Vertex(file, address + 0x14);
			isLoaded = true;
			objdef = GetObjectDefinition();

            GetHandleMatrix();
        }

		public virtual ObjectDefinition GetObjectDefinition()
		{
			if (id < LevelData.ObjDefs.Count)
				return LevelData.ObjDefs[id];
			else
				return DefaultObjectDefinition.DefaultInstance;
		}

		[ParenthesizePropertyName(true)]
		public string Name { get { return objdef.Name; } }
		[ParenthesizePropertyName(true)]
		public string InternalName { get { return objdef.InternalName; } }
		protected bool isLoaded = false;

		protected ushort id;
		[Editor(typeof(IDEditor), typeof(UITypeEditor))]
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

		[DisplayName("Clip Level")]
		public ClipSetting ClipSetting
		{
			get { return (ClipSetting)ClipLevel; }
			set { ClipLevel = (ushort)value; }
		}

        public override Vertex Position { get { return position; } set { position = value; GetHandleMatrix(); } }
        public override Rotation Rotation { get { return rotation; } set { rotation = value; GetHandleMatrix(); } }
        protected Vertex scale = new Vertex();
        public Vertex Scale { get { return scale; } set { scale = value; GetHandleMatrix(); } }

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
			LevelData.SETItems[LevelData.Character].Add(this);
		}

		public override void Delete()
		{
			LevelData.SETItems[LevelData.Character].Remove(this);
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
			bytes.AddRange(BitConverter.GetBytes((ushort)(id | (cliplevel << 12))));
			unchecked
			{
				bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.X));
				bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.Y));
				bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.Z));
			}
			bytes.AddRange(Position.GetBytes());
			bytes.AddRange(Scale.GetBytes());
			return bytes.ToArray();
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
		All,
		HighOnly,
		MediumAndHigh
	}
}
