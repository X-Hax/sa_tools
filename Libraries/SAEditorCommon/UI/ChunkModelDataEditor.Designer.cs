namespace SAModel.SAEditorCommon.UI
{
	partial class ChunkModelDataEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChunkModelDataEditor));
			buttonMoveMeshUp = new System.Windows.Forms.Button();
			buttonMoveMeshDown = new System.Windows.Forms.Button();
			listViewMeshes = new System.Windows.Forms.ListView();
			columnHeaderMatID = new System.Windows.Forms.ColumnHeader();
			columnHeaderCnkNum = new System.Windows.Forms.ColumnHeader();
			columnHeaderCnkID1 = new System.Windows.Forms.ColumnHeader();
			columnHeaderCnkType1 = new System.Windows.Forms.ColumnHeader();
			columnHeaderCnkID2 = new System.Windows.Forms.ColumnHeader();
			columnHeaderCnkType2 = new System.Windows.Forms.ColumnHeader();
			columnHeaderCnkID3 = new System.Windows.Forms.ColumnHeader();
			columnHeaderCnkType3 = new System.Windows.Forms.ColumnHeader();
			buttonCloneMesh = new System.Windows.Forms.Button();
			buttonDeleteMesh = new System.Windows.Forms.Button();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
			labelModelName = new System.Windows.Forms.Label();
			labelMeshsetName = new System.Windows.Forms.Label();
			textBoxMeshsetName = new System.Windows.Forms.TextBox();
			textBoxModelName = new System.Windows.Forms.TextBox();
			textBoxModelY = new System.Windows.Forms.TextBox();
			textBoxModelZ = new System.Windows.Forms.TextBox();
			textBoxModelX = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			groupBoxLabels = new System.Windows.Forms.GroupBox();
			textBoxObjectName = new System.Windows.Forms.TextBox();
			label7 = new System.Windows.Forms.Label();
			textBoxVertexName = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			groupBoxBounds = new System.Windows.Forms.GroupBox();
			textBoxModelRadius = new System.Windows.Forms.TextBox();
			labelR = new System.Windows.Forms.Label();
			groupBoxMeshList = new System.Windows.Forms.GroupBox();
			buttonResetMeshes = new System.Windows.Forms.Button();
			buttonClose = new System.Windows.Forms.Button();
			comboBoxNode = new System.Windows.Forms.ComboBox();
			contextMenuStripMatEdit = new System.Windows.Forms.ContextMenuStrip(components);
			editDiffuseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editAmbientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editSpecularToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editTextureIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editStripAlphaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			statusStrip1.SuspendLayout();
			groupBoxLabels.SuspendLayout();
			groupBoxBounds.SuspendLayout();
			groupBoxMeshList.SuspendLayout();
			contextMenuStripMatEdit.SuspendLayout();
			SuspendLayout();
			// 
			// buttonMoveMeshUp
			// 
			buttonMoveMeshUp.Enabled = false;
			buttonMoveMeshUp.Location = new System.Drawing.Point(1071, 32);
			buttonMoveMeshUp.Margin = new System.Windows.Forms.Padding(4);
			buttonMoveMeshUp.Name = "buttonMoveMeshUp";
			buttonMoveMeshUp.Size = new System.Drawing.Size(36, 36);
			buttonMoveMeshUp.TabIndex = 12;
			buttonMoveMeshUp.Text = "↑";
			buttonMoveMeshUp.UseVisualStyleBackColor = true;
			buttonMoveMeshUp.Click += buttonMoveMeshUp_Click;
			// 
			// buttonMoveMeshDown
			// 
			buttonMoveMeshDown.Enabled = false;
			buttonMoveMeshDown.Location = new System.Drawing.Point(1071, 76);
			buttonMoveMeshDown.Margin = new System.Windows.Forms.Padding(4);
			buttonMoveMeshDown.Name = "buttonMoveMeshDown";
			buttonMoveMeshDown.Size = new System.Drawing.Size(36, 36);
			buttonMoveMeshDown.TabIndex = 13;
			buttonMoveMeshDown.Text = "↓";
			buttonMoveMeshDown.UseVisualStyleBackColor = true;
			buttonMoveMeshDown.Click += buttonMoveMeshDown_Click;
			// 
			// listViewMeshes
			// 
			listViewMeshes.AutoArrange = false;
			listViewMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderMatID, columnHeaderCnkNum, columnHeaderCnkID1, columnHeaderCnkType1, columnHeaderCnkID2, columnHeaderCnkType2, columnHeaderCnkID3, columnHeaderCnkType3 });
			listViewMeshes.FullRowSelect = true;
			listViewMeshes.GridLines = true;
			listViewMeshes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewMeshes.Location = new System.Drawing.Point(8, 28);
			listViewMeshes.Margin = new System.Windows.Forms.Padding(4);
			listViewMeshes.MultiSelect = false;
			listViewMeshes.Name = "listViewMeshes";
			listViewMeshes.ShowGroups = false;
			listViewMeshes.Size = new System.Drawing.Size(1052, 352);
			listViewMeshes.TabIndex = 11;
			listViewMeshes.UseCompatibleStateImageBehavior = false;
			listViewMeshes.View = System.Windows.Forms.View.Details;
			listViewMeshes.SelectedIndexChanged += listViewMeshes_SelectedIndexChanged;
			listViewMeshes.MouseClick += listViewMeshes_MouseClick;
			// 
			// columnHeaderMatID
			// 
			columnHeaderMatID.Text = "Material";
			// 
			// columnHeaderCnkNum
			// 
			columnHeaderCnkNum.Text = "Chunks";
			// 
			// columnHeaderCnkID1
			// 
			columnHeaderCnkID1.Text = "ID 1";
			// 
			// columnHeaderCnkType1
			// 
			columnHeaderCnkType1.Text = "Type 1";
			// 
			// columnHeaderCnkID2
			// 
			columnHeaderCnkID2.Text = "ID 2";
			// 
			// columnHeaderCnkType2
			// 
			columnHeaderCnkType2.Text = "Type 2";
			// 
			// columnHeaderCnkID3
			// 
			columnHeaderCnkID3.Text = "ID 3";
			// 
			// columnHeaderCnkType3
			// 
			columnHeaderCnkType3.Text = "Type 3";
			// 
			// buttonCloneMesh
			// 
			buttonCloneMesh.Enabled = false;
			buttonCloneMesh.Location = new System.Drawing.Point(8, 392);
			buttonCloneMesh.Margin = new System.Windows.Forms.Padding(4);
			buttonCloneMesh.Name = "buttonCloneMesh";
			buttonCloneMesh.Size = new System.Drawing.Size(135, 36);
			buttonCloneMesh.TabIndex = 14;
			buttonCloneMesh.Text = "Clone Mesh";
			buttonCloneMesh.UseVisualStyleBackColor = true;
			buttonCloneMesh.Click += buttonCloneMesh_Click;
			// 
			// buttonDeleteMesh
			// 
			buttonDeleteMesh.Enabled = false;
			buttonDeleteMesh.Location = new System.Drawing.Point(150, 392);
			buttonDeleteMesh.Margin = new System.Windows.Forms.Padding(4);
			buttonDeleteMesh.Name = "buttonDeleteMesh";
			buttonDeleteMesh.Size = new System.Drawing.Size(135, 36);
			buttonDeleteMesh.TabIndex = 15;
			buttonDeleteMesh.Text = "Delete Mesh";
			buttonDeleteMesh.UseVisualStyleBackColor = true;
			buttonDeleteMesh.Click += buttonDeleteMesh_Click;
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabelInfo });
			statusStrip1.Location = new System.Drawing.Point(0, 732);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 15, 0);
			statusStrip1.Size = new System.Drawing.Size(1137, 32);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 11;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelInfo
			// 
			toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
			toolStripStatusLabelInfo.Size = new System.Drawing.Size(663, 25);
			toolStripStatusLabelInfo.Text = "Click a mesh to display its information here. Right-click to edit pieces, if applicable.";
			// 
			// labelModelName
			// 
			labelModelName.AutoSize = true;
			labelModelName.Location = new System.Drawing.Point(24, 72);
			labelModelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelModelName.Name = "labelModelName";
			labelModelName.Size = new System.Drawing.Size(119, 25);
			labelModelName.TabIndex = 12;
			labelModelName.Text = "Model Name:";
			// 
			// labelMeshsetName
			// 
			labelMeshsetName.AutoSize = true;
			labelMeshsetName.Location = new System.Drawing.Point(432, 32);
			labelMeshsetName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelMeshsetName.Name = "labelMeshsetName";
			labelMeshsetName.Size = new System.Drawing.Size(101, 25);
			labelMeshsetName.TabIndex = 13;
			labelMeshsetName.Text = "Poly Name:";
			// 
			// textBoxMeshsetName
			// 
			textBoxMeshsetName.Location = new System.Drawing.Point(540, 27);
			textBoxMeshsetName.Name = "textBoxMeshsetName";
			textBoxMeshsetName.Size = new System.Drawing.Size(216, 31);
			textBoxMeshsetName.TabIndex = 4;
			textBoxMeshsetName.TextChanged += textBoxMeshsetName_TextChanged;
			// 
			// textBoxModelName
			// 
			textBoxModelName.Location = new System.Drawing.Point(150, 68);
			textBoxModelName.Name = "textBoxModelName";
			textBoxModelName.Size = new System.Drawing.Size(216, 31);
			textBoxModelName.TabIndex = 2;
			textBoxModelName.TextChanged += textBoxModelName_TextChanged;
			// 
			// textBoxModelY
			// 
			textBoxModelY.Location = new System.Drawing.Point(54, 68);
			textBoxModelY.Name = "textBoxModelY";
			textBoxModelY.Size = new System.Drawing.Size(118, 31);
			textBoxModelY.TabIndex = 8;
			textBoxModelY.TextChanged += textBoxModelY_TextChanged;
			// 
			// textBoxModelZ
			// 
			textBoxModelZ.Location = new System.Drawing.Point(54, 108);
			textBoxModelZ.Name = "textBoxModelZ";
			textBoxModelZ.Size = new System.Drawing.Size(118, 31);
			textBoxModelZ.TabIndex = 9;
			textBoxModelZ.TextChanged += textBoxModelZ_TextChanged;
			// 
			// textBoxModelX
			// 
			textBoxModelX.Location = new System.Drawing.Point(54, 27);
			textBoxModelX.Name = "textBoxModelX";
			textBoxModelX.Size = new System.Drawing.Size(118, 31);
			textBoxModelX.TabIndex = 7;
			textBoxModelX.TextChanged += textBoxModelX_TextChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(21, 32);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(27, 25);
			label4.TabIndex = 23;
			label4.Text = "X:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(21, 72);
			label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(26, 25);
			label5.TabIndex = 24;
			label5.Text = "Y:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(21, 112);
			label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(26, 25);
			label6.TabIndex = 25;
			label6.Text = "Z:";
			// 
			// groupBoxLabels
			// 
			groupBoxLabels.Controls.Add(textBoxObjectName);
			groupBoxLabels.Controls.Add(label7);
			groupBoxLabels.Controls.Add(textBoxVertexName);
			groupBoxLabels.Controls.Add(label2);
			groupBoxLabels.Controls.Add(textBoxModelName);
			groupBoxLabels.Controls.Add(labelModelName);
			groupBoxLabels.Controls.Add(labelMeshsetName);
			groupBoxLabels.Controls.Add(textBoxMeshsetName);
			groupBoxLabels.Location = new System.Drawing.Point(12, 60);
			groupBoxLabels.Name = "groupBoxLabels";
			groupBoxLabels.Size = new System.Drawing.Size(764, 156);
			groupBoxLabels.TabIndex = 26;
			groupBoxLabels.TabStop = false;
			groupBoxLabels.Text = "Labels";
			// 
			// textBoxObjectName
			// 
			textBoxObjectName.Location = new System.Drawing.Point(150, 27);
			textBoxObjectName.Name = "textBoxObjectName";
			textBoxObjectName.Size = new System.Drawing.Size(216, 31);
			textBoxObjectName.TabIndex = 1;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(22, 32);
			label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(120, 25);
			label7.TabIndex = 23;
			label7.Text = "Object Name:";
			// 
			// textBoxVertexName
			// 
			textBoxVertexName.Location = new System.Drawing.Point(540, 68);
			textBoxVertexName.Name = "textBoxVertexName";
			textBoxVertexName.Size = new System.Drawing.Size(216, 31);
			textBoxVertexName.TabIndex = 5;
			textBoxVertexName.TextChanged += textBoxVertexName_TextChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(405, 72);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(128, 25);
			label2.TabIndex = 19;
			label2.Text = "Vertices Name:";
			// 
			// groupBoxBounds
			// 
			groupBoxBounds.Controls.Add(textBoxModelRadius);
			groupBoxBounds.Controls.Add(labelR);
			groupBoxBounds.Controls.Add(textBoxModelX);
			groupBoxBounds.Controls.Add(textBoxModelY);
			groupBoxBounds.Controls.Add(label6);
			groupBoxBounds.Controls.Add(textBoxModelZ);
			groupBoxBounds.Controls.Add(label5);
			groupBoxBounds.Controls.Add(label4);
			groupBoxBounds.Location = new System.Drawing.Point(783, 60);
			groupBoxBounds.Name = "groupBoxBounds";
			groupBoxBounds.Size = new System.Drawing.Size(348, 156);
			groupBoxBounds.TabIndex = 27;
			groupBoxBounds.TabStop = false;
			groupBoxBounds.Text = "Model Bounds";
			// 
			// textBoxModelRadius
			// 
			textBoxModelRadius.Location = new System.Drawing.Point(192, 68);
			textBoxModelRadius.Name = "textBoxModelRadius";
			textBoxModelRadius.Size = new System.Drawing.Size(138, 31);
			textBoxModelRadius.TabIndex = 10;
			textBoxModelRadius.TextChanged += textBoxModelRadius_TextChanged;
			// 
			// labelR
			// 
			labelR.AutoSize = true;
			labelR.Location = new System.Drawing.Point(192, 32);
			labelR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelR.Name = "labelR";
			labelR.Size = new System.Drawing.Size(69, 25);
			labelR.TabIndex = 31;
			labelR.Text = "Radius:";
			// 
			// groupBoxMeshList
			// 
			groupBoxMeshList.Controls.Add(buttonResetMeshes);
			groupBoxMeshList.Controls.Add(buttonMoveMeshUp);
			groupBoxMeshList.Controls.Add(buttonMoveMeshDown);
			groupBoxMeshList.Controls.Add(listViewMeshes);
			groupBoxMeshList.Controls.Add(buttonDeleteMesh);
			groupBoxMeshList.Controls.Add(buttonCloneMesh);
			groupBoxMeshList.Location = new System.Drawing.Point(12, 240);
			groupBoxMeshList.Name = "groupBoxMeshList";
			groupBoxMeshList.Size = new System.Drawing.Size(1118, 441);
			groupBoxMeshList.TabIndex = 28;
			groupBoxMeshList.TabStop = false;
			groupBoxMeshList.Text = "Mesh List";
			// 
			// buttonResetMeshes
			// 
			buttonResetMeshes.Location = new System.Drawing.Point(927, 392);
			buttonResetMeshes.Margin = new System.Windows.Forms.Padding(4);
			buttonResetMeshes.Name = "buttonResetMeshes";
			buttonResetMeshes.Size = new System.Drawing.Size(135, 36);
			buttonResetMeshes.TabIndex = 16;
			buttonResetMeshes.Text = "Reset Meshes";
			buttonResetMeshes.UseVisualStyleBackColor = true;
			buttonResetMeshes.Click += buttonResetMeshes_Click;
			// 
			// buttonClose
			// 
			buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonClose.Location = new System.Drawing.Point(1005, 687);
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new System.Drawing.Size(126, 36);
			buttonClose.TabIndex = 17;
			buttonClose.Text = "Close";
			buttonClose.UseVisualStyleBackColor = true;
			buttonClose.Click += buttonClose_Click;
			// 
			// comboBoxNode
			// 
			comboBoxNode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxNode.FormattingEnabled = true;
			comboBoxNode.Location = new System.Drawing.Point(12, 18);
			comboBoxNode.Margin = new System.Windows.Forms.Padding(4);
			comboBoxNode.Name = "comboBoxNode";
			comboBoxNode.Size = new System.Drawing.Size(530, 33);
			comboBoxNode.TabIndex = 0;
			comboBoxNode.SelectedIndexChanged += comboBoxNode_SelectedIndexChanged;
			// 
			// contextMenuStripMatEdit
			// 
			contextMenuStripMatEdit.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripMatEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { editDiffuseToolStripMenuItem, editAmbientToolStripMenuItem, editSpecularToolStripMenuItem, editTextureIDToolStripMenuItem, editStripAlphaToolStripMenuItem });
			contextMenuStripMatEdit.Name = "contextMenuStripMatEdit";
			contextMenuStripMatEdit.Size = new System.Drawing.Size(207, 164);
			contextMenuStripMatEdit.Opening += contextMenuStrip2_Opening;
			// 
			// editDiffuseToolStripMenuItem
			// 
			editDiffuseToolStripMenuItem.Name = "editDiffuseToolStripMenuItem";
			editDiffuseToolStripMenuItem.Size = new System.Drawing.Size(206, 32);
			editDiffuseToolStripMenuItem.Text = "Edit Diffuse";
			this.editDiffuseToolStripMenuItem.Click += new System.EventHandler(this.editDiffuseToolStripMenuItem_Click);
			// 
			// editAmbientToolStripMenuItem
			// 
			editAmbientToolStripMenuItem.Name = "editAmbientToolStripMenuItem";
			editAmbientToolStripMenuItem.Size = new System.Drawing.Size(206, 32);
			editAmbientToolStripMenuItem.Text = "Edit Ambient";
			this.editAmbientToolStripMenuItem.Click += new System.EventHandler(this.editAmbientToolStripMenuItem_Click);
			// 
			// editSpecularToolStripMenuItem
			// 
			editSpecularToolStripMenuItem.Name = "editSpecularToolStripMenuItem";
			editSpecularToolStripMenuItem.Size = new System.Drawing.Size(206, 32);
			editSpecularToolStripMenuItem.Text = "Edit Specular";
			this.editSpecularToolStripMenuItem.Click += new System.EventHandler(this.editSpecularToolStripMenuItem_Click);
			// 
			// editTextureIDToolStripMenuItem
			// 
			editTextureIDToolStripMenuItem.Name = "editTextureIDToolStripMenuItem";
			editTextureIDToolStripMenuItem.Size = new System.Drawing.Size(206, 32);
			editTextureIDToolStripMenuItem.Text = "Edit Texture ID";
			this.editTextureIDToolStripMenuItem.Click += new System.EventHandler(this.editTextureIDToolStripMenuItem_Click);
			// 
			// editStripAlphaToolStripMenuItem
			// 
			editStripAlphaToolStripMenuItem.Name = "editStripAlphaToolStripMenuItem";
			editStripAlphaToolStripMenuItem.Size = new System.Drawing.Size(206, 32);
			editStripAlphaToolStripMenuItem.Text = "Edit Strip Alpha";
			this.editStripAlphaToolStripMenuItem.Click += new System.EventHandler(this.editStripAlphaToolStripMenuItem_Click);
			// 
			// ChunkModelDataEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			ClientSize = new System.Drawing.Size(1137, 764);
			Controls.Add(comboBoxNode);
			Controls.Add(buttonClose);
			Controls.Add(groupBoxMeshList);
			Controls.Add(groupBoxBounds);
			Controls.Add(groupBoxLabels);
			Controls.Add(statusStrip1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkModelDataEditor";
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Model Data Editor";
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			groupBoxLabels.ResumeLayout(false);
			groupBoxLabels.PerformLayout();
			groupBoxBounds.ResumeLayout(false);
			groupBoxBounds.PerformLayout();
			groupBoxMeshList.ResumeLayout(false);
			contextMenuStripMatEdit.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.Button buttonMoveMeshUp;
		private System.Windows.Forms.Button buttonMoveMeshDown;
		private System.Windows.Forms.ListView listViewMeshes;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkID1;
		private System.Windows.Forms.Button buttonCloneMesh;
		private System.Windows.Forms.Button buttonDeleteMesh;
		private System.Windows.Forms.ColumnHeader columnHeaderMatID;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkNum;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
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
		private System.Windows.Forms.ComboBox comboBoxNode;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkType1;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkType2;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkType3;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkID2;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkID3;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripMatEdit;
		private System.Windows.Forms.ToolStripMenuItem editDiffuseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editAmbientToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editSpecularToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editTextureIDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editStripAlphaToolStripMenuItem;
	}
}