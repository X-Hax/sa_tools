namespace SADXTweaker2
{
    partial class LevelTextureListEditor
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
            this.textureAddress = new System.Windows.Forms.NumericUpDown();
            this.textureList = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textureName = new SADXTweaker2.FileListControl();
            this.levelList = new System.Windows.Forms.ComboBox();
            this.addButton = new System.Windows.Forms.Button();
            this.pasteButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.textureAddress)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textureAddress
            // 
            this.textureAddress.Hexadecimal = true;
            this.textureAddress.Location = new System.Drawing.Point(60, 48);
            this.textureAddress.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.textureAddress.Name = "textureAddress";
            this.textureAddress.Size = new System.Drawing.Size(90, 20);
            this.textureAddress.TabIndex = 7;
            this.textureAddress.ValueChanged += new System.EventHandler(this.textureAddress_ValueChanged);
            // 
            // textureList
            // 
            this.textureList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textureList.Location = new System.Drawing.Point(48, 39);
            this.textureList.Name = "textureList";
            this.textureList.Size = new System.Drawing.Size(134, 21);
            this.textureList.TabIndex = 2;
            this.textureList.SelectedIndexChanged += new System.EventHandler(this.textureList_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Address:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 36;
            this.label6.Text = "Filename:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 62;
            this.label8.Text = "List:";
            // 
            // deleteButton
            // 
            this.deleteButton.AutoSize = true;
            this.deleteButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(230, 37);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(48, 23);
            this.deleteButton.TabIndex = 4;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 63;
            this.label1.Text = "Item:";
            // 
            // textureName
            // 
            this.textureName.AutoSize = true;
            this.textureName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.textureName.Directory = null;
            this.textureName.Extension = "pvm";
            this.textureName.Location = new System.Drawing.Point(64, 19);
            this.textureName.Name = "textureName";
            this.textureName.Size = new System.Drawing.Size(177, 23);
            this.textureName.TabIndex = 6;
            this.textureName.Title = "Choose Texture File...";
            this.textureName.Value = "";
            this.textureName.ValueChanged += new System.EventHandler(this.textureName_ValueChanged);
            // 
            // levelList
            // 
            this.levelList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.levelList.Location = new System.Drawing.Point(44, 12);
            this.levelList.Name = "levelList";
            this.levelList.Size = new System.Drawing.Size(228, 21);
            this.levelList.TabIndex = 1;
            this.levelList.SelectedIndexChanged += new System.EventHandler(this.levelList_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.AutoSize = true;
            this.addButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addButton.Location = new System.Drawing.Point(188, 37);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(36, 23);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // pasteButton
            // 
            this.pasteButton.AutoSize = true;
            this.pasteButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pasteButton.Enabled = false;
            this.pasteButton.Location = new System.Drawing.Point(228, 156);
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(44, 23);
            this.pasteButton.TabIndex = 9;
            this.pasteButton.Text = "Paste";
            this.pasteButton.UseVisualStyleBackColor = true;
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.AutoSize = true;
            this.copyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.copyButton.Enabled = false;
            this.copyButton.Location = new System.Drawing.Point(181, 156);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(41, 23);
            this.copyButton.TabIndex = 8;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.textureAddress);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textureName);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox1.Size = new System.Drawing.Size(247, 84);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PVM Data";
            // 
            // LevelTextureListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 191);
            this.Controls.Add(this.textureList);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.levelList);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.pasteButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.groupBox1);
            this.Name = "LevelTextureListEditor";
            this.ShowIcon = false;
            this.Text = "Level Texture List Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LevelTextureListEditor_FormClosing);
            this.Load += new System.EventHandler(this.LevelTextureListEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.textureAddress)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown textureAddress;
        private System.Windows.Forms.ComboBox textureList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Label label1;
        private SADXTweaker2.FileListControl textureName;
        private System.Windows.Forms.ComboBox levelList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}