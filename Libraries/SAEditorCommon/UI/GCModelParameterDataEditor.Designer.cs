using System.Windows.Forms;

namespace SAModel.SAEditorCommon.UI
{
	partial class GCModelParameterDataEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCModelParameterDataEditor));
			statusStrip1 = new StatusStrip();
			toolStripStatusLabelInfo = new ToolStripStatusLabel();
			buttonClose = new Button();
			contextMenuStripMatEdit = new ContextMenuStrip(components);
			editPCMatToolStripMenuItem = new ToolStripMenuItem();
			editTextureIDToolStripMenuItem = new ToolStripMenuItem();
			editStripAlphaToolStripMenuItem = new ToolStripMenuItem();
			groupBoxParamList = new GroupBox();
			buttonResetAll = new Button();
			buttonResetParameter = new Button();
			listViewParameters = new ListView();
			columnHeaderParamID = new ColumnHeader();
			columnHeaderParamType = new ColumnHeader();
			columnHeaderParamData = new ColumnHeader();
			contextMenuStripVertCol = new ContextMenuStrip(components);
			showVertexCollectionToolStripMenuItem = new ToolStripMenuItem();
			textureBox = new PictureBox();
			groupBoxTexData = new GroupBox();
			numericUpDownTexID = new NumericUpDown();
			label1 = new Label();
			checkBoxUnkTexMode = new CheckBox();
			checkBoxMirrorV = new CheckBox();
			checkBoxMirrorU = new CheckBox();
			checkBoxWrapV = new CheckBox();
			checkBoxWrapU = new CheckBox();
			groupBoxBlendMode = new GroupBox();
			srcAlphaCombo = new ComboBox();
			dstAlphaCombo = new ComboBox();
			label3 = new Label();
			label2 = new Label();
			groupBoxEnvMap = new GroupBox();
			checkBoxEnvMap = new CheckBox();
			colorDialog = new ColorDialog();
			diffuseSettingBox = new GroupBox();
			diffuseBUpDown = new NumericUpDown();
			diffuseGUpDown = new NumericUpDown();
			diffuseRUpDown = new NumericUpDown();
			label4 = new Label();
			label5 = new Label();
			label6 = new Label();
			labelAlpha = new Label();
			alphaDiffuseNumeric = new NumericUpDown();
			diffuseColorBox = new Panel();
			diffuseLabel = new Label();
			groupBoxUVScale = new GroupBox();
			comboBoxUVScale = new ComboBox();
			statusStrip1.SuspendLayout();
			contextMenuStripMatEdit.SuspendLayout();
			groupBoxParamList.SuspendLayout();
			contextMenuStripVertCol.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)textureBox).BeginInit();
			groupBoxTexData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownTexID).BeginInit();
			groupBoxBlendMode.SuspendLayout();
			groupBoxEnvMap.SuspendLayout();
			diffuseSettingBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)diffuseBUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)diffuseGUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)diffuseRUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).BeginInit();
			groupBoxUVScale.SuspendLayout();
			SuspendLayout();
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelInfo });
			statusStrip1.Location = new System.Drawing.Point(0, 504);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new Padding(2, 0, 15, 0);
			statusStrip1.Size = new System.Drawing.Size(1348, 32);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 11;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelInfo
			// 
			toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
			toolStripStatusLabelInfo.Size = new System.Drawing.Size(701, 25);
			toolStripStatusLabelInfo.Text = "Click a parameter to display its information here. Right-click to edit pieces, if applicable.";
			// 
			// buttonClose
			// 
			buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonClose.DialogResult = DialogResult.OK;
			buttonClose.Location = new System.Drawing.Point(1216, 459);
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
			contextMenuStripMatEdit.Items.AddRange(new ToolStripItem[] { editPCMatToolStripMenuItem, editTextureIDToolStripMenuItem, editStripAlphaToolStripMenuItem });
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
			// groupBoxParamList
			// 
			groupBoxParamList.Controls.Add(buttonResetAll);
			groupBoxParamList.Controls.Add(buttonResetParameter);
			groupBoxParamList.Controls.Add(listViewParameters);
			groupBoxParamList.Location = new System.Drawing.Point(568, 12);
			groupBoxParamList.Name = "groupBoxParamList";
			groupBoxParamList.Size = new System.Drawing.Size(744, 443);
			groupBoxParamList.TabIndex = 29;
			groupBoxParamList.TabStop = false;
			groupBoxParamList.Text = "Parameter Data";
			// 
			// buttonResetAll
			// 
			buttonResetAll.Location = new System.Drawing.Point(608, 392);
			buttonResetAll.Name = "buttonResetAll";
			buttonResetAll.Size = new System.Drawing.Size(119, 34);
			buttonResetAll.TabIndex = 13;
			buttonResetAll.Text = "Reset All";
			buttonResetAll.UseVisualStyleBackColor = true;
			buttonResetAll.Click += buttonResetAll_Click;
			// 
			// buttonResetParameter
			// 
			buttonResetParameter.Location = new System.Drawing.Point(369, 392);
			buttonResetParameter.Name = "buttonResetParameter";
			buttonResetParameter.Size = new System.Drawing.Size(233, 34);
			buttonResetParameter.TabIndex = 12;
			buttonResetParameter.Text = "Reset Selected Parameter";
			buttonResetParameter.UseVisualStyleBackColor = true;
			buttonResetParameter.Click += buttonResetParameter_Click;
			// 
			// listViewParameters
			// 
			listViewParameters.AutoArrange = false;
			listViewParameters.Columns.AddRange(new ColumnHeader[] { columnHeaderParamID, columnHeaderParamType, columnHeaderParamData });
			listViewParameters.FullRowSelect = true;
			listViewParameters.GridLines = true;
			listViewParameters.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			listViewParameters.Location = new System.Drawing.Point(6, 28);
			listViewParameters.MultiSelect = false;
			listViewParameters.Name = "listViewParameters";
			listViewParameters.ShowGroups = false;
			listViewParameters.Size = new System.Drawing.Size(721, 352);
			listViewParameters.TabIndex = 11;
			listViewParameters.UseCompatibleStateImageBehavior = false;
			listViewParameters.View = View.Details;
			listViewParameters.SelectedIndexChanged += listViewParameters_SelectedIndexChanged;
			// 
			// columnHeaderParamID
			// 
			columnHeaderParamID.Text = "Index";
			// 
			// columnHeaderParamType
			// 
			columnHeaderParamType.Text = "Type";
			// 
			// columnHeaderParamData
			// 
			columnHeaderParamData.Text = "Data";
			// 
			// contextMenuStripVertCol
			// 
			contextMenuStripVertCol.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripVertCol.Items.AddRange(new ToolStripItem[] { showVertexCollectionToolStripMenuItem });
			contextMenuStripVertCol.Name = "contextMenuStripVertCol";
			contextMenuStripVertCol.Size = new System.Drawing.Size(265, 36);
			// 
			// showVertexCollectionToolStripMenuItem
			// 
			showVertexCollectionToolStripMenuItem.Name = "showVertexCollectionToolStripMenuItem";
			showVertexCollectionToolStripMenuItem.Size = new System.Drawing.Size(264, 32);
			showVertexCollectionToolStripMenuItem.Text = "Show Vertex Collection";
			// 
			// textureBox
			// 
			textureBox.BackgroundImageLayout = ImageLayout.Center;
			textureBox.BorderStyle = BorderStyle.FixedSingle;
			textureBox.Location = new System.Drawing.Point(16, 30);
			textureBox.Name = "textureBox";
			textureBox.Size = new System.Drawing.Size(203, 203);
			textureBox.SizeMode = PictureBoxSizeMode.CenterImage;
			textureBox.TabIndex = 30;
			textureBox.TabStop = false;
			textureBox.Click += textureBox_Click;
			// 
			// groupBoxTexData
			// 
			groupBoxTexData.Controls.Add(numericUpDownTexID);
			groupBoxTexData.Controls.Add(label1);
			groupBoxTexData.Controls.Add(checkBoxUnkTexMode);
			groupBoxTexData.Controls.Add(checkBoxMirrorV);
			groupBoxTexData.Controls.Add(checkBoxMirrorU);
			groupBoxTexData.Controls.Add(checkBoxWrapV);
			groupBoxTexData.Controls.Add(checkBoxWrapU);
			groupBoxTexData.Controls.Add(textureBox);
			groupBoxTexData.Location = new System.Drawing.Point(12, 12);
			groupBoxTexData.Name = "groupBoxTexData";
			groupBoxTexData.Size = new System.Drawing.Size(364, 292);
			groupBoxTexData.TabIndex = 31;
			groupBoxTexData.TabStop = false;
			groupBoxTexData.Text = "Texture Data";
			// 
			// numericUpDownTexID
			// 
			numericUpDownTexID.Location = new System.Drawing.Point(106, 250);
			numericUpDownTexID.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			numericUpDownTexID.Name = "numericUpDownTexID";
			numericUpDownTexID.Size = new System.Drawing.Size(76, 31);
			numericUpDownTexID.TabIndex = 37;
			numericUpDownTexID.ValueChanged += numericUpDownTexID_ValueChanged;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(6, 252);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(94, 25);
			label1.TabIndex = 36;
			label1.Text = "Texture ID:";
			// 
			// checkBoxUnkTexMode
			// 
			checkBoxUnkTexMode.AutoSize = true;
			checkBoxUnkTexMode.Location = new System.Drawing.Point(239, 185);
			checkBoxUnkTexMode.Name = "checkBoxUnkTexMode";
			checkBoxUnkTexMode.Size = new System.Drawing.Size(113, 29);
			checkBoxUnkTexMode.TabIndex = 35;
			checkBoxUnkTexMode.Text = "Unknown";
			checkBoxUnkTexMode.UseVisualStyleBackColor = true;
			checkBoxUnkTexMode.Click += checkBoxUnkTexMode_Click;
			// 
			// checkBoxMirrorV
			// 
			checkBoxMirrorV.AutoSize = true;
			checkBoxMirrorV.Location = new System.Drawing.Point(239, 150);
			checkBoxMirrorV.Name = "checkBoxMirrorV";
			checkBoxMirrorV.Size = new System.Drawing.Size(103, 29);
			checkBoxMirrorV.TabIndex = 34;
			checkBoxMirrorV.Text = "Mirror V";
			checkBoxMirrorV.UseVisualStyleBackColor = true;
			checkBoxMirrorV.Click += checkBoxMirrorV_Click;
			// 
			// checkBoxMirrorU
			// 
			checkBoxMirrorU.AutoSize = true;
			checkBoxMirrorU.Location = new System.Drawing.Point(239, 115);
			checkBoxMirrorU.Name = "checkBoxMirrorU";
			checkBoxMirrorU.Size = new System.Drawing.Size(104, 29);
			checkBoxMirrorU.TabIndex = 33;
			checkBoxMirrorU.Text = "Mirror U";
			checkBoxMirrorU.UseVisualStyleBackColor = true;
			checkBoxMirrorU.Click += checkBoxMirrorU_Click;
			// 
			// checkBoxWrapV
			// 
			checkBoxWrapV.AutoSize = true;
			checkBoxWrapV.Location = new System.Drawing.Point(239, 80);
			checkBoxWrapV.Name = "checkBoxWrapV";
			checkBoxWrapV.Size = new System.Drawing.Size(97, 29);
			checkBoxWrapV.TabIndex = 32;
			checkBoxWrapV.Text = "Wrap V";
			checkBoxWrapV.UseVisualStyleBackColor = true;
			checkBoxWrapV.Click += checkBoxWrapV_Click;
			// 
			// checkBoxWrapU
			// 
			checkBoxWrapU.AutoSize = true;
			checkBoxWrapU.Location = new System.Drawing.Point(239, 45);
			checkBoxWrapU.Name = "checkBoxWrapU";
			checkBoxWrapU.Size = new System.Drawing.Size(98, 29);
			checkBoxWrapU.TabIndex = 31;
			checkBoxWrapU.Text = "Wrap U";
			checkBoxWrapU.UseVisualStyleBackColor = true;
			checkBoxWrapU.Click += checkBoxWrapU_Click;
			// 
			// groupBoxBlendMode
			// 
			groupBoxBlendMode.Controls.Add(srcAlphaCombo);
			groupBoxBlendMode.Controls.Add(dstAlphaCombo);
			groupBoxBlendMode.Controls.Add(label3);
			groupBoxBlendMode.Controls.Add(label2);
			groupBoxBlendMode.Location = new System.Drawing.Point(12, 306);
			groupBoxBlendMode.Name = "groupBoxBlendMode";
			groupBoxBlendMode.Size = new System.Drawing.Size(287, 192);
			groupBoxBlendMode.TabIndex = 32;
			groupBoxBlendMode.TabStop = false;
			groupBoxBlendMode.Text = "Blend Modes";
			groupBoxBlendMode.Enter += groupBox2_Enter;
			// 
			// srcAlphaCombo
			// 
			srcAlphaCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			srcAlphaCombo.FormattingEnabled = true;
			srcAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "SourceColor", "InverseSourceColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			srcAlphaCombo.Location = new System.Drawing.Point(16, 66);
			srcAlphaCombo.Margin = new Padding(6, 4, 6, 4);
			srcAlphaCombo.Name = "srcAlphaCombo";
			srcAlphaCombo.Size = new System.Drawing.Size(244, 33);
			srcAlphaCombo.TabIndex = 35;
			srcAlphaCombo.SelectedIndexChanged += srcAlphaCombo_SelectedIndexChanged;
			// 
			// dstAlphaCombo
			// 
			dstAlphaCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			dstAlphaCombo.FormattingEnabled = true;
			dstAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "SourceColor", "InverseSourceColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			dstAlphaCombo.Location = new System.Drawing.Point(16, 145);
			dstAlphaCombo.Margin = new Padding(6, 4, 6, 4);
			dstAlphaCombo.Name = "dstAlphaCombo";
			dstAlphaCombo.Size = new System.Drawing.Size(244, 33);
			dstAlphaCombo.TabIndex = 36;
			dstAlphaCombo.SelectedIndexChanged += dstAlphaCombo_SelectedIndexChanged;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(10, 115);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(157, 25);
			label3.TabIndex = 1;
			label3.Text = "Destination Alpha:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(10, 37);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(121, 25);
			label2.TabIndex = 0;
			label2.Text = "Source Alpha:";
			// 
			// groupBoxEnvMap
			// 
			groupBoxEnvMap.Controls.Add(checkBoxEnvMap);
			groupBoxEnvMap.Location = new System.Drawing.Point(305, 306);
			groupBoxEnvMap.Name = "groupBoxEnvMap";
			groupBoxEnvMap.Size = new System.Drawing.Size(257, 94);
			groupBoxEnvMap.TabIndex = 33;
			groupBoxEnvMap.TabStop = false;
			groupBoxEnvMap.Text = "Environment Map";
			// 
			// checkBoxEnvMap
			// 
			checkBoxEnvMap.AutoSize = true;
			checkBoxEnvMap.Location = new System.Drawing.Point(36, 39);
			checkBoxEnvMap.Name = "checkBoxEnvMap";
			checkBoxEnvMap.Size = new System.Drawing.Size(179, 29);
			checkBoxEnvMap.TabIndex = 0;
			checkBoxEnvMap.Text = "Environment Map";
			checkBoxEnvMap.UseVisualStyleBackColor = true;
			checkBoxEnvMap.Click += checkBoxEnvMap_Click;
			// 
			// colorDialog
			// 
			colorDialog.AnyColor = true;
			colorDialog.FullOpen = true;
			colorDialog.SolidColorOnly = true;
			// 
			// diffuseSettingBox
			// 
			diffuseSettingBox.Controls.Add(diffuseBUpDown);
			diffuseSettingBox.Controls.Add(diffuseGUpDown);
			diffuseSettingBox.Controls.Add(diffuseRUpDown);
			diffuseSettingBox.Controls.Add(label4);
			diffuseSettingBox.Controls.Add(label5);
			diffuseSettingBox.Controls.Add(label6);
			diffuseSettingBox.Controls.Add(labelAlpha);
			diffuseSettingBox.Controls.Add(alphaDiffuseNumeric);
			diffuseSettingBox.Controls.Add(diffuseColorBox);
			diffuseSettingBox.Controls.Add(diffuseLabel);
			diffuseSettingBox.Location = new System.Drawing.Point(384, 13);
			diffuseSettingBox.Margin = new Padding(6, 4, 6, 4);
			diffuseSettingBox.Name = "diffuseSettingBox";
			diffuseSettingBox.Padding = new Padding(6, 4, 6, 4);
			diffuseSettingBox.Size = new System.Drawing.Size(178, 291);
			diffuseSettingBox.TabIndex = 34;
			diffuseSettingBox.TabStop = false;
			diffuseSettingBox.Text = "Diffuse";
			// 
			// diffuseBUpDown
			// 
			diffuseBUpDown.Location = new System.Drawing.Point(76, 176);
			diffuseBUpDown.Margin = new Padding(6, 4, 6, 4);
			diffuseBUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			diffuseBUpDown.Name = "diffuseBUpDown";
			diffuseBUpDown.Size = new System.Drawing.Size(73, 31);
			diffuseBUpDown.TabIndex = 8;
			diffuseBUpDown.ValueChanged += diffuseBUpDown_ValueChanged;
			// 
			// diffuseGUpDown
			// 
			diffuseGUpDown.Location = new System.Drawing.Point(76, 128);
			diffuseGUpDown.Margin = new Padding(6, 4, 6, 4);
			diffuseGUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			diffuseGUpDown.Name = "diffuseGUpDown";
			diffuseGUpDown.Size = new System.Drawing.Size(73, 31);
			diffuseGUpDown.TabIndex = 7;
			diffuseGUpDown.ValueChanged += diffuseGUpDown_ValueChanged;
			// 
			// diffuseRUpDown
			// 
			diffuseRUpDown.Location = new System.Drawing.Point(76, 80);
			diffuseRUpDown.Margin = new Padding(6, 4, 6, 4);
			diffuseRUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			diffuseRUpDown.Name = "diffuseRUpDown";
			diffuseRUpDown.Size = new System.Drawing.Size(73, 31);
			diffuseRUpDown.TabIndex = 6;
			diffuseRUpDown.ValueChanged += diffuseRUpDown_ValueChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(25, 179);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(49, 25);
			label4.TabIndex = 5;
			label4.Text = "Blue:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(12, 131);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(62, 25);
			label5.TabIndex = 4;
			label5.Text = "Green:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(28, 83);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(46, 25);
			label6.TabIndex = 3;
			label6.Text = "Red:";
			// 
			// labelAlpha
			// 
			labelAlpha.AutoSize = true;
			labelAlpha.Location = new System.Drawing.Point(12, 227);
			labelAlpha.Margin = new Padding(6, 0, 6, 0);
			labelAlpha.Name = "labelAlpha";
			labelAlpha.Size = new System.Drawing.Size(62, 25);
			labelAlpha.TabIndex = 2;
			labelAlpha.Text = "Alpha:";
			// 
			// alphaDiffuseNumeric
			// 
			alphaDiffuseNumeric.Location = new System.Drawing.Point(76, 224);
			alphaDiffuseNumeric.Margin = new Padding(6, 4, 6, 4);
			alphaDiffuseNumeric.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			alphaDiffuseNumeric.Name = "alphaDiffuseNumeric";
			alphaDiffuseNumeric.Size = new System.Drawing.Size(73, 31);
			alphaDiffuseNumeric.TabIndex = 2;
			alphaDiffuseNumeric.ValueChanged += alphaDiffuseNumeric_ValueChanged;
			// 
			// diffuseColorBox
			// 
			diffuseColorBox.BorderStyle = BorderStyle.FixedSingle;
			diffuseColorBox.Location = new System.Drawing.Point(76, 32);
			diffuseColorBox.Margin = new Padding(6, 4, 6, 4);
			diffuseColorBox.Name = "diffuseColorBox";
			diffuseColorBox.Size = new System.Drawing.Size(64, 32);
			diffuseColorBox.TabIndex = 1;
			diffuseColorBox.TabStop = true;
			diffuseColorBox.Click += diffuseColorBox_Click;
			// 
			// diffuseLabel
			// 
			diffuseLabel.AutoSize = true;
			diffuseLabel.Location = new System.Drawing.Point(15, 35);
			diffuseLabel.Margin = new Padding(6, 0, 6, 0);
			diffuseLabel.Name = "diffuseLabel";
			diffuseLabel.Size = new System.Drawing.Size(59, 25);
			diffuseLabel.TabIndex = 0;
			diffuseLabel.Text = "Color:";
			// 
			// groupBoxUVScale
			// 
			groupBoxUVScale.Controls.Add(comboBoxUVScale);
			groupBoxUVScale.Location = new System.Drawing.Point(305, 404);
			groupBoxUVScale.Name = "groupBoxUVScale";
			groupBoxUVScale.Size = new System.Drawing.Size(257, 91);
			groupBoxUVScale.TabIndex = 35;
			groupBoxUVScale.TabStop = false;
			groupBoxUVScale.Text = "UV Scale Data";
			// 
			// comboBoxUVScale
			// 
			comboBoxUVScale.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxUVScale.FormattingEnabled = true;
			comboBoxUVScale.Items.AddRange(new object[] { "Normal Scale (Default)", "No UV 1", "No UV 2", "No UV 3", "No UV 4", "No UV 5", "No UV 6", "No UV 7", "Normal Scale (1/1 Scale)", "1/2 Scale", "1/4 Scale", "1/8 Scale", "1/16 Scale", "1/32 Scale", "1/64 Scale", "1/128 Scale", "No UV (Scale9)" });
			comboBoxUVScale.Location = new System.Drawing.Point(13, 37);
			comboBoxUVScale.Margin = new Padding(6, 4, 6, 4);
			comboBoxUVScale.Name = "comboBoxUVScale";
			comboBoxUVScale.Size = new System.Drawing.Size(227, 33);
			comboBoxUVScale.TabIndex = 37;
			comboBoxUVScale.SelectedIndexChanged += comboBoxUVScale_SelectedIndexChanged;
			// 
			// GCModelParameterDataEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new System.Drawing.Size(1348, 536);
			Controls.Add(groupBoxUVScale);
			Controls.Add(diffuseSettingBox);
			Controls.Add(groupBoxEnvMap);
			Controls.Add(groupBoxBlendMode);
			Controls.Add(groupBoxTexData);
			Controls.Add(groupBoxParamList);
			Controls.Add(buttonClose);
			Controls.Add(statusStrip1);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new Padding(4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "GCModelParameterDataEditor";
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Parameter Editor";
			Load += MaterialEditor_Load;
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			contextMenuStripMatEdit.ResumeLayout(false);
			groupBoxParamList.ResumeLayout(false);
			contextMenuStripVertCol.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)textureBox).EndInit();
			groupBoxTexData.ResumeLayout(false);
			groupBoxTexData.PerformLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownTexID).EndInit();
			groupBoxBlendMode.ResumeLayout(false);
			groupBoxBlendMode.PerformLayout();
			groupBoxEnvMap.ResumeLayout(false);
			groupBoxEnvMap.PerformLayout();
			diffuseSettingBox.ResumeLayout(false);
			diffuseSettingBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)diffuseBUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)diffuseGUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)diffuseRUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).EndInit();
			groupBoxUVScale.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.ColorDialog colorDialog;
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
		private System.Windows.Forms.ColumnHeader columnHeaderCnkType;
		private System.Windows.Forms.ColumnHeader columnHeaderCnkData;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripMatEdit;
		private System.Windows.Forms.ToolStripMenuItem editPCMatToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editTextureIDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editStripAlphaToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBoxParamList;
		private System.Windows.Forms.ListView listViewParameters;
		private System.Windows.Forms.ColumnHeader columnHeaderParamID;
		private System.Windows.Forms.ColumnHeader columnHeaderParamType;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripVertCol;
		private System.Windows.Forms.ToolStripMenuItem showVertexCollectionToolStripMenuItem;
		private System.Windows.Forms.Button buttonResetParameter;
		private System.Windows.Forms.Button buttonResetAll;
		private System.Windows.Forms.ColumnHeader columnHeaderParamData;
		private System.Windows.Forms.PictureBox textureBox;
		private System.Windows.Forms.GroupBox groupBoxTexData;
		private System.Windows.Forms.CheckBox checkBoxUnkTexMode;
		private System.Windows.Forms.CheckBox checkBoxMirrorV;
		private System.Windows.Forms.CheckBox checkBoxMirrorU;
		private System.Windows.Forms.CheckBox checkBoxWrapV;
		private System.Windows.Forms.CheckBox checkBoxWrapU;
		private System.Windows.Forms.NumericUpDown numericUpDownTexID;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBoxBlendMode;
		private System.Windows.Forms.GroupBox groupBoxEnvMap;
		private System.Windows.Forms.CheckBox checkBoxEnvMap;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox diffuseSettingBox;
		private System.Windows.Forms.NumericUpDown diffuseBUpDown;
		private System.Windows.Forms.NumericUpDown diffuseGUpDown;
		private System.Windows.Forms.NumericUpDown diffuseRUpDown;
		private System.Windows.Forms.Label labelAlpha;
		private System.Windows.Forms.NumericUpDown alphaDiffuseNumeric;
		private System.Windows.Forms.Panel diffuseColorBox;
		private System.Windows.Forms.Label diffuseLabel;
		private ComboBox srcAlphaCombo;
		private ComboBox dstAlphaCombo;
		private GroupBox groupBoxUVScale;
		private ComboBox comboBoxUVScale;
	}
}