using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SAModel.Direct3D.TextureSystem;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ChunkModelBlendAlphaDataEditor : Form
	{
		#region Events

		public delegate void FormUpdatedHandler(object sender, EventArgs e);
		public event FormUpdatedHandler FormUpdated;

		#endregion

		private PolyChunk PolyData; // Poly data that is being edited
		private readonly PolyChunk PolyDataOriginal; // Original poly data at the time of opening the dialog

		public ChunkModelBlendAlphaDataEditor(PolyChunk mats, string matsName = null)
		{
			PolyData = mats;
			PolyDataOriginal = mats.Clone();
			InitializeComponent();
			if (!string.IsNullOrEmpty(matsName))
				this.Text = "Poly Data Editor: " + matsName;
		}

		private void MaterialEditor_Load(object sender, EventArgs e)
		{
			SetControls(PolyData);
		}

		#region UI Updates

		/// <summary>
		/// Populates the form with data from a material index.
		/// </summary>
		/// <param name="index">Index of the material to use.</param>
		private void SetControls(PolyChunk poly)
		{
			// Setting general
			PolyChunkBitsBlendAlpha pba = (PolyChunkBitsBlendAlpha)poly;
			srcAlphaCombo.SelectedIndex = (int)pba.SourceAlpha;
			dstAlphaCombo.SelectedIndex = (int)pba.DestinationAlpha;

			//DisplayFlags(index);

			// Material list controls
			resetButton.Enabled = true;
		}

		private void RaiseFormUpdated()
		{
			FormUpdated?.Invoke(this, EventArgs.Empty);
		}

		//private void DisplayFlags(PolyChunk)
		//{
		//	labelFlags.Text = String.Format("{0:X8}", materials[index].Flags);
		//}

		#endregion
		#region General Control Event Methods

		private void doneButton_Click(object sender, EventArgs e)
		{
			checkSettingsOnClose();
			Close();
		}
		//Because resetting sometimes applies incorrect changes
		private void checkSettingsOnClose()
		{
			PolyChunkBitsBlendAlpha pba = (PolyChunkBitsBlendAlpha)PolyData;
			pba.SourceAlpha = (AlphaInstruction)srcAlphaCombo.SelectedIndex;
			pba.DestinationAlpha = (AlphaInstruction)dstAlphaCombo.SelectedIndex;
		}

		#endregion
		#region Flag Check Event Methods

		private void userFlagsNumeric_ValueChanged(object sender, EventArgs e)
		{
			//materials[comboMaterial.SelectedIndex].UserFlags = (byte)userFlagsNumeric.Value;
			RaiseFormUpdated();
		}

		private void srcAlphaCombo_SelectionChangeCommitted(object sender, EventArgs e)
		{
			PolyChunkBitsBlendAlpha pba = (PolyChunkBitsBlendAlpha)PolyData;
			pba.SourceAlpha = (AlphaInstruction)srcAlphaCombo.SelectedIndex;
			RaiseFormUpdated();
		}

		private void dstAlphaCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			PolyChunkBitsBlendAlpha pba = (PolyChunkBitsBlendAlpha)PolyData;
			pba.DestinationAlpha = (AlphaInstruction)dstAlphaCombo.SelectedIndex;
			RaiseFormUpdated();
		}

		#endregion
		#region Material List Editing

		private void resetButton_Click(object sender, EventArgs e)
		{
			SetControls(PolyDataOriginal);
			RaiseFormUpdated();
		}
		#endregion

		private void generalSettingBox_Enter(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}
	}
}