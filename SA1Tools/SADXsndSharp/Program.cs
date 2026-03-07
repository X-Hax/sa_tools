using System;
using System.Windows.Forms;

namespace SADXsndSharp
{
	static class Program
	{
		public static string[] Arguments { get; private set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]

		static void Main(string[] args)
		{
			Arguments = args;
			Application.EnableVisualStyles();
			Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
