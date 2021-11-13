using System;
using System.Windows.Forms;

namespace SA2EventViewer
{
	static class Program
	{
		static internal string[] Arguments { get; set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Arguments = args;
			Application.EnableVisualStyles();
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
