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
    public partial class ProjectSelect : Form
    {
        private GameConfig gameConfig;

        public ProjectSelect()
        {
            InitializeComponent();

            gameConfig = new GameConfig();
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            gameConfig.ShowDialog();
        }
    }
}
