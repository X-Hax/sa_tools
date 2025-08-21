namespace SAFontEdit
{
	partial class ExportWidthDialog
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
			buttonExport = new System.Windows.Forms.Button();
			buttonCancel = new System.Windows.Forms.Button();
			numericUpDownSpaceWidth = new System.Windows.Forms.NumericUpDown();
			numericUpDownEmptyWidth = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)numericUpDownSpaceWidth).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownEmptyWidth).BeginInit();
			SuspendLayout();
			// 
			// buttonExport
			// 
			buttonExport.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			buttonExport.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonExport.Location = new System.Drawing.Point(48, 72);
			buttonExport.Name = "buttonExport";
			buttonExport.Size = new System.Drawing.Size(75, 23);
			buttonExport.TabIndex = 0;
			buttonExport.Text = "Export...";
			buttonExport.UseVisualStyleBackColor = true;
			buttonExport.Click += buttonExport_Click;
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			buttonCancel.Location = new System.Drawing.Point(129, 72);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new System.Drawing.Size(75, 23);
			buttonCancel.TabIndex = 1;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// numericUpDownSpaceWidth
			// 
			numericUpDownSpaceWidth.Location = new System.Drawing.Point(146, 7);
			numericUpDownSpaceWidth.Name = "numericUpDownSpaceWidth";
			numericUpDownSpaceWidth.Size = new System.Drawing.Size(56, 23);
			numericUpDownSpaceWidth.TabIndex = 2;
			numericUpDownSpaceWidth.Value = new decimal(new int[] { 10, 0, 0, 0 });
			// 
			// numericUpDownEmptyWidth
			// 
			numericUpDownEmptyWidth.Location = new System.Drawing.Point(146, 41);
			numericUpDownEmptyWidth.Name = "numericUpDownEmptyWidth";
			numericUpDownEmptyWidth.Size = new System.Drawing.Size(56, 23);
			numericUpDownEmptyWidth.TabIndex = 3;
			numericUpDownEmptyWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(48, 9);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(89, 15);
			label1.TabIndex = 4;
			label1.Text = "Width of space:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 43);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(125, 15);
			label2.TabIndex = 5;
			label2.Text = "Width of empty items:";
			// 
			// ExportWidthDialog
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(216, 107);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(numericUpDownEmptyWidth);
			Controls.Add(numericUpDownSpaceWidth);
			Controls.Add(buttonCancel);
			Controls.Add(buttonExport);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Name = "ExportWidthDialog";
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Export Width";
			((System.ComponentModel.ISupportInitialize)numericUpDownSpaceWidth).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownEmptyWidth).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button buttonExport;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.NumericUpDown numericUpDownSpaceWidth;
		private System.Windows.Forms.NumericUpDown numericUpDownEmptyWidth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}