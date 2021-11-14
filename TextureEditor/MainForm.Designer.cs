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
            this.importTexturePackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTexturePackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMipmapsToAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highQualityGVMsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alphaSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enablePAKAlphaForAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disablePAKAlphaForAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.palettedTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compatibleGVPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exportMaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPalettedIndexedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPalettedFullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureFilteringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.importExportPanel = new System.Windows.Forms.Panel();
            this.exportButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.dataFormatLabel = new System.Windows.Forms.Label();
            this.pixelFormatLabel = new System.Windows.Forms.Label();
            this.palettePreview = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonLoadPalette = new System.Windows.Forms.Button();
            this.buttonSavePalette = new System.Windows.Forms.Button();
            this.buttonResetPalette = new System.Windows.Forms.Button();
            this.panelPaletteInfo = new System.Windows.Forms.Panel();
            this.labelPaletteBankPreview = new System.Windows.Forms.Label();
            this.numericUpDownStartColor = new System.Windows.Forms.NumericUpDown();
            this.labelStartColor = new System.Windows.Forms.Label();
            this.labelStartBank = new System.Windows.Forms.Label();
            this.numericUpDownStartBank = new System.Windows.Forms.NumericUpDown();
            this.comboBoxCurrentPaletteBank = new System.Windows.Forms.ComboBox();
            this.labelPaletteFormat = new System.Windows.Forms.Label();
            this.texturePreviewZoomTrackBar = new System.Windows.Forms.TrackBar();
            this.textureImage = new System.Windows.Forms.PictureBox();
            this.globalIndex = new System.Windows.Forms.NumericUpDown();
            this.textureName = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.dummyPanel = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelZoomInfo = new System.Windows.Forms.Label();
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
            this.importExportPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.palettePreview)).BeginInit();
            this.panel3.SuspendLayout();
            this.panelPaletteInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartBank)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.texturePreviewZoomTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textureImage)).BeginInit();
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
            labelOriginalSize.Size = new System.Drawing.Size(30, 13);
            labelOriginalSize.TabIndex = 11;
            labelOriginalSize.Text = "Size:";
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
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(604, 24);
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
            this.importTexturePackToolStripMenuItem,
            this.exportTexturePackToolStripMenuItem,
            this.toolStripSeparator1,
            this.recentFilesToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
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
            // importTexturePackToolStripMenuItem
            // 
            this.importTexturePackToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.import;
            this.importTexturePackToolStripMenuItem.Name = "importTexturePackToolStripMenuItem";
            this.importTexturePackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.importTexturePackToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.importTexturePackToolStripMenuItem.Text = "&Import texture pack...";
            this.importTexturePackToolStripMenuItem.ToolTipText = "Import a folder texture pack with an index file.";
            this.importTexturePackToolStripMenuItem.Click += new System.EventHandler(this.importTexturePackToolStripMenuItem_Click);
            // 
            // exportTexturePackToolStripMenuItem
            // 
            this.exportTexturePackToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.export;
            this.exportTexturePackToolStripMenuItem.Name = "exportTexturePackToolStripMenuItem";
            this.exportTexturePackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportTexturePackToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.exportTexturePackToolStripMenuItem.Text = "&Export texture pack...";
            this.exportTexturePackToolStripMenuItem.ToolTipText = "Export a folder texture pack with an index file.";
            this.exportTexturePackToolStripMenuItem.Click += new System.EventHandler(this.exportTexturePackToolStripMenuItem_Click);
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
            this.highQualityGVMsToolStripMenuItem,
            this.alphaSortingToolStripMenuItem,
            this.palettedTexturesToolStripMenuItem,
            this.textureFilteringToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 22);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // addMipmapsToAllToolStripMenuItem
            // 
            this.addMipmapsToAllToolStripMenuItem.Name = "addMipmapsToAllToolStripMenuItem";
            this.addMipmapsToAllToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.addMipmapsToAllToolStripMenuItem.Text = "Add &Mipmaps to All";
            this.addMipmapsToAllToolStripMenuItem.ToolTipText = "Enable the Mipmap flag for all textures.";
            this.addMipmapsToAllToolStripMenuItem.Click += new System.EventHandler(this.addMipmapsToAllToolStripMenuItem_Click);
            // 
            // highQualityGVMsToolStripMenuItem
            // 
            this.highQualityGVMsToolStripMenuItem.CheckOnClick = true;
            this.highQualityGVMsToolStripMenuItem.Name = "highQualityGVMsToolStripMenuItem";
            this.highQualityGVMsToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.highQualityGVMsToolStripMenuItem.Text = "High &Quality GVMs (GC only)";
            this.highQualityGVMsToolStripMenuItem.ToolTipText = "Enable support for lossless textures in GVM files. Only supported by SA2B Gamecub" +
    "e.";
            this.highQualityGVMsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.highQualityGVMsToolStripMenuItem_CheckedChanged);
            // 
            // alphaSortingToolStripMenuItem
            // 
            this.alphaSortingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enablePAKAlphaForAllToolStripMenuItem,
            this.disablePAKAlphaForAllToolStripMenuItem});
            this.alphaSortingToolStripMenuItem.Enabled = false;
            this.alphaSortingToolStripMenuItem.Name = "alphaSortingToolStripMenuItem";
            this.alphaSortingToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.alphaSortingToolStripMenuItem.Text = "Alpha Sorting Flag";
            this.alphaSortingToolStripMenuItem.ToolTipText = "Transparency flags for SA2 PC PAKs.";
            // 
            // enablePAKAlphaForAllToolStripMenuItem
            // 
            this.enablePAKAlphaForAllToolStripMenuItem.Name = "enablePAKAlphaForAllToolStripMenuItem";
            this.enablePAKAlphaForAllToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.enablePAKAlphaForAllToolStripMenuItem.Text = "Enable for All";
            this.enablePAKAlphaForAllToolStripMenuItem.ToolTipText = "Set the RGB5A3 pixel format for all PAK items.";
            this.enablePAKAlphaForAllToolStripMenuItem.Click += new System.EventHandler(this.enablePAKAlphaForAllToolStripMenuItem_Click);
            // 
            // disablePAKAlphaForAllToolStripMenuItem
            // 
            this.disablePAKAlphaForAllToolStripMenuItem.Name = "disablePAKAlphaForAllToolStripMenuItem";
            this.disablePAKAlphaForAllToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.disablePAKAlphaForAllToolStripMenuItem.Text = "Disable for All";
            this.disablePAKAlphaForAllToolStripMenuItem.ToolTipText = "Remove the RGB5A3 format flag in all PAK items.";
            this.disablePAKAlphaForAllToolStripMenuItem.Click += new System.EventHandler(this.disablePAKAlphaForAllToolStripMenuItem_Click);
            // 
            // palettedTexturesToolStripMenuItem
            // 
            this.palettedTexturesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compatibleGVPToolStripMenuItem,
            this.toolStripSeparator4,
            this.exportMaskToolStripMenuItem,
            this.exportPalettedIndexedToolStripMenuItem,
            this.exportPalettedFullToolStripMenuItem});
            this.palettedTexturesToolStripMenuItem.Name = "palettedTexturesToolStripMenuItem";
            this.palettedTexturesToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.palettedTexturesToolStripMenuItem.Text = "Paletted Textures";
            // 
            // compatibleGVPToolStripMenuItem
            // 
            this.compatibleGVPToolStripMenuItem.Checked = true;
            this.compatibleGVPToolStripMenuItem.CheckOnClick = true;
            this.compatibleGVPToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.compatibleGVPToolStripMenuItem.Name = "compatibleGVPToolStripMenuItem";
            this.compatibleGVPToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.compatibleGVPToolStripMenuItem.Text = "SADX/SA2 compatible GVPs";
            this.compatibleGVPToolStripMenuItem.ToolTipText = "Use ARGB4444 and ARGB1555 pixel formats for Gamecube paletted textures. Required " +
    "for SA2 PC, SADX GC and SA2B GC.";
            this.compatibleGVPToolStripMenuItem.CheckedChanged += new System.EventHandler(this.compatibleGVPToolStripMenuItem_CheckedChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(218, 6);
            // 
            // exportMaskToolStripMenuItem
            // 
            this.exportMaskToolStripMenuItem.Checked = true;
            this.exportMaskToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportMaskToolStripMenuItem.Name = "exportMaskToolStripMenuItem";
            this.exportMaskToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.exportMaskToolStripMenuItem.Text = "Export Mask (indexed)";
            this.exportMaskToolStripMenuItem.ToolTipText = "Export indexed textures with the default palette.";
            this.exportMaskToolStripMenuItem.Click += new System.EventHandler(this.exportMaskToolStripMenuItem_Click);
            // 
            // exportPalettedIndexedToolStripMenuItem
            // 
            this.exportPalettedIndexedToolStripMenuItem.Name = "exportPalettedIndexedToolStripMenuItem";
            this.exportPalettedIndexedToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.exportPalettedIndexedToolStripMenuItem.Text = "Export Paletted (indexed)";
            this.exportPalettedIndexedToolStripMenuItem.ToolTipText = "Export indexed textures with the currently applied palette set.";
            this.exportPalettedIndexedToolStripMenuItem.Click += new System.EventHandler(this.exportPalettedIndexedToolStripMenuItem_Click);
            // 
            // exportPalettedFullToolStripMenuItem
            // 
            this.exportPalettedFullToolStripMenuItem.Name = "exportPalettedFullToolStripMenuItem";
            this.exportPalettedFullToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.exportPalettedFullToolStripMenuItem.Text = "Export Paletted (full color)";
            this.exportPalettedFullToolStripMenuItem.ToolTipText = "Export indexed textures as non-indexed images after applying the current palette." +
    "";
            this.exportPalettedFullToolStripMenuItem.Click += new System.EventHandler(this.exportPalettedFullToolStripMenuItem_Click);
            // 
            // textureFilteringToolStripMenuItem
            // 
            this.textureFilteringToolStripMenuItem.Checked = true;
            this.textureFilteringToolStripMenuItem.CheckOnClick = true;
            this.textureFilteringToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.textureFilteringToolStripMenuItem.Name = "textureFilteringToolStripMenuItem";
            this.textureFilteringToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.textureFilteringToolStripMenuItem.Text = "Texture Filtering";
            this.textureFilteringToolStripMenuItem.ToolTipText = "Enable bicubic interpolation when resizing the preview image.";
            this.textureFilteringToolStripMenuItem.CheckedChanged += new System.EventHandler(this.textureFilteringToolStripMenuItem_CheckedChanged);
            this.textureFilteringToolStripMenuItem.Click += new System.EventHandler(this.textureFilteringToolStripMenuItem_Click);
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
            this.splitContainer1.Size = new System.Drawing.Size(604, 515);
            this.splitContainer1.SplitterDistance = 212;
            this.splitContainer1.TabIndex = 1;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.IntegralHeight = false;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(208, 482);
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
            this.panel1.Size = new System.Drawing.Size(208, 29);
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
            this.checkBoxPAKUseAlpha.Click += new System.EventHandler(this.checkBoxPAKUseAlpha_Click);
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
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.textureSizeLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.importExportPanel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.dataFormatLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pixelFormatLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.palettePreview, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.panelPaletteInfo, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.texturePreviewZoomTrackBar, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textureImage, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelZoomInfo, 0, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 106);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 11;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(338, 365);
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
            this.textureSizeLabel.Visible = false;
            // 
            // importExportPanel
            // 
            this.importExportPanel.AutoSize = true;
            this.importExportPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.importExportPanel.Controls.Add(this.exportButton);
            this.importExportPanel.Controls.Add(this.importButton);
            this.importExportPanel.Location = new System.Drawing.Point(3, 195);
            this.importExportPanel.Name = "importExportPanel";
            this.importExportPanel.Size = new System.Drawing.Size(123, 29);
            this.importExportPanel.TabIndex = 5;
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
            // palettePreview
            // 
            this.palettePreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.palettePreview.Location = new System.Drawing.Point(0, 227);
            this.palettePreview.Margin = new System.Windows.Forms.Padding(0);
            this.palettePreview.Name = "palettePreview";
            this.palettePreview.Size = new System.Drawing.Size(33, 33);
            this.palettePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.palettePreview.TabIndex = 13;
            this.palettePreview.TabStop = false;
            this.palettePreview.Visible = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.buttonLoadPalette);
            this.panel3.Controls.Add(this.buttonSavePalette);
            this.panel3.Controls.Add(this.buttonResetPalette);
            this.panel3.Location = new System.Drawing.Point(3, 328);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(174, 34);
            this.panel3.TabIndex = 27;
            // 
            // buttonLoadPalette
            // 
            this.buttonLoadPalette.Location = new System.Drawing.Point(3, 3);
            this.buttonLoadPalette.Name = "buttonLoadPalette";
            this.buttonLoadPalette.Size = new System.Drawing.Size(52, 23);
            this.buttonLoadPalette.TabIndex = 17;
            this.buttonLoadPalette.Text = "Load...";
            this.buttonLoadPalette.UseVisualStyleBackColor = true;
            this.buttonLoadPalette.Visible = false;
            this.buttonLoadPalette.Click += new System.EventHandler(this.buttonLoadPalette_Click);
            // 
            // buttonSavePalette
            // 
            this.buttonSavePalette.Location = new System.Drawing.Point(61, 3);
            this.buttonSavePalette.Name = "buttonSavePalette";
            this.buttonSavePalette.Size = new System.Drawing.Size(52, 23);
            this.buttonSavePalette.TabIndex = 18;
            this.buttonSavePalette.Text = "Save...";
            this.buttonSavePalette.UseVisualStyleBackColor = true;
            this.buttonSavePalette.Visible = false;
            this.buttonSavePalette.Click += new System.EventHandler(this.buttonSavePalette_Click);
            // 
            // buttonResetPalette
            // 
            this.buttonResetPalette.Location = new System.Drawing.Point(119, 3);
            this.buttonResetPalette.Name = "buttonResetPalette";
            this.buttonResetPalette.Size = new System.Drawing.Size(52, 23);
            this.buttonResetPalette.TabIndex = 19;
            this.buttonResetPalette.Text = "Reset";
            this.buttonResetPalette.UseVisualStyleBackColor = true;
            this.buttonResetPalette.Visible = false;
            this.buttonResetPalette.Click += new System.EventHandler(this.buttonResetPalette_Click);
            // 
            // panelPaletteInfo
            // 
            this.panelPaletteInfo.Controls.Add(this.labelPaletteBankPreview);
            this.panelPaletteInfo.Controls.Add(this.numericUpDownStartColor);
            this.panelPaletteInfo.Controls.Add(this.labelStartColor);
            this.panelPaletteInfo.Controls.Add(this.labelStartBank);
            this.panelPaletteInfo.Controls.Add(this.numericUpDownStartBank);
            this.panelPaletteInfo.Controls.Add(this.comboBoxCurrentPaletteBank);
            this.panelPaletteInfo.Controls.Add(this.labelPaletteFormat);
            this.panelPaletteInfo.Location = new System.Drawing.Point(183, 230);
            this.panelPaletteInfo.Name = "panelPaletteInfo";
            this.panelPaletteInfo.Size = new System.Drawing.Size(152, 92);
            this.panelPaletteInfo.TabIndex = 20;
            this.panelPaletteInfo.Visible = false;
            // 
            // labelPaletteBankPreview
            // 
            this.labelPaletteBankPreview.AutoSize = true;
            this.labelPaletteBankPreview.Location = new System.Drawing.Point(3, 24);
            this.labelPaletteBankPreview.Name = "labelPaletteBankPreview";
            this.labelPaletteBankPreview.Size = new System.Drawing.Size(71, 13);
            this.labelPaletteBankPreview.TabIndex = 21;
            this.labelPaletteBankPreview.Text = "Palette Bank:";
            this.labelPaletteBankPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDownStartColor
            // 
            this.numericUpDownStartColor.Location = new System.Drawing.Point(84, 62);
            this.numericUpDownStartColor.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numericUpDownStartColor.Name = "numericUpDownStartColor";
            this.numericUpDownStartColor.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownStartColor.TabIndex = 25;
            this.numericUpDownStartColor.ValueChanged += new System.EventHandler(this.numericUpDownStartColor_ValueChanged);
            // 
            // labelStartColor
            // 
            this.labelStartColor.AutoSize = true;
            this.labelStartColor.Location = new System.Drawing.Point(81, 45);
            this.labelStartColor.Name = "labelStartColor";
            this.labelStartColor.Size = new System.Drawing.Size(59, 13);
            this.labelStartColor.TabIndex = 24;
            this.labelStartColor.Text = "Start Color:";
            // 
            // labelStartBank
            // 
            this.labelStartBank.AutoSize = true;
            this.labelStartBank.Location = new System.Drawing.Point(3, 45);
            this.labelStartBank.Name = "labelStartBank";
            this.labelStartBank.Size = new System.Drawing.Size(60, 13);
            this.labelStartBank.TabIndex = 23;
            this.labelStartBank.Text = "Start Bank:";
            // 
            // numericUpDownStartBank
            // 
            this.numericUpDownStartBank.Location = new System.Drawing.Point(4, 62);
            this.numericUpDownStartBank.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.numericUpDownStartBank.Name = "numericUpDownStartBank";
            this.numericUpDownStartBank.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownStartBank.TabIndex = 22;
            this.numericUpDownStartBank.ValueChanged += new System.EventHandler(this.numericUpDownStartBank_ValueChanged);
            // 
            // comboBoxCurrentPaletteBank
            // 
            this.comboBoxCurrentPaletteBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCurrentPaletteBank.FormattingEnabled = true;
            this.comboBoxCurrentPaletteBank.Location = new System.Drawing.Point(85, 21);
            this.comboBoxCurrentPaletteBank.Name = "comboBoxCurrentPaletteBank";
            this.comboBoxCurrentPaletteBank.Size = new System.Drawing.Size(60, 21);
            this.comboBoxCurrentPaletteBank.TabIndex = 20;
            this.comboBoxCurrentPaletteBank.SelectedIndexChanged += new System.EventHandler(this.comboBoxCurrentPaletteBank_SelectedIndexChanged);
            // 
            // labelPaletteFormat
            // 
            this.labelPaletteFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPaletteFormat.AutoSize = true;
            this.labelPaletteFormat.Location = new System.Drawing.Point(3, 5);
            this.labelPaletteFormat.Name = "labelPaletteFormat";
            this.labelPaletteFormat.Size = new System.Drawing.Size(46, 13);
            this.labelPaletteFormat.TabIndex = 12;
            this.labelPaletteFormat.Text = "Palette: ";
            this.labelPaletteFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelPaletteFormat.Visible = false;
            // 
            // texturePreviewZoomTrackBar
            // 
            this.texturePreviewZoomTrackBar.Location = new System.Drawing.Point(3, 60);
            this.texturePreviewZoomTrackBar.Maximum = 8;
            this.texturePreviewZoomTrackBar.Name = "texturePreviewZoomTrackBar";
            this.texturePreviewZoomTrackBar.Size = new System.Drawing.Size(171, 45);
            this.texturePreviewZoomTrackBar.TabIndex = 28;
            this.texturePreviewZoomTrackBar.Value = 4;
            this.texturePreviewZoomTrackBar.Scroll += new System.EventHandler(this.texturePreviewZoomTrackBar_Scroll);
            // 
            // textureImage
            // 
            this.textureImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textureImage.Location = new System.Drawing.Point(0, 128);
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
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(604, 22);
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
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(111, 64);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(110, 30);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = global::TextureEditor.Properties.Resources.paste;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(110, 30);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // labelZoomInfo
            // 
            this.labelZoomInfo.AutoSize = true;
            this.labelZoomInfo.Location = new System.Drawing.Point(3, 108);
            this.labelZoomInfo.Name = "labelZoomInfo";
            this.labelZoomInfo.Size = new System.Drawing.Size(60, 13);
            this.labelZoomInfo.TabIndex = 29;
            this.labelZoomInfo.Text = "Zoom: N/A";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 561);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dummyPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
            this.importExportPanel.ResumeLayout(false);
            this.importExportPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.palettePreview)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panelPaletteInfo.ResumeLayout(false);
            this.panelPaletteInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartBank)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.texturePreviewZoomTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textureImage)).EndInit();
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
		private System.Windows.Forms.Panel importExportPanel;
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
		private System.Windows.Forms.ToolStripMenuItem exportTexturePackToolStripMenuItem;
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
		private System.Windows.Forms.ToolStripMenuItem highQualityGVMsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importTexturePackToolStripMenuItem;
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
		private System.Windows.Forms.Label labelPaletteFormat;
		private System.Windows.Forms.PictureBox palettePreview;
		private System.Windows.Forms.ToolStripMenuItem palettedTexturesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportMaskToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportPalettedIndexedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportPalettedFullToolStripMenuItem;
		private System.Windows.Forms.Panel panelPaletteInfo;
		private System.Windows.Forms.Button buttonLoadPalette;
		private System.Windows.Forms.Button buttonSavePalette;
		private System.Windows.Forms.Button buttonResetPalette;
		private System.Windows.Forms.Label labelPaletteBankPreview;
		private System.Windows.Forms.ComboBox comboBoxCurrentPaletteBank;
		private System.Windows.Forms.NumericUpDown numericUpDownStartColor;
		private System.Windows.Forms.Label labelStartColor;
		private System.Windows.Forms.Label labelStartBank;
		private System.Windows.Forms.NumericUpDown numericUpDownStartBank;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.ToolStripMenuItem compatibleGVPToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.TrackBar texturePreviewZoomTrackBar;
		private System.Windows.Forms.ToolStripMenuItem textureFilteringToolStripMenuItem;
		private System.Windows.Forms.Label labelZoomInfo;
	}
}

