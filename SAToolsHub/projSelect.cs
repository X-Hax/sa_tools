using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAToolsHub
{
	public partial class projSelect : Form
	{
		public delegate void ProjectSelectionHandler(SA_Tools.Game game, string projectName, string fullProjectPath);
		public event ProjectSelectionHandler ProjectSelected;

		public Action SelectionCanceled;

		DirectoryInfo sadxDirectory;
		DirectoryInfo sa2Directory;

		private struct ProjectName
		{
			public SA_Tools.Game Game;
			public string Name;
		}

		List<ProjectName> projectNames = new List<ProjectName>();

		public projSelect()
		{
			InitializeComponent();
		}

		private void projSelect_Shown(object senter, EventArgs e)
		{
			btnOpen.Enabled = false;
			lstProjects.FullRowSelect = true;
			projectNames.Clear();

			sadxDirectory = new DirectoryInfo(Program.Settings.SADXPCPath + "\\Projects\\");
			if (sadxDirectory.Exists)
			{
				// add all of our sadxpc entries

				DirectoryInfo[] projectDirs = sadxDirectory.GetDirectories();

				foreach (DirectoryInfo projectFolder in projectDirs)
				{
					projectNames.Add(new ProjectName() { Name = projectFolder.Name, Game = SA_Tools.Game.SADX });
				}
			}

			sa2Directory = new DirectoryInfo(Program.Settings.SA2PCPath + "\\Projects\\");
			if (sa2Directory.Exists)
			{
				// add all of our sa2pc entries
				DirectoryInfo[] projectDirs = sa2Directory.GetDirectories();

				foreach (DirectoryInfo projectFolder in projectDirs)
				{
					projectNames.Add(new ProjectName() { Name = projectFolder.Name, Game = SA_Tools.Game.SA2B });
				}
			}

			lstProjects.Items.Clear();
			foreach (ProjectName projectName in projectNames)
			{
				ListViewItem row = new ListViewItem(projectName.Name);
				row.SubItems.Add(projectName.Game.ToString());
				lstProjects.Items.Add(row);
			}
		}

		private void btnOpen_Click(object sender, EventArgs e)
		{
			int projSel = lstProjects.FocusedItem.Index;

			ProjectName projectName = projectNames[projSel];

			string projectFolderPath = (projectName.Game == SA_Tools.Game.SADX) ?
				sadxDirectory.FullName : sa2Directory.FullName;


			projectFolderPath = Path.Combine(projectFolderPath, projectName.Name);

			ProjectSelected(projectName.Game, projectName.Name, projectFolderPath);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void lstProjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstProjects.SelectedItems.Count > 0)
			{
				btnOpen.Enabled = true;
			}
		}
	}
}
