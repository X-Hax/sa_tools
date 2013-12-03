namespace SADXTweaker2
{
    partial class ChaoMessageFileEditor
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            this.line = new System.Windows.Forms.RichTextBox();
            this.filename = new System.Windows.Forms.ComboBox();
            this.language = new System.Windows.Forms.ComboBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.messageNum = new System.Windows.Forms.ComboBox();
            this.messageRemoveButton = new System.Windows.Forms.Button();
            this.messageAddButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(26, 13);
            label1.TabIndex = 0;
            label1.Text = "File:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 42);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(58, 13);
            label3.TabIndex = 2;
            label3.Text = "Language:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 98);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(53, 13);
            label4.TabIndex = 8;
            label4.Text = "Message:";
            // 
            // line
            // 
            this.line.AcceptsTab = true;
            this.line.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.line.DetectUrls = false;
            this.line.Enabled = false;
            this.line.Location = new System.Drawing.Point(12, 122);
            this.line.Name = "line";
            this.line.Size = new System.Drawing.Size(284, 130);
            this.line.TabIndex = 11;
            this.line.Text = "";
            this.line.TextChanged += new System.EventHandler(this.line_TextChanged);
            // 
            // filename
            // 
            this.filename.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filename.FormattingEnabled = true;
            this.filename.Items.AddRange(new object[] {
            "CHAODX_MESSAGE_BLACKMARKET",
            "CHAODX_MESSAGE_HINT",
            "CHAODX_MESSAGE_ITEM",
            "CHAODX_MESSAGE_ODEKAKE",
            "CHAODX_MESSAGE_PLAYERACTION",
            "CHAODX_MESSAGE_RACE",
            "CHAODX_MESSAGE_SYSTEM",
            "MSGALITEM",
            "MSGALKINDERBL",
            "MSGALKINDERPR",
            "MSGALWARN"});
            this.filename.Location = new System.Drawing.Point(44, 12);
            this.filename.Name = "filename";
            this.filename.Size = new System.Drawing.Size(252, 21);
            this.filename.TabIndex = 3;
            this.filename.SelectedIndexChanged += new System.EventHandler(this.field_SelectedIndexChanged);
            // 
            // language
            // 
            this.language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.language.FormattingEnabled = true;
            this.language.Items.AddRange(new object[] {
            "Japanese",
            "English",
            "French",
            "Spanish",
            "German"});
            this.language.Location = new System.Drawing.Point(76, 39);
            this.language.Name = "language";
            this.language.Size = new System.Drawing.Size(121, 21);
            this.language.TabIndex = 5;
            this.language.SelectedIndexChanged += new System.EventHandler(this.field_SelectedIndexChanged);
            // 
            // loadButton
            // 
            this.loadButton.Enabled = false;
            this.loadButton.Location = new System.Drawing.Point(12, 66);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 6;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(93, 66);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 7;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // messageNum
            // 
            this.messageNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.messageNum.FormattingEnabled = true;
            this.messageNum.Location = new System.Drawing.Point(71, 95);
            this.messageNum.Name = "messageNum";
            this.messageNum.Size = new System.Drawing.Size(121, 21);
            this.messageNum.TabIndex = 9;
            this.messageNum.SelectedIndexChanged += new System.EventHandler(this.messageNum_SelectedIndexChanged);
            // 
            // messageRemoveButton
            // 
            this.messageRemoveButton.AutoSize = true;
            this.messageRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.messageRemoveButton.Enabled = false;
            this.messageRemoveButton.Location = new System.Drawing.Point(240, 93);
            this.messageRemoveButton.Name = "messageRemoveButton";
            this.messageRemoveButton.Size = new System.Drawing.Size(57, 23);
            this.messageRemoveButton.TabIndex = 12;
            this.messageRemoveButton.Text = "Remove";
            this.messageRemoveButton.UseVisualStyleBackColor = true;
            this.messageRemoveButton.Click += new System.EventHandler(this.messageRemoveButton_Click);
            // 
            // messageAddButton
            // 
            this.messageAddButton.AutoSize = true;
            this.messageAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.messageAddButton.Location = new System.Drawing.Point(198, 93);
            this.messageAddButton.Name = "messageAddButton";
            this.messageAddButton.Size = new System.Drawing.Size(36, 23);
            this.messageAddButton.TabIndex = 11;
            this.messageAddButton.Text = "Add";
            this.messageAddButton.UseVisualStyleBackColor = true;
            this.messageAddButton.Click += new System.EventHandler(this.messageAddButton_Click);
            // 
            // ChaoMessageFileEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 264);
            this.Controls.Add(this.line);
            this.Controls.Add(this.messageRemoveButton);
            this.Controls.Add(this.messageAddButton);
            this.Controls.Add(this.messageNum);
            this.Controls.Add(label4);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.language);
            this.Controls.Add(this.filename);
            this.Controls.Add(label3);
            this.Controls.Add(label1);
            this.Name = "ChaoMessageFileEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Chao Message File Editor";
            this.Load += new System.EventHandler(this.ChaoMessageFileEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox filename;
        private System.Windows.Forms.ComboBox language;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ComboBox messageNum;
        private System.Windows.Forms.RichTextBox line;
        private System.Windows.Forms.Button messageRemoveButton;
        private System.Windows.Forms.Button messageAddButton;
    }
}

