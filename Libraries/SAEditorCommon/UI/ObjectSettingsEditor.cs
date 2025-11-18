using System;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ObjectSettingsEditor : Form
	{
		#region Events

		public delegate void FormUpdatedHandler(object sender, EventArgs e);
		public event FormUpdatedHandler FormUpdated;

		#endregion

		private NJS_OBJECT objData; // Object data that is being edited
		private readonly NJS_OBJECT objDataOriginal; // Original object data at the time of opening the dialog

		public ObjectSettingsEditor(NJS_OBJECT obj, string objName = null)
		{
			objData = obj;
			objDataOriginal = obj.Clone();
			InitializeComponent();
			if (!string.IsNullOrEmpty(objName))
				this.Text = "Object Settings Editor: " + objName;
		}
		private void ObjectSettingsEditor_Load(object sender, EventArgs e)
		{
			SetControls(objData.Clone());
		}

		private void SetControls(NJS_OBJECT obj)
		{
			// Setting general
			checkBoxIgnorePos.Checked = (obj.Flags & ObjectFlags.NoPosition) != 0;
			checkBoxIgnoreRot.Checked = (obj.Flags & ObjectFlags.NoRotate) != 0;
			checkBoxIgnoreScl.Checked = (obj.Flags & ObjectFlags.NoScale) != 0;
			checkBoxSkipDraw.Checked = (obj.Flags & ObjectFlags.NoDisplay) != 0;
			checkBoxSkipChild.Checked = (obj.Flags & ObjectFlags.NoChildren) != 0;
			checkBoxZYXRot.Checked = (obj.Flags & ObjectFlags.RotateZYX) != 0;
			checkBoxNoAnim.Checked = (obj.Flags & ObjectFlags.NoAnimate) != 0;
			checkBoxNoShape.Checked = (obj.Flags & ObjectFlags.NoMorph) != 0;
			checkBoxClip.Checked = (obj.Flags & ObjectFlags.Clip) != 0;
			checkBoxModifier.Checked = (obj.Flags & ObjectFlags.Modifier) != 0;
			checkBoxQuaternion.Checked = (obj.Flags & ObjectFlags.Quaternion) != 0;
			checkBoxRotateBase.Checked = (obj.Flags & ObjectFlags.RotateBase) != 0;
			checkBoxRotateSet.Checked = (obj.Flags & ObjectFlags.RotateSet) != 0;
			checkBoxEnvelope.Checked = (obj.Flags & ObjectFlags.Envelope) != 0;

			textBoxPosX.Text = obj.Position.X.ToString();
			textBoxPosY.Text = obj.Position.Y.ToString();
			textBoxPosZ.Text = obj.Position.Z.ToString();
			textBoxRotX.Text = obj.Rotation.X.ToString("X");
			textBoxRotY.Text = obj.Rotation.Y.ToString("X");
			textBoxRotZ.Text = obj.Rotation.Z.ToString("X");
			textBoxSclX.Text = obj.Scale.X.ToString();
			textBoxSclY.Text = obj.Scale.Y.ToString();
			textBoxSclZ.Text = obj.Scale.Z.ToString();

			// Material list controls
			RaiseFormUpdated();
		}

		private void RaiseFormUpdated()
		{
			FormUpdated?.Invoke(this, EventArgs.Empty);
		}
		private void checkSettingsOnClose()
		{
			ObjectFlags newflags = 0;
			if (checkBoxIgnorePos.Checked || (float.Parse(textBoxPosX.Text) == 0 && float.Parse(textBoxPosY.Text) == 0
				&& float.Parse(textBoxPosZ.Text) == 0))
				newflags |= ObjectFlags.NoPosition;
			if (checkBoxIgnoreRot.Checked || (int.Parse(textBoxRotX.Text, System.Globalization.NumberStyles.HexNumber) == 0 && int.Parse(textBoxRotY.Text, System.Globalization.NumberStyles.HexNumber) == 0
				&& int.Parse(textBoxRotZ.Text, System.Globalization.NumberStyles.HexNumber) == 0))
				newflags |= ObjectFlags.NoRotate;
			if (checkBoxIgnoreScl.Checked || (float.Parse(textBoxSclX.Text) == 1.0f && float.Parse(textBoxSclY.Text) == 1.0f
				&& float.Parse(textBoxSclZ.Text) == 1.0f))
				newflags |= ObjectFlags.NoScale;
			if (checkBoxSkipDraw.Checked)
				newflags |= ObjectFlags.NoDisplay;
			if (checkBoxSkipChild.Checked)
				newflags |= ObjectFlags.NoChildren;
			if (checkBoxZYXRot.Checked)
				newflags |= ObjectFlags.RotateZYX;
			if (checkBoxNoAnim.Checked)
				newflags |= ObjectFlags.NoAnimate;
			if (checkBoxNoShape.Checked)
				newflags |= ObjectFlags.NoMorph;
			if (checkBoxClip.Checked)
				newflags |= ObjectFlags.Clip;
			if (checkBoxModifier.Checked)
				newflags |= ObjectFlags.Modifier;
			if (checkBoxQuaternion.Checked)
				newflags |= ObjectFlags.Quaternion;
			if (checkBoxRotateBase.Checked)
				newflags |= ObjectFlags.RotateBase;
			if (checkBoxRotateSet.Checked)
				newflags |= ObjectFlags.RotateSet;
			if (checkBoxEnvelope.Checked)
				newflags |= ObjectFlags.Envelope;
			objData.Flags = newflags;
			objData.Position.X = float.Parse(textBoxPosX.Text);
			objData.Position.Y = float.Parse(textBoxPosY.Text);
			objData.Position.Z = float.Parse(textBoxPosZ.Text);
			objData.Rotation.X = int.Parse(textBoxRotX.Text, System.Globalization.NumberStyles.HexNumber);
			objData.Rotation.Y = int.Parse(textBoxRotY.Text, System.Globalization.NumberStyles.HexNumber);
			objData.Rotation.Z = int.Parse(textBoxRotZ.Text, System.Globalization.NumberStyles.HexNumber);
			objData.Scale.X = float.Parse(textBoxSclX.Text);
			objData.Scale.Y = float.Parse(textBoxSclY.Text);
			objData.Scale.Z = float.Parse(textBoxSclZ.Text);
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			checkSettingsOnClose();
			Close();
		}

		private void buttonResetData_Click(object sender, EventArgs e)
		{
			SetControls(objDataOriginal.Clone());
		}

		private void checkBoxIgnorePos_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxIgnorePos.Checked ? ObjectFlags.NoPosition : ~ObjectFlags.NoPosition;
			RaiseFormUpdated();
		}

		private void checkBoxIgnoreRot_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxIgnoreRot.Checked ? ObjectFlags.NoRotate : ~ObjectFlags.NoRotate;
			RaiseFormUpdated();
		}

		private void checkBoxIgnoreScl_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxIgnoreScl.Checked ? ObjectFlags.NoScale : ~ObjectFlags.NoScale;
			RaiseFormUpdated();
		}

		private void checkBoxSkipDraw_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxSkipDraw.Checked ? ObjectFlags.NoDisplay : ~ObjectFlags.NoDisplay;
			RaiseFormUpdated();
		}

		private void checkBoxSkipChild_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxSkipChild.Checked ? ObjectFlags.NoChildren : ~ObjectFlags.NoChildren;
			RaiseFormUpdated();
		}

		private void checkBoxZYXRot_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxZYXRot.Checked ? ObjectFlags.RotateZYX : ~ObjectFlags.RotateZYX;
			RaiseFormUpdated();
		}

		private void checkBoxNoAnim_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxNoAnim.Checked ? ObjectFlags.NoAnimate : ~ObjectFlags.NoAnimate;
			RaiseFormUpdated();
		}

		private void checkBoxNoShape_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxNoShape.Checked ? ObjectFlags.NoMorph : ~ObjectFlags.NoMorph;
			RaiseFormUpdated();
		}

		private void checkBoxClip_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxClip.Checked ? ObjectFlags.Clip : ~ObjectFlags.Clip;
			RaiseFormUpdated();
		}

		private void checkBoxModifier_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxModifier.Checked ? ObjectFlags.Modifier : ~ObjectFlags.Modifier;
			RaiseFormUpdated();
		}

		private void checkBoxQuaternion_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxQuaternion.Checked ? ObjectFlags.Quaternion : ~ObjectFlags.Quaternion;
			RaiseFormUpdated();
		}

		private void checkBoxRotateBase_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxRotateBase.Checked ? ObjectFlags.RotateBase : ~ObjectFlags.RotateBase;
			RaiseFormUpdated();
		}

		private void checkBoxRotateSet_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxRotateSet.Checked ? ObjectFlags.RotateSet : ~ObjectFlags.RotateSet;
			RaiseFormUpdated();
		}

		private void checkBoxEnvelope_Click(object sender, EventArgs e)
		{
			objData.Flags |= checkBoxEnvelope.Checked ? ObjectFlags.Envelope : ~ObjectFlags.Envelope;
			RaiseFormUpdated();
		}
	}
}
