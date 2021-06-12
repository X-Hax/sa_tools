using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using SA_Tools;

namespace SAEditorCommon.ProjectManagement
{
	public class Templates
	{
		public class SplitTemplate
		{
			[XmlElement("GameInfo", typeof(SplitInfo))]
			public SplitInfo GameInfo { get; set; }
			[XmlElement("SplitEntry", typeof(SplitEntry))]
			public List<SplitEntry> SplitEntries { get; set; }
			[XmlElement("SplitEntryMDL", typeof(SplitEntryMDL))]
			public List<SplitEntryMDL> SplitMDLEntries { get; set; }
		}

		public class ProjectTemplate
		{
			[XmlElement("GameInfo", typeof(ProjectInfo))]
			public ProjectInfo GameInfo { get; set; }
			[XmlElement("SplitEntry", typeof(SplitEntry))]
			public List<SplitEntry> SplitEntries { get; set; }
			[XmlElement("SplitEntryMDL", typeof(SplitEntryMDL))]
			public List<SplitEntryMDL> SplitMDLEntries { get; set; }
		}

		public class SplitInfo
		{
			[XmlAttribute("gameName")]
			public string GameName { get; set; }
			[XmlAttribute("dataFolder")]
			public string DataFolder { get; set; }
			[XmlAttribute("checkFile")]
			public string CheckFile { get; set; }
		}

		public class ProjectInfo
		{
			[XmlAttribute("gameName")]
			public string GameName { get; set; }
			[XmlAttribute("gameSystemFolder")]
			public string GameSystemFolder { get; set; }
			[XmlAttribute("modSystemFolder")]
			public string ModSystemFolder { get; set; }
			[XmlAttribute("canBuild")]
			public bool CanBuild { get; set; }
		}

		public class SplitEntry
		{
			[XmlAttribute("SourceFile")]
			public string SourceFile { get; set; }
			[XmlAttribute("IniFile")]
			public string IniFile { get; set; }
			[XmlAttribute("CmnName")]
			public string CmnName { get; set; }
		}

		public class SplitEntryMDL
		{
			[XmlAttribute("BigEndian")]
			public bool BigEndian { get; set; }
			[XmlAttribute("ModelFile")]
			public string ModelFile { get; set; }
			[XmlElement("MotionFile")]
			public List<string> MotionFiles { get; set; }
		}
	}

	public class ProjectSettings
	{
		public enum Frequency
		{
			daily,
			weekly,
			monthly
		}

		public enum Themes
		{
			light,
			dark
		}

		public bool AutoUpdate { get; set; }
		public Frequency UpdateFrequency { get; set; }
		public Themes HubTheme { get; set; }

		public static ProjectSettings Load(string iniPath)
		{
			if (File.Exists(iniPath))
			{
				return (ProjectSettings)IniSerializer.Deserialize(typeof(ProjectSettings), iniPath);
			}
			else
			{
				ProjectSettings result = new ProjectSettings()
				{
					AutoUpdate = false,
					UpdateFrequency = Frequency.monthly,
					HubTheme = Themes.light,
				};

				return result;
			}
		}
		public static ProjectSettings Load()
		{
			return Load(GetSettingsPath());
		}

		private static string GetSettingsPath()
		{
			return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Settings.ini");
		}

		public void Save()
		{
			IniSerializer.Serialize(this, GetSettingsPath());
		}
	}

	public class ProjectFunctions
	{
		private static bool checkFileHashes(string game, string checkFileHash)
		{
			List<string> hashes = new List<string>();
			switch (game)
			{
				case "SA1":
					hashes.Add("060cad2ceefc07c7429085f30a356046");
					break;
				case "SA2":
					hashes.Add("1c1b63fcb551187e7c7b456b4e28a022");
					hashes.Add("ef0138133ff61fca7075fd520abb7618");
					break;
				case "SA2TT":
					hashes.Add("f3d6cf600af7d8daf156eee220225379");
					break;
				case "SA1AD":
					hashes.Add("fcb1da8942278871136e41e127ce979b");
					break;
				case "SADXGC":
					hashes.Add("036e39da11f19e6e3598ae6384dcae14");
					break;
				case "SADX360":
					hashes.Add("e1f01f48442cf711e2206370c60b7218");
					break;
				case "SADXPC":
					hashes.Add("c6d65712475602252bfce53d0d8b7d6f");
					hashes.Add("4d9b59aea4ee361e4f25475df1447bfd");
					hashes.Add("3cd8ce57f5e1946762e7526a429e572f");
					hashes.Add("e46580fc390285174acae895a90c5c84");
					hashes.Add("b215d5dfc16514c0cc354449c101c7d0");
					hashes.Add("f8c0b356519d7c459f7b726d63462791");
					hashes.Add("a35eb183684e3eb964351de391db82e8");
					break;
				case "SA2PC":
					hashes.Add("4f03dff9b720986cd922ab461d2a6c69");
					hashes.Add("1c28ae7958300273484a32923788a060");
					break;
			}

			if (!hashes.Any(h => h.Equals(checkFileHash, StringComparison.OrdinalIgnoreCase)))
			{
				return false;
			}
			else
				return true;
		}
		
