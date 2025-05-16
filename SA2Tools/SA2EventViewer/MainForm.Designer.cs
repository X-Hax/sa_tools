using System.Windows.Forms.VisualStyles;

namespace SA2EventViewer
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
			menuStrip1 = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showHintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showAdvancedCameraInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			RenderPanel = new System.Windows.Forms.UserControl();
			backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			textBoxEntityName = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			listViewEntities = new System.Windows.Forms.ListView();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			label1 = new System.Windows.Forms.Label();
			numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
			exportSA2MDLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportSA2BMDLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			camModeLabel = new System.Windows.Forms.ToolStripStatusLabel();
			cameraPosLabel = new System.Windows.Forms.ToolStripStatusLabel();
			cameraAngleLabel = new System.Windows.Forms.ToolStripStatusLabel();
			cameraFOVLabel = new System.Windows.Forms.ToolStripStatusLabel();
			sceneNumLabel = new System.Windows.Forms.ToolStripStatusLabel();
			animFrameLabel = new System.Windows.Forms.ToolStripStatusLabel();
			eventFrameLabel = new System.Windows.Forms.ToolStripStatusLabel();
			MessageTimer = new System.Windows.Forms.Timer(components);
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			buttonOpen = new System.Windows.Forms.ToolStripButton();
			buttonSave = new System.Windows.Forms.ToolStripButton();
			buttonSaveAs = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			buttonLighting = new System.Windows.Forms.ToolStripButton();
			buttonMaterialColors = new System.Windows.Forms.ToolStripButton();
			buttonShowHints = new System.Windows.Forms.ToolStripButton();
			buttonPreferences = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			buttonSolid = new System.Windows.Forms.ToolStripButton();
			buttonVertices = new System.Windows.Forms.ToolStripButton();
			buttonWireframe = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			buttonPrevScene = new System.Windows.Forms.ToolStripButton();
			buttonNextScene = new System.Windows.Forms.ToolStripButton();
			buttonPreviousFrame = new System.Windows.Forms.ToolStripButton();
			buttonPlayScene = new System.Windows.Forms.ToolStripButton();
			buttonNextFrame = new System.Windows.Forms.ToolStripButton();
			menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
			contextMenuStrip1.SuspendLayout();
			statusStrip1.SuspendLayout();
			toolStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem });
			menuStrip1.Location = new System.Drawing.Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
			menuStrip1.Size = new System.Drawing.Size(1581, 33);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openToolStripMenuItem, exportToolStripMenuItem, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
			fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Image = Properties.Resources.open;
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.Size = new System.Drawing.Size(170, 34);
			openToolStripMenuItem.Text = "&Open...";
			openToolStripMenuItem.Click += openToolStripMenuItem_Click;
			// 
			// exportToolStripMenuItem
			// 
			exportToolStripMenuItem.Enabled = false;
			exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			exportToolStripMenuItem.Size = new System.Drawing.Size(170, 34);
			exportToolStripMenuItem.Text = "&Export";
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new System.Drawing.Size(170, 34);
			exitToolStripMenuItem.Text = "E&xit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.Checked = true;
			editToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { preferencesToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(58, 29);
			editToolStripMenuItem.Text = "&Edit";
			// 
			// preferencesToolStripMenuItem
			// 
			preferencesToolStripMenuItem.Image = Properties.Resources.settings;
			preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			preferencesToolStripMenuItem.Size = new System.Drawing.Size(204, 34);
			preferencesToolStripMenuItem.Text = "Preferences";
			preferencesToolStripMenuItem.Click += preferencesToolStripMenuItem_Click;
			// 
			// viewToolStripMenuItem
			// 
			viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { showCameraToolStripMenuItem, showHintsToolStripMenuItem, showAdvancedCameraInfoToolStripMenuItem });
			viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			viewToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
			viewToolStripMenuItem.Text = "&View";
			// 
			// showCameraToolStripMenuItem
			// 
			showCameraToolStripMenuItem.Checked = true;
			showCameraToolStripMenuItem.CheckOnClick = true;
			showCameraToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			showCameraToolStripMenuItem.Name = "showCameraToolStripMenuItem";
			showCameraToolStripMenuItem.Size = new System.Drawing.Size(295, 34);
			showCameraToolStripMenuItem.Text = "Show &Camera";
			showCameraToolStripMenuItem.Click += showCameraToolStripMenuItem_Click;
			// 
			// showHintsToolStripMenuItem
			// 
			showHintsToolStripMenuItem.Checked = true;
			showHintsToolStripMenuItem.CheckOnClick = true;
			showHintsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			showHintsToolStripMenuItem.Image = Properties.Resources.messages;
			showHintsToolStripMenuItem.Name = "showHintsToolStripMenuItem";
			showHintsToolStripMenuItem.Size = new System.Drawing.Size(295, 34);
			showHintsToolStripMenuItem.Text = "Show Hints";
			showHintsToolStripMenuItem.ToolTipText = "Show Hints";
			showHintsToolStripMenuItem.CheckedChanged += showHintsToolStripMenuItem_CheckedChanged;
			// 
			// showAdvancedCameraInfoToolStripMenuItem
			// 
			showAdvancedCameraInfoToolStripMenuItem.CheckOnClick = true;
			showAdvancedCameraInfoToolStripMenuItem.Name = "showAdvancedCameraInfoToolStripMenuItem";
			showAdvancedCameraInfoToolStripMenuItem.Size = new System.Drawing.Size(295, 34);
			showAdvancedCameraInfoToolStripMenuItem.Text = "Show Camera Rotation";
			showAdvancedCameraInfoToolStripMenuItem.ToolTipText = "Show more details on camera mode, rotation etc.";
			// 
			// RenderPanel
			// 
			RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			RenderPanel.Location = new System.Drawing.Point(0, 0);
			RenderPanel.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			RenderPanel.Name = "RenderPanel";
			RenderPanel.Size = new System.Drawing.Size(1020, 1056);
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
			splitContainer1.Location = new System.Drawing.Point(0, 78);
			splitContainer1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(RenderPanel);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(textBoxEntityName);
			splitContainer1.Panel2.Controls.Add(label2);
			splitContainer1.Panel2.Controls.Add(listViewEntities);
			splitContainer1.Panel2.Controls.Add(tableLayoutPanel1);
			splitContainer1.Size = new System.Drawing.Size(1581, 1056);
			splitContainer1.SplitterDistance = 1020;
			splitContainer1.SplitterWidth = 7;
			splitContainer1.TabIndex = 2;
			// 
			// textBoxEntityName
			// 
			textBoxEntityName.Location = new System.Drawing.Point(143, 345);
			textBoxEntityName.Name = "textBoxEntityName";
			textBoxEntityName.Size = new System.Drawing.Size(257, 31);
			textBoxEntityName.TabIndex = 3;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(6, 348);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(131, 25);
			label2.TabIndex = 2;
			label2.Text = "Selected Entity:";
			// 
			// listViewEntities
			// 
			listViewEntities.AutoArrange = false;
			listViewEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1 });
			listViewEntities.FullRowSelect = true;
			listViewEntities.GridLines = true;
			listViewEntities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewEntities.Location = new System.Drawing.Point(0, 415);
			listViewEntities.MultiSelect = false;
			listViewEntities.Name = "listViewEntities";
			listViewEntities.Size = new System.Drawing.Size(539, 584);
			listViewEntities.TabIndex = 1;
			listViewEntities.UseCompatibleStateImageBehavior = false;
			listViewEntities.View = System.Windows.Forms.View.Details;
			listViewEntities.SelectedIndexChanged += listViewEntities_SelectedIndexChanged;
			// 
			// columnHeader1
			// 
			columnHeader1.Text = "Entities";
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.Controls.Add(propertyGrid1, 0, 1);
			tableLayoutPanel1.Controls.Add(label1, 0, 0);
			tableLayoutPanel1.Controls.Add(numericUpDown1, 1, 0);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 3;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
			tableLayoutPanel1.Size = new System.Drawing.Size(554, 341);
			tableLayoutPanel1.TabIndex = 0;
			// 
			// propertyGrid1
			// 
			tableLayoutPanel1.SetColumnSpan(propertyGrid1, 2);
			propertyGrid1.Dock = System.Windows.Forms.DockStyle.Top;
			propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDarkDark;
			propertyGrid1.Location = new System.Drawing.Point(0, 41);
			propertyGrid1.Margin = new System.Windows.Forms.Padding(0);
			propertyGrid1.Name = "propertyGrid1";
			propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			propertyGrid1.Size = new System.Drawing.Size(554, 262);
			propertyGrid1.TabIndex = 14;
			propertyGrid1.ToolbarVisible = false;
			propertyGrid1.PropertyValueChanged += propertyGrid1_PropertyValueChanged;
			// 
			// label1
			// 
			label1.Anchor = System.Windows.Forms.AnchorStyles.None;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(6, 8);
			label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(92, 25);
			label1.TabIndex = 15;
			label1.Text = "Timescale:";
			label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// numericUpDown1
			// 
			numericUpDown1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			numericUpDown1.DecimalPlaces = 2;
			numericUpDown1.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
			numericUpDown1.Location = new System.Drawing.Point(110, 5);
			numericUpDown1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			numericUpDown1.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
			numericUpDown1.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
			numericUpDown1.Name = "numericUpDown1";
			numericUpDown1.Size = new System.Drawing.Size(130, 31);
			numericUpDown1.TabIndex = 16;
			numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
			numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
			// 
			// contextMenuStrip1
			// 
			contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { exportSA2MDLToolStripMenuItem, exportSA2BMDLToolStripMenuItem });
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new System.Drawing.Size(220, 68);
			// 
			// exportSA2MDLToolStripMenuItem
			// 
			exportSA2MDLToolStripMenuItem.Enabled = false;
			exportSA2MDLToolStripMenuItem.Name = "exportSA2MDLToolStripMenuItem";
			exportSA2MDLToolStripMenuItem.Size = new System.Drawing.Size(219, 32);
			exportSA2MDLToolStripMenuItem.Text = "Export &SA2MDL";
			exportSA2MDLToolStripMenuItem.Click += exportSA2MDLToolStripMenuItem_Click;
			// 
			// exportSA2BMDLToolStripMenuItem
			// 
			exportSA2BMDLToolStripMenuItem.Enabled = false;
			exportSA2BMDLToolStripMenuItem.Name = "exportSA2BMDLToolStripMenuItem";
			exportSA2BMDLToolStripMenuItem.Size = new System.Drawing.Size(219, 32);
			exportSA2BMDLToolStripMenuItem.Text = "Export SA2BMDL";
			exportSA2BMDLToolStripMenuItem.Click += exportSA2BMDLToolStripMenuItem_Click;
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { camModeLabel, cameraPosLabel, cameraAngleLabel, cameraFOVLabel, sceneNumLabel, animFrameLabel, eventFrameLabel });
			statusStrip1.Location = new System.Drawing.Point(0, 1134);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 23, 0);
			statusStrip1.Size = new System.Drawing.Size(1581, 36);
			statusStrip1.TabIndex = 3;
			statusStrip1.Text = "statusStrip1";
			// 
			// camModeLabel
			// 
			camModeLabel.Name = "camModeLabel";
			camModeLabel.Size = new System.Drawing.Size(96, 29);
			camModeLabel.Text = "Event Cam";
			// 
			// cameraPosLabel
			// 
			cameraPosLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			cameraPosLabel.Name = "cameraPosLabel";
			cameraPosLabel.Size = new System.Drawing.Size(166, 29);
			cameraPosLabel.Text = "Camera Pos: 0, 0, 0";
			// 
			// cameraAngleLabel
			// 
			cameraAngleLabel.Name = "cameraAngleLabel";
			cameraAngleLabel.Size = new System.Drawing.Size(0, 29);
			// 
			// cameraFOVLabel
			// 
			cameraFOVLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			cameraFOVLabel.Name = "cameraFOVLabel";
			cameraFOVLabel.Size = new System.Drawing.Size(69, 29);
			cameraFOVLabel.Text = "FOV: 0";
			// 
			// sceneNumLabel
			// 
			sceneNumLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			sceneNumLabel.Name = "sceneNumLabel";
			sceneNumLabel.Size = new System.Drawing.Size(81, 29);
			sceneNumLabel.Text = "Scene: 0";
			// 
			// animFrameLabel
			// 
			animFrameLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			animFrameLabel.Name = "animFrameLabel";
			animFrameLabel.Size = new System.Drawing.Size(142, 29);
			animFrameLabel.Text = "Scene Frame: -1";
			// 
			// eventFrameLabel
			// 
			eventFrameLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			eventFrameLabel.Name = "eventFrameLabel";
			eventFrameLabel.Size = new System.Drawing.Size(151, 29);
			eventFrameLabel.Text = "Overall Frame: -1";
			eventFrameLabel.Click += toolStripStatusLabel1_Click;
			// 
			// MessageTimer
			// 
			MessageTimer.Enabled = true;
			MessageTimer.Interval = 1;
			MessageTimer.Tick += MessageTimer_Tick;
			// 
			// toolStrip1
			// 
			toolStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { buttonOpen, buttonSave, buttonSaveAs, toolStripSeparator1, buttonLighting, buttonMaterialColors, buttonShowHints, buttonPreferences, toolStripSeparator2, buttonSolid, buttonVertices, buttonWireframe, toolStripSeparator3, buttonPrevScene, buttonNextScene, buttonPreviousFrame, buttonPlayScene, buttonNextFrame });
			toolStrip1.Location = new System.Drawing.Point(0, 33);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			toolStrip1.Size = new System.Drawing.Size(1581, 45);
			toolStrip1.TabIndex = 2;
			toolStrip1.Text = "toolStrip1";
			// 
			// buttonOpen
			// 
			buttonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonOpen.Image = Properties.Resources.open;
			buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonOpen.Name = "buttonOpen";
			buttonOpen.Size = new System.Drawing.Size(40, 40);
			buttonOpen.Text = "Open";
			buttonOpen.ToolTipText = "Open File";
			buttonOpen.Click += buttonOpen_Click;
			// 
			// buttonSave
			// 
			buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonSave.Image = (System.Drawing.Image)resources.GetObject("buttonSave.Image");
			buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonSave.Name = "buttonSave";
			buttonSave.Size = new System.Drawing.Size(40, 40);
			buttonSave.Text = "Save";
			buttonSave.Click += buttonSave_Click;
			// 
			// buttonSaveAs
			// 
			buttonSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonSaveAs.Image = (System.Drawing.Image)resources.GetObject("buttonSaveAs.Image");
			buttonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonSaveAs.Name = "buttonSaveAs";
			buttonSaveAs.Size = new System.Drawing.Size(40, 40);
			buttonSaveAs.Text = "SaveAs";
			buttonSaveAs.ToolTipText = "Save As";
			buttonSaveAs.Click += buttonSaveAs_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(6, 45);
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
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(6, 45);
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
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(6, 45);
			// 
			// buttonPrevScene
			// 
			buttonPrevScene.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonPrevScene.Enabled = false;
			buttonPrevScene.Image = Properties.Resources.prevanim;
			buttonPrevScene.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonPrevScene.Name = "buttonPrevScene";
			buttonPrevScene.Size = new System.Drawing.Size(40, 40);
			buttonPrevScene.Text = "Previous Scene";
			buttonPrevScene.Click += buttonPrevScene_Click;
			// 
			// buttonNextScene
			// 
			buttonNextScene.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonNextScene.Enabled = false;
			buttonNextScene.Image = Properties.Resources.nextanim;
			buttonNextScene.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonNextScene.Name = "buttonNextScene";
			buttonNextScene.Size = new System.Drawing.Size(40, 40);
			buttonNextScene.Text = "Next Scene";
			buttonNextScene.Click += buttonNextScene_Click;
			// 
			// buttonPreviousFrame
			// 
			buttonPreviousFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonPreviousFrame.Enabled = false;
			buttonPreviousFrame.Image = Properties.Resources.prevframe;
			buttonPreviousFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonPreviousFrame.Name = "buttonPreviousFrame";
			buttonPreviousFrame.Size = new System.Drawing.Size(40, 40);
			buttonPreviousFrame.Text = "Previous Frame";
			buttonPreviousFrame.Click += buttonPrevFrame_Click;
			// 
			// buttonPlayScene
			// 
			buttonPlayScene.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			buttonPlayScene.Enabled = false;
			buttonPlayScene.Image = Properties.Resources.playanim;
			buttonPlayScene.ImageTransparentColor = System.Drawing.Color.Magenta;
			buttonPlayScene.Name = "buttonPlayScene";
			buttonPlayScene.Size = new System.Drawing.Size(40, 40);
			buttonPlayScene.Text = "Play/Stop Animation";
			buttonPlayScene.Click += buttonPlayScene_Click;
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
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(1581, 1170);
			Controls.Add(splitContainer1);
			Controls.Add(statusStrip1);
			Controls.Add(toolStrip1);
			Controls.Add(menuStrip1);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MainMenuStrip = menuStrip1;
			Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			Name = "MainForm";
			Text = "SA2 Event Viewer";
			WindowState = System.Windows.Forms.FormWindowState.Maximized;
			Deactivate += MainForm_Deactivate;
			FormClosing += MainForm_FormClosing;
			Load += MainForm_Load;
			ResizeBegin += MainForm_ResizeBegin;
			ResizeEnd += MainForm_ResizeEnd;
			Resize += MainForm_Resize;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
			contextMenuStrip1.ResumeLayout(false);
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.UserControl RenderPanel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel cameraPosLabel;
		private System.Windows.Forms.ToolStripStatusLabel sceneNumLabel;
		private System.Windows.Forms.ToolStripStatusLabel animFrameLabel;
		private System.Windows.Forms.ToolStripStatusLabel camModeLabel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.ToolStripMenuItem showCameraToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel cameraFOVLabel;
		private System.Windows.Forms.ToolStripMenuItem exportSA2MDLToolStripMenuItem;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton buttonOpen;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton buttonSolid;
		private System.Windows.Forms.ToolStripButton buttonVertices;
		private System.Windows.Forms.ToolStripButton buttonWireframe;
		private System.Windows.Forms.ToolStripButton buttonPreferences;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton buttonLighting;
		private System.Windows.Forms.ToolStripButton buttonMaterialColors;
		private System.Windows.Forms.ToolStripButton buttonShowHints;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton buttonPrevScene;
		private System.Windows.Forms.ToolStripButton buttonNextScene;
		private System.Windows.Forms.ToolStripButton buttonPreviousFrame;
		private System.Windows.Forms.ToolStripButton buttonNextFrame;
		private System.Windows.Forms.ToolStripButton buttonPlayScene;
		private System.Windows.Forms.ToolStripMenuItem showHintsToolStripMenuItem;
		private System.Windows.Forms.Timer MessageTimer;
		private System.Windows.Forms.ToolStripMenuItem showAdvancedCameraInfoToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel cameraAngleLabel;
		private System.Windows.Forms.ToolStripMenuItem exportSA2BMDLToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel eventFrameLabel;
		private System.Windows.Forms.ToolStripButton buttonSave;
		private System.Windows.Forms.ToolStripButton buttonSaveAs;
		private System.Windows.Forms.ListView listViewEntities;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxEntityName;
	}
}

