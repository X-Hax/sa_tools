namespace ModGenerator
{
	partial class MainForm
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
            this.InstructionLabel = new System.Windows.Forms.Label();
            this.NewProjectButton = new System.Windows.Forms.Button();
            this.buildExistingButton = new System.Windows.Forms.Button();
            this.StartPanel = new System.Windows.Forms.Panel();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.StartPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // InstructionLabel
            // 
            this.InstructionLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.InstructionLabel.AutoSize = true;
            this.InstructionLabel.Location = new System.Drawing.Point(117, 74);
            this.InstructionLabel.Name = "InstructionLabel";
            this.InstructionLabel.Size = new System.Drawing.Size(127, 13);
            this.InstructionLabel.TabIndex = 0;
            this.InstructionLabel.Text = "What do you want to do?";
            this.InstructionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // NewProjectButton
            // 
            this.NewProjectButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NewProjectButton.Location = new System.Drawing.Point(88, 118);
            this.NewProjectButton.Name = "NewProjectButton";
            this.NewProjectButton.Size = new System.Drawing.Size(179, 23);
            this.NewProjectButton.TabIndex = 1;
            this.NewProjectButton.Text = "Start a new Project";
            this.NewProjectButton.UseVisualStyleBackColor = true;
            this.NewProjectButton.Click += new System.EventHandler(this.NewProjectButton_Click);
            // 
            // buildExistingButton
            // 
            this.buildExistingButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buildExistingButton.Location = new System.Drawing.Point(88, 183);
            this.buildExistingButton.Name = "buildExistingButton";
            this.buildExistingButton.Size = new System.Drawing.Size(179, 23);
            this.buildExistingButton.TabIndex = 2;
            this.buildExistingButton.Text = "Build an Existing Project";
            this.buildExistingButton.UseVisualStyleBackColor = true;
            this.buildExistingButton.Click += new System.EventHandler(this.buildExistingButton_Click);
            // 
            // StartPanel
            // 
            this.StartPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StartPanel.Controls.Add(this.NewProjectButton);
            this.StartPanel.Controls.Add(this.buildExistingButton);
            this.StartPanel.Controls.Add(this.InstructionLabel);
            this.StartPanel.Location = new System.Drawing.Point(0, 0);
            this.StartPanel.Name = "StartPanel";
            this.StartPanel.Size = new System.Drawing.Size(374, 288);
            this.StartPanel.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 288);
            this.Controls.Add(this.StartPanel);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mod Generator";
            this.StartPanel.ResumeLayout(false);
            this.StartPanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label InstructionLabel;
		private System.Windows.Forms.Button NewProjectButton;
		private System.Windows.Forms.Button buildExistingButton;
		private System.Windows.Forms.Panel StartPanel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
	}
}