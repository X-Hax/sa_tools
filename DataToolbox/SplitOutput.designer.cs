namespace SonicRetro.SAModel.DataToolbox
{
	partial class SplitProgress
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
            this.buttonCloseSplitProgress = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkboxPause = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonCloseSplitProgress
            // 
            this.buttonCloseSplitProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCloseSplitProgress.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCloseSplitProgress.Enabled = true;
            this.buttonCloseSplitProgress.Location = new System.Drawing.Point(697, 406);
            this.buttonCloseSplitProgress.Name = "buttonCloseSplitProgress";
            this.buttonCloseSplitProgress.Size = new System.Drawing.Size(75, 23);
            this.buttonCloseSplitProgress.TabIndex = 1;
            this.buttonCloseSplitProgress.Text = "Close";
            this.buttonCloseSplitProgress.UseVisualStyleBackColor = true;
            this.buttonCloseSplitProgress.Click += new System.EventHandler(this.buttonCloseSplitProgress_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Split progress will be displayed below.";
            // 
            // txtConsole
            // 
            this.txtConsole.Location = new System.Drawing.Point(12, 24);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.Size = new System.Drawing.Size(760, 369);
            this.txtConsole.TabIndex = 3;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
            this.timer1.Interval = 60;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkboxPause
            // 
            this.checkboxPause.AutoSize = true;
            this.checkboxPause.Location = new System.Drawing.Point(611, 410);
            this.checkboxPause.Name = "checkboxPause";
            this.checkboxPause.Size = new System.Drawing.Size(73, 17);
            this.checkboxPause.TabIndex = 4;
            this.checkboxPause.Text = "Pause log";
            this.checkboxPause.UseVisualStyleBackColor = true;
            this.checkboxPause.CheckedChanged += new System.EventHandler(this.checkboxPause_CheckedChanged);
            // 
            // SplitProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.checkboxPause);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCloseSplitProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplitProgress";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Split Progress";
            this.Shown += new System.EventHandler(this.SplitProgress_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button buttonCloseSplitProgress;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtConsole;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.CheckBox checkboxPause;
	}
}

