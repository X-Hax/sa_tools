namespace SAModel.SAMDL
{
	partial class AnimOrientation
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
            this.labelPosition = new System.Windows.Forms.Label();
            this.positionXNumeric = new System.Windows.Forms.NumericUpDown();
            this.positionYNumeric = new System.Windows.Forms.NumericUpDown();
            this.positionZNumeric = new System.Windows.Forms.NumericUpDown();
            this.rotationZNumeric = new System.Windows.Forms.NumericUpDown();
            this.rotationYNumeric = new System.Windows.Forms.NumericUpDown();
            this.labelRotation = new System.Windows.Forms.Label();
            this.rotationXNumeric = new System.Windows.Forms.NumericUpDown();
            this.scaleZNumeric = new System.Windows.Forms.NumericUpDown();
            this.scaleYNumeric = new System.Windows.Forms.NumericUpDown();
            this.scaleLabel = new System.Windows.Forms.Label();
            this.scaleXNumeric = new System.Windows.Forms.NumericUpDown();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.positionXNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationXNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // labelPosition
            // 
            this.labelPosition.AutoSize = true;
            this.labelPosition.Location = new System.Drawing.Point(36, 9);
            this.labelPosition.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPosition.Name = "labelPosition";
            this.labelPosition.Size = new System.Drawing.Size(94, 15);
            this.labelPosition.TabIndex = 4;
            this.labelPosition.Text = "Position (X, Y, Z)";
            // 
            // positionXNumeric
            // 
            this.positionXNumeric.DecimalPlaces = 6;
            this.positionXNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.positionXNumeric.Location = new System.Drawing.Point(36, 27);
            this.positionXNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.positionXNumeric.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.positionXNumeric.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.positionXNumeric.Name = "positionXNumeric";
            this.positionXNumeric.Size = new System.Drawing.Size(65, 23);
            this.positionXNumeric.TabIndex = 5;
            this.positionXNumeric.ValueChanged += new System.EventHandler(this.positionXNumeric_ValueChanged);
            // 
            // positionYNumeric
            // 
            this.positionYNumeric.DecimalPlaces = 6;
            this.positionYNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.positionYNumeric.Location = new System.Drawing.Point(107, 27);
            this.positionYNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.positionYNumeric.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.positionYNumeric.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.positionYNumeric.Name = "positionYNumeric";
            this.positionYNumeric.Size = new System.Drawing.Size(65, 23);
            this.positionYNumeric.TabIndex = 6;
            this.positionYNumeric.ValueChanged += new System.EventHandler(this.positionYNumeric_ValueChanged);
            // 
            // positionZNumeric
            // 
            this.positionZNumeric.DecimalPlaces = 6;
            this.positionZNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.positionZNumeric.Location = new System.Drawing.Point(178, 27);
            this.positionZNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.positionZNumeric.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.positionZNumeric.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.positionZNumeric.Name = "positionZNumeric";
            this.positionZNumeric.Size = new System.Drawing.Size(65, 23);
            this.positionZNumeric.TabIndex = 7;
            this.positionZNumeric.ValueChanged += new System.EventHandler(this.positionZNumeric_ValueChanged);
            // 
            // rotationZNumeric
            // 
            this.rotationZNumeric.DecimalPlaces = 6;
            this.rotationZNumeric.Location = new System.Drawing.Point(178, 71);
            this.rotationZNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rotationZNumeric.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.rotationZNumeric.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.rotationZNumeric.Name = "rotationZNumeric";
            this.rotationZNumeric.Size = new System.Drawing.Size(65, 23);
            this.rotationZNumeric.TabIndex = 11;
            this.rotationZNumeric.ValueChanged += new System.EventHandler(this.rotationZNumeric_ValueChanged);
            // 
            // rotationYNumeric
            // 
            this.rotationYNumeric.DecimalPlaces = 6;
            this.rotationYNumeric.Location = new System.Drawing.Point(107, 71);
            this.rotationYNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rotationYNumeric.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.rotationYNumeric.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.rotationYNumeric.Name = "rotationYNumeric";
            this.rotationYNumeric.Size = new System.Drawing.Size(65, 23);
            this.rotationYNumeric.TabIndex = 10;
            this.rotationYNumeric.ValueChanged += new System.EventHandler(this.rotationYNumeric_ValueChanged);
            // 
            // labelRotation
            // 
            this.labelRotation.AutoSize = true;
            this.labelRotation.Location = new System.Drawing.Point(36, 53);
            this.labelRotation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelRotation.Name = "labelRotation";
            this.labelRotation.Size = new System.Drawing.Size(96, 15);
            this.labelRotation.TabIndex = 8;
            this.labelRotation.Text = "Rotation (X, Y, Z)";
            // 
            // rotationXNumeric
            // 
            this.rotationXNumeric.DecimalPlaces = 6;
            this.rotationXNumeric.Location = new System.Drawing.Point(36, 71);
            this.rotationXNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rotationXNumeric.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.rotationXNumeric.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.rotationXNumeric.Name = "rotationXNumeric";
            this.rotationXNumeric.Size = new System.Drawing.Size(65, 23);
            this.rotationXNumeric.TabIndex = 9;
            this.rotationXNumeric.ValueChanged += new System.EventHandler(this.rotationXNumeric_ValueChanged);
            // 
            // scaleZNumeric
            // 
            this.scaleZNumeric.DecimalPlaces = 6;
            this.scaleZNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.scaleZNumeric.Location = new System.Drawing.Point(178, 115);
            this.scaleZNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.scaleZNumeric.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.scaleZNumeric.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.scaleZNumeric.Name = "scaleZNumeric";
            this.scaleZNumeric.Size = new System.Drawing.Size(65, 23);
            this.scaleZNumeric.TabIndex = 15;
            this.scaleZNumeric.ValueChanged += new System.EventHandler(this.scaleZNumeric_ValueChanged);
            // 
            // scaleYNumeric
            // 
            this.scaleYNumeric.DecimalPlaces = 6;
            this.scaleYNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.scaleYNumeric.Location = new System.Drawing.Point(107, 115);
            this.scaleYNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.scaleYNumeric.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.scaleYNumeric.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.scaleYNumeric.Name = "scaleYNumeric";
            this.scaleYNumeric.Size = new System.Drawing.Size(65, 23);
            this.scaleYNumeric.TabIndex = 14;
            this.scaleYNumeric.ValueChanged += new System.EventHandler(this.scaleYNumeric_ValueChanged);
            // 
            // scaleLabel
            // 
            this.scaleLabel.AutoSize = true;
            this.scaleLabel.Location = new System.Drawing.Point(34, 97);
            this.scaleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.scaleLabel.Name = "scaleLabel";
            this.scaleLabel.Size = new System.Drawing.Size(78, 15);
            this.scaleLabel.TabIndex = 12;
            this.scaleLabel.Text = "Scale (X, Y, Z)";
            // 
            // scaleXNumeric
            // 
            this.scaleXNumeric.DecimalPlaces = 6;
            this.scaleXNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.scaleXNumeric.Location = new System.Drawing.Point(36, 115);
            this.scaleXNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.scaleXNumeric.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.scaleXNumeric.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.scaleXNumeric.Name = "scaleXNumeric";
            this.scaleXNumeric.Size = new System.Drawing.Size(65, 23);
            this.scaleXNumeric.TabIndex = 13;
            this.scaleXNumeric.ValueChanged += new System.EventHandler(this.scaleXNumeric_ValueChanged);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(48, 149);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(88, 27);
            this.okButton.TabIndex = 17;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click_1);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(143, 149);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(88, 27);
            this.cancelButton.TabIndex = 16;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click_1);
            // 
            // AnimOrientation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(284, 188);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.scaleZNumeric);
            this.Controls.Add(this.scaleYNumeric);
            this.Controls.Add(this.scaleLabel);
            this.Controls.Add(this.scaleXNumeric);
            this.Controls.Add(this.rotationZNumeric);
            this.Controls.Add(this.rotationYNumeric);
            this.Controls.Add(this.labelRotation);
            this.Controls.Add(this.rotationXNumeric);
            this.Controls.Add(this.positionZNumeric);
            this.Controls.Add(this.positionYNumeric);
            this.Controls.Add(this.labelPosition);
            this.Controls.Add(this.positionXNumeric);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnimOrientation";
            this.ShowInTaskbar = false;
            this.Text = "Enter a base orientation";
            ((System.ComponentModel.ISupportInitialize)(this.positionXNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationXNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelPosition;
		private System.Windows.Forms.NumericUpDown positionXNumeric;
		private System.Windows.Forms.NumericUpDown positionYNumeric;
		private System.Windows.Forms.NumericUpDown positionZNumeric;
		private System.Windows.Forms.NumericUpDown rotationZNumeric;
		private System.Windows.Forms.NumericUpDown rotationYNumeric;
		private System.Windows.Forms.Label labelRotation;
		private System.Windows.Forms.NumericUpDown rotationXNumeric;
		private System.Windows.Forms.NumericUpDown scaleZNumeric;
		private System.Windows.Forms.NumericUpDown scaleYNumeric;
		private System.Windows.Forms.Label scaleLabel;
		private System.Windows.Forms.NumericUpDown scaleXNumeric;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
	}
}

