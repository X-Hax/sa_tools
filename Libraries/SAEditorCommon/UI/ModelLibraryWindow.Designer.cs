namespace SAModel.SAEditorCommon.UI
{
    partial class ModelLibraryWindow
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
            this.modelLibraryControl1 = new SAModel.SAEditorCommon.UI.ModelLibraryControl();
            this.SuspendLayout();
            // 
            // modelLibraryControl1
            // 
            this.modelLibraryControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelLibraryControl1.Location = new System.Drawing.Point(0, 0);
            this.modelLibraryControl1.Name = "modelLibraryControl1";
            this.modelLibraryControl1.SelectedModel = null;
            this.modelLibraryControl1.Size = new System.Drawing.Size(962, 395);
            this.modelLibraryControl1.TabIndex = 0;
            // 
            // ModelLibraryWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 395);
            this.Controls.Add(this.modelLibraryControl1);
            this.Name = "ModelLibraryWindow";
            this.Text = "ModelLibraryWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModelLibraryWindow_FormClosing);
            this.Shown += new System.EventHandler(this.ModelLibraryWindow_Shown);
            this.ResumeLayout(false);

        }


        #endregion

        public ModelLibraryControl modelLibraryControl1;
    }
}