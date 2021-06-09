namespace SonicRetro.SAModel.SAEditorCommon.UI
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ignoreMaterialColorsCheck = new System.Windows.Forms.CheckBox();
            this.fullBrightCheck = new System.Windows.Forms.CheckBox();
            this.cullModeDropdown = new System.Windows.Forms.ComboBox();
            this.cullingLabel = new System.Windows.Forms.Label();
            this.polyFillLabel = new System.Windows.Forms.Label();
            this.fillModeDropDown = new System.Windows.Forms.ComboBox();
            this.drawDistLabel = new System.Windows.Forms.Label();
            this.drawDistSlider = new System.Windows.Forms.TrackBar();
            this.doneButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.KeyboardShortcutButton = new System.Windows.Forms.Button();
            this.ResetDefaultKeybindButton = new System.Windows.Forms.Button();
            this.drawDistGroupBox = new System.Windows.Forms.GroupBox();
            this.setDrawDistSlider = new System.Windows.Forms.TrackBar();
            this.setDrawDistLabel = new System.Windows.Forms.Label();
            this.levelDrawDistSlider = new System.Windows.Forms.TrackBar();
            this.levelDrawDistLabel = new System.Windows.Forms.Label();
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
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drawDistSlider)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.drawDistGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setDrawDistSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.levelDrawDistSlider)).BeginInit();
            this.tabControlOptions.SuspendLayout();
            this.tabPageDisplay.SuspendLayout();
            this.tabPageControl.SuspendLayout();
            this.groupBoxActions.SuspendLayout();
            this.groupBoxKeys.SuspendLayout();
            this.groupBoxDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ignoreMaterialColorsCheck);
            this.groupBox1.Controls.Add(this.fullBrightCheck);
            this.groupBox1.Controls.Add(this.cullModeDropdown);
            this.groupBox1.Controls.Add(this.cullingLabel);
            this.groupBox1.Controls.Add(this.polyFillLabel);
            this.groupBox1.Controls.Add(this.fillModeDropDown);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 168);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Render Options";
            // 
            // ignoreMaterialColorsCheck
            // 
            this.ignoreMaterialColorsCheck.AutoSize = true;
            this.ignoreMaterialColorsCheck.Location = new System.Drawing.Point(15, 143);
            this.ignoreMaterialColorsCheck.Name = "ignoreMaterialColorsCheck";
            this.ignoreMaterialColorsCheck.Size = new System.Drawing.Size(126, 17);
            this.ignoreMaterialColorsCheck.TabIndex = 6;
            this.ignoreMaterialColorsCheck.Text = "Ignore material colors";
            this.ignoreMaterialColorsCheck.UseVisualStyleBackColor = true;
            this.ignoreMaterialColorsCheck.Click += new System.EventHandler(this.ignoreMaterialColorsCheck_Click);
            // 
            // fullBrightCheck
            // 
            this.fullBrightCheck.AutoSize = true;
            this.fullBrightCheck.Location = new System.Drawing.Point(15, 120);
            this.fullBrightCheck.Name = "fullBrightCheck";
            this.fullBrightCheck.Size = new System.Drawing.Size(97, 17);
            this.fullBrightCheck.TabIndex = 5;
            this.fullBrightCheck.Text = "Disable lighting";
            this.fullBrightCheck.UseVisualStyleBackColor = true;
            this.fullBrightCheck.Click += new System.EventHandler(this.fullBrightCheck_Click);
            // 
            // cullModeDropdown
            // 
            this.cullModeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cullModeDropdown.FormattingEnabled = true;
            this.cullModeDropdown.Items.AddRange(new object[] {
            "None (Draws both sides)",
            "Clockwise",
            "Counter-Clockwise"});
            this.cullModeDropdown.Location = new System.Drawing.Point(15, 90);
            this.cullModeDropdown.Name = "cullModeDropdown";
            this.cullModeDropdown.Size = new System.Drawing.Size(196, 21);
            this.cullModeDropdown.TabIndex = 4;
            this.cullModeDropdown.SelectionChangeCommitted += new System.EventHandler(this.cullModeDropdown_SelectionChangeCommitted);
            // 
            // cullingLabel
            // 
            this.cullingLabel.AutoSize = true;
            this.cullingLabel.Location = new System.Drawing.Point(12, 74);
            this.cullingLabel.Name = "cullingLabel";
            this.cullingLabel.Size = new System.Drawing.Size(90, 13);
            this.cullingLabel.TabIndex = 2;
            this.cullingLabel.Text = "Backface Culling:";
            // 
            // polyFillLabel
            // 
            this.polyFillLabel.AutoSize = true;
            this.polyFillLabel.Location = new System.Drawing.Point(12, 26);
            this.polyFillLabel.Name = "polyFillLabel";
            this.polyFillLabel.Size = new System.Drawing.Size(93, 13);
            this.polyFillLabel.TabIndex = 3;
            this.polyFillLabel.Text = "Polygon Fill Mode:";
            // 
            // fillModeDropDown
            // 
            this.fillModeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fillModeDropDown.FormattingEnabled = true;
            this.fillModeDropDown.Items.AddRange(new object[] {
            "Vertex-Only",
            "Wireframe",
            "Solid"});
            this.fillModeDropDown.Location = new System.Drawing.Point(15, 42);
            this.fillModeDropDown.Name = "fillModeDropDown";
            this.fillModeDropDown.Size = new System.Drawing.Size(196, 21);
            this.fillModeDropDown.TabIndex = 2;
            this.fillModeDropDown.SelectionChangeCommitted += new System.EventHandler(this.fillModeDropDown_SelectionChangeCommitted);
            // 
            // drawDistLabel
            // 
            this.drawDistLabel.AutoSize = true;
            this.drawDistLabel.Location = new System.Drawing.Point(15, 25);
            this.drawDistLabel.Name = "drawDistLabel";
            this.drawDistLabel.Size = new System.Drawing.Size(47, 13);
            this.drawDistLabel.TabIndex = 1;
            this.drawDistLabel.Text = "General:";
            // 
            // drawDistSlider
            // 
            this.drawDistSlider.Location = new System.Drawing.Point(6, 41);
            this.drawDistSlider.Maximum = 30000;
            this.drawDistSlider.Minimum = 1000;
            this.drawDistSlider.Name = "drawDistSlider";
            this.drawDistSlider.Size = new System.Drawing.Size(227, 45);
            this.drawDistSlider.SmallChange = 100;
            this.drawDistSlider.TabIndex = 0;
            this.drawDistSlider.TickFrequency = 500;
            this.drawDistSlider.Value = 6000;
            this.drawDistSlider.Scroll += new System.EventHandler(this.drawDistSlider_Scroll);
            // 
            // doneButton
            // 
            this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.doneButton.Location = new System.Drawing.Point(434, 289);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 1;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.KeyboardShortcutButton);
            this.groupBox2.Location = new System.Drawing.Point(593, 133);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 89);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input Options";
            // 
            // KeyboardShortcutButton
            // 
            this.KeyboardShortcutButton.Location = new System.Drawing.Point(6, 21);
            this.KeyboardShortcutButton.Name = "KeyboardShortcutButton";
            this.KeyboardShortcutButton.Size = new System.Drawing.Size(243, 23);
            this.KeyboardShortcutButton.TabIndex = 0;
            this.KeyboardShortcutButton.Text = "Configure Keyboard Shortcuts";
            this.KeyboardShortcutButton.UseVisualStyleBackColor = true;
            this.KeyboardShortcutButton.Click += new System.EventHandler(this.KeyboardShortcutButton_Click);
            // 
            // ResetDefaultKeybindButton
            // 
            this.ResetDefaultKeybindButton.Location = new System.Drawing.Point(6, 210);
            this.ResetDefaultKeybindButton.Name = "ResetDefaultKeybindButton";
            this.ResetDefaultKeybindButton.Size = new System.Drawing.Size(201, 23);
            this.ResetDefaultKeybindButton.TabIndex = 1;
            this.ResetDefaultKeybindButton.Text = "Restore Default Keys";
            this.ResetDefaultKeybindButton.UseVisualStyleBackColor = true;
            this.ResetDefaultKeybindButton.Click += new System.EventHandler(this.ResetDefaultKeybindButton_Click);
            // 
            // drawDistGroupBox
            // 
            this.drawDistGroupBox.Controls.Add(this.setDrawDistSlider);
            this.drawDistGroupBox.Controls.Add(this.setDrawDistLabel);
            this.drawDistGroupBox.Controls.Add(this.levelDrawDistSlider);
            this.drawDistGroupBox.Controls.Add(this.levelDrawDistLabel);
            this.drawDistGroupBox.Controls.Add(this.drawDistSlider);
            this.drawDistGroupBox.Controls.Add(this.drawDistLabel);
            this.drawDistGroupBox.Location = new System.Drawing.Point(267, 6);
            this.drawDistGroupBox.Name = "drawDistGroupBox";
            this.drawDistGroupBox.Size = new System.Drawing.Size(243, 219);
            this.drawDistGroupBox.TabIndex = 3;
            this.drawDistGroupBox.TabStop = false;
            this.drawDistGroupBox.Text = "Draw Distance";
            // 
            // setDrawDistSlider
            // 
            this.setDrawDistSlider.Enabled = false;
            this.setDrawDistSlider.Location = new System.Drawing.Point(6, 169);
            this.setDrawDistSlider.Maximum = 30000;
            this.setDrawDistSlider.Minimum = 1000;
            this.setDrawDistSlider.Name = "setDrawDistSlider";
            this.setDrawDistSlider.Size = new System.Drawing.Size(227, 45);
            this.setDrawDistSlider.SmallChange = 100;
            this.setDrawDistSlider.TabIndex = 5;
            this.setDrawDistSlider.TickFrequency = 500;
            this.setDrawDistSlider.Value = 6000;
            this.setDrawDistSlider.Scroll += new System.EventHandler(this.setDrawDistSlider_Scroll);
            // 
            // setDrawDistLabel
            // 
            this.setDrawDistLabel.AutoSize = true;
            this.setDrawDistLabel.Enabled = false;
            this.setDrawDistLabel.Location = new System.Drawing.Point(15, 153);
            this.setDrawDistLabel.Name = "setDrawDistLabel";
            this.setDrawDistLabel.Size = new System.Drawing.Size(87, 13);
            this.setDrawDistLabel.TabIndex = 4;
            this.setDrawDistLabel.Text = "SET/CAM Items:";
            // 
            // levelDrawDistSlider
            // 
            this.levelDrawDistSlider.Enabled = false;
            this.levelDrawDistSlider.Location = new System.Drawing.Point(6, 105);
            this.levelDrawDistSlider.Maximum = 30000;
            this.levelDrawDistSlider.Minimum = 1000;
            this.levelDrawDistSlider.Name = "levelDrawDistSlider";
            this.levelDrawDistSlider.Size = new System.Drawing.Size(227, 45);
            this.levelDrawDistSlider.SmallChange = 100;
            this.levelDrawDistSlider.TabIndex = 3;
            this.levelDrawDistSlider.TickFrequency = 500;
            this.levelDrawDistSlider.Value = 6000;
            this.levelDrawDistSlider.Scroll += new System.EventHandler(this.levelDrawDistSlider_Scroll);
            // 
            // levelDrawDistLabel
            // 
            this.levelDrawDistLabel.AutoSize = true;
            this.levelDrawDistLabel.Enabled = false;
            this.levelDrawDistLabel.Location = new System.Drawing.Point(15, 89);
            this.levelDrawDistLabel.Name = "levelDrawDistLabel";
            this.levelDrawDistLabel.Size = new System.Drawing.Size(84, 13);
            this.levelDrawDistLabel.TabIndex = 2;
            this.levelDrawDistLabel.Text = "Level Geometry:";
            // 
            // tabControlOptions
            // 
            this.tabControlOptions.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControlOptions.Controls.Add(this.tabPageDisplay);
            this.tabControlOptions.Controls.Add(this.tabPageControl);
            this.tabControlOptions.Location = new System.Drawing.Point(0, 0);
            this.tabControlOptions.Name = "tabControlOptions";
            this.tabControlOptions.SelectedIndex = 0;
            this.tabControlOptions.Size = new System.Drawing.Size(524, 287);
            this.tabControlOptions.TabIndex = 7;
            // 
            // tabPageDisplay
            // 
            this.tabPageDisplay.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageDisplay.Controls.Add(this.groupBox1);
            this.tabPageDisplay.Controls.Add(this.drawDistGroupBox);
            this.tabPageDisplay.Location = new System.Drawing.Point(4, 25);
            this.tabPageDisplay.Name = "tabPageDisplay";
            this.tabPageDisplay.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDisplay.Size = new System.Drawing.Size(516, 258);
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
            this.tabPageControl.Location = new System.Drawing.Point(4, 25);
            this.tabPageControl.Name = "tabPageControl";
            this.tabPageControl.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageControl.Size = new System.Drawing.Size(516, 258);
            this.tabPageControl.TabIndex = 1;
            this.tabPageControl.Text = "Control Options";
            // 
            // radioButtonZoom
            // 
            this.radioButtonZoom.AutoSize = true;
            this.radioButtonZoom.Location = new System.Drawing.Point(446, 234);
            this.radioButtonZoom.Name = "radioButtonZoom";
            this.radioButtonZoom.Size = new System.Drawing.Size(52, 17);
            this.radioButtonZoom.TabIndex = 17;
            this.radioButtonZoom.Text = "Zoom";
            this.radioButtonZoom.UseVisualStyleBackColor = true;
            this.radioButtonZoom.Click += new System.EventHandler(this.radioButtonZoom_Click);
            // 
            // groupBoxActions
            // 
            this.groupBoxActions.Controls.Add(this.listBoxActions);
            this.groupBoxActions.Controls.Add(this.ResetDefaultKeybindButton);
            this.groupBoxActions.Location = new System.Drawing.Point(6, 6);
            this.groupBoxActions.Name = "groupBoxActions";
            this.groupBoxActions.Size = new System.Drawing.Size(217, 242);
            this.groupBoxActions.TabIndex = 8;
            this.groupBoxActions.TabStop = false;
            this.groupBoxActions.Text = "Actions";
            // 
            // listBoxActions
            // 
            this.listBoxActions.FormattingEnabled = true;
            this.listBoxActions.Location = new System.Drawing.Point(6, 19);
            this.listBoxActions.Name = "listBoxActions";
            this.listBoxActions.Size = new System.Drawing.Size(201, 186);
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
            this.groupBoxKeys.Location = new System.Drawing.Point(229, 6);
            this.groupBoxKeys.Name = "groupBoxKeys";
            this.groupBoxKeys.Size = new System.Drawing.Size(281, 224);
            this.groupBoxKeys.TabIndex = 10;
            this.groupBoxKeys.TabStop = false;
            this.groupBoxKeys.Text = "Keys";
            // 
            // groupBoxDescription
            // 
            this.groupBoxDescription.Controls.Add(this.labelDescription);
            this.groupBoxDescription.Location = new System.Drawing.Point(9, 136);
            this.groupBoxDescription.Name = "groupBoxDescription";
            this.groupBoxDescription.Size = new System.Drawing.Size(266, 77);
            this.groupBoxDescription.TabIndex = 13;
            this.groupBoxDescription.TabStop = false;
            this.groupBoxDescription.Text = "Description";
            // 
            // labelDescription
            // 
            this.labelDescription.Location = new System.Drawing.Point(6, 16);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(248, 53);
            this.labelDescription.TabIndex = 5;
            // 
            // buttonResetSelectedKey
            // 
            this.buttonResetSelectedKey.Location = new System.Drawing.Point(150, 107);
            this.buttonResetSelectedKey.Name = "buttonResetSelectedKey";
            this.buttonResetSelectedKey.Size = new System.Drawing.Size(125, 23);
            this.buttonResetSelectedKey.TabIndex = 12;
            this.buttonResetSelectedKey.Text = "Reset Selected Key";
            this.buttonResetSelectedKey.UseVisualStyleBackColor = true;
            this.buttonResetSelectedKey.Click += new System.EventHandler(this.buttonResetSelectedKey_Click);
            // 
            // buttonClearSelectedKey
            // 
            this.buttonClearSelectedKey.Location = new System.Drawing.Point(9, 107);
            this.buttonClearSelectedKey.Name = "buttonClearSelectedKey";
            this.buttonClearSelectedKey.Size = new System.Drawing.Size(125, 23);
            this.buttonClearSelectedKey.TabIndex = 11;
            this.buttonClearSelectedKey.Text = "Clear Selected Key";
            this.buttonClearSelectedKey.UseVisualStyleBackColor = true;
            this.buttonClearSelectedKey.Click += new System.EventHandler(this.buttonClearSelectedKey_Click);
            // 
            // textBoxModifier
            // 
            this.textBoxModifier.Location = new System.Drawing.Point(90, 79);
            this.textBoxModifier.Name = "textBoxModifier";
            this.textBoxModifier.ReadOnly = true;
            this.textBoxModifier.Size = new System.Drawing.Size(173, 20);
            this.textBoxModifier.TabIndex = 10;
            this.textBoxModifier.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxModifier_KeyDown);
            this.textBoxModifier.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxModifier_MouseDown);
			this.textBoxModifier.ShortcutsEnabled = false;
			// 
			// textBoxAltKey
			// 
			this.textBoxAltKey.Location = new System.Drawing.Point(90, 49);
            this.textBoxAltKey.Name = "textBoxAltKey";
            this.textBoxAltKey.ReadOnly = true;
            this.textBoxAltKey.Size = new System.Drawing.Size(173, 20);
            this.textBoxAltKey.TabIndex = 9;
            this.textBoxAltKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxAltKey_KeyDown);
            this.textBoxAltKey.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxAltKey_MouseDown);
			this.textBoxAltKey.ShortcutsEnabled = false;
			// 
			// textBoxMainKey
			// 
			this.textBoxMainKey.Location = new System.Drawing.Point(90, 19);
            this.textBoxMainKey.Name = "textBoxMainKey";
            this.textBoxMainKey.ReadOnly = true;
            this.textBoxMainKey.Size = new System.Drawing.Size(173, 20);
            this.textBoxMainKey.TabIndex = 8;
            this.textBoxMainKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMainKey_KeyDown);
            this.textBoxMainKey.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxMainKey_MouseDown);
			this.textBoxMainKey.ShortcutsEnabled = false;
			// 
			// labelMainKey
			// 
			this.labelMainKey.AutoSize = true;
            this.labelMainKey.Location = new System.Drawing.Point(30, 22);
            this.labelMainKey.Name = "labelMainKey";
            this.labelMainKey.Size = new System.Drawing.Size(54, 13);
            this.labelMainKey.TabIndex = 4;
            this.labelMainKey.Text = "Main Key:";
            // 
            // labelModifier
            // 
            this.labelModifier.AutoSize = true;
            this.labelModifier.Location = new System.Drawing.Point(19, 82);
            this.labelModifier.Name = "labelModifier";
            this.labelModifier.Size = new System.Drawing.Size(71, 13);
            this.labelModifier.TabIndex = 7;
            this.labelModifier.Text = "Modifier Key: ";
            // 
            // labelAltKey
            // 
            this.labelAltKey.AutoSize = true;
            this.labelAltKey.Location = new System.Drawing.Point(6, 52);
            this.labelAltKey.Name = "labelAltKey";
            this.labelAltKey.Size = new System.Drawing.Size(84, 13);
            this.labelAltKey.TabIndex = 6;
            this.labelAltKey.Text = "Alternative Key: ";
            // 
            // radioButtonLook
            // 
            this.radioButtonLook.AutoSize = true;
            this.radioButtonLook.Location = new System.Drawing.Point(391, 234);
            this.radioButtonLook.Name = "radioButtonLook";
            this.radioButtonLook.Size = new System.Drawing.Size(49, 17);
            this.radioButtonLook.TabIndex = 16;
            this.radioButtonLook.Text = "Look";
            this.radioButtonLook.UseVisualStyleBackColor = true;
            this.radioButtonLook.Click += new System.EventHandler(this.radioButtonLook_Click);
            // 
            // radioButtonMove
            // 
            this.radioButtonMove.AutoSize = true;
            this.radioButtonMove.Checked = true;
            this.radioButtonMove.Location = new System.Drawing.Point(333, 234);
            this.radioButtonMove.Name = "radioButtonMove";
            this.radioButtonMove.Size = new System.Drawing.Size(52, 17);
            this.radioButtonMove.TabIndex = 15;
            this.radioButtonMove.TabStop = true;
            this.radioButtonMove.Text = "Move";
            this.radioButtonMove.UseVisualStyleBackColor = true;
            this.radioButtonMove.Click += new System.EventHandler(this.radioButtonMove_Click);
            // 
            // labelCameraModifier
            // 
            this.labelCameraModifier.AutoSize = true;
            this.labelCameraModifier.Location = new System.Drawing.Point(236, 236);
            this.labelCameraModifier.Name = "labelCameraModifier";
            this.labelCameraModifier.Size = new System.Drawing.Size(86, 13);
            this.labelCameraModifier.TabIndex = 14;
            this.labelCameraModifier.Text = "Camera Modifier:";
            // 
            // EditorOptionsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 318);
            this.Controls.Add(this.tabControlOptions);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.doneButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
            this.Name = "EditorOptionsEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editor Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorOptionsEditor_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drawDistSlider)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.drawDistGroupBox.ResumeLayout(false);
            this.drawDistGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setDrawDistSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.levelDrawDistSlider)).EndInit();
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

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.Label drawDistLabel;
        private System.Windows.Forms.TrackBar drawDistSlider;
        private System.Windows.Forms.Label polyFillLabel;
        private System.Windows.Forms.ComboBox fillModeDropDown;
        private System.Windows.Forms.Label cullingLabel;
        private System.Windows.Forms.ComboBox cullModeDropdown;
        private System.Windows.Forms.CheckBox fullBrightCheck;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button KeyboardShortcutButton;
        private System.Windows.Forms.Button ResetDefaultKeybindButton;
		private System.Windows.Forms.GroupBox drawDistGroupBox;
		private System.Windows.Forms.TrackBar setDrawDistSlider;
		private System.Windows.Forms.Label setDrawDistLabel;
		private System.Windows.Forms.TrackBar levelDrawDistSlider;
		private System.Windows.Forms.Label levelDrawDistLabel;
        private System.Windows.Forms.CheckBox ignoreMaterialColorsCheck;
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
	}
}