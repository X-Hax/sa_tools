using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using SAModel.SAEditorCommon.ProjectManagement;

namespace SAModel.DataToolbox
{
	public partial class MainForm : Form
	{
		byte[] file;
		Properties.Settings Settings = Properties.Settings.Default;
        Dictionary<string, string> templateList = new Dictionary<string, string>();

        public MainForm()
		{
			InitializeComponent();
			InitializeNumberBoxes();
			radioButtonSearchData.Checked = true; // Scan tab
		}

		private void InitializeNumberBoxes()
		{
			// Binary tab
			numericUpDownBinaryKey.ValueChanged += NumericUpDownBinaryKey_ValueChanged;
			numericUpDownBinaryAddress.ValueChanged += NumericUpDownBinaryAddress_ValueChanged;
			numericUpDownBinaryOffset.ValueChanged += NumericUpDownBinaryOffset_ValueChanged;
			// Scanner tab
			numericUpDownScanBinaryKey.ValueChanged += NumericUpDownScanBinaryKey_ValueChanged;
			numericUpDownStartAddr.ValueChanged += NumericUpDownStartAddr_ValueChanged;
			numericUpDownEndAddr.ValueChanged += NumericUpDownEndAddr_ValueChanged;
			numericUpDownOffset.ValueChanged += NumericUpDownOffset_ValueChanged;
			numericUpDownStartAddr.Hexadecimal =
			   numericUpDownEndAddr.Hexadecimal =
			   numericUpDownScanBinaryKey.Hexadecimal =
			   numericUpDownOffset.Hexadecimal = true;
			numericUpDownStartAddr.Maximum =
				numericUpDownEndAddr.Maximum =
				numericUpDownScanBinaryKey.Maximum =
				numericUpDownOffset.Maximum = new decimal(new int[] {
			-1,
			0,
			0,
			0});
			numericUpDownStartAddr.Minimum =
			numericUpDownEndAddr.Minimum =
			numericUpDownScanBinaryKey.Minimum =
			numericUpDownOffset.Minimum
			= new decimal(new int[] {
			-1,
			0,
			0,
			-2147483648});
		}

		string[] SortTemplateList(string[] originalList)
        {
            var ordered = originalList.OrderBy(str => Path.GetFileNameWithoutExtension(str));
            List<string> result = new List<string>();
            // Put SADXPC first and SA2PC second
            foreach (string file in ordered)
            {
                if (file.Contains("DX") && file.Contains("PC"))
                    result.Insert(0, file);
                else if (file.Contains("SA2") && file.Contains("PC"))
                    result.Add(file);
            }
            // Add other items
            foreach (string file in ordered)
            {
                if (!result.Contains(file))
                    result.Add(file);
            }
            return result.ToArray();
        }

