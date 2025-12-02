using System;
using System.Drawing;
using System.Windows.Forms;
using SAModel.Direct3D.TextureSystem;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ChunkModelTextureDataEditor : Form
	{
		#region Events

		public delegate void FormUpdatedHandler(object sender, EventArgs e);
		public event FormUpdatedHandler FormUpdated;

		#endregion

		private PolyChunk PolyData; // Poly data that is being edited
		private readonly PolyChunk PolyDataOriginal; // Original poly data at the time of opening the dialog
		private readonly BMPInfo[] textures;

		public ChunkModelTextureDataEditor(PolyChunk mats, BMPInfo[] textures, string matsName = null)
		{
			PolyData = mats;
			PolyDataOriginal = mats.Clone();
			this.textures = textures;
			InitializeComponent();
			if (!string.IsNullOrEmpty(matsName))
				this.Text = "Texture Data Editor: " + matsName;
		}

		private void MaterialEditor_Load(object sender, EventArgs e)
		{
			//PolyData = new PolyChunkTinyTextureID();
			SetControls(PolyData.Clone());
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
		private void SetControls(PolyChunk poly)
		{
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)poly;
			// Setting general
			if (textures != null && pct.TextureID < textures.Length) textureBox.Image = DrawPreviewImage(textures[pct.TextureID].Image);
			filterModeDropDown.SelectedIndex = (int)pct.FilterMode;
			mipmapDropDown.SelectedIndex = pct.Flags & 0xF;
			textureIDNumeric.Value = pct.TextureID;
			superSampleCheck.Checked = pct.SuperSample;
			clampUCheck.Checked = pct.ClampU;
			clampVCheck.Checked = pct.ClampV;
			flipUCheck.Checked = pct.FlipU;
			flipVCheck.Checked = pct.FlipV;
			//DisplayFlags(index);

			// Material list controls
			resetButton.Enabled = true;
			RaiseFormUpdated();
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

		private void textureBox_Click(object sender, EventArgs e)
		{
			if (textures == null)
			{
				MessageBox.Show("No textures loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (PolyData is PolyChunkTinyTextureID)
			{
				PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
				if (pct.TextureID < 0)
				{
					MessageBox.Show("This piece does not use a unique texture ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				using (TexturePicker texPicker = new TexturePicker(textures, pct.TextureID))
				{
					if (texPicker.ShowDialog(this) == DialogResult.OK)
					{
						pct.TextureID = (ushort)texPicker.SelectedValue;
						textureBox.Image = DrawPreviewImage(textures[(ushort)textureIDNumeric.Value].Image);

						RaiseFormUpdated();
					}
				}
			}
		}

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
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
			pct.FilterMode = (FilterMode)filterModeDropDown.SelectedIndex;
			pct.MipmapDAdjust = mipmapDropDown.SelectedIndex * 0.25f;
			pct.SuperSample = superSampleCheck.Checked;
			pct.ClampU = clampUCheck.Checked;
			pct.ClampV = clampVCheck.Checked;
			pct.FlipU = flipUCheck.Checked;
			pct.FlipV = flipVCheck.Checked;
			pct.TextureID = (ushort)textureIDNumeric.Value;
		}
		private void superSampleCheck_Click(object sender, EventArgs e)
		{
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
			pct.SuperSample = superSampleCheck.Checked;
			RaiseFormUpdated();
		}

		private void clampUCheck_Click(object sender, EventArgs e)
		{
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
			pct.ClampU = clampUCheck.Checked;
			RaiseFormUpdated();
		}

		private void clampVCheck_Click(object sender, EventArgs e)
		{
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
			pct.ClampV = clampVCheck.Checked;
			RaiseFormUpdated();
		}

		private void flipUCheck_Click(object sender, EventArgs e)
		{
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
			pct.FlipU = flipUCheck.Checked;
			RaiseFormUpdated();
		}

		private void flipVCheck_Click(object sender, EventArgs e)
		{
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
			pct.FlipV = flipVCheck.Checked;
			RaiseFormUpdated();
		}

		private void userFlagsNumeric_ValueChanged(object sender, EventArgs e)
		{
			//materials[comboMaterial.SelectedIndex].UserFlags = (byte)userFlagsNumeric.Value;
			RaiseFormUpdated();
		}

		private void filterModeDropDown_SelectionChangeCommitted(object sender, EventArgs e)
		{
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
			pct.FilterMode = (FilterMode)filterModeDropDown.SelectedIndex;
			RaiseFormUpdated();
		}

		#endregion
		#region Material List Editing

		private void resetButton_Click(object sender, EventArgs e)
		{
			SetControls(PolyDataOriginal.Clone());
		}
		#endregion

		private void generalSettingBox_Enter(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void textureIDNumeric_ValueChanged(object sender, EventArgs e)
		{
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
			pct.TextureID = (ushort)textureIDNumeric.Value;
			if (textures != null && pct.TextureID < textures.Length) textureBox.Image = DrawPreviewImage(textures[pct.TextureID].Image);
			RaiseFormUpdated();
		}

		private void mipmapDropDown_SelectionChangeCommitted(object sender, EventArgs e)
		{
			PolyChunkTinyTextureID pct = (PolyChunkTinyTextureID)PolyData;
			pct.MipmapDAdjust = mipmapDropDown.SelectedIndex * 0.25f;
			RaiseFormUpdated();
		}
	}
}