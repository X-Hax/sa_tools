namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	partial class ModelLibrary
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.modelListView = new System.Windows.Forms.ListView();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.modelListView);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel2_MouseDown);
			this.splitContainer1.Panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel2_MouseMove);
			this.splitContainer1.Panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel2_MouseUp);
			this.splitContainer1.Size = new System.Drawing.Size(951, 392);
			this.splitContainer1.SplitterDistance = 588;
			this.splitContainer1.TabIndex = 0;
			// 
			// modelListView
			// 
			this.modelListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modelListView.Location = new System.Drawing.Point(0, 0);
			this.modelListView.MultiSelect = false;
			this.modelListView.Name = "modelListView";
			this.modelListView.Size = new System.Drawing.Size(588, 392);
			this.modelListView.TabIndex = 1;
			this.modelListView.TileSize = new System.Drawing.Size(256, 256);
			this.modelListView.UseCompatibleStateImageBehavior = false;
			this.modelListView.SelectedIndexChanged += new System.EventHandler(this.modelListView_SelectedIndexChanged);
			// 
			// ModelLibrary
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(951, 392);
			this.Controls.Add(this.splitContainer1);
			this.Name = "ModelLibrary";
			this.Text = "ModelLibrary";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModelLibrary_FormClosing);
			this.Shown += new System.EventHandler(this.ModelLibrary_Shown);
			this.ResizeEnd += new System.EventHandler(this.ModelLibrary_ResizeEnd);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView modelListView;
	}
}