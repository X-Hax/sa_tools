using System;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon.UI
{
	public partial class LabelEditor : Form
	{
		private string OriginalValue;
		private bool IsNumeric;
		public string Result;

		public LabelEditor(string originalValue, string headerText = "", bool numeric = false)
		{
			IsNumeric = numeric;
			InitializeComponent();
			Text = !string.IsNullOrEmpty(headerText) ? headerText : (numeric ? "Edit Value: " : "Edit Label: " + originalValue);
			textBox1.Text = originalValue;
			OriginalValue = originalValue;
			textBox1.SelectAll();
			textBox1.Focus();
		}

		private void buttonReset_Click(object sender, EventArgs e)
		{
			textBox1.Text = OriginalValue;
		}

		private void LabelEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (IsNumeric)
			{
				if (!int.TryParse(textBox1.Text, out int result))
				{
					MessageBox.Show(this, "Unable to parse text '" + textBox1.Text + "' as an integer value. Enter a valid number and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Cancel = true;
					return;
				}
			}
			Result = textBox1.Text;
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
