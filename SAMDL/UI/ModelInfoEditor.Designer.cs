namespace SAModel.SAMDL.UI
{
	partial class ModelInfoEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelInfoEditor));
            this.buttonApplyAnimation = new System.Windows.Forms.Button();
            this.textBoxAnimation = new System.Windows.Forms.TextBox();
            this.buttonClearAnimations = new System.Windows.Forms.Button();
            this.buttonRemoveAnimation = new System.Windows.Forms.Button();
            this.buttonAddAnimation = new System.Windows.Forms.Button();
            this.listBoxAnimations = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.textBoxAuthor = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonApplyAnimation
            // 
            this.buttonApplyAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApplyAnimation.Location = new System.Drawing.Point(318, 520);
            this.buttonApplyAnimation.Name = "buttonApplyAnimation";
            this.buttonApplyAnimation.Size = new System.Drawing.Size(71, 32);
            this.buttonApplyAnimation.TabIndex = 6;
            this.buttonApplyAnimation.Text = "Apply";
            this.buttonApplyAnimation.UseVisualStyleBackColor = true;
            this.buttonApplyAnimation.Click += new System.EventHandler(this.buttonApplyAnimation_Click);
            // 
            // textBoxAnimation
            // 
            this.textBoxAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAnimation.Location = new System.Drawing.Point(17, 477);
            this.textBoxAnimation.Name = "textBoxAnimation";
            this.textBoxAnimation.Size = new System.Drawing.Size(373, 31);
            this.textBoxAnimation.TabIndex = 3;
            // 
            // buttonClearAnimations
            // 
            this.buttonClearAnimations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearAnimations.Location = new System.Drawing.Point(395, 520);
            this.buttonClearAnimations.Name = "buttonClearAnimations";
            this.buttonClearAnimations.Size = new System.Drawing.Size(71, 32);
            this.buttonClearAnimations.TabIndex = 7;
            this.buttonClearAnimations.Text = "Clear";
            this.buttonClearAnimations.UseVisualStyleBackColor = true;
            this.buttonClearAnimations.Click += new System.EventHandler(this.buttonClearAnimations_Click);
            // 
            // buttonRemoveAnimation
            // 
            this.buttonRemoveAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveAnimation.Location = new System.Drawing.Point(434, 477);
            this.buttonRemoveAnimation.Name = "buttonRemoveAnimation";
            this.buttonRemoveAnimation.Size = new System.Drawing.Size(32, 32);
            this.buttonRemoveAnimation.TabIndex = 5;
            this.buttonRemoveAnimation.Text = "-";
            this.buttonRemoveAnimation.UseVisualStyleBackColor = true;
            this.buttonRemoveAnimation.Click += new System.EventHandler(this.buttonRemoveAnimation_Click);
            // 
            // buttonAddAnimation
            // 
            this.buttonAddAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddAnimation.Location = new System.Drawing.Point(396, 477);
            this.buttonAddAnimation.Name = "buttonAddAnimation";
            this.buttonAddAnimation.Size = new System.Drawing.Size(32, 32);
            this.buttonAddAnimation.TabIndex = 4;
            this.buttonAddAnimation.Text = "+";
            this.buttonAddAnimation.UseVisualStyleBackColor = true;
            this.buttonAddAnimation.Click += new System.EventHandler(this.buttonAddAnimation_Click);
            // 
            // listBoxAnimations
            // 
            this.listBoxAnimations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAnimations.FormattingEnabled = true;
            this.listBoxAnimations.ItemHeight = 25;
            this.listBoxAnimations.Location = new System.Drawing.Point(17, 117);
            this.listBoxAnimations.Name = "listBoxAnimations";
            this.listBoxAnimations.Size = new System.Drawing.Size(445, 354);
            this.listBoxAnimations.TabIndex = 2;
            this.listBoxAnimations.SelectedIndexChanged += new System.EventHandler(this.listBoxAnimations_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 25);
            this.label4.TabIndex = 5;
            this.label4.Text = "Animations:";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(134, 49);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(329, 31);
            this.textBoxDescription.TabIndex = 1;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // textBoxAuthor
            // 
            this.textBoxAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAuthor.Location = new System.Drawing.Point(134, 12);
            this.textBoxAuthor.Name = "textBoxAuthor";
            this.textBoxAuthor.Size = new System.Drawing.Size(329, 31);
            this.textBoxAuthor.TabIndex = 0;
            this.textBoxAuthor.TextChanged += new System.EventHandler(this.textBoxAuthor_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Description:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Author:";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(354, 568);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(112, 38);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // ModelInfoEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(478, 618);
            this.Controls.Add(this.buttonApplyAnimation);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textBoxAnimation);
            this.Controls.Add(this.buttonClearAnimations);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonRemoveAnimation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonAddAnimation);
            this.Controls.Add(this.textBoxAuthor);
            this.Controls.Add(this.listBoxAnimations);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label4);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ModelInfoEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Model Info";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox listBoxAnimations;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.TextBox textBoxAuthor;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Button buttonClearAnimations;
		private System.Windows.Forms.Button buttonRemoveAnimation;
		private System.Windows.Forms.Button buttonAddAnimation;
		private System.Windows.Forms.Button buttonApplyAnimation;
		private System.Windows.Forms.TextBox textBoxAnimation;
	}
}