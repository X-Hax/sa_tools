namespace SAToolsHub
{
	partial class buildWindow
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
			this.btnManual = new System.Windows.Forms.Button();
			this.btnAuto = new System.Windows.Forms.Button();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.chkAll = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// btnManual
			// 
			this.btnManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnManual.Location = new System.Drawing.Point(12, 269);
			this.btnManual.Name = "btnManual";
			this.btnManual.Size = new System.Drawing.Size(135, 33);
			this.btnManual.TabIndex = 0;
			this.btnManual.Text = "Manual";
			this.toolTip1.SetToolTip(this.btnManual, "Opens a window for you to manually assets and export them to an INI mod or to C c" +
        "ode.");
			this.btnManual.UseVisualStyleBackColor = true;
			this.btnManual.Click += new System.EventHandler(this.btnManual_Click);
			// 
			// btnAuto
			// 
			this.btnAuto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAuto.Location = new System.Drawing.Point(167, 269);
			this.btnAuto.Name = "btnAuto";
			this.btnAuto.Size = new System.Drawing.Size(135, 33);
			this.btnAuto.TabIndex = 1;
			this.btnAuto.Text = "Automatic";
			this.toolTip1.SetToolTip(this.btnAuto, "Automatically compiles modified assets into an INI mod.");
			this.btnAuto.UseVisualStyleBackColor = true;
			this.btnAuto.Click += new System.EventHandler(this.btnAuto_Click);
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.CheckOnClick = true;
			this.checkedListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Location = new System.Drawing.Point(12, 38);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(290, 225);
			this.checkedListBox1.TabIndex = 3;
			// 
			// chkAll
			// 
			this.chkAll.AutoSize = true;
			this.chkAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkAll.Location = new System.Drawing.Point(12, 12);
			this.chkAll.Name = "chkAll";
			this.chkAll.Size = new System.Drawing.Size(83, 20);
			this.chkAll.TabIndex = 4;
			this.chkAll.Text = "Check All";
			this.toolTip1.SetToolTip(this.chkAll, "Check all items (This may be slow)");
			this.chkAll.UseVisualStyleBackColor = true;
			this.chkAll.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// buildWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(314, 312);
			this.Controls.Add(this.chkAll);
			this.Controls.Add(this.checkedListBox1);
			this.Controls.Add(this.btnAuto);
			this.Controls.Add(this.btnManual);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "buildWindow";
			this.Text = "Build Mod";
			this.Shown += new System.EventHandler(this.buildWindow_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnManual;
		private System.Windows.Forms.Button btnAuto;
		private System.Windows.Forms.CheckedListBox checkedListBox1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBox chkAll;
	}
}