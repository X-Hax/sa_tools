using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System;
using System.DirectoryServices.ActiveDirectory;
using SAModel.Direct3D.TextureSystem;
using SAModel.GC;
using static VrSharp.Xvr.DirectXTexUtility;
using SharpDX.Direct3D9;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ChunkModelDataEditor : Form
	{
		public NJS_OBJECT editedHierarchy;

		private Attach editedModel;
		private Attach originalModel;
		private bool freeze;
		private int previousNodeIndex;
		private readonly BMPInfo[] textures;

		public ChunkModelDataEditor(NJS_OBJECT objectOriginal, BMPInfo[] textures, int index = 0)
		{
			if (objectOriginal == null)
				return;
			InitializeComponent();
			freeze = true;
			comboBoxNode.Items.Clear();
			editedHierarchy = objectOriginal.Clone();
			editedHierarchy.FixParents();
			editedHierarchy.FixSiblings();
			BuildNodeList();
			comboBoxNode.SelectedIndex = index;
			BuildVertexDataList();
			BuildPolyChunkList();
			this.textures = textures;
			freeze = false;
		}

		#region Vertex management

		#endregion
		#region Mesh management
		private void updateStripData(List<PolyChunk> modelchunks, int chunkID, PolyChunkStrip pcs)
		{
			modelchunks[chunkID] = pcs;
		}
		private void updateTextureData(List<PolyChunk> modelchunks, int chunkID, PolyChunkTinyTextureID ttid)
		{
			modelchunks[chunkID] = ttid;
		}
		private void updateMaterialData(List<PolyChunk> modelchunks, int chunkID, PolyChunkMaterial mat)
		{
			modelchunks[chunkID] = mat;
		}
		private void editDiffuseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string polydata = listViewMeshes.SelectedItems[0].SubItems[1].Text;
			int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			PolyChunkMaterial mat;
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			if (polydata.StartsWith("Material"))
			{
				using (ChunkModelMaterialDataEditor de = new ChunkModelMaterialDataEditor(selectedObj[matID]))
				{
					mat = (PolyChunkMaterial)selectedObj[matID];
					de.FormUpdated += (s, ev) => updateMaterialData(selectedObj, matID, mat);
					de.ShowDialog(this);
				}
			}
			BuildPolyChunkList();
		}
		private void editTextureIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string polydata = listViewMeshes.SelectedItems[0].SubItems[1].Text;
			int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			PolyChunkTinyTextureID ttid;
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			if (polydata.StartsWith("Tiny"))
			{
				using (ChunkModelTextureDataEditor de = new ChunkModelTextureDataEditor(selectedObj[matID], textures))
				{
					ttid = (PolyChunkTinyTextureID)selectedObj[matID];
					de.FormUpdated += (s, ev) => updateTextureData(selectedObj, matID, ttid);
					de.ShowDialog(this);
				}
			}
			BuildPolyChunkList();
		}

		private void editStripAlphaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string polytype = listViewMeshes.SelectedItems[0].SubItems[1].Text;
			int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			PolyChunkStrip pcs;
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			if (polytype.StartsWith("Strip"))
			{
				using (ChunkModelStripDataEditor de = new ChunkModelStripDataEditor(selectedObj[matID]))
				{
					pcs = (PolyChunkStrip)selectedObj[matID];
					de.FormUpdated += (s, ev) => updateStripData(selectedObj, matID, pcs);
					de.ShowDialog(this);
				}
			}
			BuildPolyChunkList();
		}

		private void showVertexCollectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int vertID = int.Parse(listViewVertices.SelectedItems[0].SubItems[0].Text);
			List<VertexChunk> selectedObj = ((ChunkAttach)editedModel).Vertex;
			VertexChunk selectedVert = selectedObj[listViewVertices.SelectedIndices[0]];
			ChunkModelVertexDataEditor vde = new ChunkModelVertexDataEditor(selectedVert);
			if (vde.ShowDialog(this) == DialogResult.OK)
			{
				return;
			}
		}

		private void buttonResetMeshes_Click(object sender, System.EventArgs e)
		{
			((ChunkAttach)editedModel).Poly.Clear();
			foreach (PolyChunk mesh in ((ChunkAttach)originalModel).Poly)
				((ChunkAttach)editedModel).Poly.Add(mesh);
			BuildPolyChunkList();
		}

		private void buttonCloneMesh_Click(object sender, System.EventArgs e)
		{
			int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			selectedMeshes.Add(selectedObj[matID]);
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			foreach (PolyChunk mesh in selectedMeshes)
				selectedObj.Insert(matID + 1, mesh.Clone());
			BuildPolyChunkList();
			SelectMesh(Math.Min(listViewMeshes.Items.Count - 1, index + 1));
		}

		private void buttonDeleteMesh_Click(object sender, System.EventArgs e)
		{
			int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			selectedMeshes.Add(selectedObj[matID]);
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			foreach (PolyChunk mesh in selectedMeshes)
				selectedObj.Remove(mesh);
			BuildPolyChunkList();
			SelectMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveMeshUp_Click(object sender, System.EventArgs e)
		{
			int prevIndex = listViewMeshes.SelectedItems[0].Index - 1;
			int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			int pmatID = int.Parse(listViewMeshes.Items[prevIndex].SubItems[0].Text);
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			selectedMeshes.Add(selectedObj[matID]);
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			foreach (PolyChunk mesh in selectedMeshes)
				selectedObj.Insert(pmatID, mesh);
			foreach (PolyChunk mesh in selectedMeshes)
				selectedObj.RemoveAt(pmatID + 2);
			BuildPolyChunkList();
			SelectMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveMeshDown_Click(object sender, System.EventArgs e)
		{
			int nextIndex = listViewMeshes.SelectedItems[0].Index + 1;
			int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			int nmatID = int.Parse(listViewMeshes.Items[nextIndex].SubItems[0].Text);
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			selectedMeshes.Add(selectedObj[matID]);
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			foreach (PolyChunk mesh in selectedMeshes)
			{
				selectedObj.Insert(nmatID + 1, mesh);
			}
			foreach (PolyChunk mesh in selectedMeshes)
			{
				selectedObj.RemoveAt(matID);
			}
			BuildPolyChunkList();
			SelectMesh(Math.Min(listViewMeshes.Items.Count - 1, index + 1));
		}

		private void SelectMesh(int index)
		{
			listViewMeshes.SelectedIndices.Add(index);
		}
		#endregion
		#region Editing model labels and bounds

		private void textBoxModelName_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze)
				return;
			if (!string.IsNullOrEmpty(textBoxModelName.Text))
				editedModel.Name = textBoxModelName.Text;
		}

		private void textBoxMeshsetName_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze)
				return;
			if (!string.IsNullOrEmpty(textBoxMeshsetName.Text))
			{
				if (editedModel is ChunkAttach catt)
					catt.PolyName = textBoxMeshsetName.Text;
			}
		}

		private void textBoxVertexName_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze)
				return;
			if (!string.IsNullOrEmpty(textBoxVertexName.Text))
			{
				if (editedModel is ChunkAttach catt)
					((ChunkAttach)editedModel).VertexName = textBoxVertexName.Text;
			}
		}

		private void textBoxModelX_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze || string.IsNullOrEmpty(textBoxModelX.Text))
				return;
			float newrad = originalModel.Bounds.Center.X;
			bool result = float.TryParse(textBoxModelX.Text, out newrad);
			if (result)
				editedModel.Bounds.Center.X = newrad;
			else
				textBoxModelX.Text = originalModel.Bounds.Center.X.ToString("0.#######");
		}

		private void textBoxModelY_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze || string.IsNullOrEmpty(textBoxModelY.Text))
				return;
			float newrad = originalModel.Bounds.Center.Y;
			bool result = float.TryParse(textBoxModelY.Text, out newrad);
			if (result)
				editedModel.Bounds.Center.Y = newrad;
			else
				textBoxModelY.Text = originalModel.Bounds.Center.Y.ToString("0.#######");
		}

		private void textBoxModelZ_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze || string.IsNullOrEmpty(textBoxModelZ.Text))
				return;
			float newrad = originalModel.Bounds.Center.Z;
			bool result = float.TryParse(textBoxModelZ.Text, out newrad);
			if (result)
				editedModel.Bounds.Center.Z = newrad;
			else
				textBoxModelZ.Text = originalModel.Bounds.Center.Z.ToString("0.#######");
		}

		private void textBoxModelRadius_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze || string.IsNullOrEmpty(textBoxModelRadius.Text))
				return;
			float newrad = originalModel.Bounds.Radius;
			bool result = float.TryParse(textBoxModelRadius.Text, out newrad);
			if (result)
				editedModel.Bounds.Radius = newrad;
			else
				textBoxModelRadius.Text = originalModel.Bounds.Radius.ToString("0.#######");
		}
		#endregion
		#region UI
		private void BuildNodeList()
		{
			NJS_OBJECT[] objs = editedHierarchy.GetObjects();
			for (int i = 0; i < objs.Length; i++)
				comboBoxNode.Items.Add(i.ToString() + ": " + objs[i].Name.ToString());
		}
		private void BuildVertexDataList()
		{
			listViewVertices.Items.Clear();
			groupBoxVertList.Enabled = editedModel != null;
			if (editedModel is ChunkAttach catt)
			{
				if (catt.Vertex != null)
				{
					Dictionary<int, VertexChunk> chunks = new Dictionary<int, VertexChunk>();
					for (int i = 0; i < catt.Vertex.Count; i++)
					{
						chunks.Add(i, catt.Vertex[i]);
					}
					foreach (KeyValuePair<int, VertexChunk> verts in chunks)
					{
						ListViewItem newvert = new ListViewItem(verts.Key.ToString());
						VertexChunk vc = verts.Value;
						string mainvtype = "Vertex";
						switch (vc.Type)
						{
							case ChunkType.Vertex_VertexSH:
								mainvtype += "_SH";
								break;
							case ChunkType.Vertex_VertexNormalSH:
								mainvtype += "_VN_SH";
								break;
							case ChunkType.Vertex_Vertex:
								break;
							case ChunkType.Vertex_VertexDiffuse8:
								mainvtype += "_D8";
								break;
							case ChunkType.Vertex_VertexUserFlags:
								mainvtype += "_UF";
								break;
							case ChunkType.Vertex_VertexNinjaFlags:
								mainvtype += "_NF";
								break;
							case ChunkType.Vertex_VertexDiffuseSpecular5:
								mainvtype += "_S5";
								break;
							case ChunkType.Vertex_VertexDiffuseSpecular4:
								mainvtype += "_S4";
								break;
							case ChunkType.Vertex_VertexDiffuseSpecular16:
								mainvtype += "_IN";
								break;
							case ChunkType.Vertex_VertexNormal:
								mainvtype += "_VN";
								break;
							case ChunkType.Vertex_VertexNormalDiffuse8:
								mainvtype += "_VN_D8";
								break;
							case ChunkType.Vertex_VertexNormalUserFlags:
								mainvtype += "_VN_UF";
								break;
							case ChunkType.Vertex_VertexNormalNinjaFlags:
								mainvtype += "_VN_NF";
								break;
							case ChunkType.Vertex_VertexNormalDiffuseSpecular5:
								mainvtype += "_VN_S5";
								break;
							case ChunkType.Vertex_VertexNormalDiffuseSpecular4:
								mainvtype += "_VN_S4";
								break;
							case ChunkType.Vertex_VertexNormalDiffuseSpecular16:
								mainvtype += "_VN_IN";
								break;
							case ChunkType.Vertex_VertexNormalX:
								mainvtype += "_VNX";
								break;
							case ChunkType.Vertex_VertexNormalXDiffuse8:
								mainvtype += "_VNX_D8";
								break;
							case ChunkType.Vertex_VertexNormalXUserFlags:
								mainvtype += "_VNX_UF";
								break;
						}
						newvert.SubItems.Add(mainvtype);
						string vertexdata = "";
						switch (vc.Type)
						{
							case ChunkType.Vertex_VertexNormalNinjaFlags:
							case ChunkType.Vertex_VertexNinjaFlags:
								if (vc.Flags >> 4 == 8 && vc.HasWeight)
								{
									vertexdata += "V_CONTINUE, ";
								}
								vertexdata += "Weight " + vc.WeightStatus.ToString() + ", ";
								break;
						}
						string ent = vc.VertexCount == 1 ? " Entity" : " Entities";
						vertexdata += vc.VertexCount.ToString() + ent;
						newvert.SubItems.Add(vertexdata);
						listViewVertices.Items.Add(newvert);
					}
				}
				listViewVertices.SelectedIndices.Clear();
				listViewVertices.SelectedItems.Clear();
				listViewVertices_SelectedIndexChanged(null, null);
				listViewVertices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			}
		}
		private void BuildPolyChunkList()
		{
			listViewMeshes.Items.Clear();
			groupBoxMeshList.Enabled = editedModel != null;
			if (editedModel is ChunkAttach catt)
			{
				//editedHierarchy.StripPolyCache();
				Dictionary<int, PolyChunk> chunks = new Dictionary<int, PolyChunk>();
				if (catt.Poly != null)
				{
					for (int i = 0; i < catt.Poly.Count; i++)
					{
						chunks.Add(i, catt.Poly[i]);
					}

					foreach (KeyValuePair<int, PolyChunk> meshset in chunks)
					{
						ListViewItem newmesh = new ListViewItem(meshset.Key.ToString());
						switch (meshset.Value)
						{
							case PolyChunkBitsCachePolygonList pccp:
								newmesh.SubItems.Add("Bits_CP");
								break;
							case PolyChunkNull pcn:
								newmesh.SubItems.Add(pcn.Type.ToString());
								break;
							case PolyChunkBitsDrawPolygonList pcdp:
								newmesh.SubItems.Add("Bits_DP");
								break;
							case PolyChunkVolume pcv:
								newmesh.SubItems.Add(pcv.Type.ToString());
								break;
							case PolyChunkMaterial pcm:
								string mtype = "Material_";
								if (pcm.Diffuse.HasValue)
									mtype += "D";
								if (pcm.Ambient.HasValue)
									mtype += "A";
								if (pcm.Specular.HasValue)
									mtype += "S";
								if (pcm.Second)
									mtype += "2";
								newmesh.SubItems.Add(mtype);
								string diffuse = "";
								string ambient = "";
								string specular = "";
								if (pcm.Diffuse.HasValue)
									diffuse = "D(" + pcm.Diffuse.Value.A + ", " + pcm.Diffuse.Value.R + ", " + pcm.Diffuse.Value.G + ", " + pcm.Diffuse.Value.B + ")";
								if (pcm.Ambient.HasValue)
								{
									if (pcm.Diffuse.HasValue)
									{
										ambient = ", ";
									}
									ambient += "A(" + pcm.Ambient.Value.A + ", " + pcm.Ambient.Value.R + ", " + pcm.Ambient.Value.G + ", " + pcm.Ambient.Value.B + ")";
								}
								if (pcm.Specular.HasValue)
								{
									if (pcm.Diffuse.HasValue || pcm.Ambient.HasValue)
									{
										specular = ", ";
									}
									specular += "S(" + pcm.Specular.Value.A + ", " + pcm.Specular.Value.R + ", " + pcm.Specular.Value.G + ", " + pcm.Specular.Value.B + ")" + ", Exp " + pcm.SpecularExponent.ToString();
								}
								string sourcealpha = ", BS_";
								string destalpha = ", BD_";
								switch (pcm.SourceAlpha)
								{
									case AlphaInstruction.Zero:
										sourcealpha += "ZER";
										break;
									case AlphaInstruction.One:
										sourcealpha += "ONE";
										break;
									case AlphaInstruction.OtherColor:
										sourcealpha += "OC";
										break;
									case AlphaInstruction.InverseOtherColor:
										sourcealpha += "IOC";
										break;
									case AlphaInstruction.SourceAlpha:
										sourcealpha += "SA";
										break;
									case AlphaInstruction.InverseSourceAlpha:
										sourcealpha += "ISA";
										break;
									case AlphaInstruction.DestinationAlpha:
										sourcealpha += "DA";
										break;
									case AlphaInstruction.InverseDestinationAlpha:
										sourcealpha += "IDA";
										break;
								}
								switch (pcm.DestinationAlpha)
								{
									case AlphaInstruction.Zero:
										destalpha += "ZER";
										break;
									case AlphaInstruction.One:
										destalpha += "ONE";
										break;
									case AlphaInstruction.OtherColor:
										destalpha += "OC";
										break;
									case AlphaInstruction.InverseOtherColor:
										destalpha += "IOC";
										break;
									case AlphaInstruction.SourceAlpha:
										destalpha += "SA";
										break;
									case AlphaInstruction.InverseSourceAlpha:
										destalpha += "ISA";
										break;
									case AlphaInstruction.DestinationAlpha:
										destalpha += "DA";
										break;
									case AlphaInstruction.InverseDestinationAlpha:
										destalpha += "IDA";
										break;
								}

								newmesh.SubItems.Add(diffuse + ambient + specular + sourcealpha + destalpha);
								break;
							case PolyChunkTinyTextureID pct:
								float mip = pct.MipmapDAdjust;
								string texdata = "";
								string clamp = ", CL_";
								string flip = ", FL_";
								string filter = ", FM_";
								string mipd = ", DA_";
								texdata += "TID " + pct.TextureID;
								switch (pct.Flags & 0xF)
								{
									case 0:
									default:
										mipd += "000";
										break;
									case 1:
										mipd += "025";
										break;
									case 2:
										mipd += "050";
										break;
									case 3:
										mipd += "075";
										break;
									case 4:
										mipd += "100";
										break;
									case 5:
										mipd += "125";
										break;
									case 6:
										mipd += "150";
										break;
									case 7:
										mipd += "175";
										break;
									case 8:
										mipd += "200";
										break;
									case 9:
										mipd += "225";
										break;
									case 10:
										mipd += "250";
										break;
									case 11:
										mipd += "275";
										break;
									case 12:
										mipd += "300";
										break;
									case 13:
										mipd += "325";
										break;
									case 14:
										mipd += "350";
										break;
									case 15:
										mipd += "375";
										break;
								}
								if ((pct.Flags & 0xF) != 0)
									texdata += mipd;
								texdata += pct.SuperSample ? ", SS" : "";
								clamp += pct.ClampU ? "U" : "";
								clamp += pct.ClampV ? "V" : "";
								if (pct.ClampU || pct.ClampV)
									texdata += clamp;
								flip += pct.FlipU ? "U" : "";
								flip += pct.FlipV ? "V" : "";
								if (pct.FlipU || pct.FlipV)
									texdata += flip;
								switch (pct.FilterMode)
								{
									case FilterMode.PointSampled:
										filter += "PS";
										break;
									case FilterMode.Bilinear:
										filter += "BF";
										break;
									case FilterMode.Trilinear:
										filter += "TF";
										break;
								}
								texdata += filter;
								newmesh.SubItems.Add(pct.Type.ToString());
								newmesh.SubItems.Add(texdata);
								break;
							case PolyChunkStrip pcs:
								string stripflags = "";
								string striptype = "Strip";
								stripflags += pcs.UseAlpha ? "UA" : "";
								if (!pcs.UseAlpha)
									stripflags += pcs.DoubleSide ? "DB" : "";
								else
									stripflags += pcs.DoubleSide ? ", DB" : "";
								if ((!pcs.UseAlpha) && (!pcs.DoubleSide))
									stripflags += pcs.EnvironmentMapping ? "ENV" : "";
								else
									stripflags += pcs.EnvironmentMapping ? ", ENV" : "";
								if ((!pcs.UseAlpha) && (!pcs.DoubleSide) && (!pcs.EnvironmentMapping))
									stripflags += pcs.FlatShading ? "FL" : "";
								else
									stripflags += pcs.FlatShading ? ", FL" : "";
								if ((!pcs.UseAlpha) && (!pcs.DoubleSide) && (!pcs.EnvironmentMapping) && (!pcs.FlatShading))
									stripflags += pcs.IgnoreLight ? "IL" : "";
								else
									stripflags += pcs.IgnoreLight ? ", IL" : "";
								if ((!pcs.UseAlpha) && (!pcs.DoubleSide) && (!pcs.EnvironmentMapping) && (!pcs.FlatShading) && (!pcs.IgnoreLight))
									stripflags += pcs.IgnoreAmbient ? "IA" : "";
								else
									stripflags += pcs.IgnoreAmbient ? ", IA" : "";
								if ((!pcs.UseAlpha) && (!pcs.DoubleSide) && (!pcs.EnvironmentMapping) && (!pcs.FlatShading) && (!pcs.IgnoreLight) && (!pcs.IgnoreAmbient))
									stripflags += pcs.IgnoreSpecular ? "IS" : "";
								else
									stripflags += pcs.IgnoreSpecular ? ", IS" : "";
								switch (pcs.Type)
								{
									case ChunkType.Strip_Strip:
										break;
									case ChunkType.Strip_StripUVN:
										striptype += "_UVN";
										break;
									case ChunkType.Strip_StripUVH:
										striptype += "_UVH";
										break;
									case ChunkType.Strip_StripNormal:
										striptype += "_VN";
										break;
									case ChunkType.Strip_StripUVNNormal:
										striptype += "_UVN_VN";
										break;
									case ChunkType.Strip_StripUVHNormal:
										striptype += "_UVH_VN";
										break;
									case ChunkType.Strip_StripColor:
										striptype += "_D8";
										break;
									case ChunkType.Strip_StripUVNColor:
										striptype += "_UVN_D8";
										break;
									case ChunkType.Strip_StripUVHColor:
										striptype += "_UVH_D8";
										break;
									case ChunkType.Strip_Strip2:
										striptype += "_2";
										break;
									case ChunkType.Strip_StripUVN2:
										striptype += "_UVN2";
										break;
									case ChunkType.Strip_StripUVH2:
										striptype += "_UVH2";
										break;
								}
								newmesh.SubItems.Add(striptype);
								newmesh.SubItems.Add(stripflags);
								break;
							default:
								newmesh.SubItems.Add("WHAT");
								break;
						}
						listViewMeshes.Items.Add(newmesh);
					}
				}
			}
			listViewMeshes.SelectedIndices.Clear();
			listViewMeshes.SelectedItems.Clear();
			listViewMeshes_SelectedIndexChanged(null, null);
			listViewMeshes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		private void listViewMeshes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listViewMeshes.SelectedIndices.Count == 0)
			{
				buttonCloneMesh.Enabled = buttonDeleteMesh.Enabled = buttonMoveMeshDown.Enabled = buttonMoveMeshUp.Enabled = false;
				return;
			}
			int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			int prevIndex = 0;
			if (listViewMeshes.SelectedIndices[0] != listViewMeshes.Items[0].Index)
				prevIndex = listViewMeshes.SelectedItems[0].Index - 1;
			string polytype = listViewMeshes.SelectedItems[0].SubItems[1].Text;
			string prevmatstart = "";
			if (listViewMeshes.Items.Count > 1)
				prevmatstart = listViewMeshes.Items[prevIndex].SubItems[1].Text;
			string pdata = "";
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			PolyChunk selectedMesh = selectedObj[matID];
			if (polytype == "Null" || polytype.StartsWith("Bits"))
				buttonCloneMesh.Enabled = false;
			else
				buttonCloneMesh.Enabled = true;
			editPCMatToolStripMenuItem.Enabled = polytype.Contains("Material");
			editTextureIDToolStripMenuItem.Enabled = polytype.StartsWith("Tiny");
			editStripAlphaToolStripMenuItem.Enabled = polytype.StartsWith("Strip");
			buttonDeleteMesh.Enabled = selectedObj.Count > 1;
			buttonMoveMeshUp.Enabled = selectedObj.IndexOf(selectedMesh) > 0;
			if (prevmatstart.StartsWith("Bits"))
				buttonMoveMeshUp.Enabled = false;
			buttonMoveMeshDown.Enabled = matID + 1 < selectedObj.Count;
			if (polytype.StartsWith("Bits"))
				buttonMoveMeshDown.Enabled = false;
			if (polytype.StartsWith("Material") || polytype.StartsWith("Tiny") || polytype.StartsWith("Strip"))
			{
				if (listViewMeshes.SelectedItems[0].SubItems[2].Text != "")
				{
					pdata += ", ";
				}
				pdata += listViewMeshes.SelectedItems[0].SubItems[2].Text;
			}
			// Status bar
			string bardata = polytype;
			if (pdata != null || pdata != "")
				bardata += pdata;
			StringBuilder sb = new StringBuilder();
			sb.Append("Attributes: ");
			sb.Append(bardata);
			toolStripStatusLabelInfo.Text = sb.ToString();
		}

		private void listViewMeshes_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && listViewMeshes.SelectedIndices.Count != 0)
				contextMenuStripMatEdit.Show(listViewMeshes, e.Location);
		}

		private void listViewVertices_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && listViewVertices.SelectedIndices.Count != 0)
				contextMenuStripVertCol.Show(listViewVertices, e.Location);
		}

		private void comboBoxNode_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = comboBoxNode.SelectedIndex;
			if (index == -1)
				return;
			NJS_OBJECT[] objs = editedHierarchy.GetObjects();
			// Apply changes
			if (!freeze)
				objs[previousNodeIndex].Name = textBoxObjectName.Text;
			textBoxObjectName.Text = objs[index].Name;
			if (objs[previousNodeIndex].Attach != null && editedModel != null)
				objs[previousNodeIndex].Attach = editedModel.Clone();
			// Load new stuff
			if (objs[index].Attach != null)
			{
				editedModel = objs[index].Attach.Clone();
				originalModel = objs[index].Attach.Clone();
				textBoxModelName.Enabled = true;
				textBoxModelName.Text = editedModel.Name;
				textBoxModelRadius.Enabled = true;
				textBoxModelRadius.Text = editedModel.Bounds.Radius.ToString("0.#######");
				textBoxModelX.Enabled = true;
				textBoxModelX.Text = editedModel.Bounds.Center.X.ToString("0.#######");
				textBoxModelY.Enabled = true;
				textBoxModelY.Text = editedModel.Bounds.Center.Y.ToString("0.#######");
				textBoxModelZ.Enabled = true;
				textBoxModelZ.Text = editedModel.Bounds.Center.Z.ToString("0.#######");

				if (objs[index].Attach is ChunkAttach catt)
				{
					textBoxMeshsetName.Enabled = true;
					textBoxMeshsetName.Text = catt.PolyName;
					textBoxVertexName.Enabled = true;
					textBoxVertexName.Text = catt.VertexName;
				}
			}
			else
			{
				textBoxMeshsetName.Enabled = textBoxVertexName.Enabled = textBoxModelName.Enabled = textBoxModelRadius.Enabled = textBoxModelX.Enabled = textBoxModelY.Enabled = textBoxModelZ.Enabled = false;
				textBoxModelName.Text = textBoxMeshsetName.Text = textBoxVertexName.Text = textBoxModelName.Text = textBoxModelRadius.Text = textBoxModelX.Text = textBoxModelY.Text = textBoxModelZ.Text = "";
				editedModel = null;
			}
			previousNodeIndex = comboBoxNode.SelectedIndex;
			BuildVertexDataList();
			BuildPolyChunkList();
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			comboBoxNode_SelectedIndexChanged(sender, e);
		}
		#endregion

		private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{

		}

		private void listViewVertices_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewVertices.SelectedIndices.Count == 0)
			{
				return;
			}
			showVertexCollectionToolStripMenuItem.Enabled = true;
			string verttype = listViewVertices.SelectedItems[0].SubItems[1].Text;
			string vdata = listViewVertices.SelectedItems[0].SubItems[2].Text;
			string bardata = verttype;
			if (vdata != null)
				bardata += ", " + vdata;
			StringBuilder sb = new StringBuilder();
			sb.Append("Attributes: ");
			sb.Append(bardata);
			toolStripStatusLabelInfo.Text = sb.ToString();
		}
	}
}