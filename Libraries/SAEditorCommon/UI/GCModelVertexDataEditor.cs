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
	public partial class GCModelVertexDataEditor : Form
	{
		public NJS_OBJECT editedHierarchy;
		private Attach editedModel;
		private Attach originalModel;

		private GCVertexSet VertData; // Vertex data that is being edited
		private readonly GCVertexSet VertDataOriginal; // Original vertex data at the time of opening the dialog
		private bool freeze;

		public GCModelVertexDataEditor(GCVertexSet verts, int index = 0)
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
			List<IOVtx> points = VertData.Data;

			string vdata = "";
			groupBoxVertList.Enabled = true;
			if (VertData != null)
			{
				for (int i = 0; i < VertData.Data.Count; i++)
				{
					ListViewItem newvert = new ListViewItem(i.ToString());
					//Vertex points always exist
					switch (VertData.StructType)
					{
						case GCStructType.Position_XYZ:
							vdata = "VERT( ";
							break;
						case GCStructType.Normal_XYZ:
							vdata = "NORM( ";
							break;
						case GCStructType.Color_RGBA:
							vdata = "COLOR( ";
							break;
						case GCStructType.TexCoord_ST:
							vdata = "UV( ";
							break;
					}
					vdata += VertData.Data[i].ToGCMDEStruct() + " )";
					newvert.SubItems.Add(vdata);
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