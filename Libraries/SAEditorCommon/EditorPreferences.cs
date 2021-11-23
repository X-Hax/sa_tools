using SplitTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;


namespace SAModel.SAEditorCommon
{
    /// <summary>
    /// Editor-specific user settings stored in EditorPreferences.ini
    /// </summary>
    public class SettingsFile
	{
		public static string GetSettingsPath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".ini");
		}

		public void Save()
		{
			string path = GetSettingsPath();
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			IniSerializer.Serialize(this, path);
		}

		public class Settings_SALVL : SettingsFile
		{
			[DefaultValue(true)]
			public bool ShowWelcomeScreen { get; set; }
			[DefaultValue(true)]
			public bool DisableModelLibrary { get; set; }
			[DefaultValue(0)]
			public int LibrarySplitterPosition { get; set; }
			[DefaultValue(0)]
			public int ItemsSplitterPosition { get; set; }
			[DefaultValue(0)]
			public int PropertiesSplitterPosition { get; set; }
			[DefaultValue(10000.0f)]
			public float DrawDistance_General { get; set; }
			[DefaultValue(6000.0f)]
			public float DrawDistance_SET { get; set; }
			[DefaultValue(6000.0f)]
			public float DrawDistance_Geometry { get; set; }
			[DefaultValue(1)]
			public int CameraModifier { get; set; }
			public bool AlternativeCamera { get; set; }
			public bool MouseWrapScreen { get; set; }

			public Settings_SALVL()
			{
				ShowWelcomeScreen = true;
				DisableModelLibrary = true;
				LibrarySplitterPosition = 0;
				ItemsSplitterPosition = 0;
				PropertiesSplitterPosition = 0;
				DrawDistance_General = 10000;
				DrawDistance_SET = 6000;
				DrawDistance_Geometry = 6000;
				CameraModifier = 1;
				AlternativeCamera = false;
				MouseWrapScreen = false;
			}

			public static Settings_SALVL Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SALVL>(path);
				else
					return new Settings_SALVL();
			}
		}

		public class Settings_SAMDL : SettingsFile
		{
			[DefaultValue(true)]
			public bool ShowWelcomeScreen { get; set; }

			[DefaultValue(1.125f)]
			public float CamMoveSpeed { get; set; }

			[DefaultValue(10000.0f)]
			public float DrawDistance { get; set; }

			[DefaultValue(1)]
			public int CameraModifier { get; set; }

			[DefaultValue(false)]
			public bool AlternativeCamera { get; set; }

			[DefaultValue(0.0f)]
			public float BackLightAmbientR { get; set; }

			[DefaultValue(0.0f)]
			public float BackLightAmbientG { get; set; }

			[DefaultValue(0.0f)]
			public float BackLightAmbientB { get; set; }

			public Settings_SAMDL()
			{
				ShowWelcomeScreen = true;
				CamMoveSpeed = 1.125f;
				DrawDistance = 10000.0f;
				CameraModifier = 1;
				AlternativeCamera = false;
				BackLightAmbientR = 0.0f;
				BackLightAmbientG = 0.0f;
				BackLightAmbientB = 0.0f;
			}

			public static Settings_SAMDL Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SAMDL>(path);
				else
					return new Settings_SAMDL();
			}
		}

		public class Settings_SA2EventViewer : SettingsFile
		{
			[DefaultValue(55000.0f)]
			public float DrawDistance_General { get; set; }
			[DefaultValue(1)]
			public int CameraModifier { get; set; }

			public Settings_SA2EventViewer()
			{
				DrawDistance_General = 55000.0f;
				CameraModifier = 1;
			}

			public static Settings_SA2EventViewer Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SA2EventViewer>(path);
				else
					return new Settings_SA2EventViewer();
			}
		}

        public class Settings_TextureEditor : SettingsFile
		{
            [DefaultValue(false)]
            public bool HighQualityGVM { get; set; }
            [DefaultValue(true)]
            public bool SACompatiblePalettes { get; set; }
            [DefaultValue(true)]
            public bool EnableFiltering { get; set; }

            public Settings_TextureEditor()
            {
                HighQualityGVM = false;
                SACompatiblePalettes = true;
                EnableFiltering = true;
            }

			public static Settings_TextureEditor Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_TextureEditor>(path);
				else
					return new Settings_TextureEditor();
			}
		}

		public class Settings_SA2CutsceneTextEditor : SettingsFile
		{
			[DefaultValue(false)]
			public bool BigEndian { get; set; }
			[DefaultValue(false)]
			public bool UseSJIS { get; set; }
			[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
			public List<string> RecentFiles { get; set; } = new List<string>();

			public Settings_SA2CutsceneTextEditor()
			{
				BigEndian = false;
				UseSJIS = false;
				RecentFiles = new List<string>();
			}

			public static Settings_SA2CutsceneTextEditor Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SA2CutsceneTextEditor>(path);
				else
					return new Settings_SA2CutsceneTextEditor();
			}
		}

		public class Settings_SA2MessageFileEditor : SettingsFile
		{
			[DefaultValue(false)]
			public bool BigEndian { get; set; }
			[DefaultValue(false)]
			public bool UseSJIS { get; set; }
			[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
			public List<string> RecentFiles { get; set; } = new List<string>();

			public Settings_SA2MessageFileEditor()
			{
				BigEndian = false;
				UseSJIS = false;
				RecentFiles = new List<string>();
			}

			public static Settings_SA2MessageFileEditor Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SA2MessageFileEditor>(path);
				else
					return new Settings_SA2MessageFileEditor();
			}
		}
	}
}