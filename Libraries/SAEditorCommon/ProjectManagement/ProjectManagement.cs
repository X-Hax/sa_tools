using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using SplitTools;

namespace SAEditorCommon.ProjectManagement
{
	public class Templates
	{
		/// <summary>
		/// Full schema for the Sonic Adventure templates used when creating Project files.
		/// </summary>
		public class SplitTemplate
		{
			/// <summary>
			/// Stores the game information for splitting data.
			/// </summary>
			[XmlElement("GameInfo", typeof(SplitInfo))]
			public SplitInfo GameInfo { get; set; }
			/// <summary>
			/// Ini data map files used for splitting data from the games.
			/// </summary>
			[XmlElement("SplitEntry", typeof(SplitEntry))]
			public List<SplitEntry> SplitEntries { get; set; }
			/// <summary>
			/// Model and Motion archive entries, only in SA2 formatted templates.
			/// </summary>
			[XmlElement("SplitEntryMDL", typeof(SplitEntryMDL))]
			public List<SplitEntryMDL> SplitMDLEntries { get; set; }
		}

		/// <summary>
		/// Full schema for the Sonic Adventure Project (*.sap) files.
		/// </summary>
		public class ProjectTemplate
		{
			/// <summary>
			/// Stores the project information.
			/// </summary>
			[XmlElement("GameInfo", typeof(ProjectInfo))]
			public ProjectInfo GameInfo { get; set; }
			/// <summary>
			/// Data map files used in the creation of the Project file.
			/// </summary>
			[XmlElement("SplitEntry", typeof(SplitEntry))]
			public List<SplitEntry> SplitEntries { get; set; }
			/// <summary>
			/// Model and Motion archive entries, only in SA2 formatted projects.
			/// </summary>
			[XmlElement("SplitEntryMDL", typeof(SplitEntryMDL))]
			public List<SplitEntryMDL> SplitMDLEntries { get; set; }
		}

		/// <summary>
		/// <para>
		/// Sonic Adventure Template Information class. Stores the: 
		/// </para>
		/// <para>
		/// The game's name.
		/// </para>
		/// <para>
		/// The directory for the split ini files in the SA Tools;
		/// </para>
		/// <para>
		/// A file to compare to known hashes to verify the game;
		/// </para>
		/// </summary>
		public class SplitInfo
		{
			/// <summary>
			/// Name of the game the template is for.
			/// </summary>
			[XmlAttribute("gameName")]
			public string GameName { get; set; }
			/// <summary>
			/// Data folder containing the Ini Data Mapping files within SA Tools.
			/// </summary>
			[XmlAttribute("dataFolder")]
			public string DataFolder { get; set; }
            /// <summary>
			/// Game data folder name, e.g. 'SONICADV', 'resource/gd_PC' or 'system'.
			/// </summary>
			[XmlAttribute("gameDataFolderName")]
            public string GameDataFolderName { get; set; }
            /// <summary>
            /// The filename to be checked against verified hashes to verify the game being used.
            /// </summary>
            [XmlAttribute("checkFile")]
			public string CheckFile { get; set; }
            /// <summary>
			/// MD5 hashes (comma-separated) to be checked against to verify the game being used.
			/// </summary>
			[XmlAttribute("checkHashes")]
            public string CheckHashes { get; set; }
            /// <summary>
            /// Bool to check if the levels in the project can be opened in SALVL project mode.
            /// </summary>
            [XmlAttribute("canUseSALVL")]
            public bool CanUseSALVL { get; set; }
        }

