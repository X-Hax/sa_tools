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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxNJA = new System.Windows.Forms.CheckBox();
            this.buttonRemoveAllBatch = new System.Windows.Forms.Button();
            this.buttonRemoveSelBatch = new System.Windows.Forms.Button();
            this.buttonConvertBatch = new System.Windows.Forms.Button();
            this.checkBoxSameOutputFolderBatch = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonAddBatch = new System.Windows.Forms.Button();
            this.listBoxStructConverter = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.buttonClearAllSplit = new System.Windows.Forms.Button();
            this.buttonRemoveSplit = new System.Windows.Forms.Button();
            this.comboBoxGameSelect = new System.Windows.Forms.ComboBox();
            this.buttonSplit = new System.Windows.Forms.Button();
            this.checkBoxSameFolderSplit = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_AddFilesSplit = new System.Windows.Forms.Button();
            this.listBox_SplitFiles = new System.Windows.Forms.ListBox();
            this.numericUpDownKey = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.NumericUpDownAddress = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.fileSelector1 = new SonicRetro.SAModel.DataExtractor.FileSelector();
            this.groupBoxBinary.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSelector1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonExtract
            // 
            this.buttonExtract.Enabled = false;
            this.buttonExtract.Location = new System.Drawing.Point(354, 360);
            this.buttonExtract.Name = "buttonExtract";
            this.buttonExtract.Size = new System.Drawing.Size(67, 23);
            this.buttonExtract.TabIndex = 18;
            this.buttonExtract.Text = "&Extract";
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
            this.textBoxAuthor.Size = new System.Drawing.Size(346, 20);
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
            this.textBoxDescription.Size = new System.Drawing.Size(345, 85);
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
            this.checkBoxMemory.Size = new System.Drawing.Size(103, 17);
            this.checkBoxMemory.TabIndex = 35;
            this.checkBoxMemory.Text = "Memory address";
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
            "SA Tools file + C structs",
            "SA Tools file + C structs + NJA"});
            this.comboBoxExportAs.Location = new System.Drawing.Point(74, 361);
            this.comboBoxExportAs.Name = "comboBoxExportAs";
            this.comboBoxExportAs.Size = new System.Drawing.Size(264, 21);
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
            this.groupBoxBinary.Size = new System.Drawing.Size(409, 147);
            this.groupBoxBinary.TabIndex = 39;
            this.groupBoxBinary.TabStop = false;
            this.groupBoxBinary.Text = "Binary Data";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(437, 423);
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
            this.tabPage1.Size = new System.Drawing.Size(429, 397);
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
            this.tabPage2.Controls.Add(this.checkBoxNJA);
            this.tabPage2.Controls.Add(this.buttonRemoveAllBatch);
            this.tabPage2.Controls.Add(this.buttonRemoveSelBatch);
            this.tabPage2.Controls.Add(this.buttonConvertBatch);
            this.tabPage2.Controls.Add(this.checkBoxSameOutputFolderBatch);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.buttonAddBatch);
            this.tabPage2.Controls.Add(this.listBoxStructConverter);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(429, 397);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Struct Converter";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBoxNJA
            // 
            this.checkBoxNJA.AutoSize = true;
            this.checkBoxNJA.Location = new System.Drawing.Point(11, 353);
            this.checkBoxNJA.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxNJA.Name = "checkBoxNJA";
            this.checkBoxNJA.Size = new System.Drawing.Size(156, 17);
            this.checkBoxNJA.TabIndex = 46;
            this.checkBoxNJA.Text = "Also convert models to NJA";
            this.checkBoxNJA.UseVisualStyleBackColor = true;
            // 
            // buttonRemoveAllBatch
            // 
            this.buttonRemoveAllBatch.Location = new System.Drawing.Point(342, 95);
            this.buttonRemoveAllBatch.Name = "buttonRemoveAllBatch";
            this.buttonRemoveAllBatch.Size = new System.Drawing.Size(80, 23);
            this.buttonRemoveAllBatch.TabIndex = 45;
            this.buttonRemoveAllBatch.Text = "Clear all";
            this.buttonRemoveAllBatch.UseVisualStyleBackColor = true;
            this.buttonRemoveAllBatch.Click += new System.EventHandler(this.buttonRemoveAllBatch_Click);
            // 
            // buttonRemoveSelBatch
            // 
            this.buttonRemoveSelBatch.Enabled = false;
            this.buttonRemoveSelBatch.Location = new System.Drawing.Point(342, 66);
            this.buttonRemoveSelBatch.Name = "buttonRemoveSelBatch";
            this.buttonRemoveSelBatch.Size = new System.Drawing.Size(80, 23);
            this.buttonRemoveSelBatch.TabIndex = 44;
            this.buttonRemoveSelBatch.Text = "&Remove";
            this.buttonRemoveSelBatch.UseVisualStyleBackColor = true;
            this.buttonRemoveSelBatch.Click += new System.EventHandler(this.buttonRemoveSelBatch_Click);
            // 
            // buttonConvertBatch
            // 
            this.buttonConvertBatch.Enabled = false;
            this.buttonConvertBatch.Location = new System.Drawing.Point(342, 301);
            this.buttonConvertBatch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonConvertBatch.Name = "buttonConvertBatch";
            this.buttonConvertBatch.Size = new System.Drawing.Size(80, 25);
            this.buttonConvertBatch.TabIndex = 43;
            this.buttonConvertBatch.Text = "&Convert";
            this.buttonConvertBatch.UseVisualStyleBackColor = true;
            this.buttonConvertBatch.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // checkBoxSameOutputFolderBatch
            // 
            this.checkBoxSameOutputFolderBatch.AutoSize = true;
            this.checkBoxSameOutputFolderBatch.Location = new System.Drawing.Point(11, 332);
            this.checkBoxSameOutputFolderBatch.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSameOutputFolderBatch.Name = "checkBoxSameOutputFolderBatch";
            this.checkBoxSameOutputFolderBatch.Size = new System.Drawing.Size(191, 17);
            this.checkBoxSameOutputFolderBatch.TabIndex = 42;
            this.checkBoxSameOutputFolderBatch.Text = "Same output folder as source file(s)";
            this.checkBoxSameOutputFolderBatch.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(353, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "Batch export level, model and animation files to C structs and Ninja ASCII.";
            // 
            // buttonAddBatch
            // 
            this.buttonAddBatch.Location = new System.Drawing.Point(342, 36);
            this.buttonAddBatch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAddBatch.Name = "buttonAddBatch";
            this.buttonAddBatch.Size = new System.Drawing.Size(80, 25);
            this.buttonAddBatch.TabIndex = 1;
            this.buttonAddBatch.Text = "&Add...";
            this.buttonAddBatch.UseVisualStyleBackColor = true;
            this.buttonAddBatch.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // listBoxStructConverter
            // 
            this.listBoxStructConverter.AllowDrop = true;
            this.listBoxStructConverter.FormattingEnabled = true;
            this.listBoxStructConverter.HorizontalScrollbar = true;
            this.listBoxStructConverter.Location = new System.Drawing.Point(8, 36);
            this.listBoxStructConverter.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxStructConverter.Name = "listBoxStructConverter";
            this.listBoxStructConverter.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxStructConverter.Size = new System.Drawing.Size(330, 290);
            this.listBoxStructConverter.TabIndex = 0;
            this.listBoxStructConverter.SelectedIndexChanged += new System.EventHandler(this.listBoxStructConverter_SelectedIndexChanged);
            this.listBoxStructConverter.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxStructConverter_DragDrop);
            this.listBoxStructConverter.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxStructConverter_DragEnter);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.buttonClearAllSplit);
            this.tabPage3.Controls.Add(this.buttonRemoveSplit);
            this.tabPage3.Controls.Add(this.comboBoxGameSelect);
            this.tabPage3.Controls.Add(this.buttonSplit);
            this.tabPage3.Controls.Add(this.checkBoxSameFolderSplit);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.button_AddFilesSplit);
            this.tabPage3.Controls.Add(this.listBox_SplitFiles);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(429, 397);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Split";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // buttonClearAllSplit
            // 
            this.buttonClearAllSplit.Location = new System.Drawing.Point(342, 120);
            this.buttonClearAllSplit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClearAllSplit.Name = "buttonClearAllSplit";
            this.buttonClearAllSplit.Size = new System.Drawing.Size(80, 25);
            this.buttonClearAllSplit.TabIndex = 51;
            this.buttonClearAllSplit.Text = "Clear &all";
            this.buttonClearAllSplit.UseVisualStyleBackColor = true;
            this.buttonClearAllSplit.Click += new System.EventHandler(this.buttonClearAllSplit_Click);
            // 
            // buttonRemoveSplit
            // 
            this.buttonRemoveSplit.Enabled = false;
            this.buttonRemoveSplit.Location = new System.Drawing.Point(342, 91);
            this.buttonRemoveSplit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRemoveSplit.Name = "buttonRemoveSplit";
            this.buttonRemoveSplit.Size = new System.Drawing.Size(80, 25);
            this.buttonRemoveSplit.TabIndex = 50;
            this.buttonRemoveSplit.Text = "&Remove";
            this.buttonRemoveSplit.UseVisualStyleBackColor = true;
            this.buttonRemoveSplit.Click += new System.EventHandler(this.buttonRemoveSplit_Click);
            // 
            // comboBoxGameSelect
            // 
            this.comboBoxGameSelect.FormattingEnabled = true;
            this.comboBoxGameSelect.Items.AddRange(new object[] {
            "SA1 (Dreamcast)",
            "SA1 Autodemo",
            "SADX (PC)",
            "SADX (X360 prototype)",
            "SA2 (Dreamcast)",
            "SA2: The Trial (Dreamcast)",
            "SA2 (PC)"});
            this.comboBoxGameSelect.Location = new System.Drawing.Point(7, 32);
            this.comboBoxGameSelect.Name = "comboBoxGameSelect";
            this.comboBoxGameSelect.Size = new System.Drawing.Size(183, 21);
            this.comboBoxGameSelect.TabIndex = 49;
            this.comboBoxGameSelect.SelectedIndexChanged += new System.EventHandler(this.comboBoxGameSelect_SelectedIndexChanged);
            // 
            // buttonSplit
            // 
            this.buttonSplit.Enabled = false;
            this.buttonSplit.Location = new System.Drawing.Point(342, 301);
            this.buttonSplit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSplit.Name = "buttonSplit";
            this.buttonSplit.Size = new System.Drawing.Size(80, 25);
            this.buttonSplit.TabIndex = 48;
            this.buttonSplit.Text = "&Split";
            this.buttonSplit.UseVisualStyleBackColor = true;
            this.buttonSplit.Click += new System.EventHandler(this.buttonSplit_Click);
            // 
            // checkBoxSameFolderSplit
            // 
            this.checkBoxSameFolderSplit.AutoSize = true;
            this.checkBoxSameFolderSplit.Location = new System.Drawing.Point(11, 332);
            this.checkBoxSameFolderSplit.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSameFolderSplit.Name = "checkBoxSameFolderSplit";
            this.checkBoxSameFolderSplit.Size = new System.Drawing.Size(191, 17);
            this.checkBoxSameFolderSplit.TabIndex = 47;
            this.checkBoxSameFolderSplit.Text = "Same output folder as source file(s)";
            this.checkBoxSameFolderSplit.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 12);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(280, 13);
            this.label3.TabIndex = 46;
            this.label3.Text = "Split binary files using data mapping for a supported game.";
            // 
            // button_AddFilesSplit
            // 
            this.button_AddFilesSplit.Location = new System.Drawing.Point(342, 62);
            this.button_AddFilesSplit.Margin = new System.Windows.Forms.Padding(2);
            this.button_AddFilesSplit.Name = "button_AddFilesSplit";
            this.button_AddFilesSplit.Size = new System.Drawing.Size(80, 25);
            this.button_AddFilesSplit.TabIndex = 45;
            this.button_AddFilesSplit.Text = "&Add...";
            this.button_AddFilesSplit.UseVisualStyleBackColor = true;
            this.button_AddFilesSplit.Click += new System.EventHandler(this.button_AddFilesSplit_Click);
            // 
            // listBox_SplitFiles
            // 
            this.listBox_SplitFiles.AllowDrop = true;
            this.listBox_SplitFiles.FormattingEnabled = true;
            this.listBox_SplitFiles.HorizontalScrollbar = true;
            this.listBox_SplitFiles.Location = new System.Drawing.Point(8, 62);
            this.listBox_SplitFiles.Margin = new System.Windows.Forms.Padding(2);
            this.listBox_SplitFiles.Name = "listBox_SplitFiles";
            this.listBox_SplitFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_SplitFiles.Size = new System.Drawing.Size(330, 264);
            this.listBox_SplitFiles.TabIndex = 44;
            this.listBox_SplitFiles.SelectedIndexChanged += new System.EventHandler(this.listBox_SplitFiles_SelectedIndexChanged);
            this.listBox_SplitFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_SplitFiles_DragDrop);
            this.listBox_SplitFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox_SplitFiles_DragEnter);
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
            // fileSelector1
            // 
            this.fileSelector1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.fileSelector1.DefaultExt = "";
            this.fileSelector1.FileName = "";
            this.fileSelector1.Filter = "Binary Files|*.exe;*.dll;*.bin;*.prs|All Files|*.*";
            this.fileSelector1.Location = new System.Drawing.Point(42, 38);
            this.fileSelector1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fileSelector1.Name = "fileSelector1";
            this.fileSelector1.Size = new System.Drawing.Size(378, 24);
            this.fileSelector1.TabIndex = 27;
            this.fileSelector1.FileNameChanged += new System.EventHandler(this.fileSelector1_FileNameChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 423);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Extractor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxBinary.ResumeLayout(false);
            this.groupBoxBinary.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownAddress)).EndInit();
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
		private System.Windows.Forms.Button buttonConvertBatch;
		private System.Windows.Forms.CheckBox checkBoxSameOutputFolderBatch;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonAddBatch;
		private System.Windows.Forms.ListBox listBoxStructConverter;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.ComboBox comboBoxGameSelect;
		private System.Windows.Forms.Button buttonSplit;
		private System.Windows.Forms.CheckBox checkBoxSameFolderSplit;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button_AddFilesSplit;
		private System.Windows.Forms.ListBox listBox_SplitFiles;
		private System.Windows.Forms.Button buttonRemoveAllBatch;
		private System.Windows.Forms.Button buttonRemoveSelBatch;
		private System.Windows.Forms.Button buttonClearAllSplit;
		private System.Windows.Forms.Button buttonRemoveSplit;
		private System.Windows.Forms.CheckBox checkBoxNJA;
	}
}

