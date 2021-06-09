using System;
using System.IO;
using System.Windows.Forms;

namespace DLCTool
{
	static partial class Program
    {
        internal static string[] args;
        public static MainForm primaryForm;

        [STAThread]
        static void Main(string[] args)
        {
            Program.args = args;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            primaryForm = new MainForm();
            Application.Run(primaryForm);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (primaryForm != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                string errDesc = "DLC Tool has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
                    "If you wish to report a bug, please include the following in your report:";
				SonicRetro.SAModel.SAEditorCommon.ErrorDialog report = new SonicRetro.SAModel.SAEditorCommon.ErrorDialog("DLC Tool", errDesc, ex.ToString());
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
                string logPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\DLCTool.log";
                System.IO.File.WriteAllText(logPath, e.ExceptionObject.ToString());
                MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath + ".", "DLC Tool Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}