using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon
{
    public partial class ErrorDialog : Form
    {
        private string programName, errorDescription, log;

        public ErrorDialog(string programName, string errorDescription, string log)
        {
            InitializeComponent();
            this.programName = programName;
            this.errorDescription = errorDescription;
            this.log = log;
        }

        static void UpdateClipboard(object info)
        {
            string text = (string)info;
            Clipboard.SetText(text);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Thread newThread = new Thread(new ParameterizedThreadStart(UpdateClipboard));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start(textBoxLog.Text);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/issues") { CreateNoWindow = true });
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ErrorReportDialog_Load(object sender, EventArgs e)
        {
            this.Text = programName + " Error";
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
            labelErrorDescription.Text = errorDescription;
            textBoxLog.Text = text.ToString();
        }
    }
}