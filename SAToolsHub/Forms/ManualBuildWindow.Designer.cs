namespace SAModel.SAEditorCommon
{
    partial class ManualBuildWindow
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
            this.assemblyItemTabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.LoadingLabel = new System.Windows.Forms.Label();
            this.IniExportButton = new System.Windows.Forms.Button();
            this.CPPExportButton = new System.Windows.Forms.Button();
            this.CheckAllButton = new System.Windows.Forms.Button();
            this.UncheckAllButton = new System.Windows.Forms.Button();
            this.CheckModifiedButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.searchLab = new System.Windows.Forms.Label();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.assemblyItemTabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // assemblyItemTabs
            // 
            this.assemblyItemTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assemblyItemTabs.Controls.Add(this.tabPage1);
            this.assemblyItemTabs.Location = new System.Drawing.Point(14, 14);
            this.assemblyItemTabs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.assemblyItemTabs.Name = "assemblyItemTabs";
            this.assemblyItemTabs.SelectedIndex = 0;
            this.assemblyItemTabs.Size = new System.Drawing.Size(905, 450);
            this.assemblyItemTabs.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.LoadingLabel);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Size = new System.Drawing.Size(897, 422);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // LoadingLabel
            // 
            this.LoadingLabel.AutoSize = true;
            this.LoadingLabel.Location = new System.Drawing.Point(360, 178);
            this.LoadingLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LoadingLabel.Name = "LoadingLabel";
            this.LoadingLabel.Size = new System.Drawing.Size(50, 15);
            this.LoadingLabel.TabIndex = 6;
            this.LoadingLabel.Text = "Loading";
            // 
            // IniExportButton
            // 
            this.IniExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.IniExportButton.Location = new System.Drawing.Point(553, 517);
            this.IniExportButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.IniExportButton.Name = "IniExportButton";
            this.IniExportButton.Size = new System.Drawing.Size(177, 35);
            this.IniExportButton.TabIndex = 1;
            this.IniExportButton.Text = "Export INI";
            this.toolTip1.SetToolTip(this.IniExportButton, "Exports selected items to an INI mod.");
            this.IniExportButton.UseVisualStyleBackColor = true;
            this.IniExportButton.Click += new System.EventHandler(this.IniExportButton_Click);
            // 
            // CPPExportButton
            // 
            this.CPPExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CPPExportButton.Location = new System.Drawing.Point(737, 517);
            this.CPPExportButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CPPExportButton.Name = "CPPExportButton";
            this.CPPExportButton.Size = new System.Drawing.Size(177, 35);
            this.CPPExportButton.TabIndex = 2;
            this.CPPExportButton.Text = "Export C++";
            this.toolTip1.SetToolTip(this.CPPExportButton, "Exports selected items to a C++ file.");
            this.CPPExportButton.UseVisualStyleBackColor = true;
            this.CPPExportButton.Click += new System.EventHandler(this.CPPExportButton_Click);
            // 
            // CheckAllButton
            // 
            this.CheckAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CheckAllButton.Location = new System.Drawing.Point(14, 517);
            this.CheckAllButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckAllButton.Name = "CheckAllButton";
            this.CheckAllButton.Size = new System.Drawing.Size(134, 35);
            this.CheckAllButton.TabIndex = 3;
            this.CheckAllButton.Text = "Check All";
            this.toolTip1.SetToolTip(this.CheckAllButton, "Check all items in this tab.");
            this.CheckAllButton.UseVisualStyleBackColor = true;
            this.CheckAllButton.Click += new System.EventHandler(this.CheckAllButton_Click);
            // 
            // UncheckAllButton
            // 
            this.UncheckAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.UncheckAllButton.Location = new System.Drawing.Point(155, 517);
            this.UncheckAllButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.UncheckAllButton.Name = "UncheckAllButton";
            this.UncheckAllButton.Size = new System.Drawing.Size(148, 35);
            this.UncheckAllButton.TabIndex = 4;
            this.UncheckAllButton.Text = "Uncheck All";
            this.toolTip1.SetToolTip(this.UncheckAllButton, "Uncheck all items in this tab.");
            this.UncheckAllButton.UseVisualStyleBackColor = true;
            this.UncheckAllButton.Click += new System.EventHandler(this.UncheckAllButton_Click);
            // 
            // CheckModifiedButton
            // 
            this.CheckModifiedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CheckModifiedButton.Location = new System.Drawing.Point(310, 517);
            this.CheckModifiedButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckModifiedButton.Name = "CheckModifiedButton";
            this.CheckModifiedButton.Size = new System.Drawing.Size(172, 35);
            this.CheckModifiedButton.TabIndex = 5;
            this.CheckModifiedButton.Text = "Check Modified";
            this.toolTip1.SetToolTip(this.CheckModifiedButton, "Check all modified items in this tab.");
            this.CheckModifiedButton.UseVisualStyleBackColor = true;
            this.CheckModifiedButton.Click += new System.EventHandler(this.CheckModifiedButton_Click);
            // 
            // searchLab
            // 
            this.searchLab.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.searchLab.AutoSize = true;
            this.searchLab.BackColor = System.Drawing.Color.Transparent;
            this.searchLab.Location = new System.Drawing.Point(346, 479);
            this.searchLab.Name = "searchLab";
            this.searchLab.Size = new System.Drawing.Size(45, 15);
            this.searchLab.TabIndex = 6;
            this.searchLab.Text = "Search:";
            // 
            // searchBox
            // 
            this.searchBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.searchBox.Location = new System.Drawing.Point(397, 476);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(156, 23);
            this.searchBox.TabIndex = 7;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // ManualBuildWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 565);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.searchLab);
            this.Controls.Add(this.assemblyItemTabs);
            this.Controls.Add(this.CheckModifiedButton);
            this.Controls.Add(this.UncheckAllButton);
            this.Controls.Add(this.CheckAllButton);
            this.Controls.Add(this.CPPExportButton);
            this.Controls.Add(this.IniExportButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ManualBuildWindow";
            this.Text = "Manual Item Selection";
            this.Shown += new System.EventHandler(this.ManualBuildWindow_Shown);
            this.assemblyItemTabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl assemblyItemTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button IniExportButton;
        private System.Windows.Forms.Button CPPExportButton;
        private System.Windows.Forms.Button CheckAllButton;
        private System.Windows.Forms.Button UncheckAllButton;
        private System.Windows.Forms.Button CheckModifiedButton;
        private System.Windows.Forms.Label LoadingLabel;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label searchLab;
		private System.Windows.Forms.TextBox searchBox;
	}
}