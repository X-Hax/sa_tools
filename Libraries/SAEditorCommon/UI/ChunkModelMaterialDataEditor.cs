using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SAModel.Direct3D.TextureSystem;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ChunkModelMaterialDataEditor : Form
	{
		#region Events

		public delegate void FormUpdatedHandler(object sender, EventArgs e);
		public event FormUpdatedHandler FormUpdated;

		#endregion

		private PolyChunk PolyData; // Poly data that is being edited
		private readonly PolyChunk PolyDataOriginal; // Original poly data at the time of opening the dialog

		public ChunkModelMaterialDataEditor(PolyChunk mats, string matsName = null)
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
			PolyChunkMaterial pcm = (PolyChunkMaterial)poly;
			srcAlphaCombo.SelectedIndex = (int)pcm.SourceAlpha;
			dstAlphaCombo.SelectedIndex = (int)pcm.DestinationAlpha;
			diffuseSettingBox.Enabled = pcm.Diffuse.HasValue;
			ambientSettingBox.Enabled = pcm.Ambient.HasValue;
			specularSettingBox.Enabled = pcm.Specular.HasValue;

			if (pcm.Diffuse.HasValue)
			{
				diffuseRUpDown.Value = pcm.Diffuse.Value.R;
				diffuseGUpDown.Value = pcm.Diffuse.Value.G;
				diffuseBUpDown.Value = pcm.Diffuse.Value.B;
				alphaDiffuseNumeric.Value = pcm.Diffuse.Value.A;
			}
			if (pcm.Ambient.HasValue)
			{
				ambientRUpDown.Value = pcm.Ambient.Value.R;
				ambientGUpDown.Value = pcm.Ambient.Value.G;
				ambientBUpDown.Value = pcm.Ambient.Value.B;
			}
			if (pcm.Specular.HasValue)
			{
				specularRUpDown.Value = pcm.Specular.Value.R;
				specularGUpDown.Value = pcm.Specular.Value.G;
				specularBUpDown.Value = pcm.Specular.Value.B;
				alphaSpecularNumeric.Value = pcm.Specular.Value.A;
				exponentTextBox.Text = pcm.SpecularExponent.ToString();
			}
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

		private void diffuseColorBox_Click(object sender, EventArgs e)
		{
			PolyChunkMaterial pcm = (PolyChunkMaterial)PolyData;
			colorDialog.Color = pcm.Diffuse.Value;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				pcm.Diffuse = Color.FromArgb((int)alphaDiffuseNumeric.Value, colorDialog.Color);
				diffuseColorBox.BackColor = pcm.Diffuse.Value;
				diffuseRUpDown.Value = pcm.Diffuse.Value.R;
				diffuseGUpDown.Value = pcm.Diffuse.Value.G;
				diffuseBUpDown.Value = pcm.Diffuse.Value.B;
				RaiseFormUpdated();
			}
		}

		private void ambientColorBox_Click(object sender, EventArgs e)
		{
			PolyChunkMaterial pcm = (PolyChunkMaterial)PolyData;
			colorDialog.Color = pcm.Ambient.Value;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				pcm.Ambient = Color.FromArgb(255, colorDialog.Color);
				ambientColorBox.BackColor = pcm.Ambient.Value;
				ambientRUpDown.Value = pcm.Ambient.Value.R;
				ambientGUpDown.Value = pcm.Ambient.Value.G;
				ambientBUpDown.Value = pcm.Ambient.Value.B;
				RaiseFormUpdated();
			}
		}

		private void specColorBox_Click(object sender, EventArgs e)
		{
			PolyChunkMaterial pcm = (PolyChunkMaterial)PolyData;
			colorDialog.Color = pcm.Specular.Value;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				pcm.Specular = Color.FromArgb((int)alphaSpecularNumeric.Value, colorDialog.Color);
				specColorBox.BackColor = pcm.Specular.Value;
				specularRUpDown.Value = pcm.Specular.Value.R;
				specularGUpDown.Value = pcm.Specular.Value.G;
				specularBUpDown.Value = pcm.Specular.Value.B;
				RaiseFormUpdated();
			}
		}

		private void doneButton_Click(object sender, EventArgs e)
		{
			checkSettingsOnClose();
			if (exponentTextBox.Text.Length == 0)
			{
				Close();
			}
			else
			{
				ValidateExponent();
				Close();
			}
		}
		//Because resetting sometimes applies incorrect changes
		private void checkSettingsOnClose()
		{
			PolyChunkMaterial pcm = (PolyChunkMaterial)PolyData;
			if (pcm.Diffuse.HasValue)
				pcm.Diffuse = diffuseColorBox.BackColor;
			if (pcm.Ambient.HasValue)
				pcm.Ambient = ambientColorBox.BackColor;
			if (pcm.Specular.HasValue)
				pcm.Specular = specColorBox.BackColor;
			pcm.SourceAlpha = (AlphaInstruction)srcAlphaCombo.SelectedIndex;
			pcm.DestinationAlpha = (AlphaInstruction)dstAlphaCombo.SelectedIndex;
		}

		private void exponentTextBox_Leave(object sender, EventArgs e)
		{
			ValidateExponent();
			RaiseFormUpdated();
		}

		private void ValidateExponent()
		{
			// check to see if exponent can be parsed
			PolyChunkMaterial pcm = (PolyChunkMaterial)PolyData;
			if (!float.TryParse(exponentTextBox.Text, out float expParse))
			{
				MessageBox.Show("Specular exponent was invalid - setting to 10");
				pcm.SpecularExponent = 10;
			}
			else
			{
				pcm.SpecularExponent = (byte)expParse;
			}
		}

		private void alphaDiffuseNumeric_ValueChanged(object sender, EventArgs e)
		{
			SetDiffuseFromNumerics();
		}

		private void alphaSpecularNumeric_ValueChanged(object sender, EventArgs e)
		{
			SetSpecularFromNumerics();
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
			PolyChunkMaterial pcm = (PolyChunkMaterial)PolyData;
			pcm.SourceAlpha = (AlphaInstruction)srcAlphaCombo.SelectedIndex;
			RaiseFormUpdated();
		}

		private void dstAlphaCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			PolyChunkMaterial pcm = (PolyChunkMaterial)PolyData;
			pcm.DestinationAlpha = (AlphaInstruction)dstAlphaCombo.SelectedIndex;
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

		void SetDiffuseFromNumerics()
		{
			diffuseColorBox.BackColor = Color.FromArgb((int)alphaDiffuseNumeric.Value, (int)diffuseRUpDown.Value, (int)diffuseGUpDown.Value, (int)diffuseBUpDown.Value);
			//diffuseColorBox.BackColor = pcm.Diffuse.Value;
			RaiseFormUpdated();
		}
		void SetAmbientFromNumerics()
		{
			PolyChunkMaterial pcm = (PolyChunkMaterial)PolyData;
			ambientColorBox.BackColor = Color.FromArgb(0xFF, (int)ambientRUpDown.Value, (int)ambientGUpDown.Value, (int)ambientBUpDown.Value);
			//ambientColorBox.BackColor = pcm.Ambient.Value;
			RaiseFormUpdated();
		}
		void SetSpecularFromNumerics()
		{
			PolyChunkMaterial pcm = (PolyChunkMaterial)PolyData;
			specColorBox.BackColor = Color.FromArgb((int)alphaSpecularNumeric.Value, (int)specularRUpDown.Value, (int)specularGUpDown.Value, (int)specularBUpDown.Value);
			//specColorBox.BackColor = pcm.Specular.Value;
			RaiseFormUpdated();
		}

		private void diffuseRUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetDiffuseFromNumerics();
		}

		private void diffuseGUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetDiffuseFromNumerics();
		}

		private void diffuseBUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetDiffuseFromNumerics();
		}

		private void ambientRUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetAmbientFromNumerics();
		}

		private void ambientGUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetAmbientFromNumerics();
		}

		private void ambientBUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetAmbientFromNumerics();
		}

		private void specularRUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetSpecularFromNumerics();
		}

		private void specularGUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetSpecularFromNumerics();
		}

		private void specularBUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetSpecularFromNumerics();
		}
	}
}