namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	partial class TextureRemappingDialog
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SourceTileList = new System.Windows.Forms.ListView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.DestinationTileList = new System.Windows.Forms.ListView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.button2 = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 14);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(41, 13);
			label1.TabIndex = 9;
			label1.Text = "Source";
			// 
			// label2
			// 
			label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(740, 14);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(60, 13);
			label2.TabIndex = 10;
			label2.Text = "Destination";
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(0, 0);
			this.okButton.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "&OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(81, 0);
			this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// SourceTileList
			// 
			this.SourceTileList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.SourceTileList.BackColor = System.Drawing.SystemColors.Window;
			this.SourceTileList.HideSelection = false;
			this.SourceTileList.LargeImageList = this.imageList1;
			this.SourceTileList.Location = new System.Drawing.Point(12, 38);
			this.SourceTileList.MultiSelect = false;
			this.SourceTileList.Name = "SourceTileList";
			this.SourceTileList.Size = new System.Drawing.Size(300, 422);
			this.SourceTileList.TabIndex = 2;
			this.SourceTileList.UseCompatibleStateImageBehavior = false;
			this.SourceTileList.SelectedIndexChanged += new System.EventHandler(this.TileList_SelectedIndexChanged);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(128, 128);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// DestinationTileList
			// 
			this.DestinationTileList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.DestinationTileList.BackColor = System.Drawing.SystemColors.Window;
			this.DestinationTileList.HideSelection = false;
			this.DestinationTileList.LargeImageList = this.imageList1;
			this.DestinationTileList.Location = new System.Drawing.Point(500, 38);
			this.DestinationTileList.MultiSelect = false;
			this.DestinationTileList.Name = "DestinationTileList";
			this.DestinationTileList.Size = new System.Drawing.Size(300, 422);
			this.DestinationTileList.TabIndex = 3;
			this.DestinationTileList.UseCompatibleStateImageBehavior = false;
			this.DestinationTileList.SelectedIndexChanged += new System.EventHandler(this.TileList_SelectedIndexChanged);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.okButton);
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Location = new System.Drawing.Point(328, 437);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(156, 23);
			this.panel1.TabIndex = 4;
			// 
			// button1
			// 
			this.button1.Enabled = false;
			this.button1.Location = new System.Drawing.Point(318, 9);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "&Add";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.IntegralHeight = false;
			this.listBox1.Location = new System.Drawing.Point(318, 38);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(176, 393);
			this.listBox1.TabIndex = 8;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Enabled = false;
			this.button2.Location = new System.Drawing.Point(419, 9);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 11;
			this.button2.Text = "&Remove";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// TextureRemappingDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(812, 472);
			this.Controls.Add(this.button2);
			this.Controls.Add(label2);
			this.Controls.Add(label1);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.DestinationTileList);
			this.Controls.Add(this.SourceTileList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TextureRemappingDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Texture Remapping";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ListView SourceTileList;
		private System.Windows.Forms.ListView DestinationTileList;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ImageList imageList1;
	}
}

