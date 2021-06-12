using SplitTools.SplitDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon.DLLModGenerator
{
	public partial class DLLModGenUI : Form
	{
		public DLLModGenUI()
		{
			InitializeComponent();
		}

		private string projectFolder = "";
		private string modFolder = "";
		private string fileName = "";

		DllIniData IniData;
		Dictionary<string, bool> itemsToExport = new Dictionary<string, bool>();

		public void SetProjectFolder(string projectFolder)
		{
			this.projectFolder = projectFolder;
		}

		public void SetModFolder(string modFolder)
		{
			this.modFolder = modFolder;
		}

		public void OpenFile(string fileName)
		{
			this.fileName = fileName;

			LoadINIFile(Path.Combine(projectFolder, fileName));
			ShowDialog();
		}

		private void LoadINIFile(string fileName)
		{
			IniData = DLLModGen.LoadINI(fileName, ref itemsToExport);
			UpdateListView();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{

		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog()
			{
				DefaultExt = "ini",
				Filter = "INI Files|*.ini|All Files|*.*"
			})
			{
				fileDialog.InitialDirectory = projectFolder;
				fileDialog.FileName = fileName + "_data.ini";

				if (fileDialog.ShowDialog(this) == DialogResult.OK)
				{
					LoadINIFile(fileDialog.FileName);
				}
			}
		}

		private void UpdateListView()
		{
			listView1.BeginUpdate();
			listView1.Items.Clear();

			foreach (KeyValuePair<string, FileTypeHash> item in IniData.Files)
			{
				KeyValuePair<string, bool> exportStatus = itemsToExport.First(export => export.Key == item.Key);

				bool modified = exportStatus.Value;

				listView1.Items.Add(new ListViewItem(new[] { item.Key, modified ? "Yes" : "No" }) { Checked = modified }); ;
			}

			listView1.EndUpdate();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void CheckAllButton_Click(object sender, EventArgs e)
		{
			listView1.BeginUpdate();
			foreach (ListViewItem item in listView1.Items)
				item.Checked = true;
			listView1.EndUpdate();
		}

		private void CheckModifiedButton_Click(object sender, EventArgs e)
		{
			listView1.BeginUpdate();
			foreach (ListViewItem item in listView1.Items)
				item.Checked = item.SubItems[2].Text != "No";
			listView1.EndUpdate();
		}

		private void UncheckAllButton_Click(object sender, EventArgs e)
		{
			listView1.BeginUpdate();
			foreach (ListViewItem item in listView1.Items)
				item.Checked = false;
			listView1.EndUpdate();
		}

		private void ExportCPPButton_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog fileDialog = new SaveFileDialog() { DefaultExt = "cpp", Filter = "C++ source files|*.cpp", InitialDirectory = Environment.CurrentDirectory, RestoreDirectory = true })
			{
				fileDialog.InitialDirectory = modFolder;
				fileDialog.FileName = fileName;

				if (fileDialog.ShowDialog(this) == DialogResult.OK)
				{
					DLLModGen.ExportCPP(IniData, itemsToExport, fileDialog.FileName);
				}
			}
		}

		private void ExportINIButton_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog fileDialog = new SaveFileDialog() { DefaultExt = "ini", Filter = "INI files|*.ini", InitialDirectory = Environment.CurrentDirectory, RestoreDirectory = true })
			{
				fileDialog.InitialDirectory = modFolder;
				fileDialog.FileName = fileName;

				if (fileDialog.ShowDialog(this) == DialogResult.OK)
				{
					DLLModGen.ExportINI(IniData, itemsToExport, fileDialog.FileName);
				}
			}
		}

		private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			itemsToExport[e.Item.Text] = e.Item.Checked;
		}
	}

	static class Extensions
	{
		internal static List<string> GetLabels(this LandTable land)
		{
			List<string> labels = new List<string>() { land.Name };
			if (land.COLName != null)
			{
				labels.Add(land.COLName);
				foreach (COL col in land.COL)
					if (col.Model != null)
						labels.AddRange(col.Model.GetLabels());
			}
			if (land.AnimName != null)
			{
				labels.Add(land.AnimName);
				foreach (GeoAnimData gan in land.Anim)
				{
					if (gan.Model != null)
						labels.AddRange(gan.Model.GetLabels());
					if (gan.Animation != null)
						labels.Add(gan.Animation.Name);
				}
			}
			return labels;
		}

		internal static List<string> GetLabels(this NJS_OBJECT obj)
		{
			List<string> labels = new List<string>() { obj.Name };
			if (obj.Attach != null)
				labels.AddRange(obj.Attach.GetLabels());
			if (obj.Children != null)
				foreach (NJS_OBJECT o in obj.Children)
					labels.AddRange(o.GetLabels());
			return labels;
		}

		internal static List<string> GetLabels(this Attach att)
		{
			List<string> labels = new List<string>() { att.Name };
			if (att is BasicAttach bas)
			{
				if (bas.VertexName != null)
					labels.Add(bas.VertexName);
				if (bas.NormalName != null)
					labels.Add(bas.NormalName);
				if (bas.MaterialName != null)
					labels.Add(bas.MaterialName);
				if (bas.MeshName != null)
					labels.Add(bas.MeshName);
			}
			else if (att is ChunkAttach cnk)
			{
				if (cnk.VertexName != null)
					labels.Add(cnk.VertexName);
				if (cnk.PolyName != null)
					labels.Add(cnk.PolyName);
			}
			return labels;
		}
	}
}