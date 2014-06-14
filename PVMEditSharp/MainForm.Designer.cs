namespace PVMEditSharp
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.removeTextureButton = new System.Windows.Forms.Button();
			this.addTextureButton = new System.Windows.Forms.Button();
			this.textureName = new System.Windows.Forms.TextBox();
			this.globalIndex = new System.Windows.Forms.NumericUpDown();
			this.textureImage = new System.Windows.Forms.PictureBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.importButton = new System.Windows.Forms.Button();
			this.exportButton = new System.Windows.Forms.Button();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.globalIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textureImage)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 6);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(38, 13);
			label1.TabIndex = 0;
			label1.Text = "Name:";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(584, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exportAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.recentFilesToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.listBox1);
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.exportButton);
			this.splitContainer1.Panel2.Controls.Add(this.importButton);
			this.splitContainer1.Panel2.Controls.Add(this.panel2);
			this.splitContainer1.Panel2.Controls.Add(this.globalIndex);
			this.splitContainer1.Panel2.Controls.Add(label2);
			this.splitContainer1.Panel2.Controls.Add(this.textureName);
			this.splitContainer1.Panel2.Controls.Add(label1);
			this.splitContainer1.Size = new System.Drawing.Size(584, 538);
			this.splitContainer1.SplitterDistance = 194;
			this.splitContainer1.TabIndex = 1;
			// 
			// listBox1
			// 
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.IntegralHeight = false;
			this.listBox1.Location = new System.Drawing.Point(0, 0);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(190, 505);
			this.listBox1.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.removeTextureButton);
			this.panel1.Controls.Add(this.addTextureButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 505);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(190, 29);
			this.panel1.TabIndex = 1;
			// 
			// removeTextureButton
			// 
			this.removeTextureButton.AutoSize = true;
			this.removeTextureButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.removeTextureButton.Enabled = false;
			this.removeTextureButton.Location = new System.Drawing.Point(54, 3);
			this.removeTextureButton.Name = "removeTextureButton";
			this.removeTextureButton.Size = new System.Drawing.Size(57, 23);
			this.removeTextureButton.TabIndex = 1;
			this.removeTextureButton.Text = "Remove";
			this.removeTextureButton.UseVisualStyleBackColor = true;
			// 
			// addTextureButton
			// 
			this.addTextureButton.AutoSize = true;
			this.addTextureButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.addTextureButton.Location = new System.Drawing.Point(3, 3);
			this.addTextureButton.Name = "addTextureButton";
			this.addTextureButton.Size = new System.Drawing.Size(45, 23);
			this.addTextureButton.TabIndex = 0;
			this.addTextureButton.Text = "Add...";
			this.addTextureButton.UseVisualStyleBackColor = true;
			// 
			// textureName
			// 
			this.textureName.Enabled = false;
			this.textureName.Location = new System.Drawing.Point(78, 3);
			this.textureName.Name = "textureName";
			this.textureName.Size = new System.Drawing.Size(185, 20);
			this.textureName.TabIndex = 1;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(3, 31);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(69, 13);
			label2.TabIndex = 2;
			label2.Text = "Global Index:";
			// 
			// globalIndex
			// 
			this.globalIndex.Enabled = false;
			this.globalIndex.Location = new System.Drawing.Point(78, 29);
			this.globalIndex.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
			this.globalIndex.Name = "globalIndex";
			this.globalIndex.Size = new System.Drawing.Size(120, 20);
			this.globalIndex.TabIndex = 3;
			// 
			// textureImage
			// 
			this.textureImage.Location = new System.Drawing.Point(0, 0);
			this.textureImage.Margin = new System.Windows.Forms.Padding(0);
			this.textureImage.Name = "textureImage";
			this.textureImage.Size = new System.Drawing.Size(100, 50);
			this.textureImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.textureImage.TabIndex = 4;
			this.textureImage.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.AutoScroll = true;
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel2.Controls.Add(this.textureImage);
			this.panel2.Location = new System.Drawing.Point(6, 55);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(256, 256);
			this.panel2.TabIndex = 5;
			// 
			// importButton
			// 
			this.importButton.AutoSize = true;
			this.importButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.importButton.Enabled = false;
			this.importButton.Location = new System.Drawing.Point(6, 317);
			this.importButton.Name = "importButton";
			this.importButton.Size = new System.Drawing.Size(55, 23);
			this.importButton.TabIndex = 6;
			this.importButton.Text = "Import...";
			this.importButton.UseVisualStyleBackColor = true;
			// 
			// exportButton
			// 
			this.exportButton.AutoSize = true;
			this.exportButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.exportButton.Enabled = false;
			this.exportButton.Location = new System.Drawing.Point(67, 317);
			this.exportButton.Name = "exportButton";
			this.exportButton.Size = new System.Drawing.Size(56, 23);
			this.exportButton.TabIndex = 7;
			this.exportButton.Text = "Export...";
			this.exportButton.UseVisualStyleBackColor = true;
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.newToolStripMenuItem.Text = "&New";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.S)));
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.saveAsToolStripMenuItem.Text = "Save &As...";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
			// 
			// recentFilesToolStripMenuItem
			// 
			this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
			this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.recentFilesToolStripMenuItem.Text = "&Recent Files";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(183, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			// 
			// exportAllToolStripMenuItem
			// 
			this.exportAllToolStripMenuItem.Name = "exportAllToolStripMenuItem";
			this.exportAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.exportAllToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.exportAllToolStripMenuItem.Text = "&Export All...";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 562);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "PVM Editor";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.globalIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textureImage)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button removeTextureButton;
		private System.Windows.Forms.Button addTextureButton;
		private System.Windows.Forms.TextBox textureName;
		private System.Windows.Forms.NumericUpDown globalIndex;
		private System.Windows.Forms.PictureBox textureImage;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button exportButton;
		private System.Windows.Forms.Button importButton;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem recentFilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportAllToolStripMenuItem;
	}
}

