namespace SAModel.SAEditorCommon.UI
{
	partial class ObjectSettingsEditor
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
			groupBoxEvalFlags = new System.Windows.Forms.GroupBox();
			checkBoxEnvelope = new System.Windows.Forms.CheckBox();
			checkBoxRotateSet = new System.Windows.Forms.CheckBox();
			checkBoxRotateBase = new System.Windows.Forms.CheckBox();
			checkBoxQuaternion = new System.Windows.Forms.CheckBox();
			checkBoxModifier = new System.Windows.Forms.CheckBox();
			checkBoxClip = new System.Windows.Forms.CheckBox();
			checkBoxNoShape = new System.Windows.Forms.CheckBox();
			checkBoxNoAnim = new System.Windows.Forms.CheckBox();
			checkBoxZYXRot = new System.Windows.Forms.CheckBox();
			checkBoxSkipChild = new System.Windows.Forms.CheckBox();
			checkBoxSkipDraw = new System.Windows.Forms.CheckBox();
			checkBoxIgnoreScl = new System.Windows.Forms.CheckBox();
			checkBoxIgnoreRot = new System.Windows.Forms.CheckBox();
			checkBoxIgnorePos = new System.Windows.Forms.CheckBox();
			groupBoxPos = new System.Windows.Forms.GroupBox();
			textBoxPosZ = new System.Windows.Forms.TextBox();
			textBoxPosY = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			textBoxPosX = new System.Windows.Forms.TextBox();
			groupBoxRot = new System.Windows.Forms.GroupBox();
			textBoxRotZ = new System.Windows.Forms.TextBox();
			textBoxRotY = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			textBoxRotX = new System.Windows.Forms.TextBox();
			groupBoxScl = new System.Windows.Forms.GroupBox();
			textBoxSclZ = new System.Windows.Forms.TextBox();
			textBoxSclY = new System.Windows.Forms.TextBox();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			textBoxSclX = new System.Windows.Forms.TextBox();
			buttonResetData = new System.Windows.Forms.Button();
			toolTip1 = new System.Windows.Forms.ToolTip(components);
			buttonClose = new System.Windows.Forms.Button();
			groupBoxEvalFlags.SuspendLayout();
			groupBoxPos.SuspendLayout();
			groupBoxRot.SuspendLayout();
			groupBoxScl.SuspendLayout();
			SuspendLayout();
			// 
			// groupBoxEvalFlags
			// 
			groupBoxEvalFlags.Controls.Add(checkBoxEnvelope);
			groupBoxEvalFlags.Controls.Add(checkBoxRotateSet);
			groupBoxEvalFlags.Controls.Add(checkBoxRotateBase);
			groupBoxEvalFlags.Controls.Add(checkBoxQuaternion);
			groupBoxEvalFlags.Controls.Add(checkBoxModifier);
			groupBoxEvalFlags.Controls.Add(checkBoxClip);
			groupBoxEvalFlags.Controls.Add(checkBoxNoShape);
			groupBoxEvalFlags.Controls.Add(checkBoxNoAnim);
			groupBoxEvalFlags.Controls.Add(checkBoxZYXRot);
			groupBoxEvalFlags.Controls.Add(checkBoxSkipChild);
			groupBoxEvalFlags.Controls.Add(checkBoxSkipDraw);
			groupBoxEvalFlags.Controls.Add(checkBoxIgnoreScl);
			groupBoxEvalFlags.Controls.Add(checkBoxIgnoreRot);
			groupBoxEvalFlags.Controls.Add(checkBoxIgnorePos);
			groupBoxEvalFlags.Location = new System.Drawing.Point(32, 28);
			groupBoxEvalFlags.Name = "groupBoxEvalFlags";
			groupBoxEvalFlags.Size = new System.Drawing.Size(259, 542);
			groupBoxEvalFlags.TabIndex = 0;
			groupBoxEvalFlags.TabStop = false;
			groupBoxEvalFlags.Text = "Eval Flags";
			// 
			// checkBoxEnvelope
			// 
			checkBoxEnvelope.AutoSize = true;
			checkBoxEnvelope.Location = new System.Drawing.Point(19, 492);
			checkBoxEnvelope.Name = "checkBoxEnvelope";
			checkBoxEnvelope.Size = new System.Drawing.Size(110, 29);
			checkBoxEnvelope.TabIndex = 13;
			checkBoxEnvelope.Text = "Envelope";
			checkBoxEnvelope.UseVisualStyleBackColor = true;
			checkBoxEnvelope.Click += checkBoxEnvelope_Click;
			// 
			// checkBoxRotateSet
			// 
			checkBoxRotateSet.AutoSize = true;
			checkBoxRotateSet.Location = new System.Drawing.Point(19, 457);
			checkBoxRotateSet.Name = "checkBoxRotateSet";
			checkBoxRotateSet.Size = new System.Drawing.Size(119, 29);
			checkBoxRotateSet.TabIndex = 12;
			checkBoxRotateSet.Text = "Rotate Set";
			checkBoxRotateSet.UseVisualStyleBackColor = true;
			checkBoxRotateSet.Click += checkBoxRotateSet_Click;
			// 
			// checkBoxRotateBase
			// 
			checkBoxRotateBase.AutoSize = true;
			checkBoxRotateBase.Location = new System.Drawing.Point(19, 422);
			checkBoxRotateBase.Name = "checkBoxRotateBase";
			checkBoxRotateBase.Size = new System.Drawing.Size(130, 29);
			checkBoxRotateBase.TabIndex = 11;
			checkBoxRotateBase.Text = "Rotate Base";
			checkBoxRotateBase.UseVisualStyleBackColor = true;
			checkBoxRotateBase.Click += checkBoxRotateBase_Click;
			// 
			// checkBoxQuaternion
			// 
			checkBoxQuaternion.AutoSize = true;
			checkBoxQuaternion.Location = new System.Drawing.Point(19, 387);
			checkBoxQuaternion.Name = "checkBoxQuaternion";
			checkBoxQuaternion.Size = new System.Drawing.Size(161, 29);
			checkBoxQuaternion.TabIndex = 10;
			checkBoxQuaternion.Text = "Use Quaternion";
			checkBoxQuaternion.UseVisualStyleBackColor = true;
			checkBoxQuaternion.Click += checkBoxQuaternion_Click;
			// 
			// checkBoxModifier
			// 
			checkBoxModifier.AutoSize = true;
			checkBoxModifier.Location = new System.Drawing.Point(19, 352);
			checkBoxModifier.Name = "checkBoxModifier";
			checkBoxModifier.Size = new System.Drawing.Size(105, 29);
			checkBoxModifier.TabIndex = 9;
			checkBoxModifier.Text = "Modifier";
			checkBoxModifier.UseVisualStyleBackColor = true;
			checkBoxModifier.Click += checkBoxModifier_Click;
			// 
			// checkBoxClip
			// 
			checkBoxClip.AutoSize = true;
			checkBoxClip.Location = new System.Drawing.Point(19, 317);
			checkBoxClip.Name = "checkBoxClip";
			checkBoxClip.Size = new System.Drawing.Size(68, 29);
			checkBoxClip.TabIndex = 8;
			checkBoxClip.Text = "Clip";
			checkBoxClip.UseVisualStyleBackColor = true;
			checkBoxClip.Click += checkBoxClip_Click;
			// 
			// checkBoxNoShape
			// 
			checkBoxNoShape.AutoSize = true;
			checkBoxNoShape.Location = new System.Drawing.Point(19, 282);
			checkBoxNoShape.Name = "checkBoxNoShape";
			checkBoxNoShape.Size = new System.Drawing.Size(179, 29);
			checkBoxNoShape.TabIndex = 7;
			checkBoxNoShape.Text = "No Shape Motion";
			checkBoxNoShape.UseVisualStyleBackColor = true;
			checkBoxNoShape.Click += checkBoxNoShape_Click;
			// 
			// checkBoxNoAnim
			// 
			checkBoxNoAnim.AutoSize = true;
			checkBoxNoAnim.Location = new System.Drawing.Point(19, 247);
			checkBoxNoAnim.Name = "checkBoxNoAnim";
			checkBoxNoAnim.Size = new System.Drawing.Size(149, 29);
			checkBoxNoAnim.TabIndex = 6;
			checkBoxNoAnim.Text = "No Animation";
			checkBoxNoAnim.UseVisualStyleBackColor = true;
			checkBoxNoAnim.Click += checkBoxNoAnim_Click;
			// 
			// checkBoxZYXRot
			// 
			checkBoxZYXRot.AutoSize = true;
			checkBoxZYXRot.Location = new System.Drawing.Point(19, 212);
			checkBoxZYXRot.Name = "checkBoxZYXRot";
			checkBoxZYXRot.Size = new System.Drawing.Size(175, 29);
			checkBoxZYXRot.TabIndex = 5;
			checkBoxZYXRot.Text = "Use ZYX Rotation";
			checkBoxZYXRot.UseVisualStyleBackColor = true;
			checkBoxZYXRot.Click += checkBoxZYXRot_Click;
			// 
			// checkBoxSkipChild
			// 
			checkBoxSkipChild.AutoSize = true;
			checkBoxSkipChild.Location = new System.Drawing.Point(19, 177);
			checkBoxSkipChild.Name = "checkBoxSkipChild";
			checkBoxSkipChild.Size = new System.Drawing.Size(142, 29);
			checkBoxSkipChild.TabIndex = 4;
			checkBoxSkipChild.Text = "Skip Children";
			toolTip1.SetToolTip(checkBoxSkipChild, "Bypasses drawing any models that are lower in the object's hierarchy.");
			checkBoxSkipChild.UseVisualStyleBackColor = true;
			checkBoxSkipChild.Click += checkBoxSkipChild_Click;
			// 
			// checkBoxSkipDraw
			// 
			checkBoxSkipDraw.AutoSize = true;
			checkBoxSkipDraw.Location = new System.Drawing.Point(19, 142);
			checkBoxSkipDraw.Name = "checkBoxSkipDraw";
			checkBoxSkipDraw.Size = new System.Drawing.Size(131, 29);
			checkBoxSkipDraw.TabIndex = 3;
			checkBoxSkipDraw.Text = "Hide Model";
			toolTip1.SetToolTip(checkBoxSkipDraw, "Prevents any models associated with the object from being drawn.\r\nFor objects with weighted meshes, this hides the entire mesh.");
			checkBoxSkipDraw.UseVisualStyleBackColor = true;
			checkBoxSkipDraw.Click += checkBoxSkipDraw_Click;
			// 
			// checkBoxIgnoreScl
			// 
			checkBoxIgnoreScl.AutoSize = true;
			checkBoxIgnoreScl.Location = new System.Drawing.Point(19, 107);
			checkBoxIgnoreScl.Name = "checkBoxIgnoreScl";
			checkBoxIgnoreScl.Size = new System.Drawing.Size(135, 29);
			checkBoxIgnoreScl.TabIndex = 2;
			checkBoxIgnoreScl.Text = "Ignore Scale";
			toolTip1.SetToolTip(checkBoxIgnoreScl, "If enabled, this ignores the object's scale values and defaults to 1, 1, 1.\r\n");
			checkBoxIgnoreScl.UseVisualStyleBackColor = true;
			checkBoxIgnoreScl.Click += checkBoxIgnoreScl_Click;
			// 
			// checkBoxIgnoreRot
			// 
			checkBoxIgnoreRot.AutoSize = true;
			checkBoxIgnoreRot.Location = new System.Drawing.Point(19, 72);
			checkBoxIgnoreRot.Name = "checkBoxIgnoreRot";
			checkBoxIgnoreRot.Size = new System.Drawing.Size(162, 29);
			checkBoxIgnoreRot.TabIndex = 1;
			checkBoxIgnoreRot.Text = "Ignore Rotation";
			toolTip1.SetToolTip(checkBoxIgnoreRot, "If enabled, this ignores the object's rotation values and defaults to 0, 0, 0.\r\n");
			checkBoxIgnoreRot.UseVisualStyleBackColor = true;
			checkBoxIgnoreRot.Click += checkBoxIgnoreRot_Click;
			// 
			// checkBoxIgnorePos
			// 
			checkBoxIgnorePos.AutoSize = true;
			checkBoxIgnorePos.Location = new System.Drawing.Point(19, 37);
			checkBoxIgnorePos.Name = "checkBoxIgnorePos";
			checkBoxIgnorePos.Size = new System.Drawing.Size(158, 29);
			checkBoxIgnorePos.TabIndex = 0;
			checkBoxIgnorePos.Text = "Ignore Position";
			toolTip1.SetToolTip(checkBoxIgnorePos, "If enabled, this ignores the object's position values and defaults to 0, 0, 0.");
			checkBoxIgnorePos.UseVisualStyleBackColor = true;
			checkBoxIgnorePos.Click += checkBoxIgnorePos_Click;
			// 
			// groupBoxPos
			// 
			groupBoxPos.Controls.Add(textBoxPosZ);
			groupBoxPos.Controls.Add(textBoxPosY);
			groupBoxPos.Controls.Add(label3);
			groupBoxPos.Controls.Add(label2);
			groupBoxPos.Controls.Add(label1);
			groupBoxPos.Controls.Add(textBoxPosX);
			groupBoxPos.Location = new System.Drawing.Point(314, 28);
			groupBoxPos.Name = "groupBoxPos";
			groupBoxPos.Size = new System.Drawing.Size(273, 171);
			groupBoxPos.TabIndex = 1;
			groupBoxPos.TabStop = false;
			groupBoxPos.Text = "Position";
			// 
			// textBoxPosZ
			// 
			textBoxPosZ.Location = new System.Drawing.Point(55, 118);
			textBoxPosZ.Name = "textBoxPosZ";
			textBoxPosZ.Size = new System.Drawing.Size(185, 31);
			textBoxPosZ.TabIndex = 5;
			// 
			// textBoxPosY
			// 
			textBoxPosY.Location = new System.Drawing.Point(55, 77);
			textBoxPosY.Name = "textBoxPosY";
			textBoxPosY.Size = new System.Drawing.Size(185, 31);
			textBoxPosY.TabIndex = 4;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(22, 121);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(26, 25);
			label3.TabIndex = 3;
			label3.Text = "Z:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(22, 80);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(26, 25);
			label2.TabIndex = 2;
			label2.Text = "Y:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(22, 39);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(27, 25);
			label1.TabIndex = 1;
			label1.Text = "X:";
			// 
			// textBoxPosX
			// 
			textBoxPosX.Location = new System.Drawing.Point(55, 36);
			textBoxPosX.Name = "textBoxPosX";
			textBoxPosX.Size = new System.Drawing.Size(185, 31);
			textBoxPosX.TabIndex = 0;
			// 
			// groupBoxRot
			// 
			groupBoxRot.Controls.Add(textBoxRotZ);
			groupBoxRot.Controls.Add(textBoxRotY);
			groupBoxRot.Controls.Add(label4);
			groupBoxRot.Controls.Add(label5);
			groupBoxRot.Controls.Add(label6);
			groupBoxRot.Controls.Add(textBoxRotX);
			groupBoxRot.Location = new System.Drawing.Point(314, 207);
			groupBoxRot.Name = "groupBoxRot";
			groupBoxRot.Size = new System.Drawing.Size(273, 171);
			groupBoxRot.TabIndex = 2;
			groupBoxRot.TabStop = false;
			groupBoxRot.Text = "Rotation";
			// 
			// textBoxRotZ
			// 
			textBoxRotZ.Location = new System.Drawing.Point(55, 118);
			textBoxRotZ.Name = "textBoxRotZ";
			textBoxRotZ.Size = new System.Drawing.Size(185, 31);
			textBoxRotZ.TabIndex = 11;
			// 
			// textBoxRotY
			// 
			textBoxRotY.Location = new System.Drawing.Point(55, 77);
			textBoxRotY.Name = "textBoxRotY";
			textBoxRotY.Size = new System.Drawing.Size(185, 31);
			textBoxRotY.TabIndex = 10;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(22, 121);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(26, 25);
			label4.TabIndex = 9;
			label4.Text = "Z:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(22, 80);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(26, 25);
			label5.TabIndex = 8;
			label5.Text = "Y:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(22, 39);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(27, 25);
			label6.TabIndex = 7;
			label6.Text = "X:";
			// 
			// textBoxRotX
			// 
			textBoxRotX.Location = new System.Drawing.Point(55, 36);
			textBoxRotX.Name = "textBoxRotX";
			textBoxRotX.Size = new System.Drawing.Size(185, 31);
			textBoxRotX.TabIndex = 6;
			// 
			// groupBoxScl
			// 
			groupBoxScl.Controls.Add(textBoxSclZ);
			groupBoxScl.Controls.Add(textBoxSclY);
			groupBoxScl.Controls.Add(label7);
			groupBoxScl.Controls.Add(label8);
			groupBoxScl.Controls.Add(label9);
			groupBoxScl.Controls.Add(textBoxSclX);
			groupBoxScl.Location = new System.Drawing.Point(314, 397);
			groupBoxScl.Name = "groupBoxScl";
			groupBoxScl.Size = new System.Drawing.Size(273, 173);
			groupBoxScl.TabIndex = 3;
			groupBoxScl.TabStop = false;
			groupBoxScl.Text = "Scale";
			// 
			// textBoxSclZ
			// 
			textBoxSclZ.Location = new System.Drawing.Point(55, 118);
			textBoxSclZ.Name = "textBoxSclZ";
			textBoxSclZ.Size = new System.Drawing.Size(185, 31);
			textBoxSclZ.TabIndex = 11;
			// 
			// textBoxSclY
			// 
			textBoxSclY.Location = new System.Drawing.Point(55, 77);
			textBoxSclY.Name = "textBoxSclY";
			textBoxSclY.Size = new System.Drawing.Size(185, 31);
			textBoxSclY.TabIndex = 10;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(22, 121);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(26, 25);
			label7.TabIndex = 9;
			label7.Text = "Z:";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(22, 80);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(26, 25);
			label8.TabIndex = 8;
			label8.Text = "Y:";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(22, 39);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(27, 25);
			label9.TabIndex = 7;
			label9.Text = "X:";
			// 
			// textBoxSclX
			// 
			textBoxSclX.Location = new System.Drawing.Point(55, 36);
			textBoxSclX.Name = "textBoxSclX";
			textBoxSclX.Size = new System.Drawing.Size(185, 31);
			textBoxSclX.TabIndex = 6;
			// 
			// buttonResetData
			// 
			buttonResetData.Location = new System.Drawing.Point(357, 582);
			buttonResetData.Name = "buttonResetData";
			buttonResetData.Size = new System.Drawing.Size(112, 34);
			buttonResetData.TabIndex = 4;
			buttonResetData.Text = "Reset Data";
			buttonResetData.UseVisualStyleBackColor = true;
			buttonResetData.Click += buttonResetData_Click;
			// 
			// buttonClose
			// 
			buttonClose.Location = new System.Drawing.Point(475, 582);
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new System.Drawing.Size(112, 34);
			buttonClose.TabIndex = 5;
			buttonClose.Text = "Close";
			buttonClose.UseVisualStyleBackColor = true;
			buttonClose.Click += buttonClose_Click;
			// 
			// ObjectSettingsEditor
			// 
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			ClientSize = new System.Drawing.Size(622, 637);
			Controls.Add(buttonClose);
			Controls.Add(buttonResetData);
			Controls.Add(groupBoxScl);
			Controls.Add(groupBoxRot);
			Controls.Add(groupBoxPos);
			Controls.Add(groupBoxEvalFlags);
			Name = "ObjectSettingsEditor";
			Text = "Object Settings Editor";
			Load += ObjectSettingsEditor_Load;
			groupBoxEvalFlags.ResumeLayout(false);
			groupBoxEvalFlags.PerformLayout();
			groupBoxPos.ResumeLayout(false);
			groupBoxPos.PerformLayout();
			groupBoxRot.ResumeLayout(false);
			groupBoxRot.PerformLayout();
			groupBoxScl.ResumeLayout(false);
			groupBoxScl.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxEvalFlags;
		private System.Windows.Forms.GroupBox groupBoxPos;
		private System.Windows.Forms.GroupBox groupBoxRot;
		private System.Windows.Forms.GroupBox groupBoxScl;
		private System.Windows.Forms.CheckBox checkBoxSkipDraw;
		private System.Windows.Forms.CheckBox checkBoxIgnoreScl;
		private System.Windows.Forms.CheckBox checkBoxIgnoreRot;
		private System.Windows.Forms.CheckBox checkBoxIgnorePos;
		private System.Windows.Forms.CheckBox checkBoxNoAnim;
		private System.Windows.Forms.CheckBox checkBoxZYXRot;
		private System.Windows.Forms.CheckBox checkBoxSkipChild;
		private System.Windows.Forms.CheckBox checkBoxNoShape;
		private System.Windows.Forms.CheckBox checkBoxModifier;
		private System.Windows.Forms.CheckBox checkBoxClip;
		private System.Windows.Forms.CheckBox checkBoxQuaternion;
		private System.Windows.Forms.CheckBox checkBoxEnvelope;
		private System.Windows.Forms.CheckBox checkBoxRotateSet;
		private System.Windows.Forms.CheckBox checkBoxRotateBase;
		private System.Windows.Forms.TextBox textBoxPosZ;
		private System.Windows.Forms.TextBox textBoxPosY;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxPosX;
		private System.Windows.Forms.TextBox textBoxRotZ;
		private System.Windows.Forms.TextBox textBoxRotY;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxRotX;
		private System.Windows.Forms.TextBox textBoxSclZ;
		private System.Windows.Forms.TextBox textBoxSclY;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textBoxSclX;
		private System.Windows.Forms.Button buttonResetData;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonClose;
	}
}