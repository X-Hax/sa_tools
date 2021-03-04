namespace SonicRetro.SAModel.DataToolbox
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
            this.buttonBinaryExtract = new System.Windows.Forms.Button();
            this.comboBoxBinaryFormat = new System.Windows.Forms.ComboBox();
            this.labelFormat = new System.Windows.Forms.Label();
            this.checkBoxBinaryHex = new System.Windows.Forms.CheckBox();
            this.labelAddress = new System.Windows.Forms.Label();
            this.ComboBoxBinaryType = new System.Windows.Forms.ComboBox();
            this.labelKey = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxBinaryAuthor = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxBinaryDescription = new System.Windows.Forms.TextBox();
            this.checkBoxBinaryBigEndian = new System.Windows.Forms.CheckBox();
            this.comboBoxBinaryItemType = new System.Windows.Forms.ComboBox();
            this.labelType = new System.Windows.Forms.Label();
            this.checkBoxBinaryMemory = new System.Windows.Forms.CheckBox();
            this.labelFile = new System.Windows.Forms.Label();
            this.groupBoxBinary = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageBinaryData = new System.Windows.Forms.TabPage();
            this.buttonBinaryBrowse = new System.Windows.Forms.Button();
            this.textBoxBinaryFilename = new System.Windows.Forms.TextBox();
            this.checkBoxBinaryJSON = new System.Windows.Forms.CheckBox();
            this.checkBoxBinaryNJA = new System.Windows.Forms.CheckBox();
            this.checkBoxBinaryStructs = new System.Windows.Forms.CheckBox();
            this.checkBoxBinarySAModel = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageStructConverter = new System.Windows.Forms.TabPage();
            this.checkBoxStructConvStructs = new System.Windows.Forms.CheckBox();
            this.checkBoxStructConvJSON = new System.Windows.Forms.CheckBox();
            this.checkBoxStructConvNJA = new System.Windows.Forms.CheckBox();
            this.buttonStructConvRemoveAllBatch = new System.Windows.Forms.Button();
            this.buttonStructConvRemoveSelBatch = new System.Windows.Forms.Button();
            this.buttonStructConvConvertBatch = new System.Windows.Forms.Button();
            this.checkBoxStructConvSameOutputFolderBatch = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonStructConvAddBatch = new System.Windows.Forms.Button();
            this.listBoxStructConverter = new System.Windows.Forms.ListBox();
            this.tabPageSplit = new System.Windows.Forms.TabPage();
            this.checkBoxFindAllSplit = new System.Windows.Forms.CheckBox();
            this.buttonClearAllSplit = new System.Windows.Forms.Button();
            this.buttonRemoveSplit = new System.Windows.Forms.Button();
            this.comboBoxSplitGameSelect = new System.Windows.Forms.ComboBox();
            this.buttonSplitStart = new System.Windows.Forms.Button();
            this.checkBoxSameFolderSplit = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonAddFilesSplit = new System.Windows.Forms.Button();
            this.listBoxSplitFiles = new System.Windows.Forms.ListBox();
            this.tabPageSplitMDL = new System.Windows.Forms.TabPage();
            this.buttonMDLBrowse = new System.Windows.Forms.Button();
            this.textBoxMDLFilename = new System.Windows.Forms.TextBox();
            this.checkBoxMDLSameFolder = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonMDLAnimFilesRemove = new System.Windows.Forms.Button();
            this.buttonAnimFilesClear = new System.Windows.Forms.Button();
            this.checkBoxMDLBigEndian = new System.Windows.Forms.CheckBox();
            this.buttonSplitMDL = new System.Windows.Forms.Button();
            this.buttonAnimFilesAdd = new System.Windows.Forms.Button();
            this.animationFilesLabel = new System.Windows.Forms.Label();
            this.listBoxMDLAnimationFiles = new System.Windows.Forms.ListBox();
            this.hexNumericBinaryOffset = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.numericUpDownBinaryKey = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.numericUpDownBinaryAddress = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.groupBoxBinary.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageBinaryData.SuspendLayout();
            this.tabPageStructConverter.SuspendLayout();
            this.tabPageSplit.SuspendLayout();
            this.tabPageSplitMDL.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hexNumericBinaryOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonBinaryExtract
            // 
            this.buttonBinaryExtract.Enabled = false;
            this.buttonBinaryExtract.Location = new System.Drawing.Point(353, 357);
            this.buttonBinaryExtract.Name = "buttonBinaryExtract";
            this.buttonBinaryExtract.Size = new System.Drawing.Size(67, 23);
            this.buttonBinaryExtract.TabIndex = 18;
            this.buttonBinaryExtract.Text = "&Start";
            this.buttonBinaryExtract.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxBinaryFormat
            // 
            this.comboBoxBinaryFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBinaryFormat.FormattingEnabled = true;
            this.comboBoxBinaryFormat.Items.AddRange(new object[] {
            "Basic (SA1/SADX GC)",
            "BasicDX (SADX PC)",
            "Chunk (SA2)",
            "GC (SA2B/SA2PC)"});
            this.comboBoxBinaryFormat.Location = new System.Drawing.Point(64, 134);
            this.comboBoxBinaryFormat.Name = "comboBoxBinaryFormat";
            this.comboBoxBinaryFormat.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBinaryFormat.TabIndex = 26;
            // 
            // labelFormat
            // 
            this.labelFormat.AutoSize = true;
            this.labelFormat.Location = new System.Drawing.Point(16, 134);
            this.labelFormat.Name = "labelFormat";
            this.labelFormat.Size = new System.Drawing.Size(42, 13);
            this.labelFormat.TabIndex = 25;
            this.labelFormat.Text = "Format:";
            // 
            // checkBoxBinaryHex
            // 
            this.checkBoxBinaryHex.AutoSize = true;
            this.checkBoxBinaryHex.Checked = true;
            this.checkBoxBinaryHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBinaryHex.Location = new System.Drawing.Point(177, 50);
            this.checkBoxBinaryHex.Name = "checkBoxBinaryHex";
            this.checkBoxBinaryHex.Size = new System.Drawing.Size(45, 17);
            this.checkBoxBinaryHex.TabIndex = 23;
            this.checkBoxBinaryHex.Text = "Hex";
            this.checkBoxBinaryHex.UseVisualStyleBackColor = true;
            this.checkBoxBinaryHex.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
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
            this.label4.Location = new System.Drawing.Point(27, 251);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Author:";
            // 
            // textBoxBinaryAuthor
            // 
            this.textBoxBinaryAuthor.Location = new System.Drawing.Point(74, 248);
            this.textBoxBinaryAuthor.Name = "textBoxBinaryAuthor";
            this.textBoxBinaryAuthor.Size = new System.Drawing.Size(346, 20);
            this.textBoxBinaryAuthor.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 278);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Description:";
            // 
            // textBoxBinaryDescription
            // 
            this.textBoxBinaryDescription.AcceptsReturn = true;
            this.textBoxBinaryDescription.AcceptsTab = true;
            this.textBoxBinaryDescription.Location = new System.Drawing.Point(75, 278);
            this.textBoxBinaryDescription.Multiline = true;
            this.textBoxBinaryDescription.Name = "textBoxBinaryDescription";
            this.textBoxBinaryDescription.Size = new System.Drawing.Size(345, 56);
            this.textBoxBinaryDescription.TabIndex = 31;
            // 
            // checkBoxBinaryBigEndian
            // 
            this.checkBoxBinaryBigEndian.AutoSize = true;
            this.checkBoxBinaryBigEndian.Location = new System.Drawing.Point(177, 73);
            this.checkBoxBinaryBigEndian.Name = "checkBoxBinaryBigEndian";
            this.checkBoxBinaryBigEndian.Size = new System.Drawing.Size(77, 17);
            this.checkBoxBinaryBigEndian.TabIndex = 32;
            this.checkBoxBinaryBigEndian.Text = "Big Endian";
            this.checkBoxBinaryBigEndian.UseVisualStyleBackColor = true;
            // 
            // comboBoxBinaryItemType
            // 
            this.comboBoxBinaryItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBinaryItemType.FormattingEnabled = true;
            this.comboBoxBinaryItemType.Items.AddRange(new object[] {
            "Level",
            "Model",
            "Action"});
            this.comboBoxBinaryItemType.Location = new System.Drawing.Point(64, 103);
            this.comboBoxBinaryItemType.Name = "comboBoxBinaryItemType";
            this.comboBoxBinaryItemType.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBinaryItemType.TabIndex = 33;
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(2, 106);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(56, 13);
            this.labelType.TabIndex = 34;
            this.labelType.Text = "Data type:";
            // 
            // checkBoxBinaryMemory
            // 
            this.checkBoxBinaryMemory.AutoSize = true;
            this.checkBoxBinaryMemory.Location = new System.Drawing.Point(228, 50);
            this.checkBoxBinaryMemory.Name = "checkBoxBinaryMemory";
            this.checkBoxBinaryMemory.Size = new System.Drawing.Size(103, 17);
            this.checkBoxBinaryMemory.TabIndex = 35;
            this.checkBoxBinaryMemory.Text = "Memory address";
            this.checkBoxBinaryMemory.UseVisualStyleBackColor = true;
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
            // groupBoxBinary
            // 
            this.groupBoxBinary.Controls.Add(this.label6);
            this.groupBoxBinary.Controls.Add(this.hexNumericBinaryOffset);
            this.groupBoxBinary.Controls.Add(this.comboBoxBinaryFormat);
            this.groupBoxBinary.Controls.Add(this.labelKey);
            this.groupBoxBinary.Controls.Add(this.numericUpDownBinaryKey);
            this.groupBoxBinary.Controls.Add(this.ComboBoxBinaryType);
            this.groupBoxBinary.Controls.Add(this.checkBoxBinaryMemory);
            this.groupBoxBinary.Controls.Add(this.numericUpDownBinaryAddress);
            this.groupBoxBinary.Controls.Add(this.labelType);
            this.groupBoxBinary.Controls.Add(this.labelAddress);
            this.groupBoxBinary.Controls.Add(this.checkBoxBinaryBigEndian);
            this.groupBoxBinary.Controls.Add(this.comboBoxBinaryItemType);
            this.groupBoxBinary.Controls.Add(this.checkBoxBinaryHex);
            this.groupBoxBinary.Controls.Add(this.labelFormat);
            this.groupBoxBinary.Location = new System.Drawing.Point(11, 73);
            this.groupBoxBinary.Name = "groupBoxBinary";
            this.groupBoxBinary.Size = new System.Drawing.Size(409, 165);
            this.groupBoxBinary.TabIndex = 39;
            this.groupBoxBinary.TabStop = false;
            this.groupBoxBinary.Text = "Binary Data";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 37;
            this.label6.Text = "Offset:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageBinaryData);
            this.tabControl1.Controls.Add(this.tabPageStructConverter);
            this.tabControl1.Controls.Add(this.tabPageSplit);
            this.tabControl1.Controls.Add(this.tabPageSplitMDL);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(437, 415);
            this.tabControl1.TabIndex = 40;
            // 
            // tabPageBinaryData
            // 
            this.tabPageBinaryData.Controls.Add(this.buttonBinaryBrowse);
            this.tabPageBinaryData.Controls.Add(this.textBoxBinaryFilename);
            this.tabPageBinaryData.Controls.Add(this.checkBoxBinaryJSON);
            this.tabPageBinaryData.Controls.Add(this.checkBoxBinaryNJA);
            this.tabPageBinaryData.Controls.Add(this.checkBoxBinaryStructs);
            this.tabPageBinaryData.Controls.Add(this.checkBoxBinarySAModel);
            this.tabPageBinaryData.Controls.Add(this.label1);
            this.tabPageBinaryData.Controls.Add(this.labelFile);
            this.tabPageBinaryData.Controls.Add(this.groupBoxBinary);
            this.tabPageBinaryData.Controls.Add(this.textBoxBinaryDescription);
            this.tabPageBinaryData.Controls.Add(this.label4);
            this.tabPageBinaryData.Controls.Add(this.textBoxBinaryAuthor);
            this.tabPageBinaryData.Controls.Add(this.label5);
            this.tabPageBinaryData.Controls.Add(this.buttonBinaryExtract);
            this.tabPageBinaryData.Location = new System.Drawing.Point(4, 22);
            this.tabPageBinaryData.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageBinaryData.Name = "tabPageBinaryData";
            this.tabPageBinaryData.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageBinaryData.Size = new System.Drawing.Size(429, 389);
            this.tabPageBinaryData.TabIndex = 0;
            this.tabPageBinaryData.Text = "Binary Data Extractor";
            this.tabPageBinaryData.UseVisualStyleBackColor = true;
            // 
            // buttonBinaryBrowse
            // 
            this.buttonBinaryBrowse.Location = new System.Drawing.Point(348, 38);
            this.buttonBinaryBrowse.Name = "buttonBinaryBrowse";
            this.buttonBinaryBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBinaryBrowse.TabIndex = 46;
            this.buttonBinaryBrowse.Text = "Browse...";
            this.buttonBinaryBrowse.UseVisualStyleBackColor = true;
            this.buttonBinaryBrowse.Click += new System.EventHandler(this.buttonBinaryBrowse_Click);
            // 
            // textBoxBinaryFilename
            // 
            this.textBoxBinaryFilename.AllowDrop = true;
            this.textBoxBinaryFilename.Location = new System.Drawing.Point(44, 40);
            this.textBoxBinaryFilename.Name = "textBoxBinaryFilename";
            this.textBoxBinaryFilename.Size = new System.Drawing.Size(298, 20);
            this.textBoxBinaryFilename.TabIndex = 45;
            this.textBoxBinaryFilename.TextChanged += new System.EventHandler(this.textBoxBinaryFilename_TextChanged);
            this.textBoxBinaryFilename.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxBinaryFilename_DragDrop);
            this.textBoxBinaryFilename.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxBinaryFilename_DragEnter);
            // 
            // checkBoxBinaryJSON
            // 
            this.checkBoxBinaryJSON.AutoSize = true;
            this.checkBoxBinaryJSON.Location = new System.Drawing.Point(158, 363);
            this.checkBoxBinaryJSON.Name = "checkBoxBinaryJSON";
            this.checkBoxBinaryJSON.Size = new System.Drawing.Size(159, 17);
            this.checkBoxBinaryJSON.TabIndex = 44;
            this.checkBoxBinaryJSON.Text = "Convert animations to JSON";
            this.checkBoxBinaryJSON.UseVisualStyleBackColor = true;
            this.checkBoxBinaryJSON.CheckedChanged += new System.EventHandler(this.checkBox_JSON_CheckedChanged);
            // 
            // checkBoxBinaryNJA
            // 
            this.checkBoxBinaryNJA.AutoSize = true;
            this.checkBoxBinaryNJA.Location = new System.Drawing.Point(8, 363);
            this.checkBoxBinaryNJA.Name = "checkBoxBinaryNJA";
            this.checkBoxBinaryNJA.Size = new System.Drawing.Size(134, 17);
            this.checkBoxBinaryNJA.TabIndex = 43;
            this.checkBoxBinaryNJA.Text = "Convert models to NJA";
            this.checkBoxBinaryNJA.UseVisualStyleBackColor = true;
            this.checkBoxBinaryNJA.CheckedChanged += new System.EventHandler(this.checkBox_NJA_CheckedChanged);
            // 
            // checkBoxBinaryStructs
            // 
            this.checkBoxBinaryStructs.AutoSize = true;
            this.checkBoxBinaryStructs.Location = new System.Drawing.Point(158, 340);
            this.checkBoxBinaryStructs.Name = "checkBoxBinaryStructs";
            this.checkBoxBinaryStructs.Size = new System.Drawing.Size(100, 17);
            this.checkBoxBinaryStructs.TabIndex = 42;
            this.checkBoxBinaryStructs.Text = "Export C structs";
            this.checkBoxBinaryStructs.UseVisualStyleBackColor = true;
            this.checkBoxBinaryStructs.CheckedChanged += new System.EventHandler(this.checkBox_Structs_CheckedChanged);
            // 
            // checkBoxBinarySAModel
            // 
            this.checkBoxBinarySAModel.AutoSize = true;
            this.checkBoxBinarySAModel.Checked = true;
            this.checkBoxBinarySAModel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBinarySAModel.Location = new System.Drawing.Point(8, 340);
            this.checkBoxBinarySAModel.Name = "checkBoxBinarySAModel";
            this.checkBoxBinarySAModel.Size = new System.Drawing.Size(123, 17);
            this.checkBoxBinarySAModel.TabIndex = 41;
            this.checkBoxBinarySAModel.Text = "Export SAModel files";
            this.checkBoxBinarySAModel.UseVisualStyleBackColor = true;
            this.checkBoxBinarySAModel.CheckedChanged += new System.EventHandler(this.checkBox_SAModel_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(364, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "Extract data from binary files and export it as C structs, Ninja ASCII or JSON.";
            // 
            // tabPageStructConverter
            // 
            this.tabPageStructConverter.Controls.Add(this.checkBoxStructConvStructs);
            this.tabPageStructConverter.Controls.Add(this.checkBoxStructConvJSON);
            this.tabPageStructConverter.Controls.Add(this.checkBoxStructConvNJA);
            this.tabPageStructConverter.Controls.Add(this.buttonStructConvRemoveAllBatch);
            this.tabPageStructConverter.Controls.Add(this.buttonStructConvRemoveSelBatch);
            this.tabPageStructConverter.Controls.Add(this.buttonStructConvConvertBatch);
            this.tabPageStructConverter.Controls.Add(this.checkBoxStructConvSameOutputFolderBatch);
            this.tabPageStructConverter.Controls.Add(this.label2);
            this.tabPageStructConverter.Controls.Add(this.buttonStructConvAddBatch);
            this.tabPageStructConverter.Controls.Add(this.listBoxStructConverter);
            this.tabPageStructConverter.Location = new System.Drawing.Point(4, 22);
            this.tabPageStructConverter.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageStructConverter.Name = "tabPageStructConverter";
            this.tabPageStructConverter.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageStructConverter.Size = new System.Drawing.Size(429, 389);
            this.tabPageStructConverter.TabIndex = 1;
            this.tabPageStructConverter.Text = "Struct Converter";
            this.tabPageStructConverter.UseVisualStyleBackColor = true;
            // 
            // checkBoxStructConvStructs
            // 
            this.checkBoxStructConvStructs.AutoSize = true;
            this.checkBoxStructConvStructs.Checked = true;
            this.checkBoxStructConvStructs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStructConvStructs.Location = new System.Drawing.Point(158, 340);
            this.checkBoxStructConvStructs.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStructConvStructs.Name = "checkBoxStructConvStructs";
            this.checkBoxStructConvStructs.Size = new System.Drawing.Size(100, 17);
            this.checkBoxStructConvStructs.TabIndex = 48;
            this.checkBoxStructConvStructs.Text = "Export C structs";
            this.checkBoxStructConvStructs.UseVisualStyleBackColor = true;
            this.checkBoxStructConvStructs.CheckedChanged += new System.EventHandler(this.checkBox_StructConvStructs_CheckedChanged);
            // 
            // checkBoxStructConvJSON
            // 
            this.checkBoxStructConvJSON.AutoSize = true;
            this.checkBoxStructConvJSON.Location = new System.Drawing.Point(158, 363);
            this.checkBoxStructConvJSON.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStructConvJSON.Name = "checkBoxStructConvJSON";
            this.checkBoxStructConvJSON.Size = new System.Drawing.Size(159, 17);
            this.checkBoxStructConvJSON.TabIndex = 47;
            this.checkBoxStructConvJSON.Text = "Convert animations to JSON";
            this.checkBoxStructConvJSON.UseVisualStyleBackColor = true;
            this.checkBoxStructConvJSON.CheckedChanged += new System.EventHandler(this.checkBox_StructConvJSON_CheckedChanged);
            // 
            // checkBoxStructConvNJA
            // 
            this.checkBoxStructConvNJA.AutoSize = true;
            this.checkBoxStructConvNJA.Location = new System.Drawing.Point(8, 363);
            this.checkBoxStructConvNJA.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStructConvNJA.Name = "checkBoxStructConvNJA";
            this.checkBoxStructConvNJA.Size = new System.Drawing.Size(134, 17);
            this.checkBoxStructConvNJA.TabIndex = 46;
            this.checkBoxStructConvNJA.Text = "Convert models to NJA";
            this.checkBoxStructConvNJA.UseVisualStyleBackColor = true;
            this.checkBoxStructConvNJA.CheckedChanged += new System.EventHandler(this.checkBox_StructConvNJA_CheckedChanged);
            // 
            // buttonStructConvRemoveAllBatch
            // 
            this.buttonStructConvRemoveAllBatch.Location = new System.Drawing.Point(342, 95);
            this.buttonStructConvRemoveAllBatch.Name = "buttonStructConvRemoveAllBatch";
            this.buttonStructConvRemoveAllBatch.Size = new System.Drawing.Size(80, 23);
            this.buttonStructConvRemoveAllBatch.TabIndex = 45;
            this.buttonStructConvRemoveAllBatch.Text = "Clear all";
            this.buttonStructConvRemoveAllBatch.UseVisualStyleBackColor = true;
            this.buttonStructConvRemoveAllBatch.Click += new System.EventHandler(this.buttonRemoveAllBatch_Click);
            // 
            // buttonStructConvRemoveSelBatch
            // 
            this.buttonStructConvRemoveSelBatch.Enabled = false;
            this.buttonStructConvRemoveSelBatch.Location = new System.Drawing.Point(342, 66);
            this.buttonStructConvRemoveSelBatch.Name = "buttonStructConvRemoveSelBatch";
            this.buttonStructConvRemoveSelBatch.Size = new System.Drawing.Size(80, 23);
            this.buttonStructConvRemoveSelBatch.TabIndex = 44;
            this.buttonStructConvRemoveSelBatch.Text = "&Remove";
            this.buttonStructConvRemoveSelBatch.UseVisualStyleBackColor = true;
            this.buttonStructConvRemoveSelBatch.Click += new System.EventHandler(this.buttonRemoveSelBatch_Click);
            // 
            // buttonStructConvConvertBatch
            // 
            this.buttonStructConvConvertBatch.Enabled = false;
            this.buttonStructConvConvertBatch.Location = new System.Drawing.Point(353, 357);
            this.buttonStructConvConvertBatch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStructConvConvertBatch.Name = "buttonStructConvConvertBatch";
            this.buttonStructConvConvertBatch.Size = new System.Drawing.Size(67, 23);
            this.buttonStructConvConvertBatch.TabIndex = 43;
            this.buttonStructConvConvertBatch.Text = "&Start";
            this.buttonStructConvConvertBatch.UseVisualStyleBackColor = true;
            this.buttonStructConvConvertBatch.Click += new System.EventHandler(this.buttonConvertBatch_Click);
            // 
            // checkBoxStructConvSameOutputFolderBatch
            // 
            this.checkBoxStructConvSameOutputFolderBatch.AutoSize = true;
            this.checkBoxStructConvSameOutputFolderBatch.Location = new System.Drawing.Point(8, 340);
            this.checkBoxStructConvSameOutputFolderBatch.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStructConvSameOutputFolderBatch.Name = "checkBoxStructConvSameOutputFolderBatch";
            this.checkBoxStructConvSameOutputFolderBatch.Size = new System.Drawing.Size(126, 17);
            this.checkBoxStructConvSameOutputFolderBatch.TabIndex = 42;
            this.checkBoxStructConvSameOutputFolderBatch.Text = "Same output folder(s)";
            this.checkBoxStructConvSameOutputFolderBatch.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(378, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "Batch export level, model and animation files to C structs, Ninja ASCII or JSON.";
            // 
            // buttonStructConvAddBatch
            // 
            this.buttonStructConvAddBatch.Location = new System.Drawing.Point(342, 36);
            this.buttonStructConvAddBatch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStructConvAddBatch.Name = "buttonStructConvAddBatch";
            this.buttonStructConvAddBatch.Size = new System.Drawing.Size(80, 25);
            this.buttonStructConvAddBatch.TabIndex = 1;
            this.buttonStructConvAddBatch.Text = "&Add...";
            this.buttonStructConvAddBatch.UseVisualStyleBackColor = true;
            this.buttonStructConvAddBatch.Click += new System.EventHandler(this.button1_Click_1);
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
            // tabPageSplit
            // 
            this.tabPageSplit.Controls.Add(this.checkBoxFindAllSplit);
            this.tabPageSplit.Controls.Add(this.buttonClearAllSplit);
            this.tabPageSplit.Controls.Add(this.buttonRemoveSplit);
            this.tabPageSplit.Controls.Add(this.comboBoxSplitGameSelect);
            this.tabPageSplit.Controls.Add(this.buttonSplitStart);
            this.tabPageSplit.Controls.Add(this.checkBoxSameFolderSplit);
            this.tabPageSplit.Controls.Add(this.label3);
            this.tabPageSplit.Controls.Add(this.buttonAddFilesSplit);
            this.tabPageSplit.Controls.Add(this.listBoxSplitFiles);
            this.tabPageSplit.Location = new System.Drawing.Point(4, 22);
            this.tabPageSplit.Name = "tabPageSplit";
            this.tabPageSplit.Size = new System.Drawing.Size(429, 389);
            this.tabPageSplit.TabIndex = 2;
            this.tabPageSplit.Text = "Split";
            this.tabPageSplit.UseVisualStyleBackColor = true;
            // 
            // checkBoxFindAllSplit
            // 
            this.checkBoxFindAllSplit.AutoSize = true;
            this.checkBoxFindAllSplit.Checked = true;
            this.checkBoxFindAllSplit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFindAllSplit.Location = new System.Drawing.Point(8, 340);
            this.checkBoxFindAllSplit.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxFindAllSplit.Name = "checkBoxFindAllSplit";
            this.checkBoxFindAllSplit.Size = new System.Drawing.Size(289, 17);
            this.checkBoxFindAllSplit.TabIndex = 52;
            this.checkBoxFindAllSplit.Text = "Search for split INI files (uncheck to use a single INI file)";
            this.checkBoxFindAllSplit.UseVisualStyleBackColor = true;
            // 
            // buttonClearAllSplit
            // 
            this.buttonClearAllSplit.Location = new System.Drawing.Point(344, 120);
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
            this.buttonRemoveSplit.Location = new System.Drawing.Point(344, 91);
            this.buttonRemoveSplit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRemoveSplit.Name = "buttonRemoveSplit";
            this.buttonRemoveSplit.Size = new System.Drawing.Size(80, 25);
            this.buttonRemoveSplit.TabIndex = 50;
            this.buttonRemoveSplit.Text = "&Remove";
            this.buttonRemoveSplit.UseVisualStyleBackColor = true;
            this.buttonRemoveSplit.Click += new System.EventHandler(this.buttonRemoveSplit_Click);
            // 
            // comboBoxSplitGameSelect
            // 
            this.comboBoxSplitGameSelect.FormattingEnabled = true;
            this.comboBoxSplitGameSelect.Items.AddRange(new object[] {
            "SA1 (Dreamcast)",
            "SA1 Autodemo",
            "SADX (PC)",
			"SADX (Gamecube)",
			"SADX (X360 prototype)",
            "SA2 (Dreamcast)",
            "SA2: The Trial (Dreamcast)",
            "SA2 (PC)"});
            this.comboBoxSplitGameSelect.Location = new System.Drawing.Point(7, 32);
            this.comboBoxSplitGameSelect.Name = "comboBoxSplitGameSelect";
            this.comboBoxSplitGameSelect.Size = new System.Drawing.Size(183, 21);
            this.comboBoxSplitGameSelect.TabIndex = 49;
            this.comboBoxSplitGameSelect.SelectedIndexChanged += new System.EventHandler(this.comboBoxGameSelect_SelectedIndexChanged);
            // 
            // buttonSplitStart
            // 
            this.buttonSplitStart.Enabled = false;
            this.buttonSplitStart.Location = new System.Drawing.Point(353, 357);
            this.buttonSplitStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSplitStart.Name = "buttonSplitStart";
            this.buttonSplitStart.Size = new System.Drawing.Size(67, 23);
            this.buttonSplitStart.TabIndex = 48;
            this.buttonSplitStart.Text = "&Start";
            this.buttonSplitStart.UseVisualStyleBackColor = true;
            this.buttonSplitStart.Click += new System.EventHandler(this.buttonSplit_Click);
            // 
            // checkBoxSameFolderSplit
            // 
            this.checkBoxSameFolderSplit.AutoSize = true;
            this.checkBoxSameFolderSplit.Location = new System.Drawing.Point(8, 363);
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
            // buttonAddFilesSplit
            // 
            this.buttonAddFilesSplit.Location = new System.Drawing.Point(344, 62);
            this.buttonAddFilesSplit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAddFilesSplit.Name = "buttonAddFilesSplit";
            this.buttonAddFilesSplit.Size = new System.Drawing.Size(80, 25);
            this.buttonAddFilesSplit.TabIndex = 45;
            this.buttonAddFilesSplit.Text = "&Add...";
            this.buttonAddFilesSplit.UseVisualStyleBackColor = true;
            this.buttonAddFilesSplit.Click += new System.EventHandler(this.button_AddFilesSplit_Click);
            // 
            // listBoxSplitFiles
            // 
            this.listBoxSplitFiles.AllowDrop = true;
            this.listBoxSplitFiles.FormattingEnabled = true;
            this.listBoxSplitFiles.HorizontalScrollbar = true;
            this.listBoxSplitFiles.Location = new System.Drawing.Point(8, 62);
            this.listBoxSplitFiles.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxSplitFiles.Name = "listBoxSplitFiles";
            this.listBoxSplitFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxSplitFiles.Size = new System.Drawing.Size(330, 264);
            this.listBoxSplitFiles.TabIndex = 44;
            this.listBoxSplitFiles.SelectedIndexChanged += new System.EventHandler(this.listBox_SplitFiles_SelectedIndexChanged);
            this.listBoxSplitFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_SplitFiles_DragDrop);
            this.listBoxSplitFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox_SplitFiles_DragEnter);
            // 
            // tabPageSplitMDL
            // 
            this.tabPageSplitMDL.Controls.Add(this.buttonMDLBrowse);
            this.tabPageSplitMDL.Controls.Add(this.textBoxMDLFilename);
            this.tabPageSplitMDL.Controls.Add(this.checkBoxMDLSameFolder);
            this.tabPageSplitMDL.Controls.Add(this.label8);
            this.tabPageSplitMDL.Controls.Add(this.label7);
            this.tabPageSplitMDL.Controls.Add(this.buttonMDLAnimFilesRemove);
            this.tabPageSplitMDL.Controls.Add(this.buttonAnimFilesClear);
            this.tabPageSplitMDL.Controls.Add(this.checkBoxMDLBigEndian);
            this.tabPageSplitMDL.Controls.Add(this.buttonSplitMDL);
            this.tabPageSplitMDL.Controls.Add(this.buttonAnimFilesAdd);
            this.tabPageSplitMDL.Controls.Add(this.animationFilesLabel);
            this.tabPageSplitMDL.Controls.Add(this.listBoxMDLAnimationFiles);
            this.tabPageSplitMDL.Location = new System.Drawing.Point(4, 22);
            this.tabPageSplitMDL.Name = "tabPageSplitMDL";
            this.tabPageSplitMDL.Size = new System.Drawing.Size(429, 389);
            this.tabPageSplitMDL.TabIndex = 3;
            this.tabPageSplitMDL.Text = "SplitMDL";
            this.tabPageSplitMDL.UseVisualStyleBackColor = true;
            // 
            // buttonMDLBrowse
            // 
            this.buttonMDLBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMDLBrowse.Location = new System.Drawing.Point(344, 37);
            this.buttonMDLBrowse.Name = "buttonMDLBrowse";
            this.buttonMDLBrowse.Size = new System.Drawing.Size(80, 25);
            this.buttonMDLBrowse.TabIndex = 52;
            this.buttonMDLBrowse.Text = "Browse...";
            this.buttonMDLBrowse.UseVisualStyleBackColor = true;
            this.buttonMDLBrowse.Click += new System.EventHandler(this.buttonMDLBrowse_Click);
            // 
            // textBoxMDLFilename
            // 
            this.textBoxMDLFilename.AllowDrop = true;
            this.textBoxMDLFilename.Location = new System.Drawing.Point(42, 40);
            this.textBoxMDLFilename.Name = "textBoxMDLFilename";
            this.textBoxMDLFilename.Size = new System.Drawing.Size(293, 20);
            this.textBoxMDLFilename.TabIndex = 51;
            this.textBoxMDLFilename.TextChanged += new System.EventHandler(this.textBoxMDLFilename_TextChanged);
            this.textBoxMDLFilename.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxMDLFilename_DragDrop);
            this.textBoxMDLFilename.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxMDLFilename_DragEnter);
            // 
            // checkBoxMDLSameFolder
            // 
            this.checkBoxMDLSameFolder.AutoSize = true;
            this.checkBoxMDLSameFolder.Location = new System.Drawing.Point(8, 363);
            this.checkBoxMDLSameFolder.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxMDLSameFolder.Name = "checkBoxMDLSameFolder";
            this.checkBoxMDLSameFolder.Size = new System.Drawing.Size(189, 17);
            this.checkBoxMDLSameFolder.TabIndex = 50;
            this.checkBoxMDLSameFolder.Text = "Same output folder as the MDL file";
            this.checkBoxMDLSameFolder.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 43);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 48;
            this.label8.Text = "File:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 12);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(132, 13);
            this.label7.TabIndex = 47;
            this.label7.Text = "Split SA2/SA2B MDL files.";
            // 
            // buttonMDLAnimFilesRemove
            // 
            this.buttonMDLAnimFilesRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMDLAnimFilesRemove.Enabled = false;
            this.buttonMDLAnimFilesRemove.Location = new System.Drawing.Point(344, 116);
            this.buttonMDLAnimFilesRemove.Name = "buttonMDLAnimFilesRemove";
            this.buttonMDLAnimFilesRemove.Size = new System.Drawing.Size(80, 25);
            this.buttonMDLAnimFilesRemove.TabIndex = 23;
            this.buttonMDLAnimFilesRemove.Text = "Remove";
            this.buttonMDLAnimFilesRemove.UseVisualStyleBackColor = true;
            this.buttonMDLAnimFilesRemove.Click += new System.EventHandler(this.buttonMDLAnimFilesRemove_Click);
            // 
            // buttonAnimFilesClear
            // 
            this.buttonAnimFilesClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAnimFilesClear.Location = new System.Drawing.Point(344, 147);
            this.buttonAnimFilesClear.Name = "buttonAnimFilesClear";
            this.buttonAnimFilesClear.Size = new System.Drawing.Size(80, 25);
            this.buttonAnimFilesClear.TabIndex = 23;
            this.buttonAnimFilesClear.Text = "Clear all";
            this.buttonAnimFilesClear.UseVisualStyleBackColor = true;
            this.buttonAnimFilesClear.Click += new System.EventHandler(this.buttonAnimFilesClear_Click);
            // 
            // checkBoxMDLBigEndian
            // 
            this.checkBoxMDLBigEndian.AutoSize = true;
            this.checkBoxMDLBigEndian.Location = new System.Drawing.Point(307, 65);
            this.checkBoxMDLBigEndian.Name = "checkBoxMDLBigEndian";
            this.checkBoxMDLBigEndian.Size = new System.Drawing.Size(120, 17);
            this.checkBoxMDLBigEndian.TabIndex = 22;
			this.checkBoxMDLBigEndian.Checked = true;
			this.checkBoxMDLBigEndian.Text = "Big Endian (GC/PC)";
            this.checkBoxMDLBigEndian.UseVisualStyleBackColor = true;
            // 
            // buttonSplitMDL
            // 
            this.buttonSplitMDL.Enabled = false;
            this.buttonSplitMDL.Location = new System.Drawing.Point(353, 357);
            this.buttonSplitMDL.Name = "buttonSplitMDL";
            this.buttonSplitMDL.Size = new System.Drawing.Size(67, 23);
            this.buttonSplitMDL.TabIndex = 18;
            this.buttonSplitMDL.Text = "&Start";
            this.buttonSplitMDL.UseVisualStyleBackColor = true;
            this.buttonSplitMDL.Click += new System.EventHandler(this.buttonSplitMDL_Click);
            // 
            // buttonAnimFilesAdd
            // 
            this.buttonAnimFilesAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAnimFilesAdd.Location = new System.Drawing.Point(344, 85);
            this.buttonAnimFilesAdd.Name = "buttonAnimFilesAdd";
            this.buttonAnimFilesAdd.Size = new System.Drawing.Size(80, 25);
            this.buttonAnimFilesAdd.TabIndex = 17;
            this.buttonAnimFilesAdd.Text = "Add...";
            this.buttonAnimFilesAdd.UseVisualStyleBackColor = true;
            this.buttonAnimFilesAdd.Click += new System.EventHandler(this.buttonAnimFilesAdd_Click);
            // 
            // animationFilesLabel
            // 
            this.animationFilesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.animationFilesLabel.AutoSize = true;
            this.animationFilesLabel.Location = new System.Drawing.Point(8, 69);
            this.animationFilesLabel.Name = "animationFilesLabel";
            this.animationFilesLabel.Size = new System.Drawing.Size(77, 13);
            this.animationFilesLabel.TabIndex = 16;
            this.animationFilesLabel.Text = "Animation Files";
            // 
            // listBoxMDLAnimationFiles
            // 
            this.listBoxMDLAnimationFiles.AllowDrop = true;
            this.listBoxMDLAnimationFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxMDLAnimationFiles.FormattingEnabled = true;
            this.listBoxMDLAnimationFiles.Location = new System.Drawing.Point(8, 85);
            this.listBoxMDLAnimationFiles.Name = "listBoxMDLAnimationFiles";
            this.listBoxMDLAnimationFiles.Size = new System.Drawing.Size(330, 264);
            this.listBoxMDLAnimationFiles.TabIndex = 15;
            this.listBoxMDLAnimationFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxMDLAnimationFiles_SelectedIndexChanged);
            this.listBoxMDLAnimationFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxMDLAnimationFiles_DragDrop);
            this.listBoxMDLAnimationFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxMDLAnimationFiles_DragEnter);
            // 
            // hexNumericBinaryOffset
            // 
            this.hexNumericBinaryOffset.Hexadecimal = true;
            this.hexNumericBinaryOffset.Location = new System.Drawing.Point(65, 72);
            this.hexNumericBinaryOffset.Name = "hexNumericBinaryOffset";
            this.hexNumericBinaryOffset.Size = new System.Drawing.Size(98, 20);
            this.hexNumericBinaryOffset.TabIndex = 36;
            // 
            // numericUpDownBinaryKey
            // 
            this.numericUpDownBinaryKey.Hexadecimal = true;
            this.numericUpDownBinaryKey.Location = new System.Drawing.Point(64, 20);
            this.numericUpDownBinaryKey.Name = "numericUpDownBinaryKey";
            this.numericUpDownBinaryKey.Size = new System.Drawing.Size(98, 20);
            this.numericUpDownBinaryKey.TabIndex = 24;
            // 
            // numericUpDownBinaryAddress
            // 
            this.numericUpDownBinaryAddress.Hexadecimal = true;
            this.numericUpDownBinaryAddress.Location = new System.Drawing.Point(64, 46);
            this.numericUpDownBinaryAddress.Name = "numericUpDownBinaryAddress";
            this.numericUpDownBinaryAddress.Size = new System.Drawing.Size(98, 20);
            this.numericUpDownBinaryAddress.TabIndex = 21;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 415);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Toolbox";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxBinary.ResumeLayout(false);
            this.groupBoxBinary.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageBinaryData.ResumeLayout(false);
            this.tabPageBinaryData.PerformLayout();
            this.tabPageStructConverter.ResumeLayout(false);
            this.tabPageStructConverter.PerformLayout();
            this.tabPageSplit.ResumeLayout(false);
            this.tabPageSplit.PerformLayout();
            this.tabPageSplitMDL.ResumeLayout(false);
            this.tabPageSplitMDL.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hexNumericBinaryOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryAddress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ComboBox comboBoxBinaryFormat;
        internal System.Windows.Forms.Label labelFormat;
        internal System.Windows.Forms.CheckBox checkBoxBinaryHex;
        internal System.Windows.Forms.Label labelAddress;
        internal System.Windows.Forms.ComboBox ComboBoxBinaryType;
        internal System.Windows.Forms.Label labelKey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxBinaryAuthor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxBinaryDescription;
        private System.Windows.Forms.Button buttonBinaryExtract;
        private System.Windows.Forms.CheckBox checkBoxBinaryBigEndian;
		internal System.Windows.Forms.Label labelType;
		internal System.Windows.Forms.ComboBox comboBoxBinaryItemType;
		internal System.Windows.Forms.CheckBox checkBoxBinaryMemory;
		internal System.Windows.Forms.Label labelFile;
		private System.Windows.Forms.GroupBox groupBoxBinary;
        internal SAEditorCommon.UI.HexNumericUpdown numericUpDownBinaryKey;
        internal SAEditorCommon.UI.HexNumericUpdown numericUpDownBinaryAddress;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageBinaryData;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPageStructConverter;
		private System.Windows.Forms.Button buttonStructConvConvertBatch;
		private System.Windows.Forms.CheckBox checkBoxStructConvSameOutputFolderBatch;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonStructConvAddBatch;
		private System.Windows.Forms.ListBox listBoxStructConverter;
		private System.Windows.Forms.TabPage tabPageSplit;
		private System.Windows.Forms.ComboBox comboBoxSplitGameSelect;
		private System.Windows.Forms.Button buttonSplitStart;
		private System.Windows.Forms.CheckBox checkBoxSameFolderSplit;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonAddFilesSplit;
		private System.Windows.Forms.ListBox listBoxSplitFiles;
		private System.Windows.Forms.Button buttonStructConvRemoveAllBatch;
		private System.Windows.Forms.Button buttonStructConvRemoveSelBatch;
		private System.Windows.Forms.Button buttonClearAllSplit;
		private System.Windows.Forms.Button buttonRemoveSplit;
		private System.Windows.Forms.CheckBox checkBoxStructConvNJA;
		private System.Windows.Forms.CheckBox checkBoxBinaryJSON;
		private System.Windows.Forms.CheckBox checkBoxBinaryNJA;
		private System.Windows.Forms.CheckBox checkBoxBinaryStructs;
		private System.Windows.Forms.CheckBox checkBoxBinarySAModel;
		internal System.Windows.Forms.Label label6;
		internal SAEditorCommon.UI.HexNumericUpdown hexNumericBinaryOffset;
		private System.Windows.Forms.CheckBox checkBoxStructConvJSON;
		private System.Windows.Forms.CheckBox checkBoxStructConvStructs;
		private System.Windows.Forms.CheckBox checkBoxFindAllSplit;
		private System.Windows.Forms.TabPage tabPageSplitMDL;
		internal System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button buttonAnimFilesClear;
		private System.Windows.Forms.CheckBox checkBoxMDLBigEndian;
		private System.Windows.Forms.Button buttonSplitMDL;
		private System.Windows.Forms.Button buttonAnimFilesAdd;
		private System.Windows.Forms.Label animationFilesLabel;
		private System.Windows.Forms.ListBox listBoxMDLAnimationFiles;
		private System.Windows.Forms.CheckBox checkBoxMDLSameFolder;
		private System.Windows.Forms.Button buttonMDLBrowse;
		private System.Windows.Forms.TextBox textBoxMDLFilename;
		private System.Windows.Forms.Button buttonMDLAnimFilesRemove;
		private System.Windows.Forms.Button buttonBinaryBrowse;
		private System.Windows.Forms.TextBox textBoxBinaryFilename;
	}
}

