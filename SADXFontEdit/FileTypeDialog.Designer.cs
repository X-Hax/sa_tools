namespace SADXFontEdit
{
	partial class FileTypeDialog
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
            this.radioButton_1252 = new System.Windows.Forms.RadioButton();
            this.radioButton_J2022 = new System.Windows.Forms.RadioButton();
            this.radioButton_Custom = new System.Windows.Forms.RadioButton();
            this.groupBox_Codepage = new System.Windows.Forms.GroupBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.radioButton_Unicode = new System.Windows.Forms.RadioButton();
            this.radioButton_Simple = new System.Windows.Forms.RadioButton();
            this.radioButton_Fontdata = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonLoadCharMap = new System.Windows.Forms.Button();
            this.checkBox_CharMap = new System.Windows.Forms.CheckBox();
            this.groupBox_Codepage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButton_1252
            // 
            this.radioButton_1252.AutoSize = true;
            this.radioButton_1252.Checked = true;
            this.radioButton_1252.Location = new System.Drawing.Point(9, 29);
            this.radioButton_1252.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButton_1252.Name = "radioButton_1252";
            this.radioButton_1252.Size = new System.Drawing.Size(144, 24);
            this.radioButton_1252.TabIndex = 2;
            this.radioButton_1252.TabStop = true;
            this.radioButton_1252.Text = "Western (1252)";
            this.radioButton_1252.UseVisualStyleBackColor = true;
            this.radioButton_1252.CheckedChanged += new System.EventHandler(this.radioButton_1252_CheckedChanged);
            // 
            // radioButton_J2022
            // 
            this.radioButton_J2022.AutoSize = true;
            this.radioButton_J2022.Location = new System.Drawing.Point(9, 63);
            this.radioButton_J2022.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButton_J2022.Name = "radioButton_J2022";
            this.radioButton_J2022.Size = new System.Drawing.Size(163, 24);
            this.radioButton_J2022.TabIndex = 3;
            this.radioButton_J2022.Text = "Japanese (50220)";
            this.radioButton_J2022.UseVisualStyleBackColor = true;
            this.radioButton_J2022.CheckedChanged += new System.EventHandler(this.radioButton_J2022_CheckedChanged);
            // 
            // radioButton_Custom
            // 
            this.radioButton_Custom.AutoSize = true;
            this.radioButton_Custom.Location = new System.Drawing.Point(9, 131);
            this.radioButton_Custom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButton_Custom.Name = "radioButton_Custom";
            this.radioButton_Custom.Size = new System.Drawing.Size(89, 24);
            this.radioButton_Custom.TabIndex = 5;
            this.radioButton_Custom.Text = "Custom";
            this.radioButton_Custom.UseVisualStyleBackColor = true;
            this.radioButton_Custom.CheckedChanged += new System.EventHandler(this.radioButton_Custom_CheckedChanged);
            // 
            // groupBox_Codepage
            // 
            this.groupBox_Codepage.Controls.Add(this.numericUpDown1);
            this.groupBox_Codepage.Controls.Add(this.radioButton_Unicode);
            this.groupBox_Codepage.Controls.Add(this.radioButton_1252);
            this.groupBox_Codepage.Controls.Add(this.radioButton_Custom);
            this.groupBox_Codepage.Controls.Add(this.radioButton_J2022);
            this.groupBox_Codepage.Location = new System.Drawing.Point(16, 98);
            this.groupBox_Codepage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox_Codepage.Name = "groupBox_Codepage";
            this.groupBox_Codepage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox_Codepage.Size = new System.Drawing.Size(239, 178);
            this.groupBox_Codepage.TabIndex = 6;
            this.groupBox_Codepage.TabStop = false;
            this.groupBox_Codepage.Text = "Codepage";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Location = new System.Drawing.Point(105, 131);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            99000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 26);
            this.numericUpDown1.TabIndex = 8;
            this.numericUpDown1.Value = new decimal(new int[] {
            1252,
            0,
            0,
            0});
            // 
            // radioButton_Unicode
            // 
            this.radioButton_Unicode.AutoSize = true;
            this.radioButton_Unicode.Location = new System.Drawing.Point(9, 97);
            this.radioButton_Unicode.Name = "radioButton_Unicode";
            this.radioButton_Unicode.Size = new System.Drawing.Size(143, 24);
            this.radioButton_Unicode.TabIndex = 7;
            this.radioButton_Unicode.Text = "Unicode (1200)";
            this.radioButton_Unicode.UseVisualStyleBackColor = true;
            this.radioButton_Unicode.CheckedChanged += new System.EventHandler(this.radioButton_Unicode_CheckedChanged);
            // 
            // radioButton_Simple
            // 
            this.radioButton_Simple.AutoSize = true;
            this.radioButton_Simple.Location = new System.Drawing.Point(16, 34);
            this.radioButton_Simple.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButton_Simple.Name = "radioButton_Simple";
            this.radioButton_Simple.Size = new System.Drawing.Size(82, 24);
            this.radioButton_Simple.TabIndex = 1;
            this.radioButton_Simple.Text = "Simple";
            this.radioButton_Simple.UseVisualStyleBackColor = true;
            this.radioButton_Simple.CheckedChanged += new System.EventHandler(this.radioButton_Simple_CheckedChanged);
            // 
            // radioButton_Fontdata
            // 
            this.radioButton_Fontdata.AutoSize = true;
            this.radioButton_Fontdata.Checked = true;
            this.radioButton_Fontdata.Location = new System.Drawing.Point(136, 34);
            this.radioButton_Fontdata.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButton_Fontdata.Name = "radioButton_Fontdata";
            this.radioButton_Fontdata.Size = new System.Drawing.Size(119, 24);
            this.radioButton_Fontdata.TabIndex = 0;
            this.radioButton_Fontdata.TabStop = true;
            this.radioButton_Fontdata.Text = "FONTDATA";
            this.radioButton_Fontdata.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Data format:";
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(167, 284);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 34);
            this.button2.TabIndex = 8;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonLoadCharMap
            // 
            this.buttonLoadCharMap.Location = new System.Drawing.Point(162, 61);
            this.buttonLoadCharMap.Name = "buttonLoadCharMap";
            this.buttonLoadCharMap.Size = new System.Drawing.Size(88, 32);
            this.buttonLoadCharMap.TabIndex = 10;
            this.buttonLoadCharMap.Text = "Load...";
            this.buttonLoadCharMap.UseVisualStyleBackColor = true;
            this.buttonLoadCharMap.Click += new System.EventHandler(this.buttonLoadCharMap_Click);
            // 
            // checkBox_CharMap
            // 
            this.checkBox_CharMap.AutoSize = true;
            this.checkBox_CharMap.Location = new System.Drawing.Point(16, 66);
            this.checkBox_CharMap.Name = "checkBox_CharMap";
            this.checkBox_CharMap.Size = new System.Drawing.Size(140, 24);
            this.checkBox_CharMap.TabIndex = 11;
            this.checkBox_CharMap.Text = "Character map";
            this.checkBox_CharMap.UseVisualStyleBackColor = true;
            this.checkBox_CharMap.CheckedChanged += new System.EventHandler(this.checkBox_CharMap_CheckedChanged);
            // 
            // FileTypeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(272, 332);
            this.Controls.Add(this.checkBox_CharMap);
            this.Controls.Add(this.buttonLoadCharMap);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButton_Fontdata);
            this.Controls.Add(this.radioButton_Simple);
            this.Controls.Add(this.groupBox_Codepage);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FileTypeDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Open";
            this.groupBox_Codepage.ResumeLayout(false);
            this.groupBox_Codepage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButton_1252;
		private System.Windows.Forms.RadioButton radioButton_J2022;
		private System.Windows.Forms.RadioButton radioButton_Custom;
		private System.Windows.Forms.GroupBox groupBox_Codepage;
		private System.Windows.Forms.RadioButton radioButton_Simple;
		private System.Windows.Forms.RadioButton radioButton_Fontdata;
		private System.Windows.Forms.RadioButton radioButton_Unicode;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Button buttonLoadCharMap;
		private System.Windows.Forms.CheckBox checkBox_CharMap;
	}
}