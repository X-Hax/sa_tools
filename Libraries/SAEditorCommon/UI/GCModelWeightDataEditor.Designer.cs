namespace SAModel.SAEditorCommon.UI
{
	partial class GCModelWeightDataEditor
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCModelWeightDataEditor));
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
			buttonClose = new System.Windows.Forms.Button();
			contextMenuStripMatEdit = new System.Windows.Forms.ContextMenuStrip(components);
			editPCMatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editTextureIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editStripAlphaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			groupBoxVertList = new System.Windows.Forms.GroupBox();
			buttonDeleteVertex = new System.Windows.Forms.Button();
			buttonResetVertices = new System.Windows.Forms.Button();
			listViewVertices = new System.Windows.Forms.ListView();
			columnHeaderVertID = new System.Windows.Forms.ColumnHeader();
			columnHeaderVertData = new System.Windows.Forms.ColumnHeader();
			contextMenuStripVertCol = new System.Windows.Forms.ContextMenuStrip(components);
			showVertexCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			columnHeaderNormData = new System.Windows.Forms.ColumnHeader();
			columnHeaderIndex = new System.Windows.Forms.ColumnHeader();
			columnHeaderWeight = new System.Windows.Forms.ColumnHeader();
			statusStrip1.SuspendLayout();
			contextMenuStripMatEdit.SuspendLayout();
			groupBoxVertList.SuspendLayout();
			contextMenuStripVertCol.SuspendLayout();
			SuspendLayout();
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabelInfo });
			statusStrip1.Location = new System.Drawing.Point(0, 345);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
			statusStrip1.Size = new System.Drawing.Size(561, 22);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 11;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelInfo
			// 
			toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
			toolStripStatusLabelInfo.Size = new System.Drawing.Size(477, 17);
			toolStripStatusLabelInfo.Text = "Click a vertex piece to display its information here. Right-click to edit pieces, if applicable.";
			// 
			// buttonClose
			// 
			buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonClose.Location = new System.Drawing.Point(473, 316);
			buttonClose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new System.Drawing.Size(84, 24);
			buttonClose.TabIndex = 17;
			buttonClose.Text = "Close";
			buttonClose.UseVisualStyleBackColor = true;
			buttonClose.Click += buttonClose_Click;
			// 
			// contextMenuStripMatEdit
			// 
			contextMenuStripMatEdit.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripMatEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { editPCMatToolStripMenuItem, editTextureIDToolStripMenuItem, editStripAlphaToolStripMenuItem });
			contextMenuStripMatEdit.Name = "contextMenuStripMatEdit";
			contextMenuStripMatEdit.Size = new System.Drawing.Size(68, 70);
			contextMenuStripMatEdit.Opening += contextMenuStrip2_Opening;
			// 
			// editPCMatToolStripMenuItem
			// 
			editPCMatToolStripMenuItem.Name = "editPCMatToolStripMenuItem";
			editPCMatToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
			// 
			// editTextureIDToolStripMenuItem
			// 
			editTextureIDToolStripMenuItem.Name = "editTextureIDToolStripMenuItem";
			editTextureIDToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
			// 
			// editStripAlphaToolStripMenuItem
			// 
			editStripAlphaToolStripMenuItem.Name = "editStripAlphaToolStripMenuItem";
			editStripAlphaToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
			// 
			// groupBoxVertList
			// 
			groupBoxVertList.Controls.Add(buttonDeleteVertex);
			groupBoxVertList.Controls.Add(buttonResetVertices);
			groupBoxVertList.Controls.Add(listViewVertices);
			groupBoxVertList.Location = new System.Drawing.Point(8, 15);
			groupBoxVertList.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			groupBoxVertList.Name = "groupBoxVertList";
			groupBoxVertList.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
			groupBoxVertList.Size = new System.Drawing.Size(522, 295);
			groupBoxVertList.TabIndex = 29;
			groupBoxVertList.TabStop = false;
			groupBoxVertList.Text = "Vertex Data";
			// 
			// buttonDeleteVertex
			// 
			buttonDeleteVertex.Location = new System.Drawing.Point(391, 261);
			buttonDeleteVertex.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			buttonDeleteVertex.Name = "buttonDeleteVertex";
			buttonDeleteVertex.Size = new System.Drawing.Size(93, 23);
			buttonDeleteVertex.TabIndex = 13;
			buttonDeleteVertex.Text = "Delete Vertex";
			buttonDeleteVertex.UseVisualStyleBackColor = true;
			// 
			// buttonResetVertices
			// 
			buttonResetVertices.Location = new System.Drawing.Point(293, 261);
			buttonResetVertices.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			buttonResetVertices.Name = "buttonResetVertices";
			buttonResetVertices.Size = new System.Drawing.Size(95, 23);
			buttonResetVertices.TabIndex = 12;
			buttonResetVertices.Text = "Reset Vertices";
			buttonResetVertices.UseVisualStyleBackColor = true;
			// 
			// listViewVertices
			// 
			listViewVertices.AutoArrange = false;
			listViewVertices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderVertID, columnHeaderIndex, columnHeaderVertData, columnHeaderNormData, columnHeaderWeight });
			listViewVertices.FullRowSelect = true;
			listViewVertices.GridLines = true;
			listViewVertices.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewVertices.Location = new System.Drawing.Point(4, 19);
			listViewVertices.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			listViewVertices.MultiSelect = false;
			listViewVertices.Name = "listViewVertices";
			listViewVertices.ShowGroups = false;
			listViewVertices.Size = new System.Drawing.Size(502, 236);
			listViewVertices.TabIndex = 11;
			listViewVertices.UseCompatibleStateImageBehavior = false;
			listViewVertices.View = System.Windows.Forms.View.Details;
			listViewVertices.SelectedIndexChanged += listViewVertices_SelectedIndexChanged;
			// 
			// columnHeaderVertID
			// 
			columnHeaderVertID.Text = "ID";
			// 
			// columnHeaderVertData
			// 
			columnHeaderVertData.Text = "Vertex";
			// 
			// contextMenuStripVertCol
			// 
			contextMenuStripVertCol.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripVertCol.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { showVertexCollectionToolStripMenuItem });
			contextMenuStripVertCol.Name = "contextMenuStripVertCol";
			contextMenuStripVertCol.Size = new System.Drawing.Size(195, 26);
			// 
			// showVertexCollectionToolStripMenuItem
			// 
			showVertexCollectionToolStripMenuItem.Name = "showVertexCollectionToolStripMenuItem";
			showVertexCollectionToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			showVertexCollectionToolStripMenuItem.Text = "Show Vertex Collection";
			// 
			// columnHeaderNormData
			// 
			columnHeaderNormData.Text = "Normal";
			// 
			// columnHeaderIndex
			// 
			columnHeaderIndex.Text = "Index";
			// 
			// columnHeaderWeight
			// 
			columnHeaderWeight.Text = "Weight";
			// 
			// GCModelWeightDataEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			ClientSize = new System.Drawing.Size(561, 367);
			Controls.Add(groupBoxVertList);
			Controls.Add(buttonClose);
			Controls.Add(statusStrip1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "GCModelWeightDataEditor";
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Weight Data Editor";
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			contextMenuStripMatEdit.ResumeLayout(false);
			groupBoxVertList.ResumeLayout(false);
			contextMenuStripVertCol.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripMatEdit;
		private System.Windows.Forms.ToolStripMenuItem editPCMatToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editTextureIDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editStripAlphaToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBoxVertList;
		private System.Windows.Forms.ListView listViewVertices;
		private System.Windows.Forms.ColumnHeader columnHeaderVertID;
		private System.Windows.Forms.ColumnHeader columnHeaderVertData;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripVertCol;
		private System.Windows.Forms.ToolStripMenuItem showVertexCollectionToolStripMenuItem;
		private System.Windows.Forms.Button buttonResetVertices;
		private System.Windows.Forms.Button buttonDeleteVertex;
		private System.Windows.Forms.ColumnHeader columnHeaderIndex;
		private System.Windows.Forms.ColumnHeader columnHeaderNormData;
		private System.Windows.Forms.ColumnHeader columnHeaderWeight;
	}
}