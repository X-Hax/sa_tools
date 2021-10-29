namespace SADXTweaker2
{
    partial class LevelClearFlagListEditor
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
            this.flag = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.objectList = new System.Windows.Forms.ComboBox();
            this.level = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.levelList = new System.Windows.Forms.ComboBox();
            this.addButton = new System.Windows.Forms.Button();
            this.pasteButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.flag)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flag
            // 
            this.flag.Hexadecimal = true;
            this.flag.Location = new System.Drawing.Point(48, 45);
            this.flag.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.flag.Name = "flag";
            this.flag.Size = new System.Drawing.Size(55, 20);
            this.flag.TabIndex = 7;
            this.flag.ValueChanged += new System.EventHandler(this.flag_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 39;
            this.label7.Text = "Level:";
            // 
            // objectList
            // 
            this.objectList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectList.Location = new System.Drawing.Point(48, 39);
            this.objectList.Name = "objectList";
            this.objectList.Size = new System.Drawing.Size(128, 21);
            this.objectList.TabIndex = 2;
            this.objectList.SelectedIndexChanged += new System.EventHandler(this.objectList_SelectedIndexChanged);
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
            this.level.Location = new System.Drawing.Point(48, 18);
            this.level.Name = "level";
            this.level.Size = new System.Drawing.Size(158, 21);
            this.level.TabIndex = 6;
            this.level.SelectedIndexChanged += new System.EventHandler(this.level_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 63;
            this.label1.Text = "Item:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 62;
            this.label8.Text = "List:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Flag:";
            // 
            // deleteButton
            // 
            this.deleteButton.AutoSize = true;
            this.deleteButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(224, 37);
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
            this.addButton.Location = new System.Drawing.Point(182, 37);
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
            this.pasteButton.Location = new System.Drawing.Point(228, 153);
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(44, 23);
            this.pasteButton.TabIndex = 9;
            this.pasteButton.Text = "Paste";
            this.pasteButton.UseVisualStyleBackColor = true;
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.AutoSize = true;
            this.copyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.copyButton.Enabled = false;
            this.copyButton.Location = new System.Drawing.Point(181, 153);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(41, 23);
            this.copyButton.TabIndex = 8;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.flag);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.level);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox1.Size = new System.Drawing.Size(212, 81);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Data";
            // 
            // LevelClearFlagListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 187);
            this.Controls.Add(this.objectList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.levelList);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.pasteButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.groupBox1);
            this.Name = "LevelClearFlagListEditor";
            this.ShowIcon = false;
            this.Text = "Level Clear Flag List Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LevelClearFlagListEditor_FormClosing);
            this.Load += new System.EventHandler(this.LevelClearFlagListEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.flag)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown flag;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox objectList;
        private System.Windows.Forms.ComboBox level;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ComboBox levelList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}