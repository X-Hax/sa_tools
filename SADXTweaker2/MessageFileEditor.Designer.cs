namespace SADXTweaker2
{
    partial class MessageFileEditor
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.GroupBox groupBox2;
			System.Windows.Forms.GroupBox groupBox3;
			System.Windows.Forms.GroupBox groupBox4;
			System.Windows.Forms.Label label6;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageFileEditor));
			this.eventNotSet = new System.Windows.Forms.CheckBox();
			this.eventFlag = new System.Windows.Forms.NumericUpDown();
			this.eventRemoveButton = new System.Windows.Forms.Button();
			this.eventAddButton = new System.Windows.Forms.Button();
			this.eventFlagNum = new System.Windows.Forms.ComboBox();
			this.npcNotSet = new System.Windows.Forms.CheckBox();
			this.npcFlag = new System.Windows.Forms.NumericUpDown();
			this.npcFlagRemoveButton = new System.Windows.Forms.Button();
			this.npcFlagAddButton = new System.Windows.Forms.Button();
			this.npcFlagNum = new System.Windows.Forms.ComboBox();
			this.big = new System.Windows.Forms.CheckBox();
			this.gamma = new System.Windows.Forms.CheckBox();
			this.amy = new System.Windows.Forms.CheckBox();
			this.tikal = new System.Windows.Forms.CheckBox();
			this.knuckles = new System.Windows.Forms.CheckBox();
			this.tails = new System.Windows.Forms.CheckBox();
			this.eggman = new System.Windows.Forms.CheckBox();
			this.sonic = new System.Windows.Forms.CheckBox();
			this.line = new System.Windows.Forms.RichTextBox();
			this.lineRemoveButton = new System.Windows.Forms.Button();
			this.lineAddButton = new System.Windows.Forms.Button();
			this.lineNum = new System.Windows.Forms.ComboBox();
			this.field = new System.Windows.Forms.ComboBox();
			this.character = new System.Windows.Forms.ComboBox();
			this.language = new System.Windows.Forms.ComboBox();
			this.loadButton = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.setEventUnset = new System.Windows.Forms.CheckBox();
			this.setEventNum = new System.Windows.Forms.NumericUpDown();
			this.setEventFlag = new System.Windows.Forms.CheckBox();
			this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
			this.voice = new System.Windows.Forms.CheckBox();
			this.groupRemoveButton = new System.Windows.Forms.Button();
			this.groupAddButton = new System.Windows.Forms.Button();
			this.groupNum = new System.Windows.Forms.ComboBox();
			this.npcID = new System.Windows.Forms.ComboBox();
			this.npcRemoveButton = new System.Windows.Forms.Button();
			this.npcAddButton = new System.Windows.Forms.Button();
			this.searchButton = new System.Windows.Forms.Button();
			this.voiceNum = new SADXTweaker2.FileListControl();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			groupBox3 = new System.Windows.Forms.GroupBox();
			groupBox4 = new System.Windows.Forms.GroupBox();
			label6 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.eventFlag)).BeginInit();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.npcFlag)).BeginInit();
			groupBox3.SuspendLayout();
			groupBox4.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.setEventNum)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 15);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(32, 13);
			label1.TabIndex = 0;
			label1.Text = "Field:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 42);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(56, 13);
			label2.TabIndex = 1;
			label2.Text = "Character:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(12, 69);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(58, 13);
			label3.TabIndex = 2;
			label3.Text = "Language:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(12, 125);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(46, 13);
			label4.TabIndex = 8;
			label4.Text = "NPC ID:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(12, 8);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(39, 13);
			label5.TabIndex = 0;
			label5.Text = "Group:";
			// 
			// groupBox1
			// 
			groupBox1.AutoSize = true;
			groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox1.Controls.Add(this.eventNotSet);
			groupBox1.Controls.Add(this.eventFlag);
			groupBox1.Controls.Add(this.eventRemoveButton);
			groupBox1.Controls.Add(this.eventAddButton);
			groupBox1.Controls.Add(this.eventFlagNum);
			groupBox1.Location = new System.Drawing.Point(12, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			groupBox1.Size = new System.Drawing.Size(238, 82);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Event Flags";
			// 
			// eventNotSet
			// 
			this.eventNotSet.AutoSize = true;
			this.eventNotSet.Enabled = false;
			this.eventNotSet.Location = new System.Drawing.Point(62, 47);
			this.eventNotSet.Name = "eventNotSet";
			this.eventNotSet.Size = new System.Drawing.Size(68, 17);
			this.eventNotSet.TabIndex = 9;
			this.eventNotSet.Text = "Not Set?";
			this.eventNotSet.UseVisualStyleBackColor = true;
			this.eventNotSet.CheckedChanged += new System.EventHandler(this.eventFlag_ValueChanged);
			// 
			// eventFlag
			// 
			this.eventFlag.Enabled = false;
			this.eventFlag.Hexadecimal = true;
			this.eventFlag.Location = new System.Drawing.Point(6, 46);
			this.eventFlag.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
			this.eventFlag.Name = "eventFlag";
			this.eventFlag.Size = new System.Drawing.Size(50, 20);
			this.eventFlag.TabIndex = 8;
			this.eventFlag.ValueChanged += new System.EventHandler(this.eventFlag_ValueChanged);
			// 
			// eventRemoveButton
			// 
			this.eventRemoveButton.AutoSize = true;
			this.eventRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.eventRemoveButton.Enabled = false;
			this.eventRemoveButton.Location = new System.Drawing.Point(175, 17);
			this.eventRemoveButton.Name = "eventRemoveButton";
			this.eventRemoveButton.Size = new System.Drawing.Size(57, 23);
			this.eventRemoveButton.TabIndex = 7;
			this.eventRemoveButton.Text = "Remove";
			this.eventRemoveButton.UseVisualStyleBackColor = true;
			this.eventRemoveButton.Click += new System.EventHandler(this.eventRemoveButton_Click);
			// 
			// eventAddButton
			// 
			this.eventAddButton.AutoSize = true;
			this.eventAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.eventAddButton.Location = new System.Drawing.Point(133, 17);
			this.eventAddButton.Name = "eventAddButton";
			this.eventAddButton.Size = new System.Drawing.Size(36, 23);
			this.eventAddButton.TabIndex = 6;
			this.eventAddButton.Text = "Add";
			this.eventAddButton.UseVisualStyleBackColor = true;
			this.eventAddButton.Click += new System.EventHandler(this.eventAddButton_Click);
			// 
			// eventFlagNum
			// 
			this.eventFlagNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.eventFlagNum.FormattingEnabled = true;
			this.eventFlagNum.Location = new System.Drawing.Point(6, 19);
			this.eventFlagNum.Name = "eventFlagNum";
			this.eventFlagNum.Size = new System.Drawing.Size(121, 21);
			this.eventFlagNum.TabIndex = 5;
			this.eventFlagNum.SelectedIndexChanged += new System.EventHandler(this.eventFlagNum_SelectedIndexChanged);
			// 
			// groupBox2
			// 
			groupBox2.AutoSize = true;
			groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox2.Controls.Add(this.npcNotSet);
			groupBox2.Controls.Add(this.npcFlag);
			groupBox2.Controls.Add(this.npcFlagRemoveButton);
			groupBox2.Controls.Add(this.npcFlagAddButton);
			groupBox2.Controls.Add(this.npcFlagNum);
			groupBox2.Location = new System.Drawing.Point(12, 91);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			groupBox2.Size = new System.Drawing.Size(238, 82);
			groupBox2.TabIndex = 1;
			groupBox2.TabStop = false;
			groupBox2.Text = "NPC Flags";
			// 
			// npcNotSet
			// 
			this.npcNotSet.AutoSize = true;
			this.npcNotSet.Enabled = false;
			this.npcNotSet.Location = new System.Drawing.Point(62, 47);
			this.npcNotSet.Name = "npcNotSet";
			this.npcNotSet.Size = new System.Drawing.Size(68, 17);
			this.npcNotSet.TabIndex = 9;
			this.npcNotSet.Text = "Not Set?";
			this.npcNotSet.UseVisualStyleBackColor = true;
			this.npcNotSet.CheckedChanged += new System.EventHandler(this.npcFlag_ValueChanged);
			// 
			// npcFlag
			// 
			this.npcFlag.Enabled = false;
			this.npcFlag.Hexadecimal = true;
			this.npcFlag.Location = new System.Drawing.Point(6, 46);
			this.npcFlag.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
			this.npcFlag.Name = "npcFlag";
			this.npcFlag.Size = new System.Drawing.Size(50, 20);
			this.npcFlag.TabIndex = 8;
			this.npcFlag.ValueChanged += new System.EventHandler(this.npcFlag_ValueChanged);
			// 
			// npcFlagRemoveButton
			// 
			this.npcFlagRemoveButton.AutoSize = true;
			this.npcFlagRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.npcFlagRemoveButton.Enabled = false;
			this.npcFlagRemoveButton.Location = new System.Drawing.Point(175, 17);
			this.npcFlagRemoveButton.Name = "npcFlagRemoveButton";
			this.npcFlagRemoveButton.Size = new System.Drawing.Size(57, 23);
			this.npcFlagRemoveButton.TabIndex = 7;
			this.npcFlagRemoveButton.Text = "Remove";
			this.npcFlagRemoveButton.UseVisualStyleBackColor = true;
			this.npcFlagRemoveButton.Click += new System.EventHandler(this.npcFlagRemoveButton_Click);
			// 
			// npcFlagAddButton
			// 
			this.npcFlagAddButton.AutoSize = true;
			this.npcFlagAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.npcFlagAddButton.Location = new System.Drawing.Point(133, 17);
			this.npcFlagAddButton.Name = "npcFlagAddButton";
			this.npcFlagAddButton.Size = new System.Drawing.Size(36, 23);
			this.npcFlagAddButton.TabIndex = 6;
			this.npcFlagAddButton.Text = "Add";
			this.npcFlagAddButton.UseVisualStyleBackColor = true;
			this.npcFlagAddButton.Click += new System.EventHandler(this.npcFlagAddButton_Click);
			// 
			// npcFlagNum
			// 
			this.npcFlagNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.npcFlagNum.FormattingEnabled = true;
			this.npcFlagNum.Location = new System.Drawing.Point(6, 19);
			this.npcFlagNum.Name = "npcFlagNum";
			this.npcFlagNum.Size = new System.Drawing.Size(121, 21);
			this.npcFlagNum.TabIndex = 5;
			this.npcFlagNum.SelectedIndexChanged += new System.EventHandler(this.npcFlagNum_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			groupBox3.AutoSize = true;
			groupBox3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox3.Controls.Add(this.big);
			groupBox3.Controls.Add(this.gamma);
			groupBox3.Controls.Add(this.amy);
			groupBox3.Controls.Add(this.tikal);
			groupBox3.Controls.Add(this.knuckles);
			groupBox3.Controls.Add(this.tails);
			groupBox3.Controls.Add(this.eggman);
			groupBox3.Controls.Add(this.sonic);
			groupBox3.Location = new System.Drawing.Point(12, 179);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			groupBox3.Size = new System.Drawing.Size(266, 75);
			groupBox3.TabIndex = 2;
			groupBox3.TabStop = false;
			groupBox3.Text = "Characters";
			// 
			// big
			// 
			this.big.AutoSize = true;
			this.big.Checked = true;
			this.big.CheckState = System.Windows.Forms.CheckState.Checked;
			this.big.Location = new System.Drawing.Point(181, 42);
			this.big.Name = "big";
			this.big.Size = new System.Drawing.Size(41, 17);
			this.big.TabIndex = 7;
			this.big.Text = "Big";
			this.big.UseVisualStyleBackColor = true;
			this.big.CheckedChanged += new System.EventHandler(this.character_CheckedChanged);
			// 
			// gamma
			// 
			this.gamma.AutoSize = true;
			this.gamma.Checked = true;
			this.gamma.CheckState = System.Windows.Forms.CheckState.Checked;
			this.gamma.Location = new System.Drawing.Point(113, 42);
			this.gamma.Name = "gamma";
			this.gamma.Size = new System.Drawing.Size(62, 17);
			this.gamma.TabIndex = 6;
			this.gamma.Text = "Gamma";
			this.gamma.UseVisualStyleBackColor = true;
			this.gamma.CheckedChanged += new System.EventHandler(this.character_CheckedChanged);
			// 
			// amy
			// 
			this.amy.AutoSize = true;
			this.amy.Checked = true;
			this.amy.CheckState = System.Windows.Forms.CheckState.Checked;
			this.amy.Location = new System.Drawing.Point(61, 42);
			this.amy.Name = "amy";
			this.amy.Size = new System.Drawing.Size(46, 17);
			this.amy.TabIndex = 5;
			this.amy.Text = "Amy";
			this.amy.UseVisualStyleBackColor = true;
			this.amy.CheckedChanged += new System.EventHandler(this.character_CheckedChanged);
			// 
			// tikal
			// 
			this.tikal.AutoSize = true;
			this.tikal.Checked = true;
			this.tikal.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tikal.Location = new System.Drawing.Point(6, 42);
			this.tikal.Name = "tikal";
			this.tikal.Size = new System.Drawing.Size(49, 17);
			this.tikal.TabIndex = 4;
			this.tikal.Text = "Tikal";
			this.tikal.UseVisualStyleBackColor = true;
			this.tikal.CheckedChanged += new System.EventHandler(this.character_CheckedChanged);
			// 
			// knuckles
			// 
			this.knuckles.AutoSize = true;
			this.knuckles.Checked = true;
			this.knuckles.CheckState = System.Windows.Forms.CheckState.Checked;
			this.knuckles.Location = new System.Drawing.Point(190, 19);
			this.knuckles.Name = "knuckles";
			this.knuckles.Size = new System.Drawing.Size(70, 17);
			this.knuckles.TabIndex = 3;
			this.knuckles.Text = "Knuckles";
			this.knuckles.UseVisualStyleBackColor = true;
			this.knuckles.CheckedChanged += new System.EventHandler(this.character_CheckedChanged);
			// 
			// tails
			// 
			this.tails.AutoSize = true;
			this.tails.Checked = true;
			this.tails.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tails.Location = new System.Drawing.Point(136, 19);
			this.tails.Name = "tails";
			this.tails.Size = new System.Drawing.Size(48, 17);
			this.tails.TabIndex = 2;
			this.tails.Text = "Tails";
			this.tails.UseVisualStyleBackColor = true;
			this.tails.CheckedChanged += new System.EventHandler(this.character_CheckedChanged);
			// 
			// eggman
			// 
			this.eggman.AutoSize = true;
			this.eggman.Checked = true;
			this.eggman.CheckState = System.Windows.Forms.CheckState.Checked;
			this.eggman.Location = new System.Drawing.Point(65, 19);
			this.eggman.Name = "eggman";
			this.eggman.Size = new System.Drawing.Size(65, 17);
			this.eggman.TabIndex = 1;
			this.eggman.Text = "Eggman";
			this.eggman.UseVisualStyleBackColor = true;
			this.eggman.CheckedChanged += new System.EventHandler(this.character_CheckedChanged);
			// 
			// sonic
			// 
			this.sonic.AutoSize = true;
			this.sonic.Checked = true;
			this.sonic.CheckState = System.Windows.Forms.CheckState.Checked;
			this.sonic.Location = new System.Drawing.Point(6, 19);
			this.sonic.Name = "sonic";
			this.sonic.Size = new System.Drawing.Size(53, 17);
			this.sonic.TabIndex = 0;
			this.sonic.Text = "Sonic";
			this.sonic.UseVisualStyleBackColor = true;
			this.sonic.CheckedChanged += new System.EventHandler(this.character_CheckedChanged);
			// 
			// groupBox4
			// 
			groupBox4.AutoSize = true;
			groupBox4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox4.Controls.Add(this.line);
			groupBox4.Controls.Add(this.lineRemoveButton);
			groupBox4.Controls.Add(this.lineAddButton);
			groupBox4.Controls.Add(this.lineNum);
			groupBox4.Controls.Add(label6);
			groupBox4.Location = new System.Drawing.Point(12, 323);
			groupBox4.Name = "groupBox4";
			groupBox4.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			groupBox4.Size = new System.Drawing.Size(321, 121);
			groupBox4.TabIndex = 12;
			groupBox4.TabStop = false;
			groupBox4.Text = "Text";
			// 
			// line
			// 
			this.line.AcceptsTab = true;
			this.line.DetectUrls = false;
			this.line.Enabled = false;
			this.line.Location = new System.Drawing.Point(6, 46);
			this.line.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.line.Name = "line";
			this.line.Size = new System.Drawing.Size(309, 62);
			this.line.TabIndex = 11;
			this.line.Text = "";
			this.line.TextChanged += new System.EventHandler(this.line_TextChanged);
			// 
			// lineRemoveButton
			// 
			this.lineRemoveButton.AutoSize = true;
			this.lineRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.lineRemoveButton.Enabled = false;
			this.lineRemoveButton.Location = new System.Drawing.Point(211, 17);
			this.lineRemoveButton.Name = "lineRemoveButton";
			this.lineRemoveButton.Size = new System.Drawing.Size(57, 23);
			this.lineRemoveButton.TabIndex = 10;
			this.lineRemoveButton.Text = "Remove";
			this.lineRemoveButton.UseVisualStyleBackColor = true;
			this.lineRemoveButton.Click += new System.EventHandler(this.lineRemoveButton_Click);
			// 
			// lineAddButton
			// 
			this.lineAddButton.AutoSize = true;
			this.lineAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.lineAddButton.Location = new System.Drawing.Point(169, 17);
			this.lineAddButton.Name = "lineAddButton";
			this.lineAddButton.Size = new System.Drawing.Size(36, 23);
			this.lineAddButton.TabIndex = 9;
			this.lineAddButton.Text = "Add";
			this.lineAddButton.UseVisualStyleBackColor = true;
			this.lineAddButton.Click += new System.EventHandler(this.lineAddButton_Click);
			// 
			// lineNum
			// 
			this.lineNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.lineNum.FormattingEnabled = true;
			this.lineNum.Location = new System.Drawing.Point(42, 19);
			this.lineNum.Name = "lineNum";
			this.lineNum.Size = new System.Drawing.Size(121, 21);
			this.lineNum.TabIndex = 8;
			this.lineNum.SelectedIndexChanged += new System.EventHandler(this.lineNum_SelectedIndexChanged);
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(6, 22);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(30, 13);
			label6.TabIndex = 0;
			label6.Text = "Line:";
			// 
			// field
			// 
			this.field.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.field.FormattingEnabled = true;
			this.field.Items.AddRange(new object[] {
            "Station Square",
            "Mystic Ruins",
            "The Past"});
			this.field.Location = new System.Drawing.Point(50, 12);
			this.field.Name = "field";
			this.field.Size = new System.Drawing.Size(121, 21);
			this.field.TabIndex = 3;
			this.field.SelectedIndexChanged += new System.EventHandler(this.field_SelectedIndexChanged);
			// 
			// character
			// 
			this.character.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.character.FormattingEnabled = true;
			this.character.Items.AddRange(new object[] {
            "Sonic",
            "Tails",
            "Knuckles",
            "Amy",
            "Gamma",
            "Big",
            "Last"});
			this.character.Location = new System.Drawing.Point(74, 39);
			this.character.Name = "character";
			this.character.Size = new System.Drawing.Size(121, 21);
			this.character.TabIndex = 4;
			this.character.SelectedIndexChanged += new System.EventHandler(this.field_SelectedIndexChanged);
			// 
			// language
			// 
			this.language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.language.FormattingEnabled = true;
			this.language.Items.AddRange(new object[] {
            "Japanese",
            "English",
            "French",
            "Spanish",
            "German"});
			this.language.Location = new System.Drawing.Point(76, 66);
			this.language.Name = "language";
			this.language.Size = new System.Drawing.Size(121, 21);
			this.language.TabIndex = 5;
			this.language.SelectedIndexChanged += new System.EventHandler(this.field_SelectedIndexChanged);
			// 
			// loadButton
			// 
			this.loadButton.Enabled = false;
			this.loadButton.Location = new System.Drawing.Point(12, 93);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(75, 23);
			this.loadButton.TabIndex = 6;
			this.loadButton.Text = "Load";
			this.loadButton.UseVisualStyleBackColor = true;
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(93, 93);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(75, 23);
			this.saveButton.TabIndex = 7;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.Controls.Add(this.panel3);
			this.panel2.Controls.Add(this.groupRemoveButton);
			this.panel2.Controls.Add(this.groupAddButton);
			this.panel2.Controls.Add(this.groupNum);
			this.panel2.Controls.Add(label5);
			this.panel2.Enabled = false;
			this.panel2.Location = new System.Drawing.Point(0, 149);
			this.panel2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(403, 487);
			this.panel2.TabIndex = 10;
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.AutoScroll = true;
			this.panel3.Controls.Add(groupBox4);
			this.panel3.Controls.Add(this.setEventUnset);
			this.panel3.Controls.Add(this.setEventNum);
			this.panel3.Controls.Add(this.setEventFlag);
			this.panel3.Controls.Add(this.axWindowsMediaPlayer1);
			this.panel3.Controls.Add(this.voiceNum);
			this.panel3.Controls.Add(this.voice);
			this.panel3.Controls.Add(groupBox3);
			this.panel3.Controls.Add(groupBox2);
			this.panel3.Controls.Add(groupBox1);
			this.panel3.Enabled = false;
			this.panel3.Location = new System.Drawing.Point(0, 32);
			this.panel3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(403, 455);
			this.panel3.TabIndex = 4;
			// 
			// setEventUnset
			// 
			this.setEventUnset.AutoSize = true;
			this.setEventUnset.Enabled = false;
			this.setEventUnset.Location = new System.Drawing.Point(170, 298);
			this.setEventUnset.Name = "setEventUnset";
			this.setEventUnset.Size = new System.Drawing.Size(54, 17);
			this.setEventUnset.TabIndex = 11;
			this.setEventUnset.Text = "Unset";
			this.setEventUnset.UseVisualStyleBackColor = true;
			this.setEventUnset.CheckedChanged += new System.EventHandler(this.setEventNum_ValueChanged);
			// 
			// setEventNum
			// 
			this.setEventNum.Enabled = false;
			this.setEventNum.Hexadecimal = true;
			this.setEventNum.Location = new System.Drawing.Point(114, 297);
			this.setEventNum.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
			this.setEventNum.Name = "setEventNum";
			this.setEventNum.Size = new System.Drawing.Size(50, 20);
			this.setEventNum.TabIndex = 10;
			this.setEventNum.ValueChanged += new System.EventHandler(this.setEventNum_ValueChanged);
			// 
			// setEventFlag
			// 
			this.setEventFlag.AutoSize = true;
			this.setEventFlag.Location = new System.Drawing.Point(12, 298);
			this.setEventFlag.Name = "setEventFlag";
			this.setEventFlag.Size = new System.Drawing.Size(96, 17);
			this.setEventFlag.TabIndex = 7;
			this.setEventFlag.Text = "Set Event Flag";
			this.setEventFlag.UseVisualStyleBackColor = true;
			this.setEventFlag.CheckedChanged += new System.EventHandler(this.setEventFlag_CheckedChanged);
			// 
			// axWindowsMediaPlayer1
			// 
			this.axWindowsMediaPlayer1.Enabled = true;
			this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(178, 260);
			this.axWindowsMediaPlayer1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
			this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
			this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(218, 34);
			this.axWindowsMediaPlayer1.TabIndex = 6;
			// 
			// voice
			// 
			this.voice.AutoSize = true;
			this.voice.Location = new System.Drawing.Point(12, 265);
			this.voice.Name = "voice";
			this.voice.Size = new System.Drawing.Size(76, 17);
			this.voice.TabIndex = 3;
			this.voice.Text = "Play Voice";
			this.voice.UseVisualStyleBackColor = true;
			this.voice.CheckedChanged += new System.EventHandler(this.voice_CheckedChanged);
			// 
			// groupRemoveButton
			// 
			this.groupRemoveButton.AutoSize = true;
			this.groupRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupRemoveButton.Enabled = false;
			this.groupRemoveButton.Location = new System.Drawing.Point(226, 3);
			this.groupRemoveButton.Name = "groupRemoveButton";
			this.groupRemoveButton.Size = new System.Drawing.Size(57, 23);
			this.groupRemoveButton.TabIndex = 3;
			this.groupRemoveButton.Text = "Remove";
			this.groupRemoveButton.UseVisualStyleBackColor = true;
			this.groupRemoveButton.Click += new System.EventHandler(this.groupRemoveButton_Click);
			// 
			// groupAddButton
			// 
			this.groupAddButton.AutoSize = true;
			this.groupAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupAddButton.Location = new System.Drawing.Point(184, 3);
			this.groupAddButton.Name = "groupAddButton";
			this.groupAddButton.Size = new System.Drawing.Size(36, 23);
			this.groupAddButton.TabIndex = 2;
			this.groupAddButton.Text = "Add";
			this.groupAddButton.UseVisualStyleBackColor = true;
			this.groupAddButton.Click += new System.EventHandler(this.groupAddButton_Click);
			// 
			// groupNum
			// 
			this.groupNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.groupNum.FormattingEnabled = true;
			this.groupNum.Location = new System.Drawing.Point(57, 5);
			this.groupNum.Name = "groupNum";
			this.groupNum.Size = new System.Drawing.Size(121, 21);
			this.groupNum.TabIndex = 1;
			this.groupNum.SelectedIndexChanged += new System.EventHandler(this.groupNum_SelectedIndexChanged);
			// 
			// npcID
			// 
			this.npcID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.npcID.FormattingEnabled = true;
			this.npcID.Location = new System.Drawing.Point(64, 122);
			this.npcID.Name = "npcID";
			this.npcID.Size = new System.Drawing.Size(121, 21);
			this.npcID.TabIndex = 9;
			this.npcID.SelectedIndexChanged += new System.EventHandler(this.npcID_SelectedIndexChanged);
			// 
			// npcRemoveButton
			// 
			this.npcRemoveButton.AutoSize = true;
			this.npcRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.npcRemoveButton.Enabled = false;
			this.npcRemoveButton.Location = new System.Drawing.Point(233, 120);
			this.npcRemoveButton.Name = "npcRemoveButton";
			this.npcRemoveButton.Size = new System.Drawing.Size(57, 23);
			this.npcRemoveButton.TabIndex = 12;
			this.npcRemoveButton.Text = "Remove";
			this.npcRemoveButton.UseVisualStyleBackColor = true;
			this.npcRemoveButton.Click += new System.EventHandler(this.npcRemoveButton_Click);
			// 
			// npcAddButton
			// 
			this.npcAddButton.AutoSize = true;
			this.npcAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.npcAddButton.Location = new System.Drawing.Point(191, 120);
			this.npcAddButton.Name = "npcAddButton";
			this.npcAddButton.Size = new System.Drawing.Size(36, 23);
			this.npcAddButton.TabIndex = 11;
			this.npcAddButton.Text = "Add";
			this.npcAddButton.UseVisualStyleBackColor = true;
			this.npcAddButton.Click += new System.EventHandler(this.npcAddButton_Click);
			// 
			// searchButton
			// 
			this.searchButton.AutoSize = true;
			this.searchButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.searchButton.Location = new System.Drawing.Point(174, 93);
			this.searchButton.Name = "searchButton";
			this.searchButton.Size = new System.Drawing.Size(61, 23);
			this.searchButton.TabIndex = 13;
			this.searchButton.Text = "Find Text";
			this.searchButton.UseVisualStyleBackColor = true;
			this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
			// 
			// voiceNum
			// 
			this.voiceNum.Directory = null;
			this.voiceNum.Enabled = false;
			this.voiceNum.Extension = "wma";
			this.voiceNum.Location = new System.Drawing.Point(94, 260);
			this.voiceNum.Name = "voiceNum";
			this.voiceNum.Size = new System.Drawing.Size(74, 23);
			this.voiceNum.TabIndex = 4;
			this.voiceNum.Title = "Voice File";
			this.voiceNum.Value = "0000";
			this.voiceNum.ValueChanged += new System.EventHandler(this.voiceNum_ValueChanged);
			// 
			// MessageFileEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(403, 636);
			this.Controls.Add(this.searchButton);
			this.Controls.Add(this.npcRemoveButton);
			this.Controls.Add(this.npcAddButton);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.npcID);
			this.Controls.Add(label4);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.loadButton);
			this.Controls.Add(this.language);
			this.Controls.Add(this.character);
			this.Controls.Add(this.field);
			this.Controls.Add(label3);
			this.Controls.Add(label2);
			this.Controls.Add(label1);
			this.Name = "MessageFileEditor";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Message File Editor";
			this.Load += new System.EventHandler(this.MessageFileEditor_Load);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.eventFlag)).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.npcFlag)).EndInit();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			groupBox4.ResumeLayout(false);
			groupBox4.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.setEventNum)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox field;
        private System.Windows.Forms.ComboBox character;
        private System.Windows.Forms.ComboBox language;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ComboBox npcID;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox groupNum;
        private System.Windows.Forms.Button groupAddButton;
        private System.Windows.Forms.Button groupRemoveButton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button eventRemoveButton;
        private System.Windows.Forms.Button eventAddButton;
        private System.Windows.Forms.ComboBox eventFlagNum;
        private System.Windows.Forms.NumericUpDown eventFlag;
        private System.Windows.Forms.CheckBox eventNotSet;
        private System.Windows.Forms.CheckBox npcNotSet;
        private System.Windows.Forms.NumericUpDown npcFlag;
        private System.Windows.Forms.Button npcFlagRemoveButton;
        private System.Windows.Forms.Button npcFlagAddButton;
        private System.Windows.Forms.ComboBox npcFlagNum;
        private System.Windows.Forms.CheckBox sonic;
        private System.Windows.Forms.CheckBox eggman;
        private System.Windows.Forms.CheckBox tails;
        private System.Windows.Forms.CheckBox knuckles;
        private System.Windows.Forms.CheckBox big;
        private System.Windows.Forms.CheckBox gamma;
        private System.Windows.Forms.CheckBox amy;
        private System.Windows.Forms.CheckBox tikal;
        private System.Windows.Forms.CheckBox voice;
        private FileListControl voiceNum;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.CheckBox setEventFlag;
        private System.Windows.Forms.CheckBox setEventUnset;
        private System.Windows.Forms.NumericUpDown setEventNum;
        private System.Windows.Forms.Button lineRemoveButton;
        private System.Windows.Forms.Button lineAddButton;
        private System.Windows.Forms.ComboBox lineNum;
        private System.Windows.Forms.RichTextBox line;
        private System.Windows.Forms.Button npcRemoveButton;
        private System.Windows.Forms.Button npcAddButton;
		private System.Windows.Forms.Button searchButton;
    }
}

