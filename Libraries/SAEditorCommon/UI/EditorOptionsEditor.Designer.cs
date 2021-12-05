namespace SAModel.SAEditorCommon.UI
{
    partial class EditorOptionsEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorOptionsEditor));
            this.groupBoxRenderOptions = new System.Windows.Forms.GroupBox();
            this.pictureBoxBackgroundColor = new System.Windows.Forms.PictureBox();
            this.labelBackgroundColor = new System.Windows.Forms.Label();
            this.checkBoxIgnoreMaterialColors = new System.Windows.Forms.CheckBox();
            this.checkboxDisableLighting = new System.Windows.Forms.CheckBox();
            this.comboBoxBackfaceCulling = new System.Windows.Forms.ComboBox();
            this.labelBackfaceCulling = new System.Windows.Forms.Label();
            this.labelPolygonFillMode = new System.Windows.Forms.Label();
            this.comboBoxFillMode = new System.Windows.Forms.ComboBox();
            this.labelDrawDistanceGeneral = new System.Windows.Forms.Label();
            this.trackBarDrawDistanceGeneral = new System.Windows.Forms.TrackBar();
            this.buttonDone = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.KeyboardShortcutButton = new System.Windows.Forms.Button();
            this.buttonResetDefaultKeys = new System.Windows.Forms.Button();
            this.groupBoxDrawDistance = new System.Windows.Forms.GroupBox();
            this.trackBarDrawDistanceSet = new System.Windows.Forms.TrackBar();
            this.labelDrawDistanceSetCam = new System.Windows.Forms.Label();
            this.trackBarDrawDistanceLevel = new System.Windows.Forms.TrackBar();
            this.labelDrawDistanceLevel = new System.Windows.Forms.Label();
            this.tabControlOptions = new System.Windows.Forms.TabControl();
            this.tabPageDisplay = new System.Windows.Forms.TabPage();
            this.tabPageControl = new System.Windows.Forms.TabPage();
            this.radioButtonZoom = new System.Windows.Forms.RadioButton();
            this.groupBoxActions = new System.Windows.Forms.GroupBox();
            this.listBoxActions = new System.Windows.Forms.ListBox();
            this.groupBoxKeys = new System.Windows.Forms.GroupBox();
            this.groupBoxDescription = new System.Windows.Forms.GroupBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.buttonResetSelectedKey = new System.Windows.Forms.Button();
            this.buttonClearSelectedKey = new System.Windows.Forms.Button();
            this.textBoxModifier = new System.Windows.Forms.TextBox();
            this.textBoxAltKey = new System.Windows.Forms.TextBox();
            this.textBoxMainKey = new System.Windows.Forms.TextBox();
            this.labelMainKey = new System.Windows.Forms.Label();
            this.labelModifier = new System.Windows.Forms.Label();
            this.labelAltKey = new System.Windows.Forms.Label();
            this.radioButtonLook = new System.Windows.Forms.RadioButton();
            this.radioButtonMove = new System.Windows.Forms.RadioButton();
            this.labelCameraModifier = new System.Windows.Forms.Label();
            this.groupBoxRenderOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackgroundColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDrawDistanceGeneral)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBoxDrawDistance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDrawDistanceSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDrawDistanceLevel)).BeginInit();
            this.tabControlOptions.SuspendLayout();
            this.tabPageDisplay.SuspendLayout();
            this.tabPageControl.SuspendLayout();
            this.groupBoxActions.SuspendLayout();
            this.groupBoxKeys.SuspendLayout();
            this.groupBoxDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxRenderOptions
            // 
            this.groupBoxRenderOptions.Controls.Add(this.pictureBoxBackgroundColor);
            this.groupBoxRenderOptions.Controls.Add(this.labelBackgroundColor);
            this.groupBoxRenderOptions.Controls.Add(this.checkBoxIgnoreMaterialColors);
            this.groupBoxRenderOptions.Controls.Add(this.checkboxDisableLighting);
            this.groupBoxRenderOptions.Controls.Add(this.comboBoxBackfaceCulling);
            this.groupBoxRenderOptions.Controls.Add(this.labelBackfaceCulling);
            this.groupBoxRenderOptions.Controls.Add(this.labelPolygonFillMode);
            this.groupBoxRenderOptions.Controls.Add(this.comboBoxFillMode);
            this.groupBoxRenderOptions.Location = new System.Drawing.Point(7, 7);
            this.groupBoxRenderOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxRenderOptions.Name = "groupBoxRenderOptions";
            this.groupBoxRenderOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxRenderOptions.Size = new System.Drawing.Size(298, 253);
            this.groupBoxRenderOptions.TabIndex = 0;
            this.groupBoxRenderOptions.TabStop = false;
            this.groupBoxRenderOptions.Text = "Render Options";
            // 
            // pictureBoxBackgroundColor
            // 
            this.pictureBoxBackgroundColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxBackgroundColor.Location = new System.Drawing.Point(127, 201);
            this.pictureBoxBackgroundColor.Name = "pictureBoxBackgroundColor";
            this.pictureBoxBackgroundColor.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxBackgroundColor.TabIndex = 8;
            this.pictureBoxBackgroundColor.TabStop = false;
            this.pictureBoxBackgroundColor.Click += new System.EventHandler(this.pictureBoxBackgroundColor_Click);
            // 
            // labelBackgroundColor
            // 
            this.labelBackgroundColor.AutoSize = true;
            this.labelBackgroundColor.Location = new System.Drawing.Point(14, 211);
            this.labelBackgroundColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBackgroundColor.Name = "labelBackgroundColor";
            this.labelBackgroundColor.Size = new System.Drawing.Size(106, 15);
            this.labelBackgroundColor.TabIndex = 7;
            this.labelBackgroundColor.Text = "Background Color:";
            // 
            // checkBoxIgnoreMaterialColors
            // 
            this.checkBoxIgnoreMaterialColors.AutoSize = true;
            this.checkBoxIgnoreMaterialColors.Location = new System.Drawing.Point(18, 165);
            this.checkBoxIgnoreMaterialColors.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxIgnoreMaterialColors.Name = "checkBoxIgnoreMaterialColors";
            this.checkBoxIgnoreMaterialColors.Size = new System.Drawing.Size(141, 19);
            this.checkBoxIgnoreMaterialColors.TabIndex = 6;
            this.checkBoxIgnoreMaterialColors.Text = "Ignore material colors";
            this.checkBoxIgnoreMaterialColors.UseVisualStyleBackColor = true;
            this.checkBoxIgnoreMaterialColors.Click += new System.EventHandler(this.ignoreMaterialColorsCheck_Click);
            // 
            // checkboxDisableLighting
            // 
            this.checkboxDisableLighting.AutoSize = true;
            this.checkboxDisableLighting.Location = new System.Drawing.Point(18, 138);
            this.checkboxDisableLighting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkboxDisableLighting.Name = "checkboxDisableLighting";
            this.checkboxDisableLighting.Size = new System.Drawing.Size(108, 19);
            this.checkboxDisableLighting.TabIndex = 5;
            this.checkboxDisableLighting.Text = "Disable lighting";
            this.checkboxDisableLighting.UseVisualStyleBackColor = true;
            this.checkboxDisableLighting.Click += new System.EventHandler(this.fullBrightCheck_Click);
            // 
            // comboBoxBackfaceCulling
            // 
            this.comboBoxBackfaceCulling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBackfaceCulling.FormattingEnabled = true;
            this.comboBoxBackfaceCulling.Items.AddRange(new object[] {
            "None (Draws both sides)",
            "Clockwise",
            "Counter-Clockwise"});
            this.comboBoxBackfaceCulling.Location = new System.Drawing.Point(18, 104);
            this.comboBoxBackfaceCulling.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBoxBackfaceCulling.Name = "comboBoxBackfaceCulling";
            this.comboBoxBackfaceCulling.Size = new System.Drawing.Size(228, 23);
            this.comboBoxBackfaceCulling.TabIndex = 4;
            this.comboBoxBackfaceCulling.SelectionChangeCommitted += new System.EventHandler(this.cullModeDropdown_SelectionChangeCommitted);
            // 
            // labelBackfaceCulling
            // 
            this.labelBackfaceCulling.AutoSize = true;
            this.labelBackfaceCulling.Location = new System.Drawing.Point(14, 85);
            this.labelBackfaceCulling.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBackfaceCulling.Name = "labelBackfaceCulling";
            this.labelBackfaceCulling.Size = new System.Drawing.Size(98, 15);
            this.labelBackfaceCulling.TabIndex = 2;
            this.labelBackfaceCulling.Text = "Backface Culling:";
            // 
            // labelPolygonFillMode
            // 
            this.labelPolygonFillMode.AutoSize = true;
            this.labelPolygonFillMode.Location = new System.Drawing.Point(14, 30);
            this.labelPolygonFillMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPolygonFillMode.Name = "labelPolygonFillMode";
            this.labelPolygonFillMode.Size = new System.Drawing.Size(106, 15);
            this.labelPolygonFillMode.TabIndex = 3;
            this.labelPolygonFillMode.Text = "Polygon Fill Mode:";
            // 
            // comboBoxFillMode
            // 
            this.comboBoxFillMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFillMode.FormattingEnabled = true;
            this.comboBoxFillMode.Items.AddRange(new object[] {
            "Vertex-Only",
            "Wireframe",
            "Solid"});
            this.comboBoxFillMode.Location = new System.Drawing.Point(18, 48);
            this.comboBoxFillMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBoxFillMode.Name = "comboBoxFillMode";
            this.comboBoxFillMode.Size = new System.Drawing.Size(228, 23);
            this.comboBoxFillMode.TabIndex = 2;
            this.comboBoxFillMode.SelectionChangeCommitted += new System.EventHandler(this.fillModeDropDown_SelectionChangeCommitted);
            // 
            // labelDrawDistanceGeneral
            // 
            this.labelDrawDistanceGeneral.AutoSize = true;
            this.labelDrawDistanceGeneral.Location = new System.Drawing.Point(18, 29);
            this.labelDrawDistanceGeneral.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDrawDistanceGeneral.Name = "labelDrawDistanceGeneral";
            this.labelDrawDistanceGeneral.Size = new System.Drawing.Size(50, 15);
            this.labelDrawDistanceGeneral.TabIndex = 1;
            this.labelDrawDistanceGeneral.Text = "General:";
            // 
            // trackBarDrawDistanceGeneral
            // 
            this.trackBarDrawDistanceGeneral.LargeChange = 1000;
            this.trackBarDrawDistanceGeneral.Location = new System.Drawing.Point(7, 47);
            this.trackBarDrawDistanceGeneral.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackBarDrawDistanceGeneral.Maximum = 60000;
            this.trackBarDrawDistanceGeneral.Minimum = 1000;
            this.trackBarDrawDistanceGeneral.Name = "trackBarDrawDistanceGeneral";
            this.trackBarDrawDistanceGeneral.Size = new System.Drawing.Size(265, 45);
            this.trackBarDrawDistanceGeneral.SmallChange = 100;
            this.trackBarDrawDistanceGeneral.TabIndex = 0;
            this.trackBarDrawDistanceGeneral.TickFrequency = 2000;
            this.trackBarDrawDistanceGeneral.Value = 6000;
            this.trackBarDrawDistanceGeneral.Scroll += new System.EventHandler(this.drawDistSlider_Scroll);
            // 
            // buttonDone
            // 
            this.buttonDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDone.Location = new System.Drawing.Point(506, 333);
            this.buttonDone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonDone.Name = "buttonDone";
            this.buttonDone.Size = new System.Drawing.Size(88, 27);
            this.buttonDone.TabIndex = 1;
            this.buttonDone.Text = "Done";
            this.buttonDone.UseVisualStyleBackColor = true;
            this.buttonDone.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.KeyboardShortcutButton);
            this.groupBox2.Location = new System.Drawing.Point(692, 153);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(298, 103);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input Options";
            // 
            // KeyboardShortcutButton
            // 
            this.KeyboardShortcutButton.Location = new System.Drawing.Point(7, 24);
            this.KeyboardShortcutButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.KeyboardShortcutButton.Name = "KeyboardShortcutButton";
            this.KeyboardShortcutButton.Size = new System.Drawing.Size(284, 27);
            this.KeyboardShortcutButton.TabIndex = 0;
            this.KeyboardShortcutButton.Text = "Configure Keyboard Shortcuts";
            this.KeyboardShortcutButton.UseVisualStyleBackColor = true;
            this.KeyboardShortcutButton.Click += new System.EventHandler(this.KeyboardShortcutButton_Click);
            // 
            // buttonResetDefaultKeys
            // 
            this.buttonResetDefaultKeys.Location = new System.Drawing.Point(7, 242);
            this.buttonResetDefaultKeys.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonResetDefaultKeys.Name = "buttonResetDefaultKeys";
            this.buttonResetDefaultKeys.Size = new System.Drawing.Size(234, 27);
            this.buttonResetDefaultKeys.TabIndex = 1;
            this.buttonResetDefaultKeys.Text = "Restore Default Keys";
            this.buttonResetDefaultKeys.UseVisualStyleBackColor = true;
            this.buttonResetDefaultKeys.Click += new System.EventHandler(this.ResetDefaultKeybindButton_Click);
            // 
            // groupBoxDrawDistance
            // 
            this.groupBoxDrawDistance.Controls.Add(this.trackBarDrawDistanceSet);
            this.groupBoxDrawDistance.Controls.Add(this.labelDrawDistanceSetCam);
            this.groupBoxDrawDistance.Controls.Add(this.trackBarDrawDistanceLevel);
            this.groupBoxDrawDistance.Controls.Add(this.labelDrawDistanceLevel);
            this.groupBoxDrawDistance.Controls.Add(this.trackBarDrawDistanceGeneral);
            this.groupBoxDrawDistance.Controls.Add(this.labelDrawDistanceGeneral);
            this.groupBoxDrawDistance.Location = new System.Drawing.Point(312, 7);
            this.groupBoxDrawDistance.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxDrawDistance.Name = "groupBoxDrawDistance";
            this.groupBoxDrawDistance.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxDrawDistance.Size = new System.Drawing.Size(284, 253);
            this.groupBoxDrawDistance.TabIndex = 3;
            this.groupBoxDrawDistance.TabStop = false;
            this.groupBoxDrawDistance.Text = "Draw Distance";
            // 
            // trackBarDrawDistanceSet
            // 
            this.trackBarDrawDistanceSet.Enabled = false;
            this.trackBarDrawDistanceSet.LargeChange = 1000;
            this.trackBarDrawDistanceSet.Location = new System.Drawing.Point(7, 195);
            this.trackBarDrawDistanceSet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackBarDrawDistanceSet.Maximum = 60000;
            this.trackBarDrawDistanceSet.Minimum = 1000;
            this.trackBarDrawDistanceSet.Name = "trackBarDrawDistanceSet";
            this.trackBarDrawDistanceSet.Size = new System.Drawing.Size(265, 45);
            this.trackBarDrawDistanceSet.SmallChange = 100;
            this.trackBarDrawDistanceSet.TabIndex = 5;
            this.trackBarDrawDistanceSet.TickFrequency = 2000;
            this.trackBarDrawDistanceSet.Value = 6000;
            this.trackBarDrawDistanceSet.Scroll += new System.EventHandler(this.setDrawDistSlider_Scroll);
            // 
            // labelDrawDistanceSetCam
            // 
            this.labelDrawDistanceSetCam.AutoSize = true;
            this.labelDrawDistanceSetCam.Enabled = false;
            this.labelDrawDistanceSetCam.Location = new System.Drawing.Point(18, 177);
            this.labelDrawDistanceSetCam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDrawDistanceSetCam.Name = "labelDrawDistanceSetCam";
            this.labelDrawDistanceSetCam.Size = new System.Drawing.Size(92, 15);
            this.labelDrawDistanceSetCam.TabIndex = 4;
            this.labelDrawDistanceSetCam.Text = "SET/CAM Items:";
            // 
            // trackBarDrawDistanceLevel
            // 
            this.trackBarDrawDistanceLevel.Enabled = false;
            this.trackBarDrawDistanceLevel.LargeChange = 1000;
            this.trackBarDrawDistanceLevel.Location = new System.Drawing.Point(7, 121);
            this.trackBarDrawDistanceLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackBarDrawDistanceLevel.Maximum = 60000;
            this.trackBarDrawDistanceLevel.Minimum = 1000;
            this.trackBarDrawDistanceLevel.Name = "trackBarDrawDistanceLevel";
            this.trackBarDrawDistanceLevel.Size = new System.Drawing.Size(265, 45);
            this.trackBarDrawDistanceLevel.SmallChange = 100;
            this.trackBarDrawDistanceLevel.TabIndex = 3;
            this.trackBarDrawDistanceLevel.TickFrequency = 2000;
            this.trackBarDrawDistanceLevel.Value = 6000;
            this.trackBarDrawDistanceLevel.Scroll += new System.EventHandler(this.levelDrawDistSlider_Scroll);
            // 
            // labelDrawDistanceLevel
            // 
            this.labelDrawDistanceLevel.AutoSize = true;
            this.labelDrawDistanceLevel.Enabled = false;
            this.labelDrawDistanceLevel.Location = new System.Drawing.Point(18, 103);
            this.labelDrawDistanceLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDrawDistanceLevel.Name = "labelDrawDistanceLevel";
            this.labelDrawDistanceLevel.Size = new System.Drawing.Size(92, 15);
            this.labelDrawDistanceLevel.TabIndex = 2;
            this.labelDrawDistanceLevel.Text = "Level Geometry:";
            // 
            // tabControlOptions
            // 
            this.tabControlOptions.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControlOptions.Controls.Add(this.tabPageDisplay);
            this.tabControlOptions.Controls.Add(this.tabPageControl);
            this.tabControlOptions.Location = new System.Drawing.Point(0, 0);
            this.tabControlOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControlOptions.Name = "tabControlOptions";
            this.tabControlOptions.SelectedIndex = 0;
            this.tabControlOptions.Size = new System.Drawing.Size(611, 331);
            this.tabControlOptions.TabIndex = 7;
            // 
            // tabPageDisplay
            // 
            this.tabPageDisplay.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageDisplay.Controls.Add(this.groupBoxRenderOptions);
            this.tabPageDisplay.Controls.Add(this.groupBoxDrawDistance);
            this.tabPageDisplay.Location = new System.Drawing.Point(4, 27);
            this.tabPageDisplay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPageDisplay.Name = "tabPageDisplay";
            this.tabPageDisplay.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPageDisplay.Size = new System.Drawing.Size(603, 300);
            this.tabPageDisplay.TabIndex = 0;
            this.tabPageDisplay.Text = "Display Options";
            // 
            // tabPageControl
            // 
            this.tabPageControl.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageControl.Controls.Add(this.radioButtonZoom);
            this.tabPageControl.Controls.Add(this.groupBoxActions);
            this.tabPageControl.Controls.Add(this.groupBoxKeys);
            this.tabPageControl.Controls.Add(this.radioButtonLook);
            this.tabPageControl.Controls.Add(this.radioButtonMove);
            this.tabPageControl.Controls.Add(this.labelCameraModifier);
            this.tabPageControl.Location = new System.Drawing.Point(4, 27);
            this.tabPageControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPageControl.Name = "tabPageControl";
            this.tabPageControl.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPageControl.Size = new System.Drawing.Size(603, 300);
            this.tabPageControl.TabIndex = 1;
            this.tabPageControl.Text = "Control Options";
            // 
            // radioButtonZoom
            // 
            this.radioButtonZoom.AutoSize = true;
            this.radioButtonZoom.Location = new System.Drawing.Point(520, 270);
            this.radioButtonZoom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButtonZoom.Name = "radioButtonZoom";
            this.radioButtonZoom.Size = new System.Drawing.Size(57, 19);
            this.radioButtonZoom.TabIndex = 17;
            this.radioButtonZoom.Text = "Zoom";
            this.radioButtonZoom.UseVisualStyleBackColor = true;
            this.radioButtonZoom.Click += new System.EventHandler(this.radioButtonZoom_Click);
            // 
            // groupBoxActions
            // 
            this.groupBoxActions.Controls.Add(this.listBoxActions);
            this.groupBoxActions.Controls.Add(this.buttonResetDefaultKeys);
            this.groupBoxActions.Location = new System.Drawing.Point(7, 7);
            this.groupBoxActions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxActions.Name = "groupBoxActions";
            this.groupBoxActions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxActions.Size = new System.Drawing.Size(253, 279);
            this.groupBoxActions.TabIndex = 8;
            this.groupBoxActions.TabStop = false;
            this.groupBoxActions.Text = "Actions";
            // 
            // listBoxActions
            // 
            this.listBoxActions.FormattingEnabled = true;
            this.listBoxActions.ItemHeight = 15;
            this.listBoxActions.Location = new System.Drawing.Point(7, 22);
            this.listBoxActions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listBoxActions.Name = "listBoxActions";
            this.listBoxActions.Size = new System.Drawing.Size(234, 214);
            this.listBoxActions.TabIndex = 2;
            this.listBoxActions.SelectedIndexChanged += new System.EventHandler(this.listBoxActions_SelectedIndexChanged);
            // 
            // groupBoxKeys
            // 
            this.groupBoxKeys.Controls.Add(this.groupBoxDescription);
            this.groupBoxKeys.Controls.Add(this.buttonResetSelectedKey);
            this.groupBoxKeys.Controls.Add(this.buttonClearSelectedKey);
            this.groupBoxKeys.Controls.Add(this.textBoxModifier);
            this.groupBoxKeys.Controls.Add(this.textBoxAltKey);
            this.groupBoxKeys.Controls.Add(this.textBoxMainKey);
            this.groupBoxKeys.Controls.Add(this.labelMainKey);
            this.groupBoxKeys.Controls.Add(this.labelModifier);
            this.groupBoxKeys.Controls.Add(this.labelAltKey);
            this.groupBoxKeys.Enabled = false;
            this.groupBoxKeys.Location = new System.Drawing.Point(267, 7);
            this.groupBoxKeys.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxKeys.Name = "groupBoxKeys";
            this.groupBoxKeys.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxKeys.Size = new System.Drawing.Size(328, 258);
            this.groupBoxKeys.TabIndex = 10;
            this.groupBoxKeys.TabStop = false;
            this.groupBoxKeys.Text = "Keys";
            // 
            // groupBoxDescription
            // 
            this.groupBoxDescription.Controls.Add(this.labelDescription);
            this.groupBoxDescription.Location = new System.Drawing.Point(10, 157);
            this.groupBoxDescription.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxDescription.Name = "groupBoxDescription";
            this.groupBoxDescription.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxDescription.Size = new System.Drawing.Size(310, 89);
            this.groupBoxDescription.TabIndex = 13;
            this.groupBoxDescription.TabStop = false;
            this.groupBoxDescription.Text = "Description";
            // 
            // labelDescription
            // 
            this.labelDescription.Location = new System.Drawing.Point(7, 18);
            this.labelDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(289, 61);
            this.labelDescription.TabIndex = 5;
            // 
            // buttonResetSelectedKey
            // 
            this.buttonResetSelectedKey.Location = new System.Drawing.Point(175, 123);
            this.buttonResetSelectedKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonResetSelectedKey.Name = "buttonResetSelectedKey";
            this.buttonResetSelectedKey.Size = new System.Drawing.Size(146, 27);
            this.buttonResetSelectedKey.TabIndex = 12;
            this.buttonResetSelectedKey.Text = "Reset Selected Key";
            this.buttonResetSelectedKey.UseVisualStyleBackColor = true;
            this.buttonResetSelectedKey.Click += new System.EventHandler(this.buttonResetSelectedKey_Click);
            // 
            // buttonClearSelectedKey
            // 
            this.buttonClearSelectedKey.Location = new System.Drawing.Point(10, 123);
            this.buttonClearSelectedKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonClearSelectedKey.Name = "buttonClearSelectedKey";
            this.buttonClearSelectedKey.Size = new System.Drawing.Size(146, 27);
            this.buttonClearSelectedKey.TabIndex = 11;
            this.buttonClearSelectedKey.Text = "Clear Selected Key";
            this.buttonClearSelectedKey.UseVisualStyleBackColor = true;
            this.buttonClearSelectedKey.Click += new System.EventHandler(this.buttonClearSelectedKey_Click);
            // 
            // textBoxModifier
            // 
            this.textBoxModifier.Location = new System.Drawing.Point(105, 91);
            this.textBoxModifier.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBoxModifier.Name = "textBoxModifier";
            this.textBoxModifier.ReadOnly = true;
            this.textBoxModifier.ShortcutsEnabled = false;
            this.textBoxModifier.Size = new System.Drawing.Size(201, 23);
            this.textBoxModifier.TabIndex = 10;
            this.textBoxModifier.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxModifier_KeyDown);
            this.textBoxModifier.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxModifier_MouseDown);
            // 
            // textBoxAltKey
            // 
            this.textBoxAltKey.Location = new System.Drawing.Point(105, 57);
            this.textBoxAltKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBoxAltKey.Name = "textBoxAltKey";
            this.textBoxAltKey.ReadOnly = true;
            this.textBoxAltKey.ShortcutsEnabled = false;
            this.textBoxAltKey.Size = new System.Drawing.Size(201, 23);
            this.textBoxAltKey.TabIndex = 9;
            this.textBoxAltKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxAltKey_KeyDown);
            this.textBoxAltKey.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxAltKey_MouseDown);
            // 
            // textBoxMainKey
            // 
            this.textBoxMainKey.Location = new System.Drawing.Point(105, 22);
            this.textBoxMainKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBoxMainKey.Name = "textBoxMainKey";
            this.textBoxMainKey.ReadOnly = true;
            this.textBoxMainKey.ShortcutsEnabled = false;
            this.textBoxMainKey.Size = new System.Drawing.Size(201, 23);
            this.textBoxMainKey.TabIndex = 8;
            this.textBoxMainKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMainKey_KeyDown);
            this.textBoxMainKey.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxMainKey_MouseDown);
            // 
            // labelMainKey
            // 
            this.labelMainKey.AutoSize = true;
            this.labelMainKey.Location = new System.Drawing.Point(35, 25);
            this.labelMainKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMainKey.Name = "labelMainKey";
            this.labelMainKey.Size = new System.Drawing.Size(59, 15);
            this.labelMainKey.TabIndex = 4;
            this.labelMainKey.Text = "Main Key:";
            // 
            // labelModifier
            // 
            this.labelModifier.AutoSize = true;
            this.labelModifier.Location = new System.Drawing.Point(22, 95);
            this.labelModifier.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelModifier.Name = "labelModifier";
            this.labelModifier.Size = new System.Drawing.Size(80, 15);
            this.labelModifier.TabIndex = 7;
            this.labelModifier.Text = "Modifier Key: ";
            // 
            // labelAltKey
            // 
            this.labelAltKey.AutoSize = true;
            this.labelAltKey.Location = new System.Drawing.Point(7, 60);
            this.labelAltKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAltKey.Name = "labelAltKey";
            this.labelAltKey.Size = new System.Drawing.Size(92, 15);
            this.labelAltKey.TabIndex = 6;
            this.labelAltKey.Text = "Alternative Key: ";
            // 
            // radioButtonLook
            // 
            this.radioButtonLook.AutoSize = true;
            this.radioButtonLook.Location = new System.Drawing.Point(456, 270);
            this.radioButtonLook.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButtonLook.Name = "radioButtonLook";
            this.radioButtonLook.Size = new System.Drawing.Size(51, 19);
            this.radioButtonLook.TabIndex = 16;
            this.radioButtonLook.Text = "Look";
            this.radioButtonLook.UseVisualStyleBackColor = true;
            this.radioButtonLook.Click += new System.EventHandler(this.radioButtonLook_Click);
            // 
            // radioButtonMove
            // 
            this.radioButtonMove.AutoSize = true;
            this.radioButtonMove.Checked = true;
            this.radioButtonMove.Location = new System.Drawing.Point(388, 270);
            this.radioButtonMove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButtonMove.Name = "radioButtonMove";
            this.radioButtonMove.Size = new System.Drawing.Size(55, 19);
            this.radioButtonMove.TabIndex = 15;
            this.radioButtonMove.TabStop = true;
            this.radioButtonMove.Text = "Move";
            this.radioButtonMove.UseVisualStyleBackColor = true;
            this.radioButtonMove.Click += new System.EventHandler(this.radioButtonMove_Click);
            // 
            // labelCameraModifier
            // 
            this.labelCameraModifier.AutoSize = true;
            this.labelCameraModifier.Location = new System.Drawing.Point(275, 272);
            this.labelCameraModifier.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCameraModifier.Name = "labelCameraModifier";
            this.labelCameraModifier.Size = new System.Drawing.Size(99, 15);
            this.labelCameraModifier.TabIndex = 14;
            this.labelCameraModifier.Text = "Camera Modifier:";
            // 
            // EditorOptionsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(608, 367);
            this.Controls.Add(this.tabControlOptions);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonDone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "EditorOptionsEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editor Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorOptionsEditor_FormClosing);
            this.groupBoxRenderOptions.ResumeLayout(false);
            this.groupBoxRenderOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackgroundColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDrawDistanceGeneral)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBoxDrawDistance.ResumeLayout(false);
            this.groupBoxDrawDistance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDrawDistanceSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDrawDistanceLevel)).EndInit();
            this.tabControlOptions.ResumeLayout(false);
            this.tabPageDisplay.ResumeLayout(false);
            this.tabPageControl.ResumeLayout(false);
            this.tabPageControl.PerformLayout();
            this.groupBoxActions.ResumeLayout(false);
            this.groupBoxKeys.ResumeLayout(false);
            this.groupBoxKeys.PerformLayout();
            this.groupBoxDescription.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxRenderOptions;
        private System.Windows.Forms.Button buttonDone;
        private System.Windows.Forms.Label labelDrawDistanceGeneral;
        private System.Windows.Forms.TrackBar trackBarDrawDistanceGeneral;
        private System.Windows.Forms.Label labelPolygonFillMode;
        private System.Windows.Forms.ComboBox comboBoxFillMode;
        private System.Windows.Forms.Label labelBackfaceCulling;
        private System.Windows.Forms.ComboBox comboBoxBackfaceCulling;
        private System.Windows.Forms.CheckBox checkboxDisableLighting;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button KeyboardShortcutButton;
        private System.Windows.Forms.Button buttonResetDefaultKeys;
		private System.Windows.Forms.GroupBox groupBoxDrawDistance;
		private System.Windows.Forms.TrackBar trackBarDrawDistanceSet;
		private System.Windows.Forms.Label labelDrawDistanceSetCam;
		private System.Windows.Forms.TrackBar trackBarDrawDistanceLevel;
		private System.Windows.Forms.Label labelDrawDistanceLevel;
        private System.Windows.Forms.CheckBox checkBoxIgnoreMaterialColors;
        private System.Windows.Forms.TabControl tabControlOptions;
        private System.Windows.Forms.TabPage tabPageDisplay;
        private System.Windows.Forms.TabPage tabPageControl;
        private System.Windows.Forms.Label labelModifier;
        private System.Windows.Forms.Label labelAltKey;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelMainKey;
        private System.Windows.Forms.ListBox listBoxActions;
        private System.Windows.Forms.TextBox textBoxMainKey;
        private System.Windows.Forms.GroupBox groupBoxActions;
        private System.Windows.Forms.GroupBox groupBoxKeys;
        private System.Windows.Forms.TextBox textBoxModifier;
        private System.Windows.Forms.TextBox textBoxAltKey;
        private System.Windows.Forms.Button buttonResetSelectedKey;
        private System.Windows.Forms.Button buttonClearSelectedKey;
        private System.Windows.Forms.GroupBox groupBoxDescription;
		private System.Windows.Forms.RadioButton radioButtonZoom;
		private System.Windows.Forms.RadioButton radioButtonLook;
		private System.Windows.Forms.RadioButton radioButtonMove;
		private System.Windows.Forms.Label labelCameraModifier;
		private System.Windows.Forms.PictureBox pictureBoxBackgroundColor;
		private System.Windows.Forms.Label labelBackgroundColor;
	}
}