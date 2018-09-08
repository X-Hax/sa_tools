using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public partial class DefaultLightEditor : Form
	{
		public DefaultLightEditor()
		{
			InitializeComponent();
		}

		private void DefaultLightEditor_Load(object sender, EventArgs e)
		{
			comboBox1.Items.Clear();

			for (int i = 0; i < 4; i++)
			{
				comboBox1.Items.Add(i.ToString());
			}

			SetSelection(0);
		}

		private void SetSelection(int selection)
		{
			if (selection > -1) propertyGrid1.SelectedObject = EditorOptions.Direct3DDevice.GetLight(selection);
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetSelection(comboBox1.SelectedIndex);
		}
	}
}
