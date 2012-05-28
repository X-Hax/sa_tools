namespace SonicRetro.SAModel.SAMDL
{
    partial class FileTypeDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.typBinary = new System.Windows.Forms.RadioButton();
            this.typSA2MDL = new System.Windows.Forms.RadioButton();
            this.typSA2BMDL = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(14, 112);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(95, 112);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Type:";
            // 
            // typBinary
            // 
            this.typBinary.AutoSize = true;
            this.typBinary.Checked = true;
            this.typBinary.Location = new System.Drawing.Point(12, 25);
            this.typBinary.Name = "typBinary";
            this.typBinary.Size = new System.Drawing.Size(54, 17);
            this.typBinary.TabIndex = 3;
            this.typBinary.TabStop = true;
            this.typBinary.Text = "Binary";
            this.typBinary.UseVisualStyleBackColor = true;
            // 
            // typSA2MDL
            // 
            this.typSA2MDL.AutoSize = true;
            this.typSA2MDL.Location = new System.Drawing.Point(12, 48);
            this.typSA2MDL.Name = "typSA2MDL";
            this.typSA2MDL.Size = new System.Drawing.Size(90, 17);
            this.typSA2MDL.TabIndex = 4;
            this.typSA2MDL.TabStop = true;
            this.typSA2MDL.Text = "SA2 MDL File";
            this.typSA2MDL.UseVisualStyleBackColor = true;
            // 
            // typSA2BMDL
            // 
            this.typSA2BMDL.AutoSize = true;
            this.typSA2BMDL.Location = new System.Drawing.Point(12, 71);
            this.typSA2BMDL.Name = "typSA2BMDL";
            this.typSA2BMDL.Size = new System.Drawing.Size(97, 17);
            this.typSA2BMDL.TabIndex = 5;
            this.typSA2BMDL.TabStop = true;
            this.typSA2BMDL.Text = "SA2B MDL File";
            this.typSA2BMDL.UseVisualStyleBackColor = true;
            // 
            // FileTypeDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(182, 147);
            this.Controls.Add(this.typSA2BMDL);
            this.Controls.Add(this.typSA2MDL);
            this.Controls.Add(this.typBinary);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileTypeDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SA1/SA2 Model Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.RadioButton typBinary;
        internal System.Windows.Forms.RadioButton typSA2MDL;
        internal System.Windows.Forms.RadioButton typSA2BMDL;
    }
}

