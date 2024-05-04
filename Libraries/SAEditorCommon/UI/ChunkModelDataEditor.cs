using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ChunkModelDataEditor : Form
	{
		public NJS_OBJECT editedHierarchy;

		private Attach editedModel;
		private Attach originalModel;
		private bool freeze;
		private int previousNodeIndex;

		public ChunkModelDataEditor(NJS_OBJECT objectOriginal, int index = 0)
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
			BuildPolyChunkList();
			freeze = false;
		}
		#region Mesh management

		private void buttonResetMeshes_Click(object sender, System.EventArgs e)
		{
			((ChunkAttach)editedModel).Poly.Clear();
			foreach (PolyChunk mesh in ((ChunkAttach)originalModel).Poly)
				((ChunkAttach)editedModel).Poly.Add(mesh);
			BuildPolyChunkList();
		}

		private void buttonCloneMesh_Click(object sender, System.EventArgs e)
		{
			int matID1 = int.Parse(listViewMeshes.SelectedItems[0].SubItems[2].Text);
			int cnkcount = int.Parse(listViewMeshes.SelectedItems[0].SubItems[1].Text);
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			if (cnkcount == 3)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[6].Text)]);
			if (cnkcount >= 2)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[4].Text)]);
			selectedMeshes.Add(selectedObj[matID1]);
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			foreach (PolyChunk mesh in selectedMeshes)
			selectedObj.Insert(matID1 + cnkcount, mesh.Clone());
			BuildPolyChunkList();
			SelectMesh(Math.Min(listViewMeshes.Items.Count - 1, index + 1));
		}

		private void buttonDeleteMesh_Click(object sender, System.EventArgs e)
		{
			int matID1 = int.Parse(listViewMeshes.SelectedItems[0].SubItems[2].Text);
			int cnkcount = int.Parse(listViewMeshes.SelectedItems[0].SubItems[1].Text);
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			if (cnkcount == 3)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[6].Text)]);
			if (cnkcount >= 2)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[4].Text)]);
			selectedMeshes.Add(selectedObj[matID1]);
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
			int matID1 = int.Parse(listViewMeshes.SelectedItems[0].SubItems[2].Text);
			int pmatID1 = int.Parse(listViewMeshes.Items[prevIndex].SubItems[2].Text);
			int cnkcount = int.Parse(listViewMeshes.SelectedItems[0].SubItems[1].Text);
			int pcnkcount = int.Parse(listViewMeshes.Items[prevIndex].SubItems[1].Text);
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			if (cnkcount == 4)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[8].Text)]);
			if (cnkcount >= 3)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[6].Text)]);
			if (cnkcount >= 2)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[4].Text)]);
			selectedMeshes.Add(selectedObj[matID1]);
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			foreach (PolyChunk mesh in selectedMeshes)
				selectedObj.Insert(pmatID1, mesh);
			foreach (PolyChunk mesh in selectedMeshes)
				selectedObj.RemoveAt(pmatID1 + pcnkcount + cnkcount);
			BuildPolyChunkList();
			SelectMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveMeshDown_Click(object sender, System.EventArgs e)
		{
			int nextIndex = listViewMeshes.SelectedItems[0].Index + 1;
			int matID1 = int.Parse(listViewMeshes.SelectedItems[0].SubItems[2].Text);
			int nmatID1 = int.Parse(listViewMeshes.Items[nextIndex].SubItems[2].Text);
			int cnkcount = int.Parse(listViewMeshes.SelectedItems[0].SubItems[1].Text);
			int ncnkcount = int.Parse(listViewMeshes.Items[nextIndex].SubItems[1].Text);
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			List<PolyChunk> selectedMeshes = new List<PolyChunk>();
			if (cnkcount == 4)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[8].Text)]);
			if (cnkcount >= 3)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[6].Text)]);
			if (cnkcount >= 2)
				selectedMeshes.Add(selectedObj[int.Parse(listViewMeshes.SelectedItems[0].SubItems[4].Text)]);
			selectedMeshes.Add(selectedObj[matID1]);
			PolyChunk selectedMesh = selectedObj[listViewMeshes.SelectedIndices[0]];
			int index = selectedObj.IndexOf(selectedMesh);
			foreach (PolyChunk mesh in selectedMeshes)
			{
				selectedObj.Insert(nmatID1 + ncnkcount, mesh);
			}
			foreach (PolyChunk mesh in selectedMeshes)
			{
				selectedObj.RemoveAt(matID1);
			}
			BuildPolyChunkList();
			SelectMesh(Math.Min(listViewMeshes.Items.Count -1, index + 1));
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

		private void BuildPolyChunkList()
		{
			listViewMeshes.Items.Clear();
			groupBoxMeshList.Enabled = editedModel != null;
			if (editedModel is ChunkAttach catt)
			{
				//editedHierarchy.StripPolyCache();
				Dictionary<int, PolyChunk> chunks = new Dictionary<int, PolyChunk>();
				Dictionary<int, PolyChunk> nullchunk = new Dictionary<int, PolyChunk>();
				Dictionary<int, Dictionary<int, PolyChunk>> wholechunks = new Dictionary<int, Dictionary<int, PolyChunk>>();
				if (catt.Poly != null)
				{
					int n = 0;
					for (int i = 0; i < catt.Poly.Count; i++)
					{
						if (catt.Poly[i] is PolyChunkVolume)
						{
							chunks.Add(i, catt.Poly[i]);
						}
						if (catt.Poly[i] is PolyChunkBitsDrawPolygonList)
						{
							chunks.Add(i, catt.Poly[i]);
						}
						if (catt.Poly[i] is PolyChunkBitsCachePolygonList)
						{
							nullchunk.Add(i, catt.Poly[i]);
							i++;
						}
						if (catt.Poly[i] is PolyChunkNull)
						{
							nullchunk.Add(i, catt.Poly[i]);
							i++;
						}
						if (catt.Poly[i] is PolyChunkMaterial)
						{
							chunks.Add(i, catt.Poly[i]);
							i++;
						}
						if (catt.Poly[i] is PolyChunkTinyTextureID)
						{
							chunks.Add(i, catt.Poly[i]);
							i++;
						}
						if (catt.Poly[i] is PolyChunkStrip)
						{
							chunks.Add(i, catt.Poly[i]);
						}
						if (nullchunk.Count > 0)
						{
							wholechunks.Add(n, nullchunk);
							wholechunks.Add(n + 1, chunks);
							n++;
						}
						else
							wholechunks.Add(n, chunks);
						n++;
						chunks = new Dictionary<int, PolyChunk>();
						nullchunk = new Dictionary<int, PolyChunk>();
					}

					foreach (KeyValuePair<int, Dictionary<int, PolyChunk>> meshset in wholechunks)
					{
						ListViewItem newmesh = new ListViewItem(meshset.Key.ToString());
						newmesh.SubItems.Add((meshset.Value.Count).ToString());
						foreach (KeyValuePair<int, PolyChunk> mesh in meshset.Value)
						{
							newmesh.SubItems.Add(mesh.Key.ToString());
							switch (mesh.Value)
							{
								case PolyChunkBitsCachePolygonList pccp:
									newmesh.SubItems.Add(pccp.Type.ToString());
									break;
								case PolyChunkNull pcn:
									newmesh.SubItems.Add(pcn.Type.ToString());
									break;
								case PolyChunkBitsDrawPolygonList pcdp:
									newmesh.SubItems.Add(pcdp.Type.ToString());
									break;
								case PolyChunkVolume pcv:
									newmesh.SubItems.Add(pcv.Type.ToString());
									break;
								case PolyChunkMaterial pcm:
									newmesh.SubItems.Add(pcm.Type.ToString());
									break;
								case PolyChunkTinyTextureID pct:
									int tid = pct.TextureID;
									newmesh.SubItems.Add(pct.Type.ToString() + ", TexID " + tid.ToString());
									break;
								case PolyChunkStrip pcs:
									string alpha = pcs.UseAlpha ? ", Alpha" : ", No Alpha";
									newmesh.SubItems.Add(pcs.Type.ToString() + alpha);
									break;
							}
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
			int matID1 = int.Parse(listViewMeshes.SelectedItems[0].SubItems[2].Text);
			int prevIndex = 0;
			if (listViewMeshes.SelectedIndices[0] != listViewMeshes.Items[0].Index)
				prevIndex = listViewMeshes.SelectedItems[0].Index - 1;
			int cnkcount = int.Parse(listViewMeshes.SelectedItems[0].SubItems[1].Text);
			string matstart = listViewMeshes.SelectedItems[0].SubItems[3].Text;
			string prevmatstart = listViewMeshes.Items[prevIndex].SubItems[3].Text;
			List<string> materialattrs = new List<string>();
			materialattrs.Add(matstart);
			if (cnkcount >= 2)
				materialattrs.Add(listViewMeshes.SelectedItems[0].SubItems[5].Text);
			if (cnkcount == 3)
				materialattrs.Add(listViewMeshes.SelectedItems[0].SubItems[7].Text);
			List<PolyChunk> selectedObj = ((ChunkAttach)editedModel).Poly;
			PolyChunk selectedMesh = selectedObj[matID1];
			if (matstart == "Null" || matstart.StartsWith("Bits"))
				buttonCloneMesh.Enabled = false;
			else
				buttonCloneMesh.Enabled = true;
			buttonDeleteMesh.Enabled = selectedObj.Count > 1;
			buttonMoveMeshUp.Enabled = selectedObj.IndexOf(selectedMesh) > 0;
			if (prevmatstart.StartsWith("Bits"))
				buttonMoveMeshUp.Enabled = false;
			buttonMoveMeshDown.Enabled = matID1 + cnkcount < selectedObj.Count - 1;
			if (matstart.StartsWith("Bits"))
				buttonMoveMeshDown.Enabled = false;
			// Status bar
			StringBuilder sb = new StringBuilder();
			sb.Append("Attributes: ");
			if (materialattrs.Count > 1)
			{
				for (int i = 0; i < materialattrs.Count - 1; i++)
					sb.Append(materialattrs[i].ToString() + ", ");
				sb.Append(materialattrs[materialattrs.Count - 1].ToString());
			}
			else
				sb.Append(materialattrs[0].ToString());
			toolStripStatusLabelInfo.Text = sb.ToString();
		}

		private void listViewMeshes_MouseClick(object sender, MouseEventArgs e)
		{

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
			BuildPolyChunkList();
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			comboBoxNode_SelectedIndexChanged(sender, e);
		}
		#endregion
	}
}