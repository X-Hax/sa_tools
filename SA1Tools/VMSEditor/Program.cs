using System;
using System.IO;
using System.Windows.Forms;

namespace VMSEditor
{
	static partial class Program
    {
        internal static string[] args;
        public static Form primaryForm;

        public static Form CheckFile(string filepath)
        {
            switch (Path.GetExtension(filepath).ToLowerInvariant())
            {
                case ".vmi":
                    return new EditorVMI();
                case ".vms":
                default:
                    byte[] file = File.ReadAllBytes(filepath);
                    // Garden save or upload file
                    if (System.Text.Encoding.GetEncoding(932).GetString(file, 0, 4) == "CHAO" || System.Text.Encoding.GetEncoding(932).GetString(file, 0, 6) == "A-LIFE")
                        return new EditorChao();
                    // Download Data / Chao Adventure
                    else
                        for (int u = 0; u < file.Length - 11; u++)
                            if (System.Text.Encoding.GetEncoding(932).GetString(file, u, 11) == "CHAO ACCEPT")
                                return new EditorChao();
                    // If none of the above is found, run the DLC editor
                   return new EditorDLC();
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Program.args = args;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.SetCompatibleTextRenderingDefault(false);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // No arguments: Select program mode
            if (args.Length == 0)
                primaryForm = new ProgramModeSelector();
            else
                primaryForm = CheckFile(args[0]);
            Application.Run(primaryForm);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (primaryForm != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                string errDesc = "VMS Editor has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
                    "If you wish to report a bug, please include the following in your report:";
				SAModel.SAEditorCommon.ErrorDialog report = new SAModel.SAEditorCommon.ErrorDialog("VMS Editor", errDesc, ex.ToString());
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
                string logPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\VMSEditor.log";
                System.IO.File.WriteAllText(logPath, e.ExceptionObject.ToString());
                MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath + ".", "VMS Editor Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
