
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
            this.groupBoxMask = new System.Windows.Forms.GroupBox();
            this.radioButtonNonIndexed = new System.Windows.Forms.RadioButton();
            this.radioButtonIndex8 = new System.Windows.Forms.RadioButton();
            this.radioButtonIndex4 = new System.Windows.Forms.RadioButton();
            this.groupBoxPalette = new System.Windows.Forms.GroupBox();
            this.radioButtonIntensity8A = new System.Windows.Forms.RadioButton();
            this.radioButtonRGB5A3 = new System.Windows.Forms.RadioButton();
            this.radioButtonARGB8888 = new System.Windows.Forms.RadioButton();
            this.radioButtonARGB4444 = new System.Windows.Forms.RadioButton();
            this.radioButtonARGB1555 = new System.Windows.Forms.RadioButton();
            this.radioButtonRGB565 = new System.Windows.Forms.RadioButton();
            this.buttonAuto = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelPaletteInfo = new System.Windows.Forms.Label();
            this.groupBoxMask.SuspendLayout();
            this.groupBoxPalette.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMask
            // 
            this.groupBoxMask.AutoSize = true;
            this.groupBoxMask.Controls.Add(this.radioButtonNonIndexed);
            this.groupBoxMask.Controls.Add(this.radioButtonIndex8);
            this.groupBoxMask.Controls.Add(this.radioButtonIndex4);
            this.groupBoxMask.Location = new System.Drawing.Point(9, 9);
            this.groupBoxMask.Name = "groupBoxMask";
            this.groupBoxMask.Size = new System.Drawing.Size(132, 101);
            this.groupBoxMask.TabIndex = 0;
            this.groupBoxMask.TabStop = false;
            this.groupBoxMask.Text = "Mask Settings";
            // 
            // radioButtonNonIndexed
            // 
            this.radioButtonNonIndexed.AutoSize = true;
            this.radioButtonNonIndexed.Location = new System.Drawing.Point(6, 65);
            this.radioButtonNonIndexed.Name = "radioButtonNonIndexed";
            this.radioButtonNonIndexed.Size = new System.Drawing.Size(119, 17);
            this.radioButtonNonIndexed.TabIndex = 3;
            this.radioButtonNonIndexed.TabStop = true;
            this.radioButtonNonIndexed.Text = "Non-indexed bitmap";
            this.radioButtonNonIndexed.UseVisualStyleBackColor = true;
            // 
            // radioButtonIndex8
            // 
            this.radioButtonIndex8.AutoSize = true;
            this.radioButtonIndex8.Location = new System.Drawing.Point(6, 42);
            this.radioButtonIndex8.Name = "radioButtonIndex8";
            this.radioButtonIndex8.Size = new System.Drawing.Size(115, 17);
            this.radioButtonIndex8.TabIndex = 1;
            this.radioButtonIndex8.TabStop = true;
            this.radioButtonIndex8.Text = "Index8 (256 colors)";
            this.radioButtonIndex8.UseVisualStyleBackColor = true;
            // 
            // radioButtonIndex4
            // 
            this.radioButtonIndex4.AutoSize = true;
            this.radioButtonIndex4.Location = new System.Drawing.Point(6, 19);
            this.radioButtonIndex4.Name = "radioButtonIndex4";
            this.radioButtonIndex4.Size = new System.Drawing.Size(109, 17);
            this.radioButtonIndex4.TabIndex = 0;
            this.radioButtonIndex4.TabStop = true;
            this.radioButtonIndex4.Text = "Index4 (16 colors)";
            this.radioButtonIndex4.UseVisualStyleBackColor = true;
            // 
            // groupBoxPalette
            // 
            this.groupBoxPalette.AutoSize = true;
            this.groupBoxPalette.Controls.Add(this.radioButtonIntensity8A);
            this.groupBoxPalette.Controls.Add(this.radioButtonRGB5A3);
            this.groupBoxPalette.Controls.Add(this.radioButtonARGB8888);
            this.groupBoxPalette.Controls.Add(this.radioButtonARGB4444);
            this.groupBoxPalette.Controls.Add(this.radioButtonARGB1555);
            this.groupBoxPalette.Controls.Add(this.radioButtonRGB565);
            this.groupBoxPalette.Location = new System.Drawing.Point(157, 9);
            this.groupBoxPalette.Name = "groupBoxPalette";
            this.groupBoxPalette.Size = new System.Drawing.Size(203, 101);
            this.groupBoxPalette.TabIndex = 1;
            this.groupBoxPalette.TabStop = false;
            this.groupBoxPalette.Text = "Palette Format";
            // 
            // radioButtonIntensity8A
            // 
            this.radioButtonIntensity8A.AutoSize = true;
            this.radioButtonIntensity8A.Location = new System.Drawing.Point(100, 42);
            this.radioButtonIntensity8A.Name = "radioButtonIntensity8A";
            this.radioButtonIntensity8A.Size = new System.Drawing.Size(97, 17);
            this.radioButtonIntensity8A.TabIndex = 9;
            this.radioButtonIntensity8A.TabStop = true;
            this.radioButtonIntensity8A.Text = "Intensity8Alpha";
            this.radioButtonIntensity8A.UseVisualStyleBackColor = true;
            // 
            // radioButtonRGB5A3
            // 
            this.radioButtonRGB5A3.AutoSize = true;
            this.radioButtonRGB5A3.Location = new System.Drawing.Point(100, 65);
            this.radioButtonRGB5A3.Name = "radioButtonRGB5A3";
            this.radioButtonRGB5A3.Size = new System.Drawing.Size(67, 17);
            this.radioButtonRGB5A3.TabIndex = 8;
            this.radioButtonRGB5A3.TabStop = true;
            this.radioButtonRGB5A3.Text = "RGB5A3";
            this.radioButtonRGB5A3.UseVisualStyleBackColor = true;
            // 
            // radioButtonARGB8888
            // 
            this.radioButtonARGB8888.AutoSize = true;
            this.radioButtonARGB8888.Location = new System.Drawing.Point(100, 19);
            this.radioButtonARGB8888.Name = "radioButtonARGB8888";
            this.radioButtonARGB8888.Size = new System.Drawing.Size(79, 17);
            this.radioButtonARGB8888.TabIndex = 7;
            this.radioButtonARGB8888.TabStop = true;
            this.radioButtonARGB8888.Text = "ARGB8888";
            this.radioButtonARGB8888.UseVisualStyleBackColor = true;
            // 
            // radioButtonARGB4444
            // 
            this.radioButtonARGB4444.AutoSize = true;
            this.radioButtonARGB4444.Location = new System.Drawing.Point(6, 65);
            this.radioButtonARGB4444.Name = "radioButtonARGB4444";
            this.radioButtonARGB4444.Size = new System.Drawing.Size(79, 17);
            this.radioButtonARGB4444.TabIndex = 6;
            this.radioButtonARGB4444.TabStop = true;
            this.radioButtonARGB4444.Text = "ARGB4444";
            this.radioButtonARGB4444.UseVisualStyleBackColor = true;
            // 
            // radioButtonARGB1555
            // 
            this.radioButtonARGB1555.AutoSize = true;
            this.radioButtonARGB1555.Location = new System.Drawing.Point(6, 42);
            this.radioButtonARGB1555.Name = "radioButtonARGB1555";
            this.radioButtonARGB1555.Size = new System.Drawing.Size(79, 17);
            this.radioButtonARGB1555.TabIndex = 5;
            this.radioButtonARGB1555.TabStop = true;
            this.radioButtonARGB1555.Text = "ARGB1555";
            this.radioButtonARGB1555.UseVisualStyleBackColor = true;
            // 
            // radioButtonRGB565
            // 
            this.radioButtonRGB565.AutoSize = true;
            this.radioButtonRGB565.Location = new System.Drawing.Point(5, 20);
            this.radioButtonRGB565.Name = "radioButtonRGB565";
            this.radioButtonRGB565.Size = new System.Drawing.Size(66, 17);
            this.radioButtonRGB565.TabIndex = 4;
            this.radioButtonRGB565.TabStop = true;
            this.radioButtonRGB565.Text = "RGB565";
            this.radioButtonRGB565.UseVisualStyleBackColor = true;
            // 
            // buttonAuto
            // 
            this.buttonAuto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAuto.Location = new System.Drawing.Point(12, 133);
            this.buttonAuto.Name = "buttonAuto";
            this.buttonAuto.Size = new System.Drawing.Size(75, 23);
            this.buttonAuto.TabIndex = 2;
            this.buttonAuto.Text = "Auto";
            this.buttonAuto.UseVisualStyleBackColor = true;
            this.buttonAuto.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(282, 133);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(144, 133);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelPaletteInfo
            // 
            this.labelPaletteInfo.AutoSize = true;
            this.labelPaletteInfo.Location = new System.Drawing.Point(12, 112);
            this.labelPaletteInfo.Name = "labelPaletteInfo";
            this.labelPaletteInfo.Size = new System.Drawing.Size(173, 13);
            this.labelPaletteInfo.TabIndex = 6;
            this.labelPaletteInfo.Text = "Palette: 256 colors (16 colors used)";
            // 
            // IndexedImageImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 168);
            this.Controls.Add(this.labelPaletteInfo);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonAuto);
            this.Controls.Add(this.groupBoxPalette);
            this.Controls.Add(this.groupBoxMask);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "IndexedImageImportDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Indexed Image Import Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IndexedImageImportDialog_FormClosing);
            this.Shown += new System.EventHandler(this.IndexedImageImportDialog_Shown);
            this.groupBoxMask.ResumeLayout(false);
            this.groupBoxMask.PerformLayout();
            this.groupBoxPalette.ResumeLayout(false);
            this.groupBoxPalette.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
	}
}