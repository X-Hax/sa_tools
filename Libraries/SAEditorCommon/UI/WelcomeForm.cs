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
			GoToSite("https://discord.gg/7SeUadNC9d");
		}

		private void doneButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void controlsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			GoToSite("https://github.com/sonicretro/sa_tools/wiki/3D-Editor-Controls");
		}
	}
}
