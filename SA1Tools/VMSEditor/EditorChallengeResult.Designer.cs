namespace VMSEditor
{
	partial class EditorChallengeResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorChallengeResult));
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxCharacter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownEventID = new System.Windows.Forms.NumericUpDown();
            this.buttonClose = new System.Windows.Forms.Button();
            this.numericUpDownFrames = new System.Windows.Forms.NumericUpDown();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.radioButtonCart = new System.Windows.Forms.RadioButton();
            this.radioButtonEvent = new System.Windows.Forms.RadioButton();
            this.pictureBoxDataType = new System.Windows.Forms.PictureBox();
            this.textBoxIndividualID = new System.Windows.Forms.TextBox();
            this.textBoxSubmitted = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButtonInternational = new System.Windows.Forms.RadioButton();
            this.radioButtonJapan = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.challengeResultViewerHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.issueTrackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEventID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrames)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDataType)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 210);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Result (Frames):";
            // 
            // comboBoxCharacter
            // 
            this.comboBoxCharacter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCharacter.FormattingEnabled = true;
            this.comboBoxCharacter.Items.AddRange(new object[] {
            "Sonic",
            "Tails",
            "Knuckles",
            "Amy",
            "Gamma",
            "Big"});
            this.comboBoxCharacter.Location = new System.Drawing.Point(160, 227);
            this.comboBoxCharacter.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxCharacter.Name = "comboBoxCharacter";
            this.comboBoxCharacter.Size = new System.Drawing.Size(106, 23);
            this.comboBoxCharacter.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 210);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Character:";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(11, 143);
            this.labelTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(67, 15);
            this.labelTime.TabIndex = 4;
            this.labelTime.Text = "Total Time: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 252);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "Event ID:";
            // 
            // numericUpDownEventID
            // 
            this.numericUpDownEventID.Location = new System.Drawing.Point(11, 269);
            this.numericUpDownEventID.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownEventID.Maximum = new decimal(new int[] {
            1569325055,
            23283064,
            0,
            0});
            this.numericUpDownEventID.Name = "numericUpDownEventID";
            this.numericUpDownEventID.Size = new System.Drawing.Size(105, 23);
            this.numericUpDownEventID.TabIndex = 5;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(202, 269);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(64, 25);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // numericUpDownFrames
            // 
            this.numericUpDownFrames.Location = new System.Drawing.Point(11, 227);
            this.numericUpDownFrames.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownFrames.Maximum = new decimal(new int[] {
            1569325055,
            23283064,
            0,
            0});
            this.numericUpDownFrames.Name = "numericUpDownFrames";
            this.numericUpDownFrames.Size = new System.Drawing.Size(105, 23);
            this.numericUpDownFrames.TabIndex = 4;
            this.numericUpDownFrames.ValueChanged += new System.EventHandler(this.numericUpDownFrames_ValueChanged);
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
            this.menuStrip1.Size = new System.Drawing.Size(277, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::VMSEditor.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // radioButtonCart
            // 
            this.radioButtonCart.AutoSize = true;
            this.radioButtonCart.Location = new System.Drawing.Point(72, 172);
            this.radioButtonCart.Name = "radioButtonCart";
            this.radioButtonCart.Size = new System.Drawing.Size(82, 19);
            this.radioButtonCart.TabIndex = 2;
            this.radioButtonCart.Text = "Cart Result";
            this.radioButtonCart.UseVisualStyleBackColor = true;
            this.radioButtonCart.CheckedChanged += new System.EventHandler(this.radioButtonCart_CheckedChanged);
            // 
            // radioButtonEvent
            // 
            this.radioButtonEvent.AutoSize = true;
            this.radioButtonEvent.Location = new System.Drawing.Point(160, 172);
            this.radioButtonEvent.Name = "radioButtonEvent";
            this.radioButtonEvent.Size = new System.Drawing.Size(113, 19);
            this.radioButtonEvent.TabIndex = 3;
            this.radioButtonEvent.Text = "Challenge Result";
            this.radioButtonEvent.UseVisualStyleBackColor = true;
            // 
            // pictureBoxDataType
            // 
            this.pictureBoxDataType.Image = global::VMSEditor.Properties.Resources.eventresult;
            this.pictureBoxDataType.Location = new System.Drawing.Point(19, 167);
            this.pictureBoxDataType.Name = "pictureBoxDataType";
            this.pictureBoxDataType.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxDataType.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxDataType.TabIndex = 8;
            this.pictureBoxDataType.TabStop = false;
            // 
            // textBoxIndividualID
            // 
            this.textBoxIndividualID.Location = new System.Drawing.Point(90, 48);
            this.textBoxIndividualID.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxIndividualID.Name = "textBoxIndividualID";
            this.textBoxIndividualID.Size = new System.Drawing.Size(154, 23);
            this.textBoxIndividualID.TabIndex = 1;
            // 
            // textBoxSubmitted
            // 
            this.textBoxSubmitted.Location = new System.Drawing.Point(90, 21);
            this.textBoxSubmitted.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSubmitted.Name = "textBoxSubmitted";
            this.textBoxSubmitted.Size = new System.Drawing.Size(154, 23);
            this.textBoxSubmitted.TabIndex = 0;
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
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.challengeResultViewerHelpToolStripMenuItem,
            this.issueTrackerToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // challengeResultViewerHelpToolStripMenuItem
            // 
            this.challengeResultViewerHelpToolStripMenuItem.Image = global::VMSEditor.Properties.Resources.help;
            this.challengeResultViewerHelpToolStripMenuItem.Name = "challengeResultViewerHelpToolStripMenuItem";
            this.challengeResultViewerHelpToolStripMenuItem.Size = new System.Drawing.Size(236, 30);
            this.challengeResultViewerHelpToolStripMenuItem.Text = "Challenge Result Viewer Help";
            this.challengeResultViewerHelpToolStripMenuItem.Click += new System.EventHandler(this.challengeResultViewerHelpToolStripMenuItem_Click);
            // 
            // issueTrackerToolStripMenuItem
            // 
            this.issueTrackerToolStripMenuItem.Image = global::VMSEditor.Properties.Resources.bug;
            this.issueTrackerToolStripMenuItem.Name = "issueTrackerToolStripMenuItem";
            this.issueTrackerToolStripMenuItem.Size = new System.Drawing.Size(236, 30);
            this.issueTrackerToolStripMenuItem.Text = "Issue Tracker";
            this.issueTrackerToolStripMenuItem.Click += new System.EventHandler(this.issueTrackerToolStripMenuItem_Click);
            // 
            // EditorChallengeResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(277, 304);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBoxDataType);
            this.Controls.Add(this.radioButtonEvent);
            this.Controls.Add(this.radioButtonCart);
            this.Controls.Add(this.numericUpDownFrames);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.numericUpDownEventID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxCharacter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "EditorChallengeResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Result Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorChallengeResult_FormClosing);
            this.Load += new System.EventHandler(this.EditorChallengeResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEventID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrames)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDataType)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxCharacter;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownEventID;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.NumericUpDown numericUpDownFrames;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.RadioButton radioButtonCart;
		private System.Windows.Forms.RadioButton radioButtonEvent;
		private System.Windows.Forms.PictureBox pictureBoxDataType;
		private System.Windows.Forms.TextBox textBoxIndividualID;
		private System.Windows.Forms.TextBox textBoxSubmitted;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioButtonInternational;
		private System.Windows.Forms.RadioButton radioButtonJapan;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem challengeResultViewerHelpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem issueTrackerToolStripMenuItem;
	}
}