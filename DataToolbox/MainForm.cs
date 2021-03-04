using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SonicRetro.SAModel.DataToolbox
{
	public partial class MainForm : Form
	{
		byte[] file;
		Properties.Settings Settings = Properties.Settings.Default;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			// Binary File Extractor defaults
			DataMappingFolder = "SADXPC";
			comboBoxSplitGameSelect.SelectedIndex = 2;
			comboBoxBinaryFormat.SelectedIndex = 1;
			textBoxBinaryAuthor.Text = Settings.Author;
			ComboBoxBinaryType.Items.Clear();
			comboBoxBinaryItemType.SelectedIndex = 0;
			for (int i = 0; i < ModelFileTypes.Length; i++)
			{
				ComboBoxBinaryType.Items.Add(ModelFileTypes[i].name_or_type);
			}
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

		public static string DataMappingFolder;

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
				//Level
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
				//Model
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
				//Action
				case 2:
					sd = new SaveFileDialog() { DefaultExt = outfmt.ToString().ToLowerInvariant() + "mdl", Filter = outfmt.ToString().ToUpperInvariant() + "MDL Files|*." + outfmt.ToString().ToLowerInvariant() + "mdl|All Files|*.*" };
					if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						//Model
						NJS_ACTION tempaction = new NJS_ACTION(file, (int)address, (uint)numericUpDownBinaryKey.Value, (ModelFormat)comboBoxBinaryFormat.SelectedIndex, null);
						NJS_OBJECT tempmodel = tempaction.Model;
						ModelFile.CreateFile(sd.FileName, tempmodel, null, textBoxBinaryAuthor.Text, textBoxBinaryDescription.Text, null, (ModelFormat)comboBoxBinaryFormat.SelectedIndex);
						ConvertToText(sd.FileName, checkBoxBinaryStructs.Checked, checkBoxBinaryNJA.Checked, false);
						if (!checkBoxBinarySAModel.Checked) File.Delete(sd.FileName);

						//Action
						string saanimPath = Path.Combine(Path.GetDirectoryName(sd.FileName), Path.GetFileNameWithoutExtension(sd.FileName) + ".saanim");

						tempaction.Animation.Save(saanimPath);
						ConvertToText(saanimPath, checkBoxBinaryStructs.Checked, false, checkBoxBinaryJSON.Checked);

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
				uint? baseaddr = SA_Tools.HelperFunctions.SetupEXE(ref file);
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
				StructConversion.ConvertFileToText(FileName, StructConversion.TextType.CStructs, outpath + outext, dx);
			}
			if (NJA)
			{
				outext = ".nja";
				StructConversion.ConvertFileToText(FileName, StructConversion.TextType.NJA, outpath + outext, dx);
			}
			if (JSON)
			{
				outext = ".json";
				StructConversion.ConvertFileToText(FileName, StructConversion.TextType.JSON, outpath + outext, dx);
			}
		}

		private void UpdateConvertButton()
		{
			if (listBoxStructConverter.Items.Count == 0 || ((!checkBoxStructConvNJA.Checked && !checkBoxStructConvJSON.Checked && !checkBoxStructConvStructs.Checked))) buttonStructConvConvertBatch.Enabled = false;
			else buttonStructConvConvertBatch.Enabled = true;
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog() { Filter = "Levels, models and animations|*.sa?lvl;*.sa?mdl;*.saanim|All files|*.*", Multiselect = true };
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

		private void listBoxStructConverter_DragDrop(object sender, DragEventArgs e)
		{
			string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			for (int u = 0; u < fileList.Length; u++)
			{
				if (!listBoxStructConverter.Items.Contains(fileList[u])) listBoxStructConverter.Items.Add(fileList[u]);
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
			SplitProgress spl = new SplitProgress(null, listBoxSplitFiles.Items.Cast<String>().ToList(), DataMappingFolder, outdir, checkBoxFindAllSplit.Checked);
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
		private void comboBoxGameSelect_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (comboBoxSplitGameSelect.SelectedIndex)
			{
				case 0:
					DataMappingFolder = "SA1";
					break;
				case 1:
					DataMappingFolder = "AutoDemo";
					break;
				case 2:
					DataMappingFolder = "SADXPC";
					break;
				case 3:
					DataMappingFolder = "SADXGC";
					break;
				case 4:
					DataMappingFolder = "SADXX360";
					break;
				case 5:
					DataMappingFolder = "SA2";
					break;
				case 6:
					DataMappingFolder = "SA2TheTrial";
					break;
				case 7:
					DataMappingFolder = "SA2PC";
					break;
			}
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
			SplitProgress spl = new SplitProgress(null, files, null, outdir, false, checkBoxMDLBigEndian.Checked ? 2 : 1);
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
	}
}