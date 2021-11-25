using System;
using System.Windows.Forms;
using SAModel.SAEditorCommon.ProjectManagement;
using SplitTools;

namespace SADXTweaker2
{
	static class Program
	{
		internal static string[] args;
		private static MainForm FormInstance;
		internal static Templates.ProjectTemplate project;
		internal static IniData[] IniData;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Program.args = args;
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			Application.EnableVisualStyles();
			Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Application.SetCompatibleTextRenderingDefault(false);
			FormInstance = new MainForm();
			Application.Run(FormInstance);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (FormInstance != null)
			{
				Exception ex = (Exception)e.ExceptionObject;
				string errDesc = "SADXTweaker2 has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
	"If you wish to report a bug, please include the following in your report:";
				SAModel.SAEditorCommon.ErrorDialog report = new SAModel.SAEditorCommon.ErrorDialog("SADXTweaker2", errDesc, ex.ToString());
				DialogResult dgresult = report.ShowDialog(FormInstance);
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
				string logPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SADXTweaker2.log");
				System.IO.File.WriteAllText(logPath, e.ExceptionObject.ToString());
				MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath, "SADXTweaker2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
