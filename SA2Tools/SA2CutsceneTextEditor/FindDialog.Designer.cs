namespace SA2CutsceneTextEditor
{
	partial class FindDialog
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
			this.characterCheckBox = new System.Windows.Forms.CheckBox();
			this.containsTextCheckBox = new System.Windows.Forms.CheckBox();
			this.lineEdit = new System.Windows.Forms.RichTextBox();
			this.findButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.characterSelector = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.characterSelector)).BeginInit();
			this.SuspendLayout();
			// 
			// characterCheckBox
			// 
			this.characterCheckBox.AutoSize = true;
			this.characterCheckBox.Location = new System.Drawing.Point(12, 13);
			this.characterCheckBox.Name = "characterCheckBox";
			this.characterCheckBox.Size = new System.Drawing.Size(75, 17);
			this.characterCheckBox.TabIndex = 20;
			this.characterCheckBox.Text = "Character:";
			this.characterCheckBox.UseVisualStyleBackColor = true;
			this.characterCheckBox.CheckedChanged += new System.EventHandler(this.characterCheckBox_CheckedChanged);
			// 
			// containsTextCheckBox
			// 
			this.containsTextCheckBox.AutoSize = true;
			this.containsTextCheckBox.Location = new System.Drawing.Point(12, 39);
			this.containsTextCheckBox.Name = "containsTextCheckBox";
			this.containsTextCheckBox.Size = new System.Drawing.Size(94, 17);
			this.containsTextCheckBox.TabIndex = 24;
			this.containsTextCheckBox.Text = "Contains Text:";
			this.containsTextCheckBox.UseVisualStyleBackColor = true;
			this.containsTextCheckBox.CheckedChanged += new System.EventHandler(this.containsTextCheckBox_CheckedChanged);
			// 
			// lineEdit
			// 
			this.lineEdit.AcceptsTab = true;
			this.lineEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lineEdit.DetectUrls = false;
			this.lineEdit.Enabled = false;
			this.lineEdit.Location = new System.Drawing.Point(12, 62);
			this.lineEdit.Name = "lineEdit";
			this.lineEdit.Size = new System.Drawing.Size(260, 102);
			this.lineEdit.TabIndex = 25;
			this.lineEdit.Text = "";
			// 
			// findButton
			// 
			this.findButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.findButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.findButton.Location = new System.Drawing.Point(116, 170);
			this.findButton.Name = "findButton";
			this.findButton.Size = new System.Drawing.Size(75, 23);
			this.findButton.TabIndex = 26;
			this.findButton.Text = "&Find";
			this.findButton.UseVisualStyleBackColor = true;
			this.findButton.Click += new System.EventHandler(this.findButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(197, 170);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 27;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// characterSelector
			// 
			this.characterSelector.Enabled = false;
			this.characterSelector.Location = new System.Drawing.Point(93, 12);
			this.characterSelector.Name = "characterSelector";
			this.characterSelector.Size = new System.Drawing.Size(73, 20);
			this.characterSelector.TabIndex = 28;
			// 
			// FindDialog
			// 
			this.AcceptButton = this.findButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(284, 205);
			this.Controls.Add(this.characterSelector);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.findButton);
			this.Controls.Add(this.lineEdit);
			this.Controls.Add(this.containsTextCheckBox);
			this.Controls.Add(this.characterCheckBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Find";
			((System.ComponentModel.ISupportInitialize)(this.characterSelector)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.CheckBox characterCheckBox;
		private System.Windows.Forms.CheckBox containsTextCheckBox;
		private System.Windows.Forms.RichTextBox lineEdit;
		private System.Windows.Forms.Button findButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.NumericUpDown characterSelector;
	}
}