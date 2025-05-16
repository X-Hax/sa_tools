namespace SAModel.SAEditorCommon.UI
{
    partial class ChunkModelMaterialDataEditor
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
			colorDialog = new System.Windows.Forms.ColorDialog();
			diffuseSettingBox = new System.Windows.Forms.GroupBox();
			diffuseBUpDown = new System.Windows.Forms.NumericUpDown();
			diffuseGUpDown = new System.Windows.Forms.NumericUpDown();
			diffuseRUpDown = new System.Windows.Forms.NumericUpDown();
			label4 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			labelAlpha = new System.Windows.Forms.Label();
			alphaDiffuseNumeric = new System.Windows.Forms.NumericUpDown();
			diffuseColorBox = new System.Windows.Forms.Panel();
			diffuseLabel = new System.Windows.Forms.Label();
			ambientColorBox = new System.Windows.Forms.Panel();
			ambientLabel = new System.Windows.Forms.Label();
			dstAlphaCombo = new System.Windows.Forms.ComboBox();
			specColorBox = new System.Windows.Forms.Panel();
			destinationAlphaLabel = new System.Windows.Forms.Label();
			exponentTextBox = new System.Windows.Forms.TextBox();
			srcAlphaCombo = new System.Windows.Forms.ComboBox();
			srcAlphaLabel = new System.Windows.Forms.Label();
			exponentLabel = new System.Windows.Forms.Label();
			specColorLabel = new System.Windows.Forms.Label();
			doneButton = new System.Windows.Forms.Button();
			toolTip = new System.Windows.Forms.ToolTip(components);
			resetButton = new System.Windows.Forms.Button();
			ambientSettingBox = new System.Windows.Forms.GroupBox();
			ambientBUpDown = new System.Windows.Forms.NumericUpDown();
			ambientGUpDown = new System.Windows.Forms.NumericUpDown();
			ambientRUpDown = new System.Windows.Forms.NumericUpDown();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			specularSettingBox = new System.Windows.Forms.GroupBox();
			specularBUpDown = new System.Windows.Forms.NumericUpDown();
			specularGUpDown = new System.Windows.Forms.NumericUpDown();
			specularRUpDown = new System.Windows.Forms.NumericUpDown();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			blendModeSettingsBox = new System.Windows.Forms.GroupBox();
			diffuseSettingBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)diffuseBUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)diffuseGUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)diffuseRUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).BeginInit();
			ambientSettingBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ambientBUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)ambientGUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)ambientRUpDown).BeginInit();
			specularSettingBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)specularBUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)specularGUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)specularRUpDown).BeginInit();
			blendModeSettingsBox.SuspendLayout();
			SuspendLayout();
			// 
			// colorDialog
			// 
			colorDialog.AnyColor = true;
			colorDialog.FullOpen = true;
			colorDialog.SolidColorOnly = true;
			// 
			// diffuseSettingBox
			// 
			diffuseSettingBox.Controls.Add(diffuseBUpDown);
			diffuseSettingBox.Controls.Add(diffuseGUpDown);
			diffuseSettingBox.Controls.Add(diffuseRUpDown);
			diffuseSettingBox.Controls.Add(label4);
			diffuseSettingBox.Controls.Add(label2);
			diffuseSettingBox.Controls.Add(label1);
			diffuseSettingBox.Controls.Add(labelAlpha);
			diffuseSettingBox.Controls.Add(alphaDiffuseNumeric);
			diffuseSettingBox.Controls.Add(diffuseColorBox);
			diffuseSettingBox.Controls.Add(diffuseLabel);
			diffuseSettingBox.Location = new System.Drawing.Point(15, 13);
			diffuseSettingBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			diffuseSettingBox.Name = "diffuseSettingBox";
			diffuseSettingBox.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
			diffuseSettingBox.Size = new System.Drawing.Size(323, 181);
			diffuseSettingBox.TabIndex = 7;
			diffuseSettingBox.TabStop = false;
			diffuseSettingBox.Text = "Diffuse";
			diffuseSettingBox.Enter += generalSettingBox_Enter;
			// 
			// diffuseBUpDown
			// 
			diffuseBUpDown.Location = new System.Drawing.Point(76, 128);
			diffuseBUpDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			diffuseBUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			diffuseBUpDown.Name = "diffuseBUpDown";
			diffuseBUpDown.Size = new System.Drawing.Size(73, 31);
			diffuseBUpDown.TabIndex = 8;
			diffuseBUpDown.ValueChanged += diffuseBUpDown_ValueChanged;
			// 
			// diffuseGUpDown
			// 
			diffuseGUpDown.Location = new System.Drawing.Point(230, 80);
			diffuseGUpDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			diffuseGUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			diffuseGUpDown.Name = "diffuseGUpDown";
			diffuseGUpDown.Size = new System.Drawing.Size(73, 31);
			diffuseGUpDown.TabIndex = 7;
			diffuseGUpDown.ValueChanged += diffuseGUpDown_ValueChanged;
			// 
			// diffuseRUpDown
			// 
			diffuseRUpDown.Location = new System.Drawing.Point(76, 80);
			diffuseRUpDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			diffuseRUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			diffuseRUpDown.Name = "diffuseRUpDown";
			diffuseRUpDown.Size = new System.Drawing.Size(73, 31);
			diffuseRUpDown.TabIndex = 6;
			diffuseRUpDown.ValueChanged += diffuseRUpDown_ValueChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(22, 131);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(49, 25);
			label4.TabIndex = 5;
			label4.Text = "Blue:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(163, 82);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(62, 25);
			label2.TabIndex = 4;
			label2.Text = "Green:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(25, 82);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(46, 25);
			label1.TabIndex = 3;
			label1.Text = "Red:";
			// 
			// labelAlpha
			// 
			labelAlpha.AutoSize = true;
			labelAlpha.Location = new System.Drawing.Point(163, 34);
			labelAlpha.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			labelAlpha.Name = "labelAlpha";
			labelAlpha.Size = new System.Drawing.Size(62, 25);
			labelAlpha.TabIndex = 2;
			labelAlpha.Text = "Alpha:";
			// 
			// alphaDiffuseNumeric
			// 
			alphaDiffuseNumeric.Location = new System.Drawing.Point(230, 32);
			alphaDiffuseNumeric.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			alphaDiffuseNumeric.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			alphaDiffuseNumeric.Name = "alphaDiffuseNumeric";
			alphaDiffuseNumeric.Size = new System.Drawing.Size(73, 31);
			alphaDiffuseNumeric.TabIndex = 2;
			alphaDiffuseNumeric.ValueChanged += alphaDiffuseNumeric_ValueChanged;
			// 
			// diffuseColorBox
			// 
			diffuseColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			diffuseColorBox.Location = new System.Drawing.Point(76, 32);
			diffuseColorBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			diffuseColorBox.Name = "diffuseColorBox";
			diffuseColorBox.Size = new System.Drawing.Size(64, 32);
			diffuseColorBox.TabIndex = 1;
			diffuseColorBox.TabStop = true;
			toolTip.SetToolTip(diffuseColorBox, "Diffuse lighting is scattered as opposed to direct. Specifically, this 'diffuse color' will act as a tint to the model.");
			diffuseColorBox.Click += diffuseColorBox_Click;
			// 
			// diffuseLabel
			// 
			diffuseLabel.AutoSize = true;
			diffuseLabel.Location = new System.Drawing.Point(12, 34);
			diffuseLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			diffuseLabel.Name = "diffuseLabel";
			diffuseLabel.Size = new System.Drawing.Size(59, 25);
			diffuseLabel.TabIndex = 0;
			diffuseLabel.Text = "Color:";
			// 
			// ambientColorBox
			// 
			ambientColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			ambientColorBox.Location = new System.Drawing.Point(76, 40);
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
			ambientLabel.Location = new System.Drawing.Point(12, 42);
			ambientLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			ambientLabel.Name = "ambientLabel";
			ambientLabel.Size = new System.Drawing.Size(59, 25);
			ambientLabel.TabIndex = 16;
			ambientLabel.Text = "Color:";
			// 
			// dstAlphaCombo
			// 
			dstAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			dstAlphaCombo.FormattingEnabled = true;
			dstAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "OtherColor", "InverseOtherColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			dstAlphaCombo.Location = new System.Drawing.Point(34, 130);
			dstAlphaCombo.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			dstAlphaCombo.Name = "dstAlphaCombo";
			dstAlphaCombo.Size = new System.Drawing.Size(244, 33);
			dstAlphaCombo.TabIndex = 8;
			dstAlphaCombo.SelectedIndexChanged += dstAlphaCombo_SelectedIndexChanged;
			// 
			// specColorBox
			// 
			specColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			specColorBox.Location = new System.Drawing.Point(76, 32);
			specColorBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			specColorBox.Name = "specColorBox";
			specColorBox.Size = new System.Drawing.Size(64, 32);
			specColorBox.TabIndex = 3;
			specColorBox.TabStop = true;
			toolTip.SetToolTip(specColorBox, "Specular reflection is the mirror-like reflection of light from a surface. This specular color will tint the apparent highlights on the model.");
			specColorBox.Click += specColorBox_Click;
			// 
			// destinationAlphaLabel
			// 
			destinationAlphaLabel.AutoSize = true;
			destinationAlphaLabel.Location = new System.Drawing.Point(29, 104);
			destinationAlphaLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			destinationAlphaLabel.Name = "destinationAlphaLabel";
			destinationAlphaLabel.Size = new System.Drawing.Size(157, 25);
			destinationAlphaLabel.TabIndex = 13;
			destinationAlphaLabel.Text = "Destination Alpha:";
			// 
			// exponentTextBox
			// 
			exponentTextBox.Location = new System.Drawing.Point(248, 33);
			exponentTextBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			exponentTextBox.Name = "exponentTextBox";
			exponentTextBox.Size = new System.Drawing.Size(61, 31);
			exponentTextBox.TabIndex = 5;
			exponentTextBox.Leave += exponentTextBox_Leave;
			// 
			// srcAlphaCombo
			// 
			srcAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			srcAlphaCombo.FormattingEnabled = true;
			srcAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "OtherColor", "InverseOtherColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			srcAlphaCombo.Location = new System.Drawing.Point(34, 62);
			srcAlphaCombo.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			srcAlphaCombo.Name = "srcAlphaCombo";
			srcAlphaCombo.Size = new System.Drawing.Size(244, 33);
			srcAlphaCombo.TabIndex = 7;
			srcAlphaCombo.SelectionChangeCommitted += srcAlphaCombo_SelectionChangeCommitted;
			// 
			// srcAlphaLabel
			// 
			srcAlphaLabel.AutoSize = true;
			srcAlphaLabel.Location = new System.Drawing.Point(29, 36);
			srcAlphaLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			srcAlphaLabel.Name = "srcAlphaLabel";
			srcAlphaLabel.Size = new System.Drawing.Size(121, 25);
			srcAlphaLabel.TabIndex = 11;
			srcAlphaLabel.Text = "Source Alpha:";
			// 
			// exponentLabel
			// 
			exponentLabel.AutoSize = true;
			exponentLabel.Location = new System.Drawing.Point(156, 36);
			exponentLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			exponentLabel.Name = "exponentLabel";
			exponentLabel.Size = new System.Drawing.Size(90, 25);
			exponentLabel.TabIndex = 6;
			exponentLabel.Text = "Exponent:";
			// 
			// specColorLabel
			// 
			specColorLabel.AutoSize = true;
			specColorLabel.Location = new System.Drawing.Point(12, 36);
			specColorLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			specColorLabel.Name = "specColorLabel";
			specColorLabel.Size = new System.Drawing.Size(59, 25);
			specColorLabel.TabIndex = 4;
			specColorLabel.Text = "Color:";
			// 
			// doneButton
			// 
			doneButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			doneButton.Location = new System.Drawing.Point(537, 363);
			doneButton.Margin = new System.Windows.Forms.Padding(4);
			doneButton.Name = "doneButton";
			doneButton.Size = new System.Drawing.Size(132, 40);
			doneButton.TabIndex = 0;
			doneButton.Text = "Done";
			doneButton.UseVisualStyleBackColor = true;
			doneButton.Click += doneButton_Click;
			// 
			// toolTip
			// 
			toolTip.AutoPopDelay = 30000;
			toolTip.InitialDelay = 500;
			toolTip.ReshowDelay = 100;
			// 
			// resetButton
			// 
			resetButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			resetButton.Location = new System.Drawing.Point(378, 363);
			resetButton.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(132, 40);
			resetButton.TabIndex = 6;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the poly data to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// ambientSettingBox
			// 
			ambientSettingBox.Controls.Add(ambientBUpDown);
			ambientSettingBox.Controls.Add(ambientGUpDown);
			ambientSettingBox.Controls.Add(ambientRUpDown);
			ambientSettingBox.Controls.Add(label5);
			ambientSettingBox.Controls.Add(label6);
			ambientSettingBox.Controls.Add(label7);
			ambientSettingBox.Controls.Add(ambientLabel);
			ambientSettingBox.Controls.Add(ambientColorBox);
			ambientSettingBox.Location = new System.Drawing.Point(347, 13);
			ambientSettingBox.Name = "ambientSettingBox";
			ambientSettingBox.Size = new System.Drawing.Size(303, 136);
			ambientSettingBox.TabIndex = 18;
			ambientSettingBox.TabStop = false;
			ambientSettingBox.Text = "Ambient";
			// 
			// ambientBUpDown
			// 
			ambientBUpDown.Location = new System.Drawing.Point(217, 83);
			ambientBUpDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ambientBUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			ambientBUpDown.Name = "ambientBUpDown";
			ambientBUpDown.Size = new System.Drawing.Size(73, 31);
			ambientBUpDown.TabIndex = 23;
			ambientBUpDown.ValueChanged += ambientBUpDown_ValueChanged;
			// 
			// ambientGUpDown
			// 
			ambientGUpDown.Location = new System.Drawing.Point(76, 84);
			ambientGUpDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ambientGUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			ambientGUpDown.Name = "ambientGUpDown";
			ambientGUpDown.Size = new System.Drawing.Size(73, 31);
			ambientGUpDown.TabIndex = 22;
			ambientGUpDown.ValueChanged += ambientGUpDown_ValueChanged;
			// 
			// ambientRUpDown
			// 
			ambientRUpDown.Location = new System.Drawing.Point(217, 40);
			ambientRUpDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ambientRUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			ambientRUpDown.Name = "ambientRUpDown";
			ambientRUpDown.Size = new System.Drawing.Size(73, 31);
			ambientRUpDown.TabIndex = 21;
			ambientRUpDown.ValueChanged += ambientRUpDown_ValueChanged;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(163, 86);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(49, 25);
			label5.TabIndex = 20;
			label5.Text = "Blue:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(9, 86);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(62, 25);
			label6.TabIndex = 19;
			label6.Text = "Green:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(166, 42);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(46, 25);
			label7.TabIndex = 18;
			label7.Text = "Red:";
			// 
			// specularSettingBox
			// 
			specularSettingBox.Controls.Add(specularBUpDown);
			specularSettingBox.Controls.Add(specularGUpDown);
			specularSettingBox.Controls.Add(specularRUpDown);
			specularSettingBox.Controls.Add(label8);
			specularSettingBox.Controls.Add(label9);
			specularSettingBox.Controls.Add(label10);
			specularSettingBox.Controls.Add(specColorLabel);
			specularSettingBox.Controls.Add(specColorBox);
			specularSettingBox.Controls.Add(exponentLabel);
			specularSettingBox.Controls.Add(exponentTextBox);
			specularSettingBox.Location = new System.Drawing.Point(347, 155);
			specularSettingBox.Name = "specularSettingBox";
			specularSettingBox.Size = new System.Drawing.Size(323, 181);
			specularSettingBox.TabIndex = 19;
			specularSettingBox.TabStop = false;
			specularSettingBox.Text = "Specular";
			// 
			// specularBUpDown
			// 
			specularBUpDown.Location = new System.Drawing.Point(76, 130);
			specularBUpDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			specularBUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			specularBUpDown.Name = "specularBUpDown";
			specularBUpDown.Size = new System.Drawing.Size(73, 31);
			specularBUpDown.TabIndex = 21;
			specularBUpDown.ValueChanged += specularBUpDown_ValueChanged;
			// 
			// specularGUpDown
			// 
			specularGUpDown.Location = new System.Drawing.Point(230, 82);
			specularGUpDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			specularGUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			specularGUpDown.Name = "specularGUpDown";
			specularGUpDown.Size = new System.Drawing.Size(73, 31);
			specularGUpDown.TabIndex = 20;
			specularGUpDown.ValueChanged += specularGUpDown_ValueChanged;
			// 
			// specularRUpDown
			// 
			specularRUpDown.Location = new System.Drawing.Point(76, 82);
			specularRUpDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			specularRUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			specularRUpDown.Name = "specularRUpDown";
			specularRUpDown.Size = new System.Drawing.Size(73, 31);
			specularRUpDown.TabIndex = 19;
			specularRUpDown.ValueChanged += specularRUpDown_ValueChanged;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(22, 133);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(49, 25);
			label8.TabIndex = 18;
			label8.Text = "Blue:";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(163, 84);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(62, 25);
			label9.TabIndex = 17;
			label9.Text = "Green:";
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(25, 84);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(46, 25);
			label10.TabIndex = 16;
			label10.Text = "Red:";
			// 
			// blendModeSettingsBox
			// 
			blendModeSettingsBox.Controls.Add(srcAlphaCombo);
			blendModeSettingsBox.Controls.Add(destinationAlphaLabel);
			blendModeSettingsBox.Controls.Add(dstAlphaCombo);
			blendModeSettingsBox.Controls.Add(srcAlphaLabel);
			blendModeSettingsBox.Location = new System.Drawing.Point(15, 201);
			blendModeSettingsBox.Name = "blendModeSettingsBox";
			blendModeSettingsBox.Size = new System.Drawing.Size(323, 198);
			blendModeSettingsBox.TabIndex = 20;
			blendModeSettingsBox.TabStop = false;
			blendModeSettingsBox.Text = "Blend Modes";
			// 
			// ChunkModelMaterialDataEditor
			// 
			AcceptButton = doneButton;
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(684, 424);
			ControlBox = false;
			Controls.Add(blendModeSettingsBox);
			Controls.Add(specularSettingBox);
			Controls.Add(ambientSettingBox);
			Controls.Add(resetButton);
			Controls.Add(doneButton);
			Controls.Add(diffuseSettingBox);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkModelMaterialDataEditor";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Poly Material Editor";
			Load += MaterialEditor_Load;
			diffuseSettingBox.ResumeLayout(false);
			diffuseSettingBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)diffuseBUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)diffuseGUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)diffuseRUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).EndInit();
			ambientSettingBox.ResumeLayout(false);
			ambientSettingBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)ambientBUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)ambientGUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)ambientRUpDown).EndInit();
			specularSettingBox.ResumeLayout(false);
			specularSettingBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)specularBUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)specularGUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)specularRUpDown).EndInit();
			blendModeSettingsBox.ResumeLayout(false);
			blendModeSettingsBox.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.GroupBox diffuseSettingBox;
        private System.Windows.Forms.Label exponentLabel;
        private System.Windows.Forms.Label specColorLabel;
        private System.Windows.Forms.Label diffuseLabel;
        private System.Windows.Forms.TextBox exponentTextBox;
        private System.Windows.Forms.Panel diffuseColorBox;
        private System.Windows.Forms.Panel specColorBox;
        private System.Windows.Forms.Label srcAlphaLabel;
        private System.Windows.Forms.ComboBox dstAlphaCombo;
        private System.Windows.Forms.Label destinationAlphaLabel;
        private System.Windows.Forms.ComboBox srcAlphaCombo;
        private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.NumericUpDown alphaDiffuseNumeric;
		private System.Windows.Forms.Label labelAlpha;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label ambientLabel;
		private System.Windows.Forms.Panel ambientColorBox;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.NumericUpDown diffuseRUpDown;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown diffuseBUpDown;
		private System.Windows.Forms.NumericUpDown diffuseGUpDown;
		private System.Windows.Forms.GroupBox ambientSettingBox;
		private System.Windows.Forms.NumericUpDown ambientBUpDown;
		private System.Windows.Forms.NumericUpDown ambientGUpDown;
		private System.Windows.Forms.NumericUpDown ambientRUpDown;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox specularSettingBox;
		private System.Windows.Forms.NumericUpDown specularBUpDown;
		private System.Windows.Forms.NumericUpDown specularGUpDown;
		private System.Windows.Forms.NumericUpDown specularRUpDown;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox blendModeSettingsBox;
	}
}