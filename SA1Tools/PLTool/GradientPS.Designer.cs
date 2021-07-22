
namespace PLTool
{
	partial class GradientPS
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
            this.components = new System.ComponentModel.Container();
            this.pictureBoxNewPalette = new System.Windows.Forms.PictureBox();
            this.pictureBoxCurrentPalette = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.trackBarColor1 = new System.Windows.Forms.TrackBar();
            this.trackBarColor2 = new System.Windows.Forms.TrackBar();
            this.pictureBoxLeftColor = new System.Windows.Forms.PictureBox();
            this.pictureBoxAmbientColor = new System.Windows.Forms.PictureBox();
            this.pictureBoxRightColor = new System.Windows.Forms.PictureBox();
            this.pictureBoxLeftPreview = new System.Windows.Forms.PictureBox();
            this.pictureBoxRightPreview = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelColor1 = new System.Windows.Forms.Label();
            this.labelColor2 = new System.Windows.Forms.Label();
            this.buttonPresets = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TwoColors = new System.Windows.Forms.ToolStripMenuItem();
            this.ThreeColors = new System.Windows.Forms.ToolStripMenuItem();
            this.FourColors85 = new System.Windows.Forms.ToolStripMenuItem();
            this.FourColors64 = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoColors = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNewPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarColor1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarColor2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeftColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAmbientColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRightColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeftPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRightPreview)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxNewPalette
            // 
            this.pictureBoxNewPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxNewPalette.Location = new System.Drawing.Point(49, 102);
            this.pictureBoxNewPalette.Name = "pictureBoxNewPalette";
            this.pictureBoxNewPalette.Size = new System.Drawing.Size(256, 32);
            this.pictureBoxNewPalette.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxNewPalette.TabIndex = 1;
            this.pictureBoxNewPalette.TabStop = false;
            // 
            // pictureBoxCurrentPalette
            // 
            this.pictureBoxCurrentPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCurrentPalette.Location = new System.Drawing.Point(49, 31);
            this.pictureBoxCurrentPalette.Name = "pictureBoxCurrentPalette";
            this.pictureBoxCurrentPalette.Size = new System.Drawing.Size(256, 32);
            this.pictureBoxCurrentPalette.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCurrentPalette.TabIndex = 0;
            this.pictureBoxCurrentPalette.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(31, 159);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(31, 210);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // trackBarColor1
            // 
            this.trackBarColor1.Location = new System.Drawing.Point(69, 159);
            this.trackBarColor1.Maximum = 255;
            this.trackBarColor1.Name = "trackBarColor1";
            this.trackBarColor1.Size = new System.Drawing.Size(236, 45);
            this.trackBarColor1.TabIndex = 4;
            this.trackBarColor1.TickFrequency = 0;
            this.trackBarColor1.Value = 85;
            this.trackBarColor1.ValueChanged += new System.EventHandler(this.trackBarColor1_ValueChanged);
            // 
            // trackBarColor2
            // 
            this.trackBarColor2.Location = new System.Drawing.Point(69, 210);
            this.trackBarColor2.Maximum = 255;
            this.trackBarColor2.Name = "trackBarColor2";
            this.trackBarColor2.Size = new System.Drawing.Size(236, 45);
            this.trackBarColor2.TabIndex = 5;
            this.trackBarColor2.TickFrequency = 0;
            this.trackBarColor2.Value = 170;
            this.trackBarColor2.ValueChanged += new System.EventHandler(this.trackBarColor1_ValueChanged);
            // 
            // pictureBoxLeftColor
            // 
            this.pictureBoxLeftColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxLeftColor.Location = new System.Drawing.Point(31, 280);
            this.pictureBoxLeftColor.Name = "pictureBoxLeftColor";
            this.pictureBoxLeftColor.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxLeftColor.TabIndex = 6;
            this.pictureBoxLeftColor.TabStop = false;
            this.pictureBoxLeftColor.Click += new System.EventHandler(this.pictureBoxLeftColor_Click);
            // 
            // pictureBoxAmbientColor
            // 
            this.pictureBoxAmbientColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxAmbientColor.Location = new System.Drawing.Point(156, 280);
            this.pictureBoxAmbientColor.Name = "pictureBoxAmbientColor";
            this.pictureBoxAmbientColor.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxAmbientColor.TabIndex = 7;
            this.pictureBoxAmbientColor.TabStop = false;
            this.pictureBoxAmbientColor.Click += new System.EventHandler(this.pictureBoxAmbientColor_Click);
            // 
            // pictureBoxRightColor
            // 
            this.pictureBoxRightColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxRightColor.Location = new System.Drawing.Point(295, 280);
            this.pictureBoxRightColor.Name = "pictureBoxRightColor";
            this.pictureBoxRightColor.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxRightColor.TabIndex = 8;
            this.pictureBoxRightColor.TabStop = false;
            this.pictureBoxRightColor.Click += new System.EventHandler(this.pictureBoxRightColor_Click);
            // 
            // pictureBoxLeftPreview
            // 
            this.pictureBoxLeftPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxLeftPreview.Location = new System.Drawing.Point(27, 102);
            this.pictureBoxLeftPreview.Name = "pictureBoxLeftPreview";
            this.pictureBoxLeftPreview.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxLeftPreview.TabIndex = 9;
            this.pictureBoxLeftPreview.TabStop = false;
            this.pictureBoxLeftPreview.Click += new System.EventHandler(this.pictureBoxLeftPreview_Click);
            // 
            // pictureBoxRightPreview
            // 
            this.pictureBoxRightPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxRightPreview.Location = new System.Drawing.Point(311, 102);
            this.pictureBoxRightPreview.Name = "pictureBoxRightPreview";
            this.pictureBoxRightPreview.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxRightPreview.TabIndex = 10;
            this.pictureBoxRightPreview.TabStop = false;
            this.pictureBoxRightPreview.Click += new System.EventHandler(this.pictureBoxRightPreview_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Current Palette:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "New Palette:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 264);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Left Color:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(137, 264);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Ambient Color:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(278, 264);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Right Color:";
            // 
            // labelColor1
            // 
            this.labelColor1.AutoSize = true;
            this.labelColor1.Location = new System.Drawing.Point(311, 163);
            this.labelColor1.Name = "labelColor1";
            this.labelColor1.Size = new System.Drawing.Size(19, 13);
            this.labelColor1.TabIndex = 16;
            this.labelColor1.Text = "85";
            // 
            // labelColor2
            // 
            this.labelColor2.AutoSize = true;
            this.labelColor2.Location = new System.Drawing.Point(311, 216);
            this.labelColor2.Name = "labelColor2";
            this.labelColor2.Size = new System.Drawing.Size(25, 13);
            this.labelColor2.TabIndex = 17;
            this.labelColor2.Text = "170";
            // 
            // buttonPresets
            // 
            this.buttonPresets.Location = new System.Drawing.Point(22, 337);
            this.buttonPresets.Name = "buttonPresets";
            this.buttonPresets.Size = new System.Drawing.Size(75, 23);
            this.buttonPresets.TabIndex = 18;
            this.buttonPresets.Text = "Presets";
            this.buttonPresets.UseVisualStyleBackColor = true;
            this.buttonPresets.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSave.Location = new System.Drawing.Point(137, 337);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 19;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(261, 337);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 20;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // colorDialog1
            // 
            this.colorDialog1.AnyColor = true;
            this.colorDialog1.FullOpen = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TwoColors,
            this.ThreeColors,
            this.FourColors85,
            this.FourColors64,
            this.AutoColors});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(260, 114);
            // 
            // TwoColors
            // 
            this.TwoColors.Name = "TwoColors";
            this.TwoColors.Size = new System.Drawing.Size(259, 22);
            this.TwoColors.Text = "2 colors (0, 255)";
            this.TwoColors.Click += new System.EventHandler(this.TwoColors_Click);
            // 
            // ThreeColors
            // 
            this.ThreeColors.Name = "ThreeColors";
            this.ThreeColors.Size = new System.Drawing.Size(259, 22);
            this.ThreeColors.Text = "3 colors (0, 127, 255)";
            this.ThreeColors.Click += new System.EventHandler(this.ThreeColors_Click);
            // 
            // FourColors85
            // 
            this.FourColors85.Name = "FourColors85";
            this.FourColors85.Size = new System.Drawing.Size(259, 22);
            this.FourColors85.Text = "4 colors (0, 85, 170, 255)";
            this.FourColors85.Click += new System.EventHandler(this.FourColors85_Click);
            // 
            // FourColors64
            // 
            this.FourColors64.Name = "FourColors64";
            this.FourColors64.Size = new System.Drawing.Size(259, 22);
            this.FourColors64.Text = "4 colors (0, 64, 192, 255)";
            this.FourColors64.Click += new System.EventHandler(this.FourColors64_Click);
            // 
            // AutoColors
            // 
            this.AutoColors.Name = "AutoColors";
            this.AutoColors.Size = new System.Drawing.Size(259, 22);
            this.AutoColors.Text = "Use current values (0, 127, 127, 255)";
            this.AutoColors.Click += new System.EventHandler(this.AutoColors_Click);
            // 
            // GradientPS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 377);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonPresets);
            this.Controls.Add(this.labelColor2);
            this.Controls.Add(this.labelColor1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxRightPreview);
            this.Controls.Add(this.pictureBoxLeftPreview);
            this.Controls.Add(this.pictureBoxRightColor);
            this.Controls.Add(this.pictureBoxAmbientColor);
            this.Controls.Add(this.pictureBoxLeftColor);
            this.Controls.Add(this.trackBarColor2);
            this.Controls.Add(this.trackBarColor1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBoxNewPalette);
            this.Controls.Add(this.pictureBoxCurrentPalette);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GradientPS";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generate Gradient";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNewPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarColor1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarColor2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeftColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAmbientColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRightColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeftPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRightPreview)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxCurrentPalette;
		private System.Windows.Forms.PictureBox pictureBoxNewPalette;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.TrackBar trackBarColor1;
		private System.Windows.Forms.TrackBar trackBarColor2;
		private System.Windows.Forms.PictureBox pictureBoxLeftColor;
		private System.Windows.Forms.PictureBox pictureBoxAmbientColor;
		private System.Windows.Forms.PictureBox pictureBoxRightColor;
		private System.Windows.Forms.PictureBox pictureBoxLeftPreview;
		private System.Windows.Forms.PictureBox pictureBoxRightPreview;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelColor1;
		private System.Windows.Forms.Label labelColor2;
		private System.Windows.Forms.Button buttonPresets;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem TwoColors;
		private System.Windows.Forms.ToolStripMenuItem ThreeColors;
		private System.Windows.Forms.ToolStripMenuItem FourColors85;
		private System.Windows.Forms.ToolStripMenuItem FourColors64;
		private System.Windows.Forms.ToolStripMenuItem AutoColors;
	}
}