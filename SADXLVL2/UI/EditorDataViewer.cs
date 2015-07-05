using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace SonicRetro.SAModel.SADXLVL2.UI
{
	/// <summary>
	/// This class is meant for quickly looking over the memory/variable state of the editor without having to mess with the debugger.
	/// Consider making it non-modal so it can sit on another monitor and continually update.
	/// </summary>
	public partial class EditorDataViewer : Form
	{
		#region Container Structs
		private struct PropertyContainer
		{
			private PropertyInfo property;
			private TreeNode treeNode;

			public PropertyInfo Property { get { return property; } }
			public TreeNode Node { get { return treeNode; } }

			public PropertyContainer(PropertyInfo property, TreeNode treeNode)
			{
				this.property = property;
				this.treeNode = treeNode;
			}

			public void Dispose()
			{
				property = null;
				treeNode = null;
			}
		}

		private struct FieldContainer
		{
			private FieldInfo field;
			private TreeNode treeNode;

			public FieldInfo Field { get { return field;  } }
			public TreeNode Node { get { return treeNode; } }

			public FieldContainer(FieldInfo field, TreeNode treeNode)
			{
				this.field = field;
				this.treeNode = treeNode;
			}

			public void Dispose()
			{
				field = null;
				treeNode = null;
			}
		}
		#endregion

		private MainForm editorMainForm;
		private bool showFields = true;
		private bool showProperties = false;
		private int updateInterval = 5000;
		private Timer updateTimer;

		private List<string> typeNameExclude = new List<string> { "EditorDataViewer", "PropertyGrid", "MenuStrip", "ToolStripMenuItem", "UserControl", "SplitContainer", "OpenFileDialog" };
		private List<System.Type> typeList = new List<Type>();
		private List<PropertyContainer> propertyInfo = new List<PropertyContainer>();
		private List<FieldContainer> fieldInfo = new List<FieldContainer>();
		private TreeNode selectedNode;

		public EditorDataViewer(MainForm editorMainForm)
		{
			InitializeComponent();

			this.editorMainForm = editorMainForm;

			/*updateTimer = new Timer();
			updateTimer.Interval = updateInterval;
			updateTimer.Tick += new EventHandler(updateTimer_Tick);*/

			treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);

			UpdateView(editorMainForm);
		}

		// todo: find a way to store the selection and restore it during updates
		private void UpdateView(object objectToView)
		{
			StackTrace trace = new System.Diagnostics.StackTrace();
			StackFrame frame = trace.GetFrame(1);

			PropertyInfo[] properties = objectToView.GetType().GetProperties();
			FieldInfo[] fields = objectToView.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

			Cleanup(); // this does need to get called at some point to allow lists to update, but I fear that this will
						// need to be changed to allow for sub-object addition in the tree view

			if (showProperties)
			{
				foreach (PropertyInfo prop in properties)
				{
					if (typeNameExclude.Contains(prop.PropertyType.Name)) continue; // don't process prohibited types
					if (!typeList.Contains(prop.PropertyType))
					{
						AddType(prop.PropertyType);
					}

					CheckBox typeCheckBox = FindType(prop.PropertyType);
					if ((typeCheckBox != null) && (typeCheckBox.Checked))
					{
						TreeNode newNode = new TreeNode(prop.Name);
						treeView1.Nodes.Add(newNode);
						PropertyContainer newPropContainer = new PropertyContainer(prop, newNode);
						propertyInfo.Add(newPropContainer);
					}
				}
			}

			if (showFields)
			{
				foreach (FieldInfo field in fields)
				{
					if (typeNameExclude.Contains(field.FieldType.Name)) continue; // don't process prohibited types
					if (!typeList.Contains(field.FieldType))
					{
						AddType(field.FieldType);
					}

					CheckBox typeCheckBox = FindType(field.FieldType);

					if ((typeCheckBox != null) && (typeCheckBox.Checked))
					{
						TreeNode newNode = new TreeNode(field.Name);
						treeView1.Nodes.Add(newNode);
						FieldContainer newFieldContainer = new FieldContainer(field, newNode);
						fieldInfo.Add(newFieldContainer);
					}
				}
			}
		}

		private CheckBox FindType(Type type)
		{
			foreach (CheckBox checkBox in viewTypesPanel.Controls)
			{
				if (checkBox.Name == type.Name) return checkBox;
			}

			return null;
		}

		private void AddType(Type type)
		{
			typeList.Add(type);
			CheckBox propCheckBox = new CheckBox();
			propCheckBox.Name = type.Name;
			propCheckBox.Text = type.Name;
			propCheckBox.Checked = true;
			propCheckBox.CheckedChanged += new EventHandler(propCheckBox_CheckedChanged);
			this.viewTypesPanel.Controls.Add(propCheckBox);
		}

		private void Cleanup()
		{
			foreach (PropertyContainer prop in propertyInfo) prop.Dispose();
			foreach (FieldContainer field in fieldInfo) field.Dispose();
			for (int i = 0; i < typeList.Count; i++)
			{
				typeList[i] = null;
			}

			propertyInfo.Clear();
			typeList.Clear();

			foreach (CheckBox checkBox in viewTypesPanel.Controls) checkBox.Dispose();
			viewTypesPanel.Controls.Clear();

			treeView1.Nodes.Clear();

			GC.Collect();
		}

		#region Event methods
		void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			selectedNode = treeView1.SelectedNode;

			bool targetIsField = false;
			FieldContainer field = fieldInfo.Find(item => item.Node == selectedNode);
			PropertyContainer property = propertyInfo.Find(item => item.Node == selectedNode);

			targetIsField = (field.Node != null);

			if (targetIsField) propertyGrid1.SelectedObject = (object)field.Field.GetValue(editorMainForm);
			else
			{
				propertyGrid1.SelectedObject = null;
			}

			// todo: expand the list here. We can do this and avoid duplicates by creating a list of objects that have had their child fields added to the tree.
			// we'll also change the Update method to allow ANY kind of object to have its properties loaded
		}

		void propCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;

			// go through our treeview, and pull out any tree nodes that match our type
			foreach (PropertyContainer prop in propertyInfo)
			{
				if (checkBox.Text == prop.Property.PropertyType.Name)
				{
					if (checkBox.Checked) // add our treeNode if it doesn't exist
					{
						if (!treeView1.Nodes.Contains(prop.Node)) treeView1.Nodes.Add(prop.Node);
					}
					else // remove it if it does
					{
						treeView1.Nodes.Remove(prop.Node);
					}
				}
			}

			foreach (FieldContainer field in fieldInfo)
			{
				if (checkBox.Text == field.Field.FieldType.Name)
				{
					if (checkBox.Checked) // add our treeNode if it doesn't exist
					{
						if (!treeView1.Nodes.Contains(field.Node)) treeView1.Nodes.Add(field.Node);
					}
					else // remove it if it does
					{
						treeView1.Nodes.Remove(field.Node);
					}
				}
			}
		}

		void updateTimer_Tick(object sender, EventArgs e)
		{
			UpdateView(editorMainForm);
		}
		#endregion
	}
}
