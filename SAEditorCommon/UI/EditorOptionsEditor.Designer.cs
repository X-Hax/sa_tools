namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    partial class EditorOptionsEditor
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.fullBrightCheck = new System.Windows.Forms.CheckBox();
			this.cullModeDropdown = new System.Windows.Forms.ComboBox();
			this.cullingLabel = new System.Windows.Forms.Label();
			this.polyFillLabel = new System.Windows.Forms.Label();
			this.fillModeDropDown = new System.Windows.Forms.ComboBox();
			this.drawDistLabel = new System.Windows.Forms.Label();
			this.drawDistSlider = new System.Windows.Forms.TrackBar();
			this.doneButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.drawDistSlider)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.fullBrightCheck);
			this.groupBox1.Controls.Add(this.cullModeDropdown);
			this.groupBox1.Controls.Add(this.cullingLabel);
			this.groupBox1.Controls.Add(this.polyFillLabel);
			this.groupBox1.Controls.Add(this.fillModeDropDown);
			this.groupBox1.Controls.Add(this.drawDistLabel);
			this.groupBox1.Controls.Add(this.drawDistSlider);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(239, 226);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Render Options";
			// 
			// fullBrightCheck
			// 
			this.fullBrightCheck.AutoSize = true;
			this.fullBrightCheck.Location = new System.Drawing.Point(18, 191);
			this.fullBrightCheck.Name = "fullBrightCheck";
			this.fullBrightCheck.Size = new System.Drawing.Size(198, 17);
			this.fullBrightCheck.TabIndex = 5;
			this.fullBrightCheck.Text = "Use fullbright instead of natural lights";
			this.fullBrightCheck.UseVisualStyleBackColor = true;
			this.fullBrightCheck.Click += new System.EventHandler(this.fullBrightCheck_Click);
			// 
			// cullModeDropdown
			// 
			this.cullModeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cullModeDropdown.FormattingEnabled = true;
			this.cullModeDropdown.Items.AddRange(new object[] {
            "None (Draws both sides)",
            "Clockwise",
            "Counter-Clockwise"});
			this.cullModeDropdown.Location = new System.Drawing.Point(18, 153);
			this.cullModeDropdown.Name = "cullModeDropdown";
			this.cullModeDropdown.Size = new System.Drawing.Size(196, 21);
			this.cullModeDropdown.TabIndex = 4;
			this.cullModeDropdown.SelectionChangeCommitted += new System.EventHandler(this.cullModeDropdown_SelectionChangeCommitted);
			// 
			// cullingLabel
			// 
			this.cullingLabel.AutoSize = true;
			this.cullingLabel.Location = new System.Drawing.Point(15, 137);
			this.cullingLabel.Name = "cullingLabel";
			this.cullingLabel.Size = new System.Drawing.Size(90, 13);
			this.cullingLabel.TabIndex = 2;
			this.cullingLabel.Text = "Backface Culling:";
			// 
			// polyFillLabel
			// 
			this.polyFillLabel.AutoSize = true;
			this.polyFillLabel.Location = new System.Drawing.Point(15, 81);
			this.polyFillLabel.Name = "polyFillLabel";
			this.polyFillLabel.Size = new System.Drawing.Size(93, 13);
			this.polyFillLabel.TabIndex = 3;
			this.polyFillLabel.Text = "Polygon Fill Mode:";
			// 
			// fillModeDropDown
			// 
			this.fillModeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.fillModeDropDown.FormattingEnabled = true;
			this.fillModeDropDown.Items.AddRange(new object[] {
            "Vertex-Only",
            "Wireframe",
            "Solid"});
			this.fillModeDropDown.Location = new System.Drawing.Point(18, 97);
			this.fillModeDropDown.Name = "fillModeDropDown";
			this.fillModeDropDown.Size = new System.Drawing.Size(196, 21);
			this.fillModeDropDown.TabIndex = 2;
			this.fillModeDropDown.SelectionChangeCommitted += new System.EventHandler(this.fillModeDropDown_SelectionChangeCommitted);
			// 
			// drawDistLabel
			// 
			this.drawDistLabel.AutoSize = true;
			this.drawDistLabel.Location = new System.Drawing.Point(15, 21);
			this.drawDistLabel.Name = "drawDistLabel";
			this.drawDistLabel.Size = new System.Drawing.Size(80, 13);
			this.drawDistLabel.TabIndex = 1;
			this.drawDistLabel.Text = "Draw Distance:";
			// 
			// drawDistSlider
			// 
			this.drawDistSlider.Location = new System.Drawing.Point(6, 38);
			this.drawDistSlider.Maximum = 10000;
			this.drawDistSlider.Minimum = 1000;
			this.drawDistSlider.Name = "drawDistSlider";
			this.drawDistSlider.Size = new System.Drawing.Size(227, 45);
			this.drawDistSlider.SmallChange = 100;
			this.drawDistSlider.TabIndex = 0;
			this.drawDistSlider.TickFrequency = 500;
			this.drawDistSlider.Value = 1000;
			this.drawDistSlider.Scroll += new System.EventHandler(this.drawDistSlider_Scroll);
			// 
			// doneButton
			// 
			this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.doneButton.Location = new System.Drawing.Point(187, 252);
			this.doneButton.Name = "doneButton";
			this.doneButton.Size = new System.Drawing.Size(75, 23);
			this.doneButton.TabIndex = 1;
			this.doneButton.Text = "Done";
			this.doneButton.UseVisualStyleBackColor = true;
			this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
			// 
			// EditorOptionsEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(274, 287);
			this.Controls.Add(this.doneButton);
			this.Controls.Add(this.groupBox1);
			this.Name = "EditorOptionsEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Editor Options / Preferences";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.drawDistSlider)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.Label drawDistLabel;
        private System.Windows.Forms.TrackBar drawDistSlider;
        private System.Windows.Forms.Label polyFillLabel;
        private System.Windows.Forms.ComboBox fillModeDropDown;
        private System.Windows.Forms.Label cullingLabel;
        private System.Windows.Forms.ComboBox cullModeDropdown;
        private System.Windows.Forms.CheckBox fullBrightCheck;
    }
}