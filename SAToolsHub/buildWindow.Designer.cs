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
			this.components = new System.ComponentModel.Container();
			this.btnManual = new System.Windows.Forms.Button();
			this.btnAuto = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.tabEXE = new System.Windows.Forms.TabPage();
			this.chkBoxEXE = new System.Windows.Forms.CheckedListBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.btnChkAll = new System.Windows.Forms.Button();
			this.btnUnchkAll = new System.Windows.Forms.Button();
			this.tabDLL = new System.Windows.Forms.TabPage();
			this.chkBoxDLL = new System.Windows.Forms.CheckedListBox();
			this.tabMDL = new System.Windows.Forms.TabPage();
			this.chkBoxMDL = new System.Windows.Forms.CheckedListBox();
			this.tabEXE.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabDLL.SuspendLayout();
			this.tabMDL.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnManual
			// 
			this.btnManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnManual.Location = new System.Drawing.Point(12, 330);
			this.btnManual.Name = "btnManual";
			this.btnManual.Size = new System.Drawing.Size(135, 33);
			this.btnManual.TabIndex = 0;
			this.btnManual.Text = "Manual";
			this.toolTip1.SetToolTip(this.btnManual, "Opens a window for you to manually assets and export them to an INI mod or to C c" +
        "ode.");
			this.btnManual.UseVisualStyleBackColor = true;
			this.btnManual.Click += new System.EventHandler(this.btnManual_Click);
			// 
			// btnAuto
			// 
			this.btnAuto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAuto.Location = new System.Drawing.Point(153, 330);
			this.btnAuto.Name = "btnAuto";
			this.btnAuto.Size = new System.Drawing.Size(135, 33);
			this.btnAuto.TabIndex = 1;
			this.btnAuto.Text = "Automatic";
			this.toolTip1.SetToolTip(this.btnAuto, "Automatically compiles modified assets into an INI mod.");
			this.btnAuto.UseVisualStyleBackColor = true;
			this.btnAuto.Click += new System.EventHandler(this.btnAuto_Click);
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			// 
			// tabEXE
			// 
			this.tabEXE.Controls.Add(this.chkBoxEXE);
			this.tabEXE.Location = new System.Drawing.Point(4, 25);
			this.tabEXE.Name = "tabEXE";
			this.tabEXE.Padding = new System.Windows.Forms.Padding(3);
			this.tabEXE.Size = new System.Drawing.Size(268, 258);
			this.tabEXE.TabIndex = 0;
			this.tabEXE.Text = "EXE Data";
			this.tabEXE.UseVisualStyleBackColor = true;
			// 
			// chkBoxEXE
			// 
			this.chkBoxEXE.CheckOnClick = true;
			this.chkBoxEXE.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chkBoxEXE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBoxEXE.FormattingEnabled = true;
			this.chkBoxEXE.Location = new System.Drawing.Point(3, 3);
			this.chkBoxEXE.Name = "chkBoxEXE";
			this.chkBoxEXE.Size = new System.Drawing.Size(262, 252);
			this.chkBoxEXE.TabIndex = 3;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabEXE);
			this.tabControl1.Controls.Add(this.tabDLL);
			this.tabControl1.Controls.Add(this.tabMDL);
			this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(276, 287);
			this.tabControl1.TabIndex = 5;
			// 
			// btnChkAll
			// 
			this.btnChkAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnChkAll.Location = new System.Drawing.Point(12, 301);
			this.btnChkAll.Name = "btnChkAll";
			this.btnChkAll.Size = new System.Drawing.Size(135, 23);
			this.btnChkAll.TabIndex = 6;
			this.btnChkAll.Text = "Check All";
			this.toolTip1.SetToolTip(this.btnChkAll, "Checks all items in current tab. (Selecting all may take a while to process)");
			this.btnChkAll.UseVisualStyleBackColor = true;
			this.btnChkAll.Click += new System.EventHandler(this.btnChkAll_Click);
			// 
			// btnUnchkAll
			// 
			this.btnUnchkAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnUnchkAll.Location = new System.Drawing.Point(153, 301);
			this.btnUnchkAll.Name = "btnUnchkAll";
			this.btnUnchkAll.Size = new System.Drawing.Size(135, 23);
			this.btnUnchkAll.TabIndex = 7;
			this.btnUnchkAll.Text = "Uncheck All";
			this.toolTip1.SetToolTip(this.btnUnchkAll, "Unselects all items in the current tab.");
			this.btnUnchkAll.UseVisualStyleBackColor = true;
			this.btnUnchkAll.Click += new System.EventHandler(this.button1_Click);
			// 
			// tabDLL
			// 
			this.tabDLL.Controls.Add(this.chkBoxDLL);
			this.tabDLL.Location = new System.Drawing.Point(4, 25);
			this.tabDLL.Name = "tabDLL";
			this.tabDLL.Padding = new System.Windows.Forms.Padding(3);
			this.tabDLL.Size = new System.Drawing.Size(279, 258);
			this.tabDLL.TabIndex = 1;
			this.tabDLL.Text = "DLL Data";
			this.tabDLL.UseVisualStyleBackColor = true;
			// 
			// chkBoxDLL
			// 
			this.chkBoxDLL.CheckOnClick = true;
			this.chkBoxDLL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chkBoxDLL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBoxDLL.FormattingEnabled = true;
			this.chkBoxDLL.Location = new System.Drawing.Point(3, 3);
			this.chkBoxDLL.Name = "chkBoxDLL";
			this.chkBoxDLL.Size = new System.Drawing.Size(273, 252);
			this.chkBoxDLL.TabIndex = 4;
			// 
			// tabMDL
			// 
			this.tabMDL.Controls.Add(this.chkBoxMDL);
			this.tabMDL.Location = new System.Drawing.Point(4, 25);
			this.tabMDL.Name = "tabMDL";
			this.tabMDL.Padding = new System.Windows.Forms.Padding(3);
			this.tabMDL.Size = new System.Drawing.Size(268, 258);
			this.tabMDL.TabIndex = 2;
			this.tabMDL.Text = "MDL Files";
			this.tabMDL.UseVisualStyleBackColor = true;
			// 
			// chkBoxMDL
			// 
			this.chkBoxMDL.CheckOnClick = true;
			this.chkBoxMDL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chkBoxMDL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBoxMDL.FormattingEnabled = true;
			this.chkBoxMDL.Location = new System.Drawing.Point(3, 3);
			this.chkBoxMDL.Name = "chkBoxMDL";
			this.chkBoxMDL.Size = new System.Drawing.Size(262, 252);
			this.chkBoxMDL.TabIndex = 5;
			// 
			// buildWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(299, 373);
			this.Controls.Add(this.btnUnchkAll);
			this.Controls.Add(this.btnChkAll);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.btnAuto);
			this.Controls.Add(this.btnManual);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "buildWindow";
			this.Text = "Select Data Files";
			this.Shown += new System.EventHandler(this.buildWindow_Shown);
			this.tabEXE.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabDLL.ResumeLayout(false);
			this.tabMDL.ResumeLayout(false);
			this.ResumeLayout(false);

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
	}
}