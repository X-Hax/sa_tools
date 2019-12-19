namespace SonicRetro.SAModel.SAMDL
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newFileMenuitem = new System.Windows.Forms.ToolStripMenuItem();
			this.basicModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.chunkModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gamecubeModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cStructsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nJAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aSSIMPExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.textureRemappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showNodeConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showWeightsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.modelLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editMaterialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.UserControl();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.importOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.cameraPosLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.animNameLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.animFrameLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.loadAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(584, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileMenuitem,
            this.openToolStripMenuItem,
            this.loadAnimationToolStripMenuItem,
            this.loadTexturesToolStripMenuItem,
            this.saveMenuItem,
            this.saveAsToolStripMenuItem,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// newFileMenuitem
			// 
			this.newFileMenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicModelToolStripMenuItem,
            this.chunkModelToolStripMenuItem,
            this.gamecubeModelToolStripMenuItem});
			this.newFileMenuitem.Name = "newFileMenuitem";
			this.newFileMenuitem.Size = new System.Drawing.Size(180, 22);
			this.newFileMenuitem.Text = "&New";
			// 
			// basicModelToolStripMenuItem
			// 
			this.basicModelToolStripMenuItem.Name = "basicModelToolStripMenuItem";
			this.basicModelToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.basicModelToolStripMenuItem.Text = "&Basic Model";
			this.basicModelToolStripMenuItem.Click += new System.EventHandler(this.basicModelToolStripMenuItem_Click);
			// 
			// chunkModelToolStripMenuItem
			// 
			this.chunkModelToolStripMenuItem.Name = "chunkModelToolStripMenuItem";
			this.chunkModelToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.chunkModelToolStripMenuItem.Text = "&Chunk Model";
			this.chunkModelToolStripMenuItem.Click += new System.EventHandler(this.chunkModelToolStripMenuItem_Click);
			// 
			// gamecubeModelToolStripMenuItem
			// 
			this.gamecubeModelToolStripMenuItem.Name = "gamecubeModelToolStripMenuItem";
			this.gamecubeModelToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.gamecubeModelToolStripMenuItem.Text = "Gamecube Model";
			this.gamecubeModelToolStripMenuItem.Click += new System.EventHandler(this.GamecubeModelToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// loadTexturesToolStripMenuItem
			// 
			this.loadTexturesToolStripMenuItem.Name = "loadTexturesToolStripMenuItem";
			this.loadTexturesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.loadTexturesToolStripMenuItem.Text = "Load Textures...";
			this.loadTexturesToolStripMenuItem.Click += new System.EventHandler(this.loadTexturesToolStripMenuItem_Click);
			// 
			// saveMenuItem
			// 
			this.saveMenuItem.Enabled = false;
			this.saveMenuItem.Name = "saveMenuItem";
			this.saveMenuItem.Size = new System.Drawing.Size(180, 22);
			this.saveMenuItem.Text = "&Save...";
			this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Enabled = false;
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.saveAsToolStripMenuItem.Text = "Save &As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.Enabled = false;
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.importToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.importToolStripMenuItem.Text = "&Import";
			this.importToolStripMenuItem.Click += new System.EventHandler(this.aSSIMPImportToolStripMenuItem_Click);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cStructsToolStripMenuItem,
            this.nJAToolStripMenuItem,
            this.aSSIMPExportToolStripMenuItem});
			this.exportToolStripMenuItem.Enabled = false;
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.exportToolStripMenuItem.Text = "&Export";
			// 
			// cStructsToolStripMenuItem
			// 
			this.cStructsToolStripMenuItem.Name = "cStructsToolStripMenuItem";
			this.cStructsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.cStructsToolStripMenuItem.Text = "C &Structs";
			this.cStructsToolStripMenuItem.Click += new System.EventHandler(this.cStructsToolStripMenuItem_Click);
			// 
			// nJAToolStripMenuItem
			// 
			this.nJAToolStripMenuItem.Name = "nJAToolStripMenuItem";
			this.nJAToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.nJAToolStripMenuItem.Text = "NJA";
			this.nJAToolStripMenuItem.Click += new System.EventHandler(this.nJAToolStripMenuItem_Click);
			// 
			// aSSIMPExportToolStripMenuItem
			// 
			this.aSSIMPExportToolStripMenuItem.Name = "aSSIMPExportToolStripMenuItem";
			this.aSSIMPExportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.aSSIMPExportToolStripMenuItem.Text = "ASSIMP Export";
			this.aSSIMPExportToolStripMenuItem.Click += new System.EventHandler(this.aSSIMPExportToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem,
            this.findToolStripMenuItem,
            this.textureRemappingToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// preferencesToolStripMenuItem
			// 
			this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.preferencesToolStripMenuItem.Text = "Preferences";
			this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
			// 
			// findToolStripMenuItem
			// 
			this.findToolStripMenuItem.Enabled = false;
			this.findToolStripMenuItem.Name = "findToolStripMenuItem";
			this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.findToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.findToolStripMenuItem.Text = "&Find...";
			this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
			// 
			// textureRemappingToolStripMenuItem
			// 
			this.textureRemappingToolStripMenuItem.Enabled = false;
			this.textureRemappingToolStripMenuItem.Name = "textureRemappingToolStripMenuItem";
			this.textureRemappingToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.textureRemappingToolStripMenuItem.Text = "&Texture Remapping...";
			this.textureRemappingToolStripMenuItem.Click += new System.EventHandler(this.textureRemappingToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showModelToolStripMenuItem,
            this.showNodesToolStripMenuItem,
            this.showNodeConnectionsToolStripMenuItem,
            this.showWeightsToolStripMenuItem,
            this.modelLibraryToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// showModelToolStripMenuItem
			// 
			this.showModelToolStripMenuItem.Checked = true;
			this.showModelToolStripMenuItem.CheckOnClick = true;
			this.showModelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showModelToolStripMenuItem.Name = "showModelToolStripMenuItem";
			this.showModelToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.showModelToolStripMenuItem.Text = "Show &Model";
			this.showModelToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showModelToolStripMenuItem_CheckedChanged);
			// 
			// showNodesToolStripMenuItem
			// 
			this.showNodesToolStripMenuItem.CheckOnClick = true;
			this.showNodesToolStripMenuItem.Name = "showNodesToolStripMenuItem";
			this.showNodesToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.showNodesToolStripMenuItem.Text = "Show &Nodes";
			this.showNodesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showNodesToolStripMenuItem_CheckedChanged);
			// 
			// showNodeConnectionsToolStripMenuItem
			// 
			this.showNodeConnectionsToolStripMenuItem.CheckOnClick = true;
			this.showNodeConnectionsToolStripMenuItem.Name = "showNodeConnectionsToolStripMenuItem";
			this.showNodeConnectionsToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.showNodeConnectionsToolStripMenuItem.Text = "Show Node &Connections";
			this.showNodeConnectionsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showNodeConnectionsToolStripMenuItem_CheckedChanged);
			// 
			// showWeightsToolStripMenuItem
			// 
			this.showWeightsToolStripMenuItem.CheckOnClick = true;
			this.showWeightsToolStripMenuItem.Enabled = false;
			this.showWeightsToolStripMenuItem.Name = "showWeightsToolStripMenuItem";
			this.showWeightsToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.showWeightsToolStripMenuItem.Text = "Show &Weights";
			this.showWeightsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ShowWeightsToolStripMenuItem_CheckedChanged);
			// 
			// modelLibraryToolStripMenuItem
			// 
			this.modelLibraryToolStripMenuItem.Name = "modelLibraryToolStripMenuItem";
			this.modelLibraryToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.modelLibraryToolStripMenuItem.Text = "Model &Library";
			this.modelLibraryToolStripMenuItem.Click += new System.EventHandler(this.modelLibraryToolStripMenuItem_Click);
			// 
			// copyModelToolStripMenuItem
			// 
			this.copyModelToolStripMenuItem.Enabled = false;
			this.copyModelToolStripMenuItem.Name = "copyModelToolStripMenuItem";
			this.copyModelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyModelToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.copyModelToolStripMenuItem.Text = "&Copy Model";
			this.copyModelToolStripMenuItem.Click += new System.EventHandler(this.copyModelToolStripMenuItem_Click);
			// 
			// pasteModelToolStripMenuItem
			// 
			this.pasteModelToolStripMenuItem.Enabled = false;
			this.pasteModelToolStripMenuItem.Name = "pasteModelToolStripMenuItem";
			this.pasteModelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteModelToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.pasteModelToolStripMenuItem.Text = "&Paste Model";
			this.pasteModelToolStripMenuItem.Click += new System.EventHandler(this.pasteModelToolStripMenuItem_Click);
			// 
			// editMaterialsToolStripMenuItem
			// 
			this.editMaterialsToolStripMenuItem.Enabled = false;
			this.editMaterialsToolStripMenuItem.Name = "editMaterialsToolStripMenuItem";
			this.editMaterialsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.editMaterialsToolStripMenuItem.Text = "Edit &Materials...";
			this.editMaterialsToolStripMenuItem.Click += new System.EventHandler(this.editMaterialsToolStripMenuItem_Click);
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(320, 538);
			this.panel1.TabIndex = 1;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			this.panel1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyDown);
			this.panel1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyUp);
			this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
			this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
			this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
			this.panel1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.panel1_PreviewKeyDown);
			// 
			// timer1
			// 
			this.timer1.Interval = 33;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer1.Size = new System.Drawing.Size(584, 538);
			this.splitContainer1.SplitterDistance = 320;
			this.splitContainer1.TabIndex = 2;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(260, 538);
			this.tabControl1.TabIndex = 15;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.propertyGrid1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(252, 512);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Properties";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDarkDark;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Margin = new System.Windows.Forms.Padding(0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this.propertyGrid1.Size = new System.Drawing.Size(252, 512);
			this.propertyGrid1.TabIndex = 14;
			this.propertyGrid1.ToolbarVisible = false;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.treeView1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(252, 512);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Tree";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(252, 512);
			this.treeView1.TabIndex = 1;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyModelToolStripMenuItem,
            this.pasteModelToolStripMenuItem,
            this.editMaterialsToolStripMenuItem,
            this.importOBJToolStripMenuItem,
            this.exportOBJToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(182, 114);
			// 
			// importOBJToolStripMenuItem
			// 
			this.importOBJToolStripMenuItem.Enabled = false;
			this.importOBJToolStripMenuItem.Name = "importOBJToolStripMenuItem";
			this.importOBJToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.importOBJToolStripMenuItem.Text = "&Import OBJ...";
			this.importOBJToolStripMenuItem.Click += new System.EventHandler(this.importOBJToolStripMenuItem_Click);
			// 
			// exportOBJToolStripMenuItem
			// 
			this.exportOBJToolStripMenuItem.Enabled = false;
			this.exportOBJToolStripMenuItem.Name = "exportOBJToolStripMenuItem";
			this.exportOBJToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.exportOBJToolStripMenuItem.Text = "&Export OBJ...";
			this.exportOBJToolStripMenuItem.Click += new System.EventHandler(this.exportOBJToolStripMenuItem_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraPosLabel,
            this.animNameLabel,
            this.animFrameLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 538);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(584, 24);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// cameraPosLabel
			// 
			this.cameraPosLabel.Name = "cameraPosLabel";
			this.cameraPosLabel.Size = new System.Drawing.Size(106, 19);
			this.cameraPosLabel.Text = "Camera Pos: 0, 0, 0";
			// 
			// animNameLabel
			// 
			this.animNameLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.animNameLabel.Name = "animNameLabel";
			this.animNameLabel.Size = new System.Drawing.Size(102, 19);
			this.animNameLabel.Text = "Animation: None";
			// 
			// animFrameLabel
			// 
			this.animFrameLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.animFrameLabel.Name = "animFrameLabel";
			this.animFrameLabel.Size = new System.Drawing.Size(56, 19);
			this.animFrameLabel.Text = "Frame: 0";
			// 
			// loadAnimationToolStripMenuItem
			// 
			this.loadAnimationToolStripMenuItem.Name = "loadAnimationToolStripMenuItem";
			this.loadAnimationToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.loadAnimationToolStripMenuItem.Text = "Load Animation...";
			this.loadAnimationToolStripMenuItem.Click += new System.EventHandler(this.loadAnimationToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 562);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "SAMDL";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.contextMenuStrip1.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.UserControl panel1;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadTexturesToolStripMenuItem;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cStructsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteModelToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem editMaterialsToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem exportOBJToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importOBJToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showNodeConnectionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem modelLibraryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newFileMenuitem;
		private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
		private System.Windows.Forms.ToolStripMenuItem basicModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem chunkModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem nJAToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem textureRemappingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aSSIMPExportToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel cameraPosLabel;
		private System.Windows.Forms.ToolStripStatusLabel animNameLabel;
		private System.Windows.Forms.ToolStripStatusLabel animFrameLabel;
		private System.Windows.Forms.ToolStripMenuItem showWeightsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gamecubeModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadAnimationToolStripMenuItem;
	}
}
