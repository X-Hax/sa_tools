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
            this.Label_Key = new System.Windows.Forms.Label();
            this.ComboBox_FileType = new System.Windows.Forms.ComboBox();
            this.NumericUpDown_ModelAddress = new System.Windows.Forms.NumericUpDown();
            this.Label_ModelAddress = new System.Windows.Forms.Label();
            this.CheckBox_Hex = new System.Windows.Forms.CheckBox();
            this.NumericUpDown_Key = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.ComboBox_Format = new System.Windows.Forms.ComboBox();
            this.Label_Format = new System.Windows.Forms.Label();
            this.CheckBox_LoadAnimation = new System.Windows.Forms.CheckBox();
            this.NumericUpDown_AnimationAddress = new System.Windows.Forms.NumericUpDown();
            this.Label_AnimationAddress = new System.Windows.Forms.Label();
            this.CheckBox_BigEndian = new System.Windows.Forms.CheckBox();
            this.typBinary = new System.Windows.Forms.RadioButton();
            this.typSA2MDL = new System.Windows.Forms.RadioButton();
            this.typSA2BMDL = new System.Windows.Forms.RadioButton();
            this.Label_Type = new System.Windows.Forms.Label();
            this.CheckBox_Memory = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ModelAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Key)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AnimationAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // OK_Button
            // 
            this.OK_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK_Button.Location = new System.Drawing.Point(200, 199);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(67, 23);
            this.OK_Button.TabIndex = 0;
            this.OK_Button.Text = "&OK";
            // 
            // Label_Key
            // 
            this.Label_Key.AutoSize = true;
            this.Label_Key.Location = new System.Drawing.Point(12, 66);
            this.Label_Key.Name = "Label_Key";
            this.Label_Key.Size = new System.Drawing.Size(28, 13);
            this.Label_Key.TabIndex = 1;
            this.Label_Key.Text = "Key:";
            // 
            // ComboBox_FileType
            // 
            this.ComboBox_FileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_FileType.FormattingEnabled = true;
            this.ComboBox_FileType.Items.AddRange(new object[] {
            "EXE",
            "DLL",
            "1st_read.bin",
            "SA1 Level",
            "SA2 Level",
            "SA1 Event File",
            "SA2 Event File",
            "SA2PC Event File",
            "Model File"});
            this.ComboBox_FileType.Location = new System.Drawing.Point(150, 63);
            this.ComboBox_FileType.Name = "ComboBox_FileType";
            this.ComboBox_FileType.Size = new System.Drawing.Size(121, 21);
            this.ComboBox_FileType.TabIndex = 2;
            this.ComboBox_FileType.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // NumericUpDown_ModelAddress
            // 
            this.NumericUpDown_ModelAddress.Hexadecimal = true;
            this.NumericUpDown_ModelAddress.Location = new System.Drawing.Point(66, 90);
            this.NumericUpDown_ModelAddress.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.NumericUpDown_ModelAddress.Name = "NumericUpDown_ModelAddress";
            this.NumericUpDown_ModelAddress.Size = new System.Drawing.Size(98, 20);
            this.NumericUpDown_ModelAddress.TabIndex = 3;
            // 
            // Label_ModelAddress
            // 
            this.Label_ModelAddress.AutoSize = true;
            this.Label_ModelAddress.Location = new System.Drawing.Point(12, 92);
            this.Label_ModelAddress.Name = "Label_ModelAddress";
            this.Label_ModelAddress.Size = new System.Drawing.Size(48, 13);
            this.Label_ModelAddress.TabIndex = 4;
            this.Label_ModelAddress.Text = "Address:";
            // 
            // CheckBox_Hex
            // 
            this.CheckBox_Hex.AutoSize = true;
            this.CheckBox_Hex.Checked = true;
            this.CheckBox_Hex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_Hex.Location = new System.Drawing.Point(170, 91);
            this.CheckBox_Hex.Name = "CheckBox_Hex";
            this.CheckBox_Hex.Size = new System.Drawing.Size(45, 17);
            this.CheckBox_Hex.TabIndex = 9;
            this.CheckBox_Hex.Text = "Hex";
            this.CheckBox_Hex.UseVisualStyleBackColor = true;
            this.CheckBox_Hex.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
            // 
            // NumericUpDown_Key
            // 
            this.NumericUpDown_Key.Hexadecimal = true;
            this.NumericUpDown_Key.Location = new System.Drawing.Point(46, 64);
            this.NumericUpDown_Key.Name = "NumericUpDown_Key";
            this.NumericUpDown_Key.Size = new System.Drawing.Size(98, 20);
            this.NumericUpDown_Key.TabIndex = 15;
            // 
            // ComboBox_Format
            // 
            this.ComboBox_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Format.FormattingEnabled = true;
            this.ComboBox_Format.Items.AddRange(new object[] {
            "Basic",
            "SADX Basic",
            "Chunk",
            "GC"});
            this.ComboBox_Format.Location = new System.Drawing.Point(60, 165);
            this.ComboBox_Format.Name = "ComboBox_Format";
            this.ComboBox_Format.Size = new System.Drawing.Size(104, 21);
            this.ComboBox_Format.TabIndex = 17;
            // 
            // Label_Format
            // 
            this.Label_Format.AutoSize = true;
            this.Label_Format.Location = new System.Drawing.Point(12, 168);
            this.Label_Format.Name = "Label_Format";
            this.Label_Format.Size = new System.Drawing.Size(42, 13);
            this.Label_Format.TabIndex = 16;
            this.Label_Format.Text = "Format:";
            // 
            // CheckBox_LoadAnimation
            // 
            this.CheckBox_LoadAnimation.AutoSize = true;
            this.CheckBox_LoadAnimation.Location = new System.Drawing.Point(12, 116);
            this.CheckBox_LoadAnimation.Name = "CheckBox_LoadAnimation";
            this.CheckBox_LoadAnimation.Size = new System.Drawing.Size(99, 17);
            this.CheckBox_LoadAnimation.TabIndex = 18;
            this.CheckBox_LoadAnimation.Text = "Load Animation";
            this.CheckBox_LoadAnimation.UseVisualStyleBackColor = true;
            this.CheckBox_LoadAnimation.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // NumericUpDown_AnimationAddress
            // 
            this.NumericUpDown_AnimationAddress.Enabled = false;
            this.NumericUpDown_AnimationAddress.Hexadecimal = true;
            this.NumericUpDown_AnimationAddress.Location = new System.Drawing.Point(66, 139);
            this.NumericUpDown_AnimationAddress.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.NumericUpDown_AnimationAddress.Name = "NumericUpDown_AnimationAddress";
            this.NumericUpDown_AnimationAddress.Size = new System.Drawing.Size(98, 20);
            this.NumericUpDown_AnimationAddress.TabIndex = 19;
            // 
            // Label_AnimationAddress
            // 
            this.Label_AnimationAddress.AutoSize = true;
            this.Label_AnimationAddress.Location = new System.Drawing.Point(12, 141);
            this.Label_AnimationAddress.Name = "Label_AnimationAddress";
            this.Label_AnimationAddress.Size = new System.Drawing.Size(48, 13);
            this.Label_AnimationAddress.TabIndex = 20;
            this.Label_AnimationAddress.Text = "Address:";
            // 
            // CheckBox_BigEndian
            // 
            this.CheckBox_BigEndian.AutoSize = true;
            this.CheckBox_BigEndian.Location = new System.Drawing.Point(170, 167);
            this.CheckBox_BigEndian.Name = "CheckBox_BigEndian";
            this.CheckBox_BigEndian.Size = new System.Drawing.Size(77, 17);
            this.CheckBox_BigEndian.TabIndex = 21;
            this.CheckBox_BigEndian.Text = "Big Endian";
            this.CheckBox_BigEndian.UseVisualStyleBackColor = true;
            // 
            // typBinary
            // 
            this.typBinary.AutoSize = true;
            this.typBinary.Checked = true;
            this.typBinary.Location = new System.Drawing.Point(12, 28);
            this.typBinary.Name = "typBinary";
            this.typBinary.Size = new System.Drawing.Size(54, 17);
            this.typBinary.TabIndex = 22;
            this.typBinary.TabStop = true;
            this.typBinary.Text = "Binary";
            this.typBinary.UseVisualStyleBackColor = true;
            this.typBinary.CheckedChanged += new System.EventHandler(this.typBinary_CheckedChanged);
            // 
            // typSA2MDL
            // 
            this.typSA2MDL.AutoSize = true;
            this.typSA2MDL.Location = new System.Drawing.Point(75, 28);
            this.typSA2MDL.Name = "typSA2MDL";
            this.typSA2MDL.Size = new System.Drawing.Size(90, 17);
            this.typSA2MDL.TabIndex = 23;
            this.typSA2MDL.TabStop = true;
            this.typSA2MDL.Text = "SA2 MDL File";
            this.typSA2MDL.UseVisualStyleBackColor = true;
            this.typSA2MDL.CheckedChanged += new System.EventHandler(this.typSA2MDL_CheckedChanged);
            // 
            // typSA2BMDL
            // 
            this.typSA2BMDL.AutoSize = true;
            this.typSA2BMDL.Location = new System.Drawing.Point(174, 27);
            this.typSA2BMDL.Name = "typSA2BMDL";
            this.typSA2BMDL.Size = new System.Drawing.Size(97, 17);
            this.typSA2BMDL.TabIndex = 24;
            this.typSA2BMDL.TabStop = true;
            this.typSA2BMDL.Text = "SA2B MDL File";
            this.typSA2BMDL.UseVisualStyleBackColor = true;
            this.typSA2BMDL.CheckedChanged += new System.EventHandler(this.typSA2BMDL_CheckedChanged);
            // 
            // Label_Type
            // 
            this.Label_Type.AutoSize = true;
            this.Label_Type.Location = new System.Drawing.Point(12, 9);
            this.Label_Type.Name = "Label_Type";
            this.Label_Type.Size = new System.Drawing.Size(34, 13);
            this.Label_Type.TabIndex = 25;
            this.Label_Type.Text = "Type:";
            // 
            // CheckBox_Memory
            // 
            this.CheckBox_Memory.AutoSize = true;
            this.CheckBox_Memory.Location = new System.Drawing.Point(216, 91);
            this.CheckBox_Memory.Name = "CheckBox_Memory";
            this.CheckBox_Memory.Size = new System.Drawing.Size(63, 17);
            this.CheckBox_Memory.TabIndex = 26;
            this.CheckBox_Memory.Text = "Memory";
            this.CheckBox_Memory.UseVisualStyleBackColor = true;
            // 
            // ModelFileDialog
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 234);
            this.Controls.Add(this.CheckBox_Memory);
            this.Controls.Add(this.Label_Type);
            this.Controls.Add(this.typSA2BMDL);
            this.Controls.Add(this.typSA2MDL);
            this.Controls.Add(this.typBinary);
            this.Controls.Add(this.CheckBox_BigEndian);
            this.Controls.Add(this.Label_AnimationAddress);
            this.Controls.Add(this.NumericUpDown_AnimationAddress);
            this.Controls.Add(this.CheckBox_LoadAnimation);
            this.Controls.Add(this.OK_Button);
            this.Controls.Add(this.ComboBox_Format);
            this.Controls.Add(this.Label_Format);
            this.Controls.Add(this.NumericUpDown_Key);
            this.Controls.Add(this.CheckBox_Hex);
            this.Controls.Add(this.Label_ModelAddress);
            this.Controls.Add(this.NumericUpDown_ModelAddress);
            this.Controls.Add(this.ComboBox_FileType);
            this.Controls.Add(this.Label_Key);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelFileDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SA1/SA2 Model Editor";
            this.Load += new System.EventHandler(this.Dialog1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ModelAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Key)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_AnimationAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal Button OK_Button;
        internal Label Label_Key;
        internal ComboBox ComboBox_FileType;
        internal NumericUpDown NumericUpDown_ModelAddress;
        internal Label Label_ModelAddress;
        internal CheckBox CheckBox_Hex;
        internal SAEditorCommon.UI.HexNumericUpdown NumericUpDown_Key;
        internal ComboBox ComboBox_Format;
        internal Label Label_Format;
        internal NumericUpDown NumericUpDown_AnimationAddress;
        internal CheckBox CheckBox_LoadAnimation;
        private Label Label_AnimationAddress;
		internal CheckBox CheckBox_BigEndian;
		internal RadioButton typBinary;
		internal RadioButton typSA2MDL;
		internal RadioButton typSA2BMDL;
		private Label Label_Type;
		internal CheckBox CheckBox_Memory;
	}
}