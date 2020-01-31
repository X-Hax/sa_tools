using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public partial class WelcomeForm : Form
	{
		public WelcomeForm()
		{
			InitializeComponent();
		}

		public void GoToSite(string url)
		{
			System.Diagnostics.Process.Start(url);
		}

		private void WikiDocLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			GoToSite("https://github.com/sonicretro/sa_tools/blob/master/README.md");
		}

		private void discordLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			GoToSite("https://discordapp.com/invite/wsNdZ4d");
		}

		private void doneButton_Click(object sender, EventArgs e)
		{
			if(!showOnStartCheckbox.Checked) MessageBox.Show("You can view this dialog at any time by going to the 'Help->Welcome / Tutorial' menu option, or by pressing F1");
			this.Close();
		}
	}
}
