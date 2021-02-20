using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using SA_Tools;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SADXTweaker2
{
	public partial class MainForm : Form
	{
		private Properties.Settings Settings;
		private Dictionary<Form, ToolStripMenuItem> ActiveForms = new Dictionary<Form, ToolStripMenuItem>();

		public MainForm()
		{
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			InitializeComponent();
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			string errDesc = "SADXTweaker2 has crashed with the following error:\n" + e.Exception.GetType().Name + ".\n\n" +
				"If you wish to report a bug, please include the following in your report:";
			SonicRetro.SAModel.SAEditorCommon.ErrorDialog report = new SonicRetro.SAModel.SAEditorCommon.ErrorDialog("SADXTweaker2", errDesc, e.Exception.ToString());
			DialogResult dgresult = report.ShowDialog();
			switch (dgresult)
			{
				case DialogResult.Abort:
				case DialogResult.OK:
					Application.Exit();
					break;
			}
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
			if (Program.args.Length > 0)
				LoadINI(Program.args[0]);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "ini",
				Filter = "sonic_data.ini|sonic_data.ini|All Files|*.*"
			})
				if (a.ShowDialog(this) == DialogResult.OK)
					LoadINI(a.FileName);
		}

		private void recentProjectsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			fileToolStripMenuItem.DropDown.Close();
			LoadINI(Settings.MRUList[recentProjectsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}

		private void LoadINI(string filename)
		{
			CloseChildWindows();
			Program.IniData = IniSerializer.Deserialize<IniData>(filename);
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
			System.Diagnostics.Process.Start("https://github.com/sonicretro/sa_tools/issues");
		}

		private void AddChildForm(Type formType, ToolStripMenuItem menuItem)
		{
			Form form = (Form)Activator.CreateInstance(formType);
			form.FormClosed += new FormClosedEventHandler(form_FormClosed);
			ActiveForms.Add(form, menuItem);
			form.MdiParent = this;
			form.Show();
			menuItem.Checked = true;
			menuItem.Enabled = false;
		}

		private void AddChildForm(Type formType, string dataType, ToolStripMenuItem menuItem)
		{
			foreach (KeyValuePair<string, SA_Tools.FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals(dataType, StringComparison.OrdinalIgnoreCase))
				{
					AddChildForm(formType, menuItem);
					return;
				}
			MessageBox.Show(this, "No data of the specified type found!", menuItem.Text.Replace("&", ""), MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		private void form_FormClosed(object sender, FormClosedEventArgs e)
		{
			ActiveForms[(Form)sender].Checked = false;
			ActiveForms[(Form)sender].Enabled = true;
		}

		private void objectListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(ObjectListEditor), "objlist", objectListEditorToolStripMenuItem);
		}

		private void textureListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(TextureListEditor), "texlist", textureListEditorToolStripMenuItem);
		}

		private void levelTextureListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(LevelTextureListEditor), "leveltexlist", levelTextureListEditorToolStripMenuItem);
		}

		private void trialLevelListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(TrialLevelListEditor), "triallevellist", trialLevelListEditorToolStripMenuItem);
		}

		private void bossLevelListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(BossLevelListEditor), "bosslevellist", bossLevelListEditorToolStripMenuItem);
		}

		private void soundTestListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(SoundTestListEditor), "soundtestlist", soundTestListEditorToolStripMenuItem);
		}

		private void musicListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(MusicListEditor), "musiclist", musicListEditorToolStripMenuItem);
		}

		private void soundListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(SoundListEditor), "soundlist", soundListEditorToolStripMenuItem);
		}

		private void stringListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(StringListEditor), "stringarray", stringListEditorToolStripMenuItem);
		}

		private void nextLevelListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(NextLevelListEditor), "nextlevellist", nextLevelListEditorToolStripMenuItem);
		}

		private void cutsceneTextEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(CutsceneTextEditor), "cutscenetext", cutsceneTextEditorToolStripMenuItem);
		}

		private void recapScreenEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(RecapScreenEditor), "recapscreen", recapScreenEditorToolStripMenuItem);
		}

		private void levelClearFlagListEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(LevelClearFlagListEditor), "levelclearflags", levelClearFlagListEditorToolStripMenuItem);
		}

		private void messageFileEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(MessageFileEditor), messageFileEditorToolStripMenuItem);
		}

		private void nPCMessageEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(NPCMessageEditor), "npctext", nPCMessageEditorToolStripMenuItem);
		}

		private void chaoMessageFileEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddChildForm(typeof(ChaoMessageFileEditor), chaoMessageFileEditorToolStripMenuItem);
		}
	}
}