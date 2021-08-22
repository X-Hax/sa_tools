using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SAModel.SAEditorCommon;

namespace PLTool
{
	public partial class SLEditor : Form
	{
        SLFile slfile;
        bool isGamecube;
        string currentFile;

        public SLEditor(string filepath = "", bool isGC=false)
        {
            InitializeComponent();
            isGamecube = isGC;
            if (filepath != "" && File.Exists(filepath))
                LoadSLFile(filepath);
            else
            {
                slfile = new SLFile();
                UpdateNumerics();
                UpdateLabels();
            }
        }

        private void LoadSLFile(string filename)
        {
            slfile = new SLFile(File.ReadAllBytes(filename), isGamecube);
            UpdateNumerics();
            UpdateLabels();
            toolStripStatusLabelFilename.Text = Path.GetFileNameWithoutExtension(filename);
            toolStripStatusLabelLevelName.Text = LanternFilenames.GetLevelNameFromFilename(Path.GetFileNameWithoutExtension(filename));
            currentFile = Path.GetFullPath(filename);
            saveToolStripMenuItem.Enabled = true;
            this.Text = "SL Editor - " + currentFile;
        }

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using (OpenFileDialog ofd = new OpenFileDialog() { Title = "Open SL File", Filter = "SL Files|SL*.BIN|All Files|*.*" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadSLFile(ofd.FileName);
                }
            }
        }

        private void UpdateData()
        {
            slfile.RotationY = (int)numericUpDownAngleY.Value;
            slfile.RotationZ = (int)numericUpDownAngleZ.Value;
            slfile.EnvRGB = Color.FromArgb((int)numericUpDownEnvR.Value, (int)numericUpDownEnvG.Value, (int)numericUpDownEnvB.Value);
            slfile.EnvAmbientMultiplier = (float)trackBarAMB.Value / 100.0f;
            slfile.EnvDiffuseMultiplier = (float)trackBarDIF.Value / 100.0f;
            slfile.EnvSpecularMultiplier = (float)trackBarSPC.Value / 100.0f;
            slfile.FreeSlaveRT = (int)numericUpDownRT.Value;
            slfile.FreeSlaveRD = (float)trackBarRD.Value / 100.0f;
            slfile.FreeSlaveRGB = Color.FromArgb((int)numericUpDownFreeR.Value, (int)numericUpDownFreeG.Value, (int)numericUpDownFreeB.Value);          
        }

        private void UpdateNumerics()
        {
            numericUpDownEnvR.Value = slfile.EnvRGB.R;
            numericUpDownEnvG.Value = slfile.EnvRGB.G;
            numericUpDownEnvB.Value = slfile.EnvRGB.B;
            numericUpDownAngleY.Value = slfile.RotationY;
            numericUpDownAngleZ.Value = slfile.RotationZ;
            numericUpDownFreeR.Value = slfile.FreeSlaveRGB.R;
            numericUpDownFreeG.Value = slfile.FreeSlaveRGB.G;
            numericUpDownFreeB.Value = slfile.FreeSlaveRGB.B;
            trackBarAMB.Value = (int)(Math.Round(slfile.EnvAmbientMultiplier, 5) * 100);
            trackBarDIF.Value = (int)(Math.Round(slfile.EnvDiffuseMultiplier, 5) * 100);
            trackBarSPC.Value = (int)(Math.Round(slfile.EnvSpecularMultiplier, 5) * 100);
            trackBarRD.Value = (int)(Math.Round(slfile.FreeSlaveRD, 5) * 100);
            numericUpDownRT.Value = slfile.FreeSlaveRT;
        }

