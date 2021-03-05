using System.Windows.Forms.VisualStyles;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileMenuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chunkModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gamecubeModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAnimationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importAsModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importAsNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cStructsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nJAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSSIMPExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureRemappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swapUVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exportAnimationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTextureNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newModelUnloadsTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showNodeConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showVertexIndicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showWeightsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAdvancedCameraInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alternativeCameraModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.welcomeTutorialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMaterialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenderPanel = new System.Windows.Forms.UserControl();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.BackgroundPanel = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonNew = new System.Windows.Forms.ToolStripButton();
            this.buttonOpen = new System.Windows.Forms.ToolStripButton();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.buttonSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonShowNodes = new System.Windows.Forms.ToolStripButton();
            this.buttonShowNodeConnections = new System.Windows.Forms.ToolStripButton();
            this.buttonShowVertexIndices = new System.Windows.Forms.ToolStripButton();
            this.buttonShowWeights = new System.Windows.Forms.ToolStripButton();
            this.buttonLighting = new System.Windows.Forms.ToolStripButton();
            this.buttonMaterialColors = new System.Windows.Forms.ToolStripButton();
            this.buttonShowHints = new System.Windows.Forms.ToolStripButton();
            this.buttonPreferences = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonSolid = new System.Windows.Forms.ToolStripButton();
            this.buttonVertices = new System.Windows.Forms.ToolStripButton();
            this.buttonWireframe = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonPrevAnimation = new System.Windows.Forms.ToolStripButton();
            this.buttonNextAnimation = new System.Windows.Forms.ToolStripButton();
            this.buttonPrevFrame = new System.Windows.Forms.ToolStripButton();
            this.buttonPlayAnimation = new System.Windows.Forms.ToolStripButton();
            this.buttonNextFrame = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.PolyNormalstoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createPolyNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePolyNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.importContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSelectedAsMeshContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSelectedAsNodesContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.addChildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byMeshsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byFaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.transparentOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortMeshsetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hierarchyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearChildrenToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyModelDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cameraPosLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cameraAngleLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cameraModeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.animNameLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.animFrameLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.MessageTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripNew = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.basicModelToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.chunkModelToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.gamecubeModelToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStripNew.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(949, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileMenuitem,
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.loadTexturesToolStripMenuItem,
            this.unloadTextureToolStripMenuItem,
            this.loadAnimationToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveAnimationsToolStripMenuItem,
            this.toolStripSeparator3,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newFileMenuitem
            // 
            this.newFileMenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicModelToolStripMenuItem,
            this.chunkModelToolStripMenuItem,
            this.gamecubeModelToolStripMenuItem});
            this.newFileMenuitem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources._new;
            this.newFileMenuitem.Name = "newFileMenuitem";
            this.newFileMenuitem.Size = new System.Drawing.Size(168, 22);
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
            this.openToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // loadTexturesToolStripMenuItem
            // 
            this.loadTexturesToolStripMenuItem.Name = "loadTexturesToolStripMenuItem";
            this.loadTexturesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.loadTexturesToolStripMenuItem.Text = "Load Textures...";
            this.loadTexturesToolStripMenuItem.Click += new System.EventHandler(this.loadTexturesToolStripMenuItem_Click);
            // 
            // unloadTextureToolStripMenuItem
            // 
            this.unloadTextureToolStripMenuItem.Enabled = false;
            this.unloadTextureToolStripMenuItem.Name = "unloadTextureToolStripMenuItem";
            this.unloadTextureToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.unloadTextureToolStripMenuItem.Text = "Unload Textures";
            this.unloadTextureToolStripMenuItem.Click += new System.EventHandler(this.unloadTextureToolStripMenuItem_Click);
            // 
            // loadAnimationToolStripMenuItem
            // 
            this.loadAnimationToolStripMenuItem.Enabled = false;
            this.loadAnimationToolStripMenuItem.Name = "loadAnimationToolStripMenuItem";
            this.loadAnimationToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.loadAnimationToolStripMenuItem.Text = "Load Animation...";
            this.loadAnimationToolStripMenuItem.Click += new System.EventHandler(this.loadAnimationToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(165, 6);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Enabled = false;
            this.saveMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.save;
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveMenuItem.Text = "&Save Model";
            this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.saveas;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveAsToolStripMenuItem.Text = "Save Model &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveAnimationsToolStripMenuItem
            // 
            this.saveAnimationsToolStripMenuItem.Enabled = false;
            this.saveAnimationsToolStripMenuItem.Name = "saveAnimationsToolStripMenuItem";
            this.saveAnimationsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveAnimationsToolStripMenuItem.Text = "Save Ani&mation...";
            this.saveAnimationsToolStripMenuItem.Click += new System.EventHandler(this.saveAnimationsToolStripMenuItem1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(165, 6);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importAsModelToolStripMenuItem,
            this.importAsNodesToolStripMenuItem});
            this.importToolStripMenuItem.Enabled = false;
            this.importToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.import;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.importToolStripMenuItem.Text = "&Import...";
            // 
            // importAsModelToolStripMenuItem
            // 
            this.importAsModelToolStripMenuItem.Name = "importAsModelToolStripMenuItem";
            this.importAsModelToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.importAsModelToolStripMenuItem.Text = "As Model...";
            this.importAsModelToolStripMenuItem.Click += new System.EventHandler(this.importAsModelToolStripMenuItem_Click);
            // 
            // importAsNodesToolStripMenuItem
            // 
            this.importAsNodesToolStripMenuItem.Name = "importAsNodesToolStripMenuItem";
            this.importAsNodesToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.importAsNodesToolStripMenuItem.Text = "As Nodes...";
            this.importAsNodesToolStripMenuItem.Click += new System.EventHandler(this.importAsNodesToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cStructsToolStripMenuItem,
            this.nJAToolStripMenuItem,
            this.aSSIMPExportToolStripMenuItem});
            this.exportToolStripMenuItem.Enabled = false;
            this.exportToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.export;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exportToolStripMenuItem.Text = "&Export";
            // 
            // cStructsToolStripMenuItem
            // 
            this.cStructsToolStripMenuItem.Name = "cStructsToolStripMenuItem";
            this.cStructsToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.cStructsToolStripMenuItem.Text = "C &Structs...";
            this.cStructsToolStripMenuItem.Click += new System.EventHandler(this.cStructsToolStripMenuItem_Click);
            // 
            // nJAToolStripMenuItem
            // 
            this.nJAToolStripMenuItem.Name = "nJAToolStripMenuItem";
            this.nJAToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.nJAToolStripMenuItem.Text = "NJA...";
            this.nJAToolStripMenuItem.Click += new System.EventHandler(this.nJAToolStripMenuItem_Click);
            // 
            // aSSIMPExportToolStripMenuItem
            // 
            this.aSSIMPExportToolStripMenuItem.Name = "aSSIMPExportToolStripMenuItem";
            this.aSSIMPExportToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.aSSIMPExportToolStripMenuItem.Text = "Assimp...";
            this.aSSIMPExportToolStripMenuItem.Click += new System.EventHandler(this.aSSIMPExportToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(165, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem,
            this.findToolStripMenuItem,
            this.materialEditorToolStripMenuItem,
            this.textureRemappingToolStripMenuItem,
            this.swapUVToolStripMenuItem,
            this.toolStripSeparator4,
            this.exportAnimationsToolStripMenuItem,
            this.exportTextureNamesToolStripMenuItem,
            this.newModelUnloadsTexturesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 22);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.settings;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Enabled = false;
            this.findToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.find;
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.findToolStripMenuItem.Text = "&Find...";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // materialEditorToolStripMenuItem
            // 
            this.materialEditorToolStripMenuItem.Enabled = false;
            this.materialEditorToolStripMenuItem.Name = "materialEditorToolStripMenuItem";
            this.materialEditorToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.materialEditorToolStripMenuItem.Text = "Material Editor";
            this.materialEditorToolStripMenuItem.Click += new System.EventHandler(this.materialEditorToolStripMenuItem_Click);
            // 
            // textureRemappingToolStripMenuItem
            // 
            this.textureRemappingToolStripMenuItem.Enabled = false;
            this.textureRemappingToolStripMenuItem.Name = "textureRemappingToolStripMenuItem";
            this.textureRemappingToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.textureRemappingToolStripMenuItem.Text = "&Texture Remapping...";
            this.textureRemappingToolStripMenuItem.Click += new System.EventHandler(this.textureRemappingToolStripMenuItem_Click);
            // 
            // swapUVToolStripMenuItem
            // 
            this.swapUVToolStripMenuItem.Enabled = false;
            this.swapUVToolStripMenuItem.Name = "swapUVToolStripMenuItem";
            this.swapUVToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.swapUVToolStripMenuItem.Text = "Swap U/V";
            this.swapUVToolStripMenuItem.ToolTipText = "Switch Us and Vs around. For Basic models only.";
            this.swapUVToolStripMenuItem.Click += new System.EventHandler(this.swapUVToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(185, 6);
            // 
            // exportAnimationsToolStripMenuItem
            // 
            this.exportAnimationsToolStripMenuItem.Checked = true;
            this.exportAnimationsToolStripMenuItem.CheckOnClick = true;
            this.exportAnimationsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportAnimationsToolStripMenuItem.Name = "exportAnimationsToolStripMenuItem";
            this.exportAnimationsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.exportAnimationsToolStripMenuItem.Text = "Export Motion Structs";
            this.exportAnimationsToolStripMenuItem.ToolTipText = "Export animations in C structs.";
            // 
            // exportTextureNamesToolStripMenuItem
            // 
            this.exportTextureNamesToolStripMenuItem.CheckOnClick = true;
            this.exportTextureNamesToolStripMenuItem.Name = "exportTextureNamesToolStripMenuItem";
            this.exportTextureNamesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.exportTextureNamesToolStripMenuItem.Text = "Export Texture Enums";
            this.exportTextureNamesToolStripMenuItem.ToolTipText = "Use texture names in materials instead of texture IDs when exporting C structs.";
            // 
            // newModelUnloadsTexturesToolStripMenuItem
            // 
            this.newModelUnloadsTexturesToolStripMenuItem.CheckOnClick = true;
            this.newModelUnloadsTexturesToolStripMenuItem.Name = "newModelUnloadsTexturesToolStripMenuItem";
            this.newModelUnloadsTexturesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.newModelUnloadsTexturesToolStripMenuItem.Text = "Auto Unload Textures";
            this.newModelUnloadsTexturesToolStripMenuItem.ToolTipText = "Unload textures when a new model is loaded or created.";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showModelToolStripMenuItem,
            this.showNodesToolStripMenuItem,
            this.showNodeConnectionsToolStripMenuItem,
            this.showVertexIndicesToolStripMenuItem,
            this.showWeightsToolStripMenuItem,
            this.showHintsToolStripMenuItem,
            this.showAdvancedCameraInfoToolStripMenuItem,
            this.modelCodeToolStripMenuItem,
            this.modelLibraryToolStripMenuItem,
            this.alternativeCameraModeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // showModelToolStripMenuItem
            // 
            this.showModelToolStripMenuItem.Checked = true;
            this.showModelToolStripMenuItem.CheckOnClick = true;
            this.showModelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showModelToolStripMenuItem.Name = "showModelToolStripMenuItem";
            this.showModelToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.showModelToolStripMenuItem.Text = "Show &Model";
            this.showModelToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showModelToolStripMenuItem_CheckedChanged);
            // 
            // showNodesToolStripMenuItem
            // 
            this.showNodesToolStripMenuItem.CheckOnClick = true;
            this.showNodesToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.node;
            this.showNodesToolStripMenuItem.Name = "showNodesToolStripMenuItem";
            this.showNodesToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.showNodesToolStripMenuItem.Text = "Show &Nodes";
            this.showNodesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showNodesToolStripMenuItem_CheckedChanged);
            // 
            // showNodeConnectionsToolStripMenuItem
            // 
            this.showNodeConnectionsToolStripMenuItem.CheckOnClick = true;
            this.showNodeConnectionsToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.nodecon;
            this.showNodeConnectionsToolStripMenuItem.Name = "showNodeConnectionsToolStripMenuItem";
            this.showNodeConnectionsToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.showNodeConnectionsToolStripMenuItem.Text = "Show Node &Connections";
            this.showNodeConnectionsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showNodeConnectionsToolStripMenuItem_CheckedChanged);
            // 
            // showVertexIndicesToolStripMenuItem
            // 
            this.showVertexIndicesToolStripMenuItem.CheckOnClick = true;
            this.showVertexIndicesToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.vertid;
            this.showVertexIndicesToolStripMenuItem.Name = "showVertexIndicesToolStripMenuItem";
            this.showVertexIndicesToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.showVertexIndicesToolStripMenuItem.Text = "Show Vertex Indices";
            this.showVertexIndicesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showVertexIndicesToolStripMenuItem_CheckedChanged);
            // 
            // showWeightsToolStripMenuItem
            // 
            this.showWeightsToolStripMenuItem.CheckOnClick = true;
            this.showWeightsToolStripMenuItem.Enabled = false;
            this.showWeightsToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.weights;
            this.showWeightsToolStripMenuItem.Name = "showWeightsToolStripMenuItem";
            this.showWeightsToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.showWeightsToolStripMenuItem.Text = "Show &Weights";
            this.showWeightsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ShowWeightsToolStripMenuItem_CheckedChanged);
            // 
            // showHintsToolStripMenuItem
            // 
            this.showHintsToolStripMenuItem.Checked = true;
            this.showHintsToolStripMenuItem.CheckOnClick = true;
            this.showHintsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showHintsToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.messages;
            this.showHintsToolStripMenuItem.Name = "showHintsToolStripMenuItem";
            this.showHintsToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.showHintsToolStripMenuItem.Text = "Show Hints";
            this.showHintsToolStripMenuItem.ToolTipText = "Show Hints";
            this.showHintsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showHintsToolStripMenuItem_CheckedChanged);
            // 
            // showAdvancedCameraInfoToolStripMenuItem
            // 
            this.showAdvancedCameraInfoToolStripMenuItem.CheckOnClick = true;
            this.showAdvancedCameraInfoToolStripMenuItem.Name = "showAdvancedCameraInfoToolStripMenuItem";
            this.showAdvancedCameraInfoToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.showAdvancedCameraInfoToolStripMenuItem.Text = "Show Camera Rotation";
            this.showAdvancedCameraInfoToolStripMenuItem.ToolTipText = "Show more details on camera mode, rotation etc.";
            // 
            // modelCodeToolStripMenuItem
            // 
            this.modelCodeToolStripMenuItem.Enabled = false;
            this.modelCodeToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.source;
            this.modelCodeToolStripMenuItem.Name = "modelCodeToolStripMenuItem";
            this.modelCodeToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.modelCodeToolStripMenuItem.Text = "View Model Code";
            this.modelCodeToolStripMenuItem.Click += new System.EventHandler(this.modelCodeToolStripMenuItem_Click);
            // 
            // modelLibraryToolStripMenuItem
            // 
            this.modelLibraryToolStripMenuItem.Name = "modelLibraryToolStripMenuItem";
            this.modelLibraryToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.modelLibraryToolStripMenuItem.Text = "Model &Library";
            this.modelLibraryToolStripMenuItem.Click += new System.EventHandler(this.modelLibraryToolStripMenuItem_Click);
            // 
            // alternativeCameraModeToolStripMenuItem
            // 
            this.alternativeCameraModeToolStripMenuItem.CheckOnClick = true;
            this.alternativeCameraModeToolStripMenuItem.Name = "alternativeCameraModeToolStripMenuItem";
            this.alternativeCameraModeToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.alternativeCameraModeToolStripMenuItem.Text = "Alternative Camera Mode";
            this.alternativeCameraModeToolStripMenuItem.ToolTipText = "Use an alternative control mode for camera that hides the mouse cursor.";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.welcomeTutorialToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // welcomeTutorialToolStripMenuItem
            // 
            this.welcomeTutorialToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.help;
            this.welcomeTutorialToolStripMenuItem.Name = "welcomeTutorialToolStripMenuItem";
            this.welcomeTutorialToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.welcomeTutorialToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.welcomeTutorialToolStripMenuItem.Text = "Welcome / Tutorial";
            this.welcomeTutorialToolStripMenuItem.Click += new System.EventHandler(this.welcomeTutorialToolStripMenuItem_Click);
            // 
            // editMaterialsToolStripMenuItem
            // 
            this.editMaterialsToolStripMenuItem.Enabled = false;
            this.editMaterialsToolStripMenuItem.Name = "editMaterialsToolStripMenuItem";
            this.editMaterialsToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.editMaterialsToolStripMenuItem.Text = "Edit &Materials...";
            this.editMaterialsToolStripMenuItem.Click += new System.EventHandler(this.editMaterialsToolStripMenuItem_Click);
            // 
            // RenderPanel
            // 
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 0);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(621, 521);
            this.RenderPanel.TabIndex = 1;
			this.RenderPanel.SizeChanged += new System.EventHandler(this.RenderPanel_SizeChanged);
			this.RenderPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.RenderPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyDown);
            this.RenderPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyUp);
            this.RenderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.RenderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
            this.RenderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
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
            this.splitContainer1.Location = new System.Drawing.Point(0, 67);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.RenderPanel);
            this.splitContainer1.Panel1.Controls.Add(this.BackgroundPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(949, 521);
            this.splitContainer1.SplitterDistance = 621;
            this.splitContainer1.TabIndex = 2;
            // 
            // BackgroundPanel
            // 
            this.BackgroundPanel.AutoSize = true;
            this.BackgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BackgroundPanel.Location = new System.Drawing.Point(0, 0);
            this.BackgroundPanel.Name = "BackgroundPanel";
            this.BackgroundPanel.Size = new System.Drawing.Size(621, 521);
            this.BackgroundPanel.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer2.Size = new System.Drawing.Size(324, 521);
            this.splitContainer2.SplitterDistance = 256;
            this.splitContainer2.TabIndex = 16;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(324, 256);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDarkDark;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid1.Size = new System.Drawing.Size(324, 261);
            this.propertyGrid1.TabIndex = 14;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonNew,
            this.buttonOpen,
            this.buttonSave,
            this.buttonSaveAs,
            this.toolStripSeparator10,
            this.buttonShowNodes,
            this.buttonShowNodeConnections,
            this.buttonShowVertexIndices,
            this.buttonShowWeights,
            this.buttonLighting,
            this.buttonMaterialColors,
            this.buttonShowHints,
            this.buttonPreferences,
            this.toolStripSeparator11,
            this.buttonSolid,
            this.buttonVertices,
            this.buttonWireframe,
            this.toolStripSeparator12,
            this.buttonPrevAnimation,
            this.buttonNextAnimation,
            this.buttonPrevFrame,
            this.buttonPlayAnimation,
            this.buttonNextFrame});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(949, 43);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonNew
            // 
            this.buttonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNew.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources._new;
            this.buttonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(40, 40);
            this.buttonNew.Text = "New File";
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOpen.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.open;
            this.buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(40, 40);
            this.buttonOpen.Text = "Open";
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSave.Enabled = false;
            this.buttonSave.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.save;
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(40, 40);
            this.buttonSave.Text = "Save";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonSaveAs
            // 
            this.buttonSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSaveAs.Enabled = false;
            this.buttonSaveAs.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.saveas;
            this.buttonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveAs.Name = "buttonSaveAs";
            this.buttonSaveAs.Size = new System.Drawing.Size(40, 40);
            this.buttonSaveAs.Text = "Save as...";
            this.buttonSaveAs.Click += new System.EventHandler(this.buttonSaveAs_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 43);
            // 
            // buttonShowNodes
            // 
            this.buttonShowNodes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowNodes.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.node;
            this.buttonShowNodes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonShowNodes.Name = "buttonShowNodes";
            this.buttonShowNodes.Size = new System.Drawing.Size(40, 40);
            this.buttonShowNodes.Text = "Show Nodes";
            this.buttonShowNodes.Click += new System.EventHandler(this.buttonShowNodes_Click);
            // 
            // buttonShowNodeConnections
            // 
            this.buttonShowNodeConnections.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowNodeConnections.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.nodecon;
            this.buttonShowNodeConnections.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonShowNodeConnections.Name = "buttonShowNodeConnections";
            this.buttonShowNodeConnections.Size = new System.Drawing.Size(40, 40);
            this.buttonShowNodeConnections.Text = "Show Node Connections";
            this.buttonShowNodeConnections.Click += new System.EventHandler(this.buttonShowNodeConnections_Click);
            // 
            // buttonShowVertexIndices
            // 
            this.buttonShowVertexIndices.CheckOnClick = true;
            this.buttonShowVertexIndices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowVertexIndices.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.vertid;
            this.buttonShowVertexIndices.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonShowVertexIndices.Name = "buttonShowVertexIndices";
            this.buttonShowVertexIndices.Size = new System.Drawing.Size(40, 40);
            this.buttonShowVertexIndices.Text = "Show Vertex Indices";
            this.buttonShowVertexIndices.ToolTipText = "Display vertex indices for the selected model.";
            this.buttonShowVertexIndices.Click += new System.EventHandler(this.buttonShowVertexIndices_Click);
            // 
            // buttonShowWeights
            // 
            this.buttonShowWeights.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowWeights.Enabled = false;
            this.buttonShowWeights.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.weights;
            this.buttonShowWeights.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonShowWeights.Name = "buttonShowWeights";
            this.buttonShowWeights.Size = new System.Drawing.Size(40, 40);
            this.buttonShowWeights.Text = "Show Weights";
            this.buttonShowWeights.Click += new System.EventHandler(this.buttonShowWeights_Click);
            // 
            // buttonLighting
            // 
            this.buttonLighting.Checked = true;
            this.buttonLighting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonLighting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonLighting.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.lighting;
            this.buttonLighting.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLighting.Name = "buttonLighting";
            this.buttonLighting.Size = new System.Drawing.Size(40, 40);
            this.buttonLighting.Text = "Enable Lighting";
            this.buttonLighting.Click += new System.EventHandler(this.buttonLighting_Click);
            // 
            // buttonMaterialColors
            // 
            this.buttonMaterialColors.Checked = true;
            this.buttonMaterialColors.CheckOnClick = true;
            this.buttonMaterialColors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonMaterialColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMaterialColors.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.material;
            this.buttonMaterialColors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMaterialColors.Name = "buttonMaterialColors";
            this.buttonMaterialColors.Size = new System.Drawing.Size(40, 40);
            this.buttonMaterialColors.Text = "Enable Material Colors";
            this.buttonMaterialColors.CheckedChanged += new System.EventHandler(this.buttonMaterialColors_CheckedChanged);
            // 
            // buttonShowHints
            // 
            this.buttonShowHints.Checked = true;
            this.buttonShowHints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowHints.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowHints.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.messages;
            this.buttonShowHints.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonShowHints.Name = "buttonShowHints";
            this.buttonShowHints.Size = new System.Drawing.Size(40, 40);
            this.buttonShowHints.Text = "Show Hints";
            this.buttonShowHints.Click += new System.EventHandler(this.buttonShowHints_Click);
            // 
            // buttonPreferences
            // 
            this.buttonPreferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPreferences.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.settings;
            this.buttonPreferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPreferences.Name = "buttonPreferences";
            this.buttonPreferences.Size = new System.Drawing.Size(40, 40);
            this.buttonPreferences.Text = "Preferences";
            this.buttonPreferences.Click += new System.EventHandler(this.buttonPreferences_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 43);
            // 
            // buttonSolid
            // 
            this.buttonSolid.Checked = true;
            this.buttonSolid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonSolid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSolid.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.solid;
            this.buttonSolid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSolid.Name = "buttonSolid";
            this.buttonSolid.Size = new System.Drawing.Size(40, 40);
            this.buttonSolid.Text = "Render Mode: Solid";
            this.buttonSolid.Click += new System.EventHandler(this.buttonSolid_Click);
            // 
            // buttonVertices
            // 
            this.buttonVertices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonVertices.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.point;
            this.buttonVertices.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonVertices.Name = "buttonVertices";
            this.buttonVertices.Size = new System.Drawing.Size(40, 40);
            this.buttonVertices.Text = "Render Mode: Points";
            this.buttonVertices.Click += new System.EventHandler(this.buttonVertices_Click);
            // 
            // buttonWireframe
            // 
            this.buttonWireframe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonWireframe.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.wireframe;
            this.buttonWireframe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonWireframe.Name = "buttonWireframe";
            this.buttonWireframe.Size = new System.Drawing.Size(40, 40);
            this.buttonWireframe.Text = "Render Mode: Wireframe";
            this.buttonWireframe.Click += new System.EventHandler(this.buttonWireframe_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 43);
            // 
            // buttonPrevAnimation
            // 
            this.buttonPrevAnimation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPrevAnimation.Enabled = false;
            this.buttonPrevAnimation.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.prevanim;
            this.buttonPrevAnimation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPrevAnimation.Name = "buttonPrevAnimation";
            this.buttonPrevAnimation.Size = new System.Drawing.Size(40, 40);
            this.buttonPrevAnimation.Text = "Previous Animation";
            this.buttonPrevAnimation.Click += new System.EventHandler(this.buttonPrevAnimation_Click);
            // 
            // buttonNextAnimation
            // 
            this.buttonNextAnimation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNextAnimation.Enabled = false;
            this.buttonNextAnimation.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.nextanim;
            this.buttonNextAnimation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNextAnimation.Name = "buttonNextAnimation";
            this.buttonNextAnimation.Size = new System.Drawing.Size(40, 40);
            this.buttonNextAnimation.Text = "Next Animation";
            this.buttonNextAnimation.Click += new System.EventHandler(this.buttonNextAnimation_Click);
            // 
            // buttonPrevFrame
            // 
            this.buttonPrevFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPrevFrame.Enabled = false;
            this.buttonPrevFrame.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.prevframe;
            this.buttonPrevFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPrevFrame.Name = "buttonPrevFrame";
            this.buttonPrevFrame.Size = new System.Drawing.Size(40, 40);
            this.buttonPrevFrame.Text = "Previous Frame";
            this.buttonPrevFrame.Click += new System.EventHandler(this.buttonPrevFrame_Click);
            // 
            // buttonPlayAnimation
            // 
            this.buttonPlayAnimation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPlayAnimation.Enabled = false;
            this.buttonPlayAnimation.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.playanim;
            this.buttonPlayAnimation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPlayAnimation.Name = "buttonPlayAnimation";
            this.buttonPlayAnimation.Size = new System.Drawing.Size(40, 40);
            this.buttonPlayAnimation.Text = "Play/Stop Animation";
            this.buttonPlayAnimation.Click += new System.EventHandler(this.buttonPlayAnimation_Click);
            // 
            // buttonNextFrame
            // 
            this.buttonNextFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNextFrame.Enabled = false;
            this.buttonNextFrame.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.nextframe;
            this.buttonNextFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNextFrame.Name = "buttonNextFrame";
            this.buttonNextFrame.Size = new System.Drawing.Size(40, 40);
            this.buttonNextFrame.Text = "Next Frame";
            this.buttonNextFrame.Click += new System.EventHandler(this.buttonNextFrame_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyModelToolStripMenuItem,
            this.pasteModelToolStripMenuItem,
            this.toolStripSeparator9,
            this.editMaterialsToolStripMenuItem,
            this.PolyNormalstoolStripMenuItem,
            this.toolStripSeparator8,
            this.importContextMenuItem,
            this.exportContextMenuItem,
            this.toolStripSeparator6,
            this.addChildToolStripMenuItem,
            this.splitToolStripMenuItem,
            this.sortToolStripMenuItem,
            this.toolStripSeparator7,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(212, 328);
            // 
            // copyModelToolStripMenuItem
            // 
            this.copyModelToolStripMenuItem.Enabled = false;
            this.copyModelToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.copy;
            this.copyModelToolStripMenuItem.Name = "copyModelToolStripMenuItem";
            this.copyModelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyModelToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.copyModelToolStripMenuItem.Text = "&Copy Model";
            this.copyModelToolStripMenuItem.Click += new System.EventHandler(this.copyModelToolStripMenuItem_Click);
            // 
            // pasteModelToolStripMenuItem
            // 
            this.pasteModelToolStripMenuItem.Enabled = false;
            this.pasteModelToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.paste;
            this.pasteModelToolStripMenuItem.Name = "pasteModelToolStripMenuItem";
            this.pasteModelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteModelToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.pasteModelToolStripMenuItem.Text = "&Paste Model";
            this.pasteModelToolStripMenuItem.Click += new System.EventHandler(this.pasteModelToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(208, 6);
            // 
            // PolyNormalstoolStripMenuItem
            // 
            this.PolyNormalstoolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createPolyNormalToolStripMenuItem,
            this.removePolyNormalToolStripMenuItem});
            this.PolyNormalstoolStripMenuItem.Enabled = false;
            this.PolyNormalstoolStripMenuItem.Name = "PolyNormalstoolStripMenuItem";
            this.PolyNormalstoolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.PolyNormalstoolStripMenuItem.Text = "Poly Normals";
            // 
            // createPolyNormalToolStripMenuItem
            // 
            this.createPolyNormalToolStripMenuItem.Name = "createPolyNormalToolStripMenuItem";
            this.createPolyNormalToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.createPolyNormalToolStripMenuItem.Text = "Create";
            this.createPolyNormalToolStripMenuItem.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
            // 
            // removePolyNormalToolStripMenuItem
            // 
            this.removePolyNormalToolStripMenuItem.Name = "removePolyNormalToolStripMenuItem";
            this.removePolyNormalToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removePolyNormalToolStripMenuItem.Text = "Remove";
            this.removePolyNormalToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(208, 6);
            // 
            // importContextMenuItem
            // 
            this.importContextMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importSelectedAsMeshContextMenuItem,
            this.importSelectedAsNodesContextMenuItem});
            this.importContextMenuItem.Enabled = false;
            this.importContextMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.import;
            this.importContextMenuItem.Name = "importContextMenuItem";
            this.importContextMenuItem.Size = new System.Drawing.Size(211, 30);
            this.importContextMenuItem.Text = "&Import selected";
            // 
            // importSelectedAsMeshContextMenuItem
            // 
            this.importSelectedAsMeshContextMenuItem.Name = "importSelectedAsMeshContextMenuItem";
            this.importSelectedAsMeshContextMenuItem.Size = new System.Drawing.Size(164, 22);
            this.importSelectedAsMeshContextMenuItem.Text = "As Mesh...";
            this.importSelectedAsMeshContextMenuItem.Click += new System.EventHandler(this.importSelectedAsMeshContextMenuItem_Click);
            // 
            // importSelectedAsNodesContextMenuItem
            // 
            this.importSelectedAsNodesContextMenuItem.Name = "importSelectedAsNodesContextMenuItem";
            this.importSelectedAsNodesContextMenuItem.Size = new System.Drawing.Size(164, 22);
            this.importSelectedAsNodesContextMenuItem.Text = "As Child Nodes...";
            this.importSelectedAsNodesContextMenuItem.Click += new System.EventHandler(this.importSelectedAsNodesContextMenuItem_Click);
            // 
            // exportContextMenuItem
            // 
            this.exportContextMenuItem.Enabled = false;
            this.exportContextMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.export;
            this.exportContextMenuItem.Name = "exportContextMenuItem";
            this.exportContextMenuItem.Size = new System.Drawing.Size(211, 30);
            this.exportContextMenuItem.Text = "&Export selected...";
            this.exportContextMenuItem.Click += new System.EventHandler(this.exportContextMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(208, 6);
            // 
            // addChildToolStripMenuItem
            // 
            this.addChildToolStripMenuItem.Enabled = false;
            this.addChildToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.addchild;
            this.addChildToolStripMenuItem.Name = "addChildToolStripMenuItem";
            this.addChildToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.addChildToolStripMenuItem.Text = "Add Child";
            this.addChildToolStripMenuItem.Click += new System.EventHandler(this.addChildToolStripMenuItem_Click);
            // 
            // splitToolStripMenuItem
            // 
            this.splitToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.byMeshsetToolStripMenuItem,
            this.byFaceToolStripMenuItem,
            this.toolStripSeparator13,
            this.transparentOnlyToolStripMenuItem});
            this.splitToolStripMenuItem.Enabled = false;
            this.splitToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.split;
            this.splitToolStripMenuItem.Name = "splitToolStripMenuItem";
            this.splitToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.splitToolStripMenuItem.Text = "Split Meshsets";
            // 
            // byMeshsetToolStripMenuItem
            // 
            this.byMeshsetToolStripMenuItem.Name = "byMeshsetToolStripMenuItem";
            this.byMeshsetToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.byMeshsetToolStripMenuItem.Text = "By Mesh";
            this.byMeshsetToolStripMenuItem.ToolTipText = "Create a child model for each meshset in the parent model.";
            this.byMeshsetToolStripMenuItem.Click += new System.EventHandler(this.byMeshsetToolStripMenuItem_Click);
            // 
            // byFaceToolStripMenuItem
            // 
            this.byFaceToolStripMenuItem.Name = "byFaceToolStripMenuItem";
            this.byFaceToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.byFaceToolStripMenuItem.Text = "By Face";
            this.byFaceToolStripMenuItem.ToolTipText = "Create a child model for each face in the parent model\'s meshes.";
            this.byFaceToolStripMenuItem.Click += new System.EventHandler(this.byFaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(214, 6);
            // 
            // transparentOnlyToolStripMenuItem
            // 
            this.transparentOnlyToolStripMenuItem.Checked = true;
            this.transparentOnlyToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.transparentOnlyToolStripMenuItem.Name = "transparentOnlyToolStripMenuItem";
            this.transparentOnlyToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.transparentOnlyToolStripMenuItem.Text = "Don\'t Split Opaque Meshes";
            this.transparentOnlyToolStripMenuItem.ToolTipText = "Split meshes with transparent polygons as separate models but keep all opaque dat" +
    "a in the original model.";
            this.transparentOnlyToolStripMenuItem.Click += new System.EventHandler(this.transparentOnlyToolStripMenuItem_Click);
            // 
            // sortToolStripMenuItem
            // 
            this.sortToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortMeshsetsToolStripMenuItem,
            this.hierarchyToolStripMenuItem});
            this.sortToolStripMenuItem.Enabled = false;
            this.sortToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.sort;
            this.sortToolStripMenuItem.Name = "sortToolStripMenuItem";
            this.sortToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.sortToolStripMenuItem.Text = "Sort Transparent Meshes";
            // 
            // sortMeshsetsToolStripMenuItem
            // 
            this.sortMeshsetsToolStripMenuItem.Name = "sortMeshsetsToolStripMenuItem";
            this.sortMeshsetsToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.sortMeshsetsToolStripMenuItem.Text = "Model";
            this.sortMeshsetsToolStripMenuItem.ToolTipText = "Sort the model\'s meshsets by listing transparent meshes after opaque meshes.";
            this.sortMeshsetsToolStripMenuItem.Click += new System.EventHandler(this.sortMeshsetsToolStripMenuItem_Click);
            // 
            // hierarchyToolStripMenuItem
            // 
            this.hierarchyToolStripMenuItem.Name = "hierarchyToolStripMenuItem";
            this.hierarchyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.hierarchyToolStripMenuItem.Text = "Model + Children";
            this.hierarchyToolStripMenuItem.ToolTipText = "Sort the meshsets of all models in the hierarchy.";
            this.hierarchyToolStripMenuItem.Click += new System.EventHandler(this.hierarchyToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(208, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearChildrenToolStripMenuItem1,
            this.emptyModelDataToolStripMenuItem,
            this.deleteNodeToolStripMenuItem});
            this.deleteToolStripMenuItem.Enabled = false;
            this.deleteToolStripMenuItem.Image = global::SonicRetro.SAModel.SAMDL.Properties.Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // clearChildrenToolStripMenuItem1
            // 
            this.clearChildrenToolStripMenuItem1.Name = "clearChildrenToolStripMenuItem1";
            this.clearChildrenToolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
            this.clearChildrenToolStripMenuItem1.Text = "Children";
            this.clearChildrenToolStripMenuItem1.Click += new System.EventHandler(this.clearChildrenToolStripMenuItem1_Click);
            // 
            // emptyModelDataToolStripMenuItem
            // 
            this.emptyModelDataToolStripMenuItem.Name = "emptyModelDataToolStripMenuItem";
            this.emptyModelDataToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.emptyModelDataToolStripMenuItem.Text = "Model Data (Attach)";
            this.emptyModelDataToolStripMenuItem.Click += new System.EventHandler(this.emptyModelDataToolStripMenuItem_Click);
            // 
            // deleteNodeToolStripMenuItem
            // 
            this.deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
            this.deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.deleteNodeToolStripMenuItem.Text = "Node (Object)";
            this.deleteNodeToolStripMenuItem.Click += new System.EventHandler(this.deleteTheWholeHierarchyToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraPosLabel,
            this.cameraAngleLabel,
            this.cameraModeLabel,
            this.animNameLabel,
            this.animFrameLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 588);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(949, 24);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // cameraPosLabel
            // 
            this.cameraPosLabel.Name = "cameraPosLabel";
            this.cameraPosLabel.Size = new System.Drawing.Size(177, 19);
            this.cameraPosLabel.Text = "Camera X: 0.000 Y: 0.000 Z: 0.000";
            // 
            // cameraAngleLabel
            // 
            this.cameraAngleLabel.Name = "cameraAngleLabel";
            this.cameraAngleLabel.Size = new System.Drawing.Size(0, 19);
            // 
            // cameraModeLabel
            // 
            this.cameraModeLabel.Name = "cameraModeLabel";
            this.cameraModeLabel.Size = new System.Drawing.Size(0, 19);
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
            // MessageTimer
            // 
            this.MessageTimer.Enabled = true;
            this.MessageTimer.Interval = 1;
            this.MessageTimer.Tick += new System.EventHandler(this.MessageTimer_Tick);
            // 
            // contextMenuStripNew
            // 
            this.contextMenuStripNew.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripNew.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicModelToolStripMenuItem1,
            this.chunkModelToolStripMenuItem1,
            this.gamecubeModelToolStripMenuItem1});
            this.contextMenuStripNew.Name = "contextMenuStripNew";
            this.contextMenuStripNew.Size = new System.Drawing.Size(169, 70);
            // 
            // basicModelToolStripMenuItem1
            // 
            this.basicModelToolStripMenuItem1.Name = "basicModelToolStripMenuItem1";
            this.basicModelToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
            this.basicModelToolStripMenuItem1.Text = "Basic Model";
            this.basicModelToolStripMenuItem1.Click += new System.EventHandler(this.basicModelToolStripMenuItem1_Click);
            // 
            // chunkModelToolStripMenuItem1
            // 
            this.chunkModelToolStripMenuItem1.Name = "chunkModelToolStripMenuItem1";
            this.chunkModelToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
            this.chunkModelToolStripMenuItem1.Text = "Chunk Model";
            this.chunkModelToolStripMenuItem1.Click += new System.EventHandler(this.chunkModelToolStripMenuItem1_Click);
            // 
            // gamecubeModelToolStripMenuItem1
            // 
            this.gamecubeModelToolStripMenuItem1.Name = "gamecubeModelToolStripMenuItem1";
            this.gamecubeModelToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
            this.gamecubeModelToolStripMenuItem1.Text = "Gamecube Model";
            this.gamecubeModelToolStripMenuItem1.Click += new System.EventHandler(this.gamecubeModelToolStripMenuItem1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 612);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "SAMDL";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
			this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStripNew.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.UserControl RenderPanel;
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
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem exportContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importContextMenuItem;
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
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem welcomeTutorialToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem unloadTextureToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem exportAnimationsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportTextureNamesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newModelUnloadsTexturesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem swapUVToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel cameraModeLabel;
		private System.Windows.Forms.ToolStripStatusLabel cameraAngleLabel;
		private System.Windows.Forms.ToolStripMenuItem showAdvancedCameraInfoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addChildToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem showHintsToolStripMenuItem;
		private System.Windows.Forms.Timer MessageTimer;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton buttonNew;
		private System.Windows.Forms.ToolStripButton buttonOpen;
		private System.Windows.Forms.ToolStripButton buttonSave;
		private System.Windows.Forms.ToolStripButton buttonSaveAs;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
		private System.Windows.Forms.ToolStripButton buttonShowNodes;
		private System.Windows.Forms.ToolStripButton buttonShowNodeConnections;
		private System.Windows.Forms.ToolStripButton buttonShowWeights;
		private System.Windows.Forms.ToolStripButton buttonShowHints;
		private System.Windows.Forms.ToolStripButton buttonPreferences;
		private System.Windows.Forms.ToolStripButton buttonSolid;
		private System.Windows.Forms.ToolStripButton buttonWireframe;
		private System.Windows.Forms.ToolStripButton buttonVertices;
		private System.Windows.Forms.Panel BackgroundPanel;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
		private System.Windows.Forms.ToolStripButton buttonPlayAnimation;
		private System.Windows.Forms.ToolStripButton buttonPrevFrame;
		private System.Windows.Forms.ToolStripButton buttonNextFrame;
		private System.Windows.Forms.ToolStripButton buttonPrevAnimation;
		private System.Windows.Forms.ToolStripButton buttonNextAnimation;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripNew;
		private System.Windows.Forms.ToolStripMenuItem basicModelToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem chunkModelToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem gamecubeModelToolStripMenuItem1;
		private System.Windows.Forms.ToolStripButton buttonLighting;
		private System.Windows.Forms.ToolStripMenuItem materialEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAnimationsToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton buttonMaterialColors;
		private System.Windows.Forms.ToolStripMenuItem modelCodeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PolyNormalstoolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createPolyNormalToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removePolyNormalToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearChildrenToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem emptyModelDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteNodeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem splitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem byMeshsetToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem byFaceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sortToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sortMeshsetsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hierarchyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem transparentOnlyToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
		private System.Windows.Forms.ToolStripMenuItem alternativeCameraModeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importAsModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importAsNodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importSelectedAsMeshContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importSelectedAsNodesContextMenuItem;
		private System.Windows.Forms.ToolStripButton buttonShowVertexIndices;
		private System.Windows.Forms.ToolStripMenuItem showVertexIndicesToolStripMenuItem;
	}
}
