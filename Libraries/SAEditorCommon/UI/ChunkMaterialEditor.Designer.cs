namespace SAModel.SAEditorCommon.UI
{
    partial class ChunkMaterialEditor
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
			ambientColorBox = new System.Windows.Forms.Panel();
			ambientLabel = new System.Windows.Forms.Label();
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
			cloneButton = new System.Windows.Forms.Button();
			deleteButton = new System.Windows.Forms.Button();
			resetButton = new System.Windows.Forms.Button();
			flagsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)userFlagsNumeric).BeginInit();
			generalSettingBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).BeginInit();
			((System.ComponentModel.ISupportInitialize)textureBox).BeginInit();
			SuspendLayout();
			// 
			// comboMaterial
			// 
			comboMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboMaterial.FormattingEnabled = true;
			comboMaterial.Location = new System.Drawing.Point(178, 21);
			comboMaterial.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			comboMaterial.Name = "comboMaterial";
			comboMaterial.Size = new System.Drawing.Size(404, 33);
			comboMaterial.TabIndex = 1;
			comboMaterial.SelectedIndexChanged += comboMaterial_SelectedIndexChanged;
			comboMaterial.KeyDown += onKeyDown;
			// 
			// currentMaterialLabel
			// 
			currentMaterialLabel.AutoSize = true;
			currentMaterialLabel.Location = new System.Drawing.Point(21, 26);
			currentMaterialLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			currentMaterialLabel.Name = "currentMaterialLabel";
			currentMaterialLabel.Size = new System.Drawing.Size(142, 25);
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
			flagsGroupBox.Location = new System.Drawing.Point(552, 68);
			flagsGroupBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flagsGroupBox.Name = "flagsGroupBox";
			flagsGroupBox.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flagsGroupBox.Size = new System.Drawing.Size(418, 315);
			flagsGroupBox.TabIndex = 8;
			flagsGroupBox.TabStop = false;
			flagsGroupBox.Text = "Flags";
			// 
			// labelFlags
			// 
			labelFlags.AutoSize = true;
			labelFlags.Location = new System.Drawing.Point(82, 270);
			labelFlags.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			labelFlags.Name = "labelFlags";
			labelFlags.Size = new System.Drawing.Size(60, 25);
			labelFlags.TabIndex = 16;
			labelFlags.Text = "[flags]";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(10, 270);
			label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(57, 25);
			label2.TabIndex = 15;
			label2.Text = "Flags:";
			// 
			// userFlagsLabel
			// 
			userFlagsLabel.AutoSize = true;
			userFlagsLabel.Location = new System.Drawing.Point(195, 232);
			userFlagsLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			userFlagsLabel.Name = "userFlagsLabel";
			userFlagsLabel.Size = new System.Drawing.Size(97, 25);
			userFlagsLabel.TabIndex = 13;
			userFlagsLabel.Text = "User Flags:";
			// 
			// userFlagsNumeric
			// 
			userFlagsNumeric.Hexadecimal = true;
			userFlagsNumeric.Location = new System.Drawing.Point(304, 225);
			userFlagsNumeric.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			userFlagsNumeric.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			userFlagsNumeric.Name = "userFlagsNumeric";
			userFlagsNumeric.Size = new System.Drawing.Size(104, 31);
			userFlagsNumeric.TabIndex = 14;
			userFlagsNumeric.ValueChanged += userFlagsNumeric_ValueChanged;
			userFlagsNumeric.KeyDown += onKeyDown;
			// 
			// ignoreLightCheck
			// 
			ignoreLightCheck.AutoSize = true;
			ignoreLightCheck.Location = new System.Drawing.Point(182, 194);
			ignoreLightCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ignoreLightCheck.Name = "ignoreLightCheck";
			ignoreLightCheck.Size = new System.Drawing.Size(159, 29);
			ignoreLightCheck.TabIndex = 12;
			ignoreLightCheck.Text = "Ignore Lighting";
			toolTip.SetToolTip(ignoreLightCheck, "If checked, the mesh will not have any lighting applied.");
			ignoreLightCheck.UseVisualStyleBackColor = true;
			ignoreLightCheck.Click += ignoreLightCheck_Click;
			ignoreLightCheck.KeyDown += onKeyDown;
			// 
			// flatShadeCheck
			// 
			flatShadeCheck.AutoSize = true;
			flatShadeCheck.Location = new System.Drawing.Point(182, 160);
			flatShadeCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flatShadeCheck.Name = "flatShadeCheck";
			flatShadeCheck.Size = new System.Drawing.Size(131, 29);
			flatShadeCheck.TabIndex = 11;
			flatShadeCheck.Text = "Flat Shaded";
			toolTip.SetToolTip(flatShadeCheck, "If checked, polygon smoothing will be disabled and the model will appear faceted, like a cut gem or die. This flag does nothing in SADX.");
			flatShadeCheck.UseVisualStyleBackColor = true;
			flatShadeCheck.Click += flatShadeCheck_Click;
			flatShadeCheck.KeyDown += onKeyDown;
			// 
			// doubleSideCheck
			// 
			doubleSideCheck.AutoSize = true;
			doubleSideCheck.Location = new System.Drawing.Point(182, 130);
			doubleSideCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			doubleSideCheck.Name = "doubleSideCheck";
			doubleSideCheck.Size = new System.Drawing.Size(146, 29);
			doubleSideCheck.TabIndex = 10;
			doubleSideCheck.Text = "Double Sided";
			toolTip.SetToolTip(doubleSideCheck, "Doesn't do anything, since Sonic Adventure does not support backface cull.");
			doubleSideCheck.UseVisualStyleBackColor = true;
			doubleSideCheck.Click += doubleSideCheck_Click;
			doubleSideCheck.KeyDown += onKeyDown;
			// 
			// envMapCheck
			// 
			envMapCheck.AutoSize = true;
			envMapCheck.Location = new System.Drawing.Point(182, 98);
			envMapCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			envMapCheck.Name = "envMapCheck";
			envMapCheck.Size = new System.Drawing.Size(215, 29);
			envMapCheck.TabIndex = 9;
			envMapCheck.Text = "Environment Mapping";
			toolTip.SetToolTip(envMapCheck, "If checked, the texture's uv maps will be mapped to the environment and the model will appear 'shiny'.");
			envMapCheck.UseVisualStyleBackColor = true;
			envMapCheck.Click += envMapCheck_Click;
			envMapCheck.KeyDown += onKeyDown;
			// 
			// useTextureCheck
			// 
			useTextureCheck.AutoSize = true;
			useTextureCheck.Location = new System.Drawing.Point(182, 64);
			useTextureCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			useTextureCheck.Name = "useTextureCheck";
			useTextureCheck.Size = new System.Drawing.Size(127, 29);
			useTextureCheck.TabIndex = 8;
			useTextureCheck.Text = "Use Texture";
			toolTip.SetToolTip(useTextureCheck, "If checked, the texture map displayed to the left will be used. Otherwise the model will be a solid color.");
			useTextureCheck.UseVisualStyleBackColor = true;
			useTextureCheck.Click += useTextureCheck_Click;
			useTextureCheck.KeyDown += onKeyDown;
			// 
			// useAlphaCheck
			// 
			useAlphaCheck.AutoSize = true;
			useAlphaCheck.Location = new System.Drawing.Point(182, 33);
			useAlphaCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			useAlphaCheck.Name = "useAlphaCheck";
			useAlphaCheck.Size = new System.Drawing.Size(118, 29);
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
			ignoreSpecCheck.Location = new System.Drawing.Point(10, 225);
			ignoreSpecCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ignoreSpecCheck.Name = "ignoreSpecCheck";
			ignoreSpecCheck.Size = new System.Drawing.Size(162, 29);
			ignoreSpecCheck.TabIndex = 6;
			ignoreSpecCheck.Text = "Ignore Specular";
			toolTip.SetToolTip(ignoreSpecCheck, "Disables specular lighting on the material. This flag does nothing in SADX. In SA1 DC it is used for specular palette selection.");
			ignoreSpecCheck.UseVisualStyleBackColor = true;
			ignoreSpecCheck.Click += ignoreSpecCheck_Click;
			ignoreSpecCheck.KeyDown += onKeyDown;
			// 
			// flipVCheck
			// 
			flipVCheck.AutoSize = true;
			flipVCheck.Location = new System.Drawing.Point(10, 194);
			flipVCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flipVCheck.Name = "flipVCheck";
			flipVCheck.Size = new System.Drawing.Size(103, 29);
			flipVCheck.TabIndex = 5;
			flipVCheck.Text = "Mirror V";
			toolTip.SetToolTip(flipVCheck, "If checked, tiling on the V Axis is mirrored.");
			flipVCheck.UseVisualStyleBackColor = true;
			flipVCheck.Click += flipVCheck_Click;
			flipVCheck.KeyDown += onKeyDown;
			// 
			// flipUCheck
			// 
			flipUCheck.AutoSize = true;
			flipUCheck.Location = new System.Drawing.Point(10, 160);
			flipUCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flipUCheck.Name = "flipUCheck";
			flipUCheck.Size = new System.Drawing.Size(104, 29);
			flipUCheck.TabIndex = 4;
			flipUCheck.Text = "Mirror U";
			toolTip.SetToolTip(flipUCheck, "If checked, tiling on the U Axis is mirrored.");
			flipUCheck.UseVisualStyleBackColor = true;
			flipUCheck.Click += flipUCheck_Click;
			flipUCheck.KeyDown += onKeyDown;
			// 
			// clampVCheck
			// 
			clampVCheck.AutoSize = true;
			clampVCheck.Location = new System.Drawing.Point(10, 130);
			clampVCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			clampVCheck.Name = "clampVCheck";
			clampVCheck.Size = new System.Drawing.Size(105, 29);
			clampVCheck.TabIndex = 3;
			clampVCheck.Text = "Clamp V";
			toolTip.SetToolTip(clampVCheck, "Enable/Disable tiling on the V Axis.");
			clampVCheck.UseVisualStyleBackColor = true;
			clampVCheck.Click += clampVCheck_Click;
			clampVCheck.KeyDown += onKeyDown;
			// 
			// clampUCheck
			// 
			clampUCheck.AutoSize = true;
			clampUCheck.Location = new System.Drawing.Point(10, 98);
			clampUCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			clampUCheck.Name = "clampUCheck";
			clampUCheck.Size = new System.Drawing.Size(106, 29);
			clampUCheck.TabIndex = 2;
			clampUCheck.Text = "Clamp U";
			toolTip.SetToolTip(clampUCheck, "Enable/Disable tiling on the U Axis.");
			clampUCheck.UseVisualStyleBackColor = true;
			clampUCheck.Click += clampUCheck_Click;
			clampUCheck.KeyDown += onKeyDown;
			// 
			// superSampleCheck
			// 
			superSampleCheck.AutoSize = true;
			superSampleCheck.Location = new System.Drawing.Point(10, 64);
			superSampleCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			superSampleCheck.Name = "superSampleCheck";
			superSampleCheck.Size = new System.Drawing.Size(148, 29);
			superSampleCheck.TabIndex = 1;
			superSampleCheck.Text = "Super Sample";
			superSampleCheck.UseVisualStyleBackColor = true;
			superSampleCheck.Click += superSampleCheck_Click;
			superSampleCheck.KeyDown += onKeyDown;
			// 
			// pickStatusCheck
			// 
			pickStatusCheck.AutoSize = true;
			pickStatusCheck.Location = new System.Drawing.Point(10, 33);
			pickStatusCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			pickStatusCheck.Name = "pickStatusCheck";
			pickStatusCheck.Size = new System.Drawing.Size(163, 29);
			pickStatusCheck.TabIndex = 0;
			pickStatusCheck.Text = "Ignore Ambient";
			pickStatusCheck.UseVisualStyleBackColor = true;
			pickStatusCheck.Click += pickStatusCheck_Click;
			pickStatusCheck.KeyDown += onKeyDown;
			// 
			// generalSettingBox
			// 
			generalSettingBox.Controls.Add(ambientColorBox);
			generalSettingBox.Controls.Add(ambientLabel);
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
			generalSettingBox.Location = new System.Drawing.Point(21, 68);
			generalSettingBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			generalSettingBox.Name = "generalSettingBox";
			generalSettingBox.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
			generalSettingBox.Size = new System.Drawing.Size(516, 386);
			generalSettingBox.TabIndex = 7;
			generalSettingBox.TabStop = false;
			generalSettingBox.Text = "General";
			generalSettingBox.Enter += generalSettingBox_Enter;
			// 
			// ambientColorBox
			// 
			ambientColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			ambientColorBox.Location = new System.Drawing.Point(153, 72);
			ambientColorBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ambientColorBox.Name = "ambientColorBox";
			ambientColorBox.Size = new System.Drawing.Size(64, 32);
			ambientColorBox.TabIndex = 17;
			ambientColorBox.TabStop = true;
			toolTip.SetToolTip(ambientColorBox, "Ambient is the base coloration for the model when no other lighting effects are applied. This color value is typically a constant.");
			ambientColorBox.Click += ambientColorBox_Click;
			// 
			// ambientLabel
			// 
			ambientLabel.AutoSize = true;
			ambientLabel.Location = new System.Drawing.Point(12, 75);
			ambientLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			ambientLabel.Name = "ambientLabel";
			ambientLabel.Size = new System.Drawing.Size(132, 25);
			ambientLabel.TabIndex = 16;
			ambientLabel.Text = "Ambient Color:";
			ambientLabel.Click += label1_Click;
			// 
			// labelAlpha
			// 
			labelAlpha.AutoSize = true;
			labelAlpha.Location = new System.Drawing.Point(229, 35);
			labelAlpha.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			labelAlpha.Name = "labelAlpha";
			labelAlpha.Size = new System.Drawing.Size(62, 25);
			labelAlpha.TabIndex = 2;
			labelAlpha.Text = "Alpha:";
			// 
			// alphaDiffuseNumeric
			// 
			alphaDiffuseNumeric.Location = new System.Drawing.Point(303, 33);
			alphaDiffuseNumeric.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			alphaDiffuseNumeric.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			alphaDiffuseNumeric.Name = "alphaDiffuseNumeric";
			alphaDiffuseNumeric.Size = new System.Drawing.Size(73, 31);
			alphaDiffuseNumeric.TabIndex = 2;
			alphaDiffuseNumeric.ValueChanged += alphaDiffuseNumeric_ValueChanged;
			alphaDiffuseNumeric.KeyDown += onKeyDown;
			// 
			// dstAlphaCombo
			// 
			dstAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			dstAlphaCombo.FormattingEnabled = true;
			dstAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "OtherColor", "InverseOtherColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			dstAlphaCombo.Location = new System.Drawing.Point(243, 326);
			dstAlphaCombo.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			dstAlphaCombo.Name = "dstAlphaCombo";
			dstAlphaCombo.Size = new System.Drawing.Size(244, 33);
			dstAlphaCombo.TabIndex = 8;
			dstAlphaCombo.SelectedIndexChanged += dstAlphaCombo_SelectedIndexChanged;
			dstAlphaCombo.KeyDown += onKeyDown;
			// 
			// specColorBox
			// 
			specColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			specColorBox.Location = new System.Drawing.Point(153, 111);
			specColorBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			specColorBox.Name = "specColorBox";
			specColorBox.Size = new System.Drawing.Size(64, 32);
			specColorBox.TabIndex = 3;
			specColorBox.TabStop = true;
			toolTip.SetToolTip(specColorBox, "Specular reflection is the mirror-like reflection of light from a surface. This specular color will tint the apparent highlights on the model.");
			specColorBox.Click += specColorBox_Click;
			// 
			// diffuseColorBox
			// 
			diffuseColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			diffuseColorBox.Location = new System.Drawing.Point(153, 33);
			diffuseColorBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			diffuseColorBox.Name = "diffuseColorBox";
			diffuseColorBox.Size = new System.Drawing.Size(64, 32);
			diffuseColorBox.TabIndex = 1;
			diffuseColorBox.TabStop = true;
			toolTip.SetToolTip(diffuseColorBox, "Diffuse lighting is scattered as opposed to direct. Specifically, this 'diffuse color' will act as a tint to the model.");
			diffuseColorBox.Click += diffuseColorBox_Click;
			// 
			// destinationAlphaLabel
			// 
			destinationAlphaLabel.AutoSize = true;
			destinationAlphaLabel.Location = new System.Drawing.Point(238, 292);
			destinationAlphaLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			destinationAlphaLabel.Name = "destinationAlphaLabel";
			destinationAlphaLabel.Size = new System.Drawing.Size(157, 25);
			destinationAlphaLabel.TabIndex = 13;
			destinationAlphaLabel.Text = "Destination Alpha:";
			// 
			// exponentTextBox
			// 
			exponentTextBox.Location = new System.Drawing.Point(326, 111);
			exponentTextBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			exponentTextBox.Name = "exponentTextBox";
			exponentTextBox.Size = new System.Drawing.Size(61, 31);
			exponentTextBox.TabIndex = 5;
			exponentTextBox.KeyDown += onKeyDown;
			exponentTextBox.Leave += exponentTextBox_Leave;
			// 
			// srcAlphaCombo
			// 
			srcAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			srcAlphaCombo.FormattingEnabled = true;
			srcAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "OtherColor", "InverseOtherColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			srcAlphaCombo.Location = new System.Drawing.Point(243, 250);
			srcAlphaCombo.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			srcAlphaCombo.Name = "srcAlphaCombo";
			srcAlphaCombo.Size = new System.Drawing.Size(244, 33);
			srcAlphaCombo.TabIndex = 7;
			srcAlphaCombo.SelectionChangeCommitted += srcAlphaCombo_SelectionChangeCommitted;
			srcAlphaCombo.KeyDown += onKeyDown;
			// 
			// filterModeLabel
			// 
			filterModeLabel.AutoSize = true;
			filterModeLabel.Location = new System.Drawing.Point(243, 154);
			filterModeLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			filterModeLabel.Name = "filterModeLabel";
			filterModeLabel.Size = new System.Drawing.Size(106, 25);
			filterModeLabel.TabIndex = 9;
			filterModeLabel.Text = "Filter Mode:";
			// 
			// srcAlphaLabel
			// 
			srcAlphaLabel.AutoSize = true;
			srcAlphaLabel.Location = new System.Drawing.Point(238, 224);
			srcAlphaLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			srcAlphaLabel.Name = "srcAlphaLabel";
			srcAlphaLabel.Size = new System.Drawing.Size(121, 25);
			srcAlphaLabel.TabIndex = 11;
			srcAlphaLabel.Text = "Source Alpha:";
			// 
			// textureBox
			// 
			textureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			textureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			textureBox.Location = new System.Drawing.Point(21, 165);
			textureBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			textureBox.Name = "textureBox";
			textureBox.Size = new System.Drawing.Size(203, 203);
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
			filterModeDropDown.Location = new System.Drawing.Point(243, 182);
			filterModeDropDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			filterModeDropDown.Name = "filterModeDropDown";
			filterModeDropDown.Size = new System.Drawing.Size(244, 33);
			filterModeDropDown.TabIndex = 6;
			filterModeDropDown.SelectionChangeCommitted += filterModeDropDown_SelectionChangeCommitted;
			filterModeDropDown.KeyDown += onKeyDown;
			// 
			// exponentLabel
			// 
			exponentLabel.AutoSize = true;
			exponentLabel.Location = new System.Drawing.Point(226, 113);
			exponentLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			exponentLabel.Name = "exponentLabel";
			exponentLabel.Size = new System.Drawing.Size(90, 25);
			exponentLabel.TabIndex = 6;
			exponentLabel.Text = "Exponent:";
			// 
			// specColorLabel
			// 
			specColorLabel.AutoSize = true;
			specColorLabel.Location = new System.Drawing.Point(13, 113);
			specColorLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			specColorLabel.Name = "specColorLabel";
			specColorLabel.Size = new System.Drawing.Size(131, 25);
			specColorLabel.TabIndex = 4;
			specColorLabel.Text = "Specular Color:";
			// 
			// diffuseLabel
			// 
			diffuseLabel.AutoSize = true;
			diffuseLabel.Location = new System.Drawing.Point(24, 35);
			diffuseLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			diffuseLabel.Name = "diffuseLabel";
			diffuseLabel.Size = new System.Drawing.Size(120, 25);
			diffuseLabel.TabIndex = 0;
			diffuseLabel.Text = "Diffuse Color:";
			// 
			// doneButton
			// 
			doneButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			doneButton.Location = new System.Drawing.Point(842, 405);
			doneButton.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			doneButton.Name = "doneButton";
			doneButton.Size = new System.Drawing.Size(132, 40);
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
			// cloneButton
			// 
			cloneButton.Location = new System.Drawing.Point(606, 21);
			cloneButton.Margin = new System.Windows.Forms.Padding(4);
			cloneButton.Name = "cloneButton";
			cloneButton.Size = new System.Drawing.Size(86, 34);
			cloneButton.TabIndex = 4;
			cloneButton.Text = "Clone";
			toolTip.SetToolTip(cloneButton, "Create an identical copy of the material.");
			cloneButton.UseVisualStyleBackColor = true;
			cloneButton.Click += cloneButton_Click;
			// 
			// deleteButton
			// 
			deleteButton.Enabled = false;
			deleteButton.Location = new System.Drawing.Point(700, 21);
			deleteButton.Margin = new System.Windows.Forms.Padding(4);
			deleteButton.Name = "deleteButton";
			deleteButton.Size = new System.Drawing.Size(86, 34);
			deleteButton.TabIndex = 5;
			deleteButton.Text = "Delete";
			toolTip.SetToolTip(deleteButton, "Delete the material.");
			deleteButton.UseVisualStyleBackColor = true;
			deleteButton.Click += deleteButton_Click;
			// 
			// resetButton
			// 
			resetButton.Location = new System.Drawing.Point(794, 21);
			resetButton.Margin = new System.Windows.Forms.Padding(4);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(86, 34);
			resetButton.TabIndex = 6;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the material list to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// ChunkMaterialEditor
			// 
			AcceptButton = doneButton;
			AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(989, 465);
			ControlBox = false;
			Controls.Add(resetButton);
			Controls.Add(deleteButton);
			Controls.Add(cloneButton);
			Controls.Add(doneButton);
			Controls.Add(generalSettingBox);
			Controls.Add(flagsGroupBox);
			Controls.Add(currentMaterialLabel);
			Controls.Add(comboMaterial);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkMaterialEditor";
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
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).EndInit();
			((System.ComponentModel.ISupportInitialize)textureBox).EndInit();
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
		private System.Windows.Forms.Button cloneButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.Label ambientLabel;
		private System.Windows.Forms.Panel ambientColorBox;
	}
}