using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace SonicRetro.SAModel.SADXLVL2
{
    public abstract class Item : IComponent
    {
        [ReadOnly(true)]
        [ParenthesizePropertyName(true)]
        public string Type { get { return GetType().Name; } }

        [Browsable(false)]
        public abstract Vertex Position { get; set; }
        [Browsable(false)]
        public abstract Rotation Rotation { get; set; }

        [DisplayName("Position")]
        public EditableVertex _Position
        {
            get
            {
                return new EditableVertex(Position);
            }
            set
            {
                Position = value.ToVertex();
            }
        }

        [DisplayName("Rotation")]
        public EditableRotation _Rotation
        {
            get
            {
                return new EditableRotation(Rotation);
            }
            set
            {
                Rotation = value.ToRotation();
            }
        }

        public abstract float CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View);
        public abstract void Render(Device dev, MatrixStack transform, Texture[] textures, bool selected);

        #region IComponent Members
        // IComponent required by PropertyGrid control to discover IMenuCommandService supporting DesignerVerbs

        public event EventHandler Disposed;

        // ** Item of interest ** Return the site object that supports DesignerVerbs
        [Browsable(false)]
        public ISite Site
        {
            // return our "site" which connects back to us to expose our tagged methods
            get { return new DesignerVerbSite(this); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members
        // IDisposable, part of IComponent support

        public void Dispose()
        {
            // never called in this specific context with the PropertyGrid
            // but just reference the required Disposed event to avoid warnings
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        #endregion
    }

    public class DesignerVerbSite : IMenuCommandService, ISite
    {
        // our target object
        protected object _Component;

        public DesignerVerbSite(object component)
        {
            _Component = component;
        }

        #region IMenuCommandService Members
        // IMenuCommandService provides DesignerVerbs, seen as commands in the PropertyGrid control

        public void AddCommand(MenuCommand command)
        {
            throw new NotImplementedException();
        }

        public void AddVerb(DesignerVerb verb)
        {
            throw new NotImplementedException();
        }

        public MenuCommand FindCommand(CommandID commandID)
        {
            throw new NotImplementedException();
        }

        public bool GlobalInvoke(CommandID commandID)
        {
            throw new NotImplementedException();
        }

        public void RemoveCommand(MenuCommand command)
        {
            throw new NotImplementedException();
        }

        public void RemoveVerb(DesignerVerb verb)
        {
            throw new NotImplementedException();
        }

        public void ShowContextMenu(CommandID menuID, int x, int y)
        {
            throw new NotImplementedException();
        }

        // ** Item of interest ** Return the DesignerVerbs collection
        public DesignerVerbCollection Verbs
        {
            get
            {
                DesignerVerbCollection Verbs = new DesignerVerbCollection();
                // Use reflection to enumerate all the public methods on the object
                MethodInfo[] mia = _Component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (MethodInfo mi in mia)
                {
                    // Ignore any methods without a [Browsable(true)] attribute
                    object[] attrs = mi.GetCustomAttributes(typeof(BrowsableAttribute), true);
                    if (attrs == null || attrs.Length == 0)
                        continue;
                    if (!((BrowsableAttribute)attrs[0]).Browsable)
                        continue;
                    // Add a DesignerVerb with our VerbEventHandler
                    // The method name will appear in the command pane
                    string name = mi.Name;
                    attrs = mi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    if (attrs != null & attrs.Length > 0)
                        name = ((DisplayNameAttribute)attrs[0]).DisplayName;
                    Verbs.Add(new CustomDesignerVerb(name, new EventHandler(VerbEventHandler), mi));
                }
                return Verbs;
            }
        }

        // ** Item of interest ** Handle invokaction of the DesignerVerbs
        private void VerbEventHandler(object sender, EventArgs e)
        {
            // The verb is the sender
            CustomDesignerVerb verb = sender as CustomDesignerVerb;
            verb.Method.Invoke(_Component, null);
        }

        #endregion

        #region ISite Members
        // ISite required to represent this object directly to the PropertyGrid

        public IComponent Component
        {
            get { throw new NotImplementedException(); }
        }

        // ** Item of interest ** Implement the Container property
        public IContainer Container
        {
            // Returning a null Container works fine in this context
            get { return null; }
        }

        // ** Item of interest ** Implement the DesignMode property
        public bool DesignMode
        {
            // While this *is* called, it doesn't seem to matter whether we return true or false
            get { return true; }
        }

        public string Name
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IServiceProvider Members
        // IServiceProvider is the mechanism used by the PropertyGrid to discover our IMenuCommandService support

        // ** Item of interest ** Respond to requests for IMenuCommandService
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IMenuCommandService))
                return this;
            return null;
        }

        #endregion
    }

    public class CustomDesignerVerb : DesignerVerb
    {
        public MethodInfo Method { get; set; }

        public CustomDesignerVerb(string text, EventHandler handler, MethodInfo method) : base(text, handler)
        {
            Method = method;
        }
    }

    [TypeConverter(typeof(EditableVertexConverter))]
    public class EditableVertex : Vertex
    {
        public EditableVertex(Vertex item)
        {
            X = item.X;
            Y = item.Y;
            Z = item.Z;
        }

        public Vertex ToVertex()
        {
            return new Vertex() { X = this.X, Y = this.Y, Z = this.Z };
        }
    }

    public class EditableVertexConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(EditableVertex))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is EditableVertex)
                return ((EditableVertex)value).ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
                return new EditableVertex(new Vertex((string)value));
            return base.ConvertFrom(context, culture, value);
        }
    }

    [TypeConverter(typeof(EditableRotationConverter))]
    public class EditableRotation : Rotation
    {
        public EditableRotation(Rotation item)
        {
            X = item.X;
            Y = item.Y;
            Z = item.Z;
        }

        public Rotation ToRotation()
        {
            return new Rotation() { X = this.X, Y = this.Y, Z = this.Z };
        }
    }

    public class EditableRotationConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(EditableRotation))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is EditableRotation)
                return ((EditableRotation)value).ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
                return new EditableRotation(new Rotation((string)value));
            return base.ConvertFrom(context, culture, value);
        }
    }
}