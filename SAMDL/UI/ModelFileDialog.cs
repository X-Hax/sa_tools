using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SAModel.SAMDL
{
	public partial class ModelFileDialog : Form
	{
		public ModelFileDialog()
		{
			InitializeComponent();
			foreach (string item in KnownFileTypes)
				comboBoxBinaryFileType.Items.Add(item);
		}

		List<string> KnownFileTypes = new List<string> { "EXE", "DLL", "REL", "1ST_READ.BIN", "SA1 Level", "SA2 Level", "SA1 Event", "SA2 Event", "SA2PC Event", "EXE (X360)", "Model File" };

		public void CheckFilename(string filename)
		{
			// Defaults
			comboBoxBinaryFileType.Enabled = checkBoxBigEndian.Enabled = true;
			switch (CheckBinaryFile(filename).Key)
			{
				case BinaryModelFileType.SA2MDL:
					radioButtonSA2MDL.Checked = true;
					break;
				case BinaryModelFileType.SA2BMDL:
					radioButtonSA2BMDL.Checked = true;
					break;
				case BinaryModelFileType.EXE:
					comboBoxModelFormat.SelectedIndex = (filename.ToLowerInvariant().Contains("sonic2")) ? 3 : 1;
					checkBoxBigEndian.Checked = false;
					comboBoxBinaryFileType.SelectedIndex = 0;
					break;
				case BinaryModelFileType.DLL:
					comboBoxModelFormat.SelectedIndex = (filename.ToLowerInvariant().Contains("data")) ? 3 : 1;
					checkBoxBigEndian.Checked = false;
					comboBoxBinaryFileType.SelectedIndex = 1;
					break;
				case BinaryModelFileType.REL:
					comboBoxModelFormat.SelectedIndex = 0;
					comboBoxBinaryFileType.SelectedIndex = 2;
					checkBoxBigEndian.Checked = true;
					break;
				case BinaryModelFileType.DC_1STREAD:
					comboBoxModelFormat.SelectedIndex = 0;
					checkBoxBigEndian.Checked = false;
					comboBoxBinaryFileType.SelectedIndex = 3;
					break;
				case BinaryModelFileType.SA1Level:
					comboBoxModelFormat.SelectedIndex = 0;
					checkBoxBigEndian.Checked = false;
					comboBoxBinaryFileType.SelectedIndex = 4;
					break;
				case BinaryModelFileType.SA2Level:
					comboBoxModelFormat.SelectedIndex = 1;
					checkBoxBigEndian.Checked = false;
					comboBoxBinaryFileType.SelectedIndex = 5;
					break;
				case BinaryModelFileType.SA1Event:
					comboBoxModelFormat.SelectedIndex = 0;
					checkBoxBigEndian.Checked = false;
					comboBoxBinaryFileType.SelectedIndex = 6;
					break;
				case BinaryModelFileType.SA2Event:
					comboBoxModelFormat.SelectedIndex = 2;
					comboBoxBinaryFileType.SelectedIndex = 7;
					break;
				case BinaryModelFileType.SA2PCEvent:
					comboBoxModelFormat.SelectedIndex = 3;
					comboBoxBinaryFileType.SelectedIndex = 8;
					break;
				default:
					radioButtonBinary.Checked = true;
					comboBoxBinaryFileType.SelectedIndex = 10;
					comboBoxModelFormat.SelectedIndex = 0;
					break;
			}
		}

		private enum BinaryModelFileType
		{
			EXE,
			DLL,
			REL,
			DC_1STREAD,
			SA1Level,
			SA2Level,
			SA1Event,
			SA2Event,
			SA2PCEvent,
			SA2MDL,
			SA2BMDL,
			OtherKnown,
			Unknown
		};

		private static KeyValuePair<BinaryModelFileType, uint> CheckBinaryFile(string path)
		{
			// Events
			if (path.Length > 3 && path.Substring(0, 3).Equals("EV0", StringComparison.OrdinalIgnoreCase))
				return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.SA1Event, 0xCB80000);
			else if (path.Length > 2 && path.Substring(0, 2).Equals("E0", StringComparison.OrdinalIgnoreCase))
				return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.SA2Event, 0xC600000);
			// Common extensions
			switch (Path.GetExtension(path).ToLowerInvariant())
			{
				case ".exe":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.EXE, 0x400000);
				case ".dll":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.DLL, 0x1000000);
				case ".rel":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.REL, 0xC900000);
			}
			// Known filenames
			switch (Path.GetFileNameWithoutExtension(path).ToUpperInvariant())
			{
				case "1ST_READ":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.DC_1STREAD, 0x8C010000);
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
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.SA1Level, 0xC900000);
				case "ADV0100":
				case "ADV01OBJ":
				case "ADV0130":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.OtherKnown, 0xC920000);
				case "A_MOT":
				case "B_MOT":
				case "E_MOT":
				case "S_MOT":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.OtherKnown, 0xCC00000);
				case "S_SBMOT":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.OtherKnown, 0xCB08000);
				case "M_SBMOT":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.OtherKnown, 0xCADC000);
				case "AL_GARDEN00":
				case "AL_GARDEN01":
				case "AL_GARDEN02":
				case "AL_RACE":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.OtherKnown, 0x0CB80000);
				case "TIKAL_PROG":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.OtherKnown, 0xCB00000);
				case "ADVERTISE":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.OtherKnown, 0x8C900000);
				case "MOVIE":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.OtherKnown, 0x8CEB0000);
				case "DWALKMDL":
				case "EWALK2MDL":
				case "SHADOW1MDL":
				case "SONIC1MDL":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.SA2BMDL, 0);
				case "BKNUCKMDL":
				case "BROUGEMDL":
				case "BWALKMDL":
				case "CHAOS0MDL":
				case "CWALKMDL":
				case "EGGMDL":
				case "EWALK1MDL":
				case "EWALKMDL":
				case "HEWALKMDL":
				case "HKNUCKMDL":
				case "HROUGEMDL":
				case "HSHADOWMDL":
				case "HSONICMDL":
				case "HTWALKMDL":
				case "KNUCKMDL":
				case "METALSONICMDL":
				case "MILESMDL":
				case "PSOSHADOWMDL":
				case "PSOSONICMDL":
				case "ROUGEMDL":
				case "SONICMDL":
				case "SSHADOWMDL":
				case "SSONICMDL":
				case "TERIOSMDL":
				case "TICALMDL":
				case "TWALK1MDL":
				case "TWALKMDL":
				case "XEWALKMDL":
				case "XKNUCKMDL":
				case "XROUGEMDL":
				case "XSHADOWMDL":
				case "XSONICMDL":
				case "XTWALKMDL":
					return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.SA2MDL, 0);
			}
			return new KeyValuePair<BinaryModelFileType, uint>(BinaryModelFileType.Unknown, 0);
		}

		private void CheckBox_Hex_Object_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownModelAddress.Hexadecimal = checkBoxHexObject.Checked;
		}

		private void CheckBox_Hex_Motion_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownMotionAddress.Hexadecimal = checkBoxHexMotion.Checked;
		}

		private void checkBoxHexStartOffset_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownStartOffset.Hexadecimal = checkBoxHexStartOffset.Checked;
		}

		private void numericUpDownMotionAddress_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownMotionAddress.Value < 0)
				numericUpDownMotionAddress.Value = unchecked((uint)int.Parse(numericUpDownMotionAddress.Value.ToString()));
		}

		private void numericUpDownStartOffset_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownStartOffset.Value < 0)
				numericUpDownStartOffset.Value = unchecked((uint)int.Parse(numericUpDownStartOffset.Value.ToString()));
		}

		private void numericUpDownKey_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownKey.Value < 0)
				numericUpDownKey.Value = unchecked((uint)int.Parse(numericUpDownKey.Value.ToString()));
		}

		private void numericUpDownModelAddress_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownModelAddress.Value < 0)
				numericUpDownModelAddress.Value = unchecked((uint)int.Parse(numericUpDownModelAddress.Value.ToString()));
		}

		private void ComboBox_FileType_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (comboBoxBinaryFileType.SelectedIndex)
			{
				case 0: // EXE
					numericUpDownKey.Value = 0x400000;
					checkBoxBigEndian.Checked = false;
					break;
				case 1: // DLL
					numericUpDownKey.Value = 0x10000000;
					checkBoxBigEndian.Checked = false;
					break;
				case 2: // REL
					numericUpDownKey.Value = 0xC900000;
					checkBoxBigEndian.Checked = true;
					break;
				case 3: // 1ST_READ.BIN
					numericUpDownKey.Value = 0x8C010000;
					checkBoxBigEndian.Checked = false;
					break;
				case 4: // SA1 level
					numericUpDownKey.Value = 0xC900000;
					comboBoxModelFormat.SelectedIndex = 0;
					break;
				case 5: // SA2 level
					numericUpDownKey.Value = 0x8C500000;
					comboBoxModelFormat.SelectedIndex = 2;
					break;
				case 6: // SA1 event
					numericUpDownKey.Value = 0xCB80000;
					comboBoxModelFormat.SelectedIndex = 0;
					break;
				case 7: // SA2 event
					numericUpDownKey.Value = 0xC600000;
					comboBoxModelFormat.SelectedIndex = 2;
					break;
				case 8: // SA2PC event
					numericUpDownKey.Value = 0x8125FE60;
					checkBoxBigEndian.Checked = true;
					comboBoxModelFormat.SelectedIndex = 3;
					break;
				case 9: // X360 EXE
					comboBoxModelFormat.SelectedIndex = 1;
					comboBoxBinaryFileType.SelectedIndex = 9;
					checkBoxBigEndian.Checked = true;
					numericUpDownStartOffset.Value = 0xC800;
					break;
				case 10: // Unknown
					numericUpDownKey.Value = 0;
					break;
			}
		}

		private void CheckBox_LoadMotion_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownMotionAddress.Enabled = checkBoxLoadMotion.Checked;
			labelMotionAddress.Enabled = checkBoxLoadMotion.Checked;
			checkBoxHexMotion.Enabled = checkBoxLoadMotion.Checked;
			checkBoxMemoryMotion.Enabled = checkBoxLoadMotion.Checked;
		}

		private void RadioButton_Binary_CheckedChanged(object sender, EventArgs e)
		{
			checkBoxBigEndian.Enabled = labelStructure.Enabled = radioButtonObject.Enabled = radioButtonBinary.Checked;
			radioButtonAttach.Enabled = radioButtonBinary.Checked;
			radioButtonAction.Enabled = radioButtonBinary.Checked;
			comboBoxBinaryFileType.Enabled = radioButtonBinary.Checked;
			comboBoxModelFormat.Enabled = radioButtonBinary.Checked;
			numericUpDownModelAddress.Enabled = radioButtonBinary.Checked;
			numericUpDownKey.Enabled = radioButtonBinary.Checked;
			checkBoxBigEndian.Enabled = radioButtonBinary.Checked;
			checkBoxReverse.Enabled = radioButtonBinary.Checked;
			checkBoxHexObject.Enabled = radioButtonBinary.Checked;
			checkBoxMemoryObject.Enabled = radioButtonBinary.Checked;
			labelKey.Enabled = radioButtonBinary.Checked;
			labelKey.Enabled = radioButtonBinary.Checked;
			labelModelAddress.Enabled = radioButtonBinary.Checked;
			labelModelFormat.Enabled = radioButtonBinary.Checked;
			numericUpDownMotionAddress.Enabled = false;
			labelMotionAddress.Enabled = false;
			checkBoxHexMotion.Enabled = false;
			checkBoxMemoryMotion.Enabled = false;
			checkBoxLoadMotion.Enabled = false;
			labelStartOffset.Enabled = numericUpDownStartOffset.Enabled = checkBoxHexStartOffset.Enabled = radioButtonBinary.Checked;
			if (radioButtonObject.Checked && radioButtonBinary.Checked)
			{
				checkBoxLoadMotion.Enabled = radioButtonObject.Checked;
				numericUpDownMotionAddress.Enabled = checkBoxLoadMotion.Checked;
				labelMotionAddress.Enabled = checkBoxLoadMotion.Checked;
				checkBoxHexMotion.Enabled = checkBoxLoadMotion.Checked;
				checkBoxMemoryMotion.Enabled = checkBoxLoadMotion.Checked;
			}
		}

		private void RadioButton_Object_CheckedChanged(object sender, EventArgs e)
		{
			checkBoxLoadMotion.Enabled = radioButtonObject.Checked;
			numericUpDownMotionAddress.Enabled = false;
			labelMotionAddress.Enabled = false;
			checkBoxHexMotion.Enabled = false;
			checkBoxMemoryMotion.Enabled = false;
			if (radioButtonObject.Checked)
			{
				numericUpDownMotionAddress.Enabled = checkBoxLoadMotion.Checked;
				labelMotionAddress.Enabled = checkBoxLoadMotion.Checked;
				checkBoxHexMotion.Enabled = checkBoxLoadMotion.Checked;
				checkBoxMemoryMotion.Enabled = checkBoxLoadMotion.Checked;
			}
		}

		private void ModelFileDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/wiki/Working-with-Binary-Files") { CreateNoWindow = true });
		}
	}
}
