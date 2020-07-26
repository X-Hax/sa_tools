namespace SAToolsHub
{
	partial class projSelect
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
			this.lstProjects = new System.Windows.Forms.ListView();
			this.clmProject = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmGame = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOpen = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lstProjects
			// 
			this.lstProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmProject,
            this.clmGame});
			this.lstProjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lstProjects.HideSelection = false;
			this.lstProjects.Location = new System.Drawing.Point(12, 12);
			this.lstProjects.MultiSelect = false;
			this.lstProjects.Name = "lstProjects";
			this.lstProjects.Size = new System.Drawing.Size(410, 458);
			this.lstProjects.TabIndex = 0;
			this.lstProjects.UseCompatibleStateImageBehavior = false;
			this.lstProjects.View = System.Windows.Forms.View.Details;
			this.lstProjects.SelectedIndexChanged += new System.EventHandler(this.lstProjects_SelectedIndexChanged);
			// 
			// clmProject
			// 
			this.clmProject.Text = "Project Name";
			this.clmProject.Width = 216;
			// 
			// clmGame
			// 
			this.clmGame.Text = "Game";
			this.clmGame.Width = 190;
			// 
			// btnCancel
			// 
			this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCancel.Location = new System.Drawing.Point(12, 476);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOpen
			// 
			this.btnOpen.Enabled = false;
			this.btnOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnOpen.Location = new System.Drawing.Point(347, 476);
			this.btnOpen.Name = "btnOpen";
			this.btnOpen.Size = new System.Drawing.Size(75, 23);
			this.btnOpen.TabIndex = 2;
			this.btnOpen.Text = "Open";
			this.btnOpen.UseVisualStyleBackColor = true;
			this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
			// 
			// projSelect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 511);
			this.Controls.Add(this.btnOpen);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lstProjects);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "projSelect";
			this.Text = "Select a Project";
			this.Shown += new System.EventHandler(this.projSelect_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView lstProjects;
		private System.Windows.Forms.ColumnHeader clmGame;
		private System.Windows.Forms.ColumnHeader clmProject;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOpen;
	}
}