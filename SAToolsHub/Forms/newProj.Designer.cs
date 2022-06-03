namespace SAToolsHub
{
	partial class newProj
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
            this.checkBoxSaveDifferentFolder = new System.Windows.Forms.CheckBox();
            this.buttonCreateProject = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxProjFolder = new System.Windows.Forms.TextBox();
            this.labelTemplate = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.comboBoxTemplate = new System.Windows.Forms.ComboBox();
            this.labelPlatform = new System.Windows.Forms.Label();
            this.comboBoxPlatform = new System.Windows.Forms.ComboBox();
            this.labelBaseGame = new System.Windows.Forms.Label();
            this.radioButtonSA1 = new System.Windows.Forms.RadioButton();
            this.radioButtonSA2 = new System.Windows.Forms.RadioButton();
            this.groupBoxAdvancedOptions = new System.Windows.Forms.GroupBox();
            this.comboBoxLabels = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxAdvanced = new System.Windows.Forms.CheckBox();
            this.groupBoxAdvancedOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxSaveDifferentFolder
            // 
            this.checkBoxSaveDifferentFolder.AutoSize = true;
            this.checkBoxSaveDifferentFolder.Location = new System.Drawing.Point(21, 82);
            this.checkBoxSaveDifferentFolder.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.checkBoxSaveDifferentFolder.Name = "checkBoxSaveDifferentFolder";
            this.checkBoxSaveDifferentFolder.Size = new System.Drawing.Size(342, 29);
            this.checkBoxSaveDifferentFolder.TabIndex = 10;
            this.checkBoxSaveDifferentFolder.Text = "Save Project Files to a Different Folder:";
            this.checkBoxSaveDifferentFolder.UseVisualStyleBackColor = true;
            this.checkBoxSaveDifferentFolder.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // buttonCreateProject
            // 
            this.buttonCreateProject.Location = new System.Drawing.Point(21, 160);
            this.buttonCreateProject.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonCreateProject.Name = "buttonCreateProject";
            this.buttonCreateProject.Size = new System.Drawing.Size(600, 44);
            this.buttonCreateProject.TabIndex = 8;
            this.buttonCreateProject.Text = "Create Project";
            this.buttonCreateProject.UseVisualStyleBackColor = true;
            this.buttonCreateProject.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Enabled = false;
            this.buttonBrowse.Location = new System.Drawing.Point(510, 120);
            this.buttonBrowse.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(98, 37);
            this.buttonBrowse.TabIndex = 12;
            this.buttonBrowse.Text = "Browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.btnAltFolderBrowse_Click);
            // 
            // textBoxProjFolder
            // 
            this.textBoxProjFolder.Enabled = false;
            this.textBoxProjFolder.Location = new System.Drawing.Point(21, 123);
            this.textBoxProjFolder.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.textBoxProjFolder.Name = "textBoxProjFolder";
            this.textBoxProjFolder.Size = new System.Drawing.Size(479, 31);
            this.textBoxProjFolder.TabIndex = 11;
            // 
            // labelTemplate
            // 
            this.labelTemplate.AutoSize = true;
            this.labelTemplate.Location = new System.Drawing.Point(37, 116);
            this.labelTemplate.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelTemplate.Name = "labelTemplate";
            this.labelTemplate.Size = new System.Drawing.Size(152, 25);
            this.labelTemplate.TabIndex = 21;
            this.labelTemplate.Text = "Select a Template:";
            // 
            // comboBoxTemplate
            // 
            this.comboBoxTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemplate.FormattingEnabled = true;
            this.comboBoxTemplate.Location = new System.Drawing.Point(199, 113);
            this.comboBoxTemplate.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.comboBoxTemplate.Name = "comboBoxTemplate";
            this.comboBoxTemplate.Size = new System.Drawing.Size(296, 33);
            this.comboBoxTemplate.TabIndex = 3;
            this.comboBoxTemplate.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // labelPlatform
            // 
            this.labelPlatform.AutoSize = true;
            this.labelPlatform.Location = new System.Drawing.Point(40, 71);
            this.labelPlatform.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelPlatform.Name = "labelPlatform";
            this.labelPlatform.Size = new System.Drawing.Size(149, 25);
            this.labelPlatform.TabIndex = 23;
            this.labelPlatform.Text = "Select a Platform:";
            // 
            // comboBoxPlatform
            // 
            this.comboBoxPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPlatform.FormattingEnabled = true;
            this.comboBoxPlatform.Location = new System.Drawing.Point(199, 68);
            this.comboBoxPlatform.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.comboBoxPlatform.Name = "comboBoxPlatform";
            this.comboBoxPlatform.Size = new System.Drawing.Size(256, 33);
            this.comboBoxPlatform.TabIndex = 2;
            this.comboBoxPlatform.SelectedIndexChanged += new System.EventHandler(this.comboBoxPlatform_SelectedIndexChanged);
            // 
            // labelBaseGame
            // 
            this.labelBaseGame.AutoSize = true;
            this.labelBaseGame.Location = new System.Drawing.Point(21, 24);
            this.labelBaseGame.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelBaseGame.Name = "labelBaseGame";
            this.labelBaseGame.Size = new System.Drawing.Size(168, 25);
            this.labelBaseGame.TabIndex = 25;
            this.labelBaseGame.Text = "Select a Base Game:";
            // 
            // radioButtonSA1
            // 
            this.radioButtonSA1.AutoSize = true;
            this.radioButtonSA1.Checked = true;
            this.radioButtonSA1.Location = new System.Drawing.Point(199, 24);
            this.radioButtonSA1.Name = "radioButtonSA1";
            this.radioButtonSA1.Size = new System.Drawing.Size(167, 29);
            this.radioButtonSA1.TabIndex = 0;
            this.radioButtonSA1.TabStop = true;
            this.radioButtonSA1.Text = "Sonic Adventure";
            this.radioButtonSA1.UseVisualStyleBackColor = true;
            // 
            // radioButtonSA2
            // 
            this.radioButtonSA2.AutoSize = true;
            this.radioButtonSA2.Location = new System.Drawing.Point(372, 24);
            this.radioButtonSA2.Name = "radioButtonSA2";
            this.radioButtonSA2.Size = new System.Drawing.Size(182, 29);
            this.radioButtonSA2.TabIndex = 1;
            this.radioButtonSA2.Text = "Sonic Adventure 2";
            this.radioButtonSA2.UseVisualStyleBackColor = true;
            this.radioButtonSA2.CheckedChanged += new System.EventHandler(this.radioButtonSA2_CheckedChanged);
            // 
            // groupBoxAdvancedOptions
            // 
            this.groupBoxAdvancedOptions.Controls.Add(this.comboBoxLabels);
            this.groupBoxAdvancedOptions.Controls.Add(this.label1);
            this.groupBoxAdvancedOptions.Controls.Add(this.textBoxProjFolder);
            this.groupBoxAdvancedOptions.Controls.Add(this.buttonBrowse);
            this.groupBoxAdvancedOptions.Controls.Add(this.checkBoxSaveDifferentFolder);
            this.groupBoxAdvancedOptions.Enabled = false;
            this.groupBoxAdvancedOptions.Location = new System.Drawing.Point(12, 221);
            this.groupBoxAdvancedOptions.Name = "groupBoxAdvancedOptions";
            this.groupBoxAdvancedOptions.Size = new System.Drawing.Size(616, 174);
            this.groupBoxAdvancedOptions.TabIndex = 26;
            this.groupBoxAdvancedOptions.TabStop = false;
            this.groupBoxAdvancedOptions.Text = "Advanced Options";
            // 
            // comboBoxLabels
            // 
            this.comboBoxLabels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLabels.FormattingEnabled = true;
            this.comboBoxLabels.Items.AddRange(new object[] {
            "Regular (address-based)",
            "Full (if available)",
            "None (strip all metadata)"});
            this.comboBoxLabels.Location = new System.Drawing.Point(92, 40);
            this.comboBoxLabels.Name = "comboBoxLabels";
            this.comboBoxLabels.Size = new System.Drawing.Size(244, 33);
            this.comboBoxLabels.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 25);
            this.label1.TabIndex = 8;
            this.label1.Text = "Labels:";
            // 
            // checkBoxAdvanced
            // 
            this.checkBoxAdvanced.AutoSize = true;
            this.checkBoxAdvanced.Location = new System.Drawing.Point(504, 115);
            this.checkBoxAdvanced.Name = "checkBoxAdvanced";
            this.checkBoxAdvanced.Size = new System.Drawing.Size(117, 29);
            this.checkBoxAdvanced.TabIndex = 27;
            this.checkBoxAdvanced.Text = "Advanced";
            this.checkBoxAdvanced.UseVisualStyleBackColor = true;
            this.checkBoxAdvanced.CheckedChanged += new System.EventHandler(this.checkBoxAdvanced_CheckedChanged);
            // 
            // newProj
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(639, 411);
            this.Controls.Add(this.checkBoxAdvanced);
            this.Controls.Add(this.groupBoxAdvancedOptions);
            this.Controls.Add(this.radioButtonSA2);
            this.Controls.Add(this.radioButtonSA1);
            this.Controls.Add(this.labelBaseGame);
            this.Controls.Add(this.comboBoxPlatform);
            this.Controls.Add(this.labelPlatform);
            this.Controls.Add(this.comboBoxTemplate);
            this.Controls.Add(this.labelTemplate);
            this.Controls.Add(this.buttonCreateProject);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "newProj";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Project";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.newProj_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.newProj_FormClosing);
            this.Shown += new System.EventHandler(this.newProj_Shown);
            this.groupBoxAdvancedOptions.ResumeLayout(false);
            this.groupBoxAdvancedOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxSaveDifferentFolder;
		private System.Windows.Forms.Button buttonCreateProject;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.TextBox textBoxProjFolder;
		private System.Windows.Forms.Label labelTemplate;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ComboBox comboBoxTemplate;
		private System.Windows.Forms.Label labelPlatform;
		private System.Windows.Forms.ComboBox comboBoxPlatform;
		private System.Windows.Forms.Label labelBaseGame;
		private System.Windows.Forms.RadioButton radioButtonSA1;
		private System.Windows.Forms.RadioButton radioButtonSA2;
		private System.Windows.Forms.GroupBox groupBoxAdvancedOptions;
		private System.Windows.Forms.ComboBox comboBoxLabels;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxAdvanced;
	}
}