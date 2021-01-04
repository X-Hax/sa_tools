using System.IO;
using System.Windows.Forms;
using SA_Tools;

namespace SAEditorCommon.ProjectManagement
{
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

		public string SADXPCPath { get; set; }
		public string SA2PCPath { get; set; }

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
}
