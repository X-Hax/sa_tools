namespace ProjectManager
{
    partial class NewProject
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
            this.SADXPCButton = new System.Windows.Forms.RadioButton();
            this.SA2RadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.NextButton = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ProjectNameBox = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // SADXPCButton
            // 
            this.SADXPCButton.AutoSize = true;
            this.SADXPCButton.Location = new System.Drawing.Point(8, 87);
            this.SADXPCButton.Name = "SADXPCButton";
            this.SADXPCButton.Size = new System.Drawing.Size(71, 17);
            this.SADXPCButton.TabIndex = 0;
            this.SADXPCButton.TabStop = true;
            this.SADXPCButton.Text = "SADX PC";
            this.SADXPCButton.UseVisualStyleBackColor = true;
            // 
            // SA2RadioButton
            // 
            this.SA2RadioButton.AutoSize = true;
            this.SA2RadioButton.Location = new System.Drawing.Point(8, 110);
            this.SA2RadioButton.Name = "SA2RadioButton";
            this.SA2RadioButton.Size = new System.Drawing.Size(62, 17);
            this.SA2RadioButton.TabIndex = 1;
            this.SA2RadioButton.TabStop = true;
            this.SA2RadioButton.Text = "SA2 PC";
            this.SA2RadioButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Game Type:";
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(320, 146);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 23);
            this.NextButton.TabIndex = 3;
            this.NextButton.Text = "Create Project";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // BackButton
            // 
            this.BackButton.Location = new System.Drawing.Point(8, 146);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(75, 23);
            this.BackButton.TabIndex = 4;
            this.BackButton.Text = "Cancel";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Project Name";
            // 
            // ProjectNameBox
            // 
            this.ProjectNameBox.Location = new System.Drawing.Point(8, 27);
            this.ProjectNameBox.Name = "ProjectNameBox";
            this.ProjectNameBox.Size = new System.Drawing.Size(387, 20);
            this.ProjectNameBox.TabIndex = 6;
            this.ProjectNameBox.TextChanged += new System.EventHandler(this.ProjectNameBox_TextChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // NewProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 181);
            this.Controls.Add(this.ProjectNameBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SA2RadioButton);
            this.Controls.Add(this.SADXPCButton);
            this.Name = "NewProject";
            this.Text = "NewProject";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewProject_FormClosing);
            this.Shown += new System.EventHandler(this.NewProject_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton SADXPCButton;
        private System.Windows.Forms.RadioButton SA2RadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ProjectNameBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}