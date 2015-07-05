namespace ModGenerator
{
    partial class SplitSettings
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
            this.SADXOptions = new System.Windows.Forms.GroupBox();
            this.sadxFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.SA2Options = new System.Windows.Forms.GroupBox();
            this.sa2Flow = new System.Windows.Forms.FlowLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.acceptButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SADXOptions.SuspendLayout();
            this.SA2Options.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SADXOptions
            // 
            this.SADXOptions.Controls.Add(this.sadxFlow);
            this.SADXOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SADXOptions.Location = new System.Drawing.Point(0, 0);
            this.SADXOptions.Name = "SADXOptions";
            this.SADXOptions.Size = new System.Drawing.Size(332, 282);
            this.SADXOptions.TabIndex = 1;
            this.SADXOptions.TabStop = false;
            this.SADXOptions.Text = "SADX Options";
            // 
            // sadxFlow
            // 
            this.sadxFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sadxFlow.Location = new System.Drawing.Point(3, 16);
            this.sadxFlow.Name = "sadxFlow";
            this.sadxFlow.Size = new System.Drawing.Size(326, 263);
            this.sadxFlow.TabIndex = 0;
            // 
            // SA2Options
            // 
            this.SA2Options.Controls.Add(this.sa2Flow);
            this.SA2Options.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SA2Options.Location = new System.Drawing.Point(0, 0);
            this.SA2Options.Name = "SA2Options";
            this.SA2Options.Size = new System.Drawing.Size(328, 282);
            this.SA2Options.TabIndex = 0;
            this.SA2Options.TabStop = false;
            this.SA2Options.Text = "SA2 Options";
            // 
            // sa2Flow
            // 
            this.sa2Flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sa2Flow.Location = new System.Drawing.Point(3, 16);
            this.sa2Flow.Name = "sa2Flow";
            this.sa2Flow.Size = new System.Drawing.Size(322, 263);
            this.sa2Flow.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.SADXOptions);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.SA2Options);
            this.splitContainer1.Size = new System.Drawing.Size(664, 282);
            this.splitContainer1.SplitterDistance = 332;
            this.splitContainer1.TabIndex = 2;
            // 
            // acceptButton
            // 
            this.acceptButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.acceptButton.Location = new System.Drawing.Point(295, 4);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(75, 23);
            this.acceptButton.TabIndex = 0;
            this.acceptButton.Text = "OK";
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.acceptButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 282);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(664, 30);
            this.panel1.TabIndex = 3;
            // 
            // SplitSettings
            // 
            this.AcceptButton = this.acceptButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 312);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "SplitSettings";
            this.Text = "SplitSettings";
            this.SADXOptions.ResumeLayout(false);
            this.SA2Options.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox SADXOptions;
        private System.Windows.Forms.GroupBox SA2Options;
        private System.Windows.Forms.FlowLayoutPanel sadxFlow;
        private System.Windows.Forms.FlowLayoutPanel sa2Flow;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Panel panel1;
    }
}