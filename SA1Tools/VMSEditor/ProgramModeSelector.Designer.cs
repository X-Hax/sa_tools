
namespace VMSEditor
{
	partial class ProgramModeSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgramModeSelector));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonVMIEditor = new System.Windows.Forms.Button();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.buttonDLCEditor = new System.Windows.Forms.Button();
            this.buttonChaoEditor = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.buttonChallengeResultEditor = new System.Windows.Forms.Button();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.buttonWorldRankingsEditor = new System.Windows.Forms.Button();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.buttonAdditional = new System.Windows.Forms.Button();
            this.contextMenuStripTools = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.encryptDecryptDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vMSFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wholeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decodeUploadDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.contextMenuStripTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select program mode:";
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(265, 173);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(150, 32);
            this.buttonExit.TabIndex = 5;
            this.buttonExit.Text = "Exit";
            this.buttonExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonVMIEditor
            // 
            this.buttonVMIEditor.Location = new System.Drawing.Point(54, 173);
            this.buttonVMIEditor.Margin = new System.Windows.Forms.Padding(2);
            this.buttonVMIEditor.Name = "buttonVMIEditor";
            this.buttonVMIEditor.Size = new System.Drawing.Size(150, 32);
            this.buttonVMIEditor.TabIndex = 4;
            this.buttonVMIEditor.Text = "VMI Editor";
            this.buttonVMIEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonVMIEditor.UseVisualStyleBackColor = true;
            this.buttonVMIEditor.Click += new System.EventHandler(this.buttonVMIEditor_Click);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Location = new System.Drawing.Point(54, 40);
            this.buttonOpenFile.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(150, 32);
            this.buttonOpenFile.TabIndex = 1;
            this.buttonOpenFile.Text = "Auto (Open File)";
            this.buttonOpenFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // buttonDLCEditor
            // 
            this.buttonDLCEditor.Location = new System.Drawing.Point(54, 130);
            this.buttonDLCEditor.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDLCEditor.Name = "buttonDLCEditor";
            this.buttonDLCEditor.Size = new System.Drawing.Size(150, 32);
            this.buttonDLCEditor.TabIndex = 3;
            this.buttonDLCEditor.Text = "DLC Editor";
            this.buttonDLCEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonDLCEditor.UseVisualStyleBackColor = true;
            this.buttonDLCEditor.Click += new System.EventHandler(this.buttonDLCEditor_Click);
            // 
            // buttonChaoEditor
            // 
            this.buttonChaoEditor.Location = new System.Drawing.Point(54, 85);
            this.buttonChaoEditor.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChaoEditor.Name = "buttonChaoEditor";
            this.buttonChaoEditor.Size = new System.Drawing.Size(150, 32);
            this.buttonChaoEditor.TabIndex = 2;
            this.buttonChaoEditor.Text = "Chao Editor";
            this.buttonChaoEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonChaoEditor.UseVisualStyleBackColor = true;
            this.buttonChaoEditor.Click += new System.EventHandler(this.buttonChaoEditor_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VMSEditor.Properties.Resources.open;
            this.pictureBox1.InitialImage = global::VMSEditor.Properties.Resources.open;
            this.pictureBox1.Location = new System.Drawing.Point(16, 40);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::VMSEditor.Properties.Resources.editorChao;
            this.pictureBox2.InitialImage = null;
            this.pictureBox2.Location = new System.Drawing.Point(16, 85);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::VMSEditor.Properties.Resources.editorDLC;
            this.pictureBox3.InitialImage = null;
            this.pictureBox3.Location = new System.Drawing.Point(16, 130);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 32);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 8;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::VMSEditor.Properties.Resources.vmi;
            this.pictureBox4.InitialImage = null;
            this.pictureBox4.Location = new System.Drawing.Point(16, 173);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 32);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 9;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.InitialImage = null;
            this.pictureBox5.Location = new System.Drawing.Point(227, 40);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(32, 32);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5.TabIndex = 11;
            this.pictureBox5.TabStop = false;
            // 
            // buttonChallengeResultEditor
            // 
            this.buttonChallengeResultEditor.Location = new System.Drawing.Point(265, 40);
            this.buttonChallengeResultEditor.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChallengeResultEditor.Name = "buttonChallengeResultEditor";
            this.buttonChallengeResultEditor.Size = new System.Drawing.Size(150, 32);
            this.buttonChallengeResultEditor.TabIndex = 10;
            this.buttonChallengeResultEditor.Text = "Challenge Result Viewer";
            this.buttonChallengeResultEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonChallengeResultEditor.UseVisualStyleBackColor = true;
            this.buttonChallengeResultEditor.Click += new System.EventHandler(this.buttonChallengeResultEditor_Click);
            // 
            // pictureBox7
            // 
            this.pictureBox7.Enabled = false;
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.InitialImage = null;
            this.pictureBox7.Location = new System.Drawing.Point(227, 85);
            this.pictureBox7.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(32, 32);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 15;
            this.pictureBox7.TabStop = false;
            // 
            // buttonWorldRankingsEditor
            // 
            this.buttonWorldRankingsEditor.Location = new System.Drawing.Point(265, 85);
            this.buttonWorldRankingsEditor.Margin = new System.Windows.Forms.Padding(2);
            this.buttonWorldRankingsEditor.Name = "buttonWorldRankingsEditor";
            this.buttonWorldRankingsEditor.Size = new System.Drawing.Size(150, 32);
            this.buttonWorldRankingsEditor.TabIndex = 14;
            this.buttonWorldRankingsEditor.Text = "World Rank Converter";
            this.buttonWorldRankingsEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonWorldRankingsEditor.UseVisualStyleBackColor = true;
			this.buttonWorldRankingsEditor.Click += new System.EventHandler(this.buttonWorldRankingsEditor_Click);
			// 
			// pictureBox6
			// 
			this.pictureBox6.Enabled = false;
            this.pictureBox6.Image = global::VMSEditor.Properties.Resources.additionaltools;
            this.pictureBox6.InitialImage = null;
            this.pictureBox6.Location = new System.Drawing.Point(227, 130);
            this.pictureBox6.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(32, 32);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 16;
            this.pictureBox6.TabStop = false;
            // 
            // buttonAdditional
            // 
            this.buttonAdditional.Location = new System.Drawing.Point(265, 130);
            this.buttonAdditional.Name = "buttonAdditional";
            this.buttonAdditional.Size = new System.Drawing.Size(150, 32);
            this.buttonAdditional.TabIndex = 17;
            this.buttonAdditional.Text = "Additional Tools";
            this.buttonAdditional.UseVisualStyleBackColor = true;
            this.buttonAdditional.Click += new System.EventHandler(this.buttonAdditional_Click);
            // 
            // contextMenuStripTools
            // 
            this.contextMenuStripTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.encryptDecryptDataToolStripMenuItem,
            this.decodeUploadDataToolStripMenuItem});
            this.contextMenuStripTools.Name = "contextMenuStripTools";
            this.contextMenuStripTools.Size = new System.Drawing.Size(192, 48);
            // 
            // encryptDecryptDataToolStripMenuItem
            // 
            this.encryptDecryptDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vMSFileToolStripMenuItem,
            this.wholeFileToolStripMenuItem});
            this.encryptDecryptDataToolStripMenuItem.Image = global::VMSEditor.Properties.Resources.encrypt;
            this.encryptDecryptDataToolStripMenuItem.Name = "encryptDecryptDataToolStripMenuItem";
            this.encryptDecryptDataToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.encryptDecryptDataToolStripMenuItem.Text = "Encrypt/Decrypt Data";
            this.encryptDecryptDataToolStripMenuItem.ToolTipText = "Encrypt or decrypt data using DLC/Upload Data encryption";
            // 
            // vMSFileToolStripMenuItem
            // 
            this.vMSFileToolStripMenuItem.Name = "vMSFileToolStripMenuItem";
            this.vMSFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.vMSFileToolStripMenuItem.Text = "VMS File...";
            this.vMSFileToolStripMenuItem.ToolTipText = "Encrypt or decrypt a VMS file except its VMS header.";
            this.vMSFileToolStripMenuItem.Click += new System.EventHandler(this.vMSFileToolStripMenuItem_Click);
            // 
            // wholeFileToolStripMenuItem
            // 
            this.wholeFileToolStripMenuItem.Name = "wholeFileToolStripMenuItem";
            this.wholeFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.wholeFileToolStripMenuItem.Text = "Whole File...";
            this.wholeFileToolStripMenuItem.ToolTipText = "Encrypt or decrypt the whole file.";
            this.wholeFileToolStripMenuItem.Click += new System.EventHandler(this.wholeFileToolStripMenuItem_Click);
            // 
            // decodeUploadDataToolStripMenuItem
            // 
            this.decodeUploadDataToolStripMenuItem.Name = "decodeUploadDataToolStripMenuItem";
            this.decodeUploadDataToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.decodeUploadDataToolStripMenuItem.Text = "Decode Upload Data...";
            this.decodeUploadDataToolStripMenuItem.ToolTipText = "Decode and decrypt raw data from the HTML page embedded in Upload Data VMS files." +
    "";
            this.decodeUploadDataToolStripMenuItem.Click += new System.EventHandler(this.decodeUploadDataToolStripMenuItem_Click);
            // 
            // ProgramModeSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(427, 220);
            this.Controls.Add(this.buttonAdditional);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.buttonWorldRankingsEditor);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.buttonChallengeResultEditor);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonVMIEditor);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.buttonDLCEditor);
            this.Controls.Add(this.buttonChaoEditor);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "ProgramModeSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SA1 VMS Editor";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.contextMenuStripTools.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonChaoEditor;
		private System.Windows.Forms.Button buttonDLCEditor;
		private System.Windows.Forms.Button buttonOpenFile;
		private System.Windows.Forms.Button buttonVMIEditor;
		private System.Windows.Forms.Button buttonExit;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.PictureBox pictureBox4;
		private System.Windows.Forms.PictureBox pictureBox5;
		private System.Windows.Forms.Button buttonChallengeResultEditor;
		private System.Windows.Forms.PictureBox pictureBox7;
		private System.Windows.Forms.Button buttonWorldRankingsEditor;
		private System.Windows.Forms.PictureBox pictureBox6;
		private System.Windows.Forms.Button buttonAdditional;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripTools;
		private System.Windows.Forms.ToolStripMenuItem encryptDecryptDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem vMSFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem wholeFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem decodeUploadDataToolStripMenuItem;
	}
}