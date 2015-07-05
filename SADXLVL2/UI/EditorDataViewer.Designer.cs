namespace SonicRetro.SAModel.SADXLVL2.UI
{
	partial class EditorDataViewer
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
			this.viewTypesPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.HorizSplit = new System.Windows.Forms.SplitContainer();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.HorizSplit.Panel1.SuspendLayout();
			this.HorizSplit.Panel2.SuspendLayout();
			this.HorizSplit.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.HorizSplit);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.treeView1);
			this.splitContainer1.Size = new System.Drawing.Size(610, 551);
			this.splitContainer1.SplitterDistance = 197;
			this.splitContainer1.TabIndex = 0;
			// 
			// viewTypesPanel
			// 
			this.viewTypesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewTypesPanel.Location = new System.Drawing.Point(0, 0);
			this.viewTypesPanel.Name = "viewTypesPanel";
			this.viewTypesPanel.Size = new System.Drawing.Size(193, 265);
			this.viewTypesPanel.TabIndex = 0;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(409, 551);
			this.treeView1.TabIndex = 0;
			// 
			// HorizSplit
			// 
			this.HorizSplit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.HorizSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.HorizSplit.Location = new System.Drawing.Point(0, 0);
			this.HorizSplit.Name = "HorizSplit";
			this.HorizSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// HorizSplit.Panel1
			// 
			this.HorizSplit.Panel1.Controls.Add(this.viewTypesPanel);
			// 
			// HorizSplit.Panel2
			// 
			this.HorizSplit.Panel2.Controls.Add(this.propertyGrid1);
			this.HorizSplit.Size = new System.Drawing.Size(197, 551);
			this.HorizSplit.SplitterDistance = 269;
			this.HorizSplit.TabIndex = 0;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(193, 274);
			this.propertyGrid1.TabIndex = 0;
			// 
			// EditorDataViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(610, 551);
			this.Controls.Add(this.splitContainer1);
			this.Name = "EditorDataViewer";
			this.Text = "EditorDataViewer";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.HorizSplit.Panel1.ResumeLayout(false);
			this.HorizSplit.Panel2.ResumeLayout(false);
			this.HorizSplit.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.FlowLayoutPanel viewTypesPanel;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.SplitContainer HorizSplit;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
	}
}