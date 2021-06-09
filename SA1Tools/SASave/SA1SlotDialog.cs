using System;
using System.Windows.Forms;

namespace SASave
{
	public partial class SA1SlotDialog : Form
	{
		public SA1SlotDialog()
		{
			InitializeComponent();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		public int Selection
		{
			get
			{
				if (radioButton1.Checked)
					return 0;
				else if (radioButton2.Checked)
					return 1;
				else
					return 2;
			}
		}
	}
}