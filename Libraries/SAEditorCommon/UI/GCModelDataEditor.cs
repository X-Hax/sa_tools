using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System;
using SAModel.GC;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using SharpDX.Direct3D9;
using SAModel.Direct3D.TextureSystem;

namespace SAModel.SAEditorCommon.UI
{
	public partial class GCModelDataEditor : Form
	{
		public NJS_OBJECT editedHierarchy;

		private Attach editedModel;
		private Attach originalModel;
		private bool freeze;
		private int previousNodeIndex;
		private readonly BMPInfo[] textures;

		public GCModelDataEditor(NJS_OBJECT objectOriginal, BMPInfo[] textures, int index = 0)
		{
			if (objectOriginal == null)
				return;
			InitializeComponent();
			freeze = true;
			comboBoxNode.Items.Clear();
			editedHierarchy = objectOriginal.Clone();
			BuildNodeList();
			comboBoxNode.SelectedIndex = index;
			BuildVertexList();
			BuildOpaqueMeshList();
			BuildTranslucentMeshList();
			this.textures = textures;
			freeze = false;
		}

		#region Meshset label and material ID editing
		#endregion

		#region Mesh management

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

		private void buttonResetTMeshes_Click(object sender, System.EventArgs e)
		{
			((GC.GCAttach)editedModel).TranslucentMeshes.Clear();
			foreach (GCMesh mesh in ((GC.GCAttach)originalModel).TranslucentMeshes)
				((GC.GCAttach)editedModel).TranslucentMeshes.Add(mesh);
			BuildTranslucentMeshList();
		}
		private void buttonResetOMeshes_Click(object sender, System.EventArgs e)
		{
			((GC.GCAttach)editedModel).OpaqueMeshes.Clear();
			foreach (GCMesh mesh in ((GC.GCAttach)originalModel).OpaqueMeshes)
				((GC.GCAttach)editedModel).OpaqueMeshes.Add(mesh);
			BuildOpaqueMeshList();
		}

