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
		public Settings_SALVL SALVL;
		public Settings_SAMDL SAMDL;
		public Settings_SA2EventViewer SA2EventViewer;
        public Settings_TextureEditor TextureEditor;
		public Settings_SA2CutsceneTextEditor SA2CutsceneTextEditor;
		public Settings_SA2MessageFileEditor SA2MessageFileEditor;

		private static string GetSettingsPath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "EditorPreferences.ini");
		}

		public static SettingsFile Load()
		{
			string path = GetSettingsPath();
			if (File.Exists(path))
			{
				return IniSerializer.Deserialize<SettingsFile>(path);
			}
			else
			{
				SettingsFile settings = new SettingsFile
				{
					SALVL = new Settings_SALVL(),
					SAMDL = new Settings_SAMDL(),
					SA2EventViewer = new Settings_SA2EventViewer(),
                    TextureEditor = new Settings_TextureEditor(),
					SA2CutsceneTextEditor = new Settings_SA2CutsceneTextEditor(),
					SA2MessageFileEditor = new Settings_SA2MessageFileEditor()
				};
				return settings;
			}
		}

		public void Save()
		{
			string path = GetSettingsPath();
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			IniSerializer.Serialize(this, path);
		}

		public class Settings_SALVL
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
		}

		public class Settings_SAMDL
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

		}

		public class Settings_SA2EventViewer
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
		}

        public class Settings_TextureEditor
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
        }

		public class Settings_SA2CutsceneTextEditor
		{
			[IniAlwaysInclude]
			public bool BigEndian { get; set; }
			[IniAlwaysInclude]
			public bool UseSJIS { get; set; }
			[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
			public List<string> RecentFiles { get; set; } = new List<string>();

		}

		public class Settings_SA2MessageFileEditor
		{
			[IniAlwaysInclude]
			public bool BigEndian { get; set; }
			[IniAlwaysInclude]
			public bool UseSJIS { get; set; }
			[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
			public List<string> RecentFiles { get; set; } = new List<string>();
		}

	}
}