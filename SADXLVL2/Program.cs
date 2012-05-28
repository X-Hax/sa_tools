using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXLVL2
{
    static class Program
    {
        internal static string[] args;

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
            Application.Run(new MainForm());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (LevelData.MainForm != null)
                using (ErrorDialog ed = new ErrorDialog((Exception)e.ExceptionObject, false))
                    ed.ShowDialog(LevelData.MainForm);
            else
            {
                System.IO.File.WriteAllText("SADXLVL2.log", e.ExceptionObject.ToString());
                MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SADXLVL2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}