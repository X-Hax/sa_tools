using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace SADXTweaker2
{
    [Designer(typeof(FileListControlDesigner))]
    [DefaultEvent("ValueChanged")]
    public partial class FileListControl : UserControl
    {
        public FileListControl()
        {
            InitializeComponent();
        }

        public event EventHandler ValueChanged = delegate { };

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValueChanged(this, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FileList fl = new FileList(Directory, Extension, Title, Value))
                if (fl.ShowDialog(ParentForm) == DialogResult.OK)
                    Value = fl.SelectedItem;
        }

        public string Value { get { return textBox1.Text; } set { textBox1.Text = value; } }

        public string Directory { get; set; }

        public string Extension { get; set; }

        public string Title { get; set; }
    }

    public class FileListControlDesigner : ControlDesigner
    {
        public override IList SnapLines
        {
            get
            {
                ArrayList list = new ArrayList(base.SnapLines);
                list.Add(new SnapLine(SnapLineType.Baseline, 18));
                return list;
            }
        }
    }
}