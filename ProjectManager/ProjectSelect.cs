using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectManager
{
    public partial class ProjectSelect : Form
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

        public ProjectSelect()
        {
            InitializeComponent();
        }

        private void ProjectSelect_Shown(object sender, EventArgs e)
        {
            projectListBox.SelectedIndex = -1;
            OKButton.Enabled = false;

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

            projectListBox.Items.Clear();
            foreach (ProjectName projectName in projectNames)
            {
                projectListBox.Items.Add(string.Format("{0} ({1})", projectName.Name, projectName.Game.ToString()));
            }
        }

        private void projectListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(projectListBox.SelectedIndex >= 0)
            {
                OKButton.Enabled = true;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            ProjectName projectName = projectNames[projectListBox.SelectedIndex];

            string projectFolderPath = (projectName.Game == SA_Tools.Game.SADX) ?
                sadxDirectory.FullName : sa2Directory.FullName;


            projectFolderPath = Path.Combine(projectFolderPath, projectName.Name);

            this.Hide();
            ProjectSelected(projectName.Game, projectName.Name, projectFolderPath);
        }

        private void ProjectSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();

            SelectionCanceled.Invoke();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Hide();

            SelectionCanceled.Invoke();
        }
    } // end of class
} // end of namespace
