using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SAModel.Direct3D.TextureSystem;
using SAModel.GC;

namespace SAModel.SAEditorCommon.UI
{
	public partial class GCMaterialEditor : Form
	{
		#region Events

		public delegate void FormUpdatedHandler(object sender, EventArgs e);
		public event FormUpdatedHandler FormUpdated;

		#endregion

		private readonly List<NJS_MATERIAL> materials;
		private readonly List<NJS_MATERIAL> materialsOriginal; // Original material list at the time of opening the dialog
		private readonly BMPInfo[] textures;

		public GCMaterialEditor(List<NJS_MATERIAL> mats, BMPInfo[] textures, string matsName = null)
		{
			materials = mats;
			materialsOriginal = new List<NJS_MATERIAL>();
			foreach (NJS_MATERIAL material in mats)
			{
				materialsOriginal.Add(material.Clone());
			}
			this.textures = textures;
			InitializeComponent();
			if (!string.IsNullOrEmpty(matsName))
				this.Text = "Material Editor: " + matsName;
		}

		private void MaterialEditor_Load(object sender, EventArgs e)
		{
			UpdateComboBox();
		}

		#region UI Updates
		private Image DrawPreviewImage(Image image)
		{
			Bitmap bmp = new Bitmap(textureBox.Size.Width, textureBox.Size.Height);
			float scale = (float)image.Width / (float)image.Height;
			int xpos = 0;
			int ypos = 0;
			int width = textureBox.Size.Width;
			int height = textureBox.Size.Width;
			if (scale > 1.0f)
			{
				width = textureBox.Size.Width;
				height = (int)(textureBox.Size.Height / scale);
				ypos = (textureBox.Size.Height - height) / 2;
			}
			else if (scale < 1.0f)
			{
				width = (int)(textureBox.Size.Width * scale);
				height = textureBox.Size.Height;
				xpos = (textureBox.Size.Width - width) / 2;
			}
			using (Graphics gfx = Graphics.FromImage(bmp))
			{
				gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
				gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				gfx.DrawImage(image, xpos, ypos, width, height);
			}
			return bmp;
		}

		/// <summary>
		/// Populates the form with data from a material index.
		/// </summary>
		/// <param name="index">Index of the material to use.</param>
		private void SetControls(int index)
		{
			// setting general
			diffuseColorBox.BackColor = materials[index].DiffuseColor;
			alphaDiffuseNumeric.Value = materials[index].DiffuseColor.A;
			if(textures != null && materials[index].TextureID < textures.Length) textureBox.Image = DrawPreviewImage(textures[materials[index].TextureID].Image);
			filterModeDropDown.SelectedIndex = (int)materials[index].FilterMode;
			srcAlphaCombo.SelectedIndex = (int)materials[index].SourceAlpha;
			dstAlphaCombo.SelectedIndex = (int)materials[index].DestinationAlpha;

			// setting flags
			pickStatusCheck.Checked = materials[index].PickStatus;
			superSampleCheck.Checked = materials[index].SuperSample;
			clampUCheck.Checked = materials[index].ClampU;
			clampVCheck.Checked = materials[index].ClampV;
			flipUCheck.Checked = materials[index].FlipU;
			flipVCheck.Checked = materials[index].FlipV;
			useTextureCheck.Checked = materials[index].UseTexture;
			envMapCheck.Checked = materials[index].EnvironmentMap;
			doubleSideCheck.Checked = materials[index].DoubleSided;
			flatShadeCheck.Checked = materials[index].FlatShading;
			ignoreLightCheck.Checked = materials[index].IgnoreLighting;
			userFlagsNumeric.Value = materials[index].UserFlags;

			DisplayFlags(index);

			// Material list controls
			upButton.Enabled = materials.Count > 1 && index > 0;
			downButton.Enabled = materials.Count > 1 && index < materials.Count - 1;
			deleteButton.Enabled = materials.Count > 1;
		}

		private void RaiseFormUpdated()
		{
			comboMaterial.Items[comboMaterial.SelectedIndex] = (comboMaterial.SelectedIndex + ": " + GetMaterialDescription(materials[comboMaterial.SelectedIndex]));
			FormUpdated?.Invoke(this, EventArgs.Empty);
			DisplayFlags(comboMaterial.SelectedIndex);
		}

		private void DisplayFlags(int index)
		{
			labelFlags.Text = String.Format("{0:X8}", materials[index].Flags);
		}

