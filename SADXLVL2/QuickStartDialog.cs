using System;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXLVL2
{
	public partial class QuickStartDialog : Form
	{
		private readonly MainForm parent;

		public string SelectedItem { get; private set; }

		public QuickStartDialog(MainForm parent, StringCollection recentList)
		{
			InitializeComponent();

			this.parent = parent;
			foreach (string s in recentList)
				listRecentFiles.Items.Add(s);
		}

		private void listRecentFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listRecentFiles.SelectedIndices.Count > 0)
			{
				buttonGo.Enabled = true;
				SelectedItem = listRecentFiles.SelectedItem.ToString();
			}
		}

		private void listRecentFiles_DoubleClick(object sender, EventArgs e)
		{
			if (listRecentFiles.SelectedIndices.Count > 0)
				buttonGo.PerformClick();
		}

		private void buttonOpen_Click(object sender, EventArgs e)
		{
			if (parent.OpenFile())
				Close();
		}
	}
}
