using System.Windows.Forms;

namespace SAModel.SAMDL
{
    partial class ModelFileDialog : Form
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelFileDialog));
			OK_Button = new Button();
			labelKey = new Label();
			comboBoxBinaryFileType = new ComboBox();
			labelModelAddress = new Label();
			checkBoxHexObject = new CheckBox();
			comboBoxModelFormat = new ComboBox();
			labelModelFormat = new Label();
			checkBoxLoadMotion = new CheckBox();
			labelMotionAddress = new Label();
			checkBoxBigEndian = new CheckBox();
			radioButtonBinary = new RadioButton();
			radioButtonSA2MDL = new RadioButton();
			radioButtonSA2BMDL = new RadioButton();
			labelType = new Label();
			checkBoxMemoryObject = new CheckBox();
			labelStructure = new Label();
			radioButtonObject = new RadioButton();
			radioButtonAction = new RadioButton();
			checkBoxMemoryMotion = new CheckBox();
			checkBoxHexMotion = new CheckBox();
			groupBoxBinaryData = new GroupBox();
			checkBoxHexStartOffset = new CheckBox();
			numericUpDownStartOffset = new NumericUpDown();
			labelStartOffset = new Label();
			checkBoxReverse = new CheckBox();
			radioButtonAttach = new RadioButton();
			numericUpDownKey = new NumericUpDown();
			numericUpDownMotionAddress = new NumericUpDown();
			numericUpDownModelAddress = new NumericUpDown();
			groupBoxBinaryData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownStartOffset).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownKey).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownMotionAddress).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownModelAddress).BeginInit();
			SuspendLayout();
			// 
			// OK_Button
			// 
			OK_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			OK_Button.DialogResult = DialogResult.OK;
			OK_Button.Location = new System.Drawing.Point(318, 308);
			OK_Button.Margin = new Padding(4, 3, 4, 3);
			OK_Button.Name = "OK_Button";
			OK_Button.Size = new System.Drawing.Size(78, 27);
			OK_Button.TabIndex = 4;
			OK_Button.Text = "&OK";
			// 
			// labelKey
			// 
			labelKey.AutoSize = true;
			labelKey.Location = new System.Drawing.Point(49, 25);
			labelKey.Margin = new Padding(4, 0, 4, 0);
			labelKey.Name = "labelKey";
			labelKey.Size = new System.Drawing.Size(29, 15);
			labelKey.TabIndex = 1;
			labelKey.Text = "Key:";
			// 
			// comboBoxBinaryFileType
			// 
			comboBoxBinaryFileType.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxBinaryFileType.FormattingEnabled = true;
			comboBoxBinaryFileType.Location = new System.Drawing.Point(212, 22);
			comboBoxBinaryFileType.Margin = new Padding(4, 3, 4, 3);
			comboBoxBinaryFileType.Name = "comboBoxBinaryFileType";
			comboBoxBinaryFileType.Size = new System.Drawing.Size(140, 23);
			comboBoxBinaryFileType.TabIndex = 1;
			comboBoxBinaryFileType.SelectedIndexChanged += ComboBox_FileType_SelectedIndexChanged;
			// 
			// labelModelAddress
			// 
			labelModelAddress.AutoSize = true;
			labelModelAddress.Location = new System.Drawing.Point(26, 57);
			labelModelAddress.Margin = new Padding(4, 0, 4, 0);
			labelModelAddress.Name = "labelModelAddress";
			labelModelAddress.Size = new System.Drawing.Size(52, 15);
			labelModelAddress.TabIndex = 4;
			labelModelAddress.Text = "Address:";
			// 
			// checkBoxHexObject
			// 
			checkBoxHexObject.AutoSize = true;
			checkBoxHexObject.Checked = true;
			checkBoxHexObject.CheckState = CheckState.Checked;
			checkBoxHexObject.Location = new System.Drawing.Point(212, 57);
			checkBoxHexObject.Margin = new Padding(4, 3, 4, 3);
			checkBoxHexObject.Name = "checkBoxHexObject";
			checkBoxHexObject.Size = new System.Drawing.Size(46, 19);
			checkBoxHexObject.TabIndex = 3;
			checkBoxHexObject.Text = "Hex";
			checkBoxHexObject.UseVisualStyleBackColor = true;
			checkBoxHexObject.CheckedChanged += CheckBox_Hex_Object_CheckedChanged;
			// 
			// comboBoxModelFormat
			// 
			comboBoxModelFormat.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxModelFormat.FormattingEnabled = true;
			comboBoxModelFormat.Items.AddRange(new object[] { "Basic", "Basic+", "Chunk", "Ginja", "XJ" });
			comboBoxModelFormat.Location = new System.Drawing.Point(86, 121);
			comboBoxModelFormat.Margin = new Padding(4, 3, 4, 3);
			comboBoxModelFormat.Name = "comboBoxModelFormat";
			comboBoxModelFormat.Size = new System.Drawing.Size(114, 23);
			comboBoxModelFormat.TabIndex = 7;
			// 
			// labelModelFormat
			// 
			labelModelFormat.AutoSize = true;
			labelModelFormat.Location = new System.Drawing.Point(30, 124);
			labelModelFormat.Margin = new Padding(4, 0, 4, 0);
			labelModelFormat.Name = "labelModelFormat";
			labelModelFormat.Size = new System.Drawing.Size(48, 15);
			labelModelFormat.TabIndex = 16;
			labelModelFormat.Text = "Format:";
			// 
			// checkBoxLoadMotion
			// 
			checkBoxLoadMotion.AutoSize = true;
			checkBoxLoadMotion.Location = new System.Drawing.Point(21, 193);
			checkBoxLoadMotion.Margin = new Padding(4, 3, 4, 3);
			checkBoxLoadMotion.Name = "checkBoxLoadMotion";
			checkBoxLoadMotion.Size = new System.Drawing.Size(94, 19);
			checkBoxLoadMotion.TabIndex = 13;
			checkBoxLoadMotion.Text = "Load Motion";
			checkBoxLoadMotion.UseVisualStyleBackColor = true;
			checkBoxLoadMotion.CheckedChanged += CheckBox_LoadMotion_CheckedChanged;
			// 
			// labelMotionAddress
			// 
			labelMotionAddress.AutoSize = true;
			labelMotionAddress.Location = new System.Drawing.Point(21, 222);
			labelMotionAddress.Margin = new Padding(4, 0, 4, 0);
			labelMotionAddress.Name = "labelMotionAddress";
			labelMotionAddress.Size = new System.Drawing.Size(52, 15);
			labelMotionAddress.TabIndex = 20;
			labelMotionAddress.Text = "Address:";
			// 
			// checkBoxBigEndian
			// 
			checkBoxBigEndian.AutoSize = true;
			checkBoxBigEndian.Location = new System.Drawing.Point(212, 124);
			checkBoxBigEndian.Margin = new Padding(4, 3, 4, 3);
			checkBoxBigEndian.Name = "checkBoxBigEndian";
			checkBoxBigEndian.Size = new System.Drawing.Size(82, 19);
			checkBoxBigEndian.TabIndex = 8;
			checkBoxBigEndian.Text = "Big Endian";
			checkBoxBigEndian.UseVisualStyleBackColor = true;
			// 
			// radioButtonBinary
			// 
			radioButtonBinary.AutoSize = true;
			radioButtonBinary.Checked = true;
			radioButtonBinary.Location = new System.Drawing.Point(61, 14);
			radioButtonBinary.Margin = new Padding(4, 3, 4, 3);
			radioButtonBinary.Name = "radioButtonBinary";
			radioButtonBinary.Size = new System.Drawing.Size(58, 19);
			radioButtonBinary.TabIndex = 220;
			radioButtonBinary.TabStop = true;
			radioButtonBinary.Text = "Binary";
			radioButtonBinary.UseVisualStyleBackColor = true;
			radioButtonBinary.CheckedChanged += RadioButton_Binary_CheckedChanged;
			// 
			// radioButtonSA2MDL
			// 
			radioButtonSA2MDL.AutoSize = true;
			radioButtonSA2MDL.Location = new System.Drawing.Point(131, 14);
			radioButtonSA2MDL.Margin = new Padding(4, 3, 4, 3);
			radioButtonSA2MDL.Name = "radioButtonSA2MDL";
			radioButtonSA2MDL.Size = new System.Drawing.Size(94, 19);
			radioButtonSA2MDL.TabIndex = 1;
			radioButtonSA2MDL.Text = "SA2 MDL File";
			radioButtonSA2MDL.UseVisualStyleBackColor = true;
			// 
			// radioButtonSA2BMDL
			// 
			radioButtonSA2BMDL.AutoSize = true;
			radioButtonSA2BMDL.Location = new System.Drawing.Point(239, 14);
			radioButtonSA2BMDL.Margin = new Padding(4, 3, 4, 3);
			radioButtonSA2BMDL.Name = "radioButtonSA2BMDL";
			radioButtonSA2BMDL.Size = new System.Drawing.Size(101, 19);
			radioButtonSA2BMDL.TabIndex = 2;
			radioButtonSA2BMDL.Text = "SA2B MDL File";
			radioButtonSA2BMDL.UseVisualStyleBackColor = true;
			// 
			// labelType
			// 
			labelType.AutoSize = true;
			labelType.Location = new System.Drawing.Point(14, 16);
			labelType.Margin = new Padding(4, 0, 4, 0);
			labelType.Name = "labelType";
			labelType.Size = new System.Drawing.Size(35, 15);
			labelType.TabIndex = 25;
			labelType.Text = "Type:";
			// 
			// checkBoxMemoryObject
			// 
			checkBoxMemoryObject.AutoSize = true;
			checkBoxMemoryObject.Location = new System.Drawing.Point(267, 57);
			checkBoxMemoryObject.Margin = new Padding(4, 3, 4, 3);
			checkBoxMemoryObject.Name = "checkBoxMemoryObject";
			checkBoxMemoryObject.Size = new System.Drawing.Size(71, 19);
			checkBoxMemoryObject.TabIndex = 4;
			checkBoxMemoryObject.Text = "Memory";
			checkBoxMemoryObject.UseVisualStyleBackColor = true;
			// 
			// labelStructure
			// 
			labelStructure.AutoSize = true;
			labelStructure.Location = new System.Drawing.Point(17, 162);
			labelStructure.Margin = new Padding(4, 0, 4, 0);
			labelStructure.Name = "labelStructure";
			labelStructure.Size = new System.Drawing.Size(58, 15);
			labelStructure.TabIndex = 27;
			labelStructure.Text = "Structure:";
			// 
			// radioButtonObject
			// 
			radioButtonObject.AutoSize = true;
			radioButtonObject.Checked = true;
			radioButtonObject.Location = new System.Drawing.Point(84, 159);
			radioButtonObject.Margin = new Padding(4, 3, 4, 3);
			radioButtonObject.Name = "radioButtonObject";
			radioButtonObject.Size = new System.Drawing.Size(60, 19);
			radioButtonObject.TabIndex = 10;
			radioButtonObject.TabStop = true;
			radioButtonObject.Text = "Object";
			radioButtonObject.UseVisualStyleBackColor = true;
			radioButtonObject.CheckedChanged += RadioButton_Object_CheckedChanged;
			// 
			// radioButtonAction
			// 
			radioButtonAction.AutoSize = true;
			radioButtonAction.Location = new System.Drawing.Point(156, 159);
			radioButtonAction.Margin = new Padding(4, 3, 4, 3);
			radioButtonAction.Name = "radioButtonAction";
			radioButtonAction.Size = new System.Drawing.Size(60, 19);
			radioButtonAction.TabIndex = 11;
			radioButtonAction.Text = "Action";
			radioButtonAction.UseVisualStyleBackColor = true;
			// 
			// checkBoxMemoryMotion
			// 
			checkBoxMemoryMotion.AutoSize = true;
			checkBoxMemoryMotion.Enabled = false;
			checkBoxMemoryMotion.Location = new System.Drawing.Point(267, 220);
			checkBoxMemoryMotion.Margin = new Padding(4, 3, 4, 3);
			checkBoxMemoryMotion.Name = "checkBoxMemoryMotion";
			checkBoxMemoryMotion.Size = new System.Drawing.Size(71, 19);
			checkBoxMemoryMotion.TabIndex = 16;
			checkBoxMemoryMotion.Text = "Memory";
			checkBoxMemoryMotion.UseVisualStyleBackColor = true;
			// 
			// checkBoxHexMotion
			// 
			checkBoxHexMotion.AutoSize = true;
			checkBoxHexMotion.Checked = true;
			checkBoxHexMotion.CheckState = CheckState.Checked;
			checkBoxHexMotion.Enabled = false;
			checkBoxHexMotion.Location = new System.Drawing.Point(212, 220);
			checkBoxHexMotion.Margin = new Padding(4, 3, 4, 3);
			checkBoxHexMotion.Name = "checkBoxHexMotion";
			checkBoxHexMotion.Size = new System.Drawing.Size(46, 19);
			checkBoxHexMotion.TabIndex = 15;
			checkBoxHexMotion.Text = "Hex";
			checkBoxHexMotion.UseVisualStyleBackColor = true;
			checkBoxHexMotion.CheckedChanged += CheckBox_Hex_Motion_CheckedChanged;
			// 
			// groupBoxBinaryData
			// 
			groupBoxBinaryData.Controls.Add(checkBoxHexStartOffset);
			groupBoxBinaryData.Controls.Add(numericUpDownStartOffset);
			groupBoxBinaryData.Controls.Add(labelStartOffset);
			groupBoxBinaryData.Controls.Add(checkBoxReverse);
			groupBoxBinaryData.Controls.Add(radioButtonAttach);
			groupBoxBinaryData.Controls.Add(checkBoxHexMotion);
			groupBoxBinaryData.Controls.Add(comboBoxModelFormat);
			groupBoxBinaryData.Controls.Add(checkBoxBigEndian);
			groupBoxBinaryData.Controls.Add(checkBoxLoadMotion);
			groupBoxBinaryData.Controls.Add(numericUpDownKey);
			groupBoxBinaryData.Controls.Add(checkBoxMemoryMotion);
			groupBoxBinaryData.Controls.Add(labelModelFormat);
			groupBoxBinaryData.Controls.Add(numericUpDownMotionAddress);
			groupBoxBinaryData.Controls.Add(labelMotionAddress);
			groupBoxBinaryData.Controls.Add(radioButtonAction);
			groupBoxBinaryData.Controls.Add(labelKey);
			groupBoxBinaryData.Controls.Add(checkBoxMemoryObject);
			groupBoxBinaryData.Controls.Add(radioButtonObject);
			groupBoxBinaryData.Controls.Add(comboBoxBinaryFileType);
			groupBoxBinaryData.Controls.Add(labelStructure);
			groupBoxBinaryData.Controls.Add(numericUpDownModelAddress);
			groupBoxBinaryData.Controls.Add(labelModelAddress);
			groupBoxBinaryData.Controls.Add(checkBoxHexObject);
			groupBoxBinaryData.Location = new System.Drawing.Point(14, 40);
			groupBoxBinaryData.Margin = new Padding(4, 3, 4, 3);
			groupBoxBinaryData.Name = "groupBoxBinaryData";
			groupBoxBinaryData.Padding = new Padding(4, 3, 4, 3);
			groupBoxBinaryData.Size = new System.Drawing.Size(380, 258);
			groupBoxBinaryData.TabIndex = 3;
			groupBoxBinaryData.TabStop = false;
			groupBoxBinaryData.Text = "Binary data";
			// 
			// checkBoxHexStartOffset
			// 
			checkBoxHexStartOffset.AutoSize = true;
			checkBoxHexStartOffset.Checked = true;
			checkBoxHexStartOffset.CheckState = CheckState.Checked;
			checkBoxHexStartOffset.Location = new System.Drawing.Point(212, 90);
			checkBoxHexStartOffset.Margin = new Padding(4, 3, 4, 3);
			checkBoxHexStartOffset.Name = "checkBoxHexStartOffset";
			checkBoxHexStartOffset.Size = new System.Drawing.Size(46, 19);
			checkBoxHexStartOffset.TabIndex = 6;
			checkBoxHexStartOffset.Text = "Hex";
			checkBoxHexStartOffset.UseVisualStyleBackColor = true;
			checkBoxHexStartOffset.CheckedChanged += checkBoxHexStartOffset_CheckedChanged;
			// 
			// numericUpDownStartOffset
			// 
			numericUpDownStartOffset.Hexadecimal = true;
			numericUpDownStartOffset.Location = new System.Drawing.Point(86, 88);
			numericUpDownStartOffset.Margin = new Padding(4, 3, 4, 3);
			numericUpDownStartOffset.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
			numericUpDownStartOffset.Minimum = new decimal(new int[] { -1, 0, 0, int.MinValue });
			numericUpDownStartOffset.Name = "numericUpDownStartOffset";
			numericUpDownStartOffset.Size = new System.Drawing.Size(114, 23);
			numericUpDownStartOffset.TabIndex = 5;
			numericUpDownStartOffset.ValueChanged += numericUpDownStartOffset_ValueChanged;
			// 
			// labelStartOffset
			// 
			labelStartOffset.AutoSize = true;
			labelStartOffset.Location = new System.Drawing.Point(9, 90);
			labelStartOffset.Margin = new Padding(4, 0, 4, 0);
			labelStartOffset.Name = "labelStartOffset";
			labelStartOffset.Size = new System.Drawing.Size(69, 15);
			labelStartOffset.TabIndex = 34;
			labelStartOffset.Text = "Start Offset:";
			// 
			// checkBoxReverse
			// 
			checkBoxReverse.AutoSize = true;
			checkBoxReverse.Location = new System.Drawing.Point(302, 124);
			checkBoxReverse.Margin = new Padding(4, 3, 4, 3);
			checkBoxReverse.Name = "checkBoxReverse";
			checkBoxReverse.Size = new System.Drawing.Size(66, 19);
			checkBoxReverse.TabIndex = 9;
			checkBoxReverse.Text = "Reverse";
			checkBoxReverse.UseVisualStyleBackColor = true;
			// 
			// radioButtonAttach
			// 
			radioButtonAttach.AutoSize = true;
			radioButtonAttach.Location = new System.Drawing.Point(227, 159);
			radioButtonAttach.Margin = new Padding(4, 3, 4, 3);
			radioButtonAttach.Name = "radioButtonAttach";
			radioButtonAttach.Size = new System.Drawing.Size(105, 19);
			radioButtonAttach.TabIndex = 12;
			radioButtonAttach.Text = "Model (Attach)";
			radioButtonAttach.UseVisualStyleBackColor = true;
			// 
			// numericUpDownKey
			// 
			numericUpDownKey.Hexadecimal = true;
			numericUpDownKey.Location = new System.Drawing.Point(86, 22);
			numericUpDownKey.Margin = new Padding(4, 3, 4, 3);
			numericUpDownKey.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
			numericUpDownKey.Minimum = new decimal(new int[] { -1, 0, 0, int.MinValue });
			numericUpDownKey.Name = "numericUpDownKey";
			numericUpDownKey.Size = new System.Drawing.Size(114, 23);
			numericUpDownKey.TabIndex = 0;
			numericUpDownKey.ValueChanged += numericUpDownKey_ValueChanged;
			// 
			// numericUpDownMotionAddress
			// 
			numericUpDownMotionAddress.Enabled = false;
			numericUpDownMotionAddress.Hexadecimal = true;
			numericUpDownMotionAddress.Location = new System.Drawing.Point(84, 219);
			numericUpDownMotionAddress.Margin = new Padding(4, 3, 4, 3);
			numericUpDownMotionAddress.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
			numericUpDownMotionAddress.Minimum = new decimal(new int[] { -1, 0, 0, int.MinValue });
			numericUpDownMotionAddress.Name = "numericUpDownMotionAddress";
			numericUpDownMotionAddress.Size = new System.Drawing.Size(114, 23);
			numericUpDownMotionAddress.TabIndex = 14;
			numericUpDownMotionAddress.ValueChanged += numericUpDownMotionAddress_ValueChanged;
			// 
			// numericUpDownModelAddress
			// 
			numericUpDownModelAddress.Hexadecimal = true;
			numericUpDownModelAddress.Location = new System.Drawing.Point(86, 55);
			numericUpDownModelAddress.Margin = new Padding(4, 3, 4, 3);
			numericUpDownModelAddress.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
			numericUpDownModelAddress.Minimum = new decimal(new int[] { -1, 0, 0, int.MinValue });
			numericUpDownModelAddress.Name = "numericUpDownModelAddress";
			numericUpDownModelAddress.Size = new System.Drawing.Size(114, 23);
			numericUpDownModelAddress.TabIndex = 2;
			numericUpDownModelAddress.ValueChanged += numericUpDownModelAddress_ValueChanged;
			// 
			// ModelFileDialog
			// 
			AcceptButton = OK_Button;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(409, 347);
			Controls.Add(labelType);
			Controls.Add(radioButtonBinary);
			Controls.Add(groupBoxBinaryData);
			Controls.Add(radioButtonSA2MDL);
			Controls.Add(radioButtonSA2BMDL);
			Controls.Add(OK_Button);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			HelpButton = true;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ModelFileDialog";
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Load from a Binary File";
			HelpButtonClicked += ModelFileDialog_HelpButtonClicked;
			groupBoxBinaryData.ResumeLayout(false);
			groupBoxBinaryData.PerformLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownStartOffset).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownKey).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownMotionAddress).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDownModelAddress).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}
		internal Button OK_Button;
        internal Label labelKey;
        internal ComboBox comboBoxBinaryFileType;
        internal Label labelModelAddress;
        internal CheckBox checkBoxHexObject;
        internal System.Windows.Forms.NumericUpDown numericUpDownKey;
        internal ComboBox comboBoxModelFormat;
        internal Label labelModelFormat;
        internal CheckBox checkBoxLoadMotion;
        private Label labelMotionAddress;
		internal CheckBox checkBoxBigEndian;
		internal RadioButton radioButtonBinary;
		internal RadioButton radioButtonSA2MDL;
		internal RadioButton radioButtonSA2BMDL;
		private Label labelType;
		internal CheckBox checkBoxMemoryObject;
		internal Label labelStructure;
		internal RadioButton radioButtonObject;
		internal RadioButton radioButtonAction;
		internal CheckBox checkBoxMemoryMotion;
		internal CheckBox checkBoxHexMotion;
		private GroupBox groupBoxBinaryData;
		internal RadioButton radioButtonAttach;
		internal System.Windows.Forms.NumericUpDown numericUpDownModelAddress;
		internal System.Windows.Forms.NumericUpDown numericUpDownMotionAddress;
		internal CheckBox checkBoxReverse;
		internal CheckBox checkBoxHexStartOffset;
		internal NumericUpDown numericUpDownStartOffset;
		internal Label labelStartOffset;
	}
}