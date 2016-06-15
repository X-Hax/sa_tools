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

		string filename;
		bool is2010;
        private List<FENTRY> files;

        private void Form1_Load(object sender, EventArgs e)
        {
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
                    return;
            }
            int count = BitConverter.ToInt32(file, 0x10);
            files = new List<FENTRY>(count);
            listView1.Items.Clear();
            imageList1.Images.Clear();
            listView1.BeginUpdate();
            for (int i = 0; i < count; i++)
            {
                files.Add(new FENTRY(file, 0x14 + (i * 0xC)));
                imageList1.Images.Add(GetIcon(files[i].name));
                ListViewItem it = listView1.Items.Add(files[i].name, i);
                it.ForeColor = Compress.isFileCompressed(files[i].file) ? Color.Blue : Color.Black;
            }
            listView1.EndUpdate();
			Text = "SADXsndSharp - " + Path.GetFileName(filename);
			saveToolStripMenuItem.Enabled = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
			using (FolderBrowserDialog a = new FolderBrowserDialog() { ShowNewFolderButton = true })
			{
				if (filename != null)
					a.SelectedPath = Path.GetDirectoryName(filename);
				if (a.ShowDialog(this) == DialogResult.OK)
					using (StreamWriter sw = File.CreateText(Path.Combine(a.SelectedPath, "index.txt")))
					{
						List<FENTRY> list = new List<FENTRY>(files);
						list.Sort((f1, f2) => StringComparer.OrdinalIgnoreCase.Compare(f1.name, f2.name));
						foreach (FENTRY item in list)
						{
							sw.WriteLine(item.name);
							File.WriteAllBytes(Path.Combine(a.SelectedPath, item.name), Compress.ProcessBuffer(item.file));
						}
						sw.Flush();
						sw.Close();
					}
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
						imageList1.Images.Add(GetIcon(files[i].name));
						ListViewItem it = listView1.Items.Add(files[i].name, i);
						it.ForeColor = Compress.isFileCompressed(files[i].file) ? Color.Blue : Color.Black;
						i++;
					}
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
					File.WriteAllBytes(a.FileName, Compress.ProcessBuffer(files[listView1.Items.IndexOf(selectedItem)].file));
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem == null) return;
            int i = listView1.Items.IndexOf(selectedItem);
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
					files[i].name = fn;
					selectedItem.ForeColor = Compress.isFileCompressed(files[i].file) ? Color.Blue : Color.Black;
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
					int i = listView1.Items.IndexOf(selectedItem);
					foreach (string item in a.FileNames)
					{
						files.Insert(i, new FENTRY(item));
						i++;
					}
					listView1.Items.Clear();
					imageList1.Images.Clear();
					listView1.BeginUpdate();
					for (int j = 0; j < files.Count; j++)
					{
						imageList1.Images.Add(GetIcon(files[j].name));
						ListViewItem it = listView1.Items.Add(files[j].name, j);
						it.ForeColor = Compress.isFileCompressed(files[j].file) ? Color.Blue : Color.Black;
					}
					listView1.EndUpdate();
				}
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem == null) return;
            int i = listView1.Items.IndexOf(selectedItem);
            files.RemoveAt(i);
            listView1.Items.RemoveAt(i);
            imageList1.Images.RemoveAt(i);
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
            files[e.Item].name = e.Label;
            imageList1.Images[e.Item] = GetIcon(e.Label).ToBitmap();
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            string fp = Path.Combine(Path.GetTempPath(), files[listView1.SelectedIndices[0]].name);
            File.WriteAllBytes(fp, Compress.ProcessBuffer(files[listView1.SelectedIndices[0]].file));
            System.Diagnostics.Process.Start(fp);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
			filename = null;
			Text = "SADXsndSharp";
            files = new List<FENTRY>();
            listView1.Items.Clear();
            imageList1.Images.Clear();
			saveToolStripMenuItem.Enabled = false;
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
                    imageList1.Images.Add(GetIcon(files[i].name));
                    ListViewItem it = listView1.Items.Add(files[i].name, i);
                    it.ForeColor = Compress.isFileCompressed(files[i].file) ? Color.Blue : Color.Black;
                    i++;
                }
            }
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            string fn = Path.Combine(Path.GetTempPath(), files[listView1.SelectedIndices[0]].name);
            File.WriteAllBytes(fn, Compress.ProcessBuffer(files[listView1.SelectedIndices[0]].file));
            DoDragDrop(new DataObject(DataFormats.FileDrop, new string[] { fn }), DragDropEffects.All);
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
		}

        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        private static extern IntPtr ExtractIconA(int hInst, string lpszExeFileName, int nIconIndex);

        private Icon GetIcon(string file)
        {
            string iconpath = "C:\\Windows\\system32\\shell32.dll,0";
            Microsoft.Win32.RegistryKey k = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(file.IndexOf('.') > -1 ? file.Substring(file.LastIndexOf('.')) : file);
            if (k == null)
                k = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("*");
            k = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey((string)k.GetValue("", "*"));
            if (k != null)
            {
                k = k.OpenSubKey("DefaultIcon");
                if (k != null)
                    iconpath = (string)k.GetValue("", "C:\\Windows\\system32\\shell32.dll,0");
            }
            int iconind = 0;
            if (iconpath.LastIndexOf(',') > iconpath.LastIndexOf('.'))
            {
                iconind = int.Parse(iconpath.Substring(iconpath.LastIndexOf(',') + 1));
                iconpath = iconpath.Remove(iconpath.LastIndexOf(','));
            }
            try
            {
                return Icon.FromHandle(ExtractIconA(0, iconpath.Replace("%1", file), iconind));
            }
            catch (Exception)
            {
                return Icon.FromHandle(ExtractIconA(0, "C:\\Windows\\system32\\shell32.dll", 0));
            }
        }
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
}
