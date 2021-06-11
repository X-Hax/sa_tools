
namespace VMSEditor
{
    partial class EditorChaoSelectChao
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorChaoSelectChao));
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxChao = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxEggs = new System.Windows.Forms.ListBox();
            this.buttonUnselectChao = new System.Windows.Forms.Button();
            this.buttonUnselectEggs = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(589, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "Download Data and Chao Adventure files can only contain one Chao and one egg.\r\nSe" +
    "lect the Chao you would like to export.";
            // 
            // listBoxChao
            // 
            this.listBoxChao.FormattingEnabled = true;
            this.listBoxChao.ItemHeight = 20;
            this.listBoxChao.Location = new System.Drawing.Point(16, 100);
            this.listBoxChao.Name = "listBoxChao";
            this.listBoxChao.Size = new System.Drawing.Size(324, 484);
            this.listBoxChao.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Chao:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(342, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Eggs:";
            // 
            // listBoxEggs
            // 
            this.listBoxEggs.FormattingEnabled = true;
            this.listBoxEggs.ItemHeight = 20;
            this.listBoxEggs.Location = new System.Drawing.Point(346, 100);
            this.listBoxEggs.Name = "listBoxEggs";
            this.listBoxEggs.Size = new System.Drawing.Size(324, 484);
            this.listBoxEggs.TabIndex = 5;
            // 
            // buttonUnselectChao
            // 
            this.buttonUnselectChao.Location = new System.Drawing.Point(16, 591);
            this.buttonUnselectChao.Name = "buttonUnselectChao";
            this.buttonUnselectChao.Size = new System.Drawing.Size(96, 37);
            this.buttonUnselectChao.TabIndex = 6;
            this.buttonUnselectChao.Text = "Unselect";
            this.buttonUnselectChao.UseVisualStyleBackColor = true;
            this.buttonUnselectChao.Click += new System.EventHandler(this.buttonUnselectChao_Click);
            // 
            // buttonUnselectEggs
            // 
            this.buttonUnselectEggs.Location = new System.Drawing.Point(346, 590);
            this.buttonUnselectEggs.Name = "buttonUnselectEggs";
            this.buttonUnselectEggs.Size = new System.Drawing.Size(96, 37);
            this.buttonUnselectEggs.TabIndex = 7;
            this.buttonUnselectEggs.Text = "Unselect";
            this.buttonUnselectEggs.UseVisualStyleBackColor = true;
            this.buttonUnselectEggs.Click += new System.EventHandler(this.buttonUnselectEggs_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(578, 695);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(96, 37);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(476, 695);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(96, 37);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // EditoChaoSelectChao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 744);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonUnselectEggs);
            this.Controls.Add(this.buttonUnselectChao);
            this.Controls.Add(this.listBoxEggs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBoxChao);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditoChaoSelectChao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export Chao Download Data";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxChao;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listBoxEggs;
        private System.Windows.Forms.Button buttonUnselectChao;
        private System.Windows.Forms.Button buttonUnselectEggs;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}