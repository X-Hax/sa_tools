using SonicRetro.SAModel.SALVL.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SALVL
{
	public partial class PathWarning : Form
	{
		private readonly MainForm parent;

		public string SelectedItem { get; private set; }

		public List<string> RemovedItems { get; private set; }

		private void listRecentFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listRecentFiles.SelectedIndices.Count > 0)
			{
				buttonGo.Enabled = true;
				buttonRemove.Enabled = true;
				SelectedItem = listRecentFiles.SelectedItem.ToString();
			}
			else
			{
				buttonGo.Enabled = false;
				buttonRemove.Enabled = false;
			}
		}

		private void listRecentFiles_DoubleClick(object sender, EventArgs e)
		{
			if (listRecentFiles.SelectedIndices.Count > 0)
				buttonGo.PerformClick();
		}

		private void buttonOpen_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog()
			{
				DefaultExt = "ini",
				Filter = "sadxlvl.ini|sadxlvl.ini|All files (*.*)|*.*",
				Multiselect = false
			})
			{
				DialogResult result = fileDialog.ShowDialog();

				if (result == DialogResult.OK)
				{
					listRecentFiles.Items.Add(fileDialog.FileName);
				}
			}
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			if (listRecentFiles.SelectedIndices.Count > 0)
			{
				RemovedItems.Add(listRecentFiles.SelectedItem.ToString());
				listRecentFiles.Items.RemoveAt(listRecentFiles.SelectedIndices[0]);
				listRecentFiles.ClearSelected();
			}
		}
	}
}
