using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public partial class BugReportDialog : Form
	{
        private string programName, log;

		public BugReportDialog(string programName, string log)
		{
			InitializeComponent();
            this.programName = programName;
            this.log = log;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
            System.Diagnostics.Process.Start("https://github.com/sonicretro/sa_tools/issues");
            Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

        private void ErrorReportDialog_Load(object sender, EventArgs e)
		{
            StringBuilder text = new StringBuilder();
            text.Append("Program: ");
            text.AppendLine(programName);
            text.Append("Build Date: ");
            text.AppendLine(File.GetLastWriteTimeUtc(Application.ExecutablePath).ToString(CultureInfo.InvariantCulture));
            text.Append("OS Version: ");
			text.AppendLine(Environment.OSVersion.ToString());
            if (log != null)
            {
                text.AppendLine("Log:");
                text.AppendLine(log);
            }
            textBox1.Text = text.ToString();
		}
	}
}