        private void UpdateLabels()
        {
            // ENV
            labelEnvR.Text= ((float)numericUpDownEnvR.Value / 255.0f).ToString("0.000");
            labelEnvG.Text = ((float)numericUpDownEnvG.Value / 255.0f).ToString("0.000");
            labelEnvB.Text = ((float)numericUpDownEnvB.Value / 255.0f).ToString("0.000");
            labelSPC.Text = ((float)trackBarSPC.Value / 100.0f).ToString("0.00");
            labelDIF.Text = ((float)trackBarDIF.Value / 100.0f).ToString("0.00");
            labelAMB.Text = ((float)trackBarAMB.Value / 100.0f).ToString("0.00");
            toolStripStatusLabelDirection.Text = "Light Direction: " + slfile.GetLightDirection((int)numericUpDownAngleY.Value, (int)numericUpDownAngleZ.Value).ToString();

            // Free/Slave
            labelFreeR.Text = ((float)numericUpDownFreeR.Value / 255.0f).ToString("0.000");
            labelFreeG.Text = ((float)numericUpDownFreeG.Value / 255.0f).ToString("0.000");
            labelFreeB.Text = ((float)numericUpDownFreeB.Value / 255.0f).ToString("0.000");
            labelRD.Text = ((float)trackBarRD.Value / 100.0f).ToString("0.00");
            labelRT.Text = ((float)numericUpDownRT.Value / 255.0f).ToString("0.000");
        }

        private void UpdateColors()
        {
            pictureBoxColorEnv.BackColor = Color.FromArgb((int)numericUpDownEnvR.Value, (int)numericUpDownEnvG.Value, (int)numericUpDownEnvB.Value);
            pictureBoxColorFree.BackColor = Color.FromArgb((int)numericUpDownFreeR.Value, (int)numericUpDownFreeG.Value, (int)numericUpDownFreeB.Value);
        }

		private void trackBarSPC_Scroll(object sender, EventArgs e)
		{
            UpdateLabels();
		}

		private void numericUpDownEnvR_ValueChanged(object sender, EventArgs e)
		{
            UpdateLabels();
            UpdateColors();
        }

		private void pictureBoxColorEnv_Click(object sender, EventArgs e)
		{
            using (ColorDialog cd = new ColorDialog() { AnyColor = true, AllowFullOpen = true, FullOpen = true })
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    numericUpDownEnvR.Value = cd.Color.R;
                    numericUpDownEnvG.Value = cd.Color.G;
                    numericUpDownEnvB.Value = cd.Color.B;
                }
            }
		}

		private void pictureBoxColorFree_Click(object sender, EventArgs e)
		{
            using (ColorDialog cd = new ColorDialog() { AnyColor = true, AllowFullOpen = true, FullOpen = true })
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    numericUpDownFreeR.Value = cd.Color.R;
                    numericUpDownFreeG.Value = cd.Color.G;
                    numericUpDownFreeB.Value = cd.Color.B;
                }
            }
        }

		private void dreamcastPCToolStripMenuItem_Click(object sender, EventArgs e)
		{
            gamecubeToolStripMenuItem.Checked = isGamecube = false;
            dreamcastToolStripMenuItem.Checked = false;
        }

		private void gamecubeToolStripMenuItem_Click(object sender, EventArgs e)
		{
            gamecubeToolStripMenuItem.Checked = isGamecube = true;
            dreamcastToolStripMenuItem.Checked = false;
        }

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
            slfile = new SLFile();
            UpdateNumerics();
            UpdateLabels();
        }

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
            Close();
		}

        private void SaveSLFile(string path)
        {
            UpdateData();
            File.WriteAllBytes(path, slfile.GetBytes(isGamecube));
        }

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
            SaveSLFile(currentFile);
        }

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using (SaveFileDialog sfd = new SaveFileDialog() { Title = "Save SL File", Filter = "SL Files|SL*.BIN|All Files|*.*" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveSLFile(sfd.FileName);
                }
            }
        }

		private void issueTrackerToolStripMenuItem_Click(object sender, EventArgs e)
		{
            System.Diagnostics.Process.Start("https://github.com/X-Hax/sa_tools/issues");
		}

		private void pLToolHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
            System.Diagnostics.Process.Start("https://github.com/X-Hax/sa_tools/wiki/PL-Tool");
        }
	}
}
