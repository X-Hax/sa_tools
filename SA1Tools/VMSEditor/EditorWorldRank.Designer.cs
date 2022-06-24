namespace VMSEditor
{
	partial class EditorWorldRank
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorWorldRank));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelPlayTime = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButtonInternational = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButtonJapan = new System.Windows.Forms.RadioButton();
            this.textBoxSubmitted = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxIndividualID = new System.Windows.Forms.TextBox();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.worldRankingsConverterHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.issueTrackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(278, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::VMSEditor.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = global::VMSEditor.Properties.Resources.saveas;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.ToolTipText = "Save World Ranking data as a regular SA1 game save file.";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VMSEditor.Properties.Resources.editorRankings;
            this.pictureBox1.Location = new System.Drawing.Point(19, 167);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // labelPlayTime
            // 
            this.labelPlayTime.AutoSize = true;
            this.labelPlayTime.Location = new System.Drawing.Point(11, 143);
            this.labelPlayTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPlayTime.Name = "labelPlayTime";
            this.labelPlayTime.Size = new System.Drawing.Size(64, 15);
            this.labelPlayTime.TabIndex = 2;
            this.labelPlayTime.Text = "Total Time:";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(203, 174);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(64, 25);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.radioButtonInternational);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.radioButtonJapan);
            this.groupBox1.Controls.Add(this.textBoxSubmitted);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxIndividualID);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(254, 106);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Info";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 24);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "Submitted by:";
            // 
            // radioButtonInternational
            // 
            this.radioButtonInternational.AutoSize = true;
            this.radioButtonInternational.Location = new System.Drawing.Point(152, 75);
            this.radioButtonInternational.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonInternational.Name = "radioButtonInternational";
            this.radioButtonInternational.Size = new System.Drawing.Size(39, 19);
            this.radioButtonInternational.TabIndex = 3;
            this.radioButtonInternational.TabStop = true;
            this.radioButtonInternational.Text = "US";
            this.radioButtonInternational.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 51);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Individual ID:";
            // 
            // radioButtonJapan
            // 
            this.radioButtonJapan.AutoSize = true;
            this.radioButtonJapan.Location = new System.Drawing.Point(90, 75);
            this.radioButtonJapan.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonJapan.Name = "radioButtonJapan";
            this.radioButtonJapan.Size = new System.Drawing.Size(55, 19);
            this.radioButtonJapan.TabIndex = 2;
            this.radioButtonJapan.TabStop = true;
            this.radioButtonJapan.Text = "Japan";
            this.radioButtonJapan.UseVisualStyleBackColor = true;
            // 
            // textBoxSubmitted
            // 
            this.textBoxSubmitted.Location = new System.Drawing.Point(90, 21);
            this.textBoxSubmitted.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSubmitted.Name = "textBoxSubmitted";
            this.textBoxSubmitted.Size = new System.Drawing.Size(154, 23);
            this.textBoxSubmitted.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 77);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 15);
            this.label7.TabIndex = 15;
            this.label7.Text = "Region:";
            // 
            // textBoxIndividualID
            // 
            this.textBoxIndividualID.Location = new System.Drawing.Point(90, 48);
            this.textBoxIndividualID.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxIndividualID.Name = "textBoxIndividualID";
            this.textBoxIndividualID.Size = new System.Drawing.Size(154, 23);
            this.textBoxIndividualID.TabIndex = 1;
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.worldRankingsConverterHelpToolStripMenuItem,
            this.issueTrackerToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // worldRankingsConverterHelpToolStripMenuItem
            // 
            this.worldRankingsConverterHelpToolStripMenuItem.Image = global::VMSEditor.Properties.Resources.help;
            this.worldRankingsConverterHelpToolStripMenuItem.Name = "worldRankingsConverterHelpToolStripMenuItem";
            this.worldRankingsConverterHelpToolStripMenuItem.Size = new System.Drawing.Size(248, 30);
            this.worldRankingsConverterHelpToolStripMenuItem.Text = "World Rankings Converter Help";
            this.worldRankingsConverterHelpToolStripMenuItem.Click += new System.EventHandler(this.worldRankingsConverterHelpToolStripMenuItem_Click);
            // 
            // issueTrackerToolStripMenuItem
            // 
            this.issueTrackerToolStripMenuItem.Image = global::VMSEditor.Properties.Resources.bug;
            this.issueTrackerToolStripMenuItem.Name = "issueTrackerToolStripMenuItem";
            this.issueTrackerToolStripMenuItem.Size = new System.Drawing.Size(248, 30);
            this.issueTrackerToolStripMenuItem.Text = "Issue Tracker";
            this.issueTrackerToolStripMenuItem.Click += new System.EventHandler(this.issueTrackerToolStripMenuItem_Click);
            // 
            // EditorWorldRank
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(278, 210);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelPlayTime);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "EditorWorldRank";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rankings Converter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorWorldRank_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelPlayTime;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioButtonInternational;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.RadioButton radioButtonJapan;
		private System.Windows.Forms.TextBox textBoxSubmitted;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBoxIndividualID;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem worldRankingsConverterHelpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem issueTrackerToolStripMenuItem;
	}
}