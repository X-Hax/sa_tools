namespace SonicRetro.SAModel.SAMDL
{
    partial class MainForm
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
			this.components = new System.ComponentModel.Container();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.colladaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cStructsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.objToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.modelTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(584, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.loadTexturesToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// loadTexturesToolStripMenuItem
			// 
			this.loadTexturesToolStripMenuItem.Name = "loadTexturesToolStripMenuItem";
			this.loadTexturesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.loadTexturesToolStripMenuItem.Text = "Load Textures...";
			this.loadTexturesToolStripMenuItem.Click += new System.EventHandler(this.loadTexturesToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.saveToolStripMenuItem.Text = "&Save...";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colladaToolStripMenuItem,
            this.cStructsToolStripMenuItem,
            this.objToolStripMenuItem});
			this.exportToolStripMenuItem.Enabled = false;
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.exportToolStripMenuItem.Text = "&Export";
			// 
			// colladaToolStripMenuItem
			// 
			this.colladaToolStripMenuItem.Name = "colladaToolStripMenuItem";
			this.colladaToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.colladaToolStripMenuItem.Text = "&Collada";
			this.colladaToolStripMenuItem.Click += new System.EventHandler(this.colladaToolStripMenuItem_Click);
			// 
			// cStructsToolStripMenuItem
			// 
			this.cStructsToolStripMenuItem.Name = "cStructsToolStripMenuItem";
			this.cStructsToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.cStructsToolStripMenuItem.Text = "C &Structs";
			this.cStructsToolStripMenuItem.Click += new System.EventHandler(this.cStructsToolStripMenuItem_Click);
			// 
			// objToolStripMenuItem
			// 
			this.objToolStripMenuItem.Name = "objToolStripMenuItem";
			this.objToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.objToolStripMenuItem.Text = "&Obj";
			this.objToolStripMenuItem.Click += new System.EventHandler(this.objToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modelTreeToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// modelTreeToolStripMenuItem
			// 
			this.modelTreeToolStripMenuItem.Name = "modelTreeToolStripMenuItem";
			this.modelTreeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.modelTreeToolStripMenuItem.Text = "Model &Tree";
			this.modelTreeToolStripMenuItem.Click += new System.EventHandler(this.modelTreeToolStripMenuItem_Click);
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(584, 538);
			this.panel1.TabIndex = 1;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
			this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
			// 
			// timer1
			// 
			this.timer1.Interval = 33;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyModelToolStripMenuItem,
            this.pasteModelToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// copyModelToolStripMenuItem
			// 
			this.copyModelToolStripMenuItem.Enabled = false;
			this.copyModelToolStripMenuItem.Name = "copyModelToolStripMenuItem";
			this.copyModelToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.copyModelToolStripMenuItem.Text = "&Copy Model";
			this.copyModelToolStripMenuItem.Click += new System.EventHandler(this.copyModelToolStripMenuItem_Click);
			// 
			// pasteModelToolStripMenuItem
			// 
			this.pasteModelToolStripMenuItem.Enabled = false;
			this.pasteModelToolStripMenuItem.Name = "pasteModelToolStripMenuItem";
			this.pasteModelToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.pasteModelToolStripMenuItem.Text = "&Paste Model";
			this.pasteModelToolStripMenuItem.Click += new System.EventHandler(this.pasteModelToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 562);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "SAMDL";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTexturesToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem colladaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cStructsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteModelToolStripMenuItem;
    }
}

