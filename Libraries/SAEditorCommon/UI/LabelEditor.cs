using System;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon.UI
{
	public partial class LabelEditor : Form
	{
		private string original;
		public string result;

		public LabelEditor(string label)
		{
			InitializeComponent();
			Text = "Edit Label: " + label;
			textBox1.Text = label;
			original = label;
			textBox1.SelectAll();
			textBox1.Focus();
		}

		private void buttonReset_Click(object sender, EventArgs e)
		{
			textBox1.Text = original;
		}

		private void LabelEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			result = textBox1.Text;
		}

		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Return:
					DialogResult = DialogResult.OK;
					Close();
					break;
				case Keys.Escape:
					DialogResult = DialogResult.Cancel;
					Close();
					break;
				default:
					break;
			}
		}
	}
}
