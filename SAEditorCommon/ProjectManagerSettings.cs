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

        public string GetModPathForGame(SA_Tools.Game game)
        {
            switch (game)
            {
                case Game.SA1:
                case Game.SA2:
                    throw new System.NotSupportedException();
                    return "";

                case Game.SADX:
                    return  Path.Combine(SADXPCPath, "mods");
                
                case Game.SA2B:
                    return Path.Combine(SA2PCPath, "mods");

                default:
                    throw new System.NotSupportedException();
                    return "";
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
