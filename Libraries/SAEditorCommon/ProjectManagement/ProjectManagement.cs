﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using SplitTools;
using SplitTools.SAArc;
using SplitTools.Split;

namespace SAModel.SAEditorCommon.ProjectManagement
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
			/// <summary>
			/// Cutscene archive entries, only in SA2 formatted templates.
			/// </summary>
			[XmlElement("SplitEntryEvent", typeof(SplitEntryEvent))]
			public List<SplitEntryEvent> SplitEventEntries { get; set; }
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
			/// <summary>
			/// Cutscene archive entries, only in SA2 formatted templates.
			/// </summary>
			[XmlElement("SplitEntryEvent", typeof(SplitEntryEvent))]
			public List<SplitEntryEvent> SplitEventEntries { get; set; }
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
			/// A string containing a range to generate the MD5 from (e.g. 00000FFE-034F78D0 in the EXE)
			/// </summary>
			[XmlAttribute("checkRange")]
			public string CheckRange { get; set; }
			/// <summary>
			/// Classification of the project type for use with certain functions. Null if it's the "main" project template.
			/// </summary>
			[XmlAttribute("projectType")]
			public string ProjectType { get; set; }
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
			/// Classification of the project type for use with certain functions. Null if it's the "main" project template.
			/// </summary>
			[XmlAttribute("projectType")]
			public string ProjectType { get; set; }
		}
		/// <summary>
		/// Intended for use with SAMDL Project Mode
		/// </summary>
		public abstract class EntryType {}
		/// <summary>
		/// Stores names for the source file, data file, and a common name for processing data to be split.
		/// </summary>
		public class SplitEntry : EntryType
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
		public class SplitEntryMDL : EntryType
		{
			/// <summary>
			/// Model Archive filename.
			/// </summary>
			[XmlAttribute("ModelFile")]
			public string ModelFile { get; set; }
			/// <summary>
			/// Common name to be referenced for the general contents of what will be split.
			/// </summary>
			[XmlAttribute("CmnName")]
			public string CmnName { get; set; }
			/// <summary>
			/// Model labeling file to be used with the Model file.
			/// </summary>
			[XmlAttribute("LabelFile")]
			public string LabelFile { get; set; }
			/// <summary>
			/// Animation labeling file to be used with the Motion file.
			/// </summary>
			[XmlAttribute("MTNLabelFile")]
			public string MTNLabelFile { get; set; }
			/// <summary>
			/// List of Motion files uses by the Model File.
			/// </summary>
			[XmlElement("MotionFile")]
			public List<string> MotionFiles { get; set; }
		}

		/// <summary>
		/// Stores information on SA2 cutscene archives for splitting.
		/// </summary>
		public class SplitEntryEvent : EntryType
		{
			/// <summary>
			/// Cutscene Archive filename.
			/// </summary>
			[XmlAttribute("EventFile")]
			public string EventFile { get; set; }
			/// <summary>
			/// Cutscene Archive Type. Used to determine split parameters
			/// </summary>
			[XmlAttribute("EventType")]
			public string EventType { get; set; }
			/// <summary>
			/// Common name to be referenced for the event.
			/// </summary>
			[XmlAttribute("CmnName")]
			public string CmnName { get; set; }
			/// <summary>
			/// Model labeling file to be used with the Event file.
			/// </summary>
			[XmlAttribute("LabelFile")]
			public string LabelFile { get; set; }
		}
	}

	public class SAToolsHubSettings
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
		[DefaultValue(false)]
		public bool DisableX86Warning { get; set; } = false;

		[DefaultValue(0)] public long UpdateTime { get; set; }

		public static SAToolsHubSettings Load(string iniPath)
		{
			if (File.Exists(iniPath))
			{
				return (SAToolsHubSettings)IniSerializer.Deserialize(typeof(SAToolsHubSettings), iniPath);
			}
			else
			{
				SAToolsHubSettings result = new SAToolsHubSettings()
				{
					UpdateCheck = false,
					UpdateUnit = UpdateUnits.Weeks,
					HubTheme = Themes.light,
				};

				return result;
			}
		}
		public static SAToolsHubSettings Load()
		{
			return Load(GetSettingsPath());
		}
		private static string GetSettingsPath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SAToolsHub.ini");
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
				// Check if project data folder path is relative
				if (!projectFile.GameInfo.ProjectFolder.Contains(":"))
					projectFile.GameInfo.ProjectFolder = Path.Combine(Path.GetDirectoryName(fileName), projectFile.GameInfo.ProjectFolder);
				projFileStream.Close();
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
					var fsd = new FolderBrowserDialog
					{ Description = "Please select path for " + templateFile.GameInfo.GameName, UseDescriptionForTitle = true };
					fsd.ShowDialog();
					if (Directory.Exists(fsd.SelectedPath))
					{
						string checkFile = Path.Combine(fsd.SelectedPath, templateFile.GameInfo.CheckFile);
						int rangeStart = 0;
						int rangeFinish = 0;
						if (templateFile.GameInfo.CheckRange != null && templateFile.GameInfo.CheckRange != "")
						{
							string[] spl = templateFile.GameInfo.CheckRange.Split('-');
							rangeStart = int.Parse(spl[0], System.Globalization.NumberStyles.HexNumber);
							rangeFinish = int.Parse(spl[1], System.Globalization.NumberStyles.HexNumber);
						}
						if (File.Exists(checkFile))
						{
							string checkFileHash = HelperFunctions.FileHash(checkFile, rangeStart, rangeFinish);
							if (checkFileHashes(templateFile.GameInfo.CheckHashes, checkFileHash) == true)
							{
								SetGamePath(templateFile.GameInfo.GameName, fsd.SelectedPath);
								return templateFile;
							}
							else
							{
								DialogResult pathWarning = WrongMD5SumDialog(checkFileHash, checkFile, templateFile.GameInfo.GameName);
								switch (pathWarning)
								{
									case DialogResult.No:
										goto FolderSelectionNew;
									case DialogResult.Yes:
										SetGamePath(templateFile.GameInfo.GameName, fsd.SelectedPath);
										return templateFile;
									case DialogResult.Cancel:
									default:
										return null;
								}
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
					var fsd = new FolderBrowserDialog
					{ Description = "Please select path for " + templateFile.GameInfo.GameName, UseDescriptionForTitle = true };
					fsd.ShowDialog();
					if (Directory.Exists(fsd.SelectedPath))
					{
						string checkFile = Path.Combine(fsd.SelectedPath, templateFile.GameInfo.CheckFile);
						if (File.Exists(checkFile))
						{
							string checkFileHash = HelperFunctions.FileHash(checkFile);
							if (checkFileHashes(templateFile.GameInfo.CheckHashes, checkFileHash) == true)
							{
								SetGamePath(templateFile.GameInfo.GameName, fsd.SelectedPath);
								return templateFile;
							}
							else
							{
								DialogResult pathWarning = WrongMD5SumDialog(checkFileHash, checkFile, templateFile.GameInfo.GameName);
								switch (pathWarning)
								{
									case DialogResult.Yes:
										SetGamePath(templateFile.GameInfo.GameName, fsd.SelectedPath);
										return templateFile;
									case DialogResult.No:
										goto FolderSelectionMissing;
									case DialogResult.Cancel:
									default:
										return null;
								}
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
			string gamePathsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "GamePaths.ini");
			if (!File.Exists(gamePathsFile))
				return "";
			Dictionary<string, string> gamePathsList = IniSerializer.Deserialize<Dictionary<string, string>>(gamePathsFile);
			return gamePathsList.ContainsKey(gameName) ? gamePathsList[gameName] : "";
		}

		/// <summary>
		/// Sets the path for a specified game in GamePaths.ini.
		/// </summary>
		public static void SetGamePath(string gameName, string gamePath)
		{
			Dictionary<string, string> gamePathsList;
			string appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools");
			string gamePathsFilePath = Path.Combine(appdata, "GamePaths.ini");
			if (File.Exists(gamePathsFilePath))
				gamePathsList = IniSerializer.Deserialize<Dictionary<string, string>>(gamePathsFilePath);
			else
			{
				if (!Directory.Exists(appdata))
					Directory.CreateDirectory(appdata);
				gamePathsList = new Dictionary<string, string>();
			}
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

		/// <summary>
		/// Splits data from a SplitEntry.
		/// </summary>
		public static void SplitTemplateEntry(Templates.SplitEntry splitData, SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string iniFolder, string outputFolder, bool overwrite = true, bool nometa = false, bool nolabel = false)
		{
			string datafilename;
			switch (splitData.SourceFile)
			{
				case ("system/chrmodels.dll"):
					if (File.Exists(Path.Combine(gameFolder, "system/chrmodels_orig.dll")))
						datafilename = Path.Combine(gameFolder, "system/chrmodels_orig.dll");
					else
						datafilename = Path.Combine(gameFolder, splitData.SourceFile);
					break;
				case ("Data_DLL.dll"):
					if (File.Exists(Path.Combine(gameFolder, "resource/gd_PC/DLL/Win32/Data_DLL_orig.dll")))
						datafilename = Path.Combine(gameFolder, "resource/gd_PC/DLL/Win32/Data_DLL_orig.dll");
					else
						datafilename = Path.Combine(gameFolder, "resource/gd_PC/DLL/Win32/Data_DLL.dll");
					break;
				default:
					datafilename = Path.Combine(gameFolder, splitData.SourceFile);
					break;
			}
			string inifilename = Path.Combine(iniFolder, (splitData.IniFile.ToLower() + ".ini"));
			string projectFolderName = (outputFolder + "\\");

			string splitItem;

			if (splitData.CmnName != null)
				splitItem = splitData.CmnName;
			else
				splitItem = splitData.IniFile;

			if (progress != null)
			{
				progress.StepProgress();
				progress.SetStep("Splitting " + splitItem + " from " + splitData.SourceFile);
			}

			#region Validating Inputs
			if (!File.Exists(datafilename))
			{
				MessageBox.Show((datafilename + " is missing.\n\nPress OK to abort."), "Split Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				throw new Exception(SplitTools.Split.SplitERRORVALUE.NoSourceFile.ToString());
				//return (int)ERRORVALUE.NoSourceFile;
			}

			if (!File.Exists(inifilename))
			{
				MessageBox.Show((inifilename + " is missing.\n\nPress OK to abort."), "Split Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				throw new Exception(SplitTools.Split.SplitERRORVALUE.NoDataMapping.ToString());
				//return (int)ERRORVALUE.NoDataMapping;
			}
			#endregion

			// switch on file extension - if dll, use dll splitter
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(datafilename);
			string ext = fileInfo.Extension;

			switch (ext.ToLower())
			{
				case ".dll":
					SplitTools.SplitDLL.SplitDLL.SplitDLLFile(datafilename, inifilename, projectFolderName, nometa, nolabel, overwrite, true);
					break;
				case ".nb":
					SplitTools.Split.SplitNB.SplitNBFile(datafilename, false, projectFolderName, 0, inifilename, overwrite);
					break;
				default:
					SplitTools.Split.SplitBinary.SplitFile(datafilename, inifilename, projectFolderName, nometa, nolabel, overwrite, true);
					break;
			}
		}

		/// <summary>
		/// Splits data from a SplitEntryMDL.
		/// </summary>
		public static void SplitTemplateMDLEntry(Templates.SplitEntryMDL splitMDL, SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string outputFolder, string iniFolder, bool overwrite = true)
		{
			string filePath = Path.Combine(gameFolder, splitMDL.ModelFile);

			string mdllabelfile = Path.Combine(Path.Combine(iniFolder, "MDL"), (splitMDL.LabelFile.ToLower() + ".ini"));

			string mtnlabelfile = Path.Combine(Path.Combine(iniFolder, "MDL"), (splitMDL.MTNLabelFile.ToLower() + ".ini"));

			string fileOutputFolder = Path.Combine(outputFolder, "figure\\bin");

			if (progress != null)
			{
				progress.StepProgress();

				if (splitMDL.CmnName != null)
					progress.SetStep("Splitting " + splitMDL.CmnName + " from " + splitMDL.ModelFile);
				else
					progress.SetStep("Splitting data from " + splitMDL.ModelFile);
			}

			#region Validating Inputs
			if (!File.Exists(filePath))
			{
				MessageBox.Show((filePath + " is missing.\n\nPress OK to abort."), "Split Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				throw new Exception(SplitERRORVALUE.NoSourceFile.ToString());
				//return (int)ERRORVALUE.NoSourceFile;
			}
			#endregion

			if (overwrite)
				sa2MDL.Split(filePath, fileOutputFolder, splitMDL.MotionFiles.ToArray(), mdllabelfile, mtnlabelfile);
		}

		/// <summary>
		/// Splits data from a SplitEntryEvent.
		/// </summary>
		public static void SplitTemplateEventEntry(Templates.SplitEntryEvent splitEvent, SAModel.SAEditorCommon.UI.ProgressDialog progress, string gameFolder, string outputFolder, string iniFolder, bool overwrite = true)
		{
			string mainext;
			if (splitEvent.EventType == "MainBIN")
				mainext = ".bin";
			else
				mainext = ".prs";
			string filePath = Path.Combine(gameFolder, splitEvent.EventFile + $"{mainext}");
			string labelfile = Path.Combine(Path.Combine(iniFolder, "event"), (splitEvent.LabelFile.ToLower() + ".ini"));
			List<string> filePathEXArr = new List<string>();
			string filePathTex = Path.Combine(gameFolder, splitEvent.EventFile + "texlist.prs");
			string ext = null;
			switch (splitEvent.EventType)
			{
				case "Trial":
					for (int l = 0; l < 2; l++)
					{
						ext = l.ToString();
						string filePathEX = Path.Combine(gameFolder, splitEvent.EventFile + $"_{ext}.scr");
						filePathEXArr.Add(filePathEX);
					}
					break;
				case "Preview":
					for (int l = 0; l < 5; l++)
					{
						ext = l.ToString();
						string filePathEX = Path.Combine(gameFolder, splitEvent.EventFile + $"_{ext}.prs");
						filePathEXArr.Add(filePathEX);
					}
					break;
				case "MiniEvent":
					for (int l = 0; l < 5; l++)
					{
						ext = l.ToString();
						string filePathEX = Path.Combine(gameFolder, splitEvent.EventFile + $"_{ext}.scr");
						filePathEXArr.Add(filePathEX);
					}
					break;
				case "MiniEventPC":
					for (int l = 0; l < 6; l++)
					{
						ext = l.ToString();
						string filePathEX = Path.Combine(gameFolder, splitEvent.EventFile + $"_{ext}.scr");
						filePathEXArr.Add(filePathEX);
					}
					break;
				case "MainBIN":
				case "MainPRS":
					for (int l = 0; l < 6; l++)
					{ 
							switch (l)
							{
								case 0:
								case 1:
								case 2:
								case 3:
								case 4:
									ext = l.ToString();
									break;
								case 5:
									ext = "j";
									break;
							}
						string filePathEX = Path.Combine(gameFolder, splitEvent.EventFile + $"_{ext}.prs");
						filePathEXArr.Add(filePathEX);
					}
					break;
				case "MainPC":
					for (int l = 0; l < 7; l++)
					{
						switch (l)
						{
							case 0:
							case 1:
							case 2:
							case 3:
							case 4:
							case 5:
								ext = l.ToString();
								break;
							case 6:
								ext = "j";
								break;
						}
						string filePathEX = Path.Combine(gameFolder, splitEvent.EventFile + $"_{ext}.prs");
						filePathEXArr.Add(filePathEX);
					}
					break;
			}
			string fileOutputFolder = Path.Combine(outputFolder, "Event\\bin");

			if (progress != null)
			{
				progress.StepProgress();

				if (splitEvent.CmnName != null)
					progress.SetStep("Splitting " + splitEvent.CmnName + " from " + splitEvent.EventFile);
				else
					progress.SetStep("Splitting data from " + splitEvent.EventFile);
			}

			#region Validating Inputs
			if (!File.Exists(filePath))
			{
				MessageBox.Show((filePath + " is missing.\n\nPress OK to abort."), "Split Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				throw new Exception(SplitERRORVALUE.NoSourceFile.ToString());
				//return (int)ERRORVALUE.NoSourceFile;
			}
			#endregion
			if (overwrite)
			{
				switch (splitEvent.EventType)
				{
					case "MiniEvent":
					case "MiniEventPC":
						SA2MiniEvent.Split(filePath, fileOutputFolder, labelfile);
						foreach (string ex in filePathEXArr)
							sa2EventExtra.SplitMini(ex, fileOutputFolder);
						break;
					case "Trial":
					case "Preview":
						sa2Event.Split(filePath, fileOutputFolder, labelfile);
						foreach (string ex in filePathEXArr)
							sa2EventExtra.Split(ex, fileOutputFolder);
						break;
					case "MainPRS":
					case "MainPC":
						sa2Event.Split(filePath, fileOutputFolder, labelfile);
						foreach (string ex in filePathEXArr)
							sa2EventExtra.Split(ex, fileOutputFolder);
						sa2Event.SplitExternalTexlist(filePathTex, fileOutputFolder);
						break;
					case "MainBIN":
						sa2Event.Split(filePath, fileOutputFolder, labelfile);
						break;
				}
			}
		}

		/// <summary>
		/// Shows a dialog when a file specified in the template has an incorrect MD5 hash.
		/// </summary>
		private static DialogResult WrongMD5SumDialog(string checkFileHash, string checkFileName, string checkGameName)
		{
			string appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools");
			if (!Directory.Exists(appdata))
				Directory.CreateDirectory(appdata);
			File.WriteAllText(Path.Combine(appdata, Path.GetFileNameWithoutExtension(checkFileName) + "_md5.txt"), checkFileHash);
			string checkErrorMessage = "The file " + checkFileName +
				" does not match the record for the template " + checkGameName + ".\n\n" +
				"Trying to rip an incompatible version of the game may lead to crashes or missing data. " +
				"Make sure you are using the correct version of the game and try again.\n\n" +
				"Would you like to try to use this path anyway?\nSelect No to pick a different path, Cancel to exit.\n\n" +
				"MD5: " + checkFileHash;
			return MessageBox.Show(checkErrorMessage, "Incorrect Game Version", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
		}
	}
}