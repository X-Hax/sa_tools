using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
		}

		private string FramesToTimeString(int frames)
		{
			int miliseconds = (int)((float)(frames % 60) / 60.0f * 100.0f);
			int seconds = frames / 60;
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
			using (OpenFileDialog dlg = new OpenFileDialog { Filter = "Event Result US|SONICADV_H06*.VMS|Event Result JP|SONICADV_H03*.VMS|VMS Files|*.VMS|All Files|*.*" })
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
	}
}
