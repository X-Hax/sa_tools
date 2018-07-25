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
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.filePathText = new System.Windows.Forms.TextBox();
            this.fileBrowseButton = new System.Windows.Forms.Button();
            this.DataMappingLabel = new System.Windows.Forms.Label();
            this.dataMappingPathText = new System.Windows.Forms.TextBox();
            this.dataMappingBrowseButton = new System.Windows.Forms.Button();
            this.outputFolderBrowseButton = new System.Windows.Forms.Button();
            this.outputFolderPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AddJobButton = new System.Windows.Forms.Button();
            this.removeSplitJob = new System.Windows.Forms.Button();
            this.bigEndianCheckbox = new System.Windows.Forms.CheckBox();
            this.splitTypeComboBox = new System.Windows.Forms.ComboBox();
            this.controlGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlGroupBox
            // 
            this.controlGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.controlGroupBox.Size = new System.Drawing.Size(380, 243);
            this.controlGroupBox.TabIndex = 0;
            this.controlGroupBox.TabStop = false;
            this.controlGroupBox.Text = "Split Data File";
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
            // filePathText
            // 
            this.filePathText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathText.Location = new System.Drawing.Point(19, 42);
            this.filePathText.Name = "filePathText";
            this.filePathText.Size = new System.Drawing.Size(347, 20);
            this.filePathText.TabIndex = 1;
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
            // DataMappingLabel
            // 
            this.DataMappingLabel.AutoSize = true;
            this.DataMappingLabel.Location = new System.Drawing.Point(19, 100);
            this.DataMappingLabel.Name = "DataMappingLabel";
            this.DataMappingLabel.Size = new System.Drawing.Size(77, 13);
            this.DataMappingLabel.TabIndex = 3;
            this.DataMappingLabel.Text = "Data Mapping:";
            // 
            // dataMappingPathText
            // 
            this.dataMappingPathText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataMappingPathText.Location = new System.Drawing.Point(19, 119);
            this.dataMappingPathText.Name = "dataMappingPathText";
            this.dataMappingPathText.Size = new System.Drawing.Size(347, 20);
            this.dataMappingPathText.TabIndex = 4;
            // 
            // dataMappingBrowseButton
            // 
            this.dataMappingBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dataMappingBrowseButton.Location = new System.Drawing.Point(291, 145);
            this.dataMappingBrowseButton.Name = "dataMappingBrowseButton";
            this.dataMappingBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.dataMappingBrowseButton.TabIndex = 5;
            this.dataMappingBrowseButton.Text = "Browse";
            this.dataMappingBrowseButton.UseVisualStyleBackColor = true;
            this.dataMappingBrowseButton.Click += new System.EventHandler(this.dataMappingBrowseButton_Click);
            // 
            // outputFolderBrowseButton
            // 
            this.outputFolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFolderBrowseButton.Location = new System.Drawing.Point(291, 212);
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
            this.outputFolderPath.Location = new System.Drawing.Point(19, 186);
            this.outputFolderPath.Name = "outputFolderPath";
            this.outputFolderPath.Size = new System.Drawing.Size(347, 20);
            this.outputFolderPath.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Output Folder:";
            // 
            // AddJobButton
            // 
            this.AddJobButton.Location = new System.Drawing.Point(261, 259);
            this.AddJobButton.Name = "AddJobButton";
            this.AddJobButton.Size = new System.Drawing.Size(105, 23);
            this.AddJobButton.TabIndex = 1;
            this.AddJobButton.Text = "Add Split Job";
            this.AddJobButton.UseVisualStyleBackColor = true;
            this.AddJobButton.Click += new System.EventHandler(this.AddJobButton_Click);
            // 
            // removeSplitJob
            // 
            this.removeSplitJob.Location = new System.Drawing.Point(19, 259);
            this.removeSplitJob.Name = "removeSplitJob";
            this.removeSplitJob.Size = new System.Drawing.Size(125, 23);
            this.removeSplitJob.TabIndex = 2;
            this.removeSplitJob.Text = "Remove This Split Job";
            this.removeSplitJob.UseVisualStyleBackColor = true;
            this.removeSplitJob.Click += new System.EventHandler(this.removeSplitJob_Click);
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
            // 
            // splitTypeComboBox
            // 
            this.splitTypeComboBox.FormattingEnabled = true;
            this.splitTypeComboBox.Items.AddRange(new object[] {
            "Bin || DLL (sonic.exe, 1st_read.bin, CHRMODELS.dll, etc)",
            "MDL",
            "MTN",
            "NB"});
            this.splitTypeComboBox.Location = new System.Drawing.Point(245, 15);
            this.splitTypeComboBox.Name = "splitTypeComboBox";
            this.splitTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.splitTypeComboBox.TabIndex = 10;
            this.splitTypeComboBox.DropDownClosed += new System.EventHandler(this.splitTypeComboBox_DropDownClosed);
            // 
            // SplitUIControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.removeSplitJob);
            this.Controls.Add(this.AddJobButton);
            this.Controls.Add(this.controlGroupBox);
            this.Name = "SplitUIControl";
            this.Size = new System.Drawing.Size(380, 299);
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
    }
}
