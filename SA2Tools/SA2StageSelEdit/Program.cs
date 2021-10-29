using System;
using System.Windows.Forms;
using System.IO;

namespace SA2StageSelEdit
{
	static class Program
	{
		static internal string sapFile;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length > 0)
				if (args[0].Contains(".sap"))
					sapFile = Path.GetFullPath(args[0]);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
