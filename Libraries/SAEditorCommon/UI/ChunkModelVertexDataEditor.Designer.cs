namespace SAModel.SAEditorCommon.UI
{
	partial class ChunkModelVertexDataEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChunkModelVertexDataEditor));
			statusStrip1 = new System.Windows.Forms.StatusStrip();
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
			columnHeaderVertPos = new System.Windows.Forms.ColumnHeader();
			columnHeaderVertNorm = new System.Windows.Forms.ColumnHeader();
			columnHeaderVertColor = new System.Windows.Forms.ColumnHeader();
			columnHeaderVertSpec = new System.Windows.Forms.ColumnHeader();
			columnHeaderVertUF = new System.Windows.Forms.ColumnHeader();
			contextMenuStripVertCol = new System.Windows.Forms.ContextMenuStrip(components);
			showVertexCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			contextMenuStripMatEdit.SuspendLayout();
			groupBoxVertList.SuspendLayout();
			contextMenuStripVertCol.SuspendLayout();
			SuspendLayout();
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Location = new System.Drawing.Point(0, 529);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 15, 0);
			statusStrip1.Size = new System.Drawing.Size(1157, 22);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 11;
			statusStrip1.Text = "statusStrip1";
			// 
			// buttonClose
			// 
			buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonClose.Location = new System.Drawing.Point(1025, 474);
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new System.Drawing.Size(126, 36);
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
			contextMenuStripMatEdit.Size = new System.Drawing.Size(73, 76);
			contextMenuStripMatEdit.Opening += contextMenuStrip2_Opening;
			// 
			// editPCMatToolStripMenuItem
			// 
			editPCMatToolStripMenuItem.Name = "editPCMatToolStripMenuItem";
			editPCMatToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
			// 
			// editTextureIDToolStripMenuItem
			// 
			editTextureIDToolStripMenuItem.Name = "editTextureIDToolStripMenuItem";
			editTextureIDToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
			// 
			// editStripAlphaToolStripMenuItem
			// 
			editStripAlphaToolStripMenuItem.Name = "editStripAlphaToolStripMenuItem";
			editStripAlphaToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
			// 
			// groupBoxVertList
			// 
			groupBoxVertList.Controls.Add(buttonDeleteVertex);
			groupBoxVertList.Controls.Add(buttonResetVertices);
			groupBoxVertList.Controls.Add(listViewVertices);
			groupBoxVertList.Location = new System.Drawing.Point(12, 22);
			groupBoxVertList.Name = "groupBoxVertList";
			groupBoxVertList.Size = new System.Drawing.Size(1133, 443);
			groupBoxVertList.TabIndex = 29;
			groupBoxVertList.TabStop = false;
			groupBoxVertList.Text = "Vertex Data";
			// 
			// buttonDeleteVertex
			// 
			buttonDeleteVertex.Location = new System.Drawing.Point(972, 392);
			buttonDeleteVertex.Name = "buttonDeleteVertex";
			buttonDeleteVertex.Size = new System.Drawing.Size(140, 34);
			buttonDeleteVertex.TabIndex = 13;
			buttonDeleteVertex.Text = "Delete Vertex";
			buttonDeleteVertex.UseVisualStyleBackColor = true;
			// 
			// buttonResetVertices
			// 
			buttonResetVertices.Location = new System.Drawing.Point(824, 392);
			buttonResetVertices.Name = "buttonResetVertices";
			buttonResetVertices.Size = new System.Drawing.Size(142, 34);
			buttonResetVertices.TabIndex = 12;
			buttonResetVertices.Text = "Reset Vertices";
			buttonResetVertices.UseVisualStyleBackColor = true;
			// 
			// listViewVertices
			// 
			listViewVertices.AutoArrange = false;
			listViewVertices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderVertID, columnHeaderVertPos, columnHeaderVertNorm, columnHeaderVertColor, columnHeaderVertSpec, columnHeaderVertUF });
			listViewVertices.FullRowSelect = true;
			listViewVertices.GridLines = true;
			listViewVertices.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewVertices.Location = new System.Drawing.Point(6, 28);
			listViewVertices.MultiSelect = false;
			listViewVertices.Name = "listViewVertices";
			listViewVertices.ShowGroups = false;
			listViewVertices.Size = new System.Drawing.Size(1109, 352);
			listViewVertices.TabIndex = 11;
			listViewVertices.UseCompatibleStateImageBehavior = false;
			listViewVertices.View = System.Windows.Forms.View.Details;
			listViewVertices.SelectedIndexChanged += listViewVertices_SelectedIndexChanged;
			// 
			// columnHeaderVertID
			// 
			columnHeaderVertID.Text = "ID";
			// 
			// columnHeaderVertPos
			// 
			columnHeaderVertPos.Text = "Point";
			// 
			// columnHeaderVertNorm
			// 
			columnHeaderVertNorm.Text = "Normal";
			// 
			// columnHeaderVertColor
			// 
			columnHeaderVertColor.Text = "Color";
			// 
			// columnHeaderVertSpec
			// 
			columnHeaderVertSpec.Text = "Specular";
			// 
			// columnHeaderVertUF
			// 
			columnHeaderVertUF.Text = "User Flags";
			// 
			// contextMenuStripVertCol
			// 
			contextMenuStripVertCol.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripVertCol.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { showVertexCollectionToolStripMenuItem });
			contextMenuStripVertCol.Name = "contextMenuStripVertCol";
			contextMenuStripVertCol.Size = new System.Drawing.Size(265, 36);
			// 
			// showVertexCollectionToolStripMenuItem
			// 
			showVertexCollectionToolStripMenuItem.Name = "showVertexCollectionToolStripMenuItem";
			showVertexCollectionToolStripMenuItem.Size = new System.Drawing.Size(264, 32);
			showVertexCollectionToolStripMenuItem.Text = "Show Vertex Collection";
			// 
			// ChunkModelVertexDataEditor
			// 
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			ClientSize = new System.Drawing.Size(1157, 551);
			Controls.Add(groupBoxVertList);
			Controls.Add(buttonClose);
			Controls.Add(statusStrip1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkModelVertexDataEditor";
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Vertex Data Editor";
			contextMenuStripMatEdit.ResumeLayout(false);
			groupBoxVertList.ResumeLayout(false);
			contextMenuStripVertCol.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.Button buttonMoveMeshUp;
		private System.Windows.Forms.Button buttonMoveMeshDown;
		private System.Windows.Forms.ListView listViewMeshes;
		private System.Windows.Forms.Button buttonCloneMesh;
		private System.Windows.Forms.Button buttonDeleteMesh;
		private System.Windows.Forms.ColumnHeader columnHeaderMatID;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.Label labelModelName;
		private System.Windows.Forms.Label labelMeshsetName;
		private System.Windows.Forms.TextBox textBoxMeshsetName;
		private System.Windows.Forms.TextBox textBoxModelName;
		private System.Windows.Forms.TextBox textBoxModelY;
		private System.Windows.Forms.TextBox textBoxModelZ;
		private System.Windows.Forms.TextBox textBoxModelX;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBoxLabels;
		private System.Windows.Forms.GroupBox groupBoxBounds;
		private System.Windows.Forms.GroupBox groupBoxMeshList;
		private System.Windows.Forms.Button buttonResetMeshes;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.TextBox textBoxModelRadius;
		private System.Windows.Forms.Label labelR;
		private System.Windows.Forms.TextBox textBoxVertexName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxObjectName;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkType;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkData;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripMatEdit;
		private System.Windows.Forms.ToolStripMenuItem editPCMatToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editTextureIDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editStripAlphaToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBoxVertList;
		private System.Windows.Forms.ListView listViewVertices;
		private System.Windows.Forms.ColumnHeader columnHeaderVertID;
		private System.Windows.Forms.ColumnHeader columnHeaderVertPos;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripVertCol;
		private System.Windows.Forms.ToolStripMenuItem showVertexCollectionToolStripMenuItem;
		private System.Windows.Forms.Button buttonResetVertices;
		private System.Windows.Forms.Button buttonDeleteVertex;
		private System.Windows.Forms.ColumnHeader columnHeaderVertNorm;
		private System.Windows.Forms.ColumnHeader columnHeaderVertColor;
		private System.Windows.Forms.ColumnHeader columnHeaderVertSpec;
		private System.Windows.Forms.ColumnHeader columnHeaderVertUF;
	}
}