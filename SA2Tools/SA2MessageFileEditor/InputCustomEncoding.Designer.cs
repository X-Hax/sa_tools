namespace SA2MessageFileEditor
{
	partial class InputCustomEncoding
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
			buttonOK = new System.Windows.Forms.Button();
			buttonCancel = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
			SuspendLayout();
			// 
			// buttonOK
			// 
			buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			buttonOK.Location = new System.Drawing.Point(12, 69);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new System.Drawing.Size(64, 25);
			buttonOK.TabIndex = 1;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			buttonOK.Click += buttonOK_Click;
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			buttonCancel.Location = new System.Drawing.Point(85, 69);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new System.Drawing.Size(64, 25);
			buttonCancel.TabIndex = 2;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			buttonCancel.Click += buttonCancel_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 13);
			label1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(133, 15);
			label1.TabIndex = 2;
			label1.Text = "Set a custom codepage:";
			// 
			// numericUpDown1
			// 
			numericUpDown1.Location = new System.Drawing.Point(32, 35);
			numericUpDown1.Maximum = new decimal(new int[] { 65001, 0, 0, 0 });
			numericUpDown1.Minimum = new decimal(new int[] { 37, 0, 0, 0 });
			numericUpDown1.Name = "numericUpDown1";
			numericUpDown1.Size = new System.Drawing.Size(96, 23);
			numericUpDown1.TabIndex = 0;
			numericUpDown1.Value = new decimal(new int[] { 1252, 0, 0, 0 });
			// 
			// InputCustomEncoding
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(161, 106);
			Controls.Add(numericUpDown1);
			Controls.Add(label1);
			Controls.Add(buttonCancel);
			Controls.Add(buttonOK);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "InputCustomEncoding";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Codepage";
			((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
	}
}