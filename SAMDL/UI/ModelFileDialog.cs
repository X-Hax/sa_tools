using System;
using System.Windows.Forms;

namespace SAModel.SAMDL
{
	public partial class ModelFileDialog : Form
	{
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

		public ModelFileDialog()
		{
			InitializeComponent();
		}

		private void Dialog1_Load(object sender, EventArgs e)
		{
			this.ComboBox_FileType.Items.Clear();
			for (int i = 0; i < ModelFileTypes.Length; i++)
			{
				this.ComboBox_FileType.Items.Add(ModelFileTypes[i].name_or_type);
			}
		}

		private void CheckBox3_CheckedChanged(object sender, EventArgs e)
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

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			NumericUpDown_Key.Value = ModelFileTypes[ComboBox_FileType.SelectedIndex].key;
		}

		private void CheckBox_LoadMotion_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown_MotionAddress.Enabled = CheckBox_LoadMotion.Checked;
			Label_MotionAddress.Enabled = CheckBox_LoadMotion.Checked;
			CheckBox_Hex_Motion.Enabled = CheckBox_LoadMotion.Checked;
			CheckBox_Memory_Motion.Enabled = CheckBox_LoadMotion.Checked;
		}

		private void typBinary_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton_Object.Enabled = RadioButton_Binary.Checked;
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
