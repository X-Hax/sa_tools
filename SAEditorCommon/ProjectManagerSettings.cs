using System.IO;
using System.Windows;
using System.Windows.Forms;

using SA_Tools;

namespace ProjectManager
{
    /// <summary>
    /// Holds settings necessary for the project manager.
    /// </summary>
    public class ProjectManagerSettings
    {
        public string SADXPCPath { get; set; }
        public string SA2PCPath { get; set; }

        private static string GetSettingsPath()
        {
            return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Settings.ini");
        }

        public static ProjectManagerSettings Load(string iniPath)
        {
            if (File.Exists(iniPath))
            {
                return (ProjectManagerSettings)IniSerializer.Deserialize(typeof(ProjectManagerSettings), iniPath);
            }
            else
            {
                ProjectManagerSettings result = new ProjectManagerSettings()
                {
                    SA2PCPath = "",
                    SADXPCPath = ""
                };

                return result;
            }
        }

        public static ProjectManagerSettings Load()
        {
            return Load(GetSettingsPath());
        }

        public void Save()
        {
            IniSerializer.Serialize(this, GetSettingsPath());
        }
    }
}
