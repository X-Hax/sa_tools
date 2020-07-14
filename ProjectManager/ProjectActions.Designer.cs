namespace ProjectManager
{
    partial class ProjectActions
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectActions));
			this.ProjectNameLAbel = new System.Windows.Forms.Label();
			this.GameSpecificOptions = new System.Windows.Forms.GroupBox();
			this.SADXTweaker2Button = new System.Windows.Forms.Button();
			this.SAMDLButton = new System.Windows.Forms.Button();
			this.SADXLVL2Button = new System.Windows.Forms.Button();
			this.buildGroup = new System.Windows.Forms.GroupBox();
			this.AutoBuildButton = new System.Windows.Forms.Button();
			this.ManualBuildbutton = new System.Windows.Forms.Button();
			this.BuildAndRunButton = new System.Windows.Forms.Button();
			this.ConfigBuildButton = new System.Windows.Forms.Button();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.openProjectFolderButton = new System.Windows.Forms.Button();
			this.GameSpecificOptions.SuspendLayout();
			this.buildGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// ProjectNameLAbel
			// 
			this.ProjectNameLAbel.AutoSize = true;
			this.ProjectNameLAbel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProjectNameLAbel.Location = new System.Drawing.Point(12, 11);
			this.ProjectNameLAbel.Name = "ProjectNameLAbel";
			this.ProjectNameLAbel.Size = new System.Drawing.Size(121, 20);
			this.ProjectNameLAbel.TabIndex = 0;
			this.ProjectNameLAbel.Text = "Project Name:";
			// 
			// GameSpecificOptions
			// 
			this.GameSpecificOptions.Controls.Add(this.SADXTweaker2Button);
			this.GameSpecificOptions.Controls.Add(this.SAMDLButton);
			this.GameSpecificOptions.Controls.Add(this.SADXLVL2Button);
			this.GameSpecificOptions.Location = new System.Drawing.Point(12, 68);
			this.GameSpecificOptions.Name = "GameSpecificOptions";
			this.GameSpecificOptions.Size = new System.Drawing.Size(336, 192);
			this.GameSpecificOptions.TabIndex = 1;
			this.GameSpecificOptions.TabStop = false;
			this.GameSpecificOptions.Text = "Game Specific Tools";
			// 
			// SADXTweaker2Button
			// 
			this.SADXTweaker2Button.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.SADXTweaker2Button.Location = new System.Drawing.Point(6, 77);
			this.SADXTweaker2Button.Name = "SADXTweaker2Button";
			this.SADXTweaker2Button.Size = new System.Drawing.Size(324, 23);
			this.SADXTweaker2Button.TabIndex = 2;
			this.SADXTweaker2Button.Text = "SADXTweaker2";
			this.SADXTweaker2Button.UseVisualStyleBackColor = true;
			this.SADXTweaker2Button.Click += new System.EventHandler(this.SADXTweaker2Button_Click);
			// 
			// SAMDLButton
			// 
			this.SAMDLButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.SAMDLButton.Location = new System.Drawing.Point(6, 48);
			this.SAMDLButton.Name = "SAMDLButton";
			this.SAMDLButton.Size = new System.Drawing.Size(324, 23);
			this.SAMDLButton.TabIndex = 1;
			this.SAMDLButton.Text = "SAMDL";
			this.SAMDLButton.UseVisualStyleBackColor = true;
			this.SAMDLButton.Click += new System.EventHandler(this.SAMDLButton_Click);
			// 
			// SADXLVL2Button
			// 
			this.SADXLVL2Button.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.SADXLVL2Button.Location = new System.Drawing.Point(6, 19);
			this.SADXLVL2Button.Name = "SADXLVL2Button";
			this.SADXLVL2Button.Size = new System.Drawing.Size(324, 23);
			this.SADXLVL2Button.TabIndex = 0;
			this.SADXLVL2Button.Text = "SADXLVL2";
			this.SADXLVL2Button.UseVisualStyleBackColor = true;
			this.SADXLVL2Button.Click += new System.EventHandler(this.SADXLVL2Button_Click);
			// 
			// buildGroup
			// 
			this.buildGroup.Controls.Add(this.AutoBuildButton);
			this.buildGroup.Controls.Add(this.ManualBuildbutton);
			this.buildGroup.Controls.Add(this.BuildAndRunButton);
			this.buildGroup.Controls.Add(this.ConfigBuildButton);
			this.buildGroup.Location = new System.Drawing.Point(354, 68);
			this.buildGroup.Name = "buildGroup";
			this.buildGroup.Size = new System.Drawing.Size(252, 142);
			this.buildGroup.TabIndex = 2;
			this.buildGroup.TabStop = false;
			this.buildGroup.Text = "Build";
			// 
			// AutoBuildButton
			// 
			this.AutoBuildButton.Location = new System.Drawing.Point(7, 77);
			this.AutoBuildButton.Name = "AutoBuildButton";
			this.AutoBuildButton.Size = new System.Drawing.Size(239, 23);
			this.AutoBuildButton.TabIndex = 6;
			this.AutoBuildButton.Text = "Auto Build";
			this.AutoBuildButton.UseVisualStyleBackColor = true;
			this.AutoBuildButton.Click += new System.EventHandler(this.AutoBuildButton_Click);
			// 
			// ManualBuildbutton
			// 
			this.ManualBuildbutton.Location = new System.Drawing.Point(7, 48);
			this.ManualBuildbutton.Name = "ManualBuildbutton";
			this.ManualBuildbutton.Size = new System.Drawing.Size(239, 23);
			this.ManualBuildbutton.TabIndex = 5;
			this.ManualBuildbutton.Text = "Manual Build";
			this.ManualBuildbutton.UseVisualStyleBackColor = true;
			this.ManualBuildbutton.Click += new System.EventHandler(this.ManualBuildbutton_Click);
			// 
			// BuildAndRunButton
			// 
			this.BuildAndRunButton.Location = new System.Drawing.Point(6, 106);
			this.BuildAndRunButton.Name = "BuildAndRunButton";
			this.BuildAndRunButton.Size = new System.Drawing.Size(239, 23);
			this.BuildAndRunButton.TabIndex = 2;
			this.BuildAndRunButton.Text = "Build and Run";
			this.BuildAndRunButton.UseVisualStyleBackColor = true;
			this.BuildAndRunButton.Click += new System.EventHandler(this.BuildAndRunButton_Click);
			// 
			// ConfigBuildButton
			// 
			this.ConfigBuildButton.Location = new System.Drawing.Point(6, 19);
			this.ConfigBuildButton.Name = "ConfigBuildButton";
			this.ConfigBuildButton.Size = new System.Drawing.Size(240, 23);
			this.ConfigBuildButton.TabIndex = 0;
			this.ConfigBuildButton.Text = "Configure build";
			this.ConfigBuildButton.UseVisualStyleBackColor = true;
			this.ConfigBuildButton.Click += new System.EventHandler(this.ConfigBuildButton_Click);
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			// 
			// openProjectFolderButton
			// 
			this.openProjectFolderButton.Location = new System.Drawing.Point(361, 223);
			this.openProjectFolderButton.Name = "openProjectFolderButton";
			this.openProjectFolderButton.Size = new System.Drawing.Size(238, 23);
			this.openProjectFolderButton.TabIndex = 3;
			this.openProjectFolderButton.Text = "Open Project Folder";
			this.openProjectFolderButton.UseVisualStyleBackColor = true;
			this.openProjectFolderButton.Click += new System.EventHandler(this.openProjectFolderButton_Click);
			// 
			// ProjectActions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(618, 290);
			this.Controls.Add(this.openProjectFolderButton);
			this.Controls.Add(this.buildGroup);
			this.Controls.Add(this.GameSpecificOptions);
			this.Controls.Add(this.ProjectNameLAbel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ProjectActions";
			this.Text = "Project Actions";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjectActions_FormClosing);
			this.GameSpecificOptions.ResumeLayout(false);
			this.buildGroup.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ProjectNameLAbel;
        private System.Windows.Forms.GroupBox GameSpecificOptions;
        private System.Windows.Forms.Button SADXTweaker2Button;
        private System.Windows.Forms.Button SAMDLButton;
        private System.Windows.Forms.Button SADXLVL2Button;
        private System.Windows.Forms.GroupBox buildGroup;
        private System.Windows.Forms.Button ConfigBuildButton;
        private System.Windows.Forms.Button BuildAndRunButton;
        private System.Windows.Forms.Button ManualBuildbutton;
        private System.Windows.Forms.Button AutoBuildButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.Button openProjectFolderButton;
	}
}