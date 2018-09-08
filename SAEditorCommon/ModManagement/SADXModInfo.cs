using System;
using System.Collections.Generic;
using System.IO;

namespace SonicRetro.SAModel.SAEditorCommon.ModManagement
{
	public class SADXModInfo : ModInfo
	{
		public string EXEFile { get; set; }
		public string EXEData { get; set; }
		public string ADV00MODELSData { get; set; }
		public string ADV01CMODELSData { get; set; }
		public string ADV01MODELSData { get; set; }
		public string ADV02MODELSData { get; set; }
		public string ADV03MODELSData { get; set; }
		public string BOSSCHAOS0MODELSData { get; set; }
		public string CHAOSTGGARDEN02MR_DAYTIMEData { get; set; }
		public string CHAOSTGGARDEN02MR_EVENINGData { get; set; }
		public string CHAOSTGGARDEN02MR_NIGHTData { get; set; }
		public string CHRMODELSData { get; set; }
		public bool RedirectMainSave { get; set; }
		public bool RedirectChaoSave { get; set; }

		public static new IEnumerable<string> GetModFiles(DirectoryInfo directoryInfo)
		{
			string modini = Path.Combine(directoryInfo.FullName, "mod.ini");
			if (File.Exists(modini))
			{
				yield return modini;
				yield break;
			}

			foreach (DirectoryInfo item in directoryInfo.GetDirectories())
			{
				if (item.Name.Equals("system", StringComparison.OrdinalIgnoreCase) || item.Name[0] == '.')
				{
					continue;
				}

				foreach (string filename in GetModFiles(item))
					yield return filename;
			}
		}
	}
}
