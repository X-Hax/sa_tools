namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	partial class WelcomeForm
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
            this.WelcomeLabel = new System.Windows.Forms.Label();
            this.showOnStartCheckbox = new System.Windows.Forms.CheckBox();
            this.doneButton = new System.Windows.Forms.Button();
            this.IntroLabel = new System.Windows.Forms.Label();
            this.WikiDocLabel = new System.Windows.Forms.LinkLabel();
            this.discordLinkLabel = new System.Windows.Forms.LinkLabel();
            this.ThisToolLink = new System.Windows.Forms.LinkLabel();
            this.controlsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // WelcomeLabel
            // 
            this.WelcomeLabel.AutoSize = true;
            this.WelcomeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WelcomeLabel.Location = new System.Drawing.Point(9, 9);
            this.WelcomeLabel.Name = "WelcomeLabel";
            this.WelcomeLabel.Size = new System.Drawing.Size(281, 31);
            this.WelcomeLabel.TabIndex = 0;
            this.WelcomeLabel.Text = "Welcome to SA Tools!";
            // 
            // showOnStartCheckbox
            // 
            this.showOnStartCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.showOnStartCheckbox.AutoSize = true;
            this.showOnStartCheckbox.Checked = true;
            this.showOnStartCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showOnStartCheckbox.Location = new System.Drawing.Point(14, 228);
            this.showOnStartCheckbox.Name = "showOnStartCheckbox";
            this.showOnStartCheckbox.Size = new System.Drawing.Size(167, 17);
            this.showOnStartCheckbox.TabIndex = 1;
            this.showOnStartCheckbox.Text = "Show this message on startup";
            this.showOnStartCheckbox.UseVisualStyleBackColor = true;
            // 
            // doneButton
            // 
            this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.doneButton.Location = new System.Drawing.Point(439, 222);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 2;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // IntroLabel
            // 
            this.IntroLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IntroLabel.AutoSize = true;
            this.IntroLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IntroLabel.Location = new System.Drawing.Point(12, 57);
            this.IntroLabel.Name = "IntroLabel";
            this.IntroLabel.Size = new System.Drawing.Size(508, 18);
            this.IntroLabel.TabIndex = 3;
            this.IntroLabel.Text = "If this is your first time using SA Tools, make sure to check out the following:";
            // 
            // WikiDocLabel
            // 
            this.WikiDocLabel.AutoSize = true;
            this.WikiDocLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WikiDocLabel.Location = new System.Drawing.Point(10, 119);
            this.WikiDocLabel.Name = "WikiDocLabel";
            this.WikiDocLabel.Size = new System.Drawing.Size(241, 24);
            this.WikiDocLabel.TabIndex = 4;
            this.WikiDocLabel.TabStop = true;
            this.WikiDocLabel.Text = "SA Tools Documentation";
            this.WikiDocLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WikiDocLabel_LinkClicked);
            // 
            // discordLinkLabel
            // 
            this.discordLinkLabel.AutoSize = true;
            this.discordLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.discordLinkLabel.Location = new System.Drawing.Point(10, 152);
            this.discordLinkLabel.Name = "discordLinkLabel";
            this.discordLinkLabel.Size = new System.Drawing.Size(148, 24);
            this.discordLinkLabel.TabIndex = 5;
            this.discordLinkLabel.TabStop = true;
            this.discordLinkLabel.Text = "Discord Server";
            this.discordLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.discordLinkLabel_LinkClicked);
            // 
            // ThisToolLink
            // 
            this.ThisToolLink.AutoSize = true;
            this.ThisToolLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ThisToolLink.Location = new System.Drawing.Point(9, 185);
            this.ThisToolLink.Name = "ThisToolLink";
            this.ThisToolLink.Size = new System.Drawing.Size(279, 24);
            this.ThisToolLink.TabIndex = 6;
            this.ThisToolLink.TabStop = true;
            this.ThisToolLink.Text = "Tool-Specific Documentation";
            this.ThisToolLink.Visible = false;
            // 
            // controlsLinkLabel
            // 
            this.controlsLinkLabel.AutoSize = true;
            this.controlsLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controlsLinkLabel.Location = new System.Drawing.Point(10, 86);
            this.controlsLinkLabel.Name = "controlsLinkLabel";
            this.controlsLinkLabel.Size = new System.Drawing.Size(179, 24);
            this.controlsLinkLabel.TabIndex = 7;
            this.controlsLinkLabel.TabStop = true;
            this.controlsLinkLabel.Text = "3D Editor Controls";
            this.controlsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.controlsLinkLabel_LinkClicked);
            // 
            // WelcomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 257);
            this.ControlBox = false;
            this.Controls.Add(this.controlsLinkLabel);
            this.Controls.Add(this.ThisToolLink);
            this.Controls.Add(this.discordLinkLabel);
            this.Controls.Add(this.WikiDocLabel);
            this.Controls.Add(this.IntroLabel);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.showOnStartCheckbox);
            this.Controls.Add(this.WelcomeLabel);
            this.MinimumSize = new System.Drawing.Size(539, 278);
            this.Name = "WelcomeForm";
            this.Text = "SA Tools Documentation";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label WelcomeLabel;
		private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.Label IntroLabel;
		private System.Windows.Forms.LinkLabel WikiDocLabel;
		private System.Windows.Forms.LinkLabel discordLinkLabel;
		public System.Windows.Forms.CheckBox showOnStartCheckbox;
		public System.Windows.Forms.LinkLabel ThisToolLink;
		private System.Windows.Forms.LinkLabel controlsLinkLabel;
	}
}