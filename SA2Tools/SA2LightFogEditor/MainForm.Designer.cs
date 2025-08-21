namespace SA2LightFogEditor
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			menuStrip1 = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			newToolStripMenuItem = new ToolStripMenuItem();
			lightFileToolStripMenuItem = new ToolStripMenuItem();
			lightFileGCToolStripMenuItem = new ToolStripMenuItem();
			fogFileToolStripMenuItem = new ToolStripMenuItem();
			fogFileGCToolStripMenuItem = new ToolStripMenuItem();
			openToolStripMenuItem = new ToolStripMenuItem();
			recentFilesToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator1 = new ToolStripSeparator();
			saveToolStripMenuItem = new ToolStripMenuItem();
			saveAsToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator2 = new ToolStripSeparator();
			exitToolStripMenuItem = new ToolStripMenuItem();
			optionsToolStripMenuItem = new ToolStripMenuItem();
			bigEndianToolStripMenuItem = new ToolStripMenuItem();
			groupBoxLightData = new GroupBox();
			lightColorPanel = new Panel();
			lightAmbientMultiTextBox = new TextBox();
			lightColorMultiTextBox = new TextBox();
			lightZDirTextBox = new TextBox();
			lightYDirTextBox = new TextBox();
			lightXDirTextBox = new TextBox();
			lightSetNumericUpDown = new NumericUpDown();
			label7 = new Label();
			label6 = new Label();
			label5 = new Label();
			label4 = new Label();
			label3 = new Label();
			label2 = new Label();
			label1 = new Label();
			colorDialog = new ColorDialog();
			groupBoxGCLightData = new GroupBox();
			lightGCOverrideCheckBox = new CheckBox();
			label16 = new Label();
			lightGCUnk2NumericUpDown = new NumericUpDown();
			label15 = new Label();
			lightGCUnk1NumericUpDown = new NumericUpDown();
			lightGCAmbientPanel = new Panel();
			label10 = new Label();
			lightGCColorPanel = new Panel();
			lightGCZDirTextBox = new TextBox();
			lightGCYDirTextBox = new TextBox();
			lightGCXDirTextBox = new TextBox();
			lightGCSetNumericUpDown = new NumericUpDown();
			label8 = new Label();
			label9 = new Label();
			label12 = new Label();
			label13 = new Label();
			label14 = new Label();
			groupBoxFogData = new GroupBox();
			fogAlphaNumericUpDown = new NumericUpDown();
			label11 = new Label();
			groupBoxFogTable = new GroupBox();
			fogTableRichTextBox = new RichTextBox();
			fogUnkNumericUpDown = new NumericUpDown();
			fogTypeNumericUpDown = new NumericUpDown();
			fogMinDistanceTextBox = new TextBox();
			fogMaxDistanceTextBox = new TextBox();
			label20 = new Label();
			label21 = new Label();
			fogColorPanel = new Panel();
			label19 = new Label();
			label18 = new Label();
			label17 = new Label();
			toolTip1 = new ToolTip(components);
			menuStrip1.SuspendLayout();
			groupBoxLightData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)lightSetNumericUpDown).BeginInit();
			groupBoxGCLightData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)lightGCUnk2NumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)lightGCUnk1NumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)lightGCSetNumericUpDown).BeginInit();
			groupBoxFogData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)fogAlphaNumericUpDown).BeginInit();
			groupBoxFogTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)fogUnkNumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)fogTypeNumericUpDown).BeginInit();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, optionsToolStripMenuItem });
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new Size(845, 24);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, recentFilesToolStripMenuItem, toolStripSeparator1, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			newToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { lightFileToolStripMenuItem, lightFileGCToolStripMenuItem, fogFileToolStripMenuItem, fogFileGCToolStripMenuItem });
			newToolStripMenuItem.Name = "newToolStripMenuItem";
			newToolStripMenuItem.Size = new Size(186, 22);
			newToolStripMenuItem.Text = "New";
			newToolStripMenuItem.Click += newToolStripMenuItem_Click;
			// 
			// lightFileToolStripMenuItem
			// 
			lightFileToolStripMenuItem.Name = "lightFileToolStripMenuItem";
			lightFileToolStripMenuItem.Size = new Size(149, 22);
			lightFileToolStripMenuItem.Text = "Light File";
			lightFileToolStripMenuItem.Click += lightFileToolStripMenuItem_Click;
			// 
			// lightFileGCToolStripMenuItem
			// 
			lightFileGCToolStripMenuItem.Name = "lightFileGCToolStripMenuItem";
			lightFileGCToolStripMenuItem.Size = new Size(149, 22);
			lightFileGCToolStripMenuItem.Text = "Light File (GC)";
			lightFileGCToolStripMenuItem.Click += lightFileGCToolStripMenuItem_Click;
			// 
			// fogFileToolStripMenuItem
			// 
			fogFileToolStripMenuItem.Name = "fogFileToolStripMenuItem";
			fogFileToolStripMenuItem.Size = new Size(149, 22);
			fogFileToolStripMenuItem.Text = "Fog File";
			fogFileToolStripMenuItem.Click += fogFileToolStripMenuItem_Click;
			// 
			// fogFileGCToolStripMenuItem
			// 
			fogFileGCToolStripMenuItem.Name = "fogFileGCToolStripMenuItem";
			fogFileGCToolStripMenuItem.Size = new Size(149, 22);
			fogFileGCToolStripMenuItem.Text = "Fog File (GC)";
			fogFileGCToolStripMenuItem.Click += fogFileGCToolStripMenuItem_Click;
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
			openToolStripMenuItem.Size = new Size(186, 22);
			openToolStripMenuItem.Text = "Open";
			openToolStripMenuItem.Click += openToolStripMenuItem_Click;
			// 
			// recentFilesToolStripMenuItem
			// 
			recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
			recentFilesToolStripMenuItem.Size = new Size(186, 22);
			recentFilesToolStripMenuItem.Text = "Recent Files";
			recentFilesToolStripMenuItem.DropDownItemClicked += recentFilesToolStripMenuItem_DropDownItemClicked;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(183, 6);
			// 
			// saveToolStripMenuItem
			// 
			saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
			saveToolStripMenuItem.Size = new Size(186, 22);
			saveToolStripMenuItem.Text = "Save";
			saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
			// 
			// saveAsToolStripMenuItem
			// 
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
			saveAsToolStripMenuItem.Size = new Size(186, 22);
			saveAsToolStripMenuItem.Text = "Save As";
			saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new Size(183, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new Size(186, 22);
			exitToolStripMenuItem.Text = "Exit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// optionsToolStripMenuItem
			// 
			optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { bigEndianToolStripMenuItem });
			optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			optionsToolStripMenuItem.Size = new Size(61, 20);
			optionsToolStripMenuItem.Text = "Options";
			// 
			// bigEndianToolStripMenuItem
			// 
			bigEndianToolStripMenuItem.Name = "bigEndianToolStripMenuItem";
			bigEndianToolStripMenuItem.Size = new Size(130, 22);
			bigEndianToolStripMenuItem.Text = "Big Endian";
			bigEndianToolStripMenuItem.Click += bigEndianToolStripMenuItem_Click;
			// 
			// groupBoxLightData
			// 
			groupBoxLightData.Controls.Add(lightColorPanel);
			groupBoxLightData.Controls.Add(lightAmbientMultiTextBox);
			groupBoxLightData.Controls.Add(lightColorMultiTextBox);
			groupBoxLightData.Controls.Add(lightZDirTextBox);
			groupBoxLightData.Controls.Add(lightYDirTextBox);
			groupBoxLightData.Controls.Add(lightXDirTextBox);
			groupBoxLightData.Controls.Add(lightSetNumericUpDown);
			groupBoxLightData.Controls.Add(label7);
			groupBoxLightData.Controls.Add(label6);
			groupBoxLightData.Controls.Add(label5);
			groupBoxLightData.Controls.Add(label4);
			groupBoxLightData.Controls.Add(label3);
			groupBoxLightData.Controls.Add(label2);
			groupBoxLightData.Controls.Add(label1);
			groupBoxLightData.Location = new Point(12, 27);
			groupBoxLightData.Name = "groupBoxLightData";
			groupBoxLightData.Size = new Size(245, 245);
			groupBoxLightData.TabIndex = 1;
			groupBoxLightData.TabStop = false;
			groupBoxLightData.Text = "Light File Data";
			// 
			// lightColorPanel
			// 
			lightColorPanel.BorderStyle = BorderStyle.FixedSingle;
			lightColorPanel.Location = new Point(55, 194);
			lightColorPanel.Name = "lightColorPanel";
			lightColorPanel.Size = new Size(50, 35);
			lightColorPanel.TabIndex = 13;
			lightColorPanel.Click += lightColorPanel_Click;
			// 
			// lightAmbientMultiTextBox
			// 
			lightAmbientMultiTextBox.Location = new Point(125, 162);
			lightAmbientMultiTextBox.Name = "lightAmbientMultiTextBox";
			lightAmbientMultiTextBox.Size = new Size(100, 23);
			lightAmbientMultiTextBox.TabIndex = 12;
			lightAmbientMultiTextBox.Text = "0";
			toolTip1.SetToolTip(lightAmbientMultiTextBox, "This value is used to calculate the ambient light based on the assigned color.");
			lightAmbientMultiTextBox.TextChanged += lightAmbientMultiTextBox_TextChanged;
			// 
			// lightColorMultiTextBox
			// 
			lightColorMultiTextBox.Location = new Point(109, 136);
			lightColorMultiTextBox.Name = "lightColorMultiTextBox";
			lightColorMultiTextBox.Size = new Size(100, 23);
			lightColorMultiTextBox.TabIndex = 11;
			lightColorMultiTextBox.Text = "0";
			toolTip1.SetToolTip(lightColorMultiTextBox, "This value determines the strength of the specified color.");
			lightColorMultiTextBox.TextChanged += lightColorMultiTextBox_TextChanged;
			// 
			// lightZDirTextBox
			// 
			lightZDirTextBox.Location = new Point(80, 108);
			lightZDirTextBox.Name = "lightZDirTextBox";
			lightZDirTextBox.Size = new Size(100, 23);
			lightZDirTextBox.TabIndex = 10;
			lightZDirTextBox.Text = "0";
			lightZDirTextBox.TextChanged += lightZDirTextBox_TextChanged;
			// 
			// lightYDirTextBox
			// 
			lightYDirTextBox.Location = new Point(80, 80);
			lightYDirTextBox.Name = "lightYDirTextBox";
			lightYDirTextBox.Size = new Size(100, 23);
			lightYDirTextBox.TabIndex = 9;
			lightYDirTextBox.Text = "0";
			lightYDirTextBox.TextChanged += lightYDirTextBox_TextChanged;
			// 
			// lightXDirTextBox
			// 
			lightXDirTextBox.Location = new Point(80, 52);
			lightXDirTextBox.Name = "lightXDirTextBox";
			lightXDirTextBox.Size = new Size(100, 23);
			lightXDirTextBox.TabIndex = 8;
			lightXDirTextBox.Text = "0";
			lightXDirTextBox.TextChanged += lightXDirTextBox_TextChanged;
			// 
			// lightSetNumericUpDown
			// 
			lightSetNumericUpDown.Location = new Point(72, 24);
			lightSetNumericUpDown.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
			lightSetNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			lightSetNumericUpDown.Name = "lightSetNumericUpDown";
			lightSetNumericUpDown.Size = new Size(53, 23);
			lightSetNumericUpDown.TabIndex = 7;
			toolTip1.SetToolTip(lightSetNumericUpDown, "Light set usage depends on the stage. Generally, the first set is for characters and the third set is for most objects.");
			lightSetNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			lightSetNumericUpDown.ValueChanged += lightSetNumericUpDown_ValueChanged;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new Point(10, 26);
			label7.Name = "label7";
			label7.Size = new Size(56, 15);
			label7.TabIndex = 6;
			label7.Text = "Light Set:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new Point(10, 202);
			label6.Name = "label6";
			label6.Size = new Size(39, 15);
			label6.TabIndex = 5;
			label6.Text = "Color:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new Point(10, 166);
			label5.Name = "label5";
			label5.Size = new Size(110, 15);
			label5.TabIndex = 4;
			label5.Text = "Ambient Multiplier:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(10, 138);
			label4.Name = "label4";
			label4.Size = new Size(93, 15);
			label4.TabIndex = 3;
			label4.Text = "Color Multiplier:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(10, 110);
			label3.Name = "label3";
			label3.Size = new Size(68, 15);
			label3.TabIndex = 2;
			label3.Text = "Z Direction:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(10, 82);
			label2.Name = "label2";
			label2.Size = new Size(68, 15);
			label2.TabIndex = 1;
			label2.Text = "Y Direction:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(10, 54);
			label1.Name = "label1";
			label1.Size = new Size(68, 15);
			label1.TabIndex = 0;
			label1.Text = "X Direction:";
			// 
			// colorDialog
			// 
			colorDialog.AnyColor = true;
			colorDialog.FullOpen = true;
			colorDialog.SolidColorOnly = true;
			// 
			// groupBoxGCLightData
			// 
			groupBoxGCLightData.Controls.Add(lightGCOverrideCheckBox);
			groupBoxGCLightData.Controls.Add(label16);
			groupBoxGCLightData.Controls.Add(lightGCUnk2NumericUpDown);
			groupBoxGCLightData.Controls.Add(label15);
			groupBoxGCLightData.Controls.Add(lightGCUnk1NumericUpDown);
			groupBoxGCLightData.Controls.Add(lightGCAmbientPanel);
			groupBoxGCLightData.Controls.Add(label10);
			groupBoxGCLightData.Controls.Add(lightGCColorPanel);
			groupBoxGCLightData.Controls.Add(lightGCZDirTextBox);
			groupBoxGCLightData.Controls.Add(lightGCYDirTextBox);
			groupBoxGCLightData.Controls.Add(lightGCXDirTextBox);
			groupBoxGCLightData.Controls.Add(lightGCSetNumericUpDown);
			groupBoxGCLightData.Controls.Add(label8);
			groupBoxGCLightData.Controls.Add(label9);
			groupBoxGCLightData.Controls.Add(label12);
			groupBoxGCLightData.Controls.Add(label13);
			groupBoxGCLightData.Controls.Add(label14);
			groupBoxGCLightData.Enabled = false;
			groupBoxGCLightData.Location = new Point(263, 27);
			groupBoxGCLightData.Name = "groupBoxGCLightData";
			groupBoxGCLightData.Size = new Size(279, 245);
			groupBoxGCLightData.TabIndex = 14;
			groupBoxGCLightData.TabStop = false;
			groupBoxGCLightData.Text = "GC Light File Data";
			groupBoxGCLightData.Visible = false;
			// 
			// lightGCOverrideCheckBox
			// 
			lightGCOverrideCheckBox.AutoSize = true;
			lightGCOverrideCheckBox.Location = new Point(151, 26);
			lightGCOverrideCheckBox.Name = "lightGCOverrideCheckBox";
			lightGCOverrideCheckBox.Size = new Size(101, 19);
			lightGCOverrideCheckBox.TabIndex = 8;
			lightGCOverrideCheckBox.Text = "Override Light";
			toolTip1.SetToolTip(lightGCOverrideCheckBox, "If checked, the light settings will override the corresponding light data found in either the default light file or the hardcoded settings.");
			lightGCOverrideCheckBox.UseVisualStyleBackColor = true;
			lightGCOverrideCheckBox.CheckedChanged += lightGCOverrideCheckBox_CheckedChanged;
			// 
			// label16
			// 
			label16.AutoSize = true;
			label16.Location = new Point(6, 214);
			label16.Name = "label16";
			label16.Size = new Size(70, 15);
			label16.TabIndex = 21;
			label16.Text = "Unknown 2:";
			// 
			// lightGCUnk2NumericUpDown
			// 
			lightGCUnk2NumericUpDown.Location = new Point(81, 212);
			lightGCUnk2NumericUpDown.Maximum = new decimal(new int[] { -1294967296, 0, 0, 0 });
			lightGCUnk2NumericUpDown.Minimum = new decimal(new int[] { -1294967296, 0, 0, int.MinValue });
			lightGCUnk2NumericUpDown.Name = "lightGCUnk2NumericUpDown";
			lightGCUnk2NumericUpDown.Size = new Size(72, 23);
			lightGCUnk2NumericUpDown.TabIndex = 20;
			toolTip1.SetToolTip(lightGCUnk2NumericUpDown, "Seemingly does nothing.");
			lightGCUnk2NumericUpDown.ValueChanged += lightGCUnk2NumericUpDown_ValueChanged;
			// 
			// label15
			// 
			label15.AutoSize = true;
			label15.Location = new Point(6, 184);
			label15.Name = "label15";
			label15.Size = new Size(70, 15);
			label15.TabIndex = 19;
			label15.Text = "Unknown 1:";
			// 
			// lightGCUnk1NumericUpDown
			// 
			lightGCUnk1NumericUpDown.Location = new Point(81, 182);
			lightGCUnk1NumericUpDown.Maximum = new decimal(new int[] { 30000000, 0, 0, 0 });
			lightGCUnk1NumericUpDown.Minimum = new decimal(new int[] { 30000000, 0, 0, int.MinValue });
			lightGCUnk1NumericUpDown.Name = "lightGCUnk1NumericUpDown";
			lightGCUnk1NumericUpDown.Size = new Size(72, 23);
			lightGCUnk1NumericUpDown.TabIndex = 18;
			toolTip1.SetToolTip(lightGCUnk1NumericUpDown, "Seemingly does nothing.");
			lightGCUnk1NumericUpDown.ValueChanged += lightGCUnk1NumericUpDown_ValueChanged;
			// 
			// lightGCAmbientPanel
			// 
			lightGCAmbientPanel.BorderStyle = BorderStyle.FixedSingle;
			lightGCAmbientPanel.Location = new Point(202, 138);
			lightGCAmbientPanel.Name = "lightGCAmbientPanel";
			lightGCAmbientPanel.Size = new Size(50, 35);
			lightGCAmbientPanel.TabIndex = 15;
			lightGCAmbientPanel.Click += lightGCAmbientPanel_Click;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new Point(110, 146);
			label10.Name = "label10";
			label10.Size = new Size(88, 15);
			label10.TabIndex = 14;
			label10.Text = "Ambient Color:";
			// 
			// lightGCColorPanel
			// 
			lightGCColorPanel.BorderStyle = BorderStyle.FixedSingle;
			lightGCColorPanel.Location = new Point(55, 138);
			lightGCColorPanel.Name = "lightGCColorPanel";
			lightGCColorPanel.Size = new Size(50, 35);
			lightGCColorPanel.TabIndex = 13;
			lightGCColorPanel.Click += lightGCColorPanel_Click;
			// 
			// lightGCZDirTextBox
			// 
			lightGCZDirTextBox.Location = new Point(80, 108);
			lightGCZDirTextBox.Name = "lightGCZDirTextBox";
			lightGCZDirTextBox.Size = new Size(100, 23);
			lightGCZDirTextBox.TabIndex = 11;
			lightGCZDirTextBox.Text = "0";
			lightGCZDirTextBox.TextChanged += lightGCZDirTextBox_TextChanged;
			// 
			// lightGCYDirTextBox
			// 
			lightGCYDirTextBox.Location = new Point(80, 80);
			lightGCYDirTextBox.Name = "lightGCYDirTextBox";
			lightGCYDirTextBox.Size = new Size(100, 23);
			lightGCYDirTextBox.TabIndex = 10;
			lightGCYDirTextBox.Text = "0";
			lightGCYDirTextBox.TextChanged += lightGCYDirTextBox_TextChanged;
			// 
			// lightGCXDirTextBox
			// 
			lightGCXDirTextBox.Location = new Point(80, 52);
			lightGCXDirTextBox.Name = "lightGCXDirTextBox";
			lightGCXDirTextBox.Size = new Size(100, 23);
			lightGCXDirTextBox.TabIndex = 9;
			lightGCXDirTextBox.Text = "0";
			lightGCXDirTextBox.TextChanged += lightGCXDirTextBox_TextChanged;
			// 
			// lightGCSetNumericUpDown
			// 
			lightGCSetNumericUpDown.Location = new Point(72, 24);
			lightGCSetNumericUpDown.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
			lightGCSetNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			lightGCSetNumericUpDown.Name = "lightGCSetNumericUpDown";
			lightGCSetNumericUpDown.Size = new Size(53, 23);
			lightGCSetNumericUpDown.TabIndex = 7;
			toolTip1.SetToolTip(lightGCSetNumericUpDown, "Light set usage depends on the stage. Generally, the first set is for characters and the third set is for most objects.");
			lightGCSetNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			lightGCSetNumericUpDown.ValueChanged += lightGCSetNumericUpDown_ValueChanged;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new Point(10, 26);
			label8.Name = "label8";
			label8.Size = new Size(56, 15);
			label8.TabIndex = 6;
			label8.Text = "Light Set:";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new Point(10, 146);
			label9.Name = "label9";
			label9.Size = new Size(39, 15);
			label9.TabIndex = 5;
			label9.Text = "Color:";
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.Location = new Point(10, 110);
			label12.Name = "label12";
			label12.Size = new Size(68, 15);
			label12.TabIndex = 2;
			label12.Text = "Z Direction:";
			// 
			// label13
			// 
			label13.AutoSize = true;
			label13.Location = new Point(10, 82);
			label13.Name = "label13";
			label13.Size = new Size(68, 15);
			label13.TabIndex = 1;
			label13.Text = "Y Direction:";
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.Location = new Point(10, 54);
			label14.Name = "label14";
			label14.Size = new Size(68, 15);
			label14.TabIndex = 0;
			label14.Text = "X Direction:";
			// 
			// groupBoxFogData
			// 
			groupBoxFogData.Controls.Add(fogAlphaNumericUpDown);
			groupBoxFogData.Controls.Add(label11);
			groupBoxFogData.Controls.Add(groupBoxFogTable);
			groupBoxFogData.Controls.Add(fogUnkNumericUpDown);
			groupBoxFogData.Controls.Add(fogTypeNumericUpDown);
			groupBoxFogData.Controls.Add(fogMinDistanceTextBox);
			groupBoxFogData.Controls.Add(fogMaxDistanceTextBox);
			groupBoxFogData.Controls.Add(label20);
			groupBoxFogData.Controls.Add(label21);
			groupBoxFogData.Controls.Add(fogColorPanel);
			groupBoxFogData.Controls.Add(label19);
			groupBoxFogData.Controls.Add(label18);
			groupBoxFogData.Controls.Add(label17);
			groupBoxFogData.Enabled = false;
			groupBoxFogData.Location = new Point(553, 27);
			groupBoxFogData.Name = "groupBoxFogData";
			groupBoxFogData.Size = new Size(279, 347);
			groupBoxFogData.TabIndex = 15;
			groupBoxFogData.TabStop = false;
			groupBoxFogData.Text = "Fog File Data";
			groupBoxFogData.Visible = false;
			// 
			// fogAlphaNumericUpDown
			// 
			fogAlphaNumericUpDown.Location = new Point(183, 58);
			fogAlphaNumericUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
			fogAlphaNumericUpDown.Name = "fogAlphaNumericUpDown";
			fogAlphaNumericUpDown.Size = new Size(54, 23);
			fogAlphaNumericUpDown.TabIndex = 19;
			toolTip1.SetToolTip(fogAlphaNumericUpDown, "Alpha values are ignored in fog files.");
			fogAlphaNumericUpDown.ValueChanged += fogAlphaNumericUpDown_ValueChanged;
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Location = new Point(136, 60);
			label11.Name = "label11";
			label11.Size = new Size(41, 15);
			label11.TabIndex = 23;
			label11.Text = "Alpha:";
			// 
			// groupBoxFogTable
			// 
			groupBoxFogTable.Controls.Add(fogTableRichTextBox);
			groupBoxFogTable.Location = new Point(11, 154);
			groupBoxFogTable.Name = "groupBoxFogTable";
			groupBoxFogTable.Size = new Size(249, 187);
			groupBoxFogTable.TabIndex = 22;
			groupBoxFogTable.TabStop = false;
			groupBoxFogTable.Text = "Fog Table";
			// 
			// fogTableRichTextBox
			// 
			fogTableRichTextBox.BorderStyle = BorderStyle.FixedSingle;
			fogTableRichTextBox.Location = new Point(10, 22);
			fogTableRichTextBox.Name = "fogTableRichTextBox";
			fogTableRichTextBox.Size = new Size(228, 152);
			fogTableRichTextBox.TabIndex = 0;
			fogTableRichTextBox.Text = "";
			toolTip1.SetToolTip(fogTableRichTextBox, "These values do nothing in SA2B. Dreamcast uses this information to determine how the fog should look. Data found within a Big Endian file has been fixed for readability.");
			fogTableRichTextBox.TextChanged += fogTableRichTextBox_TextChanged;
			// 
			// fogUnkNumericUpDown
			// 
			fogUnkNumericUpDown.Location = new Point(203, 24);
			fogUnkNumericUpDown.Maximum = new decimal(new int[] { 30000000, 0, 0, 0 });
			fogUnkNumericUpDown.Minimum = new decimal(new int[] { 30000000, 0, 0, int.MinValue });
			fogUnkNumericUpDown.Name = "fogUnkNumericUpDown";
			fogUnkNumericUpDown.Size = new Size(59, 23);
			fogUnkNumericUpDown.TabIndex = 17;
			fogUnkNumericUpDown.ValueChanged += fogUnkNumericUpDown_ValueChanged;
			// 
			// fogTypeNumericUpDown
			// 
			fogTypeNumericUpDown.Location = new Point(52, 24);
			fogTypeNumericUpDown.Maximum = new decimal(new int[] { 30000000, 0, 0, 0 });
			fogTypeNumericUpDown.Minimum = new decimal(new int[] { 30000000, 0, 0, int.MinValue });
			fogTypeNumericUpDown.Name = "fogTypeNumericUpDown";
			fogTypeNumericUpDown.Size = new Size(54, 23);
			fogTypeNumericUpDown.TabIndex = 16;
			toolTip1.SetToolTip(fogTypeNumericUpDown, "Dreamcast and GameCube/PC fog files have different type definitions. DC uses 0 and 1, while GC/PC tends to use 2 and 3.");
			fogTypeNumericUpDown.ValueChanged += fogTypeNumericUpDown_ValueChanged;
			// 
			// fogMinDistanceTextBox
			// 
			fogMinDistanceTextBox.Location = new Point(126, 120);
			fogMinDistanceTextBox.Name = "fogMinDistanceTextBox";
			fogMinDistanceTextBox.Size = new Size(100, 23);
			fogMinDistanceTextBox.TabIndex = 21;
			fogMinDistanceTextBox.Text = "0";
			fogMinDistanceTextBox.TextChanged += fogMinDistanceTextBox_TextChanged;
			// 
			// fogMaxDistanceTextBox
			// 
			fogMaxDistanceTextBox.Location = new Point(126, 92);
			fogMaxDistanceTextBox.Name = "fogMaxDistanceTextBox";
			fogMaxDistanceTextBox.Size = new Size(100, 23);
			fogMaxDistanceTextBox.TabIndex = 20;
			fogMaxDistanceTextBox.Text = "0";
			toolTip1.SetToolTip(fogMaxDistanceTextBox, "For DC fog files, this can also be used as a density setting.");
			fogMaxDistanceTextBox.TextChanged += fogMaxDistanceTextBox_TextChanged;
			// 
			// label20
			// 
			label20.AutoSize = true;
			label20.Location = new Point(11, 122);
			label20.Name = "label20";
			label20.Size = new Size(111, 15);
			label20.TabIndex = 17;
			label20.Text = "Minimum Distance:";
			// 
			// label21
			// 
			label21.AutoSize = true;
			label21.Location = new Point(11, 94);
			label21.Name = "label21";
			label21.Size = new Size(112, 15);
			label21.TabIndex = 16;
			label21.Text = "Maximum Distance:";
			// 
			// fogColorPanel
			// 
			fogColorPanel.BorderStyle = BorderStyle.FixedSingle;
			fogColorPanel.Location = new Point(56, 52);
			fogColorPanel.Name = "fogColorPanel";
			fogColorPanel.Size = new Size(50, 35);
			fogColorPanel.TabIndex = 18;
			fogColorPanel.Click += fogColorPanel_Click;
			// 
			// label19
			// 
			label19.AutoSize = true;
			label19.Location = new Point(11, 60);
			label19.Name = "label19";
			label19.Size = new Size(39, 15);
			label19.TabIndex = 14;
			label19.Text = "Color:";
			// 
			// label18
			// 
			label18.AutoSize = true;
			label18.Location = new Point(136, 26);
			label18.Name = "label18";
			label18.Size = new Size(61, 15);
			label18.TabIndex = 1;
			label18.Text = "Unknown:";
			// 
			// label17
			// 
			label17.AutoSize = true;
			label17.Location = new Point(11, 26);
			label17.Name = "label17";
			label17.Size = new Size(35, 15);
			label17.TabIndex = 0;
			label17.Text = "Type:";
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(845, 386);
			Controls.Add(groupBoxFogData);
			Controls.Add(groupBoxGCLightData);
			Controls.Add(groupBoxLightData);
			Controls.Add(menuStrip1);
			MainMenuStrip = menuStrip1;
			Name = "MainForm";
			Text = "SA2 Light/Fog Editor";
			FormClosing += MainForm_FormClosing;
			Load += MainForm_Load;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			groupBoxLightData.ResumeLayout(false);
			groupBoxLightData.PerformLayout();
			((System.ComponentModel.ISupportInitialize)lightSetNumericUpDown).EndInit();
			groupBoxGCLightData.ResumeLayout(false);
			groupBoxGCLightData.PerformLayout();
			((System.ComponentModel.ISupportInitialize)lightGCUnk2NumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)lightGCUnk1NumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)lightGCSetNumericUpDown).EndInit();
			groupBoxFogData.ResumeLayout(false);
			groupBoxFogData.PerformLayout();
			((System.ComponentModel.ISupportInitialize)fogAlphaNumericUpDown).EndInit();
			groupBoxFogTable.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)fogUnkNumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)fogTypeNumericUpDown).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menuStrip1;
		private ToolStripMenuItem fileToolStripMenuItem;
		private GroupBox groupBoxLightData;
		private Label label2;
		private Label label1;
		private NumericUpDown lightSetNumericUpDown;
		private Label label7;
		private Label label6;
		private Label label5;
		private Label label4;
		private Label label3;
		private TextBox lightYDirTextBox;
		private TextBox lightXDirTextBox;
		private Panel lightColorPanel;
		private TextBox lightAmbientMultiTextBox;
		private TextBox lightColorMultiTextBox;
		private TextBox lightZDirTextBox;
		private ColorDialog colorDialog;
		private GroupBox groupBoxGCLightData;
		private Panel lightGCAmbientPanel;
		private Label label10;
		private Panel lightGCColorPanel;
		private TextBox lightGCZDirTextBox;
		private TextBox lightGCYDirTextBox;
		private TextBox lightGCXDirTextBox;
		private NumericUpDown lightGCSetNumericUpDown;
		private Label label8;
		private Label label9;
		private Label label12;
		private Label label13;
		private Label label14;
		private Label label16;
		private NumericUpDown lightGCUnk2NumericUpDown;
		private Label label15;
		private NumericUpDown lightGCUnk1NumericUpDown;
		private GroupBox groupBoxFogData;
		private Label label18;
		private Label label17;
		private GroupBox groupBoxFogTable;
		private RichTextBox fogTableRichTextBox;
		private NumericUpDown fogUnkNumericUpDown;
		private NumericUpDown fogTypeNumericUpDown;
		private TextBox fogMinDistanceTextBox;
		private TextBox fogMaxDistanceTextBox;
		private Label label20;
		private Label label21;
		private Panel fogColorPanel;
		private Label label19;
		private ToolStripMenuItem newToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		private ToolStripMenuItem recentFilesToolStripMenuItem;
		private ToolStripMenuItem lightFileToolStripMenuItem;
		private ToolStripMenuItem lightFileGCToolStripMenuItem;
		private ToolStripMenuItem fogFileToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem saveToolStripMenuItem;
		private ToolStripMenuItem saveAsToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem exitToolStripMenuItem;
		private CheckBox lightGCOverrideCheckBox;
		private ToolTip toolTip1;
		private NumericUpDown fogAlphaNumericUpDown;
		private Label label11;
		private ToolStripMenuItem optionsToolStripMenuItem;
		private ToolStripMenuItem bigEndianToolStripMenuItem;
		private ToolStripMenuItem fogFileGCToolStripMenuItem;
	}
}
