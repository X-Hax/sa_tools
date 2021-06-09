namespace SADXTweaker2
{
    partial class LevelActControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.level = new System.Windows.Forms.ComboBox();
            this.act = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.act)).BeginInit();
            this.SuspendLayout();
            // 
            // level
            // 
            this.level.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.level.Location = new System.Drawing.Point(0, 0);
            this.level.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.level.Name = "level";
            this.level.Size = new System.Drawing.Size(136, 21);
            this.level.TabIndex = 0;
            this.level.SelectedIndexChanged += new System.EventHandler(this.level_SelectedIndexChanged);
            // 
            // act
            // 
            this.act.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.act.AutoSize = true;
            this.act.Location = new System.Drawing.Point(142, 0);
            this.act.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.act.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.act.Name = "act";
            this.act.Size = new System.Drawing.Size(66, 20);
            this.act.TabIndex = 1;
            this.act.ValueChanged += new System.EventHandler(this.act_ValueChanged);
            // 
            // LevelActControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.act);
            this.Controls.Add(this.level);
            this.Name = "LevelActControl";
            this.Size = new System.Drawing.Size(208, 21);
            this.Load += new System.EventHandler(this.LevelActControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.act)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox level;
        private System.Windows.Forms.NumericUpDown act;
    }
}
