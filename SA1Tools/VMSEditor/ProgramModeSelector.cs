using System;
using System.Windows.Forms;

namespace VMSEditor
{
	public partial class ProgramModeSelector : Form
	{
        public ProgramModeSelector()
		{
			InitializeComponent();
		}

		private void buttonExit_Click(object sender, EventArgs e)
		{
            Close();
		}

		private void buttonChaoEditor_Click(object sender, EventArgs e)
		{
            EditorChao editor = new EditorChao();
            editor.Show();
            Hide();
        }

		private void buttonDLCEditor_Click(object sender, EventArgs e)
		{
            EditorDLC editor = new EditorDLC();
            editor.Show();
            Hide();
        }

		private void buttonVMIEditor_Click(object sender, EventArgs e)
		{
            EditorVMI editor = new EditorVMI();
            Hide();
            editor.Show();
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            
            
            
            using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "vms", Filter = "Supported Files|*.vms;*.vmi|VMS Files|*.vms|VMI Files|*.vmi|All Files|*.*" })
                if (od.ShowDialog(this) == DialogResult.OK)
                {
                    Form editor;
                    Program.args = new string[1];
                    Program.args[0] = od.FileName;
                    editor = Program.CheckFile(od.FileName);
                    Hide();
                    editor.Show();
                }
        }
    }
}
