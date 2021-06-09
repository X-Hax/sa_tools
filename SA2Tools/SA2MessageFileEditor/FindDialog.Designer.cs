namespace SA2MessageFileEditor
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
			this.voiceSelector = new System.Windows.Forms.NumericUpDown();
			this.playsVoiceCheckBox = new System.Windows.Forms.CheckBox();
			this.audioSelector = new System.Windows.Forms.NumericUpDown();
			this.playsAudioCheckBox = new System.Windows.Forms.CheckBox();
			this.containsTextCheckBox = new System.Windows.Forms.CheckBox();
			this.lineEdit = new System.Windows.Forms.RichTextBox();
			this.findButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.voiceSelector)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.audioSelector)).BeginInit();
			this.SuspendLayout();
			// 
			// voiceSelector
			// 
			this.voiceSelector.Enabled = false;
			this.voiceSelector.Location = new System.Drawing.Point(102, 38);
			this.voiceSelector.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.voiceSelector.Name = "voiceSelector";
			this.voiceSelector.Size = new System.Drawing.Size(61, 20);
			this.voiceSelector.TabIndex = 23;
			// 
			// playsVoiceCheckBox
			// 
			this.playsVoiceCheckBox.AutoSize = true;
			this.playsVoiceCheckBox.Location = new System.Drawing.Point(12, 39);
			this.playsVoiceCheckBox.Name = "playsVoiceCheckBox";
			this.playsVoiceCheckBox.Size = new System.Drawing.Size(84, 17);
			this.playsVoiceCheckBox.TabIndex = 22;
			this.playsVoiceCheckBox.Text = "Plays Voice:";
			this.playsVoiceCheckBox.UseVisualStyleBackColor = true;
			this.playsVoiceCheckBox.CheckedChanged += new System.EventHandler(this.playsVoiceCheckBox_CheckedChanged);
			// 
			// audioSelector
			// 
			this.audioSelector.Enabled = false;
			this.audioSelector.Location = new System.Drawing.Point(102, 12);
			this.audioSelector.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.audioSelector.Name = "audioSelector";
			this.audioSelector.Size = new System.Drawing.Size(61, 20);
			this.audioSelector.TabIndex = 21;
			// 
			// playsAudioCheckBox
			// 
			this.playsAudioCheckBox.AutoSize = true;
			this.playsAudioCheckBox.Location = new System.Drawing.Point(12, 13);
			this.playsAudioCheckBox.Name = "playsAudioCheckBox";
			this.playsAudioCheckBox.Size = new System.Drawing.Size(84, 17);
			this.playsAudioCheckBox.TabIndex = 20;
			this.playsAudioCheckBox.Text = "Plays Audio:";
			this.playsAudioCheckBox.UseVisualStyleBackColor = true;
			this.playsAudioCheckBox.CheckedChanged += new System.EventHandler(this.playsAudioCheckBox_CheckedChanged);
			// 
			// containsTextCheckBox
			// 
			this.containsTextCheckBox.AutoSize = true;
			this.containsTextCheckBox.Location = new System.Drawing.Point(12, 64);
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
			this.lineEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lineEdit.DetectUrls = false;
			this.lineEdit.Enabled = false;
			this.lineEdit.Location = new System.Drawing.Point(12, 87);
			this.lineEdit.Name = "lineEdit";
			this.lineEdit.Size = new System.Drawing.Size(260, 102);
			this.lineEdit.TabIndex = 25;
			this.lineEdit.Text = "";
			// 
			// findButton
			// 
			this.findButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.findButton.Location = new System.Drawing.Point(116, 195);
			this.findButton.Name = "findButton";
			this.findButton.Size = new System.Drawing.Size(75, 23);
			this.findButton.TabIndex = 26;
			this.findButton.Text = "&Find";
			this.findButton.UseVisualStyleBackColor = true;
			this.findButton.Click += new System.EventHandler(this.findButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(197, 195);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 27;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// FindDialog
			// 
			this.AcceptButton = this.findButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(284, 230);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.findButton);
			this.Controls.Add(this.lineEdit);
			this.Controls.Add(this.containsTextCheckBox);
			this.Controls.Add(this.voiceSelector);
			this.Controls.Add(this.playsVoiceCheckBox);
			this.Controls.Add(this.audioSelector);
			this.Controls.Add(this.playsAudioCheckBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Find";
			((System.ComponentModel.ISupportInitialize)(this.voiceSelector)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.audioSelector)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown voiceSelector;
		private System.Windows.Forms.CheckBox playsVoiceCheckBox;
		private System.Windows.Forms.NumericUpDown audioSelector;
		private System.Windows.Forms.CheckBox playsAudioCheckBox;
		private System.Windows.Forms.CheckBox containsTextCheckBox;
		private System.Windows.Forms.RichTextBox lineEdit;
		private System.Windows.Forms.Button findButton;
		private System.Windows.Forms.Button cancelButton;
	}
}