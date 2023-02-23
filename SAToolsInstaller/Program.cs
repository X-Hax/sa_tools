using NetCoreInstallChecker;
using NetCoreInstallChecker.Structs.Config;
using NetCoreInstallChecker.Structs.Config.Enum;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SAToolsInstaller
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (!Directory.Exists(".updates"))
				Directory.CreateDirectory(".updates");
			var finder = new FrameworkFinder(Environment.Is64BitOperatingSystem);
			var resolver = new DependencyResolver(finder);
			var framework = new Framework("Microsoft.WindowsDesktop.App", "7.0.3");
			var options = new RuntimeOptions("net7.0", framework, RollForwardPolicy.Minor);
			var result = resolver.Resolve(options);

			// Check if dependencies are missing.
			if (!result.Available)
			{
				if (MessageBox.Show(".NET 7.0 Desktop Runtime is not installed! Would you like to install it now?", "SA Tools Installer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				{
					MessageBox.Show("SA Tools cannot be installed without .NET 7.", "SA Tools Installer", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				FrameworkDownloader frameworkDownloader = new FrameworkDownloader(framework.NuGetVersion, framework.FrameworkName);
				string url = frameworkDownloader.GetDownloadUrlAsync(Environment.Is64BitOperatingSystem ? Architecture.Amd64 : Architecture.x86).GetAwaiter().GetResult();
				SAToolsHub.Updater.UpdaterWebClient client = new SAToolsHub.Updater.UpdaterWebClient();
				client.DownloadFile(url, "NET7Install.exe");
				Process.Start(new ProcessStartInfo("NET7Install.exe", "/install /passive /norestart") { UseShellExecute = true, Verb = "runas" }).WaitForExit();
			}
			SAToolsHub.Updater.LoaderDownloadDialog mainForm = new SAToolsHub.Updater.LoaderDownloadDialog(Environment.Is64BitOperatingSystem ? "http://mm.reimuhakurei.net/SA%20Tools%20x64.7z" : "http://mm.reimuhakurei.net/SA%20Tools%20x86.7z", ".updates");
			Application.Run(mainForm);
		}
	}
}
