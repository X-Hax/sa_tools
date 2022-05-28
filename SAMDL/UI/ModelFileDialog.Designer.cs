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
            this.OK_Button = new System.Windows.Forms.Button();
            this.labelKey = new System.Windows.Forms.Label();
            this.comboBoxBinaryFileType = new System.Windows.Forms.ComboBox();
            this.labelModelAddress = new System.Windows.Forms.Label();
            this.checkBoxHexObject = new System.Windows.Forms.CheckBox();
            this.comboBoxModelFormat = new System.Windows.Forms.ComboBox();
            this.labelModelFormat = new System.Windows.Forms.Label();
            this.checkBoxLoadMotion = new System.Windows.Forms.CheckBox();
            this.labelMotionAddress = new System.Windows.Forms.Label();
            this.checkBoxBigEndian = new System.Windows.Forms.CheckBox();
            this.radioButtonBinary = new System.Windows.Forms.RadioButton();
            this.radioButtonSA2MDL = new System.Windows.Forms.RadioButton();
            this.radioButtonSA2BMDL = new System.Windows.Forms.RadioButton();
            this.labelType = new System.Windows.Forms.Label();
            this.checkBoxMemoryObject = new System.Windows.Forms.CheckBox();
            this.labelStructure = new System.Windows.Forms.Label();
            this.radioButtonObject = new System.Windows.Forms.RadioButton();
            this.radioButtonAction = new System.Windows.Forms.RadioButton();
            this.checkBoxMemoryMotion = new System.Windows.Forms.CheckBox();
            this.checkBoxHexMotion = new System.Windows.Forms.CheckBox();
            this.groupBoxBinaryData = new System.Windows.Forms.GroupBox();
            this.checkBoxHexStartOffset = new System.Windows.Forms.CheckBox();
            this.numericUpDownStartOffset = new System.Windows.Forms.NumericUpDown();
            this.labelStartOffset = new System.Windows.Forms.Label();
            this.checkBoxReverse = new System.Windows.Forms.CheckBox();
            this.radioButtonAttach = new System.Windows.Forms.RadioButton();
            this.numericUpDownKey = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMotionAddress = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownModelAddress = new System.Windows.Forms.NumericUpDown();
            this.groupBoxBinaryData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotionAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModelAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // OK_Button
            // 
            this.OK_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK_Button.Location = new System.Drawing.Point(318, 308);
            this.OK_Button.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(78, 27);
            this.OK_Button.TabIndex = 0;
            this.OK_Button.Text = "&OK";
            // 
            // labelKey
            // 
            this.labelKey.AutoSize = true;
            this.labelKey.Location = new System.Drawing.Point(49, 25);
            this.labelKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelKey.Name = "labelKey";
            this.labelKey.Size = new System.Drawing.Size(29, 15);
            this.labelKey.TabIndex = 1;
            this.labelKey.Text = "Key:";
            // 
            // comboBoxBinaryFileType
            // 
            this.comboBoxBinaryFileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBinaryFileType.FormattingEnabled = true;
            this.comboBoxBinaryFileType.Location = new System.Drawing.Point(212, 22);
            this.comboBoxBinaryFileType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBoxBinaryFileType.Name = "comboBoxBinaryFileType";
            this.comboBoxBinaryFileType.Size = new System.Drawing.Size(140, 23);
            this.comboBoxBinaryFileType.TabIndex = 2;
            this.comboBoxBinaryFileType.SelectedIndexChanged += new System.EventHandler(this.ComboBox_FileType_SelectedIndexChanged);
            // 
            // labelModelAddress
            // 
            this.labelModelAddress.AutoSize = true;
            this.labelModelAddress.Location = new System.Drawing.Point(26, 57);
            this.labelModelAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelModelAddress.Name = "labelModelAddress";
            this.labelModelAddress.Size = new System.Drawing.Size(52, 15);
            this.labelModelAddress.TabIndex = 4;
            this.labelModelAddress.Text = "Address:";
            // 
            // checkBoxHexObject
            // 
            this.checkBoxHexObject.AutoSize = true;
            this.checkBoxHexObject.Checked = true;
            this.checkBoxHexObject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHexObject.Location = new System.Drawing.Point(212, 57);
            this.checkBoxHexObject.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxHexObject.Name = "checkBoxHexObject";
            this.checkBoxHexObject.Size = new System.Drawing.Size(47, 19);
            this.checkBoxHexObject.TabIndex = 9;
            this.checkBoxHexObject.Text = "Hex";
            this.checkBoxHexObject.UseVisualStyleBackColor = true;
            this.checkBoxHexObject.CheckedChanged += new System.EventHandler(this.CheckBox_Hex_Object_CheckedChanged);
            // 
            // comboBoxModelFormat
            // 
            this.comboBoxModelFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxModelFormat.FormattingEnabled = true;
            this.comboBoxModelFormat.Items.AddRange(new object[] {
            "Basic",
            "SADX Basic",
            "Chunk",
            "GC",
			"XJ"});
            this.comboBoxModelFormat.Location = new System.Drawing.Point(86, 121);
            this.comboBoxModelFormat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBoxModelFormat.Name = "comboBoxModelFormat";
            this.comboBoxModelFormat.Size = new System.Drawing.Size(114, 23);
            this.comboBoxModelFormat.TabIndex = 17;
            // 
            // labelModelFormat
            // 
            this.labelModelFormat.AutoSize = true;
            this.labelModelFormat.Location = new System.Drawing.Point(30, 124);
            this.labelModelFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelModelFormat.Name = "labelModelFormat";
            this.labelModelFormat.Size = new System.Drawing.Size(48, 15);
            this.labelModelFormat.TabIndex = 16;
            this.labelModelFormat.Text = "Format:";
            // 
            // checkBoxLoadMotion
            // 
            this.checkBoxLoadMotion.AutoSize = true;
            this.checkBoxLoadMotion.Location = new System.Drawing.Point(21, 193);
            this.checkBoxLoadMotion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxLoadMotion.Name = "checkBoxLoadMotion";
            this.checkBoxLoadMotion.Size = new System.Drawing.Size(94, 19);
            this.checkBoxLoadMotion.TabIndex = 18;
            this.checkBoxLoadMotion.Text = "Load Motion";
            this.checkBoxLoadMotion.UseVisualStyleBackColor = true;
            this.checkBoxLoadMotion.CheckedChanged += new System.EventHandler(this.CheckBox_LoadMotion_CheckedChanged);
            // 
            // labelMotionAddress
            // 
            this.labelMotionAddress.AutoSize = true;
            this.labelMotionAddress.Location = new System.Drawing.Point(21, 222);
            this.labelMotionAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMotionAddress.Name = "labelMotionAddress";
            this.labelMotionAddress.Size = new System.Drawing.Size(52, 15);
            this.labelMotionAddress.TabIndex = 20;
            this.labelMotionAddress.Text = "Address:";
            // 
            // checkBoxBigEndian
            // 
            this.checkBoxBigEndian.AutoSize = true;
            this.checkBoxBigEndian.Location = new System.Drawing.Point(212, 124);
            this.checkBoxBigEndian.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxBigEndian.Name = "checkBoxBigEndian";
            this.checkBoxBigEndian.Size = new System.Drawing.Size(82, 19);
            this.checkBoxBigEndian.TabIndex = 21;
            this.checkBoxBigEndian.Text = "Big Endian";
            this.checkBoxBigEndian.UseVisualStyleBackColor = true;
            // 
            // radioButtonBinary
            // 
            this.radioButtonBinary.AutoSize = true;
            this.radioButtonBinary.Checked = true;
            this.radioButtonBinary.Location = new System.Drawing.Point(61, 14);
            this.radioButtonBinary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButtonBinary.Name = "radioButtonBinary";
            this.radioButtonBinary.Size = new System.Drawing.Size(58, 19);
            this.radioButtonBinary.TabIndex = 22;
            this.radioButtonBinary.TabStop = true;
            this.radioButtonBinary.Text = "Binary";
            this.radioButtonBinary.UseVisualStyleBackColor = true;
            this.radioButtonBinary.CheckedChanged += new System.EventHandler(this.RadioButton_Binary_CheckedChanged);
            // 
            // radioButtonSA2MDL
            // 
            this.radioButtonSA2MDL.AutoSize = true;
            this.radioButtonSA2MDL.Location = new System.Drawing.Point(131, 14);
            this.radioButtonSA2MDL.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButtonSA2MDL.Name = "radioButtonSA2MDL";
            this.radioButtonSA2MDL.Size = new System.Drawing.Size(94, 19);
            this.radioButtonSA2MDL.TabIndex = 23;
            this.radioButtonSA2MDL.Text = "SA2 MDL File";
            this.radioButtonSA2MDL.UseVisualStyleBackColor = true;
            // 
            // radioButtonSA2BMDL
            // 
            this.radioButtonSA2BMDL.AutoSize = true;
            this.radioButtonSA2BMDL.Location = new System.Drawing.Point(239, 14);
            this.radioButtonSA2BMDL.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButtonSA2BMDL.Name = "radioButtonSA2BMDL";
            this.radioButtonSA2BMDL.Size = new System.Drawing.Size(101, 19);
            this.radioButtonSA2BMDL.TabIndex = 24;
            this.radioButtonSA2BMDL.Text = "SA2B MDL File";
            this.radioButtonSA2BMDL.UseVisualStyleBackColor = true;
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(14, 16);
            this.labelType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(34, 15);
            this.labelType.TabIndex = 25;
            this.labelType.Text = "Type:";
            // 
            // checkBoxMemoryObject
            // 
            this.checkBoxMemoryObject.AutoSize = true;
            this.checkBoxMemoryObject.Location = new System.Drawing.Point(267, 57);
            this.checkBoxMemoryObject.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxMemoryObject.Name = "checkBoxMemoryObject";
            this.checkBoxMemoryObject.Size = new System.Drawing.Size(71, 19);
            this.checkBoxMemoryObject.TabIndex = 26;
            this.checkBoxMemoryObject.Text = "Memory";
            this.checkBoxMemoryObject.UseVisualStyleBackColor = true;
            // 
            // labelStructure
            // 
            this.labelStructure.AutoSize = true;
            this.labelStructure.Location = new System.Drawing.Point(17, 162);
            this.labelStructure.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStructure.Name = "labelStructure";
            this.labelStructure.Size = new System.Drawing.Size(58, 15);
            this.labelStructure.TabIndex = 27;
            this.labelStructure.Text = "Structure:";
            // 
            // radioButtonObject
            // 
            this.radioButtonObject.AutoSize = true;
            this.radioButtonObject.Checked = true;
            this.radioButtonObject.Location = new System.Drawing.Point(84, 159);
            this.radioButtonObject.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButtonObject.Name = "radioButtonObject";
            this.radioButtonObject.Size = new System.Drawing.Size(60, 19);
            this.radioButtonObject.TabIndex = 28;
            this.radioButtonObject.TabStop = true;
            this.radioButtonObject.Text = "Object";
            this.radioButtonObject.UseVisualStyleBackColor = true;
            this.radioButtonObject.CheckedChanged += new System.EventHandler(this.RadioButton_Object_CheckedChanged);
            // 
            // radioButtonAction
            // 
            this.radioButtonAction.AutoSize = true;
            this.radioButtonAction.Location = new System.Drawing.Point(156, 159);
            this.radioButtonAction.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButtonAction.Name = "radioButtonAction";
            this.radioButtonAction.Size = new System.Drawing.Size(60, 19);
            this.radioButtonAction.TabIndex = 29;
            this.radioButtonAction.Text = "Action";
            this.radioButtonAction.UseVisualStyleBackColor = true;
            // 
            // checkBoxMemoryMotion
            // 
            this.checkBoxMemoryMotion.AutoSize = true;
            this.checkBoxMemoryMotion.Enabled = false;
            this.checkBoxMemoryMotion.Location = new System.Drawing.Point(267, 220);
            this.checkBoxMemoryMotion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxMemoryMotion.Name = "checkBoxMemoryMotion";
            this.checkBoxMemoryMotion.Size = new System.Drawing.Size(71, 19);
            this.checkBoxMemoryMotion.TabIndex = 30;
            this.checkBoxMemoryMotion.Text = "Memory";
            this.checkBoxMemoryMotion.UseVisualStyleBackColor = true;
            // 
            // checkBoxHexMotion
            // 
            this.checkBoxHexMotion.AutoSize = true;
            this.checkBoxHexMotion.Checked = true;
            this.checkBoxHexMotion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHexMotion.Enabled = false;
            this.checkBoxHexMotion.Location = new System.Drawing.Point(212, 220);
            this.checkBoxHexMotion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxHexMotion.Name = "checkBoxHexMotion";
            this.checkBoxHexMotion.Size = new System.Drawing.Size(47, 19);
            this.checkBoxHexMotion.TabIndex = 31;
            this.checkBoxHexMotion.Text = "Hex";
            this.checkBoxHexMotion.UseVisualStyleBackColor = true;
            this.checkBoxHexMotion.CheckedChanged += new System.EventHandler(this.CheckBox_Hex_Motion_CheckedChanged);
            // 
            // groupBoxBinaryData
            // 
            this.groupBoxBinaryData.Controls.Add(this.checkBoxHexStartOffset);
            this.groupBoxBinaryData.Controls.Add(this.numericUpDownStartOffset);
            this.groupBoxBinaryData.Controls.Add(this.labelStartOffset);
            this.groupBoxBinaryData.Controls.Add(this.checkBoxReverse);
            this.groupBoxBinaryData.Controls.Add(this.radioButtonAttach);
            this.groupBoxBinaryData.Controls.Add(this.checkBoxHexMotion);
            this.groupBoxBinaryData.Controls.Add(this.comboBoxModelFormat);
            this.groupBoxBinaryData.Controls.Add(this.checkBoxBigEndian);
            this.groupBoxBinaryData.Controls.Add(this.checkBoxLoadMotion);
            this.groupBoxBinaryData.Controls.Add(this.numericUpDownKey);
            this.groupBoxBinaryData.Controls.Add(this.checkBoxMemoryMotion);
            this.groupBoxBinaryData.Controls.Add(this.labelModelFormat);
            this.groupBoxBinaryData.Controls.Add(this.numericUpDownMotionAddress);
            this.groupBoxBinaryData.Controls.Add(this.labelMotionAddress);
            this.groupBoxBinaryData.Controls.Add(this.radioButtonAction);
            this.groupBoxBinaryData.Controls.Add(this.labelKey);
            this.groupBoxBinaryData.Controls.Add(this.checkBoxMemoryObject);
            this.groupBoxBinaryData.Controls.Add(this.radioButtonObject);
            this.groupBoxBinaryData.Controls.Add(this.comboBoxBinaryFileType);
            this.groupBoxBinaryData.Controls.Add(this.labelStructure);
            this.groupBoxBinaryData.Controls.Add(this.numericUpDownModelAddress);
            this.groupBoxBinaryData.Controls.Add(this.labelModelAddress);
            this.groupBoxBinaryData.Controls.Add(this.checkBoxHexObject);
            this.groupBoxBinaryData.Location = new System.Drawing.Point(14, 40);
            this.groupBoxBinaryData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxBinaryData.Name = "groupBoxBinaryData";
            this.groupBoxBinaryData.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxBinaryData.Size = new System.Drawing.Size(380, 258);
            this.groupBoxBinaryData.TabIndex = 32;
            this.groupBoxBinaryData.TabStop = false;
            this.groupBoxBinaryData.Text = "Binary data";
            // 
            // checkBoxHexStartOffset
            // 
            this.checkBoxHexStartOffset.AutoSize = true;
            this.checkBoxHexStartOffset.Checked = true;
            this.checkBoxHexStartOffset.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHexStartOffset.Location = new System.Drawing.Point(212, 90);
            this.checkBoxHexStartOffset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxHexStartOffset.Name = "checkBoxHexStartOffset";
            this.checkBoxHexStartOffset.Size = new System.Drawing.Size(47, 19);
            this.checkBoxHexStartOffset.TabIndex = 36;
            this.checkBoxHexStartOffset.Text = "Hex";
            this.checkBoxHexStartOffset.UseVisualStyleBackColor = true;
            this.checkBoxHexStartOffset.CheckedChanged += new System.EventHandler(this.checkBoxHexStartOffset_CheckedChanged);
            // 
            // numericUpDownStartOffset
            // 
            this.numericUpDownStartOffset.Hexadecimal = true;
            this.numericUpDownStartOffset.Location = new System.Drawing.Point(86, 88);
            this.numericUpDownStartOffset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDownStartOffset.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDownStartOffset.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericUpDownStartOffset.Name = "numericUpDownStartOffset";
            this.numericUpDownStartOffset.Size = new System.Drawing.Size(114, 23);
            this.numericUpDownStartOffset.TabIndex = 35;
            this.numericUpDownStartOffset.ValueChanged += new System.EventHandler(this.numericUpDownStartOffset_ValueChanged);
            // 
            // labelStartOffset
            // 
            this.labelStartOffset.AutoSize = true;
            this.labelStartOffset.Location = new System.Drawing.Point(9, 90);
            this.labelStartOffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStartOffset.Name = "labelStartOffset";
            this.labelStartOffset.Size = new System.Drawing.Size(69, 15);
            this.labelStartOffset.TabIndex = 34;
            this.labelStartOffset.Text = "Start Offset:";
            // 
            // checkBoxReverse
            // 
            this.checkBoxReverse.AutoSize = true;
            this.checkBoxReverse.Location = new System.Drawing.Point(302, 124);
            this.checkBoxReverse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxReverse.Name = "checkBoxReverse";
            this.checkBoxReverse.Size = new System.Drawing.Size(66, 19);
            this.checkBoxReverse.TabIndex = 33;
            this.checkBoxReverse.Text = "Reverse";
            this.checkBoxReverse.UseVisualStyleBackColor = true;
            // 
            // radioButtonAttach
            // 
            this.radioButtonAttach.AutoSize = true;
            this.radioButtonAttach.Location = new System.Drawing.Point(227, 159);
            this.radioButtonAttach.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioButtonAttach.Name = "radioButtonAttach";
            this.radioButtonAttach.Size = new System.Drawing.Size(105, 19);
            this.radioButtonAttach.TabIndex = 32;
            this.radioButtonAttach.Text = "Model (Attach)";
            this.radioButtonAttach.UseVisualStyleBackColor = true;
            // 
            // numericUpDownKey
            // 
            this.numericUpDownKey.Hexadecimal = true;
            this.numericUpDownKey.Location = new System.Drawing.Point(86, 22);
            this.numericUpDownKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDownKey.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDownKey.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericUpDownKey.Name = "numericUpDownKey";
            this.numericUpDownKey.Size = new System.Drawing.Size(114, 23);
            this.numericUpDownKey.TabIndex = 15;
            this.numericUpDownKey.ValueChanged += new System.EventHandler(this.numericUpDownKey_ValueChanged);
            // 
            // numericUpDownMotionAddress
            // 
            this.numericUpDownMotionAddress.Enabled = false;
            this.numericUpDownMotionAddress.Hexadecimal = true;
            this.numericUpDownMotionAddress.Location = new System.Drawing.Point(84, 219);
            this.numericUpDownMotionAddress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDownMotionAddress.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDownMotionAddress.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericUpDownMotionAddress.Name = "numericUpDownMotionAddress";
            this.numericUpDownMotionAddress.Size = new System.Drawing.Size(114, 23);
            this.numericUpDownMotionAddress.TabIndex = 19;
            this.numericUpDownMotionAddress.ValueChanged += new System.EventHandler(this.numericUpDownMotionAddress_ValueChanged);
            // 
            // numericUpDownModelAddress
            // 
            this.numericUpDownModelAddress.Hexadecimal = true;
            this.numericUpDownModelAddress.Location = new System.Drawing.Point(86, 55);
            this.numericUpDownModelAddress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDownModelAddress.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDownModelAddress.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericUpDownModelAddress.Name = "numericUpDownModelAddress";
            this.numericUpDownModelAddress.Size = new System.Drawing.Size(114, 23);
            this.numericUpDownModelAddress.TabIndex = 3;
            this.numericUpDownModelAddress.ValueChanged += new System.EventHandler(this.numericUpDownModelAddress_ValueChanged);
            // 
            // ModelFileDialog
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 347);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.radioButtonBinary);
            this.Controls.Add(this.groupBoxBinaryData);
            this.Controls.Add(this.radioButtonSA2MDL);
            this.Controls.Add(this.radioButtonSA2BMDL);
            this.Controls.Add(this.OK_Button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelFileDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Load from a Binary File";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ModelFileDialog_HelpButtonClicked);
            this.groupBoxBinaryData.ResumeLayout(false);
            this.groupBoxBinaryData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotionAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModelAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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