		/// <summary>
		/// Uses a path string to open and deserialize a Sonic Adventure Project file.
		/// </summary>
		/// <returns>ProjectTemplate File</returns>
		public static Templates.ProjectTemplate openProjectFileString(string fileName)
		{
			Templates.ProjectTemplate projectFile;

			if (fileName != null)
			{
				var projFileSerializer = new XmlSerializer(typeof(Templates.ProjectTemplate));
				var projFileStream = File.OpenRead(fileName);
				projectFile = (Templates.ProjectTemplate)projFileSerializer.Deserialize(projFileStream);
				return projectFile;
			}
			else
				return null;
		}

		/// <summary>
		/// Opens and Deserializes a Sonic Adventure Project file.
		/// </summary>
		/// <returns>ProjectTemplate File</returns>
		public static Templates.ProjectTemplate openProjectFile()
		{
			Templates.ProjectTemplate projectFile;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "Project File (*.sap)|*.sap";
			openFileDialog1.RestoreDirectory = true;

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				projectFile = openProjectFileString(openFileDialog1.FileName);
				return projectFile;
			}
			else
				return null;
		}

		/// <summary>
		/// Opens a Project Template file to begin the game data split. Asks and saves game directory to template if one does not exist.
		/// </summary>
		/// <returns>SplitTemplate file</returns>
		public static Templates.SplitTemplate openTemplateFile(string templateFilePath)
		{
			Templates.SplitTemplate templateFile;

			var templateFileSerializer = new XmlSerializer(typeof(Templates.SplitTemplate));
			var templateFileStream = File.OpenRead(templateFilePath);
			templateFile = (Templates.SplitTemplate)templateFileSerializer.Deserialize(templateFileStream);
			templateFileStream.Close();

			if (GetGamePath(templateFile.GameInfo.GameName) == "")
			{
				DialogResult gamePathWarning = MessageBox.Show(("A game path has not been supplied for this template.\n\nPlease select a valid game path containing this file: " + templateFile.GameInfo.CheckFile + ".\n\nPress OK to select a valid path for " + templateFile.GameInfo.GameName + "."), "Game Path Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				if (gamePathWarning == DialogResult.OK)
				{
					FolderSelectionNew:
					var fsd = new FolderSelect.FolderSelectDialog();
					fsd.Title = "Please select path for " + templateFile.GameInfo.GameName;
					fsd.ShowDialog();
					if (Directory.Exists(fsd.FileName))
					{
						string checkFile = Path.Combine(fsd.FileName, templateFile.GameInfo.CheckFile);
						if (File.Exists(checkFile))
						{
							string checkFileHash = HelperFunctions.FileHash(checkFile);
							if (checkFileHashes(templateFile.GameInfo.GameName, checkFileHash) == true)
							{
								TextWriter splitsWriter = File.CreateText(templateFilePath);
                                SetGamePath(templateFile.GameInfo.GameName, fsd.FileName);
								templateFileSerializer.Serialize(splitsWriter, templateFile);
								
								return templateFile;
							}
							else
							{
								DialogResult pathWarning = MessageBox.Show(("Check file " + templateFile.GameInfo.CheckFile + " is not correct for the template select.\n\n"), "Incorrect Game Version", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
								if (pathWarning == DialogResult.Retry)
								{
									goto FolderSelectionNew;
								}
								else
									return null;
							}
							
						}
						else
						{
							DialogResult pathWarning = MessageBox.Show(("Check file " + templateFile.GameInfo.CheckFile + " was not located in the supplied Directory."), "Check File Not Found", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
							if (pathWarning == DialogResult.Retry)
							{
								goto FolderSelectionNew;
							}
							else
								return null;
						}
					}
					else
					{
						DialogResult pathWarning = MessageBox.Show(("No path was supplied."), "No Path Supplied", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
						if (pathWarning == DialogResult.Retry)
						{
							goto FolderSelectionNew;
						}
						else
							return null;
					}
				}
				else
					return null;
			}
			else if (!Directory.Exists(GetGamePath(templateFile.GameInfo.GameName)))
			{
				DialogResult gamePathWarning = MessageBox.Show(("The folder for " + templateFile.GameInfo.GameName + " does not exist.\n\nPlease press OK and select the correct path for " + templateFile.GameInfo.GameName + "."), "Game Path Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				if (gamePathWarning == DialogResult.OK)
				{
				FolderSelectionMissing:
					var fsd = new FolderSelect.FolderSelectDialog();
					fsd.Title = "Please select path for " + templateFile.GameInfo.GameName;
					fsd.ShowDialog();
					if (Directory.Exists(fsd.FileName))
					{
						string checkFile = Path.Combine(fsd.FileName, templateFile.GameInfo.CheckFile);
						if (File.Exists(checkFile))
						{
							string checkFileHash = HelperFunctions.FileHash(checkFile);
							if (checkFileHashes(templateFile.GameInfo.GameName, checkFileHash) == true)
							{
                                SetGamePath(templateFile.GameInfo.GameName, fsd.FileName);
								return templateFile;
							}
							else
							{
								DialogResult pathWarning = MessageBox.Show(("Check file " + templateFile.GameInfo.CheckFile + " is not correct for the template select.\n\n"), "Incorrect Game Version", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
								if (pathWarning == DialogResult.Retry)
								{
									goto FolderSelectionMissing;
								}
								else
									return null;
							}

						}
						else
						{
							DialogResult pathWarning = MessageBox.Show(("Check file " + templateFile.GameInfo.CheckFile + " was not located in the supplied Directory."), "Check File Not Found", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
							if (pathWarning == DialogResult.Retry)
							{
								goto FolderSelectionMissing;
							}
							else
								return null;
						}
					}
					else
					{
						DialogResult pathWarning = MessageBox.Show(("No path was supplied."), "No Path Supplied", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
						if (pathWarning == DialogResult.Retry)
						{
							goto FolderSelectionMissing;
						}
						else
							return null;
					}
				}
				else
					return null;
			}
			else
				return templateFile;
		}

        /// <summary>
        /// Gets the path for a specified game from GamePaths.ini.
        /// </summary>
        /// <returns>Path string</returns>
        public static string GetGamePath(string gameName)
        {
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            // Release mode
            string gamePathsFile = Path.Combine(appPath, "GameConfig", "GamePaths.ini");
            if (File.Exists(gamePathsFile))
                goto getpath;
            // Visual Studio mode
            else
            {
                gamePathsFile = Path.Combine(appPath, "..\\GameConfig", "GamePaths.ini");
                if (File.Exists(gamePathsFile))
                    goto getpath;
            }
            return "";
        getpath:
            Dictionary<string, string> gamePathsList = IniSerializer.Deserialize<Dictionary<string, string>>(gamePathsFile);
            return gamePathsList.ContainsKey(gameName) ? gamePathsList[gameName] : "";
        }

        /// <summary>
        /// Sets the path for a specified game in GamePaths.ini.
        /// </summary>
        public static void SetGamePath(string gameName, string gamePath)
        {
            Dictionary<string, string> gamePathsList;
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            string gamePathsFilePath;
            // Release mode
            if (Directory.Exists(Path.Combine(appPath, "GameConfig")))
                gamePathsFilePath = Path.Combine(appPath, "GameConfig", "GamePaths.ini");
            // Visual Studio mode
            else
                gamePathsFilePath = Path.Combine(appPath, "..\\GameConfig", "GamePaths.ini");
            if (File.Exists(gamePathsFilePath))
                gamePathsList = IniSerializer.Deserialize<Dictionary<string, string>>(gamePathsFilePath);
            else 
                gamePathsList = new Dictionary<string, string>();
            if (gamePathsList.ContainsKey(gameName))
                gamePathsList[gameName] = gamePath;
            else
                gamePathsList.Add(gameName, gamePath);
            IniSerializer.Serialize(gamePathsList, gamePathsFilePath);
        }

        /// <summary>
        /// Returns a file path in the mod folder or in the game's fallback folder.
        /// </summary>
        public static string ModPathOrGameFallback(string path, string fallbackPath)
        {
            return (File.Exists(path)) ? path : fallbackPath;
        }
    }
}
