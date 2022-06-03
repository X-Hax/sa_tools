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
				VMSChallengeResult result = new VMSChallengeResult(File.ReadAllBytes(Program.args[0]));
				string filename = Path.Combine(Path.GetDirectoryName(Program.args[0]), Path.GetFileNameWithoutExtension(Program.args[0] + "_data.bin"));
				File.WriteAllBytes(filename, result.GetBytes());
				MessageBox.Show("Decrypted data saved as " + filename + ".");
			}
			Close();
		}
	}
}
