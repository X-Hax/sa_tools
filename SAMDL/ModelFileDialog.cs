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
			NumericUpDown_ModelAddress.Hexadecimal = CheckBox_Hex.Checked;
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			NumericUpDown_Key.Value = ModelFileTypes[ComboBox_FileType.SelectedIndex].key;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown_AnimationAddress.Enabled = CheckBox_LoadAnimation.Checked;
		}

		private void typBinary_CheckedChanged(object sender, EventArgs e)
		{
			ComboBox_FileType.Enabled = true;
			ComboBox_Format.Enabled = true;
			NumericUpDown_ModelAddress.Enabled = true;
			NumericUpDown_Key.Enabled = true;
			NumericUpDown_AnimationAddress.Enabled = true;
			CheckBox_LoadAnimation.Enabled = true;
			CheckBox_BigEndian.Enabled = true;
			CheckBox_Hex.Enabled = true;
			CheckBox_Memory.Enabled = true;
			Label_Key.Enabled = true;
			Label_Key.Enabled = true;
			Label_ModelAddress.Enabled = true;
			Label_AnimationAddress.Enabled = true;
			Label_Format.Enabled = true;
		}

		private void typSA2MDL_CheckedChanged(object sender, EventArgs e)
		{
			ComboBox_FileType.Enabled = false;
			ComboBox_Format.Enabled = false;
			NumericUpDown_ModelAddress.Enabled = false;
			NumericUpDown_Key.Enabled = false;
			NumericUpDown_AnimationAddress.Enabled = false;
			CheckBox_LoadAnimation.Enabled = false;
			CheckBox_BigEndian.Enabled = false;
			CheckBox_Hex.Enabled = false;
			CheckBox_Memory.Enabled = false;
			Label_Key.Enabled = false;
			Label_ModelAddress.Enabled = false;
			Label_AnimationAddress.Enabled = false;
			Label_Format.Enabled = false;
		}

		private void typSA2BMDL_CheckedChanged(object sender, EventArgs e)
		{
			ComboBox_FileType.Enabled = false;
			ComboBox_Format.Enabled = false;
			NumericUpDown_ModelAddress.Enabled = false;
			NumericUpDown_Key.Enabled = false;
			NumericUpDown_AnimationAddress.Enabled = false;
			CheckBox_LoadAnimation.Enabled = false;
			CheckBox_BigEndian.Enabled = false;
			CheckBox_Hex.Enabled = false;
			CheckBox_Memory.Enabled = false;
			Label_Key.Enabled = false;
			Label_ModelAddress.Enabled = false;
			Label_AnimationAddress.Enabled = false;
			Label_Format.Enabled = false;
		}
	}
}
