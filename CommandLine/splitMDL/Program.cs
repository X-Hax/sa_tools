using SA_Tools.SAArc;
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
			bool? be = null;
			if (argq.Count > 0)
				if (argq.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
				{
					be = true;
					argq.Dequeue();
				}
				else if (argq.Peek().Equals("/le", StringComparison.OrdinalIgnoreCase))
				{
					be = false;
					argq.Dequeue();
				}
			if (argq.Count > 0)
			{
				string filename = argq.Dequeue();
				Console.WriteLine("File: {0}", filename);
				sa2MDL.Split(be, filename, Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename)), argq.ToArray());
			}
			else
			{
				//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				Application.Run(new SplitMDLGUI());
			}
		}
	}
}