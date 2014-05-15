using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
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
        private IWindowsFormsEditorService edSvc;

        public IDControl(ushort val, IWindowsFormsEditorService edSvc)
        {
            value = val;
            this.edSvc = edSvc;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.listBox1 = new ListBox();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(150, 150);
            this.listBox1.TabIndex = 1;
            this.listBox1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // BlockControl
            // 
            this.Controls.Add(this.listBox1);
            this.Name = "IDControl";
            this.Load += new System.EventHandler(this.IDControl_Load);
            this.ResumeLayout(false);
        }

        private void IDControl_Load(object sender, EventArgs e)
        {
            foreach (ObjectDefinition item in LevelData.ObjDefs)
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

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
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
                IDControl idControl = new IDControl((ushort)value, edSvc);
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