		private void buttonCloneOMesh_Click(object sender, System.EventArgs e)
		{
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).OpaqueMeshes[listViewOMeshes.SelectedIndices[0]];
			int index = ((GC.GCAttach)editedModel).OpaqueMeshes.IndexOf(selectedMesh);
			((GC.GCAttach)editedModel).OpaqueMeshes.Insert(((GC.GCAttach)editedModel).OpaqueMeshes.IndexOf(selectedMesh) + 1, selectedMesh.Clone());
			//FixLabels();
			BuildOpaqueMeshList();
			SelectOMesh(Math.Min(listViewOMeshes.Items.Count - 1, index + 1));
		}
		private void buttonCloneTMesh_Click(object sender, System.EventArgs e)
		{
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).TranslucentMeshes[listViewOMeshes.SelectedIndices[0]];
			int index = ((GC.GCAttach)editedModel).TranslucentMeshes.IndexOf(selectedMesh);
			((GC.GCAttach)editedModel).TranslucentMeshes.Insert(((GC.GCAttach)editedModel).TranslucentMeshes.IndexOf(selectedMesh) + 1, selectedMesh.Clone());
			//FixLabels();
			BuildTranslucentMeshList();
			SelectTMesh(Math.Min(listViewTMeshes.Items.Count - 1, index + 1));
		}

		private void buttonDeleteOMesh_Click(object sender, System.EventArgs e)
		{
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).OpaqueMeshes[listViewOMeshes.SelectedIndices[0]];
			int index = ((GC.GCAttach)editedModel).OpaqueMeshes.IndexOf(selectedMesh);
			((GC.GCAttach)editedModel).OpaqueMeshes.Remove(selectedMesh);
			//FixLabels();
			BuildOpaqueMeshList();
			SelectOMesh(Math.Max(0, index - 1));
		}

		private void buttonDeleteTMesh_Click(object sender, System.EventArgs e)
		{
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).TranslucentMeshes[listViewOMeshes.SelectedIndices[0]];
			int index = ((GC.GCAttach)editedModel).TranslucentMeshes.IndexOf(selectedMesh);
			((GC.GCAttach)editedModel).TranslucentMeshes.Remove(selectedMesh);
			//FixLabels();
			BuildTranslucentMeshList();
			SelectTMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveOMeshUp_Click(object sender, System.EventArgs e)
		{
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).OpaqueMeshes[listViewOMeshes.SelectedIndices[0]];
			int index = ((GC.GCAttach)editedModel).OpaqueMeshes.IndexOf(selectedMesh);
			((GC.GCAttach)editedModel).OpaqueMeshes.Insert(index - 1, selectedMesh);
			((GC.GCAttach)editedModel).OpaqueMeshes.RemoveAt(index + 1);
			//FixLabels();
			BuildOpaqueMeshList();
			SelectOMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveTMeshUp_Click(object sender, System.EventArgs e)
		{
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).TranslucentMeshes[listViewOMeshes.SelectedIndices[0]];
			int index = ((GC.GCAttach)editedModel).TranslucentMeshes.IndexOf(selectedMesh);
			((GC.GCAttach)editedModel).TranslucentMeshes.Insert(index - 1, selectedMesh);
			((GC.GCAttach)editedModel).TranslucentMeshes.RemoveAt(index + 1);
			//FixLabels();
			BuildTranslucentMeshList();
			SelectTMesh(Math.Max(0, index - 1));
		}

		private void buttonMoveOMeshDown_Click(object sender, System.EventArgs e)
		{
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).OpaqueMeshes[listViewOMeshes.SelectedIndices[0]];
			int index = ((GC.GCAttach)editedModel).OpaqueMeshes.IndexOf(selectedMesh);
			((GC.GCAttach)editedModel).OpaqueMeshes.Insert(index + 2, selectedMesh);
			((GC.GCAttach)editedModel).OpaqueMeshes.RemoveAt(index);
			//FixLabels();
			BuildOpaqueMeshList();
			SelectOMesh(Math.Min(listViewOMeshes.Items.Count - 1, index + 1));
		}

		private void buttonMoveTMeshDown_Click(object sender, System.EventArgs e)
		{
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).TranslucentMeshes[listViewOMeshes.SelectedIndices[0]];
			int index = ((GC.GCAttach)editedModel).TranslucentMeshes.IndexOf(selectedMesh);
			((GC.GCAttach)editedModel).TranslucentMeshes.Insert(index + 2, selectedMesh);
			((GC.GCAttach)editedModel).TranslucentMeshes.RemoveAt(index);
			//FixLabels();
			BuildTranslucentMeshList();
			SelectTMesh(Math.Min(listViewOMeshes.Items.Count - 1, index + 1));
		}

		private void SelectOMesh(int index)
		{
			listViewOMeshes.SelectedIndices.Add(index);
		}
		private void SelectTMesh(int index)
		{
			listViewTMeshes.SelectedIndices.Add(index);
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

		private void textBoxTPolyName_TextChanged(object sender, System.EventArgs e)
		{
			//if (freeze)
			//	return;
			//if (!string.IsNullOrEmpty(textBoxTPolyName.Text))
			//{
			//	if (editedModel is GCAttach gatt)
			//		gatt.TranslucentMeshName = textBoxTPolyName.Text;
			//}
		}

		private void textBoxOPolyName_TextChanged(object sender, System.EventArgs e)
		{
			//if (freeze)
			//	return;
			//if (!string.IsNullOrEmpty(textBoxOPolyName.Text))
			//{
			//	if (editedModel is GCAttach gatt)
			//		gatt.OpaqueMeshName = textBoxOPolyName.Text;
			//}
		}

		private void textBoxVertexName_TextChanged(object sender, System.EventArgs e)
		{
			//	if (freeze)
			//		return;
			//	if (!string.IsNullOrEmpty(textBoxVertexName.Text))
			//	{
			//		if (editedModel is GCAttach)
			//			((GCAttach)editedModel).VertexName = textBoxVertexName.Text;
			//	}
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
		private void BuildVertexList()
		{
			listViewVertices.Items.Clear();
			groupBoxVertexList.Enabled = editedModel != null;
			if (editedModel is GC.GCAttach gcmodel)
			{
				Dictionary<int, GCVertexSet> vertdata = new Dictionary<int, GCVertexSet>();
				for (int i = 0; i < gcmodel.VertexData.Count; i++)
				{
					vertdata.Add(i, gcmodel.VertexData[i]);
				}
				foreach (KeyValuePair<int, GCVertexSet> verts in vertdata)
				{
					ListViewItem newvert = new ListViewItem(verts.Key.ToString());
					GCVertexSet vx = verts.Value;
					string maintype = "";
					string subtype = "";
					switch (vx.Attribute)
					{
						case GCVertexAttribute.Position:
							maintype += "Position";
							break;
						case GCVertexAttribute.Normal:
							maintype += "Normal";
							break;
						case GCVertexAttribute.Color0:
							maintype += "VColor";
							break;
						case GCVertexAttribute.Tex0:
							maintype += "UV";
							break;
					}
					newvert.SubItems.Add(maintype);
					newvert.SubItems.Add(vx.Data.Count.ToString());
					switch (vx.DataType)
					{
						case GCDataType.Signed16:
							subtype += "S16, ";
							break;
						case GCDataType.Unsigned16:
							subtype += "U16, ";
							break;
						case GCDataType.Signed8:
							subtype += "S8, ";
							break;
						case GCDataType.Unsigned8:
							subtype += "U8, ";
							break;
						case GCDataType.RGB565:
							subtype += "RGB565, ";
							break;
						case GCDataType.RGB8:
							subtype += "RGB8, ";
							break;
						case GCDataType.RGBA8:
							subtype += "RGBA8, ";
							break;
						case GCDataType.Float32:
							subtype += "F32, ";
							break;
					}
					switch (vx.StructType)
					{
						case GCStructType.Position_XY:
							subtype += "XY";
							break;
						case GCStructType.Normal_NBT:
							subtype += "NBT";
							break;
						case GCStructType.Normal_NBT3:
							subtype += "NBT3";
							break;
						case GCStructType.Position_XYZ:
						case GCStructType.Normal_XYZ:
							subtype += "XYZ";
							break;
						case GCStructType.Color_RGB:
							subtype += "RGB";
							break;
						case GCStructType.Color_RGBA:
							subtype += "RGBA";
							break;
						case GCStructType.TexCoord_S:
							subtype += "S";
							break;
						case GCStructType.TexCoord_ST:
							subtype += "ST";
							break;
					}
					newvert.SubItems.Add(subtype);
					listViewVertices.Items.Add(newvert);
				}
				listViewVertices.SelectedIndices.Clear();
				listViewVertices.SelectedItems.Clear();
				listViewVertices_SelectedIndexChanged(null, null);
				listViewVertices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			}
		}
		private void BuildOpaqueMeshList()
		{
			listViewOMeshes.Items.Clear();
			groupBoxOpaque.Enabled = false;
			if (editedModel is GC.GCAttach gcmodel)
			{
				if (gcmodel.OpaqueMeshes.Count > 0)
				{
					groupBoxOpaque.Enabled = true;
					Dictionary<int, GCMesh> meshes = new Dictionary<int, GCMesh>();
					for (int i = 0; i < gcmodel.OpaqueMeshes.Count; i++)
					{
						meshes.Add(i, gcmodel.OpaqueMeshes[i]);
					}
					foreach (KeyValuePair<int, GCMesh> meshdata in meshes)
					{
						ListViewItem gcm = new ListViewItem(meshdata.Key.ToString());
						gcm.SubItems.Add(meshdata.Value.Parameters.Count.ToString());
						gcm.SubItems.Add(meshdata.Value.Primitives.Count.ToString());
						listViewOMeshes.Items.Add(gcm);
					}
				}
			}
			listViewOMeshes.SelectedIndices.Clear();
			listViewOMeshes.SelectedItems.Clear();
			listViewOMeshes_SelectedIndexChanged(null, null);
			listViewOMeshes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}
		private void BuildTranslucentMeshList()
		{
			listViewTMeshes.Items.Clear();
			groupBoxTrans.Enabled = false;
			if (editedModel is GC.GCAttach gcmodel)
			{
				if (gcmodel.TranslucentMeshes.Count > 0)
				{
					groupBoxTrans.Enabled = true;
					Dictionary<int, GCMesh> meshes = new Dictionary<int, GCMesh>();
					for (int i = 0; i < gcmodel.TranslucentMeshes.Count; i++)
					{
						meshes.Add(i, gcmodel.TranslucentMeshes[i]);
					}
					foreach (KeyValuePair<int, GCMesh> meshdata in meshes)
					{
						ListViewItem gcm = new ListViewItem(meshdata.Key.ToString());
						gcm.SubItems.Add(meshdata.Value.Parameters.Count.ToString());
						gcm.SubItems.Add(meshdata.Value.Primitives.Count.ToString());
						listViewTMeshes.Items.Add(gcm);
					}
				}
			}
			listViewTMeshes.SelectedIndices.Clear();
			listViewTMeshes.SelectedItems.Clear();
			listViewTMeshes_SelectedIndexChanged(null, null);
			listViewTMeshes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		private void listViewOMeshes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listViewOMeshes.SelectedIndices.Count == 0)
			{
				buttonCloneOMesh.Enabled = buttonDeleteOMesh.Enabled = buttonMoveOMeshDown.Enabled = buttonMoveOMeshUp.Enabled = false;
				return;
			}
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).OpaqueMeshes[listViewOMeshes.SelectedIndices[0]];
			buttonCloneOMesh.Enabled = true;
			buttonDeleteOMesh.Enabled = ((GC.GCAttach)editedModel).OpaqueMeshes.Count > 0;
			buttonMoveOMeshUp.Enabled = ((GC.GCAttach)editedModel).OpaqueMeshes.IndexOf(selectedMesh) > 0;
			buttonMoveOMeshDown.Enabled = ((GC.GCAttach)editedModel).OpaqueMeshes.IndexOf(selectedMesh) < ((GC.GCAttach)editedModel).OpaqueMeshes.Count - 1;
			openOParameterViewerToolStripMenuItem.Enabled = selectedMesh.Parameters.Count > 0;
			openTParameterViewerToolStripMenuItem.Enabled = false;
			string meshParamAddr = selectedMesh.ParameterName;
			string meshParamCount = selectedMesh.Parameters.Count.ToString();
			string meshPrimName = selectedMesh.PrimitiveName;
			string meshPrimCount = selectedMesh.Primitives.Count.ToString();
			string meshPrimSize = selectedMesh.PrimitiveSize.ToString();
			// Status bar
			StringBuilder sb = new StringBuilder();
			sb.Append("Data: ");
			if (meshParamAddr != null)
			{
				sb.Append(meshParamAddr);
				sb.Append(", Count:");
				sb.Append(meshParamCount);
				sb.Append(", ");
			}
			if (meshPrimName != null)
			{
				sb.Append(meshPrimName);
				sb.Append(", Count:");
				sb.Append(meshPrimCount);
				sb.Append(", Primitive Size (Bytes):");
				sb.Append(meshPrimSize);
			}
			toolStripStatusLabelInfo.Text = sb.ToString();
		}

		private void listViewTMeshes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listViewTMeshes.SelectedIndices.Count == 0)
			{
				buttonCloneTMesh.Enabled = buttonDeleteTMesh.Enabled = buttonMoveTMeshDown.Enabled = buttonMoveTMeshUp.Enabled = false;
				return;
			}
			GCMesh selectedMesh = ((GC.GCAttach)editedModel).TranslucentMeshes[listViewTMeshes.SelectedIndices[0]];
			buttonCloneTMesh.Enabled = true;
			buttonDeleteTMesh.Enabled = ((GC.GCAttach)editedModel).TranslucentMeshes.Count > 0;
			buttonMoveTMeshUp.Enabled = ((GC.GCAttach)editedModel).TranslucentMeshes.IndexOf(selectedMesh) > 0;
			buttonMoveTMeshDown.Enabled = ((GC.GCAttach)editedModel).TranslucentMeshes.IndexOf(selectedMesh) < ((GC.GCAttach)editedModel).TranslucentMeshes.Count - 1;
			openTParameterViewerToolStripMenuItem.Enabled = selectedMesh.Parameters.Count > 0;
			openOParameterViewerToolStripMenuItem.Enabled = false;
			string meshParamAddr = selectedMesh.ParameterName;
			string meshParamCount = selectedMesh.Parameters.Count.ToString();
			string meshPrimName = selectedMesh.PrimitiveName;
			string meshPrimCount = selectedMesh.Primitives.Count.ToString();
			string meshPrimSize = selectedMesh.PrimitiveSize.ToString();
			// Status bar
			StringBuilder sb = new StringBuilder();
			sb.Append("Data: ");
			if (meshParamAddr != null)
			{
				sb.Append(meshParamAddr);
				sb.Append(", Count:");
				sb.Append(meshParamCount);
				sb.Append(", ");
			}
			if (meshPrimName != null)
			{
				sb.Append(meshPrimName);
				sb.Append(", Count:");
				sb.Append(meshPrimCount);
				sb.Append(", Primitive Size (Bytes):");
				sb.Append(meshPrimSize);
			}
			toolStripStatusLabelInfo.Text = sb.ToString();
		}

		private void listViewOMeshes_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && listViewOMeshes.SelectedIndices.Count != 0)
				contextMenuStripParamEdit.Show(listViewOMeshes, e.Location);
		}

		private void listViewTMeshes_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && listViewTMeshes.SelectedIndices.Count != 0)
				contextMenuStripParamEdit.Show(listViewTMeshes, e.Location);
		}

		private void listViewVertices_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && listViewVertices.SelectedIndices.Count != 0)
				contextMenuStripVertData.Show(listViewVertices, e.Location);
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
				if (objs[index].Attach is GCAttach gatt)
				{
					textBoxVertexName.Enabled = true;
					textBoxVertexName.Text = gatt.VertexName;
					textBoxOPolyName.Enabled = true;
					textBoxOPolyName.Text = gatt.OpaqueMeshName;
					textBoxTPolyName.Enabled = true;
					textBoxTPolyName.Text = gatt.TranslucentMeshName;
					//groupBoxMeshList.Enabled = true;
					//BuildMeshsetList();
				}
			}
			else
			{
				textBoxVertexName.Enabled = textBoxOPolyName.Enabled = textBoxTPolyName.Enabled = textBoxModelName.Enabled = textBoxModelRadius.Enabled = textBoxModelX.Enabled = textBoxModelY.Enabled = textBoxModelZ.Enabled = false;
				textBoxModelName.Text = textBoxVertexName.Text = textBoxOPolyName.Text = textBoxTPolyName.Text = textBoxModelName.Text = textBoxModelRadius.Text = textBoxModelX.Text = textBoxModelY.Text = textBoxModelZ.Text = "";
				editedModel = null;
			}
			previousNodeIndex = comboBoxNode.SelectedIndex;
			BuildVertexList();
			BuildOpaqueMeshList();
			BuildTranslucentMeshList();
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			comboBoxNode_SelectedIndexChanged(sender, e);
		}
		#endregion

		private void listViewVertices_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewVertices.SelectedIndices.Count == 0)
			{
				return;
			}
			viewVertexDataToolStripMenuItem.Enabled = true;
			GCVertexSet selectedVerts = ((GC.GCAttach)editedModel).VertexData[listViewVertices.SelectedItems[0].Index];
			string vertsAttr = selectedVerts.Attribute.ToString();
			string vertDataSize = selectedVerts.StructSize.ToString();
			string vertsEntries = selectedVerts.Data.Count.ToString();
			string vertsStructType = selectedVerts.StructType.ToString();
			string vertsDataType = selectedVerts.DataType.ToString();
			string vertsAddr = selectedVerts.DataName;
			string vertsTotalSize = (selectedVerts.Data.Count * selectedVerts.StructSize).ToString();
			StringBuilder sb = new StringBuilder();
			sb.Append("Attributes: ");
			sb.Append(vertsAttr);
			sb.Append(", Size (Bytes):");
			sb.Append(vertDataSize);
			sb.Append(", Entries:");
			sb.Append(vertsEntries);
			sb.Append(", Struct Type:");
			sb.Append(vertsStructType);
			sb.Append(", Data Type:");
			sb.Append(vertsDataType);
			sb.Append(", ");
			sb.Append(vertsAddr);
			sb.Append(", Total Size (Bytes):");
			sb.Append(vertsTotalSize);
			toolStripStatusLabelInfo.Text = sb.ToString();
		}

		private void openOParameterViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GCMesh oMeshData = ((GC.GCAttach)editedModel).OpaqueMeshes[listViewOMeshes.SelectedIndices[0]];

			using (GCModelParameterDataEditor de = new GCModelParameterDataEditor(oMeshData, textures))
			{
				de.ShowDialog(this);
			}
		}

		private void openTParameterViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GCMesh tMeshData = ((GC.GCAttach)editedModel).TranslucentMeshes[listViewTMeshes.SelectedIndices[0]];
			using (GCModelParameterDataEditor de = new GCModelParameterDataEditor(tMeshData, textures))
			{
				de.ShowDialog(this);
			}
		}

		private void viewVertexDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int vertID = int.Parse(listViewVertices.SelectedItems[0].SubItems[0].Text);
			List<GCVertexSet> selectedObj = ((GC.GCAttach)editedModel).VertexData;
			GCVertexSet selectedVert = selectedObj[listViewVertices.SelectedIndices[0]];
			using (GCModelVertexDataEditor vde = new GCModelVertexDataEditor(selectedVert))
			{
				vde.ShowDialog(this);
			}
		}

		private void MshData_DoubleClick(object sender, EventArgs e)
		{
			if (listViewOMeshes.SelectedItems.Count > 0)
			{
				GCMesh tMeshData = ((GC.GCAttach)editedModel).OpaqueMeshes[listViewOMeshes.SelectedIndices[0]];
				using (GCModelParameterDataEditor de = new GCModelParameterDataEditor(tMeshData, textures))
				{
					de.ShowDialog(this);
				}
			}
		}
	}
}