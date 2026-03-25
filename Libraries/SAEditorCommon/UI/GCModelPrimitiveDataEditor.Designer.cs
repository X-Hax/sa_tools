namespace SAModel.SAEditorCommon.UI
{
	partial class GCModelPrimitiveDataEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCModelPrimitiveDataEditor));
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
			buttonClose = new System.Windows.Forms.Button();
			contextMenuStripMatEdit = new System.Windows.Forms.ContextMenuStrip(components);
			editPCMatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editTextureIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editStripAlphaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			groupBoxPrimList = new System.Windows.Forms.GroupBox();
			labelPrimitiveType = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			listViewVertices = new System.Windows.Forms.ListView();
			columnHeaderPrimID = new System.Windows.Forms.ColumnHeader();
			columnHeaderPrimPosIdx = new System.Windows.Forms.ColumnHeader();
			columnHeaderPrimNorIdx = new System.Windows.Forms.ColumnHeader();
			columnHeaderPrimColorIdx = new System.Windows.Forms.ColumnHeader();
			columnHeaderUVIdx = new System.Windows.Forms.ColumnHeader();
			contextMenuStripVertCol = new System.Windows.Forms.ContextMenuStrip(components);
			showVertexCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			label2 = new System.Windows.Forms.Label();
			numericUpDownPrimSet = new System.Windows.Forms.NumericUpDown();
			statusStrip1.SuspendLayout();
			contextMenuStripMatEdit.SuspendLayout();
			groupBoxPrimList.SuspendLayout();
			contextMenuStripVertCol.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownPrimSet).BeginInit();
			SuspendLayout();
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabelInfo });
			statusStrip1.Location = new System.Drawing.Point(0, 416);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
			statusStrip1.Size = new System.Drawing.Size(372, 22);
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
			buttonClose.Location = new System.Drawing.Point(284, 387);
			buttonClose.Margin = new System.Windows.Forms.Padding(2);
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
			// groupBoxPrimList
			// 
			groupBoxPrimList.Controls.Add(labelPrimitiveType);
			groupBoxPrimList.Controls.Add(label1);
			groupBoxPrimList.Controls.Add(listViewVertices);
			groupBoxPrimList.Location = new System.Drawing.Point(11, 40);
			groupBoxPrimList.Margin = new System.Windows.Forms.Padding(2);
			groupBoxPrimList.Name = "groupBoxPrimList";
			groupBoxPrimList.Padding = new System.Windows.Forms.Padding(2);
			groupBoxPrimList.Size = new System.Drawing.Size(325, 343);
			groupBoxPrimList.TabIndex = 29;
			groupBoxPrimList.TabStop = false;
			groupBoxPrimList.Text = "Primitive Data";
			// 
			// labelPrimitiveType
			// 
			labelPrimitiveType.AutoSize = true;
			labelPrimitiveType.Location = new System.Drawing.Point(46, 30);
			labelPrimitiveType.Name = "labelPrimitiveType";
			labelPrimitiveType.Size = new System.Drawing.Size(79, 15);
			labelPrimitiveType.TabIndex = 15;
			labelPrimitiveType.Text = "TriangleSetup";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(5, 30);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(35, 15);
			label1.TabIndex = 14;
			label1.Text = "Type:";
			// 
			// listViewVertices
			// 
			listViewVertices.AutoArrange = false;
			listViewVertices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderPrimID, columnHeaderPrimPosIdx, columnHeaderPrimNorIdx, columnHeaderPrimColorIdx, columnHeaderUVIdx });
			listViewVertices.FullRowSelect = true;
			listViewVertices.GridLines = true;
			listViewVertices.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewVertices.Location = new System.Drawing.Point(4, 71);
			listViewVertices.Margin = new System.Windows.Forms.Padding(2);
			listViewVertices.MultiSelect = false;
			listViewVertices.Name = "listViewVertices";
			listViewVertices.ShowGroups = false;
			listViewVertices.Size = new System.Drawing.Size(314, 236);
			listViewVertices.TabIndex = 11;
			listViewVertices.UseCompatibleStateImageBehavior = false;
			listViewVertices.View = System.Windows.Forms.View.Details;
			listViewVertices.SelectedIndexChanged += listViewVertices_SelectedIndexChanged;
			// 
			// columnHeaderPrimID
			// 
			columnHeaderPrimID.Text = "ID";
			// 
			// columnHeaderPrimPosIdx
			// 
			columnHeaderPrimPosIdx.Text = "Position";
			// 
			// columnHeaderPrimNorIdx
			// 
			columnHeaderPrimNorIdx.Text = "Normal";
			// 
			// columnHeaderPrimColorIdx
			// 
			columnHeaderPrimColorIdx.Text = "Color";
			// 
			// columnHeaderUVIdx
			// 
			columnHeaderUVIdx.Text = "UV";
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
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(15, 15);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(53, 15);
			label2.TabIndex = 31;
			label2.Text = "Data Set:";
			// 
			// numericUpDownPrimSet
			// 
			numericUpDownPrimSet.Location = new System.Drawing.Point(73, 12);
			numericUpDownPrimSet.Name = "numericUpDownPrimSet";
			numericUpDownPrimSet.Size = new System.Drawing.Size(55, 23);
			numericUpDownPrimSet.TabIndex = 32;
			numericUpDownPrimSet.ValueChanged += numericUpDown1_ValueChanged;
			// 
			// GCModelPrimitiveDataEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			ClientSize = new System.Drawing.Size(372, 438);
			Controls.Add(numericUpDownPrimSet);
			Controls.Add(label2);
			Controls.Add(groupBoxPrimList);
			Controls.Add(buttonClose);
			Controls.Add(statusStrip1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "GCModelPrimitiveDataEditor";
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Primitive Data Editor";
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			contextMenuStripMatEdit.ResumeLayout(false);
			groupBoxPrimList.ResumeLayout(false);
			groupBoxPrimList.PerformLayout();
			contextMenuStripVertCol.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)numericUpDownPrimSet).EndInit();
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
		private System.Windows.Forms.GroupBox groupBoxPrimList;
		private System.Windows.Forms.ListView listViewVertices;
		private System.Windows.Forms.ColumnHeader columnHeaderPrimID;
		private System.Windows.Forms.ColumnHeader columnHeaderPrimPosIdx;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripVertCol;
		private System.Windows.Forms.ToolStripMenuItem showVertexCollectionToolStripMenuItem;
		private System.Windows.Forms.ColumnHeader columnHeaderPrimNorIdx;
		private System.Windows.Forms.ColumnHeader columnHeaderPrimColorIdx;
		private System.Windows.Forms.ColumnHeader columnHeaderUVIdx;
		private System.Windows.Forms.Label labelPrimitiveType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownPrimSet;
	}
}