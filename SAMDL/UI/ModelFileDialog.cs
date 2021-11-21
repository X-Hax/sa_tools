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
				ComboBox_FileType.Items.Add(item);
		}

		List<string> KnownFileTypes = new List<string> { "EXE", "DLL", "REL", "1ST_READ.BIN", "SA1 Level", "SA2 Level", "SA1 Event", "SA2 Event", "SA2PC Event", "Model File" };

		public void CheckFilename(string filename)
		{
			// Defaults
			ComboBox_FileType.Enabled = CheckBox_BigEndian.Enabled = true;
			switch (CheckBinaryFile(filename).Key)
			{
				case BinaryModelFileType.SA2MDL:
					RadioButton_SA2MDL.Checked = true;
					break;
				case BinaryModelFileType.SA2BMDL:
					RadioButton_SA2BMDL.Checked = true;
					break;
				case BinaryModelFileType.EXE:
					ComboBox_Format.SelectedIndex = (filename.ToLowerInvariant().Contains("sonic2")) ? 3 : 1;
					CheckBox_BigEndian.Checked = false;
					ComboBox_FileType.SelectedIndex = 0;
					break;
				case BinaryModelFileType.DLL:
					ComboBox_Format.SelectedIndex = (filename.ToLowerInvariant().Contains("data")) ? 3 : 1;
					CheckBox_BigEndian.Checked = false;
					ComboBox_FileType.SelectedIndex = 1;
					break;
				case BinaryModelFileType.REL:
					ComboBox_Format.SelectedIndex = 0;
					ComboBox_FileType.SelectedIndex = 2;
					CheckBox_BigEndian.Checked = true;
					break;
				case BinaryModelFileType.DC_1STREAD:
					ComboBox_Format.SelectedIndex = 0;
					CheckBox_BigEndian.Checked = false;
					ComboBox_FileType.SelectedIndex = 3;
					break;
				case BinaryModelFileType.SA1Level:
					ComboBox_Format.SelectedIndex = 0;
					CheckBox_BigEndian.Checked = false;
					ComboBox_FileType.SelectedIndex = 4;
					break;
				case BinaryModelFileType.SA2Level:
					ComboBox_Format.SelectedIndex = 1;
					CheckBox_BigEndian.Checked = false;
					ComboBox_FileType.SelectedIndex = 5;
					break;
				case BinaryModelFileType.SA1Event:
					ComboBox_Format.SelectedIndex = 0;
					CheckBox_BigEndian.Checked = false;
					ComboBox_FileType.SelectedIndex = 6;
					break;
				case BinaryModelFileType.SA2Event:
					ComboBox_Format.SelectedIndex = 2;
					ComboBox_FileType.SelectedIndex = 7;
					break;
				case BinaryModelFileType.SA2PCEvent:
					ComboBox_Format.SelectedIndex = 3;
					ComboBox_FileType.SelectedIndex = 8;
					break;
				default:
					RadioButton_Binary.Checked = true;
					ComboBox_FileType.SelectedIndex = 9;
					ComboBox_Format.SelectedIndex = 0;
					break;
			}
		}

		private struct ModelFileType
		{
			public string ModelNameOrType;
			public UInt32 Key;
		};

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
			NumericUpDown_ObjectAddress.Hexadecimal = CheckBox_Hex_Object.Checked;
		}

		private void NumericUpDown_ObjectAddress_ValueChanged(object sender, EventArgs e)
		{
			if (NumericUpDown_ObjectAddress.Value < 0)
				NumericUpDown_ObjectAddress.Value = unchecked((uint)int.Parse(NumericUpDown_ObjectAddress.Value.ToString()));
		}

		private void NumericUpDown_MotionAddress_ValueChanged(object sender, EventArgs e)
		{
			if (NumericUpDown_MotionAddress.Value < 0)
				NumericUpDown_MotionAddress.Value = unchecked((uint)int.Parse(NumericUpDown_MotionAddress.Value.ToString()));
		}

		private void NumericUpDown_Key_ValueChanged(object sender, EventArgs e)
		{
			if (NumericUpDown_Key.Value < 0)
				NumericUpDown_Key.Value = unchecked((uint)int.Parse(NumericUpDown_Key.Value.ToString()));
		}

		private void ComboBox_FileType_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (ComboBox_FileType.SelectedIndex)
			{
				case 0: // EXE
					NumericUpDown_Key.Value = 0x400000;
					CheckBox_BigEndian.Checked = false;
					break;
				case 1: // DLL
					NumericUpDown_Key.Value = 0x10000000;
					CheckBox_BigEndian.Checked = false;
					break;
				case 2: // REL
					NumericUpDown_Key.Value = 0xC900000;
					CheckBox_BigEndian.Checked = true;
					break;
				case 3: // 1ST_READ.BIN
					NumericUpDown_Key.Value = 0x8C010000;
					CheckBox_BigEndian.Checked = false;
					break;
				case 4: // SA1 level
					NumericUpDown_Key.Value = 0xC900000;
					ComboBox_Format.SelectedIndex = 0;
					break;
				case 5: // SA2 level
					NumericUpDown_Key.Value = 0x8C500000;
					ComboBox_Format.SelectedIndex = 2;
					break;
				case 6: // SA1 event
					NumericUpDown_Key.Value = 0xCB80000;
					ComboBox_Format.SelectedIndex = 0;
					break;
				case 7: // SA2 event
					NumericUpDown_Key.Value = 0xC600000;
					ComboBox_Format.SelectedIndex = 2;
					break;
				case 8: // SA2PC event
					NumericUpDown_Key.Value = 0x8125FE60;
					CheckBox_BigEndian.Checked = true;
					ComboBox_Format.SelectedIndex = 3;
					break;
				case 9: // Unknown
					NumericUpDown_Key.Value = 0;
					break;
			}
		}

		private void CheckBox_LoadMotion_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown_MotionAddress.Enabled = CheckBox_LoadMotion.Checked;
			Label_MotionAddress.Enabled = CheckBox_LoadMotion.Checked;
			CheckBox_Hex_Motion.Enabled = CheckBox_LoadMotion.Checked;
			CheckBox_Memory_Motion.Enabled = CheckBox_LoadMotion.Checked;
		}

		private void RadioButton_Binary_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton_Object.Enabled = RadioButton_Binary.Checked;
			RadioButton_Attach.Enabled = RadioButton_Binary.Checked;
			RadioButton_Action.Enabled = RadioButton_Binary.Checked;
			ComboBox_FileType.Enabled = RadioButton_Binary.Checked;
			ComboBox_Format.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_ObjectAddress.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_Key.Enabled = RadioButton_Binary.Checked;
			CheckBox_BigEndian.Enabled = RadioButton_Binary.Checked;
			CheckBox_Hex_Object.Enabled = RadioButton_Binary.Checked;
			CheckBox_Memory_Object.Enabled = RadioButton_Binary.Checked;
			Label_Key.Enabled = RadioButton_Binary.Checked;
			Label_Key.Enabled = RadioButton_Binary.Checked;
			Label_ModelAddress.Enabled = RadioButton_Binary.Checked;
			Label_Format.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_MotionAddress.Enabled = false;
			Label_MotionAddress.Enabled = false;
			CheckBox_Hex_Motion.Enabled = false;
			CheckBox_Memory_Motion.Enabled = false;
			CheckBox_LoadMotion.Enabled = false;
			if (RadioButton_Object.Checked && RadioButton_Binary.Checked)
			{
				CheckBox_LoadMotion.Enabled = RadioButton_Object.Checked;
				NumericUpDown_MotionAddress.Enabled = CheckBox_LoadMotion.Checked;
				Label_MotionAddress.Enabled = CheckBox_LoadMotion.Checked;
				CheckBox_Hex_Motion.Enabled = CheckBox_LoadMotion.Checked;
				CheckBox_Memory_Motion.Enabled = CheckBox_LoadMotion.Checked;
			}
		}

		private void CheckBox_Hex_Motion_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown_MotionAddress.Hexadecimal = CheckBox_Hex_Motion.Checked;
		}

		private void RadioButton_Object_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox_LoadMotion.Enabled = RadioButton_Object.Checked;
			NumericUpDown_MotionAddress.Enabled = false;
			Label_MotionAddress.Enabled = false;
			CheckBox_Hex_Motion.Enabled = false;
			CheckBox_Memory_Motion.Enabled = false;
			if (RadioButton_Object.Checked)
			{
				NumericUpDown_MotionAddress.Enabled = CheckBox_LoadMotion.Checked;
				Label_MotionAddress.Enabled = CheckBox_LoadMotion.Checked;
				CheckBox_Hex_Motion.Enabled = CheckBox_LoadMotion.Checked;
				CheckBox_Memory_Motion.Enabled = CheckBox_LoadMotion.Checked;
			}
		}
	}
}
