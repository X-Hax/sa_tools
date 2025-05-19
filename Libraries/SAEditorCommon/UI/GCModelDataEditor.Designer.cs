namespace SAModel.SAEditorCommon.UI
{
	partial class GCModelDataEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCModelDataEditor));
			buttonMoveTMeshUp = new System.Windows.Forms.Button();
			buttonMoveTMeshDown = new System.Windows.Forms.Button();
			listViewOMeshes = new System.Windows.Forms.ListView();
			columnHeaderIndex = new System.Windows.Forms.ColumnHeader();
			columnHeaderEntries = new System.Windows.Forms.ColumnHeader();
			columnHeaderPrim = new System.Windows.Forms.ColumnHeader();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
			labelModelName = new System.Windows.Forms.Label();
			labelMeshsetName = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			textBoxVertexName = new System.Windows.Forms.TextBox();
			textBoxOPolyName = new System.Windows.Forms.TextBox();
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
			textBoxTPolyName = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			groupBoxBounds = new System.Windows.Forms.GroupBox();
			textBoxModelRadius = new System.Windows.Forms.TextBox();
			labelR = new System.Windows.Forms.Label();
			groupBoxMeshList = new System.Windows.Forms.GroupBox();
			groupBoxTrans = new System.Windows.Forms.GroupBox();
			listViewTMeshes = new System.Windows.Forms.ListView();
			columnHeader5 = new System.Windows.Forms.ColumnHeader();
			columnHeader6 = new System.Windows.Forms.ColumnHeader();
			columnHeader9 = new System.Windows.Forms.ColumnHeader();
			buttonResetTMeshes = new System.Windows.Forms.Button();
			buttonDeleteTMesh = new System.Windows.Forms.Button();
			buttonCloneTMesh = new System.Windows.Forms.Button();
			groupBoxOpaque = new System.Windows.Forms.GroupBox();
			buttonMoveOMeshUp = new System.Windows.Forms.Button();
			buttonMoveOMeshDown = new System.Windows.Forms.Button();
			buttonResetOMeshes = new System.Windows.Forms.Button();
			buttonDeleteOMesh = new System.Windows.Forms.Button();
			buttonCloneOMesh = new System.Windows.Forms.Button();
			buttonClose = new System.Windows.Forms.Button();
			contextMenuStripParamEdit = new System.Windows.Forms.ContextMenuStrip(components);
			openOParameterViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openTParameterViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			comboBoxNode = new System.Windows.Forms.ComboBox();
			groupBoxVertexList = new System.Windows.Forms.GroupBox();
			listViewVertices = new System.Windows.Forms.ListView();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			columnHeader2 = new System.Windows.Forms.ColumnHeader();
			columnHeader3 = new System.Windows.Forms.ColumnHeader();
			columnHeader4 = new System.Windows.Forms.ColumnHeader();
			contextMenuStripVertData = new System.Windows.Forms.ContextMenuStrip(components);
			viewVertexDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			statusStrip1.SuspendLayout();
			groupBoxLabels.SuspendLayout();
			groupBoxBounds.SuspendLayout();
			groupBoxMeshList.SuspendLayout();
			groupBoxTrans.SuspendLayout();
			groupBoxOpaque.SuspendLayout();
			contextMenuStripParamEdit.SuspendLayout();
			groupBoxVertexList.SuspendLayout();
			contextMenuStripVertData.SuspendLayout();
			SuspendLayout();
			// 
			// buttonMoveTMeshUp
			// 
			buttonMoveTMeshUp.Enabled = false;
			buttonMoveTMeshUp.Location = new System.Drawing.Point(317, 28);
			buttonMoveTMeshUp.Margin = new System.Windows.Forms.Padding(4);
			buttonMoveTMeshUp.Name = "buttonMoveTMeshUp";
			buttonMoveTMeshUp.Size = new System.Drawing.Size(36, 36);
			buttonMoveTMeshUp.TabIndex = 12;
			buttonMoveTMeshUp.Text = "↑";
			buttonMoveTMeshUp.UseVisualStyleBackColor = true;
			buttonMoveTMeshUp.Click += buttonMoveTMeshUp_Click;
			// 
			// buttonMoveTMeshDown
			// 
			buttonMoveTMeshDown.Enabled = false;
			buttonMoveTMeshDown.Location = new System.Drawing.Point(317, 72);
			buttonMoveTMeshDown.Margin = new System.Windows.Forms.Padding(4);
			buttonMoveTMeshDown.Name = "buttonMoveTMeshDown";
			buttonMoveTMeshDown.Size = new System.Drawing.Size(36, 36);
			buttonMoveTMeshDown.TabIndex = 13;
			buttonMoveTMeshDown.Text = "↓";
			buttonMoveTMeshDown.UseVisualStyleBackColor = true;
			buttonMoveTMeshDown.Click += buttonMoveTMeshDown_Click;
			// 
			// listViewOMeshes
			// 
			listViewOMeshes.AutoArrange = false;
			listViewOMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderIndex, columnHeaderEntries, columnHeaderPrim });
			listViewOMeshes.FullRowSelect = true;
			listViewOMeshes.GridLines = true;
			listViewOMeshes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewOMeshes.Location = new System.Drawing.Point(7, 28);
			listViewOMeshes.Margin = new System.Windows.Forms.Padding(4);
			listViewOMeshes.MultiSelect = false;
			listViewOMeshes.Name = "listViewOMeshes";
			listViewOMeshes.ShowGroups = false;
			listViewOMeshes.Size = new System.Drawing.Size(303, 324);
			listViewOMeshes.TabIndex = 11;
			listViewOMeshes.UseCompatibleStateImageBehavior = false;
			listViewOMeshes.View = System.Windows.Forms.View.Details;
			listViewOMeshes.SelectedIndexChanged += listViewOMeshes_SelectedIndexChanged;
			listViewOMeshes.DoubleClick += MshData_DoubleClick;
			listViewOMeshes.MouseClick += listViewOMeshes_MouseClick;
			// 
			// columnHeaderIndex
			// 
			columnHeaderIndex.Text = "Index";
			// 
			// columnHeaderEntries
			// 
			columnHeaderEntries.Text = "Param Entries";
			// 
			// columnHeaderPrim
			// 
			columnHeaderPrim.Text = "Prim Count";
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabelInfo });
			statusStrip1.Location = new System.Drawing.Point(0, 742);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 15, 0);
			statusStrip1.Size = new System.Drawing.Size(1137, 22);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 11;
			statusStrip1.Text = "statusStrip1";
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
			labelModelName.Location = new System.Drawing.Point(12, 72);
			labelModelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelModelName.Name = "labelModelName";
			labelModelName.Size = new System.Drawing.Size(79, 15);
			labelModelName.TabIndex = 12;
			labelModelName.Text = "Model Name:";
			// 
			// labelMeshsetName
			// 
			labelMeshsetName.AutoSize = true;
			labelMeshsetName.Location = new System.Drawing.Point(363, 72);
			labelMeshsetName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelMeshsetName.Name = "labelMeshsetName";
			labelMeshsetName.Size = new System.Drawing.Size(113, 15);
			labelMeshsetName.TabIndex = 13;
			labelMeshsetName.Text = "Opaque Poly Name:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(405, 32);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(85, 15);
			label1.TabIndex = 15;
			label1.Text = "Vertices Name:";
			// 
			// textBoxVertexName
			// 
			textBoxVertexName.Location = new System.Drawing.Point(540, 27);
			textBoxVertexName.Name = "textBoxVertexName";
			textBoxVertexName.Size = new System.Drawing.Size(216, 23);
			textBoxVertexName.TabIndex = 3;
			textBoxVertexName.TextChanged += textBoxVertexName_TextChanged;
			// 
			// textBoxOPolyName
			// 
			textBoxOPolyName.Location = new System.Drawing.Point(540, 68);
			textBoxOPolyName.Name = "textBoxOPolyName";
			textBoxOPolyName.Size = new System.Drawing.Size(216, 23);
			textBoxOPolyName.TabIndex = 4;
			textBoxOPolyName.TextChanged += textBoxOPolyName_TextChanged;
			// 
			// textBoxModelName
			// 
			textBoxModelName.Location = new System.Drawing.Point(138, 68);
			textBoxModelName.Name = "textBoxModelName";
			textBoxModelName.Size = new System.Drawing.Size(216, 23);
			textBoxModelName.TabIndex = 2;
			textBoxModelName.TextChanged += textBoxModelName_TextChanged;
			// 
			// textBoxModelY
			// 
			textBoxModelY.Location = new System.Drawing.Point(54, 68);
			textBoxModelY.Name = "textBoxModelY";
			textBoxModelY.Size = new System.Drawing.Size(118, 23);
			textBoxModelY.TabIndex = 8;
			textBoxModelY.TextChanged += textBoxModelY_TextChanged;
			// 
			// textBoxModelZ
			// 
			textBoxModelZ.Location = new System.Drawing.Point(54, 108);
			textBoxModelZ.Name = "textBoxModelZ";
			textBoxModelZ.Size = new System.Drawing.Size(118, 23);
			textBoxModelZ.TabIndex = 9;
			textBoxModelZ.TextChanged += textBoxModelZ_TextChanged;
			// 
			// textBoxModelX
			// 
			textBoxModelX.Location = new System.Drawing.Point(54, 27);
			textBoxModelX.Name = "textBoxModelX";
			textBoxModelX.Size = new System.Drawing.Size(118, 23);
			textBoxModelX.TabIndex = 7;
			textBoxModelX.TextChanged += textBoxModelX_TextChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(21, 32);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(17, 15);
			label4.TabIndex = 23;
			label4.Text = "X:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(21, 72);
			label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(17, 15);
			label5.TabIndex = 24;
			label5.Text = "Y:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(21, 112);
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
			groupBoxLabels.Controls.Add(textBoxTPolyName);
			groupBoxLabels.Controls.Add(label2);
			groupBoxLabels.Controls.Add(textBoxModelName);
			groupBoxLabels.Controls.Add(labelModelName);
			groupBoxLabels.Controls.Add(labelMeshsetName);
			groupBoxLabels.Controls.Add(label1);
			groupBoxLabels.Controls.Add(textBoxVertexName);
			groupBoxLabels.Controls.Add(textBoxOPolyName);
			groupBoxLabels.Location = new System.Drawing.Point(12, 60);
			groupBoxLabels.Name = "groupBoxLabels";
			groupBoxLabels.Size = new System.Drawing.Size(764, 156);
			groupBoxLabels.TabIndex = 26;
			groupBoxLabels.TabStop = false;
			groupBoxLabels.Text = "Labels";
			// 
			// textBoxObjectName
			// 
			textBoxObjectName.Location = new System.Drawing.Point(138, 27);
			textBoxObjectName.Name = "textBoxObjectName";
			textBoxObjectName.Size = new System.Drawing.Size(216, 23);
			textBoxObjectName.TabIndex = 1;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(10, 32);
			label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(80, 15);
			label7.TabIndex = 23;
			label7.Text = "Object Name:";
			// 
			// textBoxTPolyName
			// 
			textBoxTPolyName.Location = new System.Drawing.Point(540, 108);
			textBoxTPolyName.Name = "textBoxTPolyName";
			textBoxTPolyName.Size = new System.Drawing.Size(216, 23);
			textBoxTPolyName.TabIndex = 5;
			textBoxTPolyName.TextChanged += textBoxTPolyName_TextChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(336, 112);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(131, 15);
			label2.TabIndex = 19;
			label2.Text = "Translucent Poly Name:";
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
			textBoxModelRadius.Size = new System.Drawing.Size(138, 23);
			textBoxModelRadius.TabIndex = 10;
			textBoxModelRadius.TextChanged += textBoxModelRadius_TextChanged;
			// 
			// labelR
			// 
			labelR.AutoSize = true;
			labelR.Location = new System.Drawing.Point(192, 32);
			labelR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelR.Name = "labelR";
			labelR.Size = new System.Drawing.Size(45, 15);
			labelR.TabIndex = 31;
			labelR.Text = "Radius:";
			// 
			// groupBoxMeshList
			// 
			groupBoxMeshList.Controls.Add(groupBoxTrans);
			groupBoxMeshList.Controls.Add(groupBoxOpaque);
			groupBoxMeshList.Location = new System.Drawing.Point(382, 240);
			groupBoxMeshList.Name = "groupBoxMeshList";
			groupBoxMeshList.Size = new System.Drawing.Size(748, 441);
			groupBoxMeshList.TabIndex = 28;
			groupBoxMeshList.TabStop = false;
			groupBoxMeshList.Text = "Mesh Data";
			// 
			// groupBoxTrans
			// 
			groupBoxTrans.Controls.Add(listViewTMeshes);
			groupBoxTrans.Controls.Add(buttonMoveTMeshUp);
			groupBoxTrans.Controls.Add(buttonResetTMeshes);
			groupBoxTrans.Controls.Add(buttonMoveTMeshDown);
			groupBoxTrans.Controls.Add(buttonDeleteTMesh);
			groupBoxTrans.Controls.Add(buttonCloneTMesh);
			groupBoxTrans.Location = new System.Drawing.Point(375, 25);
			groupBoxTrans.Name = "groupBoxTrans";
			groupBoxTrans.Size = new System.Drawing.Size(361, 406);
			groupBoxTrans.TabIndex = 19;
			groupBoxTrans.TabStop = false;
			groupBoxTrans.Text = "Translucent";
			// 
			// listViewTMeshes
			// 
			listViewTMeshes.AutoArrange = false;
			listViewTMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader5, columnHeader6, columnHeader9 });
			listViewTMeshes.FullRowSelect = true;
			listViewTMeshes.GridLines = true;
			listViewTMeshes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewTMeshes.Location = new System.Drawing.Point(7, 28);
			listViewTMeshes.Margin = new System.Windows.Forms.Padding(4);
			listViewTMeshes.MultiSelect = false;
			listViewTMeshes.Name = "listViewTMeshes";
			listViewTMeshes.ShowGroups = false;
			listViewTMeshes.Size = new System.Drawing.Size(303, 324);
			listViewTMeshes.TabIndex = 17;
			listViewTMeshes.UseCompatibleStateImageBehavior = false;
			listViewTMeshes.View = System.Windows.Forms.View.Details;
			listViewTMeshes.SelectedIndexChanged += listViewTMeshes_SelectedIndexChanged;
			listViewTMeshes.MouseClick += listViewTMeshes_MouseClick;
			// 
			// columnHeader5
			// 
			columnHeader5.Text = "Index";
			// 
			// columnHeader6
			// 
			columnHeader6.Text = "Param Entries";
			// 
			// columnHeader9
			// 
			columnHeader9.Text = "Prim Count";
			// 
			// buttonResetTMeshes
			// 
			buttonResetTMeshes.Location = new System.Drawing.Point(206, 360);
			buttonResetTMeshes.Margin = new System.Windows.Forms.Padding(4);
			buttonResetTMeshes.Name = "buttonResetTMeshes";
			buttonResetTMeshes.Size = new System.Drawing.Size(135, 36);
			buttonResetTMeshes.TabIndex = 16;
			buttonResetTMeshes.Text = "Reset Meshes";
			buttonResetTMeshes.UseVisualStyleBackColor = true;
			buttonResetTMeshes.Click += buttonResetTMeshes_Click;
			// 
			// buttonDeleteTMesh
			// 
			buttonDeleteTMesh.Enabled = false;
			buttonDeleteTMesh.Location = new System.Drawing.Point(104, 360);
			buttonDeleteTMesh.Margin = new System.Windows.Forms.Padding(4);
			buttonDeleteTMesh.Name = "buttonDeleteTMesh";
			buttonDeleteTMesh.Size = new System.Drawing.Size(94, 36);
			buttonDeleteTMesh.TabIndex = 15;
			buttonDeleteTMesh.Text = "Delete";
			buttonDeleteTMesh.UseVisualStyleBackColor = true;
			buttonDeleteTMesh.Click += buttonDeleteTMesh_Click;
			// 
			// buttonCloneTMesh
			// 
			buttonCloneTMesh.Enabled = false;
			buttonCloneTMesh.Location = new System.Drawing.Point(7, 360);
			buttonCloneTMesh.Margin = new System.Windows.Forms.Padding(4);
			buttonCloneTMesh.Name = "buttonCloneTMesh";
			buttonCloneTMesh.Size = new System.Drawing.Size(89, 36);
			buttonCloneTMesh.TabIndex = 14;
			buttonCloneTMesh.Text = "Clone";
			buttonCloneTMesh.UseVisualStyleBackColor = true;
			buttonCloneTMesh.Click += buttonCloneTMesh_Click;
			// 
			// groupBoxOpaque
			// 
			groupBoxOpaque.Controls.Add(buttonMoveOMeshUp);
			groupBoxOpaque.Controls.Add(buttonMoveOMeshDown);
			groupBoxOpaque.Controls.Add(buttonResetOMeshes);
			groupBoxOpaque.Controls.Add(buttonDeleteOMesh);
			groupBoxOpaque.Controls.Add(listViewOMeshes);
			groupBoxOpaque.Controls.Add(buttonCloneOMesh);
			groupBoxOpaque.Location = new System.Drawing.Point(8, 25);
			groupBoxOpaque.Name = "groupBoxOpaque";
			groupBoxOpaque.Size = new System.Drawing.Size(361, 406);
			groupBoxOpaque.TabIndex = 18;
			groupBoxOpaque.TabStop = false;
			groupBoxOpaque.Text = "Opaque";
			// 
			// buttonMoveOMeshUp
			// 
			buttonMoveOMeshUp.Enabled = false;
			buttonMoveOMeshUp.Location = new System.Drawing.Point(317, 28);
			buttonMoveOMeshUp.Margin = new System.Windows.Forms.Padding(4);
			buttonMoveOMeshUp.Name = "buttonMoveOMeshUp";
			buttonMoveOMeshUp.Size = new System.Drawing.Size(36, 36);
			buttonMoveOMeshUp.TabIndex = 14;
			buttonMoveOMeshUp.Text = "↑";
			buttonMoveOMeshUp.UseVisualStyleBackColor = true;
			buttonMoveOMeshUp.Click += buttonMoveOMeshUp_Click;
			// 
			// buttonMoveOMeshDown
			// 
			buttonMoveOMeshDown.Enabled = false;
			buttonMoveOMeshDown.Location = new System.Drawing.Point(317, 72);
			buttonMoveOMeshDown.Margin = new System.Windows.Forms.Padding(4);
			buttonMoveOMeshDown.Name = "buttonMoveOMeshDown";
			buttonMoveOMeshDown.Size = new System.Drawing.Size(36, 36);
			buttonMoveOMeshDown.TabIndex = 15;
			buttonMoveOMeshDown.Text = "↓";
			buttonMoveOMeshDown.UseVisualStyleBackColor = true;
			buttonMoveOMeshDown.Click += buttonMoveOMeshDown_Click;
			// 
			// buttonResetOMeshes
			// 
			buttonResetOMeshes.Location = new System.Drawing.Point(196, 360);
			buttonResetOMeshes.Margin = new System.Windows.Forms.Padding(4);
			buttonResetOMeshes.Name = "buttonResetOMeshes";
			buttonResetOMeshes.Size = new System.Drawing.Size(135, 36);
			buttonResetOMeshes.TabIndex = 16;
			buttonResetOMeshes.Text = "Reset Meshes";
			buttonResetOMeshes.UseVisualStyleBackColor = true;
			buttonResetOMeshes.Click += buttonResetOMeshes_Click;
			// 
			// buttonDeleteOMesh
			// 
			buttonDeleteOMesh.Enabled = false;
			buttonDeleteOMesh.Location = new System.Drawing.Point(99, 360);
			buttonDeleteOMesh.Margin = new System.Windows.Forms.Padding(4);
			buttonDeleteOMesh.Name = "buttonDeleteOMesh";
			buttonDeleteOMesh.Size = new System.Drawing.Size(89, 36);
			buttonDeleteOMesh.TabIndex = 15;
			buttonDeleteOMesh.Text = "Delete";
			buttonDeleteOMesh.UseVisualStyleBackColor = true;
			buttonDeleteOMesh.Click += buttonDeleteOMesh_Click;
			// 
			// buttonCloneOMesh
			// 
			buttonCloneOMesh.Enabled = false;
			buttonCloneOMesh.Location = new System.Drawing.Point(7, 360);
			buttonCloneOMesh.Margin = new System.Windows.Forms.Padding(4);
			buttonCloneOMesh.Name = "buttonCloneOMesh";
			buttonCloneOMesh.Size = new System.Drawing.Size(84, 36);
			buttonCloneOMesh.TabIndex = 14;
			buttonCloneOMesh.Text = "Clone";
			buttonCloneOMesh.UseVisualStyleBackColor = true;
			buttonCloneOMesh.Click += buttonCloneOMesh_Click;
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
			// contextMenuStripParamEdit
			// 
			contextMenuStripParamEdit.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripParamEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { openOParameterViewerToolStripMenuItem, openTParameterViewerToolStripMenuItem });
			contextMenuStripParamEdit.Name = "contextMenuStripLabels";
			contextMenuStripParamEdit.Size = new System.Drawing.Size(262, 48);
			// 
			// openOParameterViewerToolStripMenuItem
			// 
			openOParameterViewerToolStripMenuItem.Name = "openOParameterViewerToolStripMenuItem";
			openOParameterViewerToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
			openOParameterViewerToolStripMenuItem.Text = "Open Opaque Parameter Viewer";
			openOParameterViewerToolStripMenuItem.Click += openOParameterViewerToolStripMenuItem_Click;
			// 
			// openTParameterViewerToolStripMenuItem
			// 
			openTParameterViewerToolStripMenuItem.Name = "openTParameterViewerToolStripMenuItem";
			openTParameterViewerToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
			openTParameterViewerToolStripMenuItem.Text = "Open Translucent Parameter Viewer";
			openTParameterViewerToolStripMenuItem.Click += openTParameterViewerToolStripMenuItem_Click;
			// 
			// comboBoxNode
			// 
			comboBoxNode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxNode.FormattingEnabled = true;
			comboBoxNode.Location = new System.Drawing.Point(12, 18);
			comboBoxNode.Margin = new System.Windows.Forms.Padding(4);
			comboBoxNode.Name = "comboBoxNode";
			comboBoxNode.Size = new System.Drawing.Size(530, 23);
			comboBoxNode.TabIndex = 0;
			comboBoxNode.SelectedIndexChanged += comboBoxNode_SelectedIndexChanged;
			// 
			// groupBoxVertexList
			// 
			groupBoxVertexList.Controls.Add(listViewVertices);
			groupBoxVertexList.Location = new System.Drawing.Point(12, 240);
			groupBoxVertexList.Name = "groupBoxVertexList";
			groupBoxVertexList.Size = new System.Drawing.Size(364, 441);
			groupBoxVertexList.TabIndex = 29;
			groupBoxVertexList.TabStop = false;
			groupBoxVertexList.Text = "Vertex Data";
			// 
			// listViewVertices
			// 
			listViewVertices.AutoArrange = false;
			listViewVertices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
			listViewVertices.FullRowSelect = true;
			listViewVertices.GridLines = true;
			listViewVertices.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewVertices.Location = new System.Drawing.Point(8, 31);
			listViewVertices.Margin = new System.Windows.Forms.Padding(4);
			listViewVertices.MultiSelect = false;
			listViewVertices.Name = "listViewVertices";
			listViewVertices.ShowGroups = false;
			listViewVertices.Size = new System.Drawing.Size(346, 352);
			listViewVertices.TabIndex = 11;
			listViewVertices.UseCompatibleStateImageBehavior = false;
			listViewVertices.View = System.Windows.Forms.View.Details;
			listViewVertices.SelectedIndexChanged += listViewVertices_SelectedIndexChanged;
			listViewVertices.MouseClick += listViewVertices_MouseClick;
			// 
			// columnHeader1
			// 
			columnHeader1.Text = "Index";
			// 
			// columnHeader2
			// 
			columnHeader2.Text = "Type";
			// 
			// columnHeader3
			// 
			columnHeader3.Text = "Points";
			// 
			// columnHeader4
			// 
			columnHeader4.Text = "Format";
			// 
			// contextMenuStripVertData
			// 
			contextMenuStripVertData.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripVertData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { viewVertexDataToolStripMenuItem });
			contextMenuStripVertData.Name = "contextMenuStripVertData";
			contextMenuStripVertData.Size = new System.Drawing.Size(162, 26);
			// 
			// viewVertexDataToolStripMenuItem
			// 
			viewVertexDataToolStripMenuItem.Name = "viewVertexDataToolStripMenuItem";
			viewVertexDataToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			viewVertexDataToolStripMenuItem.Text = "View Vertex Data";
			viewVertexDataToolStripMenuItem.Click += viewVertexDataToolStripMenuItem_Click;
			// 
			// GCModelDataEditor
			// 
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			ClientSize = new System.Drawing.Size(1137, 764);
			Controls.Add(groupBoxVertexList);
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
			Name = "GCModelDataEditor";
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
			groupBoxTrans.ResumeLayout(false);
			groupBoxOpaque.ResumeLayout(false);
			contextMenuStripParamEdit.ResumeLayout(false);
			groupBoxVertexList.ResumeLayout(false);
			contextMenuStripVertData.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.Button buttonMoveTMeshUp;
		private System.Windows.Forms.Button buttonMoveTMeshDown;
		private System.Windows.Forms.ListView listViewOMeshes;
		private System.Windows.Forms.ColumnHeader columnHeaderIndex;
		private System.Windows.Forms.ColumnHeader columnHeaderUV;
		private System.Windows.Forms.ColumnHeader columnHeaderVcolor;
		private System.Windows.Forms.ColumnHeader columnHeaderPolynormals;
		private System.Windows.Forms.ColumnHeader columnHeaderMatID;
		private System.Windows.Forms.ColumnHeader columnHeaderEntries;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
		private System.Windows.Forms.Label labelModelName;
		private System.Windows.Forms.Label labelMeshsetName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxVertexName;
		private System.Windows.Forms.TextBox textBoxOPolyName;
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
		private System.Windows.Forms.Button buttonResetTMeshes;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.TextBox textBoxModelRadius;
		private System.Windows.Forms.Label labelR;
		private System.Windows.Forms.TextBox textBoxTPolyName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ColumnHeader columnHeaderPoly;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripParamEdit;
		private System.Windows.Forms.TextBox textBoxObjectName;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboBoxNode;
		private System.Windows.Forms.Button buttonDeleteTMesh;
		private System.Windows.Forms.Button buttonCloneTMesh;
		private System.Windows.Forms.GroupBox groupBoxVertexList;
		private System.Windows.Forms.ListView listViewVertices;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.GroupBox groupBoxTrans;
		private System.Windows.Forms.GroupBox groupBoxOpaque;
		private System.Windows.Forms.ListView listViewTMeshes;
		private System.Windows.Forms.Button buttonMoveOMeshUp;
		private System.Windows.Forms.Button buttonMoveOMeshDown;
		private System.Windows.Forms.ColumnHeader columnHeaderPrim;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.Button buttonResetOMeshes;
		private System.Windows.Forms.Button buttonDeleteOMesh;
		private System.Windows.Forms.Button buttonCloneOMesh;
		private System.Windows.Forms.ToolStripMenuItem openOParameterViewerToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripVertData;
		private System.Windows.Forms.ToolStripMenuItem viewVertexDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openTParameterViewerToolStripMenuItem;
	}
}