namespace SADXTweaker2
{
    partial class SoundTestListEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoundTestListEditor));
            this.objectList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.levelList = new System.Windows.Forms.ComboBox();
            this.addButton = new System.Windows.Forms.Button();
            this.pasteButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trackNum = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.trackName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectList
            // 
            this.objectList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectList.Location = new System.Drawing.Point(48, 39);
            this.objectList.Name = "objectList";
            this.objectList.Size = new System.Drawing.Size(134, 21);
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
            this.deleteButton.Location = new System.Drawing.Point(230, 37);
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
            this.levelList.Size = new System.Drawing.Size(228, 21);
            this.levelList.TabIndex = 1;
            this.levelList.SelectedIndexChanged += new System.EventHandler(this.levelList_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.AutoSize = true;
            this.addButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addButton.Location = new System.Drawing.Point(188, 37);
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
            this.pasteButton.Location = new System.Drawing.Point(228, 205);
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(44, 23);
            this.pasteButton.TabIndex = 10;
            this.pasteButton.Text = "Paste";
            this.pasteButton.UseVisualStyleBackColor = true;
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.AutoSize = true;
            this.copyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.copyButton.Enabled = false;
            this.copyButton.Location = new System.Drawing.Point(181, 205);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(41, 23);
            this.copyButton.TabIndex = 9;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.trackNum);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.trackName);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox1.Size = new System.Drawing.Size(266, 133);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Data";
            // 
            // trackNum
            // 
            this.trackNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.trackNum.FormattingEnabled = true;
            this.trackNum.Items.AddRange(new object[] {
            "Instruction: Amy",
            "Instruction: Big",
            "Instruction: E-102",
            "Instruction: Knuckles",
            "Instruction: Tails",
            "Instruction: Sonic",
            "My Sweet Passion",
            "Lazy Days~Livin\' In Paradise",
            "Egg Mobile ...Boss: Egg Hornet",
            "Crazy Robo ...Boss: E-101R",
            "Fight for My Own Way ...Boss: Event",
            "Heartless Colleague ...Boss: E-Series Targets",
            "The Dreamy Stage ...for Casinopolis",
            "Dilapidated Way ...for Casinopolis",
            "Blue Star ...for Casinopolis",
            "Message from Nightopia",
            "Theme of \"CHAO\"",
            "Chao Race Goal",
            "Letz Get This Party Started ...for CHAO Race Entrance",
            "Join Us 4 Happy Time ...for CHAO Race",
            "Boss: CHAOS ver.0, 2, 4",
            "Boss: CHAOS ver.6",
            "Boss: Perfect CHAOS",
            "Perfect CHAOS Revival! ...Boss: Perfect CHAOS",
            "Choose Your Buddy!",
            "Twinkle Circuit",
            "Will You Continue?",
            "Theme of \"E-102g\"",
            "Azure Blue World ...for Emerald Coast",
            "Windy and Ripply ...for Emerald Coast",
            "BIG Fishes at Emerald Coast...",
            "Egg Carrier - A Song That Keeps Us On The Move",
            "Calm After the Storm ...Egg Carrier -the ocean-",
            "Theme of \"Dr.EGGMAN\"",
            "Militant Missionary ...Boss: Egg Walker & Egg Viper",
            "ZERO The Chase-master ...Boss: Eggman Robot -ZERO-",
            "Event: Sadness",
            "Event: Strain",
            "Event: Unbound",
            "Event: Good-bye!",
            "Event: The Past",
            "Event: Fanfare for \"Dr.EGGMAN\"",
            "Mechanical Resonance ...for Final Egg",
            "Crank the Heat Up!! ...for Final Egg",
            "Fish Get!",
            "and... Fish Hits!",
            "Fish Miss!",
            "Sweet Punch ...for Hedgehog Hammer",
            "Run Through the Speed Highway ...for Speed Highway",
            "Goin\' Down!? ...for Speed Highway",
            "At Dawn ...for Speed Highway",
            "Amy: Hurry Up!",
            "Snowy Mountain ...for Icecap",
            "Limestone Cave ...for Icecap",
            "Be Cool, Be Wild and Be Groovy ...for Icecap",
            "Invincible ...No Fear!",
            "Item",
            "Jingle A",
            "Jingle B",
            "Jingle C",
            "Jingle D",
            "Jingle E",
            "Unknown from M.E.",
            "Tricky Maze ...for Lost World",
            "Danger! Chased by Rock ...for Lost World",
            "Leading Lights ...for Lost World",
            "Open your Heart -Main Theme of \"SONIC Adventure\"-",
            "Mystic Ruin",
            "Dream Dreams: A-Capella Ver. (removed)",
            "nights_k (removed)",
            "Dream Dreams: Sweet Mix in Holy Night (removed)",
            "Extend",
            "Funky Groove Makes U Hot!? ...for Options",
            "Mt. Red: a Symbol of Thrill ...for Red Mountain",
            "Red Hot Skull ...for Red Mountain",
            "Round Clear",
            "Welcome to Station Square",
            "Sand Hill",
            "Tornado Scramble ...for Sky Chase",
            "Bad Taste Aquarium ...for Hot Shelter",
            "Red Barrage Area ...for Hot Shelter",
            "Skydeck A Go! Go! ...for Sky Deck",
            "General Offensive ...for Sky Deck",
            "It Doesn\'t Matter",
            "Palmtree Panic Zone JP (Sonic CD) (removed)",
            "Hey You! It\'s Time to Speed Up!!!",
            "Theme of \"SUPER SONIC\"",
            "Super Sonic Racing (Sonic R) (removed)",
            "Believe in Myself",
            "Appearance: AMY",
            "Appearance: BIG",
            "Appearance: E-102",
            "Appearance: KNUCKLES",
            "Appearance: MILES",
            "Appearance: SONIC",
            "Theme of \"TIKAL\"",
            "Drowning",
            "Egg Carrier Transform",
            "titl_mr1",
            "titl_mr2",
            "Trial Version Quit",
            "Main Menu",
            "Title Screen",
            "Trial",
            "Twinkle Cart ...for Twinkle Park",
            "Pleasure Castle ...for Twinkle Park",
            "Fakery Way ...for Twinkle Park",
            "Windy Hill ...for Windy Valley",
            "Tornado ...for Windy Valley",
            "The Air ...for Windy Valley",
            "Mission Start!",
            "Mission Clear!",
            "Chao: Level Up!",
            "Chao: Goodbye!",
            "Chao: Naming",
            "Chao Race Entry",
            "Chao Race Gate Open",
            "Hero Chaos Chao Born 2",
            "Dark Chaos Chao Born 2",
            "Chaos Chao Born",
            "Hero Chaos Chao Born",
            "Dark Chaos Chao Born",
            "Chao Died",
            "Chao Dance",
            "Chao: Black Market"});
            this.trackNum.Location = new System.Drawing.Point(50, 49);
            this.trackNum.Name = "trackNum";
            this.trackNum.Size = new System.Drawing.Size(210, 21);
            this.trackNum.TabIndex = 7;
            this.trackNum.SelectedIndexChanged += new System.EventHandler(this.trackNum_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 37;
            this.label2.Text = "Track:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 36;
            this.label6.Text = "Title:";
            // 
            // trackName
            // 
            this.trackName.Location = new System.Drawing.Point(42, 19);
            this.trackName.Name = "trackName";
            this.trackName.Size = new System.Drawing.Size(218, 20);
            this.trackName.TabIndex = 6;
            this.trackName.TextChanged += new System.EventHandler(this.trackName_TextChanged);
            // 
            // SoundTestListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 236);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.objectList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.levelList);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.pasteButton);
            this.Controls.Add(this.copyButton);
            this.Name = "SoundTestListEditor";
            this.ShowIcon = false;
            this.Text = "Sound Test List Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SoundTestListEditor_FormClosing);
            this.Load += new System.EventHandler(this.SoundTestListEditor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox trackName;
        private System.Windows.Forms.ComboBox trackNum;
        private System.Windows.Forms.Label label2;
    }
}