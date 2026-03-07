using SAModel;
using SAModel.SAEditorCommon;
using System.Text;
using System.Windows.Forms;
using static SAModel.SAEditorCommon.SettingsFile;

namespace SA2LightFogEditor
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}
		enum LightFogFileTypes
		{
			Null,
			Light,
			Fog,
			LightGC,
			FogGC
		}
		List<Size> formSizes = new List<Size>();
		public Size currentFormSize { get; set; }
		List<string> recentFiles = new List<string>();
		string filename = null;
		string currentFormatDialog = "";
		bool bigEndian;
		string bigEndianDialog = "";
		public Vertex currentColor { get; set; }
		public Vertex currentGCColor { get; set; }
		public Vertex currentGCAmbient { get; set; }
		List<LightData> lights = new List<LightData>();
		List<GCLightData> gclights = new List<GCLightData>();
		LightData currentLightData { get { return lights[(int)lightSetNumericUpDown.Value - 1]; } }
		GCLightData currentGCLightData { get { return gclights[(int)lightGCSetNumericUpDown.Value - 1]; } }
		FogData FogFileData { get; set; }
		LightFogFileTypes currentFormat;
		Settings_SA2LightFogEditor settingsFile;
		private void MainForm_Load(object sender, EventArgs e)
		{
			formSizes.Add(new Size(groupBoxLightData.Width + 60, groupBoxLightData.Height + 160));
			formSizes.Add(new Size(groupBoxGCLightData.Width + 60, groupBoxGCLightData.Height + 160));
			formSizes.Add(new Size(groupBoxFogData.Width + 60, groupBoxFogData.Height + 160));
			Size = formSizes[0];
			groupBoxGCLightData.Location = groupBoxFogData.Location = groupBoxLightData.Location;
			settingsFile = Settings_SA2LightFogEditor.Load();
			if (settingsFile.RecentFiles != null)
				recentFiles = new List<string>(settingsFile.RecentFiles.Where(a => File.Exists(a)));
			if (recentFiles.Count > 0)
			{
				UpdateRecentFiles();
			}
			bigEndian = settingsFile.BigEndian;
			switch (settingsFile.Format)
			{
				case 0:
				default:
					currentFormat = LightFogFileTypes.Light;
					currentFormatDialog = "Light file";
					break;
				case 1:
					currentFormat = LightFogFileTypes.Fog;
					currentFormatDialog = "Fog file";
					break;
				case 2:
					currentFormat = LightFogFileTypes.LightGC;
					currentFormatDialog = "GC Light file";
					break;
				case 3:
					currentFormat = LightFogFileTypes.FogGC;
					currentFormatDialog = "GC Fog file";
					break;
			}
			InitializeLightFogData();
			if (filename != null)
				LoadFile(filename);
		}
		private void LoadFile(string filename)
		{
			this.filename = filename;
			byte[] fc = File.ReadAllBytes(filename);
			int address = 0;
			bigEndian = false;
			ClearLightFogData();
			AddRecentFile(filename);
			if (fc.Length == 0x10 || fc.Length == 0x240)
			{
				bigEndian = true;
				bigEndianToolStripMenuItem.Checked = true;
			}
			else
			{
				var addr = 0;
				var addrfloat = 0;
				var ile = ByteConverter.ToUInt16(fc, 0);
				var ilefloat = ByteConverter.ToSingle(fc, 0xC);
				switch (fc.Length)
				{
					case 0x210:
					default:
						ByteConverter.BigEndian = false;
						if (ile == 0)
						{
							ile = ByteConverter.ToUInt16(fc, 2);
							addr = 2;
							if (ile == 0)
							{
								ilefloat = ByteConverter.ToSingle(fc, 8);
								addrfloat = 8;
							}
						}
						ByteConverter.BigEndian = true;
						bigEndian = true;
						bigEndianToolStripMenuItem.Checked = true;
						if (ile == 0)
						{
							if (ilefloat > ByteConverter.ToSingle(fc, addrfloat))
							{
								bigEndian = false;
								bigEndianToolStripMenuItem.Checked = false;
							}
						}
						else
						{
							if (ile < ByteConverter.ToUInt16(fc, addr))
							{
								bigEndian = false;
								bigEndianToolStripMenuItem.Checked = false;
							}
						}
						break;
					case 0x180:
						ByteConverter.BigEndian = false;
						addr = 0xC;
						ByteConverter.BigEndian = true;
						bigEndianToolStripMenuItem.Checked = true;
						bigEndian = true;
						if (ilefloat > ByteConverter.ToSingle(fc, addr))
						{
							bigEndian = false;
							bigEndianToolStripMenuItem.Checked = false;
						}
						break;

				}
			}
			ByteConverter.BigEndian = bigEndian;
			if (bigEndianToolStripMenuItem.Checked)
			{
				bigEndianDialog = " in Big Endian";
			}
			else
			{
				bigEndianDialog = "";
			}
			if (fc.Length == 0x10)
			{
				int l = 0;
				currentFormat = LightFogFileTypes.FogGC;
				currentFormatDialog = "GC Fog file";
				groupBoxLightData.Visible = false;
				groupBoxLightData.Enabled = false;
				groupBoxGCLightData.Visible = false;
				groupBoxGCLightData.Enabled = false;
				groupBoxFogData.Visible = true;
				groupBoxFogData.Enabled = true;
				groupBoxFogTable.Enabled = false;
				Size = formSizes[2];
				FogFileData = new FogData(fc, bigEndian, false);
				do
				{
					lights.Add(new LightData());
					gclights.Add(new GCLightData());
					l++;
				}
				while (l < 12);
				Text = "SA2 Fog Editor - " + Path.GetFileName(filename);
			}
			else if (fc.Length == 0x210)
			{
				int l = 0;
				currentFormat = LightFogFileTypes.Fog;
				currentFormatDialog = "Fog file";
				groupBoxLightData.Visible = false;
				groupBoxLightData.Enabled = false;
				groupBoxGCLightData.Visible = false;
				groupBoxGCLightData.Enabled = false;
				groupBoxFogData.Visible = true;
				groupBoxFogData.Enabled = true;
				groupBoxFogTable.Enabled = true;
				FogFileData = new FogData(fc, bigEndian, true);
				Size = formSizes[2];
				do
				{
					lights.Add(new LightData());
					gclights.Add(new GCLightData());
					l++;
				}
				while (l < 12);
				Text = "SA2 Fog Editor - " + Path.GetFileName(filename);
			}
			else if (fc.Length == 0x180)
			{
				int l = 0;
				address = 0;
				currentFormat = LightFogFileTypes.Light;
				currentFormatDialog = "Light file";
				groupBoxLightData.Visible = true;
				groupBoxLightData.Enabled = true;
				groupBoxGCLightData.Visible = false;
				groupBoxGCLightData.Enabled = false;
				groupBoxFogData.Visible = false;
				groupBoxFogData.Enabled = false;
				Size = formSizes[0];
				do
				{
					lights.Add(new LightData(fc, address));
					address += LightData.Size;
				}
				while (address < fc.Length);
				do
				{
					gclights.Add(new GCLightData());
					l++;
				}
				while (l < 12);
				FogFileData = new FogData();
				Text = "SA2 Light Editor - " + Path.GetFileName(filename);
			}
			else
			{
				int l = 0;
				bigEndian = true;
				bigEndianToolStripMenuItem.Checked = true;
				address = 0;
				currentFormat = LightFogFileTypes.LightGC;
				currentFormatDialog = "GC Light file";
				groupBoxLightData.Visible = false;
				groupBoxLightData.Enabled = false;
				groupBoxGCLightData.Visible = true;
				groupBoxGCLightData.Enabled = true;
				groupBoxFogData.Visible = false;
				groupBoxFogData.Enabled = false;
				Size = formSizes[1];
				do
				{
					gclights.Add(new GCLightData(fc, address));
					address += GCLightData.Size;
				}
				while (address < fc.Length);
				do
				{
					lights.Add(new LightData());
					l++;
				}
				while (l < 12);
				FogFileData = new FogData();
				Text = "SA2 Light Editor - " + Path.GetFileName(filename);
			}
			UpdateLightFogData();
		}
		private void ClearLightFogData()
		{
			lights.Clear();
			gclights.Clear();
			FogFileData = new FogData();
		}
		private void UpdateLightFogData()
		{
			UpdateGUILightInfo();
			UpdateGUIGCLightInfo();
			UpdateGUIFogInfo();
		}

		private void UpdateGUILightInfo()
		{
			lightXDirTextBox.Text = currentLightData.LightDir.X.ToString();
			lightYDirTextBox.Text = currentLightData.LightDir.Y.ToString();
			lightZDirTextBox.Text = currentLightData.LightDir.Z.ToString();
			lightColorPanel.BackColor = Color.FromArgb(255,
				(byte)(currentLightData.Color.X * 255f),
				(byte)(currentLightData.Color.Y * 255f),
				(byte)(currentLightData.Color.Z * 255f));
			lightColorMultiTextBox.Text = currentLightData.ColorMulti.ToString();
			lightAmbientMultiTextBox.Text = currentLightData.AmbientMulti.ToString();
		}
		private void UpdateGUIGCLightInfo()
		{
			lightGCXDirTextBox.Text = currentGCLightData.LightDir.X.ToString();
			lightGCYDirTextBox.Text = currentGCLightData.LightDir.Y.ToString();
			lightGCZDirTextBox.Text = currentGCLightData.LightDir.Z.ToString();
			float lightmultiplier = GetPotentialGCLightMultiplier(currentGCLightData.Color);
			float ambientmultiplier = GetPotentialGCLightMultiplier(currentGCLightData.Ambient);
			float lightRSanity = currentGCLightData.Color.X / lightmultiplier;
			float lightGSanity = currentGCLightData.Color.Y / lightmultiplier;
			float lightBSanity = currentGCLightData.Color.Z / lightmultiplier;
			float ambRSanity = currentGCLightData.Ambient.X / ambientmultiplier;
			float ambGSanity = currentGCLightData.Ambient.Y / ambientmultiplier;
			float ambBSanity = currentGCLightData.Ambient.Z / ambientmultiplier;
			if (currentGCLightData.Color.X > 1 || currentGCLightData.Color.Y > 1 ||
				currentGCLightData.Color.Z > 1)
			{
				lightGCColorPanel.BackColor = Color.FromArgb(255,
					(int)(lightRSanity * 255),
					(int)(lightGSanity * 255),
					(int)(lightBSanity * 255));
			}
			else
			{
				lightGCColorPanel.BackColor = Color.FromArgb(255,
					(int)(currentGCLightData.Color.X * 255f),
					(int)(currentGCLightData.Color.Y * 255f),
					(int)(currentGCLightData.Color.Z * 255f));
			}
			if (currentGCLightData.Ambient.X > 1 || currentGCLightData.Ambient.Y > 1 ||
				currentGCLightData.Ambient.Z > 1)
				lightGCAmbientPanel.BackColor = Color.FromArgb(255,
					(int)(ambRSanity * 255),
					(int)(ambGSanity * 255),
					(int)(ambBSanity * 255));
			else
			{
				lightGCAmbientPanel.BackColor = Color.FromArgb(255,
					(int)(currentGCLightData.Ambient.X * 255f),
					(int)(currentGCLightData.Ambient.Y * 255f),
					(int)(currentGCLightData.Ambient.Z * 255f));
			}
			lightGCOverrideCheckBox.Checked = currentGCLightData.Override == 1;
			lightGCUnk1NumericUpDown.Value = currentGCLightData.Unk1;
			lightGCUnk2NumericUpDown.Value = currentGCLightData.Unk2;
		}
		private void UpdateGUIFogInfo()
		{
			fogTypeNumericUpDown.Value = FogFileData.Type;
			fogUnkNumericUpDown.Value = FogFileData.Unk;
			fogAlphaNumericUpDown.Value = FogFileData.A;
			fogColorPanel.BackColor = Color.FromArgb(255,
					FogFileData.R,
					FogFileData.G,
					FogFileData.B);
			fogMaxDistanceTextBox.Text = FogFileData.MaxDist.ToString();
			fogMinDistanceTextBox.Text = FogFileData.MinDist.ToString();
			if (FogFileData.FogTable != null)
			{
				string fogstring = "";
				foreach (float data in FogFileData.FogTable)
				{
					fogstring += data.ToString() + "\n";
				}
				fogTableRichTextBox.Text = fogstring;
			}
		}
		private void UpdateRecentFiles()
		{
			recentFilesToolStripMenuItem.DropDownItems.Clear();
			foreach (string item in recentFiles)
				recentFilesToolStripMenuItem.DropDownItems.Add(item);
		}
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save before exiting?", "SA2 Light/Fog Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button3))
			{
				case DialogResult.Yes:
					break;
				case DialogResult.Cancel:
					e.Cancel = true;
					return;
			}
			settingsFile.Save();
		}
		private void AddRecentFile(string filename)
		{
			if (recentFiles.Contains(filename))
				recentFiles.Remove(filename);
			recentFiles.Insert(0, filename);
			if (recentFiles.Count > 10)
				recentFiles.RemoveAt(10);
			UpdateRecentFiles();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()
			{
				DefaultExt = "bin",
				Filter = "All Compatible Files|stg*_light.bin;*_fog*.bin;stg*_light_gc.bin;|" +
				"Light Files|stg*_light.bin;stg*_light_gc.bin|" +
				"Fog Files|*_fog*.bin|" +
				"All Files|*.*"
			})
				if (dlg.ShowDialog(this) == DialogResult.OK)
					LoadFile(dlg.FileName);
		}

		private float GetPotentialGCLightMultiplier(Vertex color)
		{
			return Math.Max(Math.Max(color.X, color.Y), color.Z);
		}
		private void lightSetNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUILightInfo();
		}

		private void lightGCSetNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUIGCLightInfo();
		}

		private void lightGCOverrideCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			currentGCLightData.Override = lightGCOverrideCheckBox.Checked ? 1 : 0;
		}

		private void lightColorPanel_Click(object sender, EventArgs e)
		{
			colorDialog.Color = lightColorPanel.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				float lightR = colorDialog.Color.R / 255f;
				float lightG = colorDialog.Color.G / 255f;
				float lightB = colorDialog.Color.B / 255f;
				lightColorPanel.BackColor = colorDialog.Color;
				currentLightData.Color.X = (float)lightR;
				currentLightData.Color.Y = (float)lightG;
				currentLightData.Color.Z = (float)lightB;
			}
		}

		private void lightGCColorPanel_Click(object sender, EventArgs e)
		{
			colorDialog.Color = lightGCColorPanel.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				float lightR = colorDialog.Color.R / 255f;
				float lightG = colorDialog.Color.G / 255f;
				float lightB = colorDialog.Color.B / 255f;
				lightGCColorPanel.BackColor = colorDialog.Color;
				currentGCLightData.Color.X = (float)lightR;
				currentGCLightData.Color.Y = (float)lightG;
				currentGCLightData.Color.Z = (float)lightB;
			}
		}

		private void lightGCAmbientPanel_Click(object sender, EventArgs e)
		{
			colorDialog.Color = lightGCAmbientPanel.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				float lightR = colorDialog.Color.R / 255f;
				float lightG = colorDialog.Color.G / 255f;
				float lightB = colorDialog.Color.B / 255f;
				lightGCAmbientPanel.BackColor = colorDialog.Color;
				currentGCLightData.Ambient.X = (float)lightR;
				currentGCLightData.Ambient.Y = (float)lightG;
				currentGCLightData.Ambient.Z = (float)lightB;
			}
		}

		private void fogColorPanel_Click(object sender, EventArgs e)
		{
			colorDialog.Color = fogColorPanel.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				fogColorPanel.BackColor = colorDialog.Color;
				FogFileData.R = colorDialog.Color.R;
				FogFileData.G = colorDialog.Color.G;
				FogFileData.B = colorDialog.Color.B;
			}
		}

		private void lightXDirTextBox_TextChanged(object sender, EventArgs e)
		{
			currentLightData.LightDir.X = float.Parse(lightXDirTextBox.Text);
		}

		private void lightYDirTextBox_TextChanged(object sender, EventArgs e)
		{
			currentLightData.LightDir.Y = float.Parse(lightYDirTextBox.Text);
		}

		private void lightZDirTextBox_TextChanged(object sender, EventArgs e)
		{
			currentLightData.LightDir.Z = float.Parse(lightZDirTextBox.Text);
		}

		private void lightColorMultiTextBox_TextChanged(object sender, EventArgs e)
		{
			currentLightData.ColorMulti = float.Parse(lightColorMultiTextBox.Text);
		}

		private void lightAmbientMultiTextBox_TextChanged(object sender, EventArgs e)
		{
			currentLightData.AmbientMulti = float.Parse(lightAmbientMultiTextBox.Text);
		}

		private void lightGCXDirTextBox_TextChanged(object sender, EventArgs e)
		{
			currentGCLightData.LightDir.X = float.Parse(lightGCXDirTextBox.Text);
		}

		private void lightGCYDirTextBox_TextChanged(object sender, EventArgs e)
		{
			currentGCLightData.LightDir.Y = float.Parse(lightGCYDirTextBox.Text);
		}

		private void lightGCZDirTextBox_TextChanged(object sender, EventArgs e)
		{ 
			currentGCLightData.LightDir.Z = float.Parse(lightGCZDirTextBox.Text);
		}

		private void lightGCUnk1NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			currentGCLightData.Unk1 = (int)lightGCUnk1NumericUpDown.Value;
		}

		private void lightGCUnk2NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			currentGCLightData.Unk2 = (int)lightGCUnk2NumericUpDown.Value;
		}

		private void fogTypeNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			FogFileData.Type = (short)fogTypeNumericUpDown.Value;
		}

		private void fogUnkNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			FogFileData.Unk = (short)fogUnkNumericUpDown.Value;
		}

		private void fogAlphaNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			FogFileData.A = (byte)fogAlphaNumericUpDown.Value;
		}

		private void fogMaxDistanceTextBox_TextChanged(object sender, EventArgs e)
		{
			FogFileData.MaxDist = float.Parse(fogMaxDistanceTextBox.Text);
		}

		private void fogMinDistanceTextBox_TextChanged(object sender, EventArgs e)
		{
			FogFileData.MinDist = float.Parse(fogMinDistanceTextBox.Text);
		}

		private void fogTableRichTextBox_TextChanged(object sender, EventArgs e)
		{
			List<float>newtable = new List<float>(128);
			int i = 0;
			do
			{
				newtable.Add(0.0f);
				i++;
			}
			while (i < 128);
			if (FogFileData.FogTable != null)
			{
				int buffer;
				if (FogFileData.FogTable.Count == fogTableRichTextBox.Lines.Length)
				{
					for (int t = 0; t < FogFileData.FogTable.Count; t++)
					{
						try
						{
							FogFileData.FogTable[t] = float.Parse(fogTableRichTextBox.Lines[t]);
						}
						catch
						{
							FogFileData.FogTable[t] = 0.0f;
						}
					}
				}
				else
				{
					if (128 > fogTableRichTextBox.Lines.Length)
					{
						buffer = 128 - fogTableRichTextBox.Lines.Length;
						for (int t = 0; t < fogTableRichTextBox.Lines.Length; t++)
						{
							try
							{
								newtable[t] = float.Parse(fogTableRichTextBox.Lines[t]);
							}
							catch
							{
								newtable[t] = 0.0f;
							}
						}
						FogFileData.FogTable = newtable;
						do
						{
							FogFileData.FogTable.Add(0.0f);
							buffer--;
						}
						while (buffer > 0);
					}
					else
					{
						for (int t = 0; t < 128; t++)
						{
							try
							{
								newtable[t] = float.Parse(fogTableRichTextBox.Lines[t]);
							}
							catch
							{
								newtable[t] = 0.0f;
							}
						}
						FogFileData.FogTable = newtable;
					}
				}
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LoadFile(recentFiles[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (filename == null)
			{
				saveAsToolStripMenuItem_Click(sender, e);
				return;
			}
			SaveFile();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, $"File will be saved as a {currentFormatDialog}{bigEndianDialog}. Continue?", "SA2 Light/Fog Editor", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button3) == DialogResult.No)
			{
				return;
			}
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "bin", Filter = "Supported Files|*.bin|All Files|*.*" })
			{
				if (filename != null)
				{
					dlg.FileName = Path.GetFileName(filename);
					dlg.InitialDirectory = Path.GetDirectoryName(filename);
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					filename = dlg.FileName;
					SaveFile();
					AddRecentFile(filename);
					Text = "SA2 Light/Fog Editor - " + Path.GetFileName(filename);
				}
			}
		}
		private void SaveFile()
		{
			ByteConverter.BigEndian = bigEndian;
			List<byte> fc = new List<byte>();
			switch (currentFormat)
			{
				case LightFogFileTypes.Light:
				default:
					foreach (LightData light in lights)
						fc.AddRange(light.GetBytes());
					break;
				case LightFogFileTypes.LightGC:
					foreach (GCLightData gclight in gclights)
						fc.AddRange(gclight.GetBytes());
					break;
				case LightFogFileTypes.Fog:
					fc.AddRange(FogFileData.GetBytes(bigEndian, true));
					break;
				case LightFogFileTypes.FogGC:
					fc.AddRange(FogFileData.GetBytes(bigEndian, false));
					break;
			}
			File.WriteAllBytes(filename, fc.ToArray());
		}

		private void bigEndianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (bigEndianToolStripMenuItem.Checked)
			{
				bigEndianToolStripMenuItem.Checked = false;
				bigEndian = false;
				bigEndianDialog = "";
			}
			else
			{
				bigEndianToolStripMenuItem.Checked = true;
				bigEndian = true;
				bigEndianDialog = " in Big Endian";
			}
		}

		private void lightFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			filename = null;
			Text = "SA2 Light Editor";
			ClearLightFogData();
			currentFormat = LightFogFileTypes.Light;
			currentFormatDialog = "Light file";
			groupBoxLightData.Visible = true;
			groupBoxLightData.Enabled = true;
			groupBoxGCLightData.Visible = false;
			groupBoxGCLightData.Enabled = false;
			groupBoxFogData.Visible = false;
			groupBoxFogData.Enabled = false;
			InitializeLightFogData();
			Size = formSizes[0];
		}

		private void lightFileGCToolStripMenuItem_Click(object sender, EventArgs e)
		{
			filename = null;
			Text = "SA2 Light Editor";
			ClearLightFogData();
			currentFormat = LightFogFileTypes.LightGC;
			currentFormatDialog = "GC Light file";
			groupBoxLightData.Visible = false;
			groupBoxLightData.Enabled = false;
			groupBoxGCLightData.Visible = true;
			groupBoxGCLightData.Enabled = true;
			groupBoxFogData.Visible = false;
			groupBoxFogData.Enabled = false;
			InitializeLightFogData();
			Size = formSizes[1];
		}

		private void fogFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			filename = null;
			Text = "SA2 Fog Editor";
			ClearLightFogData();
			currentFormat = LightFogFileTypes.Fog;
			currentFormatDialog = "Fog file";
			groupBoxLightData.Visible = false;
			groupBoxLightData.Enabled = false;
			groupBoxGCLightData.Visible = false;
			groupBoxGCLightData.Enabled = false;
			groupBoxFogData.Visible = true;
			groupBoxFogData.Enabled = true;
			groupBoxFogTable.Enabled = true;
			InitializeLightFogData();
			Size = formSizes[2];
		}
		private void InitializeLightFogData()
		{
			int l = 0;
			do
			{
				lights.Add(new LightData());
				gclights.Add(new GCLightData());
				l++;
			}
			while (l < 12);
			FogFileData = new FogData();
		}
		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void fogFileGCToolStripMenuItem_Click(object sender, EventArgs e)
		{
			filename = null;
			Text = "SA2 Fog Editor";
			ClearLightFogData();
			currentFormat = LightFogFileTypes.FogGC;
			currentFormatDialog = "GC Fog file";
			groupBoxLightData.Visible = false;
			groupBoxLightData.Enabled = false;
			groupBoxGCLightData.Visible = false;
			groupBoxGCLightData.Enabled = false;
			groupBoxFogData.Visible = true;
			groupBoxFogData.Enabled = true;
			groupBoxFogTable.Enabled = false;
			InitializeLightFogData();
			Size = formSizes[2];
		}
	}
	public class LightData
	{
		public Vertex LightDir { get; set; }
		public float ColorMulti { get; set; }
		public float AmbientMulti { get; set; }
		public Vertex Color { get; set; }
		public static int Size { get { return 0x20; } }
		public LightData() 
		{ 
			LightDir = new Vertex(0, 0, 0);
			Color = new Vertex(0, 0, 0);
		}
		public LightData(byte[] file, int address)
		{
			LightDir = new Vertex(file, address);
			ColorMulti = ByteConverter.ToSingle(file, address + 0xC);
			AmbientMulti = ByteConverter.ToSingle(file, address + 0x10);
			Color = new Vertex(file, address + 0x14);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(LightDir.GetBytes());
			result.AddRange(ByteConverter.GetBytes(ColorMulti));
			result.AddRange(ByteConverter.GetBytes(AmbientMulti));
			result.AddRange(Color.GetBytes());
			result.Align(0x20);
			return result.ToArray();
		}
	}
	public class GCLightData
	{
		public Vertex LightDir { get; set; }
		public Vertex Color { get; set; }
		public Vertex Ambient { get; set; }
		public int Override { get; set; }
		public int Unk1 { get; set; }
		public int Unk2 { get; set; }
		public static int Size { get { return 0x30; } }
		public GCLightData() 
		{
			LightDir = new Vertex(0, 0, 0);
			Color = new Vertex(0, 0, 0);
			Ambient = new Vertex(0, 0, 0);
		}
		public GCLightData(byte[] file, int address)
		{
			LightDir = new Vertex(file, address);
			Color = new Vertex(file, address + 0xC);
			Ambient = new Vertex(file, address + 0x18);
			Override = ByteConverter.ToInt32(file, address + 0x24);
			Unk1 = ByteConverter.ToInt32(file, address + 0x28);
			Unk2 = ByteConverter.ToInt32(file, address + 0x2C);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(LightDir.GetBytes());
			result.AddRange(Color.GetBytes());
			result.AddRange(Ambient.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Override));
			result.AddRange(ByteConverter.GetBytes(Unk1));
			result.AddRange(ByteConverter.GetBytes(Unk2));
			result.Align(0x30);
			return result.ToArray();
		}
	}
	public class FogData
	{
		public short Type { get; set; }
		public short Unk { get; set; }
		public byte A { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public float MaxDist { get; set; }
		public float MinDist { get; set; }
		public List<float> FogTable = new List<float>();
		public FogData() 
		{
			int i = 0;
			do
			{
				FogTable.Add(0.0f);
				i++;
			}
			while (i < 128);
		}
		public FogData(byte[] file, bool bigEndian, bool DC)
		{
			Type = ByteConverter.ToInt16(file, 0);
			Unk = ByteConverter.ToInt16(file, 2);
			if (bigEndian)
			{
				A = file[4];
				R = file[5];
				G = file[6];
				B = file[7];
			}
			else
			{
				B = file[4];
				G = file[5];
				R = file[6];
				A = file[7];
			}
			MaxDist = ByteConverter.ToSingle(file, 8);
			MinDist = ByteConverter.ToSingle(file, 0xC);
			if (DC)
			{
				if (bigEndian)
				{
					byte[] arrayfix;
					byte[] arraytest = new[] { file[0x12], file[0x13], file[0x10], file[0x11] };
					float testnormal = ByteConverter.ToSingle(file, 0x10);
					float testfix = ByteConverter.ToSingle(arraytest, 0);
					if (testfix > 1 || testfix < 0 || testfix > testnormal)
					{
						for (int i = 0; i < 128; i++)
						{
							arrayfix = new[] { file[(i * 4) + 0x12], file[(i * 4) + 0x13], file[(i * 4) + 0x10], file[(i * 4) + 0x11] };
							FogTable.Add(ByteConverter.ToSingle(arrayfix, 0));
						}
					}
					else
					{
						for (int i = 0; i < 128; i++)
						{
							FogTable.Add(ByteConverter.ToSingle(file, (i * 4) + 0x10));
						}
					}
				}
				else
				{
					for (int i = 0; i < 128; i++)
					{
						FogTable.Add(ByteConverter.ToSingle(file, (i * 4) + 0x10));
					}
				}
			}
			else
			{
				int i = 0;
				do
				{
					FogTable.Add(0.0f);
					i++;
				}
				while (i < 128);
			}
		}
		public byte[] GetBytes(bool bigEndian, bool DC)
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes(Type));
			result.AddRange(ByteConverter.GetBytes(Unk));
			if (bigEndian)
			{
				result.Add(A);
				result.Add(R);
				result.Add(G);
				result.Add(B);
			}
			else
			{
				result.Add(B);
				result.Add(G);
				result.Add(R);
				result.Add(A);
			}
			result.AddRange(ByteConverter.GetBytes(MaxDist));
			result.AddRange(ByteConverter.GetBytes(MinDist));
			if (DC)
			{
				int diff = 128 - FogTable.Count;
				for (int i = 0; i < 128; i++)
					result.AddRange(ByteConverter.GetBytes(FogTable[i]));
				if (diff > 0)
				{
					result.AddRange(new byte[4 * diff]);
				}
			}
			return result.ToArray();
		}
	}
}
