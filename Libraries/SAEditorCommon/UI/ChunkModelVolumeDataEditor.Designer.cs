namespace SAModel.SAEditorCommon.UI
{
    partial class ChunkModelVolumeDataEditor
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
			blendModeSettingsBox = new System.Windows.Forms.GroupBox();
			listViewVolumeData = new System.Windows.Forms.ListView();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			columnHeader2 = new System.Windows.Forms.ColumnHeader();
			volumeTypeLabel = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			blendModeSettingsBox.SuspendLayout();
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
			doneButton.Location = new System.Drawing.Point(268, 256);
			doneButton.Name = "doneButton";
			doneButton.Size = new System.Drawing.Size(59, 27);
			doneButton.TabIndex = 4;
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
			resetButton.Location = new System.Drawing.Point(191, 256);
			resetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(55, 27);
			resetButton.TabIndex = 3;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the poly data to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// blendModeSettingsBox
			// 
			blendModeSettingsBox.Controls.Add(listViewVolumeData);
			blendModeSettingsBox.Controls.Add(volumeTypeLabel);
			blendModeSettingsBox.Controls.Add(label1);
			blendModeSettingsBox.Location = new System.Drawing.Point(10, 15);
			blendModeSettingsBox.Margin = new System.Windows.Forms.Padding(2);
			blendModeSettingsBox.Name = "blendModeSettingsBox";
			blendModeSettingsBox.Padding = new System.Windows.Forms.Padding(2);
			blendModeSettingsBox.Size = new System.Drawing.Size(316, 236);
			blendModeSettingsBox.TabIndex = 1;
			blendModeSettingsBox.TabStop = false;
			blendModeSettingsBox.Text = "Volume Data";
			// 
			// listViewVolumeData
			// 
			listViewVolumeData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2 });
			listViewVolumeData.GridLines = true;
			listViewVolumeData.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewVolumeData.Location = new System.Drawing.Point(5, 45);
			listViewVolumeData.MultiSelect = false;
			listViewVolumeData.Name = "listViewVolumeData";
			listViewVolumeData.Size = new System.Drawing.Size(306, 186);
			listViewVolumeData.TabIndex = 2;
			listViewVolumeData.UseCompatibleStateImageBehavior = false;
			listViewVolumeData.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			columnHeader1.Text = "ID";
			// 
			// columnHeader2
			// 
			columnHeader2.Text = "Data";
			// 
			// volumeTypeLabel
			// 
			volumeTypeLabel.AutoSize = true;
			volumeTypeLabel.Location = new System.Drawing.Point(89, 27);
			volumeTypeLabel.Name = "volumeTypeLabel";
			volumeTypeLabel.Size = new System.Drawing.Size(30, 15);
			volumeTypeLabel.TabIndex = 1;
			volumeTypeLabel.Text = "type";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(5, 27);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(78, 15);
			label1.TabIndex = 0;
			label1.Text = "Volume Type:";
			// 
			// ChunkModelVolumeDataEditor
			// 
			AcceptButton = doneButton;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(337, 297);
			ControlBox = false;
			Controls.Add(blendModeSettingsBox);
			Controls.Add(resetButton);
			Controls.Add(doneButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkModelVolumeDataEditor";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Modifier Volume Editor";
			Load += MaterialEditor_Load;
			blendModeSettingsBox.ResumeLayout(false);
			blendModeSettingsBox.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.GroupBox blendModeSettingsBox;
		private System.Windows.Forms.Label volumeTypeLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listViewVolumeData;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
	}
}