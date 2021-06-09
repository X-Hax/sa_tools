namespace SASave
{
    partial class TimeOfDayControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.night = new System.Windows.Forms.RadioButton();
            this.evening = new System.Windows.Forms.RadioButton();
            this.day = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.night);
            this.groupBox1.Controls.Add(this.evening);
            this.groupBox1.Controls.Add(this.day);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox1.Size = new System.Drawing.Size(182, 49);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time of Day";
            // 
            // night
            // 
            this.night.AutoSize = true;
            this.night.Location = new System.Drawing.Point(126, 19);
            this.night.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.night.Name = "night";
            this.night.Size = new System.Drawing.Size(50, 17);
            this.night.TabIndex = 2;
            this.night.Text = "Night";
            this.night.UseVisualStyleBackColor = true;
            this.night.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // evening
            // 
            this.evening.AutoSize = true;
            this.evening.Location = new System.Drawing.Point(56, 19);
            this.evening.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.evening.Name = "evening";
            this.evening.Size = new System.Drawing.Size(64, 17);
            this.evening.TabIndex = 1;
            this.evening.Text = "Evening";
            this.evening.UseVisualStyleBackColor = true;
            this.evening.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // day
            // 
            this.day.AutoSize = true;
            this.day.Checked = true;
            this.day.Location = new System.Drawing.Point(6, 19);
            this.day.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.day.Name = "day";
            this.day.Size = new System.Drawing.Size(44, 17);
            this.day.TabIndex = 0;
            this.day.TabStop = true;
            this.day.Text = "Day";
            this.day.UseVisualStyleBackColor = true;
            this.day.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // TimeOfDayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.groupBox1);
            this.Name = "TimeOfDayControl";
            this.Size = new System.Drawing.Size(182, 49);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton night;
        private System.Windows.Forms.RadioButton evening;
        private System.Windows.Forms.RadioButton day;
    }
}
