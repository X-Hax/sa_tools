using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System;
using System.DirectoryServices.ActiveDirectory;
using SAModel.Direct3D.TextureSystem;
using SAModel.GC;
using static VrSharp.Xvr.DirectXTexUtility;
using SharpDX.Direct3D9;
using System.Runtime.Intrinsics.X86;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ChunkModelVertexDataEditor : Form
	{
		public NJS_OBJECT editedHierarchy;
		private Attach editedModel;
		private Attach originalModel;

		private VertexChunk VertData; // Poly data that is being edited
		private readonly VertexChunk VertDataOriginal; // Original poly data at the time of opening the dialog
		private bool freeze;

		public ChunkModelVertexDataEditor(VertexChunk verts, int index = 0)
		{
			if (verts == null)
			{
				return;
			}
			InitializeComponent();
			freeze = true;
			VertData = verts;
			//comboBoxVertexGroup.Items.Clear();
			//comboBoxVertexGroup.SelectedIndex = index;
			BuildVertexDataList();
			freeze = false;
		}

		#region Vertex management

		#endregion
		#region Mesh management
		private void updateVertexData(List<VertexChunk> modelchunks, int chunkID)
		{
		}

		private void buttonResetVertices_Click(object sender, System.EventArgs e)
		{
			((ChunkAttach)editedModel).Vertex.Clear();
			foreach (VertexChunk mesh in ((ChunkAttach)originalModel).Vertex)
				((ChunkAttach)editedModel).Vertex.Add(mesh);
			BuildVertexDataList();
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
			//BuildPolyChunkList();
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
			//BuildPolyChunkList();
			SelectMesh(Math.Max(0, index - 1));
		}

		private void SelectMesh(int index)
		{
			listViewMeshes.SelectedIndices.Add(index);
		}
		#endregion

		#region Editing model labels and bounds

		#endregion

		#region UI
		private void BuildVertexDataList()
		{
			listViewVertices.Items.Clear();
			List<Vertex> Vertices = VertData.Vertices;
			List<Vertex> Normals = VertData.Normals;
			List<System.Drawing.Color> Diffuse = VertData.Diffuse;
			List<System.Drawing.Color> Specular = VertData.Specular;
			List<uint> UserFlags = VertData.UserFlags;
			List<uint> NinjaFlags = VertData.NinjaFlags;

			string points = "";
			string norms = "";
			string diffspec = "";
			string uf = "";
			string nf = "";
			groupBoxVertList.Enabled = true;
			if (VertData != null)
			{
				for (int i = 0; i < VertData.VertexCount; i++)
				{
					ListViewItem newvert = new ListViewItem(i.ToString());
					//Vertex points always exist
					points = "VERT(" + Vertices[i].ToString() + ")";
					switch (VertData.Type)
					{
						default:
							break;
						case ChunkType.Vertex_VertexDiffuse8:
							diffspec = ", D8888(" + Diffuse[i].ToString() + ")"; 
							break;
						case ChunkType.Vertex_VertexDiffuseSpecular16:
							diffspec = ", D16(" + Diffuse[i].ToString() + "), S16(" + Specular[i].ToString() + ")";
							break;
						case ChunkType.Vertex_VertexDiffuseSpecular4:
							diffspec = ", D4444(" + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString()
								+ "), S565(" + Specular[i].R.ToString() + ", " + Specular[i].G.ToString() + ", " + Specular[i].B.ToString() + ")";
							break;
						case ChunkType.Vertex_VertexDiffuseSpecular5:
							diffspec = "D565(" + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString()
								+ "), S565(" + Specular[i].R.ToString() + ", " + Specular[i].G.ToString() + ", " + Specular[i].B.ToString() + ")";
							break;
						case ChunkType.Vertex_VertexNinjaFlags:
							nf = ", NFlags(0x" + NinjaFlags[i].ToString("X8") + ")";
							break;
						case ChunkType.Vertex_VertexUserFlags:
							uf = ", UFlags(0x" + UserFlags[i].ToCHex() + ")";
							break;
						case ChunkType.Vertex_VertexNormal:
						case ChunkType.Vertex_VertexNormalDiffuse8:
						case ChunkType.Vertex_VertexNormalDiffuseSpecular16:
						case ChunkType.Vertex_VertexNormalDiffuseSpecular4:
						case ChunkType.Vertex_VertexNormalDiffuseSpecular5:
						case ChunkType.Vertex_VertexNormalNinjaFlags:
						case ChunkType.Vertex_VertexNormalUserFlags:
							Normals = VertData.Normals;
							norms = ", NORM(" + Normals[i].ToString() + ")";
							if (VertData.Type == ChunkType.Vertex_VertexNormalUserFlags)
							{
								UserFlags = VertData.UserFlags;
								uf = ", UFlags(0x" + UserFlags[i].ToCHex() + ")";
							}
							if (VertData.Type == ChunkType.Vertex_VertexNormalNinjaFlags)
							{
								NinjaFlags = VertData.NinjaFlags;
								nf = ", NFlags(0x" + NinjaFlags[i].ToString("X8") + ")";
							}
							switch (VertData.Type)
							{
								case ChunkType.Vertex_VertexNormalDiffuse8:
									diffspec = ", D8888(" + Diffuse[i].ToString() + ")";
									break;
								case ChunkType.Vertex_VertexNormalDiffuseSpecular16:
									diffspec = ", D16(" + Diffuse[i].ToString() + "), S16(" + Specular[i].ToString() + ")";
									break;
								case ChunkType.Vertex_VertexNormalDiffuseSpecular4:
									diffspec = ", D4444(" + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString()
									+ "), S565(" + Specular[i].R.ToString() + ", " + Specular[i].G.ToString() + ", " + Specular[i].B.ToString() + ")";
									break;
								case ChunkType.Vertex_VertexNormalDiffuseSpecular5:
									diffspec = "D565(" + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString()
									+ "), S565(" + Specular[i].R.ToString() + ", " + Specular[i].G.ToString() + ", " + Specular[i].B.ToString() + ")";
									break;
							}
							break;
					}
					newvert.SubItems.Add(points + norms + diffspec + uf + nf);
					listViewVertices.Items.Add(newvert);
				}
			}
				listViewVertices.SelectedIndices.Clear();
				listViewVertices.SelectedItems.Clear();
				listViewVertices_SelectedIndexChanged(null, null);
				listViewVertices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			Close();
		}
		#endregion

		private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{

		}

		private void listViewVertices_SelectedIndexChanged(object sender, EventArgs e)
		{
			//contextMenuStripVertCol.Enabled;
			//string verttype = listViewMeshes.SelectedItems[0].SubItems[1].Text;
			//string vdata = listViewMeshes.SelectedItems[0].SubItems[2].Text;
			//string bardata = verttype;
			//if (vdata != null)
			//	bardata += ", " + vdata;
			StringBuilder sb = new StringBuilder();
			sb.Append("Attributes: ");
			//sb.Append(bardata);
			toolStripStatusLabelInfo.Text = sb.ToString();
		}
	}
}