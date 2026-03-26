using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.Direct3D.TextureSystem;

namespace SAModel.SAEditorCommon.SETEditing
{
	/// <summary>
	/// General class for both simple (codeless) and custom coded object definitions for SALVL.
	/// </summary>
	public abstract class ObjectDefinition
	{
		#region Variables
		/// <summary>
		/// The internal name of the object from the game.
		/// </summary>
		public string InternalName { get; protected set; }

		/// <summary>
		/// Public function to assign a string to the internal name externally.
		/// </summary>
		/// <param name="name"></param>
		public void SetInternalName(string name)
		{
			InternalName = name;
		}

		/// <summary>
		/// The common/displayed name of the object in SALVL. If not assigned, it will return the <see cref="InternalName"/>.
		/// </summary>
		public abstract string Name { get; }


		#region Position Variables
		/// <summary>
		/// The distance in which an object should sit from the ground.
		/// 
		/// This is referenced when using the "To Ground" feature within SALVL.
		/// </summary>
		public virtual float DistanceFromGround { get { return 0; } }

		/// <summary>
		/// Additional X Axis position data added to the object without affecting the <see cref="SETItem"/> info.
		/// </summary>
		public virtual float AddXPosition { get { return 0; } }

		/// <summary>
		/// Additional Y Axis position data added to the object without affecting the <see cref="SETItem"/> info.
		/// </summary>
		public virtual float AddYPosition { get { return 0; } }

		/// <summary>
		/// Additional Z Axis position data added to the object without affecting the <see cref="SETItem"/> info.
		/// </summary>
		public virtual float AddZPosition { get { return 0; } }

		#endregion

		#region Rotation Variables
		/// <summary>
		/// The default X Axis rotation of an object.
		/// </summary>
		public virtual ushort DefaultXRotation { get { return 0; } }

		/// <summary>
		/// The default Y Axis rotation of an object.
		/// </summary>
		public virtual ushort DefaultYRotation { get { return 0; } }

		/// <summary>
		/// The default Z Axis rotation of an object.
		/// </summary>
		public virtual ushort DefaultZRotation { get { return 0; } }

		/// <summary>
		/// Additional X Axis rotation data added to the object without affecting the <see cref="SETItem"/> info.
		/// </summary>
		public virtual ushort AddXRotation { get { return 0; } }

		/// <summary>
		/// Additional Y Axis rotation data added to the object without affecting the <see cref="SETItem"/> info.
		/// </summary>
		public virtual ushort AddYRotation { get { return 0; } }

		/// <summary>
		/// Additional Z Axis rotation data added to the object without affecting the <see cref="SETItem"/> info.
		/// </summary>
		public virtual ushort AddZRotation { get { return 0; } }

		#endregion

		#region Scale Variables
		/// <summary>
		/// The default X Axis scale of an object.
		/// </summary>
		public virtual float DefaultXScale { get { return 1.0f; } }

		/// <summary>
		/// The default Y Axis scale of an object.
		/// </summary>
		public virtual float DefaultYScale { get { return 1.0f; } }

		/// <summary>
		/// The default Z Axis scale of an object.
		/// </summary>
		public virtual float DefaultZScale { get { return 1.0f; } }

		/// <summary>
		/// Additional X Axis scale data added to the object without affecting the <see cref="SETItem"/> info.
		/// </summary>
		public virtual float AddXScale { get { return 0; } }

		/// <summary>
		/// Additional Y Axis scale data added to the object without affecting the <see cref="SETItem"/> info.
		/// </summary>
		public virtual float AddYScale { get { return 0; } }

		/// <summary>
		/// Additional Z Axis scale data added to the object without affecting the <see cref="SETItem"/> info.
		/// </summary>
		public virtual float AddZScale { get { return 0; } }

		#endregion

		public virtual PropertySpec[] CustomProperties { get { return new PropertySpec[0]; } }
		public virtual PropertySpec[] MissionProperties { get { return null; } }
		public virtual VerbSpec[] CustomVerbs { get { return new VerbSpec[0]; } }

		#endregion

		#region Override Functions
		/// <summary>
		/// Initializes the object when it is loaded in SALVL.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="name"></param>
		public abstract void Init(ObjectData data, string name);

		/// <summary>
		/// Renders the object into the scene. 
		/// 
		/// Typically this should replicate the behavior of an object from the base game when using custom code.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="dev"></param>
		/// <param name="camera"></param>
		/// <param name="transform"></param>
		/// <returns></returns>
		public abstract List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform);

