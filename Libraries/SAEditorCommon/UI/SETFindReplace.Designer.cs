namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    partial class SETFindReplace
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
            this.ReplaceTypesButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.findItemDropDown = new System.Windows.Forms.ComboBox();
            this.replaceItemDropDown = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // ReplaceTypesButton
            // 
            this.ReplaceTypesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceTypesButton.Location = new System.Drawing.Point(61, 92);
            this.ReplaceTypesButton.Name = "ReplaceTypesButton";
            this.ReplaceTypesButton.Size = new System.Drawing.Size(90, 23);
            this.ReplaceTypesButton.TabIndex = 0;
            this.ReplaceTypesButton.Text = "Replace Types";
            this.ReplaceTypesButton.UseVisualStyleBackColor = true;
            this.ReplaceTypesButton.Click += new System.EventHandler(this.ReplaceTypesButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(157, 92);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Find:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Replace With:";
            // 
            // findItemDropDown
            // 
            this.findItemDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.findItemDropDown.FormattingEnabled = true;
            this.findItemDropDown.Location = new System.Drawing.Point(48, 15);
            this.findItemDropDown.Name = "findItemDropDown";
            this.findItemDropDown.Size = new System.Drawing.Size(121, 21);
            this.findItemDropDown.TabIndex = 4;
            // 
            // replaceItemDropDown
            // 
            this.replaceItemDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.replaceItemDropDown.FormattingEnabled = true;
            this.replaceItemDropDown.Location = new System.Drawing.Point(93, 46);
            this.replaceItemDropDown.Name = "replaceItemDropDown";
            this.replaceItemDropDown.Size = new System.Drawing.Size(121, 21);
            this.replaceItemDropDown.TabIndex = 5;
            // 
            // SETFindReplace
            // 
            this.AcceptButton = this.ReplaceTypesButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(244, 127);
            this.Controls.Add(this.replaceItemDropDown);
            this.Controls.Add(this.findItemDropDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.ReplaceTypesButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SETFindReplace";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SET Find/Replace";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ReplaceTypesButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox findItemDropDown;
        private System.Windows.Forms.ComboBox replaceItemDropDown;
    }
}