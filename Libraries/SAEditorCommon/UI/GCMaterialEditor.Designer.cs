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
			flipVCheck = new System.Windows.Forms.CheckBox();
			flipUCheck = new System.Windows.Forms.CheckBox();
			clampVCheck = new System.Windows.Forms.CheckBox();
			clampUCheck = new System.Windows.Forms.CheckBox();
			generalSettingBox = new System.Windows.Forms.GroupBox();
			labelAlpha = new System.Windows.Forms.Label();
			alphaDiffuseNumeric = new System.Windows.Forms.NumericUpDown();
			dstAlphaCombo = new System.Windows.Forms.ComboBox();
			diffuseColorBox = new System.Windows.Forms.Panel();
			destinationAlphaLabel = new System.Windows.Forms.Label();
			srcAlphaCombo = new System.Windows.Forms.ComboBox();
			filterModeLabel = new System.Windows.Forms.Label();
			srcAlphaLabel = new System.Windows.Forms.Label();
			textureBox = new System.Windows.Forms.PictureBox();
			filterModeDropDown = new System.Windows.Forms.ComboBox();
			diffuseLabel = new System.Windows.Forms.Label();
			doneButton = new System.Windows.Forms.Button();
			toolTip = new System.Windows.Forms.ToolTip(components);
			resetButton = new System.Windows.Forms.Button();
			deleteButton = new System.Windows.Forms.Button();
			cloneButton = new System.Windows.Forms.Button();
			downButton = new System.Windows.Forms.Button();
			upButton = new System.Windows.Forms.Button();
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
			comboMaterial.Location = new System.Drawing.Point(119, 14);
			comboMaterial.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			comboMaterial.Name = "comboMaterial";
			comboMaterial.Size = new System.Drawing.Size(263, 23);
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
			flagsGroupBox.Controls.Add(flipVCheck);
			flagsGroupBox.Controls.Add(flipUCheck);
			flagsGroupBox.Controls.Add(clampVCheck);
			flagsGroupBox.Controls.Add(clampUCheck);
			flagsGroupBox.Location = new System.Drawing.Point(358, 45);
			flagsGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flagsGroupBox.Name = "flagsGroupBox";
			flagsGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flagsGroupBox.Size = new System.Drawing.Size(279, 210);
			flagsGroupBox.TabIndex = 4;
			flagsGroupBox.TabStop = false;
			flagsGroupBox.Text = "Flags";
			// 
			// labelFlags
			// 
			labelFlags.AutoSize = true;
			labelFlags.Location = new System.Drawing.Point(50, 155);
			labelFlags.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelFlags.Name = "labelFlags";
			labelFlags.Size = new System.Drawing.Size(40, 15);
			labelFlags.TabIndex = 16;
			labelFlags.Text = "[flags]";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(7, 155);
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
			userFlagsNumeric.TabIndex = 14;
			userFlagsNumeric.ValueChanged += userFlagsNumeric_ValueChanged;
			userFlagsNumeric.KeyDown += onKeyDown;
			// 
			// ignoreLightCheck
			// 
			ignoreLightCheck.AutoSize = true;
			ignoreLightCheck.Location = new System.Drawing.Point(121, 107);
			ignoreLightCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ignoreLightCheck.Name = "ignoreLightCheck";
			ignoreLightCheck.Size = new System.Drawing.Size(107, 19);
			ignoreLightCheck.TabIndex = 12;
			ignoreLightCheck.Text = "Ignore Lighting";
			toolTip.SetToolTip(ignoreLightCheck, "If checked, the model will not have any lighting applied.");
			ignoreLightCheck.UseVisualStyleBackColor = true;
			ignoreLightCheck.Click += ignoreLightCheck_Click;
			ignoreLightCheck.KeyDown += onKeyDown;
			// 
			// flatShadeCheck
			// 
			flatShadeCheck.AutoSize = true;
			flatShadeCheck.Location = new System.Drawing.Point(121, 87);
			flatShadeCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flatShadeCheck.Name = "flatShadeCheck";
			flatShadeCheck.Size = new System.Drawing.Size(87, 19);
			flatShadeCheck.TabIndex = 11;
			flatShadeCheck.Text = "Flat Shaded";
			toolTip.SetToolTip(flatShadeCheck, "If checked, polygon smoothing will be disabled and the model will appear faceted, like a cut gem or die.");
			flatShadeCheck.UseVisualStyleBackColor = true;
			flatShadeCheck.Click += flatShadeCheck_Click;
			flatShadeCheck.KeyDown += onKeyDown;
			// 
			// doubleSideCheck
			// 
			doubleSideCheck.AutoSize = true;
			doubleSideCheck.Location = new System.Drawing.Point(121, 65);
			doubleSideCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			doubleSideCheck.Name = "doubleSideCheck";
			doubleSideCheck.Size = new System.Drawing.Size(96, 19);
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
			envMapCheck.Location = new System.Drawing.Point(121, 43);
			envMapCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			envMapCheck.Name = "envMapCheck";
			envMapCheck.Size = new System.Drawing.Size(145, 19);
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
			useTextureCheck.Location = new System.Drawing.Point(121, 21);
			useTextureCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			useTextureCheck.Name = "useTextureCheck";
			useTextureCheck.Size = new System.Drawing.Size(86, 19);
			useTextureCheck.TabIndex = 8;
			useTextureCheck.Text = "Use Texture";
			toolTip.SetToolTip(useTextureCheck, "If checked, the texture map displayed to the left will be used. Otherwise the model will be a solid color.");
			useTextureCheck.UseVisualStyleBackColor = true;
			useTextureCheck.Click += useTextureCheck_Click;
			useTextureCheck.KeyDown += onKeyDown;
			// 
			// flipVCheck
			// 
			flipVCheck.AutoSize = true;
			flipVCheck.Location = new System.Drawing.Point(7, 87);
			flipVCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flipVCheck.Name = "flipVCheck";
			flipVCheck.Size = new System.Drawing.Size(69, 19);
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
			flipUCheck.Location = new System.Drawing.Point(7, 65);
			flipUCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			flipUCheck.Name = "flipUCheck";
			flipUCheck.Size = new System.Drawing.Size(70, 19);
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
			clampVCheck.Location = new System.Drawing.Point(7, 43);
			clampVCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			clampVCheck.Name = "clampVCheck";
			clampVCheck.Size = new System.Drawing.Size(71, 19);
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
			clampUCheck.Location = new System.Drawing.Point(7, 21);
			clampUCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			clampUCheck.Name = "clampUCheck";
			clampUCheck.Size = new System.Drawing.Size(72, 19);
			clampUCheck.TabIndex = 2;
			clampUCheck.Text = "Clamp U";
			toolTip.SetToolTip(clampUCheck, "Enable/Disable tiling on the U Axis.");
			clampUCheck.UseVisualStyleBackColor = true;
			clampUCheck.Click += clampUCheck_Click;
			clampUCheck.KeyDown += onKeyDown;
			// 
			// generalSettingBox
			// 
			generalSettingBox.Controls.Add(labelAlpha);
			generalSettingBox.Controls.Add(alphaDiffuseNumeric);
			generalSettingBox.Controls.Add(dstAlphaCombo);
			generalSettingBox.Controls.Add(diffuseColorBox);
			generalSettingBox.Controls.Add(destinationAlphaLabel);
			generalSettingBox.Controls.Add(srcAlphaCombo);
			generalSettingBox.Controls.Add(filterModeLabel);
			generalSettingBox.Controls.Add(srcAlphaLabel);
			generalSettingBox.Controls.Add(textureBox);
			generalSettingBox.Controls.Add(filterModeDropDown);
			generalSettingBox.Controls.Add(diffuseLabel);
			generalSettingBox.Location = new System.Drawing.Point(14, 45);
			generalSettingBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			generalSettingBox.Name = "generalSettingBox";
			generalSettingBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			generalSettingBox.Size = new System.Drawing.Size(337, 257);
			generalSettingBox.TabIndex = 3;
			generalSettingBox.TabStop = false;
			generalSettingBox.Text = "General";
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
			alphaDiffuseNumeric.TabIndex = 3;
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
			dstAlphaCombo.TabIndex = 14;
			dstAlphaCombo.SelectedIndexChanged += dstAlphaCombo_SelectedIndexChanged;
			dstAlphaCombo.KeyDown += onKeyDown;
			// 
			// diffuseColorBox
			// 
			diffuseColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			diffuseColorBox.Location = new System.Drawing.Point(105, 22);
			diffuseColorBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			diffuseColorBox.Name = "diffuseColorBox";
			diffuseColorBox.Size = new System.Drawing.Size(43, 22);
			diffuseColorBox.TabIndex = 1;
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
			// srcAlphaCombo
			// 
			srcAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			srcAlphaCombo.FormattingEnabled = true;
			srcAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "OtherColor", "InverseOtherColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			srcAlphaCombo.Location = new System.Drawing.Point(162, 167);
			srcAlphaCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			srcAlphaCombo.Name = "srcAlphaCombo";
			srcAlphaCombo.Size = new System.Drawing.Size(164, 23);
			srcAlphaCombo.TabIndex = 12;
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
			textureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			textureBox.Location = new System.Drawing.Point(9, 111);
			textureBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			textureBox.Name = "textureBox";
			textureBox.Size = new System.Drawing.Size(135, 130);
			textureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
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
			filterModeDropDown.TabIndex = 10;
			filterModeDropDown.SelectionChangeCommitted += filterModeDropDown_SelectionChangeCommitted;
			filterModeDropDown.KeyDown += onKeyDown;
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
			// resetButton
			// 
			resetButton.Location = new System.Drawing.Point(573, 13);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(57, 23);
			resetButton.TabIndex = 11;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the material list to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// deleteButton
			// 
			deleteButton.Enabled = false;
			deleteButton.Location = new System.Drawing.Point(510, 13);
			deleteButton.Name = "deleteButton";
			deleteButton.Size = new System.Drawing.Size(57, 23);
			deleteButton.TabIndex = 10;
			deleteButton.Text = "Delete";
			toolTip.SetToolTip(deleteButton, "Delete the material.");
			deleteButton.UseVisualStyleBackColor = true;
			deleteButton.Click += deleteButton_Click;
			// 
			// cloneButton
			// 
			cloneButton.Location = new System.Drawing.Point(447, 13);
			cloneButton.Name = "cloneButton";
			cloneButton.Size = new System.Drawing.Size(57, 23);
			cloneButton.TabIndex = 9;
			cloneButton.Text = "Clone";
			toolTip.SetToolTip(cloneButton, "Create an identical copy of the material.");
			cloneButton.UseVisualStyleBackColor = true;
			cloneButton.Click += cloneButton_Click;
			// 
			// downButton
			// 
			downButton.Location = new System.Drawing.Point(418, 13);
			downButton.Name = "downButton";
			downButton.Size = new System.Drawing.Size(23, 23);
			downButton.TabIndex = 8;
			downButton.Text = "↓";
			toolTip.SetToolTip(downButton, "Move the material down on the material list.");
			downButton.UseVisualStyleBackColor = true;
			downButton.Click += downButton_Click;
			// 
			// upButton
			// 
			upButton.Enabled = false;
			upButton.Location = new System.Drawing.Point(389, 13);
			upButton.Name = "upButton";
			upButton.Size = new System.Drawing.Size(23, 23);
			upButton.TabIndex = 7;
			upButton.Text = "↑";
			toolTip.SetToolTip(upButton, "Move the material up on the material list.");
			upButton.UseVisualStyleBackColor = true;
			upButton.Click += upButton_Click;
			// 
			// GCMaterialEditor
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
			Name = "GCMaterialEditor";
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