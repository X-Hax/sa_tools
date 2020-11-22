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
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			SAToolsHub toolsHub;

			toolsHub = new SAToolsHub();
			//Application.ThreadException += Application_ThreadException;
			Application.Run(toolsHub);
		}
	}
}
