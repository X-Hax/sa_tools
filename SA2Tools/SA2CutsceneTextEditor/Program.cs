using System;
using System.Windows.Forms;

namespace SA2CutsceneTextEditor
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
			Application.SetCompatibleTextRenderingDefault(false);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Application.Run(new MainForm(args));
		}
	}
}
