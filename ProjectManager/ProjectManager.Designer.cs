namespace ProjectManager
{
    partial class ProjectManager
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
            this.NewProjectButton = new System.Windows.Forms.Button();
            this.OpenProjectButton = new System.Windows.Forms.Button();
            this.ConfigButton = new System.Windows.Forms.Button();
            this.SplitToolsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NewProjectButton
            // 
            this.NewProjectButton.Location = new System.Drawing.Point(25, 18);
            this.NewProjectButton.Name = "NewProjectButton";
            this.NewProjectButton.Size = new System.Drawing.Size(360, 23);
            this.NewProjectButton.TabIndex = 0;
            this.NewProjectButton.Text = "New Project";
            this.NewProjectButton.UseVisualStyleBackColor = true;
            this.NewProjectButton.Click += new System.EventHandler(this.NewProjectButton_Click);
            // 
            // OpenProjectButton
            // 
            this.OpenProjectButton.Location = new System.Drawing.Point(25, 57);
            this.OpenProjectButton.Name = "OpenProjectButton";
            this.OpenProjectButton.Size = new System.Drawing.Size(360, 23);
            this.OpenProjectButton.TabIndex = 1;
            this.OpenProjectButton.Text = "Open Project";
            this.OpenProjectButton.UseVisualStyleBackColor = true;
            // 
            // ConfigButton
            // 
            this.ConfigButton.Location = new System.Drawing.Point(25, 96);
            this.ConfigButton.Name = "ConfigButton";
            this.ConfigButton.Size = new System.Drawing.Size(360, 23);
            this.ConfigButton.TabIndex = 2;
            this.ConfigButton.Text = "Config";
            this.ConfigButton.UseVisualStyleBackColor = true;
            this.ConfigButton.Click += new System.EventHandler(this.ConfigButton_Click);
            // 
            // SplitToolsButton
            // 
            this.SplitToolsButton.Location = new System.Drawing.Point(25, 138);
            this.SplitToolsButton.Name = "SplitToolsButton";
            this.SplitToolsButton.Size = new System.Drawing.Size(360, 23);
            this.SplitToolsButton.TabIndex = 3;
            this.SplitToolsButton.Text = "Split Tools";
            this.SplitToolsButton.UseVisualStyleBackColor = true;
            // 
            // ProjectManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 184);
            this.Controls.Add(this.SplitToolsButton);
            this.Controls.Add(this.ConfigButton);
            this.Controls.Add(this.OpenProjectButton);
            this.Controls.Add(this.NewProjectButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "ProjectManager";
            this.Text = "Project Manger";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button NewProjectButton;
        private System.Windows.Forms.Button OpenProjectButton;
        private System.Windows.Forms.Button ConfigButton;
        private System.Windows.Forms.Button SplitToolsButton;
    }
}