namespace SADXsndSharp
{
    partial class Form1
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			menuStrip1 = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			tilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			sortOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ascendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			descendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportADXAsWAVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			useThe2010FormatForDATToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
			extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			listView1 = new System.Windows.Forms.ListView();
			Column_Name = new System.Windows.Forms.ColumnHeader();
			Column_Size = new System.Windows.Forms.ColumnHeader();
			Column_Index = new System.Windows.Forms.ColumnHeader();
			imageList1 = new System.Windows.Forms.ImageList(components);
			imageList2 = new System.Windows.Forms.ImageList(components);
			toolTip1 = new System.Windows.Forms.ToolTip(components);
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			buttonAdd = new System.Windows.Forms.Button();
			buttonRemove = new System.Windows.Forms.Button();
			buttonMoveUp = new System.Windows.Forms.Button();
			buttonMoveDown = new System.Windows.Forms.Button();
			buttonExtract = new System.Windows.Forms.Button();
			buttonReplace = new System.Windows.Forms.Button();
			menuStrip1.SuspendLayout();
			contextMenuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
			menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, optionsToolStripMenuItem });
			menuStrip1.Location = new System.Drawing.Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
			menuStrip1.Size = new System.Drawing.Size(271, 24);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, addFilesToolStripMenuItem, toolStripSeparator3, saveToolStripMenuItem, saveAsToolStripMenuItem, extractAllToolStripMenuItem, toolStripSeparator1, quitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			newToolStripMenuItem.Image = Properties.Resources._new;
			newToolStripMenuItem.Name = "newToolStripMenuItem";
			newToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			newToolStripMenuItem.Text = "&New";
			newToolStripMenuItem.Click += newToolStripMenuItem_Click;
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Image = Properties.Resources.open;
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			openToolStripMenuItem.Text = "&Open...";
			openToolStripMenuItem.Click += openToolStripMenuItem_Click;
			// 
			// addFilesToolStripMenuItem
			// 
			addFilesToolStripMenuItem.Image = Properties.Resources.add;
			addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
			addFilesToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			addFilesToolStripMenuItem.Text = "&Add Files...";
			addFilesToolStripMenuItem.Click += addFilesToolStripMenuItem_Click;
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(133, 6);
			// 
			// saveToolStripMenuItem
			// 
			saveToolStripMenuItem.Image = Properties.Resources.save;
			saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			saveToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			saveToolStripMenuItem.Text = "&Save";
			saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
			// 
			// saveAsToolStripMenuItem
			// 
			saveAsToolStripMenuItem.Image = Properties.Resources.saveas;
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			saveAsToolStripMenuItem.Text = "Save &As...";
			saveAsToolStripMenuItem.DropDownItemClicked += saveAsToolStripMenuItem_DropDownItemClicked;
			// 
			// extractAllToolStripMenuItem
			// 
			extractAllToolStripMenuItem.Image = Properties.Resources.export;
			extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
			extractAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			extractAllToolStripMenuItem.Text = "&Extract All...";
			extractAllToolStripMenuItem.Click += extractAllToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(133, 6);
			// 
			// quitToolStripMenuItem
			// 
			quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			quitToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			quitToolStripMenuItem.Text = "&Quit";
			quitToolStripMenuItem.Click += quitToolStripMenuItem_Click;
			// 
			// viewToolStripMenuItem
			// 
			viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { largeIconsToolStripMenuItem, smallIconsToolStripMenuItem, tilesToolStripMenuItem, detailsToolStripMenuItem, toolStripSeparator2, sortOrderToolStripMenuItem });
			viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			viewToolStripMenuItem.Text = "View";
			// 
			// largeIconsToolStripMenuItem
			// 
			largeIconsToolStripMenuItem.Image = Properties.Resources.large;
			largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
			largeIconsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			largeIconsToolStripMenuItem.Text = "Large Icons";
			largeIconsToolStripMenuItem.Click += largeIconsToolStripMenuItem_Click;
			// 
			// smallIconsToolStripMenuItem
			// 
			smallIconsToolStripMenuItem.Image = Properties.Resources.small;
			smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
			smallIconsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			smallIconsToolStripMenuItem.Text = "Small Icons";
			smallIconsToolStripMenuItem.Click += smallIconsToolStripMenuItem_Click;
			// 
			// tilesToolStripMenuItem
			// 
			tilesToolStripMenuItem.Image = Properties.Resources.tiles;
			tilesToolStripMenuItem.Name = "tilesToolStripMenuItem";
			tilesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			tilesToolStripMenuItem.Text = "Tiles";
			tilesToolStripMenuItem.Click += tilesToolStripMenuItem_Click;
			// 
			// detailsToolStripMenuItem
			// 
			detailsToolStripMenuItem.Checked = true;
			detailsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			detailsToolStripMenuItem.Image = Properties.Resources.details;
			detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
			detailsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			detailsToolStripMenuItem.Text = "Details";
			detailsToolStripMenuItem.Click += detailsToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(171, 6);
			// 
			// sortOrderToolStripMenuItem
			// 
			sortOrderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { ascendingToolStripMenuItem, descendingToolStripMenuItem });
			sortOrderToolStripMenuItem.Name = "sortOrderToolStripMenuItem";
			sortOrderToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			sortOrderToolStripMenuItem.Text = "Column Sort Order";
			// 
			// ascendingToolStripMenuItem
			// 
			ascendingToolStripMenuItem.Checked = true;
			ascendingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			ascendingToolStripMenuItem.Image = Properties.Resources.ascending;
			ascendingToolStripMenuItem.Name = "ascendingToolStripMenuItem";
			ascendingToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			ascendingToolStripMenuItem.Text = "Ascending";
			ascendingToolStripMenuItem.Click += ascendingToolStripMenuItem_Click;
			// 
			// descendingToolStripMenuItem
			// 
			descendingToolStripMenuItem.Image = Properties.Resources.descending;
			descendingToolStripMenuItem.Name = "descendingToolStripMenuItem";
			descendingToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			descendingToolStripMenuItem.Text = "Descending";
			descendingToolStripMenuItem.Click += descendingToolStripMenuItem_Click;
			// 
			// optionsToolStripMenuItem
			// 
			optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { exportADXAsWAVToolStripMenuItem, useThe2010FormatForDATToolStripMenuItem });
			optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			optionsToolStripMenuItem.Text = "Options";
			optionsToolStripMenuItem.Visible = false;
			// 
			// exportADXAsWAVToolStripMenuItem
			// 
			exportADXAsWAVToolStripMenuItem.Name = "exportADXAsWAVToolStripMenuItem";
			exportADXAsWAVToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			exportADXAsWAVToolStripMenuItem.Text = "Export ADX as WAV";
			exportADXAsWAVToolStripMenuItem.ToolTipText = "Convert ADX files to WAV automatically upon saving or playing.\r\nUncheck this if you need to extract ADX files as-is.";
			// 
			// useThe2010FormatForDATToolStripMenuItem
			// 
			useThe2010FormatForDATToolStripMenuItem.Name = "useThe2010FormatForDATToolStripMenuItem";
			useThe2010FormatForDATToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			useThe2010FormatForDATToolStripMenuItem.Text = "Use the 2010 format for DAT";
			useThe2010FormatForDATToolStripMenuItem.ToolTipText = resources.GetString("useThe2010FormatForDATToolStripMenuItem.ToolTipText");
			// 
			// contextMenuStrip1
			// 
			contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { extractToolStripMenuItem, replaceToolStripMenuItem, insertToolStripMenuItem, deleteToolStripMenuItem });
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new System.Drawing.Size(125, 92);
			// 
			// extractToolStripMenuItem
			// 
			extractToolStripMenuItem.Name = "extractToolStripMenuItem";
			extractToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			extractToolStripMenuItem.Text = "&Extract...";
			extractToolStripMenuItem.Click += extractToolStripMenuItem_Click;
			// 
			// replaceToolStripMenuItem
			// 
			replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
			replaceToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			replaceToolStripMenuItem.Text = "&Replace...";
			replaceToolStripMenuItem.Click += replaceToolStripMenuItem_Click;
			// 
			// insertToolStripMenuItem
			// 
			insertToolStripMenuItem.Name = "insertToolStripMenuItem";
			insertToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			insertToolStripMenuItem.Text = "&Insert...";
			insertToolStripMenuItem.Click += insertToolStripMenuItem_Click;
			// 
			// deleteToolStripMenuItem
			// 
			deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			deleteToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			deleteToolStripMenuItem.Text = "&Delete";
			deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
			// 
			// listView1
			// 
			listView1.AllowDrop = true;
			listView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Column_Name, Column_Size, Column_Index });
			listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			listView1.GridLines = true;
			listView1.LabelEdit = true;
			listView1.LargeImageList = imageList1;
			listView1.Location = new System.Drawing.Point(0, 24);
			listView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			listView1.MultiSelect = false;
			listView1.Name = "listView1";
			listView1.Size = new System.Drawing.Size(534, 344);
			listView1.SmallImageList = imageList2;
			listView1.TabIndex = 1;
			listView1.UseCompatibleStateImageBehavior = false;
			listView1.AfterLabelEdit += listView1_AfterLabelEdit;
			listView1.BeforeLabelEdit += listView1_BeforeLabelEdit;
			listView1.ColumnClick += listView1_ColumnClick;
			listView1.ItemActivate += listView1_ItemActivate;
			listView1.ItemDrag += listView1_ItemDrag;
			listView1.ItemSelectionChanged += listView1_ItemSelectionChanged;
			listView1.DragDrop += listView1_DragDrop;
			listView1.DragEnter += listView1_DragEnter;
			listView1.MouseClick += listView1_MouseClick;
			// 
			// Column_Name
			// 
			Column_Name.Text = "Name";
			Column_Name.Width = 240;
			// 
			// Column_Size
			// 
			Column_Size.Text = "Size";
			Column_Size.Width = 180;
			// 
			// Column_Index
			// 
			Column_Index.Text = "Index";
			// 
			// imageList1
			// 
			imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			imageList1.ImageSize = new System.Drawing.Size(72, 72);
			imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// imageList2
			// 
			imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			imageList2.ImageSize = new System.Drawing.Size(32, 32);
			imageList2.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// statusStrip1
			// 
			statusStrip1.Location = new System.Drawing.Point(0, 400);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new System.Drawing.Size(534, 22);
			statusStrip1.TabIndex = 2;
			statusStrip1.Text = "statusStrip1";
			// 
			// buttonAdd
			// 
			buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			buttonAdd.Enabled = false;
			buttonAdd.Location = new System.Drawing.Point(12, 374);
			buttonAdd.Name = "buttonAdd";
			buttonAdd.Size = new System.Drawing.Size(75, 23);
			buttonAdd.TabIndex = 3;
			buttonAdd.Text = "Add...";
			buttonAdd.UseVisualStyleBackColor = true;
			buttonAdd.Click += buttonAdd_Click;
			// 
			// buttonRemove
			// 
			buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			buttonRemove.Enabled = false;
			buttonRemove.Location = new System.Drawing.Point(93, 374);
			buttonRemove.Name = "buttonRemove";
			buttonRemove.Size = new System.Drawing.Size(75, 23);
			buttonRemove.TabIndex = 4;
			buttonRemove.Text = "Remove";
			buttonRemove.UseVisualStyleBackColor = true;
			buttonRemove.Click += buttonRemove_Click;
			// 
			// buttonMoveUp
			// 
			buttonMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			buttonMoveUp.Enabled = false;
			buttonMoveUp.Location = new System.Drawing.Point(174, 374);
			buttonMoveUp.Name = "buttonMoveUp";
			buttonMoveUp.Size = new System.Drawing.Size(75, 23);
			buttonMoveUp.TabIndex = 5;
			buttonMoveUp.Text = "Up";
			buttonMoveUp.UseVisualStyleBackColor = true;
			buttonMoveUp.Click += buttonMoveUp_Click;
			// 
			// buttonMoveDown
			// 
			buttonMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			buttonMoveDown.Enabled = false;
			buttonMoveDown.Location = new System.Drawing.Point(255, 374);
			buttonMoveDown.Name = "buttonMoveDown";
			buttonMoveDown.Size = new System.Drawing.Size(75, 23);
			buttonMoveDown.TabIndex = 6;
			buttonMoveDown.Text = "Down";
			buttonMoveDown.UseVisualStyleBackColor = true;
			buttonMoveDown.Click += buttonMoveDown_Click;
			// 
			// buttonExtract
			// 
			buttonExtract.Enabled = false;
			buttonExtract.Location = new System.Drawing.Point(367, 374);
			buttonExtract.Name = "buttonExtract";
			buttonExtract.Size = new System.Drawing.Size(75, 23);
			buttonExtract.TabIndex = 7;
			buttonExtract.Text = "Extract...";
			buttonExtract.UseVisualStyleBackColor = true;
			buttonExtract.Visible = false;
			// 
			// buttonReplace
			// 
			buttonReplace.Enabled = false;
			buttonReplace.Location = new System.Drawing.Point(448, 374);
			buttonReplace.Name = "buttonReplace";
			buttonReplace.Size = new System.Drawing.Size(75, 23);
			buttonReplace.TabIndex = 8;
			buttonReplace.Text = "Replace...";
			buttonReplace.UseVisualStyleBackColor = true;
			buttonReplace.Visible = false;
			// 
			// Form1
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(534, 422);
			Controls.Add(buttonReplace);
			Controls.Add(buttonExtract);
			Controls.Add(buttonMoveDown);
			Controls.Add(buttonMoveUp);
			Controls.Add(buttonRemove);
			Controls.Add(buttonAdd);
			Controls.Add(statusStrip1);
			Controls.Add(listView1);
			Controls.Add(menuStrip1);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			MainMenuStrip = menuStrip1;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MinimumSize = new System.Drawing.Size(550, 240);
			Name = "Form1";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "SADXsndSharp";
			Load += Form1_Load;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			contextMenuStrip1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem extractAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ImageList imageList2;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem smallIconsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
		private System.Windows.Forms.ColumnHeader Column_Name;
		private System.Windows.Forms.ColumnHeader Column_Size;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem sortOrderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ascendingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem descendingToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ColumnHeader Column_Index;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportADXAsWAVToolStripMenuItem;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Button buttonMoveUp;
		private System.Windows.Forms.Button buttonMoveDown;
		private System.Windows.Forms.ToolStripMenuItem useThe2010FormatForDATToolStripMenuItem;
		private System.Windows.Forms.Button buttonExtract;
		private System.Windows.Forms.Button buttonReplace;
	}
}

