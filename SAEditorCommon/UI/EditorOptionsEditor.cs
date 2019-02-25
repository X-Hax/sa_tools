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

		private EditorCamera camera;
		private ToolTip toolTip = new ToolTip();

		public EditorOptionsEditor(EditorCamera camera)
		{
			InitializeComponent();
			this.camera = camera;
			drawDistSlider.Value = (int)EditorOptions.RenderDrawDistance;
			fillModeDropDown.SelectedIndex = (int)EditorOptions.RenderFillMode - 1;
			cullModeDropdown.SelectedIndex = (int)EditorOptions.RenderCullMode - 1;
			drawDistLabel.Text = String.Format("Draw Distance: {0}", drawDistSlider.Value);
			toolTip.SetToolTip(fullBrightCheck, "If the scene's lighting is making it hard to work, use this to temporarily set the lighting to flat-white.");
			fullBrightCheck.Checked = EditorOptions.OverrideLighting;
		}

		private void drawDistSlider_Scroll(object sender, EventArgs e)
		{
			drawDistLabel.Text = String.Format("Draw Distance: {0}", drawDistSlider.Value);
			EditorOptions.RenderDrawDistance = drawDistSlider.Value;
			camera.DrawDistance = EditorOptions.RenderDrawDistance;
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
			FormUpdated();
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

		private void ResetDefaultKeybindButton_Click(object sender, EventArgs e)
		{
			ResetDefaultKeybindsCommand.Invoke();
		}
	}
}
