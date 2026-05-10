using SAModel.GC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TextureLib;
using static TextureLib.DirectXTexUtility;

namespace SAModel.SAEditorCommon.UI
{
	public partial class GCModelParameterDataEditor : Form
	{
		#region Events

		public delegate void FormUpdatedHandler(object sender, EventArgs e);
		public event FormUpdatedHandler FormUpdated;

		#endregion

		public NJS_OBJECT editedHierarchy;

		private List<GCParameter> ParamData; // Poly data that is being edited
		private readonly List<GCParameter> ParamDataOriginal; // Original poly data at the time of opening the dialog
		private readonly GenericTexture[] textures;

		public GCModelParameterDataEditor(GCMesh meshData, GenericTexture[] textures, int index = 0)
		{
			if (meshData == null)
			{
				return;
			}
			InitializeComponent();
			ParamDataOriginal = meshData.Clone().Parameters;
			ParamData = meshData.Parameters;
			this.textures = textures;
			//comboBoxVertexGroup.Items.Clear();
			//comboBoxVertexGroup.SelectedIndex = index;
			BuildParameterDataList();
		}
		private void MaterialEditor_Load(object sender, EventArgs e)
		{
			SetControls(ParamData);
		}
		#region Texture Image
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
		#endregion
		#region Mesh management

