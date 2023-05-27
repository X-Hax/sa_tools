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
			fileLabel = new System.Windows.Forms.Label();
			filePathTextBox = new System.Windows.Forms.TextBox();
			fileBrowseButton = new System.Windows.Forms.Button();
			listBox1 = new System.Windows.Forms.ListBox();
			animationFilesLabel = new System.Windows.Forms.Label();
			animFilesBrowse = new System.Windows.Forms.Button();
			splitButton = new System.Windows.Forms.Button();
			outputFolderBrowseButton = new System.Windows.Forms.Button();
			outputFolderPath = new System.Windows.Forms.TextBox();
			outputLabel = new System.Windows.Forms.Label();
			animFilesClear = new System.Windows.Forms.Button();
			SuspendLayout();
			// 
			// fileLabel
			// 
			fileLabel.AutoSize = true;
			fileLabel.Location = new System.Drawing.Point(14, 10);
			fileLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			fileLabel.Name = "fileLabel";
			fileLabel.Size = new System.Drawing.Size(28, 15);
			fileLabel.TabIndex = 0;
			fileLabel.Text = "File:";
			// 
			// filePathTextBox
			// 
			filePathTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			filePathTextBox.Location = new System.Drawing.Point(18, 29);
			filePathTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			filePathTextBox.Name = "filePathTextBox";
			filePathTextBox.Size = new System.Drawing.Size(440, 23);
			filePathTextBox.TabIndex = 1;
			// 
			// fileBrowseButton
			// 
			fileBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			fileBrowseButton.Location = new System.Drawing.Point(479, 25);
			fileBrowseButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			fileBrowseButton.Name = "fileBrowseButton";
			fileBrowseButton.Size = new System.Drawing.Size(139, 27);
			fileBrowseButton.TabIndex = 2;
			fileBrowseButton.Text = "Browse";
			fileBrowseButton.UseVisualStyleBackColor = true;
			fileBrowseButton.Click += fileBrowseButton_Click;
			// 
			// listBox1
			// 
			listBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			listBox1.FormattingEnabled = true;
			listBox1.ItemHeight = 15;
			listBox1.Location = new System.Drawing.Point(18, 156);
			listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			listBox1.Name = "listBox1";
			listBox1.Size = new System.Drawing.Size(440, 334);
			listBox1.TabIndex = 3;
			// 
			// animationFilesLabel
			// 
			animationFilesLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			animationFilesLabel.AutoSize = true;
			animationFilesLabel.Location = new System.Drawing.Point(14, 133);
			animationFilesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			animationFilesLabel.Name = "animationFilesLabel";
			animationFilesLabel.Size = new System.Drawing.Size(89, 15);
			animationFilesLabel.TabIndex = 4;
			animationFilesLabel.Text = "Animation Files";
			// 
			// animFilesBrowse
			// 
			animFilesBrowse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			animFilesBrowse.Location = new System.Drawing.Point(479, 156);
			animFilesBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			animFilesBrowse.Name = "animFilesBrowse";
			animFilesBrowse.Size = new System.Drawing.Size(139, 27);
			animFilesBrowse.TabIndex = 5;
			animFilesBrowse.Text = "Add...";
			animFilesBrowse.UseVisualStyleBackColor = true;
			animFilesBrowse.Click += animFilesBrowse_Click;
			// 
			// splitButton
			// 
			splitButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			splitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			splitButton.Location = new System.Drawing.Point(493, 407);
			splitButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			splitButton.Name = "splitButton";
			splitButton.Size = new System.Drawing.Size(125, 81);
			splitButton.TabIndex = 6;
			splitButton.Text = "Split";
			splitButton.UseVisualStyleBackColor = true;
			splitButton.Click += splitButton_Click;
			// 
			// outputFolderBrowseButton
			// 
			outputFolderBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			outputFolderBrowseButton.Location = new System.Drawing.Point(479, 88);
			outputFolderBrowseButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			outputFolderBrowseButton.Name = "outputFolderBrowseButton";
			outputFolderBrowseButton.Size = new System.Drawing.Size(139, 27);
			outputFolderBrowseButton.TabIndex = 9;
			outputFolderBrowseButton.Text = "Browse";
			outputFolderBrowseButton.UseVisualStyleBackColor = true;
			outputFolderBrowseButton.Click += outputFolderBrowseButton_Click;
			// 
			// outputFolderPath
			// 
			outputFolderPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			outputFolderPath.Location = new System.Drawing.Point(18, 91);
			outputFolderPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			outputFolderPath.Name = "outputFolderPath";
			outputFolderPath.Size = new System.Drawing.Size(440, 23);
			outputFolderPath.TabIndex = 8;
			// 
			// outputLabel
			// 
			outputLabel.AutoSize = true;
			outputLabel.Location = new System.Drawing.Point(14, 73);
			outputLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			outputLabel.Name = "outputLabel";
			outputLabel.Size = new System.Drawing.Size(84, 15);
			outputLabel.TabIndex = 7;
			outputLabel.Text = "Output Folder:";
			// 
			// animFilesClear
			// 
			animFilesClear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			animFilesClear.Location = new System.Drawing.Point(479, 189);
			animFilesClear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			animFilesClear.Name = "animFilesClear";
			animFilesClear.Size = new System.Drawing.Size(139, 27);
			animFilesClear.TabIndex = 11;
			animFilesClear.Text = "Clear";
			animFilesClear.UseVisualStyleBackColor = true;
			animFilesClear.Click += animFilesClear_Click;
			// 
			// SplitMDLGUI
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(648, 510);
			Controls.Add(animFilesClear);
			Controls.Add(outputFolderBrowseButton);
			Controls.Add(outputFolderPath);
			Controls.Add(outputLabel);
			Controls.Add(splitButton);
			Controls.Add(animFilesBrowse);
			Controls.Add(animationFilesLabel);
			Controls.Add(listBox1);
			Controls.Add(fileBrowseButton);
			Controls.Add(filePathTextBox);
			Controls.Add(fileLabel);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "SplitMDLGUI";
			Text = "SplitMDLGUI";
			ResumeLayout(false);
			PerformLayout();
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
		private System.Windows.Forms.Button animFilesClear;
	}
}