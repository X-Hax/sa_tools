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
			columnHeaderCnkType = new System.Windows.Forms.ColumnHeader();
			columnHeaderCnkData = new System.Windows.Forms.ColumnHeader();
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
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			toolStripButtonAddPoly = new System.Windows.Forms.ToolStripDropDownButton();
			addMaterialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addBlendAlphaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addMipmapDAdjustToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addSpecExpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addBumpMatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addNullChunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			buttonResetMeshes = new System.Windows.Forms.Button();
			buttonClose = new System.Windows.Forms.Button();
			comboBoxNode = new System.Windows.Forms.ComboBox();
			contextMenuStripMatEdit = new System.Windows.Forms.ContextMenuStrip(components);
			editPCMatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editTextureIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editStripAlphaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editAlphaBlendDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			groupBoxVertList = new System.Windows.Forms.GroupBox();
			listViewVertices = new System.Windows.Forms.ListView();
			columnHeaderVertID = new System.Windows.Forms.ColumnHeader();
			columnHeaderVertType = new System.Windows.Forms.ColumnHeader();
			columnHeaderVertData = new System.Windows.Forms.ColumnHeader();
			columnHeaderIdxOff = new System.Windows.Forms.ColumnHeader();
			contextMenuStripVertCol = new System.Windows.Forms.ContextMenuStrip(components);
			showVertexCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			listViewObjectData = new System.Windows.Forms.ListView();
			columnHeaderEval = new System.Windows.Forms.ColumnHeader();
			columnHeaderPos = new System.Windows.Forms.ColumnHeader();
			columnHeaderRot = new System.Windows.Forms.ColumnHeader();
			columnHeaderScl = new System.Windows.Forms.ColumnHeader();
			groupBox1 = new System.Windows.Forms.GroupBox();
			contextMenuStripObjSet = new System.Windows.Forms.ContextMenuStrip(components);
			editObjectSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			statusStrip1.SuspendLayout();
			groupBoxLabels.SuspendLayout();
			groupBoxBounds.SuspendLayout();
			groupBoxMeshList.SuspendLayout();
			toolStrip1.SuspendLayout();
			contextMenuStripMatEdit.SuspendLayout();
			groupBoxVertList.SuspendLayout();
			contextMenuStripVertCol.SuspendLayout();
			groupBox1.SuspendLayout();
			contextMenuStripObjSet.SuspendLayout();
			SuspendLayout();
			// 
			// buttonMoveMeshUp
			// 
			buttonMoveMeshUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			buttonMoveMeshUp.Enabled = false;
			buttonMoveMeshUp.Location = new System.Drawing.Point(474, 27);
			buttonMoveMeshUp.Margin = new System.Windows.Forms.Padding(4);
			buttonMoveMeshUp.Name = "buttonMoveMeshUp";
			buttonMoveMeshUp.Size = new System.Drawing.Size(36, 36);
			buttonMoveMeshUp.TabIndex = 17;
			buttonMoveMeshUp.Text = "↑";
			buttonMoveMeshUp.UseVisualStyleBackColor = true;
			buttonMoveMeshUp.Click += buttonMoveMeshUp_Click;
			// 
			// buttonMoveMeshDown
			// 
			buttonMoveMeshDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			buttonMoveMeshDown.Enabled = false;
			buttonMoveMeshDown.Location = new System.Drawing.Point(474, 65);
			buttonMoveMeshDown.Margin = new System.Windows.Forms.Padding(4);
			buttonMoveMeshDown.Name = "buttonMoveMeshDown";
			buttonMoveMeshDown.Size = new System.Drawing.Size(36, 36);
			buttonMoveMeshDown.TabIndex = 18;
			buttonMoveMeshDown.Text = "↓";
			buttonMoveMeshDown.UseVisualStyleBackColor = true;
			buttonMoveMeshDown.Click += buttonMoveMeshDown_Click;
			// 
			// listViewMeshes
			// 
			listViewMeshes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			listViewMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderMatID, columnHeaderCnkType, columnHeaderCnkData });
			listViewMeshes.FullRowSelect = true;
			listViewMeshes.GridLines = true;
			listViewMeshes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewMeshes.Location = new System.Drawing.Point(8, 28);
			listViewMeshes.Margin = new System.Windows.Forms.Padding(4);
			listViewMeshes.MultiSelect = false;
			listViewMeshes.Name = "listViewMeshes";
			listViewMeshes.ShowGroups = false;
			listViewMeshes.Size = new System.Drawing.Size(464, 195);
			listViewMeshes.TabIndex = 16;
			listViewMeshes.UseCompatibleStateImageBehavior = false;
			listViewMeshes.View = System.Windows.Forms.View.Details;
			listViewMeshes.SelectedIndexChanged += listViewMeshes_SelectedIndexChanged;
			listViewMeshes.MouseClick += listViewMeshes_MouseClick;
			// 
			// columnHeaderMatID
			// 
			columnHeaderMatID.Text = "ID";
			// 
			// columnHeaderCnkType
			// 
			columnHeaderCnkType.Text = "Type";
			columnHeaderCnkType.Width = 180;
			// 
			// columnHeaderCnkData
			// 
			columnHeaderCnkData.Text = "Data";
			// 
			// buttonCloneMesh
			// 
			buttonCloneMesh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			buttonCloneMesh.Enabled = false;
			buttonCloneMesh.Location = new System.Drawing.Point(8, 225);
			buttonCloneMesh.Margin = new System.Windows.Forms.Padding(4);
			buttonCloneMesh.Name = "buttonCloneMesh";
			buttonCloneMesh.Size = new System.Drawing.Size(79, 30);
			buttonCloneMesh.TabIndex = 19;
			buttonCloneMesh.Text = "Clone Poly";
			buttonCloneMesh.UseVisualStyleBackColor = true;
			buttonCloneMesh.Click += buttonCloneMesh_Click;
			// 
			// buttonDeleteMesh
			// 
			buttonDeleteMesh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			buttonDeleteMesh.Enabled = false;
			buttonDeleteMesh.Location = new System.Drawing.Point(95, 225);
			buttonDeleteMesh.Margin = new System.Windows.Forms.Padding(4);
			buttonDeleteMesh.Name = "buttonDeleteMesh";
			buttonDeleteMesh.Size = new System.Drawing.Size(86, 30);
			buttonDeleteMesh.TabIndex = 20;
			buttonDeleteMesh.Text = "Delete Poly";
			buttonDeleteMesh.UseVisualStyleBackColor = true;
			buttonDeleteMesh.Click += buttonDeleteMesh_Click;
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabelInfo });
			statusStrip1.Location = new System.Drawing.Point(0, 537);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 15, 0);
			statusStrip1.Size = new System.Drawing.Size(950, 22);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 11;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelInfo
			// 
			toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
			toolStripStatusLabelInfo.Size = new System.Drawing.Size(489, 17);
			toolStripStatusLabelInfo.Text = "Click a material piece to display its information here. Right-click to edit pieces, if applicable.";
			// 
			// labelModelName
			// 
			labelModelName.AutoSize = true;
			labelModelName.Location = new System.Drawing.Point(9, 44);
			labelModelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelModelName.Name = "labelModelName";
			labelModelName.Size = new System.Drawing.Size(79, 15);
			labelModelName.TabIndex = 12;
			labelModelName.Text = "Model Name:";
			// 
			// labelMeshsetName
			// 
			labelMeshsetName.AutoSize = true;
			labelMeshsetName.Location = new System.Drawing.Point(266, 20);
			labelMeshsetName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelMeshsetName.Name = "labelMeshsetName";
			labelMeshsetName.Size = new System.Drawing.Size(68, 15);
			labelMeshsetName.TabIndex = 13;
			labelMeshsetName.Text = "Poly Name:";
			// 
			// textBoxMeshsetName
			// 
			textBoxMeshsetName.Location = new System.Drawing.Point(341, 16);
			textBoxMeshsetName.Name = "textBoxMeshsetName";
			textBoxMeshsetName.Size = new System.Drawing.Size(144, 23);
			textBoxMeshsetName.TabIndex = 4;
			textBoxMeshsetName.TextChanged += textBoxMeshsetName_TextChanged;
			// 
			// textBoxModelName
			// 
			textBoxModelName.Location = new System.Drawing.Point(95, 40);
			textBoxModelName.Name = "textBoxModelName";
			textBoxModelName.Size = new System.Drawing.Size(144, 23);
			textBoxModelName.TabIndex = 3;
			textBoxModelName.TextChanged += textBoxModelName_TextChanged;
			// 
			// textBoxModelY
			// 
			textBoxModelY.Location = new System.Drawing.Point(33, 40);
			textBoxModelY.Name = "textBoxModelY";
			textBoxModelY.Size = new System.Drawing.Size(118, 23);
			textBoxModelY.TabIndex = 8;
			textBoxModelY.TextChanged += textBoxModelY_TextChanged;
			// 
			// textBoxModelZ
			// 
			textBoxModelZ.Location = new System.Drawing.Point(33, 64);
			textBoxModelZ.Name = "textBoxModelZ";
			textBoxModelZ.Size = new System.Drawing.Size(118, 23);
			textBoxModelZ.TabIndex = 9;
			textBoxModelZ.TextChanged += textBoxModelZ_TextChanged;
			// 
			// textBoxModelX
			// 
			textBoxModelX.Location = new System.Drawing.Point(33, 16);
			textBoxModelX.Name = "textBoxModelX";
			textBoxModelX.Size = new System.Drawing.Size(118, 23);
			textBoxModelX.TabIndex = 7;
			textBoxModelX.TextChanged += textBoxModelX_TextChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(11, 20);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(17, 15);
			label4.TabIndex = 23;
			label4.Text = "X:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(11, 44);
			label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(17, 15);
			label5.TabIndex = 24;
			label5.Text = "Y:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(11, 68);
			label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(17, 15);
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
			groupBoxLabels.Location = new System.Drawing.Point(12, 35);
			groupBoxLabels.Name = "groupBoxLabels";
			groupBoxLabels.Size = new System.Drawing.Size(491, 71);
			groupBoxLabels.TabIndex = 1;
			groupBoxLabels.TabStop = false;
			groupBoxLabels.Text = "Labels";
			// 
			// textBoxObjectName
			// 
			textBoxObjectName.Location = new System.Drawing.Point(95, 16);
			textBoxObjectName.Name = "textBoxObjectName";
			textBoxObjectName.Size = new System.Drawing.Size(144, 23);
			textBoxObjectName.TabIndex = 2;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(8, 20);
			label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(80, 15);
			label7.TabIndex = 23;
			label7.Text = "Object Name:";
			// 
			// textBoxVertexName
			// 
			textBoxVertexName.Location = new System.Drawing.Point(341, 40);
			textBoxVertexName.Name = "textBoxVertexName";
			textBoxVertexName.Size = new System.Drawing.Size(144, 23);
			textBoxVertexName.TabIndex = 5;
			textBoxVertexName.TextChanged += textBoxVertexName_TextChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(249, 44);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(85, 15);
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
			groupBoxBounds.Location = new System.Drawing.Point(509, 35);
			groupBoxBounds.Name = "groupBoxBounds";
			groupBoxBounds.Size = new System.Drawing.Size(305, 96);
			groupBoxBounds.TabIndex = 6;
			groupBoxBounds.TabStop = false;
			groupBoxBounds.Text = "Model Bounds";
			// 
			// textBoxModelRadius
			// 
			textBoxModelRadius.Location = new System.Drawing.Point(157, 40);
			textBoxModelRadius.Name = "textBoxModelRadius";
			textBoxModelRadius.Size = new System.Drawing.Size(138, 23);
			textBoxModelRadius.TabIndex = 10;
			textBoxModelRadius.TextChanged += textBoxModelRadius_TextChanged;
			// 
			// labelR
			// 
			labelR.AutoSize = true;
			labelR.Location = new System.Drawing.Point(157, 20);
			labelR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelR.Name = "labelR";
			labelR.Size = new System.Drawing.Size(45, 15);
			labelR.TabIndex = 31;
			labelR.Text = "Radius:";
			// 
			// groupBoxMeshList
			// 
			groupBoxMeshList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			groupBoxMeshList.Controls.Add(toolStrip1);
			groupBoxMeshList.Controls.Add(buttonResetMeshes);
			groupBoxMeshList.Controls.Add(buttonMoveMeshUp);
			groupBoxMeshList.Controls.Add(buttonMoveMeshDown);
			groupBoxMeshList.Controls.Add(listViewMeshes);
			groupBoxMeshList.Controls.Add(buttonDeleteMesh);
			groupBoxMeshList.Controls.Add(buttonCloneMesh);
			groupBoxMeshList.Location = new System.Drawing.Point(425, 236);
			groupBoxMeshList.Name = "groupBoxMeshList";
			groupBoxMeshList.Size = new System.Drawing.Size(516, 260);
			groupBoxMeshList.TabIndex = 15;
			groupBoxMeshList.TabStop = false;
			groupBoxMeshList.Text = "Poly Data";
			// 
			// toolStrip1
			// 
			toolStrip1.AccessibleRole = System.Windows.Forms.AccessibleRole.ButtonDropDown;
			toolStrip1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			toolStrip1.GripMargin = new System.Windows.Forms.Padding(0);
			toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButtonAddPoly });
			toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			toolStrip1.Location = new System.Drawing.Point(300, 226);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Padding = new System.Windows.Forms.Padding(0);
			toolStrip1.Size = new System.Drawing.Size(109, 27);
			toolStrip1.TabIndex = 23;
			toolStrip1.TabStop = true;
			toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonAddPoly
			// 
			toolStripButtonAddPoly.AccessibleRole = System.Windows.Forms.AccessibleRole.ButtonDropDown;
			toolStripButtonAddPoly.AutoToolTip = false;
			toolStripButtonAddPoly.BackColor = System.Drawing.SystemColors.ControlLightLight;
			toolStripButtonAddPoly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			toolStripButtonAddPoly.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { addMaterialToolStripMenuItem, addTextureToolStripMenuItem, addBlendAlphaToolStripMenuItem, addMipmapDAdjustToolStripMenuItem, addSpecExpToolStripMenuItem, addBumpMatToolStripMenuItem, addNullChunkToolStripMenuItem });
			toolStripButtonAddPoly.ImageTransparentColor = System.Drawing.Color.Magenta;
			toolStripButtonAddPoly.Margin = new System.Windows.Forms.Padding(4);
			toolStripButtonAddPoly.Name = "toolStripButtonAddPoly";
			toolStripButtonAddPoly.Size = new System.Drawing.Size(68, 19);
			toolStripButtonAddPoly.Text = "Add Poly";
			// 
			// addMaterialToolStripMenuItem
			// 
			addMaterialToolStripMenuItem.Name = "addMaterialToolStripMenuItem";
			addMaterialToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			addMaterialToolStripMenuItem.Text = "Material";
			addMaterialToolStripMenuItem.Click += materialToolStripMenuItem_Click;
			// 
			// addTextureToolStripMenuItem
			// 
			addTextureToolStripMenuItem.Name = "addTextureToolStripMenuItem";
			addTextureToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			addTextureToolStripMenuItem.Text = "Texture";
			addTextureToolStripMenuItem.Click += textureToolStripMenuItem_Click;
			// 
			// addBlendAlphaToolStripMenuItem
			// 
			addBlendAlphaToolStripMenuItem.Name = "addBlendAlphaToolStripMenuItem";
			addBlendAlphaToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			addBlendAlphaToolStripMenuItem.Text = "Blend Alpha";
			addBlendAlphaToolStripMenuItem.Click += blendAlphaToolStripMenuItem_Click;
			// 
			// addMipmapDAdjustToolStripMenuItem
			// 
			addMipmapDAdjustToolStripMenuItem.Name = "addMipmapDAdjustToolStripMenuItem";
			addMipmapDAdjustToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			addMipmapDAdjustToolStripMenuItem.Text = "Mipmap 'D' Adjust";
			addMipmapDAdjustToolStripMenuItem.Click += mipmapDAdjustToolStripMenuItem_Click;
			// 
			// addSpecExpToolStripMenuItem
			// 
			addSpecExpToolStripMenuItem.Name = "addSpecExpToolStripMenuItem";
			addSpecExpToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			addSpecExpToolStripMenuItem.Text = "Specular Exponent";
			addSpecExpToolStripMenuItem.Click += specularExponentToolStripMenuItem_Click;
			// 
			// addBumpMatToolStripMenuItem
			// 
			addBumpMatToolStripMenuItem.Name = "addBumpMatToolStripMenuItem";
			addBumpMatToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			addBumpMatToolStripMenuItem.Text = "Bump Material";
			addBumpMatToolStripMenuItem.Click += bumpMaterialToolStripMenuItem_Click;
			// 
			// addNullChunkToolStripMenuItem
			// 
			addNullChunkToolStripMenuItem.Name = "addNullChunkToolStripMenuItem";
			addNullChunkToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			addNullChunkToolStripMenuItem.Text = "Null Chunk";
			addNullChunkToolStripMenuItem.Click += nullChunkToolStripMenuItem_Click;
			// 
			// buttonResetMeshes
			// 
			buttonResetMeshes.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			buttonResetMeshes.Location = new System.Drawing.Point(189, 225);
			buttonResetMeshes.Margin = new System.Windows.Forms.Padding(4);
			buttonResetMeshes.Name = "buttonResetMeshes";
			buttonResetMeshes.Size = new System.Drawing.Size(107, 30);
			buttonResetMeshes.TabIndex = 21;
			buttonResetMeshes.Text = "Reset Poly Data";
			buttonResetMeshes.UseVisualStyleBackColor = true;
			buttonResetMeshes.Click += buttonResetMeshes_Click;
			// 
			// buttonClose
			// 
			buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonClose.Location = new System.Drawing.Point(869, 500);
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new System.Drawing.Size(73, 30);
			buttonClose.TabIndex = 23;
			buttonClose.Text = "Close";
			buttonClose.UseVisualStyleBackColor = true;
			buttonClose.Click += buttonClose_Click;
			// 
			// comboBoxNode
			// 
			comboBoxNode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxNode.FormattingEnabled = true;
			comboBoxNode.Location = new System.Drawing.Point(10, 10);
			comboBoxNode.Margin = new System.Windows.Forms.Padding(4);
			comboBoxNode.Name = "comboBoxNode";
			comboBoxNode.Size = new System.Drawing.Size(241, 23);
			comboBoxNode.TabIndex = 0;
			comboBoxNode.SelectedIndexChanged += comboBoxNode_SelectedIndexChanged;
			// 
			// contextMenuStripMatEdit
			// 
			contextMenuStripMatEdit.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripMatEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { editPCMatToolStripMenuItem, editTextureIDToolStripMenuItem, editStripAlphaToolStripMenuItem, editAlphaBlendDataToolStripMenuItem });
			contextMenuStripMatEdit.Name = "contextMenuStripMatEdit";
			contextMenuStripMatEdit.Size = new System.Drawing.Size(189, 92);
			contextMenuStripMatEdit.Opening += contextMenuStrip2_Opening;
			// 
			// editPCMatToolStripMenuItem
			// 
			editPCMatToolStripMenuItem.Name = "editPCMatToolStripMenuItem";
			editPCMatToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			editPCMatToolStripMenuItem.Text = "Edit Material Data";
			editPCMatToolStripMenuItem.Click += editDiffuseToolStripMenuItem_Click;
			// 
			// editTextureIDToolStripMenuItem
			// 
			editTextureIDToolStripMenuItem.Name = "editTextureIDToolStripMenuItem";
			editTextureIDToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			editTextureIDToolStripMenuItem.Text = "Edit Texture Data";
			editTextureIDToolStripMenuItem.Click += editTextureIDToolStripMenuItem_Click;
			// 
			// editStripAlphaToolStripMenuItem
			// 
			editStripAlphaToolStripMenuItem.Name = "editStripAlphaToolStripMenuItem";
			editStripAlphaToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			editStripAlphaToolStripMenuItem.Text = "Edit Strip Data";
			editStripAlphaToolStripMenuItem.Click += editStripAlphaToolStripMenuItem_Click;
			// 
			// editAlphaBlendDataToolStripMenuItem
			// 
			editAlphaBlendDataToolStripMenuItem.Name = "editAlphaBlendDataToolStripMenuItem";
			editAlphaBlendDataToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			editAlphaBlendDataToolStripMenuItem.Text = "Edit Blend Alpha Data";
			editAlphaBlendDataToolStripMenuItem.Click += editBlendAlphaToolStripMenuItem_Click;
			// 
			// groupBoxVertList
			// 
			groupBoxVertList.Controls.Add(listViewVertices);
			groupBoxVertList.Location = new System.Drawing.Point(12, 236);
			groupBoxVertList.Name = "groupBoxVertList";
			groupBoxVertList.Size = new System.Drawing.Size(408, 223);
			groupBoxVertList.TabIndex = 13;
			groupBoxVertList.TabStop = false;
			groupBoxVertList.Text = "Vertex Data";
			// 
			// listViewVertices
			// 
			listViewVertices.AutoArrange = false;
			listViewVertices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderVertID, columnHeaderVertType, columnHeaderVertData, columnHeaderIdxOff });
			listViewVertices.FullRowSelect = true;
			listViewVertices.GridLines = true;
			listViewVertices.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewVertices.Location = new System.Drawing.Point(6, 28);
			listViewVertices.MultiSelect = false;
			listViewVertices.Name = "listViewVertices";
			listViewVertices.ShowGroups = false;
			listViewVertices.Size = new System.Drawing.Size(396, 148);
			listViewVertices.TabIndex = 14;
			listViewVertices.UseCompatibleStateImageBehavior = false;
			listViewVertices.View = System.Windows.Forms.View.Details;
			listViewVertices.SelectedIndexChanged += listViewVertices_SelectedIndexChanged;
			listViewVertices.MouseClick += listViewVertices_MouseClick;
			// 
			// columnHeaderVertID
			// 
			columnHeaderVertID.Text = "ID";
			// 
			// columnHeaderVertType
			// 
			columnHeaderVertType.Text = "Type";
			// 
			// columnHeaderVertData
			// 
			columnHeaderVertData.Text = "Data";
			// 
			// columnHeaderIdxOff
			// 
			columnHeaderIdxOff.Text = "Index Offset";
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
			showVertexCollectionToolStripMenuItem.Click += showVertexCollectionToolStripMenuItem_Click;
			// 
			// listViewObjectData
			// 
			listViewObjectData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderEval, columnHeaderPos, columnHeaderRot, columnHeaderScl });
			listViewObjectData.FullRowSelect = true;
			listViewObjectData.GridLines = true;
			listViewObjectData.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewObjectData.Location = new System.Drawing.Point(14, 27);
			listViewObjectData.MultiSelect = false;
			listViewObjectData.Name = "listViewObjectData";
			listViewObjectData.ShowGroups = false;
			listViewObjectData.Size = new System.Drawing.Size(871, 60);
			listViewObjectData.TabIndex = 12;
			listViewObjectData.UseCompatibleStateImageBehavior = false;
			listViewObjectData.View = System.Windows.Forms.View.Details;
			listViewObjectData.SelectedIndexChanged += listView1_SelectedIndexChanged;
			listViewObjectData.MouseClick += listViewObjectData_MouseClick;
			// 
			// columnHeaderEval
			// 
			columnHeaderEval.Text = "Eval Flags";
			// 
			// columnHeaderPos
			// 
			columnHeaderPos.Text = "Position";
			// 
			// columnHeaderRot
			// 
			columnHeaderRot.Text = "Rotation";
			// 
			// columnHeaderScl
			// 
			columnHeaderScl.Text = "Scale";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(listViewObjectData);
			groupBox1.Location = new System.Drawing.Point(12, 133);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(897, 101);
			groupBox1.TabIndex = 11;
			groupBox1.TabStop = false;
			groupBox1.Text = "Object Data";
			// 
			// contextMenuStripObjSet
			// 
			contextMenuStripObjSet.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripObjSet.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { editObjectSettingsToolStripMenuItem });
			contextMenuStripObjSet.Name = "contextMenuStripObjSet";
			contextMenuStripObjSet.Size = new System.Drawing.Size(178, 26);
			// 
			// editObjectSettingsToolStripMenuItem
			// 
			editObjectSettingsToolStripMenuItem.Name = "editObjectSettingsToolStripMenuItem";
			editObjectSettingsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			editObjectSettingsToolStripMenuItem.Text = "Edit Object Settings";
			editObjectSettingsToolStripMenuItem.Click += editObjectSettingsToolStripMenuItem_Click;
			// 
			// ChunkModelDataEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(950, 559);
			Controls.Add(groupBox1);
			Controls.Add(groupBoxVertList);
			Controls.Add(comboBoxNode);
			Controls.Add(buttonClose);
			Controls.Add(groupBoxMeshList);
			Controls.Add(groupBoxBounds);
			Controls.Add(groupBoxLabels);
			Controls.Add(statusStrip1);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkModelDataEditor";
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Model Data Editor";
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			groupBoxLabels.ResumeLayout(false);
			groupBoxLabels.PerformLayout();
			groupBoxBounds.ResumeLayout(false);
			groupBoxBounds.PerformLayout();
			groupBoxMeshList.ResumeLayout(false);
			groupBoxMeshList.PerformLayout();
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			contextMenuStripMatEdit.ResumeLayout(false);
			groupBoxVertList.ResumeLayout(false);
			contextMenuStripVertCol.ResumeLayout(false);
			groupBox1.ResumeLayout(false);
			contextMenuStripObjSet.ResumeLayout(false);
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
		private System.Windows.Forms.ColumnHeader columnHeaderCnkType;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkData;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripMatEdit;
		private System.Windows.Forms.ToolStripMenuItem editPCMatToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editTextureIDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editStripAlphaToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBoxVertList;
		private System.Windows.Forms.ListView listViewVertices;
		private System.Windows.Forms.ColumnHeader columnHeaderVertID;
		private System.Windows.Forms.ColumnHeader columnHeaderVertType;
		private System.Windows.Forms.ColumnHeader columnHeaderVertData;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripVertCol;
		private System.Windows.Forms.ToolStripMenuItem showVertexCollectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editAlphaBlendDataToolStripMenuItem;
		private System.Windows.Forms.ListView listViewObjectData;
		private System.Windows.Forms.ColumnHeader columnHeaderEval;
		private System.Windows.Forms.ColumnHeader columnHeaderPos;
		private System.Windows.Forms.ColumnHeader columnHeaderRot;
		private System.Windows.Forms.ColumnHeader columnHeaderScl;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripObjSet;
		private System.Windows.Forms.ToolStripMenuItem editObjectSettingsToolStripMenuItem;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripDropDownButton toolStripButtonAddPoly;
		private System.Windows.Forms.ToolStripMenuItem addMaterialToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addTextureToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addBlendAlphaToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addMipmapDAdjustToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addSpecExpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addBumpMatToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addNullChunkToolStripMenuItem;
		private System.Windows.Forms.ColumnHeader columnHeaderIdxOff;
	}
}