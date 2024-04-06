using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ModelDataEditor : Form
	{
		public NJS_OBJECT editedHierarchy;

		private Attach editedModel;
		private Attach originalModel;
		private bool freeze;
		private int previousNodeIndex;

		public ModelDataEditor(NJS_OBJECT objectOriginal, int index = 0)
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
			BuildMeshsetList();
			freeze = false;
		}

		#region Meshset label and material ID editing

		private void toolStripMenuItemEditMaterialID_Click(object sender, EventArgs e)
		{
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			using (LabelEditor le = new LabelEditor(selectedMesh.MaterialID.ToString(), "", true))
			{
				if (le.ShowDialog(this) == DialogResult.OK)
				{
					int.TryParse(le.Result, out int result);
					selectedMesh.MaterialID = (ushort)result;
				}

			}
			FixLabels();
			BuildMeshsetList();
		}

		private void toolStripMenuItemEditPolyName_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			using (LabelEditor le = new LabelEditor(selectedMesh.PolyName))
			{
				if (le.ShowDialog(this) == DialogResult.OK)
					selectedMesh.PolyName = le.Result;
			}
			FixLabels();
			BuildMeshsetList();
		}

		private void toolStripMenuItemEditUVName_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			using (LabelEditor le = new LabelEditor(selectedMesh.UVName))
			{
				if (le.ShowDialog(this) == DialogResult.OK)
					selectedMesh.UVName = le.Result;
			}
			FixLabels();
			BuildMeshsetList();
		}

		private void toolStripMenuItemEditVcolorName_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			using (LabelEditor le = new LabelEditor(selectedMesh.VColorName))
			{
				if (le.ShowDialog(this) == DialogResult.OK)
					selectedMesh.VColorName = le.Result;
			}
			FixLabels();
			BuildMeshsetList();
		}

		private void toolStripMenuItemEditPolynormalName_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			using (LabelEditor le = new LabelEditor(selectedMesh.PolyNormalName))
			{
				if (le.ShowDialog(this) == DialogResult.OK)
					selectedMesh.PolyNormalName = le.Result;
			}
			FixLabels();
			BuildMeshsetList();
		}
		#endregion

		#region Mesh management

		private int CheckInvalidMaterials(NJS_MESHSET meshset, BasicAttach att)
		{
			if (meshset.MaterialID > att.Material.Count - 1)
			{
				meshset.MaterialID = (ushort)(att.Material.Count - 1);
				return 1;
			}
			return 0;
		}

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
			foreach (NJS_MESHSET meshset in ((BasicAttach)editedModel).Mesh)
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
			((BasicAttach)editedModel).Mesh.Clear();
			foreach (NJS_MESHSET mesh in ((BasicAttach)originalModel).Mesh)
				((BasicAttach)editedModel).Mesh.Add(mesh);
			BuildMeshsetList();
		}

		private void buttonCloneMesh_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			int index = ((BasicAttach)editedModel).Mesh.IndexOf(selectedMesh);
			((BasicAttach)editedModel).Mesh.Insert(((BasicAttach)editedModel).Mesh.IndexOf(selectedMesh) + 1, selectedMesh.Clone());
			FixLabels();
			BuildMeshsetList();
			SelectMesh(Math.Min(listViewMeshes.Items.Count - 1, index + 1));
		}

		private void buttonDeleteMesh_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			int index = ((BasicAttach)editedModel).Mesh.IndexOf(selectedMesh);
			((BasicAttach)editedModel).Mesh.Remove(selectedMesh);
			FixLabels();
			BuildMeshsetList();
			SelectMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveMeshUp_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			int index = ((BasicAttach)editedModel).Mesh.IndexOf(selectedMesh);
			((BasicAttach)editedModel).Mesh.Insert(index - 1, selectedMesh);
			((BasicAttach)editedModel).Mesh.RemoveAt(index + 1);
			FixLabels();
			BuildMeshsetList();
			SelectMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveMeshDown_Click(object sender, System.EventArgs e)
		{
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			int index = ((BasicAttach)editedModel).Mesh.IndexOf(selectedMesh);
			((BasicAttach)editedModel).Mesh.Insert(index + 2, selectedMesh);
			((BasicAttach)editedModel).Mesh.RemoveAt(index);
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
			if (freeze)
				return;
			if (!string.IsNullOrEmpty(textBoxModelName.Text))
				editedModel.Name = textBoxModelName.Text;
		}

		private void textBoxMaterialName_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze)
				return;
			if (editedModel is BasicAttach && !string.IsNullOrEmpty(textBoxMaterialName.Text))
				((BasicAttach)editedModel).MaterialName = textBoxMaterialName.Text;
		}

		private void textBoxMeshsetName_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze)
				return;
			if (!string.IsNullOrEmpty(textBoxMeshsetName.Text))
			{
				if (editedModel is BasicAttach batt)
					batt.MeshName = textBoxMeshsetName.Text;
			}
		}

		private void textBoxVertexName_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze)
				return;
			if (!string.IsNullOrEmpty(textBoxVertexName.Text))
			{
				if (editedModel is BasicAttach)
					((BasicAttach)editedModel).VertexName = textBoxVertexName.Text;
			}
		}

		private void textBoxNormalName_TextChanged(object sender, System.EventArgs e)
		{
			if (freeze)
				return;
			if (editedModel is BasicAttach && !string.IsNullOrEmpty(textBoxNormalName.Text))
				((BasicAttach)editedModel).NormalName = textBoxNormalName.Text;
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

		private void BuildMeshsetList()
		{
			listViewMeshes.Items.Clear();
			groupBoxMeshList.Enabled = editedModel != null && editedModel is BasicAttach;
			int invalidMaterials = 0;
			if (editedModel is BasicAttach batt)
			{
				foreach (NJS_MESHSET meshset in batt.Mesh)
				{
					invalidMaterials += CheckInvalidMaterials(meshset, batt);
					ListViewItem newmesh = new ListViewItem(batt.Mesh.IndexOf(meshset).ToString());
					newmesh.SubItems.Add(meshset.MaterialID.ToString());
					newmesh.SubItems.Add(meshset.PolyType.ToString());
					newmesh.SubItems.Add(meshset.Poly != null ? meshset.PolyName : "N/A");
					newmesh.SubItems.Add(meshset.UV != null ? meshset.UVName : "N/A");
					newmesh.SubItems.Add(meshset.VColor != null ? meshset.VColorName : "N/A");
					newmesh.SubItems.Add(meshset.PolyNormal != null ? meshset.PolyNormalName : "N/A");
					newmesh.SubItems.Add(CheckAlpha(meshset, batt) ? "Yes" : "No");
					listViewMeshes.Items.Add(newmesh);
				}
				if (invalidMaterials > 0)
					MessageBox.Show(this, invalidMaterials.ToString() + " materials were using invalid material IDs. " +
						"Those material IDs have been reset to the value " + 
						(batt.Material.Count - 1).ToString() + ".", "Model Data Editor Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
			NJS_MESHSET selectedMesh = ((BasicAttach)editedModel).Mesh[listViewMeshes.SelectedIndices[0]];
			buttonCloneMesh.Enabled = true;
			buttonDeleteMesh.Enabled = ((BasicAttach)editedModel).Mesh.Count > 1;
			buttonMoveMeshUp.Enabled = ((BasicAttach)editedModel).Mesh.IndexOf(selectedMesh) > 0;
			buttonMoveMeshDown.Enabled = ((BasicAttach)editedModel).Mesh.IndexOf(selectedMesh) < ((BasicAttach)editedModel).Mesh.Count - 1;
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
				if (objs[index].Attach is BasicAttach batt)
				{
					textBoxMaterialName.Enabled = true;
					textBoxMaterialName.Text = batt.MaterialName;
					textBoxMeshsetName.Enabled = true;
					textBoxMeshsetName.Text = batt.MeshName;
					textBoxVertexName.Enabled = true;
					textBoxVertexName.Text = batt.VertexName;
					textBoxNormalName.Enabled = true;
					textBoxNormalName.Text = batt.NormalName;
					groupBoxMeshList.Enabled = true;
					BuildMeshsetList();
				}
			}
			else
			{
				textBoxMaterialName.Enabled = textBoxMeshsetName.Enabled = textBoxNormalName.Enabled = textBoxVertexName.Enabled = textBoxModelName.Enabled = textBoxModelRadius.Enabled = textBoxModelX.Enabled = textBoxModelY.Enabled = textBoxModelZ.Enabled = false;
				textBoxModelName.Text = textBoxMaterialName.Text = textBoxMeshsetName.Text = textBoxNormalName.Text = textBoxVertexName.Text = textBoxModelName.Text = textBoxModelRadius.Text = textBoxModelX.Text = textBoxModelY.Text = textBoxModelZ.Text = "";
				editedModel = null;
			}
			previousNodeIndex = comboBoxNode.SelectedIndex;
			BuildMeshsetList();
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			comboBoxNode_SelectedIndexChanged(sender, e);
		}
		#endregion
	}
}