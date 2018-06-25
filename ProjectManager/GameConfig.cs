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
    public partial class GameConfig : Form
    {
        public GameConfig()
        {
            InitializeComponent();

            // try loading our settings
            SetValues();
        }

        void SetValues()
        {
            string sadxPCPath = Program.Settings.SADXPCPath;
            string sa2PCPath = Program.Settings.SA2PCPath;

            SADXPath.Text = sadxPCPath;
            SA2PCPath.Text = sa2PCPath;
        }

        void SaveSettings()
        {
            Program.Settings.SADXPCPath = SADXPath.Text;
            Program.Settings.SA2PCPath = SA2PCPath.Text;

            Program.Settings.Save();
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            SaveSettings();

            // check validity
            string sadxFailReason = "";
            bool sadxpcValid = Program.CheckSADXPCValid(out sadxFailReason);

            if(!sadxpcValid)
            {
                MessageBox.Show(sadxFailReason, "SADX PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            string sa2PCFailReason = "";
            bool sa2PCValid = Program.CheckSA2PCValid(out sa2PCFailReason);

            if(!sa2PCValid)
            {
                MessageBox.Show(sa2PCFailReason, "SA2 PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if(!Program.AnyGamesConfigured())
            {
                DialogResult dialogResult = MessageBox.Show("No games configured", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (dialogResult == DialogResult.Cancel)
                {
                    this.DialogResult = DialogResult.Abort;
                    Application.Exit();
                    return;
                }
                else
                {
                    return;
                }
            }

            // hide and save
            this.Hide();
        }

        private void GameConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.Abort) return;

            SaveSettings();

            // check validitiy

            // check validity
            string sadxFailReason = "";
            bool sadxpcValid = Program.CheckSADXPCValid(out sadxFailReason);

            if (!sadxpcValid)
            {
                MessageBox.Show(sadxFailReason, "SADX PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            string sa2PCFailReason = "";
            bool sa2PCValid = Program.CheckSA2PCValid(out sa2PCFailReason);

            if (!sa2PCValid)
            {
                MessageBox.Show(sa2PCFailReason, "SA2 PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!Program.AnyGamesConfigured())
            {
                DialogResult dialogResult = MessageBox.Show("No games configured", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (dialogResult == DialogResult.Cancel)
                {
                    this.DialogResult = DialogResult.Abort;
                    e.Cancel = false;
                    Application.Exit();
                    return;
                }
                else
                {
                    return;
                }
            }

            e.Cancel = true;
            this.Hide();
        }

        private void SADXBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult browserResult = folderBrowserDialog1.ShowDialog();

            if(browserResult == DialogResult.OK)
            {
                SADXPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void SA2PCBrowse_Click(object sender, EventArgs e)
        {
            DialogResult browserResult = folderBrowserDialog1.ShowDialog();

            if (browserResult == DialogResult.OK)
            {
                SA2PCPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void GameConfig_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                SetValues();
            }
        }
    }
}
