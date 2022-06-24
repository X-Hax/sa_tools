using System;
using System.IO;
using System.Windows.Forms;

namespace VMSEditor
{
	public partial class ProgramModeSelector : Form
	{
        public ProgramModeSelector()
		{
			InitializeComponent();
		}

		private void buttonExit_Click(object sender, EventArgs e)
		{
            Close();
		}

		private void buttonChaoEditor_Click(object sender, EventArgs e)
		{
            EditorChao editor = new EditorChao();
            editor.Show();
            Hide();
        }

		private void buttonDLCEditor_Click(object sender, EventArgs e)
		{
            EditorDLC editor = new EditorDLC();
            editor.Show();
            Hide();
        }

		private void buttonVMIEditor_Click(object sender, EventArgs e)
		{
            EditorVMI editor = new EditorVMI();
            editor.Show();
			Hide();
		}

		private void buttonChallengeResultEditor_Click(object sender, EventArgs e)
		{
			EditorChallengeResult editor = new EditorChallengeResult();
			editor.Show();
			Hide();
		}

		private void buttonWorldRankEditor_Click(object sender, EventArgs e)
		{
			EditorWorldRank editor = new EditorWorldRank();
			editor.Show();
			Hide();
		}

		private void buttonOpenFile_Click(object sender, EventArgs e)
        {          
            using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "vms", Filter = "Supported Files|*.vms;*.vmi|VMS Files|*.vms|VMI Files|*.vmi|All Files|*.*" })
                if (od.ShowDialog(this) == DialogResult.OK)
                {
                    Form editor;
                    Program.args = new string[1];
                    Program.args[0] = od.FileName;
                    editor = Program.CheckFile(od.FileName);
                    editor.Show();
					Hide();
				}
        }

		private void buttonAdditional_Click(object sender, EventArgs e)
		{
			contextMenuStripTools.Show(buttonAdditional, buttonAdditional.Width,0);
		}

		private void vMSFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "vms", Filter = "VMS Files|*.vms|All Files|*.*" })
				if (od.ShowDialog(this) == DialogResult.OK)
				{
					byte[] data = File.ReadAllBytes(od.FileName);
					int aniFrames = data[0x40] + (data[0x41] << 8);
					int dataStart = 0x80 + (aniFrames * 0x200);
					byte[] encrypted = new byte[data.Length - dataStart];
					Array.Copy(data, dataStart, encrypted, 0, encrypted.Length);
					VMSFile.DecryptData(ref encrypted);
					Array.Copy(encrypted, 0, data, dataStart, encrypted.Length);
					string outpath = Path.Combine(Path.GetDirectoryName(od.FileName), Path.GetFileNameWithoutExtension(od.FileName)) + "_dec.vms";
					File.WriteAllBytes(outpath, data);
					MessageBox.Show(this, "VMS file saved as:\n" + outpath, "VMS Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
		}

		private void wholeFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "bin", Filter = "All Files|*.*" })
				if (od.ShowDialog(this) == DialogResult.OK)
				{
					byte[] data = File.ReadAllBytes(od.FileName);
					VMSFile.DecryptData(ref data);
					string outpath = Path.Combine(Path.GetDirectoryName(od.FileName), Path.GetFileNameWithoutExtension(od.FileName)) + "_dec.bin";
					File.WriteAllBytes(outpath, data);
					MessageBox.Show(this, "Binary file saved as:\n" + outpath, "VMS Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
		}

		private void decodeUploadDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "vms", Filter = "VMS Files|*.vms|All Files|*.*" })
				if (od.ShowDialog(this) == DialogResult.OK)
				{
					byte[] data = File.ReadAllBytes(od.FileName);
					byte[] result = VMSFile.GetDataFromHTML(data);
					string outpath = Path.Combine(Path.GetDirectoryName(od.FileName), Path.GetFileNameWithoutExtension(od.FileName)) + "_dec.bin";
					File.WriteAllBytes(outpath, result);
					MessageBox.Show(this, "Binary file saved as:\n" + outpath, "VMS Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
		}
	}
}
