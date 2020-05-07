namespace SA2MessageFileEditor
{
	partial class MainForm
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
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.ToolStripMenuItem emptyToolStripMenuItem;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
			System.Windows.Forms.Label label1;
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.encodingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.shiftJISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.windows1252ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bigEndianGCSteamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lineEdit = new System.Windows.Forms.RichTextBox();
			this.messageRemoveButton = new System.Windows.Forms.Button();
			this.messageAddButton = new System.Windows.Forms.Button();
			this.messageNum = new System.Windows.Forms.ComboBox();
			this.linePanel = new System.Windows.Forms.Panel();
			this.waitTimeSelector = new SA2MessageFileEditor.TimeControl();
			this.waitTimeCheckBox = new System.Windows.Forms.CheckBox();
			this.voiceSelector = new System.Windows.Forms.NumericUpDown();
			this.playVoiceCheckBox = new System.Windows.Forms.CheckBox();
			this.audioSelector = new System.Windows.Forms.NumericUpDown();
			this.playAudioCheckBox = new System.Windows.Forms.CheckBox();
			this.lineRemoveButton = new System.Windows.Forms.Button();
			this.lineAddButton = new System.Windows.Forms.Button();
			this.lineNum = new System.Windows.Forms.ComboBox();
			this.messagePanel = new System.Windows.Forms.Panel();
			this.messageCentered = new System.Windows.Forms.CheckBox();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			label4 = new System.Windows.Forms.Label();
			emptyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			label1 = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.linePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.voiceSelector)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.audioSelector)).BeginInit();
			this.messagePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(152, 6);
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(152, 6);
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(12, 32);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(53, 13);
			label4.TabIndex = 13;
			label4.Text = "Message:";
			// 
			// emptyToolStripMenuItem
			// 
			emptyToolStripMenuItem.Enabled = false;
			emptyToolStripMenuItem.Name = "emptyToolStripMenuItem";
			emptyToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			emptyToolStripMenuItem.Text = "(empty)";
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(152, 6);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 8);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(30, 13);
			label1.TabIndex = 22;
			label1.Text = "Line:";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(334, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            toolStripSeparator2,
            this.exportToolStripMenuItem,
            toolStripSeparator4,
            this.recentFilesToolStripMenuItem,
            toolStripSeparator3,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.newToolStripMenuItem.Text = "&New";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.saveAsToolStripMenuItem.Text = "Save &As";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.exportToolStripMenuItem.Text = "&Export...";
			this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
			// 
			// recentFilesToolStripMenuItem
			// 
			this.recentFilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            emptyToolStripMenuItem});
			this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
			this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.recentFilesToolStripMenuItem.Text = "&Recent Files";
			this.recentFilesToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recentFilesToolStripMenuItem_DropDownItemClicked);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt+F4";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.encodingToolStripMenuItem,
            this.bigEndianGCSteamToolStripMenuItem,
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// encodingToolStripMenuItem
			// 
			this.encodingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shiftJISToolStripMenuItem,
            this.windows1252ToolStripMenuItem});
			this.encodingToolStripMenuItem.Name = "encodingToolStripMenuItem";
			this.encodingToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.encodingToolStripMenuItem.Text = "&Encoding";
			// 
			// shiftJISToolStripMenuItem
			// 
			this.shiftJISToolStripMenuItem.Name = "shiftJISToolStripMenuItem";
			this.shiftJISToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.shiftJISToolStripMenuItem.Text = "&Shift-JIS";
			this.shiftJISToolStripMenuItem.Click += new System.EventHandler(this.shiftJISToolStripMenuItem_Click);
			// 
			// windows1252ToolStripMenuItem
			// 
			this.windows1252ToolStripMenuItem.Checked = true;
			this.windows1252ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.windows1252ToolStripMenuItem.Name = "windows1252ToolStripMenuItem";
			this.windows1252ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.windows1252ToolStripMenuItem.Text = "&Windows-1252";
			this.windows1252ToolStripMenuItem.Click += new System.EventHandler(this.windows1252ToolStripMenuItem_Click);
			// 
			// bigEndianGCSteamToolStripMenuItem
			// 
			this.bigEndianGCSteamToolStripMenuItem.CheckOnClick = true;
			this.bigEndianGCSteamToolStripMenuItem.Name = "bigEndianGCSteamToolStripMenuItem";
			this.bigEndianGCSteamToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.bigEndianGCSteamToolStripMenuItem.Text = "&Big Endian (GC/Steam)";
			this.bigEndianGCSteamToolStripMenuItem.Click += new System.EventHandler(this.bigEndianGCSteamToolStripMenuItem_Click);
			// 
			// findToolStripMenuItem
			// 
			this.findToolStripMenuItem.Name = "findToolStripMenuItem";
			this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.findToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.findToolStripMenuItem.Text = "&Find...";
			this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
			// 
			// findNextToolStripMenuItem
			// 
			this.findNextToolStripMenuItem.Enabled = false;
			this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
			this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.findNextToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.findNextToolStripMenuItem.Text = "Find &Next";
			this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
			// 
			// lineEdit
			// 
			this.lineEdit.AcceptsTab = true;
			this.lineEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lineEdit.DetectUrls = false;
			this.lineEdit.Location = new System.Drawing.Point(12, 112);
			this.lineEdit.Name = "lineEdit";
			this.lineEdit.Size = new System.Drawing.Size(310, 99);
			this.lineEdit.TabIndex = 15;
			this.lineEdit.Text = "";
			this.lineEdit.TextChanged += new System.EventHandler(this.lineEdit_TextChanged);
			// 
			// messageRemoveButton
			// 
			this.messageRemoveButton.AutoSize = true;
			this.messageRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.messageRemoveButton.Enabled = false;
			this.messageRemoveButton.Location = new System.Drawing.Point(265, 27);
			this.messageRemoveButton.Name = "messageRemoveButton";
			this.messageRemoveButton.Size = new System.Drawing.Size(57, 23);
			this.messageRemoveButton.TabIndex = 17;
			this.messageRemoveButton.Text = "Remove";
			this.messageRemoveButton.UseVisualStyleBackColor = true;
			this.messageRemoveButton.Click += new System.EventHandler(this.messageRemoveButton_Click);
			// 
			// messageAddButton
			// 
			this.messageAddButton.AutoSize = true;
			this.messageAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.messageAddButton.Location = new System.Drawing.Point(223, 27);
			this.messageAddButton.Name = "messageAddButton";
			this.messageAddButton.Size = new System.Drawing.Size(36, 23);
			this.messageAddButton.TabIndex = 16;
			this.messageAddButton.Text = "Add";
			this.messageAddButton.UseVisualStyleBackColor = true;
			this.messageAddButton.Click += new System.EventHandler(this.messageAddButton_Click);
			// 
			// messageNum
			// 
			this.messageNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.messageNum.FormattingEnabled = true;
			this.messageNum.Location = new System.Drawing.Point(71, 29);
			this.messageNum.Name = "messageNum";
			this.messageNum.Size = new System.Drawing.Size(146, 21);
			this.messageNum.TabIndex = 14;
			this.messageNum.SelectedIndexChanged += new System.EventHandler(this.messageNum_SelectedIndexChanged);
			// 
			// linePanel
			// 
			this.linePanel.Controls.Add(this.messageCentered);
			this.linePanel.Controls.Add(this.waitTimeSelector);
			this.linePanel.Controls.Add(this.waitTimeCheckBox);
			this.linePanel.Controls.Add(this.voiceSelector);
			this.linePanel.Controls.Add(this.playVoiceCheckBox);
			this.linePanel.Controls.Add(this.audioSelector);
			this.linePanel.Controls.Add(this.playAudioCheckBox);
			this.linePanel.Controls.Add(this.lineEdit);
			this.linePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.linePanel.Enabled = false;
			this.linePanel.Location = new System.Drawing.Point(0, 32);
			this.linePanel.Name = "linePanel";
			this.linePanel.Size = new System.Drawing.Size(334, 223);
			this.linePanel.TabIndex = 18;
			// 
			// waitTimeSelector
			// 
			this.waitTimeSelector.AutoSize = true;
			this.waitTimeSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.waitTimeSelector.Centiseconds = 0;
			this.waitTimeSelector.Enabled = false;
			this.waitTimeSelector.Frames = ((uint)(0u));
			this.waitTimeSelector.Location = new System.Drawing.Point(95, 55);
			this.waitTimeSelector.Minutes = 0;
			this.waitTimeSelector.Name = "waitTimeSelector";
			this.waitTimeSelector.Seconds = 0;
			this.waitTimeSelector.Size = new System.Drawing.Size(143, 26);
			this.waitTimeSelector.TabIndex = 21;
			this.waitTimeSelector.TimeSpan = System.TimeSpan.Parse("00:00:00");
			this.waitTimeSelector.TotalCentiseconds = ((uint)(0u));
			this.waitTimeSelector.TotalFrames = 0;
			this.waitTimeSelector.ValueChanged += new System.EventHandler(this.waitTimeSelector_ValueChanged);
			// 
			// waitTimeCheckBox
			// 
			this.waitTimeCheckBox.AutoSize = true;
			this.waitTimeCheckBox.Location = new System.Drawing.Point(12, 59);
			this.waitTimeCheckBox.Name = "waitTimeCheckBox";
			this.waitTimeCheckBox.Size = new System.Drawing.Size(77, 17);
			this.waitTimeCheckBox.TabIndex = 20;
			this.waitTimeCheckBox.Text = "Wait Time:";
			this.waitTimeCheckBox.UseVisualStyleBackColor = true;
			this.waitTimeCheckBox.CheckedChanged += new System.EventHandler(this.waitTimeCheckBox_CheckedChanged);
			// 
			// voiceSelector
			// 
			this.voiceSelector.Enabled = false;
			this.voiceSelector.Location = new System.Drawing.Point(97, 29);
			this.voiceSelector.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.voiceSelector.Name = "voiceSelector";
			this.voiceSelector.Size = new System.Drawing.Size(61, 20);
			this.voiceSelector.TabIndex = 19;
			this.voiceSelector.ValueChanged += new System.EventHandler(this.voiceSelector_ValueChanged);
			// 
			// playVoiceCheckBox
			// 
			this.playVoiceCheckBox.AutoSize = true;
			this.playVoiceCheckBox.Location = new System.Drawing.Point(12, 29);
			this.playVoiceCheckBox.Name = "playVoiceCheckBox";
			this.playVoiceCheckBox.Size = new System.Drawing.Size(79, 17);
			this.playVoiceCheckBox.TabIndex = 18;
			this.playVoiceCheckBox.Text = "Play Voice:";
			this.playVoiceCheckBox.UseVisualStyleBackColor = true;
			this.playVoiceCheckBox.CheckedChanged += new System.EventHandler(this.playVoiceCheckBox_CheckedChanged);
			// 
			// audioSelector
			// 
			this.audioSelector.Enabled = false;
			this.audioSelector.Location = new System.Drawing.Point(97, 3);
			this.audioSelector.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.audioSelector.Name = "audioSelector";
			this.audioSelector.Size = new System.Drawing.Size(61, 20);
			this.audioSelector.TabIndex = 17;
			this.audioSelector.ValueChanged += new System.EventHandler(this.audioSelector_ValueChanged);
			// 
			// playAudioCheckBox
			// 
			this.playAudioCheckBox.AutoSize = true;
			this.playAudioCheckBox.Location = new System.Drawing.Point(12, 3);
			this.playAudioCheckBox.Name = "playAudioCheckBox";
			this.playAudioCheckBox.Size = new System.Drawing.Size(79, 17);
			this.playAudioCheckBox.TabIndex = 16;
			this.playAudioCheckBox.Text = "Play Audio:";
			this.playAudioCheckBox.UseVisualStyleBackColor = true;
			this.playAudioCheckBox.CheckedChanged += new System.EventHandler(this.playAudioCheckBox_CheckedChanged);
			// 
			// lineRemoveButton
			// 
			this.lineRemoveButton.AutoSize = true;
			this.lineRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.lineRemoveButton.Enabled = false;
			this.lineRemoveButton.Location = new System.Drawing.Point(265, 3);
			this.lineRemoveButton.Name = "lineRemoveButton";
			this.lineRemoveButton.Size = new System.Drawing.Size(57, 23);
			this.lineRemoveButton.TabIndex = 25;
			this.lineRemoveButton.Text = "Remove";
			this.lineRemoveButton.UseVisualStyleBackColor = true;
			this.lineRemoveButton.Click += new System.EventHandler(this.lineRemoveButton_Click);
			// 
			// lineAddButton
			// 
			this.lineAddButton.AutoSize = true;
			this.lineAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.lineAddButton.Location = new System.Drawing.Point(223, 3);
			this.lineAddButton.Name = "lineAddButton";
			this.lineAddButton.Size = new System.Drawing.Size(36, 23);
			this.lineAddButton.TabIndex = 24;
			this.lineAddButton.Text = "Add";
			this.lineAddButton.UseVisualStyleBackColor = true;
			this.lineAddButton.Click += new System.EventHandler(this.lineAddButton_Click);
			// 
			// lineNum
			// 
			this.lineNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.lineNum.FormattingEnabled = true;
			this.lineNum.Location = new System.Drawing.Point(48, 5);
			this.lineNum.Name = "lineNum";
			this.lineNum.Size = new System.Drawing.Size(169, 21);
			this.lineNum.TabIndex = 23;
			this.lineNum.SelectedIndexChanged += new System.EventHandler(this.lineNum_SelectedIndexChanged);
			// 
			// messagePanel
			// 
			this.messagePanel.Controls.Add(this.lineRemoveButton);
			this.messagePanel.Controls.Add(this.linePanel);
			this.messagePanel.Controls.Add(this.lineAddButton);
			this.messagePanel.Controls.Add(label1);
			this.messagePanel.Controls.Add(this.lineNum);
			this.messagePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.messagePanel.Enabled = false;
			this.messagePanel.Location = new System.Drawing.Point(0, 56);
			this.messagePanel.Name = "messagePanel";
			this.messagePanel.Size = new System.Drawing.Size(334, 255);
			this.messagePanel.TabIndex = 19;
			// 
			// messageCentered
			// 
			this.messageCentered.AutoSize = true;
			this.messageCentered.Location = new System.Drawing.Point(12, 89);
			this.messageCentered.Name = "messageCentered";
			this.messageCentered.Size = new System.Drawing.Size(69, 17);
			this.messageCentered.TabIndex = 22;
			this.messageCentered.Text = "Centered";
			this.messageCentered.UseVisualStyleBackColor = true;
			this.messageCentered.CheckedChanged += new System.EventHandler(this.messageCentered_CheckedChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(334, 311);
			this.Controls.Add(this.messagePanel);
			this.Controls.Add(this.messageRemoveButton);
			this.Controls.Add(this.messageAddButton);
			this.Controls.Add(this.messageNum);
			this.Controls.Add(label4);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "SA2 Message File Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.linePanel.ResumeLayout(false);
			this.linePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.voiceSelector)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.audioSelector)).EndInit();
			this.messagePanel.ResumeLayout(false);
			this.messagePanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recentFilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.RichTextBox lineEdit;
		private System.Windows.Forms.Button messageRemoveButton;
		private System.Windows.Forms.Button messageAddButton;
		private System.Windows.Forms.ComboBox messageNum;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem encodingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem shiftJISToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windows1252ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bigEndianGCSteamToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.Panel linePanel;
		private System.Windows.Forms.CheckBox playAudioCheckBox;
		private System.Windows.Forms.NumericUpDown audioSelector;
		private System.Windows.Forms.NumericUpDown voiceSelector;
		private System.Windows.Forms.CheckBox playVoiceCheckBox;
		private System.Windows.Forms.CheckBox waitTimeCheckBox;
		private TimeControl waitTimeSelector;
		private System.Windows.Forms.Button lineRemoveButton;
		private System.Windows.Forms.Button lineAddButton;
		private System.Windows.Forms.ComboBox lineNum;
		private System.Windows.Forms.Panel messagePanel;
		private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
		private System.Windows.Forms.CheckBox messageCentered;
	}
}