		private void onKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Tab:
					if (e.Control)
					{
						int index = comboMaterial.SelectedIndex;
						int count = comboMaterial.Items.Count;

						if (e.Shift)
							--index;
						else
							++index;

						index = (index < 0) ? (index + count) : (index % count);
						comboMaterial.SelectedIndex = index;
					}
					break;
			}
		}

		private string GetMaterialDescription(NJS_MATERIAL material)
		{
			List<string> matdescs = new List<string>();
			System.Text.StringBuilder sb = new();
			matdescs.Add(material.UseTexture ? "Tex " + material.TextureID : "No Tex");
			if (material.UseAlpha)
			{
				if (material.DestinationAlpha == AlphaInstruction.One || material.SourceAlpha == AlphaInstruction.One)
					matdescs.Add("Alpha ONE");
				else
					matdescs.Add("Alpha " + material.DiffuseColor.A);
			}
			if (material.IgnoreLighting)
				matdescs.Add("IgnoreLight");
			if (material.EnvironmentMap)
				matdescs.Add("EnvMap");
			sb.AppendJoin(" / ", matdescs);
			return sb.ToString();
		}

		private void UpdateComboBox()
		{
			comboMaterial.Items.Clear();
			for (int i = 0; i < materials.Count; i++)
				comboMaterial.Items.Add(i + ": " + GetMaterialDescription(materials[i]));
			if (comboMaterial.Items.Count > 0)
				comboMaterial.SelectedIndex = 0;
		}
		#endregion

		#region General Control Event Methods

		private void comboMaterial_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboMaterial.SelectedIndex > -1)
				SetControls(comboMaterial.SelectedIndex);
		}

		private void textureBox_Click(object sender, EventArgs e)
		{
			if (textures == null)
			{
				MessageBox.Show("No textures loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			using (TexturePicker texPicker = new TexturePicker(textures, materials[comboMaterial.SelectedIndex].TextureID))
			{
				if (texPicker.ShowDialog(this) == DialogResult.OK)
				{
					materials[comboMaterial.SelectedIndex].TextureID = texPicker.SelectedValue;
					textureBox.Image = DrawPreviewImage(textures[materials[comboMaterial.SelectedIndex].TextureID].Image);

					RaiseFormUpdated();
				}
			}
		}

		private void diffuseColorBox_Click(object sender, EventArgs e)
		{
			colorDialog.Color = materials[comboMaterial.SelectedIndex].DiffuseColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				materials[comboMaterial.SelectedIndex].DiffuseColor = System.Drawing.Color.FromArgb((int)alphaDiffuseNumeric.Value, colorDialog.Color);
				diffuseColorBox.BackColor = materials[comboMaterial.SelectedIndex].DiffuseColor;
				RaiseFormUpdated();
			}
		}

		private void doneButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void alphaDiffuseNumeric_ValueChanged(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].DiffuseColor = System.Drawing.Color.FromArgb((int)alphaDiffuseNumeric.Value, materials[comboMaterial.SelectedIndex].DiffuseColor);
			diffuseColorBox.BackColor = materials[comboMaterial.SelectedIndex].DiffuseColor;
			RaiseFormUpdated();
		}

		#endregion

		#region Flag Check Event Methods

		private void pickStatusCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].PickStatus = pickStatusCheck.Checked;
			RaiseFormUpdated();
		}

		private void superSampleCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].SuperSample = superSampleCheck.Checked;
			RaiseFormUpdated();
		}

		private void clampUCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].ClampU = clampUCheck.Checked;
			RaiseFormUpdated();
		}

		private void clampVCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].ClampV = clampVCheck.Checked;
			RaiseFormUpdated();
		}

		private void flipUCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].FlipU = flipUCheck.Checked;
			RaiseFormUpdated();
		}

		private void flipVCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].FlipV = flipVCheck.Checked;
			RaiseFormUpdated();
		}

		private void useTextureCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].UseTexture = useTextureCheck.Checked;
			RaiseFormUpdated();
		}

		private void envMapCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].EnvironmentMap = envMapCheck.Checked;
			RaiseFormUpdated();
		}

		private void doubleSideCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].DoubleSided = doubleSideCheck.Checked;
			RaiseFormUpdated();
		}

		private void flatShadeCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].FlatShading = flatShadeCheck.Checked;
			RaiseFormUpdated();
		}

		private void ignoreLightCheck_Click(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].IgnoreLighting = ignoreLightCheck.Checked;
			RaiseFormUpdated();
		}

		private void userFlagsNumeric_ValueChanged(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].UserFlags = (byte)userFlagsNumeric.Value;
			RaiseFormUpdated();
		}

		private void filterModeDropDown_SelectionChangeCommitted(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].FilterMode = (FilterMode)filterModeDropDown.SelectedIndex;
			RaiseFormUpdated();
		}

		private void srcAlphaCombo_SelectionChangeCommitted(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].SourceAlpha = (AlphaInstruction)srcAlphaCombo.SelectedIndex;
			RaiseFormUpdated();
		}

		private void dstAlphaCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			materials[comboMaterial.SelectedIndex].DestinationAlpha = (AlphaInstruction)dstAlphaCombo.SelectedIndex;
			RaiseFormUpdated();
		}

		#endregion

		#region Material List Editing
		private void upButton_Click(object sender, EventArgs e)
		{
			NJS_MATERIAL selectedMaterial = materials[comboMaterial.SelectedIndex];
			int index = comboMaterial.SelectedIndex;
			materials.Insert(index - 1, selectedMaterial);
			materials.RemoveAt(index + 1);
			UpdateComboBox();
			RaiseFormUpdated();
		}

		private void downButton_Click(object sender, EventArgs e)
		{
			NJS_MATERIAL selectedMaterial = materials[comboMaterial.SelectedIndex];
			int index = comboMaterial.SelectedIndex;
			materials.Insert(index + 2, selectedMaterial);
			materials.RemoveAt(index);
			UpdateComboBox();
			comboMaterial.SelectedIndex = index + 1;
			RaiseFormUpdated();
		}

		private void cloneButton_Click(object sender, EventArgs e)
		{
			int index = comboMaterial.SelectedIndex;
			materials.Insert(comboMaterial.SelectedIndex, materials[comboMaterial.SelectedIndex].Clone());
			comboMaterial.SelectedIndex = 0;
			UpdateComboBox();
			comboMaterial.SelectedIndex = index + 1;
			RaiseFormUpdated();
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			int index = comboMaterial.SelectedIndex;
			materials.RemoveAt(comboMaterial.SelectedIndex);
			comboMaterial.SelectedIndex = 0;
			UpdateComboBox();
			comboMaterial.SelectedIndex = Math.Min(materials.Count - 1, Math.Max(0, index));
			RaiseFormUpdated();
		}

		private void resetButton_Click(object sender, EventArgs e)
		{
			materials.Clear();
			foreach (NJS_MATERIAL material in materialsOriginal)
			{
				materials.Add(material.Clone());
			}
			comboMaterial.SelectedIndex = 0;
			UpdateComboBox();
			RaiseFormUpdated();
		}
		#endregion
	}
}