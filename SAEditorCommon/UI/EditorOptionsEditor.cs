using System;
using System.Windows.Forms;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public partial class EditorOptionsEditor : Form
	{
		public Action CustomizeKeybindsCommand;
		public Action ResetDefaultKeybindsCommand;
		
		public delegate void FormUpdatedHandler();
		public event FormUpdatedHandler FormUpdated;
		public event FormUpdatedHandler FormUpdatedKeys;

		private EditorCamera camera;
		private ToolTip toolTip = new ToolTip();

		ActionKeyMapping[] actionKeyMappings;
		ActionKeyMapping[] defaultActionKeyMappings;

		private enum SelectedKeyType 
		{
			None = -1,
			Main = 0,
			Alt = 1,
			Modifier = 2,
		};

		private SelectedKeyType SelectedKey = SelectedKeyType.None;

		public EditorOptionsEditor(EditorCamera camera, ActionKeyMapping[] actionKeyMappings_f, ActionKeyMapping[] defaultActionKeyMappings_f, bool setdist_enabled_a, bool leveldist_enabled_a)
		{
			InitializeComponent();
			this.camera = camera;
			drawDistSlider.Value = (int)EditorOptions.RenderDrawDistance;
			levelDrawDistSlider.Value = (int)EditorOptions.LevelDrawDistance;
			setDrawDistSlider.Value = (int)EditorOptions.SetItemDrawDistance;
			fillModeDropDown.SelectedIndex = (int)EditorOptions.RenderFillMode - 1;
			cullModeDropdown.SelectedIndex = (int)EditorOptions.RenderCullMode - 1;
			if (setdist_enabled_a)
			{
				setDrawDistLabel.Enabled = true;
				setDrawDistSlider.Enabled = true;

			}
			if (leveldist_enabled_a)
			{
				levelDrawDistLabel.Enabled = true;
				levelDrawDistSlider.Enabled = true;
			}
			toolTip.SetToolTip(fullBrightCheck, "If the scene's lighting is making it hard to work, use this to temporarily set the lighting to flat-white.");
			toolTip.SetToolTip(ignoreMaterialColorsCheck, "Treat all material colors as white like Dreamcast and Gamecube versions of SA1/SADX/SA2 do.");
			fullBrightCheck.Checked = EditorOptions.OverrideLighting;
			ignoreMaterialColorsCheck.Checked = EditorOptions.IgnoreMaterialColors;
			UpdateSliderValues();

			// Keybind editor
			actionKeyMappings = actionKeyMappings_f;
			defaultActionKeyMappings = defaultActionKeyMappings_f;
			listBoxActions.Items.Clear();
			foreach (ActionKeyMapping keyMapping in actionKeyMappings)
			{
				listBoxActions.Items.Add(keyMapping.Name);
			}
			SelectedKeyChanged();
			ModifierKeyUpdated();
		}

		private void UpdateSliderValues()
		{
			drawDistLabel.Text = String.Format("General: {0}", drawDistSlider.Value);
			levelDrawDistLabel.Text = String.Format("Level Geometry: {0}", levelDrawDistSlider.Value); ;
			setDrawDistLabel.Text = String.Format("SET/CAM Items: {0}", setDrawDistSlider.Value);
			EditorOptions.RenderDrawDistance = drawDistSlider.Value;
			EditorOptions.SetItemDrawDistance = setDrawDistSlider.Value;
			EditorOptions.LevelDrawDistance = levelDrawDistSlider.Value;
			camera.DrawDistance = EditorOptions.RenderDrawDistance;
		}

		private void drawDistSlider_Scroll(object sender, EventArgs e)
		{
			if (drawDistSlider.Value < levelDrawDistSlider.Value) levelDrawDistSlider.Value = drawDistSlider.Value;
			if (drawDistSlider.Value < setDrawDistSlider.Value) setDrawDistSlider.Value = drawDistSlider.Value;
			UpdateSliderValues();
			FormUpdated();
		}

		private void levelDrawDistSlider_Scroll(object sender, EventArgs e)
		{
			if (drawDistSlider.Value < levelDrawDistSlider.Value) drawDistSlider.Value = levelDrawDistSlider.Value;
			UpdateSliderValues();
			FormUpdated();
		}

		private void setDrawDistSlider_Scroll(object sender, EventArgs e)
		{
			if (drawDistSlider.Value < setDrawDistSlider.Value) drawDistSlider.Value = setDrawDistSlider.Value;
			UpdateSliderValues();
			FormUpdated();
		}

		private void fillModeDropDown_SelectionChangeCommitted(object sender, EventArgs e)
		{
			EditorOptions.RenderFillMode = (FillMode)fillModeDropDown.SelectedIndex + 1;
			FormUpdated();
		}

		private void cullModeDropdown_SelectionChangeCommitted(object sender, EventArgs e)
		{
			EditorOptions.RenderCullMode = (Cull)cullModeDropdown.SelectedIndex + 1;
			FormUpdated();
		}

		private void doneButton_Click(object sender, EventArgs e)
		{
			FormUpdatedKeys();
			Close();
		}

		private void fullBrightCheck_Click(object sender, EventArgs e)
		{
			EditorOptions.OverrideLighting = fullBrightCheck.Checked;
			FormUpdated();
		}

		private void KeyboardShortcutButton_Click(object sender, EventArgs e)
		{
			CustomizeKeybindsCommand.Invoke();
		}

		private void EditorOptionsEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

        private void ignoreMaterialColorsCheck_Click(object sender, EventArgs e)
        {
			EditorOptions.IgnoreMaterialColors = ignoreMaterialColorsCheck.Checked;
			FormUpdated();
		}

		#region Keybinds
		public ActionKeyMapping[] GetActionkeyMappings()
		{
			return actionKeyMappings;
		}

		private void ResetDefaultKeybindButton_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("This will replace all your keybinds with defaults. Would you like to proceed?", "Are you sure?", MessageBoxButtons.YesNo);
			if (dialogResult != DialogResult.Yes) 
				return;
			listBoxActions.Items.Clear();
			foreach (ActionKeyMapping keyMapping in actionKeyMappings)
			{
				listBoxActions.Items.Add(keyMapping.Name);
				actionKeyMappings[listBoxActions.Items.IndexOf(keyMapping.Name)].MainKey = defaultActionKeyMappings[listBoxActions.Items.IndexOf(keyMapping.Name)].MainKey;
				actionKeyMappings[listBoxActions.Items.IndexOf(keyMapping.Name)].AltKey = defaultActionKeyMappings[listBoxActions.Items.IndexOf(keyMapping.Name)].AltKey;
				actionKeyMappings[listBoxActions.Items.IndexOf(keyMapping.Name)].Modifiers = defaultActionKeyMappings[listBoxActions.Items.IndexOf(keyMapping.Name)].Modifiers;
			}
			listBoxActions.SelectedIndex = -1;
			textBoxMainKey.Text = "";
			textBoxAltKey.Text = "";
			textBoxModifier.Text = "";
			labelDescription.Text = "";
			SelectedKey = SelectedKeyType.None;
			textBoxMainKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
			textBoxAltKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
			textBoxModifier.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
			groupBoxKeys.Enabled = false;
			FormUpdatedKeys();
		}

		private void listBoxActions_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (listBoxActions.SelectedIndex == -1)
			{
				groupBoxKeys.Enabled = false;
				textBoxMainKey.Text = "";
				textBoxAltKey.Text = "";
				textBoxModifier.Text = "";
				labelDescription.Text = "";
			}
			else
			{
				groupBoxKeys.Enabled = true;
				textBoxMainKey.Text = actionKeyMappings[listBoxActions.SelectedIndex].MainKey.ToString();
				textBoxAltKey.Text = actionKeyMappings[listBoxActions.SelectedIndex].AltKey.ToString();
				textBoxModifier.Text = actionKeyMappings[listBoxActions.SelectedIndex].Modifiers.ToString();
				labelDescription.Text = actionKeyMappings[listBoxActions.SelectedIndex].Description;
			}
		}

		private void buttonClearSelectedKey_Click(object sender, EventArgs e)
		{
			switch (SelectedKey)
			{
				case SelectedKeyType.Main:
					actionKeyMappings[listBoxActions.SelectedIndex].MainKey = Keys.None;
					textBoxMainKey.Text = "None";		
					break;
				case SelectedKeyType.Alt:
					actionKeyMappings[listBoxActions.SelectedIndex].AltKey = Keys.None;
					textBoxAltKey.Text = "None";
					break;
				case SelectedKeyType.Modifier:
					actionKeyMappings[listBoxActions.SelectedIndex].Modifiers = Keys.None;
					textBoxModifier.Text = "None";
					break;
			}
		}

        private void buttonResetSelectedKey_Click(object sender, EventArgs e)
        {
			switch (SelectedKey)
			{
				case SelectedKeyType.Main:
					actionKeyMappings[listBoxActions.SelectedIndex].MainKey = defaultActionKeyMappings[listBoxActions.SelectedIndex].MainKey;
					textBoxMainKey.Text = actionKeyMappings[listBoxActions.SelectedIndex].MainKey.ToString();
					break;
				case SelectedKeyType.Alt:
					actionKeyMappings[listBoxActions.SelectedIndex].AltKey = defaultActionKeyMappings[listBoxActions.SelectedIndex].AltKey;
					textBoxAltKey.Text = actionKeyMappings[listBoxActions.SelectedIndex].AltKey.ToString();
					break;
				case SelectedKeyType.Modifier:
					actionKeyMappings[listBoxActions.SelectedIndex].Modifiers = defaultActionKeyMappings[listBoxActions.SelectedIndex].Modifiers;
					textBoxModifier.Text = actionKeyMappings[listBoxActions.SelectedIndex].Modifiers.ToString();
					break;
			}
		}

		// KeyDown and MouseDown events for text boxes

		private void textBoxMainKey_KeyDown(object sender, KeyEventArgs e)
		{
			textBoxMainKey.Text = e.KeyCode.ToString();
			actionKeyMappings[listBoxActions.SelectedIndex].MainKey = e.KeyCode;
		}

		private void textBoxMainKey_MouseDown(object sender, MouseEventArgs e)
		{
			SelectedKey = SelectedKeyType.Main;
			if (e.Button == MouseButtons.Middle)
			{
				textBoxMainKey.Text = Keys.MButton.ToString();
				actionKeyMappings[listBoxActions.SelectedIndex].MainKey = Keys.MButton;
			}
			SelectedKeyChanged();
		}

		private void textBoxAltKey_KeyDown(object sender, KeyEventArgs e)
        {
			textBoxAltKey.Text = e.KeyCode.ToString();
			actionKeyMappings[listBoxActions.SelectedIndex].AltKey = e.KeyCode;
		}

        private void textBoxAltKey_MouseDown(object sender, MouseEventArgs e)
        {
			SelectedKey = SelectedKeyType.Alt;
			if (e.Button == MouseButtons.Middle)
			{
				textBoxAltKey.Text = Keys.MButton.ToString();
				actionKeyMappings[listBoxActions.SelectedIndex].AltKey = Keys.MButton;
			}
			SelectedKeyChanged();
		}

        private void textBoxModifier_KeyDown(object sender, KeyEventArgs e)
        {
			textBoxModifier.Text = e.KeyCode.ToString();
			actionKeyMappings[listBoxActions.SelectedIndex].Modifiers = e.KeyCode;
		}

		private void textBoxModifier_MouseDown(object sender, MouseEventArgs e)
		{
			SelectedKey = SelectedKeyType.Modifier;
			if (e.Button == MouseButtons.Middle)
			{
				textBoxModifier.Text = Keys.MButton.ToString();
				actionKeyMappings[listBoxActions.SelectedIndex].Modifiers = Keys.MButton;
			}
			SelectedKeyChanged();
		}

		private void SelectedKeyChanged()
		{
			switch (SelectedKey)
			{
				case SelectedKeyType.Main:
					buttonResetSelectedKey.Enabled = buttonClearSelectedKey.Enabled = true;
					textBoxMainKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlLightLight);
					textBoxAltKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
					textBoxModifier.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
					break;
				case SelectedKeyType.Alt:
					buttonResetSelectedKey.Enabled = buttonClearSelectedKey.Enabled = true;
					textBoxMainKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
					textBoxAltKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlLightLight);
					textBoxModifier.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
					break;
				case SelectedKeyType.Modifier:
					buttonResetSelectedKey.Enabled = buttonClearSelectedKey.Enabled = true;
					textBoxMainKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
					textBoxAltKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
					textBoxModifier.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlLightLight);
					break;
				default:
					buttonResetSelectedKey.Enabled = buttonClearSelectedKey.Enabled = false;
					textBoxMainKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
					textBoxAltKey.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
					textBoxModifier.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
					break;
			}
		}

		// Select the priority key
		private void radioButtonMove_Click(object sender, EventArgs e)
		{
			camera.ModifierKey = 0;
			ModifierKeyUpdated();
		}
		private void radioButtonLook_Click(object sender, EventArgs e)
		{
			camera.ModifierKey = 1;
			ModifierKeyUpdated();
		}

		private void radioButtonZoom_Click(object sender, EventArgs e)
		{
			camera.ModifierKey = 2;
			ModifierKeyUpdated();
		}

		private void ModifierKeyUpdated()
		{
			switch (camera.ModifierKey)
			{
				case 0:
					radioButtonMove.Checked = true;
					radioButtonZoom.Checked = radioButtonLook.Checked = false;
					break;
				case 1:
					radioButtonLook.Checked = true;
					radioButtonMove.Checked = radioButtonZoom.Checked = false;
					break;
				case 2:
					radioButtonZoom.Checked = true;
					radioButtonMove.Checked = radioButtonLook.Checked = false;
					break;
			}
		}
		#endregion
	}
}
