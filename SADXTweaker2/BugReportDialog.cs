using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SADXTweaker2
{
    public partial class BugReportDialog : Form
    {
        public BugReportDialog()
        {
            InitializeComponent();
        }

        public BugReportDialog(Exception ex)
            : this()
        {
            exception = ex;
        }

        private Exception exception;
        private static DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public int unixtime(DateTime t) { return (int)Math.Floor((t.ToUniversalTime() - origin).TotalSeconds); }

        private DateTime t, build;
        private void okButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show(this, "Additional Info cannot be empty!");
                return;
            }
            HttpWebRequest r = (HttpWebRequest)HttpWebRequest.Create("http://mm.reimuhakurei.net/bugreport.php");
            r.Method = "POST";
            r.ServicePoint.Expect100Continue = false;
            byte[] byte1 = Encoding.UTF8.GetBytes("ProgramName=" + Uri.EscapeDataString(ProgramName.Text).Replace("%20", "+")
                + "&BuildDate=" + Uri.EscapeDataString(unixtime(build).ToString(NumberFormatInfo.InvariantInfo)).Replace("%20", "+")
                + "&Time=" + Uri.EscapeDataString(unixtime(t).ToString(NumberFormatInfo.InvariantInfo)).Replace("%20", "+")
                + "&OSVersion=" + Uri.EscapeDataString(OSVersion.Text).Replace("%20", "+")
                + "&Username=" + Uri.EscapeDataString(Username.Text).Replace("%20", "+")
                + "&Log=" + Uri.EscapeDataString(Log.Text).Replace("%20", "+")
                + "&AdditionalInfo=" + Uri.EscapeDataString(textBox1.Text).Replace("%20", "+"));
            r.ContentType = "application/x-www-form-urlencoded";
            r.ContentLength = byte1.Length;
            Stream rs = r.GetRequestStream();
            rs.Write(byte1, 0, byte1.Length);
            rs.Close();
            try
            {
                HttpWebResponse resp = (HttpWebResponse)r.GetResponse();
                rs = resp.GetResponseStream();
                StreamReader sr = new StreamReader(rs, Encoding.UTF8);
                string str = sr.ReadLine();
                if (int.Parse(str, NumberStyles.Integer, NumberFormatInfo.InvariantInfo) == 0)
                {
                    if (MessageBox.Show(this, "Bug report submitted successfully!\n\nWould you like to go to the bug page?", "SADXTweaker2 Bug Report", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                        System.Diagnostics.Process.Start("http://mm.reimuhakurei.net/?page=bugs&id=" + sr.ReadLine());
                }
                else
                    MessageBox.Show(this, "Bug report failed!\n\n" + sr.ReadToEnd(), "SADXTweaker2 Bug Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sr.Close();
                rs.Close();
            }
            catch (WebException ex)
            {
                MessageBox.Show(this, ex.Message);
            }
            Properties.Settings.Default.Username = Username.Text;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BugReportDialog_Load(object sender, EventArgs e)
        {
            build = File.GetLastWriteTimeUtc(Application.ExecutablePath);
            BuildDate.Text = build.ToString(CultureInfo.InvariantCulture);
            t = DateTime.UtcNow;
            Time.Text = t.ToString(CultureInfo.InvariantCulture);
            OSVersion.Text = Environment.OSVersion.ToString();
            if (string.IsNullOrEmpty(Properties.Settings.Default.Username))
                Username.Text = Environment.MachineName;
            else
                Username.Text = Properties.Settings.Default.Username;
            Log.Text = (exception == null ? string.Empty : exception.ToString());
        }
    }
}