namespace SADXsndSharp
{
	partial class OpenFromOffset
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
			numericUpDownOffset = new System.Windows.Forms.NumericUpDown();
			buttonOK = new System.Windows.Forms.Button();
			buttonCancel = new System.Windows.Forms.Button();
			checkBoxHex = new System.Windows.Forms.CheckBox();
			label1 = new System.Windows.Forms.Label();
			labelFileType = new System.Windows.Forms.Label();
			comboBox1 = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)numericUpDownOffset).BeginInit();
			SuspendLayout();
			// 
			// numericUpDownOffset
			// 
			numericUpDownOffset.Hexadecimal = true;
			numericUpDownOffset.Location = new System.Drawing.Point(210, 28);
			numericUpDownOffset.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
			numericUpDownOffset.Minimum = new decimal(new int[] { -1, 0, 0, int.MinValue });
			numericUpDownOffset.Name = "numericUpDownOffset";
			numericUpDownOffset.Size = new System.Drawing.Size(120, 23);
			numericUpDownOffset.TabIndex = 0;
			numericUpDownOffset.ValueChanged += numericUpDown1_ValueChanged;
			// 
			// buttonOK
			// 
			buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonOK.Location = new System.Drawing.Point(174, 84);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new System.Drawing.Size(75, 23);
			buttonOK.TabIndex = 1;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			buttonCancel.Location = new System.Drawing.Point(255, 84);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new System.Drawing.Size(75, 23);
			buttonCancel.TabIndex = 2;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// checkBoxHex
			// 
			checkBoxHex.AutoSize = true;
			checkBoxHex.Checked = true;
			checkBoxHex.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBoxHex.Location = new System.Drawing.Point(210, 57);
			checkBoxHex.Name = "checkBoxHex";
			checkBoxHex.Size = new System.Drawing.Size(95, 19);
			checkBoxHex.TabIndex = 3;
			checkBoxHex.Text = "Hexadecimal";
			checkBoxHex.UseVisualStyleBackColor = true;
			checkBoxHex.CheckedChanged += checkBoxHex_CheckedChanged;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(210, 10);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(63, 15);
			label1.TabIndex = 4;
			label1.Text = "File Offset:";
			// 
			// labelFileType
			// 
			labelFileType.AutoSize = true;
			labelFileType.Location = new System.Drawing.Point(12, 9);
			labelFileType.Name = "labelFileType";
			labelFileType.Size = new System.Drawing.Size(77, 15);
			labelFileType.TabIndex = 5;
			labelFileType.Text = "Archive Type:";
			// 
			// comboBox1
			// 
			comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBox1.FormattingEnabled = true;
			comboBox1.Location = new System.Drawing.Point(12, 28);
			comboBox1.Name = "comboBox1";
			comboBox1.Size = new System.Drawing.Size(192, 23);
			comboBox1.TabIndex = 6;
			comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
			// 
			// OpenFromOffset
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(342, 119);
			Controls.Add(comboBox1);
			Controls.Add(labelFileType);
			Controls.Add(label1);
			Controls.Add(checkBoxHex);
			Controls.Add(buttonCancel);
			Controls.Add(buttonOK);
			Controls.Add(numericUpDownOffset);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "OpenFromOffset";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Open a Binary File";
			((System.ComponentModel.ISupportInitialize)numericUpDownOffset).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDownOffset;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxHex;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelFileType;
		private System.Windows.Forms.ComboBox comboBox1;
	}
}