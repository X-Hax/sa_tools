namespace SADXTweaker2
{
    partial class NextLevelListEditor
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
            this.objectList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.levelList = new System.Windows.Forms.ComboBox();
            this.addButton = new System.Windows.Forms.Button();
            this.pasteButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cgMovie = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.altEntrance = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.entrance = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.altNextLevel = new SADXTweaker2.LevelActControl();
            this.label4 = new System.Windows.Forms.Label();
            this.nextLevel = new SADXTweaker2.LevelActControl();
            this.label3 = new System.Windows.Forms.Label();
            this.level = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cgMovie)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.altEntrance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.entrance)).BeginInit();
            this.SuspendLayout();
            // 
            // objectList
            // 
            this.objectList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectList.Location = new System.Drawing.Point(48, 39);
            this.objectList.Name = "objectList";
            this.objectList.Size = new System.Drawing.Size(173, 21);
            this.objectList.TabIndex = 2;
            this.objectList.SelectedIndexChanged += new System.EventHandler(this.objectList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "Item:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 53;
            this.label8.Text = "List:";
            // 
            // deleteButton
            // 
            this.deleteButton.AutoSize = true;
            this.deleteButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(269, 37);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(48, 23);
            this.deleteButton.TabIndex = 4;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // levelList
            // 
            this.levelList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.levelList.Location = new System.Drawing.Point(44, 12);
            this.levelList.Name = "levelList";
            this.levelList.Size = new System.Drawing.Size(273, 21);
            this.levelList.TabIndex = 1;
            this.levelList.SelectedIndexChanged += new System.EventHandler(this.levelList_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.AutoSize = true;
            this.addButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addButton.Location = new System.Drawing.Point(227, 37);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(36, 23);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // pasteButton
            // 
            this.pasteButton.AutoSize = true;
            this.pasteButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pasteButton.Enabled = false;
            this.pasteButton.Location = new System.Drawing.Point(273, 260);
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(44, 23);
            this.pasteButton.TabIndex = 13;
            this.pasteButton.Text = "Paste";
            this.pasteButton.UseVisualStyleBackColor = true;
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.AutoSize = true;
            this.copyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.copyButton.Enabled = false;
            this.copyButton.Location = new System.Drawing.Point(226, 260);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(41, 23);
            this.copyButton.TabIndex = 12;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.cgMovie);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.altEntrance);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.entrance);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.altNextLevel);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.nextLevel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.level);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox1.Size = new System.Drawing.Size(305, 188);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Data";
            // 
            // cgMovie
            // 
            this.cgMovie.Location = new System.Drawing.Point(73, 19);
            this.cgMovie.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.cgMovie.Name = "cgMovie";
            this.cgMovie.Size = new System.Drawing.Size(55, 20);
            this.cgMovie.TabIndex = 6;
            this.cgMovie.ValueChanged += new System.EventHandler(this.cgMovie_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 39;
            this.label7.Text = "CG Movie:";
            // 
            // altEntrance
            // 
            this.altEntrance.Location = new System.Drawing.Point(91, 152);
            this.altEntrance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.altEntrance.Name = "altEntrance";
            this.altEntrance.Size = new System.Drawing.Size(55, 20);
            this.altEntrance.TabIndex = 11;
            this.altEntrance.ValueChanged += new System.EventHandler(this.altEntrance_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 37;
            this.label6.Text = "Alt. Entrance:";
            // 
            // entrance
            // 
            this.entrance.Location = new System.Drawing.Point(73, 99);
            this.entrance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.entrance.Name = "entrance";
            this.entrance.Size = new System.Drawing.Size(55, 20);
            this.entrance.TabIndex = 9;
            this.entrance.ValueChanged += new System.EventHandler(this.entrance_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 35;
            this.label5.Text = "Entrance:";
            // 
            // altNextLevel
            // 
            this.altNextLevel.Location = new System.Drawing.Point(91, 125);
            this.altNextLevel.Name = "altNextLevel";
            this.altNextLevel.Size = new System.Drawing.Size(208, 21);
            this.altNextLevel.TabIndex = 10;
            this.altNextLevel.ValueChanged += new System.EventHandler(this.altNextLevel_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Alt. Next Level:";
            // 
            // nextLevel
            // 
            this.nextLevel.Location = new System.Drawing.Point(73, 72);
            this.nextLevel.Name = "nextLevel";
            this.nextLevel.Size = new System.Drawing.Size(208, 21);
            this.nextLevel.TabIndex = 8;
            this.nextLevel.ValueChanged += new System.EventHandler(this.nextLevel_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Next Level:";
            // 
            // level
            // 
            this.level.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.level.FormattingEnabled = true;
            this.level.Items.AddRange(new object[] {
            "Hedgehog Hammer",
            "Emerald Coast",
            "Windy Valley",
            "Twinkle Park",
            "Speed Highway",
            "Red Mountain",
            "Sky Deck",
            "Lost World",
            "Ice Cap",
            "Casinopolis",
            "Final Egg",
            "Unused",
            "Hot Shelter",
            "Unused",
            "Unused",
            "Chaos 0",
            "Chaos 2",
            "Chaos 4",
            "Chaos 6",
            "Perfect Chaos",
            "Egg Hornet",
            "Egg Walker",
            "Egg Viper",
            "ZERO",
            "E-101 β",
            "E-101mkII",
            "Station Square",
            "Unused",
            "Unused",
            "Egg Carrier Outside",
            "Unused",
            "Unused",
            "Egg Carrier Inside",
            "Mystic Ruins",
            "The Past",
            "Twinkle Circuit",
            "Sky Chase Act 1",
            "Sky Chase Act 2",
            "Sand Hill",
            "Chao Garden (Station Square)",
            "Chao Garden (Egg Carrier)",
            "Chao Garden (Mystic Ruins)",
            "Chao Garden (Chao Race)"});
            this.level.Location = new System.Drawing.Point(73, 45);
            this.level.Name = "level";
            this.level.Size = new System.Drawing.Size(139, 21);
            this.level.TabIndex = 7;
            this.level.SelectedIndexChanged += new System.EventHandler(this.level_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Level:";
            // 
            // NextLevelListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 294);
            this.Controls.Add(this.objectList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.levelList);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.pasteButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.groupBox1);
            this.Name = "NextLevelListEditor";
            this.ShowIcon = false;
            this.Text = "Next Level List Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NextLevelListEditor_FormClosing);
            this.Load += new System.EventHandler(this.NextLevelListEditor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cgMovie)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.altEntrance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.entrance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox objectList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ComboBox levelList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox level;
        private LevelActControl nextLevel;
        private System.Windows.Forms.Label label3;
        private LevelActControl altNextLevel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown entrance;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown altEntrance;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown cgMovie;
        private System.Windows.Forms.Label label7;
    }
}