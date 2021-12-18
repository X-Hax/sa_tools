namespace SAModel.SAEditorCommon.UI
{
	partial class ProgressDialog
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelTask = new System.Windows.Forms.Label();
            this.labelStep = new System.Windows.Forms.Label();
            this.checkAutoClose = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(15, 48);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.progressBar.MaximumSize = new System.Drawing.Size(467, 27);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(467, 27);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 3;
            // 
            // labelTask
            // 
            this.labelTask.AutoSize = true;
            this.labelTask.Location = new System.Drawing.Point(14, 10);
            this.labelTask.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTask.MaximumSize = new System.Drawing.Size(467, 20);
            this.labelTask.Name = "labelTask";
            this.labelTask.Size = new System.Drawing.Size(54, 15);
            this.labelTask.TabIndex = 1;
            this.labelTask.Text = "labelTask";
            // 
            // labelStep
            // 
            this.labelStep.AutoSize = true;
            this.labelStep.Location = new System.Drawing.Point(14, 25);
            this.labelStep.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStep.MaximumSize = new System.Drawing.Size(467, 20);
            this.labelStep.Name = "labelStep";
            this.labelStep.Size = new System.Drawing.Size(55, 15);
            this.labelStep.TabIndex = 2;
            this.labelStep.Text = "labelStep";
            // 
            // checkAutoClose
            // 
            this.checkAutoClose.AutoSize = true;
            this.checkAutoClose.Location = new System.Drawing.Point(14, 83);
            this.checkAutoClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkAutoClose.Name = "checkAutoClose";
            this.checkAutoClose.Size = new System.Drawing.Size(140, 19);
            this.checkAutoClose.TabIndex = 4;
            this.checkAutoClose.Text = "&Close when complete";
            this.checkAutoClose.UseVisualStyleBackColor = true;
            this.checkAutoClose.CheckedChanged += new System.EventHandler(this.checkAutoClose_CheckedChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(394, 83);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(88, 27);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // ProgressDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(496, 119);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkAutoClose);
            this.Controls.Add(this.labelStep);
            this.Controls.Add(this.labelTask);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProgressDialog";
            this.Load += new System.EventHandler(this.ProgressDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label labelTask;
		private System.Windows.Forms.Label labelStep;
		private System.Windows.Forms.CheckBox checkAutoClose;
		private System.Windows.Forms.Button buttonOK;
	}
}