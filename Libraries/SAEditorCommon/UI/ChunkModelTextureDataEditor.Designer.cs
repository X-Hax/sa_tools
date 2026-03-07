namespace SAModel.SAEditorCommon.UI
{
    partial class ChunkModelTextureDataEditor
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
			doneButton = new System.Windows.Forms.Button();
			toolTip = new System.Windows.Forms.ToolTip(components);
			resetButton = new System.Windows.Forms.Button();
			clampUCheck = new System.Windows.Forms.CheckBox();
			clampVCheck = new System.Windows.Forms.CheckBox();
			flipUCheck = new System.Windows.Forms.CheckBox();
			flipVCheck = new System.Windows.Forms.CheckBox();
			filterModeDropDown = new System.Windows.Forms.ComboBox();
			mipmapDropDown = new System.Windows.Forms.ComboBox();
			superSampleCheck = new System.Windows.Forms.CheckBox();
			textureBox = new System.Windows.Forms.PictureBox();
			filterModeLabel = new System.Windows.Forms.Label();
			generalSettingBox = new System.Windows.Forms.GroupBox();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			textureIDNumeric = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)textureBox).BeginInit();
			generalSettingBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)textureIDNumeric).BeginInit();
			SuspendLayout();
			// 
			// colorDialog
			// 
			colorDialog.AnyColor = true;
			colorDialog.FullOpen = true;
			colorDialog.SolidColorOnly = true;
			// 
			// doneButton
			// 
			doneButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			doneButton.Location = new System.Drawing.Point(397, 391);
			doneButton.Margin = new System.Windows.Forms.Padding(4);
			doneButton.Name = "doneButton";
			doneButton.Size = new System.Drawing.Size(132, 40);
			doneButton.TabIndex = 10;
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
			resetButton.Location = new System.Drawing.Point(238, 391);
			resetButton.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(132, 40);
			resetButton.TabIndex = 9;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the poly data to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// clampUCheck
			// 
			clampUCheck.AutoSize = true;
			clampUCheck.Location = new System.Drawing.Point(243, 64);
			clampUCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			clampUCheck.Name = "clampUCheck";
			clampUCheck.Size = new System.Drawing.Size(106, 29);
			clampUCheck.TabIndex = 3;
			clampUCheck.Text = "Clamp U";
			toolTip.SetToolTip(clampUCheck, "Enable/Disable tiling on the U Axis.");
			clampUCheck.UseVisualStyleBackColor = true;
			clampUCheck.Click += clampUCheck_Click;
			// 
			// clampVCheck
			// 
			clampVCheck.AutoSize = true;
			clampVCheck.Location = new System.Drawing.Point(243, 96);
			clampVCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			clampVCheck.Name = "clampVCheck";
			clampVCheck.Size = new System.Drawing.Size(105, 29);
			clampVCheck.TabIndex = 4;
			clampVCheck.Text = "Clamp V";
			toolTip.SetToolTip(clampVCheck, "Enable/Disable tiling on the V Axis.");
			clampVCheck.UseVisualStyleBackColor = true;
			clampVCheck.Click += clampVCheck_Click;
			// 
			// flipUCheck
			// 
			flipUCheck.AutoSize = true;
			flipUCheck.Location = new System.Drawing.Point(243, 128);
			flipUCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flipUCheck.Name = "flipUCheck";
			flipUCheck.Size = new System.Drawing.Size(104, 29);
			flipUCheck.TabIndex = 5;
			flipUCheck.Text = "Mirror U";
			toolTip.SetToolTip(flipUCheck, "If checked, tiling on the U Axis is mirrored.");
			flipUCheck.UseVisualStyleBackColor = true;
			flipUCheck.Click += flipUCheck_Click;
			// 
			// flipVCheck
			// 
			flipVCheck.AutoSize = true;
			flipVCheck.Location = new System.Drawing.Point(243, 160);
			flipVCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flipVCheck.Name = "flipVCheck";
			flipVCheck.Size = new System.Drawing.Size(103, 29);
			flipVCheck.TabIndex = 6;
			flipVCheck.Text = "Mirror V";
			toolTip.SetToolTip(flipVCheck, "If checked, tiling on the V Axis is mirrored.");
			flipVCheck.UseVisualStyleBackColor = true;
			flipVCheck.Click += flipVCheck_Click;
			// 
			// filterModeDropDown
			// 
			filterModeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			filterModeDropDown.FormattingEnabled = true;
			filterModeDropDown.Items.AddRange(new object[] { "Point Sampled", "Bilinear", "Trilinear A", "Trilinear B" });
			filterModeDropDown.Location = new System.Drawing.Point(243, 220);
			filterModeDropDown.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			filterModeDropDown.Name = "filterModeDropDown";
			filterModeDropDown.Size = new System.Drawing.Size(244, 33);
			filterModeDropDown.TabIndex = 7;
			toolTip.SetToolTip(filterModeDropDown, "Determines the texture filter mode used by the poly data.\r\nWhile this value is read in SA2B, all settings are treated as if they were Trilinear A without the Render Fix mod enabled.");
			filterModeDropDown.SelectionChangeCommitted += filterModeDropDown_SelectionChangeCommitted;
			// 
			// mipmapDropDown
			// 
			mipmapDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			mipmapDropDown.FormattingEnabled = true;
			mipmapDropDown.Items.AddRange(new object[] { "0 (Invalid)", "0.25", "0.50", "0.75", "1.00", "1.25", "1.50", "1.75", "2.00", "2.25", "2.50", "2.75", "3.00", "3.25", "3.50", "3.75" });
			mipmapDropDown.Location = new System.Drawing.Point(243, 290);
			mipmapDropDown.Name = "mipmapDropDown";
			mipmapDropDown.Size = new System.Drawing.Size(135, 33);
			mipmapDropDown.TabIndex = 8;
			toolTip.SetToolTip(mipmapDropDown, "This value adjusts the depth threshold for mipmap generation by using this value as a multiplier.\r\nA value of 0 is not valid.\r\nDoes nothing in SA2B without the Render Fix mod. ");
			mipmapDropDown.SelectionChangeCommitted += mipmapDropDown_SelectionChangeCommitted;
			// 
			// superSampleCheck
			// 
			superSampleCheck.AutoSize = true;
			superSampleCheck.Location = new System.Drawing.Point(243, 32);
			superSampleCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			superSampleCheck.Name = "superSampleCheck";
			superSampleCheck.Size = new System.Drawing.Size(148, 29);
			superSampleCheck.TabIndex = 2;
			superSampleCheck.Text = "Super Sample";
			superSampleCheck.UseVisualStyleBackColor = true;
			superSampleCheck.Click += superSampleCheck_Click;
			// 
			// textureBox
			// 
			textureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			textureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			textureBox.Location = new System.Drawing.Point(21, 36);
			textureBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			textureBox.Name = "textureBox";
			textureBox.Size = new System.Drawing.Size(203, 203);
			textureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			textureBox.TabIndex = 4;
			textureBox.TabStop = false;
			textureBox.Click += textureBox_Click;
			// 
			// filterModeLabel
			// 
			filterModeLabel.AutoSize = true;
			filterModeLabel.Location = new System.Drawing.Point(243, 192);
			filterModeLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			filterModeLabel.Name = "filterModeLabel";
			filterModeLabel.Size = new System.Drawing.Size(106, 25);
			filterModeLabel.TabIndex = 9;
			filterModeLabel.Text = "Filter Mode:";
			// 
			// generalSettingBox
			// 
			generalSettingBox.Controls.Add(mipmapDropDown);
			generalSettingBox.Controls.Add(label2);
			generalSettingBox.Controls.Add(label1);
			generalSettingBox.Controls.Add(textureIDNumeric);
			generalSettingBox.Controls.Add(flipVCheck);
			generalSettingBox.Controls.Add(filterModeLabel);
			generalSettingBox.Controls.Add(flipUCheck);
			generalSettingBox.Controls.Add(textureBox);
			generalSettingBox.Controls.Add(clampVCheck);
			generalSettingBox.Controls.Add(filterModeDropDown);
			generalSettingBox.Controls.Add(clampUCheck);
			generalSettingBox.Controls.Add(superSampleCheck);
			generalSettingBox.Location = new System.Drawing.Point(15, 13);
			generalSettingBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			generalSettingBox.Name = "generalSettingBox";
			generalSettingBox.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
			generalSettingBox.Size = new System.Drawing.Size(504, 354);
			generalSettingBox.TabIndex = 0;
			generalSettingBox.TabStop = false;
			generalSettingBox.Enter += generalSettingBox_Enter;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(243, 258);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(171, 25);
			label2.TabIndex = 12;
			label2.Text = "Mipmap \"D\" Adjust:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(9, 256);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(94, 25);
			label1.TabIndex = 11;
			label1.Text = "Texture ID:";
			// 
			// textureIDNumeric
			// 
			textureIDNumeric.Location = new System.Drawing.Point(110, 254);
			textureIDNumeric.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			textureIDNumeric.Name = "textureIDNumeric";
			textureIDNumeric.Size = new System.Drawing.Size(66, 31);
			textureIDNumeric.TabIndex = 1;
			textureIDNumeric.ValueChanged += textureIDNumeric_ValueChanged;
			// 
			// ChunkModelTextureDataEditor
			// 
			AcceptButton = doneButton;
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(544, 452);
			ControlBox = false;
			Controls.Add(resetButton);
			Controls.Add(doneButton);
			Controls.Add(generalSettingBox);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkModelTextureDataEditor";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Texture Data Editor";
			Load += MaterialEditor_Load;
			((System.ComponentModel.ISupportInitialize)textureBox).EndInit();
			generalSettingBox.ResumeLayout(false);
			generalSettingBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)textureIDNumeric).EndInit();
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.CheckBox superSampleCheck;
		private System.Windows.Forms.CheckBox clampUCheck;
		private System.Windows.Forms.ComboBox filterModeDropDown;
		private System.Windows.Forms.CheckBox clampVCheck;
		private System.Windows.Forms.PictureBox textureBox;
		private System.Windows.Forms.CheckBox flipUCheck;
		private System.Windows.Forms.Label filterModeLabel;
		private System.Windows.Forms.CheckBox flipVCheck;
		private System.Windows.Forms.GroupBox generalSettingBox;
		private System.Windows.Forms.NumericUpDown textureIDNumeric;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox mipmapDropDown;
	}
}