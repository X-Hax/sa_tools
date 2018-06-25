using System.IO;
using System.Windows;
using System.Windows.Forms;

using SA_Tools;

namespace ProjectManager
{
    /// <summary>
    /// Holds settings necessary for the project manager.
    /// </summary>
    public class Settings
    {
        public string SADXPCPath { get; set; }
        public string SA2PCPath { get; set; }

        private static string GetSettingsPath()
        {
            return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Settings.ini");
        }

        public static Settings Load()
        {
            if(File.Exists(GetSettingsPath()))
            {
                return (Settings) IniSerializer.Deserialize(typeof(Settings), GetSettingsPath());
            }
            else
            {
                Settings result = new Settings()
                {
                    SA2PCPath = "",
                    SADXPCPath = ""
                };

                return result;
            }
        }

        public void Save()
        {
            IniSerializer.Serialize(this, GetSettingsPath());
        }
    }
}
