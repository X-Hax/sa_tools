namespace SonicRetro.SAModel.SADXLVL2
{
	partial class QuickStartDialog
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
			this.buttonOpen = new System.Windows.Forms.Button();
			this.buttonGo = new System.Windows.Forms.Button();
			this.listRecentFiles = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonOpen
			// 
			this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOpen.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonOpen.Location = new System.Drawing.Point(297, 26);
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.Size = new System.Drawing.Size(75, 23);
			this.buttonOpen.TabIndex = 1;
			this.buttonOpen.Text = "&Open";
			this.buttonOpen.UseVisualStyleBackColor = true;
			this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
			// 
			// buttonGo
			// 
			this.buttonGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonGo.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonGo.Enabled = false;
			this.buttonGo.Location = new System.Drawing.Point(297, 55);
			this.buttonGo.Name = "buttonGo";
			this.buttonGo.Size = new System.Drawing.Size(75, 23);
			this.buttonGo.TabIndex = 2;
			this.buttonGo.Text = "&Go";
			this.buttonGo.UseVisualStyleBackColor = true;
			// 
			// listRecentFiles
			// 
			this.listRecentFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listRecentFiles.FormattingEnabled = true;
			this.listRecentFiles.HorizontalScrollbar = true;
			this.listRecentFiles.Location = new System.Drawing.Point(13, 26);
			this.listRecentFiles.Name = "listRecentFiles";
			this.listRecentFiles.Size = new System.Drawing.Size(278, 225);
			this.listRecentFiles.TabIndex = 0;
			this.listRecentFiles.SelectedIndexChanged += new System.EventHandler(this.listRecentFiles_SelectedIndexChanged);
			this.listRecentFiles.DoubleClick += new System.EventHandler(this.listRecentFiles_DoubleClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Recent Projects:";
			// 
			// QuickStartDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 262);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listRecentFiles);
			this.Controls.Add(this.buttonGo);
			this.Controls.Add(this.buttonOpen);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 300);
			this.Name = "QuickStartDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Quick Start";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOpen;
		private System.Windows.Forms.Button buttonGo;
		private System.Windows.Forms.ListBox listRecentFiles;
		private System.Windows.Forms.Label label1;
	}
}