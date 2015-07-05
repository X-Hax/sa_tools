namespace ModGenerator
{
    partial class ModBuildProgress
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
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.StepHeaderLabel = new System.Windows.Forms.Label();
            this.StepText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(12, 85);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(343, 23);
            this.Progress.TabIndex = 0;
            // 
            // StepHeaderLabel
            // 
            this.StepHeaderLabel.AutoSize = true;
            this.StepHeaderLabel.Location = new System.Drawing.Point(148, 21);
            this.StepHeaderLabel.Name = "StepHeaderLabel";
            this.StepHeaderLabel.Size = new System.Drawing.Size(69, 13);
            this.StepHeaderLabel.TabIndex = 1;
            this.StepHeaderLabel.Text = "Current Step:";
            this.StepHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StepText
            // 
            this.StepText.Location = new System.Drawing.Point(12, 41);
            this.StepText.Name = "StepText";
            this.StepText.Size = new System.Drawing.Size(343, 27);
            this.StepText.TabIndex = 2;
            this.StepText.Text = "progress goes here";
            this.StepText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ModBuildProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 143);
            this.ControlBox = false;
            this.Controls.Add(this.StepText);
            this.Controls.Add(this.StepHeaderLabel);
            this.Controls.Add(this.Progress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ModBuildProgress";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mod Build Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar Progress;
        private System.Windows.Forms.Label StepHeaderLabel;
        private System.Windows.Forms.Label StepText;
    }
}