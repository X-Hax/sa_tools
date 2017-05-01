using System.Windows.Forms;

namespace SonicRetro.SAModel.SAMDL
{
    partial class ModelFileDialog : Form
    {

        //Form overrides dispose to clean up the component list.
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //Required by the Windows Form Designer

        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
			this.OK_Button = new System.Windows.Forms.Button();
			this.Label1 = new System.Windows.Forms.Label();
			this.ComboBox1 = new System.Windows.Forms.ComboBox();
			this.NumericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.Label2 = new System.Windows.Forms.Label();
			this.CheckBox3 = new System.Windows.Forms.CheckBox();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
			this.SuspendLayout();
			// 
			// OK_Button
			// 
			this.OK_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK_Button.Location = new System.Drawing.Point(200, 139);
			this.OK_Button.Name = "OK_Button";
			this.OK_Button.Size = new System.Drawing.Size(67, 23);
			this.OK_Button.TabIndex = 0;
			this.OK_Button.Text = "&OK";
			// 
			// Label1
			// 
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(12, 15);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(28, 13);
			this.Label1.TabIndex = 1;
			this.Label1.Text = "Key:";
			// 
			// ComboBox1
			// 
			this.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox1.FormattingEnabled = true;
			this.ComboBox1.Items.AddRange(new object[] {
            "EXE",
            "DLL",
            "1st_read.bin",
            "SA1 Level",
            "SA2 Level",
            "SA1 Event File",
            "SA2 Event File",
            "SA2PC Event File",
            "Model File"});
			this.ComboBox1.Location = new System.Drawing.Point(150, 12);
			this.ComboBox1.Name = "ComboBox1";
			this.ComboBox1.Size = new System.Drawing.Size(121, 21);
			this.ComboBox1.TabIndex = 2;
			this.ComboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
			// 
			// NumericUpDown1
			// 
			this.NumericUpDown1.Hexadecimal = true;
			this.NumericUpDown1.Location = new System.Drawing.Point(66, 39);
			this.NumericUpDown1.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
			this.NumericUpDown1.Name = "NumericUpDown1";
			this.NumericUpDown1.Size = new System.Drawing.Size(98, 20);
			this.NumericUpDown1.TabIndex = 3;
			// 
			// Label2
			// 
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(12, 41);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(48, 13);
			this.Label2.TabIndex = 4;
			this.Label2.Text = "Address:";
			// 
			// CheckBox3
			// 
			this.CheckBox3.AutoSize = true;
			this.CheckBox3.Checked = true;
			this.CheckBox3.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBox3.Location = new System.Drawing.Point(170, 40);
			this.CheckBox3.Name = "CheckBox3";
			this.CheckBox3.Size = new System.Drawing.Size(45, 17);
			this.CheckBox3.TabIndex = 9;
			this.CheckBox3.Text = "Hex";
			this.CheckBox3.UseVisualStyleBackColor = true;
			this.CheckBox3.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Hexadecimal = true;
			this.numericUpDown2.Location = new System.Drawing.Point(46, 13);
			this.numericUpDown2.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(98, 20);
			this.numericUpDown2.TabIndex = 15;
			// 
			// comboBox2
			// 
			this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Items.AddRange(new object[] {
            "Basic",
            "SADX Basic",
            "Chunk"});
			this.comboBox2.Location = new System.Drawing.Point(60, 114);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(104, 21);
			this.comboBox2.TabIndex = 17;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 117);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(42, 13);
			this.label3.TabIndex = 16;
			this.label3.Text = "Format:";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(12, 65);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(99, 17);
			this.checkBox1.TabIndex = 18;
			this.checkBox1.Text = "Load Animation";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// numericUpDown3
			// 
			this.numericUpDown3.Enabled = false;
			this.numericUpDown3.Hexadecimal = true;
			this.numericUpDown3.Location = new System.Drawing.Point(66, 88);
			this.numericUpDown3.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
			this.numericUpDown3.Name = "numericUpDown3";
			this.numericUpDown3.Size = new System.Drawing.Size(98, 20);
			this.numericUpDown3.TabIndex = 19;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 90);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 13);
			this.label4.TabIndex = 20;
			this.label4.Text = "Address:";
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(170, 116);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(77, 17);
			this.checkBox2.TabIndex = 21;
			this.checkBox2.Text = "Big Endian";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// ModelFileDialog
			// 
			this.AcceptButton = this.OK_Button;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(279, 174);
			this.Controls.Add(this.checkBox2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.numericUpDown3);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.OK_Button);
			this.Controls.Add(this.comboBox2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.numericUpDown2);
			this.Controls.Add(this.CheckBox3);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.NumericUpDown1);
			this.Controls.Add(this.ComboBox1);
			this.Controls.Add(this.Label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ModelFileDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SA1/SA2 Model Editor";
			this.Load += new System.EventHandler(this.Dialog1_Load);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        internal Button OK_Button;
        internal Label Label1;
        internal ComboBox ComboBox1;
        internal NumericUpDown NumericUpDown1;
        internal Label Label2;
        internal CheckBox CheckBox3;
        internal NumericUpDown numericUpDown2;
        internal ComboBox comboBox2;
        internal Label label3;
        internal NumericUpDown numericUpDown3;
        internal CheckBox checkBox1;
        private Label label4;
		internal CheckBox checkBox2;
	}
}