using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows.Forms;

namespace SAModel.SAMDL
{
	static class Program
	{
		static internal string[] Arguments { get; set; }
		public static MainForm primaryForm;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AssemblyLoadContext.Default.Resolving += ResolveAssembly;
            Arguments = args;
			Application.EnableVisualStyles();
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.SetCompatibleTextRenderingDefault(false);
			primaryForm = new MainForm();
			Application.Run(primaryForm);
		}

        private static Assembly ResolveAssembly(AssemblyLoadContext alc, AssemblyName assemblyName)
        {
            string probeSetting = AppContext.GetData("lib") as string;
            if (string.IsNullOrEmpty(probeSetting))
                return null;

            foreach (string subdirectory in probeSetting.Split(';'))
            {
                string pathMaybe = Path.Combine(AppContext.BaseDirectory, subdirectory, $"{assemblyName.Name}.dll");
                if (File.Exists(pathMaybe))
                    return alc.LoadFromAssemblyPath(pathMaybe);
            }

            return null;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (primaryForm != null)
			{
				Exception ex = (Exception)e.ExceptionObject;
				string errDesc = "SAMDL has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
					"If you wish to report a bug, please include the following in your report:";
				SAEditorCommon.ErrorDialog report = new SAEditorCommon.ErrorDialog("SAMDL", errDesc, ex.ToString());
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
				string logPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SAMDL.log";
				System.IO.File.WriteAllText(logPath, e.ExceptionObject.ToString());
				MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath + ".", "SAMDL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
