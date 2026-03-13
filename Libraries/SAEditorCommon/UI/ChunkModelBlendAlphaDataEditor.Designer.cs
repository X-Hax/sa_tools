namespace SAModel.SAEditorCommon.UI
{
    partial class ChunkModelBlendAlphaDataEditor
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
			dstAlphaCombo = new System.Windows.Forms.ComboBox();
			destinationAlphaLabel = new System.Windows.Forms.Label();
			srcAlphaCombo = new System.Windows.Forms.ComboBox();
			srcAlphaLabel = new System.Windows.Forms.Label();
			doneButton = new System.Windows.Forms.Button();
			toolTip = new System.Windows.Forms.ToolTip(components);
			resetButton = new System.Windows.Forms.Button();
			blendModeSettingsBox = new System.Windows.Forms.GroupBox();
			checkBoxDestBuffer = new System.Windows.Forms.CheckBox();
			checkBoxSourceBuffer = new System.Windows.Forms.CheckBox();
			blendModeSettingsBox.SuspendLayout();
			SuspendLayout();
			// 
			// colorDialog
			// 
			colorDialog.AnyColor = true;
			colorDialog.FullOpen = true;
			colorDialog.SolidColorOnly = true;
			// 
			// dstAlphaCombo
			// 
			dstAlphaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			dstAlphaCombo.FormattingEnabled = true;
			dstAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "OtherColor", "InverseOtherColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			dstAlphaCombo.Location = new System.Drawing.Point(23, 87);
			dstAlphaCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			dstAlphaCombo.Name = "dstAlphaCombo";
			dstAlphaCombo.Size = new System.Drawing.Size(164, 23);
			dstAlphaCombo.TabIndex = 8;
			dstAlphaCombo.SelectedIndexChanged += dstAlphaCombo_SelectedIndexChanged;
			// 
			// destinationAlphaLabel
			// 
			destinationAlphaLabel.AutoSize = true;
			destinationAlphaLabel.Location = new System.Drawing.Point(19, 69);
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
			srcAlphaCombo.Location = new System.Drawing.Point(23, 41);
			srcAlphaCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			srcAlphaCombo.Name = "srcAlphaCombo";
			srcAlphaCombo.Size = new System.Drawing.Size(164, 23);
			srcAlphaCombo.TabIndex = 7;
			srcAlphaCombo.SelectionChangeCommitted += srcAlphaCombo_SelectionChangeCommitted;
			// 
			// srcAlphaLabel
			// 
			srcAlphaLabel.AutoSize = true;
			srcAlphaLabel.Location = new System.Drawing.Point(19, 24);
			srcAlphaLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			srcAlphaLabel.Name = "srcAlphaLabel";
			srcAlphaLabel.Size = new System.Drawing.Size(80, 15);
			srcAlphaLabel.TabIndex = 11;
			srcAlphaLabel.Text = "Source Alpha:";
			// 
			// doneButton
			// 
			doneButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			doneButton.Location = new System.Drawing.Point(202, 215);
			doneButton.Name = "doneButton";
			doneButton.Size = new System.Drawing.Size(88, 27);
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
			resetButton.Location = new System.Drawing.Point(96, 215);
			resetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(88, 27);
			resetButton.TabIndex = 6;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the poly data to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// blendModeSettingsBox
			// 
			blendModeSettingsBox.Controls.Add(checkBoxDestBuffer);
			blendModeSettingsBox.Controls.Add(checkBoxSourceBuffer);
			blendModeSettingsBox.Controls.Add(srcAlphaCombo);
			blendModeSettingsBox.Controls.Add(destinationAlphaLabel);
			blendModeSettingsBox.Controls.Add(dstAlphaCombo);
			blendModeSettingsBox.Controls.Add(srcAlphaLabel);
			blendModeSettingsBox.Location = new System.Drawing.Point(10, 15);
			blendModeSettingsBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			blendModeSettingsBox.Name = "blendModeSettingsBox";
			blendModeSettingsBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
			blendModeSettingsBox.Size = new System.Drawing.Size(279, 182);
			blendModeSettingsBox.TabIndex = 20;
			blendModeSettingsBox.TabStop = false;
			blendModeSettingsBox.Text = "Blend Modes";
			// 
			// checkBoxDestBuffer
			// 
			checkBoxDestBuffer.AutoSize = true;
			checkBoxDestBuffer.Location = new System.Drawing.Point(19, 141);
			checkBoxDestBuffer.Name = "checkBoxDestBuffer";
			checkBoxDestBuffer.Size = new System.Drawing.Size(155, 19);
			checkBoxDestBuffer.TabIndex = 17;
			checkBoxDestBuffer.Text = "Destination Buffer Select";
			checkBoxDestBuffer.UseVisualStyleBackColor = true;
			checkBoxDestBuffer.Click += checkBoxDestBuffer_Click;
			// 
			// checkBoxSourceBuffer
			// 
			checkBoxSourceBuffer.AutoSize = true;
			checkBoxSourceBuffer.Location = new System.Drawing.Point(19, 116);
			checkBoxSourceBuffer.Name = "checkBoxSourceBuffer";
			checkBoxSourceBuffer.Size = new System.Drawing.Size(131, 19);
			checkBoxSourceBuffer.TabIndex = 16;
			checkBoxSourceBuffer.Text = "Source Buffer Select";
			checkBoxSourceBuffer.UseVisualStyleBackColor = true;
			checkBoxSourceBuffer.Click += checkBoxSourceBuffer_Click;
			// 
			// ChunkModelBlendAlphaDataEditor
			// 
			AcceptButton = doneButton;
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(300, 256);
			ControlBox = false;
			Controls.Add(blendModeSettingsBox);
			Controls.Add(resetButton);
			Controls.Add(doneButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkModelBlendAlphaDataEditor";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Blend Alpha Bits Editor";
			Load += MaterialEditor_Load;
			blendModeSettingsBox.ResumeLayout(false);
			blendModeSettingsBox.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label srcAlphaLabel;
        private System.Windows.Forms.ComboBox dstAlphaCombo;
        private System.Windows.Forms.Label destinationAlphaLabel;
        private System.Windows.Forms.ComboBox srcAlphaCombo;
        private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.GroupBox blendModeSettingsBox;
		private System.Windows.Forms.CheckBox checkBoxDestBuffer;
		private System.Windows.Forms.CheckBox checkBoxSourceBuffer;
	}
}