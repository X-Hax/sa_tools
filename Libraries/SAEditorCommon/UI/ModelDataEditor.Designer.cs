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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelDataEditor));
			buttonMoveMeshUp = new System.Windows.Forms.Button();
			buttonMoveMeshDown = new System.Windows.Forms.Button();
			listViewMeshes = new System.Windows.Forms.ListView();
			columnHeaderIndex = new System.Windows.Forms.ColumnHeader();
			columnHeaderMatID = new System.Windows.Forms.ColumnHeader();
			columnHeaderType = new System.Windows.Forms.ColumnHeader();
			columnHeaderPoly = new System.Windows.Forms.ColumnHeader();
			columnHeaderUV = new System.Windows.Forms.ColumnHeader();
			columnHeaderVcolor = new System.Windows.Forms.ColumnHeader();
			columnHeaderPolynormals = new System.Windows.Forms.ColumnHeader();
			columnHeaderTrans = new System.Windows.Forms.ColumnHeader();
			buttonCloneMesh = new System.Windows.Forms.Button();
			buttonDeleteMesh = new System.Windows.Forms.Button();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
			labelModelName = new System.Windows.Forms.Label();
			labelMeshsetName = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			textBoxMaterialName = new System.Windows.Forms.TextBox();
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
			textBoxNormalName = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			textBoxVertexName = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			groupBoxBounds = new System.Windows.Forms.GroupBox();
			textBoxModelRadius = new System.Windows.Forms.TextBox();
			labelR = new System.Windows.Forms.Label();
			groupBoxMeshList = new System.Windows.Forms.GroupBox();
			buttonMaterialEditor = new System.Windows.Forms.Button();
			buttonResetMeshes = new System.Windows.Forms.Button();
			groupBoxMaterialList = new System.Windows.Forms.GroupBox();
			listViewMaterials = new System.Windows.Forms.ListView();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			columnHeader2 = new System.Windows.Forms.ColumnHeader();
			columnHeader3 = new System.Windows.Forms.ColumnHeader();
			columnHeader4 = new System.Windows.Forms.ColumnHeader();
			columnHeader5 = new System.Windows.Forms.ColumnHeader();
			columnHeader6 = new System.Windows.Forms.ColumnHeader();
			buttonClose = new System.Windows.Forms.Button();
			contextMenuStripLabels = new System.Windows.Forms.ContextMenuStrip(components);
			toolStripMenuItemEditMaterialID = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripMenuItemEditPolyName = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItemEditUVName = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItemEditVcolorName = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItemEditPolynormalName = new System.Windows.Forms.ToolStripMenuItem();
			comboBoxNode = new System.Windows.Forms.ComboBox();
			groupBox1 = new System.Windows.Forms.GroupBox();
			listViewObjectData = new System.Windows.Forms.ListView();
			columnHeaderEval = new System.Windows.Forms.ColumnHeader();
			columnHeaderPos = new System.Windows.Forms.ColumnHeader();
			columnHeaderRot = new System.Windows.Forms.ColumnHeader();
			columnHeaderScl = new System.Windows.Forms.ColumnHeader();
			contextMenuStripObjSet = new System.Windows.Forms.ContextMenuStrip(components);
			editObjectSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			groupBoxVertexList = new System.Windows.Forms.GroupBox();
			labelNormalCount = new System.Windows.Forms.Label();
			labelVertexCount = new System.Windows.Forms.Label();
			buttonViewVertexData = new System.Windows.Forms.Button();
			label9 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			statusStrip1.SuspendLayout();
			groupBoxLabels.SuspendLayout();
			groupBoxBounds.SuspendLayout();
			groupBoxMeshList.SuspendLayout();
			groupBoxMaterialList.SuspendLayout();
			contextMenuStripLabels.SuspendLayout();
			groupBox1.SuspendLayout();
			contextMenuStripObjSet.SuspendLayout();
			groupBoxVertexList.SuspendLayout();
			SuspendLayout();
			// 
			// buttonMoveMeshUp
			// 
			buttonMoveMeshUp.Enabled = false;
			buttonMoveMeshUp.Location = new System.Drawing.Point(714, 21);
			buttonMoveMeshUp.Name = "buttonMoveMeshUp";
			buttonMoveMeshUp.Size = new System.Drawing.Size(24, 24);
			buttonMoveMeshUp.TabIndex = 19;
			buttonMoveMeshUp.Text = "↑";
			buttonMoveMeshUp.UseVisualStyleBackColor = true;
			buttonMoveMeshUp.Click += buttonMoveMeshUp_Click;
			// 
			// buttonMoveMeshDown
			// 
			buttonMoveMeshDown.Enabled = false;
			buttonMoveMeshDown.Location = new System.Drawing.Point(714, 51);
			buttonMoveMeshDown.Name = "buttonMoveMeshDown";
			buttonMoveMeshDown.Size = new System.Drawing.Size(24, 24);
			buttonMoveMeshDown.TabIndex = 20;
			buttonMoveMeshDown.Text = "↓";
			buttonMoveMeshDown.UseVisualStyleBackColor = true;
			buttonMoveMeshDown.Click += buttonMoveMeshDown_Click;
			// 
			// listViewMeshes
			// 
			listViewMeshes.AutoArrange = false;
			listViewMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderIndex, columnHeaderMatID, columnHeaderType, columnHeaderPoly, columnHeaderUV, columnHeaderVcolor, columnHeaderPolynormals, columnHeaderTrans });
			listViewMeshes.FullRowSelect = true;
			listViewMeshes.GridLines = true;
			listViewMeshes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewMeshes.Location = new System.Drawing.Point(5, 19);
			listViewMeshes.MultiSelect = false;
			listViewMeshes.Name = "listViewMeshes";
			listViewMeshes.ShowGroups = false;
			listViewMeshes.Size = new System.Drawing.Size(703, 236);
			listViewMeshes.TabIndex = 18;
			listViewMeshes.UseCompatibleStateImageBehavior = false;
			listViewMeshes.View = System.Windows.Forms.View.Details;
			listViewMeshes.SelectedIndexChanged += listViewMeshes_SelectedIndexChanged;
			listViewMeshes.MouseClick += listViewMeshes_MouseClick;
			// 
			// columnHeaderIndex
			// 
			columnHeaderIndex.Text = "Index";
			// 
			// columnHeaderMatID
			// 
			columnHeaderMatID.Text = "Material";
			// 
			// columnHeaderType
			// 
			columnHeaderType.Text = "Type";
			// 
			// columnHeaderPoly
			// 
			columnHeaderPoly.Text = "Polys";
			// 
			// columnHeaderUV
			// 
			columnHeaderUV.Text = "UVs";
			// 
			// columnHeaderVcolor
			// 
			columnHeaderVcolor.Text = "Vertex Colors";
			// 
			// columnHeaderPolynormals
			// 
			columnHeaderPolynormals.Text = "Polynormals";
			// 
			// columnHeaderTrans
			// 
			columnHeaderTrans.Text = "Uses Alpha?";
			// 
			// buttonCloneMesh
			// 
			buttonCloneMesh.Enabled = false;
			buttonCloneMesh.Location = new System.Drawing.Point(5, 261);
			buttonCloneMesh.Name = "buttonCloneMesh";
			buttonCloneMesh.Size = new System.Drawing.Size(90, 24);
			buttonCloneMesh.TabIndex = 21;
			buttonCloneMesh.Text = "Clone Mesh";
			buttonCloneMesh.UseVisualStyleBackColor = true;
			buttonCloneMesh.Click += buttonCloneMesh_Click;
			// 
			// buttonDeleteMesh
			// 
			buttonDeleteMesh.Enabled = false;
			buttonDeleteMesh.Location = new System.Drawing.Point(100, 261);
			buttonDeleteMesh.Name = "buttonDeleteMesh";
			buttonDeleteMesh.Size = new System.Drawing.Size(90, 24);
			buttonDeleteMesh.TabIndex = 22;
			buttonDeleteMesh.Text = "Delete Mesh";
			buttonDeleteMesh.UseVisualStyleBackColor = true;
			buttonDeleteMesh.Click += buttonDeleteMesh_Click;
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabelInfo });
			statusStrip1.Location = new System.Drawing.Point(0, 667);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
			statusStrip1.Size = new System.Drawing.Size(916, 22);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 11;
			statusStrip1.Text = "statusStrip1";
			statusStrip1.ItemClicked += statusStrip1_ItemClicked;
			// 
			// toolStripStatusLabelInfo
			// 
			toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
			toolStripStatusLabelInfo.Size = new System.Drawing.Size(343, 17);
			toolStripStatusLabelInfo.Text = "Click a mesh to display its information. Right click to edit labels.";
			// 
			// labelModelName
			// 
			labelModelName.AutoSize = true;
			labelModelName.Location = new System.Drawing.Point(16, 48);
			labelModelName.Name = "labelModelName";
			labelModelName.Size = new System.Drawing.Size(79, 15);
			labelModelName.TabIndex = 12;
			labelModelName.Text = "Model Name:";
			// 
			// labelMeshsetName
			// 
			labelMeshsetName.AutoSize = true;
			labelMeshsetName.Location = new System.Drawing.Point(266, 21);
			labelMeshsetName.Name = "labelMeshsetName";
			labelMeshsetName.Size = new System.Drawing.Size(89, 15);
			labelMeshsetName.TabIndex = 13;
			labelMeshsetName.Text = "Meshset Name:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(7, 75);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(88, 15);
			label1.TabIndex = 15;
			label1.Text = "Material Name:";
			// 
			// textBoxMaterialName
			// 
			textBoxMaterialName.Location = new System.Drawing.Point(100, 72);
			textBoxMaterialName.Margin = new System.Windows.Forms.Padding(2);
			textBoxMaterialName.Name = "textBoxMaterialName";
			textBoxMaterialName.Size = new System.Drawing.Size(145, 23);
			textBoxMaterialName.TabIndex = 4;
			textBoxMaterialName.TextChanged += textBoxMaterialName_TextChanged;
			// 
			// textBoxMeshsetName
			// 
			textBoxMeshsetName.Location = new System.Drawing.Point(360, 18);
			textBoxMeshsetName.Margin = new System.Windows.Forms.Padding(2);
			textBoxMeshsetName.Name = "textBoxMeshsetName";
			textBoxMeshsetName.Size = new System.Drawing.Size(145, 23);
			textBoxMeshsetName.TabIndex = 5;
			textBoxMeshsetName.TextChanged += textBoxMeshsetName_TextChanged;
			// 
			// textBoxModelName
			// 
			textBoxModelName.Location = new System.Drawing.Point(100, 45);
			textBoxModelName.Margin = new System.Windows.Forms.Padding(2);
			textBoxModelName.Name = "textBoxModelName";
			textBoxModelName.Size = new System.Drawing.Size(145, 23);
			textBoxModelName.TabIndex = 3;
			textBoxModelName.TextChanged += textBoxModelName_TextChanged;
			// 
			// textBoxModelY
			// 
			textBoxModelY.Location = new System.Drawing.Point(36, 45);
			textBoxModelY.Margin = new System.Windows.Forms.Padding(2);
			textBoxModelY.Name = "textBoxModelY";
			textBoxModelY.Size = new System.Drawing.Size(80, 23);
			textBoxModelY.TabIndex = 10;
			textBoxModelY.TextChanged += textBoxModelY_TextChanged;
			// 
			// textBoxModelZ
			// 
			textBoxModelZ.Location = new System.Drawing.Point(36, 72);
			textBoxModelZ.Margin = new System.Windows.Forms.Padding(2);
			textBoxModelZ.Name = "textBoxModelZ";
			textBoxModelZ.Size = new System.Drawing.Size(80, 23);
			textBoxModelZ.TabIndex = 11;
			textBoxModelZ.TextChanged += textBoxModelZ_TextChanged;
			// 
			// textBoxModelX
			// 
			textBoxModelX.Location = new System.Drawing.Point(36, 18);
			textBoxModelX.Margin = new System.Windows.Forms.Padding(2);
			textBoxModelX.Name = "textBoxModelX";
			textBoxModelX.Size = new System.Drawing.Size(80, 23);
			textBoxModelX.TabIndex = 9;
			textBoxModelX.TextChanged += textBoxModelX_TextChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(14, 21);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(17, 15);
			label4.TabIndex = 23;
			label4.Text = "X:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(14, 48);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(17, 15);
			label5.TabIndex = 24;
			label5.Text = "Y:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(14, 75);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(17, 15);
			label6.TabIndex = 25;
			label6.Text = "Z:";
			// 
			// groupBoxLabels
			// 
			groupBoxLabels.Controls.Add(textBoxObjectName);
			groupBoxLabels.Controls.Add(label7);
			groupBoxLabels.Controls.Add(textBoxNormalName);
			groupBoxLabels.Controls.Add(label3);
			groupBoxLabels.Controls.Add(textBoxVertexName);
			groupBoxLabels.Controls.Add(label2);
			groupBoxLabels.Controls.Add(textBoxModelName);
			groupBoxLabels.Controls.Add(labelModelName);
			groupBoxLabels.Controls.Add(labelMeshsetName);
			groupBoxLabels.Controls.Add(label1);
			groupBoxLabels.Controls.Add(textBoxMaterialName);
			groupBoxLabels.Controls.Add(textBoxMeshsetName);
			groupBoxLabels.Location = new System.Drawing.Point(8, 40);
			groupBoxLabels.Margin = new System.Windows.Forms.Padding(2);
			groupBoxLabels.Name = "groupBoxLabels";
			groupBoxLabels.Padding = new System.Windows.Forms.Padding(2);
			groupBoxLabels.Size = new System.Drawing.Size(509, 104);
			groupBoxLabels.TabIndex = 1;
			groupBoxLabels.TabStop = false;
			groupBoxLabels.Text = "Labels";
			// 
			// textBoxObjectName
			// 
			textBoxObjectName.Location = new System.Drawing.Point(100, 18);
			textBoxObjectName.Margin = new System.Windows.Forms.Padding(2);
			textBoxObjectName.Name = "textBoxObjectName";
			textBoxObjectName.Size = new System.Drawing.Size(145, 23);
			textBoxObjectName.TabIndex = 2;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(15, 21);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(80, 15);
			label7.TabIndex = 23;
			label7.Text = "Object Name:";
			// 
			// textBoxNormalName
			// 
			textBoxNormalName.Location = new System.Drawing.Point(360, 72);
			textBoxNormalName.Margin = new System.Windows.Forms.Padding(2);
			textBoxNormalName.Name = "textBoxNormalName";
			textBoxNormalName.Size = new System.Drawing.Size(145, 23);
			textBoxNormalName.TabIndex = 7;
			textBoxNormalName.TextChanged += textBoxNormalName_TextChanged;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(265, 75);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(90, 15);
			label3.TabIndex = 21;
			label3.Text = "Normals Name:";
			// 
			// textBoxVertexName
			// 
			textBoxVertexName.Location = new System.Drawing.Point(360, 45);
			textBoxVertexName.Margin = new System.Windows.Forms.Padding(2);
			textBoxVertexName.Name = "textBoxVertexName";
			textBoxVertexName.Size = new System.Drawing.Size(145, 23);
			textBoxVertexName.TabIndex = 6;
			textBoxVertexName.TextChanged += textBoxVertexName_TextChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(270, 48);
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
			groupBoxBounds.Location = new System.Drawing.Point(522, 40);
			groupBoxBounds.Margin = new System.Windows.Forms.Padding(2);
			groupBoxBounds.Name = "groupBoxBounds";
			groupBoxBounds.Padding = new System.Windows.Forms.Padding(2);
			groupBoxBounds.Size = new System.Drawing.Size(232, 104);
			groupBoxBounds.TabIndex = 8;
			groupBoxBounds.TabStop = false;
			groupBoxBounds.Text = "Model Bounds";
			// 
			// textBoxModelRadius
			// 
			textBoxModelRadius.Location = new System.Drawing.Point(128, 45);
			textBoxModelRadius.Margin = new System.Windows.Forms.Padding(2);
			textBoxModelRadius.Name = "textBoxModelRadius";
			textBoxModelRadius.Size = new System.Drawing.Size(93, 23);
			textBoxModelRadius.TabIndex = 12;
			textBoxModelRadius.TextChanged += textBoxModelRadius_TextChanged;
			// 
			// labelR
			// 
			labelR.AutoSize = true;
			labelR.Location = new System.Drawing.Point(128, 21);
			labelR.Name = "labelR";
			labelR.Size = new System.Drawing.Size(45, 15);
			labelR.TabIndex = 31;
			labelR.Text = "Radius:";
			// 
			// groupBoxMeshList
			// 
			groupBoxMeshList.Controls.Add(buttonMaterialEditor);
			groupBoxMeshList.Controls.Add(buttonResetMeshes);
			groupBoxMeshList.Controls.Add(groupBoxMaterialList);
			groupBoxMeshList.Controls.Add(buttonMoveMeshUp);
			groupBoxMeshList.Controls.Add(buttonMoveMeshDown);
			groupBoxMeshList.Controls.Add(listViewMeshes);
			groupBoxMeshList.Controls.Add(buttonDeleteMesh);
			groupBoxMeshList.Controls.Add(buttonCloneMesh);
			groupBoxMeshList.Location = new System.Drawing.Point(11, 244);
			groupBoxMeshList.Margin = new System.Windows.Forms.Padding(2);
			groupBoxMeshList.Name = "groupBoxMeshList";
			groupBoxMeshList.Padding = new System.Windows.Forms.Padding(2);
			groupBoxMeshList.Size = new System.Drawing.Size(745, 417);
			groupBoxMeshList.TabIndex = 17;
			groupBoxMeshList.TabStop = false;
			groupBoxMeshList.Text = "Mesh List";
			// 
			// buttonMaterialEditor
			// 
			buttonMaterialEditor.Location = new System.Drawing.Point(198, 261);
			buttonMaterialEditor.Name = "buttonMaterialEditor";
			buttonMaterialEditor.Size = new System.Drawing.Size(154, 24);
			buttonMaterialEditor.TabIndex = 23;
			buttonMaterialEditor.Text = "Open Material Editor";
			buttonMaterialEditor.UseVisualStyleBackColor = true;
			buttonMaterialEditor.Click += buttonMaterialEditor_Click;
			// 
			// buttonResetMeshes
			// 
			buttonResetMeshes.Location = new System.Drawing.Point(618, 261);
			buttonResetMeshes.Name = "buttonResetMeshes";
			buttonResetMeshes.Size = new System.Drawing.Size(90, 24);
			buttonResetMeshes.TabIndex = 24;
			buttonResetMeshes.Text = "Reset Meshes";
			buttonResetMeshes.UseVisualStyleBackColor = true;
			buttonResetMeshes.Click += buttonResetMeshes_Click;
			// 
			// groupBoxMaterialList
			// 
			groupBoxMaterialList.Controls.Add(listViewMaterials);
			groupBoxMaterialList.Location = new System.Drawing.Point(5, 288);
			groupBoxMaterialList.Name = "groupBoxMaterialList";
			groupBoxMaterialList.Size = new System.Drawing.Size(714, 125);
			groupBoxMaterialList.TabIndex = 25;
			groupBoxMaterialList.TabStop = false;
			groupBoxMaterialList.Text = "Material List";
			// 
			// listViewMaterials
			// 
			listViewMaterials.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5, columnHeader6 });
			listViewMaterials.FullRowSelect = true;
			listViewMaterials.GridLines = true;
			listViewMaterials.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewMaterials.Location = new System.Drawing.Point(5, 22);
			listViewMaterials.MultiSelect = false;
			listViewMaterials.Name = "listViewMaterials";
			listViewMaterials.Size = new System.Drawing.Size(698, 96);
			listViewMaterials.TabIndex = 26;
			listViewMaterials.UseCompatibleStateImageBehavior = false;
			listViewMaterials.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			columnHeader1.Text = "ID";
			// 
			// columnHeader2
			// 
			columnHeader2.Text = "Diffuse";
			// 
			// columnHeader3
			// 
			columnHeader3.Text = "Specular";
			// 
			// columnHeader4
			// 
			columnHeader4.Text = "Strip Flags";
			// 
			// columnHeader5
			// 
			columnHeader5.Text = "Texture Flags";
			// 
			// columnHeader6
			// 
			columnHeader6.Text = "Blend Modes";
			// 
			// buttonClose
			// 
			buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonClose.Location = new System.Drawing.Point(826, 638);
			buttonClose.Margin = new System.Windows.Forms.Padding(2);
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new System.Drawing.Size(84, 24);
			buttonClose.TabIndex = 27;
			buttonClose.Text = "Close";
			buttonClose.UseVisualStyleBackColor = true;
			buttonClose.Click += buttonClose_Click;
			// 
			// contextMenuStripLabels
			// 
			contextMenuStripLabels.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripLabels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItemEditMaterialID, toolStripSeparator1, toolStripMenuItemEditPolyName, toolStripMenuItemEditUVName, toolStripMenuItemEditVcolorName, toolStripMenuItemEditPolynormalName });
			contextMenuStripLabels.Name = "contextMenuStripLabels";
			contextMenuStripLabels.Size = new System.Drawing.Size(203, 120);
			// 
			// toolStripMenuItemEditMaterialID
			// 
			toolStripMenuItemEditMaterialID.Name = "toolStripMenuItemEditMaterialID";
			toolStripMenuItemEditMaterialID.Size = new System.Drawing.Size(202, 22);
			toolStripMenuItemEditMaterialID.Text = "Edit Material ID...";
			toolStripMenuItemEditMaterialID.Click += toolStripMenuItemEditMaterialID_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(199, 6);
			// 
			// toolStripMenuItemEditPolyName
			// 
			toolStripMenuItemEditPolyName.Name = "toolStripMenuItemEditPolyName";
			toolStripMenuItemEditPolyName.Size = new System.Drawing.Size(202, 22);
			toolStripMenuItemEditPolyName.Text = "Edit Polys Name...";
			toolStripMenuItemEditPolyName.Click += toolStripMenuItemEditPolyName_Click;
			// 
			// toolStripMenuItemEditUVName
			// 
			toolStripMenuItemEditUVName.Name = "toolStripMenuItemEditUVName";
			toolStripMenuItemEditUVName.Size = new System.Drawing.Size(202, 22);
			toolStripMenuItemEditUVName.Text = "Edit UVs Name...";
			toolStripMenuItemEditUVName.Click += toolStripMenuItemEditUVName_Click;
			// 
			// toolStripMenuItemEditVcolorName
			// 
			toolStripMenuItemEditVcolorName.Name = "toolStripMenuItemEditVcolorName";
			toolStripMenuItemEditVcolorName.Size = new System.Drawing.Size(202, 22);
			toolStripMenuItemEditVcolorName.Text = "Edit VColor Name...";
			toolStripMenuItemEditVcolorName.Click += toolStripMenuItemEditVcolorName_Click;
			// 
			// toolStripMenuItemEditPolynormalName
			// 
			toolStripMenuItemEditPolynormalName.Name = "toolStripMenuItemEditPolynormalName";
			toolStripMenuItemEditPolynormalName.Size = new System.Drawing.Size(202, 22);
			toolStripMenuItemEditPolynormalName.Text = "Edit Polynormal Name...";
			toolStripMenuItemEditPolynormalName.Click += toolStripMenuItemEditPolynormalName_Click;
			// 
			// comboBoxNode
			// 
			comboBoxNode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxNode.FormattingEnabled = true;
			comboBoxNode.Location = new System.Drawing.Point(8, 12);
			comboBoxNode.Name = "comboBoxNode";
			comboBoxNode.Size = new System.Drawing.Size(355, 23);
			comboBoxNode.TabIndex = 0;
			comboBoxNode.SelectedIndexChanged += comboBoxNode_SelectedIndexChanged;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(listViewObjectData);
			groupBox1.Location = new System.Drawing.Point(9, 149);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(892, 90);
			groupBox1.TabIndex = 15;
			groupBox1.TabStop = false;
			groupBox1.Text = "Object Data";
			// 
			// listViewObjectData
			// 
			listViewObjectData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderEval, columnHeaderPos, columnHeaderRot, columnHeaderScl });
			listViewObjectData.FullRowSelect = true;
			listViewObjectData.GridLines = true;
			listViewObjectData.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewObjectData.Location = new System.Drawing.Point(5, 18);
			listViewObjectData.MultiSelect = false;
			listViewObjectData.Name = "listViewObjectData";
			listViewObjectData.ShowGroups = false;
			listViewObjectData.Size = new System.Drawing.Size(871, 60);
			listViewObjectData.TabIndex = 16;
			listViewObjectData.UseCompatibleStateImageBehavior = false;
			listViewObjectData.View = System.Windows.Forms.View.Details;
			listViewObjectData.DoubleClick += ObjectData_DoubleClick;
			listViewObjectData.KeyPress += ObjectData_EnterKey;
			listViewObjectData.MouseClick += listViewObjectData_MouseClick;
			// 
			// columnHeaderEval
			// 
			columnHeaderEval.Text = "Eval Flags                                                                       ";
			columnHeaderEval.Width = 300;
			// 
			// columnHeaderPos
			// 
			columnHeaderPos.Text = "Position                                   ";
			columnHeaderPos.Width = 200;
			// 
			// columnHeaderRot
			// 
			columnHeaderRot.Text = "Rotation                                   ";
			columnHeaderRot.Width = 200;
			// 
			// columnHeaderScl
			// 
			columnHeaderScl.Text = "Scale";
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
			// groupBoxVertexList
			// 
			groupBoxVertexList.Controls.Add(labelNormalCount);
			groupBoxVertexList.Controls.Add(labelVertexCount);
			groupBoxVertexList.Controls.Add(buttonViewVertexData);
			groupBoxVertexList.Controls.Add(label9);
			groupBoxVertexList.Controls.Add(label8);
			groupBoxVertexList.Location = new System.Drawing.Point(759, 41);
			groupBoxVertexList.Name = "groupBoxVertexList";
			groupBoxVertexList.Size = new System.Drawing.Size(152, 104);
			groupBoxVertexList.TabIndex = 13;
			groupBoxVertexList.TabStop = false;
			groupBoxVertexList.Text = "Vertex List";
			// 
			// labelNormalCount
			// 
			labelNormalCount.AutoSize = true;
			labelNormalCount.Location = new System.Drawing.Point(66, 45);
			labelNormalCount.Name = "labelNormalCount";
			labelNormalCount.Size = new System.Drawing.Size(13, 15);
			labelNormalCount.TabIndex = 4;
			labelNormalCount.Text = "0";
			// 
			// labelVertexCount
			// 
			labelVertexCount.AutoSize = true;
			labelVertexCount.Location = new System.Drawing.Point(62, 21);
			labelVertexCount.Name = "labelVertexCount";
			labelVertexCount.Size = new System.Drawing.Size(13, 15);
			labelVertexCount.TabIndex = 3;
			labelVertexCount.Text = "0";
			// 
			// buttonViewVertexData
			// 
			buttonViewVertexData.Location = new System.Drawing.Point(7, 71);
			buttonViewVertexData.Name = "buttonViewVertexData";
			buttonViewVertexData.Size = new System.Drawing.Size(135, 23);
			buttonViewVertexData.TabIndex = 14;
			buttonViewVertexData.Text = "View Point Data";
			buttonViewVertexData.UseVisualStyleBackColor = true;
			buttonViewVertexData.Click += buttonViewVertexData_Click;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(7, 45);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(55, 15);
			label9.TabIndex = 1;
			label9.Text = "Normals:";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(7, 21);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(53, 15);
			label8.TabIndex = 0;
			label8.Text = "Vertices: ";
			// 
			// ModelDataEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			ClientSize = new System.Drawing.Size(916, 689);
			Controls.Add(groupBoxVertexList);
			Controls.Add(groupBox1);
			Controls.Add(comboBoxNode);
			Controls.Add(buttonClose);
			Controls.Add(groupBoxMeshList);
			Controls.Add(groupBoxBounds);
			Controls.Add(groupBoxLabels);
			Controls.Add(statusStrip1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ModelDataEditor";
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
			groupBoxMaterialList.ResumeLayout(false);
			contextMenuStripLabels.ResumeLayout(false);
			groupBox1.ResumeLayout(false);
			contextMenuStripObjSet.ResumeLayout(false);
			groupBoxVertexList.ResumeLayout(false);
			groupBoxVertexList.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

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
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEditMaterialID;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListView listViewObjectData;
		private System.Windows.Forms.ColumnHeader columnHeaderEval;
		private System.Windows.Forms.ColumnHeader columnHeaderPos;
		private System.Windows.Forms.ColumnHeader columnHeaderRot;
		private System.Windows.Forms.ColumnHeader columnHeaderScl;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripObjSet;
		private System.Windows.Forms.ToolStripMenuItem editObjectSettingsToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBoxVertexList;
		private System.Windows.Forms.Label labelVertexCount;
		private System.Windows.Forms.Button buttonViewVertexData;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label labelNormalCount;
		private System.Windows.Forms.GroupBox groupBoxMaterialList;
		private System.Windows.Forms.ListView listViewMaterials;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonMaterialEditor;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
	}
}