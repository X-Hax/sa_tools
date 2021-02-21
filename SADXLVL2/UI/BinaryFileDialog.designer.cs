using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
namespace SonicRetro.SAModel.SADXLVL2
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
            this.checkboxHex = new System.Windows.Forms.CheckBox();
            this.comboLevelFormat = new System.Windows.Forms.ComboBox();
            this.labelFormat = new System.Windows.Forms.Label();
            this.checkBoxBigEndian = new System.Windows.Forms.CheckBox();
            this.numericKey = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.numericAddress = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            ((System.ComponentModel.ISupportInitialize)(this.numericKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(225, 63);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(67, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            // 
            // labelKey
            // 
            this.labelKey.AutoSize = true;
            this.labelKey.Location = new System.Drawing.Point(12, 13);
            this.labelKey.Name = "labelKey";
            this.labelKey.Size = new System.Drawing.Size(28, 13);
            this.labelKey.TabIndex = 1;
            this.labelKey.Text = "Key:";
            // 
            // comboFileKeyHint
            // 
            this.comboFileKeyHint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFileKeyHint.FormattingEnabled = true;
            this.comboFileKeyHint.Items.AddRange(new object[] {
            "EXE",
            "DLL",
            "1st_read.bin",
            "SA1 Level",
            "SA2 Level",
            "Memory Dump"});
            this.comboFileKeyHint.Location = new System.Drawing.Point(170, 10);
            this.comboFileKeyHint.Name = "comboFileKeyHint";
            this.comboFileKeyHint.Size = new System.Drawing.Size(121, 21);
            this.comboFileKeyHint.TabIndex = 2;
            this.comboFileKeyHint.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(12, 39);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(48, 13);
            this.labelAddress.TabIndex = 4;
            this.labelAddress.Text = "Address:";
            // 
            // checkboxHex
            // 
            this.checkboxHex.AutoSize = true;
            this.checkboxHex.Checked = true;
            this.checkboxHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxHex.Location = new System.Drawing.Point(170, 40);
            this.checkboxHex.Name = "checkboxHex";
            this.checkboxHex.Size = new System.Drawing.Size(45, 17);
            this.checkboxHex.TabIndex = 9;
            this.checkboxHex.Text = "Hex";
            this.checkboxHex.UseVisualStyleBackColor = true;
            this.checkboxHex.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
            // 
            // comboLevelFormat
            // 
            this.comboLevelFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLevelFormat.FormattingEnabled = true;
            this.comboLevelFormat.Items.AddRange(new object[] {
            "SA1/SADX Gamecube",
            "SADX PC",
            "SA2",
            "SA2B/SA2PC"});
            this.comboLevelFormat.Location = new System.Drawing.Point(66, 63);
            this.comboLevelFormat.Name = "comboLevelFormat";
            this.comboLevelFormat.Size = new System.Drawing.Size(149, 21);
            this.comboLevelFormat.TabIndex = 17;
            // 
            // labelFormat
            // 
            this.labelFormat.AutoSize = true;
            this.labelFormat.Location = new System.Drawing.Point(12, 66);
            this.labelFormat.Name = "labelFormat";
            this.labelFormat.Size = new System.Drawing.Size(42, 13);
            this.labelFormat.TabIndex = 16;
            this.labelFormat.Text = "Format:";
            // 
            // checkBoxBigEndian
            // 
            this.checkBoxBigEndian.AutoSize = true;
            this.checkBoxBigEndian.Location = new System.Drawing.Point(221, 40);
            this.checkBoxBigEndian.Name = "checkBoxBigEndian";
            this.checkBoxBigEndian.Size = new System.Drawing.Size(77, 17);
            this.checkBoxBigEndian.TabIndex = 18;
            this.checkBoxBigEndian.Text = "Big Endian";
            this.checkBoxBigEndian.UseVisualStyleBackColor = true;
            // 
            // numericKey
            // 
            this.numericKey.Hexadecimal = true;
            this.numericKey.Location = new System.Drawing.Point(66, 11);
            this.numericKey.Name = "numericKey";
            this.numericKey.Size = new System.Drawing.Size(98, 20);
            this.numericKey.TabIndex = 15;
            // 
            // numericAddress
            // 
            this.numericAddress.Hexadecimal = true;
            this.numericAddress.Location = new System.Drawing.Point(66, 37);
            this.numericAddress.Name = "numericAddress";
            this.numericAddress.Size = new System.Drawing.Size(98, 20);
            this.numericAddress.TabIndex = 3;
            // 
            // BinaryFileDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 91);
            this.Controls.Add(this.checkBoxBigEndian);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboLevelFormat);
            this.Controls.Add(this.labelFormat);
            this.Controls.Add(this.numericKey);
            this.Controls.Add(this.checkboxHex);
            this.Controls.Add(this.labelAddress);
            this.Controls.Add(this.numericAddress);
            this.Controls.Add(this.comboFileKeyHint);
            this.Controls.Add(this.labelKey);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal Button buttonOK;
        internal Label labelKey;
        internal ComboBox comboFileKeyHint;
        internal Label labelAddress;
        internal CheckBox checkboxHex;
        internal SAEditorCommon.UI.HexNumericUpdown numericKey;
        internal ComboBox comboLevelFormat;
        internal Label labelFormat;
		internal CheckBox checkBoxBigEndian;
		internal SAEditorCommon.UI.HexNumericUpdown numericAddress;
	}
}