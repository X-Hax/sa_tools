using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAMDL
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

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			NumericUpDown_Key.Value = ModelFileTypes[ComboBox_FileType.SelectedIndex].key;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown_MotionAddress.Enabled = CheckBox_LoadMotion.Checked;
		}

		private void typBinary_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox_Memory_Motion.Enabled = RadioButton_Binary.Checked;
			CheckBox_Hex_Motion.Enabled = RadioButton_Binary.Checked;
			RadioButton_Object.Enabled = RadioButton_Binary.Checked;
			RadioButton_Action.Enabled = RadioButton_Binary.Checked;
			ComboBox_FileType.Enabled = RadioButton_Binary.Checked;
			ComboBox_Format.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_ObjectAddress.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_Key.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_MotionAddress.Enabled = RadioButton_Binary.Checked;
			CheckBox_LoadMotion.Enabled = RadioButton_Binary.Checked;
			CheckBox_BigEndian.Enabled = RadioButton_Binary.Checked;
			CheckBox_Hex_Object.Enabled = RadioButton_Binary.Checked;
			CheckBox_Memory_Object.Enabled = RadioButton_Binary.Checked;
			Label_Key.Enabled = RadioButton_Binary.Checked;
			Label_Key.Enabled = RadioButton_Binary.Checked;
			Label_ModelAddress.Enabled = RadioButton_Binary.Checked;
			Label_MotionAddress.Enabled = RadioButton_Binary.Checked;
			Label_Format.Enabled = RadioButton_Binary.Checked;
		}

		private void RadioButton_SA2MDL_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox_Memory_Motion.Enabled = RadioButton_Binary.Checked;
			CheckBox_Hex_Motion.Enabled = RadioButton_Binary.Checked;
			Label_Structure.Enabled = RadioButton_Binary.Checked;
			RadioButton_Object.Enabled = RadioButton_Binary.Checked;
			RadioButton_Action.Enabled = RadioButton_Binary.Checked;
			ComboBox_FileType.Enabled = RadioButton_Binary.Checked;
			ComboBox_Format.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_ObjectAddress.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_Key.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_MotionAddress.Enabled = RadioButton_Binary.Checked;
			CheckBox_LoadMotion.Enabled = RadioButton_Binary.Checked;
			CheckBox_BigEndian.Enabled = RadioButton_Binary.Checked;
			CheckBox_Hex_Object.Enabled = RadioButton_Binary.Checked;
			CheckBox_Memory_Object.Enabled = RadioButton_Binary.Checked;
			Label_Key.Enabled = RadioButton_Binary.Checked;
			Label_ModelAddress.Enabled = RadioButton_Binary.Checked;
			Label_MotionAddress.Enabled = RadioButton_Binary.Checked;
			Label_Format.Enabled = RadioButton_Binary.Checked;
		}

		private void RadioButton_SA2BMDL_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox_Memory_Motion.Enabled = RadioButton_Binary.Checked;
			CheckBox_Hex_Motion.Enabled = RadioButton_Binary.Checked;
			RadioButton_Object.Enabled = RadioButton_Binary.Checked;
			RadioButton_Action.Enabled = RadioButton_Binary.Checked;
			ComboBox_FileType.Enabled = RadioButton_Binary.Checked;
			ComboBox_Format.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_ObjectAddress.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_Key.Enabled = RadioButton_Binary.Checked;
			NumericUpDown_MotionAddress.Enabled = RadioButton_Binary.Checked;
			CheckBox_LoadMotion.Enabled = RadioButton_Binary.Checked;
			CheckBox_BigEndian.Enabled = RadioButton_Binary.Checked;
			CheckBox_Hex_Object.Enabled = RadioButton_Binary.Checked;
			CheckBox_Memory_Object.Enabled = RadioButton_Binary.Checked;
			Label_Key.Enabled = RadioButton_Binary.Checked;
			Label_ModelAddress.Enabled = RadioButton_Binary.Checked;
			Label_MotionAddress.Enabled = RadioButton_Binary.Checked;
			Label_Format.Enabled = RadioButton_Binary.Checked;
		}

		private void CheckBox_Hex_Motion_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown_MotionAddress.Hexadecimal = CheckBox_Hex_Motion.Checked;
		}

		private void RadioButton_Object_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown_MotionAddress.Enabled = RadioButton_Object.Checked;
			Label_MotionAddress.Enabled = RadioButton_Object.Checked;
			CheckBox_Hex_Motion.Enabled = RadioButton_Object.Checked;
			CheckBox_Memory_Motion.Enabled = RadioButton_Object.Checked;
			CheckBox_LoadMotion.Enabled = RadioButton_Object.Checked;
		}

		private void RadioButton_Action_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown_MotionAddress.Enabled = RadioButton_Object.Checked;
			Label_MotionAddress.Enabled = RadioButton_Object.Checked;
			CheckBox_Hex_Motion.Enabled = RadioButton_Object.Checked;
			CheckBox_Memory_Motion.Enabled = RadioButton_Object.Checked;
			CheckBox_LoadMotion.Enabled = RadioButton_Object.Checked;
		}

	}
}
