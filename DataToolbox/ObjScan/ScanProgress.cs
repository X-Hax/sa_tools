using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace SAModel.DataToolbox
{
    public partial class ScanProgress : Form
    {
        public TextBoxWriter writer;
        private bool pauseLog;
        private int stepCount;
        private bool completed;

        public ScanProgress()
        {
            InitializeComponent();
            SetSteps();
            writer = new TextBoxWriter(textBoxScanOutput);
            Console.SetOut(writer);
            labelScanStatus.Text = "Scan started.";
            timerScan.Start();
            backgroundWorker.RunWorkerAsync();
        }

        private void SetSteps()
        {
            if (ObjScan.scan_sa1_land)
                stepCount++; 
            if (ObjScan.scan_sa2_land)
                stepCount++;
            if (ObjScan.scan_sa2b_land)
                stepCount++;
            if (ObjScan.scan_sa1_model)
                stepCount++;
            if (ObjScan.scan_sa2_model)
                stepCount++;
            if (ObjScan.scan_sa2b_model)
                stepCount++;
            if (ObjScan.scan_action)
                stepCount++;
            if (ObjScan.scan_motion)
                stepCount++;
            if (!string.IsNullOrEmpty(ObjScan.matchfile))
                stepCount++;
        }

        private void timerScan_Tick(object sender, EventArgs e)
        {
            if (writer != null && !pauseLog) 
                writer.WriteOut();
            if (!ObjScan.CancelScan)
            {
                labelScanStatus.Text = "Scanning for " + ObjScan.CurrentScanData + " (step " + ObjScan.CurrentStep + " of " + stepCount + ")";
            }
            labelCurrentAddress.Text = ObjScan.CurrentAddress.ToString("X8");
            labelEndAddress.Text = ObjScan.EndAddress.ToString("X8");
            progressBarAddress.Maximum = (int)ObjScan.EndAddress + (int)ObjScan.DataOffset;
            progressBarAddress.Value = (int)ObjScan.CurrentAddress;
            List<string> results = new List<string>();
            if (ObjScan.FoundSA1Landtables > 0)
                results.Add(ObjScan.FoundSA1Landtables.ToString() + " SA1 levels");
            if (ObjScan.FoundSADXLandtables > 0)
                results.Add(ObjScan.FoundSADXLandtables.ToString() + " SADXPC/X360 levels");
            if (ObjScan.FoundSA2Landtables > 0)
                results.Add(ObjScan.FoundSA2Landtables.ToString() + " SA2 levels");
            if (ObjScan.FoundSA2BLandtables > 0)
                results.Add(ObjScan.FoundSA2BLandtables.ToString() + " SA2B/SA2PC levels");
            if (ObjScan.FoundBasicModels > 0)
                results.Add(ObjScan.FoundBasicModels.ToString() + " Basic models");
            if (ObjScan.FoundChunkModels > 0)
                results.Add(ObjScan.FoundChunkModels.ToString() + " Chunk models");
            if (ObjScan.FoundGCModels > 0)
                results.Add(ObjScan.FoundGCModels.ToString() + " Ginja models");
            if (ObjScan.FoundActions > 0)
                results.Add(ObjScan.FoundActions.ToString() + " Actions");
            if (ObjScan.FoundMotions > 0)
                results.Add(ObjScan.FoundMotions.ToString() + " Motions");
            if (results.Count > 0)
                toolStripStatusLabelScanProgress.Text = "Found " + string.Join(", ", results);
            else
                toolStripStatusLabelScanProgress.Text = "Nothing found yet";
        }

        private void checkBoxPauseLog_CheckedChanged(object sender, EventArgs e)
        {
            pauseLog = checkBoxPauseLog.Checked;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ObjScan.PerformScan();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timerScan.Stop();
            completed = true;
            buttonCancelScan.Text = "Close";
        }

        private void buttonCancelScan_Click(object sender, EventArgs e)
        {
            if (!ObjScan.CancelScan && !completed)
            {
                labelScanStatus.Text = "Scan cancelled.";
                ObjScan.CancelScan = true;
            }
            else 
                Close();
        }

        private void ScanProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!completed)
                ObjScan.CancelScan = true;
        }
    }
}
