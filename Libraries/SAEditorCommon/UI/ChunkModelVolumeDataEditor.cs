using System;
using System.Windows.Forms;
using static SAModel.PolyChunkVolume;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ChunkModelVolumeDataEditor : Form
	{
		#region Events

		public delegate void FormUpdatedHandler(object sender, EventArgs e);
		public event FormUpdatedHandler FormUpdated;

		#endregion

		private PolyChunk PolyData; // Poly data that is being edited
		private readonly PolyChunk PolyDataOriginal; // Original poly data at the time of opening the dialog

		public ChunkModelVolumeDataEditor(PolyChunk mats, string matsName = null)
		{
			PolyData = mats;
			PolyDataOriginal = mats.Clone();
			InitializeComponent();
			BuildVolumeDataList();
			SetControls(mats);
			if (!string.IsNullOrEmpty(matsName))
				this.Text = "Modifier Volume Editor: " + matsName;
		}

		private void MaterialEditor_Load(object sender, EventArgs e)
		{
			SetControls(PolyData);
		}

		#region UI Updates

		/// <summary>
		/// Populates the form with data from a material index.
		/// </summary>
		/// <param name="index">Index of the material to use.</param>
		private void SetControls(PolyChunk poly)
		{
			// Setting general
			PolyChunkVolume pcv = (PolyChunkVolume)poly;
			switch (pcv.Type)
			{
				case ChunkType.Volume_Polygon3:
					volumeTypeLabel.Text = "Triangle";
					break;
				case ChunkType.Volume_Polygon4:
					volumeTypeLabel.Text = "Quad";
					break;
				case ChunkType.Volume_Strip:
					volumeTypeLabel.Text = "Strip";
					break;
				default:
					volumeTypeLabel.Text = $"Invalid Type {pcv.Type}";
					break;
		}
			//DisplayFlags(index);

			// Material list controls
			resetButton.Enabled = true;
		}

		private void RaiseFormUpdated()
		{
			FormUpdated?.Invoke(this, EventArgs.Empty);
		}

		//private void DisplayFlags(PolyChunk)
		//{
		//	labelFlags.Text = String.Format("{0:X8}", materials[index].Flags);
		//}

		#endregion
		#region General Control Event Methods

		private void doneButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void BuildVolumeDataList()
		{
			listViewVolumeData.Items.Clear();
			PolyChunkVolume pcv = (PolyChunkVolume)PolyData;
			for (int i = 0; i < pcv.PolyCount; i++)
			{
				ListViewItem newvolume = new ListViewItem(i.ToString());
				switch (pcv.Type)
				{
					case ChunkType.Volume_Polygon3:
					case ChunkType.Volume_Polygon4:
						var point = string.Empty;
						var ind = pcv.Polys[i].Indexes;
						for (int t = 0; t < ind.Length; t++)
						{
							point += ind[t].ToString() + ", ";
						}
						point = point.Remove(point.Length - 2);
						newvolume.SubItems.Add(point);
						break;
					case ChunkType.Volume_Strip:
						var str = "StripL( ";
						PolyChunkVolume.Strip sdata = (PolyChunkVolume.Strip)pcv.Polys[i];
						if (sdata.Reversed)
							str = "StripR( ";
						var stripinds = pcv.Polys[i].Indexes;
						for (int s = 0; s < stripinds.Length; s++)
						{
							str += stripinds[s].ToString() + ", ";
						}
						str = str.Remove(str.Length - 2);
						str += " )";
						newvolume.SubItems.Add(str);
						break;
					default:
						break;
				}
				listViewVolumeData.Items.Add(newvolume);
			}
				listViewVolumeData.SelectedIndices.Clear();
				listViewVolumeData.SelectedItems.Clear();
				//listViewVolumeData_SelectedIndexChanged(null, null);
				listViewVolumeData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		#endregion
		#region Flag Check Event Methods


		#endregion
		#region Material List Editing

		private void resetButton_Click(object sender, EventArgs e)
		{
			SetControls(PolyDataOriginal);
			RaiseFormUpdated();
		}
		#endregion
	}
}