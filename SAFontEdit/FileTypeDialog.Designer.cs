namespace SAFontEdit
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
			radioButtonWestern = new System.Windows.Forms.RadioButton();
			radioButtonJapanese = new System.Windows.Forms.RadioButton();
			radioButtonCustomCodepage = new System.Windows.Forms.RadioButton();
			groupBoxCodepage = new System.Windows.Forms.GroupBox();
			radioButtonAnsiTrim = new System.Windows.Forms.RadioButton();
			radioButtonKanjiSOC = new System.Windows.Forms.RadioButton();
			radioButtonKanji = new System.Windows.Forms.RadioButton();
			radioButtonCustomCharmap = new System.Windows.Forms.RadioButton();
			buttonLoadCharMap = new System.Windows.Forms.Button();
			numericUpDownCodepage = new System.Windows.Forms.NumericUpDown();
			radioButtonUnicode = new System.Windows.Forms.RadioButton();
			radioButtonSimple1Bit = new System.Windows.Forms.RadioButton();
			radioButtonFontdata = new System.Windows.Forms.RadioButton();
			buttonOK = new System.Windows.Forms.Button();
			radioButtonSimple32Bit = new System.Windows.Forms.RadioButton();
			groupBoxDataFormat = new System.Windows.Forms.GroupBox();
			groupBoxCodepage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownCodepage).BeginInit();
			groupBoxDataFormat.SuspendLayout();
			SuspendLayout();
			// 
			// radioButtonWestern
			// 
			radioButtonWestern.AutoSize = true;
			radioButtonWestern.Checked = true;
			radioButtonWestern.Location = new System.Drawing.Point(6, 20);
			radioButtonWestern.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			radioButtonWestern.Name = "radioButtonWestern";
			radioButtonWestern.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			radioButtonWestern.Size = new System.Drawing.Size(86, 23);
			radioButtonWestern.TabIndex = 0;
			radioButtonWestern.TabStop = true;
			radioButtonWestern.Text = "ANSI (1252)";
			radioButtonWestern.UseVisualStyleBackColor = true;
			// 
			// radioButtonJapanese
			// 
			radioButtonJapanese.AutoSize = true;
			radioButtonJapanese.Location = new System.Drawing.Point(6, 51);
			radioButtonJapanese.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			radioButtonJapanese.Name = "radioButtonJapanese";
			radioButtonJapanese.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			radioButtonJapanese.Size = new System.Drawing.Size(108, 23);
			radioButtonJapanese.TabIndex = 2;
			radioButtonJapanese.Text = "Shift-JIS (50220)";
			radioButtonJapanese.UseVisualStyleBackColor = true;
			// 
			// radioButtonCustomCodepage
			// 
			radioButtonCustomCodepage.AutoSize = true;
			radioButtonCustomCodepage.Location = new System.Drawing.Point(6, 138);
			radioButtonCustomCodepage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			radioButtonCustomCodepage.Name = "radioButtonCustomCodepage";
			radioButtonCustomCodepage.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			radioButtonCustomCodepage.Size = new System.Drawing.Size(82, 23);
			radioButtonCustomCodepage.TabIndex = 6;
			radioButtonCustomCodepage.Text = "Codepage:";
			radioButtonCustomCodepage.UseVisualStyleBackColor = true;
			radioButtonCustomCodepage.CheckedChanged += radioButton_Custom_CheckedChanged;
			// 
			// groupBoxCodepage
			// 
			groupBoxCodepage.Controls.Add(radioButtonAnsiTrim);
			groupBoxCodepage.Controls.Add(radioButtonKanjiSOC);
			groupBoxCodepage.Controls.Add(radioButtonKanji);
			groupBoxCodepage.Controls.Add(radioButtonCustomCharmap);
			groupBoxCodepage.Controls.Add(buttonLoadCharMap);
			groupBoxCodepage.Controls.Add(numericUpDownCodepage);
			groupBoxCodepage.Controls.Add(radioButtonUnicode);
			groupBoxCodepage.Controls.Add(radioButtonWestern);
			groupBoxCodepage.Controls.Add(radioButtonCustomCodepage);
			groupBoxCodepage.Controls.Add(radioButtonJapanese);
			groupBoxCodepage.Location = new System.Drawing.Point(12, 98);
			groupBoxCodepage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			groupBoxCodepage.Name = "groupBoxCodepage";
			groupBoxCodepage.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			groupBoxCodepage.Size = new System.Drawing.Size(248, 210);
			groupBoxCodepage.TabIndex = 1;
			groupBoxCodepage.TabStop = false;
			groupBoxCodepage.Text = "Character Map";
			// 
			// radioButtonAnsiTrim
			// 
			radioButtonAnsiTrim.AutoSize = true;
			radioButtonAnsiTrim.Location = new System.Drawing.Point(130, 20);
			radioButtonAnsiTrim.Name = "radioButtonAnsiTrim";
			radioButtonAnsiTrim.Size = new System.Drawing.Size(99, 19);
			radioButtonAnsiTrim.TabIndex = 1;
			radioButtonAnsiTrim.TabStop = true;
			radioButtonAnsiTrim.Text = "1252 Trimmed";
			radioButtonAnsiTrim.UseVisualStyleBackColor = true;
			// 
			// radioButtonKanjiSOC
			// 
			radioButtonKanjiSOC.AutoSize = true;
			radioButtonKanjiSOC.Location = new System.Drawing.Point(130, 81);
			radioButtonKanjiSOC.Name = "radioButtonKanjiSOC";
			radioButtonKanjiSOC.Size = new System.Drawing.Size(112, 19);
			radioButtonKanjiSOC.TabIndex = 4;
			radioButtonKanjiSOC.TabStop = true;
			radioButtonKanjiSOC.Text = "Kanji Font (SOC)";
			radioButtonKanjiSOC.UseVisualStyleBackColor = true;
			// 
			// radioButtonKanji
			// 
			radioButtonKanji.AutoSize = true;
			radioButtonKanji.Location = new System.Drawing.Point(6, 81);
			radioButtonKanji.Name = "radioButtonKanji";
			radioButtonKanji.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			radioButtonKanji.Size = new System.Drawing.Size(108, 23);
			radioButtonKanji.TabIndex = 3;
			radioButtonKanji.TabStop = true;
			radioButtonKanji.Text = "Kanji Font (Old)";
			radioButtonKanji.UseVisualStyleBackColor = true;
			// 
			// radioButtonCustomCharmap
			// 
			radioButtonCustomCharmap.AutoSize = true;
			radioButtonCustomCharmap.Location = new System.Drawing.Point(6, 168);
			radioButtonCustomCharmap.Name = "radioButtonCustomCharmap";
			radioButtonCustomCharmap.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			radioButtonCustomCharmap.Size = new System.Drawing.Size(67, 23);
			radioButtonCustomCharmap.TabIndex = 8;
			radioButtonCustomCharmap.TabStop = true;
			radioButtonCustomCharmap.Text = "Custom";
			radioButtonCustomCharmap.UseVisualStyleBackColor = true;
			radioButtonCustomCharmap.CheckedChanged += radioButtonCustomCharmap_CheckedChanged;
			// 
			// buttonLoadCharMap
			// 
			buttonLoadCharMap.Location = new System.Drawing.Point(95, 167);
			buttonLoadCharMap.Margin = new System.Windows.Forms.Padding(2);
			buttonLoadCharMap.Name = "buttonLoadCharMap";
			buttonLoadCharMap.Size = new System.Drawing.Size(68, 24);
			buttonLoadCharMap.TabIndex = 9;
			buttonLoadCharMap.Text = "Load...";
			buttonLoadCharMap.UseVisualStyleBackColor = true;
			buttonLoadCharMap.Click += buttonLoadCharMap_Click;
			// 
			// numericUpDownCodepage
			// 
			numericUpDownCodepage.Enabled = false;
			numericUpDownCodepage.Location = new System.Drawing.Point(95, 138);
			numericUpDownCodepage.Margin = new System.Windows.Forms.Padding(2);
			numericUpDownCodepage.Maximum = new decimal(new int[] { 99000, 0, 0, 0 });
			numericUpDownCodepage.Name = "numericUpDownCodepage";
			numericUpDownCodepage.Size = new System.Drawing.Size(68, 23);
			numericUpDownCodepage.TabIndex = 7;
			numericUpDownCodepage.Value = new decimal(new int[] { 1252, 0, 0, 0 });
			// 
			// radioButtonUnicode
			// 
			radioButtonUnicode.AutoSize = true;
			radioButtonUnicode.Location = new System.Drawing.Point(6, 109);
			radioButtonUnicode.Margin = new System.Windows.Forms.Padding(2);
			radioButtonUnicode.Name = "radioButtonUnicode";
			radioButtonUnicode.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			radioButtonUnicode.Size = new System.Drawing.Size(104, 23);
			radioButtonUnicode.TabIndex = 5;
			radioButtonUnicode.Text = "Unicode (1200)";
			radioButtonUnicode.UseVisualStyleBackColor = true;
			// 
			// radioButtonSimple1Bit
			// 
			radioButtonSimple1Bit.AutoSize = true;
			radioButtonSimple1Bit.Location = new System.Drawing.Point(6, 49);
			radioButtonSimple1Bit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			radioButtonSimple1Bit.Name = "radioButtonSimple1Bit";
			radioButtonSimple1Bit.Size = new System.Drawing.Size(91, 19);
			radioButtonSimple1Bit.TabIndex = 0;
			radioButtonSimple1Bit.Text = "Simple 1bpp";
			radioButtonSimple1Bit.UseVisualStyleBackColor = true;
			// 
			// radioButtonFontdata
			// 
			radioButtonFontdata.AutoSize = true;
			radioButtonFontdata.Checked = true;
			radioButtonFontdata.Location = new System.Drawing.Point(6, 23);
			radioButtonFontdata.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			radioButtonFontdata.Name = "radioButtonFontdata";
			radioButtonFontdata.Size = new System.Drawing.Size(139, 19);
			radioButtonFontdata.TabIndex = 1;
			radioButtonFontdata.TabStop = true;
			radioButtonFontdata.Text = "SADX2004 FONTDATA";
			radioButtonFontdata.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonOK.Location = new System.Drawing.Point(192, 314);
			buttonOK.Margin = new System.Windows.Forms.Padding(2);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new System.Drawing.Size(68, 26);
			buttonOK.TabIndex = 2;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			buttonOK.Click += buttonOK_Click;
			// 
			// radioButtonSimple32Bit
			// 
			radioButtonSimple32Bit.AutoSize = true;
			radioButtonSimple32Bit.Location = new System.Drawing.Point(130, 49);
			radioButtonSimple32Bit.Name = "radioButtonSimple32Bit";
			radioButtonSimple32Bit.Size = new System.Drawing.Size(97, 19);
			radioButtonSimple32Bit.TabIndex = 2;
			radioButtonSimple32Bit.TabStop = true;
			radioButtonSimple32Bit.Text = "Simple 32bpp";
			radioButtonSimple32Bit.UseVisualStyleBackColor = true;
			// 
			// groupBoxDataFormat
			// 
			groupBoxDataFormat.Controls.Add(radioButtonSimple1Bit);
			groupBoxDataFormat.Controls.Add(radioButtonSimple32Bit);
			groupBoxDataFormat.Controls.Add(radioButtonFontdata);
			groupBoxDataFormat.Location = new System.Drawing.Point(12, 12);
			groupBoxDataFormat.Name = "groupBoxDataFormat";
			groupBoxDataFormat.Size = new System.Drawing.Size(248, 79);
			groupBoxDataFormat.TabIndex = 0;
			groupBoxDataFormat.TabStop = false;
			groupBoxDataFormat.Text = "Data Format";
			// 
			// FileTypeDialog
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(272, 348);
			Controls.Add(groupBoxDataFormat);
			Controls.Add(buttonOK);
			Controls.Add(groupBoxCodepage);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			HelpButton = true;
			Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "FileTypeDialog";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Open";
			HelpButtonClicked += FileTypeDialog_HelpButtonClicked;
			groupBoxCodepage.ResumeLayout(false);
			groupBoxCodepage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownCodepage).EndInit();
			groupBoxDataFormat.ResumeLayout(false);
			groupBoxDataFormat.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.RadioButton radioButtonWestern;
		private System.Windows.Forms.RadioButton radioButtonJapanese;
		private System.Windows.Forms.RadioButton radioButtonCustomCodepage;
		private System.Windows.Forms.GroupBox groupBoxCodepage;
		private System.Windows.Forms.RadioButton radioButtonSimple1Bit;
		private System.Windows.Forms.RadioButton radioButtonFontdata;
		private System.Windows.Forms.RadioButton radioButtonUnicode;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.NumericUpDown numericUpDownCodepage;
		private System.Windows.Forms.Button buttonLoadCharMap;
		private System.Windows.Forms.RadioButton radioButtonCustomCharmap;
		private System.Windows.Forms.RadioButton radioButtonSimple32Bit;
		private System.Windows.Forms.GroupBox groupBoxDataFormat;
		private System.Windows.Forms.RadioButton radioButtonKanji;
		private System.Windows.Forms.RadioButton radioButtonKanjiSOC;
		private System.Windows.Forms.RadioButton radioButtonAnsiTrim;
	}
}