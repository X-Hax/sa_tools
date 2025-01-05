using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SAModel.Direct3D.TextureSystem;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ChunkModelStripDataEditor : Form
	{
		#region Events

		public delegate void FormUpdatedHandler(object sender, EventArgs e);
		public event FormUpdatedHandler FormUpdated;

		#endregion

		private PolyChunk PolyData; // Poly data that is being edited
		private readonly PolyChunk PolyDataOriginal; // Original poly data at the time of opening the dialog

		public ChunkModelStripDataEditor(PolyChunk mats, string matsName = null)
		{
			PolyData = mats;
			PolyDataOriginal = mats.Clone();
			InitializeComponent();
			if (!string.IsNullOrEmpty(matsName))
				this.Text = "Strip Data Editor: " + matsName;
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
			PolyChunkStrip pcs = (PolyChunkStrip)poly;

			// Setting flags
			ignoreAmbiCheck.Checked = pcs.IgnoreAmbient;
			ignoreSpecCheck.Checked = pcs.IgnoreSpecular;
			useAlphaCheck.Checked = pcs.UseAlpha;
			envMapCheck.Checked = pcs.EnvironmentMapping;
			doubleSideCheck.Checked = pcs.DoubleSide;
			flatShadeCheck.Checked = pcs.FlatShading;
			ignoreLightCheck.Checked = pcs.IgnoreLight;
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

		#endregion
		#region Flag Check Event Methods

		//Because resetting sometimes applies incorrect changes
		private void checkSettingsOnClose()
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.UseAlpha = useAlphaCheck.Checked;
			pcs.EnvironmentMapping = envMapCheck.Checked;
			pcs.DoubleSide = doubleSideCheck.Checked;
			pcs.FlatShading = flatShadeCheck.Checked;
			pcs.IgnoreLight = ignoreLightCheck.Checked;
			pcs.IgnoreAmbient = ignoreAmbiCheck.Checked;
			pcs.IgnoreSpecular = ignoreSpecCheck.Checked;
		}

		private void ignoreAmbiCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.IgnoreAmbient = ignoreAmbiCheck.Checked;
			RaiseFormUpdated();
		}

		private void ignoreSpecCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.IgnoreSpecular = ignoreSpecCheck.Checked;
			RaiseFormUpdated();
		}

		private void useAlphaCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.UseAlpha = useAlphaCheck.Checked;
			RaiseFormUpdated();
		}

		private void useTextureCheck_Click(object sender, EventArgs e)
		{
			//materials[comboMaterial.SelectedIndex].UseTexture = useTextureCheck.Checked;
			RaiseFormUpdated();
		}

		private void envMapCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.EnvironmentMapping = envMapCheck.Checked;
			RaiseFormUpdated();
		}

		private void doubleSideCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.DoubleSide = doubleSideCheck.Checked;
			RaiseFormUpdated();
		}

		private void flatShadeCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.FlatShading = flatShadeCheck.Checked;
			RaiseFormUpdated();
		}

		private void ignoreLightCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.IgnoreLight = ignoreLightCheck.Checked;
			RaiseFormUpdated();
		}

		private void userFlagsNumeric_ValueChanged(object sender, EventArgs e)
		{
			//materials[comboMaterial.SelectedIndex].UserFlags = (byte)userFlagsNumeric.Value;
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