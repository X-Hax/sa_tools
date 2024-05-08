using System;

namespace SAFontEdit
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			menuStripMain = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportIndividualCharactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			fONTDATAFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			simpleFormatFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			simpleFormatFile32bppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
			toolStripMenuItemSearch = new System.Windows.Forms.ToolStripMenuItem();
			listBox1 = new System.Windows.Forms.ListBox();
			panel3 = new System.Windows.Forms.Panel();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			characterTileControl1 = new CharacterTileControl();
			buttonImportCharacter = new System.Windows.Forms.Button();
			buttonExportCharacter = new System.Windows.Forms.Button();
			panel2 = new System.Windows.Forms.Panel();
			labelColor = new System.Windows.Forms.Label();
			pictureBoxFontColor = new System.Windows.Forms.PictureBox();
			labelA = new System.Windows.Forms.Label();
			labelB = new System.Windows.Forms.Label();
			labelG = new System.Windows.Forms.Label();
			labelR = new System.Windows.Forms.Label();
			numericUpDownAlpha = new System.Windows.Forms.NumericUpDown();
			numericUpDownBlue = new System.Windows.Forms.NumericUpDown();
			numericUpDownGreen = new System.Windows.Forms.NumericUpDown();
			numericUpDownRed = new System.Windows.Forms.NumericUpDown();
			trackBar1 = new System.Windows.Forms.TrackBar();
			label1 = new System.Windows.Forms.Label();
			errorProvider1 = new System.Windows.Forms.ErrorProvider(components);
			helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			sAFontEditGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			bugReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			menuStripMain.SuspendLayout();
			panel3.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontColor).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownAlpha).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownBlue).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownGreen).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownRed).BeginInit();
			((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
			((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
			SuspendLayout();
			// 
			// menuStripMain
			// 
			menuStripMain.ImageScalingSize = new System.Drawing.Size(24, 24);
			menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, toolStripTextBox1, toolStripMenuItemSearch, helpToolStripMenuItem });
			menuStripMain.Location = new System.Drawing.Point(0, 0);
			menuStripMain.Name = "menuStripMain";
			menuStripMain.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
			menuStripMain.Size = new System.Drawing.Size(876, 32);
			menuStripMain.TabIndex = 0;
			menuStripMain.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openToolStripMenuItem, importToolStripMenuItem, toolStripSeparator2, extractAllToolStripMenuItem, exportIndividualCharactersToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator1, quitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(37, 28);
			fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Image = Properties.Resources.open;
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			openToolStripMenuItem.Text = "&Open...";
			openToolStripMenuItem.Click += openToolStripMenuItem_Click;
			// 
			// importToolStripMenuItem
			// 
			importToolStripMenuItem.Image = Properties.Resources.import;
			importToolStripMenuItem.Name = "importToolStripMenuItem";
			importToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			importToolStripMenuItem.Text = "&Import...";
			importToolStripMenuItem.Click += importToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(171, 6);
			// 
			// extractAllToolStripMenuItem
			// 
			extractAllToolStripMenuItem.Enabled = false;
			extractAllToolStripMenuItem.Image = Properties.Resources.export;
			extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
			extractAllToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			extractAllToolStripMenuItem.Text = "&Export...";
			extractAllToolStripMenuItem.Click += extractAllToolStripMenuItem_Click;
			// 
			// exportIndividualCharactersToolStripMenuItem
			// 
			exportIndividualCharactersToolStripMenuItem.Enabled = false;
			exportIndividualCharactersToolStripMenuItem.Image = Properties.Resources.character;
			exportIndividualCharactersToolStripMenuItem.Name = "exportIndividualCharactersToolStripMenuItem";
			exportIndividualCharactersToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			exportIndividualCharactersToolStripMenuItem.Text = "Export &characters...";
			exportIndividualCharactersToolStripMenuItem.Click += exportIndividualCharactersToolStripMenuItem_Click;
			// 
			// saveAsToolStripMenuItem
			// 
			saveAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { fONTDATAFileToolStripMenuItem, simpleFormatFileToolStripMenuItem, simpleFormatFile32bppToolStripMenuItem });
			saveAsToolStripMenuItem.Enabled = false;
			saveAsToolStripMenuItem.Image = Properties.Resources.saveas;
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			saveAsToolStripMenuItem.Text = "Save &As";
			// 
			// fONTDATAFileToolStripMenuItem
			// 
			fONTDATAFileToolStripMenuItem.Name = "fONTDATAFileToolStripMenuItem";
			fONTDATAFileToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			fONTDATAFileToolStripMenuItem.Text = "SADX2004 FONTDATA file...";
			fONTDATAFileToolStripMenuItem.Click += fONTDATAFileToolStripMenuItem_Click;
			// 
			// simpleFormatFileToolStripMenuItem
			// 
			simpleFormatFileToolStripMenuItem.Name = "simpleFormatFileToolStripMenuItem";
			simpleFormatFileToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			simpleFormatFileToolStripMenuItem.Text = "Simple format file (1bpp)...";
			simpleFormatFileToolStripMenuItem.Click += simpleFormatFileToolStripMenuItem_Click;
			// 
			// simpleFormatFile32bppToolStripMenuItem
			// 
			simpleFormatFile32bppToolStripMenuItem.Name = "simpleFormatFile32bppToolStripMenuItem";
			simpleFormatFile32bppToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			simpleFormatFile32bppToolStripMenuItem.Text = "Simple format file (32bpp)...";
			simpleFormatFile32bppToolStripMenuItem.Click += simpleFormatFile32bppToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
			// 
			// quitToolStripMenuItem
			// 
			quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			quitToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			quitToolStripMenuItem.Text = "&Quit";
			quitToolStripMenuItem.Click += quitToolStripMenuItem_Click;
			// 
			// toolStripTextBox1
			// 
			toolStripTextBox1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			toolStripTextBox1.ForeColor = System.Drawing.SystemColors.GrayText;
			toolStripTextBox1.Name = "toolStripTextBox1";
			toolStripTextBox1.Size = new System.Drawing.Size(100, 28);
			toolStripTextBox1.Text = "Search character...";
			toolStripTextBox1.ToolTipText = "Search items by ID or character.";
			toolStripTextBox1.KeyPress += toolStripTextBox1_KeyPress;
			toolStripTextBox1.Click += toolStripTextBox1_Click;
			// 
			// toolStripMenuItemSearch
			// 
			toolStripMenuItemSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			toolStripMenuItemSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			toolStripMenuItemSearch.Image = Properties.Resources.search;
			toolStripMenuItemSearch.Name = "toolStripMenuItemSearch";
			toolStripMenuItemSearch.Size = new System.Drawing.Size(36, 28);
			toolStripMenuItemSearch.Text = "Search";
			toolStripMenuItemSearch.ToolTipText = "Search items by ID or character.";
			toolStripMenuItemSearch.Click += toolStripMenuItemSearch_Click;
			// 
			// listBox1
			// 
			listBox1.Dock = System.Windows.Forms.DockStyle.Left;
			listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			listBox1.FormattingEnabled = true;
			listBox1.IntegralHeight = false;
			listBox1.ItemHeight = 24;
			listBox1.Location = new System.Drawing.Point(0, 32);
			listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			listBox1.Name = "listBox1";
			listBox1.Size = new System.Drawing.Size(137, 502);
			listBox1.TabIndex = 1;
			listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
			// 
			// panel3
			// 
			panel3.AutoScroll = true;
			panel3.BackColor = System.Drawing.SystemColors.ControlDark;
			panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			panel3.Controls.Add(tableLayoutPanel1);
			panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			panel3.Location = new System.Drawing.Point(137, 71);
			panel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			panel3.Name = "panel3";
			panel3.Size = new System.Drawing.Size(739, 463);
			panel3.TabIndex = 12;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(characterTileControl1, 0, 0);
			tableLayoutPanel1.Controls.Add(buttonImportCharacter, 0, 1);
			tableLayoutPanel1.Controls.Add(buttonExportCharacter, 0, 2);
			tableLayoutPanel1.Location = new System.Drawing.Point(3, 4);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 3;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.Size = new System.Drawing.Size(120, 92);
			tableLayoutPanel1.TabIndex = 5;
			// 
			// characterTileControl1
			// 
			characterTileControl1.BackgroundImage = (System.Drawing.Image)resources.GetObject("characterTileControl1.BackgroundImage");
			characterTileControl1.Location = new System.Drawing.Point(3, 3);
			characterTileControl1.Name = "characterTileControl1";
			characterTileControl1.Size = new System.Drawing.Size(24, 24);
			characterTileControl1.TabIndex = 4;
			characterTileControl1.MouseDown += TilePicture_MouseDown;
			characterTileControl1.MouseMove += TilePicture_MouseMove;
			// 
			// buttonImportCharacter
			// 
			buttonImportCharacter.Location = new System.Drawing.Point(3, 33);
			buttonImportCharacter.Name = "buttonImportCharacter";
			buttonImportCharacter.Size = new System.Drawing.Size(110, 23);
			buttonImportCharacter.TabIndex = 5;
			buttonImportCharacter.Text = "Import PNG...";
			buttonImportCharacter.UseVisualStyleBackColor = true;
			buttonImportCharacter.Click += buttonImportCharacter_Click;
			// 
			// buttonExportCharacter
			// 
			buttonExportCharacter.Location = new System.Drawing.Point(3, 62);
			buttonExportCharacter.Name = "buttonExportCharacter";
			buttonExportCharacter.Size = new System.Drawing.Size(110, 23);
			buttonExportCharacter.TabIndex = 6;
			buttonExportCharacter.Text = "Export PNG...";
			buttonExportCharacter.UseVisualStyleBackColor = true;
			buttonExportCharacter.Click += buttonExportCharacter_Click;
			// 
			// panel2
			// 
			panel2.AutoSize = true;
			panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panel2.Controls.Add(labelColor);
			panel2.Controls.Add(pictureBoxFontColor);
			panel2.Controls.Add(labelA);
			panel2.Controls.Add(labelB);
			panel2.Controls.Add(labelG);
			panel2.Controls.Add(labelR);
			panel2.Controls.Add(numericUpDownAlpha);
			panel2.Controls.Add(numericUpDownBlue);
			panel2.Controls.Add(numericUpDownGreen);
			panel2.Controls.Add(numericUpDownRed);
			panel2.Controls.Add(trackBar1);
			panel2.Controls.Add(label1);
			panel2.Dock = System.Windows.Forms.DockStyle.Top;
			panel2.Location = new System.Drawing.Point(137, 32);
			panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			panel2.Name = "panel2";
			panel2.Size = new System.Drawing.Size(739, 39);
			panel2.TabIndex = 11;
			// 
			// labelColor
			// 
			labelColor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			labelColor.AutoSize = true;
			labelColor.Location = new System.Drawing.Point(368, 9);
			labelColor.Name = "labelColor";
			labelColor.Size = new System.Drawing.Size(39, 15);
			labelColor.TabIndex = 16;
			labelColor.Text = "Color:";
			// 
			// pictureBoxFontColor
			// 
			pictureBoxFontColor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			pictureBoxFontColor.BackColor = System.Drawing.Color.White;
			pictureBoxFontColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			pictureBoxFontColor.Location = new System.Drawing.Point(413, 4);
			pictureBoxFontColor.Name = "pictureBoxFontColor";
			pictureBoxFontColor.Size = new System.Drawing.Size(28, 28);
			pictureBoxFontColor.TabIndex = 15;
			pictureBoxFontColor.TabStop = false;
			pictureBoxFontColor.Click += pictureBoxFontColor_Click;
			// 
			// labelA
			// 
			labelA.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			labelA.AutoSize = true;
			labelA.Location = new System.Drawing.Point(661, 9);
			labelA.Name = "labelA";
			labelA.Size = new System.Drawing.Size(18, 15);
			labelA.TabIndex = 14;
			labelA.Text = "A:";
			// 
			// labelB
			// 
			labelB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			labelB.AutoSize = true;
			labelB.Location = new System.Drawing.Point(590, 9);
			labelB.Name = "labelB";
			labelB.Size = new System.Drawing.Size(17, 15);
			labelB.TabIndex = 13;
			labelB.Text = "B:";
			// 
			// labelG
			// 
			labelG.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			labelG.AutoSize = true;
			labelG.Location = new System.Drawing.Point(518, 9);
			labelG.Name = "labelG";
			labelG.Size = new System.Drawing.Size(18, 15);
			labelG.TabIndex = 12;
			labelG.Text = "G:";
			// 
			// labelR
			// 
			labelR.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			labelR.AutoSize = true;
			labelR.Location = new System.Drawing.Point(447, 9);
			labelR.Name = "labelR";
			labelR.Size = new System.Drawing.Size(17, 15);
			labelR.TabIndex = 11;
			labelR.Text = "R:";
			// 
			// numericUpDownAlpha
			// 
			numericUpDownAlpha.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			numericUpDownAlpha.Location = new System.Drawing.Point(685, 7);
			numericUpDownAlpha.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			numericUpDownAlpha.Name = "numericUpDownAlpha";
			numericUpDownAlpha.Size = new System.Drawing.Size(42, 23);
			numericUpDownAlpha.TabIndex = 10;
			numericUpDownAlpha.Value = new decimal(new int[] { 255, 0, 0, 0 });
			numericUpDownAlpha.ValueChanged += numericUpDownAlpha_ValueChanged;
			// 
			// numericUpDownBlue
			// 
			numericUpDownBlue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			numericUpDownBlue.Location = new System.Drawing.Point(613, 7);
			numericUpDownBlue.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			numericUpDownBlue.Name = "numericUpDownBlue";
			numericUpDownBlue.Size = new System.Drawing.Size(42, 23);
			numericUpDownBlue.TabIndex = 9;
			numericUpDownBlue.Value = new decimal(new int[] { 255, 0, 0, 0 });
			// 
			// numericUpDownGreen
			// 
			numericUpDownGreen.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			numericUpDownGreen.Location = new System.Drawing.Point(542, 7);
			numericUpDownGreen.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			numericUpDownGreen.Name = "numericUpDownGreen";
			numericUpDownGreen.Size = new System.Drawing.Size(42, 23);
			numericUpDownGreen.TabIndex = 8;
			numericUpDownGreen.Value = new decimal(new int[] { 255, 0, 0, 0 });
			// 
			// numericUpDownRed
			// 
			numericUpDownRed.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			numericUpDownRed.Location = new System.Drawing.Point(470, 7);
			numericUpDownRed.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			numericUpDownRed.Name = "numericUpDownRed";
			numericUpDownRed.Size = new System.Drawing.Size(42, 23);
			numericUpDownRed.TabIndex = 7;
			numericUpDownRed.Value = new decimal(new int[] { 255, 0, 0, 0 });
			// 
			// trackBar1
			// 
			trackBar1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			trackBar1.AutoSize = false;
			trackBar1.Location = new System.Drawing.Point(54, 9);
			trackBar1.Margin = new System.Windows.Forms.Padding(2);
			trackBar1.Maximum = 32;
			trackBar1.Minimum = 1;
			trackBar1.Name = "trackBar1";
			trackBar1.Size = new System.Drawing.Size(309, 28);
			trackBar1.TabIndex = 6;
			trackBar1.Value = 4;
			trackBar1.ValueChanged += trackBar1_Scroll;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(6, 12);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(42, 15);
			label1.TabIndex = 3;
			label1.Text = "Zoom:";
			// 
			// errorProvider1
			// 
			errorProvider1.ContainerControl = this;
			// 
			// helpToolStripMenuItem
			// 
			helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { sAFontEditGuideToolStripMenuItem, bugReportToolStripMenuItem });
			helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			helpToolStripMenuItem.Size = new System.Drawing.Size(44, 28);
			helpToolStripMenuItem.Text = "Help";
			// 
			// sAFontEditGuideToolStripMenuItem
			// 
			sAFontEditGuideToolStripMenuItem.Image = Properties.Resources.help;
			sAFontEditGuideToolStripMenuItem.Name = "sAFontEditGuideToolStripMenuItem";
			sAFontEditGuideToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
			sAFontEditGuideToolStripMenuItem.Text = "SAFontEdit Guide";
			sAFontEditGuideToolStripMenuItem.Click += sAFontEditGuideToolStripMenuItem_Click;
			// 
			// bugReportToolStripMenuItem
			// 
			bugReportToolStripMenuItem.Image = Properties.Resources.bug;
			bugReportToolStripMenuItem.Name = "bugReportToolStripMenuItem";
			bugReportToolStripMenuItem.Size = new System.Drawing.Size(188, 30);
			bugReportToolStripMenuItem.Text = "Bug Report";
			bugReportToolStripMenuItem.Click += bugReportToolStripMenuItem_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(876, 534);
			Controls.Add(panel3);
			Controls.Add(panel2);
			Controls.Add(listBox1);
			Controls.Add(menuStripMain);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			MainMenuStrip = menuStripMain;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "MainForm";
			Text = "SAFontEdit";
			Load += Form1_Load;
			MouseWheel += MainForm_MouseWheel;
			menuStripMain.ResumeLayout(false);
			menuStripMain.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			tableLayoutPanel1.ResumeLayout(false);
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontColor).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownAlpha).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownBlue).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownGreen).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownRed).EndInit();
			((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
			((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		private void MainForm_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int newval;
			if (e.Delta > 0)
				newval = trackBar1.Value + 2;
			else
				newval = trackBar1.Value - 2;
			newval = Math.Min(trackBar1.Maximum, newval);
			newval = Math.Max(trackBar1.Minimum, newval);
			trackBar1.Value = newval;
		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractAllToolStripMenuItem;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportIndividualCharactersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fONTDATAFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem simpleFormatFileToolStripMenuItem;
		private System.Windows.Forms.PictureBox pictureBoxFontColor;
		private System.Windows.Forms.Label labelA;
		private System.Windows.Forms.Label labelB;
		private System.Windows.Forms.Label labelG;
		private System.Windows.Forms.Label labelR;
		private System.Windows.Forms.NumericUpDown numericUpDownAlpha;
		private System.Windows.Forms.NumericUpDown numericUpDownBlue;
		private System.Windows.Forms.NumericUpDown numericUpDownGreen;
		private System.Windows.Forms.NumericUpDown numericUpDownRed;
		private System.Windows.Forms.Label labelColor;
		private CharacterTileControl characterTileControl1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button buttonImportCharacter;
		private System.Windows.Forms.Button buttonExportCharacter;
		private System.Windows.Forms.ToolStripMenuItem simpleFormatFile32bppToolStripMenuItem;
		private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSearch;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sAFontEditGuideToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bugReportToolStripMenuItem;
	}
}

