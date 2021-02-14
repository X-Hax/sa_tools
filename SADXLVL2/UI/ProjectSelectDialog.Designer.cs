namespace SonicRetro.SAModel.SADXLVL2
{
	partial class ProjectSelectDialog
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
            this.buttonGo = new System.Windows.Forms.Button();
            this.listProjects = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonGo
            // 
            this.buttonGo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonGo.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonGo.Enabled = false;
            this.buttonGo.Location = new System.Drawing.Point(297, 282);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(75, 23);
            this.buttonGo.TabIndex = 2;
            this.buttonGo.Text = "&Go";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // listProjects
            // 
            this.listProjects.FormattingEnabled = true;
            this.listProjects.HorizontalScrollbar = true;
            this.listProjects.Location = new System.Drawing.Point(13, 28);
            this.listProjects.Name = "listProjects";
            this.listProjects.Size = new System.Drawing.Size(359, 238);
            this.listProjects.TabIndex = 0;
            this.listProjects.SelectedIndexChanged += new System.EventHandler(this.listProjects_SelectedIndexChanged);
            this.listProjects.DoubleClick += new System.EventHandler(this.listProjects_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Available Projects:";
            // 
            // ProjectSelectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 317);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listProjects);
            this.Controls.Add(this.buttonGo);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "ProjectSelectDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Select";
            this.Shown += new System.EventHandler(this.ProjectSelectDialog_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProjectSelectDialog_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button buttonGo;
		private System.Windows.Forms.ListBox listProjects;
		private System.Windows.Forms.Label label1;
	}
}