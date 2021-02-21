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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelFileDialog));
            this.OK_Button = new System.Windows.Forms.Button();
            this.Label_Key = new System.Windows.Forms.Label();
            this.ComboBox_FileType = new System.Windows.Forms.ComboBox();
            this.Label_ModelAddress = new System.Windows.Forms.Label();
            this.CheckBox_Hex_Object = new System.Windows.Forms.CheckBox();
            this.ComboBox_Format = new System.Windows.Forms.ComboBox();
            this.Label_Format = new System.Windows.Forms.Label();
            this.CheckBox_LoadMotion = new System.Windows.Forms.CheckBox();
            this.Label_MotionAddress = new System.Windows.Forms.Label();
            this.CheckBox_BigEndian = new System.Windows.Forms.CheckBox();
            this.RadioButton_Binary = new System.Windows.Forms.RadioButton();
            this.RadioButton_SA2MDL = new System.Windows.Forms.RadioButton();
            this.RadioButton_SA2BMDL = new System.Windows.Forms.RadioButton();
            this.Label_Type = new System.Windows.Forms.Label();
            this.CheckBox_Memory_Object = new System.Windows.Forms.CheckBox();
            this.Label_Structure = new System.Windows.Forms.Label();
            this.RadioButton_Object = new System.Windows.Forms.RadioButton();
            this.RadioButton_Action = new System.Windows.Forms.RadioButton();
            this.CheckBox_Memory_Motion = new System.Windows.Forms.CheckBox();
            this.CheckBox_Hex_Motion = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RadioButton_Attach = new System.Windows.Forms.RadioButton();
            this.NumericUpDown_Key = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.NumericUpDown_MotionAddress = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.NumericUpDown_ObjectAddress = new SonicRetro.SAModel.SAEditorCommon.UI.HexNumericUpdown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Key)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_MotionAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ObjectAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // OK_Button
            // 
            this.OK_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK_Button.Location = new System.Drawing.Point(263, 261);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(67, 23);
            this.OK_Button.TabIndex = 0;
            this.OK_Button.Text = "&OK";
            // 
            // Label_Key
            // 
            this.Label_Key.AutoSize = true;
            this.Label_Key.Location = new System.Drawing.Point(32, 21);
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
            this.ComboBox_FileType.Location = new System.Drawing.Point(182, 19);
            this.ComboBox_FileType.Name = "ComboBox_FileType";
            this.ComboBox_FileType.Size = new System.Drawing.Size(121, 21);
            this.ComboBox_FileType.TabIndex = 2;
            this.ComboBox_FileType.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // Label_ModelAddress
            // 
            this.Label_ModelAddress.AutoSize = true;
            this.Label_ModelAddress.Location = new System.Drawing.Point(12, 57);
            this.Label_ModelAddress.Name = "Label_ModelAddress";
            this.Label_ModelAddress.Size = new System.Drawing.Size(48, 13);
            this.Label_ModelAddress.TabIndex = 4;
            this.Label_ModelAddress.Text = "Address:";
            // 
            // CheckBox_Hex_Object
            // 
            this.CheckBox_Hex_Object.AutoSize = true;
            this.CheckBox_Hex_Object.Checked = true;
            this.CheckBox_Hex_Object.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_Hex_Object.Location = new System.Drawing.Point(183, 56);
            this.CheckBox_Hex_Object.Name = "CheckBox_Hex_Object";
            this.CheckBox_Hex_Object.Size = new System.Drawing.Size(45, 17);
            this.CheckBox_Hex_Object.TabIndex = 9;
            this.CheckBox_Hex_Object.Text = "Hex";
            this.CheckBox_Hex_Object.UseVisualStyleBackColor = true;
            this.CheckBox_Hex_Object.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
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
            this.ComboBox_Format.Location = new System.Drawing.Point(66, 92);
            this.ComboBox_Format.Name = "ComboBox_Format";
            this.ComboBox_Format.Size = new System.Drawing.Size(98, 21);
            this.ComboBox_Format.TabIndex = 17;
            // 
            // Label_Format
            // 
            this.Label_Format.AutoSize = true;
            this.Label_Format.Location = new System.Drawing.Point(18, 95);
            this.Label_Format.Name = "Label_Format";
            this.Label_Format.Size = new System.Drawing.Size(42, 13);
            this.Label_Format.TabIndex = 16;
            this.Label_Format.Text = "Format:";
            // 
            // CheckBox_LoadMotion
            // 
            this.CheckBox_LoadMotion.AutoSize = true;
            this.CheckBox_LoadMotion.Location = new System.Drawing.Point(10, 155);
            this.CheckBox_LoadMotion.Name = "CheckBox_LoadMotion";
            this.CheckBox_LoadMotion.Size = new System.Drawing.Size(85, 17);
            this.CheckBox_LoadMotion.TabIndex = 18;
            this.CheckBox_LoadMotion.Text = "Load Motion";
            this.CheckBox_LoadMotion.UseVisualStyleBackColor = true;
            this.CheckBox_LoadMotion.CheckedChanged += new System.EventHandler(this.CheckBox_LoadMotion_CheckedChanged);
            // 
            // Label_MotionAddress
            // 
            this.Label_MotionAddress.AutoSize = true;
            this.Label_MotionAddress.Location = new System.Drawing.Point(10, 180);
            this.Label_MotionAddress.Name = "Label_MotionAddress";
            this.Label_MotionAddress.Size = new System.Drawing.Size(48, 13);
            this.Label_MotionAddress.TabIndex = 20;
            this.Label_MotionAddress.Text = "Address:";
            // 
            // CheckBox_BigEndian
            // 
            this.CheckBox_BigEndian.AutoSize = true;
            this.CheckBox_BigEndian.Location = new System.Drawing.Point(183, 94);
            this.CheckBox_BigEndian.Name = "CheckBox_BigEndian";
            this.CheckBox_BigEndian.Size = new System.Drawing.Size(77, 17);
            this.CheckBox_BigEndian.TabIndex = 21;
            this.CheckBox_BigEndian.Text = "Big Endian";
            this.CheckBox_BigEndian.UseVisualStyleBackColor = true;
            // 
            // RadioButton_Binary
            // 
            this.RadioButton_Binary.AutoSize = true;
            this.RadioButton_Binary.Checked = true;
            this.RadioButton_Binary.Location = new System.Drawing.Point(52, 12);
            this.RadioButton_Binary.Name = "RadioButton_Binary";
            this.RadioButton_Binary.Size = new System.Drawing.Size(54, 17);
            this.RadioButton_Binary.TabIndex = 22;
            this.RadioButton_Binary.TabStop = true;
            this.RadioButton_Binary.Text = "Binary";
            this.RadioButton_Binary.UseVisualStyleBackColor = true;
            this.RadioButton_Binary.CheckedChanged += new System.EventHandler(this.typBinary_CheckedChanged);
            // 
            // RadioButton_SA2MDL
            // 
            this.RadioButton_SA2MDL.AutoSize = true;
            this.RadioButton_SA2MDL.Location = new System.Drawing.Point(112, 12);
            this.RadioButton_SA2MDL.Name = "RadioButton_SA2MDL";
            this.RadioButton_SA2MDL.Size = new System.Drawing.Size(90, 17);
            this.RadioButton_SA2MDL.TabIndex = 23;
            this.RadioButton_SA2MDL.Text = "SA2 MDL File";
            this.RadioButton_SA2MDL.UseVisualStyleBackColor = true;
            // 
            // RadioButton_SA2BMDL
            // 
            this.RadioButton_SA2BMDL.AutoSize = true;
            this.RadioButton_SA2BMDL.Location = new System.Drawing.Point(205, 12);
            this.RadioButton_SA2BMDL.Name = "RadioButton_SA2BMDL";
            this.RadioButton_SA2BMDL.Size = new System.Drawing.Size(97, 17);
            this.RadioButton_SA2BMDL.TabIndex = 24;
            this.RadioButton_SA2BMDL.Text = "SA2B MDL File";
            this.RadioButton_SA2BMDL.UseVisualStyleBackColor = true;
            // 
            // Label_Type
            // 
            this.Label_Type.AutoSize = true;
            this.Label_Type.Location = new System.Drawing.Point(12, 14);
            this.Label_Type.Name = "Label_Type";
            this.Label_Type.Size = new System.Drawing.Size(34, 13);
            this.Label_Type.TabIndex = 25;
            this.Label_Type.Text = "Type:";
            // 
            // CheckBox_Memory_Object
            // 
            this.CheckBox_Memory_Object.AutoSize = true;
            this.CheckBox_Memory_Object.Location = new System.Drawing.Point(229, 56);
            this.CheckBox_Memory_Object.Name = "CheckBox_Memory_Object";
            this.CheckBox_Memory_Object.Size = new System.Drawing.Size(63, 17);
            this.CheckBox_Memory_Object.TabIndex = 26;
            this.CheckBox_Memory_Object.Text = "Memory";
            this.CheckBox_Memory_Object.UseVisualStyleBackColor = true;
            // 
            // Label_Structure
            // 
            this.Label_Structure.AutoSize = true;
            this.Label_Structure.Location = new System.Drawing.Point(7, 128);
            this.Label_Structure.Name = "Label_Structure";
            this.Label_Structure.Size = new System.Drawing.Size(53, 13);
            this.Label_Structure.TabIndex = 27;
            this.Label_Structure.Text = "Structure:";
            // 
            // RadioButton_Object
            // 
            this.RadioButton_Object.AutoSize = true;
            this.RadioButton_Object.Checked = true;
            this.RadioButton_Object.Location = new System.Drawing.Point(64, 126);
            this.RadioButton_Object.Name = "RadioButton_Object";
            this.RadioButton_Object.Size = new System.Drawing.Size(56, 17);
            this.RadioButton_Object.TabIndex = 28;
            this.RadioButton_Object.TabStop = true;
            this.RadioButton_Object.Text = "Object";
            this.RadioButton_Object.UseVisualStyleBackColor = true;
            this.RadioButton_Object.CheckedChanged += new System.EventHandler(this.RadioButton_Object_CheckedChanged);
            // 
            // RadioButton_Action
            // 
            this.RadioButton_Action.AutoSize = true;
            this.RadioButton_Action.Location = new System.Drawing.Point(126, 126);
            this.RadioButton_Action.Name = "RadioButton_Action";
            this.RadioButton_Action.Size = new System.Drawing.Size(55, 17);
            this.RadioButton_Action.TabIndex = 29;
            this.RadioButton_Action.Text = "Action";
            this.RadioButton_Action.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Memory_Motion
            // 
            this.CheckBox_Memory_Motion.AutoSize = true;
            this.CheckBox_Memory_Motion.Location = new System.Drawing.Point(228, 180);
            this.CheckBox_Memory_Motion.Name = "CheckBox_Memory_Motion";
            this.CheckBox_Memory_Motion.Size = new System.Drawing.Size(63, 17);
            this.CheckBox_Memory_Motion.TabIndex = 30;
            this.CheckBox_Memory_Motion.Text = "Memory";
            this.CheckBox_Memory_Motion.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Hex_Motion
            // 
            this.CheckBox_Hex_Motion.AutoSize = true;
            this.CheckBox_Hex_Motion.Checked = true;
            this.CheckBox_Hex_Motion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_Hex_Motion.Location = new System.Drawing.Point(181, 180);
            this.CheckBox_Hex_Motion.Name = "CheckBox_Hex_Motion";
            this.CheckBox_Hex_Motion.Size = new System.Drawing.Size(45, 17);
            this.CheckBox_Hex_Motion.TabIndex = 31;
            this.CheckBox_Hex_Motion.Text = "Hex";
            this.CheckBox_Hex_Motion.UseVisualStyleBackColor = true;
            this.CheckBox_Hex_Motion.CheckedChanged += new System.EventHandler(this.CheckBox_Hex_Motion_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RadioButton_Attach);
            this.groupBox1.Controls.Add(this.CheckBox_Hex_Motion);
            this.groupBox1.Controls.Add(this.ComboBox_Format);
            this.groupBox1.Controls.Add(this.CheckBox_LoadMotion);
            this.groupBox1.Controls.Add(this.NumericUpDown_Key);
            this.groupBox1.Controls.Add(this.CheckBox_Memory_Motion);
            this.groupBox1.Controls.Add(this.Label_Format);
            this.groupBox1.Controls.Add(this.NumericUpDown_MotionAddress);
            this.groupBox1.Controls.Add(this.Label_MotionAddress);
            this.groupBox1.Controls.Add(this.RadioButton_Action);
            this.groupBox1.Controls.Add(this.Label_Key);
            this.groupBox1.Controls.Add(this.CheckBox_Memory_Object);
            this.groupBox1.Controls.Add(this.RadioButton_Object);
            this.groupBox1.Controls.Add(this.ComboBox_FileType);
            this.groupBox1.Controls.Add(this.CheckBox_BigEndian);
            this.groupBox1.Controls.Add(this.Label_Structure);
            this.groupBox1.Controls.Add(this.NumericUpDown_ObjectAddress);
            this.groupBox1.Controls.Add(this.Label_ModelAddress);
            this.groupBox1.Controls.Add(this.CheckBox_Hex_Object);
            this.groupBox1.Location = new System.Drawing.Point(12, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(315, 215);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Binary data";
            // 
            // RadioButton_Attach
            // 
            this.RadioButton_Attach.AutoSize = true;
            this.RadioButton_Attach.Location = new System.Drawing.Point(187, 126);
            this.RadioButton_Attach.Name = "RadioButton_Attach";
            this.RadioButton_Attach.Size = new System.Drawing.Size(94, 17);
            this.RadioButton_Attach.TabIndex = 32;
            this.RadioButton_Attach.Text = "Model (Attach)";
            this.RadioButton_Attach.UseVisualStyleBackColor = true;
            // 
            // NumericUpDown_Key
            // 
            this.NumericUpDown_Key.Hexadecimal = true;
            this.NumericUpDown_Key.Location = new System.Drawing.Point(66, 19);
            this.NumericUpDown_Key.Name = "NumericUpDown_Key";
            this.NumericUpDown_Key.Size = new System.Drawing.Size(98, 20);
            this.NumericUpDown_Key.TabIndex = 15;
            // 
            // NumericUpDown_MotionAddress
            // 
            this.NumericUpDown_MotionAddress.Enabled = false;
            this.NumericUpDown_MotionAddress.Hexadecimal = true;
            this.NumericUpDown_MotionAddress.Location = new System.Drawing.Point(64, 178);
            this.NumericUpDown_MotionAddress.Name = "NumericUpDown_MotionAddress";
            this.NumericUpDown_MotionAddress.Size = new System.Drawing.Size(98, 20);
            this.NumericUpDown_MotionAddress.TabIndex = 19;
            // 
            // NumericUpDown_ObjectAddress
            // 
            this.NumericUpDown_ObjectAddress.Hexadecimal = true;
            this.NumericUpDown_ObjectAddress.Location = new System.Drawing.Point(66, 55);
            this.NumericUpDown_ObjectAddress.Name = "NumericUpDown_ObjectAddress";
            this.NumericUpDown_ObjectAddress.Size = new System.Drawing.Size(98, 20);
            this.NumericUpDown_ObjectAddress.TabIndex = 3;
            // 
            // ModelFileDialog
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 297);
            this.Controls.Add(this.Label_Type);
            this.Controls.Add(this.RadioButton_Binary);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.RadioButton_SA2MDL);
            this.Controls.Add(this.RadioButton_SA2BMDL);
            this.Controls.Add(this.OK_Button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelFileDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Load from a Binary File";
            this.Load += new System.EventHandler(this.Dialog1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Key)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_MotionAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_ObjectAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal Button OK_Button;
        internal Label Label_Key;
        internal ComboBox ComboBox_FileType;
        internal Label Label_ModelAddress;
        internal CheckBox CheckBox_Hex_Object;
        internal SAEditorCommon.UI.HexNumericUpdown NumericUpDown_Key;
        internal ComboBox ComboBox_Format;
        internal Label Label_Format;
        internal CheckBox CheckBox_LoadMotion;
        private Label Label_MotionAddress;
		internal CheckBox CheckBox_BigEndian;
		internal RadioButton RadioButton_Binary;
		internal RadioButton RadioButton_SA2MDL;
		internal RadioButton RadioButton_SA2BMDL;
		private Label Label_Type;
		internal CheckBox CheckBox_Memory_Object;
		internal Label Label_Structure;
		internal RadioButton RadioButton_Object;
		internal RadioButton RadioButton_Action;
		internal CheckBox CheckBox_Memory_Motion;
		internal CheckBox CheckBox_Hex_Motion;
		private GroupBox groupBox1;
		internal RadioButton RadioButton_Attach;
		internal SAEditorCommon.UI.HexNumericUpdown NumericUpDown_ObjectAddress;
		internal SAEditorCommon.UI.HexNumericUpdown NumericUpDown_MotionAddress;
	}
}