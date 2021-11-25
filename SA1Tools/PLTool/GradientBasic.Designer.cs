
namespace PLTool
{
	partial class GradientBasic
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
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.pictureBoxColor1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxColor2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColor1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColor2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(44, 96);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(58, 24);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(121, 96);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(58, 24);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // pictureBoxColor1
            // 
            this.pictureBoxColor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxColor1.Location = new System.Drawing.Point(16, 16);
            this.pictureBoxColor1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBoxColor1.Name = "pictureBoxColor1";
            this.pictureBoxColor1.Size = new System.Drawing.Size(25, 24);
            this.pictureBoxColor1.TabIndex = 2;
            this.pictureBoxColor1.TabStop = false;
            this.pictureBoxColor1.Click += new System.EventHandler(this.pictureBoxColor1_Click);
            // 
            // pictureBoxColor2
            // 
            this.pictureBoxColor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxColor2.Location = new System.Drawing.Point(191, 16);
            this.pictureBoxColor2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBoxColor2.Name = "pictureBoxColor2";
            this.pictureBoxColor2.Size = new System.Drawing.Size(25, 24);
            this.pictureBoxColor2.TabIndex = 3;
            this.pictureBoxColor2.TabStop = false;
            this.pictureBoxColor2.Click += new System.EventHandler(this.pictureBoxColor2_Click);
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPreview.Location = new System.Drawing.Point(16, 57);
            this.pictureBoxPreview.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(200, 24);
            this.pictureBoxPreview.TabIndex = 4;
            this.pictureBoxPreview.TabStop = false;
            // 
            // GradientBasic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(233, 131);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.pictureBoxColor2);
            this.Controls.Add(this.pictureBoxColor1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GradientBasic";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Basic Gradient";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColor1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColor2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.PictureBox pictureBoxColor1;
		private System.Windows.Forms.PictureBox pictureBoxColor2;
		private System.Windows.Forms.PictureBox pictureBoxPreview;
	}
}