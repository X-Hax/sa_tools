
namespace SAToolsHub
{
	partial class resplitMenu
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
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.btnSplit = new System.Windows.Forms.Button();
			this.chkOverwrite = new System.Windows.Forms.CheckBox();
			this.unchkAll = new System.Windows.Forms.Button();
			this.chkAll = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkedListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Location = new System.Drawing.Point(0, 0);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(350, 395);
			this.checkedListBox1.TabIndex = 0;
			// 
			// btnSplit
			// 
			this.btnSplit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSplit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSplit.Location = new System.Drawing.Point(12, 430);
			this.btnSplit.Name = "btnSplit";
			this.btnSplit.Size = new System.Drawing.Size(326, 23);
			this.btnSplit.TabIndex = 1;
			this.btnSplit.Text = "Split Selected Items";
			this.btnSplit.UseVisualStyleBackColor = true;
			this.btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
			// 
			// chkOverwrite
			// 
			this.chkOverwrite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkOverwrite.AutoSize = true;
			this.chkOverwrite.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkOverwrite.Location = new System.Drawing.Point(12, 404);
			this.chkOverwrite.Name = "chkOverwrite";
			this.chkOverwrite.Size = new System.Drawing.Size(115, 20);
			this.chkOverwrite.TabIndex = 2;
			this.chkOverwrite.Text = "Overwrite Files";
			this.chkOverwrite.UseVisualStyleBackColor = true;
			this.chkOverwrite.CheckedChanged += new System.EventHandler(this.chkOverwrite_CheckedChanged);
			// 
			// unchkAll
			// 
			this.unchkAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.unchkAll.Location = new System.Drawing.Point(308, 401);
			this.unchkAll.Name = "unchkAll";
			this.unchkAll.Size = new System.Drawing.Size(30, 23);
			this.unchkAll.TabIndex = 3;
			this.unchkAll.Text = "X";
			this.unchkAll.UseVisualStyleBackColor = true;
			this.unchkAll.Click += new System.EventHandler(this.unchkAll_Click);
			// 
			// chkAll
			// 
			this.chkAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chkAll.Location = new System.Drawing.Point(272, 401);
			this.chkAll.Name = "chkAll";
			this.chkAll.Size = new System.Drawing.Size(30, 23);
			this.chkAll.TabIndex = 4;
			this.chkAll.Text = "✓";
			this.chkAll.UseVisualStyleBackColor = true;
			this.chkAll.Click += new System.EventHandler(this.chkAll_Click);
			// 
			// resplitMenu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(350, 462);
			this.Controls.Add(this.chkAll);
			this.Controls.Add(this.unchkAll);
			this.Controls.Add(this.chkOverwrite);
			this.Controls.Add(this.btnSplit);
			this.Controls.Add(this.checkedListBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "resplitMenu";
			this.Text = "Resplit Tool";
			this.Shown += new System.EventHandler(this.resplitMenu_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckedListBox checkedListBox1;
		private System.Windows.Forms.Button btnSplit;
		private System.Windows.Forms.CheckBox chkOverwrite;
		private System.Windows.Forms.Button unchkAll;
		private System.Windows.Forms.Button chkAll;
	}
}