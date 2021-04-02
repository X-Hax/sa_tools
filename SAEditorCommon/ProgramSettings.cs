using SA_Tools;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon
{
	public class SettingsFile
	{
		//public string SADXPCPath { get; set; }
		//public string SA2PCPath { get; set; }

		public Settings_SADXLVL2 SADXLVL2;
		public Settings_SALVL SALVL;
		public Settings_SAMDL SAMDL;
		public Settings_SA2EventViewer SA2EventViewer;

		private static string GetSettingsPath()
		{
			return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "EditorPreferences.ini");
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
					//SA2PCPath = "",
					//SADXPCPath = "",
					SADXLVL2 = new Settings_SADXLVL2(),
					SALVL = new Settings_SALVL(),
					SAMDL = new Settings_SAMDL(),
					SA2EventViewer = new Settings_SA2EventViewer()
				};
				return settings;
			}
		}

		public void Save()
		{
			IniSerializer.Serialize(this, GetSettingsPath());
		}

		public class Settings_SADXLVL2
		{
			[DefaultValue(true)]
			public bool ShowWelcomeScreen { get; set; }
			[DefaultValue(false)]
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
			public Settings_SADXLVL2()
			{
				ShowWelcomeScreen = true;
				DisableModelLibrary = false;
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

		public class Settings_SALVL
		{
			[DefaultValue(true)]
			public bool ShowWelcomeScreen { get; set; }
			[DefaultValue(10000.0f)]
			public float DrawDistance_General { get; set; }
			[DefaultValue(1)]
			public int CameraModifier { get; set; }
			public bool AlternativeCamera { get; set; }
			public Settings_SALVL()
			{
				ShowWelcomeScreen = true;
				DrawDistance_General = 10000.0f;
				CameraModifier = 1;
				AlternativeCamera = false;
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
			[DefaultValue(10000.0f)]
			public float DrawDistance_General { get; set; }
			[DefaultValue(1)]
			public int CameraModifier { get; set; }
			public Settings_SA2EventViewer()
			{
				DrawDistance_General = 10000.0f;
				CameraModifier = 1;
			}
		}
	}
}
