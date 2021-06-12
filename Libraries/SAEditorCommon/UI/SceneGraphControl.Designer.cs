namespace SAModel.SAEditorCommon.UI
{
    partial class SceneGraphControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SceneGraphControl));
            this.sceneTreeView = new SAModel.SAEditorCommon.UI.MultiSelectTreeview();
            this.SuspendLayout();
            // 
            // sceneTreeView
            // 
            this.sceneTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sceneTreeView.Location = new System.Drawing.Point(0, 0);
            this.sceneTreeView.Name = "sceneTreeView";
            this.sceneTreeView.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("sceneTreeView.SelectedNodes")));
            this.sceneTreeView.Size = new System.Drawing.Size(235, 385);
            this.sceneTreeView.TabIndex = 0;
            // 
            // SceneGraphControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sceneTreeView);
            this.Name = "SceneGraphControl";
            this.Size = new System.Drawing.Size(235, 385);
            this.Load += new System.EventHandler(this.SceneGraphControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MultiSelectTreeview sceneTreeView;
    }
}
