using System;
using System.Windows.Forms;

namespace SA2MessageFileEditor
{
	public partial class FindDialog : Form
	{
		public FindDialog()
		{
			InitializeComponent();
		}

		private void playsAudioCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (playsAudioCheckBox.Checked)
			{
				audioSelector.Enabled = true;
				playsVoiceCheckBox.Checked = false;
			}
			else
				audioSelector.Enabled = false;
		}

		private void playsVoiceCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (playsVoiceCheckBox.Checked)
			{
				voiceSelector.Enabled = true;
				playsAudioCheckBox.Checked = false;
			}
			else
				voiceSelector.Enabled = false;
		}

		private void containsTextCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			lineEdit.Enabled = containsTextCheckBox.Checked;
		}

		private void ToggleFindButton()
		{
			findButton.Enabled = playsAudioCheckBox.Checked || playsVoiceCheckBox.Checked || containsTextCheckBox.Checked;
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
			if (playsAudioCheckBox.Checked)
				result.Audio = (int)audioSelector.Value;
			else if (playsVoiceCheckBox.Checked)
				result.Voice = (int)voiceSelector.Value;
			if (containsTextCheckBox.Checked)
				result.Text = lineEdit.Text.Replace(Environment.NewLine, "\n");
			return result;
		}
	}
}
