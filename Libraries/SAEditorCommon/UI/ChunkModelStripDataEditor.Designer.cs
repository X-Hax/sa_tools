using SharpDX.Direct3D9;
using System.Net.NetworkInformation;

namespace SAModel.SAEditorCommon.UI
{
    partial class ChunkModelStripDataEditor
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
			colorDialog = new System.Windows.Forms.ColorDialog();
			flagsGroupBox = new System.Windows.Forms.GroupBox();
			ignoreLightCheck = new System.Windows.Forms.CheckBox();
			flatShadeCheck = new System.Windows.Forms.CheckBox();
			doubleSideCheck = new System.Windows.Forms.CheckBox();
			envMapCheck = new System.Windows.Forms.CheckBox();
			useAlphaCheck = new System.Windows.Forms.CheckBox();
			ignoreSpecCheck = new System.Windows.Forms.CheckBox();
			ignoreAmbiCheck = new System.Windows.Forms.CheckBox();
			doneButton = new System.Windows.Forms.Button();
			toolTip = new System.Windows.Forms.ToolTip(components);
			resetButton = new System.Windows.Forms.Button();
			flagsGroupBox.SuspendLayout();
			SuspendLayout();
			// 
			// colorDialog
			// 
			colorDialog.AnyColor = true;
			colorDialog.FullOpen = true;
			colorDialog.SolidColorOnly = true;
			// 
			// flagsGroupBox
			// 
			flagsGroupBox.Controls.Add(ignoreLightCheck);
			flagsGroupBox.Controls.Add(flatShadeCheck);
			flagsGroupBox.Controls.Add(doubleSideCheck);
			flagsGroupBox.Controls.Add(envMapCheck);
			flagsGroupBox.Controls.Add(useAlphaCheck);
			flagsGroupBox.Controls.Add(ignoreSpecCheck);
			flagsGroupBox.Controls.Add(ignoreAmbiCheck);
			flagsGroupBox.Location = new System.Drawing.Point(15, 13);
			flagsGroupBox.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flagsGroupBox.Name = "flagsGroupBox";
			flagsGroupBox.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flagsGroupBox.Size = new System.Drawing.Size(266, 306);
			flagsGroupBox.TabIndex = 8;
			flagsGroupBox.TabStop = false;
			flagsGroupBox.Text = "Flags";
			// 
			// ignoreLightCheck
			// 
			ignoreLightCheck.Location = new System.Drawing.Point(20, 219);
			ignoreLightCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ignoreLightCheck.Name = "ignoreLightCheck";
			ignoreLightCheck.Size = new System.Drawing.Size(159, 29);
			ignoreLightCheck.TabIndex = 12;
			ignoreLightCheck.Text = "Ignore Lighting";
			toolTip.SetToolTip(ignoreLightCheck, "If checked, the mesh will not have any lighting applied.");
			ignoreLightCheck.UseVisualStyleBackColor = true;
			ignoreLightCheck.Click += ignoreLightCheck_Click;
			// 
			// flatShadeCheck
			// 
			flatShadeCheck.Location = new System.Drawing.Point(20, 147);
			flatShadeCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			flatShadeCheck.Name = "flatShadeCheck";
			flatShadeCheck.Size = new System.Drawing.Size(131, 29);
			flatShadeCheck.TabIndex = 11;
			flatShadeCheck.Text = "Flat Shaded";
			toolTip.SetToolTip(flatShadeCheck, "If checked, polygon smoothing will be disabled and the model will appear faceted, like a cut gem or die. This flag does nothing in SADX, or SA2B without the Render Fix mod.");
			flatShadeCheck.UseVisualStyleBackColor = true;
			flatShadeCheck.Click += flatShadeCheck_Click;
			// 
			// doubleSideCheck
			// 
			doubleSideCheck.Location = new System.Drawing.Point(20, 111);
			doubleSideCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			doubleSideCheck.Name = "doubleSideCheck";
			doubleSideCheck.Size = new System.Drawing.Size(146, 29);
			doubleSideCheck.TabIndex = 10;
			doubleSideCheck.Text = "Double Sided";
			toolTip.SetToolTip(doubleSideCheck, "If enabled, both sides of a mesh will be drawn - hence \"double sided\". When disabled, only the front faces of the mesh are drawn, with the back faces being culled. This flag does nothing in SA1 / SADX, or SA2B without the Render Fix mod.");
			doubleSideCheck.UseVisualStyleBackColor = true;
			doubleSideCheck.Click += doubleSideCheck_Click;
			// 
			// envMapCheck
			// 
			envMapCheck.Location = new System.Drawing.Point(20, 75);
			envMapCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			envMapCheck.Name = "envMapCheck";
			envMapCheck.Size = new System.Drawing.Size(215, 29);
			envMapCheck.TabIndex = 9;
			envMapCheck.Text = "Environment Mapping";
			toolTip.SetToolTip(envMapCheck, "If checked, the texture's uv maps will be mapped to the environment and the model will appear 'shiny'.");
			envMapCheck.UseVisualStyleBackColor = true;
			envMapCheck.Click += envMapCheck_Click;
			// 
			// useAlphaCheck
			// 
			useAlphaCheck.Location = new System.Drawing.Point(20, 39);
			useAlphaCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			useAlphaCheck.Name = "useAlphaCheck";
			useAlphaCheck.Size = new System.Drawing.Size(118, 29);
			useAlphaCheck.TabIndex = 7;
			useAlphaCheck.Text = "Use Alpha";
			toolTip.SetToolTip(useAlphaCheck, "If checked, texture transparency will be enabled (and possibly non-texture transparency). ");
			useAlphaCheck.UseVisualStyleBackColor = true;
			useAlphaCheck.Click += useAlphaCheck_Click;
			// 
			// ignoreSpecCheck
			// 
			ignoreSpecCheck.Location = new System.Drawing.Point(20, 255);
			ignoreSpecCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ignoreSpecCheck.Name = "ignoreSpecCheck";
			ignoreSpecCheck.Size = new System.Drawing.Size(162, 29);
			ignoreSpecCheck.TabIndex = 6;
			ignoreSpecCheck.Text = "Ignore Specular";
			toolTip.SetToolTip(ignoreSpecCheck, "Disables specular lighting on the material. This flag does nothing in SADX or SA2B.");
			ignoreSpecCheck.UseVisualStyleBackColor = true;
			ignoreSpecCheck.Click += ignoreSpecCheck_Click;
			// 
			// ignoreAmbiCheck
			// 
			ignoreAmbiCheck.Location = new System.Drawing.Point(20, 182);
			ignoreAmbiCheck.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			ignoreAmbiCheck.Name = "ignoreAmbiCheck";
			ignoreAmbiCheck.Size = new System.Drawing.Size(163, 29);
			ignoreAmbiCheck.TabIndex = 0;
			ignoreAmbiCheck.Text = "Ignore Ambient";
			toolTip.SetToolTip(ignoreAmbiCheck, "If checked, the mesh will ignore the ambient light source. In other words, the ambient light will be treated as black, darkening the model. This flag does nothing in SA2B without the Render Fix mod.");
			ignoreAmbiCheck.UseVisualStyleBackColor = true;
			ignoreAmbiCheck.CheckedChanged += ignoreAmbiCheck_CheckedChanged;
			ignoreAmbiCheck.Click += ignoreAmbiCheck_Click;
			// 
			// doneButton
			// 
			doneButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			doneButton.Location = new System.Drawing.Point(175, 351);
			doneButton.Margin = new System.Windows.Forms.Padding(4);
			doneButton.Name = "doneButton";
			doneButton.Size = new System.Drawing.Size(132, 40);
			doneButton.TabIndex = 0;
			doneButton.Text = "Done";
			doneButton.UseVisualStyleBackColor = true;
			doneButton.Click += doneButton_Click;
			// 
			// toolTip
			// 
			toolTip.AutoPopDelay = 30000;
			toolTip.InitialDelay = 500;
			toolTip.ReshowDelay = 100;
			// 
			// resetButton
			// 
			resetButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			resetButton.Location = new System.Drawing.Point(16, 351);
			resetButton.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			resetButton.Name = "resetButton";
			resetButton.Size = new System.Drawing.Size(132, 40);
			resetButton.TabIndex = 6;
			resetButton.Text = "Reset";
			toolTip.SetToolTip(resetButton, "Reset the poly data to the state it was when this dialog opened.");
			resetButton.UseVisualStyleBackColor = true;
			resetButton.Click += resetButton_Click;
			// 
			// ChunkModelStripDataEditor
			// 
			AcceptButton = doneButton;
			AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(322, 412);
			ControlBox = false;
			Controls.Add(resetButton);
			Controls.Add(doneButton);
			Controls.Add(flagsGroupBox);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChunkModelStripDataEditor";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Strip Data Editor";
			Load += MaterialEditor_Load;
			flagsGroupBox.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.GroupBox flagsGroupBox;
        private System.Windows.Forms.CheckBox ignoreAmbiCheck;
        private System.Windows.Forms.CheckBox useAlphaCheck;
        private System.Windows.Forms.CheckBox ignoreSpecCheck;
        private System.Windows.Forms.CheckBox doubleSideCheck;
        private System.Windows.Forms.CheckBox envMapCheck;
        private System.Windows.Forms.CheckBox ignoreLightCheck;
        private System.Windows.Forms.CheckBox flatShadeCheck;
        private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button resetButton;
	}
}