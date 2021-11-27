
namespace SAModel.SAMDL
{
	partial class AssimpImportDialog
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
            this.radioButtonBasicModel = new System.Windows.Forms.RadioButton();
            this.radioButtonChunkModel = new System.Windows.Forms.RadioButton();
            this.radioButtonGCModel = new System.Windows.Forms.RadioButton();
            this.labelImportAs = new System.Windows.Forms.Label();
            this.radioButtonNodes = new System.Windows.Forms.RadioButton();
            this.radioButtonSingleModel = new System.Windows.Forms.RadioButton();
            this.groupBoxModelFormat = new System.Windows.Forms.GroupBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.checkBoxLegacyOBJ = new System.Windows.Forms.CheckBox();
            this.checkBoxImportColladaRootNode = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBoxModelFormat.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonBasicModel
            // 
            this.radioButtonBasicModel.AutoSize = true;
            this.radioButtonBasicModel.Checked = true;
            this.radioButtonBasicModel.Location = new System.Drawing.Point(6, 22);
            this.radioButtonBasicModel.Name = "radioButtonBasicModel";
            this.radioButtonBasicModel.Size = new System.Drawing.Size(89, 19);
            this.radioButtonBasicModel.TabIndex = 0;
            this.radioButtonBasicModel.TabStop = true;
            this.radioButtonBasicModel.Text = "Basic Model";
            this.radioButtonBasicModel.UseVisualStyleBackColor = true;
            // 
            // radioButtonChunkModel
            // 
            this.radioButtonChunkModel.AutoSize = true;
            this.radioButtonChunkModel.Location = new System.Drawing.Point(6, 47);
            this.radioButtonChunkModel.Name = "radioButtonChunkModel";
            this.radioButtonChunkModel.Size = new System.Drawing.Size(97, 19);
            this.radioButtonChunkModel.TabIndex = 1;
            this.radioButtonChunkModel.Text = "Chunk Model";
            this.radioButtonChunkModel.UseVisualStyleBackColor = true;
            // 
            // radioButtonGCModel
            // 
            this.radioButtonGCModel.AutoSize = true;
            this.radioButtonGCModel.Location = new System.Drawing.Point(6, 72);
            this.radioButtonGCModel.Name = "radioButtonGCModel";
            this.radioButtonGCModel.Size = new System.Drawing.Size(78, 19);
            this.radioButtonGCModel.TabIndex = 2;
            this.radioButtonGCModel.Text = "GC Model";
            this.radioButtonGCModel.UseVisualStyleBackColor = true;
            // 
            // labelImportAs
            // 
            this.labelImportAs.AutoSize = true;
            this.labelImportAs.Location = new System.Drawing.Point(18, 119);
            this.labelImportAs.Name = "labelImportAs";
            this.labelImportAs.Size = new System.Drawing.Size(60, 15);
            this.labelImportAs.TabIndex = 4;
            this.labelImportAs.Text = "Import as:";
            // 
            // radioButtonNodes
            // 
            this.radioButtonNodes.AutoSize = true;
            this.radioButtonNodes.Checked = true;
            this.radioButtonNodes.Location = new System.Drawing.Point(18, 137);
            this.radioButtonNodes.Name = "radioButtonNodes";
            this.radioButtonNodes.Size = new System.Drawing.Size(59, 19);
            this.radioButtonNodes.TabIndex = 5;
            this.radioButtonNodes.TabStop = true;
            this.radioButtonNodes.Text = "Nodes";
            this.radioButtonNodes.UseVisualStyleBackColor = true;
            // 
            // radioButtonSingleModel
            // 
            this.radioButtonSingleModel.AutoSize = true;
            this.radioButtonSingleModel.Location = new System.Drawing.Point(18, 162);
            this.radioButtonSingleModel.Name = "radioButtonSingleModel";
            this.radioButtonSingleModel.Size = new System.Drawing.Size(94, 19);
            this.radioButtonSingleModel.TabIndex = 6;
            this.radioButtonSingleModel.Text = "Single Model";
            this.radioButtonSingleModel.UseVisualStyleBackColor = true;
            // 
            // groupBoxModelFormat
            // 
            this.groupBoxModelFormat.Controls.Add(this.radioButtonBasicModel);
            this.groupBoxModelFormat.Controls.Add(this.radioButtonChunkModel);
            this.groupBoxModelFormat.Controls.Add(this.radioButtonGCModel);
            this.groupBoxModelFormat.Location = new System.Drawing.Point(12, 12);
            this.groupBoxModelFormat.Name = "groupBoxModelFormat";
            this.groupBoxModelFormat.Size = new System.Drawing.Size(164, 104);
            this.groupBoxModelFormat.TabIndex = 7;
            this.groupBoxModelFormat.TabStop = false;
            this.groupBoxModelFormat.Text = "Model Format";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(15, 245);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // checkBoxLegacyOBJ
            // 
            this.checkBoxLegacyOBJ.AutoSize = true;
            this.checkBoxLegacyOBJ.Location = new System.Drawing.Point(18, 215);
            this.checkBoxLegacyOBJ.Name = "checkBoxLegacyOBJ";
            this.checkBoxLegacyOBJ.Size = new System.Drawing.Size(125, 19);
            this.checkBoxLegacyOBJ.TabIndex = 13;
            this.checkBoxLegacyOBJ.Text = "Legacy OBJ Import";
            this.checkBoxLegacyOBJ.UseVisualStyleBackColor = true;
            this.checkBoxLegacyOBJ.CheckedChanged += new System.EventHandler(this.checkBoxLegacyOBJ_CheckedChanged);
            // 
            // checkBoxImportColladaRootNode
            // 
            this.checkBoxImportColladaRootNode.AutoSize = true;
            this.checkBoxImportColladaRootNode.Location = new System.Drawing.Point(18, 190);
            this.checkBoxImportColladaRootNode.Name = "checkBoxImportColladaRootNode";
            this.checkBoxImportColladaRootNode.Size = new System.Drawing.Size(122, 19);
            this.checkBoxImportColladaRootNode.TabIndex = 14;
            this.checkBoxImportColladaRootNode.Text = "Import Root Node";
            this.checkBoxImportColladaRootNode.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(108, 245);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // AssimpImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(195, 280);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBoxImportColladaRootNode);
            this.Controls.Add(this.checkBoxLegacyOBJ);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxModelFormat);
            this.Controls.Add(this.radioButtonSingleModel);
            this.Controls.Add(this.radioButtonNodes);
            this.Controls.Add(this.labelImportAs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssimpImportDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Model Import";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AssimpImport_HelpButtonClicked);
            this.groupBoxModelFormat.ResumeLayout(false);
            this.groupBoxModelFormat.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButtonBasicModel;
		private System.Windows.Forms.RadioButton radioButtonChunkModel;
		private System.Windows.Forms.RadioButton radioButtonGCModel;
		private System.Windows.Forms.Label labelImportAs;
		private System.Windows.Forms.RadioButton radioButtonNodes;
		private System.Windows.Forms.RadioButton radioButtonSingleModel;
		private System.Windows.Forms.GroupBox groupBoxModelFormat;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.CheckBox checkBoxLegacyOBJ;
		private System.Windows.Forms.CheckBox checkBoxImportColladaRootNode;
		private System.Windows.Forms.Button button1;
	}
}