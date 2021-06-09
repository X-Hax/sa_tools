
namespace SAToolsHub
{
	partial class gameOptions
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
			this.radModManager = new System.Windows.Forms.RadioButton();
			this.radRunGame = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// radModManager
			// 
			this.radModManager.AutoSize = true;
			this.radModManager.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radModManager.Location = new System.Drawing.Point(12, 12);
			this.radModManager.Name = "radModManager";
			this.radModManager.Size = new System.Drawing.Size(146, 20);
			this.radModManager.TabIndex = 0;
			this.radModManager.TabStop = true;
			this.radModManager.Text = "Open Mod Manager";
			this.radModManager.UseVisualStyleBackColor = true;
			this.radModManager.CheckedChanged += new System.EventHandler(this.radModManager_CheckedChanged);
			// 
			// radRunGame
			// 
			this.radRunGame.AutoSize = true;
			this.radRunGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radRunGame.Location = new System.Drawing.Point(12, 38);
			this.radRunGame.Name = "radRunGame";
			this.radRunGame.Size = new System.Drawing.Size(109, 20);
			this.radRunGame.TabIndex = 1;
			this.radRunGame.TabStop = true;
			this.radRunGame.Text = "Launch Game";
			this.radRunGame.UseVisualStyleBackColor = true;
			this.radRunGame.CheckedChanged += new System.EventHandler(this.radRunGame_CheckedChanged);
			// 
			// btnOK
			// 
			this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnOK.Location = new System.Drawing.Point(93, 66);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button1.Location = new System.Drawing.Point(12, 66);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// gameOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(180, 101);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.radRunGame);
			this.Controls.Add(this.radModManager);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "gameOptions";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Game Options";
			this.Shown += new System.EventHandler(this.gameOptions_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radModManager;
		private System.Windows.Forms.RadioButton radRunGame;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button button1;
	}
}