using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAFontEdit
{
	public partial class ExportWidthDialog : Form
	{
		public int SpaceWidth;
		public int EmptyWidth;

		public ExportWidthDialog()
		{
			InitializeComponent();
		}

		private void buttonExport_Click(object sender, EventArgs e)
		{
			SpaceWidth = (int)numericUpDownSpaceWidth.Value;
			EmptyWidth = (int)numericUpDownEmptyWidth.Value;
		}
	}
}
