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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAdvancedCameraInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenderPanel = new System.Windows.Forms.UserControl();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportSA2MDLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.camModeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cameraPosLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cameraAngleLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cameraFOVLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.sceneNumLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.animFrameLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Timers.Timer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonLighting = new System.Windows.Forms.ToolStripButton();
            this.buttonMaterialColors = new System.Windows.Forms.ToolStripButton();
            this.buttonShowHints = new System.Windows.Forms.ToolStripButton();
            this.buttonPreferences = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonSolid = new System.Windows.Forms.ToolStripButton();
            this.buttonVertices = new System.Windows.Forms.ToolStripButton();
            this.buttonWireframe = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonPrevScene = new System.Windows.Forms.ToolStripButton();
            this.buttonNextScene = new System.Windows.Forms.ToolStripButton();
            this.buttonPreviousFrame = new System.Windows.Forms.ToolStripButton();
            this.buttonPlayScene = new System.Windows.Forms.ToolStripButton();
            this.buttonNextFrame = new System.Windows.Forms.ToolStripButton();
            this.exportSA2BMDLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
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
            this.openToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::SA2EventViewer.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Enabled = false;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exportToolStripMenuItem.Text = "&Export";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Checked = true;
            this.editToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 22);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Image = global::SA2EventViewer.Properties.Resources.settings;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showCameraToolStripMenuItem,
            this.showHintsToolStripMenuItem,
            this.showAdvancedCameraInfoToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // showCameraToolStripMenuItem
            // 
            this.showCameraToolStripMenuItem.Checked = true;
            this.showCameraToolStripMenuItem.CheckOnClick = true;
            this.showCameraToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showCameraToolStripMenuItem.Name = "showCameraToolStripMenuItem";
            this.showCameraToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.showCameraToolStripMenuItem.Text = "Show &Camera";
            this.showCameraToolStripMenuItem.Click += new System.EventHandler(this.showCameraToolStripMenuItem_Click);
            // 
            // showHintsToolStripMenuItem
            // 
            this.showHintsToolStripMenuItem.Checked = true;
            this.showHintsToolStripMenuItem.CheckOnClick = true;
            this.showHintsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showHintsToolStripMenuItem.Image = global::SA2EventViewer.Properties.Resources.messages;
            this.showHintsToolStripMenuItem.Name = "showHintsToolStripMenuItem";
            this.showHintsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.showHintsToolStripMenuItem.Text = "Show Hints";
            this.showHintsToolStripMenuItem.ToolTipText = "Show Hints";
            this.showHintsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showHintsToolStripMenuItem_CheckedChanged);
            // 
            // showAdvancedCameraInfoToolStripMenuItem
            // 
            this.showAdvancedCameraInfoToolStripMenuItem.CheckOnClick = true;
            this.showAdvancedCameraInfoToolStripMenuItem.Name = "showAdvancedCameraInfoToolStripMenuItem";
            this.showAdvancedCameraInfoToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.showAdvancedCameraInfoToolStripMenuItem.Text = "Show Camera Rotation";
            this.showAdvancedCameraInfoToolStripMenuItem.ToolTipText = "Show more details on camera mode, rotation etc.";
            // 
            // RenderPanel
            // 
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 0);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(685, 521);
            this.RenderPanel.TabIndex = 1;
            this.RenderPanel.SizeChanged += new System.EventHandler(this.RenderPanel_SizeChanged);
            this.RenderPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.RenderPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyDown);
            this.RenderPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyUp);
            this.RenderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.RenderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
            this.RenderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
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
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(949, 521);
            this.splitContainer1.SplitterDistance = 685;
            this.splitContainer1.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.propertyGrid1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDown1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(260, 521);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGrid1, 2);
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDarkDark;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 26);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid1.Size = new System.Drawing.Size(260, 475);
            this.propertyGrid1.TabIndex = 14;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Timescale:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numericUpDown1.DecimalPlaces = 2;
            this.numericUpDown1.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.numericUpDown1.Location = new System.Drawing.Point(67, 3);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(78, 20);
            this.numericUpDown1.TabIndex = 16;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportSA2MDLToolStripMenuItem,
            this.exportSA2BMDLToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 70);
            // 
            // exportSA2MDLToolStripMenuItem
            // 
            this.exportSA2MDLToolStripMenuItem.Enabled = false;
            this.exportSA2MDLToolStripMenuItem.Name = "exportSA2MDLToolStripMenuItem";
            this.exportSA2MDLToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportSA2MDLToolStripMenuItem.Text = "Export &SA2MDL";
            this.exportSA2MDLToolStripMenuItem.Click += new System.EventHandler(this.exportSA2MDLToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.camModeLabel,
            this.cameraPosLabel,
            this.cameraAngleLabel,
            this.cameraFOVLabel,
            this.sceneNumLabel,
            this.animFrameLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 588);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(949, 24);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // camModeLabel
            // 
            this.camModeLabel.Name = "camModeLabel";
            this.camModeLabel.Size = new System.Drawing.Size(64, 19);
            this.camModeLabel.Text = "Event Cam";
            // 
            // cameraPosLabel
            // 
            this.cameraPosLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.cameraPosLabel.Name = "cameraPosLabel";
            this.cameraPosLabel.Size = new System.Drawing.Size(110, 19);
            this.cameraPosLabel.Text = "Camera Pos: 0, 0, 0";
            // 
            // cameraAngleLabel
            // 
            this.cameraAngleLabel.Name = "cameraAngleLabel";
            this.cameraAngleLabel.Size = new System.Drawing.Size(0, 19);
            // 
            // cameraFOVLabel
            // 
            this.cameraFOVLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.cameraFOVLabel.Name = "cameraFOVLabel";
            this.cameraFOVLabel.Size = new System.Drawing.Size(45, 19);
            this.cameraFOVLabel.Text = "FOV: 0";
            // 
            // sceneNumLabel
            // 
            this.sceneNumLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.sceneNumLabel.Name = "sceneNumLabel";
            this.sceneNumLabel.Size = new System.Drawing.Size(54, 19);
            this.sceneNumLabel.Text = "Scene: 0";
            // 
            // animFrameLabel
            // 
            this.animFrameLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.animFrameLabel.Name = "animFrameLabel";
            this.animFrameLabel.Size = new System.Drawing.Size(61, 19);
            this.animFrameLabel.Text = "Frame: -1";
            // 
            // timer1
            // 
            this.timer1.Interval = 16.666666666666668D;
            this.timer1.SynchronizingObject = this;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonOpen,
            this.toolStripSeparator1,
            this.buttonLighting,
            this.buttonMaterialColors,
            this.buttonShowHints,
            this.buttonPreferences,
            this.toolStripSeparator2,
            this.buttonSolid,
            this.buttonVertices,
            this.buttonWireframe,
            this.toolStripSeparator3,
            this.buttonPrevScene,
            this.buttonNextScene,
            this.buttonPreviousFrame,
            this.buttonPlayScene,
            this.buttonNextFrame});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(949, 43);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonOpen
            // 
            this.buttonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOpen.Image = global::SA2EventViewer.Properties.Resources.open;
            this.buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(40, 40);
            this.buttonOpen.Text = "Open";
            this.buttonOpen.ToolTipText = "Open File";
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 43);
            // 
            // buttonLighting
            // 
            this.buttonLighting.Checked = true;
            this.buttonLighting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonLighting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonLighting.Image = global::SA2EventViewer.Properties.Resources.lighting;
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
            this.buttonMaterialColors.Image = global::SA2EventViewer.Properties.Resources.material;
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
            this.buttonShowHints.Image = global::SA2EventViewer.Properties.Resources.messages;
            this.buttonShowHints.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonShowHints.Name = "buttonShowHints";
            this.buttonShowHints.Size = new System.Drawing.Size(40, 40);
            this.buttonShowHints.Text = "Show Hints";
            this.buttonShowHints.Click += new System.EventHandler(this.buttonShowHints_Click);
            // 
            // buttonPreferences
            // 
            this.buttonPreferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPreferences.Image = global::SA2EventViewer.Properties.Resources.settings;
            this.buttonPreferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPreferences.Name = "buttonPreferences";
            this.buttonPreferences.Size = new System.Drawing.Size(40, 40);
            this.buttonPreferences.Text = "Preferences";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 43);
            // 
            // buttonSolid
            // 
            this.buttonSolid.Checked = true;
            this.buttonSolid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonSolid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSolid.Image = global::SA2EventViewer.Properties.Resources.solid;
            this.buttonSolid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSolid.Name = "buttonSolid";
            this.buttonSolid.Size = new System.Drawing.Size(40, 40);
            this.buttonSolid.Text = "Render Mode: Solid";
            this.buttonSolid.Click += new System.EventHandler(this.buttonSolid_Click);
            // 
            // buttonVertices
            // 
            this.buttonVertices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonVertices.Image = global::SA2EventViewer.Properties.Resources.point;
            this.buttonVertices.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonVertices.Name = "buttonVertices";
            this.buttonVertices.Size = new System.Drawing.Size(40, 40);
            this.buttonVertices.Text = "Render Mode: Points";
            this.buttonVertices.Click += new System.EventHandler(this.buttonVertices_Click);
            // 
            // buttonWireframe
            // 
            this.buttonWireframe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonWireframe.Image = global::SA2EventViewer.Properties.Resources.wireframe;
            this.buttonWireframe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonWireframe.Name = "buttonWireframe";
            this.buttonWireframe.Size = new System.Drawing.Size(40, 40);
            this.buttonWireframe.Text = "Render Mode: Wireframe";
            this.buttonWireframe.Click += new System.EventHandler(this.buttonWireframe_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 43);
            // 
            // buttonPrevScene
            // 
            this.buttonPrevScene.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPrevScene.Enabled = false;
            this.buttonPrevScene.Image = global::SA2EventViewer.Properties.Resources.prevanim;
            this.buttonPrevScene.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPrevScene.Name = "buttonPrevScene";
            this.buttonPrevScene.Size = new System.Drawing.Size(40, 40);
            this.buttonPrevScene.Text = "Previous Scene";
            this.buttonPrevScene.Click += new System.EventHandler(this.buttonPrevScene_Click);
            // 
            // buttonNextScene
            // 
            this.buttonNextScene.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNextScene.Enabled = false;
            this.buttonNextScene.Image = global::SA2EventViewer.Properties.Resources.nextanim;
            this.buttonNextScene.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNextScene.Name = "buttonNextScene";
            this.buttonNextScene.Size = new System.Drawing.Size(40, 40);
            this.buttonNextScene.Text = "Next Scene";
            this.buttonNextScene.Click += new System.EventHandler(this.buttonNextScene_Click);
            // 
            // buttonPreviousFrame
            // 
            this.buttonPreviousFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPreviousFrame.Enabled = false;
            this.buttonPreviousFrame.Image = global::SA2EventViewer.Properties.Resources.prevframe;
            this.buttonPreviousFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPreviousFrame.Name = "buttonPreviousFrame";
            this.buttonPreviousFrame.Size = new System.Drawing.Size(40, 40);
            this.buttonPreviousFrame.Text = "Previous Frame";
            this.buttonPreviousFrame.Click += new System.EventHandler(this.buttonPrevFrame_Click);
            // 
            // buttonPlayScene
            // 
            this.buttonPlayScene.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPlayScene.Enabled = false;
            this.buttonPlayScene.Image = global::SA2EventViewer.Properties.Resources.playanim;
            this.buttonPlayScene.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPlayScene.Name = "buttonPlayScene";
            this.buttonPlayScene.Size = new System.Drawing.Size(40, 40);
            this.buttonPlayScene.Text = "Play/Stop Animation";
            this.buttonPlayScene.Click += new System.EventHandler(this.buttonPlayScene_Click);
            // 
            // buttonNextFrame
            // 
            this.buttonNextFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNextFrame.Enabled = false;
            this.buttonNextFrame.Image = global::SA2EventViewer.Properties.Resources.nextframe;
            this.buttonNextFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNextFrame.Name = "buttonNextFrame";
            this.buttonNextFrame.Size = new System.Drawing.Size(40, 40);
            this.buttonNextFrame.Text = "Next Frame";
            this.buttonNextFrame.Click += new System.EventHandler(this.buttonNextFrame_Click);
            // 
            // exportSA2BMDLToolStripMenuItem
            // 
            this.exportSA2BMDLToolStripMenuItem.Enabled = false;
            this.exportSA2BMDLToolStripMenuItem.Name = "exportSA2BMDLToolStripMenuItem";
            this.exportSA2BMDLToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportSA2BMDLToolStripMenuItem.Text = "Export SA2BMDL";
			this.exportSA2BMDLToolStripMenuItem.Click += new System.EventHandler(this.exportSA2BMDLToolStripMenuItem_Click);

			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 612);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "SA2 Event Viewer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
		private System.Timers.Timer timer1;
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
		private System.Windows.Forms.ToolStripMenuItem showAdvancedCameraInfoToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel cameraAngleLabel;
		private System.Windows.Forms.ToolStripMenuItem exportSA2BMDLToolStripMenuItem;
	}
}

