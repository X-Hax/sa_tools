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
    public partial class ProjectActions : Form
    {
        public Action NavigateBack;

        SA_Tools.Game game;
        string projectFolder;
        string projectName;

        public ProjectActions()
        {
            InitializeComponent();
        }

        private void ProjectActions_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            NavigateBack.Invoke();
        }

        public void Init(SA_Tools.Game game, string projectName, string projectFolder)
        {
            this.game = game;
            this.projectName = projectName;
            ProjectNameLAbel.Text = projectName + " : " +  game.ToString();
            this.projectFolder = projectFolder;
        }
    }
}
