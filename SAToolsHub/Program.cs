using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon;
using Fclp;

namespace SAToolsHub
{
	static class Program
	{
		private static ProjectManagement.ProjectSettings settings;
		public static ProjectManagement.ProjectSettings Settings { get { return settings; } }

		public static bool AnyGamesConfigured()
		{
			// sadx validity check
			string sadxpcfailreason = "";
			bool sadxPCIsValid = GamePathChecker.CheckSADXPCValid(settings.SADXPCPath, out sadxpcfailreason);

			// sa2 valididty check next
			string sa2pcFailReason = "";
			bool sa2PCIsValid = GamePathChecker.CheckSA2PCValid(settings.SA2PCPath, out sa2pcFailReason);

			return sadxPCIsValid || sa2PCIsValid;
		}

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			SAToolsHub toolsHub;

			settings = ProjectManagement.ProjectSettings.Load();

			if (!AnyGamesConfigured())
			{
				toolSettings toolSettings = new toolSettings();
				DialogResult startConfig = toolSettings.ShowDialog();
			}

			toolsHub = new SAToolsHub();
			//Application.ThreadException += Application_ThreadException;
			Application.Run(toolsHub);
		}
	}
}
