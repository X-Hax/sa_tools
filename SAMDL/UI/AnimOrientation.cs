using System;
using System.Windows.Forms;

namespace SAModel.SAMDL
{
	public partial class AnimOrientation : Form
	{
		public Vertex Position = new Vertex();
		public Rotation AnimRotation = new Rotation();
		public Vertex AnimScale = new Vertex();
		public AnimOrientation()
		{
			InitializeComponent();
		}

		private void cancelButton_Click_1(object sender, EventArgs e)
		{
			NullUnused();
			Close();
		}

		private void okButton_Click_1(object sender, EventArgs e)
		{
			NullUnused();
			Close();
		}

		private void NullUnused()
		{
			if (Position.IsEmpty)
			{
				Position = null;
			}
			if (AnimRotation.IsEmpty)
			{
				AnimRotation = null;
			}
			if (AnimScale.IsEmpty)
			{
				AnimScale = null;
			}
		}
		private void positionXNumeric_ValueChanged(object sender, EventArgs e)
		{
			Position.X = (float)positionXNumeric.Value;
		}

		private void positionYNumeric_ValueChanged(object sender, EventArgs e)
		{
			Position.Y = (float)positionZNumeric.Value;
		}

		private void positionZNumeric_ValueChanged(object sender, EventArgs e)
		{
			Position.Z = (float)positionZNumeric.Value;
		}

		private void rotationXNumeric_ValueChanged(object sender, EventArgs e)
		{
			AnimRotation.X = Rotation.DegToBAMS((float)rotationXNumeric.Value);
		}

		private void rotationYNumeric_ValueChanged(object sender, EventArgs e)
		{
			AnimRotation.Y = Rotation.DegToBAMS((float)rotationYNumeric.Value);
		}

		private void rotationZNumeric_ValueChanged(object sender, EventArgs e)
		{
			AnimRotation.Z = Rotation.DegToBAMS((float)rotationZNumeric.Value);
		}

		private void scaleXNumeric_ValueChanged(object sender, EventArgs e)
		{
			AnimScale.X = (float)scaleXNumeric.Value;
		}

		private void scaleYNumeric_ValueChanged(object sender, EventArgs e)
		{
			AnimScale.Y = (float)scaleYNumeric.Value;
		}

		private void scaleZNumeric_ValueChanged(object sender, EventArgs e)
		{
			AnimScale.Z = (float)scaleZNumeric.Value;
		}
	}
}
