
namespace TextureEditor
{
	partial class IndexedImageImportDialog
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndexedImageImportDialog));
			groupBoxMask = new System.Windows.Forms.GroupBox();
			radioButtonNonIndexed = new System.Windows.Forms.RadioButton();
			radioButtonIndex8 = new System.Windows.Forms.RadioButton();
			radioButtonIndex4 = new System.Windows.Forms.RadioButton();
			groupBoxPalette = new System.Windows.Forms.GroupBox();
			radioButtonIntensity8A = new System.Windows.Forms.RadioButton();
			radioButtonRGB5A3 = new System.Windows.Forms.RadioButton();
			radioButtonARGB8888 = new System.Windows.Forms.RadioButton();
			radioButtonARGB4444 = new System.Windows.Forms.RadioButton();
			radioButtonARGB1555 = new System.Windows.Forms.RadioButton();
			radioButtonRGB565 = new System.Windows.Forms.RadioButton();
			buttonAuto = new System.Windows.Forms.Button();
			buttonOK = new System.Windows.Forms.Button();
			buttonCancel = new System.Windows.Forms.Button();
			labelPaletteInfo = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			checkBoxInternalClut = new System.Windows.Forms.CheckBox();
			toolTip1 = new System.Windows.Forms.ToolTip(components);
			groupBoxMask.SuspendLayout();
			groupBoxPalette.SuspendLayout();
			SuspendLayout();
			// 
			// groupBoxMask
			// 
			groupBoxMask.AutoSize = true;
			groupBoxMask.Controls.Add(radioButtonNonIndexed);
			groupBoxMask.Controls.Add(radioButtonIndex8);
			groupBoxMask.Controls.Add(radioButtonIndex4);
			groupBoxMask.Location = new System.Drawing.Point(13, 81);
			groupBoxMask.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			groupBoxMask.Name = "groupBoxMask";
			groupBoxMask.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			groupBoxMask.Size = new System.Drawing.Size(154, 117);
			groupBoxMask.TabIndex = 0;
			groupBoxMask.TabStop = false;
			groupBoxMask.Text = "Texture Settings";
			// 
			// radioButtonNonIndexed
			// 
			radioButtonNonIndexed.AutoSize = true;
			radioButtonNonIndexed.Location = new System.Drawing.Point(7, 72);
			radioButtonNonIndexed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			radioButtonNonIndexed.Name = "radioButtonNonIndexed";
			radioButtonNonIndexed.Size = new System.Drawing.Size(94, 19);
			radioButtonNonIndexed.TabIndex = 3;
			radioButtonNonIndexed.TabStop = true;
			radioButtonNonIndexed.Text = "Non-indexed";
			toolTip1.SetToolTip(radioButtonNonIndexed, "Non-indexed textures do not use palettes.");
			radioButtonNonIndexed.UseVisualStyleBackColor = true;
			// 
			// radioButtonIndex8
			// 
			radioButtonIndex8.AutoSize = true;
			radioButtonIndex8.Location = new System.Drawing.Point(7, 47);
			radioButtonIndex8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			radioButtonIndex8.Name = "radioButtonIndex8";
			radioButtonIndex8.Size = new System.Drawing.Size(123, 19);
			radioButtonIndex8.TabIndex = 1;
			radioButtonIndex8.TabStop = true;
			radioButtonIndex8.Text = "Index8 (256 colors)";
			toolTip1.SetToolTip(radioButtonIndex8, "Index8 textures use one byte per pixel, and can have up to 256 different colors.");
			radioButtonIndex8.UseVisualStyleBackColor = true;
			// 
			// radioButtonIndex4
			// 
			radioButtonIndex4.AutoSize = true;
			radioButtonIndex4.Location = new System.Drawing.Point(7, 22);
			radioButtonIndex4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			radioButtonIndex4.Name = "radioButtonIndex4";
			radioButtonIndex4.Size = new System.Drawing.Size(117, 19);
			radioButtonIndex4.TabIndex = 0;
			radioButtonIndex4.TabStop = true;
			radioButtonIndex4.Text = "Index4 (16 colors)";
			toolTip1.SetToolTip(radioButtonIndex4, "Index4 textures use one byte per two pixels, and can have up to 16 different colors.");
			radioButtonIndex4.UseVisualStyleBackColor = true;
			// 
			// groupBoxPalette
			// 
			groupBoxPalette.AutoSize = true;
			groupBoxPalette.Controls.Add(radioButtonIntensity8A);
			groupBoxPalette.Controls.Add(radioButtonRGB5A3);
			groupBoxPalette.Controls.Add(radioButtonARGB8888);
			groupBoxPalette.Controls.Add(radioButtonARGB4444);
			groupBoxPalette.Controls.Add(radioButtonARGB1555);
			groupBoxPalette.Controls.Add(radioButtonRGB565);
			groupBoxPalette.Location = new System.Drawing.Point(186, 81);
			groupBoxPalette.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			groupBoxPalette.Name = "groupBoxPalette";
			groupBoxPalette.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			groupBoxPalette.Size = new System.Drawing.Size(238, 117);
			groupBoxPalette.TabIndex = 1;
			groupBoxPalette.TabStop = false;
			groupBoxPalette.Text = "Palette Format";
			// 
			// radioButtonIntensity8A
			// 
			radioButtonIntensity8A.AutoSize = true;
			radioButtonIntensity8A.Location = new System.Drawing.Point(117, 47);
			radioButtonIntensity8A.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			radioButtonIntensity8A.Name = "radioButtonIntensity8A";
			radioButtonIntensity8A.Size = new System.Drawing.Size(113, 19);
			radioButtonIntensity8A.TabIndex = 9;
			radioButtonIntensity8A.TabStop = true;
			radioButtonIntensity8A.Text = "Intensity8Alpha8";
			toolTip1.SetToolTip(radioButtonIntensity8A, "A lossless grayscale palette that can store partial transparency. 2 bytes per pixel.\r\nGamecube only. The Sonic Adventure games on the Gamecube do not support this format.");
			radioButtonIntensity8A.UseVisualStyleBackColor = true;
			// 
			// radioButtonRGB5A3
			// 
			radioButtonRGB5A3.AutoSize = true;
			radioButtonRGB5A3.Location = new System.Drawing.Point(117, 72);
			radioButtonRGB5A3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			radioButtonRGB5A3.Name = "radioButtonRGB5A3";
			radioButtonRGB5A3.Size = new System.Drawing.Size(67, 19);
			radioButtonRGB5A3.TabIndex = 8;
			radioButtonRGB5A3.TabStop = true;
			radioButtonRGB5A3.Text = "RGB5A3";
			toolTip1.SetToolTip(radioButtonRGB5A3, resources.GetString("radioButtonRGB5A3.ToolTip"));
			radioButtonRGB5A3.UseVisualStyleBackColor = true;
			// 
			// radioButtonARGB8888
			// 
			radioButtonARGB8888.AutoSize = true;
			radioButtonARGB8888.Location = new System.Drawing.Point(117, 22);
			radioButtonARGB8888.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			radioButtonARGB8888.Name = "radioButtonARGB8888";
			radioButtonARGB8888.Size = new System.Drawing.Size(79, 19);
			radioButtonARGB8888.TabIndex = 7;
			radioButtonARGB8888.TabStop = true;
			radioButtonARGB8888.Text = "ARGB8888";
			toolTip1.SetToolTip(radioButtonARGB8888, "Lossless palettes that take 4 bytes per color.");
			radioButtonARGB8888.UseVisualStyleBackColor = true;
			// 
			// radioButtonARGB4444
			// 
			radioButtonARGB4444.AutoSize = true;
			radioButtonARGB4444.Location = new System.Drawing.Point(8, 72);
			radioButtonARGB4444.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			radioButtonARGB4444.Name = "radioButtonARGB4444";
			radioButtonARGB4444.Size = new System.Drawing.Size(79, 19);
			radioButtonARGB4444.TabIndex = 6;
			radioButtonARGB4444.TabStop = true;
			radioButtonARGB4444.Text = "ARGB4444";
			toolTip1.SetToolTip(radioButtonARGB4444, "4 bits for Alpha, R, G and B. Good for palettes that need colors with partial transparency. 2 bytes per color.");
			radioButtonARGB4444.UseVisualStyleBackColor = true;
			// 
			// radioButtonARGB1555
			// 
			radioButtonARGB1555.AutoSize = true;
			radioButtonARGB1555.Location = new System.Drawing.Point(8, 47);
			radioButtonARGB1555.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			radioButtonARGB1555.Name = "radioButtonARGB1555";
			radioButtonARGB1555.Size = new System.Drawing.Size(79, 19);
			radioButtonARGB1555.TabIndex = 5;
			radioButtonARGB1555.TabStop = true;
			radioButtonARGB1555.Text = "ARGB1555";
			toolTip1.SetToolTip(radioButtonARGB1555, "5 bits for R, G and B, and 1 bit for Alpha. Good for palettes that have a fully transparent color. 2 bytes per color.");
			radioButtonARGB1555.UseVisualStyleBackColor = true;
			// 
			// radioButtonRGB565
			// 
			radioButtonRGB565.AutoSize = true;
			radioButtonRGB565.Location = new System.Drawing.Point(8, 22);
			radioButtonRGB565.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			radioButtonRGB565.Name = "radioButtonRGB565";
			radioButtonRGB565.Size = new System.Drawing.Size(65, 19);
			radioButtonRGB565.TabIndex = 4;
			radioButtonRGB565.TabStop = true;
			radioButtonRGB565.Text = "RGB565";
			toolTip1.SetToolTip(radioButtonRGB565, "5 bits for Red and Blue, and 6 bits for Green.\r\nGood for palettes that don't have transparency. 2 bytes per color.");
			radioButtonRGB565.UseVisualStyleBackColor = true;
			// 
			// buttonAuto
			// 
			buttonAuto.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			buttonAuto.Location = new System.Drawing.Point(14, 275);
			buttonAuto.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			buttonAuto.Name = "buttonAuto";
			buttonAuto.Size = new System.Drawing.Size(88, 27);
			buttonAuto.TabIndex = 2;
			buttonAuto.Text = "Auto";
			toolTip1.SetToolTip(buttonAuto, "Analyze the image to determine the most optimal texture and palette formats.");
			buttonAuto.UseVisualStyleBackColor = true;
			buttonAuto.Click += buttonAuto_Click;
			// 
			// buttonOK
			// 
			buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonOK.Location = new System.Drawing.Point(336, 275);
			buttonOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new System.Drawing.Size(88, 27);
			buttonOK.TabIndex = 3;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			buttonCancel.Location = new System.Drawing.Point(171, 275);
			buttonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new System.Drawing.Size(88, 27);
			buttonCancel.TabIndex = 4;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// labelPaletteInfo
			// 
			labelPaletteInfo.AutoSize = true;
			labelPaletteInfo.Location = new System.Drawing.Point(13, 243);
			labelPaletteInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelPaletteInfo.Name = "labelPaletteInfo";
			labelPaletteInfo.Size = new System.Drawing.Size(188, 15);
			labelPaletteInfo.TabIndex = 6;
			labelPaletteInfo.Text = "Palette: 256 colors (16 colors used)";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 11);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(370, 60);
			label1.TabIndex = 7;
			label1.Text = "The file you are trying to import is a paletted image.\r\nYou can import it as an Indexed PVR/GVR texture and load its palette.\r\n\r\nIf you just want to import a regular color texture, select Non-indexed.";
			// 
			// checkBoxInternalClut
			// 
			checkBoxInternalClut.AutoSize = true;
			checkBoxInternalClut.Location = new System.Drawing.Point(13, 204);
			checkBoxInternalClut.Name = "checkBoxInternalClut";
			checkBoxInternalClut.Size = new System.Drawing.Size(233, 19);
			checkBoxInternalClut.TabIndex = 10;
			checkBoxInternalClut.Text = "Import as an internal palette (GVR only)";
			toolTip1.SetToolTip(checkBoxInternalClut, "Saves the palette into the texture rather than an external palette file. Gamecube only.");
			checkBoxInternalClut.UseVisualStyleBackColor = true;
			checkBoxInternalClut.CheckedChanged += checkBoxInternalClut_CheckedChanged;
			// 
			// IndexedImageImportDialog
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(437, 316);
			Controls.Add(checkBoxInternalClut);
			Controls.Add(label1);
			Controls.Add(labelPaletteInfo);
			Controls.Add(buttonCancel);
			Controls.Add(buttonOK);
			Controls.Add(buttonAuto);
			Controls.Add(groupBoxPalette);
			Controls.Add(groupBoxMask);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "IndexedImageImportDialog";
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Indexed Image Import Options";
			FormClosing += IndexedImageImportDialog_FormClosing;
			Shown += IndexedImageImportDialog_Shown;
			groupBoxMask.ResumeLayout(false);
			groupBoxMask.PerformLayout();
			groupBoxPalette.ResumeLayout(false);
			groupBoxPalette.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxMask;
		private System.Windows.Forms.RadioButton radioButtonNonIndexed;
		private System.Windows.Forms.RadioButton radioButtonIndex8;
		private System.Windows.Forms.RadioButton radioButtonIndex4;
		private System.Windows.Forms.GroupBox groupBoxPalette;
		private System.Windows.Forms.RadioButton radioButtonIntensity8A;
		private System.Windows.Forms.RadioButton radioButtonRGB5A3;
		private System.Windows.Forms.RadioButton radioButtonARGB8888;
		private System.Windows.Forms.RadioButton radioButtonARGB4444;
		private System.Windows.Forms.RadioButton radioButtonARGB1555;
		private System.Windows.Forms.RadioButton radioButtonRGB565;
		private System.Windows.Forms.Button buttonAuto;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelPaletteInfo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxInternalClut;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}