		/// <summary>
		/// <para>
		/// Sonic Adventure Project (*.sap) Information class. Stores the:
		/// </para>
		/// <para>
		/// Game's name;
		/// </para>
		/// <para>
		/// Directory of the game installation;
		/// </para>
		/// <para>
		/// Folder used by the game for its files;
		/// </para>
		/// <para>
		/// Directory of the project;
		/// </para>
		/// <para>
		/// Bool for if the game can be built;
		/// </para>
		/// </summary>
		public class ProjectInfo
		{
			/// <summary>
			/// Game name for the project stored in the *.sap file.
			/// </summary>
			[XmlAttribute("gameName")]
			public string GameName { get; set; }
            /// <summary>
			/// Game main executable (same as checkFile in game template)
			/// </summary>
			[XmlAttribute("checkFile")]
            public string CheckFile { get; set; }
            /// <summary>
            /// The directory for the main game stored in the *.sap file.
            /// </summary>
            [XmlAttribute("gameFolder")]
			public string GameFolder { get; set; } // The game's main folder, e.g. SONICADVENTUREDX
			/// <summary>
			/// The file folder used by the game stored in the *.sap file. e.g. system, gd_PC, etc
			/// </summary>
            [XmlAttribute("gameDataFolder")]
            public string GameDataFolder { get; set; } // The game's 'system' folder, e.g. SONICADV or system
			/// <summary>
			/// The directory of the files for the project stored in the *.sap file.
			/// </summary>
            [XmlAttribute("projectFolder")]
			public string ProjectFolder { get; set; }
			/// <summary>
			/// Bool for if the project can be built or not stored in the *.sap file.
			/// </summary>
			[XmlAttribute("canBuild")]
			public bool CanBuild { get; set; }
            /// <summary>
            /// Bool to check if the levels in the project can be opened in SALVL project mode.
            /// </summary>
            [XmlAttribute("canUseSALVL")]
            public bool CanUseSALVL { get; set; }
        }

		/// <summary>
		/// Stores names for the source file, data file, and a common name for processing data to be split.
		/// </summary>
		public class SplitEntry
		{
			/// <summary>
			/// Input file to be split from.
			/// </summary>
			[XmlAttribute("SourceFile")]
			public string SourceFile { get; set; }
			/// <summary>
			/// Ini Data Mapping file to be used on the source file.
			/// </summary>
			[XmlAttribute("IniFile")]
			public string IniFile { get; set; }
			/// <summary>
			/// Common name to be referenced for the general contents of what will be split.
			/// </summary>
			[XmlAttribute("CmnName")]
			public string CmnName { get; set; }
		}

		/// <summary>
		/// Stores information on SA2 Model and Motion archives for splitting.
		/// </summary>
		public class SplitEntryMDL
		{
			/// <summary>
			/// Sets if files are big endian or little endian.
			/// </summary>
			[XmlAttribute("BigEndian")]
			public bool BigEndian { get; set; }
			/// <summary>
			/// Model Archive filename.
			/// </summary>
			[XmlAttribute("ModelFile")]
			public string ModelFile { get; set; }
			/// <summary>
			/// List of Motion files uses by the Model File.
			/// </summary>
			[XmlElement("MotionFile")]
			public List<string> MotionFiles { get; set; }
		}
	}

	public class ProjectSettings
	{
		public enum UpdateUnits
		{
			Always,
			Hours,
			Days,
			Weeks,
		}

		public enum Themes
		{
			light,
			dark
		}

		[DefaultValue(true)]
		public bool UpdateCheck { get; set; } = true;
		[DefaultValue(UpdateUnits.Weeks)]
		public UpdateUnits UpdateUnit { get; set; } = UpdateUnits.Weeks;
		public Themes HubTheme { get; set; }
		[DefaultValue(1)]
		public int UpdateFrequency { get; set; } = 1;

		[DefaultValue(0)] public long UpdateTime { get; set; }

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
					UpdateCheck = false,
					UpdateUnit = UpdateUnits.Weeks,
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
        /// <summary>
		/// Checks a file's MD5 hash against a comma-separated list of known hashes.
		/// </summary>
		/// <returns>True if a match is found</returns>
		private static bool checkFileHashes(string gameHashes, string checkFileHash)
		{
            string[] hashes = gameHashes.Split(',');
			
			if (!hashes.Any(h => h.Equals(checkFileHash, StringComparison.OrdinalIgnoreCase)))
				return false;
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
        /// Opens a Project Template file to begin the game data split. Optionally asks and saves game directory to template if one does not exist.
        /// </summary>
        /// <returns>SplitTemplate file</returns>
        public static Templates.SplitTemplate openTemplateFile(string templateFilePath, bool ignoreGamePath = false)
        {
            Templates.SplitTemplate templateFile;
            var templateFileSerializer = new XmlSerializer(typeof(Templates.SplitTemplate));
            var templateFileStream = File.OpenRead(templateFilePath);
            templateFile = (Templates.SplitTemplate)templateFileSerializer.Deserialize(templateFileStream);
            templateFileStream.Close();
            if (ignoreGamePath)
                return templateFile;
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
                            if (checkFileHashes(templateFile.GameInfo.CheckHashes, checkFileHash) == true)
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
                            if (checkFileHashes(templateFile.GameInfo.CheckHashes, checkFileHash) == true)
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