		private void SetControls(List<GCParameter> gcp)
		{
			diffuseSettingBox.Enabled = false;
			groupBoxAmbient.Enabled = false;
			groupBoxSpecular.Enabled = false;
			groupBoxBlendMode.Enabled = false;
			groupBoxStripFlags.Enabled = false;
			groupBoxTexData.Enabled = false;
			groupBoxUVScale.Enabled = false;
			checkBoxIgnoreLight.Enabled = false;
			checkBoxIgnoreAmbient.Enabled = false;
			checkBoxIgnoreSpecular.Enabled = false;
			checkBoxVMaterial.Enabled = false;
			checkBoxVAmbient.Enabled = false;
			checkBoxUseAlpha.Enabled = false;
			checkBoxPunchthrough.Enabled = false;
			checkBoxDoubleSide.Enabled = false;
			checkBoxEnvMap.Enabled = false;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.VtxAttrFmt)
				{
					if ((GCVertexAttribute)(gp.Data >> 16) == GCVertexAttribute.Tex0)
					{
						groupBoxUVScale.Enabled = true;
						comboBoxUVScale.SelectedIndex = (byte)gp.Data;
					}
				}
				if (gp.Type == GC.ParameterType.StripFlags1)
				{
					groupBoxStripFlags.Enabled = true;
					checkBoxIgnoreLight.Enabled = true;
					checkBoxIgnoreAmbient.Enabled = true;
					checkBoxIgnoreSpecular.Enabled = true;
					checkBoxVMaterial.Enabled = true;
					checkBoxVAmbient.Enabled = true;
					checkBoxUseAlpha.Enabled = true;
					checkBoxPunchthrough.Enabled = true;
					checkBoxDoubleSide.Enabled = true;
					checkBoxIgnoreLight.Checked = ((GCStripFlags)(gp.Data >> 8)).HasFlag(GCStripFlags.IgnoreLight);
					checkBoxIgnoreAmbient.Checked = ((GCStripFlags)(gp.Data >> 8)).HasFlag(GCStripFlags.IgnoreAmbient);
					checkBoxIgnoreSpecular.Checked = ((GCStripFlags)(gp.Data >> 8)).HasFlag(GCStripFlags.IgnoreSpecular);
					checkBoxVMaterial.Checked = ((GCStripFlags)(gp.Data >> 8)).HasFlag(GCStripFlags.VertexDiffuse);
					checkBoxVAmbient.Checked = ((GCStripFlags)(gp.Data >> 8)).HasFlag(GCStripFlags.VertexAmbient);
					checkBoxUseAlpha.Checked = ((GCStripFlags)(gp.Data >> 8)).HasFlag(GCStripFlags.UseAlpha);
					checkBoxPunchthrough.Checked = ((GCStripFlags)(gp.Data >> 8)).HasFlag(GCStripFlags.NoPunchthrough);
					checkBoxDoubleSide.Checked = ((GCStripFlags)(gp.Data >> 8)).HasFlag(GCStripFlags.DoubleSided);
				}
				if (gp.Type == GC.ParameterType.DiffuseColor)
				{
					diffuseSettingBox.Enabled = true;
					alphaDiffuseNumeric.Value = (byte)gp.Data;
					diffuseRUpDown.Value = (byte)(gp.Data >> 8);
					diffuseGUpDown.Value = (byte)(gp.Data >> 16);
					diffuseBUpDown.Value = (byte)(gp.Data >> 24);
				}
				if (gp.Type == GC.ParameterType.AmbientColor)
				{
					groupBoxAmbient.Enabled = true;
					ambientRUpDown.Value = (byte)(gp.Data >> 8);
					ambientGUpDown.Value = (byte)(gp.Data >> 16);
					ambientBUpDown.Value = (byte)(gp.Data >> 24);
				}
				if (gp.Type == GC.ParameterType.SpecularColor)
				{
					groupBoxAmbient.Enabled = true;
					specularRUpDown.Value = (byte)(gp.Data >> 8);
					specularGUpDown.Value = (byte)(gp.Data >> 16);
					specularBUpDown.Value = (byte)(gp.Data >> 24);
				}
				if (gp.Type == GC.ParameterType.BlendAlpha)
				{
					groupBoxBlendMode.Enabled = true;
					srcAlphaCombo.SelectedIndex = (int)(gp.Data >> 11 & 7);
					dstAlphaCombo.SelectedIndex = (int)(gp.Data >> 8 & 7);
				}
				if (gp.Type == GC.ParameterType.Texture)
				{
					groupBoxTexData.Enabled = true;
					if (textures != null && (short)gp.Data < textures.Length) textureBox.Image = DrawPreviewImage(textures[(short)gp.Data].Image);
					numericUpDownTexID.Value = (short)gp.Data;
					checkBoxWrapU.Checked = ((GCTileMode)(gp.Data >> 16)).HasFlag(GCTileMode.WrapU);
					checkBoxWrapV.Checked = ((GCTileMode)(gp.Data >> 16)).HasFlag(GCTileMode.WrapV);
					checkBoxMirrorU.Checked = ((GCTileMode)(gp.Data >> 16)).HasFlag(GCTileMode.MirrorU);
					checkBoxMirrorV.Checked = ((GCTileMode)(gp.Data >> 16)).HasFlag(GCTileMode.MirrorV);
					checkBoxUnkTexMode.Checked = ((GCTileMode)(gp.Data >> 16)).HasFlag(GCTileMode.Unk_1);
				}
				if (gp.Type == GC.ParameterType.TexCoordGen)
				{
					groupBoxStripFlags.Enabled = true;
					checkBoxEnvMap.Enabled = true;
					checkBoxEnvMap.Checked = (((GCTexGenType)((gp.Data >> 12) & 0xF)) == GCTexGenType.Matrix3x4) && ((GCTexGenSrc)((gp.Data >> 4) & 0xFF) == GCTexGenSrc.Normal) && ((GCTexGenMatrix)(gp.Data & 0xF) == GCTexGenMatrix.Matrix4);
				}
			}
			RaiseFormUpdated();
		}

		void SetDiffuseFromNumerics()
		{
			diffuseColorBox.BackColor = System.Drawing.Color.FromArgb((int)alphaDiffuseNumeric.Value, (int)diffuseRUpDown.Value, (int)diffuseGUpDown.Value, (int)diffuseBUpDown.Value);
			RaiseFormUpdated();
		}
		void SetAmbientFromNumerics()
		{
			ambientColorBox.BackColor = System.Drawing.Color.FromArgb(255, (int)ambientRUpDown.Value, (int)ambientGUpDown.Value, (int)ambientBUpDown.Value);
			RaiseFormUpdated();
		}
		void SetSpecularFromNumerics()
		{
			specularColorBox.BackColor = System.Drawing.Color.FromArgb(255, (int)specularRUpDown.Value, (int)specularGUpDown.Value, (int)specularBUpDown.Value);
			RaiseFormUpdated();
		}
		private void RaiseFormUpdated()
		{
			FormUpdated?.Invoke(this, EventArgs.Empty);
			BuildParameterDataList();
		}
		#endregion

		#region UI
		private void BuildParameterDataList()
		{
			listViewParameters.Items.Clear();

			string datatype = "";
			string rawdata = "";
			groupBoxParamList.Enabled = true;
			if (ParamData != null)
			{
				for (int i = 0; i < ParamData.Count; i++)
				{
					GCParameter pdata = ParamData[i];
					ListViewItem newparam = new ListViewItem(i.ToString());
					switch (pdata.Type)
					{
						case GC.ParameterType.VtxAttrFmt:
							datatype = "VtxAttr";
							string vtxtype = "";
							string uvtype = "";
							switch ((GCVertexAttribute)(pdata.Data >> 16))
							{
								case GCVertexAttribute.PositionMatrixIdx:
									vtxtype = "PosMatrixIndex";
									break;
								case GCVertexAttribute.Position:
									vtxtype = "Point";
									break;
								case GCVertexAttribute.Color0:
									vtxtype = "Color0";
									break;
								case GCVertexAttribute.Normal:
									vtxtype = "Normal";
									break;
								case GCVertexAttribute.Tex0:
									vtxtype = "UV";
									switch ((GCUVScale)(byte)pdata.Data)
									{
										case GCUVScale.Default:
										case GCUVScale.Scale1:
											uvtype = ", Normal Scale";
											break;
										case GCUVScale.NoUV1:
										case GCUVScale.NoUV2:
										case GCUVScale.NoUV3:
										case GCUVScale.NoUV4:
										case GCUVScale.NoUV5:
										case GCUVScale.NoUV6:
										case GCUVScale.NoUV7:
										case GCUVScale.Scale9:
											uvtype = ", No UV";
											break;
										case GCUVScale.Scale2:
											uvtype = ", 1/2 Scale";
											break;
										case GCUVScale.Scale3:
											uvtype = ", 1/4 Scale";
											break;
										case GCUVScale.Scale4:
											uvtype = ", 1/8 Scale";
											break;
										case GCUVScale.Scale5:
											uvtype = ", 1/16 Scale";
											break;
										case GCUVScale.Scale6:
											uvtype = ", 1/32 Scale";
											break;
										case GCUVScale.Scale7:
											uvtype = ", 1/64 Scale";
											break;
										case GCUVScale.Scale8:
											uvtype = ", 1/128 Scale";
											break;
									}
									break;
							}
							rawdata = vtxtype + uvtype;
							break;
						case GC.ParameterType.IndexAttributeFlags:
							datatype = "AttrFlags";
							rawdata = $"{((GCIndexAttributeFlags)pdata.Data).ToString().Replace(", ", " | ")}";
							break;
						case GC.ParameterType.StripFlags1:
							datatype = "StripData";
							string flags = string.Empty;

							if (((GCStripFlags)(pdata.Data >> 8)).HasFlag(GCStripFlags.IgnoreLight))
								flags += "IL | ";
							if (((GCStripFlags)(pdata.Data >> 8)).HasFlag(GCStripFlags.IgnoreSpecular))
								flags += "IS | ";
							if (((GCStripFlags)(pdata.Data >> 8)).HasFlag(GCStripFlags.IgnoreAmbient))
								flags += "IA | ";
							if (((GCStripFlags)(pdata.Data >> 8)).HasFlag(GCStripFlags.VertexDiffuse))
								flags += "VM | ";
							if (((GCStripFlags)(pdata.Data >> 8)).HasFlag(GCStripFlags.VertexAmbient))
								flags += "VA | ";
							if (((GCStripFlags)(pdata.Data >> 8)).HasFlag(GCStripFlags.UseAlpha))
								flags += "UA | ";
							if (((GCStripFlags)(pdata.Data >> 8)).HasFlag(GCStripFlags.NoPunchthrough))
								flags += "NPT | ";
							if (((GCStripFlags)(pdata.Data >> 8)).HasFlag(GCStripFlags.DoubleSided))
								flags += "DB | ";
							if (flags == string.Empty)
								flags = "0x0";
							else
								flags = flags.Remove(flags.Length - 3);
							rawdata = $"ChannelNum({(byte)(pdata.Data & 0x3)}), TexGenCount({(byte)((pdata.Data >> 4) & 0xF)}), {flags}, ShadowStencil({(byte)((pdata.Data >> 16) & 0xF)})";
							break;
						case GC.ParameterType.BlendAlpha:
							datatype = "BlendMode";
							rawdata = $"{(GCBlendModeControl)((pdata.Data >> 11) & 7)}, {(GCBlendModeControl)((pdata.Data >> 8) & 7)}";
							break;
						case GC.ParameterType.DiffuseColor:
							datatype = "Diffuse";
							rawdata = $"A{(byte)pdata.Data}, R{(byte)(pdata.Data >> 8)}, G{(byte)(pdata.Data >> 16)}, B{(byte)(pdata.Data >> 24)}";
							break;
						case GC.ParameterType.Texture:
							datatype = "Texture";
							rawdata = $"TID {(short)pdata.Data}, {((GCTileMode)(short)(pdata.Data >> 16)).ToString().Replace(", ", " | ")}";
							break;
						case GC.ParameterType.TextureTEVMode:
							datatype = "TEVMode";
							rawdata = $"{(short)pdata.Data}, {(short)(pdata.Data >> 16)}";
							break;
						case GC.ParameterType.TexCoordGen:
							datatype = "TexCoordGen";
							rawdata = $"{(GCTexCoordID)((pdata.Data >> 16) & 0xFF)}, {(GCTexGenType)((pdata.Data >> 12) & 0xF)}, {(GCTexGenSrc)((pdata.Data >> 4) & 0xFF)}, {(GCTexGenMatrix)(pdata.Data & 0xF)}";
							break;
					}
					newparam.SubItems.Add(datatype);
					newparam.SubItems.Add(rawdata);
					listViewParameters.Items.Add(newparam);
				}
			}
			listViewParameters.SelectedIndices.Clear();
			listViewParameters.SelectedItems.Clear();
			listViewParameters_SelectedIndexChanged(null, null);
			listViewParameters.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		private void SetTextureFlags()
		{
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.Texture)
				{
					uint tex = gp.Data;
					uint tiles = 0;
					if (checkBoxMirrorU.Checked)
					{
						tiles |= 1 << 3;
					}
					if (checkBoxMirrorV.Checked)
					{
						tiles |= 1 << 1;
					}
					if (checkBoxWrapU.Checked)
					{
						tiles |= 1 << 2;
					}
					if (checkBoxWrapV.Checked)
					{
						tiles |= 1 << 0;
					}
					if (checkBoxUnkTexMode.Checked)
					{
						tiles |= 1 << 4;
					}
					tex &= 0xFFFF0000;
					tex |= (ushort)numericUpDownTexID.Value;
					tex &= 0x0000FFFF;
					tex |= (uint)tiles << 16;
					gp.Data = tex;
				}
			}
		}
		private void SetStripFlags()
		{
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.StripFlags1)
				{
					uint strips = gp.Data;
					uint flags = 0;
					if (checkBoxIgnoreLight.Checked)
						flags |= 0x1;
					if (checkBoxIgnoreSpecular.Checked)
						flags |= 0x2;
					if (checkBoxIgnoreAmbient.Checked)
						flags |= 0x4;
					if (checkBoxVMaterial.Checked)
						flags |= 0x8;
					if (checkBoxVAmbient.Checked)
						flags |= 0x10;
					if (checkBoxUseAlpha.Checked)
						flags |= 0x20;
					if (checkBoxPunchthrough.Checked)
						flags |= 0x40;
					if (checkBoxDoubleSide.Checked)
						flags |= 0x80;
					strips &= 0xFFFF00FF;
					strips |= (uint)flags << 8;
					gp.Data = strips;
				}
			}
		}


		private void checkSettingsOnClose()
		{
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.VtxAttrFmt)
				{
					if ((GCVertexAttribute)(gp.Data >> 16) == GCVertexAttribute.Tex0)
					{
						gp.Data &= 0xFFFFFF00;
						gp.Data |= (byte)comboBoxUVScale.SelectedIndex;
					}
				}
				if (gp.Type == GC.ParameterType.StripFlags1)
				{
					uint strips = gp.Data;
					uint flags = 0;
					if (checkBoxIgnoreLight.Checked)
						flags |= 0x1;
					if (checkBoxIgnoreSpecular.Checked)
						flags |= 0x2;
					if (checkBoxIgnoreAmbient.Checked)
						flags |= 0x4;
					if (checkBoxVMaterial.Checked)
						flags |= 0x8;
					if (checkBoxVAmbient.Checked)
						flags |= 0x10;
					if (checkBoxUseAlpha.Checked)
						flags |= 0x20;
					if (checkBoxPunchthrough.Checked)
						flags |= 0x40;
					if (checkBoxDoubleSide.Checked)
						flags |= 0x80;
					strips &= 0xFFFF00FF;
					strips |= (uint)flags << 8;
				}
				if (gp.Type == GC.ParameterType.BlendAlpha)
				{
					uint blend = gp.Data;
					blend &= 0xFFFFC7FF; // ~(7 << 11)
					blend |= ((uint)srcAlphaCombo.SelectedIndex & 7) << 11;
					blend &= 0xFFFFF8FF; // ~(7 << 8)
					blend |= ((uint)dstAlphaCombo.SelectedIndex & 7) << 8;
					gp.Data = blend;
				}
				if (gp.Type == GC.ParameterType.DiffuseColor)
				{
					uint amb = gp.Data;
					amb &= 0xFFFFFF00;
					amb |= (uint)alphaDiffuseNumeric.Value;
					amb &= 0xFFFF00FF;
					amb |= (uint)diffuseRUpDown.Value << 8;
					amb &= 0xFF00FFFF;
					amb |= (uint)diffuseGUpDown.Value << 16;
					amb &= 0x00FFFFFF;
					amb |= (uint)diffuseBUpDown.Value << 24;
					gp.Data = amb;
				}
				if (gp.Type == GC.ParameterType.AmbientColor)
				{
					uint amb = gp.Data;
					amb &= 0xFFFF00FF;
					amb |= (uint)ambientRUpDown.Value << 8;
					amb &= 0xFF00FFFF;
					amb |= (uint)ambientGUpDown.Value << 16;
					amb &= 0x00FFFFFF;
					amb |= (uint)ambientBUpDown.Value << 24;
					gp.Data = amb;
				}
				if (gp.Type == GC.ParameterType.SpecularColor)
				{
					uint amb = gp.Data;
					amb &= 0xFFFF00FF;
					amb |= (uint)specularRUpDown.Value << 8;
					amb &= 0xFF00FFFF;
					amb |= (uint)specularGUpDown.Value << 16;
					amb &= 0x00FFFFFF;
					amb |= (uint)specularBUpDown.Value << 24;
					gp.Data = amb;
				}
				if (gp.Type == GC.ParameterType.Texture)
				{
					uint tex = gp.Data;
					uint tiles = 0;
					if (checkBoxMirrorU.Checked)
					{
						tiles |= 1 << 3;
					}
					if (checkBoxMirrorV.Checked)
					{
						tiles |= 1 << 1;
					}
					if (checkBoxWrapU.Checked)
					{
						tiles |= 1 << 2;
					}
					if (checkBoxWrapV.Checked)
					{
						tiles |= 1 << 0;
					}
					if (checkBoxUnkTexMode.Checked)
					{
						tiles |= 1 << 4;
					}
					tex &= 0x0000FFFF;
					tex |= (uint)tiles << 16;
					gp.Data = tex;
				}
				if (gp.Type == GC.ParameterType.TexCoordGen)
				{
					uint edata = gp.Data;
					GCTexCoordID tgi = (GCTexCoordID)((edata >> 16) & 0xFF);
					GCTexGenType tgt = (GCTexGenType)((edata >> 12) & 0xF);
					GCTexGenSrc tgs = (GCTexGenSrc)((edata >> 4) & 0xFF);
					GCTexGenMatrix tgm = (GCTexGenMatrix)(edata & 0xF);
					//Environment mapping checks for these types
					if (checkBoxEnvMap.Checked)
					{
						tgt = GCTexGenType.Matrix3x4;
						tgs = GCTexGenSrc.Normal;
						tgm = GCTexGenMatrix.Matrix4;
					}
					else
					{
						tgt = GCTexGenType.Matrix2x4;
						tgs = GCTexGenSrc.Tex0;
						tgm = GCTexGenMatrix.Identity;
					}
					edata &= 0xFF00FFFF;
					edata |= (uint)tgi << 16;
					edata &= 0xFFFF0FFF;
					edata |= (uint)tgt << 12;
					edata &= 0xFFFFF00F;
					edata |= (uint)tgs << 4;
					edata &= 0xFFFFFFF0;
					edata |= (uint)tgm;
					gp.Data = edata;
				}
			}
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			checkSettingsOnClose();
			Close();
		}
		#endregion

		private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{

		}

		private void listViewParameters_SelectedIndexChanged(object sender, EventArgs e)
		{
			//contextMenuStripVertCol.Enabled;
			//GCParameter selectedParam = ParamData[listViewParameters.SelectedIndices[0]];
			if (listViewParameters.SelectedIndices.Count == 0)
			{
				buttonResetParameter.Enabled = false;
				return;
			}
			string ptype = "";
			string pdata = "";
			buttonResetParameter.Enabled = true;
			GCParameter selectedParam = ParamData[listViewParameters.SelectedIndices[0]];
			ptype = selectedParam.Type.ToString();
			pdata = listViewParameters.SelectedItems[0].SubItems[2].Text;

			StringBuilder sb = new StringBuilder();
			sb.Append("Attributes: ");
			sb.Append(ptype);
			sb.Append(", Data: ");
			sb.Append(pdata);
			toolStripStatusLabelInfo.Text = sb.ToString();
		}

		private void groupBox2_Enter(object sender, EventArgs e)
		{

		}

		private void diffuseRUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetDiffuseFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.DiffuseColor)
				{
					gp.Data &= 0xFFFF00FF;
					gp.Data |= (uint)diffuseRUpDown.Value << 8;
				}
			}
		}

		private void diffuseGUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetDiffuseFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.DiffuseColor)
				{
					gp.Data &= 0xFF00FFFF;
					gp.Data |= (uint)diffuseGUpDown.Value << 16;
				}
			}
		}

		private void diffuseBUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetDiffuseFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.DiffuseColor)
				{
					gp.Data &= 0x00FFFFFF;
					gp.Data |= (uint)diffuseBUpDown.Value << 24;
				}
			}
		}

		private void alphaDiffuseNumeric_ValueChanged(object sender, EventArgs e)
		{
			SetDiffuseFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.DiffuseColor)
				{
					gp.Data &= 0xFFFFFF00;
					gp.Data |= (uint)alphaDiffuseNumeric.Value;
				}
			}
		}

		private void diffuseColorBox_Click(object sender, EventArgs e)
		{
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.DiffuseColor)
				{
					uint edata = gp.Data;
					byte Alpha = (byte)(edata & 0xFF);
					byte Red = (byte)((edata >> 8) & 0xFF);
					byte Green = (byte)((edata >> 16) & 0xFF);
					byte Blue = (byte)(edata >> 24);
					colorDialog.Color = System.Drawing.Color.FromArgb(Alpha, Red, Green, Blue);
					if (colorDialog.ShowDialog() == DialogResult.OK)
					{
						Red = colorDialog.Color.R;
						Green = colorDialog.Color.G;
						Blue = colorDialog.Color.B;
						colorDialog.Color = System.Drawing.Color.FromArgb(Alpha, Red, Green, Blue);
						diffuseRUpDown.Value = Red;
						diffuseGUpDown.Value = Green;
						diffuseBUpDown.Value = Blue;
					}
				}
			}
			RaiseFormUpdated();
		}

		private void checkBoxWrapU_Click(object sender, EventArgs e)
		{
			SetTextureFlags();
			RaiseFormUpdated();
		}

		private void checkBoxWrapV_Click(object sender, EventArgs e)
		{
			SetTextureFlags();
			RaiseFormUpdated();
		}

		private void checkBoxMirrorU_Click(object sender, EventArgs e)
		{
			SetTextureFlags();
			RaiseFormUpdated();
		}

		private void checkBoxMirrorV_Click(object sender, EventArgs e)
		{
			SetTextureFlags();
			RaiseFormUpdated();
		}

		private void checkBoxUnkTexMode_Click(object sender, EventArgs e)
		{
			SetTextureFlags();
			RaiseFormUpdated();
		}

		private void numericUpDownTexID_ValueChanged(object sender, EventArgs e)
		{
			if (textures != null && numericUpDownTexID.Value < textures.Length) textureBox.Image = DrawPreviewImage(textures[(short)numericUpDownTexID.Value].Image);
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.Texture)
				{
					gp.Data &= 0xFFFF0000;
					gp.Data |= (ushort)numericUpDownTexID.Value;
				}
			}
			RaiseFormUpdated();
		}

		private void checkBoxEnvMap_Click(object sender, EventArgs e)
		{
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.TexCoordGen)
				{
					uint edata = gp.Data;
					GCTexCoordID tgi = (GCTexCoordID)((edata >> 16) & 0xFF);
					GCTexGenType tgt = (GCTexGenType)((edata >> 12) & 0xF);
					GCTexGenSrc tgs = (GCTexGenSrc)((edata >> 4) & 0xFF);
					GCTexGenMatrix tgm = (GCTexGenMatrix)(edata & 0xF);
					//Environment mapping checks for these types
					if (checkBoxEnvMap.Checked)
					{
						tgt = GCTexGenType.Matrix3x4;
						tgs = GCTexGenSrc.Normal;
						tgm = GCTexGenMatrix.Matrix4;
					}
					else
					{
						tgt = GCTexGenType.Matrix2x4;
						tgs = GCTexGenSrc.Tex0;
						tgm = GCTexGenMatrix.Identity;
					}
					gp.Data &= 0xFF00FFFF;
					gp.Data |= (uint)tgi << 16;
					gp.Data &= 0xFFFF0FFF;
					gp.Data |= (uint)tgt << 12;
					gp.Data &= 0xFFFFF00F;
					gp.Data |= (uint)tgs << 4;
					gp.Data &= 0xFFFFFFF0;
					gp.Data |= (uint)tgm;
				}
			}
			RaiseFormUpdated();
		}

		private void checkBoxIgnoreLight_Click(object sender, EventArgs e)
		{
			SetStripFlags();
			RaiseFormUpdated();
		}
		private void checkBoxIgnoreAmbient_Click(object sender, EventArgs e)
		{
			SetStripFlags();
			RaiseFormUpdated();
		}
		private void checkBoxIgnoreSpecular_Click(object sender, EventArgs e)
		{
			SetStripFlags();
			RaiseFormUpdated();
		}
		private void checkBoxVMaterial_Click(object sender, EventArgs e)
		{
			SetStripFlags();
			RaiseFormUpdated();
		}
		private void checkBoxVAmbient_Click(object sender, EventArgs e)
		{
			SetStripFlags();
			RaiseFormUpdated();
		}
		private void checkBoxUseAlpha_Click(object sender, EventArgs e)
		{
			SetStripFlags();
			RaiseFormUpdated();
		}
		private void checkBoxPunchthrough_Click(object sender, EventArgs e)
		{
			SetStripFlags();
			RaiseFormUpdated();
		}
		private void checkBoxDoubleSide_Click(object sender, EventArgs e)
		{
			SetStripFlags();
			RaiseFormUpdated();
		}


		private void textureBox_Click(object sender, EventArgs e)
		{
			if (textures == null)
			{
				MessageBox.Show("No textures loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			using (TexturePicker texPicker = new TexturePicker(textures, (short)numericUpDownTexID.Value))
			{
				if (texPicker.ShowDialog(this) == DialogResult.OK)
				{
					textureBox.Image = DrawPreviewImage(textures[(ushort)numericUpDownTexID.Value].Image);
					numericUpDownTexID.Value = texPicker.SelectedValue;
					RaiseFormUpdated();
				}
			}
		}

		private void buttonResetParameter_Click(object sender, EventArgs e)
		{
			List<GCParameter> originalParams = new List<GCParameter>(ParamDataOriginal.Count);
			foreach (GCParameter param in ParamDataOriginal)
			{
				originalParams.Add(param.Clone());
			}
			ParamData[listViewParameters.SelectedIndices[0]] = originalParams[listViewParameters.SelectedIndices[0]];
			SetControls(ParamData);
			BuildParameterDataList();
		}

		private void buttonResetAll_Click(object sender, EventArgs e)
		{
			List<GCParameter> originalParams = new List<GCParameter>(ParamDataOriginal.Count);
			foreach (GCParameter param in ParamDataOriginal)
			{
				originalParams.Add(param.Clone());
			}
			ParamData.Clear();
			foreach (GCParameter paramO in originalParams)
			{
				ParamData.Add(paramO.Clone());
			}
			SetControls(ParamData);
			BuildParameterDataList();
		}

		private void srcAlphaCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.BlendAlpha)
				{
					gp.Data &= 0xFFFFC7FF; // ~(7 << 11)
					gp.Data |= ((uint)srcAlphaCombo.SelectedIndex & 7) << 11;
				}
			}
			RaiseFormUpdated();
		}

		private void dstAlphaCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.BlendAlpha)
				{
					gp.Data &= 0xFFFFF8FF; // ~(7 << 8)
					gp.Data |= ((uint)dstAlphaCombo.SelectedIndex & 7) << 8;
				}
			}
			RaiseFormUpdated();
		}

		private void comboBoxUVScale_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.VtxAttrFmt)
				{
					if ((GCVertexAttribute)(gp.Data >> 16) == GCVertexAttribute.Tex0)
					{
						gp.Data &= 0xFFFFFF00;
						gp.Data |= (byte)comboBoxUVScale.SelectedIndex;
					}
				}
			}
			RaiseFormUpdated();
		}

		private void ambientRUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetAmbientFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.AmbientColor)
				{
					gp.Data &= 0xFFFF00FF;
					gp.Data |= (uint)ambientRUpDown.Value << 8;
				}
			}
		}

		private void ambientGUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetAmbientFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.AmbientColor)
				{
					gp.Data &= 0xFF00FFFF;
					gp.Data |= (uint)ambientGUpDown.Value << 16;
				}
			}
		}

		private void ambientBUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetAmbientFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.AmbientColor)
				{
					gp.Data &= 0x00FFFFFF;
					gp.Data |= (uint)ambientBUpDown.Value << 24;
				}
			}
		}

		private void specularRUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetSpecularFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.SpecularColor)
				{
					gp.Data &= 0xFFFF00FF;
					gp.Data |= (uint)specularRUpDown.Value << 8;
				}
			}
		}

		private void specularGUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetSpecularFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.SpecularColor)
				{
					gp.Data &= 0xFF00FFFF;
					gp.Data |= (uint)specularGUpDown.Value << 16;
				}
			}
		}

		private void specularBUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetSpecularFromNumerics();
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.SpecularColor)
				{
					gp.Data &= 0x00FFFFFF;
					gp.Data |= (uint)specularBUpDown.Value << 24;
				}
			}
		}

		private void ambientColorBox_Click(object sender, EventArgs e)
		{
			List<GCParameter> gcp = ParamData;
			for (int i = 0; i < gcp.Count; i++)
			{
				GCParameter gp = gcp[i];
				if (gp.Type == GC.ParameterType.AmbientColor)
				{
					uint edata = gp.Data;
					byte Red = (byte)((edata >> 8) & 0xFF);
					byte Green = (byte)((edata >> 16) & 0xFF);
					byte Blue = (byte)(edata >> 24);
					colorDialog.Color = System.Drawing.Color.FromArgb(255, Red, Green, Blue);
					if (colorDialog.ShowDialog() == DialogResult.OK)
					{
						Red = colorDialog.Color.R;
						Green = colorDialog.Color.G;
						Blue = colorDialog.Color.B;
						colorDialog.Color = System.Drawing.Color.FromArgb(255, Red, Green, Blue);
						ambientRUpDown.Value = Red;
						ambientGUpDown.Value = Green;
						ambientBUpDown.Value = Blue;
					}
				}
			}
			RaiseFormUpdated();
		}

		private void specularColorBox_Click(object sender, EventArgs e)
		{
			{
				List<GCParameter> gcp = ParamData;
				for (int i = 0; i < gcp.Count; i++)
				{
					GCParameter gp = gcp[i];
					if (gp.Type == GC.ParameterType.SpecularColor)
					{
						uint edata = gp.Data;
						byte Red = (byte)((edata >> 8) & 0xFF);
						byte Green = (byte)((edata >> 16) & 0xFF);
						byte Blue = (byte)(edata >> 24);
						colorDialog.Color = System.Drawing.Color.FromArgb(255, Red, Green, Blue);
						if (colorDialog.ShowDialog() == DialogResult.OK)
						{
							Red = colorDialog.Color.R;
							Green = colorDialog.Color.G;
							Blue = colorDialog.Color.B;
							colorDialog.Color = System.Drawing.Color.FromArgb(255, Red, Green, Blue);
							specularRUpDown.Value = Red;
							specularGUpDown.Value = Green;
							specularBUpDown.Value = Blue;
						}
					}
				}
				RaiseFormUpdated();
			}
		}
	}
}