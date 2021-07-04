using System;
using System.IO;
using System.Windows.Forms;

namespace PLTool
{
    static class Program
    {
        static internal string[] Arguments { get; set; }
        public static Form primaryForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Arguments = args;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0 && File.Exists(args[0]))
                switch (Path.GetFileNameWithoutExtension(args[0]).Substring(0, 2).ToUpperInvariant())
                {
                    case "SL":
                        primaryForm = new SLEditor(args[0]);
                        break;
                    case "PL":
                    default:
                        primaryForm = new PLEditor();
                        break;
                }
            Application.Run(primaryForm);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (primaryForm != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                string errDesc = "PL Tool has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
                    "If you wish to report a bug, please include the following in your report:";
                SAModel.SAEditorCommon.ErrorDialog report = new SAModel.SAEditorCommon.ErrorDialog("PLTool", errDesc, ex.ToString());
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
                string logPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\PLTool.log";
                System.IO.File.WriteAllText(logPath, e.ExceptionObject.ToString());
                MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath + ".", "PLTool Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
