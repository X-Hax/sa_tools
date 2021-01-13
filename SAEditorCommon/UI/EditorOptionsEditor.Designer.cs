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
            this.ResetDefaultKeybindButton = new System.Windows.Forms.Button();
            this.KeyboardShortcutButton = new System.Windows.Forms.Button();
            this.drawDistGroupBox = new System.Windows.Forms.GroupBox();
            this.setDrawDistSlider = new System.Windows.Forms.TrackBar();
            this.setDrawDistLabel = new System.Windows.Forms.Label();
            this.levelDrawDistSlider = new System.Windows.Forms.TrackBar();
            this.levelDrawDistLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drawDistSlider)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.drawDistGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setDrawDistSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.levelDrawDistSlider)).BeginInit();
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
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
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
            this.doneButton.Location = new System.Drawing.Point(443, 252);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 1;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ResetDefaultKeybindButton);
            this.groupBox2.Controls.Add(this.KeyboardShortcutButton);
            this.groupBox2.Location = new System.Drawing.Point(12, 186);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 89);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input Options";
            // 
            // ResetDefaultKeybindButton
            // 
            this.ResetDefaultKeybindButton.Location = new System.Drawing.Point(6, 50);
            this.ResetDefaultKeybindButton.Name = "ResetDefaultKeybindButton";
            this.ResetDefaultKeybindButton.Size = new System.Drawing.Size(243, 23);
            this.ResetDefaultKeybindButton.TabIndex = 1;
            this.ResetDefaultKeybindButton.Text = "Reset Default Keybinds";
            this.ResetDefaultKeybindButton.UseVisualStyleBackColor = true;
            this.ResetDefaultKeybindButton.Click += new System.EventHandler(this.ResetDefaultKeybindButton_Click);
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
            // drawDistGroupBox
            // 
            this.drawDistGroupBox.Controls.Add(this.setDrawDistSlider);
            this.drawDistGroupBox.Controls.Add(this.setDrawDistLabel);
            this.drawDistGroupBox.Controls.Add(this.levelDrawDistSlider);
            this.drawDistGroupBox.Controls.Add(this.levelDrawDistLabel);
            this.drawDistGroupBox.Controls.Add(this.drawDistSlider);
            this.drawDistGroupBox.Controls.Add(this.drawDistLabel);
            this.drawDistGroupBox.Location = new System.Drawing.Point(275, 12);
            this.drawDistGroupBox.Name = "drawDistGroupBox";
            this.drawDistGroupBox.Size = new System.Drawing.Size(243, 234);
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
            // EditorOptionsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 287);
            this.Controls.Add(this.drawDistGroupBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.doneButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "EditorOptionsEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editor Options / Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorOptionsEditor_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drawDistSlider)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.drawDistGroupBox.ResumeLayout(false);
            this.drawDistGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setDrawDistSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.levelDrawDistSlider)).EndInit();
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
    }
}