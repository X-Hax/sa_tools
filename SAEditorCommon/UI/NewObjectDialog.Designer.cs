namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	partial class NewObjectDialog
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
			this.objLstPanel = new System.Windows.Forms.Panel();
			this.objLstMission = new System.Windows.Forms.RadioButton();
			this.objLstLevel = new System.Windows.Forms.RadioButton();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			this.objLstPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 14);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(26, 13);
			label1.TabIndex = 0;
			label1.Text = "List:";
			// 
			// objLstPanel
			// 
			this.objLstPanel.AutoSize = true;
			this.objLstPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.objLstPanel.Controls.Add(this.objLstMission);
			this.objLstPanel.Controls.Add(this.objLstLevel);
			this.objLstPanel.Controls.Add(label1);
			this.objLstPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.objLstPanel.Location = new System.Drawing.Point(0, 0);
			this.objLstPanel.Name = "objLstPanel";
			this.objLstPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.objLstPanel.Size = new System.Drawing.Size(284, 35);
			this.objLstPanel.TabIndex = 0;
			// 
			// objLstMission
			// 
			this.objLstMission.AutoSize = true;
			this.objLstMission.Checked = true;
			this.objLstMission.Location = new System.Drawing.Point(92, 12);
			this.objLstMission.Name = "objLstMission";
			this.objLstMission.Size = new System.Drawing.Size(60, 17);
			this.objLstMission.TabIndex = 2;
			this.objLstMission.TabStop = true;
			this.objLstMission.Text = "&Mission";
			this.objLstMission.UseVisualStyleBackColor = true;
			// 
			// objLstLevel
			// 
			this.objLstLevel.AutoSize = true;
			this.objLstLevel.Location = new System.Drawing.Point(35, 12);
			this.objLstLevel.Name = "objLstLevel";
			this.objLstLevel.Size = new System.Drawing.Size(51, 17);
			this.objLstLevel.TabIndex = 1;
			this.objLstLevel.Text = "&Level";
			this.objLstLevel.UseVisualStyleBackColor = true;
			this.objLstLevel.CheckedChanged += new System.EventHandler(this.objLst_CheckedChanged);
			// 
			// listBox1
			// 
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(0, 35);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(284, 198);
			this.listBox1.TabIndex = 1;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.okButton);
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Controls.Add(this.numericUpDown1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 233);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(284, 29);
			this.panel1.TabIndex = 2;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(116, 3);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "&OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(197, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(12, 6);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(56, 20);
			this.numericUpDown1.TabIndex = 0;
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// NewObjectDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.objLstPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewObjectDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "New Object";
			this.Load += new System.EventHandler(this.NewObjectDialog_Load);
			this.objLstPanel.ResumeLayout(false);
			this.objLstPanel.PerformLayout();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel objLstPanel;
		private System.Windows.Forms.RadioButton objLstMission;
		private System.Windows.Forms.RadioButton objLstLevel;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Button okButton;
	}
}