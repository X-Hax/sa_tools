using System;
using System.IO;
using System.Windows.Forms;
using VMSEditor.Properties;

namespace VMSEditor
{
	public partial class EditorChallengeResult : Form
	{
		private readonly Form ProgramModeSelectorForm;

		public EditorChallengeResult(Form parent = null)
		{
			InitializeComponent();
			ProgramModeSelectorForm = parent;
			if (parent == null)
				Application.ThreadException += ProgramModeSelector.Application_ThreadException;
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
			byte[] file;
			if (Path.GetExtension(filename).ToLowerInvariant() == ".dci")
				file = VMSFile.GetVMSFromDCI(File.ReadAllBytes(filename));
			else
				file = File.ReadAllBytes(filename);
			VMSChallengeResult result = new VMSChallengeResult(file);
			numericUpDownEventID.Value = result.ResultData.EventID;
			numericUpDownFrames.Value = result.ResultData.EventTime;
			comboBoxCharacter.SelectedIndex = (int)result.ResultData.Character;
			labelTime.Text = "Total Time: " + FramesToTimeString((int)result.ResultData.EventTime);
			radioButtonCart.Checked = result.DataType == DataIDs.CartResultChecksum;
			radioButtonEvent.Checked = result.DataType == DataIDs.EventResultChecksum;
			textBoxIndividualID.Text = VMSFile.GetFieldFromHTML(file, "dcid");
			textBoxSubmitted.Text = VMSFile.GetFieldFromHTML(file, "mailid");
			radioButtonJapan.Checked = (BitConverter.ToUInt32(file, 0) == 0xDDDECDB2) || (BitConverter.ToUInt32(file, 0) == 0xC0C4B0B6);
			radioButtonInternational.Checked = System.Text.Encoding.GetEncoding(932).GetString(file, 0, 12) == "EVENT_RESULT" || System.Text.Encoding.GetEncoding(932).GetString(file, 0, 9) == "CART_TIME";
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
			labelTime.Text = "Total Time: " + FramesToTimeString((int)numericUpDownFrames.Value);
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
			if (ProgramModeSelectorForm != null)
				ProgramModeSelectorForm.Show();
			else
				Application.Exit();
		}

		private void radioButtonCart_CheckedChanged(object sender, EventArgs e)
		{
			pictureBoxDataType.Image = radioButtonCart.Checked ? Resources.cartresult : Resources.eventresult;
		}

		private void issueTrackerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/issues") { CreateNoWindow = true });
		}

		private void challengeResultViewerHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/wiki/Challenge-Result-Viewer") { CreateNoWindow = true });
		}
	}
}
