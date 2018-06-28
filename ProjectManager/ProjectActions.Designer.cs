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
            this.ProjectNameLAbel = new System.Windows.Forms.Label();
            this.GameSpecificOptions = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SAMDLButton = new System.Windows.Forms.Button();
            this.SADXLVL2Button = new System.Windows.Forms.Button();
            this.buildGroup = new System.Windows.Forms.GroupBox();
            this.BuildAndRunButton = new System.Windows.Forms.Button();
            this.ExeBuildButton = new System.Windows.Forms.Button();
            this.ConfigBuildButton = new System.Windows.Forms.Button();
            this.BuildDLLDerivedData = new System.Windows.Forms.Button();
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
            this.GameSpecificOptions.Controls.Add(this.button1);
            this.GameSpecificOptions.Controls.Add(this.SAMDLButton);
            this.GameSpecificOptions.Controls.Add(this.SADXLVL2Button);
            this.GameSpecificOptions.Location = new System.Drawing.Point(12, 68);
            this.GameSpecificOptions.Name = "GameSpecificOptions";
            this.GameSpecificOptions.Size = new System.Drawing.Size(336, 192);
            this.GameSpecificOptions.TabIndex = 1;
            this.GameSpecificOptions.TabStop = false;
            this.GameSpecificOptions.Text = "Game Specific Tools";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(6, 77);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(324, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "SADXTweaker2";
            this.button1.UseVisualStyleBackColor = true;
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
            // 
            // buildGroup
            // 
            this.buildGroup.Controls.Add(this.BuildDLLDerivedData);
            this.buildGroup.Controls.Add(this.BuildAndRunButton);
            this.buildGroup.Controls.Add(this.ExeBuildButton);
            this.buildGroup.Controls.Add(this.ConfigBuildButton);
            this.buildGroup.Location = new System.Drawing.Point(354, 68);
            this.buildGroup.Name = "buildGroup";
            this.buildGroup.Size = new System.Drawing.Size(252, 192);
            this.buildGroup.TabIndex = 2;
            this.buildGroup.TabStop = false;
            this.buildGroup.Text = "Build";
            // 
            // BuildAndRunButton
            // 
            this.BuildAndRunButton.Enabled = false;
            this.BuildAndRunButton.Location = new System.Drawing.Point(7, 106);
            this.BuildAndRunButton.Name = "BuildAndRunButton";
            this.BuildAndRunButton.Size = new System.Drawing.Size(239, 23);
            this.BuildAndRunButton.TabIndex = 2;
            this.BuildAndRunButton.Text = "Build and Run";
            this.BuildAndRunButton.UseVisualStyleBackColor = true;
            // 
            // ExeBuildButton
            // 
            this.ExeBuildButton.Location = new System.Drawing.Point(7, 77);
            this.ExeBuildButton.Name = "ExeBuildButton";
            this.ExeBuildButton.Size = new System.Drawing.Size(239, 23);
            this.ExeBuildButton.TabIndex = 1;
            this.ExeBuildButton.Text = "Build Exe-Derived Data";
            this.ExeBuildButton.UseVisualStyleBackColor = true;
            this.ExeBuildButton.Click += new System.EventHandler(this.ExeBuildButton_Click);
            // 
            // ConfigBuildButton
            // 
            this.ConfigBuildButton.Location = new System.Drawing.Point(6, 19);
            this.ConfigBuildButton.Name = "ConfigBuildButton";
            this.ConfigBuildButton.Size = new System.Drawing.Size(240, 23);
            this.ConfigBuildButton.TabIndex = 0;
            this.ConfigBuildButton.Text = "Configure build";
            this.ConfigBuildButton.UseVisualStyleBackColor = true;
            // 
            // BuildDLLDerivedData
            // 
            this.BuildDLLDerivedData.Location = new System.Drawing.Point(6, 48);
            this.BuildDLLDerivedData.Name = "BuildDLLDerivedData";
            this.BuildDLLDerivedData.Size = new System.Drawing.Size(239, 23);
            this.BuildDLLDerivedData.TabIndex = 3;
            this.BuildDLLDerivedData.Text = "Build DLL-Derived Data";
            this.BuildDLLDerivedData.UseVisualStyleBackColor = true;
            this.BuildDLLDerivedData.Click += new System.EventHandler(this.BuildDLLDerivedData_Click);
            // 
            // ProjectActions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 290);
            this.Controls.Add(this.buildGroup);
            this.Controls.Add(this.GameSpecificOptions);
            this.Controls.Add(this.ProjectNameLAbel);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button SAMDLButton;
        private System.Windows.Forms.Button SADXLVL2Button;
        private System.Windows.Forms.GroupBox buildGroup;
        private System.Windows.Forms.Button ExeBuildButton;
        private System.Windows.Forms.Button ConfigBuildButton;
        private System.Windows.Forms.Button BuildAndRunButton;
        private System.Windows.Forms.Button BuildDLLDerivedData;
    }
}