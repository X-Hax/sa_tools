using System;
using System.IO;
using System.Windows.Forms;
using VMSEditor.Properties;

namespace VMSEditor
{
	public partial class EditorChallengeResult : Form
	{
		public EditorChallengeResult()
		{
			InitializeComponent();
		}

		private void EditorChallengeResult_Load(object sender, EventArgs e)
		{
			if (Program.args.Length > 0)
			{
				LoadEventResult(Program.args[0]);
			}
		}

		private void LoadEventResult(string filename)
		{
			VMSChallengeResult result = new VMSChallengeResult(File.ReadAllBytes(filename));
			numericUpDownEventID.Value = result.ResultData.EventID;
			numericUpDownFrames.Value = result.ResultData.EventTime;
			comboBoxCharacter.SelectedIndex = (int)result.ResultData.Character;
			labelTime.Text = FramesToTimeString((int)result.ResultData.EventTime);
			radioButtonCart.Checked = result.DataType == DataIDs.CartResultChecksum;
			radioButtonEvent.Checked = result.DataType == DataIDs.EventResultChecksum;
		}

		private string FramesToTimeString(int frames)
		{
			int miliseconds = (int)((float)(frames % 60) / 60.0f * 100.0f);
			int seconds = frames / 60 - 60 * (frames / 3600);
			int minutes = frames / 3600;
			return minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + miliseconds.ToString("D2");
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void numericUpDownFrames_ValueChanged(object sender, EventArgs e)
		{
			labelTime.Text = FramesToTimeString((int)numericUpDownFrames.Value);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog { Filter = "Cart Result US|SONICADV_H05*.VMS|Cart Result JP|SONICADV_H02*.VMS|Minigame Result US|SONICADV_H06*.VMS|Minigame Result JP|SONICADV_H03*.VMS|VMS Files|*.VMS|All Files|*.*" })
			{
				if (dlg.ShowDialog() == DialogResult.OK)
					LoadEventResult(dlg.FileName);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void EditorChallengeResult_FormClosing(object sender, FormClosingEventArgs e)
		{
			Application.Exit();
		}

		private void radioButtonCart_CheckedChanged(object sender, EventArgs e)
		{
			pictureBoxDataType.Image = radioButtonCart.Checked ? Resources.cartresult : Resources.eventresult;
		}
	}
}
