namespace SADXTweaker2
{
    partial class CutsceneTextEditor
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.objectList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.levelList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.language = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.AcceptsTab = true;
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.Enabled = false;
            this.richTextBox1.Location = new System.Drawing.Point(12, 93);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(260, 159);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // objectList
            // 
            this.objectList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectList.Location = new System.Drawing.Point(48, 66);
            this.objectList.Name = "objectList";
            this.objectList.Size = new System.Drawing.Size(224, 21);
            this.objectList.TabIndex = 3;
            this.objectList.SelectedIndexChanged += new System.EventHandler(this.objectList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 59;
            this.label1.Text = "Item:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 58;
            this.label8.Text = "Scene:";
            // 
            // levelList
            // 
            this.levelList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.levelList.Location = new System.Drawing.Point(59, 12);
            this.levelList.Name = "levelList";
            this.levelList.Size = new System.Drawing.Size(213, 21);
            this.levelList.TabIndex = 1;
            this.levelList.SelectedIndexChanged += new System.EventHandler(this.levelList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 61;
            this.label2.Text = "Language:";
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
            this.language.TabIndex = 2;
            this.language.SelectedIndexChanged += new System.EventHandler(this.language_SelectedIndexChanged);
            // 
            // CutsceneTextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.language);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.objectList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.levelList);
            this.Name = "CutsceneTextEditor";
            this.ShowIcon = false;
            this.Text = "Cutscene Text Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CutsceneTextEditor_FormClosing);
            this.Load += new System.EventHandler(this.CutsceneTextEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox objectList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox levelList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox language;
    }
}