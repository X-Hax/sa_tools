namespace SAModel.DataToolbox
{
    partial class ScanProgress
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanProgress));
            this.labelCurrentAddressText = new System.Windows.Forms.Label();
            this.progressBarAddress = new System.Windows.Forms.ProgressBar();
            this.buttonCancelScan = new System.Windows.Forms.Button();
            this.textBoxScanOutput = new System.Windows.Forms.TextBox();
            this.checkBoxPauseLog = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelScanProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelScanStatus = new System.Windows.Forms.Label();
            this.timerScan = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.labelCurrentAddress = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelEndAddress = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelCurrentAddressText
            // 
            this.labelCurrentAddressText.AutoSize = true;
            this.labelCurrentAddressText.Location = new System.Drawing.Point(12, 9);
            this.labelCurrentAddressText.Name = "labelCurrentAddressText";
            this.labelCurrentAddressText.Size = new System.Drawing.Size(149, 25);
            this.labelCurrentAddressText.TabIndex = 0;
            this.labelCurrentAddressText.Text = "Current Address: ";
            // 
            // progressBarAddress
            // 
            this.progressBarAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarAddress.Location = new System.Drawing.Point(12, 37);
            this.progressBarAddress.Name = "progressBarAddress";
            this.progressBarAddress.Size = new System.Drawing.Size(760, 34);
            this.progressBarAddress.TabIndex = 1;
            // 
            // buttonCancelScan
            // 
            this.buttonCancelScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancelScan.Location = new System.Drawing.Point(660, 392);
            this.buttonCancelScan.Name = "buttonCancelScan";
            this.buttonCancelScan.Size = new System.Drawing.Size(112, 34);
            this.buttonCancelScan.TabIndex = 3;
            this.buttonCancelScan.Text = "Cancel";
            this.buttonCancelScan.UseVisualStyleBackColor = true;
            this.buttonCancelScan.Click += new System.EventHandler(this.buttonCancelScan_Click);
            // 
            // textBoxScanOutput
            // 
            this.textBoxScanOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxScanOutput.Location = new System.Drawing.Point(12, 102);
            this.textBoxScanOutput.Multiline = true;
            this.textBoxScanOutput.Name = "textBoxScanOutput";
            this.textBoxScanOutput.ReadOnly = true;
            this.textBoxScanOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxScanOutput.Size = new System.Drawing.Size(759, 284);
            this.textBoxScanOutput.TabIndex = 4;
            // 
            // checkBoxPauseLog
            // 
            this.checkBoxPauseLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxPauseLog.AutoSize = true;
            this.checkBoxPauseLog.Location = new System.Drawing.Point(536, 396);
            this.checkBoxPauseLog.Name = "checkBoxPauseLog";
            this.checkBoxPauseLog.Size = new System.Drawing.Size(118, 29);
            this.checkBoxPauseLog.TabIndex = 5;
            this.checkBoxPauseLog.Text = "Pause Log";
            this.checkBoxPauseLog.UseVisualStyleBackColor = true;
            this.checkBoxPauseLog.CheckedChanged += new System.EventHandler(this.checkBoxPauseLog_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelScanProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 434);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 32);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelScanProgress
            // 
            this.toolStripStatusLabelScanProgress.Name = "toolStripStatusLabelScanProgress";
            this.toolStripStatusLabelScanProgress.Size = new System.Drawing.Size(303, 25);
            this.toolStripStatusLabelScanProgress.Text = "Scan progress will be displayed here.";
            // 
            // labelScanStatus
            // 
            this.labelScanStatus.AutoSize = true;
            this.labelScanStatus.Location = new System.Drawing.Point(12, 74);
            this.labelScanStatus.Name = "labelScanStatus";
            this.labelScanStatus.Size = new System.Drawing.Size(126, 25);
            this.labelScanStatus.TabIndex = 7;
            this.labelScanStatus.Text = "Ready to scan.";
            // 
            // timerScan
            // 
            this.timerScan.Interval = 60;
            this.timerScan.Tick += new System.EventHandler(this.timerScan_Tick);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // labelCurrentAddress
            // 
            this.labelCurrentAddress.AutoSize = true;
            this.labelCurrentAddress.Location = new System.Drawing.Point(167, 9);
            this.labelCurrentAddress.Name = "labelCurrentAddress";
            this.labelCurrentAddress.Size = new System.Drawing.Size(92, 25);
            this.labelCurrentAddress.TabIndex = 8;
            this.labelCurrentAddress.Text = "00000000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(265, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 25);
            this.label2.TabIndex = 9;
            this.label2.Text = "/";
            // 
            // labelEndAddress
            // 
            this.labelEndAddress.AutoSize = true;
            this.labelEndAddress.Location = new System.Drawing.Point(290, 9);
            this.labelEndAddress.Name = "labelEndAddress";
            this.labelEndAddress.Size = new System.Drawing.Size(92, 25);
            this.labelEndAddress.TabIndex = 10;
            this.labelEndAddress.Text = "00000000";
            // 
            // ScanProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 466);
            this.Controls.Add(this.labelEndAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelCurrentAddress);
            this.Controls.Add(this.labelScanStatus);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.checkBoxPauseLog);
            this.Controls.Add(this.textBoxScanOutput);
            this.Controls.Add(this.buttonCancelScan);
            this.Controls.Add(this.progressBarAddress);
            this.Controls.Add(this.labelCurrentAddressText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScanProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scan Progress";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScanProgress_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCurrentAddressText;
        private System.Windows.Forms.ProgressBar progressBarAddress;
        private System.Windows.Forms.Button buttonCancelScan;
        private System.Windows.Forms.TextBox textBoxScanOutput;
        private System.Windows.Forms.CheckBox checkBoxPauseLog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelScanProgress;
        private System.Windows.Forms.Label labelScanStatus;
        private System.Windows.Forms.Timer timerScan;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Label labelCurrentAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelEndAddress;
    }
}