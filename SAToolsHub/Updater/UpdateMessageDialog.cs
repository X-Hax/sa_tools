using System;
using System.Windows.Forms;

namespace ModManagerCommon.Forms
{
	public partial class UpdateMessageDialog : Form
	{
		string message;

		public UpdateMessageDialog(string name, string message)
		{
			InitializeComponent();
			this.Text = "SA Tools";
			this.message = message;
		}

		private void UpdateMessageDialog_Load(object sender, EventArgs e)
		{
			textBox1.Text = message;
		}
	}
}
