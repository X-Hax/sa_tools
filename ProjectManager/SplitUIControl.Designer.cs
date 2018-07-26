namespace ProjectManager
{
    partial class SplitUIControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.controlGroupBox = new System.Windows.Forms.GroupBox();
            this.splitTypeComboBox = new System.Windows.Forms.ComboBox();
            this.bigEndianCheckbox = new System.Windows.Forms.CheckBox();
            this.outputFolderBrowseButton = new System.Windows.Forms.Button();
            this.outputFolderPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataMappingBrowseButton = new System.Windows.Forms.Button();
            this.dataMappingPathText = new System.Windows.Forms.TextBox();
            this.DataMappingLabel = new System.Windows.Forms.Label();
            this.fileBrowseButton = new System.Windows.Forms.Button();
            this.filePathText = new System.Windows.Forms.TextBox();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.AddJobButton = new System.Windows.Forms.Button();
            this.removeSplitJob = new System.Windows.Forms.Button();
            this.animFilesList = new System.Windows.Forms.ListView();
            this.AddNewAnimFileButton = new System.Windows.Forms.Button();
            this.RemoveAnimButton = new System.Windows.Forms.Button();
            this.controlGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlGroupBox
            // 
            this.controlGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.controlGroupBox.Controls.Add(this.RemoveAnimButton);
            this.controlGroupBox.Controls.Add(this.AddNewAnimFileButton);
            this.controlGroupBox.Controls.Add(this.animFilesList);
            this.controlGroupBox.Controls.Add(this.splitTypeComboBox);
            this.controlGroupBox.Controls.Add(this.bigEndianCheckbox);
            this.controlGroupBox.Controls.Add(this.outputFolderBrowseButton);
            this.controlGroupBox.Controls.Add(this.outputFolderPath);
            this.controlGroupBox.Controls.Add(this.label1);
            this.controlGroupBox.Controls.Add(this.dataMappingBrowseButton);
            this.controlGroupBox.Controls.Add(this.dataMappingPathText);
            this.controlGroupBox.Controls.Add(this.DataMappingLabel);
            this.controlGroupBox.Controls.Add(this.fileBrowseButton);
            this.controlGroupBox.Controls.Add(this.filePathText);
            this.controlGroupBox.Controls.Add(this.fileNameLabel);
            this.controlGroupBox.Location = new System.Drawing.Point(0, 0);
            this.controlGroupBox.Name = "controlGroupBox";
            this.controlGroupBox.Size = new System.Drawing.Size(380, 358);
            this.controlGroupBox.TabIndex = 0;
            this.controlGroupBox.TabStop = false;
            this.controlGroupBox.Text = "Split Data File";
            // 
            // splitTypeComboBox
            // 
            this.splitTypeComboBox.FormattingEnabled = true;
            this.splitTypeComboBox.Items.AddRange(new object[] {
            "Bin || DLL (sonic.exe, 1st_read.bin, CHRMODELS.dll, etc)",
            "MDL"});
            this.splitTypeComboBox.Location = new System.Drawing.Point(245, 15);
            this.splitTypeComboBox.Name = "splitTypeComboBox";
            this.splitTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.splitTypeComboBox.TabIndex = 10;
            this.splitTypeComboBox.DropDownClosed += new System.EventHandler(this.splitTypeComboBox_DropDownClosed);
            // 
            // bigEndianCheckbox
            // 
            this.bigEndianCheckbox.AutoSize = true;
            this.bigEndianCheckbox.Location = new System.Drawing.Point(43, 72);
            this.bigEndianCheckbox.Name = "bigEndianCheckbox";
            this.bigEndianCheckbox.Size = new System.Drawing.Size(242, 17);
            this.bigEndianCheckbox.TabIndex = 9;
            this.bigEndianCheckbox.Text = "Big Endian (GC Games and SA2PC MDL files)";
            this.bigEndianCheckbox.UseVisualStyleBackColor = true;
            this.bigEndianCheckbox.CheckedChanged += new System.EventHandler(this.bigEndianCheckbox_CheckedChanged);
            // 
            // outputFolderBrowseButton
            // 
            this.outputFolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFolderBrowseButton.Location = new System.Drawing.Point(291, 140);
            this.outputFolderBrowseButton.Name = "outputFolderBrowseButton";
            this.outputFolderBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.outputFolderBrowseButton.TabIndex = 8;
            this.outputFolderBrowseButton.Text = "Browse";
            this.outputFolderBrowseButton.UseVisualStyleBackColor = true;
            this.outputFolderBrowseButton.Click += new System.EventHandler(this.outputFolderBrowseButton_Click);
            // 
            // outputFolderPath
            // 
            this.outputFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFolderPath.Location = new System.Drawing.Point(19, 114);
            this.outputFolderPath.Name = "outputFolderPath";
            this.outputFolderPath.Size = new System.Drawing.Size(347, 20);
            this.outputFolderPath.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Output Folder:";
            // 
            // dataMappingBrowseButton
            // 
            this.dataMappingBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dataMappingBrowseButton.Location = new System.Drawing.Point(291, 205);
            this.dataMappingBrowseButton.Name = "dataMappingBrowseButton";
            this.dataMappingBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.dataMappingBrowseButton.TabIndex = 5;
            this.dataMappingBrowseButton.Text = "Browse";
            this.dataMappingBrowseButton.UseVisualStyleBackColor = true;
            this.dataMappingBrowseButton.Click += new System.EventHandler(this.dataMappingBrowseButton_Click);
            // 
            // dataMappingPathText
            // 
            this.dataMappingPathText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataMappingPathText.Location = new System.Drawing.Point(19, 179);
            this.dataMappingPathText.Name = "dataMappingPathText";
            this.dataMappingPathText.Size = new System.Drawing.Size(347, 20);
            this.dataMappingPathText.TabIndex = 4;
            // 
            // DataMappingLabel
            // 
            this.DataMappingLabel.AutoSize = true;
            this.DataMappingLabel.Location = new System.Drawing.Point(19, 160);
            this.DataMappingLabel.Name = "DataMappingLabel";
            this.DataMappingLabel.Size = new System.Drawing.Size(77, 13);
            this.DataMappingLabel.TabIndex = 3;
            this.DataMappingLabel.Text = "Data Mapping:";
            // 
            // fileBrowseButton
            // 
            this.fileBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileBrowseButton.Location = new System.Drawing.Point(291, 68);
            this.fileBrowseButton.Name = "fileBrowseButton";
            this.fileBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.fileBrowseButton.TabIndex = 2;
            this.fileBrowseButton.Text = "Browse";
            this.fileBrowseButton.UseVisualStyleBackColor = true;
            this.fileBrowseButton.Click += new System.EventHandler(this.fileBrowseButton_Click);
            // 
            // filePathText
            // 
            this.filePathText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathText.Location = new System.Drawing.Point(19, 42);
            this.filePathText.Name = "filePathText";
            this.filePathText.Size = new System.Drawing.Size(347, 20);
            this.filePathText.TabIndex = 1;
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.Location = new System.Drawing.Point(19, 22);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(26, 13);
            this.fileNameLabel.TabIndex = 0;
            this.fileNameLabel.Text = "File:";
            // 
            // AddJobButton
            // 
            this.AddJobButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddJobButton.Location = new System.Drawing.Point(261, 374);
            this.AddJobButton.Name = "AddJobButton";
            this.AddJobButton.Size = new System.Drawing.Size(105, 23);
            this.AddJobButton.TabIndex = 1;
            this.AddJobButton.Text = "Add Split Job";
            this.AddJobButton.UseVisualStyleBackColor = true;
            this.AddJobButton.Click += new System.EventHandler(this.AddJobButton_Click);
            // 
            // removeSplitJob
            // 
            this.removeSplitJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeSplitJob.Location = new System.Drawing.Point(19, 374);
            this.removeSplitJob.Name = "removeSplitJob";
            this.removeSplitJob.Size = new System.Drawing.Size(125, 23);
            this.removeSplitJob.TabIndex = 2;
            this.removeSplitJob.Text = "Remove This Split Job";
            this.removeSplitJob.UseVisualStyleBackColor = true;
            this.removeSplitJob.Click += new System.EventHandler(this.removeSplitJob_Click);
            // 
            // animFilesList
            // 
            this.animFilesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animFilesList.Enabled = false;
            this.animFilesList.Location = new System.Drawing.Point(19, 179);
            this.animFilesList.Name = "animFilesList";
            this.animFilesList.Size = new System.Drawing.Size(347, 127);
            this.animFilesList.TabIndex = 3;
            this.animFilesList.UseCompatibleStateImageBehavior = false;
            this.animFilesList.View = System.Windows.Forms.View.List;
            this.animFilesList.Visible = false;
            this.animFilesList.SelectedIndexChanged += new System.EventHandler(this.animFilesList_SelectedIndexChanged);
            // 
            // AddNewAnimFileButton
            // 
            this.AddNewAnimFileButton.Enabled = false;
            this.AddNewAnimFileButton.Location = new System.Drawing.Point(291, 320);
            this.AddNewAnimFileButton.Name = "AddNewAnimFileButton";
            this.AddNewAnimFileButton.Size = new System.Drawing.Size(75, 23);
            this.AddNewAnimFileButton.TabIndex = 11;
            this.AddNewAnimFileButton.Text = "Add Anim";
            this.AddNewAnimFileButton.UseVisualStyleBackColor = true;
            this.AddNewAnimFileButton.Visible = false;
            this.AddNewAnimFileButton.Click += new System.EventHandler(this.AddNewAnimFileButton_Click);
            // 
            // RemoveAnimButton
            // 
            this.RemoveAnimButton.Enabled = false;
            this.RemoveAnimButton.Location = new System.Drawing.Point(19, 320);
            this.RemoveAnimButton.Name = "RemoveAnimButton";
            this.RemoveAnimButton.Size = new System.Drawing.Size(107, 23);
            this.RemoveAnimButton.TabIndex = 12;
            this.RemoveAnimButton.Text = "Remove Anim";
            this.RemoveAnimButton.UseVisualStyleBackColor = true;
            this.RemoveAnimButton.Visible = false;
            this.RemoveAnimButton.Click += new System.EventHandler(this.RemoveAnimButton_Click);
            // 
            // SplitUIControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.removeSplitJob);
            this.Controls.Add(this.AddJobButton);
            this.Controls.Add(this.controlGroupBox);
            this.Name = "SplitUIControl";
            this.Size = new System.Drawing.Size(380, 414);
            this.controlGroupBox.ResumeLayout(false);
            this.controlGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox controlGroupBox;
        private System.Windows.Forms.Button outputFolderBrowseButton;
        private System.Windows.Forms.TextBox outputFolderPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button dataMappingBrowseButton;
        private System.Windows.Forms.TextBox dataMappingPathText;
        private System.Windows.Forms.Label DataMappingLabel;
        private System.Windows.Forms.Button fileBrowseButton;
        private System.Windows.Forms.TextBox filePathText;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.Button AddJobButton;
        private System.Windows.Forms.Button removeSplitJob;
        private System.Windows.Forms.CheckBox bigEndianCheckbox;
        private System.Windows.Forms.ComboBox splitTypeComboBox;
        private System.Windows.Forms.Button RemoveAnimButton;
        private System.Windows.Forms.Button AddNewAnimFileButton;
        private System.Windows.Forms.ListView animFilesList;
    }
}
