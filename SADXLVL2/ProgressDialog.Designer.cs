namespace SonicRetro.SAModel.SADXLVL2
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
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(12, 39);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(344, 23);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar.TabIndex = 0;
			// 
			// labelTask
			// 
			this.labelTask.AutoSize = true;
			this.labelTask.Location = new System.Drawing.Point(12, 9);
			this.labelTask.Name = "labelTask";
			this.labelTask.Size = new System.Drawing.Size(53, 13);
			this.labelTask.TabIndex = 1;
			this.labelTask.Text = "labelTask";
			// 
			// labelStep
			// 
			this.labelStep.AutoSize = true;
			this.labelStep.Location = new System.Drawing.Point(12, 22);
			this.labelStep.Name = "labelStep";
			this.labelStep.Size = new System.Drawing.Size(51, 13);
			this.labelStep.TabIndex = 2;
			this.labelStep.Text = "labelStep";
			// 
			// ProgressDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(368, 74);
			this.ControlBox = false;
			this.Controls.Add(this.labelStep);
			this.Controls.Add(this.labelTask);
			this.Controls.Add(this.progressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ProgressDialog";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressDialog_FormClosing);
			this.Shown += new System.EventHandler(this.ProgressDialog_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label labelTask;
		private System.Windows.Forms.Label labelStep;
	}
}