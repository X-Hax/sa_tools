namespace SonicRetro.SAModel.DataExtractor
{
    partial class MainForm
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
            this.buttonExtract = new System.Windows.Forms.Button();
            this.comboBoxFormat = new System.Windows.Forms.ComboBox();
            this.labelFormat = new System.Windows.Forms.Label();
            this.CheckBoxHex = new System.Windows.Forms.CheckBox();
            this.labelAddress = new System.Windows.Forms.Label();
            this.ComboBoxBinaryType = new System.Windows.Forms.ComboBox();
            this.labelKey = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxAuthor = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.checkBoxBigEndian = new System.Windows.Forms.CheckBox();
            this.comboBoxItemType = new System.Windows.Forms.ComboBox();
            this.labelType = new System.Windows.Forms.Label();
            this.checkBoxMemory = new System.Windows.Forms.CheckBox();
            this.labelFile = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxExportAs = new System.Windows.Forms.ComboBox();
            this.groupBoxBinary = new System.Windows.Forms.GroupBox();
            this.numericUpDownKey = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.NumericUpDownAddress = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.checkBoxSameOutputFolder = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.fileSelector1 = new SonicRetro.SAModel.DataExtractor.FileSelector();
            this.groupBoxBinary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownAddress)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSelector1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonExtract
            // 
            this.buttonExtract.Location = new System.Drawing.Point(262, 359);
            this.buttonExtract.Name = "buttonExtract";
            this.buttonExtract.Size = new System.Drawing.Size(67, 23);
            this.buttonExtract.TabIndex = 18;
            this.buttonExtract.Text = "Extract";
            this.buttonExtract.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxFormat
            // 
            this.comboBoxFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFormat.FormattingEnabled = true;
            this.comboBoxFormat.Items.AddRange(new object[] {
            "SA1",
            "SADX",
            "SA2",
            "SA2B"});
            this.comboBoxFormat.Location = new System.Drawing.Point(64, 104);
            this.comboBoxFormat.Name = "comboBoxFormat";
            this.comboBoxFormat.Size = new System.Drawing.Size(121, 21);
            this.comboBoxFormat.TabIndex = 26;
            // 
            // labelFormat
            // 
            this.labelFormat.AutoSize = true;
            this.labelFormat.Location = new System.Drawing.Point(16, 104);
            this.labelFormat.Name = "labelFormat";
            this.labelFormat.Size = new System.Drawing.Size(42, 13);
            this.labelFormat.TabIndex = 25;
            this.labelFormat.Text = "Format:";
            // 
            // CheckBoxHex
            // 
            this.CheckBoxHex.AutoSize = true;
            this.CheckBoxHex.Checked = true;
            this.CheckBoxHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxHex.Location = new System.Drawing.Point(177, 48);
            this.CheckBoxHex.Name = "CheckBoxHex";
            this.CheckBoxHex.Size = new System.Drawing.Size(45, 17);
            this.CheckBoxHex.TabIndex = 23;
            this.CheckBoxHex.Text = "Hex";
            this.CheckBoxHex.UseVisualStyleBackColor = true;
            this.CheckBoxHex.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(10, 48);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(48, 13);
            this.labelAddress.TabIndex = 22;
            this.labelAddress.Text = "Address:";
            // 
            // ComboBoxBinaryType
            // 
            this.ComboBoxBinaryType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxBinaryType.FormattingEnabled = true;
            this.ComboBoxBinaryType.Items.AddRange(new object[] {
            "EXE",
            "DLL",
            "1st_read.bin",
            "SA1 Level",
            "SA2 Level",
            "SA1 Event File",
            "SA2 Event File",
            "SA2PC Event File",
            "Model File"});
            this.ComboBoxBinaryType.Location = new System.Drawing.Point(177, 20);
            this.ComboBoxBinaryType.Name = "ComboBoxBinaryType";
            this.ComboBoxBinaryType.Size = new System.Drawing.Size(123, 21);
            this.ComboBoxBinaryType.TabIndex = 20;
            this.ComboBoxBinaryType.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // labelKey
            // 
            this.labelKey.AutoSize = true;
            this.labelKey.Location = new System.Drawing.Point(30, 22);
            this.labelKey.Name = "labelKey";
            this.labelKey.Size = new System.Drawing.Size(28, 13);
            this.labelKey.TabIndex = 19;
            this.labelKey.Text = "Key:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 236);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Author:";
            // 
            // textBoxAuthor
            // 
            this.textBoxAuthor.Location = new System.Drawing.Point(74, 233);
            this.textBoxAuthor.Name = "textBoxAuthor";
            this.textBoxAuthor.Size = new System.Drawing.Size(255, 20);
            this.textBoxAuthor.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 266);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Description:";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.AcceptsReturn = true;
            this.textBoxDescription.AcceptsTab = true;
            this.textBoxDescription.Location = new System.Drawing.Point(75, 263);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(254, 85);
            this.textBoxDescription.TabIndex = 31;
            // 
            // checkBoxBigEndian
            // 
            this.checkBoxBigEndian.AutoSize = true;
            this.checkBoxBigEndian.Location = new System.Drawing.Point(192, 107);
            this.checkBoxBigEndian.Name = "checkBoxBigEndian";
            this.checkBoxBigEndian.Size = new System.Drawing.Size(77, 17);
            this.checkBoxBigEndian.TabIndex = 32;
            this.checkBoxBigEndian.Text = "Big Endian";
            this.checkBoxBigEndian.UseVisualStyleBackColor = true;
            // 
            // comboBoxItemType
            // 
            this.comboBoxItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxItemType.FormattingEnabled = true;
            this.comboBoxItemType.Items.AddRange(new object[] {
            "Level",
            "Model"});
            this.comboBoxItemType.Location = new System.Drawing.Point(64, 73);
            this.comboBoxItemType.Name = "comboBoxItemType";
            this.comboBoxItemType.Size = new System.Drawing.Size(121, 21);
            this.comboBoxItemType.TabIndex = 33;
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(24, 76);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(34, 13);
            this.labelType.TabIndex = 34;
            this.labelType.Text = "Type:";
            // 
            // checkBoxMemory
            // 
            this.checkBoxMemory.AutoSize = true;
            this.checkBoxMemory.Location = new System.Drawing.Point(228, 48);
            this.checkBoxMemory.Name = "checkBoxMemory";
            this.checkBoxMemory.Size = new System.Drawing.Size(63, 17);
            this.checkBoxMemory.TabIndex = 35;
            this.checkBoxMemory.Text = "Memory";
            this.checkBoxMemory.UseVisualStyleBackColor = true;
            // 
            // labelFile
            // 
            this.labelFile.AutoSize = true;
            this.labelFile.Location = new System.Drawing.Point(12, 43);
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(26, 13);
            this.labelFile.TabIndex = 36;
            this.labelFile.Text = "File:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 364);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Export As:";
            // 
            // comboBoxExportAs
            // 
            this.comboBoxExportAs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExportAs.FormattingEnabled = true;
            this.comboBoxExportAs.Items.AddRange(new object[] {
            "SA Tools file",
            "SA Tools file + C structs"});
            this.comboBoxExportAs.Location = new System.Drawing.Point(74, 361);
            this.comboBoxExportAs.Name = "comboBoxExportAs";
            this.comboBoxExportAs.Size = new System.Drawing.Size(158, 21);
            this.comboBoxExportAs.TabIndex = 38;
            // 
            // groupBoxBinary
            // 
            this.groupBoxBinary.Controls.Add(this.comboBoxFormat);
            this.groupBoxBinary.Controls.Add(this.labelKey);
            this.groupBoxBinary.Controls.Add(this.numericUpDownKey);
            this.groupBoxBinary.Controls.Add(this.ComboBoxBinaryType);
            this.groupBoxBinary.Controls.Add(this.checkBoxMemory);
            this.groupBoxBinary.Controls.Add(this.NumericUpDownAddress);
            this.groupBoxBinary.Controls.Add(this.labelType);
            this.groupBoxBinary.Controls.Add(this.labelAddress);
            this.groupBoxBinary.Controls.Add(this.checkBoxBigEndian);
            this.groupBoxBinary.Controls.Add(this.comboBoxItemType);
            this.groupBoxBinary.Controls.Add(this.CheckBoxHex);
            this.groupBoxBinary.Controls.Add(this.labelFormat);
            this.groupBoxBinary.Location = new System.Drawing.Point(11, 73);
            this.groupBoxBinary.Name = "groupBoxBinary";
            this.groupBoxBinary.Size = new System.Drawing.Size(318, 147);
            this.groupBoxBinary.TabIndex = 39;
            this.groupBoxBinary.TabStop = false;
            this.groupBoxBinary.Text = "Binary Data";
            // 
            // numericUpDownKey
            // 
            this.numericUpDownKey.Hexadecimal = true;
            this.numericUpDownKey.Location = new System.Drawing.Point(64, 20);
            this.numericUpDownKey.Name = "numericUpDownKey";
            this.numericUpDownKey.Size = new System.Drawing.Size(98, 20);
            this.numericUpDownKey.TabIndex = 24;
            // 
            // NumericUpDownAddress
            // 
            this.NumericUpDownAddress.Hexadecimal = true;
            this.NumericUpDownAddress.Location = new System.Drawing.Point(64, 46);
            this.NumericUpDownAddress.Name = "NumericUpDownAddress";
            this.NumericUpDownAddress.Size = new System.Drawing.Size(98, 20);
            this.NumericUpDownAddress.TabIndex = 21;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(349, 414);
            this.tabControl1.TabIndex = 40;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.labelFile);
            this.tabPage1.Controls.Add(this.groupBoxBinary);
            this.tabPage1.Controls.Add(this.fileSelector1);
            this.tabPage1.Controls.Add(this.comboBoxExportAs);
            this.tabPage1.Controls.Add(this.textBoxDescription);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.textBoxAuthor);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.buttonExtract);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(341, 388);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Data Extractor";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(333, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "Extract levels or models from binary files and export them as C structs.";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonConvert);
            this.tabPage2.Controls.Add(this.checkBoxSameOutputFolder);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.buttonAdd);
            this.tabPage2.Controls.Add(this.listBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(341, 388);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Batch Converter";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonConvert
            // 
            this.buttonConvert.Location = new System.Drawing.Point(269, 360);
            this.buttonConvert.Margin = new System.Windows.Forms.Padding(2);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(69, 25);
            this.buttonConvert.TabIndex = 43;
            this.buttonConvert.Text = "Convert";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // checkBoxSameOutputFolder
            // 
            this.checkBoxSameOutputFolder.AutoSize = true;
            this.checkBoxSameOutputFolder.Location = new System.Drawing.Point(11, 332);
            this.checkBoxSameOutputFolder.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSameOutputFolder.Name = "checkBoxSameOutputFolder";
            this.checkBoxSameOutputFolder.Size = new System.Drawing.Size(191, 17);
            this.checkBoxSameOutputFolder.TabIndex = 42;
            this.checkBoxSameOutputFolder.Text = "Same output folder as source file(s)";
            this.checkBoxSameOutputFolder.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(275, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "Batch export level, model and animation files to C structs.";
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(269, 331);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(69, 25);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add files...";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(8, 36);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(329, 290);
            this.listBox1.TabIndex = 0;
            // 
            // fileSelector1
            // 
            this.fileSelector1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.fileSelector1.DefaultExt = "";
            this.fileSelector1.FileName = "";
            this.fileSelector1.Filter = "Binary Files|*.exe;*.dll;*.bin;*.prs|All Files|*.*";
            this.fileSelector1.Location = new System.Drawing.Point(45, 38);
            this.fileSelector1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fileSelector1.Name = "fileSelector1";
            this.fileSelector1.Size = new System.Drawing.Size(284, 24);
            this.fileSelector1.TabIndex = 27;
            this.fileSelector1.FileNameChanged += new System.EventHandler(this.fileSelector1_FileNameChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 414);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Data Extractor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxBinary.ResumeLayout(false);
            this.groupBoxBinary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownAddress)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSelector1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ComboBox comboBoxFormat;
        internal System.Windows.Forms.Label labelFormat;
        internal System.Windows.Forms.CheckBox CheckBoxHex;
        internal System.Windows.Forms.Label labelAddress;
        internal System.Windows.Forms.ComboBox ComboBoxBinaryType;
        internal System.Windows.Forms.Label labelKey;
        private FileSelector fileSelector1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxAuthor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button buttonExtract;
        private System.Windows.Forms.CheckBox checkBoxBigEndian;
		internal System.Windows.Forms.Label labelType;
		internal System.Windows.Forms.ComboBox comboBoxItemType;
		internal System.Windows.Forms.CheckBox checkBoxMemory;
		internal System.Windows.Forms.Label labelFile;
		private System.Windows.Forms.Label label8;
		internal System.Windows.Forms.ComboBox comboBoxExportAs;
		private System.Windows.Forms.GroupBox groupBoxBinary;
        internal SAEditorCommon.UI.HexNumericUpdown numericUpDownKey;
        internal SAEditorCommon.UI.HexNumericUpdown NumericUpDownAddress;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Button buttonConvert;
		private System.Windows.Forms.CheckBox checkBoxSameOutputFolder;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.ListBox listBox1;
	}
}

