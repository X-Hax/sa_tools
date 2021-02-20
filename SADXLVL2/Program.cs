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

			// check for our game folder validity.
			// if it isn't set up, let the user know that they need to run and configure project maanger
			// before continuing

			string projectManagerPath = "";

			projectManagerPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Settings.ini");

			ProjectManager.ProjectManagerSettings settings = ProjectManager.ProjectManagerSettings.Load(projectManagerPath);

			string sadxGamePathInvalidReason = "";

			if (args.Length == 0 && !SAEditorCommon.GamePathChecker.CheckSADXPCValid(settings.SADXPCPath, out sadxGamePathInvalidReason))
			{
				sadxGameFolder = "";
			}
			else sadxGameFolder = settings.SADXPCPath;

			primaryForm = new MainForm();
			Application.Run(primaryForm);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (primaryForm != null)
			{
				Exception ex = (Exception)e.ExceptionObject;
				string errDesc = "SADXLVL2 has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
					"If you wish to report a bug, please include the following in your report:";
				SAEditorCommon.ErrorDialog report = new SAEditorCommon.ErrorDialog("SADXLVL2", errDesc, ex.ToString());
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
				string logPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SADXLVL2.log";
				System.IO.File.WriteAllText(logPath, e.ExceptionObject.ToString());
				MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath + ".", "SADXLVL2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}