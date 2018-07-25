namespace SplitMDL
{
    partial class SplitMDLGUI
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
            this.fileLabel = new System.Windows.Forms.Label();
            this.filePathTextBox = new System.Windows.Forms.TextBox();
            this.fileBrowseButton = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.animationFilesLabel = new System.Windows.Forms.Label();
            this.animFilesBrowse = new System.Windows.Forms.Button();
            this.splitButton = new System.Windows.Forms.Button();
            this.outputFolderBrowseButton = new System.Windows.Forms.Button();
            this.outputFolderPath = new System.Windows.Forms.TextBox();
            this.outputLabel = new System.Windows.Forms.Label();
            this.bigEndianCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // fileLabel
            // 
            this.fileLabel.AutoSize = true;
            this.fileLabel.Location = new System.Drawing.Point(12, 9);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(26, 13);
            this.fileLabel.TabIndex = 0;
            this.fileLabel.Text = "File:";
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathTextBox.Location = new System.Drawing.Point(15, 25);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.Size = new System.Drawing.Size(378, 20);
            this.filePathTextBox.TabIndex = 1;
            // 
            // fileBrowseButton
            // 
            this.fileBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileBrowseButton.Location = new System.Drawing.Point(411, 22);
            this.fileBrowseButton.Name = "fileBrowseButton";
            this.fileBrowseButton.Size = new System.Drawing.Size(119, 23);
            this.fileBrowseButton.TabIndex = 2;
            this.fileBrowseButton.Text = "Browse";
            this.fileBrowseButton.UseVisualStyleBackColor = true;
            this.fileBrowseButton.Click += new System.EventHandler(this.fileBrowseButton_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(15, 135);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(378, 290);
            this.listBox1.TabIndex = 3;
            // 
            // animationFilesLabel
            // 
            this.animationFilesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.animationFilesLabel.AutoSize = true;
            this.animationFilesLabel.Location = new System.Drawing.Point(12, 115);
            this.animationFilesLabel.Name = "animationFilesLabel";
            this.animationFilesLabel.Size = new System.Drawing.Size(77, 13);
            this.animationFilesLabel.TabIndex = 4;
            this.animationFilesLabel.Text = "Animation Files";
            // 
            // animFilesBrowse
            // 
            this.animFilesBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.animFilesBrowse.Location = new System.Drawing.Point(411, 135);
            this.animFilesBrowse.Name = "animFilesBrowse";
            this.animFilesBrowse.Size = new System.Drawing.Size(119, 23);
            this.animFilesBrowse.TabIndex = 5;
            this.animFilesBrowse.Text = "Browse";
            this.animFilesBrowse.UseVisualStyleBackColor = true;
            this.animFilesBrowse.Click += new System.EventHandler(this.animFilesBrowse_Click);
            // 
            // splitButton
            // 
            this.splitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.splitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitButton.Location = new System.Drawing.Point(423, 353);
            this.splitButton.Name = "splitButton";
            this.splitButton.Size = new System.Drawing.Size(107, 70);
            this.splitButton.TabIndex = 6;
            this.splitButton.Text = "Split";
            this.splitButton.UseVisualStyleBackColor = true;
            this.splitButton.Click += new System.EventHandler(this.splitButton_Click);
            // 
            // outputFolderBrowseButton
            // 
            this.outputFolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFolderBrowseButton.Location = new System.Drawing.Point(411, 76);
            this.outputFolderBrowseButton.Name = "outputFolderBrowseButton";
            this.outputFolderBrowseButton.Size = new System.Drawing.Size(119, 23);
            this.outputFolderBrowseButton.TabIndex = 9;
            this.outputFolderBrowseButton.Text = "Browse";
            this.outputFolderBrowseButton.UseVisualStyleBackColor = true;
            this.outputFolderBrowseButton.Click += new System.EventHandler(this.outputFolderBrowseButton_Click);
            // 
            // outputFolderPath
            // 
            this.outputFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFolderPath.Location = new System.Drawing.Point(15, 79);
            this.outputFolderPath.Name = "outputFolderPath";
            this.outputFolderPath.Size = new System.Drawing.Size(378, 20);
            this.outputFolderPath.TabIndex = 8;
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(12, 63);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(74, 13);
            this.outputLabel.TabIndex = 7;
            this.outputLabel.Text = "Output Folder:";
            // 
            // bigEndianCheckbox
            // 
            this.bigEndianCheckbox.AutoSize = true;
            this.bigEndianCheckbox.Location = new System.Drawing.Point(313, 51);
            this.bigEndianCheckbox.Name = "bigEndianCheckbox";
            this.bigEndianCheckbox.Size = new System.Drawing.Size(77, 17);
            this.bigEndianCheckbox.TabIndex = 10;
            this.bigEndianCheckbox.Text = "Big Endian";
            this.bigEndianCheckbox.UseVisualStyleBackColor = true;
            // 
            // SplitMDLGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 442);
            this.Controls.Add(this.bigEndianCheckbox);
            this.Controls.Add(this.outputFolderBrowseButton);
            this.Controls.Add(this.outputFolderPath);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.splitButton);
            this.Controls.Add(this.animFilesBrowse);
            this.Controls.Add(this.animationFilesLabel);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.fileBrowseButton);
            this.Controls.Add(this.filePathTextBox);
            this.Controls.Add(this.fileLabel);
            this.Name = "SplitMDLGUI";
            this.Text = "SplitMDLGUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.Button fileBrowseButton;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label animationFilesLabel;
        private System.Windows.Forms.Button animFilesBrowse;
        private System.Windows.Forms.Button splitButton;
        private System.Windows.Forms.Button outputFolderBrowseButton;
        private System.Windows.Forms.TextBox outputFolderPath;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.CheckBox bigEndianCheckbox;
    }
}