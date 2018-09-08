namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    partial class ActionKeybindControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.descriptionLabel = new System.Windows.Forms.Label();
			this.modifierDropDown = new System.Windows.Forms.ComboBox();
			this.modifierLabel = new System.Windows.Forms.Label();
			this.altKeyDropDown = new System.Windows.Forms.ComboBox();
			this.altKeyLabel = new System.Windows.Forms.Label();
			this.mainKeyDropDown = new System.Windows.Forms.ComboBox();
			this.mainKeyLabel = new System.Windows.Forms.Label();
			this.descriptionTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.descriptionTextBox);
			this.groupBox1.Controls.Add(this.descriptionLabel);
			this.groupBox1.Controls.Add(this.modifierDropDown);
			this.groupBox1.Controls.Add(this.modifierLabel);
			this.groupBox1.Controls.Add(this.altKeyDropDown);
			this.groupBox1.Controls.Add(this.altKeyLabel);
			this.groupBox1.Controls.Add(this.mainKeyDropDown);
			this.groupBox1.Controls.Add(this.mainKeyLabel);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(371, 147);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "groupBox1";
			// 
			// descriptionLabel
			// 
			this.descriptionLabel.AutoSize = true;
			this.descriptionLabel.Location = new System.Drawing.Point(163, 79);
			this.descriptionLabel.Name = "descriptionLabel";
			this.descriptionLabel.Size = new System.Drawing.Size(63, 13);
			this.descriptionLabel.TabIndex = 7;
			this.descriptionLabel.Text = "Description:";
			// 
			// modifierDropDown
			// 
			this.modifierDropDown.FormattingEnabled = true;
			this.modifierDropDown.Location = new System.Drawing.Point(166, 41);
			this.modifierDropDown.Name = "modifierDropDown";
			this.modifierDropDown.Size = new System.Drawing.Size(121, 21);
			this.modifierDropDown.TabIndex = 6;
			// 
			// modifierLabel
			// 
			this.modifierLabel.AutoSize = true;
			this.modifierLabel.Location = new System.Drawing.Point(163, 25);
			this.modifierLabel.Name = "modifierLabel";
			this.modifierLabel.Size = new System.Drawing.Size(44, 13);
			this.modifierLabel.TabIndex = 5;
			this.modifierLabel.Text = "Modifier";
			// 
			// altKeyDropDown
			// 
			this.altKeyDropDown.FormattingEnabled = true;
			this.altKeyDropDown.Location = new System.Drawing.Point(9, 95);
			this.altKeyDropDown.Name = "altKeyDropDown";
			this.altKeyDropDown.Size = new System.Drawing.Size(121, 21);
			this.altKeyDropDown.TabIndex = 4;
			// 
			// altKeyLabel
			// 
			this.altKeyLabel.AutoSize = true;
			this.altKeyLabel.Location = new System.Drawing.Point(8, 79);
			this.altKeyLabel.Name = "altKeyLabel";
			this.altKeyLabel.Size = new System.Drawing.Size(40, 13);
			this.altKeyLabel.TabIndex = 3;
			this.altKeyLabel.Text = "Alt Key";
			// 
			// mainKeyDropDown
			// 
			this.mainKeyDropDown.FormattingEnabled = true;
			this.mainKeyDropDown.Location = new System.Drawing.Point(9, 41);
			this.mainKeyDropDown.Name = "mainKeyDropDown";
			this.mainKeyDropDown.Size = new System.Drawing.Size(121, 21);
			this.mainKeyDropDown.TabIndex = 2;
			// 
			// mainKeyLabel
			// 
			this.mainKeyLabel.AutoSize = true;
			this.mainKeyLabel.Location = new System.Drawing.Point(6, 25);
			this.mainKeyLabel.Name = "mainKeyLabel";
			this.mainKeyLabel.Size = new System.Drawing.Size(51, 13);
			this.mainKeyLabel.TabIndex = 1;
			this.mainKeyLabel.Text = "Main Key";
			// 
			// descriptionTextBox
			// 
			this.descriptionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.descriptionTextBox.Location = new System.Drawing.Point(166, 96);
			this.descriptionTextBox.Multiline = true;
			this.descriptionTextBox.Name = "descriptionTextBox";
			this.descriptionTextBox.ReadOnly = true;
			this.descriptionTextBox.Size = new System.Drawing.Size(199, 45);
			this.descriptionTextBox.TabIndex = 8;
			// 
			// ActionKeybindControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "ActionKeybindControl";
			this.Size = new System.Drawing.Size(371, 147);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label altKeyLabel;
        private System.Windows.Forms.ComboBox mainKeyDropDown;
        private System.Windows.Forms.Label mainKeyLabel;
        private System.Windows.Forms.ComboBox altKeyDropDown;
        private System.Windows.Forms.Label modifierLabel;
        private System.Windows.Forms.ComboBox modifierDropDown;
        private System.Windows.Forms.Label descriptionLabel;
		private System.Windows.Forms.TextBox descriptionTextBox;
	}
}
