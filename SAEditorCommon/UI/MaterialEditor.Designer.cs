namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    partial class MaterialEditor
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.currentMaterialLabel = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.flagsGroupBox = new System.Windows.Forms.GroupBox();
            this.userFlagsLabel = new System.Windows.Forms.Label();
            this.userFlagsNumeric = new System.Windows.Forms.NumericUpDown();
            this.ignoreLightCheck = new System.Windows.Forms.CheckBox();
            this.flatShadeCheck = new System.Windows.Forms.CheckBox();
            this.doubleSideCheck = new System.Windows.Forms.CheckBox();
            this.envMapCheck = new System.Windows.Forms.CheckBox();
            this.useTextureCheck = new System.Windows.Forms.CheckBox();
            this.useAlphaCheck = new System.Windows.Forms.CheckBox();
            this.ignoreSpecCheck = new System.Windows.Forms.CheckBox();
            this.flipVCheck = new System.Windows.Forms.CheckBox();
            this.flipUCheck = new System.Windows.Forms.CheckBox();
            this.clampVCheck = new System.Windows.Forms.CheckBox();
            this.clampUCheck = new System.Windows.Forms.CheckBox();
            this.superSampleCheck = new System.Windows.Forms.CheckBox();
            this.pickStatusCheck = new System.Windows.Forms.CheckBox();
            this.generalSettingBox = new System.Windows.Forms.GroupBox();
            this.dstAlphaCombo = new System.Windows.Forms.ComboBox();
            this.destinationAlphaLabel = new System.Windows.Forms.Label();
            this.srcAlphaCombo = new System.Windows.Forms.ComboBox();
            this.srcAlphaLabel = new System.Windows.Forms.Label();
            this.filterModeDropDown = new System.Windows.Forms.ComboBox();
            this.filterModeLabel = new System.Windows.Forms.Label();
            this.specColorBox = new System.Windows.Forms.PictureBox();
            this.diffuseColorBox = new System.Windows.Forms.PictureBox();
            this.exponentTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textureBox = new SonicRetro.SAModel.SAEditorCommon.UI.HCPictureBox();
            this.exponentLabel = new System.Windows.Forms.Label();
            this.specColorLabel = new System.Windows.Forms.Label();
            this.diffuseLabel = new System.Windows.Forms.Label();
            this.doneButton = new System.Windows.Forms.Button();
            this.flagsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userFlagsNumeric)).BeginInit();
            this.generalSettingBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specColorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.diffuseColorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(102, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(171, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // currentMaterialLabel
            // 
            this.currentMaterialLabel.AutoSize = true;
            this.currentMaterialLabel.Location = new System.Drawing.Point(12, 9);
            this.currentMaterialLabel.Name = "currentMaterialLabel";
            this.currentMaterialLabel.Size = new System.Drawing.Size(84, 13);
            this.currentMaterialLabel.TabIndex = 1;
            this.currentMaterialLabel.Text = "Current Material:";
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.SolidColorOnly = true;
            // 
            // flagsGroupBox
            // 
            this.flagsGroupBox.Controls.Add(this.userFlagsLabel);
            this.flagsGroupBox.Controls.Add(this.userFlagsNumeric);
            this.flagsGroupBox.Controls.Add(this.ignoreLightCheck);
            this.flagsGroupBox.Controls.Add(this.flatShadeCheck);
            this.flagsGroupBox.Controls.Add(this.doubleSideCheck);
            this.flagsGroupBox.Controls.Add(this.envMapCheck);
            this.flagsGroupBox.Controls.Add(this.useTextureCheck);
            this.flagsGroupBox.Controls.Add(this.useAlphaCheck);
            this.flagsGroupBox.Controls.Add(this.ignoreSpecCheck);
            this.flagsGroupBox.Controls.Add(this.flipVCheck);
            this.flagsGroupBox.Controls.Add(this.flipUCheck);
            this.flagsGroupBox.Controls.Add(this.clampVCheck);
            this.flagsGroupBox.Controls.Add(this.clampUCheck);
            this.flagsGroupBox.Controls.Add(this.superSampleCheck);
            this.flagsGroupBox.Controls.Add(this.pickStatusCheck);
            this.flagsGroupBox.Location = new System.Drawing.Point(277, 46);
            this.flagsGroupBox.Name = "flagsGroupBox";
            this.flagsGroupBox.Size = new System.Drawing.Size(246, 159);
            this.flagsGroupBox.TabIndex = 2;
            this.flagsGroupBox.TabStop = false;
            this.flagsGroupBox.Text = "Flags";
            // 
            // userFlagsLabel
            // 
            this.userFlagsLabel.AutoSize = true;
            this.userFlagsLabel.Location = new System.Drawing.Point(111, 134);
            this.userFlagsLabel.Name = "userFlagsLabel";
            this.userFlagsLabel.Size = new System.Drawing.Size(60, 13);
            this.userFlagsLabel.TabIndex = 11;
            this.userFlagsLabel.Text = "User Flags:";
            // 
            // userFlagsNumeric
            // 
            this.userFlagsNumeric.Hexadecimal = true;
            this.userFlagsNumeric.Location = new System.Drawing.Point(174, 130);
            this.userFlagsNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.userFlagsNumeric.Name = "userFlagsNumeric";
            this.userFlagsNumeric.Size = new System.Drawing.Size(59, 20);
            this.userFlagsNumeric.TabIndex = 10;
            this.userFlagsNumeric.ValueChanged += new System.EventHandler(this.userFlagsNumeric_ValueChanged);
            // 
            // ignoreLightCheck
            // 
            this.ignoreLightCheck.AutoSize = true;
            this.ignoreLightCheck.Location = new System.Drawing.Point(104, 112);
            this.ignoreLightCheck.Name = "ignoreLightCheck";
            this.ignoreLightCheck.Size = new System.Drawing.Size(96, 17);
            this.ignoreLightCheck.TabIndex = 3;
            this.ignoreLightCheck.Text = "Ignore Lighting";
            this.ignoreLightCheck.UseVisualStyleBackColor = true;
            this.ignoreLightCheck.Click += new System.EventHandler(this.ignoreLightCheck_Click);
            // 
            // flatShadeCheck
            // 
            this.flatShadeCheck.AutoSize = true;
            this.flatShadeCheck.Location = new System.Drawing.Point(104, 93);
            this.flatShadeCheck.Name = "flatShadeCheck";
            this.flatShadeCheck.Size = new System.Drawing.Size(83, 17);
            this.flatShadeCheck.TabIndex = 3;
            this.flatShadeCheck.Text = "Flat Shaded";
            this.flatShadeCheck.UseVisualStyleBackColor = true;
            this.flatShadeCheck.Click += new System.EventHandler(this.flatShadeCheck_Click);
            // 
            // doubleSideCheck
            // 
            this.doubleSideCheck.AutoSize = true;
            this.doubleSideCheck.Location = new System.Drawing.Point(104, 75);
            this.doubleSideCheck.Name = "doubleSideCheck";
            this.doubleSideCheck.Size = new System.Drawing.Size(90, 17);
            this.doubleSideCheck.TabIndex = 9;
            this.doubleSideCheck.Text = "Double Sided";
            this.doubleSideCheck.UseVisualStyleBackColor = true;
            this.doubleSideCheck.Click += new System.EventHandler(this.doubleSideCheck_Click);
            // 
            // envMapCheck
            // 
            this.envMapCheck.AutoSize = true;
            this.envMapCheck.Location = new System.Drawing.Point(104, 56);
            this.envMapCheck.Name = "envMapCheck";
            this.envMapCheck.Size = new System.Drawing.Size(129, 17);
            this.envMapCheck.TabIndex = 8;
            this.envMapCheck.Text = "Environment Mapping";
            this.envMapCheck.UseVisualStyleBackColor = true;
            this.envMapCheck.Click += new System.EventHandler(this.envMapCheck_Click);
            // 
            // useTextureCheck
            // 
            this.useTextureCheck.AutoSize = true;
            this.useTextureCheck.Location = new System.Drawing.Point(104, 37);
            this.useTextureCheck.Name = "useTextureCheck";
            this.useTextureCheck.Size = new System.Drawing.Size(84, 17);
            this.useTextureCheck.TabIndex = 7;
            this.useTextureCheck.Text = "Use Texture";
            this.useTextureCheck.UseVisualStyleBackColor = true;
            this.useTextureCheck.Click += new System.EventHandler(this.useTextureCheck_Click);
            // 
            // useAlphaCheck
            // 
            this.useAlphaCheck.AutoSize = true;
            this.useAlphaCheck.Location = new System.Drawing.Point(104, 19);
            this.useAlphaCheck.Name = "useAlphaCheck";
            this.useAlphaCheck.Size = new System.Drawing.Size(75, 17);
            this.useAlphaCheck.TabIndex = 6;
            this.useAlphaCheck.Text = "Use Alpha";
            this.useAlphaCheck.UseVisualStyleBackColor = true;
            this.useAlphaCheck.Click += new System.EventHandler(this.useAlphaCheck_Click);
            // 
            // ignoreSpecCheck
            // 
            this.ignoreSpecCheck.AutoSize = true;
            this.ignoreSpecCheck.Location = new System.Drawing.Point(6, 130);
            this.ignoreSpecCheck.Name = "ignoreSpecCheck";
            this.ignoreSpecCheck.Size = new System.Drawing.Size(101, 17);
            this.ignoreSpecCheck.TabIndex = 5;
            this.ignoreSpecCheck.Text = "Ignore Specular";
            this.ignoreSpecCheck.UseVisualStyleBackColor = true;
            this.ignoreSpecCheck.Click += new System.EventHandler(this.ignoreSpecCheck_Click);
            // 
            // flipVCheck
            // 
            this.flipVCheck.AutoSize = true;
            this.flipVCheck.Location = new System.Drawing.Point(6, 112);
            this.flipVCheck.Name = "flipVCheck";
            this.flipVCheck.Size = new System.Drawing.Size(62, 17);
            this.flipVCheck.TabIndex = 4;
            this.flipVCheck.Text = "Mirror V";
            this.flipVCheck.UseVisualStyleBackColor = true;
            this.flipVCheck.Click += new System.EventHandler(this.flipVCheck_Click);
            // 
            // flipUCheck
            // 
            this.flipUCheck.AutoSize = true;
            this.flipUCheck.Location = new System.Drawing.Point(6, 93);
            this.flipUCheck.Name = "flipUCheck";
            this.flipUCheck.Size = new System.Drawing.Size(63, 17);
            this.flipUCheck.TabIndex = 3;
            this.flipUCheck.Text = "Mirror U";
            this.flipUCheck.UseVisualStyleBackColor = true;
            this.flipUCheck.Click += new System.EventHandler(this.flipUCheck_Click);
            // 
            // clampVCheck
            // 
            this.clampVCheck.AutoSize = true;
            this.clampVCheck.Location = new System.Drawing.Point(6, 75);
            this.clampVCheck.Name = "clampVCheck";
            this.clampVCheck.Size = new System.Drawing.Size(65, 17);
            this.clampVCheck.TabIndex = 3;
            this.clampVCheck.Text = "Clamp V";
            this.clampVCheck.UseVisualStyleBackColor = true;
            this.clampVCheck.Click += new System.EventHandler(this.clampVCheck_Click);
            // 
            // clampUCheck
            // 
            this.clampUCheck.AutoSize = true;
            this.clampUCheck.Location = new System.Drawing.Point(6, 56);
            this.clampUCheck.Name = "clampUCheck";
            this.clampUCheck.Size = new System.Drawing.Size(66, 17);
            this.clampUCheck.TabIndex = 2;
            this.clampUCheck.Text = "Clamp U";
            this.clampUCheck.UseVisualStyleBackColor = true;
            this.clampUCheck.Click += new System.EventHandler(this.clampUCheck_Click);
            // 
            // superSampleCheck
            // 
            this.superSampleCheck.AutoSize = true;
            this.superSampleCheck.Location = new System.Drawing.Point(6, 37);
            this.superSampleCheck.Name = "superSampleCheck";
            this.superSampleCheck.Size = new System.Drawing.Size(92, 17);
            this.superSampleCheck.TabIndex = 1;
            this.superSampleCheck.Text = "Super Sample";
            this.superSampleCheck.UseVisualStyleBackColor = true;
            this.superSampleCheck.Click += new System.EventHandler(this.superSampleCheck_Click);
            // 
            // pickStatusCheck
            // 
            this.pickStatusCheck.AutoSize = true;
            this.pickStatusCheck.Location = new System.Drawing.Point(6, 19);
            this.pickStatusCheck.Name = "pickStatusCheck";
            this.pickStatusCheck.Size = new System.Drawing.Size(80, 17);
            this.pickStatusCheck.TabIndex = 0;
            this.pickStatusCheck.Text = "Pick Status";
            this.pickStatusCheck.UseVisualStyleBackColor = true;
            this.pickStatusCheck.Click += new System.EventHandler(this.pickStatusCheck_Click);
            // 
            // generalSettingBox
            // 
            this.generalSettingBox.Controls.Add(this.dstAlphaCombo);
            this.generalSettingBox.Controls.Add(this.destinationAlphaLabel);
            this.generalSettingBox.Controls.Add(this.srcAlphaCombo);
            this.generalSettingBox.Controls.Add(this.srcAlphaLabel);
            this.generalSettingBox.Controls.Add(this.filterModeDropDown);
            this.generalSettingBox.Controls.Add(this.filterModeLabel);
            this.generalSettingBox.Controls.Add(this.specColorBox);
            this.generalSettingBox.Controls.Add(this.diffuseColorBox);
            this.generalSettingBox.Controls.Add(this.exponentTextBox);
            this.generalSettingBox.Controls.Add(this.label1);
            this.generalSettingBox.Controls.Add(this.textureBox);
            this.generalSettingBox.Controls.Add(this.exponentLabel);
            this.generalSettingBox.Controls.Add(this.specColorLabel);
            this.generalSettingBox.Controls.Add(this.diffuseLabel);
            this.generalSettingBox.Location = new System.Drawing.Point(12, 47);
            this.generalSettingBox.Name = "generalSettingBox";
            this.generalSettingBox.Size = new System.Drawing.Size(254, 193);
            this.generalSettingBox.TabIndex = 3;
            this.generalSettingBox.TabStop = false;
            this.generalSettingBox.Text = "General";
            // 
            // dstAlphaCombo
            // 
            this.dstAlphaCombo.FormattingEnabled = true;
            this.dstAlphaCombo.Items.AddRange(new object[] {
            "Zero",
            "One",
            "OtherColor",
            "InverseOtherColor",
            "SourceAlpha",
            "InverseSourceAlpha",
            "DestinationAlpha",
            "InverseDestinationAlpha"});
            this.dstAlphaCombo.Location = new System.Drawing.Point(113, 157);
            this.dstAlphaCombo.Name = "dstAlphaCombo";
            this.dstAlphaCombo.Size = new System.Drawing.Size(121, 21);
            this.dstAlphaCombo.TabIndex = 11;
            this.dstAlphaCombo.SelectedIndexChanged += new System.EventHandler(this.dstAlphaCombo_SelectedIndexChanged);
            // 
            // destinationAlphaLabel
            // 
            this.destinationAlphaLabel.AutoSize = true;
            this.destinationAlphaLabel.Location = new System.Drawing.Point(110, 141);
            this.destinationAlphaLabel.Name = "destinationAlphaLabel";
            this.destinationAlphaLabel.Size = new System.Drawing.Size(93, 13);
            this.destinationAlphaLabel.TabIndex = 10;
            this.destinationAlphaLabel.Text = "Destination Alpha:";
            // 
            // srcAlphaCombo
            // 
            this.srcAlphaCombo.FormattingEnabled = true;
            this.srcAlphaCombo.Items.AddRange(new object[] {
            "Zero",
            "One",
            "OtherColor",
            "InverseOtherColor",
            "SourceAlpha",
            "InverseSourceAlpha",
            "DestinationAlpha",
            "InverseDestinationAlpha"});
            this.srcAlphaCombo.Location = new System.Drawing.Point(113, 117);
            this.srcAlphaCombo.Name = "srcAlphaCombo";
            this.srcAlphaCombo.Size = new System.Drawing.Size(121, 21);
            this.srcAlphaCombo.TabIndex = 9;
            this.srcAlphaCombo.SelectionChangeCommitted += new System.EventHandler(this.srcAlphaCombo_SelectionChangeCommitted);
            // 
            // srcAlphaLabel
            // 
            this.srcAlphaLabel.AutoSize = true;
            this.srcAlphaLabel.Location = new System.Drawing.Point(110, 101);
            this.srcAlphaLabel.Name = "srcAlphaLabel";
            this.srcAlphaLabel.Size = new System.Drawing.Size(74, 13);
            this.srcAlphaLabel.TabIndex = 4;
            this.srcAlphaLabel.Text = "Source Alpha:";
            // 
            // filterModeDropDown
            // 
            this.filterModeDropDown.FormattingEnabled = true;
            this.filterModeDropDown.Items.AddRange(new object[] {
            "PointSampled",
            "Bilinear",
            "Trilinear",
            "Reserved"});
            this.filterModeDropDown.Location = new System.Drawing.Point(113, 77);
            this.filterModeDropDown.Name = "filterModeDropDown";
            this.filterModeDropDown.Size = new System.Drawing.Size(121, 21);
            this.filterModeDropDown.TabIndex = 8;
            this.filterModeDropDown.SelectionChangeCommitted += new System.EventHandler(this.filterModeDropDown_SelectionChangeCommitted);
            // 
            // filterModeLabel
            // 
            this.filterModeLabel.AutoSize = true;
            this.filterModeLabel.Location = new System.Drawing.Point(110, 61);
            this.filterModeLabel.Name = "filterModeLabel";
            this.filterModeLabel.Size = new System.Drawing.Size(62, 13);
            this.filterModeLabel.TabIndex = 7;
            this.filterModeLabel.Text = "Filter Mode:";
            // 
            // specColorBox
            // 
            this.specColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.specColorBox.Location = new System.Drawing.Point(88, 35);
            this.specColorBox.Name = "specColorBox";
            this.specColorBox.Size = new System.Drawing.Size(37, 19);
            this.specColorBox.TabIndex = 6;
            this.specColorBox.TabStop = false;
            this.specColorBox.Click += new System.EventHandler(this.specColorBox_Click);
            // 
            // diffuseColorBox
            // 
            this.diffuseColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.diffuseColorBox.Location = new System.Drawing.Point(88, 11);
            this.diffuseColorBox.Name = "diffuseColorBox";
            this.diffuseColorBox.Size = new System.Drawing.Size(37, 19);
            this.diffuseColorBox.TabIndex = 4;
            this.diffuseColorBox.TabStop = false;
            this.diffuseColorBox.Click += new System.EventHandler(this.diffuseColorBox_Click);
            // 
            // exponentTextBox
            // 
            this.exponentTextBox.Location = new System.Drawing.Point(165, 35);
            this.exponentTextBox.Name = "exponentTextBox";
            this.exponentTextBox.Size = new System.Drawing.Size(54, 20);
            this.exponentTextBox.TabIndex = 4;
            this.exponentTextBox.Leave += new System.EventHandler(this.exponentTextBox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Diffuse Map:";
            // 
            // textureBox
            // 
            this.textureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textureBox.Location = new System.Drawing.Point(9, 90);
            this.textureBox.Name = "textureBox";
            this.textureBox.Size = new System.Drawing.Size(86, 86);
            this.textureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.textureBox.TabIndex = 4;
            this.textureBox.TabStop = false;
            this.textureBox.Click += new System.EventHandler(this.textureBox_Click);
            // 
            // exponentLabel
            // 
            this.exponentLabel.AutoSize = true;
            this.exponentLabel.Location = new System.Drawing.Point(131, 38);
            this.exponentLabel.Name = "exponentLabel";
            this.exponentLabel.Size = new System.Drawing.Size(28, 13);
            this.exponentLabel.TabIndex = 2;
            this.exponentLabel.Text = "Exp:";
            // 
            // specColorLabel
            // 
            this.specColorLabel.AutoSize = true;
            this.specColorLabel.Location = new System.Drawing.Point(6, 37);
            this.specColorLabel.Name = "specColorLabel";
            this.specColorLabel.Size = new System.Drawing.Size(79, 13);
            this.specColorLabel.TabIndex = 1;
            this.specColorLabel.Text = "Specular Color:";
            // 
            // diffuseLabel
            // 
            this.diffuseLabel.AutoSize = true;
            this.diffuseLabel.Location = new System.Drawing.Point(6, 17);
            this.diffuseLabel.Name = "diffuseLabel";
            this.diffuseLabel.Size = new System.Drawing.Size(70, 13);
            this.diffuseLabel.TabIndex = 0;
            this.diffuseLabel.Text = "Diffuse Color:";
            // 
            // doneButton
            // 
            this.doneButton.Location = new System.Drawing.Point(443, 217);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 4;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // MaterialEditor
            // 
            this.AcceptButton = this.doneButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 249);
            this.ControlBox = false;
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.generalSettingBox);
            this.Controls.Add(this.flagsGroupBox);
            this.Controls.Add(this.currentMaterialLabel);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MaterialEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Material Editor";
            this.Load += new System.EventHandler(this.MaterialEditor_Load);
            this.flagsGroupBox.ResumeLayout(false);
            this.flagsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userFlagsNumeric)).EndInit();
            this.generalSettingBox.ResumeLayout(false);
            this.generalSettingBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specColorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.diffuseColorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label currentMaterialLabel;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.GroupBox flagsGroupBox;
        private System.Windows.Forms.CheckBox superSampleCheck;
        private System.Windows.Forms.CheckBox pickStatusCheck;
        private System.Windows.Forms.CheckBox clampVCheck;
        private System.Windows.Forms.CheckBox clampUCheck;
        private System.Windows.Forms.CheckBox flipVCheck;
        private System.Windows.Forms.CheckBox flipUCheck;
        private System.Windows.Forms.CheckBox useAlphaCheck;
        private System.Windows.Forms.CheckBox ignoreSpecCheck;
        private System.Windows.Forms.CheckBox doubleSideCheck;
        private System.Windows.Forms.CheckBox envMapCheck;
        private System.Windows.Forms.CheckBox useTextureCheck;
        private System.Windows.Forms.CheckBox ignoreLightCheck;
        private System.Windows.Forms.CheckBox flatShadeCheck;
        private System.Windows.Forms.GroupBox generalSettingBox;
        private System.Windows.Forms.Label exponentLabel;
        private System.Windows.Forms.Label specColorLabel;
        private System.Windows.Forms.Label diffuseLabel;
        private HCPictureBox textureBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox exponentTextBox;
        private System.Windows.Forms.PictureBox diffuseColorBox;
        private System.Windows.Forms.ComboBox filterModeDropDown;
        private System.Windows.Forms.Label filterModeLabel;
        private System.Windows.Forms.PictureBox specColorBox;
        private System.Windows.Forms.Label srcAlphaLabel;
        private System.Windows.Forms.ComboBox dstAlphaCombo;
        private System.Windows.Forms.Label destinationAlphaLabel;
        private System.Windows.Forms.ComboBox srcAlphaCombo;
        private System.Windows.Forms.Label userFlagsLabel;
        private System.Windows.Forms.NumericUpDown userFlagsNumeric;
        private System.Windows.Forms.Button doneButton;
    }
}