namespace SADXTweaker2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelTextureListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trialLevelListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bossLevelListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundTestListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.musicListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stringListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextLevelListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutsceneTextEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recapScreenEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nPCMessageEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelClearFlagListEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.messageFileEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bugReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chaoMessageFileEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.windowToolStripMenuItem,
            this.helpToolStripMenuItem});
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
            this.recentProjectsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // recentProjectsToolStripMenuItem
            // 
            this.recentProjectsToolStripMenuItem.Name = "recentProjectsToolStripMenuItem";
            this.recentProjectsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.recentProjectsToolStripMenuItem.Text = "&Recent Projects";
            this.recentProjectsToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recentProjectsToolStripMenuItem_DropDownItemClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.objectListEditorToolStripMenuItem,
            this.textureListEditorToolStripMenuItem,
            this.levelTextureListEditorToolStripMenuItem,
            this.trialLevelListEditorToolStripMenuItem,
            this.bossLevelListEditorToolStripMenuItem,
            this.soundTestListEditorToolStripMenuItem,
            this.musicListEditorToolStripMenuItem,
            this.soundListEditorToolStripMenuItem,
            this.stringListEditorToolStripMenuItem,
            this.nextLevelListEditorToolStripMenuItem,
            this.cutsceneTextEditorToolStripMenuItem,
            this.recapScreenEditorToolStripMenuItem,
            this.nPCMessageEditorToolStripMenuItem,
            this.levelClearFlagListEditorToolStripMenuItem,
            this.toolStripSeparator3,
            this.messageFileEditorToolStripMenuItem,
            this.chaoMessageFileEditorToolStripMenuItem,
            this.toolStripSeparator2,
            this.cascadeToolStripMenuItem,
            this.tileHorizontalToolStripMenuItem,
            this.tileVerticalToolStripMenuItem});
            this.windowToolStripMenuItem.Enabled = false;
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "&Window";
            // 
            // objectListEditorToolStripMenuItem
            // 
            this.objectListEditorToolStripMenuItem.Name = "objectListEditorToolStripMenuItem";
            this.objectListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.objectListEditorToolStripMenuItem.Text = "Object List Editor";
            this.objectListEditorToolStripMenuItem.Click += new System.EventHandler(this.objectListEditorToolStripMenuItem_Click);
            // 
            // textureListEditorToolStripMenuItem
            // 
            this.textureListEditorToolStripMenuItem.Name = "textureListEditorToolStripMenuItem";
            this.textureListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.textureListEditorToolStripMenuItem.Text = "Texture List Editor";
            this.textureListEditorToolStripMenuItem.Click += new System.EventHandler(this.textureListEditorToolStripMenuItem_Click);
            // 
            // levelTextureListEditorToolStripMenuItem
            // 
            this.levelTextureListEditorToolStripMenuItem.Name = "levelTextureListEditorToolStripMenuItem";
            this.levelTextureListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.levelTextureListEditorToolStripMenuItem.Text = "Level Texture List Editor";
            this.levelTextureListEditorToolStripMenuItem.Click += new System.EventHandler(this.levelTextureListEditorToolStripMenuItem_Click);
            // 
            // trialLevelListEditorToolStripMenuItem
            // 
            this.trialLevelListEditorToolStripMenuItem.Name = "trialLevelListEditorToolStripMenuItem";
            this.trialLevelListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.trialLevelListEditorToolStripMenuItem.Text = "Trial Level List Editor";
            this.trialLevelListEditorToolStripMenuItem.Click += new System.EventHandler(this.trialLevelListEditorToolStripMenuItem_Click);
            // 
            // bossLevelListEditorToolStripMenuItem
            // 
            this.bossLevelListEditorToolStripMenuItem.Name = "bossLevelListEditorToolStripMenuItem";
            this.bossLevelListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.bossLevelListEditorToolStripMenuItem.Text = "Boss Level List Editor";
            this.bossLevelListEditorToolStripMenuItem.Click += new System.EventHandler(this.bossLevelListEditorToolStripMenuItem_Click);
            // 
            // soundTestListEditorToolStripMenuItem
            // 
            this.soundTestListEditorToolStripMenuItem.Name = "soundTestListEditorToolStripMenuItem";
            this.soundTestListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.soundTestListEditorToolStripMenuItem.Text = "Sound Test List Editor";
            this.soundTestListEditorToolStripMenuItem.Click += new System.EventHandler(this.soundTestListEditorToolStripMenuItem_Click);
            // 
            // musicListEditorToolStripMenuItem
            // 
            this.musicListEditorToolStripMenuItem.Name = "musicListEditorToolStripMenuItem";
            this.musicListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.musicListEditorToolStripMenuItem.Text = "Music List Editor";
            this.musicListEditorToolStripMenuItem.Click += new System.EventHandler(this.musicListEditorToolStripMenuItem_Click);
            // 
            // soundListEditorToolStripMenuItem
            // 
            this.soundListEditorToolStripMenuItem.Name = "soundListEditorToolStripMenuItem";
            this.soundListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.soundListEditorToolStripMenuItem.Text = "Sound List Editor";
            this.soundListEditorToolStripMenuItem.Click += new System.EventHandler(this.soundListEditorToolStripMenuItem_Click);
            // 
            // stringListEditorToolStripMenuItem
            // 
            this.stringListEditorToolStripMenuItem.Name = "stringListEditorToolStripMenuItem";
            this.stringListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.stringListEditorToolStripMenuItem.Text = "String List Editor";
            this.stringListEditorToolStripMenuItem.Click += new System.EventHandler(this.stringListEditorToolStripMenuItem_Click);
            // 
            // nextLevelListEditorToolStripMenuItem
            // 
            this.nextLevelListEditorToolStripMenuItem.Name = "nextLevelListEditorToolStripMenuItem";
            this.nextLevelListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.nextLevelListEditorToolStripMenuItem.Text = "Next Level List Editor";
            this.nextLevelListEditorToolStripMenuItem.Click += new System.EventHandler(this.nextLevelListEditorToolStripMenuItem_Click);
            // 
            // cutsceneTextEditorToolStripMenuItem
            // 
            this.cutsceneTextEditorToolStripMenuItem.Name = "cutsceneTextEditorToolStripMenuItem";
            this.cutsceneTextEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.cutsceneTextEditorToolStripMenuItem.Text = "Cutscene Text Editor";
            this.cutsceneTextEditorToolStripMenuItem.Click += new System.EventHandler(this.cutsceneTextEditorToolStripMenuItem_Click);
            // 
            // recapScreenEditorToolStripMenuItem
            // 
            this.recapScreenEditorToolStripMenuItem.Name = "recapScreenEditorToolStripMenuItem";
            this.recapScreenEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.recapScreenEditorToolStripMenuItem.Text = "Recap Screen Editor";
            this.recapScreenEditorToolStripMenuItem.Click += new System.EventHandler(this.recapScreenEditorToolStripMenuItem_Click);
            // 
            // nPCMessageEditorToolStripMenuItem
            // 
            this.nPCMessageEditorToolStripMenuItem.Name = "nPCMessageEditorToolStripMenuItem";
            this.nPCMessageEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.nPCMessageEditorToolStripMenuItem.Text = "NPC Message Editor";
            this.nPCMessageEditorToolStripMenuItem.Click += new System.EventHandler(this.nPCMessageEditorToolStripMenuItem_Click);
            // 
            // levelClearFlagListEditorToolStripMenuItem
            // 
            this.levelClearFlagListEditorToolStripMenuItem.Name = "levelClearFlagListEditorToolStripMenuItem";
            this.levelClearFlagListEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.levelClearFlagListEditorToolStripMenuItem.Text = "Level Clear Flag List Editor";
            this.levelClearFlagListEditorToolStripMenuItem.Click += new System.EventHandler(this.levelClearFlagListEditorToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(208, 6);
            // 
            // messageFileEditorToolStripMenuItem
            // 
            this.messageFileEditorToolStripMenuItem.Name = "messageFileEditorToolStripMenuItem";
            this.messageFileEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.messageFileEditorToolStripMenuItem.Text = "Message File Editor";
            this.messageFileEditorToolStripMenuItem.Click += new System.EventHandler(this.messageFileEditorToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(208, 6);
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.cascadeToolStripMenuItem.Text = "&Cascade";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.cascadeToolStripMenuItem_Click);
            // 
            // tileHorizontalToolStripMenuItem
            // 
            this.tileHorizontalToolStripMenuItem.Name = "tileHorizontalToolStripMenuItem";
            this.tileHorizontalToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.tileHorizontalToolStripMenuItem.Text = "Tile &Horizontal";
            this.tileHorizontalToolStripMenuItem.Click += new System.EventHandler(this.tileHorizontalToolStripMenuItem_Click);
            // 
            // tileVerticalToolStripMenuItem
            // 
            this.tileVerticalToolStripMenuItem.Name = "tileVerticalToolStripMenuItem";
            this.tileVerticalToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.tileVerticalToolStripMenuItem.Text = "Tile &Vertical";
            this.tileVerticalToolStripMenuItem.Click += new System.EventHandler(this.tileVerticalToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bugReportToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // bugReportToolStripMenuItem
            // 
            this.bugReportToolStripMenuItem.Name = "bugReportToolStripMenuItem";
            this.bugReportToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.bugReportToolStripMenuItem.Text = "&Bug Report...";
            this.bugReportToolStripMenuItem.Click += new System.EventHandler(this.bugReportToolStripMenuItem_Click);
            // 
            // chaoMessageFileEditorToolStripMenuItem
            // 
            this.chaoMessageFileEditorToolStripMenuItem.Name = "chaoMessageFileEditorToolStripMenuItem";
            this.chaoMessageFileEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.chaoMessageFileEditorToolStripMenuItem.Text = "Chao Message File Editor";
            this.chaoMessageFileEditorToolStripMenuItem.Click += new System.EventHandler(this.chaoMessageFileEditorToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 564);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "SADXTweaker2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentProjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileHorizontalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileVerticalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem levelTextureListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trialLevelListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bossLevelListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soundTestListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem musicListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soundListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bugReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stringListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextLevelListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutsceneTextEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem levelClearFlagListEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recapScreenEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem messageFileEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nPCMessageEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chaoMessageFileEditorToolStripMenuItem;
    }
}

