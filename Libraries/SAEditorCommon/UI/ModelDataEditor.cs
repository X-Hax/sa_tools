using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ModelDataEditor : Form
	{
		public BasicAttach editedModel;
		
		BasicAttach originalModel;
		bool freeze;

		public ModelDataEditor(Attach att)
		{
			if (att == null || !(att is BasicAttach))
				return;
			editedModel = (BasicAttach)att.Clone();
			originalModel = (BasicAttach)att;
			InitializeComponent();
			freeze = true;
			textBoxModelName.Text = editedModel.Name;
			textBoxMaterialName.Text = editedModel.MaterialName;
			textBoxMeshsetName.Text = editedModel.MeshName;
			textBoxVertexName.Text = editedModel.VertexName;
			textBoxNormalName.Text = editedModel.NormalName;
			textBoxModelRadius.Text = editedModel.Bounds.Radius.ToString("0.#######");
			textBoxModelX.Text = editedModel.Bounds.Center.X.ToString("0.#######");
			textBoxModelY.Text = editedModel.Bounds.Center.Y.ToString("0.#######");
			textBoxModelZ.Text = editedModel.Bounds.Center.Z.ToString("0.#######");
			freeze = false;
			BuildMeshsetList();
		}

		#region Meshset label editing
		private void toolStripMenuItemEditPolyName_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = editedModel.Mesh[listViewMeshes.SelectedIndices[0]];
			using (LabelEditor le = new LabelEditor(selectedMesh.PolyName))
			{
				if (le.ShowDialog(this) == DialogResult.OK)
					selectedMesh.PolyName = le.result;
			}
			FixLabels();
			BuildMeshsetList();
		}

		private void toolStripMenuItemEditUVName_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = editedModel.Mesh[listViewMeshes.SelectedIndices[0]];
			using (LabelEditor le = new LabelEditor(selectedMesh.UVName))
			{
				if (le.ShowDialog(this) == DialogResult.OK)
					selectedMesh.UVName = le.result;
			}
			FixLabels();
			BuildMeshsetList();
		}

		private void toolStripMenuItemEditVcolorName_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = editedModel.Mesh[listViewMeshes.SelectedIndices[0]];
			using (LabelEditor le = new LabelEditor(selectedMesh.VColorName))
			{
				if (le.ShowDialog(this) == DialogResult.OK)
					selectedMesh.VColorName = le.result;
			}
			FixLabels();
			BuildMeshsetList();
		}

		private void toolStripMenuItemEditPolynormalName_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = editedModel.Mesh[listViewMeshes.SelectedIndices[0]];
			using (LabelEditor le = new LabelEditor(selectedMesh.PolyNormalName))
			{
				if (le.ShowDialog(this) == DialogResult.OK)
					selectedMesh.PolyNormalName = le.result;
			}
			FixLabels();
			BuildMeshsetList();
		}
		#endregion

		#region Mesh management

		private bool CheckAlpha(NJS_MESHSET meshset, BasicAttach att)
		{
			if (att.Material[meshset.MaterialID] != null)
				return att.Material[meshset.MaterialID].UseAlpha;
			return false;
		}

		private void FixLabels()
		{
			List<string> labels = new List<string>();
			int counter = 0;
			foreach (NJS_MESHSET meshset in editedModel.Mesh)
			{
				// Poly
				counter = 0;
				if (meshset.Poly != null)
				{
					if (!labels.Contains(meshset.PolyName))
					{
						counter = 0;
						labels.Add(meshset.PolyName);
					}
					else
					{
						do
						{
							counter++;
							meshset.PolyName += "_" + counter.ToString();
						}
						while (labels.Contains(meshset.PolyName));
					}
				}
				// UV
				counter = 0;
				if (meshset.UV != null)
				{
					if (!labels.Contains(meshset.UVName))
					{
						counter = 0;
						labels.Add(meshset.UVName);
					}
					else
					{
						do
						{
							counter++;
							meshset.UVName += "_" + counter.ToString();
						}
						while (labels.Contains(meshset.UVName));
					}
				}
				counter = 0;
				if (meshset.VColor != null)
				{
					// VColor
					if (!labels.Contains(meshset.VColorName))
					{
						counter = 0;
						labels.Add(meshset.VColorName);
					}
					else
					{
						do
						{
							counter++;
							meshset.VColorName += "_" + counter.ToString();
						}
						while (labels.Contains(meshset.VColorName));
					}
				}
				counter = 0;
				// Polynormal	
				if (meshset.PolyNormal != null)
				{
					if (!labels.Contains(meshset.PolyNormalName))
					{
						counter = 0;
						labels.Add(meshset.PolyNormalName);
					}
					else
					{
						do
						{
							counter++;
							meshset.PolyNormalName += "_" + counter.ToString();
						}
						while (labels.Contains(meshset.PolyNormalName));
					}
				}
			}
		}

		private void buttonResetMeshes_Click(object sender, System.EventArgs e)
		{
			editedModel.Mesh.Clear();
			foreach (NJS_MESHSET mesh in originalModel.Mesh)
				editedModel.Mesh.Add(mesh);
			BuildMeshsetList();
		}

		private void buttonCloneMesh_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = editedModel.Mesh[listViewMeshes.SelectedIndices[0]];
			int index = editedModel.Mesh.IndexOf(selectedMesh);
			editedModel.Mesh.Insert(editedModel.Mesh.IndexOf(selectedMesh) + 1, selectedMesh.Clone());
			FixLabels();
			BuildMeshsetList();
			SelectMesh(Math.Min(listViewMeshes.Items.Count - 1, index + 1));
		}

		private void buttonDeleteMesh_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = editedModel.Mesh[listViewMeshes.SelectedIndices[0]];
			int index = editedModel.Mesh.IndexOf(selectedMesh);
			editedModel.Mesh.Remove(selectedMesh);
			FixLabels();
			BuildMeshsetList();
			SelectMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveMeshUp_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = editedModel.Mesh[listViewMeshes.SelectedIndices[0]];
			int index = editedModel.Mesh.IndexOf(selectedMesh);
			editedModel.Mesh.Insert(index - 1, selectedMesh);
			editedModel.Mesh.RemoveAt(index + 1);
			FixLabels();
			BuildMeshsetList();
			SelectMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveMeshDown_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = editedModel.Mesh[listViewMeshes.SelectedIndices[0]];
			int index = editedModel.Mesh.IndexOf(selectedMesh);
			editedModel.Mesh.Insert(index + 2, selectedMesh);
			editedModel.Mesh.RemoveAt(index);
			FixLabels();
			BuildMeshsetList();
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
			editedModel.Name = textBoxModelName.Text;
		}

		private void textBoxMaterialName_TextChanged(object sender, System.EventArgs e)
		{
			editedModel.MaterialName = textBoxMaterialName.Text;
		}

		private void textBoxMeshsetName_TextChanged(object sender, System.EventArgs e)
		{
			editedModel.MeshName = textBoxMeshsetName.Text;
		}

		private void textBoxVertexName_TextChanged(object sender, System.EventArgs e)
		{
			editedModel.VertexName = textBoxVertexName.Text;
		}

		private void textBoxNormalName_TextChanged(object sender, System.EventArgs e)
		{
			editedModel.NormalName = textBoxNormalName.Text;
		}

		private void textBoxModelX_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze)
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
			if (freeze)
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
			if (freeze)
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
			if (freeze)
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
		private void BuildMeshsetList()
		{
			listViewMeshes.Items.Clear();
			foreach (NJS_MESHSET meshset in editedModel.Mesh)
			{
				ListViewItem newmesh = new ListViewItem(editedModel.Mesh.IndexOf(meshset).ToString());
				newmesh.SubItems.Add(meshset.MaterialID.ToString());
				newmesh.SubItems.Add(meshset.PolyType.ToString());
				newmesh.SubItems.Add(meshset.Poly != null ? meshset.PolyName : "N/A");
				newmesh.SubItems.Add(meshset.UV != null ? meshset.UVName : "N/A");
				newmesh.SubItems.Add(meshset.VColor != null ? meshset.VColorName : "N/A");
				newmesh.SubItems.Add(meshset.PolyNormal != null ? meshset.PolyNormalName : "N/A");
				newmesh.SubItems.Add(CheckAlpha(meshset, editedModel) ? "Yes" : "No");
				listViewMeshes.Items.Add(newmesh);
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
			NJS_MESHSET selectedMesh = editedModel.Mesh[listViewMeshes.SelectedIndices[0]];
			buttonCloneMesh.Enabled = true;
			buttonDeleteMesh.Enabled = editedModel.Mesh.Count > 1;
			buttonMoveMeshUp.Enabled = editedModel.Mesh.IndexOf(selectedMesh) > 0;
			buttonMoveMeshDown.Enabled = editedModel.Mesh.IndexOf(selectedMesh) < editedModel.Mesh.Count - 1;
			// Status bar
			StringBuilder sb = new StringBuilder();
			if (selectedMesh.Poly != null)
				sb.Append("Polys: " + selectedMesh.Poly.Count + " ");
			if (selectedMesh.UV != null)
				sb.Append("UVs: " + selectedMesh.UV.Length + " ");
			if (selectedMesh.PolyNormal != null)
				sb.Append("Polynormals: " + selectedMesh.PolyNormal.Length);
			sb.Append("Attributes: " + selectedMesh.PAttr.ToString());
			toolStripStatusLabelInfo.Text = sb.ToString();
			// Label editor
			toolStripMenuItemEditPolyName.Enabled = selectedMesh.Poly != null;
			toolStripMenuItemEditUVName.Enabled = selectedMesh.UV != null;
			toolStripMenuItemEditVcolorName.Enabled = selectedMesh.VColor != null;
			toolStripMenuItemEditPolynormalName.Enabled = selectedMesh.PolyNormal != null;
		}

		private void listViewMeshes_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && listViewMeshes.SelectedIndices.Count != 0)
				contextMenuStripLabels.Show(listViewMeshes, e.Location);
		}
		#endregion
	}
}
