using System;
using System.Windows.Forms;

namespace SASave
{
	internal partial class SaveFormatDialog : Form
	{
		public SaveFormatDialog(Systems system)
		{
			InitializeComponent();
			switch (system)
			{
				case Systems.Dreamcast:
					systemDreamcast.Checked = true;
					break;
				case Systems.GameCube:
					systemGamecube.Checked = true;
					break;
				case Systems.PC:
					systemPC.Checked = true;
					break;
				case Systems.Steam:
					systemSteam.Checked = true;
					break;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void system_CheckedChanged(object sender, EventArgs e)
		{
			switch (SelectedSystem)
			{
				case Systems.Dreamcast:
					dcRegions.Visible = true;
					gcRegions.Visible = false;
					break;
				case Systems.GameCube:
					dcRegions.Visible = false;
					gcRegions.Visible = true;
					break;
				default:
					dcRegions.Visible = false;
					gcRegions.Visible = false;
					break;
			}
		}

		public Systems SelectedSystem
		{
			get
			{
				if (systemDreamcast.Checked)
					return Systems.Dreamcast;
				else if (systemGamecube.Checked)
					return Systems.GameCube;
				else if (systemGameCubePreview.Checked)
					return Systems.GameCubePreview;
				else if (system360.Checked)
					return Systems.XBox360;
				else if (systemSteam.Checked)
					return Systems.Steam;
				return Systems.PC;
			}
		}

		public Regions SelectedRegion
		{
			get
			{
				switch (SelectedSystem)
				{
					case Systems.Dreamcast:
						if (dcRegionJapan.Checked)
							return Regions.Japan;
						else if (dcRegionInternational.Checked)
							return Regions.International;
						return Regions.US;
					case Systems.GameCube:
						if (gcRegionJapan.Checked)
							return Regions.Japan;
						else if (gcRegionEurope.Checked)
							return Regions.Europe;
						return Regions.US;
					case Systems.GameCubePreview:
						return Regions.Japan;
					default:
						return Regions.US;
				}
			}
		}
	}
}