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
			components = new System.ComponentModel.Container();
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label labelOriginalSize;
			System.Windows.Forms.Label labelX;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			menuStrip1 = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newPVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newGVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newPVMXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newPAKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newXVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsPVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsGVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsPVMXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsPAKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveXVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			importTexturePackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportTexturePackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addMipmapsToAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			highQualityGVMsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			alphaSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			enablePAKAlphaForAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			disablePAKAlphaForAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			palettedTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			compatibleGVPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			exportMaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportPalettedIndexedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportPalettedFullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			textureFilteringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			usePNGInsteadOfDDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			generateNewGbixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			listBox1 = new System.Windows.Forms.ListBox();
			panel1 = new System.Windows.Forms.Panel();
			textureDownButton = new System.Windows.Forms.Button();
			textureUpButton = new System.Windows.Forms.Button();
			removeTextureButton = new System.Windows.Forms.Button();
			addTextureButton = new System.Windows.Forms.Button();
			checkBoxPAKUseAlpha = new System.Windows.Forms.CheckBox();
			numericUpDownOrigSizeY = new System.Windows.Forms.NumericUpDown();
			numericUpDownOrigSizeX = new System.Windows.Forms.NumericUpDown();
			hexIndexCheckBox = new System.Windows.Forms.CheckBox();
			indexTextBox = new System.Windows.Forms.TextBox();
			mipmapCheckBox = new System.Windows.Forms.CheckBox();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			textureSizeLabel = new System.Windows.Forms.Label();
			importExportPanel = new System.Windows.Forms.Panel();
			exportButton = new System.Windows.Forms.Button();
			importButton = new System.Windows.Forms.Button();
			dataFormatLabel = new System.Windows.Forms.Label();
			pixelFormatLabel = new System.Windows.Forms.Label();
			palettePreview = new System.Windows.Forms.PictureBox();
			panel3 = new System.Windows.Forms.Panel();
			buttonLoadPalette = new System.Windows.Forms.Button();
			buttonSavePalette = new System.Windows.Forms.Button();
			buttonResetPalette = new System.Windows.Forms.Button();
			panelPaletteInfo = new System.Windows.Forms.Panel();
			labelPaletteBankPreview = new System.Windows.Forms.Label();
			numericUpDownStartColor = new System.Windows.Forms.NumericUpDown();
			labelStartColor = new System.Windows.Forms.Label();
			labelStartBank = new System.Windows.Forms.Label();
			numericUpDownStartBank = new System.Windows.Forms.NumericUpDown();
			comboBoxCurrentPaletteBank = new System.Windows.Forms.ComboBox();
			labelPaletteFormat = new System.Windows.Forms.Label();
			texturePreviewZoomTrackBar = new System.Windows.Forms.TrackBar();
			textureImage = new System.Windows.Forms.PictureBox();
			labelZoomInfo = new System.Windows.Forms.Label();
			globalIndex = new System.Windows.Forms.NumericUpDown();
			textureName = new System.Windows.Forms.TextBox();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			dummyPanel = new System.Windows.Forms.Panel();
			contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
			copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveTextureButton = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			labelOriginalSize = new System.Windows.Forms.Label();
			labelX = new System.Windows.Forms.Label();
			menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownOrigSizeY).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownOrigSizeX).BeginInit();
			tableLayoutPanel1.SuspendLayout();
			importExportPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)palettePreview).BeginInit();
			panel3.SuspendLayout();
			panelPaletteInfo.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownStartColor).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownStartBank).BeginInit();
			((System.ComponentModel.ISupportInitialize)texturePreviewZoomTrackBar).BeginInit();
			((System.ComponentModel.ISupportInitialize)textureImage).BeginInit();
			((System.ComponentModel.ISupportInitialize)globalIndex).BeginInit();
			statusStrip1.SuspendLayout();
			contextMenuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(4, 37);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(42, 15);
			label1.TabIndex = 0;
			label1.Text = "Name:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(4, 66);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(76, 15);
			label2.TabIndex = 2;
			label2.Text = "Global Index:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(4, 7);
			label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(39, 15);
			label3.TabIndex = 8;
			label3.Text = "Index:";
			// 
			// labelOriginalSize
			// 
			labelOriginalSize.AutoSize = true;
			labelOriginalSize.Location = new System.Drawing.Point(4, 96);
			labelOriginalSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelOriginalSize.Name = "labelOriginalSize";
			labelOriginalSize.Size = new System.Drawing.Size(30, 15);
			labelOriginalSize.TabIndex = 11;
			labelOriginalSize.Text = "Size:";
			// 
			// labelX
			// 
			labelX.AutoSize = true;
			labelX.Location = new System.Drawing.Point(159, 96);
			labelX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelX.Name = "labelX";
			labelX.Size = new System.Drawing.Size(13, 15);
			labelX.TabIndex = 14;
			labelX.Text = "x";
			// 
			// menuStrip1
			// 
			menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem });
			menuStrip1.Location = new System.Drawing.Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new System.Windows.Forms.Padding(5, 1, 0, 1);
			menuStrip1.Size = new System.Drawing.Size(705, 24);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator3, importTexturePackToolStripMenuItem, exportTexturePackToolStripMenuItem, toolStripSeparator1, recentFilesToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
			fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newPVMToolStripMenuItem, newGVMToolStripMenuItem, newXVMToolStripMenuItem, newPVMXToolStripMenuItem, newPAKToolStripMenuItem });
			newToolStripMenuItem.Image = Properties.Resources._new;
			newToolStripMenuItem.Name = "newToolStripMenuItem";
			newToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			newToolStripMenuItem.Text = "&New";
			// 
			// newPVMToolStripMenuItem
			// 
			newPVMToolStripMenuItem.Name = "newPVMToolStripMenuItem";
			newPVMToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N;
			newPVMToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			newPVMToolStripMenuItem.Text = "&PVM";
			newPVMToolStripMenuItem.Click += newPVMToolStripMenuItem_Click;
			// 
			// newGVMToolStripMenuItem
			// 
			newGVMToolStripMenuItem.Name = "newGVMToolStripMenuItem";
			newGVMToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.N;
			newGVMToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			newGVMToolStripMenuItem.Text = "&GVM";
			newGVMToolStripMenuItem.Click += newGVMToolStripMenuItem_Click;
			// 
			// newPVMXToolStripMenuItem
			// 
			newPVMXToolStripMenuItem.Name = "newPVMXToolStripMenuItem";
			newPVMXToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.N;
			newPVMXToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			newPVMXToolStripMenuItem.Text = "PVM&X";
			newPVMXToolStripMenuItem.Click += newPVMXToolStripMenuItem_Click;
			// 
			// newPAKToolStripMenuItem
			// 
			newPAKToolStripMenuItem.Name = "newPAKToolStripMenuItem";
			newPAKToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.N;
			newPAKToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			newPAKToolStripMenuItem.Text = "PA&K";
			newPAKToolStripMenuItem.Click += newPAKToolStripMenuItem_Click;
			// 
			// newXVMToolStripMenuItem
			// 
			newXVMToolStripMenuItem.Name = "newXVMToolStripMenuItem";
			newXVMToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			newXVMToolStripMenuItem.Text = "XVM";
			newXVMToolStripMenuItem.Click += newXVMToolStripMenuItem_Click;
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Image = Properties.Resources.open;
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
			openToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			openToolStripMenuItem.Text = "&Open...";
			openToolStripMenuItem.Click += openToolStripMenuItem_Click;
			// 
			// saveToolStripMenuItem
			// 
			saveToolStripMenuItem.Image = Properties.Resources.save;
			saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			saveToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
			saveToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			saveToolStripMenuItem.Text = "&Save";
			saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
			// 
			// saveAsToolStripMenuItem
			// 
			saveAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { saveAsPVMToolStripMenuItem, saveAsGVMToolStripMenuItem, saveXVMToolStripMenuItem, saveAsPVMXToolStripMenuItem, saveAsPAKToolStripMenuItem });
			saveAsToolStripMenuItem.Image = Properties.Resources.saveas;
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S;
			saveAsToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			saveAsToolStripMenuItem.Text = "Save &As...";
			saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
			// 
			// saveAsPVMToolStripMenuItem
			// 
			saveAsPVMToolStripMenuItem.Name = "saveAsPVMToolStripMenuItem";
			saveAsPVMToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			saveAsPVMToolStripMenuItem.Text = "&PVM";
			saveAsPVMToolStripMenuItem.Click += saveAsPVMToolStripMenuItem_Click;
			// 
			// saveAsGVMToolStripMenuItem
			// 
			saveAsGVMToolStripMenuItem.Name = "saveAsGVMToolStripMenuItem";
			saveAsGVMToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			saveAsGVMToolStripMenuItem.Text = "&GVM";
			saveAsGVMToolStripMenuItem.Click += saveAsGVMToolStripMenuItem_Click;
			// 
			// saveAsPVMXToolStripMenuItem
			// 
			saveAsPVMXToolStripMenuItem.Name = "saveAsPVMXToolStripMenuItem";
			saveAsPVMXToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			saveAsPVMXToolStripMenuItem.Text = "PVM&X";
			saveAsPVMXToolStripMenuItem.Click += saveAsPVMXToolStripMenuItem_Click;
			// 
			// saveAsPAKToolStripMenuItem
			// 
			saveAsPAKToolStripMenuItem.Name = "saveAsPAKToolStripMenuItem";
			saveAsPAKToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			saveAsPAKToolStripMenuItem.Text = "PA&K";
			saveAsPAKToolStripMenuItem.Click += saveAsPAKToolStripMenuItem_Click;
			// 
			// saveXVMToolStripMenuItem
			// 
			saveXVMToolStripMenuItem.Name = "saveXVMToolStripMenuItem";
			saveXVMToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			saveXVMToolStripMenuItem.Text = "XVM";
			saveXVMToolStripMenuItem.Click += saveXVMToolStripMenuItem_Click;
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(222, 6);
			// 
			// importTexturePackToolStripMenuItem
			// 
			importTexturePackToolStripMenuItem.Image = Properties.Resources.import;
			importTexturePackToolStripMenuItem.Name = "importTexturePackToolStripMenuItem";
			importTexturePackToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I;
			importTexturePackToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			importTexturePackToolStripMenuItem.Text = "&Import folder texture pack...";
			importTexturePackToolStripMenuItem.ToolTipText = "Import a folder texture pack with an index file.";
			importTexturePackToolStripMenuItem.Click += importTexturePackToolStripMenuItem_Click;
			// 
			// exportTexturePackToolStripMenuItem
			// 
			exportTexturePackToolStripMenuItem.Image = Properties.Resources.export;
			exportTexturePackToolStripMenuItem.Name = "exportTexturePackToolStripMenuItem";
			exportTexturePackToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E;
			exportTexturePackToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			exportTexturePackToolStripMenuItem.Text = "&Export folder texture pack...";
			exportTexturePackToolStripMenuItem.ToolTipText = "Export a folder texture pack with an index file.";
			exportTexturePackToolStripMenuItem.Click += exportTexturePackToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
			// 
			// recentFilesToolStripMenuItem
			// 
			recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
			recentFilesToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			recentFilesToolStripMenuItem.Text = "&Recent Files";
			recentFilesToolStripMenuItem.DropDownItemClicked += recentFilesToolStripMenuItem_DropDownItemClicked;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(222, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4;
			exitToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			exitToolStripMenuItem.Text = "E&xit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { addMipmapsToAllToolStripMenuItem, highQualityGVMsToolStripMenuItem, alphaSortingToolStripMenuItem, palettedTexturesToolStripMenuItem, textureFilteringToolStripMenuItem, usePNGInsteadOfDDSToolStripMenuItem, generateNewGbixToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(39, 22);
			editToolStripMenuItem.Text = "&Edit";
			// 
			// addMipmapsToAllToolStripMenuItem
			// 
			addMipmapsToAllToolStripMenuItem.Name = "addMipmapsToAllToolStripMenuItem";
			addMipmapsToAllToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			addMipmapsToAllToolStripMenuItem.Text = "Add &Mipmaps to All";
			addMipmapsToAllToolStripMenuItem.ToolTipText = "Enable the Mipmap flag for all textures.";
			addMipmapsToAllToolStripMenuItem.Click += addMipmapsToAllToolStripMenuItem_Click;
			// 
			// highQualityGVMsToolStripMenuItem
			// 
			highQualityGVMsToolStripMenuItem.CheckOnClick = true;
			highQualityGVMsToolStripMenuItem.Name = "highQualityGVMsToolStripMenuItem";
			highQualityGVMsToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			highQualityGVMsToolStripMenuItem.Text = "High &Quality GVMs (GC only)";
			highQualityGVMsToolStripMenuItem.ToolTipText = "Enable support for lossless textures in GVM files. Only supported by SA2B Gamecube.";
			highQualityGVMsToolStripMenuItem.CheckedChanged += highQualityGVMsToolStripMenuItem_CheckedChanged;
			// 
			// alphaSortingToolStripMenuItem
			// 
			alphaSortingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { enablePAKAlphaForAllToolStripMenuItem, disablePAKAlphaForAllToolStripMenuItem });
			alphaSortingToolStripMenuItem.Enabled = false;
			alphaSortingToolStripMenuItem.Name = "alphaSortingToolStripMenuItem";
			alphaSortingToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			alphaSortingToolStripMenuItem.Text = "PAK alpha flags";
			alphaSortingToolStripMenuItem.ToolTipText = "Transparency flags for SA2 PC PAKs.";
			// 
			// enablePAKAlphaForAllToolStripMenuItem
			// 
			enablePAKAlphaForAllToolStripMenuItem.Name = "enablePAKAlphaForAllToolStripMenuItem";
			enablePAKAlphaForAllToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			enablePAKAlphaForAllToolStripMenuItem.Text = "Enable for All";
			enablePAKAlphaForAllToolStripMenuItem.ToolTipText = "Set the RGB5A3 pixel format and disable Z Write/Alpha Test for all textures in this PAK.";
			enablePAKAlphaForAllToolStripMenuItem.Click += enablePAKAlphaForAllToolStripMenuItem_Click;
			// 
			// disablePAKAlphaForAllToolStripMenuItem
			// 
			disablePAKAlphaForAllToolStripMenuItem.Name = "disablePAKAlphaForAllToolStripMenuItem";
			disablePAKAlphaForAllToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			disablePAKAlphaForAllToolStripMenuItem.Text = "Disable for All";
			disablePAKAlphaForAllToolStripMenuItem.ToolTipText = "Remove the RGB5A3 format and unset the flag that disables Z Write/Alpha Test for all textures in this PAK.";
			disablePAKAlphaForAllToolStripMenuItem.Click += disablePAKAlphaForAllToolStripMenuItem_Click;
			// 
			// palettedTexturesToolStripMenuItem
			// 
			palettedTexturesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { compatibleGVPToolStripMenuItem, toolStripSeparator4, exportMaskToolStripMenuItem, exportPalettedIndexedToolStripMenuItem, exportPalettedFullToolStripMenuItem });
			palettedTexturesToolStripMenuItem.Name = "palettedTexturesToolStripMenuItem";
			palettedTexturesToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			palettedTexturesToolStripMenuItem.Text = "Paletted Textures";
			// 
			// compatibleGVPToolStripMenuItem
			// 
			compatibleGVPToolStripMenuItem.Checked = true;
			compatibleGVPToolStripMenuItem.CheckOnClick = true;
			compatibleGVPToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			compatibleGVPToolStripMenuItem.Name = "compatibleGVPToolStripMenuItem";
			compatibleGVPToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			compatibleGVPToolStripMenuItem.Text = "SADX/SA2 compatible GVPs";
			compatibleGVPToolStripMenuItem.ToolTipText = "Use ARGB4444 and ARGB1555 pixel formats for Gamecube paletted textures. Required for SA2 PC, SADX GC and SA2B GC.";
			compatibleGVPToolStripMenuItem.CheckedChanged += compatibleGVPToolStripMenuItem_CheckedChanged;
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(218, 6);
			// 
			// exportMaskToolStripMenuItem
			// 
			exportMaskToolStripMenuItem.Checked = true;
			exportMaskToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			exportMaskToolStripMenuItem.Name = "exportMaskToolStripMenuItem";
			exportMaskToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			exportMaskToolStripMenuItem.Text = "Export Mask (indexed)";
			exportMaskToolStripMenuItem.ToolTipText = "Export indexed textures with the default palette.";
			exportMaskToolStripMenuItem.Click += exportMaskToolStripMenuItem_Click;
			// 
			// exportPalettedIndexedToolStripMenuItem
			// 
			exportPalettedIndexedToolStripMenuItem.Name = "exportPalettedIndexedToolStripMenuItem";
			exportPalettedIndexedToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			exportPalettedIndexedToolStripMenuItem.Text = "Export Paletted (indexed)";
			exportPalettedIndexedToolStripMenuItem.ToolTipText = "Export indexed textures with the currently applied palette set.";
			exportPalettedIndexedToolStripMenuItem.Click += exportPalettedIndexedToolStripMenuItem_Click;
			// 
			// exportPalettedFullToolStripMenuItem
			// 
			exportPalettedFullToolStripMenuItem.Name = "exportPalettedFullToolStripMenuItem";
			exportPalettedFullToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			exportPalettedFullToolStripMenuItem.Text = "Export Paletted (full color)";
			exportPalettedFullToolStripMenuItem.ToolTipText = "Export indexed textures as non-indexed images after applying the current palette.";
			exportPalettedFullToolStripMenuItem.Click += exportPalettedFullToolStripMenuItem_Click;
			// 
			// textureFilteringToolStripMenuItem
			// 
			textureFilteringToolStripMenuItem.Checked = true;
			textureFilteringToolStripMenuItem.CheckOnClick = true;
			textureFilteringToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			textureFilteringToolStripMenuItem.Name = "textureFilteringToolStripMenuItem";
			textureFilteringToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			textureFilteringToolStripMenuItem.Text = "Texture Filtering";
			textureFilteringToolStripMenuItem.ToolTipText = "Enable bicubic interpolation when resizing the preview image.";
			textureFilteringToolStripMenuItem.CheckedChanged += textureFilteringToolStripMenuItem_CheckedChanged;
			textureFilteringToolStripMenuItem.Click += textureFilteringToolStripMenuItem_Click;
			// 
			// usePNGInsteadOfDDSToolStripMenuItem
			// 
			usePNGInsteadOfDDSToolStripMenuItem.CheckOnClick = true;
			usePNGInsteadOfDDSToolStripMenuItem.Name = "usePNGInsteadOfDDSToolStripMenuItem";
			usePNGInsteadOfDDSToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			usePNGInsteadOfDDSToolStripMenuItem.Text = "Use PNG instead of DDS";
			usePNGInsteadOfDDSToolStripMenuItem.ToolTipText = "Use PNG instead of DDS in PAK files. PNG has better quality but loads slower and lacks built-in mipmaps.";
			usePNGInsteadOfDDSToolStripMenuItem.CheckedChanged += usePNGInsteadOfDDSToolStripMenuItem_CheckedChanged;
			// 
			// splitContainer1
			// 
			splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.Location = new System.Drawing.Point(0, 24);
			splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(listBox1);
			splitContainer1.Panel1.Controls.Add(panel1);
			splitContainer1.Panel1MinSize = 206;
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.AutoScroll = true;
			splitContainer1.Panel2.Controls.Add(checkBoxPAKUseAlpha);
			splitContainer1.Panel2.Controls.Add(labelX);
			splitContainer1.Panel2.Controls.Add(numericUpDownOrigSizeY);
			splitContainer1.Panel2.Controls.Add(numericUpDownOrigSizeX);
			splitContainer1.Panel2.Controls.Add(labelOriginalSize);
			splitContainer1.Panel2.Controls.Add(hexIndexCheckBox);
			splitContainer1.Panel2.Controls.Add(indexTextBox);
			splitContainer1.Panel2.Controls.Add(label3);
			splitContainer1.Panel2.Controls.Add(mipmapCheckBox);
			splitContainer1.Panel2.Controls.Add(tableLayoutPanel1);
			splitContainer1.Panel2.Controls.Add(globalIndex);
			splitContainer1.Panel2.Controls.Add(label2);
			splitContainer1.Panel2.Controls.Add(textureName);
			splitContainer1.Panel2.Controls.Add(label1);
			splitContainer1.Panel2.SizeChanged += SplitContainer1_Panel2_SizeChanged;
			splitContainer1.Size = new System.Drawing.Size(705, 601);
			splitContainer1.SplitterDistance = 247;
			splitContainer1.SplitterWidth = 5;
			splitContainer1.TabIndex = 1;
			// 
			// listBox1
			// 
			listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			listBox1.IntegralHeight = false;
			listBox1.ItemHeight = 15;
			listBox1.Location = new System.Drawing.Point(0, 0);
			listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			listBox1.Name = "listBox1";
			listBox1.Size = new System.Drawing.Size(243, 566);
			listBox1.TabIndex = 0;
			listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
			// 
			// panel1
			// 
			panel1.AutoSize = true;
			panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(textureDownButton);
			panel1.Controls.Add(textureUpButton);
			panel1.Controls.Add(removeTextureButton);
			panel1.Controls.Add(addTextureButton);
			panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			panel1.Location = new System.Drawing.Point(0, 566);
			panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(243, 31);
			panel1.TabIndex = 1;
			// 
			// textureDownButton
			// 
			textureDownButton.AutoSize = true;
			textureDownButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			textureDownButton.Enabled = false;
			textureDownButton.Location = new System.Drawing.Point(180, 3);
			textureDownButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			textureDownButton.Name = "textureDownButton";
			textureDownButton.Size = new System.Drawing.Size(48, 25);
			textureDownButton.TabIndex = 3;
			textureDownButton.Text = "Down";
			textureDownButton.UseVisualStyleBackColor = true;
			textureDownButton.Click += TextureDownButton_Click;
			// 
			// textureUpButton
			// 
			textureUpButton.AutoSize = true;
			textureUpButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			textureUpButton.Enabled = false;
			textureUpButton.Location = new System.Drawing.Point(136, 3);
			textureUpButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			textureUpButton.Name = "textureUpButton";
			textureUpButton.Size = new System.Drawing.Size(32, 25);
			textureUpButton.TabIndex = 2;
			textureUpButton.Text = "Up";
			textureUpButton.UseVisualStyleBackColor = true;
			textureUpButton.Click += TextureUpButton_Click;
			// 
			// removeTextureButton
			// 
			removeTextureButton.AutoSize = true;
			removeTextureButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			removeTextureButton.Enabled = false;
			removeTextureButton.Location = new System.Drawing.Point(63, 3);
			removeTextureButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			removeTextureButton.Name = "removeTextureButton";
			removeTextureButton.Size = new System.Drawing.Size(60, 25);
			removeTextureButton.TabIndex = 1;
			removeTextureButton.Text = "Remove";
			removeTextureButton.UseVisualStyleBackColor = true;
			removeTextureButton.Click += removeTextureButton_Click;
			// 
			// addTextureButton
			// 
			addTextureButton.AutoSize = true;
			addTextureButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			addTextureButton.Location = new System.Drawing.Point(4, 3);
			addTextureButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			addTextureButton.Name = "addTextureButton";
			addTextureButton.Size = new System.Drawing.Size(48, 25);
			addTextureButton.TabIndex = 0;
			addTextureButton.Text = "Add...";
			addTextureButton.UseVisualStyleBackColor = true;
			addTextureButton.Click += addTextureButton_Click;
			// 
			// checkBoxPAKUseAlpha
			// 
			checkBoxPAKUseAlpha.AutoSize = true;
			checkBoxPAKUseAlpha.Enabled = false;
			checkBoxPAKUseAlpha.Location = new System.Drawing.Point(317, 65);
			checkBoxPAKUseAlpha.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			checkBoxPAKUseAlpha.Name = "checkBoxPAKUseAlpha";
			checkBoxPAKUseAlpha.Size = new System.Drawing.Size(98, 19);
			checkBoxPAKUseAlpha.TabIndex = 15;
			checkBoxPAKUseAlpha.Text = "No Alpha Test";
			checkBoxPAKUseAlpha.UseVisualStyleBackColor = true;
			checkBoxPAKUseAlpha.CheckedChanged += checkBoxPAKUseAlpha_CheckedChanged;
			checkBoxPAKUseAlpha.Click += checkBoxPAKUseAlpha_Click;
			// 
			// numericUpDownOrigSizeY
			// 
			numericUpDownOrigSizeY.Enabled = false;
			numericUpDownOrigSizeY.Location = new System.Drawing.Point(176, 93);
			numericUpDownOrigSizeY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			numericUpDownOrigSizeY.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
			numericUpDownOrigSizeY.Name = "numericUpDownOrigSizeY";
			numericUpDownOrigSizeY.Size = new System.Drawing.Size(64, 23);
			numericUpDownOrigSizeY.TabIndex = 13;
			numericUpDownOrigSizeY.ValueChanged += numericUpDownOrigSizeY_ValueChanged;
			// 
			// numericUpDownOrigSizeX
			// 
			numericUpDownOrigSizeX.Enabled = false;
			numericUpDownOrigSizeX.Location = new System.Drawing.Point(91, 93);
			numericUpDownOrigSizeX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			numericUpDownOrigSizeX.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
			numericUpDownOrigSizeX.Name = "numericUpDownOrigSizeX";
			numericUpDownOrigSizeX.Size = new System.Drawing.Size(62, 23);
			numericUpDownOrigSizeX.TabIndex = 12;
			numericUpDownOrigSizeX.ValueChanged += numericUpDownOrigSizeX_ValueChanged;
			// 
			// hexIndexCheckBox
			// 
			hexIndexCheckBox.AutoSize = true;
			hexIndexCheckBox.Location = new System.Drawing.Point(215, 6);
			hexIndexCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			hexIndexCheckBox.Name = "hexIndexCheckBox";
			hexIndexCheckBox.Size = new System.Drawing.Size(47, 19);
			hexIndexCheckBox.TabIndex = 10;
			hexIndexCheckBox.Text = "Hex";
			hexIndexCheckBox.UseVisualStyleBackColor = true;
			hexIndexCheckBox.CheckedChanged += HexIndexCheckBox_CheckedChanged;
			// 
			// indexTextBox
			// 
			indexTextBox.Location = new System.Drawing.Point(91, 3);
			indexTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			indexTextBox.Name = "indexTextBox";
			indexTextBox.ReadOnly = true;
			indexTextBox.Size = new System.Drawing.Size(116, 23);
			indexTextBox.TabIndex = 9;
			indexTextBox.Text = "0";
			// 
			// mipmapCheckBox
			// 
			mipmapCheckBox.AutoSize = true;
			mipmapCheckBox.Enabled = false;
			mipmapCheckBox.Location = new System.Drawing.Point(238, 65);
			mipmapCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			mipmapCheckBox.Name = "mipmapCheckBox";
			mipmapCheckBox.Size = new System.Drawing.Size(71, 19);
			mipmapCheckBox.TabIndex = 7;
			mipmapCheckBox.Text = "Mipmap";
			mipmapCheckBox.UseVisualStyleBackColor = true;
			mipmapCheckBox.CheckedChanged += mipmapCheckBox_CheckedChanged;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.Controls.Add(textureSizeLabel, 0, 2);
			tableLayoutPanel1.Controls.Add(importExportPanel, 0, 6);
			tableLayoutPanel1.Controls.Add(dataFormatLabel, 0, 0);
			tableLayoutPanel1.Controls.Add(pixelFormatLabel, 0, 1);
			tableLayoutPanel1.Controls.Add(palettePreview, 0, 8);
			tableLayoutPanel1.Controls.Add(panel3, 0, 10);
			tableLayoutPanel1.Controls.Add(panelPaletteInfo, 1, 8);
			tableLayoutPanel1.Controls.Add(texturePreviewZoomTrackBar, 0, 3);
			tableLayoutPanel1.Controls.Add(textureImage, 0, 5);
			tableLayoutPanel1.Controls.Add(labelZoomInfo, 0, 4);
			tableLayoutPanel1.Location = new System.Drawing.Point(5, 122);
			tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 11;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.Size = new System.Drawing.Size(393, 387);
			tableLayoutPanel1.TabIndex = 6;
			// 
			// textureSizeLabel
			// 
			textureSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			textureSizeLabel.AutoSize = true;
			textureSizeLabel.Location = new System.Drawing.Point(0, 45);
			textureSizeLabel.Margin = new System.Windows.Forms.Padding(0, 3, 4, 3);
			textureSizeLabel.Name = "textureSizeLabel";
			textureSizeLabel.Size = new System.Drawing.Size(85, 15);
			textureSizeLabel.TabIndex = 11;
			textureSizeLabel.Text = "Actual Size: ---";
			textureSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			textureSizeLabel.Visible = false;
			// 
			// importExportPanel
			// 
			importExportPanel.AutoSize = true;
			importExportPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			importExportPanel.Controls.Add(saveTextureButton);
			importExportPanel.Controls.Add(exportButton);
			importExportPanel.Controls.Add(importButton);
			importExportPanel.Location = new System.Drawing.Point(4, 204);
			importExportPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			importExportPanel.Name = "importExportPanel";
			importExportPanel.Size = new System.Drawing.Size(191, 31);
			importExportPanel.TabIndex = 5;
			// 
			// exportButton
			// 
			exportButton.AutoSize = true;
			exportButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			exportButton.Enabled = false;
			exportButton.Location = new System.Drawing.Point(71, 3);
			exportButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			exportButton.Name = "exportButton";
			exportButton.Size = new System.Drawing.Size(60, 25);
			exportButton.TabIndex = 7;
			exportButton.Text = "Export...";
			exportButton.UseVisualStyleBackColor = true;
			exportButton.Click += exportButton_Click;
			// 
			// importButton
			// 
			importButton.AutoSize = true;
			importButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			importButton.Enabled = false;
			importButton.Location = new System.Drawing.Point(4, 3);
			importButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			importButton.Name = "importButton";
			importButton.Size = new System.Drawing.Size(62, 25);
			importButton.TabIndex = 6;
			importButton.Text = "Import...";
			importButton.UseVisualStyleBackColor = true;
			importButton.Click += importButton_Click;
			// 
			// dataFormatLabel
			// 
			dataFormatLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			dataFormatLabel.AutoSize = true;
			dataFormatLabel.Location = new System.Drawing.Point(0, 3);
			dataFormatLabel.Margin = new System.Windows.Forms.Padding(0, 3, 4, 3);
			dataFormatLabel.Name = "dataFormatLabel";
			dataFormatLabel.Size = new System.Drawing.Size(129, 15);
			dataFormatLabel.TabIndex = 6;
			dataFormatLabel.Text = "Data Format: Unknown";
			dataFormatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pixelFormatLabel
			// 
			pixelFormatLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			pixelFormatLabel.AutoSize = true;
			pixelFormatLabel.Location = new System.Drawing.Point(0, 24);
			pixelFormatLabel.Margin = new System.Windows.Forms.Padding(0, 3, 4, 3);
			pixelFormatLabel.Name = "pixelFormatLabel";
			pixelFormatLabel.Size = new System.Drawing.Size(130, 15);
			pixelFormatLabel.TabIndex = 7;
			pixelFormatLabel.Text = "Pixel Format: Unknown";
			pixelFormatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// palettePreview
			// 
			palettePreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			palettePreview.Location = new System.Drawing.Point(0, 238);
			palettePreview.Margin = new System.Windows.Forms.Padding(0);
			palettePreview.Name = "palettePreview";
			palettePreview.Size = new System.Drawing.Size(33, 33);
			palettePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			palettePreview.TabIndex = 13;
			palettePreview.TabStop = false;
			palettePreview.Visible = false;
			// 
			// panel3
			// 
			panel3.AutoSize = true;
			panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panel3.Controls.Add(buttonLoadPalette);
			panel3.Controls.Add(buttonSavePalette);
			panel3.Controls.Add(buttonResetPalette);
			panel3.Location = new System.Drawing.Point(4, 353);
			panel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			panel3.Name = "panel3";
			panel3.Size = new System.Drawing.Size(171, 31);
			panel3.TabIndex = 27;
			// 
			// buttonLoadPalette
			// 
			buttonLoadPalette.AutoSize = true;
			buttonLoadPalette.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			buttonLoadPalette.Location = new System.Drawing.Point(4, 3);
			buttonLoadPalette.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			buttonLoadPalette.Name = "buttonLoadPalette";
			buttonLoadPalette.Size = new System.Drawing.Size(52, 25);
			buttonLoadPalette.TabIndex = 17;
			buttonLoadPalette.Text = "Load...";
			buttonLoadPalette.UseVisualStyleBackColor = true;
			buttonLoadPalette.Visible = false;
			buttonLoadPalette.Click += buttonLoadPalette_Click;
			// 
			// buttonSavePalette
			// 
			buttonSavePalette.AutoSize = true;
			buttonSavePalette.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			buttonSavePalette.Location = new System.Drawing.Point(64, 3);
			buttonSavePalette.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			buttonSavePalette.Name = "buttonSavePalette";
			buttonSavePalette.Size = new System.Drawing.Size(50, 25);
			buttonSavePalette.TabIndex = 18;
			buttonSavePalette.Text = "Save...";
			buttonSavePalette.UseVisualStyleBackColor = true;
			buttonSavePalette.Visible = false;
			buttonSavePalette.Click += buttonSavePalette_Click;
			// 
			// buttonResetPalette
			// 
			buttonResetPalette.AutoSize = true;
			buttonResetPalette.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			buttonResetPalette.Location = new System.Drawing.Point(122, 3);
			buttonResetPalette.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			buttonResetPalette.Name = "buttonResetPalette";
			buttonResetPalette.Size = new System.Drawing.Size(45, 25);
			buttonResetPalette.TabIndex = 19;
			buttonResetPalette.Text = "Reset";
			buttonResetPalette.UseVisualStyleBackColor = true;
			buttonResetPalette.Visible = false;
			buttonResetPalette.Click += buttonResetPalette_Click;
			// 
			// panelPaletteInfo
			// 
			panelPaletteInfo.Controls.Add(labelPaletteBankPreview);
			panelPaletteInfo.Controls.Add(numericUpDownStartColor);
			panelPaletteInfo.Controls.Add(labelStartColor);
			panelPaletteInfo.Controls.Add(labelStartBank);
			panelPaletteInfo.Controls.Add(numericUpDownStartBank);
			panelPaletteInfo.Controls.Add(comboBoxCurrentPaletteBank);
			panelPaletteInfo.Controls.Add(labelPaletteFormat);
			panelPaletteInfo.Location = new System.Drawing.Point(212, 241);
			panelPaletteInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			panelPaletteInfo.Name = "panelPaletteInfo";
			panelPaletteInfo.Size = new System.Drawing.Size(177, 106);
			panelPaletteInfo.TabIndex = 20;
			panelPaletteInfo.Visible = false;
			// 
			// labelPaletteBankPreview
			// 
			labelPaletteBankPreview.AutoSize = true;
			labelPaletteBankPreview.Location = new System.Drawing.Point(4, 28);
			labelPaletteBankPreview.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelPaletteBankPreview.Name = "labelPaletteBankPreview";
			labelPaletteBankPreview.Size = new System.Drawing.Size(75, 15);
			labelPaletteBankPreview.TabIndex = 21;
			labelPaletteBankPreview.Text = "Palette Bank:";
			labelPaletteBankPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numericUpDownStartColor
			// 
			numericUpDownStartColor.Location = new System.Drawing.Point(98, 72);
			numericUpDownStartColor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			numericUpDownStartColor.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
			numericUpDownStartColor.Name = "numericUpDownStartColor";
			numericUpDownStartColor.Size = new System.Drawing.Size(71, 23);
			numericUpDownStartColor.TabIndex = 25;
			numericUpDownStartColor.ValueChanged += numericUpDownStartColor_ValueChanged;
			// 
			// labelStartColor
			// 
			labelStartColor.AutoSize = true;
			labelStartColor.Location = new System.Drawing.Point(94, 52);
			labelStartColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelStartColor.Name = "labelStartColor";
			labelStartColor.Size = new System.Drawing.Size(66, 15);
			labelStartColor.TabIndex = 24;
			labelStartColor.Text = "Start Color:";
			// 
			// labelStartBank
			// 
			labelStartBank.AutoSize = true;
			labelStartBank.Location = new System.Drawing.Point(4, 52);
			labelStartBank.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelStartBank.Name = "labelStartBank";
			labelStartBank.Size = new System.Drawing.Size(63, 15);
			labelStartBank.TabIndex = 23;
			labelStartBank.Text = "Start Bank:";
			// 
			// numericUpDownStartBank
			// 
			numericUpDownStartBank.Location = new System.Drawing.Point(5, 72);
			numericUpDownStartBank.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			numericUpDownStartBank.Maximum = new decimal(new int[] { 63, 0, 0, 0 });
			numericUpDownStartBank.Name = "numericUpDownStartBank";
			numericUpDownStartBank.Size = new System.Drawing.Size(71, 23);
			numericUpDownStartBank.TabIndex = 22;
			numericUpDownStartBank.ValueChanged += numericUpDownStartBank_ValueChanged;
			// 
			// comboBoxCurrentPaletteBank
			// 
			comboBoxCurrentPaletteBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxCurrentPaletteBank.FormattingEnabled = true;
			comboBoxCurrentPaletteBank.Location = new System.Drawing.Point(99, 24);
			comboBoxCurrentPaletteBank.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			comboBoxCurrentPaletteBank.Name = "comboBoxCurrentPaletteBank";
			comboBoxCurrentPaletteBank.Size = new System.Drawing.Size(69, 23);
			comboBoxCurrentPaletteBank.TabIndex = 20;
			comboBoxCurrentPaletteBank.SelectedIndexChanged += comboBoxCurrentPaletteBank_SelectedIndexChanged;
			// 
			// labelPaletteFormat
			// 
			labelPaletteFormat.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			labelPaletteFormat.AutoSize = true;
			labelPaletteFormat.Location = new System.Drawing.Point(4, 6);
			labelPaletteFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelPaletteFormat.Name = "labelPaletteFormat";
			labelPaletteFormat.Size = new System.Drawing.Size(49, 15);
			labelPaletteFormat.TabIndex = 12;
			labelPaletteFormat.Text = "Palette: ";
			labelPaletteFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			labelPaletteFormat.Visible = false;
			// 
			// texturePreviewZoomTrackBar
			// 
			texturePreviewZoomTrackBar.Location = new System.Drawing.Point(4, 66);
			texturePreviewZoomTrackBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			texturePreviewZoomTrackBar.Maximum = 8;
			texturePreviewZoomTrackBar.Name = "texturePreviewZoomTrackBar";
			texturePreviewZoomTrackBar.Size = new System.Drawing.Size(200, 45);
			texturePreviewZoomTrackBar.TabIndex = 28;
			texturePreviewZoomTrackBar.Value = 4;
			texturePreviewZoomTrackBar.Scroll += texturePreviewZoomTrackBar_Scroll;
			// 
			// textureImage
			// 
			textureImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			textureImage.Location = new System.Drawing.Point(0, 137);
			textureImage.Margin = new System.Windows.Forms.Padding(0);
			textureImage.Name = "textureImage";
			textureImage.Size = new System.Drawing.Size(64, 64);
			textureImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			textureImage.TabIndex = 4;
			textureImage.TabStop = false;
			textureImage.DragDrop += textureImage_DragDrop;
			textureImage.DragEnter += textureImage_DragEnter;
			textureImage.MouseClick += textureImage_MouseClick;
			textureImage.MouseMove += textureImage_MouseMove;
			// 
			// labelZoomInfo
			// 
			labelZoomInfo.AutoSize = true;
			labelZoomInfo.Location = new System.Drawing.Point(4, 114);
			labelZoomInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelZoomInfo.Name = "labelZoomInfo";
			labelZoomInfo.Size = new System.Drawing.Size(67, 15);
			labelZoomInfo.TabIndex = 29;
			labelZoomInfo.Text = "Zoom: N/A";
			// 
			// globalIndex
			// 
			globalIndex.Enabled = false;
			globalIndex.Location = new System.Drawing.Point(91, 63);
			globalIndex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			globalIndex.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
			globalIndex.Name = "globalIndex";
			globalIndex.Size = new System.Drawing.Size(140, 23);
			globalIndex.TabIndex = 3;
			globalIndex.ValueChanged += globalIndex_ValueChanged;
			// 
			// textureName
			// 
			textureName.Enabled = false;
			textureName.Location = new System.Drawing.Point(91, 33);
			textureName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			textureName.Name = "textureName";
			textureName.Size = new System.Drawing.Size(215, 23);
			textureName.TabIndex = 1;
			textureName.TextChanged += textureName_TextChanged;
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
			statusStrip1.Location = new System.Drawing.Point(0, 625);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
			statusStrip1.Size = new System.Drawing.Size(705, 22);
			statusStrip1.TabIndex = 2;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new System.Drawing.Size(58, 17);
			toolStripStatusLabel1.Text = "0 textures";
			// 
			// dummyPanel
			// 
			dummyPanel.Enabled = false;
			dummyPanel.Location = new System.Drawing.Point(0, 0);
			dummyPanel.Margin = new System.Windows.Forms.Padding(0);
			dummyPanel.Name = "dummyPanel";
			dummyPanel.Size = new System.Drawing.Size(117, 115);
			dummyPanel.TabIndex = 3;
			dummyPanel.Visible = false;
			// 
			// contextMenuStrip1
			// 
			contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { copyToolStripMenuItem, pasteToolStripMenuItem });
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new System.Drawing.Size(111, 64);
			// 
			// copyToolStripMenuItem
			// 
			copyToolStripMenuItem.Image = Properties.Resources.copy;
			copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			copyToolStripMenuItem.Size = new System.Drawing.Size(110, 30);
			copyToolStripMenuItem.Text = "&Copy";
			copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
			// 
			// pasteToolStripMenuItem
			// 
			pasteToolStripMenuItem.Image = Properties.Resources.paste;
			pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			pasteToolStripMenuItem.Size = new System.Drawing.Size(110, 30);
			pasteToolStripMenuItem.Text = "&Paste";
			pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
			// 
			// saveTextureButton
			// 
			saveTextureButton.AutoSize = true;
			saveTextureButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			saveTextureButton.Enabled = false;
			saveTextureButton.Location = new System.Drawing.Point(138, 3);
			saveTextureButton.Name = "saveTextureButton";
			saveTextureButton.Size = new System.Drawing.Size(50, 25);
			saveTextureButton.TabIndex = 8;
			saveTextureButton.Text = "Save...";
			saveTextureButton.UseVisualStyleBackColor = true;
			saveTextureButton.Click += saveTextureButton_Click;
			// generateNewGbixToolStripMenuItem
			// 
			generateNewGbixToolStripMenuItem.Name = "generateNewGbixToolStripMenuItem";
			generateNewGbixToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
			generateNewGbixToolStripMenuItem.Text = "Generate New Gbix";
			generateNewGbixToolStripMenuItem.Click += generateNewGbixToolStripMenuItem_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AllowDrop = true;
			ClientSize = new System.Drawing.Size(705, 647);
			Controls.Add(splitContainer1);
			Controls.Add(menuStrip1);
			Controls.Add(statusStrip1);
			Controls.Add(dummyPanel);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			MainMenuStrip = menuStrip1;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "MainForm";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Texture Editor";
			FormClosing += MainForm_FormClosing;
			Shown += MainForm_Shown;
			DragDrop += Textures_DragDrop;
			DragEnter += Textures_DragEnter;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownOrigSizeY).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownOrigSizeX).EndInit();
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			importExportPanel.ResumeLayout(false);
			importExportPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)palettePreview).EndInit();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panelPaletteInfo.ResumeLayout(false);
			panelPaletteInfo.PerformLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownStartColor).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownStartBank).EndInit();
			((System.ComponentModel.ISupportInitialize)texturePreviewZoomTrackBar).EndInit();
			((System.ComponentModel.ISupportInitialize)textureImage).EndInit();
			((System.ComponentModel.ISupportInitialize)globalIndex).EndInit();
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			contextMenuStrip1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
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
		private System.Windows.Forms.ToolStripMenuItem newXVMToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveXVMToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem usePNGInsteadOfDDSToolStripMenuItem;
		private System.Windows.Forms.Button saveTextureButton;
		private System.Windows.Forms.ToolStripMenuItem generateNewGbixToolStripMenuItem;
	}
}