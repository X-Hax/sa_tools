using System.Windows.Forms.VisualStyles;

namespace SAModel.SAMDL
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
			menuStripTopMenu = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newFileMenuitem = new System.Windows.Forms.ToolStripMenuItem();
			basicModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			chunkModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			gamecubeModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			modelListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			loadTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addTexturestoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			unloadTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			loadTexlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			unloadTexlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
			loadAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAnimationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			renderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			cStructsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			nJAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			aSSIMPExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			materialEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			exportAnimationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportTextureNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newModelUnloadsTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
			exportNJTLChunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			nJChunkSizeInLittleEndianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showNodeConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showVertexIndicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showWeightsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showHintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showAdvancedCameraInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			alternativeCameraModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			modelCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			modelInfoEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
			setDefaultAnimationOrientationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			textureRemappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			swapUVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
			importLabelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportLabelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			resetLabelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
			modelLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			welcomeTutorialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editMaterialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			RenderPanel = new System.Windows.Forms.UserControl();
			backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			BackgroundPanel = new System.Windows.Forms.Panel();
			splitContainer2 = new System.Windows.Forms.SplitContainer();
			treeView1 = new System.Windows.Forms.TreeView();
			propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			toolStripButtonBar = new System.Windows.Forms.ToolStrip();
			buttonNew = new System.Windows.Forms.ToolStripButton();
			buttonOpen = new System.Windows.Forms.ToolStripButton();
			buttonModelList = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
			buttonSave = new System.Windows.Forms.ToolStripButton();
			buttonSaveAs = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			buttonShowNodes = new System.Windows.Forms.ToolStripButton();
			buttonShowNodeConnections = new System.Windows.Forms.ToolStripButton();
			buttonShowVertexIndices = new System.Windows.Forms.ToolStripButton();
			buttonShowWeights = new System.Windows.Forms.ToolStripButton();
			buttonLighting = new System.Windows.Forms.ToolStripButton();
			buttonTextures = new System.Windows.Forms.ToolStripButton();
			buttonMaterialColors = new System.Windows.Forms.ToolStripButton();
			buttonShowHints = new System.Windows.Forms.ToolStripButton();
			buttonPreferences = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			buttonSolid = new System.Windows.Forms.ToolStripButton();
			buttonVertices = new System.Windows.Forms.ToolStripButton();
			buttonWireframe = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			buttonPrevAnimation = new System.Windows.Forms.ToolStripButton();
			buttonNextAnimation = new System.Windows.Forms.ToolStripButton();
			buttonPrevFrame = new System.Windows.Forms.ToolStripButton();
			buttonPlayAnimation = new System.Windows.Forms.ToolStripButton();
			buttonNextFrame = new System.Windows.Forms.ToolStripButton();
			buttonResetFrame = new System.Windows.Forms.ToolStripButton();
			comboAnimList = new System.Windows.Forms.ToolStripComboBox();
			contextMenuStripRightClick = new System.Windows.Forms.ContextMenuStrip(components);
			copyModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			pasteModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			editModelDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			PolyNormalstoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			createPolyNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			removePolyNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			importContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			importSelectedAsMeshContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			importSelectedAsNodesContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			selectedLegacyOBJImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			addChildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			splitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			byMeshsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			byFaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			transparentOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			sortMeshsetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			hierarchyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
			deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			clearChildrenToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			emptyModelDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			cameraPosLabel = new System.Windows.Forms.ToolStripStatusLabel();
			cameraAngleLabel = new System.Windows.Forms.ToolStripStatusLabel();
			cameraModeLabel = new System.Windows.Forms.ToolStripStatusLabel();
			animNameLabel = new System.Windows.Forms.ToolStripStatusLabel();
			animDescriptionLabel = new System.Windows.Forms.ToolStripStatusLabel();
			animFrameLabel = new System.Windows.Forms.ToolStripStatusLabel();
			MessageTimer = new System.Windows.Forms.Timer(components);
			contextMenuStripNew = new System.Windows.Forms.ContextMenuStrip(components);
			basicModelToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			chunkModelToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			gamecubeModelToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			menuStripTopMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
			splitContainer2.Panel1.SuspendLayout();
			splitContainer2.Panel2.SuspendLayout();
			splitContainer2.SuspendLayout();
			toolStripButtonBar.SuspendLayout();
			contextMenuStripRightClick.SuspendLayout();
			statusStrip1.SuspendLayout();
			contextMenuStripNew.SuspendLayout();
			SuspendLayout();
			// 
			// menuStripTopMenu
			// 
			menuStripTopMenu.BackColor = System.Drawing.SystemColors.MenuBar;
			menuStripTopMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			menuStripTopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
			menuStripTopMenu.Location = new System.Drawing.Point(0, 0);
			menuStripTopMenu.Name = "menuStripTopMenu";
			menuStripTopMenu.Padding = new System.Windows.Forms.Padding(5, 1, 0, 1);
			menuStripTopMenu.Size = new System.Drawing.Size(1107, 24);
			menuStripTopMenu.TabIndex = 0;
			menuStripTopMenu.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newFileMenuitem, openToolStripMenuItem, modelListToolStripMenuItem, toolStripSeparator1, loadTexturesToolStripMenuItem, addTexturestoolStripMenuItem, unloadTextureToolStripMenuItem, toolStripSeparator14, loadTexlistToolStripMenuItem, unloadTexlistToolStripMenuItem, toolStripSeparator15, loadAnimationToolStripMenuItem, toolStripSeparator2, saveMenuItem, saveAsToolStripMenuItem, saveAnimationsToolStripMenuItem, toolStripSeparator3, renderToolStripMenuItem, importToolStripMenuItem, exportToolStripMenuItem, toolStripSeparator5, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
			fileToolStripMenuItem.Text = "&File";
			// 
			// newFileMenuitem
			// 
			newFileMenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { basicModelToolStripMenuItem, chunkModelToolStripMenuItem, gamecubeModelToolStripMenuItem });
			newFileMenuitem.Image = Properties.Resources._new;
			newFileMenuitem.Name = "newFileMenuitem";
			newFileMenuitem.Size = new System.Drawing.Size(243, 22);
			newFileMenuitem.Text = "&New";
			// 
			// basicModelToolStripMenuItem
			// 
			basicModelToolStripMenuItem.Name = "basicModelToolStripMenuItem";
			basicModelToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			basicModelToolStripMenuItem.Text = "&Basic Model";
			basicModelToolStripMenuItem.Click += basicModelToolStripMenuItem_Click;
			// 
			// chunkModelToolStripMenuItem
			// 
			chunkModelToolStripMenuItem.Name = "chunkModelToolStripMenuItem";
			chunkModelToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			chunkModelToolStripMenuItem.Text = "&Chunk Model";
			chunkModelToolStripMenuItem.Click += chunkModelToolStripMenuItem_Click;
			// 
			// gamecubeModelToolStripMenuItem
			// 
			gamecubeModelToolStripMenuItem.Name = "gamecubeModelToolStripMenuItem";
			gamecubeModelToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			gamecubeModelToolStripMenuItem.Text = "Gamecube Model";
			gamecubeModelToolStripMenuItem.Click += GamecubeModelToolStripMenuItem_Click;
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Image = Properties.Resources.open;
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
			openToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			openToolStripMenuItem.Text = "&Open...";
			openToolStripMenuItem.Click += openToolStripMenuItem_Click;
			// 
			// modelListToolStripMenuItem
			// 
			modelListToolStripMenuItem.Enabled = false;
			modelListToolStripMenuItem.Name = "modelListToolStripMenuItem";
			modelListToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.O;
			modelListToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			modelListToolStripMenuItem.Text = "Model List...";
			modelListToolStripMenuItem.Click += modelListToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(240, 6);
			// 
			// loadTexturesToolStripMenuItem
			// 
			loadTexturesToolStripMenuItem.Name = "loadTexturesToolStripMenuItem";
			loadTexturesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T;
			loadTexturesToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			loadTexturesToolStripMenuItem.Text = "Load Textures...";
			loadTexturesToolStripMenuItem.ToolTipText = "Load a PVM/GVM/XVM, PRS, PAK etc. file.";
			loadTexturesToolStripMenuItem.Click += loadTexturesToolStripMenuItem_Click;
			// 
			// addTexturestoolStripMenuItem
			// 
			addTexturestoolStripMenuItem.Name = "addTexturestoolStripMenuItem";
			addTexturestoolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			addTexturestoolStripMenuItem.Text = "Add Textures...";
			addTexturestoolStripMenuItem.ToolTipText = "Add individual textures from PVR, GVR, or XVR files.";
			addTexturestoolStripMenuItem.Click += addTexturestoolStripMenuItem_Click;
			// 
			// unloadTextureToolStripMenuItem
			// 
			unloadTextureToolStripMenuItem.Enabled = false;
			unloadTextureToolStripMenuItem.Name = "unloadTextureToolStripMenuItem";
			unloadTextureToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			unloadTextureToolStripMenuItem.Text = "Unload Textures";
			unloadTextureToolStripMenuItem.ToolTipText = "Clear all loaded textures.";
			unloadTextureToolStripMenuItem.Click += unloadTextureToolStripMenuItem_Click;
			// 
			// toolStripSeparator14
			// 
			toolStripSeparator14.Name = "toolStripSeparator14";
			toolStripSeparator14.Size = new System.Drawing.Size(240, 6);
			// 
			// loadTexlistToolStripMenuItem
			// 
			loadTexlistToolStripMenuItem.Name = "loadTexlistToolStripMenuItem";
			loadTexlistToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.T;
			loadTexlistToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			loadTexlistToolStripMenuItem.Text = "Load Texture List...";
			loadTexlistToolStripMenuItem.ToolTipText = "Load a texture list from a .SATEX file. Useful for models that use partial texture lists.";
			loadTexlistToolStripMenuItem.Click += loadTexlistToolStripMenuItem_Click;
			// 
			// unloadTexlistToolStripMenuItem
			// 
			unloadTexlistToolStripMenuItem.Enabled = false;
			unloadTexlistToolStripMenuItem.Name = "unloadTexlistToolStripMenuItem";
			unloadTexlistToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			unloadTexlistToolStripMenuItem.Text = "Unload Texture List";
			unloadTexlistToolStripMenuItem.ToolTipText = "Unloads the partial texture list.";
			unloadTexlistToolStripMenuItem.Click += unloadTexlistToolStripMenuItem_Click;
			// 
			// toolStripSeparator15
			// 
			toolStripSeparator15.Name = "toolStripSeparator15";
			toolStripSeparator15.Size = new System.Drawing.Size(240, 6);
			// 
			// loadAnimationToolStripMenuItem
			// 
			loadAnimationToolStripMenuItem.Enabled = false;
			loadAnimationToolStripMenuItem.Name = "loadAnimationToolStripMenuItem";
			loadAnimationToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			loadAnimationToolStripMenuItem.Text = "Load Animation...";
			loadAnimationToolStripMenuItem.Click += loadAnimationToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(240, 6);
			// 
			// saveMenuItem
			// 
			saveMenuItem.Enabled = false;
			saveMenuItem.Image = Properties.Resources.save;
			saveMenuItem.Name = "saveMenuItem";
			saveMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
			saveMenuItem.Size = new System.Drawing.Size(243, 22);
			saveMenuItem.Text = "&Save Model";
			saveMenuItem.Click += saveMenuItem_Click;
			// 
			// saveAsToolStripMenuItem
			// 
			saveAsToolStripMenuItem.Enabled = false;
			saveAsToolStripMenuItem.Image = Properties.Resources.saveas;
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.S;
			saveAsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			saveAsToolStripMenuItem.Text = "Save Model &As...";
			saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
			// 
			// saveAnimationsToolStripMenuItem
			// 
			saveAnimationsToolStripMenuItem.Enabled = false;
			saveAnimationsToolStripMenuItem.Name = "saveAnimationsToolStripMenuItem";
			saveAnimationsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			saveAnimationsToolStripMenuItem.Text = "Save Ani&mation...";
			saveAnimationsToolStripMenuItem.Click += saveAnimationsToolStripMenuItem1_Click;
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(240, 6);
			// 
			// renderToolStripMenuItem
			// 
			renderToolStripMenuItem.Enabled = false;
			renderToolStripMenuItem.Name = "renderToolStripMenuItem";
			renderToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			renderToolStripMenuItem.Text = "&Render...";
			renderToolStripMenuItem.ToolTipText = "Renders the current view to an image file.";
			renderToolStripMenuItem.Click += renderToolStripMenuItem_Click;
			// 
			// importToolStripMenuItem
			// 
			importToolStripMenuItem.Enabled = false;
			importToolStripMenuItem.Image = Properties.Resources.import;
			importToolStripMenuItem.Name = "importToolStripMenuItem";
			importToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			importToolStripMenuItem.Text = "&Import...";
			importToolStripMenuItem.Click += importToolStripMenuItem_Click;
			// 
			// exportToolStripMenuItem
			// 
			exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { cStructsToolStripMenuItem, nJAToolStripMenuItem, aSSIMPExportToolStripMenuItem });
			exportToolStripMenuItem.Enabled = false;
			exportToolStripMenuItem.Image = Properties.Resources.export;
			exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			exportToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			exportToolStripMenuItem.Text = "&Export";
			// 
			// cStructsToolStripMenuItem
			// 
			cStructsToolStripMenuItem.Name = "cStructsToolStripMenuItem";
			cStructsToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
			cStructsToolStripMenuItem.Text = "C &Structs...";
			cStructsToolStripMenuItem.Click += cStructsToolStripMenuItem_Click;
			// 
			// nJAToolStripMenuItem
			// 
			nJAToolStripMenuItem.Name = "nJAToolStripMenuItem";
			nJAToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
			nJAToolStripMenuItem.Text = "NJA...";
			nJAToolStripMenuItem.Click += nJAToolStripMenuItem_Click;
			// 
			// aSSIMPExportToolStripMenuItem
			// 
			aSSIMPExportToolStripMenuItem.Name = "aSSIMPExportToolStripMenuItem";
			aSSIMPExportToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
			aSSIMPExportToolStripMenuItem.Text = "Assimp...";
			aSSIMPExportToolStripMenuItem.Click += aSSIMPExportToolStripMenuItem_Click;
			// 
			// toolStripSeparator5
			// 
			toolStripSeparator5.Name = "toolStripSeparator5";
			toolStripSeparator5.Size = new System.Drawing.Size(240, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
			exitToolStripMenuItem.Text = "E&xit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { preferencesToolStripMenuItem, findToolStripMenuItem, materialEditorToolStripMenuItem, toolStripSeparator4, exportAnimationsToolStripMenuItem, exportTextureNamesToolStripMenuItem, newModelUnloadsTexturesToolStripMenuItem, toolStripSeparator21, exportNJTLChunkToolStripMenuItem, nJChunkSizeInLittleEndianToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(39, 22);
			editToolStripMenuItem.Text = "&Edit";
			// 
			// preferencesToolStripMenuItem
			// 
			preferencesToolStripMenuItem.Image = Properties.Resources.settings;
			preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			preferencesToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
			preferencesToolStripMenuItem.Text = "Preferences";
			preferencesToolStripMenuItem.Click += preferencesToolStripMenuItem_Click;
			// 
			// findToolStripMenuItem
			// 
			findToolStripMenuItem.Enabled = false;
			findToolStripMenuItem.Image = Properties.Resources.find;
			findToolStripMenuItem.Name = "findToolStripMenuItem";
			findToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F;
			findToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
			findToolStripMenuItem.Text = "&Find...";
			findToolStripMenuItem.Click += findToolStripMenuItem_Click;
			// 
			// materialEditorToolStripMenuItem
			// 
			materialEditorToolStripMenuItem.Enabled = false;
			materialEditorToolStripMenuItem.Name = "materialEditorToolStripMenuItem";
			materialEditorToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
			materialEditorToolStripMenuItem.Text = "Material Editor...";
			materialEditorToolStripMenuItem.Click += materialEditorToolStripMenuItem_Click;
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(234, 6);
			// 
			// exportAnimationsToolStripMenuItem
			// 
			exportAnimationsToolStripMenuItem.Checked = true;
			exportAnimationsToolStripMenuItem.CheckOnClick = true;
			exportAnimationsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			exportAnimationsToolStripMenuItem.Name = "exportAnimationsToolStripMenuItem";
			exportAnimationsToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
			exportAnimationsToolStripMenuItem.Text = "Export Motion Structs";
			exportAnimationsToolStripMenuItem.ToolTipText = "Export animations in C structs.";
			// 
			// exportTextureNamesToolStripMenuItem
			// 
			exportTextureNamesToolStripMenuItem.CheckOnClick = true;
			exportTextureNamesToolStripMenuItem.Name = "exportTextureNamesToolStripMenuItem";
			exportTextureNamesToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
			exportTextureNamesToolStripMenuItem.Text = "Export Texture Enums";
			exportTextureNamesToolStripMenuItem.ToolTipText = "Use texture names in materials instead of texture IDs when exporting C structs.";
			// 
			// newModelUnloadsTexturesToolStripMenuItem
			// 
			newModelUnloadsTexturesToolStripMenuItem.CheckOnClick = true;
			newModelUnloadsTexturesToolStripMenuItem.Name = "newModelUnloadsTexturesToolStripMenuItem";
			newModelUnloadsTexturesToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
			newModelUnloadsTexturesToolStripMenuItem.Text = "Auto Unload Textures";
			newModelUnloadsTexturesToolStripMenuItem.ToolTipText = "Unload textures when a new model is loaded or created.";
			// 
			// toolStripSeparator21
			// 
			toolStripSeparator21.Name = "toolStripSeparator21";
			toolStripSeparator21.Size = new System.Drawing.Size(234, 6);
			// 
			// exportNJTLChunkToolStripMenuItem
			// 
			exportNJTLChunkToolStripMenuItem.CheckOnClick = true;
			exportNJTLChunkToolStripMenuItem.Name = "exportNJTLChunkToolStripMenuItem";
			exportNJTLChunkToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
			exportNJTLChunkToolStripMenuItem.Text = "Export NJTL Chunk";
			exportNJTLChunkToolStripMenuItem.ToolTipText = "When saving NJ/GJ files, save the NJTL/GJTL chunk as well, if it exists.";
			exportNJTLChunkToolStripMenuItem.Click += exportNJTLChunkToolStripMenuItem_Click;
			// 
			// nJChunkSizeInLittleEndianToolStripMenuItem
			// 
			nJChunkSizeInLittleEndianToolStripMenuItem.CheckOnClick = true;
			nJChunkSizeInLittleEndianToolStripMenuItem.Name = "nJChunkSizeInLittleEndianToolStripMenuItem";
			nJChunkSizeInLittleEndianToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
			nJChunkSizeInLittleEndianToolStripMenuItem.Text = "NJ Chunk Size in Little Endian";
			nJChunkSizeInLittleEndianToolStripMenuItem.ToolTipText = "Force Little Endian for chunk size in Ninja Binary files, regardless of the data's Endianness.";
			nJChunkSizeInLittleEndianToolStripMenuItem.Click += nJChunkSizeInLittleEndianToolStripMenuItem_Click;
			// 
			// viewToolStripMenuItem
			// 
			viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { showModelToolStripMenuItem, showNodesToolStripMenuItem, showNodeConnectionsToolStripMenuItem, showVertexIndicesToolStripMenuItem, showWeightsToolStripMenuItem, showHintsToolStripMenuItem, showAdvancedCameraInfoToolStripMenuItem, alternativeCameraModeToolStripMenuItem });
			viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			viewToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
			viewToolStripMenuItem.Text = "&View";
			// 
			// showModelToolStripMenuItem
			// 
			showModelToolStripMenuItem.Checked = true;
			showModelToolStripMenuItem.CheckOnClick = true;
			showModelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			showModelToolStripMenuItem.Name = "showModelToolStripMenuItem";
			showModelToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			showModelToolStripMenuItem.Text = "Show &Model";
			showModelToolStripMenuItem.CheckedChanged += showModelToolStripMenuItem_CheckedChanged;
			// 
			// showNodesToolStripMenuItem
			// 
			showNodesToolStripMenuItem.CheckOnClick = true;
			showNodesToolStripMenuItem.Image = Properties.Resources.node;
			showNodesToolStripMenuItem.Name = "showNodesToolStripMenuItem";
			showNodesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			showNodesToolStripMenuItem.Text = "Show &Nodes";
			showNodesToolStripMenuItem.CheckedChanged += showNodesToolStripMenuItem_CheckedChanged;
			// 
			// showNodeConnectionsToolStripMenuItem
			// 
			showNodeConnectionsToolStripMenuItem.CheckOnClick = true;
			showNodeConnectionsToolStripMenuItem.Image = Properties.Resources.nodecon;
			showNodeConnectionsToolStripMenuItem.Name = "showNodeConnectionsToolStripMenuItem";
			showNodeConnectionsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			showNodeConnectionsToolStripMenuItem.Text = "Show Node &Connections";
			showNodeConnectionsToolStripMenuItem.CheckedChanged += showNodeConnectionsToolStripMenuItem_CheckedChanged;
			// 
			// showVertexIndicesToolStripMenuItem
			// 
			showVertexIndicesToolStripMenuItem.CheckOnClick = true;
			showVertexIndicesToolStripMenuItem.Image = Properties.Resources.vertid;
			showVertexIndicesToolStripMenuItem.Name = "showVertexIndicesToolStripMenuItem";
			showVertexIndicesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			showVertexIndicesToolStripMenuItem.Text = "Show Vertex Indices";
			showVertexIndicesToolStripMenuItem.CheckedChanged += showVertexIndicesToolStripMenuItem_CheckedChanged;
			// 
			// showWeightsToolStripMenuItem
			// 
			showWeightsToolStripMenuItem.CheckOnClick = true;
			showWeightsToolStripMenuItem.Enabled = false;
			showWeightsToolStripMenuItem.Image = Properties.Resources.weights;
			showWeightsToolStripMenuItem.Name = "showWeightsToolStripMenuItem";
			showWeightsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			showWeightsToolStripMenuItem.Text = "Show &Weights";
			showWeightsToolStripMenuItem.CheckedChanged += ShowWeightsToolStripMenuItem_CheckedChanged;
			// 
			// showHintsToolStripMenuItem
			// 
			showHintsToolStripMenuItem.Checked = true;
			showHintsToolStripMenuItem.CheckOnClick = true;
			showHintsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			showHintsToolStripMenuItem.Image = Properties.Resources.messages;
			showHintsToolStripMenuItem.Name = "showHintsToolStripMenuItem";
			showHintsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			showHintsToolStripMenuItem.Text = "Show Hints";
			showHintsToolStripMenuItem.ToolTipText = "Show Hints";
			showHintsToolStripMenuItem.CheckedChanged += showHintsToolStripMenuItem_CheckedChanged;
			// 
			// showAdvancedCameraInfoToolStripMenuItem
			// 
			showAdvancedCameraInfoToolStripMenuItem.CheckOnClick = true;
			showAdvancedCameraInfoToolStripMenuItem.Name = "showAdvancedCameraInfoToolStripMenuItem";
			showAdvancedCameraInfoToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			showAdvancedCameraInfoToolStripMenuItem.Text = "Show Camera Rotation";
			showAdvancedCameraInfoToolStripMenuItem.ToolTipText = "Show more details on camera mode, rotation etc.";
			// 
			// alternativeCameraModeToolStripMenuItem
			// 
			alternativeCameraModeToolStripMenuItem.CheckOnClick = true;
			alternativeCameraModeToolStripMenuItem.Name = "alternativeCameraModeToolStripMenuItem";
			alternativeCameraModeToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			alternativeCameraModeToolStripMenuItem.Text = "Alternative Camera Mode";
			alternativeCameraModeToolStripMenuItem.ToolTipText = "Use an alternative control mode for camera that hides the mouse cursor.";
			// 
			// toolsToolStripMenuItem
			// 
			toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { modelCodeToolStripMenuItem, modelInfoEditorToolStripMenuItem, toolStripSeparator20, setDefaultAnimationOrientationToolStripMenuItem, textureRemappingToolStripMenuItem, swapUVToolStripMenuItem, toolStripSeparator18, importLabelsToolStripMenuItem, exportLabelsToolStripMenuItem, resetLabelsToolStripMenuItem, toolStripSeparator19, modelLibraryToolStripMenuItem });
			toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 22);
			toolsToolStripMenuItem.Text = "&Tools";
			// 
			// modelCodeToolStripMenuItem
			// 
			modelCodeToolStripMenuItem.Enabled = false;
			modelCodeToolStripMenuItem.Image = Properties.Resources.source;
			modelCodeToolStripMenuItem.Name = "modelCodeToolStripMenuItem";
			modelCodeToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
			modelCodeToolStripMenuItem.Text = "View Model Code";
			modelCodeToolStripMenuItem.ToolTipText = "View model exported as C structs.";
			modelCodeToolStripMenuItem.Click += modelCodeToolStripMenuItem_Click;
			// 
			// modelInfoEditorToolStripMenuItem
			// 
			modelInfoEditorToolStripMenuItem.Enabled = false;
			modelInfoEditorToolStripMenuItem.Image = Properties.Resources.editinfo;
			modelInfoEditorToolStripMenuItem.Name = "modelInfoEditorToolStripMenuItem";
			modelInfoEditorToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
			modelInfoEditorToolStripMenuItem.Text = "Edit Model Info...";
			modelInfoEditorToolStripMenuItem.ToolTipText = "Edit the model's author, description and animation list.";
			modelInfoEditorToolStripMenuItem.Click += modelInfoEditorToolStripMenuItem_Click;
			// 
			// toolStripSeparator20
			// 
			toolStripSeparator20.Name = "toolStripSeparator20";
			toolStripSeparator20.Size = new System.Drawing.Size(269, 6);
			// 
			// setDefaultAnimationOrientationToolStripMenuItem
			// 
			setDefaultAnimationOrientationToolStripMenuItem.Enabled = false;
			setDefaultAnimationOrientationToolStripMenuItem.Name = "setDefaultAnimationOrientationToolStripMenuItem";
			setDefaultAnimationOrientationToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
			setDefaultAnimationOrientationToolStripMenuItem.Text = "Set Anim Orientation (Loaded Anims)";
			setDefaultAnimationOrientationToolStripMenuItem.Click += setDefaultAnimationOrientationToolStripMenuItem_Click;
			// 
			// textureRemappingToolStripMenuItem
			// 
			textureRemappingToolStripMenuItem.Enabled = false;
			textureRemappingToolStripMenuItem.Name = "textureRemappingToolStripMenuItem";
			textureRemappingToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
			textureRemappingToolStripMenuItem.Text = "&Texture Remapping...";
			textureRemappingToolStripMenuItem.ToolTipText = "Change material texture IDs in bulk.";
			textureRemappingToolStripMenuItem.Click += textureRemappingToolStripMenuItem_Click;
			// 
			// swapUVToolStripMenuItem
			// 
			swapUVToolStripMenuItem.Enabled = false;
			swapUVToolStripMenuItem.Name = "swapUVToolStripMenuItem";
			swapUVToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
			swapUVToolStripMenuItem.Text = "Swap U/V";
			swapUVToolStripMenuItem.ToolTipText = "Switch Us and Vs around. For Basic models only.";
			swapUVToolStripMenuItem.Click += swapUVToolStripMenuItem_Click;
			// 
			// toolStripSeparator18
			// 
			toolStripSeparator18.Name = "toolStripSeparator18";
			toolStripSeparator18.Size = new System.Drawing.Size(269, 6);
			// 
			// importLabelsToolStripMenuItem
			// 
			importLabelsToolStripMenuItem.Enabled = false;
			importLabelsToolStripMenuItem.Name = "importLabelsToolStripMenuItem";
			importLabelsToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
			importLabelsToolStripMenuItem.Text = "Import Labels...";
			importLabelsToolStripMenuItem.ToolTipText = "Import all data labels for this model hierarchy from an .salabel file.";
			importLabelsToolStripMenuItem.Click += importLabelsToolStripMenuItem_Click;
			// 
			// exportLabelsToolStripMenuItem
			// 
			exportLabelsToolStripMenuItem.Enabled = false;
			exportLabelsToolStripMenuItem.Name = "exportLabelsToolStripMenuItem";
			exportLabelsToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
			exportLabelsToolStripMenuItem.Text = "Export Labels...";
			exportLabelsToolStripMenuItem.ToolTipText = "Export all data labels for this model hierarchy to an .salabel file.";
			exportLabelsToolStripMenuItem.Click += exportLabelsToolStripMenuItem_Click;
			// 
			// resetLabelsToolStripMenuItem
			// 
			resetLabelsToolStripMenuItem.Enabled = false;
			resetLabelsToolStripMenuItem.Name = "resetLabelsToolStripMenuItem";
			resetLabelsToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
			resetLabelsToolStripMenuItem.Text = "Reset Labels";
			resetLabelsToolStripMenuItem.ToolTipText = "Generate random labels for all data structures in the model.";
			resetLabelsToolStripMenuItem.Click += resetLabelsToolStripMenuItem_Click;
			// 
			// toolStripSeparator19
			// 
			toolStripSeparator19.Name = "toolStripSeparator19";
			toolStripSeparator19.Size = new System.Drawing.Size(269, 6);
			// 
			// modelLibraryToolStripMenuItem
			// 
			modelLibraryToolStripMenuItem.Name = "modelLibraryToolStripMenuItem";
			modelLibraryToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
			modelLibraryToolStripMenuItem.Text = "Model &Library...";
			modelLibraryToolStripMenuItem.Click += modelLibraryToolStripMenuItem_Click;
			// 
			// helpToolStripMenuItem
			// 
			helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { welcomeTutorialToolStripMenuItem });
			helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
			helpToolStripMenuItem.Text = "&Help";
			// 
			// welcomeTutorialToolStripMenuItem
			// 
			welcomeTutorialToolStripMenuItem.Image = Properties.Resources.help;
			welcomeTutorialToolStripMenuItem.Name = "welcomeTutorialToolStripMenuItem";
			welcomeTutorialToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			welcomeTutorialToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			welcomeTutorialToolStripMenuItem.Text = "Welcome / Tutorial";
			welcomeTutorialToolStripMenuItem.Click += welcomeTutorialToolStripMenuItem_Click;
			// 
			// editMaterialsToolStripMenuItem
			// 
			editMaterialsToolStripMenuItem.Enabled = false;
			editMaterialsToolStripMenuItem.Image = Properties.Resources.texture;
			editMaterialsToolStripMenuItem.Name = "editMaterialsToolStripMenuItem";
			editMaterialsToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			editMaterialsToolStripMenuItem.Text = "Edit &Materials...";
			editMaterialsToolStripMenuItem.Click += editMaterialsToolStripMenuItem_Click;
			// 
			// RenderPanel
			// 
			RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			RenderPanel.Location = new System.Drawing.Point(0, 0);
			RenderPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			RenderPanel.Name = "RenderPanel";
			RenderPanel.Size = new System.Drawing.Size(741, 615);
			RenderPanel.TabIndex = 1;
			RenderPanel.SizeChanged += RenderPanel_SizeChanged;
			RenderPanel.KeyDown += panel1_KeyDown;
			RenderPanel.KeyUp += panel1_KeyUp;
			RenderPanel.MouseDown += panel1_MouseDown;
			RenderPanel.MouseMove += Panel1_MouseMove;
			RenderPanel.MouseUp += panel1_MouseUp;
			// 
			// splitContainer1
			// 
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			splitContainer1.Location = new System.Drawing.Point(0, 67);
			splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(RenderPanel);
			splitContainer1.Panel1.Controls.Add(BackgroundPanel);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(splitContainer2);
			splitContainer1.Size = new System.Drawing.Size(1107, 615);
			splitContainer1.SplitterDistance = 741;
			splitContainer1.SplitterWidth = 5;
			splitContainer1.TabIndex = 2;
			// 
			// BackgroundPanel
			// 
			BackgroundPanel.AutoSize = true;
			BackgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			BackgroundPanel.Location = new System.Drawing.Point(0, 0);
			BackgroundPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			BackgroundPanel.Name = "BackgroundPanel";
			BackgroundPanel.Size = new System.Drawing.Size(741, 615);
			BackgroundPanel.TabIndex = 2;
			// 
			// splitContainer2
			// 
			splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer2.Location = new System.Drawing.Point(0, 0);
			splitContainer2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			splitContainer2.Name = "splitContainer2";
			splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			splitContainer2.Panel1.Controls.Add(treeView1);
			// 
			// splitContainer2.Panel2
			// 
			splitContainer2.Panel2.Controls.Add(propertyGrid1);
			splitContainer2.Size = new System.Drawing.Size(361, 615);
			splitContainer2.SplitterDistance = 302;
			splitContainer2.SplitterWidth = 5;
			splitContainer2.TabIndex = 16;
			// 
			// treeView1
			// 
			treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			treeView1.HideSelection = false;
			treeView1.Location = new System.Drawing.Point(0, 0);
			treeView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			treeView1.Name = "treeView1";
			treeView1.Size = new System.Drawing.Size(361, 302);
			treeView1.TabIndex = 1;
			treeView1.AfterSelect += treeView1_AfterSelect;
			// 
			// propertyGrid1
			// 
			propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDarkDark;
			propertyGrid1.Location = new System.Drawing.Point(0, 0);
			propertyGrid1.Margin = new System.Windows.Forms.Padding(0);
			propertyGrid1.Name = "propertyGrid1";
			propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			propertyGrid1.Size = new System.Drawing.Size(361, 308);
			propertyGrid1.TabIndex = 14;
			propertyGrid1.ToolbarVisible = false;
			propertyGrid1.PropertyValueChanged += propertyGrid1_PropertyValueChanged;
			// 
			// toolStripButtonBar
			// 
			toolStripButtonBar.BackColor = System.Drawing.SystemColors.Control;
			toolStripButtonBar.ImageScalingSize = new System.Drawing.Size(36, 36);
			toolStripButtonBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { buttonNew, buttonOpen, buttonModelList, toolStripSeparator16, buttonSave, buttonSaveAs, toolStripSeparator10, buttonShowNodes, buttonShowNodeConnections, buttonShowVertexIndices, buttonShowWeights, buttonLighting, buttonTextures, buttonMaterialColors, buttonShowHints, buttonPreferences, toolStripSeparator11, buttonSolid, buttonVertices, buttonWireframe, toolStripSeparator12, buttonPrevAnimation, buttonNextAnimation, buttonPrevFrame, buttonPlayAnimation, buttonNextFrame, buttonResetFrame, comboAnimList });
			toolStripButtonBar.Location = new System.Drawing.Point(0, 24);
			toolStripButtonBar.Name = "toolStripButtonBar";
			toolStripButtonBar.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			toolStripButtonBar.Size = new System.Drawing.Size(1107, 43);
			toolStripButtonBar.TabIndex = 2;
			toolStripButtonBar.Text = "toolStrip1";
			// 
			// buttonNew
			// 
			buttonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonNew.Image = Properties.Resources._new;
			buttonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonNew.Name = "buttonNew";
			buttonNew.Size = new System.Drawing.Size(40, 40);
			buttonNew.Text = "New File";
			buttonNew.Click += buttonNew_Click;
			// 
			// buttonOpen
			// 
			buttonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonOpen.Image = Properties.Resources.open;
			buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonOpen.Name = "buttonOpen";
			buttonOpen.Size = new System.Drawing.Size(40, 40);
			buttonOpen.Text = "Open";
			buttonOpen.Click += buttonOpen_Click;
			// 
			// buttonModelList
			// 
			buttonModelList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonModelList.Enabled = false;
			buttonModelList.Image = Properties.Resources.modellist;
			buttonModelList.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonModelList.Name = "buttonModelList";
			buttonModelList.Size = new System.Drawing.Size(40, 40);
			buttonModelList.Text = "Model List";
			buttonModelList.ToolTipText = "Model List";
			buttonModelList.Click += buttonModelList_Click;
			// 
			// toolStripSeparator16
			// 
			toolStripSeparator16.Name = "toolStripSeparator16";
			toolStripSeparator16.Size = new System.Drawing.Size(6, 43);
			// 
			// buttonSave
			// 
			buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonSave.Enabled = false;
			buttonSave.Image = Properties.Resources.save;
			buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonSave.Name = "buttonSave";
			buttonSave.Size = new System.Drawing.Size(40, 40);
			buttonSave.Text = "Save";
			buttonSave.Click += buttonSave_Click;
			// 
			// buttonSaveAs
			// 
			buttonSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonSaveAs.Enabled = false;
			buttonSaveAs.Image = Properties.Resources.saveas;
			buttonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonSaveAs.Name = "buttonSaveAs";
			buttonSaveAs.Size = new System.Drawing.Size(40, 40);
			buttonSaveAs.Text = "Save as...";
			buttonSaveAs.Click += buttonSaveAs_Click;
			// 
			// toolStripSeparator10
			// 
			toolStripSeparator10.Name = "toolStripSeparator10";
			toolStripSeparator10.Size = new System.Drawing.Size(6, 43);
			// 
			// buttonShowNodes
			// 
			buttonShowNodes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonShowNodes.Image = Properties.Resources.node;
			buttonShowNodes.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonShowNodes.Name = "buttonShowNodes";
			buttonShowNodes.Size = new System.Drawing.Size(40, 40);
			buttonShowNodes.Text = "Show Nodes";
			buttonShowNodes.Click += buttonShowNodes_Click;
			// 
			// buttonShowNodeConnections
			// 
			buttonShowNodeConnections.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonShowNodeConnections.Image = Properties.Resources.nodecon;
			buttonShowNodeConnections.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonShowNodeConnections.Name = "buttonShowNodeConnections";
			buttonShowNodeConnections.Size = new System.Drawing.Size(40, 40);
			buttonShowNodeConnections.Text = "Show Node Connections";
			buttonShowNodeConnections.Click += buttonShowNodeConnections_Click;
			// 
			// buttonShowVertexIndices
			// 
			buttonShowVertexIndices.CheckOnClick = true;
			buttonShowVertexIndices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonShowVertexIndices.Image = Properties.Resources.vertid;
			buttonShowVertexIndices.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonShowVertexIndices.Name = "buttonShowVertexIndices";
			buttonShowVertexIndices.Size = new System.Drawing.Size(40, 40);
			buttonShowVertexIndices.Text = "Show Vertex Indices";
			buttonShowVertexIndices.ToolTipText = "Display vertex indices for the selected model.";
			buttonShowVertexIndices.Click += buttonShowVertexIndices_Click;
			// 
			// buttonShowWeights
			// 
			buttonShowWeights.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonShowWeights.Enabled = false;
			buttonShowWeights.Image = Properties.Resources.weights;
			buttonShowWeights.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonShowWeights.Name = "buttonShowWeights";
			buttonShowWeights.Size = new System.Drawing.Size(40, 40);
			buttonShowWeights.Text = "Show Weights";
			buttonShowWeights.Click += buttonShowWeights_Click;
			// 
			// buttonLighting
			// 
			buttonLighting.Checked = true;
			buttonLighting.CheckState = System.Windows.Forms.CheckState.Checked;
			buttonLighting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonLighting.Image = Properties.Resources.lighting;
			buttonLighting.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonLighting.Name = "buttonLighting";
			buttonLighting.Size = new System.Drawing.Size(40, 40);
			buttonLighting.Text = "Enable Lighting";
			buttonLighting.Click += buttonLighting_Click;
			// 
			// buttonTextures
			// 
			buttonTextures.Checked = true;
			buttonTextures.CheckOnClick = true;
			buttonTextures.CheckState = System.Windows.Forms.CheckState.Checked;
			buttonTextures.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonTextures.Image = Properties.Resources.texture;
			buttonTextures.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonTextures.Name = "buttonTextures";
			buttonTextures.Size = new System.Drawing.Size(40, 40);
			buttonTextures.Text = "toolStripButton1";
			buttonTextures.ToolTipText = "Show/Hide Textures";
			buttonTextures.CheckedChanged += buttonTextures_CheckedChanged;
			// 
			// buttonMaterialColors
			// 
			buttonMaterialColors.Checked = true;
			buttonMaterialColors.CheckOnClick = true;
			buttonMaterialColors.CheckState = System.Windows.Forms.CheckState.Checked;
			buttonMaterialColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonMaterialColors.Image = Properties.Resources.material;
			buttonMaterialColors.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonMaterialColors.Name = "buttonMaterialColors";
			buttonMaterialColors.Size = new System.Drawing.Size(40, 40);
			buttonMaterialColors.Text = "Enable Material Colors";
			buttonMaterialColors.CheckedChanged += buttonMaterialColors_CheckedChanged;
			// 
			// buttonShowHints
			// 
			buttonShowHints.Checked = true;
			buttonShowHints.CheckState = System.Windows.Forms.CheckState.Checked;
			buttonShowHints.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonShowHints.Image = Properties.Resources.messages;
			buttonShowHints.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonShowHints.Name = "buttonShowHints";
			buttonShowHints.Size = new System.Drawing.Size(40, 40);
			buttonShowHints.Text = "Show Hints";
			buttonShowHints.Click += buttonShowHints_Click;
			// 
			// buttonPreferences
			// 
			buttonPreferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonPreferences.Image = Properties.Resources.settings;
			buttonPreferences.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonPreferences.Name = "buttonPreferences";
			buttonPreferences.Size = new System.Drawing.Size(40, 40);
			buttonPreferences.Text = "Preferences";
			buttonPreferences.Click += buttonPreferences_Click;
			// 
			// toolStripSeparator11
			// 
			toolStripSeparator11.Name = "toolStripSeparator11";
			toolStripSeparator11.Size = new System.Drawing.Size(6, 43);
			// 
			// buttonSolid
			// 
			buttonSolid.Checked = true;
			buttonSolid.CheckState = System.Windows.Forms.CheckState.Checked;
			buttonSolid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonSolid.Image = Properties.Resources.solid;
			buttonSolid.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonSolid.Name = "buttonSolid";
			buttonSolid.Size = new System.Drawing.Size(40, 40);
			buttonSolid.Text = "Render Mode: Solid";
			buttonSolid.Click += buttonSolid_Click;
			// 
			// buttonVertices
			// 
			buttonVertices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonVertices.Image = Properties.Resources.point;
			buttonVertices.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonVertices.Name = "buttonVertices";
			buttonVertices.Size = new System.Drawing.Size(40, 40);
			buttonVertices.Text = "Render Mode: Points";
			buttonVertices.Click += buttonVertices_Click;
			// 
			// buttonWireframe
			// 
			buttonWireframe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonWireframe.Image = Properties.Resources.wireframe;
			buttonWireframe.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonWireframe.Name = "buttonWireframe";
			buttonWireframe.Size = new System.Drawing.Size(40, 40);
			buttonWireframe.Text = "Render Mode: Wireframe";
			buttonWireframe.Click += buttonWireframe_Click;
			// 
			// toolStripSeparator12
			// 
			toolStripSeparator12.Name = "toolStripSeparator12";
			toolStripSeparator12.Size = new System.Drawing.Size(6, 43);
			// 
			// buttonPrevAnimation
			// 
			buttonPrevAnimation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonPrevAnimation.Enabled = false;
			buttonPrevAnimation.Image = Properties.Resources.prevanim;
			buttonPrevAnimation.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonPrevAnimation.Name = "buttonPrevAnimation";
			buttonPrevAnimation.Size = new System.Drawing.Size(40, 40);
			buttonPrevAnimation.Text = "Previous Animation";
			buttonPrevAnimation.Click += buttonPrevAnimation_Click;
			// 
			// buttonNextAnimation
			// 
			buttonNextAnimation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonNextAnimation.Enabled = false;
			buttonNextAnimation.Image = Properties.Resources.nextanim;
			buttonNextAnimation.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonNextAnimation.Name = "buttonNextAnimation";
			buttonNextAnimation.Size = new System.Drawing.Size(40, 40);
			buttonNextAnimation.Text = "Next Animation";
			buttonNextAnimation.Click += buttonNextAnimation_Click;
			// 
			// buttonPrevFrame
			// 
			buttonPrevFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonPrevFrame.Enabled = false;
			buttonPrevFrame.Image = Properties.Resources.prevframe;
			buttonPrevFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonPrevFrame.Name = "buttonPrevFrame";
			buttonPrevFrame.Size = new System.Drawing.Size(40, 40);
			buttonPrevFrame.Text = "Previous Frame";
			buttonPrevFrame.Click += buttonPrevFrame_Click;
			// 
			// buttonPlayAnimation
			// 
			buttonPlayAnimation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonPlayAnimation.Enabled = false;
			buttonPlayAnimation.Image = Properties.Resources.playanim;
			buttonPlayAnimation.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonPlayAnimation.Name = "buttonPlayAnimation";
			buttonPlayAnimation.Size = new System.Drawing.Size(40, 40);
			buttonPlayAnimation.Text = "Play/Stop Animation";
			buttonPlayAnimation.Click += buttonPlayAnimation_Click;
			// 
			// buttonNextFrame
			// 
			buttonNextFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonNextFrame.Enabled = false;
			buttonNextFrame.Image = Properties.Resources.nextframe;
			buttonNextFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonNextFrame.Name = "buttonNextFrame";
			buttonNextFrame.Size = new System.Drawing.Size(40, 40);
			buttonNextFrame.Text = "Next Frame";
			buttonNextFrame.Click += buttonNextFrame_Click;
			// 
			// buttonResetFrame
			// 
			buttonResetFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonResetFrame.Enabled = false;
			buttonResetFrame.Image = Properties.Resources.resetanim;
			buttonResetFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonResetFrame.Name = "buttonResetFrame";
			buttonResetFrame.Size = new System.Drawing.Size(40, 40);
			buttonResetFrame.Text = "Reset Animation Frame";
			buttonResetFrame.Click += buttonResetFrame_Click;
			// 
			// comboAnimList
			// 
			comboAnimList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboAnimList.DropDownWidth = 64;
			comboAnimList.Items.AddRange(new object[] { "None" });
			comboAnimList.MaxDropDownItems = 32;
			comboAnimList.Name = "comboAnimList";
			comboAnimList.Size = new System.Drawing.Size(121, 43);
			comboAnimList.SelectedIndexChanged += comboAnimList_SelectedIndexChanged;
			// 
			// contextMenuStripRightClick
			// 
			contextMenuStripRightClick.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { copyModelToolStripMenuItem, pasteModelToolStripMenuItem, toolStripSeparator9, editMaterialsToolStripMenuItem, editModelDataToolStripMenuItem, PolyNormalstoolStripMenuItem, toolStripSeparator8, importContextMenuItem, exportContextMenuItem, toolStripSeparator6, addChildToolStripMenuItem, splitToolStripMenuItem, sortToolStripMenuItem, toolStripSeparator7, moveUpToolStripMenuItem, moveDownToolStripMenuItem, toolStripSeparator17, deleteToolStripMenuItem });
			contextMenuStripRightClick.Name = "contextMenuStrip1";
			contextMenuStripRightClick.Size = new System.Drawing.Size(214, 424);
			// 
			// copyModelToolStripMenuItem
			// 
			copyModelToolStripMenuItem.Enabled = false;
			copyModelToolStripMenuItem.Image = Properties.Resources.copy;
			copyModelToolStripMenuItem.Name = "copyModelToolStripMenuItem";
			copyModelToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C;
			copyModelToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			copyModelToolStripMenuItem.Text = "&Copy Model";
			copyModelToolStripMenuItem.Click += copyModelToolStripMenuItem_Click;
			// 
			// pasteModelToolStripMenuItem
			// 
			pasteModelToolStripMenuItem.Enabled = false;
			pasteModelToolStripMenuItem.Image = Properties.Resources.paste;
			pasteModelToolStripMenuItem.Name = "pasteModelToolStripMenuItem";
			pasteModelToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V;
			pasteModelToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			pasteModelToolStripMenuItem.Text = "&Paste Model";
			pasteModelToolStripMenuItem.Click += pasteModelToolStripMenuItem_Click;
			// 
			// toolStripSeparator9
			// 
			toolStripSeparator9.Name = "toolStripSeparator9";
			toolStripSeparator9.Size = new System.Drawing.Size(210, 6);
			// 
			// editModelDataToolStripMenuItem
			// 
			editModelDataToolStripMenuItem.Enabled = false;
			editModelDataToolStripMenuItem.Image = Properties.Resources.meshset;
			editModelDataToolStripMenuItem.Name = "editModelDataToolStripMenuItem";
			editModelDataToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			editModelDataToolStripMenuItem.Text = "Edit Model Data...";
			editModelDataToolStripMenuItem.Click += editModelDataToolStripMenuItem_Click;
			// 
			// PolyNormalstoolStripMenuItem
			// 
			PolyNormalstoolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { createPolyNormalToolStripMenuItem, removePolyNormalToolStripMenuItem });
			PolyNormalstoolStripMenuItem.Enabled = false;
			PolyNormalstoolStripMenuItem.Name = "PolyNormalstoolStripMenuItem";
			PolyNormalstoolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			PolyNormalstoolStripMenuItem.Text = "Polynormals";
			// 
			// createPolyNormalToolStripMenuItem
			// 
			createPolyNormalToolStripMenuItem.Name = "createPolyNormalToolStripMenuItem";
			createPolyNormalToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			createPolyNormalToolStripMenuItem.Text = "Create";
			createPolyNormalToolStripMenuItem.Click += createToolStripMenuItem_Click;
			// 
			// removePolyNormalToolStripMenuItem
			// 
			removePolyNormalToolStripMenuItem.Name = "removePolyNormalToolStripMenuItem";
			removePolyNormalToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			removePolyNormalToolStripMenuItem.Text = "Remove";
			removePolyNormalToolStripMenuItem.Click += removeToolStripMenuItem_Click;
			// 
			// toolStripSeparator8
			// 
			toolStripSeparator8.Name = "toolStripSeparator8";
			toolStripSeparator8.Size = new System.Drawing.Size(210, 6);
			// 
			// importContextMenuItem
			// 
			importContextMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { importSelectedAsMeshContextMenuItem, importSelectedAsNodesContextMenuItem, selectedLegacyOBJImportToolStripMenuItem });
			importContextMenuItem.Enabled = false;
			importContextMenuItem.Image = Properties.Resources.import;
			importContextMenuItem.Name = "importContextMenuItem";
			importContextMenuItem.Size = new System.Drawing.Size(213, 30);
			importContextMenuItem.Text = "&Import Selected";
			// 
			// importSelectedAsMeshContextMenuItem
			// 
			importSelectedAsMeshContextMenuItem.Name = "importSelectedAsMeshContextMenuItem";
			importSelectedAsMeshContextMenuItem.Size = new System.Drawing.Size(182, 22);
			importSelectedAsMeshContextMenuItem.Text = "As Mesh...";
			importSelectedAsMeshContextMenuItem.Click += importSelectedAsMeshContextMenuItem_Click;
			// 
			// importSelectedAsNodesContextMenuItem
			// 
			importSelectedAsNodesContextMenuItem.Name = "importSelectedAsNodesContextMenuItem";
			importSelectedAsNodesContextMenuItem.Size = new System.Drawing.Size(182, 22);
			importSelectedAsNodesContextMenuItem.Text = "As Child Nodes...";
			importSelectedAsNodesContextMenuItem.Click += importSelectedAsNodesContextMenuItem_Click;
			// 
			// selectedLegacyOBJImportToolStripMenuItem
			// 
			selectedLegacyOBJImportToolStripMenuItem.Name = "selectedLegacyOBJImportToolStripMenuItem";
			selectedLegacyOBJImportToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			selectedLegacyOBJImportToolStripMenuItem.Text = "Legacy OBJ Import...";
			selectedLegacyOBJImportToolStripMenuItem.Click += selectedLegacyOBJImportToolStripMenuItem_Click;
			// 
			// exportContextMenuItem
			// 
			exportContextMenuItem.Enabled = false;
			exportContextMenuItem.Image = Properties.Resources.export;
			exportContextMenuItem.Name = "exportContextMenuItem";
			exportContextMenuItem.Size = new System.Drawing.Size(213, 30);
			exportContextMenuItem.Text = "&Export Selected...";
			exportContextMenuItem.Click += exportContextMenuItem_Click;
			// 
			// toolStripSeparator6
			// 
			toolStripSeparator6.Name = "toolStripSeparator6";
			toolStripSeparator6.Size = new System.Drawing.Size(210, 6);
			// 
			// addChildToolStripMenuItem
			// 
			addChildToolStripMenuItem.Enabled = false;
			addChildToolStripMenuItem.Image = Properties.Resources.addchild;
			addChildToolStripMenuItem.Name = "addChildToolStripMenuItem";
			addChildToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			addChildToolStripMenuItem.Text = "Add Child";
			addChildToolStripMenuItem.Click += addChildToolStripMenuItem_Click;
			// 
			// splitToolStripMenuItem
			// 
			splitToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { byMeshsetToolStripMenuItem, byFaceToolStripMenuItem, toolStripSeparator13, transparentOnlyToolStripMenuItem });
			splitToolStripMenuItem.Enabled = false;
			splitToolStripMenuItem.Image = Properties.Resources.split;
			splitToolStripMenuItem.Name = "splitToolStripMenuItem";
			splitToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			splitToolStripMenuItem.Text = "Split Meshsets";
			// 
			// byMeshsetToolStripMenuItem
			// 
			byMeshsetToolStripMenuItem.Name = "byMeshsetToolStripMenuItem";
			byMeshsetToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			byMeshsetToolStripMenuItem.Text = "By Mesh";
			byMeshsetToolStripMenuItem.ToolTipText = "Create a child model for each meshset in the parent model.";
			byMeshsetToolStripMenuItem.Click += byMeshsetToolStripMenuItem_Click;
			// 
			// byFaceToolStripMenuItem
			// 
			byFaceToolStripMenuItem.Name = "byFaceToolStripMenuItem";
			byFaceToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			byFaceToolStripMenuItem.Text = "By Face";
			byFaceToolStripMenuItem.ToolTipText = "Create a child model for each face in the parent model's meshes.";
			byFaceToolStripMenuItem.Click += byFaceToolStripMenuItem_Click;
			// 
			// toolStripSeparator13
			// 
			toolStripSeparator13.Name = "toolStripSeparator13";
			toolStripSeparator13.Size = new System.Drawing.Size(214, 6);
			// 
			// transparentOnlyToolStripMenuItem
			// 
			transparentOnlyToolStripMenuItem.Checked = true;
			transparentOnlyToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			transparentOnlyToolStripMenuItem.Name = "transparentOnlyToolStripMenuItem";
			transparentOnlyToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			transparentOnlyToolStripMenuItem.Text = "Don't Split Opaque Meshes";
			transparentOnlyToolStripMenuItem.ToolTipText = "Split meshes with transparent polygons as separate models but keep all opaque data in the original model.";
			transparentOnlyToolStripMenuItem.Click += transparentOnlyToolStripMenuItem_Click;
			// 
			// sortToolStripMenuItem
			// 
			sortToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { sortMeshsetsToolStripMenuItem, hierarchyToolStripMenuItem });
			sortToolStripMenuItem.Enabled = false;
			sortToolStripMenuItem.Image = Properties.Resources.sort;
			sortToolStripMenuItem.Name = "sortToolStripMenuItem";
			sortToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			sortToolStripMenuItem.Text = "Sort Transparent Meshes";
			// 
			// sortMeshsetsToolStripMenuItem
			// 
			sortMeshsetsToolStripMenuItem.Name = "sortMeshsetsToolStripMenuItem";
			sortMeshsetsToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			sortMeshsetsToolStripMenuItem.Text = "Model";
			sortMeshsetsToolStripMenuItem.ToolTipText = "Sort the model's meshsets by listing transparent meshes after opaque meshes.";
			sortMeshsetsToolStripMenuItem.Click += sortMeshsetsToolStripMenuItem_Click;
			// 
			// hierarchyToolStripMenuItem
			// 
			hierarchyToolStripMenuItem.Name = "hierarchyToolStripMenuItem";
			hierarchyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			hierarchyToolStripMenuItem.Text = "Model + Children";
			hierarchyToolStripMenuItem.ToolTipText = "Sort the meshsets of all models in the hierarchy.";
			hierarchyToolStripMenuItem.Click += hierarchyToolStripMenuItem_Click;
			// 
			// toolStripSeparator7
			// 
			toolStripSeparator7.Name = "toolStripSeparator7";
			toolStripSeparator7.Size = new System.Drawing.Size(210, 6);
			// 
			// moveUpToolStripMenuItem
			// 
			moveUpToolStripMenuItem.Enabled = false;
			moveUpToolStripMenuItem.Image = Properties.Resources.moveup;
			moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
			moveUpToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			moveUpToolStripMenuItem.Text = "Move Up in Hierarchy";
			moveUpToolStripMenuItem.Click += moveUpToolStripMenuItem_Click;
			// 
			// moveDownToolStripMenuItem
			// 
			moveDownToolStripMenuItem.Enabled = false;
			moveDownToolStripMenuItem.Image = Properties.Resources.movedown;
			moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
			moveDownToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			moveDownToolStripMenuItem.Text = "Move Down in Hierarchy";
			moveDownToolStripMenuItem.Click += moveDownToolStripMenuItem_Click;
			// 
			// toolStripSeparator17
			// 
			toolStripSeparator17.Name = "toolStripSeparator17";
			toolStripSeparator17.Size = new System.Drawing.Size(210, 6);
			// 
			// deleteToolStripMenuItem
			// 
			deleteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { clearChildrenToolStripMenuItem1, emptyModelDataToolStripMenuItem, deleteNodeToolStripMenuItem });
			deleteToolStripMenuItem.Enabled = false;
			deleteToolStripMenuItem.Image = Properties.Resources.delete;
			deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			deleteToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
			deleteToolStripMenuItem.Text = "Delete";
			// 
			// clearChildrenToolStripMenuItem1
			// 
			clearChildrenToolStripMenuItem1.Name = "clearChildrenToolStripMenuItem1";
			clearChildrenToolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
			clearChildrenToolStripMenuItem1.Text = "Children";
			clearChildrenToolStripMenuItem1.Click += clearChildrenToolStripMenuItem1_Click;
			// 
			// emptyModelDataToolStripMenuItem
			// 
			emptyModelDataToolStripMenuItem.Name = "emptyModelDataToolStripMenuItem";
			emptyModelDataToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			emptyModelDataToolStripMenuItem.Text = "Model Data (Attach)";
			emptyModelDataToolStripMenuItem.Click += emptyModelDataToolStripMenuItem_Click;
			// 
			// deleteNodeToolStripMenuItem
			// 
			deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
			deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			deleteNodeToolStripMenuItem.Text = "Node (Object)";
			deleteNodeToolStripMenuItem.Click += deleteTheWholeHierarchyToolStripMenuItem_Click;
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { cameraPosLabel, cameraAngleLabel, cameraModeLabel, animNameLabel, animDescriptionLabel, animFrameLabel });
			statusStrip1.Location = new System.Drawing.Point(0, 682);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
			statusStrip1.Size = new System.Drawing.Size(1107, 24);
			statusStrip1.TabIndex = 3;
			statusStrip1.Text = "statusStrip1";
			// 
			// cameraPosLabel
			// 
			cameraPosLabel.Name = "cameraPosLabel";
			cameraPosLabel.Size = new System.Drawing.Size(177, 19);
			cameraPosLabel.Text = "Camera X: 0.000 Y: 0.000 Z: 0.000";
			// 
			// cameraAngleLabel
			// 
			cameraAngleLabel.Name = "cameraAngleLabel";
			cameraAngleLabel.Size = new System.Drawing.Size(0, 19);
			// 
			// cameraModeLabel
			// 
			cameraModeLabel.Name = "cameraModeLabel";
			cameraModeLabel.Size = new System.Drawing.Size(0, 19);
			// 
			// animNameLabel
			// 
			animNameLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			animNameLabel.Name = "animNameLabel";
			animNameLabel.Size = new System.Drawing.Size(102, 19);
			animNameLabel.Text = "Animation: None";
			// 
			// animDescriptionLabel
			// 
			animDescriptionLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			animDescriptionLabel.Name = "animDescriptionLabel";
			animDescriptionLabel.Size = new System.Drawing.Size(106, 19);
			animDescriptionLabel.Text = "Description: None";
			// 
			// animFrameLabel
			// 
			animFrameLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			animFrameLabel.Name = "animFrameLabel";
			animFrameLabel.Size = new System.Drawing.Size(56, 19);
			animFrameLabel.Text = "Frame: 0";
			// 
			// MessageTimer
			// 
			MessageTimer.Enabled = true;
			MessageTimer.Interval = 1;
			MessageTimer.Tick += MessageTimer_Tick;
			// 
			// contextMenuStripNew
			// 
			contextMenuStripNew.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripNew.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { basicModelToolStripMenuItem1, chunkModelToolStripMenuItem1, gamecubeModelToolStripMenuItem1 });
			contextMenuStripNew.Name = "contextMenuStripNew";
			contextMenuStripNew.Size = new System.Drawing.Size(169, 70);
			// 
			// basicModelToolStripMenuItem1
			// 
			basicModelToolStripMenuItem1.Name = "basicModelToolStripMenuItem1";
			basicModelToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
			basicModelToolStripMenuItem1.Text = "Basic Model";
			basicModelToolStripMenuItem1.Click += basicModelToolStripMenuItem1_Click;
			// 
			// chunkModelToolStripMenuItem1
			// 
			chunkModelToolStripMenuItem1.Name = "chunkModelToolStripMenuItem1";
			chunkModelToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
			chunkModelToolStripMenuItem1.Text = "Chunk Model";
			chunkModelToolStripMenuItem1.Click += chunkModelToolStripMenuItem1_Click;
			// 
			// gamecubeModelToolStripMenuItem1
			// 
			gamecubeModelToolStripMenuItem1.Name = "gamecubeModelToolStripMenuItem1";
			gamecubeModelToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
			gamecubeModelToolStripMenuItem1.Text = "Gamecube Model";
			gamecubeModelToolStripMenuItem1.Click += gamecubeModelToolStripMenuItem1_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(1107, 706);
			Controls.Add(splitContainer1);
			Controls.Add(toolStripButtonBar);
			Controls.Add(menuStripTopMenu);
			Controls.Add(statusStrip1);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MainMenuStrip = menuStripTopMenu;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "MainForm";
			Text = "SAMDL";
			WindowState = System.Windows.Forms.FormWindowState.Maximized;
			Deactivate += MainForm_Deactivate;
			FormClosing += MainForm_FormClosing;
			Load += MainForm_Load;
			ResizeBegin += MainForm_ResizeBegin;
			ResizeEnd += MainForm_ResizeEnd;
			Resize += MainForm_Resize;
			menuStripTopMenu.ResumeLayout(false);
			menuStripTopMenu.PerformLayout();
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			splitContainer2.Panel1.ResumeLayout(false);
			splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
			splitContainer2.ResumeLayout(false);
			toolStripButtonBar.ResumeLayout(false);
			toolStripButtonBar.PerformLayout();
			contextMenuStripRightClick.ResumeLayout(false);
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			contextMenuStripNew.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStripTopMenu;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.UserControl RenderPanel;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadTexturesToolStripMenuItem;
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
		private System.Windows.Forms.ContextMenuStrip contextMenuStripRightClick;
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
		private System.Windows.Forms.ToolStrip toolStripButtonBar;
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
		private System.Windows.Forms.ToolStripMenuItem importSelectedAsMeshContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importSelectedAsNodesContextMenuItem;
		private System.Windows.Forms.ToolStripButton buttonShowVertexIndices;
		private System.Windows.Forms.ToolStripMenuItem showVertexIndicesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectedLegacyOBJImportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resetLabelsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
		private System.Windows.Forms.ToolStripMenuItem loadTexlistToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unloadTexlistToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
		private System.Windows.Forms.ToolStripMenuItem addTexturestoolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem modelListToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton buttonModelList;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
		private System.Windows.Forms.ToolStripButton buttonResetFrame;
		private System.Windows.Forms.ToolStripStatusLabel animDescriptionLabel;
		private System.Windows.Forms.ToolStripMenuItem editModelDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton buttonTextures;
		private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem modelInfoEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
		private System.Windows.Forms.ToolStripMenuItem importLabelsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportLabelsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setDefaultAnimationOrientationToolStripMenuItem;
		private System.Windows.Forms.ToolStripComboBox comboAnimList;
		private System.Windows.Forms.ToolStripMenuItem nJChunkSizeInLittleEndianToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
		private System.Windows.Forms.ToolStripMenuItem exportNJTLChunkToolStripMenuItem;
	}
}
