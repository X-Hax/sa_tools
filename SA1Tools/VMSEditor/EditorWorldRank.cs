using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VMSEditor
{
	public partial class EditorWorldRank : Form
	{
		private readonly Form ProgramModeSelectorForm;
		private byte[] DecryptedData;

		public EditorWorldRank(Form parent = null)
		{
			ProgramModeSelectorForm = parent;
			InitializeComponent();
			saveAsToolStripMenuItem.Enabled = false;
			if (Program.args.Length > 0)
				LoadWorldRankFile(Program.args[0]);
			else
				radioButtonInternational.Checked = true;
			if (parent == null)
				Application.ThreadException += ProgramModeSelector.Application_ThreadException;
		}

		private string FramesToTimeString(int frames)
		{
			int seconds = frames / 60 - 60 * (frames / 3600);
			int minutes = frames / 3600 - 60 * (frames / 216000);
			int hours = frames / 216000;
			return hours.ToString() + " hours, " + minutes.ToString() + " minutes, " + seconds.ToString() + " seconds";
		}

		public void LoadWorldRankFile(string filename)
		{
			byte[] file = File.ReadAllBytes(filename);
			// Get region
			radioButtonJapan.Checked = BitConverter.ToUInt32(file, 0) == 0xC0B0DEC3;
			radioButtonInternational.Checked = Encoding.GetEncoding(932).GetString(file, 0, 11) == "DATA_UPLOAD";
			// Decrypt data
			DecryptedData = VMSFile.GetDataFromHTML(file);
			// Set fields
			labelPlayTime.Text = "Total Time: " + FramesToTimeString(BitConverter.ToInt32(DecryptedData, 0x48));
			textBoxIndividualID.Text = VMSFile.GetFieldFromHTML(file, "dcid");
			textBoxSubmitted.Text = VMSFile.GetFieldFromHTML(file, "mailid");
			saveAsToolStripMenuItem.Enabled = true;
		}

		private void SaveAsSavefile(string destFilename)
		{
			byte[] header = radioButtonJapan.Checked ? Properties.Resources.save_jp : Properties.Resources.save_us;
			byte[] output = new byte[5120];
			// Copy save header
			Array.Copy(header, 0, output, 0, 0x480);
			// Copy slot 1 data
			Array.Copy(DecryptedData, 68, output, 0x480, DecryptedData.Length - 68);
			// Duplicate slot 1 data as slots 2 and 3
			Array.Copy(DecryptedData, 68, output, 0x480 + DecryptedData.Length - 68, DecryptedData.Length - 68);
			Array.Copy(DecryptedData, 68, output, 0x480 + (DecryptedData.Length - 68) * 2, DecryptedData.Length - 68);
			File.WriteAllBytes(destFilename, output);
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void EditorWorldRank_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (ProgramModeSelectorForm != null)
				ProgramModeSelectorForm.Show();
			else
				Application.Exit();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog { Filter = "World Rankings Data US|SONICADV_H04*.VMS|World Rankings Data JP|SONICADV_H01*.VMS|VMS Files|*.VMS|All Files|*.*" })
			{
				if (dlg.ShowDialog() == DialogResult.OK)
					LoadWorldRankFile(dlg.FileName);
			}
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string defname = radioButtonJapan.Checked ? "SONICADV_SYS.VMS" : "SONICADV_INT.VMS";
			using (SaveFileDialog dlg = new SaveFileDialog { FileName = defname, Filter = "Save File JP|SONICADV_SYS*.VMS|Save File US JP|SONICADV_INT*.VMS|VMS Files|*.VMS|All Files|*.*", FilterIndex = radioButtonJapan.Checked ? 1 : 2 })
			{
				if (dlg.ShowDialog() == DialogResult.OK)
					SaveAsSavefile(dlg.FileName);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void worldRankingsConverterHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/wiki/World-Rankings-Converter") { CreateNoWindow = true });
		}

		private void issueTrackerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/issues") { CreateNoWindow = true });
		}
	}
}