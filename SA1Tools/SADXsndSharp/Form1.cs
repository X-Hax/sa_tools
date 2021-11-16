//Original SADXsnd by Tux/SANiK, SADXsndSharp by MainMemory
using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using ArchiveLib;
using static ArchiveLib.DATFile;

namespace SADXsndSharp
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private ListViewColumnSorter lvwColumnSorter;
		string filename;
		bool unsaved;
		bool descending;
		private DATFile archive;
		View mainView = View.Details;

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (unsaved)
			{
				DialogResult result = ShowSaveChangesDialog();
				if (result == DialogResult.Yes) SaveFile();
				else if (result == DialogResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.FormClosing += Form1_FormClosing;
			string[] args = Environment.GetCommandLineArgs();
			if (args.Length == 1)
				archive = new DATFile();
			else
				LoadFile(args[1]);
		}

		private void LoadFile(string filename)
		{
			this.filename = Path.GetFullPath(filename);
			byte[] file = File.ReadAllBytes(filename);
			Text = "SADXsndSharp - Loading file, please wait...";
			this.Enabled = false;
			archive = new DATFile(file);
			RefreshListView(mainView);
			Text = "SADXsndSharp - " + Path.GetFileName(filename);
			this.Enabled = true;
			saveToolStripMenuItem.Enabled = true;
			unsaved = false;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (unsaved)
			{
				DialogResult result = ShowSaveChangesDialog();
				if (result == DialogResult.Yes) SaveFile();
				else if (result == DialogResult.Cancel) return;
			}
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "dat",
				Filter = "DAT Files|*.dat|All Files|*.*"
			})
				if (a.ShowDialog() == DialogResult.OK)
					LoadFile(a.FileName);
		}

		private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog() { DefaultExt = "", Filter = "", FileName = "soundpack" })
			{
				if (filename != null)
				{
					a.InitialDirectory = Path.GetDirectoryName(filename);
					a.FileName = Path.GetFileNameWithoutExtension(filename);
				}

				if (a.ShowDialog(this) == DialogResult.OK)
				{
					Directory.CreateDirectory(a.FileName);
					string dir = Path.Combine(Path.GetDirectoryName(a.FileName), Path.GetFileName(a.FileName));
					using (StreamWriter sw = File.CreateText(Path.Combine(dir, "index.txt")))
					{
						archive.Entries.Sort((f1, f2) => StringComparer.OrdinalIgnoreCase.Compare(f1.Name, f2.Name));
						for (int i = 0; i < archive.Entries.Count; i++)
						{
							Text = $"SADXsndSharp - Saving item " + i.ToString() + " of " + archive.Entries.Count.ToString() + ", please wait...";
							sw.WriteLine(archive.Entries[i].Name);
							File.WriteAllBytes(Path.Combine(dir, archive.Entries[i].Name), archive.GetFile(i));
						}
						sw.Flush();
						sw.Close();
					}
					Text = "SADXsndSharp - " + Path.GetFileName(filename);
				}
				else return;
			}
		}

		ListViewItem selectedItem;
		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				selectedItem = listView1.GetItemAt(e.X, e.Y);
				if (selectedItem != null)
				{
					contextMenuStrip1.Show(listView1, e.Location);
				}
			}
		}

		private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "wav",
				Filter = "WAV Files|*.wav|ADX Files|*.adx|All Files|*.*",
				Multiselect = true
			})
				if (a.ShowDialog() == DialogResult.OK)
				{
					foreach (string item in a.FileNames)
					{
						archive.Entries.Add(new DATEntry(item));
					}
					RefreshListView(mainView);
					unsaved = true;
				}
		}

		private void extractToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedItem == null) return;
			using (SaveFileDialog a = new SaveFileDialog()
			{
				DefaultExt = "wav",
				Filter = "WAV Files|*.wav|ADX Files|*.adx|All Files|*.*",
				FileName = selectedItem.Text
			})
				if (a.ShowDialog() == DialogResult.OK)
					File.WriteAllBytes(a.FileName, archive.GetFile(int.Parse(selectedItem.SubItems[2].Text)));
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedItem == null) return;
			int i = int.Parse(selectedItem.SubItems[2].Text);
			string fn = archive.Entries[i].Name;
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "wav",
				Filter = "WAV Files|*.wav|ADX Files|*.adx|All Files|*.*",
				FileName = fn
			})
				if (a.ShowDialog() == DialogResult.OK)
				{
					if (archive.Entries[i].Name != a.FileName)
					{
						DialogResult mb = MessageBox.Show("Keep original filename " + fn + "?", "Keep filename?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if (mb == DialogResult.Yes)
							archive.Entries[i].Data=File.ReadAllBytes(a.FileName);
						else archive.Entries[i] = new DATEntry(a.FileName);
					}
					else
						archive.ReplaceFile(a.FileName, i);
					selectedItem.ForeColor = archive.IsFileCompressed(i) ? Color.Blue : Color.Black;
					unsaved = true;
					RefreshListView(mainView);
				}
		}

		private void insertToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedItem == null) return;
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "wav",
				Filter = "WAV Files|*.wav|ADX Files|*.adx|All Files|*.*",
				Multiselect = true
			})
				if (a.ShowDialog() == DialogResult.OK)
				{
					int i = int.Parse(selectedItem.SubItems[2].Text);
					foreach (string item in a.FileNames)
					{
						archive.Entries.Add(new DATEntry(item));
						i++;
					}
					RefreshListView(mainView);
					unsaved = true;
				}
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedItem == null) return;
			int i = int.Parse(selectedItem.SubItems[2].Text);
			archive.Entries.RemoveAt(i);
			RefreshListView(mainView);
			unsaved = true;
		}

		private string oldName;
		private void listView1_BeforeLabelEdit(object sender, LabelEditEventArgs e)
		{
			oldName = e.Label;
		}

		private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (oldName == e.Label) return;
			for (int i = 0; i < archive.Entries.Count; i++)
			{
				if (archive.Entries[i].Name.Equals(e.Label, StringComparison.OrdinalIgnoreCase))
				{
					e.CancelEdit = true;
					MessageBox.Show("This name is being used by another file.");
					return;
				}
			}
			if (e.Label.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
			{
				e.CancelEdit = true;
				MessageBox.Show("This name contains invalid characters.");
				return;
			}
			archive.Entries[int.Parse(listView1.Items[e.Item].SubItems[2].Text)].Name = e.Label;
			RefreshListView(mainView);
			unsaved = true;
		}

		private void listView1_ItemActivate(object sender, EventArgs e)
		{
			string fp = Path.Combine(Path.GetTempPath(), archive.Entries[int.Parse(listView1.SelectedItems[0].SubItems[2].Text)].Name);
			File.WriteAllBytes(fp, archive.GetFile(int.Parse(listView1.SelectedItems[0].SubItems[2].Text)));
			System.Diagnostics.Process.Start(fp);
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (unsaved)
			{
				DialogResult result = ShowSaveChangesDialog();
				if (result == DialogResult.Yes) SaveFile();
				else if (result == DialogResult.Cancel) return;
			}
			filename = null;
			Text = "SADXsndSharp";
			archive = new DATFile();
			RefreshListView(mainView);
			saveToolStripMenuItem.Enabled = false;
			unsaved = false;
		}

		private void listView1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.All;
		}

		private void listView1_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] dropfiles = (string[])e.Data.GetData(DataFormats.FileDrop, true);
				int i = archive.Entries.Count;
				foreach (string item in dropfiles)
				{
					archive.Entries.Add(new DATEntry(item));
					i++;
				}
				RefreshListView(mainView);
				unsaved = true;
			}
		}

		private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
		{
			string fn = Path.Combine(Path.GetTempPath(), archive.Entries[int.Parse(listView1.SelectedItems[0].SubItems[2].Text)].Name);
			File.WriteAllBytes(fn, archive.GetFile(int.Parse(listView1.SelectedItems[0].SubItems[2].Text)));
			DoDragDrop(new DataObject(DataFormats.FileDrop, new string[] { fn }), DragDropEffects.All);
			unsaved = true;
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFile();
		}

		private void saveAsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			fileToolStripMenuItem.DropDown.Close();
			using (SaveFileDialog a = new SaveFileDialog()
			{
				DefaultExt = "dat",
				Filter = "DAT Files|*.dat|All Files|*.*"
			})
			{
				if (filename != null)
					a.FileName = Path.GetFileName(filename);
				if (a.ShowDialog() == DialogResult.OK)
				{
					filename = a.FileName;
					Text = "SADXsndSharp - " + Path.GetFileName(a.FileName);
					archive.Steam = saveAsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) > 0;
					saveToolStripMenuItem.Enabled = true;
					SaveFile();
				}
			}
		}

		private void SaveFile()
		{
			File.WriteAllBytes(filename, archive.GetBytes());
			unsaved = false;
		}

		private Icon GetIcon(string file, bool smol)
		{
			if (smol) return IconTools.GetIconForExtension(Path.GetExtension(file), ShellIconSize.SmallIcon);
			else return IconTools.GetIconForExtension(Path.GetExtension(file), ShellIconSize.LargeIcon);
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (unsaved)
			{
				DialogResult result = ShowSaveChangesDialog();
				if (result == DialogResult.Yes) SaveFile();
				else if (result == DialogResult.Cancel) return;
			}
			Close();
		}

		private DialogResult ShowSaveChangesDialog()
		{
			return MessageBox.Show("There are unsaved changes. Would you like to save the file?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
		}

		private void RefreshListView(View view)
		{
			listView1.Items.Clear();
			imageList1.Images.Clear();
			imageList2.Images.Clear();
			listView1.BeginUpdate();
			for (int j = 0; j < archive.Entries.Count; j++)
			{
				Text = $"SADXsndSharp - Loading item " + j.ToString() + " of " + archive.Entries.Count.ToString() + ", please wait...";
				if (view == View.LargeIcon || view == View.Tile) imageList1.Images.Add(GetIcon(archive.Entries[j].Name, false));
				else imageList2.Images.Add(GetIcon(archive.Entries[j].Name, true));
				ListViewItem it = listView1.Items.Add(archive.Entries[j].Name, j);
				it.SubItems.Add(archive.Entries[j].Data.Length.ToString());
				it.SubItems.Add(j.ToString());
				it.ForeColor = archive.IsFileCompressed(j) ? Color.Blue : Color.Black;
			}
			listView1.View = view;
			listView1.EndUpdate();
			Text = "SADXsndSharp - " + Path.GetFileName(filename);
		}

		private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			largeIconsToolStripMenuItem.Checked = true;
			detailsToolStripMenuItem.Checked = false;
			tilesToolStripMenuItem.Checked = false;
			smallIconsToolStripMenuItem.Checked = false;
			mainView = View.LargeIcon;
			RefreshListView(mainView);
		}

		private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			smallIconsToolStripMenuItem.Checked = true;
			largeIconsToolStripMenuItem.Checked = false;
			detailsToolStripMenuItem.Checked = false;
			tilesToolStripMenuItem.Checked = false;
			mainView = View.SmallIcon;
			RefreshListView(mainView);
		}

		private void tilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tilesToolStripMenuItem.Checked = true;
			largeIconsToolStripMenuItem.Checked = false;
			detailsToolStripMenuItem.Checked = false;
			smallIconsToolStripMenuItem.Checked = false;
			tilesToolStripMenuItem.Checked = true;
			mainView = View.Tile;
			RefreshListView(mainView);
		}

		private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			detailsToolStripMenuItem.Checked = true;
			largeIconsToolStripMenuItem.Checked = false;
			tilesToolStripMenuItem.Checked = false;
			smallIconsToolStripMenuItem.Checked = false;
			mainView = View.Details;
			RefreshListView(mainView);
		}

		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			lvwColumnSorter = new ListViewColumnSorter();
			this.listView1.ListViewItemSorter = lvwColumnSorter;
			// Set the column number that is to be sorted; default to ascending.
			lvwColumnSorter.SortColumn = e.Column;
			if (descending) lvwColumnSorter.Order = SortOrder.Descending; else lvwColumnSorter.Order = SortOrder.Ascending;
			// Perform the sort with these new sort options.
			this.listView1.Sort();
			this.listView1.ListViewItemSorter = null;
		}

		private void ascendingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ascendingToolStripMenuItem.Checked = true;
			descendingToolStripMenuItem.Checked = false;
			descending = false;
		}

		private void descendingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ascendingToolStripMenuItem.Checked = false;
			descendingToolStripMenuItem.Checked = true;
			descending = true;
		}

	}
}
