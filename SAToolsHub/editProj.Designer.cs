namespace SAToolsHub
{
	partial class editProj
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(editProj));
			this.radSADX = new System.Windows.Forms.RadioButton();
			this.radSA2 = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chkChaoRedir = new System.Windows.Forms.CheckBox();
			this.chkMainRedir = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtDLLName = new System.Windows.Forms.TextBox();
			this.chkDLLFile = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.txtAuth = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtDesc = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtVerNum = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnCreateSave = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// radSADX
			// 
			resources.ApplyResources(this.radSADX, "radSADX");
			this.radSADX.Name = "radSADX";
			this.radSADX.TabStop = true;
			this.radSADX.UseVisualStyleBackColor = true;
			// 
			// radSA2
			// 
			resources.ApplyResources(this.radSA2, "radSA2");
			this.radSA2.Name = "radSA2";
			this.radSA2.TabStop = true;
			this.radSA2.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radSA2);
			this.groupBox1.Controls.Add(this.radSADX);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.chkChaoRedir);
			this.groupBox2.Controls.Add(this.chkMainRedir);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.txtDLLName);
			this.groupBox2.Controls.Add(this.chkDLLFile);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// chkChaoRedir
			// 
			resources.ApplyResources(this.chkChaoRedir, "chkChaoRedir");
			this.chkChaoRedir.Name = "chkChaoRedir";
			this.chkChaoRedir.UseVisualStyleBackColor = true;
			// 
			// chkMainRedir
			// 
			resources.ApplyResources(this.chkMainRedir, "chkMainRedir");
			this.chkMainRedir.Name = "chkMainRedir";
			this.chkMainRedir.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// txtDLLName
			// 
			resources.ApplyResources(this.txtDLLName, "txtDLLName");
			this.txtDLLName.Name = "txtDLLName";
			// 
			// chkDLLFile
			// 
			resources.ApplyResources(this.chkDLLFile, "chkDLLFile");
			this.chkDLLFile.Name = "chkDLLFile";
			this.chkDLLFile.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// txtName
			// 
			resources.ApplyResources(this.txtName, "txtName");
			this.txtName.Name = "txtName";
			// 
			// txtAuth
			// 
			resources.ApplyResources(this.txtAuth, "txtAuth");
			this.txtAuth.Name = "txtAuth";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// txtDesc
			// 
			resources.ApplyResources(this.txtDesc, "txtDesc");
			this.txtDesc.Name = "txtDesc";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// txtVerNum
			// 
			resources.ApplyResources(this.txtVerNum, "txtVerNum");
			this.txtVerNum.Name = "txtVerNum";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// btnCreateSave
			// 
			resources.ApplyResources(this.btnCreateSave, "btnCreateSave");
			this.btnCreateSave.Name = "btnCreateSave";
			this.btnCreateSave.UseVisualStyleBackColor = true;
			// 
			// editProj
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnCreateSave);
			this.Controls.Add(this.txtVerNum);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtDesc);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtAuth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "editProj";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radSADX;
		private System.Windows.Forms.RadioButton radSA2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox txtDLLName;
		private System.Windows.Forms.CheckBox chkDLLFile;
		private System.Windows.Forms.CheckBox chkChaoRedir;
		private System.Windows.Forms.CheckBox chkMainRedir;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.TextBox txtAuth;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtDesc;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtVerNum;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnCreateSave;
	}
}