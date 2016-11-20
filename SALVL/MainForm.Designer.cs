namespace SonicRetro.SAModel.SALVL
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
			this.editInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cStructsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.debugLightingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.levelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.visibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.invisibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.UserControl();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.importFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.selectModeButton = new System.Windows.Forms.ToolStripButton();
			this.moveModeButton = new System.Windows.Forms.ToolStripButton();
			this.rotateModeButton = new System.Windows.Forms.ToolStripButton();
			this.rotateMode = new System.Windows.Forms.ToolStripButton();
			this.gizmoSpaceComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.calculateAllBoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
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
            this.openToolStripMenuItem,
            this.editInfoToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// editInfoToolStripMenuItem
			// 
			this.editInfoToolStripMenuItem.Enabled = false;
			this.editInfoToolStripMenuItem.Name = "editInfoToolStripMenuItem";
			this.editInfoToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.editInfoToolStripMenuItem.Text = "E&dit Info";
			this.editInfoToolStripMenuItem.Click += new System.EventHandler(this.editInfoToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.Enabled = false;
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.importToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.importToolStripMenuItem.Text = "&Import";
			this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportOBJToolStripMenuItem,
            this.cStructsToolStripMenuItem});
			this.exportToolStripMenuItem.Enabled = false;
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.exportToolStripMenuItem.Text = "&Export";
			// 
			// exportOBJToolStripMenuItem
			// 
			this.exportOBJToolStripMenuItem.Name = "exportOBJToolStripMenuItem";
			this.exportOBJToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.exportOBJToolStripMenuItem.Text = "&OBJ";
			this.exportOBJToolStripMenuItem.Click += new System.EventHandler(this.exportOBJToolStripMenuItem_Click);
			// 
			// cStructsToolStripMenuItem
			// 
			this.cStructsToolStripMenuItem.Name = "cStructsToolStripMenuItem";
			this.cStructsToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.cStructsToolStripMenuItem.Text = "C &Structs";
			this.cStructsToolStripMenuItem.Click += new System.EventHandler(this.cStructsToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearLevelToolStripMenuItem,
            this.duplicateToolStripMenuItem,
            this.calculateAllBoundsToolStripMenuItem,
            this.debugLightingToolStripMenuItem,
            this.preferencesToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// clearLevelToolStripMenuItem
			// 
			this.clearLevelToolStripMenuItem.Enabled = false;
			this.clearLevelToolStripMenuItem.Name = "clearLevelToolStripMenuItem";
			this.clearLevelToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.clearLevelToolStripMenuItem.Text = "Clear Level";
			this.clearLevelToolStripMenuItem.Click += new System.EventHandler(this.clearLevelToolStripMenuItem_Click);
			// 
			// duplicateToolStripMenuItem
			// 
			this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
			this.duplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.duplicateToolStripMenuItem.Text = "&Duplicate";
			this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
			// 
			// debugLightingToolStripMenuItem
			// 
			this.debugLightingToolStripMenuItem.Enabled = false;
			this.debugLightingToolStripMenuItem.Name = "debugLightingToolStripMenuItem";
			this.debugLightingToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.debugLightingToolStripMenuItem.Text = "Debug Lighting";
			this.debugLightingToolStripMenuItem.Click += new System.EventHandler(this.debugLightingToolStripMenuItem_Click);
			// 
			// preferencesToolStripMenuItem
			// 
			this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.preferencesToolStripMenuItem.Text = "&Preferences";
			this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.levelToolStripMenuItem,
            this.statsToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// levelToolStripMenuItem
			// 
			this.levelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.visibleToolStripMenuItem,
            this.invisibleToolStripMenuItem,
            this.allToolStripMenuItem});
			this.levelToolStripMenuItem.Name = "levelToolStripMenuItem";
			this.levelToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
			this.levelToolStripMenuItem.Text = "&Level";
			this.levelToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.levelToolStripMenuItem_DropDownItemClicked);
			// 
			// visibleToolStripMenuItem
			// 
			this.visibleToolStripMenuItem.Checked = true;
			this.visibleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.visibleToolStripMenuItem.Name = "visibleToolStripMenuItem";
			this.visibleToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.visibleToolStripMenuItem.Text = "&Visible";
			// 
			// invisibleToolStripMenuItem
			// 
			this.invisibleToolStripMenuItem.Name = "invisibleToolStripMenuItem";
			this.invisibleToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.invisibleToolStripMenuItem.Text = "&Invisible";
			// 
			// allToolStripMenuItem
			// 
			this.allToolStripMenuItem.Name = "allToolStripMenuItem";
			this.allToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.allToolStripMenuItem.Text = "&All";
			// 
			// statsToolStripMenuItem
			// 
			this.statsToolStripMenuItem.Enabled = false;
			this.statsToolStripMenuItem.Name = "statsToolStripMenuItem";
			this.statsToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
			this.statsToolStripMenuItem.Text = "Stats";
			this.statsToolStripMenuItem.Click += new System.EventHandler(this.statsToolStripMenuItem_Click_1);
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(318, 512);
			this.panel1.TabIndex = 1;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			this.panel1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyDown);
			this.panel1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyUp);
			this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
			this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
			this.panel1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.panel1_PreviewKeyDown);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(108, 114);
			// 
			// addToolStripMenuItem
			// 
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.addToolStripMenuItem.Text = "&Add...";
			this.addToolStripMenuItem.Click += new System.EventHandler(this.levelPieceToolStripMenuItem_Click);
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.cutToolStripMenuItem.Text = "Cu&t";
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Enabled = false;
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.deleteToolStripMenuItem.Text = "&Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 52);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
			this.splitContainer1.Size = new System.Drawing.Size(584, 512);
			this.splitContainer1.SplitterDistance = 318;
			this.splitContainer1.TabIndex = 3;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Margin = new System.Windows.Forms.Padding(0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.propertyGrid1.Size = new System.Drawing.Size(262, 512);
			this.propertyGrid1.TabIndex = 13;
			this.propertyGrid1.ToolbarVisible = false;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			// 
			// importFileDialog
			// 
			this.importFileDialog.FileName = "file";
			this.importFileDialog.Filter = "*.OBJ Format|*.obj;*.objf|NodeTable|*.txt";
			this.importFileDialog.RestoreDirectory = true;
			this.importFileDialog.Title = "Select a file to import.";
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectModeButton,
            this.moveModeButton,
            this.rotateModeButton,
            this.rotateMode,
            this.gizmoSpaceComboBox});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(584, 25);
			this.toolStrip1.TabIndex = 4;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// selectModeButton
			// 
			this.selectModeButton.Checked = true;
			this.selectModeButton.CheckOnClick = true;
			this.selectModeButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.selectModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.selectModeButton.Image = ((System.Drawing.Image)(resources.GetObject("selectModeButton.Image")));
			this.selectModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.selectModeButton.Name = "selectModeButton";
			this.selectModeButton.Size = new System.Drawing.Size(23, 22);
			this.selectModeButton.Text = "toolStripButton1";
			this.selectModeButton.ToolTipText = "Select Mode";
			this.selectModeButton.Click += new System.EventHandler(this.selectModeButton_Click);
			// 
			// moveModeButton
			// 
			this.moveModeButton.CheckOnClick = true;
			this.moveModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.moveModeButton.Image = ((System.Drawing.Image)(resources.GetObject("moveModeButton.Image")));
			this.moveModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.moveModeButton.Name = "moveModeButton";
			this.moveModeButton.Size = new System.Drawing.Size(23, 22);
			this.moveModeButton.Text = "toolStripButton1";
			this.moveModeButton.ToolTipText = "Move Mode";
			this.moveModeButton.Click += new System.EventHandler(this.moveModeButton_Click);
			// 
			// rotateModeButton
			// 
			this.rotateModeButton.CheckOnClick = true;
			this.rotateModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.rotateModeButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateModeButton.Image")));
			this.rotateModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.rotateModeButton.Name = "rotateModeButton";
			this.rotateModeButton.Size = new System.Drawing.Size(23, 22);
			this.rotateModeButton.Text = "toolStripButton1";
			this.rotateModeButton.ToolTipText = "Rotate Mode";
			this.rotateModeButton.Click += new System.EventHandler(this.rotateModeButton_Click);
			// 
			// rotateMode
			// 
			this.rotateMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.rotateMode.Image = ((System.Drawing.Image)(resources.GetObject("rotateMode.Image")));
			this.rotateMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.rotateMode.Name = "rotateMode";
			this.rotateMode.Size = new System.Drawing.Size(23, 22);
			this.rotateMode.Text = "toolStripButton1";
			this.rotateMode.Click += new System.EventHandler(this.rotateMode_Click);
			// 
			// gizmoSpaceComboBox
			// 
			this.gizmoSpaceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.gizmoSpaceComboBox.Enabled = false;
			this.gizmoSpaceComboBox.Items.AddRange(new object[] {
            "Global",
            "Local"});
			this.gizmoSpaceComboBox.Name = "gizmoSpaceComboBox";
			this.gizmoSpaceComboBox.Size = new System.Drawing.Size(121, 25);
			this.gizmoSpaceComboBox.DropDownClosed += new System.EventHandler(this.gizmoSpaceComboBox_DropDownClosed);
			// 
			// calculateAllBoundsToolStripMenuItem
			// 
			this.calculateAllBoundsToolStripMenuItem.Enabled = false;
			this.calculateAllBoundsToolStripMenuItem.Name = "calculateAllBoundsToolStripMenuItem";
			this.calculateAllBoundsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.calculateAllBoundsToolStripMenuItem.Text = "Calculate All Bounds";
			this.calculateAllBoundsToolStripMenuItem.Click += new System.EventHandler(this.calculateAllBoundsToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 564);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "SALVL";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.UserControl panel1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem levelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invisibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editInfoToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        internal System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cStructsToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog importFileDialog;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugLightingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton selectModeButton;
        private System.Windows.Forms.ToolStripButton moveModeButton;
        private System.Windows.Forms.ToolStripButton rotateModeButton;
		private System.Windows.Forms.ToolStripComboBox gizmoSpaceComboBox;
		private System.Windows.Forms.ToolStripButton rotateMode;
		private System.Windows.Forms.ToolStripMenuItem calculateAllBoundsToolStripMenuItem;
	}
}

