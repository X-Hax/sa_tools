namespace SASave
{
    partial class TimeControl
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
			this.minutes = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.seconds = new System.Windows.Forms.NumericUpDown();
			this.centiseconds = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.minutes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.seconds)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.centiseconds)).BeginInit();
			this.SuspendLayout();
			// 
			// minutes
			// 
			this.minutes.Location = new System.Drawing.Point(3, 3);
			this.minutes.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.minutes.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.minutes.Name = "minutes";
			this.minutes.Size = new System.Drawing.Size(39, 20);
			this.minutes.TabIndex = 0;
			this.minutes.ValueChanged += new System.EventHandler(this.minutes_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(42, 5);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(10, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = ":";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(91, 5);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(10, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = ":";
			// 
			// seconds
			// 
			this.seconds.Location = new System.Drawing.Point(52, 3);
			this.seconds.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.seconds.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.seconds.Name = "seconds";
			this.seconds.Size = new System.Drawing.Size(39, 20);
			this.seconds.TabIndex = 2;
			this.seconds.ValueChanged += new System.EventHandler(this.minutes_ValueChanged);
			// 
			// centiseconds
			// 
			this.centiseconds.Location = new System.Drawing.Point(101, 3);
			this.centiseconds.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.centiseconds.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.centiseconds.Name = "centiseconds";
			this.centiseconds.Size = new System.Drawing.Size(39, 20);
			this.centiseconds.TabIndex = 4;
			this.centiseconds.ValueChanged += new System.EventHandler(this.minutes_ValueChanged);
			// 
			// TimeControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.centiseconds);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.seconds);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.minutes);
			this.Name = "TimeControl";
			this.Size = new System.Drawing.Size(143, 26);
			((System.ComponentModel.ISupportInitialize)(this.minutes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.seconds)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.centiseconds)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown minutes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown seconds;
        private System.Windows.Forms.NumericUpDown centiseconds;
    }
}
