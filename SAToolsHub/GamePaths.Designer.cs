namespace SAToolsHub
{
	partial class GamePaths
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
			this.SADXPath = new System.Windows.Forms.TextBox();
			this.btnSADXPath = new System.Windows.Forms.Button();
			this.btnSA2Path = new System.Windows.Forms.Button();
			this.SA2Path = new System.Windows.Forms.TextBox();
			this.btnSavePaths = new System.Windows.Forms.Button();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// SADXPath
			// 
			this.SADXPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SADXPath.Location = new System.Drawing.Point(12, 90);
			this.SADXPath.Name = "SADXPath";
			this.SADXPath.Size = new System.Drawing.Size(279, 22);
			this.SADXPath.TabIndex = 0;
			// 
			// btnSADXPath
			// 
			this.btnSADXPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSADXPath.Location = new System.Drawing.Point(297, 90);
			this.btnSADXPath.Name = "btnSADXPath";
			this.btnSADXPath.Size = new System.Drawing.Size(75, 23);
			this.btnSADXPath.TabIndex = 2;
			this.btnSADXPath.Text = "Browse";
			this.btnSADXPath.UseVisualStyleBackColor = true;
			this.btnSADXPath.Click += new System.EventHandler(this.btnSADXPath_Click);
			// 
			// btnSA2Path
			// 
			this.btnSA2Path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSA2Path.Location = new System.Drawing.Point(297, 204);
			this.btnSA2Path.Name = "btnSA2Path";
			this.btnSA2Path.Size = new System.Drawing.Size(75, 23);
			this.btnSA2Path.TabIndex = 5;
			this.btnSA2Path.Text = "Browse";
			this.btnSA2Path.UseVisualStyleBackColor = true;
			this.btnSA2Path.Click += new System.EventHandler(this.btnSA2Path_Click);
			// 
			// SA2Path
			// 
			this.SA2Path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SA2Path.Location = new System.Drawing.Point(12, 204);
			this.SA2Path.Name = "SA2Path";
			this.SA2Path.Size = new System.Drawing.Size(279, 22);
			this.SA2Path.TabIndex = 3;
			// 
			// btnSavePaths
			// 
			this.btnSavePaths.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnSavePaths.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSavePaths.Location = new System.Drawing.Point(128, 236);
			this.btnSavePaths.Name = "btnSavePaths";
			this.btnSavePaths.Size = new System.Drawing.Size(128, 23);
			this.btnSavePaths.TabIndex = 8;
			this.btnSavePaths.Text = "Save Path Setup";
			this.btnSavePaths.UseVisualStyleBackColor = true;
			this.btnSavePaths.Click += new System.EventHandler(this.btnSavePaths_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::SAToolsHub.Properties.Resources.SADXBANNER;
			this.pictureBox2.Location = new System.Drawing.Point(12, 12);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(360, 72);
			this.pictureBox2.TabIndex = 10;
			this.pictureBox2.TabStop = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::SAToolsHub.Properties.Resources.SA2BANNER;
			this.pictureBox1.Location = new System.Drawing.Point(12, 126);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(360, 72);
			this.pictureBox1.TabIndex = 9;
			this.pictureBox1.TabStop = false;
			// 
			// GamePaths
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 271);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.btnSavePaths);
			this.Controls.Add(this.btnSA2Path);
			this.Controls.Add(this.SA2Path);
			this.Controls.Add(this.btnSADXPath);
			this.Controls.Add(this.SADXPath);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "GamePaths";
			this.Text = "Game Installation Paths";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox SADXPath;
		private System.Windows.Forms.Button btnSADXPath;
		private System.Windows.Forms.Button btnSA2Path;
		private System.Windows.Forms.TextBox SA2Path;
		private System.Windows.Forms.Button btnSavePaths;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
	}
}