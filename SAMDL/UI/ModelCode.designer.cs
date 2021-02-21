namespace SonicRetro.SAModel.SAMDL
{
	partial class ModelText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelText));
            this.buttonCloseModelText = new System.Windows.Forms.Button();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonCloseModelText
            // 
            this.buttonCloseModelText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCloseModelText.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCloseModelText.Location = new System.Drawing.Point(697, 406);
            this.buttonCloseModelText.Name = "buttonCloseModelText";
            this.buttonCloseModelText.Size = new System.Drawing.Size(75, 23);
            this.buttonCloseModelText.TabIndex = 1;
            this.buttonCloseModelText.Text = "Close";
            this.buttonCloseModelText.UseVisualStyleBackColor = true;
            this.buttonCloseModelText.Click += new System.EventHandler(this.buttonCloseModelText_Click);
            // 
            // txtModel
            // 
            this.txtModel.Location = new System.Drawing.Point(12, 12);
            this.txtModel.Multiline = true;
            this.txtModel.Name = "txtModel";
            this.txtModel.ReadOnly = true;
            this.txtModel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtModel.Size = new System.Drawing.Size(760, 381);
            this.txtModel.TabIndex = 3;
            // 
            // ModelText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.txtModel);
            this.Controls.Add(this.buttonCloseModelText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelText";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Model Code";
            this.Shown += new System.EventHandler(this.ModelText_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button buttonCloseModelText;
		private System.Windows.Forms.TextBox txtModel;
	}
}

