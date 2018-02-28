using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SA2CutsceneTextEditor
{
	public partial class FindDialog : Form
	{
		public FindDialog()
		{
			InitializeComponent();
		}

		private void characterCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			characterSelector.Enabled = characterCheckBox.Checked;
		}

		private void containsTextCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			lineEdit.Enabled = containsTextCheckBox.Checked;
		}

		private void ToggleFindButton()
		{
			findButton.Enabled = characterCheckBox.Checked || containsTextCheckBox.Checked;
		}

		private void findButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		public SearchCriteria GetSearchCriteria()
		{
			SearchCriteria result = new SearchCriteria();
			if (characterCheckBox.Checked)
				result.Character = (int)characterSelector.Value;
			if (containsTextCheckBox.Checked)
				result.Text = lineEdit.Text.Replace(Environment.NewLine, "\n");
			return result;
		}
	}
}
