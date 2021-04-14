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
		static internal string[] Arguments { get; set; }
		public static SAToolsHub toolsHub;

		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Arguments = args;
			toolsHub = new SAToolsHub();
			//Application.ThreadException += Application_ThreadException;
			Application.Run(toolsHub);
		}
	}
}