		/// <summary>
		/// Returns a <see cref="HitResult"/> if a mouse click cast collides with the object.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="Near"></param>
		/// <param name="Far"></param>
		/// <param name="Viewport"></param>
		/// <param name="Projection"></param>
		/// <param name="View"></param>
		/// <param name="transform"></param>
		/// <returns></returns>
		public abstract HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform);
		
		/// <summary>
		/// Returns a list of a <see cref="ModelTransform"/> for the object.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="transform"></param>
		/// <returns></returns>
		public abstract List<ModelTransform> GetModels(SETItem item, MatrixStack transform);

		/// <summary>
		/// Intended to set the orientation of an object based on the supplied direction.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="direction"></param>
		public virtual void SetOrientation(SETItem item, Vertex direction) { }

		/// <summary>
		/// Intended to direct the object to point toward a supplied location.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="location"></param>
		public virtual void PointTo(SETItem item, Vertex location) { }

		/// <summary>
		/// Returns the <see cref="Matrix"/> used for the handle for the object.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public abstract Matrix GetHandleMatrix(SETItem item);
		
		/// <summary>
		/// Returns a bounding sphere for the supplied SET Item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual BoundingSphere GetBounds(SETItem item)
		{
			return new BoundingSphere(item.Position, 0);
		}

		/// <summary>
		/// Returns the object's texture names and bitmaps as an array of BMPInfo.
		/// In an override of this function, for most objects it should export textures of the whole object PVM, such as OBJ_BEACH.
		/// In multi-archive objects, it should export an array of actually used textures matching the texlist.
		/// </summary>
		/// <param name="item">The SET item corresponding to the object.</param>
		/// <returns>An array of BMPInfo.</returns>
		public virtual BMPInfo[] ExportTextures(SETItem item)
		{			
			return null;
		}
		#endregion
	}

	public class ModelTransform
	{
		public NJS_OBJECT Model { get; private set; }
		public Matrix Transform { get; private set; }

		public ModelTransform(NJS_OBJECT model, Matrix transform)
		{
			Model = model;
			Transform = transform;
		}
	}

	/// <summary>
	/// Represents a single property in a PropertySpec.
	/// </summary>
	public class PropertySpec
	{
		#region Variables
		private Attribute[] attributes;

		/// <summary>
		/// Gets or sets a collection of additional Attributes for this property.
		/// 
		/// This can be used to specify attributes beyond those supported intrinsically by the PropertySpec class, such as ReadOnly and Browsable.
		/// </summary>
		public Attribute[] Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}

		private string category;

		/// <summary>
		/// Gets or sets the category name of this property.
		/// </summary>
		public string Category
		{
			get { return category; }
			set { category = value; }
		}

		private object defaultValue;

		/// <summary>
		/// Gets or sets the default value of this property.
		/// </summary>
		public object DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}

		private string description;

		/// <summary>
		/// Gets or sets the help text description of this property.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		private string name;

		/// <summary>
		/// Gets or sets the name of this property.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private Type type;

		/// <summary>
		/// Gets or sets the <see cref="System.Type"/> of this property.
		/// </summary>
		public Type Type
		{
			get { return type; }
			set { type = value; }
		}

		private Type typeConverter;

		/// <summary>
		/// Gets or sets the type converter for the value of this property.
		/// </summary>
		public Type ConverterType
		{
			get { return typeConverter; }
			set { typeConverter = value; }
		}

		private Dictionary<string, int> @enum;

		/// <summary>
		/// Internally created enumeration list using a <see cref="Dictionary{TKey, TValue}"/>.
		/// </summary>
		public Dictionary<string, int> Enumeration
		{
			get { return @enum; }
			set { @enum = value; }
		}

		private Func<SETItem, object> getMethod;

		private Action<SETItem, object> setMethod;

		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, string type, string category, string description, object defaultValue, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod)
			: this(name, Type.GetType(type), category, description, defaultValue, getMethod, setMethod) { }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, Type type, string category, string description, object defaultValue, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod)
		{
			this.name = name;
			this.type = type;
			this.category = category;
			this.description = description;
			this.defaultValue = defaultValue;
			attributes = null;
			this.getMethod = getMethod;
			this.setMethod = setMethod;
		}

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, string type, string category, string description, object defaultValue, string typeConverter, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod)
			: this(name, Type.GetType(type), category, description, defaultValue, Type.GetType(typeConverter), getMethod, setMethod) { }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, Type type, string category, string description, object defaultValue, string typeConverter, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod) :
			this(name, type, category, description, defaultValue, Type.GetType(typeConverter), getMethod, setMethod)
		{ }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, string type, string category, string description, object defaultValue, Type typeConverter, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod) :
			this(name, Type.GetType(type), category, description, defaultValue, typeConverter, getMethod, setMethod)
		{ }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, Type type, string category, string description, object defaultValue, Type typeConverter, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod) :
			this(name, type, category, description, defaultValue, getMethod, setMethod)
		{
			this.typeConverter = typeConverter;
		}

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="enum">The enumeration used by the property.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, string type, string category, string description, object defaultValue, string typeConverter, Dictionary<string, int> @enum, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod)
			: this(name, Type.GetType(type), category, description, defaultValue, Type.GetType(typeConverter), @enum, getMethod, setMethod) { }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="enum">The enumeration used by the property.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, Type type, string category, string description, object defaultValue, string typeConverter, Dictionary<string, int> @enum, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod) :
			this(name, type, category, description, defaultValue, Type.GetType(typeConverter), @enum, getMethod, setMethod)
		{ }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="enum">The enumeration used by the property.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, string type, string category, string description, object defaultValue, Type typeConverter, Dictionary<string, int> @enum, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod) :
			this(name, Type.GetType(type), category, description, defaultValue, typeConverter, @enum, getMethod, setMethod)
		{ }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="enum">The enumeration used by the property.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, Type type, string category, string description, object defaultValue, Type typeConverter, Dictionary<string, int> @enum, Func<SETItem, object> getMethod, Action<SETItem, object> setMethod) :
			this(name, type, category, description, defaultValue, typeConverter, getMethod, setMethod)
		{
			this.@enum = @enum;
		}

		#endregion

		/// <summary>
		/// Returns the Value of the property.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public object GetValue(SETItem item)
		{
			return getMethod(item);
		}

		/// <summary>
		/// Sets the Value of the property.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="value"></param>
		public void SetValue(SETItem item, object value)
		{
			setMethod(item, value);
		}
	}

	internal class PropertySpecDescriptor : PropertyDescriptor
	{
		private PropertySpec item;

		public PropertySpecDescriptor(PropertySpec item, string name, Attribute[] attrs) :
			base(name, attrs)
		{
			this.item = item;
		}

		public override Type ComponentType
		{
			get { return item.GetType(); }
		}

		public override bool IsReadOnly
		{
			get { return (Attributes.Matches(ReadOnlyAttribute.Yes)); }
		}

		public override Type PropertyType
		{
			get { return item.Type; }
		}

		public override bool CanResetValue(object component)
		{
			if (item.DefaultValue == null)
				return false;
			else
				return !GetValue(component).Equals(item.DefaultValue);
		}

		public override object GetValue(object component)
		{
			return item.GetValue((SETItem)component);
		}

		public override void ResetValue(object component)
		{
			SetValue(component, item.DefaultValue);
		}

		public override void SetValue(object component, object value)
		{
			item.SetValue((SETItem)component, value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			object val = GetValue(component);

			if (item.DefaultValue == null && val == null)
				return false;
			else
				return !val.Equals(item.DefaultValue);
		}

		public override TypeConverter Converter
		{
			get
			{
				if (item.ConverterType != null)
					return (TypeConverter)Activator.CreateInstance(item.ConverterType);
				return base.Converter;
			}
		}

		public override string Category { get { return item.Category; } }

		public override string Description { get { return item.Description; } }

		public Dictionary<string, int> Enumeration { get { return item.Enumeration; } }
	}

	public class VerbSpec
	{
		private string name;
		private Action<SETItem> method;

		/// <summary>
		/// Initializes a new instance of the VerbSpec class.
		/// </summary>
		/// <param name="name">The name of the verb displayed in the property grid.</param>
		/// <param name="method">The method called when clicked.</param>
		public VerbSpec(string name, Action<SETItem> method)
		{
			this.name = name;
			this.method = method;
		}

		/// <summary>
		/// Gets or sets the name of this property.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public void DoVerb(SETItem item)
		{
			method(item);
		}
	}
}
