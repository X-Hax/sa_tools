using System;
using System.Windows.Forms;

namespace SplitMDL
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.Run(new SplitMDLGUI());
		}
	}
}