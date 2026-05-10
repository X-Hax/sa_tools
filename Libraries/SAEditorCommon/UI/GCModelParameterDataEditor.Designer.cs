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
			button1 = new Button();
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
			groupBoxStripFlags = new GroupBox();
			checkBoxDoubleSide = new CheckBox();
			checkBoxPunchthrough = new CheckBox();
			checkBoxVAmbient = new CheckBox();
			checkBoxUseAlpha = new CheckBox();
			checkBoxVMaterial = new CheckBox();
			checkBoxIgnoreSpecular = new CheckBox();
			checkBoxIgnoreAmbient = new CheckBox();
			checkBoxIgnoreLight = new CheckBox();
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
			toolTip1 = new ToolTip(components);
			groupBoxAmbient = new GroupBox();
			ambientBUpDown = new NumericUpDown();
			ambientGUpDown = new NumericUpDown();
			ambientRUpDown = new NumericUpDown();
			label7 = new Label();
			label8 = new Label();
			label9 = new Label();
			ambientColorBox = new Panel();
			label11 = new Label();
			groupBoxSpecular = new GroupBox();
			specularBUpDown = new NumericUpDown();
			specularGUpDown = new NumericUpDown();
			specularRUpDown = new NumericUpDown();
			label10 = new Label();
			label12 = new Label();
			label13 = new Label();
			specularColorBox = new Panel();
			label14 = new Label();
			statusStrip1.SuspendLayout();
			contextMenuStripMatEdit.SuspendLayout();
			groupBoxParamList.SuspendLayout();
			contextMenuStripVertCol.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)textureBox).BeginInit();
			groupBoxTexData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDownTexID).BeginInit();
			groupBoxBlendMode.SuspendLayout();
			groupBoxStripFlags.SuspendLayout();
			diffuseSettingBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)diffuseBUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)diffuseGUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)diffuseRUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).BeginInit();
			groupBoxUVScale.SuspendLayout();
			groupBoxAmbient.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ambientBUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)ambientGUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)ambientRUpDown).BeginInit();
			groupBoxSpecular.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)specularBUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)specularGUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)specularRUpDown).BeginInit();
			SuspendLayout();
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelInfo });
			statusStrip1.Location = new System.Drawing.Point(0, 446);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new Padding(1, 0, 10, 0);
			statusStrip1.Size = new System.Drawing.Size(924, 22);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 11;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelInfo
			// 
			toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
			toolStripStatusLabelInfo.Size = new System.Drawing.Size(469, 17);
			toolStripStatusLabelInfo.Text = "Click a parameter to display its information here. Right-click to edit pieces, if applicable.";
			// 
			// buttonClose
			// 
			buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonClose.DialogResult = DialogResult.OK;
			buttonClose.Location = new System.Drawing.Point(829, 417);
			buttonClose.Margin = new Padding(2);
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new System.Drawing.Size(84, 24);
			buttonClose.TabIndex = 44;
			buttonClose.Text = "Close";
			buttonClose.UseVisualStyleBackColor = true;
			buttonClose.Click += buttonClose_Click;
			// 
			// contextMenuStripMatEdit
			// 
			contextMenuStripMatEdit.ImageScalingSize = new System.Drawing.Size(24, 24);
			contextMenuStripMatEdit.Items.AddRange(new ToolStripItem[] { editPCMatToolStripMenuItem, editTextureIDToolStripMenuItem, editStripAlphaToolStripMenuItem });
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
			// groupBoxParamList
			// 
			groupBoxParamList.Controls.Add(button1);
			groupBoxParamList.Controls.Add(buttonResetAll);
			groupBoxParamList.Controls.Add(buttonResetParameter);
			groupBoxParamList.Controls.Add(listViewParameters);
			groupBoxParamList.Location = new System.Drawing.Point(419, 8);
			groupBoxParamList.Margin = new Padding(2);
			groupBoxParamList.Name = "groupBoxParamList";
			groupBoxParamList.Padding = new Padding(2);
			groupBoxParamList.Size = new System.Drawing.Size(496, 295);
			groupBoxParamList.TabIndex = 29;
			groupBoxParamList.TabStop = false;
			groupBoxParamList.Text = "Parameter Data";
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(5, 261);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(140, 23);
			button1.TabIndex = 31;
			button1.Text = "View Primitive Data";
			button1.UseVisualStyleBackColor = true;
			// 
			// buttonResetAll
			// 
			buttonResetAll.Location = new System.Drawing.Point(405, 261);
			buttonResetAll.Margin = new Padding(2);
			buttonResetAll.Name = "buttonResetAll";
			buttonResetAll.Size = new System.Drawing.Size(79, 23);
			buttonResetAll.TabIndex = 33;
			buttonResetAll.Text = "Reset All";
			buttonResetAll.UseVisualStyleBackColor = true;
			buttonResetAll.Click += buttonResetAll_Click;
			// 
			// buttonResetParameter
			// 
			buttonResetParameter.Location = new System.Drawing.Point(246, 261);
			buttonResetParameter.Margin = new Padding(2);
			buttonResetParameter.Name = "buttonResetParameter";
			buttonResetParameter.Size = new System.Drawing.Size(155, 23);
			buttonResetParameter.TabIndex = 32;
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
			listViewParameters.Location = new System.Drawing.Point(4, 19);
			listViewParameters.Margin = new Padding(2);
			listViewParameters.MultiSelect = false;
			listViewParameters.Name = "listViewParameters";
			listViewParameters.ShowGroups = false;
			listViewParameters.Size = new System.Drawing.Size(482, 236);
			listViewParameters.TabIndex = 30;
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
			contextMenuStripVertCol.Size = new System.Drawing.Size(195, 26);
			// 
			// showVertexCollectionToolStripMenuItem
			// 
			showVertexCollectionToolStripMenuItem.Name = "showVertexCollectionToolStripMenuItem";
			showVertexCollectionToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			showVertexCollectionToolStripMenuItem.Text = "Show Vertex Collection";
			// 
			// textureBox
			// 
			textureBox.BackgroundImageLayout = ImageLayout.Center;
			textureBox.BorderStyle = BorderStyle.FixedSingle;
			textureBox.Location = new System.Drawing.Point(11, 20);
			textureBox.Margin = new Padding(2);
			textureBox.Name = "textureBox";
			textureBox.Size = new System.Drawing.Size(136, 136);
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
			groupBoxTexData.Location = new System.Drawing.Point(8, 8);
			groupBoxTexData.Margin = new Padding(2);
			groupBoxTexData.Name = "groupBoxTexData";
			groupBoxTexData.Padding = new Padding(2);
			groupBoxTexData.Size = new System.Drawing.Size(243, 195);
			groupBoxTexData.TabIndex = 1;
			groupBoxTexData.TabStop = false;
			groupBoxTexData.Text = "Texture Data";
			// 
			// numericUpDownTexID
			// 
			numericUpDownTexID.Location = new System.Drawing.Point(71, 167);
			numericUpDownTexID.Margin = new Padding(2);
			numericUpDownTexID.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			numericUpDownTexID.Name = "numericUpDownTexID";
			numericUpDownTexID.Size = new System.Drawing.Size(51, 23);
			numericUpDownTexID.TabIndex = 7;
			numericUpDownTexID.ValueChanged += numericUpDownTexID_ValueChanged;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(4, 168);
			label1.Margin = new Padding(2, 0, 2, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(62, 15);
			label1.TabIndex = 36;
			label1.Text = "Texture ID:";
			// 
			// checkBoxUnkTexMode
			// 
			checkBoxUnkTexMode.AutoSize = true;
			checkBoxUnkTexMode.Location = new System.Drawing.Point(159, 123);
			checkBoxUnkTexMode.Margin = new Padding(2);
			checkBoxUnkTexMode.Name = "checkBoxUnkTexMode";
			checkBoxUnkTexMode.Size = new System.Drawing.Size(77, 19);
			checkBoxUnkTexMode.TabIndex = 6;
			checkBoxUnkTexMode.Text = "Unknown";
			checkBoxUnkTexMode.UseVisualStyleBackColor = true;
			checkBoxUnkTexMode.Click += checkBoxUnkTexMode_Click;
			// 
			// checkBoxMirrorV
			// 
			checkBoxMirrorV.AutoSize = true;
			checkBoxMirrorV.Location = new System.Drawing.Point(159, 100);
			checkBoxMirrorV.Margin = new Padding(2);
			checkBoxMirrorV.Name = "checkBoxMirrorV";
			checkBoxMirrorV.Size = new System.Drawing.Size(69, 19);
			checkBoxMirrorV.TabIndex = 5;
			checkBoxMirrorV.Text = "Mirror V";
			checkBoxMirrorV.UseVisualStyleBackColor = true;
			checkBoxMirrorV.Click += checkBoxMirrorV_Click;
			// 
			// checkBoxMirrorU
			// 
			checkBoxMirrorU.AutoSize = true;
			checkBoxMirrorU.Location = new System.Drawing.Point(159, 77);
			checkBoxMirrorU.Margin = new Padding(2);
			checkBoxMirrorU.Name = "checkBoxMirrorU";
			checkBoxMirrorU.Size = new System.Drawing.Size(70, 19);
			checkBoxMirrorU.TabIndex = 4;
			checkBoxMirrorU.Text = "Mirror U";
			checkBoxMirrorU.UseVisualStyleBackColor = true;
			checkBoxMirrorU.Click += checkBoxMirrorU_Click;
			// 
			// checkBoxWrapV
			// 
			checkBoxWrapV.AutoSize = true;
			checkBoxWrapV.Location = new System.Drawing.Point(159, 53);
			checkBoxWrapV.Margin = new Padding(2);
			checkBoxWrapV.Name = "checkBoxWrapV";
			checkBoxWrapV.Size = new System.Drawing.Size(64, 19);
			checkBoxWrapV.TabIndex = 3;
			checkBoxWrapV.Text = "Wrap V";
			checkBoxWrapV.UseVisualStyleBackColor = true;
			checkBoxWrapV.Click += checkBoxWrapV_Click;
			// 
			// checkBoxWrapU
			// 
			checkBoxWrapU.AutoSize = true;
			checkBoxWrapU.Location = new System.Drawing.Point(159, 30);
			checkBoxWrapU.Margin = new Padding(2);
			checkBoxWrapU.Name = "checkBoxWrapU";
			checkBoxWrapU.Size = new System.Drawing.Size(65, 19);
			checkBoxWrapU.TabIndex = 2;
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
			groupBoxBlendMode.Location = new System.Drawing.Point(8, 204);
			groupBoxBlendMode.Margin = new Padding(2);
			groupBoxBlendMode.Name = "groupBoxBlendMode";
			groupBoxBlendMode.Padding = new Padding(2);
			groupBoxBlendMode.Size = new System.Drawing.Size(191, 128);
			groupBoxBlendMode.TabIndex = 14;
			groupBoxBlendMode.TabStop = false;
			groupBoxBlendMode.Text = "Blend Modes";
			groupBoxBlendMode.Enter += groupBox2_Enter;
			// 
			// srcAlphaCombo
			// 
			srcAlphaCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			srcAlphaCombo.FormattingEnabled = true;
			srcAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "SourceColor", "InverseSourceColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			srcAlphaCombo.Location = new System.Drawing.Point(11, 44);
			srcAlphaCombo.Margin = new Padding(4, 3, 4, 3);
			srcAlphaCombo.Name = "srcAlphaCombo";
			srcAlphaCombo.Size = new System.Drawing.Size(164, 23);
			srcAlphaCombo.TabIndex = 15;
			srcAlphaCombo.SelectedIndexChanged += srcAlphaCombo_SelectedIndexChanged;
			// 
			// dstAlphaCombo
			// 
			dstAlphaCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			dstAlphaCombo.FormattingEnabled = true;
			dstAlphaCombo.Items.AddRange(new object[] { "Zero", "One", "SourceColor", "InverseSourceColor", "SourceAlpha", "InverseSourceAlpha", "DestinationAlpha", "InverseDestinationAlpha" });
			dstAlphaCombo.Location = new System.Drawing.Point(11, 97);
			dstAlphaCombo.Margin = new Padding(4, 3, 4, 3);
			dstAlphaCombo.Name = "dstAlphaCombo";
			dstAlphaCombo.Size = new System.Drawing.Size(164, 23);
			dstAlphaCombo.TabIndex = 16;
			dstAlphaCombo.SelectedIndexChanged += dstAlphaCombo_SelectedIndexChanged;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(7, 77);
			label3.Margin = new Padding(2, 0, 2, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(104, 15);
			label3.TabIndex = 1;
			label3.Text = "Destination Alpha:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(7, 25);
			label2.Margin = new Padding(2, 0, 2, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(80, 15);
			label2.TabIndex = 0;
			label2.Text = "Source Alpha:";
			// 
			// groupBoxStripFlags
			// 
			groupBoxStripFlags.Controls.Add(checkBoxDoubleSide);
			groupBoxStripFlags.Controls.Add(checkBoxPunchthrough);
			groupBoxStripFlags.Controls.Add(checkBoxVAmbient);
			groupBoxStripFlags.Controls.Add(checkBoxUseAlpha);
			groupBoxStripFlags.Controls.Add(checkBoxVMaterial);
			groupBoxStripFlags.Controls.Add(checkBoxIgnoreSpecular);
			groupBoxStripFlags.Controls.Add(checkBoxIgnoreAmbient);
			groupBoxStripFlags.Controls.Add(checkBoxIgnoreLight);
			groupBoxStripFlags.Controls.Add(checkBoxEnvMap);
			groupBoxStripFlags.Location = new System.Drawing.Point(420, 305);
			groupBoxStripFlags.Margin = new Padding(2);
			groupBoxStripFlags.Name = "groupBoxStripFlags";
			groupBoxStripFlags.Padding = new Padding(2);
			groupBoxStripFlags.Size = new System.Drawing.Size(421, 105);
			groupBoxStripFlags.TabIndex = 34;
			groupBoxStripFlags.TabStop = false;
			groupBoxStripFlags.Text = "Strip Flags";
			// 
			// checkBoxDoubleSide
			// 
			checkBoxDoubleSide.AutoSize = true;
			checkBoxDoubleSide.Location = new System.Drawing.Point(144, 71);
			checkBoxDoubleSide.Name = "checkBoxDoubleSide";
			checkBoxDoubleSide.Size = new System.Drawing.Size(96, 19);
			checkBoxDoubleSide.TabIndex = 40;
			checkBoxDoubleSide.Text = "Double Sided";
			toolTip1.SetToolTip(checkBoxDoubleSide, "This flag does nothing in SA2B.\r\nIf enabled, both sides of a mesh will be drawn - hence \"double sided\". When disabled, only the front faces of the mesh are drawn, with the back faces being culled.");
			checkBoxDoubleSide.UseVisualStyleBackColor = true;
			checkBoxDoubleSide.Click += checkBoxDoubleSide_Click;
			// 
			// checkBoxPunchthrough
			// 
			checkBoxPunchthrough.AutoSize = true;
			checkBoxPunchthrough.Location = new System.Drawing.Point(280, 46);
			checkBoxPunchthrough.Name = "checkBoxPunchthrough";
			checkBoxPunchthrough.Size = new System.Drawing.Size(122, 19);
			checkBoxPunchthrough.TabIndex = 42;
			checkBoxPunchthrough.Text = "No Punchthrough";
			toolTip1.SetToolTip(checkBoxPunchthrough, "This flag does nothing in SA2B.");
			checkBoxPunchthrough.UseVisualStyleBackColor = true;
			checkBoxPunchthrough.Click += checkBoxPunchthrough_Click;
			// 
			// checkBoxVAmbient
			// 
			checkBoxVAmbient.AutoSize = true;
			checkBoxVAmbient.Location = new System.Drawing.Point(144, 46);
			checkBoxVAmbient.Name = "checkBoxVAmbient";
			checkBoxVAmbient.Size = new System.Drawing.Size(106, 19);
			checkBoxVAmbient.TabIndex = 39;
			checkBoxVAmbient.Text = "Vertex Ambient";
			checkBoxVAmbient.UseVisualStyleBackColor = true;
			checkBoxVAmbient.Click += checkBoxVAmbient_Click;
			// 
			// checkBoxUseAlpha
			// 
			checkBoxUseAlpha.AutoSize = true;
			checkBoxUseAlpha.Location = new System.Drawing.Point(280, 21);
			checkBoxUseAlpha.Name = "checkBoxUseAlpha";
			checkBoxUseAlpha.Size = new System.Drawing.Size(79, 19);
			checkBoxUseAlpha.TabIndex = 41;
			checkBoxUseAlpha.Text = "Use Alpha";
			toolTip1.SetToolTip(checkBoxUseAlpha, "This flag does nothing in SA2B.\r\nIf checked, texture transparency will be enabled (and possibly non-texture transparency). ");
			checkBoxUseAlpha.UseVisualStyleBackColor = true;
			checkBoxUseAlpha.Click += checkBoxUseAlpha_Click;
			// 
			// checkBoxVMaterial
			// 
			checkBoxVMaterial.AutoSize = true;
			checkBoxVMaterial.Location = new System.Drawing.Point(144, 21);
			checkBoxVMaterial.Name = "checkBoxVMaterial";
			checkBoxVMaterial.Size = new System.Drawing.Size(103, 19);
			checkBoxVMaterial.TabIndex = 38;
			checkBoxVMaterial.Text = "Vertex Material";
			checkBoxVMaterial.UseVisualStyleBackColor = true;
			checkBoxVMaterial.Click += checkBoxVMaterial_Click;
			// 
			// checkBoxIgnoreSpecular
			// 
			checkBoxIgnoreSpecular.AutoSize = true;
			checkBoxIgnoreSpecular.Location = new System.Drawing.Point(15, 71);
			checkBoxIgnoreSpecular.Name = "checkBoxIgnoreSpecular";
			checkBoxIgnoreSpecular.Size = new System.Drawing.Size(108, 19);
			checkBoxIgnoreSpecular.TabIndex = 37;
			checkBoxIgnoreSpecular.Text = "Ignore Specular";
			toolTip1.SetToolTip(checkBoxIgnoreSpecular, "Disables specular lighting on the material.\r\nThis flag can be set, but it does nothing in games that utilize Ginja models.");
			checkBoxIgnoreSpecular.UseVisualStyleBackColor = true;
			checkBoxIgnoreSpecular.Click += checkBoxIgnoreSpecular_Click;
			// 
			// checkBoxIgnoreAmbient
			// 
			checkBoxIgnoreAmbient.AutoSize = true;
			checkBoxIgnoreAmbient.Location = new System.Drawing.Point(15, 46);
			checkBoxIgnoreAmbient.Name = "checkBoxIgnoreAmbient";
			checkBoxIgnoreAmbient.Size = new System.Drawing.Size(109, 19);
			checkBoxIgnoreAmbient.TabIndex = 36;
			checkBoxIgnoreAmbient.Text = "Ignore Ambient";
			toolTip1.SetToolTip(checkBoxIgnoreAmbient, resources.GetString("checkBoxIgnoreAmbient.ToolTip"));
			checkBoxIgnoreAmbient.UseVisualStyleBackColor = true;
			checkBoxIgnoreAmbient.Click += checkBoxIgnoreAmbient_Click;
			// 
			// checkBoxIgnoreLight
			// 
			checkBoxIgnoreLight.AutoSize = true;
			checkBoxIgnoreLight.Location = new System.Drawing.Point(15, 21);
			checkBoxIgnoreLight.Name = "checkBoxIgnoreLight";
			checkBoxIgnoreLight.Size = new System.Drawing.Size(107, 19);
			checkBoxIgnoreLight.TabIndex = 35;
			checkBoxIgnoreLight.Text = "Ignore Lighting";
			toolTip1.SetToolTip(checkBoxIgnoreLight, "If checked, the mesh will not have any lighting applied.");
			checkBoxIgnoreLight.UseVisualStyleBackColor = true;
			checkBoxIgnoreLight.Click += checkBoxIgnoreLight_Click;
			// 
			// checkBoxEnvMap
			// 
			checkBoxEnvMap.AutoSize = true;
			checkBoxEnvMap.Location = new System.Drawing.Point(280, 71);
			checkBoxEnvMap.Margin = new Padding(2);
			checkBoxEnvMap.Name = "checkBoxEnvMap";
			checkBoxEnvMap.Size = new System.Drawing.Size(121, 19);
			checkBoxEnvMap.TabIndex = 43;
			checkBoxEnvMap.Text = "Environment Map";
			toolTip1.SetToolTip(checkBoxEnvMap, resources.GetString("checkBoxEnvMap.ToolTip"));
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
			diffuseSettingBox.Location = new System.Drawing.Point(266, 9);
			diffuseSettingBox.Margin = new Padding(4, 3, 4, 3);
			diffuseSettingBox.Name = "diffuseSettingBox";
			diffuseSettingBox.Padding = new Padding(4, 3, 4, 3);
			diffuseSettingBox.Size = new System.Drawing.Size(119, 194);
			diffuseSettingBox.TabIndex = 8;
			diffuseSettingBox.TabStop = false;
			diffuseSettingBox.Text = "Diffuse";
			// 
			// diffuseBUpDown
			// 
			diffuseBUpDown.Location = new System.Drawing.Point(51, 117);
			diffuseBUpDown.Margin = new Padding(4, 3, 4, 3);
			diffuseBUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			diffuseBUpDown.Name = "diffuseBUpDown";
			diffuseBUpDown.Size = new System.Drawing.Size(49, 23);
			diffuseBUpDown.TabIndex = 12;
			diffuseBUpDown.ValueChanged += diffuseBUpDown_ValueChanged;
			// 
			// diffuseGUpDown
			// 
			diffuseGUpDown.Location = new System.Drawing.Point(51, 85);
			diffuseGUpDown.Margin = new Padding(4, 3, 4, 3);
			diffuseGUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			diffuseGUpDown.Name = "diffuseGUpDown";
			diffuseGUpDown.Size = new System.Drawing.Size(49, 23);
			diffuseGUpDown.TabIndex = 11;
			diffuseGUpDown.ValueChanged += diffuseGUpDown_ValueChanged;
			// 
			// diffuseRUpDown
			// 
			diffuseRUpDown.Location = new System.Drawing.Point(51, 53);
			diffuseRUpDown.Margin = new Padding(4, 3, 4, 3);
			diffuseRUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			diffuseRUpDown.Name = "diffuseRUpDown";
			diffuseRUpDown.Size = new System.Drawing.Size(49, 23);
			diffuseRUpDown.TabIndex = 10;
			diffuseRUpDown.ValueChanged += diffuseRUpDown_ValueChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(17, 119);
			label4.Margin = new Padding(2, 0, 2, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(33, 15);
			label4.TabIndex = 5;
			label4.Text = "Blue:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(8, 87);
			label5.Margin = new Padding(2, 0, 2, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(41, 15);
			label5.TabIndex = 4;
			label5.Text = "Green:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(19, 55);
			label6.Margin = new Padding(2, 0, 2, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(30, 15);
			label6.TabIndex = 3;
			label6.Text = "Red:";
			// 
			// labelAlpha
			// 
			labelAlpha.AutoSize = true;
			labelAlpha.Location = new System.Drawing.Point(8, 151);
			labelAlpha.Margin = new Padding(4, 0, 4, 0);
			labelAlpha.Name = "labelAlpha";
			labelAlpha.Size = new System.Drawing.Size(41, 15);
			labelAlpha.TabIndex = 2;
			labelAlpha.Text = "Alpha:";
			// 
			// alphaDiffuseNumeric
			// 
			alphaDiffuseNumeric.Location = new System.Drawing.Point(51, 149);
			alphaDiffuseNumeric.Margin = new Padding(4, 3, 4, 3);
			alphaDiffuseNumeric.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			alphaDiffuseNumeric.Name = "alphaDiffuseNumeric";
			alphaDiffuseNumeric.Size = new System.Drawing.Size(49, 23);
			alphaDiffuseNumeric.TabIndex = 13;
			alphaDiffuseNumeric.ValueChanged += alphaDiffuseNumeric_ValueChanged;
			// 
			// diffuseColorBox
			// 
			diffuseColorBox.BorderStyle = BorderStyle.FixedSingle;
			diffuseColorBox.Location = new System.Drawing.Point(51, 21);
			diffuseColorBox.Margin = new Padding(4, 3, 4, 3);
			diffuseColorBox.Name = "diffuseColorBox";
			diffuseColorBox.Size = new System.Drawing.Size(43, 22);
			diffuseColorBox.TabIndex = 9;
			diffuseColorBox.TabStop = true;
			diffuseColorBox.Click += diffuseColorBox_Click;
			// 
			// diffuseLabel
			// 
			diffuseLabel.AutoSize = true;
			diffuseLabel.Location = new System.Drawing.Point(10, 23);
			diffuseLabel.Margin = new Padding(4, 0, 4, 0);
			diffuseLabel.Name = "diffuseLabel";
			diffuseLabel.Size = new System.Drawing.Size(39, 15);
			diffuseLabel.TabIndex = 0;
			diffuseLabel.Text = "Color:";
			// 
			// groupBoxUVScale
			// 
			groupBoxUVScale.Controls.Add(comboBoxUVScale);
			groupBoxUVScale.Location = new System.Drawing.Point(8, 336);
			groupBoxUVScale.Margin = new Padding(2);
			groupBoxUVScale.Name = "groupBoxUVScale";
			groupBoxUVScale.Padding = new Padding(2);
			groupBoxUVScale.Size = new System.Drawing.Size(191, 61);
			groupBoxUVScale.TabIndex = 17;
			groupBoxUVScale.TabStop = false;
			groupBoxUVScale.Text = "UV Scale Data";
			// 
			// comboBoxUVScale
			// 
			comboBoxUVScale.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxUVScale.FormattingEnabled = true;
			comboBoxUVScale.Items.AddRange(new object[] { "Normal Scale (Default)", "No UV 1", "No UV 2", "No UV 3", "No UV 4", "No UV 5", "No UV 6", "No UV 7", "Normal Scale (1/1 Scale)", "1/2 Scale", "1/4 Scale", "1/8 Scale", "1/16 Scale", "1/32 Scale", "1/64 Scale", "1/128 Scale", "No UV (Scale9)" });
			comboBoxUVScale.Location = new System.Drawing.Point(11, 25);
			comboBoxUVScale.Margin = new Padding(4, 3, 4, 3);
			comboBoxUVScale.Name = "comboBoxUVScale";
			comboBoxUVScale.Size = new System.Drawing.Size(164, 23);
			comboBoxUVScale.TabIndex = 18;
			comboBoxUVScale.SelectedIndexChanged += comboBoxUVScale_SelectedIndexChanged;
			// 
			// groupBoxAmbient
			// 
			groupBoxAmbient.Controls.Add(ambientBUpDown);
			groupBoxAmbient.Controls.Add(ambientGUpDown);
			groupBoxAmbient.Controls.Add(ambientRUpDown);
			groupBoxAmbient.Controls.Add(label7);
			groupBoxAmbient.Controls.Add(label8);
			groupBoxAmbient.Controls.Add(label9);
			groupBoxAmbient.Controls.Add(ambientColorBox);
			groupBoxAmbient.Controls.Add(label11);
			groupBoxAmbient.Location = new System.Drawing.Point(205, 204);
			groupBoxAmbient.Margin = new Padding(4, 3, 4, 3);
			groupBoxAmbient.Name = "groupBoxAmbient";
			groupBoxAmbient.Padding = new Padding(4, 3, 4, 3);
			groupBoxAmbient.Size = new System.Drawing.Size(101, 155);
			groupBoxAmbient.TabIndex = 19;
			groupBoxAmbient.TabStop = false;
			groupBoxAmbient.Text = "Ambient";
			// 
			// ambientBUpDown
			// 
			ambientBUpDown.Location = new System.Drawing.Point(47, 117);
			ambientBUpDown.Margin = new Padding(4, 3, 4, 3);
			ambientBUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			ambientBUpDown.Name = "ambientBUpDown";
			ambientBUpDown.Size = new System.Drawing.Size(49, 23);
			ambientBUpDown.TabIndex = 23;
			ambientBUpDown.ValueChanged += ambientBUpDown_ValueChanged;
			// 
			// ambientGUpDown
			// 
			ambientGUpDown.Location = new System.Drawing.Point(47, 85);
			ambientGUpDown.Margin = new Padding(4, 3, 4, 3);
			ambientGUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			ambientGUpDown.Name = "ambientGUpDown";
			ambientGUpDown.Size = new System.Drawing.Size(49, 23);
			ambientGUpDown.TabIndex = 22;
			ambientGUpDown.ValueChanged += ambientGUpDown_ValueChanged;
			// 
			// ambientRUpDown
			// 
			ambientRUpDown.Location = new System.Drawing.Point(47, 53);
			ambientRUpDown.Margin = new Padding(4, 3, 4, 3);
			ambientRUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			ambientRUpDown.Name = "ambientRUpDown";
			ambientRUpDown.Size = new System.Drawing.Size(49, 23);
			ambientRUpDown.TabIndex = 21;
			ambientRUpDown.ValueChanged += ambientRUpDown_ValueChanged;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(13, 119);
			label7.Margin = new Padding(2, 0, 2, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(33, 15);
			label7.TabIndex = 5;
			label7.Text = "Blue:";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(4, 87);
			label8.Margin = new Padding(2, 0, 2, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(41, 15);
			label8.TabIndex = 4;
			label8.Text = "Green:";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(15, 55);
			label9.Margin = new Padding(2, 0, 2, 0);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(30, 15);
			label9.TabIndex = 3;
			label9.Text = "Red:";
			// 
			// ambientColorBox
			// 
			ambientColorBox.BorderStyle = BorderStyle.FixedSingle;
			ambientColorBox.Location = new System.Drawing.Point(47, 21);
			ambientColorBox.Margin = new Padding(4, 3, 4, 3);
			ambientColorBox.Name = "ambientColorBox";
			ambientColorBox.Size = new System.Drawing.Size(43, 22);
			ambientColorBox.TabIndex = 20;
			ambientColorBox.TabStop = true;
			ambientColorBox.Click += ambientColorBox_Click;
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(6, 23);
			label11.Margin = new Padding(4, 0, 4, 0);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(39, 15);
			label11.TabIndex = 0;
			label11.Text = "Color:";
			// 
			// groupBoxSpecular
			// 
			groupBoxSpecular.Controls.Add(specularBUpDown);
			groupBoxSpecular.Controls.Add(specularGUpDown);
			groupBoxSpecular.Controls.Add(specularRUpDown);
			groupBoxSpecular.Controls.Add(label10);
			groupBoxSpecular.Controls.Add(label12);
			groupBoxSpecular.Controls.Add(label13);
			groupBoxSpecular.Controls.Add(specularColorBox);
			groupBoxSpecular.Controls.Add(label14);
			groupBoxSpecular.Location = new System.Drawing.Point(313, 204);
			groupBoxSpecular.Margin = new Padding(4, 3, 4, 3);
			groupBoxSpecular.Name = "groupBoxSpecular";
			groupBoxSpecular.Padding = new Padding(4, 3, 4, 3);
			groupBoxSpecular.Size = new System.Drawing.Size(101, 155);
			groupBoxSpecular.TabIndex = 24;
			groupBoxSpecular.TabStop = false;
			groupBoxSpecular.Text = "Specular";
			// 
			// specularBUpDown
			// 
			specularBUpDown.Location = new System.Drawing.Point(45, 117);
			specularBUpDown.Margin = new Padding(4, 3, 4, 3);
			specularBUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			specularBUpDown.Name = "specularBUpDown";
			specularBUpDown.Size = new System.Drawing.Size(49, 23);
			specularBUpDown.TabIndex = 28;
			specularBUpDown.ValueChanged += specularBUpDown_ValueChanged;
			// 
			// specularGUpDown
			// 
			specularGUpDown.Location = new System.Drawing.Point(45, 85);
			specularGUpDown.Margin = new Padding(4, 3, 4, 3);
			specularGUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			specularGUpDown.Name = "specularGUpDown";
			specularGUpDown.Size = new System.Drawing.Size(49, 23);
			specularGUpDown.TabIndex = 27;
			specularGUpDown.ValueChanged += specularGUpDown_ValueChanged;
			// 
			// specularRUpDown
			// 
			specularRUpDown.Location = new System.Drawing.Point(45, 53);
			specularRUpDown.Margin = new Padding(4, 3, 4, 3);
			specularRUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			specularRUpDown.Name = "specularRUpDown";
			specularRUpDown.Size = new System.Drawing.Size(49, 23);
			specularRUpDown.TabIndex = 26;
			specularRUpDown.ValueChanged += specularRUpDown_ValueChanged;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(13, 119);
			label10.Margin = new Padding(2, 0, 2, 0);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(33, 15);
			label10.TabIndex = 5;
			label10.Text = "Blue:";
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(4, 87);
			label12.Margin = new Padding(2, 0, 2, 0);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(41, 15);
			label12.TabIndex = 4;
			label12.Text = "Green:";
			// 
			// label13
			// 
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(15, 55);
			label13.Margin = new Padding(2, 0, 2, 0);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(30, 15);
			label13.TabIndex = 3;
			label13.Text = "Red:";
			// 
			// specularColorBox
			// 
			specularColorBox.BorderStyle = BorderStyle.FixedSingle;
			specularColorBox.Location = new System.Drawing.Point(45, 21);
			specularColorBox.Margin = new Padding(4, 3, 4, 3);
			specularColorBox.Name = "specularColorBox";
			specularColorBox.Size = new System.Drawing.Size(43, 22);
			specularColorBox.TabIndex = 25;
			specularColorBox.TabStop = true;
			specularColorBox.Click += specularColorBox_Click;
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(6, 23);
			label14.Margin = new Padding(4, 0, 4, 0);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(39, 15);
			label14.TabIndex = 0;
			label14.Text = "Color:";
			// 
			// GCModelParameterDataEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(924, 468);
			Controls.Add(groupBoxSpecular);
			Controls.Add(groupBoxAmbient);
			Controls.Add(groupBoxUVScale);
			Controls.Add(diffuseSettingBox);
			Controls.Add(groupBoxStripFlags);
			Controls.Add(groupBoxBlendMode);
			Controls.Add(groupBoxTexData);
			Controls.Add(groupBoxParamList);
			Controls.Add(buttonClose);
			Controls.Add(statusStrip1);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
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
			groupBoxStripFlags.ResumeLayout(false);
			groupBoxStripFlags.PerformLayout();
			diffuseSettingBox.ResumeLayout(false);
			diffuseSettingBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)diffuseBUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)diffuseGUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)diffuseRUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)alphaDiffuseNumeric).EndInit();
			groupBoxUVScale.ResumeLayout(false);
			groupBoxAmbient.ResumeLayout(false);
			groupBoxAmbient.PerformLayout();
			((System.ComponentModel.ISupportInitialize)ambientBUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)ambientGUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)ambientRUpDown).EndInit();
			groupBoxSpecular.ResumeLayout(false);
			groupBoxSpecular.PerformLayout();
			((System.ComponentModel.ISupportInitialize)specularBUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)specularGUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)specularRUpDown).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Label label2;
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
		private System.Windows.Forms.GroupBox groupBoxStripFlags;
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
		private Button button1;
		private CheckBox checkBoxVMaterial;
		private CheckBox checkBoxIgnoreSpecular;
		private CheckBox checkBoxIgnoreAmbient;
		private CheckBox checkBoxIgnoreLight;
		private ToolTip toolTip1;
		private CheckBox checkBoxDoubleSide;
		private CheckBox checkBoxPunchthrough;
		private CheckBox checkBoxVAmbient;
		private CheckBox checkBoxUseAlpha;
		private GroupBox groupBoxAmbient;
		private NumericUpDown ambientBUpDown;
		private NumericUpDown ambientGUpDown;
		private NumericUpDown ambientRUpDown;
		private Label label7;
		private Label label8;
		private Label label9;
		private Panel ambientColorBox;
		private Label label11;
		private GroupBox groupBoxSpecular;
		private NumericUpDown specularBUpDown;
		private NumericUpDown specularGUpDown;
		private NumericUpDown specularRUpDown;
		private Label label10;
		private Label label12;
		private Label label13;
		private Panel specularColorBox;
		private Label label14;
	}
}