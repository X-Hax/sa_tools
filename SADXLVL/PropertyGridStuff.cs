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
        public abstract Vertex Position { get; set; }
        public abstract Rotation Rotation { get; set; }

        public abstract float CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View);

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
                    Verbs.Add(new DesignerVerb(mi.Name, new EventHandler(VerbEventHandler)));
                }
                return Verbs;
            }
        }

        // ** Item of interest ** Handle invokaction of the DesignerVerbs
        private void VerbEventHandler(object sender, EventArgs e)
        {
            // The verb is the sender
            DesignerVerb verb = sender as DesignerVerb;
            // Enumerate the methods again to find the one named by the verb
            MethodInfo[] mia = _Component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo mi in mia)
            {
                object[] attrs = mi.GetCustomAttributes(typeof(BrowsableAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;
                if (!((BrowsableAttribute)attrs[0]).Browsable)
                    continue;
                if (verb.Text == mi.Name)
                {
                    // Invoke the method on our object (no parameters)
                    mi.Invoke(_Component, null);
                    return;
                }
            }
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
                throw new NotImplementedException();
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
}