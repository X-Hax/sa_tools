namespace SASave
{
    partial class SaveFormatDialog
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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.systemSteam = new System.Windows.Forms.RadioButton();
			this.system360 = new System.Windows.Forms.RadioButton();
			this.systemPC = new System.Windows.Forms.RadioButton();
			this.systemGamecube = new System.Windows.Forms.RadioButton();
			this.systemDreamcast = new System.Windows.Forms.RadioButton();
			this.dcRegions = new System.Windows.Forms.GroupBox();
			this.dcRegionInternational = new System.Windows.Forms.RadioButton();
			this.dcRegionUS = new System.Windows.Forms.RadioButton();
			this.dcRegionJapan = new System.Windows.Forms.RadioButton();
			this.gcRegions = new System.Windows.Forms.GroupBox();
			this.gcRegionEurope = new System.Windows.Forms.RadioButton();
			this.gcRegionUS = new System.Windows.Forms.RadioButton();
			this.gcRegionJapan = new System.Windows.Forms.RadioButton();
			this.systemGameCubePreview = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.dcRegions.SuspendLayout();
			this.gcRegions.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(418, 99);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "&Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button2.Location = new System.Drawing.Point(337, 99);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 2;
			this.button2.Text = "&OK";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox1.Controls.Add(this.systemGameCubePreview);
			this.groupBox1.Controls.Add(this.systemSteam);
			this.groupBox1.Controls.Add(this.system360);
			this.groupBox1.Controls.Add(this.systemPC);
			this.groupBox1.Controls.Add(this.systemGamecube);
			this.groupBox1.Controls.Add(this.systemDreamcast);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.groupBox1.Size = new System.Drawing.Size(480, 52);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "System";
			// 
			// systemSteam
			// 
			this.systemSteam.AutoSize = true;
			this.systemSteam.Location = new System.Drawing.Point(294, 19);
			this.systemSteam.Name = "systemSteam";
			this.systemSteam.Size = new System.Drawing.Size(55, 17);
			this.systemSteam.TabIndex = 4;
			this.systemSteam.TabStop = true;
			this.systemSteam.Text = "&Steam";
			this.systemSteam.UseVisualStyleBackColor = true;
			// 
			// system360
			// 
			this.system360.AutoSize = true;
			this.system360.Location = new System.Drawing.Point(217, 19);
			this.system360.Name = "system360";
			this.system360.Size = new System.Drawing.Size(71, 17);
			this.system360.TabIndex = 3;
			this.system360.TabStop = true;
			this.system360.Text = "&XBox 360";
			this.system360.UseVisualStyleBackColor = true;
			// 
			// systemPC
			// 
			this.systemPC.AutoSize = true;
			this.systemPC.Location = new System.Drawing.Point(172, 19);
			this.systemPC.Name = "systemPC";
			this.systemPC.Size = new System.Drawing.Size(39, 17);
			this.systemPC.TabIndex = 2;
			this.systemPC.TabStop = true;
			this.systemPC.Text = "&PC";
			this.systemPC.UseVisualStyleBackColor = true;
			this.systemPC.CheckedChanged += new System.EventHandler(this.system_CheckedChanged);
			// 
			// systemGamecube
			// 
			this.systemGamecube.AutoSize = true;
			this.systemGamecube.Location = new System.Drawing.Point(88, 19);
			this.systemGamecube.Name = "systemGamecube";
			this.systemGamecube.Size = new System.Drawing.Size(78, 17);
			this.systemGamecube.TabIndex = 1;
			this.systemGamecube.TabStop = true;
			this.systemGamecube.Text = "&GameCube";
			this.systemGamecube.UseVisualStyleBackColor = true;
			this.systemGamecube.CheckedChanged += new System.EventHandler(this.system_CheckedChanged);
			// 
			// systemDreamcast
			// 
			this.systemDreamcast.AutoSize = true;
			this.systemDreamcast.Location = new System.Drawing.Point(6, 19);
			this.systemDreamcast.Name = "systemDreamcast";
			this.systemDreamcast.Size = new System.Drawing.Size(76, 17);
			this.systemDreamcast.TabIndex = 0;
			this.systemDreamcast.TabStop = true;
			this.systemDreamcast.Text = "&Dreamcast";
			this.systemDreamcast.UseVisualStyleBackColor = true;
			this.systemDreamcast.CheckedChanged += new System.EventHandler(this.system_CheckedChanged);
			// 
			// dcRegions
			// 
			this.dcRegions.AutoSize = true;
			this.dcRegions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.dcRegions.Controls.Add(this.dcRegionInternational);
			this.dcRegions.Controls.Add(this.dcRegionUS);
			this.dcRegions.Controls.Add(this.dcRegionJapan);
			this.dcRegions.Location = new System.Drawing.Point(12, 70);
			this.dcRegions.Name = "dcRegions";
			this.dcRegions.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.dcRegions.Size = new System.Drawing.Size(240, 52);
			this.dcRegions.TabIndex = 4;
			this.dcRegions.TabStop = false;
			this.dcRegions.Text = "Region";
			this.dcRegions.Visible = false;
			// 
			// dcRegionInternational
			// 
			this.dcRegionInternational.AutoSize = true;
			this.dcRegionInternational.Location = new System.Drawing.Point(151, 19);
			this.dcRegionInternational.Name = "dcRegionInternational";
			this.dcRegionInternational.Size = new System.Drawing.Size(83, 17);
			this.dcRegionInternational.TabIndex = 2;
			this.dcRegionInternational.Text = "&International";
			this.dcRegionInternational.UseVisualStyleBackColor = true;
			// 
			// dcRegionUS
			// 
			this.dcRegionUS.AutoSize = true;
			this.dcRegionUS.Checked = true;
			this.dcRegionUS.Location = new System.Drawing.Point(66, 19);
			this.dcRegionUS.Name = "dcRegionUS";
			this.dcRegionUS.Size = new System.Drawing.Size(79, 17);
			this.dcRegionUS.TabIndex = 1;
			this.dcRegionUS.TabStop = true;
			this.dcRegionUS.Text = "&US/Europe";
			this.dcRegionUS.UseVisualStyleBackColor = true;
			// 
			// dcRegionJapan
			// 
			this.dcRegionJapan.AutoSize = true;
			this.dcRegionJapan.Location = new System.Drawing.Point(6, 19);
			this.dcRegionJapan.Name = "dcRegionJapan";
			this.dcRegionJapan.Size = new System.Drawing.Size(54, 17);
			this.dcRegionJapan.TabIndex = 0;
			this.dcRegionJapan.Text = "&Japan";
			this.dcRegionJapan.UseVisualStyleBackColor = true;
			// 
			// gcRegions
			// 
			this.gcRegions.AutoSize = true;
			this.gcRegions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.gcRegions.Controls.Add(this.gcRegionEurope);
			this.gcRegions.Controls.Add(this.gcRegionUS);
			this.gcRegions.Controls.Add(this.gcRegionJapan);
			this.gcRegions.Location = new System.Drawing.Point(12, 70);
			this.gcRegions.Name = "gcRegions";
			this.gcRegions.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.gcRegions.Size = new System.Drawing.Size(177, 52);
			this.gcRegions.TabIndex = 5;
			this.gcRegions.TabStop = false;
			this.gcRegions.Text = "Region";
			this.gcRegions.Visible = false;
			// 
			// gcRegionEurope
			// 
			this.gcRegionEurope.AutoSize = true;
			this.gcRegionEurope.Location = new System.Drawing.Point(112, 19);
			this.gcRegionEurope.Name = "gcRegionEurope";
			this.gcRegionEurope.Size = new System.Drawing.Size(59, 17);
			this.gcRegionEurope.TabIndex = 2;
			this.gcRegionEurope.Text = "&Europe";
			this.gcRegionEurope.UseVisualStyleBackColor = true;
			// 
			// gcRegionUS
			// 
			this.gcRegionUS.AutoSize = true;
			this.gcRegionUS.Checked = true;
			this.gcRegionUS.Location = new System.Drawing.Point(66, 19);
			this.gcRegionUS.Name = "gcRegionUS";
			this.gcRegionUS.Size = new System.Drawing.Size(40, 17);
			this.gcRegionUS.TabIndex = 1;
			this.gcRegionUS.TabStop = true;
			this.gcRegionUS.Text = "&US";
			this.gcRegionUS.UseVisualStyleBackColor = true;
			// 
			// gcRegionJapan
			// 
			this.gcRegionJapan.AutoSize = true;
			this.gcRegionJapan.Location = new System.Drawing.Point(6, 19);
			this.gcRegionJapan.Name = "gcRegionJapan";
			this.gcRegionJapan.Size = new System.Drawing.Size(54, 17);
			this.gcRegionJapan.TabIndex = 0;
			this.gcRegionJapan.Text = "&Japan";
			this.gcRegionJapan.UseVisualStyleBackColor = true;
			// 
			// systemGameCubePreview
			// 
			this.systemGameCubePreview.AutoSize = true;
			this.systemGameCubePreview.Location = new System.Drawing.Point(355, 19);
			this.systemGameCubePreview.Name = "systemGameCubePreview";
			this.systemGameCubePreview.Size = new System.Drawing.Size(119, 17);
			this.systemGameCubePreview.TabIndex = 5;
			this.systemGameCubePreview.TabStop = true;
			this.systemGameCubePreview.Text = "Game&Cube Preview";
			this.systemGameCubePreview.UseVisualStyleBackColor = true;
			this.systemGameCubePreview.CheckedChanged += new System.EventHandler(this.system_CheckedChanged);
			// 
			// SaveFormatDialog
			// 
			this.AcceptButton = this.button2;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(505, 134);
			this.Controls.Add(this.gcRegions);
			this.Controls.Add(this.dcRegions);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SaveFormatDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Save Format";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.dcRegions.ResumeLayout(false);
			this.dcRegions.PerformLayout();
			this.gcRegions.ResumeLayout(false);
			this.gcRegions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton systemPC;
        private System.Windows.Forms.RadioButton systemGamecube;
        private System.Windows.Forms.RadioButton systemDreamcast;
        private System.Windows.Forms.GroupBox dcRegions;
        private System.Windows.Forms.RadioButton dcRegionInternational;
        private System.Windows.Forms.RadioButton dcRegionUS;
        private System.Windows.Forms.RadioButton dcRegionJapan;
        private System.Windows.Forms.GroupBox gcRegions;
        private System.Windows.Forms.RadioButton gcRegionEurope;
        private System.Windows.Forms.RadioButton gcRegionUS;
        private System.Windows.Forms.RadioButton gcRegionJapan;
        private System.Windows.Forms.RadioButton systemSteam;
        private System.Windows.Forms.RadioButton system360;
		private System.Windows.Forms.RadioButton systemGameCubePreview;
    }
}