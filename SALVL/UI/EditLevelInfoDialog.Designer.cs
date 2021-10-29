namespace SAModel.SALVL
{
    partial class EditLevelInfoDialog
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
            System.Windows.Forms.Label label3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditLevelInfoDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.description = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.author = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textureFile = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textureList = new System.Windows.Forms.NumericUpDown();
            this.label = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxAttributeAnimation = new System.Windows.Forms.CheckBox();
            this.checkBoxAttributePVM = new System.Windows.Forms.CheckBox();
            this.checkBoxAttributeClip = new System.Windows.Forms.CheckBox();
            this.checkBoxAttributeTexlist = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownClipDist = new System.Windows.Forms.NumericUpDown();
            this.labelAttribute = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.textureList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownClipDist)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 15);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(36, 13);
            label3.TabIndex = 41;
            label3.Text = "Label:";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(144, 284);
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
            this.cancelButton.Location = new System.Drawing.Point(225, 284);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // description
            // 
            this.description.AcceptsReturn = true;
            this.description.AcceptsTab = true;
            this.description.Location = new System.Drawing.Point(76, 187);
            this.description.Multiline = true;
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(223, 85);
            this.description.TabIndex = 35;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Description:";
            // 
            // author
            // 
            this.author.Location = new System.Drawing.Point(76, 161);
            this.author.Name = "author";
            this.author.Size = new System.Drawing.Size(223, 20);
            this.author.TabIndex = 33;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Author:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Texture File Name:";
            // 
            // textureFile
            // 
            this.textureFile.Location = new System.Drawing.Point(114, 38);
            this.textureFile.Name = "textureFile";
            this.textureFile.Size = new System.Drawing.Size(110, 20);
            this.textureFile.TabIndex = 37;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(230, 40);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(52, 17);
            this.checkBox1.TabIndex = 38;
            this.checkBox1.Text = "None";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 39;
            this.label2.Text = "Texture List:";
            // 
            // textureList
            // 
            this.textureList.Hexadecimal = true;
            this.textureList.Location = new System.Drawing.Point(83, 64);
            this.textureList.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.textureList.Name = "textureList";
            this.textureList.Size = new System.Drawing.Size(91, 20);
            this.textureList.TabIndex = 40;
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(54, 12);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(218, 20);
            this.label.TabIndex = 42;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Attributes:";
            // 
            // checkBoxAttributeAnimation
            // 
            this.checkBoxAttributeAnimation.AutoSize = true;
            this.checkBoxAttributeAnimation.Location = new System.Drawing.Point(83, 118);
            this.checkBoxAttributeAnimation.Name = "checkBoxAttributeAnimation";
            this.checkBoxAttributeAnimation.Size = new System.Drawing.Size(108, 17);
            this.checkBoxAttributeAnimation.TabIndex = 44;
            this.checkBoxAttributeAnimation.Text = "Enable Animation";
            this.checkBoxAttributeAnimation.UseVisualStyleBackColor = true;
            this.checkBoxAttributeAnimation.Click += new System.EventHandler(this.checkBoxAttributeAnimation_Click);
            // 
            // checkBoxAttributePVM
            // 
            this.checkBoxAttributePVM.AutoSize = true;
            this.checkBoxAttributePVM.Location = new System.Drawing.Point(202, 136);
            this.checkBoxAttributePVM.Name = "checkBoxAttributePVM";
            this.checkBoxAttributePVM.Size = new System.Drawing.Size(76, 17);
            this.checkBoxAttributePVM.TabIndex = 45;
            this.checkBoxAttributePVM.Text = "Load PVM";
            this.checkBoxAttributePVM.UseVisualStyleBackColor = true;
            this.checkBoxAttributePVM.Click += new System.EventHandler(this.checkBoxAttributePVM_Click);
            // 
            // checkBoxAttributeClip
            // 
            this.checkBoxAttributeClip.AutoSize = true;
            this.checkBoxAttributeClip.Location = new System.Drawing.Point(83, 136);
            this.checkBoxAttributeClip.Name = "checkBoxAttributeClip";
            this.checkBoxAttributeClip.Size = new System.Drawing.Size(110, 17);
            this.checkBoxAttributeClip.TabIndex = 46;
            this.checkBoxAttributeClip.Text = "Use Clip Distance";
            this.checkBoxAttributeClip.UseVisualStyleBackColor = true;
            this.checkBoxAttributeClip.Click += new System.EventHandler(this.checkBoxAttributeClip_Click);
            // 
            // checkBoxAttributeTexlist
            // 
            this.checkBoxAttributeTexlist.AutoSize = true;
            this.checkBoxAttributeTexlist.Location = new System.Drawing.Point(202, 117);
            this.checkBoxAttributeTexlist.Name = "checkBoxAttributeTexlist";
            this.checkBoxAttributeTexlist.Size = new System.Drawing.Size(83, 17);
            this.checkBoxAttributeTexlist.TabIndex = 47;
            this.checkBoxAttributeTexlist.Text = "Load Texlist";
            this.checkBoxAttributeTexlist.UseVisualStyleBackColor = true;
            this.checkBoxAttributeTexlist.Click += new System.EventHandler(this.checkBoxAttributeTexlist_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 48;
            this.label7.Text = "Clip Distance:";
            // 
            // numericUpDownClipDist
            // 
            this.numericUpDownClipDist.Location = new System.Drawing.Point(83, 90);
            this.numericUpDownClipDist.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericUpDownClipDist.Name = "numericUpDownClipDist";
            this.numericUpDownClipDist.Size = new System.Drawing.Size(91, 20);
            this.numericUpDownClipDist.TabIndex = 49;
            // 
            // labelAttribute
            // 
            this.labelAttribute.AutoSize = true;
            this.labelAttribute.Location = new System.Drawing.Point(45, 137);
            this.labelAttribute.Name = "labelAttribute";
            this.labelAttribute.Size = new System.Drawing.Size(25, 13);
            this.labelAttribute.TabIndex = 50;
            this.labelAttribute.Text = "0xC";
            // 
            // EditLevelInfoDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 319);
            this.Controls.Add(this.labelAttribute);
            this.Controls.Add(this.numericUpDownClipDist);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBoxAttributeTexlist);
            this.Controls.Add(this.checkBoxAttributeClip);
            this.Controls.Add(this.checkBoxAttributePVM);
            this.Controls.Add(this.checkBoxAttributeAnimation);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label);
            this.Controls.Add(label3);
            this.Controls.Add(this.textureList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.textureFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.description);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.author);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditLevelInfoDialog";
            this.ShowInTaskbar = false;
            this.Text = "Edit Level Info";
            this.Load += new System.EventHandler(this.EditLevelInfoDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.textureList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownClipDist)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox description;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox author;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textureFile;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown textureList;
		private System.Windows.Forms.TextBox label;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkBoxAttributeAnimation;
		private System.Windows.Forms.CheckBox checkBoxAttributePVM;
		private System.Windows.Forms.CheckBox checkBoxAttributeClip;
		private System.Windows.Forms.CheckBox checkBoxAttributeTexlist;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDownClipDist;
		private System.Windows.Forms.Label labelAttribute;
	}
}

