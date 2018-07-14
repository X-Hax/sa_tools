namespace ProjectManager
{
    partial class GameConfig
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
            this.label1 = new System.Windows.Forms.Label();
            this.SADXPath = new System.Windows.Forms.TextBox();
            this.SADXBrowseButton = new System.Windows.Forms.Button();
            this.SA2PCPath = new System.Windows.Forms.TextBox();
            this.SA2PCLabel = new System.Windows.Forms.Label();
            this._AcceptButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SA2PCBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SADX PC Location:";
            // 
            // SADXPath
            // 
            this.SADXPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SADXPath.Location = new System.Drawing.Point(26, 39);
            this.SADXPath.Name = "SADXPath";
            this.SADXPath.Size = new System.Drawing.Size(344, 20);
            this.SADXPath.TabIndex = 1;
            // 
            // SADXBrowseButton
            // 
            this.SADXBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SADXBrowseButton.Location = new System.Drawing.Point(384, 36);
            this.SADXBrowseButton.Name = "SADXBrowseButton";
            this.SADXBrowseButton.Size = new System.Drawing.Size(101, 23);
            this.SADXBrowseButton.TabIndex = 2;
            this.SADXBrowseButton.Text = "Browse";
            this.SADXBrowseButton.UseVisualStyleBackColor = true;
            this.SADXBrowseButton.Click += new System.EventHandler(this.SADXBrowseButton_Click);
            // 
            // SA2PCPath
            // 
            this.SA2PCPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SA2PCPath.Location = new System.Drawing.Point(26, 90);
            this.SA2PCPath.Name = "SA2PCPath";
            this.SA2PCPath.Size = new System.Drawing.Size(344, 20);
            this.SA2PCPath.TabIndex = 4;
            // 
            // SA2PCLabel
            // 
            this.SA2PCLabel.AutoSize = true;
            this.SA2PCLabel.Location = new System.Drawing.Point(23, 74);
            this.SA2PCLabel.Name = "SA2PCLabel";
            this.SA2PCLabel.Size = new System.Drawing.Size(91, 13);
            this.SA2PCLabel.TabIndex = 3;
            this.SA2PCLabel.Text = "SA2 PC Location:";
            // 
            // AcceptButton
            // 
            this._AcceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._AcceptButton.Location = new System.Drawing.Point(415, 152);
            this._AcceptButton.Name = "AcceptButton";
            this._AcceptButton.Size = new System.Drawing.Size(75, 23);
            this._AcceptButton.TabIndex = 6;
            this._AcceptButton.Text = "Accept";
            this._AcceptButton.UseVisualStyleBackColor = true;
            this._AcceptButton.Click += new System.EventHandler(this.AcceptButton_Click);
            // 
            // SA2PCBrowse
            // 
            this.SA2PCBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SA2PCBrowse.Location = new System.Drawing.Point(384, 87);
            this.SA2PCBrowse.Name = "SA2PCBrowse";
            this.SA2PCBrowse.Size = new System.Drawing.Size(101, 23);
            this.SA2PCBrowse.TabIndex = 5;
            this.SA2PCBrowse.Text = "Browse";
            this.SA2PCBrowse.UseVisualStyleBackColor = true;
            this.SA2PCBrowse.Click += new System.EventHandler(this.SA2PCBrowse_Click);
            // 
            // GameConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 187);
            this.Controls.Add(this._AcceptButton);
            this.Controls.Add(this.SA2PCBrowse);
            this.Controls.Add(this.SA2PCPath);
            this.Controls.Add(this.SA2PCLabel);
            this.Controls.Add(this.SADXBrowseButton);
            this.Controls.Add(this.SADXPath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "GameConfig";
            this.Text = "GameConfig";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameConfig_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.GameConfig_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SADXPath;
        private System.Windows.Forms.Button SADXBrowseButton;
        private System.Windows.Forms.TextBox SA2PCPath;
        private System.Windows.Forms.Label SA2PCLabel;
        private System.Windows.Forms.Button _AcceptButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button SA2PCBrowse;
    }
}