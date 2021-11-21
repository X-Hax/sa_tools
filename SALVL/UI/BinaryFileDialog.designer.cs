using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
namespace SAModel.SALVL
{
    partial class BinaryFileDialog : Form
    {

        //Form overrides dispose to clean up the component list.
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //Required by the Windows Form Designer

        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BinaryFileDialog));
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelKey = new System.Windows.Forms.Label();
            this.comboFileKeyHint = new System.Windows.Forms.ComboBox();
            this.labelAddress = new System.Windows.Forms.Label();
            this.checkBoxHexLevel = new System.Windows.Forms.CheckBox();
            this.comboLevelFormat = new System.Windows.Forms.ComboBox();
            this.labelFormat = new System.Windows.Forms.Label();
            this.checkBoxBigEndian = new System.Windows.Forms.CheckBox();
            this.numericKey = new System.Windows.Forms.NumericUpDown();
            this.numericAddress = new System.Windows.Forms.NumericUpDown();
            this.numericStartOffset = new System.Windows.Forms.NumericUpDown();
            this.labelStartOffset = new System.Windows.Forms.Label();
            this.checkBoxHexStartOffset = new System.Windows.Forms.CheckBox();
            this.checkBoxReverse = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(286, 138);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(78, 27);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            // 
            // labelKey
            // 
            this.labelKey.AutoSize = true;
            this.labelKey.Location = new System.Drawing.Point(46, 18);
            this.labelKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelKey.Name = "labelKey";
            this.labelKey.Size = new System.Drawing.Size(29, 15);
            this.labelKey.TabIndex = 1;
            this.labelKey.Text = "Key:";
            // 
            // comboFileKeyHint
            // 
            this.comboFileKeyHint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFileKeyHint.FormattingEnabled = true;
            this.comboFileKeyHint.Items.AddRange(new object[] {
            "EXE",
            "EXE (X360)",
            "DLL",
            "REL",
            "1ST_READ.BIN",
            "SA1 Level",
            "SA2 Level",
            "Memory Dump"});
            this.comboFileKeyHint.Location = new System.Drawing.Point(204, 16);
            this.comboFileKeyHint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboFileKeyHint.Name = "comboFileKeyHint";
            this.comboFileKeyHint.Size = new System.Drawing.Size(157, 23);
            this.comboFileKeyHint.TabIndex = 2;
            this.comboFileKeyHint.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(23, 47);
            this.labelAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(52, 15);
            this.labelAddress.TabIndex = 4;
            this.labelAddress.Text = "Address:";
            // 
            // checkBoxHexLevel
            // 
            this.checkBoxHexLevel.AutoSize = true;
            this.checkBoxHexLevel.Checked = true;
            this.checkBoxHexLevel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHexLevel.Location = new System.Drawing.Point(205, 46);
            this.checkBoxHexLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxHexLevel.Name = "checkBoxHexLevel";
            this.checkBoxHexLevel.Size = new System.Drawing.Size(47, 19);
            this.checkBoxHexLevel.TabIndex = 9;
            this.checkBoxHexLevel.Text = "Hex";
            this.checkBoxHexLevel.UseVisualStyleBackColor = true;
            this.checkBoxHexLevel.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
            // 
            // comboLevelFormat
            // 
            this.comboLevelFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLevelFormat.FormattingEnabled = true;
            this.comboLevelFormat.Items.AddRange(new object[] {
            "SA1/SADX GC",
            "SADX PC",
            "SA2",
            "SA2B/SA2 PC"});
            this.comboLevelFormat.Location = new System.Drawing.Point(83, 103);
            this.comboLevelFormat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboLevelFormat.Name = "comboLevelFormat";
            this.comboLevelFormat.Size = new System.Drawing.Size(114, 23);
            this.comboLevelFormat.TabIndex = 17;
            // 
            // labelFormat
            // 
            this.labelFormat.AutoSize = true;
            this.labelFormat.Location = new System.Drawing.Point(27, 106);
            this.labelFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFormat.Name = "labelFormat";
            this.labelFormat.Size = new System.Drawing.Size(48, 15);
            this.labelFormat.TabIndex = 16;
            this.labelFormat.Text = "Format:";
            // 
            // checkBoxBigEndian
            // 
            this.checkBoxBigEndian.AutoSize = true;
            this.checkBoxBigEndian.Location = new System.Drawing.Point(205, 105);
            this.checkBoxBigEndian.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxBigEndian.Name = "checkBoxBigEndian";
            this.checkBoxBigEndian.Size = new System.Drawing.Size(82, 19);
            this.checkBoxBigEndian.TabIndex = 18;
            this.checkBoxBigEndian.Text = "Big Endian";
            this.checkBoxBigEndian.UseVisualStyleBackColor = true;
            // 
            // numericKey
            // 
            this.numericKey.Hexadecimal = true;
            this.numericKey.Location = new System.Drawing.Point(83, 16);
            this.numericKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericKey.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericKey.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericKey.Name = "numericKey";
            this.numericKey.Size = new System.Drawing.Size(114, 23);
            this.numericKey.TabIndex = 15;
            this.numericKey.ValueChanged += new System.EventHandler(this.numericKey_ValueChanged_1);
            // 
            // numericAddress
            // 
            this.numericAddress.Hexadecimal = true;
            this.numericAddress.Location = new System.Drawing.Point(83, 45);
            this.numericAddress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericAddress.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericAddress.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericAddress.Name = "numericAddress";
            this.numericAddress.Size = new System.Drawing.Size(114, 23);
            this.numericAddress.TabIndex = 3;
            this.numericAddress.ValueChanged += new System.EventHandler(this.numericAddress_ValueChanged_1);
            // 
            // numericStartOffset
            // 
            this.numericStartOffset.Hexadecimal = true;
            this.numericStartOffset.Location = new System.Drawing.Point(83, 74);
            this.numericStartOffset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericStartOffset.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericStartOffset.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericStartOffset.Name = "numericStartOffset";
            this.numericStartOffset.Size = new System.Drawing.Size(114, 23);
            this.numericStartOffset.TabIndex = 19;
            this.numericStartOffset.ValueChanged += new System.EventHandler(this.numericStartOffset_ValueChanged);
            // 
            // labelStartOffset
            // 
            this.labelStartOffset.AutoSize = true;
            this.labelStartOffset.Location = new System.Drawing.Point(6, 76);
            this.labelStartOffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStartOffset.Name = "labelStartOffset";
            this.labelStartOffset.Size = new System.Drawing.Size(69, 15);
            this.labelStartOffset.TabIndex = 20;
            this.labelStartOffset.Text = "Start Offset:";
            // 
            // checkBoxHexStartOffset
            // 
            this.checkBoxHexStartOffset.AutoSize = true;
            this.checkBoxHexStartOffset.Checked = true;
            this.checkBoxHexStartOffset.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHexStartOffset.Location = new System.Drawing.Point(205, 75);
            this.checkBoxHexStartOffset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxHexStartOffset.Name = "checkBoxHexStartOffset";
            this.checkBoxHexStartOffset.Size = new System.Drawing.Size(47, 19);
            this.checkBoxHexStartOffset.TabIndex = 21;
            this.checkBoxHexStartOffset.Text = "Hex";
            this.checkBoxHexStartOffset.UseVisualStyleBackColor = true;
            this.checkBoxHexStartOffset.CheckedChanged += new System.EventHandler(this.checkBoxHexStartOffset_CheckedChanged);
            // 
            // checkBoxReverse
            // 
            this.checkBoxReverse.AutoSize = true;
            this.checkBoxReverse.Location = new System.Drawing.Point(295, 105);
            this.checkBoxReverse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxReverse.Name = "checkBoxReverse";
            this.checkBoxReverse.Size = new System.Drawing.Size(66, 19);
            this.checkBoxReverse.TabIndex = 22;
            this.checkBoxReverse.Text = "Reverse";
            this.checkBoxReverse.UseVisualStyleBackColor = true;
            // 
            // BinaryFileDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 177);
            this.Controls.Add(this.checkBoxReverse);
            this.Controls.Add(this.checkBoxHexStartOffset);
            this.Controls.Add(this.labelStartOffset);
            this.Controls.Add(this.numericStartOffset);
            this.Controls.Add(this.checkBoxBigEndian);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboLevelFormat);
            this.Controls.Add(this.labelFormat);
            this.Controls.Add(this.numericKey);
            this.Controls.Add(this.checkBoxHexLevel);
            this.Controls.Add(this.labelAddress);
            this.Controls.Add(this.numericAddress);
            this.Controls.Add(this.comboFileKeyHint);
            this.Controls.Add(this.labelKey);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BinaryFileDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Load Level from a Binary File";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.BinaryFileDialog_HelpButtonClicked);
            this.Load += new System.EventHandler(this.Dialog1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartOffset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal Button buttonOK;
        internal Label labelKey;
        internal ComboBox comboFileKeyHint;
        internal Label labelAddress;
        internal CheckBox checkBoxHexLevel;
        internal System.Windows.Forms.NumericUpDown numericKey;
        internal ComboBox comboLevelFormat;
        internal Label labelFormat;
		internal CheckBox checkBoxBigEndian;
		internal System.Windows.Forms.NumericUpDown numericAddress;
		internal NumericUpDown numericStartOffset;
		internal Label labelStartOffset;
		internal CheckBox checkBoxHexStartOffset;
		internal CheckBox checkBoxReverse;
	}
}