namespace ProjectManager
{
    partial class ProjectSelect
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
            this.projectListBox = new System.Windows.Forms.ListBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // InstructionLabel
            // 
            this.InstructionLabel.AutoSize = true;
            this.InstructionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstructionLabel.Location = new System.Drawing.Point(12, 9);
            this.InstructionLabel.Name = "InstructionLabel";
            this.InstructionLabel.Size = new System.Drawing.Size(136, 20);
            this.InstructionLabel.TabIndex = 0;
            this.InstructionLabel.Text = "Select a Project";
            // 
            // projectListBox
            // 
            this.projectListBox.FormattingEnabled = true;
            this.projectListBox.Location = new System.Drawing.Point(12, 43);
            this.projectListBox.Name = "projectListBox";
            this.projectListBox.Size = new System.Drawing.Size(439, 368);
            this.projectListBox.TabIndex = 1;
            this.projectListBox.SelectedIndexChanged += new System.EventHandler(this.projectListBox_SelectedIndexChanged);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(376, 419);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // BackButton
            // 
            this.BackButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BackButton.Location = new System.Drawing.Point(12, 419);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(75, 23);
            this.BackButton.TabIndex = 3;
            this.BackButton.Text = "Cancel";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // ProjectSelect
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BackButton;
            this.ClientSize = new System.Drawing.Size(463, 450);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.projectListBox);
            this.Controls.Add(this.InstructionLabel);
            this.Name = "ProjectSelect";
            this.Text = "ProjectSelect";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjectSelect_FormClosing);
            this.Shown += new System.EventHandler(this.ProjectSelect_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label InstructionLabel;
        private System.Windows.Forms.ListBox projectListBox;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button BackButton;
    }
}