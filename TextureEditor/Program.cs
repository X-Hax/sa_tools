using System;
using System.IO;
using System.Windows.Forms;

namespace TextureEditor
{
	static class Program
	{
		public static string[] Arguments { get; private set; }
        public static MainForm primaryForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args)
		{
			Arguments = args;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            primaryForm = new MainForm();
            Application.Run(primaryForm);
		}

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (primaryForm != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                string log = "Texture Editor: New log entry on " + DateTime.Now.ToString("G") + System.Environment.NewLine + "Build date:" +
                    File.GetLastWriteTimeUtc(Application.ExecutablePath).ToString(System.Globalization.CultureInfo.InvariantCulture) + 
                    System.Environment.NewLine + ex.ToString();
                string errDesc = "Texture Editor has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
                    "If you wish to report a bug, please include the following in your report:";
                SonicRetro.SAModel.SAEditorCommon.ErrorDialog report = new SonicRetro.SAModel.SAEditorCommon.ErrorDialog("Texture Editor", errDesc, log);
                DialogResult dgresult = report.ShowDialog(primaryForm);
                switch (dgresult)
                {
                    case DialogResult.Abort:
                    case DialogResult.OK:
                        Application.Exit();
                        break;
                }
            }
            else
            {
                string logPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\TextureEditor.log";
                System.IO.File.WriteAllText(logPath, e.ExceptionObject.ToString());
                MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath + ".", "Texture Editor Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
