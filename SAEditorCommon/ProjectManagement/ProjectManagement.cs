using System;
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
			[XmlAttribute("gameSystemFolder")]
			public string GameSystemFolder { get; set; }
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

		//TODO: Add hash for SADXGC.
		public static Dictionary<string, string> checkFileHashes = new Dictionary<string, string>()
		{
			{ "SA1", "060cad2ceefc07c7429085f30a356046" },
			{ "SA1AD", "fcb1da8942278871136e41e127ce979b" },
			{ "SA2", "1c1b63fcb551187e7c7b456b4e28a022" },
			{ "SA2TT", "f3d6cf600af7d8daf156eee220225379" },
			{ "SADXGC", "" },
			{ "SADX360", "e1f01f48442cf711e2206370c60b7218" },
			{ "SADXPC", "c6d65712475602252bfce53d0d8b7d6f" },
			{ "SA2PC", "4f03dff9b720986cd922ab461d2a6c69" }
		};
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
				string fileName = openFileDialog1.FileName;
				var projFileSerializer = new XmlSerializer(typeof(Templates.ProjectTemplate));
				var projFileStream = File.OpenRead(fileName);
				projectFile = (Templates.ProjectTemplate)projFileSerializer.Deserialize(projFileStream);
				return projectFile;
			}
			else
				return null;
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

			if (templateFile.GameInfo.GameSystemFolder.Length <= 0)
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
							string fileHash = HelperFunctions.FileHash(checkFile);
							string compareHash = Templates.checkFileHashes[templateFile.GameInfo.GameName];
							if (fileHash == compareHash.ToLower())
							{
								var templateFileStreamSave = File.OpenWrite(templateFilePath);
								TextWriter splitsWriter = new StreamWriter(templateFileStreamSave);

								templateFile.GameInfo.GameSystemFolder = fsd.FileName;

								templateFileSerializer.Serialize(splitsWriter, templateFile);
								templateFileStreamSave.Close();

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
			else if (!Directory.Exists(templateFile.GameInfo.GameSystemFolder))
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
							if (HelperFunctions.FileHash(checkFile) == Templates.checkFileHashes[templateFile.GameInfo.GameName])
							{
								var templateFileStreamSave = File.OpenWrite(templateFilePath);
								TextWriter splitsWriter = new StreamWriter(templateFileStreamSave);

								templateFile.GameInfo.GameSystemFolder = fsd.FileName;

								templateFileSerializer.Serialize(splitsWriter, templateFile);
								templateFileStreamSave.Close();

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
	}
}
