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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			menuStrip1 = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			encodingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			autoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			shiftJISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			windows1252ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			bigEndianGCSteamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			textOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			lineEdit = new System.Windows.Forms.RichTextBox();
			messageRemoveButton = new System.Windows.Forms.Button();
			messageAddButton = new System.Windows.Forms.Button();
			messageNum = new System.Windows.Forms.ComboBox();
			linePanel = new System.Windows.Forms.Panel();
			messageEmerald2P = new System.Windows.Forms.CheckBox();
			messageCentered = new System.Windows.Forms.CheckBox();
			waitTimeSelector = new TimeControl();
			waitTimeCheckBox = new System.Windows.Forms.CheckBox();
			voiceSelector = new System.Windows.Forms.NumericUpDown();
			playVoiceCheckBox = new System.Windows.Forms.CheckBox();
			audioSelector = new System.Windows.Forms.NumericUpDown();
			playAudioCheckBox = new System.Windows.Forms.CheckBox();
			lineRemoveButton = new System.Windows.Forms.Button();
			lineAddButton = new System.Windows.Forms.Button();
			lineNum = new System.Windows.Forms.ComboBox();
			messagePanel = new System.Windows.Forms.Panel();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			label4 = new System.Windows.Forms.Label();
			emptyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			label1 = new System.Windows.Forms.Label();
			menuStrip1.SuspendLayout();
			linePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)voiceSelector).BeginInit();
			((System.ComponentModel.ISupportInitialize)audioSelector).BeginInit();
			messagePanel.SuspendLayout();
			SuspendLayout();
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
			label4.Location = new System.Drawing.Point(14, 37);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(56, 15);
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
			label1.Location = new System.Drawing.Point(41, 9);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(32, 15);
			label1.TabIndex = 22;
			label1.Text = "Line:";
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem });
			menuStrip1.Location = new System.Drawing.Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
			menuStrip1.Size = new System.Drawing.Size(454, 24);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripSeparator1, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator2, exportToolStripMenuItem, toolStripSeparator4, recentFilesToolStripMenuItem, toolStripSeparator3, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			newToolStripMenuItem.Name = "newToolStripMenuItem";
			newToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N;
			newToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			newToolStripMenuItem.Text = "&New";
			newToolStripMenuItem.Click += newToolStripMenuItem_Click;
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
			openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			openToolStripMenuItem.Text = "&Open...";
			openToolStripMenuItem.Click += openToolStripMenuItem_Click;
			// 
			// saveToolStripMenuItem
			// 
			saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			saveToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
			saveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			saveToolStripMenuItem.Text = "&Save";
			saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
			// 
			// saveAsToolStripMenuItem
			// 
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			saveAsToolStripMenuItem.Text = "Save &As";
			saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
			// 
			// exportToolStripMenuItem
			// 
			exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			exportToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			exportToolStripMenuItem.Text = "&Export...";
			exportToolStripMenuItem.Click += exportToolStripMenuItem_Click;
			// 
			// recentFilesToolStripMenuItem
			// 
			recentFilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { emptyToolStripMenuItem });
			recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
			recentFilesToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			recentFilesToolStripMenuItem.Text = "&Recent Files";
			recentFilesToolStripMenuItem.DropDownItemClicked += recentFilesToolStripMenuItem_DropDownItemClicked;
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt+F4";
			exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			exitToolStripMenuItem.Text = "E&xit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { encodingToolStripMenuItem, bigEndianGCSteamToolStripMenuItem, textOnlyToolStripMenuItem, findToolStripMenuItem, findNextToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			editToolStripMenuItem.Text = "&Edit";
			// 
			// encodingToolStripMenuItem
			// 
			encodingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { autoToolStripMenuItem, shiftJISToolStripMenuItem, windows1252ToolStripMenuItem, customToolStripMenuItem });
			encodingToolStripMenuItem.Name = "encodingToolStripMenuItem";
			encodingToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			encodingToolStripMenuItem.Text = "&Encoding";
			// 
			// autoToolStripMenuItem
			// 
			autoToolStripMenuItem.Name = "autoToolStripMenuItem";
			autoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			autoToolStripMenuItem.Text = "Auto";
			autoToolStripMenuItem.ToolTipText = "Try to guess character encoding from the filename. This will only apply for newly opened files.";
			autoToolStripMenuItem.Click += autoToolStripMenuItem_Click;
			// 
			// shiftJISToolStripMenuItem
			// 
			shiftJISToolStripMenuItem.Name = "shiftJISToolStripMenuItem";
			shiftJISToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			shiftJISToolStripMenuItem.Text = "&Shift-JIS";
			shiftJISToolStripMenuItem.ToolTipText = "Use Japanese encoding for parsing text.";
			shiftJISToolStripMenuItem.Click += shiftJISToolStripMenuItem_Click;
			// 
			// windows1252ToolStripMenuItem
			// 
			windows1252ToolStripMenuItem.Checked = true;
			windows1252ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			windows1252ToolStripMenuItem.Name = "windows1252ToolStripMenuItem";
			windows1252ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			windows1252ToolStripMenuItem.Text = "&Windows-1252";
			windows1252ToolStripMenuItem.ToolTipText = "Use Western European encoding for parsing text.";
			windows1252ToolStripMenuItem.Click += windows1252ToolStripMenuItem_Click;
			// 
			// customToolStripMenuItem
			// 
			customToolStripMenuItem.Name = "customToolStripMenuItem";
			customToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			customToolStripMenuItem.Text = "Custom...";
			customToolStripMenuItem.ToolTipText = "Set custom encoding for parsing text.";
			customToolStripMenuItem.Click += customToolStripMenuItem_Click;
			// 
			// bigEndianGCSteamToolStripMenuItem
			// 
			bigEndianGCSteamToolStripMenuItem.CheckOnClick = true;
			bigEndianGCSteamToolStripMenuItem.Name = "bigEndianGCSteamToolStripMenuItem";
			bigEndianGCSteamToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			bigEndianGCSteamToolStripMenuItem.Text = "&Big Endian (GC/Steam)";
			bigEndianGCSteamToolStripMenuItem.Click += bigEndianGCSteamToolStripMenuItem_Click;
			// 
			// textOnlyToolStripMenuItem
			// 
			textOnlyToolStripMenuItem.Checked = true;
			textOnlyToolStripMenuItem.CheckOnClick = true;
			textOnlyToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			textOnlyToolStripMenuItem.Name = "textOnlyToolStripMenuItem";
			textOnlyToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			textOnlyToolStripMenuItem.Text = "Text Only (Chao/SADX Files)";
			// 
			// findToolStripMenuItem
			// 
			findToolStripMenuItem.Name = "findToolStripMenuItem";
			findToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F;
			findToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			findToolStripMenuItem.Text = "&Find...";
			findToolStripMenuItem.Click += findToolStripMenuItem_Click;
			// 
			// findNextToolStripMenuItem
			// 
			findNextToolStripMenuItem.Enabled = false;
			findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
			findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			findNextToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			findNextToolStripMenuItem.Text = "Find &Next";
			findNextToolStripMenuItem.Click += findNextToolStripMenuItem_Click;
			// 
			// lineEdit
			// 
			lineEdit.AcceptsTab = true;
			lineEdit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			lineEdit.DetectUrls = false;
			lineEdit.Location = new System.Drawing.Point(14, 148);
			lineEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			lineEdit.Name = "lineEdit";
			lineEdit.Size = new System.Drawing.Size(422, 133);
			lineEdit.TabIndex = 15;
			lineEdit.Text = "";
			lineEdit.TextChanged += lineEdit_TextChanged;
			// 
			// messageRemoveButton
			// 
			messageRemoveButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			messageRemoveButton.AutoSize = true;
			messageRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			messageRemoveButton.Enabled = false;
			messageRemoveButton.Location = new System.Drawing.Point(379, 32);
			messageRemoveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			messageRemoveButton.Name = "messageRemoveButton";
			messageRemoveButton.Size = new System.Drawing.Size(60, 25);
			messageRemoveButton.TabIndex = 17;
			messageRemoveButton.Text = "Remove";
			messageRemoveButton.UseVisualStyleBackColor = true;
			messageRemoveButton.Click += messageRemoveButton_Click;
			// 
			// messageAddButton
			// 
			messageAddButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			messageAddButton.AutoSize = true;
			messageAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			messageAddButton.Location = new System.Drawing.Point(327, 32);
			messageAddButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			messageAddButton.Name = "messageAddButton";
			messageAddButton.Size = new System.Drawing.Size(39, 25);
			messageAddButton.TabIndex = 16;
			messageAddButton.Text = "Add";
			messageAddButton.UseVisualStyleBackColor = true;
			messageAddButton.Click += messageAddButton_Click;
			// 
			// messageNum
			// 
			messageNum.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			messageNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			messageNum.FormattingEnabled = true;
			messageNum.Location = new System.Drawing.Point(83, 33);
			messageNum.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			messageNum.Name = "messageNum";
			messageNum.Size = new System.Drawing.Size(234, 23);
			messageNum.TabIndex = 14;
			messageNum.SelectedIndexChanged += messageNum_SelectedIndexChanged;
			// 
			// linePanel
			// 
			linePanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			linePanel.Controls.Add(messageEmerald2P);
			linePanel.Controls.Add(messageCentered);
			linePanel.Controls.Add(waitTimeSelector);
			linePanel.Controls.Add(waitTimeCheckBox);
			linePanel.Controls.Add(voiceSelector);
			linePanel.Controls.Add(playVoiceCheckBox);
			linePanel.Controls.Add(audioSelector);
			linePanel.Controls.Add(playAudioCheckBox);
			linePanel.Controls.Add(lineEdit);
			linePanel.Enabled = false;
			linePanel.Location = new System.Drawing.Point(0, 37);
			linePanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			linePanel.Name = "linePanel";
			linePanel.Size = new System.Drawing.Size(450, 295);
			linePanel.TabIndex = 18;
			// 
			// messageEmerald2P
			// 
			messageEmerald2P.AutoSize = true;
			messageEmerald2P.Location = new System.Drawing.Point(14, 98);
			messageEmerald2P.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			messageEmerald2P.Name = "messageEmerald2P";
			messageEmerald2P.Size = new System.Drawing.Size(85, 19);
			messageEmerald2P.TabIndex = 23;
			messageEmerald2P.Text = "2P Emerald";
			messageEmerald2P.UseVisualStyleBackColor = true;
			messageEmerald2P.CheckedChanged += messageEmerald2P_CheckedChanged;
			// 
			// messageCentered
			// 
			messageCentered.AutoSize = true;
			messageCentered.Location = new System.Drawing.Point(14, 123);
			messageCentered.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			messageCentered.Name = "messageCentered";
			messageCentered.Size = new System.Drawing.Size(74, 19);
			messageCentered.TabIndex = 22;
			messageCentered.Text = "Centered";
			messageCentered.UseVisualStyleBackColor = true;
			messageCentered.CheckedChanged += messageCentered_CheckedChanged;
			// 
			// waitTimeSelector
			// 
			waitTimeSelector.AutoSize = true;
			waitTimeSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			waitTimeSelector.Centiseconds = 0;
			waitTimeSelector.Enabled = false;
			waitTimeSelector.Frames = 0U;
			waitTimeSelector.Location = new System.Drawing.Point(111, 63);
			waitTimeSelector.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			waitTimeSelector.Minutes = 0;
			waitTimeSelector.Name = "waitTimeSelector";
			waitTimeSelector.Seconds = 0;
			waitTimeSelector.Size = new System.Drawing.Size(168, 29);
			waitTimeSelector.TabIndex = 21;
			waitTimeSelector.TimeSpan = System.TimeSpan.Parse("00:00:00");
			waitTimeSelector.TotalCentiseconds = 0U;
			waitTimeSelector.TotalFrames = 0;
			waitTimeSelector.ValueChanged += waitTimeSelector_ValueChanged;
			// 
			// waitTimeCheckBox
			// 
			waitTimeCheckBox.AutoSize = true;
			waitTimeCheckBox.Location = new System.Drawing.Point(14, 68);
			waitTimeCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			waitTimeCheckBox.Name = "waitTimeCheckBox";
			waitTimeCheckBox.Size = new System.Drawing.Size(83, 19);
			waitTimeCheckBox.TabIndex = 20;
			waitTimeCheckBox.Text = "Wait Time:";
			waitTimeCheckBox.UseVisualStyleBackColor = true;
			waitTimeCheckBox.CheckedChanged += waitTimeCheckBox_CheckedChanged;
			// 
			// voiceSelector
			// 
			voiceSelector.Enabled = false;
			voiceSelector.Location = new System.Drawing.Point(113, 33);
			voiceSelector.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			voiceSelector.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
			voiceSelector.Name = "voiceSelector";
			voiceSelector.Size = new System.Drawing.Size(71, 23);
			voiceSelector.TabIndex = 19;
			voiceSelector.ValueChanged += voiceSelector_ValueChanged;
			// 
			// playVoiceCheckBox
			// 
			playVoiceCheckBox.AutoSize = true;
			playVoiceCheckBox.Location = new System.Drawing.Point(14, 33);
			playVoiceCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			playVoiceCheckBox.Name = "playVoiceCheckBox";
			playVoiceCheckBox.Size = new System.Drawing.Size(82, 19);
			playVoiceCheckBox.TabIndex = 18;
			playVoiceCheckBox.Text = "Play Voice:";
			playVoiceCheckBox.UseVisualStyleBackColor = true;
			playVoiceCheckBox.CheckedChanged += playVoiceCheckBox_CheckedChanged;
			// 
			// audioSelector
			// 
			audioSelector.Enabled = false;
			audioSelector.Location = new System.Drawing.Point(113, 3);
			audioSelector.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			audioSelector.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
			audioSelector.Name = "audioSelector";
			audioSelector.Size = new System.Drawing.Size(71, 23);
			audioSelector.TabIndex = 17;
			audioSelector.ValueChanged += audioSelector_ValueChanged;
			// 
			// playAudioCheckBox
			// 
			playAudioCheckBox.AutoSize = true;
			playAudioCheckBox.Location = new System.Drawing.Point(14, 3);
			playAudioCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			playAudioCheckBox.Name = "playAudioCheckBox";
			playAudioCheckBox.Size = new System.Drawing.Size(86, 19);
			playAudioCheckBox.TabIndex = 16;
			playAudioCheckBox.Text = "Play Audio:";
			playAudioCheckBox.UseVisualStyleBackColor = true;
			playAudioCheckBox.CheckedChanged += playAudioCheckBox_CheckedChanged;
			// 
			// lineRemoveButton
			// 
			lineRemoveButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			lineRemoveButton.AutoSize = true;
			lineRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			lineRemoveButton.Enabled = false;
			lineRemoveButton.Location = new System.Drawing.Point(380, 5);
			lineRemoveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			lineRemoveButton.Name = "lineRemoveButton";
			lineRemoveButton.Size = new System.Drawing.Size(60, 25);
			lineRemoveButton.TabIndex = 25;
			lineRemoveButton.Text = "Remove";
			lineRemoveButton.UseVisualStyleBackColor = true;
			lineRemoveButton.Click += lineRemoveButton_Click;
			// 
			// lineAddButton
			// 
			lineAddButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			lineAddButton.AutoSize = true;
			lineAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			lineAddButton.Location = new System.Drawing.Point(328, 5);
			lineAddButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			lineAddButton.Name = "lineAddButton";
			lineAddButton.Size = new System.Drawing.Size(39, 25);
			lineAddButton.TabIndex = 24;
			lineAddButton.Text = "Add";
			lineAddButton.UseVisualStyleBackColor = true;
			lineAddButton.Click += lineAddButton_Click;
			// 
			// lineNum
			// 
			lineNum.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			lineNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			lineNum.FormattingEnabled = true;
			lineNum.Location = new System.Drawing.Point(83, 6);
			lineNum.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			lineNum.Name = "lineNum";
			lineNum.Size = new System.Drawing.Size(234, 23);
			lineNum.TabIndex = 23;
			lineNum.SelectedIndexChanged += lineNum_SelectedIndexChanged;
			// 
			// messagePanel
			// 
			messagePanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			messagePanel.Controls.Add(lineRemoveButton);
			messagePanel.Controls.Add(linePanel);
			messagePanel.Controls.Add(lineAddButton);
			messagePanel.Controls.Add(label1);
			messagePanel.Controls.Add(lineNum);
			messagePanel.Enabled = false;
			messagePanel.Location = new System.Drawing.Point(0, 65);
			messagePanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			messagePanel.Name = "messagePanel";
			messagePanel.Size = new System.Drawing.Size(454, 332);
			messagePanel.TabIndex = 19;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(454, 397);
			Controls.Add(messagePanel);
			Controls.Add(messageRemoveButton);
			Controls.Add(messageAddButton);
			Controls.Add(messageNum);
			Controls.Add(label4);
			Controls.Add(menuStrip1);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			MainMenuStrip = menuStrip1;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MinimumSize = new System.Drawing.Size(470, 398);
			Name = "MainForm";
			Text = "SA2 Message File Editor";
			FormClosing += MainForm_FormClosing;
			Load += MainForm_Load;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			linePanel.ResumeLayout(false);
			linePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)voiceSelector).EndInit();
			((System.ComponentModel.ISupportInitialize)audioSelector).EndInit();
			messagePanel.ResumeLayout(false);
			messagePanel.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
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
		private System.Windows.Forms.ToolStripMenuItem textOnlyToolStripMenuItem;
		private System.Windows.Forms.CheckBox messageEmerald2P;
		private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem autoToolStripMenuItem;
	}
}

