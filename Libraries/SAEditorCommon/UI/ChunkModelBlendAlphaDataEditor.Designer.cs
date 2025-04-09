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
			dstAlphaCombo.Location = new System.Drawing.Point(34, 130);
			dstAlphaCombo.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			dstAlphaCombo.Name = "dstAlphaCombo";
			dstAlphaCombo.Size = new System.Drawing.Size(244, 33);
			dstAlphaCombo.TabIndex = 8;
			dstAlphaCombo.SelectedIndexChanged += dstAlphaCombo_SelectedIndexChanged;
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
			// doneButton
			// 
			doneButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			doneButton.Location = new System.Drawing.Point(209, 242);
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
			resetButton.Location = new System.Drawing.Point(50, 242);
			resetButton.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(132, 40);
			resetButton.TabIndex = 6;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the poly data to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// blendModeSettingsBox
			// 
			blendModeSettingsBox.Controls.Add(srcAlphaCombo);
			blendModeSettingsBox.Controls.Add(destinationAlphaLabel);
			blendModeSettingsBox.Controls.Add(dstAlphaCombo);
			blendModeSettingsBox.Controls.Add(srcAlphaLabel);
			blendModeSettingsBox.Location = new System.Drawing.Point(15, 22);
			blendModeSettingsBox.Name = "blendModeSettingsBox";
			blendModeSettingsBox.Size = new System.Drawing.Size(323, 198);
			blendModeSettingsBox.TabIndex = 20;
			blendModeSettingsBox.TabStop = false;
			blendModeSettingsBox.Text = "Blend Modes";
			// 
			// ChunkModelBlendAlphaDataEditor
			// 
			AcceptButton = doneButton;
			AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(356, 303);
			ControlBox = false;
			Controls.Add(blendModeSettingsBox);
			Controls.Add(resetButton);
			Controls.Add(doneButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
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
	}
}