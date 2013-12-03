using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using SADXPCTools;

namespace SADXTweakerLite
{
    public partial class MainForm : Form
    {
        private Properties.Settings Settings;
        private Dictionary<Form, ToolStripMenuItem> ActiveForms = new Dictionary<Form,ToolStripMenuItem>();

        public MainForm()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            InitializeComponent();
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            using (ErrorDialog ed = new ErrorDialog(e.Exception, true))
                if (ed.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
                    Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Settings = Properties.Settings.Default;
            if (Settings.MRUList == null)
                Settings.MRUList = new StringCollection();
            StringCollection mru = new StringCollection();
            foreach (string item in Settings.MRUList)
                if (File.Exists(item))
                {
                    mru.Add(item);
                    recentProjectsToolStripMenuItem.DropDownItems.Add(item.Replace("&", "&&"));
                }
            Settings.MRUList = mru;
            if (!string.IsNullOrEmpty(Settings.CleanExe))
                if (File.Exists(Settings.CleanExe))
                {
                    Program.CleanExe = File.ReadAllBytes(Settings.CleanExe);
                    chooseCleanEXEToolStripMenuItem.Checked = true;
                }
                else
                    Settings.CleanExe = null;
            if (Program.args.Length > 0)
                LoadEXE(Program.args[0]);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog a = new OpenFileDialog()
            {
                DefaultExt = "exe",
                Filter = "EXE Files|*.exe|All Files|*.*"
            })
                if (a.ShowDialog(this) == DialogResult.OK)
                    LoadEXE(a.FileName);
        }

        private void recentProjectsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            fileToolStripMenuItem.DropDown.Close();
            LoadEXE(Settings.MRUList[recentProjectsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
        }

        private void LoadEXE(string filename)
        {
            CloseChildWindows();
            Program.ExeData = File.ReadAllBytes(filename);
            windowToolStripMenuItem.Enabled = true;
            if (Settings.MRUList.Contains(filename))
            {
                recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(Settings.MRUList.IndexOf(filename));
                Settings.MRUList.Remove(filename);
            }
            Settings.MRUList.Insert(0, filename);
            recentProjectsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename));
            Environment.CurrentDirectory = Path.GetDirectoryName(filename);
        }

        private void chooseCleanEXEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog a = new OpenFileDialog()
            {
                DefaultExt = "exe",
                Filter = "EXE Files|*.exe|All Files|*.*"
            })
                if (a.ShowDialog(this) == DialogResult.OK)
                {
                    Program.CleanExe = File.ReadAllBytes(a.FileName);
                    Settings.CleanExe = a.FileName;
                    chooseCleanEXEToolStripMenuItem.Checked = true;
                }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseChildWindows();
            Close();
        }

        private void CloseChildWindows()
        {
            foreach (Form form in this.MdiChildren)
                form.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Save();
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (BugReportDialog brd = new BugReportDialog())
                brd.ShowDialog(this);
        }

        private void AddChildForm(Type formType, string dataType, ToolStripMenuItem menuItem)
        {
            Form form = (Form)Activator.CreateInstance(formType);
            form.FormClosed += new FormClosedEventHandler(form_FormClosed);
            ActiveForms.Add(form, menuItem);
            form.MdiParent = this;
            form.Show();
            menuItem.Checked = true;
            menuItem.Enabled = false;
        }

        private void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ActiveForms[(Form)sender].Checked = false;
            ActiveForms[(Form)sender].Enabled = true;
        }

        private void objectListEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AddChildForm(typeof(ObjectListEditor), "objlist", objectListEditorToolStripMenuItem);
        }
    }
}