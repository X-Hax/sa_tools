namespace SAToolsHub
{
	partial class buildWindow
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
			btnManual = new System.Windows.Forms.Button();
			btnAuto = new System.Windows.Forms.Button();
			toolTip1 = new System.Windows.Forms.ToolTip(components);
			btnChkAll = new System.Windows.Forms.Button();
			btnUnchkAll = new System.Windows.Forms.Button();
			backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			tabEXE = new System.Windows.Forms.TabPage();
			chkBoxEXE = new System.Windows.Forms.CheckedListBox();
			tabControl1 = new System.Windows.Forms.TabControl();
			tabDLL = new System.Windows.Forms.TabPage();
			chkBoxDLL = new System.Windows.Forms.CheckedListBox();
			tabMDL = new System.Windows.Forms.TabPage();
			chkBoxMDL = new System.Windows.Forms.CheckedListBox();
			tabNB = new System.Windows.Forms.TabPage();
			checkNBFiles = new System.Windows.Forms.CheckedListBox();
			tabEXE.SuspendLayout();
			tabControl1.SuspendLayout();
			tabDLL.SuspendLayout();
			tabMDL.SuspendLayout();
			tabNB.SuspendLayout();
			SuspendLayout();
			// 
			// btnManual
			// 
			btnManual.Location = new System.Drawing.Point(14, 381);
			btnManual.Margin = new System.Windows.Forms.Padding(4);
			btnManual.Name = "btnManual";
			btnManual.Size = new System.Drawing.Size(158, 38);
			btnManual.TabIndex = 0;
			btnManual.Text = "Manual";
			toolTip1.SetToolTip(btnManual, "Opens a window for you to manually assets and export them to an INI mod or to C code.");
			btnManual.UseVisualStyleBackColor = true;
			btnManual.Click += btnManual_Click;
			// 
			// btnAuto
			// 
			btnAuto.Location = new System.Drawing.Point(178, 381);
			btnAuto.Margin = new System.Windows.Forms.Padding(4);
			btnAuto.Name = "btnAuto";
			btnAuto.Size = new System.Drawing.Size(158, 38);
			btnAuto.TabIndex = 1;
			btnAuto.Text = "Automatic";
			toolTip1.SetToolTip(btnAuto, "Automatically compiles modified assets into an INI mod.");
			btnAuto.UseVisualStyleBackColor = true;
			btnAuto.Click += btnAuto_Click;
			// 
			// btnChkAll
			// 
			btnChkAll.Location = new System.Drawing.Point(14, 347);
			btnChkAll.Margin = new System.Windows.Forms.Padding(4);
			btnChkAll.Name = "btnChkAll";
			btnChkAll.Size = new System.Drawing.Size(158, 26);
			btnChkAll.TabIndex = 6;
			btnChkAll.Text = "Check All";
			toolTip1.SetToolTip(btnChkAll, "Checks all items in current tab. (Selecting all may take a while to process)");
			btnChkAll.UseVisualStyleBackColor = true;
			btnChkAll.Click += btnChkAll_Click;
			// 
			// btnUnchkAll
			// 
			btnUnchkAll.Location = new System.Drawing.Point(178, 347);
			btnUnchkAll.Margin = new System.Windows.Forms.Padding(4);
			btnUnchkAll.Name = "btnUnchkAll";
			btnUnchkAll.Size = new System.Drawing.Size(158, 26);
			btnUnchkAll.TabIndex = 7;
			btnUnchkAll.Text = "Uncheck All";
			toolTip1.SetToolTip(btnUnchkAll, "Unselects all items in the current tab.");
			btnUnchkAll.UseVisualStyleBackColor = true;
			btnUnchkAll.Click += button1_Click;
			// 
			// backgroundWorker1
			// 
			backgroundWorker1.DoWork += backgroundWorker1_DoWork;
			// 
			// tabEXE
			// 
			tabEXE.Controls.Add(chkBoxEXE);
			tabEXE.Location = new System.Drawing.Point(4, 24);
			tabEXE.Margin = new System.Windows.Forms.Padding(4);
			tabEXE.Name = "tabEXE";
			tabEXE.Padding = new System.Windows.Forms.Padding(4);
			tabEXE.Size = new System.Drawing.Size(314, 303);
			tabEXE.TabIndex = 0;
			tabEXE.Text = "EXE Data";
			tabEXE.UseVisualStyleBackColor = true;
			// 
			// chkBoxEXE
			// 
			chkBoxEXE.CheckOnClick = true;
			chkBoxEXE.Dock = System.Windows.Forms.DockStyle.Fill;
			chkBoxEXE.FormattingEnabled = true;
			chkBoxEXE.HorizontalScrollbar = true;
			chkBoxEXE.Location = new System.Drawing.Point(4, 4);
			chkBoxEXE.Margin = new System.Windows.Forms.Padding(4);
			chkBoxEXE.Name = "chkBoxEXE";
			chkBoxEXE.Size = new System.Drawing.Size(306, 295);
			chkBoxEXE.TabIndex = 3;
			chkBoxEXE.UseCompatibleTextRendering = true;
			// 
			// tabControl1
			// 
			tabControl1.Controls.Add(tabEXE);
			tabControl1.Controls.Add(tabDLL);
			tabControl1.Controls.Add(tabMDL);
			tabControl1.Controls.Add(tabNB);
			tabControl1.Location = new System.Drawing.Point(14, 14);
			tabControl1.Margin = new System.Windows.Forms.Padding(4);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new System.Drawing.Size(322, 331);
			tabControl1.TabIndex = 5;
			// 
			// tabDLL
			// 
			tabDLL.Controls.Add(chkBoxDLL);
			tabDLL.Location = new System.Drawing.Point(4, 24);
			tabDLL.Margin = new System.Windows.Forms.Padding(4);
			tabDLL.Name = "tabDLL";
			tabDLL.Padding = new System.Windows.Forms.Padding(4);
			tabDLL.Size = new System.Drawing.Size(314, 303);
			tabDLL.TabIndex = 1;
			tabDLL.Text = "DLL Data";
			tabDLL.UseVisualStyleBackColor = true;
			// 
			// chkBoxDLL
			// 
			chkBoxDLL.CheckOnClick = true;
			chkBoxDLL.Dock = System.Windows.Forms.DockStyle.Fill;
			chkBoxDLL.FormattingEnabled = true;
			chkBoxDLL.HorizontalScrollbar = true;
			chkBoxDLL.Location = new System.Drawing.Point(4, 4);
			chkBoxDLL.Margin = new System.Windows.Forms.Padding(4);
			chkBoxDLL.Name = "chkBoxDLL";
			chkBoxDLL.Size = new System.Drawing.Size(306, 295);
			chkBoxDLL.TabIndex = 4;
			chkBoxDLL.UseCompatibleTextRendering = true;
			// 
			// tabMDL
			// 
			tabMDL.Controls.Add(chkBoxMDL);
			tabMDL.Location = new System.Drawing.Point(4, 24);
			tabMDL.Margin = new System.Windows.Forms.Padding(4);
			tabMDL.Name = "tabMDL";
			tabMDL.Padding = new System.Windows.Forms.Padding(4);
			tabMDL.Size = new System.Drawing.Size(314, 303);
			tabMDL.TabIndex = 2;
			tabMDL.Text = "MDL/MTN Files";
			tabMDL.UseVisualStyleBackColor = true;
			// 
			// chkBoxMDL
			// 
			chkBoxMDL.CheckOnClick = true;
			chkBoxMDL.Dock = System.Windows.Forms.DockStyle.Fill;
			chkBoxMDL.FormattingEnabled = true;
			chkBoxMDL.HorizontalScrollbar = true;
			chkBoxMDL.Location = new System.Drawing.Point(4, 4);
			chkBoxMDL.Margin = new System.Windows.Forms.Padding(4);
			chkBoxMDL.Name = "chkBoxMDL";
			chkBoxMDL.Size = new System.Drawing.Size(306, 295);
			chkBoxMDL.TabIndex = 5;
			chkBoxMDL.UseCompatibleTextRendering = true;
			// 
			// tabNB
			// 
			tabNB.Controls.Add(checkNBFiles);
			tabNB.Location = new System.Drawing.Point(4, 24);
			tabNB.Name = "tabNB";
			tabNB.Padding = new System.Windows.Forms.Padding(3);
			tabNB.Size = new System.Drawing.Size(314, 303);
			tabNB.TabIndex = 3;
			tabNB.Text = "NB Files";
			tabNB.UseVisualStyleBackColor = true;
			// 
			// checkNBFiles
			// 
			checkNBFiles.CheckOnClick = true;
			checkNBFiles.Dock = System.Windows.Forms.DockStyle.Fill;
			checkNBFiles.FormattingEnabled = true;
			checkNBFiles.HorizontalScrollbar = true;
			checkNBFiles.Location = new System.Drawing.Point(3, 3);
			checkNBFiles.Margin = new System.Windows.Forms.Padding(4);
			checkNBFiles.Name = "checkNBFiles";
			checkNBFiles.Size = new System.Drawing.Size(308, 297);
			checkNBFiles.TabIndex = 6;
			checkNBFiles.UseCompatibleTextRendering = true;
			// 
			// buildWindow
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(349, 430);
			Controls.Add(btnUnchkAll);
			Controls.Add(btnChkAll);
			Controls.Add(tabControl1);
			Controls.Add(btnAuto);
			Controls.Add(btnManual);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4);
			Name = "buildWindow";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Select Data Files";
			Shown += buildWindow_Shown;
			tabEXE.ResumeLayout(false);
			tabControl1.ResumeLayout(false);
			tabDLL.ResumeLayout(false);
			tabMDL.ResumeLayout(false);
			tabNB.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Button btnManual;
		private System.Windows.Forms.Button btnAuto;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.TabPage tabEXE;
		private System.Windows.Forms.CheckedListBox chkBoxEXE;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabDLL;
		private System.Windows.Forms.CheckedListBox chkBoxDLL;
		private System.Windows.Forms.TabPage tabMDL;
		private System.Windows.Forms.CheckedListBox chkBoxMDL;
		private System.Windows.Forms.Button btnChkAll;
		private System.Windows.Forms.Button btnUnchkAll;
		private System.Windows.Forms.TabPage tabNB;
		private System.Windows.Forms.CheckedListBox checkNBFiles;
	}
}