using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectManager
{
    public partial class ProjectManager : Form
    {
        private GameConfig gameConfig;
        private NewProject newProject;
        private ProjectActions projectActions;

        public ProjectManager()
        {
            InitializeComponent();

            gameConfig = new GameConfig();
            newProject = new NewProject();
            newProject.ProjectCreated += NewProject_ProjectCreated;
            newProject.CreationCanceled += () => { this.Show(); };

            projectActions = new ProjectActions();
            projectActions.NavigateBack += () => { this.Show(); };
        }

        private void NewProject_ProjectCreated(SA_Tools.Game game, string projectName, string fullProjectPath)
        {
            MessageBox.Show("Project creation complete!");
            projectActions.Init(game, projectName, fullProjectPath);
            projectActions.Show();
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            gameConfig.ShowDialog();
        }

        private void NewProjectButton_Click(object sender, EventArgs e)
        {
            Hide();
            newProject.Show();
        }
    }
}
