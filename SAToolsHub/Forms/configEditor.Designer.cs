namespace SAToolsHub.Forms
{
	partial class configEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(configEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsNew = new System.Windows.Forms.ToolStripButton();
            this.tsOpen = new System.Windows.Forms.ToolStripButton();
            this.tsSave = new System.Windows.Forms.ToolStripButton();
            this.tsSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsPropsGroup = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsPropsAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPropsRem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPropSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsEnumsGroup = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsEnumsAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnumsRem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnumSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsElementAdd = new System.Windows.Forms.ToolStripButton();
            this.tsElementRem = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(1424, 357);
            this.splitContainer1.SplitterDistance = 111;
            this.splitContainer1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsNew,
            this.tsOpen,
            this.tsSave,
            this.tsSaveAs,
            this.toolStripSeparator1,
            this.tsPropsGroup,
            this.tsEnumsGroup,
            this.toolStripSeparator2,
            this.tsElementAdd,
            this.tsElementRem});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(111, 190);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsNew
            // 
            this.tsNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsNew.Image = ((System.Drawing.Image)(resources.GetObject("tsNew.Image")));
            this.tsNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsNew.Name = "tsNew";
            this.tsNew.Size = new System.Drawing.Size(109, 19);
            this.tsNew.Text = "New";
            this.tsNew.Click += new System.EventHandler(this.tsNew_Click);
            // 
            // tsOpen
            // 
            this.tsOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsOpen.Image")));
            this.tsOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsOpen.Name = "tsOpen";
            this.tsOpen.Size = new System.Drawing.Size(109, 19);
            this.tsOpen.Text = "Open";
            this.tsOpen.Click += new System.EventHandler(this.tsOpen_Click);
            // 
            // tsSave
            // 
            this.tsSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsSave.Image = ((System.Drawing.Image)(resources.GetObject("tsSave.Image")));
            this.tsSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSave.Name = "tsSave";
            this.tsSave.Size = new System.Drawing.Size(109, 19);
            this.tsSave.Text = "Save";
            // 
            // tsSaveAs
            // 
            this.tsSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("tsSaveAs.Image")));
            this.tsSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSaveAs.Name = "tsSaveAs";
            this.tsSaveAs.Size = new System.Drawing.Size(109, 19);
            this.tsSaveAs.Text = "Save As";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(109, 6);
            // 
            // tsPropsGroup
            // 
            this.tsPropsGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsPropsGroup.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsPropsAdd,
            this.tsPropsRem,
            this.tsPropSeparator});
            this.tsPropsGroup.Image = ((System.Drawing.Image)(resources.GetObject("tsPropsGroup.Image")));
            this.tsPropsGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPropsGroup.Name = "tsPropsGroup";
            this.tsPropsGroup.Size = new System.Drawing.Size(109, 19);
            this.tsPropsGroup.Text = "Properties";
            this.tsPropsGroup.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsPropsGroup_DropDownItemClicked);
            // 
            // tsPropsAdd
            // 
            this.tsPropsAdd.Name = "tsPropsAdd";
            this.tsPropsAdd.Size = new System.Drawing.Size(201, 22);
            this.tsPropsAdd.Text = "Add Property Group";
            // 
            // tsPropsRem
            // 
            this.tsPropsRem.Name = "tsPropsRem";
            this.tsPropsRem.Size = new System.Drawing.Size(201, 22);
            this.tsPropsRem.Text = "Remove Property Group";
            // 
            // tsPropSeparator
            // 
            this.tsPropSeparator.Name = "tsPropSeparator";
            this.tsPropSeparator.Size = new System.Drawing.Size(198, 6);
            // 
            // tsEnumsGroup
            // 
            this.tsEnumsGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsEnumsGroup.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsEnumsAdd,
            this.tsEnumsRem,
            this.tsEnumSeparator});
            this.tsEnumsGroup.Image = ((System.Drawing.Image)(resources.GetObject("tsEnumsGroup.Image")));
            this.tsEnumsGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsEnumsGroup.Name = "tsEnumsGroup";
            this.tsEnumsGroup.Size = new System.Drawing.Size(109, 19);
            this.tsEnumsGroup.Text = "Enums";
            this.tsEnumsGroup.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsEnumsGroup_DropDownItemClicked);
            // 
            // tsEnumsAdd
            // 
            this.tsEnumsAdd.Name = "tsEnumsAdd";
            this.tsEnumsAdd.Size = new System.Drawing.Size(187, 22);
            this.tsEnumsAdd.Text = "Add Enum Group";
            // 
            // tsEnumsRem
            // 
            this.tsEnumsRem.Name = "tsEnumsRem";
            this.tsEnumsRem.Size = new System.Drawing.Size(187, 22);
            this.tsEnumsRem.Text = "Remove Enum Group";
            // 
            // tsEnumSeparator
            // 
            this.tsEnumSeparator.Name = "tsEnumSeparator";
            this.tsEnumSeparator.Size = new System.Drawing.Size(184, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(109, 6);
            // 
            // tsElementAdd
            // 
            this.tsElementAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsElementAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsElementAdd.Image")));
            this.tsElementAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsElementAdd.Name = "tsElementAdd";
            this.tsElementAdd.Size = new System.Drawing.Size(109, 19);
            this.tsElementAdd.Text = "Add Element";
            this.tsElementAdd.Click += new System.EventHandler(this.tsElementAdd_Click);
            // 
            // tsElementRem
            // 
            this.tsElementRem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsElementRem.Image = ((System.Drawing.Image)(resources.GetObject("tsElementRem.Image")));
            this.tsElementRem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsElementRem.Name = "tsElementRem";
            this.tsElementRem.Size = new System.Drawing.Size(109, 19);
            this.tsElementRem.Text = "Rem Element";
            this.tsElementRem.Click += new System.EventHandler(this.tsElementRem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(1303, 351);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(99, 19);
            this.toolStripButton4.Text = "Save";
            // 
            // configEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1424, 357);
            this.Controls.Add(this.splitContainer1);
            this.Name = "configEditor";
            this.Text = "Config Schema Builder";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton tsNew;
		private System.Windows.Forms.ToolStripButton tsOpen;
		private System.Windows.Forms.ToolStripButton tsSave;
		private System.Windows.Forms.ToolStripButton tsSaveAs;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripDropDownButton tsPropsGroup;
		private System.Windows.Forms.ToolStripSeparator tsPropSeparator;
		private System.Windows.Forms.ToolStripMenuItem tsPropsAdd;
		private System.Windows.Forms.ToolStripMenuItem tsPropsRem;
		private System.Windows.Forms.ToolStripDropDownButton tsEnumsGroup;
		private System.Windows.Forms.ToolStripSeparator tsEnumSeparator;
		private System.Windows.Forms.ToolStripMenuItem tsEnumsAdd;
		private System.Windows.Forms.ToolStripMenuItem tsEnumsRem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton tsElementAdd;
		private System.Windows.Forms.ToolStripButton tsElementRem;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.ToolStripButton toolStripButton4;
	}
}