using SplitTools.SAArc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SplitMDL
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Queue<string> argq = new Queue<string>(args);
			if (argq.Count > 0)
			{
				string filename = argq.Dequeue();
				Console.WriteLine("File: {0}", filename);
				sa2MDL.Split(filename, Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename)), argq.ToArray());
			}
			else
			{
				//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				Application.EnableVisualStyles();
				Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
				Application.SetCompatibleTextRenderingDefault(false);

				Application.Run(new SplitMDLGUI());
			}
		}
	}
}
