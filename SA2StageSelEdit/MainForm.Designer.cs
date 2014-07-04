namespace SA2StageSelEdit
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.row = new System.Windows.Forms.NumericUpDown();
			this.column = new System.Windows.Forms.NumericUpDown();
			this.character = new System.Windows.Forms.ComboBox();
			this.level = new System.Windows.Forms.ComboBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.dummyPanel = new System.Windows.Forms.Panel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.row)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.column)).BeginInit();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(6, 6);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(36, 13);
			label1.TabIndex = 4;
			label1.Text = "Level:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(6, 33);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(56, 13);
			label2.TabIndex = 5;
			label2.Text = "Character:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(6, 59);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(45, 13);
			label3.TabIndex = 6;
			label3.Text = "Column:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(6, 85);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(32, 13);
			label4.TabIndex = 7;
			label4.Text = "Row:";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(584, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(109, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(2048, 480);
			this.panel1.TabIndex = 0;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			this.panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
			this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.row);
			this.panel2.Controls.Add(label4);
			this.panel2.Controls.Add(label3);
			this.panel2.Controls.Add(this.column);
			this.panel2.Controls.Add(label2);
			this.panel2.Controls.Add(this.character);
			this.panel2.Controls.Add(this.level);
			this.panel2.Controls.Add(label1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Enabled = false;
			this.panel2.Location = new System.Drawing.Point(384, 24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(200, 538);
			this.panel2.TabIndex = 1;
			// 
			// row
			// 
			this.row.Location = new System.Drawing.Point(92, 83);
			this.row.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
			this.row.Name = "row";
			this.row.Size = new System.Drawing.Size(96, 20);
			this.row.TabIndex = 3;
			this.row.ValueChanged += new System.EventHandler(this.row_ValueChanged);
			// 
			// column
			// 
			this.column.Location = new System.Drawing.Point(92, 57);
			this.column.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
			this.column.Name = "column";
			this.column.Size = new System.Drawing.Size(96, 20);
			this.column.TabIndex = 2;
			this.column.ValueChanged += new System.EventHandler(this.column_ValueChanged);
			// 
			// character
			// 
			this.character.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.character.FormattingEnabled = true;
			this.character.Items.AddRange(new object[] {
            "Sonic",
            "Shadow",
            "Tails",
            "Eggman",
            "Knuckles",
            "Rouge",
            "Mech Tails",
            "Mech Eggman"});
			this.character.Location = new System.Drawing.Point(68, 30);
			this.character.Name = "character";
			this.character.Size = new System.Drawing.Size(120, 21);
			this.character.TabIndex = 1;
			this.character.SelectedIndexChanged += new System.EventHandler(this.character_SelectedIndexChanged);
			// 
			// level
			// 
			this.level.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.level.FormattingEnabled = true;
			this.level.Items.AddRange(new object[] {
            "Basic Test",
            "Knuckles Test",
            "Sonic Test",
            "Green Forest",
            "White Jungle",
            "Pumpkin Hill",
            "Sky Rail",
            "Aquatic Mine",
            "Security Hall",
            "Prison Lane",
            "Metal Harbor",
            "Iron Gate",
            "Weapons Bed",
            "City Escape",
            "Radical Highway",
            "Weapons Bed 2P",
            "Wild Canyon",
            "Mission Street",
            "Dry Lagoon",
            "Sonic Vs. Shadow 1",
            "Tails Vs. Eggman 1",
            "Sand Ocean",
            "Crazy Gadget",
            "Hidden Base",
            "Eternal Engine",
            "Death Chamber",
            "Egg Quarters",
            "Lost Colony",
            "Pyramid Cave",
            "Tails Vs. Eggman 2",
            "Final Rush",
            "Green Hill",
            "Meteor Herd",
            "Knuckles Vs. Rouge",
            "Cannon\'s Core (Sonic)",
            "Cannon\'s Core (Eggman)",
            "Cannon\'s Core (Tails)",
            "Cannon\'s Core (Rouge)",
            "Cannon\'s Core (Knuckles)",
            "Mission Street 2P",
            "Final Chase",
            "Wild Canyon 2P",
            "Sonic Vs. Shadow 2",
            "Cosmic Wall",
            "Mad Space",
            "Sand Ocean 2P",
            "Dry Lagoon 2P",
            "Pyramid Race",
            "Hidden Base 2P",
            "Pool Quest",
            "Planet Quest",
            "Deck Race",
            "Downtown Race",
            "Cosmic Wall 2P",
            "Grind Race",
            "Lost Colony 2P",
            "Eternal Engine 2P",
            "Metal Harbor 2P",
            "Iron Gate 2P",
            "Death Chamber 2P",
            "F-6t BIG FOOT",
            "B-3x HOT SHOT",
            "R-1/A FLYING DOG",
            "King Boom Boo",
            "Egg Golem (Sonic)",
            "Biolizard",
            "FinalHazard",
            "Egg Golem (Eggman)",
            "68",
            "69",
            "Route 101/Route 280",
            "Kart Race",
            "72",
            "73",
            "74",
            "75",
            "76",
            "77",
            "78",
            "79",
            "80",
            "81",
            "82",
            "83",
            "84",
            "85",
            "86",
            "87",
            "88",
            "89",
            "Chao World"});
			this.level.Location = new System.Drawing.Point(48, 3);
			this.level.Name = "level";
			this.level.Size = new System.Drawing.Size(140, 21);
			this.level.TabIndex = 0;
			this.level.SelectedIndexChanged += new System.EventHandler(this.level_SelectedIndexChanged);
			// 
			// panel3
			// 
			this.panel3.AutoScroll = true;
			this.panel3.Controls.Add(this.panel1);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 24);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(384, 538);
			this.panel3.TabIndex = 2;
			// 
			// dummyPanel
			// 
			this.dummyPanel.Enabled = false;
			this.dummyPanel.Location = new System.Drawing.Point(0, 0);
			this.dummyPanel.Name = "dummyPanel";
			this.dummyPanel.Size = new System.Drawing.Size(200, 100);
			this.dummyPanel.TabIndex = 0;
			this.dummyPanel.Visible = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 562);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.dummyPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "SA2 Stage Select Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.row)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.column)).EndInit();
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox level;
        private System.Windows.Forms.ComboBox character;
        private System.Windows.Forms.NumericUpDown column;
        private System.Windows.Forms.NumericUpDown row;
		private System.Windows.Forms.Panel dummyPanel;
    }
}

