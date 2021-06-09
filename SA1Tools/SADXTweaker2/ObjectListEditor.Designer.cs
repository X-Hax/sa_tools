namespace SADXTweaker2
{
    partial class ObjectListEditor
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
			this.deleteButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.pasteButton = new System.Windows.Forms.Button();
			this.copyButton = new System.Windows.Forms.Button();
			this.objectName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.codeAddress = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.flags = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.arg2 = new System.Windows.Forms.NumericUpDown();
			this.arg1 = new System.Windows.Forms.NumericUpDown();
			this.Label3 = new System.Windows.Forms.Label();
			this.levelList = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.objectList = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.distance = new System.Windows.Forms.NumericUpDown();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.importButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.flags)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.arg2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.arg1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.distance)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// deleteButton
			// 
			this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.deleteButton.AutoSize = true;
			this.deleteButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.deleteButton.Enabled = false;
			this.deleteButton.Location = new System.Drawing.Point(188, 37);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(48, 23);
			this.deleteButton.TabIndex = 4;
			this.deleteButton.Text = "Delete";
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.AutoSize = true;
			this.addButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.addButton.Location = new System.Drawing.Point(146, 37);
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
			this.pasteButton.Location = new System.Drawing.Point(193, 231);
			this.pasteButton.Name = "pasteButton";
			this.pasteButton.Size = new System.Drawing.Size(44, 23);
			this.pasteButton.TabIndex = 12;
			this.pasteButton.Text = "Paste";
			this.pasteButton.UseVisualStyleBackColor = true;
			this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
			// 
			// copyButton
			// 
			this.copyButton.AutoSize = true;
			this.copyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.copyButton.Enabled = false;
			this.copyButton.Location = new System.Drawing.Point(146, 231);
			this.copyButton.Name = "copyButton";
			this.copyButton.Size = new System.Drawing.Size(41, 23);
			this.copyButton.TabIndex = 11;
			this.copyButton.Text = "Copy";
			this.copyButton.UseVisualStyleBackColor = true;
			this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
			// 
			// objectName
			// 
			this.objectName.Location = new System.Drawing.Point(50, 123);
			this.objectName.Name = "objectName";
			this.objectName.Size = new System.Drawing.Size(100, 20);
			this.objectName.TabIndex = 10;
			this.objectName.TextChanged += new System.EventHandler(this.objectName_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 126);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(38, 13);
			this.label6.TabIndex = 36;
			this.label6.Text = "Name:";
			// 
			// codeAddress
			// 
			this.codeAddress.Location = new System.Drawing.Point(119, 97);
			this.codeAddress.Name = "codeAddress";
			this.codeAddress.Size = new System.Drawing.Size(100, 20);
			this.codeAddress.TabIndex = 9;
			this.codeAddress.TextChanged += new System.EventHandler(this.codeAddress_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 100);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(107, 13);
			this.label5.TabIndex = 34;
			this.label5.Text = "Code Address/Label:";
			// 
			// flags
			// 
			this.flags.Hexadecimal = true;
			this.flags.Location = new System.Drawing.Point(47, 45);
			this.flags.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(64, 20);
			this.flags.TabIndex = 7;
			this.flags.ValueChanged += new System.EventHandler(this.flags_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 31;
			this.label2.Text = "Flags:";
			// 
			// arg2
			// 
			this.arg2.Hexadecimal = true;
			this.arg2.Location = new System.Drawing.Point(123, 19);
			this.arg2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(45, 20);
			this.arg2.TabIndex = 6;
			this.arg2.ValueChanged += new System.EventHandler(this.arg2_ValueChanged);
			// 
			// arg1
			// 
			this.arg1.Hexadecimal = true;
			this.arg1.Location = new System.Drawing.Point(72, 19);
			this.arg1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(45, 20);
			this.arg1.TabIndex = 5;
			this.arg1.ValueChanged += new System.EventHandler(this.arg1_ValueChanged);
			// 
			// Label3
			// 
			this.Label3.AutoSize = true;
			this.Label3.Location = new System.Drawing.Point(6, 21);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(60, 13);
			this.Label3.TabIndex = 28;
			this.Label3.Text = "Arguments:";
			// 
			// levelList
			// 
			this.levelList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.levelList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.levelList.Location = new System.Drawing.Point(44, 12);
			this.levelList.Name = "levelList";
			this.levelList.Size = new System.Drawing.Size(253, 21);
			this.levelList.TabIndex = 1;
			this.levelList.SelectedIndexChanged += new System.EventHandler(this.levelList_SelectedIndexChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(12, 15);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(26, 13);
			this.label8.TabIndex = 43;
			this.label8.Text = "List:";
			// 
			// objectList
			// 
			this.objectList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.objectList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.objectList.Location = new System.Drawing.Point(48, 39);
			this.objectList.Name = "objectList";
			this.objectList.Size = new System.Drawing.Size(92, 21);
			this.objectList.TabIndex = 2;
			this.objectList.SelectedIndexChanged += new System.EventHandler(this.objectList_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 42);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 45;
			this.label1.Text = "Item:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 73);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 13);
			this.label4.TabIndex = 46;
			this.label4.Text = "Distance:";
			// 
			// distance
			// 
			this.distance.DecimalPlaces = 3;
			this.distance.Location = new System.Drawing.Point(64, 71);
			this.distance.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            196608});
			this.distance.Name = "distance";
			this.distance.Size = new System.Drawing.Size(120, 20);
			this.distance.TabIndex = 8;
			this.distance.ValueChanged += new System.EventHandler(this.distance_ValueChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox1.Controls.Add(this.Label3);
			this.groupBox1.Controls.Add(this.distance);
			this.groupBox1.Controls.Add(this.arg1);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.arg2);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.flags);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.codeAddress);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.objectName);
			this.groupBox1.Enabled = false;
			this.groupBox1.Location = new System.Drawing.Point(12, 66);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.groupBox1.Size = new System.Drawing.Size(225, 159);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Object Data";
			// 
			// importButton
			// 
			this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.importButton.AutoSize = true;
			this.importButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.importButton.Location = new System.Drawing.Point(242, 37);
			this.importButton.Name = "importButton";
			this.importButton.Size = new System.Drawing.Size(55, 23);
			this.importButton.TabIndex = 46;
			this.importButton.Text = "Import...";
			this.importButton.UseVisualStyleBackColor = true;
			this.importButton.Click += new System.EventHandler(this.importButton_Click);
			// 
			// ObjectListEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(309, 264);
			this.Controls.Add(this.importButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.objectList);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.pasteButton);
			this.Controls.Add(this.copyButton);
			this.Controls.Add(this.levelList);
			this.Name = "ObjectListEditor";
			this.ShowIcon = false;
			this.Text = "Object List Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObjectListEditor_FormClosing);
			this.Load += new System.EventHandler(this.ObjectListEditor_Load);
			((System.ComponentModel.ISupportInitialize)(this.flags)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.arg2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.arg1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.distance)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox objectName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox levelList;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox objectList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.NumericUpDown flags;
        private System.Windows.Forms.NumericUpDown arg2;
        private System.Windows.Forms.NumericUpDown arg1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown distance;
        private System.Windows.Forms.TextBox codeAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button importButton;
    }
}