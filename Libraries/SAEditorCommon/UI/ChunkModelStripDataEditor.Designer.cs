using SharpDX.Direct3D9;
using System.Net.NetworkInformation;

namespace SAModel.SAEditorCommon.UI
{
    partial class ChunkModelStripDataEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChunkModelStripDataEditor));
			colorDialog = new System.Windows.Forms.ColorDialog();
			flagsGroupBox = new System.Windows.Forms.GroupBox();
			noAlphaTestCheck = new System.Windows.Forms.CheckBox();
			userFlagsNumericUpDown = new System.Windows.Forms.NumericUpDown();
			label4 = new System.Windows.Forms.Label();
			ignoreLightCheck = new System.Windows.Forms.CheckBox();
			flatShadeCheck = new System.Windows.Forms.CheckBox();
			doubleSideCheck = new System.Windows.Forms.CheckBox();
			envMapCheck = new System.Windows.Forms.CheckBox();
			useAlphaCheck = new System.Windows.Forms.CheckBox();
			ignoreSpecCheck = new System.Windows.Forms.CheckBox();
			ignoreAmbiCheck = new System.Windows.Forms.CheckBox();
			doneButton = new System.Windows.Forms.Button();
			toolTip = new System.Windows.Forms.ToolTip(components);
			resetButton = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			deleteAllUVButton = new System.Windows.Forms.Button();
			deleteSelectedUVButton = new System.Windows.Forms.Button();
			groupBox2 = new System.Windows.Forms.GroupBox();
			resetAllWindingsButton = new System.Windows.Forms.Button();
			flipAllWindingsButton = new System.Windows.Forms.Button();
			resetWindingButton = new System.Windows.Forms.Button();
			flipWindingButton = new System.Windows.Forms.Button();
			stripListView = new System.Windows.Forms.ListView();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			columnHeader2 = new System.Windows.Forms.ColumnHeader();
			columnHeader3 = new System.Windows.Forms.ColumnHeader();
			columnHeader4 = new System.Windows.Forms.ColumnHeader();
			columnHeader5 = new System.Windows.Forms.ColumnHeader();
			stripSetComboBox = new System.Windows.Forms.ComboBox();
			groupBox3 = new System.Windows.Forms.GroupBox();
			userFlag3NumericUpDown = new System.Windows.Forms.NumericUpDown();
			userFlag2NumericUpDown = new System.Windows.Forms.NumericUpDown();
			userFlag1NumericUpDown = new System.Windows.Forms.NumericUpDown();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			flagsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)userFlagsNumericUpDown).BeginInit();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)userFlag3NumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)userFlag2NumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)userFlag1NumericUpDown).BeginInit();
			SuspendLayout();
			// 
			// colorDialog
			// 
			colorDialog.AnyColor = true;
			colorDialog.FullOpen = true;
			colorDialog.SolidColorOnly = true;
			// 
			// flagsGroupBox
			// 
			flagsGroupBox.Controls.Add(noAlphaTestCheck);
			flagsGroupBox.Controls.Add(userFlagsNumericUpDown);
			flagsGroupBox.Controls.Add(label4);
			flagsGroupBox.Controls.Add(ignoreLightCheck);
			flagsGroupBox.Controls.Add(flatShadeCheck);
			flagsGroupBox.Controls.Add(doubleSideCheck);
			flagsGroupBox.Controls.Add(envMapCheck);
			flagsGroupBox.Controls.Add(useAlphaCheck);
			flagsGroupBox.Controls.Add(ignoreSpecCheck);
			flagsGroupBox.Controls.Add(ignoreAmbiCheck);
			flagsGroupBox.Location = new System.Drawing.Point(490, 13);
			flagsGroupBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flagsGroupBox.Name = "flagsGroupBox";
			flagsGroupBox.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flagsGroupBox.Size = new System.Drawing.Size(329, 385);
			flagsGroupBox.TabIndex = 8;
			flagsGroupBox.TabStop = false;
			flagsGroupBox.Text = "Flags";
			// 
			// noAlphaTestCheck
			// 
			noAlphaTestCheck.Location = new System.Drawing.Point(20, 291);
			noAlphaTestCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			noAlphaTestCheck.Name = "noAlphaTestCheck";
			noAlphaTestCheck.Size = new System.Drawing.Size(270, 29);
			noAlphaTestCheck.TabIndex = 16;
			noAlphaTestCheck.Text = "No Alpha Test (SA2B Only)";
			toolTip.SetToolTip(noAlphaTestCheck, "Ignores the Alpha Test setting. This flag does nothing outside of SA2B.");
			noAlphaTestCheck.UseVisualStyleBackColor = true;
			noAlphaTestCheck.Click += noAlphaTestCheck_Click;
			// 
			// userFlagsNumericUpDown
			// 
			userFlagsNumericUpDown.Location = new System.Drawing.Point(123, 328);
			userFlagsNumericUpDown.Name = "userFlagsNumericUpDown";
			userFlagsNumericUpDown.Size = new System.Drawing.Size(64, 31);
			userFlagsNumericUpDown.TabIndex = 15;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(21, 329);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(97, 25);
			label4.TabIndex = 13;
			label4.Text = "User Flags:";
			// 
			// ignoreLightCheck
			// 
			ignoreLightCheck.Location = new System.Drawing.Point(20, 219);
			ignoreLightCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ignoreLightCheck.Name = "ignoreLightCheck";
			ignoreLightCheck.Size = new System.Drawing.Size(232, 29);
			ignoreLightCheck.TabIndex = 12;
			ignoreLightCheck.Text = "Ignore Lighting";
			toolTip.SetToolTip(ignoreLightCheck, "If checked, the mesh will not have any lighting applied.");
			ignoreLightCheck.UseVisualStyleBackColor = true;
			ignoreLightCheck.Click += ignoreLightCheck_Click;
			// 
			// flatShadeCheck
			// 
			flatShadeCheck.Location = new System.Drawing.Point(20, 147);
			flatShadeCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flatShadeCheck.Name = "flatShadeCheck";
			flatShadeCheck.Size = new System.Drawing.Size(215, 29);
			flatShadeCheck.TabIndex = 11;
			flatShadeCheck.Text = "Flat Shaded";
			toolTip.SetToolTip(flatShadeCheck, "If checked, polygon smoothing will be disabled and the model will appear faceted, like a cut gem or die. This flag does nothing in SADX, or SA2B without the Render Fix mod.");
			flatShadeCheck.UseVisualStyleBackColor = true;
			flatShadeCheck.Click += flatShadeCheck_Click;
			// 
			// doubleSideCheck
			// 
			doubleSideCheck.Location = new System.Drawing.Point(20, 111);
			doubleSideCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			doubleSideCheck.Name = "doubleSideCheck";
			doubleSideCheck.Size = new System.Drawing.Size(215, 29);
			doubleSideCheck.TabIndex = 10;
			doubleSideCheck.Text = "Double Sided";
			toolTip.SetToolTip(doubleSideCheck, resources.GetString("doubleSideCheck.ToolTip"));
			doubleSideCheck.UseVisualStyleBackColor = true;
			doubleSideCheck.Click += doubleSideCheck_Click;
			// 
			// envMapCheck
			// 
			envMapCheck.Location = new System.Drawing.Point(20, 75);
			envMapCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			envMapCheck.Name = "envMapCheck";
			envMapCheck.Size = new System.Drawing.Size(260, 29);
			envMapCheck.TabIndex = 9;
			envMapCheck.Text = "Environment Mapping";
			toolTip.SetToolTip(envMapCheck, "If checked, the texture's uv maps will be mapped to the environment and the model will appear 'shiny'.");
			envMapCheck.UseVisualStyleBackColor = true;
			envMapCheck.Click += envMapCheck_Click;
			// 
			// useAlphaCheck
			// 
			useAlphaCheck.Location = new System.Drawing.Point(20, 39);
			useAlphaCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			useAlphaCheck.Name = "useAlphaCheck";
			useAlphaCheck.Size = new System.Drawing.Size(215, 29);
			useAlphaCheck.TabIndex = 7;
			useAlphaCheck.Text = "Use Alpha";
			toolTip.SetToolTip(useAlphaCheck, "If checked, texture transparency will be enabled (and possibly non-texture transparency). ");
			useAlphaCheck.UseVisualStyleBackColor = true;
			useAlphaCheck.Click += useAlphaCheck_Click;
			// 
			// ignoreSpecCheck
			// 
			ignoreSpecCheck.Location = new System.Drawing.Point(20, 255);
			ignoreSpecCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ignoreSpecCheck.Name = "ignoreSpecCheck";
			ignoreSpecCheck.Size = new System.Drawing.Size(215, 29);
			ignoreSpecCheck.TabIndex = 6;
			ignoreSpecCheck.Text = "Ignore Specular";
			toolTip.SetToolTip(ignoreSpecCheck, "Disables specular lighting on the material. This flag does nothing in SADX or SA2B.");
			ignoreSpecCheck.UseVisualStyleBackColor = true;
			ignoreSpecCheck.Click += ignoreSpecCheck_Click;
			// 
			// ignoreAmbiCheck
			// 
			ignoreAmbiCheck.Location = new System.Drawing.Point(20, 182);
			ignoreAmbiCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ignoreAmbiCheck.Name = "ignoreAmbiCheck";
			ignoreAmbiCheck.Size = new System.Drawing.Size(215, 29);
			ignoreAmbiCheck.TabIndex = 0;
			ignoreAmbiCheck.Text = "Ignore Ambient";
			toolTip.SetToolTip(ignoreAmbiCheck, "If checked, the mesh will ignore the ambient light source. In other words, the ambient light will be treated as black, darkening the model. This flag does nothing in SA2B without the Render Fix mod.");
			ignoreAmbiCheck.UseVisualStyleBackColor = true;
			ignoreAmbiCheck.Click += ignoreAmbiCheck_Click;
			// 
			// doneButton
			// 
			doneButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			doneButton.Location = new System.Drawing.Point(687, 540);
			doneButton.Margin = new System.Windows.Forms.Padding(4);
			doneButton.Name = "doneButton";
			doneButton.Size = new System.Drawing.Size(132, 40);
			doneButton.TabIndex = 0;
			doneButton.Text = "Done";
			doneButton.UseVisualStyleBackColor = true;
			doneButton.Click += doneButton_Click;
			// 
			// toolTip
			// 
			toolTip.AutoPopDelay = 30000;
			toolTip.InitialDelay = 500;
			toolTip.ReshowDelay = 100;
			// 
			// resetButton
			// 
			resetButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			resetButton.Location = new System.Drawing.Point(528, 540);
			resetButton.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(132, 40);
			resetButton.TabIndex = 6;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the poly data to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(deleteAllUVButton);
			groupBox1.Controls.Add(deleteSelectedUVButton);
			groupBox1.Controls.Add(groupBox2);
			groupBox1.Controls.Add(stripListView);
			groupBox1.Controls.Add(stripSetComboBox);
			groupBox1.Location = new System.Drawing.Point(20, 13);
			groupBox1.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
			groupBox1.Size = new System.Drawing.Size(454, 567);
			groupBox1.TabIndex = 9;
			groupBox1.TabStop = false;
			groupBox1.Text = "Strip Sets";
			// 
			// deleteAllUVButton
			// 
			deleteAllUVButton.Location = new System.Drawing.Point(237, 383);
			deleteAllUVButton.Name = "deleteAllUVButton";
			deleteAllUVButton.Size = new System.Drawing.Size(186, 34);
			deleteAllUVButton.TabIndex = 6;
			deleteAllUVButton.Text = "Delete All UV Data";
			deleteAllUVButton.UseVisualStyleBackColor = true;
			deleteAllUVButton.Click += deleteAllUVButton_Click;
			// 
			// deleteSelectedUVButton
			// 
			deleteSelectedUVButton.Location = new System.Drawing.Point(36, 383);
			deleteSelectedUVButton.Name = "deleteSelectedUVButton";
			deleteSelectedUVButton.Size = new System.Drawing.Size(179, 34);
			deleteSelectedUVButton.TabIndex = 5;
			deleteSelectedUVButton.Text = "Delete Selected UV";
			deleteSelectedUVButton.UseVisualStyleBackColor = true;
			deleteSelectedUVButton.Click += deleteSelectedUVButton_Click;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(resetAllWindingsButton);
			groupBox2.Controls.Add(flipAllWindingsButton);
			groupBox2.Controls.Add(resetWindingButton);
			groupBox2.Controls.Add(flipWindingButton);
			groupBox2.Location = new System.Drawing.Point(30, 425);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(403, 122);
			groupBox2.TabIndex = 4;
			groupBox2.TabStop = false;
			groupBox2.Text = "Windings";
			// 
			// resetAllWindingsButton
			// 
			resetAllWindingsButton.Location = new System.Drawing.Point(217, 70);
			resetAllWindingsButton.Name = "resetAllWindingsButton";
			resetAllWindingsButton.Size = new System.Drawing.Size(137, 34);
			resetAllWindingsButton.TabIndex = 5;
			resetAllWindingsButton.Text = "Reset All";
			resetAllWindingsButton.UseVisualStyleBackColor = true;
			resetAllWindingsButton.Click += resetAllWindingsButton_Click;
			// 
			// flipAllWindingsButton
			// 
			flipAllWindingsButton.Location = new System.Drawing.Point(217, 30);
			flipAllWindingsButton.Name = "flipAllWindingsButton";
			flipAllWindingsButton.Size = new System.Drawing.Size(137, 34);
			flipAllWindingsButton.TabIndex = 4;
			flipAllWindingsButton.Text = "Flip All";
			flipAllWindingsButton.UseVisualStyleBackColor = true;
			flipAllWindingsButton.Click += flipAllWindingsButton_Click;
			// 
			// resetWindingButton
			// 
			resetWindingButton.Location = new System.Drawing.Point(40, 70);
			resetWindingButton.Name = "resetWindingButton";
			resetWindingButton.Size = new System.Drawing.Size(137, 34);
			resetWindingButton.TabIndex = 3;
			resetWindingButton.Text = "Reset Selected";
			resetWindingButton.UseVisualStyleBackColor = true;
			resetWindingButton.Click += resetWindingButton_Click;
			// 
			// flipWindingButton
			// 
			flipWindingButton.Location = new System.Drawing.Point(40, 30);
			flipWindingButton.Name = "flipWindingButton";
			flipWindingButton.Size = new System.Drawing.Size(137, 34);
			flipWindingButton.TabIndex = 2;
			flipWindingButton.Text = "Flip Selected";
			flipWindingButton.UseVisualStyleBackColor = true;
			flipWindingButton.Click += flipWindingButton_Click;
			// 
			// stripListView
			// 
			stripListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5 });
			stripListView.GridLines = true;
			stripListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			stripListView.Location = new System.Drawing.Point(30, 97);
			stripListView.MultiSelect = false;
			stripListView.Name = "stripListView";
			stripListView.Size = new System.Drawing.Size(403, 269);
			stripListView.TabIndex = 1;
			stripListView.UseCompatibleStateImageBehavior = false;
			stripListView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			columnHeader1.Text = "ID";
			// 
			// columnHeader2
			// 
			columnHeader2.Text = "Index";
			// 
			// columnHeader3
			// 
			columnHeader3.Text = "UV";
			// 
			// columnHeader4
			// 
			columnHeader4.Text = "UV2";
			// 
			// columnHeader5
			// 
			columnHeader5.Text = "Color";
			// 
			// stripSetComboBox
			// 
			stripSetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			stripSetComboBox.FormattingEnabled = true;
			stripSetComboBox.Location = new System.Drawing.Point(30, 39);
			stripSetComboBox.Name = "stripSetComboBox";
			stripSetComboBox.Size = new System.Drawing.Size(403, 33);
			stripSetComboBox.TabIndex = 0;
			stripSetComboBox.SelectedIndexChanged += stripSetComboBox_SelectedIndexChanged;
			// 
			// groupBox3
			// 
			groupBox3.Controls.Add(userFlag3NumericUpDown);
			groupBox3.Controls.Add(userFlag2NumericUpDown);
			groupBox3.Controls.Add(userFlag1NumericUpDown);
			groupBox3.Controls.Add(label3);
			groupBox3.Controls.Add(label2);
			groupBox3.Controls.Add(label1);
			groupBox3.Location = new System.Drawing.Point(491, 405);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new System.Drawing.Size(328, 126);
			groupBox3.TabIndex = 10;
			groupBox3.TabStop = false;
			groupBox3.Text = "User Flags (Strip)";
			// 
			// userFlag3NumericUpDown
			// 
			userFlag3NumericUpDown.Location = new System.Drawing.Point(95, 75);
			userFlag3NumericUpDown.Name = "userFlag3NumericUpDown";
			userFlag3NumericUpDown.Size = new System.Drawing.Size(60, 31);
			userFlag3NumericUpDown.TabIndex = 8;
			// 
			// userFlag2NumericUpDown
			// 
			userFlag2NumericUpDown.Location = new System.Drawing.Point(233, 37);
			userFlag2NumericUpDown.Name = "userFlag2NumericUpDown";
			userFlag2NumericUpDown.Size = new System.Drawing.Size(60, 31);
			userFlag2NumericUpDown.TabIndex = 7;
			// 
			// userFlag1NumericUpDown
			// 
			userFlag1NumericUpDown.Location = new System.Drawing.Point(95, 37);
			userFlag1NumericUpDown.Name = "userFlag1NumericUpDown";
			userFlag1NumericUpDown.Size = new System.Drawing.Size(60, 31);
			userFlag1NumericUpDown.TabIndex = 6;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(23, 75);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(64, 25);
			label3.TabIndex = 2;
			label3.Text = "Flag 3:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(161, 37);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(64, 25);
			label2.TabIndex = 1;
			label2.Text = "Flag 2:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(23, 37);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(64, 25);
			label1.TabIndex = 0;
			label1.Text = "Flag 1:";
			// 
			// ChunkModelStripDataEditor
			// 
			AcceptButton = doneButton;
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(834, 601);
			ControlBox = false;
			Controls.Add(groupBox3);
			Controls.Add(groupBox1);
			Controls.Add(resetButton);
			Controls.Add(doneButton);
			Controls.Add(flagsGroupBox);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkModelStripDataEditor";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Strip Data Editor";
			Load += MaterialEditor_Load;
			flagsGroupBox.ResumeLayout(false);
			flagsGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)userFlagsNumericUpDown).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox2.ResumeLayout(false);
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)userFlag3NumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)userFlag2NumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)userFlag1NumericUpDown).EndInit();
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.GroupBox flagsGroupBox;
        private System.Windows.Forms.CheckBox ignoreAmbiCheck;
        private System.Windows.Forms.CheckBox useAlphaCheck;
        private System.Windows.Forms.CheckBox ignoreSpecCheck;
        private System.Windows.Forms.CheckBox doubleSideCheck;
        private System.Windows.Forms.CheckBox envMapCheck;
        private System.Windows.Forms.CheckBox ignoreLightCheck;
        private System.Windows.Forms.CheckBox flatShadeCheck;
        private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox stripSetComboBox;
		private System.Windows.Forms.ListView stripListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button flipWindingButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button resetAllWindingsButton;
		private System.Windows.Forms.Button flipAllWindingsButton;
		private System.Windows.Forms.Button resetWindingButton;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown userFlag3NumericUpDown;
		private System.Windows.Forms.NumericUpDown userFlag2NumericUpDown;
		private System.Windows.Forms.NumericUpDown userFlag1NumericUpDown;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown userFlagsNumericUpDown;
		private System.Windows.Forms.CheckBox noAlphaTestCheck;
		private System.Windows.Forms.Button deleteAllUVButton;
		private System.Windows.Forms.Button deleteSelectedUVButton;
	}
}