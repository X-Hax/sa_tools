using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace SAModel.SAEditorCommon.UI
{
	public partial class VertexNormalDataViewer : Form
	{
		public NJS_OBJECT editedHierarchy;

		private Vertex[] VertData; // Poly data that is being edited
		private Vertex[] NormData; // Poly data that is being edited
		private readonly Vertex[] VertDataOriginal;
		private readonly Vertex[] NormDataOriginal;


		public VertexNormalDataViewer(Vertex[] verts, Vertex[] norms, int index = 0)
		{
			if (verts == null)
			{
				return;
			}
			InitializeComponent();
			VertData = verts;
			NormData = norms;
			//VertDataOriginal = VertData.Clone();
			//comboBoxVertexGroup.Items.Clear();
			//comboBoxVertexGroup.SelectedIndex = index;
			BuildVertexDataList();
		}

		#region Vertex management

		#endregion
		#region Mesh management
		private void updateVertexData(List<VertexChunk> modelchunks, int chunkID)
		{
		}

		private void buttonResetVertices_Click(object sender, System.EventArgs e)
		{
			//((ChunkAttach)editedModel).Vertex.Clear();
			//foreach (VertexChunk mesh in ((ChunkAttach)originalModel).Vertex)
			//	((ChunkAttach)editedModel).Vertex.Add(mesh);
			//BuildVertexDataList();
		}

		private void buttonCloneMesh_Click(object sender, System.EventArgs e)
		{
			//int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			//List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			//List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			//selectedMeshes.Add(selectedObj[matID]);
			//PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			//int index = selectedObj.IndexOf(selectedMesh);
			//foreach (PolyChunk mesh in selectedMeshes)
			//	selectedObj.Insert(matID + 1, mesh.Clone());
			////BuildPolyChunkList();
			//SelectMesh(Math.Min(listViewMeshes.Items.Count - 1, index + 1));
		}

		private void buttonDeleteMesh_Click(object sender, System.EventArgs e)
		{
			//int matID = int.Parse(listViewMeshes.SelectedItems[0].SubItems[0].Text);
			//List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			//List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			//selectedMeshes.Add(selectedObj[matID]);
			//PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			//int index = selectedObj.IndexOf(selectedMesh);
			//foreach (PolyChunk mesh in selectedMeshes)
			//	selectedObj.Remove(mesh);
			////BuildPolyChunkList();
			//SelectMesh(Math.Max(0, index - 1));
		}

		//private void SelectMesh(int index)
		//{
		//	listViewMeshes.SelectedIndices.Add(index);
		//}
		#endregion

		#region Editing model labels and bounds

		#endregion

		#region UI
		private void BuildVertexDataList()
		{
			listViewVertices.Items.Clear();
			List<Vertex> Vertices = VertData.ToList();
			List<Vertex> Normals = NormData.ToList();

			groupBoxVertList.Enabled = true;
			if (VertData != null)
			{
				for (int i = 0; i < VertData.Length; i++)
				{
					ListViewItem newvert = new ListViewItem(i.ToString());
					//Vertex points always exist
					newvert.SubItems.Add("VERT" + Vertices[i].ToNJA());
					if (NormData.Length < VertData.Length && i > NormData.Length)
						newvert.SubItems.Add("N/A");
					else
						newvert.SubItems.Add("NORM" + Normals[i].ToNJA());
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
		}

		private void groupBoxVertList_Enter(object sender, EventArgs e)
		{

		}
	}
}