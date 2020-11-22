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
			this.btnTemplateSelect = new System.Windows.Forms.Button();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox1.Location = new System.Drawing.Point(12, 32);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(210, 20);
			this.checkBox1.TabIndex = 20;
			this.checkBox1.Text = "Save Files to a Different Folder";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// btnCreate
			// 
			this.btnCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCreate.Location = new System.Drawing.Point(12, 87);
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
			this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnBrowse.Location = new System.Drawing.Point(285, 58);
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
			this.txtProjFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtProjFolder.Location = new System.Drawing.Point(12, 58);
			this.txtProjFolder.Name = "txtProjFolder";
			this.txtProjFolder.Size = new System.Drawing.Size(267, 22);
			this.txtProjFolder.TabIndex = 17;
			// 
			// lblTemplate
			// 
			this.lblTemplate.AutoSize = true;
			this.lblTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTemplate.Location = new System.Drawing.Point(12, 9);
			this.lblTemplate.Name = "lblTemplate";
			this.lblTemplate.Size = new System.Drawing.Size(147, 16);
			this.lblTemplate.TabIndex = 21;
			this.lblTemplate.Text = "Select Game Template";
			this.lblTemplate.TextChanged += new System.EventHandler(this.lblTemplate_TextChanged);
			// 
			// btnTemplateSelect
			// 
			this.btnTemplateSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnTemplateSelect.Location = new System.Drawing.Point(251, 6);
			this.btnTemplateSelect.Name = "btnTemplateSelect";
			this.btnTemplateSelect.Size = new System.Drawing.Size(121, 23);
			this.btnTemplateSelect.TabIndex = 26;
			this.btnTemplateSelect.Text = "Select Template";
			this.btnTemplateSelect.UseVisualStyleBackColor = true;
			this.btnTemplateSelect.Click += new System.EventHandler(this.btnTemplateSelect_Click);
			// 
			// newNEWproj
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 121);
			this.Controls.Add(this.btnTemplateSelect);
			this.Controls.Add(this.lblTemplate);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.btnCreate);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.txtProjFolder);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "newNEWproj";
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
		private System.Windows.Forms.Button btnTemplateSelect;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
	}
}