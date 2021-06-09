namespace SonicRetro.SAModel.SAEditorCommon
{
	partial class ErrorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.buttonReport = new System.Windows.Forms.Button();
            this.buttonContinue = new System.Windows.Forms.Button();
            this.labelErrorDescription = new System.Windows.Forms.Label();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.labelReportNote = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonReport
            // 
            this.buttonReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReport.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonReport.Location = new System.Drawing.Point(143, 418);
            this.buttonReport.Name = "buttonReport";
            this.buttonReport.Size = new System.Drawing.Size(75, 23);
            this.buttonReport.TabIndex = 0;
            this.buttonReport.Text = "&Report";
            this.buttonReport.UseVisualStyleBackColor = true;
            this.buttonReport.Click += new System.EventHandler(this.okButton_Click);
            // 
            // buttonContinue
            // 
            this.buttonContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonContinue.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonContinue.Location = new System.Drawing.Point(224, 418);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(75, 23);
            this.buttonContinue.TabIndex = 1;
            this.buttonContinue.Text = "Continue";
            this.buttonContinue.UseVisualStyleBackColor = true;
            this.buttonContinue.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // labelErrorDescription
            // 
            this.labelErrorDescription.AutoSize = true;
            this.labelErrorDescription.Location = new System.Drawing.Point(12, 8);
            this.labelErrorDescription.Name = "labelErrorDescription";
            this.labelErrorDescription.Size = new System.Drawing.Size(332, 39);
            this.labelErrorDescription.TabIndex = 2;
            this.labelErrorDescription.Text = "The tool crashed for the reasons below.\n\nIf you wish to report a bug, please incl" +
    "ude the following in your report:";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(12, 82);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(368, 311);
            this.textBoxLog.TabIndex = 3;
            // 
            // buttonQuit
            // 
            this.buttonQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonQuit.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.buttonQuit.Location = new System.Drawing.Point(305, 418);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(75, 23);
            this.buttonQuit.TabIndex = 4;
            this.buttonQuit.Text = "Quit";
            this.buttonQuit.UseVisualStyleBackColor = true;
            // 
            // labelReportNote
            // 
            this.labelReportNote.AutoSize = true;
            this.labelReportNote.Location = new System.Drawing.Point(12, 396);
            this.labelReportNote.Name = "labelReportNote";
			this.labelReportNote.Size = new System.Drawing.Size(220, 13);
			this.labelReportNote.TabIndex = 5;
            this.labelReportNote.Text = "Click Report to copy the log to the clipboard and open GitHub.";
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.buttonReport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 453);
            this.Controls.Add(this.labelReportNote);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.labelErrorDescription);
            this.Controls.Add(this.buttonContinue);
            this.Controls.Add(this.buttonReport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SA Tools Error";
            this.Load += new System.EventHandler(this.ErrorReportDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonReport;
		private System.Windows.Forms.Button buttonContinue;
		private System.Windows.Forms.Label labelErrorDescription;
		private System.Windows.Forms.TextBox textBoxLog;
		private System.Windows.Forms.Button buttonQuit;
		private System.Windows.Forms.Label labelReportNote;
	}
}

