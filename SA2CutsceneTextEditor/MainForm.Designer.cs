namespace SA2CutsceneTextEditor
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
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
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
			this.messageEdit = new System.Windows.Forms.RichTextBox();
			this.sceneRemoveButton = new System.Windows.Forms.Button();
			this.sceneAddButton = new System.Windows.Forms.Button();
			this.sceneNum = new System.Windows.Forms.ComboBox();
			this.messagePanel = new System.Windows.Forms.Panel();
			this.messageCharacter = new System.Windows.Forms.NumericUpDown();
			this.messageRemoveButton = new System.Windows.Forms.Button();
			this.messageAddButton = new System.Windows.Forms.Button();
			this.messageNum = new System.Windows.Forms.ComboBox();
			this.scenePanel = new System.Windows.Forms.Panel();
			this.sceneID = new System.Windows.Forms.NumericUpDown();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			label4 = new System.Windows.Forms.Label();
			emptyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.messagePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.messageCharacter)).BeginInit();
			this.scenePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sceneID)).BeginInit();
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
			label4.Size = new System.Drawing.Size(41, 13);
			label4.TabIndex = 1;
			label4.Text = "Scene:";
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
			label1.Location = new System.Drawing.Point(12, 32);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(53, 13);
			label1.TabIndex = 2;
			label1.Text = "Message:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 5);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(21, 13);
			label2.TabIndex = 0;
			label2.Text = "ID:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(12, 6);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(56, 13);
			label3.TabIndex = 0;
			label3.Text = "Character:";
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
			// messageEdit
			// 
			this.messageEdit.AcceptsTab = true;
			this.messageEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.messageEdit.DetectUrls = false;
			this.messageEdit.Location = new System.Drawing.Point(12, 30);
			this.messageEdit.Name = "messageEdit";
			this.messageEdit.Size = new System.Drawing.Size(310, 157);
			this.messageEdit.TabIndex = 2;
			this.messageEdit.Text = "";
			this.messageEdit.TextChanged += new System.EventHandler(this.messageEdit_TextChanged);
			// 
			// sceneRemoveButton
			// 
			this.sceneRemoveButton.AutoSize = true;
			this.sceneRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.sceneRemoveButton.Enabled = false;
			this.sceneRemoveButton.Location = new System.Drawing.Point(265, 27);
			this.sceneRemoveButton.Name = "sceneRemoveButton";
			this.sceneRemoveButton.Size = new System.Drawing.Size(57, 23);
			this.sceneRemoveButton.TabIndex = 4;
			this.sceneRemoveButton.Text = "Remove";
			this.sceneRemoveButton.UseVisualStyleBackColor = true;
			this.sceneRemoveButton.Click += new System.EventHandler(this.sceneRemoveButton_Click);
			// 
			// sceneAddButton
			// 
			this.sceneAddButton.AutoSize = true;
			this.sceneAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.sceneAddButton.Location = new System.Drawing.Point(223, 27);
			this.sceneAddButton.Name = "sceneAddButton";
			this.sceneAddButton.Size = new System.Drawing.Size(36, 23);
			this.sceneAddButton.TabIndex = 3;
			this.sceneAddButton.Text = "Add";
			this.sceneAddButton.UseVisualStyleBackColor = true;
			this.sceneAddButton.Click += new System.EventHandler(this.sceneAddButton_Click);
			// 
			// sceneNum
			// 
			this.sceneNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.sceneNum.FormattingEnabled = true;
			this.sceneNum.Location = new System.Drawing.Point(59, 29);
			this.sceneNum.Name = "sceneNum";
			this.sceneNum.Size = new System.Drawing.Size(158, 21);
			this.sceneNum.TabIndex = 2;
			this.sceneNum.SelectedIndexChanged += new System.EventHandler(this.sceneNum_SelectedIndexChanged);
			// 
			// messagePanel
			// 
			this.messagePanel.Controls.Add(this.messageCharacter);
			this.messagePanel.Controls.Add(label3);
			this.messagePanel.Controls.Add(this.messageEdit);
			this.messagePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.messagePanel.Enabled = false;
			this.messagePanel.Location = new System.Drawing.Point(0, 56);
			this.messagePanel.Name = "messagePanel";
			this.messagePanel.Size = new System.Drawing.Size(334, 199);
			this.messagePanel.TabIndex = 6;
			// 
			// messageCharacter
			// 
			this.messageCharacter.Location = new System.Drawing.Point(74, 3);
			this.messageCharacter.Name = "messageCharacter";
			this.messageCharacter.Size = new System.Drawing.Size(71, 20);
			this.messageCharacter.TabIndex = 1;
			this.messageCharacter.ValueChanged += new System.EventHandler(this.messageCharacter_ValueChanged);
			// 
			// messageRemoveButton
			// 
			this.messageRemoveButton.AutoSize = true;
			this.messageRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.messageRemoveButton.Enabled = false;
			this.messageRemoveButton.Location = new System.Drawing.Point(265, 27);
			this.messageRemoveButton.Name = "messageRemoveButton";
			this.messageRemoveButton.Size = new System.Drawing.Size(57, 23);
			this.messageRemoveButton.TabIndex = 5;
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
			this.messageAddButton.TabIndex = 4;
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
			this.messageNum.TabIndex = 3;
			this.messageNum.SelectedIndexChanged += new System.EventHandler(this.messageNum_SelectedIndexChanged);
			// 
			// scenePanel
			// 
			this.scenePanel.Controls.Add(this.sceneID);
			this.scenePanel.Controls.Add(label2);
			this.scenePanel.Controls.Add(this.messageRemoveButton);
			this.scenePanel.Controls.Add(this.messagePanel);
			this.scenePanel.Controls.Add(this.messageAddButton);
			this.scenePanel.Controls.Add(label1);
			this.scenePanel.Controls.Add(this.messageNum);
			this.scenePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.scenePanel.Enabled = false;
			this.scenePanel.Location = new System.Drawing.Point(0, 56);
			this.scenePanel.Name = "scenePanel";
			this.scenePanel.Size = new System.Drawing.Size(334, 255);
			this.scenePanel.TabIndex = 5;
			// 
			// sceneID
			// 
			this.sceneID.Location = new System.Drawing.Point(39, 3);
			this.sceneID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.sceneID.Name = "sceneID";
			this.sceneID.Size = new System.Drawing.Size(75, 20);
			this.sceneID.TabIndex = 1;
			this.sceneID.ValueChanged += new System.EventHandler(this.sceneID_ValueChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(334, 311);
			this.Controls.Add(this.scenePanel);
			this.Controls.Add(this.sceneRemoveButton);
			this.Controls.Add(this.sceneAddButton);
			this.Controls.Add(this.sceneNum);
			this.Controls.Add(label4);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "SA2 Cutscene Text Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.messagePanel.ResumeLayout(false);
			this.messagePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.messageCharacter)).EndInit();
			this.scenePanel.ResumeLayout(false);
			this.scenePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sceneID)).EndInit();
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
		private System.Windows.Forms.RichTextBox messageEdit;
		private System.Windows.Forms.Button sceneRemoveButton;
		private System.Windows.Forms.Button sceneAddButton;
		private System.Windows.Forms.ComboBox sceneNum;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem encodingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem shiftJISToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windows1252ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bigEndianGCSteamToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.Panel messagePanel;
		private System.Windows.Forms.Button messageRemoveButton;
		private System.Windows.Forms.Button messageAddButton;
		private System.Windows.Forms.ComboBox messageNum;
		private System.Windows.Forms.Panel scenePanel;
		private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
		private System.Windows.Forms.NumericUpDown sceneID;
		private System.Windows.Forms.NumericUpDown messageCharacter;
	}
}

