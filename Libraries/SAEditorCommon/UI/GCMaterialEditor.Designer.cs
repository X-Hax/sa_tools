namespace SAModel.SAEditorCommon.UI
{
    partial class GCMaterialEditor
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
            this.comboMaterial = new System.Windows.Forms.ComboBox();
            this.currentMaterialLabel = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.flagsGroupBox = new System.Windows.Forms.GroupBox();
            this.labelFlags = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.userFlagsLabel = new System.Windows.Forms.Label();
            this.userFlagsNumeric = new System.Windows.Forms.NumericUpDown();
            this.ignoreLightCheck = new System.Windows.Forms.CheckBox();
            this.flatShadeCheck = new System.Windows.Forms.CheckBox();
            this.doubleSideCheck = new System.Windows.Forms.CheckBox();
            this.envMapCheck = new System.Windows.Forms.CheckBox();
            this.useTextureCheck = new System.Windows.Forms.CheckBox();
            this.flipVCheck = new System.Windows.Forms.CheckBox();
            this.flipUCheck = new System.Windows.Forms.CheckBox();
            this.clampVCheck = new System.Windows.Forms.CheckBox();
            this.clampUCheck = new System.Windows.Forms.CheckBox();
            this.superSampleCheck = new System.Windows.Forms.CheckBox();
            this.pickStatusCheck = new System.Windows.Forms.CheckBox();
            this.generalSettingBox = new System.Windows.Forms.GroupBox();
            this.labelAlpha = new System.Windows.Forms.Label();
            this.alphaDiffuseNumeric = new System.Windows.Forms.NumericUpDown();
            this.dstAlphaCombo = new System.Windows.Forms.ComboBox();
            this.diffuseColorBox = new System.Windows.Forms.Panel();
            this.destinationAlphaLabel = new System.Windows.Forms.Label();
            this.srcAlphaCombo = new System.Windows.Forms.ComboBox();
            this.filterModeLabel = new System.Windows.Forms.Label();
            this.srcAlphaLabel = new System.Windows.Forms.Label();
            this.textureBox = new System.Windows.Forms.PictureBox();
            this.filterModeDropDown = new System.Windows.Forms.ComboBox();
            this.diffuseLabel = new System.Windows.Forms.Label();
            this.doneButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.resetButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.cloneButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.flagsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userFlagsNumeric)).BeginInit();
            this.generalSettingBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alphaDiffuseNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // comboMaterial
            // 
            this.comboMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMaterial.FormattingEnabled = true;
            this.comboMaterial.Location = new System.Drawing.Point(119, 14);
            this.comboMaterial.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboMaterial.Name = "comboMaterial";
            this.comboMaterial.Size = new System.Drawing.Size(263, 23);
            this.comboMaterial.TabIndex = 1;
            this.comboMaterial.SelectedIndexChanged += new System.EventHandler(this.comboMaterial_SelectedIndexChanged);
            this.comboMaterial.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // currentMaterialLabel
            // 
            this.currentMaterialLabel.AutoSize = true;
            this.currentMaterialLabel.Location = new System.Drawing.Point(14, 17);
            this.currentMaterialLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.currentMaterialLabel.Name = "currentMaterialLabel";
            this.currentMaterialLabel.Size = new System.Drawing.Size(96, 15);
            this.currentMaterialLabel.TabIndex = 0;
            this.currentMaterialLabel.Text = "Current Material:";
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.FullOpen = true;
            this.colorDialog.SolidColorOnly = true;
            // 
            // flagsGroupBox
            // 
            this.flagsGroupBox.Controls.Add(this.labelFlags);
            this.flagsGroupBox.Controls.Add(this.label2);
            this.flagsGroupBox.Controls.Add(this.userFlagsLabel);
            this.flagsGroupBox.Controls.Add(this.userFlagsNumeric);
            this.flagsGroupBox.Controls.Add(this.ignoreLightCheck);
            this.flagsGroupBox.Controls.Add(this.flatShadeCheck);
            this.flagsGroupBox.Controls.Add(this.doubleSideCheck);
            this.flagsGroupBox.Controls.Add(this.envMapCheck);
            this.flagsGroupBox.Controls.Add(this.useTextureCheck);
            this.flagsGroupBox.Controls.Add(this.flipVCheck);
            this.flagsGroupBox.Controls.Add(this.flipUCheck);
            this.flagsGroupBox.Controls.Add(this.clampVCheck);
            this.flagsGroupBox.Controls.Add(this.clampUCheck);
            this.flagsGroupBox.Controls.Add(this.superSampleCheck);
            this.flagsGroupBox.Controls.Add(this.pickStatusCheck);
            this.flagsGroupBox.Location = new System.Drawing.Point(358, 45);
            this.flagsGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flagsGroupBox.Name = "flagsGroupBox";
            this.flagsGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flagsGroupBox.Size = new System.Drawing.Size(279, 210);
            this.flagsGroupBox.TabIndex = 4;
            this.flagsGroupBox.TabStop = false;
            this.flagsGroupBox.Text = "Flags";
            // 
            // labelFlags
            // 
            this.labelFlags.AutoSize = true;
            this.labelFlags.Location = new System.Drawing.Point(50, 155);
            this.labelFlags.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFlags.Name = "labelFlags";
            this.labelFlags.Size = new System.Drawing.Size(40, 15);
            this.labelFlags.TabIndex = 16;
            this.labelFlags.Text = "[flags]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 155);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 15;
            this.label2.Text = "Flags:";
            // 
            // userFlagsLabel
            // 
            this.userFlagsLabel.AutoSize = true;
            this.userFlagsLabel.Location = new System.Drawing.Point(130, 155);
            this.userFlagsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.userFlagsLabel.Name = "userFlagsLabel";
            this.userFlagsLabel.Size = new System.Drawing.Size(63, 15);
            this.userFlagsLabel.TabIndex = 13;
            this.userFlagsLabel.Text = "User Flags:";
            // 
            // userFlagsNumeric
            // 
            this.userFlagsNumeric.Hexadecimal = true;
            this.userFlagsNumeric.Location = new System.Drawing.Point(203, 150);
            this.userFlagsNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.userFlagsNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.userFlagsNumeric.Name = "userFlagsNumeric";
            this.userFlagsNumeric.Size = new System.Drawing.Size(69, 23);
            this.userFlagsNumeric.TabIndex = 14;
            this.userFlagsNumeric.ValueChanged += new System.EventHandler(this.userFlagsNumeric_ValueChanged);
            this.userFlagsNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // ignoreLightCheck
            // 
            this.ignoreLightCheck.AutoSize = true;
            this.ignoreLightCheck.Location = new System.Drawing.Point(121, 107);
            this.ignoreLightCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ignoreLightCheck.Name = "ignoreLightCheck";
            this.ignoreLightCheck.Size = new System.Drawing.Size(107, 19);
            this.ignoreLightCheck.TabIndex = 12;
            this.ignoreLightCheck.Text = "Ignore Lighting";
            this.toolTip.SetToolTip(this.ignoreLightCheck, "If checked, the model will not have any lighting applied.");
            this.ignoreLightCheck.UseVisualStyleBackColor = true;
            this.ignoreLightCheck.Click += new System.EventHandler(this.ignoreLightCheck_Click);
            this.ignoreLightCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // flatShadeCheck
            // 
            this.flatShadeCheck.AutoSize = true;
            this.flatShadeCheck.Location = new System.Drawing.Point(121, 87);
            this.flatShadeCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flatShadeCheck.Name = "flatShadeCheck";
            this.flatShadeCheck.Size = new System.Drawing.Size(87, 19);
            this.flatShadeCheck.TabIndex = 11;
            this.flatShadeCheck.Text = "Flat Shaded";
            this.toolTip.SetToolTip(this.flatShadeCheck, "If checked, polygon smoothing will be disabled and the model will appear faceted," +
        " like a cut gem or die.");
            this.flatShadeCheck.UseVisualStyleBackColor = true;
            this.flatShadeCheck.Click += new System.EventHandler(this.flatShadeCheck_Click);
            this.flatShadeCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // doubleSideCheck
            // 
            this.doubleSideCheck.AutoSize = true;
            this.doubleSideCheck.Location = new System.Drawing.Point(121, 65);
            this.doubleSideCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.doubleSideCheck.Name = "doubleSideCheck";
            this.doubleSideCheck.Size = new System.Drawing.Size(96, 19);
            this.doubleSideCheck.TabIndex = 10;
            this.doubleSideCheck.Text = "Double Sided";
            this.toolTip.SetToolTip(this.doubleSideCheck, "Doesn\'t do anything, since Sonic Adventure does not support backface cull.");
            this.doubleSideCheck.UseVisualStyleBackColor = true;
            this.doubleSideCheck.Click += new System.EventHandler(this.doubleSideCheck_Click);
            this.doubleSideCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // envMapCheck
            // 
            this.envMapCheck.AutoSize = true;
            this.envMapCheck.Location = new System.Drawing.Point(121, 43);
            this.envMapCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.envMapCheck.Name = "envMapCheck";
            this.envMapCheck.Size = new System.Drawing.Size(145, 19);
            this.envMapCheck.TabIndex = 9;
            this.envMapCheck.Text = "Environment Mapping";
            this.toolTip.SetToolTip(this.envMapCheck, "If checked, the texture\'s uv maps will be mapped to the environment and the model" +
        " will appear \'shiny\'.");
            this.envMapCheck.UseVisualStyleBackColor = true;
            this.envMapCheck.Click += new System.EventHandler(this.envMapCheck_Click);
            this.envMapCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // useTextureCheck
            // 
            this.useTextureCheck.AutoSize = true;
            this.useTextureCheck.Location = new System.Drawing.Point(121, 21);
            this.useTextureCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.useTextureCheck.Name = "useTextureCheck";
            this.useTextureCheck.Size = new System.Drawing.Size(86, 19);
            this.useTextureCheck.TabIndex = 8;
            this.useTextureCheck.Text = "Use Texture";
            this.toolTip.SetToolTip(this.useTextureCheck, "If checked, the texture map displayed to the left will be used. Otherwise the mod" +
        "el will be a solid color.");
            this.useTextureCheck.UseVisualStyleBackColor = true;
            this.useTextureCheck.Click += new System.EventHandler(this.useTextureCheck_Click);
            this.useTextureCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // flipVCheck
            // 
            this.flipVCheck.AutoSize = true;
            this.flipVCheck.Location = new System.Drawing.Point(7, 129);
            this.flipVCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flipVCheck.Name = "flipVCheck";
            this.flipVCheck.Size = new System.Drawing.Size(69, 19);
            this.flipVCheck.TabIndex = 5;
            this.flipVCheck.Text = "Mirror V";
            this.toolTip.SetToolTip(this.flipVCheck, "If checked, tiling on the V Axis is mirrored.");
            this.flipVCheck.UseVisualStyleBackColor = true;
            this.flipVCheck.Click += new System.EventHandler(this.flipVCheck_Click);
            this.flipVCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // flipUCheck
            // 
            this.flipUCheck.AutoSize = true;
            this.flipUCheck.Location = new System.Drawing.Point(7, 107);
            this.flipUCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flipUCheck.Name = "flipUCheck";
            this.flipUCheck.Size = new System.Drawing.Size(70, 19);
            this.flipUCheck.TabIndex = 4;
            this.flipUCheck.Text = "Mirror U";
            this.toolTip.SetToolTip(this.flipUCheck, "If checked, tiling on the U Axis is mirrored.");
            this.flipUCheck.UseVisualStyleBackColor = true;
            this.flipUCheck.Click += new System.EventHandler(this.flipUCheck_Click);
            this.flipUCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // clampVCheck
            // 
            this.clampVCheck.AutoSize = true;
            this.clampVCheck.Location = new System.Drawing.Point(7, 87);
            this.clampVCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clampVCheck.Name = "clampVCheck";
            this.clampVCheck.Size = new System.Drawing.Size(71, 19);
            this.clampVCheck.TabIndex = 3;
            this.clampVCheck.Text = "Clamp V";
            this.toolTip.SetToolTip(this.clampVCheck, "Enable/Disable tiling on the V Axis.");
            this.clampVCheck.UseVisualStyleBackColor = true;
            this.clampVCheck.Click += new System.EventHandler(this.clampVCheck_Click);
            this.clampVCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // clampUCheck
            // 
            this.clampUCheck.AutoSize = true;
            this.clampUCheck.Location = new System.Drawing.Point(7, 65);
            this.clampUCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clampUCheck.Name = "clampUCheck";
            this.clampUCheck.Size = new System.Drawing.Size(72, 19);
            this.clampUCheck.TabIndex = 2;
            this.clampUCheck.Text = "Clamp U";
            this.toolTip.SetToolTip(this.clampUCheck, "Enable/Disable tiling on the U Axis.");
            this.clampUCheck.UseVisualStyleBackColor = true;
            this.clampUCheck.Click += new System.EventHandler(this.clampUCheck_Click);
            this.clampUCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // superSampleCheck
            // 
            this.superSampleCheck.AutoSize = true;
            this.superSampleCheck.Location = new System.Drawing.Point(7, 43);
            this.superSampleCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.superSampleCheck.Name = "superSampleCheck";
            this.superSampleCheck.Size = new System.Drawing.Size(98, 19);
            this.superSampleCheck.TabIndex = 1;
            this.superSampleCheck.Text = "Super Sample";
            this.superSampleCheck.UseVisualStyleBackColor = true;
            this.superSampleCheck.Click += new System.EventHandler(this.superSampleCheck_Click);
            this.superSampleCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // pickStatusCheck
            // 
            this.pickStatusCheck.AutoSize = true;
            this.pickStatusCheck.Location = new System.Drawing.Point(7, 22);
            this.pickStatusCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pickStatusCheck.Name = "pickStatusCheck";
            this.pickStatusCheck.Size = new System.Drawing.Size(83, 19);
            this.pickStatusCheck.TabIndex = 0;
            this.pickStatusCheck.Text = "Pick Status";
            this.pickStatusCheck.UseVisualStyleBackColor = true;
            this.pickStatusCheck.Click += new System.EventHandler(this.pickStatusCheck_Click);
            this.pickStatusCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // generalSettingBox
            // 
            this.generalSettingBox.Controls.Add(this.labelAlpha);
            this.generalSettingBox.Controls.Add(this.alphaDiffuseNumeric);
            this.generalSettingBox.Controls.Add(this.dstAlphaCombo);
            this.generalSettingBox.Controls.Add(this.diffuseColorBox);
            this.generalSettingBox.Controls.Add(this.destinationAlphaLabel);
            this.generalSettingBox.Controls.Add(this.srcAlphaCombo);
            this.generalSettingBox.Controls.Add(this.filterModeLabel);
            this.generalSettingBox.Controls.Add(this.srcAlphaLabel);
            this.generalSettingBox.Controls.Add(this.textureBox);
            this.generalSettingBox.Controls.Add(this.filterModeDropDown);
            this.generalSettingBox.Controls.Add(this.diffuseLabel);
            this.generalSettingBox.Location = new System.Drawing.Point(14, 45);
            this.generalSettingBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.generalSettingBox.Name = "generalSettingBox";
            this.generalSettingBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.generalSettingBox.Size = new System.Drawing.Size(337, 257);
            this.generalSettingBox.TabIndex = 3;
            this.generalSettingBox.TabStop = false;
            this.generalSettingBox.Text = "General";
            // 
            // labelAlpha
            // 
            this.labelAlpha.AutoSize = true;
            this.labelAlpha.Location = new System.Drawing.Point(164, 25);
            this.labelAlpha.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAlpha.Name = "labelAlpha";
            this.labelAlpha.Size = new System.Drawing.Size(41, 15);
            this.labelAlpha.TabIndex = 2;
            this.labelAlpha.Text = "Alpha:";
            // 
            // alphaDiffuseNumeric
            // 
            this.alphaDiffuseNumeric.Location = new System.Drawing.Point(215, 22);
            this.alphaDiffuseNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.alphaDiffuseNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.alphaDiffuseNumeric.Name = "alphaDiffuseNumeric";
            this.alphaDiffuseNumeric.Size = new System.Drawing.Size(63, 23);
            this.alphaDiffuseNumeric.TabIndex = 3;
            this.alphaDiffuseNumeric.ValueChanged += new System.EventHandler(this.alphaDiffuseNumeric_ValueChanged);
            this.alphaDiffuseNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // dstAlphaCombo
            // 
            this.dstAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            this.dstAlphaCombo.Location = new System.Drawing.Point(162, 217);
            this.dstAlphaCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dstAlphaCombo.Name = "dstAlphaCombo";
            this.dstAlphaCombo.Size = new System.Drawing.Size(164, 23);
            this.dstAlphaCombo.TabIndex = 14;
            this.dstAlphaCombo.SelectedIndexChanged += new System.EventHandler(this.dstAlphaCombo_SelectedIndexChanged);
            this.dstAlphaCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // diffuseColorBox
            // 
            this.diffuseColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.diffuseColorBox.Location = new System.Drawing.Point(105, 22);
            this.diffuseColorBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.diffuseColorBox.Name = "diffuseColorBox";
            this.diffuseColorBox.Size = new System.Drawing.Size(43, 22);
            this.diffuseColorBox.TabIndex = 1;
            this.toolTip.SetToolTip(this.diffuseColorBox, "Diffuse lighting is scattered as opposed to direct. Specifically, this \'diffuse c" +
        "olor\' will act as a tint to the model.");
            this.diffuseColorBox.Click += new System.EventHandler(this.diffuseColorBox_Click);
            // 
            // destinationAlphaLabel
            // 
            this.destinationAlphaLabel.AutoSize = true;
            this.destinationAlphaLabel.Location = new System.Drawing.Point(159, 195);
            this.destinationAlphaLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.destinationAlphaLabel.Name = "destinationAlphaLabel";
            this.destinationAlphaLabel.Size = new System.Drawing.Size(104, 15);
            this.destinationAlphaLabel.TabIndex = 13;
            this.destinationAlphaLabel.Text = "Destination Alpha:";
            // 
            // srcAlphaCombo
            // 
            this.srcAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            this.srcAlphaCombo.Location = new System.Drawing.Point(162, 167);
            this.srcAlphaCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.srcAlphaCombo.Name = "srcAlphaCombo";
            this.srcAlphaCombo.Size = new System.Drawing.Size(164, 23);
            this.srcAlphaCombo.TabIndex = 12;
            this.srcAlphaCombo.SelectionChangeCommitted += new System.EventHandler(this.srcAlphaCombo_SelectionChangeCommitted);
            this.srcAlphaCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // filterModeLabel
            // 
            this.filterModeLabel.AutoSize = true;
            this.filterModeLabel.Location = new System.Drawing.Point(162, 103);
            this.filterModeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.filterModeLabel.Name = "filterModeLabel";
            this.filterModeLabel.Size = new System.Drawing.Size(70, 15);
            this.filterModeLabel.TabIndex = 9;
            this.filterModeLabel.Text = "Filter Mode:";
            // 
            // srcAlphaLabel
            // 
            this.srcAlphaLabel.AutoSize = true;
            this.srcAlphaLabel.Location = new System.Drawing.Point(159, 149);
            this.srcAlphaLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srcAlphaLabel.Name = "srcAlphaLabel";
            this.srcAlphaLabel.Size = new System.Drawing.Size(80, 15);
            this.srcAlphaLabel.TabIndex = 11;
            this.srcAlphaLabel.Text = "Source Alpha:";
            // 
            // textureBox
            // 
            this.textureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textureBox.Location = new System.Drawing.Point(9, 111);
            this.textureBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textureBox.Name = "textureBox";
            this.textureBox.Size = new System.Drawing.Size(135, 130);
            this.textureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.textureBox.TabIndex = 4;
            this.textureBox.TabStop = false;
            this.textureBox.Click += new System.EventHandler(this.textureBox_Click);
            // 
            // filterModeDropDown
            // 
            this.filterModeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterModeDropDown.FormattingEnabled = true;
            this.filterModeDropDown.Items.AddRange(new object[] {
            "PointSampled",
            "Bilinear",
            "Trilinear",
            "Reserved"});
            this.filterModeDropDown.Location = new System.Drawing.Point(162, 121);
            this.filterModeDropDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filterModeDropDown.Name = "filterModeDropDown";
            this.filterModeDropDown.Size = new System.Drawing.Size(164, 23);
            this.filterModeDropDown.TabIndex = 10;
            this.filterModeDropDown.SelectionChangeCommitted += new System.EventHandler(this.filterModeDropDown_SelectionChangeCommitted);
            this.filterModeDropDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // diffuseLabel
            // 
            this.diffuseLabel.AutoSize = true;
            this.diffuseLabel.Location = new System.Drawing.Point(16, 25);
            this.diffuseLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.diffuseLabel.Name = "diffuseLabel";
            this.diffuseLabel.Size = new System.Drawing.Size(79, 15);
            this.diffuseLabel.TabIndex = 0;
            this.diffuseLabel.Text = "Diffuse Color:";
            // 
            // doneButton
            // 
            this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.doneButton.Location = new System.Drawing.Point(550, 270);
            this.doneButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(88, 27);
            this.doneButton.TabIndex = 0;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            this.doneButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 30000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(573, 13);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(57, 23);
            this.resetButton.TabIndex = 11;
            this.resetButton.Text = "Reset";
            this.toolTip.SetToolTip(this.resetButton, "Reset the material list to the state it was when this dialog opened.");
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(510, 13);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(57, 23);
            this.deleteButton.TabIndex = 10;
            this.deleteButton.Text = "Delete";
            this.toolTip.SetToolTip(this.deleteButton, "Delete the material.");
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // cloneButton
            // 
            this.cloneButton.Location = new System.Drawing.Point(447, 13);
            this.cloneButton.Name = "cloneButton";
            this.cloneButton.Size = new System.Drawing.Size(57, 23);
            this.cloneButton.TabIndex = 9;
            this.cloneButton.Text = "Clone";
            this.toolTip.SetToolTip(this.cloneButton, "Create an identical copy of the material.");
            this.cloneButton.UseVisualStyleBackColor = true;
            this.cloneButton.Click += new System.EventHandler(this.cloneButton_Click);
            // 
            // downButton
            // 
            this.downButton.Location = new System.Drawing.Point(418, 13);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(23, 23);
            this.downButton.TabIndex = 8;
            this.downButton.Text = "↓";
            this.toolTip.SetToolTip(this.downButton, "Move the material down on the material list.");
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // upButton
            // 
            this.upButton.Enabled = false;
            this.upButton.Location = new System.Drawing.Point(389, 13);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(23, 23);
            this.upButton.TabIndex = 7;
            this.upButton.Text = "↑";
            this.toolTip.SetToolTip(this.upButton, "Move the material up on the material list.");
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // GCMaterialEditor
            // 
            this.AcceptButton = this.doneButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(648, 310);
            this.ControlBox = false;
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.cloneButton);
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.generalSettingBox);
            this.Controls.Add(this.flagsGroupBox);
            this.Controls.Add(this.currentMaterialLabel);
            this.Controls.Add(this.comboMaterial);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GCMaterialEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Material Editor";
            this.Load += new System.EventHandler(this.MaterialEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.onKeyDown);
            this.flagsGroupBox.ResumeLayout(false);
            this.flagsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userFlagsNumeric)).EndInit();
            this.generalSettingBox.ResumeLayout(false);
            this.generalSettingBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alphaDiffuseNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboMaterial;
        private System.Windows.Forms.Label currentMaterialLabel;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.GroupBox flagsGroupBox;
        private System.Windows.Forms.CheckBox superSampleCheck;
        private System.Windows.Forms.CheckBox pickStatusCheck;
        private System.Windows.Forms.CheckBox clampVCheck;
        private System.Windows.Forms.CheckBox clampUCheck;
        private System.Windows.Forms.CheckBox flipVCheck;
        private System.Windows.Forms.CheckBox flipUCheck;
        private System.Windows.Forms.CheckBox doubleSideCheck;
        private System.Windows.Forms.CheckBox envMapCheck;
        private System.Windows.Forms.CheckBox useTextureCheck;
        private System.Windows.Forms.CheckBox ignoreLightCheck;
        private System.Windows.Forms.CheckBox flatShadeCheck;
        private System.Windows.Forms.GroupBox generalSettingBox;
        private System.Windows.Forms.Label diffuseLabel;
        private System.Windows.Forms.PictureBox textureBox;
        private System.Windows.Forms.Panel diffuseColorBox;
        private System.Windows.Forms.ComboBox filterModeDropDown;
        private System.Windows.Forms.Label filterModeLabel;
        private System.Windows.Forms.Label srcAlphaLabel;
        private System.Windows.Forms.ComboBox dstAlphaCombo;
        private System.Windows.Forms.Label destinationAlphaLabel;
        private System.Windows.Forms.ComboBox srcAlphaCombo;
        private System.Windows.Forms.Label userFlagsLabel;
        private System.Windows.Forms.NumericUpDown userFlagsNumeric;
        private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.NumericUpDown alphaDiffuseNumeric;
		private System.Windows.Forms.Label labelAlpha;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label labelFlags;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button cloneButton;
		private System.Windows.Forms.Button downButton;
		private System.Windows.Forms.Button upButton;
	}
}