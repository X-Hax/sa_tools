using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXLVL2
{
	public partial class ProjectSelectDialog : Form
	{
        List<string> projects = new List<string>();

        private string selectedProject = "";

        public string SelectedProject { get { return selectedProject; } }

        public void LoadProjectList(string baseGamePath)
        {
            DirectoryInfo basePathInfo = new DirectoryInfo(Path.Combine(baseGamePath, "Projects"));

            DirectoryInfo[] projectInfoList = basePathInfo.GetDirectories();

            foreach(DirectoryInfo projectInfo in projectInfoList)
            {
                // see if sadxlv.ini exists before adding to list
                if(File.Exists(Path.Combine(projectInfo.FullName, "sadxlvl.ini")))
                {
                    projects.Add(projectInfo.Name);
                }
            }
        }

        public ProjectSelectDialog()
        {
            InitializeComponent();
        }

		private void listRecentFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listProjects.SelectedIndices.Count > 0)
			{
				buttonGo.Enabled = true;
				//SelectedItem = listRecentFiles.SelectedItem.ToString();
			}
		}

		private void listRecentFiles_DoubleClick(object sender, EventArgs e)
		{
			if (listProjects.SelectedIndices.Count > 0)
				buttonGo.PerformClick();
		}

        private void buttonGo_Click(object sender, EventArgs e)
        {
            selectedProject = listProjects.Items[listProjects.SelectedIndex].ToString();
            Hide();
        }

        private void ProjectSelectDialog_Shown(object sender, EventArgs e)
        {
            foreach(string project in projects)
            {
                listProjects.Items.Add(project);
            }
        }
    }
}
