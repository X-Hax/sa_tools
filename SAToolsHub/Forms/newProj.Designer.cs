namespace SAToolsHub
{
	partial class newProj
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
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.btnCreate = new System.Windows.Forms.Button();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.txtProjFolder = new System.Windows.Forms.TextBox();
			this.lblTemplate = new System.Windows.Forms.Label();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(12, 42);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(210, 20);
			this.checkBox1.TabIndex = 20;
			this.checkBox1.Text = "Save Files to a Different Folder";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// btnCreate
			// 
			this.btnCreate.Location = new System.Drawing.Point(12, 93);
			this.btnCreate.Name = "btnCreate";
			this.btnCreate.Size = new System.Drawing.Size(360, 23);
			this.btnCreate.TabIndex = 19;
			this.btnCreate.Text = "Create Project";
			this.btnCreate.UseVisualStyleBackColor = true;
			this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
			// 
			// btnBrowse
			// 
			this.btnBrowse.Enabled = false;
			this.btnBrowse.Location = new System.Drawing.Point(285, 64);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(87, 23);
			this.btnBrowse.TabIndex = 18;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnAltFolderBrowse_Click);
			// 
			// txtProjFolder
			// 
			this.txtProjFolder.Enabled = false;
			this.txtProjFolder.Location = new System.Drawing.Point(12, 65);
			this.txtProjFolder.Name = "txtProjFolder";
			this.txtProjFolder.Size = new System.Drawing.Size(267, 22);
			this.txtProjFolder.TabIndex = 17;
			// 
			// lblTemplate
			// 
			this.lblTemplate.AutoSize = true;
			this.lblTemplate.Location = new System.Drawing.Point(12, 15);
			this.lblTemplate.Name = "lblTemplate";
			this.lblTemplate.Size = new System.Drawing.Size(161, 16);
			this.lblTemplate.TabIndex = 21;
			this.lblTemplate.Text = "Select a Game Template:";
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(179, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(193, 24);
			this.comboBox1.TabIndex = 22;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// newProj
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 128);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.lblTemplate);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.btnCreate);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.txtProjFolder);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "newProj";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Project";
			this.Shown += new System.EventHandler(this.newProj_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button btnCreate;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox txtProjFolder;
		private System.Windows.Forms.Label lblTemplate;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ComboBox comboBox1;
	}
}