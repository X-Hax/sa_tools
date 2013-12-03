using System;
using System.Windows.Forms;

namespace SADXTweakerLite
{
    public partial class ErrorDialog : Form
    {
        private Exception exception;

        public ErrorDialog(Exception ex, bool allowContinue)
        {
            InitializeComponent();
            exception = ex;
            label1.Text = "Unhandled Exception " + ex.GetType().Name + "\n" + ex.Message;
            okButton.Enabled = allowContinue;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (BugReportDialog err = new BugReportDialog(exception))
                err.ShowDialog(Owner);
        }
    }
}