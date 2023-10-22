namespace SAModel.SAMDL
{
	partial class PickModelDialog
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
			okButton = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			modelChoice = new System.Windows.Forms.ComboBox();
			checkBox1 = new System.Windows.Forms.CheckBox();
			SuspendLayout();
			// 
			// okButton
			// 
			okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			okButton.Enabled = false;
			okButton.Location = new System.Drawing.Point(127, 75);
			okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size(88, 27);
			okButton.TabIndex = 0;
			okButton.Text = "&OK";
			okButton.UseVisualStyleBackColor = true;
			okButton.Click += okButton_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(14, 17);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(44, 15);
			label1.TabIndex = 1;
			label1.Text = "Model:";
			// 
			// modelChoice
			// 
			modelChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			modelChoice.FormattingEnabled = true;
			modelChoice.Location = new System.Drawing.Point(66, 14);
			modelChoice.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			modelChoice.Name = "modelChoice";
			modelChoice.Size = new System.Drawing.Size(148, 23);
			modelChoice.TabIndex = 2;
			modelChoice.SelectedIndexChanged += modelChoice_SelectedIndexChanged;
			// 
			// checkBox1
			// 
			checkBox1.AutoSize = true;
			checkBox1.Location = new System.Drawing.Point(14, 45);
			checkBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			checkBox1.Name = "checkBox1";
			checkBox1.Size = new System.Drawing.Size(115, 19);
			checkBox1.TabIndex = 3;
			checkBox1.Text = "Load Motion File";
			checkBox1.UseVisualStyleBackColor = true;
			checkBox1.Visible = false;
			// 
			// SA2MDLDialog
			// 
			AcceptButton = okButton;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(229, 115);
			Controls.Add(checkBox1);
			Controls.Add(modelChoice);
			Controls.Add(label1);
			Controls.Add(okButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "SA2MDLDialog";
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Select Model";
			Load += SA2MDLDialog_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label1;
		internal System.Windows.Forms.ComboBox modelChoice;
		internal System.Windows.Forms.CheckBox checkBox1;
	}
}

