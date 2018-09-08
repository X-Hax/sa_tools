using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public class IDControl : UserControl
	{
		internal ListBox listBox1;
		public ushort value { get; private set; }
		SETItem item;
		private IWindowsFormsEditorService edSvc;

		public IDControl(ushort val, SETItem item, IWindowsFormsEditorService edSvc)
		{
			value = val;
			this.item = item;
			this.edSvc = edSvc;
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			listBox1 = new ListBox();
			SuspendLayout();
			// 
			// listView1
			// 
			listBox1.Dock = DockStyle.Fill;
			listBox1.Location = new Point(0, 0);
			listBox1.Name = "listBox1";
			listBox1.Size = new Size(150, 150);
			listBox1.TabIndex = 1;
			listBox1.DoubleClick += listView1_DoubleClick;
			listBox1.SelectedIndexChanged += listView1_SelectedIndexChanged;
			// 
			// BlockControl
			// 
			Controls.Add(listBox1);
			Name = "IDControl";
			Load += IDControl_Load;
			ResumeLayout(false);
		}

		private void IDControl_Load(object sender, EventArgs e)
		{

			foreach (ObjectDefinition item in (item is MissionSETItem && ((MissionSETItem)item).ObjectList == MsnObjectList.Mission) ? LevelData.MisnObjDefs : LevelData.ObjDefs)
				listBox1.Items.Add(item.Name);
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex > -1)
				value = (ushort)listBox1.SelectedIndex;
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			edSvc.CloseDropDown();
		}
	}

	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public class IDEditor : UITypeEditor
	{
		public IDEditor()
		{
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		// Displays the UI for value selection.
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			// Uses the IWindowsFormsEditorService to display a 
			// drop-down UI in the Properties window.
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc != null)
			{
				// Display an angle selection control and retrieve the value.
				IDControl idControl = new IDControl((ushort)value, (SETItem)context.Instance, edSvc);
				edSvc.DropDownControl(idControl);
				return idControl.value;
			}
			return value;
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return false;
		}

		public override bool IsDropDownResizable
		{
			get
			{
				return true;
			}
		}
	}
}