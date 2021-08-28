using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using SAToolsHub.Updater;

namespace SAToolsHub
{

    static class Program
    {
		private const string pipeName = "sa-tools";

		static internal string[] Arguments { get; set; }
        public static SAToolsHub toolsHub;
		private static readonly Mutex mutex = new Mutex(true, pipeName);

		[STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			bool alreadyRunning;
			try { alreadyRunning = !mutex.WaitOne(0, true); }
			catch (AbandonedMutexException) { alreadyRunning = false; }

			if (args.Length > 1 && args[0] == "doupdate")
			{
				if (alreadyRunning)
					try { mutex.WaitOne(); }
					catch (AbandonedMutexException) { }
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new LoaderManifestDialog(args[1]));
				return;
			}

			if (args.Length > 1 && args[0] == "cleanupdate")
			{
				if (alreadyRunning)
					try { mutex.WaitOne(); }
					catch (AbandonedMutexException) { }
				alreadyRunning = false;
				Thread.Sleep(1000);
				try
				{
					File.Delete(args[1] + ".7z");
					Directory.Delete(args[1], true);
				}
				catch { }
			}
			
			if (alreadyRunning)
			{
				return;
			}

			Arguments = args;

			Application.Run(new SAToolsHub());
		}
    }
}
