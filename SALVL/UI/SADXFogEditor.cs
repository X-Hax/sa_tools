using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAModel.SALVL
{
	public partial class SADXFogEditor : Form
	{
		List<SplitTools.FogDataArray> FogTables;
		MainForm EditorMainForm;
		bool UnsavedChanges;
		bool Suspend;
		int lastAct;

		public SADXFogEditor(MainForm form, SplitTools.FogDataTable table, int actID)
		{
			EditorMainForm = form;
			FogTables = new List<SplitTools.FogDataArray>();
			for (int i = 0; i < table.Act.Length; i++)
				FogTables.Add(table.Act[i]);
			Suspend = true;
			InitializeComponent();
			numericUpDownAct.Value = lastAct = actID;
			LoadValues();
		}

		private void SADXFogEditor_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start " + "https://github.com/X-Hax/sa_tools/wiki/SALVL#fog-editor") { CreateNoWindow = false });
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			ApplyChanges();
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			if (UnsavedChanges)
				ApplyChangesDialog();
			Close();
		}

		private void pictureBoxNearColor_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxNearColor.BackColor);
			numericUpDownNearR.Value = newColor.R;
			numericUpDownNearG.Value = newColor.G;
			numericUpDownNearB.Value = newColor.B;
			SetFogColorPreview();
		}

		private void pictureBoxMediumColor_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxMediumColor.BackColor);
			numericUpDownMediumR.Value = newColor.R;
			numericUpDownMediumG.Value = newColor.G;
			numericUpDownMediumB.Value = newColor.B;
			SetFogColorPreview();
		}

		private void pictureBoxFarColor_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxFarColor.BackColor);
			numericUpDownFarR.Value = newColor.R;
			numericUpDownFarG.Value = newColor.G;
			numericUpDownFarB.Value = newColor.B;
			SetFogColorPreview();
		}

		private void formValuesChanged(object sender, EventArgs e)
		{
			if (!Suspend)
				UnsavedChanges = true;
			SetFogColorPreview();
		}

		private void buttonNew_Click(object sender, EventArgs e)
		{
			if (FogTables.Count < numericUpDownAct.Value + 1)
				do
					FogTables.Add(new SplitTools.FogDataArray());
				while (FogTables.Count < numericUpDownAct.Value + 1);
			FogTables[(int)numericUpDownAct.Value] = new SplitTools.FogDataArray();
			LoadValues();
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (FogTables.Count - 1 < numericUpDownAct.Value)
				return;
			FogTables.RemoveAt((int)numericUpDownAct.Value);
			LoadValues();
		}

		private void numericUpDownAct_ValueChanged(object sender, EventArgs e)
		{
			Suspend = true;
			if (UnsavedChanges)
				ApplyChangesDialog();
			LoadValues();
			Suspend = false;
			lastAct = (int)numericUpDownAct.Value;
		}

		private void ApplyChangesDialog()
		{
			DialogResult result = MessageBox.Show(this, "Apply changes?", "SALVL Fog Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			switch (result)
			{
				case DialogResult.Yes:
					ApplyChanges();
					break;
				case DialogResult.No:
				default:
					UnsavedChanges = false;
					break;
			}
		}

		private void SetFogColorPreview()
		{
			if (numericUpDownAct.Value > FogTables.Count - 1)
				return;
			pictureBoxFarColor.BackColor = Color.FromArgb((int)numericUpDownFarR.Value, (int)numericUpDownFarG.Value, (int)numericUpDownFarB.Value);
			pictureBoxMediumColor.BackColor = Color.FromArgb((int)numericUpDownMediumR.Value, (int)numericUpDownMediumG.Value, (int)numericUpDownMediumB.Value);
			pictureBoxNearColor.BackColor = Color.FromArgb((int)numericUpDownNearR.Value, (int)numericUpDownNearG.Value, (int)numericUpDownNearB.Value);
		}

		private void LoadValues()
		{
			buttonDelete.Enabled = FogTables.Count == (int)numericUpDownAct.Value + 1;
			if (numericUpDownAct.Value > FogTables.Count - 1)
				groupBoxFar.Enabled = groupBoxMedium.Enabled = groupBoxNear.Enabled = false;
			else
			{
				groupBoxFar.Enabled = groupBoxMedium.Enabled = groupBoxNear.Enabled = true;
				// High
				numericUpDownFarA.Value = FogTables[(int)numericUpDownAct.Value].ColorA_High;
				numericUpDownFarR.Value = FogTables[(int)numericUpDownAct.Value].ColorR_High;
				numericUpDownFarG.Value = FogTables[(int)numericUpDownAct.Value].ColorG_High;
				numericUpDownFarB.Value = FogTables[(int)numericUpDownAct.Value].ColorB_High;
				checkBoxFarFogEnabled.Checked = FogTables[(int)numericUpDownAct.Value].FogEnabledHigh != 0;
				numericUpDownFarFogStart.Value = (int)FogTables[(int)numericUpDownAct.Value].FogStartHigh;
				numericUpDownFarFogEnd.Value = (int)FogTables[(int)numericUpDownAct.Value].FogEndHigh;
				// Medium
				numericUpDownMediumA.Value = FogTables[(int)numericUpDownAct.Value].ColorA_Medium;
				numericUpDownMediumR.Value = FogTables[(int)numericUpDownAct.Value].ColorR_Medium;
				numericUpDownMediumG.Value = FogTables[(int)numericUpDownAct.Value].ColorG_Medium;
				numericUpDownMediumB.Value = FogTables[(int)numericUpDownAct.Value].ColorB_Medium;
				checkBoxMediumFogEnabled.Checked = FogTables[(int)numericUpDownAct.Value].FogEnabledMedium != 0;
				numericUpDownMediumFogStart.Value = (int)FogTables[(int)numericUpDownAct.Value].FogStartMedium;
				numericUpDownMediumFogEnd.Value = (int)FogTables[(int)numericUpDownAct.Value].FogEndMedium;
				// Low
				numericUpDownNearA.Value = FogTables[(int)numericUpDownAct.Value].ColorA_Low;
				numericUpDownNearR.Value = FogTables[(int)numericUpDownAct.Value].ColorR_Low;
				numericUpDownNearG.Value = FogTables[(int)numericUpDownAct.Value].ColorG_Low;
				numericUpDownNearB.Value = FogTables[(int)numericUpDownAct.Value].ColorB_Low;
				checkBoxNearFogEnabled.Checked = FogTables[(int)numericUpDownAct.Value].FogEnabledLow != 0;
				numericUpDownNearFogStart.Value = (int)FogTables[(int)numericUpDownAct.Value].FogStartLow;
				numericUpDownNearFogEnd.Value = (int)FogTables[(int)numericUpDownAct.Value].FogEndLow;
			}
			SetFogColorPreview();
			Suspend = false;
		}

		private void ApplyChanges()
		{
			// High
			FogTables[lastAct].ColorA_High = (byte)numericUpDownFarA.Value;
			FogTables[lastAct].ColorR_High = (byte)numericUpDownFarR.Value;
			FogTables[lastAct].ColorG_High = (byte)numericUpDownFarG.Value;
			FogTables[lastAct].ColorB_High = (byte)numericUpDownFarB.Value;
			FogTables[lastAct].FogEnabledHigh = checkBoxFarFogEnabled.Checked ? 1 : 0;
			FogTables[lastAct].FogStartHigh = (int)numericUpDownFarFogStart.Value;
			FogTables[lastAct].FogEndHigh = (int)numericUpDownFarFogEnd.Value;
			// Medium
			FogTables[lastAct].ColorA_Medium = (byte)numericUpDownMediumA.Value;
			FogTables[lastAct].ColorR_Medium = (byte)numericUpDownMediumR.Value;
			FogTables[lastAct].ColorG_Medium = (byte)numericUpDownMediumG.Value;
			FogTables[lastAct].ColorB_Medium = (byte)numericUpDownMediumB.Value;
			FogTables[lastAct].FogEnabledMedium = checkBoxMediumFogEnabled.Checked ? 1 : 0;
			FogTables[lastAct].FogStartMedium = (int)numericUpDownMediumFogStart.Value;
			FogTables[lastAct].FogEndMedium = (int)numericUpDownMediumFogEnd.Value;
			// Low
			FogTables[lastAct].ColorA_Low = (byte)numericUpDownNearA.Value;
			FogTables[lastAct].ColorR_Low = (byte)numericUpDownNearR.Value;
			FogTables[lastAct].ColorG_Low = (byte)numericUpDownNearG.Value;
			FogTables[lastAct].ColorB_Low = (byte)numericUpDownNearB.Value;
			FogTables[lastAct].FogEnabledLow = checkBoxNearFogEnabled.Checked ? 1 : 0;
			FogTables[lastAct].FogStartLow = (int)numericUpDownNearFogStart.Value;
			FogTables[lastAct].FogEndLow = (int)numericUpDownNearFogEnd.Value;
			UnsavedChanges = false;
			EditorMainForm.GetEditedFog(FogTables.ToArray());
		}

		private Color GetColorFromDialog(Color original)
		{
			using (ColorDialog dialog = new ColorDialog { Color = original, AllowFullOpen = true, FullOpen = true })
			{
				dialog.ShowDialog();
				if (dialog.Color != original)
					UnsavedChanges = true;
				return dialog.Color;
			}
		}
	}
}