namespace SASave
{
    partial class WeightControl
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
            this.weight1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.weight2 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.weight3 = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.time = new SASave.TimeControl();
            ((System.ComponentModel.ISupportInitialize)(this.weight1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weight2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weight3)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // weight1
            // 
            this.weight1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.weight1.Location = new System.Drawing.Point(0, 0);
            this.weight1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.weight1.Maximum = new decimal(new int[] {
            655350,
            0,
            0,
            0});
            this.weight1.Name = "weight1";
            this.weight1.Size = new System.Drawing.Size(55, 20);
            this.weight1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "g";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "g";
            // 
            // weight2
            // 
            this.weight2.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.weight2.Location = new System.Drawing.Point(0, 23);
            this.weight2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.weight2.Maximum = new decimal(new int[] {
            655350,
            0,
            0,
            0});
            this.weight2.Name = "weight2";
            this.weight2.Size = new System.Drawing.Size(55, 20);
            this.weight2.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(55, 48);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "g";
            // 
            // weight3
            // 
            this.weight3.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.weight3.Location = new System.Drawing.Point(0, 46);
            this.weight3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.weight3.Maximum = new decimal(new int[] {
            655350,
            0,
            0,
            0});
            this.weight3.Name = "weight3";
            this.weight3.Size = new System.Drawing.Size(55, 20);
            this.weight3.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.weight1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.weight3);
            this.panel1.Controls.Add(this.weight2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(68, 69);
            this.panel1.TabIndex = 6;
            this.panel1.Visible = false;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.time);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(143, 26);
            this.panel2.TabIndex = 7;
            // 
            // time
            // 
            this.time.AutoSize = true;
            this.time.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.time.Centiseconds = 0;
            this.time.Frames = ((uint)(0u));
            this.time.Location = new System.Drawing.Point(0, 0);
            this.time.Margin = new System.Windows.Forms.Padding(0);
            this.time.Minutes = 0;
            this.time.Name = "time";
            this.time.Seconds = 0;
            this.time.Size = new System.Drawing.Size(143, 26);
            this.time.TabIndex = 0;
            this.time.TimeSpan = System.TimeSpan.Parse("00:00:00");
            this.time.TotalCentiseconds = ((uint)(0u));
            this.time.TotalFrames = ((uint)(0u));
            this.time.ValueChanged += new System.EventHandler(this.Time_ValueChanged);
            // 
            // WeightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "WeightControl";
            this.Size = new System.Drawing.Size(143, 69);
            ((System.ComponentModel.ISupportInitialize)(this.weight1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weight2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weight3)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown weight1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown weight2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown weight3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private TimeControl time;
    }
}
