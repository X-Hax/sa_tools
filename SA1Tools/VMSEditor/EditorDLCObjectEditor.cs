using ArchiveLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static VMSEditor.SA1DLC;

namespace VMSEditor
{
	public partial class EditorDLCObjectEditor : Form
	{
		public VMS_DLC editorMeta = new VMS_DLC();
		PuyoFile puyo = new PuyoFile();

		public EditorDLCObjectEditor(VMS_DLC meta, PuyoFile pvm)
		{
			editorMeta = meta;
			puyo = pvm;
			InitializeComponent();
			InitializeMetadata();
		}

		private void InitializeMetadata()
		{
			comboBoxLevel.Items.Clear();
			comboBoxWarpLevel.Items.Clear();
			{
				for (int s = 0; s < LevelIDStrings.Length; s++)
				{
					comboBoxLevel.Items.Add(LevelIDStrings[s]);
					comboBoxWarpLevel.Items.Add(LevelIDStrings[s]);
				}
			}

			comboBoxMessage.Items.Clear();
			for (int s = 0; s < editorMeta.EnglishStrings.Length; s++)
				comboBoxMessage.Items.Add(s.ToString() + ": " + editorMeta.EnglishStrings[s]);
			RefreshObjectList();
		}

		private void listBoxObjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBoxObjects.SelectedIndex == -1)
				return;
			DLCObjectData item = editorMeta.Items[listBoxObjects.SelectedIndex];
			numericUpDownTriggerDistance.Value = item.TriggerDistance;
			numericUpDownSoundBank.Value = comboBoxWarpLevel.SelectedIndex = item.WarpLevelOrSoundbank;
			numericUpDownSoundID.Value = numericUpDownWarpAct.Value = item.WarpActOrSoundID;
			// Warp
			if (item.Flags.HasFlag(DLCObjectFlags.FLAG_WARP))
			{
				groupBoxPlaySound.Enabled = false;
				groupBoxWarp.Enabled = true;
			}
			// Sound
			else if (item.Flags.HasFlag(DLCObjectFlags.FLAG_SOUND))
			{
				groupBoxWarp.Enabled = false;
				groupBoxPlaySound.Enabled = true;
			}
			// Neither of the above
			else
			{
				groupBoxWarp.Enabled = false;
				groupBoxPlaySound.Enabled = false;
			}
			// Message
			comboBoxMessage.SelectedIndex = item.Message;
			comboBoxMessage.Enabled = labelMessage.Enabled = item.Flags.HasFlag(DLCObjectFlags.FLAG_MESSAGE);
			// Texture
			numericUpDownTextureID.Maximum = puyo.Entries.Count - 1;
			numericUpDownTextureID.Value = item.Texture;
			// Unknown
			numericUpDownUnknown.Value = item.Unknown3;
			// Location
			comboBoxLevel.SelectedIndex = item.Level;
			numericUpDownAct.Value = item.Act;
			numericUpDownCollectibleID.Value = item.InternalID;
			numericUpDownPosX.Value = item.X;
			numericUpDownPosY.Value = item.Y;
			numericUpDownPosZ.Value = item.Z;
			numericUpDownRotX.Value = item.RotationX;
			numericUpDownRotY.Value = item.RotationY;
			numericUpDownRotZ.Value = item.RotationZ;
			numericUpDownRotSpeedX.Value = item.RotSpeedX;
			numericUpDownRotSpeedY.Value = item.RotSpeedY;
			numericUpDownRotSpeedZ.Value = item.RotSpeedZ;
			numericUpDownScaleX.Value = item.ScaleX;
			numericUpDownScaleY.Value = item.ScaleY;
			numericUpDownScaleZ.Value = item.ScaleZ;
			// Add flags
			checkBoxFlagMessage.Checked = item.Flags.HasFlag(DLCObjectFlags.FLAG_MESSAGE);
			checkBoxFlagCollectible.Checked = item.Flags.HasFlag(DLCObjectFlags.FLAG_COLLECTIBLE);
			checkBoxFlagCollisionOnly.Checked = item.Flags.HasFlag(DLCObjectFlags.FLAG_COLLISION_ONLY);
			checkBoxFlagSolid.Checked = item.Flags.HasFlag(DLCObjectFlags.FLAG_SOLID);
			checkBoxFlagSound.Checked = item.Flags.HasFlag(DLCObjectFlags.FLAG_SOUND);
			checkBoxFlagStart.Checked = item.Flags.HasFlag(DLCObjectFlags.FLAG_CHALLENGE);
			checkBoxFlagTimer.Checked = item.Flags.HasFlag(DLCObjectFlags.FLAG_TIMER);
			checkBoxFlagWarp.Checked = item.Flags.HasFlag(DLCObjectFlags.FLAG_WARP);
			checkBoxFlagUnknown0.Checked = item.Flags.HasFlag(DLCObjectFlags.UNKNOWN_0);
			checkBoxFlagUnknown1.Checked = item.Flags.HasFlag(DLCObjectFlags.UNUSED_1);
			checkBoxFlagUnknown2.Checked = item.Flags.HasFlag(DLCObjectFlags.UNUSED_2);
			checkBoxFlagUnknown3.Checked = item.Flags.HasFlag(DLCObjectFlags.UNUSED_3);
			checkBoxFlagUnknown4.Checked = item.Flags.HasFlag(DLCObjectFlags.UNKNOWN_4);
			checkBoxFlagUnknown5.Checked = item.Flags.HasFlag(DLCObjectFlags.UNUSED_5);
			checkBoxFlagUnknown6.Checked = item.Flags.HasFlag(DLCObjectFlags.UNUSED_6);
			checkBoxFlagUnknown7.Checked = item.Flags.HasFlag(DLCObjectFlags.UNUSED_7);
			switch (item.ObjectType)
			{
				case DLCObjectTypes.TYPE_MODEL:
					comboBoxType.SelectedIndex = 0;
					break;
				case DLCObjectTypes.TYPE_SPRITE:
					comboBoxType.SelectedIndex = 1;
					break;
				case DLCObjectTypes.TYPE_INVISIBLE:
					comboBoxType.SelectedIndex = 2;
					break;
			}
		}

		private void comboBoxMessage_MouseHover(object sender, EventArgs e)
		{
			ToolTip tt = new ToolTip();
			IWin32Window win = this;
			System.Drawing.Point mousePosition = Cursor.Position;
			tt.Show(comboBoxMessage.Text, win, this.PointToClient(new Point(mousePosition.X+60, mousePosition.Y + 60)), 2000);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void RefreshObjectList()
		{
			listBoxObjects.Items.Clear();
			int count = 0;
			foreach (DLCObjectData item in editorMeta.Items)
			{
				string simpleType;
				switch (item.ObjectType)
				{
					case DLCObjectTypes.TYPE_INVISIBLE:
						simpleType = "Collision";
						break;
					case DLCObjectTypes.TYPE_MODEL:
						simpleType = "Model";
						break;
					case DLCObjectTypes.TYPE_SPRITE:
					default:
						simpleType = "Banner";
						break;
				}
				if (item.Flags.HasFlag(DLCObjectFlags.FLAG_TIMER))
				{
					if (item.Flags.HasFlag(DLCObjectFlags.FLAG_MESSAGE))
						simpleType += " (Finish)";
					else
						simpleType = "Timer";
				}
				else if (item.Flags.HasFlag(DLCObjectFlags.FLAG_CHALLENGE))
					simpleType += " (Start)";
				if (item.Flags.HasFlag(DLCObjectFlags.FLAG_COLLECTIBLE))
					simpleType += " (Collectible)";
				if (item.Flags.HasFlag(DLCObjectFlags.FLAG_WARP))
					simpleType += " (Warp)";
				listBoxObjects.Items.Add(count.ToString() + ": " + simpleType + " in " + LevelIDStrings[item.Level] + " " + item.Act.ToString());
				count++;
			}
		}

		private void buttonMoveUp_Click(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index <= 0)
				return;
			ExtensionMethods.Swap(editorMeta.Items, listBoxObjects.SelectedIndex, listBoxObjects.SelectedIndex - 1);
			RefreshObjectList();
			listBoxObjects.SelectedIndex = index - 1;
		}

		private void buttonMoveDown_Click(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0 || index >= listBoxObjects.Items.Count - 1)
				return;
			ExtensionMethods.Swap(editorMeta.Items, listBoxObjects.SelectedIndex, listBoxObjects.SelectedIndex + 1);
			RefreshObjectList();
			listBoxObjects.SelectedIndex = index + 1;
		}

		private void buttonDuplicate_Click(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData origdata = editorMeta.Items[listBoxObjects.SelectedIndex];
			DLCObjectData newdata = new DLCObjectData(origdata);
			editorMeta.Items.Insert(index + 1, newdata);
			RefreshObjectList();
			listBoxObjects.SelectedIndex = index + 1;
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			editorMeta.Items.RemoveAt(index);
			RefreshObjectList();
			if (index > listBoxObjects.Items.Count - 1)
				listBoxObjects.SelectedIndex = listBoxObjects.Items.Count - 1;
			else
				listBoxObjects.SelectedIndex = index;
		}

		private void numericUpDownTextureID_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			if (numericUpDownTextureID.Value < puyo.Entries.Count)
			{
				DLCObjectData data = editorMeta.Items[index];
				data.Texture = (byte)numericUpDownTextureID.Value;
				pictureBoxTexture.Image = puyo.Entries[data.Texture].GetBitmap();
				editorMeta.Items[index] = data;
			}
		}

		private void ToggleFlag(object sender, DLCObjectFlags flag)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			CheckBox check = (CheckBox)sender;
			if (check.Checked)
				data.Flags |= flag;
			else
				data.Flags &= ~flag;
			groupBoxWarp.Enabled = data.Flags.HasFlag(DLCObjectFlags.FLAG_WARP);
			groupBoxPlaySound.Enabled = data.Flags.HasFlag(DLCObjectFlags.FLAG_SOUND);
			comboBoxMessage.Enabled = labelMessage.Enabled = data.Flags.HasFlag(DLCObjectFlags.FLAG_MESSAGE);
			editorMeta.Items[index] = data;
			RefreshObjectList();
			listBoxObjects.SelectedIndex = index;
		}

		private void checkBoxFlagSolid_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.FLAG_SOLID);
		}

		private void checkBoxFlagSound_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.FLAG_SOUND);
		}

		private void checkBoxFlagMessage_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.FLAG_MESSAGE);
		}

		private void checkBoxFlagCollisionOnly_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.FLAG_COLLISION_ONLY);
		}


		private void checkBoxFlagWarp_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.FLAG_WARP);
		}

		private void checkBoxFlagCollectible_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.FLAG_COLLECTIBLE);

		}

		private void checkBoxFlagTimer_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.FLAG_TIMER);
		}

		private void checkBoxFlagStart_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.FLAG_CHALLENGE);
		}

		private void checkBoxFlagUnknown0_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.UNKNOWN_0);
		}

		private void checkBoxFlagUnknown1_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.UNUSED_1);
		}

		private void checkBoxFlagUnknown2_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.UNUSED_2);
		}

		private void checkBoxFlagUnknown3_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.UNUSED_3);
		}

		private void checkBoxFlagUnknown4_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.UNKNOWN_4);
		}

		private void checkBoxFlagUnknown5_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.UNUSED_5);
		}

		private void checkBoxFlagUnknown6_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.UNUSED_6);
		}

		private void checkBoxFlagUnknown7_Click(object sender, EventArgs e)
		{
			ToggleFlag(sender, DLCObjectFlags.UNUSED_7);
		}

		private void numericUpDownUnknown_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.Unknown3 = (byte)numericUpDownUnknown.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownTriggerDistance_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.TriggerDistance = (byte)numericUpDownTriggerDistance.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownWarpAct_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.WarpActOrSoundID = (byte)numericUpDownWarpAct.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownAct_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.Act = (byte)numericUpDownAct.Value;
			editorMeta.Items[index] = data;
			RefreshObjectList();
			listBoxObjects.SelectedIndex = index;
		}

		private void numericUpDownCollectibleID_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.InternalID = (byte)numericUpDownCollectibleID.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownPosX_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.X = (short)numericUpDownPosX.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownPosY_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.Y = (short)numericUpDownPosY.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownPosZ_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.Z = (short)numericUpDownPosZ.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownRotX_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.RotationX = (ushort)numericUpDownRotX.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownRotY_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.RotationY = (ushort)numericUpDownRotY.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownRotZ_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.RotationZ = (ushort)numericUpDownRotZ.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownScaleX_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.ScaleX = (byte)numericUpDownScaleX.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownScaleY_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.ScaleY = (byte)numericUpDownScaleY.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownScaleZ_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.ScaleZ = (byte)numericUpDownScaleZ.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownRotSpeedX_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.RotSpeedX = (sbyte)numericUpDownRotSpeedX.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownRotSpeedY_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.RotSpeedY = (sbyte)numericUpDownRotSpeedY.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownRotSpeedZ_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.RotSpeedZ = (sbyte)numericUpDownRotSpeedZ.Value;
			editorMeta.Items[index] = data;
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Yes;
		}

		private void numericUpDownSoundID_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.WarpActOrSoundID = (byte)numericUpDownSoundID.Value;
			editorMeta.Items[index] = data;
		}

		private void comboBoxMessage_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.Message = (byte)comboBoxMessage.SelectedIndex;
			editorMeta.Items[index] = data;
		}

		private void comboBoxWarpLevel_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.WarpLevelOrSoundbank = (byte)comboBoxWarpLevel.SelectedIndex;
			editorMeta.Items[index] = data;
		}

		private void comboBoxLevel_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.Level = (byte)comboBoxLevel.SelectedIndex;
			editorMeta.Items[index] = data;
			RefreshObjectList();
			listBoxObjects.SelectedIndex = index;
		}

		private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			DLCObjectTypes result;
			switch (comboBoxType.SelectedIndex)
			{
				case 1:
					result = DLCObjectTypes.TYPE_SPRITE;
					break;
				case 2:
					result = DLCObjectTypes.TYPE_INVISIBLE;
					break;
				case 0:
				default:
					result = DLCObjectTypes.TYPE_MODEL;
					break;
			}
			data.ObjectType = result;
			editorMeta.Items[index] = data;
			RefreshObjectList();
			listBoxObjects.SelectedIndex = index;
		}

		private void numericUpDownSoundBank_ValueChanged(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index < 0)
				return;
			DLCObjectData data = editorMeta.Items[index];
			data.WarpLevelOrSoundbank = (byte)numericUpDownSoundBank.Value;
			editorMeta.Items[index] = data;
		}

		private void numericUpDownTriggerDistance_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Change object trigger distance. Increasing the value makes it possible to touch the object from farther away.";
		}

		private void comboBoxWarpLevel_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Teleport the player to a specified level. This value is shared with the soundbank ID.";
		}

		private void numericUpDownWarpAct_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Teleport the player to a specified act. This value is shared with the sound ID.";
		}

		private void numericUpDownSoundBank_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Play a sound from a specified soundbank. This value is shared with warp level ID. See Help for more information.";
		}

		private void numericUpDownSoundID_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Play a sound with a specified ID. This value is shared with warp act. See Help for more information.";
		}

		private void comboBoxMessage_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Select the message to display when the player touches the object. The 'Show Message' flag must be enabled for this to work.";
		}

		private void numericUpDownTextureID_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Select the object's texture.";
		}

		private void comboBoxLevel_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Select the level to place this object.";
		}

		private void numericUpDownAct_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Select the act ID to place this object.";
		}

		private void comboBoxType_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Select the kind of object. 'Model' uses the stored 3D model, 'Sprite' uses a flat model.";
		}

		private void numericUpDownCollectibleID_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "'Collectible': unique object ID. 'Timer': total number of collectibles. 'Challenge': time limit (60 = 10 minutes).";
		}

		private void numericUpDownScaleX_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Set object scale by axis. The applied value is divided by 10, e.g. 10 is 1.0, 25 is 2.5 etc.";
		}

		private void numericUpDownScaleY_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Set object scale by axis. The applied value is divided by 10, e.g. 10 is 1.0, 25 is 2.5 etc.";
		}

		private void numericUpDownScaleZ_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Set object scale by axis. The applied value is divided by 10, e.g. 10 is 1.0, 25 is 2.5 etc.";
		}

		private void numericUpDownRotSpeedX_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Set rotation speed by axis. The higher the value the faster the object rotates.";
		}

		private void numericUpDownRotSpeedY_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Set rotation speed by axis. The higher the value the faster the object rotates.";
		}

		private void numericUpDownRotSpeedZ_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Set rotation speed by axis. The higher the value the faster the object rotates.";
		}

		private void numericUpDownRotX_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Set initial object rotation.";
		}

		private void numericUpDownRotY_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Set initial object rotation.";
		}

		private void numericUpDownRotZ_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Set initial object rotation.";
		}

		private void checkBoxFlagSolid_MouseHover(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "This flag enables collision for the object.";
		}

		private void checkBoxFlagSound_MouseHover(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "Play a sound when the player touches the object.";
		}

		private void checkBoxFlagMessage_MouseHover(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "Show a message when the player touches the object.";
		}

		private void checkBoxFlagCollisionOnly_MouseHover(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "Disable all actions and only use the object as collision.";
		}

		private void checkBoxFlagWarp_MouseHover(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "Teleport the player to another level when the object is activated.";
		}

		private void checkBoxFlagCollectible_MouseHover(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "This flag marks the object as a collectible item in the challenge.";
		}

		private void checkBoxFlagTimer_MouseHover(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "This flag makes the object track collectibles and keep time.";
		}

		private void checkBoxFlagStart_MouseHover(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "Start the timer when the player touches the object.";
		}

		private void checkBoxFlagUnknown0_MouseEnter(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "";
		}

		private void checkBoxFlagUnknown4_MouseEnter(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "";
		}

		private void numericUpDownPosX_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Object position.";
		}

		private void numericUpDownPosY_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Object position.";
		}

		private void numericUpDownPosZ_MouseClick(object sender, MouseEventArgs e)
		{
			hintTextToolStrip.Text = "Object position.";
		}

		private void numericUpDownUnknown_Click(object sender, EventArgs e)
		{
			hintTextToolStrip.Text = "";

		}

		private void buttonAddNew_Click(object sender, EventArgs e)
		{
			int index = listBoxObjects.SelectedIndex;
			if (index == -1)
				index = 0;
			DLCObjectData newdata = new DLCObjectData();
			editorMeta.Items.Insert(index, newdata);
			RefreshObjectList();
			listBoxObjects.SelectedIndex = index;
		}
	}

	public static class ExtensionMethods
	{
		public static void Swap<T>(this List<T> list, int index1, int index2)
		{
			T temp = list[index1];
			list[index1] = list[index2];
			list[index2] = temp;
		}
	}
}