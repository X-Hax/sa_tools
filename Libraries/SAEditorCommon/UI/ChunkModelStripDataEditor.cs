﻿using System;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon.UI
{
	public partial class ChunkModelStripDataEditor : Form
	{
		#region Events

		public delegate void FormUpdatedHandler(object sender, EventArgs e);
		public event FormUpdatedHandler FormUpdated;

		#endregion

		private PolyChunk PolyData; // Poly data that is being edited
		private readonly PolyChunk PolyDataOriginal; // Original poly data at the time of opening the dialog
		private PolyChunkStrip.Strip PolyStrip;
		private PolyChunkStrip.Strip PolyStripOriginal;
		private PolyChunkStrip.Strip[] PolyStripCollection;
		private readonly PolyChunkStrip.Strip[] PolyStripCollectionOriginal;
		public ChunkModelStripDataEditor(PolyChunk mats, string matsName = null, int index = 0)
		{
			PolyData = mats;
			PolyDataOriginal = mats.Clone();
			PolyStripCollectionOriginal = ((PolyChunkStrip)PolyDataOriginal).Strips.ToArray();
			InitializeComponent();
			stripSetComboBox.Items.Clear();
			if (!string.IsNullOrEmpty(matsName))
				this.Text = "Strip Data Editor: " + matsName;
			BuildStripSetList(mats);
			stripSetComboBox.SelectedIndex = index;
			BuildUVDataList();
			SetStripUserFlags();
		}
		private void BuildStripSetList(PolyChunk poly)
		{
			stripSetComboBox.Items.Clear();
			PolyChunkStrip pcs = (PolyChunkStrip)poly;
			PolyStripCollection = pcs.Strips.ToArray();
			string stripwinding = "";
			string stripcount = "";
			for (int i = 0; i < PolyStripCollection.Length; i++)
			{
				stripwinding = "Strip" + (PolyStripCollection[i].Reversed ? "R(" : "L(");
				stripcount = PolyStripCollection[i].Indexes.Length.ToString() + ")";
				stripSetComboBox.Items.Add(i.ToString() + ": " + stripwinding + stripcount);
			}
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
			PolyChunkStrip pcs = (PolyChunkStrip)poly;

			// Setting flags
			ignoreAmbiCheck.Checked = pcs.IgnoreAmbient;
			ignoreSpecCheck.Checked = pcs.IgnoreSpecular;
			useAlphaCheck.Checked = pcs.UseAlpha;
			envMapCheck.Checked = pcs.EnvironmentMapping;
			doubleSideCheck.Checked = pcs.DoubleSide;
			flatShadeCheck.Checked = pcs.FlatShading;
			ignoreLightCheck.Checked = pcs.IgnoreLight;
			userFlagsNumericUpDown.Value = pcs.UserFlags;
			noAlphaTestCheck.Checked = pcs.NoAlphaTest;
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
		private void BuildUVDataList()
		{
			stripListView.Items.Clear();
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			bool hasUVs = false;
			bool hasUV2s = false;
			bool hasColor = false;
			switch (pcs.Type)
			{
				case ChunkType.Strip_Strip:
				case ChunkType.Strip_Strip2:
				default:
					break;
				case ChunkType.Strip_StripColor:
					hasColor = true;
					break;
				case ChunkType.Strip_StripUVN:
				case ChunkType.Strip_StripUVH:
				case ChunkType.Strip_StripUVN2:
				case ChunkType.Strip_StripUVH2:
				case ChunkType.Strip_StripUVNNormal:
				case ChunkType.Strip_StripUVHNormal:
				case ChunkType.Strip_StripUVNColor:
				case ChunkType.Strip_StripUVHColor:
					hasUVs = true;
					switch (pcs.Type)
					{
						default:
							break;
						case ChunkType.Strip_StripUVN2:
						case ChunkType.Strip_StripUVH2:
							hasUV2s = true;
							break;
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVHColor:
							hasColor = true;
							break;
					}
					break;
			}
			for (int i = 0; i < PolyStrip.Indexes.Length; i++)
			{
				ListViewItem newstrip = new ListViewItem(i.ToString());
				newstrip.SubItems.Add(PolyStrip.Indexes[i].ToString());
				if (hasUVs)
					newstrip.SubItems.Add(PolyStrip.UVs[i].ToStruct());
				else
					newstrip.SubItems.Add("N/A");
				if (hasUV2s)
					newstrip.SubItems.Add(PolyStrip.UVs2[i].ToStruct());
				else
					newstrip.SubItems.Add("N/A");
				if (hasColor)
					newstrip.SubItems.Add(PolyStrip.VColors[i].ToStruct());
				else
					newstrip.SubItems.Add("N/A");
				stripListView.Items.Add(newstrip);
			}
			stripListView.SelectedIndices.Clear();
			stripListView.SelectedItems.Clear();
			//stripListView_SelectedIndexChanged(null, null);
			stripListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}
		private void SetStripUserFlags()
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			userFlag1NumericUpDown.Enabled = false;
			userFlag2NumericUpDown.Enabled = false;
			userFlag3NumericUpDown.Enabled = false;
			if (pcs.UserFlags > 0)
			{
				userFlag1NumericUpDown.Enabled = true;
				userFlag2NumericUpDown.Enabled = true;
				userFlag3NumericUpDown.Enabled = true;
			}
		}
		#endregion
		#region General Control Event Methods

		private void doneButton_Click(object sender, EventArgs e)
		{
			checkSettingsOnClose();
			Close();
		}

		#endregion
		#region Flag Check Event Methods

		//Because resetting sometimes applies incorrect changes
		private void checkSettingsOnClose()
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.UseAlpha = useAlphaCheck.Checked;
			pcs.EnvironmentMapping = envMapCheck.Checked;
			pcs.DoubleSide = doubleSideCheck.Checked;
			pcs.FlatShading = flatShadeCheck.Checked;
			pcs.IgnoreLight = ignoreLightCheck.Checked;
			pcs.IgnoreAmbient = ignoreAmbiCheck.Checked;
			pcs.IgnoreSpecular = ignoreSpecCheck.Checked;
			pcs.NoAlphaTest = noAlphaTestCheck.Checked;
		}

		private void ignoreAmbiCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.IgnoreAmbient = ignoreAmbiCheck.Checked;
			RaiseFormUpdated();
		}

		private void noAlphaTestCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.NoAlphaTest = noAlphaTestCheck.Checked;
			RaiseFormUpdated();
		}

		private void ignoreSpecCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.IgnoreSpecular = ignoreSpecCheck.Checked;
			RaiseFormUpdated();
		}

		private void useAlphaCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.UseAlpha = useAlphaCheck.Checked;
			RaiseFormUpdated();
		}

		private void useTextureCheck_Click(object sender, EventArgs e)
		{
			//materials[comboMaterial.SelectedIndex].UseTexture = useTextureCheck.Checked;
			RaiseFormUpdated();
		}

		private void envMapCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.EnvironmentMapping = envMapCheck.Checked;
			RaiseFormUpdated();
		}

		private void doubleSideCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.DoubleSide = doubleSideCheck.Checked;
			RaiseFormUpdated();
		}

		private void flatShadeCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.FlatShading = flatShadeCheck.Checked;
			RaiseFormUpdated();
		}

		private void ignoreLightCheck_Click(object sender, EventArgs e)
		{
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			pcs.IgnoreLight = ignoreLightCheck.Checked;
			RaiseFormUpdated();
		}

		private void userFlagsNumeric_ValueChanged(object sender, EventArgs e)
		{
			//materials[comboMaterial.SelectedIndex].UserFlags = (byte)userFlagsNumeric.Value;
			RaiseFormUpdated();
		}

		#endregion
		#region Material List Editing

		private void resetButton_Click(object sender, EventArgs e)
		{
			SetControls(PolyDataOriginal);
			BuildStripSetList(PolyDataOriginal);
			stripSetComboBox.SelectedIndex = 0;
			RaiseFormUpdated();
		}
		#endregion

		private void stripSetComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = stripSetComboBox.SelectedIndex;
			if (index == -1)
				return;
			PolyChunkStrip pcs = (PolyChunkStrip)PolyData;
			PolyChunkStrip.Strip[] strips = pcs.Strips.ToArray();
			PolyStrip = strips[index].Clone();
			PolyStripOriginal = strips[index].Clone();
			BuildUVDataList();
			SetStripUserFlags();
		}

		private void flipAllWindingsButton_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < PolyStripCollection.Length; i++)
				PolyStripCollection[i].CMDEReversed = !PolyStripCollection[i].Reversed;
			BuildStripSetList(PolyData);
			stripSetComboBox.SelectedIndex = 0;
		}

		private void resetAllWindingsButton_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < PolyStripCollectionOriginal.Length; i++)
				PolyStripCollection[i].CMDEReversed = PolyStripCollectionOriginal[i].Reversed;
			BuildStripSetList(PolyData);
			stripSetComboBox.SelectedIndex = 0;
		}

		private void flipWindingButton_Click(object sender, EventArgs e)
		{
			int index = stripSetComboBox.SelectedIndex;
			PolyStripCollection[index].CMDEReversed = !PolyStripCollection[index].Reversed;
			BuildStripSetList(PolyData);
			stripSetComboBox.SelectedIndex = index;
		}

		private void resetWindingButton_Click(object sender, EventArgs e)
		{
			int index = stripSetComboBox.SelectedIndex;
			PolyStripCollection[index].CMDEReversed = PolyStripCollectionOriginal[index].Reversed;
			BuildStripSetList(PolyData);
			stripSetComboBox.SelectedIndex = index;
		}
	}
}