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
			this.ComboBox1.Items.Clear();
			for (int i = 0; i < ModelFileTypes.Length; i++)
			{
				this.ComboBox1.Items.Add(ModelFileTypes[i].name_or_type);
			}
		}

		private void CheckBox3_CheckedChanged(object sender, EventArgs e)
		{
			NumericUpDown1.Hexadecimal = CheckBox3.Checked;
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			numericUpDown2.Value = ModelFileTypes[ComboBox1.SelectedIndex].key;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDown3.Enabled = checkBox1.Checked;
		}
	}
}
