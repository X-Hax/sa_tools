using SplitTools;
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
	public partial class SADXLightsEditor : Form
	{
		List<SADXStageLightData> StageLights;
		List<LSPaletteData> CharLights;
		SADXStageLightData CurrentStageLight;
		LSPaletteData CurrentCharLight;
		MainForm EditorMainForm;
		bool Suspend;
		bool UnsavedChanges;

		public SADXLightsEditor(MainForm form, List<SADXStageLightData> stageLights, List<LSPaletteData> charLights, SA1LevelIDs levelID, int actID)
		{
			InitializeComponent();
			Suspend = true;
			EditorMainForm = form;
			StageLights = new List<SADXStageLightData>(stageLights.Count);
			for (int i = 0; i < stageLights.Count; i++)
				StageLights.Add(stageLights[i]);
			CharLights = new List<LSPaletteData>(charLights.Count);
			for (int i = 0; i < charLights.Count; i++)
				CharLights.Add(charLights[i]);
			comboBoxLevel.SelectedIndex = (int)levelID;
			numericUpDownAct.Value = actID;
			labelLightType.Enabled = comboBoxLightType.Enabled = false;
			tabControl1.SelectedIndex = 0;
			comboBoxLightType.SelectedIndex = 0;
			Suspend = false;
			SetCurrentLights();
			FormClosing += new FormClosingEventHandler(EditorClosing);
		}

		private void SetCurrentLights()
		{
			CurrentStageLight = FindStageLight(comboBoxLevel.SelectedIndex, (int)numericUpDownAct.Value, (int)numericUpDownStageLightIndex.Value);
			CurrentCharLight = FindCharacterLight(comboBoxLevel.SelectedIndex, (int)numericUpDownAct.Value, comboBoxLightType.SelectedIndex);
			RefreshUIValues();
		}

		private void AddStageLight(int level, int act, int index)
		{
			StageLights.Add(new SADXStageLightData
			{
				Level = (SA1LevelIDs)level,
				Act = (byte)act,
				LightNum = (byte)index,
				UseDirection = true,
				Direction = new Vertex(0.866f, -0.6f, 0),
				Specular = 0.8f,
				Diffuse = 1.0f,
				RGB = new Vertex(1, 1, 1),
				AmbientRGB = new Vertex(0.5f, 0.5f, 0.5f)
			});
		}

		private void AddCharacterLight(int level, int act, int type)
		{
			LSPaletteData.LSPaletteTypes lsType;
			switch (type)
			{
				case 0:
				default:
					lsType = LSPaletteData.LSPaletteTypes.Character;
					break;
				case 1:
					lsType = LSPaletteData.LSPaletteTypes.CharacterAlt;
					break;
				case 2:
					lsType = LSPaletteData.LSPaletteTypes.Boss;
					break;
			}
			CharLights.Add(new LSPaletteData
			{
				Level = (SA1LevelIDs)level,
				Act = (byte)act,
				Type = lsType,
				Flags = LSPaletteData.LSPaletteFlags.Enabled,
				Direction = new Vertex(0, -1, 0),
				Diffuse = 1.0f,
				Ambient = new Vertex(0.7f, 0.7f, 0.7f),
				Color1Power = 20.0f,
				Color1 = new Vertex(1, 1, 1),
				Specular1Power = 15.0f,
				Specular1 = new Vertex(0.667f, 0.667f, 0.667f),
				Color2 = new Vertex(0, 0, 0),
				Color2Power = 0,
				Specular2Power = 0,
				Specular2 = new Vertex(0, 0, 0)
			});
		}

		private void RemoveStageLight(int level, int act, int index)
		{
			SADXStageLightData light = FindStageLight(level, act, index);
			if (light != null)
				StageLights.Remove(light);
		}

		private void RemoveCharLight(int level, int act, int typeUI)
		{
			LSPaletteData light = FindCharacterLight(level, act, typeUI);
			if (light != null)
				CharLights.Remove(light);
		}

		private SADXStageLightData FindStageLight(int levelID, int actID, int lightID)
		{
			foreach (SADXStageLightData light in StageLights)
				if (light.Level == (SA1LevelIDs)levelID && light.Act == actID && light.LightNum == lightID)
					return light;
			return null;
		}

		private LSPaletteData FindCharacterLight(int levelID, int actID, int lightTypeUI)
		{
			LSPaletteData.LSPaletteTypes lsPaletteType;
			switch (lightTypeUI)
			{
				case 0:
				default:
					lsPaletteType = LSPaletteData.LSPaletteTypes.Character;
					break;
				case 1:
					lsPaletteType = LSPaletteData.LSPaletteTypes.CharacterAlt;
					break;
				case 2:
					lsPaletteType = LSPaletteData.LSPaletteTypes.Boss;
					break;
			}
			foreach (LSPaletteData light in CharLights)
				if (light.Level == (SA1LevelIDs)levelID && light.Act == actID && light.Type == lsPaletteType)
					return light;
			return null;
		}

		private void RefreshUIValues()
		{
			Suspend = true;
			// Stage Light
			if (CurrentStageLight != null)
			{
				groupBoxStageAmbient.Enabled = groupBoxStageDiffuse.Enabled = groupBoxStageGeneral.Enabled = true;
				checkBoxStageUseDir.Checked = CurrentStageLight.UseDirection;
				trackBarStageDiffuse.Value = (int)(CurrentStageLight.Diffuse * 1000);
				trackBarStageSpecular.Value = (int)(CurrentStageLight.Specular * 1000);
				trackBarStageX.Value = (int)(CurrentStageLight.Direction.X * 1000);
				trackBarStageY.Value = (int)(CurrentStageLight.Direction.Y * 1000);
				trackBarStageZ.Value = (int)(CurrentStageLight.Direction.Z * 1000);
				numericUpDownStageAmbientR.Value = (int)(Math.Min(1.0f, CurrentStageLight.AmbientRGB.X) * 255);
				numericUpDownStageAmbientG.Value = (int)(Math.Min(1.0f, CurrentStageLight.AmbientRGB.Y) * 255);
				numericUpDownStageAmbientB.Value = (int)(Math.Min(1.0f, CurrentStageLight.AmbientRGB.Z) * 255);
				numericUpDownStageDifR.Value = (int)(Math.Min(1.0f, CurrentStageLight.RGB.X) * 255);
				numericUpDownStageDifG.Value = (int)(Math.Min(1.0f, CurrentStageLight.RGB.Y) * 255);
				numericUpDownStageDifB.Value = (int)(Math.Min(1.0f, CurrentStageLight.RGB.Z) * 255);
			}
			else
				groupBoxStageAmbient.Enabled = groupBoxStageDiffuse.Enabled = groupBoxStageGeneral.Enabled = false;
			// Character Light
			if (CurrentCharLight != null)
			{
				groupBoxCharAmbient.Enabled = groupBoxCharCO1.Enabled = groupBoxCharCO2.Enabled = groupBoxCharGeneral.Enabled = groupBoxCharSpecular1.Enabled = groupBoxCharSpecular2.Enabled = true;
				checkBoxCharEnabled.Checked = CurrentCharLight.Flags.HasFlag(LSPaletteData.LSPaletteFlags.Enabled);
				checkBoxCharUseDir.Checked = CurrentCharLight.Flags.HasFlag(LSPaletteData.LSPaletteFlags.UseLSLightDirection);
				checkBoxCharIgnoreDir.Checked = CurrentCharLight.Flags.HasFlag(LSPaletteData.LSPaletteFlags.IgnoreDirection);
				checkBoxCharDouble.Checked = CurrentCharLight.Flags.HasFlag(LSPaletteData.LSPaletteFlags.OverrideLastLight);
				checkBoxCharUnknown.Checked = CurrentCharLight.Flags.HasFlag(LSPaletteData.LSPaletteFlags.Unknown);
				trackBarCharDiffuse.Value = (int)(CurrentCharLight.Diffuse * 1000);
				// Light Direction
				trackBarCharX.Value = (int)(CurrentCharLight.Direction.X * 1000);
				trackBarCharY.Value = (int)(CurrentCharLight.Direction.Y * 1000);
				trackBarCharZ.Value = (int)(CurrentCharLight.Direction.Z * 1000);
				// Ambient
				numericUpDownCharAmbientR.Value = (int)(Math.Min(1.0f, CurrentCharLight.Ambient.X) * 255);
				numericUpDownCharAmbientG.Value = (int)(Math.Min(1.0f, CurrentCharLight.Ambient.Y) * 255);
				numericUpDownCharAmbientB.Value = (int)(Math.Min(1.0f, CurrentCharLight.Ambient.Z) * 255);
				// CO1
				numericUpDownCharCO1R.Value = (int)(Math.Min(1.0f, CurrentCharLight.Color1.X) * 255);
				numericUpDownCharCO1G.Value = (int)(Math.Min(1.0f, CurrentCharLight.Color1.Y) * 255);
				numericUpDownCharCO1B.Value = (int)(Math.Min(1.0f, CurrentCharLight.Color1.Z) * 255);
				trackBarCharCO1Pow.Value = (int)(CurrentCharLight.Color1Power * 100);
				// CO2
				numericUpDownCharCO2R.Value = (int)(Math.Min(1.0f, CurrentCharLight.Color2.X) * 255);
				numericUpDownCharCO2G.Value = (int)(Math.Min(1.0f, CurrentCharLight.Color2.Y) * 255);
				numericUpDownCharCO2B.Value = (int)(Math.Min(1.0f, CurrentCharLight.Color2.Z) * 255);
				trackBarCharCO2Pow.Value = (int)(CurrentCharLight.Color2Power * 100);
				// SP1
				numericUpDownCharSP1R.Value = (int)(Math.Min(1.0f, CurrentCharLight.Specular1.X) * 255);
				numericUpDownCharSP1G.Value = (int)(Math.Min(1.0f, CurrentCharLight.Specular1.Y) * 255);
				numericUpDownCharSP1B.Value = (int)(Math.Min(1.0f, CurrentCharLight.Specular1.Z) * 255);
				trackBarCharSP1Pow.Value = (int)(CurrentCharLight.Specular1Power * 100);
				// SP2
				numericUpDownCharSP2R.Value = (int)(Math.Min(1.0f, CurrentCharLight.Specular2.X) * 255);
				numericUpDownCharSP2G.Value = (int)(Math.Min(1.0f, CurrentCharLight.Specular2.Y) * 255);
				numericUpDownCharSP2B.Value = (int)(Math.Min(1.0f, CurrentCharLight.Specular2.Z) * 255);
				trackBarCharSP2Pow.Value = (int)(CurrentCharLight.Specular2Power * 100);
			}
			else
				groupBoxCharAmbient.Enabled = groupBoxCharCO1.Enabled = groupBoxCharCO2.Enabled = groupBoxCharGeneral.Enabled = groupBoxCharSpecular1.Enabled = groupBoxCharSpecular2.Enabled = false;
			Suspend = false;
		}

		private void trackBarStageDiffuse_ValueChanged(object sender, EventArgs e)
		{
			labelStageDiffuse.Text = ((float)trackBarStageDiffuse.Value / 1000.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarStageSpecular_ValueChanged(object sender, EventArgs e)
		{
			labelStageSpecular.Text = ((float)trackBarStageSpecular.Value / 1000.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarStageX_ValueChanged(object sender, EventArgs e)
		{
			labelStageX.Text = ((float)trackBarStageX.Value / 1000.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarStageY_ValueChanged(object sender, EventArgs e)
		{
			labelStageY.Text = ((float)trackBarStageY.Value / 1000.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarStageZ_ValueChanged(object sender, EventArgs e)
		{
			labelStageZ.Text = ((float)trackBarStageZ.Value / 1000.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void numericUpDownStageDif_ValueChanged(object sender, EventArgs e)
		{
			labelStageDifR.Text = ((float)numericUpDownStageDifR.Value / 255.0f).ToString("0.000");
			labelStageDifG.Text = ((float)numericUpDownStageDifG.Value / 255.0f).ToString("0.000");
			labelStageDifB.Text = ((float)numericUpDownStageDifB.Value / 255.0f).ToString("0.000");
			pictureBoxStageDiffuse.BackColor = Color.FromArgb((int)numericUpDownStageDifR.Value, (int)numericUpDownStageDifG.Value, (int)numericUpDownStageDifB.Value);
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void numericUpDownStageAmbient_ValueChanged(object sender, EventArgs e)
		{
			labelStageAmbientR.Text = ((float)numericUpDownStageAmbientR.Value / 255.0f).ToString("0.000");
			labelStageAmbientG.Text = ((float)numericUpDownStageAmbientG.Value / 255.0f).ToString("0.000");
			labelStageAmbientB.Text = ((float)numericUpDownStageAmbientB.Value / 255.0f).ToString("0.000");
			pictureBoxStageAmbient.BackColor = Color.FromArgb((int)numericUpDownStageAmbientR.Value, (int)numericUpDownStageAmbientG.Value, (int)numericUpDownStageAmbientB.Value);
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			labelLightType.Enabled = comboBoxLightType.Enabled = (tabControl1.SelectedTab == tabPageCharacterLights);
		}

		private void trackBarCharDiffuse_ValueChanged(object sender, EventArgs e)
		{
			labelCharDiffuse.Text = ((float)trackBarCharDiffuse.Value / 1000.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarCharX_ValueChanged(object sender, EventArgs e)
		{
			labelCharX.Text = ((float)trackBarCharX.Value / 1000.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarCharY_ValueChanged(object sender, EventArgs e)
		{
			labelCharY.Text = ((float)trackBarCharY.Value / 1000.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarCharZ_ValueChanged(object sender, EventArgs e)
		{
			labelCharZ.Text = ((float)trackBarCharZ.Value / 1000.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void numericUpDownCharAmbient_ValueChanged(object sender, EventArgs e)
		{
			labelCharAmbientR.Text = ((float)numericUpDownCharAmbientR.Value / 255.0f).ToString("0.000");
			labelCharAmbientG.Text = ((float)numericUpDownCharAmbientG.Value / 255.0f).ToString("0.000");
			labelCharAmbientB.Text = ((float)numericUpDownCharAmbientB.Value / 255.0f).ToString("0.000");
			pictureBoxCharAmbient.BackColor = Color.FromArgb((int)numericUpDownCharAmbientR.Value, (int)numericUpDownCharAmbientG.Value, (int)numericUpDownCharAmbientB.Value);
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void numericUpDownCharCO1_ValueChanged(object sender, EventArgs e)
		{
			labelCharCO1R.Text = ((float)numericUpDownCharCO1R.Value / 255.0f).ToString("0.000");
			labelCharCO1G.Text = ((float)numericUpDownCharCO1G.Value / 255.0f).ToString("0.000");
			labelCharCO1B.Text = ((float)numericUpDownCharCO1B.Value / 255.0f).ToString("0.000");
			pictureBoxCharCO1.BackColor = Color.FromArgb((int)numericUpDownCharCO1R.Value, (int)numericUpDownCharCO1G.Value, (int)numericUpDownCharCO1B.Value);
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void numericUpDownCharCO2_ValueChanged(object sender, EventArgs e)
		{
			labelCharCO2R.Text = ((float)numericUpDownCharCO2R.Value / 255.0f).ToString("0.000");
			labelCharCO2G.Text = ((float)numericUpDownCharCO2G.Value / 255.0f).ToString("0.000");
			labelCharCO2B.Text = ((float)numericUpDownCharCO2B.Value / 255.0f).ToString("0.000");
			pictureBoxCharCO2.BackColor = Color.FromArgb((int)numericUpDownCharCO2R.Value, (int)numericUpDownCharCO2G.Value, (int)numericUpDownCharCO2B.Value);
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void numericUpDownCharSP1_ValueChanged(object sender, EventArgs e)
		{
			labelCharSP1R.Text = ((float)numericUpDownCharSP1R.Value / 255.0f).ToString("0.000");
			labelCharSP1G.Text = ((float)numericUpDownCharSP1G.Value / 255.0f).ToString("0.000");
			labelCharSP1B.Text = ((float)numericUpDownCharSP1B.Value / 255.0f).ToString("0.000");
			pictureBoxCharSP1.BackColor = Color.FromArgb((int)numericUpDownCharSP1R.Value, (int)numericUpDownCharSP1G.Value, (int)numericUpDownCharSP1B.Value);
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void numericUpDownCharSP2_ValueChanged(object sender, EventArgs e)
		{
			labelCharSP2R.Text = ((float)numericUpDownCharSP2R.Value / 255.0f).ToString("0.000");
			labelCharSP2G.Text = ((float)numericUpDownCharSP2G.Value / 255.0f).ToString("0.000");
			labelCharSP2B.Text = ((float)numericUpDownCharSP2B.Value / 255.0f).ToString("0.000");
			pictureBoxCharSP2.BackColor = Color.FromArgb((int)numericUpDownCharSP2R.Value, (int)numericUpDownCharSP2G.Value, (int)numericUpDownCharSP2B.Value);
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarCharCO1Pow_ValueChanged(object sender, EventArgs e)
		{
			labelCharCO1Pow.Text = ((float)trackBarCharCO1Pow.Value / 100.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarCharCO2Pow_ValueChanged(object sender, EventArgs e)
		{
			labelCharCO2Pow.Text = ((float)trackBarCharCO2Pow.Value / 100.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarCharSP1Pow_ValueChanged(object sender, EventArgs e)
		{
			labelCharSP1Pow.Text = ((float)trackBarCharSP1Pow.Value / 100.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void trackBarCharSP2Pow_ValueChanged(object sender, EventArgs e)
		{
			labelCharSP2Pow.Text = ((float)trackBarCharSP2Pow.Value / 100.0f).ToString("0.000");
			if (!Suspend)
				UnsavedChanges = true;
		}

		private void comboBoxLevel_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (UnsavedChanges)
				ApplyChangesDialog();
			if (!Suspend)
				SetCurrentLights();
		}

		private void numericUpDownAct_ValueChanged(object sender, EventArgs e)
		{
			if (UnsavedChanges)
				ApplyChangesDialog();
			if (!Suspend)
				SetCurrentLights();
		}

		private void comboBoxLightType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (UnsavedChanges)
				ApplyChangesDialog();
			if (!Suspend)
				SetCurrentLights();
		}

		private void numericUpDownStageLightIndex_ValueChanged(object sender, EventArgs e)
		{
			if (UnsavedChanges)
				ApplyChangesDialog();
			if (!Suspend)
				SetCurrentLights();
		}

		private void buttonCreateLight_Click(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == tabPageStageLights)
			{
				RemoveStageLight(comboBoxLevel.SelectedIndex, (int)numericUpDownAct.Value, (int)numericUpDownStageLightIndex.Value);
				AddStageLight(comboBoxLevel.SelectedIndex, (int)numericUpDownAct.Value, (int)numericUpDownStageLightIndex.Value);
			}
			else
			{
				RemoveCharLight(comboBoxLevel.SelectedIndex, (int)numericUpDownAct.Value, comboBoxLightType.SelectedIndex);
				AddCharacterLight(comboBoxLevel.SelectedIndex, (int)numericUpDownAct.Value, comboBoxLightType.SelectedIndex);
			}
			SetCurrentLights();
			UnsavedChanges = true;
		}

		private void buttonDeleteLight_Click(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == tabPageStageLights)
				RemoveStageLight(comboBoxLevel.SelectedIndex, (int)numericUpDownAct.Value, (int)numericUpDownStageLightIndex.Value);
			else
				RemoveCharLight(comboBoxLevel.SelectedIndex, (int)numericUpDownAct.Value, comboBoxLightType.SelectedIndex);
			SetCurrentLights();
			UnsavedChanges = true;
		}

		private void ApplyChangesDialog()
		{
			DialogResult result = MessageBox.Show(this, "Apply changes?", "SALVL Lights Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

		private void ApplyChanges()
		{
			// Stage Light
			if (CurrentStageLight != null)
			{
				CurrentStageLight.UseDirection = checkBoxStageUseDir.Checked;
				CurrentStageLight.Diffuse = (float)trackBarStageDiffuse.Value / 1000.0f;
				CurrentStageLight.Specular = (float)trackBarStageSpecular.Value / 1000.0f;
				CurrentStageLight.Direction.X = (float)trackBarStageX.Value / 1000.0f;
				CurrentStageLight.Direction.Y = (float)trackBarStageY.Value / 1000.0f;
				CurrentStageLight.Direction.Z = (float)trackBarStageZ.Value / 1000.0f;
				CurrentStageLight.AmbientRGB.X = (float)numericUpDownStageAmbientR.Value / 255.0f;
				CurrentStageLight.AmbientRGB.Y = (float)numericUpDownStageAmbientG.Value / 255.0f;
				CurrentStageLight.AmbientRGB.Z = (float)numericUpDownStageAmbientB.Value / 255.0f;
				CurrentStageLight.RGB.X = (float)numericUpDownStageDifR.Value / 255.0f;
				CurrentStageLight.RGB.Y = (float)numericUpDownStageDifG.Value / 255.0f;
				CurrentStageLight.RGB.Z = (float)numericUpDownStageDifB.Value / 255.0f;
			}
			// Character Light
			if (CurrentCharLight != null)
			{
				LSPaletteData.LSPaletteFlags flags = 0;
				if (checkBoxCharEnabled.Checked)
					flags |= LSPaletteData.LSPaletteFlags.Enabled;
				if (checkBoxCharUseDir.Checked)
					flags |= LSPaletteData.LSPaletteFlags.UseLSLightDirection;
				if (checkBoxCharIgnoreDir.Checked)
					flags |= LSPaletteData.LSPaletteFlags.IgnoreDirection;
				if (checkBoxCharDouble.Checked)
					flags |= LSPaletteData.LSPaletteFlags.OverrideLastLight;
				if (checkBoxCharUnknown.Checked)
					flags |= LSPaletteData.LSPaletteFlags.Unknown;
				CurrentCharLight.Flags = flags;
				CurrentCharLight.Diffuse = (float)trackBarCharDiffuse.Value / 1000.0f;
				// Light Direction
				CurrentCharLight.Direction.X = (float)trackBarCharX.Value / 1000.0f;
				CurrentCharLight.Direction.Y= (float)trackBarCharY.Value / 1000.0f;
				CurrentCharLight.Direction.Z = (float)trackBarCharZ.Value / 1000.0f;
				// Ambient
				CurrentCharLight.Ambient.X = (float)numericUpDownCharAmbientR.Value / 255.0f;
				CurrentCharLight.Ambient.Y = (float)numericUpDownCharAmbientG.Value / 255.0f;
				CurrentCharLight.Ambient.Z = (float)numericUpDownCharAmbientB.Value / 255.0f;
				// CO1
				CurrentCharLight.Color1.X = (float)numericUpDownCharCO1R.Value / 255.0f;
				CurrentCharLight.Color1.Y = (float)numericUpDownCharCO1G.Value / 255.0f;
				CurrentCharLight.Color1.Z = (float)numericUpDownCharCO1B.Value / 255.0f;
				CurrentCharLight.Color1Power = (float)trackBarCharCO1Pow.Value / 100.0f;
				// CO2
				CurrentCharLight.Color2.X = (float)numericUpDownCharCO2R.Value / 255.0f;
				CurrentCharLight.Color2.Y = (float)numericUpDownCharCO2G.Value / 255.0f;
				CurrentCharLight.Color2.Z = (float)numericUpDownCharCO2B.Value / 255.0f;
				CurrentCharLight.Color2Power = (float)trackBarCharCO2Pow.Value / 100.0f;
				// SP1
				CurrentCharLight.Specular1.X = (float)numericUpDownCharSP1R.Value / 255.0f;
				CurrentCharLight.Specular1.Y = (float)numericUpDownCharSP1G.Value / 255.0f;
				CurrentCharLight.Specular1.Z = (float)numericUpDownCharSP1B.Value / 255.0f;
				CurrentCharLight.Specular1Power = (float)trackBarCharSP1Pow.Value / 100.0f;
				// SP2
				CurrentCharLight.Specular2.X = (float)numericUpDownCharSP2R.Value / 255.0f;
				CurrentCharLight.Specular2.Y = (float)numericUpDownCharSP2G.Value / 255.0f;
				CurrentCharLight.Specular2.Z = (float)numericUpDownCharSP2B.Value / 255.0f;
				CurrentCharLight.Specular2Power = (float)trackBarCharSP2Pow.Value / 100.0f;
			}
			UnsavedChanges = false;
			EditorMainForm.GetEditedLights(StageLights, CharLights);
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			ApplyChanges();
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			if (UnsavedChanges)
				ApplyChangesDialog();
			Close();
		}

		private void SetUnsavedChanges(object sender, EventArgs e)
		{
			if (!Suspend)
				UnsavedChanges = true;
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

		private void pictureBoxStageDiffuse_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxStageDiffuse.BackColor);
			Suspend = true;
			numericUpDownStageDifR.Value = newColor.R;
			numericUpDownStageDifG.Value = newColor.G;
			numericUpDownStageDifB.Value = newColor.B;
			Suspend = false;
		}

		private void pictureBoxStageAmbient_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxStageAmbient.BackColor);
			Suspend = true;
			numericUpDownStageAmbientR.Value = newColor.R;
			numericUpDownStageAmbientG.Value = newColor.G;
			numericUpDownStageAmbientB.Value = newColor.B;
			Suspend = false;
		}

		private void pictureBoxCharCO1_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxCharCO1.BackColor);
			Suspend = true;
			numericUpDownCharCO1R.Value = newColor.R;
			numericUpDownCharCO1G.Value = newColor.G;
			numericUpDownCharCO1B.Value = newColor.B;
			Suspend = false;
		}

		private void pictureBoxCharCO2_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxCharCO2.BackColor);
			Suspend = true;
			numericUpDownCharCO2R.Value = newColor.R;
			numericUpDownCharCO2G.Value = newColor.G;
			numericUpDownCharCO2B.Value = newColor.B;
			Suspend = false;
		}

		private void pictureBoxCharSP1_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxCharSP1.BackColor);
			Suspend = true;
			numericUpDownCharSP1R.Value = newColor.R;
			numericUpDownCharSP1G.Value = newColor.G;
			numericUpDownCharSP1B.Value = newColor.B;
			Suspend = false;
		}

		private void pictureBoxCharSP2_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxCharSP2.BackColor);
			Suspend = true;
			numericUpDownCharSP2R.Value = newColor.R;
			numericUpDownCharSP2G.Value = newColor.G;
			numericUpDownCharSP2B.Value = newColor.B;
			Suspend = false;
		}

		private void pictureBoxCharAmbient_Click(object sender, EventArgs e)
		{
			Color newColor = GetColorFromDialog(pictureBoxCharAmbient.BackColor);
			Suspend = true;
			numericUpDownCharAmbientR.Value = newColor.R;
			numericUpDownCharAmbientG.Value = newColor.G;
			numericUpDownCharAmbientB.Value = newColor.B;
			Suspend = false;
		}

		private void SADXLightsEditor_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start " + "https://github.com/X-Hax/sa_tools/wiki/SALVL#lights-editor") { CreateNoWindow = false });
		}

		private void EditorClosing(object sender, FormClosingEventArgs e)
		{
			if (UnsavedChanges)
				ApplyChangesDialog();
		}
	}
}