namespace SAModel.SAEditorCommon.UI
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
			components = new System.ComponentModel.Container();
			comboMaterial = new System.Windows.Forms.ComboBox();
			currentMaterialLabel = new System.Windows.Forms.Label();
			colorDialog = new System.Windows.Forms.ColorDialog();
			flagsGroupBox = new System.Windows.Forms.GroupBox();
			labelFlags = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			userFlagsLabel = new System.Windows.Forms.Label();
			userFlagsNumeric = new System.Windows.Forms.NumericUpDown();
			ignoreLightCheck = new System.Windows.Forms.CheckBox();
			flatShadeCheck = new System.Windows.Forms.CheckBox();
			doubleSideCheck = new System.Windows.Forms.CheckBox();
			envMapCheck = new System.Windows.Forms.CheckBox();
			useTextureCheck = new System.Windows.Forms.CheckBox();
			useAlphaCheck = new System.Windows.Forms.CheckBox();
			ignoreSpecCheck = new System.Windows.Forms.CheckBox();
			flipVCheck = new System.Windows.Forms.CheckBox();
			flipUCheck = new System.Windows.Forms.CheckBox();
			clampVCheck = new System.Windows.Forms.CheckBox();
			clampUCheck = new System.Windows.Forms.CheckBox();
			superSampleCheck = new System.Windows.Forms.CheckBox();
			pickStatusCheck = new System.Windows.Forms.CheckBox();
			generalSettingBox = new System.Windows.Forms.GroupBox();
			alphaSpecularNumeric = new System.Windows.Forms.NumericUpDown();
			label3 = new System.Windows.Forms.Label();
			labelAlpha = new System.Windows.Forms.Label();
			alphaDiffuseNumeric = new System.Windows.Forms.NumericUpDown();
			dstAlphaCombo = new System.Windows.Forms.ComboBox();
			specColorBox = new System.Windows.Forms.Panel();
			diffuseColorBox = new System.Windows.Forms.Panel();
			destinationAlphaLabel = new System.Windows.Forms.Label();
			exponentTextBox = new System.Windows.Forms.TextBox();
			srcAlphaCombo = new System.Windows.Forms.ComboBox();
			filterModeLabel = new System.Windows.Forms.Label();
			srcAlphaLabel = new System.Windows.Forms.Label();
			textureBox = new System.Windows.Forms.PictureBox();
			filterModeDropDown = new System.Windows.Forms.ComboBox();
			exponentLabel = new System.Windows.Forms.Label();
			specColorLabel = new System.Windows.Forms.Label();
			diffuseLabel = new System.Windows.Forms.Label();
			doneButton = new System.Windows.Forms.Button();
			toolTip = new System.Windows.Forms.ToolTip(components);
			upButton = new System.Windows.Forms.Button();
			downButton = new System.Windows.Forms.Button();
			cloneButton = new System.Windows.Forms.Button();
			deleteButton = new System.Windows.Forms.Button();
			resetButton = new System.Windows.Forms.Button();
			labelTexID = new System.Windows.Forms.Label();
			numericUpDownTexID = new System.Windows.Forms.NumericUpDown();
			flagsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)userFlagsNumeric).BeginInit();
			generalSettingBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)alphaSpecularNumeric).BeginInit();
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).BeginInit();
			((System.ComponentModel.ISupportInitialize)textureBox).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownTexID).BeginInit();
			SuspendLayout();
			// 
			// comboMaterial
			// 
			comboMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboMaterial.FormattingEnabled = true;
			comboMaterial.Location = new System.Drawing.Point(119, 14);
			comboMaterial.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			comboMaterial.Name = "comboMaterial";
			comboMaterial.Size = new System.Drawing.Size(271, 23);
			comboMaterial.TabIndex = 1;
			comboMaterial.SelectedIndexChanged += comboMaterial_SelectedIndexChanged;
			comboMaterial.KeyDown += onKeyDown;
			// 
			// currentMaterialLabel
			// 
			currentMaterialLabel.AutoSize = true;
			currentMaterialLabel.Location = new System.Drawing.Point(14, 17);
			currentMaterialLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			currentMaterialLabel.Name = "currentMaterialLabel";
			currentMaterialLabel.Size = new System.Drawing.Size(96, 15);
			currentMaterialLabel.TabIndex = 0;
			currentMaterialLabel.Text = "Current Material:";
			// 
			// colorDialog
			// 
			colorDialog.AnyColor = true;
			colorDialog.FullOpen = true;
			colorDialog.SolidColorOnly = true;
			// 
			// flagsGroupBox
			// 
			flagsGroupBox.Controls.Add(labelFlags);
			flagsGroupBox.Controls.Add(label2);
			flagsGroupBox.Controls.Add(userFlagsLabel);
			flagsGroupBox.Controls.Add(userFlagsNumeric);
			flagsGroupBox.Controls.Add(ignoreLightCheck);
			flagsGroupBox.Controls.Add(flatShadeCheck);
			flagsGroupBox.Controls.Add(doubleSideCheck);
			flagsGroupBox.Controls.Add(envMapCheck);
			flagsGroupBox.Controls.Add(useTextureCheck);
			flagsGroupBox.Controls.Add(useAlphaCheck);
			flagsGroupBox.Controls.Add(ignoreSpecCheck);
			flagsGroupBox.Controls.Add(flipVCheck);
			flagsGroupBox.Controls.Add(flipUCheck);
			flagsGroupBox.Controls.Add(clampVCheck);
			flagsGroupBox.Controls.Add(clampUCheck);
			flagsGroupBox.Controls.Add(superSampleCheck);
			flagsGroupBox.Controls.Add(pickStatusCheck);
			flagsGroupBox.Location = new System.Drawing.Point(358, 45);
			flagsGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flagsGroupBox.Name = "flagsGroupBox";
			flagsGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flagsGroupBox.Size = new System.Drawing.Size(279, 210);
			flagsGroupBox.TabIndex = 8;
			flagsGroupBox.TabStop = false;
			flagsGroupBox.Text = "Flags";
			// 
			// labelFlags
			// 
			labelFlags.AutoSize = true;
			labelFlags.Location = new System.Drawing.Point(55, 180);
			labelFlags.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelFlags.Name = "labelFlags";
			labelFlags.Size = new System.Drawing.Size(40, 15);
			labelFlags.TabIndex = 16;
			labelFlags.Text = "[flags]";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(7, 180);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(37, 15);
			label2.TabIndex = 15;
			label2.Text = "Flags:";
			// 
			// userFlagsLabel
			// 
			userFlagsLabel.AutoSize = true;
			userFlagsLabel.Location = new System.Drawing.Point(130, 155);
			userFlagsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			userFlagsLabel.Name = "userFlagsLabel";
			userFlagsLabel.Size = new System.Drawing.Size(63, 15);
			userFlagsLabel.TabIndex = 13;
			userFlagsLabel.Text = "User Flags:";
			// 
			// userFlagsNumeric
			// 
			userFlagsNumeric.Hexadecimal = true;
			userFlagsNumeric.Location = new System.Drawing.Point(203, 150);
			userFlagsNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			userFlagsNumeric.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			userFlagsNumeric.Name = "userFlagsNumeric";
			userFlagsNumeric.Size = new System.Drawing.Size(69, 23);
			userFlagsNumeric.TabIndex = 13;
			userFlagsNumeric.ValueChanged += userFlagsNumeric_ValueChanged;
			userFlagsNumeric.KeyDown += onKeyDown;
			// 
			// ignoreLightCheck
			// 
			ignoreLightCheck.AutoSize = true;
			ignoreLightCheck.Location = new System.Drawing.Point(121, 106);
			ignoreLightCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ignoreLightCheck.Name = "ignoreLightCheck";
			ignoreLightCheck.Size = new System.Drawing.Size(107, 19);
			ignoreLightCheck.TabIndex = 11;
			ignoreLightCheck.Text = "Ignore Lighting";
			toolTip.SetToolTip(ignoreLightCheck, "If checked, the mesh will not have any lighting applied.");
			ignoreLightCheck.UseVisualStyleBackColor = true;
			ignoreLightCheck.Click += ignoreLightCheck_Click;
			ignoreLightCheck.KeyDown += onKeyDown;
			// 
			// flatShadeCheck
			// 
			flatShadeCheck.AutoSize = true;
			flatShadeCheck.Location = new System.Drawing.Point(121, 85);
			flatShadeCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flatShadeCheck.Name = "flatShadeCheck";
			flatShadeCheck.Size = new System.Drawing.Size(87, 19);
			flatShadeCheck.TabIndex = 10;
			flatShadeCheck.Text = "Flat Shaded";
			toolTip.SetToolTip(flatShadeCheck, "If checked, polygon smoothing will be disabled and the model will appear faceted, like a cut gem or die. This flag does nothing in SADX.");
			flatShadeCheck.UseVisualStyleBackColor = true;
			flatShadeCheck.Click += flatShadeCheck_Click;
			flatShadeCheck.KeyDown += onKeyDown;
			// 
			// doubleSideCheck
			// 
			doubleSideCheck.AutoSize = true;
			doubleSideCheck.Location = new System.Drawing.Point(121, 64);
			doubleSideCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			doubleSideCheck.Name = "doubleSideCheck";
			doubleSideCheck.Size = new System.Drawing.Size(96, 19);
			doubleSideCheck.TabIndex = 9;
			doubleSideCheck.Text = "Double Sided";
			toolTip.SetToolTip(doubleSideCheck, "Doesn't do anything, since Sonic Adventure does not support backface cull.");
			doubleSideCheck.UseVisualStyleBackColor = true;
			doubleSideCheck.Click += doubleSideCheck_Click;
			doubleSideCheck.KeyDown += onKeyDown;
			// 
			// envMapCheck
			// 
			envMapCheck.AutoSize = true;
			envMapCheck.Location = new System.Drawing.Point(121, 43);
			envMapCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			envMapCheck.Name = "envMapCheck";
			envMapCheck.Size = new System.Drawing.Size(145, 19);
			envMapCheck.TabIndex = 8;
			envMapCheck.Text = "Environment Mapping";
			toolTip.SetToolTip(envMapCheck, "If checked, the texture's uv maps will be mapped to the environment and the model will appear 'shiny'.");
			envMapCheck.UseVisualStyleBackColor = true;
			envMapCheck.Click += envMapCheck_Click;
			envMapCheck.KeyDown += onKeyDown;
			// 
			// useTextureCheck
			// 
			useTextureCheck.AutoSize = true;
			useTextureCheck.Location = new System.Drawing.Point(7, 22);
			useTextureCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			useTextureCheck.Name = "useTextureCheck";
			useTextureCheck.Size = new System.Drawing.Size(86, 19);
			useTextureCheck.TabIndex = 0;
			useTextureCheck.Text = "Use Texture";
			toolTip.SetToolTip(useTextureCheck, "If checked, the texture map displayed to the left will be used. Otherwise the model will be a solid color.");
			useTextureCheck.UseVisualStyleBackColor = true;
			useTextureCheck.Click += useTextureCheck_Click;
			useTextureCheck.KeyDown += onKeyDown;
			// 
			// useAlphaCheck
			// 
			useAlphaCheck.AutoSize = true;
			useAlphaCheck.Location = new System.Drawing.Point(121, 22);
			useAlphaCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			useAlphaCheck.Name = "useAlphaCheck";
			useAlphaCheck.Size = new System.Drawing.Size(79, 19);
			useAlphaCheck.TabIndex = 7;
			useAlphaCheck.Text = "Use Alpha";
			toolTip.SetToolTip(useAlphaCheck, "If checked, texture transparency will be enabled (and possibly non-texture transparency). ");
			useAlphaCheck.UseVisualStyleBackColor = true;
			useAlphaCheck.Click += useAlphaCheck_Click;
			useAlphaCheck.KeyDown += onKeyDown;
			// 
			// ignoreSpecCheck
			// 
			ignoreSpecCheck.AutoSize = true;
			ignoreSpecCheck.Location = new System.Drawing.Point(121, 127);
			ignoreSpecCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ignoreSpecCheck.Name = "ignoreSpecCheck";
			ignoreSpecCheck.Size = new System.Drawing.Size(108, 19);
			ignoreSpecCheck.TabIndex = 12;
			ignoreSpecCheck.Text = "Ignore Specular";
			toolTip.SetToolTip(ignoreSpecCheck, "Disables specular lighting on the material. This flag does nothing in SADX. In SA1 DC it is used for specular palette selection.");
			ignoreSpecCheck.UseVisualStyleBackColor = true;
			ignoreSpecCheck.Click += ignoreSpecCheck_Click;
			ignoreSpecCheck.KeyDown += onKeyDown;
			// 
			// flipVCheck
			// 
			flipVCheck.AutoSize = true;
			flipVCheck.Location = new System.Drawing.Point(7, 148);
			flipVCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flipVCheck.Name = "flipVCheck";
			flipVCheck.Size = new System.Drawing.Size(69, 19);
			flipVCheck.TabIndex = 6;
			flipVCheck.Text = "Mirror V";
			toolTip.SetToolTip(flipVCheck, "If checked, tiling on the V Axis is mirrored.");
			flipVCheck.UseVisualStyleBackColor = true;
			flipVCheck.Click += flipVCheck_Click;
			flipVCheck.KeyDown += onKeyDown;
			// 
			// flipUCheck
			// 
			flipUCheck.AutoSize = true;
			flipUCheck.Location = new System.Drawing.Point(7, 127);
			flipUCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flipUCheck.Name = "flipUCheck";
			flipUCheck.Size = new System.Drawing.Size(70, 19);
			flipUCheck.TabIndex = 5;
			flipUCheck.Text = "Mirror U";
			toolTip.SetToolTip(flipUCheck, "If checked, tiling on the U Axis is mirrored.");
			flipUCheck.UseVisualStyleBackColor = true;
			flipUCheck.Click += flipUCheck_Click;
			flipUCheck.KeyDown += onKeyDown;
			// 
			// clampVCheck
			// 
			clampVCheck.AutoSize = true;
			clampVCheck.Location = new System.Drawing.Point(7, 106);
			clampVCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			clampVCheck.Name = "clampVCheck";
			clampVCheck.Size = new System.Drawing.Size(71, 19);
			clampVCheck.TabIndex = 4;
			clampVCheck.Text = "Clamp V";
			toolTip.SetToolTip(clampVCheck, "Enable/Disable tiling on the V Axis.");
			clampVCheck.UseVisualStyleBackColor = true;
			clampVCheck.Click += clampVCheck_Click;
			clampVCheck.KeyDown += onKeyDown;
			// 
			// clampUCheck
			// 
			clampUCheck.AutoSize = true;
			clampUCheck.Location = new System.Drawing.Point(7, 85);
			clampUCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			clampUCheck.Name = "clampUCheck";
			clampUCheck.Size = new System.Drawing.Size(72, 19);
			clampUCheck.TabIndex = 3;
			clampUCheck.Text = "Clamp U";
			toolTip.SetToolTip(clampUCheck, "Enable/Disable tiling on the U Axis.");
			clampUCheck.UseVisualStyleBackColor = true;
			clampUCheck.Click += clampUCheck_Click;
			clampUCheck.KeyDown += onKeyDown;
			// 
			// superSampleCheck
			// 
			superSampleCheck.AutoSize = true;
			superSampleCheck.Location = new System.Drawing.Point(7, 64);
			superSampleCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			superSampleCheck.Name = "superSampleCheck";
			superSampleCheck.Size = new System.Drawing.Size(98, 19);
			superSampleCheck.TabIndex = 2;
			superSampleCheck.Text = "Super Sample";
			superSampleCheck.UseVisualStyleBackColor = true;
			superSampleCheck.Click += superSampleCheck_Click;
			superSampleCheck.KeyDown += onKeyDown;
			// 
			// pickStatusCheck
			// 
			pickStatusCheck.AutoSize = true;
			pickStatusCheck.Location = new System.Drawing.Point(7, 43);
			pickStatusCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			pickStatusCheck.Name = "pickStatusCheck";
			pickStatusCheck.Size = new System.Drawing.Size(83, 19);
			pickStatusCheck.TabIndex = 1;
			pickStatusCheck.Text = "Pick Status";
			pickStatusCheck.UseVisualStyleBackColor = true;
			pickStatusCheck.Click += pickStatusCheck_Click;
			pickStatusCheck.KeyDown += onKeyDown;
			// 
			// generalSettingBox
			// 
			generalSettingBox.Controls.Add(numericUpDownTexID);
			generalSettingBox.Controls.Add(labelTexID);
			generalSettingBox.Controls.Add(alphaSpecularNumeric);
			generalSettingBox.Controls.Add(label3);
			generalSettingBox.Controls.Add(labelAlpha);
			generalSettingBox.Controls.Add(alphaDiffuseNumeric);
			generalSettingBox.Controls.Add(dstAlphaCombo);
			generalSettingBox.Controls.Add(specColorBox);
			generalSettingBox.Controls.Add(diffuseColorBox);
			generalSettingBox.Controls.Add(destinationAlphaLabel);
			generalSettingBox.Controls.Add(exponentTextBox);
			generalSettingBox.Controls.Add(srcAlphaCombo);
			generalSettingBox.Controls.Add(filterModeLabel);
			generalSettingBox.Controls.Add(srcAlphaLabel);
			generalSettingBox.Controls.Add(textureBox);
			generalSettingBox.Controls.Add(filterModeDropDown);
			generalSettingBox.Controls.Add(exponentLabel);
			generalSettingBox.Controls.Add(specColorLabel);
			generalSettingBox.Controls.Add(diffuseLabel);
			generalSettingBox.Location = new System.Drawing.Point(14, 45);
			generalSettingBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			generalSettingBox.Name = "generalSettingBox";
			generalSettingBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			generalSettingBox.Size = new System.Drawing.Size(337, 257);
			generalSettingBox.TabIndex = 7;
			generalSettingBox.TabStop = false;
			generalSettingBox.Text = "General";
			// 
			// alphaSpecularNumeric
			// 
			alphaSpecularNumeric.Location = new System.Drawing.Point(215, 51);
			alphaSpecularNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			alphaSpecularNumeric.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			alphaSpecularNumeric.Name = "alphaSpecularNumeric";
			alphaSpecularNumeric.Size = new System.Drawing.Size(63, 23);
			alphaSpecularNumeric.TabIndex = 4;
			alphaSpecularNumeric.ValueChanged += alphaSpecularNumeric_ValueChanged;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(164, 54);
			label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(41, 15);
			label3.TabIndex = 15;
			label3.Text = "Alpha:";
			// 
			// labelAlpha
			// 
			labelAlpha.AutoSize = true;
			labelAlpha.Location = new System.Drawing.Point(164, 25);
			labelAlpha.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelAlpha.Name = "labelAlpha";
			labelAlpha.Size = new System.Drawing.Size(41, 15);
			labelAlpha.TabIndex = 2;
			labelAlpha.Text = "Alpha:";
			// 
			// alphaDiffuseNumeric
			// 
			alphaDiffuseNumeric.Location = new System.Drawing.Point(215, 22);
			alphaDiffuseNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			alphaDiffuseNumeric.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			alphaDiffuseNumeric.Name = "alphaDiffuseNumeric";
			alphaDiffuseNumeric.Size = new System.Drawing.Size(63, 23);
			alphaDiffuseNumeric.TabIndex = 2;
			alphaDiffuseNumeric.ValueChanged += alphaDiffuseNumeric_ValueChanged;
			alphaDiffuseNumeric.KeyDown += onKeyDown;
			// 
			// dstAlphaCombo
			// 
			dstAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			dstAlphaCombo.FormattingEnabled = true;
			dstAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "OtherColor", "InverseOtherColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			dstAlphaCombo.Location = new System.Drawing.Point(162, 217);
			dstAlphaCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			dstAlphaCombo.Name = "dstAlphaCombo";
			dstAlphaCombo.Size = new System.Drawing.Size(164, 23);
			dstAlphaCombo.TabIndex = 9;
			dstAlphaCombo.SelectedIndexChanged += dstAlphaCombo_SelectedIndexChanged;
			dstAlphaCombo.KeyDown += onKeyDown;
			// 
			// specColorBox
			// 
			specColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			specColorBox.Location = new System.Drawing.Point(102, 51);
			specColorBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			specColorBox.Name = "specColorBox";
			specColorBox.Size = new System.Drawing.Size(43, 22);
			specColorBox.TabIndex = 3;
			specColorBox.TabStop = true;
			toolTip.SetToolTip(specColorBox, "Specular reflection is the mirror-like reflection of light from a surface. This specular color will tint the apparent highlights on the model.");
			specColorBox.Click += specColorBox_Click;
			// 
			// diffuseColorBox
			// 
			diffuseColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			diffuseColorBox.Location = new System.Drawing.Point(102, 22);
			diffuseColorBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			diffuseColorBox.Name = "diffuseColorBox";
			diffuseColorBox.Size = new System.Drawing.Size(43, 22);
			diffuseColorBox.TabIndex = 1;
			diffuseColorBox.TabStop = true;
			toolTip.SetToolTip(diffuseColorBox, "Diffuse lighting is scattered as opposed to direct. Specifically, this 'diffuse color' will act as a tint to the model.");
			diffuseColorBox.Click += diffuseColorBox_Click;
			// 
			// destinationAlphaLabel
			// 
			destinationAlphaLabel.AutoSize = true;
			destinationAlphaLabel.Location = new System.Drawing.Point(159, 195);
			destinationAlphaLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			destinationAlphaLabel.Name = "destinationAlphaLabel";
			destinationAlphaLabel.Size = new System.Drawing.Size(104, 15);
			destinationAlphaLabel.TabIndex = 13;
			destinationAlphaLabel.Text = "Destination Alpha:";
			// 
			// exponentTextBox
			// 
			exponentTextBox.Location = new System.Drawing.Point(102, 80);
			exponentTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			exponentTextBox.Name = "exponentTextBox";
			exponentTextBox.Size = new System.Drawing.Size(42, 23);
			exponentTextBox.TabIndex = 5;
			exponentTextBox.KeyDown += onKeyDown;
			exponentTextBox.Leave += exponentTextBox_Leave;
			// 
			// srcAlphaCombo
			// 
			srcAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			srcAlphaCombo.FormattingEnabled = true;
			srcAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "OtherColor", "InverseOtherColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			srcAlphaCombo.Location = new System.Drawing.Point(162, 167);
			srcAlphaCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			srcAlphaCombo.Name = "srcAlphaCombo";
			srcAlphaCombo.Size = new System.Drawing.Size(164, 23);
			srcAlphaCombo.TabIndex = 8;
			srcAlphaCombo.SelectionChangeCommitted += srcAlphaCombo_SelectionChangeCommitted;
			srcAlphaCombo.KeyDown += onKeyDown;
			// 
			// filterModeLabel
			// 
			filterModeLabel.AutoSize = true;
			filterModeLabel.Location = new System.Drawing.Point(162, 103);
			filterModeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			filterModeLabel.Name = "filterModeLabel";
			filterModeLabel.Size = new System.Drawing.Size(70, 15);
			filterModeLabel.TabIndex = 9;
			filterModeLabel.Text = "Filter Mode:";
			// 
			// srcAlphaLabel
			// 
			srcAlphaLabel.AutoSize = true;
			srcAlphaLabel.Location = new System.Drawing.Point(159, 149);
			srcAlphaLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			srcAlphaLabel.Name = "srcAlphaLabel";
			srcAlphaLabel.Size = new System.Drawing.Size(80, 15);
			srcAlphaLabel.TabIndex = 11;
			srcAlphaLabel.Text = "Source Alpha:";
			// 
			// textureBox
			// 
			textureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			textureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			textureBox.Location = new System.Drawing.Point(14, 110);
			textureBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			textureBox.Name = "textureBox";
			textureBox.Size = new System.Drawing.Size(136, 136);
			textureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			textureBox.TabIndex = 4;
			textureBox.TabStop = false;
			textureBox.Click += textureBox_Click;
			// 
			// filterModeDropDown
			// 
			filterModeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			filterModeDropDown.FormattingEnabled = true;
			filterModeDropDown.Items.AddRange(new object[] { "PointSampled", "Bilinear", "Trilinear", "Reserved" });
			filterModeDropDown.Location = new System.Drawing.Point(162, 121);
			filterModeDropDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			filterModeDropDown.Name = "filterModeDropDown";
			filterModeDropDown.Size = new System.Drawing.Size(164, 23);
			filterModeDropDown.TabIndex = 7;
			filterModeDropDown.SelectionChangeCommitted += filterModeDropDown_SelectionChangeCommitted;
			filterModeDropDown.KeyDown += onKeyDown;
			// 
			// exponentLabel
			// 
			exponentLabel.AutoSize = true;
			exponentLabel.Location = new System.Drawing.Point(34, 83);
			exponentLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			exponentLabel.Name = "exponentLabel";
			exponentLabel.Size = new System.Drawing.Size(59, 15);
			exponentLabel.TabIndex = 6;
			exponentLabel.Text = "Exponent:";
			// 
			// specColorLabel
			// 
			specColorLabel.AutoSize = true;
			specColorLabel.Location = new System.Drawing.Point(6, 54);
			specColorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			specColorLabel.Name = "specColorLabel";
			specColorLabel.Size = new System.Drawing.Size(87, 15);
			specColorLabel.TabIndex = 4;
			specColorLabel.Text = "Specular Color:";
			// 
			// diffuseLabel
			// 
			diffuseLabel.AutoSize = true;
			diffuseLabel.Location = new System.Drawing.Point(16, 25);
			diffuseLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			diffuseLabel.Name = "diffuseLabel";
			diffuseLabel.Size = new System.Drawing.Size(79, 15);
			diffuseLabel.TabIndex = 0;
			diffuseLabel.Text = "Diffuse Color:";
			// 
			// doneButton
			// 
			doneButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			doneButton.Location = new System.Drawing.Point(550, 270);
			doneButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			doneButton.Name = "doneButton";
			doneButton.Size = new System.Drawing.Size(88, 27);
			doneButton.TabIndex = 0;
			doneButton.Text = "Done";
			doneButton.UseVisualStyleBackColor = true;
			doneButton.Click += doneButton_Click;
			doneButton.KeyDown += onKeyDown;
			// 
			// toolTip
			// 
			toolTip.AutoPopDelay = 30000;
			toolTip.InitialDelay = 500;
			toolTip.ReshowDelay = 100;
			// 
			// upButton
			// 
			upButton.Enabled = false;
			upButton.Location = new System.Drawing.Point(397, 14);
			upButton.Name = "upButton";
			upButton.Size = new System.Drawing.Size(23, 23);
			upButton.TabIndex = 2;
			upButton.Text = "↑";
			toolTip.SetToolTip(upButton, "Move the material up on the material list.");
			upButton.UseVisualStyleBackColor = true;
			upButton.Click += upButton_Click;
			// 
			// downButton
			// 
			downButton.Location = new System.Drawing.Point(426, 14);
			downButton.Name = "downButton";
			downButton.Size = new System.Drawing.Size(23, 23);
			downButton.TabIndex = 3;
			downButton.Text = "↓";
			toolTip.SetToolTip(downButton, "Move the material down on the material list.");
			downButton.UseVisualStyleBackColor = true;
			downButton.Click += downButton_Click;
			// 
			// cloneButton
			// 
			cloneButton.Location = new System.Drawing.Point(455, 14);
			cloneButton.Name = "cloneButton";
			cloneButton.Size = new System.Drawing.Size(57, 23);
			cloneButton.TabIndex = 4;
			cloneButton.Text = "Clone";
			toolTip.SetToolTip(cloneButton, "Create an identical copy of the material.");
			cloneButton.UseVisualStyleBackColor = true;
			cloneButton.Click += cloneButton_Click;
			// 
			// deleteButton
			// 
			deleteButton.Enabled = false;
			deleteButton.Location = new System.Drawing.Point(518, 14);
			deleteButton.Name = "deleteButton";
			deleteButton.Size = new System.Drawing.Size(57, 23);
			deleteButton.TabIndex = 5;
			deleteButton.Text = "Delete";
			toolTip.SetToolTip(deleteButton, "Delete the material.");
			deleteButton.UseVisualStyleBackColor = true;
			deleteButton.Click += deleteButton_Click;
			// 
			// resetButton
			// 
			resetButton.Location = new System.Drawing.Point(581, 14);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(57, 23);
			resetButton.TabIndex = 6;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the material list to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// labelTexID
			// 
			labelTexID.AutoSize = true;
			labelTexID.Location = new System.Drawing.Point(164, 83);
			labelTexID.Name = "labelTexID";
			labelTexID.Size = new System.Drawing.Size(62, 15);
			labelTexID.TabIndex = 16;
			labelTexID.Text = "Texture ID:";
			// 
			// numericUpDownTexID
			// 
			numericUpDownTexID.Location = new System.Drawing.Point(232, 80);
			numericUpDownTexID.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
			numericUpDownTexID.Name = "numericUpDownTexID";
			numericUpDownTexID.Size = new System.Drawing.Size(63, 23);
			numericUpDownTexID.TabIndex = 6;
			numericUpDownTexID.ValueChanged += numericUpDownTexID_ValueChanged;
			// 
			// MaterialEditor
			// 
			AcceptButton = doneButton;
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(648, 310);
			ControlBox = false;
			Controls.Add(resetButton);
			Controls.Add(deleteButton);
			Controls.Add(cloneButton);
			Controls.Add(downButton);
			Controls.Add(upButton);
			Controls.Add(doneButton);
			Controls.Add(generalSettingBox);
			Controls.Add(flagsGroupBox);
			Controls.Add(currentMaterialLabel);
			Controls.Add(comboMaterial);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "MaterialEditor";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Material Editor";
			Load += MaterialEditor_Load;
			KeyDown += onKeyDown;
			flagsGroupBox.ResumeLayout(false);
			flagsGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)userFlagsNumeric).EndInit();
			generalSettingBox.ResumeLayout(false);
			generalSettingBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)alphaSpecularNumeric).EndInit();
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).EndInit();
			((System.ComponentModel.ISupportInitialize)textureBox).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownTexID).EndInit();
			ResumeLayout(false);
			PerformLayout();

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
        private System.Windows.Forms.PictureBox textureBox;
        private System.Windows.Forms.TextBox exponentTextBox;
        private System.Windows.Forms.Panel diffuseColorBox;
        private System.Windows.Forms.ComboBox filterModeDropDown;
        private System.Windows.Forms.Label filterModeLabel;
        private System.Windows.Forms.Panel specColorBox;
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
		private System.Windows.Forms.NumericUpDown alphaSpecularNumeric;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button upButton;
		private System.Windows.Forms.Button downButton;
		private System.Windows.Forms.Button cloneButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.NumericUpDown numericUpDownTexID;
		private System.Windows.Forms.Label labelTexID;
	}
}