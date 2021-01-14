using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using Newtonsoft.Json;

namespace SonicRetro.SAModel.DataToolbox
{
	public partial class MainForm : Form
	{
		byte[] file;
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

		Properties.Settings Settings = Properties.Settings.Default;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			DataMappingFolder = "SADXPC";
			comboBoxGameSelect.SelectedIndex = 2;
			comboBoxFormat.SelectedIndex = 1;
			textBoxAuthor.Text = Settings.Author;
			ComboBoxBinaryType.Items.Clear();
			comboBoxItemType.SelectedIndex = 0;
			for (int i = 0; i < ModelFileTypes.Length; i++)
			{
				ComboBoxBinaryType.Items.Add(ModelFileTypes[i].name_or_type);
			}
		}

		void ConvertToText(string FileName, bool CStruct = true, bool NJA = false, bool JSON = false, string outdir = "")
		{
			string outpath;
			string outext = ".c";
			if (outdir != "")
			{
				Directory.CreateDirectory(outdir);
				outpath = Path.Combine(outdir, Path.GetFileNameWithoutExtension(FileName));
			}
			else outpath = Path.Combine(Path.GetDirectoryName(FileName), Path.GetFileNameWithoutExtension(FileName));
			string extension = Path.GetExtension(FileName);
			bool dx = false;
			if (comboBoxFormat.SelectedIndex == 1) dx = true;
			switch (extension.ToLowerInvariant())
			{
				case ".sa2lvl":
				case ".sa1lvl":
					if (CStruct)
					{
						LandTable land = LandTable.LoadFromFile(FileName);
						List<string> labels = new List<string>() { land.Name };
						using (StreamWriter sw = File.CreateText(outpath))
						{
							sw.Write("/* Sonic Adventure ");
							LandTableFormat fmt = land.Format;
							switch (land.Format)
							{
								case LandTableFormat.SA1:
								case LandTableFormat.SADX:
									if (comboBoxFormat.SelectedIndex == 1)
									{
										sw.Write("DX");
										fmt = LandTableFormat.SADX;
									}
									else
									{
										sw.Write("1");
										fmt = LandTableFormat.SA1;
									}
									break;
								case LandTableFormat.SA2:
									sw.Write("2");
									fmt = LandTableFormat.SA2;
									break;
								case LandTableFormat.SA2B:
									sw.Write("2 Battle");
									fmt = LandTableFormat.SA2B;
									break;
							}
							sw.WriteLine(" LandTable");
							sw.WriteLine(" * ");
							sw.WriteLine(" * Generated by StructExporter");
							sw.WriteLine(" * ");
							if (!string.IsNullOrEmpty(land.Description))
							{
								sw.Write(" * Description: ");
								sw.WriteLine(land.Description);
								sw.WriteLine(" * ");
							}
							if (!string.IsNullOrEmpty(land.Author))
							{
								sw.Write(" * Author: ");
								sw.WriteLine(land.Author);
								sw.WriteLine(" * ");
							}
							sw.WriteLine(" */");
							sw.WriteLine();
							land.ToStructVariables(sw, fmt, labels, null);
						}
					}
					break;
				case ".sa1mdl":
				case ".sa2mdl":
					ModelFile modelFile = new ModelFile(FileName);
					NJS_OBJECT model = modelFile.Model;
					List<NJS_MOTION> animations = new List<NJS_MOTION>(modelFile.Animations);
					if (CStruct)
					{
						outext = ".c";
						using (StreamWriter sw = File.CreateText(outpath + outext))
						{
							sw.Write("/* NINJA ");
							switch (modelFile.Format)
							{
								case ModelFormat.Basic:
								case ModelFormat.BasicDX:
									if (comboBoxFormat.SelectedIndex == 1)
									{
										sw.Write("Basic (with Sonic Adventure DX additions)");
									}
									else
									{
										sw.Write("Basic");
									}
									break;
								case ModelFormat.Chunk:
									sw.Write("Chunk");
									break;
								case ModelFormat.GC:
									sw.Write("GC");
									break;
							}
							sw.WriteLine(" model");
							sw.WriteLine(" * ");
							sw.WriteLine(" * Generated by StructExporter");
							sw.WriteLine(" * ");
							if (modelFile != null)
							{
								if (!string.IsNullOrEmpty(modelFile.Description))
								{
									sw.Write(" * Description: ");
									sw.WriteLine(modelFile.Description);
									sw.WriteLine(" * ");
								}
								if (!string.IsNullOrEmpty(modelFile.Author))
								{
									sw.Write(" * Author: ");
									sw.WriteLine(modelFile.Author);
									sw.WriteLine(" * ");
								}
							}
							sw.WriteLine(" */");
							sw.WriteLine();
							List<string> labels_m = new List<string>() { model.Name };
							model.ToStructVariables(sw, dx, labels_m, null);
							foreach (NJS_MOTION anim in animations)
							{
								anim.ToStructVariables(sw);
							}
						}
					}
					if (NJA)
					{
						outext = ".nja";
						using (StreamWriter sw2 = File.CreateText(outpath + outext))
						{
							List<string> labels_nj = new List<string>() { model.Name };
							model.ToNJA(sw2, dx, labels_nj, null);
						}
					}
					break;
				case ".saanim":
					NJS_MOTION animation = NJS_MOTION.Load(FileName);
					if (CStruct)
					{
						outext = "_anim.c";
						using (StreamWriter sw = File.CreateText(outpath + outext))
						{
							sw.WriteLine("/* NINJA Motion");
							sw.WriteLine(" * ");
							sw.WriteLine(" * Generated by DataToolbox");
							sw.WriteLine(" * ");
							sw.WriteLine(" */");
							sw.WriteLine();
							animation.ToStructVariables(sw);

						}
					}
					if (JSON)
					{
						outext = ".json";
						JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
						using (TextWriter tw = File.CreateText(outpath + outext))
						using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
							js.Serialize(jtw, animation);
					}
					break;
			}
		}
		private void CheckBox3_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDownAddress.Hexadecimal = CheckBoxHex.Checked;
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			numericUpDownKey.Value = ModelFileTypes[ComboBoxBinaryType.SelectedIndex].key;
		}

		private void fileSelector1_FileNameChanged(object sender, EventArgs e)
		{
			if (!checkBox_NJA.Checked && !checkBox_SAModel.Checked && !checkBox_JSON.Checked && !checkBox_Structs.Checked)
			{ 
				buttonExtract.Enabled = false;
				return;
			}
			if (File.Exists(fileSelector1.FileName))
			{
				file = File.ReadAllBytes(fileSelector1.FileName);
				if (Path.GetExtension(fileSelector1.FileName).Equals(".prs", StringComparison.OrdinalIgnoreCase)) file = FraGag.Compression.Prs.Decompress(file);
				buttonExtract.Enabled = true;
				uint? baseaddr = SA_Tools.HelperFunctions.SetupEXE(ref file);
				if (!baseaddr.HasValue)
				{
					int u = CheckKnownFile(fileSelector1.FileName);
					if (u == -5)
					{
						numericUpDownKey.Value = 0xC600000u;
						comboBoxFormat.SelectedIndex = 2;
					}
					else if (u == -4)
					{
						numericUpDownKey.Value = 0xCB80000u;
						comboBoxFormat.SelectedIndex = 0;
					}
					else if (u != -1)
					{
						numericUpDownKey.Value = KnownBinaryFiles[u].key;
						comboBoxFormat.SelectedIndex = KnownBinaryFiles[u].data_type;
					}
					else comboBoxFormat.SelectedIndex = 1;
				}
				else numericUpDownKey.Value = (int)baseaddr;
				buttonExtract.Enabled = true;
			}
			else buttonExtract.Enabled = false;
		}

		enum SectOffs
		{
			VSize = 8,
			VAddr = 0xC,
			FSize = 0x10,
			FAddr = 0x14,
			Flags = 0x24,
			Size = 0x28
		}

		private void UpdateStartButton()
		{
			if (!File.Exists(fileSelector1.FileName) || ((!checkBox_NJA.Checked && !checkBox_SAModel.Checked && !checkBox_JSON.Checked && !checkBox_Structs.Checked))) buttonExtract.Enabled = false;
			else buttonExtract.Enabled = true;
		}
		private void UpdateConvertButton()
		{
			if (listBoxStructConverter.Items.Count == 0 || ((!checkBox_StructConvNJA.Checked && !checkBox_StructConvJSON.Checked && !checkBox_StructConvStructs.Checked))) buttonConvertBatch.Enabled = false;
			else buttonConvertBatch.Enabled = true;
		}
		private void button1_Click(object sender, EventArgs e)
		{
			bool success = false;
			uint address = (uint)NumericUpDownAddress.Value;
			if (checkBoxMemory.Checked) address -= (uint)numericUpDownKey.Value;
			LandTableFormat format = (LandTableFormat)comboBoxFormat.SelectedIndex;
			LandTableFormat outfmt = format;
			if (format == LandTableFormat.SADX) outfmt = LandTableFormat.SA1;
			ByteConverter.BigEndian = checkBoxBigEndian.Checked;
			Settings.Author = textBoxAuthor.Text;
			Settings.Save();
			SaveFileDialog sd = new SaveFileDialog();
			switch (comboBoxItemType.SelectedIndex)
			{
				//Level
				case 0:
					sd = new SaveFileDialog() { DefaultExt = outfmt.ToString().ToLowerInvariant() + "lvl", Filter = outfmt.ToString().ToUpperInvariant() + "LVL Files|*." + outfmt.ToString().ToLowerInvariant() + "lvl|All Files|*.*" };
					if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						new LandTable(file, (int)NumericUpDownAddress.Value, (uint)numericUpDownKey.Value, format) { Author = textBoxAuthor.Text, Description = textBoxDescription.Text }.SaveToFile(sd.FileName, outfmt);
						if (checkBox_Structs.Checked) ConvertToText(sd.FileName);
						if (!checkBox_SAModel.Checked) File.Delete(sd.FileName);
						success = true;
					}
					break;
				//Model
				case 1:
					sd = new SaveFileDialog() { DefaultExt = outfmt.ToString().ToLowerInvariant() + "mdl", Filter = outfmt.ToString().ToUpperInvariant() + "MDL Files|*." + outfmt.ToString().ToLowerInvariant() + "mdl|All Files|*.*" };
					if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						NJS_OBJECT tempmodel = new NJS_OBJECT(file, (int)address, (uint)numericUpDownKey.Value, (ModelFormat)comboBoxFormat.SelectedIndex, null);
						ModelFile.CreateFile(sd.FileName, tempmodel, null, textBoxAuthor.Text, textBoxDescription.Text, null, (ModelFormat)comboBoxFormat.SelectedIndex);
						ConvertToText(sd.FileName, checkBox_Structs.Checked, checkBox_NJA.Checked, false);
						if (!checkBox_SAModel.Checked) File.Delete(sd.FileName);
						success = true;
					}
					break;
				//Action
				case 2:
					sd = new SaveFileDialog() { DefaultExt = outfmt.ToString().ToLowerInvariant() + "mdl", Filter = outfmt.ToString().ToUpperInvariant() + "MDL Files|*." + outfmt.ToString().ToLowerInvariant() + "mdl|All Files|*.*" };
					if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						//Model
						NJS_ACTION tempaction = new NJS_ACTION(file, (int)address, (uint)numericUpDownKey.Value, (ModelFormat)comboBoxFormat.SelectedIndex, null);
						NJS_OBJECT tempmodel = tempaction.Model;
						ModelFile.CreateFile(sd.FileName, tempmodel, null, textBoxAuthor.Text, textBoxDescription.Text, null, (ModelFormat)comboBoxFormat.SelectedIndex);
						ConvertToText(sd.FileName, checkBox_Structs.Checked, checkBox_NJA.Checked, false);
						if (!checkBox_SAModel.Checked) File.Delete(sd.FileName);

						//Action
						string saanimPath = Path.Combine(Path.GetDirectoryName(sd.FileName), Path.GetFileNameWithoutExtension(sd.FileName) + ".saanim");

						tempaction.Animation.Save(saanimPath);
						ConvertToText(saanimPath, checkBox_Structs.Checked, false, checkBox_JSON.Checked);

						if (checkBox_SAModel.Checked)
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
			if (!checkBoxSameOutputFolderBatch.Checked)
			{
				SaveFileDialog sav = new SaveFileDialog() { Filter = "Folder|*.*", Title = "Select output folder" };
				if (sav.ShowDialog() == System.Windows.Forms.DialogResult.OK) outdir = sav.FileName;
					else return;
			}
			foreach (string item in listBoxStructConverter.Items)
			{
				ConvertToText(item, checkBox_StructConvStructs.Checked, checkBox_StructConvNJA.Checked, checkBox_StructConvJSON.Checked, outdir);
			}
			string finished = "Struct conversion finished!";
			if (outdir != "") finished += "\nConverted files are saved to " + outdir + ".";
			MessageBox.Show(finished, "Struct Converter", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void comboBoxGameSelect_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (comboBoxGameSelect.SelectedIndex)
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
					DataMappingFolder = "SADXX360";
					break;
				case 4:
					DataMappingFolder = "SA2";
					break;
				case 5:
					DataMappingFolder = "SA2TheTrial";
					break;
				case 6:
					DataMappingFolder = "SA2PC";
					break;
			}
		}

		private void button_AddFilesSplit_Click(object sender, EventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog() { Filter = "Binary files|*.exe;*.dll;*.bin;*.mdl|All files|*.*", Multiselect = true };
			if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				listBox_SplitFiles.Items.AddRange(op.FileNames);
				buttonSplit.Enabled = true;
			}
		}

		private void buttonSplit_Click(object sender, EventArgs e)
		{
			string outdir = "";
			if (!checkBoxSameFolderSplit.Checked)
			{
				SaveFileDialog sd = new SaveFileDialog() { Title = "Select output folder", FileName="output", DefaultExt = "" };
				if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					outdir = sd.FileName;
				}
				else return;
			}
				SplitProgress spl = new SplitProgress(null, listBox_SplitFiles.Items.Cast<String>().ToList(), DataMappingFolder, outdir, checkBoxFindAllSplit.Checked);
			spl.ShowDialog();
		}

		private void listBox_SplitFiles_DragDrop(object sender, DragEventArgs e)
		{
			string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			for (int u = 0; u < fileList.Length; u++)
			{
				if(!listBox_SplitFiles.Items.Contains(fileList[u])) listBox_SplitFiles.Items.Add(fileList[u]);
			}
			buttonSplit.Enabled = true;
		}

		private void listBox_SplitFiles_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void listBoxStructConverter_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBoxStructConverter.SelectedIndices.Count > 0) buttonRemoveSelBatch.Enabled = true;
			else buttonRemoveSelBatch.Enabled = false;
		}

		private void buttonRemoveSplit_Click(object sender, EventArgs e)
		{
			var selectedItems = listBox_SplitFiles.SelectedItems.Cast<String>().ToList();
			foreach (var item in selectedItems)
				listBox_SplitFiles.Items.Remove(item);
			if (listBox_SplitFiles.Items.Count == 0) buttonSplit.Enabled = false;
		}

		private void buttonRemoveSelBatch_Click(object sender, EventArgs e)
		{
			var selectedItems = listBoxStructConverter.SelectedItems.Cast<String>().ToList();
			foreach (var item in selectedItems)
				listBoxStructConverter.Items.Remove(item);
			if (listBoxStructConverter.Items.Count == 0) buttonConvertBatch.Enabled = false;
		}

		private void buttonRemoveAllBatch_Click(object sender, EventArgs e)
		{
			listBoxStructConverter.Items.Clear();
			buttonConvertBatch.Enabled = false;
		}

		private void buttonClearAllSplit_Click(object sender, EventArgs e)
		{
			listBox_SplitFiles.Items.Clear();
			buttonSplit.Enabled = false;
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

		private void listBox_SplitFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox_SplitFiles.SelectedIndices.Count > 0) buttonRemoveSplit.Enabled = true;
			else buttonRemoveSplit.Enabled = false;
		}

		private void checkBox_SAModel_CheckedChanged(object sender, EventArgs e)
		{
			UpdateStartButton();
		}

		private void checkBox_Structs_CheckedChanged(object sender, EventArgs e)
		{
			UpdateStartButton();
		}

		private void checkBox_NJA_CheckedChanged(object sender, EventArgs e)
		{
			UpdateStartButton();
		}

		private void checkBox_JSON_CheckedChanged(object sender, EventArgs e)
		{
			UpdateStartButton();
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
	}
}