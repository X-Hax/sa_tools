using System;
using System.IO;
using SonicRetro.SAModel.GC;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAMDL
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
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
