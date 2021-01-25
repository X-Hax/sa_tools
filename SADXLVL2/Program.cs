using System;
using System.IO;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXLVL2
{
	static class Program
	{
		internal static string[] args;
		public static MainForm primaryForm;

		private static string sadxGameFolder;
		public static string SADXGameFolder { get { return sadxGameFolder; } }

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

			if (args.Length == 0)
			{
				sadxGameFolder = "";
			}
			else
			{
				sadxGameFolder = args[1];
			}

			primaryForm = new MainForm();
			Application.Run(primaryForm);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (primaryForm != null)
				using (ErrorDialog ed = new ErrorDialog((Exception)e.ExceptionObject, false))
					ed.ShowDialog(primaryForm);
			else
			{
				System.IO.File.WriteAllText("SADXLVL2.log", e.ExceptionObject.ToString());
				MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SADXLVL2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}