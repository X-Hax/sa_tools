namespace SAModel.SAEditorCommon.UI
{
	partial class ModelDataEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelDataEditor));
            this.buttonMoveMeshUp = new System.Windows.Forms.Button();
            this.buttonMoveMeshDown = new System.Windows.Forms.Button();
            this.listViewMeshes = new System.Windows.Forms.ListView();
            this.columnHeaderIndex = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderMatID = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderType = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderPoly = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderUV = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderVcolor = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderPolynormals = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTrans = new System.Windows.Forms.ColumnHeader();
            this.buttonCloneMesh = new System.Windows.Forms.Button();
            this.buttonDeleteMesh = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelModelName = new System.Windows.Forms.Label();
            this.labelMeshsetName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMaterialName = new System.Windows.Forms.TextBox();
            this.textBoxMeshsetName = new System.Windows.Forms.TextBox();
            this.textBoxModelName = new System.Windows.Forms.TextBox();
            this.textBoxModelY = new System.Windows.Forms.TextBox();
            this.textBoxModelZ = new System.Windows.Forms.TextBox();
            this.textBoxModelX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBoxLabels = new System.Windows.Forms.GroupBox();
            this.textBoxObjectName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxNormalName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxVertexName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxBounds = new System.Windows.Forms.GroupBox();
            this.textBoxModelRadius = new System.Windows.Forms.TextBox();
            this.labelR = new System.Windows.Forms.Label();
            this.groupBoxMeshList = new System.Windows.Forms.GroupBox();
            this.buttonResetMeshes = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.contextMenuStripLabels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemEditPolyName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEditUVName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEditVcolorName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEditPolynormalName = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxNode = new System.Windows.Forms.ComboBox();
            this.statusStrip1.SuspendLayout();
            this.groupBoxLabels.SuspendLayout();
            this.groupBoxBounds.SuspendLayout();
            this.groupBoxMeshList.SuspendLayout();
            this.contextMenuStripLabels.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonMoveMeshUp
            // 
            this.buttonMoveMeshUp.Enabled = false;
            this.buttonMoveMeshUp.Location = new System.Drawing.Point(714, 21);
            this.buttonMoveMeshUp.Name = "buttonMoveMeshUp";
            this.buttonMoveMeshUp.Size = new System.Drawing.Size(24, 24);
            this.buttonMoveMeshUp.TabIndex = 12;
            this.buttonMoveMeshUp.Text = "↑";
            this.buttonMoveMeshUp.UseVisualStyleBackColor = true;
            this.buttonMoveMeshUp.Click += new System.EventHandler(this.buttonMoveMeshUp_Click);
            // 
            // buttonMoveMeshDown
            // 
            this.buttonMoveMeshDown.Enabled = false;
            this.buttonMoveMeshDown.Location = new System.Drawing.Point(714, 51);
            this.buttonMoveMeshDown.Name = "buttonMoveMeshDown";
            this.buttonMoveMeshDown.Size = new System.Drawing.Size(24, 24);
            this.buttonMoveMeshDown.TabIndex = 13;
            this.buttonMoveMeshDown.Text = "↓";
            this.buttonMoveMeshDown.UseVisualStyleBackColor = true;
            this.buttonMoveMeshDown.Click += new System.EventHandler(this.buttonMoveMeshDown_Click);
            // 
            // listViewMeshes
            // 
            this.listViewMeshes.AutoArrange = false;
            this.listViewMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderIndex,
            this.columnHeaderMatID,
            this.columnHeaderType,
            this.columnHeaderPoly,
            this.columnHeaderUV,
            this.columnHeaderVcolor,
            this.columnHeaderPolynormals,
            this.columnHeaderTrans});
            this.listViewMeshes.FullRowSelect = true;
            this.listViewMeshes.GridLines = true;
            this.listViewMeshes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewMeshes.HideSelection = false;
            this.listViewMeshes.Location = new System.Drawing.Point(5, 19);
            this.listViewMeshes.MultiSelect = false;
            this.listViewMeshes.Name = "listViewMeshes";
            this.listViewMeshes.ShowGroups = false;
            this.listViewMeshes.Size = new System.Drawing.Size(703, 236);
            this.listViewMeshes.TabIndex = 11;
            this.listViewMeshes.UseCompatibleStateImageBehavior = false;
            this.listViewMeshes.View = System.Windows.Forms.View.Details;
            this.listViewMeshes.SelectedIndexChanged += new System.EventHandler(this.listViewMeshes_SelectedIndexChanged);
            this.listViewMeshes.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewMeshes_MouseClick);
            // 
            // columnHeaderIndex
            // 
            this.columnHeaderIndex.Text = "Index";
            // 
            // columnHeaderMatID
            // 
            this.columnHeaderMatID.Text = "Material";
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            // 
            // columnHeaderPoly
            // 
            this.columnHeaderPoly.Text = "Polys";
            // 
            // columnHeaderUV
            // 
            this.columnHeaderUV.Text = "UVs";
            // 
            // columnHeaderVcolor
            // 
            this.columnHeaderVcolor.Text = "Vertex Colors";
            // 
            // columnHeaderPolynormals
            // 
            this.columnHeaderPolynormals.Text = "Polynormals";
            // 
            // columnHeaderTrans
            // 
            this.columnHeaderTrans.Text = "Uses Alpha?";
            // 
            // buttonCloneMesh
            // 
            this.buttonCloneMesh.Enabled = false;
            this.buttonCloneMesh.Location = new System.Drawing.Point(5, 261);
            this.buttonCloneMesh.Name = "buttonCloneMesh";
            this.buttonCloneMesh.Size = new System.Drawing.Size(90, 24);
            this.buttonCloneMesh.TabIndex = 14;
            this.buttonCloneMesh.Text = "Clone Mesh";
            this.buttonCloneMesh.UseVisualStyleBackColor = true;
            this.buttonCloneMesh.Click += new System.EventHandler(this.buttonCloneMesh_Click);
            // 
            // buttonDeleteMesh
            // 
            this.buttonDeleteMesh.Enabled = false;
            this.buttonDeleteMesh.Location = new System.Drawing.Point(100, 261);
            this.buttonDeleteMesh.Name = "buttonDeleteMesh";
            this.buttonDeleteMesh.Size = new System.Drawing.Size(90, 24);
            this.buttonDeleteMesh.TabIndex = 15;
            this.buttonDeleteMesh.Text = "Delete Mesh";
            this.buttonDeleteMesh.UseVisualStyleBackColor = true;
            this.buttonDeleteMesh.Click += new System.EventHandler(this.buttonDeleteMesh_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 487);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(758, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelInfo
            // 
            this.toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            this.toolStripStatusLabelInfo.Size = new System.Drawing.Size(343, 17);
            this.toolStripStatusLabelInfo.Text = "Click a mesh to display its information. Right click to edit labels.";
            // 
            // labelModelName
            // 
            this.labelModelName.AutoSize = true;
            this.labelModelName.Location = new System.Drawing.Point(16, 48);
            this.labelModelName.Name = "labelModelName";
            this.labelModelName.Size = new System.Drawing.Size(79, 15);
            this.labelModelName.TabIndex = 12;
            this.labelModelName.Text = "Model Name:";
            // 
            // labelMeshsetName
            // 
            this.labelMeshsetName.AutoSize = true;
            this.labelMeshsetName.Location = new System.Drawing.Point(266, 21);
            this.labelMeshsetName.Name = "labelMeshsetName";
            this.labelMeshsetName.Size = new System.Drawing.Size(89, 15);
            this.labelMeshsetName.TabIndex = 13;
            this.labelMeshsetName.Text = "Meshset Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 15;
            this.label1.Text = "Material Name:";
            // 
            // textBoxMaterialName
            // 
            this.textBoxMaterialName.Location = new System.Drawing.Point(100, 72);
            this.textBoxMaterialName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxMaterialName.Name = "textBoxMaterialName";
            this.textBoxMaterialName.Size = new System.Drawing.Size(145, 23);
            this.textBoxMaterialName.TabIndex = 3;
            this.textBoxMaterialName.TextChanged += new System.EventHandler(this.textBoxMaterialName_TextChanged);
            // 
            // textBoxMeshsetName
            // 
            this.textBoxMeshsetName.Location = new System.Drawing.Point(360, 18);
            this.textBoxMeshsetName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxMeshsetName.Name = "textBoxMeshsetName";
            this.textBoxMeshsetName.Size = new System.Drawing.Size(145, 23);
            this.textBoxMeshsetName.TabIndex = 4;
            this.textBoxMeshsetName.TextChanged += new System.EventHandler(this.textBoxMeshsetName_TextChanged);
            // 
            // textBoxModelName
            // 
            this.textBoxModelName.Location = new System.Drawing.Point(100, 45);
            this.textBoxModelName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxModelName.Name = "textBoxModelName";
            this.textBoxModelName.Size = new System.Drawing.Size(145, 23);
            this.textBoxModelName.TabIndex = 2;
            this.textBoxModelName.TextChanged += new System.EventHandler(this.textBoxModelName_TextChanged);
            // 
            // textBoxModelY
            // 
            this.textBoxModelY.Location = new System.Drawing.Point(36, 45);
            this.textBoxModelY.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxModelY.Name = "textBoxModelY";
            this.textBoxModelY.Size = new System.Drawing.Size(80, 23);
            this.textBoxModelY.TabIndex = 8;
            this.textBoxModelY.TextChanged += new System.EventHandler(this.textBoxModelY_TextChanged);
            // 
            // textBoxModelZ
            // 
            this.textBoxModelZ.Location = new System.Drawing.Point(36, 72);
            this.textBoxModelZ.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxModelZ.Name = "textBoxModelZ";
            this.textBoxModelZ.Size = new System.Drawing.Size(80, 23);
            this.textBoxModelZ.TabIndex = 9;
            this.textBoxModelZ.TextChanged += new System.EventHandler(this.textBoxModelZ_TextChanged);
            // 
            // textBoxModelX
            // 
            this.textBoxModelX.Location = new System.Drawing.Point(36, 18);
            this.textBoxModelX.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxModelX.Name = "textBoxModelX";
            this.textBoxModelX.Size = new System.Drawing.Size(80, 23);
            this.textBoxModelX.TabIndex = 7;
            this.textBoxModelX.TextChanged += new System.EventHandler(this.textBoxModelX_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 15);
            this.label4.TabIndex = 23;
            this.label4.Text = "X:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 15);
            this.label5.TabIndex = 24;
            this.label5.Text = "Y:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 15);
            this.label6.TabIndex = 25;
            this.label6.Text = "Z:";
            // 
            // groupBoxLabels
            // 
            this.groupBoxLabels.Controls.Add(this.textBoxObjectName);
            this.groupBoxLabels.Controls.Add(this.label7);
            this.groupBoxLabels.Controls.Add(this.textBoxNormalName);
            this.groupBoxLabels.Controls.Add(this.label3);
            this.groupBoxLabels.Controls.Add(this.textBoxVertexName);
            this.groupBoxLabels.Controls.Add(this.label2);
            this.groupBoxLabels.Controls.Add(this.textBoxModelName);
            this.groupBoxLabels.Controls.Add(this.labelModelName);
            this.groupBoxLabels.Controls.Add(this.labelMeshsetName);
            this.groupBoxLabels.Controls.Add(this.label1);
            this.groupBoxLabels.Controls.Add(this.textBoxMaterialName);
            this.groupBoxLabels.Controls.Add(this.textBoxMeshsetName);
            this.groupBoxLabels.Location = new System.Drawing.Point(8, 40);
            this.groupBoxLabels.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxLabels.Name = "groupBoxLabels";
            this.groupBoxLabels.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxLabels.Size = new System.Drawing.Size(509, 104);
            this.groupBoxLabels.TabIndex = 26;
            this.groupBoxLabels.TabStop = false;
            this.groupBoxLabels.Text = "Labels";
            // 
            // textBoxObjectName
            // 
            this.textBoxObjectName.Location = new System.Drawing.Point(100, 18);
            this.textBoxObjectName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxObjectName.Name = "textBoxObjectName";
            this.textBoxObjectName.Size = new System.Drawing.Size(145, 23);
            this.textBoxObjectName.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 15);
            this.label7.TabIndex = 23;
            this.label7.Text = "Object Name:";
            // 
            // textBoxNormalName
            // 
            this.textBoxNormalName.Location = new System.Drawing.Point(360, 72);
            this.textBoxNormalName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxNormalName.Name = "textBoxNormalName";
            this.textBoxNormalName.Size = new System.Drawing.Size(145, 23);
            this.textBoxNormalName.TabIndex = 6;
            this.textBoxNormalName.TextChanged += new System.EventHandler(this.textBoxNormalName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(265, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 15);
            this.label3.TabIndex = 21;
            this.label3.Text = "Normals Name:";
            // 
            // textBoxVertexName
            // 
            this.textBoxVertexName.Location = new System.Drawing.Point(360, 45);
            this.textBoxVertexName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxVertexName.Name = "textBoxVertexName";
            this.textBoxVertexName.Size = new System.Drawing.Size(145, 23);
            this.textBoxVertexName.TabIndex = 5;
            this.textBoxVertexName.TextChanged += new System.EventHandler(this.textBoxVertexName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 15);
            this.label2.TabIndex = 19;
            this.label2.Text = "Vertices Name:";
            // 
            // groupBoxBounds
            // 
            this.groupBoxBounds.Controls.Add(this.textBoxModelRadius);
            this.groupBoxBounds.Controls.Add(this.labelR);
            this.groupBoxBounds.Controls.Add(this.textBoxModelX);
            this.groupBoxBounds.Controls.Add(this.textBoxModelY);
            this.groupBoxBounds.Controls.Add(this.label6);
            this.groupBoxBounds.Controls.Add(this.textBoxModelZ);
            this.groupBoxBounds.Controls.Add(this.label5);
            this.groupBoxBounds.Controls.Add(this.label4);
            this.groupBoxBounds.Location = new System.Drawing.Point(522, 40);
            this.groupBoxBounds.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxBounds.Name = "groupBoxBounds";
            this.groupBoxBounds.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxBounds.Size = new System.Drawing.Size(232, 104);
            this.groupBoxBounds.TabIndex = 27;
            this.groupBoxBounds.TabStop = false;
            this.groupBoxBounds.Text = "Model Bounds";
            // 
            // textBoxModelRadius
            // 
            this.textBoxModelRadius.Location = new System.Drawing.Point(128, 45);
            this.textBoxModelRadius.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxModelRadius.Name = "textBoxModelRadius";
            this.textBoxModelRadius.Size = new System.Drawing.Size(93, 23);
            this.textBoxModelRadius.TabIndex = 10;
            this.textBoxModelRadius.TextChanged += new System.EventHandler(this.textBoxModelRadius_TextChanged);
            // 
            // labelR
            // 
            this.labelR.AutoSize = true;
            this.labelR.Location = new System.Drawing.Point(128, 21);
            this.labelR.Name = "labelR";
            this.labelR.Size = new System.Drawing.Size(45, 15);
            this.labelR.TabIndex = 31;
            this.labelR.Text = "Radius:";
            // 
            // groupBoxMeshList
            // 
            this.groupBoxMeshList.Controls.Add(this.buttonResetMeshes);
            this.groupBoxMeshList.Controls.Add(this.buttonMoveMeshUp);
            this.groupBoxMeshList.Controls.Add(this.buttonMoveMeshDown);
            this.groupBoxMeshList.Controls.Add(this.listViewMeshes);
            this.groupBoxMeshList.Controls.Add(this.buttonDeleteMesh);
            this.groupBoxMeshList.Controls.Add(this.buttonCloneMesh);
            this.groupBoxMeshList.Location = new System.Drawing.Point(8, 160);
            this.groupBoxMeshList.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxMeshList.Name = "groupBoxMeshList";
            this.groupBoxMeshList.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxMeshList.Size = new System.Drawing.Size(745, 294);
            this.groupBoxMeshList.TabIndex = 28;
            this.groupBoxMeshList.TabStop = false;
            this.groupBoxMeshList.Text = "Mesh List";
            // 
            // buttonResetMeshes
            // 
            this.buttonResetMeshes.Location = new System.Drawing.Point(618, 261);
            this.buttonResetMeshes.Name = "buttonResetMeshes";
            this.buttonResetMeshes.Size = new System.Drawing.Size(90, 24);
            this.buttonResetMeshes.TabIndex = 16;
            this.buttonResetMeshes.Text = "Reset Meshes";
            this.buttonResetMeshes.UseVisualStyleBackColor = true;
            this.buttonResetMeshes.Click += new System.EventHandler(this.buttonResetMeshes_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(670, 458);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(84, 24);
            this.buttonClose.TabIndex = 17;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// contextMenuStripLabels
			// 
			this.contextMenuStripLabels.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripLabels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEditPolyName,
            this.toolStripMenuItemEditUVName,
            this.toolStripMenuItemEditVcolorName,
            this.toolStripMenuItemEditPolynormalName});
            this.contextMenuStripLabels.Name = "contextMenuStripLabels";
            this.contextMenuStripLabels.Size = new System.Drawing.Size(203, 92);
            // 
            // toolStripMenuItemEditPolyName
            // 
            this.toolStripMenuItemEditPolyName.Name = "toolStripMenuItemEditPolyName";
            this.toolStripMenuItemEditPolyName.Size = new System.Drawing.Size(202, 22);
            this.toolStripMenuItemEditPolyName.Text = "Edit Polys Name...";
            this.toolStripMenuItemEditPolyName.Click += new System.EventHandler(this.toolStripMenuItemEditPolyName_Click);
            // 
            // toolStripMenuItemEditUVName
            // 
            this.toolStripMenuItemEditUVName.Name = "toolStripMenuItemEditUVName";
            this.toolStripMenuItemEditUVName.Size = new System.Drawing.Size(202, 22);
            this.toolStripMenuItemEditUVName.Text = "Edit UVs Name...";
            this.toolStripMenuItemEditUVName.Click += new System.EventHandler(this.toolStripMenuItemEditUVName_Click);
            // 
            // toolStripMenuItemEditVcolorName
            // 
            this.toolStripMenuItemEditVcolorName.Name = "toolStripMenuItemEditVcolorName";
            this.toolStripMenuItemEditVcolorName.Size = new System.Drawing.Size(202, 22);
            this.toolStripMenuItemEditVcolorName.Text = "Edit VColor Name...";
            this.toolStripMenuItemEditVcolorName.Click += new System.EventHandler(this.toolStripMenuItemEditVcolorName_Click);
            // 
            // toolStripMenuItemEditPolynormalName
            // 
            this.toolStripMenuItemEditPolynormalName.Name = "toolStripMenuItemEditPolynormalName";
            this.toolStripMenuItemEditPolynormalName.Size = new System.Drawing.Size(202, 22);
            this.toolStripMenuItemEditPolynormalName.Text = "Edit Polynormal Name...";
            this.toolStripMenuItemEditPolynormalName.Click += new System.EventHandler(this.toolStripMenuItemEditPolynormalName_Click);
            // 
            // comboBoxNode
            // 
            this.comboBoxNode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNode.FormattingEnabled = true;
            this.comboBoxNode.Location = new System.Drawing.Point(8, 12);
            this.comboBoxNode.Name = "comboBoxNode";
            this.comboBoxNode.Size = new System.Drawing.Size(355, 23);
            this.comboBoxNode.TabIndex = 0;
            this.comboBoxNode.SelectedIndexChanged += new System.EventHandler(this.comboBoxNode_SelectedIndexChanged);
            // 
            // ModelDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(758, 509);
            this.Controls.Add(this.comboBoxNode);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBoxMeshList);
            this.Controls.Add(this.groupBoxBounds);
            this.Controls.Add(this.groupBoxLabels);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelDataEditor";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Model Data Editor";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxLabels.ResumeLayout(false);
            this.groupBoxLabels.PerformLayout();
            this.groupBoxBounds.ResumeLayout(false);
            this.groupBoxBounds.PerformLayout();
            this.groupBoxMeshList.ResumeLayout(false);
            this.contextMenuStripLabels.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button buttonMoveMeshUp;
		private System.Windows.Forms.Button buttonMoveMeshDown;
		private System.Windows.Forms.ListView listViewMeshes;
		private System.Windows.Forms.ColumnHeader columnHeaderIndex;
		private System.Windows.Forms.ColumnHeader columnHeaderType;
		private System.Windows.Forms.ColumnHeader columnHeaderUV;
		private System.Windows.Forms.ColumnHeader columnHeaderVcolor;
		private System.Windows.Forms.ColumnHeader columnHeaderPolynormals;
		private System.Windows.Forms.Button buttonCloneMesh;
		private System.Windows.Forms.Button buttonDeleteMesh;
		private System.Windows.Forms.ColumnHeader columnHeaderMatID;
		private System.Windows.Forms.ColumnHeader columnHeaderTrans;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
		private System.Windows.Forms.Label labelModelName;
		private System.Windows.Forms.Label labelMeshsetName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxMaterialName;
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
		private System.Windows.Forms.TextBox textBoxNormalName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxVertexName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ColumnHeader columnHeaderPoly;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripLabels;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEditPolyName;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEditUVName;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEditVcolorName;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEditPolynormalName;
		private System.Windows.Forms.TextBox textBoxObjectName;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboBoxNode;
	}
}