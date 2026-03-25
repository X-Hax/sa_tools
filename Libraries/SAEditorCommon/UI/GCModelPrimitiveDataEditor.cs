using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System;
using SAModel.GC;

namespace SAModel.SAEditorCommon.UI
{
	public partial class GCModelPrimitiveDataEditor : Form
	{
		public NJS_OBJECT editedHierarchy;
		//private Attach editedModel;
		//private Attach originalModel;

		private List<GCPrimitive> PrimDataSet; // Vertex data that is being edited

		public GCModelPrimitiveDataEditor(GCMesh meshdata, int index = 0)
		{
			if (meshdata.Primitives == null)
			{
				return;
			}
			InitializeComponent();
			PrimDataSet = meshdata.Primitives;
			numericUpDownPrimSet.Maximum = PrimDataSet.Count - 1;
			//comboBoxVertexGroup.Items.Clear();
			//comboBoxVertexGroup.SelectedIndex = index;
			BuildPrimitiveDataList();
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
		private void BuildPrimitiveDataList()
		{
			listViewVertices.Items.Clear();
			GCPrimitive selectedPrim = PrimDataSet[(int)numericUpDownPrimSet.Value];
			string pdata = "";
			string ndata = "";
			string cdata = "";
			string udata = "";
			groupBoxPrimList.Enabled = true;
			string primtype = "";
			if (PrimDataSet != null)
			{
					List<Loop> points = selectedPrim.Loops;
					switch (selectedPrim.PrimitiveType)
					{
						case GCPrimitiveType.Triangles:
							primtype = "Triangles";
							break;
						case GCPrimitiveType.TriangleStrip:
							primtype = "Triangle Strip";
							break;
						case GCPrimitiveType.TriangleFan:
							primtype = "Triangle Fan";
							break;
						case GCPrimitiveType.Lines:
							primtype = "Lines";
							break;
						case GCPrimitiveType.LineStrip:
							primtype = "Line Strip";
							break;
						case GCPrimitiveType.Points:
							primtype = "Points";
							break;
					}
					labelPrimitiveType.Text = primtype;
					for (int l = 0; l < selectedPrim.Loops.Count; l++)
					{
						Loop pset = selectedPrim.Loops[l];
						ListViewItem newprim = new ListViewItem(l.ToString());
						pdata = pset.PositionIndex.ToString();
						ndata = pset.NormalIndex.ToString();
						cdata = pset.Color0Index.ToString();
						udata = pset.UV0Index.ToString();
						newprim.SubItems.Add(pdata);
						newprim.SubItems.Add(ndata);
						newprim.SubItems.Add(cdata);
						newprim.SubItems.Add(udata);
						listViewVertices.Items.Add(newprim);
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

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			BuildPrimitiveDataList();
		}
	}
}