using System;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	static class Program
	{
		internal static string[] args;
		private static MainForm FormInstance;
		internal static IniData IniData;

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
			{
				Exception ex = (Exception)e.ExceptionObject;
				string errDesc = "SADXTweaker2 has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
	"If you wish to report a bug, please include the following in your report:";
				SonicRetro.SAModel.SAEditorCommon.ErrorDialog report = new SonicRetro.SAModel.SAEditorCommon.ErrorDialog("SADXTweaker2", errDesc, ex.ToString());
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
				System.IO.File.WriteAllText("SADXTweaker2.log", e.ExceptionObject.ToString());
				MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SADXTweaker2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}