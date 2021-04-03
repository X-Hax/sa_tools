namespace TextureEditor
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label labelOriginalSize;
            System.Windows.Forms.Label labelX;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPVMXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPAKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsPVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsGVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsPVMXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsPAKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.importAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMipmapsToAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makePCCompatibleGVMsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alphaSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enablePAKAlphaForAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disablePAKAlphaForAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textureDownButton = new System.Windows.Forms.Button();
            this.textureUpButton = new System.Windows.Forms.Button();
            this.removeTextureButton = new System.Windows.Forms.Button();
            this.addTextureButton = new System.Windows.Forms.Button();
            this.checkBoxPAKUseAlpha = new System.Windows.Forms.CheckBox();
            this.numericUpDownOrigSizeY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownOrigSizeX = new System.Windows.Forms.NumericUpDown();
            this.hexIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.indexTextBox = new System.Windows.Forms.TextBox();
            this.mipmapCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textureSizeLabel = new System.Windows.Forms.Label();
            this.textureImage = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.exportButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.dataFormatLabel = new System.Windows.Forms.Label();
            this.pixelFormatLabel = new System.Windows.Forms.Label();
            this.globalIndex = new System.Windows.Forms.NumericUpDown();
            this.textureName = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.dummyPanel = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            labelOriginalSize = new System.Windows.Forms.Label();
            labelX = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOrigSizeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOrigSizeX)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureImage)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.globalIndex)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 32);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(38, 13);
            label1.TabIndex = 0;
            label1.Text = "Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 57);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(69, 13);
            label2.TabIndex = 2;
            label2.Text = "Global Index:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(3, 6);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(36, 13);
            label3.TabIndex = 8;
            label3.Text = "Index:";
            // 
            // labelOriginalSize
            // 
            labelOriginalSize.AutoSize = true;
            labelOriginalSize.Location = new System.Drawing.Point(3, 83);
            labelOriginalSize.Name = "labelOriginalSize";
            labelOriginalSize.Size = new System.Drawing.Size(68, 13);
            labelOriginalSize.TabIndex = 11;
            labelOriginalSize.Text = "Original Size:";
            // 
            // labelX
            // 
            labelX.AutoSize = true;
            labelX.Location = new System.Drawing.Point(136, 83);
            labelX.Name = "labelX";
            labelX.Size = new System.Drawing.Size(12, 13);
            labelX.TabIndex = 14;
            labelX.Text = "x";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
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
            this.toolStripSeparator3,
            this.importAllToolStripMenuItem,
            this.exportAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.recentFilesToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newPVMToolStripMenuItem,
            this.newGVMToolStripMenuItem,
            this.newPVMXToolStripMenuItem,
            this.newPAKToolStripMenuItem});
            this.newToolStripMenuItem.Image = global::TextureEditor.Properties.Resources._new;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // newPVMToolStripMenuItem
            // 
            this.newPVMToolStripMenuItem.Name = "newPVMToolStripMenuItem";
            this.newPVMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newPVMToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.newPVMToolStripMenuItem.Text = "&PVM";
            this.newPVMToolStripMenuItem.Click += new System.EventHandler(this.newPVMToolStripMenuItem_Click);
            // 
            // newGVMToolStripMenuItem
            // 
            this.newGVMToolStripMenuItem.Name = "newGVMToolStripMenuItem";
            this.newGVMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
            this.newGVMToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.newGVMToolStripMenuItem.Text = "&GVM";
            this.newGVMToolStripMenuItem.Click += new System.EventHandler(this.newGVMToolStripMenuItem_Click);
            // 
            // newPVMXToolStripMenuItem
            // 
            this.newPVMXToolStripMenuItem.Name = "newPVMXToolStripMenuItem";
            this.newPVMXToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.N)));
            this.newPVMXToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.newPVMXToolStripMenuItem.Text = "PVM&X";
            this.newPVMXToolStripMenuItem.Click += new System.EventHandler(this.newPVMXToolStripMenuItem_Click);
            // 
            // newPAKToolStripMenuItem
            // 
            this.newPAKToolStripMenuItem.Name = "newPAKToolStripMenuItem";
            this.newPAKToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
            this.newPAKToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.newPAKToolStripMenuItem.Text = "PA&K";
            this.newPAKToolStripMenuItem.Click += new System.EventHandler(this.newPAKToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsPVMToolStripMenuItem,
            this.saveAsGVMToolStripMenuItem,
            this.saveAsPVMXToolStripMenuItem,
            this.saveAsPAKToolStripMenuItem});
            this.saveAsToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.saveas;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveAsPVMToolStripMenuItem
            // 
            this.saveAsPVMToolStripMenuItem.Name = "saveAsPVMToolStripMenuItem";
            this.saveAsPVMToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.saveAsPVMToolStripMenuItem.Text = "&PVM";
            this.saveAsPVMToolStripMenuItem.Click += new System.EventHandler(this.saveAsPVMToolStripMenuItem_Click);
            // 
            // saveAsGVMToolStripMenuItem
            // 
            this.saveAsGVMToolStripMenuItem.Name = "saveAsGVMToolStripMenuItem";
            this.saveAsGVMToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.saveAsGVMToolStripMenuItem.Text = "&GVM";
            this.saveAsGVMToolStripMenuItem.Click += new System.EventHandler(this.saveAsGVMToolStripMenuItem_Click);
            // 
            // saveAsPVMXToolStripMenuItem
            // 
            this.saveAsPVMXToolStripMenuItem.Name = "saveAsPVMXToolStripMenuItem";
            this.saveAsPVMXToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.saveAsPVMXToolStripMenuItem.Text = "PVM&X";
            this.saveAsPVMXToolStripMenuItem.Click += new System.EventHandler(this.saveAsPVMXToolStripMenuItem_Click);
            // 
            // saveAsPAKToolStripMenuItem
            // 
            this.saveAsPAKToolStripMenuItem.Name = "saveAsPAKToolStripMenuItem";
            this.saveAsPAKToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.saveAsPAKToolStripMenuItem.Text = "PA&K";
            this.saveAsPAKToolStripMenuItem.Click += new System.EventHandler(this.saveAsPAKToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(220, 6);
            // 
            // importAllToolStripMenuItem
            // 
            this.importAllToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.import;
            this.importAllToolStripMenuItem.Name = "importAllToolStripMenuItem";
            this.importAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.importAllToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.importAllToolStripMenuItem.Text = "&Import texture pack...";
            this.importAllToolStripMenuItem.Click += new System.EventHandler(this.importAllToolStripMenuItem_Click);
            // 
            // exportAllToolStripMenuItem
            // 
            this.exportAllToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.export;
            this.exportAllToolStripMenuItem.Name = "exportAllToolStripMenuItem";
            this.exportAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportAllToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.exportAllToolStripMenuItem.Text = "&Export texture pack...";
            this.exportAllToolStripMenuItem.Click += new System.EventHandler(this.exportAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
            // 
            // recentFilesToolStripMenuItem
            // 
            this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
            this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.recentFilesToolStripMenuItem.Text = "&Recent Files";
            this.recentFilesToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recentFilesToolStripMenuItem_DropDownItemClicked);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(220, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMipmapsToAllToolStripMenuItem,
            this.makePCCompatibleGVMsToolStripMenuItem,
            this.alphaSortingToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // addMipmapsToAllToolStripMenuItem
            // 
            this.addMipmapsToAllToolStripMenuItem.Name = "addMipmapsToAllToolStripMenuItem";
            this.addMipmapsToAllToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.addMipmapsToAllToolStripMenuItem.Text = "Add &Mipmaps to All";
            this.addMipmapsToAllToolStripMenuItem.Click += new System.EventHandler(this.addMipmapsToAllToolStripMenuItem_Click);
            // 
            // makePCCompatibleGVMsToolStripMenuItem
            // 
            this.makePCCompatibleGVMsToolStripMenuItem.CheckOnClick = true;
            this.makePCCompatibleGVMsToolStripMenuItem.Name = "makePCCompatibleGVMsToolStripMenuItem";
            this.makePCCompatibleGVMsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.makePCCompatibleGVMsToolStripMenuItem.Text = "Make &PC-Compatible GVMs";
            this.makePCCompatibleGVMsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.makePCCompatibleGVMsToolStripMenuItem_CheckedChanged);
            // 
            // alphaSortingToolStripMenuItem
            // 
            this.alphaSortingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enablePAKAlphaForAllToolStripMenuItem,
            this.disablePAKAlphaForAllToolStripMenuItem});
            this.alphaSortingToolStripMenuItem.Enabled = false;
            this.alphaSortingToolStripMenuItem.Name = "alphaSortingToolStripMenuItem";
            this.alphaSortingToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.alphaSortingToolStripMenuItem.Text = "Alpha Sorting Flag";
            // 
            // enablePAKAlphaForAllToolStripMenuItem
            // 
            this.enablePAKAlphaForAllToolStripMenuItem.Name = "enablePAKAlphaForAllToolStripMenuItem";
            this.enablePAKAlphaForAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.enablePAKAlphaForAllToolStripMenuItem.Text = "Enable for All";
            this.enablePAKAlphaForAllToolStripMenuItem.Click += new System.EventHandler(this.enablePAKAlphaForAllToolStripMenuItem_Click);
            // 
            // disablePAKAlphaForAllToolStripMenuItem
            // 
            this.disablePAKAlphaForAllToolStripMenuItem.Name = "disablePAKAlphaForAllToolStripMenuItem";
            this.disablePAKAlphaForAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.disablePAKAlphaForAllToolStripMenuItem.Text = "Disable for All";
            this.disablePAKAlphaForAllToolStripMenuItem.Click += new System.EventHandler(this.disablePAKAlphaForAllToolStripMenuItem_Click);
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
            this.splitContainer1.Panel1MinSize = 206;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.checkBoxPAKUseAlpha);
            this.splitContainer1.Panel2.Controls.Add(labelX);
            this.splitContainer1.Panel2.Controls.Add(this.numericUpDownOrigSizeY);
            this.splitContainer1.Panel2.Controls.Add(this.numericUpDownOrigSizeX);
            this.splitContainer1.Panel2.Controls.Add(labelOriginalSize);
            this.splitContainer1.Panel2.Controls.Add(this.hexIndexCheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.indexTextBox);
            this.splitContainer1.Panel2.Controls.Add(label3);
            this.splitContainer1.Panel2.Controls.Add(this.mipmapCheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel2.Controls.Add(this.globalIndex);
            this.splitContainer1.Panel2.Controls.Add(label2);
            this.splitContainer1.Panel2.Controls.Add(this.textureName);
            this.splitContainer1.Panel2.Controls.Add(label1);
            this.splitContainer1.Panel2.SizeChanged += new System.EventHandler(this.SplitContainer1_Panel2_SizeChanged);
            this.splitContainer1.Size = new System.Drawing.Size(584, 515);
            this.splitContainer1.SplitterDistance = 206;
            this.splitContainer1.TabIndex = 1;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.IntegralHeight = false;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(202, 482);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.textureDownButton);
            this.panel1.Controls.Add(this.textureUpButton);
            this.panel1.Controls.Add(this.removeTextureButton);
            this.panel1.Controls.Add(this.addTextureButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 482);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(202, 29);
            this.panel1.TabIndex = 1;
            // 
            // textureDownButton
            // 
            this.textureDownButton.AutoSize = true;
            this.textureDownButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.textureDownButton.Enabled = false;
            this.textureDownButton.Location = new System.Drawing.Point(154, 3);
            this.textureDownButton.Name = "textureDownButton";
            this.textureDownButton.Size = new System.Drawing.Size(45, 23);
            this.textureDownButton.TabIndex = 3;
            this.textureDownButton.Text = "Down";
            this.textureDownButton.UseVisualStyleBackColor = true;
            this.textureDownButton.Click += new System.EventHandler(this.TextureDownButton_Click);
            // 
            // textureUpButton
            // 
            this.textureUpButton.AutoSize = true;
            this.textureUpButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.textureUpButton.Enabled = false;
            this.textureUpButton.Location = new System.Drawing.Point(117, 3);
            this.textureUpButton.Name = "textureUpButton";
            this.textureUpButton.Size = new System.Drawing.Size(31, 23);
            this.textureUpButton.TabIndex = 2;
            this.textureUpButton.Text = "Up";
            this.textureUpButton.UseVisualStyleBackColor = true;
            this.textureUpButton.Click += new System.EventHandler(this.TextureUpButton_Click);
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
            this.removeTextureButton.Click += new System.EventHandler(this.removeTextureButton_Click);
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
            this.addTextureButton.Click += new System.EventHandler(this.addTextureButton_Click);
            // 
            // checkBoxPAKUseAlpha
            // 
            this.checkBoxPAKUseAlpha.AutoSize = true;
            this.checkBoxPAKUseAlpha.Enabled = false;
            this.checkBoxPAKUseAlpha.Location = new System.Drawing.Point(272, 56);
            this.checkBoxPAKUseAlpha.Name = "checkBoxPAKUseAlpha";
            this.checkBoxPAKUseAlpha.Size = new System.Drawing.Size(89, 17);
            this.checkBoxPAKUseAlpha.TabIndex = 15;
            this.checkBoxPAKUseAlpha.Text = "Alpha Sorting";
            this.checkBoxPAKUseAlpha.UseVisualStyleBackColor = true;
            this.checkBoxPAKUseAlpha.CheckedChanged += new System.EventHandler(this.checkBoxPAKUseAlpha_CheckedChanged);
            // 
            // numericUpDownOrigSizeY
            // 
            this.numericUpDownOrigSizeY.Enabled = false;
            this.numericUpDownOrigSizeY.Location = new System.Drawing.Point(151, 81);
            this.numericUpDownOrigSizeY.Maximum = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            this.numericUpDownOrigSizeY.Name = "numericUpDownOrigSizeY";
            this.numericUpDownOrigSizeY.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownOrigSizeY.TabIndex = 13;
            this.numericUpDownOrigSizeY.ValueChanged += new System.EventHandler(this.numericUpDownOrigSizeY_ValueChanged);
            // 
            // numericUpDownOrigSizeX
            // 
            this.numericUpDownOrigSizeX.Enabled = false;
            this.numericUpDownOrigSizeX.Location = new System.Drawing.Point(78, 81);
            this.numericUpDownOrigSizeX.Maximum = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            this.numericUpDownOrigSizeX.Name = "numericUpDownOrigSizeX";
            this.numericUpDownOrigSizeX.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownOrigSizeX.TabIndex = 12;
            this.numericUpDownOrigSizeX.ValueChanged += new System.EventHandler(this.numericUpDownOrigSizeX_ValueChanged);
            // 
            // hexIndexCheckBox
            // 
            this.hexIndexCheckBox.AutoSize = true;
            this.hexIndexCheckBox.Location = new System.Drawing.Point(184, 5);
            this.hexIndexCheckBox.Name = "hexIndexCheckBox";
            this.hexIndexCheckBox.Size = new System.Drawing.Size(45, 17);
            this.hexIndexCheckBox.TabIndex = 10;
            this.hexIndexCheckBox.Text = "Hex";
            this.hexIndexCheckBox.UseVisualStyleBackColor = true;
            this.hexIndexCheckBox.CheckedChanged += new System.EventHandler(this.HexIndexCheckBox_CheckedChanged);
            // 
            // indexTextBox
            // 
            this.indexTextBox.Location = new System.Drawing.Point(78, 3);
            this.indexTextBox.Name = "indexTextBox";
            this.indexTextBox.ReadOnly = true;
            this.indexTextBox.Size = new System.Drawing.Size(100, 20);
            this.indexTextBox.TabIndex = 9;
            this.indexTextBox.Text = "0";
            // 
            // mipmapCheckBox
            // 
            this.mipmapCheckBox.AutoSize = true;
            this.mipmapCheckBox.Enabled = false;
            this.mipmapCheckBox.Location = new System.Drawing.Point(204, 56);
            this.mipmapCheckBox.Name = "mipmapCheckBox";
            this.mipmapCheckBox.Size = new System.Drawing.Size(63, 17);
            this.mipmapCheckBox.TabIndex = 7;
            this.mipmapCheckBox.Text = "Mipmap";
            this.mipmapCheckBox.UseVisualStyleBackColor = true;
            this.mipmapCheckBox.CheckedChanged += new System.EventHandler(this.mipmapCheckBox_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.textureSizeLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textureImage, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.dataFormatLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pixelFormatLabel, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 106);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(129, 157);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // textureSizeLabel
            // 
            this.textureSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textureSizeLabel.AutoSize = true;
            this.textureSizeLabel.Location = new System.Drawing.Point(0, 41);
            this.textureSizeLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.textureSizeLabel.Name = "textureSizeLabel";
            this.textureSizeLabel.Size = new System.Drawing.Size(75, 13);
            this.textureSizeLabel.TabIndex = 11;
            this.textureSizeLabel.Text = "Actual Size: ---";
            this.textureSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textureImage
            // 
            this.textureImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textureImage.Location = new System.Drawing.Point(0, 58);
            this.textureImage.Margin = new System.Windows.Forms.Padding(0);
            this.textureImage.Name = "textureImage";
            this.textureImage.Size = new System.Drawing.Size(64, 64);
            this.textureImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.textureImage.TabIndex = 4;
            this.textureImage.TabStop = false;
            this.textureImage.DragDrop += new System.Windows.Forms.DragEventHandler(this.textureImage_DragDrop);
            this.textureImage.DragEnter += new System.Windows.Forms.DragEventHandler(this.textureImage_DragEnter);
            this.textureImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textureImage_MouseClick);
            this.textureImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textureImage_MouseMove);
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.exportButton);
            this.panel2.Controls.Add(this.importButton);
            this.panel2.Location = new System.Drawing.Point(3, 125);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(123, 29);
            this.panel2.TabIndex = 5;
            // 
            // exportButton
            // 
            this.exportButton.AutoSize = true;
            this.exportButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.exportButton.Enabled = false;
            this.exportButton.Location = new System.Drawing.Point(64, 3);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(56, 23);
            this.exportButton.TabIndex = 7;
            this.exportButton.Text = "Export...";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // importButton
            // 
            this.importButton.AutoSize = true;
            this.importButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.importButton.Enabled = false;
            this.importButton.Location = new System.Drawing.Point(3, 3);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(55, 23);
            this.importButton.TabIndex = 6;
            this.importButton.Text = "Import...";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // dataFormatLabel
            // 
            this.dataFormatLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dataFormatLabel.AutoSize = true;
            this.dataFormatLabel.Location = new System.Drawing.Point(0, 3);
            this.dataFormatLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.dataFormatLabel.Name = "dataFormatLabel";
            this.dataFormatLabel.Size = new System.Drawing.Size(117, 13);
            this.dataFormatLabel.TabIndex = 6;
            this.dataFormatLabel.Text = "Data Format: Unknown";
            this.dataFormatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pixelFormatLabel
            // 
            this.pixelFormatLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pixelFormatLabel.AutoSize = true;
            this.pixelFormatLabel.Location = new System.Drawing.Point(0, 22);
            this.pixelFormatLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.pixelFormatLabel.Name = "pixelFormatLabel";
            this.pixelFormatLabel.Size = new System.Drawing.Size(116, 13);
            this.pixelFormatLabel.TabIndex = 7;
            this.pixelFormatLabel.Text = "Pixel Format: Unknown";
            this.pixelFormatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // globalIndex
            // 
            this.globalIndex.Enabled = false;
            this.globalIndex.Location = new System.Drawing.Point(78, 55);
            this.globalIndex.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.globalIndex.Name = "globalIndex";
            this.globalIndex.Size = new System.Drawing.Size(120, 20);
            this.globalIndex.TabIndex = 3;
            this.globalIndex.ValueChanged += new System.EventHandler(this.globalIndex_ValueChanged);
            // 
            // textureName
            // 
            this.textureName.Enabled = false;
            this.textureName.Location = new System.Drawing.Point(78, 29);
            this.textureName.Name = "textureName";
            this.textureName.Size = new System.Drawing.Size(185, 20);
            this.textureName.TabIndex = 1;
            this.textureName.TextChanged += new System.EventHandler(this.textureName_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(584, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(57, 17);
            this.toolStripStatusLabel1.Text = "0 textures";
            // 
            // dummyPanel
            // 
            this.dummyPanel.Enabled = false;
            this.dummyPanel.Location = new System.Drawing.Point(0, 0);
            this.dummyPanel.Margin = new System.Windows.Forms.Padding(0);
            this.dummyPanel.Name = "dummyPanel";
            this.dummyPanel.Size = new System.Drawing.Size(100, 100);
            this.dummyPanel.TabIndex = 3;
            this.dummyPanel.Visible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(103, 48);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.paste;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dummyPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "PVM Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOrigSizeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOrigSizeX)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureImage)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.globalIndex)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
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
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.CheckBox mipmapCheckBox;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addMipmapsToAllToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripMenuItem newPVMToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newGVMToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newPVMXToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsPVMToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsGVMToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsPVMXToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newPAKToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsPAKToolStripMenuItem;
		private System.Windows.Forms.Panel dummyPanel;
		private System.Windows.Forms.Label dataFormatLabel;
		private System.Windows.Forms.Label pixelFormatLabel;
		private System.Windows.Forms.CheckBox hexIndexCheckBox;
		private System.Windows.Forms.TextBox indexTextBox;
		private System.Windows.Forms.Button textureUpButton;
		private System.Windows.Forms.Button textureDownButton;
		private System.Windows.Forms.Label textureSizeLabel;
		private System.Windows.Forms.ToolStripMenuItem makePCCompatibleGVMsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.NumericUpDown numericUpDownOrigSizeY;
		private System.Windows.Forms.NumericUpDown numericUpDownOrigSizeX;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.CheckBox checkBoxPAKUseAlpha;
		private System.Windows.Forms.ToolStripMenuItem alphaSortingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem enablePAKAlphaForAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem disablePAKAlphaForAllToolStripMenuItem;
	}
}

