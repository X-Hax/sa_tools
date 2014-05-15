using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweakerLite
{
    static class Program
    {
        internal static string[] args;
        private static MainForm FormInstance;
        internal static byte[] ExeData, CleanExe;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Program.args = args;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormInstance = new MainForm();
            Application.Run(FormInstance);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (FormInstance != null)
                using (ErrorDialog ed = new ErrorDialog((Exception)e.ExceptionObject, false))
                    ed.ShowDialog(FormInstance);
            else
            {
                System.IO.File.WriteAllText("SADXTweakerLite.log", e.ExceptionObject.ToString());
                MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SADXTweakerLite Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}