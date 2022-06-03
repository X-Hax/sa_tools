
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.buttonChallengeResultEditor = new System.Windows.Forms.Button();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.buttonCartResultEditor = new System.Windows.Forms.Button();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.buttonWorldRankingsEditor = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select program mode:";
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(404, 262);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(210, 44);
            this.buttonExit.TabIndex = 5;
            this.buttonExit.Text = "Exit";
            this.buttonExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonVMIEditor
            // 
            this.buttonVMIEditor.Location = new System.Drawing.Point(88, 260);
            this.buttonVMIEditor.Name = "buttonVMIEditor";
            this.buttonVMIEditor.Size = new System.Drawing.Size(210, 48);
            this.buttonVMIEditor.TabIndex = 4;
            this.buttonVMIEditor.Text = "VMI Editor";
            this.buttonVMIEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonVMIEditor.UseVisualStyleBackColor = true;
            this.buttonVMIEditor.Click += new System.EventHandler(this.buttonVMIEditor_Click);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Location = new System.Drawing.Point(88, 60);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(210, 48);
            this.buttonOpenFile.TabIndex = 1;
            this.buttonOpenFile.Text = "Auto (Open File)";
            this.buttonOpenFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // buttonDLCEditor
            // 
            this.buttonDLCEditor.Location = new System.Drawing.Point(88, 195);
            this.buttonDLCEditor.Name = "buttonDLCEditor";
            this.buttonDLCEditor.Size = new System.Drawing.Size(210, 48);
            this.buttonDLCEditor.TabIndex = 3;
            this.buttonDLCEditor.Text = "DLC Editor";
            this.buttonDLCEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonDLCEditor.UseVisualStyleBackColor = true;
            this.buttonDLCEditor.Click += new System.EventHandler(this.buttonDLCEditor_Click);
            // 
            // buttonChaoEditor
            // 
            this.buttonChaoEditor.Location = new System.Drawing.Point(88, 127);
            this.buttonChaoEditor.Name = "buttonChaoEditor";
            this.buttonChaoEditor.Size = new System.Drawing.Size(210, 48);
            this.buttonChaoEditor.TabIndex = 2;
            this.buttonChaoEditor.Text = "Chao Editor";
            this.buttonChaoEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonChaoEditor.UseVisualStyleBackColor = true;
            this.buttonChaoEditor.Click += new System.EventHandler(this.buttonChaoEditor_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VMSEditor.Properties.Resources.open;
            this.pictureBox1.InitialImage = global::VMSEditor.Properties.Resources.open;
            this.pictureBox1.Location = new System.Drawing.Point(24, 60);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::VMSEditor.Properties.Resources.editorChao;
            this.pictureBox2.InitialImage = null;
            this.pictureBox2.Location = new System.Drawing.Point(24, 128);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(48, 48);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::VMSEditor.Properties.Resources.editorDLC;
            this.pictureBox3.InitialImage = null;
            this.pictureBox3.Location = new System.Drawing.Point(24, 195);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(48, 48);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 8;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::VMSEditor.Properties.Resources.vmi;
            this.pictureBox4.InitialImage = null;
            this.pictureBox4.Location = new System.Drawing.Point(24, 260);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(48, 48);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 9;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.InitialImage = null;
            this.pictureBox5.Location = new System.Drawing.Point(340, 60);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(48, 48);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5.TabIndex = 11;
            this.pictureBox5.TabStop = false;
			// 
			// buttonChallengeResultEditor
			// 
			this.buttonChallengeResultEditor.Enabled = false;
			this.buttonChallengeResultEditor.Location = new System.Drawing.Point(404, 60);
            this.buttonChallengeResultEditor.Name = "buttonChallengeResultEditor";
            this.buttonChallengeResultEditor.Size = new System.Drawing.Size(210, 48);
            this.buttonChallengeResultEditor.TabIndex = 10;
            this.buttonChallengeResultEditor.Text = "Challenge Result Editor";
            this.buttonChallengeResultEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonChallengeResultEditor.UseVisualStyleBackColor = true;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Enabled = false;
            this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
            this.pictureBox6.InitialImage = null;
            this.pictureBox6.Location = new System.Drawing.Point(340, 128);
            this.pictureBox6.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(48, 48);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 13;
            this.pictureBox6.TabStop = false;
            // 
            // buttonCartResultEditor
            // 
            this.buttonCartResultEditor.Enabled = false;
            this.buttonCartResultEditor.Location = new System.Drawing.Point(404, 128);
            this.buttonCartResultEditor.Name = "buttonCartResultEditor";
            this.buttonCartResultEditor.Size = new System.Drawing.Size(210, 48);
            this.buttonCartResultEditor.TabIndex = 12;
            this.buttonCartResultEditor.Text = "Cart Result Editor";
            this.buttonCartResultEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonCartResultEditor.UseVisualStyleBackColor = true;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Enabled = false;
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.InitialImage = null;
            this.pictureBox7.Location = new System.Drawing.Point(340, 195);
            this.pictureBox7.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(48, 48);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 15;
            this.pictureBox7.TabStop = false;
            // 
            // buttonWorldRankingsEditor
            // 
            this.buttonWorldRankingsEditor.Enabled = false;
            this.buttonWorldRankingsEditor.Location = new System.Drawing.Point(404, 195);
            this.buttonWorldRankingsEditor.Name = "buttonWorldRankingsEditor";
            this.buttonWorldRankingsEditor.Size = new System.Drawing.Size(210, 48);
            this.buttonWorldRankingsEditor.TabIndex = 14;
            this.buttonWorldRankingsEditor.Text = "World Rankings Editor";
            this.buttonWorldRankingsEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonWorldRankingsEditor.UseVisualStyleBackColor = true;
            // 
            // ProgramModeSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(636, 335);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.buttonWorldRankingsEditor);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.buttonCartResultEditor);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.buttonChallengeResultEditor);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
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
            this.Text = "SA1 VMS Editor";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
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
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.PictureBox pictureBox4;
		private System.Windows.Forms.PictureBox pictureBox5;
		private System.Windows.Forms.Button buttonChallengeResultEditor;
		private System.Windows.Forms.PictureBox pictureBox6;
		private System.Windows.Forms.Button buttonCartResultEditor;
		private System.Windows.Forms.PictureBox pictureBox7;
		private System.Windows.Forms.Button buttonWorldRankingsEditor;
	}
}