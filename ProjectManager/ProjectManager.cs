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
        private ProjectSelect projectSelect;
        private ManualSplit manualSplit;

        public ProjectManager()
        {
            InitializeComponent();

            gameConfig = new GameConfig();
            newProject = new NewProject();
            newProject.ProjectCreated += NewProject_ProjectCreated;
            newProject.CreationCanceled += () => { this.Show(); };

            projectActions = new ProjectActions();
            projectActions.NavigateBack += () => { this.Show(); };

            projectSelect = new ProjectSelect();
            projectSelect.ProjectSelected += ProjectSelect_ProjectSelceted;
            projectSelect.SelectionCanceled += () => { this.Show(); };

            manualSplit = new ManualSplit();
            manualSplit.OnSplitFinished += () => { this.Show(); };
            manualSplit.OnSplitCanceled += () => { this.Show(); };

            // tooltips
            ToolTip newProjectTooltip = new ToolTip();
            newProjectTooltip.SetToolTip(NewProjectButton, "Create a new project. This will create a project folder, and extract data from the game for modification.");

            ToolTip openProjectTooltip = new ToolTip();
            openProjectTooltip.SetToolTip(OpenProjectButton, "Open an existing project. From here you can launch editor tools, build the mod, and launch the game");

            ToolTip configTooltip = new ToolTip();
            configTooltip.SetToolTip(ConfigButton, "Configure Project Manager. This primarily means supplying the paths to SADXPC and SA2");

            ToolTip splitTooltip = new ToolTip();
            splitTooltip.SetToolTip(SplitToolsButton, "Manual split tools. If you're just looking to rip data and not create or modify a mod, this is what you want.");
        }

        private void ProjectSelect_ProjectSelceted(SA_Tools.Game game, string projectName, string fullProjectPath)
        {
            this.Hide();
            projectActions.Init(game, projectName, fullProjectPath);
            projectActions.Show();
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

        private void OpenProjectButton_Click(object sender, EventArgs e)
        {
            Hide();
            projectSelect.Show();
        }

        private void SplitToolsButton_Click(object sender, EventArgs e)
        {
            Hide();
            manualSplit.Show();
        }
    }
}
