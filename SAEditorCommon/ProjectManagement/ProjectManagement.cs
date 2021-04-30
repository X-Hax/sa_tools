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
		public static Templates.ProjectTemplate opeProjectFileString(string fileName)
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
		//public static Templates.SplitTemplate openTemplateFile(string templateSplit)
		//{
		//	var templateFileSerializer = new XmlSerializer(typeof(Templates.SplitTemplate));
		//	var templateFileStream = File.OpenRead(templateSplit);
		//	var templateFile = (Templates.SplitTemplate)templateFileSerializer.Deserialize(templateFileStream);

		//	gameName = templateFile.GameInfo.GameName;
		//	gamePath = templateFile.GameInfo.GameSystemFolder;
		//	dataFolder = templateFile.GameInfo.DataFolder;
		//	splitEntries = templateFile.SplitEntries;
		//	splitMdlEntries = templateFile.SplitMDLEntries;

		//	templateFileStream.Close();

		//	if (gamePath.Length <= 0)
		//	{
		//		DialogResult gamePathWarning = MessageBox.Show(("A game path has not been supplied for this template.\n\nPlease press OK to select the game path for " + gameName + "."), "Game Path Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		//		if (gamePathWarning == DialogResult.OK)
		//		{
		//			SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "", Filter = "", FileName = "texturepack" };
		//			if (dlg.FileName && folderResult.Value)
		//			{
		//				gamePath = folderDialog.SelectedPath;

		//				var templateFileStreamSave = File.OpenWrite(templateSplit);
		//				TextWriter splitsWriter = new StreamWriter(templateFileStreamSave);

		//				templateFile.GameInfo.GameSystemFolder = gamePath;

		//				templateFileSerializer.Serialize(splitsWriter, templateFile);
		//				templateFileStreamSave.Close();
		//			}
		//			else
		//			{
		//				DialogResult pathWarning = MessageBox.Show(("No path was supplied."), "No Path Supplied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		//				if (pathWarning == DialogResult.OK)
		//				{
		//					comboBox1.SelectedIndex = -1;
		//				}
		//			}
		//		}
		//	}
		//	else if (!Directory.Exists(gamePath))
		//	{
		//		DialogResult gamePathWarning = MessageBox.Show(("The folder for " + gameName + " does not exist.\n\nPlease press OK and select the correct path for " + gameName + "."), "Game Path Does Not Exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		//		if (gamePathWarning == DialogResult.OK)
		//		{
		//			var folderDialog = new VistaFolderBrowserDialog();
		//			var folderResult = folderDialog.ShowDialog();
		//			if (folderResult.HasValue && folderResult.Value)
		//			{
		//				gamePath = folderDialog.SelectedPath;

		//				var templateFileStreamSave = File.OpenWrite(templateSplit);
		//				TextWriter splitsWriter = new StreamWriter(templateFileStreamSave);

		//				templateFile.GameInfo.GameSystemFolder = gamePath;

		//				templateFileSerializer.Serialize(splitsWriter, templateFile);
		//				templateFileStreamSave.Close();
		//			}
		//			else
		//			{
		//				DialogResult pathWarning = MessageBox.Show(("No path was supplied."), "No Path Supplied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		//				if (pathWarning == DialogResult.OK)
		//				{
		//					comboBox1.SelectedIndex = -1;
		//				}
		//			}
		//		}
		//	}
		//}
	}
}
