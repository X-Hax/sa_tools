namespace SAModel.SAEditorCommon.UI
{
	partial class SETDeleteType
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
            this.cancel = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.delTypeDropDown = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(209, 54);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(128, 54);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // delTypeDropDown
            // 
            this.delTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.delTypeDropDown.FormattingEnabled = true;
            this.delTypeDropDown.Location = new System.Drawing.Point(94, 18);
            this.delTypeDropDown.Name = "delTypeDropDown";
            this.delTypeDropDown.Size = new System.Drawing.Size(190, 21);
            this.delTypeDropDown.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Type to Delete:";
            // 
            // SETDeleteType
            // 
            this.AcceptButton = this.deleteButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(296, 86);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.delTypeDropDown);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SETDeleteType";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Delete All SET Items of Type";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.ComboBox delTypeDropDown;
		private System.Windows.Forms.Label label1;
	}
}