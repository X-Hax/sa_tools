// Original SADXsnd by Tux/SANiK, SADXsndSharp by MainMemory
using ArchiveLib;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using static ArchiveLib.GenericArchive;

namespace SADXsndSharp
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		/// <summary>Currently opened archive file.</summary>
		private GenericArchive archiveFile;
		/// <summary>Filename of the currently opened archive file.</summary>
		string fileName;
		/// <summary>Buffer used in renaming archive entries.</summary>
		private string oldName;
		/// <summary>This value is true if there are unsaved changes.</summary>
		bool unsavedChanges;

		/// <summary>Current view mode of the ListView.</summary>
		View mainView = View.Details;
		/// <summary>Current selected item in the ListView.</summary>
		ListViewItem selectedItem;
		/// <summary>This value is true if the ListView's items should be sorted in descending order.</summary>
		bool descendingOrder;
		/// <summary>Used to sort ListView columns.</summary>
		private ListViewColumnSorter lvwColumnSorter;
		/// <summary>Index of the column used for sorting. Can be 0 (filename), 1 (size), or 2 (entry index in the archive).</summary>
		private int selectedColumn = 2;
		/// <summary>Indicates whether the drag-and-drop was initiated from the inside of the window (to prevent accidental drag onto itself)..</summary>
		private bool draggingFromInside;

		/// <summary>
		/// This function loads and returns the appropriate archive format.
		/// Update it when adding new formats to ArchiveLib.
		/// </summary>
		/// <param name="filename">Path to the file to load.</param>
		/// <returns>AFSFile, DATFile etc.</returns>
		private GenericArchive IdentifyAndLoadArchive(string filename)
		{
			byte[] file = File.ReadAllBytes(filename);
			switch (Path.GetExtension(filename).ToLowerInvariant())
			{
				case ".afs":
					return new AFSFile(file);
				case ".arcx":
					return new ARCXFile(file);
				case ".dat":
					return new DATFile(file);
				case ".gcaxmlt":
					return new gcaxMLTFile(file);
				case ".kat":
					return new KATFile(file);
				case ".mlt":
					return new MLTFile(file);
				case ".nj":
					return new NinjaBinaryFile(file);
				case ".pak":
					return new PAKFile(filename);
				case ".pb":
					return new PBFile(file);
				case ".pvm":
				case ".gvm":
				case ".xvm":
					return new PuyoFile(file);
				case ".pvmx":
					return new PVMXFile(file);
				case ".mld":
					return new MLDArchive(filename, file);
				case ".mdl":
					return new MDLArchive(file);
				case ".mdt":
					return new MDTArchive(file);
				default:
					return null;
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			FormClosing += Form1_FormClosing;
			if (Program.Arguments.Length > 0)
				LoadFile(Program.Arguments[0]);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (unsavedChanges)
			{
				switch (ShowSaveChangesDialog())
				{
					case DialogResult.Yes:
						SaveFile();
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						return;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// This function retrieves the index of the currently selected archive entry.
		/// </summary>
		/// <returns>Index value, or -1 if there is no selection.</returns>
		private int GetSelectedItemID()
		{
			return selectedItem == null ? -1 : int.Parse(selectedItem.SubItems[2].Text);
		}

		/// <summary>
		/// This function handles archive loading.
		/// </summary>
		/// <param name="filename">Path to the file to load.</param>
		private void LoadFile(string filename)
		{
			GenericArchive result = IdentifyAndLoadArchive(filename);
			// Check if the archive loaded successfully
			if (result == null)
			{
				MessageBox.Show(this, "Error loading file " + Path.GetFileName(filename) + ": Unsupported archive format.", "SADXSndSharp", MessageBoxButtons.OK, MessageBoxIcon.Error);
				buttonAdd.Enabled = buttonRemove.Enabled = buttonMoveUp.Enabled = buttonMoveDown.Enabled = false;
				return;
			}
			// Global stuff
			archiveFile = result;
			fileName = Path.GetFullPath(filename);
			// Load the items, disabling the form during loading
			Text = "SADXsndSharp - Loading file, please wait...";
			Enabled = false;
			RefreshListView(mainView);
			// Enable the form and set text
			Text = "SADXsndSharp - " + Path.GetFileName(filename);
			Enabled = true;
			saveToolStripMenuItem.Enabled = true;
			unsavedChanges = false;
			buttonAdd.Enabled = true;
		}

		/// <summary>
		/// This function retrieves the specified entry as a byte array.
		/// </summary>
		/// <param name="index">Entry ID</param>
		/// <returns>Byte array of the entry</returns>
		private byte[] GetFile(int index)
		{
			// DAT entries can be compressed, so they are dealt with separately
			if (archiveFile is DATFile dat)
				return dat.GetFile(index);
			// Regular archives can just fetch the entry data directly
			else
				return archiveFile.Entries[index].Data;
		}

		/// <summary>Moves the specified entry upwards in the archive (reduces index by 1).</summary>
		/// <param name="i">Index of the entry to move</param>
		private void MoveUp(int i)
		{
			GenericArchiveEntry ti = archiveFile.Entries[i];
			archiveFile.Entries.RemoveAt(i);
			archiveFile.Entries.Insert(i - 1, ti);
		}

		/// <summary>Moves the specified entry downwards in the archive (increases index by 1).</summary>
		/// <param name="i">Index of the entry to move</param>
		private void MoveDown(int i)
		{
			GenericArchiveEntry ti = archiveFile.Entries[i];
			archiveFile.Entries.RemoveAt(i);
			archiveFile.Entries.Insert(i + 1, ti);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (unsavedChanges)
			{
				switch (ShowSaveChangesDialog())
				{
					case DialogResult.Yes:
						SaveFile();
						break;
					case DialogResult.Cancel:
						return;
					default:
						break;
				}
			}
			using (OpenFileDialog a = new OpenFileDialog()
			{
				// TODO: Add filters for all supported extensions
				DefaultExt = "dat",
				Filter = "DAT Files|*.dat|All Supported Files|*.afs;*.arcx;*.dat;*.gcaxMLT;*.kat;*.mlt;*.nj;*.pak;*.pb;*.pvm;*.gvm;*.xvm;*.mld;*.mdl;*.mdt|All Files|*.*"
			})
				if (a.ShowDialog() == DialogResult.OK)
					LoadFile(a.FileName);
		}

		private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog() { DefaultExt = "", Filter = "", FileName = Path.GetFileNameWithoutExtension(fileName) })
			{
				if (fileName != null)
				{
					a.InitialDirectory = Path.GetDirectoryName(fileName);
					a.FileName = Path.GetFileNameWithoutExtension(fileName);
				}

				if (a.ShowDialog(this) == DialogResult.OK)
				{
					Directory.CreateDirectory(a.FileName);
					string dir = Path.Combine(Path.GetDirectoryName(a.FileName), Path.GetFileName(a.FileName));
					archiveFile.CreateIndexFile(dir);
					for (int i = 0; i < archiveFile.Entries.Count; i++)
					{
						string outPath = dir;
						Text = $"SADXsndSharp - Saving item " + i.ToString() + " of " + archiveFile.Entries.Count.ToString() + ", please wait...";
						byte[] save = GetFile(i);
						if (IsAdx(save) && exportADXAsWAVToolStripMenuItem.Checked)
							save = AdxToWav(save);
						// Add ARCX pathname
						if (archiveFile is ARCXFile arcx)
						{
							ARCXFile.ARCXEntry entry = (ARCXFile.ARCXEntry)arcx.Entries[i];
							outPath = Path.Combine(dir, entry.Folder);
							if (!Directory.Exists(outPath))
								Directory.CreateDirectory(outPath);
						}
						File.WriteAllBytes(Path.Combine(outPath, archiveFile.Entries[i].Name), save);
					}
					Text = "SADXsndSharp - " + Path.GetFileName(fileName);
				}
				else return;
			}
		}

		// Item context menu
		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && selectedItem != null)
				contextMenuStrip1.Show(listView1, e.Location);
		}

		/// <summary>Adds new files to the archive.</summary>
		private void AddFiles()
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
						GenericArchiveEntry entry = archiveFile.NewEntry();
						entry.Name = Path.GetFileName(item);
						entry.Data = File.ReadAllBytes(item);
					}
					RefreshListView(mainView);
					unsavedChanges = true;
				}
		}

		/// <summary>Deletes the currently selected archive entry.</summary>
		private void DeleteItem()
		{
			int i = GetSelectedItemID();
			if (i == -1)
				return;
			archiveFile.Entries.RemoveAt(i);
			RefreshListView(mainView);
			unsavedChanges = true;
		}

		private void ExtractEntry()
		{

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
				{
					byte[] save = GetFile(GetSelectedItemID());
					if (IsAdx(save) && exportADXAsWAVToolStripMenuItem.Checked)
						save = AdxToWav(save);
					File.WriteAllBytes(a.FileName, save);
				}
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedItem == null) return;
			int i = GetSelectedItemID();
			string fn = archiveFile.Entries[i].Name;
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "wav",
				Filter = "WAV Files|*.wav|ADX Files|*.adx|All Files|*.*",
				FileName = fn
			})
				if (a.ShowDialog() == DialogResult.OK)
				{
					if (archiveFile.Entries[i].Name != a.FileName)
					{
						DialogResult mb = MessageBox.Show("Keep original filename " + fn + "?", "Keep filename?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if (mb == DialogResult.No)
							archiveFile.Entries[i].Name = Path.GetFileName(a.FileName);
					}
					archiveFile.Entries[i].Data = File.ReadAllBytes(a.FileName);
					selectedItem.ForeColor = (archiveFile is DATFile dat && dat.IsFileCompressed(i)) ? Color.Blue : Color.Black;
					unsavedChanges = true;
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
					int i = GetSelectedItemID();
					foreach (string item in a.FileNames)
					{
						GenericArchiveEntry entry = archiveFile.NewEntry();
						entry.Name = Path.GetFileName(item);
						entry.Data = File.ReadAllBytes(item);
						archiveFile.Entries.Add(entry);
						i++;
					}
					RefreshListView(mainView);
					unsavedChanges = true;
				}
		}

		private void listView1_BeforeLabelEdit(object sender, LabelEditEventArgs e)
		{
			oldName = listView1.Items[e.Item].Text;
		}

		private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (oldName == e.Label) return;
			for (int i = 0; i < archiveFile.Entries.Count; i++)
			{
				if (archiveFile.Entries[i].Name.Equals(e.Label, StringComparison.OrdinalIgnoreCase))
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
			archiveFile.Entries[GetSelectedItemID()].Name = e.Label;
			RefreshListView(mainView);
			unsavedChanges = true;
		}

		private void listView1_ItemActivate(object sender, EventArgs e)
		{
			int id = GetSelectedItemID();
			string fp = Path.Combine(Path.GetTempPath(), archiveFile.Entries[id].Name);
			byte[] save = GetFile(id);
			if (IsAdx(save) && exportADXAsWAVToolStripMenuItem.Checked)
				save = AdxToWav(save);
			File.WriteAllBytes(fp, save);
			System.Diagnostics.Process.Start("explorer.exe", fp);
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (unsavedChanges)
			{
				switch (ShowSaveChangesDialog())
				{
					case DialogResult.Yes:
						SaveFile();
						break;
					case DialogResult.Cancel:
						return;
					default:
						break;
				}
			}
			fileName = null;
			Text = "SADXsndSharp";
			archiveFile = new DATFile();
			RefreshListView(mainView);
			saveToolStripMenuItem.Enabled = false;
			unsavedChanges = false;
		}

		private void listView1_DragEnter(object sender, DragEventArgs e)
		{
			if (draggingFromInside)
			{
				e.Effect = DragDropEffects.None;
				draggingFromInside = false;
		}
			else if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.All;
		}

		private void listView1_DragDrop(object sender, DragEventArgs e)
		{
			if (!draggingFromInside && e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] dropfiles = (string[])e.Data.GetData(DataFormats.FileDrop, true);
				int i = archiveFile.Entries.Count;
				foreach (string item in dropfiles)
				{
					GenericArchiveEntry entry = archiveFile.NewEntry();
					entry.Name = Path.GetFileName(item);
					entry.Data = File.ReadAllBytes(item);
					archiveFile.Entries.Add(entry);
					i++;
				}
				RefreshListView(mainView);
				unsavedChanges = true;
			}
		}

		private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
		{
			draggingFromInside = true;
			int id = GetSelectedItemID();
			string fn = Path.Combine(Path.GetTempPath(), archiveFile.Entries[id].Name);
			File.WriteAllBytes(fn, GetFile(id));
			DoDragDrop(new DataObject(DataFormats.FileDrop, new string[] { fn }), DragDropEffects.Copy);
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFile();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			fileToolStripMenuItem.DropDown.Close();
			using (SaveFileDialog a = new SaveFileDialog()
			{
				DefaultExt = "dat",
				Filter = "DAT Files|*.dat|All Files|*.*"
			})
			{
				if (fileName != null)
					a.FileName = Path.GetFileName(fileName);
				if (a.ShowDialog() == DialogResult.OK)
				{
					fileName = a.FileName;
					Text = "SADXsndSharp - " + Path.GetFileName(a.FileName);
					if (archiveFile is DATFile dat)
					{
						dat.Steam = useThe2010FormatForDATToolStripMenuItem.Checked;
					}
					saveToolStripMenuItem.Enabled = true;
					SaveFile();
				}
			}
		}

		private void SaveFile()
		{
			File.WriteAllBytes(fileName, archiveFile.GetBytes());
			unsavedChanges = false;
		}

		public bool IsAdx(byte[] data)
		{
			return (BitConverter.ToUInt16(data, 0) == 0x0080 && data[4] == 0x03 && (data[18] == 0x03 || data[18] == 0x04));
		}

		private Icon GetIcon(string file, bool smol)
		{
			if (smol) return IconTools.GetIconForExtension(Path.GetExtension(file), ShellIconSize.SmallIcon);
			else return IconTools.GetIconForExtension(Path.GetExtension(file), ShellIconSize.LargeIcon);
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private DialogResult ShowSaveChangesDialog()
		{
			return MessageBox.Show("There are unsaved changes. Would you like to save the file?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
		}

		private void RefreshListView(View view)
		{
			listView1.Items.Clear();
			bool addimg1 = imageList1.Images.Empty;
			bool addimg2 = imageList2.Images.Empty;
			listView1.BeginUpdate();
			for (int j = 0; j < archiveFile.Entries.Count; j++)
			{
				Text = $"SADXsndSharp - Loading item " + j.ToString() + " of " + archiveFile.Entries.Count.ToString() + ", please wait...";
				if (addimg1 && (view == View.LargeIcon || view == View.Tile)) 
					imageList1.Images.Add(GetIcon(archiveFile.Entries[j].Name, false));
				else if (addimg2 && (view == View.SmallIcon || view == View.Details || view == View.List))
					imageList2.Images.Add(GetIcon(archiveFile.Entries[j].Name, true));
				ListViewItem it = listView1.Items.Add(archiveFile.Entries[j].Name, j);
				it.SubItems.Add((view == View.Tile ? "Size: " : "") + archiveFile.Entries[j].Data.Length.ToString());
				it.SubItems.Add((view == View.Tile ? "Index: " : "") + j.ToString());
				it.ForeColor = (archiveFile is DATFile dat && dat.IsFileCompressed(j)) ? Color.Blue : Color.Black;
			}
			listView1.View = view;
			listView1.EndUpdate();
			Text = "SADXsndSharp - " + Path.GetFileName(fileName);
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
			selectedColumn = e.Column;
			SortListViewItems();
		}

		private void SortListViewItems()
		{
			lvwColumnSorter = new ListViewColumnSorter();
			listView1.ListViewItemSorter = lvwColumnSorter;
			lvwColumnSorter.SortColumn = selectedColumn;
			// Set the column number that is to be sorted; default to ascending.
			if (descendingOrder)
				lvwColumnSorter.Order = SortOrder.Descending;
			else
				lvwColumnSorter.Order = SortOrder.Ascending;
			// Perform the sort with these new sort options.
			listView1.Sort();
			listView1.ListViewItemSorter = null;
		}

		private void ascendingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ascendingToolStripMenuItem.Checked = true;
			descendingToolStripMenuItem.Checked = false;
			descendingOrder = false;
			SortListViewItems();
		}

		private void descendingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ascendingToolStripMenuItem.Checked = false;
			descendingToolStripMenuItem.Checked = true;
			descendingOrder = true;
			SortListViewItems();
		}

		private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddFiles();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteItem();
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			AddFiles();
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			DeleteItem();
		}

		private void buttonMoveUp_Click(object sender, EventArgs e)
		{
			int i = GetSelectedItemID();
			if (i == -1)
				return;
			MoveUp(i);
			RefreshListView(mainView);
			listView1.Items[i - 1].Focused = listView1.Items[i - 1].Selected = true;
			listView1.Focus();
			unsavedChanges = true;
		}

		private void buttonMoveDown_Click(object sender, EventArgs e)
		{
			int i = GetSelectedItemID();
			if (i == -1)
				return;
			MoveDown(i);
			RefreshListView(mainView);
			listView1.Items[i + 1].Focused = listView1.Items[i + 1].Selected = true;
			listView1.Focus();
			unsavedChanges = true;
		}

		private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				selectedItem = listView1.SelectedItems[0];
				buttonRemove.Enabled = true;
				buttonMoveUp.Enabled = GetSelectedItemID() > 0;
				buttonMoveDown.Enabled = GetSelectedItemID() < archiveFile.Entries.Count - 1;
			}
			else
			{
				selectedItem = null;
				buttonRemove.Enabled = buttonMoveUp.Enabled = buttonMoveDown.Enabled = false;
			}
		}
	}
}