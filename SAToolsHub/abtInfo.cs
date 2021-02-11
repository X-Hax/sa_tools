using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SAToolsHub
{
	public partial class formAbout : Form
	{
		public formAbout()
		{ 
			InitializeComponent();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start("https://github.com/ookii-dialogs/ookii-dialogs-wpf/blob/master/LICENSE");
			}
			catch
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start("https://github.com/sonicretro/sa_tools/graphs/contributors");
			}
			catch
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}
	}
}
