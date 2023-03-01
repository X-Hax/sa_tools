﻿using System.Collections.Generic;
using System.IO;
using SplitTools;

namespace SAModel.SAEditorCommon.ModManagement
{
	public class ModInfo
	{
		public string Name { get; set; }
		public string Author { get; set; }
		public string Version { get; set; }
		public string Category { get; set; }
		public string Description { get; set; }
		public string DLLFile { get; set; }
		public string Codes { get; set; }
		public string GitHubRepo { get; set; }
		public string GitHubAsset { get; set; }
		public string UpdateUrl { get; set; }
		public string ChangelogUrl { get; set; }
		public string GameBananaItemType { get; set; }
		public long? GameBananaItemId { get; set; }
		public string ModID { get; set; }
		[IniName("Dependency")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public List<string> Dependencies { get; set; }

		public static IEnumerable<string> GetModFiles(DirectoryInfo directoryInfo)
		{
			string modini = Path.Combine(directoryInfo.FullName, "mod.ini");
			if (File.Exists(modini))
			{
				yield return modini;
				yield break;
			}

			foreach (DirectoryInfo item in directoryInfo.GetDirectories())
			{
				if (item.Name[0] == '.')
				{
					continue;
				}

				foreach (string filename in GetModFiles(item))
					yield return filename;
			}
		}
	}
}
