using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using System.ComponentModel;

namespace SonicRetro.SAModel.SAEditorCommon.SETEditing
{
	public abstract class ObjectDefinition
	{
		public abstract void Init(ObjectData data, string name, Device dev);
		public abstract HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform);
		public abstract List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform);
		public abstract string Name { get; }
		public virtual PropertySpec[] CustomProperties { get { return new PropertySpec[0]; } }
		/// <summary>
		/// Returns a bounding sphere for the supplied SET Item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual BoundingSphere GetBounds(SETItem item, Object model=null)
		{
			BoundingSphere result = new BoundingSphere();

			if (model != null)
			{
				// build bounding spheres for all children using BoundingSphere.CreateMerged
				if (model.Attach != null)
				{
					result = model.Attach.Bounds;
				}

				ProcessModel(model, ref result);
			}

			return new BoundingSphere(item.Position, result.Radius);
		}

		private void ProcessModel(Object model, ref BoundingSphere bounds)
		{
			if (model.Attach != null) bounds = Direct3D.Extensions.Merge(bounds, model.Attach.Bounds);

			foreach (Object child in model.Children) ProcessModel(child, ref bounds);
		}

		public string InternalName { get; protected set; }
		public void SetInternalName(string name)
		{
			InternalName = name;
		}
	}

	/// <summary>
	/// Represents a single property in a PropertySpec.
	/// </summary>
	public class PropertySpec
	{
		private Attribute[] attributes;
		private string category;
		private object defaultValue;
		private string description;
		private string name;
		private Type type;
		private Type typeConverter;
		private Dictionary<string, int> @enum;
		private Func<SETItem, object> getMethod;
		private Action<SETItem, object> setMethod;

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
			this.attributes = null;
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
			this(name, type, category, description, defaultValue, Type.GetType(typeConverter), getMethod, setMethod) { }

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
			this(name, Type.GetType(type), category, description, defaultValue, typeConverter, getMethod, setMethod) { }

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
			this(name, type, category, description, defaultValue, Type.GetType(typeConverter), @enum, getMethod, setMethod) { }

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
			this(name, Type.GetType(type), category, description, defaultValue, typeConverter, @enum, getMethod, setMethod) { }

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

		/// <summary>
		/// Gets or sets a collection of additional Attributes for this property.  This can
		/// be used to specify attributes beyond those supported intrinsically by the
		/// PropertySpec class, such as ReadOnly and Browsable.
		/// </summary>
		public Attribute[] Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}

		/// <summary>
		/// Gets or sets the category name of this property.
		/// </summary>
		public string Category
		{
			get { return category; }
			set { category = value; }
		}

		/// <summary>
		/// Gets or sets the default value of this property.
		/// </summary>
		public object DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}

		/// <summary>
		/// Gets or sets the help text description of this property.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// Gets or sets the name of this property.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets or sets the type of this property.
		/// </summary>
		public Type Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// Gets or sets the type converter
		/// type for this property.
		/// </summary>
		public Type ConverterType
		{
			get { return typeConverter; }
			set { typeConverter = value; }
		}

		public object GetValue(SETItem item)
		{
			return getMethod(item);
		}

		public void SetValue(SETItem item, object value)
		{
			setMethod(item, value);
		}

		public Dictionary<string, int> Enumeration
		{
			get { return @enum; }
			set { @enum = value; }
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
				return !this.GetValue(component).Equals(item.DefaultValue);
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
			object val = this.GetValue(component);

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
}