        private void loadTemplateList(string folder)
        {
            templateList = new Dictionary<string, string>();
            string[] templateNames = SortTemplateList(Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly));
            for (int i = 0; i < templateNames.Length; i++)
            {
                templateList.Add(Path.GetFileNameWithoutExtension(templateNames[i]), templateNames[i]);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
		{
			// Binary File Extractor defaults
			comboBoxBinaryFormat.SelectedIndex = 1;
			textBoxBinaryAuthor.Text = Settings.Author;
			ComboBoxBinaryType.Items.Clear();
			comboBoxBinaryItemType.SelectedIndex = 0;
			for (int i = 0; i < ModelFileTypes.Length; i++)
			{
				ComboBoxBinaryType.Items.Add(ModelFileTypes[i].name_or_type);
			}

            // Initialize templates
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            string templatesPath;
            if (Directory.Exists(Path.Combine(appPath, "..\\GameConfig")))
                templatesPath = Path.Combine(appPath, "..\\GameConfig");
            else
                templatesPath = Path.Combine(appPath, "..\\..\\GameConfig");
            loadTemplateList(templatesPath);
            foreach (KeyValuePair<string, string> entry in templateList)
            {
                comboBoxSplitGameSelect.Items.Add(entry);
            }
            comboBoxSplitGameSelect.DisplayMember = "Key";
            if (comboBoxSplitGameSelect.Items.Count > 0)
                comboBoxSplitGameSelect.SelectedIndex = 0;
            else
                MessageBox.Show(this, "Game templates not found.", "Data Toolbox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Split screen defaults
            comboBoxLabels.SelectedIndex = 0;
        }


        #region Binary Data Extractor Tab
        public struct BinaryModelType
		{
			public string name_or_type;
			public UInt32 key;
			public int data_type;
		};

		public struct ModelFileType
		{
			public string name_or_type;
			public UInt32 key;
		};

		public static readonly ModelFileType[] ModelFileTypes = new[] {
		new ModelFileType { name_or_type = "EXE", key = 0x00400000u },
		new ModelFileType { name_or_type = "DLL", key = 0x10000000u },
		new ModelFileType { name_or_type = "1ST_READ.BIN", key = 0x8C010000u },
		new ModelFileType { name_or_type = "SA1 level", key = 0x0C900000u },
		new ModelFileType { name_or_type = "SA2 level", key = 0x8C500000u },
		new ModelFileType { name_or_type = "SA1 event/misc file", key = 0xCB80000u },
		new ModelFileType { name_or_type = "SA2 event file", key = 0xC600000u },
		new ModelFileType { name_or_type = "SA2PC event file", key = 0x8125FE60u },
		new ModelFileType { name_or_type = "Model file", key = 0u },
		};

		public static readonly BinaryModelType[] KnownBinaryFiles = new[] {
		new BinaryModelType { name_or_type = "1ST_READ", key = 0x8C010000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV00", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV02", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV03", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV0100", key = 0x0C920000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV01OBJ", key = 0x0C920000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV0130", key = 0x0C920000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_GARDEN00", key = 0x0CB80000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_GARDEN01", key = 0x0CB80000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_GARDEN02", key = 0x0CB80000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_RACE", key = 0x0CB80000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_MAIN", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS0", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS2", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS4", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS6", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS7", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_E101", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_E101R", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_EGM1", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_EGM2", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_EGM3", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_ROBO", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "SBOARD", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "SHOOTING", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "TIKAL_PROG", key = 0x0CB00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "MINICART", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "A_MOT", key = 0x0CC00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_MOT", key = 0x0CC00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "E_MOT", key = 0x0CC00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "S_MOT", key = 0x0CC00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "S_SBMOT", key = 0x0CB08000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADVERTISE", key = 0x8C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "MOVIE", key = 0x8CEB0000u, data_type = 0 },
		};
		private void textBoxBinaryFilename_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void textBoxBinaryFilename_DragDrop(object sender, DragEventArgs e)
		{
			string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			textBoxBinaryFilename.Text = fileList[0];
		}

		private int CheckKnownFile(string filename)
		{
			if (Path.GetFileNameWithoutExtension(filename).Substring(0, 3).Equals("EV0", StringComparison.OrdinalIgnoreCase))
			{
				return -4;
			}

			else if (Path.GetFileNameWithoutExtension(filename).Substring(0, 2).Equals("E0", StringComparison.OrdinalIgnoreCase))
			{
				return -5;
			}

			for (int i = 0; i < KnownBinaryFiles.Length; i++)
			{
				if (Path.GetFileNameWithoutExtension(filename).Equals(KnownBinaryFiles[i].name_or_type, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}

			return -1;
		}

		private void CheckBox3_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownBinaryAddress.Hexadecimal = checkBoxBinaryHex.Checked;
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			numericUpDownBinaryKey.Value = ModelFileTypes[ComboBoxBinaryType.SelectedIndex].key;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			bool success = false;
			uint address = (uint)numericUpDownBinaryAddress.Value;
			if (checkBoxBinaryMemory.Checked) address -= (uint)numericUpDownBinaryKey.Value;
			LandTableFormat format = (LandTableFormat)comboBoxBinaryFormat.SelectedIndex;
			LandTableFormat outfmt = format;
			if (format == LandTableFormat.SADX) outfmt = LandTableFormat.SA1;
			ByteConverter.BigEndian = checkBoxBinaryBigEndian.Checked;
			Settings.Author = textBoxBinaryAuthor.Text;
			Settings.Save();
			SaveFileDialog sd = new SaveFileDialog();
			switch (comboBoxBinaryItemType.SelectedIndex)
			{
				// Level
				case 0:
					sd = new SaveFileDialog() { DefaultExt = outfmt.ToString().ToLowerInvariant() + "lvl", Filter = outfmt.ToString().ToUpperInvariant() + "LVL Files|*." + outfmt.ToString().ToLowerInvariant() + "lvl|All Files|*.*" };
					if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						new LandTable(file, (int)numericUpDownBinaryAddress.Value, (uint)numericUpDownBinaryKey.Value, format) { Author = textBoxBinaryAuthor.Text, Description = textBoxBinaryDescription.Text }.SaveToFile(sd.FileName, outfmt);
						if (checkBoxBinaryStructs.Checked) ConvertToText(sd.FileName);
						if (!checkBoxBinarySAModel.Checked) File.Delete(sd.FileName);
						success = true;
					}
					break;
				// Model
				case 1:
					sd = new SaveFileDialog() { DefaultExt = outfmt.ToString().ToLowerInvariant() + "mdl", Filter = outfmt.ToString().ToUpperInvariant() + "MDL Files|*." + outfmt.ToString().ToLowerInvariant() + "mdl|All Files|*.*" };
					if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						NJS_OBJECT tempmodel = new NJS_OBJECT(file, (int)address, (uint)numericUpDownBinaryKey.Value, (ModelFormat)comboBoxBinaryFormat.SelectedIndex, null);
						ModelFile.CreateFile(sd.FileName, tempmodel, null, textBoxBinaryAuthor.Text, textBoxBinaryDescription.Text, null, (ModelFormat)comboBoxBinaryFormat.SelectedIndex);
						ConvertToText(sd.FileName, checkBoxBinaryStructs.Checked, checkBoxBinaryNJA.Checked, false);
						if (!checkBoxBinarySAModel.Checked) File.Delete(sd.FileName);
						success = true;
					}
					break;
				// Action
				case 2:
					sd = new SaveFileDialog() { DefaultExt = outfmt.ToString().ToLowerInvariant() + "mdl", Filter = outfmt.ToString().ToUpperInvariant() + "MDL Files|*." + outfmt.ToString().ToLowerInvariant() + "mdl|All Files|*.*" };
					if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						// Model
						NJS_ACTION tempaction = new NJS_ACTION(file, (int)address, (uint)numericUpDownBinaryKey.Value, (ModelFormat)comboBoxBinaryFormat.SelectedIndex, null);
						NJS_OBJECT tempmodel = tempaction.Model;
						ModelFile.CreateFile(sd.FileName, tempmodel, null, textBoxBinaryAuthor.Text, textBoxBinaryDescription.Text, null, (ModelFormat)comboBoxBinaryFormat.SelectedIndex);
						ConvertToText(sd.FileName, checkBoxBinaryStructs.Checked, checkBoxBinaryNJA.Checked, false);
						if (!checkBoxBinarySAModel.Checked) File.Delete(sd.FileName);

						// Action
						string saanimPath = Path.Combine(Path.GetDirectoryName(sd.FileName), Path.GetFileNameWithoutExtension(sd.FileName) + ".saanim");

						tempaction.Animation.Save(saanimPath);
						ConvertToText(saanimPath, checkBoxBinaryStructs.Checked, checkBoxBinaryNJA.Checked, checkBoxBinaryJSON.Checked);

						if (checkBoxBinarySAModel.Checked)
						{
							using (TextWriter twmain = File.CreateText(Path.Combine(Path.GetDirectoryName(sd.FileName), Path.GetFileNameWithoutExtension(sd.FileName) + ".action")))
							{
								twmain.WriteLine(Path.GetFileName(saanimPath));
								twmain.Flush();
								twmain.Close();
							}
						}
						else File.Delete(saanimPath);
						success = true;
					}
					break;
			}
			if (success)
			{
				MessageBox.Show("Data extracted!", "Binary Data Extractor", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void UpdateStartButtonBinary()
		{
			if (!File.Exists(textBoxBinaryFilename.Text) || ((!checkBoxBinaryNJA.Checked && !checkBoxBinarySAModel.Checked && !checkBoxBinaryJSON.Checked && !checkBoxBinaryStructs.Checked))) buttonBinaryExtract.Enabled = false;
			else buttonBinaryExtract.Enabled = true;
		}

		private void checkBox_SAModel_CheckedChanged(object sender, EventArgs e)
		{
			UpdateStartButtonBinary();
		}

		private void checkBox_Structs_CheckedChanged(object sender, EventArgs e)
		{
			UpdateStartButtonBinary();
		}

		private void checkBox_NJA_CheckedChanged(object sender, EventArgs e)
		{
			UpdateStartButtonBinary();
		}

		private void checkBox_JSON_CheckedChanged(object sender, EventArgs e)
		{
			UpdateStartButtonBinary();
		}

		private void textBoxBinaryFilename_TextChanged(object sender, EventArgs e)
		{
			if (!checkBoxBinaryNJA.Checked && !checkBoxBinarySAModel.Checked && !checkBoxBinaryJSON.Checked && !checkBoxBinaryStructs.Checked)
			{
				buttonBinaryExtract.Enabled = false;
				return;
			}
			if (File.Exists(textBoxBinaryFilename.Text))
			{
				file = File.ReadAllBytes(textBoxBinaryFilename.Text);
				if (Path.GetExtension(textBoxBinaryFilename.Text).Equals(".prs", StringComparison.OrdinalIgnoreCase)) file = FraGag.Compression.Prs.Decompress(file);
				buttonBinaryExtract.Enabled = true;
				uint? baseaddr = SplitTools.HelperFunctions.SetupEXE(ref file);
				if (!baseaddr.HasValue)
				{
					int u = CheckKnownFile(textBoxBinaryFilename.Text);
					if (u == -5)
					{
						numericUpDownBinaryKey.Value = 0xC600000u;
						comboBoxBinaryFormat.SelectedIndex = 2;
					}
					else if (u == -4)
					{
						numericUpDownBinaryKey.Value = 0xCB80000u;
						comboBoxBinaryFormat.SelectedIndex = 0;
					}
					else if (u != -1)
					{
						numericUpDownBinaryKey.Value = KnownBinaryFiles[u].key;
						comboBoxBinaryFormat.SelectedIndex = KnownBinaryFiles[u].data_type;
					}
					else comboBoxBinaryFormat.SelectedIndex = 1;
				}
				else numericUpDownBinaryKey.Value = (int)baseaddr;
				buttonBinaryExtract.Enabled = true;
			}
			else buttonBinaryExtract.Enabled = false;
		}

		private void buttonBinaryBrowse_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog())
			{
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					textBoxBinaryFilename.Text = fileDialog.FileName;
				}
			}
		}

