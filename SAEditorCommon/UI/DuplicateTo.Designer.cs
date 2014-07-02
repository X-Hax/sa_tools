namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	partial class DuplicateTo
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
			this.sonicCheckBox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tailsCheckBox = new System.Windows.Forms.CheckBox();
			this.knucklesCheckBox = new System.Windows.Forms.CheckBox();
			this.amyCheckBox = new System.Windows.Forms.CheckBox();
			this.gammaCheckBox = new System.Windows.Forms.CheckBox();
			this.bigCheckBox = new System.Windows.Forms.CheckBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// sonicCheckBox
			// 
			this.sonicCheckBox.AutoSize = true;
			this.sonicCheckBox.Location = new System.Drawing.Point(12, 33);
			this.sonicCheckBox.Name = "sonicCheckBox";
			this.sonicCheckBox.Size = new System.Drawing.Size(53, 17);
			this.sonicCheckBox.TabIndex = 0;
			this.sonicCheckBox.Text = "Sonic";
			this.sonicCheckBox.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(287, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "This clones the selected SET Items to the following layouts.";
			// 
			// tailsCheckBox
			// 
			this.tailsCheckBox.AutoSize = true;
			this.tailsCheckBox.Location = new System.Drawing.Point(12, 53);
			this.tailsCheckBox.Name = "tailsCheckBox";
			this.tailsCheckBox.Size = new System.Drawing.Size(48, 17);
			this.tailsCheckBox.TabIndex = 2;
			this.tailsCheckBox.Text = "Tails";
			this.tailsCheckBox.UseVisualStyleBackColor = true;
			// 
			// knucklesCheckBox
			// 
			this.knucklesCheckBox.AutoSize = true;
			this.knucklesCheckBox.Location = new System.Drawing.Point(12, 73);
			this.knucklesCheckBox.Name = "knucklesCheckBox";
			this.knucklesCheckBox.Size = new System.Drawing.Size(70, 17);
			this.knucklesCheckBox.TabIndex = 3;
			this.knucklesCheckBox.Text = "Knuckles";
			this.knucklesCheckBox.UseVisualStyleBackColor = true;
			// 
			// amyCheckBox
			// 
			this.amyCheckBox.AutoSize = true;
			this.amyCheckBox.Location = new System.Drawing.Point(12, 93);
			this.amyCheckBox.Name = "amyCheckBox";
			this.amyCheckBox.Size = new System.Drawing.Size(46, 17);
			this.amyCheckBox.TabIndex = 4;
			this.amyCheckBox.Text = "Amy";
			this.amyCheckBox.UseVisualStyleBackColor = true;
			// 
			// gammaCheckBox
			// 
			this.gammaCheckBox.AutoSize = true;
			this.gammaCheckBox.Location = new System.Drawing.Point(12, 114);
			this.gammaCheckBox.Name = "gammaCheckBox";
			this.gammaCheckBox.Size = new System.Drawing.Size(62, 17);
			this.gammaCheckBox.TabIndex = 5;
			this.gammaCheckBox.Text = "Gamma";
			this.gammaCheckBox.UseVisualStyleBackColor = true;
			// 
			// bigCheckBox
			// 
			this.bigCheckBox.AutoSize = true;
			this.bigCheckBox.Location = new System.Drawing.Point(12, 135);
			this.bigCheckBox.Name = "bigCheckBox";
			this.bigCheckBox.Size = new System.Drawing.Size(41, 17);
			this.bigCheckBox.TabIndex = 6;
			this.bigCheckBox.Text = "Big";
			this.bigCheckBox.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(215, 169);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 7;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(134, 169);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 8;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// DuplicateTo
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(306, 204);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.bigCheckBox);
			this.Controls.Add(this.gammaCheckBox);
			this.Controls.Add(this.amyCheckBox);
			this.Controls.Add(this.knucklesCheckBox);
			this.Controls.Add(this.tailsCheckBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.sonicCheckBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "DuplicateTo";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Duplicate SET Items To:";
			this.Load += new System.EventHandler(this.DuplicateTo_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox sonicCheckBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox tailsCheckBox;
		private System.Windows.Forms.CheckBox knucklesCheckBox;
		private System.Windows.Forms.CheckBox amyCheckBox;
		private System.Windows.Forms.CheckBox gammaCheckBox;
		private System.Windows.Forms.CheckBox bigCheckBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
	}
}