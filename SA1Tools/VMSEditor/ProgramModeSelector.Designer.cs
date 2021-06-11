
namespace VMSEditor
{
	partial class ProgramModeSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgramModeSelector));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonVMIEditor = new System.Windows.Forms.Button();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.buttonDLCEditor = new System.Windows.Forms.Button();
            this.buttonChaoEditor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select program mode:";
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(30, 305);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(201, 54);
            this.buttonExit.TabIndex = 5;
            this.buttonExit.Text = "Exit";
            this.buttonExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonVMIEditor
            // 
            this.buttonVMIEditor.Image = global::VMSEditor.Properties.Resources.vmi_smol;
            this.buttonVMIEditor.Location = new System.Drawing.Point(30, 245);
            this.buttonVMIEditor.Name = "buttonVMIEditor";
            this.buttonVMIEditor.Size = new System.Drawing.Size(201, 54);
            this.buttonVMIEditor.TabIndex = 4;
            this.buttonVMIEditor.Text = "VMI Editor";
            this.buttonVMIEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonVMIEditor.UseVisualStyleBackColor = true;
            this.buttonVMIEditor.Click += new System.EventHandler(this.buttonVMIEditor_Click);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Image = global::VMSEditor.Properties.Resources.open;
            this.buttonOpenFile.Location = new System.Drawing.Point(30, 65);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(201, 54);
            this.buttonOpenFile.TabIndex = 1;
            this.buttonOpenFile.Text = "Auto (Open File)";
            this.buttonOpenFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // buttonDLCEditor
            // 
            this.buttonDLCEditor.Image = global::VMSEditor.Properties.Resources.editorDLC;
            this.buttonDLCEditor.Location = new System.Drawing.Point(30, 185);
            this.buttonDLCEditor.Name = "buttonDLCEditor";
            this.buttonDLCEditor.Size = new System.Drawing.Size(201, 54);
            this.buttonDLCEditor.TabIndex = 3;
            this.buttonDLCEditor.Text = "DLC Editor (SA1)";
            this.buttonDLCEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonDLCEditor.UseVisualStyleBackColor = true;
            this.buttonDLCEditor.Click += new System.EventHandler(this.buttonDLCEditor_Click);
            // 
            // buttonChaoEditor
            // 
            this.buttonChaoEditor.Image = global::VMSEditor.Properties.Resources.editorChao;
            this.buttonChaoEditor.Location = new System.Drawing.Point(30, 125);
            this.buttonChaoEditor.Name = "buttonChaoEditor";
            this.buttonChaoEditor.Size = new System.Drawing.Size(201, 54);
            this.buttonChaoEditor.TabIndex = 2;
            this.buttonChaoEditor.Text = "Chao Editor (SA1)";
            this.buttonChaoEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonChaoEditor.UseVisualStyleBackColor = true;
            this.buttonChaoEditor.Click += new System.EventHandler(this.buttonChaoEditor_Click);
            // 
            // ProgramModeSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 381);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonVMIEditor);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.buttonDLCEditor);
            this.Controls.Add(this.buttonChaoEditor);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ProgramModeSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonChaoEditor;
		private System.Windows.Forms.Button buttonDLCEditor;
		private System.Windows.Forms.Button buttonOpenFile;
		private System.Windows.Forms.Button buttonVMIEditor;
		private System.Windows.Forms.Button buttonExit;
	}
}