		private void NumericUpDownBinaryKey_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownBinaryKey.Value < 0)
				numericUpDownBinaryKey.Value = unchecked((uint)int.Parse(numericUpDownBinaryKey.Value.ToString()));
		}

		private void NumericUpDownBinaryAddress_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownBinaryAddress.Value < 0)
				numericUpDownBinaryAddress.Value = unchecked((uint)int.Parse(numericUpDownBinaryAddress.Value.ToString()));
		}

		private void NumericUpDownBinaryOffset_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownBinaryOffset.Value < 0)
				numericUpDownBinaryOffset.Value = unchecked((uint)int.Parse(numericUpDownBinaryOffset.Value.ToString()));
		}

		#endregion

		#region Struct Converter Tab
		void ConvertToText(string FileName, bool CStruct = true, bool NJA = false, bool JSON = false, string outdir = "")
		{
			bool dx = comboBoxBinaryFormat.SelectedIndex == 1;
			string outext;
			string outpath;
			string checkname = Path.GetFileNameWithoutExtension(FileName);
			// Remove double extensions
			int count = checkname.Split('.').Length - 1;
			if (count > 0)
				checkname = Path.GetFileNameWithoutExtension(checkname);
			if (outdir != "")
			{
				Directory.CreateDirectory(outdir);
				outpath = Path.Combine(outdir, checkname);
			}
			else outpath = Path.Combine(Path.GetDirectoryName(FileName), checkname);
			if (CStruct)
			{
				outext = ".c";
                SAModel.SAEditorCommon.StructConversion.ConvertFileToText(FileName, SAModel.SAEditorCommon.StructConversion.TextType.CStructs, outpath + outext, dx, false);
            }
			if (NJA)
			{
				switch (Path.GetExtension(FileName).ToLowerInvariant())
				{
					case ".saanim":
						outext = ".nam";
						if (FileName.Contains(".nas") || FileName.Contains(".NAS"))
							outext = ".nas";
						break;
					case ".satex":
						outext = ".tls";
						break;
					case ".sa1mdl":
					case ".sa2mdl":
					default:
						outext = ".nja";
						break;
				}
				SAModel.SAEditorCommon.StructConversion.ConvertFileToText(FileName, SAModel.SAEditorCommon.StructConversion.TextType.NJA, outpath + outext, dx, false);
			}
			if (JSON)
			{
				outext = ".json";
				SAModel.SAEditorCommon.StructConversion.ConvertFileToText(FileName, SAModel.SAEditorCommon.StructConversion.TextType.JSON, outpath + outext, dx, false);
			}
		}

		private void UpdateConvertButton()
		{
			if (listBoxStructConverter.Items.Count == 0 || ((!checkBoxStructConvNJA.Checked && !checkBoxStructConvJSON.Checked && !checkBoxStructConvStructs.Checked))) buttonStructConvConvertBatch.Enabled = false;
			else buttonStructConvConvertBatch.Enabled = true;
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog() { Filter = "Levels, models, texlists and animations|*.sa?lvl;*.sa?mdl;*.satex;*.saanim|All files|*.*", Multiselect = true };
			if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				listBoxStructConverter.Items.AddRange(op.FileNames);
				UpdateConvertButton();
			}
		}

		private void buttonConvertBatch_Click(object sender, EventArgs e)
		{
			string outdir = "";
			if (!checkBoxStructConvSameOutputFolderBatch.Checked)
			{
				SaveFileDialog sav = new SaveFileDialog() { Filter = "Folder|*.*", Title = "Select output folder" };
				if (sav.ShowDialog() == System.Windows.Forms.DialogResult.OK) outdir = sav.FileName;
				else return;
			}
			foreach (string item in listBoxStructConverter.Items)
			{
				ConvertToText(item, checkBoxStructConvStructs.Checked, checkBoxStructConvNJA.Checked, checkBoxStructConvJSON.Checked, outdir);
			}
			string finished = "Struct conversion finished!";
			if (outdir != "") finished += "\nConverted files are saved to " + outdir + ".";
			MessageBox.Show(finished, "Struct Converter", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void listBoxStructConverter_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBoxStructConverter.SelectedIndices.Count > 0) buttonStructConvRemoveSelBatch.Enabled = true;
			else buttonStructConvRemoveSelBatch.Enabled = false;
		}
		private void buttonRemoveSelBatch_Click(object sender, EventArgs e)
		{
			var selectedItems = listBoxStructConverter.SelectedItems.Cast<String>().ToList();
			foreach (var item in selectedItems)
				listBoxStructConverter.Items.Remove(item);
			if (listBoxStructConverter.Items.Count == 0) buttonStructConvConvertBatch.Enabled = false;
		}

		private void buttonRemoveAllBatch_Click(object sender, EventArgs e)
		{
			listBoxStructConverter.Items.Clear();
			buttonStructConvConvertBatch.Enabled = false;
		}

        private void AddDirectoryForStructConverter(string dirname)
		{
			string[] files = Directory.GetFiles(dirname, "*.*", SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
                if (!listBoxStructConverter.Items.Contains(files[i]))
                {
                    switch (Path.GetExtension(files[i]).ToLowerInvariant())
                    {
                        case ".sa1mdl":
                        case ".sa2mdl":
                        case ".sa1lvl":
                        case ".sa2lvl":
                        case ".saanim":
						case ".satex":
							if (!listBoxStructConverter.Items.Contains(files[i]))
                                listBoxStructConverter.Items.Add(files[i]);
                            break;
                        default:
                            break;
                    }
                }
        }

        private void listBoxStructConverter_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            for (int u = 0; u < fileList.Length; u++)
            {
                if (Directory.Exists(fileList[u]))
                    AddDirectoryForStructConverter(fileList[u]);
                else if (!listBoxStructConverter.Items.Contains(fileList[u])) 
                    listBoxStructConverter.Items.Add(fileList[u]);
            }
            UpdateConvertButton();
        }

		private void listBoxStructConverter_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void checkBox_StructConvStructs_CheckedChanged(object sender, EventArgs e)
		{
			UpdateConvertButton();
		}

		private void checkBox_StructConvNJA_CheckedChanged(object sender, EventArgs e)
		{
			UpdateConvertButton();
		}

		private void checkBox_StructConvJSON_CheckedChanged(object sender, EventArgs e)
		{
			UpdateConvertButton();
		}
		#endregion

		#region Split Tab
		private void button_AddFilesSplit_Click(object sender, EventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog() { Filter = "Binary files|*.exe;*.dll;*.bin;*.mdl|All files|*.*", Multiselect = true };
			if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				listBoxSplitFiles.Items.AddRange(op.FileNames);
				buttonSplitStart.Enabled = true;
			}
		}

        private void buttonSplit_Click(object sender, EventArgs e)
        {
            string outdir = "";
            if (!checkBoxSameFolderSplit.Checked)
            {
                SaveFileDialog sd = new SaveFileDialog() { Title = "Select output folder", FileName = "output", DefaultExt = "" };
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    outdir = sd.FileName;
                }
                else return;
            }
            Templates.SplitTemplate template = ProjectFunctions.openTemplateFile(templateList[comboBoxSplitGameSelect.Text], true);
            SplitProgress spl = new SplitProgress(null, listBoxSplitFiles.Items.Cast<String>().ToList(), template, outdir, 0, comboBoxLabels.SelectedIndex);
			spl.ShowDialog();
		}

		private void listBox_SplitFiles_DragDrop(object sender, DragEventArgs e)
		{
			string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			for (int u = 0; u < fileList.Length; u++)
			{
				if (!listBoxSplitFiles.Items.Contains(fileList[u])) listBoxSplitFiles.Items.Add(fileList[u]);
			}
			buttonSplitStart.Enabled = true;
		}

		private void listBox_SplitFiles_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}
		private void buttonRemoveSplit_Click(object sender, EventArgs e)
		{
			var selectedItems = listBoxSplitFiles.SelectedItems.Cast<String>().ToList();
			foreach (var item in selectedItems)
				listBoxSplitFiles.Items.Remove(item);
			if (listBoxSplitFiles.Items.Count == 0) buttonSplitStart.Enabled = false;
		}
		private void buttonClearAllSplit_Click(object sender, EventArgs e)
		{
			listBoxSplitFiles.Items.Clear();
			buttonSplitStart.Enabled = false;
		}
		private void listBox_SplitFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBoxSplitFiles.SelectedIndices.Count > 0) buttonRemoveSplit.Enabled = true;
			else buttonRemoveSplit.Enabled = false;
		}

		#endregion

		#region SplitMDL Tab
		private void buttonAnimFilesAdd_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog() { Multiselect = true })
			{
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					listBoxMDLAnimationFiles.BeginUpdate();

					foreach (string path in fileDialog.FileNames)
					{
						listBoxMDLAnimationFiles.Items.Add(path);
					}

					listBoxMDLAnimationFiles.EndUpdate();
				}
			}
		}

		private void buttonSplitMDL_Click(object sender, EventArgs e)
		{
			string outdir = Path.GetDirectoryName(textBoxMDLFilename.Text);
			string[] animationFiles = new string[listBoxMDLAnimationFiles.Items.Count];

			for (int i = 0; i < listBoxMDLAnimationFiles.Items.Count; i++)
			{
				animationFiles[i] = listBoxMDLAnimationFiles.Items[i].ToString();
			}

			if (!checkBoxMDLSameFolder.Checked)
			{
				FolderBrowserDialog sav = new FolderBrowserDialog();
				if (sav.ShowDialog() == System.Windows.Forms.DialogResult.OK) outdir = sav.SelectedPath;
				else return;
			}

			List<string> files = new List<string>();
			files.Add(textBoxMDLFilename.Text);
			files.AddRange(listBoxMDLAnimationFiles.Items.Cast<String>().ToList());
			SplitProgress spl = new SplitProgress(null, files, null, outdir, checkBoxMDLBigEndian.Checked ? 2 : 1, comboBoxLabels.SelectedIndex);
			spl.ShowDialog();
		}

		private void buttonAnimFilesClear_Click(object sender, EventArgs e)
		{
			listBoxMDLAnimationFiles.Items.Clear();
		}

		private void listBoxMDLAnimationFiles_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void listBoxMDLAnimationFiles_DragDrop(object sender, DragEventArgs e)
		{
			string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			for (int u = 0; u < fileList.Length; u++)
			{
				if (!listBoxMDLAnimationFiles.Items.Contains(fileList[u])) listBoxMDLAnimationFiles.Items.Add(fileList[u]);
			}
			buttonSplitMDL.Enabled = File.Exists(textBoxMDLFilename.Text);
		}

		private void buttonMDLBrowse_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog())
			{
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					textBoxMDLFilename.Text = fileDialog.FileName;
				}
			}
		}

		private void textBoxMDLFilename_DragDrop(object sender, DragEventArgs e)
		{
			string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			textBoxMDLFilename.Text = fileList[0];
		}

		private void textBoxMDLFilename_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void textBoxMDLFilename_TextChanged(object sender, EventArgs e)
		{
			buttonSplitMDL.Enabled = File.Exists(textBoxMDLFilename.Text);
		}

		private void buttonMDLAnimFilesRemove_Click(object sender, EventArgs e)
		{
			var selectedItems = listBoxMDLAnimationFiles.SelectedItems.Cast<String>().ToList();
			foreach (var item in selectedItems)
				listBoxMDLAnimationFiles.Items.Remove(item);
		}

		private void listBoxMDLAnimationFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonMDLAnimFilesRemove.Enabled = listBoxMDLAnimationFiles.SelectedIndices.Count > 0;
		}
		#endregion

		#region Scanner Tab
		private string FileBrowser(string caption, string filter)
		{
			using (OpenFileDialog ofd = new OpenFileDialog { Filter = filter, Title = caption })
			{
				if (ofd.ShowDialog() == DialogResult.OK)
					return ofd.FileName;
			}
			return "";
		}

		private void buttonScanStart_Click(object sender, EventArgs e)
		{
			// Initialize variables
			ObjScan.CancelScan = false;
			ObjScan.NumSteps = 0;
			ObjScan.CurrentStep = 0;
			ObjScan.CurrentScanData = "";
			ObjScan.FoundBasicModels = 0;
			ObjScan.FoundChunkModels = 0;
			ObjScan.FoundGCModels = 0;
			ObjScan.FoundSA1Landtables = 0;
			ObjScan.FoundSA2Landtables = 0;
			ObjScan.FoundSA2BLandtables = 0;
			ObjScan.FoundActions = 0;
			ObjScan.FoundMotions = 0;
			ObjScan.FoundSADXLandtables = 0;
			// Set input parameters
			ObjScan.SourceFilename = textBoxInputFile.Text;
			ObjScan.OutputFolder = textBoxOutputFolder.Text;
			ObjScan.BigEndian = checkBoxBigEndian.Checked;
			ObjScan.NoMeta = checkBoxSkipMeta.Checked;
			ObjScan.ReverseColors = listBoxBaseGame.SelectedItem.ToString() == "SADX Gamecube";
			ObjScan.SingleOutputFolder = checkBoxSingleOutput.Checked;
			ObjScan.StartAddress = (uint)numericUpDownStartAddr.Value;
			ObjScan.EndAddress = (uint)numericUpDownEndAddr.Value;
			ObjScan.ImageBase = (uint)numericUpDownScanBinaryKey.Value;
			ObjScan.DataOffset = (uint)numericUpDownOffset.Value;
			ObjScan.BasicModelsAreDX = checkBoxBasicSADX.Checked;

			if (radioButtonSearchData.Checked)
			{
				ObjScan.matchfile = "";
				ObjScan.KeepLandtableModels = checkBoxKeepLevel.Checked;
				ObjScan.KeepChildModels = checkBoxKeepChild.Checked;
				ObjScan.SimpleSearch = checkBoxSimpleScan.Checked;
				ObjScan.ModelParts = (int)numericUpDownNodes.Value;
				ObjScan.ShortRot = checkBoxShortRot.Checked;

				// Scan parameters
				if (checkBoxSearchLandtables.Checked)
				{
					switch (listBoxBaseGame.SelectedItem.ToString())
					{
						case "SA1 Dreamcast":
							ObjScan.scan_sa1_land = true;
							ObjScan.scan_sa2_land = false;
							ObjScan.scan_sa2b_land = false;
							break;
						case "SADX Gamecube":
							ObjScan.scan_sa1_land = true;
							ObjScan.scan_sa2_land = false;
							ObjScan.scan_sa2b_land = false;
							break;
						case "SADX PC":
						case "SADX X360":
							ObjScan.scan_sa1_land = false;
							ObjScan.scan_sa2_land = false;
							ObjScan.scan_sa2b_land = false;
							break;
						case "SA2 Dreamcast":
							ObjScan.scan_sa1_land = false;
							ObjScan.scan_sa2_land = true;
							ObjScan.scan_sa2b_land = false;
							break;
						case "SA2B Gamecube":
							ObjScan.scan_sa1_land = false;
							ObjScan.scan_sa2_land = true;
							ObjScan.scan_sa2b_land = true;
							break;
						case "SA2 PC":
							ObjScan.scan_sa1_land = false;
							ObjScan.scan_sa2_land = true;
							ObjScan.scan_sa2b_land = true;
							break;
					}
				}
				ObjScan.scan_sa1_model = checkBoxSearchBasic.Checked;
				ObjScan.scan_sa2_model = checkBoxSearchChunk.Checked;
				ObjScan.scan_sa2b_model = checkBoxSearchGinja.Checked;
				ObjScan.scan_action = checkBoxSearchAction.Checked;
				ObjScan.scan_motion = checkBoxSearchMotion.Checked;
			}
			else
				ObjScan.matchfile = textBoxFindModel.Text;
			using ScanProgress scanProgress = new ScanProgress();
			scanProgress.ShowDialog();
		}

		private void listBoxBaseGame_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (listBoxBaseGame.SelectedItem.ToString())
			{
				case "SA1 Dreamcast":
					checkBoxBasicSADX.Checked = false;
					checkBoxSearchBasic.Checked = true;
					checkBoxSearchGinja.Checked = false;
					checkBoxSearchChunk.Checked = false;
					checkBoxBigEndian.Checked = false;
					numericUpDownOffset.Value = 0;
					break;
				case "SADX Gamecube":
					checkBoxBasicSADX.Checked = false;
					checkBoxSearchBasic.Checked = true;
					checkBoxSearchGinja.Checked = false;
					checkBoxBigEndian.Checked = true;
					numericUpDownOffset.Value = 0;
					break;
				case "SADX PC":
					checkBoxBasicSADX.Checked = true;
					checkBoxSearchBasic.Checked = true;
					checkBoxSearchGinja.Checked = false;
					checkBoxBigEndian.Checked = false;
					numericUpDownOffset.Value = 0;
					break;
				case "SADX X360":
					checkBoxBasicSADX.Checked = true;
					checkBoxSearchBasic.Checked = true;
					checkBoxSearchGinja.Checked = false;
					checkBoxBigEndian.Checked = true;
					numericUpDownOffset.Value = 0xC800;
					break;
				case "SA2 Dreamcast":
					checkBoxBasicSADX.Checked = false;
					checkBoxSearchBasic.Checked = false;
					checkBoxSearchGinja.Checked = false;
					checkBoxSearchChunk.Checked = true;
					checkBoxBigEndian.Checked = false;
					numericUpDownOffset.Value = 0;
					break;
				case "SA2B Gamecube":
					checkBoxBasicSADX.Checked = false;
					checkBoxSearchBasic.Checked = false;
					checkBoxSearchGinja.Checked = true;
					checkBoxSearchChunk.Checked = true;
					checkBoxBigEndian.Checked = true;
					numericUpDownOffset.Value = 0;
					break;
				case "SA2 PC":
					checkBoxBasicSADX.Checked = false;
					checkBoxSearchBasic.Checked = false;
					checkBoxSearchGinja.Checked = true;
					checkBoxSearchChunk.Checked = true;
					checkBoxBigEndian.Checked = false;
					numericUpDownOffset.Value = 0;
					break;
			}
		}

		private void checkBoxSearchMotion_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownNodes.Visible = labelNodes.Visible = checkBoxSearchMotion.Checked;
		}

		private void radioButtonSearchData_CheckedChanged(object sender, EventArgs e)
		{
			checkBoxSearchBasic.Enabled = checkBoxSearchChunk.Enabled = checkBoxSearchGinja.Enabled =
				checkBoxSearchAction.Enabled = checkBoxSearchMotion.Enabled = checkBoxSearchLandtables.Enabled =
				labelNodes.Enabled = labelNodes.Enabled = radioButtonSearchData.Checked;
			textBoxFindModel.Enabled = buttonFindBrowse.Enabled = radioButtonFindModel.Checked;
			CheckPaths();
		}

		private void NumericUpDownScanBinaryKey_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownScanBinaryKey.Value < 0)
				numericUpDownScanBinaryKey.Value = unchecked((uint)int.Parse(numericUpDownScanBinaryKey.Value.ToString()));
		}

		private void NumericUpDownStartAddr_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownStartAddr.Value < 0)
				numericUpDownStartAddr.Value = unchecked((uint)int.Parse(numericUpDownStartAddr.Value.ToString()));
		}

		private void NumericUpDownEndAddr_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownEndAddr.Value < 0)
				numericUpDownEndAddr.Value = unchecked((uint)int.Parse(numericUpDownEndAddr.Value.ToString()));
		}

		private void NumericUpDownOffset_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownOffset.Value < 0)
				numericUpDownOffset.Value = unchecked((uint)int.Parse(numericUpDownOffset.Value.ToString()));
		}

		private void buttonInputBrowse_Click(object sender, EventArgs e)
		{
			textBoxInputFile.Text = FileBrowser("Select the input file", "Binary Files|*.exe;*.dll;*.bin;*.rel;*.prs|All Files|*.*");
			if (!File.Exists(textBoxInputFile.Text))
				buttonScanStart.Enabled = false;
			else
			{
				FileInfo fi = new FileInfo(textBoxInputFile.Text);
				numericUpDownEndAddr.Value = fi.Length - numericUpDownOffset.Value;
				numericUpDownScanBinaryKey.Value = CheckBinaryFile(textBoxInputFile.Text);
				if (string.IsNullOrEmpty(textBoxOutputFolder.Text))
					textBoxOutputFolder.Text = Path.Combine(Path.GetDirectoryName(textBoxInputFile.Text), Path.GetFileNameWithoutExtension(textBoxInputFile.Text));
				buttonScanStart.Enabled = true;
			}
		}

		private uint CheckBinaryFile(string path)
		{
			if (Path.GetFileName(path).ToLowerInvariant() == "sonicapp.exe")
			{
				listBoxBaseGame.SelectedItem = "SADX X360";
				return 0x82000000;
			}
			else if (Path.GetFileName(path).ToLowerInvariant() == "sonic2app.exe")
			{
				listBoxBaseGame.SelectedItem = "SA2 PC";
			}
			// Common extensions
			switch (Path.GetExtension(path).ToLowerInvariant())
			{
				case ".exe":
					return 0x400000;
				case ".dll":
					return 0x1000000;
				case ".rel":
					return 0xC900000;
			}
			// Known filenames
			string filename = Path.GetFileNameWithoutExtension(path).ToUpperInvariant();
			// Events
			if (filename.Contains("EV") && filename.Length == 6)
			{
				listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
				return 0xCB80000;
			}
			else if (filename.Contains("E0") && filename.Length == 5)
			{
				return 0xC600000;
			}
			// Other known files
			switch (filename)
			{
				case "1ST_READ":
					return 0x8C010000;
				case "ADV00":
				case "ADV02":
				case "ADV03":
				case "AL_MAIN":
				case "B_CHAOS0":
				case "B_CHAOS2":
				case "B_CHAOS4":
				case "B_CHAOS6":
				case "B_CHAOS7":
				case "B_E101":
				case "B_E101R":
				case "B_EGM1":
				case "B_EGM2":
				case "B_EGM3":
				case "B_ROBO":
				case "SBOARD":
				case "SHOOTING":
				case "MINICART":
					listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
					return 0xC900000;
				case "ADV0100":
				case "ADV01OBJ":
				case "ADV0130":
					listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
					return 0xC920000;
				case "A_MOT":
				case "B_MOT":
				case "E_MOT":
				case "S_MOT":
					listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
					return 0xCC00000;
				case "S_SBMOT":
					listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
					return 0xCB08000;
				case "M_SBMOT":
					listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
					return 0xCADC000;
				case "AL_GARDEN00":
				case "AL_GARDEN01":
				case "AL_GARDEN02":
				case "AL_RACE":
					listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
					return 0x0CB80000;
				case "TIKAL_PROG":
					listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
					return 0xCB00000;
				case "ADVERTISE":
					listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
					return 0x8C900000;
				case "MOVIE":
					listBoxBaseGame.SelectedItem = "SA1 Dreamcast";
					return 0x8CEB0000;
				default:
					return 0;
			}
		}

		private void buttonOutputBrowse_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog fd = new FolderBrowserDialog { Description = "Select the output folder", UseDescriptionForTitle = true })
				if (fd.ShowDialog() == DialogResult.OK)
					textBoxOutputFolder.Text = fd.SelectedPath;
				else
					textBoxOutputFolder.Text = "";
		}

		private void CheckPaths()
		{
			if (radioButtonSearchData.Checked)
			{
				if (string.IsNullOrEmpty(textBoxOutputFolder.Text))
					buttonScanStart.Enabled = false;
				else
					buttonScanStart.Enabled = File.Exists(textBoxInputFile.Text);
			}
			else
			{
				if (string.IsNullOrEmpty(textBoxFindModel.Text))
					buttonScanStart.Enabled = false;
				else
					buttonScanStart.Enabled = File.Exists(textBoxFindModel.Text);
			}
		}

		private void textBoxOutputFolder_TextChanged(object sender, EventArgs e)
		{
			CheckPaths();
		}

		private void textBoxInputFile_TextChanged(object sender, EventArgs e)
		{
			CheckPaths();
		}

		private void buttonFindBrowse_Click(object sender, EventArgs e)
		{
			textBoxFindModel.Text = FileBrowser("Select the model file", "Model Files|*.sa1mdl|All Files|*.*");
			if (!File.Exists(textBoxInputFile.Text) || !File.Exists(textBoxFindModel.Text))
				buttonScanStart.Enabled = false;
			else
			{
				if (string.IsNullOrEmpty(textBoxOutputFolder.Text))
					textBoxOutputFolder.Text = Path.Combine(Path.GetDirectoryName(textBoxInputFile.Text), Path.GetFileNameWithoutExtension(textBoxInputFile.Text));
				buttonScanStart.Enabled = true;
			}
		}

		private void EnableOrDisableActions()
		{
			checkBoxSearchAction.Enabled = true;
			if (!checkBoxSearchBasic.Checked && !checkBoxSearchChunk.Checked && !checkBoxSearchGinja.Checked)
				checkBoxSearchAction.Enabled = checkBoxSearchAction.Checked = false;
		}

		private void checkBoxSearchBasic_CheckedChanged(object sender, EventArgs e)
		{
			EnableOrDisableActions();
		}

		private void checkBoxSearchChunk_CheckedChanged(object sender, EventArgs e)
		{
			EnableOrDisableActions();
		}

		private void checkBoxSearchGinja_CheckedChanged(object sender, EventArgs e)
		{
			EnableOrDisableActions();
		}
		#endregion
	}
}