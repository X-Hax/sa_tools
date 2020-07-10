//Original SADXsnd by Tux/SANiK, SADXsndSharp by MainMemory
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

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
		bool is2010;
		bool unsaved;
		bool descending;
		private List<FENTRY> files;
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
				files = new List<FENTRY>();
			else
				LoadFile(args[1]);
		}

		private void LoadFile(string filename)
		{
			this.filename = Path.GetFullPath(filename);
			byte[] file = File.ReadAllBytes(filename);
			Text = "SADXsndSharp - Loading file, please wait...";
			this.Enabled = false;
			switch (System.Text.Encoding.ASCII.GetString(file, 0, 0x10))
			{
				case "archive  V2.2\0\0\0":
					is2010 = false;
					break;
				case "archive  V2.DMZ\0":
					is2010 = true;
					break;
				default:
					MessageBox.Show("Error: Unknown archive version/type");
					this.Enabled = true;
					return;
			}
			int count = BitConverter.ToInt32(file, 0x10);
			files = new List<FENTRY>(count);
			for (int i = 0; i < count; i++)
			{
				Text = $"SADXsndSharp - Loading file " + i.ToString() + " of " + count.ToString() + ", please wait...";
				files.Add(new FENTRY(file, 0x14 + (i * 0xC)));
			}
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
					Directory.CreateDirectory(a.FileName);
				string dir = Path.Combine(Path.GetDirectoryName(a.FileName), Path.GetFileName(a.FileName));
				using (StreamWriter sw = File.CreateText(Path.Combine(dir, "index.txt")))
				{
					List<FENTRY> list = new List<FENTRY>(files);
					list.Sort((f1, f2) => StringComparer.OrdinalIgnoreCase.Compare(f1.name, f2.name));
					foreach (FENTRY item in list)
					{
						Text = $"SADXsndSharp - Saving item " + list.IndexOf(item) + " of " + files.Count.ToString() + ", please wait...";
						sw.WriteLine(item.name);
						File.WriteAllBytes(Path.Combine(dir, item.name), Compress.ProcessBuffer(item.file));
					}
					sw.Flush();
					sw.Close();
				}
				Text = "SADXsndSharp - " + Path.GetFileName(filename);
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
					int i = files.Count;
					foreach (string item in a.FileNames)
					{
						files.Add(new FENTRY(item));
						i++;
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
					File.WriteAllBytes(a.FileName, Compress.ProcessBuffer(files[int.Parse(selectedItem.SubItems[2].Text)].file));
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedItem == null) return;
			int i = int.Parse(selectedItem.SubItems[2].Text);
			string fn = files[i].name;
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "wav",
				Filter = "WAV Files|*.wav|ADX Files|*.adx|All Files|*.*",
				FileName = fn
			})
				if (a.ShowDialog() == DialogResult.OK)
				{
					files[i] = new FENTRY(a.FileName);
					if (files[i].name != fn)
					{
						DialogResult mb = MessageBox.Show("Keep original filename " + fn + "?", "Keep filename?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if (mb == DialogResult.Yes) files[i].name = fn;
					}
					selectedItem.ForeColor = Compress.isFileCompressed(files[i].file) ? Color.Blue : Color.Black;
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
						files.Insert(i, new FENTRY(item));
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
			files.RemoveAt(i);
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
			foreach (FENTRY item in files)
			{
				if (item.name.Equals(e.Label, StringComparison.OrdinalIgnoreCase))
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
			files[int.Parse(listView1.Items[e.Item].SubItems[2].Text)].name = e.Label;
			RefreshListView(mainView);
			unsaved = true;
		}

		private void listView1_ItemActivate(object sender, EventArgs e)
		{
			string fp = Path.Combine(Path.GetTempPath(), files[int.Parse(listView1.SelectedItems[0].SubItems[2].Text)].name);
			File.WriteAllBytes(fp, Compress.ProcessBuffer(files[int.Parse(listView1.SelectedItems[0].SubItems[2].Text)].file));
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
			files = new List<FENTRY>();
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
				int i = files.Count;
				foreach (string item in dropfiles)
				{
					files.Add(new FENTRY(item));
					i++;
				}
				RefreshListView(mainView);
				unsaved = true;
			}
		}

		private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
		{
			string fn = Path.Combine(Path.GetTempPath(), files[int.Parse(listView1.SelectedItems[0].SubItems[2].Text)].name);
			File.WriteAllBytes(fn, Compress.ProcessBuffer(files[int.Parse(listView1.SelectedItems[0].SubItems[2].Text)].file));
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
					is2010 = saveAsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) > 0;
					saveToolStripMenuItem.Enabled = true;
					SaveFile();
				}
			}
		}

		private void SaveFile()
		{
			int fsize = 0x14;
			int hloc = fsize;
			fsize += files.Count * 0xC;
			int tloc = fsize;
			foreach (FENTRY item in files)
			{
				fsize += item.name.Length + 1;
			}
			int floc = fsize;
			foreach (FENTRY item in files)
			{
				fsize += item.file.Length;
			}
			byte[] file = new byte[fsize];
			System.Text.Encoding.ASCII.GetBytes(is2010 ? "archive  V2.DMZ" : "archive  V2.2").CopyTo(file, 0);
			BitConverter.GetBytes(files.Count).CopyTo(file, 0x10);
			foreach (FENTRY item in files)
			{
				BitConverter.GetBytes(tloc).CopyTo(file, hloc);
				hloc += 4;
				System.Text.Encoding.ASCII.GetBytes(item.name).CopyTo(file, tloc);
				tloc += item.name.Length + 1;
				BitConverter.GetBytes(floc).CopyTo(file, hloc);
				hloc += 4;
				item.file.CopyTo(file, floc);
				floc += item.file.Length;
				BitConverter.GetBytes(item.file.Length).CopyTo(file, hloc);
				hloc += 4;
			}
			File.WriteAllBytes(filename, file);
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
			for (int j = 0; j < files.Count; j++)
			{
				Text = $"SADXsndSharp - Loading item " + j.ToString() + " of " + files.Count.ToString() + ", please wait...";
				if (view == View.LargeIcon || view == View.Tile) imageList1.Images.Add(GetIcon(files[j].name, false));
				else imageList2.Images.Add(GetIcon(files[j].name, true));
				ListViewItem it = listView1.Items.Add(files[j].name, j);
				it.SubItems.Add(files[j].file.Length.ToString());
				it.SubItems.Add(j.ToString());
				it.ForeColor = Compress.isFileCompressed(files[j].file) ? Color.Blue : Color.Black;
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

		internal class FENTRY
		{
			public string name;
			public byte[] file;

			public FENTRY()
			{
				name = string.Empty;
			}

			public FENTRY(string fileName)
			{
				name = Path.GetFileName(fileName);
				file = File.ReadAllBytes(fileName);
			}

			public FENTRY(byte[] file, int address)
			{
				name = GetCString(file, BitConverter.ToInt32(file, address));
				this.file = new byte[BitConverter.ToInt32(file, address + 8)];
				Array.Copy(file, BitConverter.ToInt32(file, address + 4), this.file, 0, this.file.Length);
			}

			private string GetCString(byte[] file, int address)
			{
				int textsize = 0;
				while (file[address + textsize] > 0)
					textsize += 1;
				return System.Text.Encoding.ASCII.GetString(file, address, textsize);
			}
		}

		internal static class Compress
		{
			private static void DecompressBuffer(ref byte[] DecompressedBuffer, byte[] CompressedBuffer /*Starting at + 20*/)
			{
				int CompressedBufferPointer = 0;
				int DecompressedBufferPointer = 0;

				//Create sliding dictionary buffer and clear first 4078 bytes of dictionary buffer to 0
				byte[] SlidingDictionary = new byte[4096];

				//Set an offset to the dictionary insertion point
				uint DictionaryInsertionOffset = 4078;

				//Decompression command
				byte CommandCounter = 0;
				byte DecompressCommand = 0;

				while (DecompressedBufferPointer < DecompressedBuffer.Length)
				{
					//Is the decompress counter zero? Load the command
					if (CommandCounter == 0)
					{
						CommandCounter = 8; //Each command has 8 sub commands, one bit per command
						DecompressCommand = CompressedBuffer[CompressedBufferPointer];
						CompressedBufferPointer++;
					}

					//Each command is a byte and is actually composed of 8 sub commands
					//A bit of 1 means to copy the byte exactly as it is, and add it to the dictionary
					//A bit of 0 means to load a special encoded format describing a repetition.
					if ((DecompressCommand & 1) == 1)
					{
						//Copy the byte exactly over
						byte RawByte = CompressedBuffer[CompressedBufferPointer];
						CompressedBufferPointer++;
						DecompressedBuffer[DecompressedBufferPointer] = RawByte;
						DecompressedBufferPointer++;

						//Add the byte to the dictionary
						SlidingDictionary[DictionaryInsertionOffset] = RawByte;
						DictionaryInsertionOffset = (DictionaryInsertionOffset + 1) & 0xFFF; //Slide the dictionary

					}
					else
					{
						//The sub command tells us there is a repetition
						//unsigned short RepetitionCode=(CompressedBufferPointer[1] << 8) | CompressedBufferPointer[0];
						byte CurrentByte = CompressedBuffer[CompressedBufferPointer];
						byte NextByte = CompressedBuffer[CompressedBufferPointer + 1]; //Lower nibble is the repetition count or RunLength
						CompressedBufferPointer += 2;

						//Calculate the offset of the byte to use in the sliding dictionary
						int DictionaryOffset = ((NextByte & 0xF0) << 4) | CurrentByte;

						//It is not really run length compression, but instead it is a dictionary based method
						//I just ran out of ideas what to name the variables
						int RunCounter = 0;
						int RunLength = (NextByte & 0xF) + 3; //Compression defines a repetition to be at minimum three bytes

						while (RunCounter < RunLength)
						{
							byte RawByte = SlidingDictionary[((RunCounter + DictionaryOffset) & 0xFFF)];
							DecompressedBuffer[DecompressedBufferPointer] = RawByte;
							DecompressedBufferPointer++;

							if (DecompressedBufferPointer >= DecompressedBuffer.Length)
								return;

							//Add the byte to the dictionary
							SlidingDictionary[DictionaryInsertionOffset] = RawByte;
							DictionaryInsertionOffset = (DictionaryInsertionOffset + 1) & 0xFFF; //Slide the dictionary

							RunCounter++;
						}
					}

					//Rotate the sub command
					CommandCounter--;
					DecompressCommand >>= 1;
				}
			}

			public static bool isFileCompressed(byte[] CompressedBuffer)
			{
				return System.Text.Encoding.ASCII.GetString(CompressedBuffer, 0, 13) == "compress v1.0";
			}

			public static byte[] ProcessBuffer(byte[] CompressedBuffer)
			{
				if (isFileCompressed(CompressedBuffer))
				{
					uint DecompressedSize = BitConverter.ToUInt32(CompressedBuffer, 16);
					byte[] DecompressedBuffer = new byte[DecompressedSize];
					//Xor Decrypt the whole buffer
					byte XorEncryptionValue = CompressedBuffer[15];

					byte[] CompBuf = new byte[CompressedBuffer.Length - 20];
					for (int i = 20; i < CompressedBuffer.Length; i++)
					{
						CompBuf[i - 20] = (byte)(CompressedBuffer[i] ^ XorEncryptionValue);
					}

					//Decompress the whole buffer
					DecompressBuffer(ref DecompressedBuffer, CompBuf);

					//Switch the buffers around so the decompressed one gets saved instead
					return DecompressedBuffer;
				}
				else
				{
					return CompressedBuffer;
				}
			}

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
