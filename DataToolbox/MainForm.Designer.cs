namespace SAModel.DataToolbox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
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
            this.numericUpDownBinaryOffset = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBinaryKey = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBinaryAddress = new System.Windows.Forms.NumericUpDown();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageBinaryData = new System.Windows.Forms.TabPage();
            this.buttonBinaryBrowse = new System.Windows.Forms.Button();
            this.textBoxBinaryFilename = new System.Windows.Forms.TextBox();
            this.checkBoxBinaryJSON = new System.Windows.Forms.CheckBox();
            this.checkBoxBinaryNJA = new System.Windows.Forms.CheckBox();
            this.checkBoxBinaryStructs = new System.Windows.Forms.CheckBox();
            this.checkBoxBinarySAModel = new System.Windows.Forms.CheckBox();
            this.tabPageStructConverter = new System.Windows.Forms.TabPage();
            this.checkBoxStructConvStructs = new System.Windows.Forms.CheckBox();
            this.checkBoxStructConvJSON = new System.Windows.Forms.CheckBox();
            this.checkBoxStructConvNJA = new System.Windows.Forms.CheckBox();
            this.buttonStructConvRemoveAllBatch = new System.Windows.Forms.Button();
            this.buttonStructConvRemoveSelBatch = new System.Windows.Forms.Button();
            this.buttonStructConvConvertBatch = new System.Windows.Forms.Button();
            this.checkBoxStructConvSameOutputFolderBatch = new System.Windows.Forms.CheckBox();
            this.buttonStructConvAddBatch = new System.Windows.Forms.Button();
            this.listBoxStructConverter = new System.Windows.Forms.ListBox();
            this.tabPageSplit = new System.Windows.Forms.TabPage();
            this.comboBoxLabels = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonClearAllSplit = new System.Windows.Forms.Button();
            this.buttonRemoveSplit = new System.Windows.Forms.Button();
            this.comboBoxSplitGameSelect = new System.Windows.Forms.ComboBox();
            this.buttonSplitStart = new System.Windows.Forms.Button();
            this.checkBoxSameFolderSplit = new System.Windows.Forms.CheckBox();
            this.buttonAddFilesSplit = new System.Windows.Forms.Button();
            this.listBoxSplitFiles = new System.Windows.Forms.ListBox();
            this.tabPageSplitMDL = new System.Windows.Forms.TabPage();
            this.buttonMDLBrowse = new System.Windows.Forms.Button();
            this.textBoxMDLFilename = new System.Windows.Forms.TextBox();
            this.checkBoxMDLSameFolder = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonMDLAnimFilesRemove = new System.Windows.Forms.Button();
            this.buttonAnimFilesClear = new System.Windows.Forms.Button();
            this.checkBoxMDLBigEndian = new System.Windows.Forms.CheckBox();
            this.buttonSplitMDL = new System.Windows.Forms.Button();
            this.buttonAnimFilesAdd = new System.Windows.Forms.Button();
            this.animationFilesLabel = new System.Windows.Forms.Label();
            this.listBoxMDLAnimationFiles = new System.Windows.Forms.ListBox();
            this.tabPageScanner = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDownOffset = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.numericUpDownNodes = new System.Windows.Forms.NumericUpDown();
            this.labelNodes = new System.Windows.Forms.Label();
            this.buttonFindBrowse = new System.Windows.Forms.Button();
            this.checkBoxSearchAction = new System.Windows.Forms.CheckBox();
            this.textBoxFindModel = new System.Windows.Forms.TextBox();
            this.checkBoxSearchLandtables = new System.Windows.Forms.CheckBox();
            this.checkBoxSearchBasic = new System.Windows.Forms.CheckBox();
            this.radioButtonFindModel = new System.Windows.Forms.RadioButton();
            this.checkBoxSearchMotion = new System.Windows.Forms.CheckBox();
            this.radioButtonSearchData = new System.Windows.Forms.RadioButton();
            this.checkBoxSearchChunk = new System.Windows.Forms.CheckBox();
            this.checkBoxSearchGinja = new System.Windows.Forms.CheckBox();
            this.buttonInputBrowse = new System.Windows.Forms.Button();
            this.listBoxBaseGame = new System.Windows.Forms.ListBox();
            this.buttonScanStart = new System.Windows.Forms.Button();
            this.numericUpDownScanBinaryKey = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxKeepLevel = new System.Windows.Forms.CheckBox();
            this.buttonOutputBrowse = new System.Windows.Forms.Button();
            this.checkBoxKeepChild = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipMeta = new System.Windows.Forms.CheckBox();
            this.checkBoxSingleOutput = new System.Windows.Forms.CheckBox();
            this.textBoxOutputFolder = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownStartAddr = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxSimpleScan = new System.Windows.Forms.CheckBox();
            this.checkBoxShortRot = new System.Windows.Forms.CheckBox();
            this.checkBoxBasicSADX = new System.Windows.Forms.CheckBox();
            this.checkBoxBigEndian = new System.Windows.Forms.CheckBox();
            this.textBoxInputFile = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.numericUpDownEndAddr = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tabPageLabelTool = new System.Windows.Forms.TabPage();
            this.checkBoxLabelDeleteFiles = new System.Windows.Forms.CheckBox();
            this.buttonLabelStart = new System.Windows.Forms.Button();
            this.checkBoxLabelClearInternal = new System.Windows.Forms.CheckBox();
            this.radioButtonLabelImport = new System.Windows.Forms.RadioButton();
            this.radioButtonLabelExport = new System.Windows.Forms.RadioButton();
            this.buttonLabelClear = new System.Windows.Forms.Button();
            this.buttonLabelRemove = new System.Windows.Forms.Button();
            this.buttonLabelAdd = new System.Windows.Forms.Button();
            this.listBoxLabelTool = new System.Windows.Forms.ListBox();
            this.statusStripTabDescription = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelTabDesc = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBoxBinary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryAddress)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageBinaryData.SuspendLayout();
            this.tabPageStructConverter.SuspendLayout();
            this.tabPageSplit.SuspendLayout();
            this.tabPageSplitMDL.SuspendLayout();
            this.tabPageScanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOffset)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanBinaryKey)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAddr)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndAddr)).BeginInit();
            this.tabPageLabelTool.SuspendLayout();
            this.statusStripTabDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonBinaryExtract
            // 
            this.buttonBinaryExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBinaryExtract.Enabled = false;
            this.buttonBinaryExtract.Location = new System.Drawing.Point(420, 435);
            this.buttonBinaryExtract.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBinaryExtract.Name = "buttonBinaryExtract";
            this.buttonBinaryExtract.Size = new System.Drawing.Size(78, 26);
            this.buttonBinaryExtract.TabIndex = 9;
            this.buttonBinaryExtract.Text = "&Start";
            this.buttonBinaryExtract.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxBinaryFormat
            // 
            this.comboBoxBinaryFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxBinaryFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBinaryFormat.FormattingEnabled = true;
            this.comboBoxBinaryFormat.Items.AddRange(new object[] {
            "Basic (SA1/DX GC)",
            "Basic+ (DX PC/X360)",
            "Chunk (SA2)",
            "Ginja (SA2B/SA2PC)"});
            this.comboBoxBinaryFormat.Location = new System.Drawing.Point(74, 145);
            this.comboBoxBinaryFormat.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxBinaryFormat.Name = "comboBoxBinaryFormat";
            this.comboBoxBinaryFormat.Size = new System.Drawing.Size(140, 23);
            this.comboBoxBinaryFormat.TabIndex = 8;
            // 
            // labelFormat
            // 
            this.labelFormat.AutoSize = true;
            this.labelFormat.Location = new System.Drawing.Point(18, 148);
            this.labelFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFormat.Name = "labelFormat";
            this.labelFormat.Size = new System.Drawing.Size(48, 15);
            this.labelFormat.TabIndex = 25;
            this.labelFormat.Text = "Format:";
            // 
            // checkBoxBinaryHex
            // 
            this.checkBoxBinaryHex.AutoSize = true;
            this.checkBoxBinaryHex.Checked = true;
            this.checkBoxBinaryHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBinaryHex.Location = new System.Drawing.Point(223, 55);
            this.checkBoxBinaryHex.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxBinaryHex.Name = "checkBoxBinaryHex";
            this.checkBoxBinaryHex.Size = new System.Drawing.Size(47, 19);
            this.checkBoxBinaryHex.TabIndex = 3;
            this.checkBoxBinaryHex.Text = "Hex";
            this.checkBoxBinaryHex.UseVisualStyleBackColor = true;
            this.checkBoxBinaryHex.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(15, 56);
            this.labelAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(52, 15);
            this.labelAddress.TabIndex = 22;
            this.labelAddress.Text = "Address:";
            // 
            // ComboBoxBinaryType
            // 
            this.ComboBoxBinaryType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.ComboBoxBinaryType.Location = new System.Drawing.Point(223, 23);
            this.ComboBoxBinaryType.Margin = new System.Windows.Forms.Padding(4);
            this.ComboBoxBinaryType.Name = "ComboBoxBinaryType";
            this.ComboBoxBinaryType.Size = new System.Drawing.Size(143, 23);
            this.ComboBoxBinaryType.TabIndex = 1;
            this.ComboBoxBinaryType.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // labelKey
            // 
            this.labelKey.AutoSize = true;
            this.labelKey.Location = new System.Drawing.Point(38, 26);
            this.labelKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelKey.Name = "labelKey";
            this.labelKey.Size = new System.Drawing.Size(29, 15);
            this.labelKey.TabIndex = 19;
            this.labelKey.Text = "Key:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 246);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 15);
            this.label4.TabIndex = 28;
            this.label4.Text = "Author:";
            // 
            // textBoxBinaryAuthor
            // 
            this.textBoxBinaryAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBinaryAuthor.Location = new System.Drawing.Point(86, 242);
            this.textBoxBinaryAuthor.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxBinaryAuthor.Name = "textBoxBinaryAuthor";
            this.textBoxBinaryAuthor.Size = new System.Drawing.Size(415, 23);
            this.textBoxBinaryAuthor.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 277);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 15);
            this.label5.TabIndex = 30;
            this.label5.Text = "Description:";
            // 
            // textBoxBinaryDescription
            // 
            this.textBoxBinaryDescription.AcceptsReturn = true;
            this.textBoxBinaryDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBinaryDescription.Location = new System.Drawing.Point(86, 277);
            this.textBoxBinaryDescription.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxBinaryDescription.Multiline = true;
            this.textBoxBinaryDescription.Name = "textBoxBinaryDescription";
            this.textBoxBinaryDescription.Size = new System.Drawing.Size(413, 128);
            this.textBoxBinaryDescription.TabIndex = 4;
            // 
            // checkBoxBinaryBigEndian
            // 
            this.checkBoxBinaryBigEndian.AutoSize = true;
            this.checkBoxBinaryBigEndian.Location = new System.Drawing.Point(223, 81);
            this.checkBoxBinaryBigEndian.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxBinaryBigEndian.Name = "checkBoxBinaryBigEndian";
            this.checkBoxBinaryBigEndian.Size = new System.Drawing.Size(82, 19);
            this.checkBoxBinaryBigEndian.TabIndex = 5;
            this.checkBoxBinaryBigEndian.Text = "Big Endian";
            this.checkBoxBinaryBigEndian.UseVisualStyleBackColor = true;
            // 
            // comboBoxBinaryItemType
            // 
            this.comboBoxBinaryItemType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxBinaryItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBinaryItemType.FormattingEnabled = true;
            this.comboBoxBinaryItemType.Items.AddRange(new object[] {
            "Level",
            "Model",
            "Action"});
            this.comboBoxBinaryItemType.Location = new System.Drawing.Point(74, 114);
            this.comboBoxBinaryItemType.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxBinaryItemType.Name = "comboBoxBinaryItemType";
            this.comboBoxBinaryItemType.Size = new System.Drawing.Size(140, 23);
            this.comboBoxBinaryItemType.TabIndex = 7;
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(6, 117);
            this.labelType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(60, 15);
            this.labelType.TabIndex = 34;
            this.labelType.Text = "Data type:";
            // 
            // checkBoxBinaryMemory
            // 
            this.checkBoxBinaryMemory.AutoSize = true;
            this.checkBoxBinaryMemory.Location = new System.Drawing.Point(283, 55);
            this.checkBoxBinaryMemory.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxBinaryMemory.Name = "checkBoxBinaryMemory";
            this.checkBoxBinaryMemory.Size = new System.Drawing.Size(114, 19);
            this.checkBoxBinaryMemory.TabIndex = 4;
            this.checkBoxBinaryMemory.Text = "Memory address";
            this.checkBoxBinaryMemory.UseVisualStyleBackColor = true;
            // 
            // labelFile
            // 
            this.labelFile.AutoSize = true;
            this.labelFile.Location = new System.Drawing.Point(11, 9);
            this.labelFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(28, 15);
            this.labelFile.TabIndex = 36;
            this.labelFile.Text = "File:";
            // 
            // groupBoxBinary
            // 
            this.groupBoxBinary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxBinary.Controls.Add(this.label6);
            this.groupBoxBinary.Controls.Add(this.numericUpDownBinaryOffset);
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
            this.groupBoxBinary.Location = new System.Drawing.Point(9, 42);
            this.groupBoxBinary.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxBinary.Name = "groupBoxBinary";
            this.groupBoxBinary.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxBinary.Size = new System.Drawing.Size(492, 192);
            this.groupBoxBinary.TabIndex = 2;
            this.groupBoxBinary.TabStop = false;
            this.groupBoxBinary.Text = "Binary Data";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 85);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 15);
            this.label6.TabIndex = 37;
            this.label6.Text = "Offset:";
            // 
            // numericUpDownBinaryOffset
            // 
            this.numericUpDownBinaryOffset.Hexadecimal = true;
            this.numericUpDownBinaryOffset.Location = new System.Drawing.Point(75, 83);
            this.numericUpDownBinaryOffset.Margin = new System.Windows.Forms.Padding(4);
            this.numericUpDownBinaryOffset.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDownBinaryOffset.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericUpDownBinaryOffset.Name = "numericUpDownBinaryOffset";
            this.numericUpDownBinaryOffset.Size = new System.Drawing.Size(139, 23);
            this.numericUpDownBinaryOffset.TabIndex = 6;
            // 
            // numericUpDownBinaryKey
            // 
            this.numericUpDownBinaryKey.Hexadecimal = true;
            this.numericUpDownBinaryKey.Location = new System.Drawing.Point(75, 23);
            this.numericUpDownBinaryKey.Margin = new System.Windows.Forms.Padding(4);
            this.numericUpDownBinaryKey.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDownBinaryKey.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericUpDownBinaryKey.Name = "numericUpDownBinaryKey";
            this.numericUpDownBinaryKey.Size = new System.Drawing.Size(140, 23);
            this.numericUpDownBinaryKey.TabIndex = 0;
            // 
            // numericUpDownBinaryAddress
            // 
            this.numericUpDownBinaryAddress.Hexadecimal = true;
            this.numericUpDownBinaryAddress.Location = new System.Drawing.Point(75, 53);
            this.numericUpDownBinaryAddress.Margin = new System.Windows.Forms.Padding(4);
            this.numericUpDownBinaryAddress.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericUpDownBinaryAddress.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this.numericUpDownBinaryAddress.Name = "numericUpDownBinaryAddress";
            this.numericUpDownBinaryAddress.Size = new System.Drawing.Size(140, 23);
            this.numericUpDownBinaryAddress.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageBinaryData);
            this.tabControl1.Controls.Add(this.tabPageStructConverter);
            this.tabControl1.Controls.Add(this.tabPageSplit);
            this.tabControl1.Controls.Add(this.tabPageSplitMDL);
            this.tabControl1.Controls.Add(this.tabPageScanner);
            this.tabControl1.Controls.Add(this.tabPageLabelTool);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(515, 520);
            this.tabControl1.TabIndex = 40;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageBinaryData
            // 
            this.tabPageBinaryData.Controls.Add(this.buttonBinaryBrowse);
            this.tabPageBinaryData.Controls.Add(this.textBoxBinaryFilename);
            this.tabPageBinaryData.Controls.Add(this.checkBoxBinaryJSON);
            this.tabPageBinaryData.Controls.Add(this.checkBoxBinaryNJA);
            this.tabPageBinaryData.Controls.Add(this.checkBoxBinaryStructs);
            this.tabPageBinaryData.Controls.Add(this.checkBoxBinarySAModel);
            this.tabPageBinaryData.Controls.Add(this.labelFile);
            this.tabPageBinaryData.Controls.Add(this.groupBoxBinary);
            this.tabPageBinaryData.Controls.Add(this.textBoxBinaryDescription);
            this.tabPageBinaryData.Controls.Add(this.label4);
            this.tabPageBinaryData.Controls.Add(this.textBoxBinaryAuthor);
            this.tabPageBinaryData.Controls.Add(this.label5);
            this.tabPageBinaryData.Controls.Add(this.buttonBinaryExtract);
            this.tabPageBinaryData.Location = new System.Drawing.Point(4, 24);
            this.tabPageBinaryData.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageBinaryData.Name = "tabPageBinaryData";
            this.tabPageBinaryData.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageBinaryData.Size = new System.Drawing.Size(507, 492);
            this.tabPageBinaryData.TabIndex = 0;
            this.tabPageBinaryData.Text = "Binary Data Extractor";
            // 
            // buttonBinaryBrowse
            // 
            this.buttonBinaryBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBinaryBrowse.Location = new System.Drawing.Point(415, 5);
            this.buttonBinaryBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBinaryBrowse.Name = "buttonBinaryBrowse";
            this.buttonBinaryBrowse.Size = new System.Drawing.Size(88, 25);
            this.buttonBinaryBrowse.TabIndex = 1;
            this.buttonBinaryBrowse.Text = "Browse...";
            this.buttonBinaryBrowse.UseVisualStyleBackColor = true;
            this.buttonBinaryBrowse.Click += new System.EventHandler(this.buttonBinaryBrowse_Click);
            // 
            // textBoxBinaryFilename
            // 
            this.textBoxBinaryFilename.AllowDrop = true;
            this.textBoxBinaryFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBinaryFilename.Location = new System.Drawing.Point(47, 6);
            this.textBoxBinaryFilename.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxBinaryFilename.Name = "textBoxBinaryFilename";
            this.textBoxBinaryFilename.Size = new System.Drawing.Size(360, 23);
            this.textBoxBinaryFilename.TabIndex = 0;
            this.textBoxBinaryFilename.TextChanged += new System.EventHandler(this.textBoxBinaryFilename_TextChanged);
            this.textBoxBinaryFilename.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxBinaryFilename_DragDrop);
            this.textBoxBinaryFilename.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxBinaryFilename_DragEnter);
            // 
            // checkBoxBinaryJSON
            // 
            this.checkBoxBinaryJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxBinaryJSON.AutoSize = true;
            this.checkBoxBinaryJSON.Location = new System.Drawing.Point(181, 440);
            this.checkBoxBinaryJSON.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxBinaryJSON.Name = "checkBoxBinaryJSON";
            this.checkBoxBinaryJSON.Size = new System.Drawing.Size(175, 19);
            this.checkBoxBinaryJSON.TabIndex = 8;
            this.checkBoxBinaryJSON.Text = "Convert animations to JSON";
            this.checkBoxBinaryJSON.UseVisualStyleBackColor = true;
            this.checkBoxBinaryJSON.CheckedChanged += new System.EventHandler(this.checkBox_JSON_CheckedChanged);
            // 
            // checkBoxBinaryNJA
            // 
            this.checkBoxBinaryNJA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxBinaryNJA.AutoSize = true;
            this.checkBoxBinaryNJA.Location = new System.Drawing.Point(6, 440);
            this.checkBoxBinaryNJA.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxBinaryNJA.Name = "checkBoxBinaryNJA";
            this.checkBoxBinaryNJA.Size = new System.Drawing.Size(122, 19);
            this.checkBoxBinaryNJA.TabIndex = 7;
            this.checkBoxBinaryNJA.Text = "Export Ninja ASCII";
            this.checkBoxBinaryNJA.UseVisualStyleBackColor = true;
            this.checkBoxBinaryNJA.CheckedChanged += new System.EventHandler(this.checkBox_NJA_CheckedChanged);
            // 
            // checkBoxBinaryStructs
            // 
            this.checkBoxBinaryStructs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxBinaryStructs.AutoSize = true;
            this.checkBoxBinaryStructs.Location = new System.Drawing.Point(181, 413);
            this.checkBoxBinaryStructs.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxBinaryStructs.Name = "checkBoxBinaryStructs";
            this.checkBoxBinaryStructs.Size = new System.Drawing.Size(109, 19);
            this.checkBoxBinaryStructs.TabIndex = 6;
            this.checkBoxBinaryStructs.Text = "Export C structs";
            this.checkBoxBinaryStructs.UseVisualStyleBackColor = true;
            this.checkBoxBinaryStructs.CheckedChanged += new System.EventHandler(this.checkBox_Structs_CheckedChanged);
            // 
            // checkBoxBinarySAModel
            // 
            this.checkBoxBinarySAModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxBinarySAModel.AutoSize = true;
            this.checkBoxBinarySAModel.Checked = true;
            this.checkBoxBinarySAModel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBinarySAModel.Location = new System.Drawing.Point(6, 413);
            this.checkBoxBinarySAModel.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxBinarySAModel.Name = "checkBoxBinarySAModel";
            this.checkBoxBinarySAModel.Size = new System.Drawing.Size(135, 19);
            this.checkBoxBinarySAModel.TabIndex = 5;
            this.checkBoxBinarySAModel.Text = "Export SAModel files";
            this.checkBoxBinarySAModel.UseVisualStyleBackColor = true;
            this.checkBoxBinarySAModel.CheckedChanged += new System.EventHandler(this.checkBox_SAModel_CheckedChanged);
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
            this.tabPageStructConverter.Controls.Add(this.buttonStructConvAddBatch);
            this.tabPageStructConverter.Controls.Add(this.listBoxStructConverter);
            this.tabPageStructConverter.Location = new System.Drawing.Point(4, 24);
            this.tabPageStructConverter.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageStructConverter.Name = "tabPageStructConverter";
            this.tabPageStructConverter.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageStructConverter.Size = new System.Drawing.Size(507, 492);
            this.tabPageStructConverter.TabIndex = 1;
            this.tabPageStructConverter.Text = "Struct Converter";
            // 
            // checkBoxStructConvStructs
            // 
            this.checkBoxStructConvStructs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxStructConvStructs.AutoSize = true;
            this.checkBoxStructConvStructs.Checked = true;
            this.checkBoxStructConvStructs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStructConvStructs.Location = new System.Drawing.Point(181, 413);
            this.checkBoxStructConvStructs.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStructConvStructs.Name = "checkBoxStructConvStructs";
            this.checkBoxStructConvStructs.Size = new System.Drawing.Size(109, 19);
            this.checkBoxStructConvStructs.TabIndex = 5;
            this.checkBoxStructConvStructs.Text = "Export C structs";
            this.checkBoxStructConvStructs.UseVisualStyleBackColor = true;
            this.checkBoxStructConvStructs.CheckedChanged += new System.EventHandler(this.checkBox_StructConvStructs_CheckedChanged);
            // 
            // checkBoxStructConvJSON
            // 
            this.checkBoxStructConvJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxStructConvJSON.AutoSize = true;
            this.checkBoxStructConvJSON.Location = new System.Drawing.Point(181, 440);
            this.checkBoxStructConvJSON.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStructConvJSON.Name = "checkBoxStructConvJSON";
            this.checkBoxStructConvJSON.Size = new System.Drawing.Size(175, 19);
            this.checkBoxStructConvJSON.TabIndex = 7;
            this.checkBoxStructConvJSON.Text = "Convert animations to JSON";
            this.checkBoxStructConvJSON.UseVisualStyleBackColor = true;
            this.checkBoxStructConvJSON.CheckedChanged += new System.EventHandler(this.checkBox_StructConvJSON_CheckedChanged);
            // 
            // checkBoxStructConvNJA
            // 
            this.checkBoxStructConvNJA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxStructConvNJA.AutoSize = true;
            this.checkBoxStructConvNJA.Location = new System.Drawing.Point(6, 440);
            this.checkBoxStructConvNJA.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStructConvNJA.Name = "checkBoxStructConvNJA";
            this.checkBoxStructConvNJA.Size = new System.Drawing.Size(122, 19);
            this.checkBoxStructConvNJA.TabIndex = 6;
            this.checkBoxStructConvNJA.Text = "Export Ninja ASCII";
            this.checkBoxStructConvNJA.UseVisualStyleBackColor = true;
            this.checkBoxStructConvNJA.CheckedChanged += new System.EventHandler(this.checkBox_StructConvNJA_CheckedChanged);
            // 
            // buttonStructConvRemoveAllBatch
            // 
            this.buttonStructConvRemoveAllBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStructConvRemoveAllBatch.Location = new System.Drawing.Point(415, 69);
            this.buttonStructConvRemoveAllBatch.Margin = new System.Windows.Forms.Padding(4);
            this.buttonStructConvRemoveAllBatch.Name = "buttonStructConvRemoveAllBatch";
            this.buttonStructConvRemoveAllBatch.Size = new System.Drawing.Size(88, 25);
            this.buttonStructConvRemoveAllBatch.TabIndex = 3;
            this.buttonStructConvRemoveAllBatch.Text = "Clear All";
            this.buttonStructConvRemoveAllBatch.UseVisualStyleBackColor = true;
            this.buttonStructConvRemoveAllBatch.Click += new System.EventHandler(this.buttonRemoveAllBatch_Click);
            // 
            // buttonStructConvRemoveSelBatch
            // 
            this.buttonStructConvRemoveSelBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStructConvRemoveSelBatch.Enabled = false;
            this.buttonStructConvRemoveSelBatch.Location = new System.Drawing.Point(415, 36);
            this.buttonStructConvRemoveSelBatch.Margin = new System.Windows.Forms.Padding(4);
            this.buttonStructConvRemoveSelBatch.Name = "buttonStructConvRemoveSelBatch";
            this.buttonStructConvRemoveSelBatch.Size = new System.Drawing.Size(88, 25);
            this.buttonStructConvRemoveSelBatch.TabIndex = 2;
            this.buttonStructConvRemoveSelBatch.Text = "&Remove";
            this.buttonStructConvRemoveSelBatch.UseVisualStyleBackColor = true;
            this.buttonStructConvRemoveSelBatch.Click += new System.EventHandler(this.buttonRemoveSelBatch_Click);
            // 
            // buttonStructConvConvertBatch
            // 
            this.buttonStructConvConvertBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStructConvConvertBatch.Enabled = false;
            this.buttonStructConvConvertBatch.Location = new System.Drawing.Point(420, 435);
            this.buttonStructConvConvertBatch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStructConvConvertBatch.Name = "buttonStructConvConvertBatch";
            this.buttonStructConvConvertBatch.Size = new System.Drawing.Size(78, 26);
            this.buttonStructConvConvertBatch.TabIndex = 8;
            this.buttonStructConvConvertBatch.Text = "&Start";
            this.buttonStructConvConvertBatch.UseVisualStyleBackColor = true;
            this.buttonStructConvConvertBatch.Click += new System.EventHandler(this.buttonConvertBatch_Click);
            // 
            // checkBoxStructConvSameOutputFolderBatch
            // 
            this.checkBoxStructConvSameOutputFolderBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxStructConvSameOutputFolderBatch.AutoSize = true;
            this.checkBoxStructConvSameOutputFolderBatch.Location = new System.Drawing.Point(6, 413);
            this.checkBoxStructConvSameOutputFolderBatch.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStructConvSameOutputFolderBatch.Name = "checkBoxStructConvSameOutputFolderBatch";
            this.checkBoxStructConvSameOutputFolderBatch.Size = new System.Drawing.Size(141, 19);
            this.checkBoxStructConvSameOutputFolderBatch.TabIndex = 4;
            this.checkBoxStructConvSameOutputFolderBatch.Text = "Same output folder(s)";
            this.checkBoxStructConvSameOutputFolderBatch.UseVisualStyleBackColor = true;
            // 
            // buttonStructConvAddBatch
            // 
            this.buttonStructConvAddBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStructConvAddBatch.Location = new System.Drawing.Point(415, 5);
            this.buttonStructConvAddBatch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStructConvAddBatch.Name = "buttonStructConvAddBatch";
            this.buttonStructConvAddBatch.Size = new System.Drawing.Size(88, 25);
            this.buttonStructConvAddBatch.TabIndex = 1;
            this.buttonStructConvAddBatch.Text = "&Add...";
            this.buttonStructConvAddBatch.UseVisualStyleBackColor = true;
            this.buttonStructConvAddBatch.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // listBoxStructConverter
            // 
            this.listBoxStructConverter.AllowDrop = true;
            this.listBoxStructConverter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxStructConverter.FormattingEnabled = true;
            this.listBoxStructConverter.HorizontalScrollbar = true;
            this.listBoxStructConverter.ItemHeight = 15;
            this.listBoxStructConverter.Location = new System.Drawing.Point(2, 4);
            this.listBoxStructConverter.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxStructConverter.Name = "listBoxStructConverter";
            this.listBoxStructConverter.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxStructConverter.Size = new System.Drawing.Size(407, 394);
            this.listBoxStructConverter.TabIndex = 0;
            this.listBoxStructConverter.SelectedIndexChanged += new System.EventHandler(this.listBoxStructConverter_SelectedIndexChanged);
            this.listBoxStructConverter.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxStructConverter_DragDrop);
            this.listBoxStructConverter.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxStructConverter_DragEnter);
            // 
            // tabPageSplit
            // 
            this.tabPageSplit.Controls.Add(this.comboBoxLabels);
            this.tabPageSplit.Controls.Add(this.label9);
            this.tabPageSplit.Controls.Add(this.buttonClearAllSplit);
            this.tabPageSplit.Controls.Add(this.buttonRemoveSplit);
            this.tabPageSplit.Controls.Add(this.comboBoxSplitGameSelect);
            this.tabPageSplit.Controls.Add(this.buttonSplitStart);
            this.tabPageSplit.Controls.Add(this.checkBoxSameFolderSplit);
            this.tabPageSplit.Controls.Add(this.buttonAddFilesSplit);
            this.tabPageSplit.Controls.Add(this.listBoxSplitFiles);
            this.tabPageSplit.Location = new System.Drawing.Point(4, 24);
            this.tabPageSplit.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageSplit.Name = "tabPageSplit";
            this.tabPageSplit.Size = new System.Drawing.Size(507, 492);
            this.tabPageSplit.TabIndex = 2;
            this.tabPageSplit.Text = "Split";
            // 
            // comboBoxLabels
            // 
            this.comboBoxLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLabels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLabels.FormattingEnabled = true;
            this.comboBoxLabels.Items.AddRange(new object[] {
            "Address-based (object_0000...)",
            "Full labels from debug symbols",
            "No labels"});
            this.comboBoxLabels.Location = new System.Drawing.Point(290, 4);
            this.comboBoxLabels.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxLabels.Name = "comboBoxLabels";
            this.comboBoxLabels.Size = new System.Drawing.Size(213, 23);
            this.comboBoxLabels.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(235, 7);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 15);
            this.label9.TabIndex = 52;
            this.label9.Text = "Labels:";
            // 
            // buttonClearAllSplit
            // 
            this.buttonClearAllSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearAllSplit.Location = new System.Drawing.Point(415, 101);
            this.buttonClearAllSplit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClearAllSplit.Name = "buttonClearAllSplit";
            this.buttonClearAllSplit.Size = new System.Drawing.Size(88, 25);
            this.buttonClearAllSplit.TabIndex = 5;
            this.buttonClearAllSplit.Text = "Clear &all";
            this.buttonClearAllSplit.UseVisualStyleBackColor = true;
            this.buttonClearAllSplit.Click += new System.EventHandler(this.buttonClearAllSplit_Click);
            // 
            // buttonRemoveSplit
            // 
            this.buttonRemoveSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveSplit.Enabled = false;
            this.buttonRemoveSplit.Location = new System.Drawing.Point(415, 67);
            this.buttonRemoveSplit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRemoveSplit.Name = "buttonRemoveSplit";
            this.buttonRemoveSplit.Size = new System.Drawing.Size(88, 25);
            this.buttonRemoveSplit.TabIndex = 4;
            this.buttonRemoveSplit.Text = "&Remove";
            this.buttonRemoveSplit.UseVisualStyleBackColor = true;
            this.buttonRemoveSplit.Click += new System.EventHandler(this.buttonRemoveSplit_Click);
            // 
            // comboBoxSplitGameSelect
            // 
            this.comboBoxSplitGameSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSplitGameSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSplitGameSelect.FormattingEnabled = true;
            this.comboBoxSplitGameSelect.Location = new System.Drawing.Point(9, 4);
            this.comboBoxSplitGameSelect.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxSplitGameSelect.Name = "comboBoxSplitGameSelect";
            this.comboBoxSplitGameSelect.Size = new System.Drawing.Size(218, 23);
            this.comboBoxSplitGameSelect.TabIndex = 0;
            // 
            // buttonSplitStart
            // 
            this.buttonSplitStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSplitStart.Enabled = false;
            this.buttonSplitStart.Location = new System.Drawing.Point(420, 435);
            this.buttonSplitStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSplitStart.Name = "buttonSplitStart";
            this.buttonSplitStart.Size = new System.Drawing.Size(78, 26);
            this.buttonSplitStart.TabIndex = 7;
            this.buttonSplitStart.Text = "&Start";
            this.buttonSplitStart.UseVisualStyleBackColor = true;
            this.buttonSplitStart.Click += new System.EventHandler(this.buttonSplit_Click);
            // 
            // checkBoxSameFolderSplit
            // 
            this.checkBoxSameFolderSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxSameFolderSplit.AutoSize = true;
            this.checkBoxSameFolderSplit.Location = new System.Drawing.Point(6, 440);
            this.checkBoxSameFolderSplit.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSameFolderSplit.Name = "checkBoxSameFolderSplit";
            this.checkBoxSameFolderSplit.Size = new System.Drawing.Size(212, 19);
            this.checkBoxSameFolderSplit.TabIndex = 6;
            this.checkBoxSameFolderSplit.Text = "Same output folder as source file(s)";
            this.checkBoxSameFolderSplit.UseVisualStyleBackColor = true;
            // 
            // buttonAddFilesSplit
            // 
            this.buttonAddFilesSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddFilesSplit.Location = new System.Drawing.Point(415, 33);
            this.buttonAddFilesSplit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAddFilesSplit.Name = "buttonAddFilesSplit";
            this.buttonAddFilesSplit.Size = new System.Drawing.Size(88, 25);
            this.buttonAddFilesSplit.TabIndex = 3;
            this.buttonAddFilesSplit.Text = "&Add...";
            this.buttonAddFilesSplit.UseVisualStyleBackColor = true;
            this.buttonAddFilesSplit.Click += new System.EventHandler(this.button_AddFilesSplit_Click);
            // 
            // listBoxSplitFiles
            // 
            this.listBoxSplitFiles.AllowDrop = true;
            this.listBoxSplitFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxSplitFiles.FormattingEnabled = true;
            this.listBoxSplitFiles.HorizontalScrollbar = true;
            this.listBoxSplitFiles.ItemHeight = 15;
            this.listBoxSplitFiles.Location = new System.Drawing.Point(7, 33);
            this.listBoxSplitFiles.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxSplitFiles.Name = "listBoxSplitFiles";
            this.listBoxSplitFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxSplitFiles.Size = new System.Drawing.Size(404, 394);
            this.listBoxSplitFiles.TabIndex = 2;
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
            this.tabPageSplitMDL.Controls.Add(this.buttonMDLAnimFilesRemove);
            this.tabPageSplitMDL.Controls.Add(this.buttonAnimFilesClear);
            this.tabPageSplitMDL.Controls.Add(this.checkBoxMDLBigEndian);
            this.tabPageSplitMDL.Controls.Add(this.buttonSplitMDL);
            this.tabPageSplitMDL.Controls.Add(this.buttonAnimFilesAdd);
            this.tabPageSplitMDL.Controls.Add(this.animationFilesLabel);
            this.tabPageSplitMDL.Controls.Add(this.listBoxMDLAnimationFiles);
            this.tabPageSplitMDL.Location = new System.Drawing.Point(4, 24);
            this.tabPageSplitMDL.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageSplitMDL.Name = "tabPageSplitMDL";
            this.tabPageSplitMDL.Size = new System.Drawing.Size(507, 492);
            this.tabPageSplitMDL.TabIndex = 3;
            this.tabPageSplitMDL.Text = "SplitMDL";
            // 
            // buttonMDLBrowse
            // 
            this.buttonMDLBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMDLBrowse.Location = new System.Drawing.Point(415, 5);
            this.buttonMDLBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.buttonMDLBrowse.Name = "buttonMDLBrowse";
            this.buttonMDLBrowse.Size = new System.Drawing.Size(88, 25);
            this.buttonMDLBrowse.TabIndex = 1;
            this.buttonMDLBrowse.Text = "Browse...";
            this.buttonMDLBrowse.UseVisualStyleBackColor = true;
            this.buttonMDLBrowse.Click += new System.EventHandler(this.buttonMDLBrowse_Click);
            // 
            // textBoxMDLFilename
            // 
            this.textBoxMDLFilename.AllowDrop = true;
            this.textBoxMDLFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMDLFilename.Location = new System.Drawing.Point(47, 6);
            this.textBoxMDLFilename.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxMDLFilename.Name = "textBoxMDLFilename";
            this.textBoxMDLFilename.Size = new System.Drawing.Size(360, 23);
            this.textBoxMDLFilename.TabIndex = 0;
            this.textBoxMDLFilename.TextChanged += new System.EventHandler(this.textBoxMDLFilename_TextChanged);
            this.textBoxMDLFilename.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxMDLFilename_DragDrop);
            this.textBoxMDLFilename.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxMDLFilename_DragEnter);
            // 
            // checkBoxMDLSameFolder
            // 
            this.checkBoxMDLSameFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxMDLSameFolder.AutoSize = true;
            this.checkBoxMDLSameFolder.Location = new System.Drawing.Point(6, 440);
            this.checkBoxMDLSameFolder.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxMDLSameFolder.Name = "checkBoxMDLSameFolder";
            this.checkBoxMDLSameFolder.Size = new System.Drawing.Size(209, 19);
            this.checkBoxMDLSameFolder.TabIndex = 7;
            this.checkBoxMDLSameFolder.Text = "Same output folder as the MDL file";
            this.checkBoxMDLSameFolder.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 9);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(28, 15);
            this.label8.TabIndex = 48;
            this.label8.Text = "File:";
            // 
            // buttonMDLAnimFilesRemove
            // 
            this.buttonMDLAnimFilesRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMDLAnimFilesRemove.Enabled = false;
            this.buttonMDLAnimFilesRemove.Location = new System.Drawing.Point(415, 99);
            this.buttonMDLAnimFilesRemove.Margin = new System.Windows.Forms.Padding(4);
            this.buttonMDLAnimFilesRemove.Name = "buttonMDLAnimFilesRemove";
            this.buttonMDLAnimFilesRemove.Size = new System.Drawing.Size(88, 25);
            this.buttonMDLAnimFilesRemove.TabIndex = 5;
            this.buttonMDLAnimFilesRemove.Text = "Remove";
            this.buttonMDLAnimFilesRemove.UseVisualStyleBackColor = true;
            this.buttonMDLAnimFilesRemove.Click += new System.EventHandler(this.buttonMDLAnimFilesRemove_Click);
            // 
            // buttonAnimFilesClear
            // 
            this.buttonAnimFilesClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAnimFilesClear.Location = new System.Drawing.Point(415, 136);
            this.buttonAnimFilesClear.Margin = new System.Windows.Forms.Padding(4);
            this.buttonAnimFilesClear.Name = "buttonAnimFilesClear";
            this.buttonAnimFilesClear.Size = new System.Drawing.Size(88, 25);
            this.buttonAnimFilesClear.TabIndex = 6;
            this.buttonAnimFilesClear.Text = "Clear all";
            this.buttonAnimFilesClear.UseVisualStyleBackColor = true;
            this.buttonAnimFilesClear.Click += new System.EventHandler(this.buttonAnimFilesClear_Click);
            // 
            // checkBoxMDLBigEndian
            // 
            this.checkBoxMDLBigEndian.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxMDLBigEndian.AutoSize = true;
            this.checkBoxMDLBigEndian.Checked = true;
            this.checkBoxMDLBigEndian.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMDLBigEndian.Location = new System.Drawing.Point(356, 38);
            this.checkBoxMDLBigEndian.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxMDLBigEndian.Name = "checkBoxMDLBigEndian";
            this.checkBoxMDLBigEndian.Size = new System.Drawing.Size(129, 19);
            this.checkBoxMDLBigEndian.TabIndex = 2;
            this.checkBoxMDLBigEndian.Text = "Big Endian (GC/PC)";
            this.checkBoxMDLBigEndian.UseVisualStyleBackColor = true;
            // 
            // buttonSplitMDL
            // 
            this.buttonSplitMDL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSplitMDL.Enabled = false;
            this.buttonSplitMDL.Location = new System.Drawing.Point(420, 435);
            this.buttonSplitMDL.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSplitMDL.Name = "buttonSplitMDL";
            this.buttonSplitMDL.Size = new System.Drawing.Size(78, 26);
            this.buttonSplitMDL.TabIndex = 8;
            this.buttonSplitMDL.Text = "&Start";
            this.buttonSplitMDL.UseVisualStyleBackColor = true;
            this.buttonSplitMDL.Click += new System.EventHandler(this.buttonSplitMDL_Click);
            // 
            // buttonAnimFilesAdd
            // 
            this.buttonAnimFilesAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAnimFilesAdd.Location = new System.Drawing.Point(415, 62);
            this.buttonAnimFilesAdd.Margin = new System.Windows.Forms.Padding(4);
            this.buttonAnimFilesAdd.Name = "buttonAnimFilesAdd";
            this.buttonAnimFilesAdd.Size = new System.Drawing.Size(88, 25);
            this.buttonAnimFilesAdd.TabIndex = 4;
            this.buttonAnimFilesAdd.Text = "Add...";
            this.buttonAnimFilesAdd.UseVisualStyleBackColor = true;
            this.buttonAnimFilesAdd.Click += new System.EventHandler(this.buttonAnimFilesAdd_Click);
            // 
            // animationFilesLabel
            // 
            this.animationFilesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.animationFilesLabel.AutoSize = true;
            this.animationFilesLabel.Location = new System.Drawing.Point(7, 43);
            this.animationFilesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.animationFilesLabel.Name = "animationFilesLabel";
            this.animationFilesLabel.Size = new System.Drawing.Size(92, 15);
            this.animationFilesLabel.TabIndex = 16;
            this.animationFilesLabel.Text = "Animation Files:";
            // 
            // listBoxMDLAnimationFiles
            // 
            this.listBoxMDLAnimationFiles.AllowDrop = true;
            this.listBoxMDLAnimationFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxMDLAnimationFiles.FormattingEnabled = true;
            this.listBoxMDLAnimationFiles.ItemHeight = 15;
            this.listBoxMDLAnimationFiles.Location = new System.Drawing.Point(7, 62);
            this.listBoxMDLAnimationFiles.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxMDLAnimationFiles.Name = "listBoxMDLAnimationFiles";
            this.listBoxMDLAnimationFiles.Size = new System.Drawing.Size(404, 364);
            this.listBoxMDLAnimationFiles.TabIndex = 3;
            this.listBoxMDLAnimationFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxMDLAnimationFiles_SelectedIndexChanged);
            this.listBoxMDLAnimationFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxMDLAnimationFiles_DragDrop);
            this.listBoxMDLAnimationFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxMDLAnimationFiles_DragEnter);
            // 
            // tabPageScanner
            // 
            this.tabPageScanner.Controls.Add(this.label11);
            this.tabPageScanner.Controls.Add(this.numericUpDownOffset);
            this.tabPageScanner.Controls.Add(this.groupBox5);
            this.tabPageScanner.Controls.Add(this.buttonInputBrowse);
            this.tabPageScanner.Controls.Add(this.listBoxBaseGame);
            this.tabPageScanner.Controls.Add(this.buttonScanStart);
            this.tabPageScanner.Controls.Add(this.numericUpDownScanBinaryKey);
            this.tabPageScanner.Controls.Add(this.groupBox4);
            this.tabPageScanner.Controls.Add(this.label12);
            this.tabPageScanner.Controls.Add(this.numericUpDownStartAddr);
            this.tabPageScanner.Controls.Add(this.groupBox1);
            this.tabPageScanner.Controls.Add(this.textBoxInputFile);
            this.tabPageScanner.Controls.Add(this.label16);
            this.tabPageScanner.Controls.Add(this.numericUpDownEndAddr);
            this.tabPageScanner.Controls.Add(this.label15);
            this.tabPageScanner.Controls.Add(this.label13);
            this.tabPageScanner.Controls.Add(this.label14);
            this.tabPageScanner.Location = new System.Drawing.Point(4, 24);
            this.tabPageScanner.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageScanner.Name = "tabPageScanner";
            this.tabPageScanner.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageScanner.Size = new System.Drawing.Size(507, 492);
            this.tabPageScanner.TabIndex = 4;
            this.tabPageScanner.Text = "Scanner";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(291, 133);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 15);
            this.label11.TabIndex = 35;
            this.label11.Text = "Data Offset:";
            // 
            // numericUpDownOffset
            // 
            this.numericUpDownOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownOffset.Location = new System.Drawing.Point(368, 131);
            this.numericUpDownOffset.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownOffset.Name = "numericUpDownOffset";
            this.numericUpDownOffset.Size = new System.Drawing.Size(126, 23);
            this.numericUpDownOffset.TabIndex = 6;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.numericUpDownNodes);
            this.groupBox5.Controls.Add(this.labelNodes);
            this.groupBox5.Controls.Add(this.buttonFindBrowse);
            this.groupBox5.Controls.Add(this.checkBoxSearchAction);
            this.groupBox5.Controls.Add(this.textBoxFindModel);
            this.groupBox5.Controls.Add(this.checkBoxSearchLandtables);
            this.groupBox5.Controls.Add(this.checkBoxSearchBasic);
            this.groupBox5.Controls.Add(this.radioButtonFindModel);
            this.groupBox5.Controls.Add(this.checkBoxSearchMotion);
            this.groupBox5.Controls.Add(this.radioButtonSearchData);
            this.groupBox5.Controls.Add(this.checkBoxSearchChunk);
            this.groupBox5.Controls.Add(this.checkBoxSearchGinja);
            this.groupBox5.Location = new System.Drawing.Point(5, 287);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(500, 144);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Scan Settings";
            // 
            // numericUpDownNodes
            // 
            this.numericUpDownNodes.Enabled = false;
            this.numericUpDownNodes.Location = new System.Drawing.Point(439, 65);
            this.numericUpDownNodes.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownNodes.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownNodes.Name = "numericUpDownNodes";
            this.numericUpDownNodes.Size = new System.Drawing.Size(50, 23);
            this.numericUpDownNodes.TabIndex = 24;
            // 
            // labelNodes
            // 
            this.labelNodes.AutoSize = true;
            this.labelNodes.Enabled = false;
            this.labelNodes.Location = new System.Drawing.Point(391, 67);
            this.labelNodes.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelNodes.Name = "labelNodes";
            this.labelNodes.Size = new System.Drawing.Size(44, 15);
            this.labelNodes.TabIndex = 44;
            this.labelNodes.Text = "Nodes:";
            // 
            // buttonFindBrowse
            // 
            this.buttonFindBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFindBrowse.Location = new System.Drawing.Point(413, 111);
            this.buttonFindBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.buttonFindBrowse.Name = "buttonFindBrowse";
            this.buttonFindBrowse.Size = new System.Drawing.Size(74, 25);
            this.buttonFindBrowse.TabIndex = 27;
            this.buttonFindBrowse.Text = "Browse...";
            this.buttonFindBrowse.UseVisualStyleBackColor = true;
            this.buttonFindBrowse.Click += new System.EventHandler(this.buttonFindBrowse_Click);
            // 
            // checkBoxSearchAction
            // 
            this.checkBoxSearchAction.AutoSize = true;
            this.checkBoxSearchAction.Location = new System.Drawing.Point(155, 66);
            this.checkBoxSearchAction.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSearchAction.Name = "checkBoxSearchAction";
            this.checkBoxSearchAction.Size = new System.Drawing.Size(66, 19);
            this.checkBoxSearchAction.TabIndex = 22;
            this.checkBoxSearchAction.Text = "Actions";
            this.checkBoxSearchAction.UseVisualStyleBackColor = true;
            // 
            // textBoxFindModel
            // 
            this.textBoxFindModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFindModel.Location = new System.Drawing.Point(8, 112);
            this.textBoxFindModel.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxFindModel.Name = "textBoxFindModel";
            this.textBoxFindModel.Size = new System.Drawing.Size(392, 23);
            this.textBoxFindModel.TabIndex = 26;
            // 
            // checkBoxSearchLandtables
            // 
            this.checkBoxSearchLandtables.AutoSize = true;
            this.checkBoxSearchLandtables.Location = new System.Drawing.Point(12, 66);
            this.checkBoxSearchLandtables.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSearchLandtables.Name = "checkBoxSearchLandtables";
            this.checkBoxSearchLandtables.Size = new System.Drawing.Size(83, 19);
            this.checkBoxSearchLandtables.TabIndex = 21;
            this.checkBoxSearchLandtables.Text = "Landtables";
            this.checkBoxSearchLandtables.UseVisualStyleBackColor = true;
            // 
            // checkBoxSearchBasic
            // 
            this.checkBoxSearchBasic.AutoSize = true;
            this.checkBoxSearchBasic.Checked = true;
            this.checkBoxSearchBasic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSearchBasic.Location = new System.Drawing.Point(12, 42);
            this.checkBoxSearchBasic.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSearchBasic.Name = "checkBoxSearchBasic";
            this.checkBoxSearchBasic.Size = new System.Drawing.Size(95, 19);
            this.checkBoxSearchBasic.TabIndex = 18;
            this.checkBoxSearchBasic.Text = "Basic Models";
            this.checkBoxSearchBasic.UseVisualStyleBackColor = true;
            this.checkBoxSearchBasic.CheckedChanged += new System.EventHandler(this.checkBoxSearchBasic_CheckedChanged);
            // 
            // radioButtonFindModel
            // 
            this.radioButtonFindModel.AutoSize = true;
            this.radioButtonFindModel.Location = new System.Drawing.Point(4, 89);
            this.radioButtonFindModel.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonFindModel.Name = "radioButtonFindModel";
            this.radioButtonFindModel.Size = new System.Drawing.Size(88, 19);
            this.radioButtonFindModel.TabIndex = 25;
            this.radioButtonFindModel.TabStop = true;
            this.radioButtonFindModel.Text = "Find Model:";
            this.radioButtonFindModel.UseVisualStyleBackColor = true;
            // 
            // checkBoxSearchMotion
            // 
            this.checkBoxSearchMotion.AutoSize = true;
            this.checkBoxSearchMotion.Location = new System.Drawing.Point(303, 66);
            this.checkBoxSearchMotion.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSearchMotion.Name = "checkBoxSearchMotion";
            this.checkBoxSearchMotion.Size = new System.Drawing.Size(70, 19);
            this.checkBoxSearchMotion.TabIndex = 23;
            this.checkBoxSearchMotion.Text = "Motions";
            this.checkBoxSearchMotion.UseVisualStyleBackColor = true;
            this.checkBoxSearchMotion.CheckedChanged += new System.EventHandler(this.checkBoxSearchMotion_CheckedChanged);
            // 
            // radioButtonSearchData
            // 
            this.radioButtonSearchData.AutoSize = true;
            this.radioButtonSearchData.Location = new System.Drawing.Point(4, 19);
            this.radioButtonSearchData.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonSearchData.Name = "radioButtonSearchData";
            this.radioButtonSearchData.Size = new System.Drawing.Size(108, 19);
            this.radioButtonSearchData.TabIndex = 17;
            this.radioButtonSearchData.TabStop = true;
            this.radioButtonSearchData.Text = "Search for Data:";
            this.radioButtonSearchData.UseVisualStyleBackColor = true;
            this.radioButtonSearchData.CheckedChanged += new System.EventHandler(this.radioButtonSearchData_CheckedChanged);
            // 
            // checkBoxSearchChunk
            // 
            this.checkBoxSearchChunk.AutoSize = true;
            this.checkBoxSearchChunk.Location = new System.Drawing.Point(155, 42);
            this.checkBoxSearchChunk.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSearchChunk.Name = "checkBoxSearchChunk";
            this.checkBoxSearchChunk.Size = new System.Drawing.Size(103, 19);
            this.checkBoxSearchChunk.TabIndex = 19;
            this.checkBoxSearchChunk.Text = "Chunk Models";
            this.checkBoxSearchChunk.UseVisualStyleBackColor = true;
            this.checkBoxSearchChunk.CheckedChanged += new System.EventHandler(this.checkBoxSearchChunk_CheckedChanged);
            // 
            // checkBoxSearchGinja
            // 
            this.checkBoxSearchGinja.AutoSize = true;
            this.checkBoxSearchGinja.Location = new System.Drawing.Point(303, 42);
            this.checkBoxSearchGinja.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSearchGinja.Name = "checkBoxSearchGinja";
            this.checkBoxSearchGinja.Size = new System.Drawing.Size(95, 19);
            this.checkBoxSearchGinja.TabIndex = 20;
            this.checkBoxSearchGinja.Text = "Ginja Models";
            this.checkBoxSearchGinja.UseVisualStyleBackColor = true;
            this.checkBoxSearchGinja.CheckedChanged += new System.EventHandler(this.checkBoxSearchGinja_CheckedChanged);
            // 
            // buttonInputBrowse
            // 
            this.buttonInputBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInputBrowse.Location = new System.Drawing.Point(415, 5);
            this.buttonInputBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.buttonInputBrowse.Name = "buttonInputBrowse";
            this.buttonInputBrowse.Size = new System.Drawing.Size(88, 25);
            this.buttonInputBrowse.TabIndex = 1;
            this.buttonInputBrowse.Text = "Browse...";
            this.buttonInputBrowse.UseVisualStyleBackColor = true;
            this.buttonInputBrowse.Click += new System.EventHandler(this.buttonInputBrowse_Click);
            // 
            // listBoxBaseGame
            // 
            this.listBoxBaseGame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxBaseGame.FormattingEnabled = true;
            this.listBoxBaseGame.ItemHeight = 15;
            this.listBoxBaseGame.Items.AddRange(new object[] {
            "SA1 Dreamcast",
            "SADX Gamecube",
            "SADX PC",
            "SADX X360",
            "SA2 Dreamcast",
            "SA2B Gamecube",
            "SA2 PC"});
            this.listBoxBaseGame.Location = new System.Drawing.Point(7, 60);
            this.listBoxBaseGame.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxBaseGame.Name = "listBoxBaseGame";
            this.listBoxBaseGame.Size = new System.Drawing.Size(267, 94);
            this.listBoxBaseGame.TabIndex = 2;
            this.listBoxBaseGame.SelectedIndexChanged += new System.EventHandler(this.listBoxBaseGame_SelectedIndexChanged);
            // 
            // buttonScanStart
            // 
            this.buttonScanStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonScanStart.Enabled = false;
            this.buttonScanStart.Location = new System.Drawing.Point(420, 435);
            this.buttonScanStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonScanStart.Name = "buttonScanStart";
            this.buttonScanStart.Size = new System.Drawing.Size(78, 26);
            this.buttonScanStart.TabIndex = 9;
            this.buttonScanStart.Text = "&Start";
            this.buttonScanStart.UseVisualStyleBackColor = true;
            this.buttonScanStart.Click += new System.EventHandler(this.buttonScanStart_Click);
            // 
            // numericUpDownScanBinaryKey
            // 
            this.numericUpDownScanBinaryKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownScanBinaryKey.Location = new System.Drawing.Point(368, 50);
            this.numericUpDownScanBinaryKey.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownScanBinaryKey.Name = "numericUpDownScanBinaryKey";
            this.numericUpDownScanBinaryKey.Size = new System.Drawing.Size(126, 23);
            this.numericUpDownScanBinaryKey.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.checkBoxKeepLevel);
            this.groupBox4.Controls.Add(this.buttonOutputBrowse);
            this.groupBox4.Controls.Add(this.checkBoxKeepChild);
            this.groupBox4.Controls.Add(this.checkBoxSkipMeta);
            this.groupBox4.Controls.Add(this.checkBoxSingleOutput);
            this.groupBox4.Controls.Add(this.textBoxOutputFolder);
            this.groupBox4.Location = new System.Drawing.Point(5, 158);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(343, 125);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output Settings";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 70);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 15);
            this.label10.TabIndex = 40;
            this.label10.Text = "Output Folder:";
            // 
            // checkBoxKeepLevel
            // 
            this.checkBoxKeepLevel.AutoSize = true;
            this.checkBoxKeepLevel.Location = new System.Drawing.Point(155, 17);
            this.checkBoxKeepLevel.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxKeepLevel.Name = "checkBoxKeepLevel";
            this.checkBoxKeepLevel.Size = new System.Drawing.Size(124, 19);
            this.checkBoxKeepLevel.TabIndex = 9;
            this.checkBoxKeepLevel.Text = "Keep Level Models";
            this.checkBoxKeepLevel.UseVisualStyleBackColor = true;
            // 
            // buttonOutputBrowse
            // 
            this.buttonOutputBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOutputBrowse.Location = new System.Drawing.Point(265, 85);
            this.buttonOutputBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOutputBrowse.Name = "buttonOutputBrowse";
            this.buttonOutputBrowse.Size = new System.Drawing.Size(74, 25);
            this.buttonOutputBrowse.TabIndex = 12;
            this.buttonOutputBrowse.Text = "Browse...";
            this.buttonOutputBrowse.UseVisualStyleBackColor = true;
            this.buttonOutputBrowse.Click += new System.EventHandler(this.buttonOutputBrowse_Click);
            // 
            // checkBoxKeepChild
            // 
            this.checkBoxKeepChild.AutoSize = true;
            this.checkBoxKeepChild.Location = new System.Drawing.Point(155, 43);
            this.checkBoxKeepChild.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxKeepChild.Name = "checkBoxKeepChild";
            this.checkBoxKeepChild.Size = new System.Drawing.Size(125, 19);
            this.checkBoxKeepChild.TabIndex = 10;
            this.checkBoxKeepChild.Text = "Keep Child Models";
            this.checkBoxKeepChild.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkipMeta
            // 
            this.checkBoxSkipMeta.AutoSize = true;
            this.checkBoxSkipMeta.Location = new System.Drawing.Point(6, 17);
            this.checkBoxSkipMeta.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSkipMeta.Name = "checkBoxSkipMeta";
            this.checkBoxSkipMeta.Size = new System.Drawing.Size(101, 19);
            this.checkBoxSkipMeta.TabIndex = 7;
            this.checkBoxSkipMeta.Text = "Skip Metadata";
            this.checkBoxSkipMeta.UseVisualStyleBackColor = true;
            // 
            // checkBoxSingleOutput
            // 
            this.checkBoxSingleOutput.AutoSize = true;
            this.checkBoxSingleOutput.Location = new System.Drawing.Point(6, 43);
            this.checkBoxSingleOutput.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSingleOutput.Name = "checkBoxSingleOutput";
            this.checkBoxSingleOutput.Size = new System.Drawing.Size(135, 19);
            this.checkBoxSingleOutput.TabIndex = 8;
            this.checkBoxSingleOutput.Text = "Single Output Folder";
            this.checkBoxSingleOutput.UseVisualStyleBackColor = true;
            // 
            // textBoxOutputFolder
            // 
            this.textBoxOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutputFolder.Location = new System.Drawing.Point(4, 87);
            this.textBoxOutputFolder.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxOutputFolder.Name = "textBoxOutputFolder";
            this.textBoxOutputFolder.Size = new System.Drawing.Size(257, 23);
            this.textBoxOutputFolder.TabIndex = 11;
            this.textBoxOutputFolder.TextChanged += new System.EventHandler(this.textBoxOutputFolder_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 9);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(28, 15);
            this.label12.TabIndex = 32;
            this.label12.Text = "File:";
            // 
            // numericUpDownStartAddr
            // 
            this.numericUpDownStartAddr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownStartAddr.Location = new System.Drawing.Point(368, 77);
            this.numericUpDownStartAddr.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownStartAddr.Name = "numericUpDownStartAddr";
            this.numericUpDownStartAddr.Size = new System.Drawing.Size(126, 23);
            this.numericUpDownStartAddr.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxSimpleScan);
            this.groupBox1.Controls.Add(this.checkBoxShortRot);
            this.groupBox1.Controls.Add(this.checkBoxBasicSADX);
            this.groupBox1.Controls.Add(this.checkBoxBigEndian);
            this.groupBox1.Location = new System.Drawing.Point(353, 158);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(152, 125);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Settings";
            // 
            // checkBoxSimpleScan
            // 
            this.checkBoxSimpleScan.AutoSize = true;
            this.checkBoxSimpleScan.Location = new System.Drawing.Point(4, 71);
            this.checkBoxSimpleScan.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSimpleScan.Name = "checkBoxSimpleScan";
            this.checkBoxSimpleScan.Size = new System.Drawing.Size(90, 19);
            this.checkBoxSimpleScan.TabIndex = 15;
            this.checkBoxSimpleScan.Text = "Simple Scan";
            this.checkBoxSimpleScan.UseVisualStyleBackColor = true;
            // 
            // checkBoxShortRot
            // 
            this.checkBoxShortRot.AutoSize = true;
            this.checkBoxShortRot.Location = new System.Drawing.Point(4, 97);
            this.checkBoxShortRot.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxShortRot.Name = "checkBoxShortRot";
            this.checkBoxShortRot.Size = new System.Drawing.Size(102, 19);
            this.checkBoxShortRot.TabIndex = 16;
            this.checkBoxShortRot.Text = "Short Rotation";
            this.checkBoxShortRot.UseVisualStyleBackColor = true;
            // 
            // checkBoxBasicSADX
            // 
            this.checkBoxBasicSADX.AutoSize = true;
            this.checkBoxBasicSADX.Location = new System.Drawing.Point(4, 45);
            this.checkBoxBasicSADX.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxBasicSADX.Name = "checkBoxBasicSADX";
            this.checkBoxBasicSADX.Size = new System.Drawing.Size(103, 19);
            this.checkBoxBasicSADX.TabIndex = 14;
            this.checkBoxBasicSADX.Text = "Basic+ Models";
            this.checkBoxBasicSADX.UseVisualStyleBackColor = false;
            // 
            // checkBoxBigEndian
            // 
            this.checkBoxBigEndian.AutoSize = true;
            this.checkBoxBigEndian.Location = new System.Drawing.Point(4, 19);
            this.checkBoxBigEndian.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxBigEndian.Name = "checkBoxBigEndian";
            this.checkBoxBigEndian.Size = new System.Drawing.Size(82, 19);
            this.checkBoxBigEndian.TabIndex = 13;
            this.checkBoxBigEndian.Text = "Big Endian";
            this.checkBoxBigEndian.UseVisualStyleBackColor = true;
            // 
            // textBoxInputFile
            // 
            this.textBoxInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInputFile.Location = new System.Drawing.Point(47, 6);
            this.textBoxInputFile.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxInputFile.Name = "textBoxInputFile";
            this.textBoxInputFile.Size = new System.Drawing.Size(360, 23);
            this.textBoxInputFile.TabIndex = 0;
            this.textBoxInputFile.TextChanged += new System.EventHandler(this.textBoxInputFile_TextChanged);
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(295, 52);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 15);
            this.label16.TabIndex = 19;
            this.label16.Text = "Binary Key:";
            // 
            // numericUpDownEndAddr
            // 
            this.numericUpDownEndAddr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownEndAddr.Location = new System.Drawing.Point(368, 104);
            this.numericUpDownEndAddr.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownEndAddr.Name = "numericUpDownEndAddr";
            this.numericUpDownEndAddr.Size = new System.Drawing.Size(126, 23);
            this.numericUpDownEndAddr.TabIndex = 5;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(285, 106);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(75, 15);
            this.label15.TabIndex = 18;
            this.label15.Text = "End Address:";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(281, 79);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(79, 15);
            this.label13.TabIndex = 17;
            this.label13.Text = "Start Address:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 43);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(68, 15);
            this.label14.TabIndex = 29;
            this.label14.Text = "Base Game:";
            // 
            // tabPageLabelTool
            // 
            this.tabPageLabelTool.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageLabelTool.Controls.Add(this.checkBoxLabelDeleteFiles);
            this.tabPageLabelTool.Controls.Add(this.buttonLabelStart);
            this.tabPageLabelTool.Controls.Add(this.checkBoxLabelClearInternal);
            this.tabPageLabelTool.Controls.Add(this.radioButtonLabelImport);
            this.tabPageLabelTool.Controls.Add(this.radioButtonLabelExport);
            this.tabPageLabelTool.Controls.Add(this.buttonLabelClear);
            this.tabPageLabelTool.Controls.Add(this.buttonLabelRemove);
            this.tabPageLabelTool.Controls.Add(this.buttonLabelAdd);
            this.tabPageLabelTool.Controls.Add(this.listBoxLabelTool);
            this.tabPageLabelTool.Location = new System.Drawing.Point(4, 24);
            this.tabPageLabelTool.Name = "tabPageLabelTool";
            this.tabPageLabelTool.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLabelTool.Size = new System.Drawing.Size(507, 492);
            this.tabPageLabelTool.TabIndex = 5;
            this.tabPageLabelTool.Text = "Label Tool";
            // 
            // checkBoxLabelDeleteFiles
            // 
            this.checkBoxLabelDeleteFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxLabelDeleteFiles.AutoSize = true;
            this.checkBoxLabelDeleteFiles.Enabled = false;
            this.checkBoxLabelDeleteFiles.Location = new System.Drawing.Point(156, 440);
            this.checkBoxLabelDeleteFiles.Name = "checkBoxLabelDeleteFiles";
            this.checkBoxLabelDeleteFiles.Size = new System.Drawing.Size(116, 19);
            this.checkBoxLabelDeleteFiles.TabIndex = 9;
            this.checkBoxLabelDeleteFiles.Text = "Delete Label Files";
            this.checkBoxLabelDeleteFiles.UseVisualStyleBackColor = true;
            // 
            // buttonLabelStart
            // 
            this.buttonLabelStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLabelStart.Location = new System.Drawing.Point(420, 435);
            this.buttonLabelStart.Name = "buttonLabelStart";
            this.buttonLabelStart.Size = new System.Drawing.Size(78, 26);
            this.buttonLabelStart.TabIndex = 8;
            this.buttonLabelStart.Text = "&Start";
            this.buttonLabelStart.UseVisualStyleBackColor = true;
            this.buttonLabelStart.Click += new System.EventHandler(this.buttonLabelStart_Click);
            // 
            // checkBoxLabelClearInternal
            // 
            this.checkBoxLabelClearInternal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxLabelClearInternal.AutoSize = true;
            this.checkBoxLabelClearInternal.Location = new System.Drawing.Point(6, 440);
            this.checkBoxLabelClearInternal.Name = "checkBoxLabelClearInternal";
            this.checkBoxLabelClearInternal.Size = new System.Drawing.Size(132, 19);
            this.checkBoxLabelClearInternal.TabIndex = 6;
            this.checkBoxLabelClearInternal.Text = "Erase Internal Labels";
            this.checkBoxLabelClearInternal.UseVisualStyleBackColor = true;
            // 
            // radioButtonLabelImport
            // 
            this.radioButtonLabelImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonLabelImport.AutoSize = true;
            this.radioButtonLabelImport.Location = new System.Drawing.Point(156, 412);
            this.radioButtonLabelImport.Name = "radioButtonLabelImport";
            this.radioButtonLabelImport.Size = new System.Drawing.Size(97, 19);
            this.radioButtonLabelImport.TabIndex = 4;
            this.radioButtonLabelImport.Text = "Import Labels";
            this.radioButtonLabelImport.UseVisualStyleBackColor = true;
            this.radioButtonLabelImport.CheckedChanged += new System.EventHandler(this.radioButtonLabelImport_CheckedChanged);
            // 
            // radioButtonLabelExport
            // 
            this.radioButtonLabelExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonLabelExport.AutoSize = true;
            this.radioButtonLabelExport.Checked = true;
            this.radioButtonLabelExport.Location = new System.Drawing.Point(6, 412);
            this.radioButtonLabelExport.Name = "radioButtonLabelExport";
            this.radioButtonLabelExport.Size = new System.Drawing.Size(95, 19);
            this.radioButtonLabelExport.TabIndex = 5;
            this.radioButtonLabelExport.TabStop = true;
            this.radioButtonLabelExport.Text = "Export Labels";
            this.radioButtonLabelExport.UseVisualStyleBackColor = true;
            this.radioButtonLabelExport.CheckedChanged += new System.EventHandler(this.radioButtonLabelExport_CheckedChanged);
            // 
            // buttonLabelClear
            // 
            this.buttonLabelClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLabelClear.Location = new System.Drawing.Point(415, 69);
            this.buttonLabelClear.Name = "buttonLabelClear";
            this.buttonLabelClear.Size = new System.Drawing.Size(88, 25);
            this.buttonLabelClear.TabIndex = 3;
            this.buttonLabelClear.Text = "Clear All";
            this.buttonLabelClear.UseVisualStyleBackColor = true;
            this.buttonLabelClear.Click += new System.EventHandler(this.buttonLabelClear_Click);
            // 
            // buttonLabelRemove
            // 
            this.buttonLabelRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLabelRemove.Location = new System.Drawing.Point(415, 36);
            this.buttonLabelRemove.Name = "buttonLabelRemove";
            this.buttonLabelRemove.Size = new System.Drawing.Size(88, 25);
            this.buttonLabelRemove.TabIndex = 2;
            this.buttonLabelRemove.Text = "&Remove";
            this.buttonLabelRemove.UseVisualStyleBackColor = true;
            this.buttonLabelRemove.Click += new System.EventHandler(this.buttonLabelRemove_Click);
            // 
            // buttonLabelAdd
            // 
            this.buttonLabelAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLabelAdd.Location = new System.Drawing.Point(415, 5);
            this.buttonLabelAdd.Name = "buttonLabelAdd";
            this.buttonLabelAdd.Size = new System.Drawing.Size(88, 25);
            this.buttonLabelAdd.TabIndex = 1;
            this.buttonLabelAdd.Text = "&Add...";
            this.buttonLabelAdd.UseVisualStyleBackColor = true;
            this.buttonLabelAdd.Click += new System.EventHandler(this.buttonLabelAdd_Click);
            // 
            // listBoxLabelTool
            // 
            this.listBoxLabelTool.AllowDrop = true;
            this.listBoxLabelTool.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxLabelTool.FormattingEnabled = true;
            this.listBoxLabelTool.ItemHeight = 15;
            this.listBoxLabelTool.Location = new System.Drawing.Point(2, 4);
            this.listBoxLabelTool.Name = "listBoxLabelTool";
            this.listBoxLabelTool.Size = new System.Drawing.Size(407, 394);
            this.listBoxLabelTool.TabIndex = 0;
            this.listBoxLabelTool.SelectedIndexChanged += new System.EventHandler(this.listBoxLabelTool_SelectedIndexChanged);
            this.listBoxLabelTool.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxLabelTool_DragDrop);
            this.listBoxLabelTool.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxLabelTool_DragEnter);
            // 
            // statusStripTabDescription
            // 
            this.statusStripTabDescription.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelTabDesc});
            this.statusStripTabDescription.Location = new System.Drawing.Point(0, 498);
            this.statusStripTabDescription.Name = "statusStripTabDescription";
            this.statusStripTabDescription.Size = new System.Drawing.Size(515, 22);
            this.statusStripTabDescription.TabIndex = 41;
            this.statusStripTabDescription.Text = "statusStrip2";
            // 
            // toolStripStatusLabelTabDesc
            // 
            this.toolStripStatusLabelTabDesc.Name = "toolStripStatusLabelTabDesc";
            this.toolStripStatusLabelTabDesc.Size = new System.Drawing.Size(87, 17);
            this.toolStripStatusLabelTabDesc.Text = "Tab description";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 520);
            this.Controls.Add(this.statusStripTabDescription);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(531, 559);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Toolbox";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxBinary.ResumeLayout(false);
            this.groupBoxBinary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBinaryAddress)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageBinaryData.ResumeLayout(false);
            this.tabPageBinaryData.PerformLayout();
            this.tabPageStructConverter.ResumeLayout(false);
            this.tabPageStructConverter.PerformLayout();
            this.tabPageSplit.ResumeLayout(false);
            this.tabPageSplit.PerformLayout();
            this.tabPageSplitMDL.ResumeLayout(false);
            this.tabPageSplitMDL.PerformLayout();
            this.tabPageScanner.ResumeLayout(false);
            this.tabPageScanner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOffset)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanBinaryKey)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAddr)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndAddr)).EndInit();
            this.tabPageLabelTool.ResumeLayout(false);
            this.tabPageLabelTool.PerformLayout();
            this.statusStripTabDescription.ResumeLayout(false);
            this.statusStripTabDescription.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        internal System.Windows.Forms.NumericUpDown numericUpDownBinaryKey;
        internal System.Windows.Forms.NumericUpDown numericUpDownBinaryAddress;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageBinaryData;
		private System.Windows.Forms.TabPage tabPageStructConverter;
		private System.Windows.Forms.Button buttonStructConvConvertBatch;
		private System.Windows.Forms.CheckBox checkBoxStructConvSameOutputFolderBatch;
		private System.Windows.Forms.Button buttonStructConvAddBatch;
		private System.Windows.Forms.ListBox listBoxStructConverter;
		private System.Windows.Forms.TabPage tabPageSplit;
		private System.Windows.Forms.ComboBox comboBoxSplitGameSelect;
		private System.Windows.Forms.Button buttonSplitStart;
		private System.Windows.Forms.CheckBox checkBoxSameFolderSplit;
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
		internal System.Windows.Forms.NumericUpDown numericUpDownBinaryOffset;
		private System.Windows.Forms.CheckBox checkBoxStructConvJSON;
		private System.Windows.Forms.CheckBox checkBoxStructConvStructs;
		private System.Windows.Forms.TabPage tabPageSplitMDL;
		internal System.Windows.Forms.Label label8;
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
		private System.Windows.Forms.ComboBox comboBoxLabels;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TabPage tabPageScanner;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.NumericUpDown numericUpDownNodes;
		private System.Windows.Forms.Label labelNodes;
		private System.Windows.Forms.Button buttonFindBrowse;
		private System.Windows.Forms.CheckBox checkBoxSearchAction;
		private System.Windows.Forms.TextBox textBoxFindModel;
		private System.Windows.Forms.CheckBox checkBoxSearchLandtables;
		private System.Windows.Forms.CheckBox checkBoxSearchBasic;
		private System.Windows.Forms.RadioButton radioButtonFindModel;
		private System.Windows.Forms.CheckBox checkBoxSearchMotion;
		private System.Windows.Forms.RadioButton radioButtonSearchData;
		private System.Windows.Forms.CheckBox checkBoxSearchChunk;
		private System.Windows.Forms.CheckBox checkBoxSearchGinja;
		private System.Windows.Forms.Button buttonScanStart;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox checkBoxKeepLevel;
		private System.Windows.Forms.Button buttonOutputBrowse;
		private System.Windows.Forms.CheckBox checkBoxKeepChild;
		private System.Windows.Forms.CheckBox checkBoxSkipMeta;
		private System.Windows.Forms.CheckBox checkBoxSingleOutput;
		private System.Windows.Forms.TextBox textBoxOutputFolder;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown numericUpDownOffset;
		private System.Windows.Forms.Button buttonInputBrowse;
		private System.Windows.Forms.ListBox listBoxBaseGame;
		private System.Windows.Forms.NumericUpDown numericUpDownScanBinaryKey;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.NumericUpDown numericUpDownStartAddr;
		private System.Windows.Forms.TextBox textBoxInputFile;
		private System.Windows.Forms.NumericUpDown numericUpDownEndAddr;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxSimpleScan;
		private System.Windows.Forms.CheckBox checkBoxShortRot;
		private System.Windows.Forms.CheckBox checkBoxBasicSADX;
		private System.Windows.Forms.CheckBox checkBoxBigEndian;
		private System.Windows.Forms.StatusStrip statusStripTabDescription;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTabDesc;
        private System.Windows.Forms.TabPage tabPageLabelTool;
        private System.Windows.Forms.CheckBox checkBoxLabelClearInternal;
        private System.Windows.Forms.RadioButton radioButtonLabelImport;
        private System.Windows.Forms.RadioButton radioButtonLabelExport;
        private System.Windows.Forms.Button buttonLabelClear;
        private System.Windows.Forms.Button buttonLabelRemove;
        private System.Windows.Forms.Button buttonLabelAdd;
        private System.Windows.Forms.ListBox listBoxLabelTool;
        private System.Windows.Forms.Button buttonLabelStart;
        private System.Windows.Forms.CheckBox checkBoxLabelDeleteFiles;
    }
}

